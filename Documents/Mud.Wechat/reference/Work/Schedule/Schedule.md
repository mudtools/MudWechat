# 管理日程

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Schedule` 域 · 管理日程族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkScheduleService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Schedule/IWechatWorkScheduleService.cs)（父，承载全部 7 支）＋ 自建 / 代开发 / 第三方三个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Schedule/IWechatWorkScheduleService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Schedule/IWechatWorkScheduleService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Schedule/IWechatWorkScheduleService_ThirdParty.cs)） |
| DTO 源文件 | [DataModels/Schedule/Schedule/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Schedule/Schedule/)（17 型，含请求 `ScheduleInfo` 与响应 `ScheduleDetail` 两个不同型） |
| 契约守卫 | [WechatScheduleContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs)（SC1b 路由表 / SC2b 父 7 + 三子 0 / SC3 / SC5）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 创建 [93648](https://developer.work.weixin.qq.com/document/path/93648) / [93703](https://developer.work.weixin.qq.com/document/path/93703) / [96824](https://developer.work.weixin.qq.com/document/path/96824)；更新 [97720](https://developer.work.weixin.qq.com/document/path/97720) / [97787](https://developer.work.weixin.qq.com/document/path/97787) / [97761](https://developer.work.weixin.qq.com/document/path/97761)；加人 [97721](https://developer.work.weixin.qq.com/document/path/97721) / [97789](https://developer.work.weixin.qq.com/document/path/97789) / [97763](https://developer.work.weixin.qq.com/document/path/97763)；减人 [97722](https://developer.work.weixin.qq.com/document/path/97722) / [97794](https://developer.work.weixin.qq.com/document/path/97794) / [97764](https://developer.work.weixin.qq.com/document/path/97764)；列表 [97723](https://developer.work.weixin.qq.com/document/path/97723) / [97796](https://developer.work.weixin.qq.com/document/path/97796) / [97765](https://developer.work.weixin.qq.com/document/path/97765)；详情 [97724](https://developer.work.weixin.qq.com/document/path/97724) / [97798](https://developer.work.weixin.qq.com/document/path/97798) / [97766](https://developer.work.weixin.qq.com/document/path/97766)；取消 [97725](https://developer.work.weixin.qq.com/document/path/97725) / [97799](https://developer.work.weixin.qq.com/document/path/97799) / [97767](https://developer.work.weixin.qq.com/document/path/97767)；更新重复日程说明 [96204](https://developer.work.weixin.qq.com/document/path/96204) / [96198](https://developer.work.weixin.qq.com/document/path/96198) / [96826](https://developer.work.weixin.qq.com/document/path/96826) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SC1b / SC2b / SC3 / SC5 + 7 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md)（2026-10-18）· 官方：本批未取文 |

## 1. 这个接口族做什么

日程的完整读写：E1 创建、E2 覆盖式更新（含重复日程三档操作）、E3/E4 增量增删参与人、E5 按日历扫列表、E6 按 ID 批取详情、E7 取消（路由叫 `del`）。日期容器由 [Calendar.md](./Calendar.md) 管理。

**不该用本族的场景**：要改日历本身（标题 / 成员 / 公开范围）用 [Calendar.md](./Calendar.md)；要预定会议室用 [MeetingRoom/Booking.md](../MeetingRoom/Booking.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发见 [Calendar.md §2.1](./Calendar.md) | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 第三方 / 代开发须先 `UseCorpScope(appKey, authCorpId, permanentCode)` 切企业作用域。
- 官方容量：日程管理员须在共享成员列表中且 ≤**3**；参与者累计 ≤**1000**，单个参与者 ID ≤**64** 字节；标题 0~128 字符（不填默认「新建事件」）、描述 ≤1000 字符、地址 ≤128 字符。
- 可见范围：E5 只能取**本应用创建的日历**下的日程；创建者与所属日历 `cal_id` **不可更新**。
- 已预约会议室的日程不能经 E2 改会议室关联字段，须先取消会议室预定；`userid`、参与人名单、日程标题 / 描述 / 地址属个人或工作数据，示例假值且不落日志。

### 2.3 官方开放的应用类型

E1–E7 对自建 / 第三方 / 代开发**等宽开放**，全部声明在公共父接口；三个子接口是零端点空标记。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkScheduleService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 7 | 否 |
| `IWechatWorkInternalScheduleService` / `...Provider...` / `...ThirdParty...` | `Mud.Wechat.Work` | 三个应用类型子接口（空标记） | 0 / 0 / 0 | 是 |

- 模块注册：`WechatModule.Schedule` / `AddScheduleApi()`；注册组名 `Schedule`。
- 端点索引：E1 `AddScheduleAsync` · E2 `UpdateScheduleAsync` · E3 `AddScheduleAttendeesAsync` · E4 `DelScheduleAttendeesAsync` · E5 `ListSchedulesByCalendarAsync` · E6 `GetScheduleAsync` · E7 `DelScheduleAsync`。

---

## 4. 端点

### E1. `AddScheduleAsync` — 创建日程

```csharp
[Post("/cgi-bin/oa/schedule/add")]
Task<AddScheduleResponse> AddScheduleAsync(
    [Body] AddScheduleRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/schedule/add` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`AddScheduleRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Schedule` | `schedule` | `ScheduleInfo?` | 是 | 日程主体，见下表 |
| `AgentId` | `agentid` | `int?` | 否 | 仅旧的第三方多应用套件需填 |

`ScheduleInfo`（创建侧）：`cal_id`（自建不填 = 应用默认日历；**第三方必填**，≤64 字节）、`admins`（≤3 且须在共享成员中）、`attendees[]`（请求侧只需 `userid`；累计 ≤1000）、`summary`、`description`、`location`、`start_time` / `end_time`（官方必填，Unix **秒级**）、`is_whole_day`（0/1）、`reminders`（请求型，11 键，无 `exclude_time_list`）、`schedule_id`（创建时不填，由响应返回）。

**响应参数**：`schedule_id`（`string?`）——后续 E2/E3/E4/E6/E7 的定位键。

**本端点注意事项**

- 提醒优先用 `remind_time_diffs`（差值秒，`-300` = 提前 5 分钟）；它与档位式 `remind_before_event_secs` **仅一个生效**，前者非空时优先（官方建议改用前者）。
- 重复参数只在 `is_repeat = 1` 时有意义：`repeat_type` 取 `0` 每日 / `1` 每周 / `2` 每月 / `5` 每年 / `7` 工作日；`is_custom_repeat = 0` 时由 `start_time + repeat_type` 推算，`= 1` 时才用 `repeat_interval` / `repeat_day_of_week` / `repeat_day_of_month`；`repeat_until` 不填或 `0` = 一直重复；`timezone` 是 UTC 偏移小时（默认 +8，-12~+12）。

### E2. `UpdateScheduleAsync` — 更新日程（覆盖式）

```csharp
[Post("/cgi-bin/oa/schedule/update")]
Task<UpdateScheduleResponse> UpdateScheduleAsync(
    [Body] UpdateScheduleRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/schedule/update` |
| 凭据 | Query `access_token` |

**请求参数**（`UpdateScheduleRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `SkipAttendees` | `skip_attendees` | `int?` | 否 | `1` = 本次不动参与人；默认 `0` = 用 `schedule.attendees` **整体覆盖** |
| `OpMode` | `op_mode` | `int?` | 否 | 重复日程：`0` 全部（默认）/ `1` 仅此日程 / `2` 将来的所有日程 |
| `OpStartTime` | `op_start_time` | `long?` | 否 | 仅 `op_mode` 为 1/2 时有效；必须是该系列**某一次**的开始时间 |
| `Schedule` | `schedule` | `ScheduleInfo?` | 是 | `schedule_id` 更新侧**必填** |

**响应参数**：`schedule_id`（`string?`）——**修改重复日程时可能是新产生的日程 ID**。

**本端点注意事项**

- 更新是**覆盖式**；增量加 / 减人走 E3 / E4。
- 非周期日程指定 `op_mode` 1/2 ⇒ **90485**；`op_start_time` 匹配不到某次周期开始时间 ⇒ **90482**；「仅修改此日程」下把重复性参数指定为重复 ⇒ **90484**。
- `op_mode = 2` 且 `op_start_time` 等于原 `start_time` 时演变为「修改全部」，**不分裂新日程**；其余非全部修改会修剪原系列并生成新日程 ⇒ **必须用响应值回写本地存储**。
- 已预约会议室的日程，11 个关联字段（`start_time` / `end_time` / `is_whole_day` / `is_repeat` / `repeat_type` / `repeat_until` / `is_custom_repeat` / `repeat_interval` / `repeat_day_of_week` / `repeat_day_of_month` / `timezone`）须先取消会议室预定才能改。

### E3. `AddScheduleAttendeesAsync` — 新增日程参与者（增量）

```csharp
[Post("/cgi-bin/oa/schedule/add_attendees")]
Task<WechatWorkResponse> AddScheduleAttendeesAsync(
    [Body] AddScheduleAttendeesRequest request, CancellationToken cancellationToken = default);
```

**请求参数**：`schedule_id`（官方必填）+ `attendees[]`（`userid` 必填；累计 ≤1000、单个 ≤64 字节）。**响应**：仅 `errcode` / `errmsg`（返回 `WechatWorkResponse`，不得新建空 DTO）。

**本端点注意事项**：**增量式**——只新增，不覆盖已有参与人；它与 E2 的 `skip_attendees = 0` 方向相反，别混用。

### E4. `DelScheduleAttendeesAsync` — 删除日程参与者（增量）

```csharp
[Post("/cgi-bin/oa/schedule/del_attendees")]
Task<WechatWorkResponse> DelScheduleAttendeesAsync(
    [Body] DelScheduleAttendeesRequest request, CancellationToken cancellationToken = default);
```

请求 / 响应形态同 E3，语义为**只移除**列出的 `attendees`，其余不动。

### E5. `ListSchedulesByCalendarAsync` — 获取日历下的日程列表

```csharp
[Post("/cgi-bin/oa/schedule/get_by_calendar")]
Task<ListSchedulesByCalendarResponse> ListSchedulesByCalendarAsync(
    [Body] ListSchedulesByCalendarRequest request, CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `CalId` | `cal_id` | `string?` | 是 | **仅本应用创建的日历** |
| `Offset` | `offset` | `int?` | 否 | 默认 0 |
| `Limit` | `limit` | `int?` | 否 | 默认 500，取值 **1~1000** |

**响应参数**：`schedule_list[]` → `ScheduleDetail`（含 `response_status`、`status`、`sequence`；**不含** `is_whole_day` 与 `exclude_time_list` 的完整形态见 E6）。

**本端点注意事项**：本域分页是 **offset + limit**（多数域是 cursor + limit）；`schedule_list` 返回空即 offset 过大应终止；**已取消日程也会返回**（`status = 1`），须自查。

### E6. `GetScheduleAsync` — 获取日程详情

```csharp
[Post("/cgi-bin/oa/schedule/get")]
Task<GetScheduleResponse> GetScheduleAsync(
    [Body] GetScheduleRequest request, CancellationToken cancellationToken = default);
```

**请求参数**：`schedule_id_list`（`List<string>?`，官方必填，一次 ≤**1000** 条）。

**响应参数**（`schedule_list[]` → `ScheduleDetail`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `schedule_id` / `cal_id` | 同名 | `string?` | 主键与所属日历 |
| `status` | `Status` | `int?` | `0` 正常 / `1` 已取消 |
| `start_time` / `end_time` | 同名 | `long?` | Unix 秒 |
| `is_whole_day` | `IsWholeDay` | `int?` | 全天日程靠它 + 零点时间戳表达 |
| `sequence` | `Sequence` | `long?` | **自增编号**（仅 E5 返回），不是时间戳 |
| `admins` / `attendees` | 同名 | `List<...>?` | 参与者可能带 `response_status` 与 `event_time` |
| `reminders` | `Reminders` | `ScheduleRemindersInfo?` | **响应型**：12 键，多 `exclude_time_list` |

**本端点注意事项**：已取消的日程也能取到详情（`status = 1`）；响应 `reminders.exclude_time_list` 是重复日程被排除的日期（元素仅 `start_time`）。

### E7. `DelScheduleAsync` — 取消日程

```csharp
[Post("/cgi-bin/oa/schedule/del")]
Task<WechatWorkResponse> DelScheduleAsync(
    [Body] DelScheduleRequest request, CancellationToken cancellationToken = default);
```

**请求参数**：`schedule_id`（必填）+ `op_mode`（`0` 删除所有 / 默认；`1` 仅删此日程；`2` 删除本次及后续）+ `op_start_time`（仅 1/2 有效）。

**响应**：仅 `errcode` / `errmsg`。

**本端点注意事项**：官方标题是「**取消**日程」但路由为 `del`（不是 `cancel`）；取消后记录仍可查（`status = 1`），与删除日历不同。

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Schedule;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddScheduleApi());

var schedules = provider.GetRequiredService<IWechatWorkInternalScheduleService>();
string calId = "CAL示例";
long start = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();

var add = await schedules.AddScheduleAsync(new AddScheduleRequest
{
    Schedule = new ScheduleInfo
    {
        CalId = calId,                                   // 第三方必填；自建可省（落默认日历）
        Summary = "示例评审",                             // 0 ~ 128 字符
        Description = "示例描述",                         // ≤1000 字符
        Location = "示例会议室",                          // ≤128 字符
        StartTime = start,                               // Unix 秒
        EndTime = start + 3600,
        Attendees = new List<ScheduleAttendee>
        {
            new ScheduleAttendee { Userid = "zhangsan" },   // 假值；请求侧只填 userid
        },
        Reminders = new ScheduleReminders
        {
            IsRemind = 1,
            RemindTimeDiffs = new List<int> { -900 },       // 提前 15 分钟；非空时优先生效
            IsRepeat = 1,
            RepeatType = 1,                                  // 0 每日 / 1 每周 / 2 每月 / 5 每年 / 7 工作日
            IsCustomRepeat = 1,
            RepeatInterval = 1,
            RepeatDayOfWeek = new List<int> { 3 },          // 1 ~ 7 = 周一至周日
            RepeatUntil = start + 90 * 86400L,              // 不填或 0 = 一直重复
            Timezone = 8,                                    // UTC 偏移小时
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(add, "/cgi-bin/oa/schedule/add");
string scheduleId = add.ScheduleId ?? throw new InvalidOperationException("创建日程未返回 schedule_id");

// 只改一次：op_mode = 1 + op_start_time 必须是该系列某次开始时间；响应的 schedule_id 可能是新日程
var update = await schedules.UpdateScheduleAsync(new UpdateScheduleRequest
{
    OpMode = 1,
    OpStartTime = start,
    SkipAttendees = 1,                                   // 0（默认）会用 attendees 整体覆盖
    Schedule = new ScheduleInfo
    {
        ScheduleId = scheduleId,                          // 更新侧必填
        StartTime = start + 3600,
        EndTime = start + 7200,
        Summary = "示例评审（改期）",
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(update, "/cgi-bin/oa/schedule/update");
scheduleId = update.ScheduleId ?? scheduleId;              // 非全部修改会生成新日程：以响应回写

// 增量加人（不要用 E2 覆盖参与人）
var addAttendees = await schedules.AddScheduleAttendeesAsync(new AddScheduleAttendeesRequest
{
    ScheduleId = scheduleId,
    Attendees = new List<ScheduleAttendee> { new ScheduleAttendee { Userid = "lisi" } },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(addAttendees, "/cgi-bin/oa/schedule/add_attendees");

// 按日历扫列表：offset + limit；空页即终止；已取消（status = 1）也会返回
var page = await schedules.ListSchedulesByCalendarAsync(new ListSchedulesByCalendarRequest
{
    CalId = calId,
    Offset = 0,
    Limit = 500,                                          // 1 ~ 1000
}, cancellationToken);
WechatWorkException.ThrowIfFailed(page, "/cgi-bin/oa/schedule/get_by_calendar");
foreach (ScheduleDetail item in page.ScheduleList ?? new List<ScheduleDetail>())
{
    if (item.Status != 0) continue;                       // 0 正常 / 1 已取消
    logger.LogInformation("日程 {ScheduleId} 开始于 {StartTime}", item.ScheduleId, item.StartTime);
}

// 按 ID 批取详情（≤1000）
var detail = await schedules.GetScheduleAsync(
    new GetScheduleRequest { ScheduleIdList = new List<string> { scheduleId } }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/schedule/get");

// 取消（路由是 del，不是 cancel）：0 全部 / 1 仅此 / 2 本次及后续
var del = await schedules.DelScheduleAsync(new DelScheduleRequest
{
    ScheduleId = scheduleId,
    OpMode = 0,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(del, "/cgi-bin/oa/schedule/del");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 覆盖 vs 增量 | E2 默认覆盖参与人；增量用 E3/E4 | 想加一个人却走 E2 → 其余参与人被清空 |
| `skip_attendees` vs `skip_public_range` | 前者属 E2，后者属日历更新 E2 | 参数串族 |
| 「取消」路由 `del` | 官方标题是取消，路由不是 `cancel` | 自造路由 404 |
| E2 返回的新 `schedule_id` | 非全部周期修改会分裂新日程 | 继续用旧 ID → 指向被修剪的系列 |
| `op_start_time` | 必须是**某一次**的开始时间，不是任意时间 | 90482 |
| 已取消仍可查 | E5/E6 都返回 `status = 1` 的记录 | 把已取消日程当有效数据 |
| 请求 / 响应 `reminders` 不同型 | 请求 11 键、响应 12 键（多 `exclude_time_list`） | 往请求传 `exclude_time_list` 静默忽略 |
| 时间单位 | 全部 Unix **秒**；`sequence` 不是时间而是自增号 | 把 `sequence` 当时间戳 |
| 读取也是 POST | E5/E6 官方即 POST | 改 GET 打红 SC1b |
| 全天日程 | 用 `is_whole_day` + 零点时间戳表达，无日期字符串 | 传 `YYYY-MM-DD` |

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| **90485** | 官方业务码（E2 注释留档） | 非周期日程指定 `op_mode` 1/2 | 调用侧逻辑错误 |
| **90482** | 同上 | `op_start_time` 匹配不到周期开始时间 | 先 E6 读回该系列的真实开始时间 |
| **90484** | 同上 | 「仅修改此日程」下把重复性参数指定为重复 | 改单次时不要传重复参数 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈 |

其余业务码以官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）为准。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `get_by_calendar` 参与者「最多 300 人」 | `ScheduleDetail` 注释照抄该数，而请求侧与 E6 侧写 1000 ⇒ 两处关系未逐项核验 | 官方 97723 |
| `response_status` 5~9 | 注释称仅 E6 出现 ⇒ 未逐项核验 | 官方 97724 |
| 频率上限 | 接口注释无「次/分钟」数值 ⇒ 未逐项核验 | 官方访问频率说明 |
| 覆盖式更新未传字段行为 | 未说明被重置还是保留 ⇒ 未逐项核验；写侧先读后写 | 官方 97720 |
| 会议室关联 11 字段之外的其余字段 | 官方只列禁改清单 ⇒ 未逐项核验 | 官方 97720 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md)
- 兄弟族：[Calendar.md](./Calendar.md) · [Todo.md](./Todo.md)
- 邻域：[MeetingRoom/Booking.md](../MeetingRoom/Booking.md)（`book_by_schedule` 消费本域 `schedule_id`）· [Mail/Receive.md](../Mail/Receive.md)（邮件日程对象同键）
- 官方页：[创建日程 93648](https://developer.work.weixin.qq.com/document/path/93648) · [更新日程 97720](https://developer.work.weixin.qq.com/document/path/97720) · [更新重复日程说明 96204](https://developer.work.weixin.qq.com/document/path/96204) · [获取日程详情 97724](https://developer.work.weixin.qq.com/document/path/97724)
