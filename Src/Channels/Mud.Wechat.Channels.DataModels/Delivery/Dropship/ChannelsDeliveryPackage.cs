// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>包裹发货物流信息（官方 <c>delivery_product_info</c> 数组元素）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>delivery_product_info</c> 元素字段集完全一致 ⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryPackage
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置包裹中商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsPackageProduct>? ProductInfos { get; set; }

    /// <summary>获取或设置快递公司名称（官方 <c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }

    /// <summary>获取或设置发货时间（官方 <c>delivery_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("delivery_time")]
    public long? DeliveryTime { get; set; }

    /// <summary>获取或设置配送方式（官方 <c>deliver_type</c>，官方未列出具体枚举值）。</summary>
    [JsonPropertyName("deliver_type")]
    public int? DeliverType { get; set; }

    /// <summary>获取或设置发货地址（官方 <c>delivery_address</c>，官方原文「字段与 address_info 相同」）。</summary>
    [JsonPropertyName("delivery_address")]
    public ChannelsDeliveryAddress? DeliveryAddress { get; set; }
}
