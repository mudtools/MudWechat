// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.groupmsg.list</c>：分页拉取企业的群发记录（含文本与附件类型）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两段式读面的第一段</b>：官方此端点给 <c>msgid</c> 与内容；「谁发了、发给谁、成没成」必须再按
/// <c>msgid</c> 调 <c>externalcontact.groupmsg.task_list</c> / <c>.send_result</c>（官方无批量执行详情）。
/// </para>
/// <para>
/// <b>附件只给类型不给资产</b>：<c>media_id</c> / <c>pic_url</c> 属宿主素材资产，模型无法消费二进制，
/// 与朋友圈域同一条边界——只投影 <c>msgtype</c> 列表。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.groupmsg.list",
    Description = "分页获取企业微信客户群发记录。参数 chat_type 必填（single 发给客户 / group 发给客户群）；start_time / end_time 必填（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601），跨度不得超过 1 个月且 end_time 必须大于 start_time；creator 可选（限定创建人 userid，不填为全部）；filter_type 可选（0 企业创建 / 1 个人创建 / 2 全部，默认 2）；limit 为单页条数（1~100，默认 50）；cursor 为分页游标（首页省略，续页传上一页的 page_token）。返回 items（每项 msgid、creator、create_time、create_type 0 企业 / 1 个人、text 文本内容、attachment_types 附件类型列表）与 count、page_token；拿到 msgid 后用 externalcontact.groupmsg.task_list 看成员执行情况、externalcontact.groupmsg.send_result 看逐客户送达结果。",
    Source = "IWechatWorkExternalContactGroupMsgService.GetGroupMsgListAsync",
    RequiredScopes = ["externalcontact.groupmsg.read"])]
public interface IWechatWorkExternalContactGroupMsgListTool
{
    /// <summary>拉取群发记录。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、count、page_token）。</returns>
    Task<string> GetGroupMsgListAsync(
        [ToolParameter("chat_type", "群发任务类型（single 发给客户 / group 发给客户群）。", Required = true)] string chat_type,
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 1 个月）。", Required = true)] string end_time,
        [ToolParameter("creator", "创建人 userid（可空 ⇒ 全部）。")] string? creator,
        [ToolParameter("filter_type", "创建人类型过滤（0 企业创建 / 1 个人创建 / 2 全部；可空 ⇒ 2）。")] int? filter_type,
        [ToolParameter("limit", "单页条数（1~100，可空 ⇒ 50）。")] int? limit,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
