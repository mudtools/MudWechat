// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 切换 MRA 默认布局请求体（<c>/cgi-bin/meeting/mra/set_default_layout</c>；会议中对 MRA 的默认布局进行设置）。
/// </summary>
/// <remarks>
/// <para>官方限制：如果当前 MRA 已显示会议自定义布局或个性布局或焦点视频，则不支持进行默认布局设置。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class SetMraDefaultLayoutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置当前成员的默认分屏设置（官方必填）：1 - 等分模式；2 - 全屏模式；3 - 1+N。</summary>
    [JsonPropertyName("default_layout")]
    public int? DefaultLayout { get; set; }

    /// <summary>获取或设置默认非视频与会者在分屏中显示方式（官方必填）：1 - 显示；2 - 隐藏。</summary>
    [JsonPropertyName("default_novideo_user")]
    public int? DefaultNovideoUser { get; set; }

    /// <summary>获取或设置被操作 MRA 设备（官方必填，详见 <see cref="MraDeviceRef"/>）。</summary>
    [JsonPropertyName("mra")]
    public MraDeviceRef? Mra { get; set; }
}
