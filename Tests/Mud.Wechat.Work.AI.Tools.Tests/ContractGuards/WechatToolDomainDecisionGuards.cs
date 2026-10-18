using FluentAssertions;
using Mud.Wechat.Work.AI.Tools.Generated;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 域决策表守卫（WT-G5）：把「某域为什么不做」从 README 散文升级为<b>可执行断言</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要</b>：没有守卫时，「不策展」的理由只活在文字里。半年后有人加了 <c>schedule.add</c>，
/// 没人知道当初为什么推迟；反之有人误删 <c>externalcontact.product_album.*</c>，也没人知道它是有意的。
/// 两类错误都会静默通过构建。
/// </para>
/// <para>
/// <b>三态语义</b>：<see cref="Decision.Curated"/> 必须至少有一个工具且全部登记在生成的名字表里；
/// <see cref="Decision.Deferred"/> / <see cref="Decision.Forbidden"/> 必须<b>一个工具都没有</b>
/// （后者是合规红线：一旦出现即红，不靠 review）。
/// </para>
/// <para>
/// <b>新增域</b>：<c>Interfaces/</c> 下出现新目录时，本表必须同批补登（表是 38 域的冻结契约，
/// 数量断言会在新域进仓而表未更新时提醒复核）。
/// </para>
/// </remarks>
public class WechatToolDomainDecisionGuards
{
    private enum Decision
    {
        Curated,
        Deferred,
        Forbidden,
    }

    /// <summary>域决策表：域目录名 →（状态，工具名前缀）。前缀为 <see langword="null"/> 表示该域不使用工具名面。</summary>
    private static readonly Dictionary<string, (Decision Status, string? Prefix)> Table = new(StringComparer.Ordinal)
    {
        ["Approval"] = (Decision.Curated, "approval"),
        ["Checkin"] = (Decision.Curated, "checkin"),
        ["Contacts"] = (Decision.Curated, "contacts"),
        ["Dial"] = (Decision.Curated, "dial"),
        ["ExternalContact"] = (Decision.Curated, "externalcontact"),
        ["Journal"] = (Decision.Curated, "journal"),
        ["KF"] = (Decision.Curated, "kf"),
        ["Living"] = (Decision.Curated, "living"),
        ["Meeting"] = (Decision.Curated, "meeting"),
        ["Message"] = (Decision.Curated, "message"),
        ["Schedule"] = (Decision.Curated, "schedule"),
        ["Wedoc"] = (Decision.Curated, "wedoc"),

        // D = 延后：入口不可自举 / 需外部服务开通 / 与工具面职责不匹配。补做时改为 Curated 并补工具。
        ["Mail"] = (Decision.Deferred, "mail"),
        ["MeetingRoom"] = (Decision.Deferred, "meetingroom"),
        ["Wedrive"] = (Decision.Deferred, "wedrive"),
        ["PromotionQrCode"] = (Decision.Deferred, "promotionqrcode"),
        ["Emergency"] = (Decision.Deferred, "emergency"),
        ["CorpGroup"] = (Decision.Deferred, "corpgroup"),

        // F = 禁止：凭据面 / 资金面 / 合规红线 / 宿主职责。出现任何工具名即视为越界。
        ["AccountId"] = (Decision.Forbidden, "accountid"),
        ["Agent"] = (Decision.Forbidden, "agent"),
        ["Aibot"] = (Decision.Forbidden, "aibot"),
        ["Authentication"] = (Decision.Forbidden, "authentication"),
        ["Basic"] = (Decision.Forbidden, "basic"),
        ["DataZone"] = (Decision.Forbidden, "datazone"),
        ["DeviceData"] = (Decision.Forbidden, "devicedata"),
        ["Gov"] = (Decision.Forbidden, "gov"),
        ["Hr"] = (Decision.Forbidden, "hr"),
        ["Identity"] = (Decision.Forbidden, "identity"),
        ["Invoice"] = (Decision.Forbidden, "invoice"),
        ["JsSdk"] = (Decision.Forbidden, "jssdk"),
        ["License"] = (Decision.Forbidden, "license"),
        ["Media"] = (Decision.Forbidden, "media"),
        ["MsgAudit"] = (Decision.Forbidden, "msgaudit"),
        ["Pay"] = (Decision.Forbidden, "pay"),
        ["PayTool"] = (Decision.Forbidden, "paytool"),
        ["School"] = (Decision.Forbidden, "school"),
        ["Security"] = (Decision.Forbidden, "security"),
        ["Webhook"] = (Decision.Forbidden, "webhook"),
    };

