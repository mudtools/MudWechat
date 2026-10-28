# DataCube 域 · 数据统计（wedata + 卡券统计 + 流量主广告分析）

微信公众号 / 服务号「数据统计」域：**28 个方法 / 26 条去重路由**
（21 支用户·图文·消息·接口统计 + 4 支卡券统计 = **25 条 POST `/datacube/*`**，
另有流量主广告分析 **3 个方法共用 1 条 GET `/publisher/stat`**，按 action 分方法）。
接口 [IMpDataCubeService.cs](./IMpDataCubeService.cs)，注册组 `DataCube`，
模块枚举 `MpModule.DataCube`（`AddDataCubeApi()` ⇒ 生成入口 `AddDataCubeWebApiHttpClient()`）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpDataCubeContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpDataCubeContractGuards.cs)
> （`DC1`~`DC7`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/DataCube/](../../../Mud.Wechat.OfficialAccount/DataCube/)）。
> **计数以守卫为准**：`DC1` 钉 `DataCubeRoutes` = 21（用户 2 + 图文 10（旧 6 + 新 4）+ 消息 7 + 接口 2）
> 且去重后仍为 21，`CardDataCubeRoutes` = 4；`DC7` 钉流量主 3 方法。
> 接口 XML 摘要里的「24 端点」、`MpModule.DataCube` 与包 README 里的「21 端点」、`QT1` 注释里的「25 端点」
> 是**同一域的四套过期口径**，均不作本文依据（详见文末缺口）。

## 端点速览（按子族分组，覆盖全部 28 个方法）

### 用户数据（2，POST `/datacube/*`）

| # | 方法 | 动词 | 路由 | 应答元素 |
|---|---|---|---|---|
| 1 | `GetUserSummaryAsync` | POST | `/datacube/getusersummary` | `MpUserSummaryItem`（含 `user_source` 渠道维度） |
| 2 | `GetUserCumulateAsync` | POST | `/datacube/getusercumulate` | `MpUserCumulateItem`（仅两字段 ⇒ **不与 1 共用**，`DC3`） |

### 图文统计·旧（6，官方均已声明「停止维护」）

| # | 方法 | 路由 |
|---|---|---|
| 3 | `GetArticleSummaryAsync` | `/datacube/getarticlesummary` |
| 4 | `GetArticleTotalAsync` | `/datacube/getarticletotal` |
| 5 | `GetUserReadAsync` | `/datacube/getuserread` |
| 6 | `GetUserReadHourAsync` | `/datacube/getuserreadhour` |
| 7 | `GetUserShareAsync` | `/datacube/getusershare` |
| 8 | `GetUserShareHourAsync` | `/datacube/getusersharehour` |

官方推荐替换面：`getarticleread` / `getarticleshare` / `getbizsummary` / `getarticletotaldetail`。
SDK **照常建模**（官方仍在响应）并在各方法 remarks 标注替换指向。

### 图文统计·新（4）

| # | 方法 | 路由 | 跨度 |
|---|---|---|---|
| 9 | `GetArticleReadAsync` | `/datacube/getarticleread` | 仅支持查询 **1 天** |
| 10 | `GetArticleShareAsync` | `/datacube/getarticleshare` | 仅 **1 天** |
| 11 | `GetBizSummaryAsync` | `/datacube/getbizsummary` | 最长 **30 天** |
| 12 | `GetArticleTotalDetailAsync` | `/datacube/getarticletotaldetail` | 仅 **1 天** |

### 消息消息与接口（9）

| # | 方法 | 路由 | 备注 |
|---|---|---|---|
| 13 | `GetUpstreamMsgAsync` | `/datacube/getupstreammsg` | 与 14/15/16 **共用** `MpUpstreamMsgItem` |
| 14 | `GetUpstreamMsgWeekAsync` | `/datacube/getupstreammsgweek` | 周/月/分时官方要求「必须为同一天」 |
| 15 | `GetUpstreamMsgMonthAsync` | `/datacube/getupstreammsgmonth` | 同上 |
| 16 | `GetUpstreamMsgHourAsync` | `/datacube/getupstreammsghour` | `ref_hour` 官方**仅示例有、字段表未列** ⇒ 不建模（照录缺口） |
| 17 | `GetUpstreamMsgDistAsync` | `/datacube/getupstreammsgdist` | 分布族 3 支共用 `MpUpstreamMsgDistItem` |
| 18 | `GetUpstreamMsgDistWeekAsync` | `/datacube/getupstreammsgdistweek` | 同上 |
| 19 | `GetUpstreamMsgDistMonthAsync` | `/datacube/getupstreammsgdistmonth` | 同上 |
| 20 | `GetInterfaceSummaryAsync` | `/datacube/getinterfacesummary` | 与 21 共用 `MpInterfaceSummaryItem`（超集） |
| 21 | `GetInterfaceSummaryHourAsync` | `/datacube/getinterfacesummaryhour` | 官方请求体表写「无」但示例带两日期 ⇒ 按同构建模 |

