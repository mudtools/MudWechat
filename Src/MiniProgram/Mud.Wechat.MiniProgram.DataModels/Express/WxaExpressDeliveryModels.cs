// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 获取面单联系人信息请求体（<c>POST /cgi-bin/express/delivery/contact/get</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact"/>。</para>
/// <para>本组接口面向<b>服务商（微信物流服务商）</b>：<c>token</c> 为商户侧下单事件中推送的 Token 字段（非模糊查询令牌），凭运单号获取面单联系人。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetContactRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }
}

/// <summary>获取面单联系人信息应答（<c>sender</c> / <c>receiver</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetContactResponse : WxaResponse
{
    /// <summary>运单 ID（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>发件人信息（<c>sender</c>），见 <see cref="WxaExpressWaybillContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressWaybillContact? Sender { get; set; }

    /// <summary>收件人信息（<c>receiver</c>），见 <see cref="WxaExpressWaybillContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressWaybillContact? Receiver { get; set; }
}

/// <summary>面单联系人（<c>contact/get</c> 的发/收件人；地址已合并省市区）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillContact
{
    /// <summary>用户姓名（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>座机号码（<c>tel</c>）。</summary>
    [JsonPropertyName("tel")]
    public string? TelephoneNumber { get; set; }

    /// <summary>手机号码（<c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? MobileNumber { get; set; }

    /// <summary>地址（<c>address</c>，已合并省市区信息）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>预览面单模板请求体（<c>POST /cgi-bin/express/delivery/template/preview</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate"/>。</para>
/// <para>服务商自定义面单模板时调用，用于预览渲染效果；<c>waybill_template</c> 为需 Base64 编码的 HTML 模板内容，<c>waybill_data</c> 为下单事件返回的 WaybillData。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPreviewTemplateRequest
{
    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单 HTML 模板内容（<c>waybill_template</c>，必填；需经 Base64 编码）。</summary>
    [JsonPropertyName("waybill_template")]
    public string? WaybillTemplate { get; set; }

    /// <summary>面单数据（<c>waybill_data</c>，必填；详情参考下单事件返回值中的 WaybillData）。</summary>
    [JsonPropertyName("waybill_data")]
    public List<WxaExpressWaybillDataItem>? WaybillData { get; set; }

    /// <summary>商户下单数据（<c>custom</c>，必填；格式为商家寄件 <c>order/add</c> 的请求体），见 <see cref="WxaExpressAddOrderRequest"/>。</summary>
    [JsonPropertyName("custom")]
    public WxaExpressAddOrderRequest? CustomData { get; set; }
}

/// <summary>预览面单模板应答（<c>rendered_waybill_template</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPreviewTemplateResponse : WxaResponse
{
    /// <summary>运单 ID（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>渲染后的面单 HTML（<c>rendered_waybill_template</c>）。</summary>
    [JsonPropertyName("rendered_waybill_template")]
    public string? RenderedWaybillTemplate { get; set; }
}

/// <summary>服务商更新订单审核结果请求体（<c>POST /cgi-bin/express/delivery/service/business/update</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatebusiness"/>。</para>
/// <para>服务商承接商户绑定时异步审核（账号绑定为异步流程），审核完成后调用本接口把结果回传微信侧。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateBusinessRequest
{
    /// <summary>委托绑定的小程序 AppId（<c>shop_app_id</c>，必填）。</summary>
    [JsonPropertyName("shop_app_id")]
    public string? ShopAppId { get; set; }

    /// <summary>物流服务商侧商户编号（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>审核结果（<c>result_code</c>，必填；0 成功，2 已开通账号，5 审核驳回，6 重复绑定）。</summary>
    [JsonPropertyName("result_code")]
    public int ResultCode { get; set; }

    /// <summary>审核结果说明（<c>result_msg</c>，驳回时必填）。</summary>
    [JsonPropertyName("result_msg")]
    public string? ResultMessage { get; set; }
}

/// <summary>服务商更新运单轨迹请求体（<c>POST /cgi-bin/express/delivery/path/update</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatepath"/>。</para>
/// <para>服务商（或运力）侧订单状态变化时调用，将轨迹实时同步给微信物流助手。<c>action_type</c> 枚举值同附录 action_type 定义。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdatePathRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹变化时间（<c>action_time</c>，必填；Unix 时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；枚举值同附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填；展示在快递轨迹详情页）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }
}

