// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Data;
using Mud.Wechat.OfficialAccount.ArticleDemo.Services;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Endpoints;

/// <summary>
/// 本地只读面（方案 §4.1）：发布台账与草稿版本快照。
/// 两者都是<b>台账而非权威源</b> —— 微信后台删了文章，本地仍会显示历史（§15.7），
/// 所以响应里固定带一句口径提示，前端再挂角标。
/// </summary>
public static class HistoryEndpoints
{
    private const string LedgerHint = "本地台账，微信为准：这里记录的是「本 Demo 写成功之后的样子」，不代表微信当前的状态。";

    public static WebApplication MapHistoryEndpoints(this WebApplication app)
    {
        app.MapGet("/api/articles/jobs", async (
            ArticleStore store,
            int limit = 50,
            CancellationToken cancellationToken = default) =>
        {
            var jobs = await store.ListJobsAsync(ApiOutcome.Clamp(limit, 1, 200), cancellationToken)
                .ConfigureAwait(false);

            return Results.Json(new
            {
                items = jobs.Select(LocalViewProjection.FromJob).ToList(),
                hint = LedgerHint,
            });
        });

        app.MapGet("/api/articles/drafts/{mediaId}/articles/{index}/revisions", async (
            ArticleStore store,
            string mediaId,
            int index,
            CancellationToken cancellationToken) =>
        {
            var revisions = await store.ListRevisionsAsync(mediaId, index, cancellationToken)
                .ConfigureAwait(false);

            return Results.Json(new
            {
                mediaId,
                index,
                items = revisions.Select(LocalViewProjection.FromRevision).ToList(),
                hint = LedgerHint + " 载入编辑器只填表单，不会自动写回微信。",
            });
        });

        return app;
    }
}
