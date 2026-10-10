// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 个人当日排班信息（<c>schedule.scheduleList.schedule_info</c>）。
/// </summary>
/// <remarks>
/// <para>schedule_id=0 且 schedule_name=「休息」表示休息日，此时 time_section 为空数组。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinDayScheduleInfo
{
    /// <summary>获取或设置当日安排班次 id（班次 id 也可在打卡规则中查询获得；0 且名称为「休息」表示休息日）。</summary>
    [JsonPropertyName("schedule_id")]
    public long? ScheduleId { get; set; }

    /// <summary>获取或设置班次名称。</summary>
    [JsonPropertyName("schedule_name")]
    public string? ScheduleName { get; set; }

    /// <summary>获取或设置班次上下班时段信息（休息日为空数组）。</summary>
    [JsonPropertyName("time_section")]
    public List<CheckinScheduleTimeSection>? TimeSection { get; set; }
}
