# Mud.Wechat

微信生态的 .NET SDK：**企业微信、公众号、小程序、微信支付 APIv3、开放平台第三方平台、微信小店 / 视频号（channels 生态）**六条产品线装在一个解决方案里，共用同一套令牌基座、回调内核与质量门禁；另有一条**在建**的腾讯广告（Marketing API v3.0）线，治理骨架与门禁已接入、并落地 OAuth 与八个业务域。

**能干什么**

- **调接口**：注入声明式客户端接口直接调微信开放 API——HTTP 拼装、序列化、令牌获取与提前刷新、errcode 失效恢复都不用你写。
- **收推送**：回调验签、解密、事件解析、分发、被动回复一条链跑完，时效窗与抗重放默认开启。
- **一套程序服务多个主体**：多应用 / 多授权企业 / 多商户按作用域切换，取错凭据在入口抛错而不是静默串号。
- **横向扩到多实例**：一个 `AddWechatRedis` 调用把令牌、企业授权、套件票据、抗重放四个状态从进程内换成 Redis。

**特色**

- **声明式 + 源生成**：`[HttpClientApi]` + `[Token]` 一贴，实现类由生成器编译期产出；序列化与配置绑定全走源生成，AOT / 裁剪下零反射。
- **契约漂移打红在编译期**：官方路由、字段名、事件键与载荷字段的配对由契约守卫和 Roslyn 分析器钉死，不等线上暴露。
- **安全默认不可协商**：回调两道闸 fail-closed；`BaseUrl` 强制 HTTPS + 主机白名单；支付线的私钥与 APIv3 密钥只以「名称」进配置。
- **老宿主到 net10.0 同一套 API**：从 `netstandard2.0`（.NET Framework 4.6.1+）一路覆盖，只用原生 `IConfiguration` / `ILogger` / DI / `ActivitySource`，不自建抽象层、不绑架你的架构。

各线的域数、端点数、事件键与载荷数见下文「能力全景」，包与守卫的分布见「包家族」与「质量门禁」。架构对齐 Mud.Feishu（FeishuV3）——包家族、令牌基座、契约守卫与质量门禁模式一致，两套 SDK 使用体验高度一致。各产品线**独立可选**，按平台装对应主包即可（其余随依赖传递）：

| 产品线 | 平台 | 包前缀 | 凭据 / 令牌模型 |
| --- | --- | --- | --- |
| 企业微信（WeCom） | 自建 / 第三方（Suite）/ 服务商代开发 | `Mud.Wechat.Work*` | `access_token` / `provider_access_token` / `suite_access_token`（Query 注入，官方契约） |
| 微信公众号 | 订阅号 / 服务号 | `Mud.Wechat.OfficialAccount*` | `Wechat.Mp.AccessToken`（普通与稳定令牌双通道）+ `jsapi` / `wx_card` 票据 |
| 微信小程序 | 小程序 | `Mud.Wechat.MiniProgram*` | **复用公众号令牌域**（同一 `/cgi-bin/token` 端点，不新增令牌类型） |
| 微信开放平台 | 公众平台第三方平台（component 体系） | `Mud.Wechat.OpenPlatform*` | `component_access_token` + 每授权方令牌（显式提供者，不走声明式 `[Token]`） |
| 微信支付 | APIv3（商户 / 服务商） | `Mud.Wechat.Pay*` | **商户 RSA 私钥签名**（`WECHATPAY2-SHA256-RSA2048`），**无 `access_token`** |
| 微信小店 / 视频号（channels 生态） | 微信小店（含视频号小店升级形态：内容运营 + 交易管理 + 本地生活三面） | `Mud.Wechat.Channels*` | `Wechat.Channels.AccessToken`（`/cgi-bin/token` 普通 + `stable_token` 稳定双通道，**独立小店 AppID**，与公众号/小程序不互通） |
| 腾讯广告（在建） | Marketing API v3.0 | `Mud.Wechat.Ads*` | OAuth `access_token` + `refresh_token`，官方要求 `access_token`/`timestamp`/`nonce` **成组、每请求现取** ⇒ 不走声明式 `[Token]` |

**配置即校验**：应用配置在 DI 注册阶段就按应用类型完成互斥必填校验（自建 `CorpId`+`AgentSecret`、第三方 / 代开发 `CorpId`+`ProviderSecret`+`SuiteId`+`SuiteSecret`），非法组合直接注册期抛错，不潜伏到第一次调用；模板 id 不提供独立属性（代开发 `template_id` 即 `suite_id`，独立字段等于允许非法状态）。

**凭据不外泄**：`AppKey` 形状受约束（含 `:` 会造成令牌键别名、跨应用串号）；企微侧被官方强制放 Query 的凭据参数，一律要求「进脱敏词表」或「登记带追踪号的自过期豁免」二选一；异常消息里的 URL 在构造期剥掉 query 与 userinfo。

## 包家族

`Src/` 下 **29 个源工程**（`Src/Core` 4 + `Src/Work` 4 + `Src/OfficialAccount` 4 + `Src/MiniProgram` 3 + `Src/Pay` 4 + `Src/OpenPlatform` 3 + `Src/Channels` 4 + `Src/Ads` 3），其中 **27 个产出 nupkg**、2 个构建期工具工程 `IsPackable=false`。可打包集**不在任何清单里硬编码**：CI、`pack.bat`、`publish.bat` 均按 `Src/**/*.csproj` 现场推导（`IsPackable` 判定），唯一被维护的名单是 AB-G6 里的「非可打包工程白名单」（仅那两个工具工程）。

### 共享层（Src/Core）

| 包 | 说明 |
| --- | --- |
| `Mud.Wechat.Abstractions` | **跨产品线共享叶层**（零工程引用）：响应契约 `IWechatApiResponse`、令牌存储端口与桥接编解码、回调密码学内核 `WechatCallbackCrypto` 与重放端口、配置基座 `WechatAppConfigBase`、`WechatApiHosts`（SSRF 白名单单一来源）、`WechatActivitySource` 可观测性契约面 |
| `Mud.Wechat.Redis` | Redis 分布式存储：四个存储端口（令牌 / 企业授权 / 套件票据 / 回调抗重放）的 Redis 实现 + 连接基座 + 健康检查 + 顺序守卫 |
| `Mud.Wechat.Callback.Generator` | 回调契约登记源码生成器（产品线中立，按档位发射企微 `OfficialPayloadContracts.RegisterAll` 与公众号 `MpPayloadContracts.RegisterAll`；小店回调档位随 P3 加挂；不打包） |
| `Mud.Wechat.Callback.Analyzers` | 回调处理器契约分析器（`MUDCB002~005`，只诊断不发射；随四个回调宿主包——企微 / 公众号 / 支付 / 小店——内嵌 `analyzers/dotnet/cs` 下发；不打包） |

依赖单向：各线主包 → `{本线 Abstractions, 本线 DataModels}` → `Mud.Wechat.Abstractions`。硬边界：`Callback` 包不引用同线主包；`Redis` 不引用任何线的主包 / Callback 包；公众号线与小程序线之间只允许「小程序 → 公众号 Abstractions」一条边；**广告线与其余六线之间零引用**（它是第一条非微信域线，任何跨线引用都会把微信线的令牌假设带进 `api.e.qq.com`，由 ADS-S1 锁定）。

### 企业微信线（Src/Work）

| 包 | 说明 |
| --- | --- |
| `Mud.Wechat.Work` | 主包：35 个业务域声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器、**会话内容存档 C SDK 原生封装**（`ExtendedSDK/Finance/`，仓内唯一不经 HTTP 的能力面） |
| `Mud.Wechat.Work.Abstractions` | 认证与多应用基座：令牌签发客户端与四管理器、多应用管理、配置面、回调事件信封与载荷契约、智能机器人信封 |
| `Mud.Wechat.Work.DataModels` | 官方 DTO + 63 个域 AOT 源生成 JSON 上下文 |
| `Mud.Wechat.Work.Callback` | 回调接收：验签、AES 解密、事件分发、HTTP 中间件、抗重放守卫、智能机器人 JSON 通道 |

### 其余六条线

