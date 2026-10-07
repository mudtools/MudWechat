// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 回调来源 IP 准入判定用例（P10：静态 ∪ 动态、CIDR、fail-open / fail-closed 边界）。
/// </summary>
public class MpCallbackSourceIpFilterTests
{
    private static readonly string[] None = Array.Empty<string>();

    /// <summary>两条来源皆未配置 ⇒ 不限来源（与既有行为一致，避免破坏既有宿主）。</summary>
    [Fact]
    public void NoWhitelist_ShouldAllowAnySource()
    {
        MpCallbackSourceIpFilter.IsAllowed(IPAddress.Parse("1.2.3.4"), None, None, out var active)
            .Should().BeTrue();
        active.Should().BeFalse();
    }

    /// <summary>动态白名单已启用但**尚未刷新出结果** ⇒ fail-open（启动窗口不得全量拒收）。</summary>
    [Fact]
    public void DynamicWhitelistNotReady_ShouldFailOpen()
    {
        var allowed = MpCallbackSourceIpFilter.IsAllowed(
            IPAddress.Parse("1.2.3.4"), None, None, out _);

        allowed.Should().BeTrue("white-list 是加固而非鉴权；真正准入闸是验签 + appid 校验");
    }

    /// <summary>静态白名单命中（精确 IP 与 IPv4 CIDR 两种写法）。</summary>
    [Fact]
    public void StaticWhitelist_ShouldMatchExactAndCidr()
    {
        MpCallbackSourceIpFilter.IsAllowed(
            IPAddress.Parse("106.55.206.146"), new[] { "106.55.206.146" }, None, out _)
            .Should().BeTrue();

        MpCallbackSourceIpFilter.IsAllowed(
            IPAddress.Parse("106.55.206.200"), new[] { "106.55.206.0/24" }, None, out _)
            .Should().BeTrue();

        MpCallbackSourceIpFilter.IsAllowed(
            IPAddress.Parse("106.55.207.1"), new[] { "106.55.206.0/24" }, None, out _)
            .Should().BeFalse();
    }

    /// <summary>动态列表命中即放行（即使不在静态白名单内）—— 两者为**并集**语义。</summary>
    [Fact]
    public void DynamicWhitelist_ShouldUnionWithStatic()
    {
        MpCallbackSourceIpFilter.IsAllowed(
            IPAddress.Parse("9.9.9.9"), new[] { "1.1.1.1" }, new[] { "9.9.9.9" }, out var active)
            .Should().BeTrue();
        active.Should().BeTrue();
    }

    /// <summary>配了白名单却拿不到来源 IP ⇒ fail-closed（不因缺少来源信息而放行）。</summary>
    [Fact]
    public void MissingRemoteIp_ShouldFailClosedWhenWhitelistConfigured()
    {
        MpCallbackSourceIpFilter.IsAllowed(null, new[] { "1.1.1.1" }, None, out _).Should().BeFalse();
        MpCallbackSourceIpFilter.IsAllowed(null, None, new[] { "1.1.1.1" }, out _).Should().BeFalse();
    }

    /// <summary>
    /// 非法条目不得误判：CIDR 仅支持 IPv4（前缀越界、网络地址非法、IPv6 地址对 IPv4 网段均不匹配）；
    /// 精确匹配为纯字符串比较（故 IPv6 字面量不参与网段判定）。
    /// </summary>
    [Fact]
    public void MalformedCidrAndNonIpv4_ShouldNotMatch()
    {
        MpCallbackSourceIpFilter.IsInCidr(IPAddress.Parse("1.2.3.4"), "1.2.3.4", "33").Should().BeFalse();
        MpCallbackSourceIpFilter.IsInCidr(IPAddress.Parse("1.2.3.4"), "not-an-ip", "24").Should().BeFalse();
        MpCallbackSourceIpFilter.IsInCidr(IPAddress.Parse("::1"), "1.2.3.0", "24").Should().BeFalse();
        MpCallbackSourceIpFilter.Matches(IPAddress.Parse("::1"), new[] { "1.2.3.0/24" }).Should().BeFalse();
    }
}
