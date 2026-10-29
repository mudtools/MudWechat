// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 订单（官方 <c>Order</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>status</c> 为数字</b>，见 <see cref="WxaProductOrderStatuses"/>；<c>openid</c> 属用户标识。</para>
/// <para><b>敏感值</b>：<c>openid</c>、<c>order_detail.pay_info</c>（<c>prepay_id</c> / <c>transaction_id</c>）、</para>
/// <para><c>delivery_info.address_info</c> 与两张备注均可能含个人信息 ⇒ 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrder
{
    /// <summary>订单号。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>订单状态，见 <see cref="WxaProductOrderStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>创建时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>订单详细数据。</summary>
    [JsonPropertyName("order_detail")]
    public WxaProductOrderDetail? OrderDetail { get; set; }

    /// <summary>售后信息。</summary>
    [JsonPropertyName("aftersale_detail")]
    public WxaProductOrderAfterSaleDetail? AftersaleDetail { get; set; }

    /// <summary>用户 <c>openid</c>（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>额外信息。</summary>
    [JsonPropertyName("ext_info")]
    public WxaProductOrderExtInfo? ExtInfo { get; set; }

    /// <summary>订单类型，见 <see cref="WxaProductOrderTypes"/>。</summary>
    [JsonPropertyName("order_type")]
    public long? OrderType { get; set; }
}
