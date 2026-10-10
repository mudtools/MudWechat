// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相关法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 用户行为数据源（官方 <c>user_action_sets[]</c> 元素，<c>struct</c>），用于转化归因。
/// </summary>
/// <remarks>
/// <b>官方原文的配对约束</b>：<c>id</c> 是「数据源 id，通过 <c>user_action_sets</c> 模块创建和获取，
/// 数据源 id 对应的类型要与 <c>user_action_sets</c> 中指定的 <c>type</c> 一致」⇒
/// <c>type</c> 与 <c>id</c> 是<b>成对</b>必填（官方两字段均标 <c>*</c>），<c>data_source_id</c> 选填。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsUserActionSet
{
    /// <summary>数据源类型（官方 <c>type</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>数据源 id（官方 <c>id</c>，<c>integer</c>，<b>必填</b>，须与 <see cref="Type"/> 对应的类型一致）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>DN 数据源 id（官方 <c>data_source_id</c>，<c>integer</c>，与知数数据源 id 一一对应）。</summary>
    [JsonPropertyName("data_source_id")]
    public long? DataSourceId { get; set; }
}

/// <summary>
/// oCPA 深度优化内容（官方 <c>deep_conversion_spec</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方原文「若此字段不传，或传空则视为无限制条件，ADX 程序化投放不可填写提交」。
/// </para>
/// <para>
/// <b>四个互斥子结构</b>：官方把「行为出价」「ROI 期望值」「强化 ROI 期望值」「深度辅助出价」配成四个
/// 平级 <c>struct</c>，每个都是 <c>{goal, 一个数值字段}</c>，数值字段类型与必填性<b>各不相同</b>
/// （<c>behavior_spec.bid_amount</c> 必填 <c>integer</c>；<c>worth_spec.expected_roi</c> 选填 <c>float</c>；
/// <c>worth_advanced_spec.expected_roi</c> 必填 <c>float</c>；<c>behavior_advanced_spec.bid_amount</c>
/// 必填 <c>integer</c>）⇒ 四个独立类型而非共用一个，共用来不及表达这些差异。
/// 同时官方另给平级简写入口（<c>deep_conversion_behavior_bid</c> / <c>deep_conversion_worth_rate</c> 等，
/// 见 <c>adgroups/add</c> 请求表），两套入口的取舍由调用方按是否已传 <c>conversion_id</c> 决定，SDK 不预判。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDeepConversionSpec
{
    /// <summary>oCPA 深度优化价值配置（官方 <c>deep_conversion_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("deep_conversion_type")]
    public string? DeepConversionType { get; set; }

    /// <summary>oCPA 优化转化行为配置（官方 <c>deep_conversion_behavior_spec</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("deep_conversion_behavior_spec")]
    public AdsDeepConversionBehaviorSpec? BehaviorSpec { get; set; }

    /// <summary>oCPA 优化 ROI 配置（官方 <c>deep_conversion_worth_spec</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("deep_conversion_worth_spec")]
    public AdsDeepConversionWorthSpec? WorthSpec { get; set; }

    /// <summary>oCPC/oCPM 优化 ROI 配置（官方 <c>deep_conversion_worth_advanced_spec</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("deep_conversion_worth_advanced_spec")]
    public AdsDeepConversionWorthAdvancedSpec? WorthAdvancedSpec { get; set; }

    /// <summary>oCPX 深度辅助配置（官方 <c>deep_conversion_behavior_advanced_spec</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("deep_conversion_behavior_advanced_spec")]
    public AdsDeepConversionBehaviorAdvancedSpec? BehaviorAdvancedSpec { get; set; }
}

/// <summary>oCPA 优化转化行为配置（官方 <c>deep_conversion_behavior_spec</c>，<c>struct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDeepConversionBehaviorSpec
{
    /// <summary>优化转化行为目标（官方 <c>goal</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("goal")]
    public string? Goal { get; set; }

    /// <summary>深度优化行为的出价，单位为分（官方 <c>bid_amount</c>，<c>integer</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }
}

/// <summary>oCPA 优化 ROI 配置（官方 <c>deep_conversion_worth_spec</c>，<c>struct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDeepConversionWorthSpec
{
    /// <summary>优化 ROI 目标（官方 <c>goal</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("goal")]
    public string? Goal { get; set; }

    /// <summary>深度优化价值效果值（官方 <c>expected_roi</c>，<c>float</c>，<b>选填</b>）。</summary>
    [JsonPropertyName("expected_roi")]
    public double? ExpectedRoi { get; set; }
}

/// <summary>oCPC/oCPM 优化 ROI 配置（官方 <c>deep_conversion_worth_advanced_spec</c>，<c>struct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDeepConversionWorthAdvancedSpec
{
    /// <summary>优化 ROI 目标（官方 <c>goal</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("goal")]
    public string? Goal { get; set; }

    /// <summary>深度优化价值效果值（官方 <c>expected_roi</c>，<c>float</c>，<b>必填</b> —— 与
    /// <see cref="AdsDeepConversionWorthSpec.ExpectedRoi"/> 的选填形态不同）。</summary>
    [JsonPropertyName("expected_roi")]
    public double? ExpectedRoi { get; set; }
}

/// <summary>oCPX 深度辅助配置（官方 <c>deep_conversion_behavior_advanced_spec</c>，<c>struct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDeepConversionBehaviorAdvancedSpec
{
    /// <summary>深度辅助优化 OG 目标（官方 <c>goal</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("goal")]
    public string? Goal { get; set; }

    /// <summary>深度辅助优化 OG 出价，单位为分（官方 <c>bid_amount</c>，<c>integer</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }
}

/// <summary>
/// 创意增强 MAX 偏好设置（官方 <c>auto_derived_creative_preference</c>，<c>struct</c>）。
/// 官方原文「非必选项，如不设置偏好，则将根据默认偏好全选方式生效」⇒ 不传与传空语义不同（前者=全选默认），
/// SDK 不做「空即全选」的本地补写。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAutoDerivedCreativePreference
{
    /// <summary>创意增强 MAX 偏好设置列表（官方 <c>auto_derived_creative_method_type_list</c>，<c>enum[]</c>）。</summary>
    [JsonPropertyName("auto_derived_creative_method_type_list")]
    public List<string>? MethodTypeList { get; set; }

    /// <summary>创意衍生选择商品列表（官方 <c>auto_derived_creative_select_product_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("auto_derived_creative_select_product_list")]
    public List<AdsAutoDerivedCreativeProduct>? SelectProductList { get; set; }
}

/// <summary>创意衍生选择的单个商品（官方 <c>auto_derived_creative_select_product_list[]</c> 元素，<c>struct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAutoDerivedCreativeProduct
{
    /// <summary>商品 id（官方 <c>product_outer_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("product_outer_id")]
    public string? ProductOuterId { get; set; }

    /// <summary>商品库 id（官方 <c>catalog_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("catalog_id")]
    public long? CatalogId { get; set; }
}
