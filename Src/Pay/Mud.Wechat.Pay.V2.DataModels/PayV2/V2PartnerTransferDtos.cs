// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 服务商付款到银行卡请求体（<c>/mmpaysptrans/pay_bank</c>，需商户证书）。
/// </summary>
/// <remarks>
/// <para><b>单笔语义</b>：本端点是<b>单笔</b>付款（一请求一收款人），与 APIv3 的明细批量形态不同，
/// 故没有 <c>detail_list</c>，也<b>没有</b> <c>bank_type</c> —— 卡类型由 <c>bank_code</c>（银行编号）隐含决定。</para>
/// <para><b>敏感字段必须加密后传入</b>：<c>enc_bank_no</c> 与 <c>enc_true_name</c> 是用<b>微信支付平台证书公钥</b>
/// 做的 <b>RSA OAEP</b> 密文（<c>RSA/ECB/OAEPPadding</c>，Java 侧即
/// <c>RSA/ECB/OAEPWITHSHA-1ANDMGF1PADDING</c>）⇒ <b>不是</b> APIv3 的 AES-GCM，也<b>不是</b>
/// <c>req_info</c> 整包加密。公钥取自风控工具族 <c>/risk/getpublickey</c>（见 P2 批次）。</para>
/// <para><b>不出明文</b>：本 DTO 的两个密文字段属个人金融信息 ⇒ SDK 不落日志、不进异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PayBankRequest
{
    /// <summary>商户订单号（<c>partner_trade_no</c>，必填 string(32)）：商户系统内唯一，重试不得更换。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>加密银行卡号（<c>enc_bank_no</c>，必填 string(64)）：平台证书公钥 RSA-OAEP 密文，见类型注释。</summary>
    [JsonPropertyName("enc_bank_no")] public string? EncBankNo { get; set; }

    /// <summary>加密收款人户名（<c>enc_true_name</c>，必填 string(64)）：平台证书公钥 RSA-OAEP 密文，见类型注释。</summary>
    [JsonPropertyName("enc_true_name")] public string? EncTrueName { get; set; }

    /// <summary>银行编号（<c>bank_code</c>，必填 string(4)）：取官方「银行编号列表」；对公账户需另按该列表填支行信息。</summary>
    [JsonPropertyName("bank_code")] public string? BankCode { get; set; }

    /// <summary>付款金额（<c>amount</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("amount")] public int? Amount { get; set; }

    /// <summary>付款描述（<c>desc</c>，选填 string(32)）。</summary>
    [JsonPropertyName("desc")] public string? Desc { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商付款到银行卡应答。
/// </summary>
/// <remarks><c>cmms_amt</c> 是官方原文字段名（疑为历史命名），照抄不做「修正」。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PayBankResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>微信付款单号（<c>payment_no</c>）。</summary>
    [JsonPropertyName("payment_no")] public string? PaymentNo { get; set; }

    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>金额（<c>cmms_amt</c>）：官方原文字段名，单位为分。</summary>
    [JsonPropertyName("cmms_amt")] public int? CmsAmount { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询服务商付款到银行卡请求体（<c>/mmpaysptrans/query_bank</c>，需商户证书）。
/// </summary>
/// <remarks>查询族<b>不要求</b>加密字段：只需 <c>partner_trade_no</c> 定位原单。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryBankRequest
{
    /// <summary>商户订单号（<c>partner_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询服务商付款到银行卡应答。
/// </summary>
/// <remarks>
/// <para><b>卡号与户名只回 MD5</b>（<c>bank_no_md5</c> / <c>true_name_md5</c>）⇒
/// 核对到账信息须由宿主自行比对原值的 MD5，官方不提供可逆回显。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryBankResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>微信付款单号（<c>payment_no</c>）。</summary>
    [JsonPropertyName("payment_no")] public string? PaymentNo { get; set; }

    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>银行卡号 MD5（<c>bank_no_md5</c>）：官方只回摘要，不回卡号。</summary>
    [JsonPropertyName("bank_no_md5")] public string? BankNoMd5 { get; set; }

    /// <summary>收款户名 MD5（<c>true_name_md5</c>）：官方只回摘要，不回户名。</summary>
    [JsonPropertyName("true_name_md5")] public string? TrueNameMd5 { get; set; }

    /// <summary>付款金额（<c>amount</c>）：单位为分。</summary>
    [JsonPropertyName("amount")] public int? Amount { get; set; }

    /// <summary>打款状态（<c>status</c>）：<c>PROCESSING</c> / <c>SUCCESS</c> / <c>FAILED</c> / <c>BANK_FAIL</c>。</summary>
    [JsonPropertyName("status")] public string? Status { get; set; }

    /// <summary>创建时间（<c>create_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("create_time")] public string? CreateTime { get; set; }

    /// <summary>付款成功时间（<c>pay_succ_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("pay_succ_time")] public string? PaySuccTime { get; set; }

    /// <summary>失败原因（<c>reason</c>）。</summary>
    [JsonPropertyName("reason")] public string? Reason { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}
