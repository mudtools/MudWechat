// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;

namespace Mud.Wechat.Redis.Tests.ContractGuards;

/// <summary>
/// T-R8：Redis 模块契约守卫（源码文本 + 反射）。
/// </summary>
public class WechatRedisContractGuards
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

    private static string ReadSource(string relativePath)
        => File.ReadAllText(SourcePath(relativePath.Replace('\\', '/').Split('/')), Encoding.UTF8);

    private static string ExtractMethod(string source, string signatureMarker, string sourceName)
    {
        var start = source.IndexOf(signatureMarker, StringComparison.Ordinal);
        start.Should().BePositive($"守卫前提：{sourceName} 中必须能定位 {signatureMarker}（签名漂移须同步更新守卫）");

        var bodyStart = source.IndexOf('{', start);
        var depth = 0;
        for (var i = bodyStart; i < source.Length; i++)
        {
            if (source[i] == '{')
            {
                depth++;
            }
            else if (source[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source.Substring(start, i - start + 1);
                }
            }
        }

        throw new InvalidOperationException($"未能提取 {sourceName} 中 {signatureMarker} 的方法体");
    }

    /// <summary>枚举 Redis 包全部源文件（相对解决方案根）。</summary>
    private static List<string> GetRedisSourceFiles()
        => Directory
            .EnumerateFiles(SourceProjectDir("Mud.Wechat.Redis"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                           && !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .ToList();

    /// <summary>
    /// RD-G1：SCAN 模式必须仅经 <see cref="WechatRedisKeyBuilder.Pattern"/> 产出——
    /// 任何 KeysAsync 调用点所在文件必须引用 Pattern 单一出口；Pattern 本体必须内含 glob 字面量转义
    /// （飞书 D10 静默失效缺陷防回归）。
    /// </summary>
    [Fact]
    public void ScanPatterns_ShouldOnlyBeProducedByKeyBuilderPattern_SingleExit()
    {
        var keyBuilderSource = ReadSource(Path.Combine("Mud.Wechat.Redis", "Services", "WechatRedisKeyBuilder.cs"));

        // Pattern 单一出口本体必须先 glob 转义再拼 ":*"（键侧 "\:" 转义在 glob 侧须再转义一次）。
        var patternBody = ExtractMethod(keyBuilderSource, "public static string Pattern(string prefix", nameof(WechatRedisKeyBuilder));
        patternBody.Should().Contain("EscapeGlobLiteral(Combine(prefix, segments))", "SCAN 模式必须经 glob 字面量转义");
        patternBody.Should().Contain("Separator + \"*\"", "模式必须以段级精确的 :* 结尾（Separator = \":\"）");

        var filesWithKeysAsync = GetRedisSourceFiles()
            .Where(path => File.ReadAllText(path).Contains("KeysAsync("))
            .ToList();
        filesWithKeysAsync.Should().NotBeEmpty("SCAN 消费点应存在于令牌/授权存储");

        foreach (var path in filesWithKeysAsync)
        {
            var source = File.ReadAllText(path);
            source.Should().Contain("WechatRedisKeyBuilder.Pattern(",
                $"KeysAsync 调用点必须消费 Pattern 单一出口（文件：{Path.GetFileName(path)}）");
        }
    }

    /// <summary>
    /// RD-G2：配置 DTO 禁用 required（配置绑定源生成以 new T() 构造 → CS9035，AGENTS.md 治理红线）。
    /// </summary>
    [Fact]
    public void ConfigDtos_ShouldNotUseRequired()
    {
        foreach (var fileName in new[] { "WechatRedisOptions.cs", "WechatRedisConnectionOptions.cs" })
        {
            var source = ReadSource(Path.Combine("Mud.Wechat.Redis", "Configuration", fileName));
            var requiredProps = Regex.Matches(source, @"^\s*public\s+(?!class|const)[^\n]*\brequired\s", RegexOptions.Multiline);
            requiredProps.Should().BeEmpty($"{fileName} 不得声明 required 属性（G2 同源）");
        }
    }

    /// <summary>
    /// RD-G3：重放守卫 fail-closed——TryMarkAsync 方法体内 Redis 异常路径必须上抛
    ///（含 WechatRedisErrors.Map 包装），不得出现「catch 吞异常返回 true/false」的反模式。
    /// </summary>
    [Fact]
    public void ReplayGuardTryMarkAsync_ShouldBeFailClosed_NoSwallowedReturns()
    {
        var source = ReadSource(Path.Combine("Mud.Wechat.Redis", "Services", "RedisWechatCallbackReplayGuard.cs"));
        var body = ExtractMethod(source, "public async Task<bool> TryMarkAsync(string key", nameof(RedisWechatCallbackReplayGuard));

        body.Should().Contain("WechatRedisErrors.Map", "Redis 异常必须包装上抛（RD3）");
        var swallowReturn = new Regex(@"catch\s*\(([^)]*)\)\s*\{\s*return (true|false)", RegexOptions.Singleline);
        swallowReturn.IsMatch(body).Should().BeFalse("禁止 catch 吞异常返回 true/false");
    }

    /// <summary>
    /// RD-G4：敏感凭据不得进入日志调用点——Redis 包源码中不得出现以 Password / ticket 值 /
    /// PermanentCode / 令牌值为参数的日志调用；连接异常消息只允许脱敏后的 options.ToString()。
    /// </summary>
    [Fact]
    public void SensitiveValues_ShouldNeverAppearInLogCalls()
    {
        var forbiddenPatterns = new Dictionary<string, Regex>
        {
            ["Password"] = new(@"Log(Trace|Debug|Information|Warning|Error|Critical)\([^)]*\.Password", RegexOptions.Singleline),
            ["PermanentCode"] = new(@"Log(Trace|Debug|Information|Warning|Error|Critical)\([^)]*PermanentCode", RegexOptions.Singleline),
            ["ticket 值变量"] = new(@"Log(Trace|Debug|Information|Warning|Error|Critical)\([^)]*\bticket\b(?!Ttl)", RegexOptions.Singleline),
        };

        foreach (var path in GetRedisSourceFiles())
        {
            var source = File.ReadAllText(path);
            foreach (var (name, pattern) in forbiddenPatterns)
            {
                pattern.IsMatch(source).Should().BeFalse(
                    $"{Path.GetFileName(path)} 不得在日志调用点携带{name}");
            }
        }

        // 连接初始化异常消息必须携带脱敏后的配置描述（options.ToString() 已掩码 Password）。
        var extensionSource = ReadSource(Path.Combine("Mud.Wechat.Redis", "Extensions", "WechatRedisServiceCollectionExtensions.cs"));
        extensionSource.Should().Contain("配置：{options}", "连接失败消息只允许携带脱敏后的配置描述");
    }

    /// <summary>
    /// RD-G5：顺序守卫对回调注册的探测全名必须与 Callback 包 InMemory 实现的 FullName 一致
    ///（R-1 后 Redis 包不引用 Callback，经全名字符串探测；漂移即测试失败，须同步更新常量）。
    /// </summary>
    [Fact]
    public void InMemoryReplayGuardTypeName_ShouldMatchCallbackImplementationFullName()
    {
        WechatRedisServiceCollectionExtensions.InMemoryReplayGuardTypeName
            .Should().Be(typeof(InMemoryWechatCallbackReplayGuard).FullName);
    }

    /// <summary>
    /// RD-G7：顺序守卫对公众号注册的探测全名必须与 MP 接口 FullName 一致
    ///（Redis 包不引用 MP.Abstractions，经全名字符串探测；漂移即测试失败，须同步更新常量）。
    /// </summary>
    [Fact]
    public void MpAppManagerTypeName_ShouldMatchOfficialAccountInterfaceFullName()
    {
        WechatRedisServiceCollectionExtensions.MpAppManagerTypeName
            .Should().Be(typeof(IMpAppManager).FullName);

        // 防静默空跑：该类型必须是接口，且与公众号**基座配置面同源**（即落在 Redis 实际依赖的那个基座包上，
        // 而非别处的同名类型）。断言「与锚点同源」而非「程序集叫什么」。
        typeof(IMpAppManager).IsInterface.Should().BeTrue();
        typeof(IMpAppManager).Assembly.Should().BeSameAs(
            typeof(Mud.Wechat.OfficialAccount.Abstractions.Configuration.MpAppConfig).Assembly,
            "RD-G7：IMpAppManager 必须与公众号基座配置面同源，全名探测才落在 Redis 依赖的那个包上");
    }

    /// <summary>
    /// RD-G6：Redis 包的依赖面（RD11）—— 不得引用任何 Callback 包，且**源码不得消费业务客户端面**。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 合并前该不变式由「csproj 只引 <c>Work.Abstractions</c>、不引 <c>Work</c> 主包」提供物理保证。
    /// 2026-10 包合并后企微线的基座与客户端同处 <c>Mud.Wechat.Work</c> 一个程序集，
    /// 程序集维度不再能表达这条边界 —— 而<b>命名空间维度一直能</b>：基座在
    /// <c>Mud.Wechat.Work.Abstractions.*</c>，客户端在 <c>Mud.Wechat.Work</c> /
    /// <c>Mud.Wechat.Work.Interfaces</c> / <c>.Internal</c> / <c>.Services</c>。
    /// 故本守卫改为「csproj 禁引 Callback + 源码禁引客户端命名空间」两段，
    /// 语义与合并前等价（存储适配器只准实现端口与取指标面，不准顺手调业务接口）。
    /// </para>
    /// <para>
    /// 附带说明：Redis 宿主按定义已 <c>AddWechatApp</c>（Redis 只服务于多应用令牌/授权存储），
    /// 因此「引用主包工程」对消费者零新增负担 —— 这正是合并得以成立的前提，而非新缺口。
    /// </para>
    /// </remarks>
    [Fact]
    public void RedisPackage_ShouldReferenceOnlyAbstractions()
    {
        var csproj = ReadSource(Path.Combine("Mud.Wechat.Redis", "Mud.Wechat.Redis.csproj"));

        // ① 包面：企微线只准一个引用（合并后为 Mud.Wechat.Work.csproj），且永不引 Callback。
        csproj.Should().Contain("Mud.Wechat.Work.csproj",
            "RD11：Redis 实现企微线存储端口，必须引企微线工程（2026-10 包合并后为该线唯一工程）");
        csproj.Should().NotContain("Mud.Wechat.Work.Callback.csproj",
            "RD11：重放守卫接口已上移叶层，Redis 不得依赖任何 Callback 包");

        // ② 源码面：不得导入业务客户端命名空间（行锚定 using 探针，文档提及与 fail-fast 文案不误报）。
        var forbiddenUsingPattern = new System.Text.RegularExpressions.Regex(
            @"^\s*(?:global\s+)?using\s+Mud\.Wechat\.Work(?:\.Interfaces|\.Services|\.Extensions|\.Internal)?\s*(?:;|=)",
            RegexOptions.Multiline);

        var hits = Directory
            .EnumerateFiles(SourcePath("Mud.Wechat.Redis"), "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .SelectMany(f => forbiddenUsingPattern.Matches(File.ReadAllText(f))
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => $"{Path.GetFileName(f)}: {m.Value.Trim()}"))
            .ToArray();

        hits.Should().BeEmpty(
            "Redis 是存储适配器，业务客户端面一旦进入其源码即把「存储层」绑到「调用链」上，" +
            $"端口实现与业务编排将无法各自演进。实际命中：{string.Join(", ", hits)}");
    }
}
