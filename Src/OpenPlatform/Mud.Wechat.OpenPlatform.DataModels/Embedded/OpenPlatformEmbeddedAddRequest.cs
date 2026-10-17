// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Embedded;

/// <summary>
/// 「添加半屏小程序」请求（<c>add_embedded</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/addEmbedded.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Embedded")]
public class OpenPlatformEmbeddedAddRequest
{
    /// <summary>被添加的半屏小程序 appid（官方 snake_case 全小写，不得驼峰化）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>申请理由（官方字段名 <c>apply_reason</c>）。</summary>
    [JsonPropertyName("apply_reason")]
    public string? ApplyReason { get; set; }
}
