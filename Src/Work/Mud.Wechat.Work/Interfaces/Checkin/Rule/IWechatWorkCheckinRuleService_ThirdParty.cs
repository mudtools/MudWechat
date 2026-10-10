// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡规则域第三方应用 SDK（零差异端点空标记）。
/// <para>
/// 第三方应用仅可使用三类公共面端点「获取员工打卡规则」（继承自 <see cref="IWechatWorkCheckinRuleService"/>）；
/// 官方权限表对「获取企业所有打卡规则」（93384/99444）与「管理打卡规则」4 个写端点（98041/98767）均标注
/// 「第三方应用：暂不支持」，故本接口不承载差异端点（能力漂移守卫：官方暂不支持亦不得补端点）。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalCheckinRuleService"/>；服务商代开发见 <see cref="IWechatWorkProviderCheckinRuleService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：第三方应用须具有「打卡」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Checkin",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCheckinRuleService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyCheckinRuleService : IWechatWorkCheckinRuleService
{
}
