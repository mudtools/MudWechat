// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>代发商品信息（官方 <c>dropship_product_list</c> 数组元素，<c>dropship/assign</c> 与 <c>dropship/cancel</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：分配订单代发与取消分配代发单两端点的 <c>dropship_product_list</c>
/// 元素字段集完全一致（<c>spu_id</c>/<c>sku_id</c>/<c>product_cnt</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipProduct
{
    /// <summary>获取或设置商品 spuid（官方 <c>spu_id</c>，必填）。</summary>
    [JsonPropertyName("spu_id")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置数量（官方 <c>product_cnt</c>，必填）。</summary>
    [JsonPropertyName("product_cnt")]
    public int ProductCnt { get; set; }
}

/// <summary>分配订单代发（<c>dropship/assign</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；<c>dropship_product_list</c> 自官方 2026-05-07 变更起为<b>必填</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipAssignRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string SupplierAppid { get; set; } = string.Empty;

    /// <summary>获取或设置代发商品信息（官方 <c>dropship_product_list</c>，必填）。</summary>
    [JsonPropertyName("dropship_product_list")]
    public List<ChannelsDropshipProduct> DropshipProductList { get; set; } = new();
}

/// <summary>取消分配代发单（<c>dropship/cancel</c>）请求体。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>supplier_appid</c> 字段，但参数表未列出——
/// SDK 按参数表建模（仅 <c>order_id</c>/<c>ds_order_id</c>/<c>dropship_product_list</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipCancelRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，必填）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string DsOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置代发商品信息（官方 <c>dropship_product_list</c>，必填）。</summary>
    [JsonPropertyName("dropship_product_list")]
    public List<ChannelsDropshipProduct> DropshipProductList { get; set; } = new();
}

/// <summary>查询代发单详情（<c>dropship/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipGetRequest
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，必填；官方类型为 string）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string DsOrderId { get; set; } = string.Empty;
}

/// <summary>查询代发单详情（<c>dropship/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipGetResponse : ChannelsResponse
{
    /// <summary>获取或设置代发单结构（官方 <c>dropship_order</c>）。</summary>
    [JsonPropertyName("dropship_order")]
    public ChannelsDropshipOrder? DropshipOrder { get; set; }
}

/// <summary>代发单结构（官方 <c>dropship_order</c> 对象，<c>dropship/get</c>）。</summary>
/// <remarks>
/// <b>官方形态照录</b>：<c>ds_order_id</c>/<c>order_id</c> 在响应侧官方类型为 number，
/// 而请求侧同名参数（<c>dropship/get</c> 的 <c>ds_order_id</c>）为 string——按官方各侧参数表分别建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipOrder
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，官方响应类型为 number）。</summary>
    [JsonPropertyName("ds_order_id")]
    public long? DsOrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>，见 <see cref="ChannelsDropshipOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置供货商名称（官方 <c>supplier_name</c>）。</summary>
    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }

    /// <summary>获取或设置关联订单号（官方 <c>order_id</c>，官方响应类型为 number）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置代发单详情（官方 <c>dropship_order_detail</c>）。</summary>
    [JsonPropertyName("dropship_order_detail")]
    public ChannelsDropshipOrderDetail? DropshipOrderDetail { get; set; }
}

/// <summary>代发单详情（官方 <c>dropship_order_detail</c> 对象，<c>dropship/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipOrderDetail
{
    /// <summary>获取或设置商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsDropshipProductInfo>? ProductInfos { get; set; }

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
}

