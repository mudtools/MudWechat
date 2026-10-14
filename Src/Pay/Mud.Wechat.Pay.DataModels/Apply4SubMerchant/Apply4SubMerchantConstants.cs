// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

/// <summary>
/// 进件申请单状态（官方 <c>applyment_state</c>，8 值）。
/// </summary>
/// <remarks>
/// 取值与语义照官方「申请单号查询申请状态」页原文
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697052"/>）：
/// <c>EDITTING</c> 系「提交申请发生错误导致」⇒ 需重新提交，并非用户可停留的中间态。
/// </remarks>
public static class Apply4SubApplymentStates
{
    /// <summary>编辑中（<c>APPLYMENT_STATE_EDITTING</c>）：提交发生错误，请尝试重新提交。</summary>
    public const string Editting = "APPLYMENT_STATE_EDITTING";

    /// <summary>审核中（<c>APPLYMENT_STATE_AUDITING</c>）：申请单正在审核中。</summary>
    public const string Auditing = "APPLYMENT_STATE_AUDITING";

    /// <summary>已驳回（<c>APPLYMENT_STATE_REJECTED</c>）：按 <c>audit_detail</c> 修改资料后重新提交。</summary>
    public const string Rejected = "APPLYMENT_STATE_REJECTED";

    /// <summary>待账户验证（<c>APPLYMENT_STATE_TO_BE_CONFIRMED</c>）：超级管理员用 <c>sign_url</c> 完成账户验证。</summary>
    public const string ToBeConfirmed = "APPLYMENT_STATE_TO_BE_CONFIRMED";

    /// <summary>待签约（<c>APPLYMENT_STATE_TO_BE_SIGNED</c>）：超级管理员用 <c>sign_url</c> 完成签约。</summary>
    public const string ToBeSigned = "APPLYMENT_STATE_TO_BE_SIGNED";

    /// <summary>开通权限中（<c>APPLYMENT_STATE_SIGNING</c>）：系统开通相关权限中，请耐心等待。</summary>
    public const string Signing = "APPLYMENT_STATE_SIGNING";

    /// <summary>已完成（<c>APPLYMENT_STATE_FINISHED</c>）：商户入驻申请已完成，<c>sub_mchid</c> 此时才出现。</summary>
    public const string Finished = "APPLYMENT_STATE_FINISHED";

    /// <summary>已作废（<c>APPLYMENT_STATE_CANCELED</c>）：申请单已被撤销。</summary>
    public const string Canceled = "APPLYMENT_STATE_CANCELED";
}

/// <summary>主体类型（官方进件 <c>subject_type</c>，5 值）。</summary>
/// <remarks>须与营业执照 / 登记证书一致（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>）。</remarks>
public static class Apply4SubSubjectTypes
{
    /// <summary>个体工商户（<c>SUBJECT_TYPE_INDIVIDUAL</c>）。</summary>
    public const string Individual = "SUBJECT_TYPE_INDIVIDUAL";

    /// <summary>企业（<c>SUBJECT_TYPE_ENTERPRISE</c>）。</summary>
    public const string Enterprise = "SUBJECT_TYPE_ENTERPRISE";

    /// <summary>政府机关（<c>SUBJECT_TYPE_GOVERNMENT</c>）。</summary>
    public const string Government = "SUBJECT_TYPE_GOVERNMENT";

    /// <summary>事业单位（<c>SUBJECT_TYPE_INSTITUTIONS</c>）。</summary>
    public const string Institutions = "SUBJECT_TYPE_INSTITUTIONS";

    /// <summary>社会组织（<c>SUBJECT_TYPE_OTHERS</c>）。</summary>
    public const string Others = "SUBJECT_TYPE_OTHERS";
}

/// <summary>
/// 证件 / 超级管理员持有人类型（官方 <c>contact_type</c> 与 <c>id_holder_type</c> 共用同一组取值）。
/// </summary>
public static class Apply4SubIdHolderTypes
{
    /// <summary>经营者 / 法定代表人（<c>LEGAL</c>）。</summary>
    public const string Legal = "LEGAL";

    /// <summary>经办人（<c>SUPER</c>）：经商户授权办理微信支付业务的人员。</summary>
    public const string Super = "SUPER";
}

