// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4Subject;

/// <summary>
/// 主体进件申请单状态（官方 <c>applyment_state</c>，8 值）。
/// </summary>
/// <remarks>
/// <para>
/// 取值照「查询申请单审核结果」页原文
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697715"/>）。
/// </para>
/// <para>
/// <b>⚠️ 与特约商户进件的 8 值状态集只有 <c>EDITTING</c> / <c>REJECTED</c> / <c>CANCELED</c> 三值重合</b>：
/// 本域用 <c>WAITTING_FOR_AUDIT</c> / <c>WAITTING_FOR_CONFIRM_CONTACT</c> /
/// <c>WAITTING_FOR_CONFIRM_LEGALPERSON</c> / <c>PASSED</c> / <c>REJECTED</c> / <c>FREEZED</c>，
/// 进件域用 <c>AUDITING</c> / <c>TO_BE_CONFIRMED</c> / <c>TO_BE_SIGNED</c> / <c>SIGNING</c> /
/// <c>FINISHED</c> / <c>REJECTED</c> ⇒ <b>不存在可写的状态映射表</b>，两域流程判定必须各写各的。
/// </para>
/// <para>
/// <b>拼写陷阱（官方原文，不得纠正）</b>：<c>EDITTING</c> 双写 T、<c>WAITTING</c> 双写 T、
/// <c>FREEZED</c> 非 <c>FROZEN</c> —— 守卫 A4SJ-B5 逐值锁定。
/// </para>
/// </remarks>
public static class Apply4SubjectApplymentStates
{
    /// <summary>编辑中（<c>APPLYMENT_STATE_EDITTING</c>）：官方语义为「提交申请发生错误导致」，需重新提交。</summary>
    public const string Editting = "APPLYMENT_STATE_EDITTING";

    /// <summary>审核中（<c>APPLYMENT_STATE_WAITTING_FOR_AUDIT</c>）：申请单正在审核中，非终态。</summary>
    public const string WaittingForAudit = "APPLYMENT_STATE_WAITTING_FOR_AUDIT";

    /// <summary>待确认联系信息（<c>APPLYMENT_STATE_WAITTING_FOR_CONFIRM_CONTACT</c>）：商家扫码 <c>qrcode_data</c> 确认联系人。</summary>
    public const string WaittingForConfirmContact = "APPLYMENT_STATE_WAITTING_FOR_CONFIRM_CONTACT";

    /// <summary>待账户验证（<c>APPLYMENT_STATE_WAITTING_FOR_CONFIRM_LEGALPERSON</c>）：需扫码完成开户意愿 / 账户验证。</summary>
    public const string WaittingForConfirmLegalperson = "APPLYMENT_STATE_WAITTING_FOR_CONFIRM_LEGALPERSON";

    /// <summary>审核通过（<c>APPLYMENT_STATE_PASSED</c>）：仍需商家扫码确认，非「可交易」。</summary>
    public const string Passed = "APPLYMENT_STATE_PASSED";

    /// <summary>审核驳回（<c>APPLYMENT_STATE_REJECTED</c>）：读 <c>reject_param</c> + <c>reject_reason</c> 后改资料重提。</summary>
    public const string Rejected = "APPLYMENT_STATE_REJECTED";

    /// <summary>已冻结（<c>APPLYMENT_STATE_FREEZED</c>，官方拼写如此）：冻结原因见 <c>reject_param</c> / <c>reject_reason</c>。</summary>
    public const string Freezed = "APPLYMENT_STATE_FREEZED";

    /// <summary>已作废（<c>APPLYMENT_STATE_CANCELED</c>）：申请单已被撤销，终态。</summary>
    public const string Canceled = "APPLYMENT_STATE_CANCELED";
}

