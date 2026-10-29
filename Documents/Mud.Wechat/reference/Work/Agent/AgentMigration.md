# 自建应用迁移成代开发应用（claim_customized_app）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Agent` 域 · 自建应用迁移成代开发应用族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkAgentMigrationService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentMigrationService.cs)（零端点父接口）· [IWechatWorkAgentMigrationService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentMigrationService_Internal.cs)（唯一自建子接口，承载 E1） |
| DTO 源文件 | [ClaimAgentCustomizedAppRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/ClaimAgentCustomizedAppRequest.cs)（请求体，全族仅此 1 型）；响应复用 [WechatWorkResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/WechatWorkResponse.cs)（`errcode` / `errmsg`） |
| 契约守卫 | [WechatAgentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs)（AG1 路由与返回 `WechatWorkResponse` / AG2 父零端点 + 唯一自建子接口 / AG3 令牌绑定 / AG4 DTO 上下文登记 / **AG5 双令牌契约**）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 服务商代开发 99617（文档号取自子接口 `<remarks>`）· **本次未重取官方页**，页面最后更新日期与正文口径见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 2 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AG1 路由 `POST /cgi-bin/agent/claim_customized_app` + 返回类型断言 / AG2 继承链（父 `DeclaredOnly` = 0、子接口集合恰 `IWechatWorkInternalAgentMigrationService`、子 1 端点）/ AG3 两接口 `Wechat.AccessToken` + Query + `access_token` / AG4 本型登记进 `AgentJsonContext` / AG5 `suite_access_token` 字段名 + 请求体恰一个公开属性 + TO1~TO3 + G5 白名单含两接口 + 两份接口与一份 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承接口 XML 注释与 2026-10-18 核验留档；凡只有官方页能支撑的项（频率、专属错误码、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

本族只有**一个**端点：把企业侧的**历史遗留自建应用**迁移成服务商代开发应用。官方把这一步称为「完成应用归属校验」——执行后该应用的归属从「企业自建」转为「服务商代开发模式」，属于组织级的归属变更，不是应用配置的修改。

使用时机有两类：服务商接手企业的存量自建应用、需要把归属转到自己名下时；以及在服务商管理端提交迁移后，**仍需重置该应用 `secret` 或提交上线审核之前**——官方明确这两个后续动作都要求先完成本接口的归属校验。

一次调用需要两种凭据同时就位：待迁移自建应用自身的 `access_token`（Query 参数，SDK 自动注入）与代开发应用模板的 `suite_access_token`（**请求体字段**，宿主显式传入）。这两者的分工正是本族全部复杂度所在（§2.3、§5）。

**不该用本族的场景**：改应用名称、头像、可见范围、可信域名走同域「设置应用」（[Agent.md](./Agent.md) E3）；只是读应用详情或列表走 E1/E2；新建一个代开发应用走服务商管理端与授权流程——本族只处理「已经存在的自建应用换归属」这一件事。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`——**待迁移的那家企业的自建应用**，不是你的服务商应用 | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()`：`Internal` 分支要求 `CorpId` + `AgentSecret`（基类另校验 AppKey 形状与 BaseUrl），配置错误启动即失败 |
| 必填配置属性 | `CorpId`（应用所属企业）+ `AgentSecret`（该自建应用自身的 `corpsecret`） | 同上；缺任一项启动即失败（`自建应用 {AppKey} 缺少 AgentSecret。`），不留到首次调用 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | `WechatTokenManagerKeys.InternalAccessToken`（`Wechat.AccessToken@Internal`） | 唯一咽喉点 `WechatAppContext.GetTokenManager`；当前应用若是 `ThirdParty` / `Provider`，抛 `WechatTokenOwnerMismatchException`（fail-fast，请求不发） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入，**不进方法签名** | 每次请求 |
| 第二凭据 | `suite_access_token`：`ClaimAgentCustomizedAppRequest.SuiteAccessToken`，**请求体字段**，不经 SDK 令牌机制 | 调用方自备；SDK 不校验非空、不校验有效期、也不代取 |

`access_token` 由 SDK 用 `CorpId + AgentSecret` 自动换取、缓存并按官方有效期刷新，**开发者不接触令牌值**。

