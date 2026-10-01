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
/// <c>IWechatAppContextSwitcher.UseAppScope(appKey)</c> 内（原 <c>BeginScope(appKey)</c>，Mud.HttpUtils 3.0.0 更名为推荐入口）
/// ——声明式客户端的 <c>[Token]</c> 按
/// 「当前环境应用」解析令牌，不切上下文会取到错误应用的套件/服务商令牌（多套件场景静默串号）。
/// </para>
/// <para>
/// <b>R3 幂等</b>：<c>ExchangeAuthCodeAsync</c> 以 <c>(appKey, authCode)</c> 复合键为粒度的进程内单飞门 +
/// 结果记忆（TTL 由 <see cref="WechatAuthorizationOptions.AuthCodeMemoTtlSeconds"/> 控制）实现幂等；
/// 换码前拿不到 <c>authCorpId</c>，故不做「按 authCorpId 串行化」。
/// </para>
/// <para>
/// <b>P1-3/P1-4 为何键必须含 appKey</b>：<c>authCode</c> 是<b>套件维度</b>的一次性码，同一
/// <c>authCode</c> 在不同 <c>appKey</c>（多套件/多代开发模板）下语义不同。原实现仅以 <c>authCode</c> 为键，
/// 会让"先到者"的换码结果（含其 <c>authCorpId</c>/<c>permanent_code</c>）被后到者<b>静默复用</b>，
/// 即向 A 应用写入 B 应用的授权对象。
/// </para>
/// </remarks>
internal sealed class WechatWorkAuthorizationService : IWechatWorkAuthorizationService
{
    private const string InstallUrlPrefix = "https://open.work.weixin.qq.com/3rdapp/install";
    private const int DefaultPreAuthCodeExpiresIn = 1200;
    private const int DefaultCustomizedUrlExpiresIn = 864000;

    /// <summary>结果记忆清理间隔（每 N 次写入触发一次过期回收，消除批量导入下的 O(n²)）。</summary>
    private const int MemoCleanupInterval = 64;

    private readonly IWechatAppManager _appManager;
    private readonly IWechatAppContextSwitcher _switcher;
    private readonly IWechatWorkProviderAuthenticationService _providerAuth;
    private readonly IWechatWorkProviderAuthenticationUrl _providerUrl;
    private readonly IWechatCorpAuthStore _authStore;
    private readonly IOptions<WechatAuthorizationOptions> _options;
    private readonly ILogger<WechatWorkAuthorizationService> _logger;

    /// <summary>换码单飞门（同一 <c>(appKey, authCode)</c> 的并发/重复调用收敛为一次 API 调用）。</summary>
    private readonly ConcurrentDictionary<string, Lazy<Task<WechatCorpAuthorization>>> _inFlight = new(StringComparer.Ordinal);

    /// <summary>换码结果记忆（复合键 → authCorpId，短 TTL）。</summary>
    private readonly ConcurrentDictionary<string, AuthCodeMemo> _memo = new(StringComparer.Ordinal);

    /// <summary>结果记忆写入计数（驱动阈值式过期回收）。</summary>
    private int _memoWrites;

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
        // Mud.HttpUtils 3.0.0 迁移（BC-27）：BeginScope(appKey) → UseAppScope(appKey)。
        // 二者在切换器内为同一实现（含守卫与「释放时自动归还上下文」），本处为逐字等价替换。
        using var scope = _switcher.UseAppScope(targetAppKey);

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
        // Mud.HttpUtils 3.0.0 迁移（BC-27）：BeginScope(appKey) → UseAppScope(appKey)。
        // 二者在切换器内为同一实现（含守卫与「释放时自动归还上下文」），本处为逐字等价替换。
        using var scope = _switcher.UseAppScope(targetAppKey);

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
        var flightKey = BuildFlightKey(targetAppKey, authCode);

        // 结果记忆命中：避免对已消费的一次性 auth_code 重复换码。
        if (_memo.TryGetValue(flightKey, out var memo) && memo.ExpiresAt > DateTimeOffset.UtcNow)
        {
            var remembered = await _authStore.GetAsync(targetAppKey, memo.AuthCorpId, cancellationToken).ConfigureAwait(false);
            if (remembered != null)
            {
                return remembered;
            }
        }

        var created = false;
        var lazy = _inFlight.GetOrAdd(
            flightKey,
            _ =>
            {
                created = true;
                // 共享任务不绑定任何单个调用者的 CT：避免"首个调用者取消"连带取消其它等待者。
                return new Lazy<Task<WechatCorpAuthorization>>(
                    () => ExchangeCoreAsync(authCode, targetAppKey, CancellationToken.None),
                    LazyThreadSafetyMode.ExecutionAndPublication);
            });