/// <summary>退货地址（<c>no_worry_return</c> 与 <c>return</c> 两族的 <c>biz_addr</c> / <c>user_addr</c> 内嵌对象）。</summary>
/// <remarks>字段以官方退货相关文档的 Body 对象 Payload 为准（2026-10-11 逐页核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressReturnAddress
{
    /// <summary>国家（<c>country</c>）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>省份（<c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>城市（<c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（<c>area</c>）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>详细地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>联系人（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>联系电话（<c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? MobileNumber { get; set; }
}

/// <summary>退货商品（退货组件 <c>goods_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressReturnGood
{
    /// <summary>退货商品的名称（<c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>退货商品图片 URL（<c>url</c>，必填）。</summary>
    [JsonPropertyName("url")]
    public string? ImageUrl { get; set; }
}

/// <summary>
/// 创建退货 ID 请求体（<c>POST /cgi-bin/express/delivery/no_worry_return/add</c>，无忧退货运费险）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid"/>。</para>
/// <para>
/// <c>shop_order_id</c> 与退货 ID 一一对应；“一个订单需要多次退货”可在商家内部用 1 个退货订单号映射多个 <c>shop_order_id</c>。
/// 创建退货 ID 时平台自动下发模板消息给用户提醒退货。相关字段用 UTF-8 编码。
/// <c>wx_pay_id</c> 为<b>已投保的微信支付单号</b>（必填）：无该字段返回 9300569，未投保即 9300570。
/// 创建成功后返回 <c>return_id</c>，平台据其下发模板消息。
/// </para>
/// <para>官方错误码：40097（order_id 为空或退货地址为空或 wx_pay_id 为空）、9300522（shop_order_id 已存在）、9300569 / 9300570（微信支付单号问题）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddReturnIdRequest
{
    /// <summary>商家内部系统使用的退货编号（<c>shop_order_id</c>，必填）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家退货地址（<c>biz_addr</c>，必填），见 <see cref="WxaExpressReturnAddress"/>。</summary>
    [JsonPropertyName("biz_addr")]
    public WxaExpressReturnAddress? ShopAddress { get; set; }

    /// <summary>用户购物时的收货地址（<c>user_addr</c>，可选），见 <see cref="WxaExpressReturnAddress"/>。</summary>
    [JsonPropertyName("user_addr")]
    public WxaExpressReturnAddress? UserAddress { get; set; }

    /// <summary>退货用户的 OpenId（<c>openid</c>，必填；用于下发模板消息）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>退货订单在商家小程序的路径（<c>order_path</c>，必填）。</summary>
    [JsonPropertyName("order_path")]
    public string? OrderPath { get; set; }

    /// <summary>退货商品列表（<c>goods_list</c>，必填），见 <see cref="WxaExpressReturnGood"/>。</summary>
    [JsonPropertyName("goods_list")]
    public List<WxaExpressReturnGood>? GoodsList { get; set; }

    /// <summary>退货订单的价格（<c>order_price</c>，必填）。</summary>
    [JsonPropertyName("order_price")]
    public int? OrderPrice { get; set; }

    /// <summary>已投保的微信支付单号（<c>wx_pay_id</c>，必填）。</summary>
    [JsonPropertyName("wx_pay_id")]
    public string? WxPayId { get; set; }
}

