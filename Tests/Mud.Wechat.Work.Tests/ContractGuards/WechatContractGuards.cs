// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 契约守卫测试（对齐 Feishu TokenMultiAppContractGuards 模式，详细设计 §14/M6）。
/// </summary>
/// <remarks>
/// F2 守卫拆分（审查报告 v1）：本类为 <c>partial</c>、按职责分文件承载
/// （QueryTokenInjection / Authorization / CredentialRedaction / InterfaceRoutes / DiBridge），
/// 断言方法逐字搬移、类名与测试 FQN 不变 —— G1~G4 与公共辅助留本文件。
/// </remarks>
public partial class WechatContractGuards
{
    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
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
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    /// <summary>
    /// 契约守卫 G1：全仓库 Mud.HttpUtils 单一版本（防混版 TypeLoadException，
    /// 对齐 Feishu「契约守卫锁定全仓库单一版本」）。
    /// </summary>
    [Fact]
    public void MudHttpUtils_PackageReference_ShouldBeSingleVersion()
    {
        var root = GetSolutionRoot();
        var csprojFiles = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        // .codeartsdoer 为本机工具临时目录（gitignored，含评测 harness 生成的临时 csproj），
                        // 契约面只约束仓库源码树 —— 与 verify-build.ps1 步骤 2 的排除口径一致，防本机假红。
                        && !f.Contains($"{Path.DirectorySeparatorChar}.codeartsdoer{Path.DirectorySeparatorChar}"))
            .ToList();

        csprojFiles.Should().NotBeEmpty();

        var versions = new HashSet<string>();
        var pattern = new System.Text.RegularExpressions.Regex(
            @"<PackageReference\s+Include=""(Mud\.HttpUtils(?:\.\w+)*)""\s+Version=""([^""]+)""",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        foreach (var file in csprojFiles)
        {
            var content = File.ReadAllText(file);
            foreach (System.Text.RegularExpressions.Match match in pattern.Matches(content))
            {
                versions.Add(match.Groups[2].Value);
            }
        }

        versions.Should().HaveCount(1, $"Mud.HttpUtils 全仓库必须锁定单一版本，实际：{string.Join(", ", versions)}");
    }

    /// <summary>
    /// 契约守卫 G2：配置 DTO 禁用 required（ConfigurationBinder 以 new T() 构造 → CS9035，
    /// 对齐 Feishu AOT-3）。
    /// </summary>
    [Fact]
    public void ConfigDtos_ShouldNotUseRequired()
    {
        var configTypes = new[] { typeof(WechatAppConfig) };
        foreach (var type in configTypes)
        {
            var requiredProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "RequiredMemberAttribute"))
                .ToList();

            requiredProps.Should().BeEmpty($"{type.Name} 不得声明 required 成员（配置绑定源生成器 CS9035）");
        }
    }

    /// <summary>
    /// 契约守卫 G3：WechatTokenTypes 常量采用 "Wechat." 前缀命名空间，
    /// 与通用 TokenTypes（"AccessToken"）隔离（详细设计 §4.2 / 关键设计决策）。
    /// </summary>
    [Fact]
    public void WechatTokenTypes_ShouldUseWechatPrefixedNamespace()
    {
        WechatTokenTypes.AccessToken.Should().Be("Wechat.AccessToken");
        WechatTokenTypes.ProviderAccessToken.Should().Be("Wechat.ProviderAccessToken");
        WechatTokenTypes.SuiteAccessToken.Should().Be("Wechat.SuiteAccessToken");

        // 与组件通用常量明确区隔。
        WechatTokenTypes.AccessToken.Should().NotBe(Mud.HttpUtils.TokenTypes.AccessToken);
    }

    /// <summary>
    /// 契约守卫 G4：令牌失效码集合与判定器集合一致（{40014,42001,42007,42009,42011}，
    /// 与《Mud.HttpUtils-企业微信SDK需要的改动.md》§3.1 保持一致）。
    /// </summary>
    [Fact]
    public void WechatErrorCodes_ShouldAlignWithDetectorCollection()
    {
        var expected = new[] { 40014, 42001, 42007, 42009, 42011 };
        var actual = new[]
        {
            WechatErrorCodes.InvalidAccessToken,
            WechatErrorCodes.ExpiredAccessToken,
            WechatErrorCodes.RelatedAccessTokenInvalid,
            WechatErrorCodes.InvalidSuiteAccessToken,
            WechatErrorCodes.InvalidProviderAccessToken,
        };

        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// 契约守卫 G12：四条线的应用管理器不得残留上游 <c>BC-30</c> 已<b>物理移除</b>的离线接缝
    /// （<c>GetWebApi</c> / <c>GetDefaultWebApi</c> / <c>RegisterSwitcherFactory</c> 及其私有异常工厂），
    /// 保证「每个公开接缝必须有真实消费点」不变量。
    /// </summary>
    /// <remarks>
    /// 上游 <c>Mud.HttpUtils</c> 3.0.8 随 <c>BC-30</c> 移除 <c>IAppManager&lt;T&gt;</c> 三成员，本仓四线实现体同批删除。
    /// 四个 <c>*AppManager</c> 分属四个程序集（且 <c>OpenPlatformAppManager</c> 为 <c>internal</c>、非
    /// <c>InternalsVisibleTo</c> 目标），反射形态需四个测试工程各写一份 ⇒ 取<b>源文本</b>形态：
    /// 按仓库根定位读四个文件、断言四个签名<b>零命中</b>（防回潮）。
    /// 作用域切换的合法入口恒为 DI 解析的 <c>I*AppContextSwitcher.UseAppScope</c> / <c>UseDefaultAppScope</c>。
    /// </remarks>
    [Fact]
    public void AppManagers_ShouldNotDeclareOrphanSeamsRemovedByBc30()
    {
        var files = new[]
        {
            SourcePath("Mud.Wechat.Work.Abstractions", "Authentication", "MultiApp", "WechatAppManager.cs"),
            SourcePath("Mud.Wechat.OfficialAccount.Abstractions", "Authentication", "MultiApp", "MpAppManager.cs"),
            SourcePath("Mud.Wechat.Channels.Abstractions", "Authentication", "MultiApp", "ChannelsAppManager.cs"),
            SourcePath("Mud.Wechat.OpenPlatform", "Authentication", "OpenPlatformAppManager.cs"),
        };

        var orphanSeams = new[] { "GetWebApi<", "GetDefaultWebApi<", "RegisterSwitcherFactory<", "NotSupportedSwitcher(" };

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            foreach (var signature in orphanSeams)
            {
                content.Should().NotContain(
                    signature,
                    $"上游 Mud.HttpUtils 3.0.8 随 BC-30 已物理移除该离线接缝（{signature}）⇒ {Path.GetFileName(file)} 不得复活；切换走 DI 解析的 I*AppContextSwitcher.UseAppScope/UseDefaultAppScope");
            }
        }
    }

}
