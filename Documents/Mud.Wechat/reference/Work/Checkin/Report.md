# 打卡报表（日报 / 月报）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Checkin` 域 · 打卡报表族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCheckinReportService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Report/IWechatWorkCheckinReportService.cs)（零端点父接口）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Report/IWechatWorkCheckinReportService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Report/IWechatWorkCheckinReportService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Report/IWechatWorkCheckinReportService_ThirdParty.cs) |
| DTO 源文件 | [DataModels/Checkin/Report/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Report/)（35 型） |
| 契约守卫 | [WechatCheckinContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)（CK1 路由 / CK2 层级与开放面 / CK3 令牌绑定 / CK4 JSON 上下文 / CK5 同路由不同构与拼写照抄） |
| 官方文档 | 日报 93374（自建）/ 96498（代开发）/ 94206（第三方旧结构）· 月报 93387 / 96499 / 94207 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 首验 + 2026-10-30 复验：在本仓临时 Web SDK 工程（`net8.0`）与全仓 82 段可运行示例合并逐字编译，0 错误（本篇 2 段；本篇的 `// 承接` 片段示例已补齐自足前置（自带 using 与服务实例解析），现同样参与编译；接口签名摘录块与 json 块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CK1~CK5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

读**已聚合好的考勤统计**，而不是原始打卡流水：

- **日报**（E1）：某员工某天的汇总（打卡次数、工作时长、最早/最晚打卡、加班、假勤、异常）。
- **月报**（E2）：某员工某月的汇总（出勤天数、应出勤天数、休息/异常天数、月总工时、加班与折算）。

与 [Record.md](./Record.md) 的分工：记录族给你「几点几分在哪儿打的卡」，报表族给你「这一天/这个月算出来的结果」。做考勤结算用报表族；做原始凭证留档用记录族。

和记录族一样，本族**父接口零端点**：日报/月报在第三方文档页是 `baseinfo` 旧结构，与自建/代开发的 `base_info` 现代结构**同路由不同构**，C# 返回类型无法同时兼容，故三分支子接口各承载一份。

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
    var day = await reportService.GetCheckinDayDataAsync(request, ct);
}
```

### 2.2 权限与业务前置

- 官方权限：自建须配置到「打卡 - 可调用接口的应用」；代开发/第三方须具有「打卡」权限。
- **`useridlist` ≤ 100 个**，超过须分批；单个 userid 官方标注 1~64 字节。
- `starttime` / `endtime` 是**当天 0 点**的 Unix 秒；日报跨度 **≤ 30 天**（月报跨度上限接口注释**未留档**，见 §7）。
- 只能读到应用**可见范围**内的成员；越界由官方报错，SDK 不预判。
- 报表里的姓名、部门、userid 属个人数据，示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1 `getcheckin_daydata` | ✅ 现代结构 | ✅ 现代结构 | ✅ `baseinfo` 旧结构 |
| E2 `getcheckin_monthdata` | ✅ 现代结构 | ✅ 现代结构 | ✅ `baseinfo` 旧结构 |

三类应用**全部开放**，但**返回结构分两套**，且第三方那套不能用现代 DTO 承载。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCheckinReportService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **0** | 否 |
| `IWechatWorkInternalCheckinReportService` | `Mud.Wechat.Work` | 自建子接口（现代结构） | 2 | 是 |
| `IWechatWorkProviderCheckinReportService` | `Mud.Wechat.Work` | 代开发子接口（现代结构） | 2 | 是 |
| `IWechatWorkThirdPartyCheckinReportService` | `Mud.Wechat.Work` | 第三方子接口（旧结构） | 2 | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`；模块注册 `WechatModule.Checkin` / `AddCheckinApi()`；注册组 `Checkin`
- 本族路由：**2 条**，按应用类型展开 **6 个声明方法**
- 端点索引：**E1** `GetCheckinDayDataAsync` · **E2** `GetCheckinMonthDataAsync`

---

## 4. 端点

### E1. `GetCheckinDayDataAsync` — 获取打卡日报数据

**签名（三个子接口各一支，路由相同、返回类型不同）**

