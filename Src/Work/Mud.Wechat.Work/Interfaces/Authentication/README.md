# Authentication 域 · 服务商登录授权

第三方/服务商套件级授权流程接口（消费 suite_access_token，属业务面）。 接口：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)、[IWechatWorkProviderAuthenticationUrl](./IWechatWorkProviderAuthenticationUrl.cs)、[IWechatWorkProviderLoginService](./IWechatWorkProviderLoginService.cs)；注册组 `Authentication`；模块枚举 `WechatModule.Authentication`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatContractGuards.Authorization.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.Authorization.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatProviderLoginContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatProviderLoginContractGuards.cs)、[WechatPayToolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)）、本域 DTO（**无独立 DTO 目录**（类型见「DTO 与序列化面」））。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkProviderAuthenticationService` | `GetPreAuthCodeAsync` | GET | `/cgi-bin/service/get_pre_auth_code` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取预授权码 |
| 2 | `IWechatWorkProviderAuthenticationService` | `SetSessionInfoAsync` | POST | `/cgi-bin/service/set_session_info` | `SuiteAccessToken`（Query），Query `suite_access_token` | 设置授权配置 |
| 3 | `IWechatWorkProviderAuthenticationService` | `GetPermanentCodeAsync` | POST | `/cgi-bin/service/get_permanent_code` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取企业永久授权码 |
| 4 | `IWechatWorkProviderAuthenticationService` | `GetAuthInfoAsync` | POST | `/cgi-bin/service/get_auth_info` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取企业授权信息 |
| 5 | `IWechatWorkProviderAuthenticationService` | `GetPermanentCodeV2Async` | POST | `/cgi-bin/service/v2/get_permanent_code` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取企业永久授权码（v2） |
| 6 | `IWechatWorkProviderAuthenticationService` | `GetAuthInfoV2Async` | POST | `/cgi-bin/service/v2/get_auth_info` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取企业授权信息（v2） |
| 7 | `IWechatWorkProviderAuthenticationService` | `GetAppQrcodeAsync` | POST | `/cgi-bin/service/get_app_qrcode` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取应用二维码 |
| 8 | `IWechatWorkProviderAuthenticationUrl` | `GetCustomizedAuthUrlAsync` | POST | `/cgi-bin/service/get_customized_auth_url` | 无 `[Token]`（见域级说明） | 获取代开发自建应用的带参授权链接（携带 `provider_access_token` 于 Query）。 |
| 9 | `IWechatWorkProviderLoginService` | `GetLoginInfoAsync` | POST | `/cgi-bin/service/get_login_info` | `ProviderAccessToken`（Query），Query `provider_access_token` | 获取登录用户信息 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkProviderAuthenticationService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 是 |
| `IWechatWorkProviderAuthenticationUrl` | — | — | — | 是 |
| `IWechatWorkProviderLoginService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkProviderAuthenticationService`

企业微信的系统管理员可以授权安装第三方应用，安装后企业微信后台会将授权凭证、授权信息等推送给服务商后台。
请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/90597

落位与形态对齐 Feishu：BaseAddress 交由运行时解析（per-app `WechatAppConfig.BaseUrl`，
默认 `https://qyapi.weixin.qq.com`），令牌由框架经 [Token] 自动注入
（TokenManagerKey 路由键 `SuiteAccessToken`，Query 注入参数名
`suite_access_token`）。

MUD005 已知接受风险：企业微信官方契约强制套件令牌走 Query 参数，无法改用 Header，
该诊断属预期且不可规避（详见详细设计 §1）。

### `IWechatWorkProviderAuthenticationUrl`

用于为代开发自建应用生成安装二维码链接（企业管理员扫码后完成安装授权）。
请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/98744

形态对齐 `IWechatWorkCorpTokenAuthentication`：
**显式传令牌参数**（`[Query("provider_access_token")]`），不带 `[Token]`——
服务商令牌是 per-app 的，须先经 `IWechatAppContextSwitcher.UseAppScope(appKey)` 切到目标应用后，
由编排服务从该应用的 `IWechatProviderTokenManager` 取值再显式传入。

因此本接口不进入契约守卫 G5（Query 令牌注入白名单），白名单保持仅
`IWechatWorkProviderAuthenticationService`。

注册复用 `AddAuthenticationWebApiHttpClient()`（RegistryGroupName = "Authentication"），不新增独立注册项。

### `IWechatWorkProviderLoginService`

