// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 假期域执行器：<c>approval.vacation.quota</c> 落到 <see cref="IWechatWorkVacationService.GetUserVacationQuotaAsync"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么单独一个执行器</b>：若把它塞进 <c>ApprovalToolHandlers</c>，执行器的软缺席会耦合到
/// <c>IWechatWorkVacationService</c>——宿主没注册假期服务时，连审批详情工具都会一起消失。
/// 一个执行器只依赖它真正需要的服务，软缺席的粒度才对。
/// </para>
/// <para>
/// <b>三种读数一起给</b>：官方单位恒为秒；按天刻度 ÷86400、按小时刻度 ÷3600。让模型做除法是这类问题的头号错误源，
/// 故执行器直接给出 <c>left_days</c> / <c>left_hours</c>，模型只需按用户问法选一个。
/// </para>
/// </remarks>
/// <param name="vacation">假期域 SDK（应用上下文由回合作用域给定）。</param>
public sealed class VacationToolHandlers(IWechatWorkVacationService vacation)
{
    /// <summary>一天对应的秒数。</summary>
    private const double SecondsPerDay = 86400d;

    /// <summary>一小时对应的秒数。</summary>
    private const double SecondsPerHour = 3600d;

    private readonly IWechatWorkVacationService _vacation =
        vacation ?? throw new ArgumentNullException(nameof(vacation));

    /// <summary>执行 <c>approval.vacation.quota</c>。</summary>
    /// <param name="args">模型侧入参（<c>userid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkApprovalVacationQuotaTool))]
    public async Task<ToolResult> GetQuotaAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ApprovalVacationQuotaArgs.Unpack(args);

        var response = await _vacation.GetUserVacationQuotaAsync(
            new GetVacationUserQuotaRequest { Userid = request.Userid },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ApprovalVacationQuota);
        }

        var items = new JsonArray();
        foreach (var item in outcome.Data?.Lists ?? [])
        {
            var left = item.Leftduration ?? 0;
            items.Add((JsonNode?)new JsonObject
            {
                ["id"] = JsonValue.Create(item.Id),
                ["name"] = item.Vacationname,
                ["assigned_seconds"] = JsonValue.Create(item.Assignduration),
                ["used_seconds"] = JsonValue.Create(item.Usedduration),
                ["left_seconds"] = JsonValue.Create(item.Leftduration),
                ["left_days"] = Math.Round(left / SecondsPerDay, 2),
                ["left_hours"] = Math.Round(left / SecondsPerHour, 2),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["userid"] = request.Userid,
            ["count"] = items.Count,
            ["items"] = items,
            ["hint"] = items.Count == 0
                ? "该成员没有可见的假期余额记录（可能未纳入假期管理，或不在应用可见范围内）。"
                : "官方单位为秒：按天请假看 left_days，按小时请假看 left_hours；回答用户时请说明口径，不要自行换算。",
        }));
    }
}
