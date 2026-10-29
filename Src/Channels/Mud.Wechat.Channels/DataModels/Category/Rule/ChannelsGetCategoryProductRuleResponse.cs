// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>获取类目下商品发布规则（<c>getcategoryproductrule</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryProductRuleResponse : ChannelsResponse
{
    /// <summary>获取或设置类目信息（官方 <c>info</c>：cat_id / name；与「获取类目信息」<c>info</c> 字段集一致 ⇒ 共用 <see cref="ChannelsCategoryInfo"/>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsCategoryInfo? Info { get; set; }

    /// <summary>获取或设置产品属性（官方 <c>product_attr_list</c>）。</summary>
    [JsonPropertyName("product_attr_list")]
    public List<ChannelsCategoryPublishAttrItem>? ProductAttrList { get; set; }

    /// <summary>获取或设置销售属性（官方 <c>sale_attr_list</c>）。</summary>
    [JsonPropertyName("sale_attr_list")]
    public List<ChannelsCategoryPublishAttrItem>? SaleAttrList { get; set; }

    /// <summary>获取或设置资质信息（官方 <c>product_qua_list</c>）。</summary>
    [JsonPropertyName("product_qua_list")]
    public List<ChannelsCategoryPublishQua>? ProductQuaList { get; set; }

    /// <summary>获取或设置价格下限（官方 <c>floor_price</c>，单位：分）。</summary>
    [JsonPropertyName("floor_price")]
    public long? FloorPrice { get; set; }

    /// <summary>获取或设置尺码表（官方 <c>size_chart</c>；与「获取类目信息」字段集一致 ⇒ 共用 <see cref="ChannelsCategorySizeChart"/>）。</summary>
    [JsonPropertyName("size_chart")]
    public ChannelsCategorySizeChart? SizeChart { get; set; }

    /// <summary>获取或设置商品编辑要求（官方 <c>product_requirement</c>；与「获取类目信息」字段集一致 ⇒ 共用 <see cref="ChannelsCategoryProductRequirement"/>）。</summary>
    [JsonPropertyName("product_requirement")]
    public ChannelsCategoryProductRequirement? ProductRequirement { get; set; }

    /// <summary>获取或设置额外服务列表（官方 <c>extra_service_list</c>，对应添加商品 extra_service 参数）。</summary>
    [JsonPropertyName("extra_service_list")]
    public List<ChannelsCategoryExtraService>? ExtraServiceList { get; set; }

    /// <summary>获取或设置是否必填 sku 条形码（官方 <c>is_need_bar_code</c>）。</summary>
    [JsonPropertyName("is_need_bar_code")]
    public bool? IsNeedBarCode { get; set; }

    /// <summary>获取或设置预售规则（官方 <c>presale_rule</c>）。</summary>
    [JsonPropertyName("presale_rule")]
    public ChannelsCategoryPresaleRule? PresaleRule { get; set; }

    /// <summary>获取或设置品牌信息（官方 <c>brand_info</c>，传入 brand_id 时返回）。</summary>
    [JsonPropertyName("brand_info")]
    public ChannelsCategoryBrandInfo? BrandInfo { get; set; }

    /// <summary>获取或设置当前类目是否为定向类目（官方 <c>is_directed_category</c>）。</summary>
    [JsonPropertyName("is_directed_category")]
    public bool? IsDirectedCategory { get; set; }

    /// <summary>获取或设置类目可售品牌范围（官方 <c>restricted_brand_list</c>）。</summary>
    [JsonPropertyName("restricted_brand_list")]
    public List<ChannelsCategoryRestrictedBrand>? RestrictedBrandList { get; set; }
}
