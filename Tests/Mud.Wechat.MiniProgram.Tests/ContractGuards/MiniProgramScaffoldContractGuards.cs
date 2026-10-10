// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线**脚手架期**架构守卫（方案 P0-a / MP-X1、X2、X4、X6 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要本守卫</b>：方案 v1 曾把「小程序线」按 <c>IsAbstract</c> 父子接口 + 独立令牌类型建模，
/// 经四视角评审否决（见方案 §9 修订 R2/R5）。这些否决结论必须在**实现期之前**锁死，否则 P1 落地时
/// 很容易「顺手」重新引入已被否决的形态——而 <c>MpTokenManagerRegistry</c> 是 <c>internal sealed</c>
/// 单槽、按 <c>MpTokenTypes.AccessToken</c> 键控，新增令牌类型会**静默**让 errcode 令牌恢复返回 null
/// （恢复链空转、无异常），属于最危险的一类漂移。
/// </para>
/// <para>
/// <b>本组守卫的形态已随 P1-c 落地演进</b>：脚手架期小程序程序集**零端点**，「重叠路由回潮」类断言恒真；
/// P1-c 起本线已有 84 端点，断言<b>真正生效</b>——参照集（公众号线路由）仍被强制断言为<b>非空</b>：
/// 参照集为空时守卫会**失败**而非静默通过，避免反射失效导致的假绿
/// （AGENTS §6「数量下限防枚举空跑」同款纪律）。
/// </para>
/// <para>
/// <b>本文件只保留「形态类」断言</b>（令牌复用 / 无 IsAbstract 父接口 / 重叠路由 / 依赖隔离 / 无回调包）；
/// 端点计数、路由表、字段名、白名单与装配面等<b>契约类</b>断言集中在
/// <c>MiniProgramContractGuards</c>（MP-X1/X2/X3/X5/X6/X7/X8）。
/// </para>
/// </remarks>
public class MiniProgramScaffoldContractGuards
{
    /// <summary>
    /// MP-X2：令牌类型复用 —— 小程序线必须复用 <c>MpTokenTypes.AccessToken</c>，
    /// **禁止**出现 <c>MiniProgramTokenTypes</c>（或任何 <c>Wechat.MiniProgram.*</c> 令牌类型）。
    /// </summary>
    [Fact]
    public void TokenTypes_ShouldReuseMpTokenTypes_WhenScaffold()
    {
        // 复用目标必须是既存的、值恒定的公众号令牌键。
        MpTokenTypes.AccessToken.Should().Be("Wechat.Mp.AccessToken",
            "小程序线复用公众号 access_token 键，改值会让既有令牌缓存整体失效");

        // 同批锁：不得新增小程序自有令牌类型类（命名约定即闸门）。
        var asm = typeof(MpTokenTypes).Assembly;
        var forbidden = asm.GetTypes()
            .Where(t => t.IsClass && t.IsPublic && t.Name.Contains("MiniProgramTokenTypes", StringComparison.Ordinal))
            .ToArray();
        forbidden.Should().BeEmpty("MiniProgramTokenTypes 会让 MpTokenManagerRegistry 单槽 Resolve 失效");

        // 小程序程序集内亦不得自立令牌类型常量类。
        var mpAsm = LoadProductLine("Mud.Wechat.MiniProgram");
        mpAsm.GetTypes()
            .Where(t => t.IsClass && t.IsPublic && t.Name.EndsWith("TokenTypes", StringComparison.Ordinal))
            .Should().BeEmpty("小程序线不得自建令牌类型常量，统一走 MpTokenTypes");
    }

