// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 创建售后请求体（<c>POST /shop/ecaftersale/add</c>）。
/// </summary>
/// <remarks>
/// <para><b>订单二选一</b>：<c>order_id</c> 与 <c>out_order_id</c> 官方均标选填且「二选一」；</para>
/// <para><c>out_aftersale_id</c> 亦为选填，未传则由微信侧生成 <c>aftersale_id</c> 并在应答回传。</para>
/// <para><b>商品只有一个</b>：<c>product_info</c> 是<b>单对象</b>（V1 的 <c>product_infos</c> 才是列表）⇒ 两代结构不得混用。</para>
/// <para><b>金额单位分</b>；<c>refund_reason</c> 与 <c>refund_reason_type</c> 均必填且须与 <see cref="WxaEcAftersaleReasonTypes"/> 的适用状态匹配。</para>
/// <para><b>敏感值</b>：<c>openid</c>、<c>refund_reason</c>、<c>media_list[].url</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleAddRequest
{
    /// <summary>微信侧订单号（与 <c>out_order_id</c> 二选一）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>商家自定义订单 ID（与 <c>order_id</c> 二选一）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>商家自定义售后 ID（选填；不传由微信生成）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>用户的 <c>openid</c>（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>售后类型（必填），见 <see cref="WxaEcAftersaleTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>售后商品（必填，单对象）。</summary>
    [JsonPropertyName("product_info")]
    public WxaEcAftersaleProductInfo? ProductInfo { get; set; }

    /// <summary>退款原因（必填，敏感）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>退款原因类型（必填），见 <see cref="WxaEcAftersaleReasonTypes"/>。</summary>
    [JsonPropertyName("refund_reason_type")]
    public int? RefundReasonType { get; set; }

    /// <summary>退款金额，单位<b>分</b>（必填）。</summary>
    [JsonPropertyName("orderamt")]
    public long? Orderamt { get; set; }

    /// <summary>图片附件（选填）。</summary>
    [JsonPropertyName("media_list")]
    public List<WxaEcAftersaleMedia>? MediaList { get; set; }
}
