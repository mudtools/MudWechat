// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantFund;

/// <summary>
/// 商户账户类型（官方 <c>account_type</c>，3 值 string，落在 path 段而非 query/body）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012721477"/>（查询账户实时余额）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012721446"/>（查询账户日终余额），2026-10-14 逐字核验。</para>
/// <para><b>三值之外无第四账户</b>：官方两页的取值字典只印 <c>BASIC</c> / <c>OPERATION</c> / <c>FEES</c> 三档，
/// 传未开通的账户类型官方回 <b>400 <c>INVALID_REQUEST</c></b>（「输入账户类型未开通」）而非 404
/// ⇒ 宿主把它当「路由不存在」重试不会收敛（守卫 MF-B4 锁三值清单，MF-B5 锁「未开通是 400 语义」）。</para>
/// <para><b>本枚举属于账户余额面</b>：提现族（<c>/v3/merchant/fund/withdraw</c>）另有 <c>account_type</c>
/// 但整套页面属【平台商户】面 ⇒ 本包零端点（守卫 MF-B2 带正对照），不复用本常量类。</para>
/// </remarks>
public static class MerchantFundAccountTypes
{
    /// <summary>基本账户（官方 <c>BASIC</c>）。</summary>
    public const string Basic = "BASIC";

    /// <summary>运营账户（官方 <c>OPERATION</c>）。</summary>
    public const string Operation = "OPERATION";

    /// <summary>手续费账户（官方 <c>FEES</c>）。</summary>
    public const string Fees = "FEES";
}

/// <summary>
/// 账户资金收入记录类型（官方 <c>income_record_type</c>，2 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012718647"/>（商户号维度）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012718620"/>（服务商维度），2026-10-18 逐字核验。</para>
/// <para><b>两值对应两种资金来源</b>：<c>OFFLINERECHARGE</c> 线下充值、<c>ENTERPRISEDIRECTREVENUE</c>
/// 企业直连收入 ⇒ 汇总时须按类型分组，混为一个口径会掩盖资金来源差异。</para>
/// </remarks>
public static class MerchantFundIncomeRecordTypes
{
    /// <summary>线下充值（官方 <c>OFFLINERECHARGE</c>）。</summary>
    public const string OfflineRecharge = "OFFLINERECHARGE";

    /// <summary>企业直连收入（官方 <c>ENTERPRISEDIRECTREVENUE</c>）。</summary>
    public const string EnterpriseDirectRevenue = "ENTERPRISEDIRECTREVENUE";
}
