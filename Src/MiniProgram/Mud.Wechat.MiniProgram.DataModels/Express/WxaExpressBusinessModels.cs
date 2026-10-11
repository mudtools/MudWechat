// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 物流助手「商家寄件」下单请求体（<c>POST /cgi-bin/express/business/order/add</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder"/>。</para>
/// <para>
/// <b>频率与前置</b>：调用前须已绑定快递账号且开通下单权限；官方建议单商户日下单量超 10 万时联系快递公司申请提额。
/// <c>add_source=2</c>（App/AppH5 订单）时不发送物流服务通知；<c>expect_time</c> 顺丰必传，
/// 0 表示已事先约定取件时间，否则传预期揽件时间戳（秒），需大于当前时间；选择预期揽件时间后<b>不要自己打单</b>。
/// 平台型小程序 <c>tagid</c> 须与入驻方账号一一对应。
/// </para>
/// <para>面单预览 <c>template/preview</c> 的 <c>custom</c> 字段与本请求体同构（官方原文：“格式是商户侧下单 addOrder 接口中的请求体”）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddOrderRequest
{
    /// <summary>订单 ID（<c>order_id</c>，必填；须保证全局唯一，不超过 512 字节）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填；<c>add_source=2</c> 时无需填写）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填；参见 <c>delivery/getall</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递客户编码或现付编码（<c>biz_id</c>，必填；参见 <c>account/getall</c>）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>快递备注信息（<c>custom_remark</c>，可选；例如“易碎物品”，不超过 1024 字节）。</summary>
    [JsonPropertyName("custom_remark")]
    public string? CustomRemark { get; set; }

    /// <summary>订单标签 ID（<c>tagid</c>，可选；平台型小程序区分入驻方，须与入驻方账号一一对应）。</summary>
    [JsonPropertyName("tagid")]
    public int? TagId { get; set; }

    /// <summary>订单来源（<c>add_source</c>，必填；0 为小程序订单，2 为 App 或 H5 订单，填 2 则不发送物流服务通知）。</summary>
    [JsonPropertyName("add_source")]
    public int AddSource { get; set; }

    /// <summary>App 或 H5 的 AppId（<c>wx_appid</c>，可选；<c>add_source=2</c> 时必填，须与开通物流助手的小程序绑定同一开放账号）。</summary>
    [JsonPropertyName("wx_appid")]
    public string? WxAppId { get; set; }

    /// <summary>发件人信息（<c>sender</c>，必填），见 <see cref="WxaExpressContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressContact? Sender { get; set; }

    /// <summary>收件人信息（<c>receiver</c>，必填），见 <see cref="WxaExpressContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressContact? Receiver { get; set; }

    /// <summary>包裹信息（<c>cargo</c>，必填；将传递给快递公司），见 <see cref="WxaExpressCargo"/>。</summary>
    [JsonPropertyName("cargo")]
    public WxaExpressCargo? Cargo { get; set; }

    /// <summary>商品信息（<c>shop</c>，必填；展示到物流服务通知和电子面单中），见 <see cref="WxaExpressShop"/>。</summary>
    [JsonPropertyName("shop")]
    public WxaExpressShop? Shop { get; set; }

    /// <summary>保价信息（<c>insured</c>，必填），见 <see cref="WxaExpressInsured"/>。</summary>
    [JsonPropertyName("insured")]
    public WxaExpressInsured? Insured { get; set; }

    /// <summary>服务类型（<c>service</c>，必填；顺丰必填，其它快递公司选填），见 <see cref="WxaExpressService"/>。</summary>
    [JsonPropertyName("service")]
    public WxaExpressService? Service { get; set; }

    /// <summary>预期的上门揽件时间（<c>expect_time</c>，可选；Unix 时间戳秒；0 表示已事先约定取件时间；顺丰必传）。</summary>
    [JsonPropertyName("expect_time")]
    public long? ExpectTime { get; set; }

    /// <summary>分单策略（<c>take_mode</c>，可选；0 线下网点签约，1 总部签约结算，不传默认线下网点签约；目前支持圆通）。</summary>
    [JsonPropertyName("take_mode")]
    public int? TakeMode { get; set; }
}

