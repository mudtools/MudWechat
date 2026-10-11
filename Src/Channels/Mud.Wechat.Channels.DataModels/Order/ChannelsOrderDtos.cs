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

/// <summary>
/// 订单状态（官方 <c>order.status</c> / 列表与搜索 <c>status</c> 过滤枚举）。
/// </summary>
public static class ChannelsOrderStatuses
{
    /// <summary>已下单（订单创建成功、等待支付）。</summary>
    public const int Placed = 5;

    /// <summary>待付款（用户下单成功，等待支付）。</summary>
    public const int PendingPayment = 10;

    /// <summary>待发货（已付款，等待商家发货）。</summary>
    public const int PendingDelivery = 20;

    /// <summary>部分发货（订单内部分商品已发货）。</summary>
    public const int PartiallyDelivered = 21;

    /// <summary>待收货（全部商品已发货，等待买家确认收货）。</summary>
    public const int PendingReceipt = 30;

    /// <summary>完成（订单交易完成）。</summary>
    public const int Completed = 100;

    /// <summary>全部商品售后完成之后，订单取消。</summary>
    public const int CancelledAfterAftersale = 200;

    /// <summary>用户取消（未支付、买家主动取消）。</summary>
    public const int UserCancelled = 250;
}

/// <summary>订单 ID 请求（order 域单订单查询类端点共用：get / deliverynegotiation/submit / virtualnumber 等）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderIdRequest
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>获取订单详情（<c>order/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetOrderRequest
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填，可通过 获取订单列表 接口获取）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>获取订单详情（<c>order/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置订单结构（官方 <c>order</c>）。</summary>
    [JsonPropertyName("order")]
    public ChannelsOrderInfo? Order { get; set; }
}

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

