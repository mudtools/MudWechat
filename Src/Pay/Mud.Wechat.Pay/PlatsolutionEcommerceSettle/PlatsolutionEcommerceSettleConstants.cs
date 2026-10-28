// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceSettle;

/// <summary>
/// 批量结算单状态（<c>state</c>）取值 —— <b>官方必填，六值</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/>（申请）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650326"/>（查询）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015198012"/>（异常结算申请），三页同集且均标<b>必填</b>。
/// </para>
/// <para>
/// <b>状态机红线</b>（六值语义逐字照页）：
/// <c>RECEIVED</c>（用户入账成功）<b>不等于</b> <c>SUCCESS</c>（结算完成）—— 入账与扣费是两步；
/// <c>DEDUCT_FEE_FAIL</c> 是「用户入账成功、扣费失败」⇒ <b>不得按失败给用户退回资金</b>，须走平台侧补扣；
/// <c>ABNORMAL</c> 是唯一需要走异常结算支（<c>…/apply-abnormal-settle</c>）的状态；
/// <c>ABNORMAL_SETTLE_SUCCESS</c> 指「异常结算<b>处理申请</b>成功」，仍非资金终态。
/// 本六值与出款族付款单状态（<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransferStates"/>）
/// <b>只共享 <c>ACCEPTED</c> / <c>SUCCESS</c> 两个字面</b>，其余互斥 ⇒ 判定器不得合并。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceSettleStates
{
    /// <summary>结算已受理（<c>ACCEPTED</c>）。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>用户入账成功（<c>RECEIVED</c>）：入账完成，扣费结果未定。</summary>
    public const string Received = "RECEIVED";

    /// <summary>结算完成（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>用户入账成功、扣费失败（<c>DEDUCT_FEE_FAIL</c>）：资金已到账，不得退款。</summary>
    public const string DeductFeeFail = "DEDUCT_FEE_FAIL";

    /// <summary>结算异常（<c>ABNORMAL</c>）：异常原因见 <c>abnormal_type</c>，处置走异常结算支。</summary>
    public const string Abnormal = "ABNORMAL";

    /// <summary>异常结算处理申请成功（<c>ABNORMAL_SETTLE_SUCCESS</c>）：指申请受理成功，非资金终态。</summary>
    public const string AbnormalSettleSuccess = "ABNORMAL_SETTLE_SUCCESS";
}

/// <summary>结算明细的结算类型（<c>settle_detail_info_list.settle_type</c>）取值 —— <b>官方必填</b>。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/> 请求表，两值。
/// <c>DEDUCTION</c> 官方释义为「交易款扣除平台商户相关服务费用」⇒ 一笔结算单的明细里
/// 收款与扣除<b>混排</b>，正负号语义由本字段承载，金额字段本身不带符号。
/// </remarks>
public static class PlatsolutionEcommerceSettleTypes
{
    /// <summary>收款（<c>RECEIVE</c>）。</summary>
    public const string Receive = "RECEIVE";

    /// <summary>交易款扣除平台商户相关服务费用（<c>DEDUCTION</c>）。</summary>
    public const string Deduction = "DEDUCTION";
}

/// <summary>
/// 结算明细的收款方类型（<c>settle_detail_info_list.receiver_type</c>）取值 —— <b>命名体系与出款族不同，不得套用</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/> 请求表，两值、官方<b>选填</b>。
/// 本域的「用户」写作 <c>USER</c>，而出款族的 <c>receiver.type</c> 是
/// <c>TRANSACTION_USER</c> / <c>WXPAY_USER</c>（见
/// <see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit.PlatsolutionEcommerceDepositAfterSalesReceiverTypes"/>）
/// ⇒ <b>两套命名体系没有交集</b>，把 <c>TRANSACTION_USER</c> 传进结算明细即 <c>PARAM_ERROR</c>；
/// 同域的 <c>MERCHANT</c> 只是字面巧合，语义载体（明细行 vs 收款方对象）亦不同。
/// </remarks>
public static class PlatsolutionEcommerceSettleReceiverTypes
{
    /// <summary>微信支付商户（<c>MERCHANT</c>）。</summary>
    public const string Merchant = "MERCHANT";

