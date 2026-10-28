// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 支付线接口命名空间分区守卫（追踪号 PAY-INTERFACE-NAMESPACE-2026-10-15，形态对齐企微线
/// <c>WechatInterfaceNamespaceContractGuards</c> 的 N1~N3）。
/// <para>
/// <b>为何支付线要单独成一支</b>：企微线的 N1~N3 扫描根是 <c>typeof(IWechatWorkAgentService).Assembly</c>
/// （企微主包），支付线的 <c>[HttpClientApi]</c> 接口<b>不在其覆盖面内</b>——历史上正因如此，
/// <c>IWechatPayMerchantManageService</c> 与 <c>IWechatPayMerchantOperationManageService</c> 两支
/// 漂到了 <c>Mud.Wechat.Pay.Interfaces.{域}</c> 而全绿无人拦截（2026-10-15 已收敛回 <c>Mud.Wechat.Pay</c>）。
/// </para>
/// <para>
/// <b>支付线口径与企微线不同，且是刻意的</b>：本线<b>没有</b>「公共父接口（<c>IsAbstract = true</c>）」形态，
/// 全部 <c>[HttpClientApi]</c> 接口都是可直接注入的运行时面 ⇒ 恒落 <c>Mud.Wechat.Pay</c>；
/// 手工通道服务（multipart 上传、本地签名、证书下载等）另落 <c>Mud.Wechat.Pay.{域}</c>
/// （如 <c>Mud.Wechat.Pay.MerchantManage</c> 的 <c>IWechatPayMerchantManageFileService</c>），
/// <b>不在本守卫口径内</b>——它的命名空间由「域目录 = 命名空间末段」的手工通道范式决定，与声明式接口的分区规则无关。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三条断言共用同一个「反射枚举」口径，且都带<b>正对照</b>：NS1 用基线计数、NS2 用两支曾漂移接口的
/// 现名解析、NS3 用逐接口配对。任一口径漂移（扫描根换错程序集、特性名拼错）都会立刻变红而不是静默恒真。
/// </para>
/// </remarks>
public class WechatPayInterfaceNamespaceContractGuards
{
    /// <summary>声明式接口的唯一合法命名空间（运行时面，无父接口分面）。</summary>
    private const string PayInterfaceNamespace = "Mud.Wechat.Pay";

