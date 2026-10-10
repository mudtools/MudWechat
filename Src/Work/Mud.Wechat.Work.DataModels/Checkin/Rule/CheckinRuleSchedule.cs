// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则请求模型排班配置（创建/修改打卡规则 <c>group.schedulelist</c> 元素，「按班次上下班」类型必填）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRuleSchedule
{
    /// <summary>获取或设置班次 id（规则内唯一；必须存在 schedule_id=0 的排班，班次名称为「休息」，且不能有 checkintime 字段）。</summary>
    [JsonPropertyName("schedule_id")]
    public long? ScheduleId { get; set; }

    /// <summary>获取或设置班次名称（规则内唯一；不可为空且字符个数不可超过 40 个；schedule_id=0 的休息排班必须为「休息」）。</summary>
    [JsonPropertyName("schedule_name")]
    public string? ScheduleName { get; set; }

    /// <summary>获取或设置班次上下班时段信息（时段个数需要介于 0~4 之间；schedule_id=0 的休息排班不可设置 time_section）。</summary>
    [JsonPropertyName("time_section")]
    public List<CheckinRuleCheckintime>? TimeSection { get; set; }

    /// <summary>获取或设置是否允许弹性时间（默认为 true；开启弹性打卡时，需要设置【迟到早退】或者【早到早走晚到晚走】其中一项）。</summary>
    [JsonPropertyName("allow_flex")]
    public bool? AllowFlex { get; set; }

    /// <summary>获取或设置允许迟到时间（单位：秒，不超过 360 分钟；有值时需要 allow_flex 为 true）。</summary>
    [JsonPropertyName("flex_on_duty_time")]
    public int? FlexOnDutyTime { get; set; }

    /// <summary>获取或设置允许早退时间（单位：秒，不超过 360 分钟；有值时需要 allow_flex 为 true）。</summary>
    [JsonPropertyName("flex_off_duty_time")]
    public int? FlexOffDutyTime { get; set; }

    /// <summary>获取或设置晚走晚到时间规则信息（有值时需要 allow_flex 为 true）。</summary>
    [JsonPropertyName("late_rule")]
    public CheckinRuleLateRule? LateRule { get; set; }

    /// <summary>获取或设置当日允许早到早走最大时间（单位：秒；有值时需要 allow_flex 为 true）。</summary>
    [JsonPropertyName("max_allow_arrive_early")]
    public int? MaxAllowArriveEarly { get; set; }

    /// <summary>获取或设置当日允许晚到晚走最大时间（单位：秒；有值时需要 allow_flex 为 true；与 flex_on_duty_time/flex_off_duty_time 互斥）。</summary>
    [JsonPropertyName("max_allow_arrive_late")]
    public int? MaxAllowArriveLate { get; set; }
}
