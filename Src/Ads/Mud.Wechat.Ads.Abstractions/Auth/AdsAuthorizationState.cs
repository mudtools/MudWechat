// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Abstractions.Auth;

/// <summary>
/// 广告线授权状态：一个应用（<c>client_id</c>）当前持有的 access/refresh 令牌对 + 它授权到的账号身份。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不是公用层 <c>IWechatTokenStore</c> 的条目</b>：那条端口的持久化编码是
/// <c>{expireMs}|{token}</c>（<c>WechatTokenBridgeCodec</c>），<b>只有一个令牌槽位</b>，
/// 装不下 refresh_token 及其有效期，更装不下 <c>authorizer_info</c>。
/// 而 v3.0 的 refresh_token 是<b>一次性</b>凭据（刷新即换新的、旧的立即失效），
/// 「access 与 refresh 必须原子地一起换」是这条线的核心不变式 ⇒ 必须自建能承载整对状态的端口
/// （<see cref="IWechatAdsAuthorizationStore"/>），而不是把两半分别塞进两个键里赌一致。
/// </para>
/// <para>
/// <b>过期时刻是绝对的、本地的</b>：官方 <c>*_expires_in</c> 返回的是<b>时长（秒）</b>，
/// 换算成 <c>*ExpireAtMs</c> 的唯一地点是 <see cref="AdsAuthorizationService"/>（用 <see cref="IAdsClock"/> 取时钟）。
/// 存储里不得出现「时长」形态，否则重启水合后会把「86400」当成 1970-01-02 判过期。
/// </para>
/// <para>
/// <b>可空性即语义</b>：<see cref="RefreshToken"/> 为空 = 从未完成授权码流程（无刷新能力）；
/// <see cref="AccessToken"/> 为空但 <see cref="RefreshToken"/> 非空 = 刷新后 access 已失效、待重取。
/// 两种状态在 <see cref="AdsAuthorizationService"/> 里走不同的失败路径，<b>不得</b>合并成一个布尔位。
/// </para>
/// </remarks>
public sealed class AdsAuthorizationState
{
    /// <summary>归属应用键（与 <c>AdsAppConfig.AppKey</c> 一致）。</summary>
    public string AppKey { get; set; } = string.Empty;

    /// <summary>访问令牌（<c>access_token</c>）。<b>不得</b>写入日志 / 遥测 / 异常消息（ADS-B5）。</summary>
    public string? AccessToken { get; set; }

    /// <summary>访问令牌的<b>绝对</b>过期时刻（Unix 毫秒）；<c>0</c> = 无有效访问令牌。</summary>
    public long AccessTokenExpireAtMs { get; set; }

    /// <summary>刷新令牌（<c>refresh_token</c>，<b>一次性</b>凭据）。<b>不得</b>写入日志 / 遥测 / 异常消息。</summary>
    public string? RefreshToken { get; set; }

    /// <summary>刷新令牌的<b>绝对</b>过期时刻（Unix 毫秒）；<c>0</c> = 无刷新能力。</summary>
    public long RefreshTokenExpireAtMs { get; set; }

    /// <summary>授权到的帐号 id（<c>authorizer_info.account_id</c>）—— 业务接口的归属主键。</summary>
    /// <remarks>刷新端点<b>不返回</b> <c>authorizer_info</c> ⇒ 本值必须在首次换码时落库并随后<b>原样保留</b>。</remarks>
    public long? AccountId { get; set; }

    /// <summary>授权帐号对应的 QQ 号（<c>authorizer_info.account_uin</c>）。</summary>
    public long? AccountUin { get; set; }

    /// <summary>授权帐号对应的微信帐号 id（<c>authorizer_info.wechat_account_id</c>）。</summary>
    public string? WechatAccountId { get; set; }

    /// <summary>授权身份类型（<c>authorizer_info.account_role_type</c>，enum 字符串）。</summary>
    public string? AccountRoleType { get; set; }

    /// <summary>权限列表（<c>authorizer_info.scope_list</c>）；<c>null</c> 与空数组含义不同：官方「为空表示拥有所属应用的所有权限」。</summary>
    public string[]? ScopeList { get; set; }

    /// <summary>访问令牌是否仍在有效期内（含提前量判定，见 <see cref="IsAccessTokenUsable"/>）。</summary>
    /// <param name="nowUtc">判定用的时钟（由 <see cref="IAdsClock"/> 提供，测试可注入）。</param>
    /// <param name="thresholdSeconds">提前刷新阈值（秒，来自 <see cref="WechatAppConfigBase.TokenRefreshThreshold"/>）。</param>
    /// <returns>仍可用则 <c>true</c>。</returns>
    public bool IsAccessTokenUsable(DateTimeOffset nowUtc, int thresholdSeconds)
        => IsUsable(AccessToken, AccessTokenExpireAtMs, nowUtc, thresholdSeconds);

