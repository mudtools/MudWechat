// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Schedule;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 日程域执行器：<c>schedule.add</c>（建日程 + 邀请）、<c>schedule.get</c>（按 ID 取详情）、
/// <c>schedule.cancel</c>（取消并通知参与者）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只做「可自举」的三件事</b>：创建（自建应用省略 <c>cal_id</c> 即写应用默认日历）、按 ID 取详情、按 ID 取消。
/// <b>不做</b> <c>list_by_calendar</c>——它只能读「应用自己创建的日历」，而 <c>cal_id</c> 在工具面无来源，
/// 做成工具只会得到「需要用户报日历 ID」。<b>不做</b> <c>update</c>——覆盖式更新让「改一个字段」变成「重写整个日程」。
/// </para>
/// <para>
/// <b>官方约束按字符计</b>：标题 128 / 描述 1000 / 地点 128 都是<b>字符</b>（与消息正文的字节口径不同），
/// 按错口径校验会造成「本地通过、服务端截断」。
/// </para>
/// </remarks>
/// <param name="schedule">日程域 SDK（应用上下文由回合作用域给定）。</param>
public sealed class ScheduleToolHandlers(IWechatWorkScheduleService schedule)
{
    /// <summary>标题字符上限。</summary>
    private const int MaxSummaryChars = 128;

    /// <summary>描述字符上限。</summary>
    private const int MaxDescriptionChars = 1000;

    /// <summary>地点字符上限。</summary>
    private const int MaxLocationChars = 128;

    /// <summary>参与者人数上限。</summary>
    private const int MaxAttendees = 1000;

    /// <summary>单个参与者 ID 的字节上限。</summary>
    private const int MaxAttendeeIdBytes = 64;

    private readonly IWechatWorkScheduleService _schedule =
        schedule ?? throw new ArgumentNullException(nameof(schedule));

    /// <summary>执行 <c>schedule.add</c>。</summary>
    /// <param name="args">模型侧入参（<c>summary</c> / <c>start_time</c> / <c>end_time</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkScheduleAddTool))]
    public async Task<ToolResult> AddAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ScheduleAddArgs.Unpack(args);

        if (request.Summary.Length > MaxSummaryChars)
        {
            return ToolResult.FromError($"参数非法：summary 超过官方上限（{request.Summary.Length} > {MaxSummaryChars} 字符）。");
        }

        if (!long.TryParse(request.StartTime, out var startUnix) || !long.TryParse(request.EndTime, out var endUnix))
        {
            return ToolResult.FromError(
                "参数非法：start_time / end_time 必须是 Unix 秒的字符串形式（如 \"1792308600\"），不接受 ISO 时间。");
        }

        if (endUnix <= startUnix)
        {
            return ToolResult.FromError("参数非法：end_time 必须大于 start_time（Unix 秒）。");
        }

        if (request.Description is { Length: > MaxDescriptionChars })
        {
            return ToolResult.FromError(
                $"参数非法：description 超过官方上限（{request.Description.Length} > {MaxDescriptionChars} 字符）。");
        }

        if (request.Location is { Length: > MaxLocationChars })
        {
            return ToolResult.FromError($"参数非法：location 超过官方上限（{request.Location.Length} > {MaxLocationChars} 字符）。");
        }

        var attendees = SplitUsers(request.Attendees);
        if (attendees.Count > MaxAttendees)
        {
            return ToolResult.FromError($"参数非法：参与者超过官方上限（{attendees.Count} > {MaxAttendees} 人）。");
        }

        var tooLong = attendees.FirstOrDefault(user => Encoding.UTF8.GetByteCount(user) > MaxAttendeeIdBytes);
        if (tooLong is not null)
        {
            return ToolResult.FromError($"参数非法：参与者 ID 超过官方上限（{MaxAttendeeIdBytes} 字节）：{tooLong}");
        }

