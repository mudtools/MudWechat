# Mud.Wechat · SDK 开发文档

七线共仓的微信生态 .NET SDK。本目录是**面向调用方**的开发文档；仓库内的域 README（`Src/**/Interfaces/**/README.md`）与 `.docs/` 设计方案面向维护者，两者视角不同、互不取代。

规范见 [Documents/_spec/文档编写规范.md](../_spec/文档编写规范.md)。

## 从哪读起

| 你是 | 先读 |
|---|---|
| 第一次接入 | `guide/快速开始.md`（未成文）→ 你所在产品线的 `reference/<线>/` |
| 要调某个具体接口 | 按源码路径直接映射到 `reference/`（见下）→ 该族的端点小节 |
| 排查凭据/令牌问题 | `guide/配置与凭据.md`（未成文）+ 该族文档的 §2 前置条件、§6 失败处理 |
| 想知道某域有哪些端点 | 该域的源码 `Interfaces/<域>/README.md`（域总览，端点速览表在此，不重抄） |

## 路径映射

```
Src/<线>/<主包>/Interfaces/<域>/<族>/   ⇄   reference/<线>/<域>/<族>.md
```

例：`Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Bot/` ⇄ [reference/Work/AccountId/Bot.md](./reference/Work/AccountId/Bot.md)

段名照抄源码目录，不翻译不改名。因此**从源码目录能直接推出文档路径，反之亦然**，覆盖度可脚本核对。

## 覆盖进度

端点数与业务域数取仓库 [README.md](../../README.md) 的「七条产品线，一个底座」表口径（该表未统一计量单位：`Work` 记契约接口数、`Channels` 记已落地端点数），此处只作规模参考；**逐端点的权威计数一律以各域契约守卫为准**。

| 产品线 | 主包 | 业务域 | 端点/契约接口 | 已成文 | 已核验 |
|---|---|---|---|---|---|
| 企业微信（WeCom） | `Mud.Wechat.Work` | 42 | 470 | **125 族（35 域：`AccountId` / `AdvancedApi` / `AdvancedFeature` / `Agent` / `Aibot` / `Approval` / `Authentication` / `Basic` / `Checkin` / `Contacts` / `CorpGroup` / `DataZone` / `DeviceData` / `Dial` / `Emergency` / `ExternalContact` / `Gov` / `Hr` / `Identity` / `Invoice` / `Journal` / `JsSdk` / `KF` / `License` / `Living` / `Mail` / `Media` / `Meeting` / `MeetingRoom` / `Message` / `MsgAudit` / `OpenHardware` / `Pay` / `PayTool` / `PromotionQrCode`）** | 124 |
| 微信公众号 | `Mud.Wechat.OfficialAccount` | 27 | 194 | 0 | 0 |
| 微信小程序 | `Mud.Wechat.MiniProgram` | 21 | 141 | 0 | 0 |
| 微信支付 APIv3/v2 | `Mud.Wechat.Pay` | 10 | 57 | 0 | 0 |
| 微信开放平台 | `Mud.Wechat.OpenPlatform` | — | 4 | 0 | 0 |
| 微信小店 / 视频号 | `Mud.Wechat.Channels` | 27 | 309 | 0 | 0 |
| 腾讯广告（在建） | `Mud.Wechat.Ads` | 8 | 33 | 0 | 0 |
| 跨线基建（叶层 / Redis / OpenTelemetry / AI） | `Mud.Wechat.Abstractions` 等 | — | — | 0 | 0 |

