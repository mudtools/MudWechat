# 基础接口（接口 IP 段 / 回调 IP 段）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Basic` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkBasicService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Basic/IWechatWorkBasicService.cs) · [IWechatWorkInternalBasicService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Basic/IWechatWorkBasicService_Internal.cs) · [IWechatWorkProviderBasicService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Basic/IWechatWorkBasicService_Provider.cs) |
| DTO 源文件 | [GetApiDomainIpResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Basic/GetApiDomainIpResponse.cs) · [GetCallbackIpResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Basic/GetCallbackIpResponse.cs) |
| 契约守卫 | [WechatBasicContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBasicContractGuards.cs)（BS1 路由 / BS2 层级 / BS3 令牌绑定 / BS4 JSON 上下文 / BS5 无请求体与 `ip_list` 契约）· [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5 白名单） |
| 官方文档 | 自建 92520 / 92521 · 服务商代开发 97073 / 98988 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 首验 + 2026-10-30 复验：在本仓临时 Web SDK 工程（`net8.0`）与全仓 82 段可运行示例合并逐字编译，0 错误（本篇 2 段；本篇的 `// 承接` 片段示例已补齐自足前置（自带 using 与服务实例解析），现同样参与编译；接口签名摘录块与 json 块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：BS1~BS5 与 G5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Basic/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

给**企业侧防火墙**配两条放行规则所需的 IP 段，是纯基础设施端点，不承载任何业务数据：

- **接口 IP 段**（`get_api_domain_ip`）：你的服务器要访问 `qyapi.weixin.qq.com` 时，该域名的解析 IP 段。
- **回调 IP 段**（`getcallbackip`）：企业微信向你提供的回调 URL 推送事件时的**来源 IP** 段。

典型用途是把返回值灌进出网白名单 / 入站防火墙，并**每天定时拉取一次**——官方说明 IP 段有变更可能，变更期间新旧段同时保留一段时间，持续拉取可避免规则过期导致网络不通。

本域没有「业务接口」含义上的权限与可见范围：官方权限说明是「无限定」。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（自建）/ `WechatAppType.Provider`（服务商代开发） | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（`Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入，**不在方法签名里** | 每次请求 |
| 令牌归属键 | 自建子接口 `Wechat.AccessToken@Internal`；代开发子接口 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

自建应用取的是**应用自身** `access_token`（`CorpId + AgentSecret` 换取）；服务商代开发取的是**授权企业级** `access_token`（`scope = authCorpId`，`permanent_code` 当应用 secret 用），调用前必须先建立企业作用域：

```csharp
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var ips = await basicService.GetCallbackIpAsync(ct);
}
```

> **自 2023-12-01 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）——不要拿「通讯录同步助手」那类系统应用的 secret 当业务应用凭据。

### 2.2 权限与业务前置

- 官方权限说明：**无限定**，不要求应用配置到任何白名单/可见范围。
- 业务约束：IP 段会变更，官方建议**每天定时拉取**；两端点**没有独立的数值型频率限制**，走官方全局访问频率限制。
- 无任何业务参数——令牌是唯一参数。

### 2.3 官方开放的应用类型

官方对**企业自建应用**与**服务商代开发**开放完全一致的 2 个端点（同路由同契约）；**第三方应用文档树没有「基础接口」分组**，故 SDK **不设第三方子接口**（BS2 锁定继承链恰为两个空标记子接口）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkBasicService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载全部 2 个端点 | 2（E1/E2） | 否 |
| `IWechatWorkInternalBasicService` | `Mud.Wechat.Work` | 自建应用子接口（空标记，归属键 `InternalAccessToken`） | 0（继承 E1/E2） | 是 |
| `IWechatWorkProviderBasicService` | `Mud.Wechat.Work` | 服务商代开发子接口（空标记，归属键 `CorpAccessToken`） | 0（继承 E1/E2） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`），可被 per-app `WechatAppConfig.BaseUrl` 覆盖
- 模块注册：`WechatModule.Basic` / `AddBasicApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Basic`
- 端点索引：**E1** `GetApiDomainIpAsync`（接口 IP 段）· **E2** `GetCallbackIpAsync`（回调 IP 段）

