// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.FundsToOversea;

/// <summary>申请资金出境请求·【商品信息】必填字段，goods_info数量不能超过10个。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderCreateRequestGoodsInfoItem
{
    /// <summary>【商品名称】商品的名称（官方字段路径 <c>goods_info.goods_name</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>【商品类目】填写说明：商户自编的分类，可支持最短一级，最长二级的分类。如为二级类目，以/间隔（官方字段路径 <c>goods_info.goods_category</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_category")]
    public string? GoodsCategory { get; set; }

    /// <summary>【商品单价】商品单价，单位：分（官方字段路径 <c>goods_info.goods_unit_price</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("goods_unit_price")]
    public long? GoodsUnitPrice { get; set; }

    /// <summary>【商品数量】商品的数量（官方字段路径 <c>goods_info.goods_quantity</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("goods_quantity")]
    public long? GoodsQuantity { get; set; }

}

/// <summary>申请资金出境请求·【卖家信息】卖家信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderCreateRequestSellerInfo
{
    /// <summary>【境外卖家经营主体名称】境外卖家经营主体名称（官方字段路径 <c>seller_info.oversea_business_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("oversea_business_name")]
    public string? OverseaBusinessName { get; set; }

    /// <summary>【境外卖家店铺名称】境外卖家店铺名称（官方字段路径 <c>seller_info.oversea_shop_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("oversea_shop_name")]
    public string? OverseaShopName { get; set; }

    /// <summary>【卖家ID】商户系统内部的卖家ID（官方字段路径 <c>seller_info.seller_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("seller_id")]
    public string? SellerId { get; set; }

}

/// <summary>申请资金出境请求·【物流信息】物流信息 注意：仅在预售场景且定金订单需单独出境时，该字段可不填；其。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderCreateRequestExpressInfo
{
    /// <summary>【物流单号】物流单号（官方字段路径 <c>express_info.courier_number</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("courier_number")]
    public string? CourierNumber { get; set; }

    /// <summary>【物流商名称】物流商名称（官方字段路径 <c>express_info.express_company_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("express_company_name")]
    public string? ExpressCompanyName { get; set; }

}

/// <summary>申请资金出境请求·【收款人信息】收款人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderCreateRequestPayeeInfo
{
    /// <summary>【收款人识别号】在接入微信支付资金出境服务时，由微信支付分配的收款人识别号（官方字段路径 <c>payee_info.payee_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("payee_id")]
    public string? PayeeId { get; set; }

}

/// <summary>申请资金出境请求·【预售信息】预售信息 注意：定金与尾款对应的订单均需在三年（1096天）内，且定。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderCreateRequestPresaleInfo
{
    /// <summary>【订单类型】预售场景下订单类型包括定金订单和尾款订单。可选取值：`DEPOSIT` 定金订单 / `BALANCE` 尾款订单（官方字段路径 <c>presale_info.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【预售订单总金额】该笔支付单所售商品的人民币实际价值，单位为分，只能为整数，该信息将用于跨境合规检查，请准确填写（官方字段路径 <c>presale_info.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【关联定金订单号】尾款订单出境时（type为BALANCE），必须填写关联的定金订单的微信支付订单号（官方字段路径 <c>presale_info.deposit_transaction_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("deposit_transaction_id")]
    public string? DepositTransactionId { get; set; }

    /// <summary>【关联尾款订单号】定金订单出境时（type为DEPOSIT），需要填写关联的尾款订单的微信支付订单号。 注意：无物流信息，需要定金单独出境时，不必填写该参数（官方字段路径 <c>presale_info.balance_transaction_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("balance_transaction_id")]
    public string? BalanceTransactionId { get; set; }

}