| 包 | 说明 |
| --- | --- |
| `Mud.Wechat.OfficialAccount` / `.Abstractions` / `.DataModels` / `.Callback` | 公众号：27 个业务域声明式客户端；令牌与票据基座（普通 / 稳定双通道）；官方 DTO；消息与事件回调（明文 / 兼容 / 安全三模式、被动回复写回） |
| `Mud.Wechat.MiniProgram` / `.Abstractions` / `.DataModels` | 小程序：19 个业务域 84 端点（登录 / 二维码与链接 / 内容安全 / 数据分析 / 订阅消息 / 动态消息 / 客服 / 硬件设备 / 运维 / 插件 / 付费 / 附近小程序 / 搜一搜 / 生物认证 / 服务市场 / 红包封面 / 学生身份 / 人脸核身 / 用工关系）；复用公众号令牌底座；官方 DTO。**无 Callback 工程**（消息接收走公众号线 XML 通道，由脚手架守卫锁定） |
| `Mud.Wechat.Pay` / `.Abstractions` / `.DataModels` / `.Callback` | 微信支付 APIv3：10 个业务域 57 端点；商户配置面与签名/验签端口；官方 DTO（snake_case 字段名照官方）；通知接收（平台证书验签 + AEAD-GCM 解密 + 三道 fail-closed 闸） |
| `Mud.Wechat.OpenPlatform` / `.Abstractions` | 开放平台第三方平台：component 令牌与授权方令牌提供者、预授权码 / 换授权 / 刷新令牌、`component_verify_ticket` 与授权变更事件接收 |
| `Mud.Wechat.Channels` / `.Abstractions` / `.DataModels` / `.Callback` | 微信小店 / 视频号（channels 生态）：27 个业务域声明式客户端（规划，P1 起逐域落地）；双通道令牌基座（`token` / `stable_token`，`UseStableToken` 切换）；官方 DTO；回调接收（msg_signature + EncodingAESKey + receiveid，与公众号同构，复用 Core 密码学与抗重放两道闸） |
| `Mud.Wechat.Ads` / `.Abstractions` / `.DataModels` | **腾讯广告 Marketing API v3.0（在建）**：8 个业务域 **33** 支端点（客户账号 3 + 营销单元 8（含 4 支批量）+ 报表 4 + 组件化创意 4 + 创意组件 4 + 图片素材 4 + 视频素材 4 + 异步任务 2，其中 `images/add` / `videos/add` 为 `multipart/form-data` 文件上传、走手写通道）+ OAuth 两支（换码 / 刷新，手写传输不走声明式客户端）；`AddAdsApp` 装授权与传输底座，`AddWechatAdsApi` 装业务接口；官方 DTO + 10 个域 AOT 源生成上下文。**仍未落地**：`async_report_files/get`（请求地址在 `dl.e.qq.com`，与业务客户端基址不同）与 §11.1 清单的 331 条未核验路由 —— 均在守卫内逐条点名而非写成空断言 |
| `Mud.Wechat.OpenTelemetry` | 可观测性一键装配（Tracing + Metrics + OTLP），委托叶层 `WechatActivitySource` 契约面 |

**目标框架**：企业微信 / 公众号 / 小程序 / 微信小店 / 广告 / Core 线为 `netstandard2.0` / `net6.0` / `net8.0` / `net10.0`；**微信支付与开放平台线为 `net6.0` / `net8.0` / `net10.0`**（刻意不含 `netstandard2.0`——`AesGcm` 在 ns2.0 不存在，由守卫 PAY-B9 锁定）。广告线只做 HTTPS + JSON、无原生密码学依赖，**不得援引该例外**（ADS-S2 锁定四档继承）。全仓 `LangVersion 13.0`。

## 安装

按产品线安装主包（其余随依赖传递）；多实例部署再引入 Redis：

```bash
# 企业微信
dotnet add package Mud.Wechat.Work
dotnet add package Mud.Wechat.Work.Callback
# 微信公众号
dotnet add package Mud.Wechat.OfficialAccount
dotnet add package Mud.Wechat.OfficialAccount.Callback
# 微信小程序（凭据复用公众号底座）
dotnet add package Mud.Wechat.MiniProgram
# 微信支付 APIv3
dotnet add package Mud.Wechat.Pay
dotnet add package Mud.Wechat.Pay.Callback
# 微信小店 / 视频号（channels 生态，P1 起逐域可用）
dotnet add package Mud.Wechat.Channels
dotnet add package Mud.Wechat.Channels.Callback
# 微信开放平台（第三方平台）
dotnet add package Mud.Wechat.OpenPlatform
# 腾讯广告 Marketing API v3.0（在建：OAuth + 8 域 33 端点）
dotnet add package Mud.Wechat.Ads
# 可选：跨线通用
dotnet add package Mud.Wechat.Redis           # Redis 分布式存储
dotnet add package Mud.Wechat.OpenTelemetry   # 可观测性装配
```

## 快速开始

### 企业微信：多应用 + 业务接口

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

第三方应用 / 服务商代开发的必填组合为 `CorpId`（服务商企业）+ `ProviderSecret` + `SuiteId` + `SuiteSecret`；代开发每家授权企业的 `permanent_code` 经换码落库，**不进配置文件**。密钥建议走环境变量 / 用户机密注入，勿提交仓库。

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b
    .AddContactApi()           // 通讯录
    .AddExternalContactApi()); // 客户联系

