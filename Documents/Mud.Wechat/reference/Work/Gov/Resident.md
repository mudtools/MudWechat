# 政民沟通 · 居民上报

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Gov` 域 · 居民上报族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkGovResidentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Resident/IWechatWorkGovResidentService.cs)（父，**零端点**）· [IWechatWorkInternalGovResidentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/Resident/IWechatWorkGovResidentService_Internal.cs)（承载全部 6 支） |
| DTO 源文件 | [DataModels/Gov/Resident/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Gov/Resident/) 与共享的 [DataModels/Gov/Report/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Gov/Report/)（`GovReportGridInfo` / `GovReportCategoryStatistic` / `GovReportLocation` / `GovReportProcessItem`；`SerializerClassName = Gov`，全域 45 型由 GV4 统一登记进 `GovJsonContext`；**子目录仅作组织，全域 DTO 命名空间统一为 `Mud.Wechat.Work.DataModels.Gov`**） |
| 契约守卫 | [WechatGovContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatGovContractGuards.cs)（**GV1e** 居民上报 6 支路由 / GV2 层级（零端点父接口 + 继承链恰 1 个自建子接口）/ GV3 令牌绑定 / GV4 全域 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建：网格 [93514](https://developer.work.weixin.qq.com/document/path/93514) · 单位统计 93515 · 个人统计 93516 · 分类统计 93517 · 事件列表 93518 · 事件详情 93519（**代开发与第三方均无对应开放面**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：GV1e/GV2/GV3/GV4 + 接口与 DTO XML 注释（含子接口「与巡查上报族的差异」专段）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（端点速览 16~21、明细）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

**居民**上报（非网格员发起）侧的只读数据面，与 [Patrol.md](./Patrol.md) 同构的六支：

| 层 | 端点 | 用途 |
|---|---|---|
| 结构 | E1 `resident/get_grid_info` | 居民上报已配置的网格及各网格管理员（**本域两支 GET 之一**） |
| 统计 | E2 `get_corp_status` | 单位/指定网格概况（**待受理**、办理中、今日上报、今日办结、累计上报、**累计受理**、累计办结） |
| | E3 `get_user_status` | 单个成员概况（办理中、今日上报、今日办结、**待受理**） |
| | E4 `category_statistic` | 按事件分类的累计上报 / 累计办结 |
| 工单 | E5 `get_order_list` | `cursor` + `limit` 分页取工单列表（**含上报人信息**） |
| | E6 `get_order_info` | 按 `order_id` 取单工单详情 |

同样**只有读取**：居民提交上报的动作发生在客户端，本族只取回数据。

形态（GV2）：官方**仅向企业自建应用开放全部 6 支** ⇒ 零端点父接口 + 唯一自建子接口；代开发文档页与自建页内容一致、**权限表同为「暂不支持」**，不得据此补子接口。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（无代开发 / 第三方子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（GV3 断言） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 自建应用须配置到「**居民上报 - 可调用接口的应用**」中；代开发应用与第三方应用均暂不支持。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- **E3 获取个人数据统计时，成员必须在应用可见范围内**。
- E2 的 `grid_id` 不传 = 整个企业概况；E4 的 `category_id` 不传 = 所有一级分类，传一级 id = 其下所有二级分类。
- E5 分页：`cursor` 首次为空、**查询条件变更须置空**；`limit` **默认 20、最大 50**；`next_cursor` **空字符串 = 最后一页**。
- E6 的 `order_id` 不为空时**其他参数无效**。
- **居民上报的工单携带上报人个人信息**：`reporter_name`（上报人名称）、`reporter_mobile`（**手机号**）、`unionid`（企业绑定了微信开发者 ID 才有）。这三项是**自然人身份数据**，加上 `desc` / 地点经纬度 / 图片视频，构成完整的居民隐私面 ⇒ 示例用假值，**严禁整单落日志**；对外展示须先做脱敏。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E6 | ✅（E3 另需成员在可见范围内） | ❌ | ❌（**文档页存在但权限表「暂不支持」**） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkGovResidentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalGovResidentService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 6 支**） | **6** | 是 |

注册组 `Gov`，装配入口 `AddGovApi()`；子接口 `InheritedFrom = nameof(WechatWorkGovResidentService)`。

## 4. 端点

六支的方法名、请求形状、分页规则与 [Patrol.md](./Patrol.md) 的四、五、六节**一一对应**，本节只列居民侧的路由与**字段差异**。

### E1. `GetGridInfoAsync` — 获取配置的网格及网格负责人

```csharp
[Get("/cgi-bin/report/resident/get_grid_info")]
Task<GovResidentGetGridInfoResponse> GetGridInfoAsync(CancellationToken cancellationToken = default);
```

- **GET 且无业务参数**；响应 `grid_list[]`（`GovReportGridInfo`：`grid_id` / `grid_name`（字符串，官方示例误标为数字）/ `grid_admin[]`）。

### E2. `GetCorpStatusAsync` — 获取单位居民上报数据统计

```csharp
[Post("/cgi-bin/report/resident/get_corp_status")]
Task<GovResidentGetCorpStatusResponse> GetCorpStatusAsync(
    [Body] GovResidentGetCorpStatusRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 与巡查侧的差异 |
|---|---|---|---|
| `grid_id`（请求） | `GridId` | `string?` | 同：不传 = 整个企业 |
| `pending` | `Pending` | `int?` | **居民侧独有「待受理」**（巡查侧是 `to_be_assigned` 待分配） |
| `total_accepted` | `TotalAccepted` | `int?` | **居民侧独有「累计受理」** |
| `processing` / `added_today` / `solved_today` / `total_case` / `total_solved` | 同名 | `int?` | 同巡查侧 |

### E3. `GetUserStatusAsync` — 获取个人居民上报数据统计

```csharp
[Post("/cgi-bin/report/resident/get_user_status")]
Task<GovResidentGetUserStatusResponse> GetUserStatusAsync(
    [Body] GovResidentGetUserStatusRequest request, CancellationToken cancellationToken = default);
```

- 请求：`userid`（官方必填，须在应用可见范围内）。
- 响应四格：`processing` / `added_today` / `solved_today` + **`pending`（待受理）**——巡查侧个人概况只有三格。

### E4. `GetCategoryStatisticAsync` — 获取上报事件分类统计

```csharp
[Post("/cgi-bin/report/resident/category_statistic")]
Task<GovResidentGetCategoryStatisticResponse> GetCategoryStatisticAsync(
    [Body] GovResidentGetCategoryStatisticRequest request, CancellationToken cancellationToken = default);
```

- 与巡查侧同形：`dashboard_list[]`（`GovReportCategoryStatistic`：`category_id` / `category_name` / `category_level` 1 一级 / 2 二级 / `total_case` / `total_solved` / `category_type` 0 其他 / 1 有效分类）。

### E5. `GetOrderListAsync` — 获取居民上报事件列表

```csharp
[Post("/cgi-bin/report/resident/get_order_list")]
Task<GovResidentGetOrderListResponse> GetOrderListAsync(
    [Body] GovResidentGetOrderListRequest request, CancellationToken cancellationToken = default);
```

- 请求：`begin_create_time` / `begin_modify_time` / `cursor` / `limit`（默认 20、最大 50）；响应 `order_list[]` + `next_cursor`（空串即末页）。
- `GovResidentOrder` 在巡查侧 `GovPatrolOrder` 的字段面之上**多三列上报人信息**：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `reporter_name` | `ReporterName` | `string?` | **上报人名称**（居民，自然人） |
| `reporter_mobile` | `ReporterMobile` | `string?` | **上报人手机号** |
| `unionid` | `Unionid` | `string?` | 上报人 unionid，**企业绑定了微信开发者 ID 才有此字段** |

其余同巡查侧：`order_id` / `desc` / `urge_type`（1 一般 / 2 重要 / 3 紧急）/ `case_name` / `grid_id` / `grid_name` / `create_time` / `image_urls[]` / `video_media_ids[]` / `location`（`name` / `address` / `latitude` / `longitude`）/ `processor_userids[]` / `process_list[]`。

### E6. `GetOrderInfoAsync` — 获取居民上报的事件详情信息

```csharp
[Post("/cgi-bin/report/resident/get_order_info")]
Task<GovResidentGetOrderInfoResponse> GetOrderInfoAsync(
    [Body] GovResidentGetOrderInfoRequest request, CancellationToken cancellationToken = default);
```

- 请求：`order_id`（官方必填，**不为空时其他参数无效**）；响应 `order_info`（`GovResidentOrder`，与 E5 列表元素同形、同样含上报人三列）。
- 流程节点 `GovReportProcessItem.process_type`：**居民上报自「2 受理」起**（巡查侧才含「1 创建」）；`status` 0 处理中 / 1 已处理。

**调用示例**（E2 取待受理积压 → E5 增量分页取工单并只统计计数 → E4 分类分布）

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

var resident = provider.GetRequiredService<IWechatWorkInternalGovResidentService>();

// ① 单位概况：居民侧多「待受理 pending」与「累计受理 total_accepted」
var corp = await resident.GetCorpStatusAsync(new GovResidentGetCorpStatusRequest(), cancellationToken);
WechatWorkException.ThrowIfFailed(corp, "/cgi-bin/report/resident/get_corp_status");
logger.LogInformation("居民上报：待受理 {Pending}，办理中 {Processing}，累计受理 {Accepted}/累计上报 {Total}",
    corp.Pending, corp.Processing, corp.TotalAccepted, corp.TotalCase);

// ② 近 7 天新建工单增量拉取：limit 默认 20、最大 50；next_cursor 空字符串即最后一页
long beginCreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 7 * 86400L;
string? cursor = null;
int urgentCount = 0;
do
{
    var page = await resident.GetOrderListAsync(
        new GovResidentGetOrderListRequest { BeginCreateTime = beginCreateTime, Cursor = cursor, Limit = 50 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/report/resident/get_order_list");

    foreach (GovResidentOrder order in page.OrderList ?? new List<GovResidentOrder>())
    {
        // reporter_name / reporter_mobile / unionid / desc / 地点 均为居民个人数据：只计数，不打印
        if (order.UrgeType == 3) urgentCount++;
    }
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));
logger.LogInformation("近 7 天紧急（urge_type=3）居民上报工单 {Count} 单", urgentCount);

// ③ 分类分布：不传 category_id 取一级；传一级 id 取其下二级
var categoryStat = await resident.GetCategoryStatisticAsync(
    new GovResidentGetCategoryStatisticRequest(), cancellationToken);
WechatWorkException.ThrowIfFailed(categoryStat, "/cgi-bin/report/resident/category_statistic");

foreach (GovReportCategoryStatistic row in categoryStat.DashboardList ?? new List<GovReportCategoryStatistic>())
{
    // category_type 0 其他 / 1 有效分类
    logger.LogInformation("分类 {CategoryId} 上报 {Case}，办结 {Solved}，类型 {Type}",
        row.CategoryId, row.TotalCase, row.TotalSolved, row.CategoryType);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族 vs [Patrol.md](./Patrol.md) | 路由段 `resident/` vs `patrol/`，**方法名逐支相同**（`GetGridInfoAsync` / `GetCorpStatusAsync`…），**DTO 是两套类型**（`GovResident*` vs `GovPatrol*`） | 注错子接口 → 拿到另一族的数据；跨族复用响应类型 → 反序列化失败 |
| `pending`（待受理）vs `to_be_assigned`（待分配） | 居民侧用 `pending`、巡查侧用 `to_be_assigned`；**居民单位侧另有 `total_accepted`（累计受理）**，巡查侧没有 | 把两族字段清单当成一张表 → 恒 null |
| 个人概况格数 | 居民侧四格（含 `pending`），巡查侧三格 | 按巡查模板读个人数据漏掉待受理 |
| `process_type` 起点 | **居民上报自「2 受理」起**，巡查侧才含「1 创建」 | 在居民工单里找创建节点、或据此算「上报时效」 |
| 上报人三列 | `reporter_name` / `reporter_mobile` / `unionid` 只在**居民侧工单**出现；`unionid` 还要求**企业绑定了微信开发者 ID** | 按必有字段处理 unionid → 空引用；把手机号写进日志 |
| 网格视图同名 `get_grid_info` | 本族与巡查族、以及 [Grid.md](./Grid.md) 的 `grid/list` / `get_user_grid_info` **是三支不同端点、三套响应**（各自只能配各自的注入接口） | 拿居民侧网格视图当网格配置源 |
| `grid_name` 类型 | 官方示例误标数字、参数表为字符串 ⇒ 本仓照字符串（守卫明令勿修正） | 按 int 反序列化抛 |
| E5 的 `cursor` 置空时机 | 查询条件变更（如换 `begin_modify_time`）必须置空重起一轮 | 续旧 cursor → 漏单或重复 |
| `next_cursor` 判完 | **空字符串**代表最后一页 | 只判 `!= null` → 死循环 |
| E1 的动词 | 官方即 **GET 且无请求体**（本域两支 GET 之一）；其余五支读取也 POST | 给 E1 造 Request 类型 / 改 POST |
| 代开发文档页 | 代开发页与自建页逐字一致、**权限表「暂不支持」** ⇒ 不设代开发子接口 | 补子接口 → GV2 打红 |
| 写端点缺失 | 本族无任何「代居民提交 / 受理 / 办结」写端点 | 在本域找写接口 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不做可见范围校验**、**不做脱敏**（`reporter_mobile` 等原样返回，脱敏归宿主）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E5 成功仍须按 `next_cursor` 是否为空串决定续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`grid_id` / `category_id` / `order_id` 不存在、成员不在可见范围（E3）、`cursor` 失效等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 未逐条摘录 ⇒ **未逐项核验** | 官方 93514~93519 + 全局错误码页 |
| `reporter_mobile` 的可见性与授权前提 | 注释只说明该字段出现在工单里，**未给脱敏规则、授权前提或是否受可见范围影响** ⇒ 未逐项核验（合规处理前不得落库/落日志） | 官方 93518 / 93519 响应参数表 |
| `unionid` 的取得前提 | 注释只写「企业绑定了微信开发者 ID 有此字段」，未给绑定关系校验入口 ⇒ 未核验 | 官方 93518 + 微信开发者 ID 绑定文档 |
| E4 传**二级** `category_id` 的行为 | 注释只给两态，未覆盖传二级 id ⇒ 未核验 | 官方 93517 |
| 两个时间筛选的关系 | `begin_create_time` 与 `begin_modify_time` 同给是「与」还是「或」、有无跨度限制 ⇒ **未逐项核验** | 官方 93518 |
| `cursor` 有效期与工单可回溯跨度 | 注释未给 ⇒ 未核验 | 官方 93518 |
| `image_urls` / `video_media_ids` 的消费方式 | URL 是否需鉴权、素材 id 如何换取内容 ⇒ 注释未说明 | 官方 93519 + 素材接口 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Gov/README.md)（21 支速览 #16~#21、逐端点明细、守卫 remarks 的「官方反直觉点」）
- 同域：[Patrol.md](./Patrol.md)（巡查上报六支，本族的同构对照）· [EventCategory.md](./EventCategory.md)（分类字典）· [Grid.md](./Grid.md)（网格配置与人员归属）
- 合规提醒：本族返回的居民身份数据（姓名 / 手机号 / unionid / 位置）须按个人信息处理要求落地，SDK 不做脱敏
- 官方页：[获取配置的网格及网格负责人（93514）](https://developer.work.weixin.qq.com/document/path/93514) · [单位居民上报数据统计（93515）](https://developer.work.weixin.qq.com/document/path/93515) · [个人（93516）](https://developer.work.weixin.qq.com/document/path/93516) · [分类统计（93517）](https://developer.work.weixin.qq.com/document/path/93517) · [居民上报事件列表（93518）](https://developer.work.weixin.qq.com/document/path/93518) · [事件详情（93519）](https://developer.work.weixin.qq.com/document/path/93519)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
