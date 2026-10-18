# Message 域 · 消息推送

企业微信「消息推送」模块群聊会话域企业自建应用 SDK：官方仅向自建应用开放本域端点
（创建群聊会话 / 修改群聊会话 / 获取群聊会话 / 应用推送消息到群聊会话），全部声明于本接口。 接口：[IWechatWorkAppChatService](./IWechatWorkAppChatService.cs)、[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)、[IWechatWorkMessageService](./IWechatWorkMessageService.cs)、[IWechatWorkInternalMessageService](./IWechatWorkMessageService_Internal.cs)、[IWechatWorkProviderMessageService](./IWechatWorkMessageService_Provider.cs)、[IWechatWorkThirdPartyMessageService](./IWechatWorkMessageService_ThirdParty.cs)、[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)、[IWechatWorkInternalSchoolMessageService](./IWechatWorkSchoolMessageService_Internal.cs)、[IWechatWorkProviderSchoolMessageService](./IWechatWorkSchoolMessageService_Provider.cs)、[IWechatWorkThirdPartySchoolMessageService](./IWechatWorkSchoolMessageService_ThirdParty.cs)、[IWechatWorkSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService.cs)、[IWechatWorkInternalSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService_Internal.cs)；注册组 `Message`；模块枚举 `WechatModule.Message`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatMessageContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work.DataModels/Message`](../../../Mud.Wechat.Work.DataModels/Message/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **37** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalAppChatService` | `CreateAppChatAsync` | POST | `/cgi-bin/appchat/create` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 创建群聊会话 |
| 2 | `IWechatWorkInternalAppChatService` | `UpdateAppChatAsync` | POST | `/cgi-bin/appchat/update` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 修改群聊会话 |
| 3 | `IWechatWorkInternalAppChatService` | `GetAppChatAsync` | GET | `/cgi-bin/appchat/get` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取群聊会话 |
| 4 | `IWechatWorkInternalAppChatService` | `SendAppChatTextMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送文本消息到群聊会话 |
| 5 | `IWechatWorkInternalAppChatService` | `SendAppChatImageMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送图片消息到群聊会话 |
| 6 | `IWechatWorkInternalAppChatService` | `SendAppChatVoiceMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送语音消息到群聊会话 |
| 7 | `IWechatWorkInternalAppChatService` | `SendAppChatVideoMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送视频消息到群聊会话 |
| 8 | `IWechatWorkInternalAppChatService` | `SendAppChatFileMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送文件消息到群聊会话 |
| 9 | `IWechatWorkInternalAppChatService` | `SendAppChatTextCardMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送文本卡片消息到群聊会话 |
| 10 | `IWechatWorkInternalAppChatService` | `SendAppChatNewsMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送图文消息到群聊会话 |
| 11 | `IWechatWorkInternalAppChatService` | `SendAppChatMpNewsMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送 mpnews 图文消息到群聊会话 |
| 12 | `IWechatWorkInternalAppChatService` | `SendAppChatMarkdownMessageAsync` | POST | `/cgi-bin/appchat/send` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 应用推送 markdown 消息到群聊会话 |
| 13 | `IWechatWorkMessageService` | `SendTextMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送文本消息 |
| 14 | `IWechatWorkMessageService` | `SendImageMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送图片消息 |
| 15 | `IWechatWorkMessageService` | `SendVoiceMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送语音消息 |
| 16 | `IWechatWorkMessageService` | `SendVideoMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送视频消息 |
| 17 | `IWechatWorkMessageService` | `SendFileMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送文件消息 |
| 18 | `IWechatWorkMessageService` | `SendTextCardMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送文本卡片消息 |
| 19 | `IWechatWorkMessageService` | `SendNewsMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送图文消息 |
| 20 | `IWechatWorkMessageService` | `SendMpNewsMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送图文消息（mpnews） |
| 21 | `IWechatWorkMessageService` | `SendMarkdownMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送 markdown 消息 |
| 22 | `IWechatWorkMessageService` | `SendMiniProgramNoticeMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送小程序通知消息 |
| 23 | `IWechatWorkMessageService` | `SendTemplateCardMessageAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token` | 发送模板卡片消息 |
| 24 | `IWechatWorkMessageService` | `UpdateTemplateCardAsync` | POST | `/cgi-bin/message/update_template_card` | `AccessToken`（Query），Query `access_token` | 更新模版卡片消息 |
| 25 | `IWechatWorkMessageService` | `RecallMessageAsync` | POST | `/cgi-bin/message/recall` | `AccessToken`（Query），Query `access_token` | 撤回应用消息 |
| 26 | `IWechatWorkThirdPartyMessageService` | `SendTemplateMsgAsync` | POST | `/cgi-bin/message/send` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 发送模板消息（仅第三方应用） |
| 27 | `IWechatWorkSchoolMessageService` | `SendSchoolTextMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送文本学校通知 |
| 28 | `IWechatWorkSchoolMessageService` | `SendSchoolImageMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送图片学校通知 |
| 29 | `IWechatWorkSchoolMessageService` | `SendSchoolVoiceMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送语音学校通知 |
| 30 | `IWechatWorkSchoolMessageService` | `SendSchoolVideoMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送视频学校通知 |
| 31 | `IWechatWorkSchoolMessageService` | `SendSchoolFileMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送文件学校通知 |
| 32 | `IWechatWorkSchoolMessageService` | `SendSchoolNewsMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送图文学校通知 |
| 33 | `IWechatWorkSchoolMessageService` | `SendSchoolMpNewsMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送 mpnews 图文学校通知 |
| 34 | `IWechatWorkSchoolMessageService` | `SendSchoolMiniProgramMessageAsync` | POST | `/cgi-bin/externalcontact/message/send` | `AccessToken`（Query），Query `access_token` | 发送小程序学校通知 |
| 35 | `IWechatWorkInternalSmartSheetGroupChatService` | `GetSmartSheetGroupChatListAsync` | POST | `/cgi-bin/wedoc/smartsheet/groupchat/list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取智能表格自动化创建的群聊列表 |
| 36 | `IWechatWorkInternalSmartSheetGroupChatService` | `GetSmartSheetGroupChatAsync` | POST | `/cgi-bin/wedoc/smartsheet/groupchat/get` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取智能表格自动化创建的群聊会话 |
| 37 | `IWechatWorkInternalSmartSheetGroupChatService` | `UpdateSmartSheetGroupChatAsync` | POST | `/cgi-bin/wedoc/smartsheet/groupchat/update` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 修改智能表格自动化创建的群聊会话 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkAppChatService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalAppChatService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkMessageService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalMessageService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderMessageService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyMessageService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkSchoolMessageService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalSchoolMessageService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderSchoolMessageService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartySchoolMessageService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkSmartSheetGroupChatService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalSmartSheetGroupChatService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkAppChatService`

官方仅向**企业自建应用**开放本域 4 个端点（appchat/create、appchat/update、appchat/get、appchat/send），
且明示第三方不可调用，服务商代开发亦无对应文档，
因此本父接口没有公共端点；全部 12 个端点声明于 `IWechatWorkInternalAppChatService`
（唯一的应用类型子接口，落位形态对齐 `IWechatWorkInternalSecurityService`）。

落位与形态对齐 `IWechatWorkSecurityService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`，应用自身凭证），
由多应用基座按当前应用上下文（AppKey + scope）路由。

权限口径：调用本域端点的自建应用，其可见范围必须为根部门；
修改 / 获取 / 推送消息要求群为该应用通过 `appchat/create` 创建。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalAppChatService`

