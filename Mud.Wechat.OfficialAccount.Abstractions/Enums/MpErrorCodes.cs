// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Enums;

/// <summary>
/// 微信公众号 / 服务号业务错误码（HTTP 200 + errcode 通道；取值以官方各端点错误码表为准）。
/// </summary>
/// <remarks>
/// <para>
/// 令牌失效码集合 <c>{40001, 40014, 42001}</c> 与 <c>MpTokenInvalidationDetector</c> 的判定集合
/// 保持一致（组件 errcode 恢复链路）；其余非零 errcode 走业务异常
/// （<see cref="Exceptions.MpException"/>）。
/// </para>
/// <para>
/// <b>40001 的双义（本 SDK 的显式裁决）</b>：官方语义为「获取 access_token 时 AppSecret 错误，
/// <b>或 access_token 无效 / 非最新</b>」——同一码跨「签发失败」与「令牌失效」两义。
/// 令牌签发接口（<c>IMpAuthentication</c>）<b>不带 [Token]、不进恢复链路</b>，其 40001 由
/// <see cref="Exceptions.MpException"/> 抛业务异常；业务端点（带 <c>[Token]</c>）的 40001
/// 归入失效码集合，交恢复链路自动刷新重试（尤其覆盖「token 已被其它进程刷新为非最新」场景）。
/// </para>
/// </remarks>
public static class MpErrorCodes
{
    /// <summary>成功。</summary>
    public const int Success = 0;

    /// <summary>系统繁忙（官方 -1，请开发者稍候再试）。</summary>
    public const int SystemError = -1;

    /// <summary>access_token 无效 / 非最新（亦用于签发接口的 AppSecret 错误）。</summary>
    public const int InvalidCredential = 40001;

    /// <summary>不合法的凭证类型（grant_type）。</summary>
    public const int InvalidGrantType = 40002;

    /// <summary>不合法的 AppID。</summary>
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

    /// <summary>该 IP 调用请求已被公众号管理员拒绝（请 24 小时后再试）。</summary>
    public const int IpRejectedRetryAfterDay = 89506;

    /// <summary>该 IP 调用请求已被公众号管理员拒绝（请 1 小时后再试）。</summary>
    public const int IpRejectedRetryAfterHour = 89507;

    /// <summary>网络通信检测：未设置回调 URL。</summary>
    public const int CallbackCheckInvalidUrl = 40201;

    /// <summary>网络通信检测：不正确的 action 参数。</summary>
    public const int CallbackCheckInvalidAction = 40202;

    /// <summary>网络通信检测：不正确的运营商参数。</summary>
    public const int CallbackCheckInvalidOperator = 40203;

    // ---------------------------------------------------------------- 用户管理·标签管理（M2，取值逐页核验官方文档）

    /// <summary>invalid openid：不合法的 OpenID（确认该用户是否已关注公众号，或是否为其他公众号的 OpenID）。</summary>
    public const int InvalidOpenId = 40003;

    /// <summary>invalid openid list size：不合法的 openid 列表长度。</summary>
    public const int InvalidOpenIdListSize = 40032;

    /// <summary>empty post data：传递的参数为空（如删除标签未带 <c>tag.id</c>）。</summary>
    public const int EmptyPostData = 44002;

    /// <summary>reach max api daily quota limit：超出接口每日调用限制。</summary>
    public const int DailyQuotaReached = 45009;

    /// <summary>标签数超限（官方：一个公众号最多可以创建 100 个标签，适当删减标签）。</summary>
    public const int TagCountExceeded = 45056;

    /// <summary>can't modify sys tag：禁止修改系统标签。</summary>
    public const int SystemTagImmutable = 45058;

    /// <summary>单用户标签数超过限制（官方：标签功能支持公众号为用户打上最多 20 个标签）。</summary>
    public const int UserTagCountExceeded = 45059;

    /// <summary>invalid tag name：检查标签名。</summary>
    public const int InvalidTagName = 45157;

    /// <summary>tag name too long：调小标签名长度（官方限制 30 个字符以内）。</summary>
    public const int TagNameTooLong = 45158;

    /// <summary>invalid tag id：非法的标签。</summary>
    public const int InvalidTagId = 45159;

    /// <summary>openid much req：一般是因为对同个 openid 并发打标 / 取消标签导致（应串行化同一 openid 的标签变更）。</summary>
    public const int ConcurrentTaggingConflict = 45169;

    /// <summary>
    /// some openid fail：<b>部分</b> openid 失败（响应体 <c>fail_openid_list</c> 给出失败的 openid，可定向重试）。
    /// </summary>
    /// <remarks>整批重放会对已成功的 openid 重复打标，故调用方应读取 <c>fail_openid_list</c> 做定向重试。</remarks>
    public const int SomeOpenIdFailed = 45171;

    /// <summary>post data format error：参数格式错误。</summary>
    public const int PostDataFormatError = 47001;

    /// <summary>api unauthorized：接口功能未授权（可在「公众平台官网 - 开发者中心页」查看接口权限）。</summary>
    public const int ApiUnauthorized = 48001;

    /// <summary>not match openid with appid：传入的 openid 不属于此 AppID。</summary>
    public const int OpenIdAppIdMismatch = 49003;

    /// <summary>user limited：用户受限，可能是用户账号被冻结或注销。</summary>
    public const int UserLimited = 50002;

    /// <summary>user is unsubscribed：用户未关注公众号。</summary>
    public const int UserUnsubscribed = 50005;

    /// <summary>access clientip is not registered：第三方平台出口 IP 未设置（仅第三方平台调用场景）。</summary>
    public const int ThirdPartyClientIpNotRegistered = 61004;

    // ---------------------------------------------------------------- 用户管理·用户信息（M2，取值逐页核验官方文档）

    /// <summary>invalid remark name：备注名非法（官方限制「长度必须小于 30 字节」）。</summary>
    public const int InvalidRemarkName = 40092;

    /// <summary>require subscribe：该 openid 未关注当前账号（设置备注名等场景）。</summary>
    public const int RequireSubscribe = 43004;

    /// <summary>
    /// 平台级「系统繁忙」码（拉黑用户接口，官方解决方案「稍后重试」）——属<b>可重试</b>错误。
    /// </summary>
    /// <remarks>官方新增码段（非 4xxxx / 6xxxx 段），故单列常量并注明可重试语义。</remarks>
    public const int BatchBlacklistSystemBusy = 268487001;

    /// <summary>平台级「系统繁忙」码（获取关注者列表接口，官方解决方案「稍后重试」）——属<b>可重试</b>错误。</summary>
    public const int GetFansSystemBusy = 268487002;
}
