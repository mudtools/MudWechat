// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品详情（官方 <c>product</c> / <c>edit_product</c> 对象，<c>product/get</c> 返回形态）。
/// </summary>
/// <remarks>线上数据（product）与草稿数据（edit_product）字段结构一致；字段来源：官方「获取商品」返回参数表。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductDetail
{
    /// <summary>获取或设置小店内部商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置外部平台自定义商品 ID（官方 <c>out_product_id</c>；添加时没录入则回包可能不包含）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置副标题（官方 <c>sub_title</c>，已废弃）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，最多 9 张，每张不超过 2MB）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置商品详情（官方 <c>desc_info</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>）。</summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }

    /// <summary>获取或设置发货账号类型（官方 <c>deliver_acct_type</c>）。</summary>
    [JsonPropertyName("deliver_acct_type")]
    public List<int>? DeliverAcctType { get; set; }

    /// <summary>获取或设置运费信息（官方 <c>express_info</c>）。</summary>
    [JsonPropertyName("express_info")]
    public ChannelsProductExpressInfo? ExpressInfo { get; set; }

    /// <summary>获取或设置售后说明（官方 <c>aftersale_desc</c>）。</summary>
    [JsonPropertyName("aftersale_desc")]
    public string? AftersaleDesc { get; set; }

    /// <summary>获取或设置限购信息（官方 <c>limited_info</c>）。</summary>
    [JsonPropertyName("limited_info")]
    public ChannelsProductLimitedInfo? LimitedInfo { get; set; }

    /// <summary>获取或设置额外服务（官方 <c>extra_service</c>）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsProductExtraService? ExtraService { get; set; }

    /// <summary>获取或设置商品线上状态（官方 <c>status</c>，见 <see cref="ChannelsProductStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置商品草稿状态（官方 <c>edit_status</c>，以 edit_product 返回值为准）。</summary>
    [JsonPropertyName("edit_status")]
    public int? EditStatus { get; set; }

    /// <summary>获取或设置商品 SKU 最小价格（官方 <c>min_price</c>，单位：分）。</summary>
    [JsonPropertyName("min_price")]
    public long? MinPrice { get; set; }

    /// <summary>获取或设置商品类目（旧类目树，官方 <c>cats</c>）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsProductCategory>? Cats { get; set; }

    /// <summary>获取或设置商品类目（新类目树，官方 <c>cats_v2</c>）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置商品参数（官方 <c>attrs</c>）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置商家自定义的商品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置 sku 信息（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsProductSkuDetail>? Skus { get; set; }

    /// <summary>获取或设置商品类型（官方 <c>product_type</c>，见 <see cref="ChannelsProductTypes"/>）。</summary>
    [JsonPropertyName("product_type")]
    public int? ProductType { get; set; }

    /// <summary>获取或设置商品草稿最近一次修改时间（官方 <c>edit_time</c>，指定获取草稿数据时才返回）。</summary>
    [JsonPropertyName("edit_time")]
    public long? EditTime { get; set; }

    /// <summary>获取或设置商品售后信息（官方 <c>after_sale_info</c>）。</summary>
    [JsonPropertyName("after_sale_info")]
    public ChannelsProductAfterSaleInfo? AfterSaleInfo { get; set; }

    /// <summary>获取或设置来源商品 id（官方 <c>src_product_id</c>；商品类型为福袋抽奖且由橱窗自营商品导入时才有值）。</summary>
    [JsonPropertyName("src_product_id")]
    public long? SrcProductId { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置是否在店铺首页隐藏（官方 <c>hide_in_window</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("hide_in_window")]
    public int? HideInWindow { get; set; }

    /// <summary>获取或设置商品待开售信息（官方 <c>timing_onsale_info</c>）。</summary>
    [JsonPropertyName("timing_onsale_info")]
    public ChannelsTimingOnsaleInfo? TimingOnsaleInfo { get; set; }

    /// <summary>获取或设置短标题（官方 <c>short_title</c>）。</summary>
    [JsonPropertyName("short_title")]
    public string? ShortTitle { get; set; }

    /// <summary>获取或设置销量（官方 <c>total_sold_num</c>）。</summary>
    [JsonPropertyName("total_sold_num")]
    public long? TotalSoldNum { get; set; }

    /// <summary>获取或设置发布模式（官方 <c>release_mode</c>：0 普通模式，1 极简模式）。</summary>
    [JsonPropertyName("release_mode")]
    public int? ReleaseMode { get; set; }

    /// <summary>获取或设置商品头图视频（官方 <c>head_videos</c>；没有上传则不返回）。</summary>
    [JsonPropertyName("head_videos")]
    public ChannelsProductHeadVideo? HeadVideos { get; set; }

    /// <summary>获取或设置 spu 维度 sku 预售配置（官方 <c>spu_deliver_info</c>）。</summary>
    [JsonPropertyName("spu_deliver_info")]
    public ChannelsSpuDeliverInfo? SpuDeliverInfo { get; set; }

    /// <summary>获取或设置商品子状态（官方 <c>sub_status</c>）。</summary>
    [JsonPropertyName("sub_status")]
    public int? SubStatus { get; set; }

    /// <summary>获取或设置 3:4 主图列表（官方 <c>vertical_head_imgs</c>，最多 1 张）。</summary>
    [JsonPropertyName("vertical_head_imgs")]
    public List<string>? VerticalHeadImgs { get; set; }

    /// <summary>获取或设置白底图列表（官方 <c>white_bg_imgs</c>，最多 1 张）。</summary>
    [JsonPropertyName("white_bg_imgs")]
    public List<string>? WhiteBgImgs { get; set; }
}
