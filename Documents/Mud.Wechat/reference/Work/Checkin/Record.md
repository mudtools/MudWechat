# 打卡记录（原始记录 / 补卡 / 添加记录 / 录入人脸）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Checkin` 域 · 打卡记录族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCheckinRecordService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Record/IWechatWorkCheckinRecordService.cs)（零端点父接口）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Record/IWechatWorkCheckinRecordService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Record/IWechatWorkCheckinRecordService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Record/IWechatWorkCheckinRecordService_ThirdParty.cs) |
| DTO 源文件 | [DataModels/Checkin/Record/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Record/)（9 型） |
| 契约守卫 | [WechatCheckinContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)（CK1 路由 / CK2 层级与开放面 / CK3 令牌绑定 / CK4 JSON 上下文 / CK5 同路由不同构与字段形态） |
| 官方文档 | 获取打卡记录数据 90262（自建）/ 96497（代开发）/ 94205（第三方旧结构）· 补卡 95803 · 添加打卡记录 99647 · 录入人脸 93378 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段，接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CK1~CK5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

读写**打卡流水**：读原始打卡记录（E1），以及三条**仅自建应用**开放的写路径——为成员补卡（E2）、通过接口写入打卡记录（E3）、录入人脸（E4）。

本族结构上有一个和其他四族都不同的特点：**父接口零端点**。

`/cgi-bin/checkin/getcheckindata` 这条路由对三种应用类型都开放，但**第三方文档页是另一套字段结构**（`agency_name` / `location_title_lat` / `location_title_lng`，且 `checkin_type` 是整数），与自建/代开发的现代结构（`lat` / `lng` / `deviceid` / `sch_checkin_time`，`checkin_type` 是中文字符串）**同路由不同构**。C# 的返回类型无法既是一个类型又兼容两套字段，因此：

| 接口 | 对 `getcheckindata` 的声明 | 返回类型 |
|---|---|---|
| `IWechatWorkInternalCheckinRecordService` | 1 支 | `GetCheckinDataResponse`（`List<CheckinRecord>`） |
| `IWechatWorkProviderCheckinRecordService` | 1 支 | 同上（现代结构，与自建同构） |
| `IWechatWorkThirdPartyCheckinRecordService` | 1 支 | `GetThirdPartyCheckinDataResponse`（`List<ThirdPartyCheckinRecord>`） |
| `IWechatWorkCheckinRecordService`（父） | **0 支** | — |

**注入的必须是应用类型子接口**：父接口 `IsAbstract = true` 不参与 DI 注册，且它一个方法都没有。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider` / `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（`Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入 | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

第三方/代开发调用前先切企业作用域（`permanent_code` 是凭据，不得进日志）：

```csharp
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var records = await recordService.GetCheckinDataAsync(request, ct);
}
```

> **自 2023-12-01 0 点起不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。

### 2.2 权限与业务前置

| 端点 | 权限 |
|---|---|
| E1 获取打卡记录数据 | 自建：配置到「打卡 - 可调用接口的应用」；代开发/第三方：具有「打卡」权限 |
| E2 补卡 / E3 添加记录 / E4 录入人脸 | **仅自建**；代开发与第三方官方均标注「暂不支持」 |

- **时间跨度**：自建/代开发 E1 跨度 ≤ **30 天**；第三方 E1 按打卡类型分档（上下班 ≤ 1 个月、外出 ≤ 3 天、全部 ≤ 3 天，系统自动校验，超过**拒绝查询**）。
- **用户列表** ≤ 100 个，超过须分批。
- 「有打卡记录即可取」，与「打卡应用」当前是否开启无关；但读到的 `userid` 集合受**应用可见范围**约束。
- **个人数据**：`userid`、经纬度、Wi-Fi MAC、打卡备注、人脸图片均属个人数据——示例一律用假值，**不得写入日志、遥测或异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1 `getcheckindata` | ✅ 现代结构 | ✅ 现代结构 | ✅ **旧结构**（不同返回类型） |
| E2 `punch_correction` | ✅ | ❌ | ❌ |
| E3 `add_checkin_record` | ✅ | ❌ | ❌ |
| E4 `addcheckinuserface` | ✅ | ❌ | ❌ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCheckinRecordService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **0** | 否 |
| `IWechatWorkInternalCheckinRecordService` | `Mud.Wechat.Work` | 自建子接口（E1 + E2/E3/E4） | 4 | 是 |
| `IWechatWorkProviderCheckinRecordService` | `Mud.Wechat.Work` | 代开发子接口（仅 E1） | 1 | 是 |
| `IWechatWorkThirdPartyCheckinRecordService` | `Mud.Wechat.Work` | 第三方子接口（仅 E1，旧结构） | 1 | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`；模块注册 `WechatModule.Checkin` / `AddCheckinApi()`；注册组 `Checkin`
- 本族路由：**4 条**（`getcheckindata` 同路由三分支），按应用类型展开 **6 个声明方法**
- 端点索引：**E1** `GetCheckinDataAsync` · **E2** `PunchCorrectionAsync` · **E3** `AddCheckinRecordAsync` · **E4** `AddCheckinUserFaceAsync`

