// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.CustomAudiences;

/// <summary>
/// <c>POST /v3.0/custom_audiences/add</c> 的请求体（2026-10-17 官方页逐表核验，6 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>name</c> / <c>type</c>。<c>type</c> 决定
/// <see cref="AudienceSpec"/> 内哪一支子 spec 生效（互斥语义见 <see cref="AdsAudienceSpec"/> remarks）。
/// <c>outer_audience_id</c> 幂等语义（官方原文）：如果该编码创建过人群，再次用该编码创建时返回之前创建的人群 id。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>人群名称（官方 <c>name</c>，必填；1–32 字节；官方约束：同一个帐号下的人群不许重名）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>人群类型（官方 <c>type</c>，必填，<c>enum</c>；可选值
    /// <c>{ CUSTOMER_FILE, LOOKALIKE, USER_ACTION, KEYWORD, AD, COMBINE }</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>外部人群编码（官方 <c>outer_audience_id</c>；1–128 字节；只能包含数字/字母/下划线；
    /// 官方约束：人群编码不能与标签编码 tag_code 重复；同编码重复创建返回既有 audience_id）。</summary>
    [JsonPropertyName("outer_audience_id")]
    public string? OuterAudienceId { get; set; }

    /// <summary>定向描述（官方 <c>description</c>；0–250 字节）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>人群信息（官方 <c>audience_spec</c>；与 <see cref="Type"/> 相关的互斥子 spec 容器）。</summary>
    [JsonPropertyName("audience_spec")]
    public AdsAudienceSpec? AudienceSpec { get; set; }
}

/// <summary><c>custom_audiences/add</c> 的 <c>data</c> 载荷（只有人群 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsAudienceIdData
{
    /// <summary>人群 id（官方 <c>audience_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("audience_id")]
    public long? AudienceId { get; set; }
}

/// <summary><c>POST /v3.0/custom_audiences/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceAddResponse : AdsResponse<AdsAudienceIdData>
{
}

/// <summary>
/// <c>POST /v3.0/custom_audiences/update</c> 的请求体（2026-10-17 官方页逐表核验，5 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>audience_id</c>。官方约束（原文）：<b>人群名称、人群描述、是否深度合作至少填写一个</b>；
/// 且「只有客户人群的创建者才能修改是否深度合作」⇒ 条件判定交官方应答 <c>code</c>，SDK 不本地拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，必填，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>人群 id（官方 <c>audience_id</c>，必填；经 <c>custom_audiences/add</c> 创建获得）。</summary>
    [JsonPropertyName("audience_id")]
    public long? AudienceId { get; set; }

    /// <summary>人群名称（官方 <c>name</c>；1–32 字节；同帐号下不许重名）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>人群描述（官方 <c>description</c>；最大 100 字节。注意与 add 页的 250 字节上限不同构）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>深度数据合作（官方 <c>cooperated</c>，<c>boolean</c>；官方原文：可将您的数据从 DMP 平台导出、
    /// 平台为您进行定制化挖掘；只有客户人群的创建者才能修改）。</summary>
    [JsonPropertyName("cooperated")]
    public bool? Cooperated { get; set; }
}

/// <summary><c>POST /v3.0/custom_audiences/update</c> 的闭合应答（官方 data 为空载荷）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceUpdateResponse : AdsResponse
{
}

/// <summary>人群条目（官方 <c>custom_audiences/get</c> 的 <c>data.list[]</c> 元素，2026-10-17 逐表核验）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceInfo
{
    /// <summary>人群 id（官方 <c>audience_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("audience_id")]
    public long? AudienceId { get; set; }

    /// <summary>人群归属的推广帐号 id（官方 <c>account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>人群名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>外部人群编码（官方 <c>outer_audience_id</c>；语义同创建入参）。</summary>
    [JsonPropertyName("outer_audience_id")]
    public string? OuterAudienceId { get; set; }

    /// <summary>人群描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>深度数据合作（官方 <c>cooperated</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("cooperated")]
    public bool? Cooperated { get; set; }

    /// <summary>人群类型（官方 <c>type</c>，<c>enum</c>；可选值同 add 入参六支）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>人群来源（官方 <c>source</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>处理状态（官方 <c>status</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>人群包在线状态（官方 <c>online_status</c>，<c>enum</c>；官方原文：人群包一段时间不更新或不用于
    /// 营销定向会被下线处理，下线后无法用于营销定向但洞察不受影响；<b>仅在处理状态为成功时返回和生效</b>）。</summary>
    [JsonPropertyName("online_status")]
    public string? OnlineStatus { get; set; }

    /// <summary>是否是人群包 owner（官方 <c>is_own</c>，<c>boolean</c>；一般人群包创建者为 owner）。</summary>
    [JsonPropertyName("is_own")]
    public bool? IsOwn { get; set; }

    /// <summary>人群错误码（官方 <c>error_code</c>，<c>integer</c>；官方原文：1 系统错误；101 种子人群活跃用户
    /// 低于 2K；102 种子人群无共同特征；201 号码包文件格式错误；202 号码包文件解析失败；203 号码包文件人群匹配失败）。</summary>
    [JsonPropertyName("error_code")]
    public long? ErrorCode { get; set; }

    /// <summary>用户覆盖数（官方 <c>user_count</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("user_count")]
    public long? UserCount { get; set; }

    /// <summary>创建时间（官方 <c>created_time</c>；格式 <c>yyyy-MM-dd HH:mm:ss</c>，字符串承载）。</summary>
    [JsonPropertyName("created_time")]
    public string? CreatedTime { get; set; }

    /// <summary>最后更新时间（官方 <c>last_modified_time</c>；格式 <c>yyyy-MM-dd HH:mm:ss</c>）。</summary>
    [JsonPropertyName("last_modified_time")]
    public string? LastModifiedTime { get; set; }

    /// <summary>人群信息（官方 <c>audience_spec</c>；回显与 type 相关的子 spec）。</summary>
    [JsonPropertyName("audience_spec")]
    public AdsAudienceSpec? AudienceSpec { get; set; }
}

/// <summary><c>custom_audiences/get</c> 的 <c>data</c> 载荷（列表 + 分页）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceListData
{
    /// <summary>人群列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsCustomAudienceInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/custom_audiences/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceGetResponse : AdsResponse<AdsCustomAudienceListData>
{
}
