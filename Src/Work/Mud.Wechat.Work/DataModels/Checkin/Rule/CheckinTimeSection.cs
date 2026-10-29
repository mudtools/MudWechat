// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则响应侧班次上下班时段（<c>group.schedulelist.time_section</c> 元素；区别于请求侧时段，本响应形态含最早一组休息时间平铺字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinTimeSection
{
    /// <summary>获取或设置时段 id（为班次中某一堆上下班时间组合的 id）。</summary>
    [JsonPropertyName("time_id")]
    public int? TimeId { get; set; }

    /// <summary>获取或设置上班时间（表示为距离当天 0 点的秒数）。</summary>
    [JsonPropertyName("work_sec")]
    public int? WorkSec { get; set; }

    /// <summary>获取或设置下班时间（表示为距离当天 0 点的秒数）。</summary>
    [JsonPropertyName("off_work_sec")]
    public int? OffWorkSec { get; set; }

    /// <summary>获取或设置上班提醒时间（表示为距离当天 0 点的秒数）。</summary>
    [JsonPropertyName("remind_work_sec")]
    public int? RemindWorkSec { get; set; }

    /// <summary>获取或设置下班提醒时间（表示为距离当天 0 点的秒数）。</summary>
    [JsonPropertyName("remind_off_work_sec")]
    public int? RemindOffWorkSec { get; set; }

    /// <summary>获取或设置最早一组休息开始时间（距离 0 点的秒）。</summary>
    [JsonPropertyName("rest_begin_time")]
    public int? RestBeginTime { get; set; }

    /// <summary>获取或设置最早一组休息结束时间（距离 0 点的秒）。</summary>
    [JsonPropertyName("rest_end_time")]
    public int? RestEndTime { get; set; }

    /// <summary>获取或设置是否允许休息。</summary>
    [JsonPropertyName("allow_rest")]
    public bool? AllowRest { get; set; }

    /// <summary>获取或设置多组休息时间（不包括最早的一组休息时间）。</summary>
    [JsonPropertyName("rest_times")]
    public List<CheckinRestTime>? RestTimes { get; set; }
}
