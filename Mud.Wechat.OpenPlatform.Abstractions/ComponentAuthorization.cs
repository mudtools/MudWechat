// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 预授权码及其失效时刻。
/// </summary>
/// <remarks>
/// <b>有效期 1800 秒</b>（官方原文）：预授权码是「用户在授权页上授权」这一动作的凭据，
/// 过期即须重新获取 —— 故本类带上失效时刻，供宿主在渲染授权页前判断是否需重取。
/// </remarks>
public sealed class PreAuthCodeResult
{
    /// <summary>创建结果。</summary>
    /// <param name="preAuthCode">预授权码。</param>
    /// <param name="expiresAt">失效时刻（UTC）。</param>
    public PreAuthCodeResult(string preAuthCode, DateTimeOffset expiresAt)
    {
        PreAuthCode = preAuthCode;
        ExpiresAt = expiresAt;
    }

    /// <summary>预授权码。</summary>
    public string PreAuthCode { get; }

    /// <summary>失效时刻（UTC）。</summary>
    public DateTimeOffset ExpiresAt { get; }
}

/// <summary>
/// 授权方的令牌集合（换取授权信息 / 刷新授权方令牌 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b><c>AccessToken</c> 可能为 <c>null</c>（这是正常情况，不是错误）</b>：官方原文
/// 「<c>authorizer_access_token</c>（在授权的公众号/小程序<b>具备 API 权限</b>时，才有此返回值）」
/// ⇒ 只授权了扫码登录之类能力的账号<b>不会</b>返回接口调用令牌。故本类用可空表达，
/// 并把「是否具备 API 权限」显式暴露为 <see cref="HasApiScope"/>，
/// 避免调用方拿到 <c>null</c> 时误判成「换取失败」。
/// </para>
/// <para>
/// <b>刷新令牌须持久化</b>：<c>authorizer_refresh_token</c> 是长期凭据，
/// 丢失即须让用户重新授权 ⇒ 宿主必须落库（本线的端口只负责返回，不做持久化）。
/// </para>
/// </remarks>
public sealed class AuthorizerTokens
{
    /// <summary>创建令牌集合。</summary>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <param name="accessToken">授权方接口调用令牌（可能为 <c>null</c>，见类注释）。</param>
    /// <param name="refreshToken">授权方刷新令牌。</param>
    /// <param name="accessTokenExpiresAt">接口调用令牌失效时刻（UTC）。</param>
    public AuthorizerTokens(
        string? authorizerAppId,
        string? accessToken,
        string? refreshToken,
        DateTimeOffset accessTokenExpiresAt)
    {
        AuthorizerAppId = authorizerAppId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
    }

    /// <summary>授权方 appid（官方 <c>authorizer_appid</c>）。</summary>
    public string? AuthorizerAppId { get; }

    /// <summary>授权方接口调用令牌（官方 <c>authorizer_access_token</c>）；无 API 权限时为 <c>null</c>。</summary>
    public string? AccessToken { get; }

    /// <summary>授权方刷新令牌（官方 <c>authorizer_refresh_token</c>）。<b>须持久化</b>。</summary>
    public string? RefreshToken { get; }

    /// <summary>接口调用令牌失效时刻（UTC）；无令牌时为默认值（不参与判断）。</summary>
    public DateTimeOffset AccessTokenExpiresAt { get; }

    /// <summary>是否具备 API 权限（即是否存在接口调用令牌）—— <c>false</c> 属正常形态，不是失败。</summary>
    public bool HasApiScope => !string.IsNullOrWhiteSpace(AccessToken);
}

/// <summary>
/// 第三方平台授权流程服务（预授权码 → 换取授权信息 → 刷新授权方令牌）。
/// </summary>
/// <remarks>
/// <para>
/// <b>完整链路</b>（官方《授权流程》）：
/// ① 平台调 <see cref="CreatePreAuthCodeAsync"/> 拿预授权码（有效期 1800 秒）；
/// ② 用预授权码拼出授权页 URL 让用户点击 / 扫码；
/// ③ 用户完成授权后，微信回调平台的 <c>redirect_uri</c>，URL 参数里带 <c>auth_code</c>
/// （官方原文「在回调 URI 中通过 URL 参数获取授权码」）；
/// ④ 平台拿 <c>auth_code</c> 调 <see cref="QueryAuthorizationAsync"/> 换取
/// <c>authorizer_access_token</c> + <c>authorizer_refresh_token</c>；
/// ⑤ 接口调用令牌 2 小时过期，用 <see cref="RefreshAuthorizerTokenAsync"/> 续期。
/// </para>
/// <para>
/// <b>第 ② 步的授权页 URL 不在本接口内</b>：它是官方页面地址的字符串拼接（非服务端调用），
/// 且本轮<b>未核验到</b>官方页面给出的确切参数表 ⇒ 按「未核验不臆造」纪律，
/// 不由 SDK 提供拼接方法（宿主按官方《授权流程》页自行拼接）。
/// </para>
/// <para>
/// <b>缓存纪律（官方原文）</b>：authorizer_access_token 有效期 2 小时，
/// 官方明确要求缓存，以免「获取/刷新接口调用令牌的 API 调用触发<b>每日限额</b>」⇒
/// 宿主应持久化并复用，<b>不要</b>每次调用业务接口前都刷新。
/// </para>
/// </remarks>
public interface IComponentAuthorizationService
{
    /// <summary>获取预授权码（有效期 1800 秒）。</summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预授权码与失效时刻，见 <see cref="PreAuthCodeResult"/>。</returns>
    /// <exception cref="WechatOpenPlatformException">官方报错或应答不可解析。</exception>
    Task<PreAuthCodeResult> CreatePreAuthCodeAsync(CancellationToken cancellationToken = default);

    /// <summary>用授权码换取授权方令牌（首次授权）。</summary>
    /// <param name="authorizationCode">授权码（授权回调 URI 上的 <c>auth_code</c> URL 参数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权方令牌集合，见 <see cref="AuthorizerTokens"/>。</returns>
    /// <exception cref="ArgumentException"><paramref name="authorizationCode"/> 为空白。</exception>
    /// <exception cref="WechatOpenPlatformException">官方报错或应答不可解析。</exception>
    Task<AuthorizerTokens> QueryAuthorizationAsync(
        string authorizationCode,
        CancellationToken cancellationToken = default);

    /// <summary>刷新授权方接口调用令牌。</summary>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <param name="authorizerRefreshToken">授权方刷新令牌（换取授权信息时得到，<b>须持久化</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>新的授权方令牌集合（含可能更新的刷新令牌），见 <see cref="AuthorizerTokens"/>。</returns>
    /// <exception cref="ArgumentException">任一参数为空白。</exception>
    /// <exception cref="WechatOpenPlatformException">官方报错或应答不可解析。</exception>
    Task<AuthorizerTokens> RefreshAuthorizerTokenAsync(
        string authorizerAppId,
        string authorizerRefreshToken,
        CancellationToken cancellationToken = default);
}
