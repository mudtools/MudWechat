# 工作台自定义展示（应用模版 + 用户工作台数据）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Agent` 域 · 工作台自定义展示族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkAgentWorkbenchService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentWorkbenchService.cs) · [IWechatWorkAgentWorkbenchService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentWorkbenchService_Internal.cs) · [IWechatWorkAgentWorkbenchService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentWorkbenchService_ThirdParty.cs) · [IWechatWorkAgentWorkbenchService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentWorkbenchService_Provider.cs)（目录：[Interfaces/Agent/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/)） |
| DTO 源文件 | [DataModels/Agent/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/)：本族 14 型 —— [SetAgentWorkbenchTemplateRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/SetAgentWorkbenchTemplateRequest.cs) · [GetAgentWorkbenchTemplateRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentWorkbenchTemplateRequest.cs) · [GetAgentWorkbenchTemplateResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentWorkbenchTemplateResponse.cs) · [SetAgentWorkbenchDataRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/SetAgentWorkbenchDataRequest.cs) · [BatchSetAgentWorkbenchDataRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/BatchSetAgentWorkbenchDataRequest.cs) · [GetAgentWorkbenchDataRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentWorkbenchDataRequest.cs) · [GetAgentWorkbenchDataResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentWorkbenchDataResponse.cs) · [AgentWorkbenchUserData](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchUserData.cs) · [AgentWorkbenchKeyData](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchKeyData.cs) · [AgentWorkbenchKeyDataItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchKeyDataItem.cs) · [AgentWorkbenchImage](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchImage.cs) · [AgentWorkbenchList](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchList.cs) · [AgentWorkbenchListItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchListItem.cs) · [AgentWorkbenchWebview](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentWorkbenchWebview.cs)（目录内共 30 型，其余属获取应用 / 菜单 / 迁移三族） |
| 契约守卫 | [WechatAgentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs)（AG1 路由与动词 / AG2 层级与端点计数 / AG3 令牌绑定 / AG4 DTO 登记 / AG5 字段陷阱）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）· 接口命名空间分区 [WechatInterfaceNamespaceContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| 官方文档 | 自建 92535 · 第三方 94620 · 服务商代开发 96454（三页覆盖本族全部 5 端点；文档号一律取自五个端点 XML 注释的 `<remarks>`）。页面最后更新日期**本次未取文**，见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 5 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AG1 `AgentWorkbenchRoutes`（5 条逐路由 + 逐动词断言 POST，含仅查询语义的两支）+ AG2 `AssertFamily`（父接口 `DeclaredOnly` 恰 5、三个应用类型子接口各恰 0、注册组 `Agent`、`InheritedFrom = WechatWorkAgentWorkbenchService`）+ AG3 本域 13 接口中的 4 个本族接口令牌绑定 + AG4 本族 14 型上下文登记 + AG5「模版数据同域两形态」与 `replace_user_data` 回显断言 + 四个接口与 14 个 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 XML 注释与 2026-10-18 核验留档；凡只有官方页能支撑的项一律进 §7 |

## 1. 这个接口族做什么

本族是「应用管理」域里**改应用在工作台首屏怎么出现**的那一面：E1/E2 设与读**应用级模版**（关键数据型 / 图片型 / 列表型 / 网页型，可同时下发企业级默认数据），E3~E5 按成员维度**写 / 批量写 / 读用户级展示数据**。

使用时机分两侧：写入侧通常是业务后端——待办数变了、订单数变了、活动图换了，就按用户推一次（E3 单人、E4 批量）；读取侧通常是做个人工作台页面或排障回读——先 E2 看应用配了哪种模版、有没有开启覆盖，再 E5 看某个成员实际生效的是哪份数据。

5 个端点官方对**企业自建 / 第三方 / 服务商代开发**三类应用开放面完全一致，全部收敛在公共父接口 `IWechatWorkAgentWorkbenchService`；三个应用类型子接口是零差异端点空标记，差别只在凭据归属域（§3）。

**不该用本族的场景**：改应用名称 / 头像 / 应用主页 / 可信域名走同域获取应用族（[Agent.md](./Agent.md) 的 E3 `SetAgentAsync`）；配底部自定义菜单走 [AgentMenu.md](./AgentMenu.md)；把自建应用迁成代开发走 [AgentMigration.md](./AgentMigration.md)；「工作台」二字也出现在通讯录与审批的业务文案里，那些不是本族。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建 `WechatAppType.Internal` · 第三方 `WechatAppType.ThirdParty` · 服务商代开发 `WechatAppType.Provider`（三类均开放全部 5 端点） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册由 `WechatAppManager` 同样校验 |
| 必填配置属性 | 自建：`CorpId` + `AgentSecret`；第三方 / 代开发：`CorpId`（**服务商企业**的 CorpId）+ `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败，不留到首次调用） |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | 自建子接口 `WechatTokenManagerKeys.InternalAccessToken`（`Wechat.AccessToken@Internal`）· 第三方 / 代开发子接口 `WechatTokenManagerKeys.CorpAccessToken`（`Wechat.AccessToken@Corp`）· **公共父接口不声明归属键**（AG3 只锁 `TokenType` / 注入模式 / 参数名，归属键由 TO1 按接口名族别锁定） | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；错配抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |

`access_token` 由 SDK 自动换取、缓存并按官方有效期刷新，**开发者不接触令牌值**（自建走 `corpsecret`、第三方走 `get_corp_token`、代开发以 `permanent_code` 走 `gettoken`）。

第三方 / 代开发调本族前必须进入目标授权企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时快照并写入「应用 + 企业」两级上下文、释放时逆序还原，嵌套安全）。第三个参数是该企业的永久授权码：代开发必传，第三方可省（缺省由 `IWechatCorpAuthStore` 提供）。`permanent_code` 是凭据：本文与示例只写参数名，不得进日志、遥测或异常消息。

### 2.2 权限与业务前置

以下逐条出自四个接口与 14 个 DTO 的 XML 注释（官方页本次未重取）：

- **须先在管理后台启用自定义展示**（管理后台—应用管理—应用—启用自定义展示）——父接口 remarks 与三个子接口 remarks 逐字留档。未启用时官方返回什么（报错码还是普通展示模式）注释未留档 ⇒ §7。
- **仅可设置当前凭证对应的应用**：`agentid` 不是任意应用 id，越权即失败。
- **一个应用仅支持配置一种模版样式**（E1 DTO 类级 remarks）：同时下发 `keydata` 与 `list` 不会变成「两种模版」，属于契约外形态，行为未留档 ⇒ §7。
- E1 的 `replace_user_data = true` **覆盖所有用户当前数据**，默认 `false`。
- E3 **须先设置应用在工作台展示的模版**（E3 接口注释原文）；`userid` / `userid_list` 里的每个成员**必须在应用可见范围内**（E3 / E4 两处注释均留档）。E4 的注释未重复「先设模版」这一前置，同族同结构下视为同一前置，但属本文推断 ⇒ §7。
- **跳转地址类型必须与应用主页匹配**：主页为网页则仅配置 `jump_url`，为小程序则仅配置 `pagepath`；类型不匹配时不同客户端平台 / 版本表现可能不一致（E1 注释 + 三个含跳转字段的结构 DTO 的类级 remarks）。这不是错误码，是**静默不一致**。
- E5：**若设置了应用模版且配置 `replace_user_data` 为 true，应用数据会覆盖个人数据，此时返回应用设置的数据**——读到的不一定是该成员的「个人数据」。
- 频率（E3 / E4 注释与 E4 DTO 类级 remarks 留档，逐字见各端点「本端点注意事项」）：E3 每个用户每个应用 10 次/分钟；E4 `userid_list` 最多 1000 个、每个应用 100000 人次/分钟。E1 / E2 / E5 的接口注释**未留档**任何频率数值 ⇒ 按官方全局限制执行，见 §7。
- 权限口径留档上有一处并存表述：父接口与第三方子接口 remarks 写「**仅第三方应用、自建应用支持自定义展示**」，而同一份注释又说「官方对三类应用开放一致的 5 个端点」，代开发子接口的 remarks 则不含该句。本文按守卫与接口声明照录，口径张力进 §7。

个人信息约束：`userid` 属成员身份信息，E5 返回的工作台展示数据（关键数据项的 key/data、列表标题）也是成员维度内容，**不得写入日志、遥测或异常消息**。本文示例一律用占位值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `SetAgentWorkbenchTemplateAsync` | 开放 | 开放 | 开放 |
| E2 `GetAgentWorkbenchTemplateAsync` | 开放 | 开放 | 开放 |
| E3 `SetAgentWorkbenchDataAsync` | 开放 | 开放 | 开放 |
| E4 `BatchSetAgentWorkbenchDataAsync` | 开放 | 开放 | 开放 |
| E5 `GetAgentWorkbenchDataAsync` | 开放 | 开放 | 开放 |

三列全开是本族与同域另外三族的**结构性差别**：获取应用族的开放面是「三类 2 + 自建 1 + 第三方/代开发 2」的两级不对称链，而本族是**一级平铺**——5 端点全在父接口，三个子接口各零端点。AG2 用 `AssertFamily(parentDeclaredEndpointCount: 5, 三个子接口各 0)` 锁定该形状；子接口集合漂移（多一个或少一个应用类型）即打红。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAgentWorkbenchService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空、**不声明归属键**） | 5（E1~E5） | 否 |
| `IWechatWorkInternalAgentWorkbenchService` | `Mud.Wechat.Work` | 自建子接口（零差异端点空标记） | 0（继承 5） | 是（`@Internal`） |
| `IWechatWorkThirdPartyAgentWorkbenchService` | `Mud.Wechat.Work` | 第三方子接口（零差异端点空标记） | 0（继承 5） | 是（`@Corp`） |
| `IWechatWorkProviderAgentWorkbenchService` | `Mud.Wechat.Work` | 服务商代开发子接口（零差异端点空标记） | 0（继承 5） | 是（`@Corp`） |

```
IWechatWorkAgentWorkbenchService            5 端点（E1~E5，三类应用一致）
├─ IWechatWorkInternalAgentWorkbenchService   +0（自建）
├─ IWechatWorkThirdPartyAgentWorkbenchService +0（第三方）
└─ IWechatWorkProviderAgentWorkbenchService   +0（代开发）
```

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Agent` / `AddAgentApi()`（`AddAllApis()` 亦包含；与获取应用 / 菜单 / 迁移三族共用同一模块与同一注册组）
- 注册组名：`Agent`（三个应用类型子接口都挂同一注册组，生成器产出 `AddAgentWebApiHttpClient()`；父接口注册组恒空）
- 生成器契约：三个子接口的 `InheritedFrom = nameof(WechatWorkAgentWorkbenchService)`，实现类落在父接口命名空间下的 `Internal`（AG2 断言）；子接口不再重复实现 5 个端点
- 端点索引：**E1** `SetAgentWorkbenchTemplateAsync` · **E2** `GetAgentWorkbenchTemplateAsync` · **E3** `SetAgentWorkbenchDataAsync` · **E4** `BatchSetAgentWorkbenchDataAsync` · **E5** `GetAgentWorkbenchDataAsync`

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，解析不到）。本族 14 个 DTO 全部登记进 `AgentJsonContext`（`internal`，示例不可引用；AG4 断言本域 30 型全量登记且 SerializerClassName 统一为 `Agent`，并把本族「端点级请求 / 响应 7 + 模版数据结构 7」共 14 型列入契约面抽查清单）。

---

## 4. 端点

五个端点的凭据面完全一致（`Wechat.AccessToken` + Query 注入 `access_token` + `application/json` 源生成序列化），**动词也一致：全部 POST**。差别只在归属键（取决于注入的是哪个子接口）与请求体形态（单用户平铺 ⇄ 批量 / 获取 `data` 包裹）。以下每节只列端点自身差异。

### E1. `SetAgentWorkbenchTemplateAsync` — 设置应用在工作台展示的模版

**签名**

```csharp
Task<WechatWorkResponse> SetAgentWorkbenchTemplateAsync(
    [Body] SetAgentWorkbenchTemplateRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/set_workbench_template` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/agent/set_workbench_template?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；第三方 / 代开发须先切企业作用域，见 §2.1） |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` / 第三方与代开发 `Wechat.AccessToken@Corp`（取决于注入的是哪个子接口） |
| 请求体编码 | `application/json`，源生成序列化（`AgentJsonContext`，AG4 锁定登记） |
| 返回类型 | 裸 `WechatWorkResponse`（只有 `errcode` / `errmsg`）——AG1 断言本端点**不得**新建空响应 DTO |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Agentid` | `agentid` | `int`（**非空**） | 是 | 应用 id。本族四个请求 DTO 的 `Agentid` 都是非可空 `int`，**未赋值时序列化为 `0`**，官方按非法 id 报错 |
| `request.Type` | `type` | `string?` | 是 | 模版类型：`keydata` / `image` / `list` / `webview`；设为 `normal` 则**取消自定义模式、切回普通展示模式**（DTO 属性注释） |
| `request.Keydata` | `keydata` | `AgentWorkbenchKeyData?` | 条件 | 关键数据型模版数据（`type` 为 `keydata` 时设置企业级默认数据）；`items` ≤4，见 §4 末尾结构对照 |
| `request.Image` | `image` | `AgentWorkbenchImage?` | 条件 | 图片型模版数据（`type` 为 `image` 时设置）；单对象，`url` 必填、最佳比例 3.35:1 |
| `request.List` | `list` | `AgentWorkbenchList?` | 条件 | 列表型模版数据（`type` 为 `list` 时设置）；`items` ≤3 |
| `request.Webview` | `webview` | `AgentWorkbenchWebview?` | 条件 | 网页型模版数据（`type` 为 `webview` 时设置）；单对象，`url` 必填 |
| `request.ReplaceUserData` | `replace_user_data` | `bool?` | 否 | 是否覆盖用户工作台数据：`true` 时**覆盖所有用户当前数据**，默认 `false` |

「条件必填」是本文对注释「`type` 为 X 时设置」的归纳：四个字段在 DTO 侧均可空、官方是否逐字段标必填未留档，但只有与 `type` 对应的那一个有意义；`type` 与数据字段不一致时官方的处置（忽略、报错还是按空数据下发）注释**未留档** ⇒ §7。SDK 不做参数校验：`type` 拼错、`items` 超 4 条都照样发出，错误以 `errcode` 回来。

```json
{
  "agentid": 1000002,
  "type": "keydata",
  "keydata": {
    "items": [
      { "key": "待办", "data": "3", "jump_url": "https://example.com/todo" }
    ]
  },
  "replace_user_data": false
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

**调用示例**

```csharp
using Mud.Wechat.Work;                          // IWechatWorkInternalAgentWorkbenchService / AddWechatWorkServices / AddAgentApi
using Mud.Wechat.Work.Abstractions;             // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions;  // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;         // 本族 DTO

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

// ② 注册 Agent 模块：本族 5 个端点全部随该模块注册，与获取应用/菜单/迁移三族共用。
builder.Services.AddWechatWorkServices(b => b.AddAgentApi());

// ③ 取可注入的自建子接口。公共父接口不注册 DI，解析不到。
var workbench = provider.GetRequiredService<IWechatWorkInternalAgentWorkbenchService>();
var agentId = int.Parse(config["Wechat:AgentId"]!);

// ④ 设置应用级模版：四个模版数据字段与 type 同级（本端点没有 data 包裹对象）。
var setTemplate = await workbench.SetAgentWorkbenchTemplateAsync(
    new SetAgentWorkbenchTemplateRequest
    {
        Agentid = agentId,                        // 官方必填；非可空 int，忘了赋值就会发 agentid=0
        Type = "keydata",                         // keydata / image / list / webview；normal = 取消自定义模式
        Keydata = new AgentWorkbenchKeyData
        {
            Items = new List<AgentWorkbenchKeyDataItem>
            {
                new() { Key = "待办", Data = "3", JumpUrl = "https://example.com/todo" },
            },
        },
        ReplaceUserData = false,                  // 置 true 会覆盖所有用户当前数据，先确认覆盖范围再改
    },
    cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!setTemplate.IsSuccess)
{
    logger.LogError("设置工作台模版失败 {ErrorCode}: {ErrorMessage}",
        setTemplate.ErrorCode, setTemplate.ErrorMessage);
    throw new InvalidOperationException($"设置工作台模版失败：{setTemplate.ErrorCode}");
}

// ⑥ 无业务负载：成功与否只能看 errcode，改完用 E2 回读校验（同一应用只有一种模版样式）。
```

**本端点注意事项**

- 本端点写的是**应用级模版 + 企业级默认数据**，不是某个成员的数据；按成员下发走 E3 / E4。
- **`type` 与四个数据字段必须配对**：`type = "list"` 却只填 `keydata` 属于契约外形态（官方行为未留档，§7）。切换模版类型等价于替换整个展示结构，一次只能有一种样式。
- `type = "normal"` 不是「不设置」，而是**主动取消自定义模式**：与「不传这个字段」语义完全不同，误用会让应用在工作台退回普通形态。
- `replace_user_data = true` 覆盖的是**所有用户**的当前数据，不是只覆盖「本次涉及的用户」；置回 `false` 能否恢复此前被覆盖的个人数据注释未留档 ⇒ §7。
- `jump_url` / `pagepath` 只能二选一且必须与应用主页类型匹配（§2.2），填错不报错、只在部分客户端表现异常。

### E2. `GetAgentWorkbenchTemplateAsync` — 获取应用在工作台展示的模版

**签名**

```csharp
Task<GetAgentWorkbenchTemplateResponse> GetAgentWorkbenchTemplateAsync(
    [Body] GetAgentWorkbenchTemplateRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/get_workbench_template` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 请求体 | 仅有 `agentid` 一个字段——**查询语义仍是 POST 带包体**（AG1 逐条断言 `PostAttribute`） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Agentid` | `agentid` | `int`（**非空**） | 是 | 应用 id；非可空，未赋值即发 `0` |

```json
{ "agentid": 1000002 }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `type` | `Type` | `string?` | 模版类型：`keydata` / `image` / `list` / `webview`；**`normal` 表示普通展示模式（未启用自定义展示）** |
| `keydata` | `Keydata` | `AgentWorkbenchKeyData?` | 关键数据型模版数据（`type` 为 `keydata` 时返回） |
| `image` | `Image` | `AgentWorkbenchImage?` | 图片型模版数据（`type` 为 `image` 时返回） |
| `list` | `List` | `AgentWorkbenchList?` | 列表型模版数据（`type` 为 `list` 时返回） |
| `webview` | `Webview` | `AgentWorkbenchWebview?` | 网页型模版数据（`type` 为 `webview` 时返回） |
| `replace_user_data` | `ReplaceUserData` | `bool?` | 是否覆盖用户工作台数据：`true` 表示应用模版数据覆盖所有用户当前数据 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "type": "keydata",
  "keydata": { "items": [{ "key": "待办", "data": "3", "jump_url": "https://example.com/todo" }] },
  "replace_user_data": false
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;

// 前置：AppType = Internal 已注册 + AddWechatWorkServices(b => b.AddAgentApi()) 已装配。
var workbench = provider.GetRequiredService<IWechatWorkInternalAgentWorkbenchService>();
var agentId = 1000002;                            // 假值：换成你的应用 id

// 查询语义同样走 POST + 包体；不要改成 GET，也不要省掉请求体。
var template = await workbench.GetAgentWorkbenchTemplateAsync(
    new GetAgentWorkbenchTemplateRequest { Agentid = agentId }, cancellationToken);
WechatWorkException.ThrowIfFailed(template, "/cgi-bin/agent/get_workbench_template");

// type == "normal" 是「未启用自定义展示」的正常返回，不是失败也不是缺少配置。
var customModeEnabled = !string.Equals(template.Type, "normal", StringComparison.Ordinal);
var defaultItems = template.Keydata?.Items ?? new List<AgentWorkbenchKeyDataItem>();
var coversUserData = template.ReplaceUserData == true;   // 为 true 时 E5 读到的是应用数据
```

**本端点注意事项**

- 读的是**应用级模版**（含企业级默认数据），不是某个成员的实际数据；成员维度走 E5。
- `type == "normal"` 是合法返回：说明应用当前处于普通展示模式（未启用自定义展示或已被 E1 设回 `normal`），不要当异常重试。
- 四个模版数据字段**只有一个非空**（好的报文里 `type` 指向哪个就只回哪个），其余为 `null`——按 `type` 分支读取，不要四路合判。
- 响应没有 `agentid` 回显：批处理多应用时由调用方自己维护 id 与响应的对应关系。

### E3. `SetAgentWorkbenchDataAsync` — 设置应用在用户工作台展示的数据

**签名**

```csharp
Task<WechatWorkResponse> SetAgentWorkbenchDataAsync(
    [Body] SetAgentWorkbenchDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/set_workbench_data` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 请求体形态 | **四个模版数据字段平铺在请求体顶层，无 `data` 包裹**（AG5 断言本 DTO 上不存在 `Data` 属性） |
| 返回类型 | 裸 `WechatWorkResponse` |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Agentid` | `agentid` | `int`（**非空**） | 是 | 应用 id |
| `request.Userid` | `userid` | `string?` | 是 | 目标成员 userid；**须在应用可见范围内**。成员身份信息，不得进日志 |
| `request.Type` | `type` | `string?` | 是 | 模版类型：`keydata` / `image` / `list` / `webview` |
| `request.Keydata` | `keydata` | `AgentWorkbenchKeyData?` | 条件 | 关键数据型数据（`type` 为 `keydata` 时设置），`items` ≤4 |
| `request.Image` | `image` | `AgentWorkbenchImage?` | 条件 | 图片型数据（`type` 为 `image` 时设置） |
| `request.List` | `list` | `AgentWorkbenchList?` | 条件 | 列表型数据（`type` 为 `list` 时设置），`items` ≤3 |
| `request.Webview` | `webview` | `AgentWorkbenchWebview?` | 条件 | 网页型数据（`type` 为 `webview` 时设置） |

```json
{
  "agentid": 1000002,
  "userid": "mud-USERID-FAKE-0001",
  "type": "list",
  "list": {
    "items": [{ "title": "待审批 2 条", "jump_url": "https://example.com/approval" }]
  }
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
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Agent;

// 前置：AppType = Internal 已注册 + AddWechatWorkServices(b => b.AddAgentApi()) 已装配，
// 且应用级模版已通过 E1 设置过（官方要求先设模版再设用户数据）。
var workbench = provider.GetRequiredService<IWechatWorkInternalAgentWorkbenchService>();
var agentId = 1000002;
var userId = "mud-USERID-FAKE-0001";               // 占位：成员 userid 属个人信息，不要写日志

var setUser = await workbench.SetAgentWorkbenchDataAsync(
    new SetAgentWorkbenchDataRequest
    {
        Agentid = agentId,
        Userid = userId,
        Type = "keydata",
        // 本端点没有 data 包裹对象：type 与四个模版数据字段在请求体同一层（对照 E4）。
        Keydata = new AgentWorkbenchKeyData
        {
            Items = new List<AgentWorkbenchKeyDataItem>
            {
                new() { Key = "待办", Data = "3" },
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(setUser, "/cgi-bin/agent/set_workbench_data");
```

**本端点注意事项**

- **频率：每个用户每个应用 10 次/分钟**（接口注释 `<summary>` 与 `<remarks>` 双处留档）⇒ 这不是可轮询的接口：按成员状态变化触发，不要「每秒同步一次」；对同一成员做多字段更新必须合并成一次调用，SDK 不代去重、不代合并。
- 请求体**平铺**：把 E4 的 `data` 对象套进来会得到「顶层没有 `type` / 四字段全空」的报文，官方按缺参处理——这是本族最容易写错请求体的地方（AG5 专门断言了本 DTO 不存在 `Data` 属性）。
- 必须先有应用级模版（§2.2）；`userid` 不在可见范围时官方报错，SDK 不预检。
- 覆写范围是**该成员在该应用工作台的数据整份替换**（报文里只有一份 `type` + 一组数据），不是字段级增量——这一点由报文结构推出，官方是否保留未携带字段**未留档** ⇒ §7。

### E4. `BatchSetAgentWorkbenchDataAsync` — 批量设置应用在用户工作台展示的数据

**签名**

```csharp
Task<WechatWorkResponse> BatchSetAgentWorkbenchDataAsync(
    [Body] BatchSetAgentWorkbenchDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/batch_set_workbench_data` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 请求体形态 | **模版数据以 `data` 对象（`AgentWorkbenchUserData`）包裹**，与 E3 的平铺形态不同（AG5 断言 `Data` 属性类型为 `AgentWorkbenchUserData`） |
| 返回类型 | 裸 `WechatWorkResponse` |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Agentid` | `agentid` | `int`（**非空**） | 是 | 应用 id |
| `request.UseridList` | `userid_list` | `List<string>?` | 是 | 目标成员列表，**最多 1000 个**（DTO 属性注释与接口注释双处留档）；每个成员须在应用可见范围内 |
| `request.Data` | `data` | `AgentWorkbenchUserData?` | 是 | 统一的用户数据对象：`type` + 与之配对的模版数据字段；**同一批用户共用这一份数据** |

```json
{
  "agentid": 1000002,
  "userid_list": ["mud-USERID-FAKE-0001", "mud-USERID-FAKE-0002"],
  "data": {
    "type": "webview",
    "webview": { "url": "https://example.com/card", "height": "double_row" }
  }
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
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Agent;

// 前置：AppType = Internal 已注册 + AddWechatWorkServices(b => b.AddAgentApi()) 已装配，应用级模版已设置。
var workbench = provider.GetRequiredService<IWechatWorkInternalAgentWorkbenchService>();
var agentId = 1000002;
var visibleUserIds = new List<string>();           // 业务侧填充：须全部在应用可见范围内

var batched = await workbench.BatchSetAgentWorkbenchDataAsync(
    new BatchSetAgentWorkbenchDataRequest
    {
        Agentid = agentId,
        UseridList = visibleUserIds.Take(1000).ToList(),   // 官方上限 1000，SDK 不裁剪、不代分批
        Data = new AgentWorkbenchUserData
        {
            Type = "list",                                  // 数据统一包在 data 里（对照 E3 的平铺）
            List = new AgentWorkbenchList
            {
                Items = new List<AgentWorkbenchListItem>
                {
                    new() { Title = "待审批 2 条", JumpUrl = "https://example.com/approval" },
                },
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(batched, "/cgi-bin/agent/batch_set_workbench_data");
```

**本端点注意事项**

- 频率：**每个应用 100000 人次/分钟**（接口注释 `<summary>` 与 `<remarks>` 双处留档），且 `userid_list` 单批**最多 1000 个**——这两个数不是一回事：单批 1000、每分钟 10 万人次，等于每分钟最多约 100 批。超 1000 条时 SDK 照样发出，官方行为未留档 ⇒ §7；正确做法是调用方按 1000 切片。
- **同一批用户共用一份 `data`**：想给不同成员发不同数据只能拆多次调用（或退回 E3 逐个设置）。
- 形态陷阱与 E3 互补：少一层 `data`（照 E3 平铺写）会得到「`data` 缺失 + 顶层多出四个不认识字段」的报文。
- 部分失败语义未留档：`userid_list` 里混入不可见成员时，是整批拒绝还是跳过该成员，注释未说明 ⇒ §7；不要假设「errcode == 0 就每个成员都写成功」。

### E5. `GetAgentWorkbenchDataAsync` — 获取应用在用户工作台展示的数据

**签名**

```csharp
Task<GetAgentWorkbenchDataResponse> GetAgentWorkbenchDataAsync(
    [Body] GetAgentWorkbenchDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/get_workbench_data` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 第三方 · 服务商代开发 |
| 请求体 | 仅 `agentid` + `userid`——查询语义仍是 POST 带包体（AG1 逐条断言 `PostAttribute`） |
| 响应体形态 | **数据以 `data` 对象包裹**，`type` 在 `data` 内部（AG5 断言 `Data` 属性类型为 `AgentWorkbenchUserData`） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Agentid` | `agentid` | `int`（**非空**） | 是 | 应用 id |
| `request.Userid` | `userid` | `string?` | 是 | 要查询的成员 userid；成员身份信息，不得进日志 |

```json
{ "agentid": 1000002, "userid": "mud-USERID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `data` | `Data` | `AgentWorkbenchUserData?` | 用户工作台展示数据（`type` + 对应模版数据） |
| `data.type` | `Type` | `string?` | 实际生效的模版类型：`keydata` / `image` / `list` / `webview`（`normal` 表示普通展示模式） |
| `data.keydata` | `Keydata` | `AgentWorkbenchKeyData?` | 关键数据型数据（`type` 为 `keydata` 时返回） |
| `data.image` | `Image` | `AgentWorkbenchImage?` | 图片型数据（`type` 为 `image` 时返回） |
| `data.list` | `List` | `AgentWorkbenchList?` | 列表型数据（`type` 为 `list` 时返回） |
| `data.webview` | `Webview` | `AgentWorkbenchWebview?` | 网页型数据（`type` 为 `webview` 时返回） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "data": {
    "type": "image",
    "image": { "url": "https://example.com/banner.png", "jump_url": "https://example.com/activity" }
  }
}
```

**E4 + E5 片段**（第三方场景，含企业作用域切换与两形态对照）

```csharp
using Mud.Wechat.Work;                              // IWechatWorkThirdPartyAgentWorkbenchService
using Mud.Wechat.Work.Abstractions.Authentication;  // IWechatAppContextSwitcher / UseCorpScope
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;             // 本族 DTO

// 前置：AddWechatApp（AppType = ThirdParty）+ AddWechatWorkServices(b => b.AddAgentApi()) 已在宿主装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var thirdPartyWorkbench = provider.GetRequiredService<IWechatWorkThirdPartyAgentWorkbenchService>();
var authCorpId = config["Wechat:AuthCorpId"]!;
var agentId = 1000002;
var visibleUserIds = new List<string>();            // 业务侧填充：≤1000 个，且须在应用可见范围内

// 企业级令牌一企一份（scope = authCorpId）：不进目标企业作用域就取不到该企业令牌。
// 第三方场景永久授权码可省（缺省由 IWechatCorpAuthStore 提供）；代开发须传 permanent_code，
// 该值是凭据——只从配置读取，不进日志/遥测/异常消息。
using (switcher.UseCorpScope("thirdparty-a", authCorpId))
{
    // E4：数据在 data 对象里，整批共用一份。
    var batched = await thirdPartyWorkbench.BatchSetAgentWorkbenchDataAsync(
        new BatchSetAgentWorkbenchDataRequest
        {
            Agentid = agentId,
            UseridList = visibleUserIds.Take(1000).ToList(),   // 1000 是单批上限，SDK 不代切片
            Data = new AgentWorkbenchUserData
            {
                Type = "list",
                List = new AgentWorkbenchList
                {
                    Items = new List<AgentWorkbenchListItem>
                    {
                        new() { Title = "待审批 2 条", JumpUrl = "https://example.com/approval" },
                    },
                },
            },
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(batched, "/cgi-bin/agent/batch_set_workbench_data");

    // E5：同样是 data 包裹，且 type 在 data 内部（E2 的模版响应 type 在顶层）。
    var userData = await thirdPartyWorkbench.GetAgentWorkbenchDataAsync(
        new GetAgentWorkbenchDataRequest { Agentid = agentId, Userid = "mud-USERID-FAKE-0001" },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(userData, "/cgi-bin/agent/get_workbench_data");

    var effectiveType = userData.Data?.Type;            // 可能是 normal（普通展示模式）
    var listTitles = (userData.Data?.List?.Items ?? new List<AgentWorkbenchListItem>())
        .Select(i => i.Title)
        .OfType<string>()
        .ToList();                                      // 成员维度内容：不得写日志
}
```

**本端点注意事项**

- **`type` 在 `data` 内部**，响应顶层没有 `type` / 四个数据字段；按 E2 的读法取 `Type` 会拿到 `null` 并误判「没有配置」。
- 读到的不一定是该成员的「个人数据」：应用模版 `replace_user_data = true` 时官方直接返回**应用设置的数据**（E5 DTO 类级 remarks）⇒ 用 E5 做「用户是否自己设置过」的判定会得到错答案。
- 查询语义仍是 POST + 包体（AG1）；`userid` 不在可见范围时官方报错。
- 无频率留档（§7）：需要给多个成员回读时按序调用，不要并发轰炸。

### 结构对照：模版数据与跳转字段（E1~E5 共用）

| 结构 | JSON 键 | 形态 | 上限 / 必填（DTO 注释原文） |
|---|---|---|---|
| `AgentWorkbenchKeyData` | `keydata` | 对象包数组 | `items` **不超过 4 个** |
| `AgentWorkbenchKeyDataItem` | `keydata.items[]` | 对象 | `data` 官方必填、**≤64 字符**；`key` **≤64 字符**；`jump_url` / `pagepath` **≤1024 字符** |
| `AgentWorkbenchImage` | `image` | 单对象 | `url` 官方必填，图片**最佳比例 3.35:1**；`jump_url` / `pagepath` 二选一 |
| `AgentWorkbenchList` | `list` | 对象包数组 | `items` **不超过 3 个** |
| `AgentWorkbenchListItem` | `list.items[]` | 对象 | `title` 官方必填、**≤128 字节**；`jump_url` / `pagepath` 二选一 |
| `AgentWorkbenchWebview` | `webview` | 单对象 | `url` 官方必填；`height` = `single_row`（106px，隐藏标题 147px）或 `double_row`（171px，隐藏标题 212px），**默认 `double_row`**；`hide_title` 默认 `false`；`enable_webview_click` 默认 `false` |
| `AgentWorkbenchUserData` | `data` | 包裹对象 | `type` + 与之配对的模版数据字段（仅 E4 请求与 E5 响应用） |

三处跳转字段的共同语义（DTO 属性注释）：`jump_url` 不填且有主页 url 则跳主页、否则跳应用会话窗口，**仅应用主页为网页时生效**；`pagepath` 不填则跳小程序主页，**仅应用主页为小程序时生效**。`webview.enable_webview_click = true` 时 **`jump_url` 失效**，链接跳转仅支持 `wxwork://openurl?url=xxxx` schema（`url` 须编码）。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E3 单用户设置（平铺）⇄ E4 批量设置 / E5 获取（`data` 包裹） | E3 的 `type` 与 `keydata/image/list/webview` **在请求体顶层**；E4 请求与 E5 响应把同一组字段塞进 `data`（`AgentWorkbenchUserData`）。AG5 用「`SetAgentWorkbenchDataRequest` 上不存在 `Data` 属性」「`BatchSet...` 与 `Get...Response` 的 `Data` 类型为 `AgentWorkbenchUserData`」双向锁定 | 照着一支的形态写另一支：官方按缺参处理，表现为「参数错误」或**静默写了个空数据**，抓包才能发现 |
| 本族 5 端点全 POST ⇄ 「查询用 GET」 | 含仅查询语义的 `get_workbench_template` / `get_workbench_data` 官方**也是 POST 且带包体**（AG1 逐条断言 `PostAttribute`，路由前缀全在 `/cgi-bin/agent/`） | 改成 GET（或按 GET 拼客户端）会打红 AG1；自行实现时官方不认动词，表现为 404 或参数缺失 |
| E2 的 `type`（响应顶层）⇄ E5 的 `type`（`data` 内部） | 模版响应是 `{type, keydata, …}`；用户数据响应是 `{data:{type, keydata, …}}` | 在 E5 响应上读 `Type` 恒为 `null`，误判「该成员没配置工作台」 |
| `keydata.items` ⇄ `list.items` | 两者**同名 `items`**，但元素类型不同、上限不同：关键数据型 ≤4（`key`/`data` ≤64 字符）、列表型 ≤3（`title` ≤128 字节） | 把 5 条关键数据当列表项塞进 `keydata`（或反之）是数量 / 结构双重越界，官方拒收且错误信息不指向具体条目 |
| `replace_user_data`（E1 请求 / E2 响应）⇄ 用户数据端点 | 该字段**只存在于模版两端点**：E1 置 `true` 覆盖所有用户数据，E2 回显该开关；E3/E4/E5 的 DTO 上都没有它 | 想在「设置某个用户数据」时用这个开关控制覆盖范围——字段根本不存在；真正的成员粒度覆盖由 E3/E4 的写入本身完成 |
| `type = "normal"` ⇄ 「不传 `type`」 | `normal` 是**主动取消自定义模式、切回普通展示模式**（E1 属性注释）；不传 `type` 属缺参 | 把 `normal` 当「默认值」传进去会静默关掉企业已上线的自定义工作台 |
| `webview.enable_webview_click = true` ⇄ 普通 `jump_url` 跳转 | 开启后 `jump_url` **失效**，只支持 schema 形式 `wxwork://openurl?url=xxxx`（`url` 须编码） | 同时配 `jump_url` 与开关，以为两处兜底，实际点击无反应 |
| `jump_url` ⇄ `pagepath` | 二选一，且必须与应用主页类型匹配：主页为网页只用 `jump_url`，为小程序只用 `pagepath`（E1 注释 + 各结构 DTO remarks） | 两个都填或填错，官方不报错，只在部分客户端平台 / 版本表现不一致（典型是「PC 能跳、手机不跳」） |
| 本族（三类全开）⇄ 获取应用族的两级不对称继承链 | 本族是**一级平铺**：父接口 5 端点 + 三个空标记子接口；获取应用族是「公共父 2 + 中间层 2 + 自建 1」的两级链 | 按 Agent.md 的形状在本族「补」一层中间父接口，会打红 AG2 的 `AssertFamily` 子接口集合断言 |
| `agentid` 非可空 `int` ⇄ 其它可选字段 | 本族四个请求 DTO 的 `Agentid` 都是非可空 `int`，未赋值即序列化为 `0`；而 `Type` / `Userid` 等为 `string?` | 忘记赋 `agentid` 不会编译报错，运行时官方报「应用不存在 / id 无效」，容易误判成权限问题 |
| 「仅第三方应用、自建应用支持自定义展示」（父接口 remarks）⇄ 「三类应用 5 端点一致开放」 | 同一份注释里两句话并存，代开发子接口的 remarks 不含前一句 | 据前一句认为代开发不支持本族、放弃 `IWechatWorkProviderAgentWorkbenchService` ⇒ 口径张力见 §7，不要按单句下结论 |

另外三条 SDK 侧行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改用 Header）。G5 把「以 Query 注入令牌」的接口名收敛为白名单，本族四个接口都在白名单内；`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo。
- **不自动判错、不校验、不代分批**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`；`items` ≤4 / ≤3、`userid_list` ≤1000、字符与字节上限、可见范围都在官方侧，SDK 不预检。
- **归属键与本族方法面上的「同一个 `access_token`」不是一回事**：`Wechat.AccessToken@Internal` 与 `Wechat.AccessToken@Corp` 声明在**子接口**上，父接口不声明。用自建应用上下文消费第三方子接口会在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；**E4 的 0 不等于「每个成员都写成功」**（部分失败语义未留档，§7） |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效 / 无效 | 本族失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效（部分接口） | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少 `access_token` 参数 / 凭据缺失（永久授权码无效等场景） | 不自愈：装配或作用域缺陷信号（多为未切 `UseCorpScope` 或 `permanent_code` 失效） |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号 |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks 留档）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」，业务侧通常看不到这些码；若稳定出现，说明刷新本身在失败（自建排查 `AgentSecret`、第三方 / 代开发排查 `permanent_code` 与授权是否已取消），加重试不解决。

本族五个端点的**专属业务错误码**（未启用自定义展示、应用不存在、`userid` 不在可见范围、`type` 与数据不配对、超出频率）接口注释均未留档 ⇒ §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AG1~AG5 + 四个接口与 14 个 DTO 的 XML 注释 + 域 README 2026-10-18）；文档号一律照抄注释，**页面最后更新日期与正文文案未逐项核验** | 逐页渲染取文：92535 / 94620 / 96454 |
| E1 / E2 / E5 的频率数值 | 三支端点的接口注释均未留档 QPS / 日配额（只有 E3 的 10 次/分钟与 E4 的 100000 人次/分钟有数值）⇒ **未逐项核验**；本文按「走官方全局限制」表述 | 官方附录「访问频率限制」（90454） |
| 「仅第三方应用、自建应用支持自定义展示」⇄「三类应用 5 端点一致开放」 | 父接口与第三方子接口 remarks 含前一句，代开发子接口 remarks 不含；而三份接口又都声明 5 端点对三类应用一致开放。两句并存于同一契约面，**官方原意未逐项核验**（可能的解释：代开发应用即「代开发自建应用」，其管理后台不暴露自定义展示开关） | 官方 92535 / 94620 / 96454 的权限说明段；若确认代开发不可用，须同批改接口注释与 AG2 开放面断言 |
| 「未启用自定义展示」时的官方行为 | 注释只写「须先在管理后台启用自定义展示」，未留档未启用时是报错码、还是静默按普通展示返回 ⇒ **未逐项核验** | 官方 92535 / 94620 / 96454 |
| `type` 与四个模版数据字段不配对时的行为 | 注释只写「`type` 为 X 时设置」，未说明传入与 `type` 不符的数据字段（或一个都不传）是忽略、报错还是清空 ⇒ **未逐项核验** | 官方 92535 / 94620 / 96454；必要时用测试企业实测 |
| E4 是否同样要求「先设模版」 | E3 注释明确留档「须先设置应用在工作台展示的模版」，E4 的接口注释与 DTO 类级 remarks 都**未重复**该前置；本文按同族同结构视为同一前置，属推断 ⇒ 未逐项核验 | 官方 92535 / 94620 / 96454 |
| `replace_user_data = true` 的覆盖可逆性 | 注释只留档「覆盖所有用户当前数据，默认 false」，**未说明置回 `false` 或被覆盖后能否恢复各成员原个人数据** ⇒ 本文只给「置 true 前先评估覆盖面」的处置 | 官方 92535；必要时先备份 E5 回读结果 |
| E3 / E4 的「整份替换 vs 字段增量」 | 报文只承载一份 `type` + 一组数据，本文据此推断为整份替换；官方是否保留未携带的字段**未留档** ⇒ 推断部分未逐项核验 | 官方 92535 / 94620 / 96454 |
| E4 超出 1000 条与部分失败的官方行为 | 注释只给「最多 1000 个」；超限是整批拒绝还是部分处理、`userid_list` 混入不可见成员时是否整批失败，均**未留档** ⇒ 客户端须自行切片并自行对账 | 官方 92535 / 94620 / 96454；实测 |
| `key` / `data` 的「字符」与 `title` 的「字节」计量口径 | DTO 注释分别是「≤64 字符」与「≤128 字节」；是否按 utf8 计、中文各占几单位**未留档**，计数口径未逐项核验 | 官方 92535 / 94620 / 96454 |
| 各模版类型 `jump_url` / `pagepath` 的长度上限 | 只有 `AgentWorkbenchKeyDataItem` 的属性注释留档「≤1024 字符」；`image` / `list.items[]` / `webview` 的同名字段无长度说明 ⇒ **未逐项核验**，不得据 keydata 的数值外推 | 官方 92535 / 94620 / 96454 |
| `webview.height` 的取值集合 | 注释只列 `single_row` / `double_row`（含各自 px 值）与默认值；其它取值（含空串）的行为**未逐项核验** | 官方 92535 / 94620 / 96454 |
| `normal` 在 E4 `data.type` 上的语义 | 只有 E1 与 `AgentWorkbenchUserData.Type` 的属性注释写「设为 normal 则取消自定义模式」；批量设置时置 `normal` 是否逐用户清空、E5 回读是否返回 `normal` **未逐项核验** | 官方 92535 / 94620 / 96454 |
| `userid` 是否接受密文 / `open_userid` 形态 | 注释只写「须在应用可见范围内」，未说明第三方 / 代开发场景该传明文 userid 还是密文 id ⇒ **未逐项核验**（同仓 AccountId 域的 ID 转换族承载密文换算） | 官方 94620 / 96454；AccountId 域的 ID 转换族守卫 |
| 本族专属错误码集合 | 除令牌失效码外，接口注释未留档任何端点级 errcode 表（未启用展示、应用不存在、可见范围、频率超限等） ⇒ **未逐项核验** | 官方全局错误码（90455） |
| 响应是否有未列出的字段 | 本文响应表只覆盖接口注释与 DTO 声明到的字段；官方是否另有字段**未逐项核验**（源生成序列化会忽略未知字段） | 官方 92535 / 94620 / 96454 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 契约陷阱汇总，本域权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)
- 兄弟族（同域，共用 `WechatModule.Agent` 与 `Agent` 注册组）：[获取应用（Agent）](./Agent.md) · [自定义菜单（AgentMenu）](./AgentMenu.md) · [自建应用迁移成代开发（AgentMigration）](./AgentMigration.md)
- 邻域：成员与可见范围的定义在通讯录域；本族只在 `Agent` 域读写工作台展示面
- 契约守卫源码：[WechatAgentContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) · [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) · 跨线计数基线 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与 [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)「令牌与认证」一节
- 本线包级公开面与已踩陷阱：[Src/Work/Mud.Wechat.Work/README.md](../../../../../Src/Work/Mud.Wechat.Work/README.md)
- 官方页（文档号照接口注释留档，本次未重取正文）：[92535](https://developer.work.weixin.qq.com/document/path/92535) · [94620](https://developer.work.weixin.qq.com/document/path/94620) · [96454](https://developer.work.weixin.qq.com/document/path/96454)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
