# Mud.Wechat.OfficialAccount

微信公众号 / 服务号 SDK **主包**：业务声明式客户端、模块注册器、AOT JsonContext 合并、素材下载双通道、JS-SDK 签名服务。对应企微线的 `Mud.Wechat.Work`。

## 内容

- **声明式业务客户端**（`Interfaces/`，命名空间平铺为 `Mud.Wechat.OfficialAccount`）：27 个域目录 / 29 个公开接口 / **188 条去重路由**（守卫 RC1 锁定）。公众号无「自建 / 套件 / 代开发」三类形态 ⇒ **不设 `IsAbstract` 公共父接口、不设应用类型子接口**，每个域接口直接作注册与注入面。

  | 目录（= 模块） | 端点 | 关键官方约束（详见各接口 XML 注释与 `MpModule` 注释） |
  |---|---|---|
  | `Basic/` | 3 | API 服务器 IP / 推送服务器 IP / 网络通信检测（`action` 仅 `dns`/`ping`/`telnet`） |
  | `Tag/` | 8 | 全部仅认证；标签上限 100、单用户标签上限 20、标签名 ≤ 30 字符 |
  | `User/` | 8 | 仅认证；批量 100 条 / 关注者单批 10000 / 黑名单单批 1000 / 拉黑单次 20 |
  | `Menu/` | 7 | 一级 3 个 / 每级 5 个二级硬上限；个性化菜单每日新增·删除 2000、测试 20000；删默认菜单级联删全部个性化菜单 |
  | `CustomerMessage/` | 3 | 下发额度 5 条/48 小时（用户发消息）或 3 条/1 分钟（菜单·关注·扫码）；聊天记录区间 ≤ 24 小时、每次 ≤ 10000 条 |
  | `KfAccount/` | 7 | 每账号最多 100 个客服账号；账号形状「前缀(≤10)@公众号微信号(≤30)」 |
  | `KfSession/` | 5 | 创建会话要求客服已绑定微信号**且在线**；未接入列表最多 100 条且无分页游标 |
  | `Template/` | 8 | 服务号专属；日调用上限 10 万次；行业每月可改 1 次；每账号可同时使用 25 个模板；结果经 `TEMPLATESENDJOBFINISH` 回执 |
  | `SubscriptionNotice/` | 7 | 服务号专属；一次性消耗用户订阅次数；模板管理前缀 `/wxaapi/newtmpl/`（无 `/cgi-bin/`） |
  | `OpenApi/` | 5 | `clear_quota` 与 `clear_quota/v2` 合计每月 10 次；`openapi/quota/clear` 每月 50 次；rid 查询有效期 7 天 |
  | `Sns/` | 4 | 服务号专属；**全部免令牌**（不消费应用级 `access_token`）；频率 5 万/分钟；`refresh_token`（30 天）生命周期归宿主 |
  | `Mass/` | 7 | `mass/send` 服务号专属；`clientmsgid` 24 小时防重；提交成功 ≠ 群发完成，结果经 `MASSSENDJOBFINISH` 推送 |
  | `Qrcode/` | 1 | 服务号专属；`showqrcode` 换图走 `mp.weixin.qq.com`、无须登录态，SDK 不建模 |
  | `AutoReply/` | 1 | 只读查询；认证/未认证的服务号/订阅号与测试号均有权限（官方原文） |
  | `Draft/` | 6 | `draft/switch` 官方已废弃不实现；`batchget` 的 `count` 1~20 |
  | `FreePublish/` | 5 | 仅认证；提交成功不等于发布完成；`batchget` 的 `count` 1~20；条目键为 `article_id` |
  | `ProductCard/` | 1 | 路径前缀 `/channels/ec/`（视频号小店域），非 `/cgi-bin/` |
  | `Comment/` | 8 | 仅认证 + 留言功能权限（88000）；以 `msg_data_id` + `index` 定位文章；评论列表 `count` ≤ 50（88010） |
  | `DataCube/` | 21 | 仅认证；全部 `POST /datacube/*`；跨度上限逐端点 1/7/15/30 天，越界由官方 61501 表达 |
  | `Media/` | 6（上传侧） | 临时素材 3 天有效；类型大小逐类核验（image 10M / voice 2M·60s / video 10M / thumb 64KB） |
  | `SmartApi/` | 12 | AI 3 + OCR 7 + 图像处理 2；OCR 七端点 100 次/天（菜单识别页无上限）；图片 < 2M；9 端点双调用形态 ⇒ 每端点双方法 |
  | `QrcodeJump/` | 4 | 服务号专属；官方 5 次/秒（44990）；发布配额每月 100 次（886000）；须先关联小程序（61007） |
  | `ShortLink/` | 2 | `long_data` ≤ 4KB、`expire_seconds` ≤ 2592000 秒（30 天），越界由官方 9410010/9410011 表达 |
  | `Store/` | 12 | **仅开放电商类目**（43104）；主体级配额上限（管理员手机/微信号/身份证/主体各 5 次） |
  | `OneCode/` | 6 | 服务号需**申请开通**（非「仅认证」）；`code_count` 须为 10000 的整数倍且 ∈ [10000, 20000000] |
  | `Invoice/` | 17 | 全部消费应用级 `access_token`、不引入 `api_ticket`；`scantitle` 独家不支持第三方代调用 |
  | `Card/` | 14（双接口） | 主体生命周期 / 投放 11 + 券码核销 3；全 POST；建卡（`card` 包装 + `card_type` 判别 11 分支）与修改（`card_id` + 分支平级、无 `advanced_info`）**不同构**；`api_ticket` 前端取卡由 `IMpTicketService` 承载；未建模 10 族共 39 端点由守卫 CD8 零路由留档 |

