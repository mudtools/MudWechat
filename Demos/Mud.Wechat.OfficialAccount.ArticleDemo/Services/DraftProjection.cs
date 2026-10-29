// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Draft;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>草稿列表项（方案 §4.3）：<c>title</c> 来自 <c>content.news_item[0].title</c> —— 官方无独立标题字段。</summary>
public sealed record DraftSummary(
    string MediaId,
    string? Title,
    string? Author,
    string? Digest,
    string? ThumbMediaId,
    int ItemCount,
    long UpdateTime);

/// <summary>草稿单篇（<c>index</c> 即更新草稿时要传的篇序号，第一篇为 0/1 由官方按下标决定，此处取数组下标）。</summary>
public sealed record DraftArticleView(
    int Index,
    string? Title,
    string? Author,
    string? Digest,
    string? Content,
    string? ContentSourceUrl,
    string? ThumbMediaId,
    int? NeedOpenComment,
    int? OnlyFansCanComment,
    string? DraftUrl);

/// <summary>草稿详情。</summary>
public sealed record DraftDetail(string MediaId, IReadOnlyList<DraftArticleView> Articles);

/// <summary>SDK 草稿 DTO → 前端投影（只下发必要字段，正文列表页不带 <c>content</c>，方案 §4.1）。</summary>
public static class DraftProjection
{
    public static DraftSummary FromListItem(MpDraftListItem item)
    {
        var first = item.Content?.NewsItems is { Count: > 0 } items ? items[0] : null;

        return new DraftSummary(
            item.MediaId ?? string.Empty,
            first?.Title,
            first?.Author,
            first?.Digest,
            first?.ThumbMediaId,
            item.Content?.NewsItems?.Count ?? 0,
            item.UpdateTime);
    }

    public static DraftArticleView FromNewsItem(MpDraftNewsItem item, int index)
        => new(
            index,
            item.Title,
            item.Author,
            item.Digest,
            item.Content,
            item.ContentSourceUrl,
            item.ThumbMediaId,
            item.NeedOpenComment,
            item.OnlyFansCanComment,
            item.Url);

    public static IReadOnlyList<DraftArticleView> FromNewsItems(IReadOnlyList<MpDraftNewsItem>? items)
    {
        if (items == null)
        {
            return Array.Empty<DraftArticleView>();
        }

        var views = new List<DraftArticleView>(items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            views.Add(FromNewsItem(items[i], i));
        }

        return views;
    }
}