用于服务商在自有网站（或 Web 登录组件 / 扫码登录链接，链接构造见官方 91139）上完成管理员扫码授权后，
以跳转回的 `auth_code` 换取登录用户信息——与「授权流接口」（get_pre_auth_code / get_permanent_code 等，
见 `IWechatWorkProviderAuthenticationService`）分族承载，互不混用。

令牌路由键说明：消费服务商主体 `ProviderAccessToken`
（Query 注入 `provider_access_token`，由 `corpid + provider_secret` 经
`get_provider_token` 换取，Abstractions 令牌底座已覆盖）。
该令牌凭据来源唯一（服务商主体级），非应用类型子接口、不声明 `TokenManagerKey` 归属域键。

**官方契约特例（errcode 缺省成功语义）**：官方 91154 载明「因历史原因，调用失败时才返回 errcode，
**无 errcode 字段视为成功**」。本 SDK 响应体直接继承 `WechatWorkResponse`
（`errcode` 为 `int`、缺省 0 ⇒ 报文缺失 errcode 字段时反序列化保持 0，
`IsSuccess` / 判错出口即按成功处理）——该判定路径由守卫 PL1 锁定，不得改为可空 errcode 或自建判定器。

**SSO 扫码登录链接构造（官方 91139，按方案默认降级为文档说明、不设 SDK 端点）**：
链接本质为页面跳转 URL 拼装（`https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=CORPID&agentid=AGENTID&redirect_uri=...&state=...`），
不产生 qyapi HTTP 请求，交由宿主按官方文档拼装；如后续需要 SDK 承载，再以独立 URL 构造器成员评估。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`provider_access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetPreAuthCodeAsync` — 获取预授权码

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`GET /cgi-bin/service/get_pre_auth_code`
- **返回**：`GetPreAuthCodeResponse` — 获取预授权码响应

**补充说明**

- 用于获取预授权码。预授权码用于企业授权时的第三方服务商安全验证。

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/90601

### 2. `SetSessionInfoAsync` — 设置授权配置

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`POST /cgi-bin/service/set_session_info`

- **请求体**：`SetSessionInfoRequest` — 请求参数
- **返回**：`WechatWorkResponse` — 设置授权配置响应

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/90602

### 3. `GetPermanentCodeAsync` — 获取企业永久授权码

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`POST /cgi-bin/service/get_permanent_code`

- **请求体**：`GetPermanentCodeRequest` — 请求参数
- **返回**：`GetPermanentCodeResponse` — 获取企业永久授权码响应

**补充说明**

- 用于通过临时授权码换取企业微信的永久授权码与授权信息。

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/90603

### 4. `GetAuthInfoAsync` — 获取企业授权信息

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`POST /cgi-bin/service/get_auth_info`

- **请求体**：`GetAuthInfoRequest` — 请求参数
- **返回**：`GetAuthInfoResponse` — 获取企业授权信息响应

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/91912

### 5. `GetPermanentCodeV2Async` — 获取企业永久授权码（v2）

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`POST /cgi-bin/service/v2/get_permanent_code`

- **请求体**：`GetPermanentCodeRequest` — 请求参数
- **返回**：`GetPermanentCodeResponse` — 获取企业永久授权码响应

**补充说明**

- 官方推荐版本：耗时更短，适合在授权回调中同步换取永久授权码（旧版易超时）。

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/100776

### 6. `GetAuthInfoV2Async` — 获取企业授权信息（v2）

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`POST /cgi-bin/service/v2/get_auth_info`

- **请求体**：`GetAuthInfoRequest` — 请求参数
- **返回**：`GetAuthInfoResponse` — 获取企业授权信息响应

**补充说明**

- 官方推荐版本：不返回插件关注二维码、性能更好。

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/100795

### 7. `GetAppQrcodeAsync` — 获取应用二维码

- **声明接口**：[IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)
- **官方契约**：`POST /cgi-bin/service/get_app_qrcode`

- **请求体**：`GetAppQrcodeRequest` — 请求参数（`GetAppQrcodeRequest`：suite_id / appid / state / style / result_type）。
- **返回**：`GetAppQrcodeResponse` — 二维码 url 地址（官方 qrcode 字段；官方契约陷阱：result_type=2 时 JSON 返回该 url，result_type=1 时返回 image/png 二进制流而非 JSON）。

**补充说明**