- **模块注册器**（`Extensions/`）：`MpModule` 枚举 28 个成员（含 `Authentication`，令牌签发随 `AddMpApp` 自动注册、不经本枚举注册路径）；`MpServiceBuilder` 27 个 `Add{域}Api()` + `AddAllApis()` + `AddModules(params MpModule[])`；每域注册委托指向源生成器产出的 `Add{域}WebApiHttpClient()`（无签入源文件）。`Build()` 期校验 `IMpAppManager` 已注册（未先 `AddMpApp` 即注册期 fail-fast）。
- **素材下载双通道**（`Media/`）：`IMpMediaDownloadService` 承载 3 条**响应为二进制流**的端点（`/cgi-bin/media/get`、`/cgi-bin/media/get/jssdk`、`/cgi-bin/material/get_material`），走 `SendRawAsync` + Content-Type 分支判错，**不进 JSON 生成管线**；命中令牌失效码时失效并重试一次。
- **JS-SDK 签名服务**（`Web/` + `Extensions/MpJsApiSignatureExtensions.cs`）：`AddMpJsApiSignature()` 注册 `IMpJsApiSignatureService`，取 `type=jsapi` 票据按官方算法产出 `wx.config` 五字段；**刻意不返回 `jsapi_ticket` 与原始签名串**。
- **动态回调来源 IP 白名单**（`Callback/`，命名空间复用 `Mud.Wechat.OfficialAccount.Callback`）：`AddMpCallbackSourceIpWhitelist()` 注册刷新服务 + 内存快照提供者，实现 Abstractions 的 `IMpCallbackSourceIpProvider`。**默认不注册即完全关闭**（该能力必然带来周期性 API 调用，故用注册式开关而非配置项）；与静态 `MpCallbackOptions.AllowedSourceIPs` 取并集。
- **AOT JsonContext 合并**（`Extensions/MpJsonResolverExtensions.cs`）：合并 OA DataModels 的 29 个域 `JsonContext` 进组件序列化管线（仅 `NET8_0_OR_GREATER`）。**该清单必须与 `Generated/` 目录逐项对齐**——组件的 AOT 分支只组合此处登记的上下文、绝不回退反射，漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。

