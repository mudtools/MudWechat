// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 修改会议基础布局请求体（<c>/cgi-bin/meeting/layout/update</c>；根据布局 ID 对设置好的会议基础布局进行修改）。
/// </summary>
/// <remarks>
/// <para>官方限制：用户座次设置区分会前和会中两种方式——会前只允许设置邀请者成员，会中只允许设置参会成员。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class UpdateMeetingLayoutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置布局 ID（官方必填）。</summary>
    [JsonPropertyName("layout_id")]
    public string? LayoutId { get; set; }

    /// <summary>获取或设置布局单页对象列表（官方必填，详见 <see cref="LayoutBasicPage"/>）。</summary>
    [JsonPropertyName("page_list")]
    public List<LayoutBasicPage>? PageList { get; set; }

    /// <summary>获取或设置是否设置为会议应用的布局（默认不设置）。</summary>
    [JsonPropertyName("enable_set_default")]
    public bool? EnableSetDefault { get; set; }
}
