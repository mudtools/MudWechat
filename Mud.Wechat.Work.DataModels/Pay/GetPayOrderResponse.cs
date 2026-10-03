// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 查询订单响应体（<c>/cgi-bin/miniapppay/get_order</c>，普通支付域）。
/// <para>
/// 官方口径：用于未收到支付通知、支付接口返回系统错误或未知状态、
/// 以及调关单/撤销前确认支付状态的场景。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayOrderResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置商户号（二级商户号，由企业微信生成并下发）。
    /// </summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置商户订单号（商户系统内部订单号）。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 获取或设置交易类型（官方可选）：JSAPI - 公众号支付、NATIVE - 扫码支付、APP、
    /// MICROPAY - 付款码支付、MWEB - H5 支付、FACEPAY - 刷脸支付。
    /// </summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>
    /// 获取或设置交易状态：SUCCESS - 支付成功、REFUND - 转入退款、NOTPAY - 未支付、
    /// CLOSED - 已关闭、REVOKED - 已撤销（付款码支付）、USERPAYING - 用户支付中（付款码支付）、
    /// PAYERROR - 支付失败、ACCEPT - 已接收等待扣款。
    /// </summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>
    /// 获取或设置交易状态描述（官方必填，如「支付失败，请重新下单支付」）。
    /// </summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>
    /// 获取或设置支付者信息（官方必填，见 <see cref="PayOrderPayer"/>）。
    /// </summary>
    [JsonPropertyName("payer")]
    public PayOrderPayer? Payer { get; set; }

    /// <summary>
    /// 获取或设置微信支付订单号（微信支付系统生成的订单号）。
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 获取或设置付款银行类型（官方可选，字符串类型银行标识）。
    /// </summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>
    /// 获取或设置附加数据（官方可选）：查询 API 和支付通知中原样返回，仅支付完成状态才返回。
    /// </summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>
    /// 获取或设置支付完成时间（官方可选）：rfc3339 格式 yyyy-MM-DDTHH:mm:ss+TIMEZONE。
    /// </summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>
    /// 获取或设置订单金额信息（见 <see cref="PayOrderTradeAmount"/>）。
    /// </summary>
    [JsonPropertyName("amount")]
    public PayOrderTradeAmount? Amount { get; set; }

    /// <summary>
    /// 获取或设置场景信息（官方可选，见 <see cref="PayOrderTradeSceneInfo"/>）。
    /// <para>
    /// 官方文档原文如此：变量列作 <c>scene_info.total</c>（发起扣款请求的商户服务器设备号），
    /// 本 SDK 照抄官方原文承载。
    /// </para>
    /// </summary>
    [JsonPropertyName("scene_info")]
    public PayOrderTradeSceneInfo? SceneInfo { get; set; }

    /// <summary>
    /// 获取或设置优惠详情列表（官方可选，见 <see cref="PayOrderPromotionDetail"/>）。
    /// </summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayOrderPromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>
/// 查询订单响应的订单金额信息（<see cref="GetPayOrderResponse.Amount"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderTradeAmount
{
    /// <summary>
    /// 获取或设置订单总金额（官方可选，单位为分）。
    /// </summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>
    /// 获取或设置用户支付金额（官方可选，单位为分）。
    /// </summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>
    /// 获取或设置货币类型（官方可选）：境内商户号仅支持人民币 CNY。
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// 获取或设置用户支付币种（官方可选）。
    /// </summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }
}

/// <summary>
/// 查询订单响应的场景信息（<see cref="GetPayOrderResponse.SceneInfo"/>）。
/// <para>
/// 官方文档原文如此：仅一个字段且变量列作 <c>scene_info.total</c>，
/// 本 SDK 照抄官方原文承载。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderTradeSceneInfo
{
    /// <summary>
    /// 获取或设置发起扣款请求的商户服务器设备号（官方原文变量为 <c>scene_info.total</c>）。
    /// </summary>
    [JsonPropertyName("total")]
    public string? Total { get; set; }
}

/// <summary>
/// 查询订单响应的优惠详情（<see cref="GetPayOrderResponse.PromotionDetail"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayOrderPromotionDetail
{
    /// <summary>
    /// 获取或设置券 ID（官方可选，商家小票 ID）。
    /// </summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>
    /// 获取或设置优惠名称（官方可选，1~64 个字符）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置优惠范围（官方可选）：GLOBAL - 全场代金券、SINGLE - 单品优惠。
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// 获取或设置优惠类型（官方可选）：CASH - 充值型代金券、NOCASH - 免充值型代金券。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置优惠券面额（单位为分）。
    /// </summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>
    /// 获取或设置活动 ID（官方可选）。
    /// </summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>
    /// 获取或设置微信出资（官方可选，单位为分）。
    /// </summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatPayContribute { get; set; }

    /// <summary>
    /// 获取或设置商户出资（官方可选，单位为分）。
    /// </summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>
    /// 获取或设置其他出资（官方可选，单位为分）。
    /// </summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>
    /// 获取或设置优惠币种（官方可选）：CNY，境内商户号仅支持人民币。
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// 获取或设置单品优惠详情列表（见 <see cref="PayPromotionGoodsDetail"/>）。
    /// </summary>
    [JsonPropertyName("goods_detail")]
    public List<PayPromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>
/// 优惠单品信息（<see cref="PayOrderPromotionDetail.GoodsDetail"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayPromotionGoodsDetail
{
    /// <summary>
    /// 获取或设置商品编码（官方必填，1~32 个字符）。
    /// </summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

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

    /// <summary>
    /// 获取或设置商品优惠金额（官方必填，单位为分）。
    /// </summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>
    /// 获取或设置商品备注（官方可选，1~128 个字符）。
    /// </summary>
    [JsonPropertyName("goods_remark")]
    public string? GoodsRemark { get; set; }
}
