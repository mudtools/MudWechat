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
using Mud.Wechat.Work.DataModels.Meeting;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 会议域只读执行器：成员会议 ID 列表 → 会议详情 → 已参会成员。
/// </summary>
/// <remarks>
/// <para>
/// <b>三跳自举</b>：<c>meeting.list</c>（成员 + 时间范围）→ <c>meeting.get</c>（主题/时间/状态）→
/// <c>meeting.attendees</c>（谁参加了）。官方无「按主题搜会议」的能力，工具面也不虚构。
/// </para>
/// <para>
/// <b>投影边界</b>：会议详情里的 <c>meeting_link</c>（入会凭证）与参会人里的 <c>tmp_openid</c>（外部临时身份）、
/// 网络质量/音视频状态/客户数据一律不投影——它们要么是可被滥用的凭证，要么是外部自然人的行为指纹。
/// </para>
/// </remarks>
/// <param name="meeting">会议域公共 SDK（三类应用公共面）。</param>
/// <param name="meetingInternal">会议详情（自建口径）。</param>
/// <param name="advanced">参会人列表（自建口径）。</param>
public sealed class MeetingToolHandlers(
    IWechatWorkMeetingService meeting,
    IWechatWorkInternalMeetingService meetingInternal,
    IWechatWorkInternalMeetingAdvancedService advanced)
{
    /// <summary>官方限制：会议 ID 列表的时间跨度上限（180 天）。</summary>
    private const long MaxListSpanSeconds = 180L * 24 * 60 * 60;

    /// <summary>官方限制：参会人列表的时间跨度上限（31 天）。</summary>
    private const long MaxAttendeeSpanSeconds = 31L * 24 * 60 * 60;

    /// <summary>官方单页上限。</summary>
    private const int MaxPageSize = 100;

    private const string TimeFormatHint =
        "时间格式非法：时间参数应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"）。";

    private readonly IWechatWorkMeetingService _meeting = meeting ?? throw new ArgumentNullException(nameof(meeting));

    private readonly IWechatWorkInternalMeetingService _meetingInternal =
        meetingInternal ?? throw new ArgumentNullException(nameof(meetingInternal));

    private readonly IWechatWorkInternalMeetingAdvancedService _advanced =
        advanced ?? throw new ArgumentNullException(nameof(advanced));

    /// <summary>执行 <c>meeting.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkMeetingListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = MeetingListArgs.Unpack(args);

        if (!ToolTimeText.TryParseUnixSeconds(request.BeginTime, out var begin))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (!ToolTimeText.TryParseUnixSeconds(request.EndTime, out var end))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (end <= begin)
        {
            return ToolResult.FromError("参数非法：end_time 必须大于 begin_time。");
        }

        if (end - begin > MaxListSpanSeconds)
        {
            return ToolResult.FromError("参数非法：时间跨度不得超过 180 天，请缩小范围后分批拉取。");
        }

        var limit = request.Limit ?? MaxPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var response = await _meeting.GetUserMeetingIdListAsync(
            new GetUserMeetingIdListRequest
            {
                Userid = request.Userid,
                Cursor = request.Cursor,
                Limit = limit,
                BeginTime = begin,
                EndTime = end,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取会议列表失败。");
        }

        var ids = new JsonArray();
        foreach (var meetingid in outcome.Data?.MeetingidList ?? [])
        {
            ids.Add((JsonNode?)JsonValue.Create(meetingid));
        }

        // 空列表是合法答案（该时间段内该成员没有会议或会议不属于本应用），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["meetingid_list"] = ids,
            ["count"] = ids.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>meeting.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>meetingid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkMeetingGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = MeetingGetArgs.Unpack(args);

        var response = await _meetingInternal.GetMeetingInfoAsync(
            new GetMeetingInfoRequest { Meetingid = request.Meetingid },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data is not { } info)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "会议不存在或无权访问。");
        }

        var members = new JsonArray();
        foreach (var member in info.Attendees?.Member ?? [])
        {
            members.Add((JsonNode?)new JsonObject
            {
                ["userid"] = member.Userid,
                ["status"] = JsonValue.Create(member.Status),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            // 官方响应不回带 meetingid，回填入参便于模型把结果与请求对齐。
            ["meetingid"] = request.Meetingid,
            ["title"] = info.Title,
            ["meeting_start"] = JsonValue.Create(info.MeetingStart),
            ["meeting_duration"] = JsonValue.Create(info.MeetingDuration),
            ["status"] = JsonValue.Create(info.Status),
            ["meeting_type"] = JsonValue.Create(info.MeetingType),
            ["location"] = info.Location,
            ["description"] = info.Description,
            ["admin_userid"] = info.AdminUserid,
            ["meeting_code"] = info.MeetingCode,
            ["cal_id"] = info.CalId,
            ["attendees"] = members,
            ["attendee_count"] = members.Count,
            ["guest_count"] = info.Guests?.Count ?? 0,
            ["has_vote"] = JsonValue.Create(info.HasVote),
            ["sub_meeting_count"] = info.SubMeetings?.Count ?? 0,
        }));
    }

    /// <summary>执行 <c>meeting.attendees</c>。</summary>
    /// <param name="args">模型侧入参（<c>meetingid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkMeetingAttendeesTool))]
    public async Task<ToolResult> ListAttendeesAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = MeetingAttendeesArgs.Unpack(args);

        long? start = null;
        long? end = null;
        if (!string.IsNullOrWhiteSpace(request.StartTime))
        {
            if (!ToolTimeText.TryParseUnixSeconds(request.StartTime, out var parsedStart))
            {
                return ToolResult.FromError(TimeFormatHint);
            }

            start = parsedStart;
        }

        if (!string.IsNullOrWhiteSpace(request.EndTime))
        {
            if (!ToolTimeText.TryParseUnixSeconds(request.EndTime, out var parsedEnd))
            {
                return ToolResult.FromError(TimeFormatHint);
            }

            end = parsedEnd;
        }

        if (start.HasValue && end.HasValue)
        {
            if (end.Value <= start.Value)
            {
                return ToolResult.FromError("参数非法：end_time 必须大于 start_time。");
            }

            if (end.Value - start.Value > MaxAttendeeSpanSeconds)
            {
                return ToolResult.FromError("参数非法：参会人查询的时间跨度不得超过 31 天。");
            }
        }

        var limit = request.Limit ?? MaxPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxPageSize} 之间，且续页须与首次一致。");
        }

        var response = await _advanced.GetAttendeeListAsync(
            new GetMeetingAttendeeListRequest
            {
                Meetingid = request.Meetingid,
                StartTime = start,
                EndTime = end,
                Cursor = request.Cursor,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取参会成员失败。");
        }

        var attendees = new JsonArray();
        foreach (var attendee in outcome.Data?.Attendees ?? [])
        {
            attendees.Add((JsonNode?)new JsonObject
            {
                ["userid"] = attendee.Userid,
                ["role"] = JsonValue.Create(attendee.Role),
                ["join_time"] = attendee.JoinTime,
                ["quit_time"] = attendee.QuitTime,
                ["join_type"] = JsonValue.Create(attendee.JoinType),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["attendees"] = attendees,
            ["count"] = attendees.Count,
            ["has_more"] = JsonValue.Create(outcome.Data?.HasMore),
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }
}
