# 企业自建应用对接（服务商主体密文 ID → 企业主体明文 ID）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Interop/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Interop/) |
| DTO 源文件 | [OpenUserIdToUserIdRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/OpenUserIdToUserIdRequest.cs) · [OpenUserIdToUserIdForBotRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/OpenUserIdToUserIdForBotRequest.cs) · [FromServiceExternalUserIdRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/FromServiceExternalUserIdRequest.cs) · [OpenUserIdToUserIdResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/OpenUserIdToUserIdResponse.cs) · [FromServiceExternalUserIdResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/FromServiceExternalUserIdResponse.cs) · [OpenUserIdMapItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/OpenUserIdMapItem.cs) |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记）+ [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1 归属域声明 / TO3 归属键矩阵） |
| 官方文档 | 95884（企业自建应用，与第三方/代开发应用对接，E1/E3）· 101521（企业自建应用，与智能机器人对接，E2）· 页面最后更新 **本次未重取**，见「证据基础」 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（仅内部证据逐项比对；未做本次官方取文） |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与 AccountId 域全部示例合并逐字编译，0 错误（15 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 路由与 HTTP 动词 / ACCT2 层级与端点计数 / ACCT3 令牌绑定 / ACCT4 上下文登记 + TO1/TO3 归属域 + 三份接口与六份 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) 逐端点核验 2026-10-18 · 官方：正文继承本域 2026-10-18 核验留档，**本次未重取官方页**，故凡只有官方页能支撑的事实（频率数值、专属错误码、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

企业微信不允许第三方应用与代开发应用拿到企业的**明文** `userid` / `external_userid`——它们在服务商主体下只以密文形态出现。本族是**把这些密文还原回企业主体明文 ID 的唯一官方入口**，因此只对企业自建应用开放：企业用自己 `access_token` 调本族，把「某第三方/代开发应用（或某智能机器人）拿到的密文 id」换算成「本企业通讯录里的明文 userid」，或把服务商主体的 `external_userid` 换算成自建应用主体的 `external_userid`。

典型时机有两类：① 服务商应用与企业自建应用对接后，企业侧要把服务商同步过来的密文 id 落回自己的员工主数据；② 自建应用消费智能机器人回调里带的密文 `open_userid`，需要还原成明文 `userid` 才能查内部系统。

调用凭证是**企业自建应用的 `access_token`**（不是服务商级 `provider_access_token`，也不是套件级 `suite_access_token`），且归属域被钉在 `Wechat.AccessToken@Internal`——用第三方/代开发应用的配置调本族会当场 fail-fast，不会静默取到别的企业的令牌。

**不该用本族的场景**：方向反了——你手里是明文 `userid`、想要服务商主体密文 `open_userid`，那是 ID 转换族的 `UserIdToOpenUserIdAsync`（`/cgi-bin/batch/userid_to_openuserid`，第三方/代开发）；你手里是企业主体**加密** userid、想要服务商主体 open_userid，那是智能机器人族的 `ServiceUserIdToOpenUserIdAsync`（`/cgi-bin/service/batch/userid_to_openuserid`）。三条路由名字极像、方向与令牌各不相同，见 §5 对照表。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（本族唯一开放面） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（本企业 CorpId）· `AgentSecret`（即 `corpsecret`） | 同上，缺任一项抛 `InvalidOperationException`（`自建应用 {AppKey} 缺少 CorpId/AgentSecret。`），不留到首次调用 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | `WechatTokenManagerKeys.InternalAccessToken`（常量值 `Wechat.AccessToken@Internal`），由子接口 `[Token]` 声明 | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；第三方/代开发应用上下文命中该键抛 `WechatTokenOwnerMismatchException`（TO3 锁定，fail-fast 而非静默取错令牌） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |

`access_token` 由 SDK 侧的自建应用令牌管理器以 `CorpId + AgentSecret` 自动换取、缓存并刷新，**开发者不接触令牌值**；令牌失效自愈链路与判定器同集合，见 §6。

