// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「基础交易」域 SDK（APIv3 下单 / 查单 / 关单）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验）：
/// JSAPI/小程序下单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>。</para>
/// <para><b>与其它产品线的根本差异 —— 无 access_token</b>：APIv3 全程以商户 API 证书做
/// RSA-SHA256 请求签名，故本接口<b>不声明 <c>[Token]</c></b>（守卫 PAY-B1 fail-closed 锁定，
/// 违反即 MUD005 与 Query 白名单漂移）。签名在传输层
/// <c>WechatPayAuthorizationHandler</c> 完成，端点方法只管路由与报文。</para>
/// <para><b>客户端</b>：走支付专用 <c>IWechatPayHttpClient</c>（命名客户端 <c>wechat-pay</c>，
/// BaseAddress 恒为 <c>https://api.mch.weixin.qq.com</c>）——不复用 <c>IEnhancedHttpClient</c>
/// 默认实例，否则会与企微线的 <c>qyapi.weixin.qq.com</c> 撞 BaseUrl 并给企微请求套上商户签名头。</para>
/// <para><b>不做运行时多态</b>：每个产品族各自一个端点方法 + 各自请求 DTO（设计方案 §2.2）。
/// APP / H5 / Native / 小程序 / 合单各族的差异只体现在<b>报文字段</b>上，用类型精确表达，
/// 不做「一个方法接枚举参数」的运行期分派 —— AOT 下源生成按声明类型序列化，多态无从谈起。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Transactions", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTransactionsService
{
    /// <summary>
    /// JSAPI / 小程序下单，获取 <c>prepay_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>
    /// （官方接口名 <c>DirectAPIv3JsapiPrepay</c>）。
    /// </summary>
    /// <param name="request">下单请求体，字段见 <see cref="JsapiPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅含 <c>prepay_id</c>（2 小时有效）的应答，见 <see cref="JsapiPrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/pay/transactions/jsapi</c>；
    /// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。</para>
    /// <para><b>业务限制（官方原文要点）</b>：① <c>out_trade_no</c> 须 6-32 字符、
    /// 仅数字与大小写字母 <c>_-|*</c>、同一商户号下唯一，重复提交报 <c>OUT_TRADE_NO_USED</c>(403)；
    /// ② <c>appid</c> 须与 <c>mchid</c> 有绑定关系，否则 <c>APPID_MCHID_NOT_MATCH</c>(400)；
    /// ③ <c>time_expire</c> 须在下单时间 15 天内、且不得早于下单后 1 分钟（官方会自动调整），
    /// 超时须先调关闭订单接口再以新商户订单号重下；④ 未指定 <c>time_expire</c> 时默认 7 天未支付即失效。</para>
    /// <para><b>频率</b>：超限返回 <c>FREQUENCY_LIMITED</c>(429)，官方要求降低请求频率。</para>
    /// <para><b>金额</b>：<c>amount.total</c> 单位为分、必须大于 0（1 元填 100）。</para>
    /// </remarks>
    [Post("/v3/pay/transactions/jsapi")]
    Task<JsapiPrepayResponse> CreateJsapiOrderAsync(
        JsapiPrepayRequest request,
        CancellationToken cancellationToken = default);
}
