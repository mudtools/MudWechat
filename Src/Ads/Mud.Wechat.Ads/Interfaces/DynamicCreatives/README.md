# DynamicCreatives 域 · 组件化创意

腾讯广告 Marketing API v3.0「组件化创意」域：获取 / 创建 / 更新 / 删除，共 **4 支端点**。
接口 [IWechatAdsDynamicCreativeService.cs](./IWechatAdsDynamicCreativeService.cs)，注册组 `DynamicCreatives`，
模块枚举 `AdsModule.DynamicCreatives`。逐页核验 2026-10-11（层级经 DOM `level-*` 树核验）。

> 事实来源限定：逐参数原文以 [IWechatAdsDynamicCreativeService.cs](./IWechatAdsDynamicCreativeService.cs) 为准；
> 路由 / 动词 / 计数 / `user_token` 逐方法面以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 在资源层级里的位置

`advertiser` → `adgroups` → **`dynamic_creatives`（本域）** → `components`。
本域是「把组件组装成一条可投放的创意」，组件本身的生命周期在
[../Components/README.md](../Components/README.md)。

## 端点速览

| 方法 | 动词 | 路由 | `user_token` | 官方顶层键数 | 必填集 |
|---|---|---|---|---|---|
| `GetAsync` | **GET** | `/v3.0/dynamic_creatives/get` | **不带** | — | `account_id` |
| `AddAsync` | POST | `/v3.0/dynamic_creatives/add` | 带 | **16** | `account_id` / `adgroup_id` / `dynamic_creative_name` / `creative_components` |
| `UpdateAsync` | POST | `/v3.0/dynamic_creatives/update` | 带 | **10** | 见请求 DTO |
| `DeleteAsync` | POST | `/v3.0/dynamic_creatives/delete` | 带 | **2** | `account_id` / `dynamic_creative_id` |

- **权限**：四端点「所属权限」均为 `ads_management`。
- **`user_token` 逐方法锁定**：官方 `add` / `update` / `delete` 三页各列该参数，`get` 页无「特定请求参数」表。
  守卫点名断言「带 `user_token` 的方法集合恒等于 `{AddAsync, DeleteAsync, UpdateAsync}`」，
  并对 `GetAsync` 做反向断言 ⇒ 给查询补凭据即红。`user_token` **属凭据**，不得进日志 / 遥测 / 异常消息。
- 四支端点 `data` 只回 `dynamic_creative_id`。

## 组件 `value` 是逐组件 union：一期不建型

`creative_components` 的 **44 个组件键**（`AdsCreativeComponents`）每个都是 `List<AdsCreativeComponentItem>`，
元素形状为 `{component_id, value, is_deleted}`。其中 **`value` 是逐组件 union**，一期以
`Dictionary<string, JsonElement>` 承载（裁剪决策 D1）：

- **读侧**：官方键原名入袋（字典键不经命名策略转换，与报表行先例同源）⇒ **零丢字段**。
- **写侧**（本域 `add` / `update`、`components/add`）由调用方以 **JSON 字面量**构造一个
  `Dictionary<string, JsonElement>`；构造样例与理由见
  [`AdsCreativeComponentItem` 的 remarks](../../DynamicCreatives/AdsDynamicCreative.cs)
  （本仓 DTO 文档是该形态的唯一权威描述，本文不另立口径）。

> **AOT 注意**：各域 `JsonContext` 为 `internal`（与本线类型同程序集；`InternalsVisibleTo` 只开放给测试工程），
> 宿主侧构造 `value` 时**不要**依赖反射重载 `JsonSerializer.Deserialize<T>(string)`——
> 全线 AOT 净零的前提是零反射 JSON（守卫 `ADS-B6`，`AdsSources_ShouldNotUseReflectionJson`）。
> 需要公开构造入口时，按 AGENTS.md §2 走「补公开面 + 同批补守卫」，不要在示例里临时发明 API。

⇒ 不要为了「类型安全」把 44 支 union 展开成 44 个 DTO：官方键集会随创意形式变，
建型必然失真；消费需求驱动时再逐组件建型（键集与字段名已由守卫锁定）。