// 注入客户端直接调用——令牌获取 / 缓存 / 提前刷新 / errcode 失效恢复全自动。
// 父接口是抽象声明（不注册），须注入应用类型子接口：
//   IWechatWorkInternal*Service（自建）/ IWechatWorkThirdParty*Service（第三方）/ IWechatWorkProvider*Service（代开发）
app.MapGet("/api/users/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));
```

第三方 / 代开发消费授权企业时，企业级令牌一企一份（`scopeKey = authCorpId`），用一次性作用域切换：

```csharp
using (switcher.UseCorpScope(appKey: "suite-a", authCorpId, authorization.PermanentCode))
{
    return await providerUsers.GetUserAsync(userid, ct);   // 作用域内令牌解析到该授权企业
}
```

调用失败统一抛 `WechatWorkException`（`ErrorCode` = 官方 errcode；`RequestUri` 构造期已剥离 query，不泄露令牌）。

### 企业微信：模块注册（35 个业务域）

主包按模块链式注册：`AddWechatWorkServices(b => b.AddContactApi().AddExternalContactApi())`，也可 `b.AddAllApis()`、`b.AddModules(...)` 按 `WechatModule` 枚举装载后 `b.Build()`。逐域能力清单见下文「能力全景（按产品线）」的企业微信小节；端点计数与路由的权威来源是 `Tests/Mud.Wechat.Work.Tests/ContractGuards/` 下的 61 个守卫文件。

### 企业微信：会话内容存档（原生 C SDK，非 HTTP）

存档机器人**不是**一个业务模块：正文要靠官方 C SDK 的进程内调用取得，故注册入口独立（`AddWechatFinanceSdk()`）、不进 `WechatModule`、不参与令牌链路（守卫 FIN-B6）。原生库**不随包分发**，按平台部署 `WeWorkFinanceSdk.dll` / `libWeWorkFinanceSdk_C.so`（Linux 文件名带 `_C` 段）。

```csharp
builder.Services.AddWechatFinanceSdk(builder.Configuration);   // 配置节 WechatFinance；须已注册 ISecretProvider

// 配置只登记「密钥名」，真实 secret 与 RSA 私钥运行期取用（启动期拒绝把 PEM 贴进配置）
// "WechatFinance": { "Robots": { "archive-01": {
//     "CorpId": "ww...", "SecretSecretName": "finance:robot:archive-01:secret",
//     "PrivateKeySecretNames": { "1": "finance:pk:archive-01:1" } } } }

var client = await provider.GetRequiredService<IWechatWorkFinanceClientFactory>()
    .GetClientAsync("archive-01", ct);                 // 同键并发只装配一次（NewSdk + Init）

var envelope = await client.GetChatDataAsync(lastSeq, limit: 200, ct);       // 密文信封，游标「至少一次」
var message = await client.DecryptChatRecordAsync(envelope.ChatData![0], ct); // 按 publickey_ver 选私钥，明文即敏感
var media = await client.GetMediaDataAsync(message.Image!.SdkFileId!, ct);    // 或 GetMediaChunkAsync 逐片落盘
```

原生调用是**阻塞**的：`...Async` 门面不代表真异步，`CancellationToken` 也不能中断已进入的原生调用（只在分片边界生效）；实例按机器人共享、不得自行 `Dispose`。完整陷阱清单见 `Src/Work/Mud.Wechat.Work/README.md`。

### 微信公众号 / 小程序

```csharp
// 公众号：令牌底座 + 业务模块 + 回调
builder.Services.AddMpApp(builder.Configuration, "MpApps")               // AppId/AppSecret + UseStableToken
        .AddMpServices(b => b.AddAllApis());                             // 27 个业务域
builder.Services.AddWechatRedis(builder.Configuration);                  // 可选：Redis 跨实例共享令牌与票据

app.UseMpCallback();                                                     // 路由 /mp/{AppKey}，GET 验证 + POST 接收

// 小程序：与公众号同属微信公众平台、同一令牌域 ⇒ 复用 AddMpApp 底座，不新增令牌类型
builder.Services.AddMpApp(builder.Configuration, "MpApps")
        .AddMiniProgramServices(b => b.AddAllApis());                    // 19 个业务域 84 端点
```

小程序码 3 个端点的响应是**图片二进制流**（失败时才是 JSON），走独立通道 `IWxaCodeService`（按 Content-Type 分支判错，返回 `WxaCodeResult : IDisposable`），不进 JSON 管线。`session_key` 与手机号 `code` 属敏感项，SDK 不入日志（守卫 MP-X7）。

### 微信支付 APIv3

```csharp
// ① 凭据底座（配置节 WechatPayMerchants；私钥与 APIv3 密钥只放**密钥名**，经 ISecretProvider 运行期取用）
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
        .AddWechatPayApi(b => b.AddAllApis());           // 10 个域 57 端点：Transactions/Refund/Bill/Certificates/…

// ② 支付通知接收（只需凭据底座，不必拉起业务接口客户端）
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
        .AddWechatPayCallback(builder.Configuration)
        .AddHandler<TransactionSuccessHandler>("TRANSACTION.SUCCESS");

app.UseWechatPayCallback();                              // 路由 /pay/{MerchantKey}
```

通知走**三道 fail-closed 闸**（平台证书 RSA-SHA256 验签 / 时间戳 ±300s / 一次性指纹 `{mchid}:SHA1(ciphertext)`）+ `AEAD_AES_256_GCM` 解密 `resource`，成功应答 `{"code":"SUCCESS"}`；指纹在分发前消费 ⇒ **处理器须幂等**（按 `out_trade_no` 落库）。

### 微信开放平台（第三方平台）

```csharp
builder.Services.AddOpenPlatform(cfg =>
{
    cfg.ComponentAppId = "wx-your-component-appid";
    cfg.ComponentAppSecret = "<第三方平台密钥>";        // 建议经密钥提供器，勿硬编码
    cfg.Token = "<消息校验 Token>";
    cfg.EncodingAesKey = "<43 位 EncodingAESKey>";
});

// 票据与授权变更事件由宿主把原始报文转发给接收器（一处入口分流两种事件）
var outcome = receiver.Receive(msgSignature, timestamp, nonce, rawBody);
// 令牌不走声明式 [Token]：显式经 IComponentTokenProvider / IAuthorizerTokenProvider 取用
```

### 微信小店 / 视频号（channels 生态）

`appsettings.json`（配置节 `ChannelsApps`，AppID 为**独立小店 AppID**——wx 开头但与公众号 / 小程序 AppID 不互通）：

```json
{
  "ChannelsApps": [
    {
      "AppKey": "default",
      "AppId": "wx-your-store-appid",
      "AppSecret": "your-app-secret",
      "UseStableToken": true
    }
  ]
}
```

```csharp
// ① 令牌与多小店底座（token / stable_token 双通道，UseStableToken 切换，默认稳定版）；
//    多小店请用 AddChannelsApp(List<ChannelsAppConfig>) 一次性注册（重复调用注册期 fail-fast）
builder.Services.AddChannelsApp(builder.Configuration);

// ② 业务模块（P1 起逐域落地：AddWechatChannelsApi(b => b.AddProductApi().AddOrderApi()...)）
builder.Services.AddWechatChannelsApi(b => b.AddAllApis());

// ③ 回调（P3 收口）：AddWechatChannelsCallback(...).AddHandler<T>(...); app.UseWechatChannelsWebhook();
```

小店令牌类型恒为 `Wechat.Channels.AccessToken`（守卫 CH-T1~T3 锁定，与公众号 / 企微令牌键天然隔离）；`/wxa/` 前缀 6 个「小程序会员服务」端点令牌归属未确认，暂缓落位（守卫 CH-V1）。

### 腾讯广告 Marketing API v3.0（在建）

```csharp
// ① 授权与传输底座（配置节 WechatAds：ClientId / ClientSecret；BaseUrl 默认 api.e.qq.com）
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");

// ② 业务接口（3 个域 15 支声明式端点，按需装配；整体装配用 b.AddAllApis()）
builder.Services.AddWechatAdsApi(b => b.AddAdvertiserApi().AddAdgroupsApi().AddReportsApi());

// ③ OAuth 换码 / 刷新：两支手写传输、独立命名客户端 ads-oauth（不挂凭据 Handler）
var state = await adsAuthorization.ExchangeAuthorizationCodeAsync(appKey, authCode, cancellationToken);

// ④ 复合查询参数必须以「单个 JSON 字面量」上送 ⇒ 端点签名收 string，由 AdsQueryJson 编码
var daily = await reportService.GetDailyAsync(
    accountId: 1234567890,
    level: "REPORT_LEVEL_ADGROUP",
    dateRange: AdsQueryJson.DateRange(new AdsDateRange { StartDate = "2026-10-01", EndDate = "2026-10-07" }),
    fields: AdsQueryJson.Fields("date", "adgroup_id", "cost"),
    groupBy: AdsQueryJson.GroupBy("adgroup_id", "date"),
    cancellationToken: cancellationToken);
```

`access_token` + `timestamp` + `nonce` 由传输层 Handler **每请求现取**（官方「全局参数」表），故本线**不使用**声明式 `[Token]`（守卫 ADS-B1 以源码文本钉死）。`account_id` 是**授权结果**而非配置项，业务请求里由调用方显式传入 —— 配置面刻意不提供 `AccountId`，以免「配置说 A 账号、令牌属于 B 账号」这种非法状态。

### 企业微信：回调接收

凭据以 `Apps` 字典为**唯一来源**，路由 `/{GlobalRoutePrefix}/{AppKey}`；类型化处理器按 AppKey 隔离注册。接收链两道闸 **fail-closed**——① 时间戳 ±300s（缺失或非数字即拒），② 一次性 SHA1 指纹（在「解密 + receiveid 校验」成功后、分发前消费 ⇒ 重推同报文被 403，**处理器须幂等**）；加解密按官方 **32 字节块 PKCS7** 手工补位与剥离（用 .NET 内置 16 字节块填充会误拒官方报文）。

```csharp
builder.Services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";
    options.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-your-corp-id",             // 自建填 CorpId；套件填 SuiteId
        AppType = WechatAppType.Internal,
        Channel = WechatCallbackChannel.App        // 应用数据通道（1=App / 2=Suite / 3=Bot）
    };
    options.Apps["suite-a"] = new WechatAppCallbackOptions { /* 套件指令通道：suite_ticket / 授权事件 / 收银台订单 */ };
    options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions { /* 通讯录同步助手 */ };
})
.AddHandler<MyUserChangeHandler>()                       // 全局注册
.AddHandler<MySuiteScopedHandler>("suite-a")             // 仅 suite-a 路由生效
.AddInterceptor<MyAuditInterceptor>();

