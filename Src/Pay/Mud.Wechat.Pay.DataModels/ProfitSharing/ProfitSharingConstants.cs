// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.ProfitSharing;

/// <summary>分账接收方类型（官方 <c>type</c> 取值，请求与应答共用）。</summary>
/// <remarks>官方枚举<b>仅两值</b>（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528995"/>）。</remarks>
public static class ProfitSharingReceiverTypes
{
    /// <summary>商户号（官方 <c>MERCHANT_ID</c>）。</summary>
    public const string MerchantId = "MERCHANT_ID";

    /// <summary>个人 openid（官方 <c>PERSONAL_OPENID</c>；用户在商户 appid 下的唯一标识）。</summary>
    public const string PersonalOpenId = "PERSONAL_OPENID";
}

/// <summary>分账接收方与商户的关系（官方 <c>relation_type</c>，10 值）。</summary>
/// <remarks>取值照官方原文枚举（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528995"/>）。</remarks>
public static class ProfitSharingRelationTypes
{
    /// <summary>门店（官方 <c>STORE</c>）。</summary>
    public const string Store = "STORE";

    /// <summary>员工（官方 <c>STAFF</c>）。</summary>
    public const string Staff = "STAFF";

    /// <summary>店主（官方 <c>STORE_OWNER</c>）。</summary>
    public const string StoreOwner = "STORE_OWNER";

    /// <summary>合作伙伴（官方 <c>PARTNER</c>）。</summary>
    public const string Partner = "PARTNER";

    /// <summary>总部（官方 <c>HEADQUARTER</c>）。</summary>
    public const string Headquarter = "HEADQUARTER";

    /// <summary>品牌方（官方 <c>BRAND</c>）。</summary>
    public const string Brand = "BRAND";

    /// <summary>分销商（官方 <c>DISTRIBUTOR</c>）。</summary>
    public const string Distributor = "DISTRIBUTOR";

    /// <summary>用户（官方 <c>USER</c>）。</summary>
    public const string User = "USER";

    /// <summary>供应商（官方 <c>SUPPLIER</c>）。</summary>
    public const string Supplier = "SUPPLIER";

    /// <summary>自定义（官方 <c>CUSTOM</c>；须同时填 <c>custom_relation</c>）。</summary>
    public const string Custom = "CUSTOM";
}

/// <summary>分账单状态（官方 <c>state</c>）。</summary>
public static class ProfitSharingOrderStates
{
    /// <summary>处理中（官方 <c>PROCESSING</c>）：<b>非终态</b>，可稍后再次查询直到 <c>FINISHED</c>。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>
    /// 分账完成（官方 <c>FINISHED</c>）：终态。
    /// <b>仅代表分账动账执行完毕</b>，各接收方是否成功须看 <c>receivers[].result</c>。
    /// </summary>
    public const string Finished = "FINISHED";
}

/// <summary>单个接收方的分账结果（官方 <c>receivers[].result</c>）。</summary>
public static class ProfitSharingReceiverResults
{
    /// <summary>待分账（官方 <c>PENDING</c>）：非终态。</summary>
    public const string Pending = "PENDING";

    /// <summary>分账成功（官方 <c>SUCCESS</c>）：终态。</summary>
    public const string Success = "SUCCESS";

    /// <summary>已关闭（官方 <c>CLOSED</c>）：终态；随附 <c>fail_reason</c>。</summary>
    public const string Closed = "CLOSED";
}

/// <summary>分账失败原因（官方 <c>receivers[].fail_reason</c>，8 值）。</summary>
/// <remarks>
/// 官方枚举照录（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524936"/>）；
/// 官方说明「<c>result</c> 为 <c>CLOSED</c> 时返回」。
/// </remarks>
public static class ProfitSharingFailReasons
{
    /// <summary>分账接收账户异常（官方 <c>ACCOUNT_ABNORMAL</c>）。</summary>
    public const string AccountAbnormal = "ACCOUNT_ABNORMAL";

