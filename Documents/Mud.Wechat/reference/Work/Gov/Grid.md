# 政民沟通 · 配置网格结构

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Gov` 域 · 网格结构族（`GovGridService`）+ 网格列表族（`GovGridListService`） |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkGovGridService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Grid/IWechatWorkGovGridService.cs)（父，承载 E1/E2/E3/E4）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Grid/IWechatWorkGovGridService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Grid/IWechatWorkGovGridService_Provider.cs)（空标记）；[IWechatWorkGovGridListService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Grid/IWechatWorkGovGridListService.cs)（父，承载 E5）· [IWechatWorkInternalGovGridListService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Grid/IWechatWorkGovGridListService_Internal.cs)（**唯一子接口，无代开发 / 第三方**） |
| DTO 源文件 | [DataModels/Gov/Grid/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Gov/Grid/)（`SerializerClassName = Gov`，全域 45 型由 GV4 统一登记进 `GovJsonContext`；**子目录仅作组织，全域 DTO 命名空间统一为 `Mud.Wechat.Work.DataModels.Gov`**） |
| 契约守卫 | [WechatGovContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatGovContractGuards.cs)（**GV1a** 网格结构 4 支路由 / **GV1b** 网格列表 1 支路由 / GV2 层级（网格结构族两空标记子接口 + **网格列表族继承链恰只有自建子接口**）/ GV3 令牌绑定 / GV4 全域 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [94478](https://developer.work.weixin.qq.com/document/path/94478) / 94479 / 94480 / [94482](https://developer.work.weixin.qq.com/document/path/94482) / [94481](https://developer.work.weixin.qq.com/document/path/94481)（网格列表）· 代开发 97136 / 97137 / 97138 / 97140 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：GV1a/GV1b/GV2/GV3/GV4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（端点速览 5~9、域级契约、明细）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

政民沟通「居民联系」的**网格树**（谁管哪一块地），5 支端点、**两个接口族**：

| 端点 | 用途 | 开放面 |
|---|---|---|
| E1 `report/grid/add` | 添加网格节点（父节点 + 负责人 + 成员），返回 `grid_id` | 自建 + 代开发 |
| E2 `report/grid/update` | 编辑节点（名称 / 父节点 / 负责人 / 成员） | 自建 + 代开发 |
| E3 `report/grid/delete` | 删除节点及其**全部子网格** | 自建 + 代开发 |
| E4 `report/grid/get_user_grid_info` | 查某成员**管理的**与**参与的**网格 | 自建 + 代开发 |
| E5 `report/grid/list` | 按 `grid_id` 取「目标网格的父网格 + 全部子网格」；**不填 = 取根节点及其子节点** | **仅自建** |

E5 之所以单独成族：官方权限表把 `grid/list` 标注为**代开发与第三方均「暂不支持」**，与 E1–E4 的开放面**不等宽** ⇒ 本仓把它拆成 `IWechatWorkGovGridListService` + 唯一自建子接口，并由 **GV2 的能力漂移守卫**锁「继承链上恰好只有自建子接口」。**不要**把它并回 E1–E4 那族，也不要为它补代开发子接口。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | E1–E4：`Internal` / `Provider`；E5：**仅** `Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（GV3 五族统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**居民联系 - 可调用接口的应用**」中；代开发应用支持，需**勾选「居民联系权限」**；第三方对网格结构全部端点标注「暂不支持」。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 结构与人数上限（父接口注释逐条留档）：
  - `grid_name` **≤30 个字**；**同一个目标网格下不能存在同名的同级子网格**；
  - 网格结构层级**最多 10 层**；
  - 每个网格**至少 1 个、最多 20 个负责人**（`grid_admin`）；
  - 成员列表（`grid_member`）**不超过 100 个**；
  - **同一成员最多能成功担任 10 个网格的管理员**（超出者进 `invalid_userids`）。
