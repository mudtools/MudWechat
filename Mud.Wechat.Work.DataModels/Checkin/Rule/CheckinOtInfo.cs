// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 企业打卡规则加班信息（获取企业所有打卡规则响应 <c>group.ot_info</c>，相关信息需要设置后才能显示）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>uptime</c> 与 <c>otapplyinfo</c> 两行在官方参数表中缺 <c>group.ot_info.</c> 前缀，
/// 按返回示例它们属于 <c>group.ot_info</c> 之下（且 <c>otapplyinfo</c> 内部还有自己的 <c>uptime</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtInfo
{
    /// <summary>获取或设置加班类型：0 - 以加班申请核算打卡记录（根据打卡记录和加班申请核算）；1 - 以打卡时间为准（根据打卡时间计算）；2 - 以加班申请审批为准（只根据加班申请计算）。官方类型标注 int32。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置允许工作日加班（true 为允许；官方参数表将 false 拼写为 flase，照抄）。</summary>
    [JsonPropertyName("allow_ot_workingday")]
    public bool? AllowOtWorkingday { get; set; }

    /// <summary>获取或设置允许非工作日加班（true 为允许；官方参数表将 false 拼写为 flase，照抄）。</summary>
    [JsonPropertyName("allow_ot_nonworkingday")]
    public bool? AllowOtNonworkingday { get; set; }

    /// <summary>获取或设置以打卡时间为准 - 加班时长计算规则信息。</summary>
    [JsonPropertyName("otcheckinfo")]
    public CheckinOtCheckInfo? Otcheckinfo { get; set; }

    /// <summary>获取或设置更新时间（Unix 时间戳；官方参数表该行缺 ot_info 前缀，按返回示例属于 ot_info 之下）。</summary>
    [JsonPropertyName("uptime")]
    public long? Uptime { get; set; }

    /// <summary>获取或设置以加班申请核算打卡记录相关信息（根据加班申请核算加班时长，只有设置相关信息且以加班申请核算打卡时才有相关信息；官方参数表该行缺 ot_info 前缀，按返回示例属于 ot_info 之下）。</summary>
    [JsonPropertyName("otapplyinfo")]
    public CheckinOtApplyInfo? Otapplyinfo { get; set; }
}
