// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>寄件 / 收件 / 退货地址人信息（官方 <c>sender</c> / <c>receiver</c> / <c>return_address</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAddress
{
    /// <summary>获取或设置人名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置联系电话（官方 <c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>获取或设置省（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置区（官方 <c>county</c>）。</summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>获取或设置街道（官方 <c>street</c>，收件人非必填）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>商品 SN 码信息（官方 <c>goods_list.sn_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSnInfo
{
    /// <summary>获取或设置设备 imei1 码（官方 <c>imei1</c>）。</summary>
    [JsonPropertyName("imei1")]
    public string? Imei1 { get; set; }

    /// <summary>获取或设置设备 imei2 码（官方 <c>imei2</c>）。</summary>
    [JsonPropertyName("imei2")]
    public string? Imei2 { get; set; }

    /// <summary>获取或设置 SN 码信息（官方 <c>sn_code</c>）。</summary>
    [JsonPropertyName("sn_code")]
    public string? SnCode { get; set; }
}

/// <summary>订单商品信息（官方 <c>ec_order_list[].goods_list</c>，取号 / 预取号 / 查单共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGoods
{
    /// <summary>获取或设置商品名（官方 <c>good_name</c>）。</summary>
    [JsonPropertyName("good_name")]
    public string? GoodName { get; set; }

    /// <summary>获取或设置商品个数（官方 <c>good_count</c>）。</summary>
    [JsonPropertyName("good_count")]
    public int? GoodCount { get; set; }

    /// <summary>获取或设置商品 product_id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置商家自定义 spu_id（官方 <c>out_product_id</c>）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置商家自定义 sku_id（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置商家自定义商品详情（官方 <c>out_goods_info</c>，若不传平台商品 id 则展示该字段在面单商品区域）。</summary>
    [JsonPropertyName("out_goods_info")]
    public string? OutGoodsInfo { get; set; }

    /// <summary>获取或设置商家自定义额外商品信息（官方 <c>goods_ext</c>，向快递公司透传，限制长度 512）。</summary>
    [JsonPropertyName("goods_ext")]
    public string? GoodsExt { get; set; }

    /// <summary>获取或设置 SN 码信息（官方 <c>sn_info</c>）。</summary>
    [JsonPropertyName("sn_info")]
    public ChannelsLogisticsEwaybillSnInfo? SnInfo { get; set; }
}

/// <summary>电子面单订单信息（官方 <c>ec_order_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillEcOrder
{
    /// <summary>获取或设置订单 id（官方 <c>ec_order_id</c>，供货商代发单为 ds_order_id）。</summary>
    [JsonPropertyName("ec_order_id")]
    public long? EcOrderId { get; set; }

    /// <summary>获取或设置订单商品信息（官方 <c>goods_list</c>）。</summary>
    [JsonPropertyName("goods_list")]
    public List<ChannelsLogisticsEwaybillGoods>? GoodsList { get; set; }

    /// <summary>获取或设置电子面单跨店铺取号的订单密钥（官方 <c>ewaybill_order_code</c>）。</summary>
    [JsonPropertyName("ewaybill_order_code")]
    public string? EwaybillOrderCode { get; set; }

    /// <summary>获取或设置跨店铺取号的订单所属店铺 appid（官方 <c>ewaybill_order_appid</c>）。</summary>
    [JsonPropertyName("ewaybill_order_appid")]
    public string? EwaybillOrderAppid { get; set; }
}

/// <summary>保价等增值服务（官方 <c>order_vas_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillVas
{
    /// <summary>获取或设置增值服务类型（官方 <c>vas_type</c>，如 INSURE / VA002 等）。</summary>
    [JsonPropertyName("vas_type")]
    public string? VasType { get; set; }

    /// <summary>获取或设置增值服务值（官方 <c>vas_value</c>，涉及金额单位一律为分）。</summary>
    [JsonPropertyName("vas_value")]
    public string? VasValue { get; set; }

    /// <summary>获取或设置增值服务描述（官方 <c>vas_detail</c>，查单响应返回）。</summary>
    [JsonPropertyName("vas_detail")]
    public string? VasDetail { get; set; }
}

/// <summary>温层等补充字段（官方 <c>ext_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillExtInfo
{
    /// <summary>
    /// 获取或设置温层信息（官方 <c>temperature_range</c>，京东专用）：0/不传为月结默认温层；
    /// 1 普通/常温；5 鲜活；6 控温；7 冷藏；8 冷冻；9 深冷。
    /// </summary>
    [JsonPropertyName("temperature_range")]
    public int? TemperatureRange { get; set; }

    /// <summary>获取或设置包裹总重量（官方 <c>package_weight_g</c>，单位 g，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_weight_g")]
    public long? PackageWeightG { get; set; }

    /// <summary>获取或设置包裹长度（官方 <c>package_space_x</c>，单位 cm，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_space_x")]
    public long? PackageSpaceX { get; set; }

    /// <summary>获取或设置包裹宽度（官方 <c>package_space_y</c>，单位 cm，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_space_y")]
    public long? PackageSpaceY { get; set; }

    /// <summary>获取或设置包裹高度（官方 <c>package_space_z</c>，单位 cm，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_space_z")]
    public long? PackageSpaceZ { get; set; }

    /// <summary>获取或设置包裹体积（官方 <c>package_volume_ccm</c>，单位 cm³，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_volume_ccm")]
    public long? PackageVolumeCcm { get; set; }
}

/// <summary>包裹体积 / 重量信息（官方 <c>delivery_info.subpackage_list</c>，顺丰支持）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSubpackage
{
    /// <summary>获取或设置商品重量（官方 <c>weight_g</c>，单位克）。</summary>
    [JsonPropertyName("weight_g")]
    public long? WeightG { get; set; }

    /// <summary>获取或设置商品长度（官方 <c>space_x</c>，单位厘米）。</summary>
    [JsonPropertyName("space_x")]
    public long? SpaceX { get; set; }

    /// <summary>获取或设置商品宽度（官方 <c>space_y</c>，单位厘米）。</summary>
    [JsonPropertyName("space_y")]
    public long? SpaceY { get; set; }

    /// <summary>获取或设置商品高度（官方 <c>space_z</c>，单位厘米）。</summary>
    [JsonPropertyName("space_z")]
    public long? SpaceZ { get; set; }

    /// <summary>获取或设置包裹编号（官方 <c>package_no</c>）。</summary>
    [JsonPropertyName("package_no")]
    public string? PackageNo { get; set; }
}

/// <summary>预约上门取件、子母件等发货信息字段（官方 <c>delivery_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillDeliveryInfo
{
    /// <summary>获取或设置发货方式（官方 <c>delivery_type</c>）：0 仓库发货；1 门店预约发货。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }

    /// <summary>获取或设置预约上门开始时间（官方 <c>collected_time_begin</c>，秒级时间戳，delivery_type=1 时必填）。</summary>
    [JsonPropertyName("collected_time_begin")]
    public long? CollectedTimeBegin { get; set; }

    /// <summary>获取或设置预约上门结束时间（官方 <c>collected_time_end</c>，秒级时间戳，delivery_type=1 时必填）。</summary>
    [JsonPropertyName("collected_time_end")]
    public long? CollectedTimeEnd { get; set; }

    /// <summary>获取或设置子母件包裹的数量（官方 <c>package_quantity</c>，2 ≤ value ≤ 300；不传为普通件）。</summary>
    [JsonPropertyName("package_quantity")]
    public int? PackageQuantity { get; set; }

    /// <summary>获取或设置包裹的体积和重量信息（官方 <c>subpackage_list</c>，顺丰支持）。</summary>
    [JsonPropertyName("subpackage_list")]
    public List<ChannelsLogisticsEwaybillSubpackage>? SubpackageList { get; set; }
}

/// <summary>电子面单取号 / 预取号请求共用形态（<c>ewaybill/biz/order/create</c> / <c>precreate</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCreateOrderRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id；create 时从预取号获取或自定义，数据内容要求 Uint64 以 String 传递）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填，经获取快递公司列表查询）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>ewaybill_acct_id</c>，必填，从 ewaybill_getacct 获取）。</summary>
    [JsonPropertyName("ewaybill_acct_id")]
    public string EwaybillAcctId { get; set; } = string.Empty;

    /// <summary>获取或设置寄件人（官方 <c>sender</c>，必填，传明文）。</summary>
    [JsonPropertyName("sender")]
    public ChannelsLogisticsEwaybillAddress? Sender { get; set; }

    /// <summary>获取或设置收件人（官方 <c>receiver</c>，必填，传小店订单内用户信息即可；供货商代发单无需上传）。</summary>
    [JsonPropertyName("receiver")]
    public ChannelsLogisticsEwaybillAddress? Receiver { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>ec_order_list</c>，必填）。</summary>
    [JsonPropertyName("ec_order_list")]
    public List<ChannelsLogisticsEwaybillEcOrder>? EcOrderList { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>，必填，每个商家分配的唯一 shop_id）。</summary>
    [JsonPropertyName("shop_id")]
    public string ShopId { get; set; } = string.Empty;

    /// <summary>获取或设置退货地址（官方 <c>return_address</c>，create 专用）。</summary>
    [JsonPropertyName("return_address")]
    public ChannelsLogisticsEwaybillAddress? ReturnAddress { get; set; }

    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，如需获取打印报文则填；无需后台模板可直接传 template_type 如 'single'）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置支持的类型（官方 <c>order_type</c>，枚举默认为 1，加盟型可不填）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置保价等增值服务（官方 <c>order_vas_list</c>）。</summary>
    [JsonPropertyName("order_vas_list")]
    public List<ChannelsLogisticsEwaybillVas>? OrderVasList { get; set; }

    /// <summary>获取或设置温层等补充字段（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsLogisticsEwaybillExtInfo? ExtInfo { get; set; }

    /// <summary>获取或设置预约上门取件、子母件等发货信息字段（官方 <c>delivery_info</c>）。</summary>
    [JsonPropertyName("delivery_info")]
    public ChannelsLogisticsEwaybillDeliveryInfo? DeliveryInfo { get; set; }
}

/// <summary>子母单号列表项（官方 <c>waybill_id_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillWaybill
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置快递单号类型（官方 <c>waybill_type</c>）：1 母单号；2 子单号。</summary>
    [JsonPropertyName("waybill_type")]
    public int? WaybillType { get; set; }

    /// <summary>获取或设置单号创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}

/// <summary>疑似存在风险的订单目录（官方 <c>order_risk_info[].risk_ec_order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillRiskEcOrder
{
    /// <summary>获取或设置订单 id（官方 <c>ec_order_id</c>）。</summary>
    [JsonPropertyName("ec_order_id")]
    public long? EcOrderId { get; set; }

    /// <summary>获取或设置订单商品信息（官方 <c>goods_list</c>）。</summary>
    [JsonPropertyName("goods_list")]
    public List<ChannelsLogisticsEwaybillGoods>? GoodsList { get; set; }

    /// <summary>获取或设置电子面单跨店铺取号的订单密文（官方 <c>ewaybill_order_code</c>）。</summary>
    [JsonPropertyName("ewaybill_order_code")]
    public string? EwaybillOrderCode { get; set; }

    /// <summary>获取或设置跨店铺取号的订单所属店铺 appid（官方 <c>ewaybill_order_appid</c>）。</summary>
    [JsonPropertyName("ewaybill_order_appid")]
    public string? EwaybillOrderAppid { get; set; }
}

/// <summary>商品订单疑似风险信息（官方 <c>order_risk_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillRiskInfo
{
    /// <summary>获取或设置疑似存在风险的订单目录（官方 <c>risk_ec_order_info</c>）。</summary>
    [JsonPropertyName("risk_ec_order_info")]
    public ChannelsLogisticsEwaybillRiskEcOrder? RiskEcOrderInfo { get; set; }

    /// <summary>获取或设置风险信息内容（官方 <c>risk_msg</c>）。</summary>
    [JsonPropertyName("risk_msg")]
    public string? RiskMsg { get; set; }
}

/// <summary>偏远中转集运补充物流信息（官方 <c>consolidation_waybill_info</c>，含 B 段运单信息）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillConsolidationInfo
{
    /// <summary>获取或设置偏远中转集运 B 段运单的物流商 id（官方 <c>b_segment_delivery_id</c>）。</summary>
    [JsonPropertyName("b_segment_delivery_id")]
    public string? BSegmentDeliveryId { get; set; }

    /// <summary>获取或设置偏远中转集运 B 段运单的电子面单 id（官方 <c>b_segment_ewaybill_order_id</c>）。</summary>
    [JsonPropertyName("b_segment_ewaybill_order_id")]
    public string? BSegmentEwaybillOrderId { get; set; }

    /// <summary>获取或设置偏远中转集运 B 段运单的运单号（官方 <c>b_segment_waybill_id</c>）。</summary>
    [JsonPropertyName("b_segment_waybill_id")]
    public string? BSegmentWaybillId { get; set; }
}

/// <summary>电子面单取号（<c>ewaybill/biz/order/create</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCreateOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置快递公司错误码（官方 <c>delivery_error_msg</c>）。</summary>
    [JsonPropertyName("delivery_error_msg")]
    public string? DeliveryErrorMsg { get; set; }

    /// <summary>获取或设置打印报文信息（官方 <c>print_info</c>，请求填了 template_id 时返回）。</summary>
    [JsonPropertyName("print_info")]
    public string? PrintInfo { get; set; }

    /// <summary>获取或设置子母单号列表（官方 <c>waybill_id_list</c>）。</summary>
    [JsonPropertyName("waybill_id_list")]
    public List<ChannelsLogisticsEwaybillWaybill>? WaybillIdList { get; set; }

    /// <summary>获取或设置商品订单疑似风险信息（官方 <c>order_risk_info</c>）。</summary>
    [JsonPropertyName("order_risk_info")]
    public List<ChannelsLogisticsEwaybillRiskInfo>? OrderRiskInfo { get; set; }

    /// <summary>获取或设置偏远中转集运补充物流信息（官方 <c>consolidation_waybill_info</c>）。</summary>
    [JsonPropertyName("consolidation_waybill_info")]
    public List<ChannelsLogisticsEwaybillConsolidationInfo>? ConsolidationWaybillInfo { get; set; }
}

/// <summary>电子面单预取号（<c>ewaybill/biz/order/precreate</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillPrecreateOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，用于取号接口请求参数）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }
}

