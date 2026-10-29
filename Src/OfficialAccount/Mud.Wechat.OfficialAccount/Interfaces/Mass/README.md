# Mass 域 · 群发消息

微信公众号 / 服务号「群发消息」域：**7 支端点**（按标签群发 / 按 OpenID 列表群发 / 预览 / 删除 / 查状态 / 设速度 / 取速度），全部落在 `/cgi-bin/message/mass/*`。
接口 [IMpMassMessageService.cs](./IMpMassMessageService.cs)，注册组 `Mass`，
模块枚举 `MpModule.Mass`（[MpModule.cs](../../Extensions/MpModule.cs)；`AddMassApi()` ⇒ 生成入口
`AddMassWebApiHttpClient()`，见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpMassContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs)
> （`MS1`~`MS4`、`MS7`、`MS8`；同一文件还持有带参二维码 `MS5` 与自动回复 `MS6` 的断言）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`/`QT2`）、
> [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`/`RC3`）
> 与本仓 DTO（[DataModels/Mass/](../../DataModels/Mass)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」，正文不引用外部官方文档表述。

## 端点速览

7 条路由逐条钉在 `MS1`（守卫号 `MS1`：`MassEndpoints_ShouldMatchOfficialRoutes`，`HaveCount(7)` + 全 POST），
并含两条**反向断言**：`NotContain("/cgi-bin/media/uploadnews")`、`NotContain("/cgi-bin/media/uploadimg")`。
本域在 `QT1` 白名单登记为「`IMpMassMessageService`（7 端点）」，`QT2` 断言接口 `[Token]` 的 `Name = access_token`。

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `SendAllAsync` | POST | `/cgi-bin/message/mass/sendall` | Query 注入 `access_token`（MUD005） | 仅认证；`filter` + `msgtype` + 分支对象其一 |
| 2 | `SendAsync` | POST | `/cgi-bin/message/mass/send` | 同上 | **服务号专属**（官方原文）；`touser` 列表 **2~10000** 个 |
| 3 | `PreviewAsync` | POST | `/cgi-bin/message/mass/preview` | 同上 | `touser` / `towxname` 二选一；分支比群发页多 `music` 与单图 `image` |
| 4 | `DeleteMassMsgAsync` | POST | `/cgi-bin/message/mass/delete` | 同上 | 群发「撤回」；`msg_id` / `url` 二选一 + `article_idx` |
| 5 | `GetMassMsgStatusAsync` | POST | `/cgi-bin/message/mass/get` | 同上 | 响应**仅** `msg_id` / `msg_status` |
| 6 | `SetMassSpeedAsync` | POST | `/cgi-bin/message/mass/speed/set` | 同上 | `speed` 0~4 |
| 7 | `GetMassSpeedAsync` | POST | `/cgi-bin/message/mass/speed/get` | 同上 | **POST 且无请求体**（`MS2` 钉 `speed`/`realspeed`） |

**账号归属（照录，SDK 不本地拦截）**：`sendall` / `preview` / `delete` / `get` / `speed` 双端点为
「公众号 / 服务号 —— 仅认证」；**只有 `mass/send` 是服务号专属**（官方原文「本接口支持『服务号（仅认证）』账号类型调用」）。
账号面不符由官方 `48001` 表达 —— 这是 `N1` 裁决，与本线其它域一致：不引入 `AccountType` 本地闸。

## 提交 ≠ 群发完成（本域头号陷阱）

接口 remarks 原文留档：「在返回成功时，意味着群发任务**提交成功**，并不意味着此时群发已经结束」。

- 最终结果经回调事件 **`masssendjobfinish` / `MASSSENDJOBFINISH`** 异步推送
  （`Status` 含 `send success` / `send fail` / `err(num)` 审核失败码族）；
- **该事件属回调包 `Mud.Wechat.OfficialAccount.Callback`，不在本域** —— 本域只提供端点与同步查询；
- 同步侧只有 `GetMassMsgStatusAsync` 的 `msg_status`（`SEND_SUCCESS` / `SENDING` / `SEND_FAIL` / `DELETE`）。

