// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 关闭或开启成员视频请求体（<c>/cgi-bin/meeting/realcontrol/switch_user_video</c>；支持关闭或开启 MRA 设备的视频）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class SwitchMeetingUserVideoRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置操作类型：false - 关闭视频（默认值）；true - 开启视频（仅支持 MRA 设备）。
    /// </summary>
    [JsonPropertyName("video")]
    public bool? Video { get; set; }

    /// <summary>获取或设置被操作者（官方必填，详见 <see cref="MeetingOperatedUser"/>）。</summary>
    [JsonPropertyName("operated_user")]
    public MeetingOperatedUser? OperatedUser { get; set; }
}
