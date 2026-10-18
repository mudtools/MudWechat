// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 自定义交易组件订单对象（官方 <c>Order</c> 表，订单详情与三支订单列表共用的唯一订单模型）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本类型是订单族的「读侧超集」</b>：官方 <c>/shop/order/get</c> 的 Order 表列 20 支字段，
/// 而两支按推广员/分享员的列表页示例只回其中八支 ⇒ 未出现的键在本模型里恒为 <c>null</c>，
/// 宿主不得以「列表里没给」当作官方契约不存在该字段。
/// </para>
/// <para>
/// <b>状态十一值</b>见 <see cref="WxaShopOrderStatuses"/>；<c>11</c>（收银台支付完成）官方明写
/// 「自动流转，对商家来说和 10 同等对待即可」，<c>21</c>（部分发货）仅在发货侧 <c>finish_all_delivery=0</c> 时出现。
/// </para>
/// <para><b>敏感值</b>：<c>openid</c> 与 <see cref="AddressInfo"/> 属用户标识与收件人个人信息 ⇒ 不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrder
{
    /// <summary>order_id（官方类型 <c>number(uint64)</c>）：交易组件平台订单 ID。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>create_time（官方类型 <c>string</c>）：创建时间，<c>yyyy-MM-dd HH:mm:ss</c>；官方要求与微信服务器时差不得超过 5 秒。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>out_order_id（官方类型 <c>string</c>）：商家自定义订单 ID（大小写字母数字，长度小于 128 字符）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>openid（官方类型 <c>string</c>）：用户 openid（敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>scene（官方类型 <c>number</c>）：下单时小程序场景值。</summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：订单状态，取值见 <see cref="WxaShopOrderStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>path（官方类型 <c>string</c>）：订单详情页路径，用于微信侧订单中心跳转。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>order_detail（官方类型 <c>OrderDetail</c>）：订单详细数据（商品 / 价格 / 支付）。</summary>
    [JsonPropertyName("order_detail")]
    public WxaShopOrderDetail? OrderDetail { get; set; }

    /// <summary>delivery_detail（官方类型 <c>DeliveryDetail</c>）：配送信息。</summary>
    [JsonPropertyName("delivery_detail")]
    public WxaShopOrderDeliveryDetail? DeliveryDetail { get; set; }

    /// <summary>address_info（官方类型 <c>AddressInfo</c>）：收货地址信息（收件人姓名 / 手机号 / 详细地址，敏感）。</summary>
    [JsonPropertyName("address_info")]
    public WxaShopAddressInfo? AddressInfo { get; set; }

    /// <summary>default_receiving_address（官方类型 <c>AddressInfo</c>）：默认退货地址，与 <see cref="AddressInfo"/> 同形共用类型。</summary>
    [JsonPropertyName("default_receiving_address")]
    public WxaShopAddressInfo? DefaultReceivingAddress { get; set; }

    /// <summary>settlement_info（官方类型 <c>SettlementInfo</c>）：订单资金结算信息。</summary>
    [JsonPropertyName("settlement_info")]
    public WxaShopSettlementInfo? SettlementInfo { get; set; }

    /// <summary>refund_info（官方类型 <c>RefundInfo</c>）：订单退款信息。</summary>
    [JsonPropertyName("refund_info")]
    public WxaShopRefundInfo? RefundInfo { get; set; }

    /// <summary>related_aftersale_info（官方类型 <c>RelatedAfterSaleInfo</c>）：售后单信息（注意官方类名拼写为 AfterSale、字段名为 aftersale）。</summary>
    [JsonPropertyName("related_aftersale_info")]
    public WxaShopRelatedAfterSaleInfo? RelatedAftersaleInfo { get; set; }

    /// <summary>fund_type（官方类型 <c>number</c>）：订单类型，0 普通单、1 二级商户单；仅 1 允许调 <c>/shop/order/getpaymentparams</c>（官方 1010047）。</summary>
    [JsonPropertyName("fund_type")]
    public int? FundType { get; set; }

    /// <summary>expire_time（官方类型 <c>number</c>）：秒级时间戳的订单超时时间，取支付参数时用作 <c>prepay_id</c> 过期时间；到期后官方流转为 181。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>aftersale_duration（官方类型 <c>number</c>）：确认收货后多久禁止发起售后，单位天，需 ≥5，默认 5。</summary>
    [JsonPropertyName("aftersale_duration")]
    public int? AftersaleDuration { get; set; }

    /// <summary>promotion_info（官方类型 <c>PromotionInfo</c>）：推广员与分享员信息。</summary>
    [JsonPropertyName("promotion_info")]
    public WxaShopPromotionInfo? PromotionInfo { get; set; }

    /// <summary>commission_info（官方类型 <c>CommissionInfo</c>）：分账信息。</summary>
    [JsonPropertyName("commission_info")]
    public WxaShopCommissionInfo? CommissionInfo { get; set; }

    /// <summary>receive_time（官方类型 <c>number</c>）：确认收货时间（秒级时间戳），官方明写仅 <c>fund_type = 1</c> 且确认收货后存在。</summary>
    [JsonPropertyName("receive_time")]
    public long? ReceiveTime { get; set; }
}