最终用户注入**子接口**（`IWechatWorkInternalBasicService` / `IWechatWorkProviderBasicService`），不要直接消费父接口（父接口不注册 DI，`RegistryGroupName` 恒空）。

---

## 4. 端点

### E1. `GetApiDomainIpAsync` — 获取企业微信接口IP段

**签名**

```csharp
Task<GetApiDomainIpResponse> GetApiDomainIpAsync(CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/get_api_domain_ip` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/get_api_domain_ip?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 企业自建应用 · 服务商代开发 |
| 请求体 | **无**（官方即 GET、无 body；BS5 锁定签名只剩取消令牌） |

**请求参数**

无业务参数。`access_token` 由 `[Token]` 注入，不进方法签名。

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头（继承 `WechatWorkResponse`） |
| `ip_list` | `IpList` | `List<string>`（**非空**，默认 `new()`） | 企业微信服务器 IP 段 / 地址列表，CIDR 形式字符串 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "ip_list": ["1.2.3.4", "5.6.7.0/24"]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddBasicApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;     // WechatAppType
using Mud.Wechat.Work.DataModels.Basic;       // 响应 DTO

// 宿主为 ASP.NET Core 时，GetRequiredService 与 logger 扩展方法由 Web SDK 隐式 using 提供。

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配）
builder.Services.AddWechatWorkServices(b => b.AddBasicApi());

// ③ 注入自建子接口；服务商代开发改注入 IWechatWorkProviderBasicService（先 UseCorpScope）
var basic = provider.GetRequiredService<IWechatWorkInternalBasicService>();

// ④ 接口 IP 段：用于出网白名单（qyapi.weixin.qq.com 的解析地址）
var domainIps = await basic.GetApiDomainIpAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(domainIps, "/cgi-bin/get_api_domain_ip");

