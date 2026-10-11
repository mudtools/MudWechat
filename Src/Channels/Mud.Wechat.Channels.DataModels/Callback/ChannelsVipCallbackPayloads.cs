// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 小程序会员族（静默关联取会员信息 / 静默关联通知 / 下单页初始权益 /
// 使用会员权益 / 取消使用会员权益；回调的是小程序的回调 URL）。
// 事件键：channels_ec_vip_user_info_get、channels_ec_vip_user_auto_add、channels_ec_gen_order_info_init、
// channels_ec_order_use_vip_discount、channels_ec_order_unuse_vip_discount。
// 注意：info_get / gen_order_info_init / use_vip_discount 三事件官方要求返回结构化 JSON（非 success 文本）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 静默关联时获取小程序会员信息回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.VipUserInfoGet"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 用户尝试关联会员时实时回调，商家需在小程序上线该接口并实时回调应答；若该用户已在小程序入会，
/// 本次关联无需跳转小程序、直接在小店完成。
/// </para>
/// <para>
/// <b>应答差异</b>：本事件须返回结构化 JSON（<c>errcode</c>/<c>errmsg</c>/
/// <c>info.is_vip</c>(bool，true 走静默关联)/<c>info.identity</c>/<c>info.outer_userid</c>），
/// 非「success 文本」形态。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/wxa_callback/channels_ec_vip_user_info_get.html">静默关联时获取小程序会员信息</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.VipUserInfoGet })]
public sealed class ChannelsVipUserInfoGetPayload
{
    /// <summary>获取或设置小程序 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小程序用户 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_vip_user_info_get</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置会员信息主体（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipUserInfo? Info { get; set; }
}

/// <summary>静默关联取会员信息事件主体（官方 <c>info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsVipUserInfo
{
    /// <summary>获取或设置店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }
}

/// <summary>
/// 用户静默关联小店会员通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.VipUserAutoAdd"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 用户通过静默关联方式成为小店会员时通知商家（该接口需商家小程序侧上线，回调的是小程序的回调 URL）。
/// 应答：回复 <c>success</c> 或空字符串（无需加密）。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/wxa_callback/channels_ec_vip_user_auto_add.html">用户静默关联小店会员通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.VipUserAutoAdd })]
public sealed class ChannelsVipUserAutoAddPayload
{
    /// <summary>获取或设置小程序 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小程序用户 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_vip_user_auto_add</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置会员信息主体（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipUserAddInfo? Info { get; set; }
}

/// <summary>静默关联入会事件主体（官方 <c>info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsVipUserAddInfo
{
    /// <summary>获取或设置小程序定义的用户会员身份（官方 <c>identity</c>）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>获取或设置小程序定义的用户会员 id（官方 <c>outer_userid</c>）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置关联成功时间戳，单位秒（官方 <c>add_time</c>）。</summary>
    [JsonPropertyName("add_time")]
    public long? AddTime { get; set; }
}

/// <summary>
/// 用户进入下单页获取初始会员权益回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.GenOrderInfoInit"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 用户进入下单页时实时回调，商家需返回初始会员权益（该接口需商家小程序侧上线）。
/// </para>
/// <para>
/// <b>应答差异</b>：本事件须返回结构化 JSON（<c>errcode</c>/<c>errmsg</c>/<c>info.vip_discounted_price</c>
/// (会员优惠总金额，分)/<c>info.product_infos</c>[]（product_id/sku_id/vip_discounted_price）/
/// <c>info.unuse_shop_discount</c>(可选，0 叠加/1 不叠加）），非「success 文本」形态。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/wxa_callback/channels_ec_gen_order_info_init.html">用户进入下单页获取初始会员权益</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.GenOrderInfoInit })]
public sealed class ChannelsGenOrderInfoInitPayload
{
    /// <summary>获取或设置小程序 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小程序用户 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_gen_order_info_init</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置订单信息主体结构体（官方 <c>gen_order_info</c>）。</summary>
    [JsonPropertyName("gen_order_info")]
    public ChannelsGenOrderInfo? GenOrderInfo { get; set; }
}

/// <summary>下单页订单信息主体（官方 <c>gen_order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGenOrderInfo
{
    /// <summary>获取或设置店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置下单页面的会话 id（官方 <c>order_session_id</c>）。</summary>
    [JsonPropertyName("order_session_id")]
    public string? OrderSessionId { get; set; }

    /// <summary>获取或设置场景（官方 <c>scene</c>）：1 自购；2 群送礼；3 单聊送礼。</summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>获取或设置商品信息列表（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsGenOrderProductInfo>? ProductInfos { get; set; }

    /// <summary>获取或设置价格信息（官方 <c>price_info</c>）。</summary>
    [JsonPropertyName("price_info")]
    public ChannelsGenOrderPriceInfo? PriceInfo { get; set; }
}

/// <summary>下单页商品信息（官方 <c>gen_order_info.product_infos</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGenOrderProductInfo
{
    /// <summary>获取或设置商品 spu id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 sku 数量（官方 <c>sku_cnt</c>）。</summary>
    [JsonPropertyName("sku_cnt")]
    public long? SkuCnt { get; set; }

    /// <summary>获取或设置售卖单价，单位分（官方 <c>sale_price</c>）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置商家优惠金额，单位分（官方 <c>shop_discounted_price</c>）。</summary>
    [JsonPropertyName("shop_discounted_price")]
    public long? ShopDiscountedPrice { get; set; }

    /// <summary>获取或设置商品优惠券信息列表（官方 <c>order_product_coupon_info_list</c>）。</summary>
    [JsonPropertyName("order_product_coupon_info_list")]
    public List<ChannelsGenOrderCouponInfo>? OrderProductCouponInfoList { get; set; }
}

