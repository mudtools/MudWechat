# Schedule 域 · 日程

企业微信「日程」域：本仓共落地 **13 条官方路由**（守卫 [WechatScheduleContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs)
的 SC1 / SC1b / SC1c 三张去重清单，**全部为 POST**），声明面 **13 个端点方法**
（SC2 父接口 4 + SC2b 父接口 7 + SC2c「零端点父接口 + 自建子接口 2」，日历与日程两族的六个应用类型子接口全部零端点），
按可注入面展开为 **35 个可调用端点**（日历族 3 × 4 + 日程族 3 × 7 + 待办族 1 × 2）。
分属三个接口族——管理日历族 **4 条路由**（[IWechatWorkScheduleCalendarService.cs](./Calendar/IWechatWorkScheduleCalendarService.cs) 承载 4 支公共端点 +
[Internal](./Calendar/IWechatWorkScheduleCalendarService_Internal.cs) / [Provider](./Calendar/IWechatWorkScheduleCalendarService_Provider.cs) / [ThirdParty](./Calendar/IWechatWorkScheduleCalendarService_ThirdParty.cs) 三个零端点空标记）、
管理日程族 **7 条路由**（[IWechatWorkScheduleService.cs](./Schedule/IWechatWorkScheduleService.cs) 承载 7 支公共端点 +
[Internal](./Schedule/IWechatWorkScheduleService_Internal.cs) / [Provider](./Schedule/IWechatWorkScheduleService_Provider.cs) / [ThirdParty](./Schedule/IWechatWorkScheduleService_ThirdParty.cs) 三个零端点空标记）、
待办族 **2 条路由**（[IWechatWorkScheduleTodoService.cs](./Todo/IWechatWorkScheduleTodoService.cs) **零端点父接口** + 唯一自建子接口
[IWechatWorkScheduleTodoService_Internal.cs](./Todo/IWechatWorkScheduleTodoService_Internal.cs) 承载 2 支——官方未对第三方 / 代开发开放，
SC2c 的子接口集合断言即能力漂移守卫）。共 10 份接口文件（SC3 断言接口数恰 10）。
契约面 35 个 DTO 型（SC4 `HaveCount(35)`；与「35 个可调用端点」只是数字巧合，两数来源不同、不得互推）。
注册组 `Schedule`，模块枚举 `WechatModule.Schedule`（[`AddScheduleApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 10 份接口文件的 XML 注释、
> [WechatScheduleContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs)（SC1~SC5）、
> [Src/Work/Mud.Wechat.Work/Schedule/](../../../Mud.Wechat.Work/Schedule/) 下的 DTO 声明；
> 路由与计数以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（SC3 对 10 个接口统一断言 `TokenType = Wechat.AccessToken` +
`InjectionMode.Query` + `Name = "access_token"`，MUD005 已接受风险，无法改 Header）；「开放面」指官方对该端点开放的应用类型，
由 SC2 / SC2b / SC2c 的父/子声明面计数锁死。归属域（自建 `InternalAccessToken` / 第三方与代开发 `CorpAccessToken`）见 §10。

### 管理日历族（SC1 `ScheduleCalendarRoutes`，4 条路由 / 4 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddCalendarAsync` | POST | `/cgi-bin/oa/calendar/add` | `access_token`(Query)；自建 + 第三方 + 代开发 | 93647/93702/96823；官方路由即 **add** 而非 create；响应可带 `fail_result.shares`（§9） |
| 2 | `UpdateCalendarAsync` | POST | `/cgi-bin/oa/calendar/update` | 同上 | 97716/97783/97758；覆盖式；`skip_public_range`（§3） |
| 3 | `GetCalendarAsync` | POST | `/cgi-bin/oa/calendar/get` | 同上 | 97717/97784/97759；`cal_id_list` ≤1000；响应管理员键为 **`adminis`**（SC5） |
| 4 | `DelCalendarAsync` | POST | `/cgi-bin/oa/calendar/del` | 同上 | 97718/97785/97760；无业务负载 → `WechatWorkResponse`（SC1 单独断言，不得建空响应 DTO） |

### 管理日程族（SC1b `ScheduleRoutes`，7 条路由 / 7 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 5 | `AddScheduleAsync` | POST | `/cgi-bin/oa/schedule/add` | `access_token`(Query)；自建 + 第三方 + 代开发 | 93648/93703/96824；自建可省 `cal_id`（写默认日历），第三方必填 |
| 6 | `UpdateScheduleAsync` | POST | `/cgi-bin/oa/schedule/update` | 同上 | 97720/97787/97761；覆盖式；重复日程三模式 96204/96198/96826 **同路由、非独立 API**（§4） |
| 7 | `AddScheduleAttendeesAsync` | POST | `/cgi-bin/oa/schedule/add_attendees` | 同上 | 97721/97789/97763；**增量式**；应答无业务负载 |
| 8 | `DelScheduleAttendeesAsync` | POST | `/cgi-bin/oa/schedule/del_attendees` | 同上 | 97722/97794/97764；**增量式**；应答无业务负载 |
| 9 | `ListSchedulesByCalendarAsync` | POST | `/cgi-bin/oa/schedule/get_by_calendar` | 同上 | 97723/97796/97765；分页 **offset + limit**（本仓多数域是 cursor + limit，§7） |
| 10 | `GetScheduleAsync` | POST | `/cgi-bin/oa/schedule/get` | 同上 | 97724/97798/97766；`schedule_id_list` ≤1000；已取消日程仍可取（`status = 1`） |
| 11 | `DelScheduleAsync` | POST | `/cgi-bin/oa/schedule/del` | 同上 | 97725/97799/97767；官方标题「**取消**日程」而路由为 `del`（SC1b 单独断言，勿归位为 cancel） |

### 待办族（SC1c `ScheduleTodoRoutes`，2 条路由 / 2 个声明方法）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 12 | `GetTodoAsync` | POST | `/cgi-bin/todo/get` | `access_token`(Query)；**仅自建** | 101524；**路由挂 `/cgi-bin/todo/` 段**（SC1c 断言，与日历/日程的 `oa/` 段不同） |
| 13 | `UpdateTodoAsync` | POST | `/cgi-bin/todo/update` | `access_token`(Query)；**仅自建** | 101534；按需增量；`attendees` ≤20；应答无业务负载 |

#12 / #13 只声明在自建子接口上：SC2c 断言 `IWechatWorkScheduleTodoService` 的继承链子接口集合**恰为**
`{ IWechatWorkInternalScheduleTodoService }`——补一个 Provider / ThirdParty 空标记子接口就会漂断这条能力漂移守卫。

## 1. 两支「查询」实为 POST；`add` / `del` 的命名与动词都不合直觉

SC1 / SC1b / SC1c 逐条断言 `typeof(PostAttribute)`，含仅查询语义的 `calendar/get`、`schedule/get`、
`schedule/get_by_calendar`、`todo/get` ⇒ 13 条路由**全部即 POST**，不得「顺手统一」为 GET。
两支反直觉命名由守卫单列锁定：

- #1 创建日历路由是 `/cgi-bin/oa/calendar/add`（**不是** `create`）；
- #11 官方标题是「取消日程」，路由却是 `/cgi-bin/oa/schedule/del`（**不是** `cancel`）——
  与 #4「删除日历」`calendar/del` 同形，语义差（取消 vs 删除）**只在官方标题层**，本域 DTO 未建模该差别。

无业务负载的 #4 / #7 / #8 / #11 / #13 返回类型必须是 `WechatWorkResponse`（SC1、SC1b、SC1c 三处
`ReturnType.Should().Be(typeof(Task<WechatWorkResponse>))`），不得新建空响应 DTO。

## 2. 两级定位键：`cal_id` → `schedule_id`（待办另有 `todo_id`）

| 键 | C# 属性 → JSON 名 | 类型 | 签发面 / 作用 |
|---|---|---|---|
| 日历 ID | `CalId` → `cal_id` | `string?` | #1 响应 `cal_id` 签发；#2 / #3 / #4 / #9 定位日历；#5 请求 `schedule.cal_id` 指定日程落哪个日历（该处官方注明**不多于 64 字节**且须为本应用创建的日历） |
| 日程 ID | `ScheduleId` → `schedule_id` | `string?` | #5 响应签发；#6/#7/#8/#10/#11 定位日程 |
| 待办 ID | `TodoId` → `todo_id` | `string?` | 非本域端点签发（本域只读/改，见 §11） |

三点必须分清：

- **`schedule_id` 在 `schedule` 对象里有两种必填性**：#5 创建时**不填**（由响应返回）、#6 更新时**必填**——
  两端点共用同一个扁平 [ScheduleInfo](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleInfo.cs)，差异只以可空性承载（SC5 锁定共用型）。
  日历侧同型：[ScheduleCalendar](../../../Mud.Wechat.Work/Schedule/Calendar/ScheduleCalendar.cs) 的 `cal_id` 仅更新必填。
- 本域**没有 `organizer` 键**（全仓 `Src/Work/` 检索无匹配）：管理权由 `admins` 承载（日历管理员 / 日程管理员，§8），
  待办的「谁建的」由响应侧 `creator` 承载（§11）。
- 本域**没有 `scheduleid` 与 `excludes` 两个键**：`scheduleid` / `schedulename` 是**打卡域**日报与第三方班次的键
  （`long?`，见 [CheckinDayRuleInfo.cs](../../../Mud.Wechat.Work/Checkin/Report/CheckinDayRuleInfo.cs)），
  与本文 `schedule_id`（`string?`，日程）**类型与语义都不同**；「排除日期」在本域叫
  `reminders.exclude_time_list`（[ScheduleRemindersInfo.cs](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleRemindersInfo.cs)，SC5 锁定）。

## 3. 更新是覆盖式，不是增量式：两支 `skip_*` 是唯一「别覆盖」的开关

父接口 remarks 对 #2 / #6 都写明「**更新操作是覆盖式，而不是增量式**」，两支各带一个跳过开关：

| 端点 | 开关 | 默认 | 语义 |
|---|---|---|---|
| #2 `calendar/update` | `skip_public_range`（`int?`，[UpdateScheduleCalendarRequest](../../../Mud.Wechat.Work/Schedule/Calendar/UpdateScheduleCalendarRequest.cs)） | `0` = **会**更新可订阅范围 | `1` = 本次不更新 `public_range` |
| #6 `schedule/update` | `skip_attendees`（`int?`，[UpdateScheduleRequest](../../../Mud.Wechat.Work/Schedule/Schedule/UpdateScheduleRequest.cs)） | `0` = **会**覆盖参与人 | `1` = 本次不动参与人 |

- 要**增量**加/减参与人，走 #7 / #8（`add_attendees` / `del_attendees`，两端点 XML 明示「增量式」），不要用 #6。
- **不可更新的属性**（接口 remarks + DTO 注释留档）：日历侧 `is_public`、`is_corp_calendar`（仅创建可传）、
  `set_as_default`（仅创建可传，且第三方不支持）；日程侧**创建者**与**所属日历 `cal_id`**。
  [ScheduleCalendar](../../../Mud.Wechat.Work/Schedule/Calendar/ScheduleCalendar.cs) 因此把创建侧三属性与更新侧 `cal_id` 塞进同一扁平型，
  靠「不传即不生效/官方忽略」约束——**未传字段是否被重置为默认值本仓未留档**（缺口表）。
- **已预约会议室的日程不能用 #6 改会议室关联字段**：官方清单共 11 个键
  （`start_time` / `end_time` / `is_whole_day` / `is_repeat` / `repeat_type` / `repeat_until` / `is_custom_repeat` /
  `repeat_interval` / `repeat_day_of_week` / `repeat_day_of_month` / `timezone`，父接口 remarks 逐字列出），
  须**先取消会议室预定**再更新（会议室预定侧见 [../MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService_Internal.cs](../MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)）。

## 4. 重复日程：`op_mode` 三档 ×「这一条」与「整条系列」的分叉

`op_mode` + `op_start_time` 同时出现在 #6 更新与 #11 取消两支（[UpdateScheduleRequest](../../../Mud.Wechat.Work/Schedule/Schedule/UpdateScheduleRequest.cs)、
[DelScheduleRequest](../../../Mud.Wechat.Work/Schedule/Schedule/DelScheduleRequest.cs)），
但**官方口径两套措辞不同，照抄不得统一**：

| `op_mode` | #6 更新（XML 原文） | #11 取消（XML 原文） |
|---|---|---|
| `0`（默认） | 默认**全部修改** | 默认**删除所有日程** |
| `1` | 仅修改**此日程** | 仅删除**此日程** |
| `2` | 修改**将来的所有日程** | 删除**本次及后续日程** |

- `op_start_time`（`long?`，Unix 时间戳）**仅当 `op_mode` 为 1 或 2 时有效**，且必须是该重复日程**某一次的开始时间**。
- 三个专属错误码（#6 的 remarks 留档）：非周期日程指定 `op_mode` 1/2 ⇒ **90485**；`op_start_time` 匹配不到某次周期开始时间 ⇒ **90482**；
  「仅修改此日程」下把重复性参数指定为重复 ⇒ **90484**。
- **分裂出新日程**：`UpdateScheduleResponse.ScheduleId` 注释写明「修改重复日程时为新产生的日程 ID；非全部周期修改时会修剪原日程并生成新日程」
  ⇒ 调用方**必须以响应返回值回写本地存储**，继续用旧 `schedule_id` 会指向被修剪后的系列。
- 特例（`UpdateScheduleRequest.OpStartTime` remarks）：`op_mode = 2` 且 `op_start_time` **等于原日程 `start_time`** 时演变为修改全部，**不分裂新日程**。
- 修改/删除重复日程的**特定一天或多天**时，原系列不会真被删，而是把这些日期写进响应的
  `reminders.exclude_time_list`（元素仅一键 `start_time`，`long?`，[ScheduleExcludeTime.cs](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleExcludeTime.cs)）
  ⇒ 「排除清单」只在**响应侧**出现（§6）。
- 「更新重复日程」是三份说明页（自建 96204 / 第三方 96198 / 代开发 96826），**与 #6 同一路由、不是独立 HTTP API**（SC1b 注释 + 接口 remarks）。

## 5. 时间字段全部是 Unix **秒级**时间戳；本域没有「距 0 点偏移秒」形态

对照打卡域的四种时间形态（[../Checkin/README.md](../Checkin/README.md) §3），本域**只有两种**：

| 形态 | 类型 | 字段（实测） |
|---|---|---|
| Unix **秒级**时间戳 | `long?` | `start_time`、`end_time`、`op_start_time`、`repeat_until`、参与者 `event_time`、待办 `create_time` / `end_time` / `remind_time`、排除日期 `exclude_time_list[].start_time` |
| **差值秒**（可为负） | `List<int>?` | `reminders.remind_time_diffs`：提醒时间与日程开始时间的差值，`-300` = 提前 5 分钟；取值范围 **-604800 ~ 86399** |

- `remind_before_event_secs`（`int?`）是**档位枚举式**取值：`0` 开始时 / `300` 前 5 分钟 / `900` 前 15 分钟 / `3600` 前 1 小时 / `86400` 前 1 天；
  全天日程另可 `32400`（当天 09:00）、`-172800`（前两天）、`-604800`（前 1 周）。官方注明该字段**后续将会废弃**，建议改用 `remind_time_diffs`；
  两者**仅一个生效**，`remind_time_diffs` 非空时**优先**（#5 remarks + DTO remarks 一致）。
- **全天日程靠 `is_whole_day` + 零点时间戳表达**，不是靠特殊日期串：`remind_time_diffs` 因此可能出现正数 `32400`
  （DTO remarks 明说「全天日程因 start_time 为 0 点时间戳」）。本域**没有任何** `YYYY-MM-DD` 字符串日期字段。
- `sequence`（`long?`，仅 #9 响应返回）是**自增编号**，不是时间戳（[ScheduleDetail.cs](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleDetail.cs)）。
- `timezone`（`int?`）是 **UTC 偏移小时数**（东正西负，默认 +8，范围 -12 ~ +12），与上面两形态都不同单位。

## 6. `reminders` 请求侧与响应侧**不同型**：`exclude_time_list` 只在响应侧

同一 JSON 键 `reminders` 由两个类承载，SC5 分别锁定：

| 面 | 类型 | 字段集 |
|---|---|---|
| 请求（#5 / #6 `schedule.reminders`） | [ScheduleReminders](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleReminders.cs) | `is_remind` / `is_repeat` / `remind_before_event_secs` / `remind_time_diffs` / `repeat_type` / `repeat_until` / `is_custom_repeat` / `repeat_interval` / `repeat_day_of_week` / `repeat_day_of_month` / `timezone`（**11 键，无 exclude_time_list**） |
| 响应（#9 / #10 `schedule_list[].reminders`） | [ScheduleRemindersInfo](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleRemindersInfo.cs) | 同上 **12 键**，多 `exclude_time_list` |

重复参数只在 `is_repeat = 1` 时有意义：`repeat_type` 取 `0` 每日 / `1` 每周 / `2` 每月 / `5` 每年 / `7` 工作日；
`is_custom_repeat = 0` 时系统按 `start_time` + `repeat_type` 自动推算，`= 1` 时才能用 `repeat_interval`（含义随 `repeat_type` 变，如「每周 + 3」= 每 3 周）、
`repeat_day_of_week`（`List<int>?`，1~7 = 周一至周日）、`repeat_day_of_month`（`List<int>?`，1~31）；
`repeat_until` 不填或 `0` = 一直重复。

响应侧同理共用：**#9 与 #10 的 `schedule_list` 都是 `List<ScheduleDetail>`**（SC5 断言两处 `PropertyType == typeof(List<ScheduleDetail>)`），
字段差异以可空性承载：`is_whole_day` 由 #10 返回、`sequence` 由 #9 返回。

## 7. 两支读端点的分工：按 ID 批取 vs 按日历增量扫

| 面 | 入参 | 上限 | 终止条件 / 坑 |
|---|---|---|---|
| #10 `schedule/get` | `schedule_id_list`（`List<string>?`，官方必填） | 一次最多 **1000** 条 | 需要事先掌握 ID；**已取消的日程也会返回详情**，须自查 `status` |
| #9 `schedule/get_by_calendar` | `cal_id` + `offset`（默认 0） + `limit`（默认 500，**1 ~ 1000**） | 单页 ≤1000 | 本域分页是 **offset + limit**（父接口 remarks 明写「区别于多数域的 cursor + limit」）；`schedule_list` 返回空即 offset 过大应终止；有新增日程可在原基础上继续增量；**仅可获取应用自己创建的日历**下的日程 |

两支都返回 `status`（`0` 正常 / `1` 已取消）与 `admins` / `attendees`，但 #9 侧的参与者注释注明「最多支持 **300** 人」，
而请求与 #10 侧写的是 **1000** 人（[ScheduleDetail.cs](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleDetail.cs) 注释照抄两处，二者关系未逐项核验）。
日历侧的对应读端点 #3 用 `cal_id_list`（≤**1000**，[GetScheduleCalendarRequest.cs](../../../Mud.Wechat.Work/Schedule/Calendar/GetScheduleCalendarRequest.cs)），
返回 `calendar_list`，其中管理员键为 **`adminis`**：官方参数表作 `admins`、三类应用文档页返回示例均作 `adminis`，
本仓**以示例为准**（SC5 `JsonNameShouldBe(ScheduleCalendarInfo.Adminis, "adminis")`，照抄勿「顺手修正」）。

## 8. 管理员 / 参与者 / 回执三张名单的口径

| 名单 | 键 | 约束（接口 remarks + DTO 注释） |
|---|---|---|
| 日历管理员 | `calendar.admins`（`List<string>?`） | **必须在通知范围成员（`shares`）中**，最多 **3** 人 |
| 日程管理员 | `schedule.admins` | 必须在共享成员列表中，最多 **3** 人 |
| 日历通知范围 | `calendar.shares[]`（`userid` + `permission`） | 最多 **2000** 人；`permission` 不填默认「可查看」，注释列 `1` 可查看 / `3` 仅查看闲忙状态 |
| 日历公开范围 | `calendar.public_range`（仅 `is_public = 1` 有效） | 成员 `userids` ≤**1000**、部门 `partyids` ≤**100** |
| 日程参与者 | `schedule.attendees[]` / #7 / #8 | 累计 ≤**1000** 人，单个参与者 ID ≤**64 字节** |

- **`partyids` 与打卡域不通用**：本域 `public_range` 是 `userids` / `partyids` **连写复数**、且部门为 `List<long>?`；
  打卡域 `range` 是 `party_id`（单数连写、`List<string>?`）/ `userid` / `tagid`（`List<int>?`）
  （[ScheduleCalendarPublicRange.cs](../../../Mud.Wechat.Work/Schedule/Calendar/ScheduleCalendarPublicRange.cs) vs
  [CheckinRange.cs](../../../Mud.Wechat.Work/Checkin/Rule/CheckinRange.cs)，拼写与元素类型两处都不同，跨域封装不得共用一个「部门 id 列表」模型）。
- 参与者元素在**请求侧只该有 `userid`**（官方必填），`response_status` / `event_time` **仅响应侧**
  （[ScheduleAttendee.cs](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleAttendee.cs) remarks）：
  `response_status` 为 `0` 未处理 / `1` 待定 / `2` 全部接受 / `3` 仅接受一次 / `4` 拒绝，#10 响应另可能出现
  `5` 接受本次及未来 / `6` 待定单次 / `7` 待定本次及未来 / `8` 拒绝单次 / `9` 拒绝本次及未来；
  `event_time` 是「对该时间点做出决策的时间」，配合 `response_status` 读。
- 「谁组织的」在本域**不可指定也不可查**（无 `organizer` 键）：创建者由应用令牌身份隐含，且父接口 remarks 明写「创建者…不可更新」。

## 9. 日历容量闸与「整体成功但局部失败」

日历族父接口 remarks 留档的官方容量闸：管理员须在通知范围成员中且 ≤**3**；通知范围成员 ≤**2000**；
公开范围成员 ≤**1000**、部门 ≤**100**；每人最多创建或订阅 **100** 个公共日历；每企业最多 **20** 个全员日历。
全员日历（`is_corp_calendar = 1`）本身也是公共日历的一种，**必须**给 `public_range`，且**不支持**指定颜色、默认日历、只读权限。

**部分失败形态**（SC5 锁定的契约陷阱）：#1 / #2 的成功响应仍可能携带
`fail_result.shares[]`（元素三键 `errcode` / `errmsg` / `userid`，[ScheduleCalendarFailShare.cs](../../../Mud.Wechat.Work/Schedule/Calendar/ScheduleCalendarFailShare.cs)）——
整体 `errcode == 0` **不等于**通知范围成员全部有效，判错之外必须**另查 `fail_result`**：

```csharp
if (resp.FailResult?.Shares is { Count: > 0 } shares) { /* 逐成员 errcode/errmsg/userid 处理 */ }
```

## 10. 令牌归属域、作用域切换与权限口径

| 子接口 | `TokenManagerKey` | 语义 |
|---|---|---|
| `IWechatWorkInternalScheduleCalendarService` / `IWechatWorkInternalScheduleService` / `IWechatWorkInternalScheduleTodoService` | `WechatTokenManagerKeys.InternalAccessToken` | 自建应用自身凭证 |
| `IWechatWorkProviderSchedule*Service` / `IWechatWorkThirdPartySchedule*Service`（日历 + 日程共 4 个） | `WechatTokenManagerKeys.CorpAccessToken` | 授权企业级令牌：一个 (AppKey, authCorpId) 一份，scope = authCorpId |

路由键恒为 `Wechat.AccessToken`（SC3 对 10 个接口统一断言，**不得**按应用类型拆 `TokenType`）。
SC3 还锁了待办族的两条方向性断言：**零端点父接口 `IWechatWorkScheduleTodoService` 不得声明归属域键**
（`NotBe(InternalAccessToken)` 且 `NotBe(CorpAccessToken)`），**自建待办子接口必须声明 `InternalAccessToken`**。
第三方 / 代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`
切到目标授权企业作用域（签名见 [IWechatAppContextSwitcher.cs](../../../Mud.Wechat.Work/Authentication/Interfaces/IWechatAppContextSwitcher.cs)，
一次性 `using`、释放时还原快照）——`permanent_code` 是凭据，只写参数名，**不得进日志、遥测或异常消息**
（口径见 [AGENTS.md §5.1](../../../../../AGENTS.md) 与 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) 的 TO1~TO3）。

`agentid`（`int?`）只出现在 #1 / #5 请求顶层：三方文档口径一致留档为「**仅旧的第三方多应用套件需填此参数**」，新套件不填。
读到的成员集合受**应用可见范围**约束，#9 另加一层「只能取本应用创建的日历」；
待办侧另有「可见范围需含根部门 + 仅可改本应用创建的待办」（§11）。
`userid`、日历通知成员、参与者回执与日程标题/描述/地址都可能是个人数据：示例一律用假值，不得落日志、遥测或异常消息。

## 11. 待办族：仅自建、路由段不同、增量语义与两处官方自相矛盾

形态（SC2c / SC3 锁定）：零端点父接口 + 唯一自建子接口，能力面只有「读详情」与「改状态」两支——
**本仓不承载待办的创建与删除**（`todo_id` 由邻域签发，见缺口表）。

- **增量语义**：`status` 与 `attendees` 均官方选填；`attendees` **不传或传空数组 = 不修改参与人列表**（接口 remarks + DTO 注释一致）。
  这是本域唯一的「不传即不动」写端点，与 #2 / #6 的覆盖式更新方向相反。
- **容量**：`attendees` 最多 **20** 个参与人；传入时元素两键 `userid` + `status` **均官方必填**。
- **权限**：调用应用的可见范围需要**包含根部门**；仅允许修改**当前应用创建**的待办；**不允许修改已删除**的待办。
- **两处官方自相矛盾（照抄，勿「修正」）**：
  ① 官方请求示例出现 `attendees.status = 2`，而参数表只列 `0` / `1` 两态 ⇒ **以参数表为准**，模型仍用 `int?` 承载（SC2c / DTO remarks + 守卫头部 remarks）；
  ② 同一 `status` 键在**响应**与**请求**的中文口径不同——获取详情为「0 - 已完成；1 - 进行中」，更新请求为「0 - 完成；1 - 进行中」，
  两侧注释分别照抄（[GetScheduleTodoResponse.cs](../../../Mud.Wechat.Work/Schedule/Todo/GetScheduleTodoResponse.cs)、
  [UpdateScheduleTodoRequest.cs](../../../Mud.Wechat.Work/Schedule/Todo/RequestModel/UpdateScheduleTodoRequest.cs)）。
- 待办 DTO 落在 `Schedule/Todo/RequestModel/` 子目录，但**命名空间仍是 `Mud.Wechat.Work.DataModels.Schedule`**
  （子目录仅作组织、不入命名空间，[AGENTS.md](../../../../../AGENTS.md) 已立则；SC4 的 35 型计数按命名空间统计）。

## 12. HTTP 面 `snake_case`，回调面 `PascalCase`：同一对象两张键表

本域对象在**事件推送面**另有一套键名（[CalendarChangedPayload.cs](../../../Mud.Wechat.Work.Callback/Events/Payloads/Schedule/CalendarChangedPayload.cs)、
[ScheduleChangedPayload.cs](../../../Mud.Wechat.Work.Callback/Events/Payloads/Schedule/ScheduleChangedPayload.cs)）：

| 事件键 | 载荷字段 | 文档号（自建 / 第三方 / 代开发） |
|---|---|---|
| `delete_calendar` | `CalId` | 97728 / 97806 / 97771 |
| `modify_calendar` | `CalId` | 97730 / 97808 / 97772 |
| `modify_schedule` | `CalId` + `ScheduleId` | 97731 / 97809 / 97773 |
| `delete_schedule` | `CalId` + `ScheduleId` | 97732 / 97810 / 97774 |
| `respond_schedule` | `CalId` + `ScheduleId` | 98111 / 98099 / 98110 |

要点（均出自该两文件与回调守卫 remarks）：**仅 API 创建的日历/日程产生事件**；本族**无 `ChangeType` 分组段**，信封 `Event` 节点即事件键；
回执事件（`respond_schedule`）的**回执结果不在推送报文里**，须按 `CalId` + `ScheduleId` 回查 #10 的 `response_status` 对齐；
修改/删除日程的 `FromUserName` 是「成员 UserID」，回执事件则是「进行回执操作的成员 UserID」。
守卫 CB2（`WechatCallbackEventTypes` 键全集）与 CB4（结构族载荷字段）分别锁定 5 个事件键与 `CalId` / `ScheduleId` 两键——
**改本域字段命名不会自动改回调面**，两面须同批核（见文末同批清单）。

## 13. 邻域对本域主键的四处消费（同名不同义，勿互相「归位」）

| 邻域 | 消费点 | 事实（出自邻域源码注释） |
|---|---|---|
| 会议 [`../Meeting/`](../Meeting/IWechatWorkMeetingService.cs) | [`CreateMeetingRequest.cal_id`](../../../Mud.Wechat.Work/Meeting/CreateMeetingRequest.cs) / `UpdateMeetingRequest.cal_id` / 详情响应 `cal_id` | 会议也挂日历：「不填则插入参与者的默认日历；第三方应用必须指定 `cal_id`」，且须为 `access_token` 对应应用创建的日历（≤64 字节） |
| 会议室 [`../MeetingRoom/`](../MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService_Internal.cs) | `oa/meetingroom/book_by_schedule` | 「`schedule_id` **仅可使用同应用创建的日程**」；会议室侧取消预定后，该 `schedule_id` 若未保留日历则**无法再经日程接口取详情** |
| 邮件 [`../Mail/`](../../../Mud.Wechat.Work/Mail/MailSchedule.cs) | `MailSchedule.schedule_id` | 邮件 compose_send 的日程/会议对象：「不传 `schedule_id` 是创建，传了是修改；`cancel` 必须带上」 |
| 打卡 [`../Checkin/`](../Checkin/README.md) | `schedule_id` / `scheduleid` | 打卡域的这两个键都是**班次**（`long?`），与本域「日程 ID」（`string?`）同名不同义 |

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddScheduleApi());

// 可注入面一律是应用类型子接口：三个族的父接口 IsAbstract = true、命名空间 Mud.Wechat.Work.Interfaces、不进注册组（N1~N3）。
public sealed class ScheduleSync(
    IWechatWorkInternalScheduleCalendarService calendars,
    IWechatWorkInternalScheduleService schedules,
    IWechatWorkInternalScheduleTodoService todos)
{
    // 1) 建日历：admins 必须落在 shares 内且 ≤3；成功也可能带 fail_result（§9）
    public async Task<string> CreateCalendarAsync(
        IReadOnlyList<string> memberIds, IReadOnlyList<string> adminIds, CancellationToken ct)
    {
        var resp = await calendars.AddCalendarAsync(new AddScheduleCalendarRequest
        {
            Calendar = new ScheduleCalendar
            {
                Summary = "示例日历",                       // 官方必填，1 ~ 128 字符
                Color = "#0000FF",                         // 官方必填，RGB 16 进制
                Description = "示例描述",                   // 0 ~ 512 字符
                Admins = adminIds.ToList(),                // ≤3，且须是 shares 成员
                Shares = memberIds.Select(id => new ScheduleCalendarShare
                {
                    Userid = id,                           // 个人数据：示例用假值，禁止落日志
                    Permission = 1,                        // 1 可查看 / 3 仅看闲忙
                }).ToList(),
                IsPublic = 0,                              // 仅创建可传，属性不可更新
                SetAsDefault = 1,                          // 设为本 access_token 应用的默认日历；第三方不支持
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/calendar/add");

        List<ScheduleCalendarFailShare> invalid =
            resp.FailResult?.Shares ?? new List<ScheduleCalendarFailShare>();
        foreach (ScheduleCalendarFailShare share in invalid)
        {
            // errcode == 0 仍可能有无效通知成员：逐条读 Errcode / Errmsg / Userid（userid 属个人数据，不得落日志）
        }
        return resp.CalId ?? throw new InvalidOperationException("创建日历未返回 cal_id");
    }

    // 2) 更新日历：覆盖式；不想动可订阅范围就显式 skip_public_range = 1
    public async Task UpdateCalendarAsync(string calId, string summary, CancellationToken ct)
    {
        var resp = await calendars.UpdateCalendarAsync(new UpdateScheduleCalendarRequest
        {
            SkipPublicRange = 1,
            Calendar = new ScheduleCalendar
            {
                CalId = calId,                             // 更新侧必填
                Summary = summary,
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/calendar/update");
    }

    // 3) 建日程：自建可省 cal_id（落应用默认日历）；提醒优先用 remind_time_diffs
    public async Task<string> CreateScheduleAsync(
        string calId, string attendeeId, DateTimeOffset start, CancellationToken ct)
    {
        var resp = await schedules.AddScheduleAsync(new AddScheduleRequest
        {
            Schedule = new ScheduleInfo
            {
                CalId = calId,                             // 第三方必填，≤64 字节
                Summary = "示例评审",                       // 0 ~ 128 字符，不填官方默认「新建事件」
                Description = "示例描述",                   // ≤1000 字符
                Location = "示例地点",                      // ≤128 字符
                StartTime = start.ToUnixTimeSeconds(),      // 官方必填，Unix 秒
                EndTime = start.AddHours(1).ToUnixTimeSeconds(),
                Attendees = new List<ScheduleAttendee>
                {
                    new ScheduleAttendee { Userid = attendeeId },   // 累计 ≤1000 人，单个 ≤64 字节
                },
                Reminders = new ScheduleReminders
                {
                    IsRemind = 1,
                    RemindTimeDiffs = new List<int> { -900 },      // 差值秒：提前 15 分钟；非空时优先
                    IsRepeat = 1,
                    RepeatType = 1,                                 // 0 每日/1 每周/2 每月/5 每年/7 工作日
                    IsCustomRepeat = 1,
                    RepeatInterval = 1,
                    RepeatDayOfWeek = new List<int> { 3 },          // 1 ~ 7 = 周一至周日
                    RepeatUntil = start.AddDays(90).ToUnixTimeSeconds(),   // 不填或 0 = 一直重复
                    Timezone = 8,                                   // UTC 偏移小时，-12 ~ +12
                },
            },
            // AgentId 仅旧的第三方多应用套件需填
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/schedule/add");
        return resp.ScheduleId ?? string.Empty;
    }

    // 4) 只改重复日程的某一次：op_mode = 1 + op_start_time 必须是该系列某次的开始时间；响应的 schedule_id 可能是新日程
    public async Task<string> MoveOneOccurrenceAsync(
        string scheduleId, long occurrenceStart, DateTimeOffset newStart, CancellationToken ct)
    {
        var resp = await schedules.UpdateScheduleAsync(new UpdateScheduleRequest
        {
            OpMode = 1,                                    // 0 全部 / 1 仅此日程 / 2 将来的所有
            OpStartTime = occurrenceStart,
            SkipAttendees = 1,                             // 0（默认）会用 schedule.attendees 整体覆盖
            Schedule = new ScheduleInfo
            {
                ScheduleId = scheduleId,                   // 更新侧必填
                StartTime = newStart.ToUnixTimeSeconds(),
                EndTime = newStart.AddHours(1).ToUnixTimeSeconds(),
                Summary = "示例评审（改期）",
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/schedule/update");
        return resp.ScheduleId ?? scheduleId;              // 非全部修改时会生成新日程：以响应为准回写本地
    }

    // 5) 增量加人（不要用 #6 覆盖参与人）
    public async Task AppendAttendeesAsync(string scheduleId, IReadOnlyList<string> newIds, CancellationToken ct)
    {
        var resp = await schedules.AddScheduleAttendeesAsync(new AddScheduleAttendeesRequest
        {
            ScheduleId = scheduleId,
            Attendees = newIds.Select(id => new ScheduleAttendee { Userid = id }).ToList(),
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/schedule/add_attendees");
    }

    // 6) 扫某日历下的日程：offset + limit，空页即终止；已取消（status = 1）也会返回
    public async Task<List<ScheduleDetail>> ScanCalendarAsync(string calId, CancellationToken ct)
    {
        List<ScheduleDetail> all = new List<ScheduleDetail>();
        int offset = 0;
        while (true)
        {
            var resp = await schedules.ListSchedulesByCalendarAsync(new ListSchedulesByCalendarRequest
            {
                CalId = calId,                             // 仅本应用创建的日历
                Offset = offset,
                Limit = 500,                               // 默认 500，取值 1 ~ 1000
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/schedule/get_by_calendar");

            List<ScheduleDetail> page = resp.ScheduleList ?? new List<ScheduleDetail>();
            if (page.Count == 0) break;                    // 官方：空即 offset 过大，应终止
            all.AddRange(page.FindAll(s => s.Status == 0));
            offset += page.Count;
        }
        return all;
    }

    // 7) 待办：读详情后按需改状态；attendees 不传 = 不动参与人列表（≤20，元素 userid + status 均必填）
    public async Task CompleteTodoAsync(string todoId, string attendeeId, CancellationToken ct)
    {
        var detail = await todos.GetTodoAsync(new GetScheduleTodoRequest { TodoId = todoId }, ct);
        WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/todo/get");
        List<ScheduleTodoAttendee> current =
            detail.Attendees ?? new List<ScheduleTodoAttendee>();
        // current.Count 官方上限 20；detail.Status 口径 0 已完成 / 1 进行中；creator / create_time / end_time / reminders[].remind_time 均只读
        if (current.Count > 20) { /* 参与人超限时须裁剪或分批，本仓不做本地拦截 */ }

        var resp = await todos.UpdateTodoAsync(new UpdateScheduleTodoRequest
        {
            TodoId = todoId,
            Status = 0,                                    // 更新请求口径：0 完成 / 1 进行中
            Attendees = new List<ScheduleTodoAttendee>
            {
                new ScheduleTodoAttendee { Userid = attendeeId, Status = 0 },
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/todo/update");
    }
}

// 第三方 / 代开发：同一批路由，令牌换成授权企业级 access_token（scope = authCorpId），调用前切作用域
using (switcher.UseCorpScope(appKey: "default", authCorpId, permanentCode))
{
    var resp = await thirdPartySchedules.AddScheduleAsync(new AddScheduleRequest
    {
        Schedule = new ScheduleInfo
        {
            CalId = calId,                                 // 第三方必填
            Summary = "示例评审",
            StartTime = start.ToUnixTimeSeconds(),
            EndTime = start.AddHours(1).ToUnixTimeSeconds(),
        },
    }, ct);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/schedule/add");
}
```

判错为单层普通信封（`errcode == 0` 即成功，`WechatWorkResponse.ErrorCode` / `ErrorMessage` 映射 `errcode` / `errmsg`）；
令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。
日历族 #1 / #2 还要**额外**读 `fail_result.shares`（§9），`errcode == 0` 不代表通知成员全部有效。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 全部 13 端点的**频率上限** | 本目录 10 份接口文件的 XML **无任何**「次/分钟」数值（打卡域有） | 未逐项核验；勿在 SDK 侧造频控，核验后补进 remarks 并在本文加「频率闸」表 |
| 覆盖式更新的**未传字段行为** | 注释只写「覆盖式而非增量式」+ 两支 `skip_*` 默认 0，未说明未传字段是被重置还是保留 | 危险项：写侧一律**先读后写**全量提交；核验 97716 / 97720 后补 §3 |
| `permission` 取值集是否完整 | DTO 注释只列 `1` 可查看 / `3` 仅查看闲忙，**无 2**；官方是否另有取值未逐项核验 | 以 `int?` 承载；不得写「非 1 即 3」的判定 |
| `get_by_calendar` 的「参与者最多 300 人」 | [ScheduleDetail.cs](../../../Mud.Wechat.Work/Schedule/Schedule/ScheduleDetail.cs) 注释照抄该数，而请求侧与 #10 侧写 1000 | 两处口径的关系未逐项核验；不得把 300 当请求侧上限 |
| `response_status` 的 5 ~ 9 是否只在 #10 出现 | 注释如此描述，未在 #9 页面逐项核验 | 解析侧按可空整数处理，出现新值先核验 |
| `todo_id` 的签发面 | 本域只有 `todo/get` + `todo/update`，**没有**创建/删除待办端点；官方是否存在待办 add/del 未在本域核验 | 需要创建/删除时先核验官方树，再决定是否扩本域（勿从第三方 SDK 转述借端点） |
| 待办 `attendees.status = 2`（官方示例） | 与参数表 0/1 冲突，本仓**以参数表为准**、用 `int?` 兼容 | 2 的语义未核验；判定逻辑只依赖 0/1 |
| 待办两页（101524 / 101534）在官方文档树的归属 | 本仓把待办挂在 Schedule 模块（注册组 + 守卫 SC1c/SC2c/SC3），路由段却是 `/cgi-bin/todo/` | 官方章节归属未逐项核验；模块归属改动须先过守卫再动本文 |
| `sequence` 的用途 | 注释仅「日程编号，自增数字，#9 响应返回」，是否可用于乐观锁/变更检测未留档 | 未逐项核验；勿据此实现冲突判定 |
| `remind_before_event_secs` 的废弃时间表 | 注释写「官方注明该字段后续将会废弃」，无具体时点/版本 | 未逐项核验；新代码直接用 `remind_time_diffs` |
| 会议室关联字段清单之外，房间侧其余字段能否经 #6 更新 | 父接口 remarks 逐字列出 11 个键禁改，其余未说明 | 未逐项核验；改动前先在测试企业验证并回填 §3 |
| 域叙述在 DI 与线 README 侧**滞后** | [WechatModule.cs](../../Extensions/WechatModule.cs) 的 `Schedule` remarks 与 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddScheduleApi()` XML **只写管理日历族 4 端点**；[../../README.md](../../README.md) 第 25 行只写「管理日历 / 管理日程」，漏待办族 | 以守卫 SC1/SC1b/SC1c（13 条路由）为准；改动本域时按文末清单同步这三处叙述 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（≤3 管理员 / ≤2000 shares / ≤1000 成员 / ≤100 部门 / ≤100 公共日历 / ≤20 全员日历 / ≤1000 参与者 / ≤64 字节 / ≤1000 批取 / limit 1~1000 / ≤20 待办参与人 / 90485-90482-90484） | 本目录 10 份接口文件的 XML 注释 |
| 三族路由表与去重清单（日历 4 / 日程 7 / 待办 2）+ 无负载端点必须返回 `WechatWorkResponse` | [WechatScheduleContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs) SC1（L143–L168）、SC1b（L174–L212）、SC1c（L218–L239）；表定义 L63–L137 |
| 接口层级与声明面计数（日历父 4 + 三子 0/0/0；日程父 7 + 三子 0/0/0；待办父 0 + 自建子 2）+ 子接口集合不得漂移 + `RegistryGroupName = "Schedule"` + `InheritedFrom` | SC2（L245–L259）、SC2b（L265–L279）、SC2c（L285–L298）、`AssertFamily`（L301–L331） |
| 10 个接口的 `Wechat.AccessToken` + Query + `access_token` 绑定；待办父接口禁声明归属域、自建待办必须声明 `InternalAccessToken` | SC3（L338–L376） |
| DTO 契约面计数（35 型 = 端点级 21 + 复用型 14）与 `SerializerClassName = Schedule` + 端点级清单 | SC4（L382–L436） |
| 字段名照抄与共用结构断言（`adminis` / `cal_id_list` / `calendar_list` / `skip_public_range` / `partyids` / `set_as_default` / `is_corp_calendar` / `skip_attendees` / `op_mode` / `op_start_time` / `remind_before_event_secs` / `remind_time_diffs` / `is_custom_repeat` / `repeat_until` / `repeat_day_of_week` / `repeat_day_of_month` / `exclude_time_list` / `response_status` / `event_time` / `sequence` / `todo_id` / `content` / `creator` / `create_time` / `end_time` / `reminders` / `remind_time`） | SC5（L442–L525） |
| Query 令牌注入白名单（本域 10 个接口名逐条在册） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) G5（日程段 L638–L658） |
| 令牌归属键与恢复链路（TO1~TO3） | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 接口命名空间二分（父 → `Mud.Wechat.Work.Interfaces`、可注入 → `Mud.Wechat.Work`） | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| 回调面 5 个事件键与 `CalId` / `ScheduleId` 载荷字段 | [WechatCallbackContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCallbackContractGuards.cs) CB2（日程族 5 键入册 L134–L138）与 CB4（断言 L488–L492）；载荷源文件 [CalendarChangedPayload.cs](../../../Mud.Wechat.Work.Callback/Events/Payloads/Schedule/CalendarChangedPayload.cs)、[ScheduleChangedPayload.cs](../../../Mud.Wechat.Work.Callback/Events/Payloads/Schedule/ScheduleChangedPayload.cs) |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work/Schedule/](../../../Mud.Wechat.Work/Schedule/)（`Calendar/` 13 型、`Schedule/` 17 型、`Todo/` + `Todo/RequestModel/` 5 型） |
| 上下文源生成文件（`internal`，示例不得引用） | `../../../Mud.Wechat.Work/Generated/ScheduleJsonContext.g.cs`（35 个 `[JsonSerializable]`） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（`ScheduleJsonContext.Default` 登记处在 L160） |
| 判错出口与作用域切换签名 | [WechatWorkException.cs](../../../Mud.Wechat.Work/Exceptions/WechatWorkException.cs)（`ThrowIfFailed<TResponse>`）、[IWechatAppContextSwitcher.cs](../../../Mud.Wechat.Work/Authentication/Interfaces/IWechatAppContextSwitcher.cs)（`UseCorpScope`） |
| 邻域同名键与消费点 | [../Checkin/README.md](../Checkin/README.md) §4–§5、[../MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService_Internal.cs](../MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)、[../../../Mud.Wechat.Work/Meeting/CreateMeetingRequest.cs](../../../Mud.Wechat.Work/Meeting/CreateMeetingRequest.cs)、[../../../Mud.Wechat.Work/Mail/MailSchedule.cs](../../../Mud.Wechat.Work/Mail/MailSchedule.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐三类官方文档号与业务限制（容量 / 覆盖 vs 增量 / 权限与可见范围 / 错误码，格式照既有端点，**不得弱化**），
   **同批**改 [WechatScheduleContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs)
   的三张路由表（`ScheduleCalendarRoutes` / `Routes` / `ScheduleTodoRoutes`）+ 各自 `HaveCount`（4 / 7 / 2）与 `BeEquivalentTo` 去重清单，
   并同批改 SC2 / SC2b / SC2c 的 `AssertFamily` 计数（父与各子接口声明面）。
   官方未对第三方 / 代开发开放的能力（待办族）**不得补子接口**——SC2c 的子接口集合断言就是这条能力漂移守卫。
2. 改任一「无业务负载」端点（#4 / #7 / #8 / #11 / #13）的返回类型 → 先核官方是否补了负载字段，
   否则不得新建空响应 DTO（三处守卫各自 `ReturnType.Should().Be(typeof(Task<WechatWorkResponse>))`）。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 **`Schedule`**，三族共用同一上下文）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 SC4 的类型计数（现 35）与端点级清单；`Generated/ScheduleJsonContext.g.cs` 是生成物（`internal`），**勿手改、示例不得引用**。
   新增子目录（如 `Todo/RequestModel/`）**不改变命名空间**（仍 `Mud.Wechat.Work.DataModels.Schedule`），SC4 按命名空间统计。
4. 改共用扁平结构（`ScheduleCalendar` / `ScheduleInfo` / `ScheduleDetail`）→ SC5 的三处 `PropertyType` / `List<>` 共用断言会同批漂，
   若确需拆分（例如响应侧补 `exclude_time_list` 到请求侧），先核官方参数表再拆，并同步本文 §2 / §6 / §7。
5. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddScheduleApi()` XML、
   可注入子接口的 `RegistryGroupName = "Schedule"`（父接口保持空组，N3）；SC3 的接口清单与 `HaveCount(10)` 同批改。
   **本域现存的叙述滞后（缺口表末行）应在此步一并补齐**：两处 DI XML 与 [../../README.md](../../README.md) 第 25 行都漏了管理日程族 / 待办族。
6. 令牌面改动一律不得拆 `TokenType`（恒 `Wechat.AccessToken`）；自建 / 非自建子接口的 `TokenManagerKey` 归属域须同批过
   [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）与
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5 白名单，本域 10 个接口名在册）。
7. 改本域主键或字段名（`cal_id` / `schedule_id`）时**必须同批核回调面**：`CalId` / `ScheduleId` 两个载荷键与
   `delete_calendar` / `modify_calendar` / `modify_schedule` / `delete_schedule` / `respond_schedule` 五个事件键由
   [WechatCallbackContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCallbackContractGuards.cs) 的 CB2 / CB4 锁，
   两面分属不同程序集，改一侧不会自动带动另一侧。
8. 凭据与个人数据面改动不得引入真值：`access_token`、`permanent_code` 只写参数名；
   新增任何 Query 凭据参数须同批做「补脱敏词表 / 登记豁免（附追踪号）」二选一；
   `userid`、`admins` / `shares` / `attendees` 名单、日程标题与地点、待办 `content` 不得进日志、遥测或异常消息。
9. 本文与 [../../README.md](../../README.md) 的域内端点数、族叙述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
