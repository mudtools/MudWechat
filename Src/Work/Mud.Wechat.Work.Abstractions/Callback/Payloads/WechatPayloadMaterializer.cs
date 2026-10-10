// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 载荷读面物化器：填写 <see cref="WechatCallbackPayload"/> 的元数据与 <see cref="WechatCallbackPayload.Values"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何放在 Abstractions</b>：<see cref="WechatCallbackPayload"/> 的三个属性是 <c>internal set</c>，
/// 跨程序集填写依赖本程序集既有的
/// <c>&lt;InternalsVisibleTo Include="Mud.Wechat.Work.Callback" /&gt;</c>。
/// 本类与属性同处 Abstractions，再由 Callback 的读取器调用 —— 这是<b>唯一可行</b>的放置方式
/// （<c>netstandard2.0</c> 禁 <c>init</c>，无法用 <c>init</c> 访问器表达「构造后一次写入」）。
/// </para>
/// <para>
/// <b>职责边界</b>：字段绑定由上游 <c>IPayloadContractAccessor.Bind</c> 完成，
/// 本类只补「读面元数据」（事件键 / 族 / 全量值袋）—— 对应方案 ADR-13 的分层。
/// </para>
/// <para>
/// <b>ADR-9 敏感面</b>：<see cref="SensitiveElementNames"/> 中的节点<b>不进入</b> <c>Values</c>，
/// 使载荷对象本身不可能携带凭据，可安全地日志与序列化。
/// </para>
/// </remarks>
internal static class WechatPayloadMaterializer
{
    /// <summary>
    /// 敏感凭据节点名（AGENTS.md §11：不得入日志 / 遥测 / 异常消息）。
    /// 这些节点不进入 <c>Values</c>；如需读取，宿主应从事件信封显式取用（授权族走信封，ADR-8）。
    /// </summary>
    internal static readonly string[] SensitiveElementNames =
    {
        "SuiteTicket", "AuthCode",
    };

    /// <summary>填写载荷的读面元数据。</summary>
    /// <param name="payload">已由上游映射表绑定字段的载荷实例。</param>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="family">事件族。</param>
    /// <param name="scopeNode">生效作用域节点（由上游 <c>ResolveScope</c> 解析）。</param>
    internal static void Fill(
        WechatCallbackPayload payload,
        string eventTypeKey,
        WechatCallbackEventFamily family,
        PayloadNode? scopeNode)
    {
        if (payload == null)
            return;

        payload.EventTypeKey = eventTypeKey ?? string.Empty;
        payload.Family = family;
        payload.Values = ReadValues(scopeNode);
    }

    /// <summary>
    /// 读取作用域下直系子节点的「元素名 → 全后代文本」全量视图（ADR-5）。
    /// </summary>
    /// <remarks>
    /// 规则单一可预测：值为该节点的<b>全后代文本</b>；同名重复子节点取最后一个；
    /// 敏感凭据节点被排除。
    /// </remarks>
    internal static IReadOnlyDictionary<string, string> ReadValues(PayloadNode? scopeNode)
    {
        if (scopeNode == null)
            return new Dictionary<string, string>();

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var children = scopeNode.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null)
                continue;

            if (IsSensitive(child.Name))
                continue;

            // `!`：netstandard2.0 的 string.IsNullOrEmpty 无 NotNullWhen 标注，已在上游判空。
            values[child.Name!] = child.Value ?? string.Empty;
        }

        return values;
    }

    private static bool IsSensitive(string? elementName)
    {
        if (string.IsNullOrEmpty(elementName))
            return false;

        for (var i = 0; i < SensitiveElementNames.Length; i++)
        {
            if (string.Equals(SensitiveElementNames[i], elementName, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