// ip_list 官方为 StringArray、失败示例返回空数组而非缺省，故 IpList 恒非 null；
// 仍建议按「非空数组」判断是否真的取到了段（失败时是空集合 + errcode != 0）。
if (domainIps.IpList.Count == 0)
{
    logger.LogWarning("接口 IP 段为空，errcode={ErrorCode}（其余情况下请检查白名单是否已过期）",
        domainIps.ErrorCode);
}
else
{
    foreach (var ip in domainIps.IpList)
    {
        logger.LogInformation("接口 IP 段：{Ip}", ip);
    }
}
```

**本端点注意事项**

- **官方即 GET、无请求体**：不要为「统一风格」改成 POST 或加请求 DTO（BS5 直接断言签名里除取消令牌外无其它参数）。
- **路由尾段是多段下划线** `get_api_domain_ip`；与本族另一支 `getcallbackip`（无下划线）形态不同，照抄原文即可，守卫 BS1 分条断言，不要互相「对齐」。
- **失败时仍返回 `ip_list`（空数组）**：官方失败示例为「空数组 + 42001」，所以 `IpList` 用非空集合承载——但空集合**不代表成功**，要判 `errcode`。
- 返回值是**当下的**解析结果，不是永久清单；请按官方建议每天定时拉取并更新防火墙。

### E2. `GetCallbackIpAsync` — 获取企业微信回调IP段

**签名**

```csharp
Task<GetCallbackIpResponse> GetCallbackIpAsync(CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/getcallbackip` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/getcallbackip?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 企业自建应用 · 服务商代开发 |
| 请求体 | **无** |

**请求参数**

无业务参数。

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头 |
| `ip_list` | `IpList` | `List<string>`（**非空**，默认 `new()`） | 企业微信回调时的**来源 IP** 段 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "ip_list": ["1.2.3.4", "5.6.7.0/24"]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException
// 片段自足前置：解析 E1 里用到的同一个服务实例。
var basic = provider.GetRequiredService<IWechatWorkInternalBasicService>();


var callbackIps = await basic.GetCallbackIpAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(callbackIps, "/cgi-bin/getcallbackip");

// 回调 IP 段用于「入站白名单」：企业微信回调你的 URL 时从这些地址发出。
logger.LogInformation("回调 IP 段共 {Count} 条", callbackIps.IpList.Count);
```

**本端点注意事项**

- **路由尾段无下划线**：`/cgi-bin/getcallbackip`（与 E1 的 `get_api_domain_ip` 形态不同，非笔误）。
- 官方文档页正文另附一份「目前回调 IP 列表」的**静态清单**，并明确标注仅供参考、最新列表以本接口返回为准；SDK **不建模该静态清单**（必然漂移），唯一事实来源是本端点返回值。
- 这正是**回调中间件**校验的补充手段，但注意：回调接收侧的抗重放/校验逻辑（签名、时间戳、指纹）与 IP 白名单是两回事——IP 白名单只是网络层收敛，不能替代回调验签。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `get_api_domain_ip` vs `getcallbackip` | 前者是**你出网**要访问的 API 域名 IP；后者是**回调来源** IP | 把两者灌反（出网白名单填回调段、入站白名单填接口段）→ 两边都不通 |
| 接口 IP vs 回调 IP 的性质 | 接口 IP 是 `qyapi.weixin.qq.com` 的解析地址；回调 IP 是官方推送源地址 | 只配一侧会导致「能调接口但收不到回调」或反之 |
| 自建子接口 vs 代开发子接口 | 同为 2 端点、同路由同契约，**只差令牌归属键**（`InternalAccessToken` / `CorpAccessToken`） | 代开发场景注入自建子接口 → 取不到企业令牌，运行期归属域错配抛 `WechatTokenOwnerMismatchException` |
| 本域 vs 第三方应用 | 官方第三方文档树**无**「基础接口」分组，SDK 无第三方子接口 | 在第三方应用里找本族接口会落空 |
| `ip_list` 非空集合 | 官方失败示例返回**空数组**而非缺省字段 | 以「`IpList == null` 判断失败」永远不成立；要判 `errcode` |
| 静态 IP 清单（官方页正文） | 官方标注「仅供参考」，以本接口返回为准 | 抄死静态清单进配置 → 官方调整后白名单过期 |

SDK 侧两条行为约束：

- **不自动判错**：`errcode != 0` 时 HTTP 仍 200、响应照样反序列化；必须 `IsSuccess` 或 `ThrowIfFailed`。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，G5 白名单锁定）。`access_token` 在 `SensitiveUrlRedactor` 词表内，但自己打日志时不要把完整 URL 打出去。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 42001 | `ExpiredAccessToken` | `access_token` 过期 | 官方在本族**失败示例**明确用它（同时 `ip_list` 返回空数组）；命中后走自愈链路 |
| 40014 / 42007 | `InvalidAccessToken` / `RelatedAccessTokenInvalid` | 令牌失效 / 相关令牌失效 | 同属自愈集合 |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 套件 / 服务商令牌失效 | 本域不消费这两种令牌；出现即凭据族别错配 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷 |

自愈集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`。本域无专属业务错误码，其余码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 端点级频率限制 | 接口注释只写「无独立数值型限制，走全局」，未记具体 QPS ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| IP 段变更频率 / 过渡期长度 | 注释只写「新旧段同时保留一段时间」，无具体时长 ⇒ **未逐项核验** | 官方 92520/92521 页面 |
| 官方文档正文 | 本文只摘录本仓 XML 注释；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |
| 仓内 `guide/` | 「配置与凭据」未成文，多应用/企业作用域的集中说明缺失 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Basic/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Basic/README.md)
- 回调接收面的 IP / 验签分工：`Mud.Wechat.Work.Callback`（本仓 Callback 包，回调抗重放与验签在中间件内）
- 官方页：[获取企业微信接口IP段（自建 92520）](https://developer.work.weixin.qq.com/document/path/92520) · [获取企业微信回调IP段（自建 92521）](https://developer.work.weixin.qq.com/document/path/92521) · [获取企业微信接口IP段（代开发 97073）](https://developer.work.weixin.qq.com/document/path/97073) · [获取企业微信回调IP段（代开发 98988）](https://developer.work.weixin.qq.com/document/path/98988)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
