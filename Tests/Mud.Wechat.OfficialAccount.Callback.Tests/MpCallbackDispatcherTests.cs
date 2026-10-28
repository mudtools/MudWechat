// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.Metrics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;
using Mud.Wechat.OfficialAccount.Abstractions.Metrics;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 公众号回调分发器与中间件的<b>回复链</b>用例（v1 方案 F3，MP-R1/R2）：被动回复处理器异常隔离、
/// 应答回写失败回落明文 success。
/// </summary>
/// <remarks>
/// <para>
/// 反例（同指纹二次推送仍被 403 / <c>ReplaySuspected</c>）不在本文件重复 —— 由
/// <c>MpCallbackReceiverTests</c> 用例 8 锁定；F3 的隔离与回落只发生在「指纹已消费之后」的
/// 应答组装层，不触碰接收器与指纹闸（v1 方案 §二红线）。
/// </para>
/// <para>
/// 中间件用例以 <c>DefaultHttpContext</c> 直调（无 TestHost 依赖），接收器以替身返回预置信封，
/// 把断言面精确收敛到「分发 → 应答组装 → 回落」这一段。
/// </para>
/// </remarks>
public class MpCallbackDispatcherTests
{
    private const string AppKey = "app1";
    private const string ValidAesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";

    // ---------------------------------------------------------------- 分发器层：回复处理器异常隔离（MP-R1①）

    [Fact]
    public async Task Dispatch_ShouldIsolateThrowingReplyHandler()
    {
        var (dispatcher, logger, _) = BuildDispatcher(new ThrowingReplyHandler());

        var result = await dispatcher.DispatchAsync(AppKey, NewEnvelope());

        result.Reply.Should().BeNull("回复处理器异常被隔离后按无回复处理（中间件回明文 success）");
        logger.Entries.Should().Contain(static e => e.Level == LogLevel.Error && e.Message.Contains("被动回复处理器"),
            "隔离必须记 Error（可观测不变量：静默回落等于把业务故障藏进 200）");
    }

    [Fact]
    public async Task Dispatch_ShouldPropagateCancellation_FromReplyHandler()
    {
        var (dispatcher, _, _) = BuildDispatcher(new CancellingReplyHandler());

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        Func<Task> act = () => dispatcher.DispatchAsync(AppKey, NewEnvelope(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>(
            "软超时 / 请求中止必须上抛由中间件按线语义处置（隔离只兜业务异常）");
    }

    // ---------------------------------------------------------------- 中间件层：回复链失败出口（MP-R1② / MP-R2）

    [Fact]
    public async Task Middleware_ShouldReturnSuccess_WhenReplyHandlerThrows()
    {
        var context = NewPostContext();
        var middleware = BuildMiddleware(new ThrowingReplyHandler(), validKey: true, out _, out var dispatcherLogger);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK, "回复处理器异常不得 500（事件已处理完）");
        ReadBody(context).Should().Be("success");
        dispatcherLogger.Entries.Should().Contain(static e => e.Level == LogLevel.Error && e.Message.Contains("被动回复处理器"),
            "隔离必须记 Error（可观测不变量）");
    }

    [Fact]
    public async Task Middleware_ShouldFallBackToSuccess_WhenReplyWriteFails()
    {
        // 密钥非法（非 43 位）⇒ WriteReply 的加密环节抛错 ⇒ 回落明文 success + 200（MP-R2）。
        var context = NewPostContext();
        var middleware = BuildMiddleware(new TextReplyHandler(), validKey: false, out var logger, out _);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK,
            "应答组装失败不得 500：事件已被全部处理器消费、指纹已消费，丢回复比丢事件更糟");
        ReadBody(context).Should().Be("success");
        logger.Entries.Should().Contain(static e => e.Level == LogLevel.Error && e.Message.Contains("回写失败"));
    }

    // ---------------------------------------------------------------- 回归：正常回复链路不受影响

    [Fact]
    public async Task Middleware_ShouldWriteEncryptedReply_WhenHandlerReturnsReply()
    {
        var context = NewPostContext();
        var middleware = BuildMiddleware(new TextReplyHandler(), validKey: true, out _, out _);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        context.Response.ContentType.Should().Contain("xml");
        ReadBody(context).Should().Contain("<Encrypt>", "安全模式回写加密 XML（官方契约；回归防误伤）");
    }

