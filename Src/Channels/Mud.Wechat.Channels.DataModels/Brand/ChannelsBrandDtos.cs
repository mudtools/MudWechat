// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>新增品牌资质（<c>addbrandlogic</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 请求体 <c>{"brand":{…}}</c>；<b>每个品牌只能调用一次新增接口</b>，
/// 已有申请记录的品牌须改用「更新品牌资质」。资质文件须先用「上传资质图片」接口上传获取 file_id。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsAddBrandRequest
{
    /// <summary>获取或设置品牌详情（官方 <c>brand</c>，必填）。</summary>
    [JsonPropertyName("brand")]
    public ChannelsBrandDetail Brand { get; set; } = new();
}

/// <summary>更新品牌资质（<c>updatebrandlogic</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体 <c>{"brand":{…}}</c>；调用即自动提交审核。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsUpdateBrandRequest
{
    /// <summary>获取或设置品牌详情（官方 <c>brand</c>，必填；与新增请求的 brand 对象字段集一致 ⇒ 共用 <see cref="ChannelsBrandDetail"/>）。</summary>
    [JsonPropertyName("brand")]
    public ChannelsBrandDetail Brand { get; set; } = new();
}

/// <summary>品牌资质提交详情（官方 <c>brand</c> 对象——「新增」与「更新」两请求体字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandDetail
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，必填；本端点官方类型为 number，与品牌库列表返回的 string 型不一致，照抄官方）。</summary>
    [JsonPropertyName("brand_id")]
    public long BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>，选填，从品牌库获取）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>，选填，从品牌库获取）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }

    /// <summary>获取或设置商标分类号（官方 <c>classification_no</c>，必填，取值范围 1-45）。</summary>
    [JsonPropertyName("classification_no")]
    public string ClassificationNo { get; set; } = string.Empty;

    /// <summary>获取或设置商标类型（官方 <c>trade_mark_symbol</c>，必填，见 <see cref="ChannelsBrandTradeMarkSymbols"/>）。</summary>
    [JsonPropertyName("trade_mark_symbol")]
    public int TradeMarkSymbol { get; set; }

    /// <summary>获取或设置注册信息（官方 <c>register_details</c>，选填；R 标时必填商标注册人与注册号）。</summary>
    [JsonPropertyName("register_details")]
    public ChannelsBrandRegisterDetails? RegisterDetails { get; set; }

    /// <summary>获取或设置申请信息（官方 <c>application_details</c>，选填；TM 标时必填）。</summary>
    [JsonPropertyName("application_details")]
    public ChannelsBrandApplicationDetails? ApplicationDetails { get; set; }

    /// <summary>获取或设置商标授权类型（官方 <c>grant_type</c>，选填，见 <see cref="ChannelsBrandGrantTypes"/>）。</summary>
    [JsonPropertyName("grant_type")]
    public int? GrantType { get; set; }

    /// <summary>获取或设置品牌信息（官方 <c>grant_details</c>，选填；授权品牌必填授权书与授权级数）。</summary>
    [JsonPropertyName("grant_details")]
    public ChannelsBrandGrantDetails? GrantDetails { get; set; }
}

/// <summary>商标注册信息（官方 <c>register_details</c> 对象，提交形态与响应形态字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandRegisterDetails
{
    /// <summary>获取或设置商标注册人（官方 <c>registrant</c>，R 标时必填）。</summary>
    [JsonPropertyName("registrant")]
    public string? Registrant { get; set; }

    /// <summary>获取或设置商标注册号（官方 <c>register_no</c>，R 标时必填）。</summary>
    [JsonPropertyName("register_no")]
    public string? RegisterNo { get; set; }

    /// <summary>获取或设置注册有效期开始时间（官方 <c>start_time</c>，选填，长期有效可不填）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置注册有效期结束时间（官方 <c>end_time</c>，选填，长期有效可不填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否长期有效（官方 <c>is_permanent</c>，必填）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }

    /// <summary>获取或设置商标注册证 file_id 列表（官方 <c>register_certifications</c>，R 标时必填，最多 1 张）。</summary>
    [JsonPropertyName("register_certifications")]
    public List<string>? RegisterCertifications { get; set; }

    /// <summary>获取或设置变更/续展证明 file_id 列表（官方 <c>renew_certifications</c>，选填，最多 5 张）。</summary>
    [JsonPropertyName("renew_certifications")]
    public List<string>? RenewCertifications { get; set; }
}

