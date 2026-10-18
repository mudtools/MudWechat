// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 支付押金（JSAPI 下单，服务商侧）请求体。
/// </summary>
/// <remarks>官方本页 <c>sign_type</c> 同时支持 <c>HMAC-SHA256</c> 与 <c>MD5</c>，是押金族里唯一允许 MD5 的一支。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositUnifiedOrderRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("body")] public string? Body { get; set; }
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }
    [JsonPropertyName("time_start")] public string? TimeStart { get; set; }
    [JsonPropertyName("time_expire")] public string? TimeExpire { get; set; }
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    [JsonPropertyName("product_id")] public string? ProductId { get; set; }

    /// <summary>用户标识：trade_type=JSAPI 必传，与 <see cref="SubOpenId"/> 二选一。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>子商户用户标识：传本字段必须同时传 <see cref="SubAppId"/>。</summary>
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }

    [JsonPropertyName("receipt")] public string? Receipt { get; set; }

    /// <summary>场景信息：官方要求以 JSON 字符串传入（非 XML 子树）。</summary>
    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 支付押金（JSAPI 下单）应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositUnifiedOrderResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }

    /// <summary>预支付交易会话标识，官方标注有效期 2 小时。</summary>
    [JsonPropertyName("prepay_id")] public string? PrepayId { get; set; }

    [JsonPropertyName("code_url")] public string? CodeUrl { get; set; }
}

/// <summary>
/// APIv2 支付押金（人脸支付）请求体。
/// </summary>
/// <remarks>本支 <c>sign_type</c> 仅支持 <c>HMAC-SHA256</c>；无 <c>notify_url</c>（同步返回支付结果）。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositFacePayRequest
{
    /// <summary>是否押金支付：官方为 <c>String(1)</c> 的 <c>Y</c>/<c>N</c>，不是布尔。</summary>
    [JsonPropertyName("deposit")] public string? Deposit { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("body")] public string? Body { get; set; }
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }

    /// <summary>终端 IP：官方本页标注 String(16)（unifiedorder 为 String(64)），字段名照官方原文保留。</summary>
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }

    [JsonPropertyName("time_start")] public string? TimeStart { get; set; }
    [JsonPropertyName("time_expire")] public string? TimeExpire { get; set; }
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>人脸授权码（本支特有，替代付款码分支的 auth_code）。</summary>
    [JsonPropertyName("face_code")] public string? FaceCode { get; set; }

    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 支付押金（人脸支付）应答。
/// </summary>
/// <remarks><c>is_subscribe</c> / <c>sub_is_subscribe</c> 官方标注「已废弃，默认统一返回 N」，字段仍按报文保留。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositFacePayResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("openid")] public string? OpenId { get; set; }
    [JsonPropertyName("is_subscribe")] public string? IsSubscribe { get; set; }
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }
    [JsonPropertyName("sub_is_subscribe")] public string? SubIsSubscribe { get; set; }
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    [JsonPropertyName("bank_type")] public string? BankType { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }
    [JsonPropertyName("coupon_fee")] public int? CouponFee { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("time_end")] public string? TimeEnd { get; set; }
}

/// <summary>
/// APIv2 支付押金（付款码支付）请求体。
/// </summary>
/// <remarks>字段面与人脸支同源，差异只有 <c>auth_code</c>（替 <c>face_code</c>）、无 <c>openid</c>、有 <c>scene_info</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositMicropayRequest
{
    /// <summary>是否押金支付：<c>Y</c>-押金 / <c>N</c>-普通付款码支付，官方为 String(1)。</summary>
    [JsonPropertyName("deposit")] public string? Deposit { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("body")] public string? Body { get; set; }
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }
    [JsonPropertyName("time_start")] public string? TimeStart { get; set; }
    [JsonPropertyName("time_expire")] public string? TimeExpire { get; set; }

    /// <summary>付款码：官方规则为 18 位纯数字，以 10/11/12/13/14/15 开头。</summary>
    [JsonPropertyName("auth_code")] public string? AuthCode { get; set; }

    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 支付押金（付款码支付）应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositMicropayResponse : V2DepositFacePayResponse
{
}

/// <summary>
/// APIv2 查询押金订单请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositOrderQueryRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>微信订单号：与 <see cref="OutTradeNo"/> 二选一，同时存在时本字段优先。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询押金订单应答。
/// </summary>
/// <remarks>
/// <para>官方本页第一级提示写作「交易是否成功需要查看 <c>trade_state</c> 来判断」（非 <c>result_code</c>）。</para>
/// <para><c>trade_state</c> 在押金支付场景另有 <c>SETTLING</c> / <c>SETTLEMENTFAIL</c> / <c>CONSUMED</c> 三值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositOrderQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("openid")] public string? OpenId { get; set; }
    [JsonPropertyName("is_subscribe")] public string? IsSubscribe { get; set; }
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }
    [JsonPropertyName("sub_is_subscribe")] public string? SubIsSubscribe { get; set; }

    /// <summary>交易类型：官方本页取值 <c>MICROPAY</c> / <c>DEPOSITPAY</c>。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }

    [JsonPropertyName("trade_state")] public string? TradeState { get; set; }
    [JsonPropertyName("trade_state_desc")] public string? TradeStateDesc { get; set; }
    [JsonPropertyName("bank_type")] public string? BankType { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("time_end")] public string? TimeEnd { get; set; }

    /// <summary>押金消费金额：用户消费后结算给商户的金额（本支特有，以分为单位）。</summary>
    [JsonPropertyName("consume_fee")] public int? ConsumeFee { get; set; }
}

