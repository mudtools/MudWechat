// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>乘车码</b>域（直连商户侧）1 支端点：代扣订单查询。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：查询订单
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986341"/>。</para>
/// <para><b>灰度中、暂未开放</b>（官方页原文）：「本功能目前处于灰度测试阶段、暂未开放使用，
/// 待全量开放后将通过文档门户公告同步」⇒ 声明为契约但<b>不保证可调</b>，
/// 未开通产品权限时以 <c>PAYAUTH_ERROR</c> 返回，调用方须把它当作可预期分支处理。</para>
/// <para><b>不需证书、挂在 <c>api.mch</c></b>：官方接口链接栏为
/// <c>https://api.mch.weixin.qq.com/transit/queryorder</c>，全文无 <c>api2.mch</c>。</para>
/// <para><b>只查代扣订单</b>（官方原文「该接口仅提供微信代扣订单的查询」）：与基础支付的
/// <c>/pay/orderquery</c> 不通用，交易类型恒为 <c>PAP</c>，故本族<b>不与</b>基础支付共用应答 DTO。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "TransitV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2TransitService
{
    /// <summary>
    /// 乘车码订单查询（APIv2，直连商户侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986341"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2TransitQueryOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单状态与代金券明细，见 <see cref="V2TransitQueryOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/transit/queryorder</c>；
    /// <c>transaction_id</c> 与 <c>out_trade_no</c> <b>二选一</b>（微信单号优先）；
    /// <c>sign_type</c> 选填、官方默认 <c>HMAC-SHA256</c>。</para>
    /// <para><b>三级门控</b>：第三段字段（含金额、代金券、<c>time_end</c>）要求
    /// <c>return_code</c>、<c>result_code</c>、<c>trade_state</c> <b>同判 SUCCESS</b>；
    /// 官方原话「如 <c>trade_state</c> 不为 SUCCESS，则只返回 <c>out_trade_no</c>」⇒
    /// 终态判定看 <c>trade_state</c>，不能只看 <c>result_code</c>。</para>
    /// <para><b><c>trade_state</c> 本页三档</b>：<c>SUCCESS</c>／<c>ACCEPT</c>（已接收，等待扣款）／
    /// <c>PAY_FAIL</c>；<b>无 <c>REFUND</c></b>（服务商页有四档）⇒ <c>ACCEPT</c> 是常态中间态，
    /// 轮询须按官方建议「请使用相同参数再次调用」处理 <c>SYSTEMERROR</c>。</para>
    /// <para><b>代金券明细是平铺下标键</b>：<c>coupon_type_$n</c>／<c>coupon_id_$n</c>／<c>coupon_fee_$n</c>
    /// 落 <see cref="V2TransitQueryOrderResponse.IndexedFields"/>，条数取 <c>coupon_count</c>。</para>
    /// <para><b>不出身份标识</b>：应答的 <c>openid</c> 属用户身份标识，不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/transit/queryorder")]
    Task<V2TransitQueryOrderResponse> TransitQueryOrderAsync(
        V2TransitQueryOrderRequest request, CancellationToken cancellationToken = default);
}
