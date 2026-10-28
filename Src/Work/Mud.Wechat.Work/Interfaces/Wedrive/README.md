# Wedrive 域 · 微盘（云盘）

企业微信「微盘」域：本仓共落地 **31 支端点**（守卫 [WR1](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs) 的 `allRoutes` 恰 31），分属六个接口族——
管理空间族 **4 支**（[IWechatWorkWedriveSpaceService.cs](./IWechatWorkWedriveSpaceService.cs)）、
管理空间权限族 **5 支**（[IWechatWorkWedriveSpaceAclService.cs](./IWechatWorkWedriveSpaceAclService.cs)）、
管理文件族 **11 支**（[IWechatWorkWedriveFileService.cs](./IWechatWorkWedriveFileService.cs)：9 个官方文档页承载 11 条路由，分块上传一页三路由）、
管理文件权限族 **6 支**（[IWechatWorkWedriveFileAclService.cs](./IWechatWorkWedriveFileAclService.cs)）、
版本和容量管理族 **2 支**（[IWechatWorkWedriveCapacityService.cs](./IWechatWorkWedriveCapacityService.cs)：官方单页两路由、空请求体）、
高级功能账号管理族 **3 支**（零端点父接口 [IWechatWorkWedriveVipService.cs](./IWechatWorkWedriveVipService.cs) +
唯一自建子接口 [IWechatWorkWedriveVipService_Internal.cs](./IWechatWorkWedriveVipService_Internal.cs)，官方仅自建应用开放）。
接口文件共 **22 份** = 6 个公共父接口 + 16 个应用类型子接口（五族各 Internal/Provider/ThirdParty 空标记 + vip 族仅 Internal），
与守卫 WR3 的 `HaveCount(22)` 一一对应。
注册组 `Wedrive`，模块枚举 `WechatModule.Wedrive`（[`AddWedriveApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 22 份接口文件的 XML 注释、
> [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1~WR5）、
> [Src/Work/Mud.Wechat.Work/Wedrive/](../../../Mud.Wechat.Work/Wedrive/) 下的 60 个 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌列写本仓真实契约：`access_token` 恒为 Query 注入、`TokenType` 恒 `Wechat.AccessToken`
（WR3 对全部 22 个接口逐个断言 `InjectionMode.Query` + `Name = "access_token"`，MUD005 已接受风险，无法改 Header）；
「开放面」指官方对该端点开放的应用类型，由 WR2 的层级断言锁死。
**31 条路由官方全部即 POST**，含 9 支纯查询语义端点（WR1 注释明示「勿顺手统一为 GET」）。

### 管理空间族（WR1 `WedriveSpaceRoutes`，4 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateSpaceAsync` | POST | `/cgi-bin/wedrive/space_create` | `access_token`(Query)；自建 + 第三方 + 代开发 | 93655/95857/96845；**创建者是应用本身**；`auth = 7` 最多 3 个且不支持部门 |
| 2 | `RenameSpaceAsync` | POST | `/cgi-bin/wedrive/space_rename` | `access_token`(Query)；三类 | 97856/97872/97862；无业务负载 |
| 3 | `DismissSpaceAsync` | POST | `/cgi-bin/wedrive/space_dismiss` | `access_token`(Query)；三类 | 97857/97873/97863；不可逆，解散后空间内文件不可访问 |
| 4 | `GetSpaceInfoAsync` | POST | `/cgi-bin/wedrive/space_info` | `access_token`(Query)；三类 | 97858/97874/97864；**旧版**获取空间信息，官方挂「管理空间」分组 |

### 管理空间权限族（WR1 `WedriveSpaceAclRoutes`，5 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 5 | `AddSpaceAclAsync` | POST | `/cgi-bin/wedrive/space_acl_add` | `access_token`(Query)；三类 | 93656/95858/96846；增量添加，可一次性多个；无业务负载 |
| 6 | `DelSpaceAclAsync` | POST | `/cgi-bin/wedrive/space_acl_del` | `access_token`(Query)；三类 | 97875/97947/97910；**官方请求形态不携带 `auth`** |
| 7 | `SetSpaceSettingAsync` | POST | `/cgi-bin/wedrive/space_setting` | `access_token`(Query)；三类 | 97876/97948/97911；**仅支持设置由本应用创建的空间** |
| 8 | `GetSpaceShareUrlAsync` | POST | `/cgi-bin/wedrive/space_share` | `access_token`(Query)；三类 | 97877/97949/97912；返回 `space_share_url` |
| 9 | `GetNewSpaceInfoAsync` | POST | `/cgi-bin/wedrive/new_space_info` | `access_token`(Query)；三类 | 97878/97950/97913；**新版**，官方归入本「管理空间权限」分组 |

### 管理文件族（WR1 `WedriveFileRoutes`，11 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 10 | `GetFileListAsync` | POST | `/cgi-bin/wedrive/file_list` | `access_token`(Query)；三类 | 93657/95859/96847；`start` + `limit` 分页，`limit` ≤ 1000 |
| 11 | `UploadFileAsync` | POST | `/cgi-bin/wedrive/file_upload` | `access_token`(Query)；三类 | 97880/97951/97914；整文件 base64，上限 **10M** |
| 12 | `InitFileUploadAsync` | POST | `/cgi-bin/wedrive/file_upload_init` | `access_token`(Query)；三类 | 98004/98005/98007（同页三支）；`size` ≤ 20G，命中秒传直接返回 `fileid` |
| 13 | `UploadFilePartAsync` | POST | `/cgi-bin/wedrive/file_upload_part` | `access_token`(Query)；三类 | 98004/98005/98007；2M 固定分块、`index` 从 1 开始、建议并发 ≤ 10 |
| 14 | `FinishFileUploadAsync` | POST | `/cgi-bin/wedrive/file_upload_finish` | `access_token`(Query)；三类 | 98004/98005/98007；仅凭 `upload_key` 换 `fileid` |
| 15 | `DownloadFileAsync` | POST | `/cgi-bin/wedrive/file_download` | `access_token`(Query)；三类 | 97881/97953/97915；返回 `download_url`（2 小时）+ Cookie；**仅普通文件** |
| 16 | `CreateFileAsync` | POST | `/cgi-bin/wedrive/file_create` | `access_token`(Query)；三类 | 97882/97954/97916；`file_type` 1 文件夹 / 3 文档 / 4 表格 |
| 17 | `RenameFileAsync` | POST | `/cgi-bin/wedrive/file_rename` | `access_token`(Query)；三类 | 97883/97955/97917；响应字段官方名作 `file`（非 `file_info`） |
| 18 | `MoveFileAsync` | POST | `/cgi-bin/wedrive/file_move` | `access_token`(Query)；三类 | 97884/97956/97918；`fileid` 为**数组**、支持批量 |
| 19 | `DeleteFileAsync` | POST | `/cgi-bin/wedrive/file_delete` | `access_token`(Query)；三类 | 97885/97957/97919；`fileid` 为**数组**、支持批量 |
| 20 | `GetFileInfoAsync` | POST | `/cgi-bin/wedrive/file_info` | `access_token`(Query)；三类 | 97886/97958/97920；响应字段官方名作 `file_info` |

### 管理文件权限族（WR1 `WedriveFileAclRoutes`，6 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 21 | `AddFileAclAsync` | POST | `/cgi-bin/wedrive/file_acl_add` | `access_token`(Query)；三类 | 93658/95860/96848；文件侧 `auth` **仅 1** |
| 22 | `DelFileAclAsync` | POST | `/cgi-bin/wedrive/file_acl_del` | `access_token`(Query)；三类 | 97888/97959/97922；`type` / `departmentid` 官方标注「后续将废弃」 |
| 23 | `SetFileSettingAsync` | POST | `/cgi-bin/wedrive/file_setting` | `access_token`(Query)；三类 | 97889/97960/97923；`auth_scope` 1~5，4/5 仅有管理员时可设 |
| 24 | `GetFileShareUrlAsync` | POST | `/cgi-bin/wedrive/file_share` | `access_token`(Query)；三类 | 97890/97961/97924；返回 `share_url`（区别于 #8 的 `space_share_url`） |
| 25 | `GetFilePermissionAsync` | POST | `/cgi-bin/wedrive/get_file_permission` | `access_token`(Query)；三类 | 97891/97962/97925；**路由不走 `wedrive/file_` 前缀** |
| 26 | `SetFileSecureSettingAsync` | POST | `/cgi-bin/wedrive/file_secure_setting` | `access_token`(Query)；三类 | 97892/97965/97926；**仅支持在线文档类型** |

### 版本和容量管理族（WR1 `WedriveCapacityRoutes`，2 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 27 | `GetProInfoAsync` | POST | `/cgi-bin/wedrive/mng_pro_info` | `access_token`(Query)；三类 | 95856/95861/96849（同页两支）；**无请求 DTO**，唯一参数是 `CancellationToken` |
| 28 | `GetCapacityInfoAsync` | POST | `/cgi-bin/wedrive/mng_capacity` | `access_token`(Query)；三类 | 95856/95861/96849；容量字段单位 **B** |

### 高级功能账号管理族（WR1 `WedriveVipRoutes`，3 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 29 | `BatchAddVipAsync` | POST | `/cgi-bin/wedrive/vip/batch_add` | `access_token`(Query)；**仅自建** | 99512；`userid_list` 单次 ≤ 100 |
| 30 | `BatchDelVipAsync` | POST | `/cgi-bin/wedrive/vip/batch_del` | `access_token`(Query)；**仅自建** | 99513；单次 ≤ 100 |
| 31 | `ListVipAsync` | POST | `/cgi-bin/wedrive/vip/list` | `access_token`(Query)；**仅自建** | 99514；`cursor` + `limit` 分页（`limit` 默认 100、最大 200） |

## 1. 两级定位键：`spaceid` + `fatherid` / `fileid`，且拼写无下划线

微盘的一切定位是**两级**的：先有空间 `spaceid`，再在空间内用目录/文件的 `fileid`。三条必须照抄的形态（WR5 逐个断言）：

| 键 | JSON 名 | 语义与陷阱 |
|---|---|---|
| 空间 ID | `spaceid`（**无下划线**） | 出现在 11 个类型上（WR5 逐一 `JsonNameShouldBe`），包括响应侧 [WedriveSpaceInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceInfo.cs) |
| 文件 ID | `fileid`（**无下划线**） | 单数拼写；移动/删除端点里 `fileid` 是**数组形态**（`List<string>`，WR5 断言「单数名复数承载，照抄勿改」） |
| 父目录 | `fatherid`（**无下划线**） | 接口注释留档：`fatherid` 为当前目录的 `fileid`，**根目录时填空间 `spaceid`**（#10/#16/#18 与 [WedriveFileInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileInfo.cs) 注释同口径） |

⇒ 「根目录」没有独立 id，就是 `spaceid` 本身；不要把 `fatherid` 当可缺省字段（#10 官方必填）。
另有一组定位键不属于 id 体系而是**凭据**：`selected_ticket`（微盘和文件选择器 jsapi 返回值），见 §5。

`space_name` 与空间类型：本仓三类证据只留档 [CreateWedriveSpaceRequest.cs](../../../Mud.Wechat.Work/Wedrive/CreateWedriveSpaceRequest.cs)
的 `space_sub_type`（`ulong?`）= **0 普通空间**（注释「目前只支持 0」），响应侧 [WedriveSpaceInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceInfo.cs)
与 [WedriveNewSpaceInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveNewSpaceInfo.cs) 同带 `space_sub_type`（注释「0 - 普通空间」）。
**共享空间 / 群组空间 / 个人空间等其它空间形态在本域三类证据内没有任何留档**（无取值、无枚举、无端点）⇒ 见文末缺口，不得据名称推断。

## 2. 一份成员结构承载五处权限面，`auth` 取值集合按端点而变

[WedriveSpaceAclMember.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceAclMember.cs) 是本域权限模型的唯一载体
（WR5 断言五处请求/响应属性类型恒为 `List<WedriveSpaceAclMember>`）：
`space_create.auth_info`、`space_acl_add.auth_info`、`space_acl_del.auth_info`、`file_acl_add.auth_info`、`file_acl_del.auth_info`，
以及读取侧 `auth_list.auth_info`、`file_member_list`、`co_auth_list`、`inherit_father_auth.auth_list` / `member_list`。
**文件权限成员复用空间成员结构，不另建类型**（WR4 计数注释明示「文件权限成员复用空间成员结构不另建类型」）。

同一个 `auth` 键（`ulong?`）在不同端点的合法集合**并不相同**，这是本域最容易写错的一处：

| 端点 / 键 | 本仓留档取值 | 附加约束（注释留档） |
|---|---|---|
| #1 `space_create` → `auth_info[].auth` | 1 仅下载 / 4 可预览 / 7 应用空间管理员 | 4 **仅专业版微盘企业可设置**；7 最多 3 个且**不支持部门** |
| #5 `space_acl_add` → `auth_info[].auth` | 同上（1 / 7） | 注释口径「**连同已设置的管理员**最多可指定 3 个，且不支持将部门设为管理员」 |
| #6 `space_acl_del` → `auth_info[]` | **不携带 `auth`** | 靠 `auth` 可空 + `WhenWritingNull` 保证不序列化（WR5 断言 `Auth` 的类型是 `ulong?`） |
| #21 `file_acl_add` → `auth_info[].auth` | **仅 1**（仅下载 / 仅浏览） | `type`（1 个人 / 2 部门）与 `departmentid` 官方标注「后续将废弃」 |
| #23 `file_setting` → `auth` | 普通文档 1 仅浏览（可下载）/ 4 仅预览（仅专业版）；微文档仅 1 | **不填充则保持原有状态** |
| #7 `space_setting` → `share_url_no_approve_default_auth` | 1 仅下载 / 2 可编辑 / 4 仅预览 / 5 可上传下载 / 200 自定义权限 | 邀请链接默认权限，与上面几组取值**不是同一集合** |

`departmentid` 是 `ulong?`（注释：32 位整型，取值范围 `[0, 2^32)`），`userid` 是 `string?`——两键按 `type` 择一填写，SDK **不做**本地择一校验。

**权限类端点是增量而非整体替换**：#5/#6/#21/#22 语义是「添加 / 移除一批成员」（注释「可一次性添加多个」），
#7/#23/#26 语义是「字段级部分写」——未填充的可选字段**保持原有状态**（#7 注释与 DTO remarks 均留档，靠 `WhenWritingNull` 实现）。
⇒ 不要用「先删全部再加全部」模拟整体替换，也不要把 `null` 与「不改」混为一谈。

## 3. 空间安全设置：写入口 6 字段、回显 12 字段，且「分享到企业外」两侧语义相反

写侧 [SetWedriveSpaceSettingRequest.cs](../../../Mud.Wechat.Work/Wedrive/SetWedriveSpaceSettingRequest.cs) 只有
`spaceid` + 6 个可写键（`enable_watermark` / `enable_confidential_mode` / `share_url_no_approve` /
`share_url_no_approve_default_auth` / `default_file_scope` / `ban_share_external`）；
读侧 [WedriveSpaceSecureSetting.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceSecureSetting.cs) 是 12 键的只读回显超集
（多出 `add_member_only_admin`、`enable_share_url`、`enable_share_external_admin`、`enable_space_add_external_member`、
`enable_space_add_external_member_admin`、`create_file_only_admin`）。

WR5 把最易踩的一处钉成断言——**同一件事在读写两侧字段名不同名**：

- 写入口：`ban_share_external`（**是否禁止**分享到企业外，`true` = 禁止）；
- 回显：`enable_share_external`（**是否允许**分享到企业外，`true` = 允许）。

⇒ 两侧**既不同名、语义方向也相反**，「设 `ban_share_external = true` 后回显 `enable_share_external` 应为 `false`」，
校验回显时须取反比对，不得写成同值比对，更不得「顺手统一」字段名。
其余同向键（`enable_watermark` / `enable_confidential_mode` / `share_url_no_approve` /
`share_url_no_approve_default_auth` / `default_file_scope`）才是可直接对照的。
注意文件侧的 [WedriveFileSecureSetting.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileSecureSetting.cs)
又用 `ban_share_external`（与空间写入口同名、与空间回显不同名），三处形态各异，勿互相套用。

#7 的调用前置（注释留档）：**应用通过 API 只支持设置由本应用创建的空间**；`enable_watermark` 仅专业版企业可设置。

## 4. 「获取空间信息」是两条独立路由、两个不同构响应类型

#4 `space_info`（旧版，挂官方「管理空间」分组）与 #9 `new_space_info`（新版，挂官方「管理空间权限」分组）
不是同一端点的别名——WR1 把两者同时计入 31 条路由，WR5 断言响应类型分别落
[WedriveSpaceInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceInfo.cs) 与
[WedriveNewSpaceInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveNewSpaceInfo.cs)，
并反向断言 `WedriveSpaceInfo` 上**不存在** `SecureSetting` 属性。

差集只有两处（接口注释留档）：新版额外返回 `secure_setting`（§3 的 12 键回显）与 `auth_list.quit_userid`
（已退出空间的成员 userid 列表；[WedriveSpaceAuthList.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceAuthList.cs)
注释口径是「成员在一个有权限的部门中自己退出空间或被移除权限」）。
⇒ 只需要基础信息与成员权限时用 #4；需要校验安全设置或识别退群成员时**必须**用 #9，用 #4 会静默拿不到这两键。

## 5. 上传是两条通路、其中一条三段式；下载是两段式

**通路 A（整文件，≤10M）**：#11 一次调用完成，`file_base64_content` 只填文件内容的 Base64、
**不带** `data:...;base64` 数据类型前缀（DTO remarks 留档）。

**通路 B（分块，≤20G）**：#12 → #13 → #14 三步，一个官方文档页承载三条路由。留档口径：
2M = **2097152 字节**固定分块、`block_sha` 是**按分块顺序的文件分块累积 sha1**（`List<string>`）、
`index` **从 1 开始**且类型是 `int?`（WR5 特别断言「区别于域内其余 uint32 字段」），分块可并发、**官方建议并发 ≤ 10**。

`upload_key`（#12 返回、#13/#14 回传）是**本次上传流程的凭证**：不命中秒传才返回；不得进日志、遥测或异常消息。
秒传判定 `hit_exist = true` 时 #12 直接返回 `fileid`、**上传流程即完成**，此时既没有 `upload_key` 也**不得**再调 #13/#14
（接口注释：「此时上传流程完成、无需继续分块上传」）。`skip_push_card`（`bool?`）控制文件创建完成时是否推送企业微信卡片，
注释口径「默认 false，即默认推送卡片」。

**互斥参数组**：#11/#12 的 `spaceid` + `fatherid` 与 `selected_ticket`「**必须填且仅填其中一组**」，
#15 下载侧是 `fileid` 与 `selected_ticket` 同规则（WR1/WR5 与接口注释三处一致）。
`selected_ticket` 属 jsapi 选择器凭据 ⇒ 同样不得进日志。SDK **不做**本地互斥校验，错配由应答 `errcode` 表达。

**下载两段式**：#15 不返回文件字节，只返回
[DownloadWedriveFileResponse.cs](../../../Mud.Wechat.Work/Wedrive/DownloadWedriveFileResponse.cs)
的 `download_url`（**有效期 2 小时**）+ `cookie_name` / `cookie_value`。
取回内容须由宿主自行发起对 `download_url` 的请求并携带该 Cookie——`download_url` 与其 Cookie 组合等价于凭据，
同样不得进日志/遥测/异常消息，且 2 小时窗口内用完即弃、不得长期持久化。
#15 **只支持普通文件**，不支持文件夹或微文档（请求与响应注释同口径）。

## 6. 批量与「双层失败」：本域没有 `jobid` / `task_id` 异步任务面

先划清否定面：本域 31 支端点里**没有异步任务端点**——本目录接口与
[Mud.Wechat.Work/Wedrive/](../../../Mud.Wechat.Work/Wedrive/) 全量检索无 `jobid` / `task_id` / 结果查询端点，
故 CorpGroup 域那套「提交任务 → 轮询结果」语义在微盘**不适用**；分块上传的三步是**同步连续调用**，不是异步任务。

真正的双层语义在 #29/#30：

1. **传输/信封层**：`errcode` 非 0 时由基座判定与 `WechatWorkException.ThrowIfFailed` 抛出（令牌失效自动恢复也在此层）。
2. **条目层**：`errcode == 0` **不等于全部成功**。
   [BatchAddWedriveVipResponse.cs](../../../Mud.Wechat.Work/Wedrive/BatchAddWedriveVipResponse.cs) 与
   [BatchDelWedriveVipResponse.cs](../../../Mud.Wechat.Work/Wedrive/BatchDelWedriveVipResponse.cs)
   各持 `succ_userid_list` / `fail_userid_list` 两个 `List<string>?`，必须逐条判 `fail` 列表。
   注释留档一处反直觉口径：**已经拥有高级功能账号的成员计入成功列表**（#29）。
   两响应形态同构但**分属两端点各自承载 DTO**（WR5 注释），不得合并成一个类型。

批量上限：#29/#30 的 `userid_list`（`List<string>`，WR5 断言数组形态）单次 **100** 个；
#18/#19 的 `fileid` 数组「支持批量」但**无数量上限留档**（缺口）；SDK 侧一律不裁剪、不拆批。

#18 `MoveFileAsync` 的 `replace`（`bool?`）是重名冲突策略而非幂等开关：
`true` 覆盖、`false` 冲突重命名（注释举例 `xxx(1).txt`、`xxx(1).doc`）。

## 7. 分页三形态并存，且文件列表响应是「对象内含数组」

同一域内并存两套分页形态（WR5 分别断言字段名）：

| 端点 | 形态 | 请求键 | 响应键 | 留档口径 |
|---|---|---|---|---|
| #10 `file_list` | **start + limit** | `start`（首次填 0，后续填上次返回的 `next_start`）、`limit`（≤ 1000）、`sort_type`（1~6） | `has_more`（`bool?`）、`next_start`（`ulong?`） | 接口注释明示「区别于本仓多数域的 cursor + limit」 |
| #31 `vip/list` | **cursor + limit** | `cursor`（`string?`，首次可不填）、`limit`（默认 100、最大 200） | `has_more`、`next_cursor` | 注释留档官方明确「不保证每次返回刚好为指定 `limit`，**必须**用 `has_more` 判断是否继续请求」 |

⇒ 两套翻页游标不可互换（`next_start` 是数值、`next_cursor` 是字符串），也**不要**按 `limit` 是否填满来判断结束。

#10 与 #18 的响应 `file_list` 官方形态是**对象内含 `item` 数组**
（[WedriveFileList.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileList.cs) 只有一个 `Item` 属性，
WR5 断言「不得建成裸数组」）；#17 重命名响应的字段名是 `file`、#20 是 `file_info`，
三处都指向同一个 [WedriveFileInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileInfo.cs)（WR5 逐个断言共用），
`file_size` 单位 B、`ctime` / `mtime` 是时间戳。

## 8. 分享与外链：空间「邀请链接」与文件「分享链接」是两条不同契约

| 面 | 端点 | 字段 | 本仓留档 |
|---|---|---|---|
| 空间邀请链接 | #8 `space_share` | `space_share_url` | 成员可经链接申请加入，**是否需审批由 #7 的 `share_url_no_approve` 决定**；默认权限由 `share_url_no_approve_default_auth` 决定 |
| 文件分享链接 | #24 `file_share` | `share_url` | WR5 专门断言两个字段名不同（「区别于空间邀请链接 `space_share_url`」） |
| 文件可见范围 | #23 `file_setting` | `auth_scope` 1 指定人 / 2 企业内 / 3 企业外 / 4 企业内需管理员审批 / 5 企业外需管理员审批 | 4 与 5 **仅有管理员时可设置** |
| 范围回显 | #25 `get_file_permission` | `share_range`（[WedriveFileShareRange.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileShareRange.cs) 六键：`enable_corp_internal` / `corp_internal_auth` / `enable_corp_external` / `corp_external_auth` / `corp_*_approve_only_by_admin`） | `corp_*_auth` 取值含 **255 无权限或需审批** |

**有效期**：本域三类证据只留档下载 `download_url` 的 2 小时；空间邀请链接与文件分享链接的**有效期 / 撤销面均无留档、也无撤销端点** ⇒ 入缺口。
`#25` 的 `watermark` 与 `#26` 的 `watermark` 共用
[WedriveFileWatermark.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileWatermark.cs)，
但写入口**只开放 4 个字段**（`text` / `margin_type` / `show_visitor_name` / `show_text`），
`force_by_admin` 与 `force_by_space_admin` 是读取回显专用（DTO remarks 留档，靠可空 + `WhenWritingNull` 不序列化）；
`show_visitor_name` 仅专业版支持。

## 9. 应用类型面：五族等宽 + vip 族仅自建，归属域键不在本域守卫内断言

WR2 的 `AssertFamily` 对六族各做四件事：父接口 `IsAbstract = true` 且 `RegistryGroupName` 为空、父接口声明面恰 4/5/11/6/2/0、
**继承链子接口集合不漂移**（`BeEquivalentTo`）、每个子接口 `RegistryGroupName = "Wedrive"` +
`InheritedFrom` 指向父实现类 + 声明面恰 0/0/0/0/0/3。
⇒ 前五族「官方对三类应用开放一致」，三个子接口都是**空标记**（零声明端点，全部端点来自继承链）；
vip 族是**零端点父接口 + 唯一自建子接口**，代开发与第三方子接口**不得补上**（官方标注「暂不支持」，
WR2 的 `BeEquivalentTo` 会因「补子接口」而失败——该断言对子接口集合是精确比对的单向锁）。

命名空间分区（[WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) N1~N3）：
6 个父接口落 `Mud.Wechat.Work.Interfaces`，16 个可注入子接口落 `Mud.Wechat.Work`；
用户 `using Mud.Wechat.Work;` 即可见全部可注入接口且看不到父接口。
计数基线以 [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 为准（当前 `AbstractParentInterfaces = 154` /
`InjectableInterfaces = 307`）；[AGENTS.md](../../../../../AGENTS.md) §4 与本域无关的 147/299 与 N1 内联注释的 155/308 **均已陈旧**，
以基线常量为准，勿据此调整本域。

令牌面（WR3 只锁路由键）：`TokenType` 恒 `Wechat.AccessToken`、Query 注入、参数名 `access_token`。
**归属域键不在 WR3 内重复断言**（守卫注释明示由 `WechatTokenOwnerContractGuards` 全局锁定），本仓实测声明为：

| 子接口族 | `TokenManagerKey` | 调用前置 |
|---|---|---|
| 各族 `IWechatWorkInternalWedrive*Service`（含 `IWechatWorkInternalWedriveVipService`） | `WechatTokenManagerKeys.InternalAccessToken` | 自建应用须配置到「可调用应用」列表（vip 族注释口径为「协作 - 微盘 - 可调用接口的应用」） |
| 五族 `IWechatWorkProviderWedrive*Service` / `IWechatWorkThirdPartyWedrive*Service` | `WechatTokenManagerKeys.CorpAccessToken` | 须具有「微盘」权限；调用前经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 建立「应用 + 企业」作用域（`permanent_code` 只写参数名，不得进日志） |

Query 令牌注入的接口名白名单（G5）里本域 22 个接口名逐条在列，落位见
[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（L712–L755）。
本域凭据类字段（`upload_key` / `selected_ticket` / `download_url` + Cookie / `file_base64_content`）**全部落 JSON body、不进 Query**
⇒ 不触发 G7 的 Query 凭据词表；但它们同样属敏感值，禁止写进日志、遥测或异常消息，示例一律用假值。

## 10. 无业务负载端点固定返回 `WechatWorkResponse`，空请求体端点不建请求 DTO

WR1 内联断言 11 支端点的返回类型**必须是** `Task<WechatWorkResponse>`（禁止为其新建空响应 DTO）：
#2、#3、#5、#6、#7、#13、#19、#21、#22、#23、#26。
另断言 #27/#28 的参数表**恰 1 个**且类型为 `CancellationToken`（官方请求包体为空对象 `{}` ⇒
本 SDK 不声明请求 DTO，对齐 `get_openid_migration` 无请求体先例，接口注释留档）。

## 11. 与邻域的边界：id 体系三套，互不通用

| 邻域 | 本域对应 | 不可混用之处 |
|---|---|---|
| [Media/](../Media/IWechatWorkMediaService.cs)（素材） | `/cgi-bin/media/*`，multipart/form-data 上传，返回 `media_id` | 微盘走 **JSON body + base64**，返回 `fileid`；`media_id`（注释留档：仅三天有效、企业内应用共享）与 `fileid` **不是同一 id 体系**，`media_id` 不能当 `fileid` 传给 #15/#20，反向亦然 |
| [Wedoc/](../Wedoc/)（文档） | `/cgi-bin/wedoc/*`（`create_doc` / `get_doc_base_info` / `doc_share` 等） | #16 `file_create` 用 `file_type` 3/4 在微盘里创建文档/表格并返回 `fileid`（文档另回 `url`）；Wedoc 域以其自身文档 id 操作内容。**同一份在线文档的两侧 id 不得互换**，跨域封装须显式转换 |
| [Callback 包](../../../Mud.Wechat.Work.Callback/Events/Payloads/WeDrive/) | `wedrive_insufficient_capacity` / `wedrive_space_change` / `wedrive_file_change` 三个事件键及其载荷 | 推送接收侧（XML 事件与载荷契约）**不属于本域**，由 `Callback` 包与 CB 系列守卫承载；本域全部是发送侧 HTTP API |
| CorpGroup 规则族 | `/cgi-bin/corpgroup/rule/*` 的「仅创建空间的主企业」口径 | 路由与令牌都不同，且该族有异步任务（`jobid`）、本域没有（§6），**不得**套用其轮询语义 |

字段名一律照官方原文大小写与连写（WR5 用 `JsonNameShouldBe` 逐键锁定，共 60 余个断言）：
`spaceid` / `fileid` / `fatherid` / `new_name` / `file_name` / `file_size` / `file_type` / `file_status` /
`space_share_url` / `share_url` / `quit_userid` / `auth_info` / `has_more` / `next_start` / `next_cursor` /
`sort_type` / `block_sha` / `skip_push_card` / `hit_exist` / `upload_key` / `index` / `download_url` /
`cookie_name` / `cookie_value` / `share_range` / `secure_setting` / `inherit_father_auth` / `file_member_list` /
`co_auth_list` / `watermark` / `enable_corp_internal` / `corp_internal_auth` / `enable_readonly_copy` /
`modify_only_by_admin` / `enable_readonly_comment` / `ban_share_external` / `enable_share_external` /
`is_pro` / `total_vip_acct_num` / `use_vip_acct_num` / `pro_expire_time` / `total_capacity_for_all` /
`total_capacity_for_vip` / `succ_userid_list` / `fail_userid_list` / `userid_list`。
⇒ 不按「统一命名」改写；新增字段一律照抄。

## 快速开始

```csharp
using System.Security.Cryptography;

// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddWedriveApi());

// 自建应用注入自建子接口（6 个公共父接口 IsAbstract，不可注入）
public sealed class DriveSync(
    IWechatWorkInternalWedriveSpaceService space,
    IWechatWorkInternalWedriveSpaceAclService spaceAcl,
    IWechatWorkInternalWedriveFileService file,
    IWechatWorkInternalWedriveVipService vip)
{
    // 1) 建空间（创建者是应用本身）+ 加成员：auth = 7 的管理员最多 3 个且不支持部门
    public async Task<string> CreateSpaceAsync(CancellationToken ct)
    {
        CreateWedriveSpaceResponse created = await space.CreateSpaceAsync(new CreateWedriveSpaceRequest
        {
            SpaceName = "示例空间",                 // 官方必填
            SpaceSubType = 0,                       // 目前仅支持 0（普通空间）
            AuthInfo = new List<WedriveSpaceAclMember>
            {
                new WedriveSpaceAclMember { Type = 1, Userid = "example-user-0001", Auth = 7 },
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(created, "/cgi-bin/wedrive/space_create");

        AddWedriveSpaceAclRequest add = new()
        {
            Spaceid = created.Spaceid,
            AuthInfo = new List<WedriveSpaceAclMember>
            {
                new WedriveSpaceAclMember { Type = 1, Userid = "example-user-0002", Auth = 1 },
            },
        };
        WechatWorkResponse resp = await spaceAcl.AddSpaceAclAsync(add, ct);   // 增量添加成员
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/wedrive/space_acl_add");
        return created.Spaceid!;
    }

    // 2) 列目录：start + limit（首次 start = 0，后续用 next_start），limit ≤ 1000
    public async Task<List<WedriveFileInfo>> ListFilesAsync(string spaceid, CancellationToken ct)
    {
        List<WedriveFileInfo> all = new List<WedriveFileInfo>();
        ulong start = 0;
        while (true)
        {
            GetWedriveFileListResponse resp = await file.GetFileListAsync(new GetWedriveFileListRequest
            {
                Spaceid = spaceid,
                Fatherid = spaceid,      // 根目录时 fatherid 填 spaceid
                SortType = 5,            // 5 = 修改时间升序
                Start = start,
                Limit = 500,
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/wedrive/file_list");

            List<WedriveFileInfo> items = resp.FileList?.Item ?? new List<WedriveFileInfo>();
            all.AddRange(items);
            if (resp.HasMore != true || resp.NextStart is null) break;
            start = resp.NextStart.Value;
        }
        return all;
    }

    // 3) 上传：≤10M 走整文件；更大走「初始化 → 分块 → 完成」三步，命中秒传即结束
    public async Task<string> UploadAsync(string spaceid, string fileName, byte[] content, CancellationToken ct)
    {
        if (content.Length <= 10 * 1024 * 1024)
        {
            UploadWedriveFileResponse resp = await file.UploadFileAsync(new UploadWedriveFileRequest
            {
                Spaceid = spaceid,
                Fatherid = spaceid,
                FileName = fileName,                       // 最多 255 字符，英文 1 / 汉字 2
                FileBase64Content = Convert.ToBase64String(content),  // 不带 data:...;base64 前缀
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/wedrive/file_upload");
            return resp.Fileid!;
        }

        InitWedriveFileUploadResponse init = await file.InitFileUploadAsync(new InitWedriveFileUploadRequest
        {
            Spaceid = spaceid,
            Fatherid = spaceid,
            FileName = fileName,
            Size = (ulong)content.Length,                  // ≤ 20G
            BlockSha = BuildBlockShaList(content),         // 按分块顺序的累积 sha1（口径见文末缺口）
            SkipPushCard = true,
        }, ct);
        WechatWorkException.ThrowIfFailed(init, "/cgi-bin/wedrive/file_upload_init");
        if (init.HitExist == true) return init.Fileid!;    // 秒传：流程已完成，不得再分块上传

        string uploadKey = init.UploadKey!;                // 流程凭证：不得进日志/遥测/异常消息
        const int blockSize = 2097152;                     // 2M 固定分块
        int blockCount = (content.Length + blockSize - 1) / blockSize;
        List<Task<WechatWorkResponse>> inFlight = new List<Task<WechatWorkResponse>>();
        for (int i = 0; i < blockCount; i++)
        {
            int offset = i * blockSize;
            int length = Math.Min(blockSize, content.Length - offset);
            UploadWedriveFilePartRequest part = new()
            {
                UploadKey = uploadKey,
                Index = i + 1,                             // index 从 1 开始
                FileBase64Content = Convert.ToBase64String(content, offset, length),
            };
            inFlight.Add(file.UploadFilePartAsync(part, ct));   // 官方建议并发 ≤ 10，宿主自行限速
            if (inFlight.Count >= 10)
            {
                WechatWorkResponse[] done = await Task.WhenAll(inFlight);
                foreach (WechatWorkResponse r in done)
                    WechatWorkException.ThrowIfFailed(r, "/cgi-bin/wedrive/file_upload_part");
                inFlight.Clear();
            }
        }
        WechatWorkResponse[] tail = await Task.WhenAll(inFlight);
        foreach (WechatWorkResponse r in tail)
            WechatWorkException.ThrowIfFailed(r, "/cgi-bin/wedrive/file_upload_part");

        FinishWedriveFileUploadResponse finish = await file.FinishFileUploadAsync(
            new FinishWedriveFileUploadRequest { UploadKey = uploadKey }, ct);
        WechatWorkException.ThrowIfFailed(finish, "/cgi-bin/wedrive/file_upload_finish");
        return finish.Fileid!;
    }

    // 4) 下载是两段式：本端点只拿 url + Cookie，字节流由宿主自行请求（url 有效期 2 小时）
    public async Task<(string Url, string CookieName, string CookieValue)> DownloadAsync(string fileid, CancellationToken ct)
    {
        DownloadWedriveFileResponse resp = await file.DownloadFileAsync(
            new DownloadWedriveFileRequest { Fileid = fileid }, ct);   // 仅普通文件
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/wedrive/file_download");
        return (resp.DownloadUrl!, resp.CookieName!, resp.CookieValue!);
    }

    // 5) 高级功能账号：errcode == 0 不代表全部成功，必须读 fail 列表（单次 ≤ 100）
    public async Task<List<string>> GrantVipAsync(List<string> userids, CancellationToken ct)
    {
        BatchAddWedriveVipResponse resp = await vip.BatchAddVipAsync(
            new BatchAddWedriveVipRequest { UseridList = userids }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/wedrive/vip/batch_add");
        return resp.FailUseridList ?? new List<string>();
    }

    private static List<string> BuildBlockShaList(byte[] content)
    {
        // 每块 sha1（十六进制小写）；官方要求的「累积 sha1」拼接口径本仓三类证据未留档 ⇒ 落地前先核验
        const int blockSize = 2097152;
        List<string> sha = new List<string>();
        for (int offset = 0; offset < content.Length; offset += blockSize)
        {
            int length = Math.Min(blockSize, content.Length - offset);
            sha.Add(Convert.ToHexString(SHA1.HashData(content.AsSpan(offset, length))).ToLowerInvariant());
        }
        return sha;
    }
}

// 第三方 / 代开发：先切到授权企业作用域（scope = authCorpId），再调用；permanent_code 只写参数名
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    GetWedriveNewSpaceInfoResponse info = await thirdPartySpaceAcl.GetNewSpaceInfoAsync(
        new GetWedriveNewSpaceInfoRequest { Spaceid = spaceidOfThatCorp }, ct);
    WechatWorkException.ThrowIfFailed(info, "/cgi-bin/wedrive/new_space_info");
    // 安全设置只在「新版」响应里回显；且写入口 ban_share_external 与回显 enable_share_external 方向相反
    bool? allowShareExternal = info.SpaceInfo?.SecureSetting?.EnableShareExternal;
}
```

判错为单层普通信封（`errcode == 0` 即成功），批量端点再叠条目层（§6）；
令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。
上传下载的凭证字段（`upload_key`、`selected_ticket`、`download_url` + Cookie）与文件内容 base64 一律不得进日志、遥测或异常消息。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域频率 / 并发上限 | 22 份接口文件**没有任何频率数值**（无 QPS、无每天次数）；唯一的并发口径是 #13 分块上传「官方建议并发 ≤ 10」 | 逐页核验后补接口 remarks，再更新本文速览备注；**未逐项核验** |
| `block_sha` 的「累积」拼接口径、分片数量上下限 | 注释只写「按分块顺序的文件分块累积 sha1」与「2M（2097152 字节）固定分块」，未写累积算法、未写最大片数 ⇒ **未逐项核验** | 示例里的 `BuildBlockShaList` 只是逐块 sha1，落地前必须核验官方口径；不得据此断言可用 |
| `upload_key` 有效期、秒传去重维度（按 sha 全局还是按空间） | DTO 与注释只写「不命中秒传时返回」⇒ **未逐项核验** | 需要重试/续传策略时先核验，勿在 SDK 侧加隐式重试 |
| `download_url` 取回内容时是否还需携带 `access_token` | 响应只回 `download_url` + `cookie_name` / `cookie_value`，注释未提其它凭据 ⇒ **未逐项核验** | 由宿主实测；勿假设可匿名拉取 |
| 空间邀请链接 / 文件分享链接的**有效期**与撤销面 | #8/#24 只返回链接字符串；本域 31 条路由内无「撤销分享」端点，注释无有效期数值 ⇒ **未逐项核验** | 需要过期治理时先核验官方页，再走「加端点 + 同批改 WR1 计数」 |
| `space_sub_type` 取值全集（共享 / 群组 / 个人空间） | 注释只留档 `0 - 普通空间`（且「目前只支持 0」）⇒ 其它空间形态**未逐项核验** | 出现新取值先核验，再决定是否建枚举；不得把管理后台的空间分类直接映射成该键的取值 |
| 空间安全设置可写面是否窄于本仓建模 | 写侧 [SetWedriveSpaceSettingRequest.cs](../../../Mud.Wechat.Work/Wedrive/SetWedriveSpaceSettingRequest.cs) 6 键 vs 读侧 [WedriveSpaceSecureSetting.cs](../../../Mud.Wechat.Work/Wedrive/WedriveSpaceSecureSetting.cs) 12 键；多出的 6 键（`add_member_only_admin` / `enable_share_url` / `enable_space_add_external_member(_admin)` / `create_file_only_admin` / `enable_share_external_admin`）官方是否给出 API 写入口**未逐项核验** | 若官方补写入口，须同批补 DTO 与 WR5 的 `JsonNameShouldBe` 清单，并同步改本文 §3 |
| `auth` / `auth_scope` 各集合是否还有其它合法值 | 本仓一律以 `ulong?` 承载、不建枚举（§2 表只覆盖注释留档值）；文件侧 `auth` 是否恒为 1、`corp_*_auth` 的 255 之外是否还有值**未逐项核验** | 需要枚举化时先核验，再决定是否建常量类 |
| #18 / #19 批量 `fileid` 数组的单次上限 | 注释只写「数组形态、支持批量」，无数值 ⇒ **未逐项核验** | 需要拆批时先核验官方页，不得从 #29/#30 的 100 借数值 |
| `auth = 7`「最多 3 个」的计数口径 | #1 注释「最多可指定 3 个」、#5 注释「连同已设置的管理员最多可指定 3 个」；应用自身是否占名额、部门是否计入**未逐项核验** | 不得据此在 SDK 侧做人数预校验 |
| `file_type` 的权威取值集 | [WedriveFileInfo.cs](../../../Mud.Wechat.Work/Wedrive/WedriveFileInfo.cs) remarks 记录「各文档页枚举略有出入（列表页列 1~5，其余页列 1~6 含 6:文档(幻灯片)）」，本模型按 1~6 承载；#16 创建侧注释只列 1/3/4 ⇒ 全集**未逐项核验** | 出现新类型先核验，再统一 remarks |
| `file_member_list` / `co_auth_list` / `inherit_father_auth` 的 obj-vs-数组形态 | WR5 与本仓注释记录官方「参数表列 obj、返回示例实为数组」，本 SDK 按数组承载 ⇒ 官方现行口径是否已改**未逐项核验** | 若官方改为对象形态，须同批改 WR5 类型断言与本文 §8 |
| 微盘容量与配额告警事件 | #28 的 `total_capacity_for_*` 单位 B、注释无换算与阈值；配额不足以回调事件 `wedrive_insufficient_capacity` 呈现，落 Callback 域（CB 系列守卫） | 需要容量治理时查 [Callback 载荷](../../../Mud.Wechat.Work.Callback/Events/Payloads/WeDrive/)，勿在本域加端点 |
| 「微盘是否授权 / 授权状态」类端点 | 本域 31 条路由已封闭（WR1），无此类端点，其归属面**未在本域核验** | 若需授权状态面，先定位官方所属分组再按「加面 + 同批补守卫」落地，勿塞进本域 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（255 字符、10M、20G、2M/2097152、并发 ≤ 10、2 小时、1000、100、默认 100/最大 200、`auth` 取值集合、官方权限口径、MUD005） | 本目录 22 份接口文件的 XML 注释 |
| 六族路由表与计数（4 + 5 + 11 + 6 + 2 + 3 = 31）、全 POST、`allRoutes` 全量清单 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs) WR1（L247–L361，路由表 L72–L241） |
| 11 支「无业务负载」端点返回类型 + 2 支空请求体端点参数表 | WR1 内两段循环断言（L327–L360） |
| 六族层级、父接口 `IsAbstract` / 空组、子接口集合不漂移、声明面 0/0/0/0/0/3 | WR2（L368–L440）与 `AssertFamily`（L443–L473） |
| 22 个接口的 `AccessToken` + Query + `access_token` 绑定 | WR3（L481–L522） |
| JSON 上下文登记（60 型，`SerializerClassName` 恒 `Wedrive`）与各族类型拆分口径 | WR4（L529–L606，计数注释 L540–L546） |
| 字段名 / 类型 / 共用结构 / `ban_share_external` ↔ `enable_share_external` / `fileid` 数组形态 / `index` 为 `int?` | WR5（L614–L814），辅助 `AssertRoutes`（L817–L828）、`JsonNameShouldBe`（L831–L839） |
| 域级「官方反直觉点」总述（全 POST、新旧两版、`spaceid` 拼写、start+limit 与 cursor+limit 并存、vip 仅自建） | 守卫类头 remarks（L34–L63） |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work/Wedrive/](../../../Mud.Wechat.Work/Wedrive/)（60 型，平铺无子目录） |
| 上下文源生成文件（`internal`，示例不得引用） | [../../../Mud.Wechat.Work/Generated/WedriveJsonContext.g.cs](../../../Mud.Wechat.Work/Generated/WedriveJsonContext.g.cs) |
| Query 令牌注入白名单（G5，本域 22 个接口名） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（L712–L755） |
| 令牌归属域键（TO1~TO3，本域守卫不重复断言） | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 命名空间分区与聚合计数基线 | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3）、[Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L163） |
| 线级索引（本域仅列目录，无端点计数） | [../../README.md](../../README.md)（L27） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号（自建 / 第三方 / 代开发三页）与业务限制（频率、上限、互斥参数组、覆盖写还是增量），
   **同批**改 [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)
   的六张路由表 + WR1 的 `HaveCount(4/5/11/6/2/3)`、`allRoutes` 全量清单与 `HaveCount(31)`；
   新增/删除无负载端点须同批改 WR1 的 `noPayloadEndpoints` 清单。
2. 改开放面（官方给 vip 族补代开发/第三方，或某族三类不再等宽）→ 先核验，再动子接口，并同批改
   WR2 的 `AssertFamily` 参数（父接口声明面数、子接口集合、每子接口声明面数）与
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
   的 G5 白名单（新增接口名必须入列，白名单未放宽）；`TokenType` 恒 `Wechat.AccessToken`，**不得**按应用类型拆分；
   新子接口的 `TokenManagerKey` 由 TO1~TO3 全局扫描，错配会在 `WechatAppContext.GetTokenManager` 处 fail-fast。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Wedrive")]`（本域**只有一个**上下文，
   与 CorpGroup 的三上下文形态不同）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 WR4 的 `HaveCount(60)` 与端点级抽查清单（`endpointContractTypes`）、必要时更新 WR5 的 `JsonNameShouldBe` 清单；
   生成物 `WedriveJsonContext.g.cs` 勿手改，登记面同时体现在
   [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L163）。
4. 新增接口族 / 重命名子接口 → 三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddWedriveApi()` XML（端点叙述须与本域 31 一致）、
   可注入子接口 `RegistryGroupName = "Wedrive"`（父接口保持空组，N3）；
   父接口留在 `Mud.Wechat.Work.Interfaces`、子接口留在 `Mud.Wechat.Work`（N1/N2，聚合计数改
   [Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 一处）。
5. 凭据面改动一律不得引入真值：`access_token`、`upload_key`、`selected_ticket`、`download_url` + `cookie_name` / `cookie_value`、
   `file_base64_content`、`permanent_code` 只写参数名，且不得进日志 / 遥测 / 异常消息；
   本域敏感字段全在 JSON body，若新端点把凭据放进 Query，必须同批在「补 `SensitiveUrlKeys` 词表」与「登记豁免（附追踪号）」中二选一。
6. 本文与 [../../README.md](../../README.md) 的本域表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
