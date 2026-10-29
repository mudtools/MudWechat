// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 支付信息（官方 <c>PayInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>敏感值</b>：<c>prepay_id</c> 与 <c>transaction_id</c> 属支付凭据 ⇒ 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderPayInfo
{
    /// <summary>支付方式（目前只有「微信支付」）。</summary>
    [JsonPropertyName("pay_method")]
    public string? PayMethod { get; set; }

    /// <summary>预支付 ID（敏感）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>预支付时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("prepay_time")]
    public string? PrepayTime { get; set; }

    /// <summary>支付时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("pay_time")]
    public string? PayTime { get; set; }

    /// <summary>支付订单号（敏感）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}
