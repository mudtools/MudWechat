# KF 域 · 微信客服

企业微信「微信客服」域：本仓共落地 **38 支端点方法 / 29 条去重路由**，分属八个接口族——
六族为三类应用（自建 / 第三方 / 代开发）公共面，端点收敛在 `IsAbstract` 父接口、子接口为空标记：
客服账号管理 **5 支**（[IWechatWorkKfAccountService.cs](./IWechatWorkKfAccountService.cs)，守卫 KF1a）、
接待人员管理 **3 支**（[IWechatWorkKfServicerService.cs](./IWechatWorkKfServicerService.cs)，KF1b）、
会话分配与消息收发 **15 支方法 / 5 条路由**（[IWechatWorkKfSessionService.cs](./IWechatWorkKfSessionService.cs)，KF1c）、
客户基础信息 **1 支**（[IWechatWorkKfCustomerService.cs](./IWechatWorkKfCustomerService.cs)，KF1d）、
统计管理 **2 支**（[IWechatWorkKfStatisticsService.cs](./IWechatWorkKfStatisticsService.cs)，KF1e）、
「升级服务」配置 **3 支**（[IWechatWorkKfUpgradeService.cs](./IWechatWorkKfUpgradeService.cs)，KF1h）；
另两族为单应用类型面，零端点父接口 + 唯一子接口承载端点：
机器人管理 **8 支（仅自建）**（[IWechatWorkKfKnowledgeService.cs](./IWechatWorkKfKnowledgeService.cs) 父接口零端点 +
[IWechatWorkKfKnowledgeService_Internal.cs](./IWechatWorkKfKnowledgeService_Internal.cs) 承载，KF1f）、
微信客服组件 **1 支独有（仅第三方/组件套件）**（[IWechatWorkKfComponentService.cs](./IWechatWorkKfComponentService.cs) 父接口零端点 +
[IWechatWorkKfComponentService_ThirdParty.cs](./IWechatWorkKfComponentService_ThirdParty.cs) 承载，KF1g / KF1g2）。

本目录共 **28 个 `.cs`**：8 个父接口 + 20 个应用类型子接口（六公共面族各 3 个 = 18，机器人族 1、组件族 1）。
应用类型子接口：`IWechatWork{Internal,ThirdParty,Provider}Kf{Account,Servicer,Session,Customer,Statistics,Upgrade}Service` 各一 +
`IWechatWorkInternalKfKnowledgeService` + `IWechatWorkThirdPartyKfComponentService`。

