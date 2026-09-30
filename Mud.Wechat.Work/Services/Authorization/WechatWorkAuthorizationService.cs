// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.Services.Authorization.Models;

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// 授权编排服务默认实现（Singleton；运行期经 <see cref="IWechatAppManager"/> 解析目标应用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>R7 关键约束</b>：所有需要 per-app 令牌的调用（<c>get_pre_auth_code</c> / <c>set_session_info</c> /
/// <c>get_permanent_code</c> / <c>get_auth_info</c> / <c>get_customized_auth_url</c>）必须包在
/// <c>IWechatAppContextSwitcher.BeginScope(appKey)</c> 内——声明式客户端的 <c>[Token]</c> 按
/// 「当前环境应用」解析令牌，不切上下文会取到错误应用的套件/服务商令牌（多套件场景静默串号）。
/// </para>
/// <para>
/// <b>R3 幂等</b>：<c>ExchangeAuthCodeAsync</c> 以 <c>authCode</c> 为粒度的进程内单飞门 + 结果记忆
/// （TTL 由 <see cref="WechatAuthorizationOptions.AuthCodeMemoTtlSeconds"/> 控制）实现幂等；
/// 换码前拿不到 <c>authCorpId</c>，故不做「按 authCorpId 串行化」。
/// </para>
/// </remarks>
internal sealed class WechatWorkAuthorizationService : IWechatWorkAuthorizationService
{
    private const string InstallUrlPrefix = "https://open.work.weixin.qq.com/3rdapp/install";
    private const int DefaultPreAuthCodeExpiresIn = 1200;
    private const int DefaultCustomizedUrlExpiresIn = 864000;

    private readonly IWechatAppManager _appManager;
    private readonly IWechatAppContextSwitcher _switcher;
    private readonly IWechatWorkProviderAuthenticationService _providerAuth;
    private readonly IWechatWorkProviderAuthenticationUrl _providerUrl;
    private readonly IWechatCorpAuthStore _authStore;
    private readonly IOptions<WechatAuthorizationOptions> _options;
    private readonly ILogger<WechatWorkAuthorizationService> _logger;

    /// <summary>换码单飞门（同一 authCode 的并发/重复调用收敛为一次 API 调用）。</summary>
    private readonly ConcurrentDictionary<string, Lazy<Task<WechatCorpAuthorization>>> _inFlight = new(StringComparer.Ordinal);

    /// <summary>换码结果记忆（authCode → authCorpId，短 TTL）。</summary>
    private readonly ConcurrentDictionary<string, AuthCodeMemo> _memo = new(StringComparer.Ordinal);