### 2.2 权限与业务前置

出处为接口与请求 DTO 的 XML 注释（本域 2026-10-18 留档口径），不是本文推断：

- **E1 / E3 必须显式指明密文来源**：`source_agentid` 填「企业授权的代开发自建应用或第三方应用的 agentid」（`OpenUserIdToUserIdRequest` / `FromServiceExternalUserIdRequest` 注释）。E1 还要求 `open_userid_list` **必须是 `source_agentid` 对应的应用所获取**——换了来源应用 id 就换不出明文 id。
- **双可见范围**：E1/E2 要求「成员需要同时在 `access_token` 和 `source_agentid` 所对应应用的可见范围内」（子接口 remarks）；E2 无 `source_agentid`，故只要求「成员在 `access_token` 所对应应用的可见范围内」（`OpenUserIdToUserIdForBotRequest` 注释）。E3 的主语换成「客户的跟进人，或者用户所在客户群的群主」，同样要求双可见范围。
- **批量上限**：E1/E2 的 `open_userid_list` **最多不超过 1000 个**（接口 `<summary>` 与 DTO remarks 双重留档）。E3 是**单值入单值出**，无批量语义。
- **密文大小写敏感**：本域多处注释留档 corpid / userid / external_userid 密文使用时不能再转为纯小写或纯大写 ⇒ 宿主侧不得做 `ToLower()` 归一化（域 README §9）。

### 2.3 官方开放的应用类型

官方只对企业自建应用开放本族（父接口 XML：「官方仅向自建应用开放本族端点（第三方/代开发无对应文档）」）。落到代码形态：

- 公共父接口 `IWechatWorkAccountIdInteropService` **零端点**（ACCT2 断言 `DeclaredOnly` 方法数 = 0）；
- 全部 3 个端点声明在自建子接口 `IWechatWorkInternalAccountIdInteropService` 上（ACCT2 断言恰 3）；
- 继承链上**不得**出现第三方/代开发子接口（ACCT2 `AssertDerivedInterfaces` 期望集合只有一个元素）。

