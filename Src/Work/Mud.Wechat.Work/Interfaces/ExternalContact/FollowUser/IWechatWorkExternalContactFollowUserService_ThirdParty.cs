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
/// 企业微信「客户联系」模块企业服务人员管理域第三方应用 SDK。
/// <para>
/// 除继承自 <see cref="IWechatWorkExternalContactFollowUserService"/> 的公共端点外，
/// 本接口提供第三方应用独有的「获取客户可建联成员」端点（仅营销获客类应用可调用，
/// 返回营销获客应用在管理端配置的客户可建联成员范围）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalExternalContactFollowUserService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactFollowUserService"/>。</para>
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
public interface IWechatWorkThirdPartyExternalContactFollowUserService : IWechatWorkExternalContactFollowUserService
{
    /// <summary>
    /// 获取客户可建联成员
    /// <para>获取营销获客应用在管理端配置的客户可建联成员范围（仅营销获客类应用可调用）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可建联成员 / 部门 / 标签 ID 列表（user_list / department_list / tag_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101146"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/customer_acquisition_app/get_permit")]
    Task<GetCustomerAcquisitionPermitResponse> GetCustomerAcquisitionPermitAsync(
        CancellationToken cancellationToken = default);
}
