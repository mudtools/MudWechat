# 服务商登录授权（扫码登录换登录用户信息）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Authentication` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/Authentication/IWechatWorkProviderLoginService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/IWechatWorkProviderLoginService.cs) |
| DTO 源文件 | [GetLoginInfoRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/RequestModel/GetLoginInfoRequest.cs) · [GetLoginInfoResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/ResponseModel/GetLoginInfoResponse.cs) |
| 契约守卫 | [WechatProviderLoginContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatProviderLoginContractGuards.cs)（PL1 路由 + errcode 缺省成功语义 / PL2 令牌绑定 / PL3 上下文登记） |
| 官方文档 | 91154（服务商登录授权）· 91127（SSO 概述）· 91139（扫码登录链接构造，**不设 SDK 端点**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段，接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PL1~PL3 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

让**服务商在自己网站（或自有 Web 登录组件）上完成「企业管理员扫码授权 → 拿到该管理员身份」**：管理员扫码确认后，官方把这次登录的 `auth_code` 随跳转回传给你的回调页；你用它调 `get_login_info`，换回**授权企业 corpid + 登录用户 userid/头像等**，据此在自己的业务系统里建立会话。

它解决的是「服务商侧的单点登录」，与企业自建应用的「网页授权」不是一回事：本族以**服务商主体**身份消费 `provider_access_token`，凭据与「服务商代开发/第三方应用」共用一套服务商凭证，而不是某个授权企业的 `access_token`。

三族分工：

| 族 | 用途 | 文档 |
|---|---|---|
| 授权流（`get_pre_auth_code` / `get_permanent_code` …） | 企业**安装/授权应用**，换永久授权码与授权信息 | [ProviderAuthentication.md](./ProviderAuthentication.md) |
| 带参授权链接（`get_customized_auth_url`） | 生成**安装二维码链接** | [ProviderAuthenticationUrl.md](./ProviderAuthenticationUrl.md) |
| 登录授权（`get_login_info`） | 管理员扫码**登录你的网站**，换登录用户身份 | 本文 |

**SSO 扫码登录链接（官方 91139）不设 SDK 端点**：该链接本质是页面跳转 URL 拼装（`https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=CORPID&agentid=AGENTID&redirect_uri=...&state=...`），不产生 `qyapi` HTTP 请求，由宿主按官方文档自行拼装即可。接口注释与守卫 PL1 都把「本族仅 1 个 HTTP 端点」写成断言——不要期待 SDK 提供「生成登录链接」的方法。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Provider`（服务商代开发）或 `WechatAppType.ThirdParty`（第三方） | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret`（四项齐备，缺一启动即抛） | 同上 |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken`（`Wechat.ProviderAccessToken`）→ 官方参数 `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入，**不在方法签名里** | 每次请求 |
| 令牌归属键 | 无（服务商主体级凭据，一个服务商的分身共享同一枚） | — |

`provider_access_token` 由 `ProviderTokenManager` 以 `CorpId + ProviderSecret` 换取、缓存并按 `expires_in` 刷新，**开发者不接触令牌值**。官方对 `get_provider_token` 明确警示「不能频繁调用，否则会受到频率拦截」，不要自建取令牌兜底。

> 多应用部署时，自动注入取的是**当前应用上下文**的令牌——调用前先 `IWechatAppContextSwitcher.UseAppScope(appKey)` 进入目标服务商应用，否则会取到默认应用的令牌（令牌本身同主体，但在不同服务商身份下会直接失败）。

### 2.2 权限与业务前置

- 目标企业管理员须已在官方页面完成**扫码登录授权**（登录动作在官方页面，SDK 不承载）；`auth_code` 由跳转参数带回你的 `redirect_uri`。
- `auth_code`：**最长 512 字节、单次使用、5 分钟内有效**，过期作废；重放同一 `auth_code` 必失败。
- 官方未在本族页面列出额外「可见范围」类前置；登录结果只反映该管理员自身身份。

### 2.3 官方开放的应用类型

服务商侧能力（`provider_access_token` 凭据来源唯一），SDK 不设应用类型子接口——它是独立接口 `IWechatWorkProviderLoginService`，自身可直接注入，不继承父接口、不声明归属域键（PL2 锁定）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkProviderLoginService` | `Mud.Wechat.Work` | 独立服务商令牌接口（单接口直接注册） | 1（E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`），可被 per-app `WechatAppConfig.BaseUrl` 覆盖
- 模块注册：`WechatModule.Authentication` / `AddAuthenticationApi()`
- 注册组名：`Authentication`（与授权流接口共用 `AddAuthenticationWebApiHttpClient()`，PL2 锁定）
- 端点索引：**E1** `GetLoginInfoAsync`（用 `auth_code` 换登录用户信息）

