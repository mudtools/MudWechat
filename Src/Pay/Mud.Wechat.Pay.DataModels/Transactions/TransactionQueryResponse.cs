// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 查询订单应答（微信支付 APIv3；「微信支付订单号查询」与「商户订单号查询」<b>共用同一应答体</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：微信支付订单号查询 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791899"/>；
/// 商户订单号查询 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791900"/>（2026-10-09 逐字段核验）。
/// </para>
/// <para>
/// <b>两路由共享本 DTO 的依据</b>：官方对两个查询接口给出<b>完全一致的应答字段表</b>
/// （仅请求参数不同：一个走 path 的 <c>transaction_id</c>、一个走 path 的 <c>out_trade_no</c>）。
/// 复用同一类型可避免两份必然漂移的重复定义。
/// </para>
/// <para>
/// <b>字段名照官方原文</b>（如 <c>trade_state_desc</c> / <c>payer_total</c>），
/// 契约守卫断言以官方为准，<b>不得「纠正」</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class TransactionQueryResponse : WechatPayResponse
{
    /// <summary>公众账号 ID（<c>appid</c>，string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 交易类型（<c>trade_type</c>，string(16)）：<c>JSAPI</c> / <c>NATIVE</c> / <c>APP</c> /
    /// <c>MICROPAY</c> / <c>MWEB</c> / <c>FACEPAY</c>。
    /// </summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>
    /// 交易状态（<c>trade_state</c>，string(32)）：<c>SUCCESS</c> / <c>REFUND</c> / <c>NOTPAY</c> /
    /// <c>CLOSED</c> / <c>REVOKED</c>（已撤销，付款码支付专用）/ <c>USERPAYING</c>（用户支付中，付款码支付专用）/
    /// <c>PAYERROR</c>（支付失败）。
    /// </summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>交易状态描述（<c>trade_state_desc</c>，string(256)）。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，string(32)），银行类型采用字符串枚举。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>商户数据包（<c>attach</c>，string(128)），下单时上送、原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，string(64)，rfc3339）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>支付者信息（<c>payer</c>），见 <see cref="TransactionPayerInfo"/>。</summary>
    [JsonPropertyName("payer")]
    public TransactionPayerInfo? Payer { get; set; }

    /// <summary>订单金额（<c>amount</c>），见 <see cref="TransactionAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public TransactionAmountInfo? Amount { get; set; }

    /// <summary>场景信息（<c>scene_info</c>），见 <see cref="TransactionSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public TransactionSceneInfo? SceneInfo { get; set; }

    /// <summary>优惠功能（<c>promotion_detail</c>，数组），见 <see cref="TransactionPromotionDetail"/>。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<TransactionPromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>查询订单应答的支付者信息（<c>payer</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class TransactionPayerInfo
{
    /// <summary>用户在商户 <c>appid</c> 下的唯一标识（<c>openid</c>，string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>查询订单应答的订单金额（<c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class TransactionAmountInfo
{
    /// <summary>订单总金额（<c>total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>用户支付金额（<c>payer_total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>货币类型（<c>currency</c>，string(16)），固定 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>用户支付币种（<c>payer_currency</c>，string(16)）。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }
}

/// <summary>查询订单应答的场景信息（<c>scene_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class TransactionSceneInfo
{
    /// <summary>商户端设备号（<c>device_id</c>，string(32)）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }
}

/// <summary>查询订单应答的优惠明细（<c>promotion_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class TransactionPromotionDetail
{
    /// <summary>券 ID（<c>coupon_id</c>，string(32)）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>优惠名称（<c>name</c>，string(64)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠范围（<c>scope</c>，string(32)）：<c>GLOBAL</c>（全场代金券）/ <c>SINGLE</c>（单品优惠）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>，string(32)）：<c>CASH</c>（充值代金券）/ <c>NOCASH</c>（非充值优惠券）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>优惠券面额（<c>amount</c>，整型，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>活动 ID（<c>stock_id</c>，string(32)）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>微信出资（<c>wechatpay_contribute</c>，整型，单位为分）。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>商户出资（<c>merchant_contribute</c>，整型，单位为分）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>其他出资（<c>other_contribute</c>，整型，单位为分）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>优惠币种（<c>currency</c>，string(16)），固定 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>单品优惠信息（<c>goods_detail</c>，数组），见 <see cref="TransactionPromotionGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<TransactionPromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>查询订单应答的单品优惠明细（<c>promotion_detail[].goods_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class TransactionPromotionGoodsDetail
{
    /// <summary>商品编码（<c>goods_id</c>，string(32)）。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>商品数量（<c>quantity</c>，整型）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>商品单价（<c>unit_price</c>，整型，单位为分）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>商品优惠金额（<c>discount_amount</c>，整型，单位为分）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>商品备注（<c>goods_remark</c>，string(128)）。</summary>
    [JsonPropertyName("goods_remark")]
    public string? GoodsRemark { get; set; }
}
