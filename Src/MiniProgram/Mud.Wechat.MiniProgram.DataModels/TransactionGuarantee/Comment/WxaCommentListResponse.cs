// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>查询评价列表应答（<c>GET /wxaapi/comment/mpcommentlist/get</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getccommentlist.html"/>。
/// </para>
/// <para>
/// <b>官方本页两处文档缺陷（照录，SDK 不替官方修正）</b>：① 参数表把 <c>errcode</c> 标为 <c>string</c>，
/// 而官方示例为 <c>errcode: 0</c>、同域其余 8 页均标 <c>number</c> ⇒ 沿用统一信封 <see cref="WxaResponse.ErrorCode"/>
/// 的 <see cref="int"/>，不为其单独改型；② 示例含 <c>success: true</c> 但参数表未列 ⇒ 本类型<b>不建模</b>
/// （未知 JSON 属性在源生成口径下直接忽略，判成败以 <c>errcode == 0</c> 为准）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentListResponse : WxaResponse
{
    /// <summary>本次查询的偏移数（<c>offset</c>，官方回显请求值）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>评价总数（<c>total</c>，分页终止判据）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>评价列表（<c>commentList</c>，官方类型 objarray），见 <see cref="WxaCommentListItem"/>。</summary>
    [JsonPropertyName("commentList")]
    public List<WxaCommentListItem>? CommentList { get; set; }
}