/// <summary>订单商品（官方 <c>product_infos</c> 数组元素，<c>order/get</c>）。</summary>
/// <remarks>
/// 商品维度优惠字段（merchant_discounted_price / finder_discounted_price / vip_discounted_price /
/// bulkbuy / 国补 / 平台券 / 限时抢购 / 点赞买 / 小店金币抵扣等）均照官方原文建模；
/// sku_aftersale 相关（on_aftersale_sku_cnt / finish_aftersale_sku_cnt）只计数量、明细以售后域为准。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置售卖单价（官方 <c>sale_price</c>，单位为分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 数量（官方 <c>sku_cnt</c>）。</summary>
    [JsonPropertyName("sku_cnt")]
    public long? SkuCnt { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置正在售后/退款流程中的 sku 数量（官方 <c>on_aftersale_sku_cnt</c>）。</summary>
    [JsonPropertyName("on_aftersale_sku_cnt")]
    public long? OnAftersaleSkuCnt { get; set; }

    /// <summary>获取或设置完成售后/退款的 sku 数量（官方 <c>finish_aftersale_sku_cnt</c>）。</summary>
    [JsonPropertyName("finish_aftersale_sku_cnt")]
    public long? FinishAftersaleSkuCnt { get; set; }

    /// <summary>获取或设置 sku 编码（官方 <c>sku_code</c>，商家自定义编码）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置市场单价（官方 <c>market_price</c>，单位为分）。</summary>
    [JsonPropertyName("market_price")]
    public long? MarketPrice { get; set; }

    /// <summary>获取或设置 sku 属性列表（官方 <c>sku_attrs</c>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsOrderSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置 sku 实付总价（官方 <c>real_price</c>，取 estimate_price 和 change_price 中较小值）。</summary>
    [JsonPropertyName("real_price")]
    public long? RealPrice { get; set; }

    /// <summary>获取或设置商品外部 spu id（官方 <c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品外部 sku id（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置是否有商家优惠金额（官方 <c>is_discounted</c>，非必填默认为 false）。</summary>
    [JsonPropertyName("is_discounted")]
    public bool? IsDiscounted { get; set; }

    /// <summary>获取或设置使用所有优惠后 sku 总价（官方 <c>estimate_price</c>，有优惠金额时必填）。</summary>
    [JsonPropertyName("estimate_price")]
    public long? EstimatePrice { get; set; }

    /// <summary>获取或设置是否修改过价格（官方 <c>is_change_price</c>，非必填默认为 false）。</summary>
    [JsonPropertyName("is_change_price")]
    public bool? IsChangePrice { get; set; }

    /// <summary>获取或设置改价后 sku 总价（官方 <c>change_price</c>，is_change_price 为 true 时有值）。</summary>
    [JsonPropertyName("change_price")]
    public long? ChangePrice { get; set; }

    /// <summary>获取或设置区域库存 id（官方 <c>out_warehouse_id</c>）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置商品发货信息（官方 <c>sku_deliver_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsOrderSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置商品额外服务信息（官方 <c>extra_service</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsOrderExtraService? ExtraService { get; set; }

    /// <summary>获取或设置是否使用了会员积分抵扣（官方 <c>use_deduction</c>）。</summary>
    [JsonPropertyName("use_deduction")]
    public bool? UseDeduction { get; set; }

    /// <summary>获取或设置会员积分抵扣金额（官方 <c>deduction_price</c>，单位为分）。</summary>
    [JsonPropertyName("deduction_price")]
    public long? DeductionPrice { get; set; }

    /// <summary>获取或设置本地生活券码信息（官方 <c>voucher_list</c>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsOrderVoucher>? VoucherList { get; set; }

    /// <summary>获取或设置商品优惠券信息（官方 <c>order_product_coupon_info_list</c>，逐步替换 order.order_detail.coupon_info）。</summary>
    [JsonPropertyName("order_product_coupon_info_list")]
    public List<ChannelsOrderProductCouponInfo>? OrderProductCouponInfoList { get; set; }

    /// <summary>获取或设置商品发货时效（官方 <c>delivery_deadline</c>，超时此时间未发货即为发货超时）。</summary>
    [JsonPropertyName("delivery_deadline")]
    public long? DeliveryDeadline { get; set; }

    /// <summary>获取或设置商家优惠金额（官方 <c>merchant_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("merchant_discounted_price")]
    public long? MerchantDiscountedPrice { get; set; }

    /// <summary>获取或设置达人优惠金额（官方 <c>finder_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("finder_discounted_price")]
    public long? FinderDiscountedPrice { get; set; }

    /// <summary>获取或设置是否赠品（官方 <c>is_free_gift</c>，非必填，1:是赠品）。</summary>
    [JsonPropertyName("is_free_gift")]
    public int? IsFreeGift { get; set; }

    /// <summary>获取或设置订单内商品维度会员权益优惠金额（官方 <c>vip_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("vip_discounted_price")]
    public long? VipDiscountedPrice { get; set; }

    /// <summary>获取或设置商品常量编号（官方 <c>product_unique_id</c>，订单内商品唯一标识，下单后不会变化）。</summary>
    [JsonPropertyName("product_unique_id")]
    public string? ProductUniqueId { get; set; }

    /// <summary>获取或设置收礼后发货前更换 sku 信息（官方 <c>change_sku_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("change_sku_info")]
    public ChannelsOrderChangeSkuInfo? ChangeSkuInfo { get; set; }

    /// <summary>获取或设置赠品信息（官方 <c>free_gift_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("free_gift_info")]
    public ChannelsOrderFreeGiftInfo? FreeGiftInfo { get; set; }

    /// <summary>获取或设置订单内商品维度一起买优惠金额（官方 <c>bulkbuy_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("bulkbuy_discounted_price")]
    public long? BulkbuyDiscountedPrice { get; set; }

    /// <summary>获取或设置订单内商品维度国补优惠金额（官方 <c>national_subsidy_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("national_subsidy_discounted_price")]
    public long? NationalSubsidyDiscountedPrice { get; set; }

    /// <summary>获取或设置代发单相关信息（官方 <c>dropship_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("dropship_info")]
    public ChannelsOrderDropshipInfo? DropshipInfo { get; set; }

    /// <summary>获取或设置是否闪购商品（官方 <c>is_flash_sale</c>）。</summary>
    [JsonPropertyName("is_flash_sale")]
    public bool? IsFlashSale { get; set; }

    /// <summary>获取或设置订单内商品维度地方补贴优惠金额（官方 <c>national_subsidy_merchant_discounted_price</c>，商家出资，单位为分）。</summary>
    [JsonPropertyName("national_subsidy_merchant_discounted_price")]
    public long? NationalSubsidyMerchantDiscountedPrice { get; set; }

    /// <summary>获取或设置订单内商品维度活动商家补贴（官方 <c>platform_activity_merchant_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("platform_activity_merchant_discounted_price")]
    public long? PlatformActivityMerchantDiscountedPrice { get; set; }

    /// <summary>获取或设置订单内商品维度平台券优惠金额（官方 <c>cash_coupon_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("cash_coupon_discounted_price")]
    public long? CashCouponDiscountedPrice { get; set; }

    /// <summary>获取或设置限时抢购优惠金额（官方 <c>limited_discount_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("limited_discount_discounted_price")]
    public long? LimitedDiscountDiscountedPrice { get; set; }

    /// <summary>获取或设置商家点赞买优惠金额（官方 <c>normal_friends_like_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("normal_friends_like_discounted_price")]
    public long? NormalFriendsLikeDiscountedPrice { get; set; }

    /// <summary>获取或设置供货单相关信息（官方 <c>supply_order_product_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("supply_order_product_info")]
    public ChannelsOrderSupplyOrderProductInfo? SupplyOrderProductInfo { get; set; }

    /// <summary>获取或设置商品定制信息（官方 <c>custom_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("custom_info")]
    public ChannelsOrderCustomInfo? CustomInfo { get; set; }

    /// <summary>获取或设置订单内商品维度商家实收金额（官方 <c>sku_merchant_receive_amount</c>，含平台补贴分摊，单位为分）。</summary>
    [JsonPropertyName("sku_merchant_receive_amount")]
    public long? SkuMerchantReceiveAmount { get; set; }

    /// <summary>获取或设置收礼更换 sku 相关信息（官方 <c>accept_change_sku_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("accept_change_sku_info")]
    public ChannelsOrderAcceptChangeSkuInfo? AcceptChangeSkuInfo { get; set; }

    /// <summary>获取或设置订单中同一 sku_id 下每件商品的具体信息（官方 <c>order_product_item_list</c>）。</summary>
    [JsonPropertyName("order_product_item_list")]
    public List<ChannelsOrderProductItem>? OrderProductItemList { get; set; }

    /// <summary>获取或设置本地生活信息（官方 <c>voucher_product_info</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("voucher_product_info")]
    public ChannelsOrderVoucherProductInfo? VoucherProductInfo { get; set; }

    /// <summary>获取或设置商品代发信息列表（官方 <c>dropship_info_list</c>，拆件分配场景每份对应一条）。</summary>
    [JsonPropertyName("dropship_info_list")]
    public List<ChannelsOrderDropshipInfo>? DropshipInfoList { get; set; }

    /// <summary>获取或设置订单内商品维度小店金币抵扣优惠金额（官方 <c>point_discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("point_discounted_price")]
    public long? PointDiscountedPrice { get; set; }

    /// <summary>获取或设置属性键（官方 <c>attr_key</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string? AttrKey { get; set; }

    /// <summary>
    /// 获取或设置属性值 value（官方 <c>attr_value</c>，属性自定义用）。
    /// </summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }
}