/// <summary>申请资金出境请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderCreateRequest
{
    /// <summary>【商户出境单号】订单的主键，唯一定义此资源的标识，此参数只能由数字，大小写字母_-组成。 由商户在发起资金出境请求时生成，要求在同一个商户号下唯一（官方字段路径 <c>out_order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>【二级商户号】申请资金出境的二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信订单号】微信支付返回的支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【出境金额】需要出境的人民币金额，单位：分（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【境外收款币种】境外收款币种。 注：微信支付分配的收款人识别号属性下所关联的币种，会校验该币种字段与收款人识别号的相关性（官方字段路径 <c>foreign_currency</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("foreign_currency")]
    public string? ForeignCurrency { get; set; }

    /// <summary>【商品信息】必填字段，goods_info数量不能超过10个（官方字段路径 <c>goods_info</c>，选填，类型 <c>array[GoodsInfo]</c>）。</summary>
    [JsonPropertyName("goods_info")]
    public List<FundsToOverseaOrderCreateRequestGoodsInfoItem>? GoodsInfo { get; set; }

    /// <summary>【卖家信息】卖家信息（官方字段路径 <c>seller_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("seller_info")]
    public FundsToOverseaOrderCreateRequestSellerInfo? SellerInfo { get; set; }

    /// <summary>【物流信息】物流信息 注意：仅在预售场景且定金订单需单独出境时，该字段可不填；其他场景下，该字段必填（官方字段路径 <c>express_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("express_info")]
    public FundsToOverseaOrderCreateRequestExpressInfo? ExpressInfo { get; set; }

    /// <summary>【收款人信息】收款人信息（官方字段路径 <c>payee_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payee_info")]
    public FundsToOverseaOrderCreateRequestPayeeInfo? PayeeInfo { get; set; }

    /// <summary>【预售信息】预售信息 注意：定金与尾款对应的订单均需在三年（1096天）内，且定金支付人与尾款支付人需一致（官方字段路径 <c>presale_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("presale_info")]
    public FundsToOverseaOrderCreateRequestPresaleInfo? PresaleInfo { get; set; }

}

/// <summary>申请资金出境响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476113"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaOrderResponse : WechatPayResponse
{
    /// <summary>【商户出境单号】订单的主键，唯一定义此资源的标识，此参数只能由数字，大小写字母_-组成。 由商户在发起资金出境请求时生成，要求在同一个商户号下唯一（官方字段路径 <c>out_order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>【二级商户号】申请资金出境的二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信出境单号】微信出境单号（官方字段路径 <c>order_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>【出境结果】出境的结果， 枚举值： ACCEPT：已受理 SUCCESS：出境成功 FAIL：出境失败（官方字段路径 <c>result</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>【出境失败的原因】当result为FAIL时，会出现此字段，标明出境失败原因，如果是SYSTEM_ERROR可以重新发起重试 失败原因： MCHID_FROZEN：商户已冻结，转账失败 DEAL_TIMEOUT：单据已过期 TRADE_SUIT：交易订单被交易投诉冻结 DEPARTURE_AMOUNT_NO_ENOUGH：剩余可出境金额不足 BASIC_AMOUNT_NO_ENOUGH：商户基本户余额不足 PAYMENT_NOT_SUPPORT_DEPAR…（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【请求出境人民币金额】需要出境的人民币金额，单位：分（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【真实出境外币金额】真实出境的外币金额，单位：该币种最小计价单位，当result为SUCCESS时有这个字段（官方字段路径 <c>foreign_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("foreign_amount")]
    public long? ForeignAmount { get; set; }

    /// <summary>【外币币种】出境的目标币种，由商户在资金出境申请接口传入（官方字段路径 <c>foreign_currency</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("foreign_currency")]
    public string? ForeignCurrency { get; set; }

    /// <summary>【汇率】当result为SUCCESS时有这个字段，标价币种与支付币种的兑换比例乘以10的8次方即为此值，例如美元兑换人民币的比例为6.5，则rate=650000000（官方字段路径 <c>rate</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("rate")]
    public long? Rate { get; set; }

    /// <summary>【购汇时间】当result为SUCCESS时有这个字段。遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>exchange_rate_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("exchange_rate_time")]
    public string? ExchangeRateTime { get; set; }

    /// <summary>【预计购汇时间】当result为ACCEPT时可能有这个字段，以实际结果为准。 遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点…（官方字段路径 <c>estimate_exchange_rate_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("estimate_exchange_rate_time")]
    public string? EstimateExchangeRateTime { get; set; }

    /// <summary>【真实出境人民币金额】真正出境的人民币金额，单位：分，如果是二级商户承担手续费且非收支分离，该金额=请求出境金额-手续费，否则该金额=请求出境金额（官方字段路径 <c>departure_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("departure_amount")]
    public long? DepartureAmount { get; set; }

    /// <summary>【手续费人民币金额】资金出境手续费人民币金额，单位：分（官方字段路径 <c>fee</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("fee")]
    public long? Fee { get; set; }

    /// <summary>【手续费承担商户号】手续费承担商户号（官方字段路径 <c>charge_mchid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("charge_mchid")]
    public string? ChargeMchid { get; set; }

    /// <summary>【手续费承担账户】基本账户或者手续费账户。可选取值：`BASIC` 基本账户 / `FEES` 手续费账户（官方字段路径 <c>charge_account_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("charge_account_type")]
    public string? ChargeAccountType { get; set; }

}
