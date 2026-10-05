// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取录制转写详情请求体（<c>/cgi-bin/meeting/record/transcript/get_detail</c>；获取云录制转写的详情，包含时间戳、文本等内容）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetRecordTranscriptDetailRequest
{
    /// <summary>获取或设置录制文件 ID（官方必填）。</summary>
    [JsonPropertyName("record_file_id")]
    public string? RecordFileId { get; set; }

    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置查询的起始段落 ID（获取 pid 后（含）的段落，默认从 0 开始）。</summary>
    [JsonPropertyName("pid")]
    public string? Pid { get; set; }

    /// <summary>获取或设置查询的段落数（默认查询全量数据）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
