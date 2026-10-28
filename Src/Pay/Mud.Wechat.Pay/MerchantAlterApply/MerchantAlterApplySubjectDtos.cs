// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantAlterApply;

/// <summary>
/// 主体资料变更申请的包体（官方 <c>POST /v3/mchalterapply/mchsubjectalterapplyment</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>「选填」全是条件必填</b>：官方把六个资料对象都标选填，条件写在描述里——
/// <c>alter_scope</c> 未指定「仅受益人资料」时 <c>organization_type</c> / <c>finance_institution</c> 必填，
/// 主体类型为企业/个体户时 <see cref="BusinessLicenseInfo"/> 必填，为政府机关/事业单位/社会组织时
/// <see cref="CertificateInfo"/> 必填，是金融机构时 <see cref="FinanceInstitutionInfo"/> 必填，
/// <see cref="LegalPersonInfo"/> 在同条件下必填 ⇒ SDK 不做客户端校验（条件组合有八种，误判即误拒），
/// 由官方以 <c>PARAM_ERROR</c> 拒绝（守卫 MAA-B5 锁定该「不建模条件校验」裁决）。</para>
/// <para><b>幂等与覆盖语义</b>：同一 <c>out_request_no</c> 在申请单被驳回后重提是<b>覆盖原申请单</b>，
/// 且「一个商户同一时间只能存在一笔流程中的申请单」⇒ 换新 <c>out_request_no</c> 会产生第二笔单，
/// 不是「重试」而是「重复申请」。</para>
/// <para><b>本支是加密头 <c>Wechatpay-Serial</c> 的表行必填支</b>：<see cref="LegalPersonInfo"/> 与
/// <see cref="UboInfoList"/> 内的姓名/证件号/住址须以微信支付公钥（推荐）或平台证书加密后传入，
/// 密文仍是 string ⇒ DTO 只承载密文，加密由宿主与传输层协同。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectSubmitRequest
{
    /// <summary>商户号（<c>merchant_code</c>，必填 string(32)）：本服务商负责进件的特约商户/二级商户号。</summary>
    [JsonPropertyName("merchant_code")]
    public string? MerchantCode { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 string(64)）：服务商自定义唯一编号；驳回后同号重提即覆盖原单。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>变更范围（<c>alter_scope</c>，选填 string）：见 <see cref="MerchantAlterApplyScopes"/>；缺省=变更全部主体资料。</summary>
    [JsonPropertyName("alter_scope")]
    public string? AlterScope { get; set; }

    /// <summary>主体类型（<c>organization_type</c>，选填 string）：见 <see cref="MerchantAlterApplySubjectTypes"/>。</summary>
    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }

    /// <summary>是否金融机构（<c>finance_institution</c>，选填 boolean）。</summary>
    [JsonPropertyName("finance_institution")]
    public bool? FinanceInstitution { get; set; }

    /// <summary>营业执照信息（<c>business_license_info</c>，选填 object）：主体为企业/个体户时必填。</summary>
    [JsonPropertyName("business_license_info")]
    public MerchantAlterApplySubjectBusinessLicenseInfo? BusinessLicenseInfo { get; set; }

    /// <summary>登记证书信息（<c>certificate_info</c>，选填 object）：主体为政府机关/事业单位/社会组织时必填。</summary>
    [JsonPropertyName("certificate_info")]
    public MerchantAlterApplySubjectCertificateInfo? CertificateInfo { get; set; }

    /// <summary>金融机构许可证（<c>finance_institution_info</c>，选填 object）：主体是金融机构时必填。</summary>
    [JsonPropertyName("finance_institution_info")]
    public MerchantAlterApplySubjectFinanceInstitutionInfo? FinanceInstitutionInfo { get; set; }

    /// <summary>法人身份信息（<c>legal_person_info</c>，选填 object）：变更范围未指定「仅受益人」时必填。</summary>
    [JsonPropertyName("legal_person_info")]
    public MerchantAlterApplySubjectLegalPersonInfo? LegalPersonInfo { get; set; }

    /// <summary>最终受益人信息列表（<c>ubo_info_list</c>，选填 array[UboInfo]）：企业/社会组织最多 4 个，法人已是受益人时最多 3 个。</summary>
    [JsonPropertyName("ubo_info_list")]
    public List<MerchantAlterApplySubjectUboInfo>? UboInfoList { get; set; }

    /// <summary>补充材料列表（<c>addition</c>，选填 object）：仅在官方审核额外要求时提交。</summary>
    [JsonPropertyName("addition")]
    public MerchantAlterApplySubjectAddition? Addition { get; set; }
}

