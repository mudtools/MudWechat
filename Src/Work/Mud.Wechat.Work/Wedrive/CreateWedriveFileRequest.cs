// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 新建文件夹/文档请求体（<c>/cgi-bin/wedrive/file_create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class CreateWedriveFileRequest
{
    /// <summary>获取或设置空间 spaceid（官方必填）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置父目录 fileid（官方必填；根目录时为空间 spaceid）。</summary>
    [JsonPropertyName("fatherid")]
    public string? Fatherid { get; set; }

    /// <summary>获取或设置文件类型（官方必填）：1 - 文件夹；3 - 文档(文档)；4 - 文档(表格)。</summary>
    [JsonPropertyName("file_type")]
    public ulong? FileType { get; set; }

    /// <summary>获取或设置文件名字（官方必填；最多 255 个字符，英文算 1 个，汉字算 2 个）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }
}
