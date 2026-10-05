// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议录制列表请求体（<c>/cgi-bin/meeting/record/list</c>；获取云录制记录，根据成员 ID、会议 ID、会议 code 进行查询，
/// 支持根据时间区间分页获取；如果使用成员 ID 进行查询，可以获取到该用户作为会议创建者的会议录制列表）。
/// </summary>
/// <remarks>
/// <para>官方文档陷阱：官方响应示例将录制列表字段误写为 <c>record_meetings</c>、录制文件列表字段误写为 <c>record_files</c>，参数表为
/// <c>record_list</c> 与 <c>record_file_list</c>，本模型以参数表为准。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ListMeetingRecordsRequest
{
    /// <summary>获取或设置会议 ID（不为空时优先根据会议 ID 查询；meetingid / meeting_code / userid 三者选填其中一项）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置会议 code（当 meetingid 为空且 meeting_code 不为空时根据会议 code 查询；当两者均为空时，表示查询成员所有会议的录制列表）。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置待查询成员的 userid（meetingid / meeting_code / userid 三者选填其中一项）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置查询起始时间戳（官方必填，UNIX 时间戳，单位秒；查询时间区间跨度不允许超过 31 天）。</summary>
    [JsonPropertyName("start_time")]
    public int? StartTime { get; set; }

    /// <summary>获取或设置查询结束时间戳（官方必填，UNIX 时间戳，单位秒；查询时间区间跨度不允许超过 31 天）。</summary>
    [JsonPropertyName("end_time")]
    public int? EndTime { get; set; }

    /// <summary>获取或设置分页查询游标（上一次调用时返回的 <c>next_cursor</c>，初次调用可不传）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置单次查询返回的最大数量限制（默认为 10，最大 20）。
    /// <para>官方限制：<c>limit</c> 参数必须与首次调用获得 <c>cursor</c> 时传入的 limit 一致。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
