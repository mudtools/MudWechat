// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 旧评价信息（<c>oldComment</c>，官方原文「只有改评的新评价才有」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段集照官方本页参数表</b>：<c>commentId</c> / <c>createTime</c> / <c>score</c>（标 number）/
/// <c>content</c> —— 比新评价主体<b>窄得多</b>（无订单、用户、商品等字段），
/// 且其内容字段名为 <c>ext</c> 而非 <c>txt</c>，故内容类型独立为 <see cref="WxaCommentOldContent"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentOld
{
    /// <summary>评价 id（<c>commentId</c>）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>创建时间（<c>createTime</c>，<b>单位是秒</b>）。</summary>
    [JsonPropertyName("createTime")]
    public string? CreateTime { get; set; }

    /// <summary>用户评价的打分（<c>score</c>，官方口径「每 100 对应 1 分以及 1 颗星」；本页参数表标 number）。</summary>
    [JsonPropertyName("score")]
    public long? Score { get; set; }

    /// <summary>评论的内容（<c>content</c>），见 <see cref="WxaCommentOldContent"/>。</summary>
    [JsonPropertyName("content")]
    public WxaCommentOldContent? Content { get; set; }
}