## `mass/get` 的响应**只有两个字段**（不得从事件页补全）

`MS2` 用精确集合钉死：
`statusJsonNames.Should().BeEquivalentTo(new[] { "errcode", "errmsg", "msg_id", "msg_status" })`。
⇒ `TotalCount` / `FilterCount` / `SentCount` / `ErrorCount` / `TextCount` **只存在于 `masssendjobfinish` 的推送 XML** 里，
不在本查询接口响应中（方案文档的预判已按逐页核验修正，勿「顺手补齐」）。

另一处**键形态不对称**（照录，两页各自建模）：

| DTO | `msg_id` 类型 | 依据 |
|---|---|---|
| `MpMassStatusRequest`（`mass/get` 请求） | **`string`** | 官方示例为字符串形态 `"201053012"` |
| `MpMassDeleteRequest`（`mass/delete` 请求） | **`long?`** | 群发返回的数值 msg_id |
| `MpMassSendResponse` / `MpMassStatusResponse` | `long` / `long?` | 返回侧数值形态 |

## 消息分支：`image/text/mpnews` 三条支路形态不同构

`MpMassMsgTypes`（声明在接口文件里，与接口同生共死）钉住六个 `msgtype` 取值：
`mpnews` / `text` / `voice` / `image` / `mpvideo` / `wxcard`。
**不做运行时多态**（AOT 源生成按声明类型序列化）⇒ 分支对象全部可空、由调用方保证与 `msgtype` 一致，
错配由官方 `40008` 表达。

| `msgtype` | 请求侧属性 | 载体 DTO | 关键点 |
|---|---|---|---|
| `mpnews` | `MpNews` | `MpMassMediaMessage{media_id}` | `media_id` 来自草稿箱 / 临时素材；返回 `msg_data_id` **仅图文群发时出现** |
| `text` | `Text` | `MpMassText{content}` | 纯文本 |
| `image` | `Images` | `MpMassImages{media_ids, recommend, ...}` | **属性名是复数 `images`、字段是 `media_ids` 列表**（不是单图 `media_id`） |
| `voice` | `Voice` | `MpMassMediaMessage{media_id}` | 与 `mpnews`/`image` 的载体不共用（`MS2` 分支锁定） |
| `mpvideo` | `MpVideo` | `MpMassVideoMessage{media_id, title, description}` | `title`/`description` **只在 `mass/send` 页出现** ⇒ `sendall` 用 `MpMassMediaMessage` |
| `wxcard` | `WxCard` | `MpMassWxCard{card_id, card_ext}` | 官方 `45113` 已不支持商品消息与卡券发表，词表保留供判错语义参考 |

预览页（`MpMassPreviewRequest`）额外有 `Music` 与单图 `Image`（两者都用 `MpMassMediaMessage`），
且 `MpMassWxCardExt.Signature` **只在预览页出现** —— 群发页没有该字段。
`MpMassMsgTypes` 里**没有** `music`（接口 remarks 明示「预览页另有 music，未在此列——预览请求直接传字符串」）。

## 幂等：`clientmsgid` 24 小时防重

`MpMassSendAllRequest.ClientMsgId` / `MpMassSendRequest.ClientMsgId`：**≤32 字节**，
官方 **24 小时内防重**；重复提交时官方用 `45065` 返回**已存在群发任务的 msgid**。
⇒ 重试策略应复用同一 `clientmsgid`，而不是每次新生成。

## 频次与安全保护（官方群发指南页留档，2026-10-07 核验）

| 限制 | 数值 | 相关码 |
|---|---|---|
| 群发接口调用频率 | **每分钟 60 次**，超限被拒 | `42010`（操作太快） |
| 用户接收限制 | 用户**每月只能接收 4 条**群发，多出的对该用户发送失败（认证服务号） | `45028`（当前周期内发表次数已用完） |
| 预览到微信号 | 每日 **100 次**限制 | — |
| API 群发安全保护 | 群发全部用户后**须等管理员确认**；拒绝或 **30 分钟**未确认则本次失败 | `89504` / `89505` |

