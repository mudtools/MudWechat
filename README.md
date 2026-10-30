# MudWechat

> 微信生态的 **AI 原生** .NET SDK —— 七条产品线一个底座，声明式接口 + 源生成 + AOT 净零 + 多租户隔离，
> 让 AI Agent、MCP 工具服务器与传统业务系统共享同一套可信调用面。

## 为什么做这个

传统 .NET 微信 SDK 普遍采用**静态门面 + 全局容器 + 反射序列化**架构：API 是静态方法、凭据是进程级单例、
序列化靠运行期反射。这套架构在「人写代码调接口」的时代够用，但在 AI Agent 要直接调用微信 API 的时代撞上三堵墙：

- **Agent 无法注入**：静态方法不进 DI 容器，AI Agent 经 `IServiceProvider` 解析不到服务。
- **多租户串号**：全局静态凭据字典，一个 Agent 的令牌失效波及全部，无作用域隔离。
- **AOT 不可发布**：反射序列化在 Native AOT 下大量 trim 警告，MCP 工具服务器无法单文件冷启动。

MudWechat 从第一行代码就为 AI 时代设计：**声明式契约面是 AI 工具 Schema 的生成源，DI 注入是 Agent 的调用入口，
源生成 JSON 是 AOT 部署的前提，多应用作用域切换是多租户隔离的基石。** AI 能力不是外挂补丁，是架构的必然产物。

---

## AI 底座：四个不可替代的架构决策

### ① 声明式接口契约 → MCP 工具 Schema 的编译期生成源

每个微信 API 端点都是一个带特性标注的接口方法：

```csharp
[HttpClientApi(RegistryGroupName = "Contact", TokenManage = nameof(IWechatAppManager))]
public partial interface IWechatWorkInternalUsersService : IWechatWorkWechatServiceBase
{
    [Get("/cgi-bin/user/get")]
    Task<WechatWorkUserResponse> GetUserAsync(
        [Token] [Query("access_token")] string accessToken,
        [Query("userid")] string userId,
        CancellationToken cancellationToken = default);
}
```

`[HttpClientApi]` + `[Token]` + `[Get]` / `[Post]` + `[Query]` / `[Body]` 一贴，实现类由源生成器编译期产出。
**这意味着：** 接口签名本身就是 MCP 工具的 Schema 定义——路由、参数、令牌归属、返回类型全部在编译期可知，
无需运行期反射扫描。以此为输入，一个 MCP 工具生成器可以自动把数百个微信 API 变成强类型、AOT 安全的 MCP 工具，
而不是让 AI 先「搜索 API 目录」再「生成调用代码」。

### ② DI 注入 + 多应用作用域 → 多 Agent / 多租户安全共享

```csharp
// Agent 经 DI 解析服务——不是静态方法调用
app.MapGet("/api/users/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));

// 多租户：一个 Agent 服务多个企业，用 using 作用域隔离
using (switcher.UseCorpScope(appKey: "suite-a", authCorpId, permanentCode))
{
    return await providerUsers.GetUserAsync(userid, ct);  // 作用域内令牌解析到该授权企业
}
```

凭据键三段式 `{tokenType}:{appKey}:{scopeKey}`，取错凭据在入口 **fail-fast** 抛 `WechatTokenOwnerMismatchException`，
不静默串号。一个 SDK 实例可安全服务多个 Agent、多个企业、多个应用类型，互不污染。

### ③ 源生成 JSON + AOT strict → MCP Server 单文件冷启动

全线 DTO 标 `[HttpJsonSerializable]`，`JsonContext` 由源生成器编译期发射，序列化零反射。
`net8.0` / `net10.0` 下逐源工程跑 `AotStrictMode`（10 类反射诊断升为错误）并保持**净零**。

**这意味着：** 基于 Mud.Wechat 构建的 MCP 工具服务器可以 `dotnet publish -p:PublishAot=true`，
产出单文件原生二进制，冷启动 < 100ms——这是 AI Agent 频繁拉起工具进程的性能前提。
传统 SDK 依赖的 `Newtonsoft.Json` 反射序列化在 AOT 下不可用，这条路根本走不通。

### ④ 编译期契约守卫 + Roslyn 分析器 → 漂移打红在编译期

