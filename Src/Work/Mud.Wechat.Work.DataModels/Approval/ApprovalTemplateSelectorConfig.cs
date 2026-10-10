// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 单选/多选控件配置（config.selector，获取审批模板详情响应侧）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateSelectorConfig
{
    /// <summary>
    /// 获取或设置选择类型：single-单选；multi-多选。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置选项列表，包含单选/多选控件中的所有选项，可能有多个。
    /// </summary>
    [JsonPropertyName("options")]
    public List<ApprovalTemplateSelectorOption>? Options { get; set; }

    /// <summary>
    /// 获取或设置控件关联配置（如果设置了控件关联，则会有此项）。
    /// </summary>
    [JsonPropertyName("op_relations")]
    public List<ApprovalSelectorOpRelation>? OpRelations { get; set; }
}
