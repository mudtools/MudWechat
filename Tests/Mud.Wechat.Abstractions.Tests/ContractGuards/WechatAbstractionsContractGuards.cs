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
    /// AB-G4：SSRF 白名单单一来源——`ConfigureAllowedDomains` 只允许在公用层登记点被调用，
    /// 各产品线不得各自调用（全局静态 + 整体替换 ⇒ 后调用者清空前者）。
    /// </summary>
    [Fact]
    public void AllowedDomainRegistration_ShouldBeCentralized()
    {
        foreach (var lineRoot in ProductLineSourceRoots())
        {
            var hits = Directory.EnumerateFiles(lineRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                            && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                .Where(f => File.ReadAllText(f).Contains("ConfigureAllowedDomains", StringComparison.Ordinal))
                .Select(f => Path.GetFileName(f))
                .ToList();

            hits.Should().BeEmpty(
                $"SSRF 白名单登记只能出现在公用层单点（WechatTokenRecoveryRegistration）；" +
                $"实际命中：{string.Join(", ", hits)}");
        }
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
    /// 实测口径（<c>dotnet pack Mud.Wechat.slnx -c Release</c>）：恰 <b>24</b> 个 nupkg，
    /// 其中四个回调包内均含 <c>analyzers/dotnet/cs/Mud.Wechat.Callback.Analyzers.dll</c>。
    /// </para>
    /// </remarks>
    [Fact]
    public void CallbackPackages_ShouldShipAndEmbedAnalyzer()
    {
        var root = GetSolutionRoot();

        // ① 宿主包：不得声明 IsPackable=false（否则包不再发布，诊断能力对消费者失效）。
        foreach (var hostPackage in new[]
                 {
                     "Mud.Wechat.Work.Callback/Mud.Wechat.Work.Callback.csproj",
                     "Mud.Wechat.OfficialAccount.Callback/Mud.Wechat.OfficialAccount.Callback.csproj",
                     "Mud.Wechat.Pay.Callback/Mud.Wechat.Pay.Callback.csproj",
                     // 微信小店/视频号（channels 生态）产品线（设计方案 v1 §3.5）：回调包随包下发分析器。
                     "Mud.Wechat.Channels.Callback/Mud.Wechat.Channels.Callback.csproj",
                 })
        {
            var source = File.ReadAllText(SourcePath(hostPackage.Split('/')));
            var withoutComments = StripXmlComments(source);

            withoutComments.Should().NotContain("<IsPackable>",
                $"{hostPackage} 是回调宿主包，必须产出 nupkg（分析器随包内嵌）；" +
                "若确实要停止发布，须同步修改 CI 制品数量守卫与本文档说明");

            withoutComments.Should().Contain("analyzers/dotnet/cs",
                $"{hostPackage} 必须把 Mud.Wechat.Callback.Analyzers 内嵌到 analyzers/dotnet/cs");
        }

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

        // ③ CI 期望清单必须显式列出全部回调包（口径漂移会在打包步骤 fail-closed）。
        // 注意：CI 的清单按**短名**列举（与既有写法一致），故此处按短名断言。
        var ci = File.ReadAllText(Path.Combine(root, ".github", "workflows", "dotnet-publish.yml"));
        ci.Should().Contain("Work.Callback", "CI 制品清单必须含企微回调包");
        ci.Should().Contain("OfficialAccount.Callback", "CI 制品清单必须含公众号回调包");
        ci.Should().Contain("Pay.Callback", "CI 制品清单必须含微信支付回调包");
        ci.Should().Contain("Channels.Callback", "CI 制品清单必须含小店回调包（设计方案 v1 §5.2）");
    }

    /// <summary>
    /// 受 AB-G4 / AB-G5 扫描覆盖的产品线源码根（**新增产品线必须同批登记**，否则其内的违规调用无人拦截）。
    /// </summary>
    private static string[] ProductLineSourceRoots()
    {
        return new[]
        {
            SourceProjectDir("Mud.Wechat.Work"),
            SourceProjectDir("Mud.Wechat.Work.Abstractions"),
            SourceProjectDir("Mud.Wechat.Work.Callback"),
            SourceProjectDir("Mud.Wechat.OfficialAccount"),
            SourceProjectDir("Mud.Wechat.OfficialAccount.Abstractions"),
            // 微信小程序产品线（设计方案 v2 §3）：令牌复用公众号基座，但不得自行登记白名单/判定器单槽。
            SourceProjectDir("Mud.Wechat.MiniProgram"),
            SourceProjectDir("Mud.Wechat.MiniProgram.Abstractions"),
            // 微信支付产品线（设计方案 v2 §2）：**不使用 [Token]**，白名单零改动（PAY-B8）。
            SourceProjectDir("Mud.Wechat.Pay"),
            SourceProjectDir("Mud.Wechat.Pay.Abstractions"),
            SourceProjectDir("Mud.Wechat.Pay.Callback"),
            // 微信开放平台产品线（2026-10 新增）：同受 AB-G4 / AB-G5 单点登记约束。
            SourceProjectDir("Mud.Wechat.OpenPlatform"),
            SourceProjectDir("Mud.Wechat.OpenPlatform.Abstractions"),
            // 微信小店/视频号（channels 生态）产品线（设计方案 v1）：SSRF 白名单与判定器单槽同受
            // 公用层单点登记约束（AddChannelsApp 只登记子判定器与自定义主机，不得直接写单槽）。
            SourceProjectDir("Mud.Wechat.Channels"),
            SourceProjectDir("Mud.Wechat.Channels.Abstractions"),
            SourceProjectDir("Mud.Wechat.Channels.DataModels"),
            SourceProjectDir("Mud.Wechat.Channels.Callback"),
        };
    }

    /// <summary>
    /// AB-G6：门禁脚本同批演进（防「新增产品线成为门禁盲区」）。
    /// </summary>
    /// <remarks>
    /// 四项均为「缺一即 CI/门禁失效」的硬约束：审计脚本白名单与搜索范围、DTO 标注脚本根命名空间、
    /// 解决方案工程清单、CI 制品数量。**新增产品线时本守卫是唯一会自动变红的门禁接入口。**
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
        audit.Should().Contain("Mud.Wechat.OfficialAccount'", "公众号工程必须纳入消费点搜索范围");
        audit.Should().Contain("Mud.Wechat.OfficialAccount.Callback/MpCallbackOptions.cs",
            "公众号回调配置面必须纳入审计白名单（否则配置属性无消费点也无人发现）");
        audit.Should().Contain("Mud.Wechat.OfficialAccount.Callback'",
            "公众号回调运行时包必须纳入消费点搜索范围（未纳入即门禁盲区）");
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
        audit.Should().Contain("Mud.Wechat.Pay.Callback'", "微信支付回调运行时包必须纳入消费点搜索范围");
        // 微信小程序：三工程均须纳入搜索范围（消费点可能出现在主包 / 抽象包任一处）。
        audit.Should().Contain("Mud.Wechat.MiniProgram.Abstractions'", "微信小程序抽象包必须纳入消费点搜索范围");
        audit.Should().Contain("Mud.Wechat.MiniProgram'", "微信小程序主包必须纳入消费点搜索范围");
        // 微信小店/视频号（channels 生态）产品线（设计方案 v1 §3.6）：配置 DTO 与四个搜索根必须纳入审计。
        audit.Should().Contain("Mud.Wechat.Channels.Abstractions/Configuration/ChannelsAppConfig.cs", "小店配置面必须纳入审计（缺登记即属性无消费点无人发现）");
        audit.Should().Contain("Mud.Wechat.Channels.Abstractions'", "小店抽象包必须纳入消费点搜索范围");
        audit.Should().Contain("Mud.Wechat.Channels'", "小店主包必须纳入消费点搜索范围");
        audit.Should().Contain("Mud.Wechat.Channels.DataModels'", "小店 DataModels 必须纳入消费点搜索范围");
        audit.Should().Contain("Mud.Wechat.Channels.Callback'", "小店回调运行时包必须纳入消费点搜索范围");

        var annotate = File.ReadAllText(Path.Combine(root, "scripts", "AddHttpJsonSerializable.ps1"));
        annotate.Should().Contain("$RootNamespace", "根命名空间必须参数化，否则非默认产品线根级 DTO 分组错误");

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
                     // 可观测性适配包（1 源 + 1 测试）。
                     "Src/OpenPlatform/Mud.Wechat.OpenTelemetry/Mud.Wechat.OpenTelemetry.csproj",
                     "Tests/Mud.Wechat.OpenTelemetry.Tests/Mud.Wechat.OpenTelemetry.Tests.csproj",
                     // 微信开放平台产品线（2026-10 新增，2 源 + 1 测试）。
                     "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Mud.Wechat.OpenPlatform.csproj",
                     "Src/OpenPlatform/Mud.Wechat.OpenPlatform.Abstractions/Mud.Wechat.OpenPlatform.Abstractions.csproj",
                     "Tests/Mud.Wechat.OpenPlatform.Tests/Mud.Wechat.OpenPlatform.Tests.csproj",
                     // 微信小店/视频号（channels 生态）产品线（4 源 + 2 测试，设计方案 v1 §5.2）。
                     "Src/Channels/Mud.Wechat.Channels/Mud.Wechat.Channels.csproj",
                     "Src/Channels/Mud.Wechat.Channels.Abstractions/Mud.Wechat.Channels.Abstractions.csproj",
                     "Src/Channels/Mud.Wechat.Channels.DataModels/Mud.Wechat.Channels.DataModels.csproj",
                     "Src/Channels/Mud.Wechat.Channels.Callback/Mud.Wechat.Channels.Callback.csproj",
                     "Tests/Mud.Wechat.Channels.Tests/Mud.Wechat.Channels.Tests.csproj",
                     "Tests/Mud.Wechat.Channels.Callback.Tests/Mud.Wechat.Channels.Callback.Tests.csproj",
                 })
        {
            slnx.Should().Contain(project, "新增工程必须纳入解决方案（否则 verify-build 步骤 1 覆盖不到）");
        }

        var ci = File.ReadAllText(Path.Combine(root, ".github", "workflows", "dotnet-publish.yml"));
        ci.Should().Contain("-ne 24", "制品数量守卫必须随新增产品线更新（否则打包步骤 fail-closed 必红）");
        ci.Should().NotContain("-ne 18", "旧制品数量断言已随小店线扩展作废，残留即 CI 与守卫口径分裂");
        ci.Should().NotContain("-ne 22", "回调包打包路径修复后计数为 24，残留 -ne 22 即 CI 与守卫口径分裂");
        ci.Should().NotContain("-ne 10", "旧制品数量断言已随产品线扩展作废，残留即 CI 与守卫口径分裂");
        ci.Should().NotContain("-ne 17", "OpenTelemetry 包并入后计数为 18，残留 -ne 17 即 CI 与守卫口径分裂");
        ci.Should().NotContain("-ne 11", "旧制品数量断言已随产品线扩展作废，残留即 CI 与守卫口径分裂");
    }

    /// <summary>
    /// AB-G8：<b>受控 TFM 例外</b>守卫 —— 微信支付线因 <c>AesGcm</c> 在 netstandard2.0 不存在而
    /// 显式降为 <c>net6.0;net8.0;net10.0</c>；该例外必须被锁定，不得被静默「统一」回 4 档
    /// （静默统一会在 ns2.0 上编译失败或迫使引入第三方密码学包）。
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

        // 其余产品线仍须是 4 档，支付线的例外不得外溢。
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj",
                     "Mud.Wechat.Work/Mud.Wechat.Work.csproj",
                     "Mud.Wechat.OfficialAccount/Mud.Wechat.OfficialAccount.csproj",
                     "Mud.Wechat.MiniProgram/Mud.Wechat.MiniProgram.csproj",
                 })
        {
            ReadCsproj(project).Should().NotContain("<TargetFrameworks>",
                $"{project} 应继承 Directory.Build.props 的 4 档 TFM，不得自行覆盖（唯一例外为支付线）");
        }
    }

    /// <summary>
    /// AB-G9：SSRF 白名单并集数组的<b>变更评审面</b> —— 白名单是进程级全局静态 + 整体替换，
    /// 新产品线的默认域名必须由「子域后缀覆盖」而非新增独立条目来满足（新增条目即扩大全局放行面）。
    /// </summary>
    /// <remarks>
    /// 支付 `api.mch.weixin.qq.com` 与小程序 `api.weixin.qq.com` 均已被 `weixin.qq.com` 后缀覆盖，
    /// 故两条新线对白名单**零改动**（PAY-B8 / MP-X8）。本守卫锁定该事实，防止有人「顺手」加重复条目。
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
