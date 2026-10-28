# Contacts 域 · 通讯录

企业微信「通讯录」域：本仓在**六支契约守卫**里共落地 **52 处端点声明**（各守卫的 `Routes` 数组条目数），
去重后为 **44 条官方 HTTP 路由**——差异只来自成员域与部门域把「写入 / 邀请」端点按官方「分文档声明」形态
在自建与第三方两支子接口各自重复声明（成员 +5、部门 +3），并非六支守卫之外还有隐藏端点。
接口按六子族分文件（父接口 `IsAbstract = true` 落 `Mud.Wechat.Work.Interfaces`、可注入子接口落 `Mud.Wechat.Work`，
见 [N1~N3 守卫](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)）：

- 成员管理：[IWechatWorkUsersService.cs](./IWechatWorkUsersService.cs)（父，8 条读取面）+
  [_Internal](./IWechatWorkUsersService_Internal.cs)（7）/ [_ThirdParty](./IWechatWorkUsersService_ThirdParty.cs)（8）/
  [_Provider](./IWechatWorkUsersService_Provider.cs)（空标记）
- 部门管理：[IWechatWorkDepartmentsService.cs](./IWechatWorkDepartmentsService.cs)（父，3）+
  [_Internal](./IWechatWorkDepartmentsService_Internal.cs)（3）/ [_ThirdParty](./IWechatWorkDepartmentsService_ThirdParty.cs)（3）/
  [_Provider](./IWechatWorkDepartmentsService_Provider.cs)（空标记）
- 异步导入：[IWechatWorkBatchService.cs](./IWechatWorkBatchService.cs)（父，4）+
  [_Internal](./IWechatWorkBatchService_Internal.cs) / [_ThirdParty](./IWechatWorkBatchService_ThirdParty.cs)（均空标记，**无代开发子接口**）
- 通讯录查看权限：[IWechatWorkContactRulesService.cs](./IWechatWorkContactRulesService.cs)（父，**零端点**）+
  [_Internal](./IWechatWorkContactRulesService_Internal.cs)（4，唯一应用类型子接口）
- 异步导出：[IWechatWorkExportService.cs](./IWechatWorkExportService.cs)（父，5）+
  [_Internal](./IWechatWorkExportService_Internal.cs) / [_ThirdParty](./IWechatWorkExportService_ThirdParty.cs) / [_Provider](./IWechatWorkExportService_Provider.cs)（均空标记）
- 标签管理：[IWechatWorkTagsService.cs](./IWechatWorkTagsService.cs)（父，7）+
  [_Internal](./IWechatWorkTagsService_Internal.cs) / [_ThirdParty](./IWechatWorkTagsService_ThirdParty.cs) / [_Provider](./IWechatWorkTagsService_Provider.cs)（均空标记）

