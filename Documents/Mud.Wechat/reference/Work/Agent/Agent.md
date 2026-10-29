# 获取应用（应用详情 / 应用列表 / 设置应用 / 应用权限 / 应用管理员）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Agent` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | 目录 [Interfaces/Agent/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/)：[IWechatWorkAgentService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentService.cs) · [IWechatWorkAgentService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentService_Internal.cs) · [IWechatWorkAgentThirdPartyProviderService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentThirdPartyProviderService.cs) · [IWechatWorkAgentService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentService_Provider.cs) · [IWechatWorkAgentService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentService_ThirdParty.cs) |
| DTO 源文件 | [DataModels/Agent/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/)：本族 12 型 —— [GetAgentResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentResponse.cs) · [AgentAllowUserinfos](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentAllowUserinfos.cs) · [AgentAllowUser](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentAllowUser.cs) · [AgentAllowPartys](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentAllowPartys.cs) · [AgentAllowTags](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentAllowTags.cs) · [GetAgentListResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentListResponse.cs) · [AgentItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentItem.cs) · [SetAgentRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/SetAgentRequest.cs) · [GetAgentPermissionsResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentPermissionsResponse.cs) · [AgentPermissionExtItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentPermissionExtItem.cs) · [GetAgentAdminListResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentAdminListResponse.cs) · [AgentAdminItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentAdminItem.cs)（目录内共 30 型，其余属工作台 / 菜单 / 迁移三族） |
| 契约守卫 | [WechatAgentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs)（AG1 路由与动词 / AG2 层级与端点计数 / AG3 令牌绑定 / AG4 DTO 登记 / AG5 字段陷阱）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）· 零声明路由 [WechatNonContractRouteZeroDeclarationContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs)（RD1） |
| 官方文档 | 自建 90227（E1/E2）· 90228（E3）；第三方 90363（E1/E2）· 99052（E4）· 100073（E5）；服务商代开发 96448（E1/E2）· 99108（E4）· 100072（E5）。文档号一律取自接口 XML 注释的 `<remarks>`；页面最后更新日期**本次未取文**，见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 4 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AG1 `AgentBaseRoutes` / `AgentThirdPartyProviderRoutes` / `AgentSetRoutes` 逐路由逐动词 + AG2 两级继承链集合断言 + AG3 13 接口令牌绑定 + AG4 30 型上下文登记 + AG5 字段拼写与「权限扩展面」形态断言 + 五个接口与 12 个 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 XML 注释与 2026-10-18 核验留档；凡只有官方页能支撑的项一律进 §7 |

## 1. 这个接口族做什么

本族是「应用管理」域里**读应用自身元数据、改自建应用配置**的那一面：查一个应用的完整配置（E1）、列出当前凭据能看到的应用（E2）、改自建应用的名称/头像/主页等字段（E3）、查第三方或代开发应用被授予的权限（E4）、查该应用的管理员名单（E5）。

使用时机是两类：**服务商侧做应用主页或后台**——用户进来后要判断「这个企业给我授了哪些权限」「这个人是不是应用管理员」，先跑 E4/E5；**企业侧运维脚本**——批量核对或修改自建应用配置，用 E1/E2 回读、E3 写入。

本族五个端点消费的都是同一个业务令牌类型 `Wechat.AccessToken`，但**凭据来源按应用类型分裂**：自建应用用应用自身令牌，第三方/代开发用授权企业级令牌（scope = `authCorpId`），后者调用前必须先切到目标企业作用域（见 §2.1）。

