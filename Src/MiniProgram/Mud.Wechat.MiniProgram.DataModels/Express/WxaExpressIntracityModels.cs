// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送门店地址（<c>address_info</c> 内嵌对象；创建 / 查询 / 更新门店复用）。</summary>
/// <remarks>字段以官方「同城配送」文档 2.1 AddressInfo 为准（2026-10-11 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityAddress
{
    /// <summary>省/自治区/直辖市（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>地级市（<c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（<c>area</c>，必填）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>街道（<c>street</c>，必填）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>门牌号（<c>house</c>，必填）。</summary>
    [JsonPropertyName("house")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>，必填）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>，必填）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>联系电话（<c>phone</c>，必填）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }
}

/// <summary>同城配送货品信息（<c>cargo</c> 内嵌对象；询价 <c>preaddorder</c> 与下单 <c>addorder</c> 复用）。</summary>
/// <remarks>字段以官方「同城配送」文档 2.9/2.10 核验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityCargo
{
    /// <summary>货品名称（<c>cargo_name</c>，必填；长度 ≤ 50）。</summary>
    [JsonPropertyName("cargo_name")]
    public string? CargoName { get; set; }

    /// <summary>货品类型（<c>cargo_type</c>，必填；1 食品，2 文件，3 医药，4 电商）。</summary>
    [JsonPropertyName("cargo_type")]
    public int CargoType { get; set; }

    /// <summary>货品重量（<c>cargo_weight</c>，必填；单位：kg，精确到小数点后 2 位）。</summary>
    [JsonPropertyName("cargo_weight")]
    public double CargoWeight { get; set; }

    /// <summary>货品价值（<c>cargo_price</c>，必填；单位：元，精确到小数点后 2 位）。</summary>
    [JsonPropertyName("cargo_price")]
    public double CargoPrice { get; set; }

    /// <summary>货品数量（<c>cargo_num</c>，必填）。</summary>
    [JsonPropertyName("cargo_num")]
    public int CargoCount { get; set; }
}

