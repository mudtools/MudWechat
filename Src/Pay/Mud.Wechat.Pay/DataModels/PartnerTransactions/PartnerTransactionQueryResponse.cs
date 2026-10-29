// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商查询订单应答（微信支付 APIv3；「微信支付订单号查询」与「商户订单号查询」<b>共用同一应答体</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：订单号查询 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012738964"/>、
/// 商户订单号查询 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012739008"/>（2026-10-14 逐字段核验；
/// 两路由各有 6 个产品族分身页，应答表逐页一致）。</para>
/// <para><b>两路由共享本 DTO 的依据</b>：官方对两个查询接口给出<b>完全一致的应答字段表</b>，
/// 仅查询键不同（path 的 <c>transaction_id</c> 与 <c>out_trade_no</c>）⇒ 按「表相同则共用」不建第二份。</para>
/// <para><b>服务商形态与直连域的三处可见差异</b>：① 商户标识是 <c>sp_appid</c>/<c>sp_mchid</c>/
/// <c>sub_appid</c>/<c>sub_mchid</c> 四件套（直连只有 <c>appid</c>/<c>mchid</c>）；
/// ② <c>payer</c> 是 <c>sp_openid</c> + <c>sub_openid</c> 双标识；③ 应答<b>始终带 <c>scene_info</c></b>
/// （直连查单应答里该节为选填）。字段名照官方原文，契约守卫 PT-B3 断言以官方为准，<b>不得「纠正」</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerTransactionQueryResponse : WechatPayResponse
{
    /// <summary>服务商 APPID（<c>sp_appid</c>，string(32)）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>服务商户号（<c>sp_mchid</c>，string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，string(32)），下单未传则缺省。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 交易类型（<c>trade_type</c>，string(16)）：取值见 <see cref="PartnerTradeTypes"/>
    /// （<c>JSAPI</c> / <c>NATIVE</c> / <c>APP</c> / <c>MICROPAY</c> / <c>MWEB</c> / <c>FACEPAY</c>）。
    /// </summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>
    /// 交易状态（<c>trade_state</c>，string(32)）：取值见 <see cref="PartnerTradeStates"/>。
    /// 判定支付成功须以本字段为准，<b>查单不可代替支付结果通知</b>。
    /// </summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>交易状态描述（<c>trade_state_desc</c>，string(256)）。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，string(32)），银行类型采用字符串枚举。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>附加数据（<c>attach</c>，string(128)），下单时上送、原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，string(64)，rfc3339）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>支付者信息（<c>payer</c>），见 <see cref="PartnerPayerInfo"/>（<c>sp_openid</c> / <c>sub_openid</c>）。</summary>
    [JsonPropertyName("payer")]
    public PartnerPayerInfo? Payer { get; set; }

    /// <summary>订单金额（<c>amount</c>），见 <see cref="PartnerQueryAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public PartnerQueryAmountInfo? Amount { get; set; }

    /// <summary>场景信息（<c>scene_info</c>），见 <see cref="PartnerQuerySceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public PartnerQuerySceneInfo? SceneInfo { get; set; }

    /// <summary>优惠功能（<c>promotion_detail</c>，数组），见 <see cref="PartnerPromotionDetail"/>。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<PartnerPromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>服务商查单应答的订单金额（<c>amount</c>）：比下单请求多 <c>payer_total</c>/<c>payer_currency</c>。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerQueryAmountInfo
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

/// <summary>服务商查单应答的场景信息（<c>scene_info</c>）：官方本域只有 <c>device_id</c>。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerQuerySceneInfo
{
    /// <summary>商户端设备号（<c>device_id</c>，string(32)）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }
}

/// <summary>服务商查单应答的优惠明细（<c>promotion_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerPromotionDetail
{
    /// <summary>券 ID（<c>coupon_id</c>，string(32)）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>优惠名称（<c>name</c>，string(64)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠范围（<c>scope</c>，string(32)）：取值见 <see cref="PartnerPromotionScopes"/>。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>，string(32)）：取值见 <see cref="PartnerPromotionTypes"/>。</summary>
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

    /// <summary>单品优惠信息（<c>goods_detail</c>，数组），见 <see cref="PartnerPromotionGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<PartnerPromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>服务商查单应答的单品优惠明细（<c>promotion_detail[].goods_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerPromotionGoodsDetail
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