`API` 页本身**无次数上限数值** —— 上述数字来自群发指南页的 remarks 留档，勿当作接口页硬承诺。

## 群发速度档位：`speed` 0~4 → 万条/分钟

`speed/set` 传 `{"speed":N}`；`speed/get` 是 **POST 且无请求体**
（官方参数表写「无」，页面示例给 `{"speed":3}` 属矛盾 ⇒ **SDK 按参数表建模**，`MS2` 钉 `speed`/`realspeed` 两字段）。
对照表（官方注意事项原文）：`0`→80w、`1`→60w、`2`→45w、`3`→30w、`4`→10w（每分钟）。
越界 `45083`（speed 不在 0~4）、未设置 `45084`。

## `media_id` 生命周期：群发成功后草稿素材失效

官方原文（remarks 留档）：「如果 `media_id` 是通过草稿箱 / 新建草稿接口来得到的，
那么在**群发成功后** `media_id` 会失效，后台草稿也会被自动删除」⇒ 与
[Draft 域](../Draft/README.md)「被群发或发布后从草稿箱移除」是同一件事的两面。

## 群发撤回：`mass/delete` 的四条官方限定

`DeleteMassMsgAsync` 的 remarks 逐字留档四条限制：

1. 「只有通过 **api** 发送的并且**已经发送成功**的消息才能删除」；
2. 「只能删除**文章和视频**消息，其他类型一经发送无法删除」；
3. 「删除是使消息**正文内容失效**，已收到的用户在本地仍能看到消息卡片」；
4. 「多次群发同一篇文章时，删除其中一次会删掉该内容，**导致所有群发都失效**」。

请求侧 `msg_id` 与 `url` **二选一**，配 `article_idx`（`int`）。相关码：
`40059`（msg_id 传参有误）/ `40060`（article_idx 不合法）/ `40065`（文章状态异常）/
`40204`（不允许删除洗稿文或谣言文）/ `9500001`。

## 不属于本域的两支「群发相关」端点

- `media/uploadimg`（图文正文图片）归**素材域** [IMpMediaService](../Media/IMpMediaService.cs)；
- `media/uploadnews`（旧「上传图文消息素材」）官方标注「该能力已更新为草稿箱」⇒ **不建模**（`N5` 裁决）。

两支都由 `MS1` 的反向断言钉住，本域路由表恒为 7 条。
另外官方历史上曾有 `draft/switch`（草稿开关）与「群发前需先建标签」的编排习惯，
但**本域没有 `message/mass/add_tag` 端点**：标签能力全在 Tag 域
（[IMpTagService.cs](../Tag/IMpTagService.cs)，`/cgi-bin/tags/create|get`、`tags/members/batchtagging` 等 8 支），
群发只是用 `filter.tag_id` 引用它。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddMassApi());

// 注入 IMpMassMessageService（注册组 Mass）
// 1) 按标签群发图文：filter + msgtype + 分支对象必须配对（错配官方 40008）
//    clientmsgid ≤32 字节、24 小时内防重 ⇒ 重试要复用同一个值
var sent = await mass.SendAllAsync(new MpMassSendAllRequest
{
    Filter = new MpMassFilter { IsToAll = false, TagId = tagId },
    MsgType = MpMassMsgTypes.MpNews,
    MpNews = new MpMassMediaMessage { MediaId = draftMediaId },  // 草稿 media_id：群发成功后失效
    SendIgnoreReprint = 0,                                        // 默认 0：被判为转载即停止群发
    ClientMsgId = clientMsgId,
}, ct);
long msgId = sent.MsgId;                     // msg_data_id 仅图文群发时出现
long? msgDataId = sent.MsgDataId;