⇒ 第三方/代开发应用没有通向本族的 DI 入口，也没有通向它的凭据路径（`Wechat.AccessToken@Internal` 只由自建应用装配）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdInteropService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），零端点，仅作契约基座 | 0 | 否 |
| `IWechatWorkInternalAccountIdInteropService` | `Mud.Wechat.Work` | 企业自建应用子接口，承载全部 3 端点 | 3（E1/E2/E3） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`（子接口 `RegistryGroupName`，父接口恒空；子接口 `InheritedFrom = nameof(WechatWorkAccountIdInteropService)`）
- 端点索引：**E1** `OpenUserIdToUserIdAsync`（密文 open_userid → 明文 userid，带 `source_agentid`）· **E2** `OpenUserIdToUserIdForBotAsync`（同路由，请求体无 `source_agentid`）· **E3** `ConvertFromServiceExternalUserIdAsync`（服务商主体 external_userid → 企业主体 external_userid，单值）

最终用户注入**自建子接口**，不要消费父接口（父接口不注册 DI，且一个端点也没有）。

---

## 4. 端点

### E1. `OpenUserIdToUserIdAsync` — 服务商主体密文 open_userid 转企业主体明文 userid

**签名**

```csharp
Task<OpenUserIdToUserIdResponse> OpenUserIdToUserIdAsync(
    [Body] OpenUserIdToUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/batch/openuserid_to_userid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/batch/openuserid_to_userid?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建应用凭证） |
| 适用应用类型 | 仅企业自建应用 |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 已登记请求/响应/`OpenUserIdMapItem` 三方） |
| 官方文档 | 95884 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OpenUserIdList` | `open_userid_list` | `List<string>?` | 是 | 服务商主体下的密文 open_userid，**最多不超过 1000 个**，且**必须是 `SourceAgentId` 对应的应用所获取**（接口 `<summary>` + DTO remarks） |
| `request.SourceAgentId` | `source_agentid` | `string?` | 是 | 企业授权的代开发自建应用或第三方应用的 agentid。本仓把它建模为 `string?`（作为**来源标注**），与 ID 转换族里作为**操作对象**的 `agentid`（`int?`）是两套建模，勿「顺手统一」（域 README §3） |
| —（不进方法签名） | `access_token` | — | 是 | 自建应用的接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`OpenUserIdList` 为 `null`、为空或超过 1000 条都照样发出，错误以 `errcode` 返回；分批是调用方职责。

```json
{
  "open_userid_list": ["woJIGENGOEOEJGOE", "wOSGJEIOGNG"],
  "source_agentid": "1000002"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `userid_list` | `UserIdList` | `List<OpenUserIdMapItem>?` | 转换成功的**明文 userid** 映射列表 |
| `userid_list[].userid` | `UserId` | `string?` | 企业主体下的明文 userid（本端点语义） |
| `userid_list[].open_userid` | `OpenUserId` | `string?` | 服务商主体下的密文 open_userid（回显入参值） |
| `invalid_open_userid_list` | `InvalidOpenUserIdList` | `List<string>?` | 不合法的 open_userid 列表；**如果传入的已是明文的 userid，也会在该列表返回**（响应 DTO 注释原文） |

两处必须点名的形状陷阱：① 本端点响应的 `userid_list` 是 `List<OpenUserIdMapItem>`（对象数组），而 ID 转换族 E1 请求体的 `userid_list` 是 `List<string>` —— **同 JSON 名、两种形状**，建模时按端点看 DTO，不能按字段名复用类型（域 README §9）。② `OpenUserIdMapItem` 是三个 userid/open_userid 转换端点共用的结果元素，它的 `userid` 字段在别的端点装的是密文，别按字段名假定明文。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "userid_list": [
    { "userid": "mud_zhangsan", "open_userid": "woJIGENGOEOEJGOE" }
  ],
  "invalid_open_userid_list": ["wOSGJEIOGNG"]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;            // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException（判错出口）
using Mud.Wechat.Work.DataModels.AccountId;    // 请求/响应 DTO

// 宿主为 ASP.NET Core（Web SDK）+ ImplicitUsings 时，System.Linq / System.Collections.Generic /
// Microsoft.Extensions.DependencyInjection（GetRequiredService）/ Microsoft.Extensions.Logging
// 均由 SDK 隐式提供；换非 Web 宿主形态须自行补齐 using。

// ① 注册自建应用：自建只需 CorpId + AgentSecret 两项，Validate() 在启动期校验，缺项即抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:CorpSecret"];   // 即 corpsecret；一律配置/环境变量读取，不得有真实值
});

// ② 注册本域模块（令牌底座随 AddWechatApp 装配，无需额外注册令牌链）。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析自建子接口。父接口 IWechatWorkAccountIdInteropService 零端点且不注册 DI，不可解析。
IWechatWorkInternalAccountIdInteropService interop =
    provider.GetRequiredService<IWechatWorkInternalAccountIdInteropService>();

List<string> encryptedOpenUserIds = new List<string> { "woJIGENGOEOEJGOE", "wOSGJEIOGNG" }; // 假值示例
string sourceAgentId = "1000002";   // 密文来源应用：官方拼写 source_agentid，本仓建模为 string

// ④ 服务商主体密文 open_userid → 企业主体明文 userid（官方单批 ≤1000，SDK 不裁剪、不分批）。
OpenUserIdToUserIdResponse mapResponse = await interop.OpenUserIdToUserIdAsync(
    new OpenUserIdToUserIdRequest
    {
        OpenUserIdList = encryptedOpenUserIds.Take(1000).ToList(),
        SourceAgentId = sourceAgentId,
    },
    cancellationToken);

