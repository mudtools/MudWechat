# Mud.Wechat.Ads.DataModels

腾讯广告 **Marketing API v3.0** 产品线的**官方 DTO 包**（**在建**：已随三域端点落地 5 个源生成 JSON 上下文）。

## 已承载

| 上下文 | 域 | 内容 |
| --- | --- | --- |
| `AdvertiserJsonContext` | 客户账号 | `advertiser/get`（含 `individual_qualification` / `websites[]` / `operators[]` 子树）+ `update` + `update_daily_budget` |
| `AdgroupsJsonContext` | 营销单元 | `adgroups/get` 应答全树 + `add` / `update` 请求 + 四支批量端点的 `spec[]` 与逐条结果元素 |
| `ReportsJsonContext` | 报表 | daily / hourly 共用 `AdsReportListData`、`async_reports/add` 请求与应答、`async_reports/get` 的三层链路（`result.data.file_info_list`） |
| `OAuthJsonContext` | OAuth | `oauth/token` 与 `oauth/refresh_token` 的请求 / 应答（`authorizer_info` 一层子字段） |
| `CommonJsonContext` | 跨域 | 信封基类 `AdsResponse`（判错门面属性显式 `[JsonIgnore]`）、`AdsPageInfo`、`AdsBatchResultItem`、复合查询参数 struct（`AdsDateRange` / `AdsOrderBy` / `AdsFiltering`） |

`AdsQueryJson`（`Common/`）是复合查询参数的**唯一**编码器：官方线格式要求 `date_range` / `fields` / `group_by` / `order_by` / `filtering` 各以单个紧凑 JSON 字面量上送，编码器写出的键名与 struct 声明的 `[JsonPropertyName]` 由守卫 ADS-B2 断言**双向恰等**（编码器不得有 struct 之外的键）。

三个易踩的形态事实（各有用例或守卫钉住，勿「顺手统一」）：

- **每个公共属性都必须显式 `[JsonPropertyName]`（或 `[JsonIgnore]`）**，不靠命名策略兜底。当前五个上下文都取 `SnakeCaseLower`，漏标看似无伤；但策略是脚手架产出、逐上下文独立，一旦某上下文漂移或 DTO 换目录，「是否合官方契约」就变成「文件放在哪」的偶然事实，且**两种错位都不报错**（上送被官方静默忽略、应答属性恒 `null`）。守卫 ADS-B2 因此做「覆盖面 + 形状 + 关键名」三面判定。
- **字典键不受命名策略影响**：报表行是 `Dictionary<string, JsonElement>`，而 `DictionaryKeyPolicy` 未设 ⇒ 官方返回的 `view_count` 等键**原名入袋**（`AdsReportJsonTests` 用驼峰键样例反证：若策略命中键名即刻变红）。
- **开放泛型 `AdsResponse<TData>` 刻意不登记**（标了会产 `SYSLIB1030` 死登记）⇒ 每支端点必须声明**闭合**应答类型，不能靠开放泛型取 `JsonTypeInfo`。


## 落位与约定（沿用全仓口径）

- 命名空间 `Mud.Wechat.Ads.DataModels.{域}[.{子域}]`；请求 / 响应模型分目录仅作组织，**不入命名空间**。
- 每个 DTO 标 `[HttpJsonSerializable]` ⇒ 跑 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` 产出 `Generated/{组}JsonContext.g.cs`（**生成物**，提交进版本控制、勿手改）。上下文 `internal`，经 `InternalsVisibleTo` 供主包与测试直读。
- **AOT 净零**：禁反射版 `JsonSerializer.Serialize<T>` / `Deserialize<T>`，一律走 `JsonTypeInfo` 或本线合并解析器（守卫 ADS-B6 以源码文本扫描钉死，剔除注释后计数，避免对文档提及误报）。
- **漏登记 = Native AOT 首调即失败**（JIT 下一路正常），故新增 DTO 必须同批补上下文登记断言。

## v3.0 报文形态（与微信线的三点差异）

1. 字段名为 **snake_case**，照官方原文拼写，不做大小写「修正」（全仓各线同一纪律）。
2. 响应是 **两级信封** `{code, message, message_cn, data}`：`message` 为英文、`message_cn` 为中文，两者都是官方字段 ⇒ **都要建模**，不得只留一个。
3. `data` 是**外层包装**：列表型资源的数组与游标分页字段都在 `data` 之内而非顶层 ⇒ 需要一层泛型信封建模，不能把资源 DTO 直接当数组反序列化。**包装内的具体字段名逐端核验后照官方原文建模**（官方多页此处口径不一致，见下节纪律），本文件不预先断言。

## 官方自相矛盾的处置纪律

v3.0 文档存在多处逐页不一致（批量上限、名称长度口径、游标分页字段、`account_id` 必填口径等）。纪律与开放平台线一致：**照多数页面拼写建模并在 remarks 留档**，联调发现只认另一种时**同批**改常量与用例，不得两处各写一份；**UNVERIFIED 项一律不写成守卫断言**。

## 依赖

- `Mud.Wechat.Abstractions`（响应契约 `IWechatApiResponse`）、`Mud.HttpUtils.Attributes` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（Analyzer）
- TFM：继承根 props 四档（`netstandard2.0` / `net6.0` / `net8.0` / `net10.0`；JSON 上下文仅 `net8.0+` 生效）

`InternalsVisibleTo` 开放给 `Mud.Wechat.Ads`、`Mud.Wechat.Ads.Abstractions` 与 `Mud.Wechat.Ads.Tests`。