`suite_access_token` 则相反：**SDK 不替你取**。它是代开发应用模板的调用凭证，换取链是「`suite_ticket`（官方定时推送，仓内没有任何主动获取它的端点）→ `SuiteId + SuiteSecret + SuiteTicket` → 套件令牌」，落点在 [SuiteTokenManager.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/SuiteTokenManager.cs) 与 [WechatSuiteTicketProvider.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/WechatSuiteTicketProvider.cs)。于是有两种现实形态：

- 宿主已在同一进程内注册了套件类应用（`ThirdParty` / `Provider`）并接入了 `suite_ticket` 回调：套件令牌可在该**另一个**应用上下文里取到（`IWechatAppContext.SuiteTokenManager` 只对非 `Internal` 应用装配，装配分支见 [WechatAppManager.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/MultiApp/WechatAppManager.cs)），再交给本次调用；
- 套件令牌由独立的套件侧服务产出，经安全存储或配置交给本调用。

两种形态都不改变同一条约束：**本次调用的应用上下文必须是 `Internal`**，套件令牌只能作为字符串出现在请求体里。若宿主尚未持有 `suite_ticket`，本端点根本组不出请求体——这不是 `errcode`，而是你手里没有这个值；若改用 SDK 套件令牌链，缺票据会在换取阶段抛 `InvalidOperationException` 且请求不发出（该链路的细节见 [ChatIdUpgrade.md](../AccountId/ChatIdUpgrade.md) §2.2，本文不重抄）。

`suite_access_token` 是凭据：不得写进日志、遥测或异常消息，也不要把它拼进 URL。示例一律从 `config[...]` 读。

### 2.2 权限与业务前置

以下逐条出自子接口与请求 DTO 的 XML 注释（官方页本次未重取）：

- **官方权限口径**：「仅服务商可调用；`access_token` 为待迁移自建应用的接口调用凭证（URL 参数），`suite_access_token` 为代开发应用模板的调用凭证（包体参数）。」两条同时成立——调用方是服务商，而鉴权用的是**企业那支自建应用**的令牌。
- **只有历史遗留的自建应用可迁移**：官方背景是「企业微信禁止服务商以自建应用的方式为企业开发产品」，本接口是对存量应用的收口通道，不是新应用的建站入口。
- **管理端迁移 ≠ 完成**：官方原文「服务商也可在服务商管理端提交自建应用列表进行迁移，但管理端迁移后若需重置应用 `secret` 或提交上线审核，仍须先调用本接口完成应用归属校验」。
- **不可逆**：接口自述「应用归属转为服务商代开发模式」，本域 14 条路由里没有反向端点（AG1 锁定整张路由表），SDK 也不提供撤销入口。
- **落地门槛**：调用方须先持有该自建应用的 `corpsecret` 并以其注册应用上下文。服务商如何从企业侧合法取得这支 `secret`，接口注释未给流程 ⇒ §7。
- 频率：本端点的独立 QPS / 日配额注释未留档 ⇒ §7。

### 2.3 官方开放的应用类型

官方只在**服务商代开发**文档树（99617）提供本端点，自建应用与第三方应用文档树都没有对应页面。但 SDK 的接口形态是「零端点父接口 + 唯一**自建**子接口」，而不是常见的 `_Provider` 子接口。这个反直觉形状不是建模失误，推理链如下：

1. **页面上的参数表才是调用身份的判决书**：`access_token` 是「待迁移或尚未验证归属的**自建应用**」的接口调用凭证，`suite_access_token` 只是包体参数。⇒ 本次请求的鉴权主体是自建应用。
2. **SDK 的可注入面按令牌归属域切，不按文档树切**：`_Provider` / `_ThirdParty` 子接口声明的是 `WechatTokenManagerKeys.CorpAccessToken`（授权企业级令牌，需先 `UseCorpScope` 切到目标企业）；它们取不出「自建应用自身令牌」这种东西。把端点挂到 `_Provider` 上，用户拿到的会是授权企业令牌，官方侧认不出这是哪个应用的迁移请求。
3. **自建子接口的归属键恰是 `@Internal`**，与第 1 步的鉴权主体一一对应。所以端点只能落在 `_Internal` 上。
4. **父接口保持零端点**：本域父接口的语义是「三类应用开放面一致」的收敛面（AG2 对其余三族逐个断言端点数），而本端点既不在三类一致面上、也只对自建应用令牌有效，放上父接口即等于宣称第三方应用也能调。
5. 于是继承链上**不得**出现代开发/第三方子接口——`AssertFamily` 把子接口集合锁为恰 `IWechatWorkInternalAgentMigrationService`。想「补齐」一个 `_Provider` 子接口会当场打红 AG2。