    /// <summary>刷新令牌是否仍在有效期内。</summary>
    /// <param name="nowUtc">判定用的时钟。</param>
    /// <returns>仍可用则 <c>true</c>。</returns>
    public bool IsRefreshTokenUsable(DateTimeOffset nowUtc)
        => IsUsable(RefreshToken, RefreshTokenExpireAtMs, nowUtc, 0);

    /// <summary>
    /// 生成一份<b>深拷贝</b>（存储实现用它隔离读写：状态对象会被就地轮换，共享引用等于把内部快照暴露给宿主）。
    /// </summary>
    /// <returns>字段同值、引用独立的新实例。</returns>
    public AdsAuthorizationState Clone()
        => new()
        {
            AppKey = AppKey,
            AccessToken = AccessToken,
            AccessTokenExpireAtMs = AccessTokenExpireAtMs,
            RefreshToken = RefreshToken,
            RefreshTokenExpireAtMs = RefreshTokenExpireAtMs,
            AccountId = AccountId,
            AccountUin = AccountUin,
            WechatAccountId = WechatAccountId,
            AccountRoleType = AccountRoleType,
            ScopeList = ScopeList is null ? null : (string[])ScopeList.Clone(),
        };

    /// <summary>
    /// 掩码描述：<b>只输出存在性与时长，绝不输出令牌值</b>。
    /// </summary>
    /// <remarks>本覆写是「令牌不入日志」红线的机械落点之一（守卫 ADS-B5 同时扫描 Query 凭据参数名）。</remarks>
    public override string ToString()
        => $"AdsAuthorizationState(AppKey={AppKey}, AccessToken={Presence(AccessToken)}" +
           $"/{RemainingMs(AccessTokenExpireAtMs)}s, RefreshToken={Presence(RefreshToken)}" +
           $"/{RemainingMs(RefreshTokenExpireAtMs)}s, AccountId={AccountId})";

    private static bool IsUsable(string? token, long expireAtMs, DateTimeOffset nowUtc, int thresholdSeconds)
    {
        if (string.IsNullOrEmpty(token) || expireAtMs <= 0)
        {
            return false;
        }

        // 阈值语义与其余产品线一致：到期前 threshold 秒即视为「该刷新了」，避免临界竞态。
        var cutoff = expireAtMs - (long)thresholdSeconds * 1000L;
        return nowUtc.ToUnixTimeMilliseconds() < cutoff;
    }

    private static string Presence(string? token) => string.IsNullOrEmpty(token) ? "absent" : "present";

    private static long RemainingMs(long expireAtMs)
    {
        if (expireAtMs <= 0)
        {
            return 0;
        }

        var remaining = expireAtMs - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return remaining > 0 ? remaining / 1000L : 0;
    }
}

/// <summary>
/// 广告线授权状态存储端口（<b>跨进程 / 重启不失联</b>的关键）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有持久层</b>：v3.0 的 refresh_token 是一次性凭据。若只把授权结果放在进程内存里，
/// 进程重启 = 旧 refresh_token 从未被消耗但也再也拿不到 ⇒ 只能让运营重新走一遍授权页。
/// 因此「换码 / 刷新成功后必须写穿存储」不是优化项而是正确性要求（方案 §3.4.2）。
/// </para>
/// <para>
/// <b>原子性契约</b>：<see cref="SetAsync"/> 写入的必须是<b>整对</b>令牌。
/// 实现若分两次写（先 access 后 refresh），进程在两次之间崩溃就会留下
/// 「新 access + 旧 refresh」的组合 —— 旧 refresh 已被官方作废，此后每次刷新都必然失败。
/// 默认进程内实现整对象替换；分布式实现须自行保证同键整值覆盖（如 Redis 单键 HASH/MSET）。
/// </para>
/// <para>
/// 广告线<b>不</b>复用公用层 <c>IWechatTokenStore</c>（槽位不够，理由见 <see cref="AdsAuthorizationState"/>），
/// 也不由 <c>Mud.Wechat.Redis</c> 实现（ADS-S1 双向零引用）—— 宿主需要分布式时实现本端口并预注册，
/// <c>TryAdd</c> 语义保证预注册者胜出。
/// </para>
/// </remarks>
public interface IWechatAdsAuthorizationStore
{
    /// <summary>读取指定应用的授权状态；不存在时返回 <c>null</c>。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权状态或 <c>null</c>。</returns>
    Task<AdsAuthorizationState?> GetAsync(string appKey, CancellationToken cancellationToken = default);

