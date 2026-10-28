# AccountId 域 · 账号 ID（ID 安全性升级后的密文 ID 换算）

企业微信「账号ID」域：本仓共落地 **20 支端点方法 / 19 条唯一路由**（`/cgi-bin/batch/openuserid_to_userid` 被自建对接的两个场景共用），分属**七个接口族、共 20 个接口文件**（7 个公共父接口 + 13 个应用类型子接口）。守卫
[WechatAccountIdContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1~ACCT4）逐路由、逐计数、逐令牌锁定本域契约。

| 接口族 | 端点数 | 父接口 | 子接口 |
|---|---|---|---|
| ID 转换族 | 11（父 9 + 代开发差异 2） | [IdConvert/IWechatWorkAccountIdService.cs](./IdConvert/IWechatWorkAccountIdService.cs) | [_ThirdParty](./IdConvert/IWechatWorkAccountIdService_ThirdParty.cs)（空标记）/ [_Provider](./IdConvert/IWechatWorkAccountIdService_Provider.cs)（2 支） |
| tmp_external_userid 转换族 | 1 | [TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService.cs](./TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService.cs) | [_Internal](./TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService_Internal.cs) / [_ThirdParty](./TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService_ThirdParty.cs) / [_Provider](./TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService_Provider.cs)（三支皆空标记） |
| 自建应用对接族 | 3 | [Interop/IWechatWorkAccountIdInteropService.cs](./Interop/IWechatWorkAccountIdInteropService.cs)（零端点） | [Interop/IWechatWorkAccountIdInteropService_Internal.cs](./Interop/IWechatWorkAccountIdInteropService_Internal.cs)（3 支） |
| corpid 转换族 | 1 | [Corpid/IWechatWorkAccountIdCorpidService.cs](./Corpid/IWechatWorkAccountIdCorpidService.cs) | [_ThirdParty](./Corpid/IWechatWorkAccountIdCorpidService_ThirdParty.cs) / [_Provider](./Corpid/IWechatWorkAccountIdCorpidService_Provider.cs)（空标记） |
| ID 迁移完成状态族 | 2（父 1 + 第三方差异 1） | [Migration/IWechatWorkAccountIdMigrationService.cs](./Migration/IWechatWorkAccountIdMigrationService.cs) | [_ThirdParty](./Migration/IWechatWorkAccountIdMigrationService_ThirdParty.cs)（1 支）/ [_Provider](./Migration/IWechatWorkAccountIdMigrationService_Provider.cs)（空标记） |
| 智能机器人 userid 转换族 | 1 | [Bot/IWechatWorkAccountIdBotService.cs](./Bot/IWechatWorkAccountIdBotService.cs) | [_ThirdParty](./Bot/IWechatWorkAccountIdBotService_ThirdParty.cs) / [_Provider](./Bot/IWechatWorkAccountIdBotService_Provider.cs)（空标记） |
| 群 ID 升级（新授权企业）族 | 1 | [ChatIdUpgrade/IWechatWorkAccountIdChatIdUpgradeService.cs](./ChatIdUpgrade/IWechatWorkAccountIdChatIdUpgradeService.cs)（零端点） | [ChatIdUpgrade/IWechatWorkAccountIdChatIdUpgradeService_Provider.cs](./ChatIdUpgrade/IWechatWorkAccountIdChatIdUpgradeService_Provider.cs)（1 支） |

注册组 `AccountId`，模块枚举 `WechatModule.AccountId`（[WechatModule.cs](../../Extensions/WechatModule.cs) + [`AddAccountIdApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
本域的定位是：企业微信「账号ID 安全性升级」后，**第三方应用与服务商代开发应用获取的 corpid / userid / external_userid / 群 ID
全部是服务商主体下的密文 ID，自建应用保持明文 ID**，故新旧 ID 的正反换算、批量映射与「迁移完成」的收口动作全部集中在本域。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 20 份接口文件的 XML 注释、
> [WechatAccountIdContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1~ACCT4）
> 与 [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）、
> [Src/Work/Mud.Wechat.Work/AccountId/](../../../Mud.Wechat.Work/AccountId/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，接口注释里的官方文档号（96516 / 97106 等）仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

「令牌/开放面」列写本仓实测契约：令牌一律 Query 注入（ACCT3 + G5 白名单，MUD005 为已接受风险，官方契约无法改 Header），
**三种令牌路由键**按族划分（详见 §1）；「开放面」指该端点对哪几种应用类型开放，由 ACCT2 的接口层级断言锁死。

### ID 转换族·公共父接口（ACCT2 断言父接口声明面恰 9；第三方 + 代开发可用）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `UserIdToOpenUserIdAsync` | POST | `/cgi-bin/batch/userid_to_openuserid` | `access_token`(Query)；第三方 + 代开发 | 96516/97106；明文 userid → 密文 open_userid；单批 ≤1000 |
| 2 | `GetNewExternalUserIdAsync` | POST | `/cgi-bin/externalcontact/get_new_external_userid` | `access_token`(Query)；第三方 + 代开发 | 97063/97107；建议 200、上限 1000；客服场景仅支持 48h 内会话的 id |
| 3 | `GetGroupChatNewExternalUserIdAsync` | POST | `/cgi-bin/externalcontact/groupchat/get_new_external_userid` | `access_token`(Query)；第三方 + 代开发 | 97063/97107；客户群专用，须传 `chat_id`；≤1000 |
| 4 | `ConvertUnionidToExternalUserIdAsync` | POST | `/cgi-bin/idconvert/unionid_to_external_userid` | `access_token`(Query)；第三方 + 代开发 | 95900/97108；三档业务频率；可带 `mass_call_ticket`；非客户返回 `pending_id` |
| 5 | `ConvertExternalUserIdToPendingIdAsync` | POST | `/cgi-bin/idconvert/batch/external_userid_to_pending_id` | `access_token`(Query)；第三方 + 代开发 | 95900/97108；单批 ≤100；仅认证企业；数组键名无 `_list` 后缀 |
| 6 | `ConvertExternalTagIdAsync` | POST | `/cgi-bin/idconvert/external_tagid` | `access_token`(Query)；第三方 + 代开发 | 96169/97109；≤1000；需「企业客户权限→客户基础信息」 |
| 7 | `ConvertOpenKfIdAsync` | POST | `/cgi-bin/idconvert/open_kfid` | `access_token`(Query)；第三方 + 代开发 | 97064/97110；≤1000；需「微信客服→管理账号、分配会话和收发消息」 |
| 8 | `GetOpenIdMigrationAsync` | POST | `/cgi-bin/corp/get_openid_migration` | `access_token`(Query)；第三方 + 代开发 | 96518/97104；**无请求体的 POST**（查询类官方即 POST） |
| 9 | `ApplyMassCallTicketAsync` | GET | `/cgi-bin/corp/apply_mass_call_ticket` | `access_token`(Query)；第三方 + 代开发 | 96168/96250；每企业最多一次、有效期 7 天 |

### ID 转换族·代开发差异端点（ACCT2 断言代开发子接口声明面恰 2）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 10 | `ApplyToUpgradeChatIdAsync` | POST | `/cgi-bin/idconvert/apply_to_upgrade_chatid` | `access_token`(Query)；**仅代开发** | 99601；`upgrade_time` 不得早于当前时间或 7 天之后 |
| 11 | `ConvertChatIdAsync` | POST | `/cgi-bin/idconvert/chatid` | `access_token`(Query)；**仅代开发** | 99601；≤100；须先调 #10，到达升级时间后不可再调 |

### tmp_external_userid 转换族（ACCT2：父接口恰 1，三子接口空标记）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 12 | `ConvertTmpExternalUserIdAsync` | POST | `/cgi-bin/idconvert/convert_tmp_external_userid` | `access_token`(Query)；自建 + 第三方 + 代开发 | 98729/98412/98741；≤100；调用方须与取得临时 id 的应用**同一个** |

### 自建应用对接族（ACCT2：父接口零端点，自建子接口恰 3）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 13 | `OpenUserIdToUserIdAsync` | POST | `/cgi-bin/batch/openuserid_to_userid` | `access_token`(Query)；**仅自建** | 95884；须带 `source_agentid`；≤1000 |
| 14 | `OpenUserIdToUserIdForBotAsync` | POST | `/cgi-bin/batch/openuserid_to_userid` | `access_token`(Query)；**仅自建** | 101521；与 #13 **同路由多方法**，请求体无 `source_agentid` |
| 15 | `ConvertFromServiceExternalUserIdAsync` | POST | `/cgi-bin/externalcontact/from_service_external_userid` | `access_token`(Query)；**仅自建** | 95884；单值入单值出，**非批量** |

### corpid 转换族（ACCT2：父接口恰 1，两子接口空标记）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 16 | `CorpidToOpenCorpidAsync` | POST | `/cgi-bin/service/corpid_to_opencorpid` | `provider_access_token`(Query)；第三方 + 代开发 | 97061/97105；服务商凭证鉴权，无企业 scope |

### ID 迁移完成状态族（ACCT2：父接口恰 1 + 第三方子接口恰 1）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 17 | `FinishOpenIdMigrationAsync` | POST | `/cgi-bin/service/finish_openid_migration` | `provider_access_token`(Query)；第三方 + 代开发 | 99375/99378；**迁移完成不可回退** |
| 18 | `FinishExternalUserIdMigrationAsync` | POST | `/cgi-bin/service/externalcontact/finish_external_userid_migration` | `provider_access_token`(Query)；**仅第三方** | 99375；与 #17 同属一页（见 §8） |

### 智能机器人 userid 转换族 + 群 ID 升级族（各恰 1）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 19 | `ServiceUserIdToOpenUserIdAsync` | POST | `/cgi-bin/service/batch/userid_to_openuserid` | `provider_access_token`(Query)；第三方 + 代开发 | 96516/97106；须带 `source_botid`；≤1000（见 §2） |
| 20 | `UpgradeChatIdForNewCorpAsync` | GET | `/cgi-bin/idconvert/upgrade_chatid_for_new_corp` | `suite_access_token`(Query)；**仅代开发** | 99601 接口三；**无请求体的 GET**（见 §7） |

族内计数与 ACCT1 的 because 文案逐字一致：`ID 转换族 11 + tmp 转换族 1 + 自建对接族 3 + corpid 转换族 1 + 迁移完成状态族 2 + 智能机器人族 1 + 群 ID 升级族 1 = 20`；
ACCT1 另断言「方法（接口 + 方法名）不重复」与「唯一路由 19 条」。

## 1. 三种令牌路由键：一接口族一键，且只有 AccessToken 族带归属域键

ACCT3 把 20 个接口按令牌路由键分成三组逐一断言 `TokenType` + `InjectionMode.Query` + `Name`：

| 令牌路由键 | Query 参数名 | 接口数 | 覆盖接口族 |
|---|---|---|---|
| `WechatTokenTypes.AccessToken` | `access_token` | 9 | ID 转换族（父 + 第三方 + 代开发）、tmp 转换族（父 + 三子）、自建对接族（父 + 自建子） |
| `WechatTokenTypes.ProviderAccessToken` | `provider_access_token` | 9 | corpid 转换族（父 + 两子）、迁移完成状态族（父 + 两子）、智能机器人群（父 + 两子） |
| `WechatTokenTypes.SuiteAccessToken` | `suite_access_token` | 2 | 群 ID 升级族（父 + 代开发子） |

这不是「按应用类型拆令牌」（AGENTS.md §5.1 禁止的是同一业务凭证按 Internal/ThirdParty/Provider 分裂成多个 `TokenType`；这里三种键对应的是官方**三种不同的鉴权主体**，均为 `WechatTokenTypes` 既有常量），而是**按官方鉴权主体拆族**：同一条官方文档页里的接口若分别要求企业级凭证与服务商凭证，本仓一律分成两个族——
「一接口族一令牌路由键」是 corpid 转换族、智能机器人族、群 ID 升级族三个「看似多余的小族」存在的唯一理由，各接口 XML 注释均已写明。
**不得**把它们并进 ID 转换族省两个接口文件，那会破坏 ACCT3 的分组断言。

归属域键（`TokenManagerKey`）只加在 AccessToken 族上，由
[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1）双向锁定：

| 子接口 | `TokenManagerKey` | 语义 |
|---|---|---|
| `IWechatWorkInternalAccountIdTmpExternalUserIdService`、`IWechatWorkInternalAccountIdInteropService` | `Wechat.AccessToken@Internal` | 自建应用自身凭证 |
| `IWechatWorkThirdPartyAccountIdService`、`IWechatWorkProviderAccountIdService`、`IWechatWorkThirdParty/ProviderAccountIdTmpExternalUserIdService` | `Wechat.AccessToken@Corp` | 授权企业级令牌：一企一份，scope = `authCorpId` |
| 六个 ProviderAccessToken / SuiteAccessToken 子接口 | **不得声明** | TO1 明文：非 AccessToken 族本身已无歧义，叠加归属域键即违规；且自建族不得消费这两种键 |

⇒ 第三方/代开发调 AccessToken 族前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标企业作用域
（`permanent_code` 是凭据，只写参数名，不得进日志、遥测或异常消息）；调 provider/suite 族**不需要**企业作用域，它们是服务商级/套件级凭证。
本域 20 个接口名全部登记在 G5 白名单内（[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)），
G5 是「等价集合」断言——新增 Query 注入接口不落白名单即红。

## 2. 换算方向不对称：三对同名路由/同名字段互为反向，且反向端点不共族

本域最容易误用的地方是「名字像、方向反」。三组实测对照：

| 端点 | 输入 JSON 键 | 输入语义 | 输出语义 | 令牌 |
|---|---|---|---|---|
| #1 `batch/userid_to_openuserid` | `userid_list`（`List<string>`） | 企业主体**明文** userid | 服务商主体密文 open_userid | `access_token` |
| #13/#14 `batch/openuserid_to_userid` | `open_userid_list`（`List<string>`） | 服务商主体密文 open_userid | 企业主体**明文** userid | `access_token`，**仅自建** |
| #19 `service/batch/userid_to_openuserid` | `open_userid_list`（`List<string>`） | 企业主体**加密** userid | 服务商主体 open_userid | `provider_access_token` |

⇒ #19 的路由沿用 `userid_to_openuserid` 命名，但**入参键却是 `open_userid_list`**、承载的是「企业主体加密 userid」，与 #1 的 `userid_list`
不是同一个东西；#13 与 #19 方向看似同为「加密 → open_userid」与「open_userid → 明文」，实际分属不同令牌与不同应用类型面。
三者共用同一个映射元素类型 [OpenUserIdMapItem.cs](../../../Mud.Wechat.Work/AccountId/OpenUserIdMapItem.cs)（`userid` + `open_userid`），
该 DTO 注释明确「三个 userid/openuserid 转换端点共用」——它的 `userid` 字段在 #19 场景下装的是**密文**，不要按字段名推断明文。

external_userid 同样是双向、且不共族：#2/#3/#5 把企业主体 id 转成服务商主体 id 或 pending_id（第三方/代开发），
反向的那一支是 #15 `from_service_external_userid`（自建应用把服务商主体 id 转回企业主体 id）。

## 3. 定位键不对称：`source_agentid` / `source_botid` / `agentid` / `corpid` 各归一处

| 键 | 出现的 DTO | 属性类型 | JSON 名 | 作用 |
|---|---|---|---|---|
| `source_agentid` | [OpenUserIdToUserIdRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/OpenUserIdToUserIdRequest.cs)、[FromServiceExternalUserIdRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/FromServiceExternalUserIdRequest.cs) | `string?` | `source_agentid` | 自建应用侧指明「密文来自哪个第三方/代开发应用」 |
| `source_agentid` | [OpenUserIdToUserIdForBotRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/OpenUserIdToUserIdForBotRequest.cs) | **无此属性** | — | 智能机器人场景官方不要求，故单开一支方法与一个 DTO |
| `source_botid` | [ServiceUserIdToOpenUserIdRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/ServiceUserIdToOpenUserIdRequest.cs) | `string?` | `source_botid` | 智能机器人 id，官方必填 |
| `agentid` | [FinishOpenIdMigrationRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/FinishOpenIdMigrationRequest.cs) | **`int?`** | `agentid` | 代开发场景指明应用；仅传 `corpid` 不传 `agentid` 时视为更新该企业下**同服务商所有第三方应用** |
| `corpid` | 同上、[CorpidToOpenCorpidRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/CorpidToOpenCorpidRequest.cs)、[FinishExternalUserIdMigrationRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/FinishExternalUserIdMigrationRequest.cs) | `string?` | `corpid` | provider 族以明文 corpid 定位企业（不是 open_corpid） |

⇒ 同一个「应用 id」在本域两种建模：作为**来源标注**时官方拼写是 `source_agentid` 且本仓建模为 `string?`，作为**操作对象**时拼写是 `agentid` 且建模为 `int?`。
不得「顺手统一」为同一类型，那会改坏 ACCT4 之外的序列化面。
批量端点（#1~#3/#6/#7/#11/#12/#13/#14/#19）**都不传** `corpid`/`agentid`——企业身份由令牌作用域表达；只有 provider/suite 族与自建对接族才显式传定位键。

## 4. 批量上限一览（本仓不裁剪，越界由 `errcode` 表达）

| 上限 | 端点 | 请求键 |
|---|---|---|
| **1000** | #1 `userid_list`、#3 `external_userid_list`、#6 `external_tagid_list`、#7 `open_kfid_list`、#13/#14 `open_userid_list`、#19 `open_userid_list` | 均为 `List<string>?` |
| **200 建议 / 1000 上限** | #2 `external_userid_list` | 注释同时留档两个数 |
| **100** | #5 `external_userid`、#11 `chat_id_list`、#12 `tmp_external_userid_list` | 三支同批上限，键名各不相同 |
| **1（单值）** | #15 `external_userid` | 请求体是 `string?`，非列表 |

## 5. 转换配额：三档业务频率 + `mass_call_ticket` 豁免 + 「1 天不可调用」惩罚

父接口注释与 [#4 请求体](../../../Mud.Wechat.Work/AccountId/RequestModel/ConvertUnionidToExternalUserIdRequest.cs) 留档的口径：

- #4/#5 按主体维度限流 **10 万次/小时、48 万次/天、750 万次/月**；`subject_type = 0`（主体名称是企业的）时**所有服务商共用该企业额度**，
  `subject_type = 1`（主体名称是服务商的）时按服务商计。⇒ 配额归属由**请求体字段**决定，不由令牌决定。
- **每日上限之外还有一次性配额**：#9 `apply_mass_call_ticket` 返回的 `mass_call_ticket` **每个企业最多只能获取一次、有效期 7 天**，
  传入后**只豁免业务频率限制，仍受基础频率限制**（接口注释原文）。注释同时留档：目前**仅** unionid 转第三方 external_userid（本域 #4）与
  unionid 转上下游 external_userid（上下游域端点）两支支持该凭据。
- #1 有一条与配额无关的惩罚：注释留档「传入 userid 出错次数较多会导致 **1 天不可调用**，具体阈值由授权企业的员工规模决定」⇒ 阈值本仓无数值，见缺口表。
- #12 的约束不是频率而是**同应用性**：调用接口必须与取得 `tmp_external_userid` 的应用是同一个。

## 6. 枚举实际取值集合（本仓以 `int?` + 注释承载，未建枚举）

| 字段 | 所在 DTO | 本仓留档取值 |
|---|---|---|
| `subject_type` | ConvertUnionidToExternalUserIdRequest | 0 = 主体名称是企业的（默认）、1 = 主体名称是服务商的 |
| `business_type` | [ConvertTmpExternalUserIdRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/ConvertTmpExternalUserIdRequest.cs) | 1 = 会议、2 = 收集表、3 = 智能表（文档） |
| `user_type` | 同上 | 1 = 客户、2 = 企业互联、3 = 上下游、4 = 互联企业（圈子） |
| `openid_type`（请求侧，**数组**） | [FinishOpenIdMigrationRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/FinishOpenIdMigrationRequest.cs) | 1 = userid 与 corpid、3 = external_userid 及 external_tagid；类型 `List<int>?` |
| `openid_type`（响应侧，**单值**） | [OpenIdMigrationInfoItem.cs](../../../Mud.Wechat.Work/AccountId/OpenIdMigrationInfoItem.cs) | 1 = corpid/userid、3 = external_userid；类型 `int?` |
| `status` | 同上 | 0 = 未升级、1 = 已升级 |

⇒ `openid_type` 同名不同形状（请求是数组、响应是单值），且两处注释的取值文案顺序不同（`1 - userid 与 corpid` vs `1 - corpid/userid`），均为照抄官方原文，勿「对齐」成一份枚举。
`user_type` 为 1（客户）时应用还需具有「客户联系」权限（父接口 remarks）。

## 7. HTTP 动词分布：20 支里只有 2 支 GET，且都是「无请求体」

| GET 端点 | 说明 |
|---|---|
| #9 `apply_mass_call_ticket` | 无参数、无请求体，只带 Query 令牌 |
| #20 `upgrade_chatid_for_new_corp` | 无请求体的 GET，令牌为 `suite_access_token` |

其余 18 支全为 POST，其中 **纯查询语义也走 POST** 的典型是 #8 `get_openid_migration`（`get_` 开头、无请求体、只靠 Query 令牌鉴权）；
端点 #2/#3/#5/#6/#7/#11/#12/#19 这些「查询式转换」同样一律 POST ⇒ 生成客户端时不要按「查询用 GET」的直觉改写动词，
ACCT1 逐条断言 `typeof(PostAttribute)` / `typeof(GetAttribute)`。

## 8. 官方「一页多端点」与本域的两处拆分

- **99601「群 ID 升级」页有三条接口，本域拆给两个族、两种令牌**：#10 申请升级 + #11 群 ID 转换走企业级 `access_token`（挂在代开发子接口上），
  #20 对所有新授权企业升级走 `suite_access_token`（独立族）。#20 只对**调用之后新授权**的企业生效、不影响已授权企业（父接口 remarks）。
- **99375「ID 迁移完成状态的设置」页有两条接口**：#17（userid/corpid，第三方 + 代开发共用，落父接口）与
  #18（external_userid，仅第三方，落第三方子接口）。注释留档的官方口径：**userid 与 corpid 只能同时设置为迁移完成，external_userid 可以单独设置**；
  设置迁移完成后接口不再返回该企业相关的 unionid；**迁移完成不可回退**。⇒ #17/#18 是单向闸门，调用前必须已完成全量 id 回收。
- **95884 与 101521 共用 `/cgi-bin/batch/openuserid_to_userid`**：ACCT1 用「20 方法 / 19 唯一路由」两条断言把它钉住——
  同一路由上的两个方法靠**请求 DTO 不同**（带不带 `source_agentid`）区分，不做运行时多态。
- **跨域单一所有者**：#4/#5 两支归本域唯一声明，客户联系域守卫
  [WechatExternalContactCustomerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactCustomerContractGuards.cs)（CU2）
  已改为断言「第三方子接口仅 1 条差异端点，unionid 转换 / pending_id 查询归账号ID 域单一所有者」。
  注意上下游域另有 `/cgi-bin/corpgroup/unionid_to_external_userid`（**不同路由、不同族**），不得因方法名相近而合并。

## 9. 字段名照官方原文：前缀不统一、大小写与连写各异

| 现象 | 实测 |
|---|---|
| 「新/密文」前缀三套写法 | #2/#3 输出 `new_external_userid`；#6 输出 `open_external_tagid`；#7 输出 `new_open_kfid`；#11 输出 `new_chat_id`；#16 输出 `open_corpid` ⇒ 同一语义在官方即两套前缀，本仓逐字照抄（[NewExternalUserIdItem.cs](../../../Mud.Wechat.Work/AccountId/NewExternalUserIdItem.cs)、[ExternalTagIdConvertItem.cs](../../../Mud.Wechat.Work/AccountId/ExternalTagIdConvertItem.cs)、[OpenKfIdConvertItem.cs](../../../Mud.Wechat.Work/AccountId/OpenKfIdConvertItem.cs)、[ChatIdConvertItem.cs](../../../Mud.Wechat.Work/AccountId/ChatIdConvertItem.cs)） |
| 列表键名后缀不统一 | `userid_list` / `external_userid_list` / `open_userid_list` / `external_tagid_list` / `open_kfid_list` / `chat_id_list` / `tmp_external_userid_list` 都带 `_list`，**只有 #5 的 `external_userid` 不带**（[ConvertExternalUserIdToPendingIdRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/ConvertExternalUserIdToPendingIdRequest.cs) 注释与守卫 remarks 双重标注） |
| 响应袋名不统一 | 批量转换响应：#2/#3/#6/#7/#11/#19 用 `items`，#5 用 `result`（单数），#12 用 `results`（复数），#1 用 `open_userid_list` |
| 失效袋名不统一 | `invalid_userid_list`（#1）/ `invalid_open_userid_list`（#13/#14/#19）/ `invalid_external_tagid_list`（#6）/ `invalid_open_kfid_list`（#7）/ `invalid_chat_id_list`（#11）/ `invalid_tmp_external_userid_list`（#12） |
| `userid_list` 同名异形状 | #1 请求的 `userid_list` 是 `List<string>`，#13/#14 响应的 `userid_list` 是 `List<OpenUserIdMapItem>`（[OpenUserIdToUserIdResponse.cs](../../../Mud.Wechat.Work/AccountId/OpenUserIdToUserIdResponse.cs)）⇒ 建模时必须按端点看 DTO，不能按 JSON 名复用 |
| 密文大小写敏感 | corpid / userid / external_userid 密文**大小写敏感**，多处注释留档「使用时不能再转为纯小写或纯大写」⇒ 宿主侧不得做 `ToLower()` 归一化 |

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// 注入的一律是应用类型子接口（公共父接口 IsAbstract = true，不可注入）
public sealed class IdBridge(
    IWechatWorkThirdPartyAccountIdService thirdPartyIds,
    IWechatWorkInternalAccountIdInteropService internalInterop,
    IWechatWorkThirdPartyAccountIdCorpidService corpidConvert,
    IWechatAppContextSwitcher switcher)
{
    // 1) 第三方应用：明文 userid → 服务商主体 open_userid（企业作用域内，单批 ≤ 1000）
    public async Task<List<OpenUserIdMapItem>> ToOpenUserIdsAsync(
        string appKey, string authCorpId, string permanentCode,
        IReadOnlyCollection<string> userIds, CancellationToken ct)
    {
        using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))
        {
            var resp = await thirdPartyIds.UserIdToOpenUserIdAsync(
                new UserIdToOpenUserIdRequest { UserIdList = new List<string>(userIds) }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/batch/userid_to_openuserid");

            List<OpenUserIdMapItem> mapped = resp.OpenUserIdList ?? new List<OpenUserIdMapItem>();
            List<string> invalid = resp.InvalidUserIdList ?? new List<string>();
            // invalid 是「不在可见范围/非法」的 userid，属敏感标识：不得进日志明文
            return mapped;
        }
    }

    // 2) unionid → external_userid（或 pending_id）；配额由 subject_type 决定归属
    public async Task<string> ResolveCustomerAsync(CancellationToken ct)
    {
        var resp = await thirdPartyIds.ConvertUnionidToExternalUserIdAsync(
            new ConvertUnionidToExternalUserIdRequest
            {
                Unionid = "o-UNIONID-FAKE-0001",      // 假值示例
                OpenId = "o-OPENID-FAKE-0002",
                SubjectType = 1,                       // 1 = 主体名称是服务商的（按服务商计额度）
                // MassCallTicket = "<7 天有效的一次性凭据>",  // 凭据：不得进日志/遥测/异常消息
            }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/idconvert/unionid_to_external_userid");

        // 用户尚未成为企业客户时返回 pending_id（90 天有效，不能当 external_userid 调接口）
        return resp.ExternalUserId ?? resp.PendingId!;
    }

    // 3) 自建应用反向换算：服务商密文 → 企业明文（必须带 source_agentid）
    public async Task<List<OpenUserIdMapItem>> BackToPlainUserIdsAsync(
        string sourceAgentId, IReadOnlyCollection<string> openUserIds, CancellationToken ct)
    {
        var resp = await internalInterop.OpenUserIdToUserIdAsync(
            new OpenUserIdToUserIdRequest
            {
                OpenUserIdList = new List<string>(openUserIds),
                SourceAgentId = sourceAgentId,   // string：照官方拼写 source_agentid
            }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/batch/openuserid_to_userid");
        return resp.UserIdList ?? new List<OpenUserIdMapItem>();
    }

    // 4) corpid 转换族：服务商凭证，无需 UseCorpScope（provider_access_token 无企业 scope）
    public async Task<string> ToOpenCorpIdAsync(string corpId, CancellationToken ct)
    {
        var resp = await corpidConvert.CorpidToOpenCorpidAsync(
            new CorpidToOpenCorpidRequest { CorpId = corpId }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/service/corpid_to_opencorpid");
        return resp.OpenCorpId!;   // 密文，大小写敏感：不得 ToLower/ToUpper
    }
}

// 代开发：群 ID 升级两段（先申请时间，再批量转换），以及套件级的「对新授权企业全量升级」
public sealed class ChatIdUpgrader(
    IWechatWorkProviderAccountIdService providerIds,
    IWechatWorkProviderAccountIdChatIdUpgradeService suiteUpgrade,
    IWechatAppContextSwitcher switcher)
{
    public async Task ScheduleAsync(int epochSeconds, IReadOnlyCollection<string> chatIds, CancellationToken ct)
    {
        var applied = await providerIds.ApplyToUpgradeChatIdAsync(
            new ApplyToUpgradeChatIdRequest { UpgradeTime = epochSeconds }, ct);
        WechatWorkException.ThrowIfFailed(applied, "/cgi-bin/idconvert/apply_to_upgrade_chatid");

        using (switcher.UseCorpScope("provider-app", "wwAUTHCORPIDFAKE", "PERMANENT-CODE-FAKE"))
        {
            var converted = await providerIds.ConvertChatIdAsync(
                new ConvertChatIdRequest { ChatIdList = new List<string>(chatIds) }, ct);   // ≤ 100
            WechatWorkException.ThrowIfFailed(converted, "/cgi-bin/idconvert/chatid");
            List<ChatIdConvertItem> items = converted.Items ?? new List<ChatIdConvertItem>();
            List<string> invalid = converted.InvalidChatIdList ?? new List<string>();
        }

        // 套件凭证那一支：只影响调用之后新授权的企业，且为无请求体的 GET
        var allNew = await suiteUpgrade.UpgradeChatIdForNewCorpAsync(ct);
        WechatWorkException.ThrowIfFailed(allNew, "/cgi-bin/idconvert/upgrade_chatid_for_new_corp");
    }
}
```

判错为单层普通信封（[WechatWorkResponse.cs](../../../Mud.Wechat.Work/WechatWorkResponse.cs) 的 `errcode == 0` 即成功）；
令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。#17/#18 这类「不可回退」的写操作请由宿主自行加幂等前置检查。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 文档号内部不一致 | [Migration 父接口](./Migration/IWechatWorkAccountIdMigrationService.cs) remarks 写「第三方子接口另持差异端点（**96516**）」，而[第三方子接口](./Migration/IWechatWorkAccountIdMigrationService_ThirdParty.cs) 的 XML 与守卫 ACCT1/ACCT2 注释均写 **99375** | 本文按「子接口 XML + 守卫」取 99375；父接口 remarks 的 96516 属仓内注释不一致，**未逐项核验**孰为官方页号，须核验后同批改注释与守卫 remarks |
| #1「1 天不可调用」的具体阈值 | 注释仅留档「阈值由授权企业的员工规模决定」，无数值 ⇒ **未逐项核验** | 需要本地熔断时先核验官方页，不得从邻域借数值 |
| 除 #4/#5 外各端点的频率数值 | 注释只留档 #4/#5 的三档配额与 #9 的「每企业一次 / 7 天」；#1~#3、#6~#8、#10~#20 的 QPS/日配额**未逐项核验** | 逐页核验后补接口 remarks，再更新本文速览备注 |
| `upgrade_time` 的单位与量纲 | [ApplyToUpgradeChatIdRequest.cs](../../../Mud.Wechat.Work/AccountId/RequestModel/ApplyToUpgradeChatIdRequest.cs) 建模为 `int?`，注释写「完成升级的时间戳」「不得早于当前时间或 7 天之后」，**秒/毫秒未留档**；`int` 承载 Unix 秒的 2038 边界本仓无断言 ⇒ **未逐项核验** | 使用前核验官方示例单位；如需扩量纲须同批改 DTO 与 ACCT4 类型清单 |
| `openid_type` 是否存在取值 2 | 请求侧注释只列 1 与 3，响应侧亦只列 1 与 3；2 是否被官方保留**未逐项核验** | 出现新取值先核验，再决定是否建枚举 |
| `subject_type` / `business_type` / `user_type` 的其它取值 | 本文 §6 只列注释留档值 ⇒ 全集**未逐项核验** | 不得据本文推断「只有这些值」 |
| `mass_call_ticket` 的第二支持端点归属 | 注释说支持「unionid 转上下游 external_userid」，该端点由上下游域（CorpGroup）承载 ⇒ 跨域支持面**未在本域核验** | 需要签发/消费链时查 CorpGroup 域，勿在本域加端点 |
| #19 入参「企业主体加密 userid」的签发来源 | 本域只消费该 id，其来源（智能机器人回调帧）归智能机器人/Aibot 邻域 ⇒ **未在本域核验** | 需要来源链时查邻域守卫 |
| 各端点错误码集合 | 本域接口注释未留档错误码表；令牌失效码由通用判定器覆盖 ⇒ 域内专属 errcode **未逐项核验** | 需要按码分流时先核验，不在 SDK 侧硬编码码表 |
| provider/suite 族的多应用令牌装配 | ACCT3 只锁 `TokenType`/注入模式/参数名；`provider_access_token`、`suite_access_token` 在具体应用上下文中的解析与刷新链路由 Authentication 域承载 ⇒ 本域**未逐项核验** | 装配问题时查 Authentication 域与其守卫，勿在本域补令牌逻辑 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（≤1000 / ≤200 / ≤100、三档频率、90 天、7 天、权限要求、可见范围） | 本目录 20 份接口文件的 XML 注释 + [AccountId/](../../../Mud.Wechat.Work/AccountId/) 与 [AccountId/RequestModel/](../../../Mud.Wechat.Work/AccountId/RequestModel/) 的 DTO 类级 remarks |
| 路由表 / 计数（恰 20 方法、19 唯一路由）/ HTTP 动词 | [WechatAccountIdContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs) ACCT1（`Routes` 表 L55–L106、断言 L111–L135） |
| 七接口族层级、父接口端点数、子接口空标记与差异计数、继承链不漂移 | ACCT2（L141–L232），辅助断言 `AssertAbstractParentWithEndpoints`（L344–L354）/ `AssertRegistryChild`（L356–L368）/ `AssertDerivedInterfaces`（L370–L378） |
| 三种令牌路由键 → Query 参数名（access / provider_access / suite_access） | ACCT3（L239–L290）；域内 20 接口名的白名单登记见 [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) G5（L212–L234） |
| JSON 上下文登记（AccountIdJsonContext 40 型） | ACCT4（L295–L342）与 [Generated/AccountIdJsonContext.g.cs](../../../Mud.Wechat.Work/Generated/AccountIdJsonContext.g.cs)（`internal`，不得在示例引用） |
| 归属域键（`TokenManagerKey`）与「非 AccessToken 族不得叠加」 | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) TO1~TO3 |
| 父接口/可注入接口命名空间二分与本域 7+13 归属 | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) N1~N3（聚合计数维护在 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)：`Baseline.Work` 实测 154 父 + 307 可注入；该守卫文件注释写 155/308、`AGENTS.md` §4 写 147/299，**均为陈旧文案，以常量为准**） |
| #4/#5 的跨域单一所有者 | [WechatExternalContactCustomerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactCustomerContractGuards.cs) CU2（L40–L42 注释、L150–L156 断言） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（`AccountIdJsonContext.Default` 合并行） |
| 线级总览（本域在其中） | [../../README.md](../../README.md)（其统计可能陈旧，端点数一律以本文速览与守卫为准） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与业务限制（频率/批量上限/权限/可见范围/不可回退等，**格式照既有端点、不得弱化**），
   **同批**改 [WechatAccountIdContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs) 的 `Routes` 表
   与 ACCT1 的 `HaveCount(20)` / `HaveCount(19)` 两条计数及 because 文案里的族分解算式；
   改声明面（父/子谁承载哪支）须同时改 ACCT2 的 `AssertAbstractParentWithEndpoints` / `AssertRegistryChild` 计数与 `AssertDerivedInterfaces` 期望子接口集合。
