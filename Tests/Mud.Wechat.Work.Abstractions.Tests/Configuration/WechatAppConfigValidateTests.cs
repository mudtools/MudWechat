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

    private static WechatAppConfig ProviderConfig(Action<WechatAppConfig>? patch = null)
    {
        var config = new WechatAppConfig
        {
            AppKey = "custom",
            AppType = WechatAppType.Provider,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
        };
        patch?.Invoke(config);
        return config;
    }

    [Fact]
    public void Validate_ShouldPass_WhenProviderConfigComplete()
    {
        var act = () => ProviderConfig().Validate();
        act.Should().NotThrow("代开发应用只依赖 suite_id（即模板 id）与套件/服务商凭据");
    }

    /// <summary>
    /// K2：协议上「代开发模板 id 即 suite_id」，故 <see cref="WechatAppConfig"/> 不提供独立
    /// <c>TemplateId</c> 配置项（原属性无真实消费点，仅被 <c>Validate()</c> 用于与非空 <c>SuiteId</c> 比对
    /// ⇒ 死配置）。本用例以反射锁定该契约，防止属性被"顺手加回来"。
    /// </summary>
    [Fact]
    public void Config_ShouldNotExposeTemplateId()
    {
        typeof(WechatAppConfig).GetProperty("TemplateId").Should().BeNull(
            "代开发模板 id 即 SuiteId（K2）：独立配置项属死配置，且会重新引入「二者不一致」的非法状态");
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

    /// <summary>
    /// P1-7：AppKey 形状校验——含 <c>:</c> 会造成令牌持久化键别名（跨应用串号），
    /// 含空格/<c>/</c> 会污染命名 HttpClient 名，故启动期快速失败。
    /// </summary>
    [Theory]
    [InlineData("a:b")]
    [InlineData("a b")]
    [InlineData("a/b")]
    [InlineData(":a")]
    [InlineData("-a")]
    [InlineData("中文应用")]
    public void Validate_ShouldFail_WhenAppKeyHasIllegalChars(string appKey)
    {
        var act = () => InternalConfig(c => c.AppKey = appKey).Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*AppKey*");
    }

    [Fact]
    public void Validate_ShouldFail_WhenAppKeyTooLong()
    {
        var act = () => InternalConfig(c => c.AppKey = new string('a', 129)).Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*长度*");
    }

    [Theory]
    [InlineData("default")]
    [InlineData("suite-app")]
    [InlineData("dk.template_01")]
    [InlineData("0")]
    public void Validate_ShouldPass_WhenAppKeyShapeLegal(string appKey)
    {
        var act = () => InternalConfig(c => c.AppKey = appKey).Validate();
        act.Should().NotThrow();
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