- 获取第三方应用的应用二维码（用于扫码授权安装）； `state` 可区分安装渠道，扫带参二维码授权安装后「获取企业永久授权码」会返回该 state 值。

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/95430
- 官方权限说明：要求第三方应用是已上线的第三方通用应用；state 仅可填写 a-zA-Z0-9、长度不可超过 32 个字节。
- 官方契约陷阱：result_type 官方默认为 1（返回二维码图片 buffer，非 JSON）， / 走本 SDK JSON 契约面时须传 2（返回二维码图片 url）；响应字段官方原文为 qrcode。

### 8. `GetCustomizedAuthUrlAsync` — 获取代开发自建应用的带参授权链接（携带 `provider_access_token` 于 Query）。

- **声明接口**：[IWechatWorkProviderAuthenticationUrl](./IWechatWorkProviderAuthenticationUrl.cs)
- **官方契约**：`POST /cgi-bin/service/get_customized_auth_url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `providerAccessToken` | Query `provider_access_token` | `string` | 服务商访问令牌（provider_access_token，Query 注入）。 |

- **请求体**：`GetCustomizedAuthUrlRequest` — 获取带参授权链接请求体（state + templateid_list）。
- **返回**：`GetCustomizedAuthUrlResponse?` — 带参授权链接响应（含 qrcode_url）。

**备注（原文）**

- 请参照原SDK文档：https://developer.work.weixin.qq.com/document/path/98744

### 9. `GetLoginInfoAsync` — 获取登录用户信息

- **声明接口**：[IWechatWorkProviderLoginService](./IWechatWorkProviderLoginService.cs)
- **官方契约**：`POST /cgi-bin/service/get_login_info`

- **请求体**：`GetLoginInfoRequest` — 请求体（`GetLoginInfoRequest`：auth_code，
最长 512 字节）。
- **返回**：`GetLoginInfoResponse` — 登录用户信息（corpid / userid / name / avatar / gender）。

**补充说明**

- 用户在扫码登录授权（或 Web 登录组件）中确认后，官方以 `auth_code`（随跳转返回）为凭证， 本接口换取该次登录的用户身份信息。

**备注（原文）**

- **服务商·获取登录用户信息**SDK文档：https://developer.work.weixin.qq.com/document/path/91154
- 官方约束：`auth_code` 最长 512 字节，**单次使用、5 分钟内有效**，过期作废； / **errcode 语义特例**——调用失败时才返回 errcode，无 errcode 字段视为成功； / 返回 `name` 字段自 2020-06-30 起对服务商**不再返回真实姓名**（返回 userid，历史兼容字段）。

## DTO 与序列化面

本域**没有独立的 DataModels 子目录**；端点签名直接引用的类型在下方按类型索引回溯，未命中者说明其定义不在 `Mud.Wechat.Work.DataModels`。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetPreAuthCodeResponse` | 应答 | [ProviderAuthentication/ResponseModel/GetPreAuthCodeResponse.cs](../../../Mud.Wechat.Work/ProviderAuthentication/ResponseModel/GetPreAuthCodeResponse.cs) | `pre_auth_code`, `expires_in` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../../Mud.Wechat.Work/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `SetSessionInfoRequest` | 请求体 | [ProviderAuthentication/RequestModel/SetSessionInfoRequest.cs](../../../Mud.Wechat.Work/ProviderAuthentication/RequestModel/SetSessionInfoRequest.cs) | `pre_auth_code`, `session_info`, `appid`, `auth_type` |
| `GetPermanentCodeResponse` | 应答 | [ProviderAuthentication/ResponseModel/GetPermanentCodeResponse.cs](../../../Mud.Wechat.Work/ProviderAuthentication/ResponseModel/GetPermanentCodeResponse.cs) | `access_token`, `expires_in`, `permanent_code`, `dealer_corp_info`, `auth_corp_info`, `auth_info`, `auth_user_info`, `register_code_info`, `state`, `userid`, `open_userid`, `name` … |
| `GetPermanentCodeRequest` | 请求体 | [ProviderAuthentication/RequestModel/GetPermanentCodeRequest.cs](../../../Mud.Wechat.Work/ProviderAuthentication/RequestModel/GetPermanentCodeRequest.cs) | `auth_code` |
| `GetAuthInfoResponse` | 应答 | [ProviderAuthentication/GetAuthInfoResponse.cs](../../../Mud.Wechat.Work/ProviderAuthentication/GetAuthInfoResponse.cs) | `dealer_corp_info`, `auth_corp_info`, `auth_info` |
| `GetAuthInfoRequest` | 请求体 | [ProviderAuthentication/GetAuthInfoRequest.cs](../../../Mud.Wechat.Work/ProviderAuthentication/GetAuthInfoRequest.cs) | `auth_corpid`, `permanent_code` |
| `GetAppQrcodeResponse` | 应答 | [ProviderAuthentication/ResponseModel/GetAppQrcodeResponse.cs](../../../Mud.Wechat.Work/ProviderAuthentication/ResponseModel/GetAppQrcodeResponse.cs) | `qrcode` |
| `GetAppQrcodeRequest` | 请求体 | [ProviderAuthentication/RequestModel/GetAppQrcodeRequest.cs](../../../Mud.Wechat.Work/ProviderAuthentication/RequestModel/GetAppQrcodeRequest.cs) | `suite_id`, `appid`, `state`, `style`, `result_type` |
| `GetCustomizedAuthUrlResponse` | 应答 | [ProviderAuthentication/ResponseModel/GetCustomizedAuthUrlResponse.cs](../../../Mud.Wechat.Work/ProviderAuthentication/ResponseModel/GetCustomizedAuthUrlResponse.cs) | `qrcode_url`, `expires_in` |
| `GetCustomizedAuthUrlRequest` | 请求体 | [ProviderAuthentication/RequestModel/GetCustomizedAuthUrlRequest.cs](../../../Mud.Wechat.Work/ProviderAuthentication/RequestModel/GetCustomizedAuthUrlRequest.cs) | `state`, `templateid_list` |
| `GetLoginInfoResponse` | 应答 | [ProviderAuthentication/ResponseModel/GetLoginInfoResponse.cs](../../../Mud.Wechat.Work/ProviderAuthentication/ResponseModel/GetLoginInfoResponse.cs) | `corpid`, `userid`, `name`, `avatar`, `gender` |
| `GetLoginInfoRequest` | 请求体 | [ProviderAuthentication/RequestModel/GetLoginInfoRequest.cs](../../../Mud.Wechat.Work/ProviderAuthentication/RequestModel/GetLoginInfoRequest.cs) | `auth_code` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddAuthenticationApi());

