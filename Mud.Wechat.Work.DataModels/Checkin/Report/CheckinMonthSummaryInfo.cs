// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡月报汇总信息（<c>datas.summary_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：<c>regular_days</c>（正常天数）只在官方参数表中出现，返回示例未包含，本模型可空承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinMonthSummaryInfo
{
    /// <summary>获取或设置应出勤天数。</summary>
    [JsonPropertyName("work_days")]
    public int? WorkDays { get; set; }

    /// <summary>获取或设置正常天数（仅官方参数表列出，返回示例未包含）。</summary>
    [JsonPropertyName("regular_days")]
    public int? RegularDays { get; set; }

    /// <summary>获取或设置休息天数。</summary>
    [JsonPropertyName("rest_days")]
    public int? RestDays { get; set; }

    /// <summary>获取或设置异常天数。</summary>
    [JsonPropertyName("except_days")]
    public int? ExceptDays { get; set; }

    /// <summary>获取或设置实际工作时长（为统计周期每日实际工作时长之和，单位：秒）。</summary>
    [JsonPropertyName("regular_work_sec")]
    public int? RegularWorkSec { get; set; }

    /// <summary>获取或设置标准工作时长（为统计周期每日标准工作时长之和，单位：秒）。</summary>
    [JsonPropertyName("standard_work_sec")]
    public int? StandardWorkSec { get; set; }
}
