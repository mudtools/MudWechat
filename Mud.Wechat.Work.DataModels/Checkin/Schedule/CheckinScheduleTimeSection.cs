// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 排班班次上下班时段（<c>schedule_info.time_section</c> 元素；时段 id 字段名为 id，区别于打卡规则中的 time_id）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinScheduleTimeSection
{
    /// <summary>获取或设置时段 id（为班次中某一堆上下班时间组合的 id；字段名为 id，区别于打卡规则响应中的 time_id）。</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>获取或设置上班时间（距当天 00:00 的秒数）。</summary>
    [JsonPropertyName("work_sec")]
    public int? WorkSec { get; set; }

    /// <summary>获取或设置下班时间（距当天 00:00 的秒数）。</summary>
    [JsonPropertyName("off_work_sec")]
    public int? OffWorkSec { get; set; }

    /// <summary>获取或设置上班提醒时间（距当天 00:00 的秒数）。</summary>
    [JsonPropertyName("remind_work_sec")]
    public int? RemindWorkSec { get; set; }

    /// <summary>获取或设置下班提醒时间（距当天 00:00 的秒数）。</summary>
    [JsonPropertyName("remind_off_work_sec")]
    public int? RemindOffWorkSec { get; set; }
}
