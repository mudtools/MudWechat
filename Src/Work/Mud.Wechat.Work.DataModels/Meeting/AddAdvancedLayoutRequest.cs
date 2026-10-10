// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 添加会议高级布局请求体（<c>/cgi-bin/meeting/advanced_layout/add</c>；对当前会议添加高级布局，支持批量添加）。
/// </summary>
/// <remarks>
/// <para>官方限制：单个会议最多允许添加 20 个高级布局；用户座次设置需设置参会成员；高级布局目前仅支持 H.323/SIP 会议室终端。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class AddAdvancedLayoutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置布局对象列表（官方必填，详见 <see cref="LayoutAdvancedRequest"/>）。</summary>
    [JsonPropertyName("layout_list")]
    public List<LayoutAdvancedRequest>? LayoutList { get; set; }
}
