// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 关联审批单控件选项（related_approval 元素，control 为 RelatedApproval）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalRelatedApprovalItem
{
    /// <summary>
    /// 获取或设置关联审批单的模板名（多语言；仅获取审批申请详情响应侧返回）。
    /// </summary>
    [JsonPropertyName("template_names")]
    public List<ApprovalLangTextItem>? TemplateNames { get; set; }

    /// <summary>
    /// 获取或设置关联审批单的状态（仅获取审批申请详情响应侧返回）。
    /// </summary>
    [JsonPropertyName("sp_status")]
    public int? SpStatus { get; set; }

    /// <summary>
    /// 获取或设置关联审批单的提单者（仅获取审批申请详情响应侧返回）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置关联审批单的提单时间（仅获取审批申请详情响应侧返回）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置关联审批单的审批单号。
    /// </summary>
    [JsonPropertyName("sp_no")]
    public string? SpNo { get; set; }
}
