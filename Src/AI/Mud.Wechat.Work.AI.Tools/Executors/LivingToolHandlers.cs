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
using Mud.Wechat.Work.DataModels.Living;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 直播域只读执行器：成员直播列表 → 直播详情 → 观看统计。
/// </summary>
/// <remarks>
/// <b>凭证与外部数据不外发</b>：<c>push_stream_url</c>（推流凭证）与 <c>external_users</c>（外部联系人 ID 与邀请关系）
/// 均不投影；只保留企业内部可解释的事实与外部观看人数。
/// </remarks>
/// <param name="living">直播域 SDK。</param>
public sealed class LivingToolHandlers(IWechatWorkLivingService living)
{
    private const int MaxPageSize = 100;

    private readonly IWechatWorkLivingService _living = living ?? throw new ArgumentNullException(nameof(living));

    /// <summary>执行 <c>living.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkLivingListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = LivingListArgs.Unpack(args);

        var limit = request.Limit ?? MaxPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            throw WechatToolArgumentErrors.Invalid($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var response = await _living.GetUserAllLivingIdAsync(
            new GetUserAllLivingIdRequest
            {
                Userid = request.Userid,
                Cursor = request.Cursor,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.LivingList);
        }

        var ids = new JsonArray();
        foreach (var livingid in outcome.Data?.LivingidList ?? [])
        {
            ids.Add((JsonNode?)JsonValue.Create(livingid));
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["livingid_list"] = ids,
            ["count"] = ids.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>living.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>livingid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkLivingGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = LivingGetArgs.Unpack(args);

        var response = await _living.GetLivingInfoAsync(request.Livingid, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.LivingGet);
        }

        if (outcome.Data?.LivingInfo is not { } info)
        {
            return WechatToolResults.NotFound(WechatToolNames.LivingGet);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["livingid"] = request.Livingid,
            ["theme"] = info.Theme,
            ["living_start"] = JsonValue.Create(info.LivingStart),
            ["living_duration"] = JsonValue.Create(info.LivingDuration),
            ["status"] = JsonValue.Create(info.Status),
            ["type"] = JsonValue.Create(info.Type),
            ["anchor_userid"] = info.AnchorUserid,
            ["description"] = info.Description,
            ["viewer_num"] = JsonValue.Create(info.ViewerNum),
            ["comment_num"] = JsonValue.Create(info.CommentNum),
            ["mic_num"] = JsonValue.Create(info.MicNum),
            ["online_count"] = JsonValue.Create(info.OnlineCount),
            ["subscribe_count"] = JsonValue.Create(info.SubscribeCount),
            ["replay_status"] = JsonValue.Create(info.ReplayStatus),
        }));
    }

    /// <summary>执行 <c>living.watch_stat</c>。</summary>
    /// <param name="args">模型侧入参（<c>livingid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkLivingWatchStatTool))]
    public async Task<ToolResult> GetWatchStatAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = LivingWatchStatArgs.Unpack(args);

        var response = await _living.GetWatchStatAsync(
            new GetLivingWatchStatRequest { Livingid = request.Livingid, NextKey = request.NextKey },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.LivingWatchStat);
        }

        var users = new JsonArray();
        foreach (var user in outcome.Data?.StatInfo?.Users ?? [])
        {
            users.Add((JsonNode?)new JsonObject
            {
                ["userid"] = user.Userid,
                ["watch_time"] = JsonValue.Create(user.WatchTime),
                ["is_comment"] = JsonValue.Create(user.IsComment),
                ["is_mic"] = JsonValue.Create(user.IsMic),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["users"] = users,
            ["count"] = users.Count,
            ["external_user_count"] = outcome.Data?.StatInfo?.ExternalUsers?.Count ?? 0,
            ["ending"] = JsonValue.Create(outcome.Data?.Ending),
            ["next_key"] = outcome.Data?.NextKey,
        }));
    }
}
