// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransfer;

/// <summary>
/// 特约商户授权类型（官方 <c>authorization_type</c>，3 值）。
/// </summary>
/// <remarks>
/// <para><b>决定 <c>openid</c> 的归属与出款方</b>，传错会让批次内全部明细以 <c>OPENID_INVALID</c> 失败。</para>
/// <para>官方注明 <see cref="InformationAuthorizationType"/> <b>自 2026-06-29 起停止支持</b> ⇒
/// 新建集成不得使用，存量批次仍会返回该值。</para>
/// </remarks>
public static class PartnerTransferAuthorizationTypes
{
    /// <summary>使用特约商户用户信息、出款方为服务商（官方 <c>INFORMATION_AUTHORIZATION_TYPE</c>，2026-06-29 起停支持）。</summary>
    public const string InformationAuthorizationType = "INFORMATION_AUTHORIZATION_TYPE";

    /// <summary>使用特约商户资金、出款方为特约商户，用户信息为服务商 AppID 下的 openid（官方 <c>FUND_AUTHORIZATION_TYPE</c>）。</summary>
    public const string FundAuthorizationType = "FUND_AUTHORIZATION_TYPE";

    /// <summary>使用特约商户用户信息且出款方为特约商户（官方 <c>INFORMATION_AND_FUND_AUTHORIZATION_TYPE</c>）。</summary>
    public const string InformationAndFundAuthorizationType = "INFORMATION_AND_FUND_AUTHORIZATION_TYPE";
}

/// <summary>
/// 批次单状态（官方 <c>batch_status</c>，5 值）。
/// </summary>
/// <remarks>
/// <b>与银行面的差异</b>：本域字段名为 <c>batch_status</c> 且<b>多一个 <c>WAIT_PAY</c>（待付款确认）</b>，
/// 银行面 <c>batch_state</c> 只有 4 值 ⇒ 两套状态不可合并为一个常量类。
/// <c>ACCEPTED</c> 停留超 30 分钟通常意味着出资方余额不足，须查资金流水再判定。
/// </remarks>
public static class PartnerTransferBatchStates
{
    /// <summary>待付款确认（官方 <c>WAIT_PAY</c>）：需出资商户在商家助手 / 服务商助手小程序确认。</summary>
    public const string WaitPay = "WAIT_PAY";

    /// <summary>已受理（官方 <c>ACCEPTED</c>）。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>转账中（官方 <c>PROCESSING</c>）。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>已完成（官方 <c>FINISHED</c>）：此时批次应答才带明细列表。</summary>
    public const string Finished = "FINISHED";

    /// <summary>已关闭（官方 <c>CLOSED</c>）：随附 <c>close_reason</c>。</summary>
    public const string Closed = "CLOSED";
}

/// <summary>
/// 批次单发起类型（官方 <c>batch_type</c>，2 值）。
/// </summary>
/// <remarks>页面方式发起的批次<b>不会被本域查单接口遗漏</b>，但其 <c>out_batch_no</c> 由商户平台生成，
/// 宿主按 <c>batch_id</c> 定位更稳。</remarks>
public static class PartnerTransferBatchTypes
{
    /// <summary>API 方式发起（官方 <c>API</c>）。</summary>
    public const string Api = "API";

    /// <summary>页面方式发起（官方 <c>WEB</c>）。</summary>
    public const string Web = "WEB";
}

/// <summary>
/// 批次关闭原因（官方 <c>close_reason</c>，3 值）。
/// </summary>
/// <remarks>官方原文注明<b>「可选取值可能新增，请兼容新增值并关注接口文档更新」</b> ⇒
/// 消费侧不得写成穷尽分支，未识别值按「已关闭 + 未知原因」处置。</remarks>
public static class PartnerTransferCloseReasons
{
    /// <summary>商户主动撤销（页面方式）（官方 <c>MERCHANT_REVOCATION</c>）。</summary>
    public const string MerchantRevocation = "MERCHANT_REVOCATION";

    /// <summary>系统超时关闭，可能账户余额不足或其他错误（官方 <c>OVERDUE_CLOSE</c>）。</summary>
    public const string OverdueClose = "OVERDUE_CLOSE";

    /// <summary>商户账户付款受限（官方 <c>PAYER_ACCOUNT_ABNORMAL</c>）。</summary>
    public const string PayerAccountAbnormal = "PAYER_ACCOUNT_ABNORMAL";
}