## 用法

### 注册模块

```csharp
// Program.cs：令牌与多公众号底座先行（AddMpApp 落 Abstractions），再按需链式注册模块
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(builder => builder
    .AddBasicApi()              // 基础接口
    .AddUserApi()               // 用户信息
    .AddMenuApi()               // 自定义菜单
    .AddTemplateApi());         // 模板消息（服务号）
```

### 注入客户端、调用端点

客户端基于 `Mud.HttpUtils` 声明式生成，**令牌的获取 / 缓存 / 提前刷新 / errcode 失效恢复全自动**（官方契约强制注入走 Query）：

```csharp
using Mud.Wechat.OfficialAccount;                                  // 业务接口 + 下载通道
using Mud.Wechat.OfficialAccount.Extensions;                        // AddMpServices / MpModule
using Mud.Wechat.OfficialAccount.Abstractions;                      // AddMpApp
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;       // IMpAppContextSwitcher
using Mud.Wechat.OfficialAccount.DataModels.User;                   // 域 DTO

public sealed class FollowerService(
    IMpUserService users,                       // 用户信息域
    IMpAppContextSwitcher switcher,             // 多公众号切换器（Abstractions）
    IMpMediaDownloadService media)              // 素材下载双通道
{
    // lang 为可选 Query 参数 ⇒ 取消令牌须具名传（位置实参会绑到 lang）
    public Task<MpUserInfoResponse> GetAsync(string openId, CancellationToken ct)
        => users.GetUserInfoAsync(openId, cancellationToken: ct);

    // 多公众号：切换当前应用上下文（一次性 using，释放自动归还），令牌解析到该公众号
    public async Task<MpGetFansResponse> GetFansOfAsync(string appKey, CancellationToken ct)
    {
        using var scope = switcher.UseAppScope(appKey);
        return await users.GetFansAsync(nextOpenId: null, ct);
    }

    // 下载通道：成功是文件流、失败才是 JSON errcode —— 流由调用方释放，SDK 不落盘
    public async Task SaveVoiceAsync(string mediaId, string path, CancellationToken ct)
    {
        using var result = await media.DownloadTemporaryMediaAsync(mediaId, ct);
        if (result.VideoUrl != null) return;      // 视频素材为 JSON 形态（只有下载地址）
        if (result.Content == null) return;       // 其余形态必为文件流
        await using var file = File.Create(path);
        await result.Content.CopyToAsync(file, ct);
    }
}
```

### 错误处理

非零 `errcode`（且非令牌失效码 `{40001, 40014, 42001}`）抛 `MpException`（`ErrorCode` + 构造期脱敏的 `RequestUri`）；令牌失效码由 `MpTokenInvalidationDetector` 识别后走恢复链路自动「失效缓存 → 刷新 → 重试」：

```csharp
try
{
    await users.UpdateRemarkAsync(new MpUpdateRemarkRequest { OpenId = openId, Remark = "vip" }, ct);
}
catch (MpException ex) when (ex.ErrorCode == 48001)
{
    // 接口无权限：该能力要求已认证账号，按官方 errcode 语义处理而非本地预判
}
```

### JS-SDK 前端签名

```csharp
builder.Services.AddMpApp(configuration, "MpApps")
               .AddMpJsApiSignature();           // 须在 AddMpApp 之后（依赖票据管理器）

// 页面控制器注入 IMpJsApiSignatureService，产出 wx.config 所需的 appId/nonceStr/timestamp/signature
var sign = await jsApi.SignAsync("https://example.com/page");   // URL 中的 # 片段自动去除
```

## 依赖

