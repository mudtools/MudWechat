# Reports 域 · 报表

腾讯广告 Marketing API v3.0「报表」域：日报表 / 小时报表 / 异步报表任务，共 **4 支端点、跨 3 支资源族**
（`daily_reports` 1 支 + `hourly_reports` 1 支 + `async_reports` 2 支）。
接口 [IWechatAdsReportService.cs](./IWechatAdsReportService.cs)，注册组 `Reports`，模块枚举 `AdsModule.Reports`。

> 本 README 只汇编**已在接口 XML 注释与契约守卫中留档**的事实。逐参数类型、数组长度、字节上限以
> [IWechatAdsReportService.cs](./IWechatAdsReportService.cs) 为准；路由 / 动词 / 族划分 / 三支 `level` 集合的权威以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。
> 三者冲突时以守卫 + 接口注释为准并**就地修正本文**。

## 为什么一支接口装三支资源族

官方把 `daily_reports` / `hourly_reports` / `async_reports` 分成三族、各有 `*` 动作，但三支的权限（全部
`ads_insights`）、信封与分页形态同构，且异步链路的语义是一体的（`add` 拿 `task_id` → `get` 读 `status` 与 `file_id`）
⇒ 收在一支接口、一个注册组，而不是为凑「一族一接口」拆出三个空模块。

**合并模块 ≠ 合并契约面**：族级差异（三支 `level` 集合、分页上限、`organization_id` 有无）全部留在逐方法参数面，
守卫既按路由前缀再断言一次族划分（`daily_reports` 1 / `hourly_reports` 1 / `async_reports` 2），
又逐页锁定参数名集合与 CLR 可空性。**不得**因为「同在一个接口」就把三支签名对齐。

## 端点速览

| 方法 | 动词 | 路由 | 权限 | `user_token` | 幂等头 |
|---|---|---|---|---|---|
| `GetDailyAsync` | GET | `/v3.0/daily_reports/get` | `ads_insights` | **不带** | 无 |
| `GetHourlyAsync` | GET | `/v3.0/hourly_reports/get` | `ads_insights` | **不带** | 无 |
| `AddAsyncReportAsync` | POST | `/v3.0/async_reports/add` | `ads_insights` | **不带** | **无**（重试会真的再建一个任务） |
| `GetAsyncReportAsync` | GET | `/v3.0/async_reports/get` | `ads_insights` | **不带** | 无 |

动词形态是本域最反直觉的一点：**四支里三支 GET、一支 POST**。本仓「多数查询类接口官方即 POST」的经验
在报表族**不成立**（留档 §7.8 已把它列为族级结论）⇒ 照录，不「顺手对齐」成 POST。守卫逐条数断言
（GET 恰 3、POST 恰 1 且必须是 `async_reports/add`）。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddReportsApi());

public sealed class DailyCostSync(IWechatAdsReportService reports)
{
    public async Task<PullResult> PullAsync(long accountId, string day, CancellationToken ct)
    {
        var resp = await reports.GetDailyAsync(
            accountId,
            level: "REPORT_LEVEL_ADVERTISER",
            dateRange: AdsQueryJson.DateRange(new AdsDateRange
            {
                StartDate = day,   // YYYY-MM-DD，10 字节
                EndDate = day,
            }),
            fields: AdsQueryJson.Fields("date", "account_id", "cost", "view_count", "valid_click_count"),
            groupBy: AdsQueryJson.GroupBy("date"),
            page: 1,
            pageSize: 1000,
            cancellationToken: ct);

        WechatAdsException.ThrowIfFailed(resp, "/v3.0/daily_reports/get");
        return new PullResult(resp.Data?.List, resp.Data?.PageInfo);
    }
}

