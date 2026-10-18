// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>服务商乘车码</b> 域 1 支端点：代扣订单查询。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 服务商侧，2026-10-18 逐页核验）：查询订单
/// <see href="https://pay.weixin.qq.com/doc/v2/partner/4011989909"/>。</para>
/// <para><b>与直连族的差异是超集而非改名</b>：请求面多 <c>sub_appid</c>（选填）、<c>sub_mch_id</c>（<b>必填</b>）、
/// <c>contract_id</c>（选填）、<c>version</c>（选填，接口版本号）；应答面多 <c>sub_appid</c>／<c>sub_mch_id</c>／
/// <c>sub_openid</c>／<c>user_repaid</c>／<c>contract_id</c> 五支 ⇒ 直连应答字段集是本应答的<b>真子集</b>，
/// 两个 DTO 不共用。</para>
/// <para><b>同样处于灰度</b>：官方页原文「本功能目前处于灰度测试阶段、暂未开放使用，
/// 待全量开放后将通过文档门户公告同步」；亦不需证书、挂在 <c>api.mch</c>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerTransitV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2PartnerTransitService
{
    /// <summary>
    /// 服务商乘车码订单查询（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011989909"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2PartnerTransitQueryOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单状态、子商户标识与垫资信息，见 <see cref="V2PartnerTransitQueryOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/transit/partnerpay/queryorder</c>；
    /// <c>transaction_id</c> 与 <c>out_trade_no</c> 二选一；<c>sign_type</c> 选填、默认 <c>HMAC-SHA256</c>。</para>
    /// <para><b><c>version=2.0</c> 才能看到垫资</b>：官方注「当前支付订单如发生垫资，则会在查单结果中返回
    /// 垫资相关信息（请注意对新增的信息做合适的处理，避免出现兼容问题）」，
    /// 且 <c>bank_type</c> 在「垫资 + <c>version</c>=2.0」时固定返回 <c>BPA</c>，配套
    /// <c>user_repaid</c>（<c>Y</c>已还款 / <c>N</c>未还款）⇒ 这两个字段<b>只出不进</b>、
    /// 且官方给出使用前置条件（须先确认 <c>bank_type=BPA</c> 且 <c>trade_state=SUCCESS</c>）。</para>
    /// <para><b><c>trade_state</c> 本页有四档</b>：<c>SUCCESS</c>／<c>ACCEPT</c>／<c>PAY_FAIL</c>／
    /// <b><c>REFUND</c></b>（直连页无 <c>REFUND</c>，跨页不一致照抄）。</para>
    /// <para><b>三级门控同直连</b>：交易明细段要求 <c>return_code</c>、<c>result_code</c>、<c>trade_state</c>
    /// 同为 SUCCESS，否则只回 <c>out_trade_no</c>。</para>
    /// <para><b>代金券明细是平铺下标键</b>：落
    /// <see cref="V2PartnerTransitQueryOrderResponse.IndexedFields"/>，条数取 <c>coupon_count</c>。</para>
    /// <para><b>不出身份标识</b>：<c>openid</c>／<c>sub_openid</c> 属用户身份标识，不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/transit/partnerpay/queryorder")]
    Task<V2PartnerTransitQueryOrderResponse> PartnerTransitQueryOrderAsync(
        V2PartnerTransitQueryOrderRequest request, CancellationToken cancellationToken = default);
}
