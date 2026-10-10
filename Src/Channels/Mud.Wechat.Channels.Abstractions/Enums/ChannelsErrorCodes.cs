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
}