/// <summary>物流助手「商家寄件」下单应答（<c>order_id</c> / <c>waybill_id</c> / <c>waybill_data</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddOrderResponse : WxaResponse
{
    /// <summary>快递侧错误码（<c>delivery_resultcode</c>，快递公司返回的运单创建结果；非 0 需按 <see cref="DeliveryResultMessage"/> 处理）。</summary>
    [JsonPropertyName("delivery_resultcode")]
    public int? DeliveryResultCode { get; set; }

    /// <summary>快递侧错误信息（<c>delivery_resultmsg</c>）。</summary>
    [JsonPropertyName("delivery_resultmsg")]
    public string? DeliveryResultMessage { get; set; }

    /// <summary>商户订单号（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单数据列表（<c>waybill_data</c>），见 <see cref="WxaExpressWaybillDataItem"/>。</summary>
    [JsonPropertyName("waybill_data")]
    public List<WxaExpressWaybillDataItem>? WaybillData { get; set; }
}

/// <summary>查询运单信息请求体（<c>POST /cgi-bin/express/business/order/get</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder"/>。</para>
/// <para>下单成功后会收到发货通知事件（推送 <c>waybill_id</c>），凭 <c>order_id</c> 查询运单与面单数据。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetOrderRequest
{
    /// <summary>商户订单号（<c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填；用于物流服务通知）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，可选）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单类型（<c>print_type</c>，可选；1 模板 A4，2 模板 100x150）。</summary>
    [JsonPropertyName("print_type")]
    public int? PrintType { get; set; }
}

/// <summary>查询运单信息应答（<c>print_html</c> / <c>waybill_data</c> / <c>order_status</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetOrderResponse : WxaResponse
{
    /// <summary>商户订单号（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单数据列表（<c>waybill_data</c>），见 <see cref="WxaExpressWaybillDataItem"/>。</summary>
    [JsonPropertyName("waybill_data")]
    public List<WxaExpressWaybillDataItem>? WaybillData { get; set; }

    /// <summary>面单 HTML（<c>print_html</c>，打印面单所需；拿到后直接渲染/打印）。</summary>
    [JsonPropertyName("print_html")]
    public string? PrintHtml { get; set; }

    /// <summary>运单状态（<c>order_status</c>，枚举值同物流状态查询）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }
}

/// <summary>批量查询运单信息请求体（<c>POST /cgi-bin/express/business/order/batchget</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder"/>；单次最多 50 单。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBatchGetOrderRequest
{
    /// <summary>查询单列表（<c>order_list</c>，必填；单次最多 50 单）。</summary>
    [JsonPropertyName("order_list")]
    public List<WxaExpressBatchGetOrderItem>? OrderList { get; set; }
}

/// <summary>批量查询运单信息请求项（<c>order_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBatchGetOrderItem
{
    /// <summary>商户订单号（<c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，可选）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }
}

/// <summary>
/// 批量查询运单信息应答（<c>order_list</c>：每单含 <c>errcode</c>/<c>errmsg</c> + 运单明细，
/// 与商家寄件 <c>order/get</c> 应答同形，另带 errcode 逐单错误信封）。
/// </summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBatchGetOrderResponse : WxaResponse
{
    /// <summary>运单信息列表（<c>order_list</c>），见 <see cref="WxaExpressBatchGetOrderInfo"/>。</summary>
    [JsonPropertyName("order_list")]
    public List<WxaExpressBatchGetOrderInfo>? OrderList { get; set; }
}

/// <summary>批量查询的单条运单信息（含 errcode / errmsg 逐单信封）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBatchGetOrderInfo
{
    /// <summary>错误码（<c>errcode</c>，0 表示成功）。</summary>
    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    /// <summary>错误信息（<c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>商户订单号（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单数据列表（<c>waybill_data</c>），见 <see cref="WxaExpressWaybillDataItem"/>。</summary>
    [JsonPropertyName("waybill_data")]
    public List<WxaExpressWaybillDataItem>? WaybillData { get; set; }

    /// <summary>面单 HTML（<c>print_html</c>）。</summary>
    [JsonPropertyName("print_html")]
    public string? PrintHtml { get; set; }

    /// <summary>运单状态（<c>order_status</c>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }
}

