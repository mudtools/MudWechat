// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Tests.ContractGuards;

/// <summary>
/// 公用层契约守卫（依赖单向性 + 下沉面一致性）。
/// </summary>
public class WechatAbstractionsContractGuards
{
    /// <summary>定位仓库根（锚定 <c>Mud.Wechat.slnx</c> 哨兵文件，避免按 CWD 层级硬编码）。</summary>
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

    private static string ReadCsproj(string relativePath)
    {
        var path = SourcePath(relativePath.Split('/'));
        File.Exists(path).Should().BeTrue($"守卫依赖的工程文件必须存在：{relativePath}");
        return File.ReadAllText(path);
    }

    /// <summary>
    /// 归一化路径分隔符：脚本内的字面量可能用 <c>\</c>（如 ApplyTokenOwnerKeys.ps1）或 <c>/</c>，
    /// 而 CI 为 ubuntu + windows 双 OS 矩阵 —— 不归一化会让守卫在 Linux 上判定失败。
    /// </summary>
    private static string NormalizeSeparators(string path) =>
        path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

    /// <summary>
    /// AB-G1：公用层零产品线依赖（依赖单向性：产品线 → 公用层，公用层为叶子）。
    /// </summary>
    /// <remarks>
    /// 反向依赖会让「横切层」被迫引用某个产品线的具象（AppType / 套件 / 回调信封），
    /// 其他产品线引用时被迫看到无关语义 —— 这正是 v1 方案拒绝「整体改名 Work.Abstractions」的原因。
    /// </remarks>
    [Fact]
    public void Abstractions_ShouldNotReferenceAnyProductLine()
    {
        var content = ReadCsproj("Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj");

        foreach (var forbidden in new[]
                 {
                     "Mud.Wechat.Work",
                     "Mud.Wechat.OfficialAccount",
                     "Mud.Wechat.MiniProgram",
                     "Mud.Wechat.Pay",
                     "Mud.Wechat.Redis",
                 })
        {
            content.Should().NotContain($"{forbidden}.csproj",
                "公用层不得反向引用任何产品线程序集（含 Redis 存储实现包）");
        }
    }

    /// <summary>
    /// AB-G2：各产品线 DataModels 单向引用公用层（响应基底实现判错契约的依赖方向）。
    /// </summary>
    [Fact]
    public void DataModels_ShouldReferenceAbstractionsOnly()
    {
        var dataModelsProjects = new[]
        {
            "Mud.Wechat.Work.DataModels/Mud.Wechat.Work.DataModels.csproj",
            "Mud.Wechat.OfficialAccount.DataModels/Mud.Wechat.OfficialAccount.DataModels.csproj",
            "Mud.Wechat.MiniProgram.DataModels/Mud.Wechat.MiniProgram.DataModels.csproj",
            "Mud.Wechat.Pay.DataModels/Mud.Wechat.Pay.DataModels.csproj",
            // 微信小店/视频号产品线（设计方案 v1 §3.1）：DTO 面同样只向公用层取判错契约。
            "Mud.Wechat.Channels.DataModels/Mud.Wechat.Channels.DataModels.csproj",
        };

        var sources = dataModelsProjects
            .Select(ReadCsproj)
            .ToList();

        foreach (var content in sources)
        {
            content.Should().Contain("Mud.Wechat.Abstractions.csproj",
                "DataModels 需实现公用层判错契约 IWechatApiResponse");
        }

        // 各产品线 DataModels 之间不得互相引用（产品线隔离）。
        for (var i = 0; i < dataModelsProjects.Length; i++)
        {
            foreach (var other in dataModelsProjects.Where((_, j) => j != i))
            {
                var otherRoot = Path.GetFileName(other).Replace(".csproj", string.Empty, StringComparison.Ordinal);
                sources[i].Should().NotContain(otherRoot,
                    $"DataModels 之间不得互相引用（产品线隔离）：{dataModelsProjects[i]} 不得引用 {otherRoot}");
            }
        }
    }