/// <summary>订单商品 sku 属性（官方 <c>product_infos[].sku_attrs</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSkuAttr
{
    /// <summary>获取或设置属性键（官方 <c>attr_key</c>）。</summary>
    [JsonPropertyName("attr_key")]
    public string? AttrKey { get; set; }

    /// <summary>获取或设置属性值（官方 <c>attr_value</c>）。</summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }
}

/// <summary>订单商品发货信息（官方 <c>product_infos[].sku_deliver_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSkuDeliverInfo
{
    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>，0 快递、1 无需快递）。</summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }

    /// <summary>获取或设置快递件数（官方 <c>deliver_type</c>）。</summary>
    [JsonPropertyName("deliver_type")]
    public int? DeliverType { get; set; }
}

/// <summary>商品额外服务信息（官方 <c>product_infos[].extra_service</c>，为将来扩展预留的结构）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderExtraService
{
}

/// <summary>本地生活券码信息（官方 <c>product_infos[].voucher_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderVoucher
{
    /// <summary>获取或设置券码（官方 <c>code</c>，结构以官方订单文档为准）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置券码状态（官方 <c>status</c>，枚举值以官方订单文档为准）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置核销时间（官方 <c>verify_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("verify_time")]
    public long? VerifyTime { get; set; }
}

