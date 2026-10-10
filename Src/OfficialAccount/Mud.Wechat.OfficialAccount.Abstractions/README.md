# Mud.Wechat.OfficialAccount.Abstractions

微信公众号 / 服务号 SDK **基座包**：令牌双通道与管理器、api_ticket 基座、多公众号管理与作用域切换、配置面、回调信封与载荷契约面、常量/枚举/异常/可观测性。对应企微线的 `Mud.Wechat.Work.Abstractions`，零 ASP.NET 依赖。

## 内容

- **令牌基座**（`Authentication/`）：`IMpAuthentication` 声明式签发客户端（注册组 `Authentication`，接口上刻意**不带 `[Token]`**）承载 2 条路由——`GET /cgi-bin/token`（普通通道）与 `POST /cgi-bin/stable_token`（稳定版通道）。`MpAccessTokenManagerBase : TokenManagerBase`（internal）把提前刷新阈值退化为 `min(TokenRefreshThreshold, expires_in / 2)`；`MpStableAccessTokenManager` 的 `force_refresh` 恒为 `false`（强制刷新只经 errcode 恢复链路，不允许业务侧随手绕过缓存）；`MpStandardAccessTokenManager` 走普通通道。失效判定由 `MpTokenInvalidationDetector` 单点承担（`{40001, 40014, 42001}`）。对外契约 `IMpAccessTokenManager : ITokenManager` 为空壳，实现一律 internal——消费方只能经 `IMpAppContext.AccessTokenManager` / `IMpAppContext.GetTokenManager(MpTokenTypes.AccessToken)` / `IMpAppManager.DefaultAccessTokenManager` 取用。
- **票据基座**（`Authentication/Tickets/`）：`IMpTicketService`（`GET /cgi-bin/ticket/getticket`）+ `MpTicketManagerBase` + `IMpJsApiTicketManager` / `IMpWxCardTicketManager`。官方明示 api_ticket 调用次数极有限 ⇒ **宿主不得绕过管理器逐次直调端点**；票据与令牌共用持久化仓储，键空间以 `Wechat.Mp.JsApiTicket` / `Wechat.Mp.WxCardTicket` 前缀隔离，互不覆盖。
- **多公众号基座**（`Authentication/MultiApp/`）：`IMpAppManager : IAppManager<IMpAppContext>` 补 `DefaultConfig` / `ConfiguredAppKeys` / `ConfiguredConfigs` / `TryGetConfig` / `DefaultAccessTokenManager` / `InvalidateTokenAsync(appKey, tokenType, scopes, ct)` / `PurgeAppTokensAsync` / `new GetAllApps()`；实现 `MpAppManager` 为 `internal sealed` **单槽注册表** + `IServiceScopeFactory`（按上下文建 scope，避免 Captive Dependency），注册表单一来源 = 注册期配置列表（配置不可变，无运行时替换语义 ⇒ 不设退役宽限队列）。切换器 `IMpAppContextSwitcher : IAppContextSwitcher, IAppScopeSwitcher` 为**空接口体**——公众号无企业级 scope ⇒ 无 `UseCorpScope` / `SetCorp` / `ClearCorp`。另有 `MpAuthenticationFactory` / `MpTicketFactory` / `MpHttpClientFactory`（每应用具名 HttpClient）与 `MpTokenRegistrationService`（net6+ 的 `IHostedService`，启动期预热令牌）。
- **配置面**（`Configuration/`）：`MpAppConfig : WechatAppConfigBase`（基类给 `AppKey`/`BaseUrl`/`AllowCustomBaseUrl`/`TimeoutSeconds=30`/`TokenRefreshThreshold=300`/`IsDefault`，本类补 `AppId`/`AppSecret`/`UseStableToken=true`，`DefaultBaseUrl = WechatApiHosts.OfficialAccountBaseUrl`）；`Validate()` 追加双凭据必填（启动即失败），`ToString()` 掩码输出；`MpAppConfigValidator` 经 `IValidateOptions` 兜底。范围仅覆盖自建形态，不暴露第三方平台 `component_*` / `authorizer_*` 字段。
- **回调契约面**（`Callback/`）：`MpCallbackEnvelope`（官方字段直出 + `EventTypeKey` 归一键；`AppKey`/`SecurityMode`/`IsEncrypted` 由 Callback 包内部写入）；`MpCallbackKeys.cs` 给安全模式枚举 `Plain/Compatible/Safe` 与常量类——消息类型 8、回复类型 7、事件类型 13、卡券事件 13、授权事件 3、订阅消息事件 3、认证事件 6、群发/模板回执 3；处理器模型 `IMpCallbackEventHandler`（`SupportedEventType` 为空即兜底）/ `IMpCallbackPayloadHandler` / `IMpCallbackEventHandler<TPayload>` + 抽象基类 `MpCallbackPayloadHandler<TPayload>`；声明化载荷契约 `[MpCallbackContract(EventTypes = ...)]`（`AllowMultiple`，29 处声明 / 登记 48 个事件键，守卫锁定）+ `MpPayloadContract` / `IMpPayloadContractRegistry` / `MpPayloadContractRegistry` / `IMpPayloadReader`。**源生成器挂在本工程**：`MpPayloadContracts` 的 partial 宿主与 `RegisterAll` 方法体必须同属一个程序集，而载荷 DTO 与宿主都落本包（宿主引本包即可定义类型化处理器）。`IMpCallbackSourceIpProvider` 为动态来源 IP 白名单端口（实现由主包提供）。加解密/验签/抗重放在公用层与 Callback 包。
- **常量/枚举/异常/可观测性**：`MpMenuButtonTypes`（12 种按钮类型）、`MpKfConstants`、`MpCustomMessageTypes`、`MpCallbackCheckActions`（`dns`/`ping`/`telnet` 三值，无其他取值）、`MpErrorCodes`、`MpException : WechatApiException`（`ThrowIfFailed` 单点判定）、`MpMetrics.MeterName = "Mud.Wechat.OfficialAccount"` 与 `MpActivityNames`。

