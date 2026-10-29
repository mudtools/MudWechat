# 打卡规则（员工规则 / 企业规则 / 规则增删改清）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Checkin` 域 · 打卡规则族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCheckinRuleService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Rule/IWechatWorkCheckinRuleService.cs) · [IWechatWorkCheckinRuleService_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Rule/IWechatWorkCheckinRuleService_Internal.cs) · [IWechatWorkCheckinRuleService_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Rule/IWechatWorkCheckinRuleService_Provider.cs) · [IWechatWorkCheckinRuleService_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Rule/IWechatWorkCheckinRuleService_ThirdParty.cs) |
| DTO 源文件 | [DataModels/Checkin/Rule/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Rule/)（48 型，本族端点直接引用的关键型见 §3 与各端点） |
| 契约守卫 | [WechatCheckinContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)（CK1 路由 / CK2 层级与开放面 / CK3 令牌绑定 / CK4 JSON 上下文 / CK5 契约陷阱） |
| 官方文档 | 获取员工打卡规则 90263（自建）/ 94204（第三方）/ 99443（代开发）· 获取企业所有打卡规则 93384 / 99444 · 管理打卡规则 4 写端点 98041 / 98767 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段，接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CK1~CK5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

读取与维护企业的**打卡规则**（考勤规则的配置面）：打卡时间、打卡地点、Wi-Fi、打卡范围、特殊工作日/非工作日、排班班次、加班与补卡策略等。

本族有**两种读法、四条写路径**，边界必须分清：

| 端点 | 读/写 | 视角 | 时间入参 |
|---|---|---|---|
| E1 获取员工打卡规则 | 读 | **按人按天**：某员工在某一天适用的规则（不同日期可能不同） | `datetime` = 当天 **0 点**的 Unix 秒 |
| E2 获取企业所有打卡规则 | 读 | **企业全量**：不分员工、不分日期，一次拿全部规则 | 无（请求体 `{}`） |
| E3 创建 / E4 修改 / E5 清空数组元素 / E6 删除 | 写 | 只作用于**本应用创建**的规则 | `effective_now` 是否立即生效 |

