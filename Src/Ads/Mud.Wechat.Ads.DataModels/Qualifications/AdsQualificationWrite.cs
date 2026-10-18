// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Qualifications;

/// <summary>
/// <c>POST /v3.0/qualifications/add</c> 的请求体（2026-10-17 官方页逐表核验，3 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>qualification_type</c> / <c>qualification_spec</c>。
/// <c>qualification_type</c> 决定 <see cref="AdsQualificationSpec"/> 三支互斥 spec 中哪一支生效
/// （官方原文「当且仅当 … 时可填且必填」⇒ 本地不拦截，错配由应答 <c>code</c> 表达）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填，<c>integer</c>；<b>不支持代理商 id</b>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>资质类型（官方 <c>qualification_type</c>，必填，<c>enum</c>；可选值
    /// <c>{ INDUSTRY_QUALIFICATION, AD_QUALIFICATION, ADDITIONAL_INDUSTRY_QUALIFICATION }</c>，
    /// add/update/get 三页同一集合）。</summary>
    [JsonPropertyName("qualification_type")]
    public string? QualificationType { get; set; }

    /// <summary>资质信息（官方 <c>qualification_spec</c>，必填；三支互斥 spec 见
    /// <see cref="AdsQualificationSpec"/>）。</summary>
    [JsonPropertyName("qualification_spec")]
    public AdsQualificationSpec? QualificationSpec { get; set; }
}

/// <summary>资质规格容器（官方 <c>qualification_spec</c>，三支互斥 spec，按类型取一支）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationSpec
{
    /// <summary>行业资质（官方 <c>industry_spec</c>；当且仅当 <c>qualification_type</c> 取
    /// <c>INDUSTRY_QUALIFICATION</c> 时可填且必填）。</summary>
    [JsonPropertyName("industry_spec")]
    public AdsQualificationIndustrySpec? IndustrySpec { get; set; }

    /// <summary>广告资质（官方 <c>ad_spec</c>；当且仅当 <c>qualification_type</c> 取
    /// <c>AD_QUALIFICATION</c> 时可填且必填）。</summary>
    [JsonPropertyName("ad_spec")]
    public AdsQualificationAdSpec? AdSpec { get; set; }

    /// <summary>附加行业资质（官方 <c>additional_industry_spec</c>；当且仅当 <c>qualification_type</c> 取
    /// <c>ADDITIONAL_INDUSTRY_QUALIFICATION</c> 时可填且必填；与 <see cref="IndustrySpec"/> 同构 ⇒ 共用类型）。</summary>
    [JsonPropertyName("additional_industry_spec")]
    public AdsQualificationIndustrySpec? AdditionalIndustrySpec { get; set; }
}

/// <summary>行业 / 附加行业资质 spec（官方 <c>industry_spec</c> 与 <c>additional_industry_spec</c>，同构共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationIndustrySpec
{
    /// <summary>行业 id（官方 <c>system_industry_id</c>，必填，<c>integer</c>；须填<b>二级行业</b> id，
    /// 取值见官方「行业分类」）。</summary>
    [JsonPropertyName("system_industry_id")]
    public long? SystemIndustryId { get; set; }

    /// <summary>业务范围 id（官方 <c>business_scope_id</c>，<c>integer</c>，选填）。</summary>
    [JsonPropertyName("business_scope_id")]
    public long? BusinessScopeId { get; set; }

    /// <summary>资质编码（官方 <c>qualification_code</c>，必填；<b>固定 4 字节</b>，
    /// 取值见官方「资质编码列表」）。</summary>
    [JsonPropertyName("qualification_code")]
    public string? QualificationCode { get; set; }

    /// <summary>资质图片 id 列表（官方 <c>image_id_list</c>，必填，<c>string[]</c>；数组 1–10 项、
    /// 每项 1–64 字节；图片 id 经官方「资质模块」上传获得）。</summary>
    [JsonPropertyName("image_id_list")]
    public List<string>? ImageIdList { get; set; }

    /// <summary>资质扩展字段（官方 <c>expand_field_list</c>，<c>struct[]</c>，选填；
    /// 键与约束由「获取客户资质结构接口」动态返回）。</summary>
    [JsonPropertyName("expand_field_list")]
    public List<AdsQualificationExpandField>? ExpandFieldList { get; set; }
}

/// <summary>广告资质 spec（官方 <c>ad_spec</c>；比行业 spec 少行业/业务范围两键 ⇒ 不同构，分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationAdSpec
{
    /// <summary>资质编码（官方 <c>qualification_code</c>，必填；<b>固定 4 字节</b>，
    /// 取值见官方「资质编码列表」）。</summary>
    [JsonPropertyName("qualification_code")]
    public string? QualificationCode { get; set; }

    /// <summary>资质图片 id 列表（官方 <c>image_id_list</c>，必填，<c>string[]</c>；数组 1–10 项、
    /// 每项 1–64 字节；图片 id 经官方「资质模块」上传获得）。</summary>
    [JsonPropertyName("image_id_list")]
    public List<string>? ImageIdList { get; set; }

    /// <summary>资质扩展字段（官方 <c>expand_field_list</c>，<c>struct[]</c>，选填；
    /// 键与约束由「获取客户资质结构接口」动态返回）。</summary>
    [JsonPropertyName("expand_field_list")]
    public List<AdsQualificationExpandField>? ExpandFieldList { get; set; }
}