/// <summary>
/// 主体变更的营业执照信息（官方 <c>business_license_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>有效期是 <c>license_period_begin</c> / <c>license_period_end</c> 双字段</b>，
/// 与特约商户进件（<c>Apply4SubMerchant</c>）面的 <c>*_valid_date</c> 单字段体系不同 ⇒ 两域对象不得复用
/// （方案 §2.7 四-10 的「进件族嵌套对象一律不得复用」裁定）。</para>
/// <para><b>字段名是 <c>license</c>（美式）</b>，而登记证书面的编号字段是 <c>cert_number</c>；
/// 本域十页内无 <c>licence</c> 拼写，照页即可，不得按英式「统一」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectBusinessLicenseInfo
{
    /// <summary>营业执照注册号（<c>license_number</c>，必填 string(32)）：统一社会信用代码须 18 位且以 9 开头。</summary>
    [JsonPropertyName("license_number")]
    public string? LicenseNumber { get; set; }

    /// <summary>营业执照照片（<c>license_copy</c>，必填 string(256)）：填《图片上传 API》返回的 MediaID。</summary>
    [JsonPropertyName("license_copy")]
    public string? LicenseCopy { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，必填 string(128)）：2-128 字符，不得仅含数字或特殊字符。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>法人姓名（<c>legal_person</c>，必填 string(100)）：营业执照的经营者/法定代表人姓名。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>注册地址（<c>company_address</c>，<b>本对象内选填</b> string(512)）：留空时官方查国家工商信息填入。</summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>有效期开始时间（<c>license_period_begin</c>，选填 string(128)）：<c>yyyy-MM-dd</c>，不得早于 1900-01-01。</summary>
    [JsonPropertyName("license_period_begin")]
    public string? LicensePeriodBegin { get; set; }

    /// <summary>有效期结束时间（<c>license_period_end</c>，选填 string(128)）：<c>yyyy-MM-dd</c> 或关键字「长期」。</summary>
    [JsonPropertyName("license_period_end")]
    public string? LicensePeriodEnd { get; set; }
}

/// <summary>
/// 主体变更的登记证书信息（官方 <c>certificate_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与营业执照对象同名三字段的必填性不同</b>：<c>company_address</c> 在本对象是<b>必填</b>
/// （营业执照面是选填）、有效期两字段亦必填 ⇒ 两个对象不得合并成一个类型。</para>
/// <para><b>本对象带两个已废弃枚举值</b>：<c>CERTIFICATE_TYPE_2397</c>（慈善组织公开募捐资格证书）与
/// <c>CERTIFICATE_TYPE_2398</c>（农民专业合作社法人营业执照）官方标注「已废弃」⇒ 值清单不建模为常量类
/// （守卫 MAA-B4），宿主按页原文传串即可。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectCertificateInfo
{
    /// <summary>证书类型（<c>cert_type</c>，必填 string）：政府机关/事业单位/社会组织时上传，企业与个体户不填。</summary>
    [JsonPropertyName("cert_type")]
    public string? CertType { get; set; }

    /// <summary>证书编号（<c>cert_number</c>，必填 string(32)）：须与所选证书类型匹配且符合国家标准规范。</summary>
    [JsonPropertyName("cert_number")]
    public string? CertNumber { get; set; }

    /// <summary>证书照片（<c>cert_copy</c>，必填 string(256)）：填《图片上传 API》返回的 MediaID。</summary>
    [JsonPropertyName("cert_copy")]
    public string? CertCopy { get; set; }

    /// <summary>商户名称（<c>merchant_name</c>，必填 string(128)）：登记证书上的名称。</summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>法人姓名（<c>legal_person</c>，必填 string(100)）：登记证书上的法定代表人姓名。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>注册地址（<c>company_address</c>，<b>必填</b> string(512)）：营业执照面同名字段为选填，两处不可互换。</summary>
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }

    /// <summary>有效期开始时间（<c>cert_period_begin</c>，必填 string(128)）：<c>yyyy-MM-dd</c>。</summary>
    [JsonPropertyName("cert_period_begin")]
    public string? CertPeriodBegin { get; set; }

    /// <summary>有效期结束时间（<c>cert_period_end</c>，必填 string(128)）：<c>yyyy-MM-dd</c> 或关键字「长期」。</summary>
    [JsonPropertyName("cert_period_end")]
    public string? CertPeriodEnd { get; set; }
}

