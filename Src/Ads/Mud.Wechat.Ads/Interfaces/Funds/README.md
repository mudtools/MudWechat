# Funds 域 · 资金账户

腾讯广告 Marketing API v3.0「资金账户」域：查资金 / 发起转账 / 拉流水明细 / 查实时消耗，共 **4 支端点、跨 4 支资源族**
（`funds` / `fund_transfer` / `fund_statements_detailed` / `realtime_cost`）。
接口 [IWechatAdsFundsService.cs](./IWechatAdsFundsService.cs)，注册组 `Funds`，模块枚举 `AdsModule.Funds`。逐页核验 2026-10-17。

> 事实来源限定：逐参数原文以 [IWechatAdsFundsService.cs](./IWechatAdsFundsService.cs) 为准；
> 路由 / 动词 / 计数 / 逐页枚举集合以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 为什么四支资源族合成一个模块

四族权限同一（全部 `account_management`）、信封与分页形态同构、链路语义一体（查资金 → 转账 → 拉流水 → 查实时消耗）
⇒ 一支接口、一个注册组（口径同 `AdsModule.Reports`）。

`realtime_cost/get` 官方在文档树中置于「账户管理 - 资金账户」章节内、且权限与其余三面同为 `account_management`
⇒ 落本域而**不是** `Reports` 域（报表三面是 `ads_insights` 权限族，权限面不同）。

## 端点速览

| 方法 | 动词 | 路由 | 业务必填参数 |
|---|---|---|---|
| `GetFundsAsync` | GET | `/v3.0/funds/get` | `account_id` |
| `AddTransferAsync` | POST | `/v3.0/fund_transfer/add` | `account_id` / `fund_type` / `amount` / `transfer_type` |
| `GetFundStatementsDetailedAsync` | GET | `/v3.0/fund_statements_detailed/get` | `account_id` / `fund_type` / `date_range` |
| `GetRealtimeCostAsync` | GET | `/v3.0/realtime_cost/get` | `account_id` / `level` |

- **权限**：四页「所属权限」均为官方原文 `account_management`。
- **本域一个 `user_token` 都不带**：四页均**没有**「特定请求参数」表。守卫用**反射面反证**
  （四支方法的 Query 参数名集合里出现 `user_token` 即红）——「顺手给转账补一个凭据」是违规。
- 金额单位一律是**分**；`funds/get` 的 `realtime_cost` 字段是今日实时消耗、`bill_deposit_amount` 是锁定金额。

## 逐页枚举集合不得跨页套用（本域最大陷阱）

### `fund_type`：转账页 9 支、流水页 36 支

9 支集是 36 支集的**真子集**，流水页独有 27 支（赠金细分 / 负债 / 游戏与短剧专项等）。
用转账页集合调流水页会被官方拒绝，反之亦然 ⇒ **本域不建公共枚举**，两支集合各自只写在所属端点的文档面上：

| 集合 | 大小 | 登记位置 |
|---|---|---|
| `fund_transfer/add` | **9** | 请求 DTO `AdsFundTransferAddRequest` 的 `fund_type` 属性 XML（守卫按锚点 `<c>fund_type</c>，必填，<c>enum</c>；可选值` 抽取） |
| `fund_statements_detailed/get` | **36** | 接口 `GetFundStatementsDetailedAsync` 参数 XML（锚点「本页可选值 **36** 支」） |

守卫 `FundTypeSets_ShouldStayTwoDistinctPerPageCollections` 断言：两支集合逐字相等、9 支 ⊆ 36 支、差集恰 27。
⇒ 改动任一页集合必须**同批**改对应文档面的锚点，锚点被改成散文即红。

### `realtime_cost/get` 的 `level`：三值**不带** `REPORT_LEVEL_` 前缀

本页取值为 `{ ADVERTISER, ADGROUP, ADTOTAL }`，与 `daily_reports/get` 的 `REPORT_LEVEL_ADVERTISER` 等
是**两套拼写、零交集** ⇒ 把报表拼写套到本页会被官方拒绝。
守卫 `RealtimeCostLevelValues_ShouldStayPageLocal` 甚至断言整个 funds 接口文件里 `REPORT_LEVEL_` **只出现一处**
（那一处必须是「与报表页拼写不同」的警示句本身）。

## `realtime_cost/get` 的条件必填与「无意义字段」

| 参数 | 事实 |
|---|---|
| `date`（`YYYY-MM-DD`、10 字节） | 官方原文「只支持**今天**的数据查询，当 `level=ADTOTAL` 时不需要传该值」⇒ 虽标 `*` 仍建为**可空**，条件必填由应答表达 |
| `filtering`（数组 1–**2**） | 「当 `level=ADGROUP` 或 `ADTOTAL` 时该字段必填；不传或传空视为无限制」；`field` 仅 `{ adgroup_id }`、`operator` 仅 `{ EQUALS }` |
| `page` / `page_size` | 1–99999 / 1–100 |
| `level=ADVERTISER` 时 | 条目 `adgroup_id` 官方返回 `0` 且原文标注「**无意义**」⇒ 不得据此判「数据缺失」 |

`filtering` 值为 JSON 数组字符串，构造入口唯一 `AdsQueryJson.Filtering`。