/// <summary>代发单商品信息（官方 <c>product_infos</c> 数组元素，<c>dropship/get</c>）。</summary>
/// <remarks>
/// <b>共用裁决（负向）</b>：与供货单商品（<c>supplyorder/get</c> 的 <c>product_infos</c>）结构不同
/// （本形态无 <c>sku_attrs</c>/<c>sale_price</c>/<c>supply_order_product_info</c>，代发侧为
/// <c>dropship_info</c>）⇒ 分建 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipProductInfo
{
    /// <summary>获取或设置商品 spuid（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置 sku 数量（官方 <c>sku_cnt</c>）。</summary>
    [JsonPropertyName("sku_cnt")]
    public int? SkuCnt { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

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
    /// 订单内商品唯一标识，下单后不会发生变化，可用于判断商品是否更换款式。
    /// </summary>
    [JsonPropertyName("product_unique_id")]
    public string? ProductUniqueId { get; set; }

    /// <summary>获取或设置代发相关信息（官方 <c>dropship_info</c>）。</summary>
    [JsonPropertyName("dropship_info")]
    public ChannelsDropshipInfo? DropshipInfo { get; set; }
}

/// <summary>商品发货信息（官方 <c>sku_deliver_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>sku_deliver_info</c> 字段集完全一致
/// （<c>stock_type</c>/<c>predict_delivery_time</c>/<c>full_payment_presale_delivery_type</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSkuDeliverInfo
{
    /// <summary>获取或设置商品发货类型（官方 <c>stock_type</c>，见 <see cref="ChannelsStockTypes"/>）。</summary>
    [JsonPropertyName("stock_type")]
    public int? StockType { get; set; }

    /// <summary>获取或设置预计发货时间（官方 <c>predict_delivery_time</c>，<c>stock_type = 1</c> 时返回该字段）。</summary>
    [JsonPropertyName("predict_delivery_time")]
    public long? PredictDeliveryTime { get; set; }

    /// <summary>
    /// 获取或设置预售类型（官方 <c>full_payment_presale_delivery_type</c>，
    /// <c>stock_type = 1</c> 时返回该字段，见 <see cref="ChannelsPresaleDeliveryTypes"/>）。
    /// </summary>
    [JsonPropertyName("full_payment_presale_delivery_type")]
    public int? FullPaymentPresaleDeliveryType { get; set; }
}

/// <summary>代发相关信息（官方 <c>dropship_info</c> 对象，<c>dropship/get</c> 商品维度）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipInfo
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>）。</summary>
    [JsonPropertyName("ds_order_id")]
    public long? DsOrderId { get; set; }

    /// <summary>获取或设置发货状态（官方 <c>delivery_status</c>，见 <see cref="ChannelsDeliveryStatuses"/>）。</summary>
    [JsonPropertyName("delivery_status")]
    public int? DeliveryStatus { get; set; }

    /// <summary>获取或设置售后状态（官方 <c>aftersale_status</c>，官方文档仅声明 1 表示售后中）。</summary>
    [JsonPropertyName("aftersale_status")]
    public int? AftersaleStatus { get; set; }

    /// <summary>获取或设置取消时间（官方 <c>cancel_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>获取或设置取消原因（官方 <c>cancel_scene</c>，见 <see cref="ChannelsDropshipCancelScenes"/>）。</summary>
    [JsonPropertyName("cancel_scene")]
    public int? CancelScene { get; set; }

    /// <summary>获取或设置换货申请状态（官方 <c>preshipment_change_sku_state</c>，官方文档仅声明 3 表示等待商家处理）。</summary>
    [JsonPropertyName("preshipment_change_sku_state")]
    public int? PreshipmentChangeSkuState { get; set; }
}

/// <summary>发货信息（官方 <c>delivery_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>delivery_info</c> 字段集完全一致
/// （<c>address_info</c>/<c>delivery_product_info</c>/<c>ship_done_time</c>/<c>deliver_method</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryInfo
{
    /// <summary>获取或设置地址信息（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsDeliveryAddress? AddressInfo { get; set; }

    /// <summary>获取或设置发货物流信息（官方 <c>delivery_product_info</c>）。</summary>
    [JsonPropertyName("delivery_product_info")]
    public List<ChannelsDeliveryPackage>? DeliveryProductInfo { get; set; }

    /// <summary>获取或设置发货完成时间（官方 <c>ship_done_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("ship_done_time")]
    public long? ShipDoneTime { get; set; }

    /// <summary>
    /// 获取或设置订单发货方式（官方 <c>deliver_method</c>）——官方原文照录：0 表示普通物流，
    /// 1/3 表示虚拟发货，由商品的同名字段决定。
    /// </summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }
}

