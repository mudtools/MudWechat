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

    private static string ReadCsproj(string relativePath)
    {
        var path = Path.Combine(GetSolutionRoot(), relativePath);
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
        var work = ReadCsproj("Mud.Wechat.Work.DataModels/Mud.Wechat.Work.DataModels.csproj");
        var mp = ReadCsproj("Mud.Wechat.OfficialAccount.DataModels/Mud.Wechat.OfficialAccount.DataModels.csproj");

        foreach (var content in new[] { work, mp })
        {
            content.Should().Contain("Mud.Wechat.Abstractions.csproj",
                "DataModels 需实现公用层判错契约 IWechatApiResponse");
        }

        // 两条产品线的 DataModels 之间不得互相引用（产品线隔离）。
        work.Should().NotContain("Mud.Wechat.OfficialAccount");
        mp.Should().NotContain("Mud.Wechat.Work.");
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
        var root = GetSolutionRoot();
        var productLineRoots = new[]
        {
            Path.Combine(root, "Mud.Wechat.Work"),
            Path.Combine(root, "Mud.Wechat.Work.Abstractions"),
            Path.Combine(root, "Mud.Wechat.Work.Callback"),
            Path.Combine(root, "Mud.Wechat.OfficialAccount"),
            Path.Combine(root, "Mud.Wechat.OfficialAccount.Abstractions"),
        };

        foreach (var lineRoot in productLineRoots)
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
        var root = GetSolutionRoot();
        var productLineRoots = new[]
        {
            Path.Combine(root, "Mud.Wechat.Work"),
            Path.Combine(root, "Mud.Wechat.Work.Abstractions"),
            Path.Combine(root, "Mud.Wechat.Work.Callback"),
            Path.Combine(root, "Mud.Wechat.OfficialAccount"),
            Path.Combine(root, "Mud.Wechat.OfficialAccount.Abstractions"),
        };

        foreach (var lineRoot in productLineRoots)
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
    /// AB-G6：门禁脚本同批演进（防「新增产品线成为门禁盲区」）。
    /// </summary>
    /// <remarks>
    /// 三项均为「缺一即 CI/门禁失效」的硬约束：审计脚本白名单、DT 标注脚本根命名空间、CI 制品数量。
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

        var annotate = File.ReadAllText(Path.Combine(root, "scripts", "AddHttpJsonSerializable.ps1"));
        annotate.Should().Contain("$RootNamespace", "根命名空间必须参数化，否则非默认产品线根级 DTO 分组错误");

        var slnx = File.ReadAllText(Path.Combine(root, "Mud.Wechat.slnx"));
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj",
                     "Mud.Wechat.OfficialAccount/Mud.Wechat.OfficialAccount.csproj",
                     "Mud.Wechat.OfficialAccount.Abstractions/Mud.Wechat.OfficialAccount.Abstractions.csproj",
                     "Mud.Wechat.OfficialAccount.DataModels/Mud.Wechat.OfficialAccount.DataModels.csproj",
                     "Tests/Mud.Wechat.Abstractions.Tests/Mud.Wechat.Abstractions.Tests.csproj",
                     "Tests/Mud.Wechat.OfficialAccount.Tests/Mud.Wechat.OfficialAccount.Tests.csproj",
                 })
        {
            slnx.Should().Contain(project, "新增工程必须纳入解决方案（否则 verify-build 步骤 1 覆盖不到）");
        }

        var ci = File.ReadAllText(Path.Combine(root, ".github", "workflows", "dotnet-publish.yml"));
        ci.Should().Contain("-ne 9", "制品数量守卫必须随新增产品线更新（否则打包步骤 fail-closed 必红）");
    }
}
