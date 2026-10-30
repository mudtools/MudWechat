# 上下游 · 基础接口（应用共享 / 下级凭证 / 关联客户）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `CorpGroup` 域 · 上下游基础接口族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCorpGroupService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupService.cs)（三类公共面 1 支）· [IWechatWorkCorpGroupInternalProviderService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupInternalProviderService.cs)（**中间父接口**，自建+代开发 5 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupService_ThirdParty.cs)（三支均空标记） |
| DTO 源文件 | [DataModels/CorpGroup/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/CorpGroup/)（15 型登记进 `CorpGroupJsonContext`，命名空间 `Mud.Wechat.Work.DataModels.CorpGroup`） |
| 契约守卫 | [WechatCorpGroupContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCorpGroupContractGuards.cs)（CG1 路由表 / CG2 三层继承链与「第三方不继承中间父接口」/ CG3 令牌绑定 / CG4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 应用共享 95813/95324/96872（旧树 93403/93405/96816）· 下级凭证 95816/96873（93359/96814）· 小程序 session 95817/96874（93355/96813）· unionid→external_userid 95818/96875 · unionid→pending_id 97357/98040 · external_userid→pending_id 97357/98040 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 2 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CG1~CG4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

上下游 / 企业互联这条线的**基座**：谁跟谁共享了应用、怎么拿到下级企业的调用凭证、微信客户身份如何在上下游之间换算。

| 端点 | 用途 |
|---|---|
| E1 `corp/list_app_share_info` | 拉取上级↔下级（上游↔下游）之间的**应用共享信息**（本族唯一向第三方开放的端点） |
| E2 `corp/gettoken` | 用上游应用凭证**换下级/下游企业应用的 access_token** |
| E3 `miniprogram/transfer_session` | 把上级企业的小程序登录态换成**下级企业的 session** |
| E4 `unionid_to_external_userid` | unionid → 下游企业外部联系人 `external_userid`（已添加客户） |
| E5 `unionid_to_pending_id` | unionid → 临时外部联系人 `pending_id`（未添加客户，90 天有效） |
| E6 `batch/external_userid_to_pending_id` | `external_userid` 批量 → `pending_id`（单批 ≤100） |

E4/E5/E6 三支合起来是一套**三段映射**：`unionid = pending_id = external_userid`。品牌方先给粉丝的 unionid 预生成 pending_id（E5），等该微信用户真成为下游客户后，用 E6 把 external_userid 也挂到同一 pending_id 上，或直接 E4 换真实 external_userid。

