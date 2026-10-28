// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 获取授权页链接（POST /card/invoice/getauthurl）

/// <summary>
/// 获取授权页链接（<c>POST /card/invoice/getauthurl</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>s_pappid</c> / <c>order_id</c> / <c>money</c>（单位<b>分</b>）/
/// <c>timestamp</c> / <c>source</c> / <c>ticket</c> / <c>type</c> 必填；<c>redirect_url</c> 可选。
/// </para>
/// <para>
/// <b><c>ticket</c> 的定位（关键）</b>：官方字段说明为「授权页 ticket」，<b>不是鉴权凭证</b>
/// （本接口鉴权走 Query <c>access_token</c>）；官方文档缺陷（照录）：全文<b>未说明该 ticket 从何获取</b>
/// ——SDK 不代取，归宿主（同族已有「获取 sdk 临时票据」端点可作候选来源，但官方未做关联说明）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>redirect_url</c> 说明为「只有在 <c>source</c> 为 <b>H5</b> 时需要填写」，
/// 但 <c>source</c> 的合法枚举为 <c>app</c>/<c>web</c>/<c>wxa</c>/<c>wap</c>，**不存在 H5 这一取值**
/// （疑指 <c>web</c>）；<c>wxa</c>/<c>wap</c> 是否需该字段未提及。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthUrlRequest
{
    /// <summary>获取或设置开票平台标识号（官方 <c>s_pappid</c>，必填；商户需找开票平台提供）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPappId { get; set; } = string.Empty;

    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填；商户内单笔开票请求的唯一识别号）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置订单金额（官方 <c>money</c>，必填；以<b>分</b>为单位）。</summary>
    [JsonPropertyName("money")]
    public int Money { get; set; }

    /// <summary>获取或设置时间戳（官方 <c>timestamp</c>，必填；官方未说明单位）。</summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>获取或设置开票来源（官方 <c>source</c>，必填；见 <c>MpInvoiceAuthSources</c>）。</summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>获取或设置授权成功后跳转页面（官方 <c>redirect_url</c>；官方称仅 H5（疑指 web）需要）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    /// <summary>获取或设置授权页 ticket（官方 <c>ticket</c>，必填；<b>非鉴权凭证</b>，官方未说明来源）。</summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;

    /// <summary>获取或设置授权类型（官方 <c>type</c>，必填；见 <c>MpInvoiceAuthUrlTypes</c>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}
