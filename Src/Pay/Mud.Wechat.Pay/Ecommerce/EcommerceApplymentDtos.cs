// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 二级商户进件申请（<c>POST /v3/ecommerce/applyments/</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>
/// （2026-10-15 逐字段核验，全表 72 行）。
/// </para>
/// <para>
/// <b>路由结尾斜杠是官方的，不能删</b>（<c>/v3/ecommerce/applyments/</c>）：与同域其余端点「无尾斜杠」形态相反，
/// 去掉斜杠会 404。
/// </para>
/// <para>
/// <b>覆盖式修改</b>：申请单被驳回后，用<b>相同</b> <c>out_request_no</c> 再次提交即覆盖原申请单，
/// 官方原文「每个编号对应一个申请单」⇒ 换新编号会另起一单而不是改单。
/// </para>
/// <para>
/// <b>敏感字段必须加密</b>：本请求里带「需加密」标注的字段（身份证姓名/号码、证件姓名/号码、开户名称、银行账号、
/// 超级管理员姓名/手机/证件号、受益人姓名/证件号/居住地址）须用<b>微信支付公钥</b>（或平台证书公钥）加密，
/// 并在 HTTP 头补 <c>Wechatpay-Serial</c> ⇒ 明文直传会被拒或落库错误，SDK 不代为加密（加密属宿主证书资产）。
/// </para>
/// <para>
/// <b>三个字段官方标注「即将下线，无需传入」</b>：<see cref="Owner"/>、
/// <see cref="EcommerceApplymentAccountInfo.BankAddressCode"/>、
/// <see cref="EcommerceApplymentContactInfo.BusinessAuthorizationLetter"/> ⇒ 保留以对齐官方表，但新接入不要填。
/// </para>
/// <para>
/// 图片类字段（营业执照扫描件、身份证照片、特殊资质、补充材料等）一律填 <b>MediaID</b>，
/// 须先经 <c>POST /v3/merchant/media/upload</c>（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012760432"/>）上传换取。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentRequest
{
    /// <summary>业务申请编号（<c>out_request_no</c>，必填 string(124)）：服务商自定义的唯一编号，一编号对应一申请单；被驳回后可用<b>相同编号</b>覆盖修改原单。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>主体类型（<c>organization_type</c>，必填 string(4)）：取值为纯数字串，见 <see cref="EcommerceApplymentOrganizationTypes"/>；非小微主体须与营业执照/登记证书一致。</summary>
    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }

    /// <summary>是否金融机构（<c>finance_institution</c>，选填 boolean）：官方未传入时按 <c>false</c> 处理；为 <c>true</c> 时 <see cref="FinanceInstitutionInfo"/> 必填。</summary>
    [JsonPropertyName("finance_institution")]
    public bool? FinanceInstitution { get; set; }

    /// <summary>营业执照/登记证书信息（<c>business_license_info</c>，选填）：小微/个人卖家不填；个体工商户/企业传营业执照，政府机关/事业单位/社会组织传登记证书。</summary>
    [JsonPropertyName("business_license_info")]
    public EcommerceApplymentBusinessLicenseInfo? BusinessLicenseInfo { get; set; }

    /// <summary>金融机构许可证信息（<c>finance_institution_info</c>，选填）：主体是金融机构时必填。</summary>
    [JsonPropertyName("finance_institution_info")]
    public EcommerceApplymentFinanceInstitutionInfo? FinanceInstitutionInfo { get; set; }

    /// <summary>证件持有人类型（<c>id_holder_type</c>，选填）：见 <see cref="EcommerceApplymentIdHolderTypes"/>；仅政府机关/事业单位在无法提供法人证件时传 <c>SUPER</c>。</summary>
    [JsonPropertyName("id_holder_type")]
    public string? IdHolderType { get; set; }

    /// <summary>经营者/法定代表人证件类型（<c>id_doc_type</c>，选填）：见 <see cref="EcommerceApplymentIdDocTypes"/>；未填时系统默认身份证，小微/个人卖家仅支持身份证，政府机关仅支持大陆身份证。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>法定代表人说明函（<c>authorize_letter_copy</c>，选填 MediaID）：证件持有人类型为经办人时必传，须打印全部信息并加盖公章，不支持手写。</summary>
    [JsonPropertyName("authorize_letter_copy")]
    public string? AuthorizeLetterCopy { get; set; }

    /// <summary>经营者/法定代表人身份证信息（<c>id_card_info</c>，选填）：持有人为经营者/法定代表人且证件类型为身份证时填写。</summary>
    [JsonPropertyName("id_card_info")]
    public EcommerceApplymentIdCardInfo? IdCardInfo { get; set; }

    /// <summary>经营者/法定代表人其他证件信息（<c>id_doc_info</c>，选填）：持有人为经营者/法定代表人且证件类型<b>不为</b>身份证时填写。</summary>
    [JsonPropertyName("id_doc_info")]
    public EcommerceApplymentIdDocInfo? IdDocInfo { get; set; }

    /// <summary>经营者/法定代表人是否为受益人（<c>owner</c>，选填 boolean）：<b>官方标注「即将下线处理，无需传入」</b>；主体为企业时 formerly 要求 true/false。</summary>
    [JsonPropertyName("owner")]
    public bool? Owner { get; set; }

    /// <summary>最终受益人信息列表（<c>ubo_info_list</c>，选填数组）：小微/个人卖家/个体户/政府机关/事业单位无需填；企业或社会组织最多 4 人，若受益人仅经营者/法定代表人 1 人可不填（平台自动回填）。</summary>
    [JsonPropertyName("ubo_info_list")]
    public IReadOnlyList<EcommerceApplymentUboInfo>? UboInfoList { get; set; }

    /// <summary>结算账户信息（<c>account_info</c>，必填）：商家提现收款的银行账户信息。</summary>
    [JsonPropertyName("account_info")]
    public EcommerceApplymentAccountInfo? AccountInfo { get; set; }

    /// <summary>超级管理员信息（<c>contact_info</c>，必填）：需在开户后签约、接收重要管理信息并进行资金操作，官方要求确为商户法定代表人或负责人。</summary>
    [JsonPropertyName("contact_info")]
    public EcommerceApplymentContactInfo? ContactInfo { get; set; }

    /// <summary>经营场景信息（<c>sales_scene_info</c>，必填）：官方本页把该对象标为必填（账户托管进件页亦同）。</summary>
    [JsonPropertyName("sales_scene_info")]
    public EcommerceApplymentSalesSceneInfo? SalesSceneInfo { get; set; }

    /// <summary>结算规则（<c>settlement_info</c>，选填）：未传入时系统填默认值（0.6% 费率对应规则 id、无需特殊资质的行业）。</summary>
    [JsonPropertyName("settlement_info")]
    public EcommerceApplymentSettlementInfo? SettlementInfo { get; set; }

    /// <summary>商户简称（<c>merchant_shortname</c>，必填 string(64)）：UTF-8 下最多约 21 个汉字，将在支付完成页向买家展示，须与实际售卖商品相符。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortname { get; set; }

    /// <summary>特殊资质（<c>qualifications</c>，选填 string(1024)）：仅所选行业要求提交特殊资质时必填，最多 5 张 MediaID；可授权使用总公司/分公司的特殊资质。</summary>
    [JsonPropertyName("qualifications")]
    public string? Qualifications { get; set; }

    /// <summary>补充材料（<c>business_addition_pics</c>，选填 string(1024)）：按实际审核要求提供，最多 15 个文件（照片或 PDF）的 MediaID。</summary>
    [JsonPropertyName("business_addition_pics")]
    public string? BusinessAdditionPics { get; set; }

    /// <summary>补充说明（<c>business_addition_desc</c>，选填 string(512)）：主体为「个人卖家」时必传固定声明；被要求补资金来源/资金用途时按「【资金来源：…】【资金用途：…】」格式依次填入。</summary>
    [JsonPropertyName("business_addition_desc")]
    public string? BusinessAdditionDesc { get; set; }
}

