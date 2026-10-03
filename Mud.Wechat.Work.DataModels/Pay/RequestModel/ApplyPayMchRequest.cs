// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 提交创建对外收款账户的申请单请求体（<c>/cgi-bin/miniapppay/apply_mch</c>）。
/// <para>
/// 企业通过本接口递交材料以创建对外收款账户申请单；图片类字段均须先经
/// 「提交图片」接口（<c>upload_image</c>）获取图片 ID，且只能使用该应用本身提交的图片。
/// </para>
/// <para>官方业务限制：同一提现人员最多申请 200 个商户号，同一企业最多申请 2000 个商户号。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class ApplyPayMchRequest
{
    /// <summary>
    /// 获取或设置业务申请编号（官方必填，1~32 个字符，可通过查询申请单状态接口查询）。
    /// </summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>
    /// 获取或设置主体类型（官方必填）：0 - 企业、1 - 个体、2 - 社会团体组织、3 - 事业单位。
    /// </summary>
    [JsonPropertyName("organization_type")]
    public int? OrganizationType { get; set; }

    /// <summary>
    /// 获取或设置营业执照/登记证书信息（官方必填，见 <see cref="PayBusinessLicenseInfo"/>）。
    /// </summary>
    [JsonPropertyName("business_license_info")]
    public PayBusinessLicenseInfo? BusinessLicenseInfo { get; set; }

    /// <summary>
    /// 获取或设置金融机构许可证信息（官方可选，主体为金融机构时必填）。
    /// </summary>
    [JsonPropertyName("finance_institution_info")]
    public PayFinanceInstitutionInfo? FinanceInstitutionInfo { get; set; }

    /// <summary>
    /// 获取或设置商户简称（官方必填，UTF-8，最多约 21 个汉字长度，将在支付完成页展示）。
    /// </summary>
    [JsonPropertyName("merchant_short_name")]
    public string? MerchantShortName { get; set; }

    /// <summary>
    /// 获取或设置经营者/法人证件信息（官方必填，见 <see cref="PayIdCardInfo"/>）。
    /// </summary>
    [JsonPropertyName("id_card_info")]
    public PayIdCardInfo? IdCardInfo { get; set; }

    /// <summary>
    /// 获取或设置经营者/法人是否为受益人（官方可选；主体为企业时必填；
    /// 是受益人即法人自然人时填 true，否则填 false）。
    /// </summary>
    [JsonPropertyName("owner")]
    public bool? Owner { get; set; }

    /// <summary>
    /// 获取或设置受益人证件信息（官方可选，仅企业主体且法人非受益人时填写；
    /// 证件信息结构与 <see cref="PayIdCardInfo"/> 一致）。
    /// </summary>
    [JsonPropertyName("ubo_info")]
    public PayIdCardInfo? UboInfo { get; set; }

    /// <summary>
    /// 获取或设置超级管理员信息（官方必填，见 <see cref="PayContactInfo"/>）。
    /// </summary>
    [JsonPropertyName("contact_info")]
    public PayContactInfo? ContactInfo { get; set; }

    /// <summary>
    /// 获取或设置结算账户信息（官方必填，见 <see cref="PayAccountInfo"/>）。
    /// </summary>
    [JsonPropertyName("account_info")]
    public PayAccountInfo? AccountInfo { get; set; }

    /// <summary>
    /// 获取或设置经营场景证明（官方必填，见 <see cref="PaySalesSceneInfo"/>）。
    /// </summary>
    [JsonPropertyName("sales_scene_info")]
    public PaySalesSceneInfo? SalesSceneInfo { get; set; }

    /// <summary>
    /// 获取或设置经营范围（官方必填）：如 1 - 餐饮、2 - 零售、28 - 培训机构等；
    /// 不同主体可选范围不同，取值对照以官方文档为准。
    /// </summary>
    [JsonPropertyName("business_id")]
    public int? BusinessId { get; set; }

    /// <summary>
    /// 获取或设置特殊资质（官方可选，最多 5 张图片 ID，经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("qualifications")]
    public PayPicIdList? Qualifications { get; set; }

    /// <summary>
    /// 获取或设置补充材料（官方可选，最多 5 张图片 ID，经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("business_addition_pics")]
    public PayPicIdList? BusinessAdditionPics { get; set; }

    /// <summary>
    /// 获取或设置提现人员 userid（官方必填）：须实名认证且与超级管理员为同一人，
    /// 并在应用可见范围内。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}

/// <summary>
/// 营业执照/登记证书信息（<see cref="ApplyPayMchRequest.BusinessLicenseInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBusinessLicenseInfo
{
    /// <summary>
    /// 获取或设置登记证书类型（官方可选，仅社会团体组织主体填写）：
    /// 如 2389 - 统一社会信用代码证书、2394 - 社会团体法人登记证书、
    /// 2520 - 执业许可证/执业证 等，取值对照以官方文档为准。
    /// </summary>
    [JsonPropertyName("cert_type")]
    public int? CertType { get; set; }

    /// <summary>
    /// 获取或设置证照扫描件图片 ID（官方必填，经「提交图片」接口预上传）；
    /// 须为彩色扫描或拍照图片，不接受二次剪裁、翻拍、PS。
    /// </summary>
    [JsonPropertyName("business_license_copy_open_wx_pay_media_id")]
    public string? BusinessLicenseCopyOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置注册号/统一社会信用代码（官方必填）：
    /// 个体为 15 位数字或 18 位数字（不含 I/O/Z/S/V 且 9 开头）；企业仅支持 18 位格式。
    /// </summary>
    [JsonPropertyName("business_license_number")]
    public string? BusinessLicenseNumber { get; set; }

    /// <summary>
    /// 获取或设置商户名称（官方必填，2~128 个字符；个体户特殊情况按「个体户XXX」命名）。
    /// </summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>
    /// 获取或设置经营者/法定代表人姓名（官方必填，2~100 个字符）。
    /// </summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>
    /// 获取或设置注册地址（官方可选；事业单位/社会组织必填，个体/企业建议填写，
    /// 否则系统查工商信息查不到会驳回；4~128 个字符）。
    /// </summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// 获取或设置营业期限开始日期（官方可选，格式 YYYY-MM-DD，
    /// 不早于 1900-01-01 且不晚于当天）。
    /// </summary>
    [JsonPropertyName("business_time_begin_time")]
    public string? BusinessTimeBeginTime { get; set; }

    /// <summary>
    /// 获取或设置营业期限结束日期（官方可选，格式 YYYY-MM-DD，须晚于开始时间；
    /// 长期营业填「长期」）。
    /// </summary>
    [JsonPropertyName("business_time_end_time")]
    public string? BusinessTimeEndTime { get; set; }
}

/// <summary>
/// 金融机构许可证信息（<see cref="ApplyPayMchRequest.FinanceInstitutionInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayFinanceInstitutionInfo
{
    /// <summary>
    /// 获取或设置金融机构类型（官方必填）：BANK_AGENT - 银行、PAYMENT_AGENT - 支付机构、
    /// INSURANCE - 保险、TRADE_AND_SETTLE - 交易及结算类、OTHER - 其他。
    /// </summary>
    [JsonPropertyName("finance_type")]
    public string? FinanceType { get; set; }

    /// <summary>
    /// 获取或设置金融机构许可证图片 ID 列表（官方必填，最多 5 张，
    /// 经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("finance_license_pics_open_wx_pay_media_id")]
    public List<string>? FinanceLicensePicsOpenWxPayMediaId { get; set; }
}

/// <summary>
/// 证件信息（<see cref="ApplyPayMchRequest.IdCardInfo"/> / <see cref="ApplyPayMchRequest.UboInfo"/> /
/// <see cref="PayContactInfo.ContactInfo"/>，三处结构一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayIdCardInfo
{
    /// <summary>
    /// 获取或设置证件正面照片 ID（官方必填；身份证为人像面照片，1 张，
    /// 经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("id_card_copy_open_wx_pay_media_id")]
    public string? IdCardCopyOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置证件反面照片 ID（官方可选；身份证为国徽面照片，护照无需上传，
    /// 经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("id_card_national_open_wx_pay_media_id")]
    public string? IdCardNationalOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置姓名（官方必填，2~100 个字符）。
    /// </summary>
    [JsonPropertyName("id_card_name")]
    public string? IdCardName { get; set; }

    /// <summary>
    /// 获取或设置证件号码（官方必填；支持身份证、护照、通行证、居留证等，各有格式要求）。
    /// </summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>
    /// 获取或设置证件居住地址（官方可选；企业主体必填，其他主体无需填写；
    /// 超级管理员证件信息中不填）。
    /// </summary>
    [JsonPropertyName("id_card_address")]
    public string? IdCardAddress { get; set; }

    /// <summary>
    /// 获取或设置证件有效期开始日期（官方必填，格式 YYYY-MM-DD）。
    /// </summary>
    [JsonPropertyName("id_card_valid_time_begin")]
    public string? IdCardValidTimeBegin { get; set; }

    /// <summary>
    /// 获取或设置证件有效期结束日期（官方必填，格式 YYYY-MM-DD；长期有效填「长期」）。
    /// </summary>
    [JsonPropertyName("id_card_valid_time")]
    public string? IdCardValidTime { get; set; }

    /// <summary>
    /// 获取或设置证件类型（官方可选，默认身份证）：0/8 - 身份证、1 - 护照、
    /// 2 - 香港通行证、3 - 澳门通行证、4 - 台湾通行证、5 - 外国人居留证、
    /// 6 - 港澳居民证、7 - 台湾居民证。
    /// </summary>
    [JsonPropertyName("id_doc_type")]
    public int? IdDocType { get; set; }
}

/// <summary>
/// 超级管理员信息（<see cref="ApplyPayMchRequest.ContactInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayContactInfo
{
    /// <summary>
    /// 获取或设置超级管理员类型（官方必填，字符串枚举）：
    /// 「65」- 经营者/法人、「66」- 经办人。
    /// </summary>
    [JsonPropertyName("contact_type")]
    public string? ContactType { get; set; }

    /// <summary>
    /// 获取或设置超级管理员证件信息（官方必填，结构同 <see cref="PayIdCardInfo"/>，
    /// 但不填「证件居住地址」）。
    /// <para>
    /// 官方约束：法人时只需姓名和证件号码且须与法人身份证一致；
    /// 经办人时证件类型必填。
    /// </para>
    /// </summary>
    [JsonPropertyName("contact_info")]
    public PayIdCardInfo? ContactDetail { get; set; }

    /// <summary>
    /// 获取或设置业务办理授权函图片 ID（官方可选；超级管理员为经办人时需上传，
    /// 授权函须全打印并加盖公章，经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("business_authorization_letter_open_wx_pay_media_id")]
    public string? BusinessAuthorizationLetterOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置手机号（官方必填）：11 位数字，或 5~20 位数字/连字符/加号。
    /// </summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    /// <summary>
    /// 获取或设置联系邮箱（官方必填，用于接收开户邮件及业务通知）。
    /// </summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }
}

/// <summary>
/// 结算账户信息（<see cref="ApplyPayMchRequest.AccountInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayAccountInfo
{
    /// <summary>
    /// 获取或设置账户类型（官方可选；主体为个体时必填）：74 - 对公账户、75 - 对私账户。
    /// </summary>
    [JsonPropertyName("bank_account_type")]
    public int? BankAccountType { get; set; }

    /// <summary>
    /// 获取或设置开户银行名称（官方必填，如「中国银行」）。
    /// </summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>
    /// 获取或设置开户名称（官方必填）：个人卡须与身份证姓名一致，
    /// 对公账户须与营业执照的商户名称一致。
    /// </summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>
    /// 获取或设置开户银行省市编码（官方必填，至少精确到市）。
    /// </summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>
    /// 获取或设置开户银行全称（官方可选，含「支行」；非直连银行需要填写）。
    /// </summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>
    /// 获取或设置银行账号（官方必填，长度遵循官方各银行卡号长度要求表）。
    /// </summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>
    /// 获取或设置银行卡补充资料（官方可选；主体为社会团体组织、事业单位时需填写）。
    /// </summary>
    [JsonPropertyName("bank_card_supplement")]
    public PayBankCardSupplement? BankCardSupplement { get; set; }
}

/// <summary>
/// 银行卡补充资料（<see cref="PayAccountInfo.BankCardSupplement"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayBankCardSupplement
{
    /// <summary>
    /// 获取或设置结算证明图片 ID（官方可选，1 张，经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("settlement_certificate_open_wx_pay_media_id")]
    public string? SettlementCertificateOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置关系证明图片 ID（官方可选，1 张，经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("relationship_certificate_open_wx_pay_media_id")]
    public string? RelationshipCertificateOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置其他关系证明图片 ID 列表（官方可选，最多 3 张，
    /// 经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("other_certificate_open_wx_pay_media_id")]
    public List<string>? OtherCertificateOpenWxPayMediaId { get; set; }
}