/// <summary>商标申请信息（官方 <c>application_details</c> 对象，提交形态与响应形态字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandApplicationDetails
{
    /// <summary>获取或设置商标申请受理时间（官方 <c>acceptance_time</c>，TM 标时必填）。</summary>
    [JsonPropertyName("acceptance_time")]
    public long? AcceptanceTime { get; set; }

    /// <summary>获取或设置商标注册申请受理书 file_id 列表（官方 <c>acceptance_certification</c>，TM 标时必填，最多 1 张）。</summary>
    [JsonPropertyName("acceptance_certification")]
    public List<string>? AcceptanceCertification { get; set; }

    /// <summary>获取或设置商标申请号（官方 <c>acceptance_no</c>，TM 标时必填）。</summary>
    [JsonPropertyName("acceptance_no")]
    public string? AcceptanceNo { get; set; }
}

/// <summary>品牌授权信息（官方 <c>grant_details</c> 对象，提交形态与响应形态字段集一致 ⇒ 共用）。</summary>
/// <remarks>授权品牌建议使用分级授权书结构（<c>use_split_grant_info</c> 大于 0 即使用，使用时兼容填充旧结构）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandGrantDetails
{
    /// <summary>获取或设置（旧结构）品牌销售授权书 file_id 列表（官方 <c>grant_certifications</c>，授权品牌必填，最多 9 张）。</summary>
    [JsonPropertyName("grant_certifications")]
    public List<string>? GrantCertifications { get; set; }

    /// <summary>获取或设置授权级数（官方 <c>grant_level</c>，授权品牌必填，取值 1-3）。</summary>
    [JsonPropertyName("grant_level")]
    public int? GrantLevel { get; set; }

    /// <summary>获取或设置（旧结构）授权有效期开始时间（官方 <c>start_time</c>）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置（旧结构）授权有效期结束时间（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置（旧结构）是否长期有效（官方 <c>is_permanent</c>）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }

    /// <summary>获取或设置品牌权利人证件照 file_id 列表（官方 <c>brand_owner_id_photos</c>，选填，最多 2 张）。</summary>
    [JsonPropertyName("brand_owner_id_photos")]
    public List<string>? BrandOwnerIdPhotos { get; set; }

    /// <summary>获取或设置是否使用分级授权书结构（官方 <c>use_split_grant_info</c>，大于 0 即使用）。</summary>
    [JsonPropertyName("use_split_grant_info")]
    public long? UseSplitGrantInfo { get; set; }

    /// <summary>获取或设置一级授权结构（官方 <c>grant_info_lv1</c>；多于一级授权时必填）。</summary>
    [JsonPropertyName("grant_info_lv1")]
    public ChannelsBrandGrantInfo? GrantInfoLv1 { get; set; }

    /// <summary>获取或设置二级授权结构（官方 <c>grant_info_lv2</c>；多于二级授权时必填）。</summary>
    [JsonPropertyName("grant_info_lv2")]
    public ChannelsBrandGrantInfo? GrantInfoLv2 { get; set; }

    /// <summary>获取或设置三级授权结构（官方 <c>grant_info_lv3</c>；三级授权时必填）。</summary>
    [JsonPropertyName("grant_info_lv3")]
    public ChannelsBrandGrantInfo? GrantInfoLv3 { get; set; }

    /// <summary>获取或设置联系方式列表（官方 <c>contact_info_list</c>，选填）。</summary>
    [JsonPropertyName("contact_info_list")]
    public List<ChannelsBrandContactInfo>? ContactInfoList { get; set; }
}

/// <summary>分级授权结构（官方 <c>grant_info_lv1</c>/<c>grant_info_lv2</c>/<c>grant_info_lv3</c>，三级字段集一致 ⇒ 共用一个 DTO）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandGrantInfo
{
    /// <summary>获取或设置（分级结构）品牌销售授权书 file_id 列表（官方 <c>grant_certifications</c>；对应级别授权时必填）。</summary>
    [JsonPropertyName("grant_certifications")]
    public List<string>? GrantCertifications { get; set; }

    /// <summary>获取或设置（分级结构）授权有效期开始时间（官方 <c>start_time</c>）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置（分级结构）授权有效期结束时间（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置（分级结构）是否长期有效（官方 <c>is_permanent</c>）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }
}

