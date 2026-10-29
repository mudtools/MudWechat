// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>线索订单详情（官方 <c>commssion_clue.clue_detail</c>）。
/// 五个子对象与佣金单详情<b>字段集均不同</b>（shop_info 只有 appid、product_info 的 product_id 为 string 且
/// actual_payment 语义是「可分佣金额」、order_info 只有一个 order_status、sharer_info 无 ratio/amount）⇒ 全部另建；
/// 唯 buyer_info 与佣金单侧完全同形 ⇒ 复用。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierClueDetail
{
    /// <summary>获取或设置小店商家信息（官方 <c>shop_info</c>）。</summary>
    [JsonPropertyName("shop_info")]
    public ChannelsLeagueHeadSupplierClueShopInfo? ShopInfo { get; set; }

    /// <summary>获取或设置佣金单商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsLeagueHeadSupplierClueProductInfo? ProductInfo { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsLeagueHeadSupplierClueOrderInfo? OrderInfo { get; set; }

    /// <summary>获取或设置推客信息（官方 <c>sharer_info</c>）。</summary>
    [JsonPropertyName("sharer_info")]
    public ChannelsLeagueHeadSupplierClueSharerInfo? SharerInfo { get; set; }

    /// <summary>获取或设置买家信息（官方 <c>buyer_info</c>，与佣金单侧同形 ⇒ 复用）。</summary>
    [JsonPropertyName("buyer_info")]
    public ChannelsLeagueHeadSupplierOrderBuyerInfo? BuyerInfo { get; set; }
}
