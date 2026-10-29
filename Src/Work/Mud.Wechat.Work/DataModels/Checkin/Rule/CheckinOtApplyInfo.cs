// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 企业打卡规则加班「以加班申请核算」相关信息（获取企业所有打卡规则响应 <c>group.ot_info.otapplyinfo</c>，只有设置相关信息且以加班申请核算打卡时才有）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtApplyInfo
{
    /// <summary>获取或设置允许工作日加班（true 为允许）。</summary>
    [JsonPropertyName("allow_ot_workingday")]
    public bool? AllowOtWorkingday { get; set; }

    /// <summary>获取或设置允许非工作日加班（true 为允许）。</summary>
    [JsonPropertyName("allow_ot_nonworkingday")]
    public bool? AllowOtNonworkingday { get; set; }

    /// <summary>获取或设置更新时间（Unix 时间戳）。</summary>
    [JsonPropertyName("uptime")]
    public long? Uptime { get; set; }

    /// <summary>获取或设置工作日加班 - 休息扣除配置信息（内含参数释义基本同 otcheckinfo 的休息扣除配置）。</summary>
    [JsonPropertyName("ot_workingday_restinfo")]
    public CheckinOtRestInfo? OtWorkingdayRestinfo { get; set; }

    /// <summary>获取或设置非工作日加班 - 休息扣除配置信息。</summary>
    [JsonPropertyName("ot_nonworkingday_restinfo")]
    public CheckinOtRestInfo? OtNonworkingdayRestinfo { get; set; }

    /// <summary>获取或设置非工作日加班跨天时间（距离当天 00:00 的秒数）。</summary>
    [JsonPropertyName("ot_nonworkingday_spanday_time")]
    public int? OtNonworkingdaySpandayTime { get; set; }
}
