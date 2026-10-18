# Draft 域 · 草稿管理

微信公众号 / 服务号「内容 → 草稿管理」域：**6 支端点**（新建 / 修改 / 获取 / 删除 / 取总数 / 分页拉取）。
接口 [IMpDraftService.cs](./IMpDraftService.cs)，注册组 `Draft`，
模块枚举 `MpModule.Draft`（[MpModule.cs](../../Extensions/MpModule.cs)；`AddDraftApi()` ⇒ 生成入口
`AddDraftWebApiHttpClient()`，见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpContentContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpContentContractGuards.cs)
> （`CT1`~`CT8`，草稿管理 6 + 发布能力 5 + 商品卡片 1 + 留言管理 8 共用这一支内容域守卫）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）、
> [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`）
> 与本仓 DTO（[DataModels/Draft/](../../../Mud.Wechat.OfficialAccount.DataModels/Draft/)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」，正文不引用外部官方文档表述。

## 端点速览

路由与计数以守卫为准：6 条路由逐条钉在 `CT1` 的 `DraftRoutes`（守卫号 `CT1`），
本域在 `QT1` 白名单中登记为「`IMpDraftService`（6 端点；`draft/switch` 废弃不实现）」。

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddDraftAsync` | POST | `/cgi-bin/draft/add` | Query 注入 `access_token`（MUD005） | 请求体 `articles` 为**数组**；返回草稿 `media_id`（≤128 字符） |
| 2 | `UpdateDraftAsync` | POST | `/cgi-bin/draft/update` | 同上 | 同一键名 `articles` 在本页是**单对象**，须带 `index` |
| 3 | `GetDraftAsync` | POST | `/cgi-bin/draft/get` | 同上 | 官方查询类接口也走 POST，不是 GET |
| 4 | `DeleteDraftAsync` | POST | `/cgi-bin/draft/delete` | 同上 | 不可逆 |
| 5 | `GetDraftCountAsync` | **GET** | `/cgi-bin/draft/count` | 同上 | 本域唯一 GET（`CT1` 显式断言） |
| 6 | `BatchGetDraftsAsync` | POST | `/cgi-bin/draft/batchget` | 同上 | `count` 取值 **1~20** |

**适用范围**：公众号 ✔ / 服务号 ✔（**非仅认证**，与本线的 FreePublish / Kf* 域不同）。
本域不设账号类型本地闸，账号面不符由官方错误码表达。

**第三方代调用面**：6 页官方均支持代调用（权限集 11、100）。按 `M0-R3` 裁决，
该信息**只在接口 XML remarks 记录**，不建任何 SDK 侧代理逻辑，守卫里也没有代调用断言。

## `draft/switch`：官方已废弃 ⇒ 不实现，且守卫有反向断言

官方「草稿开关」接口已废弃，本域**未建模**。这不是「暂时没写」，而是被断言钉住的终态：

- `CT1` 对 `DraftRoutes` 之外的路由表做 `NotContain("/cgi-bin/draft/switch")`；
- `QT1` 白名单条目把「`draft/switch` 废弃不实现」写进注释，防止后来者补一条端点就顺手登记；
- 因此该路由不计入 `RC1`/`RC2`，也不在 [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 的基线里。

结论：**在册与否以守卫为准**——本域在册端点数是 6，不是官方历史上出现过的 7。
若要恢复该接口，必须先推翻 `CT1` 的反向断言（而不是加一个方法就收工）。

## 官方「查询」类接口照样走 POST

`draft/get`、`draft/batchget`、`draft/delete`、`draft/update` 全部是 `POST` + JSON 请求体
（`CT1` 对每条路由断言 `PostAttribute`）。若按 REST 直觉改成 GET，会在 Native AOT 与真实调用两层同时失败。

## `batchget` 的 `count`：1~20，不是「≤100」

接口 remarks 留档官方原文「取值在 1 到 20 之间」，`CT3` 亦把 `count` 的 1~20 口径锁在发布域同一形态上。
本域另两个分页键是 `offset`（`int`）与 `no_content`（`int?`，官方为 0/1 开关，SDK 不强转 bool）；
`no_content = 1` 表示**不返回 `content` 字段**（默认 0 正常返回）。

`draft/count` 与 `draft/batchget` 两支的官方错误码表**只列 `0` 且两列文案错位**（官方页面缺陷，remarks 照录）
⇒ 不要按这两页的表推断错误语义。

## 三套定位键族不能互相顶替

| 键 | 产生方 | 消费方 | 形态 |
|---|---|---|---|
| `media_id` | `draft/add`（草稿） | `draft/get/update/delete`、`freepublish/submit`、群发 `mpnews` | 永久素材 MediaID |
| `msg_data_id` | 群发（Mass 域） | `message/mass/get`、`message/mass/delete` | 群发任务定位键 |
| `article_id` | 发布（FreePublish 域） | `freepublish/get/delete/getarticle/batchget` | 已发布文章定位键 |

`MpDraftAddResponse.MediaId` ↔ `MpDraftGetRequest.MediaId` 是本域唯一闭环；
把 `msg_data_id` 或 `article_id` 传进 `draft/get` 属于跨键族误用（键族形态由 `CT2`/`CT3` 分别钉住）。

## 生命周期：草稿被群发或发布后即从草稿箱移除

接口 remarks 原文留档「上传到草稿箱中的素材被**群发或发布**后，该素材将从草稿箱中移除」
⇒ `media_id` 随之失效。**不要**把草稿 `media_id` 当长期主键存库；
需要长期引用时用发布后的 `article_id`（FreePublish 域）。

## `add` 是数组、`update` 是单对象（官方双形态，照录不合并）

`CT2` 锁定：`MpDraftAddRequest.Articles` 类型是 `List<MpDraftArticle>`，
`MpDraftUpdateRequest.Articles` 类型是**单个** `MpDraftArticle` 且必须带 `Index`。
两套请求体不共用类型是有意为之，「复用同一个 DTO」的清理动作会破坏官方形态。
同理 `MpDraftNewsItem`（草稿回读）与素材域的 `MpMaterialNewsItem` **不共用**，
因为官方两页回传字段集不一致。

## 正文图片必须先过素材接口（前置条件）

- `content` 内的图片 url **必须**来自「上传图文消息内的图片获取 URL」接口，即
  [IMpMediaService.UploadImageAsync](../Media/IMpMediaService.cs)（`UploadImageAsync`），外部图片 url 会被官方过滤；
- `thumb_media_id`：`article_type = news` 时**必填**，且必须是**永久**素材 MediaID（非临时素材）；
- `article_type = newspic`（图片消息）时 `image_info` 必填：最多 **20** 张、**首张为封面**；
  该形态下 `content` 仅支持纯文本与部分特殊功能标签（如商品，商品**≤ 50 个**）。

`title` ≤32 字且不要使用 Unicode 转义格式（`MpDraftArticle` 字段 remarks 留档）。

## 未留档的两个「上限」不写进契约

「草稿总数上限」「单篇图文 8 条上限」在核验过的官方页**均无表述**（后者只是第三方转述）。
本域 DTO 与守卫都不体现这两项，调用方若按这两个数字做本地预校验，属于把未核验口径当契约。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddDraftApi());

// 注入 IMpDraftService（注册组 Draft）
// 1) 前置：封面必须是永久素材、正文图片必须走 uploadimg 取 url
//    这里的 mediaId / 正文图片 url 由 IMpMediaService 取得，示例只引用变量名
var createResp = await draft.AddDraftAsync(new MpDraftAddRequest
{
    Articles =
    [
        new MpDraftArticle
        {
            Title = "秋季新品预告",
            Digest = "三句话看懂本次上新",
            Content = "<p>正文图片只能用 uploadimg 返回的 url</p>",
            ThumbMediaId = thumbMediaId,          // article_type=news 时必填（永久素材）
            ContentSourceUrl = "https://example.com/a",
        },
    ],
}, ct);
string draftMediaId = createResp.MediaId ?? string.Empty;   // ≤128 字符

// 2) 分页拉取：count 取值 1~20
var page = await draft.BatchGetDraftsAsync(new MpDraftBatchGetRequest
{
    Offset = 0,
    Count = 20,
    NoContent = 1,                                 // 不回正文，只要列表元信息
}, ct);
List<MpDraftListItem> items = page.Items ?? new List<MpDraftListItem>();

// 3) 改单篇：articles 是「单对象」+ index（不是数组）
await draft.UpdateDraftAsync(new MpDraftUpdateRequest
{
    MediaId = draftMediaId,
    Index = 0,
    Articles = new MpDraftArticle
    {
        Title = "秋季新品预告（改）",
        Content = "<p>修订后的正文</p>",
        ThumbMediaId = thumbMediaId,
    },
}, ct);

// 4) 取总数（本域唯一 GET）
var count = await draft.GetDraftCountAsync(ct);    // count.TotalCount

// 5) 删除不可逆（官方注意事项原文「此操作无法撤销，请谨慎操作」）
await draft.DeleteDraftAsync(new MpDraftDeleteRequest { MediaId = draftMediaId }, ct);
```

**判错只有一层**：`MpResponse` 的 `errcode` / `errmsg`（`add` 另带 `media_id`）。
本域**没有异步回执事件**：群发结果（`MASSSENDJOBFINISH`）与发布结果（`PUBLISHJOBFINISH`）
都归回调包 `Mud.Wechat.OfficialAccount.Callback`，不在本域。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域**无独立守卫文件** | 权威断言住在 `CT` 系列（与发布/商品卡片/留言共用 `MpContentContractGuards.cs`）；`CT2`/`CT6`/`CT7`/`CT8` 里的 Draft 分片即本域全部守卫面 | 拆域需同批改守卫类 remarks 的 20 端点构成 |
| `draft/add` 的 `articles` 单请求最大条数 | 官方页无表述，仓内未钉数值 | 未核验前不要做本地条数闸 |
| 「草稿总数上限」 | 仓内**明确不编造**（remarks 留档「官方无表述」） | 保持缺口，勿从第三方转述取数 |
| `MpErrorCodes` 中 `53404`/`53405`/`53406` 之外的 add 错误码 | `CT8` 只钉已核验的 40114/41039/45166/53404~53406/53503~53505/10170001~3/88000~88010/87009 | 需要更全错误表时先逐页核验再补 `CT8` |
| `no_content` 的官方语义（0/1 之外是否有其它取值） | 字段按 `int?` 建模，取值面未逐项核验 | 核验后补 remarks |
| `newspic` 的「部分特殊功能标签」清单 | remarks 只举「商品 ≤50」一例，全集**未逐项核验** | 不得据他页补全标签白名单 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、生命周期、双形态、账号适用性、第三方权限集 | [IMpDraftService.cs](./IMpDraftService.cs) 的 XML 注释 |
| 6 条路由 + 「仅 `draft/count` 为 GET」+ `draft/switch` 反向断言 | `CT1` = `ContentEndpoints_ShouldMatchOfficialRoutes` |
| add/update 双形态、`article_type`/`image_info`/`product_info` 字段名、草稿与素材 news 不共用 | `CT2` = `DraftShapes_ShouldLockArticleFieldsAndDualForms` |
| DTO 全量登记进 `DraftJsonContext`（`expectedCount = 19`） | `CT6` = `ContentDataModels_ShouldBeRegisteredInJsonContexts` |
| `MpModule.Draft` + `AddDraftApi()` + Query 令牌白名单包含 | `CT7` = `ContentModules_ShouldBeRegisteredAndWhitelisted` |
| 已核验错误码常量 | `CT8` = `ContentErrorCodes_ShouldMatchVerifiedOfficialValues` |
| Query 令牌全集（唯一持票人） | `QT1`；路由去重计数 `RC1`/`RC2` |
| 19 个 DTO（`SerializerClassName = "Draft"`） | [DataModels/Draft/](../../../Mud.Wechat.OfficialAccount.DataModels/Draft/) |
| 生成入口 / 模块枚举 / 上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |
| 本线总览（**统计已过期**，不可当权威） | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与前置条件，**同批**更新 `CT1` 的 `DraftRoutes` 与守卫 remarks
   的「6 + 5 + 1 + 8 = 20 端点」构成；**不要**为 `draft/switch` 补端点（`CT1` 有反向断言）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Draft`，见
   [AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1)）→ 跑
   [GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) → 同批更新 `CT6` 的 `expectedCount`（19）。
3. 确认 `Generated/DraftJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.Draft` + `AddDraftApi()` + `CT7` + `QT1` + `RC1`/`RC2` +
   [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线一起改；**计数以守卫为准**。
5. 动字段名 → 官方 JSON 键名由 `CT2` 持有，改 DTO 属性名可以、改 `JsonPropertyName` 必须先核验官方页。
6. **不要**引入 `MpAppConfig.AccountType` 或本域本地能力闸；**不要**把 `update` 的 `articles` 改成数组；
   **不要**假设草稿 `media_id` 在群发/发布后仍有效。
7. **不要**把发布 / 群发回执事件写进本域：它们属回调包 `Mud.Wechat.OfficialAccount.Callback`。
8. 最后跑 [verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
