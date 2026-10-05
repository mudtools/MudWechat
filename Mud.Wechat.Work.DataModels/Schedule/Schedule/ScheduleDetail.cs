// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 日程详情（获取日程详情与获取日历下的日程列表响应 schedule_list 元素）。
/// </summary>
/// <remarks>
/// <para>被取消的日程也可以拉取详情，调用者需检查 <see cref="Status"/> 字段。
/// 两端点字段差异以可空性承载：<see cref="IsWholeDay"/> 由获取日程详情响应返回；
/// <see cref="Sequence"/> 由获取日历下的日程列表响应返回。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleDetail
{
    /// <summary>获取或设置日程 ID。</summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    /// <summary>获取或设置日程管理员 userid 列表（最多 3 人）。</summary>
    [JsonPropertyName("admins")]
    public List<string>? Admins { get; set; }

    /// <summary>获取或设置日程参与者列表（获取日历下的日程列表响应注明最多支持 300 人）。</summary>
    [JsonPropertyName("attendees")]
    public List<ScheduleAttendee>? Attendees { get; set; }

    /// <summary>获取或设置日程标题。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>获取或设置日程描述。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置提醒相关信息（含重复日程排除日期 exclude_time_list）。</summary>
    [JsonPropertyName("reminders")]
    public ScheduleRemindersInfo? Reminders { get; set; }

    /// <summary>获取或设置日程地址（不多于 128 字符）。</summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>获取或设置日程状态：0-正常；1-已取消。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置日程开始时间（Unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置日程结束时间（Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否全天日程：0-否；1-是（获取日程详情响应返回）。</summary>
    [JsonPropertyName("is_whole_day")]
    public int? IsWholeDay { get; set; }

    /// <summary>获取或设置所属日历 ID（不多于 64 字节）。</summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置日程编号（自增数字；获取日历下的日程列表响应返回）。</summary>
    [JsonPropertyName("sequence")]
    public long? Sequence { get; set; }
}
