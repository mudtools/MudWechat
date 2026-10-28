// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.V2.Extensions;

namespace Mud.Wechat.Pay.ContractGuards;

/// <summary>
/// 微信支付 APIv2 线的<b>聚合基线</b>守卫（接口二分形态 / 端点总数 / 去重路由数 / 模块成员数）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本守卫只锁「汇总数」，不锁任何域内细节</b>（逐族路由、字段表、签名形态、证书域名由各自的
/// <c>WechatPayV2{域}ContractGuards</c> 持有）。集中一处是为了让「有没有东西悄悄多出来 / 少掉」
/// 变成一次反射推导即可判定的事，而不必跨 8 个域守卫去加总。数字本体在
/// <c>Tests/ContractBaseline.cs</c> 的 <c>Baseline.PayV2</c>，改数只改那里一处。
/// </para>
/// <para>
/// <b>为什么 APIv2 与 APIv3 分列两个基线</b>：APIv3 全部走 <c>[HttpClientApi]</c> 源生成，APIv2 是
/// 「免证书族声明式 + 需证书族手写」<b>两形态并列</b>（证书按商户绑定，与命名客户端「一名一连接池」的
/// 复用模型冲突）。并成一个总数后，任一侧的漂移都会被另一侧的存量掩盖，故 <c>Baseline.Pay</c> 与
/// <c>Baseline.PayV2</c> 各锁各的程序集。
/// </para>
/// <para>
/// <b>接口二分的判据是特性、不是目录</b>：<c>Mud.Wechat.Pay.V2</c> 命名空间下的公开接口恰 25 支，
/// 带 <c>[HttpClientApi]</c> 者 12（声明式）、不带者 13（手写）。同程序集内 <c>.Credential</c> /
/// <c>.Transport</c> 的三支是基础设施端口（凭据桥接 + 命名客户端标记），不是端点接口 ⇒ 由命名空间
/// 过滤排除，<b>不得</b>为凑数把它们算进来。
/// </para>
/// </remarks>
public class WechatPayV2BaselineContractGuards
{
    /// <summary>
    /// V2-B1：注入面二分不漂移 —— 声明式接口数与手写接口数同时锁，且两个子集之和必须等于全集
    /// （判据只有「有无 <c>[HttpClientApi]</c>」一条，漏一支即判据被绕过）。
    /// </summary>
    [Fact]
    public void V2_Baseline_Interfaces_ShouldSplitIntoDeclarativeAndHandWritten()
    {
        var declarative = DeclarativeInterfaces();
        var handWritten = HandWrittenInterfaces();

        declarative.Should().HaveCount(Baseline.PayV2.HttpApiInterfaces,
            "because 免证书族的声明式接口数是注册面与 DTO 登记面的加法基础");
        handWritten.Should().HaveCount(Baseline.PayV2.HandWrittenInterfaces,
            "because 需证书族只能手写：特性一挂即由源生成客户端在无证书连接池上发出该端点");

        ServiceInterfaces().Should().HaveCount(declarative.Count + handWritten.Count,
            "because 判据只有「有无 [HttpClientApi]」一条 ⇒ 两个子集之和必须等于全集");
    }

    /// <summary>
    /// V2-B2：端点总数与去重路由数两个口径同时锁。二者相差 1 是官方事实 ——
    /// <c>/papay/deletecontract</c> 在「按协议号解约」与「按用户标识解约」两页各有独立请求表 ⇒ 同一路由两支方法。
    /// </summary>
    [Fact]
    public void V2_Baseline_EndpointsAndRoutes_ShouldMatchAggregates()
    {
        var declarativeEndpoints = DeclarativeInterfaces()
            .Sum(static t => RoutesOf(t).Count);
        var handWrittenEndpoints = HandWrittenInterfaces().Sum(static t => DeclaredMethods(t).Length);

        declarativeEndpoints.Should().Be(39,
            "because 声明式逐域端点表由各域守卫持有，此处只锁加总（BasePay 8 + Deposit 5 + Papay 6 + PartnerPapay 5 + Customs 3 + Tools 2 + Vehicle 3 + PartnerVehicle 3 + Transit 1 + PartnerTransit 1 + PayItil 1 + Xdc 1）");
        (declarativeEndpoints + handWrittenEndpoints).Should().Be(Baseline.PayV2.Endpoints,
            "because 端点按 HTTP 方法计：声明式取 [Post]/[Get] 实例数，手写取接口声明方法数");

        // 路由面：声明式来自特性，手写来自实现源文本（手写族无特性载体，源文本是唯一的路由出处）。
        var routes = DeclarativeInterfaces().SelectMany(static t => RoutesOf(t))
            .Concat(HandWrittenRoutes())
            .ToList();

        routes.Should().HaveCount(Baseline.PayV2.Endpoints,
            "because 每支端点都必须解得出一条路由；手写族解析数与接口方法数不等，即说明实现与接口脱钩或 Secure/ 源文本混入了路由形状的字面量");

        routes.Distinct().Should().HaveCount(Baseline.PayV2.DistinctRoutes,
            "because 去重前后只差 1 条，且那条重复是官方两页同路由的既定事实");

        routes.GroupBy(static r => r, StringComparer.Ordinal)
            .Where(static g => g.Count() > 1)
            .Select(static g => g.Key)
            .Should().Equal(new[] { "/papay/deletecontract" },
                "because 全线唯一的路由复用即 PAPay 双解约支 —— 出现第二条即说明有人新增了同路由方法而未登记");
    }

