# 打卡排班（排班表读取 / 排班）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Checkin` 域 · 打卡排班族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCheckinScheduleService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Schedule/IWechatWorkCheckinScheduleService.cs)（承载 2 端点）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Schedule/IWechatWorkCheckinScheduleService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Schedule/IWechatWorkCheckinScheduleService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Schedule/IWechatWorkCheckinScheduleService_ThirdParty.cs) |
| DTO 源文件 | [DataModels/Checkin/Schedule/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Schedule/)（9 型） |
| 契约守卫 | [WechatCheckinContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)（CK1 路由 / CK2 层级与开放面 / CK3 令牌绑定 / CK4 JSON 上下文 / CK5 拼写照抄） |
| 官方文档 | 获取排班 93380（自建）/ 94208（第三方）/ 96500（代开发）· 设置排班 93385 / 94209 / 96501 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 首验 + 2026-10-30 复验：在本仓临时 Web SDK 工程（`net8.0`）与全仓 82 段可运行示例合并逐字编译，0 错误（本篇 2 段；本篇的 `// 承接` 片段示例已补齐自足前置（自带 using 与服务实例解析），现同样参与编译；接口签名摘录块与 json 块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CK1~CK5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

管「**谁在哪一天上哪个班次**」：

- **E1 读取排班表**：给定成员 + 时间区间，返回每人每天的班次（班次 id、班次名、该班次的上下班时段）。
- **E2 设置排班表**：给定打卡规则 id + 年月 + 明细（成员 / 日号 / 班次 id），批量写入。

**只对「按班次上下班」规则生效**（`grouptype = 2`）。固定上下班、自由上下班的成员调 E1 拿不到排班，调 E2 会被官方拒绝——这类成员的班次是规则本身决定的，不需要排班。

与兄弟族的分工：

| 族 | 管什么 | 关系 |
|---|---|---|
| [Rule.md](./Rule.md) 打卡规则 | 班次**模板**（`schedulelist[].schedule_id` 定义每个班次的时段） | 排班引用的 `schedule_id` 必须先在规则里存在 |
| **排班（本族）** | 模板**怎么落到人头上某一天** | 读写排班表 |
| [Record.md](./Record.md) 打卡记录 | 实际打了几点的卡 | 排班是「计划」，记录是「事实」 |

和规则族/设备族一样，本族**互操作端点全部收敛在公共父接口**：三类应用的请求与响应结构完全一致（第三方没有另起一套旧结构），因此父接口承载 2 个公共端点，三个应用类型子接口是**零端点空标记**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider` / `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（`Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入 | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

第三方/代开发先切企业作用域（`permanent_code` 不得进日志）：

```csharp
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var plan = await scheduleService.GetCheckinScheduleListAsync(request, ct);
}
```

### 2.2 权限与业务前置

- 官方权限：自建须配置到「打卡 - 可调用接口的应用」；代开发/第三方须具有「打卡」权限。
- **仅「按班次上下班」规则**（`grouptype = 2`）的成员可读写排班。
- `useridlist` **≤ 100 个**，超过须分批。
- E1 的 `endtime − starttime` **≤ 1 个月**；E2 一次只处理**一个 `yearmonth`**。
- E2 的 `day` 取 **1–31**，与 `yearmonth` 联合组成唯一日期；`schedule_id = 0` 表示**休息**。
- 只能读写应用**可见范围**内的成员；越界由官方报错，SDK 不预判。
- 排班表里的 `userid`、`groupname`、`schedule_name` 属个人/组织数据，示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1 `getcheckinschedulist` | ✅ | ✅ | ✅ |
| E2 `setcheckinschedulist` | ✅ | ✅ | ✅ |

三类应用**全部开放且结构一致**——这正是父接口承载两端点、子接口零端点的原因。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCheckinScheduleService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 2 | 否 |
| `IWechatWorkInternalCheckinScheduleService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderCheckinScheduleService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyCheckinScheduleService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

