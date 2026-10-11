// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 订单族（下单 / 取消 / 支付 / 待发货 / 发货 / 确认收货 / 结算 / 其他信息更新）。
// 事件键：channels_ec_order_*（注意：其他信息更新的 Event 值为 channels_ec_order_ext_info_update，
// 其官方页面文件名为 channels_ec_order_ext_info.html）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 订单下单回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderNew"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 应答：回复 <c>success</c> 或空字符串（无需加密）。重试：接收方未响应（最长等待 5 秒）即视为推送失败，
/// 重试策略 0s/0s/10s/1min/5min/30min/60min/60min/60min，最多推送 10 次，总计约 3.6h（订单族各页一致）。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_new.html">订单下单</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderNew })]
public sealed class ChannelsOrderNewPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_new</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderBasicInfo? OrderInfo { get; set; }
}

/// <summary>
/// 订单取消回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderCancel"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_cancel.html">订单取消</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderCancel })]
public sealed class ChannelsOrderCancelPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_cancel</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderCancelInfo? OrderInfo { get; set; }
}

/// <summary>订单取消事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderCancelInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置取消类型（官方 <c>cancel_type</c>）：1 用户取消；2 超时取消；3 全部商品售后完成，订单取消。</summary>
    [JsonPropertyName("cancel_type")]
    public long? CancelType { get; set; }
}

/// <summary>
/// 订单支付成功回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderPay"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_pay.html">订单支付成功</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderPay })]
public sealed class ChannelsOrderPayPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_pay</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderPayInfo? OrderInfo { get; set; }
}

/// <summary>订单支付成功事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderPayInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置支付时间，秒级时间戳（官方 <c>pay_time</c>）。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }
}

/// <summary>
/// 订单待发货回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderWaitShipping"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_wait_shipping.html">订单待发货</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderWaitShipping })]
public sealed class ChannelsOrderWaitShippingPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_wait_shipping</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>；与「订单下单」字段集相同，共用 <see cref="ChannelsOrderBasicInfo"/>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderBasicInfo? OrderInfo { get; set; }
}

/// <summary>
/// 订单发货回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderDeliver"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_deliver.html">订单发货</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderDeliver })]
public sealed class ChannelsOrderDeliverPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_deliver</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderDeliverInfo? OrderInfo { get; set; }
}

/// <summary>订单发货事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderDeliverInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置发货完成标记（官方 <c>finish_delivery</c>）：0 尚未全部发货；1 全部商品发货完成。</summary>
    [JsonPropertyName("finish_delivery")]
    public long? FinishDelivery { get; set; }
}

/// <summary>
/// 订单确认收货回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderConfirm"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_confirm.html">订单确认收货</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderConfirm })]
public sealed class ChannelsOrderConfirmPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_confirm</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderConfirmInfo? OrderInfo { get; set; }
}

/// <summary>订单确认收货事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderConfirmInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置确认类型（官方 <c>confirm_type</c>）：1 用户确认收货；2 超时自动确认收货。</summary>
    [JsonPropertyName("confirm_type")]
    public long? ConfirmType { get; set; }
}

/// <summary>
/// 订单结算成功回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderSettle"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_settle.html">订单结算成功</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderSettle })]
public sealed class ChannelsOrderSettlePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_settle</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderSettleInfo? OrderInfo { get; set; }
}

/// <summary>订单结算成功事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderSettleInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置结算成功时间，秒级时间戳（官方 <c>settle_time</c>）。</summary>
    [JsonPropertyName("settle_time")]
    public long? SettleTime { get; set; }
}

/// <summary>
/// 订单其他信息更新回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderExtInfoUpdate"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 微信小店将订单其他信息更新通知给商户（联盟佣金、地址修改、备注、催发货、换 sku、改价、代发、
/// 发货协商、预约发货、限制发货等，以 <c>order_info.type</c> 区分）。
/// </para>
/// <para>
/// <b>键名说明</b>：官方页面文件名为 <c>channels_ec_order_ext_info.html</c>，Event 实际值为
/// <c>channels_ec_order_ext_info_update</c>（以参数表为准）。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）；重试策略同订单族（最多 10 次，约 3.6h）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/order_callback/channels_ec_order_ext_info.html">订单其他信息更新</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderExtInfoUpdate })]
public sealed class ChannelsOrderExtInfoUpdatePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置订单用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_ext_info_update</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsOrderExtInfoUpdateInfo? OrderInfo { get; set; }
}

/// <summary>订单其他信息更新事件主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderExtInfoUpdateInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>
    /// 获取或设置更新类型（官方 <c>type</c>）：1 联盟佣金信息；2 商家主动地址修改或通过用户修改地址申请；
    /// 3 商家备注修改；4 用户发起申请修改收货地址（特殊条件下需商家审批）；5 订单虚拟号码信息更新；
    /// 6 分享员信息更新；7 用户催发货；8 商家修改礼物单备注；9 用户发起发货前更换 sku 请求；
    /// 10 商家同意用户发货前更换 sku 请求；11 用户收礼时更换 sku；12 商家修改订单价格；13 真实号审核结果；
    /// 14 分配代发；15 取消代发；16 买家已同意修改，请按新地址发货；17 买家已拒绝修改，请按原地址发货；
    /// 18 买家超时未确认，修改申请已失效；19 买家已主动修改地址，请按新地址发货；20 该订单已发货，修改申请已失效；
    /// 21 用户同意发货协商申请；22 用户拒绝发货协商申请；23 发货协商超时；24 分配供货单；
    /// 25 用户预约具体发货时间；26 用户预约暂不发货；27 限制发货；28 取消限制发货。
    /// </summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}

/// <summary>
/// 订单事件基础主体（官方 <c>order_info</c>，仅 <c>order_id</c> 单字段）。
/// </summary>
/// <remarks>「订单下单」与「订单待发货」两事件的 <c>order_info</c> 字段集相同（仅 order_id），按工作单共用本类。</remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsOrderBasicInfo
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>；官方示例 3705115058471208928 超 int32，建模 long）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }
}
