// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批记录请假数据（leave，只有请假模板审批记录有此数据项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalLeaveData
{
    /// <summary>
    /// 获取或设置请假时间单位：0-半天；1-小时。
    /// </summary>
    [JsonPropertyName("timeunit")]
    public int? Timeunit { get; set; }

    /// <summary>
    /// 获取或设置请假类型：1-年假；2-事假；3-病假；4-调休假；5-婚假；6-产假；7-陪产假；8-其他。
    /// </summary>
    [JsonPropertyName("leave_type")]
    public int? LeaveType { get; set; }

    /// <summary>
    /// 获取或设置请假开始时间（Unix时间）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置请假结束时间（Unix时间）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置请假时长，单位小时。
    /// </summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    /// <summary>
    /// 获取或设置请假事由。
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
