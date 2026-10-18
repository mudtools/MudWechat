// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>meeting.list</c>：拉取某成员一段时间内的会议 ID 列表。
/// </summary>
/// <remarks>
/// <para>
/// <b>会议域的入口</b>：官方没有「按主题/模糊条件找会议」的读端点，只有「按成员 + 时间范围列 ID」；
/// 拿到 ID 后再用 <c>meeting.get</c>（详情）与 <c>meeting.attendees</c>（已参会成员）补齐。
/// </para>
/// <para>
/// <b>只能看到本应用创建的会议</b>：官方限制「仅返回该应用创建的会议」，这是权限模型而非分页问题，
/// 模型据此不得声称「企业全部会议只有这些」。
/// </para>
/// </remarks>
[AgentTool(
    name: "meeting.list",
    Description = "拉取指定成员指定时间段内的企业微信会议 ID 列表。参数 userid 为成员 UserID（先用 contacts.user.simple_list 或 contacts.user.list 获取）；begin_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601，如 \"2026-10-10T09:00:00+08:00\"），跨度不得超过 180 天且 end_time 必须大于 start_time；cursor 为上一页的 next_cursor；limit 为单页条数（1~100，默认 100）。返回 meetingid_list 与 next_cursor（为空表示已取完）。注意：只能拉取本应用创建的会议，且仅返回应用可见范围内的成员数据。",
    Source = "IWechatWorkMeetingService.GetUserMeetingIdListAsync",
    RequiredScopes = ["meeting.read"])]
public interface IWechatWorkMeetingListTool
{
    /// <summary>拉取成员会议 ID 列表。</summary>
    /// <param name="userid">成员 UserID。</param>
    /// <param name="begin_time">起始时间（Unix 秒或带时区的 ISO 8601）。</param>
    /// <param name="end_time">结束时间（Unix 秒或带时区的 ISO 8601；须大于 begin_time 且跨度 ≤ 180 天）。</param>
    /// <param name="cursor">分页游标（可空 ⇒ 首页）。</param>
    /// <param name="limit">单页条数（1~100，可空 ⇒ 官方默认 100）。</param>
    void List(
        [ToolParameter("userid", "成员 UserID。", Required = true)] string userid,
        [ToolParameter("begin_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string begin_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；须大于 begin_time 且跨度 ≤ 180 天）。", Required = true)] string end_time,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        [ToolParameter("limit", "单页条数（1~100，可空 ⇒ 官方默认 100）。")] int? limit);
}
