// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantAlterApply;

/// <summary>
/// 主体资料变更范围（官方 <c>alter_scope</c>，3 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字核验。</para>
/// <para><b>缺省语义是「全量」而不是「不改动」</b>：官方正文写明「没有选定时，默认本次申请为变更所有主体资料」
/// ⇒ 留空等价于 <see cref="Full"/>，宿主若想只改经营证件必须显式传 <see cref="BusinessCert"/>，
/// 否则会把未填的主体资料一并纳入审核（守卫 MAA-B4 锁定该缺省方向）。</para>
/// </remarks>
public static class MerchantAlterApplyScopes
{
    /// <summary>修改全部主体资料（官方 <c>ALTER_SCOPE_FULL</c>）。</summary>
    public const string Full = "ALTER_SCOPE_FULL";

    /// <summary>仅修改经营证件资料（官方 <c>ALTER_SCOPE_BUSINESS_CERT</c>）。</summary>
    public const string BusinessCert = "ALTER_SCOPE_BUSINESS_CERT";

    /// <summary>仅修改受益人资料（官方 <c>ALTER_SCOPE_UBO</c>）。</summary>
    public const string Ubo = "ALTER_SCOPE_UBO";
}

/// <summary>
/// 主体类型（官方 <c>organization_type</c>，6 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字核验。</para>
/// <para><b>事业单位的取值是 <see cref="InstitutionsCloned"/> 而非 <c>SUBJECT_TYPE_INSTITUTIONS</c></b>
/// （官方原文带 <c>_CLONED</c> 后缀，系从特约商户进件枚举复制时的历史残留，照页保留）。</para>
/// <para><b>与 <c>Apply4Subject</c>（主体进件 / 开户意愿确认）域的枚举不通用</b>：该面另有自己的主体类型值集，
/// 跨域复用常量即错（守卫 MAA-B4 只锁本域 6 值）。</para>
/// </remarks>
public static class MerchantAlterApplySubjectTypes
{
    /// <summary>企业（官方 <c>SUBJECT_TYPE_ENTERPRISE</c>）。</summary>
    public const string Enterprise = "SUBJECT_TYPE_ENTERPRISE";

    /// <summary>事业单位（官方 <c>SUBJECT_TYPE_INSTITUTIONS_CLONED</c>，带 <c>_CLONED</c> 后缀）。</summary>
    public const string InstitutionsCloned = "SUBJECT_TYPE_INSTITUTIONS_CLONED";

    /// <summary>个体工商户（官方 <c>SUBJECT_TYPE_INDIVIDUAL</c>）。</summary>
    public const string Individual = "SUBJECT_TYPE_INDIVIDUAL";

    /// <summary>社会组织（官方 <c>SUBJECT_TYPE_OTHERS</c>）。</summary>
    public const string Others = "SUBJECT_TYPE_OTHERS";

    /// <summary>政府机关（官方 <c>SUBJECT_TYPE_GOVERNMENT</c>）。</summary>
    public const string Government = "SUBJECT_TYPE_GOVERNMENT";

    /// <summary>小微商户或个人卖家（官方 <c>SUBJECT_TYPE_MICRO</c>）。</summary>
    public const string Micro = "SUBJECT_TYPE_MICRO";
}

/// <summary>
/// 金融机构类型（官方 <c>finance_type</c>，6 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字核验。</para>
/// <para>与 <c>Apply4SubMerchant</c>（特约商户进件）域的值集<b>同名同值但各自成类</b>：
/// 进件族「嵌套对象一律不得复用」的裁定同样适用于常量类，跨域引用会把两个契约版本绑成一次改动。</para>
/// </remarks>
public static class MerchantAlterApplyFinanceTypes
{
    /// <summary>银行业（官方 <c>FINANCE_TYPE_BANK_AGENT</c>）。</summary>
    public const string BankAgent = "FINANCE_TYPE_BANK_AGENT";

    /// <summary>支付机构（官方 <c>FINANCE_TYPE_PAYMENT_AGENT</c>）。</summary>
    public const string PaymentAgent = "FINANCE_TYPE_PAYMENT_AGENT";

    /// <summary>保险业（官方 <c>FINANCE_TYPE_INSURANCE</c>）。</summary>
    public const string Insurance = "FINANCE_TYPE_INSURANCE";

    /// <summary>交易及结算类金融机构（官方 <c>FINANCE_TYPE_TRADE_AND_SETTLE</c>）。</summary>
    public const string TradeAndSettle = "FINANCE_TYPE_TRADE_AND_SETTLE";

    /// <summary>小额贷款（官方 <c>FINANCE_TYPE_SMALL_LOAN</c>）。</summary>
    public const string SmallLoan = "FINANCE_TYPE_SMALL_LOAN";

    /// <summary>其他金融机构（官方 <c>FINANCE_TYPE_OTHER</c>）。</summary>
    public const string Other = "FINANCE_TYPE_OTHER";
}

