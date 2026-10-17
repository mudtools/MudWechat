// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Qualifications;

/// <summary>
/// 资质扩展字段（官方 <c>expand_field_list[]</c> 元素，add/update/get 三页同构，2026-10-17 逐表核验）。
/// </summary>
/// <remarks>
/// 键与约束不是 SDK 常量：<c>field_name</c> / <c>value</c> 的合法组合由「获取客户资质结构接口」
/// （<c>qualification_structure/get</c>，本 SDK 未消费）动态返回 ⇒ 本地不可枚举、不拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationExpandField
{
    /// <summary>扩展字段名（官方 <c>field_name</c>，必填；取资质结构接口返回的 <c>field_name</c>）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>扩展字段值（官方 <c>value</c>，必填；长度等约束见资质结构接口返回的字段定义）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>
/// 行业资质 / 附加行业资质条目（官方 <c>industry_qualifications[]</c> 与
/// <c>additional_industry_qualifications[]</c> 元素 —— 两支报文结构逐字段同构 ⇒ 共用本类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsIndustryQualificationInfo
{
    /// <summary>资质 id（官方 <c>qualification_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("qualification_id")]
    public long? QualificationId { get; set; }

    /// <summary>行业 id（官方 <c>system_industry_id</c>，<c>integer</c>；须填<b>二级行业</b> id）。</summary>
    [JsonPropertyName("system_industry_id")]
    public long? SystemIndustryId { get; set; }

    /// <summary>业务范围 id（官方 <c>business_scope_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("business_scope_id")]
    public long? BusinessScopeId { get; set; }

    /// <summary>资质编码（官方 <c>qualification_code</c>；4 字节，取值见官方「资质编码列表」）。</summary>
    [JsonPropertyName("qualification_code")]
    public string? QualificationCode { get; set; }

    /// <summary>资质图片 id 列表（官方 <c>image_id_list</c>，<c>string[]</c>；经资质模块上传获得）。</summary>
    [JsonPropertyName("image_id_list")]
    public List<string>? ImageIdList { get; set; }

    /// <summary>资质扩展字段（官方 <c>expand_field_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("expand_field_list")]
    public List<AdsQualificationExpandField>? ExpandFieldList { get; set; }

    /// <summary>资质状态（官方 <c>qualification_status</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("qualification_status")]
    public string? QualificationStatus { get; set; }

    /// <summary>资质有效日期（官方 <c>expired_date</c>；<c>YYYY-MM-DD</c>，<b>空代表永久有效</b>）。</summary>
    [JsonPropertyName("expired_date")]
    public string? ExpiredDate { get; set; }

    /// <summary>审核消息（官方 <c>reject_message</c>）。</summary>
    [JsonPropertyName("reject_message")]
    public string? RejectMessage { get; set; }

    /// <summary>创建时间（官方 <c>created_time</c>，<c>integer</c>，时间戳）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间（官方 <c>last_modified_time</c>，<c>integer</c>，时间戳）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }
}

/// <summary>广告资质条目（官方 <c>ad_qualifications[]</c> 元素；比行业条目少行业/业务范围两键、
/// 多 <c>is_mdm_shared</c> ⇒ 不同构，按 D5 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsAdQualificationInfo
{
    /// <summary>资质 id（官方 <c>qualification_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("qualification_id")]
    public long? QualificationId { get; set; }

    /// <summary>资质编码（官方 <c>qualification_code</c>；4 字节，取值见官方「资质编码列表」）。</summary>
    [JsonPropertyName("qualification_code")]
    public string? QualificationCode { get; set; }

    /// <summary>资质图片 id 列表（官方 <c>image_id_list</c>，<c>string[]</c>；经资质模块上传获得）。</summary>
    [JsonPropertyName("image_id_list")]
    public List<string>? ImageIdList { get; set; }

    /// <summary>资质扩展字段（官方 <c>expand_field_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("expand_field_list")]
    public List<AdsQualificationExpandField>? ExpandFieldList { get; set; }

    /// <summary>资质状态（官方 <c>qualification_status</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("qualification_status")]
    public string? QualificationStatus { get; set; }

    /// <summary>资质有效日期（官方 <c>expired_date</c>；<c>YYYY-MM-DD</c>，<b>空代表永久有效</b>）。</summary>
    [JsonPropertyName("expired_date")]
    public string? ExpiredDate { get; set; }

    /// <summary>资质共享状态（官方 <c>is_mdm_shared</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("is_mdm_shared")]
    public string? IsMdmShared { get; set; }

    /// <summary>审核消息（官方 <c>reject_message</c>）。</summary>
    [JsonPropertyName("reject_message")]
    public string? RejectMessage { get; set; }

    /// <summary>创建时间（官方 <c>created_time</c>，<c>integer</c>，时间戳）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间（官方 <c>last_modified_time</c>，<c>integer</c>，时间戳）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }
}

/// <summary>
/// <c>qualifications/get</c> 的 <c>data</c> 载荷（三支列表按 <c>qualification_type</c> 出现，
/// <b>无分页元信息</b> —— 2026-10-17 逐表核验）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationListData
{
    /// <summary>行业资质列表（官方 <c>industry_qualifications</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("industry_qualifications")]
    public List<AdsIndustryQualificationInfo>? IndustryQualifications { get; set; }

    /// <summary>广告资质列表（官方 <c>ad_qualifications</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("ad_qualifications")]
    public List<AdsAdQualificationInfo>? AdQualifications { get; set; }

    /// <summary>附加行业资质列表（官方 <c>additional_industry_qualifications</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("additional_industry_qualifications")]
    public List<AdsIndustryQualificationInfo>? AdditionalIndustryQualifications { get; set; }
}

/// <summary><c>GET /v3.0/qualifications/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationsGetResponse : AdsResponse<AdsQualificationListData>
{
}
