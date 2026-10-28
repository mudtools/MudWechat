# Checkin 域 · 打卡

企业微信「打卡」域：本仓共落地 **15 条官方路由**（守卫 [CK1](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs) 的去重清单，**全部为 POST**），
按应用类型展开为 **26 个声明端点方法**（CK2 逐接口计数：公共父接口 4 + 自建 11 + 代开发 8 + 第三方 3），
分属五个接口族——打卡规则族 **6 条路由**（[IWechatWorkCheckinRuleService.cs](./Rule/IWechatWorkCheckinRuleService.cs) 1 支公共端点 +
[IWechatWorkCheckinRuleService_Internal.cs](./Rule/IWechatWorkCheckinRuleService_Internal.cs) 5 支差异端点 +
[IWechatWorkCheckinRuleService_Provider.cs](./Rule/IWechatWorkCheckinRuleService_Provider.cs) 同 5 支 +
[IWechatWorkCheckinRuleService_ThirdParty.cs](./Rule/IWechatWorkCheckinRuleService_ThirdParty.cs) 零端点空标记）、
打卡记录族 **4 条路由**（[IWechatWorkCheckinRecordService.cs](./Record/IWechatWorkCheckinRecordService.cs) **零端点父接口**，三分支子接口
[IWechatWorkCheckinRecordService_Internal.cs](./Record/IWechatWorkCheckinRecordService_Internal.cs) 4 /
[IWechatWorkCheckinRecordService_Provider.cs](./Record/IWechatWorkCheckinRecordService_Provider.cs) 1 /
[IWechatWorkCheckinRecordService_ThirdParty.cs](./Record/IWechatWorkCheckinRecordService_ThirdParty.cs) 1）、
打卡报表族 **2 条路由**（[IWechatWorkCheckinReportService.cs](./Report/IWechatWorkCheckinReportService.cs) **零端点父接口**，三分支子接口各 2）、
打卡排班族 **2 条路由**（[IWechatWorkCheckinScheduleService.cs](./Schedule/IWechatWorkCheckinScheduleService.cs) 2 支公共端点 + 三个空标记子接口）、
设备打卡数据族 **1 条路由**（[IWechatWorkCheckinDeviceService.cs](./Device/IWechatWorkCheckinDeviceService.cs) 1 支公共端点 + 三个空标记子接口）。
共 20 份接口文件（5 族 × 4 应用类型面，CK3 断言接口数恰 20）。
注册组 `Checkin`，模块枚举 `WechatModule.Checkin`（[`AddCheckinApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录接口文件的 XML 注释、
> [WechatCheckinContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)（CK1~CK5）、
> [Src/Work/Mud.Wechat.Work/Checkin/](../../../Mud.Wechat.Work/Checkin/) 下的 DTO 声明；
> 路由与计数以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（CK3 对 20 个接口统一断言 `TokenType = Wechat.AccessToken` +
`InjectionMode.Query` + `Name = "access_token"`，MUD005 已接受风险，无法改 Header）；「开放面」指官方对该端点开放的应用类型，
由 CK2 的父/子声明面计数锁死。归属域（自建 `InternalAccessToken` / 第三方与代开发 `CorpAccessToken`）见 §12。

### 打卡规则族（CK1 `CheckinRuleRoutes`，6 条路由 / 11 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetCheckinOptionAsync` | POST | `/cgi-bin/checkin/getcheckinoption` | `access_token`(Query)；自建 + 第三方 + 代开发 | 唯一三类公共端点（父接口承载）；90263/94204/99443 |
| 2 | `GetCorpCheckinOptionAsync` | POST | `/cgi-bin/checkin/getcorpcheckinoption` | `access_token`(Query)；自建 + 代开发 | 93384/99444；**请求体为空 JSON 对象** |
| 3 | `AddCheckinOptionAsync` | POST | `/cgi-bin/checkin/add_checkin_option` | `access_token`(Query)；自建 + 代开发 | 98041/98767；创建时 `groupid` 被忽略 |
| 4 | `UpdateCheckinOptionAsync` | POST | `/cgi-bin/checkin/update_checkin_option` | `access_token`(Query)；自建 + 代开发 | 98041/98767；覆盖式，数组无法清空（见 §7） |
| 5 | `ClearCheckinOptionArrayFieldAsync` | POST | `/cgi-bin/checkin/clear_checkin_option_array_field` | `access_token`(Query)；自建 + 代开发 | 98041/98767；`clear_field` 四位标识 |
| 6 | `DelCheckinOptionAsync` | POST | `/cgi-bin/checkin/del_checkin_option` | `access_token`(Query)；自建 + 代开发 | 98041/98767；仅该规则的创建应用可删 |

#2–#6 在自建与代开发子接口**各声明一次**（同路由同 DTO，CK1 路由表按自建面登记，Provider 面由 CK2 计数 5 锁定）；
第三方子接口零端点——官方权限表对 #2–#6 均标注「暂不支持」（CK2 断言其声明面 `HaveCount(0)`）。

### 打卡记录族（CK1 `CheckinRecordRoutes`，4 条路由 / 6 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 7 | `GetCheckinDataAsync` | POST | `/cgi-bin/checkin/getcheckindata` | `access_token`(Query)；自建 / 代开发 / 第三方**各一支** | 自建+代开发 90262/96497 现代结构；第三方 94205 旧字段结构 |
| 8 | `PunchCorrectionAsync` | POST | `/cgi-bin/checkin/punch_correction` | `access_token`(Query)；**仅自建** | 95803；应答无业务负载 |
| 9 | `AddCheckinRecordAsync` | POST | `/cgi-bin/checkin/add_checkin_record` | `access_token`(Query)；**仅自建** | 99647；一批 ≤200 条 |
| 10 | `AddCheckinUserFaceAsync` | POST | `/cgi-bin/checkin/addcheckinuserface` | `access_token`(Query)；**仅自建** | 93378；频率 10 次/分钟（本域最严） |

#7 是「同路由三分支、响应形态不同构」——三个子接口各返回不同类型（`GetCheckinDataResponse` vs `GetThirdPartyCheckinDataResponse`），
父接口因此零端点（见 §1）。

### 打卡报表族（CK1 `CheckinReportRoutes`，2 条路由 / 6 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 11 | `GetCheckinDayDataAsync` | POST | `/cgi-bin/checkin/getcheckin_daydata` | `access_token`(Query)；自建 / 代开发 / 第三方**各一支** | 93374/96498 现代；94206 旧结构（`baseinfo`） |
| 12 | `GetCheckinMonthDataAsync` | POST | `/cgi-bin/checkin/getcheckin_monthdata` | `access_token`(Query)；自建 / 代开发 / 第三方**各一支** | 93387/96499 现代；94207 旧结构 |

### 打卡排班族（CK1 `CheckinScheduleRoutes`，2 条路由 / 2 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 13 | `GetCheckinScheduleListAsync` | POST | `/cgi-bin/checkin/getcheckinschedulist` | `access_token`(Query)；自建 + 第三方 + 代开发 | 93380/94208/96500；仅「按班次上下班」规则 |
| 14 | `SetCheckinScheduleListAsync` | POST | `/cgi-bin/checkin/setcheckinschedulist` | `access_token`(Query)；自建 + 第三方 + 代开发 | 93385/94209/96501；`schedule_id = 0` 代表休息 |

### 设备打卡数据族（CK1 `CheckinDeviceRoutes`，1 条路由 / 1 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 15 | `GetHardwareCheckinDataAsync` | POST | `/cgi-bin/hardware/get_hardware_checkin_data` | `access_token`(Query)；自建 + 第三方 + 代开发 | 94126/95176/96504；**路由挂 `/cgi-bin/hardware/` 域** |

## 1. 记录族与报表族的父接口零端点：注入面必须是应用类型子接口

CK2 对这两族断言 `parentDeclaredEndpointCount: 0`——同一路由在第三方文档页是**另一套字段结构**，
无法收敛进公共父接口。直接后果：

| 族 | 可注入接口 | 应答类型 | 字段口径 |
|---|---|---|---|
| 记录·自建 | [IWechatWorkInternalCheckinRecordService](./Record/IWechatWorkCheckinRecordService_Internal.cs) | `GetCheckinDataResponse`（`Checkindata` → `List<CheckinRecord>`） | `lat`/`lng`/`deviceid`/`sch_checkin_time`，`checkin_type` 为**中文字符串** |
| 记录·代开发 | [IWechatWorkProviderCheckinRecordService](./Record/IWechatWorkCheckinRecordService_Provider.cs) | 同上（现代结构同构） | 同上 |
| 记录·第三方 | [IWechatWorkThirdPartyCheckinRecordService](./Record/IWechatWorkCheckinRecordService_ThirdParty.cs) | `GetThirdPartyCheckinDataResponse`（→ `List<ThirdPartyCheckinRecord>`） | `agency_name`/`location_title_lat`/`location_title_lng`/`schedule_checkin_time`，`checkin_type` 为**整数** |
| 报表·自建/代开发 | `IWechatWork{Internal,Provider}CheckinReportService` | `GetCheckinDayDataResponse` / `GetCheckinMonthDataResponse` | 顶层 `base_info`/`summary_info`/`ot_info`/`overwork_info` |
| 报表·第三方 | `IWechatWorkThirdPartyCheckinReportService` | `GetThirdPartyCheckinDayDataResponse` / `GetThirdPartyCheckinMonthDataResponse` | 顶层 `baseinfo`（**无下划线**）、`acctivity_name`、`overwork`/`ov_time` 为数组 |

`IWechatWorkCheckinRecordService` / `IWechatWorkCheckinReportService` 是 `IsAbstract = true` 的契约面父接口（命名空间
`Mud.Wechat.Work.Interfaces`，不注册 DI，见 [AGENTS.md §4](../../../../../AGENTS.md) 与 N1~N3 守卫），
**不可**用于取打卡数据或报表。规则族 / 排班族 / 设备族的父接口承载公共端点，但同样不可注入——注入的一律是子接口。

## 2. `getcheckindata` 与 `getcheckinoption` 是两种读法：时间区间 vs 单日 0 点

| 面 | 时间入参 | 语义 | 上限 |
|---|---|---|---|
| #7 `getcheckindata` | `starttime` / `endtime`（`long?`，Unix 时间戳）+ `opencheckindatatype`（`int?` 1 上下班 / 2 外出 / 3 全部） | **区间**读取原始打卡记录 | 自建/代开发：≤30 天；第三方：按打卡类型分档（上下班 ≤1 个月、外出 ≤3 天、全部 ≤3 天，系统自动校验，超过拒绝查询） |
| #1 `getcheckinoption` | `datetime`（`long?`，**需要获取规则的日期当天 0 点** Unix 时间戳） | **单日**读规则；用户在不同日期的规则不一定相同 ⇒ 必须**按天**取 | 无跨度参数，一天一次 |
| #11/#12 日报/月报 | `starttime` / `endtime`（`long?`，**0 点** Unix 时间戳） | 区间读统计 | 日报 ≤30 天；月报的跨度上限本仓注释无数值（缺口） |

两支的批量闸一致：`useridlist` **不超过 100** 个，超过须分批；本仓不做本地裁剪，越界由应答 `errcode` 表达。
`opencheckindatatype` 在自建/代开发文档页为官方必填、**第三方文档页标注为非必填**（同一 DTO
[GetCheckinDataRequest.cs](../../../Mud.Wechat.Work/Checkin/Record/GetCheckinDataRequest.cs) 承载三档口径，故建模为 `int?`）。

## 3. 打卡域有四种「时间」形态，`checkin_time` 是秒级时间戳而非 `YYYY-MM-DD`

本域**没有任何** `YYYY-MM-DD` 字符串日期字段；四种形态混用是最易写错的地方：

| 形态 | 类型 | 字段（实测） |
|---|---|---|
| Unix **秒级**时间戳 | `long?` | `checkin_time`、`starttime`、`endtime`、`sch_checkin_time`、`schedule_date_time`、日报 `base_info.date`（当天 0 点）、规则 `create_time`、第三方记录的 `schedule_checkin_time` |
| **距当天 0 点的偏移秒** | `int?` | `work_sec` / `off_work_sec` / `remind_work_sec` / `remind_off_work_sec`、日报 `earliest_time` / `lastest_time`、自由上下班 `span_day_time`、**补卡请求的** `schedule_checkin_time`（32400 = 9:00） |
| **年月数字** | `int?` | `yearmonth`（如 `202011`；官方类型 `uint32`，**不是字符串**） |
| **月内日号** | `int?` | `day`（1–31，与 `yearmonth` 联合组成唯一日期） |

两个必须分清的陷阱：

- **同名键 `schedule_checkin_time` 有两种形态**：[PunchCorrectionRequest.cs](../../../Mud.Wechat.Work/Checkin/Record/PunchCorrectionRequest.cs)
  是偏移秒（`int?`，无规则对应的卡点即休息日/无规则/自由上下班**不传**），
  [ThirdPartyCheckinRecord.cs](../../../Mud.Wechat.Work/Checkin/Record/ThirdPartyCheckinRecord.cs) 是时间戳（`long?`）。CK5 分别锁定。
- **偏移秒与毫秒混在一个规则对象里**：`limit_aheadtime` / `limit_offtime` 官方未标注单位，
  本仓注释按示例量级同 `checkindate.limit_aheadtime` 推为毫秒 ⇒ **未逐项核验**（见文末缺口），不得据此做单位换算。

## 4. `groupid` / `schedule_id` / `timeline_id` 与 `time_id` / `id` 的分工

| 键 | 出现面 | 语义 |
|---|---|---|
| `groupid` | 记录 #7、规则 #1/#2/#3–#6、排班 #13/#14 | 打卡**规则** id（`long?`） |
| `schedule_id` | 记录 #7、排班 #13 响应 `schedule_info[]`、#14 请求 `items[]` | **班次** id；排班侧 `0` 代表休息（响应侧注释补「名称为「休息」」） |
| `timeline_id` | 记录 #7 响应 | **时段** id（某班次中某一组上下班时段，如 9:00–12:00） |
| `time_id` | 规则**请求**侧 `checkindate.checkintime[]` 与 `schedulelist[].time_section[]` | 时段标识：规则内唯一、大于 0 小于 99999 |
| `id` | 排班**响应**侧 `time_section[]`（[CheckinScheduleTimeSection.cs](../../../Mud.Wechat.Work/Checkin/Schedule/CheckinScheduleTimeSection.cs)） | 同一时段标识在排班响应里叫 `id`，**不叫** `time_id`（CK5 锁定，勿「顺手统一」） |

**本域不存在 `rid` 键**（`rid` 是审批域的字段），也**不存在** `newdatetime` 键——规则侧的日期入参就叫 `datetime`、
记录/报表侧叫 `starttime`/`endtime`。跨域借名会直接写出官方不认的键。

## 5. 打卡范围 `range` 的三键拼写：与日程域**不通用**

[CheckinRange.cs](../../../Mud.Wechat.Work/Checkin/Rule/CheckinRange.cs)（请求/响应共用形态，官方要求**至少有一种人员来源**）：

| 属性 → JSON 名 | 类型 | 口径 |
|---|---|---|
| `PartyId` → `party_id` | `List<string>?` | **单数连写**（`party_id`，不是 `party_ids`）；部门节点 id 以**字符串**数组承载 |
| `Userid` → `userid` | `List<string>?` | 官方参数表标注 `string`，返回示例为字符串数组 ⇒ **以示例的数组形态承载**（CK5 锁定） |
| `Tagid` → `tagid` | `List<int>?` | 标签 id 为**整数**数组（与相邻两键的字符串形态不同） |

对照日程域 `ScheduleCalendarPublicRange` 是 `partyids`（连写、`List<long>?`）⇒ **两域拼写与元素类型都不同**，
不得在跨域封装里共用一个「部门 id 列表」模型。应用可见范围决定可读的 `userid` 集合（§12），
越界成员由官方侧报错、本仓不做本地可见性预判。

## 6. 15 条路由官方全部即 POST，其中 4 支是纯查询；#2 的请求体是空 JSON 对象

守卫 CK1 逐条断言 `typeof(PostAttribute)`，含仅查询语义的 `getcheckinoption` / `getcorpcheckinoption` /
`getcheckinschedulist` / `get_hardware_checkin_data` ⇒ **不得**「顺手统一」为 GET。

`GetCorpCheckinOptionAsync`（#2）的 [GetCorpCheckinOptionRequest](../../../Mud.Wechat.Work/Checkin/Rule/GetCorpCheckinOptionRequest.cs)
是**零公共属性** DTO（CK5 以 `Should().BeEmpty()` 锁定），官方请求体即 `{}`，凭据只在 Query 的 `access_token` 上。
调用方必须传一个空实例而不是 `null`。

> 守卫注释与断言不一致处（照此读，勿被注释带偏）：CK1 的行内注释（`WechatCheckinContractGuards.cs` L152）与
> `HaveCount` 的 because 文案（L214）写作「15 条官方路由去重后 12 条」，但同处的 `BeEquivalentTo` 清单实际列 **15** 项、
> 且 `allRoutes.Should().HaveCount(15)` 断言 **15** ⇒ **以断言与清单为准**：唯一路由 15 条、路由表登记 21 行（#7 与 #11/#12 各三分支）。

## 7. 打卡规则的更新语义：数组清空另有其门

#4 `update_checkin_option` 的官方更新语义（接口注释留档，**危险操作约束**）：

| 字段位置 | 不传 / 传空 | 传值 |
|---|---|---|
| `group.*` **数组**字段 | 不更新 | **覆盖**（清空原有数组，只保留传入的数组） |
| `group.*` **非数组**字段 | 不更新 | 覆盖（并递归其所有字段） |

- 要把一级数组字段清空为空集，`update` 做不到 ⇒ **必须**走 #5 `clear_checkin_option_array_field`，
  `clear_field` 为标识列表：`1` 清 `spe_workdays`、`2` 清 `spe_offdays`、`3` 清 `wifimac_infos`、`4` 清 `loc_infos`。
- 非数组字段直接传空元素即可清空。
- #4–#6 均要求「**打卡规则仅可由该规则的创建应用修改/删除**」；#3 创建时**不要**传 `groupid`（官方忽略）。
- 三支写端点都带 `effective_now`（`bool?`，是否立即生效）。
- 创建侧容量闸（接口注释）：`wifimac_infos` 与 `loc_infos` **不能同时为空**、wifi/地点个数 ≤500、`checkindate` 个数 1~7、
  单日打卡时段 1~4、排班个数 ≤50、特殊工作日/非工作日个数 ≤366；主要错误码 `301093`（参数校验失败）、`301094`（使用旧字段/旧特性）。
- 时段字段约束（[CheckinRuleCheckintime.cs](../../../Mud.Wechat.Work/Checkin/Rule/CheckinRuleCheckintime.cs)）：所有时间须为**整分钟**（60 的倍数）、
  上下班区间与可打卡时间不可有交集、多组休息时段 ≤5 组且不可有交集；
  弹性两组**互斥**（`flex_on_duty_time`/`flex_off_duty_time` 与 `max_allow_arrive_early`/`max_allow_arrive_late`，设一组时另一组置 0）；
  `flex_time` 官方注明旧字段、**openapi 创建与修改时不可使用**。

**加班信息两侧不同构**（CK5 锁定）：请求侧 [CheckinRuleGroup.cs](../../../Mud.Wechat.Work/Checkin/Rule/CheckinRuleGroup.cs) 是 `ot_info_v2`
（`CheckinOtInfoV2`，含 `workdayconf` 等），#2 响应侧 [CheckinCorpGroup.cs](../../../Mud.Wechat.Work/Checkin/Rule/CheckinCorpGroup.cs) 是旧 `ot_info`
（`CheckinOtInfo`，含 `otapplyinfo`）——官方错误表明确 `ot_info` 为旧字段不建议使用，但响应仍是它。两份结构**不可共用**。

## 8. 频率 / 批量 / 容量闸一览（全部由官方侧执行，本仓不拦截）

| 端点 | 频率（接口注释留档值） | 批量与容量 |
|---|---|---|
| #1 `getcheckinoption` | **未留档**（缺口） | `useridlist` ≤100；按天取 |
| #2 `getcorpcheckinoption` | 60 次/分钟 | 无入参（空体） |
| #3–#6 规则写端点 | 注释无频率数值（缺口） | §7 的创建侧容量闸 |
| #7 `getcheckindata` | 600 次/分钟 | 跨度 ≤30 天（第三方按类型分档）、`useridlist` ≤100 |
| #8 `punch_correction` | 600 次/分钟 | `remark` ≤512 字节 |
| #9 `add_checkin_record` | 60 次/分钟 | 一批 ≤200 条；`location_title`/`location_detail`/`notes`/`wifiname` ≤1024 字符、`device_detail` ≤40 字符、`mediaids` ≤1、`wifimac` 须满足六段冒号分隔正则且传 `wifiname` 时必填、`lat` ∈ [-90000000, 90000000]、`lng` ∈ [-180000000, 180000000] |
| #10 `addcheckinuserface` | **10 次/分钟**（本域最严格） | 图片数据 ≤1M（base64 后填入，**非** media_id） |
| #11 `getcheckin_daydata` | 100 次/分钟 | 跨度 ≤30 天；`useridlist` ≤100，单个 userid 1~64 字节 |
| #12 `getcheckin_monthdata` | 60 次/分钟 | `useridlist` ≤100（跨度上限未留档，缺口） |
| #13/#14 `get/setcheckinschedulist` | 60 次/分钟 | `useridlist` ≤100；endtime−starttime ≤1 个月；`day` ∈ 1–31 |
| #15 `get_hardware_checkin_data` | 注释无频率数值（缺口） | 跨度 ≤1 个月；`useridlist` ≤100，无效 userid **忽略不报错** |

## 9. 照抄官方原文的字段名与「示例覆盖参数表」的六处

| 处 | 事实 | 锁定点 |
|---|---|---|
| `lastest_time` | 日报 `summary_info` 的「最晚打卡时间」官方拼写照抄（非 `latest`） | CK5 |
| `acctivity_name` / `excepion_days_cnt` | 第三方月报拼写照抄（勿修正） | CK5 |
| `scheduleList` | 排班响应 `schedule.scheduleList` 为 **camelCase**，同页其余字段全 snake_case | CK5 |
| `"group.checkin_method_type"` | 员工打卡规则返回示例出现**字面量点号键**（官方原文如此），实际按嵌套字段建模 | CK5 + [CheckinGroup.cs](../../../Mud.Wechat.Work/Checkin/Rule/CheckinGroup.cs) 注释 |
| `base_info` vs `baseinfo` | 现代结构与第三方旧结构的日报/月报顶层键**只差一个下划线** | CK5 |
| 参数表类型笔误 | `work_se`、`uint3`、`unit32`、`checkindate.checkintime` 标注 `uint32` 实为对象数组、`allow_flex` 一处标 `uint32` 一处标 `bool` | 接口注释「照抄勿修正」+ CK5 |

日报与月报的加班结构**不可共用**：日报是 `ot_info`（`CheckinDayOtInfo`），月报是 `overwork_info`
（[CheckinMonthOverworkInfo.cs](../../../Mud.Wechat.Work/Checkin/Report/CheckinMonthOverworkInfo.cs)）——
后者三个「时长」字段单数（`workday_over_sec` / `restdays_over_sec` / `holidays_over_sec`）、
六个「记为调休/加班费」字段复数（`workdays_over_as_money` / `restdays_over_as_vacation` 等），照抄勿改。
`summary_info.regular_days` 仅参数表列出、官方示例未包含（接口注释留档）。

## 10. 三支「仅自建」的记录侧写端点：各自的坑

- **#8 补卡**：审批中的审批打卡**不能**补卡，补卡后原审批打卡信息会被清除。三键时间形态并存——
  `schedule_date_time`（应打卡日期当天 0 点时间戳）、`schedule_checkin_time`（相对该日 0 点的偏移秒，可省）、
  `checkin_time`（实际打卡时间戳，相对 `schedule_checkin_time` 体现正常/迟到/早退）。应答无业务负载。
- **#9 添加打卡记录**：`records[]` 官方必填为 `userid`、`checkin_time`、`location_title`、`location_detail`、`device_type`、`device_detail`；
  `device_type` 取 `1` 门禁 / `2` 考勤机（人脸、指纹）/ `3` 其他，`device_detail` 是设备品牌（**字符串**，≤40 字符）。
  官方参数表把顶层 `records` 与元素字段混在一张表且未标嵌套关系 ⇒ 嵌套以请求示例为准。
  若原卡点为审批打卡，更新后原审批打卡信息会被清除。
- **#10 录入人脸**：`userface` 是**人脸图片 base64**、不是 media_id；对已有人脸的用户**覆盖**原脸。
  官方「必须」列把 `userid` 与 `userface` 都标注为「否」（与常规必填预期不同，照抄）。
  专属错误码 `301021` 输入参数错误 / `301069` userid 无对应成员 / `301070` 系统错误 /
  `301071` 企业内有相似人脸（**此时人脸仍录入成功**）/ `301072` 图像数据错误请更换图片。
  **人脸与考勤位置均属个人数据**：示例用假值，不得写入日志、遥测或异常消息。

## 11. 设备打卡数据：路由挂 `hardware` 域，且与 DeviceData 域的「设备打卡」不是同一支

#15 的路由是 `/cgi-bin/hardware/get_hardware_checkin_data`（CK1 单独断言 `StartWith("/cgi-bin/hardware/")`）。
本仓另有一个独立业务域 `DeviceData`（智能设备数据，路由 `/cgi-bin/devicedata/get_checkin_data`，
守卫 [WechatDeviceDataContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDeviceDataContractGuards.cs)），
两支都返回名为 `checkindata` 的键但形态不同：本域记录仅 4 字段
（`userid` / `checkin_time` / `device_sn` / `device_name`，[HardwareCheckinRecord.cs](../../../Mud.Wechat.Work/Checkin/Device/HardwareCheckinRecord.cs)）。
**不得**把两支混为一谈或互相「归位」。

`filter_type` 决定 `starttime`/`endtime` 的语义：`1`（默认）按**打卡时间**过滤、`2` 按**设备上传打卡记录的时间**过滤。
取到的是考勤设备原始记录，**不含**企业微信 app 手机打卡记录，且包含「不符合打卡规则、未被打卡应用记录」的记录。

## 12. 令牌归属域、可见范围与权限口径

| 子接口族 | `TokenManagerKey` | 语义 |
|---|---|---|
| `IWechatWorkInternalCheckin*Service`（5 个自建子接口） | `WechatTokenManagerKeys.InternalAccessToken` | 自建应用自身凭证 |
| `IWechatWorkProviderCheckin*Service` / `IWechatWorkThirdPartyCheckin*Service`（各 5 个） | `WechatTokenManagerKeys.CorpAccessToken` | 授权企业级令牌：一个 (AppKey, authCorpId) 一份，scope = authCorpId |

路由键恒为 `Wechat.AccessToken`（CK3 对 20 个接口统一断言，**不得**按应用类型拆 `TokenType`）。
第三方/代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标授权企业作用域——
`permanent_code` 是凭据，只写参数名，**不得进日志、遥测或异常消息**（口径见
[AGENTS.md §5.1](../../../../../AGENTS.md) 与 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)）。

