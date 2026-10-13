// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「支付单信息」（<c>get_order</c> 的 <c>order</c> 与 <c>get_order_list</c> 的 <c>order_list</c> 元素
/// 字段面逐字相同 ⇒ 两端点共用本类型）。
/// </summary>
/// <remarks>
/// <para><b><c>paid_amount</c> 单位是分</b>（官方原文「整型，单位：分钱」）⇒ 用 <see cref="long"/>，
/// 不得写成 <c>decimal</c> 再除 100 塞进 DTO。</para>
/// <para><b><c>description</c> 由官方截断</b>：以分号连接该支付单的所有商品描述，超过 120 字时自动截断并加省略号。</para>
/// <para><c>openid</c> 属用户级标识（禁止落日志、遥测或异常消息）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderInfo
{
    /// <summary>transaction_id（官方类型 <c>string</c>）：原支付交易对应的微信订单号。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>merchant_id（官方类型 <c>string</c>）：支付下单商户的商户号，由微信支付生成并下发。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

    /// <summary>sub_merchant_id（官方类型 <c>string</c>）：二级商户号。</summary>
    [JsonPropertyName("sub_merchant_id")]
    public string? SubMerchantId { get; set; }

    /// <summary>merchant_trade_no（官方类型 <c>string</c>）：商户系统内部订单号。</summary>
    [JsonPropertyName("merchant_trade_no")]
    public string? MerchantTradeNo { get; set; }

    /// <summary>description（官方类型 <c>string</c>）：以分号连接的该支付单的所有商品描述，当超过120字时自动截断并以 “...” 结尾。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>paid_amount（官方类型 <c>number</c>）：支付单实际支付金额，整型，单位：分钱。</summary>
    [JsonPropertyName("paid_amount")]
    public long? PaidAmount { get; set; }

    /// <summary>openid（官方类型 <c>string</c>）：支付者openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>trade_create_time（官方类型 <c>number</c>）：交易创建时间，时间戳形式。</summary>
    [JsonPropertyName("trade_create_time")]
    public long? TradeCreateTime { get; set; }

    /// <summary>pay_time（官方类型 <c>number</c>）：支付时间，时间戳形式。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }

    /// <summary>order_state（官方类型 <c>number</c>）：订单状态枚举：(1) 待发货；(2) 已发货；(3) 确认收货；(4) 交易完成；(5) 已退款；(6) 资金待结算。取值见 <see cref="WxaOrderStateValues"/></summary>
    [JsonPropertyName("order_state")]
    public long? OrderState { get; set; }

    /// <summary>in_complaint（官方类型 <c>boolean</c>）：是否处在交易纠纷中。</summary>
    [JsonPropertyName("in_complaint")]
    public bool? InComplaint { get; set; }

    /// <summary>shipping（官方类型 <c>object</c>）：发货信息。</summary>
    [JsonPropertyName("shipping")]
    public WxaOrderShippingState? Shipping { get; set; }
}
