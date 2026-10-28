# Living 域 · 直播

企业微信「直播」模块直播管理域公共 SDK（创建预约直播 + 修改预约直播 + 取消预约直播 + 删除直播回放
+ 获取微信观看直播凭证 + 获取成员直播 ID 列表 + 获取直播详情 + 获取直播观看明细
+ 获取跳转小程序商城的直播观众信息）。 接口：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)、[IWechatWorkInternalLivingService](./IWechatWorkLivingService_Internal.cs)、[IWechatWorkProviderLivingService](./IWechatWorkLivingService_Provider.cs)、[IWechatWorkThirdPartyLivingService](./IWechatWorkLivingService_ThirdParty.cs)；注册组 `Living`；模块枚举 `WechatModule.Living`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatLivingContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLivingContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work/Living`](../../../Mud.Wechat.Work/Living/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkLivingService` | `CreateLivingAsync` | POST | `/cgi-bin/living/create` | `AccessToken`（Query），Query `access_token` | 创建预约直播 |
| 2 | `IWechatWorkLivingService` | `ModifyLivingAsync` | POST | `/cgi-bin/living/modify` | `AccessToken`（Query），Query `access_token` | 修改预约直播 |
| 3 | `IWechatWorkLivingService` | `CancelLivingAsync` | POST | `/cgi-bin/living/cancel` | `AccessToken`（Query），Query `access_token` | 取消预约直播 |
| 4 | `IWechatWorkLivingService` | `DeleteReplayDataAsync` | POST | `/cgi-bin/living/delete_replay_data` | `AccessToken`（Query），Query `access_token` | 删除直播回放 |
| 5 | `IWechatWorkLivingService` | `GetLivingCodeAsync` | POST | `/cgi-bin/living/get_living_code` | `AccessToken`（Query），Query `access_token` | 获取微信观看直播凭证（在微信中观看直播或直播回放） |
| 6 | `IWechatWorkLivingService` | `GetUserAllLivingIdAsync` | POST | `/cgi-bin/living/get_user_all_livingid` | `AccessToken`（Query），Query `access_token` | 获取成员直播 ID 列表 |
| 7 | `IWechatWorkLivingService` | `GetLivingInfoAsync` | GET | `/cgi-bin/living/get_living_info` | `AccessToken`（Query），Query `access_token` | 获取直播详情（官方即 GET，livingid 走 Query） |
| 8 | `IWechatWorkLivingService` | `GetWatchStatAsync` | POST | `/cgi-bin/living/get_watch_stat` | `AccessToken`（Query），Query `access_token` | 获取直播观看明细 |
| 9 | `IWechatWorkLivingService` | `GetLivingShareInfoAsync` | POST | `/cgi-bin/living/get_living_share_info` | `AccessToken`（Query），Query `access_token` | 获取跳转小程序商城的直播观众信息 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkLivingService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalLivingService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderLivingService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyLivingService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkLivingService`

官方对三类应用开放完全一致的 9 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
企业自建应用见 `IWechatWorkInternalLivingService`，
第三方应用见 `IWechatWorkThirdPartyLivingService`，
服务商代开发见 `IWechatWorkProviderLivingService`。
注意与「家校沟通·上课直播域」（School 模块）的路由交叠：获取成员直播 ID 列表与删除直播回放两条路由
官方在直播与家校沟通两处分文档承载同一端点，本域按直播文档页独立声明接口与 DTO。
获取直播详情官方即 GET（livingid 走 Query），其余 8 个端点官方即 POST。

令牌路由键说明：三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）——
企业自建应用消费应用自身 access_token；第三方 / 服务商代开发消费授权企业级 access_token（scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：自建应用须配置到「上课直播/直播 - 可调用接口的应用」中，第三方 / 代开发应用须具有「直播」权限；
全域仅能获取 / 操作本应用创建的直播。自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口
（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalLivingService`

官方对自建应用开放与三类应用公共面一致的 9 个端点（创建预约直播 / 修改预约直播 / 取消预约直播 /
删除直播回放 / 获取微信观看直播凭证 / 获取成员直播 ID 列表 / 获取直播详情 / 获取直播观看明细 /
获取跳转小程序商城的直播观众信息），全部收敛声明于父接口 `IWechatWorkLivingService`；
本接口不承载差异端点。第三方应用见 `IWechatWorkThirdPartyLivingService`；
服务商代开发见 `IWechatWorkProviderLivingService`。