**不该用本族的场景**：改应用在工作台的展示内容走同域工作台族（[AgentWorkbench.md](./AgentWorkbench.md)），配自定义菜单走菜单族（[AgentMenu.md](./AgentMenu.md)），把自建应用迁成代开发走迁移族（[AgentMigration.md](./AgentMigration.md)）；「获取应用共享信息」是上下游 `CorpGroup` 域的端点（`/cgi-bin/corpgroup/corp/list_app_share_info`），名字里也有「应用」但不属本族。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建 `WechatAppType.Internal`（E1~E3）· 第三方 `WechatAppType.ThirdParty` 或代开发 `WechatAppType.Provider`（E1/E2/E4/E5） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册由 `WechatAppManager` 同样校验 |
| 必填配置属性 | 自建：`CorpId` + `AgentSecret`；第三方 / 代开发：`CorpId`（**服务商企业**的 CorpId）+ `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败，不留到首次调用） |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | 自建子接口 `WechatTokenManagerKeys.InternalAccessToken`（`Wechat.AccessToken@Internal`）· 第三方/代开发子接口 `WechatTokenManagerKeys.CorpAccessToken`（`Wechat.AccessToken@Corp`）· **两个父接口不声明归属键**（AG3 只锁 `TokenType` / 注入模式 / 参数名，归属键由 TO1 按接口名族别锁定） | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；错配抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |

`access_token` 由 SDK 自动换取、缓存并按官方有效期刷新，**开发者不接触令牌值**（自建走 `corpsecret`、第三方走 `get_corp_token`、代开发以 `permanent_code` 走 `gettoken`）。

第三方 / 代开发调本族前必须进入目标授权企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时快照并写入「应用 + 企业」两级上下文、释放时逆序还原，嵌套安全）。第三个参数是该企业的永久授权码（代开发必传，第三方可省——缺省时由 `IWechatCorpAuthStore` 提供）。`permanent_code` 是凭据：本文与示例只写参数名，不得进日志、遥测或异常消息。

### 2.2 权限与业务前置

以下逐条出自接口与 DTO 的 XML 注释（官方页本次未重取）：

- E1/E2：**企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用**——「应用列表」不是全企业应用清单，而是当前凭据可见集。
- E3：**仅企业可调用**（第三方以及代开发自建应用不可调用）；只能设置当前凭证对应的应用；应用名称不超过 32 个 utf8 字符；应用详情 4 至 120 个 utf8 字符；可信域名需通过域名所有权校验，否则 jssdk 受限（错误码 85005）；应用主页 url 必须以 http 或 https 开头。
- E4：官方原文「该API用于获取企业授权的代开发应用或第三方应用或获客助手组件的权限详情」——即返回当前授权关系下应用需要申请的权限列表。
- E5：官方原文「第三方服务商可以用此接口获取授权企业中某个第三方应用或者代开发应用的管理员列表（不包括外部管理员），以便服务商在用户进入应用主页之后根据是否管理员身份做权限的区分」；并留档「成员授权模式下，不返回管理员列表」。
- 频率：E4/E5 的接口注释明确留档「两页正文均未给出本端点的独立频率数字，按附录『访问频率限制』默认口径执行（每企业单个 cgi/api 1 万次/分、15 万次/小时；第三方提供商每 IP 4 万次/分、120 万次/小时）」——这是**附录默认值而非端点级数值**。E1~E3 无任何频率留档 ⇒ §7。

个人信息约束：E1 返回的可见范围 `userid` 列表与 E5 的管理员 `userid` 名单都属成员身份信息，不得写入日志、遥测或异常消息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `GetAgentAsync` | 开放 | 开放 | 开放 |
| E2 `GetAgentListAsync` | 开放 | 开放 | 开放 |
| E3 `SetAgentAsync` | **开放** | 未开放 | 未开放（「代开发自建应用」亦不可调用） |
| E4 `GetAgentPermissionsAsync` | **未开放**（自建应用文档树无该页面） | 开放 | 开放 |
| E5 `GetAgentAdminListAsync` | **未开放**（同上） | 开放 | 开放 |

上表就是 §3 继承链形状的来源：三类一致的 E1/E2 收在公共父接口，仅自建的 E3 落在自建子接口，仅第三方/代开发的 E4/E5 落在**中间层父接口**。开放面差异由 AG2 锁定——自建子接口集合、中间层子接口集合、以及「自建子接口不得继承中间层」三条断言任一漂移即打红。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAgentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空、**不声明归属键**） | 2（E1/E2） | 否 |
| `IWechatWorkInternalAgentService` | `Mud.Wechat.Work` | 自建子接口，**直接**继承公共父接口 | 1（E3）+ 继承 2 | 是（`@Internal`） |
| `IWechatWorkAgentThirdPartyProviderService` | `Mud.Wechat.Work.Interfaces` | 中间层父接口（`IsAbstract = true`、注册组恒空、**不声明归属键**） | 2（E4/E5）+ 继承 2 | 否 |
| `IWechatWorkThirdPartyAgentService` | `Mud.Wechat.Work` | 第三方子接口（零差异端点空标记，继承中间层） | 0 + 继承 4 | 是（`@Corp`） |
| `IWechatWorkProviderAgentService` | `Mud.Wechat.Work` | 服务商代开发子接口（零差异端点空标记，继承中间层） | 0 + 继承 4 | 是（`@Corp`） |

继承链是**两级**，不是一根直线：

```
IWechatWorkAgentService                    2 端点（E1/E2，三类应用一致）
├─ IWechatWorkInternalAgentService         +1（E3 设置应用）           ← 自建，直接挂父接口
└─ IWechatWorkAgentThirdPartyProviderService  +2（E4/E5）              ← 中间层，仅第三方/代开发
   ├─ IWechatWorkThirdPartyAgentService    +0（空标记）
   └─ IWechatWorkProviderAgentService      +0（空标记）
```

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Agent` / `AddAgentApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Agent`（三个应用类型子接口都挂同一注册组，生成器产出 `AddAgentWebApiHttpClient()`；两个父接口注册组恒空）
- 端点索引：**E1** `GetAgentAsync` · **E2** `GetAgentListAsync` · **E3** `SetAgentAsync` · **E4** `GetAgentPermissionsAsync` · **E5** `GetAgentAdminListAsync`

