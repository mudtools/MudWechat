# Adgroups 域 · 营销单元

腾讯广告 Marketing API v3.0「营销单元」域：查询 / 创建 / 更新 / 删除 + 四支批量改字段，共 **8 支端点**。
接口 [IWechatAdsAdgroupService.cs](./IWechatAdsAdgroupService.cs)，注册组 `Adgroups`，模块枚举 `AdsModule.Adgroups`。

> 本 README 只汇编**已在接口 XML 注释与契约守卫中留档**的事实。逐参数原文以
> [IWechatAdsAdgroupService.cs](./IWechatAdsAdgroupService.cs) 为准；路由 / 动词 / 端点计数 / 逐页凭据面以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 层级事实：本域存在的理由，也是本域不建模的东西

v3.0 的资源层级是 `advertiser`（客户账号）→ **`adgroups`（营销单元）** → `dynamic_creatives`（创意）→ `components`（创意组件）。

官方 v3.0 清单里**不存在** `campaigns/*`（推广计划层已在 v3.0 被移除）与 `ads/*`（广告实例不再作为独立资源族暴露）
⇒ 本 SDK **不建模**那两层。任何「按计划层级组织投放」的尝试都属于对齐已下线旧 API，
由守卫 `AdsRoutes_ShouldEqualGuardTablesAndNeverReviveRemovedResources` 的零路由断言拦住回潮。

## 端点速览

| 方法 | 动词 | 路由 | `user_token` | 幂等头 |
|---|---|---|---|---|
| `GetAsync` | **GET** | `/v3.0/adgroups/get` | **不带**（本页无「特定请求参数」表） | 无 |
| `AddAsync` | POST | `/v3.0/adgroups/add` | 带 | **`X-Request-Id`** |
| `UpdateAsync` | POST | `/v3.0/adgroups/update` | 带 | 无 |
| `DeleteAsync` | POST | `/v3.0/adgroups/delete` | 带 | 无 |
| `UpdateDailyBudgetAsync` | POST | `/v3.0/adgroups/update_daily_budget` | 带 | 无 |
| `UpdateConfiguredStatusAsync` | POST | `/v3.0/adgroups/update_configured_status` | 带 | 无 |
| `UpdateBidAmountAsync` | POST | `/v3.0/adgroups/update_bid_amount` | 带 | 无 |
| `UpdateDatetimeAsync` | POST | `/v3.0/adgroups/update_datetime` | 带 | 无 |

- **权限**：8 支端点官方「所属权限」均为 `ads_management`，仅 `get` 额外带 `ads_insights`。
  权限不足由应答 `code` 表达，SDK 不做本地权限预判。
- **动词**：只有 `get` 是 GET，其余 7 支均为 POST + `application/json`。
- **`user_token` 只出现在非 GET 面**：官方 4 支写端点与 4 支批量端点的「特定请求参数」表都列它，
  且原文写明「注意**不是**放在 header 中」⇒ 必须是 Query。`get` 页没有该表，一支都不带。
- **`X-Request-Id` 只存在于 `add`**（本域唯一有官方幂等面的写端点）。`update` / `delete` 与四支批量页均无该请求头表
  ⇒ 重试时每次新生成值等于放弃幂等保护；复用同一值才有「重复请求永不新建资源」的保证。
- **批量族命名不可缩写**：`update_configured_status` / `update_bid_amount` / `update_datetime` 三支的路由尾段与
  请求数组名（`update_*_spec`）成对出现。`update_datetime` 写成 `update_date` 是**官方本页使用说明里的笔误**
  （原文误写 `update_date_spec`）⇒ 报文取请求表、路由取「请求地址」。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddAdgroupsApi());

