// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>
/// 本地台账库（SQLite）：<b>台账与快照，不是权威源</b>（方案 §2）—— 草稿唯一来源是微信草稿箱，
/// 微信后台删了文章时本地 <see cref="PublishJobs"/> 仍留历史，读侧须自行看官方 <c>is_deleted</c>。
/// </summary>
public sealed class ArticleDbContext(DbContextOptions<ArticleDbContext> options) : DbContext(options)
{
    public DbSet<DraftRevision> DraftRevisions => Set<DraftRevision>();

    public DbSet<PublishJob> PublishJobs => Set<PublishJob>();

    public DbSet<PublishStatusSnapshot> PublishStatusSnapshots => Set<PublishStatusSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DraftRevision>(entity =>
        {
            entity.Property(x => x.WechatMediaId).IsRequired();
            entity.Property(x => x.Title).IsRequired();
            entity.Property(x => x.Source).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();

            // §7.3：同一 (草稿, 下标) 内版本号唯一。
            entity.HasIndex(x => new { x.WechatMediaId, x.ItemIndex, x.RevisionNo }).IsUnique();
        });

        modelBuilder.Entity<PublishJob>(entity =>
        {
            entity.Property(x => x.PublishId).IsRequired();
            entity.Property(x => x.WechatMediaId).IsRequired();
            entity.Property(x => x.SubmittedAt).IsRequired();

            entity.HasIndex(x => x.PublishId).IsUnique();
            entity.HasIndex(x => x.ArticleId);

            entity.HasMany(x => x.Snapshots)
                .WithOne(x => x.PublishJob!)
                .HasForeignKey(x => x.PublishJobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PublishStatusSnapshot>(entity =>
        {
            entity.Property(x => x.PolledAt).IsRequired();
        });
    }
}
