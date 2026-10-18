using FluentAssertions;
using Microsoft.Extensions.Options;
using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Configuration;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Agents;

/// <summary>
/// 回合事实装配器：时间锚点（本地 / UTC / Unix 三形态）、身份末位标识、群聊提示。
/// </summary>
/// <remarks>
/// 这些事实是「模型没有时钟也不该猜」的唯一解法。用例同时守两条边界：<b>完整租户 ID 不外泄</b>、
/// <b>群聊必须提示发言者可能不是可操作对象</b>（写工具的误伤主要发生在群里）。
/// </remarks>
public class WechatTurnContextAssemblerTests
{
    [Fact]
    public async Task Assemble_ShouldInjectLocalUtcAndUnixTime()
    {
        var assembler = Create(new DateTimeOffset(2026, 10, 18, 7, 30, 0, TimeSpan.Zero));

        var text = await assembler.AssembleAsync(Request(), CancellationToken.None);

        text.Should().NotBeNull();
        text.Should().Contain("2026-10-18 15:30:00", "本地时间按配置时区（默认 Asia/Shanghai）");
        text.Should().Contain("UTC 2026-10-18 07:30:00");
        text.Should().Contain("Unix 1792308600");
        text.Should().Contain("星期日");
    }

    [Fact]
    public async Task Assemble_ShouldOnlyExposeTenantIdSuffixes()
    {
        var assembler = Create(new DateTimeOffset(2026, 10, 18, 7, 30, 0, TimeSpan.Zero));

        var text = await assembler.AssembleAsync(Request(), CancellationToken.None);

        text.Should().Contain("应用：…-app", "应用键只给末 4 位");
        text.Should().Contain("企业：…rp-1");
        text.Should().NotContain("corp-app", "完整应用键可在模型输出与日志里被复制出去");
        text.Should().NotContain("corp-1");
    }

    [Fact]
    public async Task Assemble_ForGroupChat_ShouldWarnAboutSpeakerIdentity()
    {
        var assembler = Create(new DateTimeOffset(2026, 10, 18, 7, 30, 0, TimeSpan.Zero));

        var text = await assembler.AssembleAsync(Request(), CancellationToken.None);

        text.Should().Contain("群聊");
        text.Should().Contain("发言者可能不是可操作对象");
    }

    [Fact]
    public async Task Assemble_ForSingleChat_ShouldMarkSingle()
    {
        var assembler = Create(new DateTimeOffset(2026, 10, 18, 7, 30, 0, TimeSpan.Zero));
        var request = new ConversationRequest(
            "corp-app",
            ConversationScope.P2P(),
            "u1",
            "u1",
            "m1",
            "hi",
            "u1");

        var text = await assembler.AssembleAsync(request, CancellationToken.None);

        text.Should().Contain("单聊");
    }

    [Fact]
    public void Options_WithInvalidTimeZone_ShouldFailFast()
    {
        var options = new WechatAgentOptions { TimeZone = "Mars/Olympus" };

        var act = options.Validate;

        act.Should().Throw<InvalidOperationException>().WithMessage("*TimeZone*");
    }

    private static WechatTurnContextAssembler Create(DateTimeOffset now)
    {
        var options = Options.Create(new WechatAgentOptions());
        return new WechatTurnContextAssembler(new StubCorp(), options, new FixedTime(now));
    }

    private static ConversationRequest Request()
        => new("corp-app", ConversationScope.Group(), "chat-1", "zhangsan", "msg-1", "hi", "chat-1");

    private sealed class FixedTime : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTime(DateTimeOffset now) => _now = now;

        public override DateTimeOffset GetUtcNow() => _now;
    }

    private sealed class StubCorp : IWechatAiCorpContextAccessor
    {
        public WechatToolPrincipal? Current { get; set; } = new("corp-app", "corp-1", "zhangsan");
    }
}