```csharp
// 自建 / 代开发（现代结构）
Task<GetCheckinDayDataResponse> GetCheckinDayDataAsync(
    [Body] GetCheckinDayDataRequest request,
    CancellationToken cancellationToken = default);

// 第三方（旧字段结构）
Task<GetThirdPartyCheckinDayDataResponse> GetCheckinDayDataAsync(
    [Body] GetCheckinDayDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/getcheckin_daydata` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（现代）/ 第三方（旧结构） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Starttime` | `starttime` | `long?` | 是 | **0 点** Unix 秒 |
| `request.Endtime` | `endtime` | `long?` | 是 | **0 点** Unix 秒；跨度 ≤ 30 天 |
| `request.UserIdList` | `useridlist` | `List<string>?` | 是 | 不超过 100 个 |

```json
{
  "starttime": 1790000000,
  "endtime": 1790086400,
  "useridlist": ["zhangsan"]
}
```

**响应参数 — 现代结构（自建 / 代开发）**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `datas` | `Datas` | `List<CheckinDayData>?` | 每个「人 × 天」一条 |
| `datas[].base_info` | `BaseInfo` | `CheckinDayBaseInfo?` | 基础信息 |
| `base_info.date` | `Date` | `long?` | 日期（当天 0 点 Unix 秒） |
| `base_info.record_type` | `RecordType` | `int?` | 记录类型（注释给出 1/2/3/4/5/7，**无 6**） |
| `base_info.name` / `name_ex` | 同名 | `string?` | 姓名 / 别名 |
| `base_info.departs_name` | `DepartsName` | `string?` | 部门名，**分号分隔的字符串**（不是数组） |
| `base_info.acctid` | `Acctid` | `string?` | 账号 id |
| `base_info.rule_info` | `RuleInfo` | `CheckinDayRuleInfo?` | 规则 / 班次 / 时段 |
| `base_info.day_type` | `DayType` | `int?` | 日期类型（工作日/休息日/节假日） |
| `datas[].summary_info` | `SummaryInfo` | `CheckinDaySummaryInfo?` | 汇总 |
| `summary_info.checkin_count` | `CheckinCount` | `int?` | 打卡次数 |
| `summary_info.regular_work_sec` / `standard_work_sec` | 同名 | `int?` | 实际 / 标准工作时长（秒） |
| `summary_info.earliest_time` | `EarliestTime` | `int?` | 最早打卡时间，**距 0 点偏移秒** |
| `summary_info.lastest_time` | `LastestTime` | `int?` | 最晚打卡时间（**官方拼写 `lastest`**） |
| `datas[].holiday_infos` | `HolidayInfos` | `List<CheckinHolidayInfo>?` | 假勤信息（含多语言标题 `sp_title`） |
| `datas[].exception_infos` | `ExceptionInfos` | `List<CheckinExceptionInfo>?` | 校准/异常状态（`exception` / `count` / `duration`） |
| `datas[].ot_info` | `OtInfo` | `CheckinDayOtInfo?` | **加班信息（日报专属结构）** |
| `datas[].sp_items` | `SpItems` | `List<CheckinSpItem>?` | 假勤统计（`vacation_id` / `count` / `duration`） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "datas": [
    {
      "base_info": { "date": 1790000000, "record_type": 1, "name": "张三", "departs_name": "研发部;平台组" },
      "summary_info": { "checkin_count": 2, "regular_work_sec": 28800, "standard_work_sec": 28800, "earliest_time": 32400, "lastest_time": 64800 },
      "ot_info": { "ot_status": 1, "ot_duration": 0 }
    }
  ]
}
```

**响应参数 — 第三方旧结构**

| 字段（JSON） | 属性（C#） | 类型 | 与现代结构的差异 |
|---|---|---|---|
| `datas[].baseinfo` | `Baseinfo` | `ThirdPartyCheckinDayBaseInfo?` | **无下划线**（现代为 `base_info`） |
| `baseinfo.checkin_recordtype` | `CheckinRecordtype` | `int?` | 现代用 `record_type`；旧结构里 `record_type` **已废弃**，改用本字段 |
| `baseinfo.acctivity_name` | `AcctivityName` | `string?` | 官方拼写照抄 |
| `baseinfo.dep_name` | `DepName` | `string?` | 部门为 **JSON 序列化字符串** |
| `baseinfo.exception_duration` | `ExceptionDuration` | `int?` | 标量（现代在 `exception_infos[]` 列表里） |
| `datas[].rule_info` | `RuleInfo` | `ThirdPartyCheckinDayRuleInfo?` | — |
| `datas[].holiday_infos` | `HolidayInfos` | `List<ThirdPartyCheckinHolidayInfo>?` | 结构与本仓现代版不同 |
| `datas[].scheduleinfo` | `Scheduleinfo` | `ThirdPartyCheckinScheduleInfo?` | 现代结构**没有**顶层 `scheduleinfo` |

