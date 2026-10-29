# Game 域 · 小游戏

微信小程序「小游戏」域 SDK（**20 端点**：帧同步房间 4 + 用户数据存储 3 + 对局匹配规则 5 + 米大师索要单 1 + 内容安全 2 + 米大师 V2 游戏币 5）。 接口：[IWxaGameService](./IWxaGameService.cs)；注册组 `Game`；模块枚举 `MiniProgramModule.Game`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaGameContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaGameContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)、[MiniProgramNonContractRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramNonContractRouteContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/Game`](../../DataModels/Game)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaGameService = 20；按 HTTP 特性实计 **20** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaGameService` | `CreateGameRoomAsync` | POST | `/wxa/createwxagameroom` | `AccessToken`（Query），Query `access_token` | 「创建帧同步游戏房间」（`POST /wxa/createwxagameroom`，不支持第三方平台调用与云调用之外的形态）。<br>官方文档：https://developers.we… |
| 2 | `IWxaGameService` | `GetGameFrameAsync` | POST | `/wxa/getwxagameframe` | `AccessToken`（Query），Query `access_token` | 「分片拉取对局游戏帧」（**POST** `/wxa/getwxagameframe`，不支持第三方平台调用）。<br>官方文档：https://developers.weixin.qq… |
| 3 | `IWxaGameService` | `GetGameIdentityInfoAsync` | GET | `/wxa/getwxagameidentityinfo` | `AccessToken`（Query），Query `access_token` | 「获取对局玩家位次信息」（`GET /wxa/getwxagameidentityinfo`，不支持第三方平台调用）。<br>官方文档：https://developers.weixin… |
| 4 | `IWxaGameService` | `GetGameRoomInfoAsync` | GET | `/wxa/getwxagameroominfo` | `AccessToken`（Query），Query `access_token` | 「获取指定房间信息」（`GET /wxa/getwxagameroominfo`，不支持第三方平台调用）。<br>官方文档：https://developers.weixin.qq.co… |
| 5 | `IWxaGameService` | `RemoveUserStorageAsync` | POST | `/wxa/remove_user_storage` | `AccessToken`（Query），Query `access_token` | 「删除已上报KV数据」（`POST /wxa/remove_user_storage`，不支持第三方平台调用）。<br>官方文档：https://developers.weixin.qq… |
| 6 | `IWxaGameService` | `SetUserInteractiveDataAsync` | POST | `/wxa/setuserinteractivedata` | `AccessToken`（Query），Query `access_token` | 「写用户关系链互动数据存储」（`POST /wxa/setuserinteractivedata`，不支持第三方平台调用）。<br>官方文档：https://developers.wei… |
| 7 | `IWxaGameService` | `SetUserStorageAsync` | POST | `/wxa/set_user_storage` | `AccessToken`（Query），Query `access_token` | 「上报用户数据」（`POST /wxa/set_user_storage`，不支持第三方平台调用）。<br>官方文档：https://developers.weixin.qq.com/m… |
| 8 | `IWxaGameService` | `CreateMatchRuleAsync` | POST | `/wxa/business/gamematch/creatematchrule` | `AccessToken`（Query），Query `access_token` | 「创建对局匹配规则」（`POST /wxa/business/gamematch/creatematchrule`，不支持第三方平台调用）。<br>官方文档：https://develo… |
| 9 | `IWxaGameService` | `DeleteMatchRuleAsync` | POST | `/wxa/business/gamematch/deletematchrule` | `AccessToken`（Query），Query `access_token` | 「删除对局匹配规则」（`POST /wxa/business/gamematch/deletematchrule`，不支持第三方平台调用）。<br>官方文档：https://develo… |
| 10 | `IWxaGameService` | `GetAllMatchRuleAsync` | POST | `/wxa/business/gamematch/getallmatchrule` | `AccessToken`（Query），Query `access_token` | 「获取对局匹配规则」（`POST /wxa/business/gamematch/getallmatchrule`，不支持第三方平台调用）。<br>官方文档：https://develo… |
| 11 | `IWxaGameService` | `SetMatchIdOpenStateAsync` | POST | `/wxa/business/gamematch/setmatchopenstate` | `AccessToken`（Query），Query `access_token` | 「修改对局规则打开状态」（`POST /wxa/business/gamematch/setmatchopenstate`，不支持第三方平台调用）。<br>官方文档：https://de… |
| 12 | `IWxaGameService` | `UpdateMatchRuleAsync` | POST | `/wxa/business/gamematch/updatematchrule` | `AccessToken`（Query），Query `access_token` | 「修改对局匹配规则」（`POST /wxa/business/gamematch/updatematchrule`，不支持第三方平台调用）。<br>官方文档：https://develo… |
| 13 | `IWxaGameService` | `GetPayForOrderAsync` | POST | `/wxa/business/getpayfororder` | `AccessToken`（Query），Query `access_token` | 「查询索要单」（`POST /wxa/business/getpayfororder`，官方页面顶部标注**接口已废弃**）。<br>官方文档：https://developers.we… |
| 14 | `IWxaGameService` | `MsgSecCheckAsync` | POST | `/wxa/game/content_spam/msg_sec_check` | `AccessToken`（Query），Query `access_token` | 「文本内容安全识别」（`POST /wxa/game/content_spam/msg_sec_check`，不支持第三方平台调用；云调用 `wxa.game.contentSpa… |
| 15 | `IWxaGameService` | `MediaCheckAsync` | POST | `/wxa/game/content_spam/media_check_sync` | `AccessToken`（Query），Query `access_token` | 「图片内容安全识别」（`POST /wxa/game/content_spam/media_check_sync`，不支持第三方平台调用；云调用 `wxa.game.content… |
| 16 | `IWxaGameService` | `GetBalanceAsync` | POST | `/wxa/game/getbalance` | `AccessToken`（Query），Query `access_token` | 「查询游戏币余额」（`POST /wxa/game/getbalance`，不支持第三方平台调用；云调用 `wxa.game.getBalance`）。<br>官方文档：https://… |
| 17 | `IWxaGameService` | `PayAsync` | POST | `/wxa/game/pay` | `AccessToken`（Query），Query `access_token` | 「扣除游戏币」（`POST /wxa/game/pay`，不支持第三方平台调用；云调用 `wxa.game.pay`）。<br>官方文档：https://developers.weixi… |
| 18 | `IWxaGameService` | `CancelPayAsync` | POST | `/wxa/game/cancelpay` | `AccessToken`（Query），Query `access_token` | 「退回扣除游戏币」（`POST /wxa/game/cancelpay`，不支持第三方平台调用；云调用 `wxa.game.cancelPay`）。<br>官方文档：https://de… |
| 19 | `IWxaGameService` | `PresentAsync` | POST | `/wxa/game/present` | `AccessToken`（Query），Query `access_token` | 「给用户赠送游戏币」（`POST /wxa/game/present`，不支持第三方平台调用；云调用 `wxa.game.present`）。<br>官方文档：https://devel… |
| 20 | `IWxaGameService` | `QueryOrderAsync` | POST | `/wxa/game/queryorderinfo` | `AccessToken`（Query），Query `access_token` | 「查询订单状态」（`POST /wxa/game/queryorderinfo`，不支持第三方平台调用，且**本域五支 pay_v2 中唯一不支持云调用**的一支）。<br>官方文档：h… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**令牌面**：本域全部端点使用**小程序 access_token**（Query 注入，`MpTokenTypes.AccessToken`），
与小程序基础线同构；官方 20 页「第三方调用」一节**逐页写明不支持第三方平台调用** ⇒ 本域**没有**
`component_access_token` 代调用形态（与云开发域恰好相反）。

**第二重凭据面**：官方另有 8 支端点在 `access_token` 之外强制把**宿主计算**的签名放 Query ——
数据存储三支（`openid` + `signature` + `sig_method`）与米大师 V2 五支
（`signature` + `sig_method` + `pay_sig`）。三者都是方法级 `[Query]` 业务参数、
不属令牌注入面 ⇒ 不改 `[Token]` 声明。算法口径（官方签名页
https://developers.weixin.qq.com/minigame/dev/guide/open-ability/virtual-payment/signature.html）：
`signature = hex(hmac_sha256(session_key, post_body))`；
`pay_sig = hex(hmac_sha256(app_key, uri + '&' + post_body))`，其中 `uri` **只取路径、
必须舍去 `?` 及之后部分**，`app_key` 取与请求 `env` 对应的「mp-支付基础配置」AppKey，
且 `post_body` 必须与实际发出的请求体**逐字节一致**（补一个空格即 `90010`/`90011`）；
`sig_method` 官方唯一取值 `HmacSha256`。
**敏感值**：`signature` / `pay_sig` / `session_key` / 支付 `app_key` 一律
**不得进日志、遥测或异常消息**。`pay_sig` 不在组件精确匹配脱敏词表内 ⇒ 由 `AddGameApi()` 的注册委托
复用 `XPaySensitiveQueryKeys.RegisterAll()` 登记（键集单一来源，守卫 GM6 锁定该委托体真的调用登记）。

**定位键**：帧同步房间面**没有 room_id 入参** —— 四支接口里三支以 `access_info`
（`createGameRoom` 返回的 `data.access_info`，即 `onGameEnd` 侧的 gameAccessInfo）定位对局。

**官方文档笔误留档**（本域按「逐字照抄 + 不臆造」处置，均由守卫 GM1/GM4 锁定）：
① `setMatchIdOpenState` 的接口名与实际路径不一致，实际为 `/wxa/business/gamematch/setmatchopenstate`；
② `getAllMatchRule` 返回表把 `need_room_service_info` 那行名字误写成第二个 `team_member_count`
（返回示例用前者 ⇒ DTO 取 `need_room_service_info`）；
③ `setMatchIdOpenState` / `updateMatchRule` 的 `match_id` 类型列标 number，
而请求示例是 44 位字符串 ⇒ 按线格式取 string；
④ `updateMatchRule` 页面正文参数表只有 `match_id` + `open_state` 两支（与「修改匹配规则」的语义
不吻合，疑为复用状态页表格）⇒ 不臆造 `team_count` / `game_room_info` 等未出现的参数，
与 `setMatchIdOpenState` 共用同一请求类型；
⑤ `getGameFrame` 请求体表内重复列出一行「access_token / cloudbase_access_token」，与查询参数同义 ⇒
HTTP 形态只走 Query 的 `access_token`；
⑥ `business.getPayForOrder` 页面顶部标注**「接口已废弃」**但正文完整可读 ⇒ 判为在册、按 20 端点全量落地，
废弃状态在本注释与守卫 remarks 留档（不因此删除端点：官方未下架页面，且清单口径是 minigame 树全量）。

**频率限制**：本域 20 页里只有内容安全两支给出端点级数字（文本 **1 万/分钟、1000 万/天**，
同步耗时约 500ms、建议超时 1s；图片 **2 千/分钟、20 万/天**、单文件 ≤10M，同步耗时约 3s、建议超时 5s；
超限官方回 `750032` / `750033`）。其余 18 页「注意事项」写明无特殊注意事项 ⇒ 注释留档该空缺。

**落位**：`Interfaces/Game/IWxaGameService.cs`；`MiniProgramModule.Game`；
注册组 `Game`（守卫 GM1~GM7）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateGameRoomAsync` — 「创建帧同步游戏房间」（`POST /wxa/createwxagameroom`，不支持第三方平台调用与云调用之外的形态）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.createGameRoom.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/createwxagameroom`

- **请求体**：`GameCreateRoomRequest` — 请求体（必填四支 `game_tick` / `open_id_list` / `udp_reliability_strategy` / `start_percent`），见 `GameCreateRoomRequest`。
- **返回**：`GameCreateRoomResponse` — 房间访问凭证 `data.access_info`，见 `GameCreateRoomResponse`。

**备注（原文）**

- **返回的是 access_info，不是房间号**：本域房间面三支查询接口都以它定位对局。
- **官方必填口径**：`game_tick` 不得小于 33ms；`room_ext_info` ≤32 字节（超长回 `4013`， / 请求数据非法回 `4003`）；`need_user_info` 为 true 时未授权用户**加入房间直接失败**（不是「加入但不下发昵称」）。
- **官方请求示例未列 `open_id_list`**（参数表列其为必填）⇒ 以参数表为准。

### 2. `GetGameFrameAsync` — 「分片拉取对局游戏帧」（**POST** `/wxa/getwxagameframe`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameFrame.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/getwxagameframe`

- **请求体**：`GameGetFrameRequest` — 请求体（`access_info` + 帧区间 `begin_frame_id` / `end_frame_id`，官方全部必填），见 `GameGetFrameRequest`。
- **返回**：`GameGetFrameResponse` — 帧列表与 `has_more`，见 `GameGetFrameResponse`。

**备注（原文）**

- **动词是 POST**：本域「读」类端点里最易想当然的一支——官方请求体是 JSON 对象， / `begin_frame_id` / `end_frame_id` 落 Body 而非 Query；帧区间是**左闭右开**。
- **帧号从 1 开始**；`pkg_list[].b` 为 true 表示空帧，`dp_id` 是微信内部字段 / （官方原文「无需理解，为了更快速返回数据没有去掉该字段内容」）⇒ 不得据此做业务判断。
- 错误码：`1` 数据不存在 / `5` 非法的请求数据 / `6000` 获取数据失败。页面标注「本接口无代码示例」。

### 3. `GetGameIdentityInfoAsync` — 「获取对局玩家位次信息」（`GET /wxa/getwxagameidentityinfo`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameIdentityInfo.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`GET /wxa/getwxagameidentityinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accessInfo` | Query `access_info` | `string` | 房间/对局访问凭证（官方 Query 参数 `access_info`，必填）。 |
- **返回**：`GameGetIdentityInfoResponse` — 每名玩家的 `client_id` 与 `open_id`，见 `GameGetIdentityInfoResponse`。

**备注（原文）**

- **请求体为「无」**：官方请求体表明确写「无」⇒ `access_info` 只能是 Query 参数， / 不得为它造 request DTO。
- **`client_id` 不是座位号**：官方原文「一旦分配不能修改」，座位号在 / `GetGameRoomInfoAsync` 的 `member_list[].pos_num`。
- 错误码：`1` 数据不存在。

### 4. `GetGameRoomInfoAsync` — 「获取指定房间信息」（`GET /wxa/getwxagameroominfo`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameRoomInfo.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`GET /wxa/getwxagameroominfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accessInfo` | Query `access_info` | `string` | 房间/对局访问凭证（官方 Query 参数 `access_info`，必填；官方原文「不同于 access_token」）。 |
- **返回**：`GameGetRoomInfoResponse` — 房间状态 + 对局参数 + 成员列表（本域最密集的返回体），见 `GameGetRoomInfoResponse`。

**备注（原文）**

- **请求体为「无」**：同上，只有 Query 的 `access_token` + `access_info`。
- **房间状态四值**：1 组队中 / 2 对局已开始 / 3 对局已结束 / 4 房间已销毁 ⇒ 轮询须按 4 终止。
- **`headimg` / `nickname` 仅用户授权才下发**（与 `need_user_info` 联动）⇒ / 空值不是数据缺失，不得当成错误重试。
- 页面标注「本接口无代码示例」；错误码只有 `-1` / `0`。

### 5. `RemoveUserStorageAsync` — 「删除已上报KV数据」（`POST /wxa/remove_user_storage`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.removeUserStorage.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/remove_user_storage`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Query `openid` | `string` | 用户 openid（官方强制 Query，必填）。 |
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填，由宿主以 `session_key` 计算）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |

- **请求体**：`GameRemoveUserStorageRequest` — 请求体（官方参数名是**单数** `key`、类型为字符串数组），见 `GameRemoveUserStorageRequest`。
- **返回**：`WxaResponse` — 只有 errcode / errmsg 的通用应答，见 `WxaResponse`。

**备注（原文）**

- **三支存储端点的 Query 形状完全相同**（`openid` + `signature` + `sig_method` 均官方强制）， / 且都是**路由带下划线**的 `remove_user_storage` / `set_user_storage`，与同族的 `setuserinteractivedata` 连写不同 ⇒ / 照抄原文不得顺手统一。
- **参数名单数 `key`**：官方请求示例 `["gold", "score"]`，不是 `keys`。

### 6. `SetUserInteractiveDataAsync` — 「写用户关系链互动数据存储」（`POST /wxa/setuserinteractivedata`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserInteractiveData.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/setuserinteractivedata`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Query `openid` | `string` | 用户 openid（官方强制 Query，必填）。 |
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |

- **请求体**：`GameSetUserInteractiveDataRequest` — 请求体（`kv_list[].key` 目前可为 '1' - '50'），见 `GameSetUserInteractiveDataRequest`。
- **返回**：`WxaResponse` — 只有 errcode / errmsg 的通用应答，见 `WxaResponse`。

**备注（原文）**

- **请求体与 `SetUserStorageAsync` 同形**（`kv_list` / `key` / `value`）， / 但路由与语义是「关系链互动数据」，两支不得互相替代。
- 错误码：`87081` 非法的 openid / `87082` 非法的 key。

### 7. `SetUserStorageAsync` — 「上报用户数据」（`POST /wxa/set_user_storage`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserStorage.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/set_user_storage`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Query `openid` | `string` | 用户 openid（官方强制 Query，必填）。 |
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |

- **请求体**：`GameSetUserStorageRequest` — 请求体（`kv_list[].key` / `value`），见 `GameSetUserStorageRequest`。
- **返回**：`WxaResponse` — 只有 errcode / errmsg 的通用应答，见 `WxaResponse`。

**备注（原文）**

- **官方四条托管数据硬限制**（本域唯一给出实质「注意事项」的一页）：每个 openid 托管数据 / **不超过 128 个 key-value 对**（超限 `87018`）；上报列表里**每一项 key+value 长度不超过 1K（1024）字节** / （超限 `87016`）；**每个 key 长度不超过 128 字节**（超限 `87019`）；用户存储数据量超限 `87017`。
- **排行榜形态**：要把数据显示在小游戏中心排行榜时，`value` 须是顶层含 `wxgame` 的 JSON Object / 序列化字符串，`wxgame` 内为 `score`（Int32，必填）与 `update_time`（Int64，必填）， / 且 `wxgame` 下**不可自定义字段**（同级可自定义）⇒ 与参数表的 number 标注并存， / 故 `Value` 取 `JsonElement?`。

### 8. `CreateMatchRuleAsync` — 「创建对局匹配规则」（`POST /wxa/business/gamematch/creatematchrule`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.createMatchRule.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/business/gamematch/creatematchrule`

- **请求体**：`GameCreateMatchRuleRequest` — 请求体（`team_count` / `team_member_count` 必填，可带 `need_room_service_info` + `game_room_info`），见 `GameCreateMatchRuleRequest`。
- **返回**：`GameMatchRuleIdResponse` — 新建的 `match_id`，见 `GameMatchRuleIdResponse`。

**备注（原文）**

- **规模上限**：每个小游戏持有的 matchid 上限 **20**（超限 `500014`）； / `need_room_service_info` 填 1 时要求 `team_count × team_member_count` **不超过 10 人** / （超限 `500008`）⇒ 这两条是调用方须自证的规模约束，官方不静默截断。
- 返回的 `match_id` 是加入匹配时必须携带的匹配池 id；请求体无签名类计算参数。

### 9. `DeleteMatchRuleAsync` — 「删除对局匹配规则」（`POST /wxa/business/gamematch/deletematchrule`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.deleteMatchRule.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/business/gamematch/deletematchrule`

- **请求体**：`GameDeleteMatchRuleRequest` — 请求体（`match_id` 必填，官方放 Body 而非 Query），见 `GameDeleteMatchRuleRequest`。
- **返回**：`WxaResponse` — 只有 errcode / errmsg 的通用应答，见 `WxaResponse`。

**备注（原文）**

- **删除即释放匹配池**：官方原文提示线上正在使用的 matchid 须谨慎 ⇒ 这是不可逆操作， / 调用方应自行做「在用」校验。
- 错误码：`1` matchid 不存在 / `500005` 无效的 matchid / `200000` 无效的请求数据。

### 10. `GetAllMatchRuleAsync` — 「获取对局匹配规则」（`POST /wxa/business/gamematch/getallmatchrule`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.getAllMatchRule.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/business/gamematch/getallmatchrule`
- **返回**：`GameGetAllMatchRuleResponse` — 本小游戏全部 matchid 的规则与打开状态，见 `GameGetAllMatchRuleResponse`。

**备注（原文）**

- **官方是 POST 且请求体为「无」**：本域「查询」类端点同样照官方写 POST，且不得凭空造 request DTO。
- **官方返回表笔误**：`team_member_count` 出现两次，第二行描述实为 / `need_room_service_info`，返回示例亦用后者 ⇒ `NeedRoomServiceInfo` / 取真实字段名（守卫 GM4 锁定）。

### 11. `SetMatchIdOpenStateAsync` — 「修改对局规则打开状态」（`POST /wxa/business/gamematch/setmatchopenstate`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.setMatchIdOpenState.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/business/gamematch/setmatchopenstate`

- **请求体**：`GameMatchOpenStateRequest` — 请求体（`match_id` + `open_state` 0 关闭 / 1 打开），见 `GameMatchOpenStateRequest`。
- **返回**：`GameMatchRuleIdResponse` — 回显 `match_id`，见 `GameMatchRuleIdResponse`。

**备注（原文）**

- **实际路径与接口英文名不一致**：页面/接口名是 `setMatchIdOpenState`， / 官方请求地址正文写 `setmatchopenstate` ⇒ 路由照抄请求地址，不得按英文名拼路径。
- **设为 0（关闭）会释放匹配池**，官方原文提示线上使用中的 matchid 需谨慎。
- 错误码：`500012` open_state 的值不合法 / `500005` 无效的 matchid。

### 12. `UpdateMatchRuleAsync` — 「修改对局匹配规则」（`POST /wxa/business/gamematch/updatematchrule`，不支持第三方平台调用）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.updateMatchRule.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/business/gamematch/updatematchrule`

- **请求体**：`GameMatchOpenStateRequest` — 请求体（官方本页参数表只有 `match_id` + `open_state`，与 `SetMatchIdOpenStateAsync` 共用类型），见 `GameMatchOpenStateRequest`。
- **返回**：`GameMatchRuleIdResponse` — 回显 `match_id`，见 `GameMatchRuleIdResponse`。

**备注（原文）**

- **只能改状态为关闭的 matchid**（状态经 `SetMatchIdOpenStateAsync` 变更）。
- **官方文档疑似复用**：本页「请求体 Request Payload」表实际只列 `match_id` 与 `open_state` 两支， / 与页面标题「修改匹配规则」的语义不吻合 ⇒ 按逐字照抄原则**不臆造** `team_count` / / `game_room_info` 等正文未出现的参数，并与状态页共用同一请求类型（守卫 GM4 登记该共用裁决）。 / 若日后官方补齐该页参数表，须新建独立请求类型而非改共用的这支。

### 13. `GetPayForOrderAsync` — 「查询索要单」（`POST /wxa/business/getpayfororder`，官方页面顶部标注**接口已废弃**）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/business.getPayForOrder.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/business/getpayfororder`

- **请求体**：`GameGetPayForOrderRequest` — 请求体（`appid` / `order_no` / `out_trade_no` 三支官方全部必填），见 `GameGetPayForOrderRequest`。
- **返回**：`GameGetPayForOrderResponse` — 索要单信息（状态七值 + 沙箱标记 + 支付时间），见 `GameGetPayForOrderResponse`。

**备注（原文）**

- **废弃但在册**：页面标注「接口已废弃」，但文档正文完整渲染、参数表与错误码俱在，且本域口径是 / minigame 树 20 端点全量 ⇒ 保留建模（裁决见接口 remarks 第 ⑥ 条）。索要单不存在官方回 `-10000404`。
- **状态七值**：1 待支付 / 2 已支付 / 3 已超时 / 4 米大师已发放游戏币 / 5 游戏业务已发货 / / 6 游戏业务发货失败 ⇒ `4` 只代表米大师侧完成，`5` 才是业务侧发货成功；`pay_time` 仅支付后存在。
- `order_no`（微信索要单号）与 `out_trade_no`（游戏业务单号）是两支不同的单号，均必填。

### 14. `MsgSecCheckAsync` — 「文本内容安全识别」（`POST /wxa/game/content_spam/msg_sec_check`，不支持第三方平台调用；云调用 `wxa.game.contentSpam.msgSecCheck`）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.msgSecCheck.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/content_spam/msg_sec_check`

- **请求体**：`GameMsgSecCheckRequest` — 请求体（`openid` / `version` 固定 2 / `scene` 1~5 / `content` ≤2500 字），见 `GameMsgSecCheckRequest`。
- **返回**：`GameMsgSecCheckResponse` — 综合结果 + 明细策略列表，见 `GameMsgSecCheckResponse`。

**备注（原文）**

- **频率与耗时（本域仅有的端点级数字之一）**：单 AppID **1 万/分钟、1000 万/天** / （可申请上调），同步返回、一般耗时 500ms 内 ⇒ **建议超时 1s**；超限回 `750032` / `750033`。
- **与公众号 / 小程序内容安全线互不替代**：本支是游戏侧路由 `/wxa/game/content_spam/`， / `version` 固定 2（错回 `750030`）、场景值错回 `40129`、非 POST 回 `43002`、 / appid 与 openid 不匹配回 `43104`、该游戏未开通回 `750031`。
- **明细项级 errcode**：`detail[].errcode` 为 0 时该项才有效 ⇒ 不能只看接口级 errcode。 / 官方返回示例出现表格未登记的 `level` 字段 ⇒ 按参数表建模、不凭空补字段。

### 15. `MediaCheckAsync` — 「图片内容安全识别」（`POST /wxa/game/content_spam/media_check_sync`，不支持第三方平台调用；云调用 `wxa.game.contentSpam.mediaCheckSync`）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.mediaCheck.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/content_spam/media_check_sync`

- **请求体**：`GameMediaCheckRequest` — 请求体（`openid` / `version` 固定 2 / `scene` / `media_url` / `media_type` 固定 2 图片），见 `GameMediaCheckRequest`。
- **返回**：`GameMediaCheckResponse` — 综合结果 + 明细策略列表（相对文本支无 replaced_content / keyword），见 `GameMediaCheckResponse`。

**备注（原文）**

- **接口英文名与路由不同形**：英文名 `gamesecurity.mediaCheck`，实际路径是 / `media_check_sync` ⇒ 照抄请求地址。
- **频率与耗时**：单 AppID **2 千/分钟、20 万/天**，单个文件 ≤10M（可申请上调）， / 同步耗时约 3s ⇒ **建议超时 5s**；`media_type` 非法回 `750034`。
- **图片链接必须可被检测服务器下载**；官方另建议资料场景的头像定期重检（头像可被换）。

### 16. `GetBalanceAsync` — 「查询游戏币余额」（`POST /wxa/game/getbalance`，不支持第三方平台调用；云调用 `wxa.game.getBalance`）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.getBalance.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/getbalance`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填；`hex(hmac_sha256(session_key, post_body))`）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |
| `paySig` | Query `pay_sig` | `string` | 支付请求签名（官方 Query 参数 `pay_sig`，必填；`hex(hmac_sha256(app_key, uri + '&' + post_body))`，`uri` 不带 `?` 之后部分）。 |

- **请求体**：`GameGetBalanceRequest` — 请求体（`openid` / `offer_id` / `ts` / `zone_id` / `env` / `user_ip` 全部必填，`coin_type` 选填），见 `GameGetBalanceRequest`。
- **返回**：`GameGetBalanceResponse` — 总余额 + 赠送余额 + 四项累计值 + 首充标记，见 `GameGetBalanceResponse`。

**备注（原文）**

- **官方是 POST**（「查询」二字极易写成 GET）；请求体**没有** amount / description 类字段， / `balance` 是响应字段。
- **签名即业务前置**：`90010` signature 错误 / `90011` pay_sig 错误 / / `90016` sessionkey 过期须重走登录流程 ⇒ 三支 Query 签名缺一或 `post_body` 与实际发出的 body / 不逐字节一致即失败。
- **到账延迟**：官方说明某些情况下支付后余额延迟到账，需按间隔定期查询 ⇒ 不得把一次查询为 0 当失败。
- **适用范围**：官方标「小游戏（仅认证）」。

### 17. `PayAsync` — 「扣除游戏币」（`POST /wxa/game/pay`，不支持第三方平台调用；云调用 `wxa.game.pay`）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.pay.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/pay`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |
| `paySig` | Query `pay_sig` | `string` | 支付请求签名（官方 Query 参数 `pay_sig`，必填；`uri` 取 `/wxa/game/pay`）。 |

- **请求体**：`GamePayRequest` — 请求体（六支公共字段 + `amount` > 0 + `bill_no`，可选 `payitem` / `remark`），见 `GamePayRequest`。
- **返回**：`GamePayResponse` — 订单号 + 扣款后余额 + 本次消耗的赠送币，见 `GamePayResponse`。

**备注（原文）**

- **本端点是「扣币」而非下单收款**：收款走充值链路，本支只消耗用户已充值的代币余额。
- **幂等是官方给的恢复手段**：`bill_no` 在 openid + zoneid 下唯一；同一 `bill_no` / 48 小时内幂等；接口超时或 `errcode = -1` 时订单状态未知 ⇒ **必须用相同 bill_no 重试**， / 直到拿到 `90012`（订单号重复，即已扣成功）或非 -1 的明确失败。换单号重试就是重复扣款。
- `bill_no` 约束（2.0 新增）：长度 ≤63，仅限数字 / 大小写字母 / `*` / `-`，且不得以 `*` 开头。
- `90013` 余额不足是业务失败而非系统错误 ⇒ 重试无意义。

### 18. `CancelPayAsync` — 「退回扣除游戏币」（`POST /wxa/game/cancelpay`，不支持第三方平台调用；云调用 `wxa.game.cancelPay`）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.cancelPay.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/cancelpay`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |
| `paySig` | Query `pay_sig` | `string` | 支付请求签名（官方 Query 参数 `pay_sig`，必填；`uri` 取 `/wxa/game/cancelpay`）。 |

- **请求体**：`GameCancelPayRequest` — 请求体（公共六支 + 本次 `amount` + 本次 `bill_no` + 原扣除单 `pay_bill_no`），见 `GameCancelPayRequest`。
- **返回**：`GamePayResponse` — 订单号 + 变动后余额 + 本次赠送币数量，见 `GamePayResponse`。

**备注（原文）**

- **两个单号同时必填，极易漏**：`bill_no` 是本次退回单号、`pay_bill_no` 是被退回的原扣除单号。
- **部分退可多次**：须在原扣除订单有效时间内，且累计退回额不超过原扣除额。
- **官方文档复用件留档**：本页请求/返回示例是从扣除页复制的（示例体含 `payitem` / `remark` / 且缺 `pay_bill_no`），与本页参数表不一致 ⇒ 以参数表为准；应答表亦与扣除页同文（含「扣款后的余额」措辞）， / 故与 `PayAsync` 共用 `GamePayResponse`。

### 19. `PresentAsync` — 「给用户赠送游戏币」（`POST /wxa/game/present`，不支持第三方平台调用；云调用 `wxa.game.present`）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.present.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/present`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |
| `paySig` | Query `pay_sig` | `string` | 支付请求签名（官方 Query 参数 `pay_sig`，必填；`uri` 取 `/wxa/game/present`）。 |

- **请求体**：`GamePresentRequest` — 请求体（公共六支 + `amount` 不能为 0 + `bill_no`），见 `GamePresentRequest`。
- **返回**：`GamePresentResponse` — 赠送订单号 + 赠送后余额（无 used_present_amount），见 `GamePresentResponse`。

**备注（原文）**

- **与扣币页的两处差异**：参数表**没有** `payitem` / `remark`（故赠送语义无处放备注）， / 应答只有 `bill_no` + `balance` ⇒ 单独建模，不与 `GamePayResponse` 共用。
- `90012` 在本页官方描述为「订单已存在」（与扣币页同码不同措辞）⇒ 按「已赠送成功」处理。
- 本页错误码表**没有** `90000`（订单不存在）与 `90013`（余额不足）。

### 20. `QueryOrderAsync` — 「查询订单状态」（`POST /wxa/game/queryorderinfo`，不支持第三方平台调用，且**本域五支 pay_v2 中唯一不支持云调用**的一支）。
官方文档：https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.queryOrder.html。

- **声明接口**：[IWxaGameService](./IWxaGameService.cs)
- **官方契约**：`POST /wxa/game/queryorderinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `signature` | Query `signature` | `string` | 用户登录态签名（官方 Query 参数 `signature`，必填）。 |
| `sigMethod` | Query `sig_method` | `string` | 签名哈希方法（官方 Query 参数 `sig_method`，必填；固定 `HmacSha256`）。 |
| `paySig` | Query `pay_sig` | `string` | 支付请求签名（官方 Query 参数 `pay_sig`，必填；`uri` 取 `/wxa/game/queryorderinfo`——官方签名页枚举列表未单列本支，按同规则取路径部分）。 |

- **请求体**：`GameQueryOrderRequest` — 请求体（公共五支 + `out_trade_no` + `biz_id`；**官方参数表无 `user_ip`**），见 `GameQueryOrderRequest`。
- **返回**：`GameQueryOrderResponse` — 支付状态 / 发货状态 / 完成时间 / 双侧单号，见 `GameQueryOrderResponse`。

**备注（原文）**

- **接口英文名与路径不同形**：`pay_v2.queryOrder` → 实际路径 `queryorderinfo`； / 返回是**扁平结构**，无 `data.order_info[]` 包装。
- **定位键是 `out_trade_no` + `biz_id`**（不是 `bill_no`）； / `biz_id` 必填，1 代币 / 2 道具直购；未找到回 `90019`。
- **`pay_state` 与 `deliver_state` 是两个维度**：已支付（2）不等于已发货（2）⇒ / 补发货判定须看 `deliver_state`。官方请求示例缺 `zone_id` 等必填项，以参数表为准。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/Game`](../../DataModels/Game) 内共 **46** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **46** 个，`SerializerClassName` = `Game`。