/// <summary>电子面单订单 id 查询请求（<c>ewaybill/biz/order/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetOrderRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;
}

/// <summary>电子面单轨迹信息（官方 <c>order_info.path_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillPath
{
    /// <summary>获取或设置轨迹状态（官方 <c>status</c>）：0 待揽件；1 揽收；2 运输中；3 派件；4 入柜；5 签收；6 退回；7 转寄；8 异常；9 出柜；99 其他未知。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置轨迹描述（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>电子面单详情（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillOrderInfo
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>ewaybill_acct_id</c>）。</summary>
    [JsonPropertyName("ewaybill_acct_id")]
    public string? EwaybillAcctId { get; set; }

    /// <summary>获取或设置寄件人（官方 <c>sender</c>）。</summary>
    [JsonPropertyName("sender")]
    public ChannelsLogisticsEwaybillAddress? Sender { get; set; }

    /// <summary>获取或设置收件人（官方 <c>receiver</c>）。</summary>
    [JsonPropertyName("receiver")]
    public ChannelsLogisticsEwaybillAddress? Receiver { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>ec_order_list</c>）。</summary>
    [JsonPropertyName("ec_order_list")]
    public List<ChannelsLogisticsEwaybillEcOrder>? EcOrderList { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置电子面单状态（官方 <c>status</c>）：1 下单成功；2 取消；3 已打单；4 已发货。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置轨迹信息（官方 <c>path_info</c>）。</summary>
    [JsonPropertyName("path_info")]
    public List<ChannelsLogisticsEwaybillPath>? PathInfo { get; set; }

    /// <summary>获取或设置订单类型（官方 <c>order_type</c>）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置月结账号（官方 <c>monthly_card</c>）。</summary>
    [JsonPropertyName("monthly_card")]
    public string? MonthlyCard { get; set; }

    /// <summary>获取或设置保价等增值服务（官方 <c>order_vas_list</c>）。</summary>
    [JsonPropertyName("order_vas_list")]
    public List<ChannelsLogisticsEwaybillVas>? OrderVasList { get; set; }

    /// <summary>获取或设置温层等补充字段（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsLogisticsEwaybillExtInfo? ExtInfo { get; set; }
}

/// <summary>查询面单详情（<c>ewaybill/biz/order/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置电子面单列表（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsLogisticsEwaybillOrderInfo? OrderInfo { get; set; }
}

/// <summary>电子面单取消下单（<c>ewaybill/biz/order/cancel</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCancelOrderRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;
}

