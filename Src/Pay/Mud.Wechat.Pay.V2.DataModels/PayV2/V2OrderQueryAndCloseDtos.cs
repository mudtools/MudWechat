// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 查询订单请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2OrderQueryRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询订单应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2OrderQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    [JsonPropertyName("trade_state")] public string? TradeState { get; set; }
    [JsonPropertyName("trade_state_desc")] public string? TradeStateDesc { get; set; }
    [JsonPropertyName("bank_type")] public string? BankType { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }
    [JsonPropertyName("coupon_fee")] public int? CouponFee { get; set; }
    [JsonPropertyName("coupon_count")] public int? CouponCount { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("time_end")] public string? TimeEnd { get; set; }
}

/// <summary>
/// APIv2 关闭订单请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CloseOrderRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 关闭订单应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CloseOrderResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
}