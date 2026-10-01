// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游」域第三方应用 SDK。
/// <para>
/// 官方对第三方应用仅开放<b>获取应用共享信息</b>端点（95324，与自建/代开发同路由同契约，
/// 已声明于 <see cref="IWechatWorkCorpGroupService"/>，随本接口继承）；其余 5 个端点
/// （下级/下游企业凭证、小程序 session、关联客户信息）官方无第三方文档。本接口不新增端点，
/// 仅作为第三方应用的类型化契约入口存在（形态对齐飞书用户态空接口 <c>IFeishuUserV1LingoEntity</c>）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalCorpGroupService"/>；服务商代开发见 <see cref="IWechatWorkProviderCorpGroupService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，即上级/上游企业应用凭证）。
/// 官方权限口径：获取应用共享信息对「自建应用和第三方应用」开放。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CorpGroup",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCorpGroupService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyCorpGroupService : IWechatWorkCorpGroupService
{
}