一句话记法：**文档树位置讲的是「这篇文档写给谁看」，令牌归属讲的是「这次请求以谁的身份发」**；SDK 按后者建模。

第三方应用与代开发应用都解析不到本族接口：父接口不注册 DI，唯一子接口是 `_Internal`，继承链上没有它们的位置（AG2 的子接口集合断言）。本族的业务动作始终是「把自己的某支自建应用换归属」，不是「服务自己的代开发应用」。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAgentMigrationService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空、不声明归属键） | 0 | 否 |
| `IWechatWorkInternalAgentMigrationService` | `Mud.Wechat.Work` | 唯一自建子接口，继承父接口，承载 E1 | 1 | 是（`@Internal`） |

```
IWechatWorkAgentMigrationService          0 端点（仅作契约基座）
└─ IWechatWorkInternalAgentMigrationService   +1（E1 迁移成代开发应用）
```

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Agent` / `AddAgentApi()`（`AddAllApis()` 亦包含）——本族**没有**独立的模块枚举，与「获取应用 / 工作台 / 菜单」三族共用
- 注册组名：`Agent`（子接口 `RegistryGroupName`，生成器产出 `AddAgentWebApiHttpClient()`；父接口注册组恒空）
- 端点索引：**E1** `ClaimCustomizedAppAsync`（历史遗留自建应用 → 服务商代开发应用，归属校验）
- 生成物：父接口实现类 `WechatWorkAgentMigrationService` 落 `Mud.Wechat.Work.Interfaces.Internal`，子接口实现类落 `Mud.Wechat.Work.Internal`（AG2 断言子接口 `InheritedFrom = nameof(WechatWorkAgentMigrationService)`），均勿手改

最终用户注入**子接口**；父接口不注册 DI，解析不到。

---

## 4. 端点

### E1. `ClaimCustomizedAppAsync` — 把历史遗留自建应用迁移成代开发应用（归属校验）

**签名**

```csharp
Task<WechatWorkResponse> ClaimCustomizedAppAsync(
    [Body] ClaimAgentCustomizedAppRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/claim_customized_app` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/agent/claim_customized_app?access_token=ACCESS_TOKEN`（套件令牌只在包体，**不出现在 URL**） |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；待迁移自建应用自身令牌）+ `suite_access_token`（请求体字段，宿主显式传入） |
| 适用应用类型 | 官方页面只挂在服务商代开发文档树；**调用身份**为待迁移自建应用（`WechatAppType.Internal`） |
| 请求体编码 | `application/json`，源生成序列化（`AgentJsonContext`，AG4 断言本型已登记、`SerializerClassName = Agent`） |
| 返回类型 | 裸 `WechatWorkResponse`（只有 `errcode` / `errmsg`；AG1 断言本端点不得新建空响应 DTO） |
| 官方文档 | 99617（服务商代开发） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.SuiteAccessToken` | `suite_access_token` | `string`（非可空，默认 `string.Empty`） | 是 | 代开发应用模板接口调用凭证，官方**包体参数**；获取方式与第三方应用凭证相同（依赖 `suite_ticket` 推送）。套件级敏感凭据，不得进日志、遥测或异常消息 |
| —（不进方法签名） | `access_token` | — | 是 | 待迁移**自建应用自身**的接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

本请求体**只有这一个字段**（AG5 断言 `ClaimAgentCustomizedAppRequest` 的公开实例属性恰一个且名为 `SuiteAccessToken`）。「迁移哪个应用」不是参数——由 `access_token` 指向的应用决定。

SDK 不做参数校验：`SuiteAccessToken` 留空串也照样发出，错误以 `errcode` 回来。

```json
{
  "suite_access_token": "SUITE_ACCESS_TOKEN"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

响应**没有业务负载**，也不回传迁移后的应用 id 或归属状态——成功与否只能看 `errcode`。

**调用示例**

```csharp
using Mud.Wechat.Work;                          // IWechatWorkInternalAgentMigrationService / AddWechatWorkServices / AddAgentApi
using Mud.Wechat.Work.Abstractions;             // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions;  // WechatWorkException（判错出口）
using Mud.Wechat.Work.DataModels.Agent;         // ClaimAgentCustomizedAppRequest

// 宿主约定：builder / config / provider / logger / cancellationToken 由宿主提供
//（builder 为 IServiceCollection、config 为 IConfiguration、provider 为已 Build 的 IServiceProvider）；
// 本示例按 ASP.NET Core Web SDK 形态编写，Microsoft.Extensions.DependencyInjection 与
// Microsoft.Extensions.Logging 的 using 由其隐式提供。

// ① 注册的是「待迁移的那家企业的自建应用」，不是你的服务商应用。
//    它的 CorpId + AgentSecret 决定 Query 里的 access_token 是谁的 —— 也就是谁被迁走。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "legacy-internal-a";
    o.AppType = WechatAppType.Internal;                            // 归属域键 Wechat.AccessToken@Internal
    o.CorpId = config["Wechat:MigrationTargetCorpId"];
    o.AgentSecret = config["Wechat:MigrationTargetAgentSecret"];   // 该自建应用自身的 corpsecret
});

// ② 本族随 Agent 模块注册，没有独立的模块枚举。
builder.Services.AddWechatWorkServices(b => b.AddAgentApi());

// ③ 套件令牌由套件侧产出（suite_ticket 链），本篇只消费；它是凭据，只从配置读。
var suiteAccessToken = config["Wechat:SuiteAccessToken"];
if (string.IsNullOrEmpty(suiteAccessToken))
{
    // 套件侧还没换出令牌时，请求体根本组不出来；此处直接失败，不发起调用。
    throw new InvalidOperationException("缺少 Wechat:SuiteAccessToken，本次归属校验未发起。");
}

var migration = provider.GetRequiredService<IWechatWorkInternalAgentMigrationService>();

// ④ 双令牌各走各的通道：access_token 由 [Token] 注入 Query，suite_access_token 只在包体。
var result = await migration.ClaimCustomizedAppAsync(
    new ClaimAgentCustomizedAppRequest { SuiteAccessToken = suiteAccessToken },
    cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
//    本端点改变的是应用归属且不可逆 —— 失败后不要自动重发，先判断失败原因。
WechatWorkException.ThrowIfFailed(result, "/cgi-bin/agent/claim_customized_app");

// ⑥ 成功也只有 errcode/errmsg，没有可读的业务字段。
logger.LogInformation("应用归属校验已提交：errcode={ErrorCode}", result.ErrorCode);
```

宿主注册了**多个应用**时，`[Token]` 取的是当前应用上下文的令牌：先进入目标应用作用域再调用，否则被迁走的会是默认应用（片段）：

```csharp
using Mud.Wechat.Work;                              // IWechatWorkInternalAgentMigrationService
using Mud.Wechat.Work.Abstractions.Authentication;  // IWechatAppContextSwitcher / UseAppScope
using Mud.Wechat.Work.DataModels.Agent;             // ClaimAgentCustomizedAppRequest

// 前置：AddWechatApp 已注册 legacy-internal-a 以及其它应用；AddAgentApi() 已装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var migration = provider.GetRequiredService<IWechatWorkInternalAgentMigrationService>();
var request = new ClaimAgentCustomizedAppRequest { SuiteAccessToken = config["Wechat:SuiteAccessToken"]! };

using (switcher.UseAppScope("legacy-internal-a"))   // 释放时自动归还上下文，嵌套安全
{
    var response = await migration.ClaimCustomizedAppAsync(request, cancellationToken);
}
```

**本端点注意事项**

- **没有 `agentid` 参数**：目标应用由 `access_token` 隐含。想迁 A 应用却用了 B 应用的 `AppKey`，官方不会报「参数不匹配」，而是把 B 迁走——且不可逆。多应用宿主务必核对作用域。
- 官方权限是「仅服务商可调用」，但令牌是企业那支自建应用的。若你手上没有该应用的 `corpsecret`，调用会在**换取 `access_token`** 那一步失败（不是本端点的 `errcode`，请求根本没发到本端点）。
- 成功响应无业务字段：官方是否要求/建议在迁移后回读应用状态，注释未留档（§7）；不要拿本端点的 `errcode == 0` 推断「应用已可上线」。
- 除令牌失效恢复链外，SDK 不做任何自动重试；而归属变更不可逆，调用方也不要在外层再包一层「失败重发」（重复调用的官方语义未留档，§7）。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 「官方只在代开发树有这页」⇄「应用上下文须为自建应用」 | 页面归属是文档编排，鉴权身份由参数表决定：`access_token` 是待迁移**自建应用**的（父接口与子接口 XML 双重留档） | 用 `Provider` / `ThirdParty` 应用上下文调本族：`@Internal` 归属域校验失败，抛 `WechatTokenOwnerMismatchException`，请求根本不发；误以为「加个 `_Provider` 子接口就行」会打红 AG2 的子接口集合断言 |
| `suite_access_token`（包体）⇄ `access_token`（Query） | 前者在 `ClaimAgentCustomizedAppRequest`，后者由 `[Token]` 注入 URL；两者**不可互换位置** | 把套件令牌拼进 URL：凭据进了 URL（日志、代理、浏览器历史泄露面），且官方报 `access_token` 无效；把 `access_token` 放进包体：官方认不出迁移对象，且 SDK 仍会再注入一次 Query 令牌 |
| `suite_access_token` ⇄ 想给它也加 `[Token]` | `[Token]` 一次只表达**一个**令牌路由键（`TokenType` / `Name` 单值），两个凭据无法同时靠声明表达；AG5 因此把套件令牌锁成 DTO 的**唯一**属性 | 给本端点再挂一个 `[Token]`：两个凭据仍只有一个能被注入，另一个静默缺失；AG3 断言本族两接口的 `[Token]` 必须恰为 `Wechat.AccessToken` + Query + `access_token`，改写即打红 |
| 本族 ⇄ 同域「设置应用」（`/cgi-bin/agent/set`） | 都改应用，但 `set` 改字段、不换归属；本族换归属、不改字段，且**不可逆** | 把迁移当「配置开关」写进日常脚本或失败重试队列，会把应用反复提交归属校验；`set` 也不能用来「回滚」迁移 |
| 本族 ⇄ 「获取应用详情」的 `customized_publish_status` | 后者是代开发自建应用的发布状态（0 待开发 / 1 开发中 / 2 已上线 / 3 存在未上线版本），是**读**；本族是归属**写**操作 | 用 `customized_publish_status` 代替归属校验，或在迁移前指望它给出「能否迁移」的答案——两者不是同一件事 |
| 40014 / 42001 / 42007 ⇄ 42009 | 前三者是 `access_token` 族失效（走自愈）；42009 是本族**包体里**的套件令牌失效 | 见 §6：42009 会被通用判定器命中并触发一次「刷新 `access_token` → 重发」，但包体原样不变，同一码复现；把它当可重试的瞬时错误会白跑，真正的处置是重新取套件令牌 |
| 裸 `WechatWorkResponse` ⇄ 自定义响应 DTO | 本端点无业务负载，AG1 逐个断言「返回类型恰 `Task<WechatWorkResponse>`」 | 新建一个「ClaimResult」DTO 接返回值：守卫打红，且官方本来就不回传任何业务字段，字段恒为默认值 |

另外三条 SDK 侧行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改用 Header）。G5 的 Query 令牌白名单同时含本族两个接口；`access_token` 已在组件 `SensitiveUrlRedactor` 词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo。`suite_access_token` 在**包体**里，不经组件脱敏——自己打日志或记录请求体时不得带出。
- **不自动判错、不做参数校验**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。
- **套件令牌不受 SDK 令牌体系保护**：它不在缓存里、不会被刷新、也不会被失效重取。它的生命周期由套件侧（`suite_ticket` 链）负责，本族只消费一次。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；响应无业务字段 |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效 / 无效 | 本族 `access_token` 失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效 | 同上 |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | 包体 `suite_access_token` 失效 / 无效 | **本族不自愈**：该值不在 `[Token]` 链上，恢复链只能失效并刷新 `access_token` 后重发，包体不变 ⇒ 同码复现。处置是回套件侧重新换取令牌后由人工/编排层重调 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号（例如误用了服务商应用上下文） |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少 `access_token` 参数 / 凭据装配缺陷 | 不自愈：多为未 `AddWechatApp`、`AgentSecret` 未配、或当前上下文解析不到 `@Internal` 令牌管理器 |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重发」。该判定器**只看 `errcode`、不区分令牌从哪来**，所以本族的 42009 也会触发一次重发——但重注入的只有 `access_token`，包体里的套件令牌原样再发一遍。若 40014/42001/42007 稳定出现，说明 `AgentSecret` 或应用装配本身有问题，重发不解决。

本端点专属的业务错误码（例如「该应用不满足迁移条件」「应用已迁移」「套件与该企业无授权关系」等）接口注释未留档 ⇒ §7。其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方页面最后更新日期与正文 | 本次未重取官方页；文档号 99617 照抄子接口 `<remarks>`，页面更新日期、权限表原文、参数表排版均 **未逐项核验** | 99617 渲染取文后回填元信息表 |
| 本端点的独立频率数值 | 接口与 DTO 注释无 QPS / 日配额 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 本端点专属错误码集合 | 注释未留档任何端点级 `errcode`；令牌族由通用判定器覆盖 ⇒ **未逐项核验** | 官方 99617 页 + 全局错误码（90455） |
| 迁移的幂等性与重复调用语义 | 注释只说「完成应用归属校验」，未说明重复调用是成功、报错还是无操作；本文因此不给「失败自动重发」的写法 ⇒ **未逐项核验** | 官方 99617；必要时用测试企业实测 |
| 迁移后应用的可观测形态 | 迁移后 `agentid` 是否保持、该应用 `access_token` 是否仍可用、在代开发侧如何回读，注释均无表述 ⇒ **未逐项核验**；本文不作承诺 | 官方 99617 + 代开发应用管理相关页 |
| 迁移是否可撤销 | 本域路由表无反向端点（AG1 锁定 14 条），但官方管理端是否提供回退、回退后归属如何变化 ⇒ **未逐项核验** | 官方管理端文档与 99617 |
| 「历史遗留的自建应用」的判定条件 | 注释只给这一短语，未定义创建时间窗、是否需企业确认、是否需要该应用未上线 ⇒ **未逐项核验** | 官方 99617 正文的适用范围段落 |
| 服务商取得企业自建应用 `corpsecret` 的官方流程 | 官方权限是「仅服务商可调用」，令牌却是企业那支应用的；两者的交付方式（企业授权、代持、书面约定）注释未给 ⇒ **未逐项核验** | 官方 99617 与服务商合规指引 |
| `suite_access_token` 的换取与有效期 | 本篇只消费；换取链（`suite_ticket` → 套件令牌）的节奏与有效期属 Authentication 与回调域，本文 **未逐项核验** | [ChatIdUpgrade.md](../AccountId/ChatIdUpgrade.md) §2.2 · [Src/Work/Mud.Wechat.Work/Authentication/](../../../../../Src/Work/Mud.Wechat.Work/Authentication/) 及其守卫 |
| 42009 场景下的重发次数与恢复管线细节 | 判定器源码可核（只看 `errcode`），但「重发几次、是否带退避」在上游组件 `Mud.HttpUtils` 的恢复管线内，本文 **未逐项核验**；处置只到「不要依赖重试」 | `Mud.HttpUtils` 令牌恢复文档与 `WechatTokenInvalidationDetector` remarks |
| 内部证据不一致（域 README 首段） | [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md) 第 3–4 行仍把整个 `Agent` 域描述为「『自定义菜单』域企业自建应用 SDK（创建菜单 + 获取菜单 + 删除菜单，全部 3 端点官方仅自建应用开放）」，而同文件第 10 行与端点速览表已按 4 族 14 支展开 ⇒ 该首段是菜单族独立成文时期的残留，**未逐项核验**孰为本文档负责人本意；本文不据此推断任何契约 | 域 README 与 AG2 的族清单（守卫为权威） |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（4 族 14 端点速览 + 令牌归属表 + 已踩陷阱）：[Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)（本文引用不重抄）
- 同域兄弟族：应用详情 / 列表 / 设置应用 / 应用权限 / 应用管理员 — [Agent.md](./Agent.md)；工作台自定义展示 — [AgentWorkbench.md](./AgentWorkbench.md)；自定义菜单 — [AgentMenu.md](./AgentMenu.md)
- 同域内最相近的「不可逆单端点」对照：[AccountId/Migration.md](../AccountId/Migration.md)（ID 迁移完成状态，单向闸门）
- 套件级凭据链（`suite_ticket` → `suite_access_token`）：[AccountId/ChatIdUpgrade.md](../AccountId/ChatIdUpgrade.md) §2.2
- 归属域与令牌错配的 fail-fast 契约：[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）
- 契约守卫源码：[WechatAgentContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）· 跨线计数基线 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)
- 本线包级公开面与已踩陷阱：[Src/Work/Mud.Wechat.Work/README.md](../../../../../Src/Work/Mud.Wechat.Work/README.md)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与 [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)「令牌与认证」一节
- 官方页（文档号照接口注释留档，本次未重取正文）：[99617](https://developer.work.weixin.qq.com/document/path/99617)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
