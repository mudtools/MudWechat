// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 智能机器人回调面端到端测试：接收器（验签/时效/解密/空 receiveid/指纹）、分发器
/// （匹配序/应答收集/异常隔离/软超时/并发闸）、应答组装器（加密往返 + 字段名）与中间件 JSON 分派。
/// </summary>
/// <remarks>
/// 测试侧使用反射版 STJ 解析独立于 SDK 的源生成上下文（测试工程不参与 AOT strict 门禁），
/// 从而以「外部消费者视角」校验线上字节流的形态与字段名。
/// </remarks>
public class WechatBotCallbackTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "bot-push-token";
    private const string BotKey = "bot1";

    private static WechatCallbackOptions CreateOptions(Action<WechatCallbackOptions>? configure = null)
    {
        var options = new WechatCallbackOptions();
        options.Apps[BotKey] = new WechatAppCallbackOptions
        {
            PushToken = Token,
            PushEncodingAESKey = AesKey,
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,
        };
        configure?.Invoke(options);
        return options;
    }

    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    private static string QueryFor(string encrypt, string? timestamp = null, string? nonce = null)
    {
        var ts = timestamp ?? Now();
        var nc = nonce ?? Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(Token, ts, nc, encrypt);
        return $"msg_signature={signature}&timestamp={ts}&nonce={nc}";
    }

    /// <summary>构造官方形态的文本消息回调明文。</summary>
    private static string TextMessageJson(string msgId = "MSGID1")
        => "{\"msgid\":\"" + msgId + "\",\"aibotid\":\"BOTID\",\"chatid\":\"CHATID\",\"chattype\":\"single\"," +
           "\"from\":{\"userid\":\"zhangsan\"}," +
           "\"response_url\":\"https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=RC\"," +
           "\"msgtype\":\"text\",\"text\":{\"content\":\"hi\"}}";

    private static string EncryptedBody(string plainJson, string receiveId = "")
    {
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainJson, receiveId);
        return "{\"encrypt\":\"" + encrypt + "\"}";
    }

    private static string ExtractEncrypt(string jsonBody)
        => JsonSerializer.Deserialize<AibotEncryptedEnvelope>(jsonBody)!.Encrypt!;

    // ---------------------------------------------------------------- 接收器

    [Fact]
    public async Task Receiver_ShouldParseTextMessage_WhenProtocolValid()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        var body = EncryptedBody(TextMessageJson());

        var evt = await receiver.ReceiveAsync(BotKey, QueryFor(ExtractEncrypt(body)), body);

        evt.AppKey.Should().Be(BotKey);
        evt.MsgId.Should().Be("MSGID1");
        evt.AibotId.Should().Be("BOTID");
        evt.ChatType.Should().Be("single");
        evt.FromUserId.Should().Be("zhangsan");
        evt.MsgType.Should().Be("text");
        evt.EventType.Should().BeNull("消息回调无 event.eventtype");
        evt.IsEventCallback.Should().BeFalse();
        evt.EventTypeKey.Should().Be("text", "消息回调的匹配键为顶层 msgtype");
        evt.Message.Should().NotBeNull("信封必须直出强类型载荷（宿主无需自反序列化）");
        evt.Message!.Text!.Content.Should().Be("hi");
        evt.ResponseUrl.Should().Contain("response_code=RC");
    }

    [Fact]
    public async Task Receiver_ShouldParseEvent_WhenEventPayload()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        const string plain =
            "{\"msgid\":\"M2\",\"create_time\":1700000000,\"aibotid\":\"BOTID\",\"chattype\":\"single\"," +
            "\"from\":{\"userid\":\"zhangsan\"},\"msgtype\":\"event\",\"event\":{\"eventtype\":\"template_card_event\"," +
            "\"template_card_event\":{\"card_type\":\"button_interaction\",\"event_key\":\"k1\",\"task_id\":\"T1\"," +
            "\"selected_items\":{\"selected_item\":[{\"question_key\":\"q1\"," +
            "\"option_ids\":{\"option_id\":[\"o1\",\"o2\"]}}]}}}}";
        var body = EncryptedBody(plain);

        var evt = await receiver.ReceiveAsync(BotKey, QueryFor(ExtractEncrypt(body)), body);

        evt.IsEventCallback.Should().BeTrue();
        evt.EventTypeKey.Should().Be("template_card_event");
        evt.Event!.CreateTime.Should().Be(1700000000);
        evt.Event.Event!.TemplateCardEvent!.TaskId.Should().Be("T1");
        evt.Event.Event.TemplateCardEvent.EventKey.Should().Be("k1");
        evt.Event.Event.TemplateCardEvent.SelectedItems!.SelectedItem![0]
            .OptionIds!.OptionId.Should().Equal("o1", "o2");
    }

    [Fact]
    public async Task Receiver_ShouldReject_WhenSignatureMismatched()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        var body = EncryptedBody(TextMessageJson());
        var query = $"msg_signature=deadbeef&timestamp={Now()}&nonce=n1";

        await receiver.Invoking(r => r.ReceiveAsync(BotKey, query, body))
            .Should().ThrowAsync<WechatCallbackException>();
    }

    [Fact]
    public async Task Receiver_ShouldReject_WhenReplayFingerprintConsumed()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        var body = EncryptedBody(TextMessageJson());
        var query = QueryFor(ExtractEncrypt(body));

        await receiver.ReceiveAsync(BotKey, query, body);
        var replay = await receiver.Invoking(r => r.ReceiveAsync(BotKey, query, body))
            .Should().ThrowAsync<WechatCallbackException>();
        replay.Which.Kind.Should().Be(WechatCallbackFailureKind.ReplaySuspected,
            "指纹已在解密后消费（fail-closed 优先于 at-least-once）");
    }

    [Fact]
    public async Task Receiver_ShouldReject_WhenReceiveIdNotEmpty()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        // 官方 101033：企业内部智能机器人场景 receiveid 恒为空串 ⇒ 非空即拒。
        var body = EncryptedBody(TextMessageJson(), receiveId: "ww-corp");

        var mismatch = await receiver.Invoking(r => r.ReceiveAsync(BotKey, QueryFor(ExtractEncrypt(body)), body))
            .Should().ThrowAsync<WechatCallbackException>();
        mismatch.Which.Kind.Should().Be(WechatCallbackFailureKind.ReceiveIdMismatch);
    }

    [Fact]
    public async Task Receiver_ShouldReject_WhenChannelIsNotBot()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions(o =>
            {
                o.Apps[BotKey] = new WechatAppCallbackOptions
                {
                    PushToken = Token,
                    PushEncodingAESKey = AesKey,
                    AppType = WechatAppType.Internal,
                    Channel = WechatCallbackChannel.App,
                };
            })));
        var body = EncryptedBody(TextMessageJson());

        await receiver.Invoking(r => r.ReceiveAsync(BotKey, QueryFor(ExtractEncrypt(body)), body))
            .Should().ThrowAsync<WechatCallbackException>(
                "Channel != Bot 的条目不得接收 JSON 回调（App/Suite 条目为加密 XML）");
    }

    [Fact]
    public async Task Receiver_ShouldReject_WhenMsgIdMissing()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        var body = EncryptedBody("{\"aibotid\":\"BOTID\",\"msgtype\":\"text\",\"text\":{\"content\":\"hi\"}}");

        await receiver.Invoking(r => r.ReceiveAsync(BotKey, QueryFor(ExtractEncrypt(body)), body))
            .Should().ThrowAsync<WechatCallbackException>(
                "官方以 msgid 排重 ⇒ 缺失即非官方报文结构，必须拒收而非放行");
    }

    [Fact]
    public async Task Receiver_ShouldReject_WhenTimestampOutOfWindow()
    {
        var receiver = new WechatBotCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        var body = EncryptedBody(TextMessageJson());
        var encrypt = ExtractEncrypt(body);
        var stale = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 3600).ToString();
        var signature = WechatCallbackCrypto.ComputeSignature(Token, stale, "n1", encrypt);

        var outOfRange = await receiver
            .Invoking(r => r.ReceiveAsync(BotKey, $"msg_signature={signature}&timestamp={stale}&nonce=n1", body))
            .Should().ThrowAsync<WechatCallbackException>();
        outOfRange.Which.Kind.Should().Be(WechatCallbackFailureKind.TimestampOutOfRange);
    }

    // ---------------------------------------------------------------- 应答组装

    [Fact]
    public void ReplyWriter_ShouldProduceEncryptedEnvelope_WithMsgsignatureField()
    {
        var options = CreateOptions();
        var app = options.Apps[BotKey];
        const string nonce = "NONCE1";

        var payload = WechatBotReplyWriter.WriteReply(app, WechatBotReplies.Text("hello"), nonce);

        payload.Should().Contain("\"msgsignature\"").And.NotContain("msg_signature",
            "官方 101033：应答签名字段为 msgsignature（无下划线）");

        var envelope = JsonSerializer.Deserialize<AibotEncryptedEnvelope>(payload)!;
        envelope.Encrypt.Should().NotBeNullOrEmpty();
        envelope.Nonce.Should().Be(nonce, "官方要求应答 nonce 使用回调 URL 中的 nonce");
        envelope.Msgsignature.Should().NotBeNullOrEmpty();

        var expected = WechatCallbackCrypto.ComputeSignature(
            app.PushToken,
            envelope.Timestamp!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
            nonce,
            envelope.Encrypt!);
        envelope.Msgsignature.Should().Be(expected, "应答签名必须与请求侧同算法（SHA1 四元组排序）");

        var plain = WechatCallbackCrypto.Decrypt(app.PushEncodingAESKey, envelope.Encrypt!, out var receiveId);
        receiveId.Should().BeEmpty("智能机器人场景 receiveid 恒为空串（官方 101033）");

        var reply = JsonSerializer.Deserialize<AibotMessage>(plain)!;
        reply.MsgType.Should().Be("text");
        reply.Text!.Content.Should().Be("hello");
    }

    [Fact]
    public void ReplyWriter_ShouldWriteEmptyObject_WhenNoReply()
    {
        var options = CreateOptions();
        var payload = WechatBotReplyWriter.WriteReply(options.Apps[BotKey], null, "n1");

        var envelope = JsonSerializer.Deserialize<AibotEncryptedEnvelope>(payload)!;
        var plain = WechatCallbackCrypto.Decrypt(AesKey, envelope.Encrypt!, out _);
        plain.Should().Be("{}", "无应答 / feedback_event / 未匹配处理器统一回空包");
    }

    // ---------------------------------------------------------------- 分发器

    [Fact]
    public async Task Dispatcher_ShouldPreferExactHandlerAndReturnItsReply()
    {
        var (dispatcher, provider) = CreateDispatcher(
            new ProbeBotHandler<SlotA>(WechatBotEventTypes.Text, (_, _) => Task.FromResult<AibotMessage?>(WechatBotReplies.Text("exact"))),
            new ProbeBotHandler<SlotB>(string.Empty, (_, _) => Task.FromResult<AibotMessage?>(WechatBotReplies.Text("fallback"))));
        using var providerDisposable = provider;

        var result = await dispatcher.DispatchAsync(BotKey, MessageEvent());

        result.Outcome.Should().Be(WechatBotDispatchOutcome.Handled);
        result.Reply!.Text!.Content.Should().Be("exact", "精确命中即不执行兜底");
    }

    [Fact]
    public async Task Dispatcher_ShouldFallBackOnDefaultHandler_WhenNoExactMatch()
    {
        var (dispatcher, provider) = CreateDispatcher(
            new ProbeBotHandler<SlotA>(string.Empty, (_, _) => Task.FromResult<AibotMessage?>(WechatBotReplies.Text("fallback"))));
        using var providerDisposable = provider;

        var result = await dispatcher.DispatchAsync(BotKey, MessageEvent());

        result.Reply!.Text!.Content.Should().Be("fallback");
    }

    [Fact]
    public async Task Dispatcher_ShouldReturnUnhandled_WhenNoHandlerMatched()
    {
        var (dispatcher, provider) = CreateDispatcher();
        using var providerDisposable = provider;

        var result = await dispatcher.DispatchAsync(BotKey, MessageEvent());

        result.Outcome.Should().Be(WechatBotDispatchOutcome.Unhandled);
        result.Reply.Should().BeNull("未匹配处理器回空包");
    }

    [Fact]
    public async Task Dispatcher_ShouldIsolateHandlerException_AndKeepFirstNonNullReply()
    {
        var (dispatcher, provider) = CreateDispatcher(
            new ProbeBotHandler<SlotA>(string.Empty, (_, _) => throw new InvalidOperationException("boom")),
            new ProbeBotHandler<SlotB>(string.Empty, (_, _) => Task.FromResult<AibotMessage?>(WechatBotReplies.Text("second"))),
            new ProbeBotHandler<SlotC>(string.Empty, (_, _) => Task.FromResult<AibotMessage?>(WechatBotReplies.Text("third"))));
        using var providerDisposable = provider;

        var result = await dispatcher.DispatchAsync(BotKey, MessageEvent());

        result.Outcome.Should().Be(WechatBotDispatchOutcome.Handled, "单处理器异常被隔离，其余处理器继续执行");
        result.Reply!.Text!.Content.Should().Be("second", "应答取首个非 null（后注册者不得覆盖）");
    }

    [Fact]
    public async Task Dispatcher_ShouldPropagateCancellation_WhenSoftTimeout()
    {
        var (dispatcher, provider) = CreateDispatcher(
            new[]
            {
                new ProbeBotHandler<SlotA>(string.Empty, async (_, ct) =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), ct);
                    return null;
                }),
            },
            timeoutMs: 50);
        using var providerDisposable = provider;

        await dispatcher.Invoking(d => d.DispatchAsync(BotKey, MessageEvent()))
            .Should().ThrowAsync<OperationCanceledException>(
                "软超时必须向适配层传播（由其决定加密空包应答）");
    }

    [Fact]
    public async Task Dispatcher_ShouldThrow_WhenBotKeyEmpty()
    {
        var (dispatcher, provider) = CreateDispatcher();
        using var providerDisposable = provider;

        await dispatcher.Invoking(d => d.DispatchAsync(string.Empty, MessageEvent()))
            .Should().ThrowAsync<ArgumentException>();
    }

    // ---------------------------------------------------------------- 中间件

    [Fact]
    public async Task Middleware_ShouldReturnEncryptedReply_WhenJsonCallbackHandled()
    {
        var (middleware, provider) = CreateMiddleware("hi");
        using var providerDisposable = provider;

        var body = EncryptedBody(TextMessageJson());
        var context = CreatePostContext(body, QueryFor(ExtractEncrypt(body)));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
        context.Response.ContentType.Should().Contain("json");

        var payload = ResponseBody(context);
        payload.Should().Contain("msgsignature");
        var envelope = JsonSerializer.Deserialize<AibotEncryptedEnvelope>(payload)!;
        var plain = WechatCallbackCrypto.Decrypt(AesKey, envelope.Encrypt!, out _);
        JsonSerializer.Deserialize<AibotMessage>(plain)!.Text!.Content.Should().Be("hi");
    }

    [Fact]
    public async Task Middleware_ShouldReplyEmptyPacket_WhenNoBotHandlerRegistered()
    {
        var (middleware, provider) = CreateMiddleware(null);
        using var providerDisposable = provider;

        var body = EncryptedBody(TextMessageJson());
        var context = CreatePostContext(body, QueryFor(ExtractEncrypt(body)));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200, "未匹配处理器仍必须给出合法加密应答（不是 503/500）");
        var envelope = JsonSerializer.Deserialize<AibotEncryptedEnvelope>(ResponseBody(context))!;
        WechatCallbackCrypto.Decrypt(AesKey, envelope.Encrypt!, out _).Should().Be("{}");
    }

    [Fact]
    public async Task Middleware_ShouldReturn415_WhenJsonBodyTooLargeIsNotReachedButContentTypeUnknown()
    {
        var (middleware, provider) = CreateMiddleware(null, o => o.MaxRequestBodySize = 8);
        using var providerDisposable = provider;

        var body = EncryptedBody(TextMessageJson());
        var context = CreatePostContext(body, QueryFor(ExtractEncrypt(body)));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(413);
    }

    [Fact]
    public async Task Middleware_ShouldSkip_WhenPathMismatched()
    {
        var (middleware, provider) = CreateMiddleware(null);
        using var providerDisposable = provider;

        var body = EncryptedBody(TextMessageJson());
        var context = CreatePostContext(body, QueryFor(ExtractEncrypt(body)), path: "/wrong/bot1");

        await middleware.InvokeAsync(context);

        ResponseBody(context).Should().BeEmpty("非本中间件路由不打劫");
    }

    [Fact]
    public async Task Middleware_ShouldReturn403_WhenJsonChannelIsNotBot()
    {
        var (middleware, provider) = CreateMiddleware(null, o =>
        {
            o.Apps[BotKey] = new WechatAppCallbackOptions
            {
                PushToken = Token,
                PushEncodingAESKey = AesKey,
                AppType = WechatAppType.Internal,
                Channel = WechatCallbackChannel.App,
            };
        });
        using var providerDisposable = provider;

        var body = EncryptedBody(TextMessageJson());
        var context = CreatePostContext(body, QueryFor(ExtractEncrypt(body)));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403,
            "Channel != Bot 的条目收到 JSON 回调属配置错配（fail-closed 403）");
    }

    [Fact]
    public async Task Middleware_ShouldReturn415_WhenContentTypeIsNeitherXmlNorJson()
    {
        var (middleware, provider) = CreateMiddleware(null);
        using var providerDisposable = provider;

        var context = CreatePostContext("{\"encrypt\":\"x\"}", "a=1", contentType: "text/plain");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(415);
    }

    [Fact]
    public async Task Middleware_ShouldStillHandleXmlCallbacks_WhenBotSurfaceRegistered()
    {
        var (middleware, provider) = CreateMiddleware(null, null, includeXmlHandler: true);
        using var providerDisposable = provider;

        const string plainXml =
            "<xml><MsgType><![CDATA[event]]></MsgType><Event><![CDATA[change_contact]]></Event>" +
            "<ChangeType><![CDATA[create_user]]></ChangeType></xml>";
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, string.Empty);
        var context = CreateContext(
            "POST", "/wechat/" + BotKey, QueryFor(encrypt),
            $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>", "text/xml");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
        ResponseBody(context).Should().Be("success", "XML 面应答仍为 success（bot 注册不改 XML 面行为）");
    }

    // ---------------------------------------------------------------- 测试脚手架

    private static WechatBotCallbackEvent MessageEvent() => new()
    {
        MsgId = "M1",
        MsgType = WechatBotEventTypes.Text,
    };

    private static (WechatBotEventDispatcher Dispatcher, ServiceProvider Provider) CreateDispatcher(
        params IWechatBotCallbackEventHandler[] handlers)
        => CreateDispatcher(handlers, 4500);

    private static (WechatBotEventDispatcher Dispatcher, ServiceProvider Provider) CreateDispatcher(
        IWechatBotCallbackEventHandler[] handlers, int timeoutMs)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var registry = new WechatBotHandlerRegistry();
        foreach (var handler in handlers)
        {
            services.AddSingleton(handler.GetType(), handler);
            registry.Register(BotKey, handler.GetType());
        }

        var options = CreateOptions(o => o.EventHandlingTimeoutMs = timeoutMs);
        services.AddSingleton<IOptionsMonitor<WechatCallbackOptions>>(
            new TestOptionsMonitor<WechatCallbackOptions>(options));
        services.AddSingleton(registry);

        var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        return (new WechatBotEventDispatcher(
            registry,
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<WechatBotEventDispatcher>.Instance), provider);
    }

    private static (WechatCallbackMiddleware Middleware, ServiceProvider Provider) CreateMiddleware(
        string? replyText,
        Action<WechatCallbackOptions>? configureOptions = null,
        bool includeXmlHandler = false)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IOptionsMonitor<WechatCallbackOptions>>(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions(configureOptions)));

        var builder = services.AddWechatCallback(_ => { });
        if (includeXmlHandler)
        {
            builder.AddHandler<ProbeCallbackHandler>();
        }

        var botBuilder = services.AddWechatBotCallback();
        if (replyText != null)
        {
            services.AddSingleton(new ProbeReplyState { Text = replyText });
            botBuilder.AddHandler<ProbeBotHandler2>(BotKey);
        }

        var provider = services.BuildServiceProvider();
        var middleware = new WechatCallbackMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<IWechatCallbackReceiver>(),
            provider.GetRequiredService<WechatCallbackDispatcher>(),
            provider.GetRequiredService<IWechatBotCallbackReceiver>(),
            provider.GetRequiredService<WechatBotEventDispatcher>(),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            NullLogger<WechatCallbackMiddleware>.Instance);

        return (middleware, provider);
    }

    private static DefaultHttpContext CreatePostContext(
        string body, string query, string path = "/wechat/bot1", string contentType = "application/json")
        => CreateContext("POST", path, query, body, contentType);

    private static DefaultHttpContext CreateContext(
        string method, string path, string? query, string? body, string? contentType)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        if (query != null)
        {
            context.Request.QueryString = new QueryString("?" + query);
        }

        if (body != null)
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            context.Request.Body = new MemoryStream(bytes);
            context.Request.ContentLength = bytes.Length;
        }

        if (contentType != null)
        {
            context.Request.ContentType = contentType;
        }

        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static string ResponseBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// 探测处理器（构造期注入应答委托；<see cref="SupportedEventType"/> 由测试指定）。
    /// </summary>
    /// <remarks>
    /// DI 按<b>具体类型</b>注册与解析，故同一测试内多个探测处理器必须用不同闭合泛型占位
    /// （<see cref="ProbeBotHandler{TSlot}"/>）区分，否则后注册的实例会覆盖先注册的。
    /// </remarks>
    private class ProbeBotHandler : IWechatBotCallbackEventHandler
    {
        private readonly Func<WechatBotCallbackEvent, CancellationToken, Task<AibotMessage?>> _handle;

        public ProbeBotHandler(
            string supportedEventType, Func<WechatBotCallbackEvent, CancellationToken, Task<AibotMessage?>> handle)
        {
            SupportedEventType = supportedEventType;
            _handle = handle;
        }

        public string SupportedEventType { get; }

        public Task<AibotMessage?> HandleAsync(
            WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
            => _handle(eventData, cancellationToken);
    }

    /// <summary>经真实 DI 注册的 bot 处理器（中间件端到端用）。</summary>
    private sealed class ProbeBotHandler2 : IWechatBotCallbackEventHandler
    {
        private readonly ProbeReplyState _state;

        public ProbeBotHandler2(ProbeReplyState state) => _state = state;

        public string SupportedEventType => WechatBotEventTypes.Text;

        public Task<AibotMessage?> HandleAsync(
            WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult<AibotMessage?>(WechatBotReplies.Text(_state.Text ?? string.Empty));
    }

    /// <summary>XML 面处理器（验证 bot 注册不影响 XML 面分发）。</summary>
    private sealed class ProbeCallbackHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => "create_user";

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class ProbeReplyState
    {
        public string? Text { get; set; }
    }

    /// <summary>探测处理器的泛型占位派生（<typeparamref name="TSlot"/> 仅用于生成不同闭合类型）。</summary>
    private sealed class ProbeBotHandler<TSlot> : ProbeBotHandler
    {
        public ProbeBotHandler(
            string supportedEventType, Func<WechatBotCallbackEvent, CancellationToken, Task<AibotMessage?>> handle)
            : base(supportedEventType, handle)
        {
        }
    }

    private sealed class SlotA;

    private sealed class SlotB;

    private sealed class SlotC;
}
