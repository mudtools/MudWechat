// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 拒绝开票（POST /card/invoice/rejectinsert）

/// <summary>
/// 拒绝开票（<c>POST /card/invoice/rejectinsert</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>s_pappid</c> / <c>order_id</c> / <c>reason</c> 必填；<c>url</c> 可选（跳转链接）。
/// </para>
/// <para>
/// <b>不可逆语义（官方原文，勿弱化）</b>：拒绝开票后该订单<b>无法向用户再次开票</b>；
/// 如需重新开票必须使用<b>新的 <c>order_id</c></b> 并重新获取授权链接让用户再次授权；调用后用户侧会收到通知消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceRejectInsertRequest
{
    /// <summary>获取或设置开票平台标识（官方 <c>s_pappid</c>，必填；由开票平台告知商户）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPappId { get; set; } = string.Empty;

    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置拒绝原因（官方 <c>reason</c>，必填；如重复开票、抬头无效、已退货无法开票等）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置跳转链接（官方 <c>url</c>；引导用户重新发起开票 / 重新填写抬头 / 展示订单情况）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
