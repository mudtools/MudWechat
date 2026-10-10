// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PayScore;

/// <summary>
/// 支付分服务订单状态（官方 <c>state</c>，5 值）。
/// </summary>
/// <remarks>
/// <b>官方原文</b>：创建成功为 <c>CREATED</c>；官方要求消费侧<b>参考「支付分订单状态流转图」</b>
/// 做业务逻辑处理 ⇒ 状态机不得凭直觉跳转（本表只锁取值，流转规则以官方流转图为准）。
/// </remarks>
public static class PayScoreServiceOrderStates
{
    /// <summary>已创建（官方 <c>CREATED</c>）：创单成功后的初始状态。</summary>
    public const string Created = "CREATED";

    /// <summary>进行中（官方 <c>DOING</c>）。</summary>
    public const string Doing = "DOING";

    /// <summary>已完成（官方 <c>DONE</c>）。</summary>
    public const string Done = "DONE";

    /// <summary>已撤销（官方 <c>REVOKED</c>）：取消订单后的终态。</summary>
    public const string Revoked = "REVOKED";

    /// <summary>已过期（官方 <c>EXPIRED</c>）。</summary>
    public const string Expired = "EXPIRED";
}

/// <summary>
/// 订单状态说明（官方 <c>state_description</c>）。
/// </summary>
/// <remarks>
/// 官方创建页原文以「如 <c>USER_CONFIRM</c>／<c>MCH_COMPLETE</c>」举例，并注明该字段
/// <b>仅 <c>DOING</c> 状态返回</b>；本表只收录官方明确点名的两个取值。
/// </remarks>
public static class PayScoreStateDescriptions
{
    /// <summary>用户已确认（官方 <c>USER_CONFIRM</c>）。</summary>
    public const string UserConfirm = "USER_CONFIRM";

    /// <summary>商户已完结（官方 <c>MCH_COMPLETE</c>）。</summary>
    public const string MchComplete = "MCH_COMPLETE";
}

/// <summary>
/// 服务风险金名称（官方 <c>risk_fund.name</c>）。
/// </summary>
/// <remarks>
/// 官方按两种模式分组给出：<b>先免模式</b>（DEPOSIT / ADVANCE / CASH_DEPOSIT）与
/// <b>先享模式</b>（ESTIMATE_ORDER_COST）—— 不可跨模式混用。
/// </remarks>
public static class PayScoreRiskFundNames
{
    /// <summary>押金（官方 <c>DEPOSIT</c>，先免模式）。</summary>
    public const string Deposit = "DEPOSIT";

    /// <summary>预付款（官方 <c>ADVANCE</c>，先免模式）。</summary>
    public const string Advance = "ADVANCE";

    /// <summary>保证金（官方 <c>CASH_DEPOSIT</c>，先免模式）。</summary>
    public const string CashDeposit = "CASH_DEPOSIT";

    /// <summary>预估订单费用（官方 <c>ESTIMATE_ORDER_COST</c>，先享模式）。</summary>
    public const string EstimateOrderCost = "ESTIMATE_ORDER_COST";
}

/// <summary>服务开始时间的特殊取值（官方 <c>time_range.start_time</c>）。</summary>
public static class PayScoreStartTimeKeywords
{
    /// <summary>用户确认订单时开始（官方 <c>OnAccept</c>）：用于「先享后付」式按受理时刻计时。</summary>
    public const string OnAccept = "OnAccept";
}

/// <summary>收款状态（官方 <c>collection.state</c>）。</summary>
public static class PayScoreCollectionStates
{
    /// <summary>用户支付中（官方 <c>USER_PAYING</c>）。</summary>
    public const string UserPaying = "USER_PAYING";

    /// <summary>用户已支付（官方 <c>USER_PAID</c>）。</summary>
    public const string UserPaid = "USER_PAID";
}

/// <summary>收款类型（官方 <c>collection.details[].paid_type</c>）。</summary>
public static class PayScoreCollectionPaidTypes
{
    /// <summary>新收款（官方 <c>NEWTON</c>）。</summary>
    public const string Newton = "NEWTON";

    /// <summary>预授权（官方 <c>ADVANCE</c>）。</summary>
    public const string Advance = "ADVANCE";

    /// <summary>余额（官方 <c>BALANCE</c>）。</summary>
    public const string Balance = "BALANCE";
}

/// <summary>优惠范围（官方 <c>promotion_detail[].scope</c>）。</summary>
public static class PayScorePromotionScopes
{
    /// <summary>全场（官方 <c>GLOBAL</c>）。</summary>
    public const string Global = "GLOBAL";

    /// <summary>单品（官方 <c>SINGLE</c>）。</summary>
    public const string Single = "SINGLE";
}

/// <summary>优惠类型（官方 <c>promotion_detail[].type</c>）。</summary>
public static class PayScorePromotionTypes
{
    /// <summary>代金券（官方 <c>CASH</c>）。</summary>
    public const string Cash = "CASH";

    /// <summary>折扣券（官方 <c>DISCOUNT</c>）。</summary>
    public const string Discount = "DISCOUNT";
}

/// <summary>
/// 授权状态（官方 <c>authorization_state</c>，3 值）。
/// </summary>
/// <remarks>
/// <b>官方说明照录</b>（<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_3.shtml"/>，
/// 2026-10-09 核验）：<c>UNBINDUSER</c>「未绑定用户（<b>仅完成预授权</b>）」、
/// <c>AVAILABLE</c>「用户已授权服务」、<c>UNAVAILABLE</c>「用户未授权服务
/// （用户授权过服务后面<b>解除了授权</b>）」。
/// <para>
/// <b>判别要点</b>：<c>UNBINDUSER</c> 表示<b>只做了预授权、用户尚未真正授权</b> ——
/// 它与 <c>UNAVAILABLE</c>（曾授权后解除）语义相反，不可混为一谈。
/// </para>
/// </remarks>
public static class PayScoreAuthorizationStates
{
    /// <summary>未绑定用户（官方 <c>UNBINDUSER</c>）：仅完成预授权。</summary>
    public const string UnbindUser = "UNBINDUSER";

    /// <summary>用户已授权服务（官方 <c>AVAILABLE</c>）。</summary>
    public const string Available = "AVAILABLE";

    /// <summary>用户未授权服务（官方 <c>UNAVAILABLE</c>）：曾授权后解除授权。</summary>
    public const string Unavailable = "UNAVAILABLE";
}

/// <summary>
/// 同步订单状态的场景类型（官方 <c>type</c>）。
/// </summary>
/// <remarks>
/// 官方只给出<b>一个</b>取值：<c>Order_Paid</c>（「收款场景，商户固定传 <c>Order_Paid</c>，
/// 表示订单收款成功」）—— 注意它是<b>混合大小写</b>而非下划线风格，勿「规范化」。
/// </remarks>
public static class PayScoreSyncOrderTypes
{
    /// <summary>收款场景（官方 <c>Order_Paid</c>）：表示订单收款成功。</summary>
    public const string OrderPaid = "Order_Paid";
}
