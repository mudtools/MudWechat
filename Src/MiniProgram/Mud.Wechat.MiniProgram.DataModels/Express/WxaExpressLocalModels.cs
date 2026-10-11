// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 即时配送响应<b>基类</b>：承载官方「运力返回」的 <c>resultcode</c> / <c>resultmsg</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不直接用 <see cref="WxaResponse"/></b>：即时配送（<c>/cgi-bin/express/local/*</c>）全族
/// 官方返回体用 <c>resultcode</c> / <c>resultmsg</c> 表达运力侧结果，<b>而非</b> <c>errcode</c> / <c>errmsg</c>
/// （官方各页「返回体 Response Payload」2026-10-12 逐页核验）。本类型在 <see cref="WxaResponse"/> 之上
/// 补齐这两个字段，使调用方既能走通用信封、也能读运力侧结果码。
/// </para>
/// <para><b>判错口径</b>：<c>resultcode</c> 为 <c>0</c> 表示运力侧成功；非 0 时以 <c>resultmsg</c> 为准。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalResponse : WxaResponse
{
    /// <summary>运力返回的错误码（<c>resultcode</c>；<c>0</c> 表示成功）。</summary>
    [JsonPropertyName("resultcode")]
    public int ResultCode { get; set; }

    /// <summary>运力返回的错误描述（<c>resultmsg</c>）。</summary>
    [JsonPropertyName("resultmsg")]
    public string? ResultMessage { get; set; }
}

/// <summary>即时配送「发件人 / 收件人」信息（<c>sender</c> / <c>receiver</c> 内嵌对象，同构）。</summary>
/// <remarks>
/// <b>与 <see cref="WxaExpressContact"/> 的差异（勿混用）</b>：本结构带 <c>city</c> / <c>address</c> /
/// <c>address_detail</c> 三级地址与 <c>lng</c> / <c>lat</c> 坐标，<b>无</b> <c>tel</c> / <c>mobile</c> 二分。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalContact
{
    /// <summary>姓名（<c>name</c>，必填；最长不超过 256 个字符）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>城市名称（<c>city</c>，必填；如「广州市」）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>地址（<c>address</c>，必填；街道、小区、大厦等，用于定位）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>地址详情（<c>address_detail</c>，必填；楼号、单元号、层号）。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>电话/手机号（<c>phone</c>，必填；最长不超过 64 个字符）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>经度（<c>lng</c>，必填；精确到小数点后 6 位；与 <c>coordinate_type</c> 配合使用）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>，必填；精确到小数点后 6 位；与 <c>coordinate_type</c> 配合使用）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>坐标类型（<c>coordinate_type</c>，可选；<c>0</c> 火星坐标——高德/腾讯地图，<c>1</c> 百度坐标）。</summary>
    [JsonPropertyName("coordinate_type")]
    public int? CoordinateType { get; set; }
}

/// <summary>即时配送货品明细项（<c>cargo.goods_detail.goods</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGood
{
    /// <summary>货物数量（<c>good_count</c>，必填）。</summary>
    [JsonPropertyName("good_count")]
    public int GoodCount { get; set; }

    /// <summary>货品名称（<c>good_name</c>，必填）。</summary>
    [JsonPropertyName("good_name")]
    public string? GoodName { get; set; }

    /// <summary>货品单价（<c>good_price</c>，可选；精确到小数点后两位）。</summary>
    [JsonPropertyName("good_price")]
    public decimal? GoodPrice { get; set; }

    /// <summary>货品单位（<c>good_unit</c>，可选；最长不超过 20 个字符）。</summary>
    [JsonPropertyName("good_unit")]
    public string? GoodUnit { get; set; }
}

/// <summary>即时配送货物详情（<c>cargo.goods_detail</c> 内嵌对象；最长不超过 10240 个字符）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGoodsDetail
{
    /// <summary>货物列表（<c>goods</c>，必填），见 <see cref="WxaExpressLocalGood"/>。</summary>
    [JsonPropertyName("goods")]
    public WxaExpressLocalGood[]? Goods { get; set; }
}