已成文 125 篇（`AccountId` 域 7 族 20 支端点；`AdvancedApi` / `AdvancedFeature` / `Agent` / `Aibot` / `Approval` 五域 11 族 38 支端点方法 / 36 个端点小节——`ApprovalTemplate.md` 的创建与更新模板在自建与代开发子接口各声明一次、同路由，合为同一小节；`Authentication` 域 3 族 9 支端点；`Basic` 域 1 族 2 支端点；`Checkin` 域 5 族 15 条路由 / 26 个方法声明；`Contacts` 域 6 族 52 个方法声明 / 44 条去重路由——差异只来自成员与部门把写入/邀请端点在自建与第三方两支各声明一次；`CorpGroup` 域 3 族 20 支端点；`DataZone` 域 2 族 18 个方法声明 / 17 条去重路由；`DeviceData` 域 1 族 8 支端点；`Dial` 域 1 族 1 支端点；`Emergency` 域 1 族 2 支端点；`ExternalContact` 域 16 族 95 支端点（16 支域守卫逐族 `HaveCount` 之和恰为 95）；`Gov` 域 4 篇覆盖 5 个根接口 21 支端点；`Hr` 域 1 族 3 支端点；`Identity` 域 3 篇覆盖 5 个根接口 8 支端点；`Invoice` 域 1 族 4 支端点；`Journal` 域 1 族 3 支端点；`JsSdk` 域 1 族 2 支端点；`KF` 域 8 族 38 支端点方法 / 29 条去重路由；`License` 域 4 族 27 支端点；`Living` 域 1 族 9 支端点；`Media` 域 2 族 7 支端点方法 / 7 条去重路由（公共面 6 支三类等宽 + 服务商通道 1 支仅第三方）；`MeetingRoom` 域 2 族 10 支端点（**全部仅自建**，OA 会议室与 `meeting/rooms/` 两棵树勿混）；`Mail` 域 8 族 26 支端点方法 / 24 条去重路由（`compose_send` 三支同路由；三支三类等宽 + 五支仅自建）；`Meeting` 域 11 族 **106 支端点方法 / 105 条去重路由**（唯一重复即 `get_info` 在自建与第三方各声明一次；十一族中仅基础族向三类应用开放、全域唯一 GET 为 `layout/list_template`）；`Message` 域 4 族 37 支端点 / 11 条去重路由（应用消息 14 + 群聊会话 12 + 学校通知 8 + 智能表格群聊 3；`message/send` 按 `msgtype` 拆成多方法故方法数远多于路由数）；`MsgAudit` 域 4 族 5 支端点（会话内容存档：同意情况 2 + 内部群 1 + 开启成员 1 + 机器人 1）；`OpenHardware` 域 1 族 17 支端点（型号/设备/成员凭证 + 七类硬件上报；`device_secret` 与成员身份不落日志）；`Pay` 域 7 族 15 支端点（对外收款记录 2 + 资金流水 1 + 进件 3 + 商户号 2 + 小程序下单 4 + 退款 2 + 交易账单 1；仅 `Bill` 为三类应用公共面，其余六族仅自建）；`PayTool` 域 3 族 9 支端点（收款工具 4 支**须签名** + 发票管理 2 支 + 应用版本付费 3 支；前两族 `provider_access_token`、末族 `suite_access_token`，三族均零端点父 + 唯一第三方子接口）；`PromotionQrCode` 域 2 族 4 支端点（企业注册 2 支走 `provider_access_token`；通讯录迁移 2 支为迁移凭证显式传参、**不声明 `[Token]`**））/ Work 线尚余 **7 域 / 32 族 / 188 支端点声明**（`Schedule` 3 族 13 支 · `School` 8 族 45 支 · `Security` 3 族 16 支 · `Service` 2 族 5 支 · `Webhook` 1 族 9 支 · `Wedoc` 9 族 69 支 · `Wedrive` 6 族 31 支；由 `Src/Work/Mud.Wechat.Work/Interfaces/` 与 `reference/Work/` 目录逐域比对得出，端点数按接口文件 `[Post]` / `[Get]` 实计）；其余六线（公众号 / 小程序 / 支付 / 开放平台 / 小店 / 广告）尚未成文。

### 已成文清单

