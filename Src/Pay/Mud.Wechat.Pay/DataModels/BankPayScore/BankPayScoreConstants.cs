// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.BankPayScore;

/// <summary>
/// 从业机构支付分服务订单状态（官方 <c>state</c>，5 值）。
/// </summary>
/// <remarks>
/// <b>官方注明三个终态</b>：<c>DONE</c> / <c>REVOKED</c> / <c>EXPIRED</c>；
/// <c>EXPIRED</c> 的触发条件是「<c>CREATED</c> 状态超过 30 天未变动」（官方原文），
/// 且消费侧须结合 <c>collection.state</c> 与 <c>state_description</c> 按官方流转图判断。
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691343"/>。
/// </remarks>
public static class BankPayScoreServiceOrderStates
{
    /// <summary>商户已创建服务订单（官方 <c>CREATED</c>）。</summary>
    public const string Created = "CREATED";

    /// <summary>服务订单进行中（官方 <c>DOING</c>）。</summary>
    public const string Doing = "DOING";

    /// <summary>服务订单完成（官方 <c>DONE</c>，终态）。</summary>
    public const string Done = "DONE";

    /// <summary>商户取消服务订单（官方 <c>REVOKED</c>，终态）。</summary>
    public const string Revoked = "REVOKED";

    /// <summary>服务订单已失效（官方 <c>EXPIRED</c>，终态）：<c>CREATED</c> 超 30 天未变动。</summary>
    public const string Expired = "EXPIRED";
}

/// <summary>
/// 订单状态说明（官方 <c>state_description</c>，2 值）—— <b>本域只有 <c>MCH_COMPLETE</c> 一种拼写</b>。
/// </summary>
/// <remarks>
/// <para>官方注明本字段<b>仅 <c>DOING</c> 状态返回</b>，消费时须结合 <c>state</c> 与 <c>collection.state</c> 判断。</para>
/// <para><b>与服务商支付分域（P0-4）的差异</b>：那一域因存在「基于签约计划创建服务订单」页，
/// 官方另写 <c>MERCHANT_COMPLETE</c> 拼写；本域（从业机构免确认模式）无签约计划页，
/// 全部页面只出现 <c>MCH_COMPLETE</c> ⇒ 本类型刻意<b>不</b>提供 <c>MerchantComplete</c> 常量
/// （守卫 BS-B4 锁定该值集恰为两值）。</para>
/// </remarks>
public static class BankPayScoreStateDescriptions
{
    /// <summary>用户已确认（官方 <c>USER_CONFIRM</c>）。</summary>
    public const string UserConfirm = "USER_CONFIRM";

    /// <summary>商户已完结（官方 <c>MCH_COMPLETE</c>）：调完结接口成功后至扣款成功前的状态。</summary>
    public const string MchComplete = "MCH_COMPLETE";
}

/// <summary>
/// 收款状态（官方 <c>collection.state</c>，2 值）。
/// </summary>
/// <remarks>须结合订单 <c>state</c> 与 <c>state_description</c> 一起判断（官方流转图）。</remarks>
public static class BankPayScoreCollectionStates
{
    /// <summary>待支付（官方 <c>USER_PAYING</c>）。</summary>
    public const string UserPaying = "USER_PAYING";

    /// <summary>已支付（官方 <c>USER_PAID</c>）。</summary>
    public const string UserPaid = "USER_PAID";
}

/// <summary>
/// 收款成功渠道（官方 <c>collection.details[].paid_type</c>，2 值）—— <b>与直连支付分域取值集不同</b>。
/// </summary>
/// <remarks>
/// 官方本页只列 <c>NEWTON</c> 与 <c>MCH</c>；直连支付分域另有 <c>ADVANCE</c>（预授权）与
/// <c>BALANCE</c>（余额）—— 不得把直连取值抄进本域（守卫 BS-B4 断言本域取值集恰为两值）。
/// </remarks>
public static class BankPayScoreCollectionPaidTypes
{
    /// <summary>微信支付分渠道（官方 <c>NEWTON</c>）：自动扣款或用户在支付分订单页主动支付。</summary>
    public const string Newton = "NEWTON";

    /// <summary>商户渠道（官方 <c>MCH</c>）：用户经其他渠道支付后，商户调同步支付分订单状态接口成功才返回。</summary>
    public const string Merchant = "MCH";
}

/// <summary>
/// 同步支付分订单状态的场景类型（官方 <c>type</c>，本域单值）。
/// </summary>
/// <remarks>官方原文「收款场景，商户固定传 <c>Order_Paid</c>，表示订单收款成功」⇒ 无第二值。</remarks>
public static class BankPayScoreSyncTypes
{
    /// <summary>订单收款成功（官方 <c>Order_Paid</c>）：<b>注意官方此处是首字母大写的混合大小写</b>，非全大写蛇形。</summary>
    public const string OrderPaid = "Order_Paid";
}