/// <summary>即时配送货物信息（<c>cargo</c> 内嵌对象；下单 / 预下单 / 重新下单复用）。</summary>
/// <remarks>
/// 官方数值口径：金额单位为<b>元</b>、重量单位为 <b>kg</b>、尺寸单位为 <b>cm</b>，均精确到小数点后两位；
/// 取值范围 <c>goods_value</c> (0-5000]、<c>goods_height</c> (0-45]、<c>goods_width</c> (0-50]、
/// <c>goods_length</c> (0-65]、<c>goods_weight</c> (0-50]。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalCargo
{
    /// <summary>货物价格（<c>goods_value</c>，必填；单位：元，范围 (0-5000]）。</summary>
    [JsonPropertyName("goods_value")]
    public decimal GoodsValue { get; set; }

    /// <summary>货物高度（<c>goods_height</c>，可选；单位：cm，范围 (0-45]）。</summary>
    [JsonPropertyName("goods_height")]
    public decimal? GoodsHeight { get; set; }

    /// <summary>货物宽度（<c>goods_width</c>，可选；单位：cm，范围 (0-50]）。</summary>
    [JsonPropertyName("goods_width")]
    public decimal? GoodsWidth { get; set; }

    /// <summary>货物长度（<c>goods_length</c>，可选；单位：cm，范围 (0-65]）。</summary>
    [JsonPropertyName("goods_length")]
    public decimal? GoodsLength { get; set; }

    /// <summary>货物重量（<c>goods_weight</c>，必填；单位：kg，范围 (0-50]）。</summary>
    [JsonPropertyName("goods_weight")]
    public decimal GoodsWeight { get; set; }

    /// <summary>货物详情（<c>goods_detail</c>，可选），见 <see cref="WxaExpressLocalGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public WxaExpressLocalGoodsDetail? GoodsDetail { get; set; }

    /// <summary>货物取货信息（<c>goods_pickup_info</c>，可选；用于骑手到店取货，最长不超过 100 个字符）。</summary>
    [JsonPropertyName("goods_pickup_info")]
    public string? GoodsPickupInfo { get; set; }

    /// <summary>品类一级类目（<c>cargo_first_class</c>，必填；官方枚举，如「美食夜宵」「日用百货」「果蔬生鲜」等）。</summary>
    [JsonPropertyName("cargo_first_class")]
    public string? CargoFirstClass { get; set; }

    /// <summary>品类二级类目（<c>cargo_second_class</c>，必填）。</summary>
    [JsonPropertyName("cargo_second_class")]
    public string? CargoSecondClass { get; set; }
}