var service = app.Services.GetRequiredService<IWechatWorkProviderAuthenticationService>();
var ct = CancellationToken.None;
var response = await service.GetPreAuthCodeAsync(ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/service/get_pre_auth_code");
// 占位值仅用于编译示意；业务值须按 IWechatWorkProviderAuthenticationService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetPreAuthCodeAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| DTO 落位 | 本域无独立 `Mud.Wechat.Work/Authentication` 目录，类型散落在共享目录或其它域 | 以类型索引与源文件为准；新增 DTO 时先定落位 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkProviderAuthenticationService](./IWechatWorkProviderAuthenticationService.cs)、[IWechatWorkProviderAuthenticationUrl](./IWechatWorkProviderAuthenticationUrl.cs)、[IWechatWorkProviderLoginService](./IWechatWorkProviderLoginService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatContractGuards.Authorization.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.Authorization.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatProviderLoginContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatProviderLoginContractGuards.cs)、[`WechatPayToolContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs) |
| 守卫中的代表性测试方法 | `AuthorizationEndpoints_ShouldMatchOfficialRoutes`、`CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys`、`AwaitSharedAsync_CancellationSignal_ShouldRunContinuationsAsynchronously`、`ResolveAppKey_ShouldFailFast_WhenMultiAppWithoutConfiguredDefault`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces`、`ProviderLoginEndpoint_ShouldMatchOfficialRoute_AndKeepDefaultSuccessSemantics`、`ProviderLoginTokenBinding_ShouldBeProviderAccessTokenInjectedViaQuery`、`ProviderLoginDataModels_ShouldBeRegisteredInExistingJsonContext`、`PayToolEndpoints_ShouldMatchOfficialRoutes`、`PayToolTokenBinding_ShouldSplitProviderAndSuiteRoutes`、`PayToolSignatureFields_ShouldExistOnlyOnOrderFamily`、`PayToolDataModels_ShouldBeRegisteredInJsonContext` … |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | `Mud.Wechat.Work.DataModels`（无本域子目录） |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatContractGuards.Authorization.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatProviderLoginContractGuards.cs`、`WechatPayToolContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