**为什么 `IWechatWorkInternalAgentService` 上没有 E4/E5**：这两个端点的官方页面只存在于「第三方应用开发」与「服务商代开发」两棵章节树下，企业自建应用文档树无对应页面。把中间层挂到自建子接口上，等于让自建类型化面重新暴露官方未开放的端点。AG2 因此单独断言 `IWechatWorkAgentThirdPartyProviderService.IsAssignableFrom(typeof(IWechatWorkInternalAgentService)).Should().BeFalse(...)`，中间层的子接口集合也被锁定为恰为 `IWechatWorkProviderAgentService` + `IWechatWorkThirdPartyAgentService`。

代价是**读法上的不对称**：同域的「应用详情」在自建子接口上，「应用权限」却只在第三方/代开发子接口上——需要同时用两侧能力时，按应用类型注入不同接口，而不是去找一个「万能接口」。

本族 12 个 DTO 全部登记进 `AgentJsonContext`（`internal`，示例不可引用；AG4 断言本域 30 型全量登记、`SerializerClassName` 统一为 `Agent`）。

---

## 4. 端点

五个端点的凭据面一致（`Wechat.AccessToken` + Query 注入 `access_token`），差别只在归属键：E3 声明在自建子接口上（`@Internal`），E4/E5 声明在第三方/代开发子接口上（`@Corp`）。以下每节只列端点自身差异。

### E1. `GetAgentAsync` — 获取指定的应用详情

**签名**

```csharp
Task<GetAgentResponse> GetAgentAsync(
    [Query("agentid")] int agentid,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/agent/get` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/agent/get?access_token=ACCESS_TOKEN&agentid=AGENTID` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成）；`agentid` 也是 **Query** 参数，不进请求体 |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` / 第三方与代开发 `Wechat.AccessToken@Corp`（取决于注入的是哪个子接口） |
| 请求体 | 无（生成与复刻客户端时不要造包体） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `agentid` | `agentid` | `int`（方法参数，`[Query("agentid")]`） | 是 | 应用 id；接口注释注明「可在应用管理设置页查看」。取值范围与上限本仓未留档 ⇒ §7 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`agentid` 传一个不存在的值照样发出，错误以 `errcode` 回来。

```
GET /cgi-bin/agent/get?access_token=ACCESS_TOKEN&agentid=1000002
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `agentid` | `Agentid` | `int?` | 企业应用 id（**全小写、无下划线**） |
| `name` | `Name` | `string?` | 应用名称 |
| `square_logo_url` | `SquareLogoUrl` | `string?` | 应用方形头像 url |
| `description` | `Description` | `string?` | 应用详情 |
| `allow_userinfos` | `AllowUserinfos` | `AgentAllowUserinfos?` | 可见范围（人员）。**是对象**，其 `user` 数组元素为对象（含 `userid`） |
| `allow_partys` | `AllowPartys` | `AgentAllowPartys?` | 可见范围（部门）。**是对象**，其 `partyid` 是 `int` 数组 |
| `allow_tags` | `AllowTags` | `AgentAllowTags?` | 可见范围（标签）。**是对象**，其 `tagid` 是 `int` 数组 |
| `close` | `Close` | `int?` | 应用是否被停用：`0` 未停用、`1` 已停用 |
| `redirect_domain` | `RedirectDomain` | `string?` | 企业应用可信域名 |
| `report_location_flag` | `ReportLocationFlag` | `int?` | 是否打开地理位置上报：`0` 不上报、`1` 进入会话上报 |
| `isreportenter` | `Isreportenter` | `int?` | 是否上报用户进入应用事件：`0` 不接收、`1` 接收。**字段全小写无驼峰** |
| `home_url` | `HomeUrl` | `string?` | 应用主页 url |
| `customized_publish_status` | `CustomizedPublishStatus` | `int?` | 代开发自建应用发布状态：`0` 待开发、`1` 开发中、`2` 已上线、`3` 存在未上线版本。**仅代开发自建应用返回** |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "agentid": 1000002,
  "name": "示例应用",
  "square_logo_url": "https://example.com/logo.png",
  "description": "示例应用详情",
  "allow_userinfos": { "user": [{ "userid": "mud-USERID-FAKE-0001" }] },
  "allow_partys": { "partyid": [1, 2] },
  "allow_tags": { "tagid": [10] },
  "close": 0,
  "redirect_domain": "example.com",
  "report_location_flag": 0,
  "isreportenter": 1,
  "home_url": "https://example.com/app",
  "customized_publish_status": 2
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                          // IWechatWorkInternalAgentService / AddWechatWorkServices / AddAgentApi
using Mud.Wechat.Work.Abstractions;             // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.DataModels.Agent;         // GetAgentResponse 及其可见范围嵌套类型

// 宿主约定：builder / config / provider / logger / cancellationToken 由宿主提供
//（builder 为 IServiceCollection、config 为 IConfiguration、provider 为已 Build 的 IServiceProvider）；
// 本示例按 ASP.NET Core Web SDK 形态编写，Microsoft.Extensions.DependencyInjection 与
// Microsoft.Extensions.Logging 的 using 由其隐式提供。

// ① 注册自建应用：CorpId + AgentSecret 是 Internal 类型的互斥必填组合，缺任一项在注册期即抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
    o.AgentId = config["Wechat:AgentId"];          // 配置项是 string（仅展示/路由用途），业务调用传 int
});

// ② 注册 Agent 模块：本族 5 个端点全部随该模块注册，无需逐端点在模块上追加。
builder.Services.AddWechatWorkServices(b => b.AddAgentApi());

// ③ 取可注入的自建子接口。公共父接口不注册 DI，解析不到。
var agentService = provider.GetRequiredService<IWechatWorkInternalAgentService>();
var agentId = int.Parse(config["Wechat:AgentId"]!);

// ④ 调用：agentid 走 Query，既不在 URL 路径上，也不在请求体里。
var agent = await agentService.GetAgentAsync(agentId, cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!agent.IsSuccess)
{
    logger.LogError("获取应用详情失败 {ErrorCode}: {ErrorMessage}", agent.ErrorCode, agent.ErrorMessage);
    throw new InvalidOperationException($"获取应用详情失败：{agent.ErrorCode}");
}

// ⑥ 取业务字段：allow_* 是「对象包数组」，不是裸数组；userid 属成员身份，不得写日志。
var visibleUserIds = (agent.AllowUserinfos?.User ?? new List<AgentAllowUser>())
    .Select(u => u.Userid)
    .OfType<string>()
    .ToList();
var visiblePartyIds = agent.AllowPartys?.Partyid ?? new List<int>();
var visibleTagIds = agent.AllowTags?.Tagid ?? new List<int>();
var isDisabled = agent.Close == 1;                  // 0 未停用 / 1 已停用
int? publishStatus = agent.CustomizedPublishStatus;  // 仅代开发自建应用返回，其余为 null
```

