// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 单选/多选控件值（Selector）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalSelectorValue
{
    /// <summary>
    /// 获取或设置选择类型：single-单选；multi-多选（假勤控件中固定为单选）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置所选选项，多选情况下可能有多个（响应侧仅包含申请人所选择的选项，并非所有选项，若需了解所有选项需使用「获取审批模板详情」接口）。
    /// </summary>
    [JsonPropertyName("options")]
    public List<ApprovalSelectorOption>? Options { get; set; }

    /// <summary>
    /// 获取或设置扩展类型字段（官方请假组件响应示例返回 0；仅假勤场景出现）。
    /// </summary>
    [JsonPropertyName("exp_type")]
    public int? ExpType { get; set; }
}