    /// <summary>分账关系已解除（官方 <c>NO_RELATION</c>）。</summary>
    public const string NoRelation = "NO_RELATION";

    /// <summary>高风险接收方（官方 <c>RECEIVER_HIGH_RISK</c>）。</summary>
    public const string ReceiverHighRisk = "RECEIVER_HIGH_RISK";

    /// <summary>接收方未实名（官方 <c>RECEIVER_REAL_NAME_NOT_VERIFIED</c>）。</summary>
    public const string ReceiverRealNameNotVerified = "RECEIVER_REAL_NAME_NOT_VERIFIED";

    /// <summary>分账权限已解除（官方 <c>NO_AUTH</c>）。</summary>
    public const string NoAuth = "NO_AUTH";

    /// <summary>超出用户月收款限额（官方 <c>RECEIVER_RECEIPT_LIMIT</c>）。</summary>
    public const string ReceiverReceiptLimit = "RECEIVER_RECEIPT_LIMIT";

    /// <summary>分出方账户异常（官方 <c>PAYER_ACCOUNT_ABNORMAL</c>）。</summary>
    public const string PayerAccountAbnormal = "PAYER_ACCOUNT_ABNORMAL";

    /// <summary>描述参数设置失败（官方 <c>INVALID_REQUEST</c>）。</summary>
    public const string InvalidRequest = "INVALID_REQUEST";
}

/// <summary>
/// 分账回退结果（官方 <c>result</c>，请求/查询分账回退共用）。
/// </summary>
/// <remarks>
/// <b>与分账单状态是两套枚举，勿混用</b>：分账单是 <c>PROCESSING</c>/<c>FINISHED</c>（见
/// <see cref="ProfitSharingOrderStates"/>），回退单是 <c>PROCESSING</c>/<c>SUCCESS</c>/<c>FAILED</c>。
/// 二者都有 <c>PROCESSING</c> 但终态语义完全不同（官方原文）。
/// </remarks>
public static class ProfitSharingReturnResults
{
    /// <summary>处理中（官方 <c>PROCESSING</c>）：非终态。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>已成功（官方 <c>SUCCESS</c>）：终态。</summary>
    public const string Success = "SUCCESS";

    /// <summary>已失败（官方 <c>FAILED</c>）：终态；随附 <c>fail_reason</c>。</summary>
    public const string Failed = "FAILED";
}

/// <summary>
/// 分账回退失败原因（官方 <c>fail_reason</c>，5 值）。
/// </summary>
/// <remarks>
/// 官方注明<b>仅</b> <c>result=FAILED</c> 时返回
/// （<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_4.shtml"/>）。
/// 与分账单失败原因（<see cref="ProfitSharingFailReasons"/>，8 值）<b>不是同一张表</b>。
/// </remarks>
public static class ProfitSharingReturnFailReasons
{
    /// <summary>原分账接收方账户异常（官方 <c>ACCOUNT_ABNORMAL</c>）。</summary>
    public const string AccountAbnormal = "ACCOUNT_ABNORMAL";

    /// <summary>余额不足（官方 <c>BALANCE_NOT_ENOUGH</c>）。</summary>
    public const string BalanceNotEnough = "BALANCE_NOT_ENOUGH";

    /// <summary>超时关单（官方 <c>TIME_OUT_CLOSED</c>）。</summary>
    public const string TimeOutClosed = "TIME_OUT_CLOSED";

    /// <summary>原分账分出方账户异常（官方 <c>PAYER_ACCOUNT_ABNORMAL</c>）。</summary>
    public const string PayerAccountAbnormal = "PAYER_ACCOUNT_ABNORMAL";

    /// <summary>描述参数设置失败（官方 <c>INVALID_REQUEST</c>）。</summary>
    public const string InvalidRequest = "INVALID_REQUEST";
}