- **契约守卫**（`Tests/**/ContractGuards/`）：官方路由、端点计数、字段名、令牌归属域、事件键与载荷配对
  由守卫源文件钉死，改契约面必须同批更新守卫——**守卫是权威描述，不是改完再补的收尾项**。
- **回调处理器分析器**（`MUDCB002~005`）：`SupportedEventType` ↔ 载荷契约一致性在编译期校验，
  键不匹配直接编译错误，消灭运行期 `ContractMismatch` 静默丢事件。
- **配置属性审计**（`audit-config-keys.ps1`）：每个公开配置属性必须有真实消费点，无消费点即删，
  无白名单、无绕开模式。

**这意味着：** AI Agent 调用的每一个端点，其路由、参数、令牌、返回结构在编译期已被锁定。
不会出现「AI 生成的调用代码在运行期才发现路由已改名」的尴尬。

---

## 七条产品线，一个底座

| 产品线            | 包前缀                        | 业务域 | 端点         | 凭据模型                                                                      |
| ----------------- | ----------------------------- | ------ | ------------ | ----------------------------------------------------------------------------- |
| 企业微信（WeCom） | `Mud.Wechat.Work*`            | 42     | 470 契约接口 | `access_token` / `provider_access_token` / `suite_access_token`（Query 注入） |
| 微信公众号        | `Mud.Wechat.OfficialAccount*` | 27     | 194          | `Wechat.Mp.AccessToken`（普通 + 稳定双通道）+ `jsapi` / `wx_card` 票据        |
| 微信小程序        | `Mud.Wechat.MiniProgram*`     | 21     | 141          | 复用公众号令牌域（同一 `/cgi-bin/token`，不新增令牌类型）                     |
| 微信支付 APIv3    | `Mud.Wechat.Pay*`             | 10     | 57           | 商户 RSA 私钥签名（`WECHATPAY2-SHA256-RSA2048`），**无 `access_token`**       |
| 微信开放平台      | `Mud.Wechat.OpenPlatform*`    | —      | 4            | `component_access_token` + 每授权方令牌（显式提供者）                         |
| 微信小店 / 视频号 | `Mud.Wechat.Channels*`        | 27     | 309 已落地   | `Wechat.Channels.AccessToken`（`token` + `stable_token` 双通道，独立 AppID）  |
| 腾讯广告（在建）  | `Mud.Wechat.Ads*`             | 8      | 33           | OAuth `access_token` + `refresh_token`（每请求现取，不走声明式 `[Token]`）    |

**七线共仓**：共用同一套令牌基座、回调密码学内核、配置基座、SSRF 白名单、可观测性契约面与质量门禁。
各线**独立可选**，按平台装对应主包即可。依赖单向、硬边界由守卫锁定（`Callback` 不引用主包、`Redis` 不引用任何线主包、
广告线与其余六线零引用）。

---

## 核心能力

### 调接口：注入即调用

HTTP 拼装、序列化、令牌获取与提前刷新、errcode 失效恢复——全部自动。你只写业务逻辑。

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddContactApi().AddExternalContactApi());

// 注入客户端直接调用，令牌全生命周期托管
app.MapGet("/api/users/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));
```

### 收推送：回调一条链跑完

验签、AES 解密（官方 32 字节块 PKCS7 手工补位，**不用 .NET 内置 16 字节块**——会误拒官方报文）、
事件解析、分发、被动回复。两道 **fail-closed** 闸默认开启：① 时间戳 ±300s，② 一次性 SHA1 指纹。
指纹在「解密成功后、分发前」消费 ⇒ 重推同报文被 403，**处理器须幂等**。

```csharp
public sealed class MyUserChangeHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override Task HandleAsync(
        WechatCallbackEvent evt, ContactUserChangedPayload payload, CancellationToken ct)
    {
        var deptIds = payload.DepartmentIds;  // 官方 "1,2,3" 已转 List<long>
        return SaveToDbAsync(payload, ct);
    }
}
```

载荷按官方**报文结构族**声明（`[WechatCallbackContract]` + `[PayloadContract]`），字段映射由源生成器编译期产出，
键与载荷一致性由 Roslyn 分析器编译期校验。当前已登记事件键 **122 个** / 结构族载荷 **47 个**，全程零反射。

### 多主体：一套程序服务多个企业 / 商户 / 应用

多应用、多授权企业、多商户按作用域切换，取错凭据在入口抛错而不是静默串号。
企业级令牌一企一份（`scopeKey = authCorpId`），用一次性 `using` 作用域：

```csharp
using (switcher.UseCorpScope(appKey: "suite-a", authCorpId, permanentCode))
{
    // 作用域内所有调用令牌解析到该授权企业，释放时逆序还原
    return await providerUsers.GetUserAsync(userid, ct);
}
```

### 横向扩：一个调用换 Redis

```csharp
builder.Services.AddWechatRedis(builder.Configuration)       // ① 必须先于以下所有
        .AddWechatApp(builder.Configuration)                 // ② 企微多应用
        .AddChannelsApp(builder.Configuration)              // ③ 小店多应用
        .AddWechatCallback(/* ... */);                      // ④ 回调抗重放跨实例
