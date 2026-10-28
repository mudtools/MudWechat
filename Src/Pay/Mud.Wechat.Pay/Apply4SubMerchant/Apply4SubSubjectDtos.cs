// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

/// <summary>
/// 主体资料（进件 <c>subject_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 字段表照官方「提交申请单」页原文（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>，
/// 2026-10-14 逐字段核验）。
/// </para>
/// <para>
/// <b>三选一的条件必填（官方原文，C# 类型上无法表达，故全部声明为可空）</b>：
/// 主体为个体户 / 企业 ⇒ <see cref="BusinessLicenseInfo"/> 必填、<see cref="CertificateInfo"/> 不填；
/// 主体为政府机关 / 事业单位 / 社会组织 ⇒ <see cref="CertificateInfo"/> 必填。
/// <see cref="FinanceInstitutionInfo"/> 仅当 <see cref="FinanceInstitution"/> 为 <c>true</c> 时必填。
/// </para>
/// <para>
/// <b><c>certificate_letter_copy</c> 在两处出现</b>：本对象与其下 <see cref="CertificateInfo"/> 都列了该字段
/// （官方页面原文）。主体为政府机关 / 事业单位时选传 —— 上传则审核通过后即可签约，
/// 未上传则审核通过后仍需<b>汇款验证</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubSubjectInfo
{
    /// <summary>主体类型（<c>subject_type</c>，必填）：取值见 <see cref="Apply4SubSubjectTypes"/>，须与营业执照 / 登记证书一致。</summary>
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }

    /// <summary>是否是金融机构（<c>finance_institution</c>，选填 bool）：不传官方按 <c>false</c> 处理。</summary>
    [JsonPropertyName("finance_institution")]
    public bool? FinanceInstitution { get; set; }

    /// <summary>营业执照信息（<c>business_license_info</c>，条件必填），见 <see cref="Apply4SubBusinessLicenseInfo"/>。</summary>
    [JsonPropertyName("business_license_info")]
    public Apply4SubBusinessLicenseInfo? BusinessLicenseInfo { get; set; }

    /// <summary>登记证书信息（<c>certificate_info</c>，条件必填），见 <see cref="Apply4SubCertificateInfo"/>。</summary>
    [JsonPropertyName("certificate_info")]
    public Apply4SubCertificateInfo? CertificateInfo { get; set; }

    /// <summary>单位证明函照片（<c>certificate_letter_copy</c>，选填）：填文件上传 API 产出的 MediaID，语义见本类型 remarks。</summary>
    [JsonPropertyName("certificate_letter_copy")]
    public string? CertificateLetterCopy { get; set; }

    /// <summary>金融机构许可证信息（<c>finance_institution_info</c>，条件必填），见 <see cref="Apply4SubFinanceInstitutionInfo"/>。</summary>
    [JsonPropertyName("finance_institution_info")]
    public Apply4SubFinanceInstitutionInfo? FinanceInstitutionInfo { get; set; }

    /// <summary>经营者 / 法定代表人身份证件（<c>identity_info</c>，必填对象），见 <see cref="Apply4SubIdentityInfo"/>。</summary>
    [JsonPropertyName("identity_info")]
    public Apply4SubIdentityInfo? IdentityInfo { get; set; }

    /// <summary>最终受益人信息列表（<c>ubo_info_list</c>，选填数组，<b>最多 4 人</b>）：见 <see cref="Apply4SubUboInfo"/>。</summary>
    /// <remarks>
    /// 个体户 / 政府机关 / 事业单位<b>无需填写</b>；企业或社会组织按官方规则填写 ——
    /// 若最终受益所有人仅有经营者 / 法定代表人 1 人则无需填写（平台自动回填），否则须列出<b>全部</b>受益人（含经营者本人）。
    /// </remarks>
    [JsonPropertyName("ubo_info_list")]
    public List<Apply4SubUboInfo>? UboInfoList { get; set; }
}

