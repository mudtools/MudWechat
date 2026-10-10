// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批节点分支（Items.Item 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class OpenApprovalNodeItem
{
    /// <summary>
    /// 获取或设置分支审批人姓名。
    /// </summary>
    [JsonPropertyName("ItemName")]
    public string? ItemName { get; set; }

    /// <summary>
    /// 获取或设置分支审批人所在部门。
    /// </summary>
    [JsonPropertyName("ItemParty")]
    public string? ItemParty { get; set; }

    /// <summary>
    /// 获取或设置分支审批人头像。
    /// </summary>
    [JsonPropertyName("ItemImage")]
    public string? ItemImage { get; set; }

    /// <summary>
    /// 获取或设置分支审批人userid（官方字段名即为 ItemUserId）。
    /// </summary>
    [JsonPropertyName("ItemUserId")]
    public string? ItemUserId { get; set; }

    /// <summary>
    /// 获取或设置分支审批人审批操作状态：1-审批中；2-已同意；3-已驳回；4-已转审。
    /// </summary>
    [JsonPropertyName("ItemStatus")]
    public int? ItemStatus { get; set; }

    /// <summary>
    /// 获取或设置分支审批人审批意见。
    /// </summary>
    [JsonPropertyName("ItemSpeech")]
    public string? ItemSpeech { get; set; }

    /// <summary>
    /// 获取或设置分支审批人操作时间（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("ItemOpTime")]
    public long? ItemOpTime { get; set; }
}
