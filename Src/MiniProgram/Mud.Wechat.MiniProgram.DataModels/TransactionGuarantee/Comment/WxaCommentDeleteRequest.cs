// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>删除评论请求（<c>POST /wxaapi/comment/reply/delete</c>，官方「删除评论」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_deletereply.html"/>。
/// </para>
/// <para><b>定位键两字段均必填</b>：<c>replyId</c>（评论 id）由 <c>replyandcommentreplylist/get</c>
/// 响应侧 <c>list.reply.replyId</c> 取得。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentDeleteRequest
{
    /// <summary>评价的 id（<c>commentId</c>，必填）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>评论的 id（<c>replyId</c>，必填）。</summary>
    [JsonPropertyName("replyId")]
    public string? ReplyId { get; set; }
}
