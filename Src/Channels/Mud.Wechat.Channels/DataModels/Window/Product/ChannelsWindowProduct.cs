// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 橱窗商品详情（官方 <c>product</c> / <c>cps_products[]</c> 元素对象，<c>getwindowproduct</c>——两者结构一致 ⇒ 共用，
/// 仅自营商品携带 <c>page_path</c>/<c>banned_details</c>/<c>branch_info</c>/<c>limit_discount_info</c>）。
/// </summary>
/// <remarks>
/// 官方契约：自营商品橱窗 ID 即小店商品 ID；带货商品橱窗 ID 为货源商品在小店的商品 ID（另有一个平台分配的带货商品 ID）。
/// 官方注明 <c>leaf_category_id</c>（叶子类目 ID）将替代 <c>third_category_id</c>，后者最终会下掉，需尽快迁移。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowProduct
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>；自营为橱窗商品 ID，带货为带货商品 ID）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置外部商品 ID（官方 <c>out_product_id</c>；自营为商家侧外部商品 ID，带货为货源店铺的商品 ID）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品头图 url（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>获取或设置商品所属三级类目 ID（官方 <c>third_category_id</c>；官方将用 leaf_category_id 替代本字段）。</summary>
    [JsonPropertyName("third_category_id")]
    public string? ThirdCategoryId { get; set; }

    /// <summary>获取或设置商品所属叶子类目（品类）ID（官方 <c>leaf_category_id</c>）。</summary>
    [JsonPropertyName("leaf_category_id")]
    public string? LeafCategoryId { get; set; }

    /// <summary>获取或设置商品状态（官方 <c>status</c>，见 <see cref="ChannelsWindowProductStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置价格区间最大值/市场价/原价（官方 <c>market_price</c>，单位：分）。</summary>
    [JsonPropertyName("market_price")]
    public long? MarketPrice { get; set; }

    /// <summary>获取或设置价格区间最小值/销售价（官方 <c>selling_price</c>，单位：分）。</summary>
    [JsonPropertyName("selling_price")]
    public long? SellingPrice { get; set; }

    /// <summary>获取或设置剩余库存（官方 <c>stock</c>）。</summary>
    [JsonPropertyName("stock")]
    public int? Stock { get; set; }

    /// <summary>获取或设置销量（官方 <c>sales</c>）。</summary>
    [JsonPropertyName("sales")]
    public int? Sales { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，非带货商品才拥有）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置商品详情页路径（官方 <c>page_path</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("page_path")]
    public ChannelsWindowPagePath? PagePath { get; set; }

    /// <summary>获取或设置商品所属电商平台 ID（官方 <c>platform_id</c>）。</summary>
    [JsonPropertyName("platform_id")]
    public long? PlatformId { get; set; }

    /// <summary>获取或设置商品所属电商平台名（官方 <c>platform_name</c>）。</summary>
    [JsonPropertyName("platform_name")]
    public string? PlatformName { get; set; }

    /// <summary>获取或设置是否在个人橱窗页隐藏（官方 <c>is_hide_for_window</c>；官方参数表 boolean、返回示例为数字 0——矛盾照录，按参数表建模）。</summary>
    [JsonPropertyName("is_hide_for_window")]
    public bool? IsHideForWindow { get; set; }

    /// <summary>获取或设置商品是否处于禁止售卖的状态（官方 <c>banned</c>）。</summary>
    [JsonPropertyName("banned")]
    public bool? Banned { get; set; }

    /// <summary>获取或设置禁售明细（官方 <c>banned_details</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("banned_details")]
    public ChannelsWindowBannedDetails? BannedDetails { get; set; }

    /// <summary>获取或设置分店信息（官方 <c>branch_info</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("branch_info")]
    public ChannelsWindowBranchInfo? BranchInfo { get; set; }

    /// <summary>获取或设置抢购活动信息（官方 <c>limit_discount_info</c>，仅自营商品返回）。</summary>
    [JsonPropertyName("limit_discount_info")]
    public ChannelsWindowLimitDiscountInfo? LimitDiscountInfo { get; set; }

    /// <summary>获取或设置带货商品所属计划类型（官方 <c>cps_item_type</c>，带货商品返回，见 <see cref="ChannelsWindowCpsItemTypes"/>）。</summary>
    [JsonPropertyName("cps_item_type")]
    public int? CpsItemType { get; set; }

    /// <summary>获取或设置跟佣跳转链接（官方 <c>product_promotion_link</c>，用于在小程序跳转小店场景添加商品时传递跟佣信息）。</summary>
    [JsonPropertyName("product_promotion_link")]
    public string? ProductPromotionLink { get; set; }
}
