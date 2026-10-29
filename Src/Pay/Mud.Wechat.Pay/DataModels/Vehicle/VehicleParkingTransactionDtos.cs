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

/// <summary>停车订单查询（商户订单号）响应·【停车场景信息】返回信息中的trade_scene为PARKING，返回该场景信。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534308"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534441"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionQueryResponseParkingInfo
{
    /// <summary>【入场ID】微信支付分停车服务为商户分配的入场ID，商户通过入场通知接口获取入场ID（官方字段路径 <c>parking_info.parking_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("parking_id")]
    public string? ParkingId { get; set; }

    /// <summary>【车牌号】车牌号，仅包括省份+车牌，不包括特殊字符（官方字段路径 <c>parking_info.plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【车牌颜色】车牌颜色。可选取值：`BLUE` 蓝色 / `GREEN` 绿色 / `YELLOW` 黄色 / `BLACK` 黑色 / `WHITE` 白色 / `LIMEGREEN` 黄绿色（官方字段路径 <c>parking_info.plate_color</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plate_color")]
    public string? PlateColor { get; set; }

    /// <summary>【入场时间】用户入场时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>parking_info.start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【出场时间】用户出场时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>parking_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【停车场名称】所在停车位车场的名称（官方字段路径 <c>parking_info.parking_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("parking_name")]
    public string? ParkingName { get; set; }

    /// <summary>【计费时长】计费的时间长，单位为秒（官方字段路径 <c>parking_info.charging_duration</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("charging_duration")]
    public long? ChargingDuration { get; set; }

    /// <summary>【停车场设备ID】停车场设备ID（官方字段路径 <c>parking_info.device_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

}

