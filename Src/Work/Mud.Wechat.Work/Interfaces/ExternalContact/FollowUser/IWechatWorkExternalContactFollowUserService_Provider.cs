// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块企业服务人员管理域服务商代开发 SDK。
/// <para>
/// 除继承自 <see cref="IWechatWorkExternalContactFollowUserService"/> 的公共端点外，
/// 本接口提供代开发应用独有的「检查用户是否配置了客户联系功能使用权限」端点
/// （按用户 / 部门逐项检查并将结果分为 authorized / unauthorized 两组）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalExternalContactFollowUserService"/>；
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactFollowUserService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ExternalContact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkExternalContactFollowUserService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderExternalContactFollowUserService : IWechatWorkExternalContactFollowUserService
{
    /// <summary>
    /// 检查用户是否配置了客户联系功能使用权限
    /// <para>企业和第三方服务商可通过此接口检查部门或成员是否具备客户联系权限。</para>
    /// </summary>
    /// <param name="request">待检查的身份集合（<see cref="CheckFollowUserRequest"/>：userid 与 partyid 均可空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>检查结果（authorized / unauthorized 两组，各含用户与部门 id 列表）。</returns>
    /// <remarks>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96312"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/check_follow_user")]
    Task<CheckFollowUserResponse> CheckFollowUserAsync(
        [Body] CheckFollowUserRequest request,
        CancellationToken cancellationToken = default);
}
