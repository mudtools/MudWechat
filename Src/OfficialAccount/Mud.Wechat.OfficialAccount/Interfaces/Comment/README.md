# Comment 域 · 留言管理（文章评论）

微信公众号 / 服务号「留言管理」域：**8 支端点**，全部 POST（`comment/{open,close,list,markelect,unmarkelect,delete}`
+ `comment/reply/{add,delete}`）。
接口 [IMpCommentService.cs](./IMpCommentService.cs)，注册组 `Comment`，
模块枚举 `MpModule.Comment`（`AddCommentApi()` ⇒ 生成入口 `AddCommentWebApiHttpClient()`）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpContentContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpContentContractGuards.cs)
> （`CT1` / `CT5` / `CT6` / `CT7` / `CT8`）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/Comment/](../../../Mud.Wechat.OfficialAccount/Comment/)）。
> 未留档项一律进文末「已知留档缺口」，不引用外部文档、不编造守卫号。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `OpenCommentAsync` | POST | `/cgi-bin/comment/open` | Query 注入 `access_token`（MUD005） | 请求体 `MpCommentOpenRequest` |
| 2 | `CloseCommentAsync` | POST | `/cgi-bin/comment/close` | 同上 | 与 open **共用** `MpCommentOpenRequest` |
| 3 | `ListCommentsAsync` | POST | `/cgi-bin/comment/list` | 同上 | 唯一带分页的查询端点 |
| 4 | `MarkCommentElectAsync` | POST | `/cgi-bin/comment/markelect` | 同上 | 请求体 `MpCommentRefRequest` |
| 5 | `UnmarkCommentElectAsync` | POST | `/cgi-bin/comment/unmarkelect` | 同上 | 与 4/6/8 **共用** `MpCommentRefRequest` |
| 6 | `DeleteCommentAsync` | POST | `/cgi-bin/comment/delete` | 同上 | 同上 |
| 7 | `ReplyCommentAsync` | POST | `/cgi-bin/comment/reply/add` | 同上 | 定位三字段 + `content` |
| 8 | `DeleteCommentReplyAsync` | POST | `/cgi-bin/comment/reply/delete` | 同上 | 共用 `MpCommentRefRequest` |

**认证与账号面**：适用范围「公众号 / 服务号 —— **仅认证**」，并额外要求**留言功能权限**
（官方原文「公众号需具备留言功能权限」）。⇒ 比 Basic / AutoReply 多一道账号配置前置。

**第三方代调用面**：8 页均支持代调用，**权限集 id = 7**；按 M0-R3 裁决**只在 XML 注释里记录**，
不建模第三方令牌形态（与 Basic 的 M0 边界同一口径）。

**全部 8 支都带请求体**（`CT1` 断言本域无 GET）⇒ 不存在「POST 当查询用之外还有裸 GET」的形态；
唯一的查询面是 `comment/list`，它也是 POST。

## 寻址形态：`msg_data_id` + `index`，不是 `article_id`

8 支端点都用「群发返回的数据 ID `msg_data_id`（`long`）+ 多图文序号 `index`」定位文章：
- `index` **从 0 开始**；**不传即视为第一篇**（remarks 原文「不带默认第一篇」）。
- 该形制是**逐页核验对方案文档预判的修正**——早期预判的 `article_id` 形态不成立（`CT5` 锁定）。
⇒ 本域与 FreePublish / Draft 域的 `article_id` 体系**不通用**，跨域封装里不要共用「文章 id」参数。

## 共享请求 DTO：三型分工（`CT5`，N4 裁决）

| 请求 DTO | 官方字段 | 服务于 |
|---|---|---|
| [MpCommentOpenRequest](../../../Mud.Wechat.OfficialAccount/Comment/MpCommentOpenRequest.cs) | `msg_data_id` / `index` | open、close |
| [MpCommentRefRequest](../../../Mud.Wechat.OfficialAccount/Comment/MpCommentRefRequest.cs) | `msg_data_id` / `index` / `user_comment_id` | markelect、unmarkelect、delete、reply/delete |
| [MpCommentReplyAddRequest](../../../Mud.Wechat.OfficialAccount/Comment/MpCommentReplyAddRequest.cs) | 上三字段 + `content` | reply/add |
| [MpCommentListRequest](../../../Mud.Wechat.OfficialAccount/Comment/MpCommentListRequest.cs) | `msg_data_id` / `index` / `begin` / `count` / `type` | list |

**「扁平三字段」是一条裁决、不是偷懒**：`CT5` 显式断言 `MpCommentRefRequest` **不得包含**
嵌套 `user_comment` 对象——方案文档预判的嵌套形态在官方页不存在。
⇒ 遇到「把评论 id 包一层对象更规范」的提议要挡下来，那会与官方回包不同构。