/// <summary>电子面单取消下单（<c>ewaybill/biz/order/cancel</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCancelOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置快递公司错误码（官方 <c>delivery_error_msg</c>）。</summary>
    [JsonPropertyName("delivery_error_msg")]
    public string? DeliveryErrorMsg { get; set; }
}

/// <summary>打印成功通知 / 子件追加 共用的打印请求字段形态（<c>ewaybill/biz/order/print</c> / <c>addsuborder</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillPrintRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置补打标记（官方 <c>re_print</c>，必填）：1 补打单成功；0 首次打单成功。</summary>
    [JsonPropertyName("re_print")]
    public int RePrint { get; set; }
}

/// <summary>批量打印通知请求项（官方 <c>req_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillBatchPrintItem
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置补打标记（官方 <c>re_print</c>，必填）：1 补打单成功；0 首次打单成功。</summary>
    [JsonPropertyName("re_print")]
    public int RePrint { get; set; }
}

/// <summary>批量打印通知（<c>ewaybill/biz/order/batchprint</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillBatchPrintOrderRequest
{
    /// <summary>获取或设置订单信息（官方 <c>req_list</c>，必填，单次不超过 50 个）。</summary>
    [JsonPropertyName("req_list")]
    public List<ChannelsLogisticsEwaybillBatchPrintItem> ReqList { get; set; } = new();
}

