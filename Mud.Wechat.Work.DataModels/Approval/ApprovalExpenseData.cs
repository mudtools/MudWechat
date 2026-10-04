// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批记录报销数据（expense，只有报销模板的审批记录有此数据项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalExpenseData
{
    /// <summary>
    /// 获取或设置报销类型：1-差旅费；2-交通费；3-招待费；4-其他报销。
    /// </summary>
    [JsonPropertyName("expense_type")]
    public int? ExpenseType { get; set; }

    /// <summary>
    /// 获取或设置报销事由。
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// 获取或设置报销明细（历史单据字段，新申请单据不再提供）。
    /// </summary>
    [JsonPropertyName("item")]
    public List<ApprovalExpenseItem>? Item { get; set; }
}
