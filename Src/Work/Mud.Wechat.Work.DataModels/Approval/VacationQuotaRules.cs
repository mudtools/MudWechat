// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 假期额度计算规则（quota_attr.quota_rules）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class VacationQuotaRules
{
    /// <summary>
    /// 获取或设置额度计算规则区间列表（只有在选择了按照工龄计算或者按照司龄计算时有效）。
    /// </summary>
    [JsonPropertyName("list")]
    public List<VacationQuotaRuleItem>? List { get; set; }

    /// <summary>
    /// 获取或设置是否根据实际入职时间计算假期（选择后会根据员工在今年的实际工作时间发放假期）。
    /// </summary>
    [JsonPropertyName("based_on_actual_work_time")]
    public bool? BasedOnActualWorkTime { get; set; }
}
