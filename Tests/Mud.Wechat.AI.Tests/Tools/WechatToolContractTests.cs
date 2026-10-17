using FluentAssertions;
using Mud.Wechat.AI.Abstractions.Tools;
using Xunit;

namespace Mud.Wechat.AI.Tests.Tools;

/// <summary>
/// 层 1 工具身份轴与风险词表：四轴归一 / 企业维度判定 / 三档归一与缺省只读。
/// </summary>
public class WechatToolContractTests
{
    [Fact]
    public void Principal_ShouldNormalizeBlankAxesToNull()
    {
        var principal = new WechatToolPrincipal("  ", "", openId: "open-1");

        principal.AppKey.Should().BeNull();
        principal.AuthCorpId.Should().BeNull();
        principal.OpenId.Should().Be("open-1");
        principal.HasCorpScope.Should().BeFalse();
    }

    [Fact]
    public void Principal_WithAuthCorpId_ShouldExposeCorpScope()
    {
        var principal = new WechatToolPrincipal("app", authCorpId: "corp-1", openId: null, unionId: "union-1");

        principal.HasCorpScope.Should().BeTrue();
        principal.AuthCorpId.Should().Be("corp-1");
        principal.UnionId.Should().Be("union-1");
    }

    [Fact]
    public void Principal_Empty_ShouldBeAllUnknown()
    {
        WechatToolPrincipal.Empty.AppKey.Should().BeNull();
        WechatToolPrincipal.Empty.AuthCorpId.Should().BeNull();
        WechatToolPrincipal.Empty.HasCorpScope.Should().BeFalse();
    }

    [Theory]
    [InlineData(null, WechatToolRiskVocabulary.Read)]
    [InlineData("", WechatToolRiskVocabulary.Read)]
    [InlineData("  ", WechatToolRiskVocabulary.Read)]
    [InlineData("read", WechatToolRiskVocabulary.Read)]
    [InlineData("READ", WechatToolRiskVocabulary.Read)]
    [InlineData(" write ", WechatToolRiskVocabulary.Write)]
    [InlineData("high-risk-write", WechatToolRiskVocabulary.HighRiskWrite)]
    [InlineData("HIGH-RISK-WRITE", WechatToolRiskVocabulary.HighRiskWrite)]
    [InlineData("unknown-level", WechatToolRiskVocabulary.Read)]
    public void RiskVocabulary_Normalize_ShouldConvergeToThreeLevels(string? risk, string expected)
    {
        WechatToolRiskVocabulary.Normalize(risk).Should().Be(expected);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("read", false)]
    [InlineData("write", true)]
    [InlineData("high-risk-write", true)]
    public void RiskVocabulary_IsWrite_ShouldCoverBothWriteLevels(string? risk, bool expected)
    {
        WechatToolRiskVocabulary.IsWrite(risk).Should().Be(expected);
    }

    [Theory]
    [InlineData("high-risk-write", true)]
    [InlineData("write", false)]
    [InlineData("read", false)]
    public void RiskVocabulary_IsHighRiskWrite_ShouldOnlyMatchTopLevel(string? risk, bool expected)
    {
        WechatToolRiskVocabulary.IsHighRiskWrite(risk).Should().Be(expected);
    }
}
