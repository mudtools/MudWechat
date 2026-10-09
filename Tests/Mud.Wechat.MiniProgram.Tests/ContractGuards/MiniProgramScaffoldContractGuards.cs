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
/// <b>本组守卫为脚手架期形态</b>：小程序程序集当前**零端点**（P1 才落端点）。因此「重叠路由回潮」类
/// 断言此刻恒真，但其参照集（公众号线路由）被强制断言为**非空**——参照集为空时守卫会**失败**而非
/// 静默通过，避免反射失效导致的假绿（AGENTS §6「数量下限防枚举空跑」同款纪律）。
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
    /// 不采用 Work 线的父子二分）。当前零接口，断言恒成立；一旦有人引入 <c>IsAbstract</c> 父接口即失败。
    /// </summary>
    [Fact]
    public void Interfaces_ShouldNotUseAbstractParents_WhenScaffold()
    {
        var asm = LoadProductLine("Mud.Wechat.MiniProgram");

        var abstractParents = asm.GetTypes()
            .Where(t => t.IsInterface && t.IsAbstract)
            .Where(t => t.GetCustomAttributes(false)
                .Any(a => a.GetType().Name is "HttpClientApiAttribute" or "TokenAttribute"))
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
        var csproj = Path.Combine(Root, "Mud.Wechat.MiniProgram", "Mud.Wechat.MiniProgram.csproj");
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
        var dir = Path.Combine(Root, "Mud.Wechat.MiniProgram.Callback");
        Directory.Exists(dir).Should().BeFalse(
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
