// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.Abstractions.Tests.ContractGuards;

/// <summary>
/// 叶层回调协议与安全内核契约守卫（CB-L1c / CB-L1d / CB-L1e / CB-L1h / CB-L1i）。
/// </summary>
/// <remarks>
/// <para>
/// 背景（回调接收模块设计方案 v3 §0.4）：叶层只下沉「协议与安全内核」（加解密、异常面、抗重放、
/// XML 投影、类型注册表基类、中立信封接口），**不下沉**契约 / 载荷读取器 / 分发器 / 中间件。
/// 本组守卫锁定这条边界的两个方向：① 叶层不得沾产品线语义；② 下沉物必须唯一（不得在产品线内留有副本）。
/// </para>
/// <para>
/// 命名说明：<c>CB-L1*</c> 段与企微侧的 <c>CB1~CB24</c>（回调域守卫）编号形态不同，避免与既有编号混淆；
/// 企微侧另有 CB-L1a/L1b/L1f/L1g 与 CB-MP-x（见 <c>Tests/Mud.Wechat.Work.Tests</c> 与公众号测试工程）。
/// </para>
/// </remarks>
public class WechatCallbackKernelContractGuards
{
    /// <summary>叶层回调内核的源码目录（相对仓库根）。</summary>
    private const string KernelDirectory = "Mud.Wechat.Abstractions/Callback";

    /// <summary>定位仓库根（锚定 <c>Mud.Wechat.slnx</c> 哨兵文件）。</summary>
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

    private static string ReadSource(string relativePath)
    {
        var path = SourcePath(relativePath.Split('/'));
        File.Exists(path).Should().BeTrue($"守卫依赖的源文件必须存在：{relativePath}");
        return File.ReadAllText(path, Encoding.UTF8);
    }

