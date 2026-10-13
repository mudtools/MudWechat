// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价下的评论与回复集合（<c>list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方层级语义（照录，勿按字面理解命名）</b>：官方原文 <c>reply</c> = 「评论（评论回复第一条内容）」，
/// <c>commentReplyList</c> = 「回复（评论回复第二条及之后的所有集合）」——
/// 即 <c>reply</c> 是<b>单数对象</b>而非列表，回复集合才是数组。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyList
{
    /// <summary>评论（<c>reply</c>，官方为单数对象），见 <see cref="WxaCommentReply"/>。</summary>
    [JsonPropertyName("reply")]
    public WxaCommentReply? Reply { get; set; }

    /// <summary>回复列表（<c>commentReplyList</c>，官方类型 objarray），见 <see cref="WxaCommentReplyItem"/>。</summary>
    [JsonPropertyName("commentReplyList")]
    public List<WxaCommentReplyItem>? CommentReplyList { get; set; }
}
