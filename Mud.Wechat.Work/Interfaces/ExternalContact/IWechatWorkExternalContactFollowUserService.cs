// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块企业服务人员管理域公共 SDK。
/// <para>
/// 三类应用公共面端点声明于本接口；能力差异端点声明于应用类型子接口：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactFollowUserService"/>（零差异端点，空标记），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactFollowUserService"/>
/// （获取客户可建联成员，仅营销获客类应用可调用），
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactFollowUserService"/>
/// （检查用户是否配置了客户联系功能使用权限）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「客户联系 可调用接口的应用」中；第三方 / 代开发应用须具有
/// 「客户基础信息」权限；应用仅可获取可见范围内的成员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactFollowUserService
{
    /// <summary>
    /// 获取配置了客户联系功能的成员列表
    /// <para>企业和第三方服务商可通过此接口获取配置了客户联系功能的成员列表；应用仅可获取可见范围内的成员。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配置了客户联系功能的成员 userid 列表（follow_user）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92571"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92576"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96310"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/get_follow_user_list")]
    Task<GetFollowUserListResponse> GetFollowUserListAsync(
        CancellationToken cancellationToken = default);
}
