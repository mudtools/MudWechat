// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>创建回复请求（<c>POST /wxaapi/comment/commentreply/add</c>，官方「创建回复」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_addcommentreply.html"/>。
/// </para>
/// <para>
/// <b>三层结构（官方语义）</b>：评价（<c>commentId</c>）→ 评论（<c>replyId</c>，由「创建评论」产生）
/// → 回复（本端点在既有评论下追加回复）。故三字段<b>均必填</b>，缺 <c>replyId</c> 无法定位回复挂载点。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyAddRequest
{
    /// <summary>评价的 id（<c>commentId</c>，必填）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>评论的 id（<c>replyId</c>，必填）。</summary>
    [JsonPropertyName("replyId")]
    public string? ReplyId { get; set; }

    /// <summary>回复评论的内容（<c>content</c>，必填）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
