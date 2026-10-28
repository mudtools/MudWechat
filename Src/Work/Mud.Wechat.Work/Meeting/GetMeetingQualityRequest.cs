// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议健康度请求体（<c>/cgi-bin/meeting/get_quality</c>；获取已结束会议的会议及参会成员的健康度）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingQualityRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置周期性子会议 ID。
    /// <para>可通过查询成员的会议列表、查询会议接口获取返回的子会议 ID（即 <c>current_sub_meetingid</c>）；如果是周期性会议，此参数必传。</para>
    /// </summary>
    [JsonPropertyName("sub_meetingid")]
    public string? SubMeetingid { get; set; }

    /// <summary>
    /// 获取或设置参会时间过滤起始时间（官方必填，UNIX 时间戳，单位秒）。
    /// <para>
    /// 官方限制：可查询的时间区间为过去 7 天到现在；返回 meeting_id 对应会议房间下开始时间大于等于 start_time
    /// 且离 start_time 最近的一个媒体房间数据（从第一个人入会到会中成员全部离开会议形成一个媒体房间，
    /// 若同一会议号下再次有人入会则形成新的媒体房间）。
    /// </para>
    /// </summary>
    [JsonPropertyName("start_time")]
    public int? StartTime { get; set; }

    /// <summary>获取或设置分页查询游标（将上一个请求返回的 <c>next_cursor</c> 字段传入；第一次查询时可不传值）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页大小（默认 50，最大 50）。
    /// <para>官方限制：<c>limit</c> 参数必须与首次调用获得 <c>cursor</c> 时传入的 limit 一致。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