/// <summary>
/// 商户开户意愿确认（授权）状态（官方 <c>authorize_state</c>，2 值）。
/// </summary>
/// <remarks>
/// 取自「获取商户开户意愿确认状态」页原文
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467549"/>）；
/// 判的是<b>特约商户号</b>的实名认证进度，与 <see cref="Apply4SubjectApplymentStates"/> 是两个独立维度。
/// </remarks>
public static class Apply4SubjectAuthorizeStates
{
    /// <summary>未授权（<c>AUTHORIZE_STATE_UNAUTHORIZED</c>）：特约商户号还未完成实名认证。</summary>
    public const string Unauthorized = "AUTHORIZE_STATE_UNAUTHORIZED";

    /// <summary>已授权（<c>AUTHORIZE_STATE_AUTHORIZED</c>）：特约商户号已完成实名认证。</summary>
    public const string Authorized = "AUTHORIZE_STATE_AUTHORIZED";
}

/// <summary>
/// 主体类型（官方 <c>subject_type</c>，6 值）。
/// </summary>
/// <remarks>
/// <b>⚠️ 比特约商户进件多一个「小微商户」，且事业单位的取值不同</b>：本域事业单位是
/// <c>SUBJECT_TYPE_INSTITUTIONS_CLONED</c>（官方原文即带 <c>_CLONED</c> 后缀，系其内部复制产物，
/// <b>不得</b>「纠正」为 <c>SUBJECT_TYPE_INSTITUTIONS</c> —— 那是进件域的取值），
/// 本域无进件域的 <c>SUBJECT_TYPE_INSTITUTIONS</c>。照抄另一域的取值会直接 400。
/// </remarks>
public static class Apply4SubjectSubjectTypes
{
    /// <summary>企业（<c>SUBJECT_TYPE_ENTERPRISE</c>）：营业执照主体类型一般为有限公司、有限责任公司。</summary>
    public const string Enterprise = "SUBJECT_TYPE_ENTERPRISE";

    /// <summary>个体工商户（<c>SUBJECT_TYPE_INDIVIDUAL</c>）：营业执照一般为个体户、个体工商户、个体经营。</summary>
    public const string Individual = "SUBJECT_TYPE_INDIVIDUAL";

    /// <summary>事业单位（<c>SUBJECT_TYPE_INSTITUTIONS_CLONED</c>，官方原文拼写）：含医疗、教育、学校等单位。</summary>
    public const string InstitutionsCloned = "SUBJECT_TYPE_INSTITUTIONS_CLONED";

    /// <summary>政府机关（<c>SUBJECT_TYPE_GOVERNMENT</c>）：各级各类政府机关，如机关党委、税务、民政、人社、工商、商务、市监等。</summary>
    public const string Government = "SUBJECT_TYPE_GOVERNMENT";

    /// <summary>社会组织（<c>SUBJECT_TYPE_OTHERS</c>）：社会团体、民办非企业、基金会、基层群众性自治组织、农村集体经济组织等。</summary>
    public const string Others = "SUBJECT_TYPE_OTHERS";

    /// <summary>小微商户（<c>SUBJECT_TYPE_MICRO</c>）：无营业执照、免办理工商注册登记的实体商户；须填 <c>assist_prove_info</c>。</summary>
    public const string Micro = "SUBJECT_TYPE_MICRO";
}

/// <summary>
/// 持有人类型（<c>contact_info.contact_type</c> 与 <c>identification_info.id_holder_type</c> 共用同一组取值）。
/// </summary>
/// <remarks>
/// 官方取值为<b>裸值</b> <c>LEGAL</c> / <c>SUPER</c>（无 <c>APPLYMENT_</c> 之类前缀），照原文。
/// 「经办人（SUPER）」= 经商户授权办理微信支付业务的人员。
/// </remarks>
public static class Apply4SubjectHolderTypes
{
    /// <summary>经营者 / 法定代表人（<c>LEGAL</c>）。</summary>
    public const string Legal = "LEGAL";

    /// <summary>经办人（<c>SUPER</c>）：经商户授权办理微信支付业务的人员。</summary>
    public const string Super = "SUPER";
}