        // remind_before_secs 的合法档位由闭集（WechatScheduleRemindSecs）在参数层拦下，此处不再重复校验。
        var response = await _schedule.AddScheduleAsync(
            new AddScheduleRequest
            {
                Schedule = new ScheduleInfo
                {
                    Summary = request.Summary,
                    Description = request.Description,
                    Location = request.Location,
                    StartTime = startUnix,
                    EndTime = endUnix,
                    IsWholeDay = request.IsWholeDay is { } wholeDay ? (wholeDay ? 1 : 0) : null,
                    CalId = string.IsNullOrWhiteSpace(request.CalId) ? null : request.CalId,
                    Attendees = attendees.Select(user => new ScheduleAttendee { Userid = user }).ToList(),
                    Reminders = request.RemindBeforeSecs is { } secs && secs > 0
                        ? new ScheduleReminders { IsRemind = 1, RemindBeforeEventSecs = secs }
                        : null,
                },
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ScheduleAdd);
        }

        if (string.IsNullOrWhiteSpace(outcome.Data?.ScheduleId))
        {
            return WechatToolResults.NotFound(WechatToolNames.ScheduleAdd);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["schedule_id"] = outcome.Data!.ScheduleId,
        }));
    }

    /// <summary>执行 <c>schedule.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>schedule_id</c> 必填，多个用 | 分隔）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkScheduleGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ScheduleGetArgs.Unpack(args);
        var ids = SplitUsers(request.ScheduleId);
        if (ids.Count > MaxAttendees)
        {
            return ToolResult.FromError($"参数非法：schedule_id 一次最多 {MaxAttendees} 个（官方约束）。");
        }

        var response = await _schedule.GetScheduleAsync(
            new GetScheduleRequest { ScheduleIdList = ids },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ScheduleGet);
        }

        var items = new JsonArray();
        foreach (var detail in outcome.Data?.ScheduleList ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["schedule_id"] = detail.ScheduleId,
                ["summary"] = detail.Summary,
                ["description"] = detail.Description,
                ["location"] = detail.Location,
                ["status"] = JsonValue.Create(detail.Status),
                ["start_time"] = JsonValue.Create(detail.StartTime),
                ["end_time"] = JsonValue.Create(detail.EndTime),
                ["is_whole_day"] = JsonValue.Create(detail.IsWholeDay),
                ["cal_id"] = detail.CalId,
                ["admins"] = ToStringArray(detail.Admins),
                ["attendees"] = ToAttendeeArray(detail.Attendees),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["count"] = items.Count,
            ["schedules"] = items,
        }));
    }

    /// <summary>执行 <c>schedule.cancel</c>。</summary>
    /// <param name="args">模型侧入参（<c>schedule_id</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkScheduleCancelTool))]
    public async Task<ToolResult> CancelAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ScheduleCancelArgs.Unpack(args);

        if (request.OpMode is { } mode && mode is not (WechatScheduleOpModes.ThisSchedule or WechatScheduleOpModes.ThisAndFuture))
        {
            return ToolResult.FromError(
                $"参数非法：op_mode 只能是 ThisSchedule({WechatScheduleOpModes.ThisSchedule}) / ThisAndFuture({WechatScheduleOpModes.ThisAndFuture})，省略表示取消全部周期。");
        }

        // 官方：op_mode 为 1/2 时必须给 op_start_time（匹配某一次周期的开始时间）。
        long? opStartUnix = null;
        if (request.OpMode is not null)
        {
            if (string.IsNullOrWhiteSpace(request.OpStartTime)
                || !long.TryParse(request.OpStartTime, out var parsed))
            {
                return ToolResult.FromError(
                    "参数非法：op_mode 为 ThisSchedule / ThisAndFuture 时必须同时给 op_start_time（该次周期的开始时间，Unix 秒字符串）。");
            }

            opStartUnix = parsed;
        }

        var response = await _schedule.DelScheduleAsync(
            new DelScheduleRequest
            {
                ScheduleId = request.ScheduleId,
                OpMode = request.OpMode,
                OpStartTime = opStartUnix,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ScheduleCancel);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["schedule_id"] = request.ScheduleId,
        }));
    }

    private static List<string> SplitUsers(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? []
            : [.. value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

    private static JsonArray ToStringArray(List<string>? values)
    {
        var items = new JsonArray();
        foreach (var value in values ?? [])
        {
            items.Add((JsonNode?)JsonValue.Create(value));
        }

        return items;
    }

    private static JsonArray ToAttendeeArray(List<ScheduleAttendee>? attendees)
    {
        var items = new JsonArray();
        foreach (var attendee in attendees ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["userid"] = attendee.Userid,
                ["response_status"] = JsonValue.Create(attendee.ResponseStatus),
            });
        }

        return items;
    }
}
