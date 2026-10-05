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
/// 官方对第三方应用<b>仅开放「获取应用共享信息」</b> 1 个端点（95324，与自建/代开发同路由同契约），
/// 随公共父接口 <see cref="IWechatWorkCorpGroupService"/> 继承；其余 5 个端点官方无第三方文档，
/// 已下沉至 <see cref="IWechatWorkCorpGroupInternalProviderService"/>（自建/代开发专属父接口，本接口不继承）。
/// </para>
/// <para>
/// 因此本接口的<b>类型化端点面恰为 1 个</b>（<see cref="IWechatWorkCorpGroupService.ListAppShareInfoAsync"/>）：
/// 第三方应用调用方在编译期即无法触及官方未开放的端点，运行期不会收到 errcode。
/// 本接口为应用类型空标记（形态对齐飞书用户态空接口 <c>IFeishuUserV1LingoEntity</c>）。
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
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyCorpGroupService : IWechatWorkCorpGroupService
{
}
