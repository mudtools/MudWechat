// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「确认收货提醒」请求体（<c>POST /wxa/sec/order/notify_confirm_receive</c>）。
/// </summary>
/// <remarks>
/// <para><b>定位键四支全标选填、实为二选一</b>（官方注意事项「通过交易单号或商户号+商户单号来指定订单」）
/// ⇒ <c>transaction_id</c> 单用，或 <c>merchant_id</c> + <c>merchant_trade_no</c> 组合，
/// <c>sub_merchant_id</c> 仅在二级商户场景附加。</para>
/// <para><b>每个订单仅可调用一次</b>（官方原文）⇒ 非幂等，重试前须自行判重。</para>
/// <para><b><c>received_time</c> 是唯一强必填项</b>（官方标「是」）：签收时间由商户传入，必须晚于发货时间，
/// 且仅当物流类型为「物流快递」时才允许提醒。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_notifyconfirmreceive.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderConfirmReceiveRequest
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

    /// <summary>merchant_trade_no（官方类型 <c>string</c>）：商户系统内部订单号，只能是数字、大小写字母`_-*`且在同一个商户号下唯一。</summary>
    [JsonPropertyName("merchant_trade_no")]
    public string? MerchantTradeNo { get; set; }

    /// <summary>received_time（官方类型 <c>number</c>，必填）：快递签收时间，时间戳形式。</summary>
    [JsonPropertyName("received_time")]
    public long? ReceivedTime { get; set; }
}
