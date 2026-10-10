// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// P3（小程序长尾：<c>minishop</c> / 直播 / 云开发 / 第三方平台）的<b>处置裁决</b>守卫。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何要有这一组用例（而不是「什么都不做」）</b>：设计方案把 P3 列为<b>「按需」</b>，
/// 并在 §3.6 附近点名了四个入口（<c>/wxaapi/minishop/*</c>、<c>/wxaapi/broadcast/*</c>、
/// 云开发、第三方平台令牌链）。若不把「为什么不实现」写成<b>可执行</b>的裁决，后来者会照着
/// 那行清单「补」出能力 —— 而其中<b>至少一项已在官方下线</b>、另有一项被文档明令
/// <b>不得</b>进入小程序基础线。本守卫即是这四条裁决的机械化落地。
/// </para>
/// <para>
/// <b>本轮核验到的四条事实</b>（2026-10-09）：
/// </para>
/// <list type="number">
/// <item>
/// <b>小商店（minishop）已下线</b>：官方《交易组件·组件运营规范》原文
/// 「目前「小商店」已下线，请直接前往 <b>微信小店</b>」⇒ 按本仓纪律「官方声明下架即不建模」
/// <b>不实现</b>。<i>（注意区分：视频号「自定义交易组件」是另一套仍在册的能力，不在本项裁决内。）</i>
/// </item>
/// <item>
/// <b>小程序直播（<c>/wxaapi/broadcast/*</c>）停止接入</b>：公开报道（2024-04 起）称小程序直播插件
/// <b>已停止接入</b>；本轮<b>未取得</b>官方页面上的下线声明 ⇒ 按「未核验充分不实现」暂不建模。
/// <b>本项裁决是可解除的</b>（与其它三条「官方已声明」的裁决不同）：
/// 取得官方页面与最新准入状态后，可在同批解除。
/// </item>
/// <item>
/// <b>云开发不进小程序基础线</b>：官方云开发服务端调用依赖开放平台
/// <c>component_access_token</c>（令牌链与自建小程序线的 <c>access_token</c> <b>不是同一套</b>）。
/// </item>
/// <item>
/// <b>第三方平台令牌链不进小程序基础线</b>：设计方案明确「须<b>独立令牌管理器</b>，
/// <b>不塞进小程序基础线</b>（避免令牌串号）」，且列为「一期<b>可选</b>扩展」。
/// 本仓现状佐证：小程序线<b>没有</b>任何 component / authorizer 令牌类型（见 P3-R2）。
/// </item>
/// </list>
/// </remarks>
public class MiniProgramP3RulingsContractGuards
{
    /// <summary>
    /// P3-R1：P3 点名的小程序侧入口<b>均不得被建模</b>
    /// （小商店已下线；直播停止接入且未取得官方页面；云开发与第三方平台按下方 P3-R2 另判）。
    /// </summary>
    /// <remarks>
    /// 机械化判定走<b>源码扫描</b>而非只看路由：这些入口若被「补」进来，通常会先以
    /// 常量 / 注释 / 半成品路由的形式出现，只看已注册路由会漏掉早期形态。
    /// </remarks>
    [Fact]
    public void P3EntryPoints_ShouldStayUnmodeled()
    {
        var root = SourcePath("Mud.Wechat.MiniProgram");
        var sources = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        sources.Should().NotBeEmpty("防「扫描口径失效导致白名单真空」的静默空跑");

        var minishopOffenders = OffendingFiles(sources, "wxaapi/minishop");
        minishopOffenders.Should().BeEmpty(
            "官方原文「目前「小商店」已下线，请直接前往微信小店」⇒ 不得为已下线能力建模："
            + string.Join(", ", minishopOffenders));

        var broadcastOffenders = OffendingFiles(sources, "wxaapi/broadcast");
        broadcastOffenders.Should().BeEmpty(
            "小程序直播插件已停止接入，且本轮未取得官方页面上的能力声明 ⇒ 按「未核验充分不实现」暂不建模；"
            + "注意本裁决**可解除**（取得官方页面与准入状态后同批解除）：" + string.Join(", ", broadcastOffenders));
    }

