// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Callback.LongConnection;
using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.Callback.Tests;

namespace Mud.Wechat.Work.Tests.LongConnection;

/// <summary>
/// 长连接 P3 的<b>无网络</b>行为测试：帧编解码往返、流式状态机（req_id 复用 / finish 终结 / 10 分钟窗口）、
/// 运行器启动期校验（配置形状 + <b>跨面互斥</b>）。
/// </summary>
/// <remarks>
/// WebSocket 真连接属集成场景（官方 openws 需真实凭据），本文件全部停在编解码与纯逻辑层 ——
/// 与 Finance 批次「不触原生库」的测试分工同源；治理红线（TFM 门控 / 无指纹闸 / 先夺租约）由
/// <c>WechatBotContractGuards</c> 的 BT10 源码断言承担。
/// </remarks>
public class WechatBotLongConnectionTests
{
    // ---------------------------------------------------------------- 帧编解码

    [Fact]
    public void FrameCodec_ShouldRoundTripShell_WithOfficialFieldNames()
    {
        var json = WechatBotFrameCodec.EncodeFrame(
            WechatBotFrameCommands.SendMsg,
            "req-1",
            new AibotSendMessageBody { ChatId = "chat-1", ChatType = 2, MsgType = "markdown" },
            AibotJsonContext.Default.AibotSendMessageBody);

        json.Should().Contain("\"cmd\":\"aibot_send_msg\"")
            .And.Contain("\"req_id\":\"req-1\"")
            .And.Contain("\"chat_type\":2")
            .And.Contain("\"chatid\":\"chat-1\"");

        var frame = WechatBotFrameCodec.ParseFrame(json);
        frame.Cmd.Should().Be(WechatBotFrameCommands.SendMsg);
        frame.Headers!.ReqId.Should().Be("req-1");
        frame.Body.Should().NotBeNull();
    }

    [Fact]
    public void FrameCodec_ShouldRejectFrameWithoutCmd()
    {
        var act = () => WechatBotFrameCodec.ParseFrame("""{"headers":{"req_id":"r1"},"errcode":0}""");

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("cmd");
    }

    [Fact]
    public void FrameCodec_ShouldParseAckWithErrcode()
    {
        var ack = WechatBotFrameCodec.ParseAck("""{"headers":{"req_id":"r1"},"errcode":0,"errmsg":"ok"}""");

        ack.IsSuccess.Should().BeTrue();
        ack.Headers!.ReqId.Should().Be("r1");
        ack.ErrMsg.Should().Be("ok");

        WechatBotFrameCodec.ParseAck("""{"headers":{"req_id":"r2"},"errcode":10008,"errmsg":"denied"}""")
            .IsSuccess.Should().BeFalse();
    }

    // ---------------------------------------------------------------- 流式状态机

    [Fact]
    public void StreamRegistry_ShouldReuseFirstReqId_WhenRefreshingSameStream()
    {
        var registry = new WechatBotStreamRegistry();

        registry.BeginOrRefresh("s1", "req-first", finish: false).Should().Be("req-first",
            "首次发送记账本次 req_id");
        registry.BeginOrRefresh("s1", "req-second", finish: false).Should().Be("req-first",
            "官方 101463：同一次回调的所有流式回复须用相同 req_id ⇒ 刷新复用首次的 req_id");
    }

    [Fact]
    public void StreamRegistry_ShouldRejectAfterFinish_OrExpiredWindow()
    {
        var now = DateTimeOffset.UtcNow;
        var registry = new WechatBotStreamRegistry(() => now);

        registry.BeginOrRefresh("s1", "req-1", finish: true);

        var act = () => registry.BeginOrRefresh("s1", "req-2", finish: false);
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("finish");

        // 10 分钟窗口：超窗拒绝（时钟动态推进 —— 首次在 now、刷新在 now+11min）。
        var live = new WechatBotStreamRegistry(() => now);
        live.BeginOrRefresh("s2", "req-1", finish: false);
        live.BeginOrRefresh("s2", "req-2", finish: false)
            .Should().Be("req-1", "窗口内刷新正常");

        var current = now;
        var expired = new WechatBotStreamRegistry(() => current);
        expired.BeginOrRefresh("s3", "req-1", finish: false);
        current = now.AddMinutes(11);
        var actExpired = () => expired.BeginOrRefresh("s3", "req-2", finish: false);
        actExpired.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("10 分钟");
    }