/// <summary>
/// 服务开始时间的关键字取值（官方 <c>time_range.start_time</c> 的非日期取值，本域单值）。
/// </summary>
/// <remarks>
/// <para>官方 <c>start_time</c> 支持三种写法：<c>yyyyMMddHHmmss</c>、<c>yyyyMMdd</c> 与本关键字
/// <c>OnAccept</c>（表示「以免确认创单成功时间为服务开始时间」）；<c>end_time</c> <b>只支持前两种</b>，
/// 且官方规定 <c>start_time=OnAccept</c> 时 <c>end_time</c> 必须用 <c>yyyyMMddHHmmss</c>。</para>
/// <para>故 <c>time_range</c> 的两个字段是<b>格式受限的字符串</b>而非纯日期串 ⇒ 保持 <c>string</c>，
/// 不做 <see cref="DateTimeOffset"/> 往返（守卫 BS-B3 锁定两套时间格式并存）。</para>
/// </remarks>
public static class BankPayScoreStartTimeKeywords
{
    /// <summary>以创单成功时间为服务开始时间（官方 <c>OnAccept</c>）：<b>首字母大写的驼峰形态</b>，非全大写蛇形。</summary>
    public const string OnAccept = "OnAccept";
}

/// <summary>
/// 服务风险金名称（官方 <c>risk_fund.name</c>，本域单值）。
/// </summary>
/// <remarks>官方原文「订单的风险金额名称，固定传 <c>ESTIMATE_ORDER_COST</c>，代表订单预估费用」。</remarks>
public static class BankPayScoreRiskFundNames
{
    /// <summary>订单预估费用（官方 <c>ESTIMATE_ORDER_COST</c>）。</summary>
    public const string EstimateOrderCost = "ESTIMATE_ORDER_COST";
}

/// <summary>
/// 用户授权状态（官方 <c>authorization_state</c>，3 值）。
/// </summary>
/// <remarks>
/// <para>官方三值语义（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461004"/>，2026-10-14 核验）：
/// <c>UNBINDUSER</c> = 未绑定用户（<b>仅完成预授权</b>）、<c>AVAILABLE</c> = 用户已授权服务、
/// <c>UNAVAILABLE</c> = 用户授权过服务后面解除了授权。</para>
/// <para><b><c>UNBINDUSER</c> 是官方自身的不规则拼写</b>（不是 <c>UNBIND_USER</c>），
/// 且本字段官方宽度为 <c>string(16)</c>（其余授权字段多为 32）—— 照录不改。</para>
/// </remarks>
public static class BankPayScoreAuthorizationStates
{
    /// <summary>未绑定用户（官方 <c>UNBINDUSER</c>）：仅完成预授权，尚未取得用户授权。</summary>
    public const string UnbindUser = "UNBINDUSER";

    /// <summary>用户已授权服务（官方 <c>AVAILABLE</c>）。</summary>
    public const string Available = "AVAILABLE";

    /// <summary>用户未授权服务（官方 <c>UNAVAILABLE</c>）：授权过服务后已解除。</summary>
    public const string Unavailable = "UNAVAILABLE";
}

/// <summary>
/// 用户标签（官方 <c>user_label</c>，3 值）。
/// </summary>
/// <remarks>
/// <para>官方原文：「用户标签，仅授权成功时返回（并且不保证一定返回）枚举值：A、B、C」。
/// <b>比较说明在官方页面自身即被截断</b>（正文写作「（其中A&gt;B&gt;」后即接下一字段，2026-10-14 核验）
/// ⇒ 本仓只录三个值、<b>不转录排序语义</b>，宿主不得据常量名推断 A 与 C 谁优。</para>
/// </remarks>
public static class BankPayScoreUserLabels
{
    /// <summary>用户标签 A（官方 <c>A</c>）。</summary>
    public const string A = "A";

    /// <summary>用户标签 B（官方 <c>B</c>）。</summary>
    public const string B = "B";

    /// <summary>用户标签 C（官方 <c>C</c>）。</summary>
    public const string C = "C";
}

/// <summary>
/// 优惠范围（官方 <c>promotion_detail[].scope</c>，2 值）。
/// </summary>
public static class BankPayScorePromotionScopes
{
    /// <summary>全场代金券（官方 <c>GLOBAL</c>）：以订单整体可优惠金额为门槛。</summary>
    public const string Global = "GLOBAL";

    /// <summary>单品优惠（官方 <c>SINGLE</c>）：以订单中某个单品总金额为门槛。</summary>
    public const string Single = "SINGLE";
}

/// <summary>
/// 优惠类型（官方 <c>promotion_detail[].type</c>，2 值）。
/// </summary>
public static class BankPayScorePromotionTypes
{
    /// <summary>预充值（官方 <c>CASH</c>）：带结算资金，随订单结算给收款商户。</summary>
    public const string Cash = "CASH";

    /// <summary>免充值（官方 <c>NOCASH</c>）：无结算资金，不结算给收款商户。</summary>
    public const string NoCash = "NOCASH";
}
