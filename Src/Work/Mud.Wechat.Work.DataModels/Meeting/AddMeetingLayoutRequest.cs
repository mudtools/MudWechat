// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 添加会议基础布局请求体（<c>/cgi-bin/meeting/layout/add</c>；对 API 成功预定的会议添加会议基础布局，支持多个布局的添加，
/// 每个布局支持多页模板，默认选中第一页模板作为该布局的首页进行展示）。
/// </summary>
/// <remarks>
/// <para>官方限制：一场会议最多添加 10 个布局；用户座次设置区分会前和会中两种方式——会前只允许设置邀请者成员，会中只允许设置参会成员。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class AddMeetingLayoutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置布局对象列表（官方必填，详见 <see cref="LayoutBasicRequest"/>）。</summary>
    [JsonPropertyName("layout_list")]
    public List<LayoutBasicRequest>? LayoutList { get; set; }

    /// <summary>
    /// 获取或设置布局列表中会议需要应用的布局序号（从 1 开始计数）。
    /// <para>首次添加时若该参数不送，则默认选中第一个布局作为会议应用的布局。</para>
    /// </summary>
    [JsonPropertyName("default_layout_order")]
    public int? DefaultLayoutOrder { get; set; }
}
