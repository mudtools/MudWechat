// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 选品广场商品列表查询请求（机构侧官方 <c>headsupplier/selectionproducts/list/get</c> 请求体）。
/// </summary>
/// <remarks>
/// <para>
/// ①官方 <c>2026-03-09</c> 变更日志提到「新增 <c>from_head_supplier_appid</c> 字段（来源机构筛选）」，但同日核验证的
/// <b>请求参数表内并无该字段</b> ⇒ 本 DTO 照参数表建模、不回填日志提及字段（以参数表为权威）。
/// </para>
/// <para>②官方语义：<c>plan_type=1</c> 定向计划（商家/供货机构定向推给本机构的商品）、<c>=2</c> 公开计划（对所有机构可见）；
/// <c>category</c> 筛选<b>定向计划不支持</b>；<c>create_time_range</c> 与 <c>keyword</c> <b>不能共用</b>。</para>
/// <para>③本接口不支持云调用、不支持第三方平台调用，仅「联盟带货机构」账号类型可调。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetSelectionProductsRequest
{
    /// <summary>获取或设置计划类型（官方 <c>plan_type</c>，必填，见 <see cref="ChannelsLeagueHeadSupplierSelectionPlanTypes"/>：1 定向计划 / 2 公开计划）。</summary>
    [JsonPropertyName("plan_type")]
    public int PlanType { get; set; }

    /// <summary>获取或设置商品属性筛选条件（官方 <c>spu_item_condition</c>）。</summary>
    [JsonPropertyName("spu_item_condition")]
    public ChannelsLeagueHeadSupplierSelectionProductCondition? SpuItemCondition { get; set; }

    /// <summary>获取或设置类目筛选条件（官方 <c>category</c>，<b>定向计划不支持</b>）。</summary>
    [JsonPropertyName("category")]
    public ChannelsLeagueHeadSupplierSelectionCategoryCondition? Category { get; set; }

    /// <summary>获取或设置搜索关键词（官方 <c>keyword</c>；与 <c>spu_item_condition.create_time_range</c> 不能共用）。</summary>
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    /// <summary>获取或设置商品来源（官方 <c>spu_source</c>，见 <see cref="ChannelsLeagueHeadSupplierSpuSources"/>：1 商家供货 / 2 机构供货；官方默认 <c>1</c>）。</summary>
    [JsonPropertyName("spu_source")]
    public int? SpuSource { get; set; }

    /// <summary>获取或设置佣金类型（官方 <c>commission_type</c>，见 <see cref="ChannelsLeagueHeadSupplierSelectionCommissionTypes"/>：1 商家指定达人佣金 / 2 机构指定达人佣金；<c>2026-03-09</c> 新增字段）。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }

    /// <summary>获取或设置单页商品数（官方 <c>page_size</c>，最大 <c>20</c>）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>获取或设置翻页上下文，写入上一页返回的 <c>next_key</c>（官方 <c>next_key</c>，首页不填）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