/// <summary>同城配送商品项（<c>item_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityItem
{
    /// <summary>商品名称（<c>item_name</c>）。</summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; set; }

    /// <summary>商品图片 URL（<c>item_pic_url</c>）。</summary>
    [JsonPropertyName("item_pic_url")]
    public string? ItemPictureUrl { get; set; }

    /// <summary>商品数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>创建门店请求体（<c>POST /cgi-bin/express/intracity/createstore</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore"/>。</para>
/// <para>
/// 自定义门店编号 <c>out_store_id</c> 需要唯一；创建后系统生成全局唯一门店编号 <c>wx_store_id</c>，
/// 后续创建运力订单需要该门店编号。<c>order_pattern</c>：1 价格优先，2 运力优先（默认价格优先）；
/// <c>order_pattern=2</c> 时 <c>service_trans_prefer</c> 必填。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateStoreRequest
{
    /// <summary>自定义门店编号（<c>out_store_id</c>，必填；需唯一）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>门店名称（<c>store_name</c>，必填）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>运力偏好（<c>order_pattern</c>，可选；1 价格优先，2 运力优先，默认价格优先）。</summary>
    [JsonPropertyName("order_pattern")]
    public int? OrderPattern { get; set; }

    /// <summary>优先使用的运力 ID（<c>service_trans_prefer</c>，可选；<c>order_pattern=2</c> 时必填）。</summary>
    [JsonPropertyName("service_trans_prefer")]
    public string? ServiceTransIdPrefer { get; set; }

    /// <summary>门店地址信息（<c>address_info</c>，必填；务必传入正确的门店地址作为发货地址），见 <see cref="WxaExpressIntracityAddress"/>。</summary>
    [JsonPropertyName("address_info")]
    public WxaExpressIntracityAddress? AddressInfo { get; set; }
}

/// <summary>创建门店应答（<c>out_store_id</c> / <c>wx_store_id</c> / <c>appid</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateStoreResponse : WxaResponse
{
    /// <summary>自定义门店编号（<c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>；后续运力订单凭此定位）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>查询门店请求体（<c>POST /cgi-bin/express/intracity/querystore</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_querystore"/>；<c>out_store_id</c> 与 <c>wx_store_id</c> 二选一。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryStoreRequest
{
    /// <summary>自定义门店编号（<c>out_store_id</c>，可选；与 <c>wx_store_id</c> 二选一）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>，可选；与 <c>out_store_id</c> 二选一）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }
}

/// <summary>同城配送门店（<c>querystore</c> 的 <c>store_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityStore
{
    /// <summary>省/自治区/直辖市（<c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>地级市（<c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（<c>area</c>）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>街道（<c>street</c>）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>门牌号（<c>house</c>）。</summary>
    [JsonPropertyName("house")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>联系电话（<c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>自定义门店编号（<c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>门店所在城市 ID（<c>city_id</c>）。</summary>
    [JsonPropertyName("city_id")]
    public int CityId { get; set; }

    /// <summary>运力偏好（<c>order_pattern</c>）。</summary>
    [JsonPropertyName("order_pattern")]
    public int OrderPattern { get; set; }

    /// <summary>优先使用的运力 ID（<c>service_trans_prefer</c>）。</summary>
    [JsonPropertyName("service_trans_prefer")]
    public string? ServiceTransIdPrefer { get; set; }

    /// <summary>门店地址信息（<c>address_info</c>），见 <see cref="WxaExpressIntracityAddress"/>。</summary>
    [JsonPropertyName("address_info")]
    public WxaExpressIntracityAddress? AddressInfo { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>查询门店应答（<c>store_list</c> / <c>total</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_querystore"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryStoreResponse : WxaResponse
{
    /// <summary>门店列表（<c>store_list</c>），见 <see cref="WxaExpressIntracityStore"/>。</summary>
    [JsonPropertyName("store_list")]
    public List<WxaExpressIntracityStore>? StoreList { get; set; }

    /// <summary>门店数量（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; set; }
}

/// <summary>更新门店请求体（<c>POST /cgi-bin/express/intracity/updatestore</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore"/>。</para>
/// <para><c>keys</c> 为门店定位（<c>wx_store_id</c> 与 <c>out_store_id</c> 二选一），<c>content</c> 为待更新内容（均可选，只更新传入字段）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateStoreRequest
{
    /// <summary>门店编号（<c>keys</c>，必填），见 <see cref="WxaExpressUpdateStoreKey"/>。</summary>
    [JsonPropertyName("keys")]
    public WxaExpressUpdateStoreKey? Key { get; set; }

    /// <summary>更新内容（<c>content</c>，必填），见 <see cref="WxaExpressUpdateStoreContent"/>。</summary>
    [JsonPropertyName("content")]
    public WxaExpressUpdateStoreContent? Content { get; set; }
}

/// <summary>更新门店定位（<c>keys</c>；<c>wx_store_id</c> 与 <c>out_store_id</c> 二选一）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateStoreKey
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，可选）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>自定义门店编号（<c>out_store_id</c>，可选）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }
}

/// <summary>更新门店内容（<c>content</c>；仅更新传入字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateStoreContent
{
    /// <summary>门店名称（<c>store_name</c>，可选）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>运力偏好（<c>order_pattern</c>，可选）。</summary>
    [JsonPropertyName("order_pattern")]
    public int? OrderPattern { get; set; }

    /// <summary>优先使用的运力 ID（<c>service_trans_prefer</c>，可选；<c>order_pattern=2</c> 时必填）。</summary>
    [JsonPropertyName("service_trans_prefer")]
    public string? ServiceTransIdPrefer { get; set; }

    /// <summary>门店地址信息（<c>address_info</c>，可选），见 <see cref="WxaExpressIntracityAddress"/>。</summary>
    [JsonPropertyName("address_info")]
    public WxaExpressIntracityAddress? AddressInfo { get; set; }
}

/// <summary>更新门店应答（<c>out_store_id</c> / <c>wx_store_id</c> / <c>appid</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateStoreResponse : WxaResponse
{
    /// <summary>自定义门店编号（<c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>门店运费充值请求体（<c>POST /cgi-bin/express/intracity/storecharge</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge"/>；充值后返回支付链接 <c>payurl</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressStoreChargeRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>，必填）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>充值金额（<c>amount</c>，必填；单位：分）。</summary>
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}

/// <summary>门店运费充值应答（<c>payurl</c> / <c>wx_store_id</c> / <c>appid</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge"/>；<c>payurl</c> 为拉起支付的链接。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressStoreChargeResponse : WxaResponse
{
    /// <summary>支付链接（<c>payurl</c>；拉起微信支付后完成充值）。</summary>
    [JsonPropertyName("payurl")]
    public string? PayUrl { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>门店运费退款请求体（<c>POST /cgi-bin/express/intracity/storerefund</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storerefund"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressStoreRefundRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>，必填）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }
}

/// <summary>门店运费退款应答（<c>refund_amount</c> / <c>wx_store_id</c> / <c>appid</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storerefund"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressStoreRefundResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>退款金额（<c>refund_amount</c>；单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public int RefundAmount { get; set; }
}

/// <summary>查询门店运费流水请求体（<c>POST /cgi-bin/express/intracity/queryflow</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow"/>；<c>flow_type</c>：1 充值，2 扣款，3 退款，4 扣款退回。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryFlowRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>流水类型（<c>flow_type</c>，必填；1 充值，2 扣款，3 退款，4 扣款退回）。</summary>
    [JsonPropertyName("flow_type")]
    public int FlowType { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>，可选）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>开始时间（<c>begin_time</c>，可选；Unix 时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>结束时间（<c>end_time</c>，可选；Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

/// <summary>同城配送运费流水（<c>queryflow</c> 的 <c>flow_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityFlow
{
    /// <summary>流水类型（<c>flow_type</c>）。</summary>
    [JsonPropertyName("flow_type")]
    public int FlowType { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>支付单号（<c>pay_order_id</c>）。</summary>
    [JsonPropertyName("pay_order_id")]
    public string? PayOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>账单编号（<c>bill_id</c>）。</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>支付金额（<c>pay_amount</c>；单位：分）。</summary>
    [JsonPropertyName("pay_amount")]
    public int PayAmount { get; set; }

    /// <summary>退款金额（<c>refund_amount</c>；单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public int? RefundAmount { get; set; }

    /// <summary>扣款金额（<c>deduct_amount</c>；单位：分）。</summary>
    [JsonPropertyName("deduct_amount")]
    public int? DeductAmount { get; set; }

    /// <summary>支付状态（<c>pay_status</c>；0 待支付，1 已支付，2 支付失败）。</summary>
    [JsonPropertyName("pay_status")]
    public string? PayStatus { get; set; }

    /// <summary>退款状态（<c>refund_status</c>；0 待退款，1 已退款，2 退款失败）。</summary>
    [JsonPropertyName("refund_status")]
    public string? RefundStatus { get; set; }

    /// <summary>支付时间（<c>pay_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("pay_time")]
    public long PayTime { get; set; }

    /// <summary>退款时间（<c>refund_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("refund_time")]
    public long RefundTime { get; set; }

    /// <summary>创建时间（<c>create_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>消费截止时间（<c>consume_deadline</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("consume_deadline")]
    public long? ConsumeDeadline { get; set; }

    /// <summary>配送状态（<c>delivery_status</c>；1 待取货，2 已取货，3 已送达，4 已取消）。</summary>
    [JsonPropertyName("delivery_status")]
    public int? DeliveryStatus { get; set; }

    /// <summary>配送完成时间（<c>delivery_finished_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("delivery_finished_time")]
    public long? DeliveryFinishedTime { get; set; }

    /// <summary>累计支付金额（<c>total_pay_amt</c>；单位：分）。</summary>
    [JsonPropertyName("total_pay_amt")]
    public int? TotalPayAmount { get; set; }

    /// <summary>累计退款金额（<c>total_refund_amt</c>；单位：分）。</summary>
    [JsonPropertyName("total_refund_amt")]
    public int? TotalRefundAmount { get; set; }

    /// <summary>累计扣款金额（<c>total_deduct_amt</c>；单位：分）。</summary>
    [JsonPropertyName("total_deduct_amt")]
    public int? TotalDeductAmount { get; set; }
}

/// <summary>查询门店运费流水应答（<c>flow_list</c> / <c>total</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryFlowResponse : WxaResponse
{
    /// <summary>流水列表（<c>flow_list</c>），见 <see cref="WxaExpressIntracityFlow"/>。</summary>
    [JsonPropertyName("flow_list")]
    public List<WxaExpressIntracityFlow>? FlowList { get; set; }

    /// <summary>流水数量（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; set; }
}

/// <summary>查询门店余额请求体（<c>POST /cgi-bin/express/intracity/balancequery</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_balancequery"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBalanceQueryRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>，必填）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }
}

/// <summary>同城配送余额明细（<c>balance_detail</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityBalanceDetail
{
    /// <summary>余额（<c>balance</c>；单位：分）。</summary>
    [JsonPropertyName("balance")]
    public int Balance { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>运力名称（<c>service_trans_name</c>）。</summary>
    [JsonPropertyName("service_trans_name")]
    public string? ServiceTransName { get; set; }

    /// <summary>充值订单列表（<c>order_list</c>），见 <see cref="WxaExpressIntracityChargeOrder"/>。</summary>
    [JsonPropertyName("order_list")]
    public List<WxaExpressIntracityChargeOrder>? OrderList { get; set; }
}

/// <summary>同城配送充值订单（<c>balance_detail.order_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityChargeOrder
{
    /// <summary>充值订单号（<c>payorder_id</c>）。</summary>
    [JsonPropertyName("payorder_id")]
    public string? PayOrderId { get; set; }

    /// <summary>充值金额（<c>charge_amt</c>；单位：分）。</summary>
    [JsonPropertyName("charge_amt")]
    public int ChargeAmount { get; set; }

    /// <summary>未使用的余额（<c>unused_amt</c>；单位：分）。</summary>
    [JsonPropertyName("unused_amt")]
    public int UnusedAmount { get; set; }

    /// <summary>充值生效时间（<c>begin_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>充值失效时间（<c>end_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}

/// <summary>查询门店余额应答（<c>all_balance</c> / <c>balance_detail</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_balancequery"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBalanceQueryResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>总余额（<c>all_balance</c>；单位：分）。</summary>
    [JsonPropertyName("all_balance")]
    public int TotalBalance { get; set; }

    /// <summary>余额详情列表（<c>balance_detail</c>），见 <see cref="WxaExpressIntracityBalanceDetail"/>。</summary>
    [JsonPropertyName("balance_detail")]
    public List<WxaExpressIntracityBalanceDetail>? BalanceDetailList { get; set; }
}

/// <summary>同城配送询价 / 下单共用请求体（<c>preaddorder</c> 询价 + <c>addorder</c> 下单）。</summary>
/// <remarks>
/// <para>询价 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder"/>：调用后返回预估费用与服务信息，商家可据此展示给用户。</para>
/// <para>下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>：询价后确认下单，凭 <c>store_order_id</c> 幂等（重复下单返回同一配送单）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityOrderBaseRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>用户姓名（<c>user_name</c>，必填）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>用户手机号（<c>user_phone</c>，必填）。</summary>
    [JsonPropertyName("user_phone")]
    public string? UserPhoneNumber { get; set; }

    /// <summary>用户位置经度（<c>user_lng</c>，必填）。</summary>
    [JsonPropertyName("user_lng")]
    public double UserLongitude { get; set; }

    /// <summary>用户位置纬度（<c>user_lat</c>，必填）。</summary>
    [JsonPropertyName("user_lat")]
    public double UserLatitude { get; set; }

    /// <summary>用户详细地址（<c>user_address</c>，必填）。</summary>
    [JsonPropertyName("user_address")]
    public string? UserAddress { get; set; }

    /// <summary>货品信息（<c>cargo</c>，必填），见 <see cref="WxaExpressIntracityCargo"/>。</summary>
    [JsonPropertyName("cargo")]
    public WxaExpressIntracityCargo? Cargo { get; set; }

    /// <summary>是否使用沙箱（<c>use_sandbox</c>，可选；true 走测试环境不真实计费）。</summary>
    [JsonPropertyName("use_sandbox")]
    public bool? IsSandbox { get; set; }
}

/// <summary>同城配送询价应答（<c>service_trans_id</c> / <c>distance</c> / <c>est_fee</c> 等）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPreAddOrderResponse : WxaResponse
{
    /// <summary>运力 ID（<c>service_trans_id</c>；下单时须传回）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>配送距离（<c>distance</c>；单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>预估费用（<c>est_fee</c>；单位：分）。</summary>
    [JsonPropertyName("est_fee")]
    public int EstimatedFee { get; set; }

    /// <summary>预计送达时间（<c>expected_finished_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("expected_finished_time")]
    public long? ExpectedFinishTime { get; set; }

    /// <summary>承诺送达时间（<c>promise_delivery_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("promise_delivery_time")]
    public long? PromiseDeliveryTime { get; set; }
}

/// <summary>同城配送下单请求体（<c>POST /cgi-bin/express/intracity/addorder</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddIntracityOrderRequest : WxaExpressIntracityOrderBaseRequest
{
    /// <summary>商家配送单号（<c>store_order_id</c>，必填；用于幂等，重复下单返回同一配送单）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>用户 OpenId（<c>user_openid</c>，必填）。</summary>
    [JsonPropertyName("user_openid")]
    public string? UserOpenId { get; set; }

    /// <summary>序号（<c>order_seq</c>，可选；搭配 <c>#冲单#</c> 用于催单消息）。</summary>
    [JsonPropertyName("order_seq")]
    public string? OrderSequence { get; set; }

    /// <summary>收货码类型（<c>verify_code_type</c>，可选；0 收货原则，1 收货码）。</summary>
    [JsonPropertyName("verify_code_type")]
    public int? VerifyCodeType { get; set; }

    /// <summary>订单详情页路径（<c>order_detail_path</c>，可选）。</summary>
    [JsonPropertyName("order_detail_path")]
    public string? OrderDetailPath { get; set; }

    /// <summary>回调 URL（<c>callback_url</c>，可选；配送状态变化时异步通知）。</summary>
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    /// <summary>商品列表（<c>item_list</c>，可选），见 <see cref="WxaExpressIntracityItem"/>。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaExpressIntracityItem>? ItemList { get; set; }
}

/// <summary>同城配送下单应答（<c>wx_order_id</c> / <c>fee</c> / <c>distance</c> 等）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddIntracityOrderResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>运单号（<c>trans_order_id</c>）。</summary>
    [JsonPropertyName("trans_order_id")]
    public string? TransOrderId { get; set; }

    /// <summary>运力侧运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>配送距离（<c>distance</c>；单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>运费（<c>fee</c>；单位：分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>取货码（<c>fetch_code</c>）。</summary>
    [JsonPropertyName("fetch_code")]
    public string? FetchCode { get; set; }

    /// <summary>序号（<c>order_seq</c>）。</summary>
    [JsonPropertyName("order_seq")]
    public string? OrderSequence { get; set; }
}

/// <summary>同城配送查询配送单请求体（<c>POST /cgi-bin/express/intracity/queryorder</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder"/>；<c>wx_store_id</c> 必填，<c>store_order_id</c> 与 <c>wx_order_id</c> 二选一。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryIntracityOrderRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>，可选；与 <c>wx_order_id</c> 二选一）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>，可选；与 <c>store_order_id</c> 二选一）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }
}

/// <summary>同城配送收货人信息（<c>receiver_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityReceiver
{
    /// <summary>收货人姓名（<c>receiver_name</c>）。</summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }

    /// <summary>收货地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>收货人手机号（<c>phone_num</c>）。</summary>
    [JsonPropertyName("phone_num")]
    public string? PhoneNumber { get; set; }
}

/// <summary>同城配送门店信息（<c>store_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityStoreInfo
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>门店名称（<c>store_name</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>门店地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>经度（<c>lng</c>）。</summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>纬度（<c>lat</c>）。</summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>门店电话（<c>phone_num</c>）。</summary>
    [JsonPropertyName("phone_num")]
    public string? PhoneNumber { get; set; }
}

/// <summary>同城配送骑手信息（<c>transporter_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityTransporter
{
    /// <summary>骑手姓名（<c>transporter_name</c>）。</summary>
    [JsonPropertyName("transporter_name")]
    public string? TransporterName { get; set; }

    /// <summary>骑手手机号（<c>transporter_phone</c>）。</summary>
    [JsonPropertyName("transporter_phone")]
    public string? TransporterPhoneNumber { get; set; }
}

/// <summary>同城配送查询配送单应答（配送单全量明细）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryIntracityOrderResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>配送单状态（<c>order_status</c>；1 待接单，2 待取货，3 配送中，4 已完成，5 已取消）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>用户 OpenId（<c>user_openid</c>）。</summary>
    [JsonPropertyName("user_openid")]
    public string? UserOpenId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>运单号（<c>delivery_no</c>）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>配送距离（<c>distance</c>；单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>实际费用（<c>actualfee</c>；单位：分）。</summary>
    [JsonPropertyName("actualfee")]
    public int ActualFee { get; set; }

    /// <summary>扣款金额（<c>deductfee</c>；单位：分）。</summary>
    [JsonPropertyName("deductfee")]
    public int DeductFee { get; set; }

    /// <summary>下单时间（<c>create_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>接单时间（<c>accept_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("accept_time")]
    public long? AcceptTime { get; set; }

    /// <summary>取货时间（<c>fetch_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("fetch_time")]
    public long? FetchTime { get; set; }

    /// <summary>送达时间（<c>finish_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("finish_time")]
    public long? FinishTime { get; set; }

    /// <summary>取消时间（<c>cancel_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>预计送达时间（<c>expected_finish_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("expected_finish_time")]
    public long? ExpectedFinishTime { get; set; }

    /// <summary>取货码（<c>fetch_code</c>）。</summary>
    [JsonPropertyName("fetch_code")]
    public string? FetchCode { get; set; }

    /// <summary>收货码（<c>recv_code</c>）。</summary>
    [JsonPropertyName("recv_code")]
    public string? ReceiveCode { get; set; }

    /// <summary>序号（<c>order_seq</c>）。</summary>
    [JsonPropertyName("order_seq")]
    public string? OrderSequence { get; set; }

    /// <summary>商品列表（<c>item_list</c>），见 <see cref="WxaExpressIntracityItem"/>。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaExpressIntracityItem>? ItemList { get; set; }

    /// <summary>货品信息（<c>cargo_info</c>），见 <see cref="WxaExpressIntracityCargo"/>。</summary>
    [JsonPropertyName("cargo_info")]
    public WxaExpressIntracityCargo? CargoInfo { get; set; }

    /// <summary>收货人信息（<c>receiver_info</c>），见 <see cref="WxaExpressIntracityReceiver"/>。</summary>
    [JsonPropertyName("receiver_info")]
    public WxaExpressIntracityReceiver? ReceiverInfo { get; set; }

    /// <summary>门店信息（<c>store_info</c>），见 <see cref="WxaExpressIntracityStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public WxaExpressIntracityStoreInfo? StoreInfo { get; set; }

    /// <summary>骑手信息（<c>transporter_info</c>），见 <see cref="WxaExpressIntracityTransporter"/>。</summary>
    [JsonPropertyName("transporter_info")]
    public WxaExpressIntracityTransporter? TransporterInfo { get; set; }
}

