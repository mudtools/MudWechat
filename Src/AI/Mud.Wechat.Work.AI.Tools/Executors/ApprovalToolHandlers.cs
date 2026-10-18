// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 审批域执行器：<c>approval.list</c>（拉单号）与 <c>approval.get</c>（按单号取详情）、
/// <c>approval.template.get</c>（取模板字段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两段式读面</b>：列表端点只回 <c>sp_no_list</c>，详情端点才回结构与流转事实。合成一个工具会让
/// 「列出我这周提交的单子」被迫拉 100 份详情；拆开则模型按需二次调用，且列表本身不含任何表单内容（更安全）。
/// </para>
/// <para>
/// <b>官方限制前置</b>：单次 ≤ 100 条、<c>end_time &gt; start_time</c>、跨度 ≤ 31 天、频率 ≤ 600 次/分。
/// 前三条在执行器里收口（越界即参数类错误，模型可自我纠错）；频率交给令牌与审计观测。
/// </para>
/// <para>
/// <b>详情只投影结构与流转事实</b>：<c>apply_data.contents[].value</c>（金额、收款账户、证件号等）与
/// <c>comments[].commentcontent</c> / <c>sp_record.details[].speech</c>（意见正文）一律不外发——它们一旦进入
/// 提示词与日志即不可撤回。需要具体表单值时由宿主在自己的授权面内取，不走工具面。
/// </para>
/// </remarks>
/// <param name="approval">审批域 SDK（应用上下文由回合作用域给定）。</param>
/// <param name="template">审批模板域 SDK。</param>
public sealed class ApprovalToolHandlers(
    IWechatWorkApprovalService approval,
    IWechatWorkApprovalTemplateService template)
{
    private readonly IWechatWorkApprovalService _approval = approval ?? throw new ArgumentNullException(nameof(approval));

    private readonly IWechatWorkApprovalTemplateService _template =
        template ?? throw new ArgumentNullException(nameof(template));

    /// <summary>执行 <c>approval.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>sp_no</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkApprovalGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ApprovalGetArgs.Unpack(args);

        var response = await _approval.GetApprovalDetailAsync(
            new GetApprovalDetailRequest { SpNo = request.SpNo },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.Info is not { } info)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "审批单不存在或无权访问（info 为空）。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["sp_no"] = info.SpNo,
            ["sp_name"] = info.SpName,
            ["sp_status"] = JsonValue.Create(info.SpStatus),
            ["template_id"] = info.TemplateId,
            ["apply_time"] = JsonValue.Create(info.ApplyTime),
            ["applyer"] = info.Applyer is { } applyer
                ? new JsonObject { ["userid"] = applyer.Userid, ["partyid"] = applyer.Partyid }
                : null,
            ["batch_applyer"] = ToUserIdArray(info.BatchApplyer),
            ["records"] = ToRecordArray(info.SpRecord),
            ["notifyer"] = ToUserIdArray(info.Notifyer),
            ["comment_count"] = info.Comments?.Count ?? 0,
            ["form_fields"] = ToFieldTitleArray(info.ApplyData),
        }));
    }

    /// <summary>官方单次拉取上限（条）。</summary>
    private const int MaxListSize = 100;

    /// <summary>官方时间跨度上限（天）。</summary>
    private const int MaxSpanDays = 31;

    /// <summary>执行 <c>approval.list</c>（拉审批单号）。</summary>
    /// <param name="args">模型侧入参（<c>start_time</c> / <c>end_time</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果（sp_no_list、count、next_cursor、hint）。</returns>
    [ToolHandler(typeof(IWechatWorkApprovalListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ApprovalListArgs.Unpack(args);

        if (!long.TryParse(request.StartTime, out var start) || !long.TryParse(request.EndTime, out var end))
        {
            return ToolResult.FromError(
                "参数非法：start_time / end_time 必须是 Unix 秒的字符串形式（如 \"1792308600\"），不接受 ISO 时间。");
        }

        if (end <= start)
        {
            return ToolResult.FromError("参数非法：end_time 必须大于 start_time（官方要求，且跨度不超过 31 天）。");
        }

        if (end - start > MaxSpanDays * 86400L)
        {
            return ToolResult.FromError(
                $"参数非法：时间跨度超过官方上限 31 天（当前 {((end - start) / 86400d):0.#} 天），请缩小范围后分段拉取。");
        }

        var size = request.Size ?? MaxListSize;
        if (size is < 1 or > MaxListSize)
        {
            return ToolResult.FromError($"参数非法：size 须在 1..{MaxListSize} 之间，实际值: {size}。");
        }

        List<ApprovalInfoFilter>? filters = null;
        if (request.SpStatus is { } status)
        {
            filters = [new ApprovalInfoFilter { Key = "sp_status", Value = status.ToString(CultureInfo.InvariantCulture) }];
        }

        var response = await _approval.BatchGetApprovalNumbersAsync(
            new BatchGetApprovalNumbersRequest
            {
                Starttime = start.ToString(CultureInfo.InvariantCulture),
                Endtime = end.ToString(CultureInfo.InvariantCulture),
                NewCursor = request.Cursor,
                Size = size,
                Filters = filters,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取审批单号失败。");
        }

        var items = new JsonArray();
        foreach (var spNo in outcome.Data?.SpNoList ?? [])
        {
            items.Add((JsonNode?)JsonValue.Create(spNo));
        }

        // 空页是合法答案（该范围内没有单据 / 已翻完），不转错误：模型据此换条件或停止翻页。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["sp_no_list"] = items,
            ["count"] = items.Count,
            ["next_cursor"] = outcome.Data?.NewNextCursor,
            ["hint"] = items.Count == 0
                ? "该时间范围内没有符合条件的审批单；可放宽 sp_status 或扩大时间范围（跨度 ≤ 31 天）。"
                : null,
        }));
    }

    /// <summary>审批节点投影：只保留状态与「谁在什么时候操作」，不含审批意见正文。</summary>
    private static JsonArray ToRecordArray(List<ApprovalSpRecord>? records)
    {
        var nodes = new JsonArray();
        foreach (var node in records ?? [])
        {
            var approvers = new JsonArray();
            foreach (var detail in node.Details ?? [])
            {
                approvers.Add((JsonNode?)new JsonObject
                {
                    ["userid"] = detail.Approver?.Userid,
                    ["sp_status"] = JsonValue.Create(detail.SpStatus),
                    ["sptime"] = JsonValue.Create(detail.Sptime),
                });
            }

            nodes.Add((JsonNode?)new JsonObject
            {
                ["sp_status"] = JsonValue.Create(node.SpStatus),
                ["approverattr"] = JsonValue.Create(node.Approverattr),
                ["approvers"] = approvers,
            });
        }

        return nodes;
    }

    /// <summary>userid 引用列表投影。</summary>
    private static JsonArray ToUserIdArray(List<ApprovalUserRef>? users)
    {
        var items = new JsonArray();
        foreach (var user in users ?? [])
        {
            items.Add((JsonNode?)JsonValue.Create(user.Userid));
        }

        return items;
    }

    /// <summary>执行 <c>approval.template.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>template_id</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkApprovalTemplateGetTool))]
    public async Task<ToolResult> GetTemplateAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ApprovalTemplateGetArgs.Unpack(args);

        var response = await _template.GetTemplateDetailAsync(
            new GetApprovalTemplateDetailRequest { TemplateId = request.TemplateId },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.TemplateContent is not { } content)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "审批模板不存在或无权访问（template_content 为空）。");
        }

        var names = new JsonArray();
        foreach (var name in outcome.Data.TemplateNames ?? [])
        {
            if (!string.IsNullOrWhiteSpace(name.Text))
            {
                names.Add((JsonNode?)JsonValue.Create(name.Text));
            }
        }

        var controls = new JsonArray();
        foreach (var control in content.Controls ?? [])
        {
            controls.Add((JsonNode?)new JsonObject
            {
                ["control"] = control.Property?.Control,
                ["id"] = control.Property?.Id,
                ["title"] = FirstText(control.Property?.Title),
                ["require"] = JsonValue.Create(control.Property?.Require),
            });
        }

        // 模板是结构不是数据：只给控件类型 / ID / 标题 / 是否必填，config（选项值等）不外发。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["template_id"] = request.TemplateId,
            ["template_names"] = names,
            ["controls"] = controls,
            ["control_count"] = controls.Count,
        }));
    }

    /// <summary>多语言文本列表投影：取首个非空文本（模板名 / 控件标题的模型可读形态）。</summary>
    private static string? FirstText(List<ApprovalLangTextItem>? items)
    {
        foreach (var item in items ?? [])
        {
            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                return item.Text;
            }
        }

        return null;
    }

    /// <summary>表单字段名投影：只给控件标题（结构线索），不给控件取值。</summary>
    private static JsonArray ToFieldTitleArray(ApprovalApplyData? applyData)
    {
        var fields = new JsonArray();
        foreach (var item in applyData?.Contents ?? [])
        {
            if (item.Hidden == 1)
            {
                continue;
            }

            var title = item.Title?.FirstOrDefault()?.Text;
            if (!string.IsNullOrWhiteSpace(title))
            {
                fields.Add((JsonNode?)JsonValue.Create(title));
            }
        }

        return fields;
    }
}
