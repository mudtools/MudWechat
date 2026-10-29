// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询订单」响应的部分退款记录（官方 <c>partial_refund_info</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxRefundRecord
{
    /// <summary>获取或设置商户退款单号（官方 <c>refund_out_id</c>）。</summary>
    [JsonPropertyName("refund_out_id")]
    public string? RefundOutId { get; set; }

    /// <summary>获取或设置微信退款单号（官方 <c>refund_order_id</c>）。</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_fee</c>，单位分）。</summary>
    [JsonPropertyName("refund_fee")]
    public int RefundFee { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>refund_reason</c>）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>获取或设置退款状态（官方 <c>refund_status</c>）。</summary>
    [JsonPropertyName("refund_status")]
    public string? RefundStatus { get; set; }

    /// <summary>获取或设置退款完成时间（官方 <c>refund_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("refund_finish_time")]
    public long? RefundFinishTime { get; set; }
}
