// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>
/// 启动期库装配（方案 §7.2）：<c>Migrate()</c> 而非 <c>EnsureCreated()</c> ——
/// 迁移文件入库，读者克隆后无需装 <c>dotnet-ef</c> 即可起来；后续加表走增量迁移而不是删库重建。
/// </summary>
public static class ArticleDbInitializer
{
    /// <summary>建库 → 应用迁移 → 开 WAL（读写并发，轮询期间列表仍可查）。</summary>
    public static async Task InitializeAsync(
        IDbContextFactory<ArticleDbContext> factory,
        string databasePath,
        CancellationToken cancellationToken)
    {
        await using (var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
        {
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        // PRAGMA 必须走裸连接：EF 的连接管理不保证同一物理连接，journal_mode 是库级持久设置，执行一次即可。
        await using var connection = new SqliteConnection(DbPathResolver.ConnectionString(databasePath));
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode=WAL;";
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