权限口径（各族父接口 remarks 一致留档）：自建应用须配置到「**打卡 - 可调用接口的应用**」中；
代开发与第三方应用须具有「打卡」权限；#15 的第三方还要求**设备型号关联该第三方应用**。
**自 2023-12-01 0 点起不再支持通过系统应用 secret 调用**（存量企业暂不受影响）。
读到的 `userid` 集合受**应用可见范围**约束；`userid`、考勤地理位置、wifi MAC、人脸与打卡备注都是个人数据，
示例一律用假值且不得落日志。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddCheckinApi());

// 自建应用注入自建子接口（父接口 IsAbstract，不可注入；记录/报表两族父接口还零端点）
public sealed class AttendanceSync(
    IWechatWorkInternalCheckinRecordService records,
    IWechatWorkInternalCheckinRuleService rules,
    IWechatWorkInternalCheckinReportService reports,
    IWechatWorkInternalCheckinScheduleService schedules)
{
    // 1) 按天取规则：datetime 是「那一天 0 点」的 Unix 时间戳，不是一段区间
    public async Task PullRulesAsync(DateOnly day, IReadOnlyList<string> userIds, CancellationToken ct)
    {
        long midnight = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local)
                              .ToUnixTimeSeconds();

        var option = await rules.GetCheckinOptionAsync(new GetCheckinOptionRequest
        {
            Datetime = midnight,
            UserIdList = userIds.ToList(),   // 官方：不超过 100 个，超过须分批
        }, ct);
        WechatWorkException.ThrowIfFailed(option, "/cgi-bin/checkin/getcheckinoption");

        List<CheckinOptionInfo> info = option.Info ?? new List<CheckinOptionInfo>();
        foreach (CheckinOptionInfo item in info)
        {
            // item.Group?.Grouptype：1 固定上下班 / 2 按班次 / 3 自由上下班
            // item.Group?.Checkindate 元素的 work_sec/off_work_sec 是「距 0 点秒数」，不是时间戳
        }
    }

    // 2) 按区间取打卡记录：跨度 ≤30 天、useridlist ≤100、600 次/分钟
    public async Task<List<CheckinRecord>> PullRecordsAsync(
        DateTimeOffset from, DateTimeOffset to, IReadOnlyList<string> userIds, CancellationToken ct)
    {
        var resp = await records.GetCheckinDataAsync(new GetCheckinDataRequest
        {
            Opencheckindatatype = 3,             // 1 上下班 / 2 外出 / 3 全部
            Starttime = from.ToUnixTimeSeconds(),
            Endtime = to.ToUnixTimeSeconds(),
            UserIdList = userIds.ToList(),
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/checkin/getcheckindata");

        List<CheckinRecord> list = resp.Checkindata ?? new List<CheckinRecord>();
        foreach (CheckinRecord r in list)
        {
            long ts = r.CheckinTime ?? 0;        // 秒级 Unix 时间戳（非 YYYY-MM-DD）
            // r.CheckinType / r.ExceptionType 为中文字符串（多个异常以分号间隔）
            // r.Lat / r.Lng 为实际经纬度的 1000000 倍（GCJ-02），仅位置打卡返回
        }
        return list;
    }

    // 3) 月报：overwork_info 的单数/复数键名照抄官方原文
    public async Task PullMonthAsync(DateTimeOffset from, DateTimeOffset to, string userId, CancellationToken ct)
    {
        var resp = await reports.GetCheckinMonthDataAsync(new GetCheckinMonthDataRequest
        {
            Starttime = from.ToUnixTimeSeconds(),
            Endtime = to.ToUnixTimeSeconds(),
            UserIdList = new List<string> { userId },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/checkin/getcheckin_monthdata");

        List<CheckinMonthData> datas = resp.Datas ?? new List<CheckinMonthData>();
        foreach (CheckinMonthData d in datas)
        {
            int? workdayOverSec = d.OverworkInfo?.WorkdayOverSec;      // 单数：workday_over_sec
            int? asMoney = d.OverworkInfo?.WorkdaysOverAsMoney;        // 复数：workdays_over_as_money
        }
    }

    // 4) 排班读写：yearmonth 是数字（202611），day 取 1–31，schedule_id = 0 表示休息
    public async Task SetRestDayAsync(
        long groupId, int yearMonth, string userId, int day, CancellationToken ct)
    {
        var resp = await schedules.SetCheckinScheduleListAsync(new SetCheckinScheduleListRequest
        {
            Groupid = groupId,
            Yearmonth = yearMonth,
            Items = new List<CheckinSetScheduleItem>
            {
                new CheckinSetScheduleItem { Userid = userId, Day = day, ScheduleId = 0 },
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/checkin/setcheckinschedulist");
    }

    // 5) 修改规则**无法**清空一级数组：必须走清空数组元素端点（1 spe_workdays / 2 spe_offdays / 3 wifimac_infos / 4 loc_infos）
    public async Task ClearLocationsAsync(long groupId, CancellationToken ct)
    {
        var resp = await rules.ClearCheckinOptionArrayFieldAsync(new ClearCheckinOptionArrayFieldRequest
        {
            Groupid = groupId,
            ClearField = new List<int> { 4 },
            EffectiveNow = true,
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/checkin/clear_checkin_option_array_field");
    }
}

// 第三方 / 代开发：企业级令牌 —— 同一个 #7 路由，但响应是旧字段结构
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var thirdParty = await thirdPartyRecords.GetCheckinDataAsync(new GetCheckinDataRequest
    {
        Starttime = fromUnixSeconds,
        Endtime = toUnixSeconds,
        UserIdList = new List<string> { "userid-placeholder" },
    }, ct);
    WechatWorkException.ThrowIfFailed(thirdParty, "/cgi-bin/checkin/getcheckindata");
    List<ThirdPartyCheckinRecord> oldShape = thirdParty.Checkindata ?? new List<ThirdPartyCheckinRecord>();
    // 这里的 checkin_type 是整数、经纬度键名为 location_title_lat / location_title_lng
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。
`#2 GetCorpCheckinOptionAsync` 须传**空实例**（`new GetCorpCheckinOptionRequest()`），官方请求体即 `{}`。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| #1 `getcheckinoption` 的频率上限 | 接口注释只留档 `useridlist` ≤100 与「按天获取」，**无频率数值** | 逐页核验官方 90263 后补 remarks，再更新 §8 表 |
| #12 月报（自建/代开发）的时间跨度上限 | 注释留档 `useridlist` ≤100 与 60 次/分钟，**跨度未标注**（第三方口径注释写 ≤30 天） | 不得从日报的 30 天借来当结论；核验后补注释 |
| #3–#6 规则写端点与 #15 设备打卡的频率数值 | 注释无数值（#2 有 60 次/分钟） | 逐页核验后补齐，勿在 SDK 侧造频控 |
| `checkin_type` / `exception_type` 中文字符串的完整取值集 | 注释列举「上班打卡 / 下班打卡 / 外出打卡」「时间异常 / 地点异常 / 未打卡 / wifi 异常 / 非常用设备」，**是否为全集未逐项核验** | 以 `string` 承载；勿写「等于某常量才算通过」的判定 |
| 第三方 `checkin_type` 的整数取值域 | 参数表标注字符串、返回示例为整数，本仓按示例建模为 `int?`（CK5），**取值全集未逐项核验** | 需要判定时先核验官方 94205，不得从自建侧借枚举 |
| 报表 `exception_infos.exception`、`base_info.record_type`、`grouptype`、`option_out_range`、`type` 等枚举全集 | `record_type` 注释给出 1/2/3/4/5/7（**无 6 值**）；其余取值集合本仓未逐项核验 | 以 `int?` 承载，出现新取值先核验再决定是否建枚举 |
| `sp_items[].vacation_id` 的签发面 | 注释说「可通过审批相关接口获取假期详情」⇒ 归属 `Approval` 邻域，**未在本域核验** | 需要假期字典时查审批域，勿在本域加端点 |
| `limit_aheadtime` / `limit_offtime` 的单位 | 官方未标注单位，本仓注释按示例量级推为毫秒 ⇒ **未逐项核验** | 不做单位换算；核验后写进 DTO remarks |
| `AddCheckinOptionRequest` 的 `group` 完整必填集与错误码全集 | 注释留档 301093/301094 与 §7 容量闸；其余错误码**未逐项核验** | 需要错误码表时逐页核验 98041 |
| `rid` / `newdatetime` / `party_ids` 三个键名 | 全仓 `Src/Work/` 检索**无**这三个键 ⇒ 属邻域或口头别名，本域对应物分别是 `groupid`/`schedule_id`/`timeline_id`、`datetime`、`party_id` | 不得据这些名字改 DTO；核验官方页原文拼写后再动 |
| 打卡附件 `mediaids` 的下载面 | 注释指向 `media/get`（媒体域），**归属邻域、未在本域核验** | 附件读取走媒体域，勿在本域加端点 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（≤100 / ≤30 天 / ≤200 条 / ≤1M / 512 字节 / 1024 / 40 字符 / lat-lng 范围 / 301xxx 错误码 / 2023-12-01 口径） | 本目录 20 份接口文件的 XML 注释 |
| 路由表与计数（5 族路由表 21 行、去重清单 15 条、hardware 域单列断言） | [WechatCheckinContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs) CK1（L50–L216，含五张 `*Routes` 表与 `BeEquivalentTo` 清单） |
| 父/子接口层级与声明面计数（父 0/0/1/2/1、自建 5/4/2/0/0、代开发 5/1/2/0/0、第三方 0/1/2/0/0） | CK2（L219–L317，含 `AssertFamily` 辅助） |
| `Wechat.AccessToken` + Query + `access_token` 绑定、接口数恰 20 | CK3（L320–L362） |
| DTO 契约面计数（104 型 = 规则 48 + 记录 9 + 报表 35 + 排班 9 + 设备 3）与 `SerializerClassName = Checkin` | CK4（L365–L419） |
| 字段形态与拼写照抄断言（`baseinfo`/`scheduleList`/`lastest_time`/`acctivity_name`/`id`/`party_id`/空请求体等） | CK5（L422–L513） |
| Query 令牌注入白名单（G5）与令牌归属键（TO1~TO3） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 接口命名空间二分（父 → `Mud.Wechat.Work.Interfaces`、可注入 → `Mud.Wechat.Work`） | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work/Checkin/](../../../Mud.Wechat.Work/Checkin/)（`Rule/`、`Record/`、`Report/`、`Schedule/`、`Device/`） |
| 上下文源生成文件（`internal`，示例不得引用） | `../../../Mud.Wechat.Work/Generated/CheckinJsonContext.g.cs` |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（`CheckinJsonContext.Default` 登记处） |
| 邻域「设备打卡」另一支 | [WechatDeviceDataContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDeviceDataContractGuards.cs)（`/cgi-bin/devicedata/get_checkin_data`） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与业务限制（频率 / 串行 / 覆盖删除 / 权限可见范围，格式照既有端点，**不得弱化**），
   **同批**改 [WechatCheckinContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)
   的五张路由表 + CK1 的 `BeEquivalentTo` 清单与 `HaveCount(15)`，并同批改 CK2 的 `AssertFamily` 计数（父与各子接口的声明面）。
   官方对第三方「暂不支持」的端点**不得补**（CK2 的第三方面计数即能力漂移守卫）。
2. 第三方文档页若与现代结构对齐（或旧结构补新字段）→ 先核验，再决定是否把三分支收敛回父接口；
   这会同时改 CK1 路由表行数、CK2 的 `parentDeclaredEndpointCount`（0 → n）与本 README §1 表格。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 **`Checkin`**，五族共用同一上下文）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 CK4 的类型计数（现 104）与端点级清单；`Generated/CheckinJsonContext.g.cs` 是生成物，**勿手改**。
4. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddCheckinApi()` XML（其族叙述须与本域计数一致）、
   可注入子接口的 `RegistryGroupName = "Checkin"`（父接口保持空组，N3）；CK3 的接口清单与 `HaveCount(20)` 同批改。
5. 令牌面改动一律不得拆 `TokenType`（恒 `Wechat.AccessToken`）；自建/非自建子接口的 `TokenManagerKey` 归属域须同批过
   [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）与
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5 白名单）。
6. 凭据与个人数据面改动不得引入真值：`access_token`、`permanent_code` 只写参数名；
   新增任何 Query 凭据参数须同批做「补脱敏词表 / 登记豁免（附追踪号）」二选一；
   `userid`、`lat`/`lng`、`wifimac`、`userface`、打卡备注与地点信息不得进日志、遥测或异常消息。
7. 本文与 [../../README.md](../../README.md) 的域内端点数、族叙述须同步更新（线 README 现仅列
   「规则 / 记录 / 报表 / 排班 / 设备打卡数据」五族名，无计数，改动时补计数）；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
