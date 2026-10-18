// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json;
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
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ApprovalGet);
        }

        if (outcome.Data?.Info is not { } info)
        {
            return WechatToolResults.NotFound(WechatToolNames.ApprovalGet);
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
            throw WechatToolArgumentErrors.Invalid(
                "参数非法：start_time / end_time 必须是 Unix 秒的字符串形式（如 \"1792308600\"），不接受 ISO 时间。");
        }

        if (end <= start)
        {
            throw WechatToolArgumentErrors.Invalid("参数非法：end_time 必须大于 start_time（官方要求，且跨度不超过 31 天）。");
        }

        if (end - start > MaxSpanDays * 86400L)
        {
            throw WechatToolArgumentErrors.Invalid(
                $"参数非法：时间跨度超过官方上限 31 天（当前 {((end - start) / 86400d):0.#} 天），请缩小范围后分段拉取。");
        }

        var size = request.Size ?? MaxListSize;
        if (size is < 1 or > MaxListSize)
        {
            throw WechatToolArgumentErrors.Invalid($"参数非法：size 须在 1..{MaxListSize} 之间，实际值: {size}。");
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
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ApprovalList);
        }

        // 空页是合法答案（该范围内没有单据 / 已翻完），不转错误：模型据此换条件或停止翻页。
        var envelope = ToolResultJsons.PageEnvelope(
            hasMore: !string.IsNullOrEmpty(outcome.Data?.NewNextCursor),
            pageToken: outcome.Data?.NewNextCursor);
        var items = envelope["items"]!.AsArray();
        foreach (var spNo in outcome.Data?.SpNoList ?? [])
        {
            items.Add((JsonNode?)JsonValue.Create(spNo));
        }

        envelope["count"] = items.Count;
        if (items.Count == 0)
        {
            envelope["hint"] = "该时间范围内没有符合条件的审批单；可放宽 sp_status 或扩大时间范围（跨度 ≤ 31 天）。";
        }

        return ToolResultPipeline.OkJson(envelope, 0);
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
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ApprovalTemplateGet);
        }

        if (outcome.Data?.TemplateContent is not { } content)
        {
            return WechatToolResults.NotFound(WechatToolNames.ApprovalTemplateGet);
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

    /// <summary>执行 <c>approval.apply</c>（按模板控件 ID 填值提交）。</summary>
    /// <param name="args">模型侧入参（<c>template_id</c> / <c>creator_userid</c> / <c>fields_json</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果（ok、sp_no、filled_controls）。</returns>
    /// <remarks>
    /// <b>提交前按模板回校</b>：未知控件 ID、必填缺失、控件类型不支持，全部在调用官方接口之前拒绝。
    /// 静默丢弃一个字段（例如附件）比提交失败更糟——用户会以为附件已经带上了。
    /// </remarks>
    [ToolHandler(typeof(IWechatWorkApprovalApplyTool))]
    public async Task<ToolResult> ApplyAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ApprovalApplyArgs.Unpack(args);

        JsonObject? fields;
        try
        {
            fields = JsonNode.Parse(request.FieldsJson) as JsonObject;
        }
        catch (JsonException ex)
        {
            throw WechatToolArgumentErrors.Invalid(
                $"参数非法：fields_json 不是合法 JSON（{ex.Message}）。应为对象，键是控件 ID、值是取值。");
        }

        if (fields is null || fields.Count == 0)
        {
            throw WechatToolArgumentErrors.Invalid(
                "参数非法：fields_json 必须是非空 JSON 对象（键 = 控件 ID，值 = 取值）；空表单不能提交。");
        }

        // 模板是控件 ID / 类型 / 必填的权威来源：先取模板再校验，而不是相信模型自报的控件结构。
        var templateResponse = await _template.GetTemplateDetailAsync(
            new GetApprovalTemplateDetailRequest { TemplateId = request.TemplateId },
            cancellationToken).ConfigureAwait(false);
        var templateOutcome = WechatApiResultReader.Read(templateResponse);
        if (!templateOutcome.Ok)
        {
            return WechatToolOutcomeErrors.From(templateOutcome, WechatToolNames.ApprovalApply);
        }

        if (templateOutcome.Data?.TemplateContent is not { } content)
        {
            return WechatToolResults.NotFound(WechatToolNames.ApprovalApply);
        }

        var controls = new Dictionary<string, ApprovalTemplateControlProperty>(StringComparer.Ordinal);
        foreach (var control in content.Controls ?? [])
        {
            if (control.Property is { } property && !string.IsNullOrWhiteSpace(property.Id))
            {
                controls[property.Id] = property;
            }
        }

        if (controls.Count == 0)
        {
            throw WechatToolArgumentErrors.Invalid("该模板没有可填控件（可能是系统模板或权限不足），无法自动提交。");
        }

        var contents = new List<ApprovalControlItem>(fields.Count);
        foreach (var (id, node) in fields)
        {
            if (!controls.TryGetValue(id, out var property))
            {
                throw WechatToolArgumentErrors.Invalid(
                    $"参数非法：控件 ID \"{id}\" 不在模板中。可用控件：{string.Join(", ", controls.Keys)}（先调 approval.template.get 核对）。");
            }

            var (value, error) = MapControlValue(property.Control, node);
            if (error is not null)
            {
                throw WechatToolArgumentErrors.Invalid($"参数非法：控件 {id}（{property.Control}）{error}");
            }

            contents.Add(new ApprovalControlItem { Id = id, Value = value });
        }

        var missing = controls
            .Where(pair => pair.Value.Require == 1 && !fields.ContainsKey(pair.Key))
            .Select(pair => $"{pair.Key}({FirstText(pair.Value.Title)})")
            .ToArray();
        if (missing.Length > 0)
        {
            throw WechatToolArgumentErrors.Invalid(
                $"参数非法：必填控件缺失 {string.Join("、", missing)}。请补齐后重新提交（不要让用户承担漏填的后果）。");
        }

        List<ApprovalSummaryLine>? summary = null;
        if (!string.IsNullOrWhiteSpace(request.Summary))
        {
            summary = [.. request.Summary
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(text => new ApprovalSummaryLine
                {
                    SummaryInfo = [new ApprovalLangTextItem { Text = text }],
                })];
        }

        var response = await _approval.ApplyApprovalAsync(
            new ApplyApprovalRequest
            {
                CreatorUserid = request.CreatorUserid,
                TemplateId = request.TemplateId,
                UseTemplateApprover = request.UseTemplateApprover is false ? 0 : 1,
                ApplyData = new ApprovalApplyData { Contents = contents },
                SummaryList = summary,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ApprovalApply);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["sp_no"] = outcome.Data?.SpNo,
            ["filled_controls"] = contents.Count,
        }));
    }

    /// <summary>
    /// 控件取值映射：按<b>模板声明的控件类型</b>（而不是模型自报的类型）决定值的形状。
    /// </summary>
    /// <remarks>
    /// 只覆盖能安全映射的 7 种控件；其余（附件、明细表、假期、位置、关联审批…）明确拒绝并让用户去审批应用填写——
    /// 静默丢字段比提交失败更危险。日期/选择器同时接受「简写」（字符串/数组）与「完整对象」两种形态，
    /// 因为模型对「日期是一个时间戳」和「单选是一个 key」的心智更自然。
    /// </remarks>
    private static (ApprovalControlValue? Value, string? Error) MapControlValue(string? control, JsonNode? node)
    {
        switch (control)
        {
            case "Text":
            case "Textarea":
                return TryGetScalar(node, out var text)
                    ? (new ApprovalControlValue { Text = text }, null)
                    : (null, "需要字符串值。");

            case "Number":
                return TryGetScalar(node, out var number)
                    ? (new ApprovalControlValue { NewNumber = number }, null)
                    : (null, "需要数字（以字符串形式给出）。");

            case "Money":
                return TryGetScalar(node, out var money)
                    ? (new ApprovalControlValue { NewMoney = money }, null)
                    : (null, "需要金额（以字符串形式给出）。");

            case "Date":
                if (node is JsonObject dateObject)
                {
                    var stamp = dateObject["timestamp"]?.ToString();
                    if (string.IsNullOrWhiteSpace(stamp))
                    {
                        return (null, "需要 timestamp（Unix 秒）。");
                    }

                    return (new ApprovalControlValue
                    {
                        Date = new ApprovalDateValue
                        {
                            Type = dateObject["type"]?.ToString() ?? "day",
                            Timestamp = stamp,
                        },
                    }, null);
                }

                return TryGetScalar(node, out var timestamp)
                    ? (new ApprovalControlValue { Date = new ApprovalDateValue { Type = "day", Timestamp = timestamp } }, null)
                    : (null, "需要 Unix 秒字符串，或 {\"type\":\"day\",\"timestamp\":\"...\"}。");

            case "Selector":
                if (node is JsonArray multi)
                {
                    var options = new List<ApprovalSelectorOption>(multi.Count);
                    foreach (var item in multi)
                    {
                        if (!TryGetScalar(item, out var key))
                        {
                            return (null, "多选需要字符串 key 数组。");
                        }

                        options.Add(new ApprovalSelectorOption { Key = key });
                    }

                    return (new ApprovalControlValue
                    {
                        Selector = new ApprovalSelectorValue { Type = "multi", Options = options },
                    }, null);
                }

                return TryGetScalar(node, out var singleKey)
                    ? (new ApprovalControlValue
                    {
                        Selector = new ApprovalSelectorValue
                        {
                            Type = "single",
                            Options = [new ApprovalSelectorOption { Key = singleKey }],
                        },
                    }, null)
                    : (null, "需要选项 key（单选）或 key 数组（多选）；key 由模板 config 给出。");

            case "Members":
                if (node is not JsonArray members)
                {
                    return (null, "需要 userid 数组。");
                }

                var userids = new List<ApprovalMemberItem>(members.Count);
                foreach (var item in members)
                {
                    if (!TryGetScalar(item, out var userid))
                    {
                        return (null, "需要 userid 字符串数组。");
                    }

                    userids.Add(new ApprovalMemberItem { Userid = userid });
                }

                return (new ApprovalControlValue { Members = userids }, null);

            default:
                return (null,
                    "属于暂不支持自动提交的控件类型（附件 / 明细表 / 假期 / 位置 / 关联审批等）；"
                    + "请在审批应用中填写该字段，或改用模板中只含文本/数字/金额/日期/选择/成员的消息提交。");
        }
    }

    /// <summary>取标量值的字符串形态（字符串 / 数字 / 布尔都接受；对象与数组不接受）。</summary>
    private static bool TryGetScalar(JsonNode? node, out string value)
    {
        value = string.Empty;
        if (node is not JsonValue scalar)
        {
            return false;
        }

        if (scalar.TryGetValue<string>(out var text))
        {
            value = text;
            return true;
        }

        if (scalar.TryGetValue<decimal>(out var number))
        {
            value = number.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        return false;
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
