// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则加班配置 V2（创建/修改打卡规则 <c>group.ot_info_v2</c>，官方必填）。
/// </summary>
/// <remarks>
/// <para>
/// workdayconf/restdayconf/holidayconf 三组配置的 <c>allow_ot</c>、<c>type</c> 均为官方必填；
/// 旧字段 <c>ot_info</c> 官方错误表明确「ot_info是旧字段，不建议使用」（301094）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtInfoV2
{
    /// <summary>获取或设置工作日加班配置（allow_ot、type 官方必填）。</summary>
    [JsonPropertyName("workdayconf")]
    public CheckinOtConf? Workdayconf { get; set; }

    /// <summary>获取或设置休息日加班配置（定义同 workdayconf；allow_ot、type 官方必填）。</summary>
    [JsonPropertyName("restdayconf")]
    public CheckinOtConf? Restdayconf { get; set; }

    /// <summary>获取或设置节假日加班配置（定义同 workdayconf；allow_ot、type 官方必填）。</summary>
    [JsonPropertyName("holidayconf")]
    public CheckinOtConf? Holidayconf { get; set; }

    /// <summary>获取或设置加班时长的单位与取整配置。</summary>
    [JsonPropertyName("time_unit_config")]
    public CheckinOtTimeUnitConfig? TimeUnitConfig { get; set; }
}
