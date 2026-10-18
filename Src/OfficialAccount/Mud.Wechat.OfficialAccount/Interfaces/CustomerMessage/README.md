# CustomerMessage 域 · 客服消息（发送 / 输入状态 / 聊天记录）

微信公众号 / 服务号「客服消息 → 客服消息」子分组：**3 支端点**（发送客服消息 + 客服输入状态 + 获取聊天记录）。
接口 [IMpCustomerMessageService.cs](./IMpCustomerMessageService.cs)，注册组 `CustomerMessage`，
模块枚举 `MpModule.CustomerMessage`（`AddCustomerMessageApi()` ⇒ 生成入口 `AddCustomerMessageWebApiHttpClient()`）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpCustomerMessageContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpCustomerMessageContractGuards.cs)
> （`CM1`~`CM9`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/CustomerMessage/](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `SendCustomMessageAsync` | POST | `/cgi-bin/message/custom/send` | Query 注入 `access_token`（MUD005） | 单接口 + 平铺分支字段，按 `msgtype` 取一支 |
| 2 | `SetTypingAsync` | POST | `/cgi-bin/message/custom/typing` | 同上 | 公众号形态路径；小程序/小游戏为 `.../custom/business/typing`，**本域不覆盖** |
| 3 | `GetMsgListAsync` | POST | **`/customservice/msgrecord/getmsglist`** | 同上 | **官方路径无 `/cgi-bin` 前缀**（`CM1` 显式断言 `/customservice/` 前缀，勿「顺手补齐」） |

**认证与账号面（重要澄清）**：官方本子分组三端点适用范围均为
「公众号 —— 仅认证 / 服务号 —— 仅认证」⇒ **认证的订阅号亦可调用，并非服务号专属**。
据此 `CM8` 锁定一条反向断言：`MpAppConfig` **不得**出现 `AccountType` 配置项——
账号类型不符时官方用 `40200 invalid account type` / `48001` 明确表达，
本 SDK 不建「无真实消费点」的本地能力闸。

**第三方代调用面**：本域三页官方是否支持代调用**未逐项核验**（接口 remarks 未留档），见缺口。

## 下发额度：两套不同的窗口，别混用

| 触发场景 | 额度（官方明文，remarks + `CM` 留档） |
|---|---|
| 用户主动发消息 | **5 条 / 48 小时** |
| 点击自定义菜单 | **3 条 / 1 分钟** |
| 关注公众号 | **3 条 / 1 分钟** |
| 扫描二维码 | **3 条 / 1 分钟** |

- **菜单触发面极窄**：仅 `click`、`scancode_push`、`scancode_waitmsg` **三类**菜单事件会产生客服下发额度，
  其余菜单类型（`view`、`miniprogrampage`、`article` 等）**不产生**额度。
  ⇒ 「配了菜单就能推客服消息」是错的。
- SDK **不做本地额度计数**（跨进程与多实例无从对齐），一律由官方回包表达。
  **注意**：本域三页 remarks 留档的错误码里**没有**「额度用尽」专属码
  （发送页只留 `-1`/`40001`/`40013`/`45008`/`70000`）⇒ 该码未逐项核验，见文末缺口，
  正文不编造具体数字。

## `send`：一个接口、平铺 16 个官方键、11 个消息分支

请求体 [MpSendCustomMessageRequest](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/Send/MpSendCustomMessageRequest.cs)
是**扁平结构**——`touser` + `msgtype` + 各分支对象**平级**（官方形态即如此，不是本仓的设计选择）：

| `msgtype`（常量在 `MpCustomMessageTypes`） | 分支属性 | 官方必填字段（`send` 页分支字段表） |
|---|---|---|
| `text` | `Text`（`MpTextMessage?`） | `content` |
| `image` | `Image`（`MpMediaMessage?`） | `media_id` |
| `voice` | `Voice`（`MpMediaMessage?`） | `media_id`（**与 image 共用同一类型**） |
| `video` | `Video`（`MpVideoMessage?`） | `media_id` + `thumb_media_id`（`title`/`description` 可选；**独立类型，不与前两者共用**） |
| `music` | `Music`（`MpMusicMessage?`） | `title` + `description` + `musicurl` + `thumb_media_id` |
| `news` | `News`（`MpNewsMessage?`） | `articles`，**1 条以内**（超限即 `45008`） |
| `mpnews` | `MpNews`（`MpMediaMessage?`） | `media_id`——官方标注**草稿灰度完成后不再支持** ⇒ 应改用 `mpnewsarticle` |
| `mpnewsarticle` | `MpNewsArticle`（`MpMpNewsArticleMessage?`） | `article_id`（来自「发布」系列接口） |
| `msgmenu` | `MsgMenu`（`MpMsgMenuMessage?`） | `list[{id,content}]`，`head_content` / `tail_content` 选填 |
| `wxcard` | `WxCard`（`MpWxCardMessage?`） | `card_id` |
| `miniprogrampage` | `MiniProgramPage`（`MpMiniProgramPageMessage?`） | `title` + `appid` + `pagepath` + `thumb_media_id` |

另有两个非分支键：`customservice`（`CustomService`）与 `aimsgcontext`（`AiMsgContext`），
以及 `businessid`（`BusinessId`，`string?`）。

**两处「以分支字段表为准」的裁决（`CM6`）**：
1. 官方 `send` 页的 `msgtype` **说明只列 4 种**（text/image/link/miniprogrampage），
   但同页**分支字段表覆盖 10+ 支** ⇒ SDK 按**分支字段表**建模，`MpCustomMessageTypes` 覆盖全部分支
   （含说明段遗漏的分支；常量表另含 `link`）。
2. `Image` / `Voice` / `MpNews` 三支**共用** `MpMediaMessage`（字段集相同），
   `Video` **不得**并入该共用（多 `thumb_media_id` 等键）——`CM4` 逐类型锁字段名。

⇒ 加新分支时改的是「常量表 + 分支属性 + `CM4`/`CM6` 断言」三处，不要新增一套「消息基类」。

## `typing`：30 秒前置 + 不可重复

[MpTypingRequest](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/MpTypingRequest.cs)
官方三键：`touser`（`ToUser`）、`command`（`Command`）、`businessid`（`BusinessId`）。
`command` 取值由 `CM5` 钉死在 `MpTypingCommands`：`Typing = "Typing"` / `CancelTyping = "CancelTyping"`
（**首字母大写的官方原文取值**，不是 `typing`）。

| 约束（官方明文） | 对应错误码 |
|---|---|
| 需**之前 30 秒内**与该用户有过消息交互 | `45080 need sending message to user or recving message from user in the last 30 seconds before typing` |
| 已处于输入状态时**不可重复下发** | `45081 you are already typing` |
| `command` 取值不对 | `45072` |
| 账号类型不符 | `40200` |

本域只覆盖公众号/服务号路径 `/cgi-bin/message/custom/typing`；
小程序/小游戏形态是 `/cgi-bin/message/custom/business/typing`，**未建模**。

## `getmsglist`：游标分页、两条硬上限、一个官方文档缺陷

| 官方键 | C# 属性（类型） | 事实 |
|---|---|---|
| `starttime` | `StartTime`（`long`） | Unix 时间戳，**秒** |
| `endtime` | `EndTime`（`long`） | 与起始时间跨度**不得超过 24 小时** |
| `msgid` | `MsgId`（`long`） | 分页**游标**：取上一页应答的 `msgid` 续拉 |
| `number` | `Number`（`int`） | 每次最多 **10000 条** |

- **官方文档缺陷（照录，勿据其省字段）**：该页字段区标注「请求体：无」，但请求示例明确带这 4 个字段
  ⇒ SDK 按**示例**建模，否则无法分页拉取。
- 需已开通/升级**新版客服功能**，否则 `65400`；`65416` 查询参数不合法、`65417` 时间段超出限制。
- 应答 [MpGetMsgListResponse](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/MpGetMsgListResponse.cs)
  = `recordlist`（`RecordList`）+ `number`（`Number`）+ `msgid`（`MsgId`），
  单条 [MpMsgRecord](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/MpMsgRecord.cs)
  = `openid` / `opercode` / `text` / `time` / `worker`。
  操作码常量（`CM5`）：`MpMsgRecordOperCodes.WorkerSent = 2002`（客服发送）、
  `WorkerReceived = 2003`（客服接收）；**官方其余 opercode 取值未逐项核验**（见缺口）。
- `openid` 与 `worker` 属敏感/用户标识：**不得**写入日志、遥测或异常消息。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddCustomerMessageApi());