app.UseWechatWebhook();                                  // GET echo 验证 + POST 事件接收
```

类型化处理器——继承抽象基类，只覆写两个成员；结构族事件的类别由信封 `ChangeType` 判别：

```csharp
public sealed class MyUserChangeHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;   // 写常量不写字面量（MUDCB004）

    public override Task HandleAsync(
        WechatCallbackEvent evt, ContactUserChangedPayload payload, CancellationToken ct)
    {
        var deptIds = payload.DepartmentIds;      // 官方 "1,2,3" 已转 List<long>
        var name = payload.Name ?? "(未授权)";     // 权限分层：未授权字段即 null
        // 接收成功 ≠ 处理成功——指纹在分发前已消费，重推同报文被 403，处理器须幂等
        return SaveToDbAsync(payload, ct);
    }
}
```

载荷体系按官方**报文结构族**声明（`[WechatCallbackContract]` 事件键 + 族前置 + 开放面；`[PayloadContract]` 字段映射），登记方法体由 `Mud.Wechat.Callback.Generator` 编译期发射，键与载荷的一致性由 `Mud.Wechat.Callback.Analyzers` 编译期校验，全程零反射。当前已登记事件键 **122 个** / 结构族载荷 **47 个**，未登记键由 `GenericCallbackPayload` 兜底。

智能机器人回调为 **JSON 报文**（`{"encrypt":"..."}`，官方 101033），独立接收面：`AddWechatBotCallback().AddHandler<T>(botKey)`，返回式处理器（`null` = 加密空包），仅 `net8.0+` 可用。

### 分布式存储（多实例部署）

`AddWechatRedis` **必须先于** `AddWechatApp` / `AddMpApp` / `AddChannelsApp` / `AddWechatCallback` / `AddMpCallback`（`TryAdd` 语义，颠倒即默认进程内实现静默生效且注册期 fail-fast）：

```csharp
builder.Services.AddWechatRedis(builder.Configuration)      // ① Redis 连接 + 四个存储端口（默认注册健康检查）
        .AddWechatApp(builder.Configuration)                // ② 企微多应用基座（Redis 令牌存储已就位）
        .AddChannelsApp(builder.Configuration)             // ③ 小店多应用基座（同端口，键按令牌类型隔离）
        .AddWechatCallback(/* 同上文 */)                   // ④ 回调（抗重放窗口跨实例生效）
