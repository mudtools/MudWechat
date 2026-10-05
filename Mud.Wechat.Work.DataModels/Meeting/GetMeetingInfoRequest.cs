// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议详情请求体（<c>/cgi-bin/meeting/get_info</c>；自建应用与第三方应用请求形态一致）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="MeetingCode"/> 与 <see cref="SubMeetingid"/> 仅预约会议高级管理文档页声明；
/// 高级管理文档页口径：<c>meetingid</c> 与 <c>meeting_code</c> 必须填一个（基础文档页口径为仅传 <c>meetingid</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingInfoRequest
{
    /// <summary>获取或设置会议 ID（与 <see cref="MeetingCode"/> 至少填一个）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置入会码（与 <see cref="Meetingid"/> 至少填一个；仅预约会议高级管理文档页声明该参数）。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置周期性会议子会议 ID（仅预约会议高级管理文档页声明该参数）。</summary>
    [JsonPropertyName("sub_meetingid")]
    public string? SubMeetingid { get; set; }
}