## 令牌键布局

令牌类型路由键**只有一个**：`MpTokenTypes.AccessToken = "Wechat.Mp.AccessToken"`——接口层 `[Token]` 声明、errcode 恢复链路、`InvalidateTokenAsync` 默认参数全部用它。
普通/稳定通道的差异不外泄到声明面，只体现在两处：注册期按 `MpAppConfig.UseStableToken` 选实现类；持久化存储键前缀取 `Wechat.Mp.StandardAccessToken` 或 `Wechat.Mp.StableAccessToken`。
新增令牌类型键属红线操作：`MpAppManager` / `MpTokenManagerRegistry` 是单槽实现，未登记的键 `Resolve` 返回 `null` ⇒ errcode 自愈静默失效（小程序线因此复用本键，见其 README）。

## 用法（配置与作用域切换）

### 配置形状

```json
{
  "MpApps": [
    { "AppKey": "default", "AppId": "wx000000000000000a", "AppSecret": "…" },
    { "AppKey": "brand-b", "AppId": "wx000000000000000b", "AppSecret": "…",
      "UseStableToken": false, "TokenRefreshThreshold": 600 }
  ]
}
```

### 注册（三个重载）

```csharp
// 1) 反射式配置绑定：裁剪 / Native AOT 下有 [RequiresUnreferencedCode] 告警
builder.Services.AddMpApp(builder.Configuration, "MpApps");      // sectionName 默认 "MpApps"

// 2) 代码配置单应用 / 3) 代码配置多应用（推荐：绑定形状可静态分析）
builder.Services.AddMpApp(cfg => { cfg.AppKey = "default"; cfg.AppId = appId; cfg.AppSecret = secret; });
builder.Services.AddMpApp(apps);                                   // List<MpAppConfig>
```