/// <summary>
/// 主体变更的金融机构许可证信息（官方 <c>finance_institution_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本对象是全域唯一与特约商户进件面同名的嵌套对象</b>（<c>finance_type</c> + <c>finance_license_pics</c>），
/// 即便如此也<b>不复用</b> <c>Apply4SubMerchant</c> 的类型：值集虽同，两域契约各自演进
/// （方案 §2.7 四-10），复用会把两个官方页面绑成一次改动。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectFinanceInstitutionInfo
{
    /// <summary>金融机构类型（<c>finance_type</c>，必填 string）：见 <see cref="MerchantAlterApplyFinanceTypes"/>。</summary>
    [JsonPropertyName("finance_type")]
    public string? FinanceType { get; set; }

    /// <summary>金融机构许可证图片（<c>finance_license_pics</c>，必填 array[string]）：最多 5 张 MediaID。</summary>
    [JsonPropertyName("finance_license_pics")]
    public List<string>? FinanceLicensePics { get; set; }
}

/// <summary>
/// 主体变更的法人身份信息（官方 <c>legal_person_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>字段名体系是 <c>card_*</c>，与特约商户进件面的 <c>identification_*</c> 分家</b>
/// （同一语义在两域不同名 ⇒ 跨域复制 DTO 会静默丢字段，守卫 MAA-B3 两侧断言）。</para>
/// <para><b><c>card_name</c> / <c>card_number</c> / <c>card_address</c> 三字段须加密</b>：
/// 官方逐字段注明「该字段需进行加密处理」，且本支把 <c>Wechatpay-Serial</c> 列为<b>请求头表行必填</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectLegalPersonInfo
{
    /// <summary>证件持有人类型（<c>id_holder_type</c>，必填 string）：见 <see cref="MerchantAlterApplyIdHolderTypes"/>。</summary>
    [JsonPropertyName("id_holder_type")]
    public string? IdHolderType { get; set; }

    /// <summary>证件类型（<c>id_doc_type</c>，选填 string）：持有人为经营者/法人时填写，见 <see cref="MerchantAlterApplyIdDocTypes"/>。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>法定代表人说明函（<c>authorize_letter_copy</c>，选填 string(256)）：持有人为经办人时必填。</summary>
    [JsonPropertyName("authorize_letter_copy")]
    public string? AuthorizeLetterCopy { get; set; }

    /// <summary>证件正面照片（<c>card_front</c>，选填 string(256)）：持有人为法人时上传。</summary>
    [JsonPropertyName("card_front")]
    public string? CardFront { get; set; }

    /// <summary>证件反面照片（<c>card_back</c>，选填 string(256)）：护照类型无需上传。</summary>
    [JsonPropertyName("card_back")]
    public string? CardBack { get; set; }

    /// <summary>证件姓名（<c>card_name</c>，选填 string(100)）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("card_name")]
    public string? CardName { get; set; }

    /// <summary>证件号码（<c>card_number</c>，选填 string(512)）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("card_number")]
    public string? CardNumber { get; set; }

    /// <summary>证件居住地址（<c>card_address</c>，选填 string(512)）：持有人为法人且主体为企业时填写，<b>须加密</b>。</summary>
    [JsonPropertyName("card_address")]
    public string? CardAddress { get; set; }

    /// <summary>证件有效期开始时间（<c>card_period_begin</c>，选填 string(128)）：<c>yyyy-MM-dd</c>。</summary>
    [JsonPropertyName("card_period_begin")]
    public string? CardPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>card_period_end</c>，选填 string(128)）：<c>yyyy-MM-dd</c> 或关键字「长期」。</summary>
    [JsonPropertyName("card_period_end")]
    public string? CardPeriodEnd { get; set; }

    /// <summary>经营者/法人是否为受益人（<c>as_ubo</c>，选填 boolean）：主体类型为企业时需要填写。</summary>
    [JsonPropertyName("as_ubo")]
    public bool? AsUbo { get; set; }
}