/// <summary>
/// 进件「营业执照/登记证书」信息（<c>business_license_info</c>）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>（内层四字段标必填）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>（同字段标选填）⇒ 两支进件共用本类，
/// 必填性以各自请求 DTO 的 Remarks 为准，本类一律可空。
/// 官方命名规范：商户名称 2-128 字符、前后无空白、不能仅含数字/特殊字符、仅支持 utf-8；
/// 个体户在「登记名称为空/仅数字/仅特殊字符/无/无字号」时须按「个体户+经营者姓名」命名，且不得以企业/公司/农民专业合作社结尾。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentBusinessLicenseInfo
{
    /// <summary>证书类型（<c>cert_type</c>，选填）：见 <see cref="EcommerceApplymentCertTypes"/>；主体为个体工商户/企业时不填。</summary>
    [JsonPropertyName("cert_type")]
    public string? CertType { get; set; }

    /// <summary>营业执照/登记证书扫描件（<c>business_license_copy</c> MediaID）：彩色扫描件或数码拍摄件，黑白复印件需加盖完整公章；不得添加无关水印，仅可上传 1 张。</summary>
    [JsonPropertyName("business_license_copy")]
    public string? BusinessLicenseCopy { get; set; }

    /// <summary>营业执照注册号/统一社会信用代码（<c>business_license_number</c>）：个体户可 15 位数字或 18 位（9 开头）；企业仅 18 位（9 开头，大写英文字母不含 I/O/Z/S/V）。</summary>
    [JsonPropertyName("business_license_number")]
    public string? BusinessLicenseNumber { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，string(128)）：填登记证书上的名称；个体户特殊情形须按「个体户+经营者姓名」命名，不得以企业/公司/农民专业合作社结尾。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>经营者/法定代表人姓名（<c>legal_person</c>，string(100)）：须与证件一致，2-100 字符、不能仅含特殊字符。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>注册地址（<c>company_address</c>，选填）：政府机关/事业单位/社会组织必填；企业/个体户建议填写，未填时系统查国家工商信息补入，查不到则被驳回。</summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>营业期限（<c>business_time</c>，选填）：政府机关/事业单位/社会组织必填；企业/个体户未填时系统查工商信息补入；结束时间≥2099-01-01 时系统按「长期」处理；长期填「长期」。</summary>
    [JsonPropertyName("business_time")]
    public string? BusinessTime { get; set; }
}

/// <summary>金融机构许可证信息（<c>finance_institution_info</c>）：主体为金融机构时必填。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentFinanceInstitutionInfo
{
    /// <summary>金融机构类型（<c>finance_type</c>，必填）：须与营业执照/登记证书一致，见 <see cref="EcommerceFinanceTypes"/>。</summary>
    [JsonPropertyName("finance_type")]
    public string? FinanceType { get; set; }

    /// <summary>金融机构许可证图片（<c>finance_license_pics</c>，必填 MediaID 数组）：按所属金融机构类型的许可证要求提供，最多 5 张，须为申请商家主体所属（可授权总/分公司）。</summary>
    [JsonPropertyName("finance_license_pics")]
    public IReadOnlyList<string>? FinanceLicensePics { get; set; }
}

/// <summary>
/// 进件身份证信息（<c>id_card_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两支进件的必填集不同</b>：标准进件 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>
/// 六字段全标必填；账户托管进件 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>
/// 只有姓名与号码必填，照片与有效期为选填（官方原文：未填有效期时系统对上传的身份证照片做识别获取，识别不到则驳回）
/// ⇒ 共用本类（同字段名同类型，仅必填性差异），可空传递由宿主按目标接口决定。
/// </para>
/// <para>
/// <b><see cref="IdCardName"/> 与 <see cref="IdCardNumber"/> 需加密</b>（微信支付公钥或平台证书公钥），
/// 且请求头须上送 <c>Wechatpay-Serial</c>。号码官方规范作「7 位数字+1 位数字|X」，属官方原文表述，照抄勿改写。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentIdCardInfo
{
    /// <summary>身份证人像面照片（<c>id_card_copy</c>，string(256) MediaID）：正面拍摄、清晰、四角完整、无反光遮挡；不得翻拍/截图/镜像/PS，仅 1 张。</summary>
    [JsonPropertyName("id_card_copy")]
    public string? IdCardCopy { get; set; }

    /// <summary>身份证国徽面照片（<c>id_card_national</c>，string(256) MediaID）：要求同人像面，仅 1 张。</summary>
    [JsonPropertyName("id_card_national")]
    public string? IdCardNational { get; set; }

    /// <summary>身份证姓名（<c>id_card_name</c>，必填 string(100)）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>。</summary>
    [JsonPropertyName("id_card_name")]
    public string? IdCardName { get; set; }

    /// <summary>身份证号码（<c>id_card_number</c>，必填）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>；官方规范原文为「7位数字+1位数字|X」。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>身份证开始时间（<c>id_card_valid_time_begin</c>）：合法 YYYY-MM-DD，不早于 1900-01-01、不大于等于当前日期。</summary>
    [JsonPropertyName("id_card_valid_time_begin")]
    public string? IdCardValidTimeBegin { get; set; }

    /// <summary>身份证结束时间（<c>id_card_valid_time</c>）：合法 YYYY-MM-DD 或「长期」，须晚于开始时间。</summary>
    [JsonPropertyName("id_card_valid_time")]
    public string? IdCardValidTime { get; set; }
}

/// <summary>
/// 进件「非身份证」证件信息（<c>id_doc_info</c>）：证件类型不为身份证时使用。
/// </summary>
/// <remarks>
/// <b><see cref="IdDocName"/> 与 <see cref="IdDocNumber"/> 需加密</b>并上送 <c>Wechatpay-Serial</c>。
/// 反面照片（<see cref="IdDocCopyBack"/>）在来往通行证、外国人居留证、港澳/台湾居民居住证时必须，护照不需要。
/// 证件号码官方规范：护照 4-15 位数字|字母|连字符；港澳来往内地通行证 H/M + 8 或 10 位；台湾来往大陆通行证 8 或 10 位数字；
/// 外国人居留证 15 位数字|字母；港澳/台湾居民居住证 17 位数字+1 位数字|X。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentIdDocInfo
{
    /// <summary>证件姓名（<c>id_doc_name</c>，必填 string(100)）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>。</summary>
    [JsonPropertyName("id_doc_name")]
    public string? IdDocName { get; set; }

    /// <summary>证件号码（<c>id_doc_number</c>，必填）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>；格式随证件类型而异。</summary>
    [JsonPropertyName("id_doc_number")]
    public string? IdDocNumber { get; set; }

    /// <summary>证件正面照片（<c>id_doc_copy</c>，string(256) MediaID）：仅 1 张，可为彩色照片/扫描件/加盖鲜章复印件，可加微信支付相关水印。</summary>
    [JsonPropertyName("id_doc_copy")]
    public string? IdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>id_doc_copy_back</c>，选填 string(256) MediaID）：来往通行证/外国人居留证/港澳居住证/台湾居住证需传，护照不需。</summary>
    [JsonPropertyName("id_doc_copy_back")]
    public string? IdDocCopyBack { get; set; }

    /// <summary>证件开始日期（<c>doc_period_begin</c>，必填）：合法 YYYY-MM-DD，不早于 1900-01-01、不大于等于当前日期。</summary>
    [JsonPropertyName("doc_period_begin")]
    public string? DocPeriodBegin { get; set; }

    /// <summary>证件结束日期（<c>doc_period_end</c>，必填）：合法 YYYY-MM-DD 或「长期」，须晚于开始日期。</summary>
    [JsonPropertyName("doc_period_end")]
    public string? DocPeriodEnd { get; set; }
}

/// <summary>
/// 最终受益人信息（<c>ubo_info_list[]</c> 的元素形态）。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>，八字段同集（官方全部标选填）。
/// </para>
/// <para>
/// 官方判定口径：受益所有人须满足至少其一 —— 直接或间接拥有超过 25% 公司股权或表决权；
/// 通过人事、财务等其他方式对公司控制；或用 25% 股权等条件不能识别时的其他合规判定，详见官方原文。
/// 最多上传 4 人，若含经营者/法定代表人须一并填写。
/// </para>
/// <para>
/// <b><see cref="UboIdDocName"/>、<see cref="UboIdDocNumber"/>、<see cref="UboIdDocAddress"/> 三个字段需加密</b>
/// 并上送 <c>Wechatpay-Serial</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentUboInfo
{
    /// <summary>证件类型（<c>ubo_id_doc_type</c>，选填）：见 <see cref="EcommerceApplymentIdDocTypes"/>。</summary>
    [JsonPropertyName("ubo_id_doc_type")]
    public string? UboIdDocType { get; set; }

    /// <summary>证件正面照片（<c>ubo_id_doc_copy</c>，选填 MediaID）：身份证传人像面，仅 1 张。</summary>
    [JsonPropertyName("ubo_id_doc_copy")]
    public string? UboIdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>ubo_id_doc_copy_back</c>，选填 MediaID）：身份证传国徽面，护照无需，仅 1 张。</summary>
    [JsonPropertyName("ubo_id_doc_copy_back")]
    public string? UboIdDocCopyBack { get; set; }

    /// <summary>证件姓名（<c>ubo_id_doc_name</c>，选填 string(100)）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>。</summary>
    [JsonPropertyName("ubo_id_doc_name")]
    public string? UboIdDocName { get; set; }

    /// <summary>证件号码（<c>ubo_id_doc_number</c>，选填）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>；格式随证件类型而异。</summary>
    [JsonPropertyName("ubo_id_doc_number")]
    public string? UboIdDocNumber { get; set; }

    /// <summary>证件居住地址（<c>ubo_id_doc_address</c>，选填）：<b>需加密</b>并上送 <c>Wechatpay-Serial</c>；1-512 字符，按身份证住址填写。</summary>
    [JsonPropertyName("ubo_id_doc_address")]
    public string? UboIdDocAddress { get; set; }

    /// <summary>证件有效期开始时间（<c>ubo_id_doc_period_begin</c>，选填）：合法 YYYY-MM-DD，不早于 1900-01-01、不大于等于当前日期。</summary>
    [JsonPropertyName("ubo_id_doc_period_begin")]
    public string? UboIdDocPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>ubo_id_doc_period_end</c>，选填）：合法 YYYY-MM-DD 或「长期」，须晚于开始时间。</summary>
    [JsonPropertyName("ubo_id_doc_period_end")]
    public string? UboIdDocPeriodEnd { get; set; }
}

/// <summary>
/// 进件结算账户信息（<c>account_info</c>，两支进件均必填）。
/// </summary>
/// <remarks>
/// <para>
/// <b>开户银行取值必须来自查询接口</b>：对私调用「查询支持个人业务的银行列表 API」
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697583"/>）、对公调用
/// 「查询支持对公业务的银行列表 API」（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697626"/>）；
/// 支行侧由「查询支行列表 API」（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697673"/>）取
/// <see cref="BankBranchId"/> 或 <see cref="BankName"/>，官方要求二者<b>二选一</b>。
/// </para>
/// <para>
/// <b>开户名称一致性硬约束</b>：对私须与身份证姓名一致，对公须与营业执照「商户名称」一致；
/// <see cref="AccountName"/>、<see cref="AccountNumber"/> <b>需加密</b>并上送 <c>Wechatpay-Serial</c>。
/// </para>
/// <para>
/// <see cref="BankAddressCode"/> 官方标「即将下线处理，无需传入」；该字段仅标准进件页列出，
/// 账户托管进件页已无该项 ⇒ 共用超集，托管支不填即可（<c>WhenWritingNull</c> 不会发出）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentAccountInfo
{
    /// <summary>账户类型（<c>bank_account_type</c>，必填 string(2)）：<c>74</c>-对公、<c>75</c>-对私，见 <see cref="EcommerceBankAccountTypes"/>；可选性按主体类型分流。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>开户银行（<c>account_bank</c>，必填 string(10)）：须取自「查询支持个人/对公业务的银行列表 API」返回值。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户名称（<c>account_name</c>，必填 string(128)）：<b>需加密</b>；对私须与身份证姓名一致，对公须与营业执照商户名称一致。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>开户银行省市编码（<c>bank_address_code</c>，选填 string(12)）：<b>官方标注「即将下线处理，无需传入」</b>，且仅标准进件页列出。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填 string(64)）：由查询接口「是否需要填写支行」判定；与开户银行全称（含支行）二选一。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>开户银行全称（含支行）（<c>bank_name</c>，选填 string(128)）：与开户银行联行号二选一，须取自查询支行列表 API。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填 string(128)）：<b>需加密</b>；数字，长度遵循对公/对私卡号长度要求表。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }
}

/// <summary>
/// 进件超级管理员信息（<c>contact_info</c>，两支进件均必填）。
/// </summary>
/// <remarks>
/// <para>
/// 官方定位：超级管理员需在开户后签约，可接收日常重要管理信息并进行资金操作，须为商户法定代表人或负责人。
/// <see cref="ContactName"/>、<see cref="MobilePhone"/>、<see cref="ContactIdCardNumber"/> <b>需加密</b>
/// 并上送 <c>Wechatpay-Serial</c>。
/// </para>
/// <para>
/// <b>管理员类型为经办人（<c>66</c>）时</b>才需要 <see cref="ContactIdDocType"/>、
/// <see cref="ContactIdDocCopy"/>、<see cref="ContactIdDocCopyBack"/>、证件有效期起止这一组字段；
/// 类型为 <c>65</c>（经营者/法定代表人）时证件号须与法定代表人身份证一致。
/// </para>
/// <para>
/// 手机号官方规范：前后无空白，须为 11 位手机号码，或 5-20 位数字/连字符“-”/加号“+”。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentContactInfo
{
    /// <summary>超级管理员类型（<c>contact_type</c>，必填 string(2)）：<c>65</c>-经营者/法定代表人、<c>66</c>-经办人，见 <see cref="EcommerceContactTypes"/>；可选性按主体类型分流。</summary>
    [JsonPropertyName("contact_type")]
    public string? ContactType { get; set; }

    /// <summary>超级管理员姓名（<c>contact_name</c>，必填 string(10)）：<b>需加密</b>；类型为法定代表人时须与其身份证姓名一致。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>超级管理员证件类型（<c>contact_id_doc_type</c>，选填）：管理员为经办人时上传，见 <see cref="EcommerceApplymentIdDocTypes"/>。</summary>
    [JsonPropertyName("contact_id_doc_type")]
    public string? ContactIdDocType { get; set; }

    /// <summary>超级管理员证件号码（<c>contact_id_card_number</c>，选填）：<b>需加密</b>；类型为法定代表人时须与其身份证号一致。</summary>
    [JsonPropertyName("contact_id_card_number")]
    public string? ContactIdCardNumber { get; set; }

    /// <summary>超级管理员证件正面照片（<c>contact_id_doc_copy</c>，选填 MediaID）：管理员为经办人时上传，身份证传人像面，仅 1 张。</summary>
    [JsonPropertyName("contact_id_doc_copy")]
    public string? ContactIdDocCopy { get; set; }

    /// <summary>超级管理员证件反面照片（<c>contact_id_doc_copy_back</c>，选填 MediaID）：管理员为经办人时上传，护照不需反面。</summary>
    [JsonPropertyName("contact_id_doc_copy_back")]
    public string? ContactIdDocCopyBack { get; set; }

    /// <summary>超级管理员证件有效期开始时间（<c>contact_id_doc_period_begin</c>，选填）：经办人时上传；YYYY-MM-DD，不早于 1900-01-01、不大于等于当前日期。</summary>
    [JsonPropertyName("contact_id_doc_period_begin")]
    public string? ContactIdDocPeriodBegin { get; set; }

    /// <summary>超级管理员证件有效期结束时间（<c>contact_id_doc_period_end</c>，选填）：经办人时上传；YYYY-MM-DD 或「长期」，须晚于开始时间。</summary>
    [JsonPropertyName("contact_id_doc_period_end")]
    public string? ContactIdDocPeriodEnd { get; set; }

    /// <summary>业务办理授权函（<c>business_authorization_letter</c>，选填 MediaID）：<b>官方标注「即将下线处理，无需传入」</b>，且仅标准进件页列出。</summary>
    [JsonPropertyName("business_authorization_letter")]
    public string? BusinessAuthorizationLetter { get; set; }

    /// <summary>超级管理员手机（<c>mobile_phone</c>，必填）：<b>需加密</b>；11 位手机号或 5-20 位数字/“-”/“+”，用于接收重要管理信息与操作验证码。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }
}

/// <summary>
/// 进件经营场景/店铺信息（<c>sales_scene_info</c>，两支进件的对象本身均必填）。
/// </summary>
/// <remarks>
/// <b>店铺二维码与店铺链接二选一必填</b>（官方原文）：本类两个字段都做成可空，SDK 不代为补默认值，
/// 由宿主保证至少其一非空。
/// <see cref="MiniProgramSubAppid"/> 完成后由系统把二级商户号与该 AppID 绑定（即 <c>sub_appid</c>，发起支付时可传），
/// 认证主体须与二级商户主体一致。
/// 官方在标准进件页把 <see cref="StoreName"/> 标必填、在账户托管进件页标选填 ⇒ 共用超集、可空。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentSalesSceneInfo
{
    /// <summary>店铺名称（<c>store_name</c>，string(256)）：填店铺全称。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>店铺链接（<c>store_url</c>，选填 string(1024)）：可直接访问店铺主页的链接，须以 <c>https://</c> 或 <c>http://</c> 开头、不能含转义字符；与店铺二维码二选一。</summary>
    [JsonPropertyName("store_url")]
    public string? StoreUrl { get; set; }

    /// <summary>店铺二维码（<c>store_qr_code</c>，选填 string(256) MediaID）：电商小程序可传店铺页小程序二维码，仅 1 张；与店铺链接二选一。</summary>
    [JsonPropertyName("store_qr_code")]
    public string? StoreQrCode { get; set; }

    /// <summary>商家小程序 AppID（<c>mini_program_sub_appid</c>，选填 string(256)）：认证主体须与二级商户一致，入驻后系统绑定为 <c>sub_appid</c>。</summary>
    [JsonPropertyName("mini_program_sub_appid")]
    public string? MiniProgramSubAppid { get; set; }
}

/// <summary>
/// 进件结算规则（<c>settlement_info</c>，选填）。
/// </summary>
/// <remarks>
/// 官方：电商平台未传入时系统填默认值（<see cref="SettlementId"/> 默认 0.6% 费率对应规则 id，
/// <see cref="QualificationType"/> 默认无需特殊资质的行业名）。
/// <b>结算规则 ID 须与电商平台「开通工具箱」选择的费率档位匹配</b>，取值见「电商二级商户结算规则对照表」
/// （<see href="https://kf.qq.com/faq/220228qEfuAz220228bMFji6.html"/>）⇒ 硬编码规则 id 会随档位调整而失效。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentSettlementInfo
{
    /// <summary>结算规则 ID（<c>settlement_id</c>，选填 integer）：须匹配开通工具箱选择的费率档位；未传默认 0.6% 档。</summary>
    [JsonPropertyName("settlement_id")]
    public int? SettlementId { get; set; }

    /// <summary>所属行业（<c>qualification_type</c>，选填 string(200)）：映射特殊资质要求；未传默认填无需特殊资质的行业名。</summary>
    [JsonPropertyName("qualification_type")]
    public string? QualificationType { get; set; }
}

/// <summary>
/// 二级商户进件申请（<c>POST /v3/ecommerce/applyments/</c>）与<b>账户托管进件</b>
/// （<c>POST /v3/ecommerce/applyments/submit-account-hosting-applyment</c>）、
/// 两支撤销共用<b>同一应答形态</b>：仅回申请单号与业务申请编号。
/// </summary>
/// <remarks>
/// <para>
/// <b>本应答不含审核结论</b>：进件是异步审核，提交成功只代表申请单已创建，
/// <c>sub_mchid</c> 与状态必须另走 <see cref="EcommerceApplymentQueryResponse"/> 查询。
/// </para>
/// <para>
/// <b>提交签约核身信息</b>（<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>）的应答
/// 只有 <c>applyment_id</c> 一行 ⇒ 与 <see cref="OutRequestNo"/> 属「少一行」差异，共用本类，
/// 托管/撤销两支才保证回传该字段，核身支按官方表不返回。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentSubmitResponse : WechatPayResponse
{
    /// <summary>微信支付申请单号（<c>applyment_id</c>，必填 integer）：由微信支付分配，后续查询/撤销/签约核身都以此或 <see cref="OutRequestNo"/> 定位。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 string(124)）：服务商自定义唯一编号，审核通过后生成微信支付商户号。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }
}

/// <summary>
/// 进件申请单查询应答（22 行）—— <b>四个查询页共用同一形态</b>：
/// 按申请单号（<c>GET /v3/ecommerce/applyments/{applyment_id}</c>，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691469"/>）与
/// 按业务申请编号（<c>GET /v3/ecommerce/applyments/out-request-no/{out_request_no}</c>，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691376"/>）两支，
/// 加上账户托管进件对应的 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616714"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616708"/> 两支，
/// 2026-10-15 逐字段比对结果为<b>完全一致</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>整张表除 <c>applyment_state</c>/<c>applyment_state_desc</c>/<c>out_request_no</c>/<c>applyment_id</c>
/// 之外全为选填</b>，且各字段只在特定状态下出现（见 <see cref="EcommerceApplymentStates"/>）
/// ⇒ 判状态要用枚举比对，不能用「某字段是否为 null」倒推状态。
/// </para>
/// <para>
/// <b>应答里的敏感字段是密文</b>：<see cref="EcommerceApplymentAccountValidation.AccountName"/> 与
/// <see cref="EcommerceApplymentAccountValidation.AccountNo"/> 官方明确要求用 API 证书私钥解密
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4013059060"/>）⇒ SDK 不解密、原样透传字符串，
/// 且这两值不得进日志或异常消息。
/// </para>
/// <para>
/// <see cref="SignUrl"/> 与 <see cref="LegalValidationUrl"/> 官方建议转二维码后由申请人微信扫码打开；
/// 平台代理签约模式下 <c>REJECTED</c> 状态也可能返回 <c>sign_url</c>（配合 <see cref="WxpaySignRequired"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentQueryResponse : WechatPayResponse
{
    /// <summary>申请状态（<c>applyment_state</c>，必填 string(32)）：见 <see cref="EcommerceApplymentStates"/>，本应答的权威进度字段。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>申请状态描述（<c>applyment_state_desc</c>，必填 string(1024)）：中文可读文案，示例「审核中」；仅展示用，判定请用 <see cref="ApplymentState"/>。</summary>
    [JsonPropertyName("applyment_state_desc")]
    public string? ApplymentStateDesc { get; set; }

    /// <summary>签约链接（<c>sign_url</c>，选填 string(256)）：状态为 <c>NEED_SIGN</c> 才返回（平台代理签约模式下 <c>REJECTED</c> 且 <see cref="WxpaySignRequired"/>=true 时也可能返回）。</summary>
    [JsonPropertyName("sign_url")]
    public string? SignUrl { get; set; }

    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，选填 string(32)）：仅 <c>NEED_SIGN</c> 或 <c>FINISH</c> 返回。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>汇款账户验证信息（<c>account_validation</c>，选填）：仅 <c>ACCOUNT_NEED_VERIFY</c> 返回，据此汇款完成账户验证。</summary>
    [JsonPropertyName("account_validation")]
    public EcommerceApplymentAccountValidation? AccountValidation { get; set; }

    /// <summary>驳回原因详情（<c>audit_detail</c>，选填数组）：仅 <c>REJECTED</c> 或 <c>FROZEN</c> 返回，逐项列出资料名称与驳回原因。</summary>
    [JsonPropertyName("audit_detail")]
    public IReadOnlyList<EcommerceApplymentAuditDetail>? AuditDetail { get; set; }

    /// <summary>法人验证链接（<c>legal_validation_url</c>，选填 string(256)）：<c>ACCOUNT_NEED_VERIFY</c> 且通过系统校验的申请单才返回，让法人扫码完成账户验证。</summary>
    [JsonPropertyName("legal_validation_url")]
    public string? LegalValidationUrl { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 string(128)）：提交接口填写的编号。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>微信支付申请单号（<c>applyment_id</c>，必填 integer）：微信支付分配的申请单号。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }

    /// <summary>签约状态（<c>sign_state</c>，选填 string(32)）：见 <see cref="EcommerceApplymentSignStates"/>；主体名称/法人/管理员/主体类型被改动后需重新签约。</summary>
    [JsonPropertyName("sign_state")]
    public string? SignState { get; set; }

    /// <summary>是否要求走微信支付签约模式（<c>wxpay_sign_required</c>，选填 boolean）：平台代理签约模式下由监管与业务风险判定，<b>仅在 <c>REJECTED</c> 时返回</b>；为 true 时须取签约链接让管理员扫码签约。</summary>
    [JsonPropertyName("wxpay_sign_required")]
    public bool? WxpaySignRequired { get; set; }
}

/// <summary>
/// 汇款账户验证信息（<c>account_validation</c>）：申请状态为 <c>ACCOUNT_NEED_VERIFY</c> 时返回。
/// </summary>
/// <remarks>
/// <para>
/// <b>户名与卡号是密文</b>：官方要求按「如何使用 API 证书解密敏感字段」
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4013059060"/>）解密，本类原样透传，
/// 长度 <c>string(1024)</c> 即密文长度 ⇒ 拿密文与明文比对恒不等。
/// </para>
/// <para>
/// <b><c>account_no</c> 只在结算账户为对私时返回</b>（官方原文），故为选填；对私汇款须用该卡号对应的账户。
/// 汇款金额为分（<see cref="PayAmount"/>），且须在 <see cref="Deadline"/> 前完成，备注须按 <see cref="Remark"/> 填写。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentAccountValidation
{
    /// <summary>付款户名（<c>account_name</c>，必填 string(1024)）：<b>密文</b>，需以 API 证书私钥解密；商户须用该户名的账户汇款。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>付款卡号（<c>account_no</c>，选填 string(1024)）：<b>密文</b>；结算账户为对私时才返回。</summary>
    [JsonPropertyName("account_no")]
    public string? AccountNo { get; set; }

    /// <summary>汇款金额（<c>pay_amount</c>，必填 integer）：单位为<b>分</b>，需汇款的精确金额。</summary>
    [JsonPropertyName("pay_amount")]
    public long? PayAmount { get; set; }

    /// <summary>收款卡号（<c>destination_account_number</c>，必填 string(128)）：收款账户的卡号（明文）。</summary>
    [JsonPropertyName("destination_account_number")]
    public string? DestinationAccountNumber { get; set; }

    /// <summary>收款户名（<c>destination_account_name</c>，必填 string(128)）。</summary>
    [JsonPropertyName("destination_account_name")]
    public string? DestinationAccountName { get; set; }

    /// <summary>开户银行（<c>destination_account_bank</c>，必填 string(128)）：收款账户的开户银行名称。</summary>
    [JsonPropertyName("destination_account_bank")]
    public string? DestinationAccountBank { get; set; }

    /// <summary>省市信息（<c>city</c>，必填 string(128)）：收款账户的省市。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>备注信息（<c>remark</c>，必填 string(128)）：商户汇款时需要填写的备注，通常含校验要素，须原文填入。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>汇款截止时间（<c>deadline</c>，必填 string(128)）：须在此时间前完成汇款，逾期验证失效。</summary>
    [JsonPropertyName("deadline")]
    public string? Deadline { get; set; }
}

/// <summary>
/// 单条资料项的审核情况（<c>audit_detail[]</c>）：仅 <c>REJECTED</c> / <c>FROZEN</c> 状态返回。
/// </summary>
/// <remarks>
/// <see cref="ParamName"/> 是提交申请单的资料项名称（官方原文，与请求体字段名对应但不保证逐字相同）
/// ⇒ 用它把驳回原因映射回表单字段时须做名称对照，不要假设等于 JSON 键名。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceApplymentAuditDetail
{
    /// <summary>参数名称（<c>param_name</c>，必填 string(32)）：被驳回的资料项名称。</summary>
    [JsonPropertyName("param_name")]
    public string? ParamName { get; set; }

    /// <summary>驳回原因（<c>reject_reason</c>，必填 string(1024)）：该资料项被驳回的具体原因。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>
/// 账户托管进件申请（<c>POST /v3/ecommerce/applyments/submit-account-hosting-applyment</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>
/// （2026-10-15 逐字段核验，全表 71 行）。
/// </para>
/// <para>
/// <b>与标准进件是两套请求体，不是同一类的两种填法</b>（逐行比对实测）：
/// 本页多出 <see cref="IdCardCompleteStatement"/> 与 <see cref="ProxySignVerifyMode"/>（必填）两个字段，
/// 同时<b>删除</b>标准进件的 <c>owner</c>、<c>account_info.bank_address_code</c>、
/// <c>contact_info.business_authorization_letter</c>（三者官方均已标「即将下线」）；
/// 内层九个对象字段名与类型全同、仅必填性不同 ⇒ 内层共用同一批类型，外层分开建模。
/// </para>
/// <para>
/// <b>必填性整体比标准进件宽松</b>：本页把营业执照扫描件/注册号/商户名称/法人姓名、身份证正反面照片、
/// 证件照片、店铺名称等一律降为选填（官方按主体类型与平台留存资料决定是否必须）；
/// 但身份证/证件的<b>姓名与号码、结算账户三要素、超级管理员姓名与手机</b>仍必填且需加密。
/// </para>
/// <para>
/// <see cref="ProxySignVerifyMode"/> 决定后续流程：为 <c>true</c>（平台代理签约+代理核身）时，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/> 的 <c>pre_sign_time</c>
/// 与 <c>proxy_verify_identity_info</c> 转为必填；为 <c>false</c> 时走微信支付自身签约与核身。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceAccountHostingApplymentRequest
{
    /// <summary>业务申请编号（<c>out_request_no</c>，必填）：服务商自定义唯一编号，一编号对应一申请单；被驳回后可用相同编号覆盖修改原单。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>主体类型（<c>organization_type</c>，必填）：七值与标准进件同集，见 <see cref="EcommerceApplymentOrganizationTypes"/>；注意本页值为数字串而非 <c>SUBJECT_TYPE_*</c>。</summary>
    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }

    /// <summary>是否金融机构（<c>finance_institution</c>，选填 boolean）：未传入按 <c>false</c> 处理。</summary>
    [JsonPropertyName("finance_institution")]
    public bool? FinanceInstitution { get; set; }

    /// <summary>营业执照/登记证书信息（<c>business_license_info</c>，选填）：本页内层字段全为选填。</summary>
    [JsonPropertyName("business_license_info")]
    public EcommerceApplymentBusinessLicenseInfo? BusinessLicenseInfo { get; set; }

    /// <summary>金融机构许可证信息（<c>finance_institution_info</c>，选填）：主体是金融机构时必填。</summary>
    [JsonPropertyName("finance_institution_info")]
    public EcommerceApplymentFinanceInstitutionInfo? FinanceInstitutionInfo { get; set; }

    /// <summary>证件持有人类型（<c>id_holder_type</c>，选填）：见 <see cref="EcommerceApplymentIdHolderTypes"/>；本页 <c>LEGAL</c> 官方标签作「法定代表人」。</summary>
    [JsonPropertyName("id_holder_type")]
    public string? IdHolderType { get; set; }

    /// <summary>经营者/法人证件类型（<c>id_doc_type</c>，选填）：见 <see cref="EcommerceApplymentIdDocTypes"/>；未填默认身份证，政府机关仅支持大陆身份证。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>法定代表人说明函（<c>authorize_letter_copy</c>，选填 MediaID）：持有人为经办人时必须上传。</summary>
    [JsonPropertyName("authorize_letter_copy")]
    public string? AuthorizeLetterCopy { get; set; }

    /// <summary>法人影印件信息辅助填写的平台声明（<c>id_card_complete_statement</c>，选填）：<b>本页特有字段</b>，主体为个体户/企业/事业单位/社会组织/政府机关且法人影印件为空时必填，官方要求传入固定值「客户确认你公司用已留存的法人影印件做主体归集和关联管理」；小微/个人卖家必填经营者影印件，该值无须填。</summary>
    [JsonPropertyName("id_card_complete_statement")]
    public string? IdCardCompleteStatement { get; set; }

    /// <summary>经营者/法人身份证信息（<c>id_card_info</c>，选填）：持有人为经营者/法人且证件类型为身份证时填写。</summary>
    [JsonPropertyName("id_card_info")]
    public EcommerceApplymentIdCardInfo? IdCardInfo { get; set; }

    /// <summary>经营者/法人其他证件信息（<c>id_doc_info</c>，选填）：持有人为经营者/法人且证件类型不为身份证时填写。</summary>
    [JsonPropertyName("id_doc_info")]
    public EcommerceApplymentIdDocInfo? IdDocInfo { get; set; }

    /// <summary>最终受益人信息列表（<c>ubo_info_list</c>，选填数组）：企业或社会组织时按要求填写，最多 4 人。</summary>
    [JsonPropertyName("ubo_info_list")]
    public IReadOnlyList<EcommerceApplymentUboInfo>? UboInfoList { get; set; }

    /// <summary>结算账户信息（<c>account_info</c>，必填）：本页无 <c>bank_address_code</c> 字段。</summary>
    [JsonPropertyName("account_info")]
    public EcommerceApplymentAccountInfo? AccountInfo { get; set; }

    /// <summary>超级管理员信息（<c>contact_info</c>，必填）：本页无 <c>business_authorization_letter</c> 字段。</summary>
    [JsonPropertyName("contact_info")]
    public EcommerceApplymentContactInfo? ContactInfo { get; set; }

    /// <summary>店铺信息（<c>sales_scene_info</c>，必填）：本页内层四字段全为选填，但二维码与链接仍要求二选一。</summary>
    [JsonPropertyName("sales_scene_info")]
    public EcommerceApplymentSalesSceneInfo? SalesSceneInfo { get; set; }

    /// <summary>结算规则（<c>settlement_info</c>，选填）：未传入时系统填默认值。</summary>
    [JsonPropertyName("settlement_info")]
    public EcommerceApplymentSettlementInfo? SettlementInfo { get; set; }

    /// <summary>商户简称（<c>merchant_shortname</c>，必填）：最多约 21 个汉字，支付完成页向买家展示，须与实际售卖商品相符。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortname { get; set; }

    /// <summary>特殊资质（<c>qualifications</c>，选填）：仅所选行业需要时提供，最多 5 张 MediaID。</summary>
    [JsonPropertyName("qualifications")]
    public string? Qualifications { get; set; }

    /// <summary>补充材料（<c>business_addition_pics</c>，选填）：按审核要求提供，本页官方表述为最多 15 张照片的 MediaID。</summary>
    [JsonPropertyName("business_addition_pics")]
    public string? BusinessAdditionPics { get; set; }

    /// <summary>补充说明（<c>business_addition_desc</c>，选填）：主体为「个人卖家」时必传固定声明；被要求补资金来源/资金用途时依次填入。</summary>
    [JsonPropertyName("business_addition_desc")]
    public string? BusinessAdditionDesc { get; set; }

    /// <summary>是否走平台代理签约和代理核身入驻模式（<c>proxy_sign_verify_mode</c>，<b>必填</b> boolean）：<b>本页特有字段</b>；<c>true</c>=平台代理签约+代理核身（随后须提交签约核身信息），<c>false</c>=走微信支付签约与核身。</summary>
    [JsonPropertyName("proxy_sign_verify_mode")]
    public bool? ProxySignVerifyMode { get; set; }
}

/// <summary>
/// 提交签约核身信息（<c>POST /v3/ecommerce/applyments/submit-sign-verify-info</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>
/// （2026-10-15 逐字段核验，全表 4 行）。
/// </para>
/// <para>
/// <b>定位申请单的两个字段二选一</b>：<see cref="OutRequestNo"/> 与 <see cref="ApplymentId"/> 官方均标选填，
/// 原文「和申请单号二选一」/「和业务申请编号二选一」⇒ SDK 不做互斥校验（两端点参数形态不同，
/// 全空由官方报 <c>PARAM_ERROR</c>），但须注意官方同时约束「一个商户同一时间只能存在一笔流程中的申请单，
/// 申请单被驳回请使用原单重试」。
/// </para>
/// <para>
/// <see cref="PreSignTime"/> 与 <see cref="ProxyVerifyIdentityInfo"/> 的条件必填由进件时的
/// <c>proxy_sign_verify_mode=true</c> 决定（见 <see cref="EcommerceAccountHostingApplymentRequest.ProxySignVerifyMode"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceSignVerifyInfoRequest
{
    /// <summary>业务申请编号（<c>out_request_no</c>，选填 string(124)）：与 <see cref="ApplymentId"/> 二选一；被驳回时填相同编号即覆盖修改原申请单。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>申请单号（<c>applyment_id</c>，选填 integer）：微信支付分配的申请单号，与 <see cref="OutRequestNo"/> 二选一。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }

    /// <summary>预签署微信支付协议时间（<c>pre_sign_time</c>，选填）：「走平台代理签约和代理核身入驻模式」时必填；rfc3339 格式 <c>YYYY-MM-DDTHH:mm:ss+TIMEZONE</c>。</summary>
    [JsonPropertyName("pre_sign_time")]
    public string? PreSignTime { get; set; }

    /// <summary>平台代理核身信息（<c>proxy_verify_identity_info</c>，选填）：「走平台代理签约和代理核身入驻模式」时必填。</summary>
    [JsonPropertyName("proxy_verify_identity_info")]
    public EcommerceProxyVerifyIdentityInfo? ProxyVerifyIdentityInfo { get; set; }
}

/// <summary>
/// 平台代理核身信息（<c>proxy_verify_identity_info</c>）：由代理核身模式下的电商平台回填商户核身结果。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>。
/// 两字段官方均标必填，但外层对象本身选填 ⇒ 本类属性做成可空，条件必填性由调用侧按模式保证。
/// <b><see cref="VerifyIdentityResult"/> 为 <c>false</c> 表示核身不通过</b>，官方未定义失败重试的额外入参，
/// 需以进件查询应答的状态为准判断后续动作。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProxyVerifyIdentityInfo
{
    /// <summary>核身方式（<c>verify_identity_type</c>，必填）：见 <see cref="EcommerceVerifyIdentityTypes"/>，面对面/远程二选一。</summary>
    [JsonPropertyName("verify_identity_type")]
    public string? VerifyIdentityType { get; set; }

    /// <summary>核身结果（<c>verify_identity_result</c>，必填 boolean）：<c>true</c>=核身通过，<c>false</c>=核身不通过。</summary>
    [JsonPropertyName("verify_identity_result")]
    public bool? VerifyIdentityResult { get; set; }
}

/// <summary>
/// 提交代理签约核身信息应答（1 行）：<c>POST /v3/ecommerce/applyments/submit-sign-verify-info</c>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>（2026-10-15 核验；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>与进件受理应答（2 行）不共用</b>：本页应答表<b>只有 <c>applyment_id</c> 一行</b>，官方不再回填
/// <c>out_request_no</c>（2026-10-15 逐字段比对：DIFF）⇒ 独立成类型。
/// 本接口入参的 <c>out_request_no</c> 与 <c>applyment_id</c> 是二选一的定位键，而应答只保证回申请单号。
/// </para>
/// <para>
/// <b>本应答不含任何状态字段</b>：官方未回「核身信息是否被采纳」⇒ 提交结果只能回到进件查询
/// （<c>GET /v3/ecommerce/applyments/{applyment_id}</c>）读 <c>applyment_state</c> 判断。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceSignVerifyInfoResponse : WechatPayResponse
{
    /// <summary>微信支付申请单号（<c>applyment_id</c>，必填 integer）：由微信支付分配。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }
}
