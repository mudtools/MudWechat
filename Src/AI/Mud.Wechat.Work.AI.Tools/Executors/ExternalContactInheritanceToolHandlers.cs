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
using Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 客户继承域只读执行器：待分配的离职成员客户清单。
/// </summary>
/// <remarks>
/// <para>
/// <b>只读到底</b>：官方这一域有三个写端点（分配离职客户 / 分配离职群 / 在职继承），
/// 全部不外发为工具——它们是高影响、不可撤回的动作，且官方有离职年限、实名、可见范围等前置条件，
/// 应由宿主在审批流里做；工具面只回答「谁的客户还没人接手」。
/// </para>
/// <para>
/// <b>接替状态本波不策展</b>：<c>GetResignedTransferResultAsync</c> / <c>GetTransferResultAsync</c> 是只读端点，
/// 但方法名含引擎危险词表里的 <c>transfer</c> ⇒ 被判 <c>high-risk-write</c>，而引擎暂无「显式风险覆盖」槽
/// （与 <c>approval.list</c> 因 <c>batch</c> 被误判同理，见 README 陷阱 10）。标只读即构建失败，
/// 标写面又会让每次查询都要求人工审批，故两工具**未落地**，待上游支持覆盖槽后再补。
/// </para>
/// </remarks>
/// <param name="resigned">离职继承域 SDK。</param>
public sealed class ExternalContactInheritanceToolHandlers(IWechatWorkExternalContactResignedInheritanceService resigned)
{
    /// <summary>待分配清单的官方单页上限。</summary>
    private const int MaxUnassignedPageSize = 1000;

    /// <summary>待分配清单的官方默认单页条数。</summary>
    private const int DefaultUnassignedPageSize = 1000;

    private readonly IWechatWorkExternalContactResignedInheritanceService _resigned =
        resigned ?? throw new ArgumentNullException(nameof(resigned));

    /// <summary>执行 <c>externalcontact.resigned.unassigned_list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactResignedUnassignedListTool))]
    public async Task<ToolResult> ListUnassignedAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactResignedUnassignedListArgs.Unpack(args);

        var pageSize = request.PageSize ?? DefaultUnassignedPageSize;
        if (pageSize is < 1 or > MaxUnassignedPageSize)
        {
            return ToolResult.FromError($"参数非法：page_size 必须在 1~{MaxUnassignedPageSize} 之间。");
        }

        var response = await _resigned.GetUnassignedListAsync(
            new GetUnassignedListRequest { PageSize = pageSize, Cursor = request.Cursor },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactResignedUnassignedList);
        }

        var info = new JsonArray();
        foreach (var item in outcome.Data?.Info ?? [])
        {
            info.Add((JsonNode?)new JsonObject
            {
                ["handover_userid"] = item.HandoverUserid,
                ["external_userid"] = item.ExternalUserid,
                ["dimission_time"] = JsonValue.Create(item.DimissionTime),
            });
        }

        // 空列表是合法答案（当前没有待分配的离职客户），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["info"] = info,
            ["count"] = info.Count,
            ["is_last"] = JsonValue.Create(outcome.Data?.IsLast),
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }
}
