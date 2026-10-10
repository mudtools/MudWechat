// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Enums;

/// <summary>
/// 微信小店 / 视频号（channels 生态）业务错误码（HTTP 200 + errcode 通道；取值以官方各端点错误码表为准）。
/// </summary>
/// <remarks>
/// <para>
/// 令牌失效码集合 <c>{40001, 40014, 42001}</c> 与 <c>ChannelsTokenInvalidationDetector</c> 的判定集合
/// 保持一致（组件 errcode 恢复链路）；其余非零 errcode 走业务异常
/// （<see cref="Exceptions.WechatChannelsException"/>）。
/// </para>
/// <para>
/// <b>40001 的双义（本 SDK 的显式裁决）</b>：官方语义为「获取 access_token 时 AppSecret 错误，
/// <b>或 access_token 无效 / 非最新</b>」——同一码跨「签发失败」与「令牌失效」两义。
/// 令牌签发接口（<c>IChannelsAuthentication</c>）<b>不带 [Token]、不进恢复链路</b>，其 40001 由
/// <see cref="Exceptions.WechatChannelsException"/> 抛业务异常；业务端点（带 <c>[Token]</c>）的 40001
/// 归入失效码集合，交恢复链路自动刷新重试（尤其覆盖「token 已被其它进程刷新为非最新」场景）。
/// </para>
/// <para>
/// <b>本文件只保留 P0 令牌基座所需的通用码段</b>（与公众号线共用同一套 /cgi-bin 基础面码表）：
/// 各业务域专属错误码（商品 / 订单 / 售后 / 资金等）随 P1/P2 逐域核验官方文档后按域补齐。
/// </para>
/// </remarks>
public static class ChannelsErrorCodes
{
    /// <summary>成功。</summary>
    public const int Success = 0;

    /// <summary>系统繁忙（官方 -1，请开发者稍候再试）。</summary>
    public const int SystemError = -1;

    /// <summary>access_token 无效 / 非最新（亦用于签发接口的 AppSecret 错误）。</summary>
    public const int InvalidCredential = 40001;

    /// <summary>不合法的凭证类型（grant_type）。</summary>
    public const int InvalidGrantType = 40002;

    /// <summary>不合法的 AppID（小店 AppID wx 开头但与公众号/小程序 AppID 不互通）。</summary>
    public const int InvalidAppId = 40013;

    /// <summary>无效的 AppSecret。</summary>
    public const int InvalidSecret = 40125;

    /// <summary>调用接口的 IP 地址不在白名单中。</summary>
    public const int IpNotInWhitelist = 40164;

    /// <summary>缺少 appid 参数。</summary>
    public const int MissingAppId = 41002;

    /// <summary>缺少 secret 参数。</summary>
    public const int MissingAppSecret = 41004;

    /// <summary>需要 POST 请求（<c>getStableAccessToken</c> 仅支持 POST）。</summary>
    public const int RequirePostMethod = 43002;

    /// <summary>AppSecret 已被冻结（需解冻后再次调用）。</summary>
    public const int FrozenSecret = 40243;

    /// <summary>access_token 无效。</summary>
    public const int InvalidAccessToken = 40014;

    /// <summary>access_token 过期。</summary>
    public const int ExpiredAccessToken = 42001;

    /// <summary>调用超过天级频率限制（可调用 <c>clear_quota</c> 恢复额度）。</summary>
    public const int DailyQuotaExceeded = 45009;

    /// <summary>API 调用太频繁（分钟级配额）。</summary>
    public const int MinuteQuotaExceeded = 45011;

    /// <summary>此次调用需要管理员确认。</summary>
    public const int AdminConfirmationRequired = 89503;

    /// <summary>该 IP 调用请求已被小店管理员拒绝（请 24 小时后再试）。</summary>
    public const int IpRejectedRetryAfterDay = 89506;

    /// <summary>该 IP 调用请求已被小店管理员拒绝（请 1 小时后再试）。</summary>
    public const int IpRejectedRetryAfterHour = 89507;

    // ---- Basic 域（设计方案 v1 P1：openApi 管理 + callback check + 双 IP，官方错误码核验）----

    /// <summary>未设置回调 URL（callback/check）。</summary>
    public const int CallbackUrlNotSet = 40201;

    /// <summary>非法 action（callback/check，官方 40202）。</summary>
    public const int InvalidCheckAction = 40202;

    /// <summary>非法运营商参数（callback/check，官方 40203）。</summary>
    public const int InvalidCheckOperator = 40203;

    /// <summary>清零次数达到上限（clear_quota / clear_quota/v2，官方 48006；每账号每月 10 次清零机会）。</summary>
    public const int ClearQuotaLimitReached = 48006;

    /// <summary>rid 不存在（openapi/rid/get，官方 76001；rid 有效期仅 7 天）。</summary>
    public const int RidNotFound = 76001;

    /// <summary>rid 为空或格式错误（openapi/rid/get，官方 76002）。</summary>
    public const int RidInvalid = 76002;

    /// <summary>无权查询（openapi/rid/get，官方 76003；rid 属其他账号）。</summary>
    public const int RidPermissionDenied = 76003;

    /// <summary>rid 过期（openapi/rid/get，官方 76004；仅支持 7 天内）。</summary>
    public const int RidExpired = 76004;

    /// <summary>cgi_path not found（openapi/quota/get|clear，官方 76021）。</summary>
    public const int CgiPathNotFound = 76021;

    /// <summary>无权限使用该 cgi_path（openapi/quota/get|clear，官方 76022；token 与 api 所属账号不符）。</summary>
    public const int CgiPathPermissionDenied = 76022;

    // ---- Funds 域（设计方案 v1 P1：资金结算 16 端点，官方错误码核验）----

    /// <summary>token 太长（qrcode/get，官方 -2；二维码 ticket 参数超长）。</summary>
    public const int QrcodeTokenTooLong = -2;

    /// <summary>违规行为，橱窗被禁止使用（getwithdrawlist，官方 10022002；请前往「带货中心-&gt;个人中心-&gt;带货权限」检查橱窗带货权限）。</summary>
    public const int WindowForbidden = 10022002;

    /// <summary>暂无数据（getfundsflowdetail，官方 10021302）。</summary>
    public const int FundsFlowNotFound = 10021302;

    /// <summary>错误的 ticket（qrcode/get，官方 60208）。</summary>
    public const int QrcodeTicketInvalid = 60208;

    /// <summary>ticket 已失效（qrcode/get，官方 60220）。</summary>
    public const int QrcodeTicketExpired = 60220;

    /// <summary>参数错误（listorderflow，官方 669900000；具体查看 errmsg）。</summary>
    public const int OrderFlowParamError = 669900000;

    /// <summary>系统异常（listorderflow，官方 669900001；请重试）。</summary>
    public const int OrderFlowSystemError = 669900001;

    /// <summary>暂无数据（getcity / getbanklist / getsubbranch / getbankbynum，官方 9710001）。</summary>
    public const int BankDataNotFound = 9710001;
}