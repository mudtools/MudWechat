// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Mud.HttpUtils;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;

namespace Mud.Wechat.Work.Tests;

/// <summary>
/// Mud.HttpUtils 组件升级冒烟测试：
/// 验证 errcode 令牌失效恢复（Phase A）、共享令牌管理器标记（Phase B）
/// 与既有 SDK 契约在升级后的运行时行为。
/// </summary>
/// <remarks>
/// 历史沿革：2.0.10（开发中间态）→ 2.0.9（首个正式上架版）→ <b>3.0.0</b>（BC-27 多应用切换 API 收敛）。
/// </remarks>
public class MudHttpUtilsUpgradeSmokeTests
{
    private const string SuiteAccessQueryParam = "suite_access_token";
    private const string ErrcodeExpiredBody = """{"errcode":42001,"errmsg":"access_token expired"}""";
    private const string ErrcodeOkBody = """{"errcode":0,"errmsg":"ok"}""";

    #region 基建

    /// <summary>IOptionsMonitor 假实现（组件公共构造函数要求）。</summary>
    private sealed class FakeOptionsMonitor : IOptionsMonitor<TokenRecoveryOptions>
    {
        public TokenRecoveryOptions CurrentValue { get; init; } = new();
        public TokenRecoveryOptions Get(string? name) => CurrentValue;
        public IDisposable? OnChange(Action<TokenRecoveryOptions, string?> listener) => null;
    }

    /// <summary>模拟企业微信 errcode 失效判定器（Phase C C3 的测试替身，按 §11.2 形态）。</summary>
    private sealed class WechatErrcodeDetector : ITokenInvalidationDetector
    {
        private static readonly long[] InvalidErrcodes = { 40014, 42001, 42007, 42009, 42011 };

        public int IsTokenInvalidCalled;

        public bool ShouldInspect(HttpRequestMessage request)
            => request.RequestUri?.Host.EndsWith("weixin.qq.com", StringComparison.OrdinalIgnoreCase) == true;

