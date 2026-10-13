// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>确认和解请求（<c>POST /wxaapi/comment/confirmcompromise</c>，官方「确认和解」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_confirmcompromise.html"/>。
/// </para>
/// <para>
/// <b>官方前置（错误码 <c>10000</c> 的成因）</b>：官方错误码表标注「和解：不是差评 / 发送过模板消息」即失败
/// ⇒ 和解挽回仅适用于<b>差评</b>，且该评价<b>尚未发过</b>「差评客服会话」模板消息
/// （该状态见 <see cref="WxaCommentExtInfo.IsAlreadySendTmpl"/>）。
/// </para>
/// <para><b>图片凭证</b>：<c>picList</c> 承载的是素材 <c>mediaId</c> 集合（官方指向素材管理），三字段官方均标必填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentCompromiseRequest
{
    /// <summary>评价的 id（<c>commentId</c>，必填）。</summary>
    [JsonPropertyName("commentId")]
    public string? CommentId { get; set; }

    /// <summary>和解的图片 mediaId 集合（<c>picList</c>，必填；来源为素材管理新增图片素材）。</summary>
    [JsonPropertyName("picList")]
    public List<string>? PicList { get; set; }

    /// <summary>和解的文本内容（<c>content</c>，必填）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
