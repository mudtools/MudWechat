// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.moment.publish_list</c>：查看企业发表的朋友圈在各执行成员处的发表状态。
/// </summary>
/// <remarks>
/// <para>
/// <b>两段式读面的第二段</b>：<c>moment.list</c> 给出朋友圈（<c>create_type = 0</c> 为企业发表），本工具给出
/// 「谁该发、发了没发」。它是 <c>moment.comments</c> 的前置——企业发表的朋友圈必须带上执行成员 userid 才能查互动。
/// </para>
/// <para>
/// <b>仅限企业发表</b>：官方只接受企业发表的朋友圈 id，个人朋友圈会直接报错；工具在描述里点明，避免无效调用。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.moment.publish_list",
    Description = "获取企业发表的朋友圈在各成员处的执行列表。参数 moment_id 为企业发表的朋友圈 ID（先用 externalcontact.moment.list 取 create_type=0 的记录）；cursor 为分页游标（首页省略，续页传上一页的 next_cursor）；limit 为单页条数（1~1000，可空 ⇒ 500）。返回 task_list（每项 userid 与 publish_status 0 未发表 / 1 已发表）、count 与 next_cursor。仅支持企业发表的朋友圈（个人朋友圈会报错）；某成员的 userid 可作为 externalcontact.moment.comments 的入参查其互动。",
    Source = "IWechatWorkExternalContactMomentService.GetMomentPublishListAsync",
    RequiredScopes = ["externalcontact.moment.read"])]
public interface IWechatWorkExternalContactMomentPublishListTool
{
    /// <summary>列出企业发表朋友圈的执行成员与发表状态。</summary>
    /// <param name="moment_id">企业发表的朋友圈 ID（先用 externalcontact.moment.list 获取）。</param>
    /// <param name="cursor">分页游标（可空 ⇒ 首页）。</param>
    /// <param name="limit">单页条数（1~1000，可空 ⇒ 500）。</param>
    void List([ToolParameter("moment_id", "企业发表的朋友圈 ID（先用 externalcontact.moment.list 获取）。", Required = true)] string moment_id, [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor, [ToolParameter("limit", "单页条数（1~1000，可空 ⇒ 500）。")] int? limit);
}