**本端点注意事项**

- 第三方/代开发调用时注入的是 `IWechatWorkThirdPartyAgentService` / `IWechatWorkProviderAgentService`，且必须先 `UseCorpScope` 切到目标企业（片段见 E4/E5）。
- `customized_publish_status` 只在代开发自建应用上出现：拿它判断「应用是否上线」对自建与第三方应用恒为 `null`，不是业务失败。
- `allow_userinfos` / `allow_partys` / `allow_tags` 三者是**对象**（`{"user":[{...}]}` / `{"partyid":[...]}` / `{"tagid":[...]}`）；按裸数组解析会整段丢字段。

### E2. `GetAgentListAsync` — 获取 access_token 对应的应用列表

**签名**

```csharp
Task<GetAgentListResponse> GetAgentListAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/agent/list` |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 请求参数 | **无任何业务参数**（AG1 断言方法参数恰为 `CancellationToken`），只带 Query 令牌 |
| 请求体 | 无 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入；本端点无其它参数 |

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `agentlist` | `Agentlist` | `List<AgentItem>?` | 应用列表（**全小写、无下划线**，响应体只有这一个业务字段） |
| `agentlist[].agentid` | `Agentid` | `int?` | 企业应用 id |
| `agentlist[].name` | `Name` | `string?` | 企业应用名称 |
| `agentlist[].square_logo_url` | `SquareLogoUrl` | `string?` | 企业应用方形头像 url |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "agentlist": [{ "agentid": 1000002, "name": "示例应用", "square_logo_url": "https://example.com/logo.png" }]
}
```

调用形态同 E1，去掉 `agentid`：

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Agent;

// 前置：AppType = Internal 已注册 + AddWechatWorkServices(b => b.AddAgentApi()) 已装配。
var agentService = provider.GetRequiredService<IWechatWorkInternalAgentService>();

var list = await agentService.GetAgentListAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(list, "/cgi-bin/agent/list");

var applications = list.Agentlist ?? new List<AgentItem>();        // 元素只有 id / 名称 / 方形头像
```

**本端点注意事项**

- 列表项**只有三个字段**：要可见范围、可信域名、停用状态必须按 `agentid` 逐个调 E1，不要指望列表页回传。
- 返回集是「当前凭据可见的应用」，不是全企业应用清单（§2.2 第一条）。

### E3. `SetAgentAsync` — 设置应用（仅自建）

**签名**

```csharp
Task<WechatWorkResponse> SetAgentAsync(
    [Body] SetAgentRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/set` |