---

## 4. 端点

### E1. `GetCheckinDataAsync` — 获取打卡记录数据

**签名（三个子接口各一支，路由相同、返回类型不同）**

```csharp
// 自建 / 代开发（现代结构）
Task<GetCheckinDataResponse> GetCheckinDataAsync(
    [Body] GetCheckinDataRequest request,
    CancellationToken cancellationToken = default);

// 第三方（旧字段结构）
Task<GetThirdPartyCheckinDataResponse> GetCheckinDataAsync(
    [Body] GetCheckinDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/getcheckindata` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 · 代开发（现代结构）/ 第三方（旧结构） |
| 请求体编码 | `application/json`，源生成序列化（`CheckinJsonContext`） |

**请求参数**（三种应用类型共用一个请求 DTO）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Opencheckindatatype` | `opencheckindatatype` | `int?` | 自建/代开发：是；第三方文档页标注否 | `1` 上下班打卡 · `2` 外出打卡 · `3` 全部打卡 |
| `request.Starttime` | `starttime` | `long?` | 是 | 开始时间（Unix 秒） |
| `request.Endtime` | `endtime` | `long?` | 是 | 结束时间（Unix 秒） |
| `request.UserIdList` | `useridlist` | `List<string>?` | 是 | 不超过 100 个，超过须分批 |

```json
{
  "opencheckindatatype": 3,
  "starttime": 1790000000,
  "endtime": 1790086400,
  "useridlist": ["zhangsan", "lisi"]
}
```

**响应参数 — 现代结构（自建 / 代开发）**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 统一响应头 |
| `checkindata` | `Checkindata` | `List<CheckinRecord>?` | 打卡记录 |
| `checkindata[].userid` | `Userid` | `string?` | 成员 userid |
| `checkindata[].groupname` | `Groupname` | `string?` | 规则名 |
| `checkindata[].checkin_type` | `CheckinType` | **`string?`** | 打卡类型，**中文字符串**（如「上班打卡」） |
| `checkindata[].exception_type` | `ExceptionType` | `string?` | 异常类型，中文字符串，多个以**分号**分隔 |
| `checkindata[].checkin_time` | `CheckinTime` | `long?` | 打卡时间（Unix 秒） |
| `checkindata[].sch_checkin_time` | `SchCheckinTime` | `long?` | 标准（应）打卡时间 |
| `checkindata[].location_title` / `location_detail` | 同名 | `string?` | 打卡地点名称 / 详情 |
| `checkindata[].lat` / `lng` | `Lat` / `Lng` | `long?` | 经纬度 × **1000000**（GCJ-02） |
| `checkindata[].deviceid` | `Deviceid` | `string?` | 打卡设备 id |
| `checkindata[].wifiname` / `wifimac` | 同名 | `string?` | Wi-Fi 名称 / MAC |
| `checkindata[].notes` | `Notes` | `string?` | 打卡备注 |
| `checkindata[].mediaids` | `Mediaids` | `List<string>?` | 附件媒体 id（下载走媒体域 `media/get`） |
| `checkindata[].groupid` / `schedule_id` / `timeline_id` | 同名 | `long?` | 规则 / 班次 / 时段 id |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "checkindata": [
    {
      "userid": "zhangsan",
      "groupname": "默认规则",
      "checkin_type": "上班打卡",
      "exception_type": "时间异常",
      "checkin_time": 1790030000,
      "sch_checkin_time": 1790028000,
      "lat": 0,
      "lng": 0,
      "groupid": 1
    }
  ]
}
```

**响应参数 — 第三方旧结构**

| 字段（JSON） | 属性（C#） | 类型 | 与现代结构的差异 |
|---|---|---|---|
| `checkindata[].checkin_type` | `CheckinType` | **`int?`** | 参数表标字符串、返回示例为整数，SDK 按示例建模（CK5） |
| `checkindata[].agency_name` | `AgencyName` | `string?` | 现代结构**没有**该字段 |
| `checkindata[].location_title_lat` / `location_title_lng` | `LocationTitleLat` / `LocationTitleLng` | `long?` | 替代现代的 `lat` / `lng` |
| `checkindata[].schedule_checkin_time` | `ScheduleCheckinTime` | `long?` | 替代现代的 `sch_checkin_time`，且此处是**时间戳** |
| — | — | — | 旧结构**没有** `lat`/`lng`/`deviceid`/`sch_checkin_time` |

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