/// <summary>同城配送取消配送单请求体（<c>POST /cgi-bin/express/intracity/cancelorder</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder"/>。</para>
/// <para><c>cancel_reason_id</c> 取消原因枚举：1001 用户取消，1002 门店取消，1003 骑手取消，1004 其它；骑手已接单后取消可能产生扣款（<c>deductfee</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCancelIntracityOrderRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，可选）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>，可选；与 <c>wx_order_id</c> 二选一）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>，可选；与 <c>store_order_id</c> 二选一）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>取消原因 ID（<c>cancel_reason_id</c>，必填；1001 用户取消，1002 门店取消，1003 骑手取消，1004 其它）。</summary>
    [JsonPropertyName("cancel_reason_id")]
    public int CancelReasonId { get; set; }

    /// <summary>取消原因描述（<c>cancel_reason</c>，可选）。</summary>
    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; set; }
}

/// <summary>同城配送取消配送单应答（<c>order_status</c> / <c>deductfee</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCancelIntracityOrderResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>取消后配送单状态（<c>order_status</c>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>扣款金额（<c>deductfee</c>；单位：分；骑手已接单时取消可能扣款）。</summary>
    [JsonPropertyName("deductfee")]
    public int? DeductFee { get; set; }
}

/// <summary>模拟配送状态变更回调请求体（<c>POST /cgi-bin/express/intracity/mocknotify</c>，仅用于<b>测试环境</b>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_mocknotify"/>。</para>
/// <para>仅测试环境可用：模拟配送单状态变化，触发配置的 <c>callback_url</c> 回调，用于商家侧联调。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressMockNotifyRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，可选）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>，可选；与 <c>wx_order_id</c> 二选一）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>，可选；与 <c>store_order_id</c> 二选一）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>模拟变更后的配送单状态（<c>order_status</c>，必填；1 待接单，2 待取货，3 配送中，4 已完成，5 已取消）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }
}