/// <summary>联系方式（官方 <c>contact_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandContactInfo
{
    /// <summary>获取或设置联系方式类型（官方 <c>key</c>，如联系电话）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置联系方式内容（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>品牌资质审核单 ID 响应（<c>addbrandlogic</c> 与 <c>updatebrandlogic</c> 响应字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandAuditIdResponse : ChannelsResponse
{
    /// <summary>获取或设置审核单 ID（官方 <c>audit_id</c>，提交审核成功后返回）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }
}

/// <summary>获取品牌库列表（<c>getallbrandslogic</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetAllBrandsRequest
{
    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，默认 10，不超过 50）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，选填；上次请求返回的翻页上下文，不传则默认获取第一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取品牌库列表（<c>getallbrandslogic</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetAllBrandsResponse : ChannelsResponse
{
    /// <summary>获取或设置品牌库中的品牌信息列表（官方 <c>brands</c>）。</summary>
    [JsonPropertyName("brands")]
    public List<ChannelsBrandLibraryItem>? Brands { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>next_key</c>，用于请求下一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置是否还有下一页内容（官方 <c>continue_flag</c>）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }
}

/// <summary>品牌库品牌条目（官方 <c>brands</c> 数组元素，<c>getallbrandslogic</c>）。</summary>
/// <remarks>官方本端点 <c>brand_id</c> 类型为 <b>string</b>（与提交/详情侧 number 型不一致，照抄官方）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandLibraryItem
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，官方本端点为 string 型）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }
}

/// <summary>撤回品牌资质审核（<c>cancelauditbrandlogic</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsCancelBrandAuditRequest
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，必填，官方示例 "10000000"）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;

    /// <summary>获取或设置要撤销的审核单 ID（官方 <c>audit_id</c>，必填，提交审核成功后返回）。</summary>
    [JsonPropertyName("audit_id")]
    public string AuditId { get; set; } = string.Empty;
}

/// <summary>删除品牌资质（<c>deletebrandlogic</c>）请求体。</summary>
/// <remarks>官方业务限制：删除品牌资质后，对应品牌关联的商品将会被下架。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsDeleteBrandRequest
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，必填，官方示例 "10000000"）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;
}

/// <summary>获取品牌资质申请详情（<c>getbrandlogic</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetBrandRequest
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，必填，官方示例 "10000531"）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;
}

/// <summary>获取品牌资质申请详情（<c>getbrandlogic</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetBrandResponse : ChannelsResponse
{
    /// <summary>获取或设置品牌资质详情（官方 <c>brand</c>；与两个列表接口的元素字段集一致 ⇒ 共用 <see cref="ChannelsBrandApplicationInfo"/>）。</summary>
    [JsonPropertyName("brand")]
    public ChannelsBrandApplicationInfo? Brand { get; set; }
}

