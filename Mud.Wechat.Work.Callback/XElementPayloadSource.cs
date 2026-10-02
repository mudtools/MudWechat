// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// <c>XElement</c> → <see cref="PayloadNode"/> 投影器：本包对 XML 的<b>唯一触点</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>架构边界（守卫 CB14/CB18）</b>：上游 <c>Mud.HttpUtils</c> 的载荷契约是 XML-free 的，
/// 故「报文 → 节点」的适配必须由<b>消费方</b>提供；本类是全包唯一允许出现
/// <c>System.Xml.Linq</c> 类型的文件（另有 <see cref="WechatCallbackReceiver"/> 仅用于提取请求体的
/// <c>Encrypt</c> 节点，不参与载荷路径）。
/// </para>
/// <para>
/// <b>投影语义与上游一致</b>：<see cref="PayloadNode.Value"/> 取元素的<b>全后代文本</b>
/// （即 <c>XElement.Value</c> 语义），属性进入 <c>Attributes</c>，直接子元素进入 <c>Children</c>。
/// 该规则与载荷的 <c>Values</c> 全量袋规则同源，保证「作用域视图」与「全量袋视图」不打架。
/// </para>
/// </remarks>
internal static class XElementPayloadSource
{
    /// <summary>
    /// 解析解密明文为根元素（本包对 XML 类别的<b>唯一使用点</b>之一）。
    /// </summary>
    /// <param name="decryptedXml">解密后的明文。</param>
    /// <returns>根元素；明文缺失或非法时为 <see langword="null"/>。</returns>
    /// <remarks>
    /// 明文非 XML（协议外报文）在此被吞掉并返回 <c>null</c> —— 与旧解析器「不抛」的语义一致
    /// （行为保持矩阵 B1/B10），由读取器映射为 <c>MalformedPayload</c> 状态。
    /// </remarks>
    internal static XElement? TryParseRoot(string? decryptedXml)
    {
        if (string.IsNullOrEmpty(decryptedXml))
            return null;

        try
        {
            return XDocument.Parse(decryptedXml!).Root;
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    /// <summary>把 XML 元素树投影为上游节点树（一次性、不可变）。</summary>
    /// <param name="element">根元素。</param>
    /// <returns>节点树根节点。</returns>
    internal static PayloadNode Project(XElement element)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        var children = new List<PayloadNode>();
        foreach (var child in element.Elements())
        {
            children.Add(Project(child));
        }

        var attributes = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var attribute in element.Attributes())
        {
            // 命名空间声明不是业务属性，排除（避免 xmlns 进入 Attributes 干扰 NameAttribute 取值）。
            if (attribute.IsNamespaceDeclaration)
                continue;

            attributes[attribute.Name.LocalName] = attribute.Value;
        }

        return new PayloadNode(element.Name.LocalName, element.Value, children, attributes);
    }

    /// <summary>
    /// 源容器：<c>XElement</c>（信封视图）+ 惰性投影的 <see cref="PayloadNode"/>（载荷视图）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 定义为 <see cref="XElementPayloadSource"/> 的<b>嵌套类型</b>：本类是全包唯一允许出现 XML 类型的文件，
    /// 把「持有 <c>XElement</c>」的容器也放在此处，可使「XML 触点唯一」的守卫（CB14）
    /// 只需放行<b>一个</b>文件 —— 否则缓存文件也会因持有该类型而被判违规。
    /// </para>
    /// <para>
    /// 必须为非空引用类型：<c>ConditionalWeakTable&lt;TKey, TValue&gt;</c> 要求 <c>TValue : class</c>，
    /// 且避免依赖工厂回调返回 <c>null</c> 时的行为差异 —— 「解析失败」以 <see cref="Root"/> 为 <c>null</c>
    /// 表达，而非容器本身为 <c>null</c>。
    /// </para>
    /// </remarks>
    internal sealed class SourceHolder
    {
        private PayloadNode? _projected;

        internal SourceHolder(XElement? root)
        {
            Root = root;
        }

        /// <summary>解密明文的根节点；明文缺失或非法时为 <c>null</c>。</summary>
        internal XElement? Root { get; }

        /// <summary>节点投影（首次访问时物化并缓存；<see cref="Root"/> 为 <c>null</c> 时恒为 <c>null</c>）。</summary>
        internal PayloadNode? Projected
        {
            get
            {
                var cached = _projected;
                if (cached != null || Root == null)
                    return cached;

                var built = Project(Root);
                _projected = built;
                return built;
            }
        }
    }
}