| 凭据参数 | `access_token`（Query 注入）+ 归属键 `Wechat.AccessToken@Internal` |
| 适用应用类型 | **仅企业自建应用**（AG1 断言本端点声明在 `IWechatWorkInternalAgentService` 上；第三方/代开发子接口没有该方法） |
| 请求体编码 | `application/json`，源生成序列化（`AgentJsonContext`，AG4 锁定登记） |
| 返回类型 | 裸 `WechatWorkResponse`（只有 `errcode` / `errmsg`）——AG1 断言本端点**不得**新建空响应 DTO |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Agentid` | `agentid` | `int`（**非空**） | 是 | 企业应用的 id。本 DTO 是全族唯一用非可空 `int` 的请求字段，其余业务字段均可空 |
| `request.ReportLocationFlag` | `report_location_flag` | `int?` | 否 | 是否打开地理位置上报：`0` 不上报、`1` 进入会话上报 |
| `request.LogoMediaid` | `logo_mediaid` | `string?` | 否 | 应用头像的 mediaid（经素材管理接口上传图片获得）。**拼写是 `mediaid` 不是 `media_id`** |
| `request.Name` | `name` | `string?` | 否 | 应用名称，不超过 32 个 utf8 字符 |
| `request.Description` | `description` | `string?` | 否 | 应用详情，4 至 120 个 utf8 字符 |
| `request.RedirectDomain` | `redirect_domain` | `string?` | 否 | 可信域名；需通过域名所有权校验，否则 jssdk 受限（错误码 85005） |
| `request.Isreportenter` | `isreportenter` | `int?` | 否 | 是否上报用户进入应用事件：`0` 不接收、`1` 接收 |
| `request.HomeUrl` | `home_url` | `string?` | 否 | 应用主页 url，必须以 http 或 https 开头 |

```json
{
  "agentid": 1000002,
  "report_location_flag": 0,
  "logo_mediaid": "MEDIAID-FAKE-0001",
  "name": "示例应用",
  "description": "示例应用详情",
  "redirect_domain": "example.com",
  "isreportenter": 1,
  "home_url": "https://example.com/app"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功 |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;

// 前置：AppType = Internal 已注册 + AddWechatWorkServices(b => b.AddAgentApi()) 已装配。
var internalAgent = provider.GetRequiredService<IWechatWorkInternalAgentService>();
var agentId = int.Parse(config["Wechat:AgentId"]!);

var updated = await internalAgent.SetAgentAsync(
    new SetAgentRequest
    {
        Agentid = agentId,                            // 官方必填；C# 侧非可空 int
        Name = "示例应用",                             // ≤32 个 utf8 字符
        Description = "由运维脚本设置的应用详情",        // 4~120 个 utf8 字符
        HomeUrl = "https://example.com/app",          // 必须以 http/https 开头
        // 未赋值的字段（logo_mediaid / redirect_domain / isreportenter …）是「保持原值」还是被覆盖，
        // 接口注释未留档（§7）：改配置前先用 E1 回读，改完再回读校验。
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/agent/set");   // 无业务负载，只能判 errcode
```

**本端点注意事项**

- 第三方/代开发**没有**这个方法：`IWechatWorkThirdPartyAgentService` / `IWechatWorkProviderAgentService` 上编译期就不存在 `SetAgentAsync`，不要去父接口上找。
- 官方把除 `agentid` 外的 7 个业务字段都列为「非必填」，但 E3 是**写操作**：字符数、域名所有权、`home_url` 协议、`logo_mediaid` 有效性都在官方侧校验，SDK 不预检。
- 响应无业务字段，成功与否只能看 `errcode`；失败不会自动回滚已生效的字段。

### E4. `GetAgentPermissionsAsync` — 获取应用权限详情（仅第三方 / 代开发）

**签名**

```csharp
Task<GetAgentPermissionsResponse> GetAgentPermissionsAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/get_permissions` |
| 凭据参数 | `access_token`（Query 注入）+ 归属键 `Wechat.AccessToken@Corp` |
| 适用应用类型 | 第三方 · 服务商代开发（自建应用文档树无该页面，故不在 `IWechatWorkInternalAgentService` 上） |
| 请求体 | **无业务字段**（AG1 断言方法参数恰为 `CancellationToken`；接口注释说明「官方请求体不含业务字段，故方法不声明 `[Body]` 参数」） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入；官方「参数说明」表只列 `access_token`，无任何业务入参 |

```
POST /cgi-bin/agent/get_permissions?access_token=ACCESS_TOKEN
（无请求体）
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `app_permissions` | `AppPermissionList` | `List<string>?` | 权限列表。元素是「分类:子分类:权限」形态字符串，取值字典在官方页附录「应用权限说明」中按分类给出，且区分「仅第三方应用返回」与「仅代开发应用返回」；附录随官方扩充，**SDK 不做枚举建模** |
| `app_permissions_ext` | `AppPermissionExtList` | `List<AgentPermissionExtItem>?` | 权限扩展信息。官方口径：「若存在应用权限为二次授权并且有时效时，返回该字段列表」——**条件返回** |
| `app_permissions_ext[].permission_name` | `PermissionName` | `string?` | 权限名称（与 `app_permissions` 元素同形态） |
| `app_permissions_ext[].expire_time` | `ExpireTime` | `long?` | 权限过期时间，**秒级时间戳**；「若权限不会过期则 `expire_time` 字段值为 0」 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "app_permissions": ["企业客户:客户基础信息", "通讯录:成员基础信息:读取"],
  "app_permissions_ext": [{ "permission_name": "企业客户:客户基础信息", "expire_time": 1798761600 }]
}
```

**本端点注意事项**

- 两个字段的**存在性互相独立**：`app_permissions` 非空不代表 `app_permissions_ext` 一定存在；`app_permissions_ext` 为 `null` 是「没有二次授权时效」的正常形态，不是数据缺失。
- `expire_time == 0` 表示**不会过期**（永久有效），不是「已过期」也不是缺失值；判失效只能拿非 0 值与当前时间比较。

### E5. `GetAgentAdminListAsync` — 获取应用管理员列表（仅第三方 / 代开发）

**签名**

```csharp
Task<GetAgentAdminListResponse> GetAgentAdminListAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/get_admin_list` |
| 凭据参数 | `access_token`（Query 注入）+ 归属键 `Wechat.AccessToken@Corp` |
| 适用应用类型 | 第三方 · 服务商代开发（自建应用文档树无该页面） |
| 请求体 | **无业务字段**（AG1 断言同上） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入；官方「参数说明」表只列 `access_token` |

```
POST /cgi-bin/agent/get_admin_list?access_token=ACCESS_TOKEN
（无请求体）
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1；官方注明 `errcode` 为 0 才表示「正常返回，可读取 admin 的管理员列表」 |
| `admin` | `AdministratorList` | `List<AgentAdminItem>?` | 管理员列表。**字段名不带 `_list` 后缀**；官方口径「不包括外部管理员」「成员授权模式下，不返回管理员列表」 |
| `admin[].userid` | `UserId` | `string?` | 成员账号 |
| `admin[].auth_type` | `AuthType` | `int?` | 该管理员对应用的权限：`0` 发消息权限、`1` 管理权限 |

```json
{ "errcode": 0, "errmsg": "ok", "admin": [{ "userid": "mud-USERID-FAKE-0001", "auth_type": 1 }] }
```

**E4 + E5 片段**（第三方场景，含企业作用域切换）

```csharp
using Mud.Wechat.Work;                              // IWechatWorkThirdPartyAgentService
using Mud.Wechat.Work.Abstractions.Authentication;  // IWechatAppContextSwitcher / UseCorpScope
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;             // AgentPermissionExtItem / AgentAdminItem

// 前置：AddWechatApp（AppType = ThirdParty）+ AddWechatWorkServices(b => b.AddAgentApi()) 已在宿主装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var thirdPartyAgent = provider.GetRequiredService<IWechatWorkThirdPartyAgentService>();
var authCorpId = config["Wechat:AuthCorpId"]!;

// 企业级令牌一企一份（scope = authCorpId）：不进目标企业作用域就取不到该企业令牌。
// 第三方场景永久授权码可省（缺省由 IWechatCorpAuthStore 提供）；代开发须传 permanent_code，
// 该值是凭据 —— 只从配置读取，不进日志/遥测/异常消息。
using (switcher.UseCorpScope("thirdparty-a", authCorpId))
{
    var permissions = await thirdPartyAgent.GetAgentPermissionsAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(permissions, "/cgi-bin/agent/get_permissions");

    var granted = permissions.AppPermissionList ?? new List<string>();
    var expiring = (permissions.AppPermissionExtList ?? new List<AgentPermissionExtItem>())
        .Where(p => p.ExpireTime is > 0)        // 0 = 不会过期；非 0 才是秒级失效时刻
        .ToList();

    var admins = await thirdPartyAgent.GetAgentAdminListAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(admins, "/cgi-bin/agent/get_admin_list");

    // 成员授权模式下官方不返回 admin：AdministratorList 为 null 是正常路径，不是失败。
    var managerUserIds = (admins.AdministratorList ?? new List<AgentAdminItem>())
        .Where(a => a.AuthType == 1)            // 1 = 管理权限；0 = 发消息权限（不是「无权限」）
        .Select(a => a.UserId)
        .OfType<string>()
        .ToHashSet(StringComparer.Ordinal);
}
```

**本端点注意事项**

- `admin` 字段名**不带 `_list`**；按 `admin_list` 取名会拿到空集合。
- 「成员授权模式下不返回管理员列表」⇒ `AdministratorList == null` 与「无管理员」在报文上不可区分，业务侧不要把两者分开处理。
- 与旧服务商形态 `/cgi-bin/service/get_admin_list` 同名不同族：那一支以 `suite_access_token` 调用、元素多回传 `open_userid`，官方已注明不再维护；本仓**零声明**该路由（RD1 把 35 条非契约路由在 `Src/` 内断言零命中），两侧元素类型不共用。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 自建子接口 ⇄ 第三方/代开发子接口 | E4/E5 只在 `IWechatWorkThirdPartyAgentService` / `IWechatWorkProviderAgentService` 上（经中间层 `IWechatWorkAgentThirdPartyProviderService` 继承），自建子接口**不继承中间层**；E3 只在自建子接口上 | 想在自建应用上调 E4/E5 会找不到方法；改注入中间层父接口则该接口不注册 DI，解析失败。这正是不对称继承链要表达的事实，不要去「补齐」 |
| E1 的 `agentid`（Query）⇄ E3 的 `agentid`（请求体） | 同一域里 `agentid` 有两种承载位置：E1 是 `[Query("agentid")] int` 方法参数，E3 是 `SetAgentRequest.Agentid` 请求体字段 | 把 E1 的 id 塞进包体（或反向）官方不认，表现为 `errcode` 报参数缺失而不是「查不到应用」 |
| E4/E5 的 POST 形态 ⇄ 「查询就该用 GET」 | 两支都是官方 **POST**，且**请求体无任何业务字段**；SDK 方法签名只有 `CancellationToken`（AG1 逐个断言「恰一个参数且类型是 `CancellationToken`」） | 顺手加 `[Body]` 或改成 GET 会打红 AG1；自行拼客户端时带包体会与官方契约不符 |
| `admin`（E5）⇄ `/cgi-bin/service/get_admin_list` | 前者 `access_token` 企业级令牌、元素 `userid` + `auth_type`；后者 `suite_access_token`、元素多回传 `open_userid`，官方已不维护、本仓零声明 | 复用旧接口的元素类型会把 `open_userid` 解析丢掉，得到「userid 全为空」的名单 |
| `app_permissions_ext` ⇄ `app_permissions` | 后者恒为「已授权限清单」，前者是**条件返回**的二次授权时效扩展；`expire_time == 0` 表示**永不过期** | 以「`app_permissions_ext` 为空 ⇒ 无权限」判定会误判；把 `0` 当过期时间戳比较会得到「1970 年已过期」，从而错误地禁用整个应用 |
| `allow_userinfos` / `allow_partys` / `allow_tags` ⇄ 裸数组 | 三者都是**对象包数组**：`{"user":[{"userid":…}]}`、`{"partyid":[…]}`、`{"tagid":[…]}`；部门与标签数组元素是 `int` | 按 `string[]` / `int[]` 解析（或手写 JSON 取 `allow_partys[0]`）会整段丢可见范围，误判为「应用对所有人生效」 |
| `isreportenter` ⇄ `report_location_flag` | 两个「上报」开关字段名风格完全不同：前者**全小写无驼峰**，后者是下划线词；前者出现在 E1 响应与 E3 请求，后者只表示地理位置上报 | 按驼峰猜 `isReportEnter` 会在抓包对账与手写报文时全部落空（SDK 侧已按官方原文映射，只有手拼时才踩） |
| `customized_publish_status` ⇄ 通用发布状态 | 官方注明**仅代开发自建应用返回**（`0` 待开发 / `1` 开发中 / `2` 已上线 / `3` 存在未上线版本） | 对自建或第三方应用读该字段恒为 `null`，当成「应用未上线」会误停用 |
| 本族 E1 ⇄ `CorpGroup` 域的「获取应用共享信息」 | 后者路由 `/cgi-bin/corpgroup/corp/list_app_share_info`，属上下游域；本族全部路由都在 `/cgi-bin/agent/` 段下 | 按名字把两支并进同一族会打红 AG1 的 14 条路由清单（本域路由集合被逐条锁定），且调用方会引错域与令牌面 |

另外三条 SDK 侧行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改用 Header）。G5 把「以 Query 注入令牌」的接口名收敛为白名单，本族五个接口都在白名单内；`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo。
- **不自动判错、不做参数校验**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`；字符数、域名校验、`agentid` 合法性都在官方侧。
- **归属键与本族方法面上的「同一个 `access_token`」不是一回事**：`Wechat.AccessToken@Internal` 与 `Wechat.AccessToken@Corp` 声明在**子接口**上，父接口不声明。用自建应用上下文消费第三方子接口会在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；E5 另需注意「`errcode == 0` 但 `admin` 不返回」是官方明示的正常形态 |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效 / 无效 | 本族失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效（部分接口） | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少 `access_token` 参数 / 凭据缺失（永久授权码无效等场景） | 不自愈：装配或作用域缺陷信号（多为未切 `UseCorpScope` 或 `permanent_code` 失效） |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号 |
| 85005 | —（`WechatErrorCodes` **未声明**该常量） | 可信域名未通过域名所有权校验（jssdk 受限） | E3 专属：只出现在 `SetAgentRequest.RedirectDomain` 的 DTO 注释里；出现说明域名未完成校验，重试无效 |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks 留档）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」，业务侧通常看不到这些码；若稳定出现，说明刷新本身在失败（自建排查 `AgentSecret`、第三方/代开发排查 `permanent_code` 与授权是否已取消），加重试不解决。

本族各端点的专属业务错误码（除 85005）接口注释未留档 ⇒ §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AG1~AG5 + 五个接口与 12 个 DTO 的 XML 注释 + 域 README 2026-10-18）；文档号一律照抄注释，**页面最后更新日期与正文文案未逐项核验** | 逐页渲染取文：90227 / 90228 / 90363 / 99052 / 100073 / 96448 / 99108 / 100072 |
| E1/E2 的独立页面归属 | 接口注释对 `agent/get` 与 `agent/list` **都引用 90227 / 90363 / 96448**（同一页覆盖两支）；`agent/list` 是否有独立页面号**未逐项核验** | 官方三棵树的应用管理章节；若官方拆页须同批改接口注释与 AG1 注释 |
| E1~E3 的频率数值 | 三支端点的接口注释均未留档 QPS / 日配额；E4/E5 只有「按附录默认口径」的说明，**非端点级数值** ⇒ 全部 **未逐项核验** | 官方附录「访问频率限制」（90454） |
| E4/E5 的附录默认口径本身 | 接口注释留档的「每企业单个 cgi/api 1 万次/分、15 万次/小时；第三方提供商每 IP 4 万次/分、120 万次/小时」来自官方附录的通用口径，本仓未逐项比对附录原文 | 官方 90454 |
| `agentid` 的取值范围 / 上限 | E1 只留档「可在应用管理设置页查看」，**无数值区间**；E3 的 `Agentid` 同为 `int` 且无区间 ⇒ 不得据本文设本地校验 | 官方 90227 / 90228 |
| E3 未赋值字段的语义 | 官方把除 `agentid` 外的 7 个业务字段都列为非必填，但**未留档**「未传的字段保持原值」还是「按默认值覆盖」⇒ 本文只给「先回读、改后再回读」的处置 | 官方 90228；必要时用测试企业实测 |
| `logo_mediaid` 的素材约束 | DTO 注释只说「通过素材管理接口上传图片获得」，图片格式 / 尺寸 / 有效期属素材域，**未在本域核验** | 素材管理域（`Media` 模块）及其守卫 |
| E5「成员授权模式」下 `admin` 的形态 | 官方只留档「不返回管理员列表」，**未说明是缺字段还是空数组** ⇒ 只能按 `null` 判定，两者是否可区分 **未逐项核验** | 官方 100073 / 100072 |
| `app_permissions_ext` 与 `app_permissions` 的对应关系 | 官方只说「二次授权并且有时效时返回该字段列表」，**未留档是否逐项一一对应**（数量、顺序、是否可能多条描述同一权限） | 官方 99052 / 99108 |
| `app_permissions` 的取值字典 | 权限字符串为「分类:子分类:权限」形态，取值字典在官方页附录且区分第三方/代开发；本仓**不做枚举建模**，字典内容未核验 | 官方 99052 / 99108 附录「应用权限说明」 |
| E1 响应是否有未列出的字段 | 本文响应表只覆盖接口注释与 DTO 声明到的字段；官方是否另有字段**未逐项核验**（源生成序列化会忽略未知字段） | 官方 90227 / 90363 / 96448 |
| 本族专属错误码集合 | 除 E3 注释提到的 85005 外，接口注释未留档任何端点级 errcode 表；且 85005 **未在 `WechatErrorCodes` 中声明为常量** | 官方全局错误码（90455）；是否补常量须与守卫同批决定 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 契约陷阱汇总，本域权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)
- 兄弟族（同域，共用 `WechatModule.Agent` 与 `Agent` 注册组）：[工作台自定义展示（AgentWorkbench）](./AgentWorkbench.md) · [自定义菜单（AgentMenu）](./AgentMenu.md) · [自建应用迁移成代开发（AgentMigration）](./AgentMigration.md)
- 邻域：上下游 `CorpGroup` 域的「获取应用共享信息」（`/cgi-bin/corpgroup/corp/list_app_share_info`，名字相近但不同族）
- 契约守卫源码：[WechatAgentContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) · [WechatNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs) · 跨线计数基线 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与 [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)「令牌与认证」一节
- 本线包级公开面与已踩陷阱：[Src/Work/Mud.Wechat.Work/README.md](../../../../../Src/Work/Mud.Wechat.Work/README.md)
- 官方页（文档号照接口注释留档，本次未重取正文）：[90227](https://developer.work.weixin.qq.com/document/path/90227) · [90228](https://developer.work.weixin.qq.com/document/path/90228) · [90363](https://developer.work.weixin.qq.com/document/path/90363) · [96448](https://developer.work.weixin.qq.com/document/path/96448) · [99052](https://developer.work.weixin.qq.com/document/path/99052) · [99108](https://developer.work.weixin.qq.com/document/path/99108) · [100073](https://developer.work.weixin.qq.com/document/path/100073) · [100072](https://developer.work.weixin.qq.com/document/path/100072)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
