// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Tests.Configuration;

/// <summary>公众号配置校验：账号凭据必填、默认域名、SSRF 白名单与脱敏。</summary>
public class MpAppConfigTests
{
    private static MpAppConfig Valid() => new()
    {
        AppKey = "mp-main",
        AppId = "wx-test",
        AppSecret = "secret-test",
    };

    /// <summary>C1：合法配置通过。</summary>
    [Fact]
    public void Validate_ShouldPassForCompleteConfig()
    {
        var config = Valid();
        var act = () => config.Validate();
        act.Should().NotThrow();
    }

    /// <summary>C2：缺 AppId / AppSecret 抛 <see cref="InvalidOperationException"/>（启动期快速失败）。</summary>
    [Theory]
    [InlineData("", "secret")]
    [InlineData("wx", "")]
    public void Validate_ShouldRequireAccountCredentials(string appId, string secret)
    {
        var config = Valid();
        config.AppId = appId;
        config.AppSecret = secret;

        var act = () => config.Validate();
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>C3：BaseUrl 留空回落公众号默认域名（公用层常量单一来源）。</summary>
    [Fact]
    public void Validate_ShouldFallbackToOfficialAccountBaseUrl()
    {
        var config = Valid();
        config.BaseUrl = string.Empty;
        config.Validate();

        config.BaseUrl.Should().Be(WechatApiHosts.OfficialAccountBaseUrl);
    }

    /// <summary>C4：非白名单域名在 <c>AllowCustomBaseUrl=false</c> 时拒绝；显式放开后放行。</summary>
    [Fact]
    public void Validate_ShouldEnforceSsrfWhitelist()
    {
        var config = Valid();
        config.BaseUrl = "https://gateway.example.com";

        var act = () => config.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*不在白名单内*");

        config.AllowCustomBaseUrl = true;
        var act2 = () => config.Validate();
        act2.Should().NotThrow("显式放开 AllowCustomBaseUrl 后允许私有化/网关部署");
    }

    /// <summary>C5：非 HTTPS 拒绝。</summary>
    [Fact]
    public void Validate_ShouldRejectNonHttpsBaseUrl()
    {
        var config = Valid();
        config.BaseUrl = "http://api.weixin.qq.com";

        var act = () => config.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*绝对 HTTPS*");
    }

    /// <summary>C6：AppKey 形状非法拒绝（继承自公用基座的通用校验）。</summary>
    [Fact]
    public void Validate_ShouldRejectIllegalAppKey()
    {
        var config = Valid();
        config.AppKey = "mp:1";

        var act = () => config.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*AppKey*");
    }

    /// <summary>C7：数值范围校验（超时 / 令牌阈值）。</summary>
    [Theory]
    [InlineData(0, 300)]
    [InlineData(301, 300)]
    [InlineData(30, 10)]
    [InlineData(30, 3601)]
    public void Validate_ShouldEnforceNumericRanges(int timeoutSeconds, int threshold)
    {
        var config = Valid();
        config.TimeoutSeconds = timeoutSeconds;
        config.TokenRefreshThreshold = threshold;

        var act = () => config.Validate();
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>C8：<c>ToString</c> 输出诊断信息且绝不包含 AppSecret（日志安全红线）。</summary>
    [Fact]
    public void ToString_ShouldNeverLeakAppSecret()
    {
        var config = Valid();
        var text = config.ToString();

        text.Should().Contain("mp-main");
        text.Should().Contain("wx-test");
        text.Should().NotContain("secret-test", "AppSecret 永不落日志/遥测");
    }

    /// <summary>C9：<c>UseStableToken</c> 默认开启（官方推荐稳定版接口）。</summary>
    [Fact]
    public void UseStableToken_ShouldDefaultToTrue()
        => new MpAppConfig().UseStableToken.Should().BeTrue("官方推荐使用稳定版接口获取凭据");
}
