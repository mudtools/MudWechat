// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 查询退款请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundQueryRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("out_refund_no")] public string? OutRefundNo { get; set; }
    [JsonPropertyName("refund_id")] public string? RefundId { get; set; }
    [JsonPropertyName("offset")] public int? Offset { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询退款应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundQueryResponse
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
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    [JsonPropertyName("refund_count")] public int? RefundCount { get; set; }
}

/// <summary>
/// APIv2 查询退款 V2 请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundQueryV2Request : V2RefundQueryRequest
{
}

/// <summary>
/// APIv2 查询退款 V2 应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2RefundQueryV2Response : V2RefundQueryResponse
{
}

/// <summary>
/// APIv2 撤销订单请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ReverseRequest
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
/// APIv2 撤销订单应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ReverseResponse
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
    /// <summary>是否需要继续调用撤销（<c>recall</c>）：<c>Y</c> 表示需要再次调用，<c>N</c> 表示成功，<c>FAILED</c> 表示失败。</summary>
    [JsonPropertyName("recall")] public string? Recall { get; set; }
}

/// <summary>
/// APIv2 下载对账单请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DownloadBillRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>账单日期（<c>bill_date</c>，必填 string(8)）：格式 <c>yyyyMMdd</c>。</summary>
    [JsonPropertyName("bill_date")] public string? BillDate { get; set; }
    /// <summary>账单类型（<c>bill_type</c>，选填 string(32)）：<c>ALL</c> / <c>SUCCESS</c> / <c>REFUND</c>。</summary>
    [JsonPropertyName("bill_type")] public string? BillType { get; set; }
    /// <summary>压缩类型（<c>tar_type</c>，选填 string(32)）：<c>GZIP</c>。</summary>
    [JsonPropertyName("tar_type")] public string? TarType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 下载资金账单请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DownloadFundFlowRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("bill_date")] public string? BillDate { get; set; }
    /// <summary>资金账户类型（<c>account_type</c>，必填 string(32)）：<c>Basic</c>（基本账户）/ <c>Operation</c>（运营账户）/ <c>Fees</c>（手续费账户）。</summary>
    [JsonPropertyName("account_type")] public string? AccountType { get; set; }
    [JsonPropertyName("tar_type")] public string? TarType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询结算状态请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SettlementQueryRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询结算状态应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SettlementQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>结算状态（<c>settlement_status</c>）：<c>CLOSED</c>（已结算）/ <c>OPEN</c>（未结算）。</summary>
    [JsonPropertyName("settlement_status")] public string? SettlementStatus { get; set; }
}

/// <summary>
/// APIv2 查询汇率请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ExchangeRateQueryRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>币种（<c>fee_type</c>，必填 string(16)）：如 <c>USD</c> / <c>HKD</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("date")] public string? Date { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询汇率应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ExchangeRateQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>汇率（<c>exchange_rate</c>）：如 <c>100</c> 表示 1 外币 = 100 分人民币。</summary>
    [JsonPropertyName("exchange_rate")] public string? ExchangeRate { get; set; }
}