// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Services;
using Mud.Wechat.OfficialAccount.DataModels.FreePublish;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Endpoints;

/// <summary>
/// 已发布文章端点（方案 §4.1 / §4.2）。
/// <b>只读是官方契约而非 Demo 取舍</b>：没有 <c>freepublish/update</c>，改内容只能删除（不可逆）后重发。
/// </summary>
public static class PublishedEndpoints
{
    private const string BatchGetPath = "/cgi-bin/freepublish/batchget";
    private const string GetArticlePath = "/cgi-bin/freepublish/getarticle";
    private const string DeletePath = "/cgi-bin/freepublish/delete";

    private const int PageMax = 20;

    public static WebApplication MapPublishedReadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/articles/published", async (
            IMpFreePublishService freePublish,
            int offset = 0,
            int count = 10,
            CancellationToken cancellationToken = default) =>
        {
            var response = await freePublish.BatchGetPublishedAsync(new MpFreePublishBatchGetRequest
            {
                Offset = Math.Max(0, offset),
                Count = ApiOutcome.Clamp(count, 1, PageMax),

                // 同草稿列表：标题只在 content.news_item[0] 里，no_content=1 时列表页拿不到标题。
                NoContent = 0,
            }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, BatchGetPath);
            }

            return Results.Json(new
            {
                items = (response.Items ?? new List<MpFreePublishListItem>())
                    .Select(PublishedProjection.FromListItem).ToList(),
                totalCount = response.TotalCount,
                itemCount = response.ItemCount,
            });
        });

        app.MapGet("/api/articles/published/{articleId}", async (
            IMpFreePublishService freePublish,
            string articleId,
            CancellationToken cancellationToken) =>
        {
            var response = await freePublish.GetArticleAsync(
                new MpFreePublishGetArticleRequest { ArticleId = articleId }, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, GetArticlePath);
            }

            return Results.Json(new PublishedDetail(
                articleId, PublishedProjection.FromNewsItems(response.NewsItems)));
        });

        return app;
    }

    /// <summary>撤下已发布文章（不可逆，写闸内）。</summary>
    public static WebApplication MapPublishedWriteEndpoints(this WebApplication app)
    {
        app.MapDelete("/api/articles/published/{articleId}", async (
            IMpFreePublishService freePublish,
            string articleId,
            int? index,
            string? confirm,
            CancellationToken cancellationToken) =>
        {
            if (!string.Equals(confirm, articleId, StringComparison.Ordinal))
            {
                return ApiOutcome.Invalid($"撤下已发布文章不可逆：confirm 必须等于 {articleId}");
            }

            var response = await freePublish.DeletePublishAsync(new MpFreePublishDeleteRequest
            {
                ArticleId = articleId,

                // 官方：index 省略或 0 = 删除该发布任务下的全部篇章（第一篇编号是 1）。
                Index = index,
            }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, DeletePath);
            }

            return Results.Json(new { articleId, index = index ?? 0, deleted = true });
        });

        return app;
    }
}
