// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>生成 URL Link 请求体（<c>POST /wxa/generate_urllink</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaUrlLinkRequest
{
    /// <summary>小程序页面路径（<c>path</c>，必填；不可带 <c>.html</c> 后缀，可带 query）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面参数（<c>query</c>，选填；<b>须已 URL 编码</b>，官方原文）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）；取值语义以官方页面为准，SDK 不做本地校验。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，选填）；单位与上限随 <c>expire_type</c>，以官方页面为准。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效绝对时间（<c>expire_time</c>，选填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>云开发配置（<c>cloud_base</c>，选填），见 <see cref="WxaCloudBase"/>。</summary>
    [JsonPropertyName("cloud_base")]
    public WxaCloudBase? CloudBase { get; set; }
}

/// <summary>生成 URL Link 应答（<c>url_link</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaUrlLinkResponse : WxaResponse
{
    /// <summary>生成的加密链接（<c>url_link</c>）。</summary>
    [JsonPropertyName("url_link")]
    public string? UrlLink { get; set; }
}

/// <summary>查询 URL Link 请求体（<c>POST /wxa/query_urllink</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQueryUrlLinkRequest
{
    /// <summary>待查询的加密链接（<c>url_link</c>，<c>query_type = 0</c> 时必填）。</summary>
    [JsonPropertyName("url_link")]
    public string? UrlLink { get; set; }

    /// <summary>
    /// 查询类型（<c>query_type</c>，选填）：<c>0</c>（默认）= 查询链接配置；<c>1</c> = 查询当天剩余访问次数。
    /// </summary>
    [JsonPropertyName("query_type")]
    public long? QueryType { get; set; }
}

/// <summary>查询 URL Link 应答（<c>url_link_info</c> 或 <c>quota_info</c>）。</summary>
/// <remarks>
/// 官方示例另含 <c>cloud_base.doamin</c>（<b>官方拼写如此</b>）等未列入参数表的字段；
/// 本 DTO <b>不建模示例专属字段</b>（不据示例断言线上字段名），宿主可按需自行读取原始报文。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQueryUrlLinkResponse : WxaResponse
{
    /// <summary>链接配置（<c>url_link_info</c>；<c>query_type = 1</c> 时缺省），见 <see cref="WxaUrlLinkInfo"/>。</summary>
    [JsonPropertyName("url_link_info")]
    public WxaUrlLinkInfo? UrlLinkInfo { get; set; }

    /// <summary>额度配置（<c>quota_info</c>；<c>query_type = 1</c> 时返回），见 <see cref="WxaQuotaInfo"/>。</summary>
    [JsonPropertyName("quota_info")]
    public WxaQuotaInfo? QuotaInfo { get; set; }
}

/// <summary>URL Link 配置（<c>url_link_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaUrlLinkInfo
{
    /// <summary>小程序 AppID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>小程序页面路径（<c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面参数（<c>query</c>）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>创建时间（<c>create_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>到期失效时间（<c>expire_time</c>，Unix 秒级时间戳；<c>0</c> 表示永久生效）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>要打开的小程序版本（<c>env_version</c>）：<c>release</c>（正式）/ <c>trial</c>（体验）/ <c>develop</c>（开发）。</summary>
    [JsonPropertyName("env_version")]
    public string? EnvVersion { get; set; }
}

/// <summary>额度配置（<c>quota_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQuotaInfo
{
    /// <summary>当天剩余访问次数（<c>remain_visit_quota</c>；加密 URL Link / Scheme 共用该额度）。</summary>
    [JsonPropertyName("remain_visit_quota")]
    public long? RemainVisitQuota { get; set; }
}

