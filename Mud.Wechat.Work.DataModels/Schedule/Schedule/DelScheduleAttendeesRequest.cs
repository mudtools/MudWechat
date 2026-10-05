// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 删除日程参与者请求体（<c>/cgi-bin/oa/schedule/del_attendees</c>）。
/// </summary>
/// <remarks>
/// <para>本端点为增量式请求方式（仅移除指定参与者，不覆盖其余参与者）；参与者列表最多可添加 1000 人。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class DelScheduleAttendeesRequest
{
    /// <summary>获取或设置日程 ID（官方必填；创建日程时返回的 ID）。</summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    /// <summary>获取或设置日程参与者列表（最多可添加 1000 人）。</summary>
    [JsonPropertyName("attendees")]
    public List<ScheduleAttendee>? Attendees { get; set; }
}