/// <summary>商品优惠券信息（官方 <c>product_infos[].order_product_coupon_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderProductCouponInfo
{
    /// <summary>获取或设置优惠券 id（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券名称（官方 <c>coupon_name</c>）。</summary>
    [JsonPropertyName("coupon_name")]
    public string? CouponName { get; set; }

    /// <summary>获取或设置优惠金额（官方 <c>discount_fee</c>，单位为分）。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }
}

/// <summary>订单详细数据信息（官方 <c>order.order_detail</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDetail
{
    /// <summary>获取或设置商品优惠券信息（官方 <c>coupon_info</c>，逐步被 product_infos[].order_product_coupon_info_list 替换）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsOrderCouponInfo? CouponInfo { get; set; }
}

/// <summary>订单售后信息（官方 <c>order.aftersale_detail</c>，结构以官方售后域文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderAftersaleDetail
{
    /// <summary>获取或设置售后单 ID（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public string? AftersaleId { get; set; }

    /// <summary>获取或设置售后单状态（官方 <c>aftersale_status</c>，枚举值以官方售后域文档为准）。</summary>
    [JsonPropertyName("aftersale_status")]
    public int? AftersaleStatus { get; set; }
}

/// <summary>订单对应礼物单信息（官方 <c>order.order_present_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderPresentInfo
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }
}

/// <summary>同城单信息（官方 <c>order.intra_city_order_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsIntraCityOrderInfo
{
}

/// <summary>潮玩订单用户信息（官方 <c>order.user_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderUserInfo
{
}

/// <summary>订单风控信息（官方 <c>order.risk_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderRiskInfo
{
}

/// <summary>支付信息（官方 <c>order.pay_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderPayInfo
{
    /// <summary>获取或设置预支付单号（官方 <c>prepay_id</c>）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>获取或设置支付单号（官方 <c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置支付时间（官方 <c>pay_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }
}

/// <summary>价格信息（官方 <c>order.price_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderPriceInfo
{
    /// <summary>获取或设置商品总价（官方 <c>product_price</c>，单位为分）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>获取或设置订单总价（官方 <c>order_price</c>，单位为分）。</summary>
    [JsonPropertyName("order_price")]
    public long? OrderPrice { get; set; }

    /// <summary>获取或设置运费（官方 <c>freight</c>，单位为分）。</summary>
    [JsonPropertyName("freight")]
    public long? Freight { get; set; }

    /// <summary>获取或设置折扣金额（官方 <c>discounted_price</c>，单位为分）。</summary>
    [JsonPropertyName("discounted_price")]
    public long? DiscountedPrice { get; set; }

    /// <summary>获取或设置是否改价（官方 <c>is_change_price</c>）。</summary>
    [JsonPropertyName("is_change_price")]
    public bool? IsChangePrice { get; set; }

    /// <summary>获取或设置改价后价格变动金额（官方 <c>changed_price</c>，单位为分）。</summary>
    [JsonPropertyName("changed_price")]
    public long? ChangedPrice { get; set; }
}

