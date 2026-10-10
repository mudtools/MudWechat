// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// JSAPI / 小程序下单请求体（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>（同内容另有
/// <c>4012791856</c> 入口）；<b>POST /v3/pay/transactions/jsapi</b>，2026-10-09 逐字段核验。</para>
/// <para><b>字段名照官方原文</b>（含 <c>out_trade_no</c> / <c>support_fapiao</c> / <c>settle_info</c>
/// 等拼写）—— 契约守卫 PAY-B5 断言以官方为准，<b>不得「纠正」为更顺眼的写法</b>。</para>
/// <para><b>必填</b>：<c>appid</c> / <c>mchid</c> / <c>description</c> / <c>out_trade_no</c> /
/// <c>notify_url</c> / <c>amount</c> / <c>payer</c>。其余选填一律可空 ——
/// 与企微线同形：AOT 下源生成按声明类型序列化，可空性即「是否随报文上送」的唯一表达
/// （<b>不做运行时多态</b>，见设计方案 §2.2）。</para>
/// <para><b>时间格式</b>：<c>time_expire</c> 须为 rfc3339（<c>yyyy-MM-ddTHH:mm:ss+08:00</c>），
/// 故保持字符串而非 <see cref="DateTimeOffset"/> —— 序列化器对偏移量的往返格式不可控，
/// 一字节偏差即官方 <c>PARAM_ERROR</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiPrepayRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。须与 <c>mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)），用户微信账单的商品字段中可见。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 商户订单号（<c>out_trade_no</c>，必填 string(32)）：6-32 字符，
    /// 只能是数字、大小写字母 <c>_-|*</c>，同一商户号下唯一。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 支付结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）。超时后无法支付；
    /// 官方建议超时先调关闭订单接口再以新商户订单号重下。
    /// </summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(128)），对用户不可见，查单与回调均原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，必填 string(255)），支付成功回调通知的接收地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)），代金券按标记匹配优惠。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>电子发票入口开放标识（<c>support_fapiao</c>，选填 boolean）。</summary>
    [JsonPropertyName("support_fapiao")]
    public bool? SupportFapiao { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填），见 <see cref="JsapiAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public JsapiAmountInfo? Amount { get; set; }

    /// <summary>支付者信息（<c>payer</c>，必填），见 <see cref="JsapiPayerInfo"/>。</summary>
    [JsonPropertyName("payer")]
    public JsapiPayerInfo? Payer { get; set; }

    /// <summary>商品详情（<c>detail</c>，选填），见 <see cref="JsapiGoodsDetail"/>。</summary>
    [JsonPropertyName("detail")]
    public JsapiGoodsDetail? Detail { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="JsapiSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public JsapiSceneInfo? SceneInfo { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填），见 <see cref="JsapiSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public JsapiSettleInfo? SettleInfo { get; set; }
}

/// <summary>订单金额（<c>amount</c>，必填对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiAmountInfo
{
    /// <summary>订单总金额（<c>total</c>，必填整型，单位为分，必须大于 0；1 元填 100）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>货币类型（<c>currency</c>，选填 string(16)），固定传 <c>CNY</c>（ISO 4217 三位字母代码）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>支付者信息（<c>payer</c>，必填对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiPayerInfo
{
    /// <summary>用户标识（<c>openid</c>，必填 string(128)），用户在商户 <c>appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>商品详情（<c>detail</c>，选填对象）；压缩后总长不超过 6144 字节。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiGoodsDetail
{
    /// <summary>订单原价（<c>cost_price</c>，选填整型，单位为分）。与支付金额不相等则不享受优惠。</summary>
    [JsonPropertyName("cost_price")]
    public long? CostPrice { get; set; }

    /// <summary>商品小票 ID（<c>invoice_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    /// <summary>单品列表（<c>goods_detail</c>，选填 object 数组，至少 1 条）。</summary>
    [JsonPropertyName("goods_detail")]
    public List<JsapiGoodsDetailItem>? GoodsDetail { get; set; }
}

/// <summary>单品明细（<c>detail.goods_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiGoodsDetailItem
{
    /// <summary>商户侧商品编码（<c>merchant_goods_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("merchant_goods_id")]
    public string? MerchantGoodsId { get; set; }

    /// <summary>微信支付商品编码（<c>wechatpay_goods_id</c>，选填 string(32)），官方统一商品编号。</summary>
    [JsonPropertyName("wechatpay_goods_id")]
    public string? WechatpayGoodsId { get; set; }

    /// <summary>商品名称（<c>goods_name</c>，选填 string(256)）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>quantity</c>，必填整型）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>商品单价（<c>unit_price</c>，必填整型，单位为分；须为商户优惠后单价）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }
}

/// <summary>场景信息（<c>scene_info</c>，选填对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiSceneInfo
{
    /// <summary>用户终端 IP（<c>payer_client_ip</c>，必填 string(45)，支持 IPv4 / IPv6）。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>商户端设备号（<c>device_id</c>，选填 string(32)），门店号或收银设备 ID。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>商户门店信息（<c>store_info</c>，选填对象）。</summary>
    [JsonPropertyName("store_info")]
    public JsapiStoreInfo? StoreInfo { get; set; }
}

/// <summary>商户门店信息（<c>scene_info.store_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiStoreInfo
{
    /// <summary>门店编号（<c>id</c>，必填 string(32)），商户自定义，总长不超过 32 字符。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>门店名称（<c>name</c>，选填 string(256)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>地区编码（<c>area_code</c>，选填 string(32)），见官方省市区编号对照表。</summary>
    [JsonPropertyName("area_code")]
    public string? AreaCode { get; set; }

    /// <summary>详细地址（<c>address</c>，选填 string(512)）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>结算信息（<c>settle_info</c>，选填对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiSettleInfo
{
    /// <summary>
    /// 分账标识（<c>profit_sharing</c>，选填 boolean）：<c>true</c> 表示支付成功后可分账，
    /// 资金将冻结转入基本账户不可用余额；<c>false</c>/不传则直接转入可用余额。
    /// </summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }
}
