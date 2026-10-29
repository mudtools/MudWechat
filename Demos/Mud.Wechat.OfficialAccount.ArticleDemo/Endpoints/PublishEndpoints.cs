// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Data;
using Mud.Wechat.OfficialAccount.ArticleDemo.Services;
using Mud.Wechat.OfficialAccount.DataModels.FreePublish;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Endpoints;

/// <summary>发布成功后的文章地址（官方 <c>article_detail.item[]</c>）。</summary>
public sealed record ArticleUrlView(int? Idx, string? ArticleUrl);

/// <summary>官方 <c>article_detail</c> 的投影。</summary>
public sealed record ArticleDetailView(int? Count, IReadOnlyList<ArticleUrlView> Items);

/// <summary>发布结果（方案 §4.3）：<c>publish_id</c> 与 <c>article_id</c> 是两套 id，不得混用。</summary>
public sealed record PublishSubmitResult(
    string PublishId,
    int? PublishStatus,
    string? ArticleId,
    ArticleDetailView? ArticleDetail,
    IReadOnlyList<long>? FailIdx);

/// <summary>提交发布的请求体。</summary>
internal sealed record PublishBody(string MediaId);

/// <summary>发布链端点（方案 §4.1 / §4.2 / §5.3）。</summary>
public static class PublishEndpoints
{
    private const string SubmitPath = "/cgi-bin/freepublish/submit";
    private const string GetPath = "/cgi-bin/freepublish/get";

    /// <summary>单次状态查询（读端点，顺带落一条快照）。</summary>
    public static WebApplication MapPublishReadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/articles/publish/{publishId}", async (
            IMpFreePublishService freePublish,
            ArticleStore store,
            string publishId,
            CancellationToken cancellationToken) =>
        {
            var response = await freePublish.GetPublishStatusAsync(
                new MpFreePublishGetRequest { PublishId = publishId }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, GetPath);
            }

            await store.AppendSnapshotAsync(
                publishId,
                null,
                response.PublishStatus,
                null,
                response.ArticleId,
                PublishStatus.ToFailIdxJson(response.FailIdx),
                PublishStatus.IsTerminal(response.PublishStatus),
                DateTimeOffset.UtcNow,
                cancellationToken).ConfigureAwait(false);

            return Results.Json(FromStatus(publishId, response));
        });

        return app;
    }

    /// <summary>写端点：提交发布 + 服务端轮询到终态。</summary>
    public static WebApplication MapPublishWriteEndpoints(this WebApplication app)
    {
        app.MapPost("/api/articles/publish", async (
            IMpFreePublishService freePublish,
            ArticleStore store,
            PublishBody body,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(body.MediaId))
            {
                return ApiOutcome.Invalid("mediaId 不能为空");
            }

            var submit = await freePublish.SubmitAsync(
                new MpFreePublishSubmitRequest { MediaId = body.MediaId }, cancellationToken).ConfigureAwait(false);

            if (!submit.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(submit, SubmitPath);
            }

            var publishId = submit.PublishId ?? string.Empty;

            // 先落台账再查状态：一旦进程在这两步之间被杀掉，重启后 /api/articles/jobs 仍能看到这条任务。
            await store.SaveJobAsync(new PublishJob
            {
                PublishId = publishId,
                WechatMediaId = body.MediaId,
                SubmittedAt = DateTimeOffset.UtcNow,
            }, cancellationToken).ConfigureAwait(false);

            return await PollOnceAsync(freePublish, store, publishId, body.MediaId, cancellationToken)
                .ConfigureAwait(false);
        });

        app.MapGet("/api/articles/publish/{publishId}/wait", async (
            IMpFreePublishService freePublish,
            ArticleStore store,
            PublishStatusWaiter waiter,
            string publishId,
            int timeoutSeconds = PublishStatusWaiter.DefaultTimeoutSeconds,
            CancellationToken cancellationToken = default) =>
        {
            var outcome = await waiter
                .WaitAsync(publishId, timeoutSeconds, cancellationToken)
                .ConfigureAwait(false);

            switch (outcome.Kind)
            {
                case PublishWaitKind.Completed:
                    return Results.Json(WithAttempts(FromStatus(publishId, outcome.Response!), outcome.Attempts));

                case PublishWaitKind.ApiFailure:
                    return Results.Json(new
                    {
                        errorCode = outcome.ErrorCode,
                        errorMessage = outcome.ErrorMessage,
                        requestUri = GetPath,
                        hint = ErrorHints.Describe(outcome.ErrorCode),
                    }, statusCode: StatusCodes.Status502BadGateway);

                default:
                    // 超时/次数用尽 ≠ 失败：官方仍在处理，只是本 Demo 不等了。
                    return Results.Json(new
                    {
                        publishId,
                        publishStatus = outcome.Response?.PublishStatus,
                        attempts = outcome.Attempts,
                        hint = outcome.Kind == PublishWaitKind.TimedOut
                            ? $"等待 {timeoutSeconds}s 仍在发布中：稍后用 publishId 再查，或看 /api/articles/jobs（台账已落库，跨重启可续查）"
                            : $"轮询 {outcome.Attempts} 次仍在发布中：请改用 /api/articles/jobs 续查",
                    }, statusCode: StatusCodes.Status502BadGateway);
            }
        });

        return app;
    }

    /// <summary>提交后立即查一次状态（提交 ≠ 完成）。</summary>
    private static async Task<IResult> PollOnceAsync(
        IMpFreePublishService freePublish,
        ArticleStore store,
        string publishId,
        string mediaId,
        CancellationToken cancellationToken)
    {
        var response = await freePublish.GetPublishStatusAsync(
            new MpFreePublishGetRequest { PublishId = publishId }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            await store.AppendSnapshotAsync(
                publishId, mediaId, null, response.ErrorCode, null, null, terminal: false,
                DateTimeOffset.UtcNow, cancellationToken).ConfigureAwait(false);

            return Results.Json(new
            {
                publishId,
                submitted = true,
                errorCode = response.ErrorCode,
                errorMessage = response.ErrorMessage,
                requestUri = GetPath,
                hint = "发布已受理，但状态查询失败：用 /api/articles/publish/{publishId}/wait 续查",
            });
        }

        await store.AppendSnapshotAsync(
            publishId,
            mediaId,
            response.PublishStatus,
            null,
            response.ArticleId,
            PublishStatus.ToFailIdxJson(response.FailIdx),
            PublishStatus.IsTerminal(response.PublishStatus),
            DateTimeOffset.UtcNow,
            cancellationToken).ConfigureAwait(false);

        return Results.Json(FromStatus(publishId, response));
    }

    private static PublishSubmitResult FromStatus(string publishId, MpFreePublishGetResponse response)
        => new(
            publishId,
            response.PublishStatus,
            response.ArticleId,
            response.ArticleDetail == null
                ? null
                : new ArticleDetailView(
                    response.ArticleDetail.Count,
                    (response.ArticleDetail.Items ?? new List<MpFreePublishArticleUrlItem>())
                        .Select(x => new ArticleUrlView(x.Idx, x.ArticleUrl))
                        .ToList()),
            response.FailIdx);

    // 轮询次数只在 wait 响应里出现，投影记录本身不带该字段。
    private static object WithAttempts(PublishSubmitResult result, int attempts)
        => new
        {
            publishId = result.PublishId,
            publishStatus = result.PublishStatus,
            articleId = result.ArticleId,
            articleDetail = result.ArticleDetail,
            failIdx = result.FailIdx,
            attempts,
        };
}
