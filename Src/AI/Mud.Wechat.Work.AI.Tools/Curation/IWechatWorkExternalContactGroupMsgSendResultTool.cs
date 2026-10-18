// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.groupmsg.send_result</c>：按 msgid + userid 读取逐客户/逐群送达结果。
/// </summary>
/// <remarks>
/// <para>
/// <b>钥匙是两段</b>：官方按「群发 + 发送成员」定位送达结果，<c>userid</c> 须先经
/// <c>externalcontact.groupmsg.task_list</c> 取得（与朋友圈互动数据的钥匙同构）。
/// </para>
/// <para>
/// <b>名单级投影的边界</b>：给 <c>external_userid</c> / <c>chat_id</c>——与 <c>contact.list</c> /
/// <c>groupchat.list</c> 同级的自有数据；不含消息内容（此端点本来就不返回内容）。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.groupmsg.send_result",
    Description = "获取指定成员发送指定群发消息的逐客户/逐群送达结果。参数 msgid 为群发消息 ID（先用 externalcontact.groupmsg.list 获取）；userid 为发送该群发的成员 userid（必填，先用 externalcontact.groupmsg.task_list 取到执行成员）；limit 为单页条数（1~1000，默认 500）；cursor 为分页游标（首页省略，续页传上一页的 next_cursor）。返回 send_list（每项 userid、external_userid 客户 ID、chat_id 客户群 ID、status 0 未发送 / 1 已发送、send_time 发送时间戳）、count 与 next_cursor。external_userid 与 chat_id 只会出现一个（分别对应发给客户与发给客户群的群发）。",
    Source = "IWechatWorkExternalContactGroupMsgService.GetGroupMsgSendResultAsync",
    RequiredScopes = ["externalcontact.groupmsg.read"])]
public interface IWechatWorkExternalContactGroupMsgSendResultTool
{
    /// <summary>读取逐客户送达结果。</summary>
    /// <returns>白名单投影后的 JSON 文本（send_list、count、next_cursor）。</returns>
    Task<string> GetGroupMsgSendResultAsync(
        [ToolParameter("msgid", "群发消息 ID（先用 externalcontact.groupmsg.list 获取）。", Required = true)] string msgid,
        [ToolParameter("userid", "发送该群发的成员 userid（先用 externalcontact.groupmsg.task_list 取到执行成员）。", Required = true)] string userid,
        [ToolParameter("limit", "单页条数（1~1000，可空 ⇒ 500）。")] int? limit,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
