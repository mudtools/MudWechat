// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批流程子节点（process_list.node_list.sub_node_list 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalSubNodeInfo
{
    /// <summary>
    /// 获取或设置处理人信息。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置审批/办理意见。
    /// </summary>
    [JsonPropertyName("speech")]
    public string? Speech { get; set; }

    /// <summary>
    /// 获取或设置子节点状态：1-审批中；2-同意；3-驳回；4-转审；11-退回给指定审批人；12-加签；13-同意并加签；14-办理；15-转交。
    /// </summary>
    [JsonPropertyName("sp_yj")]
    public int? SpYj { get; set; }

    /// <summary>
    /// 获取或设置操作时间。
    /// </summary>
    [JsonPropertyName("sptime")]
    public long? Sptime { get; set; }

    /// <summary>
    /// 获取或设置附件（微盘文件无法获取，media_id 具体使用请参考「文档-获取临时素材」）。
    /// </summary>
    [JsonPropertyName("media_ids")]
    public List<string>? MediaIds { get; set; }
}
