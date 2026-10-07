// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「网页开发 → 网页授权」域 SDK（4 端点，<b>服务号专属</b>；全部<b>免令牌端点</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：网页授权为<b>服务号专属</b>能力，服务端 API 文档位于服务号域
/// <see href="https://developers.weixin.qq.com/doc/service/api/"/>（/doc/service/api/webdev/access/ 前缀；
/// subscription 订阅号域无该目录——2026-10-07 逐页核验确认）。
/// </para>
/// <para>
/// <b>I4/I5 裁决落地（勿「顺手」改动）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>本接口不带 [Token] 特性</b>：四端点全部<b>不消费应用级 access_token</b>——
/// <c>sns/oauth2/access_token</c>/<c>refresh_token</c> 以 <c>appid</c>+<c>secret</c> 入参换<b>用户级</b>
/// 授权凭证；<c>sns/auth</c>/<c>sns/userinfo</c> 消费的是<b>用户级</b> access_token（由调用方显式传入）。
/// 进令牌注入管线会把应用级 access_token 错误地注入这些端点。</item>
/// <item><b>不建模用户级令牌管理器</b>（I5）：sns 的 access_token 每 openid 一份、refresh_token
/// 30 天且刷新后轮换——per-(app,openid) 键空间 + 轮换回写语义过重；SDK 只提供端点，
/// <b>refresh_token 的持久化与轮换归宿主</b>（守卫锁定：本接口不得出现 [Token]）。</item>
/// <item><b>secret 走 Query（官方契约）</b>：<c>sns/oauth2/access_token</c> 的 <c>secret</c> 即公众号
/// AppSecret、官方强制 Query 传（组件 <c>SensitiveUrlRedactor</c> 词表已覆盖 <c>secret</c>，URL 日志脱敏生效）。</item>
/// </list>
/// <para>
/// <b>域级业务约束（逐页核验）</b>：适用范围均为「服务号 —— 仅认证」（官方字段表）；
/// access_token / refresh_token / userinfo 三页频率限制原文均为「<b>5 万/分钟</b>」；
/// 不支持云调用与第三方平台调用（4 页官方声明）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Sns", TokenManage = nameof(IMpAppManager))]
public interface IMpSnsService
{
    /// <summary>
    /// 换取用户授权凭证（通过 code 换网页授权用户 access_token）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsaccesstoken.html"/>
    /// （官方接口英文名 <c>snsAccessToken</c>）。
    /// </summary>
    /// <param name="appId">公众号的唯一标识（官方 <c>appid</c>，必填）。</param>
    /// <param name="secret">公众号的 appsecret（官方 <c>secret</c>，必填；官方契约 Query 传，词表已覆盖脱敏）。</param>
    /// <param name="code">第一步（授权回调）获取的 code 参数（官方 <c>code</c>，必填）。</param>
    /// <param name="grantType">授权类型（官方 <c>grant_type</c>，必填；固定 <see cref="MpSnsGrantTypes.AuthorizationCode"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户级授权凭证（access_token / refresh_token / openid / unionid / is_snapshotuser）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>、无请求体；Query appid / secret / code / grant_type 均必填。
    /// 频率限制：<b>5 万/分钟</b>（官方原文）。
    /// </para>
    /// <para>
    /// <b>两种 scope（官方原文）</b>：snsapi_base 静默授权（仅取 openid）；snsapi_userinfo
    /// 需用户手动同意（可取基本信息，无须关注）。unionid 仅 snsapi_userinfo 作用域返回。
    /// </para>
    /// <para>官方错误码：<c>40029</c>（invalid code，无效的 code 参数）。</para>
    /// </remarks>
    [Get("/sns/oauth2/access_token")]
    Task<MpSnsAccessTokenResponse> GetAccessTokenAsync(
        [Query("appid")] string appId,
        [Query("secret")] string secret,
        [Query("code")] string code,
        [Query("grant_type")] string grantType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新用户授权凭证。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsrefreshtoken.html"/>
    /// （官方接口英文名 <c>snsRefreshToken</c>）。
    /// </summary>
    /// <param name="appId">公众号的唯一标识（官方 <c>appid</c>，必填）。</param>
    /// <param name="refreshToken">通过 access_token 接口获取到的 refresh_token（官方 <c>refresh_token</c>，必填）。</param>
    /// <param name="grantType">授权类型（官方 <c>grant_type</c>，必填；固定 <see cref="MpSnsGrantTypes.RefreshToken"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>刷新后的用户级授权凭证（含 scope）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>、无请求体；频率限制 <b>5 万/分钟</b>。
    /// </para>
    /// <para>
    /// <b>refresh_token 有效期 30 天</b>（官方原文「refresh_token 有效期为 30 天，当 refresh_token
    /// 失效之后，需要用户重新授权」）；刷新后 refresh_token 轮换——新值须由宿主回写持久化（I5 裁决）。
    /// </para>
    /// <para>官方错误码：<c>40029</c>（官方解决方案列原文为「js_code无效」，系小程序文案复用，照录）。</para>
    /// </remarks>
    [Get("/sns/oauth2/refresh_token")]
    Task<MpSnsRefreshTokenResponse> RefreshTokenAsync(
        [Query("appid")] string appId,
        [Query("refresh_token")] string refreshToken,
        [Query("grant_type")] string grantType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检验用户授权凭证（access_token 是否有效）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsauth.html"/>
    /// （官方接口英文名 <c>snsAuth</c>）。
    /// </summary>
    /// <param name="accessToken">网页授权接口调用凭证（官方 <c>access_token</c>，<b>用户级</b>，必填）。</param>
    /// <param name="openId">用户的唯一标识（官方 <c>openid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>检验结果（<see cref="MpResponse.IsSuccess"/> 为 true 表示有效；40003 表示 openid 不合法）。</returns>
    /// <remarks>
    /// 官方契约：<b>GET</b>、无请求体；有效凭证返回 <c>{"errcode":0,"errmsg":"ok"}</c>，
    /// 无效返回 <c>{"errcode":40003,"errmsg":"invalid openid"}</c>。
    /// 官方错误码：<c>40003</c>（invalid openid）。
    /// </remarks>
    [Get("/sns/auth")]
    Task<MpSnsAuthResponse> AuthAsync(
        [Query("access_token")] string accessToken,
        [Query("openid")] string openId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取授权用户信息（需 snsapi_userinfo 作用域）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsuserinfo.html"/>
    /// （官方接口英文名 <c>snsUserInfo</c>）。
    /// </summary>
    /// <param name="accessToken">网页授权接口调用凭证（官方 <c>access_token</c>，<b>用户级</b>，必填）。</param>
    /// <param name="openId">用户的唯一标识（官方 <c>openid</c>，必填）。</param>
    /// <param name="lang">返回语言版本（官方 <c>lang</c>，可选：zh_CN 简体 / zh_TW 繁体 / en 英语）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权用户资料（含 nickname / headimgurl 等完整字段——与 /cgi-bin/user/info 停供字段集不同）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>、无请求体；频率限制 <b>5 万/分钟</b>。
    /// </para>
    /// <para>
    /// <b>作用域前置（官方原文）</b>：必须使用以 snsapi_userinfo 构建的授权链接并获得用户同意，
    /// 否则返回 <c>48001</c>；因用户已同意，<b>无须关注公众号</b>即可获取基本信息。
    /// </para>
    /// <para>
    /// <b>字段集与 /cgi-bin/user/info 分别核验（勿照抄）</b>：本接口在有效授权下<b>仍返回</b>
    /// nickname / sex / province / city / country / headimgurl / privilege / unionid——
    /// 基础信息接口 2021-12-27 起已停供头像昵称（见 <see cref="User.MpUserInfo"/> remarks）。
    /// headimgurl「若用户更换头像，原有头像 URL 将失效」（官方原文）。
    /// </para>
    /// <para>官方错误码：<c>40003</c>（invalid openid）。</para>
    /// </remarks>
    [Get("/sns/userinfo")]
    Task<MpSnsUserInfoResponse> GetUserInfoAsync(
        [Query("access_token")] string accessToken,
        [Query("openid")] string openId,
        [Query("lang")] string? lang = null,
        CancellationToken cancellationToken = default);
}

/// <summary>网页授权 <c>grant_type</c> 取值（官方两值，各端点固定其一）。</summary>
public static class MpSnsGrantTypes
{
    /// <summary>通过 code 换取用户授权凭证（sns/oauth2/access_token 专用）。</summary>
    public const string AuthorizationCode = "authorization_code";

    /// <summary>通过 refresh_token 刷新用户授权凭证（sns/oauth2/refresh_token 专用）。</summary>
    public const string RefreshToken = "refresh_token";
}