**写端点的硬约束**（最容易踩）：规则**只能由创建它的应用**修改/删除；修改是**覆盖语义**（见 §4 E4），且**一级数组字段无法通过修改接口清空为空集**——必须走 E5。E3 创建时**不要**传 `groupid`（官方忽略该字段）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（自建）/ `WechatAppType.Provider`（服务商代开发）/ `WechatAppType.ThirdParty`（第三方，仅 E1） | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（`Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入，**不在方法签名里** | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

第三方/代开发消费的是**授权企业级**令牌（一个 `(AppKey, authCorpId)` 一份），调用前先建立企业作用域：

```csharp
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var rules = await ruleService.GetCheckinOptionAsync(request, ct);
}
```

> `permanent_code` 是凭据：只写参数名，**不得进日志、遥测或异常消息**。
> **自 2023-12-01 0 点起不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。

### 2.2 权限与业务前置

- 官方权限：**自建应用**须配置到「打卡 - 可调用接口的应用」中；**代开发 / 第三方**须具有「打卡」权限。
- 可见范围：只能读到应用可见范围内成员的规则；`useridlist` 超出可见范围由官方报错，SDK 不做本地预判。
- `useridlist` **不超过 100 个**，超过须自行分批（E1）。
- E1 必须**按天**取：`datetime` 传「需要获取规则的那一天 0 点」的 Unix 秒，不是区间。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1 `getcheckinoption` | ✅ | ✅ | ✅ |
| E2 `getcorpcheckinoption` | ✅ | ✅ | ❌ 官方标注「暂不支持」 |
| E3–E6 管理打卡规则 | ✅ | ✅ | ❌ 官方标注「暂不支持」 |

第三方文档树对 E2–E6 明确标注暂不支持，故 SDK 的第三方子接口是**零端点空标记**（CK2 以 `HaveCount(0)` 锁死，官方日后开放须先核文档再同批改守卫，**不得**先补端点）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCheckinRuleService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载 E1 | 1 | 否 |
| `IWechatWorkInternalCheckinRuleService` | `Mud.Wechat.Work` | 自建子接口，承载 E2–E6 | 5 | 是 |
| `IWechatWorkProviderCheckinRuleService` | `Mud.Wechat.Work` | 代开发子接口，承载 E2–E6（同路由同 DTO） | 5 | 是 |
| `IWechatWorkThirdPartyCheckinRuleService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`），可被 per-app `WechatAppConfig.BaseUrl` 覆盖
- 模块注册：`WechatModule.Checkin` / `AddCheckinApi()`；注册组 `Checkin`（五族共用 `AddCheckinWebApiHttpClient()`）
- 本族路由：**6 条**（去重后），按应用类型展开为 **11 个声明方法**（父 1 + 自建 5 + 代开发 5 + 第三方 0）

端点索引：**E1** `GetCheckinOptionAsync` · **E2** `GetCorpCheckinOptionAsync` · **E3** `AddCheckinOptionAsync` · **E4** `UpdateCheckinOptionAsync` · **E5** `ClearCheckinOptionArrayFieldAsync` · **E6** `DelCheckinOptionAsync`

关键 DTO：请求 `GetCheckinOptionRequest` / `GetCorpCheckinOptionRequest`（空体）/ `AddCheckinOptionRequest` / `UpdateCheckinOptionRequest` / `ClearCheckinOptionArrayFieldRequest` / `DelCheckinOptionRequest`；规则体 `CheckinRuleGroup`（请求侧，含 `ot_info_v2`）与 `CheckinGroup`（E1 响应）/ `CheckinCorpGroup`（E2 响应，含旧 `ot_info`）；打卡范围 `CheckinRange`。

---

## 4. 端点

### E1. `GetCheckinOptionAsync` — 获取员工打卡规则

**签名**

```csharp
Task<GetCheckinOptionResponse> GetCheckinOptionAsync(
    [Body] GetCheckinOptionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/getcheckinoption` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/checkin/getcheckinoption?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| 请求体编码 | `application/json`，源生成序列化（`CheckinJsonContext`，CK4） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Datetime` | `datetime` | `long?` | 是 | 需要获取规则的**那一天 0 点**的 Unix 时间戳（秒） |
| `request.UserIdList` | `useridlist` | `List<string>?` | 是 | 用户列表，**不超过 100 个**，超过须分批 |

```json
{
  "datetime": 1790000000,
  "useridlist": ["zhangsan", "lisi"]
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头 |
| `info` | `Info` | `List<CheckinOptionInfo>?` | 每个用户一条 |
| `info[].userid` | `Userid` | `string?` | 打卡人员 userid |
| `info[].group` | `Group` | `CheckinGroup?` | 该用户当天的打卡规则 |

`CheckinGroup` 关键字段（完整字段见 [CheckinGroup.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Rule/CheckinGroup.cs)）：

| JSON | 属性 | 类型 | 说明 |
|---|---|---|---|
| `grouptype` | `Grouptype` | `int?` | 规则类型：1 固定上下班 / 2 按班次上下班 / 3 自由上下班 |
| `groupid` | `Groupid` | `long?` | 规则 id |
| `groupname` | `Groupname` | `string?` | 规则名 |
| `checkindate` | `Checkindate` | `List<CheckinGroupCheckindate>?` | 按星期的打卡时间配置 |
| `checkindate[].workdays` | `Workdays` | `List<int>?` | 生效星期 |
| `checkindate[].checkintime` | `Checkintime` | `List<CheckinCheckintime>?` | 时段：`work_sec`/`off_work_sec`/提醒/弹性（**距 0 点的偏移秒**） |
| `spe_workdays` / `spe_offdays` | `SpeWorkdays` / `SpeOffdays` | `List<CheckinSpeDay>?` | 特殊工作日 / 非工作日 |
| `wifimac_infos` | `WifimacInfos` | `List<CheckinWifiInfo>?` | Wi-Fi 打卡点（`wifiname` / `wifimac`） |
| `loc_infos` | `LocInfos` | `List<CheckinLocInfo>?` | 地点打卡点（`lat`/`lng`/`loc_title`/`distance`） |
| `schedulelist` | `Schedulelist` | `List<CheckinScheduleInfo>?` | 班次（按班次规则时） |
| `offwork_interval_time` | `OffworkIntervalTime` | `int?` | 下班打卡允许间隔 |
| `span_day_time` | `SpanDayTime` | `int?` | 自由上下班跨天时长 |
| `standard_work_duration` | `StandardWorkDuration` | `int?` | 标准工作时长 |
| `checkin_method_type` | `CheckinMethodType` | `int?` | 打卡方式 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "info": [
    {
      "userid": "zhangsan",
      "group": {
        "grouptype": 1,
        "groupid": 1,
        "groupname": "默认规则",
        "checkindate": [
          { "workdays": [1, 2, 3, 4, 5], "checkintime": [{ "work_sec": 32400, "off_work_sec": 64800 }] }
        ]
      }
    }
  ]
}
```

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

// ② 注册打卡模块（令牌基础设施随 AddWechatApp 装配）
builder.Services.AddWechatWorkServices(b => b.AddCheckinApi());

// ③ 注入自建子接口：父接口 IsAbstract 不参与 DI，且本族差异端点只在子接口上
var rules = provider.GetRequiredService<IWechatWorkInternalCheckinRuleService>();

// ④ 按天取规则：datetime 必须是「那一天 0 点」的 Unix 秒，不是区间。
//    userid 是个人数据：可落库，但不要进日志/遥测。
var day = new DateTime(2026, 10, 29, 0, 0, 0, DateTimeKind.Local);
var userIds = new List<string> { "zhangsan", "lisi" };   // 官方上限 100，超过须分批

var option = await rules.GetCheckinOptionAsync(
    new GetCheckinOptionRequest
    {
        Datetime = new DateTimeOffset(day).ToUnixTimeSeconds(),
        UserIdList = userIds,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(option, "/cgi-bin/checkin/getcheckinoption");

foreach (var item in option.Info ?? new List<CheckinOptionInfo>())
{
    // grouptype：1 固定上下班 / 2 按班次上下班 / 3 自由上下班
    logger.LogInformation("成员 {UserId} 当日规则类型 {GroupType}，规则 id {GroupId}",
        item.Userid, item.Group?.Grouptype, item.Group?.Groupid);
}
```

**本端点注意事项**

- **`datetime` 是单日 0 点**，不是区间：传区间内任意时刻或区间起止都不符合官方语义。
- `work_sec` / `off_work_sec` 等是**距当天 0 点的偏移秒**（如 32400 = 9:00），不是 Unix 时间戳。
- 官方返回示例里出现过**字面量点号键** `"group.checkin_method_type"`——那是官方示例原文的写法，实际语义是 `group` 下的嵌套字段 `checkin_method_type`，SDK 按嵌套建模（CK5 锁定），不要去找名为 `group.checkin_method_type` 的顶层属性。
- 官方参数表存在 `work_se`、`uint3`、`unit32`、`allow_flex` 一处标 `uint32` 一处标 `bool` 等笔误，SDK 按**示例**建模，照抄勿修正。
- 频率上限：**接口注释未留档**（见 §7 缺口）。

### E2. `GetCorpCheckinOptionAsync` — 获取企业所有打卡规则

**签名**

```csharp
Task<GetCorpCheckinOptionResponse> GetCorpCheckinOptionAsync(
    [Body] GetCorpCheckinOptionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/getcorpcheckinoption` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（第三方暂不支持） |
| 请求体 | **空 JSON 对象 `{}`**（`GetCorpCheckinOptionRequest` 零公共属性，CK5 以 `Should().BeEmpty()` 锁定） |

**请求参数**

无业务参数。**必须传 `new GetCorpCheckinOptionRequest()` 空实例**，不要传 `null`。

```json
{}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头 |
| `group` | `Group` | `List<CheckinCorpGroup>?` | 企业全部打卡规则 |
| `group[].range` | `Range` | `CheckinRange?` | 打卡范围（`party_id` / `userid` / `tagid`） |
| `group[].white_users` | `WhiteUsers` | `List<string>?` | 白名单成员 |
| `group[].type` | `Type` | `int?` | 规则类型（与 `grouptype` 并存，官方原文如此） |
| `group[].reporterinfo` | `Reporterinfo` | `CheckinReporterInfo?` | 汇报对象 |
| `group[].ot_info` | `OtInfo` | `CheckinOtInfo?` | **旧版**加班配置（含 `otapplyinfo`） |
| `group[].create_time` / `create_userid` / `update_userid` | 同名属性 | `long?` / `string?` / `string?` | 审计字段 |
| `group[].checkindate[].checkintime` | `Checkintime` | `List<CheckinCheckintime>?` | 参数表标注 `uint32`，实为对象数组（CK5） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "group": [
    {
      "grouptype": 1,
      "groupid": 1,
      "groupname": "默认规则",
      "range": { "userid": ["zhangsan"], "party_id": [], "tagid": [] },
      "ot_info": { "type": 1, "allow_ot_workingday": true }
    }
  ]
}
```

**调用示例**

```csharp
// 承接 E1 的装配与 rules 实例；服务商代开发注入 IWechatWorkProviderCheckinRuleService（先 UseCorpScope）。

// 请求体必须显式传空实例（官方即 {}），不要传 null。
var corpOption = await rules.GetCorpCheckinOptionAsync(
    new GetCorpCheckinOptionRequest(), cancellationToken);
WechatWorkException.ThrowIfFailed(corpOption, "/cgi-bin/checkin/getcorpcheckinoption");

logger.LogInformation("企业打卡规则共 {Count} 条", corpOption.Group?.Count ?? 0);
```

**本端点注意事项**

- **请求体是空对象**：`GetCorpCheckinOptionRequest` 没有属性是刻意的，凭据只在 Query 上。
- **响应侧 `ot_info` 是旧结构**：官方错误表明确 `ot_info` 是旧字段不建议使用，但本端点响应仍返回它；与写端点请求侧的 `ot_info_v2` **不同构**（见 §5）。
- 频率：**60 次/分钟**。

### E3. `AddCheckinOptionAsync` — 创建打卡规则

**签名**

```csharp
Task<WechatWorkResponse> AddCheckinOptionAsync(
    [Body] AddCheckinOptionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/add_checkin_option` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（第三方暂不支持） |
| 响应 | 仅 `errcode` / `errmsg`，无业务负载 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Group` | `group` | `CheckinRuleGroup?` | 是 | 规则完整定义（**不要传 `groupid`**，官方忽略） |
| `request.EffectiveNow` | `effective_now` | `bool?` | 否 | 是否立即生效 |

`CheckinRuleGroup` 关键字段：`grouptype`、`groupname`、`checkindate[]`（`workdays` + `checkintime[]`）、`spe_workdays[]` / `spe_offdays[]`、`wifimac_infos[]`、`loc_infos[]`、`range`、`schedulelist[]`、`ot_info_v2`、`buka_remind`、`allow_apply_bk_cnt`、`option_out_range`、`use_face_detect` 等（完整清单见 [CheckinRuleGroup.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Rule/CheckinRuleGroup.cs)）。

**官方容量闸与格式约束**（接口注释留档）：

| 约束 | 值 |
|---|---|
| `wifimac_infos` 与 `loc_infos` | **不能同时为空** |
| Wi-Fi / 地点个数 | ≤ 500 |
| `checkindate` 个数 | 1 ~ 7 |
| 单日打卡时段 | 1 ~ 4 |
| 排班个数 | ≤ 50 |
| 特殊工作日 / 非工作日个数 | ≤ 366 |
| 时段内所有时间 | 必须为**整分钟**（60 的倍数） |
| 上下班区间与可打卡时间 | 不可有交集 |
| 休息时段 | ≤ 5 组且彼此不可有交集 |
| 弹性两组 | `flex_on_duty_time`/`flex_off_duty_time` 与 `max_allow_arrive_early`/`max_allow_arrive_late` **互斥**（设一组时另一组置 0） |
| `flex_time` | 官方注明**旧字段**，openapi 创建/修改**不可使用** |

```json
{
  "group": {
    "grouptype": 1,
    "groupname": "默认规则",
    "checkindate": [
      { "workdays": [1, 2, 3, 4, 5], "checkintime": [{ "time_id": 1, "work_sec": 32400, "off_work_sec": 64800 }] }
    ],
    "loc_infos": [{ "lat": 0, "lng": 0, "loc_title": "公司", "loc_detail": "前台", "distance": 100 }]
  },
  "effective_now": true
}
```

**调用示例**

```csharp
// 承接 E1 的装配与 rules 实例。

var created = await rules.AddCheckinOptionAsync(
    new AddCheckinOptionRequest
    {
        Group = new CheckinRuleGroup
        {
            Grouptype = 1,
            Groupname = "示例规则",
            Checkindate = new List<CheckinRuleCheckindate>
            {
                new CheckinRuleCheckindate
                {
                    Workdays = new List<int> { 1, 2, 3, 4, 5 },
                    Checkintime = new List<CheckinRuleCheckintime>
                    {
                        new CheckinRuleCheckintime { TimeId = 1, WorkSec = 32400, OffWorkSec = 64800 },
                    },
                },
            },
            // wifimac_infos 与 loc_infos 不能同时为空：至少给一种打卡点
            LocInfos = new List<CheckinLocInfo>
            {
                new CheckinLocInfo { Lat = 0, Lng = 0, LocTitle = "示例地点", LocDetail = "示例详情", Distance = 100 },
            },
        },
        EffectiveNow = true,
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(created, "/cgi-bin/checkin/add_checkin_option");
logger.LogInformation("创建打卡规则完成");
```

**本端点注意事项**

- **创建时不要传 `groupid`**：官方会忽略它。
- 主要错误码：`301093`（参数校验失败）、`301094`（使用旧字段/旧特性）——尤其注意 `flex_time`、`ot_info` 这类旧字段会触发 301094。
- 频率数值接口注释**未留档**（缺口）。

### E4. `UpdateCheckinOptionAsync` — 修改打卡规则

**签名**

```csharp
Task<WechatWorkResponse> UpdateCheckinOptionAsync(
    [Body] UpdateCheckinOptionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/update_checkin_option` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（第三方暂不支持） |
| 前提 | **规则仅可由创建它的应用修改**；官方未按 `appKey` 校验，而是按应用身份 |

**请求参数**：`group`（`CheckinRuleGroup`，**须含 `groupid`**，否则报错）+ `effective_now`。

**官方更新语义（危险）**：

| 字段位置 | 不传 / 传空 | 传值 |
|---|---|---|
| `group.*` **数组**字段 | **不更新** | **覆盖**（清空原有数组，只保留传入的数组） |
| `group.*` **非数组**字段 | **不更新** | 覆盖（并递归其所有字段） |

因此：

- 「只想改规则名」必须**只传 `groupid` + `groupname`**，漏传的数组不会被清空（这是好消息），但漏传的非数组字段也不会被保留——它们保持原值，不会被置空。
- 想**清空**一级数组字段（`spe_workdays` / `spe_offdays` / `wifimac_infos` / `loc_infos`）**做不到**，必须走 E5。
- 想清空非数组字段，直接传空元素即可。

```json
{
  "group": { "groupid": 1, "groupname": "改名后的规则" },
  "effective_now": true
}
```

**调用示例**

```csharp
// 承接 E1 的装配与 rules 实例。

var updated = await rules.UpdateCheckinOptionAsync(
    new UpdateCheckinOptionRequest
    {
        // 只改名字：只传 groupid + groupname，其余字段不传 = 不更新
        Group = new CheckinRuleGroup { Groupid = 1, Groupname = "改名后的规则" },
        EffectiveNow = true,
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/checkin/update_checkin_option");
```

**本端点注意事项**

- **必须带 `groupid`**，否则官方报错。
- 数组字段「传值 = 覆盖」意味着**传一个元素就会把原数组清成这一个元素**——想追加而不是替换，必须先读 E2 拿到原数组、合并后再整体传回，且并发场景要自行串行化。
- 想清空数组 → E5。这是本族最容易写错的一条。

### E5. `ClearCheckinOptionArrayFieldAsync` — 清空打卡规则数组元素

**签名**

```csharp
Task<WechatWorkResponse> ClearCheckinOptionArrayFieldAsync(
    [Body] ClearCheckinOptionArrayFieldRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/clear_checkin_option_array_field` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（第三方暂不支持） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Groupid` | `groupid` | `long?` | 是 | 目标规则 id |
| `request.ClearField` | `clear_field` | `List<int>?` | 是 | 清空标识：`1` `spe_workdays` · `2` `spe_offdays` · `3` `wifimac_infos` · `4` `loc_infos` |
| `request.EffectiveNow` | `effective_now` | `bool?` | 否 | 是否立即生效 |

```json
{
  "groupid": 1,
  "clear_field": [4],
  "effective_now": true
}
```

**调用示例**

```csharp
// 承接 E1 的装配与 rules 实例。

var cleared = await rules.ClearCheckinOptionArrayFieldAsync(
    new ClearCheckinOptionArrayFieldRequest
    {
        Groupid = 1,
        ClearField = new List<int> { 4 },   // 4 = loc_infos
        EffectiveNow = true,
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(cleared, "/cgi-bin/checkin/clear_checkin_option_array_field");
```

**本端点注意事项**

- **`wifimac_infos` 与 `loc_infos` 不可同时为空**：如果规则当前只配了地点打卡（`loc_infos`），单发 `clear_field: [4]` 会被官方拒绝——必须先补 Wi-Fi 打卡点再清地点，或同时调整。
- `clear_field` 是**标识列表**，可一次清多个数组。
- 仅创建该规则的应用可清空。

### E6. `DelCheckinOptionAsync` — 删除打卡规则

**签名**

```csharp
Task<WechatWorkResponse> DelCheckinOptionAsync(
    [Body] DelCheckinOptionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/del_checkin_option` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（第三方暂不支持） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明 |
|---|---|---|---|---|
| `request.Groupid` | `groupid` | `long?` | 是 | 要删除的规则 id |
| `request.EffectiveNow` | `effective_now` | `bool?` | 否 | 是否立即生效 |

```json
{ "groupid": 1, "effective_now": true }
```

**调用示例**

```csharp
// 承接 E1 的装配与 rules 实例。

var deleted = await rules.DelCheckinOptionAsync(
    new DelCheckinOptionRequest { Groupid = 1, EffectiveNow = true },
    cancellationToken);

WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/checkin/del_checkin_option");
```

**本端点注意事项**

- **仅创建该规则的应用可删除**；企业后台手工创建的规则调不到（会报无权限类错误）。
- 官方该端点的导语与「修改」相同（「修改该应用为企业创建的打卡规则」）——官方原文如此，不是 SDK 抄错。
- 删除不可逆：删除前建议先用 E2 拉一份快照留档（规则含打卡范围、地点、班次等大量配置）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 `getcheckinoption` vs E2 `getcorpcheckinoption` | E1 **按人按天**（`datetime` 单日 0 点 + `useridlist` ≤100）；E2 **企业全量**（空体） | 用 E1 拉「企业全部规则」会漏掉不在名单里的规则；用 E2 想查「某人某天的规则」拿不到按日视图 |
| E4 修改 vs E5 清空数组 | 修改的数组字段「不传=不更新、传值=覆盖」，**不能清空**；清空必须走 E5 | 以为「传空数组 = 清空」→ 实际是「不更新」，配置静默保持原样 |
| 请求侧 `ot_info_v2` vs 响应侧 `ot_info` | 写端点（E3/E4）用 `ot_info_v2`（`workdayconf` 等）；E2 响应用旧 `ot_info`（`otapplyinfo`） | 把响应的 `ot_info` 原样回传给 E4 → 触发 301094（使用旧字段）；也可能静默丢弃 |
| `grouptype` vs `type` | E1/E3/E4 用 `grouptype`；E2 响应同时有 `type` | 在 E2 响应里取 `grouptype` 时不要假设 `type` 可替代 |
| `time_id` vs `id` | 规则侧时段标识叫 `time_id`（1~99999，规则内唯一）；排班响应侧叫 `id` | 跨族复制模型 → 键名不认 |
| `lat`/`lng` 单位 | 规则与记录里的经纬度是**实际度数的 1000000 倍**（GCJ-02） | 直接当浮点度数用 → 位置全错 |
| `range.party_id` / `userid` / `tagid` | `party_id` 单数连写且为字符串数组；`userid` 参数表标 string 实为数组；`tagid` 是**整数**数组 | 与日程域 `partyids`（`List<long>`）混用 → 拼写与类型双错 |
| 官方示例 `"group.checkin_method_type"` | 字面量点号键是官方示例原文，实际为嵌套字段 | 去找顶层点号属性会落空 |
| `flex_time` / `flex_on_duty_time` | `flex_time` 是旧字段，openapi 创建/修改**不可用**；弹性用 `flex_on_duty_time`/`flex_off_duty_time` 或 `max_allow_arrive_*`（两组互斥） | 传 `flex_time` → 301094 |
| 第三方子接口 | 零端点空标记（E2–E6 官方暂不支持） | 第三方应用调用 E2–E6 必然失败；不要「补上」这些端点 |

SDK 侧两条通用约束：

- **不自动判错**：`errcode != 0` 时 HTTP 仍 200、响应照样反序列化；必须 `IsSuccess` 或 `ThrowIfFailed`。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险）。`access_token` 在 `SensitiveUrlRedactor` 词表内，但自己打日志时不要把完整 URL 打出去。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 301093 | 官方业务码 | 参数校验失败 | E3–E6 主错误码：容量闸/格式约束不满足 |
| 301094 | 官方业务码 | 使用旧字段/旧特性 | 例如传 `flex_time`、把 `ot_info` 回传 |
| 42001 / 40014 / 42007 | `ExpiredAccessToken` / `InvalidAccessToken` / `RelatedAccessTokenInvalid` | 令牌过期/失效 | 自愈集合，命中后自动刷新重试 |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 套件/服务商令牌失效 | 本族不消费；出现即凭据族别错配 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配缺陷 |

`301093` 排查顺序：`wifimac_infos` 与 `loc_infos` 是否同时为空 → Wi-Fi/地点是否超 500 → `checkindate` 是否 1~7 → 单日时段是否 1~4 → 时间是否整分钟 → 上下班区间与可打卡时间是否相交 → 休息时段是否 ≤5 且互不相交 → 弹性两组是否同时设置。
`301094` 排查：是否用了 `flex_time`、是否把 E1/E2 响应里的旧结构（`ot_info`）直接回传给写端点。

其余码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E1 频率上限 | 接口注释只留 `useridlist` ≤100 + 按天取，**无频率数值** ⇒ **未逐项核验** | 官方 90263 |
| E3–E6 频率上限 | 注释无数值（只有 E2 留了 60 次/分钟）⇒ **未逐项核验** | 官方 98041 / 98767 |
| 301093/301094 之外的业务码全集 | 注释只列两个主码 ⇒ **未逐项核验** | 官方 98041 错误码表 |
| `CheckinRuleGroup` 逐字段必填集 | 注释给出容量闸与格式约束，未逐字段标注必填 ⇒ **未逐项核验** | 官方 98041 参数表 |
| `limit_aheadtime` / `limit_offtime` 单位 | 官方未标注；域 README 推为毫秒 ⇒ **未逐项核验**，不做单位换算 | 官方规则页参数表 |
| `grouptype` / `option_out_range` / `type` 枚举全集 | 注释给出 1/2/3（规则类型）等，其余取值集合未逐项核验 | 官方参数表 |
| 官方文档正文 | 本文只摘录本仓 XML 注释；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md)（§7 更新语义、§8 频率/容量闸、§9 拼写照抄）
- 兄弟族：[Record.md](./Record.md)（打卡记录）· [Report.md](./Report.md)（日报/月报）· [Schedule.md](./Schedule.md)（排班）· [Device.md](./Device.md)（设备打卡数据）
- 官方页：[获取员工打卡规则（自建 90263）](https://developer.work.weixin.qq.com/document/path/90263) · [获取企业所有打卡规则（93384）](https://developer.work.weixin.qq.com/document/path/93384) · [管理打卡规则（98041）](https://developer.work.weixin.qq.com/document/path/98041)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
