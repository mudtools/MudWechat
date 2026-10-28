// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 添加视图请求体（<c>/cgi-bin/wedoc/smartsheet/add_view</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段名原文为 <c>property_ganttobect</c>（<c>gantt</c> + <c>oject</c>，多一个 o），属官方拼写陷阱，字段名照抄官方原文，不得「顺手修正」为 <c>property_gantt_object</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class AddSmartSheetViewRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置 Smartsheet 子表 ID（官方必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置视图标题（官方必填）。</summary>
    [JsonPropertyName("view_title")]
    public string? ViewTitle { get; set; }

    /// <summary>
    /// 获取或设置视图类型（官方必填），见官方 <c>ViewType</c>。
    /// 官方取值：<c>VIEW_TYPE_GRID</c> 表格视图、<c>VIEW_TYPE_KANBAN</c> 看板视图、<c>VIEW_TYPE_GALLERY</c> 画册视图、<c>VIEW_TYPE_GANTT</c> 甘特视图、<c>VIEW_TYPE_CALENDAR</c> 日历视图。
    /// </summary>
    [JsonPropertyName("view_type")]
    public string? ViewType { get; set; }

    /// <summary>获取或设置甘特视图属性（官方 <c>property_ganttobect</c>，非必填），添加甘特视图时必填。</summary>
    [JsonPropertyName("property_ganttobect")]
    public SmartSheetGanttViewProperty? PropertyGanttObject { get; set; }

    /// <summary>获取或设置日历视图属性（官方 <c>property_calendar</c>，非必填），添加日历视图时必填。</summary>
    [JsonPropertyName("property_calendar")]
    public SmartSheetCalendarViewProperty? PropertyCalendar { get; set; }
}
