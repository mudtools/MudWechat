// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>meeting.get</c>：按会议 ID 读取会议详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>自建口径</b>：会议详情的端点只声明在应用类型子接口上（<c>_Internal</c> / <c>_ThirdParty</c>，
/// 无代开发变体），本工具消费自建口径；代开发部署下该工具软缺席。
/// </para>
/// <para>
/// <b>不外发入会链接</b>：官方响应含 <c>meeting_link</c>（可被任何拿到链接的人用于入会），与工具面「结果进提示词
/// 即不可撤回」的前提冲突，执行器不投影；需要用户入会时由宿主在受控渠道下发。
/// </para>
/// </remarks>
[AgentTool(
    name: "meeting.get",
    Description = "按会议 ID 获取企业微信预约会议详情。参数 meetingid 为会议 ID（先用 meeting.list 获取）。返回 title（会议主题）、meeting_start（开始时间戳）、meeting_duration（时长秒）、status（0 待开始 / 1 进行中 / 2 已结束 / 3 已取消 / 4 已过期）、meeting_type（0 预约 / 1 快速 / 2 网络研讨会）、location、description、admin_userid（管理员）、meeting_code（入会号码）、cal_id、attendees（受邀成员 userid 与状态）、guest_count、has_vote。仅能读取本应用创建的会议；快速会议只返回已参与成员。不含入会链接（避免入会凭证外泄）。",
    Source = "IWechatWorkInternalMeetingService.GetMeetingInfoAsync",
    RequiredScopes = ["meeting.read"])]
public interface IWechatWorkMeetingGetTool
{
    /// <summary>读取会议详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（title、meeting_start、meeting_duration、status、meeting_type、location、description、admin_userid、meeting_code、cal_id、attendees、guest_count、has_vote）。</returns>
    Task<string> GetMeetingInfoAsync(
        [ToolParameter("meetingid", "会议 ID。", Required = true)] string meetingid,
        CancellationToken cancellationToken = default);
}
