// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「收银台」模块发票管理域公共 SDK
/// （<c>/cgi-bin/paytool/</c> 下的发票路由族：获取发票列表 / 标记开票状态）。
/// </summary>
/// <remarks>
/// <para>
/// 官方在第三方应用开发与服务商代开发两棵文档树的「收银台 → 发票管理」分组下提供本族端点、
/// 共享同一端点页（第三方树 99436/99437 = 代开发树 99447/99448），
/// 企业自建应用开发文档树<b>无对应 API</b>；且走服务商凭证
/// <c>provider_access_token</c>，与自建 / 授权企业的 <c>access_token</c> 路由键不同，
/// 故按「独立 provider 令牌 ⇒ 独立成族」落位：父接口零端点（<see cref="IWechatWorkPayToolInvoiceService"/>
/// 为 <c>IsAbstract</c>），全部 2 个端点声明于唯一子接口
/// <see cref="IWechatWorkThirdPartyPayToolInvoiceService"/>。
/// </para>
/// <para>
/// <b>无需签名</b>：与同域「收款工具」族不同，本族 2 个端点的官方参数表<b>不含</b>
/// <c>nonce_str</c> / <c>ts</c> / <c>sig</c>，调用时不要附加签名三要素。
/// </para>
/// <para>
/// 同域另两族：收款工具族见 <see cref="IWechatWorkPayToolOrderService"/>（同走 provider_access_token，<b>需签名</b>）；
/// 应用版本付费族见 <see cref="IWechatWorkPayToolVersionSuiteService"/>（走 suite_access_token）。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkPayToolInvoiceService
{
}