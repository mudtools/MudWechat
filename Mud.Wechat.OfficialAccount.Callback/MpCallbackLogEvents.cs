// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 公众号回调链路的**结构化日志事件 id**（P10 增强）：为运维告警/埋点提供稳定锚点。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何用稳定 id 而非纯文本</b>：文本随版本漂移，无法作为告警规则/看板的过滤键；
/// id 一旦分配**不得复用或改号**（语义变更请新增 id），否则历史告警口径会被静默改写。
/// </para>
/// <para>号段：<c>5100~5199</c> 保留给公众号回调链路。</para>
/// </remarks>
public static class MpCallbackLogEvents
{
    /// <summary>来源 IP 不在白名单（403）。</summary>
    public static readonly EventId SourceIpRejected = new(5101, nameof(SourceIpRejected));

    /// <summary>回调验证失败（验签/时效/重放/解密/appid/明文拒收）——统一 403 + 空体。</summary>
    public static readonly EventId VerificationFailed = new(5102, nameof(VerificationFailed));

    /// <summary>事件分发软超时（回 <c>success</c> + 200，不触发重推）。</summary>
    public static readonly EventId DispatchTimedOut = new(5103, nameof(DispatchTimedOut));

    /// <summary>回调处理未预期异常（500）。</summary>
    public static readonly EventId UnhandledError = new(5104, nameof(UnhandledError));

    /// <summary>动态来源 IP 白名单已注册但尚无数据（放行并告警一次）。</summary>
    public static readonly EventId SourceIpWhitelistNotReady = new(5105, nameof(SourceIpWhitelistNotReady));

    /// <summary>未找到事件处理器（unhandled，已接收不重推）。</summary>
    public static readonly EventId HandlerNotFound = new(5106, nameof(HandlerNotFound));

    /// <summary>处理器执行失败（已隔离，继续其余处理器）。</summary>
    public static readonly EventId HandlerFailed = new(5107, nameof(HandlerFailed));

    /// <summary>被动回复组装时未命中回调配置。</summary>
    public static readonly EventId ReplyConfigMissing = new(5108, nameof(ReplyConfigMissing));

    /// <summary>回调 AppId 与多应用基座配置不一致（仅告警，按回调配置继续）。</summary>
    public static readonly EventId AppIdCrossCheckMismatch = new(5109, nameof(AppIdCrossCheckMismatch));
}