/// <summary>
/// 主体变更的最终受益人信息（官方 <c>ubo_info_list[]</c> 列表项）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与 <see cref="MerchantAlterApplySubjectLegalPersonInfo"/> 是两套必填性</b>：本类型无
/// <c>id_holder_type</c> / <c>authorize_letter_copy</c> / <c>as_ubo</c> 三字段，且 <c>card_back</c> 之外的
/// 姓名/号码/住址/有效期全部转<b>必填</b> ⇒ 两个对象不得共用（守卫 MAA-B6）。</para>
/// <para><b>官方在本项描述里保留了法人面的说明文</b>（「当证件持有人类型为法人时…」），
/// 系文档残留，不影响字段名与必填性 ⇒ 按表行承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectUboInfo
{
    /// <summary>证件类型（<c>id_doc_type</c>，必填 string）：见 <see cref="MerchantAlterApplyIdDocTypes"/>。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>证件正面照片（<c>card_front</c>，必填 string(256)）。</summary>
    [JsonPropertyName("card_front")]
    public string? CardFront { get; set; }

    /// <summary>证件反面照片（<c>card_back</c>，选填 string(256)）：护照类型无需上传。</summary>
    [JsonPropertyName("card_back")]
    public string? CardBack { get; set; }

    /// <summary>证件姓名（<c>card_name</c>，必填 string(100)）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("card_name")]
    public string? CardName { get; set; }

    /// <summary>证件号码（<c>card_number</c>，必填 string(512)）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("card_number")]
    public string? CardNumber { get; set; }

    /// <summary>证件居住地址（<c>card_address</c>，必填 string(512)）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("card_address")]
    public string? CardAddress { get; set; }

    /// <summary>证件有效期开始时间（<c>card_period_begin</c>，必填 string(128)）：<c>yyyy-MM-dd</c>。</summary>
    [JsonPropertyName("card_period_begin")]
    public string? CardPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>card_period_end</c>，必填 string(128)）：<c>yyyy-MM-dd</c> 或关键字「长期」。</summary>
    [JsonPropertyName("card_period_end")]
    public string? CardPeriodEnd { get; set; }
}

/// <summary>
/// 主体变更的补充材料（官方 <c>addition</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字段核验。</para>
/// <para><b>五组材料都是 array[string] 的 MediaID 列表、每组最多 5 张</b>，且官方全部标选填
/// （只在「实际审核额外要求」时使用）⇒ 本对象整体缺省是正常路径，不是漏填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectAddition
{
    /// <summary>银行开户许可证（<c>bank_openaccount_license</c>，选填 array[string]）：最多 5 张 MediaID。</summary>
    [JsonPropertyName("bank_openaccount_license")]
    public List<string>? BankOpenaccountLicense { get; set; }

    /// <summary>开户核准通知书（<c>openaccount_approval</c>，选填 array[string]）：最多 5 张 MediaID。</summary>
    [JsonPropertyName("openaccount_approval")]
    public List<string>? OpenaccountApproval { get; set; }

    /// <summary>法人身份辅助证明（<c>legal_other_prove</c>，选填 array[string]）：最多 5 张 MediaID。</summary>
    [JsonPropertyName("legal_other_prove")]
    public List<string>? LegalOtherProve { get; set; }

    /// <summary>发证机关证明（<c>agency_prove</c>，选填 array[string]）：最多 5 张 MediaID。</summary>
    [JsonPropertyName("agency_prove")]
    public List<string>? AgencyProve { get; set; }

    /// <summary>受益所有人补充材料（<c>ubo_prove</c>，选填 array[string]）：受益人证件不足以核实时提交。</summary>
    [JsonPropertyName("ubo_prove")]
    public List<string>? UboProve { get; set; }
}

