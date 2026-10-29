// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4Subject;

/// <summary>
/// 主体信息（主体进件 <c>subject_info</c>，官方文档
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>四个条件必填块按主体类型分流</b>（官方原文，字段表内类型均为选填，由主体类型决定实际必填）：
/// 企业 / 个体户 ⇒ <see cref="BusinessLicenceInfo"/>；政府机关 / 事业单位 / 社会组织 ⇒
/// <see cref="CertificateInfo"/>；小微商户 ⇒ <see cref="AssistProveInfo"/>；
/// 主体是金融机构（<see cref="IsFinanceInstitution"/> 为 <c>true</c>）⇒ <see cref="FinanceInstitutionInfo"/>。
/// </para>
/// <para>
/// <b>⚠️ 本域字段名与特约商户进件三处不同，不可照抄</b>：营业执照块本域为
/// <c>business_licence_info</c>（英式 <c>licence</c>，进件域为美式 <c>business_license_info</c>）；
/// 是否金融机构本域为 <c>is_finance_institution</c>（进件域为 <c>finance_institution</c>）；
/// 单位证明函本域为 <c>company_prove_copy</c>（进件域为 <c>certificate_letter_copy</c>）。
/// 守卫 A4SJ-B3 逐字段锁死本域拼写，任何「对齐两个域」的改动都会在此 fail。
/// </para>
/// <para>
/// <b>法定代表人身份信息不在本对象内</b>：本域是顶层的 <c>identification_info</c>，
/// 进件域则嵌在 <c>subject_info.identity_info</c> —— 层级差异同属两域契约，照官方原文建模。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectSubjectInfo
{
    /// <summary>主体类型（<c>subject_type</c>，必填）：取值见 <see cref="Apply4SubjectSubjectTypes"/>，须与营业执照 / 登记证书一致。</summary>
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }

    /// <summary>是否金融机构（<c>is_finance_institution</c>，选填 boolean）：本域字段名带 <c>is_</c> 前缀，未传官方默认 <c>false</c>。</summary>
    [JsonPropertyName("is_finance_institution")]
    public bool? IsFinanceInstitution { get; set; }

    /// <summary>营业执照信息（<c>business_licence_info</c>，选填对象）：主体类型为企业 / 个体户时必填，见 <see cref="Apply4SubjectBusinessLicenceInfo"/>。</summary>
    [JsonPropertyName("business_licence_info")]
    public Apply4SubjectBusinessLicenceInfo? BusinessLicenceInfo { get; set; }

    /// <summary>登记证书信息（<c>certificate_info</c>，选填对象）：主体为政府机关 / 事业单位 / 社会组织时必填，见 <see cref="Apply4SubjectCertificateInfo"/>。</summary>
    [JsonPropertyName("certificate_info")]
    public Apply4SubjectCertificateInfo? CertificateInfo { get; set; }

    /// <summary>单位证明函照片（<c>company_prove_copy</c>，选填 string(255)）：主体类型为政府机关 / 事业单位时必填，填图片上传 API 返回的 MediaID。</summary>
    [JsonPropertyName("company_prove_copy")]
    public string? CompanyProveCopy { get; set; }

    /// <summary>辅助证明材料信息（<c>assist_prove_info</c>，选填对象）：主体类型为小微商户时必填，见 <see cref="Apply4SubjectAssistProveInfo"/>。</summary>
    [JsonPropertyName("assist_prove_info")]
    public Apply4SubjectAssistProveInfo? AssistProveInfo { get; set; }

    /// <summary>经营许可证（<c>special_operation_list</c>，选填数组）：特殊行业的经营许可信息，官方限<b>最多 5 个行业</b>。</summary>
    [JsonPropertyName("special_operation_list")]
    public List<Apply4SubjectSpecialOperation>? SpecialOperationList { get; set; }

    /// <summary>金融机构许可证（<c>finance_institution_info</c>，选填对象）：主体是金融机构时必填，见 <see cref="Apply4SubjectFinanceInstitutionInfo"/>。</summary>
    [JsonPropertyName("finance_institution_info")]
    public Apply4SubjectFinanceInstitutionInfo? FinanceInstitutionInfo { get; set; }
}

