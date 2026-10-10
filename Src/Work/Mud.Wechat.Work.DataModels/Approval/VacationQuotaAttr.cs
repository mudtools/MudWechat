// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 假期发放相关配置（quota_attr）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class VacationQuotaAttr
{
    /// <summary>
    /// 获取或设置假期发放类型：0-不限额；1-自动按年发放；2-手动发放；3-自动按月发放。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置自动发放时间戳；若假期发放为自动发放，此参数代表自动发放日期。
    /// </summary>
    /// <remarks>
    /// <para>返回时间戳的年份是无意义的，请只使用返回时间的月和日；若 at_entry_date 为 true，该字段无效，假期发放时间为员工入职时间。</para>
    /// </remarks>
    [JsonPropertyName("autoreset_time")]
    public long? AutoresetTime { get; set; }

    /// <summary>
    /// 获取或设置自动发放时长，单位为秒。
    /// </summary>
    /// <remarks>
    /// <para>只有自动按年发放和自动按月发放时有效；若选择了按照工龄和司龄发放，该字段无效，发放时长请使用区间中的 quota。</para>
    /// </remarks>
    [JsonPropertyName("autoreset_duration")]
    public long? AutoresetDuration { get; set; }

    /// <summary>
    /// 获取或设置额度计算类型（自动按年发放时有效）：0-固定额度；1-按工龄计算；2-按司龄计算。
    /// </summary>
    [JsonPropertyName("quota_rule_type")]
    public int? QuotaRuleType { get; set; }

    /// <summary>
    /// 获取或设置额度计算规则（自动按年发放时有效）。
    /// </summary>
    [JsonPropertyName("quota_rules")]
    public VacationQuotaRules? QuotaRules { get; set; }

    /// <summary>
    /// 获取或设置是否按照入职日期发放假期（只有在自动按年发放类型有效，选择后发放假期的时间会成为员工入职的日期）。
    /// </summary>
    [JsonPropertyName("at_entry_date")]
    public bool? AtEntryDate { get; set; }

    /// <summary>
    /// 获取或设置自动按月发放的发放时间（只有自动按月发放类型有效）。
    /// </summary>
    [JsonPropertyName("auto_reset_month_day")]
    public int? AutoResetMonthDay { get; set; }
}