四个接口**都**标 `[Token(TokenType = WechatTokenTypes.AccessToken, InjectionMode = Query, Name = "access_token")]`；归属域键只写在三个子接口上。注册组 `Checkin`，模块枚举 `WechatModule.Checkin`，装配入口 `AddCheckinApi()`。

**注入的是子接口，不是父接口。** 父接口 `IsAbstract = true`、不注册 DI，`GetRequiredService<IWechatWorkCheckinScheduleService>()` 会抛异常。

## 4. 端点

### E1. `GetCheckinScheduleListAsync` — 获取打卡人员排班信息

**签名**

```csharp
Task<GetCheckinScheduleListResponse> GetCheckinScheduleListAsync(
    [Body] GetCheckinScheduleListRequest request,
    CancellationToken cancellationToken = default);
```

- 声明接口：`IWechatWorkCheckinScheduleService`（父接口，经任一子接口注入后调用）

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/checkin/getcheckinschedulist` |
| 请求体 | `GetCheckinScheduleListRequest` |
| 令牌 | Query `access_token`（`Wechat.AccessToken`） |
| 频率 | 60 次/分钟 |

**请求参数**（`GetCheckinScheduleListRequest`）

| 字段（JSON） | 属性（C#） | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `useridlist` | `UserIdList` | `List<string>?` | 是 | 要取排班的成员，**≤ 100 个** |
| `starttime` | `Starttime` | `long?` | 是 | 开始时间，Unix **秒级**时间戳 |
| `endtime` | `Endtime` | `long?` | 是 | 结束时间，Unix 秒级时间戳；与 `starttime` 跨度 **≤ 1 个月** |

**响应参数**（`GetCheckinScheduleListResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `schedule_list` | `ScheduleList` | `List<CheckinScheduleListInfo>?` | 每人每月一条 |
| `schedule_list[].userid` | `Userid` | `string?` | 成员 userid |
| `schedule_list[].yearmonth` | `Yearmonth` | `int?` | 年月数字（如 `202610`），**不是字符串** |
| `schedule_list[].groupid` | `Groupid` | `long?` | 打卡规则 id |
| `schedule_list[].groupname` | `Groupname` | `string?` | 打卡规则名 |
| `schedule_list[].schedule.scheduleList` | `Schedule.ScheduleList` | `List<CheckinDaySchedule>?` | 个人排班表；**键名是 camelCase** |
| `…scheduleList[].day` | `Day` | `int?` | 当月第几天（1–31） |
| `…scheduleList[].schedule_info.schedule_id` | `ScheduleId` | `long?` | 当日班次 id（`0` + 名称「休息」= 休息日） |
| `…schedule_info.schedule_name` | `ScheduleName` | `string?` | 班次名称 |
| `…schedule_info.time_section[]` | `TimeSection` | `List<CheckinScheduleTimeSection>?` | 班次上下班时段；休息日为**空数组** |
| `…time_section[].id` | `Id` | `int?` | 时段 id；**键名是 `id`，不是 `time_id`** |
| `…time_section[].work_sec` / `off_work_sec` | 同名 | `int?` | 上班 / 下班时间，**距当天 0 点的偏移秒** |
| `…time_section[].remind_work_sec` / `remind_off_work_sec` | 同名 | `int?` | 上/下班提醒时间，同样为偏移秒 |

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Checkin;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddCheckinApi());

var schedules = provider.GetRequiredService<IWechatWorkInternalCheckinScheduleService>();

var start = new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Local));
var end = new DateTimeOffset(new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Local));

