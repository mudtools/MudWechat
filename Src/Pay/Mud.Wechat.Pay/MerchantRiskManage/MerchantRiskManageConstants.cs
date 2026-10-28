// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantRiskManage;

/// <summary>
/// 风险共建面的交易风险类型（官方 <c>risk_type</c>，5 值 string）。
/// </summary>
/// <remarks>
/// <para><b>本域有两套同名不同型的 <c>risk_type</c>，绝不可互相赋值或合并常量类</b>：
/// 风险订单同步面（<c>ec-trade-risk/query-risk-trades</c>）的 <c>risk_type</c> 是 <b>integer</b>
/// （官方行内枚举 <c>1-涉嫌赌博,2-涉嫌欺诈,3-涉嫌色情,4-涉嫌洗钱</c>，共 4 值）；
/// 风险共建面（<c>trade-risk-information</c> / <c>trade-risk-result</c>）的 <c>risk_type</c> 是
/// <b>string</b>（本类 5 值）⇒ 按名字跨族复用会让整型面收到字符串、静默丢字段（守卫 MRB-B3）。</para>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467535"/>（值清单内联印在描述单元格，2026-10-14 逐字核验）。</para>
/// </remarks>
public static class MerchantRiskManageTradeRiskTypes
{
    /// <summary>其它风险（官方 <c>OTHER_RISK</c>）。</summary>
    public const string OtherRisk = "OTHER_RISK";

    /// <summary>涉嫌诱导支付（官方 <c>INDUCE_PAYMENT</c>）。</summary>
    public const string InducePayment = "INDUCE_PAYMENT";

    /// <summary>涉嫌欺诈（官方 <c>FRAUD</c>）。</summary>
    public const string Fraud = "FRAUD";

    /// <summary>涉嫌赌博（官方 <c>GAMBLING</c>）。</summary>
    public const string Gambling = "GAMBLING";

    /// <summary>涉嫌色情（官方 <c>CYBERSEX</c>）。</summary>
    public const string Cybersex = "CYBERSEX";
}

/// <summary>
/// 风险共建面的处置手段（官方 <c>punish_type</c>，10 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467535"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012467540"/>，2026-10-14 逐字核验。</para>
/// <para><b>「查询风险信息」应答与「处理结果回传」请求共用本值集</b>，但必填性不同：
/// 查询侧 <c>punish_type</c> 选填、回传侧<b>必填</b>。</para>
/// <para><b>选 <see cref="OtherPunish"/> 时官方要求把实际手段写进 <c>additional_punish_type</c></b>
/// （该字段是 string(255) 自由文本，回传页示例形态为 <c>"SUSPEND_SETTLEMENT_NDAYS:10"</c>，
/// 官方<b>未给</b>该补充串的取值字典）⇒ 不得据此建枚举。</para>
/// </remarks>
public static class MerchantRiskManagePunishTypes
{
    /// <summary>其它处罚（官方 <c>OTHER_PUNISH</c>）：须配 <c>additional_punish_type</c> 说明。</summary>
    public const string OtherPunish = "OTHER_PUNISH";

    /// <summary>关闭支付（官方 <c>DISABLE_PAYMENT</c>）。</summary>
    public const string DisablePayment = "DISABLE_PAYMENT";

    /// <summary>延长结算周期 T+N（官方 <c>SUSPEND_SETTLEMENT</c>）。</summary>
    public const string SuspendSettlement = "SUSPEND_SETTLEMENT";

    /// <summary>关闭提现（官方 <c>DISABLE_WITHDRAW</c>）。</summary>
    public const string DisableWithdraw = "DISABLE_WITHDRAW";

    /// <summary>清退商户（官方 <c>REMOVE_MERCHANTS</c>）。</summary>
    public const string RemoveMerchants = "REMOVE_MERCHANTS";

    /// <summary>加入商户黑名单（官方 <c>ADD_MERCHANTS_BLACKLIST</c>）。</summary>
    public const string AddMerchantsBlacklist = "ADD_MERCHANTS_BLACKLIST";

    /// <summary>订单退款（官方 <c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";

    /// <summary>冻结单笔订单资金（官方 <c>FREEZING_FUND_BY_TRANSACTION</c>）。</summary>
    public const string FreezingFundByTransaction = "FREEZING_FUND_BY_TRANSACTION";

    /// <summary>冻结商户资金（官方 <c>FREEZING_FUND_BY_MERCHANTS</c>）。</summary>
    public const string FreezingFundByMerchants = "FREEZING_FUND_BY_MERCHANTS";

    /// <summary>封禁用户账号（官方 <c>BLOCKED_USER</c>）。</summary>
    public const string BlockedUser = "BLOCKED_USER";
}