/// <summary>下单页商品优惠券信息（官方 <c>product_infos[].order_product_coupon_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGenOrderCouponInfo
{
    /// <summary>获取或设置用户优惠券 id（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置优惠券类型（官方 <c>coupon_type</c>）：1 商家优惠。</summary>
    [JsonPropertyName("coupon_type")]
    public long? CouponType { get; set; }

    /// <summary>获取或设置优惠金额，单位分，该张优惠券抵扣该商品的金额（官方 <c>discounted_price</c>）。</summary>
    [JsonPropertyName("discounted_price")]
    public long? DiscountedPrice { get; set; }
}

/// <summary>下单页价格信息（官方 <c>gen_order_info.price_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGenOrderPriceInfo
{
    /// <summary>获取或设置会员优惠前商品价格，单位分（官方 <c>product_price_before_vip_discount</c>）。</summary>
    [JsonPropertyName("product_price_before_vip_discount")]
    public long? ProductPriceBeforeVipDiscount { get; set; }

    /// <summary>获取或设置商品总价，单位分（官方 <c>product_price</c>）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>获取或设置商家优惠金额，单位分（官方 <c>shop_discounted_price</c>）。</summary>
    [JsonPropertyName("shop_discounted_price")]
    public long? ShopDiscountedPrice { get; set; }
}

/// <summary>
/// 用户下单使用会员权益回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderUseVipDiscount"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 用户提交订单（未支付成功）且使用会员权益时实时回调，商家确认后才能以此金额下单
/// （该接口需商家小程序侧上线）。
/// </para>
/// <para>
/// <b>应答差异</b>：本事件须返回结构化 JSON（<c>errcode</c>/<c>errmsg</c>/<c>result</c>
/// (number，1 允许下单、2 不允许下单，为空或其他值时拦截下单)），非「success 文本」形态。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/wxa_callback/channels_ec_order_use_vip_discount.html">用户下单使用会员权益</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderUseVipDiscount })]
public sealed class ChannelsOrderUseVipDiscountPayload
{
    /// <summary>获取或设置小程序 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小程序用户 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_use_vip_discount</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置订单信息主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsUseVipDiscountOrderInfo? OrderInfo { get; set; }
}

/// <summary>使用会员权益订单信息主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsUseVipDiscountOrderInfo
{
    /// <summary>获取或设置店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置下单页面的会话 id（官方 <c>order_session_id</c>）。</summary>
    [JsonPropertyName("order_session_id")]
    public string? OrderSessionId { get; set; }

    /// <summary>获取或设置场景（官方 <c>scene</c>）：1 自购；2 群送礼；3 单聊送礼。</summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>获取或设置用户某次下单行为的唯一 id（官方 <c>unique_id</c>）。</summary>
    [JsonPropertyName("unique_id")]
    public string? UniqueId { get; set; }

    /// <summary>获取或设置价格信息（官方 <c>price_info</c>）。</summary>
    [JsonPropertyName("price_info")]
    public ChannelsVipDiscountPriceInfo? PriceInfo { get; set; }
}

/// <summary>使用会员权益价格信息（官方 <c>order_info.price_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsVipDiscountPriceInfo
{
    /// <summary>获取或设置会员优惠前商品价格，单位分（官方 <c>product_price_before_vip_discount</c>）。</summary>
    [JsonPropertyName("product_price_before_vip_discount")]
    public long? ProductPriceBeforeVipDiscount { get; set; }

    /// <summary>获取或设置会员优惠总金额，单位分（官方 <c>vip_discounted_price</c>）。</summary>
    [JsonPropertyName("vip_discounted_price")]
    public long? VipDiscountedPrice { get; set; }

    /// <summary>获取或设置会员优惠后商品价格，单位分（官方 <c>product_price_after_vip_discount</c>）。</summary>
    [JsonPropertyName("product_price_after_vip_discount")]
    public long? ProductPriceAfterVipDiscount { get; set; }
}

/// <summary>
/// 用户取消使用会员权益回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderUnuseVipDiscount"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 调用过「用户下单使用会员权益」后下单失败时通知商家，以 <c>order_info.unique_id</c> 识别一次下单操作
/// （礼物订单选好权益不支付也推送；自购订单提交未支付后取消不推送，用订单取消事件 cancel_type=1/2 感知）。
/// 应答：回复 <c>success</c> 或空字符串（无需加密）。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/wxa_callback/channels_ec_order_unuse_vip_discount.html">用户取消使用会员权益</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderUnuseVipDiscount })]
public sealed class ChannelsOrderUnuseVipDiscountPayload
{
    /// <summary>获取或设置小程序 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小程序用户 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_unuse_vip_discount</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置订单信息主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsUnuseVipDiscountOrderInfo? OrderInfo { get; set; }
}

/// <summary>取消使用会员权益订单信息主体（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsUnuseVipDiscountOrderInfo
{
    /// <summary>获取或设置店铺 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置下单页面的会话 id（官方 <c>order_session_id</c>）。</summary>
    [JsonPropertyName("order_session_id")]
    public string? OrderSessionId { get; set; }

    /// <summary>获取或设置用户某次下单行为的唯一 id（官方 <c>unique_id</c>，与 channels_ec_order_use_vip_discount 事件的同名字段对应）。</summary>
    [JsonPropertyName("unique_id")]
    public string? UniqueId { get; set; }
}
