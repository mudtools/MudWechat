// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.CustomAudiences;

/// <summary>人群数据文件条目（官方 <c>custom_audience_files/get</c> 的 <c>data.list[]</c> 元素，2026-10-17 逐表核验）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceFileInfo
{
    /// <summary>人群 id（官方 <c>audience_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("audience_id")]
    public long? AudienceId { get; set; }

    /// <summary>数据文件 id（官方 <c>custom_audience_file_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("custom_audience_file_id")]
    public long? CustomAudienceFileId { get; set; }

    /// <summary>文件名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>号码包用户 id 类型（官方 <c>user_id_type</c>，<c>enum</c>；可选值
    /// <c>{ HASH_IDFA, HASH_IMEI, HASH_MOBILE_PHONE, IDFA, IMEI, WX_OPENID, WX_UNIONID,
    /// WECHAT_OPENID, SALTED_HASH_IMEI, SALTED_HASH_IDFA, OAID, HASH_OAID, SHA256_MOBILE_PHONE,
    /// MD5_SHA256_IMEI, MD5_SHA256_IDFA, MD5_SHA256_OAID, CAID }</c>）。</summary>
    [JsonPropertyName("user_id_type")]
    public string? UserIdType { get; set; }

    /// <summary>文件操作类型（官方 <c>operation_type</c>，<c>enum</c>；不填默认 APPEND；可选值
    /// <c>{ APPEND, REDUCE }</c>）。</summary>
    [JsonPropertyName("operation_type")]
    public string? OperationType { get; set; }

    /// <summary>微信 appid（官方 <c>open_app_id</c>；官方原文：当且仅当数据文件中的用户 id 类型为
    /// WECHAT_OPENID（WX_OPENID）时有效）。</summary>
    [JsonPropertyName("open_app_id")]
    public string? OpenAppId { get; set; }

    /// <summary>文件状态（官方 <c>process_status</c>，<c>enum</c>；枚举详情未逐项核验）。</summary>
    [JsonPropertyName("process_status")]
    public string? ProcessStatus { get; set; }

    /// <summary>处理状态码（官方 <c>process_code</c>，<c>integer</c>；官方原文：处理完成后 0 表示成功、非 0 失败）。</summary>
    [JsonPropertyName("process_code")]
    public long? ProcessCode { get; set; }

    /// <summary>错误具体信息（官方 <c>error_message</c>）。</summary>
    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }

    /// <summary>文件总行数（官方 <c>line_count</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("line_count")]
    public long? LineCount { get; set; }

    /// <summary>格式正确行数（官方 <c>valid_line_count</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("valid_line_count")]
    public long? ValidLineCount { get; set; }

    /// <summary>文件包含的用户数（官方 <c>user_count</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("user_count")]
    public long? UserCount { get; set; }

    /// <summary>文件大小（官方 <c>size</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }

    /// <summary>创建时间（官方 <c>created_time</c>；格式 <c>yyyy-MM-dd HH:mm:ss</c>）。</summary>
    [JsonPropertyName("created_time")]
    public string? CreatedTime { get; set; }
}

/// <summary><c>custom_audience_files/get</c> 的 <c>data</c> 载荷（列表 + 分页）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceFileListData
{
    /// <summary>数据文件列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsCustomAudienceFileInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/custom_audience_files/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceFileGetResponse : AdsResponse<AdsCustomAudienceFileListData>
{
}

/// <summary><c>custom_audience_files/add</c>（multipart 通道）应答的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceFileIdData
{
    /// <summary>数据文件 id（官方 <c>custom_audience_file_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("custom_audience_file_id")]
    public long? CustomAudienceFileId { get; set; }
}

/// <summary><c>POST /v3.0/custom_audience_files/add</c> 的闭合应答（手写 multipart 通道承载）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCustomAudienceFileUploadResponse : AdsResponse<AdsCustomAudienceFileIdData>
{
}