---

## 4. 端点

### E1. `GetLoginInfoAsync` — 获取登录用户信息

**签名**

```csharp
Task<GetLoginInfoResponse> GetLoginInfoAsync(
    [Body] GetLoginInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_login_info` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_login_info?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 服务商代开发 · 第三方（服务商主体） |
| 请求体编码 | `application/json`，源生成序列化（`ProviderAuthenticationJsonContext`，PL3 锁定登记） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.AuthCode` | `auth_code` | `string` | 是 | 扫码登录授权返回的临时授权码；**最长 512 字节、单次使用、5 分钟内有效** |
| —（不进方法签名） | `provider_access_token` | — | 是 | 服务商接口调用凭证，由 `[Token]` 在 Query 注入 |

```json
{
  "auth_code": "AUTH_CODE_FROM_REDIRECT"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | **可能整体缺失**，缺失即成功（见下方注意事项） |
| `corpid` | `CorpId` | `string?` | 授权方企业（扫码登录的企业）corpid |
| `userid` | `Userid` | `string?` | 登录用户在企业内的 userid |
| `name` | `Name` | `string?` | 官方 2020-06-30 起**不再返回真实姓名**，实测返回 userid——勿当真实姓名消费 |
| `avatar` | `Avatar` | `string?` | 头像 url |
| `gender` | `Gender` | `string?` | 性别，`"1"` 男 / `"2"` 女（官方示例按字符串传输，故以 `string` 承载） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "corpid": "CORPID",
  "userid": "zhangsan",
  "name": "zhangsan",
  "avatar": "http://wx.qlogo.cn/...",
  "gender": "1"
}
```

> 官方原文：「因历史原因，调用失败时才返回 errcode，**无 errcode 字段视为成功**」。上面 JSON 里的 `errcode: 0` 只是成功形态之一——真实成功报文可能**根本没有** `errcode` 键。

**调用示例**

```csharp
using Mud.Wechat.Work;                        // 可注入接口 + AddWechatWorkServices / AddAuthenticationApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;     // WechatAppType
using Mud.Wechat.Work.DataModels.ProviderAuthentication; // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，GetRequiredService 与 logger 扩展方法由 Web SDK 隐式 using 提供。

// ① 注册服务商应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.Provider;       // 第三方应用改填 WechatAppType.ThirdParty
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配）
builder.Services.AddWechatWorkServices(b => b.AddAuthenticationApi());

// ③ 解析接口。provider_access_token 由 [Token] 自动注入，方法签名里看不到令牌。
var loginApi = provider.GetRequiredService<IWechatWorkProviderLoginService>();

// ④ auth_code 来自你 redirect_uri 上的跳转参数（单次使用，5 分钟内有效，先落库/先校验再消费）。
string authCode = "<跳转回传的 auth_code>";

var login = await loginApi.GetLoginInfoAsync(
    new GetLoginInfoRequest { AuthCode = authCode },
    cancellationToken);

// ⑤ 官方契约特例：成功报文可能不含 errcode（缺省 0 ⇒ IsSuccess 为 true），
//    失败时才带 errcode。ThrowIfFailed 已按该语义判定，不要自己写「errcode 必须存在」。
WechatWorkException.ThrowIfFailed(login, "/cgi-bin/service/get_login_info");

