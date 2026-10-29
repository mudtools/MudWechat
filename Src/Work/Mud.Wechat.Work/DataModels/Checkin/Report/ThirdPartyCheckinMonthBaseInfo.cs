// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡月报统计基本信息（<c>datas.baseinfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>acctivity_name</c> 官方即此拼写（少了一个 t）；<c>excepion_days_cnt</c> 官方拼写少了一个 t（非 exception_days_cnt），照抄勿改。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinMonthBaseInfo
{
    /// <summary>获取或设置活动名称（官方拼写 acctivity_name 照抄）。</summary>
    [JsonPropertyName("acctivity_name")]
    public string? AcctivityName { get; set; }

    /// <summary>获取或设置日均工作时长（秒）。</summary>
    [JsonPropertyName("avg_work_sec")]
    public int? AvgWorkSec { get; set; }

    /// <summary>获取或设置日均标准工作时长（秒）。</summary>
    [JsonPropertyName("avg_std_work_sec")]
    public int? AvgStdWorkSec { get; set; }

    /// <summary>获取或设置出勤天数。</summary>
    [JsonPropertyName("days")]
    public int? Days { get; set; }

    /// <summary>获取或设置工作日天数。</summary>
    [JsonPropertyName("work_days")]
    public int? WorkDays { get; set; }

    /// <summary>获取或设置异常统计信息。</summary>
    [JsonPropertyName("exception_infos")]
    public List<ThirdPartyCheckinMonthException>? ExceptionInfos { get; set; }

    /// <summary>获取或设置固定上下班出勤天数。</summary>
    [JsonPropertyName("regular_days_cnt")]
    public int? RegularDaysCnt { get; set; }

    /// <summary>获取或设置固定上下班工作总时长（秒）。</summary>
    [JsonPropertyName("regular_work_sec")]
    public int? RegularWorkSec { get; set; }

    /// <summary>获取或设置固定上下班出勤工作日天数。</summary>
    [JsonPropertyName("regular_work_days_cnt")]
    public int? RegularWorkDaysCnt { get; set; }

    /// <summary>获取或设置加班信息（参数表描述为单对象，返回示例为数组，以示例的数组形态承载）。</summary>
    [JsonPropertyName("overwork")]
    public List<ThirdPartyCheckinOverwork>? Overwork { get; set; }

    /// <summary>获取或设置迟到次数。</summary>
    [JsonPropertyName("late_comes_cnt")]
    public int? LateComesCnt { get; set; }

    /// <summary>获取或设置迟到时长（秒）。</summary>
    [JsonPropertyName("late_comes_duration")]
    public int? LateComesDuration { get; set; }

    /// <summary>获取或设置早退次数。</summary>
    [JsonPropertyName("leave_earlies_cnt")]
    public int? LeaveEarliesCnt { get; set; }

    /// <summary>获取或设置早退时长（秒）。</summary>
    [JsonPropertyName("leave_earlies_duration")]
    public int? LeaveEarliesDuration { get; set; }

    /// <summary>获取或设置旷工次数。</summary>
    [JsonPropertyName("absenteeisms_cnt")]
    public int? AbsenteeismsCnt { get; set; }

    /// <summary>获取或设置旷工天数（官方拼写 absenteeisms_cnt/absenteeisms_days_cnt 照抄）。</summary>
    [JsonPropertyName("absenteeisms_days_cnt")]
    public int? AbsenteeismsDaysCnt { get; set; }

    /// <summary>获取或设置缺卡次数。</summary>
    [JsonPropertyName("missed_checks_cnt")]
    public int? MissedChecksCnt { get; set; }

    /// <summary>获取或设置假期信息。</summary>
    [JsonPropertyName("holiday_infos")]
    public List<ThirdPartyCheckinHolidayInfo>? HolidayInfos { get; set; }

    /// <summary>获取或设置加班审批信息（参数表描述为单对象，返回示例为数组，以示例的数组形态承载；与 overwork 结构相同但语义不同：ov_time 是审批口径、overwork 是打卡/核算口径）。</summary>
    [JsonPropertyName("ov_time")]
    public List<ThirdPartyCheckinOverwork>? OvTime { get; set; }
}
