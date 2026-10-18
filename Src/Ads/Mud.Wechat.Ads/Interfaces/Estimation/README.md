# Estimation 域 · 广告预估

腾讯广告 Marketing API v3.0「广告预估」域：**1 支端点**（`estimation/get`，查询型 POST）。
接口 [IWechatAdsEstimationService.cs](./IWechatAdsEstimationService.cs)，注册组 `Estimation`，
模块枚举 `AdsModule.Estimation`（`AddEstimationApi()`）。逐页核验 2026-10-17。

> 事实来源限定：逐参数原文以接口文件 XML 注释与 `AdsEstimationGetRequest` remarks 为准；
> 路由 / 计数 / 子树复用关系以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 方法 | 动词 | 路由 | 权限 | `user_token` | `X-Request-Id` |
|---|---|---|---|---|---|
| `GetEstimationAsync` | **POST** | `/v3.0/estimation/get` | `ads_management` | 无 | 无 |

- **权限 `ads_management`** —— 与 Funds / Qualifications 两域的 `account_management` **不同权限族**，
  这正是本域**独立成域**、不并入既有域的理由。
- **本页没有「特定请求参数」表，也没有请求头参数表** ⇒ 不带 `user_token`、不带 `X-Request-Id`
  （守卫 `ADS-B5` 的反证面；`EstimationEndpoints_ShouldMatchOfficialRouteTable` 逐条断言）。

## 「查询型 POST」：动词与语义相反，照官方不改

官方动词是 `POST`、`Content-Type: application/json`，但语义是**只读预估**、不产生任何投放资源。
本仓**不因「看起来该是 GET」而改写**：路由按原文 `/v3.0/estimation/get`、属性按 `[Post]`，
守卫显式断言 `attr.Should().BeOfType<PostAttribute>()` —— 这是本线「查询类官方即 POST」口径的
又一处逐页事实（与 Reports 域的 3 支 GET 形成对照，见 [../Reports/README.md](../Reports/README.md)）。

## 请求体：顶层 17 字段，只有 `account_id` 必填

`account_id` 官方必填（原文「有操作权限的帐号 id，**不支持代理商 id**」，口径同 Qualifications 域）。
其余 16 支全部可空 ⇒ 序列化 `WhenWritingNull` 省略未填分支，由官方按「预估默认形态」处理。

**条件必填不本地拦截**：子树里的 `*`（`marketing_target_detail` 内的 `marketing_target_detail_id` /
`marketing_target_sub_detail_id`、targeting 内的 `location_types`、scene_spec 内的
`excluded_dimension` 等）是「**父结构在场时**必填」，本地拦截会挡掉「整个父结构不传」这一合法形态。

### 四支子树复用 Adgroups 域类型（不许复制第二套）

2026-10-17 逐字段比对（字段名、类型、层级）与 `adgroups/add` 页**同构** ⇒ 直接复用：

| 本请求键 | 复用类型（`DataModels/Adgroups/`） | 守卫锚点 |
|---|---|---|
| `targeting` | `AdsAdgroupTargeting` | `EstimationEndpoints_...` 断言属性类型 |
| `scene_spec` | `AdsAdgroupSceneSpec` | 同上 |
| `marketing_carrier_detail` | `AdsMarketingCarrierDetail` | — |
| `marketing_target_detail` | `AdsMarketingTargetDetail` | — |

**复制一套同形 DTO 是本域最大的维护陷阱**：官方改字段名时只会改到其中一份，两份立即漂移，
且定向取值域（`regions` / `business_districts` 等 id）由 TargetingTags 域供给，跨域各建各的必然对不上。

### ADX 程序化投放的禁用面

官方对若干字段逐字标注「ADX 程序化投放不可填写提交」：

| 字段 | ADX 下的官方约束 |
|---|---|
| `automatic_site_enabled` / `targeting` / `scene_spec` | **不可填写提交** |
| `bid_mode` | 仅支持 `{ BID_MODE_CPC, BID_MODE_CPM }` |
| `bid_amount` | 默认填 `200`（单位分） |
| `time_series` | 默认填 **336 个 `1`** |

投放模式由**账号侧**决定，SDK **不预判**（与 `AdsAdgroupAddRequest` remarks 同口径）⇒ 同一请求体
在不同账号上可能一侧成功一侧被 `code` 拒绝，这不是 SDK 缺陷，别用本地校验去「修平」。

