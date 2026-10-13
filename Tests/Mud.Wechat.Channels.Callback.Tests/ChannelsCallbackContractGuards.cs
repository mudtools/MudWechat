// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.Callback.Tests;

/// <summary>
/// 小店回调包契约守卫（CB-CH 系列）：组合根末端注册表封闭接线（v1 方案 F15①/D12）。
/// </summary>
/// <remarks>
/// <para>
/// 本文件为 <c>ChannelsCallbackScaffoldGuards</c> 备注中规划的 P3 期守卫载体（CB-CH 系列），
/// 首枚落地 CB-CH-SEAL；后续回调契约守卫随设计方案 v1 §3.5 / §5.1 在本类扩充。
/// </para>
/// <para>
/// 与企微 CB-SEAL / 公众号 CB-MP-SEAL / 支付 PAY-CB-SEAL 四线同构：各线回调注册表的
/// <c>Seal()</c> 封闭接线由<b>本线</b>守卫锁定（不跨线读源码，保持守卫的产品线归属）。
/// </para>
/// </remarks>
public class ChannelsCallbackContractGuards
{
    /// <summary>
    /// CB-CH-SEAL（v1 方案 F15①/D12）：处理器 / 拦截器 / 结构化应答三张注册表必须在
    /// <c>UseWechatChannelsWebhook()</c>（组合根末端）<c>Seal()</c> 封闭，且封闭先于管道挂载。
    /// </summary>
    /// <remarks>
    /// 「注册表已实现 Seal 但宿主接线漏调」不产生任何编译期 / 运行期信号（注册只发生在
    /// UseXxxCallback 内，无调用即空转）⇒ 只能以源码断言锁定。漏封闭的后果是运行期漏注册
    /// 处理器到线上首个回调才静默 unhandled（分布式故障），违背 F15「组装期 fail-fast」意图。
    /// </remarks>
    [Fact]
    public void CallbackRegistries_ShouldBeSealedAtCompositionRoot()
    {
        var source = File.ReadAllText(SourcePath(
            "Mud.Wechat.Channels.Callback", "WechatChannelsApplicationBuilderExtensions.cs"));

        foreach (var registry in new[]
                 {
                     "ChannelsCallbackHandlerRegistry",
                     "ChannelsCallbackInterceptorRegistry",
                     "ChannelsCallbackReplyRegistry",
                 })
        {
            // 断言取「注册表名 + >().Seal()」子串：对全限定与短名两种形态均成立（与其余三线同口径）。
            source.Should().Contain($"{registry}>().Seal()",
                $"UseWechatChannelsWebhook() 必须封闭 {registry}（F15①：运行期漏注册处理器须在组装期暴露，不得静默 unhandled）");
        }

        // 顺序即契约：封闭先于管道挂载（取末次 Seal() 代码位与管道挂载比较；
        // 管道挂载锚点取 "UseMiddleware<" —— 泛型实参仅存在于代码行，注释字样不得参与比较）。
        var lastSealIndex = source.LastIndexOf("Seal()", StringComparison.Ordinal);
        var middlewareIndex = source.IndexOf("UseMiddleware<", StringComparison.Ordinal);
        lastSealIndex.Should().BeGreaterThan(-1, "UseWechatChannelsWebhook() 必须存在 Seal() 封闭调用");
        lastSealIndex.Should().BeLessThan(middlewareIndex,
            "CB-CH-SEAL：封闭必须先于 UseMiddleware —— 管道构建期即完成封闭，迟封闭等于漏封闭");
    }

    /// <summary>解决方案根目录定位（与其余各线守卫同款判据）。</summary>
    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Mud.Wechat.slnx")))
        {
            directory = directory.Parent!;
        }

        return directory!.FullName;
    }

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(GetSolutionRoot(), $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f)!)
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
