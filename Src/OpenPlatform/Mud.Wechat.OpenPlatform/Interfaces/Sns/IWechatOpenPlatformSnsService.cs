// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.Sns;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台（第三方平台）「代公众号网页授权」域 SDK
/// （<c>sns/oauth2/component/access_token</c> 与 <c>sns/oauth2/component/refresh_token</c>——
/// 第三方平台<b>代公众号</b>完成网页授权换码 / 续期的两个端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌语义（官方契约，已逐端点核验）</b>：本组端点的令牌<b>值</b>是平台自身
/// <c>component_access_token</c>，且官方 query 参数名就是 <c>component_access_token</c>
/// （注意与「平台管理」域的 <c>access_token</c> 命名不同——两处均已按官方页面核验）。
/// </para>
/// <para>
/// <b>与公众号线 <c>/sns/oauth2/*</c> 的关系</b>：同前缀不同段（第三方平台版多
/// <c>component_appid</c> / <c>component_access_token</c> 两参、以平台令牌替代 secret）；
/// 守卫锁定两线路由零交叠。
/// </para>
/// <para>
/// MUD005 已知接受风险：开放平台官方契约强制令牌走 Query 参数，无法改用 Header；
/// URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <see cref="WechatOpenPlatformException"/> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Sns", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "component_access_token")]
public interface IWechatOpenPlatformSnsService
{
    /// <summary>
    /// 通过授权码换取网页授权的用户级令牌（第三方平台代公众号形态）。
    /// </summary>
    /// <param name="appId">公众号 <c>appid</c>（官方 <c>appid</c>，必填；被代授权的授权方）。</param>
    /// <param name="code">第一步获得的授权码（官方 <c>code</c>，必填；<b>一次性</b>）。</param>
    /// <param name="grantType">授权类型（官方 <c>grant_type</c>，固定 <c>authorization_code</c>；SDK 级默认值）。</param>
    /// <param name="componentAppId">第三方平台 <c>appid</c>（官方 <c>component_appid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户级令牌（<b>由调用方持久化与轮换</b>，refresh_token 30 天有效）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/Before_Develop/Official_Accounts/official_account_website_authorization.html"/></para>
    /// <para>官方约束：本端点有 <b>IP 白名单</b>限制（须在第三方平台侧配置）；<c>code</c> 一次性，重复使用报 <c>40029</c>。</para>
    /// <para>官方错误码：<c>40029</c>（invalid code）/ <c>40163</c>（code been used）/ <c>40125</c> 等。</para>
    /// </remarks>
    [Get("/sns/oauth2/component/access_token")]
    Task<OpenPlatformSnsComponentTokenResponse> GetSnsComponentAccessTokenAsync(
        [Query("appid")] string appId,
        [Query("code")] string code,
        [Query("grant_type")] string grantType = "authorization_code",
        [Query("component_appid")] string? componentAppId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新网页授权的用户级令牌（第三方平台代公众号形态）。
    /// </summary>
    /// <param name="appId">公众号 <c>appid</c>（官方 <c>appid</c>，必填）。</param>
    /// <param name="refreshToken">此前获得的刷新凭证（官方 <c>refresh_token</c>，必填）。</param>
    /// <param name="grantType">授权类型（官方 <c>grant_type</c>，固定 <c>refresh_token</c>；SDK 级默认值）。</param>
    /// <param name="componentAppId">第三方平台 <c>appid</c>（官方 <c>component_appid</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>刷新后的用户级令牌（原 <c>access_token</c> 失效语义由官方决定，新旧并存期照官方说明）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/Before_Develop/Official_Accounts/official_account_website_authorization.html"/></para>
    /// <para>官方约束：<c>refresh_token</c> 30 天有效，失效后须用户重新授权。</para>
    /// </remarks>
    [Get("/sns/oauth2/component/refresh_token")]
    Task<OpenPlatformSnsComponentTokenResponse> RefreshSnsComponentTokenAsync(
        [Query("appid")] string appId,
        [Query("refresh_token")] string refreshToken,
        [Query("grant_type")] string grantType = "refresh_token",
        [Query("component_appid")] string? componentAppId = null,
        CancellationToken cancellationToken = default);
}
