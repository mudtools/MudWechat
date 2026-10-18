// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 审核状态数据（官方 <c>data</c>）。
/// </summary>
/// <remarks>
/// <para><c>register_status=0</c> 时其余状态字段才有意义；<c>register_status</c> 0 成功 / 1 已发送协议未签约 / 2 未发送或协议过期。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductAuditStatusData
{
    /// <summary>注册状态（0 成功 / 1 已发送协议未签约 / 2 未发送或协议过期）。</summary>
    [JsonPropertyName("register_status")]
    public int? RegisterStatus { get; set; }

    /// <summary>商家信息状态（1~6）。</summary>
    [JsonPropertyName("merchant_info_status")]
    public int? MerchantInfoStatus { get; set; }

    /// <summary>账户验证状态（1~6）。</summary>
    [JsonPropertyName("acct_verify_status")]
    public int? AcctVerifyStatus { get; set; }

    /// <summary>基础信息状态（1~6）。</summary>
    [JsonPropertyName("basic_info_status")]
    public int? BasicInfoStatus { get; set; }

    /// <summary>支付签约状态（1~6）。</summary>
    [JsonPropertyName("pay_sign_status")]
    public int? PaySignStatus { get; set; }

    /// <summary>基础信息驳回原因。</summary>
    [JsonPropertyName("audit_reject_reasons")]
    public string? AuditRejectReasons { get; set; }

    /// <summary>法人验证链接。</summary>
    [JsonPropertyName("legal_validation_url")]
    public string? LegalValidationUrl { get; set; }

    /// <summary>支付资质审核详情。</summary>
    [JsonPropertyName("pay_audit_detail")]
    public WxaProductPayAuditDetail? PayAuditDetail { get; set; }

    /// <summary>注册的 appid。</summary>
    [JsonPropertyName("registered_appid")]
    public string? RegisteredAppid { get; set; }

    /// <summary>支付签约 url。</summary>
    [JsonPropertyName("sign_url")]
    public string? SignUrl { get; set; }
}