/// <summary>生成 URL Scheme 请求体（<c>POST /wxa/generatescheme</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaSchemeRequest
{
    /// <summary>跳转小程序配置（<c>jump_wxa</c>），见 <see cref="WxaJumpInfo"/>。</summary>
    [JsonPropertyName("jump_wxa")]
    public WxaJumpInfo? JumpWxa { get; set; }

    /// <summary>是否过期（<c>is_expire</c>，选填；<c>false</c> 表示长期有效）。</summary>
    [JsonPropertyName("is_expire")]
    public bool? IsExpire { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）；取值语义以官方页面为准。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，选填）。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效绝对时间（<c>expire_time</c>，选填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

/// <summary>跳转小程序配置（<c>jump_wxa</c> / <c>cloud_base</c> 共用的三字段形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaJumpInfo
{
    /// <summary>小程序页面路径（<c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面参数（<c>query</c>）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>小程序版本（<c>env_version</c>）：<c>release</c> / <c>trial</c> / <c>develop</c>。</summary>
    [JsonPropertyName("env_version")]
    public string? EnvVersion { get; set; }
}

/// <summary>云开发配置（<c>cloud_base</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaCloudBase
{
    /// <summary>云开发环境 ID（<c>env</c>）。</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>云开发页面路径（<c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>云开发页面参数（<c>query</c>）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }
}

/// <summary>生成 URL Scheme 应答（<c>openlink</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaSchemeResponse : WxaResponse
{
    /// <summary>生成的 scheme 码（<c>openlink</c>）。</summary>
    [JsonPropertyName("openlink")]
    public string? OpenLink { get; set; }
}

/// <summary>查询 URL Scheme 请求体（<c>POST /wxa/queryscheme</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQuerySchemeRequest
{
    /// <summary>待查询的 scheme（<c>scheme</c>，<c>query_type = 0</c> 时必填）。</summary>
    [JsonPropertyName("scheme")]
    public string? Scheme { get; set; }

    /// <summary>查询类型（<c>query_type</c>，选填）：<c>0</c>（默认）配置；<c>1</c> 当天剩余访问次数。</summary>
    [JsonPropertyName("query_type")]
    public long? QueryType { get; set; }
}

/// <summary>查询 URL Scheme 应答（<c>scheme_info</c> 或 <c>quota_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQuerySchemeResponse : WxaResponse
{
    /// <summary>scheme 配置（<c>scheme_info</c>；<c>query_type = 1</c> 时缺省），见 <see cref="WxaSchemeInfo"/>。</summary>
    [JsonPropertyName("scheme_info")]
    public WxaSchemeInfo? SchemeInfo { get; set; }

    /// <summary>额度配置（<c>quota_info</c>；<c>query_type = 1</c> 时返回），见 <see cref="WxaQuotaInfo"/>。</summary>
    [JsonPropertyName("quota_info")]
    public WxaQuotaInfo? QuotaInfo { get; set; }
}

/// <summary>URL Scheme 配置（<c>scheme_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaSchemeInfo
{
    /// <summary>小程序 AppID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>小程序页面路径（<c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面参数（<c>query</c>）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>创建时间（<c>create_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>到期失效时间（<c>expire_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>要打开的小程序版本（<c>env_version</c>）：<c>release</c> / <c>trial</c> / <c>develop</c>。</summary>
    [JsonPropertyName("env_version")]
    public string? EnvVersion { get; set; }
}

/// <summary>生成 ShortLink 请求体（<c>POST /wxa/genwxashortlink</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaShortLinkRequest
{
    /// <summary>
    /// 小程序页面路径（<c>page_url</c>，必填）：官方要求该页面<b>已配置 <c>sitemap.json</c></b>；
    /// <b>不支持 <c>query</c> 参数</b>（与 URL Link / Scheme 的本质差异）。
    /// </summary>
    [JsonPropertyName("page_url")]
    public string? PageUrl { get; set; }

    /// <summary>短链标题（<c>page_title</c>，选填）。</summary>
    [JsonPropertyName("page_title")]
    public string? PageTitle { get; set; }

    /// <summary>是否永久有效（<c>is_permanent</c>，选填；<c>true</c> 时忽略失效参数）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）；取值语义以官方页面为准。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，选填）。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效绝对时间（<c>expire_time</c>，选填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

/// <summary>生成 ShortLink 应答（<c>link</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaShortLinkResponse : WxaResponse
{
    /// <summary>生成的短链（<c>link</c>）。</summary>
    [JsonPropertyName("link")]
    public string? Link { get; set; }
}

/// <summary>生成 NFC scheme 请求体（<c>POST /wxa/generatenfcscheme</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>qrcode-link/url-scheme/api_generatenfcscheme.html</c>。</para>
/// <para>
/// <b>NFC 场景（官方原文）</b>：适用于 NFC 拉起小程序的业务场景（如智能硬件碰一碰直达页面）；
/// <see cref="Sn"/>（设备序列号）与 <see cref="ModelId"/>（设备型号 ID）为<b> NFC 专属必填参数</b>，
/// 其余字段与 <see cref="WxaSchemeRequest"/> 同形但<b>不可等价互换</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaNfcSchemeRequest
{
    /// <summary>设备序列号（<c>sn</c>，必填）。</summary>
    [JsonPropertyName("sn")]
    public string? Sn { get; set; }

    /// <summary>设备型号 ID（<c>model_id</c>，必填）。</summary>
    [JsonPropertyName("model_id")]
    public string? ModelId { get; set; }

    /// <summary>跳转小程序配置（<c>jump_wxa</c>，必填），见 <see cref="WxaJumpInfo"/>。</summary>
    [JsonPropertyName("jump_wxa")]
    public WxaJumpInfo? JumpWxa { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）：<c>0</c> 失效时间间隔 / <c>1</c> 失效日期（默认 <c>0</c>）。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，<c>expire_type = 0</c> 时必填；单位天，官方范围 7~30）。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效日期（<c>expire_time</c>，<c>expire_type = 1</c> 时必填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>是否过期（<c>is_expire</c>，选填；官方原文：使用失效字段前先将此值设为 <c>true</c>）。</summary>
    [JsonPropertyName("is_expire")]
    public bool? IsExpire { get; set; }
}
