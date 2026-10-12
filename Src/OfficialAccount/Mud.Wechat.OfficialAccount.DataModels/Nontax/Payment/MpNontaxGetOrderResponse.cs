// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询订单」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetOrderResponse : MpResponse
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>获取或设置支付方式（官方 <c>fee_type</c>）。</summary>
    [JsonPropertyName("fee_type")]
    public int FeeType { get; set; }

    /// <summary>获取或设置订单总金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置微信支付订单号（官方 <c>trans_id</c>，可选）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>bank_name</c>，可选）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户退款单号（官方 <c>refund_out_id</c>，可选）。</summary>
    [JsonPropertyName("refund_out_id")]
    public string? RefundOutId { get; set; }

    /// <summary>获取或设置微信退款单号（官方 <c>refund_order_id</c>，可选）。</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>refund_reason</c>，可选）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>获取或设置退款完成时间（官方 <c>refund_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("refund_finish_time")]
    public long? RefundFinishTime { get; set; }

    /// <summary>获取或设置部分退款记录（官方 <c>partial_refund_info</c>，可选）。</summary>
    [JsonPropertyName("partial_refund_info")]
    public MpNontaxRefundRecord[]? PartialRefundInfo { get; set; }

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，可选）。</summary>
    [JsonPropertyName("department_code")]
    public string? DepartmentCode { get; set; }

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>）。</summary>
    [JsonPropertyName("department_name")]
    public string? DepartmentName { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置缴费信息来源（官方 <c>payment_info_source</c>）。</summary>
    [JsonPropertyName("payment_info_source")]
    public int PaymentInfoSource { get; set; }

    /// <summary>获取或设置票据类型编码（官方 <c>bill_type_code</c>，可选）。</summary>
    [JsonPropertyName("bill_type_code")]
    public string? BillTypeCode { get; set; }

    /// <summary>获取或设置票据号（官方 <c>bill_no</c>，可选）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>）。</summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>获取或设置场景（官方 <c>scene</c>）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>获取或设置订单明细列表（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public MpNontaxOrderBillItem[]? ItemList { get; set; }

    /// <summary>获取或设置通知历史（官方 <c>notify_history</c>）。</summary>
    [JsonPropertyName("notify_history")]
    public MpNontaxNotifyHistory[]? NotifyHistory { get; set; }

    /// <summary>获取或设置支付完成时间（官方 <c>pay_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long? PayFinishTime { get; set; }

    /// <summary>获取或设置下单时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}