### 卡券统计（4，POST `/datacube/getcard*`）

| # | 方法 | 路由 | 请求体 |
|---|---|---|---|
| 22 | `GetCardBizUinInfoAsync` | `/datacube/getcardbizuininfo` | `MpCardBizUinInfoRequest`（日期 + `cond_source`） |
| 23 | `GetCardCardInfoAsync` | `/datacube/getcardcardinfo` | `MpCardCardInfoRequest`（再追加 `card_id`，选填） |
| 24 | `GetCardMemberCardInfoAsync` | `/datacube/getcardmembercardinfo` | **复用** `MpCardBizUinInfoRequest`（与帐号级同形） |
| 25 | `GetCardMemberCardDetailAsync` | `/datacube/getcardmembercarddetail` | `MpCardMemberCardDetailRequest`（`card_id` 官方**必填**） |

四支日期区间均 **≤ 1 天**。`cond_source`：0 全部 / 1 普通券（公众号）来源 / 2 小程序来源。

### 流量主广告数据分析（3 方法 / 1 条路由，GET）

| # | 方法 | 动词 | 路由 | 固定 `action`（Query 默认值） | 跨度 |
|---|---|---|---|---|---|
| 26 | `GetPublisherAdSlotGeneralAsync` | GET | `/publisher/stat` | `publisher_adpos_general` | **90 天**，`page_size` 上限 90 |
| 27 | `GetPublisherCpsGeneralAsync` | GET | `/publisher/stat` | `publisher_cps_general` | **60 天**；**不需要广告位参数** |
| 28 | `GetPublisherSettlementAsync` | GET | `/publisher/stat` | `publisher_settlement` | **无跨度限制** |

**权限 / 令牌**：全部 25 支 `/datacube/*` 与 3 支流量主端点均 Query 注入 `access_token`（MUD005）。
适用范围「公众号 / 服务号 —— **仅认证**」。第三方代调用留档只有两处：
用户数据族 **权限集 id = 2**（`getusersummary` remarks）、接口族 **权限集 id = 3**（`getinterfacesummary` remarks）；
其余各族官方代调用面**未逐项核验**（见缺口）。

## 请求体只有一个：`MpDateRangeRequest`，但跨度措辞每页不同（勿互相套用）

21 支 `/datacube/*` 的 wedata 端点**共用** [MpDateRangeRequest](../../../Mud.Wechat.OfficialAccount/DataCube/MpDateRangeRequest.cs)
（`begin_date` / `end_date`，格式 `yyyy-MM-dd`）——`DC2` 反射断言「每支都只有这一个请求体参数」。

**各端点上限照官方各页原文，SDK 不做本地跨度校验，越界由官方 `61501` 表达**：

| 族 | 官方跨度表述 |
|---|---|
| 用户 2 支 | 「最大跨度 7 天」 |
| `getupstreammsg` | 「与 `end_date` 差值小于 7 天」 |
| 消息周 / 月 / 分时 | 「结束日期（**必须为同一天**）」；数据打在周期首日 |
| 消息分布 3 支 | 「跨度不超过 15 天」 |
| `getinterfacesummary` | 「最大时间跨度 30 天」 |
| `getinterfacesummaryhour` | 「最大时间跨度为 1 天」（注意事项章节） |
| 新图文 `getarticleread` / `getarticleshare` / `getarticletotaldetail` | 「日期范围仅支持查询 1 天」 |
| `getbizsummary` | 「最长支持查询 30 天」 |
| 旧图文 6 支 | 参数表**仅**「结束日期（最大值为昨日）」，**无天数表述** |
| 卡券统计 4 支 | 区间 ≤ 1 天 |
| 流量主 adpos / cps / settlement | 90 天 / 60 天 / 无限制 |