/// <summary>即时配送订单信息（<c>order_info</c> 内嵌对象；下单 / 预下单 / 重新下单复用）。</summary>
/// <remarks>
/// 除 <c>delivery_service_code</c> 外<b>全部可选</b>；<c>order_type=1</c>（预约单）时，
/// 官方要求设置 <c>expected_delivery_time</c> 或 <c>expected_finish_time</c> 或 <c>expected_pick_time</c> 之一。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalOrderInfo
{
    /// <summary>配送服务代码（<c>delivery_service_code</c>，可选；不同配送公司自定义，顺丰和达达不填）。</summary>
    [JsonPropertyName("delivery_service_code")]
    public string? DeliveryServiceCode { get; set; }

    /// <summary>期望派单时间（<c>expected_delivery_time</c>，可选；Unix 秒级时间戳，达达支持）。</summary>
    [JsonPropertyName("expected_delivery_time")]
    public long? ExpectedDeliveryTime { get; set; }

    /// <summary>订单类型（<c>order_type</c>，可选；<c>0</c> 即时单，<c>1</c> 预约单）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>门店订单流水号（<c>poi_seq</c>，可选；建议提供，方便骑手门店取货，最长不超过 32 个字符）。</summary>
    [JsonPropertyName("poi_seq")]
    public string? PoiSeq { get; set; }

    /// <summary>备注（<c>note</c>，可选；最长不超过 200 个字符）。</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>用户下单付款时间（<c>order_time</c>，可选；顺丰<b>必填</b>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("order_time")]
    public long? OrderTime { get; set; }

    /// <summary>是否保价（<c>is_insured</c>，可选；<c>0</c> 非保价，<c>1</c> 保价）。</summary>
    [JsonPropertyName("is_insured")]
    public int? IsInsured { get; set; }

    /// <summary>保价金额（<c>declared_value</c>，可选；单位：元，精确到分）。</summary>
    [JsonPropertyName("declared_value")]
    public decimal? DeclaredValue { get; set; }

    /// <summary>小费（<c>tips</c>，可选；单位：元；官方原文「下单一般不加小费」）。</summary>
    [JsonPropertyName("tips")]
    public decimal? Tips { get; set; }

    /// <summary>是否直拿直送（<c>is_direct_delivery</c>，可选；<c>0</c> 不需要，<c>1</c> 需要；闪送必须选 <c>1</c>）。</summary>
    [JsonPropertyName("is_direct_delivery")]
    public int? IsDirectDelivery { get; set; }

    /// <summary>骑手应付金额（<c>cash_on_delivery</c>，可选；单位：元，精确到分）。</summary>
    [JsonPropertyName("cash_on_delivery")]
    public decimal? CashOnDelivery { get; set; }

    /// <summary>骑手应收金额（<c>cash_on_pickup</c>，可选；单位：元，精确到分）。</summary>
    [JsonPropertyName("cash_on_pickup")]
    public decimal? CashOnPickup { get; set; }

    /// <summary>物流流向（<c>rider_pick_method</c>，可选；<c>1</c> 门店取件送至用户，<c>2</c> 用户取件送至门店）。</summary>
    [JsonPropertyName("rider_pick_method")]
    public int? RiderPickMethod { get; set; }

    /// <summary>是否需要收货码（<c>is_finish_code_needed</c>，可选；<c>0</c> 不需要，<c>1</c> 需要）。</summary>
    [JsonPropertyName("is_finish_code_needed")]
    public int? IsFinishCodeNeeded { get; set; }

    /// <summary>是否需要取货码（<c>is_pickup_code_needed</c>，可选；<c>0</c> 不需要，<c>1</c> 需要）。</summary>
    [JsonPropertyName("is_pickup_code_needed")]
    public int? IsPickupCodeNeeded { get; set; }

    /// <summary>期望送达时间（<c>expected_finish_time</c>，可选；Unix 秒级时间戳，美团、顺丰同城急送支持）。</summary>
    [JsonPropertyName("expected_finish_time")]
    public long? ExpectedFinishTime { get; set; }

    /// <summary>期望取货时间（<c>expected_pick_time</c>，可选；Unix 秒级时间戳）。</summary>
    /// <remarks>官方在 <c>order_type</c> 的说明中点名本字段为预约单三选一之一。</remarks>
    [JsonPropertyName("expected_pick_time")]
    public long? ExpectedPickTime { get; set; }
}

/// <summary>即时配送商品信息（<c>shop</c> 内嵌对象；会展示到物流通知消息中）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalShop
{
    /// <summary>商家小程序路径（<c>wxa_path</c>，可选；建议为订单页面）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }

    /// <summary>商品缩略图 URL（<c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImageUrl { get; set; }

    /// <summary>商品名称（<c>goods_name</c>；不超过 128 字节）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>goods_count</c>）。</summary>
    [JsonPropertyName("goods_count")]
    public int? GoodsCount { get; set; }

    /// <summary>
    /// 下单小程序的 AppId（<c>wxa_appid</c>，可选；<b>仅即时配送 <c>order/add</c> 生效</b>）。
    /// </summary>
    /// <remarks>结算方式为「第三方向配送公司统一结算」时<b>必填</b>：第三方用自己的小程序替授权商户下单，并传授权小程序 AppId，配送通知可回流授权商户小程序。</remarks>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppId { get; set; }
}