**调用示例**

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddCheckinApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;     // WechatAppType
using Mud.Wechat.Work.DataModels.Checkin;     // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，GetRequiredService 与 logger 扩展方法由 Web SDK 隐式 using 提供。

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册打卡模块
builder.Services.AddWechatWorkServices(b => b.AddCheckinApi());

// ③ 报表族父接口零端点且不可注入：自建注入本接口，第三方须改注入 IWechatWorkThirdPartyCheckinReportService（旧结构）
var reports = provider.GetRequiredService<IWechatWorkInternalCheckinReportService>();

var start = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.FromHours(8));
var end = new DateTimeOffset(2026, 10, 29, 0, 0, 0, TimeSpan.FromHours(8));

var dayResp = await reports.GetCheckinDayDataAsync(
    new GetCheckinDayDataRequest
    {
        Starttime = start.ToUnixTimeSeconds(),   // 0 点 Unix 秒
        Endtime = end.ToUnixTimeSeconds(),       // 跨度 ≤ 30 天
        UserIdList = new List<string> { "zhangsan" },   // ≤100
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(dayResp, "/cgi-bin/checkin/getcheckin_daydata");

foreach (var data in dayResp.Datas ?? new List<CheckinDayData>())
{
    // earliest_time / lastest_time 是「距 0 点偏移秒」，不是时间戳；departs_name 是分号分隔字符串
    logger.LogInformation("成员 {Acctid} 打卡 {Count} 次，标准工时 {StdSec} 秒",
        data.BaseInfo?.Acctid, data.SummaryInfo?.CheckinCount, data.SummaryInfo?.StandardWorkSec);
}
```

**本端点注意事项**

- **`base_info` vs `baseinfo`**：现代与第三方旧结构只差一个下划线，用错类型会静默拿到全 `null`。
- **`lastest_time` 是官方拼写**（不是 `latest_time`），CK5 锁定。
- `earliest_time` / `lastest_time` 是**距 0 点的偏移秒**，不是时间戳。
- **日报的加班结构是 `ot_info`**，月报是 `overwork_info`——两者**不可共用**（CK5 断言了两者的属性类型不同）。
- 第三方旧结构里 `record_type` 已废弃、改用 `checkin_recordtype`；`dep_name` 是 JSON 序列化**字符串**（要自己再解析一层）。
- 频率：**100 次/分钟**。

### E2. `GetCheckinMonthDataAsync` — 获取打卡月报数据

**签名（三个子接口各一支）**

```csharp
// 自建 / 代开发（现代结构）
Task<GetCheckinMonthDataResponse> GetCheckinMonthDataAsync(
    [Body] GetCheckinMonthDataRequest request,
    CancellationToken cancellationToken = default);

// 第三方（旧字段结构）
Task<GetThirdPartyCheckinMonthDataResponse> GetCheckinMonthDataAsync(
    [Body] GetCheckinMonthDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/getcheckin_monthdata` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（现代）/ 第三方（旧结构） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Starttime` | `starttime` | `long?` | 是 | **0 点** Unix 秒 |
| `request.Endtime` | `endtime` | `long?` | 是 | **0 点** Unix 秒（跨度上限注释未留档） |
| `request.UserIdList` | `useridlist` | `List<string>?` | 是 | 不超过 100 个 |

```json
{
  "starttime": 1790000000,
  "endtime": 1792500000,
  "useridlist": ["zhangsan"]
}
```

**响应参数 — 现代结构**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `datas` | `Datas` | `List<CheckinMonthData>?` | 每个「人 × 月」一条 |
| `datas[].base_info` | `BaseInfo` | `CheckinMonthBaseInfo?` | 基础信息（`record_type` / `name` / `name_ex` / `departs_name` / `acctid` / `rule_info`） |
| `datas[].summary_info` | `SummaryInfo` | `CheckinMonthSummaryInfo?` | 汇总 |
| `summary_info.work_days` / `rest_days` / `except_days` | 同名 | `int?` | 出勤 / 休息 / 异常天数 |
| `summary_info.regular_days` | `RegularDays` | `int?` | 正常出勤天数（**参数表列出、官方示例未包含**） |
| `summary_info.regular_work_sec` / `standard_work_sec` | 同名 | `int?` | 实际 / 标准月工时（秒） |
| `datas[].exception_infos` | `ExceptionInfos` | `List<CheckinExceptionInfo>?` | 异常统计 |
| `datas[].sp_items` | `SpItems` | `List<CheckinSpItem>?` | 假勤统计 |
| `datas[].overwork_info` | `OverworkInfo` | `CheckinMonthOverworkInfo?` | **加班情况（月报专属结构）** |
| `overwork_info.workday_over_sec` / `restdays_over_sec` / `holidays_over_sec` | 同名 | `int?` | 三个「时长」字段为**单数**形式 |
| `overwork_info.workdays_over_as_vacation` / `workdays_over_as_money` 等六项 | 同名 | `int?` | 六个「记为调休/加班费」字段为**复数**形式 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "datas": [
    {
      "base_info": { "record_type": 1, "name": "张三", "departs_name": "研发部" },
      "summary_info": { "work_days": 20, "rest_days": 8, "except_days": 0, "regular_work_sec": 576000, "standard_work_sec": 576000 },
      "overwork_info": { "workday_over_sec": 0, "workdays_over_as_money": 0 }
    }
  ]
}
```

**响应参数 — 第三方旧结构**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `datas[].baseinfo` | `Baseinfo` | `ThirdPartyCheckinMonthBaseInfo?` | 旧结构全部统计都在 `baseinfo` 里 |
| `baseinfo.acctivity_name` | `AcctivityName` | `string?` | 官方拼写照抄 |
| `baseinfo.avg_work_sec` / `avg_std_work_sec` | 同名 | `int?` | 日均实际 / 标准工时 |
| `baseinfo.days` / `work_days` | 同名 | `int?` | 天数 / 出勤天数 |
| `baseinfo.regular_days_cnt` / `regular_work_days_cnt` | 同名 | `int?` | 正常天数 / 正常出勤天数 |
| `baseinfo.late_comes_cnt` / `late_comes_duration` | 同名 | `int?` | 迟到次数 / 时长 |
| `baseinfo.leave_earlies_cnt` / `leave_earlies_duration` | 同名 | `int?` | 早退次数 / 时长 |
| `baseinfo.absenteeisms_cnt` / `absenteeisms_days_cnt` | 同名 | `int?` | 旷工次数 / 天数 |
| `baseinfo.missed_checks_cnt` | `MissedChecksCnt` | `int?` | 漏打卡次数 |
| `baseinfo.exception_infos` | `ExceptionInfos` | `List<ThirdPartyCheckinMonthException>?` | 异常明细 |
| `baseinfo.overwork` / `baseinfo.ov_time` | 同名 | `List<ThirdPartyCheckinOverwork>?` | **参数表描述为单对象、返回示例为数组**，SDK 按示例建模（CK5） |
| `overwork[].work_sec` / `days_cnt` / `excepion_days_cnt` | 同名 | `int?` | 时长 / 天数 / 异常天数（`excepion` 为官方拼写） |

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException
using Mud.Wechat.Work.DataModels.Checkin;      // 请求/响应 DTO
// 片段自足前置：与 E1 使用同一个自建子接口实例。
var reports = provider.GetRequiredService<IWechatWorkInternalCheckinReportService>();
var start = new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Local));
var end = new DateTimeOffset(new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Local));


var monthResp = await reports.GetCheckinMonthDataAsync(
    new GetCheckinMonthDataRequest
    {
        Starttime = start.ToUnixTimeSeconds(),
        Endtime = end.ToUnixTimeSeconds(),
        UserIdList = new List<string> { "zhangsan" },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(monthResp, "/cgi-bin/checkin/getcheckin_monthdata");

foreach (var data in monthResp.Datas ?? new List<CheckinMonthData>())
{
    // 单数三字段 + 复数六字段混排，照抄官方原始拼写。
    logger.LogInformation("成员 {Acctid} 出勤 {WorkDays} 天，工作日加班 {OverSec} 秒",
        data.BaseInfo?.Acctid, data.SummaryInfo?.WorkDays, data.OverworkInfo?.WorkdayOverSec);
}
```

**本端点注意事项**

- **`overwork_info` 的单数/复数混排是官方原文**：`workday_over_sec`（单数）与 `workdays_over_as_money`（复数）同在一个对象里，CK5 逐字段锁定，不要「统一」。
- **`summary_info.regular_days` 只在参数表出现，官方示例没有**——可能缺失，按可空处理。
- 第三方旧结构把**全部统计塞进 `baseinfo`**，没有 `summary_info` / `overwork_info` 分层；`overwork` 与 `ov_time` 都是**数组**（参数表写单对象，以示例为准）。
- 频率：**60 次/分钟**；跨度上限**未留档**（不要从日报的 30 天外推）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `base_info`（现代）vs `baseinfo`（第三方） | 只差一个下划线，两套是**不同 DTO** | 用错类型 → 所有字段静默为 `null`，报表全空 |
| 日报 `ot_info` vs 月报 `overwork_info` | 两个加班结构字段名与形态都不同（CK5 断言属性类型不同） | 共用一个「加班」模型 → 一半字段取不到 |
| `workday_over_sec`（单数）vs `workdays_over_as_money`（复数） | 官方原文如此 | 「顺手统一」为复数 → JSON 名不匹配，值为 0 |
| `lastest_time` | 官方拼写就是 `lastest` | 写成 `latest_time` → 无法映射 |
| `earliest_time` / `lastest_time` 单位 | 距 0 点**偏移秒**，不是时间戳 | 当时间戳格式化 → 1970 年 |
| `departs_name` | 分号分隔**字符串** | 当数组用 → 只拿到一个拼接串 |
| 现代 `record_type` vs 第三方 `checkin_recordtype` | 旧结构里 `record_type` 已废弃 | 第三方取 `record_type` → 恒为 0/缺失 |
| 第三方 `dep_name` | 是 JSON 序列化字符串 | 当普通字符串展示 → 露出 `{"...":...}` 原文 |
| `acctivity_name` / `excepion_days_cnt` | 官方拼写照抄 | 修正拼写 → 映射失败 |
| 日报 30 天跨度 vs 月报 | 日报 ≤30 天；月报上限未留档 | 把 30 天套到月报 → 可能被拒或误判 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 301021 | 官方业务码 | 输入参数错误 | 参数/时间跨度不合法 |
| 301070 | 官方业务码 | 系统错误 | 偶发，可退避重试 |
| 301075 | 官方业务码 | 输入参数错误（排班族专属，本族一般不出） | 见 [Schedule.md](./Schedule.md) |
| 42001 / 40014 / 42007 | `ExpiredAccessToken` / `InvalidAccessToken` / `RelatedAccessTokenInvalid` | 令牌过期/失效 | 自愈集合 |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 套件/服务商令牌失效 | 本族不消费；出现即凭据族错配 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

超出时间跨度时官方**直接拒绝查询**（不是截断返回），按 `301021` 一类参数错误处理。
其余码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 月报时间跨度上限 | 接口注释只留 `useridlist` ≤100 与 60 次/分钟，**跨度无数值** ⇒ **未逐项核验**（不得从日报 30 天外推） | 官方 93387 |
| `exception_infos.exception` / `base_info.record_type` / `day_type` 枚举全集 | 注释给出 `record_type` 1/2/3/4/5/7，其余集合未核验 | 官方参数表 |
| `sp_items[].vacation_id` 的字典来源 | 注释指向审批域，**未在本域核验** | 审批域文档 |
| 第三方 `rule_info.checkin_time` / `scheduleinfo.timesec` 的真实形态 | 注释记「参数表描述为标量、实为对象/数组」⇒ 已按示例建模，**未逐项核验** | 官方 94206 |
| 官方文档正文 | 本文只摘录本仓 XML 注释；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md)（§1 父接口零端点、§9 拼写照抄与加班结构不对称）
- 兄弟族：[Rule.md](./Rule.md)（打卡规则）· [Record.md](./Record.md)（打卡记录）· [Schedule.md](./Schedule.md)（排班）· [Device.md](./Device.md)（设备打卡数据）
- 官方页：[获取打卡日报数据（自建 93374）](https://developer.work.weixin.qq.com/document/path/93374) · [获取打卡月报数据（自建 93387）](https://developer.work.weixin.qq.com/document/path/93387) · [第三方日报（94206）](https://developer.work.weixin.qq.com/document/path/94206) · [第三方月报（94207）](https://developer.work.weixin.qq.com/document/path/94207)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
