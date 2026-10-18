# Mail 域 · 邮件（企业邮箱 / 应用邮箱 / 邮件群组 / 公共邮箱）

企业微信「邮件」域：本仓共落地 **26 支端点**，分属**八个接口族**（守卫逐族计数相加 = 26，见下表各分组标题）。
应用邮箱侧三支族对**三类应用（自建 / 第三方 / 代开发）等宽开放**，端点全部收敛在 `IsAbstract` 公共父接口、
子接口为零端点空标记；管理端侧五支族官方**仅自建应用开放**，形态为「零端点父接口 + 仅自建子接口承载全部端点」，
且**不声明**第三方/代开发子接口（能力漂移守卫）。

| 族 | 端点数 | 守卫 | 父接口 | 承载端点的子接口 |
|---|---|---|---|---|
| 发送邮件 | 3 | M1 | [IWechatWorkMailSendService.cs](./Send/IWechatWorkMailSendService.cs) | [\_Internal](./Send/IWechatWorkMailSendService_Internal.cs) / [\_ThirdParty](./Send/IWechatWorkMailSendService_ThirdParty.cs) / [\_Provider](./Send/IWechatWorkMailSendService_Provider.cs)（均空标记） |
| 获取接收的邮件 | 2 | M1b | [IWechatWorkMailReceiveService.cs](./Receive/IWechatWorkMailReceiveService.cs) | [\_Internal](./Receive/IWechatWorkMailReceiveService_Internal.cs) / [\_ThirdParty](./Receive/IWechatWorkMailReceiveService_ThirdParty.cs) / [\_Provider](./Receive/IWechatWorkMailReceiveService_Provider.cs)（均空标记） |
| 管理应用邮箱账号 | 2 | M1c / M2 | [IWechatWorkMailAccountService.cs](./Account/IWechatWorkMailAccountService.cs)（零端点） | [IWechatWorkInternalMailAccountService](./Account/IWechatWorkMailAccountService_Internal.cs) 2 支 |
| 管理邮件群组 | 5 | M5 / M2b | [IWechatWorkMailGroupService.cs](./Group/IWechatWorkMailGroupService.cs)（零端点） | [IWechatWorkInternalMailGroupService](./Group/IWechatWorkMailGroupService_Internal.cs) 5 支 |
| 管理公共邮箱 | 7 | M6 / M2b | [IWechatWorkMailPublicMailService.cs](./PublicMail/IWechatWorkMailPublicMailService.cs)（零端点） | [IWechatWorkInternalMailPublicMailService](./PublicMail/IWechatWorkMailPublicMailService_Internal.cs) 7 支 |
| 高级功能账号 | 3 | M7 / M2b | [IWechatWorkMailVipService.cs](./Vip/IWechatWorkMailVipService.cs)（零端点） | [IWechatWorkInternalMailVipService](./Vip/IWechatWorkMailVipService_Internal.cs) 3 支 |
| 成员邮箱操作 | 2 | M8 / M2b | [IWechatWorkMailUserService.cs](./User/IWechatWorkMailUserService.cs)（零端点） | [IWechatWorkInternalMailUserService](./User/IWechatWorkMailUserService_Internal.cs) 2 支 |
| 其他邮件客户端登录设置 | 2 | M9 / M2b | [IWechatWorkMailUserOptionService.cs](./UserOption/IWechatWorkMailUserOptionService.cs)（零端点） | [IWechatWorkInternalMailUserOptionService](./UserOption/IWechatWorkMailUserOptionService_Internal.cs) 2 支 |

