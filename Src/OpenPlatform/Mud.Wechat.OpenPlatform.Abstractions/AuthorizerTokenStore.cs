// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 某个授权方的令牌快照（接口调用令牌 + 刷新令牌）。
/// </summary>
/// <remarks>
/// <para>
/// <b>刷新令牌是长期凭据，必须持久化</b>：它换取的是「代表该授权方调用接口」的能力，
/// 丢失即须让用户重新授权。接口调用令牌（2 小时）只是缓存。
/// </para>
/// <para>
/// <b>刷新失败时不得把快照写坏</b>：写入一律是<b>整体替换</b>，且实现应只在拿到有效值时写入
/// （旧值多撑一轮优于把长期凭据清空）。
/// </para>
/// </remarks>
public sealed class AuthorizerTokenSnapshot
{
    /// <summary>创建快照。</summary>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <param name="accessToken">接口调用令牌（可能为 <c>null</c>：无 API 权限时官方不返回）。</param>
    /// <param name="refreshToken">刷新令牌（长期凭据）。</param>
    /// <param name="accessTokenExpiresAt">接口调用令牌失效时刻（UTC）。</param>
    /// <param name="updatedAt">本快照的写入时刻（UTC，用于诊断）。</param>
    public AuthorizerTokenSnapshot(
        string authorizerAppId,
        string? accessToken,
        string? refreshToken,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset updatedAt)
    {
        AuthorizerAppId = authorizerAppId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>授权方 appid（<b>分槽键</b>）。</summary>
    public string AuthorizerAppId { get; }

    /// <summary>接口调用令牌（<c>authorizer_access_token</c>）。</summary>
    public string? AccessToken { get; }

    /// <summary>刷新令牌（<c>authorizer_refresh_token</c>）。</summary>
    public string? RefreshToken { get; }

    /// <summary>接口调用令牌失效时刻（UTC）。</summary>
    public DateTimeOffset AccessTokenExpiresAt { get; }

    /// <summary>写入时刻（UTC；本地时钟，仅用于诊断）。</summary>
    public DateTimeOffset UpdatedAt { get; }
}

/// <summary>
/// 授权方令牌存储端口（<b>按 <c>authorizer_appid</c> 分槽</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须是「存储」而不是进程内字段</b>：刷新令牌是长期凭据，
/// 且授权方数量随业务增长（每个授权方一把令牌）——
/// 只放进程内会让「重启即全部丢失、须让所有用户重新授权」。
/// </para>
/// <para>
/// <b>多实例部署须换分布式实现</b>（Redis 等），与各线存储端口同款纪律：
/// 否则同一授权方在不同实例上各自刷新，会互相覆盖刷新令牌；
/// 且官方对「获取/刷新接口调用令牌」有<b>每日限额</b>，实例数会成倍放大调用量。
/// </para>
/// </remarks>
public interface IAuthorizerTokenStore
{
    /// <summary>读取指定授权方的令牌快照。</summary>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <param name="snapshot">命中时的快照；未命中为 <c>null</c>。</param>
    /// <returns>是否存在快照。</returns>
    bool TryGet(string? authorizerAppId, out AuthorizerTokenSnapshot? snapshot);

    /// <summary>整体替换指定授权方的令牌快照。</summary>
    /// <param name="snapshot">新快照（<b>整体替换</b>，不做字段级合并）。</param>
    void Set(AuthorizerTokenSnapshot snapshot);
}

/// <summary>
/// 授权方令牌的<b>进程内</b>存储（<see cref="IAuthorizerTokenStore"/> 的默认实现）。
/// </summary>
/// <remarks>
/// <b>仅用于单实例 / 测试</b>：生产多实例部署须换分布式实现，否则刷新令牌会随实例各存一份
/// （见端口 remarks）。
/// </remarks>
public sealed class InMemoryAuthorizerTokenStore : IAuthorizerTokenStore
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, AuthorizerTokenSnapshot> _slots
        = new System.Collections.Concurrent.ConcurrentDictionary<string, AuthorizerTokenSnapshot>(StringComparer.Ordinal);

    /// <inheritdoc />
    public bool TryGet(string? authorizerAppId, out AuthorizerTokenSnapshot? snapshot)
    {
        if (string.IsNullOrWhiteSpace(authorizerAppId))
        {
            snapshot = null;
            return false;
        }

        return _slots.TryGetValue(authorizerAppId!, out snapshot);
    }

    /// <inheritdoc />
    public void Set(AuthorizerTokenSnapshot snapshot)
    {
        if (snapshot == null)
        {
            throw new ArgumentNullException(nameof(snapshot));
        }

        if (string.IsNullOrWhiteSpace(snapshot.AuthorizerAppId))
        {
            throw new ArgumentException("授权方 appid 不能为空。", nameof(snapshot));
        }

        _slots[snapshot.AuthorizerAppId] = snapshot;
    }
}

/// <summary>
/// 授权方接口调用令牌的提供者（<b>带缓存与按需刷新</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>缓存纪律（官方原文）</b>：<c>authorizer_access_token</c> 有效期 2 小时，官方明确要求
/// 缓存，以免「获取/刷新接口调用令牌的 API 调用触发<b>每日限额</b>」⇒
/// 调用方应<b>每次</b>都经本端口取令牌，由它决定是否真的去刷新。
/// </para>
/// <para>
/// <b>失败语义（fail-closed）</b>：无法给出可用令牌时<b>必须抛出</b>
/// （<see cref="WechatOpenPlatformException"/>），绝不返回空串 —— 让调用方拿着空令牌去拼 URL，
/// 只会得到一个语焉不详的 40001，掩盖「该授权关系已失效」这一真相。
/// </para>
/// </remarks>
public interface IAuthorizerTokenProvider
{
    /// <summary>
    /// 接受一次授权结果（<b>首次授权或更新授权后</b>调用），把刷新令牌落库。
    /// </summary>
    /// <param name="tokens">由 <c>api_query_auth</c> 换取的授权方令牌。</param>
    /// <remarks>
    /// <b>必须在拿到授权码后立即调用</b>：刷新令牌拿到才算真正「授权完成」；
    /// 只把授权码留在内存里，进程重启即须让用户重新授权。
    /// </remarks>
    void AcceptAuthorization(AuthorizerTokens tokens);

    /// <summary>获取指定授权方当前可用的接口调用令牌（必要时自动刷新）。</summary>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>可用的 <c>authorizer_access_token</c>。</returns>
    /// <exception cref="ArgumentException"><paramref name="authorizerAppId"/> 为空白。</exception>
    /// <exception cref="WechatOpenPlatformException">无刷新令牌、官方报错、或令牌已失效且刷新失败。</exception>
    Task<string> GetAuthorizerAccessTokenAsync(
        string authorizerAppId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 使指定授权方的缓存令牌立即失效（下次取用将强制走刷新链；刷新令牌等长期凭据不受影响）。
    /// </summary>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <exception cref="ArgumentException"><paramref name="authorizerAppId"/> 为空白。</exception>
    /// <remarks>供声明式 <c>[Token]</c> 客户端的令牌管理器在 errcode 恢复路径调用（仅清短期令牌，不删授权关系）。</remarks>
    void Invalidate(string authorizerAppId);
}
