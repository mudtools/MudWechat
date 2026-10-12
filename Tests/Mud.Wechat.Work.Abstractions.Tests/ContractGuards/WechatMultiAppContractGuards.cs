// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Text;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Tests.ContractGuards;

/// <summary>
/// 多应用管理域契约守卫（MA1~MA6，多应用域方案 §8.1 / 评审 MR13 / 缺陷修复方案 P2-1、P2-3）：
/// 钉死 M8 删除顺序、M4 重建白名单收敛（P2-3 聚合展开）、M3 停机竞态闸、M7 SetCorp 前置校验、
/// P0-1 企业令牌 scope 汇合点、P2-1 重建节流记账时机与 RemoveApp 记账清理的关键约束防回归。
/// 源码文本守卫沿用 G9 / CB1~CB4 先例；方法体提取（花括号配平）防跨方法误判。
/// </summary>
public class WechatMultiAppContractGuards
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

    /// <summary>
    /// 按「签名标记 → 花括号配平」提取方法体（含签名到闭括号的全文）。
    /// 仅适用于方法体内字符串字面量不含 ASCII 花括号的方法（当前 4 个守卫目标均满足）。
    /// </summary>
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

    /// <summary>
    /// MA1（M8/F8）：<c>WechatAppManager.RemoveApp</c> 方法体内 <c>_lazyContexts.TryRemove</c>
    /// 必须先于 <c>_configs.Remove</c>——锁定「返回 false ⇒ 零突变」，防回归引入自造 desync
    ///（失败早退路径留下「配置已删、Lazy 尚在」的非法状态）。
    /// </summary>
    [Fact]
    public void RemoveApp_ShouldRemoveLazyBeforeConfig()
    {
        var source = ReadSource(Path.Combine(
            "Mud.Wechat.Work.Abstractions", "Authentication", "MultiApp", "WechatAppManager.cs"));
        var removeAppBody = ExtractMethod(source, "public bool RemoveApp(string appKey)", nameof(WechatAppManager));

        var tryRemoveIndex = removeAppBody.IndexOf("_lazyContexts.TryRemove", StringComparison.Ordinal);
        var configRemoveIndex = removeAppBody.IndexOf("_configs.Remove", StringComparison.Ordinal);

        tryRemoveIndex.Should().BePositive("MA1：RemoveApp 必须经 _lazyContexts.TryRemove 判定存在性");
        configRemoveIndex.Should().BePositive("MA1：RemoveApp 必须成对删除 _configs 条目");
        tryRemoveIndex.Should().BeLessThan(configRemoveIndex,
            "MA1：TryRemove 必须先于配置删除（M8：失败早退路径零突变，防「配置已删、Lazy 尚在」desync）");
    }

    /// <summary>
    /// MA2（M4/F4）：<c>IsTransientInitFailure</c> 方法体不得包含 IOE 白名单判定——
    /// 装配路径的确定性 DI 失败（GetRequiredService / 命名客户端缺失）留在白名单内会把配置错误
    /// 伪装成可重试并驱动 5s 节流反复重建（叠加 F1 即慢性泄漏）。
    /// </summary>
    [Fact]
    public void IsTransientInitFailure_ShouldNotWhitelistDeterministicDiFailures()
    {
        var source = ReadSource(Path.Combine(
            "Mud.Wechat.Work.Abstractions", "Authentication", "MultiApp", "WechatAppManager.cs"));
        var methodBody = ExtractMethod(source, "private static bool IsTransientInitFailure(Exception ex)", nameof(WechatAppManager));

        methodBody.Should().NotContain("InvalidOperationException",
            "MA2：IOE 不得回到瞬时白名单（M4：装配路径 IOE 全为 DI 确定性失败，直抛不重建）");
        methodBody.Should().Contain("HttpRequestException",
            "MA2：瞬时白名单限于 IO 型异常组（HttpRequestException/TimeoutException/IOException/SocketException）");
        methodBody.Should().Contain("OperationCanceledException",
            "MA2：取消异常必须显式排除在白名单外（取消不得驱动重建）");
        methodBody.Should().Contain("AggregateException",
            "MA2/P2-3：聚合形态必须逐项展开 InnerExceptions（DI/装配可能聚合多内因）");
        methodBody.Should().NotContain("ex.InnerException",
            "MA2/P2-3：不得对单 InnerException 做通用递归（「确定性外因包裹瞬时内因」会被误判为瞬时）");
    }

    /// <summary>
    /// MA6（P2-1）：<c>TryRebuildLazy</c> 只在 <c>_lazyContexts.TryUpdate</c> <b>成功</b>后才记账节流窗口
    /// （配置缺失 / CAS 竞争失败等未生效的重建不得消耗窗口），且 <c>RemoveApp</c> 清除该 appKey 的记账
    /// （防 <c>_lastRebuildMs</c> 无界增长 + 残留记账抑制复用键的新应用重建）。
    /// </summary>
    [Fact]
    public void TryRebuildLazy_ShouldAccountThrottleOnlyAfterSuccessfulSwap()
    {
        var source = ReadSource(Path.Combine(
            "Mud.Wechat.Work.Abstractions", "Authentication", "MultiApp", "WechatAppManager.cs"));

        var rebuildBody = ExtractMethod(source,
            "private bool TryRebuildLazy(string appKey, Lazy<IWechatAppContext> failed, Exception cause)",
            nameof(WechatAppManager));
        var swapIndex = rebuildBody.IndexOf("_lazyContexts.TryUpdate", StringComparison.Ordinal);
        var recordIndex = rebuildBody.IndexOf("_lastRebuildMs[appKey] = nowMs", StringComparison.Ordinal);

        swapIndex.Should().BePositive("MA6：重建必须经 Lazy 表 CAS 换新");
        recordIndex.Should().BeGreaterThan(swapIndex,
            "MA6（P2-1）：只有 CAS 成功换上新 Lazy 才记账——无效重建消耗节流窗口会使真实故障的重建被误节流");

        var removeBody = ExtractMethod(source, "public bool RemoveApp(string appKey)", nameof(WechatAppManager));
        removeBody.Should().Contain("_lastRebuildMs.TryRemove(appKey",
            "MA6（P2-1）：RemoveApp 必须清除节流记账（防无界增长）");
    }

    /// <summary>
    /// MA3（M3/F3）：<c>WechatAppContextRetirement.Enqueue</c> 方法体必须含 <c>_disposed</c> 闸
    /// 且落闸时立即 <c>Dispose</c> 上下文——停机后到达的入队若落进已停摆队列即永久泄漏。
    /// </summary>
    [Fact]
    public void RetirementEnqueue_ShouldGateOnDisposed()
    {
        var source = ReadSource(Path.Combine(
            "Mud.Wechat.Work.Abstractions", "Authentication", "MultiApp", "WechatAppContextRetirement.cs"));
        var enqueueBody = ExtractMethod(source, "public void Enqueue(string appKey, IWechatAppContext context)", nameof(WechatAppContextRetirement));

        var gateIndex = enqueueBody.IndexOf("_disposed != 0", StringComparison.Ordinal);
        var disposeIndex = enqueueBody.IndexOf("context.Dispose()", StringComparison.Ordinal);

        gateIndex.Should().BePositive("MA3：Enqueue 必须含停机闸（EnqueueCleanup 已有同款，M3 消除不对称）");
        disposeIndex.Should().BePositive("MA3：落闸时必须立即 Dispose 上下文（不入死队列，防永久泄漏）");
        gateIndex.Should().BeLessThan(enqueueBody.IndexOf("_retireDelay", StringComparison.Ordinal),
            "MA3：停机闸必须先于宽限期判定（停机语义优先于一切入队路径）");
    }

    /// <summary>
    /// MA4（M7/F7）：<c>WechatCorpContext.SetCorp</c> 方法体必须含 null 与空白校验——
    /// authCorpId 是企业令牌 scope 的唯一来源，空白值会延迟到首次令牌刷新才报错。
    /// </summary>
    [Fact]
    public void SetCorp_ShouldValidateAuthCorpId()
    {
        var source = ReadSource(Path.Combine(
            "Mud.Wechat.Work.Abstractions", "Authentication", "MultiApp", "WechatCorpContext.cs"));
        var setCorpBody = ExtractMethod(source, "public static void SetCorp(string? appKey, string authCorpId, string? permanentCode)", nameof(WechatCorpContext));

        setCorpBody.Should().Contain("ArgumentNullException(nameof(authCorpId))",
            "MA4：authCorpId 为 null 必须在写入点抛 ArgumentNullException（M7）");
        setCorpBody.Should().Contain("string.IsNullOrWhiteSpace(authCorpId)",
            "MA4：空白串同拒（MR9：空白 CorpId 无法成为合法 scope）");
    }

    /// <summary>
    /// MA5a（P0-1）：<c>CorpTokenManager</c> 必须覆写<b>唯一汇合点</b>
    /// <c>GetOrRefreshTokenAsync(CancellationToken)</c>，把环境 authCorpId 升级为一等 scope
    /// （方法体内须同时出现环境解析与「以非空 scopes 调带 scope 重载」两要素）。
    /// </summary>
    [Fact]
    public void CorpTokenManager_ShouldLiftAmbientScopeOnConvergencePoint()
    {
        var source = ReadCorpTokenManagerSource();
        var body = ExtractMethod(source,
            "public override Task<string> GetOrRefreshTokenAsync(CancellationToken cancellationToken = default)",
            nameof(CorpTokenManager));

        body.Should().Contain("ResolveAmbientScopeOrNull",
            "MA5a：无 scope 入口必须经环境解析取得 authCorpId（R9 归属闸同源）");
        body.Should().Contain("base.GetOrRefreshTokenAsync(new[] { authCorpId",
            "MA5a：解析到 authCorpId 后必须以非空 scopes 调带 scope 重载（键第三段 = authCorpId）");
        body.Should().Contain("string.IsNullOrEmpty(authCorpId)",
            "MA5a：无环境企业上下文时须显式回退基类兜底（fail-fast 文案不变）");

        var helperBody = ExtractMethod(source, "private string? ResolveAmbientScopeOrNull()", nameof(CorpTokenManager));
        helperBody.Should().Contain("Options.AppKey",
            "MA5a：环境解析必须含 R9 归属校验（与 RefreshTokenWithScopesAsync 同源、不弱化）");
    }

    /// <summary>
    /// MA5b（P1-1）：<c>CorpTokenManager</c> 必须覆写 <c>InvalidateTokenAsync</c>，
    /// 且「显式 scopes 优先」分支在环境解析之前。
    /// </summary>
    [Fact]
    public void CorpTokenManager_ShouldScopeInvalidateByAmbientContext()
    {
        var source = ReadCorpTokenManagerSource();
        var body = ExtractMethod(source,
            "public override Task<TokenResult> InvalidateTokenAsync(string[]? scopes = null, CancellationToken cancellationToken = default)",
            nameof(CorpTokenManager));

        var explicitBranchIndex = body.IndexOf("scopes is { Length: > 0 }", StringComparison.Ordinal);
        var ambientIndex = body.IndexOf("ResolveAmbientScopeOrNull", StringComparison.Ordinal);

        explicitBranchIndex.Should().BePositive("MA5b：显式 scopes 分支必须存在（行为不变）");
        ambientIndex.Should().BePositive("MA5b：无 scopes 时必须按环境解析补齐 scope（防失效空转）");
        explicitBranchIndex.Should().BeLessThan(ambientIndex,
            "MA5b：显式 scopes 优先于环境解析（调用方显式意图不得被环境覆盖）");
        body.Should().Contain("base.InvalidateTokenAsync(new[] { authCorpId",
            "MA5b：解析到 authCorpId 后须以非空 scopes 走带 scope 失效（清掉 corpId 槽）");
    }

    /// <summary>
    /// MA5c（反向断言，防回潮）：<c>CorpTokenManager</c> <b>不得</b>覆写
    /// <c>GetTokenAsync(CancellationToken)</c>（基座已委托到汇合点，双覆写只会带来方法体漂移），
    /// 且汇合点覆写体不得退化为「无 scope 直通唯一返回路径」。
    /// </summary>
    [Fact]
    public void CorpTokenManager_ShouldNotDoubleOverrideOrBypassScope()
    {
        var source = ReadCorpTokenManagerSource();

        source.Should().NotContain("public override Task<string> GetTokenAsync(",
            "MA5c：基座 GetTokenAsync 已委托到 GetOrRefreshTokenAsync(ct)，本类再覆写即为双份方法体（冗余 + 漂移风险）");

        var body = ExtractMethod(source,
            "public override Task<string> GetOrRefreshTokenAsync(CancellationToken cancellationToken = default)",
            nameof(CorpTokenManager));

        body.Should().Contain("base.GetOrRefreshTokenAsync(new[] { authCorpId",
            "MA5c：覆写体不得退化为无 scope 直通（必须存在带 scope 的委托路径）");
    }

    private static string ReadCorpTokenManagerSource()
        => ReadSource(Path.Combine(
            "Mud.Wechat.Work.Abstractions", "Authentication", "TokenManager", "CorpTokenManager.cs"));
}