消费应用自身 access_token（路由键 `AccessToken`，Query 注入 `access_token`）。
官方权限口径：自建应用须配置到「上课直播/直播 - 可调用接口的应用」中；
全域仅能获取 / 操作本应用创建的直播。自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口
（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderLivingService`

官方对服务商代开发开放与三类应用公共面一致的 9 个端点（创建预约直播 / 修改预约直播 / 取消预约直播 /
删除直播回放 / 获取微信观看直播凭证 / 获取成员直播 ID 列表 / 获取直播详情 / 获取直播观看明细 /
获取跳转小程序商城的直播观众信息），全部收敛声明于父接口 `IWechatWorkLivingService`；
本接口不承载差异端点。企业自建应用见 `IWechatWorkInternalLivingService`；
第三方应用见 `IWechatWorkThirdPartyLivingService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。官方权限口径：
代开发应用须具有「直播」权限；全域仅能获取 / 操作本应用创建的直播。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyLivingService`

官方对第三方应用开放与三类应用公共面一致的 9 个端点（创建预约直播 / 修改预约直播 / 取消预约直播 /
删除直播回放 / 获取微信观看直播凭证 / 获取成员直播 ID 列表 / 获取直播详情 / 获取直播观看明细 /
获取跳转小程序商城的直播观众信息），全部收敛声明于父接口 `IWechatWorkLivingService`；
本接口不承载差异端点。企业自建应用见 `IWechatWorkInternalLivingService`；
服务商代开发见 `IWechatWorkProviderLivingService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。官方权限口径：
第三方应用须具有「直播」权限；全域仅能获取 / 操作本应用创建的直播。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateLivingAsync` — 创建预约直播

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/create`

- **请求体**：`CreateLivingRequest` — 请求体（`CreateLivingRequest`：anchor_userid / theme / living_start / living_duration / type / description / agentid / remind_time / activity_cover_mediaid / activity_share_mediaid / activity_detail）。
- **返回**：`CreateLivingResponse` — 直播 id（livingid）。

**补充说明**

- 创建一个预约直播，返回直播 id（可通过此 id 调用「进入直播」接口——包括小程序接口和 JS-SDK 接口—— 实现主播到点后的开播操作，以及观众进入直播详情预约和观看直播）。
- 官方限制：直播标题最多支持 20 个 utf8 字符；直播简介最多 100 个 utf8 字符， 仅对通用直播、小班课、大班课、企业培训生效；大班课和小班课仅 k12 学校和 IT 行业类型能够发起； 活动直播附图最多支持传 5 张，超过五张取前五张。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93637
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93717
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96837

### 2. `ModifyLivingAsync` — 修改预约直播

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/modify`

- **请求体**：`ModifyLivingRequest` — 请求体（`ModifyLivingRequest`：livingid / theme / living_start / living_duration / type / description / remind_time）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 修改一个预约直播的信息。
- 官方限制：仅允许修改当前应用创建的直播，且仅允许修改预约状态下的直播 id； 直播标题最多支持 60 个字节，直播简介最多支持 300 个字节。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93640
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93720
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96839

### 3. `CancelLivingAsync` — 取消预约直播

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/cancel`

- **请求体**：`CancelLivingRequest` — 请求体（`CancelLivingRequest`：livingid）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 取消一个指定的预约直播。
- 官方限制：仅允许取消当前应用创建的直播，且仅允许取消预约状态下的直播 id。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93638
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93718
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96838

### 4. `DeleteReplayDataAsync` — 删除直播回放

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/delete_replay_data`

- **请求体**：`DeleteLivingReplayDataRequest` — 请求体（`DeleteLivingReplayDataRequest`：livingid）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 删除指定直播的回放数据。
- 官方限制：仅允许删除当前应用自己创建的直播。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93874
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93719
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96841

### 5. `GetLivingCodeAsync` — 获取微信观看直播凭证（在微信中观看直播或直播回放）

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/get_living_code`

- **请求体**：`GetLivingCodeRequest` — 请求体（`GetLivingCodeRequest`：livingid / openid）。
- **返回**：`GetLivingCodeResponse` — 微信观看直播凭证（living_code）。

**补充说明**

- 获取微信观看直播凭证 living_code；获取后可在微信 H5（开放标签 username 固定为 `gh_25e071b83ee0`） 或小程序（appId 固定为 `wx7424030d69bde86e`，路径 `pages/watch/index?living_code=LIVING_CODE`， 回放页追加 `&replay=1`）中唤起直播小程序观看直播或直播回放。
- 官方限制：仅允许获取当前应用创建的微信观看直播凭证；living_code 5 分钟内可以重复使用，且仅能在微信上使用。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93641
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93721
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96840

