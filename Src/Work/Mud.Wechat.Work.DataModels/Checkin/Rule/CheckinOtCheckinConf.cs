// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则加班配置 V2「只以打卡时间为准」配置（<c>ot_info_v2.*.checkin</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtCheckinConf
{
    /// <summary>获取或设置下班后多久开始计算加班（单位：秒，不超过 720 分钟；默认 30 分钟）。</summary>
    [JsonPropertyName("ot_time_start")]
    public int? OtTimeStart { get; set; }

    /// <summary>获取或设置最小加班时长（单位：秒，不超过 720 分钟；默认 30 分钟；必须小于 ot_time_max）。</summary>
    [JsonPropertyName("ot_time_min")]
    public int? OtTimeMin { get; set; }

    /// <summary>获取或设置最大加班时长（单位：秒，不超过 720 分钟；默认 240 分钟）。</summary>
    [JsonPropertyName("ot_time_max")]
    public int? OtTimeMax { get; set; }

    /// <summary>获取或设置休息时间配置（使用同 ot_info_v2.workdayconf.apply.restinfo）。</summary>
    [JsonPropertyName("restinfo")]
    public CheckinOtV2RestInfo? Restinfo { get; set; }
}
