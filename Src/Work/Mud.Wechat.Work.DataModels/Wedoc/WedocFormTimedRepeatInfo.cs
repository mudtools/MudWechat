// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表的定时重复设置项（官方 <c>timed_repeat_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>week_flag</c> 仅 <c>repeat_type</c> 为 0（每周）时可填、<c>skip_holiday</c> 仅 1（每天）时可填、
/// <c>day_of_month</c> 仅 2（每月）时可填；开启定时重复时 <c>fill_in_range</c> 必填；
/// <c>rule_ctime</c> / <c>rule_mtime</c> 仅在获取收集表信息响应中返回。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormTimedRepeatInfo
{
    /// <summary>获取或设置是否开启定时重复（官方 <c>enable</c>）。</summary>
    [JsonPropertyName("enable")]
    public bool? Enable { get; set; }

    /// <summary>获取或设置第一次提醒的时间戳（官方 <c>remind_time</c>），重复提醒按相关字段计算。</summary>
    [JsonPropertyName("remind_time")]
    public uint? RemindTime { get; set; }

    /// <summary>
    /// 获取或设置重复类型（官方 <c>repeat_type</c>）。
    /// 官方取值：<c>0</c> 每周、<c>1</c> 每天、<c>2</c> 每月。
    /// </summary>
    [JsonPropertyName("repeat_type")]
    public uint? RepeatType { get; set; }

    /// <summary>
    /// 获取或设置每周几重复（官方 <c>week_flag</c>，bit 组合），仅 <c>repeat_type</c> 为 0 时可填。
    /// 官方取值：<c>1</c> 星期一、<c>2</c> 星期二、<c>4</c> 星期三、<c>8</c> 星期四、<c>16</c> 星期五、<c>32</c> 星期六、<c>64</c> 星期日。
    /// </summary>
    [JsonPropertyName("week_flag")]
    public uint? WeekFlag { get; set; }

    /// <summary>获取或设置自动跳过节假日（官方 <c>skip_holiday</c>），仅 <c>repeat_type</c> 为 1 时可填。</summary>
    [JsonPropertyName("skip_holiday")]
    public bool? SkipHoliday { get; set; }

    /// <summary>获取或设置每月的第几天（官方 <c>day_of_month</c>，取值 1-31），仅 <c>repeat_type</c> 为 2 时可填。</summary>
    [JsonPropertyName("day_of_month")]
    public uint? DayOfMonth { get; set; }

    /// <summary>
    /// 获取或设置是否允许补填（官方 <c>fork_finish_type</c>）。
    /// 官方取值：<c>0</c> 允许、<c>1</c> 仅当天、<c>2</c> 最后五天内、<c>3</c> 一个月内、<c>4</c> 下一次生成前。
    /// </summary>
    [JsonPropertyName("fork_finish_type")]
    public uint? ForkFinishType { get; set; }

    /// <summary>获取或设置定时重复规则创建时间戳（官方 <c>rule_ctime</c>），仅响应中返回。</summary>
    [JsonPropertyName("rule_ctime")]
    public ulong? RuleCtime { get; set; }

    /// <summary>获取或设置定时重复规则修改时间戳（官方 <c>rule_mtime</c>），仅响应中返回。</summary>
    [JsonPropertyName("rule_mtime")]
    public ulong? RuleMtime { get; set; }
}
