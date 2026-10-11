// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 「开始推送票据」请求（官方 <c>api_start_push_ticket</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>SDK 不自动回填</b>：声明式客户端无凭证注入通道（本端点免令牌、也无配置级 secret），
/// <c>component_appid</c> / <c>component_secret</c> 须由调用方显式提供
/// （凭证仅进请求体，序列化面不含 URL / 日志面；调用方自行保证不落日志）。
/// </para>
/// <para>官方语义：引导微信服务器开始向本票据接收 URL 推送 <c>component_verify_ticket</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformStartPushTicketRequest
{
    /// <summary>获取或设置第三方平台 <c>appid</c>（官方 <c>component_appid</c>）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>获取或设置第三方平台 <c>appsecret</c>（官方 <c>component_secret</c>；敏感字段，勿落日志）。</summary>
    [JsonPropertyName("component_secret")]
    public string? ComponentSecret { get; set; }
}

/// <summary>
/// 「清除 API 每日调用配额（v2）」请求（官方 <c>clear_quota/v2</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformClearQuotaRequest
{
    /// <summary>获取或设置要清除配额的账号 <c>appid</c>（可以是授权方或平台自身；官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置第三方平台 <c>appid</c>（官方 <c>component_appid</c>）。</summary>
    [JsonPropertyName("component_appid")]
    public string? ComponentAppId { get; set; }

    /// <summary>获取或设置要清除配额的账号的 <c>appsecret</c>（官方 <c>appsecret</c>；敏感字段，勿落日志）。</summary>
    [JsonPropertyName("appsecret")]
    public string? AppSecret { get; set; }
}

/// <summary>
/// 「快速创建小程序（企业法定名称主体）」请求（官方 <c>fastregisterweapp?action=create</c>）。
/// </summary>
/// <remarks>
/// <para>官方约束：同一企业主体持续失败多次（如法人微信号不一致）将被限制提交；注册结果经
/// <c>notify_third_fasteregister</c> 事件推送（属小程序面事件，本线不建模其载荷）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterCreateRequest
{
    /// <summary>获取或设置企业名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置企业代码（官方 <c>code</c>，必填；一般为统一社会信用代码）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置企业代码类型（官方 <c>code_type</c>，必填；1=统一社会信用代码（十八位统一社会信用代码），2=组织机构代码（九位组织机构代码））。</summary>
    [JsonPropertyName("code_type")]
    public int CodeType { get; set; }

    /// <summary>获取或设置法人姓名（官方 <c>legal_persona_name</c>，必填）。</summary>
    [JsonPropertyName("legal_persona_name")]
    public string LegalPersonaName { get; set; } = string.Empty;

    /// <summary>获取或设置法人微信号（官方 <c>legal_persona_wechat</c>，必填；法人须绑定银行卡的微信实名账号）。</summary>
    [JsonPropertyName("legal_persona_wechat")]
    public string LegalPersonaWechat { get; set; } = string.Empty;

    /// <summary>获取或设置第三方联系电话（官方 <c>component_phone</c>，可选）。</summary>
    [JsonPropertyName("component_phone")]
    public string? ComponentPhone { get; set; }
}

/// <summary>
/// 「查询快速创建小程序任务状态」请求（官方 <c>fastregisterweapp?action=search</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterSearchRequest
{
    /// <summary>获取或设置企业名称（官方 <c>name</c>，必填；与创建时一致）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置法人姓名（官方 <c>legal_persona_name</c>，必填）。</summary>
    [JsonPropertyName("legal_persona_name")]
    public string LegalPersonaName { get; set; } = string.Empty;

    /// <summary>获取或设置法人微信号（官方 <c>legal_persona_wechat</c>，必填）。</summary>
    [JsonPropertyName("legal_persona_wechat")]
    public string LegalPersonaWechat { get; set; } = string.Empty;
}
