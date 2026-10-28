// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则加班配置 V2 休息扣除配置（<c>ot_info_v2.*.apply/checkin/applycheckin.restinfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 与获取企业所有打卡规则响应侧旧 <c>ot_info</c> 的休息扣除配置（<see cref="CheckinOtRestInfo"/>）不同构：
/// 本模型指定休息时间为 <c>fix_time_rule_list</c> 数组（不超过 10 个），旧结构为单对象 <c>fix_time_rule</c>。
/// 选择按加班时长扣除时不可设置按休息时段扣除的字段，反之亦然。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinOtV2RestInfo
{
    /// <summary>获取或设置休息扣除配置（不可超过 2）：0 - 不扣除；1 - 指定休息时间扣除；2 - 按加班时长扣除。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置指定休息时间扣除的配置（不超过 10 个；type 为 1 时有意义；多个时段之间不可存在交集，且之间不可超过 24 小时）。</summary>
    [JsonPropertyName("fix_time_rule_list")]
    public List<CheckinOtFixTimeRule>? FixTimeRuleList { get; set; }

    /// <summary>获取或设置按加班时长扣除的配置（type 为 2 时有意义；items 个数需要大于 0 且不超过 5，ot_time 需要不断递增且为 15 分钟的倍数）。</summary>
    [JsonPropertyName("cal_ottime_rule")]
    public CheckinOtCalRule? CalOttimeRule { get; set; }
}