本族形态与其它域不同的一点：**存在「中间父接口」**。`IWechatWorkCorpGroupInternalProviderService` 既是父接口的子接口、又是自建/代开发子接口的父接口，专门承载「官方只给自建 + 代开发」的 5 支；第三方子接口**直接继承公共父接口**，因此它的类型化面只有 1 支端点。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider`（E2–E6）；`WechatAppType.ThirdParty` 仅 E1 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CG3 对全部接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

**E3 的令牌语义是本域最容易踩的坑**：它消费的 `access_token` 是 **E2 返回的下级/下游企业凭证**，不是上游自己的令牌。而本仓令牌基座**只托管调用方自身应用的令牌、不自动缓存 E2 返回的凭证** —— 需要宿主把它写入令牌存储并切换上下文，或自行注入（接口注释明示）。该凭证与 `session_key` 都是凭据/敏感值，**不得进日志、遥测或异常消息**。

### 2.2 权限与业务前置

- E1：`agentid` 为上级/上游企业应用 id（必填）；`limit` **最大 100**，默认或 `0` 表示全量；官方建议分页（`cursor`）或指定 `corpid` 拉取。
- E2：`corpid` + `agentid` 均为**已授权**的下级/下游企业与应用；返回凭证最长 **512 字节**，生命周期由调用方管理。
- E3：`userid` 与 `session_key` 都来自 `code2Session`、**各不多于 64 字节**；且该凭证对应的下级应用必须是上级应用**分享而来**的。
- E4/E5/E6 共同前置：应用须是**上下游共享应用**且具有**客户联系权限**；上游企业须**已认证**；`unionid` 与 `openid` 主体须与当前企业一致、且来自**同一个小程序**。
- 频率（E4/E5/E6，按**上游企业**维度）：**10 万次/小时、48 万次/天、750 万次/月**；E4 传有效 `mass_call_ticket` 可豁免该档限制（仍受基础频率约束），定位是数据初始化场景。
- `pending_id` **有效期 90 天**、共享应用内唯一 ⇒ 预转换后须在窗口内完成真实换取。
- E6 单批**最多 100 个** `external_userid`；上游应用须**先调用过 E5**；该客户的跟进人（或传入 `chat_id` 时的群主）必须在应用可见范围内。
- **自 2023-12-01 0 点起不再支持通过系统应用 secret 调用**。本仓不做本地限流与批次裁剪，越界由 `errcode` 表达。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 应用共享信息 | ✅ | ✅（95324） | ✅ |
| E2 下级企业凭证 | ✅ | ❌ 官方无文档 | ✅ |
| E3 下级企业小程序 session | ✅ | ❌ | ✅ |
| E4–E6 关联客户信息 | ✅ | ❌ | ✅ |

⇒ 注入 `IWechatWorkThirdPartyCorpGroupService` 时**只有 E1 可调用**，这不是本仓漏挂：CG2 反向断言第三方子接口**不得**继承中间父接口，其类型化面恰为 1。官方若补第三方文档页，须先核验再同批改 CG1/CG2 与 G5 白名单。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 类型化面（含继承） | 可注入 DI |
|---|---|---|---|---|---|
| `IWechatWorkCorpGroupService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 1 | 1 | 否 |
| `IWechatWorkCorpGroupInternalProviderService` | `Mud.Wechat.Work.Interfaces` | **中间父接口**（`IsAbstract = true`，`InheritedFrom = WechatWorkCorpGroupService`） | 5 | 6 | 否 |
| `IWechatWorkInternalCorpGroupService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | **6** | 是 |
| `IWechatWorkProviderCorpGroupService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | **6** | 是 |
| `IWechatWorkThirdPartyCorpGroupService` | `Mud.Wechat.Work` | 第三方子接口（空标记，**不继承中间父接口**） | 0 | **1** | 是 |

