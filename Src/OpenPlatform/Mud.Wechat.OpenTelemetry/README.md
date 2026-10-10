# Mud.Wechat.OpenTelemetry

Mud.Wechat **全产品线共用的 OpenTelemetry 一键装配包**：一次调用即注册追踪与指标采集，自动挂上微信 SDK 的 ActivitySource（单根源）、各产品线 Meter 与 Mud.HttpUtils 的 HTTP 可观测性源，默认导出至本地 OTLP gRPC 端点。

定位要点：本包**不是 ActivitySource / Meter 的定义者**——源定义在 `Src/Core/Mud.Wechat.Abstractions/Observability/WechatActivitySource.cs`（`Name = "Mud.Wechat"`，标签 `wechat.product` / `wechat.app_key` / `outcome` / `error.type` / `wechat.correlation_id`，产品线取值 `work` / `officialaccount` / `miniprogram` / `openplatform` / `pay`）与 `Core/Metrics/MeterExtensions.cs`（`RecordDuration` 直方图扩展）。本包只做「贡献描述 + 选项映射 + 注册期校验」，装配本身委托上游共享内核 `MudObservabilityBootstrap.AddMudObservability`（`Mud.HttpUtils.OpenTelemetry`）。

## 内容

- **DI 入口**（`WechatOpenTelemetryExtensions`）：两个重载，均返回 `OpenTelemetryBuilder` 便于继续追加配置——
  - `AddWechatOpenTelemetry(Action<WechatOpenTelemetryOptions>?)`：代码配置；
  - `AddWechatOpenTelemetry(IConfiguration, string sectionPath = "WechatOpenTelemetry", Action<WechatOpenTelemetryOptions>?)`：配置绑定走 `Configure<T>(o => section.Bind(o))` 源生成路径（AOT `IL2026`/`IL3050` 净零），附加委托在绑定后执行、可覆盖。
- **产品贡献描述**（`internal CreateContribution`）：`ProductName = "Mud.Wechat"`、单根源 `WechatActivitySource.Name`、产品线 Meter **通配 `"Mud.Wechat*"`**（覆盖现有与未来全部产品线）、`DefaultServiceName = "Mud.Wechat.Application"`、`DefaultServiceVersion = WechatActivitySource.Version`、`IncludeMudHttpSources ← IncludeMudHttpUtils`。
- **配置面**（`WechatOpenTelemetryOptions`，19 个公开属性，节 `WechatOpenTelemetry`）：

  | 属性 | 默认值 |
  |---|---|
  | `EnableTracing` / `EnableMetrics` | `true` / `true` |
  | `EnableLogging`（OTLP 日志导出） | `false` |
  | `IncludeMudHttpUtils` | `true` |
  | `EnableHttpClientInstrumentation` / `EnableAspNetCoreInstrumentation` | `true` / `true` |
  | `OtlpEndpoint` | `http://localhost:4317`（设 `null` 则不配导出器，自行追加） |
  | `OtlpExportProtocol` | `Grpc`（内网仅 4318 时改 `HttpProtobuf`） |
  | `OtlpHeaders` | `null` |
  | `UseShortExporterTimeout` | `false`（`true` 时导出超时压到 5s，便于开发调试） |
  | `ExportBatchSize` / `ExportIntervalMilliseconds` | `null`（SDK 默认 512 / 5000ms，仅 >0 生效） |
  | `ServiceName` | `"Mud.Wechat.Application"` |
  | `ServiceVersion` | `WechatActivitySource.Version`（与根 `Directory.Build.props` 的 `<Version>` 同步） |
  | `DeploymentEnvironment` | `"production"` |
  | `SamplingRatio` | `1.0`（`ParentBasedSampler(TraceIdRatioBasedSampler)`，生产建议 0.1~0.3） |
  | `ConfigureTracing` / `ConfigureMetrics` / `ConfigureLogging` | `null`（在本包默认配置之后执行，可追加/覆盖） |

