// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 业务接口命名空间分区守卫：把「父接口不可直接给最终用户使用」从文档约定升级为可验证不变量。
/// <para>
/// 分区规则（唯一判据为 <see cref="HttpClientApiAttribute.IsAbstract"/>）：
/// 公共父接口（<c>IsAbstract = true</c>，不注册 DI、仅作继承基座）落契约面命名空间
/// <c>Mud.Wechat.Work.Interfaces</c>；可注入接口（子接口与独立端点接口）落运行时面命名空间
/// <c>Mud.Wechat.Work</c>。
/// </para>
/// <para>
/// 该分区使最终用户只 <c>using Mud.Wechat.Work;</c> 时，自动完成列表中不出现任何父接口
/// （父接口不可从 DI 解析，本就不应被直接消费）；同时使「父接口必须与子接口分命名空间」
/// 可被断言锁定，杜绝新增父接口误写回 <c>Mud.Wechat.Work</c> 而静默失去隔离。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 连带契约：源生成器按「接口名去 <c>I</c> 前缀、落 <c>&lt;接口命名空间&gt;.Internal</c>」发射父接口实现类，
/// 子接口经 <c>InheritedFrom</c> 引用该实现类，故父接口迁移命名空间时实现类随之迁入
/// <c>Mud.Wechat.Work.Interfaces.Internal</c>，并须由 <c>Mud.Wechat.Work/GlobalUsings.cs</c>
/// 的 <c>global using</c> 引入。本守卫的 N2 即锁定该生成器契约。
/// </para>
/// <para>
/// 参照架构：<c>D:/Repos/MudFeishu/FeishuV3</c> 采用 <c>Mud.Feishu.Interfaces</c> +
/// <c>Mud.Feishu.Interfaces.Internal</c>；本仓为对齐该约定并额外区分父/子接口面而作此分区。
/// </para>
/// </remarks>
public class WechatInterfaceNamespaceContractGuards
{
    /// <summary>父接口所在契约面命名空间。</summary>
    private const string ContractNamespace = "Mud.Wechat.Work.Interfaces";

    /// <summary>可注入接口所在运行时面命名空间。</summary>
    private const string RuntimeNamespace = "Mud.Wechat.Work";

    /// <summary>生成器发射父接口实现类的命名空间（契约面命名空间的 <c>.Internal</c> 子段）。</summary>
    private const string GeneratedInternalNamespace = ContractNamespace + ".Internal";

    /// <summary>
    /// 取出主包中全部标记 <see cref="HttpClientApiAttribute"/> 的业务接口。
    /// </summary>
    private static List<Type> BusinessInterfaces() =>
        typeof(IWechatWorkAgentService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.GetCustomAttribute<HttpClientApiAttribute>() is not null)
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .ToList();

    // ------------------------------------------------------------------
    // N1：接口命名空间与 IsAbstract 形态严格二分。
    // ------------------------------------------------------------------

    [Fact]
    public void InterfaceNamespace_ShouldPartitionAbstractParentFromInjectable()
    {
        var interfaces = BusinessInterfaces();

        interfaces.Should().NotBeEmpty("主包业务接口均须标记 [HttpClientApi]");

        foreach (var iface in interfaces)
        {
            var isAbstract = iface.GetCustomAttribute<HttpClientApiAttribute>()!.IsAbstract;

            iface.Namespace.Should().Be(isAbstract ? ContractNamespace : RuntimeNamespace,
                $"{iface.Name} 的命名空间须与 IsAbstract 形态一致：父接口（IsAbstract = true）落 {ContractNamespace}，"
                + $"可注入接口落 {RuntimeNamespace}；写错将静默失去「父接口不出现在用户自动完成列表」的隔离");
        }

        // 分区计数锁定：父接口 147 + 可注入接口 299。
        // 两个数字是**聚合报警**，集中维护在 Tests/ContractBaseline.cs（改数字只改那一处）。
        interfaces.Count(t => t.GetCustomAttribute<HttpClientApiAttribute>()!.IsAbstract)
            .Should().Be(Baseline.Work.AbstractParentInterfaces,
                "公共父接口总数漂移须先核对官方开放面，再同批调整 Tests/ContractBaseline.cs");
        interfaces.Count(t => !t.GetCustomAttribute<HttpClientApiAttribute>()!.IsAbstract)
            .Should().Be(Baseline.Work.InjectableInterfaces,
                "可注入接口总数漂移须先核对官方开放面，再同批调整 Tests/ContractBaseline.cs");

        // 双向无残留：两个命名空间内不得混入形态不符的接口。
        typeof(IWechatWorkAgentService).Assembly.GetTypes()
            .Where(t => t.IsInterface
                     && t.Namespace is { } ns
                     && (ns == ContractNamespace || ns == RuntimeNamespace)
                     && t.GetCustomAttribute<HttpClientApiAttribute>() is null)
            .Select(t => t.FullName)
            .Should().BeEmpty("两个业务接口命名空间内不得存在未标记 [HttpClientApi] 的接口");
    }

