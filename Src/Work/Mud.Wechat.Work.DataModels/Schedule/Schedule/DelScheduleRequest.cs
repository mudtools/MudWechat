// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 取消日程请求体（<c>/cgi-bin/oa/schedule/del</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class DelScheduleRequest
{
    /// <summary>获取或设置日程 ID（官方必填）。</summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    /// <summary>获取或设置操作模式（重复日程时有效）：0-默认删除所有日程；1-仅删除此日程；2-删除本次及后续日程（详见官方「重复日程的不同操作模式」）。</summary>
    [JsonPropertyName("op_mode")]
    public int? OpMode { get; set; }

    /// <summary>获取或设置操作起始时间（仅操作模式为 1 或 2 时有效；该时间必须是重复日程的某一次开始时间，Unix 时间戳）。</summary>
    [JsonPropertyName("op_start_time")]
    public long? OpStartTime { get; set; }
}
