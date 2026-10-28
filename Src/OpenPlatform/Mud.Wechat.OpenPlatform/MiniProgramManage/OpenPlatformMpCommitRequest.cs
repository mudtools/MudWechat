// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「提交代码」（<c>commit</c>）请求：把模板库中的代码提交到授权方小程序体验版。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/commit.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpCommitRequest
{
    /// <summary>代码模板 ID（官方必填 <c>template_id</c>，number）。</summary>
    [JsonPropertyName("template_id")]
    public long? TemplateId { get; set; }

    /// <summary>小程序额外的配置（官方必填 <c>ext_json</c>，string，JSON 字符串原样透传）。</summary>
    [JsonPropertyName("ext_json")]
    public string? ExtJson { get; set; }

    /// <summary>用户自定义版本号（官方选填 <c>user_version</c>，string）。</summary>
    [JsonPropertyName("user_version")]
    public string? UserVersion { get; set; }

    /// <summary>用户自定义版本描述（官方选填 <c>user_desc</c>，string）。</summary>
    [JsonPropertyName("user_desc")]
    public string? UserDesc { get; set; }
}
