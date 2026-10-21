using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Mud.Wechat.AI.Tests.ContractGuards;

/// <summary>
/// 产品环境量纪律守卫：AI 面的异步流上下文<b>只能</b>来自中立 <c>AmbientScope&lt;T&gt;</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须由门禁守</b>：自写 <c>AsyncLocal&lt;T&gt;</c> 的经典形态是「赋值 → finally 复位」。它在没有嵌套时
/// 完全正确，一旦出现嵌套（续跑轮嵌在事件轮之后、工具作用域嵌在回合作用域里）就会在<b>乱序释放</b>时把外层值抹成
/// <see langword="null"/> 或旧值——表现是「同一轮里工具读到别的租户」，属多租户越权读取，且只在少数路径复现。
/// </para>
/// <para>
/// <b>替代品是唯一解</b>：中立 <c>AmbientScope&lt;T&gt;</c> 提供栈式恢复 + 所有权校验（乱序释放不回滚内层），
/// 产品实现只做「薄壳声明 T 与生命周期」，不自建存储。
/// </para>
/// <para>
/// <b>判据精度</b>：只认代码形态 <c>new AsyncLocal&lt;</c>——注释里提到这个名字是合法的（说明文字必须能写清
/// 「为什么不用它」）。
/// </para>
/// </remarks>
public class WechatAmbientScopeGuards
{
    [Fact]
    public void AiSurface_ShouldNotDeclareRawAsyncLocal()
    {
        var pattern = new Regex(@"new\s+AsyncLocal\s*<", RegexOptions.CultureInvariant);
        var root = RepoRoot();

        var offenders = Directory
            .EnumerateFiles(Path.Combine(root, "Src", "AI"), "*.cs", SearchOption.AllDirectories)
            .Where(static p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => pattern.IsMatch(File.ReadAllText(path)))
            .Select(path => Path.GetRelativePath(root, path))
            .ToList();

        offenders.Should().BeEmpty(
            "AI 面的环境量必须走中立 AmbientScope<T>（栈式恢复 + 所有权校验）；裸 AsyncLocal + 手工复位在嵌套场景会覆盖外层值");
    }

    [Fact]
    public void CorpContextAccessor_ShouldBeThinShellOverAmbientScope()
    {
        var path = Path.Combine(RepoRoot(), "Src", "AI", "Mud.Wechat.Work.AI", "Agents", "IWechatAiCorpContextAccessor.cs");
        var source = File.ReadAllText(path);

        source.Should().Contain("AmbientScope<WechatToolPrincipal>",
            "产品环境量是中立 AmbientScope 的薄壳（只声明 T 与生命周期），不自建存储");
        source.Should().Contain("IDisposable Begin(",
            "作用域形态入口必须存在：手工「赋值 → 复位」无法表达嵌套还原");
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}
