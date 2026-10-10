// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 关闭成员屏幕共享请求体（<c>/cgi-bin/meeting/realcontrol/close_screen_share</c>；停止成员发起的屏幕共享）。
/// </summary>
/// <remarks>
/// <para>官方限制：目前暂不支持 MRA 设备作为被操作者的情况。</para>
/// <para>
/// 官方文档陷阱：参数表将 <c>operated_user</c> 标注为 object[]，官方请求示例为单个对象
/// （字段命名单数口径一致），本模型以单个对象承载。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CloseMeetingScreenShareRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置被操作成员（官方必填，详见 <see cref="MeetingOperatedUser"/>）。</summary>
    [JsonPropertyName("operated_user")]
    public MeetingOperatedUser? OperatedUser { get; set; }
}
