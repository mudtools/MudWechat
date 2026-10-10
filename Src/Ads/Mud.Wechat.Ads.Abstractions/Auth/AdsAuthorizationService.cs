// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using Mud.Wechat.Ads.Abstractions.Configuration;
using Mud.Wechat.Ads.Abstractions.Exceptions;
using Mud.Wechat.Ads.Abstractions.Transport;
using Mud.Wechat.Ads.DataModels.OAuth;

namespace Mud.Wechat.Ads.Abstractions.Auth;

/// <summary>
/// 广告线授权编排（授权码交换 + refresh_token 轮换 + 令牌取用）。
/// </summary>
/// <remarks>
/// <para>
/// 典型用法：
/// <code>
/// // 1) 引导用户到授权页拿到 authorization_code（跳转链接由宿主生成，本 SDK 不建）
/// var state = await auth.ExchangeAuthorizationCodeAsync("default", code);
/// // 2) 之后业务请求的令牌由传输层自动取用，宿主不需要手工传
/// </code>
/// </para>
/// <para>
/// <b>为什么刷新走 <c>oauth/refresh_token</c> 而不是 <c>oauth/token?grant_type=refresh_token</c></b>：
/// 两支端点官方语义<b>不同</b>（2026-10-10 逐页核验）。
/// <list type="bullet">
/// <item><description><c>oauth/token</c> 应答字段表：<c>refresh_token</c>「当 grant_type=refresh_token 时<b>不返回</b>」，
/// 且该页<b>没有</b>说明旧 refresh_token 是否随之作废 ⇒ 用它刷新会留下「新 access + 未知状态的 refresh」的模糊态。</description></item>
/// <item><description><c>oauth/refresh_token</c> 明确返回<b>两支新令牌</b>，并写明
/// 「<b>原 Access Token 及 Refresh Token 会失效</b>」⇒ 语义完整、可被 SDK 正确持久化。</description></item>
/// </list>
/// 因此本 SDK 的刷新策略只用后者；前者作为「授权码换令牌」端点使用（<c>grant_type=refresh_token</c>
/// 那一支官方枚举由调用方自行决定是否触碰，SDK <b>不</b>提供入口，避免同一应用内两套刷新语义并存）。
/// </para>
/// </remarks>
public interface IAdsAuthorizationService : IAdsAccessTokenProvider
{
    /// <summary>
    /// 用授权码换取 access/refresh 令牌对并写入存储（<b>单飞 + 结果记忆</b>）。
    /// </summary>
    /// <param name="appKey">应用键（必须已注册）。</param>
    /// <param name="authorizationCode">官方 <c>authorization_code</c>（≤64 字节，<b>一次性</b>：换取成功后即失效）。</param>
    /// <param name="redirectUri">回调地址；<c>null</c> / 空白时回落 <see cref="AdsAppConfig.RedirectUri"/>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>落库后的授权状态。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="authorizationCode"/> 为 <c>null</c>。</exception>
    /// <exception cref="WechatAdsException">官方拒绝换取（应答 <c>code != 0</c>）。</exception>
    /// <remarks>
    /// <para>
    /// <b>单飞的必要性</b>：授权码是一次性凭据，并发换取时第二个请求必然被官方拒（并把一次本可成功的授权
    /// 变成一次失败）。同一 <c>(appKey, authorizationCode)</c> 的并发调用因此只放行一个真实请求，
    /// 其余等锁后<b>复查存储</b>直接复用结果。
    /// </para>
    /// </remarks>
    Task<AdsAuthorizationState> ExchangeAuthorizationCodeAsync(
        string appKey,
        string authorizationCode,
        string? redirectUri = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 立即刷新令牌对（不等访问令牌到期）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>刷新后的授权状态。</returns>
    /// <exception cref="WechatAdsReauthorizationRequiredException">
    /// 无 refresh_token、refresh_token 已过期，或官方拒绝刷新（此时存储中的授权状态<b>已被删除</b>）。
    /// </exception>
    Task<AdsAuthorizationState> RefreshAsync(string appKey, CancellationToken cancellationToken = default);

    /// <summary>读取当前落库的授权状态（诊断用；不含任何令牌的明文输出约定见 <see cref="AdsAuthorizationState"/>）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权状态；从未授权过则为 <c>null</c>。</returns>
    Task<AdsAuthorizationState?> GetStateAsync(string appKey, CancellationToken cancellationToken = default);
}

/// <summary>
/// <see cref="IAdsAuthorizationService"/> 默认实现。
/// </summary>
/// <remarks>
/// <para>
/// <b>并发模型</b>：每 <c>appKey</c> 一把 <see cref="SemaphoreSlim"/>，串行化「读存储 → 判定 → 网络 → 写存储」
/// 整个临界区。锁只在进程内生效；多实例部署必须由宿主提供分布式 <see cref="IWechatAdsAuthorizationStore"/>，
/// 并接受一个事实 —— <b>跨实例的并发刷新在 SDK 层面无法用进程锁消除</b>，只能靠存储实现
/// （同一份分布式存储 ⇒ 后到的刷新覆盖前者；一次性 refresh 的浪费属可接受代价，
/// 若不可接受应由宿主用分布式锁包裹）。这一取舍写进 README 而非藏起来。
/// </para>
/// <para>
/// <b>ADS-B3（本类的失败顺序）</b>：任何「刷新不可继续」的路径都是
/// <c>store.RemoveAsync(...)</c> → <c>throw WechatAdsReauthorizationRequiredException</c>，
/// <b>顺序即契约</b>：先抛后删会在崩溃 / 吞异常的宿主下把一份官方已作废的 refresh_token 留在库里。
/// </para>
/// </remarks>
public sealed class AdsAuthorizationService : IAdsAuthorizationService
{
    private readonly IAdsAppManager _apps;
    private readonly IWechatAdsAuthorizationStore _store;
    private readonly IAdsOAuthHttpClient _httpClient;
    private readonly IAdsClock _clock;
    private readonly ILogger<AdsAuthorizationService>? _logger;

    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim> _gates =
        new(StringComparer.Ordinal);

