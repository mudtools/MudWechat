// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;

namespace Mud.Wechat.Demo.Shared;

/// <summary>
/// Demo 侧「设置链」的构造：入库模板 &lt; 环境专用文件 &lt; 本机真值文件 &lt; 命令行，
/// 环境变量再由 <see cref="DemoEnv"/> 压在最上层。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不直接改 <c>builder.Configuration.Sources</c></b>：ASP.NET Core 的
/// <c>WebApplicationBuilder</c> 在 <c>Build()</c> 时把宿主那一条链<b>接在自己 sources 尾部</b>，
/// 事后插进去的源会被链尾的 <c>appsettings.json</c> 反向压过 ——
/// 表现是「本机真值文件明明存在却读不到，读到的反而是模板里的占位符」，且不报错（DM-14 实测踩过）。
/// 插入位置依赖内部实现，跨版本还会漂。
/// </para>
/// <para>
/// 因此这里独立构造一条只读链：文件按「先加者优先级低」的顺序登记，内容根取
/// <c>AppContext.BaseDirectory</c>（Demo 直接执行 bin 下 dll 时与 ASP.NET 的内容根一致），
/// <c>optional: true</c> 让任何一层缺失都静默跳过，最终仍拿不到值时由 <see cref="DemoEnv"/> 一次性列全缺失项。
/// </para>
/// </remarks>
public static class DemoConfigSources
{
    /// <summary>
    /// 本机真值文件名。<c>.gitignore</c> 的 <c>**/appsettings.local.json</c> 已覆盖它 ——
    /// 入库的 <c>appsettings.json</c> 只该放占位符与非机密项。
    /// </summary>
    public const string LocalFileName = "appsettings.local.json";

    /// <summary>入库模板文件名（占位符会被 <see cref="DemoEnv"/> 按「未配置」处理）。</summary>
    public const string TemplateFileName = "appsettings.json";

    /// <summary>
    /// 构造 Demo 的设置链。
    /// </summary>
    /// <param name="environmentName">当前环境名（<c>Development</c> 时额外读 <c>appsettings.Development.json</c>）。</param>
    /// <param name="args">命令行参数（最低一层，但压过所有文件）。</param>
    /// <param name="localFileName">本机真值文件名。</param>
    public static IConfiguration BuildSettingsChain(
        string? environmentName,
        string[]? args,
        string localFileName = LocalFileName)
    {
        // 独立建造器：基点钉在程序集目录，与内容根口径一致（门禁直接执行 bin 下的 dll）。
        var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory);

        // 先加 = 优先级低，后加 = 优先级高。
        builder.AddJsonFile(TemplateFileName, optional: true, reloadOnChange: false);

        if (!string.IsNullOrEmpty(environmentName))
        {
            builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false);
        }

        builder.AddJsonFile(localFileName, optional: true, reloadOnChange: false);

        if (args is { Length: > 0 })
        {
            builder.AddCommandLine(args);
        }

        return builder.Build();
    }
}