### 6. `GetUserAllLivingIdAsync` — 获取成员直播 ID 列表

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/get_user_all_livingid`

- **请求体**：`GetUserAllLivingIdRequest` — 请求体（`GetUserAllLivingIdRequest`：userid / cursor / limit）。
- **返回**：`GetUserAllLivingIdResponse` — 分页游标（next_cursor，返回空字符串代表已经是最后一页）与直播 ID 列表（livingid_list）。

**补充说明**

- 获取指定成员的所有直播 ID。
- 官方限制：只能获取本应用创建的直播；limit 建议填 20，默认值和最大值都为 100； 以 next_cursor 分页，返回空字符串代表已经是最后一页。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93634
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93714
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96834
- 本路由与「家校沟通·上课直播域」的获取老师直播 ID 列表为官方同一端点（分文档承载）。

### 7. `GetLivingInfoAsync` — 获取直播详情（官方即 GET，livingid 走 Query）

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/living/get_living_info`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `livingid` | Query `livingid` | `string` | 直播 ID。 |
- **返回**：`GetLivingInfoResponse` — 直播详情信息（`LivingDetail`：theme / living_start / living_duration / status / reserve_start / reserve_living_duration / description / anchor_userid / main_department / viewer_num / comment_num / mic_num / open_replay / replay_status / type / push_stream_url / online_count / subscribe_count）。

**补充说明**

- 获取直播详情（主题、开播时间、时长、状态、预约信息、描述、主播、观看/评论/连麦人数、回放开关与状态、 直播类型、推流地址、在线人数与预约人数）。
- 官方限制：只能获取本应用创建的直播；push_stream_url 仅活动直播且状态待开播时返回； replay_status 仅 open_replay 为 1 时返回。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93635
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93715
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96835

### 8. `GetWatchStatAsync` — 获取直播观看明细

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/get_watch_stat`

- **请求体**：`GetLivingWatchStatRequest` — 请求体（`GetLivingWatchStatRequest`：livingid / next_key）。
- **返回**：`GetLivingWatchStatResponse` — 是否结束（ending）、分页 key（next_key）与统计信息列表（stat_info.users / stat_info.external_users）。

**补充说明**

- 获取所有观看直播的人员统计（企业成员与外部成员）。
- 官方限制：只能获取本应用创建的直播；以响应 ending 字段判断是否拉完 （0 表示还有更多数据需继续拉取，1 表示已拉完），next_key 分页，初次调用可填 "0"； 邀请人字段（invitor_userid / invitor_external_userid）仅「推广产品」直播支持。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93636
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93716
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96836

### 9. `GetLivingShareInfoAsync` — 获取跳转小程序商城的直播观众信息

- **声明接口**：[IWechatWorkLivingService](./IWechatWorkLivingService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/living/get_living_share_info`

- **请求体**：`GetLivingShareInfoRequest` — 请求体（`GetLivingShareInfoRequest`：ww_share_code）。
- **返回**：`GetLivingShareInfoResponse` — 直播 id（livingid）、观众 userid / external_userid（viewer_userid / viewer_external_userid）与邀请人 userid / external_userid（invitor_userid / invitor_external_userid）。

**补充说明**