/// <summary>营业执照信息（进件 <c>subject_info.business_license_info</c>）。</summary>
/// <remarks>主体为个体户 / 企业时必填，需年检章齐全（当年注册除外）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubBusinessLicenseInfo
{
    /// <summary>营业执照照片（<c>license_copy</c>，必填）：填文件上传 API 产出的 MediaID，水印仅限微信支付业务相关。</summary>
    [JsonPropertyName("license_copy")]
    public string? LicenseCopy { get; set; }

    /// <summary>注册号 / 统一社会信用代码（<c>license_number</c>，必填）：个体户可填 15 位数字或 18 位（以 9 开头）；企业须 18 位且以 9 开头，不得含 I/O/Z/S/V。</summary>
    [JsonPropertyName("license_number")]
    public string? LicenseNumber { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，必填 string(128)）：2-128 字符，个体户登记名称为空时按「个体户＋经营者姓名」命名，且不得以「企业 / 公司 / 农民专业合作社」结尾。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>个体户经营者 / 法定代表人姓名（<c>legal_person</c>，必填）：2-100 字符，与营业执照一致。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>注册地址（<c>license_address</c>，选填）：未填时官方查国家工商信息回填；查不到会被驳回，故建议填写。1-512 字符。</summary>
    [JsonPropertyName("license_address")]
    public string? LicenseAddress { get; set; }

    /// <summary>有效期限开始日期（<c>period_begin</c>，选填）：<c>yyyy-MM-dd</c>，未填时官方查工商回填。</summary>
    [JsonPropertyName("period_begin")]
    public string? PeriodBegin { get; set; }

    /// <summary>有效期限结束日期（<c>period_end</c>，选填）：<c>yyyy-MM-dd</c> 或 <c>长期</c>，须晚于开始日期。</summary>
    [JsonPropertyName("period_end")]
    public string? PeriodEnd { get; set; }
}

/// <summary>登记证书信息（进件 <c>subject_info.certificate_info</c>）。</summary>
/// <remarks>主体为政府机关 / 事业单位 / 社会组织时必填；个体户 / 企业<b>不填</b>本对象。</remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubCertificateInfo
{
    /// <summary>登记证书照片（<c>cert_copy</c>，必填）：填文件上传 API 产出的 MediaID。</summary>
    [JsonPropertyName("cert_copy")]
    public string? CertCopy { get; set; }

    /// <summary>登记证书类型（<c>cert_type</c>，必填）：取值见 <see cref="Apply4SubCertificateTypes"/>，与主体类型有对应约束（政府机关仅 2389、事业单位仅 2388）。</summary>
    [JsonPropertyName("cert_type")]
    public string? CertType { get; set; }

    /// <summary>证书号（<c>cert_number</c>，必填 string(32)）：须与所选证书类型匹配；除政府证明文件外需满足 18 位数字或大写字母（不含 I/O/Z/S/V）。</summary>
    [JsonPropertyName("cert_number")]
    public string? CertNumber { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，必填 string(128)）：填登记证书上的名称，2-128 字符。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>注册地址（<c>company_address</c>，必填）：填登记证书的注册地址，1-512 字符。</summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>法定代表人（<c>legal_person</c>，必填 string(100)）：2-100 字符，不能仅含特殊字符。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>有效期限开始日期（<c>period_begin</c>，必填）：<c>yyyy-MM-dd</c>，不得早于 1900-01-01、不得晚于当前日期。</summary>
    [JsonPropertyName("period_begin")]
    public string? PeriodBegin { get; set; }

    /// <summary>有效期限结束日期（<c>period_end</c>，必填）：<c>yyyy-MM-dd</c> 或 <c>长期</c>，须晚于开始日期。</summary>
    [JsonPropertyName("period_end")]
    public string? PeriodEnd { get; set; }

    /// <summary>单位证明函照片（<c>certificate_letter_copy</c>，选填）：官方在本对象与上级 <see cref="Apply4SubSubjectInfo"/> 两处都列出该字段（页面原文），语义同上级字段说明。</summary>
    [JsonPropertyName("certificate_letter_copy")]
    public string? CertificateLetterCopy { get; set; }
}

/// <summary>金融机构许可证信息（进件 <c>subject_info.finance_institution_info</c>）。</summary>
/// <remarks>仅当主体是金融机构（<c>finance_institution = true</c>）时必填。</remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubFinanceInstitutionInfo
{
    /// <summary>金融机构类型（<c>finance_type</c>，必填）：取值见 <see cref="Apply4SubFinanceTypes"/>，须与营业执照 / 登记证书一致。</summary>
    [JsonPropertyName("finance_type")]
    public string? FinanceType { get; set; }

    /// <summary>金融机构许可证图片（<c>finance_license_pics</c>，必填数组，<b>最多 5 张</b>）：填文件上传 API 产出的 MediaID。</summary>
    [JsonPropertyName("finance_license_pics")]
    public List<string>? FinanceLicensePics { get; set; }
}

/// <summary>
/// 经营者 / 法定代表人身份证件（进件 <c>subject_info.identity_info</c>）。
/// </summary>
/// <remarks>
/// <b>身份证与其他证件二选一</b>：证件类型为身份证时填 <see cref="IdCardInfo"/>，
/// 否则填 <see cref="IdDocInfo"/>（官方条件必填，类型上均为可空）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubIdentityInfo
{
    /// <summary>证件持有人类型（<c>id_holder_type</c>，选填）：取值见 <see cref="Apply4SubIdHolderTypes"/>；仅政府机关 / 事业单位在无法提供法定代表人证件、改传经办人时填写。</summary>
    [JsonPropertyName("id_holder_type")]
    public string? IdHolderType { get; set; }

    /// <summary>证件类型（<c>id_doc_type</c>，选填）：取值见 <see cref="Apply4SubIdDocTypes"/>；持有人为法定代表人时填写，政府机关仅支持中国大陆居民身份证。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>法定代表人说明函（<c>authorize_letter_copy</c>，选填）：证件持有人类型为<b>经办人</b>时必须上传，填 MediaID。</summary>
    [JsonPropertyName("authorize_letter_copy")]
    public string? AuthorizeLetterCopy { get; set; }

    /// <summary>身份证信息（<c>id_card_info</c>，条件必填），见 <see cref="Apply4SubIdCardInfo"/>。</summary>
    [JsonPropertyName("id_card_info")]
    public Apply4SubIdCardInfo? IdCardInfo { get; set; }

    /// <summary>其他类型证件信息（<c>id_doc_info</c>，条件必填），见 <see cref="Apply4SubIdDocInfo"/>。</summary>
    [JsonPropertyName("id_doc_info")]
    public Apply4SubIdDocInfo? IdDocInfo { get; set; }

    /// <summary>经营者 / 法定代表人是否为受益人（<c>owner</c>，选填 bool）：<b>官方明示即将下线、无需传入</b>。</summary>
    [JsonPropertyName("owner")]
    public bool? Owner { get; set; }
}

/// <summary>身份证信息（进件 <c>identity_info.id_card_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubIdCardInfo
{
    /// <summary>身份证人像面照片（<c>id_card_copy</c>，必填）：填 MediaID，不得翻拍 / 截图 / 镜像 / PS。</summary>
    [JsonPropertyName("id_card_copy")]
    public string? IdCardCopy { get; set; }

    /// <summary>身份证国徽面照片（<c>id_card_national</c>，必填）：填 MediaID。</summary>
    [JsonPropertyName("id_card_national")]
    public string? IdCardNational { get; set; }

    /// <summary>身份证姓名（<c>id_card_name</c>，必填，<b>需加密</b>）：2-100 字符。</summary>
    [JsonPropertyName("id_card_name")]
    public string? IdCardName { get; set; }

    /// <summary>身份证号码（<c>id_card_number</c>，必填，<b>需加密</b>）：17 位数字 + 1 位数字或 X。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>身份证居住地址（<c>id_card_address</c>，选填，<b>需加密</b>）：主体为<b>企业</b>时必填，其他主体无需上传；1-512 字符。</summary>
    [JsonPropertyName("id_card_address")]
    public string? IdCardAddress { get; set; }

    /// <summary>身份证有效期开始时间（<c>card_period_begin</c>，必填）：<c>yyyy-MM-dd</c>。</summary>
    [JsonPropertyName("card_period_begin")]
    public string? CardPeriodBegin { get; set; }

    /// <summary>身份证有效期结束时间（<c>card_period_end</c>，必填）：<c>yyyy-MM-dd</c> 或 <c>长期</c>。</summary>
    [JsonPropertyName("card_period_end")]
    public string? CardPeriodEnd { get; set; }
}

/// <summary>其他类型证件信息（进件 <c>identity_info.id_doc_info</c>，证件类型非身份证时使用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubIdDocInfo
{
    /// <summary>证件正面照片（<c>id_doc_copy</c>，必填）：填 MediaID。</summary>
    [JsonPropertyName("id_doc_copy")]
    public string? IdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>id_doc_copy_back</c>，选填）：往来通行证 / 外国人居留证 / 港澳居民居住证 / 台湾居民居住证需上传；护照无需。</summary>
    [JsonPropertyName("id_doc_copy_back")]
    public string? IdDocCopyBack { get; set; }

    /// <summary>证件姓名（<c>id_doc_name</c>，必填，<b>需加密</b>）：2-100 字符。</summary>
    [JsonPropertyName("id_doc_name")]
    public string? IdDocName { get; set; }

    /// <summary>证件号码（<c>id_doc_number</c>，必填，<b>需加密</b>）：号码规范按证件类型而异（护照 4-15 位、通行证 8/10 位等，官方原文）。</summary>
    [JsonPropertyName("id_doc_number")]
    public string? IdDocNumber { get; set; }

    /// <summary>证件居住地址（<c>id_doc_address</c>，选填，<b>需加密</b>）：主体为<b>企业</b>时需填写；1-512 字符。</summary>
    [JsonPropertyName("id_doc_address")]
    public string? IdDocAddress { get; set; }

    /// <summary>证件有效期开始时间（<c>doc_period_begin</c>，必填）：<c>yyyy-MM-dd</c>。</summary>
    [JsonPropertyName("doc_period_begin")]
    public string? DocPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>doc_period_end</c>，必填）：<c>yyyy-MM-dd</c> 或 <c>长期</c>。</summary>
    [JsonPropertyName("doc_period_end")]
    public string? DocPeriodEnd { get; set; }
}

/// <summary>最终受益人信息（进件 <c>subject_info.ubo_info_list[]</c>，最多 4 人）。</summary>
/// <remarks>
/// 官方「受益所有人」判定口径（三选一即需申报）：直接或间接拥有超过 25% 股权或表决权的自然人；
/// 通过人事、财务等其他方式控制公司的自然人；公司高级管理人员（经理、副经理、财务负责人、董秘及章程规定的其他人员）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubUboInfo
{
    /// <summary>证件类型（<c>ubo_id_doc_type</c>，必填）：取值见 <see cref="Apply4SubIdDocTypes"/>。</summary>
    [JsonPropertyName("ubo_id_doc_type")]
    public string? UboIdDocType { get; set; }

    /// <summary>证件正面照片（<c>ubo_id_doc_copy</c>，必填）：身份证请传人像面，填 MediaID。</summary>
    [JsonPropertyName("ubo_id_doc_copy")]
    public string? UboIdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>ubo_id_doc_copy_back</c>，选填）：身份证请传国徽面；护照无需上传反面。</summary>
    [JsonPropertyName("ubo_id_doc_copy_back")]
    public string? UboIdDocCopyBack { get; set; }

    /// <summary>证件姓名（<c>ubo_id_doc_name</c>，必填，<b>需加密</b>）：2-100 字符。</summary>
    [JsonPropertyName("ubo_id_doc_name")]
    public string? UboIdDocName { get; set; }

    /// <summary>证件号码（<c>ubo_id_doc_number</c>，必填，<b>需加密</b>）：号码规范按证件类型而异（官方原文）。</summary>
    [JsonPropertyName("ubo_id_doc_number")]
    public string? UboIdDocNumber { get; set; }

    /// <summary>证件居住地址（<c>ubo_id_doc_address</c>，必填，<b>需加密</b>）：1-512 字符。</summary>
    [JsonPropertyName("ubo_id_doc_address")]
    public string? UboIdDocAddress { get; set; }

    /// <summary>证件有效期开始时间（<c>ubo_period_begin</c>，必填）：<c>yyyy-MM-dd</c>，不得早于 1900-01-01。</summary>
    [JsonPropertyName("ubo_period_begin")]
    public string? UboPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>ubo_period_end</c>，必填）：<c>yyyy-MM-dd</c> 或 <c>长期</c>。</summary>
    [JsonPropertyName("ubo_period_end")]
    public string? UboPeriodEnd { get; set; }
}