/// <summary>证件类型（官方 <c>id_doc_type</c> / <c>contact_id_doc_type</c> / <c>ubo_id_doc_type</c>，8 值）。</summary>
public static class Apply4SubIdDocTypes
{
    /// <summary>中国大陆居民-身份证（<c>IDENTIFICATION_TYPE_IDCARD</c>）。</summary>
    public const string IdCard = "IDENTIFICATION_TYPE_IDCARD";

    /// <summary>其他国家或地区居民-护照（<c>IDENTIFICATION_TYPE_OVERSEA_PASSPORT</c>）。</summary>
    public const string OverSeaPassport = "IDENTIFICATION_TYPE_OVERSEA_PASSPORT";

    /// <summary>中国香港居民-来往内地通行证（<c>IDENTIFICATION_TYPE_HONGKONG_PASSPORT</c>）。</summary>
    public const string HongKongPassport = "IDENTIFICATION_TYPE_HONGKONG_PASSPORT";

    /// <summary>中国澳门居民-来往内地通行证（<c>IDENTIFICATION_TYPE_MACAO_PASSPORT</c>）。</summary>
    public const string MacaoPassport = "IDENTIFICATION_TYPE_MACAO_PASSPORT";

    /// <summary>中国台湾居民-来往大陆通行证（<c>IDENTIFICATION_TYPE_TAIWAN_PASSPORT</c>）。</summary>
    public const string TaiwanPassport = "IDENTIFICATION_TYPE_TAIWAN_PASSPORT";

    /// <summary>外国人居留证（<c>IDENTIFICATION_TYPE_FOREIGN_RESIDENT</c>）。</summary>
    public const string ForeignResident = "IDENTIFICATION_TYPE_FOREIGN_RESIDENT";

    /// <summary>港澳居民居住证（<c>IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT</c>）。</summary>
    public const string HongKongMacaoResident = "IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT";

    /// <summary>台湾居民居住证（<c>IDENTIFICATION_TYPE_TAIWAN_RESIDENT</c>）。</summary>
    public const string TaiwanResident = "IDENTIFICATION_TYPE_TAIWAN_RESIDENT";
}

/// <summary>
/// 登记证书类型（官方 <c>cert_type</c>）。
/// </summary>
/// <remarks>
/// 主体为「个体工商户 / 企业」时<b>不填</b>；政府机关仅 <c>CERTIFICATE_TYPE_2389</c>，
/// 事业单位仅 <c>CERTIFICATE_TYPE_2388</c>，社会组织可取多值
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>）。
/// </remarks>
public static class Apply4SubCertificateTypes
{
    /// <summary>事业单位法人证书（<c>CERTIFICATE_TYPE_2388</c>）。</summary>
    public const string InstitutionsLegalPerson = "CERTIFICATE_TYPE_2388";

    /// <summary>统一社会信用代码证书（<c>CERTIFICATE_TYPE_2389</c>）：政府机关 / 社会组织。</summary>
    public const string UnifiedCreditCode = "CERTIFICATE_TYPE_2389";

    /// <summary>社会团体法人登记证书（<c>CERTIFICATE_TYPE_2394</c>）。</summary>
    public const string SocialGroupLegalPerson = "CERTIFICATE_TYPE_2394";

    /// <summary>民办非企业单位登记证书（<c>CERTIFICATE_TYPE_2395</c>）。</summary>
    public const string PrivateNonEnterprise = "CERTIFICATE_TYPE_2395";

    /// <summary>基金会法人登记证书（<c>CERTIFICATE_TYPE_2396</c>）。</summary>
    public const string FoundationLegalPerson = "CERTIFICATE_TYPE_2396";

    /// <summary>宗教活动场所登记证（<c>CERTIFICATE_TYPE_2399</c>）。</summary>
    public const string ReligiousActivitySite = "CERTIFICATE_TYPE_2399";

    /// <summary>政府部门下发的其他有效证明文件（<c>CERTIFICATE_TYPE_2400</c>）。</summary>
    public const string OtherGovernmentFile = "CERTIFICATE_TYPE_2400";

    /// <summary>执业许可证 / 执业证（<c>CERTIFICATE_TYPE_2520</c>）。</summary>
    public const string PracticeLicense = "CERTIFICATE_TYPE_2520";