    /// <summary>
    /// AB-G3：下游产品线工程必须反向引用公用层（防「守卫静默空跑」——若某产品线漏引，
    /// 其类型解析会落到陈旧的传输依赖上，行为不可预期）。
    /// </summary>
    [Fact]
    public void ProductLineProjects_ShouldReferenceAbstractions()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Work.Abstractions/Mud.Wechat.Work.Abstractions.csproj",
                     "Mud.Wechat.OfficialAccount.Abstractions/Mud.Wechat.OfficialAccount.Abstractions.csproj",
                     "Mud.Wechat.MiniProgram.Abstractions/Mud.Wechat.MiniProgram.Abstractions.csproj",
                     "Mud.Wechat.Pay.Abstractions/Mud.Wechat.Pay.Abstractions.csproj",
                     "Mud.Wechat.OpenPlatform.Abstractions/Mud.Wechat.OpenPlatform.Abstractions.csproj",
                     "Mud.Wechat.Channels.Abstractions/Mud.Wechat.Channels.Abstractions.csproj",
                 })
        {
            ReadCsproj(project).Should().Contain("Mud.Wechat.Abstractions.csproj", $"{project} 必须引用公用层");
        }
    }

    /// <summary>
    /// AB-G4：SSRF 白名单单一来源——白名单登记（无论整体替换 <c>ConfigureAllowedDomains</c> 还是
    /// 增量并集 <c>AddAllowedDomain</c>，W-17 后采用后者）只允许在公用层登记点被调用，
    /// 各产品线不得各自调用（增量并集虽幂等，登记点分散会让「白名单包含哪些域名」失去单一事实源）。
    /// </summary>
    [Fact]
    public void AllowedDomainRegistration_ShouldBeCentralized()
    {
        // ① 产品线不得出现任一登记入口（ConfigureAllowedDomains / AddAllowedDomain）。
        foreach (var lineRoot in ProductLineSourceRoots())
        {
            var hits = Directory.EnumerateFiles(lineRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                            && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                .Where(f =>
                {
                    var text = File.ReadAllText(f);
                    return text.Contains("ConfigureAllowedDomains", StringComparison.Ordinal)
                           || text.Contains("AddAllowedDomain", StringComparison.Ordinal);
                })
                .Select(f => Path.GetFileName(f))
                .ToList();

            hits.Should().BeEmpty(
                $"SSRF 白名单登记只能出现在公用层单点（WechatTokenRecoveryRegistration）；" +
                $"实际命中：{string.Join(", ", hits)}");
        }

        // ② 公用层内 AddAllowedDomain 只在 WechatTokenRecoveryRegistration 一处（W-17 语义矩阵的代码面锚点）。
        var coreRoot = SourceProjectDir("Mud.Wechat.Abstractions");
        var addAllowedHits = Directory.EnumerateFiles(coreRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(f =>
            {
                var codeOnly = System.Text.RegularExpressions.Regex.Replace(
                    File.ReadAllText(f), @"//.*?$|/\*.*?\*/|///.*?$", string.Empty,
                    System.Text.RegularExpressions.RegexOptions.Multiline
                    | System.Text.RegularExpressions.RegexOptions.Singleline);
                return codeOnly.Contains("AddAllowedDomain", StringComparison.Ordinal);
            })
            .Select(Path.GetFileName)
            .ToList();

        addAllowedHits.Should().ContainSingle("AddAllowedDomain（增量并集）只能在 WechatTokenRecoveryRegistration 出现一次")
            .Which.Should().Be("WechatTokenRecoveryRegistration.cs");
    }

    /// <summary>
    /// W-44(a)：授权器注册面——<c>AllowAllAppAccessAuthorizer</c>（无条件放行）在 <c>Src</c> 归零，
    /// 每条带切换能力的线（Work / MP / Channels）恰好在各自扩展内注册一次 <c>IAppAccessAuthorizer</c>
    /// （W-21：appKey 白名单，与注册表同源）。开放平台线有意不注册（形态差异见其扩展 XML 注释）。
    /// </summary>
    [Fact]
    public void AppAccessAuthorizer_Registration_ShouldBeAllowListPerSwitchableLine()
    {
        // ① 无条件放行实现不得再出现在生产代码（含注释——防止"顺手照抄"回潮）。
        var srcRoot = Path.Combine(GetSolutionRoot(), "Src");
        var allowAllHits = Directory.EnumerateFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(f => File.ReadAllText(f).Contains("AllowAllAppAccessAuthorizer", StringComparison.Ordinal))
            .Select(f => Path.GetFileName(f))
            .ToList();
        allowAllHits.Should().BeEmpty(
            "W-21：AllowAllAppAccessAuthorizer（无条件放行）已被 AppKeyAllowListAuthorizer（appKey 白名单）替代，" +
            $"生产代码不得回潮。实际命中：{string.Join(", ", allowAllHits)}");

        // ② 三线各恰好多注册一次 IAppAccessAuthorizer，且形态为白名单（与注册表同源）。
        foreach (var (project, extensionFile, switcherTypeName) in new[]
                 {
                     ("Mud.Wechat.Work.Abstractions", "Extensions/WechatWorkMultiAppExtensions.cs", "IWechatAppContextSwitcher"),
                     ("Mud.Wechat.OfficialAccount.Abstractions", "Extensions/MpMultiAppExtensions.cs", "IMpAppContextSwitcher"),
                     ("Mud.Wechat.Channels.Abstractions", "Extensions/ChannelsMultiAppExtensions.cs", "IChannelsAppContextSwitcher"),
                 })
        {
            var source = File.ReadAllText(Path.Combine(
                SourceProjectDir(project), extensionFile.Replace('/', Path.DirectorySeparatorChar)));
            var count = System.Text.RegularExpressions.Regex.Matches(
                source, @"TryAddSingleton<IAppAccessAuthorizer>").Count;
            count.Should().Be(1, $"{project} 恰好注册一次 IAppAccessAuthorizer");
            source.Should().Contain("AppKeyAllowListAuthorizer(",
                $"{project} 的授权器必须为 appKey 白名单形态（与注册表同源）");
            source.Should().Contain(switcherTypeName,
                $"{project} 扩展必须装配带切换能力的上下文（授权面才有意义）");
        }
    }

    /// <summary>
    /// W-44(b)：<c>TokenRecoveryOptions</c> 配置节名锁定 —— 三线绑定/变更令牌均以
    /// <see cref="TokenRecoveryOptions.SectionName"/> 为节名锚点，常量漂移会静默改变宿主配置契约。
    /// </summary>
    [Fact]
    public void TokenRecoveryOptions_SectionName_ShouldRemainStable()
    {
        TokenRecoveryOptions.SectionName.Should().Be("MudHttpTokenRecovery",
            "节名是宿主 appsettings 的公开契约（三线共用同名节，多线共存时变更令牌只需一份）");
    }

    /// <summary>
    /// AB-G5：令牌失效判定器登记单一来源——产品线只能「注册子判定器」，
    /// 不得直接写 `TokenRecoveryOptions.TokenInvalidationDetector`（单槽属性，后写者覆盖前者）。
    /// </summary>
    [Fact]
    public void TokenInvalidationDetector_ShouldNotBeAssignedByProductLines()
    {
        foreach (var lineRoot in ProductLineSourceRoots())
        {
            var hits = Directory.EnumerateFiles(lineRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                            && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                .Where(f => File.ReadAllText(f).Contains("TokenInvalidationDetector =", StringComparison.Ordinal))
                .Select(f => Path.GetFileName(f))
                .ToList();

            hits.Should().BeEmpty(
                $"选项属性为单槽，只能由公用层组合器写入；产品线须注册子判定器（ITokenInvalidationDetector）。" +
                $"实际命中：{string.Join(", ", hits)}");
        }
    }

    /// <summary>
    /// AB-G7：<b>打包契约</b>守卫 —— 三个回调宿主包<b>必须产出 nupkg</b>（分析器随包内嵌），
    /// 而生成器 / 分析器工程必须 <c>IsPackable=false</c>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 起因：回调 <b>宿主包</b>的 csproj 注释曾写「工程 IsPackable=false，不新增 nupkg」，
    /// 与实际行为（各产出 1 个 nupkg，且 CI 期望数已含它们）<b>矛盾</b>。
    /// 该注释会诱导后来者「按注释修正」为 <c>IsPackable=false</c> ⇒ 既让 CI 计数 fail-closed 变红，
    /// 又使 <c>Mud.Wechat.Callback.Analyzers</c> 失去随包下发渠道（诊断能力对消费者失效）。
    /// </para>
    /// <para>
    /// 实测口径（<c>dotnet pack Mud.Wechat.slnx -c Release</c>）：恰 <b>27</b> 个 nupkg（与 CI 制品数量
    /// 守卫、AB-G6 同一口径），其中四个回调包内均含 <c>analyzers/dotnet/cs/Mud.Wechat.Callback.Analyzers.dll</c>。
    /// </para>
    /// <para>
    /// 2026-10 源码归类后曾实测只有 <b>17</b> 个：三个回调包 pack 失败（下述 ①-b 的相对路径仍指向旧的
    /// <c>Src/</c>），而 CI 的 <c>-ne 18</c> 断言同时是错的（少列 OpenPlatform 两包）——
    /// 双重失真使「构建全绿 + CI 口径」都掩盖了分析器已停止下发这一事实，故本守卫补 ①-b。
    /// </para>
    /// </remarks>
    [Fact]
    public void CallbackPackages_ShouldShipAndEmbedAnalyzer()
    {
        var root = GetSolutionRoot();

        // ① 宿主包：不得声明 IsPackable=false（否则包不再发布，诊断能力对消费者失效）。
        var hostPackages = new[]
        {
            "Mud.Wechat.Work.Callback/Mud.Wechat.Work.Callback.csproj",
            "Mud.Wechat.OfficialAccount.Callback/Mud.Wechat.OfficialAccount.Callback.csproj",
            "Mud.Wechat.Pay.Callback/Mud.Wechat.Pay.Callback.csproj",
            // 微信小店/视频号（channels 生态）产品线（设计方案 v1 §3.5）：回调包随包下发分析器。
            "Mud.Wechat.Channels.Callback/Mud.Wechat.Channels.Callback.csproj",
        };
        // 各包内嵌 Include 的完整字面量（P2-2 一致性断言用）。
        var includeLiterals = new List<string>();

        foreach (var hostPackage in hostPackages)
        {
            var projectPath = SourcePath(hostPackage.Split('/'));
            var source = File.ReadAllText(projectPath);
            var withoutComments = StripXmlComments(source);

            withoutComments.Should().NotContain("<IsPackable>",
                $"{hostPackage} 是回调宿主包，必须产出 nupkg（分析器随包内嵌）；" +
                "若确实要停止发布，须同步修改 CI 制品数量守卫与本文档说明");

            withoutComments.Should().Contain("analyzers/dotnet/cs",
                $"{hostPackage} 必须把 Mud.Wechat.Callback.Analyzers 内嵌到 analyzers/dotnet/cs");

            // ①-b 内嵌路径必须**当前可解析**。该 None Include 是字面相对路径而非 ProjectReference，
            // 目录归类移动后不会有任何编译期提示，只在 `dotnet pack` 时报「Could not find a part of the path」
            // ——2026-10 源码归类即把 Analyzers 从 Src/ 移到 Src/Core/ 而使三个回调包静默失去下发渠道。
            var includeMatch = Regex.Match(withoutComments,
                @"Include=""([^""\\]*(?:\\[^""\\]*)*?Mud\.Wechat\.Callback\.Analyzers)\\bin",
                RegexOptions.IgnoreCase);
            includeMatch.Success.Should().BeTrue(
                $"{hostPackage} 的内嵌分析器 Include 路径必须指向 Mud.Wechat.Callback.Analyzers 目录（bin 之前）");

            var relativeDir = includeMatch.Groups[1].Value
                .Replace("$(MSBuildThisFileDirectory)", string.Empty, StringComparison.Ordinal);
            // csproj 字面量用 `\`，MSBuild 在 unix 会归一为 `/`（构建/打包照常解析）；
            // 守卫须按同一口径归一 —— 否则 Linux 上整段被当作单个目录名，恒不存在（CI ubuntu 失败）。
            var resolvedDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, NormalizeSeparators(relativeDir)));
            Directory.Exists(resolvedDir).Should().BeTrue(
                $"{hostPackage} 内嵌分析器路径解析后必须存在：{relativeDir} ⇒ {resolvedDir}；" +
                "源码目录归类移动时须同批改此相对路径");

            includeLiterals.Add(includeMatch.Groups[0].Value);
        }

        // ①-c 内嵌 Include 字面量四包逐字符一致（P2-2）：同一 $(Configuration)、同一 netstandard2.0 目录。
        // 分叉即「同一批 pack 内嵌出不同版本/形态的分析器资产」——与单次全量 pack 必然同位取用的新鲜度前提冲突。
        includeLiterals.Should().HaveCount(hostPackages.Length, "每个回调宿主包须解析到一处内嵌 Include");
        includeLiterals.Distinct(StringComparer.Ordinal).Should().HaveCount(1,
            "AB-G7：四包内嵌 Include 路径字面量必须逐字符一致（当前：" +
            string.Join(" | ", includeLiterals.Distinct(StringComparer.Ordinal)) + "）——分叉即分析器资产分叉");

        // ② 工具链工程：生成器 / 分析器不得独立打包（否则多出无消费者的空包）。
        foreach (var toolProject in new[]
                 {
                     "Mud.Wechat.Callback.Generator/Mud.Wechat.Callback.Generator.csproj",
                     "Mud.Wechat.Callback.Analyzers/Mud.Wechat.Callback.Analyzers.csproj",
                 })
        {
            var source = File.ReadAllText(SourcePath(toolProject.Split('/')));
            StripXmlComments(source).Should().Contain("<IsPackable>false</IsPackable>",
                $"{toolProject} 为随包下发的工具链，必须 IsPackable=false");
        }

        // ③ 制品数量守卫的口径：CI 不再持有包名清单，改为按 IsPackable 现场推导可打包集（与 AB-G6 同口径）。
        //    「三个回调包仍在可打包集内」由 ①（宿主包不得声明 IsPackable=false，否则推导会同步少包 = 静默停发）保证，
        //    故此处无须再断言 CI 文本含包名 —— 断言口径本身即可。
        var ci = File.ReadAllText(Path.Combine(root, ".github", "workflows", "dotnet-publish.yml"));
        ci.Should().Contain("<IsPackable>false",
            "CI 制品数量守卫必须按 IsPackable 现场推导可打包集，不得硬编码包名清单");
    }

    /// <summary>
    /// 从 slnx 现场推导 <c>/src/</c> 下全部工程目录（F1 接入口收敛：新增产品线只登记 slnx，
    /// 守卫侧扫描根零手工维护）。slnx 为标准 XML，<c>XmlDocument</c> 直接可解。
    /// </summary>
    private static string[] SlnxSourceProjectDirs()
    {
        var root = GetSolutionRoot();
        var doc = new System.Xml.XmlDocument();
        doc.Load(Path.Combine(root, "Mud.Wechat.slnx"));

        var dirs = new List<string>();
        foreach (System.Xml.XmlNode folder in doc.SelectNodes("/Solution/Folder")!)
        {
            var name = folder.Attributes?["Name"]?.Value ?? string.Empty;
            if (!name.StartsWith("/src/", StringComparison.Ordinal))
            {
                // 仅 /src/*：/tests/ 与 /Demos/ 的工程不得进入产品线扫描面
                //（守卫源文件自身含被扫描字面量，纳入即让守卫扫到自己）。
                continue;
            }

            foreach (System.Xml.XmlNode project in folder.SelectNodes("Project")!)
            {
                var path = project.Attributes?["Path"]?.Value;
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var dir = Path.GetDirectoryName(Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar)));
                if (!string.IsNullOrEmpty(dir))
                {
                    dirs.Add(dir);
                }
            }
        }

        dirs.Should().NotBeEmpty("slnx /src/ 推导不得为空（解析失效即守卫扫描面失真）");
        return dirs.Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>
    /// 受 AB-G4 / AB-G5 扫描覆盖的产品线源码根：<b>由 slnx /src/ 现场推导</b>（F1，新增产品线零手工登记），
    /// 仅排除 <c>/src/Core/</c>——Core 是「公用层单点登记」的合法居所
    /// （<c>WechatTokenRecoveryRegistration</c> 的 <c>AddAllowedDomain</c> / 判定器组合器写单槽），
    /// 纳入扫描会让守卫扫到 SDK 自身的合法调用点。
    /// </summary>
    private static string[] ProductLineSourceRoots()
    {
        return SlnxSourceProjectDirs()
            .Where(static p => !p.Contains($"{Path.DirectorySeparatorChar}Core{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();
    }

    /// <summary>
    /// AB-G6：门禁脚本同批演进（防「新增产品线成为门禁盲区」）。
    /// </summary>
    /// <remarks>
    /// 五项均为「缺一即 CI/门禁失效」的硬约束：审计脚本白名单与搜索范围、DTO 标注脚本根命名空间、
    /// 解决方案工程清单、CI 制品数量、本地打包脚本（pack.bat / publish.bat）的包清单与期望数。**新增产品线时本守卫是唯一会自动变红的门禁接入口。**
    /// </remarks>
    [Fact]
    public void GovernanceScripts_ShouldCoverAllProductLines()
    {
        var root = GetSolutionRoot();

        var audit = File.ReadAllText(Path.Combine(root, "scripts", "audit-config-keys.ps1"));
        audit.Should().Contain("Mud.Wechat.Abstractions/Configuration/WechatAppConfigBase.cs",
            "配置基座属性上移后必须仍在审计白名单内");
        audit.Should().Contain("Mud.Wechat.OfficialAccount.Abstractions/Configuration/MpAppConfig.cs",
            "公众号配置必须纳入审计");
        audit.Should().Contain("Mud.Wechat.OfficialAccount.Callback/MpCallbackOptions.cs",
            "公众号回调配置面必须纳入审计白名单（否则配置属性无消费点也无人发现）");
        // 微信支付：商户配置面与回调配置面（设计方案 v2 §5.3 耦合点表）。
        // **双向不变式**：配置 DTO 文件存在 ⇔ 已登记进 $configFiles。
        //   · 文件已建却未登记 ⇒ 审计脚本扫不到它 ⇒ 该 DTO 的配置属性完全无人核查（门禁盲区）；
        //   · 已登记但文件不存在 ⇒ 审计脚本对缺失文件 fail-closed 硬错误（本轮实测踩过）。
        // 因此「先登记占位、后建文件」与「先建文件、后登记」两个方向都被本守卫拦下；
        // P0-b/P0-c 落这两个 DTO 时**创建与登记必须同批**（AGENTS §2 同批纪律）。
        foreach (var payConfigDto in new[]
                 {
                     "Mud.Wechat.Pay.Abstractions/Configuration/WechatPayMerchantConfig.cs",
                     "Mud.Wechat.Pay.Callback/WechatPayCallbackOptions.cs",
                 })
        {
            var exists = File.Exists(SourcePath(payConfigDto.Split('/')));
            var registered = audit.Contains(payConfigDto);
            registered.Should().Be(exists,
                $"支付线配置 DTO「{payConfigDto}」的文件存在性与 $configFiles 登记必须同批（存在={exists}，登记={registered}）");
        }
        // F1：消费点搜索根由 slnx /src/ 现场推导（新增产品线手工注册点收敛到 slnx 一处，
        // 字面根清单的逐线断言随之废除 —— OpenPlatform.DataModels 拆包后未补登记即为其漂移实证）。
        // ① 推导逻辑存在：入口函数名锁定，字面根清单（$searchRoots = @(）不得复活。
        audit.Should().Contain("function Get-SearchRootsFromSlnx",
            "audit 的消费点搜索根必须由 slnx /src/ 推导（F1 接入口收敛）");
        audit.Should().NotContain("$searchRoots = @(",
            "字面搜索根清单不得复活 —— 清单与工程目录各自成文正是 F1 要消除的漂移源");
        // ② 活性一致：按同款逻辑从 slnx 推导 /src/ 工程名，历史字面清单（26 根）覆盖的每个根
        //    必须仍被推导覆盖（推导为超集：新增的 DataModels / 工具工程目录无配置消费引用，安全）。
        var slnxSourceRootNames = SlnxSourceProjectDirs()
            .Select(static p => Path.GetFileName(p))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var heritageRoot in new[]
                 {
                     "Mud.Wechat.Abstractions", "Mud.Wechat.Redis", "Mud.Wechat.OpenTelemetry",
                     "Mud.Wechat.Work", "Mud.Wechat.Work.Abstractions", "Mud.Wechat.Work.Callback",
                     "Mud.Wechat.OfficialAccount", "Mud.Wechat.OfficialAccount.Abstractions",
                     "Mud.Wechat.OfficialAccount.DataModels", "Mud.Wechat.OfficialAccount.Callback",
                     "Mud.Wechat.MiniProgram", "Mud.Wechat.MiniProgram.Abstractions", "Mud.Wechat.MiniProgram.DataModels",
                     "Mud.Wechat.Pay", "Mud.Wechat.Pay.Abstractions", "Mud.Wechat.Pay.DataModels", "Mud.Wechat.Pay.Callback",
                     "Mud.Wechat.OpenPlatform", "Mud.Wechat.OpenPlatform.Abstractions",
                     "Mud.Wechat.Channels", "Mud.Wechat.Channels.Abstractions",
                     "Mud.Wechat.Channels.DataModels", "Mud.Wechat.Channels.Callback",
                     "Mud.Wechat.Ads", "Mud.Wechat.Ads.Abstractions", "Mud.Wechat.Ads.DataModels",
                 })
        {
            slnxSourceRootNames.Should().Contain(heritageRoot,
                $"历史字面搜索根 {heritageRoot} 必须仍被 slnx /src/ 推导覆盖（活性一致；缺项即 slnx 漏项或推导失效）");
        }        // 腾讯广告配置 DTO 与支付线同一双向不变式：文件存在 ⇔ 已登记进 $configFiles
        // （2026-10-10 补登记 —— 此前只断言了搜索根、漏了 DTO 本身，AdsAppConfig 曾整体绕过审计）。
        var adsConfigDto = "Mud.Wechat.Ads.Abstractions/Configuration/AdsAppConfig.cs";
        var adsDtoExists = File.Exists(SourcePath(adsConfigDto.Split('/')));
        audit.Contains(adsConfigDto).Should().Be(adsDtoExists,
            $"广告线配置 DTO「{adsConfigDto}」的文件存在性与 $configFiles 登记必须同批（存在={adsDtoExists}，登记={audit.Contains(adsConfigDto)}）");

        var annotate = File.ReadAllText(Path.Combine(root, "scripts", "AddHttpJsonSerializable.ps1"));
        annotate.Should().Contain("$RootNamespace", "根命名空间必须参数化，否则非默认产品线根级 DTO 分组错误");

        // ① 脚本默认路径必须**当前可解析**：2026-10 源码归类后源工程迁入 Src/<Area>/<ProjectName>，
        //    两个脚本的默认目标一度仍指向仓库根旧路径 ⇒ AGENTS §2 记载的标准 DTO 流程不带参数即失败
        //    （GenerateJsonContext 被 Test-Path 挡下、AddHttpJsonSerializable 直接 throw）。本断言防其再漂移。
        var gen = File.ReadAllText(Path.Combine(root, "scripts", "GenerateJsonContext.ps1"));

        var defaultTarget = Regex.Match(gen, @"\[string\]\$TargetProject = ""([^""]+)""").Groups[1].Value;
        defaultTarget.Should().NotBeNullOrEmpty("GenerateJsonContext.ps1 必须声明 $TargetProject 默认值");
        File.Exists(Path.Combine(root, defaultTarget.Replace('/', Path.DirectorySeparatorChar)))
            .Should().BeTrue($"$TargetProject 默认值必须可解析（源码归类后须指向 Src/<Area>/…）：{defaultTarget}");

        var defaultOutputDir = Regex.Match(gen, @"\[string\]\$OutputDir = ""([^""]+)""").Groups[1].Value;
        defaultOutputDir.Should().NotBeNullOrEmpty("GenerateJsonContext.ps1 必须声明 $OutputDir 默认值");
        Directory.Exists(Path.Combine(root, defaultOutputDir.Replace('/', Path.DirectorySeparatorChar)))
            .Should().BeTrue($"$OutputDir 默认值必须可解析：{defaultOutputDir}");

        // 同一类漂移的其余落点：凡以 `Join-Path $RepoRoot '<相对路径>'` 声明默认目录的脚本，均须可解析。
        // （ApplyTokenOwnerKeys.ps1 亦曾停留在仓库根旧路径 —— 2026-10 源码归类的同类残留。）
        foreach (var scriptName in new[] { "AddHttpJsonSerializable.ps1", "ApplyTokenOwnerKeys.ps1" })
        {
            var scriptText = File.ReadAllText(Path.Combine(root, "scripts", scriptName));
            var relativeDir = Regex.Match(scriptText, @"Join-Path \$RepoRoot '([^']+)'").Groups[1].Value;
            relativeDir.Should().NotBeNullOrEmpty($"{scriptName} 必须声明默认目录（Join-Path $RepoRoot '…'）");
            Directory.Exists(Path.Combine(root, NormalizeSeparators(relativeDir)))
                .Should().BeTrue($"{scriptName} 的默认目录必须可解析（源码归类后须指向 Src/<Area>/…）：{relativeDir}");
        }

        // ② 脚手架工具版本须与全仓 Mud.HttpUtils 版本 lockstep（脚本注释即如此声称，G1 亦锁全仓单一版本）：
        //    实际曾停留在 3.0.2 而全仓已 3.0.3 ⇒ 属静默漂移（脚本不与任何门禁比对版本），故在此断言。
        var toolVersion = Regex.Match(gen, @"\[string\]\$ToolVersion = ""([^""]+)""").Groups[1].Value;
        toolVersion.Should().NotBeNullOrEmpty("GenerateJsonContext.ps1 必须声明 $ToolVersion 默认值");

        var componentVersions = Directory
            .EnumerateFiles(Path.Combine(root, "Src"), "*.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .SelectMany(static f => File.ReadAllLines(f))
            .Where(static line => line.Contains("Include=\"Mud.HttpUtils\"", StringComparison.Ordinal))
            .Select(static line => Regex.Match(line, @"Version=""([^""]+)""").Groups[1].Value)
            .Where(static v => v.Length > 0)
            .Distinct()
            .ToArray();

        componentVersions.Should().HaveCount(1, "全仓 Mud.HttpUtils 必须单一版本（与 G1 同口径）");
        toolVersion.Should().Be(componentVersions[0],
            "GenerateJsonContext.ps1 的 $ToolVersion 必须与全仓 Mud.HttpUtils 版本 lockstep（否则脚手架生成物与编译期 Generator 版本错配）");

        var slnx = File.ReadAllText(Path.Combine(root, "Mud.Wechat.slnx"));
        foreach (var project in new[]
                 {
                     // 2026-10 源码归类后，源工程位于 Src/<Area>/<ProjectName>（下同）。
                     "Src/Core/Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj",
                     "Src/OfficialAccount/Mud.Wechat.OfficialAccount/Mud.Wechat.OfficialAccount.csproj",
                     "Src/OfficialAccount/Mud.Wechat.OfficialAccount.Abstractions/Mud.Wechat.OfficialAccount.Abstractions.csproj",
                     "Src/OfficialAccount/Mud.Wechat.OfficialAccount.DataModels/Mud.Wechat.OfficialAccount.DataModels.csproj",
                     "Src/OfficialAccount/Mud.Wechat.OfficialAccount.Callback/Mud.Wechat.OfficialAccount.Callback.csproj",
                     "Src/Core/Mud.Wechat.Callback.Generator/Mud.Wechat.Callback.Generator.csproj",
                     "Src/Core/Mud.Wechat.Callback.Analyzers/Mud.Wechat.Callback.Analyzers.csproj",
                     // 可观测性装配包（1 源 + 1 测试）：**跨线共享**（单根源 + 产品线通配 Meter），
                     // 不属任何产品线，故落 Src/Core（2026-10 由 Src/OpenPlatform 迁入；仅路径变，
                     // 包标识 / 命名空间 / TFM 均不变，对消费者零影响）。
                     "Src/Core/Mud.Wechat.OpenTelemetry/Mud.Wechat.OpenTelemetry.csproj",
                     "Tests/Mud.Wechat.Abstractions.Tests/Mud.Wechat.Abstractions.Tests.csproj",
                     "Tests/Mud.Wechat.OfficialAccount.Tests/Mud.Wechat.OfficialAccount.Tests.csproj",
                     "Tests/Mud.Wechat.OfficialAccount.Callback.Tests/Mud.Wechat.OfficialAccount.Callback.Tests.csproj",
                     // 微信小程序产品线（3 源 + 1 测试）。
                     "Src/MiniProgram/Mud.Wechat.MiniProgram/Mud.Wechat.MiniProgram.csproj",
                     "Src/MiniProgram/Mud.Wechat.MiniProgram.Abstractions/Mud.Wechat.MiniProgram.Abstractions.csproj",
                     "Src/MiniProgram/Mud.Wechat.MiniProgram.DataModels/Mud.Wechat.MiniProgram.DataModels.csproj",
                     "Tests/Mud.Wechat.MiniProgram.Tests/Mud.Wechat.MiniProgram.Tests.csproj",
                     // 微信支付产品线（4 源 + 2 测试）。
                     "Src/Pay/Mud.Wechat.Pay/Mud.Wechat.Pay.csproj",
                     "Src/Pay/Mud.Wechat.Pay.Abstractions/Mud.Wechat.Pay.Abstractions.csproj",
                     "Src/Pay/Mud.Wechat.Pay.DataModels/Mud.Wechat.Pay.DataModels.csproj",
                     "Src/Pay/Mud.Wechat.Pay.Callback/Mud.Wechat.Pay.Callback.csproj",
                     "Tests/Mud.Wechat.Pay.Tests/Mud.Wechat.Pay.Tests.csproj",
                     "Tests/Mud.Wechat.Pay.Callback.Tests/Mud.Wechat.Pay.Callback.Tests.csproj",
                     // 可观测性适配包的测试工程（源工程已登记于上方 Src/Core 组）。
                     "Tests/Mud.Wechat.OpenTelemetry.Tests/Mud.Wechat.OpenTelemetry.Tests.csproj",
                     // 微信开放平台产品线（2026-10 新增，3 源 + 1 测试；B1 落地 DataModels）。
                     "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Mud.Wechat.OpenPlatform.csproj",
                     "Src/OpenPlatform/Mud.Wechat.OpenPlatform.Abstractions/Mud.Wechat.OpenPlatform.Abstractions.csproj",
                     "Src/OpenPlatform/Mud.Wechat.OpenPlatform.DataModels/Mud.Wechat.OpenPlatform.DataModels.csproj",
                     "Tests/Mud.Wechat.OpenPlatform.Tests/Mud.Wechat.OpenPlatform.Tests.csproj",
                     // 微信小店/视频号（channels 生态）产品线（4 源 + 2 测试，设计方案 v1 §5.2）。
                     "Src/Channels/Mud.Wechat.Channels/Mud.Wechat.Channels.csproj",
                     "Src/Channels/Mud.Wechat.Channels.Abstractions/Mud.Wechat.Channels.Abstractions.csproj",
                     "Src/Channels/Mud.Wechat.Channels.DataModels/Mud.Wechat.Channels.DataModels.csproj",
                     "Src/Channels/Mud.Wechat.Channels.Callback/Mud.Wechat.Channels.Callback.csproj",
                     "Tests/Mud.Wechat.Channels.Tests/Mud.Wechat.Channels.Tests.csproj",
                     "Tests/Mud.Wechat.Channels.Callback.Tests/Mud.Wechat.Channels.Callback.Tests.csproj",
                     // 腾讯广告产品线（2026-10 新增，3 源 + 1 测试）。
                     "Src/Ads/Mud.Wechat.Ads/Mud.Wechat.Ads.csproj",
                     "Src/Ads/Mud.Wechat.Ads.Abstractions/Mud.Wechat.Ads.Abstractions.csproj",
                     "Src/Ads/Mud.Wechat.Ads.DataModels/Mud.Wechat.Ads.DataModels.csproj",
                     "Tests/Mud.Wechat.Ads.Tests/Mud.Wechat.Ads.Tests.csproj",
                 })
        {
            slnx.Should().Contain(project, "新增工程必须纳入解决方案（否则 verify-build 步骤 1 覆盖不到）");
        }

        var ci = File.ReadAllText(Path.Combine(root, ".github", "workflows", "dotnet-publish.yml"));
        // 制品数量守卫必须与「现场推导值」比对，不得硬编码包数；下列历史硬编码值一律不得复活。
        ci.Should().Contain("-ne \"$EXPECTED\"",
            "CI 制品数量守卫必须与由 Src/**/*.csproj 推导出的可打包集比对，不得硬编码包数");
        foreach (var staleCount in new[] { "-ne 10", "-ne 11", "-ne 17", "-ne 18", "-ne 21", "-ne 22", "-ne 23", "-ne 24" })
        {
            ci.Should().NotContain(staleCount,
                $"旧制品数量断言 {staleCount} 已作废（口径改为现场推导），残留即 CI 与工程目录分裂");
        }

        // ⑤ 可打包集**单一来源**：由 Src/**/*.csproj 现场推导（未声明 IsPackable=false 者），
        // CI / pack.bat / publish.bat 三处均须走推导，不得再各自持有名单 —— 2026-10 的两次漂移
        // （CI 停在 18、bat 停在 11 且路径写死在仓库根）正是「清单与工程目录各自成文」所致。
        var srcCsprojs = Directory
            .EnumerateFiles(Path.Combine(root, "Src"), "*.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .OrderBy(static f => f, StringComparer.Ordinal)
            .ToArray();

        static bool IsNonPackable(string csproj) =>
            File.ReadAllText(csproj).Contains("<IsPackable>false", StringComparison.Ordinal);

        // 「误设 IsPackable=false」不会体现在「推导值 vs 产物」的比对里（推导值会同降，比对恒成立）
        // ⇒ 由本白名单兜住：非可打包工程只能是这两个构建期工具。
        srcCsprojs
            .Where(static f => IsNonPackable(f))
            .Select(static f => Path.GetFileNameWithoutExtension(f))
            .OrderBy(static n => n, StringComparer.Ordinal)
            .Should().Equal(
                new[] { "Mud.Wechat.Callback.Analyzers", "Mud.Wechat.Callback.Generator" },
                "非可打包源工程是刻意白名单（仅两个构建期工具）；其它工程被误设 IsPackable=false 会静默少发一个包，"
                + "而「期望值现场推导」无法察觉该变化");

        // 每个可打包工程都必须已纳入解决方案（slnx 漏项 ⇒ 打包产物少于推导值）。
        foreach (var csproj in srcCsprojs.Where(static f => !IsNonPackable(f)))
        {
            var relative = Path.GetRelativePath(root, csproj).Replace('\\', '/');
            slnx.Should().Contain(relative,
                $"可打包工程 {relative} 必须已纳入解决方案（否则 verify-build / dotnet pack 会静默漏掉它）");
        }

        foreach (var script in new[] { "pack.bat", "publish.bat" })
        {
            var bat = File.ReadAllText(Path.Combine(root, script));
            bat.Should().Contain("<IsPackable>false",
                $"{script} 的可打包集必须由源工程现场推导（IsPackable 判定），不得再硬编码名单");
            bat.Should().Contain("dir /s /b \"Src\\*.csproj\"",
                $"{script} 必须现场枚举 Src/**/*.csproj 得到可打包集");
            // EXPECTED=0 是现场推导的初始化（合法）；任何非零字面量都是硬编码期望包数。
            Regex.IsMatch(bat, @"EXPECTED=[1-9]").Should().BeFalse(
                $"{script} 不得硬编码期望包数（EXPECTED=<非零数字>）；期望值须现场推导");
            bat.Should().NotContain("PACKAGES=",
                $"{script} 不得再持有 PACKAGES 硬编码清单 —— 清单漂移正是本守卫存在的初衷");
        }
    }

    /// <summary>
    /// AB-G6 布局不变式（审查报告 v1 P2-1 配套）：<c>Src</c> 下 csproj 保持
    /// <c>{根}/{一级}/{二级}/{文件名}.csproj</c> 两层目录形态，<c>Tests</c> 下保持
    /// <c>{根}/{工程}/{文件名}.csproj</c> 一层目录形态。
    /// </summary>
    /// <remarks>
    /// verify-build.ps1 步骤 2/3 的工程枚举已改为有界深度（<c>-Recurse -Depth 2</c>；Windows PowerShell 5.1
    /// 无长路径支持，仓库根无界递归会撞 MAX_PATH——AGENTS.md §1 记载的实际案例）。有界化引入新假设
    /// 「工程必在既定层级内」：若未来出现更深嵌套工程，步骤 2 将静默漏掉（正是该设置要防的假绿形态）。
    /// 本守卫活体枚举两根下全部 csproj（测试进程为 .NET Core，无 MAX_PATH 限制），把口头目录约定升级为
    /// 守卫锁定——先有守卫后有假设。布局要变化时，本守卫与 verify-build 的枚举深度须同批调整。
    /// </remarks>
    [Fact]
    public void SourceAndTestLayout_ShouldStayTwoLevelDeep()
    {
        var root = GetSolutionRoot();
        // Src 工程为「{一级}/{二级}/{文件名}」两层目录形态（3 段）；测试工程直接位于 Tests/ 下一层（2 段）。
        // 两者均在 verify-build 步骤 2/3 的有界深度 2 枚举范围内；更深嵌套会被静默漏掉（更浅形态同样破坏既有约定）。
        foreach (var (relativeRoot, label, expectedSegments) in new[] { ("Src", "源工程", 3), ("Tests", "测试工程", 2) })
        {
            var rootDir = Path.Combine(root, relativeRoot);
            var relativePaths = Directory
                .EnumerateFiles(rootDir, "*.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(f => Path.GetRelativePath(rootDir, f))
                .ToArray();

            relativePaths.Should().NotBeEmpty($"{relativeRoot} 下必须枚举到工程（空枚举 = verify-build 步骤静默空跑的前兆）");

            foreach (var relative in relativePaths)
            {
                var segments = relative.Split(Path.DirectorySeparatorChar);
                segments.Length.Should().Be(expectedSegments,
                    $"{label} {relativeRoot}/{relative.Replace('\\', '/')} 必须保持既定目录形态：" +
                    "verify-build 步骤枚举为有界深度，更深嵌套会被静默漏掉（布局变化须与枚举深度同批调整）");
            }
        }
    }

    /// <summary>
    /// AB-G8：<b>受控 TFM 例外</b>守卫 —— 微信支付线因 <c>AesGcm</c> 在 netstandard2.0 不存在而
    /// 显式降为 <c>net6.0;net8.0;net10.0</c>；该例外必须被锁定，不得被静默「统一」回 4 档
    /// （静默统一会在 ns2.0 上编译失败或迫使引入第三方密码学包）。
    /// 开放平台线同样降为 3 档，但理由是<b>刻意收窄</b>到组件 <c>AddMudHttpClient</c> 的最小面而非密码学缺口；
    /// <b>AI 域</b>（2026-10 新增，第四个受控例外）降为 2 档 <c>net8.0;net10.0</c>：消费 <c>Mud.AI.*</c> 1.0.1 传递要求的
    /// <c>M.E.*</c> 10.0.12，而 M.E.* 10.x 官方支持面为 net8.0+（net6.0 上 NuGet 发「doesn't support net6.0」告警），
    /// 且不为一条新线把 30 个既有工程的低 TFM 构建拖进未测试组合；三条既有线之外的任何工程覆盖 TFM 即例外外溢，
    /// 本守卫的后半段逐工程锁死。
    /// </summary>
    [Fact]
    public void PayLine_ShouldKeepControlledTfmException()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Pay/Mud.Wechat.Pay.csproj",
                     "Mud.Wechat.Pay.Abstractions/Mud.Wechat.Pay.Abstractions.csproj",
                     "Mud.Wechat.Pay.DataModels/Mud.Wechat.Pay.DataModels.csproj",
                     "Mud.Wechat.Pay.Callback/Mud.Wechat.Pay.Callback.csproj",
                 })
        {
            var content = ReadCsproj(project);

            content.Should().Contain("<TargetFrameworks>net6.0;net8.0;net10.0</TargetFrameworks>",
                $"{project} 必须保持支付线受控 TFM 例外（AesGcm 在 netstandard2.0 不存在）");
            content.Should().Contain("netstandard2.0",
                $"{project} 必须在 csproj 注释中记录该例外的理由（AesGcm / AEAD_AES_256_GCM 为 APIv3 官方强制）");
        }

        // AI 域（第四个受控例外）：只认 net8.0;net10.0 两档，且必须记录理由（Mud.AI 1.0.1 → M.E.* 10.x 支持面）。
        foreach (var project in new[]
                 {
                     "Mud.Wechat.AI.Abstractions/Mud.Wechat.AI.Abstractions.csproj",
                     "Mud.Wechat.AI.Agent/Mud.Wechat.AI.Agent.csproj",
                     "Mud.Wechat.Work.AI/Mud.Wechat.Work.AI.csproj",
                 })
        {
            var content = ReadCsproj(project);

            content.Should().Contain("<TargetFrameworks>net8.0;net10.0</TargetFrameworks>",
                $"{project} 必须保持 AI 域受控 TFM 例外（Mud.AI 1.0.1 → M.E.* 10.x 官方支持面为 net8.0+）");
            content.Should().Contain("AB-G8",
                $"{project} 必须在 csproj 注释中记录该例外由 AB-G8 锁定");
        }

        // 其余产品线仍须是 4 档，各例外的不得外溢。
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj",
                     "Mud.Wechat.Work/Mud.Wechat.Work.csproj",
                     "Mud.Wechat.OfficialAccount/Mud.Wechat.OfficialAccount.csproj",
                     "Mud.Wechat.MiniProgram/Mud.Wechat.MiniProgram.csproj",
                     // 腾讯广告线（2026-10 新增）：只做 HTTPS + JSON、无原生密码学依赖，不得援引例外。
                     "Mud.Wechat.Ads/Mud.Wechat.Ads.csproj",
                     "Mud.Wechat.Ads.Abstractions/Mud.Wechat.Ads.Abstractions.csproj",
                     "Mud.Wechat.Ads.DataModels/Mud.Wechat.Ads.DataModels.csproj",
                 })
        {
            ReadCsproj(project).Should().NotContain("<TargetFrameworks>",
                $"{project} 应继承 Directory.Build.props 的 4 档 TFM，不得自行覆盖" +
                "（受控例外只有支付线、开放平台线与 AI 域，后者是 Mud.AI 1.0.1 的 M.E.* 10.x 支持面约束）");
        }
    }

    /// <summary>
    /// AB-G9：SSRF 白名单并集数组的<b>变更评审面</b> —— 白名单是进程级全局静态 + 整体替换，
    /// 新产品线的默认域名<b>优先</b>由「子域后缀覆盖」满足；覆盖不到时才允许追加一条后缀域，
    /// 且<b>永不</b>逐主机登记（那会让放行面随端点增长失控）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 支付 <c>api.mch.weixin.qq.com</c> 与小程序 <c>api.weixin.qq.com</c> 均已被 <c>weixin.qq.com</c>
    /// 后缀覆盖，故两条线对白名单<b>零改动</b>（PAY-B8 / MP-X8）。
    /// </para>
    /// <para>
    /// <b>腾讯广告是第一条覆盖不到的线</b>（<c>api.e.qq.com</c>，非 <c>weixin.qq.com</c> 子域）⇒
    /// 本批以 <c>e.qq.com</c> <b>一条后缀域</b>接入，这是「并集追加而非替换」的合法形态，
    /// 代价是全局放行面被扩大（任一产品线的 <c>BaseUrl</c> 现可指向 <c>*.e.qq.com</c>）。
    /// 本守卫因此从「数组恰为两条」升级为<b>形态断言</b>：只允许后缀级条目、禁止 FQDN 条目、禁止重复条目。
    /// 产品线侧的「不得自行登记」仍由 AB-G4 逐线扫描（广告线侧另有 ADS-B4 定点强化）。
    /// </para>
    /// </remarks>
    [Fact]
    public void AllowedBaseUrlDomains_ShouldCoverNewLinesWithoutDuplication()
    {
        var hosts = File.ReadAllText(
            SourcePath("Mud.Wechat.Abstractions", "WechatApiHosts.cs"));

        var allowed = WechatApiHosts.AllowedBaseUrlDomains;

        allowed.Should().Contain("weixin.qq.com",
            "weixin.qq.com 一项即覆盖公众号与微信支付域名（host 等于域或以其子域结尾即放行）");

        // 支付与小程序默认域名必须被既有条目覆盖 ⇒ 不得为其新增独立白名单条目。
        foreach (var host in new[] { "api.weixin.qq.com", "api.mch.weixin.qq.com", "api2.mch.weixin.qq.com" })
        {
            allowed.Should().NotContain(host,
                $"{host} 已被 weixin.qq.com 后缀覆盖，新增独立条目只会扩大全局放行面（判定语义见 WechatApiHosts 注释）");
        }

        // 广告线：只允许一条后缀域，业务主机不得各自入表。
        allowed.Should().Contain("e.qq.com",
            "api.e.qq.com 不被任何微信系后缀覆盖，广告线必须以 e.qq.com 接入并集");
        foreach (var host in new[] { "api.e.qq.com", "developers.e.qq.com" })
        {
            allowed.Should().NotContain(host,
                $"{host} 已被 e.qq.com 后缀覆盖，逐主机登记即放行面失控（ADS-B4 同口径）");
        }

        allowed.Should().OnlyContain(static s => s.Length > 0,
            "白名单条目不得为空串（空串在「以子域结尾即放行」的判定下会放行全部主机）");

        allowed.Distinct(StringComparer.OrdinalIgnoreCase).Should().HaveCount(allowed.Length,
            "白名单条目重复不会报错但会让并集语义不可审计");

        // 组件 UrlValidator 为全局静态 + 整体替换：**代码中**不得二次登记。
        // 注意：WechatApiHosts.cs 的 XML 注释会引用 `ConfigureAllowedDomains` 这一方法名来解释语义，
        // 故须先剔除注释再计数，否则守卫会对「文档提及」误报（此坑已踩过一次）。
        var codeOnly = System.Text.RegularExpressions.Regex.Replace(
            hosts, @"//.*?$|/\*.*?\*/|///.*?$", string.Empty,
            System.Text.RegularExpressions.RegexOptions.Multiline
            | System.Text.RegularExpressions.RegexOptions.Singleline);

        codeOnly.Should().Contain("AllowedBaseUrlDomains");
        System.Text.RegularExpressions.Regex.Matches(codeOnly, @"ConfigureAllowedDomains").Count.Should().Be(0,
            "白名单登记只能发生在 WechatTokenRecoveryRegistration，不得在常量类内二次登记");
    }

    /// <summary>
    /// AB-G10：配置 DTO 禁用 <c>required</c> —— <b>跨产品线源码扫描</b>（自维护，不依赖类型清单）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 背景：配置绑定为源生成（<c>EnableConfigurationBindingGenerator=true</c>），生成器以 <c>new T()</c>
    /// 构造配置 DTO，故 DTO 使用 <c>required</c> 成员必然触发 <c>CS9035</c>。
    /// </para>
    /// <para>
    /// 为何用<b>源码扫描</b>而非反射列举类型：反射方案必须硬编码类型清单
    /// （既有 G2 只列了 <c>WechatAppConfig</c>，公众号与新增产品线均不在内），而测试工程若要看到其他产品线的
    /// 内部类型就得跨产品线引用测试工程 —— 既破坏产品线隔离，又制造新的漂移点。
    /// 源码扫描按「文件位置」识别配置 DTO，新增产品线无需改本守卫即自动纳入覆盖。
    /// </para>
    /// </remarks>
    [Fact]
    public void ConfigDtos_ShouldNotUseRequired_AcrossAllProductLines()
    {
        var root = GetSolutionRoot();

        // 配置 DTO 的位置约定：{产品线}.Abstractions/Configuration/*.cs，或 {产品线}.Callback/*CallbackOptions.cs。
        var configDtoFiles = Directory
            .EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}Tests{Path.DirectorySeparatorChar}"))
            .Where(f =>
            {
                var normalized = f.Replace('\\', '/');
                return (normalized.Contains(".Abstractions/Configuration/", StringComparison.Ordinal)
                        || normalized.EndsWith("CallbackOptions.cs", StringComparison.Ordinal))
                    && normalized.Contains("/Mud.Wechat.", StringComparison.Ordinal);
            })
            .ToList();

        configDtoFiles.Should().NotBeEmpty("配置 DTO 扫描规则失效：未匹配到任何配置文件（路径约定已变更？）");

        var offenders = new List<string>();
        foreach (var file in configDtoFiles)
        {
            var withoutComments = System.Text.RegularExpressions.Regex.Replace(
                File.ReadAllText(file), @"//.*?$|/\*.*?\*/", string.Empty,
                System.Text.RegularExpressions.RegexOptions.Multiline
                | System.Text.RegularExpressions.RegexOptions.Singleline);

            if (System.Text.RegularExpressions.Regex.IsMatch(withoutComments, @"\brequired\b"))
            {
                offenders.Add(Path.GetFileName(file));
            }
        }

        offenders.Should().BeEmpty(
            "配置 DTO 禁用 required 成员（配置绑定源生成器以 new T() 构造，必报 CS9035）；" +
            $"校验应改写进 Validate()。实际命中：{string.Join(", ", offenders)}");
    }

    /// <summary>去掉 XML 注释，避免注释文本里的属性字样被误判为真实配置。</summary>
    private static string StripXmlComments(string xml)
    {
        var result = System.Text.RegularExpressions.Regex.Replace(xml, "<!--.*?-->", string.Empty,
            System.Text.RegularExpressions.RegexOptions.Singleline);
        return result;
    }
}