/// <summary>
/// 主体变更申请单的查询应答（<b>按申请单号与按「商户号 + 业务申请编号」两支共用</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090655"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090667"/>，2026-10-14 逐字段核验。</para>
/// <para><b>两页应答表逐字段相同</b>（字段名、必填性、长度、<c>state</c> 的 6 值清单全同）⇒ 按「表相同则共用」
/// 归并为一型（守卫 MAA-B6）。<b>与超管族的查询应答分建</b>：两页字段名与长度完全相同，
/// 唯一差异是 <c>state</c> 的值清单（本族 6 值含 <c>APPLYMENT_STATE_EDITTING</c>、该族 5 值）
/// ⇒ 值集属字段表的一部分，按「表不同则分建」处理，各族一型（方案 §2.7 四-9）。</para>
/// <para><b>「提交成功」不等于「变更完成」</b>：本域是审核流，唯一闭环路径是轮询 <see cref="State"/>
/// 到 <see cref="MerchantAlterApplySubjectStates.Finished"/>；<c>MODIFING</c> 档是「系统改资料中」，
/// 此时旧资料仍在生效。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectQueryResponse : WechatPayResponse
{
    /// <summary>商户号（<c>merchant_code</c>，必填 string(32)）。</summary>
    [JsonPropertyName("merchant_code")]
    public string? MerchantCode { get; set; }

    /// <summary>申请单号（<c>apply_id</c>，必填 string(64)）：微信支付分配。</summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>申请单状态（<c>state</c>，必填 string）：本族 6 值，见 <see cref="MerchantAlterApplySubjectStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总体驳回原因（<c>audit_reject_reason</c>，选填 string(1024)）：仅 <c>REJECTED</c> 状态返回。</summary>
    [JsonPropertyName("audit_reject_reason")]
    public string? AuditRejectReason { get; set; }

    /// <summary>逐项驳回明细（<c>audit_reject_detail</c>，选填 array[object]）：与 <c>audit_reject_reason</c> 同时返回。</summary>
    [JsonPropertyName("audit_reject_detail")]
    public List<MerchantAlterApplyAuditRejectDetail>? AuditRejectDetail { get; set; }
}

/// <summary>
/// 主体族撤销申请的应答（官方 <c>result</c> 单字段）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090672"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本族撤销回 <c>result</c>，另两族回 <c>{apply_id, state}</c></b>（见
/// <see cref="MerchantAlterApplyRevokeResponse"/>）⇒ 三族两支「撤销」不是同一契约，
/// 按族分建（守卫 MAA-B6）。撤销失败也是 HTTP 200 + <c>result=FAIL</c>，不是错误码。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubjectRevokeResponse : WechatPayResponse
{
    /// <summary>撤销状态（<c>result</c>，必填 string(10)）：见 <see cref="MerchantAlterApplyRevokeResults"/>。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }
}

/// <summary>
/// 驳回逐项明细（官方 <c>audit_reject_detail[]</c> 列表项，主体族与超管族两支查询共用）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090655"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617911"/>，2026-10-14 逐字段核验。</para>
/// <para><b>两族该列表的字段表逐字符相同</b>（<c>param_name</c> string(128) 选填 +
/// <c>reject_reason</c> string(1024) 选填）⇒ 共用一型；与特约商户进件域的
/// <c>Apply4SubAuditDetail</c> 虽同名不同源，<b>不复用</b>（跨域复用把两页契约绑成一次改动）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyAuditRejectDetail
{
    /// <summary>被驳回的字段名（<c>param_name</c>，选填 string(128)）：官方按报文路径给出，如 <c>business_license_info.merchant_name</c>。</summary>
    [JsonPropertyName("param_name")]
    public string? ParamName { get; set; }

    /// <summary>该字段的驳回原因（<c>reject_reason</c>，选填 string(1024)）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}
