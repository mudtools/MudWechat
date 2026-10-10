// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店线令牌归属守卫（设计方案 v1 P0-c / CH-T1~T3 + CH-V1）。
/// </summary>
/// <remarks>
/// <para>
/// 小店 AppID 与公众号 / 小程序 AppID <b>不互通</b> ⇒ 本线必须持有独立令牌类型
/// <see cref="ChannelsTokenTypes.AccessToken"/>（<c>"Wechat.Channels.AccessToken"</c>），不得复用
/// <c>MpTokenTypes.AccessToken</c>（复用会污染 <c>MpTokenManagerRegistry</c> 单槽 —— 其按
/// <c>Wechat.Mp.AccessToken</c> 键控，新键 <c>Resolve</c> 返回 <c>null</c> ⇒ errcode 令牌自愈<b>静默失效</b>，
/// 属最危险的一类漂移）。
/// </para>
/// <para>
/// 脚手架期本线主包<b>零接口</b>，故「全部 <c>[Token]</c> 接口恒用小店令牌类型」类断言此刻恒真；
/// 但<b>字面量契约</b>、<b>注册表键路由</b>、<b>程序集引用</b>与<b>未确认路由禁止声明</b>四类断言
/// 从今天起就真正生效 —— P1 落接口后前一类断言立即接管。
/// </para>
/// </remarks>
public class ChannelsTokenOwnerContractGuards
{
    /// <summary>
    /// CH-T1：令牌类型字面量契约 + 全部 <c>[Token]</c> 接口（P1 起）恒用
    /// <see cref="ChannelsTokenTypes.AccessToken"/>。
    /// </summary>
    /// <remarks>
    /// 字面量即契约：改键值等于改契约，必须同批修订本守卫与设计方案 v1 §3.2。
    /// 键值采用 <c>"Wechat.Channels."</c> 前缀命名空间，与企微 <c>Wechat.AccessToken</c> /
    /// 公众号 <c>Wechat.Mp.AccessToken</c> 在共享 <c>ITokenManagerRegistry</c> 中天然隔离。
    /// </remarks>
    [Fact]
    public void TokenTypes_ShouldBeChannelsIndependent()
    {
        ChannelsTokenTypes.AccessToken.Should().Be("Wechat.Channels.AccessToken",
            "小店令牌类型字面量是契约（设计方案 v1 §3.2），改值必须同批修订守卫与方案");

        // 隔离判定：不得与任一既有线的令牌键同值（否则共享注册表串号）；
        // 既有键值分别由各线自家守卫锁定（企微 WechatTokenTypes=…、公众号 MpTokenTypes=…），此处以字面量断言。
        ChannelsTokenTypes.AccessToken.Should().NotBe(MpTokenTypes.AccessToken, "不得复用公众号令牌键");
        ChannelsTokenTypes.AccessToken.Should().NotBe("Wechat.AccessToken", "不得复用企微令牌键");
        ChannelsTokenTypes.AccessToken.Should().NotBe("Wechat.Mp.AccessToken", "不得复用公众号令牌键字面量");

        // P1 起生效：主包全部 [Token] 接口必须恒用小店令牌类型。
        var asm = LoadProductLine("Mud.Wechat.Channels");
        var violating = asm.GetTypes()
            .Where(static t => t.IsInterface && t.GetCustomAttribute<TokenAttribute>() is { } attr
                               && !string.Equals(attr.TokenType, ChannelsTokenTypes.AccessToken, StringComparison.Ordinal))
            .Select(static t => $"{t.Name} (TokenType='{t.GetCustomAttribute<TokenAttribute>()!.TokenType}')")
            .ToArray();
        violating.Should().BeEmpty(
            "小店线接口必须恒用 ChannelsTokenTypes.AccessToken（CH-T1）：" + string.Join(", ", violating));
    }

    /// <summary>
    /// CH-T2：令牌注入契约 —— <c>[Token]</c> 接口（P1 起）必须为 Query 注入且参数名恒为
    /// <c>access_token</c>；同时锁定注册表键路由：小店注册表只认 <c>Wechat.Channels.AccessToken</c>，
    /// 对公众号 / 企微键返回 <c>null</c>（恢复链放弃重试而非取错令牌）。
    /// </summary>
    [Fact]
    public void TokenInjection_ShouldBeQueryAccessToken_AndRegistryDisjoint()
    {
        // TokenType → 官方 Query 参数名的唯一映射（设计方案 v1 §3.2：官方契约强制 Query 注入）。
        var expectedQueryName = "access_token";

        var asm = LoadProductLine("Mud.Wechat.Channels");
        var violation = asm.GetTypes()
            .Where(t => t.IsInterface && t.GetCustomAttribute<TokenAttribute>() is { } attr
                        && (attr.InjectionMode != TokenInjectionMode.Query
                            || !string.Equals(attr.Name, expectedQueryName, StringComparison.Ordinal)))
            .Select(static t =>
            {
                var attr = t.GetCustomAttribute<TokenAttribute>()!;
                return $"{t.Name} (InjectionMode={attr.InjectionMode}, Name='{attr.Name}')";
            })
            .ToArray();
        violation.Should().BeEmpty(
            "小店接口令牌必须 Query 注入且参数名为 'access_token'（CH-T2，官方契约）：" +
            string.Join(", ", violation));

        // 注册表键路由（今天起生效）：单槽注册表只解析小店令牌键。
        var manager = new Mock<IChannelsAccessTokenManager>();
        var registry = new ChannelsTokenManagerRegistry(manager.Object);

        registry.Resolve(ChannelsTokenTypes.AccessToken).Should().BeSameAs(manager.Object,
            "注册表必须解析小店令牌键（errcode 恢复链路依赖）");
        registry.Resolve(MpTokenTypes.AccessToken).Should().BeNull(
            "注册表不得解析公众号令牌键（否则 errcode 恢复会取到错误凭据）");
        registry.Resolve("Wechat.AccessToken").Should().BeNull(
            "注册表不得解析企微令牌键");
    }

