// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerPayScore;

/// <summary>
/// 服务商支付分服务订单状态（官方 <c>state</c>，5 值）。
/// </summary>
/// <remarks>
/// <b>官方注明三个终态</b>：<c>DONE</c> / <c>REVOKED</c> / <c>EXPIRED</c>；
/// <c>EXPIRED</c> 的触发条件是「<c>CREATED</c> 状态超过 30 天未变动」（官方原文），
/// 且消费侧须结合 <c>collection.state</c> 与 <c>state_description</c> 按官方流转图判断。
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692256"/>。
/// </remarks>
public static class PartnerPayScoreServiceOrderStates
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
/// 订单状态说明（官方 <c>state_description</c>）—— <b>本域两套拼写并存，照录各自页</b>。
/// </summary>
/// <remarks>
/// <para><b>官方两页写法不一致</b>：创建 / 查询支付分订单两页写
/// <see cref="MchComplete"/>（<c>MCH_COMPLETE</c>），而「基于签约计划创建服务订单」页写
/// <see cref="MerchantComplete"/>（<c>MERCHANT_COMPLETE</c>）。两个常量都保留、不做归一，
/// 宿主判等须按对应端点页的取值（守卫 PS-B4 锁定该差异）。</para>
/// <para>官方注明本字段<b>仅 <c>DOING</c> 状态返回</b>。</para>
/// </remarks>
public static class PartnerPayScoreStateDescriptions
{
    /// <summary>用户已确认（官方 <c>USER_CONFIRM</c>）。</summary>
    public const string UserConfirm = "USER_CONFIRM";

    /// <summary>商户已完结（官方 <c>MCH_COMPLETE</c>，服务订单两页的拼写）。</summary>
    public const string MchComplete = "MCH_COMPLETE";

    /// <summary>商户已完结（官方 <c>MERCHANT_COMPLETE</c>，签约计划服务订单页的拼写）。</summary>
    public const string MerchantComplete = "MERCHANT_COMPLETE";
}

/// <summary>
/// 收款状态（官方 <c>collection.state</c>，2 值）。
/// </summary>
/// <remarks>须结合订单 <c>state</c> 与 <c>state_description</c> 一起判断（官方流转图）。</remarks>
public static class PartnerPayScoreCollectionStates
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
/// <b>本域只有两值</b>：官方服务商业字段表列 <c>NEWTON</c> 与 <c>MCH</c>；
/// 直连支付分域另有 <c>ADVANCE</c>（预授权）与 <c>BALANCE</c>（余额）——
/// 不得把直连取值抄进服务商面（守卫 PS-B4 断言本域取值集恰为两值）。
/// </remarks>
public static class PartnerPayScoreCollectionPaidTypes
{
    /// <summary>微信支付分渠道（官方 <c>NEWTON</c>）：自动扣款或用户在支付分订单页主动支付。</summary>
    public const string Newton = "NEWTON";

    /// <summary>商户渠道（官方 <c>MCH</c>）：用户经其他渠道支付后，商户调同步服务订单状态接口成功才返回。</summary>
    public const string Merchant = "MCH";
}

/// <summary>
/// 同步服务订单状态的场景类型（官方 <c>type</c>，本域单值）。
/// </summary>
/// <remarks><b>大小写照官方原文</b>：<c>Order_Paid</c>（首字母大写的驼峰 + 下划线），不是全大写枚举形态。</remarks>
public static class PartnerPayScoreSyncTypes
{
    /// <summary>订单收款成功（官方 <c>Order_Paid</c>，商户固定传该值）。</summary>
    public const string OrderPaid = "Order_Paid";
}

/// <summary>
/// 服务风险金名称（官方 <c>risk_fund.name</c>，本域单值）。
/// </summary>
/// <remarks>
/// <b>本域官方字段表「固定传」<see cref="EstimateOrderCost"/></b>：先免模式的 <c>DEPOSIT</c>/
/// <c>ADVANCE</c>/<c>CASH_DEPOSIT</c> 属于直连支付分域，服务商免确认模式不提供 —— 直连页示例报文里的
/// <c>"name":"DEPOSIT"</c> 是官方示例自身的疏漏，不得据此放宽本域取值。
/// </remarks>
public static class PartnerPayScoreRiskFundNames
{
    /// <summary>订单预估费用（官方 <c>ESTIMATE_ORDER_COST</c>）。</summary>
    public const string EstimateOrderCost = "ESTIMATE_ORDER_COST";
}

/// <summary>
/// 服务开始时间的关键字取值（官方 <c>time_range.start_time</c> 的第三种形态）。
/// </summary>
/// <remarks>
/// 官方该字段支持 <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c> / <c>OnAccept</c> 三形态；
/// 传本值表示「以创单成功时间为服务开始时间」，且此时 <c>end_time</c> <b>必须</b>用 <c>yyyyMMddHHmmss</c>。
/// </remarks>
public static class PartnerPayScoreStartTimeKeywords
{
    /// <summary>免确认创单成功时间（官方 <c>OnAccept</c>）。</summary>
    public const string OnAccept = "OnAccept";
}

/// <summary>
/// 场景权限的场景值（官方 <c>scene</c>，当前单值）。
/// </summary>
/// <remarks>官方「开启场景服务权限」页当前只开放 <c>TRANSACTION</c>，且该场景要求填 <c>transaction_scene_info</c>。</remarks>
public static class PartnerPayScoreScenes
{
    /// <summary>交易场景（官方 <c>TRANSACTION</c>）。</summary>
    public const string Transaction = "TRANSACTION";
}

