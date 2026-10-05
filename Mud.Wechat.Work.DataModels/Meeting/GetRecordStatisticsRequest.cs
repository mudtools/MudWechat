// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取录制文件访问统计请求体（<c>/cgi-bin/meeting/record/get_statistics</c>；获取会议录制 ID 对应的访问数据，按照天维度返回）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetRecordStatisticsRequest
{
    /// <summary>获取或设置会议录制 ID（官方必填）。</summary>
    [JsonPropertyName("meeting_record_id")]
    public string? MeetingRecordId { get; set; }

    /// <summary>获取或设置查询起始时间戳（UNIX 时间戳，单位秒；默认展示最近 31 天的数据，时间区间不允许超过 31 天）。</summary>
    [JsonPropertyName("start_time")]
    public int? StartTime { get; set; }

    /// <summary>获取或设置查询结束时间戳（UNIX 时间戳，单位秒；默认展示最近 31 天的数据，时间区间不允许超过 31 天）。</summary>
    [JsonPropertyName("end_time")]
    public int? EndTime { get; set; }
}
