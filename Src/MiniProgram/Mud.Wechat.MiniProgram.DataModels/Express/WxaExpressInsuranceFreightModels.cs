// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>无忧退货「发货地址 / 收货地址」（<c>delivery_place</c> / <c>receipt_place</c> 内嵌对象，同构）。</summary>
/// <remarks>官方字段以 <c>insurance_freight/createorder</c> 的「Body.delivery_place Object Payload」为准（2026-10-12 核验）；<c>county</c> 为<b>区</b>（非 <c>area</c>，勿按其它域习惯改写）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceFreightPlace
{
    /// <summary>省（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>市（<c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区（<c>county</c>，必填）。</summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>详细地址（<c>address</c>，必填）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>无忧退货投保商品项（<c>product_info.goods_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceFreightGoods
{
    /// <summary>投保商品名称（<c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>投保商品图片 URL（<c>url</c>，必填）。</summary>
    [JsonPropertyName("url")]
    public string? PictureUrl { get; set; }
}

/// <summary>无忧退货投保订单信息（<c>product_info</c> 内嵌对象）。</summary>
/// <remarks>官方原文：用于微信下发投保和理赔通知给用户，用户点击可查看投保订单、点击订单可跳回商家小程序。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceFreightProductInfo
{
    /// <summary>投保订单在商家小程序的 path（<c>order_path</c>，必填）。</summary>
    [JsonPropertyName("order_path")]
    public string? OrderPath { get; set; }

    /// <summary>投保商品列表（<c>goods_list</c>，必填；元素为对象），见 <see cref="WxaExpressInsuranceFreightGoods"/>。</summary>
    [JsonPropertyName("goods_list")]
    public WxaExpressInsuranceFreightGoods[]? GoodsList { get; set; }
}

/// <summary>发货时投保请求体（<c>POST /wxa/business/insurance_freight/createorder</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createorder"/>（接口英文名 <c>insurance_freight_createorder</c>）。</para>
/// <para>
/// <b>官方约束</b>：<c>openid</c> 必须和理赔 <c>openid</c> 一致；一个微信支付单号<b>只能投保一次</b>；
/// <c>pay_time</c> 与微信支付时间误差须在 3 天内。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateInsuranceOrderRequest
{
    /// <summary>买家 OpenId（<c>openid</c>，必填；必须和理赔 openid 一致）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付单号（<c>order_no</c>，必填；一个微信支付单号只能投保一次）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>微信支付时间（<c>pay_time</c>，必填；秒级时间戳，与官方时间误差 3 天内）。</summary>
    [JsonPropertyName("pay_time")]
    public long PayTime { get; set; }

    /// <summary>微信支付金额（<c>pay_amount</c>，必填；单位：分）。</summary>
    [JsonPropertyName("pay_amount")]
    public long PayAmount { get; set; }

    /// <summary>发货运单号（<c>delivery_no</c>，必填）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>发货地址（<c>delivery_place</c>，必填），见 <see cref="WxaExpressInsuranceFreightPlace"/>。</summary>
    [JsonPropertyName("delivery_place")]
    public WxaExpressInsuranceFreightPlace? DeliveryPlace { get; set; }

    /// <summary>收货地址（<c>receipt_place</c>，必填；结构与 <c>delivery_place</c> 相同），见 <see cref="WxaExpressInsuranceFreightPlace"/>。</summary>
    [JsonPropertyName("receipt_place")]
    public WxaExpressInsuranceFreightPlace? ReceiptPlace { get; set; }

    /// <summary>投保订单信息（<c>product_info</c>，必填），见 <see cref="WxaExpressInsuranceFreightProductInfo"/>。</summary>
    [JsonPropertyName("product_info")]
    public WxaExpressInsuranceFreightProductInfo? ProductInfo { get; set; }
}

/// <summary>发货时投保响应（<c>POST /wxa/business/insurance_freight/createorder</c>）。</summary>
/// <remarks>官方错误码要点：<c>41009</c>（missing openid）。<c>estimate_amount</c> 为保险公司<b>预估</b>理赔金额（单位：分）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateInsuranceOrderResponse : WxaResponse
{
    /// <summary>保单号（<c>policy_no</c>）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>保险止期（<c>insurance_end_date</c>；格式 <c>yyyy-mm-dd hh24:mi:ss</c>）。</summary>
    [JsonPropertyName("insurance_end_date")]
    public string? InsuranceEndDate { get; set; }

    /// <summary>保险公司预估理赔金额（<c>estimate_amount</c>；单位：分）。</summary>
    [JsonPropertyName("estimate_amount")]
    public long EstimateAmount { get; set; }

    /// <summary>保费（<c>premium</c>；单位：分）。</summary>
    [JsonPropertyName("premium")]
    public long Premium { get; set; }
}

/// <summary>无忧退理赔请求体（<c>POST /wxa/business/insurance_freight/claim</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_claim"/>（接口英文名 <c>insurance_freight_claim</c>）。</para>
/// <para><b>官方原文</b>：本接口<b>收到用户退货后</b>再触发；<c>openid</c> / <c>order_no</c> 须与投保时<b>保持一致</b>，<c>refund_delivery_no</c> 理赔退款运单号<b>唯一</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressClaimRequest
{
    /// <summary>买家 OpenId（<c>openid</c>，必填；与投保保持一致）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付单号（<c>order_no</c>，必填；与投保保持一致）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>退款运单号（<c>refund_delivery_no</c>，必填；理赔退款运单号唯一）。</summary>
    [JsonPropertyName("refund_delivery_no")]
    public string? RefundDeliveryNo { get; set; }

    /// <summary>退款快递公司（<c>refund_company</c>，必填）。</summary>
    [JsonPropertyName("refund_company")]
    public string? RefundCompany { get; set; }
}

/// <summary>无忧退理赔响应（<c>POST /wxa/business/insurance_freight/claim</c>）。</summary>
/// <remarks><c>is_home_pick_up</c> 枚举：<c>0</c> 否，<c>1</c> 是。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressClaimResponse : WxaResponse
{
    /// <summary>理赔报案号（<c>report_no</c>；成功申请理赔时返回）。</summary>
    [JsonPropertyName("report_no")]
    public string? ReportNo { get; set; }

    /// <summary>是否上门取件（<c>is_home_pick_up</c>；<c>0</c> 否，<c>1</c> 是）。</summary>
    [JsonPropertyName("is_home_pick_up")]
    public int IsHomePickUp { get; set; }
}

/// <summary>查询无忧退货开通状态响应（<c>GET /wxa/business/insurance_freight/query_open</c>）。</summary>
/// <remarks><c>is_open</c> 枚举：<c>0</c> 否，<c>1</c> 是。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryInsuranceOpenResponse : WxaResponse
{
    /// <summary>是否开通（<c>is_open</c>；<c>0</c> 否，<c>1</c> 是）。</summary>
    [JsonPropertyName("is_open")]
    public int IsOpen { get; set; }
}

/// <summary>申请充值订单号请求体（<c>POST /wxa/business/insurance_freight/createchargeid</c>，支持自定义金额）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createchargeid"/>（接口英文名 <c>insurance_freight_createchargeid</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateChargeIdRequest
{
    /// <summary>充值金额（<c>quota</c>，必填；单位：分）。</summary>
    [JsonPropertyName("quota")]
    public long Quota { get; set; }
}

/// <summary>申请充值订单号响应（<c>POST /wxa/business/insurance_freight/createchargeid</c>）。</summary>
/// <remarks><b>精度警示（官方原文）</b>：<c>order_id</c> 为 64 位整数，JS 等语言须按 string 传递以防精度丢失；本 SDK 以 <c>long</c> 承载。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateChargeIdResponse : WxaResponse
{
    /// <summary>充值订单 ID（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }
}

/// <summary>申请支付请求体（<c>POST /wxa/business/insurance_freight/applypay</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_applypay"/>（接口英文名 <c>insurance_freight_applypay</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressApplyPayRequest
{
    /// <summary>充值订单 ID（<c>order_id</c>，必填；与 <see cref="WxaExpressCreateChargeIdResponse.OrderId"/> 保持一致）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }
}

/// <summary>申请支付响应（<c>POST /wxa/business/insurance_freight/applypay</c>）。</summary>
/// <remarks><b>官方特别提示</b>：充值后退款<b>只能退回原充值账号</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressApplyPayResponse : WxaResponse
{
    /// <summary>充值链接（<c>pay_url</c>）。</summary>
    [JsonPropertyName("pay_url")]
    public string? PayUrl { get; set; }
}

/// <summary>充值订单项（<c>list</c> 元素，<c>getpayorderlist</c> 响应）。</summary>
/// <remarks>
/// 订单状态 <c>order_status</c>：1 待支付、2 支付成功、3 使用中、4 已用完、5 退款中、6 已退款、10 支付超时；
/// 退款状态 <c>refund_status</c>：1 未退款、2 退款中、4 退款成功、5 退款失败。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPayOrderItem
{
    /// <summary>充值单号（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>订单状态（<c>order_status</c>；枚举见类说明）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>充值金额（<c>total_price</c>）。</summary>
    [JsonPropertyName("total_price")]
    public long TotalPrice { get; set; }

    /// <summary>订单创建时间（<c>create_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>支付时间（<c>pay_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("pay_time")]
    public long PayTime { get; set; }

    /// <summary>是否可以退款（<c>can_refund</c>）。</summary>
    /// <remarks>官方参数表标注为 <c>number</c>，但官方返回示例为布尔 <c>true</c>；本 SDK 按示例实际线格式以 <c>bool</c> 承载。</remarks>
    [JsonPropertyName("can_refund")]
    public bool CanRefund { get; set; }

    /// <summary>退款时间（<c>refund_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("refund_time")]
    public long RefundTime { get; set; }

    /// <summary>退款状态（<c>refund_status</c>；枚举见类说明）。</summary>
    [JsonPropertyName("refund_status")]
    public int RefundStatus { get; set; }

    /// <summary>退款金额（<c>refund_amt</c>）。</summary>
    [JsonPropertyName("refund_amt")]
    public long RefundAmount { get; set; }
}

/// <summary>拉取充值订单信息请求体（<c>POST /wxa/business/insurance_freight/getpayorderlist</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getpayorderlist"/>（接口英文名 <c>insurance_freight_getpayorderlist</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayOrderListRequest
{
    /// <summary>订单状态过滤（<c>status_list</c>，必填；枚举见 <see cref="WxaExpressPayOrderItem"/>）。</summary>
    [JsonPropertyName("status_list")]
    public int[]? StatusList { get; set; }

    /// <summary>分页偏移（<c>offset</c>，可选）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>分页大小（<c>limit</c>，可选）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

/// <summary>拉取充值订单信息响应（<c>POST /wxa/business/insurance_freight/getpayorderlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayOrderListResponse : WxaResponse
{
    /// <summary>总数（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>充值订单列表（<c>list</c>），见 <see cref="WxaExpressPayOrderItem"/>。</summary>
    [JsonPropertyName("list")]
    public WxaExpressPayOrderItem[]? List { get; set; }
}

/// <summary>拉取理赔摘要请求体（<c>POST /wxa/business/insurance_freight/getsummary</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getsummary"/>（接口英文名 <c>insurance_freight_getsummary</c>）；可查询当前保费、投保单量、理赔单量、账号余额等。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetSummaryRequest
{
    /// <summary>查询开始时间（<c>begin_time</c>，必填；秒级时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>查询结束时间（<c>end_time</c>，必填；秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}

/// <summary>拉取理赔摘要响应（<c>POST /wxa/business/insurance_freight/getsummary</c>）。</summary>
/// <remarks>官方错误码要点：<c>-202</c> 内部错误（可稍后重试）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetSummaryResponse : WxaResponse
{
    /// <summary>投保总数（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>理赔总数（<c>claim_num</c>）。</summary>
    [JsonPropertyName("claim_num")]
    public int ClaimCount { get; set; }

    /// <summary>理赔成功数（<c>claim_succ_num</c>）。</summary>
    [JsonPropertyName("claim_succ_num")]
    public int ClaimSuccessCount { get; set; }

    /// <summary>当前保费（<c>premium</c>；单位：分）。</summary>
    [JsonPropertyName("premium")]
    public long Premium { get; set; }

    /// <summary>当前账号余额（<c>funds</c>；单位：分）。</summary>
    [JsonPropertyName("funds")]
    public long Funds { get; set; }

    /// <summary>是否不能投保（<c>need_close</c>；系统安全原因不能投保）。</summary>
    [JsonPropertyName("need_close")]
    public bool NeedClose { get; set; }
}

/// <summary>保单项（<c>list</c> 元素，<c>getorderlist</c> 响应）。</summary>
/// <remarks>
/// 保单状态 <c>status</c>：2 保障中、4 理赔中、5 理赔成功、6 理赔失败、7 投保过期；
/// <c>is_home_pick_up</c>：<c>0</c> 否、<c>1</c> 是。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceOrderItem
{
    /// <summary>微信支付单号（<c>order_no</c>）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>保单号（<c>policy_no</c>）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>理赔报案号（<c>report_no</c>）。</summary>
    [JsonPropertyName("report_no")]
    public string? ReportNo { get; set; }

    /// <summary>发货运单号（<c>delivery_no</c>）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>退款运单号（<c>refund_delivery_no</c>）。</summary>
    [JsonPropertyName("refund_delivery_no")]
    public string? RefundDeliveryNo { get; set; }

    /// <summary>保费（<c>premium</c>；单位：分）。</summary>
    [JsonPropertyName("premium")]
    public long Premium { get; set; }

    /// <summary>预估理赔金额（<c>estimate_amount</c>；单位：分）。</summary>
    [JsonPropertyName("estimate_amount")]
    public long EstimateAmount { get; set; }

    /// <summary>保单状态（<c>status</c>；枚举见类说明）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>理赔打款失败原因（<c>pay_fail_reason</c>）。</summary>
    [JsonPropertyName("pay_fail_reason")]
    public string? PayFailReason { get; set; }

    /// <summary>理赔款打给用户的时间（<c>pay_finish_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long PayFinishTime { get; set; }

    /// <summary>是否上门取件（<c>is_home_pick_up</c>；<c>0</c> 否，<c>1</c> 是）。</summary>
    [JsonPropertyName("is_home_pick_up")]
    public int IsHomePickUp { get; set; }

    /// <summary>保险止期（<c>insurance_end_date</c>；格式 <c>yyyy-mm-dd hh24:mi:ss</c>）。</summary>
    /// <remarks>本字段未列入官方「返回体参数表」，但出现在官方返回示例中，故一并承载。</remarks>
    [JsonPropertyName("insurance_end_date")]
    public string? InsuranceEndDate { get; set; }
}

/// <summary>拉取保单信息请求体（<c>POST /wxa/business/insurance_freight/getorderlist</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getorderlist"/>（接口英文名 <c>insurance_freight_getorderlist</c>）。</para>
/// <para><b>全字段可选</b>（官方原文）：不传即按默认口径拉取；<c>limit</c> 默认 100、<b>最大 100</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetOrderListRequest
{
    /// <summary>买家 OpenId（<c>openid</c>，可选；与投保理赔保持一致）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付单号（<c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>保单号（<c>policy_no</c>，可选）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>理赔报案号（<c>report_no</c>，可选）。</summary>
    [JsonPropertyName("report_no")]
    public string? ReportNo { get; set; }

    /// <summary>发货运单号（<c>delivery_no</c>，可选）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>退款运单号（<c>refund_delivery_no</c>，可选）。</summary>
    [JsonPropertyName("refund_delivery_no")]
    public string? RefundDeliveryNo { get; set; }

    /// <summary>查询开始时间（<c>begin_time</c>，可选；秒级时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>查询结束时间（<c>end_time</c>，可选；秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>保单状态过滤（<c>status_list</c>，可选；枚举见 <see cref="WxaExpressInsuranceOrderItem"/>）。</summary>
    [JsonPropertyName("status_list")]
    public int[]? StatusList { get; set; }

    /// <summary>分页偏移（<c>offset</c>，可选）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>分页大小（<c>limit</c>，可选；默认 100，最大 100）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>排序方式（<c>sort_direct</c>，可选；<c>0</c> 按 create_time 正序，<c>1</c> 倒序）。</summary>
    [JsonPropertyName("sort_direct")]
    public int? SortDirection { get; set; }
}

/// <summary>拉取保单信息响应（<c>POST /wxa/business/insurance_freight/getorderlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetOrderListResponse : WxaResponse
{
    /// <summary>总数（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>保单列表（<c>list</c>），见 <see cref="WxaExpressInsuranceOrderItem"/>。</summary>
    [JsonPropertyName("list")]
    public WxaExpressInsuranceOrderItem[]? List { get; set; }
}

/// <summary>设置保费告警余额请求体（<c>POST /wxa/business/insurance_freight/update_notify_funds</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_update_notify_funds"/>
/// （接口英文名 <c>insurance_freight_update_notify_funds</c>）。
/// <b>官方原文</b>：自定义余额为 xx 时通知小程序管理员（余额无改动情况下 24h 内通知一次），通知消息经<b>微信公众平台</b>下发。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateNotifyFundsRequest
{
    /// <summary>通知的金额（<c>notify_funds</c>，必填；单位：分；设置为 <c>0</c> 表示不通知）。</summary>
    [JsonPropertyName("notify_funds")]
    public long NotifyFunds { get; set; }
}
