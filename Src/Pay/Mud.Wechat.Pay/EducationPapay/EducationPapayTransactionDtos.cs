// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationPapay;

/// <summary>教育代扣下单（受理）请求·【订单金额】 订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524731"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465548"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionCreateRequestAmount
{
    /// <summary>【总金额】 订单总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>教育代扣下单（受理）请求·【设备信息】 设备信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524731"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465548"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionCreateRequestDeviceInformation
{
    /// <summary>【设备号】 商户设备号（官方字段路径 <c>device_information.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备ip】 商户端设备IP（发起扣款请求的商户服务器ip）（官方字段路径 <c>device_information.device_ip</c>，<b>必填</b>，类型 <c>string(46)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>教育代扣下单（受理）请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524731"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465548"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionCreateRequest
{
    /// <summary>【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商品或支付单简要描述】 商品或支付单简要描述（官方字段路径 <c>body</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>【商户数据】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【商户订单号】 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-且在同一个商户号下唯一。重新发起一笔支付要使用原订单号，避免重复支付（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商品标记】 商品标记（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【支付回调通知地址】 异步接收微信支付结果通知的回调地址，通知url必须为外网可访问的url，不能携带参数。请使用https协议链接（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【场景信息枚举】 教育续费通 EDUCATION_PERIOD（官方字段路径 <c>trade_scene</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_scene")]
    public string? TradeScene { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public EducationPapayTransactionCreateRequestAmount? Amount { get; set; }

    /// <summary>【设备信息】 设备信息（官方字段路径 <c>device_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_information")]
    public EducationPapayTransactionCreateRequestDeviceInformation? DeviceInformation { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

}

/// <summary>教育代扣订单查询（微信订单号）响应·【支付者】 支付者。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523468"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465567"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionQueryResponsePayer
{
    /// <summary>【服务商APPID下的用户标识】 用户在服务商appid下的唯一标识（官方字段路径 <c>payer.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【子商户APPID下的用户标识】 用户在子商户appid下的唯一标识，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>payer.sub_openid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>教育代扣订单查询（微信订单号）响应·【订单金额】 订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523468"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465567"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionQueryResponseAmount
{
    /// <summary>【订单金额】 订单总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【用户支付金额】 用户实际支付金额，单位为分，只能为整数（官方字段路径 <c>amount.payer_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>【折扣】 订单折扣（官方字段路径 <c>amount.discount_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_total")]
    public long? DiscountTotal { get; set; }

    /// <summary>【货币类型】 符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>教育代扣订单查询（微信订单号）响应·【设备信息】 设备信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523468"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465567"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionQueryResponseDeviceInformation
{
    /// <summary>【设备号】 商户设备号（官方字段路径 <c>device_information.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备ip】 商户端设备IP（发起扣款请求的商户服务器ip）（官方字段路径 <c>device_information.device_ip</c>，<b>必填</b>，类型 <c>string(46)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>教育代扣订单查询（微信订单号）响应·【优惠详情】 优惠详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523468"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465567"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionQueryResponsePromotionDetailItem
{
    /// <summary>【券ID】 券ID（官方字段路径 <c>promotion_detail.coupon_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>【优惠名称】 优惠名称（官方字段路径 <c>promotion_detail.name</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【优惠范围】 GLOBAL：全场代金券；SINGLE：单品优惠（官方字段路径 <c>promotion_detail.scope</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【优惠类型】 COUPON-代金券，需要走结算资金的充值型代金券,（境外商户券币种与支付币种一致），DISCOUNT-优惠券，不走结算资金的免充值型优惠券，（境外商户券币种与标价币种一致）（官方字段路径 <c>promotion_detail.type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【优惠券面额】 优惠券面额（官方字段路径 <c>promotion_detail.amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【活动ID】 活动ID，批次ID（官方字段路径 <c>promotion_detail.stock_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【微信出资】 特指由微信支付商户平台创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.wechatpay_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>【商户出资】 特指商户自己创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.merchant_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>【其他出资】 其他出资方出资金额，单位为分（官方字段路径 <c>promotion_detail.other_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

}

/// <summary>教育代扣订单查询（微信订单号）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012523468"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465567"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayTransactionQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户订单号】 商户订单号（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【微信支付订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户数据】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【付款银行】 银行类型，采用字符串类型的银行标识，值列表详见 银行类型（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【支付完成时间】 支付成功时返回，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【交易状态】 枚举值：SUCCESS—支付成功，REFUND—转入退款，ACCEPTED—已受理，未支付，PAY_FAIL--支付失败(其他原因，如银行返回失败)（官方字段路径 <c>trade_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>【交易状态描述】 对当前订单状态的描述和下一步操作的指引（官方字段路径 <c>trade_state_description</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("trade_state_description")]
    public string? TradeStateDescription { get; set; }

    /// <summary>【支付者】 支付者（官方字段路径 <c>payer</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payer")]
    public EducationPapayTransactionQueryResponsePayer? Payer { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public EducationPapayTransactionQueryResponseAmount? Amount { get; set; }

    /// <summary>【设备信息】 设备信息（官方字段路径 <c>device_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_information")]
    public EducationPapayTransactionQueryResponseDeviceInformation? DeviceInformation { get; set; }

    /// <summary>【优惠详情】 优惠详情（官方字段路径 <c>promotion_detail</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<EducationPapayTransactionQueryResponsePromotionDetailItem>? PromotionDetail { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，该字段返回为空（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

}