/// <summary>批量打印通知（<c>ewaybill/biz/order/batchprint</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillBatchPrintOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置成功的单号（官方 <c>succ_ewaybill_order_id</c>）。</summary>
    [JsonPropertyName("succ_ewaybill_order_id")]
    public List<string>? SuccEwaybillOrderId { get; set; }

    /// <summary>获取或设置失败的单号（官方 <c>fail_ewaybill_order_id</c>）。</summary>
    [JsonPropertyName("fail_ewaybill_order_id")]
    public List<string>? FailEwaybillOrderId { get; set; }
}

/// <summary>电子面单子件追加（<c>ewaybill/biz/order/addsuborder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAddSubOrderRequest
{
    /// <summary>获取或设置电子面单订单号（官方 <c>ewaybill_order_id</c>，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置运单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置追加面单数量（官方 <c>add_package_quantity</c>，必填，1 ≤ value ≤ 300）。</summary>
    [JsonPropertyName("add_package_quantity")]
    public int AddPackageQuantity { get; set; }

    /// <summary>获取或设置面单模板 id（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置店铺 id（官方 <c>shop_id</c>，必填，从查询开通账号信息接口获取）。</summary>
    [JsonPropertyName("shop_id")]
    public string ShopId { get; set; } = string.Empty;

    /// <summary>获取或设置电子面单账号 id（官方 <c>ewaybill_acct_id</c>，必填，从查询开通账号信息接口获取）。</summary>
    [JsonPropertyName("ewaybill_acct_id")]
    public string EwaybillAcctId { get; set; } = string.Empty;

    /// <summary>获取或设置包裹的体积和重量信息（官方 <c>subpackage_list</c>，顺丰支持）。</summary>
    [JsonPropertyName("subpackage_list")]
    public List<ChannelsLogisticsEwaybillSubpackage>? SubpackageList { get; set; }

    /// <summary>获取或设置子单额外信息（官方 <c>sub_order_ext_info</c>，如商家与快递定义的交易分单类型）。</summary>
    [JsonPropertyName("sub_order_ext_info")]
    public string? SubOrderExtInfo { get; set; }
}