⇒ 把「7 天」当成全域规则、或把新图文的「1 天」套到旧图文，都是错的。

**错误码三件套**（`DC6` 锁定 `MpErrorCodes` 常量）：
`DataCubeDateFormatError = 61500`（date format error）、
`DataCubeDateRangeError = 61501`（date range error，跨度超限）、
`DataCubeDataNotReady = 61503`（数据尚未生成）。跨页复用 ⇒ 不要按端点各建一套。

## 数据起始、就绪时间与统计窗口

| 事实（remarks 照录） | 影响 |
|---|---|
| 多端点数据始于 **2014-12-01** | 更早区间必然空 |
| 新图文族始于 **2025-11-01** | 与旧图文族起点不同，迁移期两套数据不可比 |
| 流量主数据始于 **2016-01-01** | 同上 |
| 「建议每日 **8 点后**查询前一天数据」 | 早于 8 点易撞 `61503` |
| `getarticletotal`「最多统计发表日后 **7 天**数据」 | 之后该篇不再增长 |
| `getarticletotaldetail`「每篇仅统计发表日期起 **30 天**内」 | 与上一条是**两个不同窗口**，勿混 |
| 阅读量总和 **≥ 3** 才纳入统计 | 小样本静默缺席，不是接口故障 |
| 新图文 4 支响应顶层 `is_delay`（`false` = 最新） | 数据延迟用 `is_delay` 表达，需读该位而非猜时间 |

## 字段名反直觉处（`DC3` 逐条钉死，禁止「顺手统一」）

| 官方键 | 陷阱 |
|---|---|
| `MpUpstreamMsgItem` 恰 `{ref_date, msg_type, msg_user, msg_count}` | 日/周/月/时**四端点完全同构** ⇒ 共用一个元素类型；`ref_hour` 不建模 |
| `MpUserSummaryItem.user_source` | 取值集合 `0/1/17/30/57/100/161/149/200/201`（渠道维度）；`MpUserCumulateItem` 仅 `ref_date` + `cumulate_user` ⇒ **两支不共用** |
| `feed_share_from_session_cnt` / `_feed_cnt` / `_other_cnt` | 反直觉 **`_cnt`** 后缀；同族其余为 `_count`（对照 `int_page_from_session_read_count`） |
| `details` vs `detail_list` | `getarticletotal` 的嵌套数组叫 `details`、`getarticletotaldetail` 的叫 `detail_list` ⇒ 照各自页面，不合并 |
| `zaikan_user` | 「爱心赞人数」官方拼写就是 `zaikan`，**勿修正** |
| `count_interval` | `0`='0' / `1`='1-5' / `2`='6-10' / `3`='10次以上'（分布族） |
| `praise_money` | **单位为分** |
| `position` | `1`~`5` 对应 `0~20%` 至 `80%~100%` 跳出区间 |
| `total_time_cost` | 总耗时，除以 `callback_count` 才得平均耗时（接口族） |
| `ref_hour` | 仅**分时**端点返回（`getuserreadhour` / `getusersharehour` / `getinterfacesummaryhour`）⇒ 日/时分共用同一 DTO，该字段可空 |

## 流量主三方法：一 action 一端点，不是运行时多态

`DC7` 锁定的裁决：官方用 `?action=` 区分三种数据，本域按 DataCube 风格
**一个 action 一个方法**，action 以**带默认值的 Query 参数**表达 ⇒ 调用方通常不必显式传；
把它改成「一个方法 + 运行时传 action」会让守卫的路由表与返回类型断言失效。

Query 面：`page`（默认 1）、`page_size`（默认 90，**每页最大 90**）、`start_date`、`end_date`，
`ad_slot` 仅 adpos 方法有（选填，不传即返回全部类型广告位**除返佣商品**的数据）。
应答信封是**双层**：外层 [MpResponse](../../../Mud.Wechat.OfficialAccount/MpResponse.cs) 的 `errcode`/`errmsg`
（走统一判错），内层 `base_resp` = [`MpPublisherBaseResp`](../../../Mud.Wechat.OfficialAccount/DataCube/Publisher/MpPublisherBaseResp.cs)
——其官方键为 **`ret`（`int`）与 `err_msg`（`string?`）**，不是 `errcode`/`errmsg` ⇒
只看外层判错会漏掉业务失败。官方错误码：`45009`（请求过于频繁）/ `45010`（无效的接口名）/
`1701`（参数错误）/ `2009`（无效的流量主）。
`settlement` 方向：只要与起止时间有重合，对应结算区间数据都会返回（跨月即返回多个上/下半月区间）；
其顶层 `body` / `revenue_all` / `penalty_all` / `settled_revenue_all`（`long`）+ `settlement_list` + `total_num`。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddDataCubeApi());

