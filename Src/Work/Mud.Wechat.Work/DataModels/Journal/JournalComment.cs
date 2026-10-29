// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报评论（官方 comments 元素）。
/// <para>
/// 官方 <c>commentid</c> / <c>tocommentid</c> 的线格式为数字（与审批备注侧的 commentid 字符串形态不同），
/// 本仓不做数值-字符串转换，故以 <c>long</c> 承载。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalComment
{
    /// <summary>获取或设置评论 ID（官方 commentid，数字形态）。</summary>
    [JsonPropertyName("commentid")]
    public long CommentId { get; set; }

    /// <summary>获取或设置被回复的评论 ID（官方 tocommentid，数字形态；非回复评论时不出现）。</summary>
    [JsonPropertyName("tocommentid")]
    public long? ToCommentId { get; set; }

    /// <summary>获取或设置评论人信息（官方 comment_userinfo）。</summary>
    [JsonPropertyName("comment_userinfo")]
    public ApprovalUserRef? CommentUser { get; set; }

    /// <summary>获取或设置评论内容（官方 content）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>获取或设置评论时间戳（官方 comment_time，秒级）。</summary>
    [JsonPropertyName("comment_time")]
    public long CommentTimestamp { get; set; }
}
