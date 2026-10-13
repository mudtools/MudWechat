// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>删除回复请求（<c>POST /wxaapi/comment/commentreply/delete</c>，官方「删除回复」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_deletecommentreply.html"/>。
/// </para>
/// <para>
/// <b>三字段定位键均必填</b>：与「删除评论」（两字段）的唯一差异是此处需 <c>commentReplyId</c>
/// ——回复 id 由 <c>replyandcommentreplylist/get</c> 响应侧 <c>list.commentReplyList[].commentReplyId</c> 取得。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyDeleteRequest
{
    /// <summary>评价的 id（<c>commentId</c>，必填）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>评论的 id（<c>replyId</c>，必填）。</summary>
    [JsonPropertyName("replyId")]
    public string? ReplyId { get; set; }

    /// <summary>回复评论的 id（<c>commentReplyId</c>，必填）。</summary>
    [JsonPropertyName("commentReplyId")]
    public string? CommentReplyId { get; set; }
}
