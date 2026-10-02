// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using Mud.HttpUtils.Payloads;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调事件载荷读取器：把事件信封解析为<b>状态化</b>的强类型载荷。
/// </summary>
/// <remarks>
/// <para>
/// <b>单次解析（ADR-6）</b>：与 <see cref="WechatCallbackReceiver"/> 共享
/// <see cref="WechatPayloadSourceCache"/>，每个事件的 XML 解析与节点投影各发生<b>恰好一次</b>；
/// 多处理器并发读取同一事件时亦只投影一次。
/// </para>
/// <para>
/// <b>不抛（B1/B10）</b>：键不匹配、明文缺失、明文非 XML、契约错配全部以
/// <see cref="WechatPayloadReadStatus"/> 表达 —— 沿用旧解析器「返回 <c>null</c> 不抛」的语义，
/// 但把 5 种失败从「一个 <c>null</c>」拆开（ADR-7）。
/// </para>
/// <para>
/// <b>未知事件降级（ADR-4）</b>：事件键未登记契约 ⇒ 降级为 <see cref="GenericCallbackPayload"/>
/// （<see cref="WechatPayloadReadStatus.GenericFallback"/>），<c>Values</c> 携带全部直系子节点
/// ⇒ 官方新增事件当天即可被结构化读取，无需 SDK 发版。
/// </para>
/// </remarks>
public sealed class WechatCallbackPayloadReader : IWechatPayloadReader
{
    private readonly IWechatPayloadContractRegistry _registry;
    private readonly WechatPayloadSourceCache _sources;

    /// <summary>创建载荷读取器。</summary>
    /// <param name="registry">事件键契约注册表（组合根期登记）。</param>
    /// <param name="sourceCache">源缓存（默认与接收器共享 <see cref="WechatPayloadSourceCache.Shared"/>）。</param>
    /// <remarks>
    /// 构造函数为 <c>internal</c>：<see cref="WechatPayloadSourceCache"/> 是实现细节（internal），
    /// 而本类是公开类型 —— 若两者都为 public 会把缓存细节泄漏进公共 API 面。
    /// 宿主经 DI 取用（<c>AddWechatCallback</c> 内以工厂委托注册），不应自行 new。
    /// </remarks>
    internal WechatCallbackPayloadReader(
        IWechatPayloadContractRegistry registry,
        WechatPayloadSourceCache? sourceCache = null)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _sources = sourceCache ?? WechatPayloadSourceCache.Shared;
    }

    /// <inheritdoc />
    public WechatPayloadReadResult<TPayload> Read<TPayload>(WechatCallbackEvent evt)
        where TPayload : WechatCallbackPayload
    {
        var result = ReadCore(evt, typeof(TPayload));

        // 降级路径也<b>携带载荷</b>（GenericCallbackPayload），不得因状态非 Matched 而丢弃。
        if (result.Payload is not TPayload typed)
        {
            return WechatPayloadReadResult<TPayload>.Failed(result.Status, result.EventTypeKey, result.Diagnostic);
        }

        if (result.Status == WechatPayloadReadStatus.Matched)
        {
            return WechatPayloadReadResult<TPayload>.Matched(typed, result.EventTypeKey);
        }

        if (result.Status == WechatPayloadReadStatus.GenericFallback)
        {
            return WechatPayloadReadResult<TPayload>.Generic(typed, result.EventTypeKey);
        }

        return WechatPayloadReadResult<TPayload>.Failed(result.Status, result.EventTypeKey, result.Diagnostic);
    }

    /// <inheritdoc />
    public WechatPayloadReadResult<WechatCallbackPayload> Read(WechatCallbackEvent evt, Type payloadType)
    {
        if (payloadType == null)
            throw new ArgumentNullException(nameof(payloadType));

        return ReadCore(evt, payloadType);
    }

    private WechatPayloadReadResult<WechatCallbackPayload> ReadCore(WechatCallbackEvent? evt, Type payloadType)
    {
        if (evt == null)
        {
            return WechatPayloadReadResult<WechatCallbackPayload>.Failed(
                WechatPayloadReadStatus.EnvelopeMissing, string.Empty, "事件为 null。");
        }

        if (string.IsNullOrEmpty(evt.DecryptedXml))
        {
            return WechatPayloadReadResult<WechatCallbackPayload>.Failed(
                WechatPayloadReadStatus.EnvelopeMissing, evt.EventTypeKey, "信封未携带解密明文。");
        }

        var root = _sources.GetOrCreate(evt).Projected;
        if (root == null)
        {
            return WechatPayloadReadResult<WechatCallbackPayload>.Failed(
                WechatPayloadReadStatus.MalformedPayload, evt.EventTypeKey, "解密明文不是合法的 XML。");
        }

        var key = evt.EventTypeKey;
        if (string.IsNullOrEmpty(key))
        {
            return WechatPayloadReadResult<WechatCallbackPayload>.Failed(
                WechatPayloadReadStatus.KeyMismatch, string.Empty,
                "事件键不可判别（InfoType / ChangeType / Event 三者皆空）。");
        }

        // 未登记契约 ⇒ 降级为通用载荷（ADR-4）
        if (!_registry.TryResolve(key!, out var contract) || contract == null)
        {
            return FallbackGeneric(root, evt, key!);
        }

        // 纵深防御：族前置条件（B4）。分发器的事件键级闸已判一次，读取路径再判一次 ——
        // 使直接调用读取器（不经分发器）的路径同样具备「同名 ChangeType 不得跨族串门」的约束。
        if (!contract.MatchesEnvelope(evt))
        {
            return WechatPayloadReadResult<WechatCallbackPayload>.Failed(
                WechatPayloadReadStatus.ContractMismatch, key!,
                "报文的 Event（" + evt.Event + "）/事件族（" + evt.EventFamily + "）与契约 " +
                contract.EventTypeKey + " 声明的族前置条件不一致。");
        }

        if (contract.Accessor.PayloadType != payloadType)
        {
            return WechatPayloadReadResult<WechatCallbackPayload>.Failed(
                WechatPayloadReadStatus.ContractMismatch, key!,
                "事件 " + key + " 的契约载荷为 " + contract.Accessor.PayloadType.Name +
                "，与请求的 " + payloadType.Name + " 不一致（宿主接线错误）。");
        }

        var accessor = contract.Accessor;
        var payload = accessor.CreateInstance();
        accessor.Bind(root, payload);
        var scope = accessor.ResolveScope(root);

        if (payload is WechatCallbackPayload typed)
        {
            WechatPayloadMaterializer.Fill(typed, key!, evt.EventFamily, scope);
        }

        return WechatPayloadReadResult<WechatCallbackPayload>.Matched(
            payload as WechatCallbackPayload ?? new GenericCallbackPayload(), key!);
    }

    private static WechatPayloadReadResult<WechatCallbackPayload> FallbackGeneric(
        PayloadNode root, WechatCallbackEvent evt, string key)
    {
        var map = GenericCallbackPayload.PayloadFieldMap;
        var payload = new GenericCallbackPayload();
        map.Bind(root, payload);

        WechatPayloadMaterializer.Fill(payload, key, evt.EventFamily, map.ResolveScope(root));

        return WechatPayloadReadResult<WechatCallbackPayload>.Generic(payload, key);
    }
}
