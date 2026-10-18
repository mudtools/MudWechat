// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 售后单详情（V1 订单分区的 <c>after_sale_order</c>）。
/// </summary>
/// <remarks>
/// <para><b>枚举字段在应答里是符号名</b>：<c>status</c> 示例为 <c>"MERCHANT_REFUND_SUCCESS"</c>、</para>
/// <para><c>reason</c> 为 <c>"NO_LONGER_WANT"</c>、<c>type</c> 为 <c>"REFUND"</c>，而官方枚举表给的是数字 ⇒</para>
/// <para>本类三支一律建模为 <c>string</c>，值域说明见 <see cref="WxaProductOrderAfterSaleStatuses"/> /</para>
/// <para><see cref="WxaProductOrderAfterSaleReasonTypes"/>；<b>不得</b>按数字反序列化。</para>
/// <para><b>敏感值</b>：<c>openid</c>、<c>details.tel_number</c>、<c>details.prove_imgs</c>、<c>refund_resp</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductAfterSaleOrder
{
    /// <summary>售后单号（官方示例注释；非订单号）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>售后单状态（符号名，非数字）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>用户 <c>openid</c>（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>售后单对应的订单号。</summary>
    [JsonPropertyName("original_order_id")]
    public long? OriginalOrderId { get; set; }

    /// <summary>售后商品。</summary>
    [JsonPropertyName("product_info")]
    public WxaProductAfterSaleProductInfo? ProductInfo { get; set; }

    /// <summary>售后详情。</summary>
    [JsonPropertyName("details")]
    public WxaProductAfterSaleDetails? Details { get; set; }

    /// <summary>退款信息。</summary>
    [JsonPropertyName("refund_info")]
    public WxaProductAfterSaleRefundInfo? RefundInfo { get; set; }

    /// <summary>退货物流。</summary>
    [JsonPropertyName("return_info")]
    public WxaProductAfterSaleReturnInfo? ReturnInfo { get; set; }

    /// <summary>商家上传信息。</summary>
    [JsonPropertyName("merchant_upload_info")]
    public WxaProductAfterSaleMerchantUploadInfo? MerchantUploadInfo { get; set; }

    /// <summary>售后单创建时间，秒级时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>售后单更新时间，秒级时间戳。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>退款原因（符号名）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>退款响应。</summary>
    [JsonPropertyName("refund_resp")]
    public WxaProductAfterSaleRefundResp? RefundResp { get; set; }

    /// <summary>退款方式（符号名：REFUND / RETURN）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
