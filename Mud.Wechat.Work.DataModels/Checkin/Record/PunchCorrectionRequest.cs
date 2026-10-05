// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 为打卡人员补卡请求体（<c>/cgi-bin/checkin/punch_correction</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：审批中的审批打卡不能补卡，补卡后原审批打卡的信息会清除；备注不超过 512 字节。
/// <c>schedule_checkin_time</c> 为相对打卡日期 0 点的偏移秒数（如 9 点整为 32400），与 <c>schedule_date_time</c>/<c>checkin_time</c>
/// 的 Unix 时间戳形态并存；无规则对应的打卡时间点（休息日打卡、无规则打卡、自由上下班）不传本字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class PunchCorrectionRequest
{
    /// <summary>获取或设置需要补卡的成员 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置应打卡日期（当天 0 点的 Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("schedule_date_time")]
    public long? ScheduleDateTime { get; set; }

    /// <summary>获取或设置应打卡时间点（相对打卡日期 0 点的偏移秒数，如 9 点整为 32400；可通过获取员工打卡规则获取对应的规则打卡时间点，如 work_sec/off_work_sec；对于没有规则对应的打卡时间点（休息日打卡、无规则打卡、自由上下班）不用填）。</summary>
    [JsonPropertyName("schedule_checkin_time")]
    public int? ScheduleCheckinTime { get; set; }

    /// <summary>获取或设置实际打卡时间（Unix 时间戳，官方必填；相对于 schedule_checkin_time 的实际打卡时间，具体可以表现为正常/迟到/早退）。</summary>
    [JsonPropertyName("checkin_time")]
    public long? CheckinTime { get; set; }

    /// <summary>获取或设置备注信息（不超过 512 字节）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}
