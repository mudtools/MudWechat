// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 补卡控件值（PunchCorrection，补卡模板特有）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalPunchCorrectionValue
{
    /// <summary>
    /// 获取或设置异常状态说明（官方示例如 "上班-未打卡"）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置补卡时间（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// 获取或设置版本标识，为 1 的时候为新版补卡，daymonthyear 有值。
    /// </summary>
    [JsonPropertyName("version")]
    public int? Version { get; set; }

    /// <summary>
    /// 获取或设置补卡日期 0 点 Unix时间戳。
    /// </summary>
    [JsonPropertyName("daymonthyear")]
    public long? Daymonthyear { get; set; }
}
