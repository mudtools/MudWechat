// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.OpenTelemetry;
using OpenTelemetry.Logs;

namespace Mud.Wechat.OpenTelemetry.Tests;

/// <summary>
/// 共享装配内核（<c>Mud.HttpUtils.OpenTelemetry</c>）迁移后的产品侧契约锁定。
/// </summary>
/// <remarks>
/// <para>覆盖四类不可回归的契约：① 产品贡献描述（单根源 + 产品线通配 Meter + MudHttp 源开关）；
/// ② 选项映射的**逐属性完整性**（同时是 <c>scripts/audit-config-keys.ps1</c> 的消费点要求）；
/// ③ 注册期真校验（修复此前「非法配置静默通过」）；④ 单一入口禁令（内核 fail-fast）。</para>
/// </remarks>
public class WechatOpenTelemetrySharedKernelTests
{
    // ============================================================
    // ① 产品贡献描述
    // ============================================================

    [Fact]
    public void CreateContribution_ShouldDeclareWechatProductContract()
    {
        var contribution = WechatOpenTelemetryExtensions.CreateContribution(new WechatOpenTelemetryOptions());

        contribution.ProductName.Should().Be("Mud.Wechat");
        // 单根源：全产品线共用 "Mud.Wechat"，产品线由 wechat.product 标签 + ActivityName 产品线段区分。
        contribution.ActivitySourceName.Should().Be(WechatActivitySource.Name);
        contribution.ActivitySourceName.Should().Be("Mud.Wechat");
        // 产品线 Meter 用通配（覆盖 Mud.Wechat.Work / Mud.Wechat.OfficialAccount 等），不用精确 Meter。
        contribution.MeterWildcard.Should().Be("Mud.Wechat*");
        contribution.MeterName.Should().BeNull("微信为多产品线，必须走通配 Meter 而非精确 Meter");
        contribution.DefaultServiceName.Should().Be("Mud.Wechat.Application");
        contribution.DefaultServiceVersion.Should().Be(WechatActivitySource.Version);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateContribution_ShouldMirrorIncludeMudHttpUtils(bool includeMudHttpUtils)
    {
        var options = new WechatOpenTelemetryOptions { IncludeMudHttpUtils = includeMudHttpUtils };

        var contribution = WechatOpenTelemetryExtensions.CreateContribution(options);

        contribution.IncludeMudHttpSources.Should().Be(includeMudHttpUtils,
            "IncludeMudHttpUtils 必须原样落到内核的 IncludeMudHttpSources（源/Meter 由内核去重注册）");
    }

    // ============================================================
    // ② 选项映射的逐属性完整性（audit-config-keys.ps1 消费点）
    // ============================================================

    [Fact]
    public void Mapper_ShouldMapEveryPublicOptionProperty()
    {
        var headers = new Dictionary<string, string> { ["Authorization"] = "Bearer token" };
        Action<TracerProviderBuilder> tracing = _ => { };
        Action<MeterProviderBuilder> metrics = _ => { };
        Action<LoggerProviderBuilder> logging = _ => { };

        var options = new WechatOpenTelemetryOptions
        {
            EnableTracing = false,
            EnableMetrics = false,
            EnableLogging = true,
            IncludeMudHttpUtils = false,
            EnableHttpClientInstrumentation = false,
            EnableAspNetCoreInstrumentation = false,
            OtlpEndpoint = new Uri("http://otel-collector:4317"),
            OtlpExportProtocol = Mud.HttpUtils.OpenTelemetry.OtlpExportProtocol.HttpProtobuf,
            OtlpHeaders = headers,
            UseShortExporterTimeout = true,
            ExportBatchSize = 256,
            ExportIntervalMilliseconds = 3000,
            ServiceName = "my-wechat-app",
            ServiceVersion = "9.9.9",
            DeploymentEnvironment = "staging",
            SamplingRatio = 0.25,
            ConfigureTracing = tracing,
            ConfigureMetrics = metrics,
            ConfigureLogging = logging,
        };

        var core = WechatOpenTelemetryOptionsMapper.ToCore(options);

        // 每个公开可写属性都必须被映射（漏映射 = 配置静默失效 + 配置审计门禁红）。
        core.EnableTracing.Should().BeFalse();
        core.EnableMetrics.Should().BeFalse();
        core.EnableLogging.Should().BeTrue();
        core.EnableHttpClientInstrumentation.Should().BeFalse();
        core.EnableAspNetCoreInstrumentation.Should().BeFalse();
        core.OtlpEndpoint.Should().Be(new Uri("http://otel-collector:4317"));
        core.OtlpExportProtocol.Should().Be(Mud.HttpUtils.OpenTelemetry.OtlpExportProtocol.HttpProtobuf);
        core.OtlpHeaders.Should().BeSameAs(headers);
        core.UseShortExporterTimeout.Should().BeTrue();
        core.ExportBatchSize.Should().Be(256);
        core.ExportIntervalMilliseconds.Should().Be(3000);
        core.ServiceName.Should().Be("my-wechat-app");
        core.ServiceVersion.Should().Be("9.9.9");
        core.DeploymentEnvironment.Should().Be("staging");
        core.SamplingRatio.Should().Be(0.25);
        core.ConfigureTracing.Should().BeSameAs(tracing);
        core.ConfigureMetrics.Should().BeSameAs(metrics);
        core.ConfigureLogging.Should().BeSameAs(logging);
    }

    [Fact]
    public void Options_ShouldExposeOtlpEnhancedSurface_WithUpstreamDefaults()
    {
        var options = new WechatOpenTelemetryOptions();

        options.OtlpExportProtocol.Should().Be(Mud.HttpUtils.OpenTelemetry.OtlpExportProtocol.Grpc);
        options.OtlpHeaders.Should().BeNull();
        options.UseShortExporterTimeout.Should().BeFalse();
        options.ExportBatchSize.Should().BeNull();
        options.ExportIntervalMilliseconds.Should().BeNull();
    }

    // ============================================================
    // ③ 注册期真校验（修复：此前 IValidateOptions 管道永不触发）
    // ============================================================

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrowOptionsValidationException_AtRegistration_WhenServiceNameEmpty()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatOpenTelemetry(o => o.ServiceName = "  ");

        act.Should().Throw<OptionsValidationException>()
            .Where(e => e.OptionsType == typeof(WechatOpenTelemetryOptions))
            .Where(e => e.Message.Contains("ServiceName"));
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrowOptionsValidationException_AtRegistration_WhenOtlpEndpointIsRelative()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatOpenTelemetry(
            o => o.OtlpEndpoint = new Uri("/relative/path", UriKind.Relative));

        act.Should().Throw<OptionsValidationException>()
            .Where(e => e.Message.Contains("OtlpEndpoint"));
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrowOptionsValidationException_AtRegistration_WhenExportBatchSizeNegative()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatOpenTelemetry(o => o.ExportBatchSize = -1);

        act.Should().Throw<OptionsValidationException>()
            .Where(e => e.Message.Contains("ExportBatchSize"));
    }

    // ============================================================
    // ④ 单一入口禁令（内核 fail-fast）+ 幂等
    // ============================================================

    [Fact]
    public void AddWechatOpenTelemetry_ShouldThrow_WhenCalledTogetherWithMudHttpBootstrap()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry();

        var act = () => services.AddMudHttpOpenTelemetry();

        act.Should().Throw<InvalidOperationException>()
            .Where(e => e.Message.Contains("Mud.Wechat") && e.Message.Contains("Mud.HttpUtils"));
    }

    [Fact]
    public void AddWechatOpenTelemetry_ShouldBeIdempotent_WhenCalledTwiceWithSameProduct()
    {
        var services = new ServiceCollection();
        services.AddWechatOpenTelemetry();

        var act = () => services.AddWechatOpenTelemetry();

        act.Should().NotThrow("同一产品重复注册由内核幂等短路（不再重复装配，避免 Span/指标翻倍）");
    }
}