// 注入 IMpCustomerMessageService（注册组 CustomerMessage）
// 1) 发文本：msgtype 用常量，分支对象与 msgtype 必须配对
await customer.SendCustomMessageAsync(new MpSendCustomMessageRequest
{
    ToUser = openid,                                   // 用户标识：不得进日志
    MsgType = MpCustomMessageTypes.Text,
    Text = new MpTextMessage { Content = "您的取件码已生成" },
}, ct);

// 2) 发图文：官方 news.articles 限 1 条以内，超限即 45008
//    mpnews 已弃用（草稿灰度完成后不再支持）⇒ 新接入用 mpnewsarticle + article_id
await customer.SendCustomMessageAsync(new MpSendCustomMessageRequest
{
    ToUser = openid,
    MsgType = MpCustomMessageTypes.News,
    News = new MpNewsMessage
    {
        Articles = [new MpNewsArticle { Title = "标题", Url = "https://example.com/a" }],
    },
}, ct);

// 3) 输入状态：command 取值为官方原文 "Typing" / "CancelTyping"（首字母大写）
await customer.SetTypingAsync(new MpTypingRequest
{
    ToUser = openid,
    Command = MpTypingCommands.Typing,
}, ct);                          // 前置：30 秒内与该用户有交互（45080）；不可重复下发（45081）