AOT 源生成上下文：[`GameJsonContext.g.cs`](./../../DataModels/Generated/GameJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GameCreateRoomResponse` | 应答 | [Game/LockStep/GameCreateRoomResponse.cs](../../DataModels/Game/LockStep/GameCreateRoomResponse.cs) | `data` |
| `GameCreateRoomRequest` | 请求体 | [Game/LockStep/GameCreateRoomRequest.cs](../../DataModels/Game/LockStep/GameCreateRoomRequest.cs) | `game_tick`, `open_id_list`, `udp_reliability_strategy`, `start_percent`, `need_user_info`, `game_last_time`, `room_ext_info`, `need_game_seed` |
| `GameGetFrameResponse` | 应答 | [Game/LockStep/GameGetFrameResponse.cs](../../DataModels/Game/LockStep/GameGetFrameResponse.cs) | `data` |
| `GameGetFrameRequest` | 请求体 | [Game/LockStep/GameGetFrameRequest.cs](../../DataModels/Game/LockStep/GameGetFrameRequest.cs) | `access_info`, `begin_frame_id`, `end_frame_id` |
| `GameGetIdentityInfoResponse` | 应答 | [Game/LockStep/GameGetIdentityInfoResponse.cs](../../DataModels/Game/LockStep/GameGetIdentityInfoResponse.cs) | `data` |
| `GameGetRoomInfoResponse` | 应答 | [Game/LockStep/GameGetRoomInfoResponse.cs](../../DataModels/Game/LockStep/GameGetRoomInfoResponse.cs) | `data` |
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `GameRemoveUserStorageRequest` | 请求体 | [Game/Storage/GameRemoveUserStorageRequest.cs](../../DataModels/Game/Storage/GameRemoveUserStorageRequest.cs) | `key` |
| `GameSetUserInteractiveDataRequest` | 请求体 | [Game/Storage/GameSetUserInteractiveDataRequest.cs](../../DataModels/Game/Storage/GameSetUserInteractiveDataRequest.cs) | `kv_list` |
| `GameSetUserStorageRequest` | 请求体 | [Game/Storage/GameSetUserStorageRequest.cs](../../DataModels/Game/Storage/GameSetUserStorageRequest.cs) | `kv_list` |
| `GameMatchRuleIdResponse` | 应答 | [Game/GameMatch/GameMatchRuleIdResponse.cs](../../DataModels/Game/GameMatch/GameMatchRuleIdResponse.cs) | `match_id` |
| `GameCreateMatchRuleRequest` | 请求体 | [Game/GameMatch/GameCreateMatchRuleRequest.cs](../../DataModels/Game/GameMatch/GameCreateMatchRuleRequest.cs) | `team_count`, `team_member_count`, `need_room_service_info`, `game_room_info` |
| `GameDeleteMatchRuleRequest` | 请求体 | [Game/GameMatch/GameDeleteMatchRuleRequest.cs](../../DataModels/Game/GameMatch/GameDeleteMatchRuleRequest.cs) | `match_id` |
| `GameGetAllMatchRuleResponse` | 应答 | [Game/GameMatch/GameGetAllMatchRuleResponse.cs](../../DataModels/Game/GameMatch/GameGetAllMatchRuleResponse.cs) | `match_rule_list` |
| `GameMatchOpenStateRequest` | 请求体 | [Game/GameMatch/GameMatchOpenStateRequest.cs](../../DataModels/Game/GameMatch/GameMatchOpenStateRequest.cs) | `match_id`, `open_state` |
| `GameGetPayForOrderResponse` | 应答 | [Game/Midas/GameGetPayForOrderResponse.cs](../../DataModels/Game/Midas/GameGetPayForOrderResponse.cs) | `payfororder` |
| `GameGetPayForOrderRequest` | 请求体 | [Game/Midas/GameGetPayForOrderRequest.cs](../../DataModels/Game/Midas/GameGetPayForOrderRequest.cs) | `appid`, `order_no`, `out_trade_no` |
| `GameMsgSecCheckResponse` | 应答 | [Game/SecCheck/GameMsgSecCheckResponse.cs](../../DataModels/Game/SecCheck/GameMsgSecCheckResponse.cs) | `trace_id`, `result`, `detail` |
| `GameMsgSecCheckRequest` | 请求体 | [Game/SecCheck/GameMsgSecCheckRequest.cs](../../DataModels/Game/SecCheck/GameMsgSecCheckRequest.cs) | `openid`, `version`, `scene`, `content`, `nickname` |
| `GameMediaCheckResponse` | 应答 | [Game/SecCheck/GameMediaCheckResponse.cs](../../DataModels/Game/SecCheck/GameMediaCheckResponse.cs) | `trace_id`, `result`, `detail` |
| `GameMediaCheckRequest` | 请求体 | [Game/SecCheck/GameMediaCheckRequest.cs](../../DataModels/Game/SecCheck/GameMediaCheckRequest.cs) | `openid`, `version`, `scene`, `media_url`, `media_type` |
| `GameGetBalanceResponse` | 应答 | [Game/Midas/GameGetBalanceResponse.cs](../../DataModels/Game/Midas/GameGetBalanceResponse.cs) | `balance`, `present_balance`, `sum_save`, `sum_present`, `sum_balance`, `sum_cost`, `first_save` |
| `GameGetBalanceRequest` | 请求体 | [Game/Midas/GameGetBalanceRequest.cs](../../DataModels/Game/Midas/GameGetBalanceRequest.cs) | `openid`, `offer_id`, `ts`, `zone_id`, `env`, `user_ip`, `coin_type` |
| `GamePayResponse` | 应答 | [Game/Midas/GamePayResponse.cs](../../DataModels/Game/Midas/GamePayResponse.cs) | `bill_no`, `balance`, `used_present_amount` |
| `GamePayRequest` | 请求体 | [Game/Midas/GamePayRequest.cs](../../DataModels/Game/Midas/GamePayRequest.cs) | `openid`, `offer_id`, `ts`, `zone_id`, `env`, `user_ip`, `amount`, `bill_no`, `payitem`, `remark` |
| `GameCancelPayRequest` | 请求体 | [Game/Midas/GameCancelPayRequest.cs](../../DataModels/Game/Midas/GameCancelPayRequest.cs) | `openid`, `offer_id`, `ts`, `zone_id`, `env`, `user_ip`, `amount`, `bill_no`, `pay_bill_no` |
| `GamePresentResponse` | 应答 | [Game/Midas/GamePresentResponse.cs](../../DataModels/Game/Midas/GamePresentResponse.cs) | `bill_no`, `balance` |
| `GamePresentRequest` | 请求体 | [Game/Midas/GamePresentRequest.cs](../../DataModels/Game/Midas/GamePresentRequest.cs) | `openid`, `offer_id`, `ts`, `zone_id`, `env`, `user_ip`, `amount`, `bill_no` |
| `GameQueryOrderResponse` | 应答 | [Game/Midas/GameQueryOrderResponse.cs](../../DataModels/Game/Midas/GameQueryOrderResponse.cs) | `product_id`, `pay_state`, `deliver_state`, `pay_finish_time`, `out_trade_no`, `mch_order_no`, `transaction_id` |
| `GameQueryOrderRequest` | 请求体 | [Game/Midas/GameQueryOrderRequest.cs](../../DataModels/Game/Midas/GameQueryOrderRequest.cs) | `openid`, `offer_id`, `ts`, `zone_id`, `env`, `out_trade_no`, `biz_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Game;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddGameApi());

var service = app.Services.GetRequiredService<IWxaGameService>();
var ct = CancellationToken.None;
var response = await service.GetAllMatchRuleAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/business/gamematch/getallmatchrule");
// 占位值仅用于编译示意；业务值须按 IWxaGameService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAllMatchRuleAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaGameService](./IWxaGameService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaGameContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaGameContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)、[`MiniProgramNonContractRouteContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramNonContractRouteContractGuards.cs) |
| 守卫中的代表性测试方法 | `GameEndpoints_ShouldMatchOfficialRoutesAndVerbs`、`GameInterface_ShouldBindMpAccessToken_AndBeRegisteredInThreeStage`、`GameEndpoints_ShouldLockCredentialAndBodyFace`、`GameOfficialDocDefectsAndSharedTypeRulings_ShouldStayRegistered`、`GameDataModels_ShouldBeRegisteredInJsonContext`、`GameModuleRegistration_ShouldReusePaySigRegistrationPath`、`GameRoutes_ShouldBeDeclaredOnlyInMiniProgramLine_AndSensitiveValuesNeverLogged`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/Game`](../../DataModels/Game) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaGameContractGuards.cs`、`MiniProgramContractGuards.cs`、`MiniProgramNonContractRouteContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
