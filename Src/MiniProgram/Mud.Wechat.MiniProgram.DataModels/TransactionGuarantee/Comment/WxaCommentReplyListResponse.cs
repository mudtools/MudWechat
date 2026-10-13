// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>查询评论列表应答（<c>GET /wxaapi/comment/replyandcommentreplylist/get</c>，官方「查询评论列表」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getcommentreplylist.html"/>。
/// </para>
/// <para>
/// <b>返回体只有 <c>errcode</c> 与 <c>list</c> 两个字段</b>（参数表未列 <c>errmsg</c>）⇒ 信封字段走基类，
/// 评论主体在 <c>list</c> 内再分「评论 + 回复列表」两层，见 <see cref="WxaCommentReplyList"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyListResponse : WxaResponse
{
    /// <summary>评论（官方说明「评论回复第一条内容」的外壳），见 <see cref="WxaCommentReplyList"/>。</summary>
    [JsonPropertyName("list")]
    public WxaCommentReplyList? List { get; set; }
}