/// <summary>
/// 证件类型（官方 <c>contact_id_doc_type</c> / <c>identification_type</c> / <c>ubo_id_doc_type</c> 三处共用同一组取值，8 值）。
/// </summary>
/// <remarks>
/// 取自「提交申请单」页原文（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>）；
/// 政府机关与小微商户的法定代表人证件<b>仅支持</b> <c>IDENTIFICATION_TYPE_IDCARD</c>。
/// 与特约商户进件同组取值但两域各自建类（命名空间不同，守卫各自断言）。
/// </remarks>
public static class Apply4SubjectIdDocTypes
{
    /// <summary>中国大陆居民-身份证（<c>IDENTIFICATION_TYPE_IDCARD</c>）。</summary>
    public const string IdCard = "IDENTIFICATION_TYPE_IDCARD";

    /// <summary>其他国家或地区居民-护照（<c>IDENTIFICATION_TYPE_OVERSEA_PASSPORT</c>）。</summary>
    public const string OverseaPassport = "IDENTIFICATION_TYPE_OVERSEA_PASSPORT";

    /// <summary>中国香港居民-来往内地通行证（<c>IDENTIFICATION_TYPE_HONGKONG_PASSPORT</c>）。</summary>
    public const string HongkongPassport = "IDENTIFICATION_TYPE_HONGKONG_PASSPORT";

    /// <summary>中国澳门居民-来往内地通行证（<c>IDENTIFICATION_TYPE_MACAO_PASSPORT</c>）。</summary>
    public const string MacaoPassport = "IDENTIFICATION_TYPE_MACAO_PASSPORT";

    /// <summary>中国台湾居民-来往大陆通行证（<c>IDENTIFICATION_TYPE_TAIWAN_PASSPORT</c>）。</summary>
    public const string TaiwanPassport = "IDENTIFICATION_TYPE_TAIWAN_PASSPORT";

    /// <summary>外国人居留证（<c>IDENTIFICATION_TYPE_FOREIGN_RESIDENT</c>）。</summary>
    public const string ForeignResident = "IDENTIFICATION_TYPE_FOREIGN_RESIDENT";

    /// <summary>港澳居民居住证（<c>IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT</c>）。</summary>
    public const string HongkongMacaoResident = "IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT";

    /// <summary>台湾居民居住证（<c>IDENTIFICATION_TYPE_TAIWAN_RESIDENT</c>）。</summary>
    public const string TaiwanResident = "IDENTIFICATION_TYPE_TAIWAN_RESIDENT";
}

/// <summary>
/// 登记证书类型（官方 <c>cert_type</c>，10 值，取值为 <c>CERTIFICATE_TYPE_&lt;四位代码&gt;</c>）。
/// </summary>
/// <remarks>
/// 取自「提交申请单」页原文；<b>证书类型与主体类型严格绑定</b>：事业单位只有
/// <c>_2388</c>，政府机关只有 <c>_2389</c>，社会组织可选其余 8 个 ——
/// 代码号（2388 等）是官方字典值，不是可推导的编号。
/// </remarks>
public static class Apply4SubjectCertificateTypes
{
    /// <summary>事业单位法人证书（<c>CERTIFICATE_TYPE_2388</c>，事业单位）。</summary>
    public const string InstitutionsLegalPersonLicense = "CERTIFICATE_TYPE_2388";

    /// <summary>统一社会信用代码证书（<c>CERTIFICATE_TYPE_2389</c>，政府机关 / 社会组织）。</summary>
    public const string UnifiedSocialCreditCodeLicense = "CERTIFICATE_TYPE_2389";

    /// <summary>社会团体法人登记证书（<c>CERTIFICATE_TYPE_2394</c>，社会组织）。</summary>
    public const string SocialOrganizationLegalPersonLicense = "CERTIFICATE_TYPE_2394";

    /// <summary>民办非企业单位登记证书（<c>CERTIFICATE_TYPE_2395</c>，社会组织）。</summary>
    public const string PrivateNonEnterpriseUnitLicense = "CERTIFICATE_TYPE_2395";

