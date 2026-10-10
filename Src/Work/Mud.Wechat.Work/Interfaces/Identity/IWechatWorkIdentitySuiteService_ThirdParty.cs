// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Identity;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块第三方网页授权登录域第三方应用 SDK：
/// 官方仅向第三方应用开放本域端点（获取访问用户身份 / 获取访问用户敏感信息；
/// 企业微信 Web 登录复用同两端点换取登录用户身份），全部声明于本接口。
/// <para>服务商代开发官方不开放本族端点（不允许代开发自建应用调用），不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费服务商套件级 suite_access_token（路由键 <see cref="WechatTokenTypes.SuiteAccessToken"/>）。
/// 官方约束：跳转的域名须完全匹配 suite_access_token 对应第三方应用的可信域名，否则返回 50001。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Identity",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkIdentitySuiteService))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkThirdPartyIdentitySuiteService : IWechatWorkIdentitySuiteService
{
    /// <summary>
    /// 获取访问用户身份（第三方）
    /// <para>网页授权登录（构造网页授权链接颁发 code）与企业微信 Web 登录（Web 登录组件颁发 code）共用本端点换取用户身份：</para>
    /// <para>用户属于某企业时返回 corpid / userid / open_userid
    ///（该企业与第三方应用无授权关系时 userid 返回密文 UserId；open_userid 全局唯一、
    /// 对同一服务商不同应用取同一成员结果相同，仅第三方应用可获取）；
    /// 授权 scope 为 snsapi_privateinfo 且用户在应用可见范围内时返回 user_ticket（随 expires_in 返回有效秒数）；
    /// 拥有智能专区文档存档权限的应用还会返回 user_doc_ticket（灰度内测中）。</para>
    /// <para>用户不属于任何企业时返回 openid（对当前服务商唯一）。</para>
    /// </summary>
    /// <param name="code">通过成员授权获取到的 code（官方必填；最大 512 字节，每次授权不同，仅可使用一次，5 分钟未被使用自动过期）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>访问用户身份（corpid / userid / open_userid / user_ticket 或 openid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用·网页授权登录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91121"/></para>
    /// <para><b>第三方应用·企业微信Web登录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98179"/></para>
    /// </remarks>
    [Get("/cgi-bin/service/auth/getuserinfo3rd")]
    Task<GetUserInfo3rdResponse> GetUserInfo3rdAsync(
        [Query("code")] string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取访问用户敏感信息（第三方）
    /// <para>凭 <see cref="GetUserInfo3rdAsync"/> 返回的 user_ticket 换取成员敏感信息；成员必须在授权应用的可见范围内。</para>
    /// <para>第三方应用仅返回 corpid / userid / name / gender / avatar / qr_code，
    /// 不含手机、邮箱、企业邮箱、地址等字段（对齐官方 91122 响应面）。</para>
    /// </summary>
    /// <param name="request">敏感信息请求体（<see cref="GetUserDetail3rdRequest"/>：user_ticket）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>访问用户敏感信息（corpid / userid / name / gender / avatar / qr_code）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91122"/></para>
    /// <para>官方契约陷阱：name 字段自 2019 年 12 月 30 日起对新创建第三方应用不再返回真实姓名（以 userid 代替返回），
    /// 2020 年 6 月 30 日起对所有历史第三方应用生效，展示姓名须改用「通讯录展示组件」。</para>
    /// </remarks>
    [Post("/cgi-bin/service/auth/getuserdetail3rd")]
    Task<GetUserDetail3rdResponse> GetUserDetail3rdAsync(
        [Body] GetUserDetail3rdRequest request,
        CancellationToken cancellationToken = default);
}
