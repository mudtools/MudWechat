// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>
/// 设计时工厂（<c>dotnet ef migrations add</c> 用）：存在本类时 EF 不会去启动整个 Web 宿主，
/// 因此生成迁移<b>不需要</b> <c>MUDWECHAT_DEMO_MP_APP_ID</c> 等凭据 —— 否则 <c>Program.cs</c> 的
/// 启动期 <c>DemoEnv.RequireAll</c> 会把设计时操作一起拦掉。
/// </summary>
internal sealed class ArticleDbContextFactory : IDesignTimeDbContextFactory<ArticleDbContext>
{
    public ArticleDbContext CreateDbContext(string[] args)
    {
        // 设计时不读 appsettings：迁移只依赖模型，落库路径在这里没有任何副作用。
        // 需要按文件路径生成时，走 MUDWECHAT_DEMO_MP_DB_PATH 即可（DemoEnv 变量优先）。
        var configuration = new ConfigurationBuilder().Build();
        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseSqlite(DbPathResolver.ConnectionString(DbPathResolver.Resolve(configuration)))
            .Options;

        return new ArticleDbContext(options);
    }
}