// ⑥ Name 自 2020-06-30 起对服务商不再返回真实姓名（返回 userid），业务侧以 Userid 为主键。
//    userid / avatar 属个人数据：可落库，但不要写进日志与遥测。
logger.LogInformation("服务商登录成功：corp={CorpId}", login.CorpId);
```

**本端点注意事项**

- **errcode 缺省成功语义是本族的头号陷阱**：官方「无 errcode 字段视为成功」。SDK 的 `GetLoginInfoResponse` 直接继承 `WechatWorkResponse`（`errcode` 为 `int`、缺省 0），缺字段时 `IsSuccess` 即为 `true`；守卫 PL1 锁定「不得改成可空 errcode / 自定义 IsSuccess / 自建判定器」。因此**不要**自行判断「响应里没有 errcode 所以是异常报文」。
- **`auth_code` 一次性**：同一码重放必失败；跳转页收到 `auth_code` 后不要先展示、后兑换的异步流程拖到 5 分钟外。
- **`name` 不是姓名**：服务商拿到的 `name` 自 2020-06-30 起即 userid，展示姓名需要企业侧另行提供。
- **`gender` 是字符串**：`"1"`/`"2"`，不是整数——按 `int` 解析会抛反序列化异常。
- 登录成功后你只拿到**身份**，没有拿到企业级 `access_token`；要调企业数据接口须另走授权流拿 `permanent_code`（见 [ProviderAuthentication.md](./ProviderAuthentication.md)）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 「无 errcode 视为成功」 vs 其他企微端点 | 其余端点**必有** `errcode`，非 0 即失败；本端点是官方历史特例 | 复制其他端点的「严格校验 errcode 存在」写法会误判成功响应为失败 |
| `get_login_info` vs 授权流 `get_permanent_code` | 前者用**登录** `auth_code` 换身份；后者用**安装授权**临时码换 `permanent_code` | 两类 `auth_code` 不可互换，混用报无效码 |
| 本族 `provider_access_token` 自动注入 vs [ProviderAuthenticationUrl.md](./ProviderAuthenticationUrl.md) 显式传参 | 本族带 `[Token]`、Query 自动注入；带参授权链接那支不带 `[Token]`，令牌走方法首参 | 按带参链接的写法去传令牌，本族方法没有该参数 |
| `get_login_info` vs [ProviderAuthentication.md](./ProviderAuthentication.md) 的 `get_app_qrcode` / 本域的 `get_customized_auth_url` | 后两者生成**安装**入口；本端点发生在安装**之后**的登录环节 | 把安装链接当登录入口、或把登录 `auth_code` 送去换永久码 |
| SSO 扫码登录链接（91139） | 纯 URL 拼装，主机为 `open.work.weixin.qq.com`，不产生 qyapi 请求 | 在 SDK 里找「生成登录链接」方法会落空；需自行按官方文档拼装 |
| `gender` 字符串 vs 其他端点的整数枚举 | 官方示例按字符串传 | 按 `int` 建模/解析会抛异常 |

SDK 侧通用行为约束：

- **不自动判错**：`errcode != 0` 时 HTTP 仍为 200、响应照样反序列化成功；必须 `IsSuccess` 或 `ThrowIfFailed`。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险）。`provider_access_token` 在组件 `SensitiveUrlRedactor` 脱敏词表内，但自己打日志时不要把完整 URL 打出去。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 / **字段缺失** | `WechatErrorCodes.Success` | 成功（本族两种形态都算成功） | 正常路径 |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效/过期 | 本端点族别失效码，走恢复链路自愈 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 企业级 `access_token` 族失效 | 本端点不消费该令牌；出现即凭据族别错配 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 同上 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 属装配/配置缺陷，不自愈 |

令牌失效自愈集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`，命中后「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」。若 42011 稳定复现，说明刷新本身在失败：排查 `ProviderSecret`、服务商是否已开通能力、官方是否对 `get_provider_token` 限流——加重试不解决这三类成因。

`auth_code` 过期/重放等本端点专属业务码官方未在接口注释中留档，见 §7；通用码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `auth_code` 失效的具体 errcode | 接口注释只留「单次使用、5 分钟有效」，未记码值 ⇒ **未逐项核验** | 官方 91154 页面 + 全局错误码（90455） |
| 端点级频率限制 | 接口 XML 注释未记录 QPS/次数 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| SSO 登录链接的 `state` 长度上限 | 官方 91139 的 `state` 规则本文未取证（与安装链接 ≤128 字节不同）⇒ **未逐项核验** | 官方 91139 页面 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，未复述 91154/91127 正文；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |
| 仓内 `guide/` | 「配置与凭据」「错误处理」未成文，多应用作用域与判错口径无集中文档 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md)
- 兄弟族：[ProviderAuthentication.md](./ProviderAuthentication.md)（套件授权流）· [ProviderAuthenticationUrl.md](./ProviderAuthenticationUrl.md)（代开发带参授权链接）
- 官方页：[服务商登录授权（91154）](https://developer.work.weixin.qq.com/document/path/91154) · [SSO 概述（91127）](https://developer.work.weixin.qq.com/document/path/91127) · [扫码登录链接（91139）](https://developer.work.weixin.qq.com/document/path/91139) · [获取服务商凭证（91200）](https://developer.work.weixin.qq.com/document/path/91200)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
