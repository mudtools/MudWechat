// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 视图（官方 View；添加视图响应 <c>view</c>、更新视图响应 <c>view</c>、查询视图响应 <c>views</c> 元素共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetView
{
    /// <summary>获取或设置视图 ID（官方 <c>view_id</c>）。</summary>
    [JsonPropertyName("view_id")]
    public string? ViewId { get; set; }

    /// <summary>获取或设置视图标题（官方 <c>view_title</c>）。</summary>
    [JsonPropertyName("view_title")]
    public string? ViewTitle { get; set; }

    /// <summary>
    /// 获取或设置视图类型（官方 <c>view_type</c>），见官方 <c>ViewType</c>。
    /// 官方取值：<c>VIEW_TYPE_GRID</c> 表格视图、<c>VIEW_TYPE_KANBAN</c> 看板视图、<c>VIEW_TYPE_GALLERY</c> 画册视图、<c>VIEW_TYPE_GANTT</c> 甘特视图、<c>VIEW_TYPE_CALENDAR</c> 日历视图；官方另列出未知类型 <c>VEW_UNKNOWN</c>（原文如此，缺 <c>I</c>），且官方明示传递该值不合法。
    /// </summary>
    [JsonPropertyName("view_type")]
    public string? ViewType { get; set; }

    /// <summary>获取或设置视图属性（官方 <c>property</c>），更新视图响应与查询视图响应返回。</summary>
    [JsonPropertyName("property")]
    public SmartSheetViewProperty? Property { get; set; }
}
