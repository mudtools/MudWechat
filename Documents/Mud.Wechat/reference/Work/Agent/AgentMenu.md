# 自定义菜单（创建 / 获取 / 删除，仅企业自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Agent` 域 · 自定义菜单族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [Interfaces/Agent/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/)：[IWechatWorkAgentMenuService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentMenuService.cs)（公共父接口，零端点）· [IWechatWorkAgentMenuService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/IWechatWorkAgentMenuService_Internal.cs)（唯一自建子接口，3 端点） |
| DTO 源文件 | [DataModels/Agent/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/)：本族 3 型 —— [CreateAgentMenuRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/CreateAgentMenuRequest.cs) · [GetAgentMenuResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/GetAgentMenuResponse.cs) · [AgentMenuButton](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/AgentMenuButton.cs) |
| 契约守卫 | [WechatAgentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs)（AG1 路由与动词 + `agentid` Query 参数 + 裸响应形态 / AG2 `AssertFamily` 父零端点 + 自建子 3 端点 / AG3 令牌绑定 / AG4 上下文登记 / AG5 `sub_button` 递归与 `button` 数组）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 自建 90231（创建菜单）· 90232（获取菜单）· 90233（删除菜单）；权限口径原文「仅企业可调用；第三方不可调用」，服务商代开发章节无对应 API。文档号取自两个接口文件的 `<remarks>`；**页面最后更新日期与正文本次未重取**，见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 3 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AG1 三条菜单路由逐动词（`Post` / `Get` / `Get`）+ `AssertAgentidQueryParameter` 三连断言 + 裸 `WechatWorkResponse` 形态断言 / AG2 `AssertFamily`（父 0 端点、子恰 3、继承链不得出现第三方或代开发子接口）/ AG3 两个菜单接口的 `AccessToken` + Query + `access_token` / AG4 三个菜单 DTO 登记 `AgentJsonContext` 且 `SerializerClassName = Agent` / AG5 `SubButton` 为 `List<AgentMenuButton>`、`sub_button` 与 `button` 字段名 / TO1~TO3 归属域键 / G5 白名单 / 两份接口与三个 DTO 的 XML 注释 / [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承接口 XML 注释与本域 2026-10-18 核验留档；凡只有官方页能支撑的项（频率、专属错误码、页面更新日期、覆盖语义）一律进 §7 |

## 1. 这个接口族做什么

本族是「应用管理」域里**给单个企业自建应用配自定义菜单**的那一面：把一棵两级菜单树写进应用（E1），回读当前菜单树（E2），把整棵树删掉（E3）。自定义菜单就是用户在企业微信里打开该应用时看到的那排入口，通常随应用版本发布改一次、平时不动。

三个端点在同一批场景里配合使用：改版时用 E1 写入新树，排障或写操作前用 E2 回读留档，回收测试应用或下线入口时用 E3 清空。E1 与 E3 是写操作，E2 是读操作，**但 E3 的 HTTP 动词也是 GET**（官方契约），不能按动词判断危险程度。

本族只对**企业自建应用**开放：三个端点的接口注释权限说明均为「仅企业可调用；第三方不可调用」，服务商代开发章节没有对应 API。落到代码形态就是「零端点公共父接口 + 唯一自建子接口」，第三方与代开发**没有**通向本族的 DI 入口（AG2 锁定）。

**不该用本族的场景**：改应用在工作台的展示模板与用户数据走同域工作台族（5 支，路由均挂 `/cgi-bin/agent/` 段：`set_workbench_template` / `get_workbench_template` / `set_workbench_data` / `batch_set_workbench_data` / `get_workbench_data`），改应用名称、头像、主页走获取应用族的 `SetAgentAsync`（见 [Agent.md](./Agent.md)）。菜单只是会话入口的展示层，不承载权限或可见范围。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（仅企业自建应用；第三方与代开发无本族入口） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId` + `AgentSecret`（`AgentSecret` 即 `corpsecret`，是凭据，不得进日志） | 同上；缺任一项抛 `InvalidOperationException`（`自建应用 {AppKey} 缺少 CorpId/AgentSecret`），启动即失败 |
| 可选配置属性 | `AgentId`（**`string`**，注释原文「业务可选，仅作展示/路由用途」） | 不参与 `Validate()`；它与端点参数 `int agentid` 是两个东西，见 §5 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | `WechatTokenManagerKeys.InternalAccessToken`（`Wechat.AccessToken@Internal`，声明在自建子接口上；父接口只声明 `TokenType`、不声明归属键） | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；错配抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |
| 企业作用域 | **不需要**：自建应用消费应用自身令牌，不涉及 `authCorpId` scope，无需 `UseCorpScope` | — |

`access_token` 由 SDK 用 `CorpId + AgentSecret` 自动换取、缓存并按官方有效期刷新，**开发者不接触令牌值**；示例里只出现 `config["Wechat:AgentSecret"]` 这类占位读取，不出现任何真实密钥。

### 2.2 权限与业务前置

以下逐条出自接口与 DTO 的 XML 注释（官方页本次未重取）：

- 三个端点：**仅企业可调用；第三方不可调用**（E1/E2/E3 的 `<remarks>` 逐字重复）——本族没有第三方或代开发的等价面。
- `agentid`：官方必填，注释只写「企业应用 id（官方必填；可在应用管理设置页查看）」，**没有数值区间**；SDK 不做校验，传不存在的 id 照样发出，错误以 `errcode` 回来。
- E1 结构限制：一级菜单（`button`）个数 **1~3 个**；二级菜单（`sub_button`）个数 **1~5 个**；主菜单名 **≤16 字节**、子菜单名 **≤40 字节**；菜单 `key` **≤128 字节**；网页链接 `url` **≤1024 字节**（建议使用 https）；`appid` **仅限与企业绑定的小程序**（E1 的 `<summary>` 与 `AgentMenuButton` 的 `<remarks>` 双重留档）。
- E1 的 `type` 取值共 9 种，含必填字段的三种：`click`（`key` 必填）、`view`（`url` 必填）、`view_miniprogram`（`appid` 与 `pagepath` 必填）；其余六种为 `scancode_push`、`scancode_waitmsg`、`pic_sysphoto`、`pic_photo_or_album`、`pic_weixin`、`location_select`，注释未为它们单列必填字段（§7）。
- E3：**覆盖式删除**——「删除指定应用的全部自定义菜单（覆盖式删除，无法按单个菜单项删除）」；没有「删一个菜单项」的端点。
- 频率：三个端点的注释均未留档 QPS 或日配额 ⇒ §7，不得按记忆填数。

个人信息与敏感面：菜单名可能承载业务信息（部门名、活动名、内部入口名），属业务内容而非凭据，但同样不建议整棵树写进日志或遥测；`agentid` 是可公开的应用 id，不属敏感数据。本族不返回任何成员身份信息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `CreateMenuAsync` | 开放 | 未开放（「第三方不可调用」） | 未开放（代开发章节无对应 API） |
| E2 `GetMenuAsync` | 开放 | 未开放 | 未开放 |
| E3 `DeleteMenuAsync` | 开放 | 未开放 | 未开放 |

上表就是 §3 继承链形状的来源：开放面全在自建一侧，故公共父接口**零端点**、唯一子接口承载全部 3 个端点，继承链上不得出现 `_ThirdParty` / `_Provider` 兄弟接口（AG2 的 `AssertFamily` 对子接口集合做等价断言，多一个即打红）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAgentMenuService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空、不声明归属键） | 0 | 否 |
| `IWechatWorkInternalAgentMenuService` | `Mud.Wechat.Work` | 自建子接口（`InheritedFrom = nameof(WechatWorkAgentMenuService)`） | 3（E1/E2/E3） | 是（`@Internal`） |

```
IWechatWorkAgentMenuService              0 端点（仅作契约基座，不注册 DI）
└─ IWechatWorkInternalAgentMenuService   3 端点（创建 / 获取 / 删除），唯一子接口
```

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Agent` / `AddAgentApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Agent`（生成器产出 `AddAgentWebApiHttpClient()`；父接口注册组恒空）
- 端点索引：**E1** `CreateMenuAsync` · **E2** `GetMenuAsync` · **E3** `DeleteMenuAsync`

最终用户注入**自建子接口**；父接口不注册 DI、也没有任何端点，解析它会失败。本族 3 个 DTO 均登记进 `AgentJsonContext`（`internal`，示例不可引用；AG4 断言本域 30 型全量登记、`SerializerClassName` 统一 `Agent`）。

---

## 4. 端点

三个端点的凭据面完全一致（`Wechat.AccessToken` + Query 注入 `access_token` + 归属键 `Wechat.AccessToken@Internal`），`agentid` 也一致地走 **Query**。差异只在动词、请求体与响应形态，以下每节只列端点自身差异。

### E1. `CreateMenuAsync` — 为指定应用写入菜单树

**签名**

```csharp
Task<WechatWorkResponse> CreateMenuAsync(
    [Query("agentid")] int agentid,
    [Body] CreateAgentMenuRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/menu/create` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/menu/create?access_token=ACCESS_TOKEN&agentid=AGENTID` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成）；`agentid` 也是 **Query** 参数（AG1 断言 `[Query("agentid")] int`），不进请求体 |
| 适用应用类型 | 仅企业自建应用 |
| 令牌归属键 | `Wechat.AccessToken@Internal` |
| 请求体编码 | `application/json`，源生成序列化（`AgentJsonContext`，AG4 锁定登记） |
| 返回类型 | 裸 `WechatWorkResponse`（只有 `errcode` / `errmsg`）——AG1 断言本端点**不得**新建空响应 DTO |
| 官方文档 | 90231 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `agentid` | `agentid` | `int`（方法参数，`[Query("agentid")]`） | 是 | 企业应用 id；注释只写「可在应用管理设置页查看」，取值范围未留档（§7） |
| `request.Button` | `button` | `List<AgentMenuButton>?` | 是（DTO 注释「官方必填」） | 一级菜单数组，个数应为 **1~3 个** |
| `request.Button[].Type` | `type` | `string?` | 是（DTO 注释「官方必填」；容器按钮的官方口径未留档，见 §7） | 响应动作类型，9 种取值见 §2.2 |
| `request.Button[].Name` | `name` | `string?` | 是（「不能为空」） | 菜单名；**一级 ≤16 字节**（字节而非字符） |
| `request.Button[].Key` | `key` | `string?` | `click` 等点击类型必填 | 菜单 KEY 值，用于消息接口推送，**≤128 字节** |
| `request.Button[].Url` | `url` | `string?` | `view` 必填 | 网页链接，**≤1024 字节**，建议使用 https |
| `request.Button[].Appid` | `appid` | `string?` | `view_miniprogram` 必填 | 小程序 appid，**仅限与企业绑定的小程序** |
| `request.Button[].Pagepath` | `pagepath` | `string?` | `view_miniprogram` 必填 | 小程序页面路径 |
| `request.Button[].SubButton` | `sub_button` | `List<AgentMenuButton>?` | 否（为容器按钮时才有） | 二级菜单数组，个数应为 **1~5 个**；元素与一级**同构**（同一个 `AgentMenuButton` 类型） |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入，调用方不传 |

二级菜单元素的字段与一级完全相同（同一个类型），长度口径的差别只有一条注释明示：**子菜单 `name` ≤40 字节**（一级是 16 字节），其余字段限制一视同仁。

SDK 不做任何参数校验：个数越界、字节超限、`appid` 未绑定、`type` 拼错都在官方侧判定，失败以 `errcode` 回来。

```json
{
  "button": [
    { "type": "click", "name": "帮助中心", "key": "MENU_HELP" },
    {
      "name": "更多",
      "sub_button": [
        { "type": "view", "name": "官网", "url": "https://example.com/" },
        { "type": "view_miniprogram", "name": "打卡", "appid": "wx0000000000000000", "pagepath": "pages/index/index" }
      ]
    }
  ]
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
using Mud.Wechat.Work;                          // IWechatWorkInternalAgentMenuService / AddWechatWorkServices / AddAgentApi
using Mud.Wechat.Work.Abstractions;             // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions;  // WechatWorkException（判错出口）
using Mud.Wechat.Work.DataModels.Agent;         // CreateAgentMenuRequest / AgentMenuButton / GetAgentMenuResponse

// 宿主约定：builder / config / provider / logger / cancellationToken 由宿主提供
//（builder 为 IServiceCollection、config 为 IConfiguration、provider 为已 Build 的 IServiceProvider）；
// 本示例按 ASP.NET Core Web SDK 形态编写，Microsoft.Extensions.DependencyInjection 与
// Microsoft.Extensions.Logging 的 using 由其隐式提供。

// ① 注册自建应用：CorpId + AgentSecret 是 Internal 类型的互斥必填组合，缺任一项注册期即抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:AgentSecret"];      // 凭据：只从配置读取，不进日志
    o.AgentId = config["Wechat:AgentId"];              // 配置项是 string（仅展示/路由用途），端点是 int
});

