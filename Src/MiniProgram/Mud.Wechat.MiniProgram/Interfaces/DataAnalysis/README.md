# DataAnalysis 域 · 数据分析

微信小程序「数据分析」域：**11 支端点**（访问趋势 3 + 访问留存 3 + 用户画像 1 + 访问分布 1 +
访问页面 1 + 数据概况 1 + 性能 1）。接口 [IWxaDataAnalysisService.cs](./IWxaDataAnalysisService.cs)，
注册组 `DataAnalysis`（`RegistryGroupName = "DataAnalysis"`、`TokenManage = nameof(IMpAppManager)`），
DI 组方法 [`AddDataAnalysisApi()`](../../Extensions/MiniProgramServiceBuilder.cs)（`MiniProgramModule.DataAnalysis`）。
接口 XML 记本域**逐页核验日为 2026-10-09**；本 README 的复核日为 **2026-10-18**。

> **事实来源限定**：本文所有断言只来自本仓的接口文件、契约守卫与 DTO 声明，
> 不引用任何外部文档原文。凡本仓核不到的（官方频率数字、错误码全集、`key`/`index` 之外的取值语义），
> 一律集中列在文末「已知留档缺口」并标 **未逐项核验**，不做推测。

## 端点速览

令牌域**复用** `MpTokenTypes.AccessToken`（值 `"Wechat.Mp.AccessToken"`），注入 **Query 的 `access_token`**
（守卫 MP-X2 / MP-X3）。本域 11 支**全部为 POST**（含 10 支语义上的查询）。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetDailyVisitTrendAsync` | POST | `/datacube/getweanalysisappiddailyvisittrend` | `access_token`(Query) | 体 `WxaDateRangeRequest`；跨度 1 天 |
| 2 | `GetWeeklyVisitTrendAsync` | POST | `/datacube/getweanalysisappidweeklyvisittrend` | `access_token`(Query) | 同上，周维度；上界为昨日所在周 |
| 3 | `GetMonthlyVisitTrendAsync` | POST | `/datacube/getweanalysisappidmonthlyvisittrend` | `access_token`(Query) | 同上，月维度 |
| 4 | `GetDailyRetainAsync` | POST | `/datacube/getweanalysisappiddailyretaininfo` | `access_token`(Query) | 跨度 1 天；`key` 语义见下 |
| 5 | `GetWeeklyRetainAsync` | POST | `/datacube/getweanalysisappidweeklyretaininfo` | `access_token`(Query) | `key` 语义同上 |
| 6 | `GetMonthlyRetainAsync` | POST | `/datacube/getweanalysisappidmonthlyretaininfo` | `access_token`(Query) | `key` 语义同上 |
| 7 | `GetUserPortraitAsync` | POST | `/datacube/getweanalysisappiduserportrait` | `access_token`(Query) | 跨度差 **0 / 6 / 29** 天；`ref_date` 为区间串 |
| 8 | `GetVisitDistributionAsync` | POST | `/datacube/getweanalysisappidvisitdistribution` | `access_token`(Query) | 跨度 1 天；`list[].index` 三枚举 |
| 9 | `GetVisitPageAsync` | POST | `/datacube/getweanalysisappidvisitpage` | `access_token`(Query) | 跨度 1 天；**只返回 top 200** |
| 10 | `GetDailySummaryAsync` | POST | `/datacube/getweanalysisappiddailysummarytrend` | `access_token`(Query) | 跨度 1 天；应答**无顶层 `ref_date`** |
| 11 | `GetPerformanceBootDataAsync` | POST | `/wxa/business/performance/boot` | `access_token`(Query) | **本域唯一非 `/datacube/` 路由**；体为 `WxaPerformanceBootRequest` |

## 路由前缀族：`/datacube/` 连写族 + 一支 `/wxa/` 例外

| 前缀族 | 支数 | 说明 |
|---|---|---|
| `/datacube/getweanalysisappid…` | 10 | 同一长前缀 `getweanalysisappid` **全连写、无分隔符**，只换尾部周期词与指标词 |
| `/wxa/business/performance/boot` | 1 | 与其余 10 支**不同前缀族**；官方清单里它属数据分析域但路由在 `/wxa/business/` 下 |

- **连写陷阱（本域最高频抄写错误）**：`getweanalysisappiddailyvisittrend` 是
  `get` + `weanalysis` + `appid` + `daily` + `visittrend` 的连写，
  **没有任何下划线**。写成 `getweanalysisappid_daily_visit_trend` 即 404。
- **词尾不是统一规则**：趋势族是 `…visittrend`，留存族是 `…retaininfo`（不是 `retain`），
  概况族是 `…summarytrend`（不是 `summary`），页面族是 `…visitpage`，画像是 `…userportrait`，
  分布是 `…visitdistribution` ⇒ **逐字照抄，不要按构词法生成**。MP-X5 用逐字数组钉死。
- **不要**因为「数据类端点该在 `/datacube/`」就把第 11 支搬进 `/datacube/`；
  也**不要**因为前缀不一致就把它移出本域（守卫按现状锁定）。
- **与公众号线的路由重复是被禁止的**：本域与 OfficialAccount 的 `DataCube` 族若出现同一路由，
  会由 MP-X6（`MiniProgramScaffoldContractGuards`）判红——两条线各留自己的接口，不共用常量。

## 「官方查询端点其实是 POST」

本域 10 支查询语义端点全部是 **POST + JSON 请求体 `{"begin_date":…,"end_date":…}`**，
不是 GET + Query。加上第 11 支，**11 支无一 GET**。

- 因此**没有**任何一支走 `[Query]` 业务参数（只有令牌 Query）。
- 跨域封装**不得**按「查询 ⇒ GET」推断；与 `Auth` 域（查询类走 GET）形态相反。

## 日期跨度与上界：三种口径并存

本域**不使用**统一日期类型，10 支共用 [WxaDateRangeRequest](../../../Mud.Wechat.MiniProgram.DataModels/DataAnalysis/WxaDateRangeRequest.cs)
（`BeginDate`→`begin_date` `string?`、`EndDate`→`end_date` `string?`，格式 `yyyymmdd`），
但官方对跨度的约束按族不同：

| 族 | 跨度约束（接口 XML 原文） | 上界 |
|---|---|---|
| 趋势（1–3）/ 留存（4–6）/ 分布（8）/ 页面（9）/ 概况（10） | **限定查询 1 天**（`begin_date` 与 `end_date` 相同；weekly / monthly 为对应周期） | 最大值为**昨日** |
| 用户画像（7） | **本域唯一非 1 天**：相差天数限定为 **0 / 6 / 29**（对应最近 1 / 7 / 30 天） | 最大值为**昨日** |
| 性能（11） | 不用日期区间，用 `time` | 见下节 |

- **SDK 不做任何本地日期校验**（与公众号线 `DataCube` 同款纪律）：越界、跨度不合法、
  日期格式错一律由应答 `errcode` 表达。**不得**在本域封装里加 `begin_date == end_date` 之类断言。
- `WxaDateRangeRequest.BeginDate` / `EndDate` 是 `string?`（不是 `DateOnly`/`DateTime`）——
  传 `DateOnly` 会破坏 MP-X5 的逐字键名口径；格式化责任在调用方。

## 应答信封不成一形：逐端点各自 DTO，**没有统一信封抽象**

这是逐页核验出的**事实**而非设计选择。四种形态（字段名逐字取自
[DataModels/DataAnalysis/](../../../Mud.Wechat.MiniProgram.DataModels/DataAnalysis/)）：

| 形态 | 适用端点 | 应答类型与顶层键 |
|---|---|---|
| A：`{list:[…]}`，**无顶层 `ref_date`** | 趋势 1–3 | `WxaVisitTrendResponse.List` → `List<WxaVisitTrendItem>?`；`ref_date` 在**条目内**（`WxaVisitTrendItem.RefDate`） |
| A：`{ref_date, list:[…]}` | 分布 8、页面 9 | `WxaVisitDistributionResponse` / `WxaVisitPageResponse`：`RefDate`→`ref_date` + `List` |
| A′：`{list:[…]}`，`ref_date` 在条目内 | 概况 10 | `WxaDailySummaryResponse.List` → `WxaDailySummaryItem { ref_date, visit_total, share_pv, share_uv }` |
| B：`{ref_date, visit_uv_new:[…], visit_uv:[…]}` | 留存 4–6 | `WxaRetainResponse`：两数组均为 `List<WxaKeyValueItem>?`（`key`/`value` 皆 `long?`） |
| C：`{ref_date, visit_uv_new:{…}, visit_uv:{…}}` | 画像 7 | `WxaUserPortraitResponse`：两属性为 **`WxaPortrait`（对象，不是数组）**，内含 `province`/`city`/`genders`/`platforms`/`devices`/`ages` 六维 `List<WxaPortraitItem>?` |
| D：`{data:[{key,value}]}` | 性能 11 | `WxaPerformanceBootResponse.Data` → `List<WxaPerformanceDataItem>?`（`Key`→`key` `string?`、`Value`→`value` `double?`） |

- **B 与 C 同名键、不同结构**（`visit_uv_new` / `visit_uv` 在留存是数组、在画像是对象）⇒
  判空与遍历逻辑**不可跨两支复用**；写通用取值函数即出错。
- **趋势族的 `ref_date` 只在条目里**，顶层没有；概况族同理。误当作顶层读会拿到 `null`。
- 全部应答继承 `WxaResponse` ⇒ 外层判错用 `ErrorCode`/`ErrorMessage`（`IsSuccess`）；
  本域**没有**逐条结果码。
- 官方文档自身不一致处**照录**：画像六维的字段表统一列 `id`/`name`/`value`，
  但示例里 `devices` 元素**只有** `name`/`value` ⇒ `WxaPortraitItem.Id` 建模为 `long?` 可空，
  本线**不以示例覆盖字段表、也不本地补齐**。

## 留存 `key`、分布 `index`、性能 `time`/`module`：本域三处取值集合

1. **留存 `key`**（端点 4–6）：`0` 表示当天、`1` 表示 1 天后……官方取值集合为
   **`0,1,2,3,4,5,6,7,14,30`**；`key = 0` 时为当日新增 / 活跃用户数，`key > 0` 时为留存用户数。
   **默认只返回 `key = 0`**，其余需按官方口径取值 ⇒ 「数组长度就是留存天数」是错误推论。
2. **分布 `list[].index`**（端点 8）：三个枚举值 `access_source_session_cnt`（访问来源分布）/
   `access_staytime_info`（访问时长分布）/ `access_depth_info`（访问深度分布）；
   内层 `ItemList`→`item_list`（`List<WxaKeyValueItem>?`），其 `key` 是**各 `index` 下的场景 id**
   ⇒ 同一 `key` 数值在不同 `index` 下语义不同，**不得**跨 `index` 归并。
   `WxaVisitDistributionItem.ItemList` 的 `item_list` 键名由 MP-X5 锁定。
3. **性能（端点 11）**：`WxaPerformanceBootRequest { Time→"time" string?, Module→"module" long?,
   ExtInfo→"extinfo" string? }`。`time` 格式 **`yyyymmddhh`、精确到小时**（不是 `yyyymmdd`），
   `module` = `1` 启动性能 / `2` 运行性能。

## 权限集与第三方代调用面

- 接口 XML 记本域**支持第三方平台代调用**，**权限集 18**，可用 `authorizer_access_token`；
  **但本线只覆盖自建形态**。
- 「本线不建代调用链」由 [MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)
  的 **P3-R2** 机械锁定（本线不得出现 component / authorizer 令牌链）。
  该守卫的注释行剥离逻辑曾把本文件的边界注释误判为违规 ⇒ 改本文件顶部注释时留意。
- **适用范围：小程序 ✔ / 小游戏 ✔**（接口 XML 原文），本域无小游戏专用分支端点。
- 本域**没有**二进制/文本手工通道（本线手工通道共 6 支，全在 `QrCodeLink`/`Feedback`/`Express`/`Kf`），
  因此**无** `IDisposable` 应答。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

// 两段式装配：MP 令牌底座先行，再装本域模块
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddDataAnalysisApi());

// 注入 IWxaDataAnalysisService（示例用字段名 dataAnalysis）
// 1) 日趋势：begin_date 与 end_date 同一天（yyyymmdd），上界为昨日。SDK 不校验跨度。
var trend = await dataAnalysis.GetDailyVisitTrendAsync(
    new WxaDateRangeRequest { BeginDate = "20261015", EndDate = "20261015" }, ct);
WxaException.ThrowIfFailed(trend, "/datacube/getweanalysisappiddailyvisittrend");
// 注意：ref_date 在条目里，顶层没有
List<WxaVisitTrendItem> items = trend.List ?? new List<WxaVisitTrendItem>();
foreach (WxaVisitTrendItem i in items)
{
    Console.WriteLine($"{i.RefDate} pv={i.VisitPv} uv={i.VisitUv} stay={i.StayTimeSession}");
}

// 2) 留存：默认只回 key=0；数组元素是 {key,value}
var retain = await dataAnalysis.GetDailyRetainAsync(
    new WxaDateRangeRequest { BeginDate = "20261015", EndDate = "20261015" }, ct);
WxaException.ThrowIfFailed(retain, "/datacube/getweanalysisappiddailyretaininfo");
long? sameDayNew = retain.VisitUvNew?.FirstOrDefault(x => x.Key == 0)?.Value;

// 3) 画像：跨度差必须是 0 / 6 / 29；visit_uv_new 是对象而非数组
var portrait = await dataAnalysis.GetUserPortraitAsync(
    new WxaDateRangeRequest { BeginDate = "20261009", EndDate = "20261015" }, ct);
WxaException.ThrowIfFailed(portrait, "/datacube/getweanalysisappiduserportrait");
var top = portrait.VisitUv?.Province is { Count: > 0 } p ? p[0].Name : null;
// devices 元素的 Id 可能为 null（官方示例即缺 id），不要当作数据缺陷去本地补齐

// 4) 访问页面：只有 top 200，按 page_visit_pv 排序；无分页游标
var pages = await dataAnalysis.GetVisitPageAsync(
    new WxaDateRangeRequest { BeginDate = "20261015", EndDate = "20261015" }, ct);

// 5) 性能：time 是 yyyymmddhh（小时精度），module 1=启动 / 2=运行
var perf = await dataAnalysis.GetPerformanceBootDataAsync(
    new WxaPerformanceBootRequest { Time = "2026101510", Module = 1 }, ct);
WxaException.ThrowIfFailed(perf, "/wxa/business/performance/boot");
List<WxaPerformanceDataItem> metrics = perf.Data ?? new List<WxaPerformanceDataItem>();   // { key, value }，value 为 double?
```