- **选项映射**（`internal WechatOpenTelemetryOptionsMapper.ToCore`）：逐属性映射到内核 `MudObservabilityOptions`，是**每个公开配置属性唯一真实消费点**（`audit-config-keys.ps1` 门禁；文件必须留在本工程目录内）；`IncludeMudHttpUtils` 不经映射、走 `CreateContribution`。
- **注册期校验**：`SamplingRatio` 越界先行抛 `ArgumentOutOfRangeException`；其余非法值（空白 `ServiceName`/`ServiceVersion`/`DeploymentEnvironment`、相对 URI `OtlpEndpoint`）由 `Validate()` 拦截并抛 `OptionsValidationException`——扩展方法**显式调用**校验（预构建实例经 `OptionsWrapper` 注册，DI 的 `IValidateOptions` 通道不会被 options 管道触发），非法配置启动期即失败而非静默。

## 用法

```csharp
// Program.cs：一键开启（追踪 + 指标默认开，日志导出默认关）
builder.Services.AddWechatOpenTelemetry(builder.Configuration);   // 绑定节 WechatOpenTelemetry

// 或代码配置，返回值可继续追加 SDK 原生配置
builder.Services.AddWechatOpenTelemetry(o =>
{
    o.OtlpEndpoint = new Uri("http://otel-collector:4317");
    o.ServiceName = "my-wechat-app";
    o.SamplingRatio = 0.1;
});
```

## 依赖

- ProjectReference 仅 `Mud.Wechat.Abstractions`（Core：`WechatActivitySource` / `MeterExtensions`）
- `Mud.HttpUtils.OpenTelemetry` 3.0.3（共享装配内核；本包**不直接引用** `Mud.HttpUtils`，`MudHttpActivitySource` / `MudHttpMeter` 由内核内部注册）
- `OpenTelemetry` / `.Extensions.Hosting` / `.Exporter.OpenTelemetryProtocol` / `.Instrumentation.Http` / `.Instrumentation.AspNetCore` 均 1.16.0
- `FrameworkReference Microsoft.AspNetCore.App`（net6.0+；netstandard2.0 不引用）
- TFM：**不覆写，继承根 `Directory.Build.props` 四档** `netstandard2.0;net6.0;net8.0;net10.0`。

## 说明

- **单一入口禁令（必须遵守）**：调用本包后**不得**再调 `AddMudHttpOpenTelemetry()`——两条完整管道叠加会让 Resource `service.name` 被覆盖、同一 Span 被两个 OTLP 导出器重复上报；内核按产品名拒绝不同产品重复注册。仅用微信 SDK 的宿主调本方法即可，Mud.HttpUtils 的源与指标已由 `IncludeMudHttpUtils` 覆盖。
- netstandard2.0 下 ASP.NET Core Instrumentation 由共享内核（`#if NETSTANDARD2_0` 分支）强制置 `false`；net6.0 下 OTel 1.16.0 及其 M.E.* 传递依赖回落 netstandard2.0 资产（与 Redis 包同款提示性警告，`SuppressTfmSupportBuildWarnings` 已处理）。
- 低 TFM 定版对齐根 props（netstandard2.0/net6.0 下 `Microsoft.Extensions.*` 钉 10.0.9，消 `MSB3277`）。
- 安全边界：本包只注册源与导出管线，不采集报文内容；凭据脱敏仍由各产品线日志层负责（标签设计均为非秘密维度，如 `wechat.app_key`）。
- 测试（`Tests/Mud.Wechat.OpenTelemetry.Tests`）：`WechatWildcardMeterTests`（通配 Meter 契约）、`WechatOpenTelemetrySharedKernelTests`（`Mapper_ShouldMapEveryPublicOptionProperty` 逐属性映射、重复入口/幂等）、`WechatOpenTelemetryOptionsValidationTests`（越界拦截）、`WechatOpenTelemetryExtensionsTests`（DI 面）。
