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
using Microsoft.Extensions.Logging;
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
    /// <param name="configureOptions">回调配置定制（在默认通配应用之上追加/改写）。</param>
    /// <param name="configure">AddWechatCallback 建造者定制（注册处理器/拦截器）。</param>
    /// <param name="preRegister"><b>先于</b> AddWechatCallback 的服务预注册（TryAdd 语义：宿主预注册者按契约胜出，
    /// 用于注入自定义 <c>IWechatCallbackReplayGuard</c> 桩或接收器桩）。</param>
    /// <param name="logger">中间件日志器（默认 NullLogger；断言日志级别时传 <see cref="CapturingLogger{T}"/>）。</param>
    private static (WechatCallbackMiddleware Middleware, ServiceProvider Provider) CreateMiddleware(
        Action<WechatCallbackOptions>? configureOptions = null,
        Action<WechatCallbackServiceBuilder>? configure = null,
        Action<ServiceCollection>? preRegister = null,
        ILogger<WechatCallbackMiddleware>? logger = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IWechatSuiteTicketStore, InMemoryWechatSuiteTicketStore>();
        services.AddSingleton<IWechatCorpAuthStore, InMemoryWechatCorpAuthStore>();
        services.AddSingleton<IOptionsMonitor<WechatCallbackOptions>>(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions(configureOptions)));
        preRegister?.Invoke(services);

        var builder = services.AddWechatCallback(_ => { });
        configure?.Invoke(builder);

        var provider = services.BuildServiceProvider();
        var middleware = new WechatCallbackMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<IWechatCallbackReceiver>(),
            provider.GetRequiredService<WechatCallbackDispatcher>(),
            provider.GetRequiredService<IWechatBotCallbackReceiver>(),
            provider.GetRequiredService<WechatBotEventDispatcher>(),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            logger ?? NullLogger<WechatCallbackMiddleware>.Instance);

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

    private static string QueryFor(string encrypt, string? timestamp = null, string? nonce = null, string? token = null)
    {
        var ts = timestamp ?? Now();
        var nc = nonce ?? Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(token ?? Token, ts, nc, encrypt);
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
            provider.GetRequiredService<IWechatBotCallbackReceiver>(),
            provider.GetRequiredService<WechatBotEventDispatcher>(),
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
            provider.GetRequiredService<IWechatBotCallbackReceiver>(),
            provider.GetRequiredService<WechatBotEventDispatcher>(),
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
            provider.GetRequiredService<IWechatBotCallbackReceiver>(),
            provider.GetRequiredService<WechatBotEventDispatcher>(),
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
    public async Task InvokeAsync_ShouldReturn415_WhenPostBodyNotXmlNorJson()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var context = CreateContext("POST", "/wechat/app1", body: "plain", contentType: "text/plain");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(415, "回调报文为加密 XML 或加密 JSON，其余 Content-Type 一律 415");
    }

    /// <summary>
    /// v1.2：JSON 报文不再被 415 拒绝，而是进入智能机器人通道；
    /// 本用例的配置为 <b>App 通道</b>（默认），故 fail-closed 返回 403（Channel 错配）。
    /// </summary>
    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenJsonPostedToAppChannelEntry()
    {
        var (middleware, provider) = CreateMiddleware();
        using var _ = provider;
        var context = CreateContext(
            "POST", "/wechat/app1", query: "msg_signature=x&timestamp=1&nonce=n",
            body: "{\"encrypt\":\"x\"}", contentType: "application/json");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403,
            "JSON 回调只可能来自 Channel=Bot 的条目；App 条目收到 JSON 属配置错配（fail-closed）");
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

    [Theory]
    [InlineData(200, "success")]
    [InlineData(302, "success")]
    [InlineData(399, "success")]
    [InlineData(400, "rejected:400")]
    [InlineData(413, "rejected:413")]
    [InlineData(415, "rejected:415")]
    [InlineData(500, "rejected:500")]
    [InlineData(503, "rejected:503")]
    public void ResolveOutcome_ShouldBucketByStatusCode(int statusCode, string expected)
    {
        // R-P3-4（架构审查报告 v2 P3-4）：短路应答（413/415/400/500）不得再误记 success——
        // outcome 按最终状态码分桶（自由字符串维度），观测归真。
        WechatCallbackMiddleware.ResolveOutcome(statusCode).Should().Be(expected);
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

    /// <summary>
    /// v1.2 修复：仅前缀路由（<c>/{prefix}</c>，通讯录同步助手形态）必须按通配键解析——
    /// 中间件把空键归一为 <c>"*"</c>；否则分发器对空键 fail-fast（<c>ArgumentException</c> → 500），
    /// 症状为「GET 验签可通过、POST 事件恒 500」，通讯录同步助手事件永远无法送达（隐蔽且难排查）。
    /// </summary>
    [Fact]
    public async Task InvokeAsync_ShouldReturn200Success_WhenPostUsesPrefixOnlyRoute()
    {
        var (middleware, provider) = CreateMiddleware(
            configure: b => b.AddHandler<CollectingHandler>());
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain, path: "/wechat");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200, "仅前缀路由落通配键桶，不得因空键触发分发器 fail-fast");
        ResponseBody(context).Should().Be("success");
        CollectingHandler.LastUserId.Should().Be("zhangsan", "通配键处理器桶必须被枚举到");
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

    // ------------------------------------------------- 基础设施异常分流（审查报告 v1 P1-1）

    [Fact]
    public async Task InvokeAsync_ShouldReturn503AndLogError_WhenReplayGuardStorageFails()
    {
        // 分布式指纹守卫存储故障 → 专用 catch → 503 触发官方 96238 重推；
        // 严禁落入验证类 403（Redis 故障期事件会静默丢失）。日志级别对齐服务器侧故障（LogError）。
        var logger = new CapturingLogger<WechatCallbackMiddleware>();
        var (middleware, provider) = CreateMiddleware(
            preRegister: s => s.AddSingleton<IWechatCallbackReplayGuard>(
                new ThrowingReplayGuard(new WechatCallbackInfrastructureException("存储不可用",
                    new InvalidOperationException("模拟 Redis 故障")))),
            logger: logger);
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(503, "基础设施故障 → 503 触发企业微信重推");
        logger.Entries.Should().Contain(e => e.Level == LogLevel.Error,
            "服务器侧故障记 Error（区别于验证类的 Warning）");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenReplayGuardThrowsValidationException()
    {
        // 防收窄回归：验证类失败（WechatCallbackException）仍必须走 403 分支，
        // 不得被基础设施专用 catch 吞并（catch 顺序与类型过滤的双重保险）。
        var (middleware, provider) = CreateMiddleware(
            preRegister: s => s.AddSingleton<IWechatCallbackReplayGuard>(
                new ThrowingReplayGuard(
                    new WechatCallbackException(WechatCallbackFailureKind.ReplaySuspected, "指纹已消费"))));
        using var _ = provider;
        var context = CreatePostContext(UserCreatedPlain);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403, "验证类失败（重放）恒 403、不重推");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn503_WhenEchoPathHitsInfrastructureFailure()
    {
        // echo 正常路径不触达指纹闸（只过时效闸），本用例以桩接收器验证 catch 链对 echo 同样生效：
        // echo 期间的同类基础设施异常同样映射 503（共享同一异常映射面）。
        var (middleware, provider) = CreateMiddleware(
            preRegister: s => s.AddSingleton<IWechatCallbackReceiver>(new ThrowingEchoReceiver()));
        using var _ = provider;
        var context = CreateContext("GET", "/wechat/app1", query: "msg_signature=x&timestamp=0&nonce=n&echostr=e");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(503, "基础设施异常映射对 echo 路径同样生效");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn503_WhenBotChannelReplayGuardStorageFails()
    {
        // Bot JSON 通道与 XML 通道共用同一 catch 链与异常映射：指纹守卫存储故障 → 503。
        // （Bot 官方只推一次且指纹已消费，503 不改变丢失结局，但保证语义正确与可观测归因。）
        var botToken = "bot-push-token";
        var (middleware, provider) = CreateMiddleware(
            configureOptions: o => o.Apps["bot1"] = new WechatAppCallbackOptions
            {
                PushToken = botToken,
                PushEncodingAESKey = AesKey,
                AppType = WechatAppType.Internal,
                Channel = WechatCallbackChannel.Bot,
            },
            preRegister: s => s.AddSingleton<IWechatCallbackReplayGuard>(
                new ThrowingReplayGuard(new WechatCallbackInfrastructureException("存储不可用",
                    new InvalidOperationException("模拟 Redis 故障")))));
        using var _ = provider;
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, BotTextMessageJson(), receiveId: string.Empty);
        var body = "{\"encrypt\":\"" + encrypt + "\"}";
        var context = CreateContext(
            "POST", "/wechat/bot1",
            query: QueryFor(encrypt, token: botToken),
            body: body, contentType: "application/json");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(503, "Bot 通道与 XML 通道共用基础设施异常映射");
    }

    // ---------------------------------------------------------------- 测试替身

    /// <summary>Bot 通道官方形态的文本消息明文（与 WechatBotCallbackTests 同款最小报文）。</summary>
    private static string BotTextMessageJson()
        => "{\"msgid\":\"MSGID1\",\"aibotid\":\"BOTID\",\"chatid\":\"CHATID\",\"chattype\":\"single\"," +
           "\"from\":{\"userid\":\"zhangsan\"},\"msgtype\":\"text\",\"text\":{\"content\":\"hi\"}}";

    /// <summary>按设定抛出异常的重放守卫桩（TryAdd 语义下预注册即胜出，接管指纹闸）。</summary>
    private sealed class ThrowingReplayGuard : IWechatCallbackReplayGuard
    {
        private readonly Exception _exception;

        public ThrowingReplayGuard(Exception exception) => _exception = exception;

        public Task<bool> TryMarkAsync(string key, TimeSpan window, CancellationToken cancellationToken = default)
            => Task.FromException<bool>(_exception);
    }

    /// <summary>echo 抛基础设施异常的接收器桩（验证 catch 链对 echo 路径的覆盖）。</summary>
    private sealed class ThrowingEchoReceiver : IWechatCallbackReceiver
    {
        public Task<string> EchoAsync(string appKey, string urlQuery, CancellationToken cancellationToken = default)
            => Task.FromException<string>(new WechatCallbackInfrastructureException(
                "echo 期间基础设施故障", new InvalidOperationException("模拟存储故障")));

        public Task<WechatCallbackEvent> ReceiveAsync(
            string appKey, string urlQuery, string body, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("本桩仅覆盖 echo 路径");
    }

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