    [Fact]
    public async Task Middleware_ShouldWriteSuccess_WhenNoReplyHandlerMatches()
    {
        var context = NewPostContext();
        var middleware = BuildMiddleware(new NullReplyHandler(), validKey: true, out _, out _);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        ReadBody(context).Should().Be("success", "无回复 = 官方许可的明文 success（既有语义不变）");
    }

    // ---------------------------------------------------------------- 中间件层：观测接线（总案 D5 / OB-1）

    [Fact]
    public async Task Middleware_ShouldRecordSuccessOutcome_WhenPostHandled()
    {
        var recorded = new List<(string Instrument, double Value, Dictionary<string, object?> Tags)>();
        using var listener = NewMetricsListener(recorded);

        var context = NewPostContext();
        var middleware = BuildMiddleware(new NullReplyHandler(), validKey: true, out _, out _);

        await middleware.InvokeAsync(context);

        recorded.Should().Contain(r => r.Instrument == "callback.request.duration"
                && (string?)r.Tags["wechat.product"] == "officialaccount",
            "每个命中路由的入站请求记一笔全程耗时（接线点：中间件）");
        recorded.Should().Contain(r => r.Instrument == "callback.request" && r.Value == 1
                && (string?)r.Tags["outcome"] == "success"
                && (string?)r.Tags["wechat.app_key"] == "app1",
            "成功路径记 outcome=success 计数");
    }

