// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则加班配置 V2 单组配置（<c>ot_info_v2.workdayconf</c> / <c>restdayconf</c> / <c>holidayconf</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtConf
{
    /// <summary>获取或设置是否允许加班（官方必填）。</summary>
    [JsonPropertyName("allow_ot")]
    public bool? AllowOt { get; set; }

    /// <summary>获取或设置加班核算类型（官方必填）：0 - 以加班申请和打卡时间取交集；1 - 以打卡时间为准；2 - 只以审批单为准（默认为 2；type 不可超过 2）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置只以审批为准时的配置（type 为 2 时有意义）。</summary>
    [JsonPropertyName("apply")]
    public CheckinOtApplyConf? Apply { get; set; }

    /// <summary>获取或设置只以打卡时间为准的配置（type 为 1 时有意义）。</summary>
    [JsonPropertyName("checkin")]
    public CheckinOtCheckinConf? Checkin { get; set; }

    /// <summary>获取或设置审批和打卡取交集配置（type 为 0 时有意义）。</summary>
    [JsonPropertyName("applycheckin")]
    public CheckinOtApplyCheckinConf? Applycheckin { get; set; }

    /// <summary>获取或设置允许加班时长记为调休和加班费（默认为 false）。</summary>
    [JsonPropertyName("ot_trans_enable")]
    public bool? OtTransEnable { get; set; }

    /// <summary>获取或设置加班转化类型：1 - 记为调休；2 - 记为加班费（默认为 1；不可超过 2）。</summary>
    [JsonPropertyName("ot_trans_type")]
    public int? OtTransType { get; set; }

    /// <summary>获取或设置同步假期信息（记为调休）。</summary>
    [JsonPropertyName("vacation")]
    public CheckinOtVacation? Vacation { get; set; }

    /// <summary>获取或设置加班时段（默认为 0）：0 - 全天；1 - 仅允许上班前；2 - 仅允许下班后。</summary>
    [JsonPropertyName("ot_time_range")]
    public int? OtTimeRange { get; set; }
}
