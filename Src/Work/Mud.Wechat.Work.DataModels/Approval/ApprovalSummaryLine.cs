// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批申请摘要行（summary_list 元素，用于显示在审批通知卡片、审批列表的摘要信息，最多 3 行）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalSummaryLine
{
    /// <summary>
    /// 获取或设置摘要行信息，用于定义某一行摘要显示的内容（摘要行显示文字不要超过 20 个字符）。
    /// </summary>
    [JsonPropertyName("summary_info")]
    public List<ApprovalLangTextItem>? SummaryInfo { get; set; }
}
