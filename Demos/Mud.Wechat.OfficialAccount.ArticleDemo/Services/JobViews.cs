// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Data;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>发布任务的一轮轮询（方案 §4.3 的 <c>snapshots</c> 元素）。</summary>
public sealed record SnapshotView(int? Status, int? ErrorCode, DateTimeOffset PolledAt, int AttemptNo);

/// <summary>发布台账条目：跨重启可见，「提交 ≠ 完成」的证据链就在这里。</summary>
public sealed record JobView(
    string PublishId,
    string MediaId,
    int? LatestStatus,
    string? ArticleId,
    int Attempts,
    DateTimeOffset SubmittedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<SnapshotView> Snapshots);

/// <summary>
/// 草稿版本快照：<c>title/digest/source/createdAt</c> 供列表展示，
/// 其余字段供「载入编辑器」（方案 §5.2 —— 只填表单，<b>不自动写回微信</b>，避免一次误点覆盖草稿）。
/// </summary>
public sealed record RevisionView(
    int RevisionId,
    string MediaId,
    int Index,
    int RevisionNo,
    string Title,
    string? Digest,
    string Source,
    DateTimeOffset CreatedAt,
    string? Content,
    string? Author,
    string? ContentSourceUrl,
    string? ThumbMediaId,
    int? NeedOpenComment,
    int? OnlyFansCanComment);

/// <summary>本地实体 → 前端投影。</summary>
public static class LocalViewProjection
{
    public static JobView FromJob(PublishJob job)
        => new(
            job.PublishId,
            job.WechatMediaId,
            job.LatestStatus,
            job.ArticleId,
            job.Attempts,
            job.SubmittedAt,
            job.CompletedAt,
            job.Snapshots
                .OrderBy(x => x.AttemptNo)
                .Select(x => new SnapshotView(x.Status, x.ErrorCode, x.PolledAt, x.AttemptNo))
                .ToList());

    public static RevisionView FromRevision(DraftRevision revision)
        => new(
            revision.Id,
            revision.WechatMediaId,
            revision.ItemIndex,
            revision.RevisionNo,
            revision.Title,
            revision.Digest,
            revision.Source,
            revision.CreatedAt,
            revision.ContentHtml,
            revision.Author,
            revision.ContentSourceUrl,
            revision.ThumbMediaId,
            revision.NeedOpenComment,
            revision.OnlyFansCanComment);
}
