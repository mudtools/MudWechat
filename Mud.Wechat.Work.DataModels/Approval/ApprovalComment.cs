// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批申请备注信息（comments 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalComment
{
    /// <summary>
    /// 获取或设置备注人信息（官方字段名为 commentUserInfo）。
    /// </summary>
    [JsonPropertyName("commentUserInfo")]
    public ApprovalUserRef? CommentUserInfo { get; set; }

    /// <summary>
    /// 获取或设置备注提交时间戳（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("commenttime")]
    public long? Commenttime { get; set; }

    /// <summary>
    /// 获取或设置备注文本内容。
    /// </summary>
    [JsonPropertyName("commentcontent")]
    public string? Commentcontent { get; set; }

    /// <summary>
    /// 获取或设置备注id。
    /// </summary>
    [JsonPropertyName("commentid")]
    public string? Commentid { get; set; }

    /// <summary>
    /// 获取或设置备注附件id，可能有多个（微盘文件无法获取，media_id 具体使用请参考「文档-获取临时素材」）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public List<string>? MediaId { get; set; }
}
