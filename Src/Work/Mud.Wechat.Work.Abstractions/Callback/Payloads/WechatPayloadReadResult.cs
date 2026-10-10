// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 载荷读取结果（状态化，取代旧解析器的静默 <c>null</c>）。
/// </summary>
/// <typeparam name="TPayload">目标载荷类型。</typeparam>
/// <remarks>
/// <see cref="Diagnostic"/> 供日志/排障使用，<b>不得</b>包含凭据或解密明文（ADR-9）。
/// </remarks>
public sealed class WechatPayloadReadResult<TPayload>
    where TPayload : WechatCallbackPayload
{
    private WechatPayloadReadResult(
        WechatPayloadReadStatus status,
        TPayload? payload,
        string eventTypeKey,
        string? diagnostic)
    {
        Status = status;
        Payload = payload;
        EventTypeKey = eventTypeKey;
        Diagnostic = diagnostic;
    }

    /// <summary>读取状态。</summary>
    public WechatPayloadReadStatus Status { get; }

    /// <summary>绑定成功的载荷实例；<see cref="Status"/> 既非 <see cref="WechatPayloadReadStatus.Matched"/>
    /// 亦非 <see cref="WechatPayloadReadStatus.GenericFallback"/> 时为 <see langword="null"/>。</summary>
    public TPayload? Payload { get; }

    /// <summary>参与判定的事件类型键。</summary>
    public string EventTypeKey { get; }

    /// <summary>人类可读的诊断信息（不含凭据）。</summary>
    public string? Diagnostic { get; }

    /// <summary>
    /// 是否成功产出载荷：<see cref="Status"/> 为 <see cref="WechatPayloadReadStatus.Matched"/>
    /// 或 <see cref="WechatPayloadReadStatus.GenericFallback"/>。
    /// </summary>
    public bool HasPayload => Payload != null;

    /// <summary>成功（契约命中并绑定完成）。</summary>
    internal static WechatPayloadReadResult<TPayload> Matched(TPayload payload, string key)
        => new(WechatPayloadReadStatus.Matched, payload, key, null);

    /// <summary>
    /// 降级成功（事件键未登记契约 ⇒ 产出 <see cref="GenericCallbackPayload"/>）。
    /// 与 <see cref="Failed"/> 的区别：本状态<b>携带载荷</b>，属可观测降级而非错误。
    /// </summary>
    internal static WechatPayloadReadResult<TPayload> Generic(TPayload payload, string key)
        => new(WechatPayloadReadStatus.GenericFallback, payload, key, null);

    /// <summary>失败（不携带载荷）。</summary>
    internal static WechatPayloadReadResult<TPayload> Failed(
        WechatPayloadReadStatus status, string key, string? diagnostic = null)
        => new(status, null, key, diagnostic);
}
