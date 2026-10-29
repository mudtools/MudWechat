# 第三方 / 服务商套件授权流（预授权码 → 永久授权码 → 授权信息）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Authentication` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/Authentication/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/) |
| DTO 源文件 | [GetPreAuthCodeResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/ResponseModel/GetPreAuthCodeResponse.cs) · [SetSessionInfoRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/RequestModel/SetSessionInfoRequest.cs) · [GetPermanentCodeRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/RequestModel/GetPermanentCodeRequest.cs) · [GetPermanentCodeResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/ResponseModel/GetPermanentCodeResponse.cs) · [GetAuthInfoRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/GetAuthInfoRequest.cs) · [GetAuthInfoResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/GetAuthInfoResponse.cs) · [GetAppQrcodeRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/RequestModel/GetAppQrcodeRequest.cs) · [GetAppQrcodeResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/ResponseModel/GetAppQrcodeResponse.cs) |
| 契约守卫 | [WechatContractGuards.Authorization.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.Authorization.cs)（G6 授权端点路由）· [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5 Query 令牌白名单） |
| 官方文档 | 套件授权总览 90597 · 90601 / 90602 / 90603 / 91912（v1）· 100776 / 100795（v2）· 95430（应用二维码） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段，接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：G5 Query 令牌白名单 / G6 授权端点路由 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档，仅作定位标识 |

## 1. 这个接口族做什么

第三方应用（Suite）与代开发套件的**授权主流程**：先取一枚**预授权码**（`pre_auth_code`），宿主把它拼进官方授权页 URL 交给企业管理员；管理员确认安装后，官方把**临时授权码**（`auth_code`）回调给服务商后台；服务商再用 `auth_code` 换取**永久授权码**（`permanent_code`）与该企业的授权信息，此后凭 `permanent_code` 反复查询授权状态。

另外两支业务辅助端点：`set_session_info` 在授权页打开前限定本次可授权的应用与授权类型；`get_app_qrcode` 生成安装二维码（可带 `state` 区分渠道，扫带参码安装后 `get_permanent_code` 会把该 `state` 原样带回）。

**不该用本族的场景**：管理员扫码**登录**服务商自有网站、需要换出「登录用户是谁」，那是 [ProviderLoginService.md](./ProviderLoginService.md)（`provider_access_token` + `get_login_info`）；为代开发自建应用生成**带参授权链接**，是 [ProviderAuthenticationUrl.md](./ProviderAuthenticationUrl.md)；换到 `permanent_code` 之后的业务调用（通讯录、打卡、审批…）属各业务域，且用的是**企业级** `access_token`。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty`（第三方通用应用）；代开发套件侧填 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()`；运行期动态注册由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId` · `ProviderSecret` · `SuiteId` · `SuiteSecret`（第三方/代开发两类均要求四项齐备） | 同上，缺任一项抛 `InvalidOperationException`（启动即失败） |
| 令牌类型 | `WechatTokenTypes.SuiteAccessToken`（常量值 `Wechat.SuiteAccessToken`）→ 官方参数 `suite_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由接口上的 `[Token]` 自动注入，**不进方法签名** | 每次请求 |
| 令牌归属键 | 无 `TokenManagerKey`（套件令牌是「一个套件一份」，不是应用类型子接口，不声明归属域键） | — |

`suite_access_token` 由 `SuiteTokenManager` 以 `suite_id` + `suite_secret` + **`suite_ticket`** 换取并缓存。`suite_ticket` 只能由官方**每 10 分钟推送到回调地址**，不能主动获取；未入库时 `IWechatSuiteTicketProvider` 会抛出并提示宿主配置回调接收。因此**本族可用性依赖回调链路已接通**，见 §7。

### 2.2 权限与业务前置

- 第三方应用须是**已上线**的第三方通用应用（`get_app_qrcode` 的官方权限说明；其余端点要求企业已授权安装）。
- 授权动作由**企业管理员**在官方授权页完成，SDK 不承载页面；服务商只需按官方链接格式拼装并携带 `pre_auth_code`。
- `permanent_code` 是**凭据**：入库前的任何日志、遥测、异常消息都不得包含它（`SensitiveUrlRedactor` 只擦 URL，不会擦你自建的日志字段）。SDK 侧由 `IWechatCorpAuthStore` 按 `(AppKey, AuthCorpId)` 复合键持久化，业务代码通常不需要自己保存。

### 2.3 官方开放的应用类型

官方对**第三方应用**与**服务商代开发**开放本族，对企业自建应用不开放。SDK 因此不设 `_Internal` 分支：`IWechatWorkProviderAuthenticationService` 自身就是可注入接口（不是 `IsAbstract` 父接口），直接注入使用。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkProviderAuthenticationService` | `Mud.Wechat.Work` | 套件级授权流（`suite_access_token`） | 7（E1~E7） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`），可被 per-app `WechatAppConfig.BaseUrl` 覆盖（私有化部署）
- 模块注册：`WechatModule.Authentication` / `AddAuthenticationApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Authentication`
- 端点索引：**E1** `GetPreAuthCodeAsync`（取预授权码）· **E2** `SetSessionInfoAsync`（限定本次授权范围）· **E3** `GetPermanentCodeAsync`（v1 换永久授权码）· **E4** `GetAuthInfoAsync`（v1 查授权信息）· **E5** `GetPermanentCodeV2Async`（v2）· **E6** `GetAuthInfoV2Async`（v2）· **E7** `GetAppQrcodeAsync`（安装二维码）

---

## 4. 端点

### E1. `GetPreAuthCodeAsync` — 获取预授权码

**签名**

```csharp
Task<GetPreAuthCodeResponse> GetPreAuthCodeAsync(CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/service/get_pre_auth_code` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_pre_auth_code?suite_access_token=SUITE_ACCESS_TOKEN` |
| 凭据参数 | `suite_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 请求体 | 无 |

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `pre_auth_code` | `PreAuthCode` | `string` | 预授权码，最长 512 字节 |
| `expires_in` | `ExpiresIn` | `int` | 预授权码有效期（秒） |

```json
{ "errcode": 0, "errmsg": "ok", "pre_auth_code": "PRE_AUTH_CODE", "expires_in": 1200 }
```

> 示例中的 `expires_in` 数值为**占位**，本仓未留档官方具体秒数——按响应值使用，不要写死（见 §7）。

**本端点注意事项**

- 预授权码是**一次性**授权会话凭证，不是可长期复用的密钥；获取后应立刻拼进授权页 URL 并交付给管理员，不要缓存到自己库里。
- 该值出现在 URL 上，`SensitiveUrlRedactor` 不会覆盖 `pre_auth_code` 词条（它只覆盖令牌参数）；**不要把它打日志**。

---

### E2. `SetSessionInfoAsync` — 设置授权配置

**签名**

```csharp
Task<WechatWorkResponse> SetSessionInfoAsync(
    [Body] SetSessionInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/set_session_info` |
| 凭据参数 | `suite_access_token`（Query 注入） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.PreAuthCode` | `pre_auth_code` | `string` | 是 | E1 取得的预授权码 |
| `request.Session.AppIdList` | `session_info.appid` | `IList<int>?` | 否 | 本次允许授权的应用 id 列表（限定安装页可勾选的应用） |
| `request.Session.AuthType` | `session_info.auth_type` | `int?` | 否 | 授权类型 |

```json
{
  "pre_auth_code": "PRE_AUTH_CODE",
  "session_info": { "appid": [1, 2], "auth_type": 1 }
}
```

**响应参数**：仅 `errcode` / `errmsg`（`WechatWorkResponse` 无业务负载）。

**本端点注意事项**

- E2 必须在 **E1 之后、管理员打开授权页之前**调用；调用后该 `pre_auth_code` 的授权页才受本次配置约束。
- `session_info` 是**覆盖式**配置：不传即不限定，传了即按传入集合限定。

---

### E3. `GetPermanentCodeAsync` — 获取企业永久授权码（v1）

**签名**

```csharp
Task<GetPermanentCodeResponse> GetPermanentCodeAsync(
    [Body] GetPermanentCodeRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_permanent_code` |
| 凭据参数 | `suite_access_token`（Query 注入） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.TempAuthCode` | `auth_code` | `string` | 是 | 官方回调（授权通知）下发的临时授权码；属性名带 `Temp` 前缀以区分「永久授权码」 |

```json
{ "auth_code": "AUTH_CODE" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头 |
| `access_token` | `AccessToken` | `string?` | 官方当场下发的**企业级** `access_token`（≤512 字节）；**代开发自建应用安装时不返回** |
| `expires_in` | `ExpiresIn` | `int` | 上述企业令牌的有效期（秒）；代开发安装时不返回 |
| `permanent_code` | `PermanentCode` | `string?` | 企业永久授权码（≤512 字节），后续查授权信息/换取企业令牌的凭据 |
| `dealer_corp_info` | `DealerCorpInfo` | `DealerCorpInfo` | 代理服务商企业信息（应用被代理后才有） |
| `auth_corp_info` | `AuthCorpInfo` | `AuthCorpDetailInfo` | 授权方企业信息 |
| `auth_info` | `AuthInfo` | `AuthInfo` | 授权信息；通讯录应用未开启实体应用、以及「第三方会话存档接口」场景不返回 |
| `auth_user_info` | `AuthUserInfo` | `AuthUserInfo` | 授权管理员信息（`userid` / `open_userid` / `name` / `avatar`），可能不返回 |
| `register_code_info` | `RegisterCodeInfo` | `RegisterCodeInfo` | 扫推广二维码安装时返回（`register_code` / `template_id` / `state`） |
| `state` | `State` | `string?` | 扫码或授权链接中携带的 `state`（可区分安装渠道） |

**本端点注意事项**

- **`permanent_code` 与 `access_token` 都是凭据**：不得写入日志、遥测或异常消息。SDK 的企业令牌由 `CorpTokenManager` 以 `permanent_code` 自行换取（第三方走 `get_corp_token`、代开发走 `gettoken`），**不需要**把本响应里的 `access_token` 回填给 SDK。
- 官方回调必须在几秒内应答；v1 在回调里同步换取永久授权码容易超时，优先用 E5（官方推荐版本）。

---

### E4. `GetAuthInfoAsync` — 获取企业授权信息（v1）

**签名**

```csharp
Task<GetAuthInfoResponse> GetAuthInfoAsync(
    [Body] GetAuthInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_auth_info` |
| 凭据参数 | `suite_access_token`（Query 注入） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.AuthorizerCorpId` | `auth_corpid` | `string` | 是 | 授权方（企业）CorpId |
| `request.PermanentAuthCode` | `permanent_code` | `string` | 是 | E3/E5 得到的永久授权码 |

```json
{ "auth_corpid": "AUTH_CORPID", "permanent_code": "PERMANENT_CODE" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `dealer_corp_info` | `DealerCorpInfo` | `DealerCorpInfo` | 代理服务商企业信息（被代理后才有） |
| `auth_corp_info` | `AuthCorpInfo` | `AuthCorpDetailInfoExt` | 授权方企业信息（本端点用 `Ext` 变体，比 E3 多扩展字段） |
| `auth_info` | `AuthInfo` | `AuthInfo` | 授权信息（同上：通讯录应用/会话存档场景可能缺失） |

**本端点注意事项**

- 查询已授权企业的**当前**授权状态用本端点；`permanent_code` 失效或企业取消授权时官方报错——此时应清理本地授权记录，而不是重试。
- 企业**取消授权**后官方会推送 `cancel_auth` 事件，SDK 的 `IWechatAuthorizationCoordinator` 会按 `SuiteId` 命中集清理，无需业务代码主动调本端点轮询。

---

### E5. `GetPermanentCodeV2Async` — 获取企业永久授权码（v2）

- **方法与路由**：`POST` `/cgi-bin/service/v2/get_permanent_code`
- **签名与请求/响应**：与 E3 完全相同（`GetPermanentCodeRequest` → `GetPermanentCodeResponse`）
- **差异**：官方推荐版本，**耗时更短**，适合在授权回调中**同步**换取永久授权码（v1 易超时）

### E6. `GetAuthInfoV2Async` — 获取企业授权信息（v2）

- **方法与路由**：`POST` `/cgi-bin/service/v2/get_auth_info`
- **签名与请求/响应**：与 E4 完全相同（`GetAuthInfoRequest` → `GetAuthInfoResponse`）
- **差异**：官方推荐版本，**不返回插件关注二维码**、性能更好

> E5/E6 是独立路由而不是同一个方法的重载：`v2` 段在路径里，守卫 G6 对两条路由分别断言，不可合并。

---

### E7. `GetAppQrcodeAsync` — 获取应用二维码

**签名**

```csharp
Task<GetAppQrcodeResponse> GetAppQrcodeAsync(
    [Body] GetAppQrcodeRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_app_qrcode` |
| 凭据参数 | `suite_access_token`（Query 注入） |
| 适用应用类型 | 第三方应用（须为已上线的第三方通用应用） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.SuiteId` | `suite_id` | `string` | 是 | 第三方应用 id（`ww` / `wx` 开头的 suiteid） |
| `request.Appid` | `appid` | `long?` | 否 | 单应用不需要；多应用旧套件才需传（不传默认 1） |
| `request.State` | `state` | `string?` | 否 | 区分安装渠道；仅 `a-zA-Z0-9`，≤32 字节；扫带参码安装后 E3/E5 会带回该值 |
| `request.Style` | `style` | `long?` | 否 | `0`/`1` 带说明外框（实体物料 / 屏幕类），`2`/`3`/`4` 不带外框（小/中/大）；官方默认不带外框小尺寸 |
| `request.ResultType` | `result_type` | `long?` | 否 | **本 SDK 必须传 `2`**：`2` → JSON 返回 `qrcode` url；`1`（官方默认）→ `image/png` 二进制流，JSON 反序列化必然失败 |

```json
{ "suite_id": "SUITE_ID", "state": "channel01", "style": 2, "result_type": 2 }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `qrcode` | `Qrcode` | `string?` | 二维码图片 **url**（官方字段名就是 `qrcode`，不是 `qr_code`） |

**本端点注意事项**

- 忘了传 `result_type = 2` 是本端点最常见的失败形态：HTTP 200，但响应体是 PNG 字节流，反序列化抛异常/得到空对象——**不是**权限问题。
- `state` 只允许 `a-zA-Z0-9` 且 ≤32 字节；它与代开发带参授权链接 `state` 的规则**不同**（后者 ≤32 字节但同样限字符集，安装链接的 `state` 才是 ≤128 字节），不要互相套用。

**调用示例**

```csharp
using Mud.Wechat.Work;                      // 可注入接口 + AddWechatWorkServices / AddAuthenticationApi
using Mud.Wechat.Work.Abstractions;         // AddWechatApp（扩展方法落此命名空间，易漏）
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatAppType
using Mud.Wechat.Work.DataModels.ProviderAuthentication; // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，Microsoft.Extensions.DependencyInjection（GetRequiredService）与
// Microsoft.Extensions.Logging（logger 扩展方法）由 Web SDK 隐式 using 提供，无需显式引入。

// ① 注册第三方套件应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
//    Validate() 对 ThirdParty / Provider 要求四项齐备（CorpId + ProviderSecret + SuiteId + SuiteSecret）。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块（套件令牌基础设施随 AddWechatApp 装配）。
builder.Services.AddWechatWorkServices(b => b.AddAuthenticationApi());

// ③ 解析接口并走授权主流程（套件令牌来自回调推送的 suite_ticket，见 §2.1）。
var auth = provider.GetRequiredService<IWechatWorkProviderAuthenticationService>();

// 3a) 取预授权码
var preAuth = await auth.GetPreAuthCodeAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(preAuth, "/cgi-bin/service/get_pre_auth_code");

// 3b) 可选：限定本次授权页允许安装的应用 / 授权类型
await auth.SetSessionInfoAsync(new SetSessionInfoRequest
{
    PreAuthCode = preAuth.PreAuthCode,
    Session = new SessionInfo { AppIdList = new List<int> { 1 }, AuthType = 1 },
}, cancellationToken);

// 3c) 管理员在官方授权页确认后，官方回调把 auth_code 送到服务商后台。
//     这里以业务变量承载，真实值不得出现在日志中。
string tempAuthCode = "<官方回调下发的 auth_code>";

// 3d) 用临时授权码换永久授权码（官方推荐 v2：耗时更短，适合回调内同步完成）
var permanent = await auth.GetPermanentCodeV2Async(
    new GetPermanentCodeRequest { TempAuthCode = tempAuthCode },
    cancellationToken);
WechatWorkException.ThrowIfFailed(permanent, "/cgi-bin/service/v2/get_permanent_code");

// ④ permanent_code / access_token 均为凭据：只落 IWechatCorpAuthStore（SDK 既有链路）或宿主密钥库，
//    不要打印、不要拼进异常消息。
logger.LogInformation("企业 {AuthCorpId} 授权完成，state={State}",
    permanent.AuthCorpInfo?.CorpId, permanent.State);

// ⑤ 后续查询授权状态用 E4/E6；查询失败说明授权已失效，应清理本地记录而不是重试。
var info = await auth.GetAuthInfoV2Async(
    new GetAuthInfoRequest
    {
        AuthorizerCorpId = permanent.AuthCorpInfo!.CorpId,
        PermanentAuthCode = permanent.PermanentCode!,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/service/v2/get_auth_info");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `get_permanent_code` vs `get_auth_info` | 前者**消费** `auth_code` 换永久授权码（一次授权会话只成功一次）；后者**查询**已有企业的授权状态（可重复调用） | 拿 `permanent_code` 当 `auth_code` 传进 E3，或反过来用 `auth_code` 查 E4，均报参数错误 |
| v1 与 v2 两套路由 | `v2` 段在路径内（`/cgi-bin/service/v2/...`），请求/响应 DTO 相同 | 合并成「带参数的同一方法」会破坏路由契约（G6 分条断言） |
| 本族 `suite_access_token` vs [ProviderLoginService.md](./ProviderLoginService.md) 的 `provider_access_token` | 前者是**套件**凭证（依赖 `suite_ticket`），后者是**服务商主体**凭证（`corpid + provider_secret`） | 令牌族别错配必然 42009 / 42011，且无法靠重试自愈 |
| `get_app_qrcode` 的 `result_type` | `1` 返回 PNG 二进制，`2` 返回 JSON url | 不传或传 1 → 反序列化失败，被误判为「无权限」 |
| `pre_auth_code` / `auth_code` / `permanent_code` | 三者生命周期完全不同：预授权码（授权页会话）→ 临时授权码（回调一次性）→ 永久授权码（长期凭据） | 混存或复用会导致授权失败、甚至把长期凭据写进日志 |
| `GetPermanentCodeResponse.AccessToken` | 官方当场下发的企业令牌，与本 SDK `CorpTokenManager` 自行换取的企业令牌是两条链 | 把它回填给 SDK 或长期缓存，会与 SDK 的缓存/持久层形成双源，密钥轮换时读到旧令牌 |

另外两条 SDK 侧行为约束：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险）。`suite_access_token` 在 `SensitiveUrlRedactor` 词表内，但 `pre_auth_code` / `auth_code` / `permanent_code` **不在**——它们出现在你自建的 URL 或日志里时不会被擦除。
- **不自动判错**：声明式客户端只保证 HTTP 传输成功；失败响应的 `errcode != 0` 仍以 HTTP 200 返回，需自行判 `IsSuccess` 或调 `WechatWorkException.ThrowIfFailed(response, 路由)`。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效/过期 | 本族族别失效码，走恢复链路自愈 |
| 61028 | `InvalidSuiteTicket` | `suite_ticket` 无效 | **不自愈**：说明回调未接通或推送已过期，`suite_access_token` 刷新必然失败 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 企业级 `access_token` 族失效 | 本族不消费企业级 `access_token`；出现即凭据族错配的信号 |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族不消费该令牌；出现即调错了族（见 §5） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷 |

令牌失效自愈集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`。命中后链路为「失效缓存 → 强制刷新 → 重注入 → 重试」。

**若 42009 稳定复现，先查 `suite_ticket`**：`IWechatSuiteTicketProvider` 在未入库时直接抛出（提示配置回调），而不是拿旧票重试。61028 同理。这类问题加重试无效。

本族各端点的**业务错误码**（如无效 `auth_code`、企业未授权等）本仓接口注释未逐条留档，见 §7；通用码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 本族端点级频率限制 | 接口 XML 注释**未记录**任何 QPS / 次数数字 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| `pre_auth_code` 的实际有效期 | 只留档「`expires_in`（秒）」字段；本文示例中的 `1200` 为占位，**不是**官方断言 | 官方 90601 页面正文（本批未取文） |
| 各端点业务错误码 | 无效 `auth_code`、企业未授权、`permanent_code` 失效等官方码未在接口注释留档 ⇒ **未逐项核验** | 官方全局错误码（90455）+ 各端点页 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文**；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |
| `register_code_info` / `dealer_corp_info` 的完整字段表 | 本文只列顶层语义，不递归展开嵌套类型 | [GetPermanentCodeResponse.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/ResponseModel/GetPermanentCodeResponse.cs) 及同目录类型 |
| 仓内 `guide/` 与 `appendix/` | 「配置与凭据」「错误码」两篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（9 端点速览 + 令牌矩阵 + 逐端点核验）：[Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md)
- 兄弟族：[ProviderAuthenticationUrl.md](./ProviderAuthenticationUrl.md)（代开发带参授权链接，显式传 `provider_access_token`）· [ProviderLoginService.md](./ProviderLoginService.md)（扫码登录换用户信息）
- 授权编排与回调自动化的 SDK 侧实现：`IWechatWorkAuthorizationService` / `IWechatAuthorizationCoordinator`（见 `AGENTS.md` §5.4）
- 官方页：[套件授权总览（90597）](https://developer.work.weixin.qq.com/document/path/90597) · [获取预授权码（90601）](https://developer.work.weixin.qq.com/document/path/90601) · [获取企业永久授权码 v2（100776）](https://developer.work.weixin.qq.com/document/path/100776) · [获取应用二维码（95430）](https://developer.work.weixin.qq.com/document/path/95430)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
