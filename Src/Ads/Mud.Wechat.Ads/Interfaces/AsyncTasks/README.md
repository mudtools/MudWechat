# AsyncTasks 域 · 异步任务

腾讯广告 Marketing API v3.0「异步任务」域：**2 支端点**（`async_tasks/add|get`）。
接口 [IWechatAdsAsyncTaskService.cs](./IWechatAdsAsyncTaskService.cs)，注册组 `AsyncTasks`，
模块枚举 `AdsModule.AsyncTasks`（`AddAsyncTasksApi()`）。逐页核验 2026-10-11（层级经 DOM `level-*` 树核验）。

> 事实来源限定：逐参数原文以接口文件 XML 注释为准；路由 / 计数以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 方法 | 动词 | 路由 | 权限 | `user_token` |
|---|---|---|---|---|
| `AddAsync` | POST | `/v3.0/async_tasks/add` | `ads_management` | 无 |
| `GetAsync` | GET | `/v3.0/async_tasks/get` | `ads_management` | 无 |

- **两页均未另列 `user_token`** ⇒ 一个都不带；由
  `RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` 反射面反向断言。
- 全局参数 `access_token` + `timestamp` + `nonce` 由 `AdsAuthorizationHandler` 成组注入（守卫 `ADS-B1`）。
- 计数钉成「恰 2」：官方目录树若新增任务族端点，须逐页核验后同批改 `AsyncTasksRoutes`。

## 双层失败语义：`result.code` 才是任务执行结果

`get` 应答的判定面有**两层**，与批量族（`adgroups/update_*`）同款：

| 层 | 键 | 含义 |
|---|---|---|
| 外层信封 | `code` / `message` / `message_cn` | 「查询请求被受理」，`ThrowIfFailed` 判的就是这一层 |
| 条目 `result` | `code` / `message` / `data` | **任务执行结果**，`0` = 任务跑成功；任务未完成时整个 `result` 缺省 |

- **`result` 层没有 `message_cn`**（官方本页 `result` 只有 `code`/`message`/`data` 三键，与外层信封不同构）；
  形态同报表域的 `async_reports/get` ⇒ 想拿中文描述只能读外层。
- `result.code` 建模为 `int?`（**可空是契约**）：写成非空会把「官方还没回该条 code」读成 `0` = 任务成功。
  只判外层 `code == 0` 就把任务当完成，是本域最典型的误用。

**类型分建（D5 裁决）**：本域产物类型是 `AdsAsyncTaskExecutionResult`，报表域 `async_reports/get`
的是 `AdsAsyncTaskResult`。两支**外层同构、`data` 形状完全不同**（任务产物列表 vs 报表）⇒
按域各建一份，不收敛成公共类型；跨域「复用」会让任一页改字段时另一页被动越出官方契约。

## `add`：三支互斥 spec 由 `task_type` 选支

必填 `account_id` / `task_name` / `task_type`；`task_spec` 内三支**互斥**，`task_type` 决定哪一支生效：

| `task_spec` 键（官方原文） | 生效类型 | 载荷要点 |
|---|---|---|
| `task_type_create_android_channel_package_spec` | 创建安卓渠道包 | 必填 `myapp_auth_key` / `android_app_id` / `android_channel_package_spec[]`（元素只有 `package_name` + `download_url`） |
| `task_type_update_android_channel_package_spec` | 更新安卓渠道包 | 同上，但元素多一支 `channel_package_id` ⇒ 官方两页不同构 ⇒ **另建** `AdsAndroidChannelPackageUpdateSpec`，不复用创建版 |
| `task_type_delete_creative_asset_spec` | 清理创意资产 | `component_id_list`（`long[]`）/ `image_id_list`（`string[]`）/ `video_id_list`（**`string[]`**）/ `component_clean_strategy` |

- **`task_type` 可选值未逐项核验**（官方页与 spec 键名一一对应，本仓不建枚举）⇒ 取值以官方页为准，
  填错由应答 `code` 表达，SDK 不本地拦截。
- **`myapp_auth_key` 是第三方授权 key**：它出现在请求体里 ⇒ 与凭据同类，**不得**写进日志、遥测或
  异常消息（对齐 §8 的凭据面纪律；守卫 `ADS-B5` 登记的是 Query 侧凭据，本键走 body）。
- `component_clean_strategy` 四支：`is_delete_creative_asset` / `is_unbind_dynamic_creatives` /
  `is_delete_incomplete_dynamic_creatives`（三支 `boolean`）+ `delete_related_creative_asset_type_list`
  （官方标 `array` 未细标元素类型 ⇒ 以字符串承载）。
- 应答 `data` **只回 `task_id`** ⇒ 建任务与取结果之间没有任何同步反馈面，轮询归调用方。

## `get`：`account_id` 必填，任务状态与产物

`GetAsync(accountId, filtering, page, pageSize)` —— `account_id` 官方必填；
`filtering` 是 JSON 数组字符串，入口唯一 `AdsQueryJson.Filtering`（**本页 `field` / `operator`
可选值未逐项核验**，取值域去官方页读，勿从邻域借）。

