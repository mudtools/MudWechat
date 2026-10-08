// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenTelemetry;

/// <summary>
/// <see cref="WechatOpenTelemetryOptions"/> → 上游共享内核选项 <see cref="MudObservabilityOptions"/> 的显式映射。
/// </summary>
/// <remarks>
/// <para><b>为何用映射而非 options 继承</b>：配置绑定源生成器（<c>EnableConfigurationBindingGenerator</c>）
/// 对**继承属性**的支持需先实测，未验证即用会导致 AOT 下绑定静默失效；显式映射零风险且可静态审查。</para>
/// <para><b>门禁约束</b>：<c>scripts/audit-config-keys.ps1</c> 要求每个公开标量配置属性在
/// <c>$searchRoots</c>（含 <c>Mud.Wechat.OpenTelemetry</c>）内存在真实**读取**消费点。
/// 本文件即该消费点的唯一承载者 —— <b>逐属性读取，不得遗漏</b>；
/// 且本文件必须留在 <c>Mud.Wechat.OpenTelemetry</c> 目录内（移出即审计门禁红）。</para>
/// <para>本方法不做任何校验与默认值回填：校验在 <see cref="WechatOpenTelemetryExtensions"/> 注册期完成，
/// 缺省回填（<c>ServiceName</c> / <c>ServiceVersion</c>）由内核按贡献默认值完成。</para>
/// </remarks>
internal static class WechatOpenTelemetryOptionsMapper
{
    /// <summary>
    /// 逐属性映射到共享内核选项。
    /// </summary>
    /// <param name="options">微信可观测性选项。</param>
    /// <returns>共享内核选项。</returns>
    internal static MudObservabilityOptions ToCore(WechatOpenTelemetryOptions options)
    {
        return new MudObservabilityOptions
        {
            EnableTracing = options.EnableTracing,
            EnableMetrics = options.EnableMetrics,
            EnableLogging = options.EnableLogging,
            EnableHttpClientInstrumentation = options.EnableHttpClientInstrumentation,
            EnableAspNetCoreInstrumentation = options.EnableAspNetCoreInstrumentation,
            OtlpEndpoint = options.OtlpEndpoint,
            // 两个枚举取值一一对应（Grpc = 0 / HttpProtobuf = 1），转型安全；
            // 两侧均为「本仓/上游自定义枚举」而非 OTel SDK 枚举（后者的 Grpc 成员已过时）。
            OtlpExportProtocol = (MudOtlpExportProtocol)options.OtlpExportProtocol,
            OtlpHeaders = options.OtlpHeaders,
            UseShortExporterTimeout = options.UseShortExporterTimeout,
            ExportBatchSize = options.ExportBatchSize,
            ExportIntervalMilliseconds = options.ExportIntervalMilliseconds,
            ServiceName = options.ServiceName,
            ServiceVersion = options.ServiceVersion,
            DeploymentEnvironment = options.DeploymentEnvironment,
            SamplingRatio = options.SamplingRatio,
            ConfigureTracing = options.ConfigureTracing,
            ConfigureMetrics = options.ConfigureMetrics,
            ConfigureLogging = options.ConfigureLogging,
        };
    }
}
