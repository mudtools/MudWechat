// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Exceptions;

namespace Mud.Wechat.Work.Abstractions.Tests.Configuration;

/// <summary>
/// WechatAppConfig.Validate() 按 AppType 互斥必填项组合校验（产品规划 §7.2 / 详细设计 §8.4）。
/// </summary>
public class WechatAppConfigValidateTests
{
    private static WechatAppConfig InternalConfig(Action<WechatAppConfig>? patch = null)
    {
        var config = new WechatAppConfig
        {
            AppKey = "default",
            AppType = WechatAppType.Internal,
            CorpId = "ww-corp",
            AgentSecret = "agent-secret",
        };
        patch?.Invoke(config);
        return config;
    }

    [Fact]
    public void Validate_ShouldPass_WhenInternalConfigComplete()
    {
        var act = () => InternalConfig().Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ShouldFail_WhenInternalMissingAgentSecret()
    {
        var act = () => InternalConfig(c => c.AgentSecret = string.Empty).Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*AgentSecret*");
    }

    [Fact]
    public void Validate_ShouldFail_WhenInternalMissingCorpId()
    {
        var act = () => InternalConfig(c => c.CorpId = string.Empty).Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*CorpId*");
    }

    [Fact]
    public void Validate_ShouldFail_WhenAppKeyEmpty()
    {
        var act = () => InternalConfig(c => c.AppKey = string.Empty).Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*AppKey*");
    }

    [Fact]
    public void Validate_ShouldFail_WhenThirdPartyMissingSuiteFields()
    {
        var config = new WechatAppConfig
        {
            AppKey = "suite",
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
        };

        var act = () => config.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*SuiteId*");
    }

    [Fact]
    public void Validate_ShouldPass_WhenThirdPartyComplete()
    {
        var config = new WechatAppConfig
        {
            AppKey = "suite",
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
        };

        var act = () => config.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ShouldPass_WhenProviderTypeSameAsThirdParty()
    {
        var config = new WechatAppConfig
        {
            AppKey = "custom",
            AppType = WechatAppType.Provider,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
            TemplateId = "tpl-1",
        };

        var act = () => config.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ShouldFail_WhenBaseUrlNotHttps()
    {
        var act = () => InternalConfig(c => c.BaseUrl = "http://qyapi.weixin.qq.com").Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*HTTPS*");
    }

    [Fact]
    public void Validate_ShouldFail_WhenBaseUrlOutsideWhitelistAndCustomNotAllowed()
    {
        var act = () => InternalConfig(c => c.BaseUrl = "https://evil.example.com").Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*白名单*");
    }

    [Fact]
    public void Validate_ShouldPass_WhenCustomBaseUrlAllowed()
    {
        var act = () => InternalConfig(c =>
        {
            c.BaseUrl = "https://gateway.example.com";
            c.AllowCustomBaseUrl = true;
        }).Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTokenRefreshThresholdTooLow()
    {
        var act = () => InternalConfig(c => c.TokenRefreshThreshold = 10).Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*TokenRefreshThreshold*");
    }

    [Fact]
    public void Validator_ShouldFailOptionsPipeline_WhenConfigInvalid()
    {
        var validator = new WechatAppConfigValidator();
        var result = validator.Validate(null, InternalConfig(c => c.AgentSecret = string.Empty));
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("AgentSecret");
    }

    [Fact]
    public void Validator_ShouldRejectMultipleDefaultApps()
    {
        var validator = new WechatAppConfigValidator();
        var configs = new List<WechatAppConfig>
        {
            InternalConfig(c => c.AppKey = "a"),
            InternalConfig(c => c.AppKey = "b"),
        };
        configs[0].IsDefault = true;
        configs[1].IsDefault = true;

        var result = validator.Validate(null, configs);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("默认应用");
    }
}