    /// <summary>创建授权编排服务。</summary>
    public WechatWorkAuthorizationService(
        IWechatAppManager appManager,
        IWechatAppContextSwitcher switcher,
        IWechatWorkProviderAuthenticationService providerAuth,
        IWechatWorkProviderAuthenticationUrl providerUrl,
        IWechatCorpAuthStore authStore,
        IOptions<WechatAuthorizationOptions> options,
        ILogger<WechatWorkAuthorizationService> logger)
    {
        _appManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        _switcher = switcher ?? throw new ArgumentNullException(nameof(switcher));
        _providerAuth = providerAuth ?? throw new ArgumentNullException(nameof(providerAuth));
        _providerUrl = providerUrl ?? throw new ArgumentNullException(nameof(providerUrl));
        _authStore = authStore ?? throw new ArgumentNullException(nameof(authStore));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<WechatAuthorizationUrl> CreateSuiteAuthorizationUrlAsync(
        WechatAuthorizationUrlRequest request, string? appKey = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var redirectUri = string.IsNullOrWhiteSpace(request.RedirectUri)
            ? _options.Value.AuthorizationRedirectUri
            : request.RedirectUri;
        if (string.IsNullOrWhiteSpace(redirectUri))
        {
            throw new InvalidOperationException(
                "缺少授权回跳地址：请在 WechatAuthorizationUrlRequest.RedirectUri 传入，或配置 " +
                $"WechatAuthorizationOptions.AuthorizationRedirectUri。");
        }

        if (!string.IsNullOrEmpty(request.State) && Encoding.UTF8.GetByteCount(request.State) > 128)
        {
            throw new ArgumentException("授权链接 state 不得超过 128 字节。", nameof(request));
        }

        var targetAppKey = ResolveAppKey(appKey);
        using var scope = _switcher.BeginScope(targetAppKey);

        var suiteId = _appManager.GetApp(targetAppKey).Config.SuiteId;

        var preAuth = await _providerAuth.GetPreAuthCodeAsync(cancellationToken).ConfigureAwait(false);
        WechatWorkException.ThrowIfFailed(preAuth);

        var preAuthCode = preAuth!.PreAuthCode;
        var expiresIn = preAuth.ExpiresIn > 0 ? preAuth.ExpiresIn : DefaultPreAuthCodeExpiresIn;

        if (request.SessionInfo != null)
        {
            var sessionInfo = new SetSessionInfoRequest
            {
                PreAuthCode = preAuthCode,
                Session = request.SessionInfo.Session,
            };

            var sessionResponse = await _providerAuth.SetSessionInfoAsync(sessionInfo, cancellationToken).ConfigureAwait(false);
            WechatWorkException.ThrowIfFailed(sessionResponse);
        }

        var url = new StringBuilder(InstallUrlPrefix)
            .Append("?suite_id=").Append(Uri.EscapeDataString(suiteId))
            .Append("&pre_auth_code=").Append(Uri.EscapeDataString(preAuthCode))
            .Append("&redirect_uri=").Append(Uri.EscapeDataString(redirectUri))
            .ToString();

        if (!string.IsNullOrEmpty(request.State))
        {
            url += "&state=" + Uri.EscapeDataString(request.State);
        }

        _logger.LogInformation(
            "已生成第三方应用授权安装链接（应用 {AppKey}，SuiteId {SuiteId}，有效期 {ExpiresIn}s）。",
            targetAppKey, suiteId, expiresIn);

        return new WechatAuthorizationUrl(url, preAuthCode, expiresIn, DateTimeOffset.UtcNow.AddSeconds(expiresIn));
    }

    /// <inheritdoc />
    public async Task<WechatCustomizedAuthUrl> CreateCustomizedAuthorizationUrlAsync(
        string state, IReadOnlyList<string> templateIdList, string? appKey = null, CancellationToken cancellationToken = default)
    {
        ValidateCustomizedState(state);
        if (templateIdList == null || templateIdList.Count == 0)
        {
            throw new ArgumentException("templateid_list 不能为空。", nameof(templateIdList));
        }

        if (templateIdList.Count > 9)
        {
            throw new ArgumentException("templateid_list 最多 9 个。", nameof(templateIdList));
        }

        if (templateIdList.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("templateid_list 不得包含空值。", nameof(templateIdList));
        }

        var targetAppKey = ResolveAppKey(appKey);
        using var scope = _switcher.BeginScope(targetAppKey);

        var context = _appManager.GetApp(targetAppKey);
        var providerTokenManager = context.ProviderTokenManager
            ?? throw new InvalidOperationException(
                $"应用 {targetAppKey} 未配置服务商令牌管理器（ProviderTokenManager），" +
                "代开发带参授权链接需要第三方/服务商（Provider）应用类型。");

        var providerAccessToken = await providerTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);

        var response = await _providerUrl.GetCustomizedAuthUrlAsync(
            providerAccessToken,
            new GetCustomizedAuthUrlRequest { State = state, TemplateIdList = templateIdList.ToList() },
            cancellationToken).ConfigureAwait(false);

        WechatWorkException.ThrowIfFailed(response);

        var qrcodeUrl = response!.QrcodeUrl;
        // 显式判空（而非 string.IsNullOrEmpty）：netstandard2.0 缺少 NotNullWhen 标注，流分析无法收窄。
        if (qrcodeUrl == null || qrcodeUrl.Length == 0)
        {
            throw new WechatWorkException(-1, "get_customized_auth_url 响应缺少 qrcode_url。");
        }

        var expiresIn = response.ExpiresIn > 0 ? response.ExpiresIn : DefaultCustomizedUrlExpiresIn;
        _logger.LogInformation(
            "已生成代开发带参授权链接（应用 {AppKey}，模板数 {Count}，有效期 {ExpiresIn}s）。",
            targetAppKey, templateIdList.Count, expiresIn);

        return new WechatCustomizedAuthUrl(qrcodeUrl, expiresIn, DateTimeOffset.UtcNow.AddSeconds(expiresIn));
    }