    /// <summary>基层群众性自治组织特别法人统一社会信用代码证（<c>CERTIFICATE_TYPE_2521</c>）。</summary>
    public const string GrassrootsAutonomyOrganization = "CERTIFICATE_TYPE_2521";

    /// <summary>农村集体经济组织登记证（<c>CERTIFICATE_TYPE_2522</c>）。</summary>
    public const string RuralCollectiveEconomicOrganization = "CERTIFICATE_TYPE_2522";
}

/// <summary>金融机构类型（官方进件 <c>finance_institution_info.finance_type</c>，6 值）。</summary>
public static class Apply4SubFinanceTypes
{
    /// <summary>银行业（<c>BANK_AGENT</c>）：商业银行、政策性银行、农村合作银行、村镇银行、开发性金融机构等。</summary>
    public const string BankAgent = "BANK_AGENT";

    /// <summary>支付机构（<c>PAYMENT_AGENT</c>）：非银行类支付机构。</summary>
    public const string PaymentAgent = "PAYMENT_AGENT";

    /// <summary>保险业（<c>INSURANCE</c>）：保险、保险中介、保险代理、保险经纪等。</summary>
    public const string Insurance = "INSURANCE";

    /// <summary>交易及结算类金融机构（<c>TRADE_AND_SETTLE</c>）：交易所、登记结算类机构、银行卡清算机构、资金清算中心等。</summary>
    public const string TradeAndSettle = "TRADE_AND_SETTLE";

    /// <summary>小额贷款机构（<c>SMALL_LOAN</c>）：取得小额贷款业务经营资质、不吸收公众存款的地方金融组织。</summary>
    public const string SmallLoan = "SMALL_LOAN";

    /// <summary>其他金融机构（<c>OTHER</c>）：财务公司、信托、金融资产管理、金融租赁、汽车金融、消费金融、证券等。</summary>
    public const string Other = "OTHER";
}

/// <summary>经营方式 / 销售场景类型（官方进件 <c>sales_info.sales_scenes_type</c>，6 值，数组）。</summary>
public static class Apply4SubSalesScenesTypes
{
    /// <summary>线下场所（<c>SALES_SCENES_STORE</c>）。</summary>
    public const string Store = "SALES_SCENES_STORE";

    /// <summary>服务号与公众号（<c>SALES_SCENES_MP</c>）。</summary>
    public const string Mp = "SALES_SCENES_MP";

    /// <summary>小程序（<c>SALES_SCENES_MINI_PROGRAM</c>）。</summary>
    public const string MiniProgram = "SALES_SCENES_MINI_PROGRAM";

    /// <summary>互联网网站（<c>SALES_SCENES_WEB</c>）。</summary>
    public const string Web = "SALES_SCENES_WEB";

    /// <summary>App（<c>SALES_SCENES_APP</c>）。</summary>
    public const string App = "SALES_SCENES_APP";

    /// <summary>企业微信（<c>SALES_SCENES_WEWORK</c>）。</summary>
    public const string WeWork = "SALES_SCENES_WEWORK";
}

/// <summary>进件结算账户类型（官方进件 <c>bank_account_info.bank_account_type</c>，仅两值）。</summary>
/// <remarks>主体为企业 / 政府机关 / 事业单位 / 社会组织时<b>只能</b>对公；个体户可对公或对私。</remarks>
public static class Apply4SubBankAccountTypes
{
    /// <summary>对公银行账户（<c>BANK_ACCOUNT_TYPE_CORPORATE</c>）。</summary>
    public const string Corporate = "BANK_ACCOUNT_TYPE_CORPORATE";

    /// <summary>经营者个人银行卡（<c>BANK_ACCOUNT_TYPE_PERSONAL</c>）。</summary>
    public const string Personal = "BANK_ACCOUNT_TYPE_PERSONAL";
}

/// <summary>
/// 结算账户类型（官方「修改结算账户 / 查询结算账户」的 <c>account_type</c>，仅两值）。
/// </summary>
/// <remarks>
/// <b>与进件的 <see cref="Apply4SubBankAccountTypes"/> 是两套取值</b>：本组前缀为
/// <c>ACCOUNT_TYPE_</c>，进件侧为 <c>BANK_ACCOUNT_TYPE_</c> —— 官方原文如此，<b>不得</b>统一。
/// </remarks>
public static class Apply4SubSettlementAccountTypes
{
    /// <summary>对公银行账户（<c>ACCOUNT_TYPE_BUSINESS</c>）。</summary>
    public const string Business = "ACCOUNT_TYPE_BUSINESS";