/// <summary>
/// <c>POST /v3.0/qualifications/update</c> 的请求体（2026-10-17 官方页逐表核验，5 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>qualification_type</c> / <c>qualification_id</c> / <c>image_id_list</c>。
/// <b>更新面比创建面窄得多</b>：官方本页参数表没有行业 id / 资质编码（不可改）；
/// <c>expand_field_list</c> 官方原文「<b>非 null 时全量更新</b>」⇒ 传 <c>null</c>（不写该键）即不改，
/// 传列表即整体替换，<b>不做「空即忽略」的本地推断</b>（<c>[]</c> 与不传在
/// <c>WhenWritingNull</c> 下才可区分，调用方须自行把握）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填，<c>integer</c>；<b>不支持代理商 id</b>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>资质类型（官方 <c>qualification_type</c>，必填，<c>enum</c>；可选值同
    /// <see cref="AdsQualificationAddRequest.QualificationType"/>）。</summary>
    [JsonPropertyName("qualification_type")]
    public string? QualificationType { get; set; }

    /// <summary>资质 id（官方 <c>qualification_id</c>，必填，<c>integer</c>；经 add 或 get 获得）。</summary>
    [JsonPropertyName("qualification_id")]
    public long? QualificationId { get; set; }

    /// <summary>资质图片 id 列表（官方 <c>image_id_list</c>，必填，<c>string[]</c>；数组 1–10 项、
    /// 每项 1–64 字节 —— <b>必填即整体替换</b>，本页没有「不传即不改」语义）。</summary>
    [JsonPropertyName("image_id_list")]
    public List<string>? ImageIdList { get; set; }

    /// <summary>资质扩展字段（官方 <c>expand_field_list</c>，<c>struct[]</c>；官方原文
    /// 「非 null 时全量更新」）。</summary>
    [JsonPropertyName("expand_field_list")]
    public List<AdsQualificationExpandField>? ExpandFieldList { get; set; }
}

/// <summary><c>qualifications/add|update</c> 的 <c>data</c> 载荷（两页同形：只有资质 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationIdData
{
    /// <summary>资质 id（官方 <c>qualification_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("qualification_id")]
    public long? QualificationId { get; set; }
}

/// <summary><c>POST /v3.0/qualifications/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationAddResponse : AdsResponse<AdsQualificationIdData>
{
}

/// <summary><c>POST /v3.0/qualifications/update</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationUpdateResponse : AdsResponse<AdsQualificationIdData>
{
}

/// <summary>
/// <c>POST /v3.0/qualifications/delete</c> 的请求体（2026-10-18 官方页逐表核验，4 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>qualification_type</c>。
/// <c>qualification_id</c> 与 <c>qualification_id_list</c> 官方均标<b>选填</b>，两者语义分别是
/// 「删除单个」与「批量删除」；官方未在参数表印二者互斥或必填其一，SDK 按原文建模、不本地拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationDeleteRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填，<c>integer</c>；<b>不支持代理商 id</b>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>资质类型（官方 <c>qualification_type</c>，必填，<c>enum</c>；可选值同
    /// <see cref="AdsQualificationAddRequest.QualificationType"/>）。</summary>
    [JsonPropertyName("qualification_type")]
    public string? QualificationType { get; set; }

    /// <summary>资质 id（官方 <c>qualification_id</c>，选填，<c>integer</c>；删除单个资质时使用）。</summary>
    [JsonPropertyName("qualification_id")]
    public long? QualificationId { get; set; }

    /// <summary>资质 id 列表（官方 <c>qualification_id_list</c>，选填，<c>integer[]</c>；批量删除时使用）。</summary>
    [JsonPropertyName("qualification_id_list")]
    public List<long>? QualificationIdList { get; set; }
}

/// <summary><c>POST /v3.0/qualifications/delete</c> 的 <c>data</c> 载荷（2026-10-18 逐表核验：两个 id 键）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationDeleteData
{
    /// <summary>被删除的资质 id（官方 <c>qualification_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("qualification_id")]
    public long? QualificationId { get; set; }

    /// <summary>被批量删除的资质 id 列表（官方 <c>qualification_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("qualification_id_list")]
    public List<long>? QualificationIdList { get; set; }
}

/// <summary><c>POST /v3.0/qualifications/delete</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Qualifications")]
public class AdsQualificationDeleteResponse : AdsResponse<AdsQualificationDeleteData>
{
}