    /// <summary>基金会法人登记证书（<c>CERTIFICATE_TYPE_2396</c>，社会组织）。</summary>
    public const string FoundationLicense = "CERTIFICATE_TYPE_2396";

    /// <summary>宗教活动场所登记证（<c>CERTIFICATE_TYPE_2399</c>，社会组织）。</summary>
    public const string ReligiousVenueLicense = "CERTIFICATE_TYPE_2399";

    /// <summary>政府部门下发的其他有效证明文件（<c>CERTIFICATE_TYPE_2400</c>，社会组织）。</summary>
    public const string OtherGovernmentCertificate = "CERTIFICATE_TYPE_2400";

    /// <summary>执业许可证 / 执业证（<c>CERTIFICATE_TYPE_2520</c>，社会组织）。</summary>
    public const string PracticeLicense = "CERTIFICATE_TYPE_2520";

    /// <summary>基层群众性自治组织特别法人统一社会信用代码证（<c>CERTIFICATE_TYPE_2521</c>，社会组织）。</summary>
    public const string GrassrootsAutonomyOrganization = "CERTIFICATE_TYPE_2521";

    /// <summary>农村集体经济组织登记证（<c>CERTIFICATE_TYPE_2522</c>，社会组织）。</summary>
    public const string RuralCollectiveEconomicOrganization = "CERTIFICATE_TYPE_2522";
}

/// <summary>
/// 小微经营类型（官方 <c>assist_prove_info.micro_biz_type</c>，3 值）。
/// </summary>
/// <remarks>取值决定同块内 <c>store_name</c> / <c>store_address</c> 的<b>填写语义</b>（官方原文），而非字段是否必填。</remarks>
public static class Apply4SubjectMicroBizTypes
{
    /// <summary>门店场所（<c>MICRO_TYPE_STORE</c>）。</summary>
    public const string Store = "MICRO_TYPE_STORE";

    /// <summary>流动经营 / 便民服务（<c>MICRO_TYPE_MOBILE</c>）：<c>store_address</c> 官方要求填「无」。</summary>
    public const string Mobile = "MICRO_TYPE_MOBILE";

    /// <summary>线上商品 / 服务交易（<c>MICRO_TYPE_ONLINE</c>）：<c>store_address</c> 官方要求填电商平台名称。</summary>
    public const string Online = "MICRO_TYPE_ONLINE";
}

/// <summary>
/// 金融机构类型（官方 <c>finance_institution_info.finance_type</c>，6 值）。
/// </summary>
/// <remarks>须与营业执照 / 登记证书一致；取值与特约商户进件的金融类型同名，但本域独立建类。</remarks>
public static class Apply4SubjectFinanceTypes
{
    /// <summary>银行业（<c>BANK_AGENT</c>）：商业银行、政策性银行、农村合作银行、村镇银行、开发性金融机构等。</summary>
    public const string BankAgent = "BANK_AGENT";

    /// <summary>支付机构（<c>PAYMENT_AGENT</c>）：非银行类支付机构。</summary>
    public const string PaymentAgent = "PAYMENT_AGENT";

    /// <summary>保险业（<c>INSURANCE</c>）：保险、保险中介、保险代理、保险经纪等。</summary>
    public const string Insurance = "INSURANCE";

    /// <summary>交易及结算类金融机构（<c>TRADE_AND_SETTLE</c>）：交易所、登记结算类机构、银行卡清算机构、资金清算中心等。</summary>
    public const string TradeAndSettle = "TRADE_AND_SETTLE";

    /// <summary>小额贷款机构（<c>SMALL_LOAN</c>）：取得小额贷款业务经营资质、不吸收公众存款、主要经营小额贷款业务的地方金融组织。</summary>
    public const string SmallLoan = "SMALL_LOAN";

    /// <summary>其他金融机构（<c>OTHER</c>）：财务公司、信托、金融资产管理、金融租赁、汽车金融、消费金融、证券、期货、担保、商业保理、典当行等。</summary>
    public const string Other = "OTHER";
}