/// <summary>
/// 证件持有人类型（官方 <c>id_holder_type</c>，2 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>，2026-10-14 逐字核验。</para>
/// <para><b>官方取值是 <c>LEGAL</c> / <c>SUPER</c> 两个裸值</b>（不带 <c>ID_HOLDER_TYPE_</c> 前缀，
/// 与本域其余枚举的命名风格不一致，照页保留）；<c>SUPER</c>（经办人）仅政府机关/事业单位可选，
/// 且此时 <c>authorize_letter_copy</c> 转为必填。</para>
/// </remarks>
public static class MerchantAlterApplyIdHolderTypes
{
    /// <summary>经营者/法人（官方 <c>LEGAL</c>）。</summary>
    public const string Legal = "LEGAL";

    /// <summary>经办人（官方 <c>SUPER</c>）：须配 <c>authorize_letter_copy</c>。</summary>
    public const string Super = "SUPER";
}

/// <summary>
/// 证件类型（官方 <c>id_doc_type</c> / <c>contact_id_doc_type</c>，8 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>（法人证件）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617906"/>（超级管理员证件），2026-10-14 逐字核验。</para>
/// <para><b>两族共用本值集，但字段名不同</b>：主体族是 <c>id_doc_type</c>，超管族是
/// <c>contact_id_doc_type</c> ⇒ 值集共用一个常量类、字段名由各 DTO 各自承载（守卫 MAA-B3 锁两处字段名）。</para>
/// </remarks>
public static class MerchantAlterApplyIdDocTypes
{
    /// <summary>中国大陆居民-身份证（官方 <c>IDENTIFICATION_TYPE_IDCARD</c>）。</summary>
    public const string IdCard = "IDENTIFICATION_TYPE_IDCARD";

    /// <summary>其他国家或地区居民-护照（官方 <c>IDENTIFICATION_TYPE_OVERSEA_PASSPORT</c>）。</summary>
    public const string OverseaPassport = "IDENTIFICATION_TYPE_OVERSEA_PASSPORT";

    /// <summary>中国香港居民-来往内地通行证（官方 <c>IDENTIFICATION_TYPE_HONGKONG_PASSPORT</c>）。</summary>
    public const string HongKongPassport = "IDENTIFICATION_TYPE_HONGKONG_PASSPORT";

    /// <summary>中国澳门居民-来往内地通行证（官方 <c>IDENTIFICATION_TYPE_MACAO_PASSPORT</c>）。</summary>
    public const string MacaoPassport = "IDENTIFICATION_TYPE_MACAO_PASSPORT";

    /// <summary>中国台湾居民-来往大陆通行证（官方 <c>IDENTIFICATION_TYPE_TAIWAN_PASSPORT</c>）。</summary>
    public const string TaiwanPassport = "IDENTIFICATION_TYPE_TAIWAN_PASSPORT";

    /// <summary>外国人居留证（官方 <c>IDENTIFICATION_TYPE_FOREIGN_RESIDENT</c>）。</summary>
    public const string ForeignResident = "IDENTIFICATION_TYPE_FOREIGN_RESIDENT";

    /// <summary>港澳居民证（官方 <c>IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT</c>）。</summary>
    public const string HongKongMacaoResident = "IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT";

    /// <summary>台湾居民证（官方 <c>IDENTIFICATION_TYPE_TAIWAN_RESIDENT</c>）。</summary>
    public const string TaiwanResident = "IDENTIFICATION_TYPE_TAIWAN_RESIDENT";
}

/// <summary>
/// 主体族申请单状态（官方 <c>state</c>，<b>6 值</b> string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090655"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090667"/>，2026-10-14 逐字核验。</para>
/// <para><b>主体族比另两族多一档 <see cref="Editting"/></b>：主体族 6 值、超管族与商家简称族各 5 值
/// （两族值集相同，见 <see cref="MerchantAlterApplyContactStates"/> /
/// <see cref="MerchantAlterApplyMerchantNameStates"/>；简称族的撤销页只印 1 值）。
/// <b>仍按族各建一类</b>：判据是「各族页面各自成文的清单」，不是「值集相同就合并」——
/// 合并后任一页扩充取值就会静默放宽另两族的断言（守卫 MAA-B4 三族各自断言）。</para>
/// <para><b>两处官方误拼照页</b>：<c>APPLYMENT_STATE_EDITTING</c>（三个 T）、
/// <c>APPLYMENT_STATE_MODIFING</c>（单 F）⇒ 修正拼写会与应答值不匹配。</para>
/// </remarks>
public static class MerchantAlterApplySubjectStates
{
    /// <summary>编辑中（官方 <c>APPLYMENT_STATE_EDITTING</c>，三 T 误拼照页）：仅主体族有此档。</summary>
    public const string Editting = "APPLYMENT_STATE_EDITTING";

    /// <summary>审核中（官方 <c>APPLYMENT_STATE_AUDITING</c>）。</summary>
    public const string Auditing = "APPLYMENT_STATE_AUDITING";

