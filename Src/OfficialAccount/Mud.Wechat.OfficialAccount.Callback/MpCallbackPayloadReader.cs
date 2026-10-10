// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 公众号载荷读取器：按事件键解析为强类型载荷（单次解析、未登记降级、不抛）。
/// </summary>
/// <remarks>
/// <para>
/// <b>单次解析</b>：与接收器共享叶层 <c>WechatPayloadSourceCache</c>（弱键缓存，键 = 信封对象），
/// 每个事件的 XML 解析与节点投影各发生恰好一次。
/// </para>
/// <para>
/// <b>与企微读取器的差异</b>：公众号契约无「族前置条件 / 事件键级开放面」，故本读取器不做族判定
/// （合法性判定在公众号侧不存在对应语义，见方案 D8）。
/// </para>
/// </remarks>
public sealed class MpCallbackPayloadReader : IMpPayloadReader
{
    private readonly IMpPayloadContractRegistry _registry;
    private readonly WechatPayloadSourceCache _sources;

    /// <summary>创建载荷读取器（宿主经 DI 取用；构造函数 <c>internal</c>：缓存是实现细节）。</summary>
    /// <param name="registry">事件键契约注册表（组合根期登记）。</param>
    /// <param name="sourceCache">源缓存（默认与接收器共享 <c>WechatPayloadSourceCache.Shared</c>）。</param>
    internal MpCallbackPayloadReader(
        IMpPayloadContractRegistry registry,
        WechatPayloadSourceCache? sourceCache = null)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _sources = sourceCache ?? WechatPayloadSourceCache.Shared;
    }

    /// <inheritdoc />
    public MpPayloadReadResult<TPayload> Read<TPayload>(MpCallbackEnvelope envelope)
        where TPayload : MpCallbackPayload
    {
        var result = ReadCore(envelope, typeof(TPayload));

        if (result.Payload is not TPayload typed)
        {
            return MpPayloadReadResult<TPayload>.Failed(result.Status, result.EventTypeKey, result.Diagnostic);
        }

        return result.Status == MpPayloadReadStatus.Matched
            ? MpPayloadReadResult<TPayload>.Matched(typed, result.EventTypeKey)
            : MpPayloadReadResult<TPayload>.Generic(typed, result.EventTypeKey);
    }

    /// <inheritdoc />
    public MpPayloadReadResult<MpCallbackPayload> Read(MpCallbackEnvelope envelope, Type payloadType)
    {
        if (payloadType == null)
        {
            throw new ArgumentNullException(nameof(payloadType));
        }

        return ReadCore(envelope, payloadType);
    }

    private MpPayloadReadResult<MpCallbackPayload> ReadCore(MpCallbackEnvelope? envelope, Type payloadType)
    {
        if (envelope == null)
        {
            return MpPayloadReadResult<MpCallbackPayload>.Failed(
                MpPayloadReadStatus.EnvelopeMissing, string.Empty, "事件为 null。");
        }

        if (string.IsNullOrEmpty(envelope.DecryptedXml))
        {
            return MpPayloadReadResult<MpCallbackPayload>.Failed(
                MpPayloadReadStatus.EnvelopeMissing, envelope.EventTypeKey, "信封未携带解密明文。");
        }

        var root = _sources.GetOrCreate(envelope, envelope.DecryptedXml).Projected;
        if (root == null)
        {
            return MpPayloadReadResult<MpCallbackPayload>.Failed(
                MpPayloadReadStatus.MalformedPayload, envelope.EventTypeKey, "解密明文不是合法的 XML。");
        }

        var key = envelope.EventTypeKey;
        if (string.IsNullOrEmpty(key))
        {
            return MpPayloadReadResult<MpCallbackPayload>.Failed(
                MpPayloadReadStatus.KeyMismatch, string.Empty, "事件键不可判别（Event 与 MsgType 皆空）。");
        }

        // 未登记契约 ⇒ 通用降级（官方新增事件/待核验事件族在零 SDK 改动下仍可读）。
        if (!_registry.TryResolve(key!, out var contract) || contract == null)
        {
            return FallbackGeneric(root, key!);
        }

        if (contract.Accessor.PayloadType != payloadType)
        {
            return MpPayloadReadResult<MpCallbackPayload>.Failed(
                MpPayloadReadStatus.ContractMismatch, key!,
                "事件 " + key + " 的契约载荷为 " + contract.Accessor.PayloadType.Name +
                "，与请求的 " + payloadType.Name + " 不一致（宿主接线错误）。");
        }

        var accessor = contract.Accessor;
        var payload = accessor.CreateInstance();
        accessor.Bind(root, payload);
        var scope = accessor.ResolveScope(root);

        if (payload is MpCallbackPayload typed)
        {
            MpPayloadMaterializer.Fill(typed, key!, scope);
        }

        return MpPayloadReadResult<MpCallbackPayload>.Matched(
            payload as MpCallbackPayload ?? new GenericCallbackPayload(), key!);
    }

    private static MpPayloadReadResult<MpCallbackPayload> FallbackGeneric(PayloadNode root, string key)
    {
        var map = GenericCallbackPayload.PayloadFieldMap;
        var payload = new GenericCallbackPayload();
        map.Bind(root, payload);

        MpPayloadMaterializer.Fill(payload, key, map.ResolveScope(root));

        return MpPayloadReadResult<MpCallbackPayload>.Generic(payload, key);
    }
}
