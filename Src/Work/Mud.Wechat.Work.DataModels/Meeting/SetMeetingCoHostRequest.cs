// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 管理联席主持人请求体（<c>/cgi-bin/meeting/realcontrol/set_cohost</c>；设置或撤销会议中的参会者联席主持人身份）。
/// </summary>
/// <remarks>
/// <para>官方限制：目前暂不支持 MRA 设备作为被操作者的情况。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class SetMeetingCoHostRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置具体设置动作（官方必填）：true - 设置联席主持人；false - 撤销联席主持人。</summary>
    [JsonPropertyName("action")]
    public bool? Action { get; set; }

    /// <summary>获取或设置被操作成员（官方必填，详见 <see cref="MeetingOperatedUser"/>）。</summary>
    [JsonPropertyName("operated_user")]
    public MeetingOperatedUser? OperatedUser { get; set; }
}