/// <summary>
/// 即时配送下单 / 预下单 / 重新下单<b>共用</b>请求体
/// （<c>POST /cgi-bin/express/local/business/order/{add,pre_add,readd}</c>）。
/// </summary>
/// <remarks>
/// <para>官方三端点请求体字段<b>完全一致</b>，仅 <c>delivery_token</c> 的适用性不同（见该字段说明）。</para>
/// <para>
/// <b>敏感字段</b>：<c>delivery_sign</c> 为「用配送公司提供的 <c>appSecret</c> 加密的校验串」——
/// 与 <c>secret</c> 同级敏感，<b>不得</b>入日志 / 遥测 / 异常消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAddOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成，不超过 128 字节）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>下单用户的 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>发件人信息（<c>sender</c>，必填），见 <see cref="WxaExpressLocalContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressLocalContact? Sender { get; set; }

    /// <summary>收件人信息（<c>receiver</c>，必填），见 <see cref="WxaExpressLocalContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressLocalContact? Receiver { get; set; }

    /// <summary>货物信息（<c>cargo</c>，必填），见 <see cref="WxaExpressLocalCargo"/>。</summary>
    [JsonPropertyName("cargo")]
    public WxaExpressLocalCargo? Cargo { get; set; }

    /// <summary>订单信息（<c>order_info</c>，必填），见 <see cref="WxaExpressLocalOrderInfo"/>。</summary>
    [JsonPropertyName("order_info")]
    public WxaExpressLocalOrderInfo? OrderInfo { get; set; }

    /// <summary>商品信息（<c>shop</c>，必填；会展示到物流通知消息中），见 <see cref="WxaExpressLocalShop"/>。</summary>
    [JsonPropertyName("shop")]
    public WxaExpressLocalShop? Shop { get; set; }

    /// <summary>
    /// 预下单令牌（<c>delivery_token</c>，必填）。
    /// </summary>
    /// <remarks>
    /// <b>官方原文</b>：<c>order/add</c> 与 <c>order/readd</c> 传「预下单接口返回的参数，配送公司可保证在一段时间内运费不变」；
    /// <c>order/pre_add</c> 的官方参数表中<b>无</b>本字段（预下单本身就是产出方）。
    /// </remarks>
    [JsonPropertyName("delivery_token")]
    public string? DeliveryToken { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；用配送公司提供的 appSecret 加密；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；美团、闪送必填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>子商户 ID（<c>sub_biz_id</c>，可选；区分小程序内部多个子商户）。</summary>
    [JsonPropertyName("sub_biz_id")]
    public string? SubBizId { get; set; }
}

/// <summary>即时配送下单 / 重新下单响应（<c>POST .../order/{add,readd}</c>）。</summary>
/// <remarks>
/// <b>官方字段名照原文</b>：<c>insurancfee</c>（保价费）在本页即为该拼写，<b>勿</b>改写成 <c>insurancefee</c>
/// （预下单页 <c>pre_add</c> 用的是 <c>insurancefee</c>，两页官方拼写不一致，各自照抄）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAddOrderResponse : WxaExpressLocalResponse
{
    /// <summary>实际运费（<c>fee</c>，单位：元；运费减去优惠券费用）。</summary>
    [JsonPropertyName("fee")]
    public decimal Fee { get; set; }

    /// <summary>运费（<c>deliverfee</c>，单位：元）。</summary>
    [JsonPropertyName("deliverfee")]
    public decimal DeliverFee { get; set; }

    /// <summary>优惠券费用（<c>couponfee</c>，单位：元）。</summary>
    [JsonPropertyName("couponfee")]
    public decimal CouponFee { get; set; }

    /// <summary>小费（<c>tips</c>，单位：元）。</summary>
    [JsonPropertyName("tips")]
    public decimal Tips { get; set; }

    /// <summary>保价费（<c>insurancfee</c>，单位：元；官方本页原文拼写）。</summary>
    [JsonPropertyName("insurancfee")]
    public decimal InsuranceFee { get; set; }

    /// <summary>配送距离（<c>distance</c>，整数，单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>配送单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>配送状态（<c>order_status</c>；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>收货码（<c>finish_code</c>）。</summary>
    [JsonPropertyName("finish_code")]
    public int FinishCode { get; set; }

    /// <summary>取货码（<c>pickup_code</c>）。</summary>
    [JsonPropertyName("pickup_code")]
    public int PickupCode { get; set; }

    /// <summary>预计骑手接单时间（<c>dispatch_duration</c>，单位：秒；无法预计填 <c>0</c>）。</summary>
    [JsonPropertyName("dispatch_duration")]
    public int DispatchDuration { get; set; }
}