// 注入 IMpDataCubeService（注册组 DataCube）
// 1) 用户增减：跨度 ≤ 7 天，日期格式 yyyy-MM-dd
var users = await dataCube.GetUserSummaryAsync(new MpDateRangeRequest
{
    BeginDate = "2026-10-01",
    EndDate = "2026-10-07",
}, ct);
MpException.ThrowIfFailed(users, "/datacube/getusersummary");
foreach (MpUserSummaryItem item in users.List ?? new List<MpUserSummaryItem>())
{
    // user_source：0/1/17/30/57/100/161/149/200/201（渠道维度，cumulate 族没有这一维）
    Console.WriteLine($"{item.RefDate} source={item.UserSource} +{item.NewUser} -{item.CancelUser}");
}

// 2) 周/月消息：官方要求 begin 与 end 为同一天，且数据打在周期首日
await dataCube.GetUpstreamMsgWeekAsync(new MpDateRangeRequest
{
    BeginDate = "2026-10-05",
    EndDate = "2026-10-05",
}, ct);

// 3) 卡券统计：请求体在日期区间上追加来源过滤（cond_source 0/1/2），区间 ≤ 1 天
var cardStat = await dataCube.GetCardBizUinInfoAsync(new MpCardBizUinInfoRequest
{
    BeginDate = "2026-10-07",
    EndDate = "2026-10-07",
    CardSource = 0,
}, ct);

// 4) 流量主：action 由方法默认值固定，调用方只给分页与日期
var adpos = await dataCube.GetPublisherAdSlotGeneralAsync(
    page: 1,
    pageSize: 90,
    startDate: "2026-08-01",
    endDate: "2026-10-07",
    cancellationToken: ct);
