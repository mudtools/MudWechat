// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 导入会议报名信息请求体（<c>/cgi-bin/meeting/enroll/import</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：会议未开启报名时返回未开启报名错误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ImportMeetingEnrollsRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置报名成员列表（官方必填，详见 <see cref="MeetingEnrollImportItem"/>）。</summary>
    [JsonPropertyName("enroll_list")]
    public List<MeetingEnrollImportItem>? EnrollList { get; set; }
}
