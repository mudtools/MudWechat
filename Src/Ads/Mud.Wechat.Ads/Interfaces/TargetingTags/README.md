# TargetingTags 域 · 定向标签

腾讯广告 Marketing API v3.0「定向标签」域：**1 支端点**（`targeting_tags/get`）。
接口 [IWechatAdsTargetingTagService.cs](./IWechatAdsTargetingTagService.cs)，注册组 `TargetingTags`，
模块枚举 `AdsModule.TargetingTags`（`AddTargetingTagsApi()`）。逐页核验 2026-10-17。

> 事实来源限定：逐参数原文以接口文件 XML 注释与 `AdsTagSpec` / `AdsTagQuerySpec` remarks 为准；
> 路由 / 参数顺序 / 键集以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 方法 | 动词 | 路由 | 权限 | `user_token` |
|---|---|---|---|---|
| `GetAsync` | GET | `/v3.0/targeting_tags/get` | `ads_management` | 无 |

- **本页没有「特定请求参数」表** ⇒ 不带 `user_token`（守卫 `ADS-B5`）；全局参数
  `access_token` + `timestamp` + `nonce` 由 `AdsAuthorizationHandler` 成组注入（`ADS-B1`）。
- **Query 参数顺序即契约**：守卫断言逐字 `Equal({ "account_id", "type", "tag_spec" })`，
  顺序变化（哪怕语义不变）即红。

## 本域是投放定向取值的**供给侧**

`adgroups` 定向的 `regions` / `business_districts` / `device_brand_model` 三支列表，官方原文均标注
「**可通过 targeting_tags/get 接口获取**」⇒ 本域应答的 `id`（`long`）就是那些参数的取值来源：

```
targeting_tags/get (type=REGION)  →  id: 110000  →  AdsAdgroupTargeting.Regions = [110000]
                                                   →  AdsEstimationGetRequest.Targeting.Regions = [110000]
```

`AdsAdgroupTargeting.Regions` 是 `List<long>?`（整数，不是字符串）⇒ 回填时**不要**做 id 的字符串化，
也不要把 `name` 当取值（见下文「组树以 `id` 为键」）。

## 三个参数

| 参数 | 官方 | 事实 |
|---|---|---|
| `account_id` | `integer`，**必填** | 原文「有操作权限的帐号 id，**不支持代理商 id**」（同 Qualifications / Estimation 域口径） |
| `type` | `enum`，**必填** | 本页「枚举详情」**未展开取值全集** ⇒ 不建枚举、不本地枚举。正文与示例可证得的取值：`BEHAVIOR`（此时 `tag_spec.behavior_spec` 必填）、`INTEREST`（此时 `tag_spec.interest_spec` 必填）、`REGION`（官方示例形态**不带** `tag_spec`） |
| `tag_spec` | `struct`，选填 | **JSON 对象字符串**，构造入口唯一 `AdsQueryJson.TagSpec`；官方 curl 原文 `-d 'tag_spec={"interest_spec":{"query_spec":{}}}'` |

**`tag_spec` 必须是单个 JSON 字面量字符串**：GET 上的 `struct` 参数若走数组/复杂类型绑定形态，
官方一律不认（守卫同时断言参数 CLR 类型 `== typeof(string)`）。这与本线其余复合 Query 参数
（`fields` / `filtering` / `date_range` / `group_by` / `order_by`）同口径，入口集中在 `AdsQueryJson`。

## `tag_spec` 内部的三级必填链

```
AdsTagSpec
 ├─ behavior_spec ┐  两支兄弟结构逐字段同构（官方把子字段重复登记了两遍）
 └─ interest_spec ┘  ⇒ 共用 AdsTagQuerySpec 一支类型，不复制第二套
      ├─ query_mode              【必填】enum，5 支（2026-10-17 逐字核验）：
      │                          { TARGETING_TAG_QUERY_MODE_SEARCH, _RECOMMEND, _COMMON,
      │                            _ADVANCED_RECOMMEND, _VERIFY_QUERY_LIST }
      ├─ query_spec              「当 query_mode 为 VERIFY_QUERY_LIST / SEARCH / RECOMMEND 时必填」
      │    ├─ query                          1–200 字节
      │    ├─ query_list                     数组 1–250、每项 1–64 字节；
      │    │                                 「当且仅当 query_mode = VERIFY_QUERY_LIST 时可填且必填」
      │    │                                 ⇒ 该模式下**应答仅返回有效关键词**
      │    ├─ max_result_number              最小 1、最大 50
      │    └─ excluding_targeting_tags       数组 1–250；「已经选中的标签，在结果中不会再次出现」
      └─ advanced_recommend_type  「当 query_mode = ADVANCED_RECOMMEND 时必填」，2 支：
                                 { ADVANCED_RECOMMEND_TYPE_SYSTEMATIC, ADVANCED_RECOMMEND_TYPE_INDUSTRY_HOT }
```

**整条链都是条件必填，SDK 一律不本地拦截**（判定跨三层结构、且合法组合随 `type` 而变），
错配由应答 `code` 表达。`REGION` 形态不需要 `tag_spec` ⇒ 「不传整个 `tag_spec`」是合法请求，
本地「必填」校验反而会挡掉它。

