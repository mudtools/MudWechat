# 政民沟通 · 配置事件类别

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Gov` 域 · 事件类别族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkGovEventCategoryService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/EventCategory/IWechatWorkGovEventCategoryService.cs)（父，承载全部 4 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/EventCategory/IWechatWorkGovEventCategoryService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/EventCategory/IWechatWorkGovEventCategoryService_Provider.cs)（均空标记，**无第三方子接口**） |
| DTO 源文件 | [DataModels/Gov/EventCategory/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Gov/EventCategory/)（`SerializerClassName = Gov`，全域 45 型由 GV4 统一登记进 `GovJsonContext`；**子目录仅作组织，全域 DTO 命名空间统一为 `Mud.Wechat.Work.DataModels.Gov`**） |
| 契约守卫 | [WechatGovContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatGovContractGuards.cs)（**GV1c** 事件类别 4 支路由 / GV2 层级（自建 + 代开发空标记、无第三方子接口）/ GV3 令牌绑定 / GV4 全域 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [94536](https://developer.work.weixin.qq.com/document/path/94536) / 94537 / 94538 / [94540](https://developer.work.weixin.qq.com/document/path/94540) · 代开发 97141 / 97142 / 97143 / [99548](https://developer.work.weixin.qq.com/document/path/99548) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：GV1c/GV2/GV3/GV4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（端点速览 1~4、域级契约、§4 明细）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

政民沟通「居民联系」的**上报事件类别字典**（一级 / 二级两层），4 支端点：

| 端点 | 用途 |
|---|---|
| E1 `report/grid/add_cata` | 添加一级或二级事件类别，返回 `category_id` |
| E2 `report/grid/update_cata` | 修改类别（名称 / 层级 / 所属一级分类） |
| E3 `report/grid/delete_cata` | 删除类别 |
| E4 `report/grid/list_cata` | 取全部已配置类别；**官方无请求包体** |

这套类别是网格员在**巡查上报**（[Patrol.md](./Patrol.md)）与**居民上报**（[Resident.md](./Resident.md)）时填入的事件分类，也是两支 `category_statistic` 响应里 `dashboard_list` 的分类来源——**先配类别，再谈上报与统计**。

形态（GV2）：官方对**自建与代开发开放完全一致的 4 支** ⇒ 全收敛父接口 + 两个空标记子接口；**第三方对配置事件类别域全部端点标注「暂不支持」⇒ 本家族不设第三方子接口**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider`（**无第三方子接口**） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（GV3 五族统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**居民联系 - 可调用接口的应用**」中；代开发应用支持，需**勾选「居民联系权限」**；第三方暂不支持。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）——这条同时写在父接口与代开发子接口注释里，是政民沟通全域的前置门槛。
- 类别量纲：`category_name` **不超过 30 个字**；**同一一级分类下的二级分类名字不能一样**；`level` **只能传 1 或 2**；**`level = 2` 时 `parent_category_id` 必传**。
- E4 **无请求包体**（官方契约，只有 `access_token` Query 参数）——本仓因此没有 `GovGetEventCategoryListRequest` 类型。
- 类别名称是业务配置数据、不含个人数据，但上报事件描述（在 Patrol/Resident 侧）含地点与描述，勿整块落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅ | ❌（权限表逐支标注「暂不支持」） | ✅（需勾选居民联系权限） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkGovEventCategoryService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **4** | 否 |
| `IWechatWorkInternalGovEventCategoryService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderGovEventCategoryService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Gov`，装配入口 `AddGovApi()`（模块枚举 `WechatModule.Gov`）；子接口 `InheritedFrom = nameof(WechatWorkGovEventCategoryService)`。

## 4. 端点

### E1. `AddEventCategoryAsync` — 添加事件类别

```csharp
[Post("/cgi-bin/report/grid/add_cata")]
Task<GovAddEventCategoryResponse> AddEventCategoryAsync(
    [Body] GovAddEventCategoryRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/report/grid/add_cata`（**事件类别的路由在 `report/grid/` 段下**，见 §5） |
| 令牌 | Query `access_token` |

**请求参数**（`GovAddEventCategoryRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `category_name` | `CategoryName` | `string?` | **是** | 分类名称，**≤30 字**；同一一级分类下二级名不可重复 |
| `level` | `Level` | `int?` | **是** | **只能传 1 或 2** |
| `parent_category_id` | `ParentCategoryId` | `string?` | 条件 | **`level = 2` 时必传**（所属一级分类 id） |

**响应参数**（`GovAddEventCategoryResponse`）：`category_id`（`string?`，新创建的分类 id）。

### E2. `UpdateEventCategoryAsync` — 修改事件类别

```csharp
[Post("/cgi-bin/report/grid/update_cata")]
Task<WechatWorkResponse> UpdateEventCategoryAsync(
    [Body] GovUpdateEventCategoryRequest request, CancellationToken cancellationToken = default);