// 2) 给全体群发（is_to_all）后需管理员确认，拒绝或 30 分钟未确认 ⇒ 89504/89505
await mass.SendAllAsync(new MpMassSendAllRequest
{
    Filter = new MpMassFilter { IsToAll = true },
    MsgType = MpMassMsgTypes.Text,
    Text = new MpMassText { Content = "系统将于今晚维护" },
}, ct);

// 3) 按 OpenID 列表群发：服务号专属，touser 最少 2 个、最多 10000 个
//    openid 属用户级敏感标识：不得进日志 / 遥测 / 异常消息
await mass.SendAsync(new MpMassSendRequest
{
    ToUserList = openIds.Take(10000).ToList(),
    MsgType = MpMassMsgTypes.Image,
    Images = new MpMassImages { MediaIds = mediaIds, Recommend = "活动推荐" },
}, ct);

// 4) 预览：touser / towxname 二选一（预览到微信号每日 100 次）
await mass.PreviewAsync(new MpMassPreviewRequest
{
    ToUser = previewOpenId,
    MsgType = MpMassMsgTypes.Text,
    Text = new MpMassText { Content = "预览样式检查" },
}, ct);

// 5) 查状态：响应只有 msg_id / msg_status，统计数字来自事件推送不在这里
//    注意请求侧 msg_id 是 string（官方示例形态）
var st = await mass.GetMassMsgStatusAsync(new MpMassStatusRequest { MsgId = msgId.ToString() }, ct);
string msgStatus = st.MsgStatus ?? string.Empty;   // SEND_SUCCESS / SENDING / SEND_FAIL / DELETE

// 6) 设置并读取群发速度（speed/get 无请求体）
await mass.SetMassSpeedAsync(new MpMassSpeedSetRequest { Speed = 1 }, ct);   // 1 → 60w/分钟
var speed = await mass.GetMassSpeedAsync(ct);
int realSpeed = speed.RealSpeed;

