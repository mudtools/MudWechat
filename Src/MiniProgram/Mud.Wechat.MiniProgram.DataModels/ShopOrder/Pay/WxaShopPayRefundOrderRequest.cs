// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 请求退款（<c>/shop/pay/refundorder</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayRefundOrderRequest
{
    /// <summary>用户 openid（<b>必填</b>；敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>订单对应的商家商户号（<b>必填</b>；敏感，不得进日志）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商家交易单号（<b>必填</b>）。</summary>
    [JsonPropertyName("trade_no")]
    public string? TradeNo { get; set; }

    /// <summary>支付单号（<b>必填</b>；敏感，不得进日志）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商家退款单号（<b>必填</b>）：只能是数字、大小写字母与 <c>_-|*@</c>，长度 6～32，小程序系统内唯一；<b>同一退款单号多次请求只退一笔</b>。</summary>
    [JsonPropertyName("refund_no")]
    public string? RefundNo { get; set; }

    /// <summary>订单总金额（<b>必填</b>），<b>单位分</b>。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>退款金额（<b>必填</b>），<b>单位分</b>。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }
}
