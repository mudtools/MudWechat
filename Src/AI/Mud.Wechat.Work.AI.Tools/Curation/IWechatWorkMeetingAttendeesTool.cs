// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>meeting.attendees</c>：读取会议的已参会成员列表。
/// </summary>
/// <remarks>
/// <para>
/// <b>自建口径</b>：参会人端点只声明在 <c>IWechatWorkMeetingAdvancedService_Internal</c> 上，代开发/第三方部署下软缺席。
/// </para>
/// <para>
/// <b>官方约束较细</b>：时间区间不得超过 31 天（都不传时默认前推 31 天，最大跨度 90 天）；<c>limit</c> 每页 ≤100
/// 且<b>续页必须与首次取 cursor 时相同</b>——执行器把这条写进描述，避免模型翻页时改页大小导致游标失效。
/// </para>
/// </remarks>
[AgentTool(
    name: "meeting.attendees",
    Description = "获取企业微信会议（含网络研讨会）的已参会成员列表。参数 meetingid 为会议 ID（先用 meeting.list 获取）；start_time / end_time 可选（Unix 秒或带时区的 ISO 8601，两者都传时跨度不得超过 31 天，都不传时默认前推 31 天）；cursor 为上一页 next_cursor；limit 为单页条数（1~100，默认 100，且续页必须与首次一致）。返回 attendees（userid、role 角色、join_time、quit_time、join_type 入会方式）、count、has_more 与 next_cursor。会议未开始时返回空列表。仅返回本应用创建的会议、且在应用可见范围内的成员；不含临时 openid、网络质量、音视频状态与客户数据。",
    Source = "IWechatWorkInternalMeetingAdvancedService.GetAttendeeListAsync",
    RequiredScopes = ["meeting.read"])]
public interface IWechatWorkMeetingAttendeesTool
{
    /// <summary>读取已参会成员列表。</summary>
    /// <param name="meetingid">会议 ID。</param>
    /// <param name="start_time">起始时间（可空；Unix 秒或带时区的 ISO 8601）。</param>
    /// <param name="end_time">结束时间（可空；与 start_time 同时出现时跨度 ≤ 31 天）。</param>
    /// <param name="cursor">分页游标（可空 ⇒ 首页）。</param>
    /// <param name="limit">单页条数（1~100，可空 ⇒ 100；续页须与首次一致）。</param>
    void List(
        [ToolParameter("meetingid", "会议 ID。", Required = true)] string meetingid,
        [ToolParameter("start_time", "起始时间（可空；Unix 秒或带时区的 ISO 8601）。")] string? start_time,
        [ToolParameter("end_time", "结束时间（可空；与 start_time 同时出现时跨度 ≤ 31 天）。")] string? end_time,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        [ToolParameter("limit", "单页条数（1~100，可空 ⇒ 100；续页须与首次一致）。")] int? limit);
}