/// <summary>配送信息（官方 <c>order.delivery_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDeliveryInfo
{
    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>，枚举值以官方订单文档为准）。</summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }

    /// <summary>获取或设置配送商品信息列表（官方 <c>delivery_product_info</c>）。</summary>
    [JsonPropertyName("delivery_product_info")]
    public List<ChannelsOrderDeliveryProductInfo>? DeliveryProductInfo { get; set; }
}

/// <summary>订单配送商品信息（官方 <c>delivery_info.delivery_product_info</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDeliveryProductInfo
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，非主流快递公司可填 OTHER）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置包裹中的商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsDeliveryProductInfo>? ProductInfos { get; set; }
}

/// <summary>包裹商品信息（官方 <c>delivery_product_info.product_infos</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsDeliveryProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}

/// <summary>额外信息（官方 <c>order.ext_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderExtInfo
{
}

/// <summary>优惠券信息（官方 <c>order.coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderCouponInfo
{
    /// <summary>获取或设置优惠券 id（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置优惠类型（官方 <c>type</c>，枚举值以官方订单文档为准）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置优惠金额（官方 <c>discount_fee</c>，单位为分）。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }
}

/// <summary>分佣信息（官方 <c>order.commission_infos</c> 数组元素，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderCommissionInfo
{
    /// <summary>获取或设置分佣商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置分佣金额（官方 <c>commission</c>，单位为分）。</summary>
    [JsonPropertyName("commission")]
    public long? Commission { get; set; }
}

/// <summary>分享员信息（官方 <c>order.sharer_info</c>，【已经下线，不再维护】照抄原文）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSharerInfo
{
}

/// <summary>结算信息（官方 <c>order.settle_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSettleInfo
{
    /// <summary>获取或设置订单收入（官方 <c>revenue</c>，单位为分）。</summary>
    [JsonPropertyName("revenue")]
    public long? Revenue { get; set; }

    /// <summary>获取或设置商品退款（官方 <c>product_refund</c>，单位为分）。</summary>
    [JsonPropertyName("product_refund")]
    public long? ProductRefund { get; set; }

    /// <summary>获取或设置运费退款（官方 <c>freight_refund</c>，单位为分）。</summary>
    [JsonPropertyName("freight_refund")]
    public long? FreightRefund { get; set; }
}

/// <summary>分享员信息（官方 <c>order.sku_sharer_infos</c> 数组元素，【已经下线，不再维护】照抄原文）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSkuSharerInfo
{
}

/// <summary>授权账号信息（官方 <c>order.agent_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderAgentInfo
{
}

/// <summary>订单来源信息（官方 <c>order.source_infos</c> 数组元素，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSourceInfo
{
    /// <summary>获取或设置订单来源（官方 <c>source</c>，枚举值以官方订单文档为准）。</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }
}

/// <summary>订单退款信息（官方 <c>order.refund_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderRefundInfo
{
    /// <summary>获取或设置退款金额（官方 <c>refund_fee</c>，单位为分）。</summary>
    [JsonPropertyName("refund_fee")]
    public long? RefundFee { get; set; }
}

/// <summary>需代写的商品贺卡信息（官方 <c>order.greeting_card_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderGreetingCardInfo
{
}

/// <summary>商品定制信息（官方 <c>order.custom_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderCustomInfo
{
}

/// <summary>订单导购信息（官方 <c>order.guide_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderGuideInfo
{
}

/// <summary>订单收礼后发货前更换 sku 信息（官方 <c>product_infos[].change_sku_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderChangeSkuInfo
{
    /// <summary>获取或设置换款后的商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置换款后的 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }
}

