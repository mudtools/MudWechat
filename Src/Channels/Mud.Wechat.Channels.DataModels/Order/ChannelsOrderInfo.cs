// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「订单主文档」DTO（order/get、order/list/get、order/search、order/price/update、
// order/address/update、order/addressmodify/*、order/merchantnotes/update、order/presentnote/add、
// order/presentsuborder/get、order/preshipmentchangesku/*、order/blindboxitem/set 等核心端点）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>订单结构（官方 <c>order</c> 对象，<c>order/get</c> 响应主载荷）。</summary>
/// <remarks>
/// <para>
/// 字段名与官方文档一一对应（含已下线字段 sharer_info / sku_sharer_infos 与已废弃字段
/// present_order_id —— 照抄原文、不删不改名，守卫锁定）。<c>product_infos</c> 商品列表
/// 的完整结构见 <see cref="ChannelsOrderProductInfo"/>。
/// </para>
/// <para>
/// 订单状态枚举见 <see cref="ChannelsOrderStatuses"/>；礼物单类型（<c>present_send_type</c>）与
/// 本地生活订单预约状态（<c>voucher_order_status</c>）为官方枚举，以官方文档为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderInfo
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置订单详细数据信息（官方 <c>order_detail</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("order_detail")]
    public ChannelsOrderDetail? OrderDetail { get; set; }

    /// <summary>获取或设置售后信息（官方 <c>aftersale_detail</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("aftersale_detail")]
    public ChannelsOrderAftersaleDetail? AftersaleDetail { get; set; }

    /// <summary>获取或设置订单归属人身份标识（官方 <c>openid</c>；自购场景为支付者，礼物场景为收礼者）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置订单归属人在开放平台的唯一标识符（官方 <c>unionid</c>，需小店已绑定开放平台账号）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置是否礼物订单（官方 <c>is_present</c>）。</summary>
    [JsonPropertyName("is_present")]
    public bool? IsPresent { get; set; }

    /// <summary>获取或设置礼物 id（官方 <c>present_order_id</c>，【废弃】照抄原文）。</summary>
    [JsonPropertyName("present_order_id")]
    public long? PresentOrderId { get; set; }

    /// <summary>获取或设置礼物订单留言（官方 <c>present_note</c>）。</summary>
    [JsonPropertyName("present_note")]
    public string? PresentNote { get; set; }

    /// <summary>获取或设置礼物订单赠送者 openid（官方 <c>present_giver_openid</c>，仅送礼订单返回）。</summary>
    [JsonPropertyName("present_giver_openid")]
    public string? PresentGiverOpenId { get; set; }

    /// <summary>获取或设置礼物订单赠送者在开放平台的唯一标识符（官方 <c>present_giver_unionid</c>）。</summary>
    [JsonPropertyName("present_giver_unionid")]
    public string? PresentGiverUnionId { get; set; }

    /// <summary>获取或设置礼物订单 ID（官方 <c>present_order_id_str</c>，官方类型为 number，照抄）。</summary>
    [JsonPropertyName("present_order_id_str")]
    public long? PresentOrderIdStr { get; set; }

    /// <summary>获取或设置礼物单类型（官方 <c>present_send_type</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("present_send_type")]
    public int? PresentSendType { get; set; }

    /// <summary>获取或设置订单对应礼物单信息（官方 <c>order_present_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("order_present_info")]
    public ChannelsOrderPresentInfo? OrderPresentInfo { get; set; }

    /// <summary>获取或设置是否闪购订单（官方 <c>is_flash_sale_order</c>）。</summary>
    [JsonPropertyName("is_flash_sale_order")]
    public bool? IsFlashSaleOrder { get; set; }

    /// <summary>获取或设置同城单信息（官方 <c>intra_city_order_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("intra_city_order_info")]
    public ChannelsIntraCityOrderInfo? IntraCityOrderInfo { get; set; }

    /// <summary>获取或设置本地生活订单预约状态（官方 <c>voucher_order_status</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("voucher_order_status")]
    public int? VoucherOrderStatus { get; set; }

    /// <summary>获取或设置潮玩订单用户信息（官方 <c>user_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("user_info")]
    public ChannelsOrderUserInfo? UserInfo { get; set; }

    /// <summary>获取或设置订单风控信息（官方 <c>risk_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("risk_info")]
    public ChannelsOrderRiskInfo? RiskInfo { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsOrderProductInfo>? ProductInfos { get; set; }

    /// <summary>获取或设置支付信息（官方 <c>pay_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("pay_info")]
    public ChannelsOrderPayInfo? PayInfo { get; set; }

    /// <summary>获取或设置价格信息（官方 <c>price_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("price_info")]
    public ChannelsOrderPriceInfo? PriceInfo { get; set; }

    /// <summary>获取或设置配送信息（官方 <c>delivery_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("delivery_info")]
    public ChannelsOrderDeliveryInfo? DeliveryInfo { get; set; }

    /// <summary>获取或设置额外信息（官方 <c>ext_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsOrderExtInfo? ExtInfo { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>coupon_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsOrderCouponInfo? CouponInfo { get; set; }

    /// <summary>获取或设置分佣信息（官方 <c>commission_infos</c>）。</summary>
    [JsonPropertyName("commission_infos")]
    public List<ChannelsOrderCommissionInfo>? CommissionInfos { get; set; }

    /// <summary>获取或设置分享员信息（官方 <c>sharer_info</c>，【已经下线，不再维护】照抄原文）。</summary>
    [JsonPropertyName("sharer_info")]
    public ChannelsOrderSharerInfo? SharerInfo { get; set; }

    /// <summary>获取或设置结算信息（官方 <c>settle_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("settle_info")]
    public ChannelsOrderSettleInfo? SettleInfo { get; set; }

    /// <summary>获取或设置分享员信息（官方 <c>sku_sharer_infos</c>，【已经下线，不再维护】照抄原文）。</summary>
    [JsonPropertyName("sku_sharer_infos")]
    public List<ChannelsOrderSkuSharerInfo>? SkuSharerInfos { get; set; }

    /// <summary>获取或设置授权账号信息（官方 <c>agent_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("agent_info")]
    public ChannelsOrderAgentInfo? AgentInfo { get; set; }

    /// <summary>获取或设置订单来源信息（官方 <c>source_infos</c>）。</summary>
    [JsonPropertyName("source_infos")]
    public List<ChannelsOrderSourceInfo>? SourceInfos { get; set; }

    /// <summary>获取或设置订单退款信息（官方 <c>refund_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("refund_info")]
    public ChannelsOrderRefundInfo? RefundInfo { get; set; }

    /// <summary>获取或设置需代写的商品贺卡信息（官方 <c>greeting_card_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("greeting_card_info")]
    public ChannelsOrderGreetingCardInfo? GreetingCardInfo { get; set; }

    /// <summary>获取或设置商品定制信息（官方 <c>custom_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("custom_info")]
    public ChannelsOrderCustomInfo? CustomInfo { get; set; }

    /// <summary>获取或设置订单导购信息（官方 <c>guide_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("guide_info")]
    public ChannelsOrderGuideInfo? GuideInfo { get; set; }
}