```

令牌、企业授权、套件票据、回调抗重放四个状态从进程内换成 Redis，多实例部署一行接入。

### 智能机器人：JSON 通道 + 长连接

企业微信智能机器人回调为 **JSON 报文**（`{"encrypt":"..."}`），独立接收面，返回式处理器（`null` = 加密空包）。
长连接（`wss://openws.work.weixin.qq.com`）支持帧收发、心跳重连、流式状态机、素材三步上传、Redis 租约主备——
这是 AI Agent 驱动企微机器人的基础设施，仅 `net8.0+` 可用。

---

## 安全默认不可协商

| 闸         | 约束                                                                                                                        |
| ---------- | --------------------------------------------------------------------------------------------------------------------------- |
| 回调抗重放 | 两道 fail-closed：时间戳 ±300s + 一次性 SHA1 指纹；分布式守卫异常必须上抛（→ 5xx → 官方重试），**禁止**吞异常放行           |
| SSRF 防线  | `BaseUrl` 强制 HTTPS + 主机白名单（`AllowCustomBaseUrl=false`）；自定义主机须登记 `WechatCustomBaseUrlRegistry`             |
| 凭据脱敏   | 官方强制放 Query 的凭据参数，一律「进脱敏词表」或「登记带追踪号的自过期豁免」二选一；异常 URL 构造期剥 query 与 userinfo    |
| 凭据不外泄 | `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket` 不进日志、遥测、异常消息 |
| 支付密钥   | 私钥与 APIv3 密钥只以「名称」进配置，运行期经 `ISecretProvider` 取用；启动期拒绝把 PEM 贴进配置                             |
| 配置即校验 | 应用配置在 DI 注册阶段按应用类型完成互斥必填校验，非法组合直接注册期抛错，不潜伏到第一次调用                                |

---

## 与传统方案的差异

| 维度       | 传统 .NET 微信 SDK 的普遍形态                                                  | Mud.Wechat                                                            |
| ---------- | ------------------------------------------------------------------------------ | --------------------------------------------------------------------- |
| API 形态   | 静态类 + 静态方法（`XxxApi.Send(...)`）                                        | 声明式接口 + DI 注入 + 源生成实现类                                   |
| 凭据管理   | 全局静态容器，键 = `AppId`，进程级单例                                         | 三段式键 + 作用域切换 + 归属域 fail-fast 校验                         |
| 序列化     | `Newtonsoft.Json` 反射（AOT 不可用）                                           | 源生成 `JsonContext`（`JsonTypeInfo`），AOT strict 净零               |
| MCP 工具   | 反射调用器（`Type.GetType` + `method.Invoke`）或「API 目录搜索 → AI 生成代码」 | 声明式接口即 Schema 源，可编译期生成强类型 MCP 工具                   |
| AI 集成    | Sample 层补丁（挂 `AIChatAsync()` 于 MessageHandler）                          | 架构原生：DI 注入 + 多租户 + AOT 是 AI Agent / MCP 的前提             |
| 契约校验   | 运行期发现路由 / 字段漂移                                                      | 编译期契约守卫 + Roslyn 分析器打红                                    |
| 回调加解密 | .NET 内置 16 字节块 PKCS7（误拒官方 17~32 字节 pad）                           | 官方 32 字节块手工补位 / 剥离                                         |
| 目标框架   | `net462` + `netstandard2.0` + `LangVersion 10`                                 | `netstandard2.0` / `net6.0` / `net8.0` / `net10.0` + `LangVersion 13` |
| 产品线覆盖 | 单线或数线，各仓独立                                                           | 七线共仓，统一令牌基座 / 回调内核 / 质量门禁                          |
| 配置审计   | 无                                                                             | 每个公开配置属性必须有真实消费点，无白名单无绕开                      |

