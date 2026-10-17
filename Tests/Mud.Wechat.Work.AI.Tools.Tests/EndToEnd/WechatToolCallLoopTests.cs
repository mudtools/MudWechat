using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.AI.Abstractions.Conversations;
using Mud.AI.Agent.Events;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Extensions;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.EndToEnd;

/// <summary>
/// 端到端闭环（伪模型）：<b>模型返回 tool_call → 工具经执行链落地 SDK → 结果回填 → 第二轮生成回复</b>。
/// </summary>
/// <remarks>
/// <para>
/// 这条用例覆盖的正是装配边界：`AddWechatWorkTools`（工具面 + 白名单投影）与 `AddWechatWorkAgent`（Agent）
/// 各自独立注册，只有在 DI 里相遇才证明「工具真的进了模型视野并能被调用」——
/// 单看任一侧的单元测试都发现不了「白名单投影了但 Agent 没接」这类断链。
/// </para>
/// <para>
/// 工具调用循环由 MAF 的 <c>FunctionInvokingChatClient</c> 承担（<c>ChatClientAgent</c> 在输入客户端不含该设施时自动插入），
/// 本用例因此用「伪模型按轮次返回」验证循环确实发生（第二轮请求里出现 <see cref="FunctionResultContent"/>）。
/// </para>
/// </remarks>
public class WechatToolCallLoopTests
{
    [Fact]
    public async Task RunTurnAsync_WhenModelCallsTool_ShouldExecuteToolAndFeedResultBack()
    {
        var requests = new List<IReadOnlyList<ChatMessage>>();
        var round = 0;
        var chatClient = new Mock<IChatClient>(MockBehavior.Loose);
        chatClient
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<ChatMessage> messages, ChatOptions? _, CancellationToken _) =>
            {
                requests.Add(messages.ToList());
                return Interlocked.Increment(ref round) == 1
                    ? new ChatResponse(new ChatMessage(ChatRole.Assistant,
                    [
                        new FunctionCallContent(
                            "call-1",
                            WechatToolNames.ContactsDepartmentGet,
                            new Dictionary<string, object?> { ["department_id"] = 2 }),
                    ]))
                    : new ChatResponse(new ChatMessage(ChatRole.Assistant, "该部门是研发部。"));
            });

        var departments = new Mock<IWechatWorkDepartmentsService>(MockBehavior.Loose);
        departments
            .Setup(d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse
            {
                ErrorCode = 0,
                Department = new DepartmentInfo { Id = 2, Name = "研发部", ParentId = 1 },
            });

        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Allow());

        using var provider = BuildProvider(chatClient.Object, departments.Object, authorizer.Object);
        var agent = provider.GetRequiredService<WechatAgent>();
        var sink = new Mock<IConversationReplySink>(MockBehavior.Loose);

        var outcome = await agent.RunTurnAsync(Request(), sink.Object);

        outcome.ResponseText.Should().Be("该部门是研发部。");

        requests.Should().HaveCount(2, "第一轮返回 tool_call，第二轮携带工具结果再生成");
        var toolResults = requests[1]
            .SelectMany(static m => m.Contents)
            .OfType<FunctionResultContent>()
            .ToArray();
        toolResults.Should().ContainSingle();
        toolResults[0].Result!.ToString().Should().Contain("研发部", "工具结果必须原样回填给模型");

        departments.Verify(d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()), Times.Once);
        authorizer.Verify(
            a => a.AuthorizeAsync(
                It.Is<WechatToolAuthorizationContext>(ctx => ctx.ToolName == WechatToolNames.ContactsDepartmentGet),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static ServiceProvider BuildProvider(
        IChatClient chatClient,
        IWechatWorkDepartmentsService departments,
        IWechatToolAuthorizer authorizer)
    {
        var switcher = new Mock<IWechatAppContextSwitcher>(MockBehavior.Loose);
        switcher.Setup(s => s.UseAppScope(It.IsAny<string>())).Returns(new FakeScope());
        switcher.Setup(s => s.UseDefaultAppScope()).Returns(new FakeScope());

        var services = new ServiceCollection()
            .AddSingleton(chatClient)
            .AddSingleton(departments)
            .AddSingleton(authorizer)
            .AddSingleton(switcher.Object);

        return services
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"))
            .AddWechatWorkAgent(options => options.SummaryThreshold = 0)
            .BuildServiceProvider();
    }

    private static ConversationRequest Request()
        => new("corp-app", ConversationScope.Group(), "chat-1", "u1", "m1", "研发部是哪个部门？", "chat-1");

    private sealed class FakeScope : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
