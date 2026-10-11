# Mud.Wechat.Abstractions

Mud.Wechat 全仓的**跨产品线共享叶层**：七条产品线（企业微信 / 公众号 / 小程序 / 开放平台 / 微信支付 / 微信小店·视频号 / 腾讯广告）共同依赖的最底层包——响应契约、令牌存储端口与桥接编解码、回调密码学与信封契约、配置基座、异常判定、可观测性契约面。

**零 ProjectReference**（契约守卫 AB-G1 锁定），依赖单向向下收敛：任何包都可以引它，它不引任何本仓工程。

## 内容

- **响应契约**（`Contracts/`）：`IWechatApiResponse`（`ErrorCode` / `ErrorMessage` / `IsSuccess`）——各产品线响应 DTO 的统一基底，Work.DataModels 等据此实现。
- **异常判定**（`Exceptions/`）：`WechatApiException` 与 `WechatApiResponseGuard.ThrowIfFailed<TResponse>(...)`——把「官方 `errcode != 0`」的判定收敛为单点，各线不再各写一份。
- **令牌存储端口**（`TokenManager/`）：`IWechatTokenStore`（继承组件 `ITokenStore`）与可选批量端口 `IWechatTokenStoreBatchRemove`；`InMemoryWechatTokenStore` 为默认进程内实现；`WechatTokenBridgeCodec`（internal）负责 `令牌 + 过期毫秒` 的单键存储编码；`WechatCompositeTokenInvalidationDetector` 组合各线失效码判定器。
- **令牌恢复注册**（`TokenManager/WechatTokenRecoveryRegistration.cs`）：
  - `AddWechatApiHosts(IServiceCollection)`——SSRF 白名单的唯一登记点（把 `WechatApiHosts.AllowedBaseUrlDomains` 交予组件侧）；
  - `AddWechatTokenRecovery(IServiceCollection)`——注册恢复选项、校验器与令牌提供者，内部先调 `AddWechatApiHosts`，并对后台刷新服务做 `Any(descriptor…)` 去重（防多线重复注册产生多个刷新宿主）。
- **回调共享内核**（`Callback/`）：`WechatCallbackCrypto`（SHA1 排序验签 + AES-256-CBC + 官方 32 字节块手工 PKCS7 补位/剥离、`Encrypt`/`Decrypt(out receiveId)`/`ParseSignatureQuery`）、`IWechatCallbackEnvelope`、`IWechatCallbackReplayGuard` + `InMemoryWechatCallbackReplayGuard`（fail-closed 语义在端口层定义）、`WechatCallbackException` / `WechatCallbackFailureKind` / `WechatCallbackDispatchOutcome`、`WechatCallbackQuery`、`WechatCallbackRouteKeys.Wildcard`（`"*"`）、`XElementPayloadSource`（同名兄弟合并投影器）与 `WechatPayloadSourceCache`、`WechatCallbackTypeRegistry<T>`（「应用键 → 类型」注册表基座：专属桶先于通配桶的匹配序在此单点定义，各线以派生类承载自己的处理器标记接口）。
- **配置基座**（`Configuration/WechatAppConfigBase`）：`AppKey` / `BaseUrl` / `AllowCustomBaseUrl` / `TimeoutSeconds` / `TokenRefreshThreshold` / `IsDefault` 等各线公共属性，子类只补本线凭据。
- **主机与键形状护栏**（根目录）：`WechatApiHosts`（`WorkBaseUrl = https://qyapi.weixin.qq.com`、`OfficialAccountBaseUrl = https://api.weixin.qq.com`、`AllowedBaseUrlDomains`）、`WechatAppKeyValidator`（`[A-Za-z0-9]` 开头 + `[\w.\-_]` + ≤128，防令牌键别名）、`WechatCustomBaseUrlRegistry`（internal，自定义主机登记）。
- **可观测性契约面**（`Observability/` `Metrics/`）：`WechatActivitySource`（源名恒 `Mud.Wechat`、`Tags.Product/AppKey/Outcome/ErrorType/CorrelationId`、`Products.{work,officialaccount,miniprogram,openplatform,pay}`）与 `MeterExtensions.RecordDuration(Histogram<double>[,TagList])`。**本包只定义契约，不引入 OpenTelemetry 依赖**——装配层在 `Mud.Wechat.OpenTelemetry`。

## 用法

宿主通常不直接使用本包（它随各线主包传递引入）。仅在两种场景需要显式打交道：

```csharp
// ① 自建存储端口实现（替换默认进程内实现）时，端口在本包：
services.TryAddSingleton<IWechatTokenStore>(new MyTokenStore());
services.TryAddSingleton<IWechatCallbackReplayGuard>(new MyReplayGuard());   // 故障必须上抛，不得吞

// ② 产品线无关的响应判定与令牌恢复装配（各线 DI 入口内部即走这两步）：
services.AddWechatTokenRecovery();
WechatApiResponseGuard.ThrowIfFailed(response);        // 非零 errcode → WechatApiException
```

自研 Activity / Meter 时按契约面打标签，勿另起键名：

```csharp
using var activity = WechatActivitySource.Instance.StartActivity("send");
activity?.SetTag(WechatActivitySource.Tags.Product, WechatActivitySource.Products.Work);
```

## 依赖

- `Mud.HttpUtils` 3.0.3（唯一外部依赖）
- 被引而不引：`Work.Abstractions` / `OfficialAccount.Abstractions` / `MiniProgram.Abstractions` / `Pay.Abstractions` / `OpenPlatform.Abstractions` / `Redis` / `OpenTelemetry` 等下游包均依赖本包。
- `InternalsVisibleTo` 授予各线主包与测试工程（含 `DynamicProxyGenAssembly2` 供 Moq）。

## 说明

- **目标框架**：`netstandard2.0` / `net6.0` / `net8.0` / `net10.0`（继承根 `Directory.Build.props`）。`netstandard2.0` 无 `IsExternalInit` polyfill ⇒ 本包内 `init` / `record` 一律禁用。
- **回调密码学的填充口径不可「修正」**：官方按 32 字节块手工 PKCS7，pad 取值范围 17..32，禁用 .NET 内置 `PaddingMode.PKCS7`（16 块）否则会误拒合法报文。
- **抗重放是 fail-closed 语义**：`IWechatCallbackReplayGuard` 实现遇到存储故障必须原样上抛（→ 5xx → 官方重试），禁止吞异常返回 `true`（放行重放）或 `false`（静默丢事件且官方不再重试）。
- **Pay 线不使用本包的 XML 密码学**（其为 JSON + AEAD-GCM 体系），只复用响应契约、配置基座与可观测性契约面。
- 本包的契约面由 `Tests/Mud.Wechat.Abstractions.Tests/ContractGuards/` 锁定：`WechatAbstractionsContractGuards`（AB-G1 叶子无工程引用 … AB-G7 回调宿主包必须产 nupkg 且内嵌 `analyzers/dotnet/cs`、两工具工程 `IsPackable=false`）与 `WechatCallbackKernelContractGuards`（CB-L1 系列，编号与企微侧 CB1~CB24 避让）。新增/改动共享面须同批更新守卫。
- 七条产品线在同一宿主共存的能力边界由 `CrossProductLineCoexistenceTests`（X1~X7）锁定——尤其「同一进程内各线令牌互不串号」。
