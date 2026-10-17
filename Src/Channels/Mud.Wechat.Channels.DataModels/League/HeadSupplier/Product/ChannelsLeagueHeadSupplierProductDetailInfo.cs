// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 机构侧商品信息（官方 <c>headsupplier/productdetail/get</c> 返回的 <c>item.product_info</c> 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 与小店商家侧 <c>product/get</c> 的 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsProductDetail"/> <b>字段集不同</b>：
/// 机构侧无 <c>min_price</c> / <c>express_info</c> / <c>cats</c>（旧类目树）/ <c>aftersale_desc</c> 等，
/// 多 <c>freight_info</c>（完整物流模板回包）、<c>monthly_sales_range</c>、<c>product_spring_festival_delivery_config</c>、
/// <c>good_evaluation_ratio</c>、<c>is_brand_good</c> ⇒ 分建 DTO，仅字段集完全同形的子对象（<c>desc_info</c>）复用商家侧类型。
/// </para>
/// <para>
/// 官方同页「代码示例」的 <c>product_info</c> 内出现 <c>express_info</c> / <c>statistical_data</c> 两个对象，
/// 但返回参数表内无此二字段 ⇒ 照参数表建模，不回填示例独有字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProductDetailInfo
{
    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置副标题（官方 <c>sub_title</c>，添加时未录入则回包可能不含该字段）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，最多 9 张，每张不超过 2MB）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置商详信息（官方 <c>desc_info</c>，与商家侧同形 ⇒ 复用 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsProductDescInfo"/>）。</summary>
    [JsonPropertyName("desc_info")]
    public Mud.Wechat.Channels.DataModels.Product.ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置 sku 信息（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLeagueHeadSupplierProductDetailSku>? Skus { get; set; }

    /// <summary>获取或设置新类目树类目信息（官方 <c>cats_v2</c>，机构侧元素含 <c>cat_name</c>，与商家侧仅 <c>cat_id</c> 不同 ⇒ 分建）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsLeagueHeadSupplierProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置商品状态（官方 <c>status</c>：<c>5</c> 上架销售中，其他值为非正常状态）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置近 30 天销量范围（官方 <c>monthly_sales_range</c>，档位取值见 <see cref="ChannelsLeagueHeadSupplierMonthlySalesRanges"/>）。</summary>
    [JsonPropertyName("monthly_sales_range")]
    public int? MonthlySalesRange { get; set; }

    /// <summary>获取或设置服务保障信息（官方 <c>extra_service</c>，机构侧 8 个开关，比商家侧多 3 个 ⇒ 分建）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsLeagueHeadSupplierProductDetailExtraService? ExtraService { get; set; }

    /// <summary>获取或设置春节不打烊信息（官方 <c>product_spring_festival_delivery_config</c>）。</summary>
    [JsonPropertyName("product_spring_festival_delivery_config")]
    public ChannelsLeagueHeadSupplierSpringFestivalDeliveryConfig? ProductSpringFestivalDeliveryConfig { get; set; }

    /// <summary>获取或设置物流信息（官方 <c>freight_info</c>，2026-03-09 新增）。</summary>
    [JsonPropertyName("freight_info")]
    public ChannelsLeagueHeadSupplierFreightInfo? FreightInfo { get; set; }

    /// <summary>获取或设置是否有单品资质（官方 <c>has_qualification</c>，boolean）。</summary>
    [JsonPropertyName("has_qualification")]
    public bool? HasQualification { get; set; }

    /// <summary>获取或设置是否热门品牌（官方 <c>is_hot_brand</c>，boolean；<c>2026-05-09</c> 官方标注废弃，替代字段见 <c>is_brand_good</c>）。</summary>
    [JsonPropertyName("is_hot_brand")]
    public bool? IsHotBrand { get; set; }

    /// <summary>获取或设置商品短标题（官方 <c>short_title</c>）。</summary>
    [JsonPropertyName("short_title")]
    public string? ShortTitle { get; set; }

    /// <summary>获取或设置商品视频列表（官方 <c>head_video_info</c>，注意官方为单数 <c>info</c> 的数组，与商家侧 <c>head_videos</c> 字段名不同）。</summary>
    [JsonPropertyName("head_video_info")]
    public List<ChannelsLeagueHeadSupplierHeadVideoInfo>? HeadVideoInfo { get; set; }

    /// <summary>获取或设置好评率（官方 <c>good_evaluation_ratio</c>，范围 0~100000）。</summary>
    [JsonPropertyName("good_evaluation_ratio")]
    public int? GoodEvaluationRatio { get; set; }

    /// <summary>获取或设置是否品牌好物（官方 <c>is_brand_good</c>，boolean；<c>2026-05-09</c> 新增，替代 <c>is_hot_brand</c>）。</summary>
    [JsonPropertyName("is_brand_good")]
    public bool? IsBrandGood { get; set; }
}
