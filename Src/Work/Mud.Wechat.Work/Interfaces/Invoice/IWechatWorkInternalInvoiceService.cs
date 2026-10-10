// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「电子发票」域企业自建应用 SDK。
/// <para>
/// 官方向自建应用开放的电子发票端点集（查询电子发票、更新发票状态、批量更新发票状态、批量查询电子发票）
/// 与三类应用的公共面完全重合，全部继承自 <see cref="IWechatWorkInvoiceService"/>；
/// 因此本接口不新增端点，仅作为自建应用的类型化契约入口存在（形态对齐
/// <see cref="IWechatWorkInternalMediaService"/> 空标记子接口）。
/// </para>
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartyInvoiceService"/>；服务商代开发见 <see cref="IWechatWorkProviderInvoiceService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入）。
/// 官方权限口径：报销更新类接口仅认证的企业微信账号有接口权限；批量查询须企业激活人数超过 200。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Invoice",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkInvoiceService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalInvoiceService : IWechatWorkInvoiceService
{
}