## `list` 的分页与上限：`count >= 50` 会被拒绝

| 参数 | 事实（照官方原文） |
|---|---|
| `begin` | 必填，偏移量（非游标） |
| `count` | 必填，「获取数目（**>=50 会被拒绝**）」⇒ 单次上限语义由错误码 `88010` 表达，官方描述原文拼写为「**cout**」，照录不修正 |
| `type` | 必填：`0` 普通评论 & 精选评论 / `1` 普通评论 / `2` 精选评论（官方原文三值） |
| 应答 | `total`（总数）+ `comment[]`（**注意键名是单数 `comment`**，属性 `Comments`） |

- **没有 `page_info`、没有游标**：分页靠 `begin` + `count` 与应答 `total` 自行推进，
  「是否还有下一页」由 `begin + count < total` 判断（本 SDK 不做本地聚合与翻页编排）。
- 评论条目字段：`user_comment_id`、`create_time`、`content`、`comment_type`（`int?`）、
  `openid`、`reply`（`{content, create_time}`，可空）——`openid` 是**用户标识**，
  不得写入日志、遥测或异常消息。

## 逐端点错误码：各页表长得不一样（守卫 `CT8` 锁定常量）

| 端点 | 本仓留档的官方错误码 |
|---|---|
| `comment/open` | `-1`、`40001`、`45009`、`88000`（没有留言权限）、`88001`（图文不存在）、`88002`（文章存在敏感信息） |
| `comment/close` | 官方页**仅列** `-1`、`40001`——与 open 对偶但**明显更短**，remarks 记为「疑官方未列全，照录」 |
| `comment/list` | `-1`、`40001`、`45009`、`88000`、`88010`（count range error，官方原文拼写「cout」） |
| `comment/markelect` | `45009`、`88000`、`88001`、`88003`（精选评论数已达上限）、`88004`（已被用户删除，无法精选）、`88008`（该评论不存在） |
| `comment/unmarkelect` | `45009`、`88000`、`88001`、`88008` |
| `comment/delete` | `45009`、`88000`、`88001`、`88008` |
| `comment/reply/add` | `45009`、`88000`、`88001`、`88005`（已经回复过了）、`88007`（回复超过长度限制或为空——官方文案「或为 0」照录）、`88008` |
| `comment/reply/delete` | `45009`、`87009`（invalid signature——**本接口无签名参数，疑官方全局码表残留，照录**）、`88000`、`88001`、`88008` |

⇒ 不要用一张「本域错误码表」套全部 8 页；`88002` 只在 open 页出现、`88003`/`88004` 只在 markelect 页出现，
这些差异是逐页事实。`88000` 属**账号配置态**问题（无留言权限），不是调用错误——SDK 不做本地拦截。

## 精选与回复的配额语义

- 精选数量存在官方上限（`88003`），**具体数值本仓未留档**（见缺口）。
- 回复一次性：同一评论重复回复返回 `88005`「已经回复过了」⇒ 幂等性由官方表达，SDK 不做本地去重。
- 回复内容长度受限（`88007`），**限制值本仓未留档**；官方文案「超过长度限制或为空（或为 0）」照录。
- 7 支写端点（open / close / markelect / unmarkelect / delete / reply/add / reply/delete）的应答都是
  `MpResponse`（仅 `errcode` / `errmsg`）⇒ **没有异步回执事件**
  （与 Mass 的 `MASSSENDJOBFINISH`、Template 的 `TEMPLATESENDJOBFINISH` 族不同，本域同步生效）。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddCommentApi());

// 注入 IMpCommentService（注册组 Comment）
// 1) 打开某篇群发文章的留言（msg_data_id 来自群发返回，index 从 0 开始）
await comment.OpenCommentAsync(new MpCommentOpenRequest
{
    MsgDataId = msgDataId,
    Index = 0,                                 // 不传即官方默认第一篇
}, ct);

// 2) 分页拉取评论：count 必须 < 50，type 0 全部 / 1 普通 / 2 精选
var begin = 0;
var page = await comment.ListCommentsAsync(new MpCommentListRequest
{
    MsgDataId = msgDataId,
    Index = 0,
    Begin = begin,
    Count = 40,                                // >=50 官方直接拒绝（88010）
    Type = 0,
}, ct);
MpException.ThrowIfFailed(page, "/cgi-bin/comment/list");

foreach (MpComment item in page.Comments ?? new List<MpComment>())      // 官方键名是单数 comment
{
    Console.WriteLine($"#{item.UserCommentId} type={item.CommentType} replied={item.Reply is not null}");
    // item.OpenId 属用户标识：不得写日志、遥测或异常消息
}
// 无 page_info：靠请求侧 begin/count 与应答 total 自行推进
var hasMore = page.Comments is { Count: > 0 } c && begin + c.Count < page.Total;