    [Fact]
    public void StreamRegistry_ShouldRejectEmptyStreamId()
    {
        var registry = new WechatBotStreamRegistry();

        var act = () => registry.BeginOrRefresh(string.Empty, "req-1", finish: false);

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("stream.id");
    }

    // ---------------------------------------------------------------- 运行器启动校验

    private static WechatBotOptions NewBotOptions(string botKey = "bot1") => new()
    {
        Bots = new Dictionary<string, WechatBotAppOptions>
        {
            [botKey] = new WechatBotAppOptions { BotId = "BOT-ID", BotSecret = "secret-value", EnableLongConnection = true },
        },
    };

    private static WechatCallbackOptions NewCallbackOptions(string botKey = "bot1") => new();

    [Fact]
    public async Task Runner_StartAsync_ShouldFailFast_WhenBothModesConfiguredForSameBotKey()
    {
        var callbackOptions = NewCallbackOptions();
        callbackOptions.Apps["bot1"] = new WechatAppCallbackOptions
        {
            PushToken = "t",
            PushEncodingAESKey = new string('a', 43),
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,
        };

        using var provider = BuildProvider(NewBotOptions(), callbackOptions, out var runner);

        Func<Task> act = () => runner.StartAsync(CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("二选一")
            .And.Contain("bot1", "官方 101463：长连接 / 回调地址切换即失效 ⇒ 同 BotKey 双配置必须启动期点名");
    }

    [Fact]
    public async Task Runner_StartAsync_ShouldPass_WhenOnlyLongConnectionConfigured()
    {
        using var provider = BuildProvider(NewBotOptions(), NewCallbackOptions(), out var runner);

        await runner.StartAsync(new CancellationTokenSource(50).Token);
        // StartAsync 通过即跨面互斥校验通过（ExecuteAsync 在取消后自然结束）。
    }

    [Fact]
    public async Task Runner_ShouldValidateBotOptions_WhenBotsMissing()
    {
        var runner = new WechatBotLongConnectionRunner(
            new WechatBotOptions(),
            NewCallbackOptions(),
            CreateDispatcher(),
            lease: null,
            NullLogger<WechatBotLongConnectionRunner>.Instance);

        Func<Task> act = () => runner.StartAsync(CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("Bots");
    }

    private static ServiceProvider BuildProvider(
        WechatBotOptions botOptions, WechatCallbackOptions callbackOptions, out WechatBotLongConnectionRunner runner)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Options.Create(botOptions));
        services.AddSingleton(Options.Create(callbackOptions));
        services.AddSingleton(botOptions);
        services.AddSingleton(callbackOptions);

        var dispatcher = CreateDispatcher();
        services.AddSingleton(dispatcher);

        var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        runner = new WechatBotLongConnectionRunner(
            botOptions,
            callbackOptions,
            dispatcher,
            lease: null,
            NullLogger<WechatBotLongConnectionRunner>.Instance);
        return provider;
    }

    private static WechatBotEventDispatcher CreateDispatcher()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new WechatBotHandlerRegistry());
        var provider = services.BuildServiceProvider();
        return new WechatBotEventDispatcher(
            provider.GetRequiredService<WechatBotHandlerRegistry>(),
            new TestOptionsMonitor<WechatCallbackOptions>(new WechatCallbackOptions()),
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<WechatBotEventDispatcher>.Instance);
    }
}

#endif
