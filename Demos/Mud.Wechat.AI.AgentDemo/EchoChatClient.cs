// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace Mud.Wechat.AI.AgentDemo;

/// <summary>
/// 离线模型替身（<see cref="IChatClient"/>）：让 Agent 管线在没有真实模型凭据时也能整条跑通。
/// </summary>
/// <remarks>
/// <para>
/// <b>它演示的是什么、不是什么</b>：回合管线（会话读写 → 历史裁剪 → 工具下发 → 工具结果回填 → 主动回复）
/// 与模型无关，替身只顶掉「生成文本」这一步；因此本 Demo 离线验的是<b>装配与链路</b>，不是智能。
/// 换真实模型只需把 <c>AddSingleton&lt;IChatClient, EchoChatClient&gt;()</c> 换成 <c>AddChatClient(...)</c>，
/// 其余装配一行不改 —— 这正是本 Demo 想证明的产品事实。
/// </para>
/// <para>
/// <b>工具循环自发触发</b>：MAF 的 <c>ChatClientAgent</c> 在内层客户端不含函数调用设施时自动插入
/// <c>FunctionInvokingChatClient</c>，故本替身只要回一条 <see cref="FunctionCallContent"/>，
/// 工具就会真的被执行、结果会真的回填进下一轮请求。触发条件是三重与门
/// （问题含「部门」∧ 本轮确有该工具 ∧ 还没执行过工具），第三重是防自旋的硬约束
/// —— 少了它，替身会对同一条消息反复发起调用，直到 <c>WechatAi:MaxToolCallsPerTurn</c> 才收口。
/// </para>
/// <para>
/// <b>同步与流式两条分支共用同一构造逻辑</b>：回合走哪条由中立管线决定（本 Demo 两种都验过），
/// 若把工具触发只写在一条分支上，另一条分支就退化成「不回显工具」的假离线。
/// </para>
/// </remarks>
internal sealed class EchoChatClient : IChatClient
{
    /// <summary>触发工具调用的关键词（与 Demo 的问题文本同源，刻意不用英文以便 curl 直接中文提问）。</summary>
    private const string ToolTrigger = "部门";

    /// <summary>本替身使用的工具名（与生成物契约名逐字一致）。</summary>
    private const string DepartmentGetTool = "contacts.department.get";

    /// <summary>工具入参名（与 golden schema 的属性名一致）。</summary>
    private const string DepartmentIdArgument = "department_id";

    /// <inheritdoc />
    public ChatClientMetadata Metadata { get; } =
        new("echo", new Uri("https://demo.invalid/echo-chat-client"), "echo-demo-1");

    /// <inheritdoc />
    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new ChatResponse(BuildMessage(Materialize(messages), options)));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(Materialize(messages), options);

        // 函数调用整体下发（不切文本）：拆开发送会让 FICC 收到半截调用。
        if (message.Contents.Any(static content => content is FunctionCallContent))
        {
            yield return new ChatResponseUpdate(ChatRole.Assistant, message.Contents);
            yield break;
        }

        // 文本故意分两段下发：证明流式分支真被管线消费（TurnOutcome.Streamed 由此而来），
        // 而不是「同步应答 + 一次性转流」的假流式。
        foreach (var piece in Split(message.Text))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new ChatResponseUpdate(ChatRole.Assistant, piece);
            await Task.Yield();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // 替身无非托管资源：IChatClient 派生 IDisposable，方法体必须存在但什么都不做。
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (serviceKey is not null)
        {
            return null;
        }

        return serviceType.IsInstanceOfType(this) ? this : serviceType == typeof(ChatClientMetadata) ? Metadata : null;
    }

    /// <summary>物化消息序列（上游可能传入惰性序列，多次枚举会重复消费）。</summary>
    private static IList<ChatMessage> Materialize(IEnumerable<ChatMessage> messages)
        => messages as IList<ChatMessage> ?? messages.ToList();

    /// <summary>构造一条应答消息：要么发起一次工具调用，要么回显「本轮看到了什么」。</summary>
    private static ChatMessage BuildMessage(IList<ChatMessage> messages, ChatOptions? options)
    {
        var question = LastUserText(messages);
        var toolResults = messages.Sum(static message =>
            message.Contents.Count(static content => content is FunctionResultContent));
        var toolOffered = options?.Tools?.Any(static tool =>
            tool is AIFunction function && string.Equals(function.Name, DepartmentGetTool, StringComparison.Ordinal)) == true;

        if (toolResults == 0 && toolOffered && question?.Contains(ToolTrigger, StringComparison.Ordinal) == true)
        {
            var arguments = new Dictionary<string, object?> { [DepartmentIdArgument] = 1 };
            return new ChatMessage(ChatRole.Assistant, new AIContent[]
            {
                new FunctionCallContent(Guid.NewGuid().ToString("N"), DepartmentGetTool, arguments),
            });
        }

        var text = toolResults > 0
            ? $"已消费 {toolResults} 条工具结果后作答（离线替身；原始问题：{question}）"
            : $"离线替身应答（本轮消息 {messages.Count} 条、可见工具 {options?.Tools?.Count ?? 0} 个）：{question}";

        return new ChatMessage(ChatRole.Assistant, text);
    }

    /// <summary>取最后一条 user 文本（历史里的最早消息不参与应答构造）。</summary>
    private static string? LastUserText(IList<ChatMessage> messages)
    {
        for (var i = messages.Count - 1; i >= 0; i--)
        {
            if (messages[i].Role == ChatRole.User && !string.IsNullOrWhiteSpace(messages[i].Text))
            {
                return messages[i].Text;
            }
        }

        return null;
    }

    private static IEnumerable<string> Split(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        var half = text.Length / 2;
        yield return text.Substring(0, half);
        yield return text.Substring(half);
    }
}
