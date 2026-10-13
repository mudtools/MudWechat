// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>差评处理进度（<c>processInfo</c>，官方原文「只有差评才会有」）。</summary>
/// <remarks>
/// <para>
/// <b>当前状态判据（官方原文）</b>：<c>actionList</c> 是数组，「<b>从数组最后往前数有 <c>updateTime</c> 的就是当前状态</b>」
/// ⇒ SDK 不做状态推导（顺序与缺值语义由官方决定），宿主按该口径自行取当前节点。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentProcessInfo
{
    /// <summary>进度的具体状态列表（<c>actionList</c>，官方类型 objarray），见 <see cref="WxaCommentProcessAction"/>。</summary>
    [JsonPropertyName("actionList")]
    public List<WxaCommentProcessAction>? ActionList { get; set; }

    /// <summary>评价进度对应的评价 id（<c>commentId</c>）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }
}
