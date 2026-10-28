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