/// <summary>赠品信息（官方 <c>product_infos[].free_gift_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderFreeGiftInfo
{
    /// <summary>获取或设置赠品商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置赠品数量（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}

/// <summary>代发单相关信息（官方 <c>product_infos[].dropship_info</c>，结构以官方代发域文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDropshipInfo
{
}

/// <summary>供货单相关信息（官方 <c>product_infos[].supply_order_product_info</c>，结构以官方供货域文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderSupplyOrderProductInfo
{
}

/// <summary>收礼更换 sku 相关信息（官方 <c>product_infos[].accept_change_sku_info</c>，结构以官方订单文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderAcceptChangeSkuInfo
{
}

/// <summary>订单中同一 sku_id 下每件商品的具体信息（官方 <c>product_infos[].order_product_item_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderProductItem
{
    /// <summary>获取或设置某件商品的常量编号（官方 <c>item_unique_id</c>）。</summary>
    [JsonPropertyName("item_unique_id")]
    public string? ItemUniqueId { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }
}

/// <summary>本地生活信息（官方 <c>product_infos[].voucher_product_info</c>，结构以官方本地生活文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderVoucherProductInfo
{
}

/// <summary>时间范围（官方 <c>order/list/get</c> 的 <c>create_time_range</c>/<c>update_time_range</c> 与
/// <c>userbooking/list</c> 的 <c>time_range_order_create</c>/<c>time_range_booking_predict</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderTimeRange
{
    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，秒级时间戳，与 start_time 间隔不可超过 7 天）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

/// <summary>获取订单列表（<c>order/list/get</c>）请求体。</summary>
/// <remarks>官方要求 create_time_range / update_time_range 两个时间范围<b>至少填一个</b>（守卫锁定）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetOrderListRequest
{
    /// <summary>获取或设置订单创建时间范围（官方 <c>create_time_range</c>）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsOrderTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置订单更新时间范围（官方 <c>update_time_range</c>）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsOrderTimeRange? UpdateTimeRange { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置买家身份标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，上一页请求返回）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>订单号列表响应（<c>order/list/get</c> 与 <c>order/search</c> 共用响应形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderIdListResponse : ChannelsResponse
{
    /// <summary>获取或设置订单号列表（官方 <c>order_id_list</c>）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<string>? OrderIdList { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，下一页请求返回）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}

/// <summary>订单搜索条件（官方 <c>order/search</c> 的 <c>search_condition</c> 对象）。</summary>
/// <remarks>官方要求「搜索条件下的参数必须至少设置一个字段，否则接口报错」（守卫锁定）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSearchOrderCondition
{
    /// <summary>获取或设置售后存在性过滤（官方 <c>on_aftersale_order_exist</c>：0=无售后订单，1=有售后订单）。</summary>
    [JsonPropertyName("on_aftersale_order_exist")]
    public int? OnAftersaleOrderExist { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置商品标题关键词（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置收件人（官方 <c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>
    /// 获取或设置收件人电话（官方 <c>tel_number</c>，【已废弃】请使用 <see cref="TelNumberLast4"/>）。
    /// </summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，只搜一个订单时使用）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置买家备注（官方 <c>customer_notes</c>）。</summary>
    [JsonPropertyName("customer_notes")]
    public string? CustomerNotes { get; set; }

    /// <summary>获取或设置收件人电话后四位（官方 <c>tel_number_last4</c>）。</summary>
    [JsonPropertyName("tel_number_last4")]
    public string? TelNumberLast4 { get; set; }

    /// <summary>获取或设置申请修改地址审核中（官方 <c>address_under_review</c>）。</summary>
    [JsonPropertyName("address_under_review")]
    public bool? AddressUnderReview { get; set; }

    /// <summary>获取或设置礼物单号（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }
}

/// <summary>订单搜索（<c>order/search</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSearchOrderRequest
{
    /// <summary>获取或设置搜索条件（官方 <c>search_condition</c>，至少设置一个字段）。</summary>
    [JsonPropertyName("search_condition")]
    public ChannelsSearchOrderCondition SearchCondition { get; set; } = new();

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public long PageSize { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，必填，上一页请求返回）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>订单搜索（<c>order/search</c>）响应（与 <c>order/list/get</c> 共用 <see cref="ChannelsOrderIdListResponse"/>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSearchOrderResponse : ChannelsOrderIdListResponse
{
}

/// <summary>修改订单备注（<c>order/merchantnotes/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsUpdateMerchantNotesRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置备注内容（官方 <c>merchant_notes</c>，必填）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string MerchantNotes { get; set; } = string.Empty;

    /// <summary>获取或设置备注标签颜色（官方 <c>merchant_notes_tag_color</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("merchant_notes_tag_color")]
    public int? MerchantNotesTagColor { get; set; }
}

/// <summary>订单改价商品明细（官方 <c>order/price/update</c> 的 <c>change_order_infos</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderPriceChangeInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置商品 sku（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public long SkuId { get; set; }

    /// <summary>获取或设置修改后的商品实付价格（官方 <c>change_price</c>，必填，单位为分）。</summary>
    [JsonPropertyName("change_price")]
    public long ChangePrice { get; set; }
}

/// <summary>修改订单价格（<c>order/price/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsChangeOrderPriceRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>获取或设置商品信息列表（官方 <c>change_order_infos</c>，必填）。</summary>
    [JsonPropertyName("change_order_infos")]
    public List<ChannelsOrderPriceChangeInfo> ChangeOrderInfos { get; set; } = new();

    /// <summary>获取或设置是否修改运费（官方 <c>change_express</c>，必填）。</summary>
    [JsonPropertyName("change_express")]
    public bool ChangeExpress { get; set; }

    /// <summary>获取或设置修改后的运费价格（官方 <c>express_fee</c>，change_express 为 true 时指定，不填默认为 0，单位为分）。</summary>
    [JsonPropertyName("express_fee")]
    public long? ExpressFee { get; set; }
}

/// <summary>收货地址（官方 <c>order/address/update</c> 的 <c>user_address</c> 与
/// <c>sensitiveinfo/decode</c> 的 <c>address_info</c> 共用字段集）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderAddress
{
    /// <summary>获取或设置收货人姓名（官方 <c>user_name</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置省份（官方 <c>province_name</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置城市（官方 <c>city_name</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置区（官方 <c>county_name</c>）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail_info</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置国家码（官方 <c>national_code</c>）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>获取或设置普通订单联系方式（官方 <c>tel_number</c>，订单 deliver_method=0 必填；虚拟号场景为脱敏号码）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置门牌号码（官方 <c>house_number</c>）。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }

    /// <summary>获取或设置虚拟商品订单联系方式（官方 <c>virtual_order_tel_number</c>，虚拟商品订单必填，deliver_method=1）。</summary>
    [JsonPropertyName("virtual_order_tel_number")]
    public string? VirtualOrderTelNumber { get; set; }
}

/// <summary>修改订单地址（<c>order/address/update</c>）请求体。</summary>
/// <remarks>提交修改后将进入「协商」流程，需买家同意后方可修改成功（官方业务限制，守卫锁定）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsChangeOrderAddressRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>获取或设置新地址（官方 <c>user_address</c>，必填）。</summary>
    [JsonPropertyName("user_address")]
    public ChannelsOrderAddress UserAddress { get; set; } = new();
}

/// <summary>同意用户修改收货地址申请（<c>order/addressmodify/accept</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsAcceptAddressModifyRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>拒绝买家的订单修改收货地址申请（<c>order/addressmodify/reject</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsRejectAddressModifyRequest
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>礼物订单新增备注信息（<c>order/presentnote/add</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsAddPresentNoteRequest
{
    /// <summary>获取或设置礼物订单 ID（官方 <c>present_order_id</c>，必填）。</summary>
    [JsonPropertyName("present_order_id")]
    public string PresentOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置礼物订单备注信息（官方 <c>notes</c>，必填）。</summary>
    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;
}

/// <summary>获取礼物单的子单列表（<c>order/presentsuborder/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetPresentSubOrderRequest
{
    /// <summary>获取或设置礼物单号（官方 <c>present_order_id</c>，必填）。</summary>
    [JsonPropertyName("present_order_id")]
    public string PresentOrderId { get; set; } = string.Empty;
}

/// <summary>获取礼物单的子单列表（<c>order/presentsuborder/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetPresentSubOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置订单号列表（官方 <c>order_ids</c>）。</summary>
    [JsonPropertyName("order_ids")]
    public List<string>? OrderIds { get; set; }
}

/// <summary>获取所有待发货前更换 sku 待处理请求（<c>order/preshipmentchangesku/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetPreShipmentChangeSkuRequest
{
    /// <summary>获取或设置分页页码（官方 <c>page</c>，必填）。</summary>
    [JsonPropertyName("page")]
    public long Page { get; set; }

    /// <summary>获取或设置每一页需要展示的条数（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public long PageSize { get; set; }
}

/// <summary>获取所有待发货前更换 sku 待处理请求（<c>order/preshipmentchangesku/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetPreShipmentChangeSkuResponse : ChannelsResponse
{
    /// <summary>获取或设置等待商家处理的换款请求订单 id（官方 <c>order_ids</c>）。</summary>
    [JsonPropertyName("order_ids")]
    public List<string>? OrderIds { get; set; }
}

/// <summary>同意待发货前更换 sku 请求（<c>order/preshipmentchangesku/approve</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsApprovePreShipmentChangeSkuRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>拒绝待发货前更换 sku 请求（<c>order/preshipmentchangesku/reject</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsRejectPreShipmentChangeSkuRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>盲盒款式回传项（官方 <c>order/blindboxitem/set</c> 的 <c>item_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsBlindBoxItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置某件商品的常量编号（官方 <c>item_unique_id</c>，必填）。</summary>
    [JsonPropertyName("item_unique_id")]
    public string ItemUniqueId { get; set; } = string.Empty;

    /// <summary>获取或设置盲盒款式 id（官方 <c>blind_box_item_id</c>，必填）。</summary>
    [JsonPropertyName("blind_box_item_id")]
    public string BlindBoxItemId { get; set; } = string.Empty;
}

/// <summary>盲盒款式设置失败项（官方 <c>order/blindboxitem/set</c> 的 <c>fail_item_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsBlindBoxFailItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置某件商品的常量编号（官方 <c>item_unique_id</c>）。</summary>
    [JsonPropertyName("item_unique_id")]
    public string? ItemUniqueId { get; set; }

    /// <summary>获取或设置错误码（官方 <c>errcode</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("errcode")]
    public int? ErrorCode { get; set; }

    /// <summary>获取或设置错误信息（官方 <c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }
}

/// <summary>设置潮玩订单盲盒款式（<c>order/blindboxitem/set</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSetBlindBoxItemRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置回传的盲盒款式列表（官方 <c>item_list</c>，必填）。</summary>
    [JsonPropertyName("item_list")]
    public List<ChannelsBlindBoxItem> ItemList { get; set; } = new();
}

/// <summary>设置潮玩订单盲盒款式（<c>order/blindboxitem/set</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSetBlindBoxItemResponse : ChannelsResponse
{
    /// <summary>获取或设置设置失败的盲盒款式列表（官方 <c>fail_item_list</c>）。</summary>
    [JsonPropertyName("fail_item_list")]
    public List<ChannelsBlindBoxFailItem>? FailItemList { get; set; }
}