`AppKey == "default"` 者自动成为默认应用；多个 `IsDefault` 注册期即抛。`AddMpApp` 只能调用一次（重复注册 fail-fast），且必须先于业务模块与 `AddMudHttpClient`——`IMpAppContextSwitcher` / `IAppContextHolder` / `IAppContextSwitcher` 必须解析到**同一实例**，否则声明式 `[Token]` 客户端读到的上下文恒为 `null`，多公众号下会静默回落到默认公众号令牌。

### 作用域切换与直接取用凭据

```csharp
using Mud.Wechat.OfficialAccount.Abstractions;                      // AddMpApp / MpTicketTypes
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;       // 管理器 / 切换器 / 票据管理器

public sealed class MpCredentialProbe(
    IMpAppContextSwitcher switcher, IMpAppManager apps, IMpJsApiTicketManager jsApiTicket)
{
    // using 一次性作用域：进入时写入应用上下文快照，释放时幂等还原（可嵌套）
    public async Task<string> TokenOfAsync(string appKey, CancellationToken ct)
    {
        using var scope = switcher.UseAppScope(appKey);
        return await switcher.GetTokenAsync();                     // 作用域内解析到该公众号
    }

    // 不切换上下文，按配置直接定位（配置读取不触发懒加载实例化）
    public bool TryConfigure(string appKey, out string? appId)
    {
        var hit = apps.TryGetConfig(appKey, out var cfg);
        appId = hit ? cfg!.AppId : null;
        return hit;
    }

    // 票据：管理器负责存储与提前刷新，宿主取走后自行用于前端签名
    public Task<string> JsApiTicketAsync(CancellationToken ct) => jsApiTicket.GetTicketAsync(ct);

    // 凭据轮换后清库（返回实际删除的键数量）
    public Task<int> PurgeAsync(string appKey, CancellationToken ct)
        => apps.PurgeAppTokensAsync(appKey, ct);
}
```

## 依赖

- `Mud.Wechat.Abstractions`（公用层：配置基座、令牌存储端口、响应契约与异常基底、回调密码学与重放端口、`WechatApiHosts`）
- `Mud.Wechat.OfficialAccount.DataModels`
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（分析器）、`Mud.Wechat.Callback.Generator`（分析器，仅生成登记体，不产运行时引用）
- `Microsoft.Extensions.Http` / `Hosting.Abstractions`（net8.0/net10.0 → 10.0.9，netstandard2.0/net6.0 → 8.0.1）

## 说明

- 目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`、`Nullable enable`），可打包。
- 配置 DTO 禁用 `required`（`ConfigurationBinder` 以 `new T()` 构造 ⇒ `CS9035`），必填校验统一在 `Validate()`。
- `AddMpApp` 落在本包而非主包：注册期需调用本程序集 internal 的 `AddAuthenticationWebApiHttpClient()`（源生成的令牌/票据客户端入口），主包跨不过程序集边界。
- `UseStableToken` 默认 `true`（官方推荐通道，普通通道令牌照会互斥失效）；按应用维度选择，同一实例可对不同公众号混用两通道。
- 令牌与票据的默认存储为进程内实现（`TryAdd` 语义）：多实例部署须由宿主先行注册分布式 `IWechatTokenStore`，或引用 `Mud.Wechat.Redis`。
- `netstandard2.0` 下禁 `init`/`record`/`with`、无 `ArgumentNullException.ThrowIfNull`；抽象基类为规避 `CS8701`（接口默认实现限制）而提供空实现虚方法。
- `InternalsVisibleTo`：主包、Callback 包及两者测试项目 + `DynamicProxyGenAssembly2`——internal 的管理器/工厂/注册表不对外暴露，属刻意收敛。
- `MUD005`（Query 传令牌）与 `SYSLIB1100/1101` 项目级抑制：前者源于官方契约强制 `access_token` 走 Query，后者源于配置反射绑定与 internal 上下文。