```

- 请求：`category_id`（必填）+ `category_name`（必填，≤30 字）+ `level`（必填，1/2）+ `parent_category_id`（`level = 2` 时必传）——**四字段面比 E1 多一个 `category_id`，其余同为整单提交**。
- 响应：`WechatWorkResponse`（仅 `errcode` / `errmsg`）。

### E3. `DeleteEventCategoryAsync` — 删除事件类别

```csharp
[Post("/cgi-bin/report/grid/delete_cata")]
Task<WechatWorkResponse> DeleteEventCategoryAsync(
    [Body] GovDeleteEventCategoryRequest request, CancellationToken cancellationToken = default);
```

- 请求：`category_id`（`string?`，官方必填）。
- 注释未给「删除一级分类时其下二级分类如何处理」的官方口径 ⇒ 见 §7。

### E4. `GetEventCategoryListAsync` — 获取事件类别列表

```csharp
[Post("/cgi-bin/report/grid/list_cata")]
Task<GovGetEventCategoryListResponse> GetEventCategoryListAsync(CancellationToken cancellationToken = default);
```

- **官方本端点无请求包体** ⇒ 方法签名里没有 `[Body]` 形参，只有 `cancellationToken`。
- 响应 `category_list[]`（`GovEventCategoryInfo`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `category_id` | `CategoryId` | `string?` | 分类 id |
| `category_name` | `CategoryName` | `string?` | 分类名称 |
| `level` | `Level` | `int?` | **1 一级分类 / 2 二级分类** |
| `parent_category_id` | `ParentCategoryId` | `string?` | 所属一级分类 id；**层级为 1 时该字段为空** |

> 字段名以官方 JSON 示例 `category_list` 为准（**参数说明表误写为 `cata_list`**，守卫按示例锁定，见 §5）。

**调用示例**（E1 建一级 → 在其下建两个二级 → E4 读回并按层级归组）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Gov;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "gov-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddGovApi());

var categories = provider.GetRequiredService<IWechatWorkInternalGovEventCategoryService>();

// ① 一级分类：level=1 时不传 parent_category_id
var root = await categories.AddEventCategoryAsync(
    new GovAddEventCategoryRequest { CategoryName = "市政设施", Level = 1 }, cancellationToken);
WechatWorkException.ThrowIfFailed(root, "/cgi-bin/report/grid/add_cata");
string rootCategoryId = root.CategoryId ?? throw new InvalidOperationException("响应未返回 category_id");

// ② 二级分类：level=2 时 parent_category_id 必传；同一父级下名称不可重复
var childIds = new List<string>();
foreach (string name in new[] { "路灯损坏", "井盖缺失" })
{
    var child = await categories.AddEventCategoryAsync(
        new GovAddEventCategoryRequest { CategoryName = name, Level = 2, ParentCategoryId = rootCategoryId },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(child, "/cgi-bin/report/grid/add_cata");
    if (child.CategoryId is { Length: > 0 } childId) childIds.Add(childId);
}

// ③ 列表：官方无请求包体，只发 access_token
var all = await categories.GetEventCategoryListAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(all, "/cgi-bin/report/grid/list_cata");

var roots = new List<GovEventCategoryInfo>();
var childrenByParent = new Dictionary<string, List<GovEventCategoryInfo>>(StringComparer.Ordinal);
foreach (GovEventCategoryInfo item in all.CategoryList ?? new List<GovEventCategoryInfo>())
{
    if (item.Level == 1 || string.IsNullOrEmpty(item.ParentCategoryId))
    {
        roots.Add(item);
        continue;
    }
    if (!childrenByParent.TryGetValue(item.ParentCategoryId!, out var bucket))
        childrenByParent[item.ParentCategoryId!] = bucket = new List<GovEventCategoryInfo>();
    bucket.Add(item);
}
logger.LogInformation("一级分类 {RootCount} 个，其下二级分类共 {ChildCount} 个",
    roots.Count, childrenByParent.Values.Sum(list => list.Count));

// ④ 改名：update 请求体是「整单提交」形态，category_id / category_name / level 都要带齐
if (childIds.Count > 0)
{
    var renamed = await categories.UpdateEventCategoryAsync(
        new GovUpdateEventCategoryRequest
        {
            CategoryId = childIds[0],
            CategoryName = "路灯不亮",
            Level = 2,
            ParentCategoryId = rootCategoryId,
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(renamed, "/cgi-bin/report/grid/update_cata");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 路由前缀 `report/grid/` vs `report/patrol/` / `report/resident/` | **事件类别四支挂在 `grid/` 段下**（`add_cata` 等），但它是与「网格结构」平行的另一族；上报侧的同类统计在 `patrol/`、`resident/` 段 | 以为类别端点在 `report/` 根下或 `resident/` 下 → 路由写错 |
| `category_list` vs `cata_list` | 官方**响应 JSON 示例**为 `category_list`，**参数说明表误写 `cata_list`**；本仓以示例为准（DTO 注释同文留档） | 按参数表建模 → 反序列化取不到列表 |
| 「无请求包体」的 E4 | 官方本端点就没有 body ⇒ 方法签名只接受 `cancellationToken`，本域也**没有**对应 Request 类型 | 给 E4 造请求体 → 打坏 GV1c/GV4 |
| `level` 的取值 | **只能 1 或 2**（两层封顶，不像网格结构支持 10 层） | 传 3 → 参数错误 |
| `parent_category_id` 的条件必填 | `level = 2` 时必传；`level = 1` 时不传，且**列表响应里一级分类该字段为空** | 一级分类硬塞父 id |
| 同名规则的作用域 | 「同一**一级分类**下的二级分类名字不能一样」——不同父级下可同名 | 以为全局唯一而加去重逻辑 |
| E2 的提交形态 | 请求体含 `category_id` + `category_name` + `level`（+ `parent_category_id`），**不是**「只传要改的字段」的补丁形态 | 只传 name 以为其余不动 |
| 删除的级联语义 | 注释未给「删一级是否连带二级」⇒ **未逐项核验**（见 §7），别假设与网格删除同规则 | 按网格「连带子树」直觉处理 |
| 与网格结构族的分工 | 本族配**事件分类字典**；网格树（谁管哪块地）在 [Grid.md](./Grid.md)；上报工单在 [Patrol.md](./Patrol.md) / [Resident.md](./Resident.md) | 在类别端点里找成员/负责人配置 |
| 应用类型 | **无第三方子接口**（官方逐支「暂不支持」）；代开发须勾选居民联系权限 | 注入第三方接口（不存在） |
| 系统应用 secret | **2023-12-01 0 点起不再支持系统应用 secret 调用**（存量企业暂不受影响） | 沿用系统应用凭证 → 新企业直接不可用 |
| 动词 | 4 支全 POST（含 E4 读取） | 把 E4 改 GET → 打坏 GV1c 与 G5 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做本地必填校验**（`level` / `category_name` / `parent_category_id` 的条件必填由官方校验）、**不校验 30 字与同名规则**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功仍要判 `category_id` 是否返回（后续建二级要用它） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

名称超长、同名冲突、`level` 非法、`parent_category_id` 缺失或不存在、类别条数上限等业务码**接口注释未逐条留档** ⇒ 见 §7。E2/E3 返回体只有 `errcode` / `errmsg`，业务失败信息无处可取，HTTP 仍 200。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 类别条数上限 | 注释给了名称长度、层级、同名规则，**未给一级/二级类别的数量上限** ⇒ **未逐项核验** | 官方 94536 参数表 |
| 删除一级分类的级联语义 | 注释未说明其下二级分类是否一并删除、是否要求先删空 ⇒ 未核验 | 官方 94538 |
| 类别是否可跨上报族共用 | 注释说网格员在「巡查上报」与「居民上报」时填入配置的类别，**未说明两族是否共用同一套类别** ⇒ 未核验 | 官方 94536 + [Patrol.md](./Patrol.md) / [Resident.md](./Resident.md) 的 `category_statistic` |
| `category_id` 的稳定性 | 未说明修改层级或父级后 id 是否变化 ⇒ 未核验 | 官方 94537 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方 94536~94540 + 全局错误码页 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验，勿从通讯录域借数值 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（21 支端点速览 #1~#4、令牌表、逐端点契约明细与守卫 GV1c/GV2/GV3/GV4）
- 同域：[Grid.md](./Grid.md)（网格树：谁管哪块地，10 层 / 20 负责人 / 100 成员）· [Patrol.md](./Patrol.md) · [Resident.md](./Resident.md)（两支消费本族类别的的上报族，`category_statistic` 的分类来源）
- 官方页：[添加事件类别（94536）](https://developer.work.weixin.qq.com/document/path/94536) · [修改事件类别（94537）](https://developer.work.weixin.qq.com/document/path/94537) · [删除事件类别（94538）](https://developer.work.weixin.qq.com/document/path/94538) · [获取事件类别列表（94540）](https://developer.work.weixin.qq.com/document/path/94540)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
