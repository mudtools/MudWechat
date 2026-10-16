// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.WebizpayBill;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「微企付企业支付 · 企业支付账单与出资凭证」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b> ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>「普通服务商」身份口径</b>：本族 12 页「支持商户」栏<b>恒为【普通服务商】</b>（无【平台商户】/【普通商户】分身页），
/// 请求一律以 <c>sp_mchid</c>（服务商商户号）+ <c>sub_mchid</c>（出资子商户号）双层定位 ⇒ 服务商身份由调用侧签名承载；
/// <b>唯一例外</b>是门店主体匹配两支只印 <c>sp_mchid</c>（守卫 WEB-ST）⇒ 不得跨域补字段。</para>
/// <para><b>两支四型、两两不同形</b>：出资凭证 6 行请求 / 11 行应答，业务账单 4 行请求 / 7 行应答 ⇒ 请求与应答各建一型， <b>不得</b>因「都返回 <c>download_url</c>」合并。</para>
/// <para><b>下载地址 5 分钟内有效</b>（官方原话：凭证「地址5分钟内有效」、账单「该地址5min内有效」）⇒ 取到 URL 必须立即下载， 不得缓存复用；账单支另带 <c>hash_type</c>（官方<b>固定 SHA1</b>）与 <c>hash_value</c>（文件 SHA1 摘要，供服务商侧校验一致性）， 凭证支<b>零摘要字段</b>。</para>
/// <para><b>日期口径不同</b>：凭证支 <c>start_date</c> + <c>end_date</c>（区间，两者均「仅支持三个月内」）；账单支只有 <c>date</c> （官方明写「<b>仅支持按一天的纬度查询</b>」、三个月内）且 <c>type</c> 当前<b>只有 <c>QYZFALL</c></b> 一个取值 ⇒ 不得把区间套到账单支。</para>
/// <para><b><c>entity_name</c> 只在凭证支</b>（选填，指定主体名称时仅下载该主体下所有收单商户的企业支付订单，用于找对应收单商户开票）； 账单支零该字段 ⇒ 两支请求 DTO 不得互套。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "WebizpayBill", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayWebizpayBillService
{
    /// <summary>
    /// 申请企业商户企业支付出资凭证。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507157"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="request">申请企业商户企业支付出资凭证请求体，字段见 <see cref="WebizpayBillProofRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请企业商户企业支付出资凭证，见 <see cref="WebizpayBillProofResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/bill/proof</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayBillProofResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/bill/proof")]
    Task<WebizpayBillProofResponse> ApplyBillProofAsync(
        [Body] WebizpayBillProofRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请企业商户企业支付业务账单。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507142"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="request">申请企业商户企业支付业务账单请求体，字段见 <see cref="WebizpayBillTradeBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请企业商户企业支付业务账单，见 <see cref="WebizpayBillTradeBillResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/bill/trade-bill</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayBillTradeBillResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/bill/trade-bill")]
    Task<WebizpayBillTradeBillResponse> ApplyTradeBillAsync(
        [Body] WebizpayBillTradeBillRequest request,
        CancellationToken cancellationToken = default);
}
