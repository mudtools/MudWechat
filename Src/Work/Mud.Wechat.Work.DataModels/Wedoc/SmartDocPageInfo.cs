// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 页面信息（官方 PageInfo；添加页面请求与响应、更新页面请求与响应共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocPageInfo
{
    /// <summary>获取或设置页面 ID（官方 <c>page_id</c>，由系统生成）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置页面标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置父页面 ID（官方 <c>parent_id</c>），用于创建或移动子页面；为空表示第一层页面。
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 获取或设置插入位置（官方 <c>after_id</c>），表示新页面排在哪个页面之后；
    /// 添加页面时为空表示插入到最前，更新页面时为空表示移动到最后。
    /// </summary>
    [JsonPropertyName("after_id")]
    public string? AfterId { get; set; }

    /// <summary>
    /// 获取或设置页面布局模式（官方 <c>layout_mode</c>），见官方 <c>PageLayoutMode</c>。
    /// 官方取值：<c>1</c> 默认布局、<c>2</c> 纸张布局、<c>3</c> 全宽布局。
    /// </summary>
    [JsonPropertyName("layout_mode")]
    public uint? LayoutMode { get; set; }
}