/// <summary>
/// 用户授权状态（官方 <c>authorization_state</c>）—— <b>本域同名字段两套值集</b>。
/// </summary>
/// <remarks>
/// <para>按<b>授权协议号</b>查询页列三值（含 <see cref="UnbindUser"/>），按 <b>OpenID</b> 查询页只列两值
/// （<see cref="Unavailable"/> / <see cref="Available"/>）⇒ 载荷模型按超集承载，
/// 判等须按对应端点页的取值集（守卫 PS-B4 锁定）。</para>
/// <para>注意与直连支付分域的差异：直连侧另有 <c>expired</c> 一类取值，本域不提供。</para>
/// </remarks>
public static class PartnerPayScoreAuthorizationStates
{
    /// <summary>不可用（官方 <c>UNAVAILABLE</c>）：用户未开通或已解除授权。</summary>
    public const string Unavailable = "UNAVAILABLE";

    /// <summary>可用（官方 <c>AVAILABLE</c>）。</summary>
    public const string Available = "AVAILABLE";

    /// <summary>用户解绑（官方 <c>UNBINDUSER</c>）：<b>仅按授权协议号查询页返回</b>。</summary>
    public const string UnbindUser = "UNBINDUSER";
}

/// <summary>优惠范围（官方 <c>promotion_detail[].scope</c>）。</summary>
public static class PartnerPayScorePromotionScopes
{
    /// <summary>全场代金券（官方 <c>GLOBAL</c>）：以订单整体可优惠金额为门槛。</summary>
    public const string Global = "GLOBAL";

    /// <summary>单品优惠（官方 <c>SINGLE</c>）：以订单中某个单品的总金额为门槛。</summary>
    public const string Single = "SINGLE";
}

/// <summary>优惠资金类型（官方 <c>promotion_detail[].type</c>）。</summary>
public static class PartnerPayScorePromotionTypes
{
    /// <summary>预充值（官方 <c>CASH</c>）：带结算资金，随订单结算给收款商户。</summary>
    public const string Cash = "CASH";

    /// <summary>免充值（官方 <c>NOCASH</c>）：无资金结算给收款商户。</summary>
    public const string NoCash = "NOCASH";
}

/// <summary>支付分计划状态（官方 <c>plan_state</c>，2 值）。</summary>
public static class PartnerPayScorePlanStates
{
    /// <summary>计划正常进行中，可以签约（官方 <c>NORMAL</c>）。</summary>
    public const string Normal = "NORMAL";

    /// <summary>计划已停止（官方 <c>STOP</c>）：无法新签约，<b>已签约的计划仍可继续完成服务</b>。</summary>
    public const string Stop = "STOP";
}

/// <summary>计划签约状态（官方 <c>sign_state</c>，5 值）。</summary>
public static class PartnerPayScoreSignStates
{
    /// <summary>用户未签约（官方 <c>UNSIGNED</c>）。</summary>
    public const string Unsigned = "UNSIGNED";

    /// <summary>用户已签约（官方 <c>SIGNED</c>）。</summary>
    public const string Signed = "SIGNED";

    /// <summary>签约计划已取消（官方 <c>SIGN_PLAN_CANCEL</c>）。</summary>
    public const string SignPlanCancel = "SIGN_PLAN_CANCEL";

    /// <summary>签约计划的所有计划详情已全部完成（官方 <c>COMPLETE</c>）。</summary>
    public const string Complete = "COMPLETE";

    /// <summary>签约计划已过期（官方 <c>EXPIRE</c>）：注意本值为 <c>EXPIRE</c>，与服务订单的 <c>EXPIRED</c> 不同。</summary>
    public const string Expire = "EXPIRE";
}

/// <summary>签约计划取消类型（官方 <c>cancel_sign_type</c>，4 值）。</summary>
public static class PartnerPayScoreCancelSignTypes
{
    /// <summary>已签约协议未取消（官方 <c>NOT_CANCEL</c>）。</summary>
    public const string NotCancel = "NOT_CANCEL";

    /// <summary>用户取消已签约的协议（官方 <c>USER</c>）。</summary>
    public const string User = "USER";

    /// <summary>商户取消已签约的协议（官方 <c>MERCHANT</c>）。</summary>
    public const string Merchant = "MERCHANT";

    /// <summary>用户解除服务授权时连带取消服务下的已签约协议（官方 <c>REVOKE_SERVICE</c>）。</summary>
    public const string RevokeService = "REVOKE_SERVICE";
}

/// <summary>计划明细状态（官方 <c>signed_detail_list[].plan_detail_state</c>，4 值）。</summary>
public static class PartnerPayScorePlanDetailStates
{
    /// <summary>本计划详情还未使用（官方 <c>NOT_USED</c>）。</summary>
    public const string NotUsed = "NOT_USED";

    /// <summary>使用中，已有对应服务订单（官方 <c>USING</c>）。</summary>
    public const string Using = "USING";

    /// <summary>已使用，对应服务订单已完成支付（官方 <c>USED</c>）。</summary>
    public const string Used = "USED";

    /// <summary>已取消使用，对应服务订单已取消（官方 <c>SIGN_PLAN_DETAIL_CANCEL</c>）。</summary>
    public const string SignPlanDetailCancel = "SIGN_PLAN_DETAIL_CANCEL";
}
