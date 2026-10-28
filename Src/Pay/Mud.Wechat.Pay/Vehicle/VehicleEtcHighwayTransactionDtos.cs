// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Vehicle;

/// <summary>ETC高速场景订单扣款下单请求·【高速场景信息】 高速场景信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionRequestHighwayInfo
{
    /// <summary>【车牌号】 车牌号。仅包括省份+车牌，不包括特殊字符（官方字段路径 <c>highway_info.plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【入站时间】 用户入闸时间，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>highway_info.start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【出站时间】 用户出闸时间，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>highway_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【通道类型】 高速通道类型，目前可选：ETC、MTC。商户扣费前必须确认当前车牌的标识属性，用户车牌必须具有该通道标识时，才允许扣费（官方字段路径 <c>highway_info.channel_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; set; }

    /// <summary>【车辆类型】 车辆的类型，可选值： CAR ：客车 TRUCK：货车（官方字段路径 <c>highway_info.car_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("car_type")]
    public string? CarType { get; set; }

    /// <summary>【入口名称】 高速收费入口站的名称，需提前提供进行录入（官方字段路径 <c>highway_info.entrance</c>，<b>必填</b>，类型 <c>string(96)</c>）。</summary>
    [JsonPropertyName("entrance")]
    public string? Entrance { get; set; }

    /// <summary>【出口名称】 高速收费出口站的名称，需提前提供进行录入（官方字段路径 <c>highway_info.exit</c>，选填，类型 <c>string(96)</c>）。</summary>
    [JsonPropertyName("exit")]
    public string? Exit { get; set; }

    /// <summary>【承载人数】 当前车辆核载人数，纯数字（官方字段路径 <c>highway_info.carrying_capacity</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("carrying_capacity")]
    public long? CarryingCapacity { get; set; }

    /// <summary>【荷载范围】 当前车辆核载人数区间，格式数字-数字（官方字段路径 <c>highway_info.carrying_range</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("carrying_range")]
    public string? CarryingRange { get; set; }

}

