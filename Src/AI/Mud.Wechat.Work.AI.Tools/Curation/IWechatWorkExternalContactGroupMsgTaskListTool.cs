// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.groupmsg.task_list</c>：按 msgid 读取群发在各成员处的执行情况。
/// </summary>
/// <remarks>
/// <para>
/// <b>「成员确认后才发」的观测面</b>：官方群发不会自动送达，需成员在企业微信 2.7.5 以上终端确认；
/// 本工具给出每个成员的状态（未发/已发）与发送时间，是判断「群发卡在谁那」的唯一入口。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.groupmsg.task_list",
    Description = "获取指定群发消息在各成员处的发送任务与状态。参数 msgid 为群发消息 ID（先用 externalcontact.groupmsg.list 获取）；limit 为单页条数（1~1000，默认 500）；cursor 为分页游标（首页省略，续页传上一页的 page_token）。返回 items（每项 userid、status 0 未发送 / 1 已发送、send_time 发送时间戳）、count 与 page_token。群发需成员在企业微信客户端确认后才真正送达，本工具即用于查看该确认/发送进度；逐客户送达结果请改用 externalcontact.groupmsg.send_result。",
    Source = "IWechatWorkExternalContactGroupMsgService.GetGroupMsgTaskListAsync",
    RequiredScopes = ["externalcontact.groupmsg.read"])]
public interface IWechatWorkExternalContactGroupMsgTaskListTool
{
    /// <summary>读取群发成员执行任务。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、count、page_token）。</returns>
    Task<string> GetGroupMsgTaskListAsync(
        [ToolParameter("msgid", "群发消息 ID（先用 externalcontact.groupmsg.list 获取）。", Required = true)] string msgid,
        [ToolParameter("limit", "单页条数（1~1000，可空 ⇒ 500）。")] int? limit,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
