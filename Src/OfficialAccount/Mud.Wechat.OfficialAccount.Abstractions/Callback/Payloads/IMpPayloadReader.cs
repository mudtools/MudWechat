// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 回调事件载荷读取端口：把事件信封解析为<b>状态化</b>的强类型载荷。
/// </summary>
/// <remarks>
/// <para>
/// <b>单次解析</b>：读取器与接收器共享叶层 <c>WechatPayloadSourceCache</c>（弱键缓存，
/// 键 = 信封对象），每个事件的 <c>XDocument.Parse</c> 与节点投影各发生<b>恰好一次</b>。
/// </para>
/// <para>
/// <b>不抛</b>：键不匹配 / 明文非 XML / 未登记契约均以 <see cref="MpPayloadReadStatus"/> 表达。
/// </para>
/// </remarks>
public interface IMpPayloadReader
{
    /// <summary>按事件键解析指定类型的载荷。</summary>
    /// <typeparam name="TPayload">目标载荷类型（须已按事件键登记契约；<see cref="GenericCallbackPayload"/> 免登记）。</typeparam>
    /// <param name="envelope">回调事件信封（含解密明文）。</param>
    /// <returns>状态化读取结果。</returns>
    MpPayloadReadResult<TPayload> Read<TPayload>(MpCallbackEnvelope envelope)
        where TPayload : MpCallbackPayload;

    /// <summary>按事件键解析载荷（动态路径：由分发器按处理器声明的载荷类型调用）。</summary>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="payloadType">目标载荷类型。</param>
    /// <returns>状态化读取结果；未登记契约时降级为 <see cref="GenericCallbackPayload"/>。</returns>
    MpPayloadReadResult<MpCallbackPayload> Read(MpCallbackEnvelope envelope, Type payloadType);
}

/// <summary>
/// 载荷读面物化器：填写 <see cref="MpCallbackPayload"/> 的元数据与 <c>Values</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何放在 Abstractions</b>：<see cref="MpCallbackPayload"/> 的属性是 <c>internal set</c>，
/// 跨程序集填写依赖本程序集的 <c>InternalsVisibleTo</c>（与企微侧同构）；本类与属性同处 Abstractions，
/// 再由 Callback 的读取器调用 —— <c>netstandard2.0</c> 禁 <c>init</c>，这是「构造后一次写入」的唯一可行形态。
/// </para>
/// <para><b>职责边界</b>：字段绑定由上游 <c>IPayloadContractAccessor.Bind</c> 完成，本类只补读面元数据。</para>
/// </remarks>
internal static class MpPayloadMaterializer
{
    /// <summary>填写载荷的读面元数据。</summary>
    /// <param name="payload">已由上游映射表绑定字段的载荷实例。</param>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="scopeNode">生效作用域节点（由上游 <c>ResolveScope</c> 解析）。</param>
    internal static void Fill(MpCallbackPayload payload, string eventTypeKey, PayloadNode? scopeNode)
    {
        if (payload == null)
        {
            return;
        }

        payload.EventTypeKey = eventTypeKey ?? string.Empty;
        payload.Values = ReadValues(scopeNode);
    }

    /// <summary>读取作用域下直系子节点的「元素名 → 全后代文本」全量视图（同名重复取最后）。</summary>
    /// <param name="scopeNode">作用域节点。</param>
    /// <returns>全量值袋（无作用域时为空字典）。</returns>
    internal static IReadOnlyDictionary<string, string> ReadValues(PayloadNode? scopeNode)
    {
        if (scopeNode == null)
        {
            return new Dictionary<string, string>();
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var children = scopeNode.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || string.IsNullOrEmpty(child.Name))
            {
                continue;
            }

            values[child.Name!] = child.Value ?? string.Empty;
        }

        return values;
    }
}