## 应答：无分页的**树**

`data.list[]` 元素 7 字段：`id` / `name` / `parent_id` / `parent_name` / `city_level` / `tag_class` /
`available_for_individual`。**没有 `page_info`** ⇒ 一次全量返回，与 CustomAudiences / AsyncTasks
等分页域形态不同（判「有没有下一页」在本域无意义）。

组树三条：

1. **以 `id` 为键，绝不以名称为键**（父子关系由 `parent_id` / `parent_name` 组织，名称可重复）。
2. **根节点是 `parent_id = 0` 且 `parent_name = ""`（空串，不是缺省）** ⇒ 判根要用空串比较，
   用「字段是否缺席」判根会漏掉全部根节点。
3. `city_level` / `tag_class` 官方标 `enum` 但**本页枚举详情未展开** ⇒ 建模为 `string` 留档
   （应答示例出现 `CITY_LEVEL_NONE`），**不得**从其他页借枚举集合。

`available_for_individual`（`boolean`）是**个人主体可用性**标记 ⇒ 自建/个人资质账号筛选定向时是有效闸，
但它是**应答侧数据**，SDK 不用它做本地拦截。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddTargetingTagsApi());

// 1) 取地域标签（REGION 形态：官方示例不带 tag_spec）
var regions = await tags.GetAsync(1234567890, "REGION", cancellationToken: ct);
WechatAdsException.ThrowIfFailed(regions, "/v3.0/targeting_tags/get");

// 组树：以 id 为键；根节点 parent_id = 0 且 parent_name 为空串
var byParent = regions.Data!.List!
    .GroupBy(static t => t.ParentId ?? 0)
    .ToDictionary(static g => g.Key, static g => g.ToList());
var roots = byParent[0];                        // parent_id == 0 的全部根节点

// 2) 按关键词搜兴趣标签（tag_spec 走唯一入口，产出单个 JSON 对象字符串）
var tagSpec = AdsQueryJson.TagSpec(new AdsTagSpec
{
    InterestSpec = new AdsTagQuerySpec
    {
        QueryMode = "TARGETING_TAG_QUERY_MODE_SEARCH",
        QuerySpec = new AdsTagQueryCondition { Query = "宠物", MaxResultNumber = 20 },
    },
});
var interest = await tags.GetAsync(1234567890, "INTEREST", tagSpec, ct);
WechatAdsException.ThrowIfFailed(interest, "/v3.0/targeting_tags/get");
```

**判错只有一层**（普通信封）；本域没有批量/异步族的双层判定面。

## 逐页集合差异（合并即抹平官方差异）

| 面 | 本域 | 邻域对照 |
|---|---|---|
| `type` 取值全集 | **未展开**，只可证得 3 支 | Adgroups 的定向枚举另有全集，**不得互借** |
| `query_mode` 取值 | 5 支，带 `TARGETING_TAG_QUERY_MODE_` 前缀 | Funds 的 `level` 三值**零前缀**（该页官方即如此） |
| 分页 | **无** `page_info` | Advertiser/CustomAudiences 有分页或游标 |
| 根节点表达 | `parent_id=0` + `parent_name=""` 空串 | — |
| 结构参数 | `tag_spec` 单键 JSON **对象**字符串 | `filtering`/`fields` 是 JSON **数组**字符串 |

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `type` 的取值全集 | 官方「枚举详情」未展开 ⇒ 只有正文与示例可证的 `BEHAVIOR` / `INTEREST` / `REGION` | 需要其它取值时先逐页核验官方页，再补接口 remarks 与本文 |
| `city_level` / `tag_class` 取值集 | 同上，未展开 ⇒ `string` 留档 | 不得从邻域借枚举 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（含必填星号与 curl 原文） | `IWechatAdsTargetingTagService.cs` 的 XML 注释 |
| 路由 / 计数（恰 1）/ Query 顺序与 `tag_spec` 的 CLR 类型 | `TargetingTagsEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `TargetingTagsRoutes` |
| `tag_spec` 内部键集（`behavior_spec` / `interest_spec` 等） | `AdsQueryJsonKeys_ShouldMatchCompositeDtoContracts` |
| 三级必填链与各枚举逐字取值 | `AdsTagSpec` / `AdsTagQuerySpec` / `AdsTagQueryCondition` remarks（`DataModels/TargetingTags/`） |
| 应答 id 回填到哪三支定向参数 | `AdsAdgroupTargeting` 对应字段的 remarks（`DataModels/Adgroups/`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/targeting_tags/get` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `TargetingTagsRoutes` 与计数断言（1）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `TargetingTags`）→ 跑两个脚本 →
   更新登记断言（守卫 `ADS-B6`）。
3. 动 `AdsTagSpec` 的键名 → 必须同批改 `AdsQueryJson.TagSpec` 的手写写出逻辑与键集断言（两侧不同批 = 静默丢字段）。
4. **不要**给本端点加 `user_token`；**不要**把 `tag_spec` 改成复杂类型绑定；**不要**把 `type`
   补成本地枚举（官方未展开取值 ⇒ 补齐即编造）。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
