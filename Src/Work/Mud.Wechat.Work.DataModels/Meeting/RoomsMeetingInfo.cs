// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室下的会议信息对象（获取 Rooms 会议室下的会议列表响应 <c>meeting_info_list</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：<see cref="Status"/> 为字符串枚举（MEETING_STATE_*，官方契约如此，非整数），
/// 且本端点的枚举集合与获取网络研讨会详情页略有差异（本页含 MEETING_STATE_NULL、不含 MEETING_STATE_ABOUT_TO_START）；
/// <see cref="MeetingType"/> 本页标注「4：rooms 投屏会议」（网络研讨会详情页同义值标注为 3），照抄本页原文。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsMeetingInfo
{
    /// <summary>获取或设置会议 ID。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置有效会议 Code。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置会议主题。</summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    /// <summary>
    /// 获取或设置会议状态（官方为字符串枚举）。
    /// <para>取值：MEETING_STATE_INVALID - 非法或未知状态；MEETING_STATE_INIT - 待开始；MEETING_STATE_CANCELLED - 已取消；
    /// MEETING_STATE_STARTED - 进行中；MEETING_STATE_ENDED - 已删除；MEETING_STATE_NULL - 无状态（过了预定结束时间，会中无人）；
    /// MEETING_STATE_RECYCLED - 已回收（过了预定开始时间 30 天，会议号被后台回收，无法再进入）。</para>
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置会议类型：0 - 一次性会议；1 - 周期性会议；2 - 微信专属会议；4 - rooms 投屏会议；5 - 个人会议号会议；6 - 网络研讨会（Webinar）。</summary>
    [JsonPropertyName("meeting_type")]
    public int? MeetingType { get; set; }

    /// <summary>获取或设置会议预订开始时间（Unix 秒）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置会议预订结束时间（Unix 秒）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