    /// <summary>
    /// V2-B3：模块成员数 = 生成注册组 ∪ 手写白名单。
    /// 成员数（23）<b>小于</b>接口数（25）是「一域两形态」的必然结果：押金需证书三支复用 <c>DepositV2</c>、
    /// 基础支付证书五支复用 <c>BasePayV2</c>。
    /// </summary>
    [Fact]
    public void V2_Baseline_Modules_ShouldBeGeneratedGroupsUnionHandWrittenWhitelist()
    {
        var members = Enum.GetNames(typeof(PayV2Module));
        members.Should().HaveCount(Baseline.PayV2.Modules,
            "because 模块成员是「生成注册组 ∪ 手写白名单」的并集，加一支族必同时多一个成员");

        var generatedGroups = DeclarativeInterfaces()
            .Select(static t => t.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .ToList();

        // 一域一接口：声明式接口数与注册组数相等 ⇒ 本线不存在「一组多接口」（那是 APIv3 收付通的独有形态）。
        generatedGroups.Should().OnlyHaveUniqueItems(
            "because APIv2 每支声明式接口独占一个注册组；出现一组多接口即与本线的族划分口径冲突");
        generatedGroups.Should().HaveCount(Baseline.PayV2.HttpApiInterfaces);

        members.Should().BeEquivalentTo(
            generatedGroups.Concat(WechatPayV2CertFamilyContractGuards.HandWrittenModules),
            "because 成员既不得多（凭空出现无注册器的成员 ⇒ 静默不注册）也不得少（漏认领同理）");

        WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().HaveCount(11,
            "because 手写白名单认领的是「模块成员」而非「接口」：13 支手写接口只对应 11 个成员，另两支随既存域成员注册");
    }

    /// <summary>
    /// V2-B4：汇总数必须有载体 —— 每支声明式接口的注册组名须是 <c>PayV2Module</c> 成员且带 <c>V2</c> 后缀，
    /// 每支手写接口须在 <c>Secure/</c> 下有同名实现类源文件。
    /// </summary>
    [Fact]
    public void V2_Baseline_EveryFamily_ShouldHaveItsOwnCarrier()
    {
        foreach (var declarative in DeclarativeInterfaces())
        {
            var registryGroup = declarative.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName;
            registryGroup.Should().EndWith("V2",
                $"{declarative.Name} 的注册组名须与 PayV2Module 成员同名（APIv3 组名无 V2 后缀，两线组名不得互认）");
            Enum.TryParse<PayV2Module>(registryGroup, out _).Should().BeTrue(
                $"{declarative.Name} 的注册组 {registryGroup} 必须是 PayV2Module 成员");
            DeclaredMethods(declarative).Should().NotBeEmpty($"{declarative.Name} 不得是空接口占位");
        }

        var secureSources = Directory.EnumerateFiles(
                Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Secure"), "*.cs")
            .Select(static f => Path.GetFileNameWithoutExtension(f))
            .ToList();

        foreach (var handWritten in HandWrittenInterfaces())
        {
            secureSources.Should().Contain(handWritten.Name.Substring(1),
                $"{handWritten.Name} 是手写接口，须在 Secure/ 下有同名实现类 —— 否则基线的「手写」计数没有载体");
        }
    }

    /// <summary>本线「可注入端点接口」全集：主程序集内命名空间恰为 <c>Mud.Wechat.Pay.V2</c> 的公开接口。</summary>
    private static List<Type> ServiceInterfaces()
        => typeof(IWechatPayV2Service).Assembly.GetTypes()
            .Where(static t => (t.Namespace ?? string.Empty).StartsWith("Mud.Wechat.Pay.V2", StringComparison.Ordinal))
            .Where(static t => t.IsInterface && t.IsPublic && t.Namespace == "Mud.Wechat.Pay.V2")
            .ToList();

    private static List<Type> DeclarativeInterfaces()
        => ServiceInterfaces()
            .Where(static t => t.GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false).Any())
            .ToList();

    private static List<Type> HandWrittenInterfaces()
        => ServiceInterfaces()
            .Where(static t => !t.GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false).Any())
            .ToList();

    private static MethodInfo[] DeclaredMethods(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static List<string> RoutesOf(Type interfaceType)
        => DeclaredMethods(interfaceType)
            .Select(static m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(static r => r != null)
            .Select(static r => r!)
            .ToList();

    /// <summary>
    /// 手写族的路由只能从实现源文本取（无特性载体）。取「路由形状的字面量」：以 <c>/</c> 开头、
    /// 仅由字母数字与 <c>/_-</c> 组成、且出现在 <c>Secure/</c> 目录下的引号串。
    /// </summary>
    private static List<string> HandWrittenRoutes()
    {
        var pattern = new Regex("\"(/[A-Za-z0-9_/-]+)\"", RegexOptions.Compiled);
        var routes = new List<string>();

        foreach (var file in Directory.EnumerateFiles(
                     Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Secure"), "*.cs"))
        {
            foreach (System.Text.RegularExpressions.Match match in pattern.Matches(File.ReadAllText(file)))
            {
                routes.Add(match.Groups[1].Value);
            }
        }

        return routes;
    }

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

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
}
