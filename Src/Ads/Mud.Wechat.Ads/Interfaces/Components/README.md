# Components 域 · 创意组件

腾讯广告 Marketing API v3.0「创意组件」域：组件的查 / 建 / 删 + 组件详情探测，共 **4 支端点、跨 2 支资源族**
（`components/get|add|delete` + `component_detail/get`）。
接口 [IWechatAdsComponentService.cs](./IWechatAdsComponentService.cs)，注册组 `Components`，
模块枚举 `AdsModule.Components`。逐页核验 2026-10-11（层级经 DOM `level-*` 树核验）。

> 事实来源限定：逐参数原文以 [IWechatAdsComponentService.cs](./IWechatAdsComponentService.cs) 为准；
> 路由 / 动词 / 计数 / `user_token` 面以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 在资源层级里的位置与「一族一模块」的例外

`advertiser` → `adgroups` → `dynamic_creatives` → **`components`（本域，层级最底）**。
组件是被创意引用的原子单元（文案 / 图片 / 视频 / 行动按钮…），`dynamic_creatives` 负责组装它们。

`component_detail/get` 不属于 `components/*` 资源族，但它与本域三支同为 `ads_management` 权限、
语义是「先探组件在该创意形式下需要什么」⇒ 与三支收在同一接口、同一注册组（理由见 `AdsModule.Components`）。
守卫按族划分再断言一次：`components/` 三支 + `component_detail/` 一支 = 恰 4。

## 端点速览

| 方法 | 动词 | 路由 | `user_token` | 必填集 |
|---|---|---|---|---|
| `GetAsync` | GET | `/v3.0/components/get` | 不带 | —（参数全可选） |
| `AddAsync` | POST | `/v3.0/components/add` | **带** | `component_value` / `component_sub_type` |
| `DeleteAsync` | POST | `/v3.0/components/delete` | 不带 | `component_id` |
| `GetDetailAsync` | GET | `/v3.0/component_detail/get` | 不带 | —（`ad_context` 内含条件必填链） |

**`user_token` 只在 `add` 一页**：官方 `delete` 与两支 get 页均未列「特定请求参数」表。
守卫点名断言「带 `user_token` 的方法集合恒等于 `{AddAsync}`」⇒ 给删除或查询补凭据即红。

## `component_value`：40 键**单数** struct（与创意域的 44 键数组不同构）

本域容器是 `AdsComponentValue`，**40 个组件键、每键是单个 `struct`**；
而 `dynamic_creatives` 的 `creative_components` 是 **44 个键、每键是数组**。
两支**同名不同构**（见 `AdsComponentValue` remarks）⇒ 不得抽成公共类型、不得按另一支的形状填这一支。

组件条目元素形状 `[{component_id, value, is_deleted}]` 两支一致 ⇒ 元素类型**复用**
`Mud.Wechat.Ads.DataModels.DynamicCreatives.AdsCreativeComponentItem`（`value` 同样是逐组件 union，
以 `Dictionary<string, JsonElement>` 开放承载、零丢字段；写侧构造口径见
[../DynamicCreatives/README.md](../DynamicCreatives/README.md)）。

## `component_detail/get` 的 `ad_context`：唯一没有 `AdsQueryJson` 入口的复合参数

`ad_context` 官方为 `struct`、**3 层深**，内部必填链为
`marketing_goal` / `marketing_carrier_type` / `marketing_target_type` / `site_set` / `creative_template_id`。
它是 **Query 上的 JSON 对象字符串**（声明式客户端会把复杂类型逐属性展平，那不是官方线格式）⇒ 参数类型为 `string`。

但 `AdsQueryJson` 只覆盖 `Fields` / `Filtering` / `DateRange` / `GroupBy` / `OrderBy` / `TagSpec` 六支
（与复合 DTO 的键名由守卫 `AdsQueryJsonKeys_ShouldMatchCompositeDtoContracts` 锁定），**没有 `ad_context` 入口**
⇒ 本支由调用方自构 JSON 字面量。要把它收进 `AdsQueryJson`，须按 AGENTS.md §2「加公开面 + 同批补守卫」走。

## 删除：两种策略，且没有 `user_token`

`delete_strategy` 可选值 `{ DELETE_STRATEGY_FORCE, DELETE_STRATEGY_RESTRICTED }`（强制 / 受限）。
请求体官方 4 键、`component_id` 必填；`data` 只回 `component_id`。
注意本支是**四支里唯一有「策略」语义的写端点**，官方未在本页说明被创意引用中的组件在两种策略下的行为差异
⇒ 先探（`component_detail/get`）再删是稳妥路径，SDK 不做引用关系预判。

## 查询页的两个专属参数

- `organization_id`：本域两支 get 页都有（与 `Reports` 域「hourly 页没有」形成对照，逐页取原文）。
- `component_id_filtering_mode`：官方 `enum`，**可选值本仓未逐项核验**（接口 XML 已如实标注）
  ⇒ 需要时先逐页核验官方页面再补进 remarks，不要凭印象传值。
- `filtering` / `fields` 均为 JSON 字面量字符串，入口 `AdsQueryJson.Filtering` / `AdsQueryJson.Fields`。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddComponentsApi());

var list = await components.GetAsync(accountId: 1234567890, page: 1, pageSize: 100, cancellationToken: ct);
WechatAdsException.ThrowIfFailed(list, "/v3.0/components/get");

var del = await components.DeleteAsync(
    new AdsComponentDeleteRequest { ComponentId = 555, DeleteStrategy = "DELETE_STRATEGY_RESTRICTED" },
    cancellationToken: ct);
WechatAdsException.ThrowIfFailed(del, "/v3.0/components/delete");
```

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数与逐端点必填集原文 | [IWechatAdsComponentService.cs](./IWechatAdsComponentService.cs) 的 XML 注释 |
| 路由 / 动词 / 计数（恰 4）/ 族划分 | `ComponentsEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `ComponentsRoutes` |
| `user_token` 仅 `AddAsync` | `RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` |
| 40 键单数容器与 44 键数组容器的差异 | `AdsComponentValue` / `AdsCreativeComponents` remarks（`.DataModels/{Components,DynamicCreatives}/`） |
| 复合参数键名一致性 | `AdsQueryJsonKeys_ShouldMatchCompositeDtoContracts`、`AdsDataModelJsonNames_ShouldStayOfficialSnakeCase` |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{components/get,components/add,components/delete,component_detail/get}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `ComponentsRoutes` 与计数断言。
2. 改组件键集（40 支）→ 键名与 `[JsonPropertyName]` 由 snake_case 守卫锁定；`value` 仍走开放字典。
3. 若把 `ad_context` 收进 `AdsQueryJson` → 同批更新 `AdsQueryJsonKeys…` 的键名比对与本文说明。
4. 改 `user_token` 面 → 同批更新 `RestrictedWriteSurfaces…` 里被点名的方法集合。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
