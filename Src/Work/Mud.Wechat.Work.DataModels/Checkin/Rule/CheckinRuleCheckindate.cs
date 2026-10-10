// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则请求模型打卡时间配置（创建/修改打卡规则 <c>group.checkindate</c> 元素，「固定时间上下班」类型必填）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRuleCheckindate
{
    /// <summary>获取或设置工作日（固定时间上下班或自由上下班：1 到 6 分别表示星期一到星期六，0 表示星期日；开启大小周的固定上下班规则返回空或不返回；按班次上下班：表示拉取班次的日期。workdays 需在 0~6 之间，个数不超过 7 个，且规则内不可重复）。</summary>
    [JsonPropertyName("workdays")]
    public List<int>? Workdays { get; set; }

    /// <summary>获取或设置工作日上下班打卡时间信息（打卡时段个数需要大于 0 且不超过 4）。</summary>
    [JsonPropertyName("checkintime")]
    public List<CheckinRuleCheckintime>? Checkintime { get; set; }

    /// <summary>获取或设置弹性时间（晚于上班/早于下班不算迟到，单位 ms；官方注明为旧字段，openapi 创建和修改规则时无法使用该字段）。</summary>
    [JsonPropertyName("flex_time")]
    public int? FlexTime { get; set; }

    /// <summary>获取或设置是否允许弹性时间（默认为 false；开启弹性打卡时，需要设置【迟到早退】或者【早到早走晚到晚走】其中一项）。</summary>
    [JsonPropertyName("allow_flex")]
    public bool? AllowFlex { get; set; }

    /// <summary>获取或设置允许迟到时间（单位：秒，不超过 360 分钟；有值时需要 allow_flex 为 true；与 max_allow_arrive_early/max_allow_arrive_late 互斥，设置其中一组时另一组数值置 0）。</summary>
    [JsonPropertyName("flex_on_duty_time")]
    public int? FlexOnDutyTime { get; set; }

    /// <summary>获取或设置允许早退时间（单位：秒，不超过 360 分钟；有值时需要 allow_flex 为 true）。</summary>
    [JsonPropertyName("flex_off_duty_time")]
    public int? FlexOffDutyTime { get; set; }

    /// <summary>获取或设置当日允许早到早走最大时间（单位：秒；有值时需要 allow_flex 为 true）。</summary>
    [JsonPropertyName("max_allow_arrive_early")]
    public int? MaxAllowArriveEarly { get; set; }

    /// <summary>获取或设置当日允许晚到晚走最大时间（单位：秒；有值时需要 allow_flex 为 true；与 flex_on_duty_time/flex_off_duty_time 互斥）。</summary>
    [JsonPropertyName("max_allow_arrive_late")]
    public int? MaxAllowArriveLate { get; set; }

    /// <summary>获取或设置晚走晚到时间规则信息（有值时需要 allow_flex 为 true；设置晚走晚到需要开启 allow_offwork_after_time）。</summary>
    [JsonPropertyName("late_rule")]
    public CheckinRuleLateRule? LateRule { get; set; }

    /// <summary>获取或设置大小周配置（固定上下班规则时有效）。</summary>
    [JsonPropertyName("biweekly")]
    public CheckinBiweekly? Biweekly { get; set; }
}
