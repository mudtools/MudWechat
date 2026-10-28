// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 更新日程请求体（<c>/cgi-bin/oa/schedule/update</c>）。
/// </summary>
/// <remarks>
/// <para>更新操作是<b>覆盖式</b>而不是增量式；增量更新参与人请用新增/删除日程参与者端点。
/// 重复日程的三种操作模式详见官方「更新重复日程」说明页（自建 96204 / 第三方 96198 / 代开发 96826）：
/// 非周期日程指定 op_mode 为 1 或 2 会报错 90485；「仅修改此日程」模式下不能将重复性参数指定为重复（报错 90484）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class UpdateScheduleRequest
{
    /// <summary>获取或设置是否不更新参与人：0-否；1-是（默认 0）。</summary>
    [JsonPropertyName("skip_attendees")]
    public int? SkipAttendees { get; set; }

    /// <summary>获取或设置操作模式（重复日程时有效）：0-默认全部修改；1-仅修改此日程；2-修改将来的所有日程。</summary>
    [JsonPropertyName("op_mode")]
    public int? OpMode { get; set; }

    /// <summary>
    /// 获取或设置操作起始时间（仅 op_mode 为 1 或 2 时有效；必须是重复日程某一次的开始时间，Unix 时间戳）。
    /// </summary>
    /// <remarks>
    /// <para>匹配不到某次周期的开始时间会报错 90482；特例：op_mode 为 2 且等于原日程 start_time 时演变为修改全部，不分裂新日程。</para>
    /// </remarks>
    [JsonPropertyName("op_start_time")]
    public long? OpStartTime { get; set; }

    /// <summary>获取或设置日程信息（官方必填；schedule_id 必填）。</summary>
    [JsonPropertyName("schedule")]
    public ScheduleInfo? Schedule { get; set; }
}