## 必填组合由创意形式决定，SDK 不校验

`creative_components` 的 44 个组件键**官方不逐个标必填**，实际必填组合随 `creative_template_id`（创意形式）变化，
而官方**未在页面给出逐形式矩阵** ⇒ SDK 不做本地校验，组件缺失由应答 `code` 表达。
要拿「某形式需要哪些组件」的事实，走 `component_detail/get`（见 Components 域）。

## `update` 与 `add` 不同构（三条差异）

1. 顶层 **10 键**对 `add` 的 16 键——**创建期事实字段不可更新**，别按 `add` 的形状填 `update` 的请求体。
2. `update` 另有 `is_retry_batch_update`（`add` 页没有）。
3. `creative_components` 在 `update` 上是**增量更新**语义（不是整体替换）。

## 查询页的双分页形态

应答同时定义 `page_info`（普通翻页）与 `cursor_page_info`（游标翻页），取哪组由请求的 `pagination_mode` 决定。

**游标类型与本线其它域不同构**：本域 `cursor_page_info` 的 `next_cursor` / `previous_cursor` 是**字符串**，
而 `advertiser/get` 的游标是**整数** ⇒ 逐域分建 DTO，不得抽成公共游标类型（`adgroups/get` 的游标同样是字符串、
且请求侧参数名 `cursor` 与应答侧 `next_cursor` **不同名**）。

> **本域未核验面（照录，勿当已核验事实使用）**：`filtering` 的 `field` / `operator` 可选值、
> `pagination_mode` 的可选值在接口 XML 里明确标注「**未逐项核验**」。需要使用时先逐页核验官方页面，
> 把结论补进接口 remarks，**同批**更新本 README。

## 邻域约束：同 `adgroup` 下创意写操作必须串行

官方对创意族写「同一个营销单元（`adgroup`）下，创意的新建、更新和删除操作**必须串行执行**」，
粒度是 `adgroup_id`。该事实当前登记在
[IWechatAdsAdgroupService.cs](../Adgroups/IWechatAdsAdgroupService.cs) 的接口 remarks
（「串行约束不在本域但在邻域」一段），**本域接口尚未逐页摘录该句原文** ⇒ 串行节奏归调用方，SDK 不做进程内排队。
若要把它变成本域契约面，须先回到官方页面取原文再补进接口注释。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddDynamicCreativesApi());

var resp = await dynamicCreatives.GetAsync(
    accountId: 1234567890,
    filtering: AdsQueryJson.Filtering(new AdsFiltering
    {
        Field = "adgroup_id",
        Operator = "EQUALS",
        Values = ["987654321"],
    }),
    page: 1,
    pageSize: 100,
    cancellationToken: ct);

WechatAdsException.ThrowIfFailed(resp, "/v3.0/dynamic_creatives/get");
```

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数与逐端点必填集原文 | [IWechatAdsDynamicCreativeService.cs](./IWechatAdsDynamicCreativeService.cs) 的 XML 注释 |
| 路由 / 动词 / 计数（恰 4） | `DynamicCreativesEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `DynamicCreativesRoutes` |
| `user_token` 逐方法集合 | `RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` |
| 44 组件键与 `value` union 形态 | `AdsCreativeComponents` / `AdsCreativeComponentItem` remarks（`.DataModels/DynamicCreatives/`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/{get,add,update,delete}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `DynamicCreativesRoutes` 与计数断言。
2. 改 `user_token` 面（增/删方法的该参数）→ 同批更新 `RestrictedWriteSurfaces…` 里被点名的方法集合。
3. 新增组件键 → 键名与 `[JsonPropertyName]` 由 `AdsDataModelJsonNames_ShouldStayOfficialSnakeCase` 锁定；
   `value` 仍走开放字典，除非有明确消费需求才建型。
4. 补「未逐项核验」面 → 必须先逐页核验，再写进接口 remarks，**不要**在 README 里先落未核定的取值。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