/// <summary>即时配送预下单响应（<c>POST /cgi-bin/express/local/business/order/pre_add</c>）。</summary>
/// <remarks>
/// <b>运力差异（官方原文）</b>：达达支持返回配送费用 / 距离 / 预计接单时间与 <c>delivery_token</c>（有效期 3 分钟）；
/// 顺丰同城与闪送不返回 <c>delivery_token</c>；美团配送返回 <c>0</c> 仅表示校验通过。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalPreAddOrderResponse : WxaExpressLocalResponse
{
    /// <summary>实际运费（<c>fee</c>，单位：元；运费减去优惠券费用）。</summary>
    [JsonPropertyName("fee")]
    public decimal Fee { get; set; }

    /// <summary>运费（<c>deliverfee</c>，单位：元）。</summary>
    [JsonPropertyName("deliverfee")]
    public decimal DeliverFee { get; set; }

    /// <summary>优惠券费用（<c>couponFee</c>，单位：元；官方本页原文驼峰拼写）。</summary>
    [JsonPropertyName("couponFee")]
    public decimal CouponFee { get; set; }

    /// <summary>小费（<c>tips</c>，单位：元）。</summary>
    [JsonPropertyName("tips")]
    public decimal Tips { get; set; }

    /// <summary>保价费（<c>insurancefee</c>，单位：元）。</summary>
    [JsonPropertyName("insurancefee")]
    public decimal InsuranceFee { get; set; }

    /// <summary>配送距离（<c>distance</c>，单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>运费锁定令牌（<c>delivery_token</c>；配送公司可返回，下单时带上可保证一段时间内运费不变）。</summary>
    [JsonPropertyName("delivery_token")]
    public string? DeliveryToken { get; set; }

    /// <summary>预计骑手接单时间（<c>dispatch_duration</c>，单位：秒；无法预计填 <c>0</c>）。</summary>
    [JsonPropertyName("dispatch_duration")]
    public int DispatchDuration { get; set; }
}

/// <summary>即时配送拉取配送单信息请求体（<c>POST .../order/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGetOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记，只有一个门店时可不填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }
}

/// <summary>即时配送拉取配送单信息响应（<c>POST .../order/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGetOrderResponse : WxaExpressLocalResponse
{
    /// <summary>配送状态（<c>order_status</c>；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>配送单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>骑手姓名（<c>rider_name</c>）。</summary>
    [JsonPropertyName("rider_name")]
    public string? RiderName { get; set; }

    /// <summary>骑手电话（<c>rider_phone</c>；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("rider_phone")]
    public string? RiderPhoneNumber { get; set; }

    /// <summary>骑手位置经度（<c>rider_lng</c>；配送中时返回）。</summary>
    [JsonPropertyName("rider_lng")]
    public double? RiderLongitude { get; set; }

    /// <summary>骑手位置纬度（<c>rider_lat</c>；配送中时返回）。</summary>
    [JsonPropertyName("rider_lat")]
    public double? RiderLatitude { get; set; }

    /// <summary>预计还剩多久送达（<c>reach_time</c>，单位：秒；配送中时返回）。</summary>
    [JsonPropertyName("reach_time")]
    public int? ReachTime { get; set; }
}

/// <summary>
/// 即时配送取消 / 预取消配送单<b>共用</b>请求体（<c>POST .../order/{cancel,precancel}</c>）。
/// </summary>
/// <remarks>
/// <c>cancel_reason_id</c>：<c>1</c> 暂时不需要邮寄、<c>2</c> 价格不合适、<c>3</c> 订单信息有误重新下单、
/// <c>4</c> 骑手取货不及时、<c>5</c> 骑手配送不及时、<c>6</c> 其他原因（选 <c>6</c> 时须填 <c>cancel_reason</c>）。
/// 官方差异：<c>cancel</c> 页 <c>cancel_reason_id</c> / <c>cancel_reason</c> 为必填，<c>precancel</c> 页为可选。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalCancelOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填；顺丰同城必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>取消原因 ID（<c>cancel_reason_id</c>；枚举见类说明）。</summary>
    [JsonPropertyName("cancel_reason_id")]
    public int? CancelReasonId { get; set; }

    /// <summary>取消原因（<c>cancel_reason</c>；<c>cancel_reason_id=6</c> 时必填）。</summary>
    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；闪送必填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }
}

/// <summary>
/// 即时配送取消 / 预取消配送单<b>共用</b>响应（<c>POST .../order/{cancel,precancel}</c>）。
/// </summary>
/// <remarks><c>deduct_fee</c> 语义按端点区分：<c>cancel</c> 为<b>扣除</b>的违约金，<c>precancel</c> 为<b>预计扣除</b>的违约金（单位：元，精确到分）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalCancelOrderResponse : WxaExpressLocalResponse
{
    /// <summary>违约金（<c>deduct_fee</c>；单位：元，精确到分）。</summary>
    [JsonPropertyName("deduct_fee")]
    public decimal DeductFee { get; set; }

    /// <summary>说明（<c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}

/// <summary>即时配送异常件退回商家确认请求体（<c>POST .../order/confirm_return</c>）。</summary>
/// <remarks>官方场景：订单配送异常、骑手把货物退还给商家，商家收货后调用本接口确认收货。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalConfirmReturnRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；闪送必填，值为店铺 ID）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>备注（<c>remark</c>，可选）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>即时配送添加小费请求体（<c>POST .../order/addtips</c>）。</summary>
/// <remarks>
/// <b>官方原文</b>：仅可对<b>待接单状态</b>的订单加小费；小费以<b>最新一次</b>为准，故下次金额必须<b>大于</b>上一次。
/// 各家规则：顺丰同城急送（上限 10 次、200 元封顶）、闪送（固定档位 2/3/5/10/15/20/50/100）、美团配送（不支持）、达达（须大于上一次）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAddTipsRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>小费金额（<c>tips</c>，必填；单位：元；各家配送公司最大值不同）。</summary>
    [JsonPropertyName("tips")]
    public decimal Tips { get; set; }

    /// <summary>备注（<c>remark</c>，必填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；只有一个门店时闪送必填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }
}