/// <summary>创建退货 ID 应答（<c>return_id</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddReturnIdResponse : WxaResponse
{
    /// <summary>退货 ID（<c>return_id</c>）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}

/// <summary>查询退货 ID 状态请求体（<c>POST /cgi-bin/express/delivery/no_worry_return/get</c>，无忧退货运费险）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_getreturnid"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetReturnIdRequest
{
    /// <summary>退货 ID（<c>return_id</c>，必填）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}

/// <summary>解绑退货 ID 请求体（<c>POST /cgi-bin/express/delivery/no_worry_return/unbind</c>，无忧退货运费险）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_unbindreturnid"/>。</para>
/// <para>与用户达成协商「无需退货」时解除商家退货单与退货 ID 的绑定；仅当用户自主填写运单号时才支持解绑。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUnbindReturnIdRequest
{
    /// <summary>退货 ID（<c>return_id</c>，必填）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}

/// <summary>
/// 退货 ID 状态应答（<c>status</c> / <c>waybill_id</c> / <c>order_status</c> / <c>delivery_name</c> / <c>delivery_id</c>，
/// <c>no_worry_return/get</c> 与 <c>return/get</c> 两端点同形复用）。
/// </summary>
/// <remarks>
/// <para><c>status</c>：0 用户未填写退货信息，1 预约上门取件，2 填写自行寄回运单号；<c>order_status</c> 为运单状态（0 已下单待揽件 … 99 未知）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressReturnStatusResponse : WxaResponse
{
    /// <summary>退货 ID 状态（<c>status</c>；0 未填写退货信息，1 预约上门取件，2 填写自行寄回运单号）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>运单状态（<c>order_status</c>；0 已下单待揽件，1 已揽件，2 运输中，3 派件中，4 已签收，5 异常，6 代签收，7 揽收失败，8 签收失败，11 已取消，13 退件中，14 已退件，99 未知）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>运力公司编码（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运力公司名称（<c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>物流消息商品信息（<c>open_msg</c> 族的 <c>goods_info</c> 内嵌对象）。</summary>
/// <remarks>字段以官方「物流消息」相关文档核验（2026-10-11）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillGoods
{
    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品图片 URL（<c>goods_img_url</c>）。</summary>
    [JsonPropertyName("goods_img_url")]
    public string? GoodsImageUrl { get; set; }

    /// <summary>商品详情描述（<c>goods_desc</c>）。</summary>
    [JsonPropertyName("goods_desc")]
    public string? GoodsDescription { get; set; }

    /// <summary>商品详情列表（<c>detail_list</c>），见 <see cref="WxaExpressWaybillGoodsDetail"/>。</summary>
    [JsonPropertyName("detail_list")]
    public List<WxaExpressWaybillGoodsDetail>? DetailList { get; set; }
}

/// <summary>物流消息商品详情项（<c>goods_info.detail_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillGoodsDetail
{
    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品图片 URL（<c>goods_img_url</c>）。</summary>
    [JsonPropertyName("goods_img_url")]
    public string? GoodsImageUrl { get; set; }

    /// <summary>商品详情描述（<c>goods_desc</c>）。</summary>
    [JsonPropertyName("goods_desc")]
    public string? GoodsDescription { get; set; }
}

/// <summary>用户绑定运单请求体（<c>POST /cgi-bin/express/delivery/open_msg/follow_waybill</c>，物流消息）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill"/>。</para>
/// <para>
/// <b>频率上限（官方原文）</b>：同一 appid 最多调用 <b>10 万次/日</b>；同一用户最多被调用 <b>50 次/日</b>；
/// 同一用户最多同时绑定 <b>100 个</b>运单号。绑定后向该用户下发物流状态通知。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressFollowWaybillRequest
{
    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>发件人手机号（<c>sender_phone</c>，可选；建议绑定后提醒用户）。</summary>
    [JsonPropertyName("sender_phone")]
    public string? SenderPhoneNumber { get; set; }

    /// <summary>收件人手机号（<c>receiver_phone</c>，可选）。</summary>
    [JsonPropertyName("receiver_phone")]
    public string? ReceiverPhoneNumber { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }

    /// <summary>微信支付交易单号（<c>trans_id</c>，可选）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>订单详情页路径（<c>order_detail_path</c>，可选；用户点击物流通知卡片跳转的小程序路径）。</summary>
    [JsonPropertyName("order_detail_path")]
    public string? OrderDetailPath { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，可选；命中可云端识别的物流公司时可不填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }
}

/// <summary>用户绑定 / 查询运单应答（<c>waybill_token</c>，后续查询凭据）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressFollowWaybillResponse : WxaResponse
{
    /// <summary>运单 Token（<c>waybill_token</c>；后续查询运单轨迹 / 更新商品时凭此定位）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }
}

/// <summary>查询运单轨迹请求体（<c>POST /cgi-bin/express/delivery/open_msg/query_follow_trace</c>，物流消息）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_queryfollowtrace"/>；凭 <see cref="WxaExpressFollowWaybillResponse.WaybillToken"/> 查询。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryFollowTraceRequest
{
    /// <summary>运单 Token（<c>waybill_token</c>，必填；绑定/查询运单时返回）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>更新运单商品信息请求体（<c>POST /cgi-bin/express/delivery/open_msg/update_follow_waybill_goods</c>，物流消息）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_updatefollowwaybillgood"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateFollowWaybillGoodsRequest
{
    /// <summary>运单 Token（<c>waybill_token</c>，必填）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }
}

/// <summary>查询运单轨迹应答（<c>open_msg</c> 族 <c>query_follow_trace</c> 与 <c>query_trace</c> 同形复用）。</summary>
/// <remarks>
/// <para><c>waybill_info.status</c>：0 无轨迹（未揽收），1 快递已揽收，2 运输中，3 已签收，4 派件中，5 退回件，6 签收异常，7 拒收，8 揽件异常，9 变更配送地址，10 派件延迟，11 已退回，12 已寄出，13 转投。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceResponse : WxaResponse
{
    /// <summary>运单信息（<c>waybill_info</c>），见 <see cref="WxaExpressTraceWaybill"/>。</summary>
    [JsonPropertyName("waybill_info")]
    public WxaExpressTraceWaybill? WaybillInfo { get; set; }

    /// <summary>店铺信息（<c>shop_info</c>），见 <see cref="WxaExpressTraceShop"/>。</summary>
    [JsonPropertyName("shop_info")]
    public WxaExpressTraceShop? ShopInfo { get; set; }

    /// <summary>快递信息（<c>delivery_info</c>），见 <see cref="WxaExpressTraceDelivery"/>。</summary>
    [JsonPropertyName("delivery_info")]
    public WxaExpressTraceDelivery? DeliveryInfo { get; set; }
}

/// <summary>查询运单轨迹的运单信息（<c>waybill_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceWaybill
{
    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>运单状态（<c>status</c>；枚举见 <see cref="WxaExpressTraceResponse"/> 备注）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

/// <summary>查询运单轨迹的店铺信息（<c>shop_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceShop
{
    /// <summary>商品信息（<c>goods_info</c>），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }
}

/// <summary>查询运单轨迹的快递信息（<c>delivery_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceDelivery
{
    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司名称（<c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>查询运单轨迹请求体（<c>POST /cgi-bin/express/delivery/open_msg/query_trace</c>，物流查询插件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_querytrace"/>。</para>
/// <para><b>频率上限（官方原文）</b>：一个 appid 最多调用 <b>10 万次/日</b>；一个用户最多调用 <b>100 次/日</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryTraceRequest
{
    /// <summary>运单 Token（<c>waybill_token</c>，必填；trace_waybill 时返回）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>查询运单信息请求体（<c>POST /cgi-bin/express/delivery/open_msg/trace_waybill</c>，物流查询插件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill"/>。</para>
/// <para><c>delivery_id</c> 快递公司 ID（必填；无单号无法匹配快递公司时可用 single-delivery 场景的运单绑定先转正）；返回 <c>waybill_token</c> 供后续查询。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceWaybillRequest
{
    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>发件人手机号（<c>sender_phone</c>，可选；快递公司未命中时可用寄/收件人手机号加权匹配）。</summary>
    [JsonPropertyName("sender_phone")]
    public string? SenderPhoneNumber { get; set; }

    /// <summary>收件人手机号（<c>receiver_phone</c>，可选）。</summary>
    [JsonPropertyName("receiver_phone")]
    public string? ReceiverPhoneNumber { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>微信支付交易单号（<c>trans_id</c>，可选）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>订单详情页路径（<c>order_detail_path</c>，可选）。</summary>
    [JsonPropertyName("order_detail_path")]
    public string? OrderDetailPath { get; set; }
}

/// <summary>查询运单信息应答（<c>waybill_token</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceWaybillResponse : WxaResponse
{
    /// <summary>运单 Token（<c>waybill_token</c>；后续 query_trace 查询轨迹时使用）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }
}

/// <summary>更新运单商品信息请求体（<c>POST /cgi-bin/express/delivery/open_msg/update_waybill_goods</c>，物流查询插件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_updatewaybillgoods"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateWaybillGoodsRequest
{
    /// <summary>运单 Token（<c>waybill_token</c>，必填）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }
}

/// <summary>获取快递公司列表应答（<c>POST /cgi-bin/express/delivery/open_msg/get_delivery_list</c>，物流消息 / 查询插件共用）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_getdeliverylist"/>。</para>
/// <para>返回支持物流消息 / 查询能力的快递公司列表；调用时请求体可传空对象。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetDeliveryListResponse : WxaResponse
{
    /// <summary>快递公司数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>快递公司列表（<c>delivery_list</c>），见 <see cref="WxaExpressOpenMsgDelivery"/>。</summary>
    [JsonPropertyName("delivery_list")]
    public List<WxaExpressOpenMsgDelivery>? DeliveryList { get; set; }
}

/// <summary>物流消息 / 查询插件支持快递公司（<c>delivery_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressOpenMsgDelivery
{
    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司名称（<c>delivery_name</c>）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}

/// <summary>
/// 创建退货 ID 请求体（<c>POST /cgi-bin/express/delivery/return/add</c>，退货组件）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn"/>。</para>
/// <para>
/// 与无忧退货运费险不同：<b>本族无 <c>wx_pay_id</c></b>（不要求保单）；<c>shop_order_id</c> 与退货 ID 一一对应，
/// “一个订单需要多次退货”时可在商家内部 1 个退货订单号映射多个 <c>shop_order_id</c>。
/// 创建退货 ID 时平台自动下发模板消息给用户提醒退货。
/// </para>
/// <para>官方错误码：40097（order_id 或退货地址为空）、9300522（shop_order_id 已存在）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddReturnRequest
{
    /// <summary>商家内部系统使用的退货编号（<c>shop_order_id</c>，必填）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家退货地址（<c>biz_addr</c>，必填），见 <see cref="WxaExpressReturnAddress"/>。</summary>
    [JsonPropertyName("biz_addr")]
    public WxaExpressReturnAddress? ShopAddress { get; set; }

    /// <summary>用户购物时的收货地址（<c>user_addr</c>，可选），见 <see cref="WxaExpressReturnAddress"/>。</summary>
    [JsonPropertyName("user_addr")]
    public WxaExpressReturnAddress? UserAddress { get; set; }

    /// <summary>退货用户的 OpenId（<c>openid</c>，必填；用于下发模板消息）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>退货订单在商家小程序的路径（<c>order_path</c>，必填）。</summary>
    [JsonPropertyName("order_path")]
    public string? OrderPath { get; set; }

    /// <summary>退货商品列表（<c>goods_list</c>，必填），见 <see cref="WxaExpressReturnGood"/>。</summary>
    [JsonPropertyName("goods_list")]
    public List<WxaExpressReturnGood>? GoodsList { get; set; }

    /// <summary>退货订单的价格（<c>order_price</c>，必填）。</summary>
    [JsonPropertyName("order_price")]
    public int OrderPrice { get; set; }
}

/// <summary>创建退货 ID 应答（<c>return_id</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddReturnResponse : WxaResponse
{
    /// <summary>退货 ID（<c>return_id</c>）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}

/// <summary>查询退货 ID 状态请求体（<c>POST /cgi-bin/express/delivery/return/get</c>，退货组件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_getreturn"/>；返回见 <see cref="WxaExpressReturnStatusResponse"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetReturnRequest
{
    /// <summary>退货 ID（<c>return_id</c>，必填）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}

/// <summary>解绑退货 ID 请求体（<c>POST /cgi-bin/express/delivery/return/unbind</c>，退货组件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_unbindreturn"/>。</para>
/// <para>商家同意退货后与用户达成协商「无需退货」时解除绑定；仅当用户自主填写运单号（自行寄回）时才支持解绑。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUnbindReturnRequest
{
    /// <summary>退货 ID（<c>return_id</c>，必填）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}

/// <summary>运力更新订单状态请求体（<c>POST /cgi-bin/express/delivery/single_waybill/update</c>，散单寄件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderstatus"/>。</para>
/// <para>
/// <c>token</c> 为商户侧下单事件中推送的 Token 字段；<c>action_type</c> 枚举见附录：
/// 90001 网点接单 / 90002 分配业务员 / 90010 待支付 / 90011 已支付 / 100001 揽件成功 / 100002 揽件失败 /
/// 200001 更新运输轨迹 / 300002 派送中 / 300003 签收成功 / 300004 签收失败 / 300005 第三方代收入库 /
/// 300006 第三方代收取出 / 300007 代签收 / 400001 订单取消 / 400002 订单滞留 / 400003 订单退回 /
/// 400004 订单拒收 / 400005 问题件 / 500001 其他未分类。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillUpdateRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，可选）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹变化 Unix 时间戳（<c>action_time</c>，必填）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；与普通单保持一致，参见附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填；展示在快递轨迹详情页，若有手机号码则直接写 11 位手机号码，UTF-8 编码）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>取件员姓名（<c>pickup_courier_name</c>，必填）。</summary>
    [JsonPropertyName("pickup_courier_name")]
    public string? PickupCourierName { get; set; }

    /// <summary>取件员电话（<c>pickup_courier_phone</c>，必填）。</summary>
    [JsonPropertyName("pickup_courier_phone")]
    public string? PickupCourierPhone { get; set; }

    /// <summary>派件员姓名（<c>delivery_courier_name</c>，必填）。</summary>
    [JsonPropertyName("delivery_courier_name")]
    public string? DeliveryCourierName { get; set; }

    /// <summary>派件员电话（<c>delivery_courier_phone</c>，必填）。</summary>
    [JsonPropertyName("delivery_courier_phone")]
    public string? DeliveryCourierPhone { get; set; }
}

/// <summary>运力更新订单费用请求体（<c>POST /cgi-bin/express/delivery/single_waybill/fee</c>，散单寄件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderfee"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillFeeRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>是否需要支付（<c>need_pay</c>，必填；0 表示无需支付，1 表示需要支付）。</summary>
    [JsonPropertyName("need_pay")]
    public int NeedPay { get; set; }

    /// <summary>实际快递费用（<c>fee</c>，单位：分；结算价）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>原价（<c>original_fee</c>，单位：分）。</summary>
    [JsonPropertyName("original_fee")]
    public int OriginalFee { get; set; }

    /// <summary>基础运费（<c>base_fee</c>，单位：分）。</summary>
    [JsonPropertyName("base_fee")]
    public int BaseFee { get; set; }

    /// <summary>保价费用（<c>insured_fee</c>，单位：分）。</summary>
    [JsonPropertyName("insured_fee")]
    public int InsuredFee { get; set; }

    /// <summary>其他费用（<c>other_fee</c>，单位：分）。</summary>
    [JsonPropertyName("other_fee")]
    public int OtherFee { get; set; }

    /// <summary>费用备注（<c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>费用商品名称（<c>pay_goods_name</c>）。</summary>
    [JsonPropertyName("pay_goods_name")]
    public string? PayGoodsName { get; set; }
}

/// <summary>运力取消订单请求体（<c>POST /cgi-bin/express/delivery/single_waybill/cancel_order</c>，散单寄件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterdeliverycancel"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillCancelRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>取消原因（<c>reason</c>，必填）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>运力对订单退款请求体（<c>POST /cgi-bin/express/delivery/single_waybill/refund_order</c>，散单寄件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterdeliveryrefund"/>；<c>fee</c> 为退款金额（单位：分）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillRefundRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>退款金额（<c>fee</c>，必填；单位：分）。</summary>
    [JsonPropertyName("fee")]
    public int RefundFee { get; set; }
}

/// <summary>运力拉取商家寄件对账文件请求体（<c>POST /cgi-bin/express/delivery/single_waybill/get_bill</c>，散单寄件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scattergetbill"/>。</para>
/// <para>
/// <b>成功时返回对账文件内容（非 JSON），失败时才返回 JSON</b>（错误信封 <c>errcode</c>/<c>errmsg</c>）⇒
/// 本端点走独立请求形态 <see cref="Mud.Wechat.MiniProgram.IWxaExpressBillService"/>（不进入 JSON 反序列化管线）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillGetBillRequest
{
    /// <summary>对账日期（<c>date</c>，必填；格式 YYYY-MM-DD）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>对账类型（<c>type</c>，必填；ALL 全部 / SUCCESS 成功订单 / REFUND 退款订单）。</summary>
    [JsonPropertyName("type")]
    public string? BillType { get; set; }
}

/// <summary>运力回传用户评价反馈请求体（<c>POST /cgi-bin/express/delivery/single_waybill/evaluate_feedback</c>，散单寄件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterevaluatefeedback"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillEvaluateFeedbackRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>评价处理结果（<c>result</c>，必填；0 已处理，1 不处理）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>处理结果描述（<c>desc</c>，可选）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}

/// <summary>运力返回用户投诉处理结果请求体（<c>POST /cgi-bin/express/delivery/scatter/update_complaint_result</c>，散单寄件）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdatecomplainresult"/>
/// （接口英文名 <c>scatterupdatecomplainresult</c>，2026-10-12 核验）。成功仅返回 <c>errcode</c>/<c>errmsg</c>。
/// </para>
/// <para>
/// <b>官方原文</b>：<c>token</c> 为<b>商户侧下单事件中推送的 Token 字段</b>——与用户评价反馈
/// （<see cref="WxaExpressSingleWaybillEvaluateFeedbackRequest"/>）同源，<b>不得</b>用应用级令牌替代。
/// 本接口<b>不支持</b>第三方平台代调用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateComplaintResultRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>处理结果（<c>result</c>，必填）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>处理结果说明（<c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}

/// <summary>物流轨迹推送的「寄件人 / 收件人」信息（<c>sender</c> / <c>receiver</c> 内嵌对象，<c>pathnotify</c> 专用）。</summary>
/// <remarks>
/// <para>
/// 官方字段以 <c>delivery/pathnotify</c> 的「Body.sender / Body.receiver Object Payload」为准（2026-10-12 核验）。
/// </para>
/// <para>
/// <b>与 <see cref="WxaExpressContact"/> 的差异（勿混用）</b>：本结构的收寄件人只有单一 <c>phone</c>
/// （无 <c>tel</c>/<c>mobile</c> 二选一），且带 <c>street</c> / <c>id</c> 两个地址扩展字段。
/// 官方原文：姓名<b>收件人必传、寄件人非必传</b>；电话<b>收件人必传、寄件人非必传</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyContact
{
    /// <summary>姓名（<c>name</c>；收件人必传，寄件人非必传）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>电话（<c>phone</c>；收件人必传，寄件人非必传）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>省（<c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>市（<c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区/县（<c>area</c>，必填）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>街道（<c>street</c>，可选）。</summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>地址（<c>address</c>，可选）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>地址 ID（<c>id</c>，可选）。</summary>
    [JsonPropertyName("id")]
    public string? AddressId { get; set; }
}

/// <summary>待推送的物流轨迹（<c>path</c> 内嵌对象，<c>pathnotify</c> 专用）。</summary>
/// <remarks>
/// <c>action_type</c> 为官方枚举（与普通单一致）：<c>90001</c> 网点接单、<c>90002</c> 分配业务员、
/// <c>100001</c> 揽件成功、<c>200001</c> 更新运输轨迹、<c>300002</c> 派送中、<c>300003</c> 签收成功、
/// <c>400001</c> 订单取消、<c>500001</c> 兜底状态等。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyPath
{
    /// <summary>轨迹变化 Unix 时间戳（<c>action_time</c>，必填；单位：秒）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；官方枚举值，见类说明）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填；展示在快递轨迹详情页中）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>取件员姓名（<c>pickup_courier_name</c>，可选；分配取件员成功时返回）。</summary>
    [JsonPropertyName("pickup_courier_name")]
    public string? PickupCourierName { get; set; }

    /// <summary>取件员电话（<c>pickup_courier_phone</c>，可选；分配取件员成功时返回）。</summary>
    [JsonPropertyName("pickup_courier_phone")]
    public string? PickupCourierPhoneNumber { get; set; }

    /// <summary>派件员姓名（<c>delivery_courier_name</c>，可选；分配派件员成功时返回）。</summary>
    [JsonPropertyName("delivery_courier_name")]
    public string? DeliveryCourierName { get; set; }

    /// <summary>派件员电话（<c>delivery_courier_phone</c>，可选；分配派件员成功时返回）。</summary>
    [JsonPropertyName("delivery_courier_phone")]
    public string? DeliveryCourierPhoneNumber { get; set; }
}

/// <summary>推送已绑定物流轨迹信息请求体（<c>POST /cgi-bin/express/delivery/pathnotify</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliverypathnotify"/>
/// （接口英文名 <c>deliveryPathNotify</c>）。运力方调用本接口向已绑定用户推送物流轨迹消息。
/// </para>
/// <para>
/// 响应 <c>exist</c> 表示<b>该手机号是否已绑定</b>（<c>0</c> 未绑定 / <c>1</c> 已绑定）；
/// 未绑定时轨迹消息<b>不会</b>送达，需先引导用户完成绑定。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyRequest
{
    /// <summary>寄件人（<c>sender</c>，必填），见 <see cref="WxaExpressPathNotifyContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressPathNotifyContact? Sender { get; set; }

    /// <summary>收件人（<c>receiver</c>，必填），见 <see cref="WxaExpressPathNotifyContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressPathNotifyContact? Receiver { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>当前需要推送消息的轨迹（<c>path</c>，必填），见 <see cref="WxaExpressPathNotifyPath"/>。</summary>
    [JsonPropertyName("path")]
    public WxaExpressPathNotifyPath? Path { get; set; }

    /// <summary>运单创建时间（<c>create_time</c>，必填；Unix 时间戳，单位：秒）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

/// <summary>推送已绑定物流轨迹信息响应（<c>POST /cgi-bin/express/delivery/pathnotify</c>）。</summary>
/// <remarks>官方错误码要点：<c>150004</c>（运力配置不存在）、<c>932001</c>~<c>932003</c>（反查运单系统 / 逻辑错误 / 数据异常）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyResponse : WxaResponse
{
    /// <summary>用户是否绑定（<c>exist</c>；<c>0</c> 未绑定，<c>1</c> 已绑定）。</summary>
    [JsonPropertyName("exist")]
    public int Exist { get; set; }
}

/// <summary>用户手机状态查询请求体（<c>POST /cgi-bin/express/delivery/userquery</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliveryuserquery"/>
/// （接口英文名 <c>deliveryUserQuery</c>）。运力方调用本接口查询某手机号是否已绑定。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUserQueryRequest
{
    /// <summary>手机号（<c>phone</c>，必填）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }
}

/// <summary>用户手机状态查询响应（<c>POST /cgi-bin/express/delivery/userquery</c>）。</summary>
/// <remarks>错误码口径与 <see cref="WxaExpressPathNotifyResponse"/> 相同（<c>150004</c> 运力配置不存在、<c>932001</c>~<c>932003</c> 反查运单异常）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUserQueryResponse : WxaResponse
{
    /// <summary>用户是否绑定（<c>exist</c>；<c>0</c> 未绑定，<c>1</c> 已绑定）。</summary>
    [JsonPropertyName("exist")]
    public int Exist { get; set; }
}