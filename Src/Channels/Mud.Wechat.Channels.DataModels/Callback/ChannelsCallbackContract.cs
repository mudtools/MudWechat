// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 微信小店回调载荷的事件键声明（契约登记<b>单一来源</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 载荷类标本特性后，契约登记生成器（中立工具 <c>Mud.Wechat.Callback.Generator</c> 的小店档位）按
/// 登记宿主签名推断档位，编译期发射 <c>ChannelsPayloadContracts.RegisterAll</c> 方法体 ——
/// 「特性声明」与「注册表内容」不会漂移（手写登记表是禁止形态）。
/// </para>
/// <para>
/// <b>同键多声明</b>：<c>AllowMultiple = true</c>，同一载荷服务于多个事件键时逐键声明；同键重复声明由生成器去重。
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ChannelsCallbackContractAttribute : Attribute
{
    /// <summary>本载荷覆盖的事件类型键集合（官方解密明文顶层 <c>Event</c> 取值，建议引用常量类）。</summary>
    public string[] EventTypes { get; set; } = Array.Empty<string>();
}

/// <summary>
/// 事件键级载荷契约（小店侧，JSON 反序列化形态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微 / 公众号契约的差异（有意为之）</b>：小店回调明文为<b> JSON</b>（官方强制安全模式），
/// 载荷经源生成 JSON 上下文直接反序列化，<b>不经</b> XML 字段映射器（<c>[PayloadContract]</c> /
/// <c>IPayloadContractAccessor</c> 机制不适用）—— 故契约只有「事件键 + 载荷类型」两项。
/// </para>
/// <para>
/// 官方新增事件键若不登记契约，类型化处理器读取不到载荷（分发器记 Error 跳过该处理器），
/// 信封级处理器不受影响；键登记后处理器即获得强类型载荷。
/// </para>
/// </remarks>
public sealed class ChannelsPayloadContract
{
    private ChannelsPayloadContract(string eventTypeKey, Type payloadType)
    {
        EventTypeKey = eventTypeKey;
        PayloadType = payloadType;
    }

    /// <summary>事件类型键（= 回调事件信封的 <c>EventTypeKey</c>）。</summary>
    public string EventTypeKey { get; }

    /// <summary>载荷类型（须登记进本线源生成 JSON 上下文，AOT 可解析）。</summary>
    public Type PayloadType { get; }

    /// <summary>创建契约。</summary>
    /// <param name="eventTypeKey">事件类型键（须为官方 <c>Event</c> 取值）。</param>
    /// <param name="payloadType">载荷类型。</param>
    /// <returns>契约实例。</returns>
    /// <exception cref="ArgumentException"><paramref name="eventTypeKey"/> 为空白。</exception>
    public static ChannelsPayloadContract Create(string eventTypeKey, Type payloadType)
    {
        if (string.IsNullOrWhiteSpace(eventTypeKey))
        {
            throw new ArgumentException("事件键不能为空（契约登记以事件键为唯一索引）。", nameof(eventTypeKey));
        }

        if (payloadType == null)
        {
            throw new ArgumentNullException(nameof(payloadType));
        }

        return new ChannelsPayloadContract(eventTypeKey, payloadType);
    }

}

/// <summary>
/// 事件键 → 载荷契约注册表（组合根期登记，运行期只读）。
/// </summary>
public interface IChannelsPayloadContractRegistry
{
    /// <summary>登记（同键重复登记以最后一次为准；特性同键合并由生成器负责）。</summary>
    /// <param name="contract">载荷契约。</param>
    void Register(ChannelsPayloadContract contract);

    /// <summary>按事件键解析契约。</summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="contract">命中的契约；未命中为 <c>null</c>。</param>
    /// <returns>命中返回 <c>true</c>。</returns>
    bool TryResolve(string eventTypeKey, out ChannelsPayloadContract? contract);

    /// <summary>已登记的事件键集合（<b>防静默空跑</b>：守卫与测试据此断言登记面下限）。</summary>
    IReadOnlyCollection<string> RegisteredKeys { get; }
}

/// <summary>
/// 契约注册表默认实现（并发字典；组合根期写入、运行期只读）。
/// </summary>
public sealed class ChannelsPayloadContractRegistry : IChannelsPayloadContractRegistry
{
    private readonly ConcurrentDictionary<string, ChannelsPayloadContract> _contracts =
        new(StringComparer.Ordinal);

    /// <inheritdoc />
    public void Register(ChannelsPayloadContract contract)
    {
        if (contract == null)
        {
            throw new ArgumentNullException(nameof(contract));
        }

        _contracts[contract.EventTypeKey] = contract;
    }

    /// <inheritdoc />
    public bool TryResolve(string eventTypeKey, out ChannelsPayloadContract? contract)
    {
        contract = null;
        if (string.IsNullOrEmpty(eventTypeKey))
        {
            return false;
        }

        return _contracts.TryGetValue(eventTypeKey, out contract);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> RegisteredKeys => _contracts.Keys.ToArray();
}

/// <summary>
/// 载荷契约登记宿主（生成器的发射目标；方法体由生成器补齐）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何宿主落 DataModels</b>：载荷 DTO 与 <c>[ChannelsCallbackContract]</c> 声明都在本程序集，
/// 生成器「只挂载荷工程」（对齐企微线先例）；跨程序集发射 partial 实现会产出 CS0759
/// （实现与定义必须同程序集）。
/// </para>
/// <para>
/// 生成器扫描本类型并按其 <c>RegisterAll</c> 签名中的注册表接口类型
/// （<c>IChannelsPayloadContractRegistry</c>）推断小店档位；生成器若未找到任何
/// <c>[ChannelsCallbackContract]</c> 声明将发出编译期诊断（MUDCB106），不会静默产出空方法体。
/// </para>
/// </remarks>
public static partial class ChannelsPayloadContracts
{
    /// <summary>登记全部小店回调载荷契约（调用点：组合根期 <c>AddWechatChannelsCallback</c>）。</summary>
    /// <param name="registry">契约注册表。</param>
    public static partial void RegisterAll(IChannelsPayloadContractRegistry registry);
}