// ⑤ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
//    不判就是把空集合当成功。
WechatWorkException.ThrowIfFailed(mapResponse, "/cgi-bin/batch/openuserid_to_userid");

// ⑥ 取业务字段。本端点是「部分成功」语义：两个列表可同时非空，必须逐条对账。
List<OpenUserIdMapItem> mapped = mapResponse.UserIdList ?? new List<OpenUserIdMapItem>();
List<string> invalidOpenUserIds = mapResponse.InvalidOpenUserIdList ?? new List<string>();

if (mapped.Count == 0)
    logger.LogWarning("整批未还原成明文 userid，invalid 数量 {Count}", invalidOpenUserIds.Count);

// invalid 里是敏感标识，不得进日志明文；这里只取计数。
logger.LogInformation("还原 {Count} 条明文 userid", mapped.Count);
```

**本端点注意事项**

- 单批 1000 条上限来自接口 `<summary>` 与 DTO remarks；超限的官方失败形态（整批拒绝还是部分处理）本文未取证，见 §7。
- `source_agentid` 填错（应用不匹配、企业未授权该应用）时是 `errcode` 非 0 还是整批落 `invalid_open_userid_list`，注释未留档 ⇒ 见 §7，按未核验处理：生产上两个分支都要有告警。
- 「传入的已是明文 userid」官方**不报错**，整批落 `invalid_open_userid_list`（响应 DTO 注释）⇒ 表现为「调用成功但一条都没还原」，是最难定位的静默失败。

### E2. `OpenUserIdToUserIdForBotAsync` — 智能机器人密文 open_userid 转明文 userid（同路由、无 `source_agentid`）

**签名**

```csharp
Task<OpenUserIdToUserIdResponse> OpenUserIdToUserIdForBotAsync(
    [Body] OpenUserIdToUserIdForBotRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/batch/openuserid_to_userid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/batch/openuserid_to_userid?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建应用凭证） |
| 适用应用类型 | 仅企业自建应用 |
| 请求体编码 | `application/json`（`OpenUserIdToUserIdForBotRequest` 已登记 `AccountIdJsonContext`，ACCT4） |
| 官方文档 | 101521 |

**与 E1 的关系**：路由、HTTP 动词、响应类型与 E1 **完全相同**，唯一差别是请求体**没有 `source_agentid`**（`OpenUserIdToUserIdForBotRequest` 只有 `OpenUserIdList` 一个属性）。接口 XML 把这条列为「官方契约陷阱」；ACCT1 用「20 方法 / 19 唯一路由」两条断言把它钉住——本仓的处置是**两个方法 + 两个 DTO**，不做运行时多态（不用一个方法配可选字段），因为声明式客户端按声明类型源生成序列化，可选字段方案在 AOT 下无法稳定表达「本场景不得出现该键」。

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OpenUserIdList` | `open_userid_list` | `List<string>?` | 是 | 智能机器人获取的密文 open_userid，**最多不超过 1000 个**（接口 `<summary>` + DTO remarks） |
| —（本场景无此字段） | `source_agentid` | — | 否 | `OpenUserIdToUserIdForBotRequest` **不提供该属性**；官方本场景不要求（域 README §3） |
| —（不进方法签名） | `access_token` | — | 是 | Query 注入 |

可见范围口径与 E1 不同：本场景只要求「成员在 `access_token` 所对应应用的可见范围内」（`OpenUserIdToUserIdForBotRequest` remarks），不涉及 `source_agentid` 对应应用。

```json
{
  "open_userid_list": ["woJIGENGOEOEJGOE"]
}
```

**响应参数**

与 E1 同一类型 `OpenUserIdToUserIdResponse`，字段表、两处形状陷阱与语义完全一致（`userid_list` 为明文 userid 映射、`invalid_open_userid_list` 承接不合法值与「误传明文」），见 E1 的响应参数表，不再重抄。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "userid_list": [
    { "userid": "mud_zhangsan", "open_userid": "woJIGENGOEOEJGOE" }
  ],
  "invalid_open_userid_list": []
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.AccountId;    // 请求/响应 DTO

// 前置同 E1：应用与模块已由宿主完成 AddWechatApp + AddAccountIdApi 注册，此处只走「解析 → 调用 → 判错 → 取值」。
IWechatWorkInternalAccountIdInteropService botInterop =
    provider.GetRequiredService<IWechatWorkInternalAccountIdInteropService>();

List<string> botOpenUserIds = new List<string> { "woJIGENGOEOEJGOE" };   // 假值：机器人回调帧里的密文 id

OpenUserIdToUserIdResponse botResponse = await botInterop.OpenUserIdToUserIdForBotAsync(
    new OpenUserIdToUserIdForBotRequest { OpenUserIdList = botOpenUserIds },
    cancellationToken);
WechatWorkException.ThrowIfFailed(botResponse, "/cgi-bin/batch/openuserid_to_userid");

List<OpenUserIdMapItem> botMapped = botResponse.UserIdList ?? new List<OpenUserIdMapItem>();
List<string> botInvalid = botResponse.InvalidOpenUserIdList ?? new List<string>();
logger.LogInformation("机器人侧 id 还原 {Ok} 条、不合法 {Invalid} 条", botMapped.Count, botInvalid.Count);
```

**本端点注意事项**

- 选错方法不会当场报错：`OpenUserIdToUserIdAsync`（E1）也能吃下机器人侧的密文 id，但官方要求 E1 的 `open_userid_list` 必须是 `source_agentid` 对应应用所获取 —— 机器人 id 没有对应来源应用，用 E1 调本场景属于契约外用法，其失败形态本文未取证（§7）。
- 机器人密文 open_userid 的**签发来源**（回调帧字段）不在本域，归智能机器人邻域 ⇒ 见 §7。

### E3. `ConvertFromServiceExternalUserIdAsync` — 服务商主体 external_userid 转企业主体 external_userid

**签名**

```csharp
Task<FromServiceExternalUserIdResponse> ConvertFromServiceExternalUserIdAsync(
    [Body] FromServiceExternalUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalcontact/from_service_external_userid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalcontact/from_service_external_userid?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建应用凭证） |
| 适用应用类型 | 仅企业自建应用 |
| 请求体编码 | `application/json`（`FromServiceExternalUserIdRequest` / `FromServiceExternalUserIdResponse` 已登记 `AccountIdJsonContext`，ACCT4） |
| 官方文档 | 95884 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ExternalUserId` | `external_userid` | `string?` | 是 | **服务商主体**的 external_userid，必须是 `SourceAgentId` 对应的应用所获取（DTO 注释）。**单值入参，请求体是 `string?` 而不是列表** |
| `request.SourceAgentId` | `source_agentid` | `string?` | 是 | 企业授权的代开发自建应用或第三方应用的 agentid（`string?` 建模，同 E1） |
| —（不进方法签名） | `access_token` | — | 是 | Query 注入 |

本端点**没有批量语义**：域 README §4 把上限一览的最后一行写作「**1（单值）** | #15 `external_userid` | 请求体是 `string?`，非列表」，与本 DTO 声明一致。多个 id 要转就多次调用，SDK 与官方都不代为聚合。

```json
{
  "external_userid": "wmSERVICEEXTERNALIDFAKE",
  "source_agentid": "1000002"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `external_userid` | `ExternalUserId` | `string?` | 企业主体（自建应用）的 external_userid |

响应里**没有** `invalid_*` 袋：单值转换只有「成功拿到新 id」与「`errcode` 非 0」两种形态（就本仓可核验的部分而言；失败形态全集见 §7）。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "external_userid": "wmENTERPRISEEXTERNALIDFAKE"
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.AccountId;    // 请求/响应 DTO

// 前置同 E1：应用与模块已注册。本端点单值入单值出，循环调用是调用方的职责。
IWechatWorkInternalAccountIdInteropService externalInterop =
    provider.GetRequiredService<IWechatWorkInternalAccountIdInteropService>();

string serviceExternalUserId = "wmSERVICEEXTERNALIDFAKE";   // 假值：服务商主体 id
string sourceAgentIdForExternal = "1000002";

FromServiceExternalUserIdResponse externalResponse =
    await externalInterop.ConvertFromServiceExternalUserIdAsync(
        new FromServiceExternalUserIdRequest
        {
            ExternalUserId = serviceExternalUserId,
            SourceAgentId = sourceAgentIdForExternal,
        },
        cancellationToken);
WechatWorkException.ThrowIfFailed(externalResponse, "/cgi-bin/externalcontact/from_service_external_userid");

// 密文大小写敏感：不得 ToLower()/ToUpper() 归一化（域 README §9 的既有口径）。
string enterpriseExternalUserId = externalResponse.ExternalUserId ?? string.Empty;
```

**本端点注意事项**

- 可见范围的主语是「客户的跟进人，或者用户所在客户群的群主」，需同时在 `access_token` 与 `source_agentid` 对应应用的可见范围内（子接口 remarks 原文）——跟进人变更后重算，同一服务商主体 id 可能换算出不同结果或失败。
- 与 ID 转换族的正向端点（服务商主体 external_userid 的产出端，`/cgi-bin/idconvert/unionid_to_external_userid` 等）**不同族、不同应用类型面**：正向那几支对第三方/代开发开放，本支只对自建开放，二者不能互为「撤销」（域 README §2、§8）。

---

## 5. 陷阱与易混对照

**三向不对称对照表**（本族最核心的易混点：三条路由尾段名字都像 `userid` ↔ `openuserid`，但方向、输入承载物、令牌、开放面四件事两两不同）：

| 端点 | 路由 | 输入 JSON 键 | 输入承载的语义 | 输出语义 | 令牌与开放面 |
|---|---|---|---|---|---|
| ID 转换族 `UserIdToOpenUserIdAsync` | `/cgi-bin/batch/userid_to_openuserid` | `userid_list`（`List<string>`） | 企业主体**明文** userid | 服务商主体密文 open_userid | `access_token` + `Wechat.AccessToken@Corp`；第三方 + 代开发 |
| **本族 E1/E2** `OpenUserIdToUserId*Async` | `/cgi-bin/batch/openuserid_to_userid` | `open_userid_list`（`List<string>`） | 服务商主体密文 open_userid | 企业主体**明文** userid | `access_token` + `Wechat.AccessToken@Internal`；**仅自建** |
| 智能机器人族 `ServiceUserIdToOpenUserIdAsync` | `/cgi-bin/service/batch/userid_to_openuserid` | `open_userid_list`（`List<string>`） | 企业主体**加密** userid | 服务商主体 open_userid（另返回 `open_corpid`） | `provider_access_token`；第三方 + 代开发 |

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| ID 转换族 `UserIdToOpenUserIdAsync`（`/cgi-bin/batch/userid_to_openuserid`） | 与本族 E1 **仅差一个 `open` 前缀**，方向完全相反（明文 → 密文 vs 密文 → 明文），且令牌归属域一个是 `@Corp` 一个是 `@Internal` | 拿自建配置调那一条、或拿第三方配置调本族，都会在 `WechatAppContext.GetTokenManager` 处抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）；复制路由时极易只改一半 |
| 智能机器人族 `ServiceUserIdToOpenUserIdAsync`（`/cgi-bin/service/batch/userid_to_openuserid`） | 路由多一段 `service/`、凭据是服务商级 `provider_access_token`、输入键写成 `open_userid_list` 承载的却是**企业主体加密 userid** | 把它当本端的失败兜底 = 换了个方向和换了个鉴权主体，仍拿不到明文 id |
| E1 与 E2（**同路由两个方法**） | 唯一差别是请求体带不带 `source_agentid`（两个不同 DTO）；路由、动词、响应类型完全相同 | 用 E1 承载机器人 id 要凭空填一个来源 agentid；用 E2 承载第三方密文 id 则丢了官方要求的来源声明 —— 抓包对比时看不出请求地址差别，只能看请求体 |
| E1 响应的 `userid_list` 与 ID 转换族请求的 `userid_list` | 同 JSON 名、不同形状：前者 `List<OpenUserIdMapItem>`，后者 `List<string>` | 复用同一个 C# 类型/同一份反序列化代码会当场类型不匹配；按 JSON 名建模是本域最常见的写法缺陷（域 README §9） |
| `OpenUserIdMapItem.UserId` 的语义 | 该元素被三个转换端点共用，`userid` 在本族是**明文**、在智能机器人族是**密文** | 按字段名推断明文，把密文当主键落库 ⇒ 与通讯录 join 不上，且无异常 |
| 把明文 `userid` 传进 `open_userid_list` | 官方**不报错**，`errcode` 仍为 0，明文值整批落 `invalid_open_userid_list`（响应 DTO 注释原文） | 业务侧表现为「全部转换失败但调用成功」，日志无异常，最难定位 |
| 只判 `errcode == 0` 就认为成功 | E1/E2 是**部分成功**语义：`userid_list` 与 `invalid_open_userid_list` 可同时非空 | 缺量的 id 被静默丢弃，下游按「已还原成明文」处理，主键对不上 |

另外三条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 为已接受风险，无法改用 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 词表内，异常消息里的 `RequestUri` 构造期剥离 query 与 userinfo——但自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。
- **归属域键在唯一咽喉点校验**：`IWechatWorkInternalAccountIdInteropService` 声明 `TokenManagerKey = Wechat.AccessToken@Internal`（TO1），父接口不声明归属键；`WechatTokenRouting.OwnedKeys(Internal)` 与 `OwnedKeys(ThirdParty/Provider)` 的键集互不相交（TO2），错配是异常而非取错值。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（**不代表全部还原成功**，E1/E2 见 §5） | 正常路径 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 失效/过期/关联凭证失效 | 本族的族别失效码，命中即走令牌自愈链路，业务侧通常看不到 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷（未 `AddWechatApp`、或未 `AddAccountIdApi()`） |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | `suite_access_token` / `provider_access_token` 失效 | 本族不消费这两种令牌；出现即凭据族别错配的信号，回查应用配置与所调接口 |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`，命中后的链路是「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」。本族是 `access_token` + `@Internal`，**若 40014/42001 稳定出现，说明刷新本身在失败**，可依次排查：`AgentSecret`（corpsecret）是否正确、应用是否被停用、自建应用凭证是否被别处并发刷新挤掉。加重试不解决这三类成因。

本族三支端点的专属业务错误码，接口注释未留档、本文亦未向官方页取证 ⇒ **未逐项核验**，见 §7。需要按码分流时先核验，不要在 SDK 侧硬编码码表。其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（文档号 90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方页面最后更新日期 | 本次未重取官方页（任务纪律：官方取证由文档负责人串行完成），正文继承本域 2026-10-18 逐端点核验留档 ⇒ **未逐项核验** | 95884 / 101521 官方页渲染取文后回填元信息表 |
| 本族三支端点的独立频率数值 | 接口与 DTO 注释只留档批量上限（≤1000）与可见范围，无 QPS/日配额 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 本族专属错误码集合 | 注释未留档错误码表；令牌失效码由通用判定器覆盖 ⇒ **未逐项核验** | 官方附录「全局错误码」（90455）逐端点核对后补接口 remarks |
| 超过 1000 条时官方行为 | 注释只写「最多不超过 1000 个」，未说明超限是整批拒绝还是部分处理 ⇒ **未逐项核验** | 95884 页实测或官方附录 90454 |
| `source_agentid` 不匹配 / 应用未授权的失败形态 | 注释只写「必须是 `source_agentid` 对应的应用所获取」，未说明是 `errcode` 非 0 还是整批落 `invalid_open_userid_list` ⇒ **未逐项核验** | 95884 页；核验后同批补接口 `<summary>` |
| E2（机器人场景）误用 E1 方法的行为 | 本仓按「官方不要求 `source_agentid`」拆成两个 DTO/两个方法，官方对「机器人 id + 带 `source_agentid`」的组合如何处理无留档 ⇒ **未逐项核验** | 101521 与 95884 两页对照 |
| 密文 open_userid ↔ 明文 userid 映射的有效期与幂等性 | 注释与域 README 均无表述，本文不作假设性承诺 ⇒ **未逐项核验** | 95884 页与账号 ID 安全性升级总览页（96516） |
| `source_botid` 与机器人密文 id 的签发来源链 | 本域只消费 id，其来源（智能机器人回调帧）归 Aibot 邻域 ⇒ **未在本域核验** | [Interfaces/Aibot/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Aibot/) 与 Aibot 域守卫 |
| 内部证据不一致（守卫 remarks 的方向表述） | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs) 的类级 remarks 把「未明确企业身份场景」（`/cgi-bin/service/batch/userid_to_openuserid`）写成「实际转换为 open_userid → userid」，而该端点的接口 `<summary>`、请求 DTO（`open_userid_list` 承载企业主体加密 userid）与响应 DTO（产出 `open_userid`）一致指向 **加密 userid → open_userid**（[Bot.md](./Bot.md) 亦同）⇒ 本文按「接口 + DTO」取证，方向以 §5 对照表为准；该 remarks 措辞须由维护者核验后修订 | ACCT1~ACCT3 断言（路由/层级/令牌，均为权威）＋ `ServiceUserIdToOpenUserIdRequest`/`Response` 的 XML 注释 |
| 内部证据不一致（域 README 的非 AccessToken 子接口计数） | [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) §1 归属域表格写「六个 ProviderAccessToken / SuiteAccessToken 子接口 **不得声明**」，而 ACCT3 的 provider 组子接口恰 6 个（corpid 2 + 迁移 2 + 机器人 2）、suite 组子接口 1 个（本域群 ID 升级族代开发子接口），合计 **7** 个非 AccessToken 子接口 ⇒ 该「六个」只在「仅统计 ProviderAccessToken 子接口」时成立，文案把 Suite 子接口一并计入则偏少一项 ⇒ **未逐项核验孰为本意** | ACCT3 的三个接口数组（权威）＋ TO1 的非 AccessToken 分支 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 三向不对称、定位键不对称、批量上限、字段名陷阱汇总）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（§1 令牌路由键、§2 方向不对称、§3 定位键、§4 批量上限、§8 一页多端点拆分，本文引用不重抄）
- 兄弟族（同为 userid/open_userid 换算链的两端）：
  - [Bot.md](./Bot.md) — 智能机器人 userid 转换族（`provider_access_token`，企业主体加密 userid → 服务商主体 open_userid）
  - [IdConvert.md](./IdConvert.md)（ID 转换族，含反向的 `UserIdToOpenUserIdAsync` 与代开发的群 ID 转换两支）
- 归属域与令牌错配的 fail-fast 契约：[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）
- 自建应用令牌管理器：[Src/Work/Mud.Wechat.Work/Authentication/](../../../../../Src/Work/Mud.Wechat.Work/Authentication/)
- 凭据与多应用作用域：`guide/配置与凭据.md`（**尚未成文**）
- 官方页：[自建应用与第三方/代开发应用的对接（95884）](https://developer.work.weixin.qq.com/document/path/95884) · [自建应用与智能机器人的对接（101521）](https://developer.work.weixin.qq.com/document/path/101521)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
