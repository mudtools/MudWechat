// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Data;
using Mud.Wechat.OfficialAccount.ArticleDemo.Services;
using Mud.Wechat.OfficialAccount.DataModels.Draft;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Endpoints;

/// <summary>编辑器提交的一篇文章（字段名照官方，<c>index</c> 由路由给）。</summary>
internal sealed record ArticleInput(
    string? Title,
    string? Author,
    string? Digest,
    string? Content,
    string? ContentSourceUrl,
    string? ThumbMediaId,
    int? NeedOpenComment,
    int? OnlyFansCanComment);

/// <summary>新建草稿请求体：官方 <c>draft/add</c> 的 <c>articles</c> 是<b>数组</b>。</summary>
internal sealed record DraftCreateBody(List<ArticleInput> Articles);

/// <summary>草稿域端点（方案 §4.1 / §4.2 / §5.2）。</summary>
public static class DraftEndpoints
{
    private const string BatchGetPath = "/cgi-bin/draft/batchget";
    private const string GetPath = "/cgi-bin/draft/get";
    private const string AddPath = "/cgi-bin/draft/add";
    private const string UpdatePath = "/cgi-bin/draft/update";
    private const string DeletePath = "/cgi-bin/draft/delete";

    /// <summary>官方 <c>draft/batchget</c> 与 <c>freepublish/batchget</c> 的 count 均为 1~20（守卫 CT/MD 锁定）。</summary>
    private const int PageMax = 20;

    /// <summary>读端点：不受写闸限制。</summary>
    public static WebApplication MapDraftReadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/articles/drafts", async (
            IMpDraftService drafts,
            int? offset = null,
            int? count = null,
            CancellationToken cancellationToken = default) =>
        {
            // no_content 必须为 0：官方草稿列表**没有独立 title 字段**，标题只存在于 content.news_item[0].title。
            // 取回 content 后由投影裁掉，绝不把整段正文下发给列表页（方案 §4.1）。
            var response = await drafts.BatchGetDraftsAsync(new MpDraftBatchGetRequest
            {
                Offset = Math.Max(0, offset ?? 0),
                Count = ApiOutcome.Clamp(count ?? 10, 1, PageMax),
                NoContent = 0,
            }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, BatchGetPath);
            }

            return Results.Json(new
            {
                items = (response.Items ?? new List<MpDraftListItem>()).Select(DraftProjection.FromListItem).ToList(),
                totalCount = response.TotalCount,
                itemCount = response.ItemCount,
            });
        });

        app.MapGet("/api/articles/drafts/{mediaId}", async (
            IMpDraftService drafts,
            string mediaId,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(mediaId))
            {
                return ApiOutcome.Invalid("mediaId 不能为空");
            }

            var response = await drafts.GetDraftAsync(
                new MpDraftGetRequest { MediaId = mediaId }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, GetPath);
            }

            // news_item 的下标即更新草稿时要传的 index（方案 §3）。
            return Results.Json(new DraftDetail(mediaId, DraftProjection.FromNewsItems(response.NewsItems)));
        });

        return app;
    }

    /// <summary>写端点：仅在写闸放行时映射（闸关 ⇒ 路由不存在 ⇒ 404）。</summary>
    public static WebApplication MapDraftWriteEndpoints(this WebApplication app)
    {
        app.MapPost("/api/articles/drafts", async (
            IMpDraftService drafts,
            ArticleStore store,
            DraftCreateBody body,
            CancellationToken cancellationToken) =>
        {
            if (body.Articles is not { Count: > 0 } articles)
            {
                return ApiOutcome.Invalid("articles 不能为空数组");
            }

            var request = new MpDraftAddRequest();
            foreach (var article in articles)
            {
                request.Articles.Add(ToDraftArticle(article));
            }

            var response = await drafts.AddDraftAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, AddPath);
            }

            var mediaId = response.MediaId ?? string.Empty;

            // 写微信成功之后才落快照：本地台账记录的是「官方已接受的样子」，不是第二权威源（§2）。
            for (var i = 0; i < articles.Count; i++)
            {
                await store.AddRevisionAsync(
                    mediaId, i, ToRevisionInput(articles[i]), ArticleStore.SourceCreate, cancellationToken)
                    .ConfigureAwait(false);
            }

            return Results.Json(new { mediaId, articleCount = articles.Count });
        });

        app.MapPut("/api/articles/drafts/{mediaId}/articles/{index}", async (
            IMpDraftService drafts,
            ArticleStore store,
            string mediaId,
            int index,
            ArticleInput article,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(mediaId))
            {
                return ApiOutcome.Invalid("mediaId 不能为空");
            }

            if (index < 0)
            {
                return ApiOutcome.Invalid("index 不能为负（草稿第一篇的下标是 0）");
            }

            // 官方 quirk：draft/update 的 articles 是**单对象**，与 draft/add 的数组不同形（守卫 CT2 锁定，照录不抹平）。
            var response = await drafts.UpdateDraftAsync(new MpDraftUpdateRequest
            {
                MediaId = mediaId,
                Index = index,
                Articles = ToDraftArticle(article),
            }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, UpdatePath);
            }

            var revision = await store.AddRevisionAsync(
                mediaId, index, ToRevisionInput(article), ArticleStore.SourceUpdate, cancellationToken)
                .ConfigureAwait(false);

            return Results.Json(new { mediaId, index, revisionNo = revision.RevisionNo });
        });

        app.MapDelete("/api/articles/drafts/{mediaId}", async (
            IMpDraftService drafts,
            string mediaId,
            string? confirm,
            CancellationToken cancellationToken) =>
        {
            // 不可逆操作：确认串必须等于目标 id（前端再加一次弹窗，方案 §11.4）。
            if (!string.Equals(confirm, mediaId, StringComparison.Ordinal))
            {
                return ApiOutcome.Invalid($"删除草稿不可逆：confirm 必须等于 {mediaId}");
            }

            var response = await drafts.DeleteDraftAsync(
                new MpDraftDeleteRequest { MediaId = mediaId }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, DeletePath);
            }

            return Results.Json(new { mediaId, deleted = true });
        });

        return app;
    }

    private static MpDraftArticle ToDraftArticle(ArticleInput input) => new()
    {
        Title = input.Title ?? string.Empty,
        Author = input.Author,
        Digest = input.Digest,
        Content = input.Content ?? string.Empty,
        ContentSourceUrl = input.ContentSourceUrl,
        ThumbMediaId = input.ThumbMediaId,
        NeedOpenComment = input.NeedOpenComment,
        OnlyFansCanComment = input.OnlyFansCanComment,
    };

    private static ArticleStore.RevisionInput ToRevisionInput(ArticleInput input)
        => new(
            input.Title ?? string.Empty,
            input.Author,
            input.Digest,
            input.Content,
            input.ContentSourceUrl,
            input.ThumbMediaId,
            input.NeedOpenComment,
            input.OnlyFansCanComment);
}
