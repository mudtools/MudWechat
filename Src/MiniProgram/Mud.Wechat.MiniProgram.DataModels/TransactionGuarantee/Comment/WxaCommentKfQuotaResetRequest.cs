// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>重置 Api 客服 quota 请求（<c>POST /wxaapi/comment/apikfquota/reset</c>，官方「重置Api客服quota」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_resetapikfquota.html"/>。
/// </para>
/// <para>
/// <b>语义</b>：差评客服会话（api 客服）的沟通额度按评价计数，官方将「重置」挂在评价维度上
/// ⇒ 定位键仅 <c>commentId</c> 一个字段（官方标必填）。与 <c>confirmcompromise</c> 同为「差评挽回」流程动作，
/// 受同一组错误码约束。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentKfQuotaResetRequest
{
    /// <summary>评价的 id（<c>commentId</c>，必填）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }
}
