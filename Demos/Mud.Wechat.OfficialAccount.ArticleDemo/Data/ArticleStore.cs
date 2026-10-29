// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>
/// 本地台账的<b>唯一</b>写入口（方案 §7.5）：端点不直接写 LINQ。
/// 每个方法一个短生命周期 <see cref="ArticleDbContext"/>（只经 <see cref="IDbContextFactory{TContext}"/> 取），
/// 绝不跨请求复用 —— SQLite 单写，长事务会把列表查询一起堵死。
/// </summary>
public sealed class ArticleStore
{
    /// <summary>快照来源：新建草稿。</summary>
    public const string SourceCreate = "Create";

    /// <summary>快照来源：编辑草稿。</summary>
    public const string SourceUpdate = "Update";

    private readonly IDbContextFactory<ArticleDbContext> _factory;

    public ArticleStore(IDbContextFactory<ArticleDbContext> factory)
        => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <summary>写快照时的文章字段（与 SDK DTO 解耦：store 不认识微信类型）。</summary>
    public readonly record struct RevisionInput(
        string Title,
        string? Author,
        string? Digest,
        string? ContentHtml,
        string? ContentSourceUrl,
        string? ThumbMediaId,
        int? NeedOpenComment,
        int? OnlyFansCanComment);

    /// <summary>落一条版本快照（同 <c>(mediaId, index)</c> 内版本号自增）。</summary>
    public async Task<DraftRevision> AddRevisionAsync(
        string mediaId,
        int itemIndex,
        RevisionInput input,
        string source,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaId);

        using var db = await _factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var maxNo = await db.DraftRevisions
            .Where(x => x.WechatMediaId == mediaId && x.ItemIndex == itemIndex)
            .Select(x => (int?)x.RevisionNo)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false);

        var revision = new DraftRevision
        {
            WechatMediaId = mediaId,
            ItemIndex = itemIndex,
            RevisionNo = (maxNo ?? 0) + 1,
            Title = input.Title,
            Author = input.Author,
            Digest = input.Digest,
            ContentHtml = input.ContentHtml,
            ContentSourceUrl = input.ContentSourceUrl,
            ThumbMediaId = input.ThumbMediaId,
            NeedOpenComment = input.NeedOpenComment,
            OnlyFansCanComment = input.OnlyFansCanComment,
            Source = source,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.DraftRevisions.Add(revision);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return revision;
    }

    /// <summary>版本快照列表（只读展示，按版本号升序）。</summary>
    public async Task<List<DraftRevision>> ListRevisionsAsync(
        string mediaId,
        int itemIndex,
        CancellationToken cancellationToken)
    {
        using var db = await _factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        return await db.DraftRevisions
            .AsNoTracking()
            .Where(x => x.WechatMediaId == mediaId && x.ItemIndex == itemIndex)
            .OrderByDescending(x => x.RevisionNo)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>登记发布任务（按 <c>PublishId</c> 幂等：已存在则覆盖字段，不产生第二行）。</summary>
    public async Task SaveJobAsync(PublishJob job, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(job.PublishId);

        using var db = await _factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var existing = await db.PublishJobs
            .FirstOrDefaultAsync(x => x.PublishId == job.PublishId, cancellationToken)
            .ConfigureAwait(false);

        if (existing == null)
        {
            db.PublishJobs.Add(job);
        }
        else
        {
            existing.WechatMediaId = job.WechatMediaId;
            existing.LatestStatus ??= job.LatestStatus;
            existing.ArticleId ??= job.ArticleId;
            existing.FailIdxJson ??= job.FailIdxJson;
            existing.CompletedAt ??= job.CompletedAt;
            existing.Attempts = Math.Max(existing.Attempts, job.Attempts);
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 追加一条状态快照并顺带刷新任务行。
    /// <c>publishId</c> 不在台账时（例如只单独查过状态）自动补一行，来源草稿记空串 ——
    /// 台账是「本 Demo 见过的发布」，不是官方的镜像。
    /// </summary>
    public async Task AppendSnapshotAsync(
        string publishId,
        string? mediaId,
        int? status,
        int? errorCode,
        string? articleId,
        string? failIdxJson,
        bool terminal,
        DateTimeOffset polledAt,
        CancellationToken cancellationToken)
    {
        using var db = await _factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var job = await db.PublishJobs.FirstOrDefaultAsync(x => x.PublishId == publishId, cancellationToken)
            .ConfigureAwait(false);

        if (job == null)
        {
            job = new PublishJob
            {
                PublishId = publishId,
                WechatMediaId = mediaId ?? string.Empty,
                SubmittedAt = polledAt,
            };
            db.PublishJobs.Add(job);
        }

        // 轮询序号由 store 记账：新行尚未落库时查询返回 0 ⇒ 本轮即第 1 轮。
        var attemptNo = job.Id == 0
            ? 1
            : await db.PublishStatusSnapshots.CountAsync(x => x.PublishJobId == job.Id, cancellationToken)
                .ConfigureAwait(false) + 1;

        if (!string.IsNullOrEmpty(mediaId))
        {
            job.WechatMediaId = mediaId;
        }

job.LatestStatus = status;
        job.Attempts = Math.Max(job.Attempts, attemptNo);
        job.ArticleId ??= articleId;
        job.FailIdxJson ??= failIdxJson;
        job.CompletedAt ??= terminal ? polledAt : null;

        db.PublishStatusSnapshots.Add(new PublishStatusSnapshot
        {
            PublishJob = job,
            Status = status,
            ErrorCode = errorCode,
            PolledAt = polledAt,
            AttemptNo = attemptNo,
        });

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>发布历史（新→旧），带状态时间线。</summary>
    public async Task<List<PublishJob>> ListJobsAsync(int limit, CancellationToken cancellationToken)
    {
        using var db = await _factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        // SQLite provider 不支持在 DateTimeOffset 列上 ORDER BY（TEXT 存的是「日期+偏移」，
        // 跨偏移的字典序 ≠ 时间序，Provider 直接拒绝翻译）。故服务端按自增 Id（= 落库顺序）取页，
        // 再在客户端按 SubmittedAt 精确排序 —— 页内顺序修正，页边界仍由插入顺序决定。
        var rows = await db.PublishJobs
            .AsNoTracking()
            .Include(x => x.Snapshots)
            .OrderByDescending(x => x.Id)
            .Take(limit)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.OrderByDescending(x => x.SubmittedAt).ToList();
    }

    /// <summary>台账行数摘要（供 <c>/healthz</c>），无表时返回 0。</summary>
    public async Task<(int Jobs, int Revisions)> CountAsync(CancellationToken cancellationToken)
    {
        using var db = await _factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var jobs = await db.PublishJobs.CountAsync(cancellationToken).ConfigureAwait(false);
        var revisions = await db.DraftRevisions.CountAsync(cancellationToken).ConfigureAwait(false);
        return (jobs, revisions);
    }
}