/// <summary>获取品牌资质申请列表（<c>getbrandlistlogic</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetBrandListRequest
{
    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，默认 10，不超过 50）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// 获取或设置状态筛选（官方 <c>status</c>，选填，不传或传 0 返回全部；见 <see cref="ChannelsBrandListFilterStatuses"/>——
    /// 与品牌资质状态 <see cref="ChannelsBrandStatuses"/> 语义不同，勿混用）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置翻页游标（官方 <c>next_key</c>，选填；首请求传空，返回空表示最后一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取品牌资质申请列表（<c>getbrandlistlogic</c>）响应。</summary>
/// <remarks>官方业务限制：返回的是商家提交的申请记录（草稿数据）——要获取线上生效中的资质，请使用「获取生效中的品牌资质列表」接口。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetBrandListResponse : ChannelsResponse
{
    /// <summary>获取或设置品牌资质申请单列表（官方 <c>brands</c>）。</summary>
    [JsonPropertyName("brands")]
    public List<ChannelsBrandApplicationInfo>? Brands { get; set; }

    /// <summary>获取或设置符合条件的申请单总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置翻页游标（官方 <c>next_key</c>，返回空表示最后一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取生效中的品牌资质列表（<c>getvalidbrandlistlogic</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetValidBrandListRequest
{
    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，默认 10，不超过 50）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页游标（官方 <c>next_key</c>，选填；不传则默认获取第一页，返回空表示已到最后一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取生效中的品牌资质列表（<c>getvalidbrandlistlogic</c>）响应。</summary>
/// <remarks>仅返回状态为生效中（3）和即将过期（5）的品牌资质；即将过期的资质不影响商品正常售卖，但建议尽快续期。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsGetValidBrandListResponse : ChannelsResponse
{
    /// <summary>获取或设置生效中的品牌资质列表（官方 <c>brands</c>）。</summary>
    [JsonPropertyName("brands")]
    public List<ChannelsBrandApplicationInfo>? Brands { get; set; }

    /// <summary>获取或设置符合条件的品牌资质总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置翻页游标（官方 <c>next_key</c>，返回空表示已到最后一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>
/// 品牌资质申请单（官方 <c>brand</c> 对象与两个列表接口 <c>brands</c> 数组元素字段集一致 ⇒ 共用；
/// 覆盖 <c>getbrandlogic</c> / <c>getbrandlistlogic</c> / <c>getvalidbrandlistlogic</c> 三端点）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandApplicationInfo
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>；本形态官方类型为 number，与品牌库列表 string 型不一致，照抄官方）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }

    /// <summary>获取或设置商标分类号（官方 <c>classification_no</c>，取值范围 1-45）。</summary>
    [JsonPropertyName("classification_no")]
    public string? ClassificationNo { get; set; }

    /// <summary>获取或设置商标类型（官方 <c>trade_mark_symbol</c>，见 <see cref="ChannelsBrandTradeMarkSymbols"/>）。</summary>
    [JsonPropertyName("trade_mark_symbol")]
    public int? TradeMarkSymbol { get; set; }

    /// <summary>获取或设置注册信息（官方 <c>register_details</c>）。</summary>
    [JsonPropertyName("register_details")]
    public ChannelsBrandRegisterDetails? RegisterDetails { get; set; }

    /// <summary>获取或设置申请信息（官方 <c>application_details</c>）。</summary>
    [JsonPropertyName("application_details")]
    public ChannelsBrandApplicationDetails? ApplicationDetails { get; set; }

    /// <summary>获取或设置商标授权类型（官方 <c>grant_type</c>，见 <see cref="ChannelsBrandGrantTypes"/>）。</summary>
    [JsonPropertyName("grant_type")]
    public int? GrantType { get; set; }

    /// <summary>获取或设置品牌信息（官方 <c>grant_details</c>）。</summary>
    [JsonPropertyName("grant_details")]
    public ChannelsBrandGrantDetails? GrantDetails { get; set; }

    /// <summary>获取或设置申请单状态（官方 <c>status</c>，见 <see cref="ChannelsBrandStatuses"/>；生效中列表仅返回 3 / 5）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置审核结果（官方 <c>audit_result</c>）。</summary>
    [JsonPropertyName("audit_result")]
    public ChannelsBrandAuditResult? AuditResult { get; set; }
}

/// <summary>品牌资质审核结果（官方 <c>audit_result</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandAuditResult
{
    /// <summary>获取或设置审核单 ID（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }

    /// <summary>获取或设置审核不通过原因（官方 <c>reject_reason</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>商标类型常量（官方 <c>trade_mark_symbol</c> 枚举值）。</summary>
public static class ChannelsBrandTradeMarkSymbols
{
    /// <summary>R 标（已注册商标）。</summary>
    public const int Registered = 1;

    /// <summary>TM 标（申请中商标）。</summary>
    public const int Trademark = 2;
}

/// <summary>商标授权类型常量（官方 <c>grant_type</c> 枚举值）。</summary>
public static class ChannelsBrandGrantTypes
{
    /// <summary>自有品牌。</summary>
    public const int Owned = 1;

    /// <summary>授权品牌。</summary>
    public const int Authorized = 2;
}

/// <summary>品牌资质状态常量（官方 <c>status</c> 枚举值，<c>getbrandlogic</c> / 两个列表接口）。</summary>
public static class ChannelsBrandStatuses
{
    /// <summary>审核中。</summary>
    public const int Auditing = 1;

    /// <summary>审核失败。</summary>
    public const int Rejected = 2;

    /// <summary>已生效。</summary>
    public const int Effective = 3;

    /// <summary>已撤回。</summary>
    public const int Withdrawn = 4;

    /// <summary>即将过期（不影响商品售卖）。</summary>
    public const int ExpiringSoon = 5;

    /// <summary>已过期。</summary>
    public const int Expired = 6;
}

/// <summary>品牌资质申请列表状态筛选常量（官方 <c>status</c> 请求参数枚举值，<c>getbrandlistlogic</c>——与品牌资质状态枚举语义不同，勿混用）。</summary>
public static class ChannelsBrandListFilterStatuses
{
    /// <summary>获取全部（不传或传 0 返回全部）。</summary>
    public const int All = 0;

    /// <summary>审核中。</summary>
    public const int Auditing = 1;

    /// <summary>审核失败。</summary>
    public const int Rejected = 2;

    /// <summary>审核通过（含即将过期 / 已过期）。</summary>
    public const int Approved = 3;

    /// <summary>已撤回。</summary>
    public const int Withdrawn = 4;
}
