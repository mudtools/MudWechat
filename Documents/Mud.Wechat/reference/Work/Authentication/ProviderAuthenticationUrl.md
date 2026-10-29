# 代开发自建应用的带参授权链接（显式传 provider_access_token）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Authentication` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/Authentication/IWechatWorkProviderAuthenticationUrl.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/IWechatWorkProviderAuthenticationUrl.cs) |
| DTO 源文件 | [GetCustomizedAuthUrlRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/RequestModel/GetCustomizedAuthUrlRequest.cs) · [GetCustomizedAuthUrlResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ProviderAuthentication/ResponseModel/GetCustomizedAuthUrlResponse.cs) |
| 契约守卫 | [WechatContractGuards.Authorization.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.Authorization.cs)（G6：路由 + 无 `[Token]` + 首参为 `[Query("provider_access_token")]`）· [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5：**不**在白名单内） |
| 官方文档 | 98744 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段，接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：G5 白名单（本接口不在内）/ G6 路由与令牌形态断言 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

为**代开发自建应用**生成一枚**带参安装授权链接**（`qrcode_url`）：服务商把链接渲染成二维码，企业管理员扫码后进入官方授权页，勾选模板并完成安装。链接里带 `state`，可用来区分渠道/租户；授权完成后该 `state` 会出现在后续换取永久授权码的响应里（见 [ProviderAuthentication.md](./ProviderAuthentication.md) 的 E3/E5）。

它与其他两个族的分工：`get_customized_auth_url` 只负责**生成链接**（代开发自建应用安装），授权流程本身仍走 [ProviderAuthentication.md](./ProviderAuthentication.md)；管理员扫码**登录**服务商网站换用户信息走 [ProviderLoginService.md](./ProviderLoginService.md)。

**为什么它单列一族**：本接口是**显式传令牌**形态——请求上不带 `[Token]`，`provider_access_token` 作为普通 Query 参数由调用方传入（守卫 G6 对「无 `[Token]` + 首参 `[Query("provider_access_token")]`」分条断言，G5 白名单里没有它）。因此它不能像本域其他接口那样「解析接口就发请求」，必须先切应用作用域再取令牌。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Provider`（服务商代开发） | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret`（四项齐备，缺一启动即抛） | 同上 |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken`（`Wechat.ProviderAccessToken`）→ 官方参数 `provider_access_token` | — |
| 令牌注入 | **无自动注入**：本接口不带 `[Token]`，令牌走方法首参 `[Query("provider_access_token")]` | 每次调用由调用方传入 |
| 令牌归属键 | 无（服务商主体级凭据，一个服务商全部分身共享） | — |

`provider_access_token` 由 `ProviderTokenManager` 以 `CorpId + ProviderSecret` 换取、缓存并按 `expires_in` 刷新。**推荐取法**：先 `IWechatAppContextSwitcher.UseAppScope(appKey)` 进入目标应用作用域，再经该应用上下文取令牌（示例用 `switcher.GetTokenAsync(WechatTokenTypes.ProviderAccessToken, ct)`；底层即该应用的 `IWechatProviderTokenManager`）。

> 代开发的 `template_id` 就是 `suite_id`，因此 `WechatAppConfig` **不提供独立的 `TemplateId`** 属性——不要去找它，也不要在配置里另建一份。

### 2.2 权限与业务前置

- 服务商须具备代开发能力，且目标企业管理员完成扫码安装（授权动作在官方页面，SDK 不承载）。
- `templateid_list` 里传的是**代开发模板 id**（= 该模板对应的 `suite_id`），官方限制最多 9 个。
- `state` 仅允许 `a-zA-Z0-9`、最长 32 字节；授权成功后原样回传。

### 2.3 官方开放的应用类型

官方仅对**服务商代开发**开放本端点。SDK 不设应用类型子接口——它是独立端点接口（`IWechatWorkProviderAuthenticationUrl`），自身可直接注入，不继承任何父接口。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkProviderAuthenticationUrl` | `Mud.Wechat.Work` | 独立端点接口（显式 Query 令牌，无 `[Token]`） | 1（E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`），可被 per-app `WechatAppConfig.BaseUrl` 覆盖
- 模块注册：`WechatModule.Authentication` / `AddAuthenticationApi()`；HTTP 客户端复用 `AddAuthenticationWebApiHttpClient()`（`RegistryGroupName = "Authentication"`），**不新增独立注册项**
- 注册组名：`Authentication`
- 端点索引：**E1** `GetCustomizedAuthUrlAsync`（生成代开发自建应用的带参授权链接）

> 接口上声明的是 `[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), ...)]` 而非 `TokenManage = ...`：没有令牌管理器参与，客户端只负责发请求。

---

## 4. 端点

### E1. `GetCustomizedAuthUrlAsync` — 获取代开发自建应用的带参授权链接

**签名**

```csharp
Task<GetCustomizedAuthUrlResponse?> GetCustomizedAuthUrlAsync(
    [Query("provider_access_token")] string providerAccessToken,
    [Body] GetCustomizedAuthUrlRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_customized_auth_url` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_customized_auth_url?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（**方法首参**，`[Query]` 显式传入；无 `[Token]` 自动注入） |
| 适用应用类型 | 服务商代开发 |
| 请求体编码 | `application/json`，源生成序列化（`ProviderAuthentication` 序列化组） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `providerAccessToken` | `provider_access_token`（Query） | `string` | 是 | 服务商访问令牌，由调用方显式传入（见 §2.1 的推荐取法） |
| `request.State` | `state` | `string?` | 否 | 授权方标识，仅 `a-zA-Z0-9`、最长 32 字节，授权成功后原样回传 |
| `request.TemplateIdList` | `templateid_list` | `List<string>` | 是 | 代开发模板 id 列表（= 模板对应 `suite_id`），**最多 9 个** |

```json
{
  "state": "channel01",
  "templateid_list": ["SUITE_ID_1", "SUITE_ID_2"]
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头 |
| `qrcode_url` | `QrcodeUrl` | `string?` | 带参授权二维码链接，宿主自行渲染为二维码图片 |
| `expires_in` | `ExpiresIn` | `int` | 链接/二维码有效期（秒），官方默认 864000 秒（10 天） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "qrcode_url": "https://open.work.weixin.qq.com/...",
  "expires_in": 864000
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                        // 可注入接口 + AddWechatWorkServices / AddAuthenticationApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp / WechatTokenTypes
using Mud.Wechat.Work.Abstractions.Authentication; // IWechatAppContextSwitcher（UseAppScope / GetTokenAsync）
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;     // WechatAppType
using Mud.Wechat.Work.DataModels.ProviderAuthentication; // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，GetRequiredService 与 logger 扩展方法由 Web SDK 隐式 using 提供。

// ① 注册代开发应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.Provider;
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块
builder.Services.AddWechatWorkServices(b => b.AddAuthenticationApi());

// ③ 先切应用作用域，再取该应用的 provider_access_token（接口无 [Token]，必须显式传）
var urlApi = provider.GetRequiredService<IWechatWorkProviderAuthenticationUrl>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();

string providerToken;
using (switcher.UseAppScope("provider-a"))
{
    providerToken = await switcher.GetTokenAsync(
        WechatTokenTypes.ProviderAccessToken, cancellationToken);
}

// ④ 生成带参授权链接。qrcode_url 交给前端渲染二维码，服务端不需要保存图片。
var response = await urlApi.GetCustomizedAuthUrlAsync(
    providerToken,
    new GetCustomizedAuthUrlRequest
    {
        State = "channel01",                       // 仅 a-zA-Z0-9，≤32 字节
        TemplateIdList = new List<string> { "<代开发模板 id（= suite_id）>" }, // 最多 9 个
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(response, "/cgi-bin/service/get_customized_auth_url");

// ⑤ 响应类型可空（HTTP 204 / 异常报文下可能为 null），取值前显式判空。
if (response is null || string.IsNullOrEmpty(response.QrcodeUrl))
{
    throw new InvalidOperationException("带参授权链接响应为空，无法渲染二维码。");
}

logger.LogInformation("带参授权链接已生成，state={State}，有效期 {ExpiresIn}s",
    "channel01", response.ExpiresIn);
```

**本端点注意事项**

- **令牌不会被自动注入**：漏传 `providerAccessToken`（或传空串）时请求照样发出，官方报缺少凭据——这不是 DI 装配问题。
- 返回类型是 `GetCustomizedAuthUrlResponse?`：可空是刻意的，判空前不要直接 `.QrcodeUrl`。
- `qrcode_url` 本身可被任意人扫码安装**该模板**：它等价于一枚限时邀请凭证，不要公开张贴到公网页面，也不要把完整链接打日志（`SensitiveUrlRedactor` 只处理令牌参数，不覆盖业务链接）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| [ProviderAuthentication.md](./ProviderAuthentication.md) 的 `get_app_qrcode` | 那支是**第三方通用应用**的安装二维码，令牌 `suite_access_token` 自动注入、`result_type` 决定 JSON/PNG；本支是**代开发自建应用**的带参链接，令牌显式传入、只返回 url | 两族都叫「二维码」但凭证与适用形态不同，混用会 42009/42011 或拿错应用 |
| 本族显式 `provider_access_token` vs ProviderLogin 自动注入 | 同是服务商令牌，本接口用方法首参传入、G5 白名单不含本接口 | 误以为「反正都是 provider token」而不切应用作用域，多应用部署时会取到默认应用的令牌 |
| `state` 规则三种 | 本端点 `state` ≤32 字节且仅 `[a-zA-Z0-9]`；安装链接 `state` ≤128 字节；登录 `state` 由宿主自定义 | 把 ≤128 的规则套到本端点 → 官方拒绝 |
| `template_id` 的来源 | 代开发 `template_id` **就是** `suite_id`，配置里没有独立 `TemplateId` 属性 | 在配置里找 `TemplateId` 会落空；手工另存一份必然与 `SuiteId` 漂移 |
| `qrcode_url` 的语义 | 生成即是一条**可用**的安装入口，不是「占位图片地址」 | 当作普通资源 URL 缓存/公开，等于把安装入口泄漏给无关方 |

SDK 侧两条行为约束：

- **不自动判错**：`errcode != 0` 时 HTTP 仍为 200，响应照样反序列化；必须自行判 `IsSuccess` 或调 `WechatWorkException.ThrowIfFailed`。
- **令牌族别不要靠猜**：本接口只认 `provider_access_token`；传 `suite_access_token` 或企业 `access_token` 都会失败，且失败码可能落在「令牌失效」区间而触发无意义的重试。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效/过期 | 本端点族别失效码，走恢复链路自愈（刷新后重注入） |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本端点不消费该令牌；出现即把套件令牌当服务商令牌传了 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 企业级 `access_token` 族失效 | 同上，属凭据族错配信号 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 首参传空/漏传；不自愈 |

若 42011 稳定复现，说明**刷新本身在失败**：排查 `ProviderSecret` 是否正确、服务商企业是否已开通代开发能力、官方是否对 `get_provider_token` 做了频率拦截（官方对该换取接口明确警示「不能频繁调用」）。加重试不解决这三类成因。

本端点专属业务码（模板不存在、`state` 非法等）本仓未逐条留档，见 §7；通用码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 端点级频率限制 | 接口 XML 注释**未记录** QPS / 次数数字 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 业务错误码 | 模板不存在/未上线、`state` 非法、超出 9 个模板等官方码未留档 ⇒ **未逐项核验** | 官方 98744 页面 + 全局错误码（90455） |
| `expires_in` 的官方表述 | DTO 注释留档「默认 864000 秒（10 天）」；是否随模板/渠道变化 ⇒ **未逐项核验** | 官方 98744 页面 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文**；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |
| 仓内 `guide/` | 「配置与凭据」未成文，`UseAppScope` 的多应用细则无集中文档 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Authentication/README.md)
- 兄弟族：[ProviderAuthentication.md](./ProviderAuthentication.md)（套件授权流，`suite_access_token`）· [ProviderLoginService.md](./ProviderLoginService.md)（扫码登录换用户信息）
- 同一「显式 Query 传令牌、不带 `[Token]`」形态的对照：`IWechatWorkCorpTokenAuthentication`（令牌签发面）、通讯录迁移族（`PromotionQrCode`）
- 官方页：[获取代开发自建应用的带参授权链接（98744）](https://developer.work.weixin.qq.com/document/path/98744) · [获取服务商凭证（91200）](https://developer.work.weixin.qq.com/document/path/91200)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