public readonly record struct PullResult(List<Dictionary<string, JsonElement>>? Rows, AdsPageInfo? PageInfo);
```

同步两页的复合参数（`date_range` / `fields` / `group_by` / `filtering` / `order_by`）**全部必填或全部可选按下表**，
且全部是 `string` 形态的 JSON 字面量，构造入口唯一 `AdsQueryJson`。

## 三支 `level` 集合互不相同（守卫逐页锁定，禁止合并）

`level` 是查询层级，三支页面上的可选值集合**是官方页面之间的真实差异**，不是笔误：

| 页面 | 集合大小 | 相对形态 |
|---|---|---|
| `daily_reports/get` | **17** | 基准集合 |
| `hourly_reports/get` | **8** | daily 的**真子集**（无组件 / 素材 / 营销资产 / 商品 / 版位等层级） |
| `async_reports/add` | **21** | 在 daily 上**增**人口属性与落地页等 7 支（`AGE` / `GENDER` / `REGION` / `CITY` / `LANDING_PAGE` / `AD_UNION` / `OS`）、**删** `VIDEO_HIGHLIGHT` / `WECHAT_SHOP_PRODUCT` / `PLAYLET` 3 支 |

用 daily 的集合调 hourly 页会被官方拒绝。⇒ **不得**把三支合并成一支公共枚举，`level` 一律以 `string` 承载、
由调用方传官方原文值。daily 页原文另注明「查询业务单元报表时 `level` 只支持组件层级」。

## 逐页参数差异速查

| 维度 | daily | hourly | async_reports/add | async_reports/get |
|---|---|---|---|---|
| 日期 | `date_range`（JSON 对象），最早 1 年内、任意跨度 | `date_range`，**≤90 天且跨度 1 天、`start_date` 必须等于 `end_date`** | **无 `date_range`**：单支 `date` + `granularity`（`HOURLY`→最近 30 天、`DAILY`→最近 365 天） | 无 |
| 字段清单 | `fields` | `fields` | 叫 **`report_fields`** | 无 |
| `group_by` | 必填，数组 1–10、**每项 ≤255 字节** | 必填，数组 1–10、**每项 ≤64 字节** | **选填**，上限 **5** | 无 |
| `filtering` | 数组 1–40，`field` 16 支 | 数组 1–40，`field` 8 支（daily 的子集） | 数组上限 **5** | 数组 1–5，`field` **只有 `task_id` / `task_name` 两支** |
| `operator` | 普遍 `{EQUALS, IN}`；`smart_delivery_platform` 另允许 `NOT_EQUALS` | `{EQUALS, IN}` | `{EQUALS, CONTAINS, LESS_EQUALS, LESS, GREATER_EQUALS, GREATER, IN, NOT_EQUALS}` | 同 async add（8 支） |
| `page` | 1–99999，默认 1 | **1–100**，默认 1 | 无 | 1–99999，默认 1 |
| `page_size` | 1–2000，默认 10 | 1–2000，默认 10 | 无 | **1–100**，默认 10 |
| `organization_id` | **有**（0–9999999999） | **没有** | — | **有**（0–9999999999） |
| `account_id` 必填面 | 官方**未加星**但描述实际必填 ⇒ CLR 取**非可空** | **有**星号 ⇒ 非可空 | 必填 | 标**选填** ⇒ CLR 可空 |
| `time_line` | `{REQUEST_TIME, REPORTING_TIME, ACTIVE_TIME}` | 同 | — | — |

`hourly_reports/get` **没有** `organization_id` ⇒ 签名里没有该参数；「为了和 daily 一致而补一个」会发送官方未定义的参数。

## 两条只写在「使用说明」里的上限（SDK 不本地拦截）

1. **同步两页 `page * page_size <= 20000`**，超限须改用 `AddAsyncReportAsync`。
   hourly 页并列的「`page` 上限 100」与该乘积式只在 `page_size = 200` 附近自洽——官方两约束并列即矛盾，照录。
2. **`async_reports/add` 的创建频率**：官方原文「每账号（`account_id`）限制最多 5 分钟创建 1 个异步报表任务，
   多种级别异步报表任务算多个」。这是 v3.0 已核验页面里**唯一**一处按账号计的**创建频率**上限。
   SDK **不做进程内节流**：多实例部署下进程节流既挡不住跨实例并发、又会在单实例误伤合法调用，
   节奏归调用方（分布式节流属宿主侧能力，不在 SDK 内造第二套时钟）。

## 应答形态

| 端点 | 应答类型 | `data` 载荷 |
|---|---|---|
| `GetDailyAsync` / `GetHourlyAsync` | `AdsDailyReportResponse` / `AdsHourlyReportResponse` | 共用 `AdsReportListData`：报表行列表 + `page_info` |
| `AddAsyncReportAsync` | `AdsAsyncReportAddResponse` | 只回 `task_id` |
| `GetAsyncReportAsync` | `AdsAsyncReportGetResponse` | 任务列表（`task_id` / `task_name` / `status` / `created_time` / `result.file_info_list[]{file_id, md5}`）+ `page_info` |

- **报表行的键集由请求的 `fields` 决定** ⇒ 未请求的指标不会出现在应答里；取分版位数据须**同时**在
  `fields` 与 `group_by` 里带 `site_set`。
- hourly 应答示例含约 150 支指标（`hour` / `cost` / `view_count` / `valid_click_count` / `ctr` / `cpc` /
  `thousand_display_price` 等），但实际键集仍以 `fields` 为准。
- **全部金额类指标（`cost` 等）官方单位是分**，与 `advertiser` / `adgroups` 的预算、出价字段同一口径；SDK 不做单位换算。

## 轮询异步任务的正确写法

`status` **不在** `async_reports/get` 的可过滤集合里（该页 `field` 只有 `task_id` / `task_name`）
⇒ 「轮询到完成」必须按 `task_id` 过滤后**读回元素的 `Status`**，
不能把 `TASK_STATUS_COMPLETED` 当过滤条件传。

两条官方时限（原文）：「异步任务获取接口支持 **7 日内**任务记录的查询，单生成任务文件下载有效期为 **24 小时**」
⇒ 任务记录只在 7 日窗口内可查；拿到 `file_id` 后必须在 24 小时内完成下载，否则只能重建任务。

## 本域到 `file_id` 为止：下载端点未落地

`file_id` + `task_id` 要交给 `async_report_files/get`，而那页的请求地址**逐字是另一台主机**
`https://dl.e.qq.com/v3.0/async_report_files/get`（官方 curl 同页逐字给出）。本线目前只有一条指向 `api.e.qq.com`
的业务客户端 ⇒ 该端点**尚未落地**（需先决定第二条基址的客户端形态）。

