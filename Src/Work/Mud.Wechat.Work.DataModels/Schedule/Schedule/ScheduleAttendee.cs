// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 日程参与者（创建/更新日程与新增/删除日程参与者的 attendees 元素，及获取日程详情/日历下日程列表响应 attendees 元素；参与者累计最多 1000 人）。
/// </summary>
/// <remarks>
/// <para>请求侧仅承载 <see cref="Userid"/>（官方必填）；<see cref="ResponseStatus"/> 与 <see cref="EventTime"/> 仅出现在响应侧。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleAttendee
{
    /// <summary>获取或设置日程参与者 ID（官方必填；不多于 64 字节）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置参与者接受状态（仅响应侧）：0-未处理；1-待定；2-全部接受；3-仅接受一次；4-拒绝；
    /// 获取日程详情响应另可能出现 5-接受本次及未来；6-待定单次；7-待定本次及未来；8-拒绝单次；9-拒绝本次及未来。
    /// </summary>
    [JsonPropertyName("response_status")]
    public int? ResponseStatus { get; set; }

    /// <summary>获取或设置对该时间点日程做出决策的时间（仅响应侧；Unix 时间戳，配合 response_status 使用）。</summary>
    [JsonPropertyName("event_time")]
    public long? EventTime { get; set; }
}
