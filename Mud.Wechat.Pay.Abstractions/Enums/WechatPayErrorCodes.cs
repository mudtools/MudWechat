// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Enums;

/// <summary>
/// 微信支付 APIv3 业务错误码（HTTP 4xx/5xx + <c>{"code":"…","message":"…"}</c> 通道）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微 / 公众号的根本差异</b>：APIv3 的错误码是<b>字符串</b>（如 <c>SIGN_ERROR</c>），
/// 既不是 HTTP 200 内嵌 <c>errcode</c>，也不是整数 —— 故本类只提供 <see cref="string"/> 常量，
/// 不复用 <c>MpErrorCodes</c> / 企微 errcode 的整数面。
/// </para>
/// <para>
/// <b>为何支付线没有 errcode 令牌自愈</b>：支付<b>没有 <c>access_token</c></b>（凭据是商户 RSA 私钥签名），
/// 故不存在「令牌失效 → 刷新 → 重试」链路；失败分类一律按官方 <c>code</c> 决策
/// （见设计方案 §2.7）。本类的职责是<b>分类</b>，是否重试由宿主决定。
/// </para>
/// <para>
/// 取值逐条核对官方「返回错误码」页面（普通商户文档中心）。<b>不得据语义「纠正」官方拼写</b>。
/// </para>
/// </remarks>
public static class WechatPayErrorCodes
{
    /// <summary>接口返回系统错误（可重试）。</summary>
    public const string SystemError = "SYSTEM_ERROR";

    /// <summary>签名错误：商户私钥 / 序列号 / 签名串与官方约定不符。</summary>
    public const string SignError = "SIGN_ERROR";

    /// <summary>参数错误：请求体字段缺失、格式非法或取值越界。</summary>
    public const string ParamError = "PARAM_ERROR";

    /// <summary>商户无权限调用该接口（未开通产品 / 权限未生效）。</summary>
    public const string NoAuth = "NO_AUTH";

    /// <summary>账号异常 / 请求不合法（官方多接口共用）。</summary>
    public const string InvalidRequest = "INVALID_REQUEST";

    /// <summary>请求频率超限（官方建议降低请求频率，勿并发重试）。</summary>
    public const string FrequencyLimited = "FREQUENCY_LIMITED";

    /// <summary>接口请求超限（与 <see cref="FrequencyLimited"/> 并存的官方码，语义同为限流）。</summary>
    public const string RateLimited = "RATE_LIMITED";

    /// <summary>账号余额不足（退款 / 转账等资金类接口）。</summary>
    public const string NotEnough = "NOT_ENOUGH";

    /// <summary><c>appid</c> 与 <c>mchid</c> 不匹配（须先在商户平台完成绑定）。</summary>
    public const string AppIdMchIdNotMatch = "APPID_MCHID_NOT_MATCH";

    /// <summary>商户订单号重复（同一商户号下 <c>out_trade_no</c> 唯一）。</summary>
    public const string OutTradeNoUsed = "OUT_TRADE_NO_USED";

    /// <summary>订单不存在。</summary>
    public const string OrderNotExist = "ORDER_NOT_EXIST";

    /// <summary>订单已关闭。</summary>
    public const string OrderClosed = "ORDER_CLOSED";

    /// <summary>商户不存在。</summary>
    public const string MchNotExists = "MCH_NOT_EXISTS";

    /// <summary>资源已存在（重复创建）。</summary>
    public const string ResourceAlreadyExists = "RESOURCE_ALREADY_EXISTS";

    /// <summary>资源不存在。</summary>
    public const string ResourceNotExists = "RESOURCE_NOT_EXISTS";

    /// <summary>账单不存在（申请账单：指定日期无可下载账单）。</summary>
    public const string BillNotExist = "BILL_NOT_EXIST";

    /// <summary>账单文件不存在或已过期（下载账单：<c>download_url</c> 有效期仅 5 分钟）。</summary>
    public const string FileNotExist = "FILE_NOT_EXIST";

    /// <summary>交易单号非法。</summary>
    public const string InvalidTransactionId = "INVALID_TRANSACTIONID";

    /// <summary>退款单不存在（查询单笔退款 / 发起异常退款）。</summary>
    public const string RefundNotExist = "REFUND_NOT_EXIST";

    /// <summary>退款金额非法（超过可退金额或格式错误）。</summary>
    public const string RefundFeeInvalid = "REFUND_FEE_INVALID";

    /// <summary>当前订单状态不支持退款。</summary>
    public const string TransactionNotSupportRefund = "TRANSACTION_NOT_SUPPORT_REFUND";

    /// <summary>商户退款单号重复。</summary>
    public const string OutRefundNoUsed = "OUT_REFUND_NO_USED";

    /// <summary>用户账号异常（退款需用户确认或账号被冻结）。</summary>
    public const string UserAccountAbnormal = "USER_ACCOUNT_ABNORMAL";

    /// <summary><c>openid</c> 与商户 <c>appid</c> 不匹配。</summary>
    public const string OpenIdMismatch = "OPENID_MISMATCH";

    /// <summary>请求参数过长（官方 <c>INVALID_REQ_TOO_LONG</c>）。</summary>
    public const string InvalidRequestTooLong = "INVALID_REQ_TOO_LONG";
}