        try
        {
            return await AwaitSharedAsync(lazy.Value, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // P1-3：仅创建者移除——非创建者提前完成时移除会误删"他人正在等待的飞行"，
            // 使后续调用者重复发起换码（一次性 authCode 将直接失败）。
            if (created)
            {
                ((ICollection<KeyValuePair<string, Lazy<Task<WechatCorpAuthorization>>>>)_inFlight)
                    .Remove(new KeyValuePair<string, Lazy<Task<WechatCorpAuthorization>>>(flightKey, lazy));
            }
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

        // Mud.HttpUtils 3.0.0 迁移（BC-27）：BeginScope(appKey) → UseAppScope(appKey)。
        // 二者在切换器内为同一实现（含守卫与「释放时自动归还上下文」），本处为逐字等价替换。
        using var scope = _switcher.UseAppScope(targetAppKey);

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

        // R4/P1-5：先失效令牌（失败即中止，授权记录保持完整 ⇒ 调用方可直接重试）；
        // 旧实现"先删库后失效"在失效失败时留下「记录已删、令牌仍可用」的不一致窗口。
        await _appManager.InvalidateTokenAsync(
            targetAppKey, WechatTokenTypes.AccessToken, new[] { authCorpId }, cancellationToken).ConfigureAwait(false);

        // 仅失效本 AppKey 的该企业令牌（同一 authCorpId 在其它套件下是独立授权，不得连带失效）。
        await _authStore.RemoveAsync(targetAppKey, authCorpId, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("已撤销企业授权（应用 {AppKey}，AuthCorpId {AuthCorpId}）。", targetAppKey, authCorpId);
    }

    private async Task<WechatCorpAuthorization> ExchangeCoreAsync(
        string authCode, string appKey, CancellationToken cancellationToken)
    {
        // 同 ExchangeAuthCode 等入口：迁移到作用域式推荐入口（守卫与归还语义完全一致）。
        using var scope = _switcher.UseAppScope(appKey);

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
        RememberAuthCode(appKey, authCode, auth.AuthCorpId);

        _logger.LogInformation(
            "授权换码成功并已落库（应用 {AppKey}，AuthCorpId {AuthCorpId}，代开发 {IsCustomizedApp}）。",
            appKey, auth.AuthCorpId, auth.IsCustomizedApp);
        return auth;
    }

    /// <summary>
    /// 复合键（长度前缀拼接，天然单射）：<c>appKey</c> 与 <c>authCode</c> 无需转义即可唯一还原，
    /// 且不引入分隔符转义规则（与组件 <c>ScopeKeyBuilder</c> 的 memoKey 手法同源）。
    /// </summary>
    private static string BuildFlightKey(string appKey, string authCode)
        => appKey.Length.ToString(CultureInfo.InvariantCulture) + ":" + appKey + authCode;

    /// <summary>
    /// 等待共享任务，但只让<b>本调用者</b>的取消影响"本调用者的等待"，不影响共享任务本身。
    /// </summary>
    /// <remarks>
    /// <c>netstandard2.0</c> 无 <c>Task.WaitAsync</c>，故手工实现（<c>CancellationToken.Register</c> 在 ns2.0 可用）。
    /// </remarks>
    private static async Task<T> AwaitSharedAsync<T>(Task<T> shared, CancellationToken cancellationToken)
    {
        if (!cancellationToken.CanBeCanceled)
        {
            return await shared.ConfigureAwait(false);
        }

        var cancelSignal = new TaskCompletionSource<object?>();
        using (cancellationToken.Register(
                   static state => ((TaskCompletionSource<object?>)state!).TrySetResult(null), cancelSignal))
        {
            var finished = await Task.WhenAny(shared, cancelSignal.Task).ConfigureAwait(false);
            if (!ReferenceEquals(finished, shared))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }

        return await shared.ConfigureAwait(false);
    }

    private void RememberAuthCode(string appKey, string authCode, string authCorpId)
    {
        var ttl = _options.Value.AuthCodeMemoTtlSeconds;
        if (ttl <= 0)
        {
            return;
        }

        _memo[BuildFlightKey(appKey, authCode)] = new AuthCodeMemo(authCorpId, DateTimeOffset.UtcNow.AddSeconds(ttl));

        // P2-6：阈值式过期回收（每 N 次写入一次），消除"每次写入全量遍历"在批量导入下的 O(n²)。
        if (Interlocked.Increment(ref _memoWrites) % MemoCleanupInterval != 0)
        {
            return;
        }

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