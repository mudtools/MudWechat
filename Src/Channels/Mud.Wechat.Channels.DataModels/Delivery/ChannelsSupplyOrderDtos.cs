// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取供货单列表（<c>supplyorder/list</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + JSON 请求体；时间范围至少填一个且起止距离不可超过 7 天。</para>
/// <para>
/// <b>官方变更日志照录</b>：2026-05-25 起 <c>shop_appid</c> 字段已删除、新增 <c>supplier_appid</c>——
/// SDK 按变更后参数表建模，不登记 <c>shop_appid</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderListRequest
{
    /// <summary>获取或设置供货单创建时间范围（官方 <c>create_time_range</c>，选填，时间范围至少填一个）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置供货单状态（官方 <c>status</c>，选填；官方页声明「具体枚举值请参考下文」但未列出具体值——照录）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，选填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可通过订单号反查供货单）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置供货单更新时间范围（官方 <c>update_time_range</c>，选填）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}

/// <summary>搜索供货单（<c>supplyorder/search</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + JSON 请求体；<c>search_condition</c> 与 <c>page_info</c> 官方参数表均标必填；
/// <c>search_condition</c> 各字段必须至少设置一个，否则接口报错。</para>
/// <para>
/// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>tel_last4</c>/<c>user_name</c> 两字段，
/// 但参数表未列出——SDK 按参数表建模，不登记表外字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderSearchRequest
{
    /// <summary>获取或设置搜索条件（官方 <c>search_condition</c>，必填，至少设置一个字段）。</summary>
    [JsonPropertyName("search_condition")]
    public ChannelsSupplyOrderSearchCondition SearchCondition { get; set; } = new();

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，必填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo PageInfo { get; set; } = new();
}

/// <summary>供货单搜索条件（官方 <c>search_condition</c> 对象，<c>supplyorder/search</c>）。</summary>
/// <remarks>
/// <b>共用裁决（负向）</b>：与代发单搜索条件（<c>dropship/search</c> 的 <c>search_condition</c>）
/// 字段集当前一致，但本端点官方示例已出现表外字段（<c>tel_last4</c>/<c>user_name</c>）预告两形态可独立演化
/// ⇒ 按端点分建，不做跨端点共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderSearchCondition
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可根据订单号反查供货单）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置供货单状态（官方 <c>status</c>，选填；官方页声明「具体枚举值请参考下文」但未列出具体值——照录）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置供货单创建时间范围（官方 <c>create_time_range</c>，选填，时间范围至少填一个）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置运单号（官方 <c>waybill_id</c>，选填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置订单下单时间范围（官方 <c>order_create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("order_create_time_range")]
    public ChannelsDeliveryTimeRange? OrderCreateTimeRange { get; set; }

    /// <summary>获取或设置货品编码（官方 <c>product_code</c>，选填）。</summary>
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; set; }

    /// <summary>获取或设置货品名称（官方 <c>spu_name</c>，选填）。</summary>
    [JsonPropertyName("spu_name")]
    public string? SpuName { get; set; }

    /// <summary>获取或设置供货商名称（官方 <c>supplier_appname</c>，选填）。</summary>
    [JsonPropertyName("supplier_appname")]
    public string? SupplierAppname { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>，选填）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置供货单更新时间范围（官方 <c>update_time_range</c>，选填，时间范围至少填一个）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}

