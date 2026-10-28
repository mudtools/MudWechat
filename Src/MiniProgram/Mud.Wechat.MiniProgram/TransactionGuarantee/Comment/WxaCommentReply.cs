// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>评论（<c>list.reply</c>，官方为「评论回复第一条内容」）。</summary>
/// <remarks>
/// <b>定位键</b>：<see cref="ReplyId"/> 是删除评论（<c>reply/delete</c>）与创建回复（<c>commentreply/add</c>）的必填参数来源。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReply
{
    /// <summary>评价 id（<c>commentId</c>）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>评论 id（<c>replyId</c>）。</summary>
    [JsonPropertyName("replyId")]
    public string? ReplyId { get; set; }

    /// <summary>创建时间（<c>createTime</c>，<b>单位是秒</b>；官方参数表标 string）。</summary>
    [JsonPropertyName("createTime")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间（<c>updateTime</c>，<b>单位是秒</b>；官方参数表标 string）。</summary>
    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }

    /// <summary>回复内容（<c>replyContent</c>，与回复列表元素共用），见 <see cref="WxaCommentReplyContent"/>。</summary>
    [JsonPropertyName("replyContent")]
    public WxaCommentReplyContent? ReplyContent { get; set; }

    /// <summary>评论的主体（<c>replyObject</c>，与回复列表元素共用），见 <see cref="WxaCommentReplyObject"/>。</summary>
    [JsonPropertyName("replyObject")]
    public WxaCommentReplyObject? ReplyObject { get; set; }
}
