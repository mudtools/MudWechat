// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批记录报销明细（expense.item 元素，历史单据字段，新申请单据不再提供）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalExpenseItem
{
    /// <summary>
    /// 获取或设置费用类型：1-飞机票；2-火车票；3-的士费；4-住宿费；5-餐饮费；6-礼品费；7-活动费；8-通讯费；9-补助；10-其他。
    /// </summary>
    [JsonPropertyName("expenseitem_type")]
    public int? ExpenseitemType { get; set; }

    /// <summary>
    /// 获取或设置发生时间（Unix时间）。
    /// </summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// 获取或设置费用金额，单位元。
    /// </summary>
    [JsonPropertyName("sums")]
    public long? Sums { get; set; }

    /// <summary>
    /// 获取或设置明细事由。
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
