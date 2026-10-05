// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 静音成员请求体（<c>/cgi-bin/meeting/realcontrol/mute_user</c>；会议中成员静音操作）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：参数表将 <c>option</c> 标注为 string、<c>operated_user</c> 标注为 object[]，
/// 官方请求示例分别为布尔值与单个对象，本模型均以示例为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MuteMeetingUserRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置静音操作（官方必填）：true - 静音；false - 解除静音。</summary>
    [JsonPropertyName("option")]
    public bool? Option { get; set; }

    /// <summary>获取或设置被操作成员（官方必填，详见 <see cref="MeetingOperatedUser"/>）。</summary>
    [JsonPropertyName("operated_user")]
    public MeetingOperatedUser? OperatedUser { get; set; }
}