本域 11 支都返回普通 JSON 信封，无流式应答、无需 `using` / `Dispose`。
应答只有聚合指标，**不含**任何用户级标识或凭据；`access_token` 由 SDK 注入 Query，
调用方不得自行拼接、也不得写入日志或异常消息。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **本域无独立守卫文件** | 断言落在 [MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)：MP-X5 的 `RoutesOf(…, nameof(IWxaDataAnalysisService))` 11 条逐字数组 + 合计计数注释（`DataAnalysis 11`）、MP-X5 字段名（`begin_date` / `item_list` / `page_visit_pv`）、MP-X3 令牌名；另由 P3-R2 锁代调用面 | 若要收拢需新建专属守卫并同批更新本文「事实来源」 |
| 官方逐端点调用频率 / 限流数字 | 接口 XML 未登记 ⇒ **未逐项核验** | 不得写入本文 |
| 错误码全集 | 接口 XML 只出现 `-1`（system error）与 `40001`（令牌无效，走自愈） | 需要完整判错表时先逐页核验 |
| 留存 `key` 在 weekly / monthly 两支的集合是否与 daily 完全一致 | 接口 XML 只写「`key` 语义同上」 | **未逐项核验**，勿按 daily 集合外推 |
| 分布 `item_list[].key` 的场景 id 取值表 | 本仓只承载为 `long?`，无枚举面 | **未逐项核验** |
| 性能 `data[].key` 的指标名全集 | 本仓只承载为 `string?` | **未逐项核验**，勿硬编码指标名 |
| `stay_time_uv` / `stay_time_session` / `visit_depth` / `page_staytime_pv` 的单位 | DTO 以 `double?` 承载，单位口径未登记 | **未逐项核验** |
| 画像族「最近 1 / 7 / 30 天」与相差 0 / 6 / 29 的换算是否含首末日 | 只照录接口 XML 原文 | **未逐项核验** |
| 权限集 18 的官方名称与开通流程 | 只登记数字 | **未逐项核验** |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、官方 URL、跨度/`key`/`index`/`top 200` 约束、信封不成一的取证 | [IWxaDataAnalysisService.cs](./IWxaDataAnalysisService.cs) 的 XML 注释 |
| 路由集合（11 条逐字）与全线合计计数 398 | `MiniProgramContractGuards` 的 MP-X5；基线在 `Tests/ContractBaseline.cs` |
| 令牌绑定与复用 | MP-X3；MP-X2（`MiniProgramScaffoldContractGuards`） |
| 与公众号线路由不得重复 | MP-X6（`MiniProgramScaffoldContractGuards`） |
| 本线不建 component/authorizer 令牌链 | P3-R2（`MiniProgramP3RulingsContractGuards`） |
| 请求 / 应答字段名与类型（17 个文件） | [../../../Mud.Wechat.MiniProgram.DataModels/DataAnalysis/](../../../Mud.Wechat.MiniProgram.DataModels/DataAnalysis/)（`SerializerClassName = "DataAnalysis"`） |
| AOT 序列化上下文（`internal`，示例中不得引用） | `../../../Mud.Wechat.MiniProgram.DataModels/Generated/DataAnalysisJsonContext.g.cs` |
| 本线端点总表与装配入口 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方 URL 与跨度约束，**同批**更新 MP-X5 的本域 11 条路由数组
   与合计计数（全线 398 = 392 特性路由 + 6 手工通道，基线 `Tests/ContractBaseline.cs`）。
2. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `DataAnalysis`）→
   跑 `scripts/AddHttpJsonSerializable.ps1` 与 `scripts/GenerateJsonContext.ps1` →
   确认并入 `DataAnalysisJsonContext`；JSON 键**照官方原文**（`begin_date`/`item_list`/`page_visit_pv`
   等由 MP-X5 锁定，驼峰化即红）。
3. **改注册面** → `MiniProgramModule.DataAnalysis` + `AddDataAnalysisApi()`
   （[MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）与本域注册委托同步；
   新增接口须仍落 `RegistryGroupName = "DataAnalysis"`。
4. **更新本文**：速览表、四种信封形态对照、三处取值集合与缺口。
5. **不要**为「统一」而引入跨端点的通用信封类型；**不要**把画像的 `visit_uv_new` 当数组处理；
   **不要**把第 11 支的路由改成 `/datacube/…`；**不要**给本域加 component/authorizer 令牌参数（P3-R2）；
   **不要**加本地日期跨度校验。
6. **本线无回调工程**：消息与事件接收属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），
   形态由 [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)
   （MP-X4）锁定，**不要**顺手补一个回调工程。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
