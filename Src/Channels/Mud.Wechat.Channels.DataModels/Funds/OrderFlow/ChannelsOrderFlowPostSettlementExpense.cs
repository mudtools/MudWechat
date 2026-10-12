// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>结算后支出（官方 <c>post_settlement_expense</c> 对象）。</summary>
/// <remarks>金额字段单位均为<b>分</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowPostSettlementExpense
{
    /// <summary>获取或设置买家退款（官方 <c>buyer_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("buyer_refund_amount")]
    public long? BuyerRefundAmount { get; set; }

    /// <summary>获取或设置平台优惠退款金额（官方 <c>platform_discount_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_discount_refund_amount")]
    public long? PlatformDiscountRefundAmount { get; set; }

    /// <summary>获取或设置达人退款金额（官方 <c>promoter_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_refund_amount")]
    public long? PromoterRefundAmount { get; set; }

    /// <summary>获取或设置运费险保费（官方 <c>freight_insurance_make_up_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_make_up_amount")]
    public long? FreightInsuranceMakeUpAmount { get; set; }

    /// <summary>获取或设置运费险补缴本单结算状态（官方 <c>freight_insurance_make_up_settle_state</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_settle_state")]
    public int? FreightInsuranceMakeUpSettleState { get; set; }

    /// <summary>获取或设置运费险补缴本单的订单 id（官方 <c>freight_insurance_make_up_order_id</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_order_id")]
    public long? FreightInsuranceMakeUpOrderId { get; set; }
}
