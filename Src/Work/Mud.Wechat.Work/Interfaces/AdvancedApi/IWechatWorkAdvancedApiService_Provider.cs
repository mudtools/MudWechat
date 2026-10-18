// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「高级接口（订单与代付）」域<b>服务商代开发 / 第三方应用</b> SDK（类型化契约入口）。
/// <para>官方 6 支端点全部以 <c>provider_access_token</c> 鉴权，声明于公共父接口
/// <see cref="IWechatWorkAdvancedApiService"/>，本接口为其服务商凭证形态的契约入口。</para>
/// </summary>
/// <remarks>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// <para>
/// 令牌类型为 <c>WechatTokenTypes.ProviderAccessToken</c>（服务商级），故不声明 <c>TokenManagerKey</c> 归属域键。
/// </para>
/// <para>
/// <b>高危写面提示</b>：本接口的 <see cref="IWechatWorkAdvancedApiService.CreateOrderAsync"/> 与
/// <see cref="IWechatWorkAdvancedApiService.SubmitOrderPayAsync"/> 分别产生真实订单与不可变更支付方式的
/// 扣款任务，调用方应置于自身审批 / 幂等保护之下。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AdvancedApi",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAdvancedApiService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkProviderAdvancedApiService : IWechatWorkAdvancedApiService
{
}
