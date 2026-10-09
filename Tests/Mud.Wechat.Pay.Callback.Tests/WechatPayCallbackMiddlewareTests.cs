// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 支付回调中间件的路由与应答映射用例（方案 §2.6「应答」行）。
/// </summary>
/// <remarks>
/// <b>为何单列应答映射</b>：官方以 HTTP 状态 + <c>code</c> 决定是否重推；
/// 映射错一格会出现「该重推的不重推（丢单）」或「该止住的重推（重复处理）」。
/// </remarks>
public class WechatPayCallbackMiddlewareTests
{
    /// <summary>合法通知 ⇒ 200 + <c>{"code":"SUCCESS"}</c>，且分发器把事件交给了处理器。</summary>
    [Fact]
    public async Task Invoke_ShouldReturnSuccess_WhenNotificationValid()
    {
        using var harness = new MiddlewareHarness();
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var response = await harness.InvokeAsync("POST", headers, body);

        response.StatusCode.Should().Be(200);
        response.Body.Should().Be(WechatPayCallbackMiddleware.SuccessResponseBody);
        response.ContentType.Should().Contain("application/json");
        harness.Handler.LastEventType.Should().Be("TRANSACTION.SUCCESS");
    }

    /// <summary>验签失败 ⇒ 403 + 固定 FAIL 文案（不区分原因，避免探测 oracle）。</summary>
    [Fact]
    public async Task Invoke_ShouldReturn403_WhenSignatureInvalid()
    {
        using var harness = new MiddlewareHarness();
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);
        var unknownSerial = new WechatPayCallbackHeaders(
            headers.Timestamp, headers.Nonce, headers.Signature, "UNKNOWN-SERIAL-XYZ");

        var response = await harness.InvokeAsync("POST", unknownSerial, body);