/// <summary>查询支持同城配送的城市请求体（<c>POST /cgi-bin/express/intracity/getcity</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getcity"/>
/// （接口英文名 <c>intracity_getcity</c>，2026-10-12 核验）。
/// </para>
/// <para>
/// <c>service_trans_id</c> <b>不传</b>即返回<b>全部运力</b>的支持城市；传值则只返回该运力的结果
/// （官方示例取值：<c>SFTC</c> 顺丰同城、<c>DADA</c> 达达）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetCityRequest
{
    /// <summary>运力 ID（<c>service_trans_id</c>，可选；不传则返回全部运力结果）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }
}

/// <summary>单个运力支持的城市信息（<c>support_list</c> 元素，<c>getcity</c> 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCitySupport
{
    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>该运力支持的城市列表（<c>city_list</c>），见 <see cref="WxaExpressCity"/>。</summary>
    [JsonPropertyName("city_list")]
    public WxaExpressCity[]? CityList { get; set; }
}

/// <summary>同城配送城市项（<c>city_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCity
{
    /// <summary>城市行政区域编码（<c>city_code</c>；官方示例：北京市 <c>110000</c>、天津市 <c>120000</c>）。</summary>
    [JsonPropertyName("city_code")]
    public int CityCode { get; set; }

    /// <summary>城市名称（<c>city_name</c>；官方示例：「北京市」）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }
}

