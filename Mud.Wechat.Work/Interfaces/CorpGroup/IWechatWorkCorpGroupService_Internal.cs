// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游」域企业自建应用 SDK。
/// <para>
/// 官方对自建应用开放了与代开发应用完全一致的 6 个上下游端点 = 公共父接口的「获取应用共享信息」
/// + <see cref="IWechatWorkCorpGroupInternalProviderService"/> 的 5 个自建/代开发专属端点，
/// 全部经继承获得；本接口不新增端点，仅作为自建应用的类型化契约入口存在
/// （形态对齐飞书用户态空接口 <c>IFeishuUserV1LingoEntity</c>）。
/// </para>
/// <para>服务商代开发见 <see cref="IWechatWorkProviderCorpGroupService"/>；
/// 第三方应用仅开放 1 个端点，见 <see cref="IWechatWorkThirdPartyCorpGroupService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，即上级/上游企业应用凭证）。
/// 官方权限口径：应用须为上下游共享的应用，多数端点要求客户联系权限与主体认证。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CorpGroup",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCorpGroupInternalProviderService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalCorpGroupService : IWechatWorkCorpGroupInternalProviderService
{
}