- E2 的 `grid_member` **为空表示清空所有成员**（E1 侧无此清空语义）。
- E3 的前提：**目标网格及其子网格中不包含网格员时**才可删除「目标网格 + 其全部子网格」；**根节点的 `grid_id` 可以填空**。
- E5 的 `grid_id` 不填 = 拉**根节点及其儿子节点**（不是全树）。
- 成员 userid 是员工标识：示例用假值，负责人 / 成员名单不要整块落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅ | ❌ | ✅（勾选居民联系权限） |
| E5 | ✅ | ❌（暂不支持） | ❌（**暂不支持 ⇒ 无代开发子接口**） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkGovGridService` | `Mud.Wechat.Work.Interfaces` | 网格结构父接口 | **4**（E1–E4） | 否 |
| `IWechatWorkInternalGovGridService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderGovGridService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkGovGridListService` | `Mud.Wechat.Work.Interfaces` | 网格列表父接口（`IsAbstract = true`） | **1**（E5） | 否 |
| `IWechatWorkInternalGovGridListService` | `Mud.Wechat.Work` | 自建子接口（**继承链上唯一的子接口**） | 0 | 是 |

注册组 `Gov`，装配入口 `AddGovApi()`。两个族的父接口都带 `IsAbstract = true`，可注入面是两个自建/代开发子接口；**取网格列表只应注入 `IWechatWorkInternalGovGridListService`**。

## 4. 端点

### E1. `AddGridAsync` — 添加网格

```csharp
[Post("/cgi-bin/report/grid/add")]
Task<GovAddGridResponse> AddGridAsync(
    [Body] GovAddGridRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/report/grid/add` |
| 令牌 | Query `access_token` |

**请求参数**（`GovAddGridRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `grid_name` | `GridName` | `string?` | **是** | ≤30 字；同一目标网格下不可同名同级 |
| `grid_parent_id` | `GridParentId` | `string?` | **是** | 父节点网格 id；**层级最多 10 层** |
| `grid_admin` | `GridAdmin` | `List<string>?` | **是** | 负责人 userid，**≥1 且 ≤20** |
| `grid_member` | `GridMember` | `List<string>?` | 否 | 成员 userid，**≤100**；同一成员最多成功担任 10 个网格的管理员 |

**响应参数**（`GovAddGridResponse`）：`grid_id`（新网格 id）+ `invalid_userids`（**不合法的 userid 列表**，如超 10 个管理员上限的成员）。

### E2. `UpdateGridAsync` — 编辑网格

```csharp
[Post("/cgi-bin/report/grid/update")]
Task<GovUpdateGridResponse> UpdateGridAsync(
    [Body] GovUpdateGridRequest request, CancellationToken cancellationToken = default);
```

- 请求：`grid_id`（必填）+ `grid_name`（必填）+ `grid_parent_id`（必填）+ `grid_admin`（必填，1~20）+ `grid_member`（**为空表示清空所有成员**）。
- 响应：`invalid_userids`（**不回传 `grid_id`**）。
- 与 E1 的差别不只是多一个 `grid_id`：**四个字段都是必填** ⇒ 编辑是「整单提交」，不是补丁式更新。

### E3. `DeleteGridAsync` — 删除网格

```csharp
[Post("/cgi-bin/report/grid/delete")]
Task<WechatWorkResponse> DeleteGridAsync(
    [Body] GovDeleteGridRequest request, CancellationToken cancellationToken = default);
```

- 请求：`grid_id`（`string?`，官方必填，**根节点可以填空**）。
- 语义：**连带删除其全部子网格**；前提是**目标网格及其子网格中不包含网格员**。

### E4. `GetUserGridInfoAsync` — 获取用户负责及参与的网格列表

```csharp
[Post("/cgi-bin/report/grid/get_user_grid_info")]
Task<GovGetUserGridInfoResponse> GetUserGridInfoAsync(
    [Body] GovGetUserGridInfoRequest request, CancellationToken cancellationToken = default);
```

- 请求：`userid`（`string?`，官方必填）。
- 响应：`manage_grids[]`（作为**管理员**管理的网格）与 `joined_grids[]`（作为**成员**参与的网格），元素同为 `GovGridBrief`＝`grid_id` + `grid_name`（**两个列表是同一类型**）。
- **读取语义却 POST**（GV1a 逐字锁定）。

### E5. `GetGridListAsync` — 获取网格列表（仅自建）

```csharp
[Post("/cgi-bin/report/grid/list")]
Task<GovGetGridListResponse> GetGridListAsync(
    [Body] GovGetGridListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `grid_id` | `GridId` | `string?` | 否 | **不填 = 拉根节点及其儿子节点** |

**响应参数**：`grid_list[]`（`GovGridInfo`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `grid_id` | `GridId` | `string?` | 网格 id |
| `grid_name` | `GridName` | `string?` | 网格名称（**字符串**；官方响应示例把它误标成数字 `"grid_name": 1`，参数说明表明确是名称，本仓照字符串建模） |
| `grid_parent_id` | `GridParentId` | `string?` | 父节点网格 id |
| `grid_admin[]` / `grid_member[]` | `GridAdmin` / `GridMember` | `List<string>?` | 管理员 / 成员 userid 列表 |

**调用示例**（E1 建子网格 → E5 从根逐层展开 → E4 查某成员网格 → E2 调整成员）

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

var grid = provider.GetRequiredService<IWechatWorkInternalGovGridService>();
// 网格列表官方仅向自建开放 ⇒ 只有自建子接口可注入（无代开发 / 第三方子接口）
var gridList = provider.GetRequiredService<IWechatWorkInternalGovGridListService>();

// ① 取根层：grid_id 不填 = 根节点及其儿子节点
var top = await gridList.GetGridListAsync(new GovGetGridListRequest(), cancellationToken);
WechatWorkException.ThrowIfFailed(top, "/cgi-bin/report/grid/list");
string? parentId = (top.GridList is { Count: > 0 } list ? list[0].GridId : null)
    ?? throw new InvalidOperationException("未取到根层网格");

// ② 在某个父网格下新增子网格：负责人 1~20，成员 ≤100
var added = await grid.AddGridAsync(
    new GovAddGridRequest
    {
        GridName = "示例第一网格",           // ≤30 字，同父级下不可同名
        GridParentId = parentId,            // 层级最多 10 层
        GridAdmin = new List<string> { "zhangsan" },   // 假值
        GridMember = new List<string> { "lisi", "wangwu" },   // 假值
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/report/grid/add");

// invalid_userids 是「部分失败」信号：超 10 个管理员上限等成员被剔除，但请求整体成功
if (added.InvalidUserids is { Count: > 0 } invalid)
    logger.LogWarning("新增网格 {GridId} 有 {Count} 个不合法 userid 被剔除（详情不落日志）",
        added.GridId, invalid.Count);

// ③ 查某成员负责及参与的网格：两支列表同元素类型，读法一致
var mine = await grid.GetUserGridInfoAsync(new GovGetUserGridInfoRequest { Userid = "zhangsan" }, cancellationToken);
WechatWorkException.ThrowIfFailed(mine, "/cgi-bin/report/grid/get_user_grid_info");
logger.LogInformation("成员 {Userid}：管理 {Managed} 个网格，参与 {Joined} 个网格",
    "zhangsan", mine.ManageGrids?.Count ?? 0, mine.JoinedGrids?.Count ?? 0);

// ④ 编辑：grid_name / grid_parent_id / grid_admin 三个字段仍必填；grid_member 传空集合 = 清空成员
var updated = await grid.UpdateGridAsync(
    new GovUpdateGridRequest
    {
        GridId = added.GridId,
        GridName = "示例第一网格（改名）",
        GridParentId = parentId,
        GridAdmin = new List<string> { "lisi" },
        GridMember = new List<string>(),      // 空 = 清空所有成员
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/report/grid/update");

// ⑤ 删除：连带全部子网格，前提是目标及其子网格中不含网格员；根节点 grid_id 可填空
var deleted = await grid.DeleteGridAsync(new GovDeleteGridRequest { GridId = added.GridId }, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/report/grid/delete");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `grid/list`（E5）vs `grid/get_user_grid_info`（E4） | E5 按**网格**取「父 + 全部子」结构视图（**仅自建**）；E4 按**人**取「管理的 / 参与的」两个列表（自建 + 代开发） | 想查某人负责的网格却调 E5；代开发想调 E5 → 根本没有该子接口 |
| 两个接口族 | 官方开放面不等宽 ⇒ E5 独立成族且继承链只有自建子接口（GV2 能力漂移守卫） | 并回一族 / 给 E5 补代开发子接口 → 守卫打红 |
| E1 vs E2 的必填面 | E1 四要素里 `grid_member` 可选；E2 的 `grid_name` / `grid_parent_id` / `grid_admin` **均必填**（整单提交） | 只传要改的字段 → 参数缺失 |
| `grid_member` 传空 | **E2 里空集合 = 清空全部成员**；E1 里没有该清空语义 | 想「不改成员」却传了空集合 |
| `grid_admin` vs `grid_member` | 前者是**负责人**（1~20，且每人最多成功担任 10 个网格的管理员）；后者是成员（≤100） | 把负责人塞进成员列表 → 网格无负责人 |
| `invalid_userids` | **部分失败信号**：请求整体成功但个别 userid 被剔除；E1 回 `grid_id` + `invalid_userids`，**E2 只回 `invalid_userids`** | 只看 `errcode == 0` 就以为所有人都挂上了 |
| `grid_name` 的响应类型 | 官方**响应示例**误标为数字、**参数说明表**是字符串 ⇒ 本仓照字符串建模（守卫锁定，勿「顺手修正」） | 按 int 反序列化 → 直接抛 |
| E3 的连带删除 | 删除「目标网格 **+ 其全部子网格**」，且要求目标及其子网格中**不含网格员** | 以为只删单点 → 子树没了；有网格员时反复重试 |
| 根节点 `grid_id` 填空 | E3 明确「根节点的 grid_id 可以填空」，与 E1 的 `grid_parent_id` 必填不同 | 给根节点删除硬造一个 id |
| 10 层 vs 30 字 vs 100 成员 | 三个上限各管一件事（深度 / 名称长度 / 成员数），**负责人上限是 1~20** | 把 20 当成员上限 |
| 与事件类别族的路由同段 | 本族与 [EventCategory.md](./EventCategory.md) 都在 `/cgi-bin/report/grid/` 下（`add` 与 `add_cata` 等），**不是**同一族 | 按前缀归族 → 误以为类别端点在别处 |
| 应用类型 | 第三方对本域全部端点「暂不支持」⇒ 无第三方子接口；E5 连代开发也不开放 | 注入不存在的子接口 |
| 系统应用 secret | **2023-12-01 0 点起不再支持**（存量企业暂不受影响） | 用系统应用凭证新建接入 |
| 动词 | 5 支全 POST（两支读取 E4/E5 也 POST） | 改 GET → 打坏 GV1a/GV1b 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验层级深度 / 人数上限 / 同名规则**（超限由官方拒绝或静默剔除进 `invalid_userids`）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | **E1/E2 成功仍要判 `invalid_userids`**（部分成员被剔除） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`grid_id` 不存在、超过 10 层、同级同名、负责人数量越界、含网格员导致不可删除等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `invalid_userids` 的成因清单 | 注释只说「不合法的 userid 列表」，未列成因（超 10 网格上限 / 非可见范围 / 不存在）⇒ **未逐项核验** | 官方 94478 / 94479 |
| 网格总数与单父下子节点数上限 | 注释给层级 ≤10、负责人 ≤20、成员 ≤100，**未给节点总数** ⇒ 未核验 | 官方 94478 |
| E5 的返回边界 | 注释说「目标网格的父网格及全部子网格」，未说明是否含目标自身、是否递归到叶（多层）⇒ **未逐项核验** | 官方 94481 |
| E3 含网格员时的错误码与文案 | 注释给前提，未给失败码 ⇒ 未核验 | 官方 94480 |
| 「网格员」与 `grid_member` / `grid_admin` 的对应关系 | 删除前提提到「网格员」，本域未定义该角色是 admin 还是 member ⇒ 未核验 | 官方 94480 + 巡查上报文档 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方 94478~94482 + 全局错误码页 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（21 支端点速览 #5~#9、令牌表、逐端点明细、守卫 GV1a/GV1b/GV2「能力漂移」说明）
- 同域：[EventCategory.md](./EventCategory.md)（同段路由的事件类别字典）· [Patrol.md](./Patrol.md) / [Resident.md](./Resident.md)（`get_grid_info` 是各自族**自己**的网格视图，与本族 E5 不同路由与不同响应）
- 官方页：[添加网格（94478）](https://developer.work.weixin.qq.com/document/path/94478) · [编辑网格（94479）](https://developer.work.weixin.qq.com/document/path/94479) · [删除网格（94480）](https://developer.work.weixin.qq.com/document/path/94480) · [获取网格列表（94481）](https://developer.work.weixin.qq.com/document/path/94481) · [获取用户网格信息（94482）](https://developer.work.weixin.qq.com/document/path/94482)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
