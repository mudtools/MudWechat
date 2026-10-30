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
| 企业微信（WeCom） | `Mud.Wechat.Work` | 35 | 446 | **66 族（20 域：`AccountId` / `AdvancedApi` / `AdvancedFeature` / `Agent` / `Aibot` / `Approval` / `Authentication` / `Basic` / `Checkin` / `Contacts` / `CorpGroup` / `DataZone` / `DeviceData` / `Dial` / `Emergency` / `ExternalContact` / `Gov` / `Hr` / `Identity` / `Invoice`）** | 65 |
| 微信公众号 | `Mud.Wechat.OfficialAccount` | 27 | 194 | 0 | 0 |
| 微信小程序 | `Mud.Wechat.MiniProgram` | 21 | 141 | 0 | 0 |
| 微信支付 APIv3/v2 | `Mud.Wechat.Pay` | 10 | 57 | 0 | 0 |
| 微信开放平台 | `Mud.Wechat.OpenPlatform` | — | 4 | 0 | 0 |
| 微信小店 / 视频号 | `Mud.Wechat.Channels` | 27 | 309 | 0 | 0 |
| 腾讯广告（在建） | `Mud.Wechat.Ads` | 8 | 33 | 0 | 0 |
| 跨线基建（叶层 / Redis / OpenTelemetry / AI） | `Mud.Wechat.Abstractions` 等 | — | — | 0 | 0 |

已成文 66 篇（`AccountId` 域 7 族 20 支端点；`AdvancedApi` / `AdvancedFeature` / `Agent` / `Aibot` / `Approval` 五域 11 族 38 支端点方法 / 36 个端点小节——`ApprovalTemplate.md` 的创建与更新模板在自建与代开发子接口各声明一次、同路由，合为同一小节；`Authentication` 域 3 族 9 支端点；`Basic` 域 1 族 2 支端点；`Checkin` 域 5 族 15 条路由 / 26 个方法声明；`Contacts` 域 6 族 52 个方法声明 / 44 条去重路由——差异只来自成员与部门把写入/邀请端点在自建与第三方两支各声明一次；`CorpGroup` 域 3 族 20 支端点；`DataZone` 域 2 族 18 个方法声明 / 17 条去重路由；`DeviceData` 域 1 族 8 支端点；`Dial` 域 1 族 1 支端点；`Emergency` 域 1 族 2 支端点；`ExternalContact` 域 16 族 95 支端点（16 支域守卫逐族 `HaveCount` 之和恰为 95）；`Gov` 域 4 篇覆盖 5 个根接口 21 支端点；`Hr` 域 1 族 3 支端点；`Identity` 域 3 篇覆盖 5 个根接口 8 支端点；`Invoice` 域 1 族 4 支端点）/ 待写按族计约 **165+ 篇**（族数量级，非端点数量级；确切缺口由 `reference/` 与 `Interfaces/` 目录比对得出，不靠人工记清单）。

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

`已核验` = 正文每个断言可指到内部证据或带日期官方取文 + 示例逐字编译通过（见规范 §7）；各篇 §7「已知留档缺口」登记的是官方未提供或本文不主张的项，不因缺口表存在而降级。

推进顺序建议：**先按线补 `guide/`（凭据模型各线不同，重复解释成本最高）→ 再按域补 L3（一次一个域，与该域守卫同批评审）→ 最后统一 `appendix/`**。

## 本项目的特殊约束（写文档前必须知道）

- **域 README 已是权威端点速览**：L3 文档不重抄速览表，只链接过去（规范 §3 的 L4 由源码目录 README 承担）。
- **契约守卫是路由与计量的事实来源**：`Tests/**/ContractGuards/`，改契约面必须同批改守卫与文档。
- **接口 XML 注释里的官方文档号只作定位标识**，不作为事实来源；需要官方正文时必须渲染后取文并记日期（本项目官方站正文由 JS 渲染，直接抓 HTML 只会拿到导航）。
- **锚点不一致要留档不要静默改写**：见样版文档 §7「已知留档缺口」的处理方式。
- 文档改动不进 CHANGELOG；但**契约面变更未同批改文档视为半成品**（对齐 `AGENTS.md` §2 同批纪律）。
