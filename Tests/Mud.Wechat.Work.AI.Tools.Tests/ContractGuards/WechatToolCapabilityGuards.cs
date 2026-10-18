using FluentAssertions;
using Mud.Wechat.Work.AI.Tools.Generated;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 能力目录守卫（WT-C1~C2，FIX-24 改形）：把「策展面 × SDK 面」的规模事实钉在编译期内嵌类型上。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不产报告文件</b>：markdown 报告既不会失败也不会被 CI 读取 —— 它只是「漂移的第四份副本」。
/// 目录事实（<see cref="WechatToolCapabilityCatalog"/>）由生成器在编译期算好并<b>内嵌为常量</b>，
/// 守卫直接读它 ⇒ 漂移当场变红，不需要额外的落地物、不需要改门禁脚本。
/// </para>
/// <para>
/// <b>为什么这些数必须相等</b>：<c>CuratedToolCount</c> 是生成器视角的「已策展工具数」，
/// <c>WechatToolNames.All</c> 是契约常量视角的同一条事实。两者口径不同（一个是投影面、一个是常量面），
/// 一旦分叉说明生成器的策展判定与契约发射不一致 —— 模型会看到「Schema 有、常量无」或反之的工具。
/// </para>
/// <para>
/// <b>为什么不锁 <c>SdkMethodCount</c> 的具体值</b>：它随企微 SDK 加端点而变（842 → 更高），
/// 锁死会让每次 SDK 升级都误红。锁的是<b>聚合一致性</b>（逐域之和 = 总数）与<b>策展面不越界</b>
/// （62 个工具 ⊆ 842 个方法）。
/// </para>
/// </remarks>
public class WechatToolCapabilityGuards
{
    /// <summary>38 个能力分组（分组轴 = 接口名解析出的 domain 段）。</summary>
    private const int FrozenDomainCount = 38;

    [Fact]
    public void Catalog_CuratedCount_ShouldMatchContractConstants()
    {
        WechatToolCapabilityCatalog.CuratedToolCount.Should().Be(
            WechatToolNames.All.Length,
            "生成器视角的策展数与契约常量数必须同源（分叉 ⇒ 模型看到 Schema 有、常量无的工具）");
    }

    [Fact]
    public void Catalog_DomainCount_ShouldBeFrozen()
    {
        WechatToolCapabilityCatalog.DomainCount.Should().Be(
            FrozenDomainCount,
            "能力分组数变化意味着 SDK 接口命名域变化（或分组轴实现变了），必须显式确认");
        WechatToolCapabilityCatalog.MethodsByDomain.Should().HaveCount(FrozenDomainCount);
    }

    [Fact]
    public void Catalog_MethodsByDomain_ShouldSumToTotal()
    {
        WechatToolCapabilityCatalog.MethodsByDomain.Values.Sum().Should().Be(
            WechatToolCapabilityCatalog.SdkMethodCount,
            "逐域方法数之和必须等于方法总数（否则能力目录漏算某个域）");
    }

    [Fact]
    public void Catalog_OutputSchemaCoverage_ShouldNotExceedCurated()
    {
        WechatToolCapabilityCatalog.OutputSchemaCoveredToolCount.Should().BeLessThanOrEqualTo(
            WechatToolCapabilityCatalog.CuratedToolCount,
            "分子不得超过分母（覆盖率不得大于 1）");

        // 已知形态（MUDWT009）：14 个工具的输出 Schema 因深度超限被截断 ⇒ 覆盖数恒小于策展数。
        // 这不是缺陷信号，而是「深度截断」这一事实的可观测面；一旦两者相等，说明截断面消失了，
        // 应当复核生成器的深度上限是否被无意放宽（放宽会让 Schema 体积失控）。
        WechatToolCapabilityCatalog.OutputSchemaCoveredToolCount.Should().BeLessThan(
            WechatToolCapabilityCatalog.CuratedToolCount,
            "当前有 14 个工具的输出 Schema 被深度截断（MUDWT009）；相等意味着截断面消失，需复核深度上限");
    }

    [Fact]
    public void Catalog_CuratedTools_ShouldBeSubsetOfSdkMethods()
    {
        WechatToolCapabilityCatalog.CuratedToolCount.Should().BeLessThan(
            WechatToolCapabilityCatalog.SdkMethodCount,
            "策展面是 SDK 面的一部分（62 ≪ 842）；策展数追上方法总数说明策展判定退化成了全量暴露");
    }
}