    /// <summary>
    /// CH-T3：不得复用 <c>MpTokenTypes</c> —— 程序集引用级锁定 + 源码文本复核：
    /// 小店线任一程序集不得引用公众号 Abstractions 程序集，也不得出现公众号令牌键字符串。
    /// </summary>
    [Fact]
    public void ChannelsAssemblies_ShouldNotReferenceMpTokenTypes()
    {
        foreach (var assemblyName in new[]
                 {
                     "Mud.Wechat.Channels",
                     "Mud.Wechat.Channels.Abstractions",
                     "Mud.Wechat.Channels.DataModels",
                     "Mud.Wechat.Channels.Callback",
                 })
        {
            var asm = LoadProductLine(assemblyName);
            asm.GetReferencedAssemblies()
                .Select(static a => a.Name)
                .Should().NotContain("Mud.Wechat.OfficialAccount.Abstractions",
                    $"{assemblyName} 不得引用公众号令牌基座（MpTokenTypes 所在程序集）");
        }

        // 源码文本复核：防「注释 / 未编译代码」绕过程序集断言（议案：令牌键是常量，文本即契约）。
        var channelsRoot = SourceProjectDir("Mud.Wechat.Channels");
        Directory.EnumerateFiles(channelsRoot, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .SelectMany(static f => File.ReadLines(f))
            .Where(static line => line.Contains("MpTokenTypes", StringComparison.Ordinal)
                                  || line.Contains("\"Wechat.Mp.AccessToken\"", StringComparison.Ordinal))
            .Should().BeEmpty("小店线源码不得出现 MpTokenTypes / 公众号令牌键（CH-T3，防复用回潮）");
    }

    /// <summary>
    /// CH-V1：<c>/wxa/vip/*</c> 路由未确认令牌归属（设计方案 v1 §4.1 风险门），<b>不得声明</b>。
    /// </summary>
    /// <remarks>
    /// 该 6 个「小程序会员服务」端点（<c>wxa/vip/user/info/*</c>、<c>wxa/vip/user/list/get</c>、
    /// <c>wxa/vip/shop/list/get</c>）出现在小店文档但路径在 wxa 小程序域，令牌归属未核实 ⇒
    /// <b>默认暂缓落位</b>：核对结论三选一（A 小程序令牌 / B 小店令牌 / C 维持暂缓），
    /// 确认归属前任何 <c>/wxa/</c> 路由不得进入小店线接口面。
    /// </remarks>
    [Fact]
    public void WxaVipRoutes_ShouldNotBeDeclared_WhileOwnershipUnknown()
    {
        foreach (var assemblyName in new[]
                 {
                     "Mud.Wechat.Channels",
                     "Mud.Wechat.Channels.Abstractions",
                     "Mud.Wechat.Channels.Callback",
                 })
        {
            var asm = LoadProductLine(assemblyName);
            var wxaRoutes = CollectRequestUris(asm).Where(static r => r.StartsWith("/wxa/", StringComparison.Ordinal)).ToArray();
            wxaRoutes.Should().BeEmpty(
                $"{assemblyName} 不得声明 /wxa/vip/* 路由（CH-V1，设计方案 §4.1 未确认阻塞）：" +
                string.Join(", ", wxaRoutes));
        }
    }

    // ---- helpers ------------------------------------------------------------

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

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static Assembly LoadProductLine(string name)
    {
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == name)
            ?? Assembly.Load(name);
        asm.GetName().Name.Should().Be(name);
        return asm;
    }

    private static HashSet<string> CollectRequestUris(Assembly asm)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var type in asm.GetTypes())
        {
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic
                                                   | BindingFlags.Instance | BindingFlags.Static
                                                   | BindingFlags.DeclaredOnly))
            {
                foreach (var attr in member.GetCustomAttributes(false))
                {
                    var uri = attr.GetType().GetProperty("RequestUri")?.GetValue(attr) as string;
                    if (!string.IsNullOrWhiteSpace(uri))
                    {
                        set.Add(uri!);
                    }
                }
            }

            foreach (var attr in type.GetCustomAttributes(false))
            {
                var uri = attr.GetType().GetProperty("RequestUri")?.GetValue(attr) as string;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    set.Add(uri!);
                }
            }
        }
        return set;
    }
}