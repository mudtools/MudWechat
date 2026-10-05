// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 页面层级节点（官方 <c>pages</c> 元素；获取页面结构响应 <c>/cgi-bin/wedoc/smartdoc/get_page_hierarchy</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方返回的是<b>扁平数组</b>，不含嵌套树：<c>parent_id</c> 为空字符串表示根页面，
/// 非空 <c>parent_id</c> 指向父页面的 <c>page_id</c>，同一 <c>parent_id</c> 下的页面互为兄弟页面，
/// 调用方须自行按 <c>parent_id</c> 构建树形结构。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocPageNode
{
    /// <summary>获取或设置页面 ID（官方 <c>page_id</c>）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置父页面 ID（官方 <c>parent_id</c>）；为空字符串表示该页面是根页面。</summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>获取或设置页面标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
