// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「获取票据授权链接」请求（官方 <c>getbillauthurl</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetBillAuthUrlRequest
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置财政局的 <c>appid</c>（官方 <c>s_pappid</c>，必填）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPAppId { get; set; } = string.Empty;

    /// <summary>获取或设置金额（官方 <c>money</c>，单位分，必填）。</summary>
    [JsonPropertyName("money")]
    public int Money { get; set; }

    /// <summary>获取或设置时间戳（官方 <c>timestamp</c>，秒级 Unix 时间戳，可选；缺省 SDK 以当前时间生成——本 SDK 不代填，由官方或调用方决定）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>获取或设置来源（官方 <c>source</c>，必填）。</summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>获取或设置授权后跳转地址（官方 <c>redirect_url</c>，可选）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    /// <summary>获取或设置票据下载票据（官方 <c>ticket</c>，必填；官方 <c>api_ticket</c>）。</summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;
}
