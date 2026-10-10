// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 小程序下单请求体（<c>/cgi-bin/miniapppay/create_order</c>，普通支付域）。
/// <para>
/// 官方业务限制：商户系统内部订单号只能是数字、大小写字母、<c>_-|*</c> 且在同一商户号下唯一；
/// 货币类型 CNY（境内商户号仅支持人民币）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class CreatePayOrderRequest
{
    /// <summary>
    /// 获取或设置应用 ID（官方必填，二级商户申请的公众号或移动应用 appid）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置商户号（官方必填，二级商户号，由企业微信生成并下发）。
    /// </summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置商户订单号（官方必填，6~32 个字符）：
    /// 商户系统内部订单号，只能是数字、大小写字母、<c>_-|*</c> 且在同一商户号下唯一。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 获取或设置商品描述（官方必填，1~127 个字符）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置下单场景 key（官方可选，1~256 个字符）：用于统计企微成员发出小程序的交易业绩，
    /// 可从小程序 URL 获取；统计结果展示在「对外收款 - 成员业绩」中，不传则不统计。
    /// </summary>
    [JsonPropertyName("scenekey")]
    public string? SceneKey { get; set; }

    /// <summary>
    /// 获取或设置订单金额信息（官方必填，见 <see cref="PayOrderAmount"/>）。
    /// </summary>
    [JsonPropertyName("amount")]
    public PayOrderAmount? Amount { get; set; }

    /// <summary>
    /// 获取或设置支付者信息（官方必填，见 <see cref="PayOrderPayer"/>）。
    /// </summary>
    [JsonPropertyName("payer")]
    public PayOrderPayer? Payer { get; set; }

    /// <summary>
    /// 获取或设置交易结束时间（官方可选，1~64 个字符）：
    /// 订单失效时间，遵循 rfc3339 标准，格式为 yyyy-MM-DDTHH:mm:ss+TIMEZONE。
    /// </summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>
    /// 获取或设置附加数据（官方可选，1~128 个字符）：在查单和支付通知中原样返回。
    /// </summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>
    /// 获取或设置订单优惠标记（官方可选，1~32 个字符）。
    /// </summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>
    /// 获取或设置优惠功能信息（官方可选，见 <see cref="PayOrderDetail"/>）。
    /// </summary>
    [JsonPropertyName("detail")]
    public PayOrderDetail? Detail { get; set; }

    /// <summary>
    /// 获取或设置场景信息（官方必填，见 <see cref="PayOrderSceneInfo"/>）。
    /// </summary>
    [JsonPropertyName("scene_info")]
    public PayOrderSceneInfo? SceneInfo { get; set; }
}

/// <summary>
/// 订单金额信息（<see cref="CreatePayOrderRequest.Amount"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderAmount
{
    /// <summary>
    /// 获取或设置订单总金额（官方必填，单位为分）。
    /// </summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>
    /// 获取或设置货币类型（官方必填）：CNY - 人民币，境内商户号仅支持人民币。
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 支付者信息（<see cref="CreatePayOrderRequest.Payer"/> /
/// <see cref="GetPayOrderResponse.Payer"/>，下单与查单结构一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderPayer
{
    /// <summary>
    /// 获取或设置用户在子商户 appid 下的唯一标识（官方必填，1~128 个字符）。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 优惠功能信息（<see cref="CreatePayOrderRequest.Detail"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderDetail
{
    /// <summary>
    /// 获取或设置订单原价（官方可选，单位为分）：用于记录整张小票的交易金额；
    /// 当订单原价与支付金额不相等则不享受优惠；防止同一张小票分多次支付享受多次优惠，
    /// 正常支付订单不必上传。
    /// </summary>
    [JsonPropertyName("cost_price")]
    public long? CostPrice { get; set; }

    /// <summary>
    /// 获取或设置商品小票 ID（官方可选，1~32 个字符）。
    /// </summary>
    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    /// <summary>
    /// 获取或设置单品列表（官方可选，见 <see cref="PayOrderGoodsDetail"/>）。
    /// </summary>
    [JsonPropertyName("goods_detail")]
    public List<PayOrderGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>
/// 下单单品信息（<see cref="PayOrderDetail.GoodsDetail"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderGoodsDetail
{
    /// <summary>
    /// 获取或设置商户侧商品编码（官方必填，1~32 个字符）：
    /// 由半角大小写字母、数字、中划线、下划线中的一种或几种组成。
    /// </summary>
    [JsonPropertyName("merchant_goods_id")]
    public string? MerchantGoodsId { get; set; }

    /// <summary>
    /// 获取或设置微信支付商品编码（官方可选，1~32 个字符）：
    /// 微信支付定义的统一商品编号（没有可不传）。
    /// </summary>
    [JsonPropertyName("wechatpay_goods_id")]
    public string? WechatPayGoodsId { get; set; }

    /// <summary>
    /// 获取或设置商品名称（官方可选，1~256 个字符）。
    /// </summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>
    /// 获取或设置商品数量（官方必填）：用户购买的数量。
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>
    /// 获取或设置商品单价（官方必填，单位为分）。
    /// </summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }
}

/// <summary>
/// 下单场景信息（<see cref="CreatePayOrderRequest.SceneInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderSceneInfo
{
    /// <summary>
    /// 获取或设置用户终端 IP（官方必填，1~45 个字符）：支持 IPv4 和 IPv6 两种格式。
    /// </summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>
    /// 获取或设置商户端设备号（官方可选，1~32 个字符）：门店号或收银设备 ID。
    /// </summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// 获取或设置门店信息（官方必填，见 <see cref="PayOrderStoreInfo"/>）。
    /// </summary>
    [JsonPropertyName("store_info")]
    public PayOrderStoreInfo? StoreInfo { get; set; }
}

/// <summary>
/// 下单门店信息（<see cref="PayOrderSceneInfo.StoreInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderStoreInfo
{
    /// <summary>
    /// 获取或设置门店编号（官方必填，1~32 个字符）：商户侧门店编号。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置门店名称（官方可选，1~256 个字符）：商户侧门店名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置地区编码（官方可选，1~32 个字符）：详见官方省市区编号对照表。
    /// </summary>
    [JsonPropertyName("area_code")]
    public string? AreaCode { get; set; }

    /// <summary>
    /// 获取或设置详细地址（官方可选，1~512 个字符）：详细的商户门店地址。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