守卫把「未落地」钉成**可见的**计数——本域路由表恰 4 支而非 5 支，而不是留一条空断言假装覆盖。
SSRF 白名单以后缀域 `e.qq.com` 一条接入，`dl.e.qq.com` 已在放行面内，缺的只是客户端形态。

## 官方自相矛盾清单（照录，SDK 一律不校验）

`daily_reports/get` 四处：① `account_id` 未加星却实际必填（必填性取描述而非取星号）；② `end_date` 的约束引用参数表里
**不存在**的 `begin_date`；③ `group_by` / `fields` 标必填但官方示例两支都没给；④ `level` 集合缺 `async_reports/add`
上出现的 7 支（该页另删 3 支）。

`hourly_reports/get` 三处：① `end_date` 又引用不存在的 `begin_date`；② 示例把 `start_date=2024-01-02` /
`end_date=2024-01-01` 写反，**违反该页自己的「start 必须等于 end」约束**；③ 示例应答 `page_info.total_number = 1`
而 `list` 有 2 项。

`async_reports/add` 一处：官方示例把必填的 `report_fields` 整个省略。

⇒ SDK 不替官方「修正」，也不把这些矛盾做成本地校验；矛盾本身登记在 `.docs` 留档 §9。

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数类型 / 数组长度 / 字节上限 / 条件必填原文 | [IWechatAdsReportService.cs](./IWechatAdsReportService.cs) 的 XML 注释 |
| 路由 / 动词 / 族划分 / 端点计数（恰 4） | `AdsContractGuards` 的 `ReportsEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `ReportsRoutes` |
| 三支 `level` 集合 | 同守卫的 `DailyReportLevels`(17) / `HourlyReportLevels`(8) / `AsyncReportAddLevels`(21) |
| 逐页参数名集合与 CLR 可空性 | `ReportQueryParameters_ShouldMatchPerPageOfficialTables`、`ReportQueryParameterNullability_ShouldFollowPerPageStar` |
| 本域四页均无 `user_token` | 同上一条守卫点名（`ADS-B5` 的登记面管不住「不该带的参数」） |
| 同步/异步上限、7 日与 24 小时时限 | 接口 remarks + 留档 §7.4 / §7.8 / §9 |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{daily_reports/get,hourly_reports/get,async_reports/add,async_reports/get}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 注释补齐官方文档 URL + 逐页上限 / 时限 / 条件必填，**同批**更新 `ReportsRoutes`
   与族划分断言（新增端点未入表即红）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取命名空间域段 `Reports`）→
   跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 更新守卫的上下文登记断言。
3. 若要落地 `async_report_files/get`：**先决定第二基址（`dl.e.qq.com`）的客户端形态**，再动契约面与守卫计数
   （4 → 5），并同步删除本域 README 与接口 remarks 里的「未落地」表述。
4. **不要**把三支 `level` 合并成公共枚举，**不要**给 hourly 补 `organization_id`，**不要**把三支 GET「对齐」成 POST。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