    /// <inheritdoc />
    public async Task<WechatCorpAuthorization> ExchangeAuthCodeAsync(
        string authCode, string? appKey = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authCode))
        {
            throw new ArgumentException("authCode 不能为空。", nameof(authCode));
        }

        var targetAppKey = ResolveAppKey(appKey);

        // 结果记忆命中：避免对已消费的一次性 auth_code 重复换码。
        if (_memo.TryGetValue(authCode, out var memo) && memo.ExpiresAt > DateTimeOffset.UtcNow)
        {
            var remembered = await _authStore.GetAsync(targetAppKey, memo.AuthCorpId, cancellationToken).ConfigureAwait(false);
            if (remembered != null)
            {
                return remembered;
            }
        }

        var lazy = _inFlight.GetOrAdd(
            authCode,
            code => new Lazy<Task<WechatCorpAuthorization>>(
                () => ExchangeCoreAsync(code, targetAppKey, cancellationToken),
                LazyThreadSafetyMode.ExecutionAndPublication));

        try
        {
            return await lazy.Value.ConfigureAwait(false);
        }
        finally
        {
            // 失败 / 完成即释放单飞门（成功结果由结果记忆 + 仓储承载）。
            _inFlight.TryRemove(authCode, out _);
        }
    }

    /// <inheritdoc />
    public async Task<WechatCorpAuthorization> RefreshAuthorizationAsync(
        string authCorpId, string? appKey = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authCorpId))
        {
            throw new ArgumentException("authCorpId 不能为空。", nameof(authCorpId));
        }

        var targetAppKey = ResolveAppKey(appKey);

        var existing = await _authStore.GetAsync(targetAppKey, authCorpId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"未找到应用 {targetAppKey} 下企业 {authCorpId} 的授权记录，无法刷新授权信息。");

        using var scope = _switcher.BeginScope(targetAppKey);

        var request = new GetAuthInfoRequest
        {
            AuthorizerCorpId = authCorpId,
            PermanentAuthCode = existing.PermanentCode,
        };

        var response = _options.Value.UseV2AuthApi
            ? await _providerAuth.GetAuthInfoV2Async(request, cancellationToken).ConfigureAwait(false)
            : await _providerAuth.GetAuthInfoAsync(request, cancellationToken).ConfigureAwait(false);

        WechatWorkException.ThrowIfFailed(response);

        WechatAuthorizationMapper.ApplyAuthInfo(existing, response!, Now());
        await _authStore.SetAsync(existing, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("已刷新企业授权信息（应用 {AppKey}，AuthCorpId {AuthCorpId}）。", targetAppKey, authCorpId);
        return existing;
    }

    /// <inheritdoc />
    public Task<WechatCorpAuthorization?> GetAuthorizationAsync(
        string authCorpId, string? appKey = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authCorpId))
        {
            throw new ArgumentException("authCorpId 不能为空。", nameof(authCorpId));
        }

        return _authStore.GetAsync(ResolveAppKey(appKey), authCorpId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<WechatCorpAuthorization>> ListAuthorizationsAsync(
        string? appKey = null, CancellationToken cancellationToken = default)
        => _authStore.ListAsync(ResolveAppKey(appKey), cancellationToken);

    /// <inheritdoc />
    public async Task RevokeAuthorizationAsync(
        string authCorpId, string? appKey = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authCorpId))
        {
            throw new ArgumentException("authCorpId 不能为空。", nameof(authCorpId));
        }

        var targetAppKey = ResolveAppKey(appKey);

        await _authStore.RemoveAsync(targetAppKey, authCorpId, cancellationToken).ConfigureAwait(false);

        // R4：仅失效本 AppKey 的该企业令牌（同一 authCorpId 在其它套件下是独立授权，不得连带失效）。
        await _appManager.InvalidateTokenAsync(
            targetAppKey, WechatTokenTypes.AccessToken, new[] { authCorpId }, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("已撤销企业授权（应用 {AppKey}，AuthCorpId {AuthCorpId}）。", targetAppKey, authCorpId);
    }

    private async Task<WechatCorpAuthorization> ExchangeCoreAsync(
        string authCode, string appKey, CancellationToken cancellationToken)
    {
        using var scope = _switcher.BeginScope(appKey);

        var request = new GetPermanentCodeRequest { TempAuthCode = authCode };
        var response = _options.Value.UseV2AuthApi
            ? await _providerAuth.GetPermanentCodeV2Async(request, cancellationToken).ConfigureAwait(false)
            : await _providerAuth.GetPermanentCodeAsync(request, cancellationToken).ConfigureAwait(false);

        WechatWorkException.ThrowIfFailed(response);

        var auth = WechatAuthorizationMapper.FromPermanentCode(appKey, response!, Now());
        if (string.IsNullOrEmpty(auth.AuthCorpId))
        {
            throw new WechatWorkException(-1, "get_permanent_code 响应缺少 auth_corp_info.corpid，无法定位授权企业。");
        }

        if (string.IsNullOrEmpty(auth.PermanentCode))
        {
            throw new WechatWorkException(-1, "get_permanent_code 响应缺少 permanent_code。");
        }

        await _authStore.SetAsync(auth, cancellationToken).ConfigureAwait(false);
        RememberAuthCode(authCode, auth.AuthCorpId);

        _logger.LogInformation(
            "授权换码成功并已落库（应用 {AppKey}，AuthCorpId {AuthCorpId}，代开发 {IsCustomizedApp}）。",
            appKey, auth.AuthCorpId, auth.IsCustomizedApp);
        return auth;
    }

    private void RememberAuthCode(string authCode, string authCorpId)
    {
        var ttl = _options.Value.AuthCodeMemoTtlSeconds;
        if (ttl <= 0)
        {
            return;
        }

        _memo[authCode] = new AuthCodeMemo(authCorpId, DateTimeOffset.UtcNow.AddSeconds(ttl));

        // 顺带清理过期项，避免长时间运行下无界增长。
        var now = DateTimeOffset.UtcNow;
        foreach (var pair in _memo)
        {
            if (pair.Value.ExpiresAt <= now)
            {
                _memo.TryRemove(pair.Key, out _);
            }
        }
    }

    private string ResolveAppKey(string? appKey)
    {
        // `is { Length: > 0 }` 用于在 netstandard2.0 下把可空引用收窄为非空（该 TFM 无 NotNullWhen 标注）。
        if (appKey is { Length: > 0 } && !string.IsNullOrWhiteSpace(appKey))
        {
            return appKey;
        }

        var configured = _options.Value.DefaultAppKey;
        if (configured is { Length: > 0 } && !string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var defaultAppKey = _appManager.DefaultConfig.AppKey;
        if (string.IsNullOrEmpty(defaultAppKey))
        {
            throw new InvalidOperationException("无法解析目标应用键：未显式传入 appKey、未配置 WechatAuthorizationOptions.DefaultAppKey，且默认应用键为空。");
        }

        return defaultAppKey;
    }

    private static void ValidateCustomizedState(string state)
    {
        if (string.IsNullOrEmpty(state))
        {
            throw new ArgumentException("state 不能为空。", nameof(state));
        }

        if (Encoding.UTF8.GetByteCount(state) > 32)
        {
            throw new ArgumentException("state 不得超过 32 字节。", nameof(state));
        }

        foreach (var ch in state)
        {
            var allowed = (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9');
            if (!allowed)
            {
                throw new ArgumentException($"state 仅允许 a-zA-Z0-9，含非法字符 '{ch}'。", nameof(state));
            }
        }
    }

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private sealed class AuthCodeMemo
    {
        public AuthCodeMemo(string authCorpId, DateTimeOffset expiresAt)
        {
            AuthCorpId = authCorpId;
            ExpiresAt = expiresAt;
        }

        public string AuthCorpId { get; }

        public DateTimeOffset ExpiresAt { get; }
    }
}