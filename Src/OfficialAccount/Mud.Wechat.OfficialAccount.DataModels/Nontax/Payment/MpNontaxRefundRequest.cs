// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「退款」请求（官方 <c>refund</c>）。
/// </summary>
/// <remarks>部分退款时 <see cref="RefundFee"/> 必填；全额退款可缺省。</remarks>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxRefundRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置退款原因（官方 <c>reason</c>，必填）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置退款金额（官方 <c>refund_fee</c>，单位分，可选；全额退款可缺省）。</summary>
    [JsonPropertyName("refund_fee")]
    public int? RefundFee { get; set; }

    /// <summary>获取或设置商户退款单号（官方 <c>refund_out_id</c>，可选；部分退款必填）。</summary>
    [JsonPropertyName("refund_out_id")]
    public string? RefundOutId { get; set; }
}
