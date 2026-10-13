// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 回复项（<c>list.commentReplyList[]</c>，官方为「评论回复第二条及之后的所有集合」的元素）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="WxaCommentReply"/> 的字段名差异照录</b>：官方对「评论」用 <c>replyId</c> /
/// <c>replyContent</c> / <c>replyObject</c>，对「回复」用 <c>commentReplyId</c> /
/// <c>commentReplyContent</c> / <c>commentReplyObject</c> ⇒ 两类型不合并；
/// 其中 <c>commentReplyId</c> 是删除回复（<c>commentreply/delete</c>）的必填定位键。
/// 内容主体两形态字段集一致 ⇒ 共用 <see cref="WxaCommentReplyContent"/> 与 <see cref="WxaCommentReplyObject"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyItem
{
    /// <summary>评价 id（<c>commentId</c>）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>回复 id（<c>commentReplyId</c>）。</summary>
    [JsonPropertyName("commentReplyId")]
    public string? CommentReplyId { get; set; }

    /// <summary>评价创建时间（<c>createTime</c>，<b>单位是秒</b>；官方原文如此描述本字段）。</summary>
    [JsonPropertyName("createTime")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间（<c>updateTime</c>，<b>单位是秒</b>）。</summary>
    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }

    /// <summary>回复内容（<c>commentReplyContent</c>），见 <see cref="WxaCommentReplyContent"/>。</summary>
    [JsonPropertyName("commentReplyContent")]
    public WxaCommentReplyContent? CommentReplyContent { get; set; }

    /// <summary>评论的主体（<c>commentReplyObject</c>），见 <see cref="WxaCommentReplyObject"/>。</summary>
    [JsonPropertyName("commentReplyObject")]
    public WxaCommentReplyObject? CommentReplyObject { get; set; }
}
