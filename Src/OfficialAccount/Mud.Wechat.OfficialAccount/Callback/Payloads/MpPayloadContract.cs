// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 事件键级载荷契约（公众号侧）：上游字段映射 + 事件键。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微侧的差异（有意为之）</b>：企微契约承载「族前置条件 + 事件键级开放面」（
/// <c>WechatCallbackEventFamily</c>/<c>WechatAppTypeSet</c>/<c>WechatCallbackChannel</c>），
/// 公众号<b>没有</b>套件授权族与「应用类型 × 回调通道」模型，官方也不按应用模式分通道，
/// 故本类型只有「事件键 + 字段映射访问器」两项 —— 不照搬企微契约（照搬即引入恒为默认值的死字段）。
/// </para>
/// <para>
/// 官方新增事件键若不登记契约，读取器按 <see cref="GenericCallbackPayload"/> 降级（不抛）；
/// 键登记后处理器即可获得强类型载荷（编译期校验字段名与元素名配对）。
/// </para>
/// </remarks>
public sealed class MpPayloadContract
{
    private MpPayloadContract(string eventTypeKey, IPayloadContractAccessor accessor)
    {
        EventTypeKey = eventTypeKey;
        Accessor = accessor;
    }

    /// <summary>事件类型键（= <see cref="MpCallbackEnvelope.EventTypeKey"/>）。</summary>
    public string EventTypeKey { get; }

    /// <summary>上游字段映射契约（非泛型视图：<c>PayloadType</c>/<c>CreateInstance</c>/<c>Bind</c>/<c>ResolveScope</c>）。</summary>
    public IPayloadContractAccessor Accessor { get; }

    /// <summary>创建契约。</summary>
    /// <param name="eventTypeKey">事件类型键（须为官方 <c>MsgType</c>/<c>Event</c> 取值）。</param>
    /// <param name="accessor">载荷字段映射访问器（由 <c>[PayloadContract]</c> 源生成器产出）。</param>
    /// <returns>契约实例。</returns>
    /// <exception cref="ArgumentException"><paramref name="eventTypeKey"/> 为空白。</exception>
    public static MpPayloadContract Create(string eventTypeKey, IPayloadContractAccessor accessor)
    {
        if (string.IsNullOrWhiteSpace(eventTypeKey))
        {
            throw new ArgumentException("事件键不能为空（契约登记以事件键为唯一索引）。", nameof(eventTypeKey));
        }

        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        return new MpPayloadContract(eventTypeKey, accessor);
    }

    /// <summary>报文信封是否满足本契约（事件键一致）。</summary>
    /// <param name="envelope">回调事件信封。</param>
    /// <returns><c>true</c> = 命中。</returns>
    /// <remarks>纵深防御：读取器在按契约绑定前再判一次（分发器已按键匹配过一次）。</remarks>
    public bool MatchesEnvelope(MpCallbackEnvelope envelope)
        => envelope != null && string.Equals(envelope.EventTypeKey, EventTypeKey, StringComparison.Ordinal);
}

/// <summary>
/// 事件键 → 载荷契约注册表（组合根期登记，运行期只读）。
/// </summary>
public interface IMpPayloadContractRegistry
{
    /// <summary>登记（同键重复登记以最后一次为准，`AllowMultiple` 特性同键合并由生成器负责）。</summary>
    /// <param name="contract">载荷契约。</param>
    void Register(MpPayloadContract contract);

    /// <summary>按事件键解析契约。</summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="contract">命中的契约；未命中为 <c>null</c>。</param>
    /// <returns>命中返回 <c>true</c>。</returns>
    bool TryResolve(string eventTypeKey, out MpPayloadContract? contract);

    /// <summary>已登记的事件键集合（<b>防静默空跑</b>：守卫与测试据此断言登记面下限）。</summary>
    IReadOnlyCollection<string> RegisteredKeys { get; }
}

/// <summary>
/// 契约注册表默认实现（并发字典；组合根期写入、运行期只读）。
/// </summary>
public sealed class MpPayloadContractRegistry : IMpPayloadContractRegistry
{
    private readonly ConcurrentDictionary<string, MpPayloadContract> _contracts =
        new(StringComparer.Ordinal);

    /// <inheritdoc />
    public void Register(MpPayloadContract contract)
    {
        if (contract == null)
        {
            throw new ArgumentNullException(nameof(contract));
        }

        _contracts[contract.EventTypeKey] = contract;
    }

    /// <inheritdoc />
    public bool TryResolve(string eventTypeKey, out MpPayloadContract? contract)
    {
        if (string.IsNullOrEmpty(eventTypeKey))
        {
            contract = null;
            return false;
        }

        return _contracts.TryGetValue(eventTypeKey, out contract);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> RegisteredKeys => _contracts.Keys.ToArray();
}
