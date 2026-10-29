// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 提交商户信息请求体（<c>POST /product/register/submit_merchantinfo</c>）。
/// </summary>
/// <remarks>
/// <para>含营业执照、身份证、结算账户与超级管理员信息 ⇒ 主体资质与个人金融 / 敏感个人信息集中面；官方硬限制每个主体类型字段集不同，<c>applyment_version</c> 2022-06-30 后须填 1。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductMerchantInfoRequest
{
    /// <summary>小程序 ID（必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>主体类型（"4" 个体工商户 / "2" 企业）。</summary>
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }

    /// <summary>营业执照信息（必填）。</summary>
    [JsonPropertyName("busi_license")]
    public WxaProductBusiLicense? BusiLicense { get; set; }

    /// <summary>组织机构代码证信息。</summary>
    [JsonPropertyName("organization_code_info")]
    public WxaProductOrganizationCodeInfo? OrganizationCodeInfo { get; set; }

    /// <summary>身份证信息（必填）。</summary>
    [JsonPropertyName("id_card_info")]
    public WxaProductIdCardInfo? IdCardInfo { get; set; }

    /// <summary>超级管理员信息（必填）。</summary>
    [JsonPropertyName("super_administrator_info")]
    public WxaProductSuperAdministratorInfo? SuperAdministratorInfo { get; set; }

    /// <summary>商户简称（最多 16 个汉字）。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortname { get; set; }

    /// <summary>特殊资质。</summary>
    [JsonPropertyName("special_qualification")]
    public WxaProductPicFileList? SpecialQualification { get; set; }

    /// <summary>补充材料。</summary>
    [JsonPropertyName("supplementary_material")]
    public WxaProductPicFileList? SupplementaryMaterial { get; set; }

    /// <summary>补充描述。</summary>
    [JsonPropertyName("supplementary_desc")]
    public string? SupplementaryDesc { get; set; }

    /// <summary>结算账户信息（必填）。</summary>
    [JsonPropertyName("settle_acct_info")]
    public WxaProductSettleAcctInfo? SettleAcctInfo { get; set; }

    /// <summary>支付资质版本号（默认 0；2022-06-30 后须填 1）。</summary>
    [JsonPropertyName("applyment_version")]
    public int? ApplymentVersion { get; set; }
}