## 转账（`fund_transfer/add`）的六条官方事实

1. **金额区间**：`amount` 单位为分，最小 **5,000**、最大 **2,000,000,000**。实际可转金额还取决于账户余额与冻结
   （SDK 不可见）⇒ 不做本地拦截，越界由应答 `code` 表达。
2. **转账类型 × 财务自理标签**（官方原文搭配，勿弱化）：

   | `transfer_type` | 方向与前置标签 |
   |---|---|
   | `AGENCY_TO_ADVERTISER` | 一代服务商 → 子客账户，或二代 → 二代子客（二代子客须「支持转入转出或仅转入」） |
   | `ADVERTISER_TO_AGENCY` | 子客 → 一代/二代服务商（二代子客须「支持转入转出」） |
   | `AGENCY_TO_SUB_ADVERTISER` | 一代 → 二代子客（须「禁止转入转出」） |
   | `SUB_ADVERTISER_TO_AGENCY` | 二代子客 → 一代（须「禁止转入转出或仅转入」） |
   | `AGENCY_TO_SUB_AGENCY` / `SUB_AGENCY_TO_AGENCY` | 一代 ↔ 二代（二代须「支持转入转出或仅转入」） |

   标签状态在官方侧、SDK 读不到 ⇒ 搭配错误由应答 `code` 表达。
3. **幂等靠 `external_bill_no`**：同一客户下唯一且**不支持大写**；重复提交时应答 `is_repeated == true`。
   本域**没有** `X-Request-Id` 请求头表 ⇒ 幂等只能靠这个业务键。
4. **「只查不转」两个开关**：`pre_fetch_amount = 1` 只取 `recommend_amount`、`pre_check = 1` 只回 `check_result`，
   两者都**不进行实际转账**。预校验前先想清楚这两个值。
5. `transfer_try_best` / `high_frequency_transfer` **只对特定 `transfer_type` 生效**（逐支语义见请求 DTO 的属性 XML）。
6. **预校验明细的错误描述字段官方拼写是 `messageCn`（camelCase）**，与信封的 `message_cn` **不同构**
   ⇒ DTO 按原文照录，不要「统一命名」。

## 流水明细（`fund_statements_detailed/get`）

- `date_range` 是 **JSON 对象字符串**（官方 curl 写成单个键值对），构造入口唯一 `AdsQueryJson.DateRange`；
  `start_date` / `end_date` 均 `YYYY-MM-DD`、10 字节；官方原文「支持**两年内**的数据查询」。
- `account_id` 官方口径「含代理商和账户 id」——**与 `qualifications` 域的「不支持代理商 id」口径相反**，两域勿互套。
- `page`（1–99999）与 `primary_key`（「上一页最后一条记录的 `primary_key`」，首页不填）**并存**：
  官方未说明两者如何配套 ⇒ SDK 不预判、原样透传，以应答为准。

## 应答形态

| 端点 | `data` | 分页 |
|---|---|---|
| `funds/get` | 资金账户列表 | **无分页元信息**（一次返回）；`effect_funds[]` 按有效期逐条展开（`effect_date` / `expire_date` 为 `YYYY-MM-DD`） |
| `fund_transfer/add` | 转账结果（`is_repeated` / `recommend_amount` / `check_result`） | — |
| `fund_statements_detailed/get` | 流水列表 | `page_info` |
| `realtime_cost/get` | 实时消耗列表 | `page_info` |

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddFundsApi());

var today = DateTime.Now.ToString("yyyy-MM-dd");   // 官方口径 GMT+8，且本页只支持今天
var resp = await funds.GetRealtimeCostAsync(
    accountId: 1234567890,
    level: "ADVERTISER",                            // 不带 REPORT_LEVEL_ 前缀
    date: today,
    cancellationToken: ct);

WechatAdsException.ThrowIfFailed(resp, "/v3.0/realtime_cost/get");
```

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数类型 / 区间 / 条件必填原文 | [IWechatAdsFundsService.cs](./IWechatAdsFundsService.cs) 的 XML 注释 |
| 路由 / 动词 / 计数（恰 4）/ 无 `user_token` | `FundsEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `FundsRoutes` |
| `fund_type` 两支集合（9 / 36） | `FundTypeSets_ShouldStayTwoDistinctPerPageCollections` + `TransferFundTypes` / `StatementFundTypes` |
| `realtime_cost` 的 `level` 三值 | `RealtimeCostLevelValues_ShouldStayPageLocal` |
| 转账请求体逐字段语义 | `AdsFundTransferAddRequest` 的属性 XML（`.DataModels/Funds/`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{funds/get,fund_transfer/add,fund_statements_detailed/get,realtime_cost/get}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档 URL + 区间 / 时限 / 条件必填，**同批**更新 `FundsRoutes` 与计数断言。
2. 改任一支 `fund_type` 集合 → 只改所属端的文档面，并保证守卫锚点文本原样在场；**不得**抽成公共枚举。
3. 新增 Query 凭据参数名 → 「补脱敏词表」或「登记带追踪号的自过期豁免」二选一（`ADS-B5`）。
4. **不要**给本域任何端点加 `user_token`；**不要**把报表页的 `level` 拼写搬进 `realtime_cost`。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
