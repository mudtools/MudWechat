// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.OpenAccount;

/// <summary>
/// 「创建开放平台账号」请求（官方 <c>open/create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAccountCreateRequest
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>appid</c>，必填；待绑定开放平台账号的授权账号）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;
}

/// <summary>
/// 开放平台账号 <c>appid</c> 响应（官方 <c>open/create</c> / <c>open/get</c> 共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAppIdResponse : OpenPlatformResponse
{
    /// <summary>获取或设置开放平台账号 <c>appid</c>（官方 <c>open_appid</c>）。</summary>
    [JsonPropertyName("open_appid")]
    public string? OpenAppId { get; set; }
}

/// <summary>
/// 「获取开放平台账号信息」请求（官方 <c>open/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAccountGetRequest
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;
}

/// <summary>
/// 「绑定开放平台账号」请求（官方 <c>open/bind</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAccountBindRequest
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>获取或设置开放平台账号 <c>appid</c>（官方 <c>open_appid</c>，必填）。</summary>
    [JsonPropertyName("open_appid")]
    public string OpenAppId { get; set; } = string.Empty;
}

/// <summary>
/// 「取消绑定开放平台账号」请求（官方 <c>open/unbind</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAccountUnbindRequest
{
    /// <summary>获取或设置授权方 <c>appid</c>（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>获取或设置开放平台账号 <c>appid</c>（官方 <c>open_appid</c>，必填）。</summary>
    [JsonPropertyName("open_appid")]
    public string OpenAppId { get; set; } = string.Empty;
}

/// <summary>
/// 「检查是否已绑定开放平台账号」响应（官方 <c>open/have</c>；请求体为空 JSON）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAccountHaveResponse : OpenPlatformResponse
{
    /// <summary>获取或设置是否已有开放平台账号（官方 <c>have_open</c>；官方以数字 0/1 承载布尔语义）。</summary>
    [JsonPropertyName("have_open")]
    public int HaveOpen { get; set; }
}

/// <summary>
/// 「检查主体与开放平台账号主体是否一致」响应（官方 <c>open/sameentity</c>；GET、无请求体，<c>appid</c> 走 Query）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenAccount")]
public class OpenPlatformOpenAccountSameEntityResponse : OpenPlatformResponse
{
    /// <summary>获取或设置主体是否一致（官方 <c>same_entity</c>；官方以数字 0/1 承载布尔语义）。</summary>
    [JsonPropertyName("same_entity")]
    public int SameEntity { get; set; }
}
