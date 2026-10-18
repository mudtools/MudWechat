// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.resigned.unassigned_list</c>：读取待分配的离职成员客户清单。
/// </summary>
/// <remarks>
/// <para>
/// <b>只读，不分配</b>：官方此端点只给「谁的客户还没人接手」；重新分配是写面（且官方要求离职不超过 1 年、
/// 接替成员实名等前置条件），工具面不越界——模型据此汇报风险，宿主在审批流里做分配。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.resigned.unassigned_list",
    Description = "获取待分配的离职成员客户列表（零必填参数）。参数 page_size 为单页条数（1~1000，默认 1000）；cursor 为分页游标（首页省略，续页传上一页的 next_cursor）。返回 info（每项 handover_userid 离职成员 userid、external_userid 客户 ID、dimission_time 离职时间戳）、is_last（true 表示已到最后一页）与 next_cursor。这些客户尚未被重新分配，可据此识别「无人跟进」的风险客户；分配动作是写面（需宿主审批流），本工具只读。",
    Source = "IWechatWorkExternalContactResignedInheritanceService.GetUnassignedListAsync",
    RequiredScopes = ["externalcontact.inheritance.read"])]
public interface IWechatWorkExternalContactResignedUnassignedListTool
{
    /// <summary>读取待分配的离职成员客户清单。</summary>
    /// <returns>白名单投影后的 JSON 文本（info、is_last、next_cursor）。</returns>
    Task<string> GetUnassignedListAsync(
        [ToolParameter("page_size", "单页条数（1~1000，可空 ⇒ 1000）。")] int? page_size,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