---

## 安装

**规则一句话：按平台装 1 个主包；要收推送再加 1 个 `.Callback`；多实例加 `Mud.Wechat.Redis`；要看链路加 `Mud.Wechat.OpenTelemetry`。** 基座、DTO、客户端接口与 DI 注册全在主包内，无需逐个安装。

```bash
# 企业微信
dotnet add package Mud.Wechat.Work
dotnet add package Mud.Wechat.Work.Callback          # 收推送 / 智能机器人
# 微信公众号
dotnet add package Mud.Wechat.OfficialAccount
dotnet add package Mud.Wechat.OfficialAccount.Callback
# 微信小程序（凭据复用公众号底座，无回调包）
dotnet add package Mud.Wechat.MiniProgram
# 微信支付（APIv3 与 APIv2 同包）
dotnet add package Mud.Wechat.Pay
dotnet add package Mud.Wechat.Pay.Callback           # 收支付通知
# 微信小店 / 视频号
dotnet add package Mud.Wechat.Channels
dotnet add package Mud.Wechat.Channels.Callback
# 微信开放平台（第三方平台 component）
dotnet add package Mud.Wechat.OpenPlatform
# 腾讯广告（在建）
dotnet add package Mud.Wechat.Ads
# AI 能力（Agent / MCP 工具服务器）
dotnet add package Mud.Wechat.AI                     # 跨七线中立层
dotnet add package Mud.Wechat.Work.AI                # 企微 Agent 底座
dotnet add package Mud.Wechat.Work.AI.Tools          # 策展工具面
dotnet add package Mud.Wechat.Work.AI.Mcp            # MCP 协议层
# 跨线通用
dotnet add package Mud.Wechat.Redis                  # Redis 分布式存储
dotnet add package Mud.Wechat.OpenTelemetry          # 可观测性装配
```

全仓共 **18 个 NuGet 包**（2026-10 包合并：原 35 个）。合并只改程序集与包归属，**命名空间一字未动** —— 老写法的 `using` 与契约面照旧可用。

## 快速开始

### 企业微信

`appsettings.json`（按 `AppType` 校验互斥必填项，配置错误在 DI 注册阶段即抛出）：

```json
{
  "WechatApps": [
    {
      "AppKey": "default",
      "AppType": "Internal",
      "CorpId": "ww-your-corp-id",
      "AgentId": "1000002",
      "AgentSecret": "your-agent-secret"
    }
  ]
}
```

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b
    .AddContactApi()
    .AddExternalContactApi());

app.MapGet("/api/users/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));
```

### 微信公众号 / 小程序

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps")
        .AddMpServices(b => b.AddAllApis());         // 27 个业务域
builder.Services.AddMpApp(builder.Configuration, "MpApps")
        .AddMiniProgramServices(b => b.AddAllApis()); // 21 个业务域 141 端点
app.UseMpCallback();
```

### 微信支付 APIv3

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
        .AddWechatPayApi(b => b.AddAllApis());       // 10 个域 57 端点
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
        .AddWechatPayCallback(builder.Configuration)
        .AddHandler<TransactionSuccessHandler>("TRANSACTION.SUCCESS");
app.UseWechatPayCallback();
```

### 微信小店 / 视频号

```csharp
builder.Services.AddChannelsApp(builder.Configuration);
builder.Services.AddWechatChannelsApi(b => b.AddAllApis());  // 29 个业务域 394 端点
builder.Services.AddWechatChannelsCallback(/* ... */).AddHandler<T>(...);
app.UseWechatChannelsWebhook();
```

### 回调接收

```csharp
builder.Services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";
    options.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-your-corp-id",
        AppType = WechatAppType.Internal,
        Channel = WechatCallbackChannel.App
    };
})
.AddHandler<MyUserChangeHandler>()
.AddInterceptor<MyAuditInterceptor>();