/// <summary>停车订单查询（商户订单号）响应·【支付者信息】支付者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534308"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534441"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionQueryResponsePayer
{
    /// <summary>【用户在AppID下的标识】用户在AppID下的唯一标识（官方字段路径 <c>payer.openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户在Sub_AppID下的标识】用户在Sub_AppID下的标识，商户扣费时传入了Sub_AppID，则会返回该用户在Sub_AppID下的标识（官方字段路径 <c>payer.sub_openid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>停车订单查询（商户订单号）响应·【订单金额信息】订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534308"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534441"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionQueryResponseAmount
{
    /// <summary>【订单金额】订单总金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>【用户实际支付金额】用户实际支付金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.payer_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>【折扣】订单折扣（官方字段路径 <c>amount.discount_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_total")]
    public long? DiscountTotal { get; set; }

}

/// <summary>停车订单查询（商户订单号）响应·【优惠信息】优惠信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534308"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534441"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionQueryResponsePromotionDetailItem
{
    /// <summary>【券ID】券或者立减优惠ID（官方字段路径 <c>promotion_detail.coupon_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>【优惠名称】优惠名称（官方字段路径 <c>promotion_detail.name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【优惠范围】GLOBAL-全场代金券， SINGLE-单品优惠（官方字段路径 <c>promotion_detail.scope</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【优惠类型】枚举值： CASH：充值型代金券 NOCASH：免充值型代金券（官方字段路径 <c>promotion_detail.type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【活动ID】在微信商户后台配置的批次ID（官方字段路径 <c>promotion_detail.stock_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【优惠券面额】用户享受优惠的金额（官方字段路径 <c>promotion_detail.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【微信出资】特指由微信支付商户平台创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.wechatpay_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>【商户出资】特指商户自己创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.merchant_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>【其他出资】其他出资方出资金额，单位为分（官方字段路径 <c>promotion_detail.other_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>【优惠币种】CNY：人民币，境内商户号仅支持人民币（官方字段路径 <c>promotion_detail.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>停车订单查询（商户订单号）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534308"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534441"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionQueryResponse : WechatPayResponse
{
    /// <summary>【公众账号ID】AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【服务描述】商户自定义字段，用于交易账单中对扣费服务的描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【订单创建时间】订单成功创建时返回，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【商户订单号】商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【微信支付订单号】微信支付订单号（官方字段路径 <c>transaction_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【交易状态】SUCCESS—支付成功 ACCEPTED—已接收，等待扣款 PAY_FAIL–支付失败(其他原因，如银行返回失败) REFUND—转入退款（官方字段路径 <c>trade_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>【交易状态描述】对当前订单状态的描述和下一步操作的指引（官方字段路径 <c>trade_state_description</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("trade_state_description")]
    public string? TradeStateDescription { get; set; }

    /// <summary>【支付完成时间】订单支付完成时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【付款银行】银行类型，采用字符串类型的银行标识。BPA：该笔订单由微信进行垫付（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【用户是否已还款】枚举值： Y：用户已还款 N：用户未还款 注意：使用此字段前需先确认bank_type字段值为BPA以及 trade_state字段值为SUCCESS（官方字段路径 <c>user_repaid</c>，选填，类型 <c>string(1)</c>）。</summary>
    [JsonPropertyName("user_repaid")]
    public string? UserRepaid { get; set; }

    /// <summary>【附加数据】附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【交易场景】交易场景值，目前支持 ：PARKING：车场停车场景（官方字段路径 <c>trade_scene</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_scene")]
    public string? TradeScene { get; set; }

    /// <summary>【停车场景信息】返回信息中的trade_scene为PARKING，返回该场景信息（官方字段路径 <c>parking_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("parking_info")]
    public VehicleParkingTransactionQueryResponseParkingInfo? ParkingInfo { get; set; }

    /// <summary>【支付者信息】支付者信息（官方字段路径 <c>payer</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payer")]
    public VehicleParkingTransactionQueryResponsePayer? Payer { get; set; }

    /// <summary>【订单金额信息】订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public VehicleParkingTransactionQueryResponseAmount? Amount { get; set; }

    /// <summary>【优惠信息】优惠信息（官方字段路径 <c>promotion_detail</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<VehicleParkingTransactionQueryResponsePromotionDetailItem>? PromotionDetail { get; set; }

    /// <summary>【子商户公众账号ID】子商户申请的公众号或移动应用AppID，需要在服务商的商户平台为子商户绑定（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户号】微信支付分配的子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>停车扣费受理下单请求·【订单金额】订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionRequestAmount
{
    /// <summary>【订单金额】订单总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>停车扣费受理下单请求·【停车场景信息】当交易场景为PARKING时，需要在该字段添加停车场景信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionRequestParkingInfo
{
    /// <summary>【入场ID】微信支付分停车服务为商户分配的入场ID，商户通过入场通知接口获取入场ID（官方字段路径 <c>parking_info.parking_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("parking_id")]
    public string? ParkingId { get; set; }

    /// <summary>【车牌号】车牌号，仅包括省份+车牌，不包括特殊字符（官方字段路径 <c>parking_info.plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【车牌颜色】车牌颜色。可选取值：`BLUE` 蓝色 / `GREEN` 绿色 / `YELLOW` 黄色 / `BLACK` 黑色 / `WHITE` 白色 / `LIMEGREEN` 黄绿色（官方字段路径 <c>parking_info.plate_color</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plate_color")]
    public string? PlateColor { get; set; }

    /// <summary>【入场时间】用户入场时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>parking_info.start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【出场时间】用户出场时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>parking_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【停车场名称】所在停车位车场的名称（官方字段路径 <c>parking_info.parking_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("parking_name")]
    public string? ParkingName { get; set; }

    /// <summary>【计费时长】计费的时间长，单位为秒（官方字段路径 <c>parking_info.charging_duration</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("charging_duration")]
    public long? ChargingDuration { get; set; }

    /// <summary>【停车场设备ID】停车场设备ID（官方字段路径 <c>parking_info.device_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

}

/// <summary>停车扣费受理下单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionRequest
{
    /// <summary>【公众账号ID】 AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【服务描述】商户自定义字段，用于交易账单中对扣费服务的描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【附加数据】附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【商户订单号】商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【交易场景】交易场景值，目前支持 ：PARKING：车场停车场景（官方字段路径 <c>trade_scene</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("trade_scene")]
    public string? TradeScene { get; set; }

    /// <summary>【订单优惠标记】代金券或立减优惠功能的参数，说明详见代金券或立减优惠（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【回调通知URL】接受扣款结果异步回调通知的URL，注意回调URL只接受HTTPS（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【分账标识】Y：是，需要分账 N：否，不分账 字母要求大写，不传默认不分账，分账详细说明见直连分账API、服务商分账API文档（官方字段路径 <c>profit_sharing</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("profit_sharing")]
    public string? ProfitSharing { get; set; }

    /// <summary>【订单金额】订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public VehicleParkingTransactionRequestAmount? Amount { get; set; }

    /// <summary>【停车场景信息】当交易场景为PARKING时，需要在该字段添加停车场景信息（官方字段路径 <c>parking_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("parking_info")]
    public VehicleParkingTransactionRequestParkingInfo? ParkingInfo { get; set; }

    /// <summary>【子公众账号ID】子公众账号ID，服务商模式下选传，用于扣费信息的商户信息展示（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户号】微信支付分配的子商户号，服务商模式下必传（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>停车扣费受理下单响应·【停车场景信息】返回信息中的trade_scene为PARKING，返回该场景信。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionResponseParkingInfo
{
    /// <summary>【入场ID】微信支付分停车服务为商户分配的入场ID，商户通过入场通知接口获取入场ID（官方字段路径 <c>parking_info.parking_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("parking_id")]
    public string? ParkingId { get; set; }

    /// <summary>【车牌号】车牌号，仅包括省份+车牌，不包括特殊字符（官方字段路径 <c>parking_info.plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【车牌颜色】车牌颜色。可选取值：`BLUE` 蓝色 / `GREEN` 绿色 / `YELLOW` 黄色 / `BLACK` 黑色 / `WHITE` 白色 / `LIMEGREEN` 黄绿色（官方字段路径 <c>parking_info.plate_color</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plate_color")]
    public string? PlateColor { get; set; }

    /// <summary>【入场时间】用户入场时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>parking_info.start_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>【出场时间】用户出场时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>parking_info.end_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>【停车场名称】所在停车位车场的名称（官方字段路径 <c>parking_info.parking_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("parking_name")]
    public string? ParkingName { get; set; }

    /// <summary>【计费时长】计费的时间长，单位为秒（官方字段路径 <c>parking_info.charging_duration</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("charging_duration")]
    public long? ChargingDuration { get; set; }

    /// <summary>【停车场设备ID】停车场设备ID（官方字段路径 <c>parking_info.device_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

}

/// <summary>停车扣费受理下单响应·【支付者信息】支付者信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionResponsePayer
{
    /// <summary>【用户在AppID下的标识】 用户在AppID下的唯一标识（官方字段路径 <c>payer.openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户在Sub_AppID下的标识】 用户在Sub_AppID下的标识，商户扣费时传入了Sub_AppID，则会返回该用户在Sub_AppID下的标识（官方字段路径 <c>payer.sub_openid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>停车扣费受理下单响应·【订单金额信息】订单金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionResponseAmount
{
    /// <summary>【订单金额】订单总金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】符合ISO 4217标准的三位字母代码，目前只支持人民币：CNY（官方字段路径 <c>amount.currency</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>【用户实际支付金额】用户实际支付金额，单位为分，只能为整数，详见支付金额（官方字段路径 <c>amount.payer_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>【折扣】订单折扣（官方字段路径 <c>amount.discount_total</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("discount_total")]
    public long? DiscountTotal { get; set; }

}

/// <summary>停车扣费受理下单响应·【优惠信息】优惠信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionResponsePromotionDetailItem
{
    /// <summary>【券ID】券或者立减优惠ID（官方字段路径 <c>promotion_detail.coupon_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>【优惠名称】优惠名称（官方字段路径 <c>promotion_detail.name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【优惠范围】GLOBAL-全场代金券， SINGLE-单品优惠（官方字段路径 <c>promotion_detail.scope</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>【优惠类型】枚举值： CASH：充值型代金券 NOCASH：免充值型代金券（官方字段路径 <c>promotion_detail.type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【活动ID】在微信商户后台配置的批次ID（官方字段路径 <c>promotion_detail.stock_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【优惠券面额】用户享受优惠的金额（官方字段路径 <c>promotion_detail.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【微信出资】特指由微信支付商户平台创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.wechatpay_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>【商户出资】特指商户自己创建的优惠，出资金额等于本项优惠总金额，单位为分（官方字段路径 <c>promotion_detail.merchant_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>【其他出资】其他出资方出资金额，单位为分（官方字段路径 <c>promotion_detail.other_contribute</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>【优惠币种】CNY：人民币，境内商户号仅支持人民币（官方字段路径 <c>promotion_detail.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>停车扣费受理下单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534352"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534427"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingTransactionResponse : WechatPayResponse
{
    /// <summary>【公众账号ID】AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【服务描述】商户自定义字段，用于交易账单中对扣费服务的描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【订单创建时间】订单成功创建时返回，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【商户订单号】商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【微信支付订单号】微信支付订单号（官方字段路径 <c>transaction_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【交易状态】SUCCESS—支付成功 ACCEPTED—已接收，等待扣款 PAY_FAIL–支付失败(其他原因，如银行返回失败) REFUND—转入退款（官方字段路径 <c>trade_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>【交易状态描述】对当前订单状态的描述和下一步操作的指引（官方字段路径 <c>trade_state_description</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("trade_state_description")]
    public string? TradeStateDescription { get; set; }

    /// <summary>【支付完成时间】订单支付完成时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>success_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【付款银行】银行类型，采用字符串类型的银行标识。BPA：该笔订单由微信进行垫付（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【用户是否已还款】枚举值： Y：用户已还款 N：用户未还款 注意：使用此字段前需先确认bank_type字段值为BPA以及 trade_state字段值为SUCCESS（官方字段路径 <c>user_repaid</c>，选填，类型 <c>string(1)</c>）。</summary>
    [JsonPropertyName("user_repaid")]
    public string? UserRepaid { get; set; }

    /// <summary>【附加数据】附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【交易场景】交易场景值，目前支持 ：PARKING：车场停车场景（官方字段路径 <c>trade_scene</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_scene")]
    public string? TradeScene { get; set; }

    /// <summary>【停车场景信息】返回信息中的trade_scene为PARKING，返回该场景信息（官方字段路径 <c>parking_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("parking_info")]
    public VehicleParkingTransactionResponseParkingInfo? ParkingInfo { get; set; }

    /// <summary>【支付者信息】支付者信息（官方字段路径 <c>payer</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("payer")]
    public VehicleParkingTransactionResponsePayer? Payer { get; set; }

    /// <summary>【订单金额信息】订单金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public VehicleParkingTransactionResponseAmount? Amount { get; set; }

    /// <summary>【优惠信息】优惠信息（官方字段路径 <c>promotion_detail</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<VehicleParkingTransactionResponsePromotionDetailItem>? PromotionDetail { get; set; }

    /// <summary>【子商户公众账号ID】子商户申请的公众号或移动应用AppID，需要在服务商的商户平台为子商户绑定（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户号】微信支付分配的子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}