    /// <summary>
    /// 反射枚举主程序集内全部带 <c>[HttpClientApi]</c> 的公共接口。
    /// 特性按<b>类型名</b>匹配（不 <c>typeof</c>）：与 <c>WechatPayDomainContractGuards</c> 的 PAY-B1 同一口径，
    /// 避免守卫依赖组件程序集的加载顺序。
    /// </summary>
    private static Type[] LoadDeclarativeInterfaces() =>
        typeof(Mud.Wechat.Pay.Extensions.PayModule).Assembly
            .GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            // 2026-10 包合并：APIv2 侧（命名空间 Mud.Wechat.Pay.V2*）与 APIv3 同程序集，
            // 但它的落位规则由 WechatPayV2* 守卫族治理（手写族不走源生成），不属本守卫主语。
            .Where(static t => !(t.Namespace ?? string.Empty)
                .StartsWith("Mud.Wechat.Pay.V2", StringComparison.Ordinal))
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "HttpClientApiAttribute"))
            .OrderBy(static t => t.FullName, StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// PAY-NS1：全部 <c>[HttpClientApi]</c> 接口的命名空间恒为 <c>Mud.Wechat.Pay</c>，
    /// 且数量等于基线（防「枚举空跑 ⇒ 零违规假绿」）。
    /// </summary>
    [Fact]
    public void PayApiInterfaces_ShouldAllLiveInPayRootNamespace_WhenEnumerated()
    {
        var interfaces = LoadDeclarativeInterfaces();

        interfaces.Should().HaveCount(Baseline.Pay.HttpApiInterfaces,
            "支付线声明式接口数量变化须同批更新 PayModule 与 Tests/ContractBaseline.cs（本守卫的枚举口径也须复核非空）");

        var offenders = interfaces
            .Where(static t => t.Namespace != PayInterfaceNamespace)
            .Select(static t => $"{t.FullName}（应为 {PayInterfaceNamespace}.{t.Name}）")
            .ToArray();

        offenders.Should().BeEmpty(
            "支付线无父接口分面，全部 [HttpClientApi] 接口落 Mud.Wechat.Pay；落进 Mud.Wechat.Pay.Interfaces.* " +
            "会让实现类跟着漂到 .Internal 子命名空间，并绕过企微线 N1~N3 的覆盖面");
    }

    /// <summary>
    /// PAY-NS2：主程序集内<b>不得存在</b>任何命名空间以 <c>Mud.Wechat.Pay.Interfaces</c> 开头的公共类型（防回潮形态断言）。
    /// <para>
    /// <b>正对照</b>：两支曾漂移的接口必须能在 <c>Mud.Wechat.Pay</c> 里按现名解析到 ——
    /// 只有「扫描有效」与「漂移已修」同时成立，上面的零命中才是裁决而不是空转。
    /// </para>
    /// </summary>
    [Fact]
    public void PayAssembly_ShouldHaveNoInterfacesPrefixedNamespace_AndPositiveControlResolves()
    {
        var assembly = typeof(Mud.Wechat.Pay.Extensions.PayModule).Assembly;

        var drifted = assembly.GetTypes()
            .Where(static t => t.IsPublic && t.Namespace is not null
                           && t.Namespace.StartsWith("Mud.Wechat.Pay.Interfaces", StringComparison.Ordinal))
            .Select(static t => t.FullName!)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        drifted.Should().BeEmpty(
            "Mud.Wechat.Pay.Interfaces.* 是 2026-10-15 已收敛的漂移形态；新增接口须落 Mud.Wechat.Pay（PAY-NS1）");

        foreach (var name in new[] { "IWechatPayMerchantManageService", "IWechatPayMerchantOperationManageService" })
        {
            assembly.GetType($"{PayInterfaceNamespace}.{name}", throwOnError: false)
                .Should().NotBeNull(
                    $"正对照失效：{name} 不在 {PayInterfaceNamespace} 下，NS2 的零命中将静默恒真");
        }
    }

    /// <summary>
    /// PAY-NS3：源生成器产出的实现类<b>随接口命名空间迁移</b>——每个声明式接口恰好一个
    /// <c>Mud.Wechat.Pay.Internal</c> 下的实现类（类名 = 接口名去 <c>I</c> 前缀）且实现该接口。
    /// <para>
    /// 这条锁的是「改名落位」的<b>下游后果</b>：接口回到 <c>Mud.Wechat.Pay</c> 后实现类必须回到
    /// <c>Mud.Wechat.Pay.Internal</c>；若有人只改接口命名空间而生成器口径理解不同，NS1 绿而 NS3 红。
    /// </para>
    /// </summary>
    [Fact]
    public void GeneratedImplementations_ShouldLiveInPayInternal_AndMatchInterfaceOneToOne()
    {
        var assembly = typeof(Mud.Wechat.Pay.Extensions.PayModule).Assembly;
        var internalTypes = assembly.GetTypes()
            .Where(static t => t.IsClass && t.Namespace == "Mud.Wechat.Pay.Internal")
            .ToArray();

        foreach (var interfaceType in LoadDeclarativeInterfaces())
        {
            var expectedName = interfaceType.Name.Substring(1);
            var matches = internalTypes
                .Where(t => t.Name == expectedName)
                .Where(t => interfaceType.IsAssignableFrom(t))
                .ToArray();

            matches.Should().HaveCount(1,
                $"接口 {interfaceType.Name} 的实现类应恰有一个且落 Mud.Wechat.Pay.Internal（实际 {matches.Length} 个）");
        }

        internalTypes.Should().HaveCountGreaterThanOrEqualTo(Baseline.Pay.HttpApiInterfaces,
            "实现类总数不得少于接口数——少于即生成器未对某支接口产出实现");
    }
}