    /// <summary>微信支付用户（<c>USER</c>）：本域独有写法，出款族对应字面是 <c>WXPAY_USER</c>。</summary>
    public const string User = "USER";
}

/// <summary>结算异常原因（<c>abnormal_type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015198012"/> 应答表，两值、官方<b>选填</b>
/// （仅 <c>state</c> 为 <see cref="PlatsolutionEcommerceSettleStates.Abnormal"/> 时出现）。
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650326"/>（查询支）<b>不印本字段</b>
/// ⇒ 异常原因只在异常结算支的应答里给出，不得指望查询支返回。
/// </remarks>
public static class PlatsolutionEcommerceSettleAbnormalTypes
{
    /// <summary>收款方授权关系已失效（<c>AUTH_RELATIONSHIP_INVALID</c>）。</summary>
    public const string AuthRelationshipInvalid = "AUTH_RELATIONSHIP_INVALID";

    /// <summary>个人卖家支付额度超限（<c>USER_PAY_QUOTA_LIMIT</c>）。</summary>
    public const string UserPayQuotaLimit = "USER_PAY_QUOTA_LIMIT";
}

/// <summary>异常结算处置类型（<c>abnormal_settle_type</c>）取值 —— <b>官方只印一个值且必填</b>。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015198012"/>：请求体 <c>abnormal_settle_type</c>
/// 与应答子树 <c>abnormal_settle_result.abnormal_settle_type</c> 均只 <c>TRANSFER_TO_ORIGINAL_RECEIVE_USER</c>
/// （转付至原收款用户）、均标<b>必填</b>；<see href="https://pay.weixin.qq.com/doc/v3/partner/4012650326"/>
/// 的应答亦回显同值 ⇒ 异常结算支<b>没有</b>「退回平台」等其它处置口径，别按第三方 SDK 补枚举。
/// </remarks>
public static class PlatsolutionEcommerceSettleAbnormalSettleTypes
{
    /// <summary>转付至原收款用户（<c>TRANSFER_TO_ORIGINAL_RECEIVE_USER</c>）：本域唯一合法取值。</summary>
    public const string TransferToOriginalReceiveUser = "TRANSFER_TO_ORIGINAL_RECEIVE_USER";
}

/// <summary>
/// 交易场景（<c>trade_scenario</c>）取值 —— <b>同名参数、两页两集，取并集建模</b>。
/// </summary>
/// <remarks>
/// 2026-10-16 逐页取「可选取值」原文比对：
/// <list type="bullet">
/// <item><description>查询支 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650326"/>：
/// <b>只印</b> <c>RECOMMERCE</c>（二手交易），标必填。</description></item>
/// <item><description>异常结算支 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015198012"/>：
/// <c>RECOMMERCE</c> / <c>INDIVIDUALTRADE</c>（个人交易）两值，标必填。</description></item>
/// <item><description>申请支 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/>：<b>不印本字段</b>。</description></item>
/// </list>
/// ⇒ 两支应答里的同一字段官方取值面宽窄不一，<b>按并集建模、不按单页收窄</b>
/// （查询支「只印一值」是页面收录差异，不是官方断言该单不可能为个人交易）。
/// 注意 <c>INDIVIDUALTRADE</c> 无下划线分隔（官方原文拼写如此），照页不得写成 <c>INDIVIDUAL_TRADE</c>。
/// </remarks>
public static class PlatsolutionEcommerceSettleTradeScenarios
{
    /// <summary>二手交易（<c>RECOMMERCE</c>）：两页共有。</summary>
    public const string Recommerce = "RECOMMERCE";

    /// <summary>个人交易（<c>INDIVIDUALTRADE</c>）：<b>只在异常结算支的应答表印出</b>。</summary>
    public const string IndividualTrade = "INDIVIDUALTRADE";
}