/// <summary>电子面单子件追加（<c>ewaybill/biz/order/addsuborder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAddSubOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置快递公司错误码（官方 <c>delivery_error_msg</c>）。</summary>
    [JsonPropertyName("delivery_error_msg")]
    public string? DeliveryErrorMsg { get; set; }

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置子母单号列表（官方 <c>waybill_id_list</c>）。</summary>
    [JsonPropertyName("waybill_id_list")]
    public List<ChannelsLogisticsEwaybillWaybill>? WaybillIdList { get; set; }

    /// <summary>获取或设置打印报文信息（官方 <c>print_info</c>，请求填了 template_id 时返回）。</summary>
    [JsonPropertyName("print_info")]
    public string? PrintInfo { get; set; }

    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }
}

/// <summary>查询开通的快递公司列表（<c>ewaybill/biz/delivery/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetDeliveryListRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>，枚举值见官方文档）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置状态过滤（官方 <c>status</c>，不填则全选）：1 绑定审核中；2 取消绑定审核中；3 已绑定；4 已解除绑定；5 绑定未通过；6 取消绑定未通过。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>快递公司列表项（官方 <c>list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillDelivery
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置快递公司名称（官方 <c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>查询开通的快递公司列表（<c>ewaybill/biz/delivery/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetDeliveryListResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺 id（官方 <c>shop_id</c>，全局唯一，一个店铺分配一个 shop_id）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置快递公司列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLogisticsEwaybillDelivery>? List { get; set; }
}

/// <summary>获取打印报文（<c>ewaybill/biz/print/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetPrintContentRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id，必填）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string EwaybillOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，必填；无需后台模板可直接传 template_type 如 'single'）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置子单的物流单号（官方 <c>sub_waybill_id</c>，不传则返回整单的打印报文）。</summary>
    [JsonPropertyName("sub_waybill_id")]
    public string? SubWaybillId { get; set; }
}

/// <summary>获取打印报文（<c>ewaybill/biz/print/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetPrintContentResponse : ChannelsResponse
{
    /// <summary>获取或设置电子面单打印报文（官方 <c>print_info</c>，打单时传给打印机使用）。</summary>
    [JsonPropertyName("print_info")]
    public string? PrintInfo { get; set; }
}

/// <summary>自定义模板配置（官方 <c>config.custom_config</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateCustomConfig
{
    /// <summary>获取或设置自定义区域宽度（官方 <c>width</c>，单位 px，10px = 1 毫米）。</summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>获取或设置自定义区域高度（官方 <c>height</c>，单位 px，10px = 1 毫米）。</summary>
    [JsonPropertyName("height")]
    public int? Height { get; set; }

    /// <summary>获取或设置自定义区域到顶部距离（官方 <c>top</c>，单位 px，10px = 1 毫米）。</summary>
    [JsonPropertyName("top")]
    public int? Top { get; set; }

    /// <summary>获取或设置自定义区域到左边距离（官方 <c>left</c>，单位 px，10px = 1 毫米）。</summary>
    [JsonPropertyName("left")]
    public int? Left { get; set; }
}

/// <summary>面单标准模板项（官方 <c>config</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateConfig
{
    /// <summary>获取或设置模板类型（官方 <c>type</c>，默认填 'single'）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置模板描述（官方 <c>desc</c>，默认填「一联单标准模板」）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置面单宽度（官方 <c>width</c>，单位毫米）。</summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>获取或设置面单高度（官方 <c>height</c>，单位毫米）。</summary>
    [JsonPropertyName("height")]
    public int? Height { get; set; }

    /// <summary>获取或设置标准模板（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置自定义模板配置（官方 <c>custom_config</c>）。</summary>
    [JsonPropertyName("custom_config")]
    public ChannelsLogisticsEwaybillTemplateCustomConfig? CustomConfig { get; set; }
}

