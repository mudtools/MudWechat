// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 企业假期管理配置项（getcorpconf 响应的 lists 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class VacationCorpConfItem
{
    /// <summary>
    /// 获取或设置假期id。
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// 获取或设置假期名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置假期时间刻度：0-按天请假；1-按小时请假。
    /// </summary>
    [JsonPropertyName("time_attr")]
    public int? TimeAttr { get; set; }

    /// <summary>
    /// 获取或设置时长计算类型：0-自然日；1-工作日。
    /// </summary>
    [JsonPropertyName("duration_type")]
    public int? DurationType { get; set; }

    /// <summary>
    /// 获取或设置假期发放相关配置。
    /// </summary>
    [JsonPropertyName("quota_attr")]
    public VacationQuotaAttr? QuotaAttr { get; set; }

    /// <summary>
    /// 获取或设置单位换算值，即 1 天对应的秒数，可将此值除以 3600 得到一天对应的小时。
    /// </summary>
    [JsonPropertyName("perday_duration")]
    public long? PerdayDuration { get; set; }

    /// <summary>
    /// 获取或设置是否关联加班调休：0-不关联；1-关联（关联后该假期类型变为调休假；官方字段名即为 is_newovertime）。
    /// </summary>
    [JsonPropertyName("is_newovertime")]
    public int? IsNewovertime { get; set; }

    /// <summary>
    /// 获取或设置入职时间大于 n 个月可用该假期，单位为月。
    /// </summary>
    [JsonPropertyName("enter_comp_time_limit")]
    public int? EnterCompTimeLimit { get; set; }

    /// <summary>
    /// 获取或设置假期过期规则。
    /// </summary>
    [JsonPropertyName("expire_rule")]
    public VacationExpireRule? ExpireRule { get; set; }
}
