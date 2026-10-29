// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商品基础详情条目（官方 <c>headsupplier/productdetail/get</c> 返回的 <c>item</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProductDetailItem
{
    /// <summary>获取或设置所属小店 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppid { get; set; }

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，number）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsLeagueHeadSupplierProductDetailInfo? ProductInfo { get; set; }

    /// <summary>获取或设置店铺信息（官方 <c>shop</c>）。</summary>
    [JsonPropertyName("shop")]
    public ChannelsLeagueHeadSupplierShopInfo? Shop { get; set; }
}
