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
using Mud.Wechat.Work.DataModels.ExternalContact.Moment;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 客户朋友圈域只读执行器：发表记录 → 企业发表的执行情况 → 互动名单。
/// </summary>
/// <remarks>
/// <para>
/// <b>三跳自举</b>：<c>moment.list</c>（按 30 天窗口取 moment_id）→ <c>moment.publish_list</c>（企业发表的朋友圈
/// 由哪些成员执行）→ <c>moment.comments</c>（按「朋友圈 + 发表成员」取互动）。官方把互动数据的钥匙做成两段，
/// 工具面顺着官方形状走，不提供「一把查全部互动」的假能力。
/// </para>
/// <para>
/// <b>投影边界</b>：文本、图片张数、有无视频、图文标题与链接、地点名进模型可见面；
/// <c>media_id</c>（宿主素材资产，模型无法消费二进制）与经纬度（行踪）不投影。互动只给互动者身份与时间——
/// 官方此端点不返回评论正文。
/// </para>
/// </remarks>
/// <param name="moment">客户朋友圈域 SDK。</param>
public sealed class ExternalContactMomentToolHandlers(IWechatWorkExternalContactMomentService moment)
{
    /// <summary>朋友圈列表的官方单页上限。</summary>
    private const int MaxListPageSize = 20;

    /// <summary>朋友圈列表的官方默认单页条数。</summary>
    private const int DefaultListPageSize = 20;

    /// <summary>发表执行列表的官方单页上限。</summary>
    private const int MaxTaskPageSize = 1000;

    /// <summary>发表执行列表的官方默认单页条数。</summary>
    private const int DefaultTaskPageSize = 500;

    /// <summary>官方允许的时间跨度上限（30 天）。</summary>
    private const long MaxSpanSeconds = 30L * 24 * 60 * 60;

    private const string TimeFormatHint =
        "时间格式非法：start_time / end_time 应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"）。";

    private readonly IWechatWorkExternalContactMomentService _moment =
        moment ?? throw new ArgumentNullException(nameof(moment));

    /// <summary>执行 <c>externalcontact.moment.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactMomentListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactMomentListArgs.Unpack(args);

        if (!ToolTimeText.TryParseUnixSeconds(request.StartTime, out var start))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (!ToolTimeText.TryParseUnixSeconds(request.EndTime, out var end))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (end <= start)
        {
            return ToolResult.FromError("参数非法：end_time 必须大于 start_time。");
        }

        if (end - start > MaxSpanSeconds)
        {
            return ToolResult.FromError("参数非法：起止时间间隔不得超过 30 天（官方口径），请缩小范围后分批拉取。");
        }

        // filter_type 的取值闭集（0~2）由声明面 EnumType 与生成的解包器收口——越界值到不了这里。
        var limit = request.Limit ?? DefaultListPageSize;
        if (limit is < 1 or > MaxListPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxListPageSize} 之间。");
        }

        var response = await _moment.GetMomentListAsync(
            new GetMomentListRequest
            {
                StartTime = start,
                EndTime = end,
                Creator = request.Creator,
                FilterType = request.FilterType,
                Cursor = request.Cursor,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactMomentList);
        }

        var moments = new JsonArray();
        foreach (var item in outcome.Data?.MomentList ?? [])
        {
            moments.Add((JsonNode?)new JsonObject
            {
                ["moment_id"] = item.MomentId,
                ["creator"] = item.Creator,
                ["create_time"] = JsonValue.Create(item.CreateTime),
                ["create_type"] = JsonValue.Create(item.CreateType),
                ["visible_type"] = JsonValue.Create(item.VisibleType),
                ["text"] = item.Text?.Content,
                ["image_count"] = item.Image?.Count ?? 0,
                ["has_video"] = item.Video is not null,
                ["link"] = ToLinkNode(item.Link),
                ["location"] = item.Location?.Name,
            });
        }

        // 空列表是合法答案（该时间段内没有发表记录），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["moment_list"] = moments,
            ["count"] = moments.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.moment.publish_list</c>。</summary>
    /// <param name="args">模型侧入参（<c>moment_id</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactMomentPublishListTool))]
    public async Task<ToolResult> ListPublishTasksAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactMomentPublishListArgs.Unpack(args);

        var limit = request.Limit ?? DefaultTaskPageSize;
        if (limit is < 1 or > MaxTaskPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxTaskPageSize} 之间。");
        }

        var response = await _moment.GetMomentPublishListAsync(
            new GetMomentPublishListRequest
            {
                MomentId = request.MomentId,
                Cursor = request.Cursor,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(
                outcome.ErrorText ?? "拉取朋友圈执行列表失败（该 moment_id 可能不是企业发表的朋友圈）。");
        }

        var tasks = new JsonArray();
        foreach (var item in outcome.Data?.TaskList ?? [])
        {
            tasks.Add((JsonNode?)new JsonObject
            {
                ["userid"] = item.Userid,
                ["publish_status"] = JsonValue.Create(item.PublishStatus),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["task_list"] = tasks,
            ["count"] = tasks.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.moment.comments</c>。</summary>
    /// <param name="args">模型侧入参（<c>moment_id</c> / <c>userid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactMomentCommentsTool))]
    public async Task<ToolResult> GetCommentsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactMomentCommentsArgs.Unpack(args);

        var response = await _moment.GetMomentCommentsAsync(
            new GetMomentCommentsRequest
            {
                MomentId = request.MomentId,
                Userid = request.Userid,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactMomentPublishList);
        }

        var comments = ToActorArray(outcome.Data?.CommentList);
        var likes = ToActorArray(outcome.Data?.LikeList);
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["comment_list"] = comments,
            ["like_list"] = likes,
            ["comment_count"] = comments.Count,
            ["like_count"] = likes.Count,
        }));
    }

    /// <summary>图文附件投影：标题 + 链接（<c>media_id</c> 属宿主素材资产，不投影）。</summary>
    private static JsonNode? ToLinkNode(MomentLinkMedia? link)
        => link is null
            ? null
            : new JsonObject
            {
                ["title"] = link.Title,
                ["url"] = link.Url,
            };

    /// <summary>互动者投影：客户给 external_userid、企业成员给 userid，二者互斥（用 actor 区分来源）。</summary>
    private static JsonArray ToActorArray(List<MomentCommentItem>? items)
    {
        var actors = new JsonArray();
        foreach (var item in items ?? [])
        {
            var isCustomer = !string.IsNullOrWhiteSpace(item.ExternalUserid);
            actors.Add((JsonNode?)new JsonObject
            {
                ["actor"] = isCustomer ? "customer" : "member",
                ["userid"] = item.Userid,
                ["external_userid"] = item.ExternalUserid,
                ["create_time"] = JsonValue.Create(item.CreateTime),
            });
        }

        return actors;
    }
}
