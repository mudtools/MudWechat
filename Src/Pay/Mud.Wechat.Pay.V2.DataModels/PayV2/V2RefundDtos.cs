// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 申请退款请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }
    /// <summary>订单金额（<c>total_fee</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    /// <summary>退款金额（<c>refund_fee</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("refund_fee")] public int? RefundFee { get; set; }
    [JsonPropertyName("refund_fee_type")] public string? RefundFeeType { get; set; }
    [JsonPropertyName("refund_desc")] public string? RefundDesc { get; set; }
    [JsonPropertyName("refund_account")] public string? RefundAccount { get; set; }
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 申请退款应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundResponse
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
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }
    /// <summary>微信退款单号（<c>refund_id</c>）。</summary>
    [JsonPropertyName("refund_id")] public string? RefundId { get; set; }
    [JsonPropertyName("refund_fee")] public int? RefundFee { get; set; }
    [JsonPropertyName("settlement_refund_fee")] public int? SettlementRefundFee { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }
    [JsonPropertyName("cash_refund_fee")] public int? CashRefundFee { get; set; }
}

/// <summary>
/// APIv2 申请退款 V2 请求体（支持分账退款）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundV2Request : V2RefundRequest
{
}

/// <summary>
/// APIv2 申请退款 V2 应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundV2Response : V2RefundResponse
{
}