第三方应用官方明示不可调用，服务商代开发无对应文档，均不设子接口。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
应用可见范围必须为根部门；修改 / 获取 / 推送消息要求群为该应用所创建。
限频：创建 1000 群/天；修改 1000 次/小时；推送消息每企业 2 万人次/分
（群 100 人，每发一次算 100 人次）＋企业规模分档小时额度（未认证或小型 ≤15 万、
中型 ≤35 万、大型 ≤70 万人次/小时）；每个成员在群中收到的同一应用消息不可超过
200 条/分、1 万条/天，超过部分被丢弃且接口不报错（推送成功不等于全员送达）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkMessageService`

官方对企业自建应用（90236 / 94888 / 94867）、第三方应用（90372 / 94945 / 94947）、
服务商代开发（96458 / 96459 / 96460）开放完全一致的 3 个端点，全部收敛声明于本接口；
应用类型子接口：自建应用见 `IWechatWorkInternalMessageService`（空标记）、
第三方应用见 `IWechatWorkThirdPartyMessageService`（另持 template_msg 模板消息差异端点，94515）、
服务商代开发见 `IWechatWorkProviderMessageService`（空标记）。

三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）：
企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

限频口径：每应用不可超过「账号上限数 × 200」人次/天（一次 API 发给 1000 人算 1000 人次）；
对同一成员不可超过 30 次/分钟、1000 次/小时，超过部分被丢弃不下发；
官方建议避开每小时 0 分 / 30 分的集中推送高峰。

接收人口径：`touser` / `toparty` / `totag` 不能同时为空；
部分接收人无权限或不存在时发送仍执行并返回无效部分（invaliduser / invalidparty / invalidtag / unlicenseduser），
常见原因是接收人不在应用可见范围内；全部接收人无权限或不存在则本次调用失败（errcode 81013）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalMessageService`

官方对自建应用仅开放与三类应用公共面一致的 3 个端点（发送应用消息 / 更新模版卡片消息 /
撤回应用消息，继承自 `IWechatWorkMessageService`）；本接口不新增端点，仅作为
自建应用的类型化契约入口存在（形态对齐 `IWechatWorkInternalExternalContactGroupChatService` 空标记）。
第三方应用见 `IWechatWorkThirdPartyMessageService`（另持 template_msg 差异端点）；
服务商代开发见 `IWechatWorkProviderMessageService`。

消费应用自身 access_token（路由键 `AccessToken`）。
官方权限口径：接收人须在应用可见范围内且具备基础接口权限；
限频为每应用「账号上限数 × 200」人次/天，同一成员 30 次/分钟、1000 次/小时（超过部分被丢弃不下发）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderMessageService`

官方对代开发仅开放与三类应用公共面一致的 3 个端点（发送应用消息 / 更新模版卡片消息 /
撤回应用消息，继承自 `IWechatWorkMessageService`）；本接口不新增端点，仅作为
代开发的类型化契约入口存在（形态对齐 `IWechatWorkProviderExternalContactGroupChatService` 空标记）。
`template_msg` 模板消息仅第三方应用支持，代开发不设差异端点。
企业自建应用见 `IWechatWorkInternalMessageService`；
第三方应用见 `IWechatWorkThirdPartyMessageService`。

消费代开发授权应用 access_token（路由键 `AccessToken`，
经 `gettoken` 以 `permanent_code` 作为 corpsecret 换取，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyMessageService`

