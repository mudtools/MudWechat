// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 小程序认证提审应答（<c>wxaauth</c> 与 <c>reauth</c> 两支端点共用；官方两页应答表逐字段相同）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthSubmitResponse : OpenPlatformResponse
{
    /// <summary>认证任务号（官方 <c>taskid</c>，string）。</summary>
    [JsonPropertyName("taskid")]
    public string? TaskId { get; set; }

    /// <summary>认证跳转链接（官方 <c>auth_url</c>，string）。</summary>
    [JsonPropertyName("auth_url")]
    public string? AuthUrl { get; set; }
}
