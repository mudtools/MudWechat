// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则请求模型上下班打卡时段（创建/修改打卡规则 <c>group.checkindate.checkintime</c> 与
/// <c>group.schedulelist.time_section</c> 共用形态，官方注明二者「定义一致」）。
/// </summary>
/// <remarks>
/// <para>所有时间字段均须为整分钟（60 的倍数）；上下班区间、可打卡时间不可有交集；多组休息时段不可超过 5 组且不可有交集。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRuleCheckintime
{
    /// <summary>获取或设置时段标识（规则内唯一，大于 0，小于 99999）。</summary>
    [JsonPropertyName("time_id")]
    public int? TimeId { get; set; }

    /// <summary>获取或设置上班时间（距 0 点秒数，整分钟；必填，且不可大于次日 00:00 点（可等于））。</summary>
    [JsonPropertyName("work_sec")]
    public int? WorkSec { get; set; }

    /// <summary>获取或设置下班时间（距 0 点秒数，整分钟；必填，且不可大于等于次日 23:59 点；上班时间必须小于下班时间，且间隔时间大于 1 分钟、小于 24 小时）。</summary>
    [JsonPropertyName("off_work_sec")]
    public int? OffWorkSec { get; set; }

    /// <summary>获取或设置上班提醒时间（距 0 点秒数，整分钟；不可晚于上班时间，且不可早于上班 20 分钟；只可以为准点、提前 5/10/15/20 分钟）。</summary>
    [JsonPropertyName("remind_work_sec")]
    public int? RemindWorkSec { get; set; }

    /// <summary>获取或设置下班提醒时间（距 0 点秒数，整分钟；不可早于下班时间，且不可晚于下班 60 分钟；只可以为准点、下班后 10/20/30/60 分钟）。</summary>
    [JsonPropertyName("remind_off_work_sec")]
    public int? RemindOffWorkSec { get; set; }

    /// <summary>获取或设置是否需要休息（默认为 false；设置休息时间字段时需要将 allow_rest 设置为 true）。</summary>
    [JsonPropertyName("allow_rest")]
    public bool? AllowRest { get; set; }

    /// <summary>获取或设置休息开始时间（allow_rest 为 true 时需要填写；距 0 点秒数）。</summary>
    [JsonPropertyName("rest_begin_time")]
    public int? RestBeginTime { get; set; }

    /// <summary>获取或设置休息截止时间（allow_rest 为 true 时需要填写；距 0 点秒数）。</summary>
    [JsonPropertyName("rest_end_time")]
    public int? RestEndTime { get; set; }

    /// <summary>获取或设置上班最早可打卡时间（距 0 点秒数，整分钟；固定上下班场景不可小于 0、不可大于次日 00:00；排班场景第一个时段不可早于上班时间 4 小时以上）。</summary>
    [JsonPropertyName("earliest_work_sec")]
    public int? EarliestWorkSec { get; set; }

    /// <summary>获取或设置上班最晚可打卡时间（距 0 点秒数，整分钟；排班场景需要是 60 的倍数）。</summary>
    [JsonPropertyName("latest_work_sec")]
    public int? LatestWorkSec { get; set; }

    /// <summary>获取或设置下班最早可打卡时间（距 0 点秒数，整分钟；固定上下班场景不可大于次日 04:00）。</summary>
    [JsonPropertyName("earliest_off_work_sec")]
    public int? EarliestOffWorkSec { get; set; }

    /// <summary>获取或设置下班最晚可打卡时间（距 0 点秒数，整分钟；固定上下班场景不可大于次日 04:00）。</summary>
    [JsonPropertyName("latest_off_work_sec")]
    public int? LatestOffWorkSec { get; set; }

    /// <summary>获取或设置不需要打上班卡（默认需要打卡）。</summary>
    [JsonPropertyName("no_need_checkon")]
    public bool? NoNeedCheckon { get; set; }

    /// <summary>获取或设置不需要打下班卡（默认需要打卡）。</summary>
    [JsonPropertyName("no_need_checkoff")]
    public bool? NoNeedCheckoff { get; set; }

    /// <summary>获取或设置多组休息时间（有值时需要 allow_rest 为 true；多组休息时段不可超过 5 组且不可有交集）。</summary>
    [JsonPropertyName("rest_times")]
    public List<CheckinRestTime>? RestTimes { get; set; }
}