    /// <summary>整值写入指定应用的授权状态（<b>必须</b>原子覆盖，见接口 remarks）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="state">待写入的授权状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task SetAsync(string appKey, AdsAuthorizationState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除指定应用的授权状态（<b>刷新失败后的唯一正确处置</b>：一次性 refresh_token 已不可信，
    /// 留着只会让后续每次请求重复撞墙）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task RemoveAsync(string appKey, CancellationToken cancellationToken = default);
}

/// <summary>
/// 默认进程内授权状态存储（单实例部署够用；多实例须由宿主实现分布式存储并预注册）。
/// </summary>
/// <remarks>读写一律返回 / 存入<b>副本</b>，避免调用方拿着同一实例就地轮换后绕过存储。</remarks>
public sealed class InMemoryWechatAdsAuthorizationStore : IWechatAdsAuthorizationStore
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, AdsAuthorizationState> _states =
        new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<AdsAuthorizationState?> GetAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_states.TryGetValue(appKey, out var state) ? state.Clone() : null);
    }

    /// <inheritdoc />
    public Task SetAsync(string appKey, AdsAuthorizationState state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        if (state is null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // 键与对象内的 AppKey 必须一致：不一致说明调用方把 A 的状态挂到了 B 的键上（跨应用串号）。
        if (!string.Equals(state.AppKey, appKey, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"授权状态的 AppKey（{state.AppKey}）与存储键（{appKey}）不一致，拒绝写入（跨应用串号防护）。",
                nameof(state));
        }

        _states[appKey] = state.Clone();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        cancellationToken.ThrowIfCancellationRequested();
        _states.TryRemove(appKey, out _);
        return Task.CompletedTask;
    }
}

/// <summary>
/// 广告线时钟缝（把「现在几点」变成一个可注入的依赖）。
/// </summary>
/// <remarks>
/// 本线的到期判定、<c>timestamp</c> 生成、以及「刷新失败」的可复现测试都依赖时钟。
/// 没有这根缝，「refresh_token 恰好过期」这类路径只能靠 <c>Task.Delay</c> 或改系统时间来测 —— 后者在 CI 上不可接受。
/// </remarks>
public interface IAdsClock
{
    /// <summary>当前 UTC 时刻（官方时区为 GMT+8，但 <c>timestamp</c> 是<b>绝对</b> Unix 秒 ⇒ 无需做时区换算）。</summary>
    DateTimeOffset UtcNow { get; }
}

/// <summary><see cref="IAdsClock"/> 的系统时钟实现。</summary>
public sealed class AdsSystemClock : IAdsClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

/// <summary>
/// 广告线授权端点常量（路由与参数名照官方原文，守卫 ADS-B2 的锁定对象）。
/// </summary>
/// <remarks>
/// 官方两页：<see href="https://developers.e.qq.com/v3.0/docs/api/oauth/token"/>、
/// <see href="https://developers.e.qq.com/v3.0/docs/api/oauth/refresh_token"/>（2026-10-10 逐页核验：
/// <b>两者均为 GET</b>，curl 示例用 <c>-G -d</c> ⇒ 参数进 Query）。
/// </remarks>
public static class AdsOAuthRoutes
{
    /// <summary>授权码换令牌（也可 <c>grant_type=refresh_token</c>，但本 SDK 的刷新走 <see cref="RefreshToken"/>，理由见其 remarks）。</summary>
    public const string Token = "/oauth/token";

    /// <summary>刷新 Refresh Token（官方语义：换新两支令牌并<b>作废旧的两支</b>）。</summary>
    public const string RefreshToken = "/oauth/refresh_token";

    /// <summary>请求类型取值：授权码（官方 <c>grant_type</c> 枚举之一）。</summary>
    public const string GrantTypeAuthorizationCode = "authorization_code";

    /// <summary>请求类型取值：刷新令牌（官方 <c>grant_type</c> 枚举之一）。</summary>
    public const string GrantTypeRefreshToken = "refresh_token";

    /// <summary>业务接口通用参数名：访问令牌。</summary>
    public const string AccessTokenParameter = "access_token";

    /// <summary>业务接口通用参数名：秒级时间戳（允许误差 300 秒）。</summary>
    public const string TimestampParameter = "timestamp";

    /// <summary>业务接口通用参数名：随机字串（≤32 字符、全局唯一）。</summary>
    public const string NonceParameter = "nonce";
}