/// <summary>供货单号列表响应（<c>supplyorder/list</c> 与 <c>supplyorder/search</c> 共用）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：获取供货单列表与搜索供货单两响应字段集完全一致
/// （<c>supply_order_id_list</c>/<c>next_page</c>/<c>total</c>/<c>has_more</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderListResponse : ChannelsResponse
{
    /// <summary>获取或设置供货单号列表（官方 <c>supply_order_id_list</c>）。</summary>
    [JsonPropertyName("supply_order_id_list")]
    public List<string>? SupplyOrderIdList { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_page</c>，下一页请求返回）。</summary>
    [JsonPropertyName("next_page")]
    public ChannelsDeliveryPageInfo? NextPage { get; set; }

    /// <summary>获取或设置总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>，true 表示有下一页，false 表示已经结束）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}

/// <summary>查询供货单详情（<c>supplyorder/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderGetRequest
{
    /// <summary>获取或设置供货单号（官方 <c>supply_order_id</c>，必填）。</summary>
    [JsonPropertyName("supply_order_id")]
    public string SupplyOrderId { get; set; } = string.Empty;
}

/// <summary>查询供货单详情（<c>supplyorder/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderGetResponse : ChannelsResponse
{
    /// <summary>获取或设置供货单结构（官方 <c>supply_order</c>）。</summary>
    [JsonPropertyName("supply_order")]
    public ChannelsSupplyOrder? SupplyOrder { get; set; }
}

/// <summary>供货单结构（官方 <c>supply_order</c> 对象，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrder
{
    /// <summary>获取或设置供货单号（官方 <c>supply_order_id</c>）。</summary>
    [JsonPropertyName("supply_order_id")]
    public string? SupplyOrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置供货单状态（官方 <c>status</c>，见 <see cref="ChannelsSupplyOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置供货商名称（官方 <c>supplier_name</c>）。</summary>
    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置供货单详情（官方 <c>supply_order_detail</c>）。</summary>
    [JsonPropertyName("supply_order_detail")]
    public ChannelsSupplyOrderDetail? SupplyOrderDetail { get; set; }
}

/// <summary>供货单详情（官方 <c>supply_order_detail</c> 对象，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderDetail
{
    /// <summary>获取或设置商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsSupplyProductInfo>? ProductInfos { get; set; }

    /// <summary>获取或设置价格信息（官方 <c>price_info</c>）。</summary>
    [JsonPropertyName("price_info")]
    public List<ChannelsSupplyPriceInfo>? PriceInfo { get; set; }

    /// <summary>获取或设置发货信息（官方 <c>delivery_info</c>）。</summary>
    [JsonPropertyName("delivery_info")]
    public ChannelsDeliveryInfo? DeliveryInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsOrderExtInfo? ExtInfo { get; set; }

    /// <summary>获取或设置需代写的商品贺卡信息（官方 <c>greeting_card_info</c>）。</summary>
    [JsonPropertyName("greeting_card_info")]
    public ChannelsGreetingCardInfo? GreetingCardInfo { get; set; }

    /// <summary>获取或设置商品定制信息（官方 <c>custom_info</c>）。</summary>
    [JsonPropertyName("custom_info")]
    public ChannelsCustomInfo? CustomInfo { get; set; }

    /// <summary>获取或设置运费（官方 <c>freight</c>，单位：分）。</summary>
    [JsonPropertyName("freight")]
    public long? Freight { get; set; }

    /// <summary>获取或设置售后信息（官方 <c>after_sale_orders</c>）。</summary>
    [JsonPropertyName("after_sale_orders")]
    public List<ChannelsSupplyAftersaleOrder>? AfterSaleOrders { get; set; }
}

/// <summary>供货单商品信息（官方 <c>product_infos</c> 数组元素，<c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用裁决（负向）</b>：与代发单商品（<c>dropship/get</c> 的 <c>product_infos</c>）结构不同
/// （本形态多 <c>sku_attrs</c>/<c>sale_price</c>/<c>supply_order_product_info</c>）⇒ 分建 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyProductInfo
{
    /// <summary>获取或设置商品 spuid（官方 <c>product_id</c>，官方类型为 string）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>，官方类型为 string）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置售价（单价）（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 数量（官方 <c>sku_cnt</c>）。</summary>
    [JsonPropertyName("sku_cnt")]
    public int? SkuCnt { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置 sku 属性/款式信息（官方 <c>sku_attrs</c>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置商品外部 spuid（官方 <c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商品外部 skuid（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置区域库存 id（官方 <c>out_warehouse_id</c>）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string? OutWarehouseId { get; set; }

    /// <summary>获取或设置商品发货信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置商品发货时效（官方 <c>delivery_deadline</c>，超时此时间未发货即为发货超时）。</summary>
    [JsonPropertyName("delivery_deadline")]
    public long? DeliveryDeadline { get; set; }

    /// <summary>获取或设置是否赠品（官方 <c>is_free_gift</c>，非必填，赠品商品返回，1 表示是赠品）。</summary>
    [JsonPropertyName("is_free_gift")]
    public int? IsFreeGift { get; set; }

    /// <summary>
    /// 获取或设置商品常量编号（官方 <c>product_unique_id</c>）——
    /// 订单内商品唯一标识，可用于判断货品是否更换款式。
    /// </summary>
    [JsonPropertyName("product_unique_id")]
    public string? ProductUniqueId { get; set; }

    /// <summary>获取或设置供货信息（官方 <c>supply_order_product_info</c>）。</summary>
    [JsonPropertyName("supply_order_product_info")]
    public ChannelsSupplyOrderProductInfo? SupplyOrderProductInfo { get; set; }
}

/// <summary>sku 属性/款式信息（官方 <c>sku_attrs</c> 数组元素，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSkuAttr
{
    /// <summary>获取或设置 sku 属性/款式类型（官方 <c>attr_key</c>）。</summary>
    [JsonPropertyName("attr_key")]
    public string? AttrKey { get; set; }

    /// <summary>获取或设置属性/款式值（官方 <c>attr_value</c>）。</summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }
}

/// <summary>供货信息（官方 <c>supply_order_product_info</c> 对象，<c>supplyorder/get</c> 商品维度）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderProductInfo
{
    /// <summary>获取或设置供货单单号（官方 <c>supply_order_id</c>）。</summary>
    [JsonPropertyName("supply_order_id")]
    public string? SupplyOrderId { get; set; }

    /// <summary>获取或设置发货状态（官方 <c>delivery_status</c>，见 <see cref="ChannelsDeliveryStatuses"/>）。</summary>
    [JsonPropertyName("delivery_status")]
    public int? DeliveryStatus { get; set; }

    /// <summary>
    /// 获取或设置售后状态（官方 <c>aftersale_status</c>，见 <see cref="ChannelsSupplyAftersaleStatuses"/>）。
    /// </summary>
    [JsonPropertyName("aftersale_status")]
    public int? AftersaleStatus { get; set; }

    /// <summary>获取或设置取消时间（官方 <c>cancel_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>获取或设置取消原因（官方 <c>cancel_scene</c>，见 <see cref="ChannelsSupplyCancelScenes"/>）。</summary>
    [JsonPropertyName("cancel_scene")]
    public int? CancelScene { get; set; }

    /// <summary>获取或设置换 sku（官方 <c>preshipment_change_sku_state</c>）。</summary>
    [JsonPropertyName("preshipment_change_sku_state")]
    public int? PreshipmentChangeSkuState { get; set; }

    /// <summary>获取或设置供货价（单价）（官方 <c>supply_price</c>，单位：分）。</summary>
    [JsonPropertyName("supply_price")]
    public long? SupplyPrice { get; set; }
}

/// <summary>供货单价格信息（官方 <c>price_info</c> 数组元素，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyPriceInfo
{
    /// <summary>获取或设置供货单总价（官方 <c>total_price</c>，单位：分；官方口径：商品售价 × 商品数量 + 运费）。</summary>
    [JsonPropertyName("total_price")]
    public long? TotalPrice { get; set; }
}

/// <summary>供货单售后信息（官方 <c>after_sale_orders</c> 数组元素，<c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyAftersaleOrder
{
    /// <summary>获取或设置售后单号（官方 <c>aftersale_order_id</c>）。</summary>
    [JsonPropertyName("aftersale_order_id")]
    public string? AftersaleOrderId { get; set; }
}

/// <summary>供货单状态常量（官方 <c>status</c> 枚举值，<c>supplyorder/get</c>）。</summary>
public static class ChannelsSupplyOrderStatuses
{
    /// <summary>待发货。</summary>
    public const int PendingDelivery = 100;

    /// <summary>部分发货。</summary>
    public const int PartialDelivered = 199;

    /// <summary>已发货。</summary>
    public const int Delivered = 200;

    /// <summary>已取消。</summary>
    public const int Canceled = 300;

    /// <summary>已完成。</summary>
    public const int Completed = 400;
}

/// <summary>供货单取消原因常量（官方 <c>cancel_scene</c> 枚举值，供货 <c>supply_order_product_info</c>）。</summary>
/// <remarks><b>共用裁决（负向）</b>：代发侧 <c>cancel_scene</c> 同值不同义（1 售后 / 2 商家取消代发）⇒ 分建 <see cref="ChannelsDropshipCancelScenes"/>。</remarks>
public static class ChannelsSupplyCancelScenes
{
    /// <summary>用户售后。</summary>
    public const int UserAfterSale = 1;

    /// <summary>平台砍单。</summary>
    public const int PlatformCutOrder = 2;
}

/// <summary>供货售后状态常量（官方 <c>aftersale_status</c> 枚举值，供货 <c>supply_order_product_info</c>）。</summary>
public static class ChannelsSupplyAftersaleStatuses
{
    /// <summary>无售后/售后被关闭。</summary>
    public const int NoneOrClosed = 0;

    /// <summary>货品售后中（需结合售后单判断详情）。</summary>
    public const int InAfterSale = 1;

    /// <summary>货品已售后完成（已退货/退款完成）。</summary>
    public const int Finished = 2;
}
