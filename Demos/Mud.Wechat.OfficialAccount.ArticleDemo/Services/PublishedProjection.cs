// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.FreePublish;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>已发布列表项：官方条目键是 <c>article_id</c>（不是 <c>item_id</c>，也不是草稿的 <c>media_id</c>）。</summary>
public sealed record PublishedSummary(
    string ArticleId,
    string? Title,
    string? Author,
    string? Digest,
    string? ThumbUrl,
    int ItemCount,
    bool? IsDeleted,
    long UpdateTime);

/// <summary>已发布单篇（<b>只读</b>：官方无 <c>freepublish/update</c>，改内容只能删除后重发）。</summary>
public sealed record PublishedArticleView(
    string? Title,
    string? Author,
    string? Digest,
    string? Content,
    string? ContentSourceUrl,
    string? ThumbMediaId,
    string? ThumbUrl,
    string? Url,
    bool? IsDeleted);

/// <summary>已发布详情。</summary>
public sealed record PublishedDetail(string ArticleId, IReadOnlyList<PublishedArticleView> NewsItems);

/// <summary>SDK 发布 DTO → 前端投影。</summary>
public static class PublishedProjection
{
    public static PublishedSummary FromListItem(MpFreePublishListItem item)
    {
        var first = item.Content?.NewsItems is { Count: > 0 } items ? items[0] : null;

        return new PublishedSummary(
            item.ArticleId ?? string.Empty,
            first?.Title,
            first?.Author,
            first?.Digest,
            first?.ThumbUrl,
            item.Content?.NewsItems?.Count ?? 0,
            first?.IsDeleted,
            item.UpdateTime);
    }

    public static PublishedArticleView FromNewsItem(MpFreePublishNewsItem item)
        => new(
            item.Title,
            item.Author,
            item.Digest,
            item.Content,
            item.ContentSourceUrl,
            item.ThumbMediaId,
            item.ThumbUrl,
            item.Url,
            item.IsDeleted);

    public static IReadOnlyList<PublishedArticleView> FromNewsItems(IReadOnlyList<MpFreePublishNewsItem>? items)
    {
        if (items == null)
        {
            return Array.Empty<PublishedArticleView>();
        }

        var views = new List<PublishedArticleView>(items.Count);
        foreach (var item in items)
        {
            views.Add(FromNewsItem(item));
        }

        return views;
    }
}
