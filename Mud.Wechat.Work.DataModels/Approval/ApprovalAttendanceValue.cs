// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 假勤控件值（Attendance，出差/外出/加班组件；请假组件 Vacation 的时间部分亦复用此结构）。
/// </summary>
/// <remarks>
/// <para>官方限制：加班的时间跨度不能超过七天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalAttendanceValue
{
    /// <summary>
    /// 获取或设置假勤组件时间选择范围。
    /// </summary>
    [JsonPropertyName("date_range")]
    public ApprovalDateRangeValue? DateRange { get; set; }

    /// <summary>
    /// 获取或设置假勤组件类型：1-请假；2-补卡；3-出差；4-外出；5-加班。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置时长分片信息（非必填；2020/10/01之前的历史表单不支持时长分片）。
    /// </summary>
    [JsonPropertyName("slice_info")]
    public ApprovalAttendanceSliceInfo? SliceInfo { get; set; }
}
