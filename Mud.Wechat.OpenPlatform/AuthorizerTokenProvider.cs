// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Mud.Wechat.OpenPlatform.Abstractions;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 授权方接口调用令牌提供者（<see cref="IAuthorizerTokenProvider"/>）：缓存 + 按需刷新 + <b>按 appid 分槽单飞</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须缓存（官方原文）</b>：<c>authorizer_access_token</c> 有效期 2 小时，官方明确要求缓存，
/// 以免「获取/刷新接口调用令牌的 API 调用触发<b>每日限额</b>」⇒ 业务侧每次要用令牌都应经本类取，
/// 由它决定是否真的去刷新。
/// </para>
/// <para>
/// <b>按 appid 分槽</b>：刷新闸是<b>每个授权方一把</b>，而不是全局一把 ——
/// 否则某个授权方的刷新卡住（官方慢响应）会连带阻塞其它授权方取令牌，
/// 把「一个商户的问题」放大成「全平台抖动」。
/// </para>
/// <para>
/// <b>刷新失败但旧令牌仍可用 ⇒ 沿用</b>；旧令牌已失效 ⇒ <b>fail-closed 抛出</b>
/// （与平台令牌的处置一致：进提前窗口 ≠ 已失效；而已失效时绝不返回过期令牌冒充成功）。
/// </para>
/// <para>
/// <b>已知限制（诚实记录）</b>：分槽闸表按 appid 增长，<b>不回收</b>。
/// 授权方数量在业务量级下（数百~数千）可忽略；若达到数万级，应改为带淘汰的分布式锁实现。
/// 同理，进程内默认存储只适用于单实例（见 <see cref="IAuthorizerTokenStore"/>）。
/// </para>
/// </remarks>
public sealed class AuthorizerTokenProvider : IAuthorizerTokenProvider
{
    private readonly IAuthorizerTokenStore _store;
    private readonly IComponentAuthorizationService _authorization;
    private readonly IOpenPlatformClock _clock;
    private readonly ILogger<AuthorizerTokenProvider>? _logger;

    /// <summary>按 appid 的刷新闸（见类注释：分槽而非全局）。</summary>
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates =
        new ConcurrentDictionary<string, SemaphoreSlim>(StringComparer.Ordinal);

    /// <summary>创建提供者。</summary>
    /// <param name="store">令牌存储（多实例部署须换分布式实现）。</param>
    /// <param name="authorization">授权流程服务（用于刷新令牌）。</param>
    /// <param name="clock">时间源。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <exception cref="ArgumentNullException">任一必填依赖为 <c>null</c>。</exception>
    public AuthorizerTokenProvider(
        IAuthorizerTokenStore store,
        IComponentAuthorizationService authorization,
        IOpenPlatformClock clock,
        ILogger<AuthorizerTokenProvider>? logger = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _authorization = authorization ?? throw new ArgumentNullException(nameof(authorization));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger;
    }

    /// <inheritdoc />
    public void AcceptAuthorization(AuthorizerTokens tokens)
    {
        if (tokens == null)
        {
            throw new ArgumentNullException(nameof(tokens));
        }

        if (string.IsNullOrWhiteSpace(tokens.AuthorizerAppId))
        {
            throw new ArgumentException("授权方 appid 不能为空。", nameof(tokens));
        }

        if (string.IsNullOrWhiteSpace(tokens.RefreshToken) && string.IsNullOrWhiteSpace(tokens.AccessToken))
        {
            // 既无长期凭据也无短期令牌：存下来毫无意义，且会让后续误以为「已授权」。
            throw new ArgumentException("授权结果既无刷新令牌也无接口调用令牌，无可保存内容。", nameof(tokens));
        }

        _store.Set(new AuthorizerTokenSnapshot(
            tokens.AuthorizerAppId!,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            _clock.UtcNow));
    }

    /// <inheritdoc />
    public async Task<string> GetAuthorizerAccessTokenAsync(
        string authorizerAppId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorizerAppId))
        {
            throw new ArgumentException("授权方 appid 不能为空。", nameof(authorizerAppId));
        }

        var cached = Read(authorizerAppId);
        if (IsFresh(cached))
        {
            return cached!.AccessToken!;
        }

        var gate = _gates.GetOrAdd(authorizerAppId, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 双检：等锁期间可能已被别的调用刷新（同一 appid 的并发单飞）。
            cached = Read(authorizerAppId);
            if (IsFresh(cached))
            {
                return cached!.AccessToken!;
            }

            if (string.IsNullOrWhiteSpace(cached?.RefreshToken))
            {
                throw new WechatOpenPlatformException(
                    $"授权方 {authorizerAppId} 没有可用的刷新令牌（授权关系未建立或已被取消）⇒ 无法获取接口调用令牌。");
            }

            try
            {
                var refreshed = await _authorization
                    .RefreshAuthorizerTokenAsync(authorizerAppId, cached!.RefreshToken!, cancellationToken)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(refreshed.AccessToken))
                {
                    throw new WechatOpenPlatformException(
                        $"刷新授权方 {authorizerAppId} 的令牌：官方应答未包含 authorizer_access_token。");
                }

                // 整体替换保存：刷新令牌若被官方更新则采用新值，否则沿用旧值（不得清空长期凭据）。
                var snapshot = new AuthorizerTokenSnapshot(
                    authorizerAppId,
                    refreshed.AccessToken,
                    string.IsNullOrWhiteSpace(refreshed.RefreshToken) ? cached.RefreshToken : refreshed.RefreshToken,
                    refreshed.AccessTokenExpiresAt,
                    _clock.UtcNow);

                _store.Set(snapshot);
                return snapshot.AccessToken!;
            }
            catch (Exception ex) when (ex is WechatOpenPlatformException or HttpRequestException)
            {
                // 旧令牌仍可用 ⇒ 沿用（进提前窗口 ≠ 已失效），否则 fail-closed 上抛。
                if (cached != null
                    && ComponentAccessTokenPolicy.IsUsable(cached.AccessToken, cached.AccessTokenExpiresAt, _clock.UtcNow))
                {
                    _logger?.LogWarning(
                        "授权方 {AuthorizerAppId} 令牌刷新失败，本次沿用仍有效的旧令牌（原因类型：{ReasonType}）。",
                        authorizerAppId, ex.GetType().Name);
                    return cached.AccessToken!;
                }

                throw;
            }
        }
        finally
        {
            gate.Release();
        }
    }

    private AuthorizerTokenSnapshot? Read(string authorizerAppId)
        => _store.TryGet(authorizerAppId, out var snapshot) ? snapshot : null;

    /// <summary>「既可用、又无需刷新」——只有这种状态才走缓存快车道。</summary>
    private bool IsFresh(AuthorizerTokenSnapshot? snapshot)
    {
        if (snapshot == null)
        {
            return false;
        }

        var now = _clock.UtcNow;
        return ComponentAccessTokenPolicy.IsUsable(snapshot.AccessToken, snapshot.AccessTokenExpiresAt, now)
               && !ComponentAccessTokenPolicy.ShouldRefresh(
                   snapshot.AccessToken,
                   snapshot.AccessTokenExpiresAt,
                   now,
                   TimeSpan.FromSeconds(OpenPlatformContract.AuthorizerTokenRefreshLeadSeconds));
    }
}