var scheduleResp = await schedules.GetCheckinScheduleListAsync(
    new GetCheckinScheduleListRequest
    {
        UserIdList = new List<string> { "zhangsan", "lisi" },
        Starttime = start.ToUnixTimeSeconds(),
        Endtime = end.ToUnixTimeSeconds(),
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(scheduleResp, "/cgi-bin/checkin/getcheckinschedulist");

foreach (var entry in scheduleResp.ScheduleList ?? new List<CheckinScheduleListInfo>())
{
    foreach (var dayPlan in entry.Schedule?.ScheduleList ?? new List<CheckinDaySchedule>())
    {
        var info = dayPlan.ScheduleInfo;
        logger.LogInformation(
            "成员 {UserId} {YearMonth} 第 {Day} 日班次 {ScheduleName}（id={ScheduleId}），时段 {SectionCount} 组",
            entry.Userid, entry.Yearmonth, dayPlan.Day,
            info?.ScheduleName, info?.ScheduleId, info?.TimeSection?.Count ?? 0);
    }
}
```

**本端点注意事项**

- **`time_section` 里是偏移秒不是时间戳**：`work_sec = 32400` 表示 9:00（当天 0 点 + 9 小时），不是 Unix 时间。
- **休息日**：`schedule_id = 0`、`schedule_name = "休息"`、`time_section` 为**空数组**——别把空数组当「数据缺失」。
- **`schedule.scheduleList` 是 camelCase**，同页其余字段全是 snake_case，官方原文如此（CK5 锁定）；C# 属性名 `ScheduleList` 映射到 `scheduleList`。
- 区间跨度按「1 个月」理解（官方注释原文），跨月取数请拆成多次调用。
- 频率 60 次/分钟，配合 `useridlist` 的 100 上限做二维分批。

### E2. `SetCheckinScheduleListAsync` — 为打卡人员排班

**签名**

```csharp
Task<WechatWorkResponse> SetCheckinScheduleListAsync(
    [Body] SetCheckinScheduleListRequest request,
    CancellationToken cancellationToken = default);
```

- 声明接口：`IWechatWorkCheckinScheduleService`（父接口）

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/checkin/setcheckinschedulist` |
| 请求体 | `SetCheckinScheduleListRequest` |
| 令牌 | Query `access_token`（`Wechat.AccessToken`） |
| 频率 | 60 次/分钟 |
| 应答 | 仅 `errcode` / `errmsg`，无业务负载 |

**请求参数**（`SetCheckinScheduleListRequest`）

| 字段（JSON） | 属性（C#） | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `groupid` | `Groupid` | `long?` | 是 | 打卡规则 id（须为「按班次上下班」规则） |
| `yearmonth` | `Yearmonth` | `int?` | 是 | 年月数字（如 `202610`），一次一个 |
| `items[]` | `Items` | `List<CheckinSetScheduleItem>?` | 是 | 排班明细 |
| `items[].userid` | `Userid` | `string?` | 是 | 成员 userid |
| `items[].day` | `Day` | `int?` | 是 | 当月第几天，**1–31** |
| `items[].schedule_id` | `ScheduleId` | `long?` | 是 | 该规则下的班次 id；**`0` 代表休息** |

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException
using Mud.Wechat.Work.DataModels.Checkin;      // 请求/响应 DTO
// 片段自足前置：与 E1 使用同一个自建子接口实例；groupid 生产上取 E1 响应里的 entry.Groupid。
var schedules = provider.GetRequiredService<IWechatWorkInternalCheckinScheduleService>();

var setResp = await schedules.SetCheckinScheduleListAsync(
    new SetCheckinScheduleListRequest
    {
        Groupid = 987654321,
        Yearmonth = 202610,
        Items = new List<CheckinSetScheduleItem>
        {
            new() { Userid = "zhangsan", Day = 1, ScheduleId = 123 }, // 白班
            new() { Userid = "zhangsan", Day = 2, ScheduleId = 456 }, // 夜班
            new() { Userid = "zhangsan", Day = 3, ScheduleId = 0 },   // 休息
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(setResp, "/cgi-bin/checkin/setcheckinschedulist");
```

**本端点注意事项**

- **`schedule_id` 必须来自该 `groupid` 规则下的班次**（规则 `schedulelist[]` 的班次 id），跨规则借用会被拒。
- **`schedule_id = 0` 是「休息」的合法取值**，不是「无值」——想清空某天排班就显式写 0。
- **官方参数表把 `groupid`/`yearmonth` 与 `items` 元素字段混在一张表**且未标嵌套关系，嵌套以官方请求示例为准（SDK 已按示例建模为 `items[]`）。
- 一次请求只落一个 `yearmonth`；给某人排多个月要分多次调用。
- 应答无业务负载：只需判 `errcode`。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `time_section[].id` vs 规则的 `time_id` | 排班响应叫 `id`，规则请求叫 `time_id`（CK5 锁定） | 「顺手统一」→ 字段取不到 |
| 排班的 `schedule_id` vs 规则的 `schedulelist[].schedule_id` | 同名同义（班次模板 id），排班只是**引用** | 以为排班侧是另一套 id → 传错值 |
| `schedule_id = 0` | 请求侧 = 休息；响应侧同样配「休息」名 | 当缺失值过滤掉 → 休息日排不进去 |
| `yearmonth` | `int?` 数字（`202610`） | 当字符串 `"2026-10"` → 反序列化为 0 |
| `scheduleList`（camelCase） | 个人排班表的键名唯一 camelCase | 写成 `schedule_list` → 拿不到任何日排班 |
| `work_sec` 等 | 距当天 0 点**偏移秒** | 当时间戳格式化 → 1970 年 |
| 排班区间 vs 规则 `datetime` | 排班用 `starttime`/`endtime` 区间；规则用单日 0 点 `datetime` | 互借参数名 → 官方报参数错误 |
| 只有「按班次上下班」 | 固定/自由上下班没有排班 | 对 `grouptype ≠ 2` 调用 → 空结果或被拒 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 301021 | 官方业务码 | userid 错误 | 成员不存在 / 越界 |
| 301070 | 官方业务码 | 系统错误 | 偶发，可退避重试 |
| 301075 | 官方业务码 | 输入参数错误 | 参数/跨度/规则类型不合法 |
| 42001 / 40014 / 42007 | `ExpiredAccessToken` / `InvalidAccessToken` / `RelatedAccessTokenInvalid` | 令牌过期/失效 | 自愈集合 |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 套件/服务商令牌失效 | 本族不消费；出现即凭据族错配 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

其余码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E2 单次 `items` 条数上限 | 接口注释只留频率 60 次/分钟与 `day` 1–31，**条数上限无数值** ⇒ **未逐项核验** | 官方 93385 |
| E2 是否支持跨月批量 | 请求体只有单个 `yearmonth`，多个月须多次调用（已按结构推定）；官方是否有批量语义**未核验** | 官方 93385 |
| 未排班日的响应形态 | 注释只说明「休息日 → `schedule_id = 0` + 空 `time_section`」；**完全未排班**的日期是否出现在 `scheduleList` 中**未留档** | 官方 93380 |
| `yearmonth` 合法范围 | 建模为 `int?`，官方合法区间（能否设置次月/跨年）未留档 | 官方 93385 |
| 官方文档正文 | 本文只摘录本仓 XML 注释；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md)（§4 `groupid`/`schedule_id`/`time_id`/`id` 分工、§8 频率表）
- 兄弟族：[Rule.md](./Rule.md)（打卡规则）· [Record.md](./Record.md)（打卡记录）· [Report.md](./Report.md)（日报/月报）· [Device.md](./Device.md)（设备打卡数据）
- 官方页：[获取打卡人员排班信息（自建 93380）](https://developer.work.weixin.qq.com/document/path/93380) · [为打卡人员排班（自建 93385）](https://developer.work.weixin.qq.com/document/path/93385) · [第三方 94208](https://developer.work.weixin.qq.com/document/path/94208) · [第三方 94209](https://developer.work.weixin.qq.com/document/path/94209)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
