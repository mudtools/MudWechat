// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 发货物流（官方 <c>DeliveryProductInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>敏感值</b>：<c>waybill_id</c> 与发货地址禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderDeliveryProductInfo
{
    /// <summary>快递单号（敏感）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>快递公司编码。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司名称。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }

    /// <summary>发货时间，秒级时间戳。</summary>
    [JsonPropertyName("delivery_time")]
    public long? DeliveryTime { get; set; }

    /// <summary>配送方式，见 <see cref="WxaProductOrderDeliveryTypes"/>。</summary>
    [JsonPropertyName("deliver_type")]
    public string? DeliverType { get; set; }

    /// <summary>发货地址。</summary>
    [JsonPropertyName("delivery_address")]
    public WxaProductOrderAddressInfo? DeliveryAddress { get; set; }

    /// <summary>包裹中商品列表。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaProductOrderFreightProductInfo>? ProductInfos { get; set; }
}
