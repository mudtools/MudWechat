// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则加班配置 V2 加班时长单位与取整配置（<c>ot_info_v2.time_unit_config</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtTimeUnitConfig
{
    /// <summary>获取或设置加班单位：2 - 小时；3 - 天（默认为 2）。</summary>
    [JsonPropertyName("ot_time_unit")]
    public int? OtTimeUnit { get; set; }

    /// <summary>获取或设置单位换算：1 天 = x 小时（单位为秒；取值范围 3600-86400，且必须是 0.1 小时的倍数；默认 8 小时）。</summary>
    [JsonPropertyName("perday_duration_secs")]
    public int? PerdayDurationSecs { get; set; }

    /// <summary>获取或设置取整方式：1 - 四舍五入；2 - 向上取整；3 - 向下取整（默认为 1）。</summary>
    [JsonPropertyName("rounding_method")]
    public int? RoundingMethod { get; set; }

    /// <summary>获取或设置四舍五入时保留的小数位（默认为 10 表示 0 位小数，1 表示 1 位小数；目前仅支持整数或 1 位小数）。</summary>
    [JsonPropertyName("rounding_precision")]
    public int? RoundingPrecision { get; set; }

    /// <summary>获取或设置取整步长（单位为秒；取整方式为向上/向下取整时需填写，默认为 3600 表示 1 小时。加班单位为小时时向上取整可选 1800/3600，向下取整可选 1800/3600/7200/10800/14400；加班单位为天时可选 43200 或 86400）。</summary>
    [JsonPropertyName("step_size")]
    public int? StepSize { get; set; }
}