    /// <summary>
    /// P3-R2：小程序线<b>不得</b>引入开放平台令牌链
    /// （<c>component_access_token</c> / <c>authorizer_access_token</c> / <c>component_verify_ticket</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>这条是本组里最硬的一条（防令牌串号）</b>：设计方案明文要求第三方平台令牌链
    /// 「须独立令牌管理器，<b>不塞进小程序基础线</b>」—— 因为小程序线<b>复用</b>公众号线的令牌域
    /// （同一 <c>MpTokenTypes.AccessToken</c>、同一单槽 <c>Resolve</c>），
    /// 一旦把 component 令牌塞进来，两种令牌会共用同一把槽 ⇒ 互相覆盖、
    /// <b>errCode 自愈静默失效</b>，且表现为「偶发 40001」这种最难定位的形态。
    /// </para>
    /// <para>
    /// 将来要做第三方平台，正确落点是一条<b>独立产品线 + 独立令牌管理器</b>
    /// （参照企微线已存在的多级令牌形态：<c>IWechatProviderTokenManager</c> /
    /// <c>IWechatSuiteTokenManager</c> / <c>WechatTokenRouting</c> 的归属单源做法），
    /// <b>不是</b>往小程序线里加接口。
    /// </para>
    /// </remarks>
    [Fact]
    public void MiniProgramLine_ShouldNotIntroduceComponentTokenChain()
    {
        // ① 源码层面：不得出现开放平台令牌字样。
        var root = SourcePath("Mud.Wechat.MiniProgram");
        var sources = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var token in new[] { "component_access_token", "authorizer_access_token", "component_verify_ticket" })
        {
            OffendingFiles(sources, token).Should().BeEmpty(
                $"小程序线不得引用开放平台令牌 {token}（设计方案红线：须独立令牌管理器，避免令牌串号）");
        }

        // ② 类型层面：小程序线不得出现 component / authorizer 令牌管理器实现。
        typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsPublic && !t.IsNested)
            .Select(static t => t.Name)
            .Where(static n => n.Contains("ComponentToken", StringComparison.OrdinalIgnoreCase)
                               || n.Contains("AuthorizerToken", StringComparison.OrdinalIgnoreCase))
            .Should().BeEmpty("第三方平台令牌管理器属**独立产品线**，不得落在小程序线内");

        // ③ 令牌注入仍必须是自建形态的单级 access_token（与 MP-X2 同口径的定点复述）。
        typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false).Any(static a => a.GetType().Name == "TokenAttribute"))
            .Select(static t => t.GetCustomAttributes(false).Single(static a => a.GetType().Name == "TokenAttribute"))
            .Select(static a => a.GetType().GetProperty("TokenType")!.GetValue(a))
            .Should().OnlyContain(t => Equals(t, MpTokenTypes.AccessToken),
                "小程序线所有带令牌接口都复用公众号 access_token，不得新增令牌类型");
    }

    // ---- helpers -------------------------------------------------------------

    /// <summary>
    /// 找出<b>代码行</b>里出现 <paramref name="needle"/> 的文件。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>只在非注释行里找，是刻意的</b>：本线多处文件以注释<b>声明边界</b>
    /// （例如「第三方平台模式下需 authorizer_access_token，本线不支持」）——
    /// 这类<b>文档化边界</b>必须保留，也<b>不应</b>被判为违规；真正的红线是把它写成<b>代码</b>
    /// （引用、常量、注入名）。故本辅助方法先剥掉 <c>//</c> / <c>///</c> / <c>*</c> / <c>/*</c> 开头的行。
    /// </para>
    /// <para>
    /// 本轮实测正是这条区分救了场：首版「整文件包含」的粗口径把
    /// <c>IWxaDataAnalysisService.cs</c> 的边界注释误判为违规而假红 ——
    /// <b>机械判据的粒度过粗，会把正确的东西判错</b>，这是守卫类代码最常见的自伤形态。
    /// </para>
    /// </remarks>
    private static string[] OffendingFiles(string[] sources, string needle)
        => sources
            .Where(f => File.ReadAllLines(f).Any(line => IsCodeLine(line)
                                                         && line.Contains(needle, StringComparison.OrdinalIgnoreCase)))
            .Select(static f => Path.GetFileName(f))
            .ToArray();

    private static bool IsCodeLine(string line)
    {
        var trimmed = line.TrimStart();
        return !(trimmed.StartsWith("//", StringComparison.Ordinal)
                 || trimmed.StartsWith("/*", StringComparison.Ordinal)
                 || trimmed.StartsWith("*", StringComparison.Ordinal));
    }

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