```

### 可观测性

```csharp
builder.Services.AddWechatOpenTelemetry(o =>
{
    o.OtlpEndpoint = new Uri("http://localhost:4317");
    o.SamplingRatio = 1.0;
});     // Tracing + Metrics 默认开、日志默认关；单一入口——不得同时调用 AddMudHttpOpenTelemetry()
```

契约面在叶层：ActivitySource 名恒 `Mud.Wechat`，标签 `wechat.product` / `wechat.app_key` / `wechat.correlation_id`，`product` 取 `work` / `officialaccount` / `miniprogram` / `openplatform` / `pay`。

## 能力全景（按产品线）

下列数字全部取自契约守卫与模块枚举（权威口径见「质量门禁」的守卫索引），不是宣传口径。

| 产品线 | 业务域 | 端点 / 契约面 | 回调接收面 | 守卫族 |
| --- | --- | --- | --- | --- |
| 企业微信 | **35** | **446** 个契约接口（147 公共父接口 + 299 可注入子接口），逐域端点与路由由域守卫锁定；另有会话存档原生封装 4 方法门面（非 HTTP） | **122** 已登记事件键 / **47** 结构族载荷（常量 138、官方 130）+ 智能机器人 JSON 通道 | G1~G10、TO1~TO3、N1~N3、CB1~CB24、WEB1~WEB4、FIN-B1~B6 + 逐域 |
| 微信公众号 | **27**（+ 认证基座） | 主接口去重 **188** ⇒ 全量 **194**（+ 令牌 / 票据 3 + 下载通道 3），官方面 **196** | **7 类键集 48 键**（消息 7 + 事件 13 + 卡券 13 + 授权 3 + 订阅 3 + 认证 6 + 发送结果 3）+ 7 种被动回复类型；明文 / 兼容 / 安全三模式 | 逐域前缀 + QT + RC + CD + CB-L1 系列 + CB-MP 系列 |
| 微信小程序 | **19** | **84**（Auth 8 + QrCodeLink 9 + Security 3 + DataAnalysis 11 + SubscribeMessage 4 + DynamicMessage 3 + Kf 9 + HardwareDevice 9 + Operation 10 + Plugin 2 + Charge 2 + NearbyPoi 4 + Search 1 + Soter 1 + ServiceMarket 2 + RedPacketCover 1 + Student 1 + FaceVerify 2 + LaborUse 2） | **无 Callback 工程**——消息接收属公众号 XML 通道，由脚手架守卫锁定 | MP-X1~MP-X9 |
| 微信支付 APIv3 | **10** | **57**（+ 账单 / 发票文件下载通道，非 JSON 生成管线） | 通知接收：平台证书验签 + `AEAD_AES_256_GCM` 解密 + 三道 fail-closed 闸 | PAY-B1~B11、PAY-CB1 |
| 微信开放平台 | — | **4** 个 component 端点 + 双层令牌链 | `component_verify_ticket` + 授权变更事件接收 | 契约测试（`OpenPlatformContractTests` 等） |
| 微信小店 / 视频号 | **27**（规划，P1 起逐域落地） | **318 端点**（规划口径：小店清单 287 + 视频号清单 49 − 精确重叠 18；`token`/`stable_token` 进令牌基座、其余 8 个 `/cgi-bin/` 基础端点落 Basic 域；`/wxa/` 6 个暂缓） | 自建 `Mud.Wechat.Channels.Callback` 包（P3 收口：订单 / 售后 / 物流 / 纠纷等事件键与载荷族登记） | CH-X1~X4、CH-T1~T3、CH-V1、CH-R1/R2 + 逐域（P1 起） |
| 腾讯广告（在建） | **3** | **15** 个声明式端点（客户账号 3 + 营销单元 8（含 4 支批量）+ 报表 4）+ OAuth 两支手写传输（换码 / 刷新） | 无（v3.0 无推送回调，报表走 `async_reports` 拉取） | ADS-S1/S2（依赖边界·TFM）、ADS-B1/B4/B5/B6（凭据注入形态·SSRF 并集口径·Query 凭据脱敏·AOT 净零）、ADS-B2 逐路由 / 逐参数名 / 逐层级 / 逐字段名镜像官方原文、ADS-B3 刷新一次性语义；未落地域在守卫内**逐条点名**而非写成空断言 |

### 企业微信：35 个业务域 / 446 个契约接口

35 个 `WechatModule` 枚举成员各对应一个 `Add{域}Api()`（注册用法见上文「企业微信：模块注册」），下表逐域列出能力面：

| 注册方法 | 域 | 能力概述 |
| --- | --- | --- |
| `AddExternalContactApi()` | 客户联系 | 服务人员 / 客户 / 客户标签 / 在职·离职继承 / 客户群 / 群发 / 朋友圈 / 商品相册 / 联系我 / 拦截规则 / 统计 / 附件 / 获客助手等族 |
| `AddMessageApi()` | 消息推送 | 发送应用消息（每 msgtype 一端点）/ 群聊会话 / 家校学校通知 / 智能表格群聊 |
| `AddContactApi()` | 通讯录 | 成员 / 部门 / 标签 / 查看权限 / 异步导入 / 异步导出六域 |
| `AddApprovalApi()` | 审批 | 审批申请数据 / 审批模板 / 假期管理 / 审批流程引擎 |
| `AddMediaApi()` | 素材管理 | 临时素材上传·获取 / 上传图片 / 高清语音 / 异步上传 / 服务商上传 |
| `AddIdentityApi()` | 身份验证 | 网页授权登录 / Web 登录身份获取 / 二次验证 |
| `AddJsSdkApi()` | JS-SDK | 企业 / 应用 `jsapi_ticket` 获取 |
| `AddAgentApi()` | 应用管理 | 获取应用 / 工作台自定义展示 / 自定义菜单 / 自建应用迁移代开发 |
| `AddAuthenticationApi()` | 授权流 | `get_pre_auth_code` / `set_session_info` / `get_permanent_code` / `get_auth_info` / `get_customized_auth_url` + 授权编排 |
| `AddBasicApi()` | 基础接口 | 企业微信接口 IP 段 / 回调 IP 段 |
| `AddCheckinApi()` | 打卡 | 打卡规则 / 记录 / 报表 / 排班 / 设备打卡数据 |
| `AddMeetingApi()` | 会议 | 预约会议管理 / 会议统计 |
| `AddScheduleApi()` | 日程 | 日历管理 / 日程管理 |
| `AddWedocApi()` | 文档 | 管理文档 / 文档内容 / 表格内容 / 智能表格内容（子表 / 视图 / 字段 / 记录 / 编组） |
| `AddWedriveApi()` | 微盘 | 空间 / 空间权限 / 文件 / 文件权限 / 版本容量 / 高级功能账号 |
| `AddAccountIdApi()` | 账号 ID | ID 与 `tmp_external_userid` / `corpid` 转换、ID 迁移、智能机器人 userid 转换、群 ID 升级等七接口族 |
| `AddKfApi()` | 微信客服 | 客服账号管理 + 接待人员管理 |
| `AddMailApi()` | 邮件 | 应用邮箱发送·接收 / 邮箱账号管理 / 邮件群组 / 公共邮箱 / 高级功能账号 / 成员邮箱操作 |
| `AddPayApi()` | 企业支付 | 对外收款 / 商户号管理 / 资金流水 / 退款 / 交易账单 |
| `AddSecurityApi()` | 安全管理 | 文件防泄漏 / 设备管理 / 截屏录屏 / 域名 IP / 高级功能账号 / 操作日志 |
| `AddCorpGroupApi()` | 上下游 | 基础接口 + 关联客户信息 + 上下游通讯录管理 |
| `AddSchoolApi()` | 家校沟通 | 家校基础 / 管理配置 / 学生与家长 / 访问授权 / 健康上报 / 上课直播 / 学生付款等子域 |
| `AddLivingApi()` | 直播 | 预约直播 / 直播回放 / 观看凭证 / 直播详情 / 观看明细 |
| `AddDataZoneApi()` | 数据与智能专区 | 基础接口域 + 应用调用专区程序域 |
| `AddMsgAuditApi()` | 会话内容存档 | 开启成员 / 机器人信息 / 会话同意情况 / 内部群信息 |
| `AddInvoiceApi()` | 电子发票 | 查询 / 更新状态 / 批量更新 / 批量查询 |
| `AddGovApi()` | 政民沟通 | 网格结构 / 事件类别 / 巡查上报 / 居民上报 |
| `AddEmergencyApi()` | 紧急通知 | 语音电话 + 接听状态 |
| `AddPromotionQrCodeApi()` | 推广二维码 | 企业注册（注册码 / 注册状态）+ 通讯录迁移（官方仅第三方开放） |
| `AddPayToolApi()` | 收银台 | 收款工具 / 发票管理 / 应用版本付费（官方仅第三方开放，`HMAC-SHA256` 签名） |
| `AddAibotApi()` | 智能机器人 | 主动回复消息（`response_code` 一次性凭据鉴权）；回调接收与被动回复走 Callback 包 JSON 通道 |
| `AddLicenseApi()` | 接口调用许可 | 订单管理 13 / 账号管理 9 / 应用管理 1 / 自动激活设置 2，共 25 端点（官方仅第三方与代开发，走 `provider_access_token`） |
| `AddWebhookApi()` | 群机器人 Webhook | 发送消息 8 种 msgtype + 上传媒体文件，共 9 端点；凭据为 URL 上的 `key`（注册期登记为强制掩码参数名） |
| `AddHrApi()` | 人事助手 | 花名册字段配置 / 读取 / 更新 3 端点（官方仅自建） |
| `AddDialApi()` | 公费电话 | 拨打记录查询 1 端点（官方仅自建） |

各域面向的应用类型存在差异（官方仅自建开放 / 三类应用公共面 / 差异端点在子接口），详见接口 XML 注释与契约守卫。

> ⚠️ **三条「支付 / 资金」面勿混淆**：上表的 `AddPayApi()`（企业支付）与 `AddPayToolApi()`（收银台）属**企业微信**支付能力，走企微 `access_token`；**微信支付 APIv3** 产品线（`Mud.Wechat.Pay*`）凭据为商户 RSA 私钥签名、**无 `access_token`**、四包零 `[Token]` 声明（守卫 PAY-B1 fail-closed）；**微信小店资金结算**（`Channels.AddFundsApi`）走小店 `access_token`，语义是小店余额 / 结算账户 / 提现 / 流水，不是交易收单（设计方案 v1 §4.6）

> ⚠️ **`AddMsgAuditApi()` 只管存档的配置面**：开启成员 / 机器人信息 / 会话同意情况 / 内部群信息都是 HTTP 端点，但**取会话正文、解密、下载媒体不是**——那是官方 C SDK（`WeWorkFinanceSdk`）的进程内调用，落主包 `ExtendedSDK/Finance/`、注册入口 `AddWechatFinanceSdk()`（**不进 `WechatModule`、不挂 `[HttpClientApi]`、不登记 SSRF 白名单**，守卫 FIN-B6）。故企业微信的「35 个业务域」口径不含它，四方法门面由 `WechatFinanceContractGuards`（FIN-B1~B6）另锁一层。

### 微信公众号：27 个业务域 / 188 个去重端点

`MpModule` 28 个成员（27 个可注册业务域 + `Authentication` 令牌与票据基座），`AddMpServices(b => b.AddAllApis())` 一键装载。计数口径由 `MpRouteCountGuard` 定义：**同一路由的多方法形态只计 1 条端点**（如 OCR 的 `*ByUpload` / `*ByUrl` 双形态）。

| 注册方法 | 域 | 端点 | 关键约束（守卫锁定） |
| --- | --- | --- | --- |
| `AddBasicApi()` | 基础接口 | 3 | API 服务器 IP + 推送服务器 IP + 网络通信检测；3 端点均支持第三方平台令牌 |
| `AddTagApi()` | 标签管理 | 8 | 批量打标 / 取消 / 获取标签列表等；**仅认证** |
| `AddUserApi()` | 用户管理 | 8 | 用户信息 7 + openid 转换 1；黑名单三端点官方路径在 `tags/members` 前缀下 |
| `AddMenuApi()` | 自定义菜单 | 7 | 查询菜单信息族 |
| `AddCustomerMessageApi()` | 客服消息 | 3 | 发送客服消息 / 输入状态 / 聊天记录 |
| `AddKfAccountApi()` | 客服管理 | 7 | 全部 / 在线客服列表、增删改账号、头像、邀请绑定 |
| `AddKfSessionApi()` | 会话控制 | 5 | 创建 / 关闭会话、会话状态 / 列表、未接入列表 |
| `AddTemplateApi()` | 模板消息 | 8 | 发送 1 + 行业 2 + 模板管理 3 + 拦截查询 1 + 一次性订阅 1（一次性订阅并入本域） |
| `AddSubscriptionNoticeApi()` | 订阅通知 | 7 | `bizsend` 1 + `/wxaapi/newtmpl/*` 6，**服务号专属** |
| `AddOpenApiApi()` | 额度管理 | 5 | 双接口同注册组：`IMpOpenApiService` 4 端点带令牌 + `IMpOpenApiTokenFreeService` 1 端点免令牌（`clear_quota/v2` 是额度耗尽应急逃生端点） |
| `AddSnsApi()` | 网页授权 | 4 | **服务号专属**且全部免应用级 `access_token` |
| `AddMassApi()` | 群发消息 | 7 | `sendall` / `send` / `preview` / `delete` / `get` / `speed`（双端点）；`uploadimg` 归素材域 |
| `AddQrcodeApi()` | 带参二维码 | 1 | `qrcode/create`，**服务号专属** |
| `AddAutoReplyApi()` | 自动回复 | 1 | 只读查询；认证 / 未认证服务号与测试号均可调用 |
| `AddDraftApi()` | 草稿管理 | 6 | `add`/`update`/`get`/`delete`/`count`/`batchget`；`draft/switch` 官方已废弃 ⇒ 不实现 |
| `AddFreePublishApi()` | 发布能力 | 5 | 仅认证 |
| `AddProductCardApi()` | 商品卡片 | 1 | `/channels/ec/…` 视频号小店前缀，**非** `/cgi-bin/` |
| `AddCommentApi()` | 留言管理 | 8 | 仅认证 + 留言权限 |
| `AddDataCubeApi()` | 数据统计 | 21 | 用户 2 + 图文 10 + 消息 7 + 接口 2，**全部 POST `/datacube/*`**、请求体同构；跨度上限措辞逐端点核验（1 / 7 / 15 / 30 天） |
| `AddMediaApi()` | 素材管理 | 6 | 临时上传 1 + 永久上传 / 计数 / 列表 / 删除 4 + `uploadimg` 1；另有**下载通道 3**（非 JSON 管线，不计入 188） |
| `AddSmartApiApi()` | 智能接口 | 12 | AI 3 + OCR 7 + 图像处理 2；**9 端点双调用形态**（form `img` / Query `img_url` 互斥 ⇒ 每端点双方法，共 21 个方法）；OCR 100 次/天、图片 <2M；九端点支持第三方代调用（权限集 117） |
| `AddQrcodeJumpApi()` | 扫码打开小程序 | 4 | `/cgi-bin/wxopen/qrcodejump*`，**服务号专属** |
| `AddShortLinkApi()` | 长转短链 | 2 | `/cgi-bin/shorten/*` |
| `AddStoreApi()` | 门店小程序 | 12 | 类目 / 主体申请与审核 / 修改主体 / 省市区 / 地图点位搜索等 |
| `AddOneCodeApi()` | 一物一码 | 6 | `/intp/marketcode/*`（非 `/cgi-bin/` 前缀） |
| `AddInvoiceApi()` | 微信发票 | 17 | 商户开票 5 + 开票平台 5 + 发票报销 4 + 极速开发票 3；17 页全部用 `access_token`、**零 `api_ticket`** |
| `AddCardApi()` | 卡券 | 14 | 主体生命周期 / 投放 11 + 券码核销 3，**双接口同注册组**（`IMpCardService` / `IMpCardCodeService`，照 openApi 先例）；全 POST；建卡 `card` 包装 + `card_type` 判别 11 分支与修改方向「平级 + 无 `advanced_info`」**不同构**；`/card/` 前缀与发票域 17 端点**共用**（CD8 双向锁定）；前端取卡 `api_ticket` 由 `IMpTicketService` 承载；未建模 10 族共 39 端点以零路由断言留档 |
| —（基座） | 认证与票据 | 3 | `token` + `stable_token` + `ticket/getticket`；`jsapi` / `wx_card` 两类票据经 `IMpTicketManager` 取用后由宿主自行使用，不参与请求注入与 errcode 恢复链路 |

### 微信小程序：19 个业务域 / 84 个端点

与公众号同属微信公众平台、**同一令牌域**（MP-X2 禁止新增令牌类型，否则 errcode 自愈静默失效）⇒ 复用 `AddMpApp` 底座，跨线路由重复由 MP-X1 全局校验。

| 注册方法 | 域 | 端点 | 能力与约束 |
| --- | --- | --- | --- |
| `AddAuthApi()` | 登录与身份 | 8 | `IWxaAuthService` 7 端点（`checksession` / `resetusersessionkey` / `getuserphonenumber` / `getpaidunionid` / 插件用户 openpid / 检查加密信息 / 用户 encryptKey）+ `IWxaCode2SessionService`（`sns/jscode2session`，免令牌）；`session_key` 与手机号 `code` 不入日志（MP-X7） |
| `AddQrCodeLinkApi()` | 二维码与链接 | 9 | 小程序码 **3 端点走手工通道 `IWxaCodeService`**（响应为图片二进制，失败才是 JSON ⇒ 按 Content-Type 分支判错，返回 `WxaCodeResult : IDisposable`）+ 短链 / URL Scheme / NFC Scheme / URL Link 的生成与查询 6 端点 |
| `AddSecurityApi()` | 内容安全 | 3 | `msg_sec_check`（同步文本）+ `media_check_async`（异步媒体，回调结果另取）+ `getuserriskrank`（用户安全等级）；`/wxa/img_sec_check` 官方已下架 ⇒ 不实现 |
| `AddDataAnalysisApi()` | 数据分析 | 11 | `/datacube/getweanalysisappid*`：日 / 周 / 月访问趋势与留存 + 用户画像 + 访问分布 + 访问页面 + 数据概况 + `wxa/business/performance/boot`；应答信封不成一形 ⇒ 逐端点各自 DTO |
| `AddSubscribeMessageApi()` | 订阅消息 | 4 | 发送订阅消息 + 用户通知开关 / 扩展（`set_user_notify` / `set_user_notifyext` / `get_user_notify`）；模板与类目等「设置面」归公众号线 |
| `AddDynamicMessageApi()` | 动态消息 | 3 | `cgi-bin/message/wxopen/activityid/create` + 动态消息发送 + 聊天工具动态卡片消息 |
| `AddKfApi()` | 客服 | 9 | 客服角色 2 + 客服子商户 4 + 微信客服绑定 3（只补公众号线**未覆盖**的端点，单注册组） |
| `AddHardwareDeviceApi()` | 硬件设备 | 9 | 设备消息发送 + `wxa/getsnticket` + 设备组 4（建组 / 查 / 加删设备）+ License 3 |
| `AddOperationApi()` | 运维中心 | 10 | `IWxaOperationService` 9 端点（域名配置 / 性能·来源·客户端版本 / 实时与错误日志 / 反馈列表 / 灰度发布）+ `IWxaFeedbackMediaService`（反馈图片手工通道，图片二进制） |
| `AddPluginApi()` | 插件管理 | 2 | `/wxa/devplugin`、`/wxa/plugin`（`action` 驱动，单路由多操作） |
| `AddChargeApi()` | 付费管理 | 2 | 资源包用量查询 + 最近平均用量查询 |
| `AddNearbyPoiApi()` | 附近小程序 | 4 | 添加 / 删除地点 + 查看地点列表 + 设置展示状态；添加后进入**审核**，`poi_id` 为删除与展示状态的键 |
| `AddSearchApi()` | 微信搜一搜 | 1 | `wxaapi_submitpages` 搜一搜数据推送 |
| `AddSoterApi()` | 生物认证 | 1 | SOTER 生物认证秘钥签名验证 |
| `AddServiceMarketApi()` | 服务市场 | 2 | 调用服务市场接口 + 异步获取处理数据 |
| `AddRedPacketCoverApi()` | 红包封面 | 1 | 获取微信红包封面；`ctoken` 为发放凭据，属敏感信息（禁止落日志） |
| `AddStudentApi()` | 学生身份 | 1 | 快速获取学生身份 |
| `AddFaceVerifyApi()` | 人脸核身 | 2 | 获取人脸核身会话唯一标识 + 查询真实验证结果；`cert_info` 含证件姓名 / 号码（禁止落日志），官方标注不支持第三方平台代调用 |
| `AddLaborUseApi()` | 用工关系 | 2 | 推送用工消息 + 解绑用工关系 |

小程序线另有 `WxaErrorCodes`（13 个错误码常量）与 `MiniProgramScaffoldContractGuards` 锁定的「无 Callback 工程」形态。

**智能接口（OCR / 图像处理 / AI 语音，12 端点）不在小程序线建模**（MP-X9 留档裁决）：两侧 URI 逐字相同、且小程序侧消费同一 `access_token` ⇒ 克隆即同时踩 MP-X1（跨线零重复）与 MP-X6（不回潮公众号已有路由），并制造两份 DTO + 两个 JsonContext + 两套守卫的纯重复维护。小程序侧请直接注入公众号线的 `IMpSmartApiService`（`AddMpApp` 已是本线硬前置）。MP-X9 含反静默绿断言：公众号线这 12 条路由缺席即报红。

### 微信支付 APIv3：10 个业务域 / 57 个端点

凭据模型与其余四线根本不同：**商户 RSA 私钥签名**（`WECHATPAY2-SHA256-RSA2048`），全线**零 `[Token]` 声明**（PAY-B1 fail-closed），私钥与 APIv3 密钥只以名称进配置、运行期经 `ISecretProvider` 取用。

| 注册模块 | 域 | 端点 | 路由族与关键约束 |
| --- | --- | --- | --- |
| `Transactions` | 基础交易 | 7 | `/v3/pay/transactions/*`：JSAPI·小程序 / Native / APP / H5 四族下单 + 按 `transactionId` / `outTradeNo` 查单 + 关单 |
| `Refund` | 退款 | 3 | `/v3/refund/domestic/refunds*`：申请退款 + 按 `outRefundNo` 查询 + 异常退款 |
| `Bill` | 账单 | 2 | `tradebill` / `fundflowbill` 申请；账单文件本身走 `IWechatPayBillDownloadService` 下载通道 |
| `Certificates` | 平台证书 | 1 | `/v3/certificates`：应答与回调验签的证书来源 + `Wechatpay-Serial` 轮换 |
| `ProfitSharing` | 分账 | 9 | 接收方添加 / 删除 + 请求分账 / 查询 + 解冻剩余资金 + **回退单独立资源族**（`/profitsharing/return-orders`，非分账单子资源）+ 待分金额 + 分账账单 |
| `PayScore` | 支付分 | 11 | 服务订单 创建 / 查询 / 取消 / 完结 / 修改 / 催收扣款 / 同步 + 授权面（预授权、按 `authorizationCode` 查询与解除、按 `openid` 查询） |
| `CombineTransactions` | 合单支付 | 6 | JSAPI / Native / APP / H5 四场景下单 + 合单关单 + 合单查询；**无合单退款**——合单订单只能按子单走退款域 |
| `Transfer` | 商家转账 | 6 | 发起转账 + 按 `outBillNo` / `transferBillNo` 查询 + 撤销 + 电子回单两查询；撤销走普通商户面路由，服务商 `/partner/` 变体属另一套文档不得混入 |
| `NewTaxControlFapiao` | 电子发票 | 5 | 开具 + 查询 + 冲红 + 获取下载信息 + 插入卡包；上传（multipart）与下载（30s 有效 URL，不签名验签）不属生成式接口 |
| `MarketingFavor` | 代金券 | 7 | 创建批次 + 券详情 + 批次 启动 / 暂停 / 重启 / 详情 + 发券；两族路径前缀不一致（创建 `/coupon-stocks`、动作 `/stocks/…`，官方原文如此） |

### 微信开放平台（第三方平台）

单入口 `AddOpenPlatform(cfg => …)`，**全部 `TryAdd`**（宿主预注册实现优先，如把票据存储换成 Redis 分布式实现）。不走声明式 `[Token]`，令牌经显式提供者取用，两侧都是「单飞门 + 结果记忆」：

| 端点 / 端口 | 说明 |
| --- | --- |
| `/cgi-bin/component/api_component_token` | `component_access_token`（`IComponentTokenProvider`，提前刷新窗口默认对齐官方建议） |
| `/cgi-bin/component/api_create_preauthcode` | 预授权码，官方有效期 1800 秒 |
| `/cgi-bin/component/api_query_auth` | 用 `auth_code` 换授权方令牌与刷新令牌（`IComponentAuthorizationService`） |
| `/cgi-bin/component/api_authorizer_token` | 刷新授权方令牌（`IAuthorizerTokenProvider`，`authorizer_access_token` 有效期 2 小时） |
| `IComponentVerifyTicketStore` / `IWechatOpenPlatformHttpClient` / `IOpenPlatformClock` | 票据存储端口、命名客户端、可注入时钟（测试确定性） |
| `ComponentVerifyTicketReceiver.Receive(...)` | `component_verify_ticket` 与授权变更事件**一处入口分流**，返回 `ComponentPushResult`（票据推送结论为 8 态枚举，区分「重复推送 / 校验失败 / 存储失败」等原因） |

### 微信小店 / 视频号：27 个业务域 / 318 端点（规划口径）

微信小店与视频号经路由比对确认为**同一产品**的两面（内容运营面 + 交易管理面 + 本地生活面），合并为一条产品线（`.docs/微信小店/微信小店×视频号产品线设计方案 v1.md`）。下表端点计数为**规划口径**：小店清单 287 + 视频号清单 49 − 精确重叠 18 = **318**，同一路由只计 1；`/wxa/` 6 个「小程序会员服务」端点令牌归属未确认、暂缓落位（守卫 CH-V1）；`token` / `stable_token` 进令牌基座不入域计数。**权威口径以 P1 起逐域守卫为准。**

| 注册方法 | 域 | 端点 | 路由族与关键约束 |
| --- | --- | --- | --- |
| `AddBasicApi()` | 基础接口 | 8 | `/cgi-bin/openapi|clear_quota|callback|get_*`（剔除 token/stable_token 2 个进基座） |
| `AddResourceApi()` | 资源管理 | 7 | `/shop/ec/basics/*`（图片 / 资质 / 视频分块上传） |
| `AddShopApi()` | 店铺管理 | 4 | `/channels/ec/basics/(info|shop/*)` |
| `AddHomePageApi()` | 主页管理 | 14 | `/channels/ec/store/window/*`、`/channels/ec/basics/homepage/*`、`/channels/ec/store/classification/*` |
| `AddProductApi()` | 商品管理 | 43 | `/channels/ec/product/*`（商品 / 库存 / 赠品 / 买赠活动 / 限时抢购） |
| `AddFavoriteApi()` | 收藏管理 | 1 | `/channels/ec/favorites/count/get` |
| `AddCategoryApi()` | 类目管理 | 11 | `/shop/ec/category/*`、`/channels/ec/category/*`（类目 8 + 类目规则 3） |
| `AddOrderApi()` | 订单管理 | 27 | `/channels/ec/order/*`、`/channels/ec/merchant/privatenumber/*` |
| `AddFundsApi()` | 资金结算 | 16 | `/channels/ec/funds/*`、`/shop/funds/*`（小店余额 / 结算账户 / 提现 / 流水，**非**支付收单） |
| `AddMarketingApi()` | 营销管理 | 7 | `/channels/ec/coupon/*`（优惠券） |
| `AddAftersaleApi()` | 售后管理 | 27 | `/channels/ec/aftersale/*`（售后单 / 纠纷单 / 保障单） |
| `AddKfApi()` | 商家客服 | 2 | `/channels/ec/commkf/*` |
| `AddQicApi()` | 质检管理 | 5 | `/channels/ec/qic/inspect/*` |
| `AddLogisticsApi()` | 物流发货 | 28 | `/channels/ec/merchant/address|freight*`、`/channels/ec/logistics/ewaybill/*`、`/channels/ec/order/delivery*` |
| `AddWarehouseApi()` | 区域仓库 | 11 | `/channels/ec/warehouse/*`、`/channels/ec/basics/addresscode/get` |
| `AddLeagueApi()` | 优选联盟 | 11 | `/channels/ec/league/*`（带货者 / 商品） |
| `AddBrandApi()` | 品牌资质 | 8 | `/shop/ec/brand/*`、`/channels/ec/brand/*` |
| `AddDeliveryApi()` | 代发与供货 | 16 | `/channels/ec/supplier/*`、`/channels/ec/order/(dropship|supplyorder)*` |
| `AddWecomApi()` | 企业微信关联 | 1 | `/channels/ec/wecom/get_wecom_id` |
| `AddMiniStoreApi()` | 连接小程序 | 10 | `/channels/ec/open/*`、`/channels/ec/b2c/*`、`/channels/ec/order/present*`（基础 + 授权送礼） |
| `AddCompassApi()` | 罗盘 | 14 | `/channels/ec/compass/*`（商家版 10 + 达人版 4） |
| `AddVipApi()` | 会员营销 | 4 | `/channels/ec/vip/user/*` |
| `AddLiveApi()` | 直播与留资 | 12 | `/channels/finderlive/*`、`/channels/leads/*`、`/channels/livedashboard/*`（视频号内容面） |
| `AddWindowApi()` | 橱窗与本地生活商品 | 13 | `/channels/ec/window/product/*`、`/channels/ec/product/locallife/*`（双前缀同族并组） |
| `AddLocallifeApi()` | 本地生活 | 8 | `/channels/ec/voucher/*`（团购券 / 核销 / 售后 / 券账单） |
| `AddSubsidyApi()` | 国补管理 | 3 | `/channels/ec/subsidy/*` |
| `AddPlatformKfApi()` | 客诉工单 | 4 | `/channels/ec/platformkf/*` |

小店线**平铺命名空间、无 `IsAbstract` 父接口**（守卫 CH-X2）；四包不引用既有六线任何工程（守卫 CH-X3）；与既有线路由交叠必须为空、仅共享基础设施路由白名单内可重复声明——`/cgi-bin/token`、`/cgi-bin/stable_token`（守卫 CH-R1）；回调不借公众号 / 企微既有通道（守卫 CH-X4）。**`ProductCard` 是公众号线既存先例**（`IMpProductCardService`，公众号令牌）——小店线不得重复声明（守卫 CH-R1）。

### 腾讯广告：3 个业务域 / 15 个声明式端点（在建）

路由与逐参数名取自 2026-10-10 逐页核验的官方原文，权威口径在 `AdsContractGuards`（ADS-B2 系列）。层级断言只写经 DOM `level-*` 核验过的条目，其余照录平面证据并标未核验（留档见 `.docs/Ads-v3.0-官方页面核验留档.md`）。

| 注册方法 | 域 | 端点 | 能力概述 |
| --- | --- | --- | --- |
| `AddAdvertiserApi()` | 客户账号 | 3 | `advertiser/get`（GET，`fields` 自选返回列）+ `advertiser/update` + `advertiser/update_daily_budget`（后两支官方即 POST） |
| `AddAdgroupsApi()` | 营销单元 | 8 | `adgroups/get`（GET，游标与页码两套分页并存）+ `add` / `update` / `delete`（官方**无批量删除**形态）+ 四支批量端点（`update_daily_budget` / `update_configured_status` / `update_bid_amount` / `update_datetime`）；仅 `add` 带 `X-Request-Id` 幂等头 |
| `AddReportsApi()` | 报表 | 4 | `daily_reports/get` + `hourly_reports/get`（官方即 GET）+ `async_reports/add`（本族唯一 POST）+ `async_reports/get`（轮询任务态） |
| —— | OAuth（无注册模块） | 2 | `oauth/token`、`oauth/refresh_token`（**路径无 `/v3.0` 前缀**，均 GET、参数进 Query，独立命名客户端 `ads-oauth` 不挂凭据 Handler） |

三条不可归并的官方差异，各由一支守卫锁定：报表 `level` 三支集合**互不相同且差异双向**（daily 17 / hourly 8 / `async_reports/add` 21，合并成公共枚举即红）；分页上限逐页不同（daily `page` 99999、hourly 100，`async_reports/get` 的 `page_size` 上限 10 而同步页 2000）；`account_id` 的代理商口径**逐页相反**（同步页「不支持代理商 id」、async 页「包括代理商和账户 id」）。异步报表是**双层判定**——外层 `code == 0` 只代表任务受理，`result.code` 才代表生成结果，且 `result` 层无 `message_cn`。

**未落地（在守卫内点名可见，非空断言）**：`dynamic_creatives` / `components` / `images` / `videos` / `async_tasks`（官方文档层级只有平面证据，须先补 DOM 核验才能建模）；`async_report_files/get`（请求地址在**另一台主机** `dl.e.qq.com`，本线目前只有一条指向 `api.e.qq.com` 的业务客户端 ⇒ 报表域端点是恰 4 支而非 5 支，由守卫计数钉住）。

## 质量门禁

```bash
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1   # Release 全量构建 + 逐源项目 AOT strict + 逐测试工程测试
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-config-keys.ps1  # 配置属性消费点审计

dotnet build Mud.Wechat.slnx -c Release
dotnet test Tests/Mud.Wechat.Work.Tests -c Release -f net8.0 --filter "FullyQualifiedName~ContractGuards"
```

**16 个测试工程**（`Tests/`，镜像源结构，单 TFM `net8.0`）覆盖七条产品线 + Core 叶层 + Redis。契约守卫分布：企业微信 `Tests/Mud.Wechat.Work.Tests/ContractGuards/` 61 个文件（通用 G1~G10、令牌归属 TO1~TO3、命名空间分区 N1~N3、回调 CB 系列、群机器人 WEB1~WEB4、会话存档原生封装 FIN-B1~B6 + 逐域端点/路由守卫）、公众号 24 个（逐域前缀 + QT 令牌注入白名单 + RC 路由计数纪律 + CD 卡券契约）、小程序 MP-X1~MP-X9、支付 PAY-B1~B11 与 PAY-CB1、小店 `Tests/Mud.Wechat.Channels.Tests/ContractGuards/`（CH-X1~X4 形态 / CH-T1~T3+CH-V1 令牌归属 / CH-R1~R2 路由，P1 起逐域守卫加挂）、叶层 AB-G1~G10 与 CB-L1 系列、Redis RD-G1~G7、广告线 ADS-S1/S2 + ADS-B1~B6 共 1 个守卫文件 24 条断言（依赖边界与 TFM 口径、凭据注入形态、逐路由 / 逐参数名 / 逐字段层级镜像官方原文、刷新的一次性语义与失败顺序、Query 凭据脱敏覆盖面、AOT 净零与上下文登记完整性；未落地项在守卫内逐条点名而非写成空断言）。

**新增产品线的门禁接入口**只有一个：AB-G6 会同时校验解决方案工程清单（每个可打包工程须已在 `slnx` 内）、`audit-config-keys.ps1` 搜索根、CI / `pack.bat` / `publish.bat` 三处的**推导口径**与 DTO 标注脚本根命名空间——漏接一项即红。**包清单是单一来源**：可打包集由 `Src/**/*.csproj` 现场推导（未声明 `<IsPackable>false` 者），三处各自推导、不持有名单也不硬编码包数；唯一被维护的名单是「非可打包工程白名单」（仅两个构建期工具），由 AB-G6 兜住「某工程被误设 `IsPackable=false`」这一推导无法察觉的失效。AB-G7 另锁一条易踩面：四个回调宿主包以**字面相对路径**内嵌分析器 DLL，源码目录归类移动会让 `dotnet pack` 少 4 个包而构建与测试全绿（2026-10 实际踩过）。

**AOT / 裁剪**：`net8.0` / `net10.0` 下逐源工程跑 `AotStrictMode`（把 10 类反射诊断升为错误）并保持净零；六条线 DataModels 共 **132 个源生成 JSON 上下文**（Work 63 / 公众号 29 / 小程序 20 / 支付 12 / 广告 5 / 小店 3），配置绑定同样源生成。Work 侧的合并解析器里 `FinanceJsonContext` 是**唯一有意排除项**：会话存档不经 HTTP（原生库在进程内写回明文 JSON），并入即是一支永不命中的死注册（守卫 FIN-B4 锁该形态）。

真实 Redis 端到端用例由环境变量门控（CI 默认不跑）：

```bash
WECHAT_REDIS_TESTS_CONNECTION=localhost:6379 dotnet test Tests/Mud.Wechat.Redis.Tests
```

## 目录结构

```
Mud.Wechat/
├── Src/
│   ├── Core/                # 跨产品线共享：Abstractions 叶层 / Redis / Callback 生成器与分析器
│   ├── Work/                # 企业微信线（4 包）
│   ├── OfficialAccount/     # 公众号线（4 包）
│   ├── MiniProgram/         # 小程序线（3 包，无 Callback）
│   ├── Pay/                 # 微信支付 APIv3 线（4 包）
│   ├── OpenPlatform/        # 开放平台线（2 包）+ OpenTelemetry 装配
│   ├── Channels/            # 微信小店/视频号线（4 包）
│   └── Ads/                 # 腾讯广告 Marketing API v3.0 线（3 包，在建：OAuth + 3 域 15 端点已落地）
├── Tests/                   # 16 个测试工程（单 TFM net8.0，含 ContractGuards/）
├── Demos/                   # 示例工程（联系人功能 + 联系人事件回调；不入主链、被 AOT 冒烟排除）
├── scripts/                 # verify-build / audit-config-keys / GenerateJsonContext / AddHttpJsonSerializable / ApplyTokenOwnerKeys
├── .docs/                   # 方案与设计文档（中文；已 gitignore，fresh clone 无此目录）
├── Directory.Build.props    # 全局 MSBuild 属性（TFM / LangVersion / Version 唯一来源）
├── pack.bat / publish.bat   # 打包与推送
└── Mud.Wechat.slnx          # 解决方案
```

各包的职责、公开面、配置节与已踩陷阱见对应目录下的 `README.md`；跨域不可违反的约束（令牌归属域、回调 fail-closed、命名空间二分等）见 `AGENTS.md`。

## 许可证

本项目主要遵循 MIT 许可证进行分发和使用，许可证位于源代码树根目录中的 `LICENSE-MIT` 文件。