// 3) 回复一条评论（重复回复官方报 88005）
await comment.ReplyCommentAsync(new MpCommentReplyAddRequest
{
    MsgDataId = msgDataId,
    Index = 0,
    UserCommentId = 1234567,
    Content = "感谢关注",
}, ct);

// 4) 取消精选：与 markelect/delete 共用同一个请求 DTO
await comment.UnmarkCommentElectAsync(new MpCommentRefRequest
{
    MsgDataId = msgDataId,
    Index = 0,
    UserCommentId = 1234567,
}, ct);
```

**判错只有一层**（`errcode` / `errmsg`）。本域 7 支写端点**没有异步回执**，
与 Mass 的 `MASSSENDJOBFINISH` / Template 的 `TEMPLATESENDJOBFINISH` 形态不同。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **本域无独立守卫文件** | 约束散落于 `MpContentContractGuards.cs`（`CT1` 路由与「全 POST」、`CT5` 寻址与共用 DTO 裁决、`CT6` AOT 登记、`CT7` 注册三段式、`CT8` 错误码）与 `QT1` 白名单 | 若与 Draft / FreePublish / ProductCard 的断言继续膨胀，评估拆出 `MpCommentContractGuards.cs`（拆文件须同批改本文与包 README 的守卫前缀表） |
| 精选评论数上限、回复长度上限的具体数值 | 只有错误码（`88003` / `88007`），**数值未逐项核验** | 核验后补 remarks 与守卫常量；核验前不写数字 |
| 官方对 `comment/close` 的错误码表是否完整 | remarks 记「疑官方未列全，照录」 | 复核该页；不要从 open 页借码 |
| `reply/delete` 页的 `87009` | 本接口无签名参数，疑官方全局码表残留，**照录不解读** | 复核后决定是否从守卫常量中剔除 |
| 多图文 `index` 的上界 | 本仓**未留档**（只记「从 0 开始、默认第一篇」） | 核验官方图文族上限后补齐 |
| 第三方令牌（`authorizer_access_token`）形态 | 按 M0-R3 只在 XML 记录权限集 7，不建模 | 落地须走「加形态 + 同批改守卫」 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文、仅认证 + 留言权限、寻址形制核验修正 | [IMpCommentService.cs](./IMpCommentService.cs) 的 XML 注释 |
| 8 条路由与「本域全 POST」（草稿 6 / 发布 5 / 留言 8 三域同守卫） | `CT1` = `ContentEndpoints_ShouldMatchOfficialRoutes` |
| 寻址形态 + 三型共用 DTO 裁决 + 「`MpCommentRefRequest` 不得含 `user_comment`」 | `CT5` = `CommentShapes_ShouldLockAddressingAndSharedDtos` |
| DTO 全量登记进 `CommentJsonContext`（`AssertContextGroup(..., "Mud.Wechat.OfficialAccount.DataModels.Comment", CommentJsonContext.Default, "Comment", 7, ...)`） | `CT6` = `ContentDataModels_ShouldBeRegisteredInJsonContexts` |
| `MpModule.Comment` + `AddCommentApi()` + Query 令牌白名单包含本接口 | `CT7` = `ContentModules_ShouldBeRegisteredAndWhitelisted` |
| `88000`/`88001`/`88002`/`88003`/`88004`/`88005`/`88007`/`88008`/`88010`/`87009` 常量 | `CT8` = `ContentErrorCodes_ShouldMatchVerifiedOfficialValues` |
| 全量 Query 令牌白名单（本域注释口径「8 端点，仅认证 + 留言权限」） | `QT1`（`MpQueryTokenWhitelistGuard.cs`） |
| DTO 字段与 JSON 名 | [DataModels/Comment/](../../../Mud.Wechat.OfficialAccount/Comment/)（7 个文件） |
| 生成入口与模块枚举、上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `CT1` 的留言路由表与计数（8）、
   `QT1` 条目注释的端点数、以及 `MpRouteCountGuard`（`RC1`/`RC2`）与
   `Tests/ContractBaseline.cs` 的 `OfficialAccount.MainInterfaceRoutes`（259）。**计数以守卫为准**。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Comment`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 同批更新 `CT6` 的登记断言（7 个类型）。
3. 确认 `Generated/CommentJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.Comment` + `AddCommentApi()` + `CT7` + `QT1` 一起改。
5. **不要**把寻址换成 `article_id`；**不要**给 `MpCommentRefRequest` 加嵌套 `user_comment`；
   **不要**把 `count` 提到 50 及以上；**不要**把 close 页的错误码表「对齐」open 页；
   **不要**在本域引入异步回执语义。
6. 最后跑 `scripts/verify-build.ps1`，全绿才算完成。