除继承公共父接口 `IWechatWorkMessageService` 的 3 个端点外，
第三方应用官方另支持 `template_msg` 模板消息类型
（`SendTemplateMsgAsync`，经发送应用消息端点 `/cgi-bin/message/send` 发送，文档 94515；
自建应用与服务商代开发均无对应文档，不设差异端点）。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：服务商需在管理端申请模版（正式授权的应用需审批通过），
接口传参的内容必须与申请的模版匹配；管理员授权模式下仅能给可见范围之内的成员推送消息，
成员授权模式下可通过 selected_ticket_list 向可见范围外的成员推送。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkSchoolMessageService`

官方对三类应用开放完全一致的 8 个端点（发送「学校通知」8 种 msgtype：
text / image / voice / video / file / news / mpnews / miniprogram；
自建 91609、第三方 92291、服务商代开发 96720/96723），全部收敛声明于本接口；
应用类型子接口均为零差异端点空标记：自建见 `IWechatWorkInternalSchoolMessageService`、
第三方见 `IWechatWorkThirdPartySchoolMessageService`、
服务商代开发见 `IWechatWorkProviderSchoolMessageService`。

三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）：
企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

权限口径：学校管理员需要将应用配置在「家长可使用的应用」才可调用；
通过 recv_scope / to_parent_userid / to_student_userid / to_party / toall 指定接收对象
（家长 / 学生 / 家长和学生三态，单次最多 1000 个家长或学生、100 个班级）；
id 转译仅文本 / 图文 / mpnews / 小程序四类消息的部分字段支持。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalSchoolMessageService`

官方对三类应用开放完全一致的 8 个端点（发送「学校通知」），全部收敛于
`IWechatWorkSchoolMessageService`；本接口不新增端点，仅作为
自建应用的类型化契约入口存在（形态对齐 `IWechatWorkInternalPayBillService` 空标记）。
第三方应用见 `IWechatWorkThirdPartySchoolMessageService`；
服务商代开发见 `IWechatWorkProviderSchoolMessageService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
学校管理员需要将应用配置在「家长可使用的应用」才可调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderSchoolMessageService`

官方对三类应用开放完全一致的 8 个端点（发送「学校通知」，代开发文档 96720/96723），
全部收敛于 `IWechatWorkSchoolMessageService`；本接口不新增端点，仅作为
服务商代开发的类型化契约入口存在（形态对齐 `IWechatWorkProviderPayBillService` 空标记）。
自建应用见 `IWechatWorkInternalSchoolMessageService`；
第三方应用见 `IWechatWorkThirdPartySchoolMessageService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：代开发应用须具有「家校沟通」权限；学校管理员需要将应用配置在「家长可使用的应用」才可调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartySchoolMessageService`

官方对三类应用开放完全一致的 8 个端点（发送「学校通知」，第三方文档 92291），
全部收敛于 `IWechatWorkSchoolMessageService`；本接口不新增端点，仅作为
第三方应用的类型化契约入口存在（形态对齐 `IWechatWorkThirdPartyPayBillService` 空标记）。
自建应用见 `IWechatWorkInternalSchoolMessageService`；
服务商代开发见 `IWechatWorkProviderSchoolMessageService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：第三方应用须具有「家校沟通」使用权限；学校管理员需要将应用配置在「家长可使用的应用」才可调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkSmartSheetGroupChatService`

官方仅向**企业自建应用**开放本域 3 个端点（wedoc/smartsheet/groupchat/list、get、update），
第三方应用与服务商代开发均无对应文档，不设子接口，因此本父接口没有公共端点；
全部 3 个端点声明于 `IWechatWorkInternalSmartSheetGroupChatService`
（唯一的应用类型子接口，落位形态对齐 `IWechatWorkInternalAppChatService`）。

落位与形态对齐 `IWechatWorkAppChatService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`，应用自身凭证），
由多应用基座按当前应用上下文（AppKey + scope）路由。

权限口径：仅自建应用可调用，且需配置到文档「可调用应用」列表中的应用，
应用的可见范围需包含根部门；操作的智能表格须为当前应用创建。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalSmartSheetGroupChatService`

第三方应用与服务商代开发官方无对应文档，不设子接口。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
需配置到文档「可调用应用」列表中的应用，可见范围需包含根部门；操作的智能表格须为当前应用创建。
并发限制：获取群聊列表 / 获取群聊会话为 20，修改群聊会话为 10；
修改群聊会话每企业变更次数不可超过 1000 次/小时，且**同一群聊的修改需要串行执行**。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 9 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90248
- （出现 11 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90236
- （出现 11 次）**第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90372
- （出现 11 次）**服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96458
- （出现 8 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/91609
- （出现 8 次）**第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/92291
- （出现 7 次）**服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96720

### 1. `CreateAppChatAsync` — 创建群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/create`

- **请求体**：`CreateAppChatRequest` — 创建请求体（`CreateAppChatRequest`：name / owner / userlist / chatid）。
- **返回**：`CreateAppChatResponse` — 群聊的唯一标志（chatid）。

**补充说明**

- chatid 不填则系统随机生成；群成员至少 2 人、至多 2000 人（含应用）， 且不可超过管理端配置的「群成员人数上限」；每企业创建群数不可超过 1000 个/天。
- 刚创建的群如果没有下发消息，在旧版本企业微信终端上可能不会出现该群。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90245

### 2. `UpdateAppChatAsync` — 修改群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/update`