/// <summary>取消运单请求体（<c>POST /cgi-bin/express/business/order/cancel</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_cancelorder"/>。</para>
/// <para>只能取消<b>待揽件</b>运单（官方原文：揽件成功或运输中的运单无法取消）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCancelOrderRequest
{
    /// <summary>商户订单号（<c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，可选）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }
}

/// <summary>取消运单应答（<c>delivery_resultcode</c> / <c>delivery_resultmsg</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_cancelorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCancelOrderResponse : WxaResponse
{
    /// <summary>快递侧错误码（<c>delivery_resultcode</c>）。</summary>
    [JsonPropertyName("delivery_resultcode")]
    public int? DeliveryResultCode { get; set; }

    /// <summary>快递侧错误信息（<c>delivery_resultmsg</c>）。</summary>
    [JsonPropertyName("delivery_resultmsg")]
    public string? DeliveryResultMessage { get; set; }
}

/// <summary>查询物流轨迹请求体（<c>POST /cgi-bin/express/business/path/get</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPathRequest
{
    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }
}

/// <summary>查询物流轨迹应答（<c>path_item_list</c> 轨迹列表）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath"/>。<c>action_type</c> 枚举值同附录轨迹类型（揽件/运输/派送/异常等）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPathResponse : WxaResponse
{
    /// <summary>用户 OpenId（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹信息数量（<c>path_item_num</c>）。</summary>
    [JsonPropertyName("path_item_num")]
    public int PathItemCount { get; set; }

    /// <summary>轨迹信息列表（<c>path_item_list</c>），见 <see cref="WxaExpressPathItem"/>。</summary>
    [JsonPropertyName("path_item_list")]
    public List<WxaExpressPathItem>? PathItemList { get; set; }
}

/// <summary>物流轨迹节点（<c>path_item_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathItem
{
    /// <summary>轨迹发生时间（<c>action_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，枚举值同附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，展示在快递轨迹详情页）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }
}

/// <summary>获取快递公司列表应答（<c>GET /cgi-bin/express/business/delivery/getall</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getdelivery"/>。</para>
/// <para><b>GET</b> 请求（无请求体）；<c>can_use_cash</c> 表示是否支持现金支付、<c>can_get_quota</c> 表示是否支持电子面单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetAllDeliveryResponse : WxaResponse
{
    /// <summary>快递公司数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>快递公司列表（<c>data</c>），见 <see cref="WxaExpressDeliveryCompany"/>。</summary>
    [JsonPropertyName("data")]
    public List<WxaExpressDeliveryCompany>? DeliveryList { get; set; }
}

/// <summary>快递公司信息（<c>delivery/getall</c> 的 <c>data</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressDeliveryCompany
{
    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司名称（<c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }

    /// <summary>是否支持现金支付（<c>can_use_cash</c>）。</summary>
    [JsonPropertyName("can_use_cash")]
    public bool? CanUseCash { get; set; }

    /// <summary>是否支持电子面单（<c>can_get_quota</c>）。</summary>
    [JsonPropertyName("can_get_quota")]
    public bool? CanGetQuota { get; set; }

    /// <summary>现付编码（<c>cash_biz_id</c>）。</summary>
    [JsonPropertyName("cash_biz_id")]
    public string? CashBusinessId { get; set; }

    /// <summary>服务类型列表（<c>service_type</c>），见 <see cref="WxaExpressService"/>。</summary>
    [JsonPropertyName("service_type")]
    public List<WxaExpressService>? ServiceList { get; set; }
}

/// <summary>获取快递账号列表应答（<c>GET /cgi-bin/express/business/account/getall</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getaccount"/>。</para>
/// <para><b>GET</b> 请求（无请求体）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetAllAccountResponse : WxaResponse
{
    /// <summary>快递账号数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>快递账号列表（<c>list</c>），见 <see cref="WxaExpressAccount"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaExpressAccount>? AccountList { get; set; }
}