接口文件共 20 份 = 8 个公共父接口 + 12 个可注入子接口（3 族 × 3 应用类型 + 5 族 × 仅自建），
与守卫 [M3](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs) 枚举的接口集合一致。
注册组 `Mail`，模块枚举 `WechatModule.Mail`（[WechatModule.cs](../../Extensions/WechatModule.cs) L71–L72），
注册入口 [`AddMailApi()`](../../Extensions/WechatWorkServiceBuilder.cs)（L217–L218）。逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 20 份接口文件的 XML 注释、
> [WechatMailContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M1、M1b、M1c、M2、M2b、M3、M4、M5~M9）、
> [Src/Work/Mud.Wechat.Work.DataModels/Mail/](../../../Mud.Wechat.Work.DataModels/Mail/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号（97445 / 95510 / 100183 等）仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断、不借邻域数值。

## 端点速览

令牌列写本仓真实契约：`access_token` 恒为 Query 注入、`TokenType` 恒 `Wechat.AccessToken`（守卫 M3，L437–L474；
MUD005 已知接受风险——企业微信官方契约强制 Query 传令牌，无法改 Header）；「开放面」指官方对该族开放的应用类型，
由 M2 / M2b 的「父接口声明面 + 子接口类型化面」计数锁死。全部 26 支的路由逐条以 M1~M9 的八张路由表为准
（表源码位置见文末「事实来源」）。

### 发送邮件族（M1，3 支 · 三类应用公共面）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `SendNormalMailAsync` | POST | `/cgi-bin/exmail/app/compose_send` | `access_token`(Query)；自建 97445 + 第三方 97515 + 代开发 97504 | 请求体 [MailSendRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/MailSendRequest.cs)；`to.emails` 与 `to.userids` 至少传一个 |
| 2 | `SendScheduleMailAsync` | POST | `/cgi-bin/exmail/app/compose_send` | 同上；自建 97854 + 第三方 97867 + 代开发 97865 | [#1 公共字段](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/MailSendScheduleRequest.cs) + `schedule`（官方必填）；应用还需「日程」权限 |
| 3 | `SendMeetingMailAsync` | POST | `/cgi-bin/exmail/app/compose_send` | 同上；自建 97855 + 第三方 97868 + 代开发 97866 | [请求体](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/MailSendMeetingRequest.cs) `schedule` + `meeting` 同时必填；还需「日程」+「会议」权限 |

三支共用同一路由（M1 断言 `Distinct` = 1，L201–L202），差异全在请求包体 ⇒ 三种请求 DTO 各自独立、**不做运行时多态**。

### 获取接收的邮件族（M1b，2 支 · 三类应用公共面）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 4 | `GetMailListAsync` | POST | `/cgi-bin/exmail/app/get_mail_list` | `access_token`(Query)；自建 97369 + 第三方 97516 + 代开发 97505 | 官方即 POST；`limit` 默认 100、最大 1000；cursor + has_more 翻页 |
| 5 | `ReadMailAsync` | POST | `/cgi-bin/exmail/app/read_mail` | 同上；自建 97979 + 第三方 97983 + 代开发 97982 | 返回邮件 eml 原文（`mail_data`）；`mail_id` 由 #4 取得 |

### 管理应用邮箱账号族（M1c，2 支 · 仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 6 | `UpdateEmailAliasAsync` | POST | `/cgi-bin/exmail/app/update_email_alias` | `access_token`(Query) + `InternalAccessToken`；**仅自建** 97373 | 换绑后**原应用邮箱转为别名邮箱保留且仍可收信** |
| 7 | `GetEmailAliasAsync` | POST | `/cgi-bin/exmail/app/get_email_alias` | 同上；**仅自建** 97991 | 官方「不需要请求包体」⇒ 方法参数**只有** `CancellationToken`（M1c 断言 `ContainSingle`） |

### 管理邮件群组族（M5，5 支 · 仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 8 | `CreateMailGroupAsync` | POST | `/cgi-bin/exmail/group/create` | `access_token`(Query) + `InternalAccessToken`；**仅自建** 95510 | 成员四列表至少填一项；`groupname` ≤200 字节且不得重名 |
| 9 | `UpdateMailGroupAsync` | POST | `/cgi-bin/exmail/group/update` | 同上；**仅自建** 97995 | 列表字段三态语义；成员不允许全部清空 |
| 10 | `DeleteMailGroupAsync` | POST | `/cgi-bin/exmail/group/delete` | 同上；**仅自建** 97996 | 仅 `groupid` |
| 11 | `GetMailGroupAsync` | **GET** | `/cgi-bin/exmail/group/get` | 同上；**仅自建** 97997 | `groupid` 走 `[Query("groupid")]`（M5 断言） |
| 12 | `SearchMailGroupAsync` | **GET** | `/cgi-bin/exmail/group/search` | 同上；**仅自建** 97998 | `fuzzy`（必填）+ `groupid`（选填）走 Query；`fuzzy=0` 取全部 |

### 管理公共邮箱族（M6，7 支 · 仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 13 | `CreatePublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/create` | `access_token`(Query) + `InternalAccessToken`；**仅自建** 95511 | 可选 `create_auth_code=1` 同时建客户端专用密码（密码仅返回一次） |
| 14 | `UpdatePublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/update` | 同上；**仅自建** 98000 | 别名为**覆盖式**更新、≤5 个；API 创建的专用密码 ≤10 个 |
| 15 | `DeletePublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/delete` | 同上；**仅自建** 98001 | 仅 `id`（整数） |
| 16 | `GetPublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/get` | 同上；**仅自建** 98002 | 官方即 POST；批量形态，`id_list` 为**平铺整数数组** |
| 17 | `SearchPublicMailAsync` | **GET** | `/cgi-bin/exmail/publicmail/search` | 同上；**仅自建** 98003 | `fuzzy`（必填）+ `email`（选填）走 Query（M6 断言） |
| 18 | `GetPublicMailAuthCodeListAsync` | POST | `/cgi-bin/exmail/publicmail/get_auth_code_list` | 同上；**仅自建** 100183 | **不返回密码本身**，只有 id / 时间 / 备注 |
| 19 | `DeletePublicMailAuthCodeAsync` | POST | `/cgi-bin/exmail/publicmail/delete_auth_code` | 同上；**仅自建** 100184 | `id` + `auth_code_id` 双键 |

### 高级功能账号族（M7，3 支 · 仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 20 | `BatchAddVipAsync` | POST | `/cgi-bin/exmail/vip/batch_add` | `access_token`(Query) + `InternalAccessToken`；**仅自建** 99316 | 同步批量（非异步任务）；单次 `userid_list` ≤100 |
| 21 | `BatchDelVipAsync` | POST | `/cgi-bin/exmail/vip/batch_del` | 同上；**仅自建** 99317 | 单次 ≤100；应答分 `succ_userid_list` / `fail_userid_list` |
| 22 | `ListVipAsync` | POST | `/cgi-bin/exmail/vip/list` | 同上；**仅自建** 99318 | `limit` 默认 100、最大 200；官方明说「不保证每次刚好 limit」⇒ 只看 `has_more` |

### 成员邮箱操作族（M8，2 支 · 仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 23 | `SetEmailAccountStatusAsync` | POST | `/cgi-bin/exmail/account/act_email` | `access_token`(Query) + `InternalAccessToken`；**仅自建** 95512 | `userid` / `publicemail_id` 至少一项，同时传**只操作 userid**；不可禁用超管与企业创建人 |
| 24 | `GetMailUnreadCountAsync` | POST | `/cgi-bin/exmail/mail/get_newcount` | 同上；**仅自建** 95514 | 官方即 POST，`userid` 在包体而非 Query |

### 其他邮件客户端登录设置族（M9，2 支 · 仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 25 | `GetUserOptionAsync` | POST | `/cgi-bin/exmail/useroption/get` | `access_token`(Query) + `InternalAccessToken`；**仅自建** 95513 | 官方即 POST；功能项以 `option.list` 结构承载 |
| 26 | `UpdateUserOptionAsync` | POST | `/cgi-bin/exmail/useroption/update` | 同上；**仅自建** 98008 | 同一 `option.list` 形态；`value` 是**字符串** `"1"`/`"0"` |

## 1. 层级形态二分：三支「三类等宽族」与五支「仅自建族」

M2（L336–L392）与 M2b（L399–L431）把两种形态分别钉死，改动任一支族的应用类型面都要同批改守卫：

- **发送邮件族 / 获取接收的邮件族**：父接口声明面恰 3 / 恰 2，三个应用类型子接口声明面**各恰 0**（空标记），
  子接口一律 `InheritedFrom` 父接口生成实现类（`WechatWorkMailSendService` / `WechatWorkMailReceiveService`），
  避免生成器重复实现公共端点。⇒ 注入 `IWechatWorkInternalMailSendService` 与
  `IWechatWorkThirdPartyMailSendService` 得到的端点集合完全相同，差异只在令牌归属键与权限口径。
- **管理应用邮箱账号 + 管理端五族（共六族）**：父接口声明面**恰 0**、`IsAbstract = true`、`RegistryGroupName` 恒空，
  全部端点落在自建子接口（2 / 5 / 7 / 3 / 2 / 2）；继承链上**恰好只有自建子接口**
  （官方未向第三方与代开发开放本域 ⇒ 本仓**不得**声明 `IWechatWorkThirdParty…` / `IWechatWorkProvider…`，
  那是「凭空扩面」而非补齐对称性）。

命名空间分区遵循 Work 线通用规则（父接口 → `Mud.Wechat.Work.Interfaces`、可注入接口 → `Mud.Wechat.Work`），
由 [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3）锁定，本域无特例。

## 2. 定位键五种形态，互不可替换

本域「谁是主键」在六族之间**并不同构**，用错键型是最常见的写法事故：

| 键 | 类型 | 出现处 | 口径 |
|---|---|---|---|
| （无） | — | 应用邮箱侧（#1~#7） | 应用邮箱由**调用令牌自身**唯一定位，无独立邮箱 id；`GetEmailAliasAsync` 连包体都不要 |
| `groupid` | `string`（**邮箱格式**） | 群组族 #8~#12、[MailGroupBrief](../../../Mud.Wechat.Work.DataModels/Mail/MailGroupBrief.cs) | 群组 ID 本身就是一个邮箱地址（接口注释「邮件群组 ID，邮箱格式」）⇒ 不是整数、也不能拿公共邮箱 id 顶替 |
| `id` / `publicemail_id` | `long` | 公共邮箱族 #13~#19、#23 | 公共邮箱是**整数 ID**；创建返回 [CreatePublicMailResponse.Id](../../../Mud.Wechat.Work.DataModels/Mail/CreatePublicMailResponse.cs)、更新与删除都吃它；`act_email` 侧键名换成 `publicemail_id` |
| `userid` | `string` | #22~#26、`to.userids` 等 | 成员维度一律 userid，且需在应用可见范围内 |
| `mail_id` / `schedule_id` | `string` | #5 / [MailSchedule](../../../Mud.Wechat.Work.DataModels/Mail/MailSchedule.cs) | `mail_id` 由 #4 列表取得；`schedule_id` 缺省即「新建日程/会议」，传了即「修改」，`method=cancel` 时**必须带上** |

另有一处**跨域字段口径**：群组成员列表 `email_list` / `allow_emaillist` 读的是成员的 `biz_mail` 字段
（[CreateMailGroupRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/CreateMailGroupRequest.cs) L28、L60 注释），
即「成员的邮箱地址」而非 userid；该字段由通讯录域签发，本域不锁其取值链。

## 3. 覆盖式与三态列表：更新语义的三种写法

管理端所有「列表」字段都**不是增量增删**，官方语义在两个包装结构上统一承载：

| 结构 | 载荷 | 更新语义（注释原文） |
|---|---|---|
| [MailStringList](../../../Mud.Wechat.Work.DataModels/Mail/MailStringList.cs) | `list: List<string>?` | **不传则保持不变，传空结构或空数组则清空** |
| [MailUintList](../../../Mud.Wechat.Work.DataModels/Mail/MailUintList.cs) | `list: List<long>?` | 同上 |

由此得到三条必须写清的差异：

1. **三态由「属性是否为 null」表达，而非集合内容**：`EmailList = null` ⇒ 官方保持原值；
   `EmailList = new MailStringList()`（`list` 为 null）或 `new MailStringList { List = new List<string>() }` ⇒ **清空**。
   本域 53 个 DTO 仅带 `[HttpJsonSerializable]` / `[JsonPropertyName]`、无任何校验特性 ⇒ 本仓不本地拦截「清空后成员为空」这类非法组合，越界由应答 `errcode` 表达。
2. **「不允许全部清空」是硬约束**：群组成员四列表（`email_list` / `group_list` / `department_list` / `tag_list`）
   与公共邮箱使用成员三列表（`userid_list` / `department_list` / `tag_list`）在创建时**至少填一个**、
   更新时**不允许清空到全空**（#8/#9/#13/#14 注释）。⇒ 想「只换部门不动成员」时必须把不想改的字段留 null。
3. **公共邮箱别名是「覆盖式更新」的显式声明**：[UpdatePublicMailRequest.AliasList](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/UpdatePublicMailRequest.cs) L46–L53
   注释写明「最多 5 个别名；更新时为覆盖式更新，传空结构或传空数组会清空当前邮箱别名」，
   且单个别名须为**有效企业邮箱格式、6~64 字节、企业内唯一**。
4. 应用邮箱侧的 #6 是另一种整体替换：`new_email` 换绑后**原地址降级为别名邮箱并保留收信能力**（不是删除），
   查询面 #7 的应答因此是「主邮箱 `email` + `alias_list` 列表」两件套
   （[GetEmailAliasResponse](../../../Mud.Wechat.Work.DataModels/Mail/GetEmailAliasResponse.cs)）。

## 4. 凭据口径：只有三类凭据，且都不进日志

- **`access_token`**：官方强制 Query 注入（M3 断言 `InjectionMode.Query` + `Name = "access_token"`，MUD005 已知接受风险）。
  归属域由 `[Token(TokenManagerKey = …)]` 声明：三支三类公共族自建侧 `InternalAccessToken`、
  第三方/代开发侧 `CorpAccessToken`（授权企业级令牌，scope = `authCorpId`），管理端六族只有 `InternalAccessToken`；
  一致性由 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）锁定，
  Query 令牌接口白名单由 [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5，本域接口名清单见 L499–L530）锁定。
- **客户端专用密码 `auth_code`**：请求侧只传开关与备注（`create_auth_code` = 0/1、`auth_code_info.remark`），
  **密码本身只出现在创建/更新应答里、且仅返回一次**（[CreatePublicMailResponse](../../../Mud.Wechat.Work.DataModels/Mail/CreatePublicMailResponse.cs)、
  [UpdatePublicMailResponse](../../../Mud.Wechat.Work.DataModels/Mail/UpdatePublicMailResponse.cs)）；
  事后 #18 只能拿到 `auth_code_id` / `create_time` / `last_use_time` / `remark`（[PublicMailAuthCode](../../../Mud.Wechat.Work.DataModels/Mail/PublicMailAuthCode.cs)，注释明示「本列表不返回密码本身」）。
  ⇒ 宿主必须**在创建当次就安全落库**，否则不可恢复，只能 #19 删除后重建；单个公共邮箱经 API 创建的专用密码
  **不超过 10 个（不含已删除）**；`remark` 未设置时官方默认「办公PC」，≤128 字节且须 UTF-8。
  该值是凭据：不得进日志、遥测或异常消息，示例与配置一律占位符。
- **会议入会密码 `meeting.option.password`**：[MailMeetingOption](../../../Mud.Wechat.Work.DataModels/Mail/MailMeetingOption.cs) L16–L18
  口径为「仅可输入 4-6 位纯数字」，同样是凭据，不得回显。
- **本域不存在 `mail_secret` / 邮箱账号密码类参数**（实测：20 份接口与 53 个 DTO 中无此类字段）。
  企业微信邮件 API 的鉴权只有 `access_token`，公共邮箱「密码」只有上述一次性 `auth_code`。

第三方 / 代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标授权企业作用域
（`permanent_code` 只写参数名，不得进日志）；口径见 [AGENTS.md §5.1](../../../../../AGENTS.md)。

## 5. 批量上限与容量闸（全部来自接口注释 / DTO 注释，逐条留档）

| 端点 | 上限 | 出处 |
|---|---|---|
| #1/#2/#3 | 附件 + 正文总大小 ≤ **50M**；附件个数 ≤ **200**；`to.emails` 与 `to.userids` 至少传一个 | 父接口 XML remarks + [MailAttachment](../../../Mud.Wechat.Work.DataModels/Mail/MailAttachment.cs) |
| #2 | `schedule.schedule_admins` ≤ **3** 人，只支持 userid、须同企业且在参与人中 | 父接口 XML + [MailSchedule](../../../Mud.Wechat.Work.DataModels/Mail/MailSchedule.cs) L45–L52 |
| #3 | `meeting.hosts` ≤ **10**（只填 userid）；`meeting.meeting_admins` **仅 1 人** | [MailMeeting](../../../Mud.Wechat.Work.DataModels/Mail/MailMeeting.cs) |
| #4 | `limit` 默认 **100**、最大 **1000** | [GetMailListRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/GetMailListRequest.cs) |
| #8/#9 | `groupname` ≤ **200 字节**、不得重名；成员四列表至少一项；`allow_type`∈{0,1,2} 时**不得**传三个 allow_* 列表，`=3` 时**必须**至少传一项 | 子接口 XML + [CreateMailGroupRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/CreateMailGroupRequest.cs) |
| #13/#14 | 公共邮箱名 ≤ **64 字符或 32 汉字**、不得重名；使用成员三列表不能同时为空；别名 ≤ **5 个**、单个 6~64 字节 | 子接口 XML + UpdatePublicMailRequest |
| #13/#14 | API 创建的客户端专用密码 ≤ **10 个**（不含已删除）；`remark` ≤ **128 字节** | 子接口 XML remarks + PublicMailAuthCodeInfo |
| #16 | `id_list` 批量整型数组（单批上限注释无数值 ⇒ 见缺口） | [GetPublicMailRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/GetPublicMailRequest.cs) |
| #20/#21 | 单次 `userid_list` ≤ **100** | 子接口 XML |
| #22 | `limit` 默认 **100**、最大 **200**；且官方注明「不保证每次返回刚好 limit」 | [ListMailVipRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/ListMailVipRequest.cs) + [ListMailVipResponse](../../../Mud.Wechat.Work.DataModels/Mail/ListMailVipResponse.cs) |
| #23 | `userid` 与 `publicemail_id` 至少传一项，**同时传只操作 `userid`**；不可禁用超管与企业创建人 | 子接口 XML |
| 全族 | 「可调用应用」列表与权限（邮件 / 日程 / 会议）由宿主在管理后台配置；本域无 SDK 侧限流或批次裁剪代码（DTO 仅 `[HttpJsonSerializable]` / `[JsonPropertyName]`，越界由应答 `errcode` 表达） | 各族子接口 XML 权限口径 + 本域 DTO 特性实测 |

**频率上限**：本域 20 份接口注释中**未留档任何 QPS / 每日次数数值**（实测 grep 无「频率 / 次/秒 / 并发」命中）⇒ 见缺口表。

## 6. 官方查询类走 POST：只有三支是 GET

26 支里只有 **#11 `group/get`、#12 `group/search`、#17 `publicmail/search`** 是 GET 且参数走 Query，
其余 23 支全部是 POST（含所有「查询/获取」语义端点）。守卫逐条钉住动词与参数名：
M5 断言 `group/get` 的 `groupid`、`group/search` 的 `fuzzy` + `groupid` **必须以 `[Query("…")]` 显式承载**；
M6 断言 `publicmail/search` 的 `fuzzy` + `email` 同理。⇒ **不得**为「REST 美观」把
`publicmail/get`（`id_list`）、`useroption/get`（`userid` + `type`）、`mail/get_newcount`（`userid`）、
`app/get_email_alias`、`app/get_mail_list`、`app/read_mail`、`vip/list`、`account/act_email` 改成 GET，
也**不得**把 GET 三支的 Query 参数塞进包体。
`fuzzy` 在本仓按 `long` 建模（1 = 模糊搜索、0 = 取全部），是必填位置参数，无默认值。

## 7. 字段名照官方原文，含三处官方文档自相矛盾

**照抄的连写与大小写**（改一个字母即与官方报文错配）：`allow_emaillist` / `allow_departmentlist` / `allow_taglist`
（**无下划线**，与同报文里的 `email_list` / `department_list` / `tag_list` **拼法不同**，
见 [GetMailGroupResponse](../../../Mud.Wechat.Work.DataModels/Mail/GetMailGroupResponse.cs) L47–L57）、
`publicemail_id`、`biz_mail`、`mail_data`、`get_newcount`、`water_mark_type`（同族里唯一带下划线的会议字段）、
`enable_waiting_room`、`remind_before_event_mins`、`repeat_week_of_month`。
另有 `repeat_week_of_month`：**官方参数表未列出、仅请求示例携带（空数组形态）**，
本仓按示例原文承载并在 [MailReminders](../../../Mud.Wechat.Work.DataModels/Mail/MailReminders.cs) L64–L72 注释留档，勿当臆造字段删除。

DTO 层留档的三处「官方文档与示例不一致」，本仓取舍都有注释理由，改动前必读：

| 位置 | 矛盾 | 本仓取舍 |
|---|---|---|
| [GetMailListResponse.NextCursor](../../../Mud.Wechat.Work.DataModels/Mail/GetMailListResponse.cs) L17–L24 | 官方对 `next_cursor` 的**说明文字**写成「应用邮箱账号中邮件未读数」，与游标语义明显不符（疑笔误） | 按官方请求示例与翻页用法以**游标**承载（`string?`） |
| `GetMailListResponse.HasMore` vs `ListMailVipResponse.HasMore` | 同为 `has_more`，收件箱列表按 **0/1 数值**、高级功能账号列表按**布尔** `true` | 前者 `long?`、后者 `bool?` ⇒ **两个类型不共用**，勿「统一」 |
| [SearchMailGroupResponse.Count](../../../Mud.Wechat.Work.DataModels/Mail/SearchMailGroupResponse.cs) L16–L24 | 官方参数表标 `count` 为 string，响应示例作数字（如 `2`） | 以示例为准按 `long?` 承载 |

`option.list` 两处形态也不同：请求侧 [GetMailUserOptionRequest.Type](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/GetMailUserOptionRequest.cs) 是
**平铺的 `type` 整数数组**（1 强制启用安全登录 / 2 IMAP/SMTP / 3 POP/SMTP / 4 是否启用安全登录），
响应侧则是 [MailUserOptionList](../../../Mud.Wechat.Work.DataModels/Mail/MailUserOptionList.cs) → `list: List<MailUserOption>`，
且项内 `value` 是**字符串** `"1"`/`"0"`（[MailUserOption](../../../Mud.Wechat.Work.DataModels/Mail/MailUserOption.cs) L23–L25）。

## 8. `enable_id_trans`：第三方专属，模板语法照原文

仅 **#1~#3** 有该字段（`int?`，0 否 / 1 是，默认 0）。官方注明「仅第三方应用需要用到，企业自建应用可忽略」，
且**只有 `subject`、`content`、`attachment_list[].file_name` 三处支持转译**。模板语法（[MailSendRequest](../../../Mud.Wechat.Work.DataModels/Mail/RequestModel/MailSendRequest.cs) L53–L66 注释照官方原文留档）：

```
$departmentName=DEPARTMENT_ID$   $userName=USERID$
$userAlias=USERID$               $userAliasOrName=USERID$
```

模板语法无效或 userid / 部门 id 无效时**保留原样不替换**；`userAlias` 无别名时保留原样；
`userAliasOrName` 别名优先于姓名。本仓对该字段原样透传（无本地校验或替换代码）。

## 9. 邮件回调事件不在本域：`receive_email` 跨族同名的消歧方式

「应用邮箱收到邮件」「公共邮箱收到邮件」是**回调推送（XML）**，由 [Mud.Wechat.Work.Callback](../../../Mud.Wechat.Work.Callback/) 包承载，
**不属于本域**（本域 26 支全部是主动 HTTP 调用）。此处只写消歧契约，避免把两侧搅在一起：

- 官方裸 `ChangeType` 值 `receive_email` 在**应用邮箱族**与**公共邮箱族**中**同名** ⇒ 逐 `ChangeType` 做键无法消歧，
  故本仓邮箱族**以族事件值为事件键**：`app_email_change`（官方 97495 / 97517 / 97506，三份正文逐字一致，ADR-14）与
  `public_email_change`（官方 100180，**仅自建**），类别由信封 `ChangeType` 判别——
  与客户联系/获客族同一套规则。常量见 [WechatCallbackEventTypes.cs](../../../Mud.Wechat.Work.Abstractions/Callback/WechatCallbackEventTypes.cs) L614–L647。
- 该规则在 [WechatCallbackEvent.cs](../../../Mud.Wechat.Work.Abstractions/Callback/WechatCallbackEvent.cs) L138–L139 与 L279–L293 的键解析咽喉点实现，
  键集完整性由 [WechatCallbackContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCallbackContractGuards.cs)（CB2，L121–L122）锁定，
  跨族不串号（应用邮箱报文不得命中公共邮箱载荷）由 [WechatCallbackPayloadReaderTests.cs](../../../../../Tests/Mud.Wechat.Work.Callback.Tests/WechatCallbackPayloadReaderTests.cs) L1379–L1431 锁定。
- 载荷面：[AppEmailChangedPayload](../../../Mud.Wechat.Work.Callback/Events/Payloads/Mail/AppEmailChangedPayload.cs) 只有 `Amount`；
  [PublicEmailChangedPayload](../../../Mud.Wechat.Work.Callback/Events/Payloads/Mail/PublicEmailChangedPayload.cs) 有 `Id` + `Amount`（应用邮箱报文无 `Id` 节点），
  契约登记为 `[WechatCallbackContract]` 声明式（族前置 `Unknown`、通道 `App`；应用邮箱 `WechatAppTypeSet.All`、公共邮箱 `Internal`）。
  ⇒ **不要**在本域接口里加「拉取收件箱」之外的回调式能力，也不要给载荷加 `WechatAppType` 分支。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMailApi());

// 自建应用：注入「仅自建」子接口（8 个父接口均 IsAbstract，不可注入）
public sealed class MailOps(
    IWechatWorkInternalMailSendService send,
    IWechatWorkInternalMailReceiveService receive,
    IWechatWorkInternalMailGroupService groups,
    IWechatWorkInternalMailPublicMailService publicMails,
    IWechatWorkInternalMailVipService vips,
    IWechatWorkInternalMailAccountService accounts)
{
    // 1) 发一封带附件的普通邮件（正文 html；附件 ≤200 个、含正文总量 ≤50M）
    public async Task SendAsync(string userid, CancellationToken ct)
    {
        var resp = await send.SendNormalMailAsync(new MailSendRequest
        {
            To = new MailRecipient { Userids = new List<string> { userid } },  // to.emails / to.userids 至少一项
            Subject = "月度对账单",
            Content = "<p>请查收。</p>",
            ContentType = "html",                                              // html（默认）/ text
            AttachmentList =
            [
                new MailAttachment
                {
                    FileName = "statement.txt",
                    Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("demo body")),
                },
            ],
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/exmail/app/compose_send");
    }

    // 2) 翻页读收件箱：has_more 是 0/1 数值（与 vip/list 的布尔 has_more 不同型）
    public async Task<List<string>> PullMailIdsAsync(long beginAt, long endAt, CancellationToken ct)
    {
        List<string> mailIds = new List<string>();
        string? cursor = null;
        while (true)
        {
            var resp = await receive.GetMailListAsync(new GetMailListRequest
            {
                BeginTime = beginAt,        // Unix 时间戳（官方必填）
                EndTime = endAt,
                Cursor = cursor,            // 首次可不填
                Limit = 1000,               // 默认 100、最大 1000
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/exmail/app/get_mail_list");

            List<MailListItem> items = resp.MailList ?? new List<MailListItem>();
            foreach (MailListItem item in items)
            {
                if (item.MailId is not null) mailIds.Add(item.MailId);
            }

            if (resp.HasMore != 1 || string.IsNullOrEmpty(resp.NextCursor)) break;
            cursor = resp.NextCursor;
        }
        return mailIds;
    }

    // 3) 建公共邮箱并顺带创建客户端专用密码：auth_code 仅本次返回，必须立刻安全落库（不得进日志）
    public async Task<long> CreatePublicMailAsync(CancellationToken ct)
    {
        var resp = await publicMails.CreatePublicMailAsync(new CreatePublicMailRequest
        {
            Email = "demo-service@example.invalid",
            Name = "示例客服邮箱",
            UseridList = new MailStringList { List = new List<string> { "demo_user_0001" } },
            CreateAuthCode = 1,                                          // 0 否（默认）/ 1 是
            AuthCodeInfo = new PublicMailAuthCodeInfo { Remark = "办公PC" },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/exmail/publicmail/create");
        // resp.AuthCode 为凭据：只交宿主密钥存储，不写日志、不进异常消息
        return resp.Id ?? 0L;
    }

    // 4) 更新邮件群组：只改名称 ⇒ 其余列表留 null（不传 = 保持原值；传空结构/空数组 = 清空）
    public async Task RenameGroupAsync(string groupid, string newName, CancellationToken ct)
    {
        var resp = await groups.UpdateMailGroupAsync(new UpdateMailGroupRequest
        {
            Groupid = groupid,      // 群组 ID 本身就是邮箱格式字符串
            Groupname = newName,    // ≤200 字节、不得与其他群组重名
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/exmail/group/update");
    }

    // 5) 分配高级功能账号：同步批量、单次 ≤100，成败分两个列表返回
    public async Task<(int Succ, int Fail)> GrantVipAsync(List<string> userids, CancellationToken ct)
    {
        var resp = await vips.BatchAddVipAsync(new BatchAddMailVipRequest
        {
            UseridList = userids.Take(100).ToList(),
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/exmail/vip/batch_add");
        List<string> succ = resp.SuccUseridList ?? new List<string>();
        List<string> fail = resp.FailUseridList ?? new List<string>();
        return (succ.Count, fail.Count);
    }

    // 6) 查询应用自己的应用邮箱账号（官方不需要请求包体 ⇒ 方法签名只有一个 CancellationToken）
    public async Task<(string Email, List<string> AliasList)> DescribeAppMailboxAsync(CancellationToken ct)
    {
        var resp = await accounts.GetEmailAliasAsync(ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/exmail/app/get_email_alias");
        return (resp.Email ?? string.Empty, resp.AliasList ?? new List<string>());
    }
}

// 第三方 / 代开发：三类公共面族可用（#1~#5），先切授权企业作用域再调用
using (switcher.UseCorpScope(appKey: "default", authCorpId, permanentCode))
{
    var list = await thirdPartyMailReceive.GetMailListAsync(
        new GetMailListRequest { BeginTime = beginAt, EndTime = endAt }, ct);
    WechatWorkException.ThrowIfFailed(list, "/cgi-bin/exmail/app/get_mail_list");
}
```

> `GetEmailAliasAsync` 挂在**管理应用邮箱账号族**的
> [IWechatWorkInternalMailAccountService](./Account/IWechatWorkMailAccountService_Internal.cs) 上（与公共邮箱族无关），
> 这正是「无包体端点」的调用形态。

判错为单层普通信封（`errcode == 0` 即成功，15 个应答 DTO 全部继承 `WechatWorkResponse`）；
令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域各端点频率上限 | 20 份接口注释**未留档任何 QPS / 每日次数数值**（含三类公共面族与仅自建六族）⇒ **未逐项核验** | 逐页核验后补接口 remarks，再更新本文第 5 节；不得从邻域借数值 |
| 时间戳单位 | `begin_time` / `end_time`（#4）、`start_time` / `end_time` / `repeat_until`（#2/#3）、`create_time` / `last_use_time`（#18）注释只写「Unix 时间戳」，**秒/毫秒未标明** ⇒ 未逐项核验 | 需要精确到单位的场景先核验官方页 |
| #16 `id_list` 单批上限 | DTO 只有 `List<long>? IdList`，注释无最大值；守卫只锁路由 ⇒ 未逐项核验 | 批次裁剪前先核验，勿拿 #20/#21 的 100 套用 |
| `mail_data`（eml）体量与编码 | #5 注释只说「完整原始邮件格式内容」，无大小上限、无编码口径 ⇒ 未逐项核验 | 大邮件场景自行核验并限流，勿在 SDK 侧加隐式截断 |
| `method` 取值全集 | [MailSchedule](../../../Mud.Wechat.Work.DataModels/Mail/MailSchedule.cs) L21–L27 留档 `request` / `cancel`（默认 request）；其它官方值是否存在**未逐项核验** | 出现新值先核验再决定是否建枚举 |
| 会议 `option` 取值边界 | [MailMeetingOption](../../../Mud.Wechat.Work.DataModels/Mail/MailMeetingOption.cs) 留档 9 个字段的枚举值与默认值；`enter_restraint` 是否存在值 1、`auto_record` 与官方会议域是否等价**未逐项核验** | 与会议域交叉时查 `Interfaces/Meeting/`，勿互相套用 |
| `allow_type` 之外的权限维度 | 注释留档 0 企业成员 / 1 任何人 / 2 组内成员 / 3 自定义成员；群组是否另有发信白名单类字段 ⇒ 未逐项核验 | 需要更细权限时先核验官方页 |
| 「可调用应用」配置的可调用链 | 各族注释统一要求「配置到可调用应用列表中的应用 secret 所获取的 access_token」，但配置动作与生效时机不在本仓留档 ⇒ 未逐项核验 | 属管理后台配置面，勿在 SDK 侧探测 |
| `email_list` 取值来源（成员 `biz_mail`） | 本域只消费该邮箱地址；字段由通讯录域写入，其可见范围与空值行为**未在本域核验** | 需要校验链时查 `Interfaces/Contacts/` |
| 守卫 M3 注释的接口计数 | 注释写「邮件模块全部 **10** 个接口」，而该用例 `interfaces` 数组实际枚举 **20** 个（8 父 + 12 子）；M2/M2b 的形态断言与代码实测一致 | 判定为**注释陈旧、断言正确**；本文以实测 20 为准。若整理该注释须同批保持断言不变 |
| 本域与 [../../README.md](../../README.md) 的口径 | 线 README 第 30 行只写域定位、无端点计数；模块枚举数（38）为该文件自述 ⇒ 与本域无冲突，但其统计可能整体陈旧 | 以本域守卫实测（26 支）为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（50M / 200 个、200 字节、64 字符或 32 汉字、5 个别名、10 个专用密码、128 字节备注、100 / 200 / 1000 的 limit、3 / 10 / 1 的主持人与管理员、id 转译模板语法、权限与「可调用应用」口径） | 本目录 20 份接口文件的 XML 注释 |
| 发送邮件族路由 / 计数（恰 3）与「三端点共用 compose_send」 | [WechatMailContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs) M1（L192–L203，表 `MailSendRoutes` L52–L63） |
| 接收族路由 / 计数（恰 2） | M1b（L208–L216，表 L68–L76） |
| 应用邮箱账号族（恰 2）+「get_email_alias 不得有 [Body]」 | M1c（L222–L235，表 L82–L90） |
| 群组族（恰 5）+ GET/Query 参数名（`groupid` / `fuzzy`） | M5（L241–L269，表 L96–L113） |
| 公共邮箱族（恰 7）+ `fuzzy` / `email` Query 断言 | M6（L275–L293，表 L119–L142） |
| 高级功能账号族（恰 3）+ 路由必须落 `/cgi-bin/exmail/vip/`（与安全管理域 `security/vip` 互不相通） | M7（L299–L307，表 L148–L159）；邻域见 [IWechatWorkInternalMailVipService.cs](./Vip/IWechatWorkMailVipService_Internal.cs) L81–L83 与 [Interfaces/Security/IWechatWorkSecurityVipService_Internal.cs](../Security/IWechatWorkSecurityVipService_Internal.cs) |
| 成员邮箱操作族（恰 2）/ 客户端登录设置族（恰 2） | M8（L313–L318，表 L164–L172）、M9（L324–L329，表 L178–L186） |
| 层级形态（三类公共族父接口 3/2 + 子接口 0；六族父接口 0 + 自建子接口 2/5/7/3/2/2）、`IsAbstract`、`InheritedFrom` 父实现类 | M2（L336–L392）、M2b（L399–L431）、辅助断言 `AssertFamilyHierarchy`（L545–L576）、`AssertRoutes`（L528–L539） |
| 令牌绑定（20 个接口 × `Wechat.AccessToken` + Query + `access_token`） | M3（L437–L474） |
| AOT JSON 上下文登记（恰 53 个契约面类型，逐族分组） | M4（L479–L525）+ [MailJsonContext.g.cs](../../../Mud.Wechat.Work.DataModels/Generated/MailJsonContext.g.cs)（生成物，`internal`）+ [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) L157 |
| DTO 属性名 / 类型 / JSON 名 / 取值域与三处官方矛盾注释 | [../../../Mud.Wechat.Work.DataModels/Mail/](../../../Mud.Wechat.Work.DataModels/Mail/)（响应与共享结构在根、请求在 `RequestModel/`；命名空间恒为 `Mud.Wechat.Work.DataModels.Mail`） |
| Query 令牌注入白名单（G5，本域 20 个接口名清单） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) L499–L530 |
| 令牌归属键（TO1~TO3）与命名空间二分（N1~N3） | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)、[WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) |
| 邮箱族回调（事件键消歧、载荷字段、开放面） | [WechatCallbackEventTypes.cs](../../../Mud.Wechat.Work.Abstractions/Callback/WechatCallbackEventTypes.cs) L614–L647、[WechatCallbackEvent.cs](../../../Mud.Wechat.Work.Abstractions/Callback/WechatCallbackEvent.cs) L138–L139 / L279–L293、[WechatCallbackContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCallbackContractGuards.cs) CB2 L121–L122、[WechatCallbackPayloadReaderTests.cs](../../../../../Tests/Mud.Wechat.Work.Callback.Tests/WechatCallbackPayloadReaderTests.cs) L1379–L1431 |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs) L71–L72、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) L217–L218、本目录子接口的 `RegistryGroupName = "Mail"` |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方文档号（三类公共端点须三模式齐列）与业务限制（频率 / 容量 / 覆盖语义 / 权限可见范围），
   **同批**改 [WechatMailContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)
   对应的八张路由表与该族的 `HaveCount` +（仅发送族）`Distinct` 断言；
   改开放面须同时改 M2 / M2b 的父接口声明面计数与子接口类型化面计数——**子接口计数即官方开放面**，
   给「仅自建」六族凭空加第三方/代开发子接口前必须先核实官方是否补页。
2. **改动词或参数承载方式** → M5 / M6 显式断言 GET 端点的 `[Query("groupid")] / [Query("fuzzy")] / [Query("email")]`，
   M1c 断言 `get_email_alias` **只允许一个 `CancellationToken` 参数**；把官方 POST 查询类改 GET 会把这三处一起打红。
3. **加 / 改 DTO** → 标 `[HttpJsonSerializable(SerializerClassName = "Mail")]`（本域 53 型共用**单个** `MailJsonContext`）→
   跑 [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) 与
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)（`Generated/MailJsonContext.g.cs` 是生成物，勿手改）→
   同批更新 M4 的类型清单与 [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) 的上下文装配；
   新增包装结构时优先复用 `MailStringList` / `MailUintList`，不要再造同义 `{list:[…]}` 壳。
4. **新增接口族 / 重命名子接口** → 三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddMailApi()` XML（其端点叙述须与本域计数一致）、
   可注入子接口 `RegistryGroupName = "Mail"`（父接口保持空组，N3 锁定），并跑 N1~N3。
5. **改回调面（不在本域）** → 邮箱族事件键 / 载荷改动落在
   [Mud.Wechat.Work.Callback](../../../Mud.Wechat.Work.Callback/) 与本域无关的文件里：`[WechatCallbackContract]` +
   `[PayloadContract]` + CB 系列守卫（CB2 / CB4b / CB4c），并同批核对族事件值的消歧注释；
   **邮件相关回调事件属 `Mud.Wechat.Work.Callback` 包，不在本域**，勿在本目录新增「收信」类端点。
6. **凭据面** → `access_token`、`auth_code`、`meeting.option.password`、`permanent_code` 一律只写参数名、不得出现真值，
   不进日志 / 遥测 / 异常消息；新增 Query 侧凭据参数名须同批走「补脱敏词表 / 登记豁免（附追踪号）」二选一（G7 口径）。
7. **文档同步** → 本文与 [../../README.md](../../README.md) 的域定位与端点计数表述同步更新；
   最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1) 与
   [../../../../../scripts/audit-config-keys.ps1](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