    /// <summary>经营者个人银行卡（<c>ACCOUNT_TYPE_PRIVATE</c>）。</summary>
    public const string Private = "ACCOUNT_TYPE_PRIVATE";
}

/// <summary>结算账户验证结果（官方「查询结算账户」的 <c>verify_result</c>，3 值）。</summary>
public static class Apply4SubSettlementVerifyResults
{
    /// <summary>验证成功，该账户可正常发起提现（<c>VERIFY_SUCCESS</c>）。</summary>
    public const string Success = "VERIFY_SUCCESS";

    /// <summary>验证失败，该账户无法发起提现（<c>VERIFY_FAIL</c>）。</summary>
    public const string Fail = "VERIFY_FAIL";

    /// <summary>验证中，商户可发起提现尝试（<c>VERIFYING</c>）。</summary>
    public const string Verifying = "VERIFYING";
}

/// <summary>
/// 结算账户<b>修改申请</b>审核状态（官方「查询结算账户修改申请状态」的 <c>verify_result</c>，3 值）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="Apply4SubSettlementVerifyResults"/> 不同名也不同值</b>：本组是「申请单审核」
/// （<c>AUDIT_*</c>），前者是「账户验证」；同名字段 <c>verify_result</c> 在两页语义不同，勿合并成一个类型。
/// </remarks>
public static class Apply4SubSettlementAuditResults
{
    /// <summary>审核成功（<c>AUDIT_SUCCESS</c>）。</summary>
    public const string Success = "AUDIT_SUCCESS";

    /// <summary>审核中（<c>AUDITING</c>）：修改结算账户为异步审核，须轮询本状态。</summary>
    public const string Auditing = "AUDITING";

    /// <summary>审核驳回（<c>AUDIT_FAIL</c>）：<c>verify_fail_reason</c> 给出具体原因。</summary>
    public const string Fail = "AUDIT_FAIL";
}

/// <summary>银行账号展示规则（官方 <c>account_number_rule</c>，2 值，不传默认 <c>V1</c>）。</summary>
public static class Apply4SubAccountNumberRules
{
    /// <summary>掩码 v1（<c>ACCOUNT_NUMBER_RULE_MASK_V1</c>）：保留前 2 位和最后 2 位，中间以 * 代替。</summary>
    public const string MaskV1 = "ACCOUNT_NUMBER_RULE_MASK_V1";

    /// <summary>掩码 v2（<c>ACCOUNT_NUMBER_RULE_MASK_V2</c>）：保留前 6 位和最后 4 位，中间以 * 代替。</summary>
    public const string MaskV2 = "ACCOUNT_NUMBER_RULE_MASK_V2";
}

/// <summary>进件域官方业务错误码（字符串码，配合 <c>WechatPayException.PayErrorCode</c> 判定）。</summary>
/// <remarks>
/// 取自「申请单号查询申请状态」与「修改结算账户」页错误码表原文
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697052"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761102"/>）。
/// <b><c>FREQENCY_LIMIT</c> 少一个字母 U 系官方原文拼写，不得「纠正」</b>（同 PAY-B5 字段名纪律）。
/// </remarks>
public static class Apply4SubMerchantErrorCodes
{
    /// <summary>400 <c>APPLYMENT_NOT_EXIST</c>：申请单不存在。</summary>
    public const string ApplymentNotExist = "APPLYMENT_NOT_EXIST";

    /// <summary>400 <c>PROCESSING</c>：存在流程进行中的申请单（同 <c>business_code</c> 重入）。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>404 <c>ORDER_NOT_EXIST</c>：查询的修改结算账户申请单不存在。</summary>
    public const string OrderNotExist = "ORDER_NOT_EXIST";

    /// <summary>429 <c>FREQENCY_LIMIT</c>：频率超限（官方原文拼写如此）。</summary>
    public const string FrequencyLimit = "FREQENCY_LIMIT";

    /// <summary>403 <c>RATE_LIMITED</c>：查询频率限制（官方状态码在部分页面为 403）。</summary>
    public const string RateLimited = "RATE_LIMITED";
}