2. 改令牌面 → ACCT3 的三组接口名单必须同步，且 20 个接口名要同步出现在
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) 的 G5 白名单（等值断言，多一个少一个都红）。
   新增 Query 凭据参数名须二选一：补组件脱敏词表，或登记 G7 豁免并附追踪号——`access_token` / `provider_access_token` / `suite_access_token` 均已在词表内。
   AccessToken 族子接口改族别时须核对 `TokenManagerKey`（TO1：自建 ↔ `Wechat.AccessToken@Internal`、第三方/代开发 ↔ `@Corp`，
   provider/suite 令牌**不得**叠加归属域键）。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "AccountId")]` → 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) 与
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)（`Generated/AccountIdJsonContext.g.cs` 是生成物，勿手改）→
   同批更新 ACCT4 的类型清单与 `HaveCount(40)`；合并面 [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) 已含该上下文，无需再登记。
4. 新增接口族 / 重命名子接口 → 三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 的 `AccountId` 枚举 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddAccountIdApi()` XML（其「七接口族 / 三种令牌路由键」叙述须与实测一致）、
   可注入子接口的 `RegistryGroupName = "AccountId"` 与 `InheritedFrom = nameof(父实现类)`（父接口保持 `IsAbstract = true` + 空注册组）。
   新增/迁移接口须跑 N1~N3，并同批改 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 的两个聚合计数（只改这一处）。
5. 凭据面改动一律不得引入真值：`access_token` / `provider_access_token` / `suite_access_token` / `mass_call_ticket` / `permanent_code` 只写参数名，
   不得进日志、遥测或异常消息；`corpid` / `userid` / `openid` / `unionid` / `external_userid` / `pending_id` / `tmp_external_userid` 按敏感标识处理，
   文档与示例一律用假值；密文大小写敏感的既有口径不得在 SDK 侧做归一化转换。
6. 本文与 [../../README.md](../../README.md) 的域内端点数、族划分表述须同步更新；
   最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1) 与 [../../../../../scripts/audit-config-keys.ps1](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