- `Mud.Wechat.Abstractions`（公用层）、`Mud.Wechat.OfficialAccount.Abstractions`、`Mud.Wechat.OfficialAccount.DataModels`
- `Mud.HttpUtils` 3.0.9、`Mud.HttpUtils.Generator` 3.0.9（分析器）
- **不引用 `Mud.Wechat.OfficialAccount.Callback`**（回调包只依赖 Abstractions，本包只实现其配置侧端口）

## 说明

- 目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`），可打包。
- `MUD005`（Query 传令牌的 URL 泄露面）项目级抑制：微信公众平台官方契约强制 `access_token` 走 Query、不支持 Header 注入，属已知接受风险；库内遥测由 `SensitiveUrlRedactor` 脱敏。
- **免令牌端点走方法级 opt-out**（上游 3.0.7 / F-1 起）：`clear_quota/v2` 应急逃生端点以 `[Token(Disabled = true)]` 随 `IMpOpenApiService` 同接口承载（A-7 合并，2026-10-14 裁决；原独立接口 `IMpOpenApiTokenFreeService` 已 `[Obsolete]`，一版后移除）；`IMpSnsService`（sns 四端点）为整域免令牌，仍独立成接口（无同域带令牌宿主接口可合并）。
- 下载通道不计入接口特性路由，但计入守卫 RC2 的全量 194 条（188 接口 + 3 令牌/票据 + 3 下载路径常量）；官方面索引页唯一路由 196 条（RC4，`.docs/` 缺失时自动跳过）。
- `netstandard2.0` 下禁 `init`/`record`/`with`、无 `ArgumentNullException.ThrowIfNull`；新增代码沿用既有 TFM 条件编译形态。
- 契约守卫位于 `Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/`（24 个文件，按域前缀 BS/TG/US/MG/CM/KF/TP/SN/OA/MS/CT/DC/MD/SM/QJ/SL/ST/OT/IT/TK/CD + QT Query 令牌注入白名单 + RC 路由计数纪律）。**端点 / 路由 / DTO / 注册面变更须同批更新守卫**。

---

## 原 `Mud.Wechat.OfficialAccount.Abstractions` 包面（2026-10 包合并并入本工程，命名空间不变）


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
- `Mud.HttpUtils` 3.0.9、`Mud.HttpUtils.Generator` 3.0.9（分析器）、`Mud.Wechat.Callback.Generator`（分析器，仅生成登记体，不产运行时引用）
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


---

## 原 `Mud.Wechat.OfficialAccount.DataModels` 包面（2026-10 包合并并入本工程，命名空间不变）


微信公众号 / 服务号 SDK **数据模型包**——官方接口出入参 DTO 与 AOT 源生成序列化上下文，独立交付以避免宿主引入不必要的运行时依赖。

## 内容

- 公众号官方 API 的请求/响应 DTO，按域目录与接口域一一对应（`AutoReply` … `User` 27 个业务域 + `WebDev`（JS-SDK 签名出参）），全部标注 `[HttpJsonSerializable(SerializerClassName = "<域段>")]`（`Mud.HttpUtils.Attributes`），共 **457 个标注类型**。
- 响应基底 `MpResponse`（根命名空间直属，归 `Common` 组）实现公用层判错契约 `IWechatApiResponse`（`ErrorCode` / `ErrorMessage` / `IsSuccess`），使 `MpException.ThrowIfFailed` 能跨产品线单点判定；124 个响应 DTO 继承本基底，`errcode` 为 0 时官方可能整体缺省该字段。
- 官方字段多为 `snake_case` ⇒ 逐属性 `[JsonPropertyName]` 显式对齐（上下文只负责元数据，不承担命名策略纠偏）。
- **文件布局**：一类型一文件（文件名 == 类型名）；域目录下的**功能族子目录仅作组织、不入命名空间**（命名空间恒为 `Mud.Wechat.OfficialAccount.DataModels.{域}`，`SerializerClassName` 仍取域段）。仅「一组同族端点共用多个类型」时才建族目录（如 `Card/Gift`、`DataCube/Article`、`Nontax/Payment`）；单端点的请求/响应三元组不建目录——类型名本身已含端点名，目录是重复信息。
- `Generated/` 目录（当前 **29** 个上下文文件）：由 `scripts/GenerateJsonContext.ps1`（`mud-jsonctx` 脚手架）按标注生成的源生成 `JsonSerializerContext`，`<auto-generated>` 头、**勿手改**、须随 DTO 变更同批提交。

## 用法（JsonContext 生成流程）

新增/变更 DTO 后按序执行（脚本路径均相对仓库根，默认参数指向 Work 线，公众号线必须显式覆盖）：

```powershell
# 1) 补标注：SerializerClassName 取命名空间最后一段（根命名空间直属文件归 Common）
./scripts/AddHttpJsonSerializable.ps1 `
    -RootPath Src/OfficialAccount/Mud.Wechat.OfficialAccount.DataModels `
    -RootNamespace Mud.Wechat.OfficialAccount.DataModels

