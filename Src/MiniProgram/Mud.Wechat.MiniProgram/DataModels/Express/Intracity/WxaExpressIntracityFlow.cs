// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送运费流水（<c>queryflow</c> 的 <c>flow_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityFlow
{
    /// <summary>流水类型（<c>flow_type</c>）。</summary>
    [JsonPropertyName("flow_type")]
    public int FlowType { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>支付单号（<c>pay_order_id</c>）。</summary>
    [JsonPropertyName("pay_order_id")]
    public string? PayOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>账单编号（<c>bill_id</c>）。</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>支付金额（<c>pay_amount</c>；单位：分）。</summary>
    [JsonPropertyName("pay_amount")]
    public int PayAmount { get; set; }

    /// <summary>退款金额（<c>refund_amount</c>；单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public int? RefundAmount { get; set; }

    /// <summary>扣款金额（<c>deduct_amount</c>；单位：分）。</summary>
    [JsonPropertyName("deduct_amount")]
    public int? DeductAmount { get; set; }

    /// <summary>支付状态（<c>pay_status</c>；0 待支付，1 已支付，2 支付失败）。</summary>
    [JsonPropertyName("pay_status")]
    public string? PayStatus { get; set; }

    /// <summary>退款状态（<c>refund_status</c>；0 待退款，1 已退款，2 退款失败）。</summary>
    [JsonPropertyName("refund_status")]
    public string? RefundStatus { get; set; }

    /// <summary>支付时间（<c>pay_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("pay_time")]
    public long PayTime { get; set; }

    /// <summary>退款时间（<c>refund_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("refund_time")]
    public long RefundTime { get; set; }

    /// <summary>创建时间（<c>create_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>消费截止时间（<c>consume_deadline</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("consume_deadline")]
    public long? ConsumeDeadline { get; set; }

    /// <summary>配送状态（<c>delivery_status</c>；1 待取货，2 已取货，3 已送达，4 已取消）。</summary>
    [JsonPropertyName("delivery_status")]
    public int? DeliveryStatus { get; set; }

    /// <summary>配送完成时间（<c>delivery_finished_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("delivery_finished_time")]
    public long? DeliveryFinishedTime { get; set; }

    /// <summary>累计支付金额（<c>total_pay_amt</c>；单位：分）。</summary>
    [JsonPropertyName("total_pay_amt")]
    public int? TotalPayAmount { get; set; }

    /// <summary>累计退款金额（<c>total_refund_amt</c>；单位：分）。</summary>
    [JsonPropertyName("total_refund_amt")]
    public int? TotalRefundAmount { get; set; }

    /// <summary>累计扣款金额（<c>total_deduct_amt</c>；单位：分）。</summary>
    [JsonPropertyName("total_deduct_amt")]
    public int? TotalDeductAmount { get; set; }
}
