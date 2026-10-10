// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 创建/更新日程请求的提醒信息（schedule.reminders 对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：<see cref="RemindBeforeEventSecs"/> 仅支持固定档位（0-开始时、300-前 5 分钟、900-前 15 分钟、3600-前 1 小时、86400-前 1 天），
/// 全天日程另支持 32400（当天 09:00）、-172800（前两天）、-604800（前 1 周）；官方建议改用 <see cref="RemindTimeDiffs"/>
///（remind_before_event_secs 后续将会废弃）；<see cref="RemindTimeDiffs"/> 非空时优先，两者仅一个生效。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleReminders
{
    /// <summary>获取或设置是否需要提醒：0-否；1-是。</summary>
    [JsonPropertyName("is_remind")]
    public int? IsRemind { get; set; }

    /// <summary>获取或设置是否重复日程：0-否；1-是。</summary>
    [JsonPropertyName("is_repeat")]
    public int? IsRepeat { get; set; }

    /// <summary>获取或设置日程开始前多少秒提醒（is_remind = 1 时有效；仅支持 0 / 300 / 900 / 3600 / 86400；官方注明该字段后续将会废弃）。</summary>
    [JsonPropertyName("remind_before_event_secs")]
    public int? RemindBeforeEventSecs { get; set; }

    /// <summary>
    /// 获取或设置提醒时间与日程开始时间的差值（is_remind = 1 时有效，可多个；如 -300 表示开始前 5 分钟提醒）。
    /// </summary>
    /// <remarks>
    /// <para>全天日程因 start_time 为 0 点时间戳，可能出现正数 32400（当天 9 点）；取值范围 -604800 ~ 86399；
    /// 本字段非空时优先于 remind_before_event_secs，两者仅一个生效。</para>
    /// </remarks>
    [JsonPropertyName("remind_time_diffs")]
    public List<int>? RemindTimeDiffs { get; set; }

    /// <summary>获取或设置重复类型（is_repeat = 1 时有效）：0-每日；1-每周；2-每月；5-每年；7-工作日。</summary>
    [JsonPropertyName("repeat_type")]
    public int? RepeatType { get; set; }

    /// <summary>获取或设置重复结束时刻（Unix 时间戳；不填或 0 表示一直重复）。</summary>
    [JsonPropertyName("repeat_until")]
    public long? RepeatUntil { get; set; }

    /// <summary>获取或设置是否自定义重复：0-否；1-是（is_repeat = 1 时有效）。</summary>
    /// <remarks>
    /// <para>is_custom_repeat = 0 时系统按 start_time 与 repeat_type 自动推算下次重复时间；
    /// = 1 时可配合 repeat_day_of_week / repeat_day_of_month 指定日期，并可用 repeat_interval 设置间隔。</para>
    /// </remarks>
    [JsonPropertyName("is_custom_repeat")]
    public int? IsCustomRepeat { get; set; }

    /// <summary>获取或设置重复间隔（仅自定义重复有效，含义随 repeat_type 变化，如每周类型 = 3 表示每 3 周一次）。</summary>
    [JsonPropertyName("repeat_interval")]
    public int? RepeatInterval { get; set; }

    /// <summary>获取或设置每周周几重复（仅自定义重复且每周类型有效；1 ~ 7 表示周一至周日）。</summary>
    [JsonPropertyName("repeat_day_of_week")]
    public List<int>? RepeatDayOfWeek { get; set; }

    /// <summary>获取或设置每月哪几天重复（仅自定义重复且每月类型有效；1 ~ 31）。</summary>
    [JsonPropertyName("repeat_day_of_month")]
    public List<int>? RepeatDayOfMonth { get; set; }

    /// <summary>获取或设置时区 UTC 偏移量（东正西负，如 +8 表示东八区；默认东八区，范围 -12 ~ +12）。</summary>
    [JsonPropertyName("timezone")]
    public int? Timezone { get; set; }
}
