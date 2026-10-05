// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则请求模型晚走晚到时间规则（创建/修改打卡规则 <c>group.checkindate.late_rule</c> / <c>group.schedulelist.late_rule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 请求侧 <c>offwork_after_time</c>/<c>onwork_flex_time</c> 为 late_rule 直接子字段，同时存在
/// <see cref="Timerules"/> 数组版本（timerules 不为空时优先使用 timerules）；
/// 响应侧（获取企业所有打卡规则）late_rule 仅含 allow_offwork_after_time 与 timerules（见 <see cref="CheckinLateRule"/>）。
/// 官方错误表中晚走晚到大小关系的两条描述语义相反（官方原文矛盾，照抄）：
/// 一条为「onwork_flex_time 需要大于 offwork_after_time」，另一条为「offwork_after_time 需要大于等于 onwork_flex_time」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRuleLateRule
{
    /// <summary>获取或设置晚走的时间（距离最晚一个下班的时间，单位：秒，不超过 600 分钟；有值时需要 allow_offwork_after_time 为 true）。</summary>
    [JsonPropertyName("offwork_after_time")]
    public int? OffworkAfterTime { get; set; }

    /// <summary>获取或设置第二天第一个班次允许迟到的弹性时间（单位：秒，不超过 600 分钟；有值时需要 allow_offwork_after_time 为 true）。</summary>
    [JsonPropertyName("onwork_flex_time")]
    public int? OnworkFlexTime { get; set; }

    /// <summary>获取或设置是否允许超时下班（下班晚走次日晚到；允许时 offwork_after_time、onwork_flex_time 才有意义；设置晚走晚到需要开启本项）。</summary>
    [JsonPropertyName("allow_offwork_after_time")]
    public bool? AllowOffworkAfterTime { get; set; }

    /// <summary>获取或设置晚走晚到时间规则（有值时需要 allow_offwork_after_time 为 true；timerules 不为空时优先使用 timerules）。</summary>
    [JsonPropertyName("timerules")]
    public List<CheckinLateTimeRule>? Timerules { get; set; }
}