其它值得单独记住的字段约束：`adgroup_name`「同一帐号下不允许重复」、1–60 个等宽字符 / 最大 180 字节；
`site_set` 数组 1–32，官方限制「当前单站点，或 `SITE_SET_TENCENT_NEWS` + `SITE_SET_TENCENT_VIDEO` 的组合」；
`optimization_goal` 与定向内 `conversion_behavior_list` **同枚举集**（`OPTIMIZATIONGOAL_*` 大集合，逐值看官方页）。

## 应答：单结构体 16 字段，全是「区间 / 上限 / 建议」

`data` 是**单结构体**（不是 `list[]`，没有分页），字段名前缀即语义，逐字照录：

| 组 | 字段 |
|---|---|
| 覆盖与曝光**最大值** | `approximate_count`（日覆盖用户数最大值）、`impression`（日曝光量最大值）、`max_active_user_count` |
| 覆盖区间 | `min_users_daily` ~ `max_users_daily` |
| 曝光区间 | `min_exposure_daily` ~ `max_exposure_daily` |
| 出价区间 | `min_bid_amount` ~ `max_bid_amount`（单位**分**） |
| 建议出价 | `suggest_min_bid_amount` ~ `suggest_max_bid_amount`、`suggest_bid_content_ocpa`（仅 oCPC/oCPM 返回） |
| 定向建议 | `targeting_status`、`suggest_targeting[]` |
| 分阶段建议 | `stage_bid_info[]`：`stage_name` / `lower_bound` / `upper_bound` / `default_bid`（分）/ `ratio`（竞胜概率，`float`） |
| 能力位 | `is_real_exposure_supported`（当前请求是否满足真实曝光量预估要求） |

**三条使用红线**：
1. `approximate_count` / `impression` 是**日最大值**，不是均值也不是总量 —— 拿它当「预计消耗」做预算
   推导会把口径整体放大。
2. 所有金额单位是**分**（本线统一），`ratio` 是 `double` 承载的官方 `float`。
3. `targeting_status` 官方标 `enum` 但**本页枚举详情未展开** ⇒ 留档为 `string`，
   **不得**从 Reports / Adgroups 页借枚举集合。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddEstimationApi());

var resp = await estimation.GetEstimationAsync(new AdsEstimationGetRequest
{
    AccountId = 1234567890,
    MarketingGoal = "MARKETINGGOAL_ECOMMERCE_ORDER",   // 逐值照官方页，勿从邻域借
    SiteSet = ["SITE_SET_TENCENT_NEWS"],
    BidMode = "BID_MODE_OCPM",
    BidAmount = 2000,                                   // 单位：分
    Targeting = new AdsAdgroupTargeting                 // 复用 Adgroups 域类型
    {
        Regions = [110000, 310000],                     // List<long>：取值来自 targeting_tags/get 的 id（见该域 README）
    },
}, ct);

WechatAdsException.ThrowIfFailed(resp, "/v3.0/estimation/get");
var d = resp.Data!;
// 只按区间展示，不把上限当承诺
Console.WriteLine($"日覆盖 {d.MinUsersDaily}~{d.MaxUsersDaily}，建议出价 {d.SuggestMinBidAmount}~{d.SuggestMaxBidAmount} 分");
```

**判错只有一层**：普通信封，`code == 0` 即预估成功；本域没有批量/异步族的双层判定面。

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐字段原文（含 ADX 禁用面、条件必填 `*`） | `IWechatAdsEstimationService.cs` + `AdsEstimationGetRequest` remarks |
| 路由 / 计数（恰 1）/ `[Post]` / 子树复用类型 | `EstimationEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `EstimationRoutes` |
| 定向子树逐字段语义 | [../Adgroups/README.md](../Adgroups/README.md) 与 `AdsAdgroupTargeting` |
| 定向取值（`regions` 等）从哪来 | [../TargetingTags/README.md](../TargetingTags/README.md) |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/estimation/get` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `EstimationRoutes` 与计数断言（1）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Estimation`）→ 跑两个脚本 →
   更新登记断言（守卫 `ADS-B6`）。
3. **不要**为「对齐 REST 语义」把 `[Post]` 改成 `[Get]`；**不要**给本域加 `user_token` 或 `X-Request-Id`；
   **不要**复制一套同形的 targeting/scene_spec DTO（守卫按属性类型断言，复制即红）。
4. 想加「ADX 账号本地预判」=> 先解决「模式由账号侧决定、SDK 不可见」这一前提，否则属越出契约的本地拦截。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
