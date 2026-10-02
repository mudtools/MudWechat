// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 中间件端到端测试（v1 方案 §5.3 决策表 / §8，DefaultHttpContext 直驱）：
/// 路由提取、未知应用跳过、方法 405、Content-Type 415、体长 413、IP 白名单 403、
/// GET echo 200 回显、POST 200 success、重放 403、拦截器中断 503。
/// </summary>
public class WechatCallbackMiddlewareTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "push-token";
    private const string CorpId = "ww-corp";

    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    private static WechatCallbackOptions CreateOptions(Action<WechatCallbackOptions>? configure = null)
    {
        var options = new WechatCallbackOptions();
        options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions
        {
            PushToken = Token,
            PushEncodingAESKey = AesKey,
            ReceiveId = CorpId,
        };
        configure?.Invoke(options);
        return options;
    }

    /// <summary>经真实 AddWechatCallback 装配（接收器 + 分发器 + 注册表），构造被测中间件。</summary>
    private static (WechatCallbackMiddleware Middleware, ServiceProvider Provider) CreateMiddleware(
        Action<WechatCallbackOptions>? configureOptions = null,
        Action<WechatCallbackServiceBuilder>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IWechatSuiteTicketStore, InMemoryWechatSuiteTicketStore>();
        services.AddSingleton<IWechatCorpAuthStore, InMemoryWechatCorpAuthStore>();
        services.AddSingleton<IOptionsMonitor<WechatCallbackOptions>>(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions(configureOptions)));

        var builder = services.AddWechatCallback(_ => { });
        configure?.Invoke(builder);

        var provider = services.BuildServiceProvider();
        var middleware = new WechatCallbackMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<IWechatCallbackReceiver>(),
            provider.GetRequiredService<WechatCallbackDispatcher>(),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            NullLogger<WechatCallbackMiddleware>.Instance);

        return (middleware, provider);
    }

    private static DefaultHttpContext CreateContext(
        string method, string path, string? query = null,
        string? body = null, string? contentType = null, IPAddress? remoteIp = null)
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

        context.Connection.RemoteIpAddress = remoteIp ?? IPAddress.Parse("127.0.0.1");
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static string ResponseBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static string QueryFor(string encrypt, string? timestamp = null, string? nonce = null)
    {
        var ts = timestamp ?? Now();
        var nc = nonce ?? Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(Token, ts, nc, encrypt);
        return $"msg_signature={signature}&timestamp={ts}&nonce={nc}";
    }

    private static string EncryptedEventBody(string encrypt)
        => $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

    /// <summary>
    /// 单次加密构造 POST 上下文（Encrypt 为随机前缀加密——签名与请求体<b>必须</b>复用同一密文）。
    /// </summary>
    private static DefaultHttpContext CreatePostContext(string plainXml, string path = "/wechat/app1")
    {
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, CorpId);
        return CreateContext("POST", path, query: QueryFor(encrypt), body: EncryptedEventBody(encrypt), contentType: "text/xml");
    }

    /// <summary>构造 echo 查询串（echostr 充当 encrypt 参与签名，且必须出现在查询串中）。</summary>
    private static string EchoQueryFor(string echoStr)
    {
        var ts = Now();
        var nc = Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(Token, ts, nc, echoStr);
        return $"msg_signature={signature}&timestamp={ts}&nonce={nc}&echostr={Uri.EscapeDataString(echoStr)}";
    }

    private const string UserCreatedPlain =
        "<xml><MsgType><![CDATA[event]]></MsgType><Event><![CDATA[change_contact]]></Event>" +
        "<ChangeType><![CDATA[create_user]]></ChangeType><UserID><![CDATA[zhangsan]]></UserID></xml>";

    // ---------------------------------------------------------------- 路由

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenRoutePrefixMismatched()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var nextCalled = false;
        var middlewareWithProbe = new WechatCallbackMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            provider.GetRequiredService<IWechatCallbackReceiver>(),
            provider.GetRequiredService<WechatCallbackDispatcher>(),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            NullLogger<WechatCallbackMiddleware>.Instance);
        var context = CreateContext("GET", "/other/app1");

        await middlewareWithProbe.InvokeAsync(context);

        nextCalled.Should().BeTrue("非回调路由不打劫");
        context.Response.StatusCode.Should().Be(200, "默认状态未被改动");
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenDeepPathUnderPrefix()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var nextCalled = false;
        var middlewareWithProbe = new WechatCallbackMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            provider.GetRequiredService<IWechatCallbackReceiver>(),
            provider.GetRequiredService<WechatCallbackDispatcher>(),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            NullLogger<WechatCallbackMiddleware>.Instance);
        var context = CreateContext("POST", "/wechat/app1/extra");

        await middlewareWithProbe.InvokeAsync(context);

        nextCalled.Should().BeTrue("深层路径非本中间件职责（仅 /{prefix}/{appKey} 两段）");
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenAppUnknownAndNoWildcard()
    {
        var (middleware, provider) = CreateMiddleware(o =>
        {
            o.Apps.Remove(WechatCallbackOptions.WildcardAppKey);
            o.Apps["app1"] = new WechatAppCallbackOptions { PushToken = Token, PushEncodingAESKey = AesKey };
        });
        using var _ = provider;
        var nextCalled = false;
        var middlewareWithProbe = new WechatCallbackMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            provider.GetRequiredService<IWechatCallbackReceiver>(),
            provider.GetRequiredService<WechatCallbackDispatcher>(),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            NullLogger<WechatCallbackMiddleware>.Instance);
        var context = CreateContext("GET", "/wechat/unknown-app");

        await middlewareWithProbe.InvokeAsync(context);

        nextCalled.Should().BeTrue("未知 AppKey 且无通配时跳过（404 由末端给出）");
    }

    // ---------------------------------------------------------------- 方法 / Content-Type / 体长

    [Fact]
    public async Task InvokeAsync_ShouldReturn405_WhenMethodNotGetOrPost()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var context = CreateContext("PUT", "/wechat/app1");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(405, "回调协议方法固定 GET/POST（v1.2 删除 AllowedHttpMethods）");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn415_WhenPostBodyNotXml()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var context = CreateContext("POST", "/wechat/app1", body: "{}", contentType: "application/json");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(415, "回调报文为 XML 非 JSON");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn413_WhenBodyExceedsLimit()
    {
        var (middleware, provider) = CreateMiddleware(o => o.MaxRequestBodySize = 16);
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(413, "体长逐块按字节计数校验");
    }

    // ---------------------------------------------------------------- IP 白名单

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenIpNotInWhitelist()
    {
        var (middleware, provider) = CreateMiddleware(o => o.AllowedSourceIPs.Add("10.0.0.0/8"));
        using var _ = provider;
        var context = CreateContext("GET", "/wechat/app1", remoteIp: IPAddress.Parse("192.168.1.1"));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403, "白名单非空且客户端不在列");
    }

    [Fact]
    public async Task InvokeAsync_ShouldPass_WhenIpInCidrWhitelist()
    {
        var (middleware, provider) = CreateMiddleware(o => o.AllowedSourceIPs.Add("10.0.0.0/8"));
        using var _ = provider;
        var context = CreateContext(
            "GET", "/wechat/app1",
            query: EchoQueryFor(WechatCallbackCrypto.Encrypt(AesKey, "echo-ip", CorpId)),
            remoteIp: IPAddress.Parse("10.1.2.3"));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200, "IPv4 CIDR 段命中放行");
        ResponseBody(context).Should().Be("echo-ip");
    }

    // ---------------------------------------------------------------- GET echo / POST 接收

    [Fact]
    public async Task InvokeAsync_ShouldEchoPlain_With200()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var context = CreateContext(
            "GET", "/wechat/app1",
            query: EchoQueryFor(WechatCallbackCrypto.Encrypt(AesKey, "echo-str-1", CorpId)));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
        ResponseBody(context).Should().Be("echo-str-1", "URL 验证回显明文（无引号/BOM/换行）");
        context.Response.ContentType.Should().Contain("text/plain");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenEchoSignatureInvalid()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var echoStr = WechatCallbackCrypto.Encrypt(AesKey, "x", CorpId);
        var context = CreateContext(
            "GET", "/wechat/app1",
            query: $"msg_signature=0000000000000000000000000000000000000000&timestamp={Now()}&nonce=n&echostr={Uri.EscapeDataString(echoStr)}");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403, "验签失败 fail-closed");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn200Success_AndDispatchHandler_WhenPostEventValid()
    {
        var (middleware, provider) = CreateMiddleware(
            configure: b => b.AddHandler<CollectingHandler>());
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
        ResponseBody(context).Should().Be("success", "官方推荐应答体");
        CollectingHandler.LastUserId.Should().Be("zhangsan", "类型化处理器经分发执行");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenPostReplayed()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var plain = UserCreatedPlain;
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plain, CorpId);
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";
        var nonce = "same-nonce";
        var query = QueryFor(encrypt, nonce: nonce);

        var first = CreateContext("POST", "/wechat/app1", query: query, body: body, contentType: "text/xml");
        await middleware.InvokeAsync(first);
        first.Response.StatusCode.Should().Be(200);

        var replay = CreateContext("POST", "/wechat/app1", query: query, body: body, contentType: "text/xml");
        await middleware.InvokeAsync(replay);

        replay.Response.StatusCode.Should().Be(403, "P0-2：同指纹第二击判重放拒绝（fail-closed）");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn503_WhenInterceptorInterrupts()
    {
        var (middleware, provider) = CreateMiddleware(
            configure: b => b.AddInterceptor<InterruptingInterceptor>());
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(503, "拦截器中断 → 503 触发企业微信重推");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn200_WhenUnhandledEvent()
    {
        // 无精确处理器：内置兜底处理器接管并输出 unhandled 告警——应答仍 200（事件已接收）。
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200, "unhandled 已由兜底处理器告警，应答不受影响");
        ResponseBody(context).Should().Be("success");
    }

    // ---------------------------------------------------------------- 测试替身

    /// <summary>收集 UserId 的类型化处理器（端到端验证分发链路）。</summary>
    private sealed class CollectingHandler : IWechatCallbackEventHandler
    {
        /// <summary>最近处理的 UserId（静态：处理器实例为 Transient）。</summary>
        public static string? LastUserId { get; private set; }

        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            LastUserId = eventData.DecryptedXml?.Contains("zhangsan") == true ? "zhangsan" : "other";
            return Task.CompletedTask;
        }
    }

    /// <summary>中断拦截器（Before false）。</summary>
    private sealed class InterruptingInterceptor : IWechatCallbackEventInterceptor
    {
        public Task<bool> BeforeHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task AfterHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