    // ------------------------------------------------------------------
    // N2：生成器契约——父接口实现类随接口命名空间迁入 .Internal 子段。
    // ------------------------------------------------------------------

    [Fact]
    public void ParentImplementation_ShouldBeGeneratedInContractInternalNamespace()
    {
        var asm = typeof(IWechatWorkAgentService).Assembly;
        var parents = BusinessInterfaces()
            .Where(t => t.GetCustomAttribute<HttpClientApiAttribute>()!.IsAbstract)
            .ToList();

        foreach (var parent in parents)
        {
            // 生成器规则：实现类名 = 接口名去 I 前缀。
            var expectedName = parent.Name[1..];

            var impl = asm.GetType($"{GeneratedInternalNamespace}.{expectedName}");
            impl.Should().NotBeNull(
                $"父接口 {parent.Name} 的生成实现类应为 {GeneratedInternalNamespace}.{expectedName}"
                + $"（生成器规则：接口名去 I 前缀、落 <接口命名空间>.Internal）");

            impl!.Should().BeAssignableTo(parent,
                $"{expectedName} 必须实现 {parent.Name}，否则子接口的 InheritedFrom 继承链断裂");
        }

        // 两个生成命名空间按接口形态干净二分：父接口实现类只落契约面 .Internal，
        // 可注入接口实现类只落运行时面 .Internal（子接口仍在 Mud.Wechat.Work，故其实现类
        // 继续落 Mud.Wechat.Work.Internal——这是预期形态，不得据此判定迁移不彻底）。
        // 判据取「生成器为该实现类配套发射的本体接口」＝ I + 实现类名：
        // 子接口实现类因继承父接口而同时实现父接口，不能用传递性的 GetInterfaces() 判形态。
        var all = BusinessInterfaces().ToDictionary(t => t.Name, StringComparer.Ordinal);
        var generated = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                     && t.Namespace is { } ns
                     && (ns == GeneratedInternalNamespace || ns == RuntimeNamespace + ".Internal"))
            .ToList();

        generated.Should().NotBeEmpty("生成器实现类须存在于两个 .Internal 命名空间之一");

        foreach (var type in generated)
        {
            var contract = type.Namespace == GeneratedInternalNamespace;
            var ownName = "I" + type.Name;

            all.Should().ContainKey(ownName, $"{type.FullName} 须有配套本体接口 {ownName}（实现类名 = 接口名去 I 前缀）");

            var own = all[ownName];
            own.GetCustomAttribute<HttpClientApiAttribute>()!.IsAbstract.Should().Be(contract,
                $"{type.FullName} 位于 {type.Namespace}，其本体接口 {ownName} 的 IsAbstract 形态须与所在生成命名空间一致"
                + $"（父接口实现类落 {GeneratedInternalNamespace}，可注入接口实现类落 {RuntimeNamespace}.Internal）");

            own.Namespace.Should().Be(contract ? ContractNamespace : RuntimeNamespace,
                $"{ownName} 的命名空间与其生成实现类所在命名空间不匹配");
        }

        // 全量覆盖：每个业务接口都须有配套实现类，且恰好一个。
        generated.Select(t => "I" + t.Name).Should().BeEquivalentTo(all.Keys,
            "每个 [HttpClientApi] 接口都须恰好一个生成实现类，无遗漏无多余");
    }

    // ------------------------------------------------------------------
    // N3：隔离有效性——父接口不可从 DI 解析（命名空间分区不得被误当成唯一隔离手段）。
    // ------------------------------------------------------------------

    [Fact]
    public void ParentInterfaces_ShouldRemainUnresolvableFromDi()
    {
        // 命名空间分区只是 IDE 展示层隔离，运行期隔离仍由 IsAbstract 不注册 DI 提供；
        // 此处锁定「父接口集合非空且全部标记 IsAbstract」，避免有人误以为改名即可放开直接调用。
        var parents = BusinessInterfaces()
            .Where(t => t.GetCustomAttribute<HttpClientApiAttribute>()!.IsAbstract)
            .ToList();

        parents.Should().NotBeEmpty("父接口集合不得为空");

        foreach (var parent in parents)
        {
            parent.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName
                .Should().BeNullOrEmpty(
                    $"{parent.Name} 不进入注册组（注册面由子接口承载），否则将可被直接解析调用");
        }
    }
}