/// <summary>收货/发货地址信息（官方 <c>address_info</c> 与 <c>delivery_address</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <para>
/// <b>共用 DTO 裁决</b>：①官方原文「<c>delivery_address</c> 字段与 <c>address_info</c> 相同」⇒ 同一 DTO；
/// ②代发单与供货单两侧地址字段集一致（供货单参数表未列 <c>national_code</c>，官方示例包含且代发侧
/// 参数表声明「已废弃」⇒ 统一登记，供旧报文兼容）⇒ 共用本 DTO。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryAddress
{
    /// <summary>获取或设置收货人姓名（官方 <c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置省份（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置城市（官方 <c>city_name</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置区（官方 <c>county_name</c>）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail_info</c>）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置国家码（官方 <c>national_code</c>，官方已废弃，请勿使用）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>tel_number</c>）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置门牌号码（官方 <c>house_number</c>）。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }

    /// <summary>获取或设置虚拟发货订单联系方式（官方 <c>virtual_order_tel_number</c>，<c>deliver_method = 1</c> 时返回）。</summary>
    [JsonPropertyName("virtual_order_tel_number")]
    public string? VirtualOrderTelNumber { get; set; }

    /// <summary>获取或设置额外的联系方式信息（官方 <c>tel_number_ext_info</c>，虚拟号码相关）。</summary>
    [JsonPropertyName("tel_number_ext_info")]
    public ChannelsTelNumberExtInfo? TelNumberExtInfo { get; set; }

    /// <summary>获取或设置是否使用虚拟号码（官方 <c>use_tel_number</c>：0 不使用，1 使用）。</summary>
    [JsonPropertyName("use_tel_number")]
    public int? UseTelNumber { get; set; }

    /// <summary>获取或设置标识当前店铺下一个唯一的用户收货地址（官方 <c>hash_code</c>）。</summary>
    [JsonPropertyName("hash_code")]
    public string? HashCode { get; set; }
}

/// <summary>虚拟号码联系方式信息（官方 <c>tel_number_ext_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsTelNumberExtInfo
{
    /// <summary>获取或设置脱敏手机号（官方 <c>real_tel_number</c>）。</summary>
    [JsonPropertyName("real_tel_number")]
    public string? RealTelNumber { get; set; }

    /// <summary>获取或设置完整的虚拟号码（官方 <c>virtual_tel_number</c>）。</summary>
    [JsonPropertyName("virtual_tel_number")]
    public string? VirtualTelNumber { get; set; }

    /// <summary>获取或设置主动兑换虚拟号码次数（官方 <c>get_virtual_tel_cnt</c>）。</summary>
    [JsonPropertyName("get_virtual_tel_cnt")]
    public int? GetVirtualTelCnt { get; set; }

    /// <summary>获取或设置主动兑换的虚拟号码过期时间（官方 <c>virtual_tel_expire_time</c>）。</summary>
    [JsonPropertyName("virtual_tel_expire_time")]
    public long? VirtualTelExpireTime { get; set; }
}

/// <summary>包裹发货物流信息（官方 <c>delivery_product_info</c> 数组元素）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>delivery_product_info</c> 元素字段集完全一致 ⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryPackage
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置包裹中商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsPackageProduct>? ProductInfos { get; set; }

    /// <summary>获取或设置快递公司名称（官方 <c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }

    /// <summary>获取或设置发货时间（官方 <c>delivery_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("delivery_time")]
    public long? DeliveryTime { get; set; }

    /// <summary>获取或设置配送方式（官方 <c>deliver_type</c>，官方未列出具体枚举值）。</summary>
    [JsonPropertyName("deliver_type")]
    public int? DeliverType { get; set; }

    /// <summary>获取或设置发货地址（官方 <c>delivery_address</c>，官方原文「字段与 address_info 相同」）。</summary>
    [JsonPropertyName("delivery_address")]
    public ChannelsDeliveryAddress? DeliveryAddress { get; set; }
}

/// <summary>包裹中商品信息（官方 <c>delivery_product_info</c> 的 <c>product_infos</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsPackageProduct
{
    /// <summary>获取或设置商品 spuid（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 skuid（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }
}