// ② 注册 Agent 模块：本族 3 个端点全部随该模块注册。
builder.Services.AddWechatWorkServices(b => b.AddAgentApi());

// ③ 取可注入的自建子接口（父接口 IsAbstract、零端点、不注册 DI，解析不到）。
var menuService = provider.GetRequiredService<IWechatWorkInternalAgentMenuService>();
var agentId = int.Parse(config["Wechat:AgentId"]!);    // 与配置项的 string 形态无关，这里是业务参数

// ④ 两级菜单：一级 1~3 个，二级 1~5 个；二级元素与一级同构。
var request = new CreateAgentMenuRequest
{
    Button = new List<AgentMenuButton>
    {
        new AgentMenuButton
        {
            Type = "click",                            // click 必须给 key，消息事件靠它区分
            Name = "帮助中心",                          // 一级名 ≤16 字节（汉字按 3 字节算）
            Key = "MENU_HELP"
        },
        new AgentMenuButton
        {
            Name = "更多",                              // 容器按钮：只挂 sub_button
            SubButton = new List<AgentMenuButton>
            {
                new AgentMenuButton { Type = "view", Name = "官网", Url = "https://example.com/" },
                new AgentMenuButton
                {
                    Type = "view_miniprogram",
                    Name = "打卡",                          // 二级名上限 40 字节
                    Appid = "wx0000000000000000",           // 必须是与企业绑定的小程序
                    Pagepath = "pages/index/index"
                }
            }
        }
    }
};

