// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 用户 PC 端登录请求（<c>POST /wedata/wedata_login</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/wedata_login.html"/></para>
/// <para>官方约束：须先完成权限集与登录配置设置；<c>client_ip</c>/<c>user_agent</c> 属用户环境信息，<b>不得</b>落日志。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataLoginRequest
{
    /// <summary>用户在服务商系统的唯一标识（官方 <c>uid</c>，必填）。</summary>
    [JsonPropertyName("uid")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>用户在服务商系统的会话标识（官方 <c>user_session</c>，必填）。</summary>
    [JsonPropertyName("user_session")]
    public string UserSession { get; set; } = string.Empty;

    /// <summary>用户 IP（官方 <c>client_ip</c>）。</summary>
    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }

    /// <summary>用户 User-Agent（官方 <c>user_agent</c>）。</summary>
    [JsonPropertyName("user_agent")]
    public string? UserAgent { get; set; }
}
