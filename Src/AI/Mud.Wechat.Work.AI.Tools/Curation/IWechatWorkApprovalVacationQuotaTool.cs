// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>approval.vacation.quota</c>：查询指定成员的假期余额。
/// </summary>
/// <remarks>
/// <para>
/// <b>「我还有几天年假」必须查、不能算</b>：余额由假期管理配置（发放规则、有效期、按天/按小时刻度）决定，
/// 任何凭经验推算都会错。这也是「提请假」之前唯一需要的事实。
/// </para>
/// <para>
/// <b>官方单位是秒</b>：按天刻度需 ÷86400、按小时刻度需 ÷3600。执行器同时给出三种读数
/// （原始秒 / 折算天 / 折算小时），让模型按用户问法选，而不是让它做除法——模型算错单位是这类问题的头号错误。
/// </para>
/// <para>
/// <b>只读且不碰「修改余额」</b>：同域还有 <c>SetUserVacationQuotaAsync</c>（改余额），它属人事/薪酬面，
/// 不策展（域决策表 F 面口径）。
/// </para>
/// </remarks>
[AgentTool(
    name: "approval.vacation.quota",
    Description = "查询指定成员的假期余额（年假/调休等）。参数 userid 为成员 UserID（先用 contacts.user.resolve 解析）。返回 items（每项 id / name 假期名称 / assigned_seconds 发放 / used_seconds 已用 / left_seconds 剩余 / left_days 折算天 / left_hours 折算小时）。官方以秒为单位：按天请假看 left_days，按小时请假看 left_hours；回答用户时请直接用折算值并说明口径。仅查询，不修改余额。",
    Source = "IWechatWorkVacationService.GetUserVacationQuotaAsync",
    RequiredScopes = ["approval.read"],
    IsWrite = false)]
public interface IWechatWorkApprovalVacationQuotaTool
{
    /// <summary>查询成员假期余额。</summary>
    /// <returns>白名单投影后的 JSON 文本（userid、count、items）。</returns>
    Task<string> GetUserVacationQuotaAsync(
        [ToolParameter("userid", "成员 UserID。", Required = true)] string userid,
        CancellationToken cancellationToken = default);
}