/// <summary>
/// 明细单状态（官方 <c>detail_status</c>，6 值）。
/// </summary>
/// <remarks>
/// <b>比银行面多 <c>INIT</c> 与 <c>WAIT_PAY</c> 两值</b>（银行 <c>detail_state</c> 只列 4 值）；
/// 官方明确 <c>FAIL</c> 时须先确认失败原因再决定是否<b>仅对该笔明细</b>重发（并非整个批次）。
/// </remarks>
public static class PartnerTransferDetailStates
{
    /// <summary>初始态，系统转账校验中（官方 <c>INIT</c>）。</summary>
    public const string Init = "INIT";

    /// <summary>待确认（官方 <c>WAIT_PAY</c>）：符合免密条件时系统自动扭转为转账中。</summary>
    public const string WaitPay = "WAIT_PAY";

    /// <summary>转账中（官方 <c>PROCESSING</c>）。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>转账成功（官方 <c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>转账失败（官方 <c>FAIL</c>）：随附 <c>fail_reason</c>。</summary>
    public const string Fail = "FAIL";

    /// <summary>退票（官方 <c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";
}

/// <summary>
/// 查批次时的明细状态过滤器（官方 query <c>detail_status</c>，4 值）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="PartnerTransferDetailStates"/> 不是同一值集</b>：过滤器取 <c>ALL</c>，
/// 且不含 <c>INIT</c>/<c>PROCESSING</c>/<c>REFUND</c>；银行面的同名过滤器叫 <c>detail_state</c>
/// 且<b>没有</b> <c>WAIT_PAY</c> ⇒ 两域各建一类，不得互为别名（守卫 PTF-B4）。
/// </remarks>
public static class PartnerTransferDetailStateFilters
{
    /// <summary>全部（官方 <c>ALL</c>）：同时查询成功、失败与待确认的明细。</summary>
    public const string All = "ALL";

    /// <summary>仅转账失败（官方 <c>FAIL</c>）。</summary>
    public const string Fail = "FAIL";

    /// <summary>仅转账成功（官方 <c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>仅待确认（官方 <c>WAIT_PAY</c>）：本域独有档位，银行面没有。</summary>
    public const string WaitPay = "WAIT_PAY";
}

/// <summary>
/// 批量转账用途（官方 <c>transfer_purpose</c>，6 值）。
/// </summary>
public static class PartnerTransferPurposes
{
    /// <summary>给用户支付货物采购资金（官方 <c>GOODSPAYMENT</c>）。</summary>
    public const string GoodsPayment = "GOODSPAYMENT";

    /// <summary>给用户支付业务推广佣金（官方 <c>COMMISSION</c>）。</summary>
    public const string Commission = "COMMISSION";

    /// <summary>给用户支付交易退款（官方 <c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";

    /// <summary>企业给员工支付差旅等报销资金（官方 <c>REIMBURSEMENT</c>）。</summary>
    public const string Reimbursement = "REIMBURSEMENT";

    /// <summary>给司机支付运输费用（官方 <c>FREIGHT</c>）。</summary>
    public const string Freight = "FREIGHT";

    /// <summary>其他（官方 <c>OTHERS</c>）。</summary>
    public const string Others = "OTHERS";
}

/// <summary>
/// 转账场景（官方 <c>transfer_scene</c>，2 值）。
/// </summary>
/// <remarks><see cref="PayrollCardTransfer"/> 场景须为明细传入加密后的 <c>user_name</c>（官方务工卡实名校验）。</remarks>
public static class PartnerTransferScenes
{
    /// <summary>普通转账（官方 <c>ORDINARY_TRANSFER</c>，默认）。</summary>
    public const string OrdinaryTransfer = "ORDINARY_TRANSFER";

    /// <summary>给使用微信务工卡的用户转账（官方 <c>PAYROLL_CARD_TRANSFER</c>）。</summary>
    public const string PayrollCardTransfer = "PAYROLL_CARD_TRANSFER";
}

/// <summary>
/// 收款账户类型（官方明细应答 <c>account_type</c>，2 值）。
/// </summary>
/// <remarks>
/// <b>官方正文与取值表自身不一致</b>：说明文字写作 <c>WXPAY_ACCOUNT</c>，而「可选取值」与示例报文为
/// <c>WECHATPAY_ACCOUNT</c> ⇒ 本类只采<b>取值表拼写</b>，宿主比较时勿按正文笔写判等（守卫 PTF-B4 锁定两值集）。
/// </remarks>
public static class PartnerTransferAccountTypes
{
    /// <summary>微信支付零钱账户（官方 <c>WECHATPAY_ACCOUNT</c>）。</summary>
    public const string WechatpayAccount = "WECHATPAY_ACCOUNT";

    /// <summary>银行卡账户（官方 <c>BANK_ACCOUNT</c>）：此时应答另附 <c>bank_name</c> 与 <c>bank_card_number_tail</c>。</summary>
    public const string BankAccount = "BANK_ACCOUNT";
}