    [Fact]
    public async Task Middleware_ShouldRecordFailureOutcome_WhenContentTypeRejected()
    {
        var recorded = new List<(string Instrument, double Value, Dictionary<string, object?> Tags)>();
        using var listener = NewMetricsListener(recorded);

        var context = NewPostContext();
        context.Request.ContentType = "application/json"; // 公众号回调恒 XML ⇒ 415，outcome=failure
        var middleware = BuildMiddleware(new NullReplyHandler(), validKey: true, out _, out _);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status415UnsupportedMediaType);
        recorded.Should().Contain(r => r.Instrument == "callback.request" && r.Value == 1
                && (string?)r.Tags["outcome"] == "failure"
                && (string?)r.Tags["error.type"] == "content_type",
            "415 拒收进 failure 计数（前置校验已上提 InvokeAsync，outcome 可见）");
    }

    /// <summary>订阅公众号线指标 Meter 的监听器（计数/直方图同收，断言用）。</summary>
    private static MeterListener NewMetricsListener(
        List<(string Instrument, double Value, Dictionary<string, object?> Tags)> recorded)
    {
        var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, enable) =>
        {
            if (instrument.Meter.Name == MpMetrics.MeterName)
            {
                enable.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
            recorded.Add((instrument.Name, value, ToTags(tags))));
        listener.SetMeasurementEventCallback<double>((instrument, value, tags, _) =>
            recorded.Add((instrument.Name, value, ToTags(tags))));
        listener.Start();
        return listener;
    }

    private static Dictionary<string, object?> ToTags(ReadOnlySpan<KeyValuePair<string, object?>> tags)
    {
        var dict = new Dictionary<string, object?>(tags.Length);
        foreach (var tag in tags)
        {
            dict[tag.Key] = tag.Value;
        }

        return dict;
    }

    // ---------------------------------------------------------------- 测试基建

    private static MpCallbackEnvelope NewEnvelope() => new()
    {
        ToUserName = "gh_app",
        FromUserName = "user-openid",
        MsgType = "text",
        DecryptedXml = "<xml><Content>hi</Content></xml>",
    };

    private static DefaultHttpContext NewPostContext()
    {
        var context = new DefaultHttpContext();
        var body = Encoding.UTF8.GetBytes("<xml><Content>hi</Content></xml>");
        context.Request.Body = new MemoryStream(body);
        context.Request.ContentLength = body.Length;
        context.Request.Method = "POST";
        context.Request.ContentType = "text/xml";
        context.Request.Path = "/mp/" + AppKey;
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static string ReadBody(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(((MemoryStream)context.Response.Body).ToArray());
    }

    private static (MpCallbackDispatcher Dispatcher, CapturingLogger<MpCallbackDispatcher> Logger, MpCallbackReplyHandlerRegistry Replies)
        BuildDispatcher(IMpCallbackReplyHandler replyHandler)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient(replyHandler.GetType());
        var provider = services.BuildServiceProvider();

        var replies = new MpCallbackReplyHandlerRegistry();
        replies.Register(MpCallbackOptions.WildcardAppKey, replyHandler.GetType());

        var logger = new CapturingLogger<MpCallbackDispatcher>();
        var dispatcher = new MpCallbackDispatcher(
            new MpCallbackHandlerRegistry(),
            new MpCallbackInterceptorRegistry(),
            replies,
            new StubPayloadReader(),
            new TestOptionsMonitor<MpCallbackOptions>(new MpCallbackOptions()),
            provider.GetRequiredService<IServiceScopeFactory>(),
            logger);
        return (dispatcher, logger, replies);
    }

    private static MpCallbackMiddleware BuildMiddleware(
        IMpCallbackReplyHandler replyHandler, bool validKey,
        out CapturingLogger<MpCallbackMiddleware> logger, out CapturingLogger<MpCallbackDispatcher> dispatcherLogger)
    {
        var (dispatcher, dispatcherLog, _) = BuildDispatcher(replyHandler);
        dispatcherLogger = dispatcherLog;
        logger = new CapturingLogger<MpCallbackMiddleware>();

        var options = new MpCallbackOptions();
        options.Apps[AppKey] = new MpAppCallbackOptions
        {
            PushToken = "mp-token",
            PushEncodingAESKey = validKey ? ValidAesKey : "short",
            AppId = "gh_app",
        };

        return new MpCallbackMiddleware(
            _ => Task.CompletedTask,
            new StubReceiver(NewEnvelope()),
            dispatcher,
            new TestOptionsMonitor<MpCallbackOptions>(options),
            new ServiceCollection().BuildServiceProvider(),
            logger);
    }

    /// <summary>载荷读取替身（本用例只含信封级处理器，载荷路径不应被触及）。</summary>
    private sealed class StubPayloadReader : IMpPayloadReader
    {
        public MpPayloadReadResult<TPayload> Read<TPayload>(MpCallbackEnvelope envelope)
            where TPayload : MpCallbackPayload
            => throw new InvalidOperationException("测试未预期调用载荷读取（本用例只含信封级处理器）");

        public MpPayloadReadResult<MpCallbackPayload> Read(MpCallbackEnvelope envelope, Type payloadType)
            => throw new InvalidOperationException("测试未预期调用载荷读取（本用例只含信封级处理器）");
    }

    /// <summary>接收器替身（返回预置信封，把断言面收敛到分发与应答组装）。</summary>
    private sealed class StubReceiver : IMpCallbackReceiver
    {
        private readonly MpCallbackEnvelope _envelope;

        public StubReceiver(MpCallbackEnvelope envelope) => _envelope = envelope;

        public Task<MpCallbackEnvelope> ReceiveAsync(
            string appKey, string urlQuery, string body, CancellationToken cancellationToken = default)
            => Task.FromResult(_envelope);

        public Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default)
            => Task.FromResult("echo");
    }

    private sealed class ThrowingReplyHandler : IMpCallbackReplyHandler
    {
        public Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("回复处理器故障（测试替身）");
    }

    private sealed class CancellingReplyHandler : IMpCallbackReplyHandler
    {
        public Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default)
            => Task.FromCanceled<MpCallbackReply?>(cancellationToken);
    }

    private sealed class TextReplyHandler : IMpCallbackReplyHandler
    {
        public Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default)
            => Task.FromResult<MpCallbackReply?>(
                new MpCallbackReply("text", new Dictionary<string, string?> { ["Content"] = "hi" }));
    }

    private sealed class NullReplyHandler : IMpCallbackReplyHandler
    {
        public Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default)
            => Task.FromResult<MpCallbackReply?>(null);
    }

    /// <summary>记录日志条目的最小 <see cref="ILogger{T}"/> 实现（供断言隔离 Error 与回落 Error）。</summary>
    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }
}