/// <summary>快递账号信息（<c>account/getall</c> 的 <c>list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAccount
{
    /// <summary>快递公司客户编码（<c>biz_id</c>）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>账号状态（<c>status_code</c>；0 未绑定，1 绑定中，2 已绑定，3 绑定失败）。</summary>
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    /// <summary>账号别名（<c>alias</c>）。</summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>绑定失败原因（<c>remark_wrong_msg</c>）。</summary>
    [JsonPropertyName("remark_wrong_msg")]
    public string? RemarkWrongMessage { get; set; }

    /// <summary>绑定备注（<c>remark_content</c>）。</summary>
    [JsonPropertyName("remark_content")]
    public string? RemarkContent { get; set; }

    /// <summary>账号单量配额（<c>quota_num</c>；-1 表示不限制）。</summary>
    [JsonPropertyName("quota_num")]
    public int? QuotaNumber { get; set; }

    /// <summary>配额更新时间（<c>quota_update_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("quota_update_time")]
    public long? QuotaUpdateTime { get; set; }

    /// <summary>服务类型列表（<c>service_type</c>），见 <see cref="WxaExpressService"/>。</summary>
    [JsonPropertyName("service_type")]
    public List<WxaExpressService>? ServiceList { get; set; }

    /// <summary>账号更新时间（<c>update_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>账号创建时间（<c>create_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

/// <summary>绑定 / 解绑快递账号请求体（<c>POST /cgi-bin/express/business/account/bind</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_bindaccount"/>。</para>
/// <para><c>type=bind</c> 绑定（须传入快递公司签约的 <c>password</c>）、<c>type=unbind</c> 解绑。绑定为<b>异步</b>流程：返回结果仅表示受理，最终状态经回调/查询确认。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBindAccountRequest
{
    /// <summary>操作类型（<c>type</c>，必填；bind 绑定 / unbind 解绑）。</summary>
    [JsonPropertyName("type")]
    public string? Action { get; set; }

    /// <summary>快递公司客户编码（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司签约密码（<c>password</c>，绑定必填；解绑不填）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>备注内容（<c>remark_content</c>，可选；子商户模式用，以区分入驻方）。</summary>
    [JsonPropertyName("remark_content")]
    public string? RemarkContent { get; set; }
}

/// <summary>获取打印员列表应答（<c>GET /cgi-bin/express/business/printer/getall</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getprinter"/>。<b>GET</b> 请求（无请求体）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPrinterResponse : WxaResponse
{
    /// <summary>打印员数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>打印员 OpenId 列表（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public List<string>? PrinterOpenIdList { get; set; }

    /// <summary>标签 ID 列表（<c>tagid_list</c>；平台型小程序区分入驻方）。</summary>
    [JsonPropertyName("tagid_list")]
    public List<string>? TagIdList { get; set; }
}

/// <summary>绑定 / 解绑打印员请求体（<c>POST /cgi-bin/express/business/printer/update</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_updateprinter"/>。</para>
/// <para><c>update_type=bind</c> 绑定 / <c>unbind</c> 解绑；绑定后商家端下单可指定打印员（快递员）接收面单打印通知。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdatePrinterRequest
{
    /// <summary>操作类型（<c>update_type</c>，必填；bind 绑定 / unbind 解绑）。</summary>
    [JsonPropertyName("update_type")]
    public string? Action { get; set; }

    /// <summary>打印员 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? PrinterOpenId { get; set; }

    /// <summary>标签 ID 列表（<c>tagid_list</c>，可选；平台型小程序区分入驻方）。</summary>
    [JsonPropertyName("tagid_list")]
    public List<string>? TagIdList { get; set; }
}

/// <summary>查看电子面单余额请求体（<c>POST /cgi-bin/express/business/quota/get</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getquota"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetQuotaRequest
{
    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司客户编码（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }
}

/// <summary>查看电子面单余额应答（<c>quota_num</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getquota"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetQuotaResponse : WxaResponse
{
    /// <summary>电子面单余额（<c>quota_num</c>；-1 表示不限制）。</summary>
    [JsonPropertyName("quota_num")]
    public int QuotaNumber { get; set; }
}

/// <summary>模拟快递公司更新订单状态请求体（<c>POST /cgi-bin/express/business/test_update_order</c>，仅用于<b>测试环境</b>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_testupdateorder"/>。</para>
/// <para>日常测试时调用本接口模拟快递公司推送轨迹，观察商户侧回调联调；生产环境不可调用（官方原文：仅供测试使用）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTestUpdateOrderRequest
{
    /// <summary>快递公司客户编码（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>商户订单号（<c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹发生时间（<c>action_time</c>，必填；Unix 时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；枚举值同附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }
}