        response.StatusCode.Should().Be(403);
        response.Body.Should().Be(WechatPayCallbackMiddleware.FailureResponseBody);
        response.Body.Should().NotContain("UNKNOWN-SERIAL-XYZ", "失败文案必须固定，不得回显请求细节");
    }

    /// <summary>未登记商户 ⇒ 403（不得回落默认商户）。</summary>
    [Fact]
    public async Task Invoke_ShouldReturn403_WhenMerchantUnknown()
    {
        using var harness = new MiddlewareHarness();
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var response = await harness.InvokeAsync("POST", headers, body, merchantKey: "8888888888");

        response.StatusCode.Should().Be(403);
    }

    /// <summary>GET ⇒ 405（APIv3 通知无 GET echo 流程）。</summary>
    [Fact]
    public async Task Invoke_ShouldReturn405_WhenMethodNotPost()
    {
        using var harness = new MiddlewareHarness();

        var response = await harness.InvokeAsync("GET", default, "{}");

        response.StatusCode.Should().Be(405);
    }

    /// <summary>非 JSON Content-Type ⇒ 415。</summary>
    [Fact]
    public async Task Invoke_ShouldReturn415_WhenContentTypeNotJson()
    {
        using var harness = new MiddlewareHarness();
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var response = await harness.InvokeAsync("POST", headers, body, contentType: "text/plain");

        response.StatusCode.Should().Be(415);
    }

    /// <summary>体长超限 ⇒ 413。</summary>
    [Fact]
    public async Task Invoke_ShouldReturn413_WhenBodyTooLarge()
    {
        using var harness = new MiddlewareHarness();
        harness.Fixture.Options.MaxRequestBodySize = 8;
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var response = await harness.InvokeAsync("POST", headers, body);

        response.StatusCode.Should().Be(413);
    }

    /// <summary>非本中间件路由 ⇒ 交后续中间件（不得吞掉）。</summary>
    [Theory]
    [InlineData("/other/1900000000")]
    [InlineData("/pay")]
    [InlineData("/pay/1900000000/extra")]
    public async Task Invoke_ShouldDelegateToNext_WhenRouteNotMatched(string path)
    {
        using var harness = new MiddlewareHarness();
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var response = await harness.InvokeAsync("POST", headers, body, path: path);

        response.NextCalled.Should().BeTrue();
    }

    /// <summary>处理器抛异常 ⇒ 仍 200 SUCCESS（单处理器异常隔离；通知已接收）。</summary>
    [Fact]
    public async Task Invoke_ShouldStillReturnSuccess_WhenHandlerThrows()
    {
        using var harness = new MiddlewareHarness(throwInHandler: true);
        var (headers, body) = harness.Fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var response = await harness.InvokeAsync("POST", headers, body);

        response.StatusCode.Should().Be(200);
        response.Body.Should().Be(WechatPayCallbackMiddleware.SuccessResponseBody);
    }

    /// <summary>无匹配处理器 ⇒ 200 SUCCESS（事件已接收，官方不应重推）。</summary>
    [Fact]
    public async Task Invoke_ShouldReturnSuccess_WhenNoHandlerMatched()
    {
        using var harness = new MiddlewareHarness();
        var (headers, body) = harness.Fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson, eventType: "REFUND.SUCCESS");

        var response = await harness.InvokeAsync("POST", headers, body);

        response.StatusCode.Should().Be(200);
        response.Body.Should().Be(WechatPayCallbackMiddleware.SuccessResponseBody);
    }

    /// <summary>中间件一次调用结果的快照。</summary>
    private sealed class MiddlewareResponse
    {
        public MiddlewareResponse(int statusCode, string body, string contentType, bool nextCalled)
        {
            StatusCode = statusCode;
            Body = body;
            ContentType = contentType;
            NextCalled = nextCalled;
        }

        public int StatusCode { get; }

        public string Body { get; }

        public string ContentType { get; }

        public bool NextCalled { get; }
    }

    /// <summary>记录型处理器。</summary>
    private sealed class RecordingHandler : IWechatPayNotificationHandler
    {
        private readonly MiddlewareHarness _harness;

        public RecordingHandler(MiddlewareHarness harness) => _harness = harness;

        public string? LastEventType { get; private set; }

        public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
        {
            LastEventType = context.EventType;

            if (_harness.ThrowInHandler)
            {
                throw new InvalidOperationException("处理器故意抛错（验证异常隔离）");
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>装配真实接收器 / 分发器的中间件测试夹具。</summary>
    private sealed class MiddlewareHarness : IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly WechatPayCallbackMiddleware _middleware;

        public MiddlewareHarness(bool throwInHandler = false)
        {
            Fixture = new WechatPayCallbackTestFixture();
            ThrowInHandler = throwInHandler;

            Handler = new RecordingHandler(this);

            var services = new ServiceCollection();
            services.AddSingleton(Handler);
            _provider = services.BuildServiceProvider();

            var registry = new WechatPayCallbackHandlerRegistry();
            registry.Register("TRANSACTION.SUCCESS", typeof(RecordingHandler));

            _middleware = new WechatPayCallbackMiddleware(
                _ =>
                {
                    NextCalled = true;
                    return Task.CompletedTask;
                },
                Fixture.CreateReceiver(),
                new WechatPayCallbackDispatcher(
                    registry, _provider, Fixture.OptionsMonitor,
                    NullLogger<WechatPayCallbackDispatcher>.Instance),
                Fixture.OptionsMonitor,
                NullLogger<WechatPayCallbackMiddleware>.Instance);
        }

        public WechatPayCallbackTestFixture Fixture { get; }

        public RecordingHandler Handler { get; }

        public bool ThrowInHandler { get; }

        public bool NextCalled { get; private set; }

        public async Task<MiddlewareResponse> InvokeAsync(
            string method,
            WechatPayCallbackHeaders headers,
            string body,
            string merchantKey = WechatPayCallbackTestFixture.MerchantKey,
            string path = "",
            string contentType = "application/json")
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path.Length > 0 ? path : $"/pay/{merchantKey}";
            context.Request.ContentType = contentType;

            var bytes = Encoding.UTF8.GetBytes(body);
            context.Request.Body = new MemoryStream(bytes);
            context.Request.ContentLength = bytes.Length;

            SetHeader(context, "Wechatpay-Timestamp", headers.Timestamp);
            SetHeader(context, "Wechatpay-Nonce", headers.Nonce);
            SetHeader(context, "Wechatpay-Signature", headers.Signature);
            SetHeader(context, "Wechatpay-Serial", headers.SerialNumber);

            context.Response.Body = new MemoryStream();

            await _middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body, Encoding.UTF8).ReadToEndAsync();

            return new MiddlewareResponse(
                context.Response.StatusCode,
                responseBody,
                context.Response.ContentType ?? string.Empty,
                NextCalled);
        }

        public void Dispose() => _provider.Dispose();

        private static void SetHeader(DefaultHttpContext context, string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                context.Request.Headers[name] = value;
            }
        }
    }
}