注册组 `CorpGroup`，模块枚举 `WechatModule.CorpGroup`，装配入口 `AddCorpGroupApi()`。

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddCorpGroupApi());
```

## 4. 端点

### E1. `ListAppShareInfoAsync` — 获取应用共享信息

```csharp
Task<ListAppShareInfoResponse> ListAppShareInfoAsync(
    [Body] ListAppShareInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/corpgroup/corp/list_app_share_info`（查询语义走 POST） |
| 声明接口 | 公共父接口（三类应用均可调） |
| 令牌 | Query `access_token`（上级/上游企业应用凭证） |

**请求参数**（`ListAppShareInfoRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `agentid` | `AgentId` | `int` | 是 | 上级/上游企业应用 agentid |
| `business_type` | `BusinessType` | `int?` | 否 | `0` 企业互联/局校互联、`1` 上下游企业；不填由官方按默认处理 |
| `corpid` | `CorpId` | `string?` | 否 | 指定时仅拉取该下级/下游企业 |
| `limit` | `Limit` | `int?` | 否 | **最大 100**；默认或 0 为全量 |
| `cursor` | `Cursor` | `string?` | 否 | 上一次返回的游标；首次可不填 |

**响应参数**（`ListAppShareInfoResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `corp_list[]` | `CorpList` | `List<AppShareCorpInfo>?` | 应用共享信息列表 |
| `corp_list[].corpid` | `CorpId` | `string` | 下级/下游企业 corpid |
| `corp_list[].corp_name` | `CorpName` | `string?` | 企业名称 |
| `corp_list[].agentid` | `AgentId` | `int?` | 下级/下游企业应用 id |
| `ending` | `Ending` | `int?` | **1 已拉取完毕 / 0 未拉完** |
| `next_cursor` | `NextCursor` | `string?` | 下次请求游标；无更多数据时返回空 |

**调用示例**（本族主示例：分页拉取共享应用清单）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.CorpGroup;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "upstream-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddCorpGroupApi());

var corpGroup = provider.GetRequiredService<IWechatWorkInternalCorpGroupService>();

string? cursor = null;
while (true)
{
    var resp = await corpGroup.ListAppShareInfoAsync(
        new ListAppShareInfoRequest
        {
            AgentId = 1000002,          // 上游应用 agentid（假值）
            BusinessType = 1,           // 1 上下游
            Limit = 100,                // 最大值 100
            Cursor = cursor,            // 首次不填
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/corpgroup/corp/list_app_share_info");

    foreach (AppShareCorpInfo corp in resp.CorpList ?? new List<AppShareCorpInfo>())
    {
        // 这三个字段就是 E2 的入参来源（corpid + agentid）。
        logger.LogInformation("共享下游 corp={CorpId} agent={AgentId}", corp.CorpId, corp.AgentId);
    }

    // 终止条件有两处：ending == 1，或 next_cursor 为空——两个都要判。
    if (resp.Ending == 1 || string.IsNullOrEmpty(resp.NextCursor)) break;
    cursor = resp.NextCursor;
}
```

**本端点注意事项**

- 分页终止有**两个信号**（`ending` 与 `next_cursor`），只判一个会在末页多打一次或提前退出。
- 官方权限说明写的是「自建应用和第三方应用」——本端点是本域**唯一**在第三方文档树里有页的端点。
- 旧「企业互联」文档树 93403/93405/96816 与 95813/95324/96872 是**同一契约的别名**，不是六个端点。

### E2. `GetCorpGroupTokenAsync` — 获取下级/下游企业的 access_token

```csharp
Task<GetCorpGroupTokenResponse> GetCorpGroupTokenAsync(
    [Body] GetCorpGroupTokenRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/corpgroup/corp/gettoken` |
| 声明接口 | 中间父接口 `IWechatWorkCorpGroupInternalProviderService`（自建 + 代开发） |

**请求参数**（`GetCorpGroupTokenRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `CorpId` | `string` | 是 | 已授权的下级/下游企业 corpid |
| `agentid` | `AgentId` | `int` | 是 | 已授权的下级/下游企业应用 id |
| `business_type` | `BusinessType` | `int?` | 否 | `0` 企业互联/局校互联（默认）、`1` 上下游企业 |

**响应参数**：`access_token`（`string?`，**最长 512 字节，敏感**）、`expires_in`（`int`，秒）。

**本端点注意事项**

- 这是**「换取别人家的令牌」**的端点：SDK 令牌基座不会替你缓存它，E3 要用就得自己管生命周期。
- 不要把返回的 `access_token` 打进日志，也不要塞进异常消息。

### E3. `TransferMiniProgramSessionAsync` — 下级企业小程序 session

```csharp
Task<TransferMiniProgramSessionResponse> TransferMiniProgramSessionAsync(
    [Body] TransferMiniProgramSessionRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/miniprogram/transfer_session`（**路由挂 `miniprogram/` 域，但归属本域**） |
| 声明接口 | 中间父接口 |
| 令牌 | **下级/下游企业的 `access_token`**（E2 所得），而非上游自身令牌 |

**请求参数**：`userid`（`string`，必填，来自 `code2Session`，≤64 字节）、`session_key`（`string`，必填，上级企业的会话密钥，≤64 字节，敏感）。
**响应参数**：`userid`（`string?`，下级企业侧用户 ID）、`session_key`（`string?`，**下级企业侧会话密钥，敏感**）。

**本端点注意事项**：前置条件是「该凭证对应的下级应用必须是上级应用分享而来的」，否则换取不到有效 session。路由前缀与 MiniProgram 线无关——它是企微上下游域的一条 miniprogram 前缀路由（CG1 逐条钉死）。

### E4. `UnionidToExternalUserIdAsync` — unionid 换外部联系人（已添加客户）

```csharp
Task<UnionidToExternalUserIdResponse> UnionidToExternalUserIdAsync(
    [Body] UnionidToExternalUserIdRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/unionid_to_external_userid`。

**请求参数**

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `unionid` | `UnionId` | `string` | 是 | 微信客户 unionid |
| `openid` | `OpenId` | `string` | 是 | 微信客户 openid（须与 unionid 同主体、同小程序） |
| `corpid` | `CorpId` | `string?` | 否 | 指定换取的企业；不填拉取所有企业 |
| `mass_call_ticket` | `MassCallTicket` | `string?` | 否 | 大批量调用凭据（**凭据，勿落日志**）；有效时可豁免 §2.2 的三档限流 |

**响应参数**：`external_userid_info[]` → `ExternalUserIdInfo`：`corpid`（`string?`）、`external_userid`（`string?`）。

### E5. `UnionidToPendingIdAsync` — unionid 换 pending_id（未添加客户）

```csharp
Task<UnionidToPendingIdResponse> UnionidToPendingIdAsync(
    [Body] UnionidToPendingIdRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/unionid_to_pending_id`；请求 `unionid` + `openid`（均必填）。
- 响应 `pending_id`（`string?`）：**临时外部联系人 ID，有效期 90 天、共享应用内唯一**。

### E6. `ExternalUserIdToPendingIdAsync` — external_userid 批量换 pending_id

```csharp
Task<ExternalUserIdToPendingIdResponse> ExternalUserIdToPendingIdAsync(
    [Body] ExternalUserIdToPendingIdRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/corpgroup/batch/external_userid_to_pending_id`（路由含 `batch/`，仍属本族） |

**请求参数**

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `external_userid` | `ExternalUserIds` | `List<string>?` | 是 | **单批 ≤100**；注意 JSON 名是**单数** `external_userid` |
| `chat_id` | `ChatId` | `string?` | 否 | 传入时只检查**群主**是否在可见范围，并忽略该群以外的 `external_userid` |

**响应参数**：`result[]` → `PendingIdResultItem`：`external_userid`（`string?`）、`pending_id`（`string?`，未成为客户时返回，90 天有效）。

**调用示例**（E4–E6 的映射串联；片段形态，仅演示 API 形状）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.CorpGroup;
// 片段自足前置：与上面 E1 示例使用同一个自建子接口实例。
var corpGroup = provider.GetRequiredService<IWechatWorkInternalCorpGroupService>();

// ① 粉丝阶段先占位：unionid -> pending_id（90 天内要完成下一步，否则需重新生成）
var pending = await corpGroup.UnionidToPendingIdAsync(
    new UnionidToPendingIdRequest { UnionId = "<unionid>", OpenId = "<openid>" }, cancellationToken);
WechatWorkException.ThrowIfFailed(pending, "/cgi-bin/corpgroup/unionid_to_pending_id");

// ② 客户阶段回填：external_userid -> pending_id（单批 ≤100，JSON 键是单数 external_userid）
var mapped = await corpGroup.ExternalUserIdToPendingIdAsync(
    new ExternalUserIdToPendingIdRequest { ExternalUserIds = new List<string> { "<external_userid>" } },
    cancellationToken);
WechatWorkException.ThrowIfFailed(mapped, "/cgi-bin/corpgroup/batch/external_userid_to_pending_id");

foreach (PendingIdResultItem item in mapped.Result ?? new List<PendingIdResultItem>())
{
    logger.LogInformation("映射 external_userid={ExternalUserId} pending_id={PendingId}",
        item.ExternalUserId, item.PendingId);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E2 返回的 `access_token` vs 基座托管的 `access_token` | 前者是**下级/下游企业**凭证、SDK 不缓存；后者是当前应用上下文令牌 | 以为换完就能自动用于 E3 → 权限错 |
| E3 的路由前缀 `/cgi-bin/miniprogram/` | 归属本域，不属 MiniProgram 线 | 按前缀找线 → 找不到端点 |
| E6 的 JSON 键 `external_userid` vs 属性名 `ExternalUserIds` | 键是**单数**、属性是复数集合 | 手拼 JSON 用 `external_userids` → 官方不认 |
| `pending_id` vs `external_userid` | pending_id 是 90 天有效的临时 ID，客户未加入时才有 | 当永久主键存储 → 90 天后失效 |
| E1 的 `ending` vs `next_cursor` | 两个分页终止信号 | 只判一个 → 死循环或漏尾页 |
| 第三方子接口 | 类型化面仅 1（不继承中间父接口，CG2 反向断言） | 在第三方注入面上找 E2–E6 → 不存在 |
| 旧「企业互联」文档号 93403/93359/93355 | 与新树同路由同契约的**别名**，不是新端点 | 按文档号数量加端点 → 契约重复 |
| `93404` 获取下级企业域付费版本信息 | **不是 HTTP 端点**，本仓建模为 `Agent.IsSharedFromOtherCorp`（`bool`）字段 | 据此加端点 → 官方无此契约 |
| E4/E5/E6 的频率 | 按**上游企业**维度三档限流，且 E4 的 `mass_call_ticket` 只豁免这三档、不豁免基础频率 | 用 ticket 绕开一切限流 → 仍被拒 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功仍要看 `ending` / `next_cursor` 决定是否续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合（针对**调用方自身**令牌；E2 换来的外部凭证不在基座自愈范围内） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |
| 非 0（业务） | 官方业务码 | 非共享应用、无客户联系权限、主体不一致、超出限流等 | **具体码值接口注释未留档**（见 §7） |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E1/E2/E3 的频率 | 注释只留 E4/E5/E6 的三档限流；其余端点数值 QPS **未逐项核验** | 官方 95813/95816/95817 |
| 各端点业务错误码 | 注释未登记码值（含限流命中、主体不一致） | 官方对应页 |
| `business_type` 取值全集 | 注释留档 `0` 企业互联/局校互联、`1` 上下游；其它值是否存在**未逐项核验** | 官方 95813 |
| E2 凭证的续期与并发口径 | 注释只给 `expires_in` 与 512 字节上限，未给「同一 corpid+agentid 重复换取」的行为 | 官方 95816 |
| `mass_call_ticket` 的签发端点 | 本域只消费该参数，签发面归属**未在本域核验**（邻域 ExternalContact） | 见域 README 缺口表 |
| 代开发对 `97441`/`97442` 的可达性 | 属通讯录族端点，代开发文档树无对应页 ⇒ 未逐项核验，见 [CorpGroupContacts.md](./CorpGroupContacts.md) | 官方代开发树 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md)（§1 应用类型面不等宽、§2 `gettoken` 不进令牌基座、§4 unionid 三档频率与 90 天、§8 旧文档号别名）
- 同域：[CorpGroupContacts.md](./CorpGroupContacts.md)（上下游通讯录管理）· [CorpGroupRules.md](./CorpGroupRules.md)（对接规则）
- 邻域：外部联系人体系见 `Interfaces/ExternalContact/`（`external_userid` 的签发与读取侧）
- 官方页：[获取应用共享信息（自建 95813）](https://developer.work.weixin.qq.com/document/path/95813) · [获取下级企业 access_token（95816）](https://developer.work.weixin.qq.com/document/path/95816) · [上下游关联客户信息（95818）](https://developer.work.weixin.qq.com/document/path/95818)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
