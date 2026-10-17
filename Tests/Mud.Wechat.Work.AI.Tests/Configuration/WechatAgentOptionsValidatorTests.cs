using FluentAssertions;
using Microsoft.Extensions.Options;
using Mud.Wechat.Work.AI.Configuration;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Configuration;

/// <summary>
/// 配置面校验：默认值合法、非法值 fail-fast 且文案点名属性、两个产品事实状态键的「空 = 关闭」语义。
/// </summary>
public class WechatAgentOptionsValidatorTests
{
    private readonly WechatAgentOptionsValidator _validator = new();

    [Fact]
    public void Validate_Defaults_ShouldSucceed()
    {
        var result = _validator.Validate(null, new WechatAgentOptions());

        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(WechatAgentOptions.Instructions))]
    [InlineData(nameof(WechatAgentOptions.SessionTtlSeconds))]
    [InlineData(nameof(WechatAgentOptions.MaxHistoryMessages))]
    [InlineData(nameof(WechatAgentOptions.SummaryThreshold))]
    [InlineData(nameof(WechatAgentOptions.MaxHistoryTokens))]
    [InlineData(nameof(WechatAgentOptions.MaxToolCallsPerTurn))]
    [InlineData(nameof(WechatAgentOptions.HistoryStateKey))]
    public void Validate_InvalidValue_ShouldFailAndNameProperty(string property)
    {
        var options = new WechatAgentOptions();
        switch (property)
        {
            case nameof(WechatAgentOptions.Instructions):
                options.Instructions = "  ";
                break;
            case nameof(WechatAgentOptions.SessionTtlSeconds):
                options.SessionTtlSeconds = 0;
                break;
            case nameof(WechatAgentOptions.MaxHistoryMessages):
                options.MaxHistoryMessages = 0;
                break;
            case nameof(WechatAgentOptions.SummaryThreshold):
                options.SummaryThreshold = 2; // 0（禁用）或 ≥4
                break;
            case nameof(WechatAgentOptions.MaxHistoryTokens):
                options.MaxHistoryTokens = -1;
                break;
            case nameof(WechatAgentOptions.MaxToolCallsPerTurn):
                options.MaxToolCallsPerTurn = 101;
                break;
            case nameof(WechatAgentOptions.HistoryStateKey):
                options.HistoryStateKey = " ";
                break;
            default:
                throw new InvalidOperationException(property);
        }

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain(property);
    }

    [Fact]
    public void Validate_BothPendingReplyKeysBlank_ShouldBeAllowedAsOutboxDisabled()
    {
        var options = new WechatAgentOptions
        {
            PendingReplyStateKey = string.Empty,
            PendingReplyTurnStateKey = string.Empty,
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue("两个状态键皆空是「关闭 outbox 补发」的合法形态（管线按未配置处理）");
    }

    [Theory]
    [InlineData("", "wechat.agent.pending-reply-turn")]
    [InlineData("wechat.agent.pending-reply", "")]
    public void Validate_HalfConfiguredPendingReplyKeys_ShouldFail(string stateKey, string turnKey)
    {
        var options = new WechatAgentOptions
        {
            PendingReplyStateKey = stateKey,
            PendingReplyTurnStateKey = turnKey,
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse("两个 outbox 状态键必须成对（只配一半会在首轮生成后抛异常且不落库）");
        result.FailureMessage.Should().Contain(nameof(WechatAgentOptions.PendingReplyTurnStateKey));
    }

    [Fact]
    public void Validate_NullOptions_ShouldFail()
    {
        var result = _validator.Validate(null, null!);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void Defaults_ShouldBeSelfConsistent()
    {
        var options = new WechatAgentOptions();

        options.HistoryStateKey.Should().Be(WechatAgentOptions.DefaultHistoryStateKey);
        options.PendingReplyStateKey.Should().Be(WechatAgentOptions.DefaultPendingReplyStateKey);
        options.Instructions.Should().Be(WechatAgentOptions.DefaultInstructions);
        options.SummaryThreshold.Should().BeGreaterThan(0, "默认应启用渐进式摘要（长会话成本可控）");
    }
}
