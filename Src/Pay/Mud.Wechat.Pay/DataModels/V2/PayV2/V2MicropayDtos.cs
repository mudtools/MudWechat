// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 付款码支付请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2MicropayRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("body")] public string? Body { get; set; }
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }
    [JsonPropertyName("goods_tag")] public string? GoodsTag { get; set; }
    [JsonPropertyName("limit_pay")] public string? LimitPay { get; set; }
    /// <summary>授权码（<c>auth_code</c>，必填 string(128)）：用户付款码内容。</summary>
    [JsonPropertyName("auth_code")] public string? AuthCode { get; set; }
    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 付款码支付应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2MicropayResponse
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
    /// <summary>交易类型（<c>trade_type</c>）：固定为 <c>MICROPAY</c>。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    /// <summary>付款银行（<c>bank_type</c>）。</summary>
    [JsonPropertyName("bank_type")] public string? BankType { get; set; }
    /// <summary>货币种类（<c>fee_type</c>）。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    /// <summary>现金支付金额（<c>cash_fee</c>）：单位为分。</summary>
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }
    /// <summary>现金支付货币类型（<c>cash_fee_type</c>）。</summary>
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }
    /// <summary>代金券金额（<c>coupon_fee</c>）：单位为分。</summary>
    [JsonPropertyName("coupon_fee")] public int? CouponFee { get; set; }
    /// <summary>代金券使用数量（<c>coupon_count</c>）。</summary>
    [JsonPropertyName("coupon_count")] public int? CouponCount { get; set; }
    /// <summary>微信支付订单号（<c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    /// <summary>商户订单号（<c>out_trade_no</c>）。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    /// <summary>商家数据包（<c>attach</c>）。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    /// <summary>支付完成时间（<c>time_end</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_end")] public string? TimeEnd { get; set; }
}