    /// <summary>创建授权编排服务。</summary>
    /// <param name="apps">应用注册表。</param>
    /// <param name="store">授权状态存储。</param>
    /// <param name="httpClient">OAuth 客户端（<b>不带</b>令牌注入 Handler）。</param>
    /// <param name="clock">时钟缝。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <exception cref="ArgumentNullException">任一必填参数为 <c>null</c>。</exception>
    public AdsAuthorizationService(
        IAdsAppManager apps,
        IWechatAdsAuthorizationStore store,
        IAdsOAuthHttpClient httpClient,
        IAdsClock clock,
        ILogger<AdsAuthorizationService>? logger = null)
    {
        _apps = apps ?? throw new ArgumentNullException(nameof(apps));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AdsAuthorizationState> ExchangeAuthorizationCodeAsync(
        string appKey,
        string authorizationCode,
        string? redirectUri = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        if (string.IsNullOrWhiteSpace(authorizationCode))
        {
            throw new ArgumentNullException(nameof(authorizationCode));
        }

        var config = _apps.GetRequiredApp(appKey);
        var uri = BuildUri(
            config,
            AdsOAuthRoutes.Token,
            ("client_id", config.ClientId),
            ("client_secret", config.ClientSecret),
            ("grant_type", AdsOAuthRoutes.GrantTypeAuthorizationCode),
            ("authorization_code", authorizationCode),
            ("redirect_uri", FirstNonBlank(redirectUri, config.RedirectUri)));

        var gate = Gate(appKey);
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 结果记忆：同键并发里若已有别人换取成功，直接复用，不再打第二次（授权码一次性）。
            var existing = await _store.GetAsync(appKey, cancellationToken).ConfigureAwait(false);
            if (existing is not null && existing.IsAccessTokenUsable(_clock.UtcNow, 0))
            {
                _logger?.LogDebug("腾讯广告应用 {AppKey} 已有可用授权状态，跳过重复的授权码交换。", appKey);
                return existing;
            }

            var response = await GetAsync(uri, cancellationToken).ConfigureAwait(false);
            var data = RequireData(response, uri);
            if (string.IsNullOrEmpty(data.AccessToken))
            {
                throw new InvalidOperationException(
                    $"腾讯广告 oauth/token 应答缺少 access_token（AppKey={appKey}），无法建立授权状态。");
            }

            var nowMs = _clock.UtcNow.ToUnixTimeMilliseconds();
            var state = new AdsAuthorizationState
            {
                AppKey = appKey,
                AccessToken = data.AccessToken,
                AccessTokenExpireAtMs = ExpireAtMs(nowMs, data.AccessTokenExpiresIn, DefaultAccessTokenLifetimeSeconds),
                RefreshToken = data.RefreshToken,
                RefreshTokenExpireAtMs = ExpireAtMs(nowMs, data.RefreshTokenExpiresIn, DefaultRefreshTokenLifetimeSeconds),
                AccountId = data.AuthorizerInfo?.AccountId,
                AccountUin = data.AuthorizerInfo?.AccountUin,
                WechatAccountId = data.AuthorizerInfo?.WechatAccountId,
                AccountRoleType = data.AuthorizerInfo?.AccountRoleType,
                ScopeList = data.AuthorizerInfo?.ScopeList?.ToArray(),
            };

            if (string.IsNullOrEmpty(state.RefreshToken))
            {
                // 授权码流程官方恒返回 refresh_token。缺失 = 该应用永远只能等到 access 过期后重新授权，
                // 属必须在启动期/首换期点名的形态异常，静默落库只会把问题推到生产。
                throw new InvalidOperationException(
                    $"腾讯广告 oauth/token 应答缺少 refresh_token（AppKey={appKey}）。" +
                    "授权码流程必须返回一次性刷新令牌；请核对应用的授权回调配置与 scope。");
            }

            await _store.SetAsync(appKey, state, cancellationToken).ConfigureAwait(false);
            _logger?.LogInformation("腾讯广告应用 {AppKey} 授权码交换成功，AccountId={AccountId}。", appKey, state.AccountId);
            return state;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<AdsAuthorizationState> RefreshAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        var config = _apps.GetRequiredApp(appKey);
        var gate = Gate(appKey);
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await RefreshUnsafeAsync(config, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            gate.Release();
        }
    }

    /// <inheritdoc />
    public Task<AdsAuthorizationState?> GetStateAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        return _store.GetAsync(appKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetAccessTokenAsync(string? appKey, CancellationToken cancellationToken = default)
    {
        var config = string.IsNullOrWhiteSpace(appKey) ? _apps.ResolveCurrent() : _apps.GetRequiredApp(appKey!);

        var gate = Gate(config.AppKey);
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var state = await _store.GetAsync(config.AppKey, cancellationToken).ConfigureAwait(false);
            if (state is not null && state.IsAccessTokenUsable(_clock.UtcNow, config.TokenRefreshThreshold))
            {
                return state.AccessToken!;
            }

            // 到这里访问令牌要么没有、要么进入阈值期 ⇒ 走刷新（刷新内部按 ADS-B3 顺序处置失败）。
            var refreshed = await RefreshUnsafeAsync(config, cancellationToken).ConfigureAwait(false);
            return refreshed.AccessToken!;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>
    /// 刷新主体。<b>调用方必须已持有 <see cref="Gate"/>（每 AppKey 的闸，容量 1）</b>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为什么不加锁也不判重入</b>：<see cref="SemaphoreSlim"/> 不可重入，而「取令牌」与「显式刷新」
    /// 是两条同质的链路（都要读存储 → 判定 → 网络 → 写存储）。把加锁留在公开入口、让本方法只吃「已持锁」
    /// 前提，比在方法内部猜「我是不是已经在锁里」更稳 —— 后者需要一个跨 await 的重入账本，
    /// 而那种账本一旦与闸的真实容量不一致就是死锁。公开入口各持一次锁、共用同一主体，
    /// 也保证了失败顺序（ADS-B3）只有一份实现。
    /// </para>
    /// <para>
    /// 存储读取在本方法内进行，因此经 <see cref="GetAccessTokenAsync"/> 进入时天然是「锁后再读」——
    /// 即「别人刚刷完」的复查（见该方法的 remarks）。
    /// </para>
    /// </remarks>
    private async Task<AdsAuthorizationState> RefreshUnsafeAsync(
        AdsAppConfig config,
        CancellationToken cancellationToken)
    {
        var appKey = config.AppKey;
        var state = await _store.GetAsync(appKey, cancellationToken).ConfigureAwait(false);

        if (state is null || string.IsNullOrEmpty(state.RefreshToken))
        {
            // 无刷新能力：清掉残留（若有）后要求重新授权。
            if (state is not null)
            {
                await _store.RemoveAsync(appKey, cancellationToken).ConfigureAwait(false);
            }

            throw new WechatAdsReauthorizationRequiredException(
                appKey,
                state is null ? "存储中没有授权状态" : "存储中没有 refresh_token");
        }

        if (!state.IsRefreshTokenUsable(_clock.UtcNow))
        {
            await _store.RemoveAsync(appKey, cancellationToken).ConfigureAwait(false);
            throw new WechatAdsReauthorizationRequiredException(appKey, "refresh_token 已过期");
        }

        var uri = BuildUri(
            config,
            AdsOAuthRoutes.RefreshToken,
            ("client_id", config.ClientId),
            ("client_secret", config.ClientSecret),
            ("refresh_token", state.RefreshToken));

        AdsTokenResponse response;
        try
        {
            response = await GetAsync(uri, cancellationToken).ConfigureAwait(false);
        }
        catch (WechatAdsException ex)
        {
            // 官方拒绝刷新 ⇒ 旧 refresh_token 已不可信（一次性凭据），先删后抛（ADS-B3 的顺序即契约）。
            await _store.RemoveAsync(appKey, cancellationToken).ConfigureAwait(false);
            throw new WechatAdsReauthorizationRequiredException(appKey, "刷新请求被官方拒绝", ex.ErrorCode);
        }

        var data = RequireData(response, uri);
        if (string.IsNullOrEmpty(data.AccessToken) || string.IsNullOrEmpty(data.RefreshToken))
        {
            await _store.RemoveAsync(appKey, cancellationToken).ConfigureAwait(false);
            throw new WechatAdsReauthorizationRequiredException(
                appKey,
                "oauth/refresh_token 应答缺少 access_token 或 refresh_token（旧值已被官方作废，无法安全落库）");
        }

        var nowMs = _clock.UtcNow.ToUnixTimeMilliseconds();
        var rotated = new AdsAuthorizationState
        {
            AppKey = appKey,
            AccessToken = data.AccessToken,
            AccessTokenExpireAtMs = ExpireAtMs(nowMs, data.AccessTokenExpiresIn, DefaultAccessTokenLifetimeSeconds),
            RefreshToken = data.RefreshToken,
            RefreshTokenExpireAtMs = ExpireAtMs(nowMs, data.RefreshTokenExpiresIn, DefaultRefreshTokenLifetimeSeconds),
            // 刷新端点不返回 authorizer_info ⇒ 身份字段必须原样继承，否则 AccountId 在第一次刷新后消失。
            AccountId = state.AccountId,
            AccountUin = state.AccountUin,
            WechatAccountId = state.WechatAccountId,
            AccountRoleType = state.AccountRoleType,
            ScopeList = state.ScopeList,
        };

        await _store.SetAsync(appKey, rotated, cancellationToken).ConfigureAwait(false);
        _logger?.LogInformation("腾讯广告应用 {AppKey} 令牌刷新成功（一次性 refresh_token 已轮换）。", appKey);
        return rotated;
    }

    /// <summary>官方 access_token 的缺省有效时长（秒）：应答未给出 <c>access_token_expires_in</c> 时使用。</summary>
    /// <remarks>
    /// 取值即官方应答示例值（86400 / 2592000）。<b>为何要有缺省</b>：字段表标了 integer，
    /// 但 token 页的<b>应答示例里 <c>access_token_expires_in</c> 出现过、也可能不出现</b>（官方文档自相矛盾点之一），
    /// 缺省值取「官方自己给的示例值」比取 0（= 立刻过期、每次请求都刷）安全得多。
    /// </remarks>
    internal const long DefaultAccessTokenLifetimeSeconds = 86400;

    /// <summary>官方 refresh_token 的缺省有效时长（秒）。</summary>
    internal const long DefaultRefreshTokenLifetimeSeconds = 2592000;

    private SemaphoreSlim Gate(string appKey) => _gates.GetOrAdd(appKey, static _ => new SemaphoreSlim(1, 1));

    private async Task<AdsTokenResponse> GetAsync(Uri uri, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await _httpClient.SendAsync<AdsTokenResponse>(request, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // 判错前不记录 URI：Query 里就是 client_secret / refresh_token（RedactUri 会剥 query，异常侧安全）。
        WechatAdsException.ThrowIfFailed(response, uri.GetLeftPart(UriPartial.Path));
        return response!;
    }

    private static AdsTokenData RequireData(AdsTokenResponse response, Uri uri)
        => response.Data ?? throw new InvalidOperationException(
            $"腾讯广告 OAuth 应答缺少 data 载荷：{uri.GetLeftPart(UriPartial.Path)}");

    /// <summary>时长（秒）→ 绝对过期毫秒；官方未给时长时回落 <paramref name="defaultLifetimeSeconds"/>。</summary>
    private static long ExpireAtMs(long nowMs, long? lifetimeSeconds, long defaultLifetimeSeconds)
    {
        var seconds = lifetimeSeconds is > 0 ? lifetimeSeconds.Value : defaultLifetimeSeconds;
        return nowMs + seconds * 1000L;
    }

    private static string? FirstNonBlank(string? primary, string? fallback)
        => !string.IsNullOrWhiteSpace(primary) ? primary : (string.IsNullOrWhiteSpace(fallback) ? null : fallback);

    /// <summary>
    /// 组装 Query（<b>官方两页均为 GET</b>，curl 用 <c>-G -d</c> ⇒ 参数进 Query，<b>不是</b>请求体）。
    /// </summary>
    /// <remarks>空值一律<b>不上送</b>：官方 <c>redirect_uri</c> 为选填，送空串与不送在部分网关上语义不同。</remarks>
    private static Uri BuildUri(AdsAppConfig config, string path, params (string Name, string? Value)[] parameters)
    {
        var builder = new StringBuilder(config.BaseUrl.TrimEnd('/'));
        builder.Append(path);
        var appended = false;
        foreach (var (name, value) in parameters)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            builder.Append(appended ? '&' : '?');
            appended = true;
            builder.Append(Uri.EscapeDataString(name));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value!));
        }

        return new Uri(builder.ToString(), UriKind.Absolute);
    }
}
