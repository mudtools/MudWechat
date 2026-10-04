// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 假期过期规则（expire_rule）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class VacationExpireRule
{
    /// <summary>
    /// 获取或设置过期规则类型：1-按固定时间过期；2-从发放日按年过期；3-从发放日按月过期；4-不过期。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置有效期，按年过期为年，按月过期为月（只有在以上两种情况时有效）。
    /// </summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    /// <summary>
    /// 获取或设置失效日期（只有按固定时间过期时有效）。
    /// </summary>
    [JsonPropertyName("date")]
    public VacationExpireDate? Date { get; set; }

    /// <summary>
    /// 获取或设置是否允许延长有效期。
    /// </summary>
    [JsonPropertyName("extern_duration_enable")]
    public bool? ExternDurationEnable { get; set; }

    /// <summary>
    /// 获取或设置延长有效期的具体时间（只有在 extern_duration_enable 为 true 时有效）。
    /// </summary>
    [JsonPropertyName("extern_duration")]
    public VacationExternDuration? ExternDuration { get; set; }
}
