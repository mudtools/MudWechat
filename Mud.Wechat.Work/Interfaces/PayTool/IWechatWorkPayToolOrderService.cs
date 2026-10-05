// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「收银台」模块收款工具域公共 SDK
/// （<c>/cgi-bin/paytool/</c> 下的收款订单路由族：创建收款订单 / 取消收款订单 / 获取收款订单列表 / 获取收款订单详情）。
/// </summary>
/// <remarks>
/// <para>
/// 官方仅在<b>第三方应用开发</b>文档树的「收银台」分组下提供本族端点（98045/98046/98053/98054），
/// 企业自建应用开发与服务商代开发文档树<b>均无对应 API</b>；且走服务商凭证
/// <c>provider_access_token</c>，与自建 / 授权企业的 <c>access_token</c> 路由键不同，
/// 故按「独立 provider 令牌 ⇒ 独立成族」落位：父接口零端点（<see cref="IWechatWorkPayToolOrderService"/>
/// 为 <c>IsAbstract</c>），全部 4 个端点声明于唯一子接口
/// <see cref="IWechatWorkThirdPartyPayToolOrderService"/>
/// （落位形态对齐 <see cref="IWechatWorkServicePromotionQrCodeService"/> 零端点父接口 + 唯一第三方子接口承载）。
/// </para>
/// <para>
/// <b>签名强制</b>：本族 4 个端点的请求体均须携带 <c>nonce_str</c> / <c>ts</c> / <c>sig</c>，
/// 签名算法见官方 <see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768 签名算法</see>
/// （HMAC-SHA256 + Base64，密钥取自「工作台→企业微信服务商助手→工具→收银台→收银台 API 调用密钥」），
/// SDK 侧实现见 <c>WechatPayToolSignature</c>。同域「发票管理」族<b>无需签名</b>。
/// </para>
/// <para>
/// 同域另两族：发票管理族见 <see cref="IWechatWorkPayToolInvoiceService"/>（同走 provider_access_token）；
/// 应用版本付费族见 <see cref="IWechatWorkPayToolVersionSuiteService"/>（走 suite_access_token）。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkPayToolOrderService
{
}