// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡日报汇总信息（<c>datas.summary_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：<c>earliest_time</c>/<c>lastest_time</c>（注意 lastest 为官方拼写）是距离 0 点的秒数（如 38827），不是 Unix 时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinDaySummaryInfo
{
    /// <summary>获取或设置当日打卡次数。</summary>
    [JsonPropertyName("checkin_count")]
    public int? CheckinCount { get; set; }

    /// <summary>获取或设置当日实际工作时长（单位：秒）。</summary>
    [JsonPropertyName("regular_work_sec")]
    public int? RegularWorkSec { get; set; }

    /// <summary>获取或设置当日标准工作时长（单位：秒）。</summary>
    [JsonPropertyName("standard_work_sec")]
    public int? StandardWorkSec { get; set; }

    /// <summary>获取或设置当日最早打卡时间（距离 0 点的秒数，非 Unix 时间戳）。</summary>
    [JsonPropertyName("earliest_time")]
    public int? EarliestTime { get; set; }

    /// <summary>获取或设置当日最晚打卡时间（距离 0 点的秒数，非 Unix 时间戳；官方拼写 lastest 照抄）。</summary>
    [JsonPropertyName("lastest_time")]
    public int? LastestTime { get; set; }
}