/// <summary>ETC高速场景订单扣款下单请求·【订单金额】 订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionRequestAmount
{
    /// <summary>【订单金额】 订单总金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>ETC高速场景订单扣款下单请求·【设备信息】 设备信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionRequestDeviceInfo
{
    /// <summary>【商户端设备号】 商户端设备号（官方字段路径 <c>device_info.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备IP】 商户端设备IP（官方字段路径 <c>device_info.device_ip</c>，<b>必填</b>，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>ETC高速场景订单扣款下单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionRequest
{
    /// <summary>【商户订单号】 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-且在同一个商户号下唯一。重新发起一笔支付要使用原订单号，避免重复支付（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【特约商户号】特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户appid】商户申请的公众号AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【特约商户appid】微信支付分配的公众账号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【微信侧ETC绑定号】 一串随机的数字、a-z、A-Z的 32位长度字符串（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【商品信息】 商品信息（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【附加信息】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【营销信息】 商品标记，代金券或立减优惠功能的参数，说明详见代金券或立减优惠（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【高速场景信息】 高速场景信息（官方字段路径 <c>highway_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("highway_info")]
    public VehicleEtcHighwayTransactionRequestHighwayInfo? HighwayInfo { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public VehicleEtcHighwayTransactionRequestAmount? Amount { get; set; }

    /// <summary>【设备信息】 设备信息（官方字段路径 <c>device_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_info")]
    public VehicleEtcHighwayTransactionRequestDeviceInfo? DeviceInfo { get; set; }

}

/// <summary>ETC高速场景订单扣款下单响应·【支付者】 支付者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionResponsePayer
{
    /// <summary>【用户在服务商app的标识】 用户在对应授权appid下用户唯一标识（官方字段路径 <c>payer.sp_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【用户在特约商户app的标识】用户在特约商户的唯一标识（官方字段路径 <c>payer.sub_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>ETC高速场景订单扣款下单响应·【订单金额】 订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionResponseAmount
{
    /// <summary>【订单金额】 订单总金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>【用户支付金额】 用户实际支付金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.payer_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>【折扣】订单折扣百分比，例如：86为八六折（官方字段路径 <c>amount.discount_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_total")]
    public long? DiscountTotal { get; set; }

}

/// <summary>ETC高速场景订单扣款下单响应·【设备信息】 设备信息对象。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionResponseDeviceInfo
{
    /// <summary>【商户端设备号】 商户端设备号（官方字段路径 <c>device_info.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备IP】 商户端设备IP（官方字段路径 <c>device_info.device_ip</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>ETC高速场景订单扣款下单响应·【优惠详情】 优惠功能信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionResponsePromotionDetailItem
{
    /// <summary>【券ID】 券或者立减优惠id（官方字段路径 <c>promotion_detail.promotion_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("promotion_id")]
    public string? PromotionId { get; set; }

    /// <summary>【优惠名称】 优惠名称（官方字段路径 <c>promotion_detail.name</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【优惠范围】 GLOBAL-全场代金券 SINGLE-单品优惠（官方字段路径 <c>promotion_detail.scope</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【优惠类型】枚举值： COUPON-代金券，需要走结算资金的充值型代金券,（境外商户券币种与支付币种一致），DISCOUNT-优惠券，不走结算资金的免充值型优惠券，（境外商户券币种与标价币种一致）（官方字段路径 <c>promotion_detail.type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【活动ID】 在微信商户后台配置的批次ID（官方字段路径 <c>promotion_detail.activity_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【优惠券面额】 用户享受优惠的金额（官方字段路径 <c>promotion_detail.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【微信出资】 特指由微信支付商户平台创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.wxpay_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wxpay_contribute")]
    public long? WxpayContribute { get; set; }

    /// <summary>【商户出资】 特指商户自己创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.merchant_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>【其他出资】 其他出资方出资金额，单位为分（官方字段路径 <c>promotion_detail.other_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

}

/// <summary>ETC高速场景订单扣款下单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715331"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcHighwayTransactionResponse : WechatPayResponse
{
    /// <summary>【商户号】商户号，由微信支付生成并下发（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户号】特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户appid】商户申请的公众号AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【特约商户appid】微信支付分配的公众账号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商户订单号】 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-且在同一个商户号下唯一。重新发起一笔支付要使用原订单号，避免重复支付（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【微信支付订单号】微信支付系统生成的订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户数据】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【交易类型】 枚举值： PAP 委托代扣支付（官方字段路径 <c>trade_type</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>【付款银行】 银行类型，采用字符串类型的银行标识，值列表详见 银行类型（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【支付完成时间】 支付成功时返回，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【交易状态】 订单支付状态，可选值：SUCCESS—支付成功，REFUND—转入退款，ACCEPTED—已受理，未支付，PAY_FAIL--支付失败(其他原因，如银行返回失败)（官方字段路径 <c>trade_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>【交易状态描述】 对当前订单状态的描述和下一步操作的指引（官方字段路径 <c>trade_state_desc</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>【支付者】 支付者信息（官方字段路径 <c>payer</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payer")]
    public VehicleEtcHighwayTransactionResponsePayer? Payer { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public VehicleEtcHighwayTransactionResponseAmount? Amount { get; set; }

    /// <summary>【设备信息】 设备信息对象（官方字段路径 <c>device_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_info")]
    public VehicleEtcHighwayTransactionResponseDeviceInfo? DeviceInfo { get; set; }

    /// <summary>【优惠详情】 优惠功能信息（官方字段路径 <c>promotion_detail</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<VehicleEtcHighwayTransactionResponsePromotionDetailItem>? PromotionDetail { get; set; }

}

/// <summary>ETC订单查询（微信订单号）响应·【支付者】 支付者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715719"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcTransactionResponsePayer
{
    /// <summary>【用户在服务商app的标识】 用户在对应授权appid下用户唯一标识（官方字段路径 <c>payer.sp_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【用户在特约商户app的标识】用户在特约商户的唯一标识（官方字段路径 <c>payer.sub_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>ETC订单查询（微信订单号）响应·【订单金额】 订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715719"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcTransactionResponseAmount
{
    /// <summary>【订单金额】 订单总金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>【用户支付金额】 用户实际支付金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.payer_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>【折扣】订单折扣百分比，例如：86为八六折（官方字段路径 <c>amount.discount_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_total")]
    public long? DiscountTotal { get; set; }

}

/// <summary>ETC订单查询（微信订单号）响应·【设备信息】 设备信息对象。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715719"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcTransactionResponseDeviceInfo
{
    /// <summary>【商户端设备号】 商户端设备号（官方字段路径 <c>device_info.device_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>【商户端设备IP】 商户端设备IP（官方字段路径 <c>device_info.device_ip</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("device_ip")]
    public string? DeviceIp { get; set; }

}

/// <summary>ETC订单查询（微信订单号）响应·【优惠详情】 优惠功能信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715719"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcTransactionResponsePromotionDetailItem
{
    /// <summary>【券ID】 券或者立减优惠id（官方字段路径 <c>promotion_detail.promotion_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("promotion_id")]
    public string? PromotionId { get; set; }

    /// <summary>【优惠名称】 优惠名称（官方字段路径 <c>promotion_detail.name</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【优惠范围】枚举值： GLOBAL：全场代金 SINGLE：单品优惠（官方字段路径 <c>promotion_detail.scope</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【优惠类型】枚举值： COUPON-代金券，需要走结算资金的充值型代金券,（境外商户券币种与支付币种一致） DISCOUNT-优惠券，不走结算资金的免充值型优惠券，（境外商户券币种与标价币种一致）（官方字段路径 <c>promotion_detail.type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【活动ID】 在微信商户后台配置的批次ID（官方字段路径 <c>promotion_detail.activity_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【优惠券面额】 用户享受优惠的金额（官方字段路径 <c>promotion_detail.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【微信出资】 特指由微信支付商户平台创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.wxpay_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wxpay_contribute")]
    public long? WxpayContribute { get; set; }

    /// <summary>【商户出资】 特指商户自己创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.merchant_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>【其他出资】 其他出资方出资金额，单位为分（官方字段路径 <c>promotion_detail.other_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

}

/// <summary>ETC订单查询（微信订单号）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715719"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcTransactionResponse : WechatPayResponse
{
    /// <summary>【商户号】 商户号，由微信支付生成并下发（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户号】 特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户appid】商户申请的公众号AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【特约商户appid】微信支付分配的公众账号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商户订单号】 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-且在同一个商户号下唯一。重新发起一笔支付要使用原订单号，避免重复支付（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【微信支付订单号】微信支付系统生成的订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【商户数据】 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据（官方字段路径 <c>attach</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【交易类型】 枚举值： PAP 委托代扣支付（官方字段路径 <c>trade_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>【付款银行】 银行类型，采用字符串类型的银行标识，值列表详见 银行类型（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【支付完成时间】 支付成功时返回，按照使用 rfc3339 所定义的格式，格式为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>success_time</c>，选填，类型 <c>string(25)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【交易状态】 订单支付状态，枚举值：SUCCESS—支付成功，REFUND—转入退款，ACCEPTED—已受理，未支付，PAY_FAIL--支付失败(其他原因，如银行返回失败)（官方字段路径 <c>trade_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>【交易状态描述】 对当前订单状态的描述和下一步操作的指引（官方字段路径 <c>trade_state_desc</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>【支付者】 支付者信息（官方字段路径 <c>payer</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payer")]
    public VehicleEtcTransactionResponsePayer? Payer { get; set; }

    /// <summary>【订单金额】 订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public VehicleEtcTransactionResponseAmount? Amount { get; set; }

    /// <summary>【设备信息】 设备信息对象（官方字段路径 <c>device_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("device_info")]
    public VehicleEtcTransactionResponseDeviceInfo? DeviceInfo { get; set; }

    /// <summary>【优惠详情】 优惠功能信息（官方字段路径 <c>promotion_detail</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<VehicleEtcTransactionResponsePromotionDetailItem>? PromotionDetail { get; set; }

}
