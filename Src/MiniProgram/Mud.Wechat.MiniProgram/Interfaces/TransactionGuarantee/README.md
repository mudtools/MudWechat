# TransactionGuarantee 域 · 交易保障 · 评价管理

微信小程序「交易保障 · 评价管理」域 SDK（9 端点：评价列表 / 详情 / 评论回复列表 3 查 +
评论增删 2 + 回复增删 2 + 确认和解 + 重置 API 客服 quota，官方「交易保障」分组 → 评价管理，
路由族 `/wxaapi/comment/*`）。 接口：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)、[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)、[IWxaTradeGuaranteeService](./IWxaTradeGuaranteeService.cs)；注册组 `TransactionGuarantee`；模块枚举 `MiniProgramModule.TransactionGuarantee`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaTransactionGuaranteeContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaTransactionGuaranteeContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)、[MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee`](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaTradeCommentService = 9；IWxaTradeComplaintService = 5；IWxaTradeGuaranteeService = 2；按 HTTP 特性实计 **16** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaTradeCommentService` | `GetCommentListAsync` | GET | `/wxaapi/comment/mpcommentlist/get` | `AccessToken`（Query），Query `access_token` | 查询评价列表（`GET /wxaapi/comment/mpcommentlist/get`）。<br>官方文档：https://developers.weixin.qq.com/min… |
| 2 | `IWxaTradeCommentService` | `GetCommentInfoAsync` | GET | `/wxaapi/comment/commentinfo/get` | `AccessToken`（Query），Query `access_token` | 查询评价详情（`GET /wxaapi/comment/commentinfo/get`）。<br>官方文档：https://developers.weixin.qq.com/minip… |
| 3 | `IWxaTradeCommentService` | `GetReplyAndCommentReplyListAsync` | GET | `/wxaapi/comment/replyandcommentreplylist/get` | `AccessToken`（Query），Query `access_token` | 查询评论列表（`GET /wxaapi/comment/replyandcommentreplylist/get`，官方一次返回「评论 + 回复」两层）。<br>官方文档：https:/… |
| 4 | `IWxaTradeCommentService` | `AddReplyAsync` | POST | `/wxaapi/comment/reply/add` | `AccessToken`（Query），Query `access_token` | 创建评论（`POST /wxaapi/comment/reply/add`，评价下的第一层内容）。<br>官方文档：https://developers.weixin.qq.com/mi… |
| 5 | `IWxaTradeCommentService` | `DeleteReplyAsync` | POST | `/wxaapi/comment/reply/delete` | `AccessToken`（Query），Query `access_token` | 删除评论（`POST /wxaapi/comment/reply/delete`）。<br>官方文档：https://developers.weixin.qq.com/miniprogr… |
| 6 | `IWxaTradeCommentService` | `AddCommentReplyAsync` | POST | `/wxaapi/comment/commentreply/add` | `AccessToken`（Query），Query `access_token` | 创建回复（`POST /wxaapi/comment/commentreply/add`，评论下的追加回复）。<br>官方文档：https://developers.weixin.qq.… |
| 7 | `IWxaTradeCommentService` | `DeleteCommentReplyAsync` | POST | `/wxaapi/comment/commentreply/delete` | `AccessToken`（Query），Query `access_token` | 删除回复（`POST /wxaapi/comment/commentreply/delete`）。<br>官方文档：https://developers.weixin.qq.com/mi… |
| 8 | `IWxaTradeCommentService` | `ConfirmCompromiseAsync` | POST | `/wxaapi/comment/confirmcompromise` | `AccessToken`（Query），Query `access_token` | 确认和解（`POST /wxaapi/comment/confirmcompromise`，差评挽回流程的商家动作）。<br>官方文档：https://developers.weixin… |
| 9 | `IWxaTradeCommentService` | `ResetApiKfQuotaAsync` | POST | `/wxaapi/comment/apikfquota/reset` | `AccessToken`（Query），Query `access_token` | 重置 API 客服 quota（`POST /wxaapi/comment/apikfquota/reset`）。<br>官方文档：https://developers.weixin.q… |
| 10 | `IWxaTradeComplaintService` | `GetComplaintOrderDetailAsync` | GET | `/wxaapi/minishop/complaintOrderDetail` | `AccessToken`（Query），Query `access_token` | 查询投诉单详情（`GET /wxaapi/minishop/complaintOrderDetail`）。<br>官方文档：https://developers.weixin.qq.co… |
| 11 | `IWxaTradeComplaintService` | `RespondComplaintAsync` | POST | `/wxaapi/minishop/bussiRespondComplaint` | `AccessToken`（Query），Query `access_token` | 商家回应投诉（`POST /wxaapi/minishop/bussiRespondComplaint`，同意 / 拒绝和解）。<br>官方文档：https://developers.w… |
| 12 | `IWxaTradeComplaintService` | `SupplyProofAsync` | POST | `/wxaapi/minishop/bussiSupplyProof` | `AccessToken`（Query），Query `access_token` | 商家补充凭证（`POST /wxaapi/minishop/bussiSupplyProof`）。<br>官方文档：https://developers.weixin.qq.com/mi… |
| 13 | `IWxaTradeComplaintService` | `SupplyRefundAsync` | POST | `/wxaapi/minishop/bussiSupplyRefund` | `AccessToken`（Query），Query `access_token` | 商家提交退款凭证（`POST /wxaapi/minishop/bussiSupplyRefund`，退货场景的处理凭证）。<br>官方文档：https://developers.wei… |
| 14 | `IWxaTradeComplaintService` | `AppealAsync` | POST | `/wxaapi/minishop/busiAppeal` | `AccessToken`（Query），Query `access_token` | 商家申诉（`POST /wxaapi/minishop/busiAppeal`，对平台判责结果提出申诉）。<br>官方文档：https://developers.weixin.qq.co… |
| 15 | `IWxaTradeGuaranteeService` | `GetGuaranteeStatusAsync` | GET | `/wxaapi/wxamptrade/get_guarantee_status` | `AccessToken`（Query），Query `access_token` | 获取交易保障标状态（`GET /wxaapi/wxamptrade/get_guarantee_status`）。<br>官方文档：https://developers.weixin.q… |
| 16 | `IWxaTradeGuaranteeService` | `GetPenaltyListAsync` | GET | `/wxaapi/wxamptrade/get_penalty_list` | `AccessToken`（Query），Query `access_token` | 获取小程序交易体验分违规记录（`GET /wxaapi/wxamptrade/get_penalty_list`）。<br>官方文档：https://developers.weixin.… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaTradeCommentService`

**三层对象模型（官方语义，决定端点定位键）**：官方把该域对象分为
**评价**（`commentId`，用户对订单的打分）→ **评论**（`replyId`，评价下的第一条内容）
→ **回复**（`commentReplyId`，评论之后的所有追加内容）。因此
`reply/*` 两端点操作「评论」、`commentreply/*` 两端点操作「回复」，
且 `replyandcommentreplylist/get` 一次返回两层 —— 命名易混，
本 SDK 的方法名与请求 DTO 名一律**照官方路由段**命名，不做「语义更正」。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`；第三方平台代商家调用为**权限集 id 151**（9 端点逐页一致），
以 `authorizer_access_token` 代调用。适用范围均为「小程序」账号类型。

**GET 端点的「请求体」表建模裁决**：本域 3 个查询端点官方动词为 GET，却在页面给出
「请求体 Request Payload」表；官方**代码示例**一致地以查询串发送
（如 `mpcommentlist/get?filterType=1&offset=0&limit=8&startTime=...&endTime=...&access_token=xxx`）
⇒ 本 SDK 按示例落地为 `[Query]`，不为 GET 声明请求体（全仓零 GET + `[Body]` 先例）。

**本域公共错误码（9 端点逐页同一张表，照录）**：`-10403` 无权限操作该评价/评论/回复、
`-10008` 服务异常、`-10007` 无权限调用接口、`-10003` 参数不匹配、
`-10001` 参数异常、`10000` 和解场景为「不是差评 / 发送过模板消息」，其它场景为参数异常。
**注意本域错误码为负数族**，与全局 `4xxxxx` 令牌失效码不同源，
不属令牌自愈触发集。

### `IWxaTradeComplaintService`

**路由族归属裁决（本域最重要的一条边界）**：5 端点路由为 `/wxaapi/minishop/complaintOrderDetail`、
`/wxaapi/minishop/bussiRespondComplaint`、`/wxaapi/minishop/bussiSupplyProof`、
`/wxaapi/minishop/bussiSupplyRefund`、`/wxaapi/minishop/busiAppeal` —— 前缀 `minishop`
是历史遗留命名，但官方 2026-10-13 已把这 5 页**整族迁入**小程序服务端 API 的
`transaction-guarantee/complaint` 章节，逐页「适用范围」明示「支持『小程序』账号类型调用」，
令牌为小程序 `access_token`（权限集 id **76** 代调用）⇒ 归小程序线 TransactionGuarantee 域。
小程序线 P3-R1 的 `/wxaapi/minishop/*` 零声明裁决已据此**收窄**为
「小商店店铺 / 商品管理面零声明；官方仍在册的投诉处理 5 条在册」，
小店（Channels）线侧不得声明本族路由。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。

**路由拼写照官方原文，禁止「顺手规整」（两处反直觉点）**：
① 三个商家动作端点是 `bussi`（三个 s 起头的 `bussi`），而申诉端点是 `busi`（`busiAppeal`），
官方两页拼写不一致；② 本域路由为**驼峰**动词名，而同分区的 basic 域是 `snake_case`
（`get_penalty_list`）。全部照录，不做归一化。

**状态机前置（官方错误码原文）**：4 个动作端点共有 `309`「状态不对（可能是已经转到下一个状态了）」，
官方解决方案栏写「开发者自行校验投诉单状态」⇒ 动作前应先调查询详情端点确认
`complaintOrder.status` / `item.itemType` + `blameResult`，据此选择回应 / 补充凭证 / 提交退款 / 申诉。

### `IWxaTradeGuaranteeService`

**域归属（官方目录 2026-10-13 逐页核验）**：本域 2 端点位于官方服务端 API 的
`transaction-guarantee/basic` 章节，路由前缀 `/wxaapi/wxamptrade/`，
适用范围「小程序」账号类型 ⇒ 与同分区的「评价管理」（`/wxaapi/comment/*`，9 端点）
与「投诉管理」（`/wxaapi/minishop/complaint*` 等 5 端点）合为本线 **TransactionGuarantee** 域，
共用注册组 `TransactionGuarantee`；三张接口的分工见各自 XML 注释。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`，不新增令牌类型常量（新增会让 `MpTokenManagerRegistry` 单槽
`Resolve` 返回 `null` ⇒ errcode 令牌自愈静默失效）。

**第三方平台代调用**：官方逐页明示支持第三方平台代商家调用 ——
`get_guarantee_status` 权限集 id **30、151**（获得其中之一即可），
`get_penalty_list` 权限集 id **151**，均以 `authorizer_access_token` 代调用；
本 SDK 只声明小程序应用级 `access_token`（Query），第三方代调用形态由宿主以小程序应用配置供给令牌。

**调用边界**：本域端点均为服务器端 HTTP 调用，官方未提供云调用形态。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）官方错误码：`0` 成功 / `1` 单号不存在 / `309` 状态不对 / `10001` 参数异常 / `40001` access_token 无效。
- （出现 3 次）官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`（权限集 id 76）。

### 1. `GetCommentListAsync` — 查询评价列表（`GET /wxaapi/comment/mpcommentlist/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getccommentlist.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`GET /wxaapi/comment/mpcommentlist/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `startTime` | Query `startTime` | `string` | 查询时间段开始时间（官方参数表类型 string，秒级时间戳字符串，必填）。 |
| `endTime` | Query `endTime` | `string` | 查询时间段结束时间（官方参数表类型 string，秒级时间戳字符串，必填）。 |
| `filterType` | Query `filterType` | `long?` | 过滤的数据类型（选填；取值见 `WxaCommentFilterType`）。 |
| `offset` | Query `offset` | `long?` | 偏移数（选填，官方原文「默认值为 0 代表首页拉取」）。 |
| `limit` | Query `limit` | `long?` | 每页数量（选填，官方原文「默认值为 8」）。 |
- **返回**：`DataModels.TransactionGuarantee.WxaCommentListResponse` — 评价列表（`offset` / `total` / `commentList[]`），见 `WxaCommentListResponse`。

**备注（原文）**

- 官方契约：**GET**；Query 携带 `access_token` 及本页参数表的五个业务参数（照官方示例以查询串发送）。
- **列表与详情的字段类型口径差异（官方原文如此）**：本端点 `commentList[]` 的 / `amount` / `score` 参数表类型为 `number`，而「查询评价详情」的 / `info.content.amount` / `score` 类型为 `string` ⇒ 两个 DTO 各照本页参数表落地， / 宿主不得假设两端类型一致。
- **媒体互斥（官方原文）**：`content.media[]` 中「视频跟图片只能存在一种，不同时存在； / 图片可以有多张，视频只会有一个」⇒ `img` 与 `video`/`videoCover`/`videoDuration` 互斥。

### 2. `GetCommentInfoAsync` — 查询评价详情（`GET /wxaapi/comment/commentinfo/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getcommentinfo.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`GET /wxaapi/comment/commentinfo/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `commentId` | Query `commentId` | `string` | 评价 id（官方必填）。 |
- **返回**：`DataModels.TransactionGuarantee.WxaCommentInfoResponse` — 评价详情（`info` / `processInfo` / `oldComment`），见 `WxaCommentInfoResponse`。

**备注（原文）**

- 官方契约：**GET**；Query 携带 `access_token` 与 `commentId`（官方示例 `?commentId=123456&access_token=xxx`）。
- **两个可选块的官方限定（照录）**：`processInfo`「只有差评才会有」—— / 其 `actionList[]` 为进度节点（`type`：`1` 发表差评 / `2` 开发者处理 / / `3` 用户调研 / `4` 用户改评），官方定位当前状态的规则是 / 「**从数组最后往前数，有 `updateTime` 的就是当前状态**」； / `oldComment`「只有改评的新评价才有」，其内容字段名为 `ext`（而非列表侧的 `txt`）。

### 3. `GetReplyAndCommentReplyListAsync` — 查询评论列表（`GET /wxaapi/comment/replyandcommentreplylist/get`，官方一次返回「评论 + 回复」两层）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getcommentreplylist.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`GET /wxaapi/comment/replyandcommentreplylist/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `commentId` | Query `commentId` | `string` | 评价 id（官方必填）。 |
- **返回**：`DataModels.TransactionGuarantee.WxaCommentReplyListResponse` — 评论与回复（`list.reply` + `list.commentReplyList[]`），见 `WxaCommentReplyListResponse`。

**备注（原文）**

- 官方契约：**GET**；Query 携带 `access_token` 与 `commentId`（官方示例 `?commentId=123456&access_token=xxx`）。
- **`list` 为对象而非数组（官方原文）**：官方把 `list` 建为单个对象，内含 / `reply`（评论，即「评论回复第一条内容」）与 `commentReplyList` / （回复，即「评论回复第二条及之后的所有集合」）⇒ 本 SDK 不得把 `list` 改成列表。
- **第一层的定位键是 `replyId`、第二层是 `commentReplyId`**， / 分别对应 `AddCommentReplyAsync` / `DeleteCommentReplyAsync` 的入参。

### 4. `AddReplyAsync` — 创建评论（`POST /wxaapi/comment/reply/add`，评价下的第一层内容）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_addreply.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`POST /wxaapi/comment/reply/add`

- **请求体**：`DataModels.TransactionGuarantee.WxaCommentAddRequest` — 请求体（`commentId` / `content` 必填），见 `WxaCommentAddRequest`。
- **返回**：`DataModels.TransactionGuarantee.WxaCommentOpResponse` — 操作结果（`success` + `errcode`/`errmsg`），见 `WxaCommentOpResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON（`commentId`、`content`）；Query 携带 `access_token`。
- **与 `AddCommentReplyAsync` 的分层区别**：本端点在**评价**下创建**评论** / （只需 `commentId`），回复的回复走 `commentreply/add`（还需 `replyId`）。 / 删除同理：`DeleteReplyAsync` / `DeleteCommentReplyAsync`。

### 5. `DeleteReplyAsync` — 删除评论（`POST /wxaapi/comment/reply/delete`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_deletereply.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`POST /wxaapi/comment/reply/delete`

- **请求体**：`DataModels.TransactionGuarantee.WxaCommentDeleteRequest` — 请求体（`commentId` / `replyId` 必填），见 `WxaCommentDeleteRequest`。
- **返回**：`DataModels.TransactionGuarantee.WxaCommentOpResponse` — 操作结果（`success` + `errcode`/`errmsg`），见 `WxaCommentOpResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON（`commentId`、`replyId`）；Query 携带 `access_token`。
- **删除为不可逆操作**：官方无回收站/恢复端点，定位键 `replyId` 须先经查询评论列表取得。

### 6. `AddCommentReplyAsync` — 创建回复（`POST /wxaapi/comment/commentreply/add`，评论下的追加回复）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_addcommentreply.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`POST /wxaapi/comment/commentreply/add`

- **请求体**：`DataModels.TransactionGuarantee.WxaCommentReplyAddRequest` — 请求体（`commentId` / `replyId` / `content` 必填），见 `WxaCommentReplyAddRequest`。
- **返回**：`DataModels.TransactionGuarantee.WxaCommentOpResponse` — 操作结果（`success` + `errcode`/`errmsg`），见 `WxaCommentOpResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON（`commentId`、`replyId`、`content`）；Query 携带 `access_token`。
- 本端点创建的对象在官方模型里是「回复」（`commentReplyId` 标识），归属某条**评论**（`replyId`）。

### 7. `DeleteCommentReplyAsync` — 删除回复（`POST /wxaapi/comment/commentreply/delete`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_deletecommentreply.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`POST /wxaapi/comment/commentreply/delete`

- **请求体**：`DataModels.TransactionGuarantee.WxaCommentReplyDeleteRequest` — 请求体（`commentId` / `replyId` / `commentReplyId` 三者必填），见 `WxaCommentReplyDeleteRequest`。
- **返回**：`DataModels.TransactionGuarantee.WxaCommentOpResponse` — 操作结果（`success` + `errcode`/`errmsg`），见 `WxaCommentOpResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON（三个 id 全必填）；Query 携带 `access_token`。
- **定位键为三层全链（官方原文）**：本端点要求同时传 `commentId`、`replyId`、 / `commentReplyId`，而非仅最内层 id ⇒ 宿主需保留父层 id，或先调查询评论列表重取。

### 8. `ConfirmCompromiseAsync` — 确认和解（`POST /wxaapi/comment/confirmcompromise`，差评挽回流程的商家动作）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_confirmcompromise.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`POST /wxaapi/comment/confirmcompromise`

- **请求体**：`DataModels.TransactionGuarantee.WxaCommentCompromiseRequest` — 请求体（`commentId` / `picList` / `content` 必填），见 `WxaCommentCompromiseRequest`。
- **返回**：`DataModels.TransactionGuarantee.WxaCommentOpResponse` — 操作结果（`success` + `errcode`/`errmsg`），见 `WxaCommentOpResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON（`commentId`、`picList`、`content`）；Query 携带 `access_token`。
- **`picList` 为 mediaId 集合（官方原文「和解的图片 mediaId 集合，可参考素材管理」）** / ⇒ 图片须先上传取得 mediaId，不能直接传 URL。
- **业务前置（由错误码反推，官方原文）**：`10000` 在和解场景的含义是 / 「不是差评 / 发送过模板消息」⇒ 仅**差评**且**尚未发起过差评客服会话模板消息**的评价可确认和解； / 差评可和解的判定口径亦可用列表侧 `extInfo.isAlreadySendTmpl` 预判。

### 9. `ResetApiKfQuotaAsync` — 重置 API 客服 quota（`POST /wxaapi/comment/apikfquota/reset`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_resetapikfquota.html。

- **声明接口**：[IWxaTradeCommentService](./IWxaTradeCommentService.cs)
- **官方契约**：`POST /wxaapi/comment/apikfquota/reset`

- **请求体**：`DataModels.TransactionGuarantee.WxaCommentKfQuotaResetRequest` — 请求体（`commentId` 必填，单字段），见 `WxaCommentKfQuotaResetRequest`。
- **返回**：`DataModels.TransactionGuarantee.WxaCommentOpResponse` — 操作结果（`success` + `errcode`/`errmsg`），见 `WxaCommentOpResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON（仅 `commentId`）；Query 携带 `access_token`。
- **与差评客服会话的关联**：官方把本端点归入评价管理，语义为按评价重置其 API 客服会话额度； / 客服侧额度与消息规则属 Kf 域（`AddKfApi()`），本 SDK 不跨域推断，调用方须按官方页面语义使用。

### 10. `GetComplaintOrderDetailAsync` — 查询投诉单详情（`GET /wxaapi/minishop/complaintOrderDetail`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_getorderdetail.html。

- **声明接口**：[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)
- **官方契约**：`GET /wxaapi/minishop/complaintOrderDetail`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintOrderId` | Query `complaintOrderId` | `string` | 投诉单 id（官方参数表类型 string，必填）。 |
- **返回**：`DataModels.TransactionGuarantee.WxaComplaintDetailResponse` — 投诉单详情（`complaintOrder` / `item` / `returnBill`），见 `WxaComplaintDetailResponse`。

**备注（原文）**

- 官方契约：**GET**；Query 携带 `access_token` 与 `complaintOrderId`（官方在 GET 端点下给出「请求体」表，但按示例以查询串发送，同域裁决见 `WxaTradePenaltyListResponse` 注）。
- **本端点是其余 4 个动作端点的定位键来源（官方原文）**：`returnBill.returnId` / 「需要通过查询投诉单详情接口获取」；商家动作前应以此接口为准读取 `status` 与 `expireTime`。
- **图片链接有时效**：`customerMaterial.mediaIdList` 官方明示「该图片 url 有过期时间， / 如需查看图片，需每次重新调用该接口获取」⇒ 不得缓存复用。
- 官方错误码：`-1`（system error，系统繁忙稍后再试）。

### 11. `RespondComplaintAsync` — 商家回应投诉（`POST /wxaapi/minishop/bussiRespondComplaint`，同意 / 拒绝和解）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_respondcomplaint.html。

- **声明接口**：[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)
- **官方契约**：`POST /wxaapi/minishop/bussiRespondComplaint`

- **请求体**：`DataModels.TransactionGuarantee.WxaComplaintRespondRequest` — 请求体（`content` 与 `mediaIdList` 二选一、`complaintOrderId`、`bussiHandle`），见 `WxaComplaintRespondRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`（权限集 id 76 可代调用）。
- **回应后进入用户确认段**：`bussiHandle` 取 `1` 同意和解 / `2` 拒绝和解 / （见 `WxaComplaintBussiHandle`）， / 官方投诉单状态随之进入 `203`/`204`「已回应，待用户确认」， / 超时未确认会转为 `202` / `108` 等超时分支。

### 12. `SupplyProofAsync` — 商家补充凭证（`POST /wxaapi/minishop/bussiSupplyProof`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_supplyproof.html。

- **声明接口**：[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)
- **官方契约**：`POST /wxaapi/minishop/bussiSupplyProof`

- **请求体**：`DataModels.TransactionGuarantee.WxaComplaintProofRequest` — 请求体（`content` 与 `mediaIdList` 二选一 + `complaintOrderId`；与申诉端点共用类型），见 `WxaComplaintProofRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- **与「商家申诉」端点请求体完全一致**（三字段同名同类型同必填）⇒ 共用 / `WxaComplaintProofRequest`，裁决依据记于该类型 remarks。
- **适用状态**：官方进度节点 `itemType` `16`「待商家补充凭证」、 / `14`「待用户补充凭证」、`18`「平台要求双方补充凭证」； / 商家侧对应 `106`/`108` 状态。超时会转入 `112` 商家补充凭证超时。

### 13. `SupplyRefundAsync` — 商家提交退款凭证（`POST /wxaapi/minishop/bussiSupplyRefund`，退货场景的处理凭证）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_submitrefund.html。

- **声明接口**：[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)
- **官方契约**：`POST /wxaapi/minishop/bussiSupplyRefund`

- **请求体**：`DataModels.TransactionGuarantee.WxaComplaintRefundProofRequest` — 请求体（三通用字段 + 退货状态下必填的 `acceptReturn` / `returnId`），见 `WxaComplaintRefundProofRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- **退货分支专用**：当平台判责为商责且经过退货（进度 `itemType` 31/32 且 `blameResult == 1`， / 单状态 `308`/`309`「待用户退货中」）时走本端点，须回传 `returnId` / （由 `GetComplaintOrderDetailAsync` 的 `returnBill` 取得）； / 未走退货（`blameResult == 0`「待上传处理凭证」）时走 `SupplyProofAsync`。
- **官方未公布 `acceptReturn` 枚举**：参数表只给语义「确认是否收货（退货状态下必填）」， / 无枚举表 ⇒ 不建常量类（见 `WxaComplaintRefundProofRequest`）。

### 14. `AppealAsync` — 商家申诉（`POST /wxaapi/minishop/busiAppeal`，对平台判责结果提出申诉）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_busiappeal.html。

- **声明接口**：[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)
- **官方契约**：`POST /wxaapi/minishop/busiAppeal`

- **请求体**：`DataModels.TransactionGuarantee.WxaComplaintProofRequest` — 请求体（`content` 与 `mediaIdList` 二选一 + `complaintOrderId`；与补充凭证端点共用类型），见 `WxaComplaintProofRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- **申诉窗口（官方枚举）**：`complaintOrder.appealState` 为 `401`「待商家申诉」时才可发起； / `402` 已超过申诉时间、`403` 申诉中、`117` 申诉成功、`118` 申诉失败、`0` 未进入申诉阶段。
- **本端点独有的官方错误码**：`2`「传入 mediaIdList 或 content 格式有误」、 / `1002`「当前状态不支持该操作」——其余为 `0` / `1` / `10001`。
- **路由拼写照录**：本端点为 `busiAppeal`（`busi`），其余三个商家动作为 `bussi`，官方两族拼写不一致。

### 15. `GetGuaranteeStatusAsync` — 获取交易保障标状态（`GET /wxaapi/wxamptrade/get_guarantee_status`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/basic/api_getguaranteestatus.html。

- **声明接口**：[IWxaTradeGuaranteeService](./IWxaTradeGuaranteeService.cs)
- **官方契约**：`GET /wxaapi/wxamptrade/get_guarantee_status`
- **返回**：`DataModels.TransactionGuarantee.WxaTradeGuaranteeStatusResponse` — 保障标状态（`isActived` / `msg` / `reasons`），见 `WxaTradeGuaranteeStatusResponse`。

**备注（原文）**

- 官方契约：**GET**，无请求体、无业务 Query 参数；Query 仅携带 `access_token`。
- **语义（官方两个应答示例）**：`isActived` 为「是否激活交易保障标」；未激活时 / `reasons` 逐条给出未开通原因（如「经营时长小于 3 年」「保证金余额不足」）， / `msg` 为状态描述（**已激活也可能带提示**，官方示例「已开通但未引入展示组件」）。
- 官方错误码：`200002`（参数错误）。

### 16. `GetPenaltyListAsync` — 获取小程序交易体验分违规记录（`GET /wxaapi/wxamptrade/get_penalty_list`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/basic/api_getpenaltylist.html。

- **声明接口**：[IWxaTradeGuaranteeService](./IWxaTradeGuaranteeService.cs)
- **官方契约**：`GET /wxaapi/wxamptrade/get_penalty_list`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `offset` | Query `offset` | `long` | 偏移量（官方原文「获取从第 offset 条开始的 limit 条记录（序号从 0 开始），最大不超过总记录数」）。 |
| `limit` | Query `limit` | `long` | 本次读取条数（官方原文「最大不超过 100」）。 |
- **返回**：`DataModels.TransactionGuarantee.WxaTradePenaltyListResponse` — 违规记录列表（`appealList` / `currentScore` / `totalNum`），见 `WxaTradePenaltyListResponse`。

**备注（原文）**

- 官方契约：**GET**；Query 携带 `access_token` 与 `offset`、`limit`。
- **GET 带「请求体」参数表的建模裁决**：官方本页在 GET 端点下给出「请求体 Request Payload」表 / （`offset` / `limit` 均必填），但官方**代码示例**实际以查询串发送 / （`?access_token=xxx&offset=0&limit=10`）⇒ 本 SDK 按示例落地为 `[Query]`， / 不为 GET 声明请求体（全仓零 GET + `[Body]` 先例，声明后组件行为不可预期）。
- **记录状态**：`appealList[].status` 官方枚举 `2` 扣分审批通过 / `4` 申诉中 / / `5` 申述驳回（原文如此）/ `6` 申诉成功。
- 官方错误码：`20002`（参数非法）/ `200002`（参数错误）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee`](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/) 内共 **41** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **41** 个，`SerializerClassName` = `TransactionGuarantee`。

AOT 源生成上下文：[`TransactionGuaranteeJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram.DataModels/Generated/TransactionGuaranteeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaCommentListResponse` | 应答 | [TransactionGuarantee/Comment/WxaCommentListResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentListResponse.cs) | `offset`, `total`, `commentList` |
| `WxaCommentInfoResponse` | 应答 | [TransactionGuarantee/Comment/WxaCommentInfoResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentInfoResponse.cs) | `info`, `processInfo`, `oldComment` |
| `WxaCommentReplyListResponse` | 应答 | [TransactionGuarantee/Comment/WxaCommentReplyListResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentReplyListResponse.cs) | `list` |
| `WxaCommentOpResponse` | 应答 | [TransactionGuarantee/Comment/WxaCommentOpResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentOpResponse.cs) | `success` |
| `WxaCommentAddRequest` | 请求体 | [TransactionGuarantee/Comment/WxaCommentAddRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentAddRequest.cs) | `commentId`, `content` |
| `WxaCommentDeleteRequest` | 请求体 | [TransactionGuarantee/Comment/WxaCommentDeleteRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentDeleteRequest.cs) | `commentId`, `replyId` |
| `WxaCommentReplyAddRequest` | 请求体 | [TransactionGuarantee/Comment/WxaCommentReplyAddRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentReplyAddRequest.cs) | `commentId`, `replyId`, `content` |
| `WxaCommentReplyDeleteRequest` | 请求体 | [TransactionGuarantee/Comment/WxaCommentReplyDeleteRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentReplyDeleteRequest.cs) | `commentId`, `replyId`, `commentReplyId` |
| `WxaCommentCompromiseRequest` | 请求体 | [TransactionGuarantee/Comment/WxaCommentCompromiseRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentCompromiseRequest.cs) | `commentId`, `picList`, `content` |
| `WxaCommentKfQuotaResetRequest` | 请求体 | [TransactionGuarantee/Comment/WxaCommentKfQuotaResetRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Comment/WxaCommentKfQuotaResetRequest.cs) | `commentId` |
| `WxaComplaintDetailResponse` | 应答 | [TransactionGuarantee/Complaint/WxaComplaintDetailResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Complaint/WxaComplaintDetailResponse.cs) | `complaintOrder`, `item`, `returnBill` |
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaComplaintRespondRequest` | 请求体 | [TransactionGuarantee/Complaint/WxaComplaintRespondRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Complaint/WxaComplaintRespondRequest.cs) | `content`, `complaintOrderId`, `mediaIdList`, `bussiHandle` |
| `WxaComplaintProofRequest` | 请求体 | [TransactionGuarantee/Complaint/WxaComplaintProofRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Complaint/WxaComplaintProofRequest.cs) | `content`, `complaintOrderId`, `mediaIdList` |
| `WxaComplaintRefundProofRequest` | 请求体 | [TransactionGuarantee/Complaint/WxaComplaintRefundProofRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Complaint/WxaComplaintRefundProofRequest.cs) | `content`, `complaintOrderId`, `mediaIdList`, `acceptReturn`, `returnId` |
| `WxaTradeGuaranteeStatusResponse` | 应答 | [TransactionGuarantee/Basic/WxaTradeGuaranteeStatusResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Basic/WxaTradeGuaranteeStatusResponse.cs) | `isActived`, `msg`, `reasons` |
| `WxaTradePenaltyListResponse` | 应答 | [TransactionGuarantee/Basic/WxaTradePenaltyListResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/Basic/WxaTradePenaltyListResponse.cs) | `appealList`, `currentScore`, `totalNum` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddTransactionGuaranteeApi());

var service = app.Services.GetRequiredService<IWxaTradeGuaranteeService>();
var ct = CancellationToken.None;
var response = await service.GetGuaranteeStatusAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxaapi/wxamptrade/get_guarantee_status");
// 占位值仅用于编译示意；业务值须按 IWxaTradeGuaranteeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetGuaranteeStatusAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaTradeCommentService](./IWxaTradeCommentService.cs)、[IWxaTradeComplaintService](./IWxaTradeComplaintService.cs)、[IWxaTradeGuaranteeService](./IWxaTradeGuaranteeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaTransactionGuaranteeContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaTransactionGuaranteeContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)、[`MiniProgramP3RulingsContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs) |
| 守卫中的代表性测试方法 | `TransactionGuaranteeEndpoints_ShouldMatchOfficialRoutes`、`TransactionGuaranteeEndpoints_ShouldRespectGetQueryAndPostBodyShapes`、`TransactionGuaranteeInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup`、`TransactionGuaranteeDataModels_ShouldBeRegisteredInJsonContext`、`TransactionGuaranteeDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee`](../../../Mud.Wechat.MiniProgram.DataModels/TransactionGuarantee/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaTransactionGuaranteeContractGuards.cs`、`MiniProgramContractGuards.cs`、`MiniProgramP3RulingsContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