/// <summary>订单扩展信息（官方 <c>ext_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>ext_info</c> 参数表字段集完全一致
/// （<c>customer_notes</c>/<c>merchant_notes</c>/<c>present_notes</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsOrderExtInfo
{
    /// <summary>获取或设置用户备注（官方 <c>customer_notes</c>）。</summary>
    [JsonPropertyName("customer_notes")]
    public string? CustomerNotes { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置礼物备注（官方 <c>present_notes</c>）。</summary>
    [JsonPropertyName("present_notes")]
    public string? PresentNotes { get; set; }
}

/// <summary>商品贺卡信息（官方 <c>greeting_card_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGreetingCardInfo
{
    /// <summary>获取或设置贺卡落款（官方 <c>giver_name</c>，用户选填）。</summary>
    [JsonPropertyName("giver_name")]
    public string? GiverName { get; set; }

    /// <summary>获取或设置贺卡称谓（官方 <c>receiver_name</c>，用户选填）。</summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }

    /// <summary>获取或设置贺卡内容（官方 <c>greeting_message</c>，用户必填）。</summary>
    [JsonPropertyName("greeting_message")]
    public string? GreetingMessage { get; set; }
}

/// <summary>商品定制信息（官方 <c>custom_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsCustomInfo
{
    /// <summary>获取或设置定制图片（官方 <c>custom_img_url</c>，<c>custom_type = 2</c> 时返回）。</summary>
    [JsonPropertyName("custom_img_url")]
    public string? CustomImgUrl { get; set; }

    /// <summary>获取或设置定制文字（官方 <c>custom_word</c>，<c>custom_type = 1</c> 时返回）。</summary>
    [JsonPropertyName("custom_word")]
    public string? CustomWord { get; set; }

    /// <summary>获取或设置定制类型（官方 <c>custom_type</c>，见 <see cref="ChannelsCustomTypes"/>）。</summary>
    [JsonPropertyName("custom_type")]
    public int? CustomType { get; set; }

    /// <summary>获取或设置定制预览图片（官方 <c>custom_preview_img_url</c>，开启了定制预览时返回）。</summary>
    [JsonPropertyName("custom_preview_img_url")]
    public string? CustomPreviewImgUrl { get; set; }
}

/// <summary>秒级时间范围（官方 <c>create_time_range</c>/<c>update_time_range</c>/<c>order_create_time_range</c> 对象，
/// <c>dropship/list</c>、<c>dropship/search</c>、<c>supplyorder/list</c>、<c>supplyorder/search</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：四个拉单/搜单端点的三种时间范围对象字段集完全一致
/// （<c>start_time</c>/<c>end_time</c>）⇒ 共用本 DTO；官方限制起止距离不可超过 7 天。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryTimeRange
{
    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，必填，秒级时间戳，距离 <c>end_time</c> 不可超过 7 天）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，必填，秒级时间戳，距离 <c>start_time</c> 不可超过 7 天）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}

/// <summary>分页信息（官方 <c>page_info</c> 请求对象与 <c>next_page</c> 响应对象）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：<c>dropship/list</c>、<c>dropship/search</c>、<c>supplyorder/list</c>、
/// <c>supplyorder/search</c> 四端点的请求 <c>page_info</c> 与响应 <c>next_page</c> 字段集完全一致
/// （<c>offset</c>/<c>limit</c>/<c>next_key</c>）⇒ 共用本 DTO；
/// <c>next_key</c> 代替 <c>offset</c> 分页性能更优（首页传空，后续用响应 <c>next_page</c> 回填）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryPageInfo
{
    /// <summary>获取或设置游标（官方 <c>offset</c>，选填，从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置分页大小（官方 <c>limit</c>，必填，&lt;= 100）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// 获取或设置分页标识（官方 <c>next_key</c>，选填）——代替 <c>offset</c>，分页性能更优
    /// （首页传空，后续用响应 <c>next_page</c> 的 <c>next_key</c> 回填）。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>拉取代发单列表（<c>dropship/list</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + JSON 请求体；<c>create_time_range</c>/<c>update_time_range</c> 起止距离不可超过 7 天。</para>
/// <para>
/// <b>官方文档矛盾照录</b>：参数表 <c>order_id</c> 类型为 number，请求示例为字符串
/// <c>"37000204335323251"</c>——SDK 按参数表建模（long）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipListRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可根据订单号反查代发单；官方参数表类型为 number，示例为字符串——照录）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>，选填，见 <see cref="ChannelsDropshipOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置代发单创建时间范围（官方 <c>create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，选填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }

    /// <summary>获取或设置代发单更新时间范围（官方 <c>update_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}

/// <summary>搜索代发单（<c>dropship/search</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + JSON 请求体；<c>search_condition</c> 必填；<c>page_info.limit</c> &lt;= 10（本端点专属，严于其他端点的 100）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipSearchRequest
{
    /// <summary>获取或设置搜索条件（官方 <c>search_condition</c>，必填）。</summary>
    [JsonPropertyName("search_condition")]
    public ChannelsDropshipSearchCondition SearchCondition { get; set; } = new();

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，选填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }
}

/// <summary>代发单搜索条件（官方 <c>search_condition</c> 对象，<c>dropship/search</c>）。</summary>
/// <remarks>
/// <b>共用裁决（负向）</b>：与供货单搜索条件（<c>supplyorder/search</c> 的 <c>search_condition</c>）
/// 字段集当前一致，但供货侧官方示例已出现表外字段（<c>tel_last4</c>/<c>user_name</c>）预告两形态可独立演化
/// ⇒ 按端点分建，不做跨端点共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipSearchCondition
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可根据订单号反查代发单）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>，选填，见 <see cref="ChannelsDropshipOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置代发单创建时间范围（官方 <c>create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置运单号（官方 <c>waybill_id</c>，选填，只可独立查询）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置订单下单时间范围（官方 <c>order_create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("order_create_time_range")]
    public ChannelsDeliveryTimeRange? OrderCreateTimeRange { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>product_code</c>，选填）。</summary>
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>spu_name</c>，选填）。</summary>
    [JsonPropertyName("spu_name")]
    public string? SpuName { get; set; }

    /// <summary>获取或设置供货商名称（官方 <c>supplier_appname</c>，选填）。</summary>
    [JsonPropertyName("supplier_appname")]
    public string? SupplierAppname { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>，选填）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置代发单更新时间范围（官方 <c>update_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}

/// <summary>代发单号列表响应（<c>dropship/list</c> 与 <c>dropship/search</c> 共用）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：拉取代发单列表与搜索代发单两响应字段集完全一致
/// （<c>ds_order_id_list</c>/<c>next_page</c>/<c>total</c>/<c>has_more</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipOrderListResponse : ChannelsResponse
{
    /// <summary>获取或设置代发单号列表（官方 <c>ds_order_id_list</c>）。</summary>
    [JsonPropertyName("ds_order_id_list")]
    public List<string>? DsOrderIdList { get; set; }

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

/// <summary>代发单状态常量（官方 <c>status</c> 枚举值，<c>dropship/list</c>、<c>dropship/search</c>、<c>dropship/get</c>）。</summary>
public static class ChannelsDropshipOrderStatuses
{
    /// <summary>待发货。</summary>
    public const int PendingDelivery = 100;

    /// <summary>部分发货。</summary>
    public const int PartialDelivered = 199;

    /// <summary>已发货。</summary>
    public const int Delivered = 200;

    /// <summary>已取消。</summary>
    public const int Canceled = 300;
}

/// <summary>发货状态常量（官方 <c>delivery_status</c> 枚举值，代发 <c>dropship_info</c> 与供货 <c>supply_order_product_info</c> 同值同义 ⇒ 共用）。</summary>
public static class ChannelsDeliveryStatuses
{
    /// <summary>待发货。</summary>
    public const int PendingDelivery = 10;

    /// <summary>部分发货。</summary>
    public const int PartialDelivered = 19;

    /// <summary>已发货。</summary>
    public const int Delivered = 20;

    /// <summary>已取消。</summary>
    public const int Canceled = 30;
}

/// <summary>代发单取消原因常量（官方 <c>cancel_scene</c> 枚举值，代发 <c>dropship_info</c>）。</summary>
/// <remarks><b>共用裁决（负向）</b>：供货侧 <c>cancel_scene</c> 同值不同义（1 用户售后 / 2 平台砍单）⇒ 分建 <see cref="ChannelsSupplyCancelScenes"/>。</remarks>
public static class ChannelsDropshipCancelScenes
{
    /// <summary>售后。</summary>
    public const int AfterSale = 1;

    /// <summary>商家取消代发。</summary>
    public const int MerchantCancel = 2;
}

/// <summary>商品发货类型常量（官方 <c>stock_type</c> 枚举值，<c>sku_deliver_info</c>）。</summary>
public static class ChannelsStockTypes
{
    /// <summary>现货。</summary>
    public const int InStock = 0;

    /// <summary>全款预售。</summary>
    public const int FullPaymentPresale = 1;
}

/// <summary>预售类型常量（官方 <c>full_payment_presale_delivery_type</c> 枚举值，<c>sku_deliver_info</c>）。</summary>
public static class ChannelsPresaleDeliveryTypes
{
    /// <summary>付款后 n 天发货。</summary>
    public const int AfterPaymentDays = 0;

    /// <summary>预售结束后 n 天发货。</summary>
    public const int AfterPresaleEndDays = 1;
}

/// <summary>定制类型常量（官方 <c>custom_type</c> 枚举值，<c>custom_info</c>）。</summary>
public static class ChannelsCustomTypes
{
    /// <summary>文字定制。</summary>
    public const int Word = 1;

    /// <summary>图片定制。</summary>
    public const int Image = 2;
}
