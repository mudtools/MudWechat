// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationSchoolPay;

/// <summary>校园缴费订单查询（微信订单号）响应·【支付者】 支付者。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469063"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionQueryResponsePayer
{
    /// <summary>【商户AppID下的用户标识】 用户在商户AppID下的唯一标识。 如果是服务商模式接入则为服务商商户AppID下的唯一标识；如果是直连商户模式接入则为直连商户AppID下的唯一标识（官方字段路径 <c>payer.openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【子商户AppID下的用户标识】 用户在子商户AppID下的唯一标识。如果是服务商模式接入则有值；如果是直连商户模式接入则该字段为空（官方字段路径 <c>payer.sub_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>校园缴费订单查询（微信订单号）响应·【订单金额】 订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469063"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionQueryResponseAmount
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

    /// <summary>【货币类型】 符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(3)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>校园缴费订单查询（微信订单号）响应·【设备信息】 设备信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469063"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionQueryResponseDeviceInfo
{
    /// <summary>【设备号】 商户设备号，最大长度32个字符（官方字段路径 <c>device_info.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备IP】 商户端设备IP（发起扣款请求的商户服务器IP），支持IPv6（官方字段路径 <c>device_info.device_ip</c>，<b>必填</b>，类型 <c>string(46)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>校园缴费订单查询（微信订单号）响应·【优惠详情】 优惠详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469063"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionQueryResponsePromotionDetailItem
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

/// <summary>校园缴费订单查询（微信订单号）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469063"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionQueryResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号。如果是服务商模式接入则为服务商商户号；如果是直连商户模式接入则为直连商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户AppID】 微信公众平台（mp.weixin.qq.com）为服务商商户分配的公众账号ID，与商户号存在绑定关系。 如果是服务商模式接入则为服务商商户号绑定的AppID；如果是直连商户模式接入则为直连商户号绑定的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系。如果是服务商模式接入则有值；如果是直连商户模式接入则该字段为空（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与子商户号存在绑定关系。如果是服务商模式接入则有值；如果是直连商户模式接入则该字段为空（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商户订单号】 商户自行生成的订单号，要求32个字符内，只能是数字、大小写字母和 `_-`，且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【微信支付订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【交易类型】 交易类型，取值如下：JSAPI，NATIVE，APP，MICROPAY，MWEB，FACEPAY（官方字段路径 <c>trade_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>【交易状态】 本支付订单的状态，可选枚举值：SUCCESS—支付成功;REFUND—转入退款;ACCEPTED—已受理，未支付;PAY_FAIL-支付失败(其他原因，如银行返回失败);PAY_BACK-用户归还欠款（仅当本单由微信支付预先垫付资金，即 `bank_type=BPA` 时有可能发生）（官方字段路径 <c>trade_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>【交易状态描述】 对当前订单状态的描述和下一步操作的指引（官方字段路径 <c>trade_state_desc</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>【付款银行】 银行类型，采用字符串类型的银行标识，值列表详见 银行类型。特别地，使用 BPA 表示本笔订单由微信支付预先垫付资金（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【商户数据】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【支付完成时间】 支付订单已成功才会返回，使用 rfc3339 所定义的格式，即为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【支付者】 支付者（官方字段路径 <c>payer</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payer")]
    public EducationSchoolPayTransactionQueryResponsePayer? Payer { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public EducationSchoolPayTransactionQueryResponseAmount? Amount { get; set; }

    /// <summary>【设备信息】 设备信息（官方字段路径 <c>device_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_info")]
    public EducationSchoolPayTransactionQueryResponseDeviceInfo? DeviceInfo { get; set; }

    /// <summary>【优惠详情】 优惠详情（官方字段路径 <c>promotion_detail</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<EducationSchoolPayTransactionQueryResponsePromotionDetailItem>? PromotionDetail { get; set; }

}

/// <summary>校园缴费下单请求·【场景信息】 本次交易所涉及的具体场景信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469030"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionCreateRequestSceneInfo
{
    /// <summary>【消费时间】 本次消费的发生时间，使用 rfc3339 所定义的格式，即为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>scene_info.start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【学校编号】 当前消费所属学校在校园轻松付产品中的编号。服务商在商户平台接入校园轻松付后，可以申请录入学校信息，申请成功后由微信支付生成学校编号。 微信支付会校验扣费合约 `contract_no` 是否属于该学校，校验不匹配将拒绝受理扣款（官方字段路径 <c>scene_info.school_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("school_id")]
    public string? SchoolId { get; set; }

    /// <summary>【消费场景类型】 本次消费所属的场景类型，应与商户在商户平台(pay.weixin.qq.com)中申请校园轻松付产品权限时录入的经营范围相匹配。可选取值：`CANTEEN` 食堂 / `SHOP` 学校超市或小卖部 / `INFIRMARY` 校医院 / `TRAFFIC` 校内交通 / `OTHERS` 校内其他消费场景 / `LARGE` 大额场景（官方字段路径 <c>scene_info.scene_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scene_type")]
    public string? SceneType { get; set; }

}

/// <summary>校园缴费下单请求·【设备信息】 设备信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469030"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionCreateRequestDeviceInfo
{
    /// <summary>【设备号】 商户设备号，最大长度32个字符（官方字段路径 <c>device_info.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备IP】 商户端设备IP（发起扣款请求的商户服务器IP），支持IPv6（官方字段路径 <c>device_info.device_ip</c>，<b>必填</b>，类型 <c>string(46)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>校园缴费下单请求·【分账结算信息】 分账结算相关信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469030"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionCreateRequestSettleInfo
{
    /// <summary>【是否指定分账】 是否指定分账，默认false（官方字段路径 <c>settle_info.profit_sharing</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }

}

/// <summary>校园缴费下单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469030"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayTransactionCreateRequest
{
    /// <summary>【商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与商户号存在绑定关系。 如果是服务商模式接入则为服务商商户号绑定的AppID；如果是直连商户模式接入则为直连商户号绑定的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系。如果是服务商模式接入则必填；如果是直连商户模式接入则不用填写（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与子商户号存在绑定关系。如果是服务商模式接入则必填；如果是直连商户模式接入则不用填写（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商品描述】 商品描述，格式为：学校-消费地点-销售商品。最长127个字符（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【商户数据】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据，最长127个字符（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【商户订单号】 商户自行生成的订单号，要求32个字符内，只能是数字、大小写字母和 `_-`，且在同一个商户号下唯一。重新发起一笔支付要使用原订单号，避免重复支付（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【订单优惠标记】 用于代金券核销或立减优惠功能，需携带与代金券或立减优惠中相同的goods_tag方可核销优惠（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【用户账号】 进行当前消费的学生的用户账号，如学生学号。必须与签署当前合约(由参数 `contract_id` 标识)时使用的用户账号一致。 微信支付将对该参数与合约中的 `user_id` 进行对比验证，二者不一致时拒绝受理扣款（官方字段路径 <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>ReqAmountInfo</c>）。</summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    /// <summary>【场景信息】 本次交易所涉及的具体场景信息（官方字段路径 <c>scene_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scene_info")]
    public EducationSchoolPayTransactionCreateRequestSceneInfo? SceneInfo { get; set; }

    /// <summary>【设备信息】 设备信息（官方字段路径 <c>device_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_info")]
    public EducationSchoolPayTransactionCreateRequestDeviceInfo? DeviceInfo { get; set; }

    /// <summary>【分账结算信息】 分账结算相关信息（官方字段路径 <c>settle_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("settle_info")]
    public EducationSchoolPayTransactionCreateRequestSettleInfo? SettleInfo { get; set; }

}