# 2) 重生成上下文：按分组发射 Generated/{组}JsonContext.g.cs
./scripts/GenerateJsonContext.ps1 `
    -TargetProject Src/OfficialAccount/Mud.Wechat.OfficialAccount/Mud.Wechat.OfficialAccount.csproj `
    -OutputDir Src/OfficialAccount/Mud.Wechat.OfficialAccount/Generated

# 3) AOT 严格门禁复测（未登记的标注类型会被组件分析器 AOT006 打红）
dotnet build Src/OfficialAccount/Mud.Wechat.OfficialAccount.DataModels -c Release -f net8.0 -p:AotStrictMode=true
```

生成的每个上下文形如：

```csharp
#if NET8_0_OR_GREATER
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = false)]
[JsonSerializable(typeof(global::Mud.Wechat.OfficialAccount.DataModels.MpResponse))]
internal partial class CommonJsonContext : JsonSerializerContext { }
#endif
```

宿主侧无需手工串联上下文：主包 `MpJsonResolverExtensions` 把 29 个上下文合并进组件序列化管线（仅 net8.0+），`internal` 类型经 `InternalsVisibleTo` 对主包与守卫测试直读。业务代码只需注入声明式客户端，序列化由源生成元数据驱动：

```csharp
// 反序列化走 JsonTypeInfo，勿以反射方式调用 JsonSerializer.Serialize<T>/Deserialize<T>
MpUserInfoResponse info = await users.GetUserInfoAsync(openId, cancellationToken: ct);
```

## 说明

- `SerializerClassName` = DTO 命名空间的域段，与目录树一一对应 ⇒ 每个上下文与其域 DTO 同命名空间（对齐 Mud.Feishu.DataModels「每模块一上下文」）。**域分组全量清单以 `Generated/` 目录为准**，本文不逐一维护。
- 脚手架把每个上下文放在「该分组第一个标注类型的命名空间」：单命名空间分组（含 `Common`）稳定；若重生成后命名漂移，消费方 `using` 会编译失败——修 `using`，不修生成物。
- 上下文以 `#if NET8_0_OR_GREATER` 门控：低 TFM（netstandard2.0 / net6.0）走反射 STJ，AOT 门禁只对 net8.0+ 构建生效。
- 每个 DTO 单独标注（不启用脚手架的多态自动补全开关）：登记单元即 457 条 `JsonSerializable`，无开放泛型参与。
- 目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`），可打包；本包为纯 DTO，无 DI 入口、无令牌/客户端类型。

## 依赖

`Mud.Wechat.Abstractions`（叶层，仅取 `IWechatApiResponse` 响应契约；依赖方向单向 DataModels → 公用层）+ `Mud.HttpUtils.Attributes` 3.0.9（零依赖特性包）+ `Mud.HttpUtils.Generator` 3.0.9（仅分析器，`PrivateAssets=all`）。

