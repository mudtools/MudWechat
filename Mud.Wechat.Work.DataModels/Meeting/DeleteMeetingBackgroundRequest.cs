// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 删除会议背景请求体（<c>/cgi-bin/meeting/layout/delete_background</c>；根据背景 ID 删除单个会议背景）。
/// </summary>
/// <remarks>
/// <para>官方限制：正在被会议应用的背景无法删除，请先设置成其他背景或恢复成会议的默认黑色背景后再行删除。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class DeleteMeetingBackgroundRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置背景 ID（官方必填）。</summary>
    [JsonPropertyName("background_id")]
    public string? BackgroundId { get; set; }
}
