// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 录制附件下载对象（获取录制地址/单个录制文件详情响应 <c>meeting_summary</c> 与 <c>ai_meeting_transcripts</c> 嵌套对象；
/// 两个官方结构同构，共用本类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RecordFileDownload
{
    /// <summary>获取或设置下载文件地址（链接有效期为一个小时）。</summary>
    [JsonPropertyName("download_address")]
    public string? DownloadAddress { get; set; }

    /// <summary>获取或设置下载文件类型（例如：txt、pdf、docx）。</summary>
    [JsonPropertyName("file_type")]
    public string? FileType { get; set; }
}
