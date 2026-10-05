// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「电子发票」域第三方应用（Suite）SDK。
/// <para>
/// 官方向第三方应用开放的电子发票端点集（查询电子发票、更新发票状态、批量更新发票状态、批量查询电子发票）
/// 与三类应用的公共面完全重合，全部继承自 <see cref="IWechatWorkInvoiceService"/>；
/// 因此本接口不新增端点，仅作为第三方应用的类型化契约入口存在。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalInvoiceService"/>；服务商代开发见 <see cref="IWechatWorkProviderInvoiceService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（经 <c>get_corp_token</c> 以 <c>permanent_code</c> 换取，路由键
/// <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId）——调用前须经
/// <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// 官方权限口径：报销更新类接口仅认证的企业微信账号有接口权限；批量查询须企业激活人数超过 200。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Invoice",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkInvoiceService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyInvoiceService : IWechatWorkInvoiceService
{
}
