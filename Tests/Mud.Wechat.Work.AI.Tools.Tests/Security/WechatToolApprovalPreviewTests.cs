using FluentAssertions;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools.Security;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 审批预览的行为锚：<b>审批人能看到「发给谁、发什么」</b>，且敏感串已脱敏、正文限长。
/// </summary>
/// <remarks>
/// 预览是 HITL 能否成立的唯一依据：只有摘要（哈希）时组织里必然退化为「一律批准」或「一律拒绝」。
/// 同时预览<b>不进审计</b>（生命周期不同），这条分工由 <see cref="WechatToolAuditRecord"/> 无预览字段锁定。
/// </remarks>
public class WechatToolApprovalPreviewTests
{
    [Fact]
    public void Build_ForMessageSend_ShouldExposeTargetsAndContent()
    {
        var preview = WechatToolApprovalPreview.Build(
            "message.send",
            WechatToolRisk.Write,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["touser"] = "zhangsan|lisi",
                ["content"] = "明天 10 点开会",
            });

        preview.ActionLabel.Should().Contain("发送应用消息");
        preview.TargetSummary.Should().Contain("touser:zhangsan").And.Contain("touser:lisi");
        preview.ContentPreview.Should().Be("明天 10 点开会");
        preview.Reversible.Should().BeNull("本波次尚无撤回工具，可补救性未知必须如实表达");
    }

    [Fact]
    public void Build_ForHighRiskWrite_ShouldMarkIrreversible()
    {
        var preview = WechatToolApprovalPreview.Build(
            "wedoc.doc.delete",
            WechatToolRisk.HighRiskWrite,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["docid"] = "doc-1" });

        preview.RiskLabel.Should().Contain("不可逆");
        preview.Reversible.Should().BeFalse();
        preview.TargetSummary.Should().Contain("docid:doc-1");
    }

    [Fact]
    public void Build_ShouldMaskPhoneNumber()
    {
        var preview = WechatToolApprovalPreview.Build(
            "message.send",
            WechatToolRisk.Write,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["touser"] = "zhangsan",
                ["content"] = "联系电话 13800138000，请回拨",
            });

        preview.ContentPreview.Should().Contain("1**********").And.NotContain("13800138000");
    }

    [Fact]
    public void Build_ShouldTruncateLongContent()
    {
        var content = new string('详', WechatToolApprovalPreview.MaxContentLength + 50);

        var preview = WechatToolApprovalPreview.Build(
            "message.send",
            WechatToolRisk.Write,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["content"] = content });

        preview.ContentPreview!.Length.Should().Be(WechatToolApprovalPreview.MaxContentLength + 1);
        preview.ContentPreview.Should().EndWith("…");
    }

    [Fact]
    public void Build_WithManyTargets_ShouldSummarizeRemainder()
    {
        var touser = string.Join('|', Enumerable.Range(0, 12).Select(i => $"user{i}"));

        var preview = WechatToolApprovalPreview.Build(
            "message.send",
            WechatToolRisk.Write,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["touser"] = touser });

        preview.TargetSummary.Should().HaveCountLessThanOrEqualTo(WechatToolApprovalPreview.MaxTargets + 1);
        preview.TargetSummary.Should().Contain(item => item.Contains("等 12 个"));
    }

    [Fact]
    public void ToDisplayText_ShouldRenderReadableLines()
    {
        var text = WechatToolApprovalPreview.Build(
            "message.send",
            WechatToolRisk.Write,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["touser"] = "zhangsan",
                ["content"] = "明天 10 点开会",
            }).ToDisplayText();

        text.Should().Contain("动作：").And.Contain("目标：").And.Contain("内容：");
    }
}