/// <summary>即时配送发起绑定账号请求体（<c>POST .../shop/add</c>）。</summary>
/// <remarks><b>官方约束</b>：只能由第三方服务商调用；成功后小程序管理员会收到模板消息，点击详情进入配送公司网站完成绑定。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalBindAccountRequest
{
    /// <summary>配送公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }
}

/// <summary>配送公司项（<c>list</c> 元素，<c>delivery/getall</c> 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalDeliveryItem
{
    /// <summary>配送公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>配送公司名称（<c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>即时配送获取已支持的配送公司列表响应（<c>POST .../business/delivery/getall</c>；<b>无请求体</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGetAllDeliveryResponse : WxaExpressLocalResponse
{
    /// <summary>配送公司列表（<c>list</c>），见 <see cref="WxaExpressLocalDeliveryItem"/>。</summary>
    [JsonPropertyName("list")]
    public WxaExpressLocalDeliveryItem[]? List { get; set; }
}

/// <summary>已绑定的商家签约账号项（<c>shop_list</c> 元素，<c>shop/get</c> 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalShopAccountItem
{
    /// <summary>商家 ID（<c>shopid</c>）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>配送公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>审核状态（<c>audit_result</c>；<c>0</c> 审核通过，<c>1</c> 审核中，<c>2</c> 审核不通过）。</summary>
    [JsonPropertyName("audit_result")]
    public int AuditResult { get; set; }
}

/// <summary>即时配送拉取已绑定账号响应（<c>POST .../business/shop/get</c>；<b>无请求体</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGetBindAccountResponse : WxaExpressLocalResponse
{
    /// <summary>已绑定的商家签约账号列表（<c>shop_list</c>），见 <see cref="WxaExpressLocalShopAccountItem"/>。</summary>
    [JsonPropertyName("shop_list")]
    public WxaExpressLocalShopAccountItem[]? ShopList { get; set; }
}

/// <summary>
/// 即时配送「模拟更新配送单状态」<b>共用</b>请求体
/// （<c>POST .../business/{test_update_order,realmock_update_order}</c>）。
/// </summary>
/// <remarks>
/// <b>两端点差异（官方原文）</b>：<c>test_update_order</c> 用于<b>沙盒环境</b>（订单不真实流转到运力方，
/// <c>shopid</c> 必须是 <c>test_shop_id</c>、<b>不传</b> <c>delivery_sign</c>）；
/// <c>realmock_update_order</c> 会把请求<b>转发到运力测试环境</b>（目前支持顺丰同城与达达，<b>必传</b> <c>delivery_sign</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalMockUpdateOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；沙盒端点 <c>test_update_order</c> 必须为 <c>test_shop_id</c>）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送状态（<c>order_status</c>，必填；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>状态变更时间点（<c>action_time</c>，必填；Unix 秒级时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>附加信息（<c>action_msg</c>，可选）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>；<b>仅</b> <c>realmock_update_order</c> 必填，沙盒端点不传；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }
}

/// <summary>即时配送骑手信息（<c>agent</c> 内嵌对象；运力方更新状态时骑手接单需返回）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAgent
{
    /// <summary>骑手姓名（<c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>骑手电话（<c>phone</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>电话是否加密（<c>is_phone_encrypted</c>，可选；默认 <c>0</c> 不加密，<c>1</c> 加密）。</summary>
    [JsonPropertyName("is_phone_encrypted")]
    public int? IsPhoneEncrypted { get; set; }
}

