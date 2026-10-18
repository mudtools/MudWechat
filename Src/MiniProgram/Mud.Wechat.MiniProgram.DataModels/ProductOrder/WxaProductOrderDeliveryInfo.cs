// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 配送信息（官方 <c>DeliveryInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>express_fee</c> 按数组建模</b>：官方表标 object、示例给数组 ⇒ 以示例为准并在此留档。</para>
/// <para><b>敏感值</b>：<c>address_info</c> / <c>pickup_address</c> 与运单号禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderDeliveryInfo
{
    /// <summary>收件地址。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductOrderAddressInfo? AddressInfo { get; set; }

    /// <summary>配送方式（目前只有「快递」）。</summary>
    [JsonPropertyName("delivery_method")]
    public string? DeliveryMethod { get; set; }

    /// <summary>运费方式列表（官方示例为数组）。</summary>
    [JsonPropertyName("express_fee")]
    public List<WxaProductOrderDeliveryExpressFee>? ExpressFee { get; set; }

    /// <summary>发货物流信息列表。</summary>
    [JsonPropertyName("delivery_product_info")]
    public List<WxaProductOrderDeliveryProductInfo>? DeliveryProductInfo { get; set; }

    /// <summary>发货完成时间，秒级时间戳。</summary>
    [JsonPropertyName("ship_done_time")]
    public long? ShipDoneTime { get; set; }

    /// <summary>运费险信息。</summary>
    [JsonPropertyName("insurance_info")]
    public WxaProductOrderInsuranceInfo? InsuranceInfo { get; set; }

    /// <summary>配送方式，见 <see cref="WxaProductOrderDeliveryTypes"/>。</summary>
    [JsonPropertyName("deliver_type")]
    public string? DeliverType { get; set; }

    /// <summary>线下送达时间，秒级时间戳。</summary>
    [JsonPropertyName("offline_delivery_time")]
    public long? OfflineDeliveryTime { get; set; }

    /// <summary>线下自提时间，秒级时间戳。</summary>
    [JsonPropertyName("offline_pickup_time")]
    public long? OfflinePickupTime { get; set; }

    /// <summary>自提地址。</summary>
    [JsonPropertyName("pickup_address")]
    public WxaProductOrderAddressInfo? PickupAddress { get; set; }
}
