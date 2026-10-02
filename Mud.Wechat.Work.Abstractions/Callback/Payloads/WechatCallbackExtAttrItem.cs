// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 成员扩展属性项（<c>create_user</c>/<c>update_user</c> 报文的 <c>ExtAttr/Item</c> 节点）。
/// </summary>
/// <remarks>
/// 由上游 <c>PayloadFieldMapGenerator</c> 的 <c>ItemsWithAttributes</c> 形态填充：
/// 容器 <c>ExtAttr</c> → 项 <c>Item</c>，名称取 <c>Name</c> 与 <c>Type</c> 属性，值取 <c>Text</c> 子元素。
/// 需无参构造（上游 <c>ItemsWithAttributes&lt;TItem&gt; where TItem : class, new()</c>）。
/// </remarks>
public sealed class WechatCallbackExtAttrItem
{
    /// <summary>扩展属性名称（<c>Item</c> 节点的 <c>Name</c> 属性）。</summary>
    public string? Name { get; set; }

    /// <summary>扩展属性类型（<c>Item</c> 节点的 <c>Type</c> 属性：0 = 文本，1 = 网页）。</summary>
    public string? Type { get; set; }

    /// <summary>扩展属性值（<c>Text</c> 子节点；网页类型为 url）。</summary>
    public string? Value { get; set; }
}