/// <summary>获取面单标准模板（<c>ewaybill/biz/template/config</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetTemplateConfigResponse : ChannelsResponse
{
    /// <summary>
    /// 获取或设置所有快递公司模板信息汇总（官方 <c>config</c>，键为快递公司 delivery_id，值为模板类型 → 模板项字典）。
    /// </summary>
    [JsonPropertyName("config")]
    public Dictionary<string, Dictionary<string, ChannelsLogisticsEwaybillTemplateConfig>>? Config { get; set; }
}

/// <summary>面单模板信息选项（官方 <c>template_info.options</c>，总数必须为 8 个，顺序按数组序）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateOption
{
    /// <summary>获取或设置信息类型（官方 <c>option_id</c>，填 0-7）：0 商品总数量；1 商品名称+规格+编码+数量；2 商品名称+规格+数量；3 商品名称+数量；4 店铺名称；5 订单号；6 买家留言；7 卖家备注。</summary>
    [JsonPropertyName("option_id")]
    public int OptionId { get; set; }

    /// <summary>获取或设置打印字号（官方 <c>font_size</c>，0 默认大小，1 加大）。</summary>
    [JsonPropertyName("font_size")]
    public int FontSize { get; set; }

    /// <summary>获取或设置字体是否加粗（官方 <c>is_bold</c>）。</summary>
    [JsonPropertyName("is_bold")]
    public bool? IsBold { get; set; }

    /// <summary>获取或设置是否选用此信息（官方 <c>is_open</c>）。</summary>
    [JsonPropertyName("is_open")]
    public bool? IsOpen { get; set; }
}

/// <summary>面单模板信息（官方 <c>template_info</c> / <c>template_list</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateInfo
{
    /// <summary>获取或设置模板 id（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置模板名（官方 <c>template_name</c>，同一快递公司下不可重复）。</summary>
    [JsonPropertyName("template_name")]
    public string? TemplateName { get; set; }

    /// <summary>获取或设置模板描述（官方 <c>template_desc</c>，默认填「一联单标准模板」）。</summary>
    [JsonPropertyName("template_desc")]
    public string? TemplateDesc { get; set; }

    /// <summary>获取或设置模板类型（官方 <c>template_type</c>，默认填 'single'）。</summary>
    [JsonPropertyName("template_type")]
    public string? TemplateType { get; set; }

    /// <summary>获取或设置模板信息选项（官方 <c>options</c>，总数必须为 8 个）。</summary>
    [JsonPropertyName("options")]
    public List<ChannelsLogisticsEwaybillTemplateOption>? Options { get; set; }

    /// <summary>获取或设置是否为该快递公司默认模板（官方 <c>is_default</c>）。</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>获取或设置模板创建时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置模板更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>根据模板 ID 获取面单模板信息（<c>ewaybill/biz/template/getbyid</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetTemplateByIdRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}

/// <summary>根据模板 ID 获取面单模板信息（<c>ewaybill/biz/template/getbyid</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetTemplateByIdResponse : ChannelsResponse
{
    /// <summary>获取或设置模板信息（官方 <c>template_info</c>）。</summary>
    [JsonPropertyName("template_info")]
    public ChannelsLogisticsEwaybillTemplateInfo? TemplateInfo { get; set; }
}

/// <summary>某个快递公司的模板信息汇总（官方 <c>total_template</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTotalTemplate
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置该快递公司的默认模板 id（官方 <c>default_template_id</c>）。</summary>
    [JsonPropertyName("default_template_id")]
    public string? DefaultTemplateId { get; set; }

    /// <summary>获取或设置模板信息列表（官方 <c>template_list</c>）。</summary>
    [JsonPropertyName("template_list")]
    public List<ChannelsLogisticsEwaybillTemplateInfo>? TemplateList { get; set; }
}

/// <summary>获取面单模板信息（<c>ewaybill/biz/template/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetTemplateRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }
}

/// <summary>获取面单模板信息（<c>ewaybill/biz/template/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetTemplateResponse : ChannelsResponse
{
    /// <summary>获取或设置所有快递公司模板信息汇总（官方 <c>total_template</c>）。</summary>
    [JsonPropertyName("total_template")]
    public List<ChannelsLogisticsEwaybillTotalTemplate>? TotalTemplate { get; set; }
}

