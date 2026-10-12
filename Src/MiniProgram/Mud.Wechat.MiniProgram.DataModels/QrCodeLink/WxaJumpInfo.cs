// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

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
