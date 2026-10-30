# 政民沟通 · 巡查上报

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Gov` 域 · 巡查上报族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkGovPatrolService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Patrol/IWechatWorkGovPatrolService.cs)（父，**零端点**）· [IWechatWorkInternalGovPatrolService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Patrol/IWechatWorkGovPatrolService_Internal.cs)（承载全部 6 支） |
| DTO 源文件 | [DataModels/Gov/Patrol/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Gov/Patrol/) 与共享的 [DataModels/Gov/Report/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Gov/Report/)（`GovReportGridInfo` / `GovReportCategoryStatistic` / `GovReportLocation` / `GovReportProcessItem`；`SerializerClassName = Gov`，全域 45 型由 GV4 统一登记进 `GovJsonContext`；**子目录仅作组织，全域 DTO 命名空间统一为 `Mud.Wechat.Work.DataModels.Gov`**） |
| 契约守卫 | [WechatGovContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatGovContractGuards.cs)（**GV1d** 巡查上报 6 支路由 / GV2 层级（零端点父接口 + 继承链恰 1 个自建子接口）/ GV3 令牌绑定 / GV4 全域 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建：网格 [93531](https://developer.work.weixin.qq.com/document/path/93531) · 单位统计 93532 · 个人统计 93533 · 分类统计 93534 · 事件详情 93535 · 事件列表 93536（**代开发与第三方均无对应开放面**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：GV1d/GV2/GV3/GV4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（端点速览 10~15、明细、守卫官方反直觉点）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

网格员**巡查上报**侧的只读数据面，6 支端点分三层：

| 层 | 端点 | 用途 |
|---|---|---|
| 结构 | E1 `patrol/get_grid_info` | 巡查上报已配置的网格及各网格管理员（**本域两支 GET 之一**） |
| 统计 | E2 `get_corp_status` | 单位/指定网格维度概况（待分配、办理中、今日上报、今日办结、累计上报、累计办结） |
| | E3 `get_user_status` | 单个成员概况（办理中、今日上报、今日办结） |
| | E4 `category_statistic` | 按事件分类的累计上报 / 累计办结 |
| 工单 | E5 `get_order_list` | `cursor` + `limit` 分页取工单列表 |
| | E6 `get_order_info` | 按 `order_id` 取单工单详情（结构与列表元素一致） |

本族**只有读取**，没有任何「提交上报 / 派单 / 办结」的写端点——上报动作发生在企业微信客户端里，本 SDK 只做数据取回。

形态（GV2）：官方**仅向企业自建应用开放全部 6 支** ⇒ 零端点父接口 + 唯一自建子接口承载（对齐身份验证二次验证族、企业支付域、会话内容存档域）。**代开发文档页（97146~97158）与自建页逐字一致，但权限表同为「暂不支持」**——**不得据此为两族补代开发子接口**（守卫 remarks 原文点名这条）。

与居民上报族的对照见 [Resident.md](./Resident.md)（同名六支、`resident/` 段路由、统计多两格、工单多上报人三字段）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（无代开发 / 第三方子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（GV3 断言） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 自建应用须配置到「**巡查上报 - 可调用接口的应用**」中；**代开发应用与第三方应用均暂不支持**。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- **E3 获取个人数据统计时，成员必须在应用可见范围内**。
- E2 的 `grid_id` 不传 = **取整个企业的概况**；E4 的 `category_id` 不传 = 取**所有一级分类**，传一级分类 id = 取该一级分类下的**所有二级分类**。
- E5 分页三约束（注释原文）：`cursor` **首次查询为空、查询条件有变更需置空**；`limit` **不填默认 20 条、最大 50**；`next_cursor` **为空字符串代表已是最后一页**。
- E6 的 `order_id` **不为空时其他参数无效**（官方 `get_order_list` 请求体与详情共用同一形状）。
- 工单内容（事件描述 `desc`、发生地点经纬度、图片/视频、处理人 userid、流程备注）属**敏感业务与个人数据**：示例用假值，工单详情**不要整体落日志**；`video_media_ids` 是素材 id、非长期凭据。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E6 | ✅（E3 另需成员在可见范围内） | ❌ | ❌（**文档页存在但权限表「暂不支持」**） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkGovPatrolService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalGovPatrolService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 6 支**） | **6** | 是 |

注册组 `Gov`，装配入口 `AddGovApi()`；子接口 `InheritedFrom = nameof(WechatWorkGovPatrolService)`。GV2 锁「继承链上不得出现代开发 / 第三方子接口」。

## 4. 端点

### E1. `GetGridInfoAsync` — 获取配置的网格及网格负责人

```csharp
[Get("/cgi-bin/report/patrol/get_grid_info")]
Task<GovPatrolGetGridInfoResponse> GetGridInfoAsync(CancellationToken cancellationToken = default);
```

- **GET 且无业务参数**（除 `access_token`）；**无请求 DTO**（本域不存在 `GovPatrolGetGridInfoRequest`）。
- 响应 `grid_list[]`（`GovReportGridInfo`）：`grid_id`（`string?`）、`grid_name`（**字符串**——官方响应示例误标为数字，参数说明表明确为名称，本仓照字符串）、`grid_admin[]`（管理员 userid 列表）。

### E2. `GetCorpStatusAsync` — 获取单位巡查上报数据统计

```csharp
[Post("/cgi-bin/report/patrol/get_corp_status")]
Task<GovPatrolGetCorpStatusResponse> GetCorpStatusAsync(
    [Body] GovPatrolGetCorpStatusRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `grid_id`（请求） | `GridId` | `string?` | 不传 = 整个企业概况 |
| `to_be_assigned` | `ToBeAssigned` | `int?` | **待分配**数量 |
| `processing` | `Processing` | `int?` | 办理中 |
| `added_today` | `AddedToday` | `int?` | 今日上报 |
| `solved_today` | `SolvedToday` | `int?` | 今日办结 |
| `total_case` | `TotalCase` | `int?` | 累计上报 |
| `total_solved` | `TotalSolved` | `int?` | 累计办结 |

> 「待分配」`to_be_assigned` 是**巡查侧独有**的一格；居民侧对应的是「待受理」`pending` + 「累计受理」`total_accepted`（见 [Resident.md](./Resident.md)）。

### E3. `GetUserStatusAsync` — 获取个人巡查上报数据统计

```csharp
[Post("/cgi-bin/report/patrol/get_user_status")]
Task<GovPatrolGetUserStatusResponse> GetUserStatusAsync(
    [Body] GovPatrolGetUserStatusRequest request, CancellationToken cancellationToken = default);
```

- 请求：`userid`（`string?`，官方必填，**须在应用可见范围内**）。
- 响应只有三格：`processing` / `added_today` / `solved_today`（**个人侧没有「待分配」**）。

### E4. `GetCategoryStatisticAsync` — 获取上报事件分类统计

```csharp
[Post("/cgi-bin/report/patrol/category_statistic")]
Task<GovPatrolGetCategoryStatisticResponse> GetCategoryStatisticAsync(
    [Body] GovPatrolGetCategoryStatisticRequest request, CancellationToken cancellationToken = default);
```

- 请求：`category_id`（`string?`，不传 = 所有一级分类；传一级 = 该级下所有二级）。
- 响应 `dashboard_list[]`（`GovReportCategoryStatistic`）：`category_id` / `category_name` / **`category_level`（1 一级分组 / 2 二级分类）** / `total_case`（累计上报）/ `total_solved`（累计办结）/ **`category_type`（0 其他 / 1 有效分类）**。

### E5. `GetOrderListAsync` — 获取巡查上报事件列表

```csharp
[Post("/cgi-bin/report/patrol/get_order_list")]
Task<GovPatrolGetOrderListResponse> GetOrderListAsync(
    [Body] GovPatrolGetOrderListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `begin_create_time` | `BeginCreateTime` | `long?` | 只返回该时间戳之后**新创建**的上报 |
| `begin_modify_time` | `BeginModifyTime` | `long?` | 只返回该时间戳之后**新修改**的上报 |
| `cursor` | `Cursor` | `string?` | 首次为空；**查询条件变更须置空** |
| `limit` | `Limit` | `int?` | **默认 20，最大 50** |
| `next_cursor`（响应） | `NextCursor` | `string?` | **空字符串 = 最后一页** |
| `order_list[]`（响应） | `OrderList` | `List<GovPatrolOrder>?` | 工单列表，见下表 |

`GovPatrolOrder`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `order_id` | `OrderId` | `string?` | 工单 id |
| `desc` | `Desc` | `string?` | 事件描述（敏感文本，勿落日志） |
| `urge_type` | `UrgeType` | `int?` | **1 一般 / 2 重要 / 3 紧急** |
| `case_name` / `grid_name` / `grid_id` | 同名 | `string?` | 事件类别名称 / 所属网格 |
| `create_time` | `CreateTime` | `long?` | 创建时间（Unix 秒） |
| `image_urls[]` / `video_media_ids[]` | 同名 | `List<string>?` | 工单图片 URL / 视频素材 id |
| `location` | `Location` | `GovReportLocation?` | `name` / `address` / `latitude`（**double，90~-90**）/ `longitude`（**double，180~-180**） |
| `processor_userids[]` | `ProcessorUserids` | `List<string>?` | **当前流程**处理人 |
| `process_list[]` | `ProcessList` | `List<GovReportProcessItem>?` | 历史流程，见下 |

`GovReportProcessItem`：`process_type`（**1 创建 / 2 受理 / 3 分配 / 4 转交 / 5 办结 / 6 拒绝 / 7 办理中**；官方参数表口径：**巡查上报含「1 创建」，居民上报自「2 受理」起**）、`solve_userid`（该流程办结人）、`process_desc`、`status`（**0 处理中 / 1 已处理**）、`solved_time`、`image_urls[]`、`video_media_ids[]`。

### E6. `GetOrderInfoAsync` — 获取巡查上报的事件详情信息

```csharp
[Post("/cgi-bin/report/patrol/get_order_info")]
Task<GovPatrolGetOrderInfoResponse> GetOrderInfoAsync(
    [Body] GovPatrolGetOrderInfoRequest request, CancellationToken cancellationToken = default);
```

- 请求：`order_id`（`string?`，官方必填）；**不为空时其他参数无效**。
- 响应：`order_info`（`GovPatrolOrder`，**结构与 E5 列表元素完全一致**）。

**调用示例**（E1 取网格 → E2/E4 取概况与分类统计 → E5 增量分页拉工单 → E6 取单条详情）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Gov;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "gov-a";
    o.AppType = WechatAppType.Internal;   // 官方仅自建开放：无代开发 / 第三方子接口
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddGovApi());

var patrol = provider.GetRequiredService<IWechatWorkInternalGovPatrolService>();

// ① 网格及负责人：本域两支 GET 之一，无业务参数
var grids = await patrol.GetGridInfoAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(grids, "/cgi-bin/report/patrol/get_grid_info");
string? firstGridId = grids.GridList is { Count: > 0 } gridList ? gridList[0].GridId : null;

// ② 单位概况：grid_id 不传 = 整个企业；巡查侧独有「待分配 to_be_assigned」
var corp = await patrol.GetCorpStatusAsync(
    new GovPatrolGetCorpStatusRequest { GridId = firstGridId }, cancellationToken);
WechatWorkException.ThrowIfFailed(corp, "/cgi-bin/report/patrol/get_corp_status");
logger.LogInformation("巡查上报：待分配 {ToBeAssigned}，办理中 {Processing}，今日上报 {Added}，累计办结 {Solved}",
    corp.ToBeAssigned, corp.Processing, corp.AddedToday, corp.TotalSolved);

// ③ 分类统计：不传 category_id 取一级，传一级 id 取其下二级
var categoryStat = await patrol.GetCategoryStatisticAsync(
    new GovPatrolGetCategoryStatisticRequest(), cancellationToken);
WechatWorkException.ThrowIfFailed(categoryStat, "/cgi-bin/report/patrol/category_statistic");
foreach (GovReportCategoryStatistic row in categoryStat.DashboardList ?? new List<GovReportCategoryStatistic>())
{
    // category_type：0 其他 / 1 有效分类；category_level：1 一级 / 2 二级
    logger.LogInformation("分类 {CategoryId}（{Level} 级）上报 {Case} 办结 {Solved}",
        row.CategoryId, row.CategoryLevel, row.TotalCase, row.TotalSolved);
}

// ④ 工单增量拉取：begin_create_time 之后新创建；limit 默认 20 最大 50；next_cursor 空串即最后一页
long beginCreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 7 * 86400L;
string? cursor = null;
do
{
    var page = await patrol.GetOrderListAsync(
        new GovPatrolGetOrderListRequest { BeginCreateTime = beginCreateTime, Cursor = cursor, Limit = 50 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/report/patrol/get_order_list");

    foreach (GovPatrolOrder order in page.OrderList ?? new List<GovPatrolOrder>())
    {
        // 只记标识与计数：desc、地点经纬度、图片/视频、处理人名单属敏感数据，不落日志
        logger.LogInformation("工单 {OrderId} 紧急度 {Urge}，流程节点 {Steps}",
            order.OrderId, order.UrgeType, order.ProcessList?.Count ?? 0);
    }
    cursor = page.NextCursor;
    // 查询条件若发生变化（如换 begin_create_time），须把 cursor 置空重新起一轮
} while (!string.IsNullOrEmpty(cursor));

// ⑤ 详情：order_id 不为空时其他参数无效；结构就是列表元素
var detail = await patrol.GetOrderInfoAsync(new GovPatrolGetOrderInfoRequest { OrderId = "ORDER假值" }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/report/patrol/get_order_info");

foreach (GovReportProcessItem step in detail.OrderInfo?.ProcessList ?? new List<GovReportProcessItem>())
{
    // process_type 1 创建 / 2 受理 / 3 分配 / 4 转交 / 5 办结 / 6 拒绝 / 7 办理中；status 0 处理中 / 1 已处理
    logger.LogInformation("流程 {ProcessType} 状态 {Status}，办结时间 {SolvedTime}",
        step.ProcessType, step.Status, step.SolvedTime);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族 vs [Resident.md](./Resident.md) | 六支**同名同形**，只差路由段 `patrol/` vs `resident/`，**且 DTO 是两套类型**（`GovPatrol*` vs `GovResident*`） | 用巡查的响应类型反序列化居民响应 → 类型不匹配 |
| 巡查统计 vs 居民统计 | 单位概况：巡查有 **`to_be_assigned`（待分配）**；居民有 **`pending`（待受理）+ `total_accepted`（累计受理）**。个人概况：巡查三格，居民四格（多 `pending`） | 照搬字段清单 → 居民侧「待受理」恒为 null |
| `process_type` 起点 | 官方参数表口径：**巡查上报含「1 创建」**，**居民上报自「2 受理」起** | 在居民工单里找「创建」节点 |
| 网格视图有两支同名 `get_grid_info` | 本族 E1（`patrol/get_grid_info`）与居民族（`resident/get_grid_info`）路由、响应类型都不同；**与「配置网格结构」族的 `grid/list` / `get_user_grid_info`（[Grid.md](./Grid.md)）又是另一套** | 拿上报侧的网格视图当网格配置源 |
| `grid_name` 的类型 | 官方响应示例误标为数字（`"grid_name": 1`），参数说明表是名称 ⇒ 本仓**照字符串**建模（守卫 remarks 明令勿「顺手修正」） | 按 int 反序列化直接抛 |
| E2 的 `grid_id` 不传 | 语义是「整个企业概况」，不是报错 | 以为必填而拒绝取全企业数据 |
| E4 的 `category_id` 两态 | 不传 = 一级分类集合；传**一级** id = 其下二级集合（**传二级 id 的行为未定义** ⇒ §7） | 传二级 id 以为取明细 |
| E5 的 `cursor` 置空时机 | **查询条件有变更就必须置空**重起一轮 | 改了 `begin_create_time` 还续旧 cursor → 漏单或重复 |
| `next_cursor` 判完 | **空字符串**代表最后一页（本域不是「字段不返回」形态） | 只判 `!= null` → 空转 |
| `limit` 上限 | 默认 20、**最大 50**（远小于本域其它列表端点的 100/1000） | 传 200 期望大页 |
| E6 的其他参数 | `order_id` 不为空时**其他参数无效**（详情与列表共用形状） | 以为可叠加筛选 |
| E1 的动词 | 官方即 **GET 且无请求体**；其余五支读取同样 POST（GV1d 逐字锁定） | 给 E1 造 Request 类型 / 改 POST |
| 代开发文档页 | 代开发页（97146~97158）与自建页**逐字一致但权限表「暂不支持」** ⇒ 不设代开发子接口 | 据文档页存在就补子接口（GV2 打红） |
| 写端点缺失 | 本族**无任何上报提交 / 派单 / 办结写端点**，动作在客户端侧 | 在本域找写接口 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不做可见范围校验**、**不做分页游标管理**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E5 成功仍须按 `next_cursor` 是否为空串决定是否续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`grid_id` / `category_id` / `order_id` 不存在、成员不在可见范围（E3）、`cursor` 失效等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 未逐条摘录 ⇒ **未逐项核验** | 官方 93531~93536 + 全局错误码页 |
| E4 传**二级** `category_id` 的行为 | 注释只给「不传 = 一级集合、传一级 = 其下二级」，未给传二级 id 的口径 ⇒ 未核验 | 官方 93534 |
| `begin_create_time` / `begin_modify_time` 同时给的关系 | 注释分别描述两个筛选，未说明是「与」还是「或」、是否有跨度限制 ⇒ **未逐项核验** | 官方 93536 |
| `cursor` 的有效期 | 注释未给 ⇒ 未核验 | 官方 93536 |
| 工单量 / 时间范围上限 | 未给可查询的历史跨度（对比统计类端点的 30 天 / 180 天口径，本族未写）⇒ 未逐项核验 | 官方 93536 |
| `image_urls` 与 `video_media_ids` 的消费方式 | URL 是否需鉴权、素材 id 是否经「获取临时素材」换取 ⇒ 注释未说明，未核验 | 官方 93535 + 素材接口 |
| `urge_type` / `category_type` 取值是否会扩展 | 注释给当前枚举，未说明扩展策略 ⇒ 未核验 | 官方 93534 / 93535 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（21 支速览 #10~#15、逐端点明细、守卫 remarks 的「官方反直觉点」清单）
- 同域：[Resident.md](./Resident.md)（居民上报六支，统计与工单字段的差异全在此处对照）· [Grid.md](./Grid.md)（网格配置与人员归属）· [EventCategory.md](./EventCategory.md)（`case_name` / `category_statistic` 的分类字典来源）
- 官方页：[获取配置的网格及网格负责人（93531）](https://developer.work.weixin.qq.com/document/path/93531) · [单位巡查上报数据统计（93532）](https://developer.work.weixin.qq.com/document/path/93532) · [个人（93533）](https://developer.work.weixin.qq.com/document/path/93533) · [分类统计（93534）](https://developer.work.weixin.qq.com/document/path/93534) · [事件详情（93535）](https://developer.work.weixin.qq.com/document/path/93535) · [事件列表（93536）](https://developer.work.weixin.qq.com/document/path/93536)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
