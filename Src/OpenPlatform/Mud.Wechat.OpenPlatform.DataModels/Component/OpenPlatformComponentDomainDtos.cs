// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 「修改第三方平台服务器域名」请求（官方 <c>modify_wxa_server_domain</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>列表为分号拼接字符串（官方线上形态）</b>：<c>wxa_server_domain</c> 官方以 <c>a.com;b.com</c>
/// 的<b>单字符串</b>承载多个域名（非 JSON 数组）——DTO 保持字符串原样，不做数组互转。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformModifyWxaServerDomainRequest
{
    /// <summary>获取或设置操作类型（官方 <c>action</c>，必填）：<c>add</c> 添加 / <c>delete</c> 删除 / <c>set</c> 覆盖 / <c>get</c> 获取。</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    /// <summary>获取或设置是否同时修改「已发布版本」的域名配置（官方 <c>is_modify_published_together</c>，可选）。</summary>
    [JsonPropertyName("is_modify_published_together")]
    public bool? IsModifyPublishedTogether { get; set; }

    /// <summary>获取或设置服务器域名列表（官方 <c>wxa_server_domain</c>，可选；<b>分号拼接的单字符串</b>，如 <c>a.com;b.com</c>）。</summary>
    [JsonPropertyName("wxa_server_domain")]
    public string? WxaServerDomain { get; set; }
}

/// <summary>
/// 「修改第三方平台服务器域名」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformModifyWxaServerDomainResponse : OpenPlatformResponse
{
    /// <summary>获取或设置已发布版本生效的服务器域名（官方 <c>published_wxa_server_domain</c>；分号拼接字符串）。</summary>
    [JsonPropertyName("published_wxa_server_domain")]
    public string? PublishedWxaServerDomain { get; set; }

    /// <summary>获取或设置测试版生效的服务器域名（官方 <c>testing_wxa_server_domain</c>；分号拼接字符串）。</summary>
    [JsonPropertyName("testing_wxa_server_domain")]
    public string? TestingWxaServerDomain { get; set; }

    /// <summary>获取或设置校验失败的域名（官方 <c>invalid_wxa_server_domain</c>；分号拼接字符串）。</summary>
    [JsonPropertyName("invalid_wxa_server_domain")]
    public string? InvalidWxaServerDomain { get; set; }
}

/// <summary>
/// 「修改第三方平台跳转（H5）域名」请求（官方 <c>modify_wxa_jump_domain</c>）。
/// </summary>
/// <remarks>
/// <para><b>列表为分号拼接字符串</b>（与 <see cref="OpenPlatformModifyWxaServerDomainRequest"/> 同形态）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformModifyWxaJumpDomainRequest
{
    /// <summary>获取或设置操作类型（官方 <c>action</c>，必填）：<c>add</c> / <c>delete</c> / <c>set</c> / <c>get</c>。</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    /// <summary>获取或设置是否同时修改「已发布版本」的域名配置（官方 <c>is_modify_published_together</c>，可选）。</summary>
    [JsonPropertyName("is_modify_published_together")]
    public bool? IsModifyPublishedTogether { get; set; }

    /// <summary>获取或设置跳转（H5）域名列表（官方 <c>wxa_jump_h5_domain</c>，可选；<b>分号拼接的单字符串</b>）。</summary>
    [JsonPropertyName("wxa_jump_h5_domain")]
    public string? WxaJumpH5Domain { get; set; }
}

/// <summary>
/// 「修改第三方平台跳转（H5）域名」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformModifyWxaJumpDomainResponse : OpenPlatformResponse
{
    /// <summary>获取或设置已发布版本生效的跳转域名（官方 <c>published_wxa_jump_h5_domain</c>；分号拼接字符串）。</summary>
    [JsonPropertyName("published_wxa_jump_h5_domain")]
    public string? PublishedWxaJumpH5Domain { get; set; }

    /// <summary>获取或设置测试版生效的跳转域名（官方 <c>testing_wxa_jump_h5_domain</c>；分号拼接字符串）。</summary>
    [JsonPropertyName("testing_wxa_jump_h5_domain")]
    public string? TestingWxaJumpH5Domain { get; set; }

    /// <summary>获取或设置校验失败的域名（官方 <c>invalid_wxa_jump_h5_domain</c>；分号拼接字符串）。</summary>
    [JsonPropertyName("invalid_wxa_jump_h5_domain")]
    public string? InvalidWxaJumpH5Domain { get; set; }
}

/// <summary>
/// 「获取第三方平台业务域名校验文件」响应（官方 <c>get_domain_confirmfile</c>；请求体为空 JSON）。
/// </summary>
/// <remarks>官方返回待放置到域名校验位置的文件名与文件内容；文件内容为文本形态（官方以 JSON 字段承载，非二进制流）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformGetDomainConfirmFileResponse : OpenPlatformResponse
{
    /// <summary>获取或设置校验文件名（官方 <c>file_name</c>）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>获取或设置校验文件内容（官方 <c>file_content</c>；文本）。</summary>
    [JsonPropertyName("file_content")]
    public string? FileContent { get; set; }
}
