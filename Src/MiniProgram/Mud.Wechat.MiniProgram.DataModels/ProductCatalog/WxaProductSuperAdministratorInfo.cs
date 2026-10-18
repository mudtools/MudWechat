// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 超级管理员信息（官方 <c>super_administrator_info</c>）。
/// </summary>
/// <remarks>
/// <para><c>type=65</c> 法人 / 经营者、<c>type=66</c> 负责人；负责人须补证件类型、正反面、有效期与授权函。手机号与证件号属敏感个人信息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductSuperAdministratorInfo
{
    /// <summary>管理员类型（65 法人 / 经营者、66 负责人）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>管理员姓名（敏感）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>证件号码（敏感）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>手机号（敏感）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>邮箱。</summary>
    [JsonPropertyName("mail")]
    public string? Mail { get; set; }

    /// <summary>证件类型（type=66 时必填）。</summary>
    [JsonPropertyName("contact_id_doc_type")]
    public string? ContactIdDocType { get; set; }

    /// <summary>证件正面照片（type=66 时必填）。</summary>
    [JsonPropertyName("contact_id_doc_copy")]
    public WxaProductPicFile? ContactIdDocCopy { get; set; }

    /// <summary>证件反面照片（type=66 时必填）。</summary>
    [JsonPropertyName("contact_id_doc_copy_back")]
    public WxaProductPicFile? ContactIdDocCopyBack { get; set; }

    /// <summary>证件开始时间（type=66 时必填）。</summary>
    [JsonPropertyName("contact_id_doc_period_begin")]
    public string? ContactIdDocPeriodBegin { get; set; }

    /// <summary>证件结束时间（type=66 时必填）。</summary>
    [JsonPropertyName("contact_id_doc_period_end")]
    public string? ContactIdDocPeriodEnd { get; set; }

    /// <summary>业务办理授权函（type=66 时必填）。</summary>
    [JsonPropertyName("business_authorization_letter")]
    public WxaProductPicFile? BusinessAuthorizationLetter { get; set; }
}
