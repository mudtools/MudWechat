// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 更新售后请求体（<c>POST /shop/ecaftersale/update</c>）。
/// </summary>
/// <remarks>
/// <para><b>不支持改售后类型</b>：官方原文「<c>type</c> 需要和售后单当前类型一致，不支持修改售后类型」。</para>
/// <para><b>金额单位分</b>；<c>media_list</c> 是用户凭证图片，选填。官方「见枚举值定义 emAfterSalesReason」为断句残缺，</para>
/// <para>取值仍以 <see cref="WxaEcAftersaleReasonTypes"/> 为准。</para>
/// <para><b>敏感值</b>：<c>openid</c>、<c>refund_reason</c>、<c>media_list[].url</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleUpdateRequest
{
    /// <summary>商家自定义售后 ID（与 <c>aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>微信侧售后单号（与 <c>out_aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }

    /// <summary>退款金额，单位<b>分</b>（必填）。</summary>
    [JsonPropertyName("orderamt")]
    public long? Orderamt { get; set; }

    /// <summary>用户的 <c>openid</c>（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>售后类型（必填，须与当前类型一致）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>退款原因（必填，敏感）。</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>退款原因类型（必填），见 <see cref="WxaEcAftersaleReasonTypes"/>。</summary>
    [JsonPropertyName("refund_reason_type")]
    public int? RefundReasonType { get; set; }

    /// <summary>用户上传的凭证图片（选填）。</summary>
    [JsonPropertyName("media_list")]
    public List<WxaEcAftersaleMedia>? MediaList { get; set; }
}