DI 注册组名 `Contact`（六子族共用），模块枚举
[`WechatModule.Contact`](../../Extensions/WechatModule.cs)（[**`AddContactApi()`**](../../Extensions/WechatWorkServiceBuilder.cs)
—— **单数**，是本仓既有命名，勿「顺手改成 `AddContactsApi`」）。逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 21 个接口文件的 XML 注释、六支契约守卫
> （U/D/B/CR/E/T 系列，见文末「事实来源」）、[DataModels/Contacts](../../../Mud.Wechat.Work/Contacts/)
> 与 [DataModels/Contracts/Users](../../../Mud.Wechat.Work/Contacts/Users/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，接口注释里的官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

「令牌 / 开放面」列写本仓真实契约：`access_token` 恒为 Query 注入（U3 / D3 / B3 / CR3 / E3 / T3，MUD005 已接受风险，
无法改 Header）；「开放面」指官方对该端点开放、且本仓据以落位的应用类型集合——父接口的读取面对三类应用同时开放
（代开发子接口是空标记、经继承获得父接口端点），写入 / 邀请端点仅在自建与第三方两支子接口各自声明。

### 成员子族（守卫 U1，23 处声明 / 18 条去重路由）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetUserAsync` | GET | `/cgi-bin/user/get` | `access_token`(Query)；三类 | 读取成员；敏感字段按可见范围与停返时间线（见 §4） |
| 2 | `GetUserSimpleListAsync` | GET | `/cgi-bin/user/simplelist` | 三类 | 部门成员摘要（userid/name/department/open_userid） |
| 3 | `GetUserDetailListAsync` | GET | `/cgi-bin/user/list` | 三类 | 部门成员详情；官方提示性能较低 |
| 4 | `ListUserIdsAsync` | POST | `/cgi-bin/user/list_id` | 三类 | 游标分页 ID 列表；2022-08 加固后官方推荐替代接口；**查询却走 POST** |
| 5 | `GetUserIdByMobileAsync` | POST | `/cgi-bin/user/getuserid` | 三类 | 手机号换 userid；路由名 `getuserid` 全连写 |
| 6 | `GetUserIdByEmailAsync` | POST | `/cgi-bin/user/get_userid_by_email` | 三类 | 邮箱换 userid |
| 7 | `ConvertUserIdToOpenIdAsync` | POST | `/cgi-bin/user/convert_to_openid` | 三类 | 企业支付场景；成员须在可见范围且微信登录 / 关注插件 |
| 8 | `ConvertOpenIdToUserIdAsync` | POST | `/cgi-bin/user/convert_to_userid` | 三类 | 与 #7 同文档页（90202/90338/96261） |
| 9 | `CreateUserAsync` | POST | `/cgi-bin/user/create` | 自建 + 第三方 | 自建与第三方各声明一次（同路由）；须通讯录同步 / 第三方通讯录应用 |
| 10 | `UpdateUserAsync` | POST | `/cgi-bin/user/update` | 自建 + 第三方 | 除 userid 外全可选，未传字段不更新 |
| 11 | `DeleteUserAsync` | GET | `/cgi-bin/user/delete` | 自建 + 第三方 | 删成员 |
| 12 | `BatchDeleteUsersAsync` | POST | `/cgi-bin/user/batchdelete` | 自建 + 第三方 | ≤200 个 userid；**有无效即整批失败**（见 §5） |
| 13 | `InviteMembersAsync` | POST | `/cgi-bin/batch/invite` | 自建 + 第三方 | 路由在 `batch/` 下但归属成员域；user≤1000 / party≤100 / tag≤100 |
| 14 | `CompleteSecondaryAuthAsync` | GET | `/cgi-bin/user/authsucc` | **仅自建** | 登录二次验证放行 |
| 15 | `GetJoinQrcodeAsync` | GET | `/cgi-bin/corp/get_join_qrcode` | **仅自建** | 路由在 `corp/` 下但归属成员域；join_qrcode 有效期 7 天 |
| 16 | `ListMemberAuthAsync` | POST | `/cgi-bin/user/list_member_auth` | **仅第三方** | 「成员授权」模式分页取 open_userid；limit 默认与最大 1000 |
| 17 | `CheckMemberAuthAsync` | POST | `/cgi-bin/user/check_member_auth` | **仅第三方** | 查询成员是否已授权 |
| 18 | `ListSelectedTicketUsersAsync` | POST | `/cgi-bin/user/list_selected_ticket_user` | **仅第三方** | 选人 ticket 换 open_userid |

### 部门子族（守卫 D1，9 处声明 / 6 条去重路由）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 19 | `GetDepartmentListAsync` | GET | `/cgi-bin/department/list` | 三类 | 递归取子孙部门；性能较低，官方建议改用 #20 + #21 |
| 20 | `GetChildDepartmentIdListAsync` | GET | `/cgi-bin/department/simplelist` | 三类 | 取子部门 ID 列表（id/parentid/order） |
| 21 | `GetDepartmentAsync` | GET | `/cgi-bin/department/get` | 三类 | 取单个部门详情 |
| 22 | `CreateDepartmentAsync` | POST | `/cgi-bin/department/create` | 自建 + 第三方 | **单次一个部门**（见 §5）；层级 ≤15、总数 ≤3 万 |
| 23 | `UpdateDepartmentAsync` | POST | `/cgi-bin/department/update` | 自建 + 第三方 | 非必须字段未指定即不改 |
| 24 | `DeleteDepartmentAsync` | GET | `/cgi-bin/department/delete` | 自建 + 第三方 | 不能删根部门、不能删含子部门/成员的部门 |

### 异步导入子族（守卫 B1，4 条）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 25 | `SyncUsersAsync` | POST | `/cgi-bin/batch/syncuser` | 自建 + 第三方 | 增量更新成员；先上传 csv 取 `media_id` |
| 26 | `ReplaceUsersAsync` | POST | `/cgi-bin/batch/replaceuser` | 自建 + 第三方 | 全量覆盖成员（危险，见 §6） |
| 27 | `ReplaceDepartmentsAsync` | POST | `/cgi-bin/batch/replaceparty` | 自建 + 第三方 | 全量覆盖部门 |
| 28 | `GetBatchJobResultAsync` | GET | `/cgi-bin/batch/getresult` | 自建 + 第三方 | **路由 `getresult` 无下划线**；jobid 走 Query（≤64 字节） |

### 通讯录查看权限子族（守卫 CR1，4 条，全部仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 29 | `CreateContactRulesAsync` | POST | `/cgi-bin/contactrule/create` | **仅自建**（通讯录同步应用） | 一次 ≤100 条；每条 `rule_id` 不填 |
| 30 | `GetContactRulesAsync` | POST | `/cgi-bin/contactrule/list` | **仅自建** | **读取列表也是 POST 且无请求体参数**（见 §8） |
| 31 | `UpdateContactRulesAsync` | POST | `/cgi-bin/contactrule/update` | **仅自建** | 一次 ≤100 条；每条 `rule_id` 必填、**不能改规则类型** |
| 32 | `DeleteContactRulesAsync` | POST | `/cgi-bin/contactrule/delete` | **仅自建** | 一次 ≤100 个 `rule_id` |

### 异步导出子族（守卫 E1，5 条）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 33 | `ExportUsersAsync` | POST | `/cgi-bin/export/simple_user` | 三类 | 导出成员（simple list）；返回 `jobid` |
| 34 | `ExportUserDetailsAsync` | POST | `/cgi-bin/export/user` | 三类 | 导出成员详情 |
| 35 | `ExportDepartmentsAsync` | POST | `/cgi-bin/export/department` | 三类 | 导出部门 |
| 36 | `ExportTagUsersAsync` | POST | `/cgi-bin/export/taguser` | 三类 | 导出标签成员（要求标签读取权限） |
| 37 | `GetExportResultAsync` | GET | `/cgi-bin/export/get_result` | 三类 | **路由 `get_result` 带下划线**（与 #28 拼写不同，见 §7） |

### 标签子族（守卫 T1，7 条）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 38 | `CreateTagAsync` | POST | `/cgi-bin/tag/create` | 三类 | tagname ≤32 字、全局不重名；标签总数 ≤3000 |
| 39 | `UpdateTagNameAsync` | POST | `/cgi-bin/tag/update` | 三类 | 更新标签名；须为该标签创建者 |
| 40 | `DeleteTagAsync` | GET | `/cgi-bin/tag/delete` | 三类 | tagid 走 Query |
| 41 | `GetTagMembersAsync` | GET | `/cgi-bin/tag/get` | 三类 | 无调用限制，仅返回可见范围内成员 |
| 42 | `AddTagMembersAsync` | POST | `/cgi-bin/tag/addtagusers` | 三类 | userlist≤1000、partylist≤100；三态返回（见 §9） |
| 43 | `RemoveTagMembersAsync` | POST | `/cgi-bin/tag/deltagusers` | 三类 | 与 #42 共用响应体 |
| 44 | `GetTagListAsync` | GET | `/cgi-bin/tag/list` | 三类 | 自建 / 同步助手取全部；第三方 / 代开发仅取自己创建的 |

## 1. 接口层级：能力不等宽是本域的第一约束

六子族的落位不是「整齐划一」，而是按官方对各应用类型的实际开放面差异化落位，且分别由一支守卫锁定：

- **成员 / 部门**（U2 / D2）：公共读取面收敛父接口，写入端点按官方「分文档声明」形态在**自建与第三方两支子接口各自重复声明**
  （同路由、令牌作用域不同）。代开发子接口 `IWechatWorkProviderUsersService` / `IWechatWorkProviderDepartmentsService`
  是**空标记**，U2 / D2 以 `DeclaredOnly` 反射断言其「不得新增端点」——代开发只继承父接口读取面，官方未开放写入。
  成员域第三方子接口另有三支「成员授权模式」专属端点（#16–#18），自建无。
- **异步导入**（B2）：自建与第三方两支子接口均为空标记，且**不存在代开发子接口**（官方对代开发无此功能），
  B3 令牌断言只枚举父 / 自建 / 第三方三支。
- **通讯录查看权限**（CR2）：父接口**零端点**，全部 4 支收敛于**唯一**的应用类型子接口 `IWechatWorkInternalContactRulesService`
  （官方仅向自建 / 通讯录同步应用开放）；不设第三方 / 代开发子接口。
- **异步导出 / 标签**（E2 / T2）：全部端点收敛父接口、对三类应用完全一致，三支子接口均为空标记。

⇒ 判「某端点归哪支接口」的唯一依据是「官方对哪几类应用开放」+「该守卫的路由表落在 `Interface` 字段的是父接口还是子接口」，
不要凭「写入端点理应放子接口 / 读取端点理应放父接口」的直觉重排——那会同时打坏 U/D/B/CR/E/T 与 G5。

## 2. 令牌链与凭据归属域：`sys` 同步助手是本域的写权限咽喉

- 六子族全部令牌路由键恒为 `WechatTokenTypes.AccessToken`、`TokenType` 不按应用类型拆分、Query 注入参数名 `access_token`
  （U3/D3/B3/CR3/E3/T3）；这些接口名整体登记在 G5 白名单
  [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
  内，新增 Query 凭据参数须走「补词表 / 登记豁免」二选一。
- 归属域（`TokenManagerKey`）由
  [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)
  （TO1~TO3，按程序集反射枚举、不写名单）锁定，本域取值：自建子接口 = `InternalAccessToken`；
  第三方 / 代开发子接口 = `CorpAccessToken`（企业级令牌，`scope = authCorpId`，一企一份，调用前须经
  `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切作用域）。`permanent_code` 只写参数名，
  **不得进日志 / 遥测 / 异常消息**。
- **写权限咽喉**：成员 / 部门的创建更新删除、批量导入、通讯录查看权限四族，接口注释的官方权限口径均为
  「须以**通讯录同步助手**（自建）或第三方通讯录应用的 secret 签发的令牌调用」——即 `access_token` 本身不区分，
  但换令牌用的 `corpsecret` 必须是通讯录同步助手那一个（参数名 `corpsecret`，凭据，不得落日志）。
- **跨域提示**：通讯录同步助手在**回调接收面**以通配键 `"*"` 路由（通讯录变更事件的推送与分发属 `Callback` 包，
  不是本域的 HTTP 端点），口径见 [AGENTS.md §5.5](../../../../../AGENTS.md)「分发」行与
  [Callback README](../../../Mud.Wechat.Work.Callback/README.md)。本域接口层不承载任何回调事件，勿在此处加事件 DTO。

## 3. 定位键与 `order` 的语义反转（本域最易踩的坑）

四个定位键的分工与长度约束（[UserInfo.cs](../../../Mud.Wechat.Work/Contacts/Users/UserInfo.cs)、
[CreateUserRequest.cs](../../../Mud.Wechat.Work/Contacts/Users/RequestModel/CreateUserRequest.cs)、
[UserDepartmentInfo.cs](../../../Mud.Wechat.Work/Contacts/Users/UserDepartmentInfo.cs)）：

| 键 | JSON 名 | 口径 |
|---|---|---|
| 成员账号 | `userid` | 企业内唯一、1~64 字节、不区分大小写；create 时仅数字/字母及 `_-@.`、首字符须数字或字母 |
| 全局标识 | `open_userid` | 第三方调用 `user/get` 时**在 `userid` 字段位返回 open_userid**；同一服务商下跨应用同值、最长 64 字节；`list_id` 对第三方 / 代开发返回 open_userid |
| 部门 | `department` | `List<int>`；成员授权模式下固定返回根部门 1 |
| 部门定位 | `id` | 32 位整型；见 §5 根部门与 create 约束 |

**`order` 在成员侧与部门侧语义相反**——这是唯一「照名字推断必错」的字段：

- 成员 `order`（`List<long>`，与 `department` 一一对应）：**数值越大越靠前**，官方范围 `[0, 2^32)` 故以 64 位整数承载，成员授权模式下不返回。
- 部门 `order`（[DepartmentInfo.cs](../../../Mud.Wechat.Work/Contacts/Department/DepartmentInfo.cs)、
  [DepartmentIdInfo.cs](../../../Mud.Wechat.Work/Contacts/Department/DepartmentIdInfo.cs)、
  [CreateDepartmentRequest.cs](../../../Mud.Wechat.Work/Contacts/Department/RequestModel/CreateDepartmentRequest.cs)）：
  **数值越小越靠前**，与成员相反；同样 `[0, 2^32)`、`long?`。

⇒ 跨成员 / 部门复用同一个「排序值」比较器或「取最大 / 最小即首位」的封装会静默错序，二者必须分别实现。
（`hand_join` 与部门 `parentname` 两键**未在本仓接口 / DTO / 守卫出现**，本域不建模，见文末缺口。）

## 4. `user/get` / `user/simplelist` / `user/list_id` 的信息面差异与「可见范围」

三支读取端点返回的字段集与「能看到谁」并不相同，接口注释留档的口径（[IWechatWorkUsersService.cs](./IWechatWorkUsersService.cs)）：

- **信息面递减**：`user/get`（#1，完整 `UserInfo`）＞ `user/list`（#3，同 `user/get` 字段集的批量版）＞
  `user/simplelist`（#2，仅 userid/name/department/open_userid）＞ `user/list_id`（#4，仅 userid↔department 关系）。
- **可见范围**：`user/get` / `#2` / `#3` 只能取到 token 对应应用「可见范围内」的成员 / 部门；`#4 list_id` 是 2022-08
  通讯录安全加固后官方推荐的替代接口，自建须以通讯录同步 secret 调用（返回 `userid`）、第三方须通讯录编辑授权（返回 `open_userid`）。
- **敏感字段停返时间线**（接口注释，均落 `UserInfo` 的可空字段而非删除字段）：自 **2022-06-20** 起新建的自建 / 代开发应用
  不再返回头像 / 性别 / 手机 / 邮箱 / 企业邮箱 / 个人二维码 / 地址（需 oauth2 手工授权）；自 **2022-08-15** 起
  「通讯录同步」新增 IP 不能再调用 `user/get`（#1）与 `department/list`（#19），官方建议改用 `user/list_id` / `department/simplelist`。
  第三方调用 `#2` 时 `name` 自 2019-12-30 起以 userid 代替、`#19` 不可获取 `name` / `name_en`。
- **`gender` 形态陷阱**：官方参数表标注整数、但请求 / 响应示例按字符串传输（`"1"` / `"2"`），故 `UserInfo.Gender` /
  `CreateUserRequest.Gender` 均为 `string?`，勿改回 `int`。

## 5. 写入的原子性差异：`department/create` 单次一个 vs 批量接口的部分成功

本域「一次能不能塞多条、失败会不会带成功」在不同端点上口径不同，SDK **不做**本地拆分或补偿，越界由 `errcode` 表达：

| 端点 | 批量能力 | 失败语义 |
|---|---|---|
| `department/create`（#22） | **单次一个部门** | 无部分成功；层级 ≤15、部门总数 ≤3 万，建议与创建成员串行 |
| `user/create`（#9） | 单成员 | 填了不存在的部门 ⇒ **官方自动新建**，结果回显在 [CreateUserResponse.CreatedDepartmentList](../../../Mud.Wechat.Work/Contacts/Users/CreateUserResponse.cs)（`created_department_list`）；部门 ≤100 个、每部门成员 ≤3 万 |
| `user/batchdelete`（#12） | ≤200 userid | **存在无效 userid 即整批失败**，无部分删除 |
| `tag/addtagusers` / `deltagusers`（#42/#43） | userlist≤1000、partylist≤100 | **三态**：全部合法 / 部分非法（仍 `errcode=0` 附非法列表）/ 全部非法（报错），见 §9 |
| `contactrule/*`（#29/#31/#32） | ≤100 | 见 §8 |

⇒ 「整批失败」与「部分成功」不可混为一谈：批量删除成员用整批失败、批量增删标签成员用部分成功，
封装时不要把两者的返回值判空逻辑写成同一套。

## 6. 异步导入族（`batch/`）：csv 前置 + 两段式 + 危险操作阈值

- **两段式**：#25/#26/#27 只返回受理结果 `JobId`（[BatchJobResponse](../../../Mud.Wechat.Work/Contacts/Batch/BatchJobResponse.cs)），
  必须轮询 #28 `GetBatchJobResultAsync(jobid)` 拿 [GetBatchJobResultResponse](../../../Mud.Wechat.Work/Contacts/Batch/GetBatchJobResultResponse.cs)：
  `status` **1 开始 / 2 进行中 / 3 完成**、`type` 取值 `sync_user` / `replace_user` / `replace_party` / `invite_user`、
  `percentage`、`remaintime`（分钟）、完成后 `result`（逐条明细，形状按 `type` 区分）。#28 **只能查已提交过的历史任务**。
- **csv 前置**：`media_id` 须先经素材接口上传取得（[BatchImportUsersRequest](../../../Mud.Wechat.Work/Contacts/Batch/RequestModel/BatchImportUsersRequest.cs)）；
  `to_invite` 官方默认 `true`（不填不出网、由官方按默认处理）。
- **全量覆盖成员（#26）是危险操作**：文件中不存在、通讯录中存在的成员将被删除；官方中止阈值为「需删除成员 >50 人且 >现有人数 20%，
  或 <50 人且 >现有人数 80%」时中止并返回错误码。全量覆盖部门（#27）对「仍有成员 / 子部门的部门」暂缓删除。
- **路由拼写**：`batch/getresult`（#28，**无下划线**）与导出族 `export/get_result`（#37，**有下划线**）、上下游 `corpgroup/getresult` 三种拼写各异，不得「统一命名」顺手改写（U1 / B1 / E1 各自锁定）。

## 7. 异步导出族（`export/`）：任务 + 加密结果文件的两段式

- 与导入族同为「提交任务取 `jobid` ⇒ 轮询取结果文件」两段式，但**终态语义是文件而非逐条明细**：
  #37 返回 [GetExportResultResponse](../../../Mud.Wechat.Work/Contacts/Export/GetExportResultResponse.cs) 的 `status`
  **0 未处理 / 1 处理中 / 2 完成 / 3 异常失败**（注意：与导入 #28 的 1/2/3 三态**不是同一状态机**，0 起点、且多了失败态）
  与 `data_list[]`（[ExportDataFile](../../../Mud.Wechat.Work/Contacts/Export/ExportDataFile.cs)：`url` / `size` / `md5`）。
- **数据文件是密文**：提交任务时传入 `encoding_aeskey`（[ExportRequest](../../../Mud.Wechat.Work/Contacts/Export/RequestModel/ExportRequest.cs) 必填，
  Base64 定长 43 字符，**敏感凭据、不得进日志**），导出文件按 AES-256-CBC 加密（`AESKey = Base64_Decode(encoding_aeskey + "=")`、
  IV 取前 16 字节、PKCS#7 填充），**解密由调用方自行完成**，本 SDK 不提供解密。`block_size` 支持 `[10^4, 10^6]`、默认 `10^6`。
- **下载链接有效期 2 小时、支持 Range 头分段下载**；调用 `#37` 的身份须与提交任务的一致，否则取不到文件。
- **完成通知是回调事件、不是 HTTP 端点**：官方 `batch_job_result`（`JobType = export_simple_user / export_user / export_department / export_tag`）
  推送至提交任务的回调地址 ⇒ 属 `Callback` 域（见 §2 跨域提示），本域 E1 路由表不含它、也不在此建模。
- **接口注释未出现 `sync` / `template_ids` 字段**：本域导出请求体只有 `encoding_aeskey` / `block_size`（标签导出另加 `tagid`），
  二者在本仓证据中不存在，见文末缺口。

## 8. 通讯录查看权限族（`contactrule/`）：条件适用字段 + 全 POST

「通讯录隐藏规则」4 支（101536/101537/101539/101540）**仅自建、且仅通讯录同步应用可调用**，
`rules[]` 一次最多 100 条（[CreateContactRulesRequest](../../../Mud.Wechat.Work/Contacts/ContactRules/RequestModel/CreateContactRulesRequest.cs)）。
频控（接口注释）：**创建 / 修改 / 删除共用 1 分钟 5 次、1 小时 10 次、1 天 60 次**；#30 读取列表注释无频率数值。

规则体 [ContactRule](../../../Mud.Wechat.Work/Contacts/ContactRules/ContactRule.cs) 的字段适用性随 `rule_type` 变（本仓不本地拦截，错配由 `errcode` 表达）：

| 字段 | 口径 |
|---|---|
| `rule_type` | **1 隐藏部门/成员、2 限制查看外部门、3 限制查看所有人**；**修改时不能更新规则类型** |
| `rule_id` | `int?`；创建时不填（官方生成，返回的 `rule_ids` 与请求顺序对应）、修改 / 删除时必填 |
| `range` / `whitelist` | 目标范围与白名单（[ContactRuleRange](../../../Mud.Wechat.Work/Contacts/ContactRules/ContactRuleRange.cs)） |
| `exclude` / `is_allowed_search` / `is_allowed_conversation` | **仅「限制查看外部门 / 所有人」（type 2/3）时使用**，type 1 无此三项语义 |

⇒ 本域唯一的「读取走 POST」端点在这里：**#30 `contactrule/list` 是 POST 且无请求体参数**（`GetContactRulesAsync(CancellationToken)`，CR1 断言 `[Post]`），
不要因「是查询就改成 GET」。

## 9. 标签族：归属 `Tags` 目录 + 响应形态陷阱

- 标签管理全部 7 支端点官方对三类应用一致开放 ⇒ 全落 [IWechatWorkTagsService.cs](./IWechatWorkTagsService.cs)（父接口）、三支子接口空标记（T2）；
  DTO 落 [Contacts/Tags/](../../../Mud.Wechat.Work/Contacts/Tags/) 子目录、命名空间 `...DataModels.Contacts.Tags`。
- **增删标签成员是三态返回**（[ChangeTagMembersResponse](../../../Mud.Wechat.Work/Contacts/Tags/ChangeTagMembersResponse.cs)）：
  全部合法、部分非法（仍 `errcode=0`，附非法清单）、全部非法（返回错误码）。两支共用同一响应体。
- **响应形态陷阱**（与消息推送域同类的坑）：非法成员 `invalidlist` 是**竖线 `\|` 分隔的字符串**（如 `usr1|usr2`）、
  非法部门 `invalidparty` 是**数组** ⇒ 二者不共用、不可都按数组解析。
- **「全部非法」错误码两文档树标注不一致**：自建增成员文档标 40070、第三方 / 代开发删成员文档标 40031 ⇒ 以实际返回为准，勿写死判据。

## 10. 字段名照官方原文大小写与连写 + 目录 / 命名空间既存不一致（沿用勿修正）

- **本域大量端点是「查询却走 POST」**（U1/D1/CR1/E1/T1 的 `typeof(PostAttribute)` 行）：`#4 list_id`、`#5 getuserid`、
  `#6 get_userid_by_email`、`#7/#8 convert_to_*`、`#16/#17/#18` 成员授权三支、`#25/#26/#27` 导入三支、`#30` 读取规则列表、
  `#33/#34/#35/#36` 导出四支、`#38/#39/#42/#43` 标签四支——一律以守卫登记的动词为准。
- **路由名照官方原文、勿「规范化」**：`simplelist`（#2/#20）、`batchdelete`（#12）、`authsucc`（#14）、`getuserid`（#5，全连写）、
  `addtagusers` / `deltagusers`（#42/#43）、`contactrule/*`（#29–#32，单数连写）、`get_result` vs `getresult`（§6）。
  这些拼写差异均由各守卫的 `RequestUri.Should().Be(...)` 逐条钉死。

- **目录 / 命名空间 / 模块三处不一致是本域既存形态，新增沿用、勿「顺手对齐」**（[AGENTS.md §4](../../../../../AGENTS.md) 不一致表已登记）：
  1. **接口目录 `Interfaces/Contacts/` 无子目录**——六子族接口文件全部平铺在 `Contacts/` 根下（连 Export 服务也在根），
     而 **DTO 有子目录** `Contacts/{Users,Batch,ContactRules,Department,Export,Tags}/`。
  2. **`Contacts/Users/` 的命名空间独插 `Contracts` 段**：成员域 DTO 物理在 `DataModels/Contacts/Users/`、
     命名空间却是 `Mud.Wechat.Work.DataModels.Contracts.Users`（如 [UserInfo.cs](../../../Mud.Wechat.Work/Contacts/Users/UserInfo.cs) 首行 `namespace ...DataModels.Contracts.Users;`），
     是全仓**唯一**插入 `Contracts` 段的域；其余五子族命名空间为 `...DataModels.Contacts.{子族}`。
  3. **模块 / 注册入口为单数 `Contact`**：枚举 `WechatModule.Contact`、`AddContactApi()`、注册组名 `Contact`，
     而接口目录与 DTO 目录是复数 `Contacts`。
  以上三点由 `AGENTS.md` §4 明文列为「既存不一致、新增沿用」，改动任一处都会牵动 `UsersJsonContext` 登记（U4）、
  N1~N3 分区与 [WechatModule.cs](../../Extensions/WechatModule.cs) / [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)，
  **不得**借本 README 之机「顺手统一」。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册通讯录模块（六子族共用 Contact 组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

// 自建应用：注入自建子接口（公共父接口 IsAbstract，不可注入；写入须通讯录同步助手 secret 换的令牌）
public sealed class DirectorySync(
    IWechatWorkInternalUsersService users,
    IWechatWorkInternalDepartmentsService departments,
    IWechatWorkInternalContactRulesService contactRules,
    IWechatWorkInternalExportService export)
{
    // 1) 游标分页拉取「成员 ID 列表」（2022-08 加固后的官方推荐接口；查询走 POST）
    public async Task PullAllUserIdsAsync(CancellationToken ct)
    {
        string? cursor = null;
        while (true)
        {
            var resp = await users.ListUserIdsAsync(
                new ListUserIdsRequest { Cursor = cursor, Limit = 10000 }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/user/list_id");

            List<UserDepartmentInfo> items =
                resp.DeptUser ?? new List<UserDepartmentInfo>();   // 勿写 ?? []
            foreach (UserDepartmentInfo rel in items)
            {
                // 自建返回 userid、第三方/代开发返回 open_userid：
                _ = rel.UserId ?? rel.OpenUserId;                   // rel.Department 为 int?
            }

            if (string.IsNullOrEmpty(resp.NextCursor)) break;       // 空游标 = 已无更多
            cursor = resp.NextCursor;
        }
    }

    // 2) 创建部门（单次一个；order 值小靠前）→ 创建成员（order 值大靠前，二者相反）
    public async Task CreateOrgUnitAsync(CancellationToken ct)
    {
        var dept = await departments.CreateDepartmentAsync(new CreateDepartmentRequest
        {
            Name = "示例部门",            // 1~64 个 UTF-8 字符，同层唯一
            ParentId = 1,                 // 根部门 id 恒为 1
            Order = 100,                  // 部门 order：值小靠前
        }, ct);
        WechatWorkException.ThrowIfFailed(dept, "/cgi-bin/department/create");

        var created = await users.CreateUserAsync(new CreateUserRequest
        {
            UserId = "zhangsan",          // 假值；企业内唯一、1~64 字节
            Name = "张三",                 // 假值
            Department = new List<int> { dept.Id },
            Order = new List<long> { 1 }, // 成员 order：值大靠前（与部门相反）
            Mobile = "13800000000",       // 假值；mobile/email 不能同时为空；凭据敏感
        }, ct);
        WechatWorkException.ThrowIfFailed(created, "/cgi-bin/user/create");
        // 若填了不存在的部门，官方自动新建并回显在 created.CreatedDepartmentList
    }

    // 3) 创建通讯录隐藏规则（仅通讯录同步应用；一次 ≤100 条；type 2/3 才用 exclude/is_allowed_*）
    public async Task HideExternalAsync(CancellationToken ct)
    {
        var resp = await contactRules.CreateContactRulesAsync(new CreateContactRulesRequest
        {
            Rules = new List<ContactRule>
            {
                new ContactRule { RuleType = 2 },   // 2 限制查看外部门；RuleId 不填
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/contactrule/create");
        // resp.RuleIds 与请求规则顺序对应
    }

    // 4) 异步导出成员：提交任务（传 encoding_aeskey）→ 轮询 get_result 取加密文件（SDK 不解密）
    public async Task<string> ExportMembersAsync(string base64AesKey43, CancellationToken ct)
    {
        var job = await export.ExportUsersAsync(new ExportRequest
        {
            EncodingAesKey = base64AesKey43,   // 敏感：定长 43、勿写进日志
            BlockSize = 1000000,
        }, ct);
        WechatWorkException.ThrowIfFailed(job, "/cgi-bin/export/simple_user");

        var result = await export.GetExportResultAsync(job.JobId!, ct);   // 生产应轮询至 status==2
        WechatWorkException.ThrowIfFailed(result, "/cgi-bin/export/get_result");
        // result.Status: 0 未处理 / 1 处理中 / 2 完成 / 3 异常失败；result.DataList[].Url 2 小时有效、可 Range 分段
        return job.JobId!;
    }
}

// 第三方 / 代开发：先切到授权企业作用域，再调用（令牌解析为该 authCorpId 的企业级 access_token）
using (switcher.UseCorpScope(appKey: "default", authCorpId, permanentCode))
{
    var user = await thirdPartyUsers.GetUserAsync("zhangsan", ct);   // 第三方在 userid 字段位返回 open_userid
    WechatWorkException.ThrowIfFailed(user, "/cgi-bin/user/get");
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试；
异步任务（导入 / 导出）的「是否完成」由调用方轮询 `status`，SDK 不做隐式重试或本地批次裁剪。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `hand_join` / 部门 `parentname` | 二者在本域接口文件、DTO、六支守卫的三类证据中**均未出现**，本仓不建模 ⇒ 是否为官方 create/update 报文可选字段**未逐项核验** | 需要时逐页核验官方文档，再按 §2「加 DTO + 同批补守卫」落地，不得凭字段名推断约束 |
| 导出族的 `sync` / `template_ids` | [ExportRequest](../../../Mud.Wechat.Work/Contacts/Export/RequestModel/ExportRequest.cs) 只有 `encoding_aeskey` / `block_size`、标签导出加 `tagid`；`sync` / `template_ids` **未在本仓出现** ⇒ 属邻域字段或不存在，**未逐项核验** | 不得从邻域（如上下游导入、审批模板）借字段塞进导出请求体 |
| 除 `contactrule/*`（5/10/60）与 `replaceuser` 危险阈值外，各端点的数值 QPS | 接口注释只在通讯录查看权限四支留档了频率；成员 / 部门 / 标签 / 导入 / 导出的逐端点频率上限**未逐项核验** | 需要限流时先核验官方页，再补接口 remarks |
| `GetBatchJobResultAsync` 的 `result` 逐条明细形状 | [GetBatchJobResultResponse](../../../Mud.Wechat.Work/Contacts/Batch/GetBatchJobResultResponse.cs) 注释说明形状按 `type` 区分、元素落 `BatchTaskResultItem`，但各 `type` 的完整字段集**未逐项核验** | 解析明细前先核验 [BatchTaskResultItem.cs](../../../Mud.Wechat.Work/Contacts/Batch/BatchTaskResultItem.cs) 与官方页 |
| 「异步导入」是否含 `invite_user` 的提交端点 | #28 的 `type` 枚举含 `invite_user`，但本域邀请端点 #13 `batch/invite` 是同步接口；`invite_user` 任务由哪个提交端点产生**未在本域核验** | 需要异步邀请时查邻域 / 官方页，勿在成员域加提交端点 |
| `contactrule/list` 的频率与响应 `rules` 全字段 | #30 注释仅「POST、无请求体」；其频率数值、以及 `GetContactRulesResponse` 回显字段是否等同 `ContactRule` 全集**未逐项核验** | 需要缓存或校验回显时先核验 [GetContactRulesResponse.cs](../../../Mud.Wechat.Work/Contacts/ContactRules/GetContactRulesResponse.cs) |
| 代开发能否调用 `list_id` 及返回 open_userid 的确切口径 | 代开发子接口是空标记、继承父接口读取面（#4 在其内）；但接口注释对 `list_id` 只描述了自建 / 第三方两种 secret 与返回 | 以官方代开发文档为准，勿据自建口径断言代开发行为 |
| `UserInfo.enable` / `wxplugin_status` / `english_name` | DTO 注释标注「官方现行响应参数表未单列，为兼容既有部署与 Senparc 契约保留」⇒ 是否仍在实际响应中返回**未逐项核验** | 判空使用，勿当作必然存在 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（长度、≤100/≤200/≤1000、频率 5/10/60、停返时间线、危险阈值、AES/Range/2h） | 本目录 21 个接口文件的 XML 注释 |
| 成员族路由 / 计数（23 声明 / 18 去重）、代开发空标记、层级与 Contact 注册组 | [WechatUsersContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatUsersContractGuards.cs) U1（Routes 表）、U2、U3、U4 |
| 部门族路由 / 计数（9 / 6）、`order` 语义、自动新建部门 | [WechatDepartmentsContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDepartmentsContractGuards.cs) D1~D4 |
| 通讯录限制族路由 / 计数（4，全 POST）、父接口零端点、仅自建 | [WechatContactRulesContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContactRulesContractGuards.cs) CR1~CR4 |
| 导出族路由 / 计数（5）、`get_result` 下划线告警、三类一致 | [WechatExportContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExportContractGuards.cs) E1~E4 |
| 导入族路由 / 计数（4）、无代开发子接口 | [WechatBatchContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBatchContractGuards.cs) B1~B4 |
| 标签族路由 / 计数（7）、三类一致、响应形态 | [WechatTagsContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTagsContractGuards.cs) T1~T4 |
| 令牌 Query 注入白名单（G5）与凭据脱敏 | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[AGENTS.md §5.1/§8](../../../../../AGENTS.md) |
| 令牌归属域（TO1~TO3）与接口命名空间二分（N1~N3） | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)、[WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [DataModels/Contacts/Users/](../../../Mud.Wechat.Work/Contacts/Users/)（命名空间 `...Contracts.Users`）、[Contacts/Batch/](../../../Mud.Wechat.Work/Contacts/Batch/)、[Contacts/ContactRules/](../../../Mud.Wechat.Work/Contacts/ContactRules/)、[Contacts/Department/](../../../Mud.Wechat.Work/Contacts/Department/)、[Contacts/Export/](../../../Mud.Wechat.Work/Contacts/Export/)、[Contacts/Tags/](../../../Mud.Wechat.Work/Contacts/Tags/) |
| 各子族 JSON 上下文（源生成物，勿手改） | [UsersJsonContext.g.cs](../../../Mud.Wechat.Work/Generated/UsersJsonContext.g.cs)、DepartmentJsonContext / BatchJsonContext / ContactRulesJsonContext / ExportJsonContext / TagsJsonContext |
| 模块三段式（枚举 + `AddContactApi()` + 源生成 `AddContactWebApiHttpClient()`） | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) |
| 通讯录变更事件回调（通配键 `"*"`、`batch_job_result`） | 跨 `Callback` 域：[Callback README](../../../Mud.Wechat.Work.Callback/README.md)、[AGENTS.md §5.5](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方文档号与业务限制（频率 / 串行 / 覆盖删除 / 可见范围），**同批**改对应子族守卫的
   `Routes` 表与 `HaveCount`（成员 23/18、部门 9/6、导入 4、通讯录限制 4、导出 5、标签 7）+ `Distinct` 双断言；
   改开放面须同时改层级守卫（U2/D2/B2/CR2/E2/T2）的「子接口 `DeclaredOnly` 是否为空」断言与代开发子接口取舍。
2. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`，`SerializerClassName` 取所在子族：`Users`（物理在 `Contacts/Users/`、命名空间 `Contracts.Users`）、
   `Department` / `Batch` / `ContactRules` / `Export` / `Tags` → 跑
   [`../../../../../scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) 与
   [`../../../../../scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1)（生成物勿手改）→
   同批更新各守卫 `DataModels_ShouldBeRegisteredInJsonContext`（U4/D4/B4/CR4/E4/T4）的类型清单。
3. **新增 / 迁移 `[HttpClientApi]` 接口** → 命名空间按 `IsAbstract` 二分（父 → `Mud.Wechat.Work.Interfaces`、可注入 → `Mud.Wechat.Work`），
   子接口挂 `RegistryGroupName = "Contact"`、`InheritedFrom = nameof(父实现类)`；跑
   [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3）并同批调整其 147/299 计数。
4. **令牌面改动** → 子接口 `TokenManagerKey`（自建 `InternalAccessToken` / 第三方与代开发 `CorpAccessToken`）由 TO1~TO3 反射锁定；
   新增 Query 凭据参数名须同批登记 G5 白名单与脱敏词表（`corpsecret` / `permanent_code` / `media_id` 关联凭据只写参数名，不进日志 / 遥测 / 异常消息）。
5. **本 README 与线索引** → 端点数、`AddContactApi()` 的「六域」叙述、
   [线 README](../../README.md) 的 `Interfaces/Contacts/` 条目须与本域实测同步；AGENTS.md §4 的「目录 / 命名空间 / 模块不一致表」若因此变动须同批改。
6. 最后跑 [`../../../../../scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与
   [`../../../../../scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成（本次文档撰写未执行门禁）。
