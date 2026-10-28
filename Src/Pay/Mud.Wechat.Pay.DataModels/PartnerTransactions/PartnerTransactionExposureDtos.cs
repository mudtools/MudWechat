// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商查询微信分付曝光建议请求（官方 <c>POST /v3/pay/partner/transactions/exposure</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方字段面（2026-10-25 .md 直抓，页 ID 4014326522）</b>：四分商户标识全必填
/// （<c>sub_appid</c> 除外）；<c>payer</c> 选填且三标识<b>只需填其一</b>
/// （<c>sp_openid</c> / <c>phone_number_hash</c> / <c>sub_openid</c>）——与分期曝光族
/// （<see cref="PartnerInstalExposureRequest"/> 的 <c>user_id</c> 对象 + <c>user_id_type</c>）不同表 ⇒ 分建。</para>
/// <para><b>与分期曝光族的分工</b>：本族是「下单前查分付曝光建议」（应答 <c>fqf_exposure</c> +
/// 调起用 <c>market_exposure_token</c>）；分期族是「分期入口曝光 + 档期息费减免券」。两族并存非替代。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerTransactionExposureRequest
{
    /// <summary>获取或设置服务商商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>获取或设置服务商 AppID（<c>sp_appid</c>，必填 string(32)；须与 sp_mchid 绑定）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>获取或设置子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>获取或设置子商户 AppID（<c>sub_appid</c>，选填 string(32)；须与 sub_mchid 绑定）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>获取或设置支付者信息（<c>payer</c>，选填；三标识只需填其一）。</summary>
    [JsonPropertyName("payer")]
    public PartnerTransactionExposurePayer? Payer { get; set; }

    /// <summary>获取或设置商户订单号（<c>out_trade_no</c>，必填 string(32)；须与下单接口的订单号一致）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}

/// <summary>服务商分付曝光的支付者节（官方 <c>payer</c>；三标识只需填其一）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerTransactionExposurePayer
{
    /// <summary>获取或设置用户服务商标识（<c>sp_openid</c>，选填 string(128)；使用时需传 sp_appid）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenId { get; set; }

    /// <summary>获取或设置用户手机号哈希（<c>phone_number_hash</c>，选填 string(64)：SM3 摘要大写十六进制）。</summary>
    [JsonPropertyName("phone_number_hash")]
    public string? PhoneNumberHash { get; set; }

    /// <summary>获取或设置用户子商户标识（<c>sub_openid</c>，选填 string(128)；使用时需传 sub_appid）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }
}

/// <summary>
/// 从业机构查询微信分付曝光建议请求（官方 <c>POST /v3/pay/acquiring-bank/transactions/exposure</c>）。
/// </summary>
/// <remarks>
/// <para><b>支持商户类型为从业机构（银行 / 支付机构）</b>，字段面与服务商族完全不同表
/// （<c>acquiring_bank_id</c> / <c>merchant_id</c> / <c>channel_id</c> 三方标识），故 DTO 独立、
/// 不与 <see cref="PartnerTransactionExposureRequest"/> 共用。端点落在
/// <see cref="Mud.Wechat.Pay.IWechatPayPartnerTransactionsService"/> 上是该族当前唯一成员
/// （前缀例外，与 instal 曝光同形态），从业机构族后续扩域时随迁。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class AcquiringBankTransactionExposureRequest
{
    /// <summary>获取或设置从业机构号（<c>acquiring_bank_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>获取或设置从业机构应用 AppID（<c>acquiring_bank_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("acquiring_bank_appid")]
    public string? AcquiringBankAppId { get; set; }

    /// <summary>获取或设置特约商户号（<c>merchant_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

    /// <summary>获取或设置特约商户应用 AppID（<c>merchant_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("merchant_appid")]
    public string? MerchantAppId { get; set; }

    /// <summary>获取或设置服务商号（<c>channel_id</c>，必填 string(32)；收单服务商 ID）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>获取或设置支付者信息（<c>payer</c>，选填；三标识只需填其一）。</summary>
    [JsonPropertyName("payer")]
    public AcquiringBankTransactionExposurePayer? Payer { get; set; }

    /// <summary>获取或设置商户订单号（<c>out_trade_no</c>，必填 string(32)；须与下单接口的订单号一致）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}

/// <summary>从业机构分付曝光的支付者节（官方 <c>payer</c>；三标识只需填其一）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class AcquiringBankTransactionExposurePayer
{
    /// <summary>获取或设置用户标识·从业机构（<c>acquiring_bank_openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("acquiring_bank_openid")]
    public string? AcquiringBankOpenId { get; set; }

    /// <summary>获取或设置用户手机号哈希（<c>phone_number_hash</c>，选填 string(64)：SM3 摘要大写十六进制）。</summary>
    [JsonPropertyName("phone_number_hash")]
    public string? PhoneNumberHash { get; set; }

    /// <summary>获取或设置用户标识·特约商户（<c>merchant_openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("merchant_openid")]
    public string? MerchantOpenId { get; set; }
}

/// <summary>
/// 微信分付曝光建议应答（官方 <c>transactions/exposure</c> 与 <c>acquiring-bank/transactions/exposure</c>
/// 两页应答表<b>逐项一致</b> ⇒ 同表共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerTransactionExposureResponse : WechatPayResponse
{
    /// <summary>获取或设置是否建议曝光微信分付（<c>fqf_exposure</c>，必填 boolean）。</summary>
    [JsonPropertyName("fqf_exposure")]
    public bool? FqfExposure { get; set; }

    /// <summary>获取或设置曝光凭证（<c>market_exposure_token</c>，选填 string(128)；建议曝光时返回，调起支付时在 <c>package</c> 参数传入）。</summary>
    [JsonPropertyName("market_exposure_token")]
    public string? MarketExposureToken { get; set; }
}