    /// <summary>
    /// MP-X4：接口形态 —— 小程序线**平铺命名空间、无 <c>IsAbstract</c> 公共父接口**（对齐公众号线形态，
    /// 不采用 Work 线的父子二分）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>判据是「特性属性」而非 <c>Type.IsAbstract</c></b>：反射里 <c>Type.IsAbstract</c> 对
    /// <b>接口恒为 <c>true</c></b>（接口本就是抽象类型），照它判定会让本守卫<b>永远为红</b> ——
    /// 反过来说，脚手架期「零接口」时它又恒为绿，属<b>双向失效</b>的写法（P1-c 落地时实测踩到）。
    /// 真正的判据是 <c>[HttpClientApi(IsAbstract = true)]</c> 的特性属性（决定是否进 DI 注册组）。
    /// </para>
    /// <para>P1-c 起本线已有 5 个接口（4 带令牌 + 1 免令牌），断言由此真正生效。</para>
    /// </remarks>
    [Fact]
    public void Interfaces_ShouldNotUseAbstractParents_WhenContractLanded()
    {
        var asm = LoadProductLine("Mud.Wechat.MiniProgram");

        var abstractParents = asm.GetTypes()
            .Where(static t => t.IsInterface)
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name is "HttpClientApiAttribute" or "TokenAttribute"
                                 && a.GetType().GetProperty("IsAbstract")?.GetValue(a) is true))
            .ToArray();

        abstractParents.Should().BeEmpty(
            "小程序线不采用 IsAbstract 公共父接口形态（方案 §4 MP-X4）；如需改造须先改方案并同步更新本守卫");
    }

    /// <summary>
    /// MP-X6：重叠零回潮 —— 已判定为「与公众号线重复」的 18 条路由，**不得**在小程序程序集内再次声明。
    /// </summary>
    /// <remarks>
    /// 参照集 = 公众号程序集上 <c>[RequestUri]</c> 的去重集合，强制非空（防反射空跑假绿）。
    /// 小程序侧当前为空集合 ⇒ 断言恒真；P1 落端点后本守卫立即生效。
    /// </remarks>
    [Fact]
    public void OverlappingRoutes_ShouldNotBeDeclared_WhenMpLineAddsEndpoints()
    {
        var oaRoutes = CollectRequestUris(typeof(MpTokenTypes).Assembly);
        oaRoutes.Should().NotBeEmpty("公众号线路由是本守卫的参照集，为空说明反射口径失效，守卫会假绿");

        var mpRoutes = CollectRequestUris(LoadProductLine("Mud.Wechat.MiniProgram"));

        var overlap = mpRoutes.Intersect(oaRoutes, StringComparer.Ordinal).ToArray();
        overlap.Should().BeEmpty(
            "小程序线不得重复声明公众号线已有的路由（方案 §4 MP-X6）：" +
            string.Join(", ", overlap));
    }

    /// <summary>
    /// MP-X1 / MP-X5：工程引用隔离 —— 小程序线只允许向上引用 <c>Mud.Wechat.Abstractions</c>，
    /// **禁止**引用公众号主包 / DataModels / Callback。
    /// </summary>
    [Fact]
    public void ProjectReferences_ShouldNotDependOnOfficialAccountLine_WhenScaffold()
    {
        var csproj = SourcePath("Mud.Wechat.MiniProgram", "Mud.Wechat.MiniProgram.csproj");
        File.Exists(csproj).Should().BeTrue($"工程文件缺失：{csproj}");

        var xml = File.ReadAllText(csproj);
        foreach (var banned in new[]
                 {
                     "Mud.Wechat.OfficialAccount.csproj",
                     "Mud.Wechat.OfficialAccount.DataModels.csproj",
                     "Mud.Wechat.OfficialAccount.Callback.csproj",
                 })
        {
            xml.Should().NotContain(banned,
                "小程序线与公众号线必须平行，只允许共享 Abstractions 叶层（方案 §4 MP-X1）");
        }
    }

    /// <summary>
    /// MP-X4 补充：小程序线**不得**存在独立回调包（消息接收走公众号线既有 XML 通道）。
    /// </summary>
    [Fact]
    public void CallbackProject_ShouldNotExist_WhenScaffold()
    {
        Directory.EnumerateFiles(Root, "Mud.Wechat.MiniProgram.Callback.csproj", SearchOption.AllDirectories)
            .Should().BeEmpty(
                "小程序线不建独立回调包；如方案变更须先改方案 §5 并同步更新本守卫与 CB-L1g");
    }

    // ---- helpers -------------------------------------------------------------

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
