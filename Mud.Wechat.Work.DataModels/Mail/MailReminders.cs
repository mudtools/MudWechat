// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 日程/会议的重复与提醒相关字段（官方 reminders 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailReminders
{
    /// <summary>获取或设置是否有提醒（官方 is_remind）：0 - 不提醒，1 - 提醒。</summary>
    [JsonPropertyName("is_remind")]
    public int? IsRemind { get; set; }

    /// <summary>
    /// 获取或设置日程/会议开始前后多少分钟提醒（官方 remind_before_event_mins，当 is_remind=1 时有效）。
    /// <para>例如 15 表示开始前 15 分钟提醒，-15 表示开始后 15 分钟提醒。</para>
    /// </summary>
    [JsonPropertyName("remind_before_event_mins")]
    public int? RemindBeforeEventMins { get; set; }

    /// <summary>
    /// 获取或设置时区（官方 timezone，UTC 偏移量表示，东区为正数、西区为负数）。
    /// <para>默认为北京时间东八区，取值范围 -12 ~ +12。</para>
    /// </summary>
    [JsonPropertyName("timezone")]
    public int? Timezone { get; set; }

    /// <summary>获取或设置是否重复（官方 is_repeat）：0 - 否，1 - 是。</summary>
    [JsonPropertyName("is_repeat")]
    public int? IsRepeat { get; set; }

    /// <summary>获取或设置是否自定义重复（官方 is_custom_repeat）：0 - 否，1 - 是；当 is_repeat 为 1 时有效。</summary>
    /// <remarks>
    /// <para>为 0 时系统根据 start_time 和 repeat_type 自动计算重复时间；
    /// 为 1 时可配合 repeat_day_of_week / repeat_day_of_month 指定，并可用 repeat_interval 指定间隔。</para>
    /// </remarks>
    [JsonPropertyName("is_custom_repeat")]
    public int? IsCustomRepeat { get; set; }

    /// <summary>
    /// 获取或设置重复类型（官方 repeat_type，当 is_repeat=1 时有效）：
    /// 0 - 每日，1 - 每周，2 - 每月，5 - 每年。
    /// </summary>
    [JsonPropertyName("repeat_type")]
    public int? RepeatType { get; set; }

    /// <summary>
    /// 获取或设置重复间隔（官方 repeat_interval，仅自定义重复时有效，随 repeat_type 含义不同）。
    /// <para>例如指定 2 且 repeat_type 为每周，则每 2 周一次。</para>
    /// </summary>
    [JsonPropertyName("repeat_interval")]
    public int? RepeatInterval { get; set; }

    /// <summary>获取或设置每周周几重复（官方 repeat_day_of_week，取值 1~7 表示周一至周日；仅自定义重复且类型为每周时有效）。</summary>
    [JsonPropertyName("repeat_day_of_week")]
    public List<int>? RepeatDayOfWeek { get; set; }

    /// <summary>获取或设置每月第几周重复（官方 repeat_week_of_month）。</summary>
    /// <remarks>
    /// <para>
    /// 官方参数表未列出本字段，但官方请求包体示例中携带（空数组形态）；本模型按示例原文承载，
    /// 处理器不应假设官方参数表语义，以官方后续文档更新为准。
    /// </para>
    /// </remarks>
    [JsonPropertyName("repeat_week_of_month")]
    public List<int>? RepeatWeekOfMonth { get; set; }

    /// <summary>获取或设置每月哪几天重复（官方 repeat_day_of_month，取值 1~31；仅自定义重复且类型为每月或每年时有效）。</summary>
    [JsonPropertyName("repeat_day_of_month")]
    public List<int>? RepeatDayOfMonth { get; set; }

    /// <summary>
    /// 获取或设置每年哪几个月重复（官方 repeat_month_of_year，取值 1~12；仅自定义重复且类型为每年时有效）。
    /// <para>每年重复需与 repeat_day_of_month 配合指定某一天。</para>
    /// </summary>
    [JsonPropertyName("repeat_month_of_year")]
    public List<int>? RepeatMonthOfYear { get; set; }

    /// <summary>获取或设置重复结束时刻，Unix 时间戳（官方 repeat_until，当 is_repeat=1 时有效；不填或填 0 表示一直重复）。</summary>
    [JsonPropertyName("repeat_until")]
    public long? RepeatUntil { get; set; }
}
