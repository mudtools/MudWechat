// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 请假控件值（Vacation，请假模板特有；请假类型强关联审批应用中的假期管理）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalVacationValue
{
    /// <summary>
    /// 获取或设置请假类型选择（所选选项为假期管理中的假期类型，固定为单选）。
    /// </summary>
    [JsonPropertyName("selector")]
    public ApprovalSelectorValue? Selector { get; set; }

    /// <summary>
    /// 获取或设置假勤组件（时间范围与假勤类型）。
    /// </summary>
    [JsonPropertyName("attendance")]
    public ApprovalAttendanceValue? Attendance { get; set; }
}
