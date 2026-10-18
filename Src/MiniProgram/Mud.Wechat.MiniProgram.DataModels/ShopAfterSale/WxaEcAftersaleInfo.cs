// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// V2 售后单详情（官方 <c>AfterSaleInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>时间是毫秒字符串</b>：<c>create_time</c> / <c>update_time</c> 官方标 <c>string</c>、单位 <b>ms</b> ⇒</para>
/// <para>与 V1 的 <c>yyyy-MM-dd HH:mm:ss</c> 不是同一口径，不得互转类型。</para>
/// <para><b><c>complaint_order_id_list</c> 最多一项</b>：官方原文「目前一个售后单只会有一个关联的纠纷单」。</para>
/// <para><b>敏感值</b>：<c>openid</c>、<c>return_address_info</c>（收件人姓名 / 电话 / 地址）属个人信息；</para>
/// <para><c>refund_pay_detail.refund_id</c> 属支付凭据 ⇒ 一律禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleInfo
{
    /// <summary>外部售后单号（与 <c>aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>微信侧售后单号。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }

    /// <summary>外部订单号。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>微信侧订单号。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>售后商品信息（单商品，非列表）。</summary>
    [JsonPropertyName("product_info")]
    public WxaEcAftersaleProductInfo? ProductInfo { get; set; }

    /// <summary>售后类型，见 <see cref="WxaEcAftersaleTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>退货信息。</summary>
    [JsonPropertyName("return_info")]
    public WxaEcAftersaleReturnInfo? ReturnInfo { get; set; }

    /// <summary>退款金额，单位<b>分</b>。</summary>
    [JsonPropertyName("orderamt")]
    public long? Orderamt { get; set; }

    /// <summary>售后理由类型，见 <see cref="WxaEcAftersaleReasonTypes"/>。</summary>
    [JsonPropertyName("refund_reason_type")]
    public int? RefundReasonType { get; set; }

    /// <summary>售后理由补充描述（敏感）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>图片附件列表。</summary>
    [JsonPropertyName("media_list")]
    public List<WxaEcAftersaleMedia>? MediaList { get; set; }

    /// <summary>售后状态，见 <see cref="WxaEcAftersaleStates"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>申请时间，单位毫秒（字符串）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间，单位毫秒（字符串）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>用户 <c>openid</c>（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>退款支付信息。</summary>
    [JsonPropertyName("refund_pay_detail")]
    public WxaEcAftersaleRefundPayDetail? RefundPayDetail { get; set; }

    /// <summary>物流助手 ID。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }

    /// <summary>关联纠纷单 ID 列表（最多一项）。</summary>
    [JsonPropertyName("complaint_order_id_list")]
    public List<long>? ComplaintOrderIdList { get; set; }

    /// <summary>售后单退货地址。</summary>
    [JsonPropertyName("return_address_info")]
    public WxaEcAftersaleReturnAddressInfo? ReturnAddressInfo { get; set; }
}