// 7) 撤回（只删得掉 api 发送且已成功的文章/视频；正文失效但用户本地仍有卡片）
await mass.DeleteMassMsgAsync(new MpMassDeleteRequest { MsgId = msgId, ArticleIdx = 0 }, ct);
```

**判错只有一层**：`errcode` / `errmsg`。本域的群发**结果回执不在本域**：
`MASSSENDJOBFINISH` 属回调包 `Mud.Wechat.OfficialAccount.Callback`（`TEMPLATESENDJOBFINISH` 属模板域，勿混用）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域**无独立守卫文件** | 断言与带参二维码（`MS5`）、自动回复（`MS6`）、一次性订阅（`MS3`）共用 [MpMassContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs) | 拆域需同批改守卫 remarks 的「群发 + 订阅 + 二维码 + 自动回复」构成 |
| 官方是否存在 `message/mass/add_tag` | 本仓**无该端点**、也无留档说明；标签能力在 Tag 域 | 未逐项核验 ⇒ 不得臆造端点，需要时先逐页核验 |
| 第三方平台代调用面（7 页） | 接口 remarks **未留档**（只留账号归属与 N1 裁决） | 逐页核验 → 补 remarks → 同批更新本文 |
| `MASSSENDJOBFINISH` 的完整 `Status` 取值与 `err(num)` 审核失败码族 | 本域 remarks 只列 `send success` / `send fail` / `err(num)` 三类 | 报文建模在回调包，本域不扩 |
| 每分钟 60 次 / 每月 4 条的**计量口径**（账号级还是接口级） | remarks 只抄指南页原文，未定量化归属 | 未逐项核验，勿据此做本地限流器 |
| `send_ignore_reprint` 的取值语义 | 官方两页参数表形态不一致（只出现在说明与示例，未入参数表）⇒ 按说明建模、默认 0 | 全集未逐项核验 |
| `MpMassImages.Recommend` / `NeedOpenComment` / `OnlyFansCanComment` 的生效条件 | 字段名由 `MS2` 钉住，**触发条件未核验** | 勿按「可选即无用」删除 |
| `mass/send` 的 `touser` 上限 10000 之外是否有分页/队列 | 仓内只留「2~10000」 | 保持缺口 |
| `45065` 返回体里已存在任务的 msgid 字段名 | 只留语义「返回已存在群发任务的 msgid」 | 需实际报文核验后补 DTO remarks |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、归属差异、提交≠完成、频次、安全保护、速度档、撤回四限制、N1/N4/N5 裁决 | [IMpMassMessageService.cs](./IMpMassMessageService.cs) 的 XML 注释与 `MpMassMsgTypes` |
| 7 条路由 + 全 POST + `NotContain(uploadnews/uploadimg)` | `MS1` = `MassEndpoints_ShouldMatchOfficialRoutes` |
| 分支 DTO、`touser` 2~10000、`filter`/`clientmsgid`/`send_ignore_reprint`、`statusJsonNames` 精确集合、`speed`/`realspeed` | `MS2` = `MassRequestShapes_ShouldLockBranchDtos` |
| 一次性订阅的域边界（端点在 `IMpTemplateService.SendOneTimeSubscribeAsync`、DTO 落 `DataModels.Mass`、有 `color`） | `MS3` = `OneTimeSubscribe_ShouldLiveInTemplateDomainWithColorField` |
| DTO 全量登记进 `MassJsonContext`（`expectedCount = 19`，含一次性订阅 3） | `MS4` = `MassDataModels_ShouldBeRegisteredInJsonContext` |
| `MpModule.Mass` + `AddMassApi()` + 白名单「本域在内」（全量由 `QT1` 单点持有） | `MS7` = `MassQrcodeAutoReplyModules_ShouldBeRegisteredAndWhitelisted` |
| 已核验错误码常量（40152/40215/45162/45028/45062/45065/45066/45067/45113/48021/48022/41040/40130/89504/89505/40062/40052/40053） | `MS8` = `MassErrorCodes_ShouldMatchVerifiedOfficialValues` |
| Query 令牌白名单与令牌绑定 | `QT1`（全集持票人）、`QT2`（本域接口 `access_token` 断言） |
| 19 个 DTO（`SerializerClassName = "Mass"`） | [DataModels/Mass/](../../DataModels/Mass) |
| 生成入口 / 模块枚举 / 上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |
| 本线总览（**统计已过期**，不可当权威） | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与频次/前置条件，**同批**更新 `MS1` 的 7 条路由表
   与两条反向断言（**不要**把 `uploadimg` / `uploadnews` 拉回本域）；
   路由数变化要同步 `RC1`/`RC2` 与 [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线。
2. 加 / 改消息分支 → `MpMassMsgTypes` 常量 + 请求体属性 + `MS2` 分支断言三处同批；
   分支对象一律可空、按 `msgtype` 携带其一，**不得**引入运行时多态（AOT 源生成按声明类型序列化）。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Mass`，见
   [AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1)）→ 跑
   [GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) → 同批更新 `MS4` 的 `expectedCount`（19）。
   注意一次性订阅的 3 个 DTO **也登记在 Mass 上下文**（`MS3` 的域边界裁决）。
4. 确认 `Generated/MassJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
5. 动注册形态 → `MpModule.Mass` + `AddMassApi()` + `MS7` + `QT1` 一起改；**计数以守卫为准**。
6. 加 / 改错误码 → 逐页核验后同批补 `MS8` 常量断言与 `MpErrorCodes`。
7. **不要**做 `AccountType` 本地闸（`mass/send` 的服务号专属由官方 `48001` 表达）；
   **不要**给 `mass/get` 响应补 `TotalCount`/`SentCount`；**不要**把 `speed/get` 改成 GET 或加请求体；
   **不要**每次重试都新生成 `clientmsgid`；**不要把 `msg_id` 统一成一个类型**（`get` 请求是 `string`、`delete` 请求是 `long?`）。
8. 群发结果回执（`MASSSENDJOBFINISH`）属回调包 `Mud.Wechat.OfficialAccount.Callback`，
   **不要**在本域加事件模型。
9. 最后跑 [verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
