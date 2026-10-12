// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>供货信息（官方 <c>supply_order_product_info</c> 对象，<c>supplyorder/get</c> 商品维度）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderProductInfo
{
    /// <summary>获取或设置供货单单号（官方 <c>supply_order_id</c>）。</summary>
    [JsonPropertyName("supply_order_id")]
    public string? SupplyOrderId { get; set; }

    /// <summary>获取或设置发货状态（官方 <c>delivery_status</c>，见 <see cref="ChannelsDeliveryStatuses"/>）。</summary>
    [JsonPropertyName("delivery_status")]
    public int? DeliveryStatus { get; set; }

    /// <summary>
    /// 获取或设置售后状态（官方 <c>aftersale_status</c>，见 <see cref="ChannelsSupplyAftersaleStatuses"/>）。
    /// </summary>
    [JsonPropertyName("aftersale_status")]
    public int? AftersaleStatus { get; set; }

    /// <summary>获取或设置取消时间（官方 <c>cancel_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>获取或设置取消原因（官方 <c>cancel_scene</c>，见 <see cref="ChannelsSupplyCancelScenes"/>）。</summary>
    [JsonPropertyName("cancel_scene")]
    public int? CancelScene { get; set; }

    /// <summary>获取或设置换 sku（官方 <c>preshipment_change_sku_state</c>）。</summary>
    [JsonPropertyName("preshipment_change_sku_state")]
    public int? PreshipmentChangeSkuState { get; set; }

    /// <summary>获取或设置供货价（单价）（官方 <c>supply_price</c>，单位：分）。</summary>
    [JsonPropertyName("supply_price")]
    public long? SupplyPrice { get; set; }
}
