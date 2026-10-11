// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 售后族（售后单更新 / 纠纷单更新 / 保障单更新）。
// 事件键：channels_ec_aftersale_update、channels_ec_complaint_update、channels_ec_guarantee_update。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 售后单更新通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.AftersaleUpdate"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方矛盾照录</b>：<c>wxa_vip_discounted_price</c> 在官方参数表中为<b>顶层</b>字段
/// （与 <c>finder_shop_aftersale_status_update</c> 平级），而官方示例将其嵌在
/// <c>finder_shop_aftersale_status_update</c> 内 —— 按参数表建模为顶层字段。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/aftersale_callback/channels_ec_aftersale_update.html">售后单更新通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.AftersaleUpdate })]
public sealed class ChannelsAftersaleUpdatePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置固定的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_aftersale_update</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>finder_shop_aftersale_status_update</c>）。</summary>
    [JsonPropertyName("finder_shop_aftersale_status_update")]
    public ChannelsAftersaleStatusInfo? AftersaleStatusUpdate { get; set; }

    /// <summary>获取或设置小程序会员已优惠金额，单位分（官方 <c>wxa_vip_discounted_price</c>，参数表为顶层字段、官方示例嵌在事件主体内，按参数表建模）。</summary>
    [JsonPropertyName("wxa_vip_discounted_price")]
    public long? WxaVipDiscountedPrice { get; set; }
}

/// <summary>售后单更新事件主体（官方 <c>finder_shop_aftersale_status_update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsAftersaleStatusInfo
{
    /// <summary>
    /// 获取或设置售后单状态（官方 <c>status</c>，字符串枚举）：USER_CANCELD 用户取消申请；
    /// MERCHANT_PROCESSING 商家受理中；MERCHANT_REJECT_REFUND 商家拒绝退款；MERCHANT_REJECT_RETURN 商家拒绝退货退款；
    /// USER_WAIT_RETURN 待买家退货；RETURN_CLOSED 退货退款关闭；MERCHANT_WAIT_RECEIPT 待商家收货；
    /// MERCHANT_OVERDUE_REFUND 商家逾期未退款；MERCHANT_REFUND_SUCCESS 退款完成；MERCHANT_RETURN_SUCCESS 退货退款完成；
    /// PLATFORM_REFUNDING 平台退款中；PLATFORM_REFUND_FAIL 平台退款失败；USER_WAIT_CONFIRM 待用户确认；
    /// MERCHANT_REFUND_RETRY_FAIL 商家打款失败，客服关闭售后；MERCHANT_FAIL 售后关闭；
    /// USER_WAIT_CONFIRM_UPDATE 待用户处理商家协商；USER_WAIT_HANDLE_MERCHANT_AFTER_SALE 待用户处理商家代发起的售后申请；
    /// WAIT_PACKAGE_INTERCEPT 物流线上拦截中；MERCHANT_REJECT_EXCHANGE 商家拒绝换货；MERCHANT_REJECT_RESHIP 商家拒绝发货；
    /// USER_WAIT_RECEIPT 待用户收货；MERCHANT_EXCHANGE_SUCCESS 换货完成；PLATFORM_PROCESSING 平台审核中。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置小店售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置小店订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 纠纷单更新通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ComplaintUpdate"/>）。
/// </summary>
/// <remarks>
/// <para>微信小店将纠纷单更新的消息通知给商户。应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/aftersale_callback/channels_ec_complaint_update.html">纠纷单更新通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ComplaintUpdate })]
public sealed class ChannelsComplaintUpdatePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置固定的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_complaint_update</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置纠纷单状态主体（官方 <c>finder_shop_complaint</c>）。</summary>
    [JsonPropertyName("finder_shop_complaint")]
    public ChannelsComplaintStatusInfo? Complaint { get; set; }
}

/// <summary>纠纷单状态事件主体（官方 <c>finder_shop_complaint</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsComplaintStatusInfo
{
    /// <summary>
    /// 获取或设置纠纷单状态（官方 <c>complaint_status</c>，数字枚举）：100 待商家处理纠纷；101 待客服处理；
    /// 102 取消客服介入；103 客服处理中；104 待用户补充凭证；105 用户已补充凭证；106 待商家补充凭证；
    /// 107 商家已补充凭证；108 待双方补充凭证；109 双方补充凭证超时；110 待商家确认；111 商家申诉中；
    /// 112 调解完成；113 客服核实；114 重新退款中；115 退款核实完成；116 调解关闭；
    /// 305 用户补充凭证超时；307 商家补充凭证超时。
    /// </summary>
    [JsonPropertyName("complaint_status")]
    public long? ComplaintStatus { get; set; }

    /// <summary>获取或设置小店售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置纠纷单号（官方 <c>complaint_id</c>）。</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>获取或设置小店订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 保障单更新通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.GuaranteeUpdate"/>）。
/// </summary>
/// <remarks>
/// <para>微信小店将保障单更新的消息通知给商户。应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/aftersale_callback/channels_ec_guarantee_update.html">保障单更新通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.GuaranteeUpdate })]
public sealed class ChannelsGuaranteeUpdatePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置固定的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_guarantee_update</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>finder_shop_guarantee_status_update</c>）。</summary>
    [JsonPropertyName("finder_shop_guarantee_status_update")]
    public ChannelsGuaranteeStatusInfo? GuaranteeStatusUpdate { get; set; }
}

/// <summary>保障单状态事件主体（官方 <c>finder_shop_guarantee_status_update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGuaranteeStatusInfo
{
    /// <summary>
    /// 获取或设置保障单状态（官方 <c>status</c>，字符串枚举）：STATUS_WAIT_MERCHANT_HANDLE 等待商家处理；
    /// STATUS_WAIT_PLATFORM_HANDLE 等待平台处理；STATUS_WAIT_USER_CONFIRM 等待用户确认；
    /// STATUS_WAIT_MERCHANT_PROOF 等待商家举证；STATUS_WAIT_USER_PROOF 等待用户举证；
    /// STATUS_WAIT_BOTH_PROOF 等待双方举证；STATUS_WAIT_OP_COMFIRM 等待 OP 确认（官方拼写 COMFIRM 照抄）；
    /// STATUS_WAIT_PAYSCORE_DONE 等待支付分付款；STATUS_NO_NEED_PAY 无需赔付；STATUS_PAYING 赔付中；
    /// STATUS_PAY_BLOCK 假一赔四金额异常，待确认；STATUS_PAY_SUCC 赔付成功；STATUS_PAY_FAIL 赔付失败；
    /// STATUS_USER_CANCEL 用户取消申请。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置小店保障单号（官方 <c>guarantee_order_id</c>）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public string? GuaranteeOrderId { get; set; }

    /// <summary>获取或设置小店订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