条目 `AdsAsyncTaskInfo`：`task_id` / `task_name` / `task_type` / `status` / `created_time` / `result`。

- **`status` 可选值未逐项核验** ⇒ 建模为 `string?`，**不要**写「等于某个常量才算完成」的判定；
  判定依据取 `result.code`（在场且为 `0`）比 `status` 字符串更可靠。
- `result.data` 四种产物列表按任务类型出现：`file_info_list`（`file_id` + `md5`）/
  `channel_package_info_list` / `union_channel_package_info_list` /
  `review_element_prereview_result_list`（预审核结果，其 `pre_review_details` 官方未细标结构 ⇒
  以 `List<JsonElement>` 开放承载）。
- 应答带 `page_info` ⇒ 本域分页形态与 Qualifications / TargetingTags 两域（**无分页**）不同。

## 与报表域异步链路的分工

| | `async_reports/*`（Reports 域） | `async_tasks/*`（本域） |
|---|---|---|
| 用途 | 拉大区间报表数据 | 渠道包批处理、创意资产清理等**非报表**任务 |
| 产物落地 | 文件（`file_id`，24 小时）+ 7 日任务记录 | `result.data` 内联返回四种列表 |
| `result` 类型 | `AdsAsyncTaskResult` | `AdsAsyncTaskExecutionResult` |
| 双层判定 | 有 | 有（同款） |

两条链路**不共用端点、不共用类型**；跨域「合并成一个异步任务族」会同时抹平两套 `data` 契约。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddAsyncTasksApi());

// 1) 建「清理创意资产」任务（task_type 与 spec 选支必须对应）
var created = await tasks.AddAsync(new AdsAsyncTaskAddRequest
{
    AccountId = 1234567890,
    TaskName = "清理 2025 素材",
    TaskType = "task_type_delete_creative_asset",  // 占位值：全集未逐项核验，须照官方页逐字填
    TaskSpec = new AdsAsyncTaskSpec
    {
        DeleteCreativeAsset = new AdsTaskDeleteCreativeAssetSpec
        {
            ImageIdList = ["img-1", "img-2"],   // 官方本页 image_id 是 string
            ComponentCleanStrategy = new AdsComponentCleanStrategy { IsUnbindDynamicCreatives = true },
        },
    },
}, ct);
WechatAdsException.ThrowIfFailed(created, "/v3.0/async_tasks/add");
var taskId = created.Data!.TaskId!.Value;

// 2) 轮询：外层判「查询被受理」，条目 result 判「任务执行结果」
var polled = await tasks.GetAsync(1234567890,
    AdsQueryJson.Filtering(new AdsFiltering { Field = "task_id", Operator = "EQUALS", Values = [taskId.ToString()] }),
    cancellationToken: ct);
WechatAdsException.ThrowIfFailed(polled, "/v3.0/async_tasks/get");

var item = polled.Data!.List!.FirstOrDefault();
bool taskSucceeded = item?.Result?.Code == 0;   // result 为 null = 任务未完成，不是失败也不是成功
```

**`result == null` 的三态**：外层失败 / 任务在跑 / 任务已完成 —— 只有第三种能读产物列表，
把 `null` 当成功会立刻在 `result.data` 上空集合。

## 官方逐页事实与未核验面（照录，不补猜测）

| 面 | 状态 |
|---|---|
| `task_type` 可选值全集 | **未逐项核验**（与 spec 键名一一对应） |
| `status` 可选值全集 | **未逐项核验** ⇒ `string` 承载 |
| `get` 页 `filtering` 的 `field` / `operator` 取值域 | **未逐项核验** |
| `result` 层键集 | 已核验：`{code, message, data}`，无 `message_cn` |
| `video_id_list` 元素类型 | 官方本页标 `string[]`（与 `videos/*` 页的 `video_id` 整数形态不一致，属官方自身不一致，照录） |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文 | `IWechatAdsAsyncTaskService.cs` 的 XML 注释 |
| 路由 / 计数（恰 2） | `AsyncTasksEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `AsyncTasksRoutes` |
| 双层判定的报文级锁定 | `AdsMaterialAndTaskJsonTests.AsyncTaskGetResponse_ShouldBindTwoLayerResult` |
| 三支互斥 spec 与选支表 | `AdsAsyncTaskSpec` remarks（`Src/Ads/Mud.Wechat.Ads.DataModels/AsyncTasks/`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{async_tasks/add,async_tasks/get}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `AsyncTasksRoutes` 与计数断言（2）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `AsyncTasks`）→ 跑两个脚本 →
   更新登记断言（守卫 `ADS-B6`）。
3. 补 `task_type` / `status` 的取值全集 → 先逐页核验官方页，再同批把本文「未核验」行改成留档事实；
   **不得**从邻域借枚举。
4. **不要**给本域加 `user_token`；**不要**把 `result.code` 改成非空；**不要**与报表域的
   `AdsAsyncTaskResult` 合并成公共类型。
5. **不要**把 `myapp_auth_key` 写进任何日志或异常消息。
6. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