// ⑤ 调用：agentid 走 Query，既不在 URL 路径上，也不在请求体里。
var created = await menuService.CreateMenuAsync(agentId, request, cancellationToken);

// ⑥ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功；本端点无业务字段可读。
if (!created.IsSuccess)
{
    logger.LogError("创建菜单失败 {ErrorCode}: {ErrorMessage}", created.ErrorCode, created.ErrorMessage);
    throw new InvalidOperationException($"创建菜单失败：{created.ErrorCode}");
}

// ⑦ 回读校验：写入成功不代表树与预期一致（官方是否整树替换、是否丢弃未知字段均未留档），
//    唯一可靠的核对方式是再调一次 E2。
var applied = await menuService.GetMenuAsync(agentId, cancellationToken);
WechatWorkException.ThrowIfFailed(applied, "/cgi-bin/menu/get");
var topLevelCount = (applied.Button ?? new List<AgentMenuButton>()).Count;
```

**本端点注意事项**

- `agentid` 是 **Query** 参数（AG1 逐方法断言），`button` 是请求体；把 id 一起塞进包体或把按钮树挪到 Query 都会与官方契约不符。
- 一级菜单名上限是 **16 字节**，不是 16 个字符：UTF-8 下每个汉字 3 字节，五个汉字即触上限，SDK 不预检。
- 本次写入是否**整树替换**既有菜单、被省略的字段是保持还是清零，接口注释未留档（§7）⇒ 改菜单前先用 E2 回读留档，改完再回读核对。
- 响应只有 `errcode` / `errmsg`，无法从响应确认「到底写进去了什么」；需要确认就回读 E2。

### E2. `GetMenuAsync` — 获取指定应用的菜单树

**签名**

```csharp
Task<GetAgentMenuResponse> GetMenuAsync(
    [Query("agentid")] int agentid,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/menu/get`（官方即 GET，勿「顺手统一」为 POST） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/menu/get?access_token=ACCESS_TOKEN&agentid=AGENTID` |
| 凭据参数 | `access_token`（Query 注入）+ 归属键 `Wechat.AccessToken@Internal` |
| 适用应用类型 | 仅企业自建应用 |
| 请求体 | 无（GET，方法签名只有 `agentid` 与 `CancellationToken`） |
| 官方文档 | 90232 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `agentid` | `agentid` | `int`（方法参数，`[Query("agentid")]`） | 是 | 企业应用 id；注释只写「可在应用管理设置页查看」 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入；本端点无其它参数 |

```
GET /cgi-bin/menu/get?access_token=ACCESS_TOKEN&agentid=1000002
（无请求体）
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（`GetAgentMenuResponse` 继承 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `button` | `Button` | `List<AgentMenuButton>?` | 一级菜单数组；元素结构见下表，**与创建请求共用同一 `AgentMenuButton` 类型**（AG5 断言） |
| `button[].type` | `Type` | `string?` | 响应动作类型 |
| `button[].name` | `Name` | `string?` | 菜单名 |
| `button[].key` | `Key` | `string?` | 菜单 KEY 值 |
| `button[].url` | `Url` | `string?` | 网页链接 |
| `button[].pagepath` | `Pagepath` | `string?` | 小程序页面路径 |
| `button[].appid` | `Appid` | `string?` | 小程序 appid |
| `button[].sub_button` | `SubButton` | `List<AgentMenuButton>?` | 二级菜单数组，元素与一级同构（递归） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "button": [
    { "type": "click", "name": "帮助中心", "key": "MENU_HELP", "sub_button": [] },
    {
      "name": "更多",
      "sub_button": [
        { "type": "view", "name": "官网", "url": "https://example.com/" },
        { "type": "view_miniprogram", "name": "打卡", "appid": "wx0000000000000000", "pagepath": "pages/index/index" }
      ]
    }
  ]
}
```

**调用示例**（前置同 E1 的 ①②：应用注册与模块注册；服务解析在本段内补出）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;          // GetAgentMenuResponse / AgentMenuButton

var menuService = provider.GetRequiredService<IWechatWorkInternalAgentMenuService>();
var agentId = 1000002;                           // 演示用应用 id；实际取业务配置

var menu = await menuService.GetMenuAsync(agentId, cancellationToken);
WechatWorkException.ThrowIfFailed(menu, "/cgi-bin/menu/get");

// 无菜单时 button 是「缺字段」「空数组」还是「errcode 非 0」，注释未留档（§7）：
// 这里统一按「null 与空数组都是没有菜单」处理，不要用 null 判定失败。
var buttons = menu.Button ?? new List<AgentMenuButton>();

// 两级树摊平：有 sub_button 取二级，否则二级视作自身。
var clickKeys = buttons
    .SelectMany(b => b.SubButton is { Count: > 0 } sub ? sub : new List<AgentMenuButton> { b })
    .Where(b => string.Equals(b.Type, "click", StringComparison.Ordinal))
    .Select(b => b.Key)
    .OfType<string>()
    .ToList();
```

**本端点注意事项**

- 响应与创建请求**共用** `AgentMenuButton`：抓到什么结构就按什么结构反序列化，不要另造一套只读 DTO（AG5 断言 `GetAgentMenuResponse.Button` 就是 `List<AgentMenuButton>`）。
- `Button == null` 与 `Button` 为空数组在报文上是否可区分，注释未留档；两者都按「无菜单」处理更安全。
- 回读在写操作前后各做一次是最省事的审计手段：菜单树没有版本号，只有这一份当前快照。
- 菜单名可能含业务信息，整棵树不要原样写进日志或遥测。

### E3. `DeleteMenuAsync` — 删除整棵菜单树（覆盖式）

**签名**

```csharp
Task<WechatWorkResponse> DeleteMenuAsync(
    [Query("agentid")] int agentid,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/menu/delete`（官方即 GET；**动词是 GET 不代表这是查询**） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/menu/delete?access_token=ACCESS_TOKEN&agentid=AGENTID` |
| 凭据参数 | `access_token`（Query 注入）+ 归属键 `Wechat.AccessToken@Internal` |
| 适用应用类型 | 仅企业自建应用 |
| 请求体 | 无（AG1 断言方法参数恰为 `agentid` + `CancellationToken`，返回裸 `WechatWorkResponse`） |
| 官方文档 | 90233 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `agentid` | `agentid` | `int`（方法参数，`[Query("agentid")]`） | 是 | 要清空菜单的应用 id；删除粒度是**整棵树** |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

```
GET /cgi-bin/menu/delete?access_token=ACCESS_TOKEN&agentid=1000002
（无请求体，也无「删哪个菜单项」的参数）
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功 |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**（前置同 E1 的 ①②：应用注册与模块注册；服务解析在本段内补出）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Agent;          // 先回读留档时用

var menuService = provider.GetRequiredService<IWechatWorkInternalAgentMenuService>();
var agentId = 1000002;                           // 演示用应用 id；实际取业务配置

// 高危写面：调用成功即该应用整棵树消失，没有「按单个菜单项删除」的端点，也没有撤销。
// 宿主侧必须先取得人工确认（这里用运维开关表达），不要放进自动重试、订正任务或批量脚本的默认路径。
var operatorConfirmed = string.Equals(
    config["Operations:ConfirmMenuDelete"], "true", StringComparison.OrdinalIgnoreCase);
if (!operatorConfirmed)
{
    throw new InvalidOperationException(
        "删除菜单需人工确认：请显式设置 Operations:ConfirmMenuDelete=true 后再执行（整树覆盖式删除，不可撤销）。");
}

// 建议：删除前先调 E2 回读一次并自行留档，删除后无法从本族取回旧树。
var backup = await menuService.GetMenuAsync(agentId, cancellationToken);
WechatWorkException.ThrowIfFailed(backup, "/cgi-bin/menu/get");
logger.LogWarning("即将删除应用 {AgentId} 的菜单树，一级菜单 {Count} 个", agentId, backup.Button?.Count ?? 0);

var deleted = await menuService.DeleteMenuAsync(agentId, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/menu/delete");
```

**本端点注意事项**

- 删除是**整树覆盖式**的：不存在「删掉某个按钮」的参数，想删一个入口只能整树重写（E1）或整树删除（E3）。
- 动词是 GET，但语义是写且不可撤销；按 REST 直觉把它放进轮询、幂等重试或「探测可用性」的循环里，一次误触发就是整棵树没了，重试不会把它找回来。
- 删除成功也只返回 `errcode` / `errmsg`：无法从响应确认「删掉了哪些」。删除后若要验证，回读 E2（无菜单时的响应形态见 §7）。
- 生产环境建议把本端点收在运维入口后面，不与发布流水线自动串联。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族的 `agentid`（Query）⇄ 同域「设置应用」的 `Agentid`（请求体） | 本族三端点都是 `[Query("agentid")] int`（AG1 对三个方法逐一断言）；`SetAgentRequest.Agentid` 才是请求体字段 | 把 id 塞进 `CreateAgentMenuRequest` 或 URL 路径里，官方按「缺参」报错，而不是「应用不存在」，排查方向会被带偏 |
| `GET /cgi-bin/menu/delete` ⇄ 「GET 是查询、可安全重试」 | 删除是写操作且整树覆盖，动词仍是官方指定的 GET（AG1 断言 `typeof(GetAttribute)`） | 放进轮询、自动重试或幂等封装里，一次误触发整树消失；把 GET 当「无副作用」还会绕过自己的写操作审批 |
| 本族「POST + GET + GET」⇄ 同域工作台族「5 支全 POST」 | 工作台族连仅查询语义的 `get_workbench_template` / `get_workbench_data` 都是 POST（AG1 断言）；本族创建 POST、获取与删除 GET | 按「Agent 域都是 POST」统一动词会打红 AG1；自行拼 URL 时动词错会被网关或官方拒绝 |
| 创建 / 删除返回裸 `WechatWorkResponse` ⇄ 获取返回 `GetAgentMenuResponse` | 前两者只有 `errcode` / `errmsg`（AG1 断言不得为无负载端点新建响应 DTO） | 从创建或删除的响应里读 `Button` 会拿到 null（类型上根本不存在），误判为「写入的菜单丢了」 |
| `AgentMenuButton.SubButton` 的递归类型 ⇄ 官方两层口径 | C# 侧是 `List<AgentMenuButton>`，可无限嵌套；官方注释只给「一级 1~3、二级 1~5」的数量限制，未定义第三层的官方行为（§7） | 按类型能写出三层树，官方可能拒绝或静默丢弃，只有回读 E2 才会发现层级没生效 |
| `AgentMenuButton.Name` 的字节上限 ⇄ 字符数直觉 | 上限是**字节**：一级 ≤16、二级 ≤40；UTF-8 每个汉字 3 字节 | 写五个汉字的一级菜单名即触上限；SDK 不预检，失败只以 `errcode` 回来 |
| `WechatAppConfig.AgentId`（`string`）⇄ 端点参数 `agentid`（`int`） | 配置项注释原文「业务可选，仅作展示/路由用途」，不参与 `Validate()`；端点参数是必填 `int` | 把 `config["Wechat:AgentId"]` 直接传给方法编不过；反过来以为「配置里填了 AgentId，SDK 会自动带上」就会漏传参数 |
| 一个 `type` 走天下 ⇄ 9 种 `type` 各有必填字段 | `click`→`key`、`view`→`url`、`view_miniprogram`→`appid` + `pagepath`；其余 6 种注释未列必填字段 | 只写 `type` + `name` 的 `click` 按钮缺 `key`，点击后消息事件无法对上，线上表现为「点了没反应」 |
| 「删除菜单」⇄ 「隐藏 / 下线某个入口」 | 本族没有按单项删除，也没有隐藏或停用菜单的端点；能做的只有整树重写或整树删除 | 想下线一个入口却去调 E3，整棵树一起没了；正确做法是用 E1 写入一份少了该入口的新树 |

另外三条 SDK 侧行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 为已接受风险，无法改用 Header）。G5 的 Query 令牌白名单收录了 `IWechatWorkAgentMenuService` 与 `IWechatWorkInternalAgentMenuService`；`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo。
- **不自动判错、不做参数校验**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`；个数、字节数、`type` 合法性、`appid` 绑定关系都在官方侧判定。
- **归属键与本族方法面上的「同一个 `access_token`」不是一回事**：`Wechat.AccessToken@Internal` 只声明在自建子接口上，父接口不声明。把本族接口注入到第三方或代开发应用上下文，会在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；E1/E3 的成功只代表请求被受理，业务结果要回读 E2 |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效 / 无效 | 本族失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效（部分接口） | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少 `access_token` 参数 / 凭据缺失 | 不自愈：装配缺陷信号（未 `AddWechatApp`、未 `AddAgentApi()`，或 `CorpId` / `AgentSecret` 不完整） |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号（误用套件级客户端） |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号 |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks 留档）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重新注入 → 重试」，业务侧通常看不到这些码；若稳定出现，说明自建应用 `AgentSecret` 本身在失败，加重试不解决。

本族各端点的专属业务错误码（如按钮个数越界、`appid` 未绑定、菜单结构非法对应的码）接口注释未留档 ⇒ §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AG1~AG5 + 两份接口与三个 DTO 的 XML 注释 + 域 README 2026-10-18）；文档号照抄注释，**90231 / 90232 / 90233 的页面最后更新日期与正文文案未逐项核验** | 逐页渲染取文后回填元信息表 |
| 三端点的频率数值 | 接口注释未留档 QPS / 日配额 / 月配额 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 端点专属错误码集合 | 除 §6 的令牌族与 `41001` 外，注释未留档任何端点级 `errcode` 表；按钮越界、结构非法、`appid` 未绑定的具体码 **未逐项核验** | 官方 90231 / 90232 / 90233 与全局错误码（90455） |
| E1 是否为「整树替换」 | 注释只写「为指定应用创建自定义菜单」，**未留档**重复调用是替换整棵树还是合并，也未说明被省略字段是保持还是清零 ⇒ 本文只给「先回读、改后再回读」的处置 | 官方 90231；必要时用测试企业实测 |
| 容器按钮是否需要 / 禁止 `type` | 含 `sub_button` 的一级按钮在本文示例里只给 `name`；官方注释未说明这种按钮是否允许或必须省略 `type` / `key` ⇒ **未逐项核验** | 官方 90231 的菜单结构示例 |
| 菜单层数超过两级 | C# 类型是递归的（`SubButton` 类型仍是 `List<AgentMenuButton>`），可写出三层；官方注释只定义一级与二级的数量限制，第三层的官方行为（拒绝、忽略还是报错）**未逐项核验** | 官方 90231；核验前不要按类型可表达能力假设官方支持 |
| 「应用没有菜单」时的 E2 响应形态 | 注释未说明是 `errcode` 非 0、`button` 缺字段，还是空数组 ⇒ 本文按「`null` 与空数组都视为无菜单」处理，两者是否可区分 **未逐项核验** | 官方 90232；可对新应用实测 |
| `agentid` 与令牌所属应用不一致时的行为 | 注释未留档「只能操作当前凭证对应的应用」这类限制（获取应用族有类似表述，本族没有）⇒ 传入同企业其它应用的 `agentid` 会怎样 **未逐项核验** | 官方 90231 / 90232 / 90233 权限说明 |
| `agentid` 的取值范围 / 上限 | 注释只写「可在应用管理设置页查看」，无区间 ⇒ 不得据本文设本地校验 | 官方 90231 |
| 写入后用户端生效时延 | 注释未留档菜单是否即时生效、是否需要重新进入会话、是否有缓存 ⇒ **未逐项核验** | 官方 90231 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联源码链接与官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 同域兄弟族（共用 `WechatModule.Agent` 与 `Agent` 注册组）：[获取应用 / 设置应用 / 应用权限 / 管理员列表（Agent.md）](./Agent.md) · [工作台自定义展示（AgentWorkbench.md）](./AgentWorkbench.md) · [自建应用迁移成代开发（AgentMigration.md）](./AgentMigration.md)
- 域总览（族清单 + 14 支端点速览 + 令牌矩阵 + 契约陷阱汇总）：[Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Agent/README.md)
- 契约守卫源码：[WechatAgentContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) · 接口命名空间分区 [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) · 跨线计数基线 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)
- DTO 与 JSON 上下文落位：[DataModels/Agent/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Agent/)（本族 3 型登记在 `AgentJsonContext`，`SerializerClassName = Agent`）
- 本线包级公开面与已踩陷阱：[Src/Work/Mud.Wechat.Work/README.md](../../../../../Src/Work/Mud.Wechat.Work/README.md)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与域 README「令牌与认证」一节
- 官方页（文档号照接口注释留档，本次未重取正文）：[创建菜单（90231）](https://developer.work.weixin.qq.com/document/path/90231) · [获取菜单（90232）](https://developer.work.weixin.qq.com/document/path/90232) · [删除菜单（90233）](https://developer.work.weixin.qq.com/document/path/90233)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
