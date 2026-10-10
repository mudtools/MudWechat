// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 获取日历下的日程列表响应体（<c>/cgi-bin/oa/schedule/get_by_calendar</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：当返回的 schedule_list 为空表示 offset 过大，应终止获取；有新增日程时可在原基础上继续增量获取；
/// 被取消的日程也会返回，调用者需检查 status 字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ListSchedulesByCalendarResponse : WechatWorkResponse
{
    /// <summary>获取或设置日程列表。</summary>
    [JsonPropertyName("schedule_list")]
    public List<ScheduleDetail>? ScheduleList { get; set; }
}
