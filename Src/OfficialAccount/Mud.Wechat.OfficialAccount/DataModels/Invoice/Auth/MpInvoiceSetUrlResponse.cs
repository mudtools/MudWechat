// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 获取开票平台识别码（POST /card/invoice/seturl）

/// <summary>
/// 获取开票平台识别码（<c>POST /card/invoice/seturl</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>无请求体</b>；返回 <c>invoice_url</c>（该开票平台专用的授权链接）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：① 接口名「获取开票平台识别码」，但返回字段<b>没有独立的识别码 /
/// <c>s_pappid</c> 字段</b>，仅返回 <c>invoice_url</c> —— <c>s_pappid</c> 须<b>从该 URL 内解析</b>
/// （官方说明反复围绕 <c>s_pappid</c> 却未单列该字段）；② 本页把 <c>errcode</c> 标为 <c>string</c>
/// （官方全站罕见，SDK 仍以 <see cref="MpResponse.ErrorCode"/> 的 <c>int</c> 承载，矛盾照录）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSetUrlResponse : MpResponse
{
    /// <summary>获取或设置该开票平台专用的授权链接（官方 <c>invoice_url</c>；<c>s_pappid</c> 须从中解析）。</summary>
    [JsonPropertyName("invoice_url")]
    public string? InvoiceUrl { get; set; }
}