// ③ 记录族父接口零端点且不可注入，必须注入应用类型子接口
//    代开发 → IWechatWorkProviderCheckinRecordService；第三方 → IWechatWorkThirdPartyCheckinRecordService（旧结构）
var records = provider.GetRequiredService<IWechatWorkInternalCheckinRecordService>();

var from = new DateTimeOffset(2026, 10, 28, 0, 0, 0, TimeSpan.FromHours(8));
var to = from.AddDays(1);
var userIds = new List<string> { "zhangsan", "lisi" };   // 官方上限 100，超过须分批

var resp = await records.GetCheckinDataAsync(
    new GetCheckinDataRequest
    {
        Opencheckindatatype = 3,                      // 1 上下班 / 2 外出 / 3 全部
        Starttime = from.ToUnixTimeSeconds(),         // 跨度 ≤ 30 天（自建/代开发）
        Endtime = to.ToUnixTimeSeconds(),
        UserIdList = userIds,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/checkin/getcheckindata");

// userid / 经纬度 / 备注都是个人数据：只做业务处理，不要写进日志。
logger.LogInformation("拉取打卡记录 {Count} 条", resp.Checkindata?.Count ?? 0);
```

**本端点注意事项**

- **`checkin_type` 的两种形态**：现代结构是中文字符串（`string?`），第三方旧结构是整数（`int?`）。**不要**写「等于某常量才算通过」的判定——中文字符串的完整取值集官方未在注释中列举完整。
- **`exception_type` 多值用分号分隔**，不是数组。
- **`lat`/`lng` 是度数的 1000000 倍**（`long`）：想得到度数须除以 `1_000_000`，且坐标系为 **GCJ-02**（不是 WGS84）。
- **`schedule_checkin_time` 同名不同义**：本端点第三方结构里是**时间戳**；补卡请求（E2）里是**偏移秒**。
- 第三方跨度按打卡类型分档且**系统自动校验、超过直接拒绝**（不是截断）。
- 频率：**600 次/分钟**（自建/代开发注释留档）。

### E2. `PunchCorrectionAsync` — 为打卡人员补卡（仅自建）

**签名**

```csharp
Task<WechatWorkResponse> PunchCorrectionAsync(
    [Body] PunchCorrectionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/punch_correction` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | **仅自建**（代开发/第三方官方标注暂不支持） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Userid` | `userid` | `string?` | 是 | 补卡成员 |
| `request.ScheduleDateTime` | `schedule_date_time` | `long?` | 是 | **应打卡日期当天 0 点**的 Unix 秒 |
| `request.ScheduleCheckinTime` | `schedule_checkin_time` | `int?` | 否 | 应打卡时间点，**相对当天 0 点的偏移秒**（`32400` = 9:00）；无规则对应的卡点（休息日 / 无规则 / 自由上下班）**不传** |
| `request.CheckinTime` | `checkin_time` | `long?` | 是 | 实际打卡时间（Unix 秒），与 `schedule_checkin_time` 的差值决定正常/迟到/早退 |
| `request.Remark` | `remark` | `string?` | 否 | 备注，≤ **512 字节** |

```json
{
  "userid": "zhangsan",
  "schedule_date_time": 1790000000,
  "schedule_checkin_time": 32400,
  "checkin_time": 1790030600,
  "remark": "忘记打卡，补录"
}
```

**调用示例**

```csharp
// 承接 E1 的装配与 records 实例。

var punched = await records.PunchCorrectionAsync(
    new PunchCorrectionRequest
    {
        Userid = "zhangsan",
        ScheduleDateTime = from.ToUnixTimeSeconds(),   // 应打卡日期当天 0 点
        ScheduleCheckinTime = 32400,                    // 9:00，自由上下班等无卡点场景不要传
        CheckinTime = from.AddHours(9).ToUnixTimeSeconds(),
        Remark = "示例备注",
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(punched, "/cgi-bin/checkin/punch_correction");
```

**本端点注意事项**

- **审批中的审批打卡不能补卡**；补卡后原审批打卡信息会被**清除**。补卡前若存在进行中的补卡审批，先确认状态。
- 三种时间形态并存：`schedule_date_time`（日期 0 点时间戳）、`schedule_checkin_time`（**偏移秒**）、`checkin_time`（时间戳）——不要互相顶替。
- 频率 **600 次/分钟**。

### E3. `AddCheckinRecordAsync` — 添加打卡记录（仅自建）

**签名**

```csharp
Task<WechatWorkResponse> AddCheckinRecordAsync(
    [Body] AddCheckinRecordRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/add_checkin_record` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | **仅自建** |

**请求参数**

顶层只有 `records`（`List<CheckinAddRecordItem>?`）。元素字段：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Records[].Userid` | `userid` | `string?` | 是 | 成员 userid |
| `Records[].CheckinTime` | `checkin_time` | `long?` | 是 | 打卡时间（Unix 秒） |
| `Records[].LocationTitle` | `location_title` | `string?` | 是 | 地点名称，≤ 1024 字符 |
| `Records[].LocationDetail` | `location_detail` | `string?` | 是 | 地点详情，≤ 1024 字符 |
| `Records[].DeviceType` | `device_type` | `int?` | 是 | `1` 门禁 · `2` 考勤机（人脸/指纹）· `3` 其他 |
| `Records[].DeviceDetail` | `device_detail` | `string?` | 是 | 设备品牌（**字符串**），≤ 40 字符 |
| `Records[].Notes` | `notes` | `string?` | 否 | 备注，≤ 1024 字符 |
| `Records[].Wifiname` / `Wifimac` | 同名 | `string?` | 否 | 传 `wifiname` 时 `wifimac` **必填**，且须满足六段冒号分隔正则 |
| `Records[].Mediaids` | `mediaids` | `List<string>?` | 否 | **最多 1 个** |
| `Records[].Lat` / `Lng` | 同名 | `long?` | 否 | 度数 × 1000000；范围 `[-90000000, 90000000]` / `[-180000000, 180000000]` |

```json
{
  "records": [
    {
      "userid": "zhangsan",
      "checkin_time": 1790030000,
      "location_title": "示例地点",
      "location_detail": "示例详情",
      "device_type": 2,
      "device_detail": "示例品牌"
    }
  ]
}
```

**调用示例**

```csharp
// 承接 E1 的装配与 records 实例。

var added = await records.AddCheckinRecordAsync(
    new AddCheckinRecordRequest
    {
        Records = new List<CheckinAddRecordItem>
        {
            new CheckinAddRecordItem
            {
                Userid = "zhangsan",
                CheckinTime = from.AddHours(9).ToUnixTimeSeconds(),
                LocationTitle = "示例地点",
                LocationDetail = "示例详情",
                DeviceType = 2,
                DeviceDetail = "示例品牌",
            },
        },
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(added, "/cgi-bin/checkin/add_checkin_record");
```

**本端点注意事项**

- **一批最多 200 条**；超出须分批。
- 官方参数表把顶层 `records` 与元素字段混在一张表且未标嵌套关系——**嵌套结构以官方请求示例为准**（SDK 已按嵌套建模）。
- 写入的记录会匹配打卡规则后参与明细与统计展示；若原卡点为审批打卡，更新后原审批打卡信息会**被清除**。
- 频率 **60 次/分钟**。

### E4. `AddCheckinUserFaceAsync` — 录入打卡人员人脸信息（仅自建）

**签名**

```csharp
Task<WechatWorkResponse> AddCheckinUserFaceAsync(
    [Body] AddCheckinUserFaceRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/checkin/addcheckinuserface` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | **仅自建** |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Userid` | `userid` | `string?` | 官方标注「否」（照抄） | 成员 userid |
| `request.Userface` | `userface` | `string?` | 官方标注「否」（照抄） | 人脸图片数据，**base64 处理后填入**，**不是 media_id** |

```json
{ "userid": "zhangsan", "userface": "<base64 人脸图片数据>" }
```

**调用示例**

```csharp
// 承接 E1 的装配与 records 实例。

// userface 是 base64 图片数据（≤1M），不是 media_id；示例用占位串。
var face = await records.AddCheckinUserFaceAsync(
    new AddCheckinUserFaceRequest
    {
        Userid = "zhangsan",
        Userface = Convert.ToBase64String(Array.Empty<byte>()),   // 实际传入真实图片字节
    },
    cancellationToken);

WechatWorkException.ThrowIfFailed(face, "/cgi-bin/checkin/addcheckinuserface");
```

**本端点注意事项**

- **对已有人脸的用户会覆盖原人脸**，操作前确认业务授权（这是生物识别数据，通常需单独的用户同意流程）。
- **图片 ≤ 1M**；频率 **10 次/分钟**——本域最严格，批量录入必须自行排队。
- 专属错误码：`301021` 输入参数错误 · `301069` userid 无对应成员 · `301070` 系统错误 · `301071` 企业内存在相似人脸（**此时人脸仍然录入成功**）· `301072` 图像数据错误请更换图片。
- 人脸数据**不得写日志、遥测或异常消息**；`301071` 也不要顺手把人脸或相似成员信息打进消息里。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 父接口 `IWechatWorkCheckinRecordService` | **零端点**且不可注入 | 解析父接口失败（未注册 DI）；想用父接口统一处理三分支 → 类型对不上 |
| 现代结构 vs 第三方旧结构 | `checkin_type` 字符串 vs 整数；`lat`/`lng` vs `location_title_lat`/`location_title_lng`；`sch_checkin_time` vs `schedule_checkin_time` | 用 `CheckinRecord` 去接第三方响应 → 字段静默为 `null`，统计全空 |
| `schedule_checkin_time` 的两种形态 | 补卡**请求**是偏移秒（`int?`）；第三方记录**响应**是时间戳（`long?`） | 互相顶替 → 时间错到几十年之外 |
| `checkin_time` 与 `sch_checkin_time` | 前者是实际打卡，后者是**应**打卡时间 | 用实际时间做「是否迟到」判定 → 永远不迟到 |
| `lat`/`lng` 单位 | 度数的 1000000 倍，GCJ-02 | 当度数用 → 位置跑到海里 |
| `useridlist` ≤100 / 跨度 ≤30 天 | 官方硬闸，SDK 不裁剪 | 超限请求照样发出，回 errcode；第三方跨度按类型分档且超限拒绝 |
| 补卡 vs 添加打卡记录 | 补卡作用于「已有规则卡点」；添加记录是写入一条原始流水 | 用补卡补「自由上下班」卡点 → 需省略 `schedule_checkin_time`，否则不匹配 |
| 录入人脸 vs 媒体上传 | `userface` 是 **base64 图片**，不是 media_id | 传 media_id → `301072` 或参数错误 |
| 设备打卡数据（[Device.md](./Device.md)） | 那是 `hardware` 域的原始设备流水（4 字段） | 与本族 E1 混为一谈 → 取不到地点/规则等字段 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 301021 | 官方业务码 | 输入参数错误 | E4 主码；其余端点参数错误也可能落此类 |
| 301069 | 官方业务码 | userid 无对应成员 | E4 专属 |
| 301070 | 官方业务码 | 系统错误 | E2/E3/E4 均可能出现 |
| 301071 | 官方业务码 | 企业内存在相似人脸（**人脸仍录入成功**） | E4 专属：不要按「失败」重试 |
| 301072 | 官方业务码 | 人脸图像数据错误，请更换图片 | E4 专属 |
| 42001 / 40014 / 42007 | `ExpiredAccessToken` / `InvalidAccessToken` / `RelatedAccessTokenInvalid` | 令牌过期/失效 | 自愈集合 |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 套件/服务商令牌失效 | 本族不消费；出现即凭据族错配 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

其余码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `checkin_type` / `exception_type` 中文字符串的完整取值集 | 注释列举「上班打卡/下班打卡/外出打卡」「时间异常/地点异常/未打卡/wifi 异常/非常用设备」，**是否全集未逐项核验** | 官方 90262 返回参数表 |
| 第三方 `checkin_type` 整数取值域 | 参数表标字符串、示例为整数，SDK 按示例建模，**取值全集未核验** | 官方 94205 |
| 媒体附件下载面 | `mediaids` 指向媒体域 `media/get`，**未在本域核验** | 媒体域文档 |
| `301071`「人脸仍录入成功」的最终落库形态 | 注释明确「仍然会录入成功」，但相似人脸冲突的处理建议未留档 ⇒ **未逐项核验** | 官方 93378 |
| 官方文档正文 | 本文只摘录本仓 XML 注释；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md)（§1 父接口零端点、§3 四种时间形态、§10 三支仅自建写端点）
- 兄弟族：[Rule.md](./Rule.md)（打卡规则）· [Report.md](./Report.md)（日报/月报）· [Schedule.md](./Schedule.md)（排班）· [Device.md](./Device.md)（设备打卡数据）
- 官方页：[获取打卡记录数据（自建 90262）](https://developer.work.weixin.qq.com/document/path/90262) · [补卡（95803）](https://developer.work.weixin.qq.com/document/path/95803) · [添加打卡记录（99647）](https://developer.work.weixin.qq.com/document/path/99647) · [录入人脸（93378）](https://developer.work.weixin.qq.com/document/path/93378)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
