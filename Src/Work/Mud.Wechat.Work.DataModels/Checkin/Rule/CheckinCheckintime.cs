// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则响应侧上下班打卡时间（<c>checkindate.checkintime</c> / <c>spe_workdays.checkintime</c> 共用元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinCheckintime
{
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
}
