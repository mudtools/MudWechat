// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构商品推广详情（官方 <c>promotiondetail/get</c> 的 <c>item</c> 对象）。</summary>
/// <remarks>官方注明：本对象只含<b>推广参数</b>（佣金率、推广链接等），商品基础信息（图、类目等）须走「获取商品基础详情」<c>productdetail/get</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierPromotionDetailItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品所属店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppid { get; set; }

    /// <summary>获取或设置供货来源（官方 <c>spu_source</c>：1 商家供货 / 2 机构供货）。</summary>
    [JsonPropertyName("spu_source")]
    public int? SpuSource { get; set; }

    /// <summary>获取或设置商品佣金信息（官方 <c>commission_info</c>）。</summary>
    [JsonPropertyName("commission_info")]
    public ChannelsLeagueHeadSupplierPromotionCommissionInfo? CommissionInfo { get; set; }

    /// <summary>获取或设置供货机构信息（官方 <c>head_supplier_info</c>）。</summary>
    [JsonPropertyName("head_supplier_info")]
    public ChannelsLeagueHeadSupplierInfo? HeadSupplierInfo { get; set; }

    /// <summary>获取或设置商品合作信息（官方 <c>cooperative_info</c>）。</summary>
    [JsonPropertyName("cooperative_info")]
    public ChannelsLeagueHeadSupplierCooperativeInfo? CooperativeInfo { get; set; }
}