public sealed class AdgroupPause(IWechatAdsAdgroupService adgroups)
{
    public async Task<IReadOnlyList<(long Id, int Code)>> PauseAsync(long accountId, IReadOnlyList<long> ids, CancellationToken ct)
    {
        var req = new AdsAdgroupUpdateConfiguredStatusRequest
        {
            AccountId = accountId,
            UpdateConfiguredStatusSpec = ids.Select(id => new AdsAdgroupUpdateConfiguredStatusSpec
            {
                AdgroupId = id,
                ConfiguredStatus = "AD_STATUS_SUSPEND",
            }).ToList(),
        };

        var resp = await adgroups.UpdateConfiguredStatusAsync(req, cancellationToken: ct);
        WechatAdsException.ThrowIfFailed(resp, "/v3.0/adgroups/update_configured_status"); // 只保证外层被受理

        // 外层 code == 0 不代表每条成功：逐条判定不可省略。
        return resp.Data?.List?.Select(i => (i.AdgroupId ?? 0, i.Code ?? -1)).ToList() ?? [];
    }
}
```

## 查询页（`adgroups/get`）的四个反直觉点

1. **翻页上限与 `advertiser` 域不同**：本页 `page` 1–**100**、`page_size` 1–100；而 `advertiser/get` 的 `page` 上限是 1000
   ⇒ 两域翻页策略不同，别共用一个常量。
2. **游标两侧不同名**：请求传 `cursor`（`string`，0–10 字节，首次不填），回填值取应答的 **`next_cursor`**。
   官方原文「如果你希望获取全量数据，推荐采用游标分页」，但每次查询时除游标分页参数外**其余参数须保持一致**；
   游标模式下未保存初次数据快照 ⇒ 数据变化可能导致结果不一致；游标有效期 **24 小时**。
3. **`fields` 的合法取值官方本页没有枚举** ⇒ 事实取值集即 `AdsAdgroupInfo` 的字段名；
   未请求的字段在应答里为 `null` 属正常形态，不是缺陷。
4. **官方本页没有可用的应答示例**：示例恰为 `{"code":0,"message":"","message_cn":""}`，整个 `data` 缺席，
   而应答字段表有近 80 个顶层字段 ⇒ 应答形态的唯一来源是字段表；建模细节与三处「平面阅读会读错层级」的坑
   见 `AdsAdgroupInfo` remarks（层级由守卫 `AdgroupsFieldLevels_ShouldMatchVerifiedDocumentHierarchy` 锁定）。

`filtering`（数组 1–255，官方原文「若获取联盟营销位信息此字段必填」）的 `field` 可选值 9 支：
`{ adgroup_id, adgroup_name, created_time, last_modified_time, material_package_id, configured_status,
joint_budget_rule_id, auto_derived_creative_enabled, rta_target_id }`；`operator` 逐字段不同
（`adgroup_id` → `{EQUALS, IN}`、`adgroup_name` → `{EQUALS, CONTAINS}`、时间两支 → `{EQUALS, LESS_EQUALS, LESS,
GREATER_EQUALS, GREATER}`、`configured_status` 与 `auto_derived_creative_enabled` 仅 `{EQUALS}`）；
`values` 在 `adgroup_id` + `IN` 下长度 1–100，时间类字段 10 字节，`configured_status` 取
`{AD_STATUS_NORMAL, AD_STATUS_SUSPEND}`。

**官方本页自相矛盾（照录，SDK 不校验）**：`values` 规则引用了 `field` 可选值集合里根本不存在的
`smart_delivery_platform`；而可选值里的 `rta_target_id` 没有任何 operator/values 规则。

## 写侧：条件必填极重，SDK 一律不拦

- `add` 官方参数表 **162 行**，顶层必填集为 `account_id` / `adgroup_name` / `marketing_goal` /
  `marketing_carrier_type` / `begin_date` / `end_date` / `time_series`；`data` 只回新建的 `adgroup_id`。
- 官方对大量「可选结构内的子字段」标 `*`（如 `targeting.geo_location.location_types`、
  `user_action_sets[].type/id`、`deep_conversion_*_spec.goal`、`prospect_retargeting.enabled`），
  真实语义是「**父结构在场时**子字段必填」⇒ 本地按 `*` 拦截会把「整个父结构不传」这一合法形态一起挡掉。
- 两条跨结构二选一依赖推广产品类型（`marketing_asset_outer_spec` 与 `marketing_asset_id` 互斥；
  动态商品营销时 `mpa_spec` 必填），SDK 侧不可见该类型 ⇒ 不预判。
- **ADX 程序化投放的禁用面**：官方对 `scene_spec`、`daily_budget`、`deep_conversion_spec`、`configured_status`
  等十余字段重复标注「ADX 程序化投放不可填写提交」，完整清单见 `AdsAdgroupAddRequest` remarks。
- `update` 官方参数表 **123 行**，顶层仅 `account_id` + `adgroup_id` 必填。
  **官方不开放的字段集是事实而非遗漏**：`marketing_goal` / `marketing_carrier_type` / `site_set` /
  `automatic_site_enabled` / `mpa_spec` / `dca_spec` / `marketing_asset*` / `bid_scene` / `smart_bid_type`
  等约 23 支在 `add` 存在、在 `update` 不存在 ⇒ **不得**为「对齐 add」补上。
- `delete` 官方只有 `account_id` + `adgroup_id` 两个必填字段，**不存在批量删除形态**。
  本页没有使用说明章节、没写删除可否恢复；但 `get` 提供 `is_deleted` 请求参数 ⇒ 官方为**标记删除**。
  该推断只作提示，不构成 SDK 承诺。

## 四支批量端点的共同形状与逐支差异

共同：请求体 = `account_id` + 一个 `update_*_spec` 数组；应答 = `data.list[]`（**每条自带 `code`**）
+ `data.fail_id_list`。**判错必须两层都做**——外层 `code == 0` 只表示请求被受理，
部分失败时外层仍为 `0`，只判外层会把「半数失败」当成全成功。四支批量方法因此都不做单层抛出判定。

| 端点 | 数组上限 | 本页特有事实 |
|---|---|---|
| `update_daily_budget` | 官方**未标**（同族另三支标 100，账户级同名端点也标 100）⇒ 视为文档遗漏，SDK 不做本地长度拦截 | `daily_budget` 四条数值约束见 `AdsAdgroupUpdateDailyBudgetSpec`；返回顺序与请求 `spec` 顺序**一致** ⇒ 可按索引配对；`adgroup_id` 不允许重复 |
| `update_configured_status` | 100 | 四支里内部一致性最好的一支；`configured_status` 也可经 `UpdateAsync` 逐单元修改，本端点差异只在「一次最多 100 且逐条返回」 |
| `update_bid_amount` | 100 | 单位为分；ADX 程序化投放默认填 **200**；合法区间官方指向「出价规则」章节（按 `bid_mode` / `optimization_goal` 组合而不同）⇒ SDK 不做任何范围校验 |
| `update_datetime` | 100 | **每条至少给出一个日期/时段字段**（三字段本身全标选填 ⇒ 条件必填，不预判）；微信流量另有「更新后的结束时间至少是当前时间 6 小时之后」；官方示例体省略了标必填的 spec 数组 |

金额（预算、出价）单位一律是**分**，与 `advertiser` / 报表域同一口径；SDK 不做单位换算。

## 邻域约束：创意族串行不在本域

官方对创意族写「同一个营销单元（`adgroup`）下，创意的新建、更新和删除操作**必须串行执行**」——
该约束以 `adgroup_id` 为粒度，但**属 `dynamic_creatives` 域的文档面**（见
[../DynamicCreatives/README.md](../DynamicCreatives/README.md)）。本域只承载「`adgroup` 自身」的读写。

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数类型 / 数组长度 / 条件必填原文 | [IWechatAdsAdgroupService.cs](./IWechatAdsAdgroupService.cs) 的 XML 注释 |
| 路由 / 动词 / 端点计数（恰 8） | `AdgroupsEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `AdgroupsRoutes` |
| 逐页 `user_token` 与 `X-Request-Id` 有无 | `AdgroupsQueryCredentialAndIdempotencyHeader_ShouldMatchPerPageOfficialTables`、`RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` |
| 应答字段层级（近 80 顶层字段） | `AdgroupsFieldLevels_ShouldMatchVerifiedDocumentHierarchy` + `AdsAdgroupInfo` remarks |
| 批量逐条自带 `code` | `BatchResultItems_ShouldCarryTheirOwnCode` |
| 已下线资源族不得回潮 | `AdsRoutes_ShouldEqualGuardTablesAndNeverReviveRemovedResources` |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/adgroups/{get,add,update,delete,update_daily_budget,update_configured_status,update_bid_amount,update_datetime}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 注释补齐官方文档 URL + 数组上限 / 串行 / 覆盖删除 / 权限面，**同批**更新
   `AdgroupsRoutes` 与端点计数断言（新增端点未入表即红）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取命名空间域段 `Adgroups`）→
   跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 更新守卫的上下文登记断言。
3. 改 `update` 的字段面时先核对官方「add 有、update 没有」清单，别按「应该一致」补字段。
4. **不要**引入 `campaigns` / `ads` 两支资源族；**不要**给批量端点加数组长度本地拦截。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
