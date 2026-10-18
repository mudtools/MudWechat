// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 拉起收银台所需的支付参数（官方 <c>PaymentParams</c>；<c>/shop/order/getpaymentparams</c> 与
/// <c>/shop/pay/createorder</c> 两支应答共用同一类型）。
/// </summary>
/// <remarks>
/// <para><b>本类型是官方少见的「驼峰键名」区</b>：<c>timeStamp</c> / <c>nonceStr</c> / <c>paySign</c> / <c>signType</c>
/// 与 <c>package</c> 全部照官方原文钉住，不得按本仓 snake_case 惯例改写（守卫 SO5 逐条锁定五个字面量）。</para>
/// <para><b><c>package</c> 是保留字形态的键名</b>（值形如 <c>prepay_id=***</c>）⇒ 属性名写 <c>PackageValue</c>、JSON 名写 <c>package</c>。</para>
/// <para><b>类型口径陷阱</b>：官方 <c>/shop/order/getpaymentparams</c> 示例把 <c>timeStamp</c> 印成数字（1639124652），
/// 而 <c>/shop/pay/createorder</c> 的类型列印为 <c>string</c> ⇒ 本仓按类型列取 <see cref="string"/>，
/// 由源生成器把数字读成字符串的风险在 remarks 留档，宿主若拿到空串须自行按官方示例回读原始报文。</para>
/// <para><b>敏感值</b>：<c>paySign</c>（支付签名）与 <c>package</c>（内含 prepay_id）不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPaymentParams
{
    /// <summary>timeStamp（官方类型列 <c>string</c>，示例为数字）：秒级时间戳。</summary>
    [JsonPropertyName("timeStamp")]
    public string? Timestamp { get; set; }

    /// <summary>nonceStr（官方类型 <c>string</c>）：随机字符串，长度 32 字符以下。</summary>
    [JsonPropertyName("nonceStr")]
    public string? NonceStr { get; set; }

    /// <summary>package（官方类型 <c>string</c>）：统一下单返回的 prepay_id 参数值，格式 <c>prepay_id=***</c>（敏感，不得进日志）。</summary>
    [JsonPropertyName("package")]
    public string? PackageValue { get; set; }

    /// <summary>paySign（官方类型 <c>string</c>）：支付签名（敏感，不得进日志）。</summary>
    [JsonPropertyName("paySign")]
    public string? PaySign { get; set; }

    /// <summary>signType（官方类型 <c>string</c>）：签名算法（官方示例为 MD5）。</summary>
    [JsonPropertyName("signType")]
    public string? SignType { get; set; }
}