- 「推广产品」直播观众跳转小程序商城时会在小程序 path 中带上 ww_share_code 参数， 通过该参数换取观众与邀请人身份信息。
- 官方限制：ww_share_code 五分钟内有效；跳转的小程序需要与企业有绑定关系。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94442
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94578
- **服务商代开发**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96843

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/Living`](../../../Mud.Wechat.Work/Living/) 内共 **19** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **19** 个，`SerializerClassName` = `Living`。

AOT 源生成上下文：[`LivingJsonContext.g.cs`](./../../../Mud.Wechat.Work/Generated/LivingJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CreateLivingResponse` | 应答 | [Living/CreateLivingResponse.cs](../../../Mud.Wechat.Work/Living/CreateLivingResponse.cs) | `livingid` |
| `CreateLivingRequest` | 请求体 | [Living/CreateLivingRequest.cs](../../../Mud.Wechat.Work/Living/CreateLivingRequest.cs) | `anchor_userid`, `theme`, `living_start`, `living_duration`, `type`, `description`, `agentid`, `remind_time`, `activity_cover_mediaid`, `activity_share_mediaid`, `activity_detail` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../../Mud.Wechat.Work/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `ModifyLivingRequest` | 请求体 | [Living/ModifyLivingRequest.cs](../../../Mud.Wechat.Work/Living/ModifyLivingRequest.cs) | `livingid`, `theme`, `living_start`, `living_duration`, `type`, `description`, `remind_time` |
| `CancelLivingRequest` | 请求体 | [Living/CancelLivingRequest.cs](../../../Mud.Wechat.Work/Living/CancelLivingRequest.cs) | `livingid` |
| `DeleteLivingReplayDataRequest` | 请求体 | [Living/DeleteLivingReplayDataRequest.cs](../../../Mud.Wechat.Work/Living/DeleteLivingReplayDataRequest.cs) | `livingid` |
| `GetLivingCodeResponse` | 应答 | [Living/GetLivingCodeResponse.cs](../../../Mud.Wechat.Work/Living/GetLivingCodeResponse.cs) | `living_code` |
| `GetLivingCodeRequest` | 请求体 | [Living/GetLivingCodeRequest.cs](../../../Mud.Wechat.Work/Living/GetLivingCodeRequest.cs) | `livingid`, `openid` |
| `GetUserAllLivingIdResponse` | 应答 | [Living/GetUserAllLivingIdResponse.cs](../../../Mud.Wechat.Work/Living/GetUserAllLivingIdResponse.cs) | `next_cursor`, `livingid_list` |
| `GetUserAllLivingIdRequest` | 请求体 | [Living/GetUserAllLivingIdRequest.cs](../../../Mud.Wechat.Work/Living/GetUserAllLivingIdRequest.cs) | `userid`, `cursor`, `limit` |
| `GetLivingInfoResponse` | 应答 | [Living/GetLivingInfoResponse.cs](../../../Mud.Wechat.Work/Living/GetLivingInfoResponse.cs) | `living_info` |
| `GetLivingWatchStatResponse` | 应答 | [Living/GetLivingWatchStatResponse.cs](../../../Mud.Wechat.Work/Living/GetLivingWatchStatResponse.cs) | `ending`, `next_key`, `stat_info` |
| `GetLivingWatchStatRequest` | 请求体 | [Living/GetLivingWatchStatRequest.cs](../../../Mud.Wechat.Work/Living/GetLivingWatchStatRequest.cs) | `livingid`, `next_key` |
| `GetLivingShareInfoResponse` | 应答 | [Living/GetLivingShareInfoResponse.cs](../../../Mud.Wechat.Work/Living/GetLivingShareInfoResponse.cs) | `livingid`, `viewer_userid`, `viewer_external_userid`, `invitor_userid`, `invitor_external_userid` |
| `GetLivingShareInfoRequest` | 请求体 | [Living/GetLivingShareInfoRequest.cs](../../../Mud.Wechat.Work/Living/GetLivingShareInfoRequest.cs) | `ww_share_code` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Living;
using Mud.Wechat.Work.DataModels;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddLivingApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalLivingService>();
var ct = CancellationToken.None;
var response = await service.CancelLivingAsync(new CancelLivingRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/living/cancel");
// 占位值仅用于编译示意；业务值须按 IWechatWorkLivingService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CancelLivingAsync`）；请求体 `CancelLivingRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkLivingService](./IWechatWorkLivingService.cs)、[IWechatWorkInternalLivingService](./IWechatWorkLivingService_Internal.cs)、[IWechatWorkProviderLivingService](./IWechatWorkLivingService_Provider.cs)、[IWechatWorkThirdPartyLivingService](./IWechatWorkLivingService_ThirdParty.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatLivingContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLivingContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatSchoolContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs) |
| 守卫中的代表性测试方法 | `LivingEndpoints_ShouldMatchOfficialRoutes`、`LivingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`、`LivingTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`LivingDataModels_ShouldBeRegisteredInJsonContext`、`LivingDataModels_ShouldLockOfficialContractTraps`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces`、`SchoolEndpoints_ShouldMatchOfficialRoutes`、`SchoolSettingEndpoints_ShouldMatchOfficialRoutes`、`SchoolUserEndpoints_ShouldMatchOfficialRoutes`、`SchoolDepartmentEndpoints_ShouldMatchOfficialRoutes`、`SchoolAuthEndpoints_ShouldMatchOfficialRoutes`、`SchoolHealthReportEndpoints_ShouldMatchOfficialRoutes` … |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/Living`](../../../Mud.Wechat.Work/Living/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatLivingContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatSchoolContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