// 4) 聊天记录：时间段 ≤ 24 小时、每次 ≤ 10000 条；游标是 msgid
var records = await customer.GetMsgListAsync(new MpGetMsgListRequest
{
    StartTime = startUnixSeconds,
    EndTime = endUnixSeconds,
    MsgId = 0,
    Number = 1000,
}, ct);
foreach (var r in records.RecordList ?? [])
{
    var kind = r.OperCode switch
    {
        MpMsgRecordOperCodes.WorkerSent => "客服发出",
        MpMsgRecordOperCodes.WorkerReceived => "客服接收",
        _ => "其它（本仓未核验取值）",
    };
    Console.WriteLine($"[{r.Time}] {kind}");           // 不打印 r.OpenId / r.Worker
}
```

**判错只有一层**（`errcode` / `errmsg`；`send` / `typing` 返回 `MpResponse`）。
本域**没有异步回执事件**——`MASSSENDJOBFINISH` 属群发域、`TEMPLATESENDJOBFINISH` 属模板域，勿混用。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 第三方平台代调用面（3 页） | 接口 remarks **未留档** ⇒ 按未核验处理 | 重新逐页核验 → 补 remarks → 同批更新本文 |
| `msgrecord` 的 `opercode` 全集 | 仓内只钉 `2002` / `2003` 两值（`CM5`），其余**未逐项核验** | 需要完整会话重建时先核验官方页，不得从邻域借枚举 |
| `recordlist[].text` 的形态（消息正文还是摘要） | remarks **未定性** | 核验后补 DTO remarks 与本文 |
| `customservice` / `aimsgcontext` 两个非分支键的官方使用条件 | 字段名由 `CM4` 锁定，**触发条件未核验** | 核验后补 remarks；核验前不要按「可选即无用」删除 |
| `businessid` 与小程序/小游戏客服链路的对应关系 | 本域只建模公众号路径 | 落地 `business/typing` 须走「加端点 + 同批改 `CM1` 计数」 |
| 额度耗尽的错误码集合 | remarks 只留发送页的 `-1`/`40001`/`40013`/`45008`/`70000` 与 typing/record 页若干，「额度用尽」专属码**未逐项核验** | 核验后补进 `CM9` 常量断言 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、额度表、菜单触发面、账号适用性澄清 | [IMpCustomerMessageService.cs](./IMpCustomerMessageService.cs) 的 XML 注释 |
| 3 条路由 + `/customservice/` 前缀断言 | `CM1` = `CustomerMessageEndpoints_ShouldMatchOfficialRoutes` |
| 注册形态与令牌绑定 | `CM2` = `CustomerMessageInterface_ShouldRegisterDirectlyWithQueryToken` |
| DTO 全量登记进 `CustomerMessageJsonContext`（`expectedCount = 18`） | `CM3` = `CustomerMessageDataModels_ShouldBeRegisteredInJsonContext` |
| 各分支官方 JSON 字段名与「Image/Voice/MpNews 共用、Video 不共用」 | `CM4` = `MessageBranchFieldNames_ShouldMatchOfficialBranchTable` |
| typing / 聊天记录字段与上限、`MpTypingCommands`、`MpMsgRecordOperCodes` | `CM5` = `TypingAndMsgRecordModels_ShouldMatchOfficialContracts` |
| `MpCustomMessageTypes` 覆盖官方分支字段表全部分支 | `CM6` = `CustomMessageTypes_ShouldCoverAllOfficialBranches` |
| `MpModule.CustomerMessage` + `AddCustomerMessageApi()` + 白名单 | `CM7` = `CustomerMessageModule_ShouldBeRegistered` |
| 「不引入 `MpAccountType`」的决策锁定 | `CM8` = `AccountTypeGate_ShouldNotExistWithoutRealConsumptionPoint` |
| 已核验错误码常量 | `CM9` = `CustomerMessageErrorCodes_ShouldMatchVerifiedOfficialValues` |
| Query 令牌白名单（唯一持票人） | `QT1`（`MpQueryTokenWhitelistGuard.cs`） |
| 分支 DTO | [DataModels/CustomerMessage/Send/](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/Send/)（14 个文件）与 [DataModels/CustomerMessage/](../../../Mud.Wechat.OfficialAccount.DataModels/CustomerMessage/)（4 个文件） |
| 生成入口与模块枚举、上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与额度/前置条件，**同批**更新 `CM1` 的 3 条路由表；
   动到 `customservice` 前缀时保留 `CM1` 的「无 `/cgi-bin` 前缀」断言，勿「补齐」成 `/cgi-bin/...`。
2. 加 / 改消息分支 → 常量（`MpCustomMessageTypes`）+ 请求体属性 + `CM4` 字段名断言 + `CM6` 覆盖断言，四处同批。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `CustomerMessage`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 同批更新 `CM3` 的 `expectedCount`（18）。
4. 确认 `Generated/CustomerMessageJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
5. 动注册形态 → `MpModule.CustomerMessage` + `AddCustomerMessageApi()` + `QT1` + `MpRouteCountGuard`
   （`RC1`/`RC2`）与 `Tests/ContractBaseline.cs` 基线一起改；**计数以守卫为准**。
6. **不要**引入 `MpAppConfig.AccountType` 或本域本地能力闸（`CM8` 反向断言）；
   **不要**把 `command` 写成小写 `typing`；**不要**把 `Video` 并进 `MpMediaMessage`；
   **不要**假设任意菜单类型都能换来客服额度。
7. 最后跑 `scripts/verify-build.ps1`，全绿才算完成。