- 注册组 `Kf`；模块枚举 [`WechatModule.Kf`](../../Extensions/WechatModule.cs)；[`AddKfApi()`](../../Extensions/WechatWorkServiceBuilder.cs)（源生成器产出 `AddKfWebApiHttpClient()`）。
- 令牌路由键恒为 `Wechat.AccessToken`，Query 注入参数名 `access_token`（守卫 KF3，MUD005 已知接受风险，无法改 Header）。
- 契约面 DTO 全量 **107 型**登记进 `KfJsonContext`（守卫 KF4；该上下文为 `internal`，示例不得引用）。
- 逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 28 份接口文件的 XML 注释、
> [WechatKfContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（KF1a~KF1k / KF2 / KF2b / KF3 / KF4）、
> [Src/Work/Mud.Wechat.Work.DataModels/Kf/](../../../Mud.Wechat.Work.DataModels/Kf/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

「令牌/开放面」列写本仓真实契约：令牌一律 `access_token`(Query)；「开放面」指官方对该端点开放的应用类型，
由 KF2（六公共面族：自建 + 第三方 + 代开发子接口全为空标记、端点全在父接口）与 KF2b（机器人族仅自建、组件族仅第三方）锁死。

### 客服账号管理族（KF1a，5 支，三类公共）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddAccountAsync` | POST | `/cgi-bin/kf/account/add` | `access_token`(Query)；自建+第三方+代开发 | 94662 / 代开发 96404；name+media_id 必填，≤5000 账号 |
| 2 | `GetAccountListAsync` | POST | `/cgi-bin/kf/account/list` | 同上 | 94661 / 96415；**官方即 POST**，offset+limit(1~100)；组件版 99368 共用此路由 |
| 3 | `DeleteAccountAsync` | POST | `/cgi-bin/kf/account/del` | 同上 | 94663 / 96405 |
| 4 | `UpdateAccountAsync` | POST | `/cgi-bin/kf/account/update` | 同上 | 94664 / 96406；name/media_id 均可选填 |
| 5 | `GetAccountContactWayAsync` | POST | `/cgi-bin/kf/add_contact_way` | 同上 | 94665 / 96416；**挂在 kf/ 根下、不在 account/ 子路径**；组件版 99400 共用此路由 |

### 接待人员管理族（KF1b，3 支，三类公共）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 6 | `AddServicerAsync` | POST | `/cgi-bin/kf/servicer/add` | 同上 | 94646 / 96418；userid_list 与 department_id_list 至少填一个 |
| 7 | `DeleteServicerAsync` | POST | `/cgi-bin/kf/servicer/del` | 同上 | 94647 / 96419；成员/部门各 ≤100，超出分批 |
| 8 | `GetServicerListAsync` | GET | `/cgi-bin/kf/servicer/list` | 同上 | 94645 / 96420；`open_kfid` 走 Query（官方必填、非可空，KF1b 锁定） |

### 会话分配与消息收发族（KF1c，15 支方法 / 5 条路由，三类公共）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 9 | `GetServiceStateAsync` | POST | `/cgi-bin/kf/service_state/get` | 同上 | 94669 / 第三方 94698 / 96425 |
| 10 | `TransitServiceStateAsync` | POST | `/cgi-bin/kf/service_state/trans` | 同上 | 同 #9；应答 `msg_code` 见 §6 |
| 11 | `SyncMessageAsync` | POST | `/cgi-bin/kf/sync_msg` | 同上 | 94670 / 94699 / 96426；游标续拉见 §5 |
| 12 | `SendTextMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 94677 / 94700 / 96427；msgtype=text |
| 13 | `SendImageMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=image |
| 14 | `SendVoiceMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=voice |
| 15 | `SendVideoMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=video |
| 16 | `SendFileMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=file |
| 17 | `SendLinkMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=link |
| 18 | `SendMiniProgramMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=miniprogram |
| 19 | `SendMenuMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=**msgmenu** |
| 20 | `SendLocationMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=location |
| 21 | `SendCaLinkMessageAsync` | POST | `/cgi-bin/kf/send_msg` | 同上 | 同上；msgtype=ca_link |
| 22 | `SendTextMsgOnEventAsync` | POST | `/cgi-bin/kf/send_msg_on_event` | 同上 | 95122 / 94910 / 96428；msgtype=text |
| 23 | `SendMenuMsgOnEventAsync` | POST | `/cgi-bin/kf/send_msg_on_event` | 同上 | 同上；msgtype=msgmenu |

> #12~#21 同路由多方法、每 msgtype 一请求体（形态对齐消息推送 `message/send` 先例）；#22~#23 同理，2 种 msgtype 共用 `send_msg_on_event`。

### 客户基础信息族（KF1d，1 支，三类公共）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 24 | `BatchGetCustomerInfoAsync` | POST | `/cgi-bin/kf/customer/batchget` | 同上 | 95159 / 95149 / 96429；路由 `batchget` 为官方原文连写，勿改 `batch_get` |

### 统计管理族（KF1e，2 支，三类公共）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 25 | `GetCorpStatisticAsync` | POST | `/cgi-bin/kf/get_corp_statistic` | 同上 | 95489 / 95492 / 96432；企业汇总，**官方即 POST** |
| 26 | `GetServicerStatisticAsync` | POST | `/cgi-bin/kf/get_servicer_statistic` | 同上 | 95490 / 95493 / 96433；接待人员明细 |

### 机器人管理族（KF1f，8 支，**仅自建**，全部声明于 `IWechatWorkInternalKfKnowledgeService`）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 27 | `AddKnowledgeGroupAsync` | POST | `/cgi-bin/kf/knowledge/add_group` | `access_token`(Query)；**仅自建** | 95971；分组名不可重复、总数 ≤100 |
| 28 | `DeleteKnowledgeGroupAsync` | POST | `/cgi-bin/kf/knowledge/del_group` | 同上 | 95971；默认分组 is_default=1 不可删 |
| 29 | `UpdateKnowledgeGroupAsync` | POST | `/cgi-bin/kf/knowledge/mod_group` | 同上 | 95971；路由为 **mod_group** 非 update |
| 30 | `GetKnowledgeGroupListAsync` | POST | `/cgi-bin/kf/knowledge/list_group` | 同上 | 95971；cursor+limit(默认500/最大1000)+has_more |
| 31 | `AddKnowledgeIntentAsync` | POST | `/cgi-bin/kf/knowledge/add_intent` | 同上 | 95972；单分组问答 ≤200、答案暂仅 1 个 |
| 32 | `DeleteKnowledgeIntentAsync` | POST | `/cgi-bin/kf/knowledge/del_intent` | 同上 | 95972 |
| 33 | `UpdateKnowledgeIntentAsync` | POST | `/cgi-bin/kf/knowledge/mod_intent` | 同上 | 95972；路由为 **mod_intent**；**整体覆盖写** |
| 34 | `GetKnowledgeIntentListAsync` | POST | `/cgi-bin/kf/knowledge/list_intent` | 同上 | 95972；附件 media_id/封面 thumb_media_id 仅写入有效、列表不返回 |

### 微信客服组件族（KF1g，1 支独有，**仅第三方/组件套件**，声明于 `IWechatWorkThirdPartyKfComponentService`）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 35 | `GetStatisticAsync` | POST | `/cgi-bin/kf/get_statistic` | `access_token`(Query) 企业级；**仅第三方** | 99367；组件版企业级口径、**无 open_kfid 入参** |

### 「升级服务」配置族（KF1h，3 支，三类公共）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 36 | `GetUpgradeServiceConfigAsync` | GET | `/cgi-bin/kf/customer/get_upgrade_service_config` | `access_token`(Query)；自建+第三方+代开发 | 94674 / 94702 / 96422；**GET 且无请求参数** |
| 37 | `UpgradeServiceAsync` | POST | `/cgi-bin/kf/customer/upgrade_service` | 同上 | 同上；type=1 填 member / type=2 填 groupchat |
| 38 | `CancelUpgradeServiceAsync` | POST | `/cgi-bin/kf/customer/cancel_upgrade_service` | 同上 | 同上 |

**合计 38 支端点方法、29 条去重路由**，与 KF1a~KF1h 各表的 `HaveCount` 断言逐一相符；28 个接口类型由 KF3 全枚举锁定。

## 1. 目录 / 命名空间不一致是既存设计，勿「顺手修正」

本域接口目录为 `Interfaces/KF/`（**大写 F**），而 DTO 目录与命名空间为 `Mud.Wechat.Work.DataModels.Kf`（**小写 f**）——
这条「接口大写、DTO 小写」的不对称是 [AGENTS.md §4 落位表](../../../../../AGENTS.md) 明列的既存点，新增沿用、勿修正。
注册入口同理为**单数** `Kf`（`WechatModule.Kf` / `AddKfApi()` / 注册组 `"Kf"`），不是 `KF`。
其余落位遵循 Work 线不变量：父接口 `IsAbstract = true` → 命名空间 `Mud.Wechat.Work.Interfaces`；
可注入子接口 → `Mud.Wechat.Work`（由 [N1~N3 守卫](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) 锁定）；
DTO 命名空间恒为 `Mud.Wechat.Work.DataModels.Kf`（子目录 `RequestModel/` 仅作组织、不入命名空间）。

## 2. 八族开放面不等宽：六族三类公共 / 机器人仅自建 / 组件仅第三方

KF2 锁定六个公共面族「父接口承载端点、三类子接口均为零差异空标记」：
每个子接口 `RegistryGroupName = "Kf"`、`InheritedFrom = 父实现类名`，且 `DeclaredOnly` 方法集恒空（能力漂移即红）。
KF2b 锁定两个单应用族「零端点父接口 + 唯一子接口承载端点」，且继承链上**不得**出现其它应用类型子接口：
机器人管理域官方仅自建开放（`IWechatWorkInternalKfKnowledgeService` 承载 8 端点），
微信客服组件域官方仅组件套件开放（`IWechatWorkThirdPartyKfComponentService` 承载端点）。
⇒ 注入 `IWechatWorkThirdPartyKfKnowledgeService` / `IWechatWorkProviderKfKnowledgeService` **不存在**，这不是漏挂而是官方无对应文档页；
不得靠「给机器人族补第三方/代开发子接口」对齐。

## 3. 令牌链路：Query `access_token` + 归属键二分

KF3 对全部 28 个接口断言 `TokenType = Wechat.AccessToken` + `InjectionMode.Query` + `Name = "access_token"`（MUD005：官方强制 Query、无法改 Header）。
子接口的凭据归属键（[TO1~TO3 守卫](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)）按应用类型二分：

| 子接口族别 | `TokenManagerKey` | 语义 |
|---|---|---|
| `IWechatWorkInternal*`（含 `InternalKfKnowledgeService`） | `WechatTokenManagerKeys.InternalAccessToken` | 自建应用自身 access_token |
| `IWechatWorkThirdParty*` / `IWechatWorkProvider*`（含 `ThirdPartyKfComponentService`） | `WechatTokenManagerKeys.CorpAccessToken` | 授权企业级 access_token（scope = authCorpId，一企一份） |

第三方 / 代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标授权企业作用域
（`permanent_code` 只写参数名，不得进日志/遥测/异常消息）。
`sync_msg` 请求体的 `token` 字段是**事件回调下发的同步令牌**，与 Query 注入的 `access_token` 是两个概念，须以请求体字段承载而非 `[Token]` 声明（KF1c 锁定）。

## 4. 客服账号族：两条「反直觉」路由 + API 管理白名单 + manage_privilege

KF1a 注释专门标注两条形状异常：
- `GetAccountListAsync` 的 `/cgi-bin/kf/account/list` **官方即 POST**（勿按 REST 直觉改 GET）；分页形态为 **offset + limit**（limit 默认/最大 100），返回条数 < limit 即到底。
- `GetAccountContactWayAsync`（获取客服账号链接）的路由 `/cgi-bin/kf/add_contact_way` **直挂 `/cgi-bin/kf/` 根下、不在 `account/` 子路径**，与其余四个 `account/*` 端点形状不同。

业务口径（接口 XML 注释）：一家企业最多 5000 个客服账号；`AddAccountAsync` 的 `name`（≤16 字符）与 `media_id`（≤128 字节，上传临时素材获取）均官方必填，创建后调用方应用自动获得该账号管理权限；
只能通过 API 管理企业「在管理后台指定允许 API 管理」的账号；自 2023-12-01 起不再支持系统应用 secret 调用（存量企业不受影响）。
**组件版共用路由的单一所有者**：官方另以组件文档 99368/99400 分文档承载「获取客服账号列表 / 链接」，但二者与客服账号管理域**同路由、同 DTO**，故只在账号域声明一次；
KF1g2 反向锁定组件子接口**不得**重复声明这两条路由。组件应用调用 `GetAccountListAsync` 时，响应 [KfAccount.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfAccount.cs) 的 `manage_privilege`（`bool?`）**不返回**（仅能拿到企业已授权的账号）。

## 5. `sync_msg`：游标续拉 + cursor/next_cursor 时效 + 不得以空列表判停

`SyncMessageAsync`（94670 / 94699 / 96426）是本域最易踩坑的读取端点，契约由 KF1c / KF1j / KF1k 三重锁定：

- **续拉协议**：请求体 [SyncKfMessageRequest.cs](../../../Mud.Wechat.Work.DataModels/Kf/RequestModel/SyncKfMessageRequest.cs) 首次不传 `cursor`（不填则从最近 3 天内最早的消息开始），其后逐页以上一次响应的 `next_cursor` 续拉；翻页**唯一终点判据是 `has_more` 为 0**——官方明令「不能通过判断 `msg_list` 是否空来停止拉取」（可能出现 `has_more=1` 而 `msg_list` 为空）。
- **`has_more` 是数字非布尔**：[SyncKfMessageResponse.cs](../../../Mud.Wechat.Work.DataModels/Kf/SyncKfMessageResponse.cs) 的 `HasMore` 原样以 `int?` 承载（官方 uint32 0/1），改成 `bool?` 即在反序列化期失败（KF1j）。`next_cursor` 官方强烈建议入库，意外丢失会导致从头拉取而延迟消息。
- **必填/选填形态双向锁定**（KF1k，追踪号 P2-QY-B8-KF-REQUIREDFACE-10）：`open_kfid` 官方**必填**（非可空 `string OpenKfId`），`token`/`cursor`/`limit`/`voice_format` 官方**均选填**（可空）。历史缺陷曾把二者写成相反口径，故本守卫双向钉死。`limit` 默认值与最大值都为 1000，实际返回条数可能更少。
- **`token` 时效与限频**：该同步令牌官方「10 分钟内有效；可不填，不填接口有严格的频率限制」⇒ 正确用法是收到回调事件后尽快带上 `token` 拉取，而非留空裸轮询。
- **读取面限制**：仅能取**最近 3 天内**的消息内容，且**不支持**读取通过 `send_msg` 发出的消息；媒体大小上限 图片/语音/视频/文件 = 2MB/2MB/10MB/20MB，超出官方改返回文本提示消息。
- **字段名照官方原文**（KF1j）：信封 `msg_list` / `has_more` / `next_cursor`；[KfSyncMessage.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfSyncMessage.cs) 载体字段 `msgmenu` / `business_card` / `channels_shop_product` / `channels_shop_order` / `merged_msg` / `open_kfid` / `send_time` 均逐字保留（勿写成 `menu` / `card` / 驼峰）。消息体按 `msgtype` 判别、按 `origin`（3-客户发送 / 4-系统事件 / 5-接待人员发送）判别来源，一份扁平报文承载全部消息类型位、不做运行时多态。

## 6. 会话状态机 `service_state` 与 `msg_code` / `open_kfid` 的定位关系

- **状态取值**（[TransitKfServiceStateRequest.cs](../../../Mud.Wechat.Work.DataModels/Kf/RequestModel/TransitKfServiceStateRequest.cs) 与 [GetKfServiceStateResponse.cs](../../../Mud.Wechat.Work.DataModels/Kf/GetKfServiceStateResponse.cs)）：0-未处理、1-由智能助手接待、2-待接入池排队中、3-由人工接待、4-已结束/未开始；**状态 4 不允许通过 API 变更**；目标接待人员未在企业微信激活返回 95014。`service_state=3` 时 `servicer_userid` 必填且须处于「正在接待」。
- **定位关系**：`open_kfid` 定位「哪个客服账号」，`external_userid` 定位「哪个客户」，两者共同锁定一条会话；`GetServiceStateAsync` / `TransitServiceStateAsync` 均以 `(open_kfid, external_userid)` 查询/变更该会话的状态。第三方 / 代开发侧 `servicer_userid` / `external_userid` 一律为密文（`open_userid` / 客服侧 external_userid）。
- **`msg_code`**：`TransitServiceStateAsync` 应答 [TransitKfServiceStateResponse.cs](../../../Mud.Wechat.Work.DataModels/Kf/TransitKfServiceStateResponse.cs) 的 `msg_code`，是会话**初次**变更为状态 2/3（回复语 code）或 4（结束语 code）时下发的**事件响应令牌**，只能用于紧随其后的一次 `send_msg_on_event` 调用（见 §7）。同步消息的事件体 [KfSyncEventMsgBody.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfSyncEventMsgBody.cs) 也带 `msg_code` / `welcome_code` / `change_type` / `old_servicer_userid` / `new_servicer_userid` / `fail_msgid` / `recall_msgid`，是 `kf_msg_or_event` 回调触发的拉取内容。

## 7. 发送消息：`msgid` 幂等 / 可发送状态 / 成功≠送达 / 素材先上传后引用

- **`msgid` 幂等**（[SendKfMsgRequest.cs](../../../Mud.Wechat.Work.DataModels/Kf/RequestModel/SendKfMsgRequest.cs)）：≤32 字节、正则 `[0-9a-zA-Z_-]*`，须在客服账号内唯一，否则官方报错；请求指定则响应 [SendKfMsgResponse.cs](../../../Mud.Wechat.Work.DataModels/Kf/SendKfMsgResponse.cs) 原样返回，未指定则系统生成。重发同 `msgid` 是宿主幂等去重的手段，SDK 侧不做本地裁剪。
- **可发送条件**：仅当客户处于「新接入待处理」或「由智能助手接待」状态时可调用 `send_msg`；客户主动发消息后 **48 小时内企业最多下发 5 条**（客户再次发消息可重新计时）。
- **成功≠送达**：接口返回 `errcode=0` 不代表消息最终发送成功，须关注「消息发送失败事件」回调（`origin=4` 事件体 `fail_msgid` / `fail_type`）。
- **素材先上传后引用**：image / voice / video / file 的 `media_id`（[KfMediaMsgBody.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfMsgBodies.cs) 四类共用载体）与 link / miniprogram 的 `thumb_media_id` 均由**上传临时素材接口（media 域）先上传后引用**，本域不含上传端点。
- **`send_msg_on_event` 的 `code`**（[SendKfEventMsgRequest.cs](../../../Mud.Wechat.Work.DataModels/Kf/RequestModel/SendKfEventMsgRequest.cs)）：`code` 由事件回调下发、**仅可使用一次**，欢迎语 / 结束会话场景**有效期仅 20 秒**（超时结束语义），须及时调用；除「用户进入会话」外，响应消息仅支持会话处于获取该 code 时的状态。菜单消息项 ≤10（`send_msg` 的 msgmenu ≤50，其中 click/view/miniprogram ≤10）。

## 8. 场景值 `scene` / `scene_param` 与客服链接不可改

`GetAccountContactWayAsync`（[GetKfAccountContactWayRequest.cs](../../../Mud.Wechat.Work.DataModels/Kf/RequestModel/GetKfAccountContactWayRequest.cs)）：`scene` ≤32 字节、正则 `[0-9a-zA-Z_-]*`；
`scene` 非空时返回链接可拼接 `scene_param=SCENE_PARAM`，用户进入会话事件原样回传该值（事件体 `scene` / `scene_param`），`SCENE_PARAM` 需 urlencode、编码前 ≤128 字节。
应答 [GetKfAccountContactWayResponse.cs](../../../Mud.Wechat.Work.DataModels/Kf/GetKfAccountContactWayResponse.cs) 的 `url` **不能修改或复制参数到其他链接**，否则进入会话事件参数校验不通过、无法回调。
`ca_link` 获客链接名片（[KfMsgBodies.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfMsgBodies.cs) 的 `KfCaLinkMsgBody.LinkUrl`）经此添加**不计入**获客助手「打开链接的客户数」统计。

## 9. 接待人员批量语义与可见范围

`AddServicerAsync` / `DeleteServicerAsync`（[AddKfServicerResponse.cs](../../../Mud.Wechat.Work.DataModels/Kf/AddKfServicerResponse.cs)）：`userid_list` 与 `department_id_list` **至少填一个**；
每个客服账号 ≤2000 个接待人员、≤20 个接待部门；删除单次成员/部门各 ≤100、超出分批；批量为**逐条返回结果**（`result_list` 与请求逐条对应），单条失败不整体报错。
第三方 / 代开发填密文 `open_userid`；接待人员须在应用可见范围内。
`GetServicerListAsync` 应答 [KfServicer.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfServicer.cs)：`status`（0-接待中 / 1-停止接待）、`stop_type`（0-停止接待 / 1-暂时挂起）、按部门接待时 `department_id`（`int?`）。

## 10. 客户基础信息 `batchget`：与 external contact 的边界 + 48 小时窗口

`BatchGetCustomerInfoAsync`（[BatchGetKfCustomerInfoRequest.cs](../../../Mud.Wechat.Work.DataModels/Kf/RequestModel/BatchGetKfCustomerInfoRequest.cs)）返回的是**微信客服会话内的客户**基础信息，与客户联系域（external contact）的 `external_userid` 是**同一 ID 空间但不同可见语义**——本端点只暴露客服会话维度需要的昵称/头像/unionid 与进入会话上下文。
约束：`external_userid_list` 必填、可填 1~100 个（超出分批）、`need_enter_session_context`（0/1，默认 0）；
查询对象须为**最近 48 小时内触发过「用户进入会话事件」或向客服发过消息**的客户，否则落入响应 `invalid_external_userid`；
第三方 / 代开发不可获取头像、性别统一返回 0、unionid 不可获取；「营销获客」应用仅能获取自身带来的客户。

## 11. 统计族：闭区间跨度 / 取整 / 次日可见 / 空值容错

`GetCorpStatisticAsync` / `GetServicerStatisticAsync`（以及组件版 `GetStatisticAsync`）：查询区间为**闭区间且最大跨度 31 天、最多查最近 180 天**；
传入非 0 点时间戳官方向下取整到当天 0 点；当天数据**次日才可获取**（建议次日 6 点后调用）；
开启 API 或授权第三方管理会话的，没有 2022-03-11 以前的数据；**无数据的指标官方直接缺省不返回**，解析须空值容错。
`GetServicerStatisticAsync` 不指定 `servicer_userid` 时返回的是客服账号维度汇总（非全体接待人员明细）。

## 12. 机器人管理族：路由命名与覆盖写

KF1f 注释锁定两条拼写：分组修改路由 `/cgi-bin/kf/knowledge/mod_group`、问答修改 `/cgi-bin/kf/knowledge/mod_intent`（**均为 mod_ 前缀，勿写成 update**）。
`UpdateKnowledgeIntentAsync` 为**整体覆盖写**——即便只改部分字段也须传完整内容。
分页 `limit` 默认 500、最大 1000，游标形态 `cursor + limit + has_more`（`has_more` 与 sync_msg 侧一致以 `int` 承载）。
默认分组（`is_default = 1`）不可删除/修改；分组总数 ≤100、单分组问答 ≤200、答案目前仅支持 1 个、不同分组问题不可重复。

## 13. 「升级服务」族：GET 无参 + 字段名冲突以官方 JSON 示例为准

`GetUpgradeServiceConfigAsync` 为 **GET 且无请求参数**（KF1h，勿改 POST）。
字段名冲突处置：官方参数表作 `department_list`、官方 JSON 示例与自建文档作 `department_id_list`，SDK **以官方 JSON 示例为准**——
`KfUpgradeMemberRange.DepartmentIdList` 的 `[JsonPropertyName]` 恒为 `department_id_list`（KF1h 断言，对齐全仓 cusor→cursor 先例）。
业务约束：升级接口指定的 `userid` / `chat_id` 必须已配置在微信客服「升级服务」的专员/客户群范围内，否则返回 95021；
`userid` 须在「客户联系」使用范围内，三个端点均要求接待人员与 chatid 对应群主在应用可见范围内。

## 14. 频率上限与「查询类官方即 POST」汇总

- **官方即 POST 的查询端点**（勿按 REST 直觉改 GET）：`account/list`（#2）、`service_state/get`（#9）、`sync_msg`（#11）、`customer/batchget`（#24）、`get_corp_statistic`（#25）、`get_servicer_statistic`（#26）、机器人族全部 8 支、组件版 `get_statistic`（#35）。本域**仅两支 GET**：`servicer/list`（#8，`open_kfid` 走 Query）与 `customer/get_upgrade_service_config`（#36，无参）。
- **频率数值**：本域接口注释留档的**唯一**显式频率口径来自 `sync_msg`——「不填 `token` 时有严格的频率限制」，但官方正文**未给出该端点的数值型阈值**；客户基础信息域（#24）注释明示 95159/95149/96429 三页均无独立频率数字，按附录「访问频率限制」（90312）默认口径执行（每企业单 cgi/api 1 万次/分、15 万次/小时；第三方提供商每 IP 4 万次/分、120 万次/小时）。
- **本仓不做本地限流与批次裁剪**，越界由应答 `errcode` 表达；其余端点的数值频率**未逐项核验**（见缺口），不得凭第三方 SDK 注释或记忆补数。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块（模块三段式：WechatModule.Kf + AddKfApi()）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddKfApi());

// 自建应用注入自建子接口（六个公共面父接口 IsAbstract，不可直接注入）
public sealed class KfInbox(
    IWechatWorkInternalKfAccountService accounts,
    IWechatWorkInternalKfServicerService servicers,
    IWechatWorkInternalKfSessionService sessions)
{
    // 1) 创建客服账号：name + media_id 均必填（media_id 由 media 域上传临时素材先取得）
    public async Task<string> CreateAccountAsync(string avatarMediaId, CancellationToken ct)
    {
        var resp = await accounts.AddAccountAsync(new AddKfAccountRequest
        {
            Name = "售前咨询",
            MediaId = avatarMediaId,        // 凭据无关，可留占位；不得记入日志的只有 token/secret
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/kf/account/add");
        return resp.OpenKfId!;              // 之后所有会话/消息端点都以该 open_kfid 定位客服账号
    }

    // 2) 添加接待人员：userid_list 与 department_id_list 至少填一个；逐条结果不整体报错
    public async Task AddStaffAsync(string openKfId, CancellationToken ct)
    {
        var resp = await servicers.AddServicerAsync(new AddKfServicerRequest
        {
            OpenKfId = openKfId,
            UserIdList = new List<string> { "zhangsan" },   // 第三方/代开发须填密文 open_userid
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/kf/servicer/add");
        List<KfServicerOperateResult> results = resp.ResultList ?? new List<KfServicerOperateResult>();
        // 逐条检查 results[i]，单条失败不影响其它条目
    }

    // 3) 同步消息与事件：首次不带 cursor，续拉只认 has_more != 0；next_cursor 每次覆盖并入库
    public async Task PullInboxAsync(string openKfId, string? syncToken, string? savedCursor, CancellationToken ct)
    {
        string? cursor = savedCursor;
        while (true)
        {
            var resp = await sessions.SyncMessageAsync(new SyncKfMessageRequest
            {
                OpenKfId = openKfId,        // 官方必填
                Token = syncToken,          // 事件回调下发的同步令牌（10 分钟有效；不填则严格限频）
                Cursor = cursor,            // 首次可为 null
                Limit = 1000,               // 默认与最大都是 1000，实际可能更少
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/kf/sync_msg");

            // 官方明令不得以 msg_list 是否为空停止拉取——必须用显式类型 + 空列表兜底
            List<KfSyncMessage> messages = resp.MessageList ?? new List<KfSyncMessage>();
            foreach (KfSyncMessage msg in messages)
            {
                // msg.MessageType 判别载体；msg.Origin：3-客户发送 / 4-系统事件 / 5-接待人员发送
                if (msg.MessageType == "event" && msg.Event?.EventType == "change_type")
                {
                    // 会话变更事件：msg.Event.MsgCode 可用于一次 send_msg_on_event
                }
            }

            // 翻页终点唯一判据：has_more == 0（官方以数字承载布尔，本仓原样 int?）
            if (resp.HasMore != 1) break;
            cursor = resp.NextCursor;       // 强烈建议入库，异常后从此续拉
        }
    }

    // 4) 转人工接待 → 拿到 msg_code → 20 秒内下发事件响应欢迎语
    public async Task TakeOverAsync(string openKfId, string externalUserId, string servicerUserId, CancellationToken ct)
    {
        var trans = await sessions.TransitServiceStateAsync(new TransitKfServiceStateRequest
        {
            OpenKfId = openKfId,
            ExternalUserId = externalUserId,    // 假值示例；第三方/代开发侧为客服域 open_userid
            ServiceState = 3,                    // 0 未处理 / 1 智能助手 / 2 待接入池 / 3 人工接待
            ServicerUserId = servicerUserId,     // 状态 3 必填
        }, ct);
        WechatWorkException.ThrowIfFailed(trans, "/cgi-bin/kf/service_state/trans");

        if (!string.IsNullOrEmpty(trans.MsgCode))
        {
            var sent = await sessions.SendTextMsgOnEventAsync(new SendKfEventTextMsgRequest
            {
                Code = trans.MsgCode,            // 一次性 code，20 秒内有效
                MsgType = "text",
                Text = new KfTextMsgBody { Content = "您好，已为您转接人工客服。" },
            }, ct);
            WechatWorkException.ThrowIfFailed(sent, "/cgi-bin/kf/send_msg_on_event");
            // 注意：errcode==0 不代表送达，须看「消息发送失败事件」回调
        }
    }
}

// 第三方 / 代开发：先切到授权企业作用域，再以企业级 access_token 调用同一批端点
using (switcher.UseCorpScope(appKey: "suite-app", authCorpId: "ww1234567890abcdef", permanentCode: permanentCode))
{
    var link = await thirdPartyAccounts.GetAccountContactWayAsync(new GetKfAccountContactWayRequest
    {
        OpenKfId = openKfId,
        Scene = "promo_2026",              // ≤32 字节；据此拼接的 url 不可改写或复制参数
    }, ct);
    WechatWorkException.ThrowIfFailed(link, "/cgi-bin/kf/add_contact_way");
    // link.Url 即客服咨询链接，可嵌入 H5 或据此生成二维码
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。
`permanent_code`、`access_token`、回调 `token`、`media_id`（临时素材虽非长期密钥，仍按凭据对待）一律不得进日志、遥测或异常消息。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域各端点数值型 QPS/频率上限 | 除 `sync_msg`「不填 token 有严格限频」（无数值）、客户信息域「按附录 90312 默认口径」外，其余 36 支端点的接口注释**未留档独立频率数字** ⇒ **未逐项核验** | 不得凭第三方 SDK 注释/记忆补数；核验官方逐页后补 remarks 再更新本文 §14 |
| `KfSyncMessage` 全量 msgtype 与载体字段 | 载体字段名由 KF1j 锁定 15 项，但 `meeting`/`calendar`/`note` 三类官方标注「暂不返回详细消息内容」的后续是否补全 **未逐项核验** | 出现新载体须同批改 [KfSyncMessage.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfSyncMessage.cs) 与 KF1j |
| `KfSyncEventMsgBody` 的 `event_type` 取值全集 | [KfSyncEventMsgBody.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfSyncEventMsgBody.cs) 注释「取值以官方参数表为准」，本仓未建枚举 ⇒ **未逐项核验** | 需按事件分派时先核验官方 94670 事件章节 |
| 各统计指标字段（`KfCorpStatisticData` / `KfServicerStatisticData` / `KfComponentStatisticData`） | KF4 仅断言类型已登记进 `KfJsonContext`，逐指标字段名/口径**未逐项核验** | 解析前读对应 Response DTO 注释，勿臆测指标含义 |
| 机器人问答附件的媒体类型全集 | [KfKnowledgeAttachments.cs](../../../Mud.Wechat.Work.DataModels/Kf/KfKnowledgeAttachments.cs) 建模 image/video/link/miniprogram 四类，官方是否还有其它附件形态 **未逐项核验** | 补附件前先核验官方 95972 附件章节 |
| 组件版 `get_statistic`（99367）与统计族 `get_corp_statistic` 的指标差异 | 二者路由/入参不同（组件无 `open_kfid`），返回指标集是否完全同构 **未逐项核验** | 需精确对齐口径时查两页原文 |
| 代开发文档号成对性 | 各端点注释均给出「自建/第三方」与「代开发 9xxxx」两套号；个别端点代开发页与自建页正文是否逐字一致 **未逐项核验** | 代开发报参数错误时先比对两棵树原文 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（含 5000 账号 / 2000 人 / 20 部门 / ≤100 / 48 小时 5 条 / 20 秒 code / 3 天 / 2MB-2MB-10MB-20MB / 31 天 / 180 天 / 1000 limit） | 本目录 28 份接口文件的 XML 注释 |
| 客服账号族路由 / 计数（恰 5）与 `add_contact_way` 根路径告警 | `WechatKfContractGuards.cs` KF1a（L56 `AccountRoutes` 表、L278 方法） |
| 接待人员族路由 / 计数（恰 3）、`servicer/list` GET + `open_kfid` 必填 Query | KF1b（L83 `ServicerRoutes`、L303 方法） |
| 会话/消息族 15 方法 / 5 路由、`sync_msg` 以 `SyncKfMessageRequest` 承载、`token` 非 `[Token]` | KF1c（L104 `SessionRoutes`、L341 方法） |
| 客户信息族计数（恰 1）、`batchget` 连写 | KF1d（L161 `CustomerRoutes`、L373 方法） |
| 统计族计数（恰 2） | KF1e（L172 `StatisticsRoutes`、L394 方法） |
| 机器人族计数（恰 8）、`mod_group`/`mod_intent` 拼写 | KF1f（L188 `KnowledgeRoutes`、L418 方法） |
| 组件族独有计数（恰 1）与「不重复声明账号路由」反向断言 | KF1g（L224 `ComponentRoutes`、L443）、KF1g2（L465） |
| 升级服务族计数（恰 3）、`department_id_list` 字段名 | KF1h（L236 `UpgradeRoutes`、L486 方法、L506 断言） |
| 撤回域零声明（`kf/get_corp_qualification`）与活体对照 | KF1i（L267 `WithdrawnCorpQualificationRoutes`、L518） |
| 同步消息官方字段名逐字保留 + `has_more` 为 `int?` | KF1j（L551） |
| 同步消息请求必填/选填形态双向锁定 | KF1k（L597，追踪号 P2-QY-B8-KF-REQUIREDFACE-10） |
| 六公共面族层级 / 空标记 / 注册组 `Kf` / `InheritedFrom` | KF2（L633） |
| 机器人族仅自建、组件族仅第三方的单链层级 | KF2b（L710） |
| 28 接口令牌绑定（AccessToken + Query + `access_token`） | KF3（L754） |
| Kf 契约面 107 型登记进 `KfJsonContext` | KF4（L804） |
| 非契约路由零声明（撤回路由的全局兜底，含正对照） | [WechatNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs) RD1（L73 类、L111 路由、L162 KF 域对照、L182 方法） |
| 令牌归属键二分（Internal vs Corp） | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3） |
| Query 令牌注入白名单 | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 接口命名空间分区 | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work.DataModels/Kf/](../../../Mud.Wechat.Work.DataModels/Kf/)（根为响应与消息体模型、`RequestModel/` 为请求体） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)（L63-65 `AddKfWebApiHttpClient` 注册、L188 `AddKfApi()`） |
| 域起始索引（无 KF 端点总数断言，勿据此推断计数） | [../../README.md](../../README.md)（L29 KF 段落，仅描述族与权限口径） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号（自建/第三方/代开发三套号）与业务限制，**同批**改
   [WechatKfContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs) 的
   `AccountRoutes`（KF1a，5）/ `ServicerRoutes`（KF1b，3）/ `SessionRoutes`（KF1c，15/5）/ `CustomerRoutes`（KF1d，1）/
   `StatisticsRoutes`（KF1e，2）/ `KnowledgeRoutes`（KF1f，8）/ `ComponentRoutes`（KF1g，1）/ `UpgradeRoutes`（KF1h，3）八张表与各自 `HaveCount` + `Distinct` 双断言。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`，`SerializerClassName` 一律取 `Kf`（本域为**单一**上下文）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` → 更新 KF4 的类型清单（现 107 型）与
   `../../../Mud.Wechat.Work.DataModels/Generated/KfJsonContext.g.cs`（生成物、勿手改）。
3. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddKfApi()` XML、可注入子接口 `RegistryGroupName = "Kf"`（父接口保持空组），
   并同批改 KF2/KF2b 的族集合与 KF3 的接口枚举；子接口须过 [TO1~TO3](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) 归属键断言。
4. 改开放面（如官方给机器人族补第三方页）→ 先核验，再调整 KF2b 的「继承链上不得出现其它应用类型子接口」反向断言，
   并同批扩 [G5 Query 白名单](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) 接口名清单。
5. 客服**回调事件**（`kf_msg_or_event` 等 `kf_*` 事件族、事件下发 `token`/`welcome_code`/`msg_code`）**属 `Mud.Wechat.Work.Callback` 线**（载荷体系与 `[WechatCallbackContract]` 见 [AGENTS.md §5.5](../../../../../AGENTS.md)），
   不在本域、不得在本目录声明；本域全部是发送/读取侧 HTTP API。
6. 凭据面改动不得引入真值：`access_token`、`permanent_code`、`sync_msg` 的 `token`、`welcome_code`/`msg_code`、`media_id` 只写参数名；示例中 `external_userid`/`open_userid`/手机号一律假值。
   新增敏感 Query 参数名须同批登记脱敏词表（G7 / `SensitiveUrlKeys.Register`）。
7. 本文与 [../../README.md](../../README.md) 的域内端点数、官方文档号表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