// 双层判错：信封 errcode + 业务 base_resp（官方键为 ret / err_msg）
MpException.ThrowIfFailed(adpos, "/publisher/stat");
if (adpos.BaseResp is { } br && br.Ret != 0)
{
    throw new MpException(br.Ret, br.ErrMsg ?? "", "/publisher/stat");
}
```

`openid` / `unionid` 不出现在本域请求中；`access_token` 只作参数名，**不得**进日志与异常消息。
查询区间建议避开 8 点前，撞到 `61503`（数据未生成）属预期行为而非故障，应退避重试而不是判失败。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **端点计数四套口径并存** | 接口 XML 摘要「24 端点」；`MpModule.DataCube` 与包 README「21 端点」；`QT1` 注释「25 端点」；守卫实测 `DC1` 21 + 4 = 25 条 POST、`DC7` 3 方法 GET ⇒ 28 方法 / 26 去重路由 | 以 `DC1` + `DC7` 为准；remarks 属待回填文本。**新增端点须同批改四处口径**（接口摘要、`MpModule` 注释、`QT1` 注释、`DC1`/`DC7` 路由表） |
| 官方文档树前缀 | 数据统计文档在 **`/doc/service/api/wedata/`**（subscription 订阅号域 404，2026-10-07 21 页全部在服务号域命中）；流量主文档在 `/doc/service/guide/product/analysis_data/ad/Ad_Analysis.html` | 文档树再迁移时逐页复核链接 |
| 第三方代调用面 | 仅用户族（权限集 2）与接口族（权限集 3）留档；图文 / 消息分布 / 卡券统计 / 流量主**未逐项核验** | 核验后补 remarks；核验前不要写权限集号 |
| `getupstreammsghour` 的 `ref_hour` | 官方字段表未列、仅示例出现 ⇒ **照录不建模** | 若官方补齐则同批改 `DC3` 共用裁决 |
| 旧图文 6 支的下线与替换节奏 | 官方声明「已停止维护」但仍在响应；**下线时间未核验** | 保持建模；替换指向写在方法 remarks |
| `MpPublisherSettlementResponse` 的结算主体字段全集 | 仓内只留 `body`/`revenue_all`/`penalty_all`/`settled_revenue_all`/`settlement_list`/`total_num` 的名称线索，逐项含义**未核验** | 核验后补 DTO remarks；单位（元/分）在核验前不写结论 |
| `DataModels/DataCube` 类型数 | `DC4` 钉 `expectedCount = 55`（请求 1 + 用户 4 + 旧图文 8 + 新图文 12 + 消息 4 + 接口 2 + 场景/跳出辅助 3 + 卡券统计 11 + 流量主 10） | 漂移必须先核对官方文档再同批改该基线，勿只改数字 |
| 包 README 的域总数（27 目录 / 29 接口 / 188 条去重路由 / RC2 194 条） | **已过期**：本包 `Interfaces/` 实测为 **30 个域目录 / 35 个公开接口**，`ContractBaseline.OfficialAccount.MainInterfaceRoutes = 259`、`TotalRoutes = 266`、`OfficialRoutes = 196` | 域 README 一律以接口文件与守卫实测为准，不引用包 README 的总数 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文、跨度措辞、停止维护声明、数据起始与就绪 | [IMpDataCubeService.cs](./IMpDataCubeService.cs) 的 XML 注释 |
| 25 条 POST 路由 / 返回类型对应关系 / 计数 21 + 4 | `DC1` = `DataCubeEndpoints_ShouldMatchOfficialRoutes` |
| 「21 支共用 `MpDateRangeRequest`」与 `begin_date`/`end_date` 键名 | `DC2` = `DateRangeRequest_ShouldBeSharedAcrossAllEndpoints` |
| 分族共用裁决与全部反直觉键名 | `DC3` = `ResponseDtos_ShouldLockFamilySharing` |
| DTO 登记 + `expectedCount = 55` + `SerializerClassName = DataCube` | `DC4` = `DataCubeDataModels_ShouldBeRegisteredInJsonContext` |
| `MpModule.DataCube` + `AddDataCubeApi()` + 白名单 | `DC5` = `DataCubeModule_ShouldBeRegisteredAndWhitelisted` |
| `61500` / `61501` / `61503` | `DC6` = `DataCubeErrorCodes_ShouldMatchVerifiedOfficialValues` |
| 流量主 3 方法：GET + 固定 action 默认值 | `DC7` = `PublisherEndpoints_ShouldBeGetWithFixedAction` |
| 日期跨度措辞全集与「不做本地跨度校验」 | [MpDateRangeRequest.cs](../../../Mud.Wechat.OfficialAccount/DataCube/MpDateRangeRequest.cs) remarks |
| 卡券统计请求三型与继承关系 | [DataCube/Card/](../../../Mud.Wechat.OfficialAccount/DataCube/Card/)（11 个文件） |
| 流量主 DTO（base_resp / adpos / cps / settlement） | [DataCube/Publisher/](../../../Mud.Wechat.OfficialAccount/DataCube/Publisher/)（10 个文件） |
| 新图文族 DTO | [DataCube/Article/](../../../Mud.Wechat.OfficialAccount/DataCube/Article/)；消息族 [DataCube/UpstreamMsg/](../../../Mud.Wechat.OfficialAccount/DataCube/UpstreamMsg/) |
| 生成入口与模块枚举、上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与**该页自己的**跨度措辞，**同批**更新
   `DC1` 的 `DataCubeRoutes`（21）/ `CardDataCubeRoutes`（4）或 `DC7` 的 `PublisherRoutes`（3），
   以及 `MpRouteCountGuard`（`RC1`/`RC2`）与 `Tests/ContractBaseline.cs` 基线；
   `/publisher/stat` 三方法**只算 1 条去重路由**，加 action 不等于加路由。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `DataCube`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 同批更新 `DC4` 的 `expectedCount`（55）。
3. 确认 `Generated/DataCubeJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 回填过期口径：接口 XML 摘要、`MpModule.DataCube` remarks、`QT1` 条目注释、包 README 与本文件
   的端点数必须统一到守卫实测值，否则下一个人仍会按 21/24/25 之一写代码。
5. **不要**跨端点套用跨度上限；**不要**加本地跨度校验（越界交官方 `61501`）；
   **不要**把 `details` 与 `detail_list` 合并、**不要**把 `_cnt` 改成 `_count`、
   **不要**把 `zaikan_user` 「修正」拼写、**不要**把流量主三方法折成一个多态方法、
   **不要**只判 `errcode` 而漏读 `base_resp`。
6. 最后跑 `scripts/verify-build.ps1`，全绿才算完成。