/// <summary>新增 / 更新面单模板请求信息（官方 <c>info</c>，create 与 update 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateInfoRequest
{
    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，update 必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置模板名（官方 <c>template_name</c>，必填，同一快递公司下不可重复）。</summary>
    [JsonPropertyName("template_name")]
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>获取或设置模板描述（官方 <c>template_desc</c>，默认填「一联单标准模板」）。</summary>
    [JsonPropertyName("template_desc")]
    public string? TemplateDesc { get; set; }

    /// <summary>获取或设置模板类型（官方 <c>template_type</c>，默认填 'single'）。</summary>
    [JsonPropertyName("template_type")]
    public string? TemplateType { get; set; }

    /// <summary>获取或设置是否为该快递公司默认模板（官方 <c>is_default</c>，必填）。</summary>
    [JsonPropertyName("is_default")]
    public bool IsDefault { get; set; }

    /// <summary>获取或设置模板信息选项（官方 <c>options</c>，必填，总数必须为 8 个，option_id 不得重复）。</summary>
    [JsonPropertyName("options")]
    public List<ChannelsLogisticsEwaybillTemplateOption> Options { get; set; } = new();
}

/// <summary>新增面单模板（<c>ewaybill/biz/template/create</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCreateTemplateRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置模板信息（官方 <c>info</c>，必填）。</summary>
    [JsonPropertyName("info")]
    public ChannelsLogisticsEwaybillTemplateInfoRequest Info { get; set; } = new();
}

/// <summary>新增面单模板（<c>ewaybill/biz/template/create</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCreateTemplateResponse : ChannelsResponse
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置模板编码（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }
}

/// <summary>更新面单模板（<c>ewaybill/biz/template/update</c>）请求体（原地覆盖写入，须带全字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillUpdateTemplateRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置模板信息（官方 <c>info</c>，必填）。</summary>
    [JsonPropertyName("info")]
    public ChannelsLogisticsEwaybillTemplateInfoRequest Info { get; set; } = new();
}

/// <summary>删除面单模板（<c>ewaybill/biz/template/delete</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillDeleteTemplateRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}

/// <summary>网点地址信息（官方 <c>account_list[].site_info.address</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSiteAddress
{
    /// <summary>获取或设置城市编码（官方 <c>city_code</c>）。</summary>
    [JsonPropertyName("city_code")]
    public string? CityCode { get; set; }

    /// <summary>获取或设置城市名（官方 <c>city_name</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置国家编码（官方 <c>country_code</c>）。</summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail_address</c>）。</summary>
    [JsonPropertyName("detail_address")]
    public string? DetailAddress { get; set; }

    /// <summary>获取或设置区县编码（官方 <c>district_code</c>）。</summary>
    [JsonPropertyName("district_code")]
    public string? DistrictCode { get; set; }

    /// <summary>获取或设置区县名（官方 <c>district_name</c>）。</summary>
    [JsonPropertyName("district_name")]
    public string? DistrictName { get; set; }

    /// <summary>获取或设置省编码（官方 <c>province_code</c>）。</summary>
    [JsonPropertyName("province_code")]
    public string? ProvinceCode { get; set; }

    /// <summary>获取或设置省份名（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置街道编码（官方 <c>street_code</c>）。</summary>
    [JsonPropertyName("street_code")]
    public string? StreetCode { get; set; }

    /// <summary>获取或设置街道名（官方 <c>street_name</c>）。</summary>
    [JsonPropertyName("street_name")]
    public string? StreetName { get; set; }
}

/// <summary>网点联系信息（官方 <c>account_list[].site_info.contact</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSiteContact
{
    /// <summary>获取或设置移动电话（官方 <c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>获取或设置名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置电话（官方 <c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}

/// <summary>网点信息（官方 <c>account_list[].site_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSiteInfo
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置网点运营状态（官方 <c>site_status</c>）：1 正常；其他值则不正常。</summary>
    [JsonPropertyName("site_status")]
    public int? SiteStatus { get; set; }

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置网点名字（官方 <c>site_name</c>）。</summary>
    [JsonPropertyName("site_name")]
    public string? SiteName { get; set; }

    /// <summary>获取或设置地址信息（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public ChannelsLogisticsEwaybillSiteAddress? Address { get; set; }

    /// <summary>获取或设置联系信息（官方 <c>contact</c>）。</summary>
    [JsonPropertyName("contact")]
    public ChannelsLogisticsEwaybillSiteContact? Contact { get; set; }

    /// <summary>获取或设置地址全名（官方 <c>site_fullname</c>）。</summary>
    [JsonPropertyName("site_fullname")]
    public string? SiteFullname { get; set; }
}