    /// <summary>枚举叶层回调内核目录下的全部 .cs（相对仓库根）。</summary>
    private static List<string> GetKernelFiles()
        => Directory
            .EnumerateFiles(SourcePath("Mud.Wechat.Abstractions", "Callback"), "*.cs",
                SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .ToList();

    /// <summary>
    /// CB-L1c：叶层零产品线依赖（csproj 不得引用任何产品线工程，含 Roslyn 工具工程旧名形态）。
    /// </summary>
    [Fact]
    public void AbstractionsPackage_ShouldNotReferenceProductLineOrToolProjects()
    {
        var csproj = ReadSource("Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj");

        foreach (var forbidden in new[]
                 {
                     "Mud.Wechat.Work",
                     "Mud.Wechat.OfficialAccount",
                     "Mud.Wechat.Redis",
                 })
        {
            csproj.Should().NotContain($"{forbidden}.csproj",
                "叶层不得反向引用任何产品线程序集（含工具工程与存储实现包）");
        }

        csproj.Should().Contain("Mud.HttpUtils",
            "叶层回调内核依赖上游 PayloadNode/PayloadFieldMap 契约（XML-free 载荷模型）");
    }

    /// <summary>
    /// CB-L1d：叶层回调内核**零产品线语义**（源码文本，排除注释行）。
    /// </summary>
    /// <remarks>
    /// 防「顺手把企微开放面矩阵、Bot 通道或中间件搬进叶层」——那是击穿 AB-G1 的最短路径。
    /// XML 类型**允许**出现（叶层有意承担「XML → PayloadNode」投影职责，见 CB-L1i 唯一性约束）。
    /// </remarks>
    [Fact]
    public void CallbackKernel_ShouldNotCarryProductLineSemantics()
    {
        var forbidden = new[]
        {
            "WechatAppType",
            "WechatAppTypeSet",
            "WechatCallbackChannel",
            "WechatCallbackEventFamily",
            "Microsoft.AspNetCore",
            "Mud.Wechat.Work",
            "Mud.Wechat.OfficialAccount",
        };

        var files = GetKernelFiles();
        files.Should().NotBeEmpty("叶层回调内核目录不得为空（守卫空转即失效）");

        foreach (var file in files)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var marker in forbidden)
                {
                    line.Should().NotContain(marker,
                        $"{Path.GetFileName(file)} 不得出现产品线语义（CB-L1d：叶层只承载协议与安全内核）");
                }
            }
        }
    }

    /// <summary>
    /// CB-L1e：叶层无 ASP.NET 依赖（csproj 层，与 CB-L1d 源码文本断言互为双保险）。
    /// </summary>
    [Fact]
    public void AbstractionsPackage_ShouldNotDependOnAspNetCore()
    {
        var csproj = ReadSource("Mud.Wechat.Abstractions/Mud.Wechat.Abstractions.csproj");

        csproj.Should().NotContain("Microsoft.AspNetCore",
            "叶层引入 ASP.NET 会把框架依赖传染给全部叶层消费者（含只声明处理器的宿主）");
        csproj.Should().NotContain("<FrameworkReference",
            "叶层不得使用 FrameworkReference（同上）");
    }

    /// <summary>
    /// CB-L1h：Work 侧公开面零破坏 —— 信封实现叶层接口，处理器签名与程序集归属不变。
    /// </summary>
    [Fact]
    public void WorkCallbackPublicSurface_ShouldBeUnchanged()
    {
        // ① 企微信封实现叶层中立接口（零签名破坏的纯实现关系）。
        typeof(IWechatCallbackEnvelope).IsAssignableFrom(typeof(WechatCallbackEvent))
            .Should().BeTrue("WechatCallbackEvent 必须实现叶层 IWechatCallbackEnvelope");

        // ② 处理器契约仍与**共享基座**（配置面 / 多应用面）同处一个程序集，且签名仍是企微信封（不得降级为基类/接口）。
        //    断言「与基座同源」而非「程序集叫什么」：表达的是分层语义（契约在共享基座、不在业务运行时包），
        //    程序集改名 / 合并不应让该语义变形。
        typeof(IWechatCallbackEventHandler).Assembly.Should().BeSameAs(
            typeof(Mud.Wechat.Work.Abstractions.Configuration.WechatAppConfig).Assembly,
            "CB-L1h：回调处理器契约必须落在共享基座包（与配置面 / 多应用面同源），"
            + "否则「只收回调」的宿主被迫依赖整个业务运行时包");
        var handle = typeof(IWechatCallbackEventHandler).GetMethod("HandleAsync");
        handle.Should().NotBeNull();
        handle!.GetParameters()[0].ParameterType.Should().Be<WechatCallbackEvent>(
            "处理器签名必须保持企微信封（否则宿主全部处理器需要改签名）");

        // ③ 载荷处理器基类与处理器契约**同源**，且泛型元数保持 1（源码兼容的前置条件）。
        typeof(WechatCallbackPayloadHandler<>).Assembly.Should().BeSameAs(
            typeof(IWechatCallbackEventHandler).Assembly,
            "CB-L1h：载荷处理器基类必须与处理器契约同处共享基座包");
        typeof(WechatCallbackPayloadHandler<>).IsGenericTypeDefinition.Should().BeTrue();
        typeof(WechatCallbackPayloadHandler<>).GetGenericArguments().Length.Should().Be(1);
    }

    /// <summary>
    /// CB-L1i：下沉物**唯一性** —— 已下沉类型的定义只能出现在叶层（防「下沉后又留副本」）。
    /// </summary>
    [Fact]
    public void SunkKernelTypes_ShouldExistOnlyInAbstractions()
    {
        var sunkFiles = new[]
        {
            "WechatCallbackCrypto.cs",
            "WechatCallbackException.cs",
            "InMemoryWechatCallbackReplayGuard.cs",
            "XElementPayloadSource.cs",
            "WechatPayloadSourceCache.cs",
            "WechatCallbackTypeRegistry.cs",
        };

        // 引擎侧全部源码（排除 obj/bin 与叶层自身）。
        var productLineSources = Directory
            .EnumerateFiles(GetSolutionRoot(), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                        && !f.Contains($"{Path.DirectorySeparatorChar}.docs{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                        && !f.StartsWith(SourceProjectDir("Mud.Wechat.Abstractions") + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var fileName in sunkFiles)
        {
            productLineSources
                .Where(f => string.Equals(Path.GetFileName(f), fileName, StringComparison.Ordinal))
                .Should().BeEmpty($"CB-L1i：{fileName} 已下沉叶层，产品线内不得留有同名实现文件（双份即双份事实来源）");
        }

        // 反向：叶层内核目录必须含全部下沉物（防「守卫只看空跑」）。
        var kernelNames = GetKernelFiles().Select(Path.GetFileName).ToList();
        foreach (var fileName in sunkFiles)
        {
            kernelNames.Should().Contain(fileName, $"CB-L1i：叶层内核应含下沉物 {fileName}");
        }
    }

    /// <summary>
    /// CB-L1i（续）：叶层回调内核内 **XML 触点唯一**（仅 <c>XElementPayloadSource.cs</c>）。
    /// </summary>
    /// <remarks>
    /// 与企微侧 CB14 同构：XML 类型只能出现在唯一投影器内 —— 词边界正则避免把
    /// 标识符 <c>XElementPayloadSource</c> 误判为 XML 类型使用（其包含子串 <c>XElement</c>）。
    /// </remarks>
    [Fact]
    public void CallbackKernel_ShouldKeepXmlTouchPointUnique()
    {
        var allowed = "XElementPayloadSource.cs";
        var xmlType = new Regex(@"\bXDocument\b|\bXElement\b", RegexOptions.Compiled);
        var xmlNamespace = "System.Xml.Linq";

        var files = GetKernelFiles();
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            if (string.Equals(name, allowed, StringComparison.Ordinal))
            {
                continue;
            }

            foreach (var line in File.ReadAllLines(file))
            {
                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                line.Should().NotContain(xmlNamespace, $"{name} 不得引入 XML 命名空间（CB-L1i）");
                xmlType.IsMatch(line).Should().BeFalse($"{name} 不得使用 XML 类型（XML 触点唯一）");
            }
        }
    }
}