- **请求体**：`UpdateAppChatRequest` — 修改请求体（`UpdateAppChatRequest`：chatid + name / owner / add_user_list / del_user_list）。
- **返回**：`UpdateAppChatResponse` — 修改结果（仅 errcode / errmsg）。

**补充说明**

- chatid 所代表的群必须是该应用所创建；每企业变更群的次数不可超过 1000 次/小时。
- 课程群聊群主必须拥有课程群创建权限，del_user_list 包含群主时 owner 必填。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98913

### 3. `GetAppChatAsync` — 获取群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`GET /cgi-bin/appchat/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `chatId` | Query `chatid` | `string` | 群聊 id（创建群聊会话时指定或系统随机生成的 chatid）。 |
- **返回**：`GetAppChatResponse` — 群聊会话信息（chat_info：chatid / name / owner / userlist / chat_type）。

**补充说明**

- chatid 所代表的群必须是该应用所创建；返回群聊的群名、群主、成员列表与群聊类型。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98914

### 4. `SendAppChatTextMessageAsync` — 应用推送文本消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendTextRequest` — 推送请求体（`AppChatSendTextRequest`：chatid + text 消息体 + safe）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `text`：content 最长不超过 2048 个字节； 支持 mentioned_list 提醒群中指定成员（@all 提醒所有人）， 支持 `<@userid>` 扩展语法 @群成员（企业微信 5.0.6 及以上版本支持）。
- chatid 所代表的群必须是该应用所创建；每企业消息发送量不可超过 2 万人次/分； 每个成员在群中收到的同一应用消息不可超过 200 条/分、1 万条/天，超过部分被丢弃且接口不报错。

### 5. `SendAppChatImageMessageAsync` — 应用推送图片消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendImageRequest` — 推送请求体（`AppChatSendImageRequest`：chatid + image 消息体 + safe）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `image`：图片媒体文件经上传临时素材接口获取（支持 JPG、PNG）。

### 6. `SendAppChatVoiceMessageAsync` — 应用推送语音消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendVoiceRequest` — 推送请求体（`AppChatSendVoiceRequest`：chatid + voice 消息体）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `voice`：语音媒体文件经上传临时素材接口获取。

### 7. `SendAppChatVideoMessageAsync` — 应用推送视频消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendVideoRequest` — 推送请求体（`AppChatSendVideoRequest`：chatid + video 消息体 + safe）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `video`：标题不超过 128 字节、描述不超过 512 字节，超过自动截断。

### 8. `SendAppChatFileMessageAsync` — 应用推送文件消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendFileRequest` — 推送请求体（`AppChatSendFileRequest`：chatid + file 消息体 + safe）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `file`：文件经上传临时素材接口获取； 保密消息仅支持 txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif 格式。

### 9. `SendAppChatTextCardMessageAsync` — 应用推送文本卡片消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendTextCardRequest` — 推送请求体（`AppChatSendTextCardRequest`：chatid + textcard 消息体）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `textcard`：标题、描述、跳转链接必填； 描述支持 br 标签或空格换行、div 标签 class 内置 gray/highlight/normal 三种文字颜色。

### 10. `SendAppChatNewsMessageAsync` — 应用推送图文消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendNewsRequest` — 推送请求体（`AppChatSendNewsRequest`：chatid + news 消息体）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `news`：一个图文消息支持 1 到 8 条图文。

### 11. `SendAppChatMpNewsMessageAsync` — 应用推送 mpnews 图文消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendMpNewsRequest` — 推送请求体（`AppChatSendMpNewsRequest`：chatid + mpnews 消息体 + safe）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `mpnews`：与普通图文一致，唯一差异是图文内容存储在企业微信； 多次发送会被认为是不同的图文，阅读、点赞统计分开计算。

### 12. `SendAppChatMarkdownMessageAsync` — 应用推送 markdown 消息到群聊会话

