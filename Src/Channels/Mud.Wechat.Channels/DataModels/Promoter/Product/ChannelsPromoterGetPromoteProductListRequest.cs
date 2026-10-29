// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取可推广的商品 id 列表请求（官方 <c>promoter/get_promote_product_list</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetPromoteProductListRequest
{
    /// <summary>获取或设置查询某个店铺下的推广商品（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置商品的计划类型（官方 <c>plan_type</c>，必填，1：定向计划 2：公开计划 3：机构定向计划 4：机构普通计划，见 <see cref="ChannelsPromoterPlanTypes"/>）。</summary>
    [JsonPropertyName("plan_type")]
    public int PlanType { get; set; }

    /// <summary>获取或设置商品查询条件（官方 <c>spu_item_condition</c>）。</summary>
    [JsonPropertyName("spu_item_condition")]
    public ChannelsPromoterSpuItemCondition? SpuItemCondition { get; set; }

    /// <summary>获取或设置商品类目查询条件（官方 <c>category</c>，定向计划不支持）。</summary>
    [JsonPropertyName("category")]
    public ChannelsPromoterProductCategoryCondition? Category { get; set; }

    /// <summary>获取或设置搜索关键词（官方 <c>keyword</c>）。</summary>
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    /// <summary>获取或设置每页商品数（官方 <c>page_size</c>，必填，最大 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，必填，第一页为空，后续传上一页返回的值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置来源机构筛选（官方 <c>from_head_supplier_appid</c>，当商品是机构公开或定向时有效）。</summary>
    [JsonPropertyName("from_head_supplier_appid")]
    public string? FromHeadSupplierAppId { get; set; }
}
