// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenTelemetry.Tests;

/// <summary>
/// <see cref="WechatOpenTelemetryOptions.Validate"/> 的单元测试。
/// </summary>
public class WechatOpenTelemetryOptionsValidationTests
{
    [Fact]
    public void Validate_ShouldSucceed_WhenAllDefaults()
    {
        var options = new WechatOpenTelemetryOptions();
        var result = options.Validate(null, options);
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenSamplingRatioOutOfRange()
    {
        var options = new WechatOpenTelemetryOptions { SamplingRatio = 1.5 };
        var result = options.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("SamplingRatio"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenSamplingRatioNegative()
    {
        var options = new WechatOpenTelemetryOptions { SamplingRatio = -0.1 };
        var result = options.Validate(null, options);
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenServiceNameEmpty()
    {
        var options = new WechatOpenTelemetryOptions { ServiceName = "" };
        var result = options.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("ServiceName"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenServiceVersionEmpty()
    {
        var options = new WechatOpenTelemetryOptions { ServiceVersion = "" };
        var result = options.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("ServiceVersion"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenDeploymentEnvironmentEmpty()
    {
        var options = new WechatOpenTelemetryOptions { DeploymentEnvironment = "" };
        var result = options.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("DeploymentEnvironment"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenOtlpEndpointIsRelativeUri()
    {
        var options = new WechatOpenTelemetryOptions
        {
            OtlpEndpoint = new Uri("/relative/path", UriKind.Relative)
        };
        var result = options.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("OtlpEndpoint"));
    }

    [Fact]
    public void Validate_ShouldSucceed_WhenOtlpEndpointIsNull()
    {
        var options = new WechatOpenTelemetryOptions { OtlpEndpoint = null };
        var result = options.Validate(null, options);
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldSucceed_WhenAllValuesValid()
    {
        var options = new WechatOpenTelemetryOptions
        {
            ServiceName = "my-app",
            ServiceVersion = "2.0.0",
            DeploymentEnvironment = "staging",
            SamplingRatio = 0.5,
            OtlpEndpoint = new Uri("http://otel:4317"),
        };
        var result = options.Validate(null, options);
        result.Succeeded.Should().BeTrue();
    }
}
