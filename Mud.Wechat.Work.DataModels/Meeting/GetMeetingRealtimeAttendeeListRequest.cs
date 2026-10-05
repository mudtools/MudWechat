// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取实时会中成员列表请求体（<c>/cgi-bin/meeting/get_realtime_attendee_list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：官方请求示例将分页游标字段误写为 <c>cursort</c>，参数表为 <c>cursor</c>，本模型以参数表为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingRealtimeAttendeeListRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置周期性会议子会议 ID。
    /// <para>可通过查询成员的会议列表、查询会议接口获取返回的子会议 ID（即 <c>current_sub_meetingid</c>）；如果是周期性会议，此参数必传。</para>
    /// </summary>
    [JsonPropertyName("sub_meetingid")]
    public string? SubMeetingid { get; set; }

    /// <summary>获取或设置分页查询游标（将上一个请求返回的 <c>next_cursor</c> 字段传入；第一次查询时可不传值）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置单次查询返回的数量（最大 50 条）。
    /// <para>官方限制：<c>limit</c> 参数必须与首次调用获得 <c>cursor</c> 时传入的 limit 一致。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