/// <summary>
/// 经营场景证明（<see cref="ApplyPayMchRequest.SalesSceneInfo"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PaySalesSceneInfo
{
    /// <summary>
    /// 获取或设置经营场景类型（官方必填）：1 - 线下门店、2 - 线上店铺。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置线上店铺商城地址（官方可选，最长 1024 字节；
    /// 线上店铺场景与商城二维码至少填一项）。
    /// </summary>
    [JsonPropertyName("store_url")]
    public string? StoreUrl { get; set; }

    /// <summary>
    /// 获取或设置线上店铺商城二维码图片 ID（官方可选，经「提交图片」接口预上传；
    /// 线上店铺场景与商城地址至少填一项）。
    /// </summary>
    [JsonPropertyName("store_pic_open_wx_pay_media_id")]
    public string? StorePicOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置线下场所省市区编码（官方可选，6 位数字字符串；线下门店场景必填）。
    /// </summary>
    [JsonPropertyName("address_code")]
    public string? AddressCode { get; set; }

    /// <summary>
    /// 获取或设置线下场所详细地址（官方可选，4~512 个字符；线下门店场景必填）。
    /// </summary>
    [JsonPropertyName("offline_address")]
    public string? OfflineAddress { get; set; }

    /// <summary>
    /// 获取或设置线下场所门头（外部）照片图片 ID（官方可选，经「提交图片」接口预上传；
    /// 线下门店场景必填）。
    /// </summary>
    [JsonPropertyName("entrance_pic_open_wx_pay_media_id")]
    public string? EntrancePicOpenWxPayMediaId { get; set; }

    /// <summary>
    /// 获取或设置店铺内部照片图片 ID（官方可选，经「提交图片」接口预上传；
    /// 线下门店场景必填）。
    /// </summary>
    [JsonPropertyName("indoor_pic_open_wx_pay_media_id")]
    public string? IndoorPicOpenWxPayMediaId { get; set; }
}

/// <summary>
/// 图片 ID 列表（<see cref="ApplyPayMchRequest.Qualifications"/> /
/// <see cref="ApplyPayMchRequest.BusinessAdditionPics"/>，官方 JSON 形状为
/// <c>{"id": ["xxx"]}</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayPicIdList
{
    /// <summary>
    /// 获取或设置图片 ID 列表（经「提交图片」接口预上传）。
    /// </summary>
    [JsonPropertyName("id")]
    public List<string>? Id { get; set; }
}