| 文档 | 端点 | 令牌族 | 状态 |
|---|---|---|---|
| [IdConvert.md](./reference/Work/AccountId/IdConvert.md) | 11 | `access_token` | `已核验` |
| [Interop.md](./reference/Work/AccountId/Interop.md) | 3 | `access_token`（自建） | `已核验` |
| [Bot.md](./reference/Work/AccountId/Bot.md) | 1 | `provider_access_token` | `已核验` |
| [Corpid.md](./reference/Work/AccountId/Corpid.md) | 1 | `provider_access_token` | `已核验` |
| [Migration.md](./reference/Work/AccountId/Migration.md) | 2 | `provider_access_token` | `待补充`（`agentid` 必填性内部证据矛盾） |
| [TmpExternalUserId.md](./reference/Work/AccountId/TmpExternalUserId.md) | 1 | `access_token`（三类应用） | `已核验` |
| [ChatIdUpgrade.md](./reference/Work/AccountId/ChatIdUpgrade.md) | 1 | `suite_access_token` | `已核验` |
| [AdvancedApi.md](./reference/Work/AdvancedApi/AdvancedApi.md) | 6 | `provider_access_token` | `已核验` |
| [AdvancedFeature.md](./reference/Work/AdvancedFeature/AdvancedFeature.md) | 3 | `access_token`（自建） | `已核验` |
| [Agent.md](./reference/Work/Agent/Agent.md) | 5 | `access_token`（三类应用） | `已核验` |
| [AgentWorkbench.md](./reference/Work/Agent/AgentWorkbench.md) | 5 | `access_token`（三类应用） | `已核验` |
| [AgentMenu.md](./reference/Work/Agent/AgentMenu.md) | 3 | `access_token`（自建） | `已核验` |
| [AgentMigration.md](./reference/Work/Agent/AgentMigration.md) | 1 | `access_token`（待迁移自建应用）＋ 请求体内 `suite_access_token`（代开发套件） | `已核验` |
| [Aibot.md](./reference/Work/Aibot/Aibot.md) | 1 | 无令牌（`response_code` 一次性 URL 凭据） | `已核验` |
| [Approval.md](./reference/Work/Approval/Approval.md) | 4 | `access_token`（三类应用；旧版数据仅自建） | `已核验` |
| [ApprovalTemplate.md](./reference/Work/Approval/ApprovalTemplate.md) | 4（6 个方法声明） | `access_token`（自建 / 代开发；复制到企业仅第三方） | `已核验` |
| [Vacation.md](./reference/Work/Approval/Vacation.md) | 3 | `access_token`（三类应用） | `已核验` |
| [ApprovalEngine.md](./reference/Work/Approval/ApprovalEngine.md) | 1 | `access_token`（三类应用） | `已核验` |
| [ProviderAuthentication.md](./reference/Work/Authentication/ProviderAuthentication.md) | 7 | `suite_access_token` / `provider_access_token` | `已核验` |
| [ProviderAuthenticationUrl.md](./reference/Work/Authentication/ProviderAuthenticationUrl.md) | 1 | `provider_access_token`（显式 Query，无 `[Token]`） | `已核验` |
| [ProviderLoginService.md](./reference/Work/Authentication/ProviderLoginService.md) | 1 | `provider_access_token` | `已核验` |
| [Basic.md](./reference/Work/Basic/Basic.md) | 2 | `access_token`（自建 / 代开发） | `已核验` |
| [Rule.md](./reference/Work/Checkin/Rule.md) | 6（11 个方法声明） | `access_token`（三类应用；管理类仅自建 / 代开发） | `已核验` |
| [Record.md](./reference/Work/Checkin/Record.md) | 4（6 个方法声明） | `access_token`（三类应用；3 支写端点仅自建） | `已核验` |
| [Report.md](./reference/Work/Checkin/Report.md) | 2（6 个方法声明） | `access_token`（三类应用；第三方为旧结构） | `已核验` |
| [Schedule.md](./reference/Work/Checkin/Schedule.md) | 2 | `access_token`（三类应用） | `已核验` |
| [Device.md](./reference/Work/Checkin/Device.md) | 1 | `access_token`（三类应用；第三方需设备型号关联） | `已核验` |
| [Users.md](./reference/Work/Contacts/Users.md) | 18（23 个方法声明） | `access_token`（三类应用；写入面仅自建 / 第三方，成员授权三支仅第三方） | `已核验` |
| [Departments.md](./reference/Work/Contacts/Departments.md) | 6（9 个方法声明） | `access_token`（三类应用；写入面仅自建 / 第三方） | `已核验` |
| [Tags.md](./reference/Work/Contacts/Tags.md) | 7 | `access_token`（三类应用一致开放） | `已核验` |
| [Batch.md](./reference/Work/Contacts/Batch.md) | 4 | `access_token`（自建 + 第三方；**官方无代开发子接口**） | `已核验` |
| [Export.md](./reference/Work/Contacts/Export.md) | 5 | `access_token`（三类应用一致开放） | `已核验` |
| [ContactRules.md](./reference/Work/Contacts/ContactRules.md) | 4 | `access_token`（**仅自建**，且仅通讯录同步应用） | `已核验` |
| [CorpGroup.md](./reference/Work/CorpGroup/CorpGroup.md) | 6 | `access_token`（E1 三类；E2–E6 仅自建 / 代开发；E3 消费下级企业凭证） | `已核验` |
| [CorpGroupContacts.md](./reference/Work/CorpGroup/CorpGroupContacts.md) | 9 | `access_token`（读取面自建 + 代开发；导入 / 移除 / 反查仅自建） | `已核验` |
| [CorpGroupRules.md](./reference/Work/CorpGroup/CorpGroupRules.md) | 5 | `access_token`（**仅自建**，且仅创建空间的主企业） | `已核验` |
| [DataZone.md](./reference/Work/DataZone/DataZone.md) | 14（15 个方法声明） | `access_token`（公共面三类；`get_corp_auth_info` 仅第三方 + 代开发；文档存档仅第三方） | `已核验` |
| [DataZoneProgram.md](./reference/Work/DataZone/DataZoneProgram.md) | 3 | `access_token`（三类应用一致开放） | `已核验` |
| [DeviceData.md](./reference/Work/DeviceData/DeviceData.md) | 8 | `access_token`（**仅第三方**，企业级令牌；E1–E7 响应体不建模 `errcode`） | `已核验` |
| [Dial.md](./reference/Work/Dial/Dial.md) | 1 | `access_token`（**仅自建**；第三方 / 代开发官方未开放） | `已核验` |
| [Emergency.md](./reference/Work/Emergency/Emergency.md) | 2 | `access_token`（**仅自建**；紧急电话呼叫 + 呼叫状态） | `已核验` |
| [Customer.md](./reference/Work/ExternalContact/Customer.md) | 11 | `access_token`（三类应用；`to_service_external_userid` 仅第三方） | `已核验` |
| [GroupMsg.md](./reference/Work/ExternalContact/GroupMsg.md) | 11 | `access_token`（三类应用） | `已核验` |
| [Moment.md](./reference/Work/ExternalContact/Moment.md) | 14 | `access_token`（三类应用；取任务结果 1 支为 GET） | `已核验` |
| [ContactWay.md](./reference/Work/ExternalContact/ContactWay.md) | 10 | `access_token`（三类应用；读取亦为 POST） | `已核验` |
| [Tag.md](./reference/Work/ExternalContact/Tag.md) | 9 | `access_token`（三类应用；企业标签 + 规则组标签两套） | `已核验` |
| [CustomerAcquisition.md](./reference/Work/ExternalContact/CustomerAcquisition.md) | 9 | `access_token`（三类应用；配额查询 1 支为 GET） | `已核验` |
| [ProductAlbum.md](./reference/Work/ExternalContact/ProductAlbum.md) | 5 | `access_token`（三类应用） | `已核验` |
| [InterceptRule.md](./reference/Work/ExternalContact/InterceptRule.md) | 5 | `access_token`（三类应用；列表为 GET、详情为 POST） | `已核验` |
| [ResignedInheritance.md](./reference/Work/ExternalContact/ResignedInheritance.md) | 4 | `access_token`（三类应用） | `已核验` |
| [GroupChat.md](./reference/Work/ExternalContact/GroupChat.md) | 3 | `access_token`（三类应用） | `已核验` |
| [JobInheritance.md](./reference/Work/ExternalContact/JobInheritance.md) | 3 | `access_token`（三类应用） | `已核验` |
| [Statistics.md](./reference/Work/ExternalContact/Statistics.md) | 3 | `access_token`（三类应用；本域唯一 `offset` + `limit` 分页） | `已核验` |
| [FollowUser.md](./reference/Work/ExternalContact/FollowUser.md) | 3 | `access_token`（三支分属：三类通用 1 / 仅第三方 1 / 仅代开发 1） | `已核验` |
| [AcquisitionComponent.md](./reference/Work/ExternalContact/AcquisitionComponent.md) | 3 | `access_token`（第三方 2 支）＋ `suite_access_token`（账单 1 支，套件级、无企业 scope） | `已核验` |
| [ServedContact.md](./reference/Work/ExternalContact/ServedContact.md) | 1 | `access_token`（**仅自建**，父接口零端点） | `已核验` |
| [Attachment.md](./reference/Work/ExternalContact/Attachment.md) | 1 | `access_token`（三类应用；multipart 上传腿在生成器 3.0.9 下不传文件字节） | `已核验` |
| [EventCategory.md](./reference/Work/Gov/EventCategory.md) | 4 | `access_token`（自建 + 代开发；**第三方暂不支持**） | `已核验` |
| [Grid.md](./reference/Work/Gov/Grid.md) | 5 | `access_token`（E1–E4 自建 + 代开发；E5 `grid/list` **仅自建** ⇒ 独立成族） | `已核验` |
| [Patrol.md](./reference/Work/Gov/Patrol.md) | 6 | `access_token`（**仅自建**；代开发文档页存在但权限表「暂不支持」） | `已核验` |
| [Resident.md](./reference/Work/Gov/Resident.md) | 6 | `access_token`（**仅自建**，同上） | `已核验` |
| [Hr.md](./reference/Work/Hr/Hr.md) | 3 | `access_token`（**仅自建**；零端点父接口 + 自建子接口承载） | `已核验` |
| [Identity.md](./reference/Work/Identity/Identity.md) | 4 | `access_token`（自建 / 代开发）＋ `suite_access_token`（第三方独立路由 `service/auth/*3rd`） | `已核验` |
| [IdentityMiniProgram.md](./reference/Work/Identity/IdentityMiniProgram.md) | 2 | `access_token`（自建 / 代开发）＋ `suite_access_token`（第三方独立路由，响应多 `open_userid`） | `已核验` |
| [IdentityTfa.md](./reference/Work/Identity/IdentityTfa.md) | 2 | `access_token`（**仅自建 / 通讯录同步**；`authsucc` 归通讯录域，守卫 IDN5） | `已核验` |
| [Invoice.md](./reference/Work/Invoice/Invoice.md) | 4 | `access_token`（三类应用等宽；E4 另需企业激活人数超 200） | `已核验` |
| [Journal.md](./reference/Work/Journal/Journal.md) | 3 | `access_token`（**仅自建**；汇报文档树只在自建根下） | `已核验` |
| [JsSdk.md](./reference/Work/JsSdk/JsSdk.md) | 2 | `access_token`（三类应用；票据须宿主缓存，SDK 不代缓存） | `已核验` |
| [KfAccount.md](./reference/Work/KF/KfAccount.md) | 5 | `access_token`（三类应用；组件版 99368/99400 同路由 ⇒ 单一所有者） | `已核验` |
| [KfServicer.md](./reference/Work/KF/KfServicer.md) | 3 | `access_token`（三类应用；添加部门 ≤20、删除各 ≤100） | `已核验` |
| [KfSession.md](./reference/Work/KF/KfSession.md) | 15（5 条路由） | `access_token`（三类应用；`sync_msg` 请求体 `token` 是回调同步令牌，非 `[Token]`） | `已核验` |
| [KfCustomer.md](./reference/Work/KF/KfCustomer.md) | 1 | `access_token`（三类应用；官方路由连写 `batchget`） | `已核验` |
| [KfStatistics.md](./reference/Work/KF/KfStatistics.md) | 2 | `access_token`（三类应用；31 天 / 180 天 / 次日可见） | `已核验` |
| [KfKnowledge.md](./reference/Work/KF/KfKnowledge.md) | 8 | `access_token`（**仅自建**；路由 `mod_group` / `mod_intent`） | `已核验` |
| [KfComponent.md](./reference/Work/KF/KfComponent.md) | 1 | `access_token`（企业级；**仅第三方/组件套件**，无归属键） | `已核验` |
| [KfUpgrade.md](./reference/Work/KF/KfUpgrade.md) | 3 | `access_token`（三类应用；E1 为 GET 且无参数） | `已核验` |
| [LicenseOrder.md](./reference/Work/License/LicenseOrder.md) | 15 | **`provider_access_token`**（第三方 + 代开发同页；四族均不声明归属键） | `已核验` |
| [LicenseAccount.md](./reference/Work/License/LicenseAccount.md) | 9 | `provider_access_token`（同上；8 支应答含密文 corpid/userid） | `已核验` |
| [LicenseApp.md](./reference/Work/License/LicenseApp.md) | 1 | `provider_access_token`（同上；字段名官方即 `trail_info`） | `已核验` |
| [LicenseAutoActive.md](./reference/Work/License/LicenseAutoActive.md) | 2 | `provider_access_token`（同上；前置为服务商已付费） | `已核验` |
| [Living.md](./reference/Work/Living/Living.md) | 9 | `access_token`（三类应用；仅能操作本应用创建的直播） | `已核验` |
| [Media.md](./reference/Work/Media/Media.md) | 6 | `access_token`（三类应用等宽；**两支返回 `byte[]?`，不可判 errcode**） | `已核验` |
| [ServiceMedia.md](./reference/Work/Media/ServiceMedia.md) | 1 | **`provider_access_token`**（仅第三方；零端点父 + 唯一子接口、无归属键） | `已核验` |
| [Booking.md](./reference/Work/MeetingRoom/Booking.md) | 6 | `access_token`（**仅自建**；必填性用类型系统表达） | `已核验` |
| [Room.md](./reference/Work/MeetingRoom/Room.md) | 4 | `access_token`（**仅自建**；`range` 覆盖写、无分页） | `已核验` |
| [Send.md](./reference/Work/Mail/Send.md) | 3 | `access_token`（三类应用；三支共用 `compose_send`） | `已核验` |
| [Receive.md](./reference/Work/Mail/Receive.md) | 2 | `access_token`（三类应用；邮箱由令牌自身定位、无邮箱 id） | `已核验` |
| [Account.md](./reference/Work/Mail/Account.md) | 2 | `access_token`（**仅自建**；`get_email_alias` 官方无请求包体） | `已核验` |
| [Group.md](./reference/Work/Mail/Group.md) | 5 | `access_token`（**仅自建**；`groupid` 即邮箱地址、两支官方即 GET） | `已核验` |
| [PublicMail.md](./reference/Work/Mail/PublicMail.md) | 7 | `access_token`（**仅自建**；`auth_code` 仅返回一次、只写参数名） | `已核验` |
| [User.md](./reference/Work/Mail/User.md) | 2 | `access_token`（**仅自建**；`userid` 与 `publicemail_id` 同传只操作前者） | `已核验` |
| [UserOption.md](./reference/Work/Mail/UserOption.md) | 2 | `access_token`（**仅自建**；`option.list` 的 `value` 是字符串 `"1"`/`"0"`） | `已核验` |
| [Vip.md](./reference/Work/Mail/Vip.md) | 3 | `access_token`（**仅自建**；同步批量 ≤100、`has_more` 为布尔） | `已核验` |
| [Meeting.md](./reference/Work/Meeting/Meeting.md) | 5（6 个方法声明） | `access_token`（三类应用；`get_info` 代开发无页 ⇒ 空标记） | `已核验` |
| [MeetingStatistics.md](./reference/Work/Meeting/MeetingStatistics.md) | 1 | `access_token`（**仅自建**；路由挂 `statistics/` 段） | `已核验` |
| [MeetingAdvanced.md](./reference/Work/Meeting/MeetingAdvanced.md) | 19 | `access_token`（**仅自建**；本域最大族，261 型中计 55 型） | `已核验` |
| [MeetingControl.md](./reference/Work/Meeting/MeetingControl.md) | 17 | `access_token`（**仅自建**；`realcontrol/` 9 + `poll/` 8） | `已核验` |
| [MeetingWebinar.md](./reference/Work/Meeting/MeetingWebinar.md) | 14 | `access_token`（**仅自建**；`title`/时间/状态为字符串形态） | `已核验` |
| [MeetingRooms.md](./reference/Work/Meeting/MeetingRooms.md) | 12 | `access_token`（**仅自建**；`get_inventory` 为无请求体的 POST） | `已核验` |
| [MeetingRecording.md](./reference/Work/Meeting/MeetingRecording.md) | 10 | `access_token`（**仅自建**；地址 6 小时、纪要与转写 1 小时） | `已核验` |
| [MeetingLayout.md](./reference/Work/Meeting/MeetingLayout.md) | 15 | `access_token`（**仅自建**；**含本域唯一 GET** `layout/list_template`） | `已核验` |
| [MeetingMra.md](./reference/Work/Meeting/MeetingMra.md) | 4 | `access_token`（**仅自建**；设备引用只有 `tmp_openid`） | `已核验` |
| [MeetingPstn.md](./reference/Work/Meeting/MeetingPstn.md) | 3 | `access_token`（**仅自建**；50 路 / 100 页 / 20 号码三套数值） | `已核验` |
| [MeetingPremiumAccount.md](./reference/Work/Meeting/MeetingPremiumAccount.md) | 5 | `access_token`（**仅自建**；`submit_*_job` + `*_job_result` 两段式） | `已核验` |
| [Message.md](./reference/Work/Message/Message.md) | 14 | `access_token`（E1–E13 三类等宽；E14 模板消息仅第三方） | `已核验` |
| [AppChat.md](./reference/Work/Message/AppChat.md) | 12 | `access_token`（**仅自建**；群聊会话管理 3 支 + 群内推送 9 种 `msgtype`） | `已核验` |
| [SchoolMessage.md](./reference/Work/Message/SchoolMessage.md) | 8 | `access_token`（三类应用等宽；`invalid_student` / `invalid_parent` 为个人数据） | `已核验` |
| [SmartSheetGroupChat.md](./reference/Work/Message/SmartSheetGroupChat.md) | 3 | `access_token`（**仅自建**；智能表格自动化建群） | `已核验` |
| [Agree.md](./reference/Work/MsgAudit/Agree.md) | 2 | `access_token`（**仅自建**；单聊 + 群聊同意情况） | `已核验` |
| [GroupChat.md](./reference/Work/MsgAudit/GroupChat.md) | 1 | `access_token`（**仅自建**；会话内容存档内部群信息） | `已核验` |
| [PermitUser.md](./reference/Work/MsgAudit/PermitUser.md) | 1 | `access_token`（**仅自建**；开启存档的成员列表） | `已核验` |
| [Robot.md](./reference/Work/MsgAudit/Robot.md) | 1 | `access_token`（**仅自建**；机器人信息） | `已核验` |
| [OpenHardware.md](./reference/Work/OpenHardware/OpenHardware.md) | 17 | **无 `[Token]`**（型号 / 设备 / 成员凭证均为显式 Query 参数；`device_secret` 类值只写参数名） | `已核验` |
| [Bill.md](./reference/Work/Pay/Bill.md) | 2 | `access_token`（**三类应用公共面**；第三方 / 代开发拿不到 `contact_info`） | `已核验` |
| [FundFlow.md](./reference/Work/Pay/FundFlow.md) | 1 | `access_token`（**仅自建**；间隔 ≤31 天、当日流水次日 11 点后生成） | `已核验` |
| [MchApply.md](./reference/Work/Pay/MchApply.md) | 3 | `access_token`（**仅自建**；图片上传腿在生成器 3.0.9 下不传文件字节） | `已核验` |
| [Merchant.md](./reference/Work/Pay/Merchant.md) | 2 | `access_token`（**仅自建**；`getmerchant` 单词连写、`allow_use_scope` 仅已绑定时返回） | `已核验` |
| [Order.md](./reference/Work/Pay/Order.md) | 4 | `access_token`（**仅自建**；小程序下单 / 查单 / 关单 / 取签名，`prepay_id` 2 小时） | `已核验` |
| [Refund.md](./reference/Work/Pay/Refund.md) | 2 | `access_token`（**仅自建**；金额单位分） | `已核验` |
| [TradeBill.md](./reference/Work/Pay/TradeBill.md) | 1 | `access_token`（**仅自建**；`download_url` 30 秒有效、`auth` 不落日志） | `已核验` |
| [PayTool/Order.md](./reference/Work/PayTool/Order.md) | 4 | **`provider_access_token`**（第三方 + 代开发同页；**请求体须带 `nonce_str` / `ts` / `sig`**） | `已核验` |
| [PayTool/Invoice.md](./reference/Work/PayTool/Invoice.md) | 2 | `provider_access_token`（同上；官方参数表**不含**签名三要素） | `已核验` |
| [PayTool/VersionSuite.md](./reference/Work/PayTool/VersionSuite.md) | 3 | **`suite_access_token`**（仅第三方 / 套件；字段名全小写 `orderid` / `suiteid`） | `已核验` |
| [ServicePromotionQrCode.md](./reference/Work/PromotionQrCode/ServicePromotionQrCode.md) | 2 | **`provider_access_token`**（仅第三方；零端点父 + 唯一子接口、无归属键） | `已核验` |
| [ContactSync.md](./reference/Work/PromotionQrCode/ContactSync.md) | 2 | **无 `[Token]`**（通讯录迁移凭证显式传参；E2 为 GET 却不可逆） | `已核验` |

`已核验` = 正文每个断言可指到内部证据或带日期官方取文 + 示例逐字编译通过（见规范 §7）；各篇 §7「已知留档缺口」登记的是官方未提供或本文不主张的项，不因缺口表存在而降级。

推进顺序建议：**先按线补 `guide/`（凭据模型各线不同，重复解释成本最高）→ 再按域补 L3（一次一个域，与该域守卫同批评审）→ 最后统一 `appendix/`**。

## 本项目的特殊约束（写文档前必须知道）

- **域 README 已是权威端点速览**：L3 文档不重抄速览表，只链接过去（规范 §3 的 L4 由源码目录 README 承担）。
- **契约守卫是路由与计量的事实来源**：`Tests/**/ContractGuards/`，改契约面必须同批改守卫与文档。
- **接口 XML 注释里的官方文档号只作定位标识**，不作为事实来源；需要官方正文时必须渲染后取文并记日期（本项目官方站正文由 JS 渲染，直接抓 HTML 只会拿到导航）。
- **锚点不一致要留档不要静默改写**：见样版文档 §7「已知留档缺口」的处理方式。
- 文档改动不进 CHANGELOG；但**契约面变更未同批改文档视为半成品**（对齐 `AGENTS.md` §2 同批纪律）。
