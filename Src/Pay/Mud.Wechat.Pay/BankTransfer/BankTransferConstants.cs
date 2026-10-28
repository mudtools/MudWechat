// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.BankTransfer;

/// <summary>
/// 转账场景（官方 <c>transfer_scene</c>，2 值，本面<b>必填</b>）。
/// </summary>
/// <remarks>取值集与服务商面 <c>PartnerTransferScenes</c> 同名同值，但本面为必填、服务商面为选填。</remarks>
public static class BankTransferScenes
{
    /// <summary>普通转账（官方 <c>ORDINARY_TRANSFER</c>，默认）。</summary>
    public const string OrdinaryTransfer = "ORDINARY_TRANSFER";

    /// <summary>给使用微信务工卡的用户转账（官方 <c>PAYROLL_CARD_TRANSFER</c>）：此时明细 <c>user_name</c> 必填密文。</summary>
    public const string PayrollCardTransfer = "PAYROLL_CARD_TRANSFER";
}

/// <summary>
/// 批量转账用途（官方 <c>transfer_purpose</c>，6 值，本面<b>必填</b>）。
/// </summary>
public static class BankTransferPurposes
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
/// 批次单状态（官方 <c>batch_state</c>，4 值）。
/// </summary>
/// <remarks>
/// <para><b>与服务商面 <c>batch_status</c> 差两值</b>：本面<b>没有</b> <c>WAIT_PAY</c>（待付款确认），
/// 因银行面由 <c>sponsor_mchid</c> 直接出资；两域状态类不得合并（守卫 BT-B4 各断言 4 / 5 值）。</para>
/// <para><c>ACCEPTED</c> 停留超 30 分钟通常是出资方余额不足，须查资金流水后再判定是否重发。</para>
/// </remarks>
public static class BankTransferBatchStates
{
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
/// 批次关闭原因（官方 <c>close_reason</c>，2 值）。
/// </summary>
/// <remarks>比服务商面少 <c>PAYER_ACCOUNT_ABNORMAL</c>（本面另两处官方文案以 <c>ACCOUNT_ERROR</c> 错误码承载）。</remarks>
public static class BankTransferCloseReasons
{
    /// <summary>银行服务商主动撤销（页面方式）（官方 <c>MERCHANT_REVOCATION</c>）。</summary>
    public const string MerchantRevocation = "MERCHANT_REVOCATION";

    /// <summary>系统超时关闭，可能账户余额不足或其他错误（官方 <c>OVERDUE_CLOSE</c>）。</summary>
    public const string OverdueClose = "OVERDUE_CLOSE";
}

/// <summary>
/// 明细单状态（官方 <c>detail_state</c>，4 值）。
/// </summary>
/// <remarks><b>比服务商面 <c>detail_status</c>（6 值）少 <c>INIT</c> 与 <c>WAIT_PAY</c></b>，且字段名不同 ⇒ 两域分建。</remarks>
public static class BankTransferDetailStates
{
    /// <summary>转账中（官方 <c>PROCESSING</c>）。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>转账成功（官方 <c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>转账失败（官方 <c>FAIL</c>）：随附 <c>fail_reason</c>，仅可对该笔明细重发。</summary>
    public const string Fail = "FAIL";

    /// <summary>退票（官方 <c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";
}

/// <summary>
/// 查批次时的明细状态过滤器（官方 query <c>detail_state</c>，3 值）。
/// </summary>
/// <remarks>
/// <b>query 参数名本身也不同</b>：本面 <c>detail_state</c>、服务商面 <c>detail_status</c>，
/// 且本面无 <c>WAIT_PAY</c> 档 ⇒ 照抄服务商面 query 名会静默丢过滤条件（守卫 BT-B2 逐端点锁定 query 名）。
/// </remarks>
public static class BankTransferDetailStateFilters
{
    /// <summary>全部（官方 <c>ALL</c>）：同时查询成功与失败的明细。</summary>
    public const string All = "ALL";

    /// <summary>仅转账成功（官方 <c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>仅转账失败（官方 <c>FAIL</c>）。</summary>
    public const string Fail = "FAIL";
}

/// <summary>
/// 电子回单状态（官方 <c>receipt_state</c>，2 值）。
/// </summary>
/// <remarks><see cref="Finished"/> 时 <c>hash_type</c> / <c>hash_value</c> / <c>download_url</c> 才出现。</remarks>
public static class BankTransferReceiptStates
{
    /// <summary>电子回单受理申请成功（官方 <c>ACCEPTED</c>）。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>电子回单文件生成成功（官方 <c>FINISHED</c>）。</summary>
    public const string Finished = "FINISHED";
}

/// <summary>
/// 电子回单文件哈希方法（官方 <c>hash_type</c>）。
/// </summary>
/// <remarks>官方原文「当前版本使用 SHA256」⇒ 本类<b>不是</b>穷尽集合，须兼容后续新增值。</remarks>
public static class BankTransferReceiptHashTypes
{
    /// <summary>SHA256（官方 <c>SHA256</c>，当前版本唯一取值）。</summary>
    public const string Sha256 = "SHA256";
}
