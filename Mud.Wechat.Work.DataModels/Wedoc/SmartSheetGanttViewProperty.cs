// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 甘特视图属性（官方 GanttViewProperty；添加视图请求的 <c>property_ganttobect</c>，添加甘特视图时必填）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetGanttViewProperty
{
    /// <summary>
    /// 获取或设置时间条起点字段 ID（官方 <c>start_date_field_id</c>，必填）。
    /// 只允许日期类型（<c>FIELD_TYPE_DATE_TIME</c>）的字段 ID。
    /// </summary>
    [JsonPropertyName("start_date_field_id")]
    public string? StartDateFieldId { get; set; }

    /// <summary>
    /// 获取或设置时间条终点字段 ID（官方 <c>end_date_field_id</c>，必填）。
    /// 只允许日期类型（<c>FIELD_TYPE_DATE_TIME</c>）的字段 ID。
    /// </summary>
    [JsonPropertyName("end_date_field_id")]
    public string? EndDateFieldId { get; set; }
}