    /// <summary>
    /// 逐域工具数（与 README 域表同批维护）：<b>合计必须等于名字表总数</b>，且逐域必须与真实前缀计数一致。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为什么需要</b>：README 上一轮把 <c>ExternalContact</c> 记为 20（实 19），而总数仍写 62 —— 散文层面的
    /// 「逐域计数」没有机械校验就必然漂移，且漂移后<b>看不出</b>（合计没变，只是分配错了）。
    /// </para>
    /// <para>
    /// <b>维护方式</b>：新增/删除工具时本表与 <see cref="WechatToolSurfaceContractGuards"/> 的冻结清单<b>同批</b>改；
    /// 两处都改反而说明「工具面变了」，而不是单点疏漏。
    /// </para>
    /// </remarks>
    private static readonly Dictionary<string, int> CuratedCounts = new(StringComparer.Ordinal)
    {
        ["approval"] = 5,
        ["checkin"] = 1,
        ["contacts"] = 12,
        ["dial"] = 1,
        ["externalcontact"] = 19,
        ["journal"] = 2,
        ["kf"] = 6,
        ["living"] = 3,
        ["meeting"] = 3,
        ["message"] = 2,
        ["schedule"] = 3,
        ["wedoc"] = 5,
    };

    [Fact]
    public void CuratedDomainCounts_ShouldMatchRegisteredToolsAndSumToTotal()
    {
        CuratedCounts.Values.Sum().Should().Be(
            WechatToolNames.All.Length,
            "逐域计数合计必须等于名字表总数（5+1+12+1+19+2+6+3+3+2+3+5 = 62）");

        CuratedCounts.Keys.Should().BeEquivalentTo(
            Table.Where(kv => kv.Value.Status == Decision.Curated).Select(kv => kv.Value.Prefix!),
            "逐域计数表与决策表的 Curated 前缀必须一一对应");

        foreach (var (prefix, expected) in CuratedCounts)
        {
            var actual = WechatToolNames.All.Count(name => name.StartsWith(prefix + ".", StringComparison.Ordinal));
            actual.Should().Be(expected, $"{prefix} 的实际工具数与逐域计数表不符（README 域表同批维护）");
        }
    }

    [Fact]
    public void DecisionTable_ShouldCoverAllThirtyEightDomains()
    {
        Table.Should().HaveCount(38, "域决策表是 Interfaces/ 38 个业务域的冻结契约；新增域必须同批登记");

        Table.Values.Count(v => v.Status == Decision.Curated).Should().Be(12);
        Table.Values.Count(v => v.Status == Decision.Deferred).Should().Be(6);
        Table.Values.Count(v => v.Status == Decision.Forbidden).Should().Be(20);
    }

    [Fact]
    public void CuratedDomains_ShouldEachExposeAtLeastOneRegisteredTool()
    {
        foreach (var (domain, (status, prefix)) in Table)
        {
            if (status != Decision.Curated)
            {
                continue;
            }

            prefix.Should().NotBeNull($"{domain} 已策展 ⇒ 必须有工具名前缀");
            WechatToolNames.All.Should().Contain(
                name => name.StartsWith(prefix + ".", StringComparison.Ordinal),
                $"{domain} 被登记为已策展，但名字表里没有任何 {prefix}.* 工具（决策表与工具面必须同批）");
        }
    }

    [Fact]
    public void DeferredAndForbiddenDomains_ShouldExposeNoTool()
    {
        foreach (var (domain, (status, prefix)) in Table)
        {
            if (status == Decision.Curated || prefix is null)
            {
                continue;
            }

            var leaks = WechatToolNames.All
                .Where(name => name.StartsWith(prefix + ".", StringComparison.Ordinal))
                .ToArray();

            leaks.Should().BeEmpty(
                status == Decision.Forbidden
                    ? $"{domain} 属禁止面（凭据/资金/合规/宿主职责），出现工具即越界：{string.Join(", ", leaks)}"
                    : $"{domain} 尚在延后清单；补做须同批把本表状态改为 Curated 并补守卫");
        }
    }

    [Fact]
    public void EveryRegisteredTool_ShouldBelongToACuratedDomain()
    {
        var prefixes = Table
            .Where(kv => kv.Value.Status == Decision.Curated)
            .Select(kv => kv.Value.Prefix!)
            .ToArray();

        var orphans = WechatToolNames.All
            .Where(name => !prefixes.Any(prefix => name.StartsWith(prefix + ".", StringComparison.Ordinal)))
            .ToArray();

        orphans.Should().BeEmpty("工具必须落在已策展域内，否则它绕过了决策表");
    }
}
