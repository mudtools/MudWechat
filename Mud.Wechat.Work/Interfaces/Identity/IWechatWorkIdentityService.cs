// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Identity;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「身份验证」模块网页授权登录域公共 SDK
/// （获取访问用户身份 + 获取访问用户敏感信息；企业微信 Web 登录复用同两端点换取登录用户身份）。
/// <para>
/// 官方对自建应用与服务商代开发开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalIdentityService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderIdentityService"/>。
/// 第三方应用走独立路由 <c>/cgi-bin/service/auth/getuserinfo3rd</c> / <c>getuserdetail3rd</c>
/// （suite_access_token 鉴权），见 <see cref="IWechatWorkIdentitySuiteService"/> 接口族，不在本接口收敛面内。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 自建应用消费应用自身 access_token；服务商代开发消费授权企业级 access_token（scope = authCorpId）——
/// 令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 官方约束：跳转的域名须完全匹配 access_token 对应应用的可信域名，否则返回 50001；
/// code 最大 512 字节、只能使用一次、5 分钟未被使用自动过期。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkIdentityService
{
    /// <summary>
    /// 获取访问用户身份
    /// <para>网页授权登录（构造网页授权链接颁发 code）与企业微信 Web 登录（Web 登录组件颁发 code）共用本端点换取用户身份：</para>
    /// <para>用户为企业成员时返回 userid（互联企业/企业互联/上下游场景格式为 <c>CorpId/userid</c>）；
    /// 授权 scope 为 snsapi_privateinfo 且用户在应用可见范围内时返回 user_ticket（有效期 1800 秒），
    /// 可凭其调用 <see cref="GetUserDetailAsync"/> 获取敏感信息（上下游/企业互联场景暂不支持）；
    /// 拥有智能专区文档存档权限的应用还会返回 user_doc_ticket（灰度内测中）。</para>
    /// <para>用户非企业成员时返回 openid；当且仅当用户是企业的客户且跟进人在应用可见范围内时返回 external_userid。</para>
    /// </summary>
    /// <param name="code">通过成员授权获取到的 code（官方必填；最大 512 字节，每次授权不同，仅可使用一次，5 分钟未被使用自动过期）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>访问用户身份（userid / user_ticket / user_doc_ticket，或 openid / external_userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用·网页授权登录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91023"/></para>
    /// <para><b>企业自建应用·企业微信Web登录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98176"/></para>
    /// <para><b>服务商代开发·网页授权登录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96442"/></para>
    /// <para><b>服务商代开发·企业微信Web登录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98177"/></para>
    /// </remarks>
    [Get("/cgi-bin/auth/getuserinfo")]
    Task<GetUserInfoResponse> GetUserInfoAsync(
        [Query("code")] string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取访问用户敏感信息
    /// <para>凭 <see cref="GetUserInfoAsync"/> 返回的 user_ticket 换取成员敏感信息；
    /// 成员必须在应用的可见范围内；敏感字段需要管理员在应用详情里选择、且成员 oauth2 授权时确认后才返回
    ///（敏感字段包括：性别、头像、员工个人二维码、手机、邮箱、企业邮箱、地址）。</para>
    /// </summary>
    /// <param name="request">敏感信息请求体（<see cref="GetUserDetailRequest"/>：user_ticket）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>访问用户敏感信息（userid / gender / avatar / qr_code / mobile / email / biz_mail / address）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95833"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96443"/></para>
    /// </remarks>
    [Post("/cgi-bin/auth/getuserdetail")]
    Task<GetUserDetailResponse> GetUserDetailAsync(
        [Body] GetUserDetailRequest request,
        CancellationToken cancellationToken = default);
}