/// <summary>查询支持同城配送的城市响应（<c>POST /cgi-bin/express/intracity/getcity</c>）。</summary>
/// <remarks>
/// 官方错误码要点：<c>48001</c>（小程序未获同城配送接口权限，需在管理后台开通）、
/// <c>934003</c>（运力 ID 错误）、<c>934009</c>（不支持该门店所在城市）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetCityResponse : WxaResponse
{
    /// <summary>各运力支持的城市信息（<c>support_list</c>），见 <see cref="WxaExpressCitySupport"/>。</summary>
    [JsonPropertyName("support_list")]
    public WxaExpressCitySupport[]? SupportList { get; set; }
}

/// <summary>查询扣费主体请求体（<c>POST /cgi-bin/express/intracity/getpaymode</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getpaymode"/>
/// （接口英文名 <c>intracity_getpaymode</c>）。
/// </para>
/// <para><b>官方原文约束</b>：<c>appid</c> <b>必须</b>与当前 <c>access_token</c> 匹配，否则返回 <c>934012</c>（appid 和 access_token 不匹配）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayModeRequest
{
    /// <summary>小程序 AppId（<c>appid</c>，必填；必须与 access_token 匹配）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>查询扣费主体响应（<c>POST /cgi-bin/express/intracity/getpaymode</c>）。</summary>
/// <remarks>
/// <c>pay_mode</c> 取值见 <see cref="WxaExpressPayModes"/>。
/// <c>pay_appid</c> 与 <c>pay_component_appid</c> <b>互斥</b>：扣费主体为小程序时返回前者、为服务商时返回后者。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayModeResponse : WxaResponse
{
    /// <summary>扣费主体（<c>pay_mode</c>；<c>PAY_MODE_STORE</c> 门店 / <c>PAY_MODE_APP</c> 小程序 / <c>PAY_MODE_COMPONENT</c> 服务商）。</summary>
    [JsonPropertyName("pay_mode")]
    public string? PayMode { get; set; }

    /// <summary>扣费小程序 AppId（<c>pay_appid</c>；扣费主体为小程序时返回）。</summary>
    [JsonPropertyName("pay_appid")]
    public string? PayAppId { get; set; }

    /// <summary>扣费服务商 AppId（<c>pay_component_appid</c>；扣费主体为服务商时返回）。</summary>
    [JsonPropertyName("pay_component_appid")]
    public string? PayComponentAppId { get; set; }
}

/// <summary>设置扣费主体请求体（<c>POST /cgi-bin/express/intracity/setpaymode</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_setpaymode"/>
/// （接口英文名 <c>intracity_setpaymode</c>）。成功仅返回 <c>errcode</c>/<c>errmsg</c>。
/// </para>
/// <para><b>官方原文约束</b>：<c>appid</c> <b>必须</b>与当前 <c>access_token</c> 匹配（同 <see cref="WxaExpressGetPayModeRequest"/>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSetPayModeRequest
{
    /// <summary>小程序 AppId（<c>appid</c>，必填；必须与 access_token 匹配）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>扣费主体（<c>pay_mode</c>，必填），取值见 <see cref="WxaExpressPayModes"/>。</summary>
    [JsonPropertyName("pay_mode")]
    public string? PayMode { get; set; }
}

/// <summary>同城配送「扣费主体」取值（<c>pay_mode</c>，官方固定值）。</summary>
/// <remarks>
/// 官方原文：<c>PAY_MODE_STORE</c> 门店、<c>PAY_MODE_APP</c> 小程序、<c>PAY_MODE_COMPONENT</c> 服务商。
/// 该组常量由 <c>intracity_setpaymode</c> 入参与 <c>intracity_getpaymode</c> 出参共用。
/// </remarks>
public static class WxaExpressPayModes
{
    /// <summary>门店扣费（<c>PAY_MODE_STORE</c>）。</summary>
    public const string Store = "PAY_MODE_STORE";

    /// <summary>小程序扣费（<c>PAY_MODE_APP</c>）。</summary>
    public const string App = "PAY_MODE_APP";

    /// <summary>服务商扣费（<c>PAY_MODE_COMPONENT</c>）。</summary>
    public const string Component = "PAY_MODE_COMPONENT";
}