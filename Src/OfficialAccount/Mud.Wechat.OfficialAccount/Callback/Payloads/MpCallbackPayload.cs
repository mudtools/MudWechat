// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 公众号回调事件载荷基类。
/// </summary>
/// <remarks>
/// <para>
/// <b>不含映射表</b>：字段映射契约由上游 <c>Mud.HttpUtils.Payloads.IPayloadFieldMap&lt;T&gt;</c> 承载并经
/// <see cref="MpPayloadContract"/> 在组合根期登记，载荷类型只保留「数据 + 读面」。
/// </para>
/// <para>
/// <b>跨程序集不变式</b>：两个属性为 <c>internal set</c>，由本程序集内的物化器填写；
/// 调用方（<c>Mud.Wechat.OfficialAccount.Callback</c>）凭 <c>InternalsVisibleTo</c> 获得读权限。
/// </para>
/// </remarks>
public abstract class MpCallbackPayload
{
    /// <summary>事件类型键（= <see cref="MpCallbackEnvelope.EventTypeKey"/>：<c>Event</c> 优先 → <c>MsgType</c>）。</summary>
    public string EventTypeKey { get; internal set; } = string.Empty;

    /// <summary>
    /// 生效作用域下直系子节点的「元素名 → 全后代文本」全量只读视图。
    /// </summary>
    /// <remarks>官方新增字段无需 SDK 发版即可读取；供宿主读取未建模字段（读取状态 <c>GenericFallback</c> 时为主要数据来源）。</remarks>
    public IReadOnlyDictionary<string, string> Values { get; internal set; } =
        new Dictionary<string, string>();
}

/// <summary>
/// 未知事件降级载荷：零字段绑定，仅承载 <see cref="MpCallbackPayload.Values"/> 全量读面。
/// </summary>
/// <remarks>
/// 事件键未登记契约时读取器返回本类型（<see cref="MpPayloadReadStatus.GenericFallback"/>），
/// 使「官方新增事件」「待核验事件族」或「宿主私有事件」在<b>零 SDK 改动</b>下仍以结构化形态到达宿主。
/// </remarks>
public sealed class GenericCallbackPayload : MpCallbackPayload
{
    /// <summary>零绑定映射契约（上游 <c>PayloadFieldMap&lt;T&gt;.Create</c>，无 <c>Map</c> 调用）。</summary>
    public static IPayloadFieldMap<GenericCallbackPayload> PayloadFieldMap { get; } =
        PayloadFieldMap<GenericCallbackPayload>.Create(nameof(GenericCallbackPayload));
}

/// <summary>
/// 载荷读取状态（<b>不抛</b>语义：失败以状态表达，见方案 §2.3/§10.2 用例 10）。
/// </summary>
public enum MpPayloadReadStatus
{
    /// <summary>按事件键命中契约并按类型绑定成功。</summary>
    Matched = 0,

    /// <summary>事件键未登记契约（或处理器以 <see cref="GenericCallbackPayload"/> 声明 ⇒ 正常降级，非失败）。</summary>
    GenericFallback = 1,

    /// <summary>信封缺失（<c>null</c> 或未携带解密明文）。</summary>
    EnvelopeMissing = 2,

    /// <summary>解密明文不是合法 XML（协议外报文）。</summary>
    MalformedPayload = 3,

    /// <summary>事件键不可判别（<c>Event</c> 与 <c>MsgType</c> 皆空）。</summary>
    KeyMismatch = 4,

    /// <summary>契约载荷类型与请求类型不一致（宿主接线错误）。</summary>
    ContractMismatch = 5,
}

/// <summary>
/// 状态化读取结果（泛型视图）。
/// </summary>
/// <typeparam name="TPayload">请求的载荷类型。</typeparam>
public sealed class MpPayloadReadResult<TPayload>
    where TPayload : MpCallbackPayload
{
    private MpPayloadReadResult(MpPayloadReadStatus status, TPayload? payload, string eventTypeKey, string? diagnostic)
    {
        Status = status;
        Payload = payload;
        EventTypeKey = eventTypeKey;
        Diagnostic = diagnostic;
    }

    /// <summary>读取状态。</summary>
    public MpPayloadReadStatus Status { get; }

    /// <summary>载荷实例（<see cref="MpPayloadReadStatus.Matched"/>/<see cref="MpPayloadReadStatus.GenericFallback"/> 时非空）。</summary>
    public TPayload? Payload { get; }

    /// <summary>事件类型键。</summary>
    public string EventTypeKey { get; }

    /// <summary>失败/降级诊断说明（<c>null</c> 表示无附加说明）。</summary>
    public string? Diagnostic { get; }

    /// <summary>命中契约并绑定成功。</summary>
    /// <param name="payload">绑定后的载荷。</param>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <returns>读取结果。</returns>
    public static MpPayloadReadResult<TPayload> Matched(TPayload payload, string eventTypeKey)
        => new(MpPayloadReadStatus.Matched, payload, eventTypeKey, null);

    /// <summary>未登记契约的通用降级（载荷实际类型为 <see cref="GenericCallbackPayload"/>）。</summary>
    /// <param name="payload">通用降级载荷。</param>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <returns>读取结果。</returns>
    public static MpPayloadReadResult<TPayload> Generic(TPayload payload, string eventTypeKey)
        => new(MpPayloadReadStatus.GenericFallback, payload, eventTypeKey, null);

    /// <summary>失败（<see cref="Payload"/> 为 <c>null</c>）。</summary>
    /// <param name="status">失败状态。</param>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="diagnostic">诊断说明。</param>
    /// <returns>读取结果。</returns>
    public static MpPayloadReadResult<TPayload> Failed(MpPayloadReadStatus status, string eventTypeKey, string? diagnostic)
        => new(status, null, eventTypeKey, diagnostic);
}
