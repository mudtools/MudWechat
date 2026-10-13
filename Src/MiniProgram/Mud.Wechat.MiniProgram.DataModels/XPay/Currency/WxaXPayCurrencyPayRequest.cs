// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「扣减代币」请求体（<c>POST /xpay/currency_pay</c>）：对用户代币余额发起扣减。
/// </summary>
/// <remarks>
/// <para><b>官方参数表把七支字段全部标注为「非必填」</b>（本域唯一如此），但 <c>openid</c> / <c>amount</c> / <c>order_id</c> 缺失时官方按 <c>268490002</c>（请求参数字段错误）拒绝 ⇒ 可空性是官方表格口径，<b>不得</b>据此认为可以少传。</para>
/// <para><b><c>payitem</c> 是字符串而非对象数组</b>：官方说明给的形态 <c>[{productid, unit_price, quantity}]</c> 只是<b>该字符串的内容示例</b>，官方类型列仍为 <c>string</c> ⇒ 线格式照官方类型落 <c>string?</c>，序列化由调用方自证（守卫 XP5 锁定该处置）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_currency_pay.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayCurrencyPayRequest
{
    /// <summary>openid（官方类型 <c>string</c>）：用户的openid</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>env（官方类型 <c>number</c>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>user_ip（官方类型 <c>string</c>）：用户ip，例如:1.1.1.1</summary>
    [JsonPropertyName("user_ip")]
    public string? UserIp { get; set; }

    /// <summary>amount（官方类型 <c>number</c>）：支付的代币数量</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>）：订单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>payitem（官方类型 <c>string</c>）：物品信息。记录到账户流水中。如:[{productid:物品id, unit_price: 单价, quantity: 数量}]</summary>
    [JsonPropertyName("payitem")]
    public string? Payitem { get; set; }

    /// <summary>remark（官方类型 <c>string</c>）：备注</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}