- **声明接口**：[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/appchat/send`

- **请求体**：`AppChatSendMarkdownRequest` — 推送请求体（`AppChatSendMarkdownRequest`：chatid + markdown 消息体）。
- **返回**：`AppChatSendResponse` — 推送结果（仅 errcode / errmsg）。

**补充说明**

- msgtype = `markdown`：仅支持 markdown 语法的子集； 支持 `<@userid>` 扩展语法 @群成员（企业微信 5.0.6 及以上版本支持）； 微信插件（原企业号）不支持展示 markdown 消息。

### 13. `SendTextMessageAsync` — 发送文本消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendTextRequest` — 发送请求体（`MessageSendTextRequest`：公共信封 + text 消息体 + safe）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid / response_code）。

**补充说明**

- msgtype = `text`：消息内容最长不超过 2048 个字节，超过将截断；支持 id 转译； 换行须使用转义的 `\n`，支持 `<a href="...">` 标签打开自定义网页。

### 14. `SendImageMessageAsync` — 发送图片消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendImageRequest` — 发送请求体（`MessageSendImageRequest`：公共信封 + image 消息体 + safe）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `image`：图片媒体文件经上传临时素材接口获取（支持 JPG、PNG）。

### 15. `SendVoiceMessageAsync` — 发送语音消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendVoiceRequest` — 发送请求体（`MessageSendVoiceRequest`：公共信封 + voice 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `voice`：语音媒体文件经上传临时素材接口获取。

### 16. `SendVideoMessageAsync` — 发送视频消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendVideoRequest` — 发送请求体（`MessageSendVideoRequest`：公共信封 + video 消息体 + safe）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `video`：标题不超过 128 字节、描述不超过 512 字节，超过自动截断。

### 17. `SendFileMessageAsync` — 发送文件消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendFileRequest` — 发送请求体（`MessageSendFileRequest`：公共信封 + file 消息体 + safe）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `file`：文件经上传临时素材接口获取； 保密消息仅支持 txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif 格式。

### 18. `SendTextCardMessageAsync` — 发送文本卡片消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendTextCardRequest` — 发送请求体（`MessageSendTextCardRequest`：公共信封 + textcard 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `textcard`：标题不超过 128 个字符、描述不超过 512 个字符，超过自动截断（支持 id 转译）； 描述支持 br 标签或空格换行、div 标签 class 内置 gray/highlight/normal 三种文字颜色； 跳转链接最长 2048 字节且须包含协议头（http/https）。

### 19. `SendNewsMessageAsync` — 发送图文消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendNewsRequest` — 发送请求体（`MessageSendNewsRequest`：公共信封 + news 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `news`：一个图文消息支持 1 到 8 条图文； 小程序（appid + pagepath）或者 url 必须填写一个。

### 20. `SendMpNewsMessageAsync` — 发送图文消息（mpnews）

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendMpNewsRequest` — 发送请求体（`MessageSendMpNewsRequest`：公共信封 + mpnews 消息体 + safe）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `mpnews`：与普通图文一致，唯一差异是图文内容存储在企业微信； 多次发送会被认为是不同的图文，阅读、点赞统计分开计算； safe 仅本类型额外支持取值 2（仅限在企业内分享）。

### 21. `SendMarkdownMessageAsync` — 发送 markdown 消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendMarkdownRequest` — 发送请求体（`MessageSendMarkdownRequest`：公共信封 + markdown 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `markdown`：仅支持 markdown 语法的子集（标题 / 加粗 / 链接 / 行内代码段 / 引用）； 字体颜色仅 info（绿）/ comment（灰）/ warning（橙红）三种内置； 微工作台（原企业号）不支持展示 markdown 消息。

### 22. `SendMiniProgramNoticeMessageAsync` — 发送小程序通知消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendMiniProgramNoticeRequest` — 发送请求体（`MessageSendMiniProgramNoticeRequest`：公共信封 + miniprogram_notice 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `miniprogram_notice`：只允许绑定了小程序的应用发送； 不支持 @all 全员发送；微工作台（原企业号）不支持展示； 2019-06-28 起用户收到的小程序通知会出现在各个独立的应用中。

### 23. `SendTemplateCardMessageAsync` — 发送模板卡片消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendTemplateCardRequest` — 发送请求体（`MessageSendTemplateCardRequest`：公共信封 + template_card 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（支持回调的卡片返回 response_code，72 小时内有效且只能使用一次）。

**补充说明**

- msgtype = `template_card`：卡片类型 card_type 取值 text_notice（文本通知型）/ news_notice（图文展示型）/ button_interaction（按钮交互型）/ vote_interaction（投票选择型）/ multiple_interaction（多项选择型）。
- 版本要求：文本通知型 / 图文展示型 / 按钮交互型需企业微信 3.1.6 及以上（附件下载需 3.1.12）， 投票选择型 / 多项选择型需 3.1.12 及以上，desc_color / horizontal_content_list type 3 / action_menu / quote_area / image_text_area / button_selection 需 3.1.18 及以上； 微工作台不支持展示模板卡片消息；没有配置回调接口的应用不可发送支持回调的卡片。

**备注（原文）**

- 发送后可经 `UpdateTemplateCardAsync` 以 response_code 更新卡片。

### 24. `UpdateTemplateCardAsync` — 更新模版卡片消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/update_template_card`

- **请求体**：`UpdateTemplateCardRequest` — 更新请求体（`UpdateTemplateCardRequest`：userids / partyids / tagids / atall + agentid + response_code + button 或 template_card）。
- **返回**：`UpdateTemplateCardResponse` — 更新结果（invaliduser 无效用户列表）。

**补充说明**

- 仅原卡片为按钮交互型、投票选择型、多项选择型的卡片， 以及填写了 action_menu 字段的文本通知型、图文展示型可以调用本接口更新。
- 支持两种模式：更新按钮为不可点击状态（button.replace_name），或更新为新的卡片（template_card）； response_code 可通过发送模版卡片消息接口与用户点击卡片的回调事件获取， 一个 code 只能调用一次且有效期 72 小时。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94888
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94945
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96459

### 25. `RecallMessageAsync` — 撤回应用消息

- **声明接口**：[IWechatWorkMessageService](./IWechatWorkMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/message/recall`

- **请求体**：`RecallMessageRequest` — 撤回请求体（`RecallMessageRequest`：msgid，从发送应用消息接口处获得）。
- **返回**：`RecallMessageResponse` — 撤回结果（仅 errcode / errmsg）。

**补充说明**

- 撤回 24 小时内通过发送应用消息接口推送的消息； 仅可撤回企业微信端的数据，微信插件端的数据不支持撤回。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94867
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94947
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96460

### 26. `SendTemplateMsgAsync` — 发送模板消息（仅第三方应用）

- **声明接口**：[IWechatWorkThirdPartyMessageService](./IWechatWorkMessageService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/message/send`

- **请求体**：`MessageSendTemplateMsgRequest` — 发送请求体（`MessageSendTemplateMsgRequest`：公共信封 + selected_ticket_list / only_unauth + template_msg 消息体）。
- **返回**：`MessageSendResponse` — 发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。

**补充说明**

- msgtype = `template_msg`：固定格式的模板消息，经发送应用消息端点发送； template_id 为服务商管理端创建模板后获得（正式授权的应用需审批通过），最长 64 字节， 接口传参的内容必须与申请的模版匹配；url 与 miniprogram 至少要填一个，都填时优先 miniprogram。
- 成员授权模式下，对于不在可见范围内的成员，可通过传入合法且未过期的 selected_ticket_list（选人 sdk / 选人 jsapi 返回，不超过 10 个）推送模板消息； 仅向未授权用户发送时可开启 only_unauth（此时自动忽略 touser / toparty / totag）。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94515（模板消息经发送应用消息端点 `/cgi-bin/message/send` 发送，官方页面未单独标注路由，以正文表述为准）。

### 27. `SendSchoolTextMessageAsync` — 发送文本学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendTextRequest` — 发送请求体（`SchoolSendTextRequest`：学校通知信封 + text 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `text`：消息内容最长不超过 2048 个字节，超过将截断（支持 id 转译）。

### 28. `SendSchoolImageMessageAsync` — 发送图片学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendImageRequest` — 发送请求体（`SchoolSendImageRequest`：学校通知信封 + image 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `image`：图片媒体文件经上传临时素材接口获取。

### 29. `SendSchoolVoiceMessageAsync` — 发送语音学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendVoiceRequest` — 发送请求体（`SchoolSendVoiceRequest`：学校通知信封 + voice 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `voice`：语音媒体文件经上传临时素材接口获取。

### 30. `SendSchoolVideoMessageAsync` — 发送视频学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendVideoRequest` — 发送请求体（`SchoolSendVideoRequest`：学校通知信封 + video 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `video`：标题不超过 128 字节、描述不超过 512 字节，超过自动截断。

### 31. `SendSchoolFileMessageAsync` — 发送文件学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendFileRequest` — 发送请求体（`SchoolSendFileRequest`：学校通知信封 + file 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `file`：文件经上传临时素材接口获取。

### 32. `SendSchoolNewsMessageAsync` — 发送图文学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendNewsRequest` — 发送请求体（`SchoolSendNewsRequest`：学校通知信封 + news 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `news`：一个图文消息支持 1 到 8 条图文，articles[].url 必填（支持 id 转译）。

### 33. `SendSchoolMpNewsMessageAsync` — 发送 mpnews 图文学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendMpNewsRequest` — 发送请求体（`SchoolSendMpNewsRequest`：学校通知信封 + mpnews 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `mpnews`：与普通图文一致，唯一差异是图文内容存储在企业微信； 多次发送会被认为是不同的图文，阅读、点赞统计分开计算（支持 id 转译）。

### 34. `SendSchoolMiniProgramMessageAsync` — 发送小程序学校通知

- **声明接口**：[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalcontact/message/send`

- **请求体**：`SchoolSendMiniProgramRequest` — 发送请求体（`SchoolSendMiniProgramRequest`：学校通知信封 + miniprogram 消息体）。
- **返回**：`SchoolMessageSendResponse` — 发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。

**补充说明**

- msgtype = `miniprogram`：appid 必须是关联到企业的小程序应用； 封面 thumb_media_id 建议尺寸 520*416（支持 id 转译）。

**备注（原文）**

- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96723

### 35. `GetSmartSheetGroupChatListAsync` — 获取智能表格自动化创建的群聊列表

- **声明接口**：[IWechatWorkInternalSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/wedoc/smartsheet/groupchat/list`

- **请求体**：`GetSmartSheetGroupChatListRequest` — 查询请求体（`GetSmartSheetGroupChatListRequest`：docid / cursor / limit）。
- **返回**：`GetSmartSheetGroupChatListResponse` — 群聊 chatid 列表（chat_id_list）、是否还有更多数据（has_more）与下一页游标（next_cursor）。

**补充说明**

- 获取当前应用创建的智能表格经自动化规则创建的群聊列表（分页）。
- 官方不保证每次返回的数据刚好为 limit，必须以返回的 has_more 判断是否继续请求； 并发限制 20。

**备注（原文）**

- 群聊会话详情经 `GetSmartSheetGroupChatAsync` 查询。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/100989

### 36. `GetSmartSheetGroupChatAsync` — 获取智能表格自动化创建的群聊会话

- **声明接口**：[IWechatWorkInternalSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/wedoc/smartsheet/groupchat/get`

- **请求体**：`GetSmartSheetGroupChatRequest` — 查询请求体（`GetSmartSheetGroupChatRequest`：docid / chat_id）。
- **返回**：`GetSmartSheetGroupChatResponse` — 群聊会话信息（name / owner / user_list）。

**补充说明**

- 通过智能表格 ID 与群聊 ID 获取群聊会话信息（群聊名称、群主、群成员列表）。
- 群成员列表仅返回当前企业成员；并发限制 20。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/101028

### 37. `UpdateSmartSheetGroupChatAsync` — 修改智能表格自动化创建的群聊会话

- **声明接口**：[IWechatWorkInternalSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService_Internal.cs)
- **官方契约**：`POST /cgi-bin/wedoc/smartsheet/groupchat/update`

- **请求体**：`UpdateSmartSheetGroupChatRequest` — 修改请求体（`UpdateSmartSheetGroupChatRequest`：docid / chat_id / owner / add_user_list / del_user_list）。
- **返回**：`UpdateSmartSheetGroupChatResponse` — 修改结果（仅 errcode / errmsg）。

**补充说明**

- 变更群主与群成员（add_user_list / del_user_list 一次最多各 500 人）； del_user_list 包含群主时 owner 必填，群主需为本企业成员，操作的成员需为对应智能表格中的成员。
- 群成员人数不可超过 2000 人；每企业变更群的次数不可超过 1000 次/小时； **同一群聊的修改需要串行执行**（勿并发调用）；并发限制 10。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/101029

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work.DataModels/Message`](../../../Mud.Wechat.Work.DataModels/Message/) 内共 **87** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **87** 个，`SerializerClassName` = `Message`。

AOT 源生成上下文：[`MessageJsonContext.g.cs`](./../../../Mud.Wechat.Work.DataModels/Generated/MessageJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CreateAppChatResponse` | 应答 | [Message/CreateAppChatResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/CreateAppChatResponse.cs) | `chatid` |
| `CreateAppChatRequest` | 请求体 | [Message/RequestModel/CreateAppChatRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/CreateAppChatRequest.cs) | `name`, `owner`, `userlist`, `chatid` |
| `UpdateAppChatResponse` | 应答 | [Message/UpdateAppChatResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/UpdateAppChatResponse.cs) | — |
| `UpdateAppChatRequest` | 请求体 | [Message/RequestModel/UpdateAppChatRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/UpdateAppChatRequest.cs) | `chatid`, `name`, `owner`, `add_user_list`, `del_user_list` |
| `GetAppChatResponse` | 应答 | [Message/GetAppChatResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/GetAppChatResponse.cs) | `chat_info` |
| `AppChatSendResponse` | 应答 | [Message/AppChatSendResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/AppChatSendResponse.cs) | — |
| `AppChatSendTextRequest` | 请求体 | [Message/RequestModel/AppChatSendTextRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendTextRequest.cs) | `text`, `safe` |
| `AppChatSendImageRequest` | 请求体 | [Message/RequestModel/AppChatSendImageRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendImageRequest.cs) | `image`, `safe` |
| `AppChatSendVoiceRequest` | 请求体 | [Message/RequestModel/AppChatSendVoiceRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendVoiceRequest.cs) | `voice` |
| `AppChatSendVideoRequest` | 请求体 | [Message/RequestModel/AppChatSendVideoRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendVideoRequest.cs) | `video`, `safe` |
| `AppChatSendFileRequest` | 请求体 | [Message/RequestModel/AppChatSendFileRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendFileRequest.cs) | `file`, `safe` |
| `AppChatSendTextCardRequest` | 请求体 | [Message/RequestModel/AppChatSendTextCardRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendTextCardRequest.cs) | `textcard` |
| `AppChatSendNewsRequest` | 请求体 | [Message/RequestModel/AppChatSendNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendNewsRequest.cs) | `news` |
| `AppChatSendMpNewsRequest` | 请求体 | [Message/RequestModel/AppChatSendMpNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendMpNewsRequest.cs) | `mpnews`, `safe` |
| `AppChatSendMarkdownRequest` | 请求体 | [Message/RequestModel/AppChatSendMarkdownRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/AppChatSendMarkdownRequest.cs) | `markdown` |
| `MessageSendResponse` | 应答 | [Message/MessageSendResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/MessageSendResponse.cs) | `invaliduser`, `invalidparty`, `invalidtag`, `unlicenseduser`, `msgid`, `response_code` |
| `MessageSendTextRequest` | 请求体 | [Message/RequestModel/MessageSendTextRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendTextRequest.cs) | `text`, `safe` |
| `MessageSendImageRequest` | 请求体 | [Message/RequestModel/MessageSendImageRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendImageRequest.cs) | `image`, `safe` |
| `MessageSendVoiceRequest` | 请求体 | [Message/RequestModel/MessageSendVoiceRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendVoiceRequest.cs) | `voice` |
| `MessageSendVideoRequest` | 请求体 | [Message/RequestModel/MessageSendVideoRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendVideoRequest.cs) | `video`, `safe` |
| `MessageSendFileRequest` | 请求体 | [Message/RequestModel/MessageSendFileRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendFileRequest.cs) | `file`, `safe` |
| `MessageSendTextCardRequest` | 请求体 | [Message/RequestModel/MessageSendTextCardRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendTextCardRequest.cs) | `textcard` |
| `MessageSendNewsRequest` | 请求体 | [Message/RequestModel/MessageSendNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendNewsRequest.cs) | `news` |
| `MessageSendMpNewsRequest` | 请求体 | [Message/RequestModel/MessageSendMpNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendMpNewsRequest.cs) | `mpnews`, `safe` |
| `MessageSendMarkdownRequest` | 请求体 | [Message/RequestModel/MessageSendMarkdownRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendMarkdownRequest.cs) | `markdown` |
| `MessageSendMiniProgramNoticeRequest` | 请求体 | [Message/RequestModel/MessageSendMiniProgramNoticeRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendMiniProgramNoticeRequest.cs) | `miniprogram_notice` |
| `MessageSendTemplateCardRequest` | 请求体 | [Message/RequestModel/MessageSendTemplateCardRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendTemplateCardRequest.cs) | `template_card` |
| `UpdateTemplateCardResponse` | 应答 | [Message/UpdateTemplateCardResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/UpdateTemplateCardResponse.cs) | `invaliduser` |
| `UpdateTemplateCardRequest` | 请求体 | [Message/RequestModel/UpdateTemplateCardRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/UpdateTemplateCardRequest.cs) | `userids`, `partyids`, `tagids`, `atall`, `agentid`, `response_code`, `enable_id_trans`, `button`, `template_card` |
| `RecallMessageResponse` | 应答 | [Message/RecallMessageResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/RecallMessageResponse.cs) | — |
| `RecallMessageRequest` | 请求体 | [Message/RequestModel/RecallMessageRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/RecallMessageRequest.cs) | `msgid` |
| `MessageSendTemplateMsgRequest` | 请求体 | [Message/RequestModel/MessageSendTemplateMsgRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/MessageSendTemplateMsgRequest.cs) | `selected_ticket_list`, `only_unauth`, `template_msg` |
| `SchoolMessageSendResponse` | 应答 | [Message/SchoolMessageSendResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/SchoolMessageSendResponse.cs) | `invalid_parent_userid`, `invalid_student_userid`, `invalid_party` |
| `SchoolSendTextRequest` | 请求体 | [Message/RequestModel/SchoolSendTextRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendTextRequest.cs) | `text` |
| `SchoolSendImageRequest` | 请求体 | [Message/RequestModel/SchoolSendImageRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendImageRequest.cs) | `image` |
| `SchoolSendVoiceRequest` | 请求体 | [Message/RequestModel/SchoolSendVoiceRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendVoiceRequest.cs) | `voice` |
| `SchoolSendVideoRequest` | 请求体 | [Message/RequestModel/SchoolSendVideoRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendVideoRequest.cs) | `video` |
| `SchoolSendFileRequest` | 请求体 | [Message/RequestModel/SchoolSendFileRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendFileRequest.cs) | `file` |
| `SchoolSendNewsRequest` | 请求体 | [Message/RequestModel/SchoolSendNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendNewsRequest.cs) | `news` |
| `SchoolSendMpNewsRequest` | 请求体 | [Message/RequestModel/SchoolSendMpNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendMpNewsRequest.cs) | `mpnews` |
| `SchoolSendMiniProgramRequest` | 请求体 | [Message/RequestModel/SchoolSendMiniProgramRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/SchoolSendMiniProgramRequest.cs) | `miniprogram` |
| `GetSmartSheetGroupChatListResponse` | 应答 | [Message/GetSmartSheetGroupChatListResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/GetSmartSheetGroupChatListResponse.cs) | `has_more`, `next_cursor`, `chat_id_list` |
| `GetSmartSheetGroupChatListRequest` | 请求体 | [Message/RequestModel/GetSmartSheetGroupChatListRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/GetSmartSheetGroupChatListRequest.cs) | `docid`, `cursor`, `limit` |
| `GetSmartSheetGroupChatResponse` | 应答 | [Message/GetSmartSheetGroupChatResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/GetSmartSheetGroupChatResponse.cs) | `name`, `owner`, `user_list` |
| `GetSmartSheetGroupChatRequest` | 请求体 | [Message/RequestModel/GetSmartSheetGroupChatRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/GetSmartSheetGroupChatRequest.cs) | `docid`, `chat_id` |
| `UpdateSmartSheetGroupChatResponse` | 应答 | [Message/UpdateSmartSheetGroupChatResponse.cs](../../../Mud.Wechat.Work.DataModels/Message/UpdateSmartSheetGroupChatResponse.cs) | — |
| `UpdateSmartSheetGroupChatRequest` | 请求体 | [Message/RequestModel/UpdateSmartSheetGroupChatRequest.cs](../../../Mud.Wechat.Work.DataModels/Message/RequestModel/UpdateSmartSheetGroupChatRequest.cs) | `docid`, `chat_id`, `owner`, `add_user_list`, `del_user_list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Message;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMessageApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalAppChatService>();
var ct = CancellationToken.None;
var response = await service.CreateAppChatAsync(new CreateAppChatRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/appchat/create");
// 占位值仅用于编译示意；业务值须按 IWechatWorkAppChatService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateAppChatAsync`）；请求体 `CreateAppChatRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkAppChatService](./IWechatWorkAppChatService.cs)、[IWechatWorkInternalAppChatService](./IWechatWorkAppChatService_Internal.cs)、[IWechatWorkMessageService](./IWechatWorkMessageService.cs)、[IWechatWorkInternalMessageService](./IWechatWorkMessageService_Internal.cs)、[IWechatWorkProviderMessageService](./IWechatWorkMessageService_Provider.cs)、[IWechatWorkThirdPartyMessageService](./IWechatWorkMessageService_ThirdParty.cs)、[IWechatWorkSchoolMessageService](./IWechatWorkSchoolMessageService.cs)、[IWechatWorkInternalSchoolMessageService](./IWechatWorkSchoolMessageService_Internal.cs)、[IWechatWorkProviderSchoolMessageService](./IWechatWorkSchoolMessageService_Provider.cs)、[IWechatWorkThirdPartySchoolMessageService](./IWechatWorkSchoolMessageService_ThirdParty.cs)、[IWechatWorkSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService.cs)、[IWechatWorkInternalSmartSheetGroupChatService](./IWechatWorkSmartSheetGroupChatService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatMessageContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `MessageEndpoints_ShouldMatchOfficialRoutes`、`MessageInterfaceHierarchy_ShouldConvergeOnMessageRegistry`、`MessageTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`MessageDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work.DataModels/Message`](../../../Mud.Wechat.Work.DataModels/Message/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatMessageContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