/// <summary>
/// APIv2 撤销押金订单请求体（需商户证书）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositReverseRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>微信订单号：与 <see cref="OutTradeNo"/> 同时存在时优先。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 撤销押金订单应答。
/// </summary>
/// <remarks>与基础支付的 <c>/pay/reverse</c> 不同：本支应答<b>不含</b> <c>recall_result</c>，且带 <c>sign</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositReverseResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
}

/// <summary>
/// APIv2 消费押金请求体（需商户证书）。
/// </summary>
/// <remarks>金额面就是 <c>total_fee</c>（押金总金额）+ <c>consume_fee</c>（消费金额），官方无 <c>deposit_total_fee</c> 一类字段。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositConsumeRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>微信订单号：与 <see cref="OutTradeNo"/> 同时存在时优先。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>押金总金额（分）。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }

    /// <summary>消费金额（分）：扣费区间 (0, 押金金额]，且累计消费 ≤ 押金金额。</summary>
    [JsonPropertyName("consume_fee")] public int? ConsumeFee { get; set; }

    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 消费押金应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositConsumeResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>押金总金额（分）。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }

    /// <summary>本次消费金额（分）。</summary>
    [JsonPropertyName("consume_fee")] public int? ConsumeFee { get; set; }

    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
}

/// <summary>
/// APIv2 押金退款请求体（需商户证书）。
/// </summary>
/// <remarks>本支以 <c>transaction_id</c> 单键定位（官方请求面<b>无</b> <c>out_trade_no</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositRefundRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>微信支付订单号（必填，本支不接受仅按商户订单号退款）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>押金退款单号：同一退款单号多次请求只退一笔，重试勿换号。</summary>
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }

    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("refund_fee")] public int? RefundFee { get; set; }
    [JsonPropertyName("refund_fee_type")] public string? RefundFeeType { get; set; }
    [JsonPropertyName("refund_desc")] public string? RefundDesc { get; set; }

    /// <summary>退款资金来源：仅老资金流商户使用（REFUND_SOURCE_UNSETTLED_FUNDS / REFUND_SOURCE_RECHARGE_FUNDS）。</summary>
    [JsonPropertyName("refund_account")] public string? RefundAccount { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 押金退款应答。
/// </summary>
/// <remarks>官方本页只有一级成功块：<c>result_code=SUCCESS</c> 仅代表<b>受理成功</b>，退款结果以查询接口为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositRefundResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }
    [JsonPropertyName("refund_id")] public string? RefundId { get; set; }
    [JsonPropertyName("refund_fee")] public int? RefundFee { get; set; }
    [JsonPropertyName("settlement_refund_fee")] public int? SettlementRefundFee { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("cash_refund_fee")] public int? CashRefundFee { get; set; }
    [JsonPropertyName("coupon_refund_fee")] public int? CouponRefundFee { get; set; }
}

/// <summary>
/// APIv2 查询押金退款请求体。
/// </summary>
/// <remarks>请求面只有 <c>refund_id</c> / <c>out_refund_no</c> 二选一，<b>无</b> <c>transaction_id</c> 与 <c>out_trade_no</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositRefundQueryRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>商户退款单号：与 <see cref="RefundId"/> 二选一，同时存在时 <c>refund_id</c> 优先。</summary>
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }

    [JsonPropertyName("refund_id")] public string? RefundId { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询押金退款应答。
/// </summary>
/// <remarks>
/// <para>押金族本页为<b>非下标</b>单退款形态（基础支付 <c>/pay/refundquery</c> 才是 <c>refund_count</c> + <c>_$n</c> 平铺）。</para>
/// <para><c>refund_recv_accout</c> 是<b>官方原文拼写</b>（accout 少一个 n），照抄不得「顺手修正」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DepositRefundQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }
    [JsonPropertyName("refund_id")] public string? RefundId { get; set; }

    /// <summary>退款渠道：ORIGINAL 原路 / BALANCE 退回余额 / OTHER_BALANCE / OTHER_BANKCARD。</summary>
    [JsonPropertyName("refund_channel")] public string? RefundChannel { get; set; }

    [JsonPropertyName("refund_fee")] public int? RefundFee { get; set; }
    [JsonPropertyName("settlement_refund_fee")] public int? SettlementRefundFee { get; set; }

    /// <summary>退款状态：SUCCESS / REFUNDCLOSE / PROCESSING / CHANGE（银行卡异常等，需商户平台人工处理）。</summary>
    [JsonPropertyName("refund_status")] public string? RefundStatus { get; set; }

    [JsonPropertyName("refund_account")] public string? RefundAccount { get; set; }

    /// <summary>退款到账账户（官方字段名为 refund_recv_accout，拼写照抄官方）。</summary>
    [JsonPropertyName("refund_recv_accout")] public string? RefundRecvAccout { get; set; }

    [JsonPropertyName("refund_success_time")] public string? RefundSuccessTime { get; set; }
}
