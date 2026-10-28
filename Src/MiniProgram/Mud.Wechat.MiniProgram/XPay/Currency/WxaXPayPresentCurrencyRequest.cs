// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「代币赠送」请求体（<c>POST /xpay/present_currency</c>）：向用户赠送无价代币。
/// </summary>
/// <remarks>
/// <para><b>幂等语义由官方兜住</b>：同一 <c>order_id</c> 重复请求返回 <c>268490004</c>（官方原文「赠送和代币支付和充值广告金相关接口会返回，表示之前的操作已经成功」）⇒ 该错误码是「已生效」而非「失败」，重试逻辑必须把它当成功处理。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_present_currency.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayPresentCurrencyRequest
{
    /// <summary>openid（官方类型 <c>string</c>，<b>必填</b>）：用户的openid</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>，<b>必填</b>）：赠送单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>amount（官方类型 <c>number</c>，<b>必填</b>）：赠送金额</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}
