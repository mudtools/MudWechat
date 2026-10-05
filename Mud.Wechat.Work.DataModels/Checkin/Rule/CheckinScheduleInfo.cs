// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则响应侧排班信息（<c>group.schedulelist</c> 元素，只有规则为按班次上下班打卡时才有该配置）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinScheduleInfo
{
    /// <summary>获取或设置班次 id。</summary>
    [JsonPropertyName("schedule_id")]
    public long? ScheduleId { get; set; }

    /// <summary>获取或设置班次名称。</summary>
    [JsonPropertyName("schedule_name")]
    public string? ScheduleName { get; set; }

    /// <summary>获取或设置班次上下班时段信息。</summary>
    [JsonPropertyName("time_section")]
    public List<CheckinTimeSection>? TimeSection { get; set; }

    /// <summary>获取或设置允许提前打卡时间（官方未标注单位，按示例量级同 checkindate.limit_aheadtime 为毫秒）。</summary>
    [JsonPropertyName("limit_aheadtime")]
    public int? LimitAheadtime { get; set; }

    /// <summary>获取或设置下班不需要打卡。</summary>
    [JsonPropertyName("noneed_offwork")]
    public bool? NoneedOffwork { get; set; }

    /// <summary>获取或设置下班 xx 秒后不允许打下班卡。</summary>
    [JsonPropertyName("limit_offtime")]
    public int? LimitOfftime { get; set; }

    /// <summary>获取或设置允许迟到时间（秒；schedulelist 下官方类型标注 uint32，checkindate 下同名标注 int32，同名不同标注照抄）。</summary>
    [JsonPropertyName("flex_on_duty_time")]
    public int? FlexOnDutyTime { get; set; }

    /// <summary>获取或设置允许早退时间（秒；schedulelist 下官方类型标注 uint32，checkindate 下同名标注 int32，同名不同标注照抄）。</summary>
    [JsonPropertyName("flex_off_duty_time")]
    public int? FlexOffDutyTime { get; set; }

    /// <summary>获取或设置是否允许弹性时间（获取员工打卡规则参数表标注 uint32、获取企业所有打卡规则标注 bool，返回示例为 false 布尔形态，本模型以 bool 承载）。</summary>
    [JsonPropertyName("allow_flex")]
    public bool? AllowFlex { get; set; }

    /// <summary>获取或设置晚走晚到时间规则信息。</summary>
    [JsonPropertyName("late_rule")]
    public CheckinLateRule? LateRule { get; set; }

    /// <summary>获取或设置最早可打卡时间限制。</summary>
    [JsonPropertyName("max_allow_arrive_early")]
    public int? MaxAllowArriveEarly { get; set; }

    /// <summary>获取或设置最晚可打卡时间限制（与 max_allow_arrive_early 和 flex_on_duty_time/flex_off_duty_time 互斥，当设置其中一组时另一组数值置 0）。</summary>
    [JsonPropertyName("max_allow_arrive_late")]
    public int? MaxAllowArriveLate { get; set; }
}
