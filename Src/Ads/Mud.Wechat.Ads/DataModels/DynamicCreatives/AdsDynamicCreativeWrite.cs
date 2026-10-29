// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.DynamicCreatives;

/// <summary>
/// <c>POST /v3.0/dynamic_creatives/add</c> 的请求体（2026-10-11 L3 核验，顶层 16 键）。
/// </summary>
/// <remarks>
/// <para>
/// 必填：<c>account_id</c> / <c>adgroup_id</c> / <c>dynamic_creative_name</c> / <c>creative_components</c>
/// （官方 `*` 标记）；其余可填。属性全部可空 + <c>WhenWritingNull</c> ⇒ 缺省字段不上送。
/// 组件 <c>value</c> 的构造方式见 <see cref="AdsCreativeComponentItem"/>。
/// </para>
/// <para>
/// <c>program_creative_info</c> 内 <c>material_derive_info[].original_adcreative_template_id_list</c>
/// 官方带 <c>*</c> 必填（仅当走程序化创意分支时）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填；不支持代理商 id）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>所属营销单元 id（官方 <c>adgroup_id</c>，必填，<c>int64</c>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>创意名称（官方 <c>dynamic_creative_name</c>，必填）。</summary>
    [JsonPropertyName("dynamic_creative_name")]
    public string? DynamicCreativeName { get; set; }

    /// <summary>创意形式 id（官方 <c>creative_template_id</c>）。</summary>
    [JsonPropertyName("creative_template_id")]
    public long? CreativeTemplateId { get; set; }

    /// <summary>投放模式（官方 <c>delivery_mode</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("delivery_mode")]
    public string? DeliveryMode { get; set; }

    /// <summary>创意类型（官方 <c>dynamic_creative_type</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("dynamic_creative_type")]
    public string? DynamicCreativeType { get; set; }

    /// <summary>创意组件组（官方 <c>creative_components</c>，必填；44 组件数组键见 <see cref="AdsCreativeComponents"/>）。</summary>
    [JsonPropertyName("creative_components")]
    public AdsCreativeComponents? CreativeComponents { get; set; }

    /// <summary>曝光监测链接（官方 <c>impression_tracking_url</c>）。</summary>
    [JsonPropertyName("impression_tracking_url")]
    public string? ImpressionTrackingUrl { get; set; }

    /// <summary>点击监测链接（官方 <c>click_tracking_url</c>）。</summary>
    [JsonPropertyName("click_tracking_url")]
    public string? ClickTrackingUrl { get; set; }

    /// <summary>程序化创意信息（官方 <c>program_creative_info</c>）。</summary>
    [JsonPropertyName("program_creative_info")]
    public AdsProgramCreativeInfo? ProgramCreativeInfo { get; set; }

    /// <summary>落地页监测链接（官方 <c>page_track_url</c>）。</summary>
    [JsonPropertyName("page_track_url")]
    public string? PageTrackUrl { get; set; }

    /// <summary>是否自动衍生程序化创意（官方 <c>auto_derived_program_creative_switch</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_derived_program_creative_switch")]
    public bool? AutoDerivedProgramCreativeSwitch { get; set; }

    /// <summary>启用状态（官方 <c>configured_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }

    /// <summary>版位校验模型（官方 <c>site_set_validate_model</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("site_set_validate_model")]
    public string? SiteSetValidateModel { get; set; }
}

/// <summary>
/// <c>POST /v3.0/dynamic_creatives/update</c> 的请求体（2026-10-11 L3 核验，顶层 10 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>dynamic_creative_id</c>。<b>补丁语义</b>：与 <c>add</c> 不同构 ——
/// 无 <c>adgroup_id</c> / <c>creative_template_id</c> / <c>delivery_mode</c> / <c>dynamic_creative_type</c> /
/// <c>program_creative_info</c> / <c>page_track_url</c>（创建期事实字段不可更新），
/// 多 <c>is_retry_batch_update</c>；<c>creative_components</c> 不带必填星号（增量更新）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>创意 id（官方 <c>dynamic_creative_id</c>，必填）。</summary>
    [JsonPropertyName("dynamic_creative_id")]
    public long? DynamicCreativeId { get; set; }

    /// <summary>创意名称（官方 <c>dynamic_creative_name</c>）。</summary>
    [JsonPropertyName("dynamic_creative_name")]
    public string? DynamicCreativeName { get; set; }

    /// <summary>创意组件组（官方 <c>creative_components</c>；增量更新语义）。</summary>
    [JsonPropertyName("creative_components")]
    public AdsCreativeComponents? CreativeComponents { get; set; }

    /// <summary>曝光监测链接（官方 <c>impression_tracking_url</c>）。</summary>
    [JsonPropertyName("impression_tracking_url")]
    public string? ImpressionTrackingUrl { get; set; }

    /// <summary>点击监测链接（官方 <c>click_tracking_url</c>）。</summary>
    [JsonPropertyName("click_tracking_url")]
    public string? ClickTrackingUrl { get; set; }

    /// <summary>是否自动衍生程序化创意（官方 <c>auto_derived_program_creative_switch</c>）。</summary>
    [JsonPropertyName("auto_derived_program_creative_switch")]
    public bool? AutoDerivedProgramCreativeSwitch { get; set; }

    /// <summary>启用状态（官方 <c>configured_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }

    /// <summary>是否重试批量更新（官方 <c>is_retry_batch_update</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_retry_batch_update")]
    public bool? IsRetryBatchUpdate { get; set; }

    /// <summary>版位校验模型（官方 <c>site_set_validate_model</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("site_set_validate_model")]
    public string? SiteSetValidateModel { get; set; }
}

/// <summary><c>POST /v3.0/dynamic_creatives/delete</c> 的请求体（2 键，均必填）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeDeleteRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>创意 id（官方 <c>dynamic_creative_id</c>，必填）。</summary>
    [JsonPropertyName("dynamic_creative_id")]
    public long? DynamicCreativeId { get; set; }
}

/// <summary><c>dynamic_creatives/add|update|delete</c> 共用的 <c>data</c> 载荷（三支应答同构，均只有创意 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeIdData
{
    /// <summary>创意 id（官方 <c>dynamic_creative_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("dynamic_creative_id")]
    public long? DynamicCreativeId { get; set; }
}

/// <summary><c>POST /v3.0/dynamic_creatives/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeAddResponse : AdsResponse<AdsDynamicCreativeIdData>
{
}

/// <summary><c>POST /v3.0/dynamic_creatives/update</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeUpdateResponse : AdsResponse<AdsDynamicCreativeIdData>
{
}

/// <summary><c>POST /v3.0/dynamic_creatives/delete</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeDeleteResponse : AdsResponse<AdsDynamicCreativeIdData>
{
}
