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
    /// <remarks>
    /// 投影含「根层同名兄弟合并」步骤（<see cref="CoalesceRepeatedSiblings"/>）：
    /// 官方存在无包装容器的重复元素列表形态——既有叶列表（如 wedoc 回调根下的
    /// <c>&lt;DocId&gt;A&lt;/DocId&gt;&lt;DocId&gt;B&lt;/DocId&gt;</c>），也有对象列表
    /// （如会议「素材上传结果」根下的 <c>&lt;UploadInfo&gt;…&lt;/UploadInfo&gt;&lt;UploadInfo&gt;…&lt;/UploadInfo&gt;</c>）——
    /// 须在投影期归拢才能被列表转换器读取。
    /// </remarks>
    internal static PayloadNode Project(XElement element)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        var root = ProjectCore(element);

        // 仅根层合并（见 CoalesceRepeatedSiblings 的保守边界）：根下的「容器 + 项」
        // 包装形态（GroupIds/GroupId、SelectedItems/SelectedItem、ExtAttr/Item 等）由 Items/
        // ItemsObject 的 ItemName 通道逐项绑定，若在容器内部合并会令 ItemName 命中合成容器
        // 而非原始项 —— 故合并严格限定在根元素这一层。
        return new PayloadNode(root.Name, root.Value, CoalesceRepeatedSiblings(root.Children), root.Attributes);
    }

    private static PayloadNode ProjectCore(XElement element)
    {
        var children = new List<PayloadNode>();
        foreach (var child in element.Elements())
        {
            children.Add(ProjectCore(child));
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
    /// 根层同名兄弟合并：≥2 个同名兄弟节点（<b>全叶组</b>或<b>全复杂组</b>）合成为一个<b>同名容器</b>节点。
    /// </summary>
    /// <param name="children">根节点的全部直接子节点（投影序）。</param>
    /// <returns>合并后的子节点列表（无合并需求时原样返回）。</returns>
    /// <remarks>
    /// <para>
    /// <b>为何需要</b>：官方存在两类「根下重复同名兄弟元素」的无包装容器列表形态——
    /// 叶列表（wedoc 回调 <c>&lt;DocId&gt;A&lt;/DocId&gt;&lt;DocId&gt;B&lt;/DocId&gt;</c>）与
    /// 对象列表（会议「素材上传结果」的 <c>UploadInfo</c> 元素，参数表明文「上传的素材对象列表」），
    /// 而上游 <see cref="PayloadNode.Child(string)"/> 对同名子节点只取第一个（重复即视为畸形输入）——
    /// 不归拢则第 2..N 个元素在映射期不可达（静默丢字段）。合并后容器即「该名」节点：
    /// 首取语义（<c>Child</c>/<c>Has</c>/<c>ResolveScope</c>）行为不变，
    /// 容器 <see cref="PayloadNode.Children"/> 携带原始元素全集供列表转换器读取
    /// （叶列表走 <c>WechatPayloadConverter.RepeatSiblings</c>、对象列表走各业务的专用方法）。
    /// </para>
    /// <para>
    /// <b>保守边界一（仅根层）</b>：根之下的「容器 + 项」包装形态
    /// （<c>GroupIds/GroupId</c>、<c>SelectedItems/SelectedItem</c>、<c>ExtAttr/Item</c> 等）
    /// 由 <c>Items</c>/<c>ItemsObject</c> 的 ItemName 通道逐项绑定 —— 若在容器内部合并会让
    /// ItemName 命中合成容器而非原始项（首项之外全丢），故合并严格限定在根元素这一层；
    /// 根层的单一容器节点（出现恰 1 次）不受影响。
    /// </para>
    /// <para>
    /// <b>保守边界二（只合并「全部命中成员同质」的组）</b>：全叶组（标量列表形态）或
    /// 全复杂组（对象列表形态）才合并；混合形态（同名既有叶又有复杂节点）不合并，
    /// 保持「首取」旧行为。
    /// </para>
    /// <para>
    /// <b>合成容器的 <see cref="PayloadNode.Value"/> 取末位成员文本</b>：与 <c>WechatPayloadMaterializer.ReadValues</c>
    /// 的「<c>Values</c> 全量袋同名重复子节点取最后一个」既有语义（ADR-5）逐字一致 ——
    /// 合并对全量袋视图零影响；属性集取空（重复叶兄弟无属性可合并的官方形态，且首个成员的属性
    /// 无法代表全集）。
    /// </para>
    /// </remarks>
    private static IReadOnlyList<PayloadNode> CoalesceRepeatedSiblings(IReadOnlyList<PayloadNode> children)
    {
        if (children.Count < 2)
        {
            return children;
        }

        // 单次计数：名字 → (出现次数, 是否全部为叶, 是否全部为复杂节点)。
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        var allLeaves = new Dictionary<string, bool>(StringComparer.Ordinal);
        var allComplex = new Dictionary<string, bool>(StringComparer.Ordinal);
        for (var i = 0; i < children.Count; i++)
        {
            var name = children[i].Name;
            var isLeaf = children[i].Children.Count == 0;
            counts[name] = counts.TryGetValue(name, out var count) ? count + 1 : 1;
            allLeaves[name] = allLeaves.TryGetValue(name, out var leaves) ? leaves && isLeaf : isLeaf;
            allComplex[name] = allComplex.TryGetValue(name, out var complex) ? complex && !isLeaf : !isLeaf;
        }

        List<PayloadNode>? coalesced = null;
        var consumed = new HashSet<string>(StringComparer.Ordinal);
        var prefixCopied = false;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (consumed.Contains(child.Name))
            {
                continue;
            }

            var qualify = counts[child.Name] >= 2 &&
                          (allLeaves[child.Name] || allComplex[child.Name]);
            if (!qualify)
            {
                coalesced?.Add(child);
                continue;
            }

            // 首次命中：收集该名全部成员（保序），在其首个出现位置放入合成容器；
            // 此前尚无任何成员被消费，前缀原样回填（仅一次）。
            var group = new List<PayloadNode>(counts[child.Name]);
            for (var j = i; j < children.Count; j++)
            {
                if (string.Equals(children[j].Name, child.Name, StringComparison.Ordinal))
                {
                    group.Add(children[j]);
                }
            }

            coalesced ??= new List<PayloadNode>(children.Count);
            if (!prefixCopied)
            {
                for (var j = 0; j < i; j++)
                {
                    coalesced.Add(children[j]);
                }

                prefixCopied = true;
            }

            consumed.Add(child.Name);
            coalesced.Add(new PayloadNode(child.Name, group[group.Count - 1].Value, group));
        }

        return coalesced ?? children;
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
