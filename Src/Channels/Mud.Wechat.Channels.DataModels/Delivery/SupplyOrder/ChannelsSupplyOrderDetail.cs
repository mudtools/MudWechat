// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>供货单详情（官方 <c>supply_order_detail</c> 对象，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderDetail
{
    /// <summary>获取或设置商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsSupplyProductInfo>? ProductInfos { get; set; }

    /// <summary>获取或设置价格信息（官方 <c>price_info</c>）。</summary>
    [JsonPropertyName("price_info")]
    public List<ChannelsSupplyPriceInfo>? PriceInfo { get; set; }

    /// <summary>获取或设置发货信息（官方 <c>delivery_info</c>）。</summary>
    [JsonPropertyName("delivery_info")]
    public ChannelsDeliveryInfo? DeliveryInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsOrderExtInfo? ExtInfo { get; set; }

    /// <summary>获取或设置需代写的商品贺卡信息（官方 <c>greeting_card_info</c>）。</summary>
    [JsonPropertyName("greeting_card_info")]
    public ChannelsGreetingCardInfo? GreetingCardInfo { get; set; }

    /// <summary>获取或设置商品定制信息（官方 <c>custom_info</c>）。</summary>
    [JsonPropertyName("custom_info")]
    public ChannelsCustomInfo? CustomInfo { get; set; }

    /// <summary>获取或设置运费（官方 <c>freight</c>，单位：分）。</summary>
    [JsonPropertyName("freight")]
    public long? Freight { get; set; }

    /// <summary>获取或设置售后信息（官方 <c>after_sale_orders</c>）。</summary>
    [JsonPropertyName("after_sale_orders")]
    public List<ChannelsSupplyAftersaleOrder>? AfterSaleOrders { get; set; }
}