/// <summary>即时配送运力方更新配送单状态请求体（<c>POST /cgi-bin/express/local/delivery/update_order</c>）。</summary>
/// <remarks>
/// <b><c>wx_token</c> 语义</b>：官方原文「下单事件中推送的 token 字段」——
/// 与物流助手散单寄件的 <c>token</c> 同源，<b>不得</b>用应用级令牌替代。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalUpdateOrderRequest
{
    /// <summary>下单事件推送的 Token（<c>wx_token</c>，必填）。</summary>
    [JsonPropertyName("wx_token")]
    public string? WxToken { get; set; }

    /// <summary>订单状态（<c>order_status</c>，必填；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>附加信息（<c>action_msg</c>，可选）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>状态变更时间点（<c>action_time</c>，必填；Unix 秒级时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>骑手信息（<c>agent</c>，可选；骑手接单时需返回），见 <see cref="WxaExpressLocalAgent"/>。</summary>
    [JsonPropertyName("agent")]
    public WxaExpressLocalAgent? Agent { get; set; }

    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配，可以是 dev_id 或 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，可选；在配送公司侧登记）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>配送公司小程序跳转路径（<c>wxa_path</c>，必填；用户收到消息后会间接跳转到该页面）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }

    /// <summary>预计送达时间（<c>expected_delivery_time</c>，可选；Unix 秒级时间戳，骑手接单时需返回）。</summary>
    [JsonPropertyName("expected_delivery_time")]
    public long? ExpectedDeliveryTime { get; set; }
}

/// <summary>即时配送「配送状态 / 订单状态」官方枚举值（<c>order_status</c>）。</summary>
/// <remarks>
/// 官方原文：最终状态包括成功状态 <c>302</c>；失败状态 <c>103</c> / <c>203</c> / <c>204</c> / <c>205</c> /
/// <c>401</c> / <c>501</c> / <c>502</c>。
/// </remarks>
public static class WxaExpressLocalOrderStatus
{
    /// <summary>配送公司接单阶段——等待分配骑手，即初始状态（<c>101</c>）。</summary>
    public const int WaitingForRider = 101;

    /// <summary>配送公司接单阶段——分配骑手成功（<c>102</c>）。</summary>
    public const int RiderAssigned = 102;

    /// <summary>配送公司接单阶段——商家取消订单，订单结束（<c>103</c>）。</summary>
    public const int CanceledByShopBeforePickup = 103;

    /// <summary>骑手取货阶段——骑手到店开始取货（<c>201</c>）。</summary>
    public const int PickingUp = 201;

    /// <summary>骑手取货阶段——取货成功（<c>202</c>）。</summary>
    public const int PickedUp = 202;

    /// <summary>骑手取货阶段——取货失败，商家取消订单，订单结束（<c>203</c>）。</summary>
    public const int PickupFailedCanceledByShop = 203;

    /// <summary>骑手取货阶段——取货失败，骑手因自身原因取消订单，订单结束（<c>204</c>）。</summary>
    public const int PickupFailedCanceledByRider = 204;

    /// <summary>骑手取货阶段——取货失败，骑手因商家原因取消订单，订单结束（<c>205</c>）。</summary>
    public const int PickupFailedDueToShop = 205;

    /// <summary>骑手配送阶段——配送中（<c>301</c>）。</summary>
    public const int Delivering = 301;

    /// <summary>骑手配送阶段——配送成功，订单结束（<c>302</c>）。</summary>
    public const int Delivered = 302;

    /// <summary>骑手配送阶段——商家取消订单，配送物品开始返还商家（<c>303</c>）。</summary>
    public const int ReturningCanceledByShop = 303;

    /// <summary>骑手配送阶段——无法联系收货人，配送物品开始返还商家（<c>304</c>）。</summary>
    public const int ReturningUnreachable = 304;

    /// <summary>骑手配送阶段——收货人拒收，配送物品开始返还商家（<c>305</c>）。</summary>
    public const int ReturningRejected = 305;

    /// <summary>骑手返回配送货品阶段——货品返还商户成功，订单结束（<c>401</c>）。</summary>
    public const int Returned = 401;

    /// <summary>因运力系统原因取消，订单结束（<c>501</c>）。</summary>
    public const int CanceledByDeliverySystem = 501;

    /// <summary>因不可抗拒因素（天气、道路管制等）取消，订单结束（<c>502</c>）。</summary>
    public const int CanceledByForceMajeure = 502;
}
