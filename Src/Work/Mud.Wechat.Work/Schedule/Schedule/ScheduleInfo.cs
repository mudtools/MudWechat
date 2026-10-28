// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 创建/更新日程请求的日程信息（<c>/cgi-bin/oa/schedule/add</c> 与 <c>/cgi-bin/oa/schedule/update</c> 共用的扁平 schedule 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 两端点参数表差异以可空性承载：<see cref="CalId"/> 仅创建日程可传（更新日程不可修改所属日历）；
/// <see cref="ScheduleId"/> 仅更新日程必填（创建日程时由响应返回）。更新操作是<b>覆盖式</b>而不是增量式；
/// 创建者与日程所属日历 ID 不可更新。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleInfo
{
    /// <summary>获取或设置日程 ID（仅更新日程必填；创建日程时由响应返回）。</summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    /// <summary>获取或设置所属日历 ID（仅创建日程可传；须为该应用创建的日历，不填写入应用默认日历，第三方应用必须指定；不多于 64 字节）。</summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置日程管理员 userid 列表（须在共享成员列表中，最多指定 3 人）。</summary>
    [JsonPropertyName("admins")]
    public List<string>? Admins { get; set; }

    /// <summary>获取或设置日程参与者列表（累计最多 1000 人）。</summary>
    [JsonPropertyName("attendees")]
    public List<ScheduleAttendee>? Attendees { get; set; }

    /// <summary>获取或设置日程标题（0 ~ 128 字符，不填默认「新建事件」）。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>获取或设置日程描述（不多于 1000 字符）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置提醒相关信息。</summary>
    [JsonPropertyName("reminders")]
    public ScheduleReminders? Reminders { get; set; }

    /// <summary>获取或设置日程地址（不多于 128 字符）。</summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>获取或设置日程开始时间（官方必填；Unix 时间戳；更新重复日程 op_mode 为 1 或 2 时须为 op_start_time 当天或之后）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置日程结束时间（官方必填；Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否全天日程：0-否；1-是（更新日程时表示是否更新成全天日程）。</summary>
    [JsonPropertyName("is_whole_day")]
    public int? IsWholeDay { get; set; }
}