/// <summary>共享账号发起方信息（官方 <c>account_list[].share</c>，acct_type 为共享账号时有效）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillShare
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置网点名字（官方 <c>site_name</c>）。</summary>
    [JsonPropertyName("site_name")]
    public string? SiteName { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>acct_id</c>）。</summary>
    [JsonPropertyName("acct_id")]
    public string? AcctId { get; set; }

    /// <summary>获取或设置发起共享方店铺名（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置共享 id（官方 <c>share_id</c>）。</summary>
    [JsonPropertyName("share_id")]
    public string? ShareId { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置直营型月结账号（官方 <c>monthly_card</c>）。</summary>
    [JsonPropertyName("monthly_card")]
    public string? MonthlyCard { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>绑定的发货地址信息（官方 <c>account_list[].sender_address</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSenderAddress
{
    /// <summary>获取或设置省（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置区县（官方 <c>county</c>）。</summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>获取或设置街道（官方 <c>street</c>）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>面单账号信息（官方 <c>account_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAccount
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置面单账号类型（官方 <c>acct_type</c>）：0 普通账号；1 共享账号。</summary>
    [JsonPropertyName("acct_type")]
    public int? AcctType { get; set; }

    /// <summary>获取或设置快递公司类型（官方 <c>company_type</c>）：1 加盟型；2 直营型。</summary>
    [JsonPropertyName("company_type")]
    public int? CompanyType { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>acct_id</c>，每绑定一个物流商网点/月结账号分配一个）。</summary>
    [JsonPropertyName("acct_id")]
    public string? AcctId { get; set; }

    /// <summary>获取或设置面单账号状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置面单余额（官方 <c>available</c>）。</summary>
    [JsonPropertyName("available")]
    public long? Available { get; set; }

    /// <summary>获取或设置累积已取单（官方 <c>allocated</c>）。</summary>
    [JsonPropertyName("allocated")]
    public long? Allocated { get; set; }

    /// <summary>获取或设置累积已回收（官方 <c>recycled</c>）。</summary>
    [JsonPropertyName("recycled")]
    public long? Recycled { get; set; }

    /// <summary>获取或设置累计已取消（官方 <c>cancel</c>）。</summary>
    [JsonPropertyName("cancel")]
    public long? Cancel { get; set; }

    /// <summary>获取或设置月结账号（官方 <c>monthly_card</c>，company_type 为直营型时有效）。</summary>
    [JsonPropertyName("monthly_card")]
    public string? MonthlyCard { get; set; }

    /// <summary>获取或设置网点信息（官方 <c>site_info</c>）。</summary>
    [JsonPropertyName("site_info")]
    public ChannelsLogisticsEwaybillSiteInfo? SiteInfo { get; set; }

    /// <summary>获取或设置共享账号发起方信息（官方 <c>share</c>，acct_type 为共享账号时有效）。</summary>
    [JsonPropertyName("share")]
    public ChannelsLogisticsEwaybillShare? Share { get; set; }

    /// <summary>获取或设置绑定的发货地址信息（官方 <c>sender_address</c>）。</summary>
    [JsonPropertyName("sender_address")]
    public ChannelsLogisticsEwaybillSenderAddress? SenderAddress { get; set; }

    /// <summary>获取或设置查询库存的返回码（官方 <c>balance_retcode</c>）。</summary>
    [JsonPropertyName("balance_retcode")]
    public int? BalanceRetcode { get; set; }

    /// <summary>获取或设置查询库存的返回信息（官方 <c>balance_retmsg</c>）。</summary>
    [JsonPropertyName("balance_retmsg")]
    public string? BalanceRetmsg { get; set; }

    /// <summary>获取或设置查询库存失败时快递公司返回的详情（官方 <c>delivery_msg</c>）。</summary>
    [JsonPropertyName("delivery_msg")]
    public string? DeliveryMsg { get; set; }
}

/// <summary>查询开通的电子面单网点 / 账号信息（<c>ewaybill/biz/account/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetAcctRequest
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置是否需要查询库存（官方 <c>need_balance</c>，必填）。</summary>
    [JsonPropertyName("need_balance")]
    public bool NeedBalance { get; set; }

    /// <summary>获取或设置状态过滤（官方 <c>status</c>）：1 绑定审核中；2 取消绑定审核中；3 已绑定；4 已解除绑定；5 绑定未通过；6 取消绑定未通过。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置分页起始位置（官方 <c>offset</c>，默认 0）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置单次请求数量（官方 <c>limit</c>，必填；need_balance=true 时建议小于 20，否则 limit 过大导致接口超时）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>acct_id</c>，每绑定一个物流商网点/月结账号分配一个）。</summary>
    [JsonPropertyName("acct_id")]
    public string? AcctId { get; set; }
}

/// <summary>查询开通的电子面单网点 / 账号信息（<c>ewaybill/biz/account/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetAcctResponse : ChannelsResponse
{
    /// <summary>获取或设置总条数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置账号列表（官方 <c>account_list</c>）。</summary>
    [JsonPropertyName("account_list")]
    public List<ChannelsLogisticsEwaybillAccount>? AccountList { get; set; }
}