        public ValueTask<bool> IsTokenInvalidAsync(
            HttpResponseMessage response, ReadOnlyMemory<byte>? body, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref IsTokenInvalidCalled);
            if (body is not { Length: > 0 } b)
                return ValueTask.FromResult(false);
            using var doc = JsonDocument.Parse(b);
            var errcode = doc.RootElement.TryGetProperty("errcode", out var e) ? e.GetInt64() : -1;
            return ValueTask.FromResult(Array.IndexOf(InvalidErrcodes, errcode) >= 0);
        }
    }

    /// <summary>共享令牌假管理器（Phase B：实现 ISharedTokenManager 即声明凭据无租户属性）。</summary>
    private sealed class SharedFakeTokenManager : TokenManagerBase, ISharedTokenManager
    {
        public override Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("shared-token");

        protected override Task<CredentialToken> RefreshTokenCoreAsync(CancellationToken cancellationToken)
            => Task.FromResult(new CredentialToken
            {
                AccessToken = "shared-token",
                Expire = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds(),
            });
    }

    /// <summary>非共享令牌假管理器（对照组：默认租户绑定守卫开启）。</summary>
    private sealed class TenantFakeTokenManager : TokenManagerBase
    {
        public override Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("tenant-token");

        protected override Task<CredentialToken> RefreshTokenCoreAsync(CancellationToken cancellationToken)
            => Task.FromResult(new CredentialToken
            {
                AccessToken = "tenant-token",
                Expire = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds(),
            });
    }

    /// <summary>构造企业微信形态请求：Query 注入 suite_access_token + 恢复上下文。</summary>
    private static HttpRequestMessage CreateWechatSuiteRequest(string staleToken = "stale-ticket")
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://qyapi.weixin.qq.com/cgi-bin/service/get_pre_auth_code?{SuiteAccessQueryParam}={staleToken}");
        request.Options.Set(new HttpRequestOptionsKey<TokenRecoveryContext>(TokenRecoveryContext.PropertyKey),
            new TokenRecoveryContext
            {
                InjectionMode = TokenInjectionMode.Query,
                QueryParameterName = SuiteAccessQueryParam,
                TokenManagerKey = WechatTokenTypes.SuiteAccessToken,
            });
        return request;
    }

    private static HttpResponseMessage Json(HttpStatusCode statusCode, string body) => new(statusCode)
    {
        Content = new StringContent(body, Encoding.UTF8, "application/json"),
    };

    #endregion

    /// <summary>
    /// V1：升级确实生效——运行时加载的是 <b>Mud.HttpUtils 3.0.0</b> 程序集
    /// （BC-27 已从 <c>IAppContextSwitcher</c> 移除 UseApp/UseDefaultApp/BeginScope(string)）。
    /// </summary>
    /// <remarks>
    /// 断言取「≥ 2.0.9」的语义下限 + 「Major ≥ 3」的升级事实：2.0.9 起含 errcode 恢复能力，
    /// 3.0.0 起含 BC-27 收敛。版本过渡期（例如回退到 2.0.x 排查问题）时本用例应显式失败，
    /// 以提醒同步回退 <c>IWechatAppContextSwitcher</c> 的接续声明。
    /// </remarks>
    [Fact]
    public void UpgradedComponent_ShouldBeAtLeast3_0_0()
    {
        var version = typeof(TokenRecoveryExecutor).Assembly.GetName().Version;
        version!.Should().NotBeNull();
        version.Should().BeGreaterThanOrEqualTo(new Version(2, 0, 9),
            "2.0.9 起含 errcode 令牌失效恢复能力（Phase A/B 的运行时前提）");
        version.Major.Should().BeGreaterThanOrEqualTo(3,
            "BC-27 适配后运行时必须是 3.0.0+（旧切换入口已从 IAppContextSwitcher 移除）");
    }

    /// <summary>
    /// V2（Phase A 核心）：HTTP 200 + errcode=42001 → 判定器判真 → 刷新 suite_access_token
    /// → Query 参数重新注入 → 重试成功。这是升级的主要目的。
    /// </summary>
    [Fact]
    public async Task ErrcodeInvalid_ShouldRefreshAndReinjectQueryToken()
    {
        var managerMock = new Mock<ITokenManager>();
        managerMock.Setup(m => m.GetOrRefreshTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("fresh-suite-token");

        var options = new TokenRecoveryOptions
        {
            RecoveryMaxRetries = 1,
            TokenInvalidationDetector = new WechatErrcodeDetector(),
        };
        var executor = new TokenRecoveryExecutor(managerMock.Object, new FakeOptionsMonitor { CurrentValue = options });

        var sentRequests = new List<HttpRequestMessage>();
        var request = CreateWechatSuiteRequest();
        var response = await executor.ExecuteAsync(
            request,
            (req, ct) =>
            {
                sentRequests.Add(req);
                return Task.FromResult(Json(HttpStatusCode.OK, ErrcodeExpiredBody));
            },
            CancellationToken.None);

        sentRequests.Count.Should().Be(2, "1 次原始发送 + 1 次恢复重试");
        managerMock.Verify(m => m.GetOrRefreshTokenAsync(It.IsAny<CancellationToken>()), Times.Once);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sentRequests[^1].RequestUri!.Query.Should().Contain($"{SuiteAccessQueryParam}=fresh-suite-token",
            "errcode 失效后应以刷新令牌重注入 Query 参数（suite_access_token）");
    }

    /// <summary>V3：errcode=0（业务成功）不触发恢复；响应体对调用方保持可读。</summary>
    [Fact]
    public async Task ErrcodeZero_ShouldNotRecoverAndKeepBodyReadable()
    {
        var managerMock = new Mock<ITokenManager>();
        var detector = new WechatErrcodeDetector();
        var options = new TokenRecoveryOptions
        {
            RecoveryMaxRetries = 1,
            TokenInvalidationDetector = detector,
        };
        var executor = new TokenRecoveryExecutor(managerMock.Object, new FakeOptionsMonitor { CurrentValue = options });

        var sendCount = 0;
        var request = CreateWechatSuiteRequest();
        var response = await executor.ExecuteAsync(
            request,
            (req, ct) =>
            {
                Interlocked.Increment(ref sendCount);
                return Task.FromResult(Json(HttpStatusCode.OK, ErrcodeOkBody));
            },
            CancellationToken.None);

        sendCount.Should().Be(1);
        managerMock.Verify(m => m.GetOrRefreshTokenAsync(It.IsAny<CancellationToken>()), Times.Never);
        detector.IsTokenInvalidCalled.Should().BeGreaterThan(0, "声明长度的 200 响应应被捕获并交给判定器");
        (await response.Content.ReadAsStringAsync(CancellationToken.None)).Should().Contain("\"errcode\":0");
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json",
            "响应体捕获替换后内容头应保持可读");
    }

    /// <summary>V4（Phase B）：ISharedTokenManager 标记使 EnforceTenantBinding 默认豁免；非标记管理器保持守卫。</summary>
    [Fact]
    public void SharedTokenManagerMarker_ShouldFlipTenantBindingDefault()
    {
        var bindingProperty = typeof(TokenManagerBase).GetProperty("EnforceTenantBinding",
            BindingFlags.NonPublic | BindingFlags.Instance);
        bindingProperty.Should().NotBeNull("Phase B 的判定点在 TokenManagerBase.EnforceTenantBinding");

        using var shared = new SharedFakeTokenManager();
        ((bool)bindingProperty!.GetValue(shared)!).Should().BeFalse(
            "实现 ISharedTokenManager 即声明凭据无租户属性（provider/suite 令牌跨 AppKey 共享）");

        using var tenant = new TenantFakeTokenManager();
        ((bool)bindingProperty.GetValue(tenant)!).Should().BeTrue("非标记管理器默认保留租户绑定守卫");
    }

    /// <summary>V5：SDK 自身契约不回归——WechatWorkResponse errcode JSON 契约与 WechatTokenTypes 常量。</summary>
    [Fact]
    public void WechatSdkContracts_ShouldRemainStable()
    {
        var resp = JsonSerializer.Deserialize<GetPreAuthCodeResponse>(
            """{"errcode":0,"errmsg":"ok","pre_auth_code":"pac-123","expires_in":7200}""");
        resp!.ErrorCode.Should().Be(0);
        resp.ErrorMessage.Should().Be("ok");
        resp.PreAuthCode.Should().Be("pac-123");

        WechatTokenTypes.AccessToken.Should().Be("Wechat.AccessToken");
        WechatTokenTypes.ProviderAccessToken.Should().Be("Wechat.ProviderAccessToken");
        WechatTokenTypes.SuiteAccessToken.Should().Be("Wechat.SuiteAccessToken");
    }
}