app.UseWechatWebhook();
```

### 可观测性

```csharp
builder.Services.AddWechatOpenTelemetry(o =>
{
    o.OtlpEndpoint = new Uri("http://localhost:4317");
    o.SamplingRatio = 1.0;
});  // Tracing + Metrics 默认开；ActivitySource 名恒 Mud.Wechat
```

---

## 质量门禁

```bash
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1    # 构建 + AOT strict + 测试
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-config-keys.ps1 # 配置消费点审计
```

- **20 个测试工程**（单 TFM `net8.0`）覆盖七条产品线 + Core 叶层 + Redis + OpenTelemetry + AI 域。
- **契约守卫**：企业微信 74 个守卫文件、公众号 28 个、小程序 MP-X1~X9、支付 PAY-B1~B11、小店 CH 系列、广告 ADS 系列。
- **AOT / 裁剪**：`net8.0` / `net10.0` 下逐源工程 `AotStrictMode` 净零；六条线共 **132 个源生成 JSON 上下文**。
- **包清单单一来源**：可打包集由 `Src/**/*.csproj` 现场推导，CI / `pack.bat` / `publish.bat` 三处各自推导、不持有名单。
- **新增产品线门禁接入口唯一**：AB-G6 同时校验解决方案工程清单、配置审计搜索根、DTO 标注脚本根命名空间、三处推导口径。

---

## 包家族

`Src/` 下 **20 个源工程** = **18 个可打包 nupkg** + 2 个构建期工具（`IsPackable=false`，随四个回调包以 `analyzers/dotnet/cs` 内嵌下发）。

2026-10 完成**包合并**（原 35 包）：一条线一个入口包——基座 + 官方 DTO + 声明式客户端 + DI 注册同处一个程序集；只有 `Callback`（把 ASP.NET Core 依赖挡在宿主侧）与 `Redis` / `OpenTelemetry`（可选基建）保持独立。**合并未改任何命名空间**，老写法的 `using` 与契约面照旧可用。

```
Src/
├── Core/                # 跨线共享基建（3 包 + 2 工具）：Abstractions 叶层 / Redis / OpenTelemetry / Callback 生成器与分析器
├── Work/                # 企业微信（2 包：主包 + Callback）
├── OfficialAccount/     # 公众号（2 包）
├── MiniProgram/         # 小程序（1 包，无 Callback）
├── Pay/                 # 微信支付（2 包：APIv3 + APIv2 同包，Callback 独立）
├── OpenPlatform/        # 开放平台（1 包）
├── Channels/            # 微信小店 / 视频号（2 包）
├── Ads/                 # 腾讯广告（1 包，在建）
└── AI/                  # AI 能力域（4 包：Mud.Wechat.AI 中立层 + Work.AI / .Tools / .Mcp）
```

依赖单向：`{线}.Callback → {线}`、`Redis → {叶层, Work}`、`MiniProgram → {叶层, OfficialAccount}`（令牌底座复用）、`AI 层 2 → {Mud.Wechat.AI, Work}`；跨线边白名单由守卫 `AB-G2` 钉死，其余跨线引用零回潮。
目标框架：企业微信 / 公众号 / 小程序 / 小店 / 广告 / Core 线为 `netstandard2.0` / `net6.0` / `net8.0` / `net10.0`；
**微信支付与开放平台线为 `net6.0` / `net8.0` / `net10.0`**（前者因 `AesGcm` 在 ns2.0 不存在，后者刻意收窄到组件最小面）；**AI 域为 `net8.0` / `net10.0`**（`Mud.AI.*` 传递要求的 `M.E.*` 10.x 支持面）。全仓 `LangVersion 13.0`。

各包的职责、公开面、配置节与已踩陷阱见对应目录下的 `README.md`；跨域不可违反的约束见 `AGENTS.md`；包结构为何是这个形态、合并中踩到哪些坑见 `.docs/MudWechat-包合并实施方案-v1.md`。

---

## 目录结构

```
Mud.Wechat/
├── Src/                    # 20 个源工程（18 包 + 2 构建期工具）
├── Tests/                  # 20 个测试工程（含 ContractGuards/）
├── Demos/                  # 示例工程
├── scripts/                # verify-build / audit-config-keys / GenerateJsonContext / ...
├── Directory.Build.props   # 全局 MSBuild 属性（TFM / LangVersion / Version 唯一来源）
├── pack.bat / publish.bat  # 打包与推送
└── Mud.Wechat.slnx         # 解决方案
```

## 许可证

本项目主要遵循 MIT 许可证进行分发和使用，许可证位于源代码树根目录中的 `LICENSE-MIT` 文件。

不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动。任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任。
