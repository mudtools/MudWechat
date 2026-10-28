// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;

namespace Mud.Wechat.Work.Tests.TokenManagers;

/// <summary>
/// WechatTokenInvalidationDetector 两阶段判定测试（详细设计 §11.2 / §18.2）。
/// </summary>
public class WechatTokenInvalidationDetectorTests
{
    private readonly WechatTokenInvalidationDetector _detector = new();

    private static HttpRequestMessage WechatRequest()
        => new(HttpMethod.Post, "https://qyapi.weixin.qq.com/cgi-bin/service/get_pre_auth_code");

    private static HttpResponseMessage WechatJsonResponse(string body)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

    [Theory]
    [InlineData(40014, true)]
    [InlineData(42001, true)]
    [InlineData(42007, true)]
    [InlineData(42009, true)]
    [InlineData(42011, true)]
    [InlineData(0, false)]
    [InlineData(40013, false, "非失效码走业务异常，不触发恢复链路")]
    [InlineData(61028, false)]
    public async Task IsTokenInvalidAsync_ShouldMatchErrcodeCollection(int errcode, bool expected, string? because = null)
    {
        var body = Encoding.UTF8.GetBytes($"{{\"errcode\":{errcode},\"errmsg\":\"x\"}}");
        var result = await _detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);
        result.Should().Be(expected, because);
    }

    [Fact]
    public async Task IsTokenInvalidAsync_ShouldReturnFalse_WhenBodyUnreadable()
    {
        // body = null（超限 / chunked / 未声明 Content-Length）→ 回退状态码判定（返回 false）。
        var result = await _detector.IsTokenInvalidAsync(WechatJsonResponse(""), null, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsTokenInvalidAsync_ShouldReturnFalse_WhenBodyNotJson()
    {
        var body = "<html>gateway error</html>"u8.ToArray();
        var result = await _detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);
        result.Should().BeFalse("非 JSON 响应体不参与判定（异常降级语义）");
    }

    [Fact]
    public void ShouldInspect_ShouldFilterNonWechatHosts()
    {
        _detector.ShouldInspect(WechatRequest()).Should().BeTrue();
        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://open.feishu.cn/open-apis")).Should().BeFalse();
        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://qyapi.weixin.qq.com")).Should().BeTrue();
        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://evil.example.com/u/weixin.qq.com")).Should().BeFalse("仅按 host 判定");
    }

    /// <summary>
    /// P2-9：显式登记的自定义 BaseUrl 主机（<c>AllowCustomBaseUrl = true</c> 的私有化/网关部署）
    /// 也必须参与判定，否则这些部署会静默失去 errcode 令牌恢复能力。
    /// </summary>
    [Fact]
    public void ShouldInspect_ShouldPass_WhenCustomBaseUrlRegistered()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig>
        {
            new()
            {
                AppKey = "private",
                AppType = WechatAppType.Internal,
                CorpId = "ww-corp",
                AgentSecret = "agent-secret",
                BaseUrl = "https://gateway.example.com",
                AllowCustomBaseUrl = true,
            },
        });

        // 自定义主机在 **DI 注册期**（AddWechatApp）登记，无需物化应用上下文
        // （物化会触发组件连接期 SSRF 校验，测试环境无法解析公网域名）。
        using var provider = services.BuildServiceProvider();

        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://gateway.example.com/cgi-bin/gettoken"))
            .Should().BeTrue("注册期登记的自定义主机应放行同步预过滤");

        _detector.ShouldInspect(new HttpRequestMessage(HttpMethod.Get, "https://other.example.com/cgi-bin/gettoken"))
            .Should().BeFalse("未登记的第三方主机仍被过滤");
    }

    // ------------------------------------------------------------------ F1：corp 槽补偿失效

    private const string CorpAppKey = "custom-agent";

    private static CorpTokenManager CreateCorpManager(Mock<IWechatWorkCorpTokenAuthentication> corpAuth)
    {
        var suite = new Mock<IWechatSuiteTokenManager>();
        suite.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("suite-token");
        return new CorpTokenManager(
            corpAuth.Object, suite.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            new InMemoryWechatCorpAuthStore(),
            Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
            {
                AppKey = CorpAppKey,
                AppType = WechatAppType.ThirdParty,
                CorpId = "ww-provider",
                ProviderSecret = "provider-secret",
                SuiteId = "ww-suite",
                SuiteSecret = "suite-secret",
                AgentSecret = "agent-secret",
            }),
            NullLogger<CorpTokenManager>.Instance);
    }

    /// <summary>按 authCorpId 可重复刷新的 corp 令牌客户端 mock。</summary>
    private static Mock<IWechatWorkCorpTokenAuthentication> CorpAuthReturning(string accessToken)
    {
        var corpAuth = new Mock<IWechatWorkCorpTokenAuthentication>();
        corpAuth
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = accessToken, ExpiresIn = 7200 });
        return corpAuth;
    }

    /// <summary>构造注入版判定器：holder.Current → mock IWechatAppContext → 给定 corp 管理器。</summary>
    private static WechatTokenInvalidationDetector CreateDetectorWithAmbientCorpContext(IWechatCorpTokenManager? corpManager)
    {
        var contextMock = new Mock<IWechatAppContext>();
        contextMock.SetupGet(c => c.AppKey).Returns(CorpAppKey);
        contextMock.SetupGet(c => c.AppType).Returns(WechatAppType.ThirdParty);
        contextMock.SetupGet(c => c.CorpTokenManager).Returns(corpManager);

        var holderMock = new Mock<IAppContextHolder>();
        holderMock.SetupGet(h => h.Current).Returns(contextMock.Object);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(holderMock.Object);
        return new WechatTokenInvalidationDetector(services.BuildServiceProvider());
    }

    [Fact]
    public async Task IsTokenInvalidAsync_ShouldEvictCorpScopedCache_WhenAccessTokenFamilyErrcodeInCorpScope()
    {
        var corpAuth = CorpAuthReturning("token-refreshed");
        using var manager = CreateCorpManager(corpAuth);
        var detector = CreateDetectorWithAmbientCorpContext(manager);

        WechatCorpContext.SetCorp(CorpAppKey, "corp-A", "pc-A");
        try
        {
            var stale = await manager.GetTokenAsync();
            stale.Should().NotBeNullOrEmpty("前置：corp 槽已有缓存令牌");

            var body = Encoding.UTF8.GetBytes("""{"errcode":40014,"errmsg":"invalid access_token"}""");
            var invalid = await detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);

            invalid.Should().BeTrue("40014 判真语义不变");

            // 补偿已清 corp 槽 ⇒ 下次取令牌触发真实刷新（修复前：corp 槽未清，缓存命中旧令牌）。
            var refreshed = await manager.GetTokenAsync();
            refreshed.Should().Be("token-refreshed", "补偿失效已清 corp 槽 ⇒ GetTokenAsync 必须走真实刷新");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2),
                "冷启动刷新一次 + 补偿失效后真实刷新一次（若补偿未生效则为 1 次——corp 槽缓存命中）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    [Theory]
    [InlineData(42009)]
    [InlineData(42011)]
    public async Task IsTokenInvalidAsync_ShouldKeepCorpCache_WhenSuiteOrProviderErrcode(int errcode)
    {
        var corpAuth = CorpAuthReturning("token-refreshed");
        using var manager = CreateCorpManager(corpAuth);
        var detector = CreateDetectorWithAmbientCorpContext(manager);

        WechatCorpContext.SetCorp(CorpAppKey, "corp-A", "pc-A");
        try
        {
            await manager.GetTokenAsync();

            var body = Encoding.UTF8.GetBytes($"{{\"errcode\":{errcode},\"errmsg\":\"x\"}}");
            var invalid = await detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);

            invalid.Should().BeTrue("判定语义不变（42009/42011 仍在失效码集合）");

            // 族别门控：suite/provider 令牌恰在 default 槽，corp 槽不得被补偿触碰。
            await manager.GetTokenAsync();
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Once,
                "corp 槽必须保持缓存命中（族别精确性：42009/42011 不触发补偿）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>坏 IServiceProvider（GetService 即抛）⇒ 补偿失败退化为现状，判定结果仍为 true、不抛。</summary>
    [Fact]
    public async Task IsTokenInvalidAsync_ShouldStillReturnTrue_WhenCompensationFails()
    {
        var brokenServices = new ServiceCollection()
            .AddSingleton<IAppContextHolder>(new ThrowingAppContextHolder())
            .BuildServiceProvider();
        var detector = new WechatTokenInvalidationDetector(brokenServices);

        var body = Encoding.UTF8.GetBytes("""{"errcode":40014,"errmsg":"invalid access_token"}""");
        var act = async () => await detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);

        (await act.Should().NotThrowAsync()).Subject.Should().BeTrue(
            "补偿失效失败不得影响判定结果（检测故障不得放大为调用失败）");
    }

    [Fact]
    public async Task IsTokenInvalidAsync_ShouldSkipCompensation_WhenNoAmbientApp()
    {
        var corpAuth = CorpAuthReturning("token-refreshed");
        using var manager = CreateCorpManager(corpAuth);

        // holder 存在但 Current 为 null（无环境应用上下文）。
        var holderMock = new Mock<IAppContextHolder>();
        holderMock.SetupGet(h => h.Current).Returns((IMudAppContext?)null);
        var services = new ServiceCollection();
        services.AddSingleton(holderMock.Object);
        var detector = new WechatTokenInvalidationDetector(services.BuildServiceProvider());

        var body = Encoding.UTF8.GetBytes("""{"errcode":40014,"errmsg":"invalid access_token"}""");
        var invalid = await detector.IsTokenInvalidAsync(WechatJsonResponse(""), body, CancellationToken.None);

        invalid.Should().BeTrue();
        corpAuth.Verify(
            c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "无环境上下文时不得触达任何管理器（静默跳过）");
    }

    /// <summary>GetService 即抛的 IAppContextHolder（补偿故障隔离用例的注入桩）。</summary>
    private sealed class ThrowingAppContextHolder : IAppContextHolder
    {
        public IMudAppContext? Current { get => throw new InvalidOperationException("holder broken"); init => throw new InvalidOperationException("holder broken"); }

        public void SwitchTo(IMudAppContext? context) => throw new InvalidOperationException("holder broken");

        public IDisposable BeginScope(IMudAppContext context) => throw new InvalidOperationException("holder broken");
    }
}
