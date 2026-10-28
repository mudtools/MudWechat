# FreePublish 域 · 发布能力

微信公众号 / 服务号「内容 → 发布能力」域：**5 支端点**（提交发布 / 查发布状态 / 删除发布 / 按 article_id 取图文 / 分页拉取已发布列表）。
接口 [IMpFreePublishService.cs](./IMpFreePublishService.cs)，注册组 `FreePublish`，
模块枚举 `MpModule.FreePublish`（[MpModule.cs](../../Extensions/MpModule.cs)；`AddFreePublishApi()` ⇒ 生成入口
`AddFreePublishWebApiHttpClient()`，见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpContentContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpContentContractGuards.cs)
> （`CT1`/`CT3`/`CT6`/`CT7`/`CT8`，发布能力与草稿/商品卡片/留言共用这一支内容域守卫）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）、
> [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`）
> 与本仓 DTO（[DataModels/FreePublish/](../../../Mud.Wechat.OfficialAccount/FreePublish/)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」，正文不引用外部官方文档表述。

## 端点速览

5 条路由逐条钉在 `CT1` 的 `FreePublishRoutes`（守卫号 `CT1`，含「本域全 POST」断言）；
本域在 `QT1` 白名单登记为「`IMpFreePublishService`（5 端点，仅认证）」。

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `SubmitAsync` | POST | `/cgi-bin/freepublish/submit` | Query 注入 `access_token`（MUD005） | 入参是**草稿** `media_id`；返回 `publish_id` |
| 2 | `GetPublishStatusAsync` | POST | `/cgi-bin/freepublish/get` | 同上 | `publish_status` 为**标量** 0~6 |
| 3 | `DeletePublishAsync` | POST | `/cgi-bin/freepublish/delete` | 同上 | 不可逆；`index` 不填或 0 ⇒ 删全部 |
| 4 | `GetArticleAsync` | POST | `/cgi-bin/freepublish/getarticle` | 同上 | 按 `article_id` 寻址，含 `is_deleted` |
| 5 | `BatchGetPublishedAsync` | POST | `/cgi-bin/freepublish/batchget` | 同上 | `count` **1~20**；条目键 `article_id` |

**适用范围**：公众号 / 服务号 —— **仅认证**（与本线的 Draft 域「非仅认证」不同）。
**未建模账号类型本地闸**：账号面不符由官方 `48001`（api 功能未授权）表达。
**第三方代调用**：官方权限集 7，按 `M0-R3` 裁决只记在接口 XML remarks，SDK 无代理逻辑。

## 提交成功 ≠ 发布完成（本域头号陷阱）

接口 remarks 原文留档：「此时只意味着发布任务**提交成功**，并不意味着此时发布已经完成」。
⇒ `SubmitAsync` 返回 `errcode = 0` 与 `publish_id` **不代表文章已上线**。

- 发布结果经 `PUBLISHJOBFINISH` 事件推送；
- 官方 API 页**仅文字提及事件名**、无独立 XML 报文页 ⇒ 本仓**暂未建模**该事件（见接口 remarks 与 `.docs` 实施记录），
  事件落地属回调包 `Mud.Wechat.OfficialAccount.Callback`，不在本域；
- 因此同步侧只能靠 `GetPublishStatusAsync` 轮询 `publish_status`，且它是**中间态**语义。

## `publish_status` 是标量，不是数组

`CT3` 把 `publish_status` 钉为标量。取值（接口 remarks 逐值留档）：

| 值 | 语义 |
|---|---|
| 0 | 发布成功 |
| 1 | 发布中 |
| 2 | 原创失败 |
| 3 | 常规失败 |
| 4 | 平台审核不通过 |
| 5 | 成功后用户删除所有文章 |
| 6 | 成功后系统封禁所有文章 |

仓内早期方案文档曾预判「数组」形态，**已被逐页核验推翻并在 `CT3` 锁死**；
DTO 侧对应 `MpFreePublishGetResponse.PublishStatus`（`int?`）与失败下标 `FailIdx`（`List<long>?`）。

## 三套 id 并存：`publish_id` → `article_id`，与草稿 `media_id`、群发 `msg_data_id` 划清

| 键 | 出处 | 用途 |
|---|---|---|
| `media_id` | Draft 域 `draft/add` | `freepublish/submit` 的**输入** |
| `publish_id` | `SubmitAsync` 返回 | `freepublish/get` 的**输入**（发布任务定位） |
| `article_id` | `freepublish/get` / `batchget` 返回 | `delete` / `getarticle` 的**输入**（文章定位） |
| `msg_data_id` | 群发域 / 本页 submit 返回字段 | 群发任务键；`MpFreePublishSubmitResponse.MsgDataId` 官方返回表有、示例缺失，**矛盾照录** |

## `batchget`：`count` 1~20，条目键是 `article_id` 而非 `item_id`

- 官方原文「取值在 1 到 20 之间」，**非** ≤100（接口 remarks 标注「已双验」）；
- 响应外层是 `total_count` / `item_count` / `item`，`item[]` 内的定位键为 `article_id`
  （`CT3` 同时断言响应字段集 `NotContain("item_id")`）；
- `no_content` 为 `int?` 开关（不强转 bool）。

## 删除 / 覆盖语义：`delete` 是「整篇或指定下标」，没有「更新首页」端点

- `DeletePublishAsync`：`article_id` 必填；`index` **从 1 开始**（第一篇 = 1），
  **不填或填 0 ⇒ 删除该 article 下全部文章**，操作**不可逆**；
- 「重新发布」在本域只能通过对新的草稿 `media_id` 再次 `SubmitAsync` 实现——
  本域**不存在**「更新发布内容 / 重写首页文章」这类端点（5 条路由由 `CT1` 穷举）；
  任务书提到的「update / 覆盖首页」在本仓代码里**没有对应实现**，不得据其描述本文口径。

## 官方文档矛盾（照录，不「修正」）

| 端点 | 矛盾 | 仓内处置 |
|---|---|---|
| `submit` | `msg_data_id` 在返回表有、示例里没有 | DTO 保留 `MsgDataId` 属性 |
| `getarticle` | 请求体说明误写「要获取的**草稿**的 article_id」 | 参数按 `article_id` 建模 |
| `getarticle` | 字段表无 `show_cover_pic` 行、示例却含该字段 | 未建模该字段（见缺口） |
| `delete`/`getarticle`/`batchget` | 官方接口英文名大小写混杂（`freepublishDelete`、`freepublishGetarticle`） | 方法名按 SDK 惯例、路由按官方路径 |

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddFreePublishApi());

// 注入 IMpFreePublishService（注册组 FreePublish）
// 1) 提交发布：入参是草稿 media_id；返回只代表「提交成功」
var submit = await freePublish.SubmitAsync(new MpFreePublishSubmitRequest
{
    MediaId = draftMediaId,
}, ct);
string publishId = submit.PublishId ?? string.Empty;

// 2) 轮询发布状态：publish_status 是标量 0~6，1 = 发布中
var status = await freePublish.GetPublishStatusAsync(new MpFreePublishGetRequest
{
    PublishId = publishId,
}, ct);
int publishStatus = status.PublishStatus ?? -1;          // -1 仅表示本仓未取到值
string articleId = status.ArticleId ?? string.Empty;    // 成功后才有，文章定位键
List<long> failIdx = status.FailIdx ?? new List<long>();

// 3) 分页拉取已发布列表：count 只能 1~20
var page = await freePublish.BatchGetPublishedAsync(new MpFreePublishBatchGetRequest
{
    Offset = 0,
    Count = 20,
    NoContent = 1,
}, ct);
List<MpFreePublishListItem> items = page.Items ?? new List<MpFreePublishListItem>();

// 4) 取已发布图文（含 is_deleted 标记）
var article = await freePublish.GetArticleAsync(new MpFreePublishGetArticleRequest
{
    ArticleId = articleId,
}, ct);

// 5) 删除：index 省略或 0 ⇒ 删除该文章全部条目，不可逆
await freePublish.DeletePublishAsync(new MpFreePublishDeleteRequest
{
    ArticleId = articleId,
    Index = 1,
}, ct);
```

**判错只有一层**：`MpResponse` 的 `errcode` / `errmsg`（`delete` 直接返回 `MpResponse`）。
**发布结果通知不在本域**：`PUBLISHJOBFINISH` 属回调包 `Mud.Wechat.OfficialAccount.Callback`。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域**无独立守卫文件** | 断言住在 `CT` 系列（`CT1` 路由、`CT3` 形态、`CT6` 计数、`CT7` 注册、`CT8` 错误码），与草稿/商品卡片/留言同文件 | 拆域需同批改 `CT` 守卫 remarks 的端点构成 |
| `PUBLISHJOBFINISH` 事件报文 | 官方 API 页仅文字提及、无独立 XML 页 ⇒ **未建模** | 核验报文后落回调包，勿落本域 |
| 任务书提到的「更新发布 / 重写首页」端点 | 本仓 5 条路由**没有**该端点，官方页是否另有能力**未核验** | 保持缺口，不得凭空加方法 |
| `show_cover_pic`（getarticle 示例字段） | 字段表无、示例有 ⇒ 未入 DTO | 核验官方字段表后再决定建模 |
| 已发布文章总数上限 / 发布频次 | 仓内**无留档数值** | 未逐项核验，勿做本地预校验 |
| 错误码全表 | `CT8` 只钉已核验的 53404~53406/53503~53505/88000~88010/87009/40114/41039/45166/10170001~3 | 需更全时先逐页核验再补 `CT8` |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、「提交 ≠ 完成」、`publish_status` 七值、id 形态、count 1~20、第三方权限集 7 | [IMpFreePublishService.cs](./IMpFreePublishService.cs) 的 XML 注释 |
| 5 条路由 + 全 POST 断言 | `CT1` = `ContentEndpoints_ShouldMatchOfficialRoutes` |
| `publish_id`/`article_id`/`publish_status`/`article_detail`/`fail_idx`/`item` 键名、`NotContain("item_id")`、count 1~20 | `CT3` = `FreePublishShapes_ShouldLockVerifiedCorrections` |
| DTO 全量登记进 `FreePublishJsonContext`（`expectedCount = 14`） | `CT6` = `ContentDataModels_ShouldBeRegisteredInJsonContexts` |
| `MpModule.FreePublish` + `AddFreePublishApi()` + 白名单包含 | `CT7` = `ContentModules_ShouldBeRegisteredAndWhitelisted` |
| 已核验错误码常量 | `CT8` = `ContentErrorCodes_ShouldMatchVerifiedOfficialValues` |
| Query 令牌全集 / 路由去重计数 | `QT1`、`RC1`/`RC2` |
| 14 个 DTO（`SerializerClassName = "FreePublish"`） | [DataModels/FreePublish/](../../../Mud.Wechat.OfficialAccount/FreePublish/) |
| 生成入口 / 模块枚举 / 上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |
| 本线总览（**统计已过期**，不可当权威） | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与「提交 ≠ 完成」类前置语义，**同批**更新 `CT1` 的
   `FreePublishRoutes` 与守卫 remarks 的端点构成（现为 6 + 5 + 1 + 8 = 20）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `FreePublish`，见
   [AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1)）→ 跑
   [GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) → 同批更新 `CT6` 的 `expectedCount`（14）。
3. 确认 `Generated/FreePublishJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.FreePublish` + `AddFreePublishApi()` + `CT7` + `QT1` + `RC1`/`RC2` +
   [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线一起改；**计数以守卫为准**。
5. 建模 `PUBLISHJOBFINISH` → 属回调包改动，需同批改回调事件表与本域 remarks 的「未建模」表述，
   **不要**把事件 DTO 塞进 `DataModels/FreePublish/`（会把 `CT6` 的 14 顶歪）。
6. **不要**把 `publish_status` 改成数组；**不要**把 batchget 条目键写成 `item_id`；
   **不要**把 `count` 放宽到 100；**不要**把「提交成功」当「已发布」向调用方回报。
7. 最后跑 [verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
