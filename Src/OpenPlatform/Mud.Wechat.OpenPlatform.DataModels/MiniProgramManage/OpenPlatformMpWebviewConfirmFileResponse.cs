// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取 webview 域名校验文件」应答（<c>get_webviewdomain_confirmfile</c>，官方为 POST、无请求体）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getJumpDomainConfirmFile.html"/>。
/// </summary>
/// <remarks>须将 <see cref="FileName"/>/<see cref="FileContent"/> 原样放置于域名根目录，<b>不得修改文件名与内容</b>，否则 <c>setwebviewdomain_directly</c> 校验失败（86103）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpWebviewConfirmFileResponse : OpenPlatformResponse
{
    /// <summary>校验文件名（官方 <c>file_name</c>，string，不得改名）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>校验文件内容（官方 <c>file_content</c>，string，不得改内容）。</summary>
    [JsonPropertyName("file_content")]
    public string? FileContent { get; set; }
}
