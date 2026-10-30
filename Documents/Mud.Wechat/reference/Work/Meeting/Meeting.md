# 预约会议基础管理 · 会议创建、修改、取消与查询

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 预约会议基础管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingService.cs)（父，承载 4 支公共端点，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingService_Internal.cs)（+1：`get_info`）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingService_ThirdParty.cs)（+1：`get_info`）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingService_Provider.cs)（**零端点空标记**） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、无子目录，`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`CreateMeetingRequest/Response`、`UpdateMeetingRequest/Response`、`CancelMeetingRequest`、`GetUserMeetingIdListRequest/Response`、`GetMeetingInfoRequest/Response` 与共用结构 `MeetingSettings` / `MeetingReminders` / `MeetingInvitees` / `MeetingHosts` / `MeetingRingUsers` / `MeetingGuest` / `MeetingAttendees` / `MeetingAttendeeMember` / `MeetingTmpExternalUser` / `MeetingSubMeeting`） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族 `MeetingBaseRoutes` 4 + `MeetingInfoRoutes` 2、全域 `allRoutes` **恰 105 条**且唯一重复即 `get_info` / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 4, 自建 1 / 代开发 0 / 第三方 1)` / MT3 令牌绑定（24 接口）/ MT4 上下文登记（261 型，本族分解 17）/ MT5 `MeetingDataModels_ShouldLockOfficialContractTraps`）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建 [99104](https://developer.work.weixin.qq.com/document/path/99104) / 第三方 98148 / 代开发 93706 / 旧页 97454 · 修改 99047 / 98154 / 93710 / 97455 · 取消 99048 / 98153 / 93709 / 97456 · 成员会议 ID 99050 / 98714 / 93707 / 97457 · 详情 99049 / 98149（自建与第三方）· 93708（第三方旧页） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（创建 → 成对修改 → 详情 → 取消））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1~MT5 + TO1~TO3 + G5 断言 + 本族 4 份接口文件 XML 注释 + 261 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 1/2/3/4/7/8/10/12 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

预约会议的**主干生命周期**，5 条路由 / 6 个方法声明（`get_info` 在自建与第三方各声明一次，同路由）：

| # | 方法 | 路由 | 开放面 |
|---|---|---|---|
| E1 | `CreateMeetingAsync` | `POST /cgi-bin/meeting/create` | 自建 + 第三方 + 代开发（公共面） |
| E2 | `UpdateMeetingAsync` | `POST /cgi-bin/meeting/update` | 同上 |
| E3 | `CancelMeetingAsync` | `POST /cgi-bin/meeting/cancel` | 同上 |
| E4 | `GetUserMeetingIdListAsync` | `POST /cgi-bin/meeting/get_user_meetingid` | 同上 |
| E5 | `GetMeetingInfoAsync` | `POST /cgi-bin/meeting/get_info` | **仅自建 + 第三方**（两个分支各声明一次，代开发无该页） |

本族是**十一族里唯一同时向三类应用开放的族**（MT2：父 4 / 自建 +1 / 第三方 +1 / 代开发 0）——其余十族全部只有自建子接口。

**本族只有「预约会议」主干**：报名、等候室、实时成员、健康度、专属入会链接等在**预约会议高级管理族**（[MeetingAdvanced.md](./MeetingAdvanced.md)，19 支、仅自建）；会中控制 [MeetingControl.md](./MeetingControl.md)；网络研讨会 [MeetingWebinar.md](./MeetingWebinar.md)；发起记录统计 [MeetingStatistics.md](./MeetingStatistics.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（E1~E4 三类等宽；E5 仅自建 + 第三方） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到授权企业作用域（`permanent_code` 只写参数名，不得进日志）。

### 2.2 权限与业务前置

- 自建应用须配置到「**协作 - 会议 - 可调用接口的应用**」列表；第三方 / 代开发须具有相应权限。
- **时间闸**：`meeting_start` 必须**大于当前时间**；`meeting_duration` **最小 300 秒、最大 86399 秒**（不足 24 小时）；结束时间由「开始 + 时长」推导，**请求侧没有 `meeting_end_time`**。
- **人数闸**：`invitees.userid` —— 普通企业最多 **100** 人；付费企业由所购在线会议室 / 高级账号容量决定、最多 **300** 人；**超过 300 人须改走高级管理族的 `set_invitees`**（单次 ≤2000，管理员必须在列表内）。**任何 userid 不合法或不在应用可见范围内直接报错**（不是静默过滤）。
- **状态闸**：E2 修改与 E3 取消**只允许作用于「当前应用创建的 + 处于预约状态」的会议**；会议进入「会议中 / 已结束」后两者都不可用，会中动作改走 `realcontrol/*`。
- **`cal_id`（与日程域的耦合点）**：≤64 字节、须为 `access_token` 对应应用创建的日历；**不填则插入参与者默认日历，第三方应用必须指定**；修改时「若会议为指定创建者的老会议，不允许指定应用身份创建的新日历」。本域**不因创建会议返回 `schedule_id`**。
- **主持人**：`settings.hosts.userid` 最多 10 个，**仅购买了会议高级/专业功能的企业可指定**，传入创建者 userid 会被官方自动过滤。
- 时长与文案上限：`title` ≤ **40 字节或 20 个 utf8 字符**；`description` ≤ **500 字节**；`location` ≤ **128 字符**（读取侧详情注释另给 `title` ≤60 字节 / `description` ≤600 字节 ⇒ 两侧不一致，见 §7）。
- 凭据类字段：`settings.password`（入会密码，**仅 4-6 位纯数字**）与 `settings.host_key`（主持人密钥，**6 位数字字符串**）⇒ 只写参数名、不回显、不进日志。
- 参会人 userid、手机号（嘉宾）、外部成员临时 ID 均属**个人数据**：示例一律假值，参会名单与入退会时间不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E4 | ✅ | ✅ | ✅ |
| E5 `get_info` | ✅ | ✅（99049 / 93708） | ❌（代开发章节无该页 ⇒ `_Provider` 为零端点空标记） |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册会议模块（十一族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMeetingApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Meeting` |
| 注册入口 | `WechatWorkServiceBuilder.AddMeetingApi()`（→ 源生成 `AddMeetingWebApiHttpClient()`） |
| 注册组名 | `"Meeting"`（十一族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMeetingService` / `IWechatWorkThirdPartyMeetingService` / `IWechatWorkProviderMeetingService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMeetingService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本域必填性不用类型表达**：`admin_userid` / `title` / `meeting_start` / `meeting_duration` 注释标「官方必填」但类型仍是 `string?` / `long?` / `int?` ⇒ 缺字段不会编译报错，只能靠调用方自检（与 MeetingRoom 域 MR8 的做法相反）。条件必填（如周期性会议的字段组合）同样不本地拦截。

### E1. `CreateMeetingAsync` — 创建预约会议

**签名**

```csharp
[Post("/cgi-bin/meeting/create")]
Task<CreateMeetingResponse> CreateMeetingAsync(
    [Body] CreateMeetingRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/create` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/create?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| 请求体 | `CreateMeetingRequest` |

**请求体** `CreateMeetingRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `admin_userid` | `AdminUserid` | `string?` | **是** | 会议管理员 userid；发起人与企业内部参与人**须在应用可见范围内** |
| `title` | `Title` | `string?` | **是** | **≤40 字节或 20 个 utf8 字符** |
| `meeting_start` | `MeetingStart` | `long?` | **是** | Unix 秒，**须大于当前时间** |
| `meeting_duration` | `MeetingDuration` | `int?` | **是** | 秒，**300 ~ 86399** |
| `description` | `Description` | `string?` | 否 | ≤500 字节 |
| `location` | `Location` | `string?` | 否 | ≤128 字符 |
| `agentid` | `Agentid` | `int?` | 否 | **仅旧的第三方多应用套件需要填** |
| `invitees` | `Invitees` | `MeetingInvitees?` | 否 | `{userid: []}`——**不合法/不可见即报错**；上限 100 / 300（超 300 走 `set_invitees`） |
| `guests` | `Guests` | `List<MeetingGuest>?` | 否 | 嘉宾（`area` / `phone_number` / `guest_name`，`area` 中国传 **`86`**，不是 `+86` 也不是 `0086`；姓名 1~16 位）——**仅高级管理文档页声明** |
| `cal_id` | `CalId` | `string?` | 条件 | ≤64 字节；**第三方应用必须指定**；不填则插入默认日历 |
| `settings` | `Settings` | `MeetingSettings?` | 否 | 会议配置（下表） |
| `reminders` | `Reminders` | `MeetingReminders?` | 否 | 周期性会议配置（见 E2 后说明） |

`MeetingSettings`（**创建 / 修改 / 详情三处共用的并集超集**，MT5 断言）：

| 字段 | 属性 | 类型 | 取值口径 |
|---|---|---|---|
| `password` | `Password` | `string?` | 入会密码，**仅 4-6 位纯数字**（官方参数表标 bool、示例按字符串传 ⇒ 本仓 `string?`） |
| `enable_waiting_room` | `EnableWaitingRoom` | `bool?` | 默认不开启 |
| `allow_enter_before_host` | `AllowEnterBeforeHost` | `bool?` | 默认允许 |
| `remind_scope` | `RemindScope` | `int?` | 1 不提醒 / 2 仅提醒主持人 / 3 提醒所有成员 / **4 指定部分成员响铃**（配合 `ring_users`）；创建页默认 2、详情页默认 3 |
| `enable_enter_mute` | `EnableEnterMute` | `int?` | 1 开启 / 0 关闭 / **2 超 6 人自动开启（默认）** |
| `enable_screen_watermark` | `EnableScreenWatermark` | `bool?` | 默认不开启 |
| `hosts` | `Hosts` | `MeetingHosts?` | 主持人 `userid[]`，最多 10、创建者被自动过滤、需购买高级功能 |
| `ring_users` | `RingUsers` | `MeetingRingUsers?` | `remind_scope=4` 时的响铃名单；**该值为 4 且列表为空 ⇒ 全部成员不响铃** |
| `allow_unmute_self` / `mute_all` | `AllowUnmuteSelf` / `MuteAll` | `bool?` | **两者单独使用不生效**：`allow_unmute_self=false` 时需指定 `mute_all=true` |
| `allow_external_user` | `AllowExternalUser` | `bool?` | 是否允许外部成员入会（默认允许）——**只是开关，不返回外部联系人 ID** |
| `watermark_type` | `WatermarkType` | `int?` | 0 单排 / 1 多排（屏幕水印开启时生效） |
| `auto_record_type` | `AutoRecordType` | `string?` | `none` / `local` / `cloud`；**依赖企业账户设置，企业锁定后须与企业配置一致** |
| `attendee_join_auto_record` | `AttendeeJoinAutoRecord` | `bool?` | 仅 `auto_record_type=cloud` 时可配 |
| `enable_host_pause_auto_record` | `EnableHostPauseAutoRecord` | `bool?` | 仅 `cloud` 时可配 |
| `enable_interpreter` | `EnableInterpreter` | `bool?` | 同声传译（**仅创建页声明**） |
| `enable_enroll` | `EnableEnroll` | `bool?` | 激活会议报名（配合高级管理族 `enroll/*`） |
| `enable_host_key` / `host_key` | `EnableHostKey` / `HostKey` | `bool?` / `string?` | 主持人密钥：**`host_key` 是 6 位数字字符串**，开启后不填则官方自动分配 |
| `enable_doc_upload_permission` / `current_hosts` / `co_hosts` | 同名 | `bool?` / `MeetingHosts?` | **仅获取详情响应返回** |

**返回** `CreateMeetingResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `meetingid` | `Meetingid` | `string?` | 会议 ID（JSON 名 **`meetingid`，无下划线**）；可用于小程序 / JS-SDK「进入会议」 |
| `excess_users` | `ExcessUsers` | `List<string>?` | 参会人中无效会议账号的 userid 列表（**仅购买会议专业版且存在无效账号时返回**） |
| `meeting_code` | `MeetingCode` | `string?` | 会议号——**仅高级管理文档页声明**，基础面不承诺 |
| `meeting_link` | `MeetingLink` | `string?` | 入会链接——同上 |

**调用示例**（创建 → 改期 → 查详情 → 取消；周期性会议取消必须带子会议 ID）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMeetingApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Meeting;      // 请求 / 响应 DTO（全域平铺同一命名空间）

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册会议模块（十一族同组）
builder.Services.AddWechatWorkServices(b => b.AddMeetingApi());

// ③ 注入自建子接口（第三方 / 代开发先 UseCorpScope；代开发无 get_info）
var meetings = provider.GetRequiredService<IWechatWorkInternalMeetingService>();

// ④ E1 创建：meeting_start 须大于当前时间、时长 300~86399 秒、invitees 须在可见范围内
long startAt = DateTimeOffset.Now.AddMinutes(30).ToUnixTimeSeconds();
var created = await meetings.CreateMeetingAsync(new CreateMeetingRequest
{
    AdminUserid = "demo_user_0001",          // 假值，勿用真人 userid
    Title = "季度评审",                       // ≤40 字节或 20 utf8 字符
    MeetingStart = startAt,
    MeetingDuration = 3600,
    Location = "线上",
    Invitees = new MeetingInvitees { Userid = new List<string> { "demo_user_0002" } },
    Settings = new MeetingSettings
    {
        EnableWaitingRoom = true,
        EnableEnterMute = 2,                 // 2 = 超过 6 人自动开启（默认）
        RemindScope = 2,                     // 2 = 仅提醒主持人
        AllowUnmuteSelf = false,
        MuteAll = true,                      // allow_unmute_self=false 时必须同时指定 mute_all=true
        AutoRecordType = "cloud",
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/meeting/create");
string meetingid = created.Meetingid!;
// created.MeetingCode / MeetingLink 仅高级管理文档页声明 ⇒ 基础面可能为空
logger.LogInformation("会议已创建（会议号是否可得：{HasCode}）", created.MeetingCode is not null);

// ⑤ E2 修改：meeting_start 与 meeting_duration 必须成对提交（改其一须同时传其二）
var updated = await meetings.UpdateMeetingAsync(new UpdateMeetingRequest
{
    Meetingid = meetingid,                   // 仅预约状态可改、仅本应用创建的会议
    MeetingStart = startAt + 3600,
    MeetingDuration = 5400,
    RemindTime = 900,                        // 与 reminders.remind_before（数组）不是同一概念
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/meeting/update");

// ⑥ E5 查详情：meetingid 与 meeting_code 至少填一个；状态 1 待开始 / 2 会议中 / 3 已结束 / 4 已取消 / 5 已过期
var info = await meetings.GetMeetingInfoAsync(new GetMeetingInfoRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/meeting/get_info");
logger.LogInformation("会议状态 {Status}，企业内部成员 {MemberCount} 人、外部临时成员 {ExternalCount} 人",
    info.Status,
    info.Attendees?.Member?.Count ?? 0,
    info.Attendees?.TmpExternalUser?.Count ?? 0);

// ⑦ E3 取消：一次性会议只需 meetingid；周期性会议不传 sub_meetingid 会取消整个系列
var cancelled = await meetings.CancelMeetingAsync(new CancelMeetingRequest
{
    Meetingid = meetingid,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(cancelled, "/cgi-bin/meeting/cancel");
```

### E2. `UpdateMeetingAsync` — 修改预约会议

**签名**

```csharp
[Post("/cgi-bin/meeting/update")]
Task<UpdateMeetingResponse> UpdateMeetingAsync(
    [Body] UpdateMeetingRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `UpdateMeetingRequest`：`meetingid`（官方必填，**仅预约状态可改**）+ `title` / `meeting_start` / `meeting_duration` / `description` / `location` / `remind_time` / `agentid` / `invitees` / `cal_id` / `settings` / `reminders`（其余全为可空）。

两条独有口径：
- **`meeting_start` 与 `meeting_duration` 必须成对修改**（接口注释与 DTO 类注释双处留档）。
- `remind_time`（`int?`，开始前 N 秒提醒，默认 0）**与 `reminders.remind_before`（数组）不是同一个概念**。

**返回** `UpdateMeetingResponse` 只有 `excess_users`（`List<string>?`）。

**`MeetingReminders`（周期性会议，创建 / 修改 / 详情三处共用同一型）**：

| 字段 | 属性 | 类型 | 口径 |
|---|---|---|---|
| `is_repeat` | `IsRepeat` | `int?` | **1 周期性 / 0 非周期性（默认）**——数值承载，非 bool |
| `repeat_type` | `RepeatType` | `int?` | **0 每天 / 1 每周 / 2 每月 / 7 每个工作日**（默认 0，仅周期性生效） |
| `repeat_interval` | `RepeatInterval` | `int?` | **基础页：仅 `repeat_type=1`（每周）支持且值 ≤2**；高级页：自定义重复时必填，含义随 `repeat_type` 变 |
| `is_custom_repeat` | `IsCustomRepeat` | `int?` | 0 否 / 1 是（仅 `is_repeat=1` 生效）；自定义时**仅能选每日 / 每周 / 每月** |
| `repeat_day_of_week` | `RepeatDayOfWeek` | `List<int>?` | 1~7（周一至周日）；自定义按周时**开始日须包含在数组里**，未指定后台自动补 |
| `repeat_day_of_month` | `RepeatDayOfMonth` | `List<int>?` | 1~31；自定义按月时同上 |
| `repeat_until_type` | `RepeatUntilType` | `int?` | **0 按日期 / 1 按次数（默认 0）**——官方参数表误写 `uint32[]`，本仓按单值 int |
| `repeat_until_count` | `RepeatUntilCount` | `int?` | 每天 / 每个工作日 / 每周最多 **200** 场，每月最多 **50** 场；**未填默认 7 次** |
| `repeat_until` | `RepeatUntil` | `long?` | Unix 时间戳；超出或未设时取官方上限 |
| `remind_before` | `RemindBefore` | `List<int>?` | **仅支持 0 / 300 / 900 / 3600 / 86400**；传其它值表现为「会议开始时提醒」；默认不提醒 |

### E3. `CancelMeetingAsync` — 取消预约会议

**签名**

```csharp
[Post("/cgi-bin/meeting/cancel")]
Task<WechatWorkResponse> CancelMeetingAsync(
    [Body] CancelMeetingRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填，仅预约状态、仅本应用创建的会议）+ `sub_meetingid`（`string?`，**仅高级管理文档页声明**；**不传则取消该系列的周期性会议** ⇒ 只取消单次必须显式带子会议 ID）。

**返回**：`WechatWorkResponse`（无业务负载，MT1 断言不得新建空响应型）。

### E4. `GetUserMeetingIdListAsync` — 获取成员会议 ID 列表

**签名**

```csharp
[Post("/cgi-bin/meeting/get_user_meetingid")]
Task<GetUserMeetingIdListResponse> GetUserMeetingIdListAsync(
    [Body] GetUserMeetingIdListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`userid`（官方必填）+ `cursor`（**初次调用可填 `"0"`**）+ `limit`（**默认值与最大值均为 100**）+ `begin_time` / `end_time`（Unix 秒，**跨度 ≤180 天**；两者都不填时官方默认 `end_time` 为当前时间）。

**返回**：`next_cursor`（当前数据最后一个 key；**未返回或空字符串表示已取完**）+ `meetingid_list`（`List<string>?`，**可能为空**）。

### E5. `GetMeetingInfoAsync` — 获取会议详情（同路由、自建与第三方各声明一次）

**签名**

```csharp
[Post("/cgi-bin/meeting/get_info")]
Task<GetMeetingInfoResponse> GetMeetingInfoAsync(
    [Body] GetMeetingInfoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` 与 `meeting_code` **至少填一个**（`meeting_code` 仅高级管理文档页声明）+ `sub_meetingid`（同上）。

**返回** `GetMeetingInfoResponse`（详情面比创建面宽，且**读写上限不同**）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `admin_userid` / `title` / `meeting_start` / `meeting_duration` / `description` / `location` | 同名 | — | **`title` ≤60 字节、`description` ≤600 字节**（与请求侧 40/500 不一致） |
| `main_department` | `MainDepartment` | `int?` | 发起人所在部门 |
| `status` | `Status` | `int?` | **1 待开始 / 2 会议中 / 3 已结束 / 4 已取消 / 5 已过期** |
| `meeting_type` | `MeetingType` | `int?` | **0 一次性 / 1 周期性 / 2 微信专属 / 3 Rooms 投屏 / 5 个人会议号 / 6 网络研讨会**（仅高级页声明） |
| `agentid` / `meeting_code` / `meeting_link` / `cal_id` | 同名 | — | 应用与入会入口 |
| `attendees` | `Attendees` | `MeetingAttendees?` | `member[]`（`MeetingAttendeeMember`）+ `tmp_external_user[]`（`MeetingTmpExternalUser`，**微信入会用户不返回**） |
| `settings` / `reminders` / `guests` | 同名 | — | 与写入侧共用同一型 |
| `has_vote` | `HasVote` | `bool?` | 是否存在投票（仅高级页声明；**仅创建人与主持人有权查询**） |

参会人状态面（`MeetingAttendeeMember` / `MeetingTmpExternalUser` 同构）：`status`（**1 已参与 / 2 未参与**）、`first_join_time`、`last_quit_time`、`total_join_count`、`cumulative_time`（秒）；临时外部 ID 另有 `tmp_external_userid`——**同一用户在不同会议中取值不同**。

> **`get_info` 的所有权限制**：只能拉取**本应用创建**的会议；**快速会议仅返回已参与成员列表**。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`meetingid` 无下划线** | 单数会议 ID 官方即 `meetingid`；`meeting_code` / `meeting_start` / `meeting_duration` 才有下划线；批量侧是 `meetingid_list`（MT5 锁定） | 写成 `meeting_id` → 官方收不到 |
| **创建应答不等于详情面** | `meeting_code` / `meeting_link` **仅高级管理文档页声明**，基础面不承诺 ⇒ 展示入会入口应以详情接口取值为准 | 假定创建必回链接 |
| 状态机与所有权 | E2/E3 要求「本应用创建 + 预约状态」；E5 只读本应用创建的会议 | 会议已开始还去 update / cancel |
| **成对修改** | E2 的 `meeting_start` 与 `meeting_duration` 必须同时传 | 只改开始时间 → 官方拒绝或时长漂移 |
| 两个「提醒」不是同一概念 | `update.remind_time`（`int?` 单值）vs `reminders.remind_before`（`List<int>`，仅 0/300/900/3600/86400） | 混用导致提醒不生效 |
| **周期性取消的默认动作** | `cancel` 不传 `sub_meetingid` ⇒ **取消整个系列** | 想停一次却把系列全删 |
| `invitees` 两种同名不同形 | 创建/修改请求里是 `MeetingInvitees{userid: List<string>}`（对象包数组）；高级族 `get_invitees`/`set_invitees` 里是 `MeetingInvitee`（**对象数组**）⇒ 不可互换（MT5） | 复制结构体导致序列化错配 |
| 超 300 人不是报错而是换端点 | 普通 100 / 付费 300 是创建面容量；更大规模须用 `set_invitees`（≤2000，管理员必须在列表内） | 以为 SDK 会分批 |
| **userid 不可见直接报错** | `invitees.userid` 与 `admin_userid` 越界不静默过滤 | 混进不可见成员 → 整个请求失败 |
| `settings` 是并集超集 | `enable_doc_upload_permission` / `current_hosts` / `co_hosts` **仅详情回传**；`enable_interpreter` 仅创建页声明；未赋值不输出 | 在创建请求里设 `current_hosts` |
| `mute_all` 与 `allow_unmute_self` | **单独使用不生效**：`allow_unmute_self=false` 须同时 `mute_all=true` | 只设一个 → 静音不落地 |
| `password` 与 `host_key` 是字符串 | 入会密码 4-6 位纯数字、主持人密钥 6 位数字**字符串**（官方参数表标 bool、示例按字符串传） | 用 `int` 承载 → 前导 0 丢失 |
| `auto_record_type` 依赖企业锁定 | 企业强制锁定后必须与企业配置一致，且 `attendee_join_auto_record` / `enable_host_pause_auto_record` **仅 `cloud` 时可配** | 与企业主张冲突而失败 |
| 读写两侧上限不一致 | 请求 `title` ≤40 字节 / `description` ≤500 字节；详情注释 `title` ≤60 / `description` ≤600 ⇒ 写入校验按请求侧口径 | 按详情侧放宽 → 创建被拒 |
| **`tmp_external_userid` 不可跨会议复用** | 同一用户在不同会议中取值不同；转成 `external_userid` 的能力**不在本域**（属 AccountId 域） | 当作稳定外部 ID 落库 |
| `allow_external_user` 只是开关 | 含义是「是否允许外部成员入会」，**不返回外部联系人 ID** | 读成外部联系人集合 |
| **`cal_id` ≠ `schedule_id`** | 本域与日程域是两套 ID；本域**不因创建会议返回 `schedule_id`**；第三方**必须**指定 `cal_id` | 拿日程 ID 当 `cal_id`，或第三方漏填 |
| 全部官方即 POST | 本族 5 条路由全是 POST；**全域 105 条只有 `layout/list_template` 是 GET** | 把 `get_info` / `get_user_meetingid` 改 GET → 打坏 MT1 与 G5 |
| 翻页 `limit` 一致性 | `cursor` + `limit` + **`next_cursor`**（MT5 锁定字段名）；`next_cursor` 为空即取完；高级族另有「后续 `limit` 必须与首次一致」 | 翻页途中改 `limit` |
| 代开发无详情端点 | `IWechatWorkProviderMeetingService` 是零端点空标记（官方代开发章节无 `get_info` 页） | 给代开发补 `GetMeetingInfoAsync` → 打坏 MT2 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验时间区间与人数上限**、**不做状态机预判**、**不做日历归属校验**、**不做本地 id 转换**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功还要看 `excess_users` 是否有人被剔除 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`admin_userid` 不可见、参会人超容量、`meeting_start` 早于当前时间、时长越界、非预约状态改 / 取消、非本应用创建、`cal_id` 归属不符等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 24 份接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 99104 / 99047 / 99048 / 99050 / 99049 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无任何 QPS / 日配额数值**（只有批次与跨度上限）⇒ 未逐项核验，**勿借用邻域数值** | 官方频率附录 90312 |
| `title` / `description` 读写上限不一致 | 40/500（请求）与 60/600（详情）两处注释并存，是否官方笔误未核验 | 官方 99104 与 99049 参数表 |
| `meeting_code` / `meeting_link` 在基础面的可得性 | 注释明示「仅高级管理文档页声明」，但自建基础页是否实际回传未核验 | 官方 99100 系列页 |
| `settings` / `reminders` 的条件必填组合 | 只留档了 `repeat_interval` 的两页口径与 `password` / `host_key` 形态，其余「何时必填」无断言 | 需要本地校验前先补 MT5 级守卫断言，勿加隐式默认值 |
| 会议附件（`userdoc` / `mediadoc` / `media_id`） | 本域接口与 261 个 DTO 全量核对后**无该形态参数**；素材上传归 Media 域 ⇒ 官方预约会议参数表是否含文档附件未在本域核验 | 官方创建预约会议页；确属日程能力则归 `Schedule` 域 |
| 快速会议的来源 | 详情注释提「快速会议仅返回已参与成员」，但本域**无创建快速会议的端点** ⇒ 其创建面未在本域建模 | 官方快速会议文档 |
| 代开发面全集 | MT2 只断言基础族代开发为空标记；其余十族代开发是否有页未逐项核验 | 官方代开发章节树 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（十一族计数表、`meetingid` 生命周期、参会人三件套、周期性与成对修改、批次上限表、字段名反直觉清单）
- 同域近邻：[MeetingAdvanced.md](./MeetingAdvanced.md)（超 300 人的 `set_invitees`、等候室、报名、专属链接、健康度）· [MeetingControl.md](./MeetingControl.md)（会议进入「会议中」后的 `realcontrol/*` 与投票）· [MeetingStatistics.md](./MeetingStatistics.md)（会议发起记录）· [MeetingWebinar.md](./MeetingWebinar.md)（网络研讨会，`title` 与字符串时间形态不同）
- 跨域边界：日历归属见 `Schedule` 域（`cal_id`）；`tmp_external_userid` 转换见 [TmpExternalUserId.md](../AccountId/TmpExternalUserId.md)；素材上传见 [Media.md](../Media/Media.md)；**OA 会议室**（`/cgi-bin/oa/meetingroom/`）与本域 Rooms 族（`/cgi-bin/meeting/rooms/`）分属两棵树，见 [Booking.md](../MeetingRoom/Booking.md)
- 官方页：[创建预约会议（99104）](https://developer.work.weixin.qq.com/document/path/99104) · [修改（99047）](https://developer.work.weixin.qq.com/document/path/99047) · [取消（99048）](https://developer.work.weixin.qq.com/document/path/99048) · [获取成员会议 ID（99050）](https://developer.work.weixin.qq.com/document/path/99050) · [获取会议详情（99049）](https://developer.work.weixin.qq.com/document/path/99049)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