/// <summary>
/// 营业执照信息（<c>subject_info.business_licence_info</c>）。
/// </summary>
/// <remarks>
/// 块内字段官方全部标<b>必填</b>（在主体类型为企业 / 个体户的前提下）。
/// <b>注册号格式</b>：企业须为 18 位数字或大写英文字母（不得含 I/O/Z/S/V）且<b>以 9 开头</b>；
/// 个体户另允许 15 位数字 —— 两个主体的格式约束不同但共用本类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectBusinessLicenceInfo
{
    /// <summary>营业执照注册号（<c>licence_number</c>，必填 string(32)）：15 位数字（个体户）或 18 位统一社会信用代码（以 9 开头）。</summary>
    [JsonPropertyName("licence_number")]
    public string? LicenceNumber { get; set; }

    /// <summary>营业执照照片（<c>licence_copy</c>，必填 string(255)）：图片上传 API 返回的 MediaID，水印仅限微信支付业务相关。</summary>
    [JsonPropertyName("licence_copy")]
    public string? LicenceCopy { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，必填 string(128)）：营业执照登记名称；登记名称为空 / 仅含数字 / 仅含特殊字符 / 「无」/「无字号」时按「个体户 + 经营人姓名」命名，且个体户不得以「企业」「公司」「农民专业合作社」结尾。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>法定代表人姓名（<c>legal_person</c>，必填 string(100)）：营业执照的经营者 / 法定代表人姓名。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>注册地址（<c>company_address</c>，必填 string）：建议填营业执照注册地址，长度 1-512。</summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// 营业执照有效日期（<c>licence_valid_date</c>，必填 string(64)）。
    /// </summary>
    /// <remarks>
    /// <b>⚠️ 类型是字符串而不是数组</b>：官方示例值为 <c>["2017-10-28","长期"]</c> ——
    /// 「日期区间」被官方定义为<b>一个装着 JSON 数组文本的字符串</b>（本域三处有效日期字段同此形态）。
    /// 把它做成 <c>List&lt;string&gt;</c> 会得到 <c>400 PARAM_ERROR</c> 而非「更类型安全」。
    /// </remarks>
    [JsonPropertyName("licence_valid_date")]
    public string? LicenceValidDate { get; set; }
}

/// <summary>
/// 登记证书信息（<c>subject_info.certificate_info</c>）。
/// </summary>
/// <remarks>
/// 政府机关 / 事业单位 / 社会组织使用；块内字段官方全部标<b>必填</b>。
/// <c>cert_type</c> 的选择与主体类型严格绑定（见 <see cref="Apply4SubjectCertificateTypes"/> 的分组注释），
/// 且「社会组织」可选值最多（统一社会信用代码证书 / 社会团体法人登记证书 / 民办非企业 / 基金会 /
/// 执业许可证 / 基层群众性自治组织 / 农村集体经济组织 / 宗教活动场所 / 政府部门其他证明）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectCertificateInfo
{
    /// <summary>证书类型（<c>cert_type</c>，必填）：取值见 <see cref="Apply4SubjectCertificateTypes"/>，须与主体类型匹配。</summary>
    [JsonPropertyName("cert_type")]
    public string? CertType { get; set; }

    /// <summary>证书编号（<c>cert_number</c>，必填 string(50)）：除政府证明文件外须为 18 位数字或大写英文字母（不得含 I/O/Z/S/V）。</summary>
    [JsonPropertyName("cert_number")]
    public string? CertNumber { get; set; }

    /// <summary>证书照片（<c>cert_copy</c>，必填 string(255)）：图片上传 API 返回的 MediaID。</summary>
    [JsonPropertyName("cert_copy")]
    public string? CertCopy { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，必填 string(128)）：登记证书上的名称。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>法定代表人姓名（<c>legal_person</c>，必填 string(100)）：登记证书上的法定代表人姓名。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>注册地址（<c>company_address</c>，必填 string）：登记证书的注册地址，长度 1-512。</summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>证书有效日期（<c>cert_valid_date</c>，必填 string(64)）：<b>字符串内嵌 JSON 数组文本</b>，官方示例 <c>["2017-10-28","长期"]</c>，同 <see cref="Apply4SubjectBusinessLicenceInfo.LicenceValidDate"/>。</summary>
    [JsonPropertyName("cert_valid_date")]
    public string? CertValidDate { get; set; }
}

/// <summary>
/// 辅助证明材料信息（<c>subject_info.assist_prove_info</c>，小微商户）。
/// </summary>
/// <remarks>
/// 主体类型为小微商户时必填；官方对三个「门店 *」字段说明<b>随经营类型语义变化</b>
/// （门店场所 / 流动经营 / 线上交易分别填门店名称、经营服务所在地、线上店铺名），
/// 其中 <c>store_address</c> 在流动经营时官方要求填「无」、线上交易时填电商平台名称 ⇒ 字段名不等于语义，勿据名推断。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectAssistProveInfo
{
    /// <summary>小微经营类型（<c>micro_biz_type</c>，必填）：取值见 <see cref="Apply4SubjectMicroBizTypes"/>。</summary>
    [JsonPropertyName("micro_biz_type")]
    public string? MicroBizType { get; set; }

    /// <summary>门店名称（<c>store_name</c>，必填 string(50)）：按经营类型分别填门店名称 / 经营服务名称 / 线上店铺名称。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>门店省市编码（<c>store_address_code</c>，必填 string(16)）：只能由数字组成，取值见官方省市对照表（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012082815"/>）。</summary>
    [JsonPropertyName("store_address_code")]
    public string? StoreAddressCode { get; set; }

    /// <summary>门店地址（<c>store_address</c>，必填 string(512)）：门店场所填详细地址；流动经营填「无」；线上交易填电商平台名称。</summary>
    [JsonPropertyName("store_address")]
    public string? StoreAddress { get; set; }

    /// <summary>门店门头照片（<c>store_header_copy</c>，必填 string(255)）：MediaID；无固定门头的市场所可 upload 岗亭 / 出入闸口。</summary>
    [JsonPropertyName("store_header_copy")]
    public string? StoreHeaderCopy { get; set; }

    /// <summary>店内环境照片（<c>store_indoor_copy</c>，必填 string(255)）：MediaID。</summary>
    [JsonPropertyName("store_indoor_copy")]
    public string? StoreIndoorCopy { get; set; }
}

/// <summary>
/// 特殊行业经营许可证（<c>subject_info.special_operation_list[]</c>，官方限最多 5 个行业）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectSpecialOperation
{
    /// <summary>行业类目 id（<c>category_id</c>，必填 integer）：见微信支付特殊行业 id 对照表。</summary>
    [JsonPropertyName("category_id")]
    public int? CategoryId { get; set; }

    /// <summary>行业经营许可证资质照片（<c>operation_copy_list</c>，选填 array&lt;string(255)&gt;）：MediaID 列表，每个行业最多 5 张；仅当所选行业为「必填经营许可证」行业时才需上传。</summary>
    [JsonPropertyName("operation_copy_list")]
    public List<string>? OperationCopyList { get; set; }
}

/// <summary>
/// 金融机构许可证（<c>subject_info.finance_institution_info</c>）。
/// </summary>
/// <remarks>主体是金融机构（<c>is_finance_institution = true</c>）时必填。</remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectFinanceInstitutionInfo
{
    /// <summary>金融机构类型（<c>finance_type</c>，必填）：取值见 <see cref="Apply4SubjectFinanceTypes"/>，须与营业执照 / 登记证书一致。</summary>
    [JsonPropertyName("finance_type")]
    public string? FinanceType { get; set; }

    /// <summary>金融机构许可证图片（<c>finance_license_pics</c>，必填 array&lt;string&gt;）：MediaID 列表，最多 5 张，可授权使用总公司 / 分公司的特殊资质。</summary>
    [JsonPropertyName("finance_license_pics")]
    public List<string>? FinanceLicensePics { get; set; }
}
