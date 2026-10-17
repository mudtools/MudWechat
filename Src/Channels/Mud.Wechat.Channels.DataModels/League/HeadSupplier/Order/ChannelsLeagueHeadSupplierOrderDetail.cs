// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>佣金单的订单详情（官方 <c>commssion_order.order_detail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierOrderDetail
{
    /// <summary>获取或设置小店商家信息（官方 <c>shop_info</c>）。</summary>
    [JsonPropertyName("shop_info")]
    public ChannelsLeagueHeadSupplierOrderShopInfo? ShopInfo { get; set; }

    /// <summary>获取或设置佣金单商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsLeagueHeadSupplierOrderProductInfo? ProductInfo { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsLeagueHeadSupplierOrderInfo? OrderInfo { get; set; }

    /// <summary>获取或设置分佣信息（官方 <c>commission_info</c>）。</summary>
    [JsonPropertyName("commission_info")]
    public ChannelsLeagueHeadSupplierOrderCommissionInfo? CommissionInfo { get; set; }

    /// <summary>获取或设置买家信息（官方 <c>buyer_info</c>）。</summary>
    [JsonPropertyName("buyer_info")]
    public ChannelsLeagueHeadSupplierOrderBuyerInfo? BuyerInfo { get; set; }
}
