// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenTelemetry.Tests;

/// <summary>
/// <see cref="WechatOpenTelemetryExtensions"/> 的单元测试。
/// </summary>
public class WechatOpenTelemetryExtensionsTests
{
    [Fact]
    public void Options_DefaultValues_ShouldBeCorrect()
    {
        var options = new WechatOpenTelemetryOptions();

        options.EnableTracing.Should().BeTrue();
        options.EnableMetrics.Should().BeTrue();
        options.EnableLogging.Should().BeFalse();
        options.IncludeMudHttpUtils.Should().BeTrue();
        options.EnableHttpClientInstrumentation.Should().BeTrue();
        options.EnableAspNetCoreInstrumentation.Should().BeTrue();
        options.OtlpEndpoint.Should().NotBeNull();
        options.OtlpEndpoint!.ToString().Should().StartWith("http://localhost:4317");
        options.ServiceName.Should().Be("Mud.Wechat.Application");
        options.ServiceVersion.Should().Be(WechatActivitySource.Version);
        options.DeploymentEnvironment.Should().Be("production");
        options.SamplingRatio.Should().Be(1.0);
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        Action act = () => ((IServiceCollection)null!).AddWechatOpenTelemetry();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        Action act = () => new ServiceCollection()
            .AddWechatOpenTelemetry((IConfiguration)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrowArgumentOutOfRangeException_WhenSamplingRatioOutOfRange()
    {
        Action act = () => new ServiceCollection().AddWechatOpenTelemetry(o => o.SamplingRatio = 1.5);
        act.Should().Throw<ArgumentOutOfRangeException>();

        Action act2 = () => new ServiceCollection().AddWechatOpenTelemetry(o => o.SamplingRatio = -0.1);
        act2.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldNotThrow_WhenTracingDisabled()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o => o.EnableTracing = false);
        // Should not throw during registration
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldNotThrow_WhenMetricsDisabled()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o => o.EnableMetrics = false);
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldNotThrow_WhenMudHttpUtilsDisabled()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o => o.IncludeMudHttpUtils = false);
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldNotThrow_WhenOtlpEndpointIsNull()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o => o.OtlpEndpoint = null);
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldNotThrow_WhenAllInstrumentationsDisabled()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o =>
        {
            o.EnableHttpClientInstrumentation = false;
            o.EnableAspNetCoreInstrumentation = false;
            o.OtlpEndpoint = null;
        });
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddWechatOpenTelemetry_WithConfiguration_ShouldBindOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["WechatOpenTelemetry:ServiceName"] = "my-wechat-app",
                ["WechatOpenTelemetry:SamplingRatio"] = "0.1",
                ["WechatOpenTelemetry:OtlpEndpoint"] = "http://otel-collector:4317",
                ["WechatOpenTelemetry:EnableLogging"] = "true",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(config);

        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<WechatOpenTelemetryOptions>>().Value;

        options.ServiceName.Should().Be("my-wechat-app");
        options.SamplingRatio.Should().Be(0.1);
        options.OtlpEndpoint!.ToString().Should().StartWith("http://otel-collector:4317");
        options.EnableLogging.Should().BeTrue();
    }

    [Fact]
    public void WechatActivitySource_Name_ShouldBeMudWechat()
    {
        WechatActivitySource.Name.Should().Be("Mud.Wechat");
    }

    [Fact]
    public void WechatActivitySource_Version_ShouldNotBeEmpty()
    {
        WechatActivitySource.Version.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void WechatActivitySource_Products_ShouldHaveExpectedValues()
    {
        WechatActivitySource.Products.Work.Should().Be("work");
        WechatActivitySource.Products.OfficialAccount.Should().Be("officialaccount");
        WechatActivitySource.Products.MiniProgram.Should().Be("miniprogram");
        WechatActivitySource.Products.OpenPlatform.Should().Be("openplatform");
        WechatActivitySource.Products.Pay.Should().Be("pay");
    }

    [Fact]
    public void WechatActivitySource_Tags_ShouldHaveExpectedValues()
    {
        WechatActivitySource.Tags.Product.Should().Be("wechat.product");
        WechatActivitySource.Tags.AppKey.Should().Be("wechat.app_key");
        WechatActivitySource.Tags.Outcome.Should().Be("outcome");
        WechatActivitySource.Tags.ErrorType.Should().Be("error.type");
        WechatActivitySource.Tags.CorrelationId.Should().Be("wechat.correlation_id");
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldRegisterIValidateOptions()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry();

        var sp = services.BuildServiceProvider();
        var validator = sp.GetService<IValidateOptions<WechatOpenTelemetryOptions>>();
        validator.Should().NotBeNull();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldRegisterIOptions()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o => o.ServiceName = "test-service");

        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<WechatOpenTelemetryOptions>>().Value;
        options.ServiceName.Should().Be("test-service");
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldInvokeConfigureTracing_WhenBuildServiceProvider()
    {
        var tracingInvoked = false;
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o =>
        {
            o.ConfigureTracing = _ => tracingInvoked = true;
        });

        // Force the OTel builder to execute deferred configurations
        services.BuildServiceProvider();

        tracingInvoked.Should().BeTrue();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldInvokeConfigureMetrics_WhenBuildServiceProvider()
    {
        var metricsInvoked = false;
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o =>
        {
            o.ConfigureMetrics = _ => metricsInvoked = true;
        });

        services.BuildServiceProvider();

        metricsInvoked.Should().BeTrue();
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldInvokeConfigureLogging_WhenBuildServiceProvider()
    {
        var loggingInvoked = false;
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry(o =>
        {
            o.EnableLogging = true;
            o.OtlpEndpoint = null;
            o.ConfigureLogging = _ => loggingInvoked = true;
        });

        services.BuildServiceProvider();

        loggingInvoked.Should().BeTrue();
    }
}