    /// <summary>已驳回（官方 <c>APPLYMENT_STATE_REJECTED</c>）：此时才回 <c>audit_reject_reason</c>。</summary>
    public const string Rejected = "APPLYMENT_STATE_REJECTED";

    /// <summary>系统修改资料中（官方 <c>APPLYMENT_STATE_MODIFING</c>，单 F 误拼照页）。</summary>
    public const string Modifing = "APPLYMENT_STATE_MODIFING";

    /// <summary>变更完成（官方 <c>APPLYMENT_STATE_FINISHED</c>）。</summary>
    public const string Finished = "APPLYMENT_STATE_FINISHED";

    /// <summary>已撤销（官方 <c>APPLYMENT_STATE_CANCELED</c>）。</summary>
    public const string Canceled = "APPLYMENT_STATE_CANCELED";
}

/// <summary>
/// 超级管理员变更族申请单状态（官方 <c>state</c>，<b>5 值</b> string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617911"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617915"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617919"/>，2026-10-14 逐字核验。</para>
/// <para><b>无 <c>APPLYMENT_STATE_EDITTING</c> 档</b>（主体族有）⇒ 本族「查询」与「撤销」两支的状态值集一致，
/// 与主体族不同集（守卫 MAA-B4）。</para>
/// </remarks>
public static class MerchantAlterApplyContactStates
{
    /// <summary>审核中（官方 <c>APPLYMENT_STATE_AUDITING</c>）。</summary>
    public const string Auditing = "APPLYMENT_STATE_AUDITING";

    /// <summary>已驳回（官方 <c>APPLYMENT_STATE_REJECTED</c>）。</summary>
    public const string Rejected = "APPLYMENT_STATE_REJECTED";

    /// <summary>系统修改资料中（官方 <c>APPLYMENT_STATE_MODIFING</c>，单 F 误拼照页）。</summary>
    public const string Modifing = "APPLYMENT_STATE_MODIFING";

    /// <summary>变更完成（官方 <c>APPLYMENT_STATE_FINISHED</c>）。</summary>
    public const string Finished = "APPLYMENT_STATE_FINISHED";

    /// <summary>已撤销（官方 <c>APPLYMENT_STATE_CANCELED</c>）：撤销支应答即此值。</summary>
    public const string Canceled = "APPLYMENT_STATE_CANCELED";
}

/// <summary>
/// 商家简称变更族申请单状态（官方 <c>state</c>，查询支 <b>5 值</b>、撤销支页内只印 <b>1 值</b>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166074"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166085"/>（5 值清单）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166090"/>（撤销页只列
/// <c>APPLYMENT_STATE_CANCELED</c>），2026-10-14 逐字核验。</para>
/// <para><b>「撤销页只印一值」不作为建模依据</b>：该页是查询族的特例应答，官方并未声明其余四值非法
/// ⇒ SDK 仍按 5 值集承载，撤销支不另建窄枚举（守卫 MAA-B4 断言本域只有三个状态常量类）。</para>
/// </remarks>
public static class MerchantAlterApplyMerchantNameStates
{
    /// <summary>审核中（官方 <c>APPLYMENT_STATE_AUDITING</c>）。</summary>
    public const string Auditing = "APPLYMENT_STATE_AUDITING";

    /// <summary>已驳回（官方 <c>APPLYMENT_STATE_REJECTED</c>）。</summary>
    public const string Rejected = "APPLYMENT_STATE_REJECTED";

    /// <summary>系统修改资料中（官方 <c>APPLYMENT_STATE_MODIFING</c>，单 F 误拼照页）。</summary>
    public const string Modifing = "APPLYMENT_STATE_MODIFING";

    /// <summary>变更完成（官方 <c>APPLYMENT_STATE_FINISHED</c>）。</summary>
    public const string Finished = "APPLYMENT_STATE_FINISHED";

    /// <summary>已撤销（官方 <c>APPLYMENT_STATE_CANCELED</c>）。</summary>
    public const string Canceled = "APPLYMENT_STATE_CANCELED";
}

/// <summary>
/// 主体族撤销申请的应答状态（官方 <c>result</c>，2 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090672"/>，2026-10-14 逐字核验。</para>
/// <para><b>本族的撤销应答是 <c>result</c> 而不是 <c>state</c></b>：超管族与简称族的撤销支回
/// <c>{apply_id, state}</c>，主体族回 <c>{result}</c>（<c>SUCCESS</c> / <c>FAIL</c>）⇒ 两支应答形态不同、
/// 不得共用类型（守卫 MAA-B6）。官方同时限定「仅编辑中/审核中/已驳回三态可撤销」，
/// 其余状态下 <c>result</c> 为 <c>FAIL</c> 而非 HTTP 错误。</para>
/// </remarks>
public static class MerchantAlterApplyRevokeResults
{
    /// <summary>撤销成功（官方 <c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>撤销失败（官方 <c>FAIL</c>）：申请单已不在可撤销状态。</summary>
    public const string Fail = "FAIL";
}
