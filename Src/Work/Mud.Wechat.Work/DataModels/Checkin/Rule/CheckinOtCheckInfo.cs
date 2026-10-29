// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 企业打卡规则加班「以打卡时间为准」时长计算规则（获取企业所有打卡规则响应 <c>group.ot_info.otcheckinfo</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtCheckInfo
{
    /// <summary>获取或设置允许工作日加班 - 加班开始时间（下班后 xx 秒开始计算加班，距离最晚下班时间的秒数，例如 1800 表示 30 分钟；默认值 30 分钟）。</summary>
    [JsonPropertyName("ot_workingday_time_start")]
    public int? OtWorkingdayTimeStart { get; set; }

    /// <summary>获取或设置允许工作日加班 - 最短加班时长（不足 xx 秒视为未加班，单位秒；默认值 30 分钟）。</summary>
    [JsonPropertyName("ot_workingday_time_min")]
    public int? OtWorkingdayTimeMin { get; set; }

    /// <summary>获取或设置允许工作日加班 - 最长加班时长（超过则视为加班 xx 秒，单位秒；默认值 240 分钟）。</summary>
    [JsonPropertyName("ot_workingday_time_max")]
    public int? OtWorkingdayTimeMax { get; set; }

    /// <summary>获取或设置允许非工作日加班 - 最短加班时长（不足 xx 秒视为未加班，单位秒；默认值 30 分钟）。</summary>
    [JsonPropertyName("ot_nonworkingday_time_min")]
    public int? OtNonworkingdayTimeMin { get; set; }

    /// <summary>获取或设置允许非工作日加班 - 最长加班时长（超过则视为加班 xx 秒，单位秒；默认值 240 分钟）。</summary>
    [JsonPropertyName("ot_nonworkingday_time_max")]
    public int? OtNonworkingdayTimeMax { get; set; }

    /// <summary>获取或设置非工作日加班跨天时间（距离当天 00:00 的秒数）。</summary>
    [JsonPropertyName("ot_nonworkingday_spanday_time")]
    public int? OtNonworkingdaySpandayTime { get; set; }

    /// <summary>获取或设置工作日加班 - 休息扣除配置信息。</summary>
    [JsonPropertyName("ot_workingday_restinfo")]
    public CheckinOtRestInfo? OtWorkingdayRestinfo { get; set; }

    /// <summary>获取或设置非工作日加班 - 休息扣除配置信息（参数信息与工作日休息扣除配置一致）。</summary>
    [JsonPropertyName("ot_nonworkingday_restinfo")]
    public CheckinOtRestInfo? OtNonworkingdayRestinfo { get; set; }
}
