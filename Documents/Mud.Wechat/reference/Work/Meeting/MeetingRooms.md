# Rooms 会议室 · 硬件会议室的预定、台账、呼叫与应答状态

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · Rooms 会议室管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingRoomsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingRoomsService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingRoomsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingRoomsService_Internal.cs)（**唯一**子接口，承载全部 12 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`Book/Release/Call/CancelCall/ListMeetingRooms/GetMeetingRoomInfo/GetMeetingRoomConfig/ListMeetingRoomMeetings/ListRoomDevices/ListRoomControllers/GetMeetingRoomInventory/GetMeetingRoomResponseStatus` 各请求与响应，以及 `RoomsMeetingRoom`、`RoomsMeetingInfo`、`RoomsBasicInfo`、`RoomsAccountInfo`、`RoomsHardwareInfo`、`RoomsPmiInfo`、`RoomsMeetingSettings`、`RoomsRecordSettings`、`RoomsDeviceInfo`、`RoomsDeviceMonitorInfo`、`RoomsControllerInfo`、`RoomsMraAddress`；本族在 MT4 的逐族分解中计 33 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(12)`、全部挂 `rooms/` 段、**`get_inventory` 断言方法参数只余 `CancellationToken`**（无请求体的 POST）/ MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 12)` / MT3 令牌绑定 / MT4 上下文登记 / MT5「Rooms 族字段形态自成一套、`RoomsMeetingSettings` ≠ `MeetingSettings`、`RoomsControllerInfo.Status` 是字符串」）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 预定 [98791](https://developer.work.weixin.qq.com/document/path/98791) · 释放 98792 · 列表 98795 · 详情 98793 · 配置 98802 · 会议室会议列表 98796 · 设备列表 98798 · 控制器列表 98799 · 资源 98809 · 呼叫 98804 · 取消呼叫 98805 · 应答状态 98806（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（资源余量 → 会议室列表 → 预定 → 占用 → 呼叫与应答 → 释放 → 详情/配置/设备/控制器））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 / MT5 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释（含两处官方文档笔误留档）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 9/10/11 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

管理**视频会议硬件会议室（Rooms 设备）**——预定给会议、看台账、呼叫入会，12 支端点全部挂 `/cgi-bin/meeting/rooms/`：

| 层 | # | 方法 | 路由 |
|---|---|---|---|
| 与会议的绑定 | E1 | `BookMeetingRoomAsync` | `rooms/book` |
| | E2 | `ReleaseMeetingRoomAsync` | `rooms/release` |
| | E6 | `ListMeetingRoomMeetingsAsync` | `rooms/list_meetings` |
| 台账 | E3 | `ListMeetingRoomsAsync` | `rooms/list` |
| | E4 | `GetMeetingRoomInfoAsync` | `rooms/get_info` |
| | E5 | `GetMeetingRoomConfigAsync` | `rooms/get_config` |
| | E7 | `ListRoomDevicesAsync` | `rooms/list_devices` |
| | E8 | `ListRoomControllersAsync` | `rooms/list_controllers` |
| | E9 | `GetMeetingRoomInventoryAsync` | `rooms/get_inventory`（**无请求体的 POST**） |
| 呼叫 | E10 | `CallMeetingRoomAsync` | `rooms/call` |
| | E11 | `CancelCallMeetingRoomAsync` | `rooms/cancel_call` |
| | E12 | `GetMeetingRoomResponseStatusAsync` | `rooms/get_response_status` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）。

**与 MeetingRoom 域（`/cgi-bin/oa/meetingroom/`）分属两棵树**：本族管**Rooms 硬件设备**（`meeting_room_id` / `rooms_id` / MRA 信令地址），OA 域管**企业实体会议室与预定**（`meetingroom_id` 是整数）。域 README 第 11 节专门提醒：**需求写「会议室」时先确认路由前缀**，两域各写各的端点，不要在一条里补另一条的能力（对照 [Booking.md](../MeetingRoom/Booking.md) / [Room.md](../MeetingRoom/Room.md)）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（TO1~TO3） | 运行期归属校验 |

### 2.2 权限与业务前置

- 自建应用须配置在「**可调用接口的应用**」列表中；写侧与会议相关的读侧还要求「**仅允许操作 / 获取该应用创建的会议的数据**」（E1/E2/E6/E10/E11/E12）；E3/E4/E5/E7/E8/E9 是本企业资源面，不要求会议归属。
- **E1 预定的硬性要求**：会议时长 **≤24 小时**且**不支持周期性会议**；支持为同一会议预定**多个** Rooms 会议室。
- **E6 的定位键二选一**：`meeting_room_id` 与 `rooms_id` 二者填其一；时间跨度 **≤90 天**；`limit` 默认与最大均为 **20**，且**必须与首次取 `cursor` 时的 `limit` 一致**。
- **E3/E7/E8 的分页形态相同**：`limit` **从 1 开始、最大 50、默认 20**，同样「必须与首次一致」；`meeting_room_name` / `controller_name` 支持模糊匹配。
- **呼叫链**：E10 `call`（`meeting_room_id` 与 `mra_address` 二选一）→ 回 `invite_id` → E11 `cancel_call`（需 `invite_id`）→ E12 `get_response_status` 查应答态。
- `RoomsMraAddress`：`protocol`（**1 SIP / 2 H.323**）+ `dial_string`（H.323 填 IP 或 E.164 号码；SIP 填 IP 或 URI）。
- **敏感数据面**：`RoomsBasicInfo.Password`（**管理员密码，base64 编码**）、`RoomsPmiInfo.PmiPwd`（入会密码，base64）、`active_code`（激活码）、`ip` / `mac` 均为凭据或可定位设备网络位置的数据 ⇒ **一律只写参数名，不得进日志、遥测与异常消息**；设备与会议室位置（城市/建筑/楼层）属组织资产信息，聚合结果不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E12 | ✅ | ❌ | ❌ |

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
| 注册组名 | `"Meeting"`（十一族共用；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMeetingRoomsService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingRoomsService`） |
| 不可注入 | `IWechatWorkMeetingRoomsService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `BookMeetingRoomAsync` — 预定 Rooms 会议室

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/book")]
Task<BookMeetingRoomResponse> BookMeetingRoomAsync(
    [Body] BookMeetingRoomRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/rooms/book` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/rooms/book?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `BookMeetingRoomRequest` |

**请求体**：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `meetingid` | `Meetingid` | `string?` | **是** | 须为本应用创建的会议 |
| `meeting_room_id_list` | `MeetingRoomIdList` | `List<string>?` | **是** | **可一次预定多个** Rooms 会议室 |
| `subject_visible` | `SubjectVisible` | `bool?` | 否 | 默认 `true`：**仅在会议开始前一小时内**才在 Room 上显示主题；预定时间晚于当前 1 小时以上的会议不受该参数影响（默认不显示） |

**返回** `BookMeetingRoomResponse.MeetingRoomList`（`List<RoomsMeetingRoom>?`）：`meeting_room_id` / `meeting_room_name` / `meeting_room_location` / `account_type`（**0 普通 / 1 专款 / 2 试用**）/ `active_code` / `participant_number` / `meeting_room_status`（**0 未激活 / 1 未绑定 / 2 空闲 / 3 使用中 / 4 离线**）/ `scheduled_status`（0 未开放预定 / 1 开放预定）/ `is_allow_call`。

**调用示例**（E9 看资源 → E3 列会议室 → E1 预定 → E6 看占用 → E10~E12 呼叫与应答 → E2 释放）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMeetingApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Meeting;      // 请求 / 响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册会议模块（十一族同组；本族仅自建开放）
builder.Services.AddWechatWorkServices(b => b.AddMeetingApi());

// ③ 注入唯一的自建子接口
var rooms = provider.GetRequiredService<IWechatWorkInternalMeetingRoomsService>();

// ④ E9 资源余量：**无请求体的 POST**（MT1 断言参数只余 CancellationToken）
var inventory = await rooms.GetMeetingRoomInventoryAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(inventory, "/cgi-bin/meeting/rooms/get_inventory");
logger.LogInformation("普通 {Normal}/{NormalUsed}、专款 {Special}/{SpecialUsed}、过期 {NormalExpired}/{SpecialExpired}",
    inventory.NormalCount, inventory.NormalUsedCount,
    inventory.SpecialCount, inventory.SpecialUsedCount,
    inventory.NormalExpiredCount, inventory.SpecialExpiredCount);

// ⑤ E3 会议室列表：limit 从 1 开始、最大 50、默认 20，且翻页期间不可变
var roomList = await rooms.ListMeetingRoomsAsync(new ListMeetingRoomsRequest
{
    MeetingRoomName = "3F",                   // 支持模糊匹配
    Limit = 20,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(roomList, "/cgi-bin/meeting/rooms/list");
string? targetRoomId = (roomList.MeetingRoomList ?? new List<RoomsMeetingRoom>())
    .FirstOrDefault(r => r.ScheduledStatus == 1 && r.MeetingRoomStatus == 2)?.MeetingRoomId;
// 注意：RoomsMeetingRoom.ActiveCode 是激活码，不得打印

// ⑥ E1 预定：会议时长 ≤24 小时、不支持周期性会议；可一次预定多个
if (targetRoomId is not null)
{
    var booked = await rooms.BookMeetingRoomAsync(new BookMeetingRoomRequest
    {
        Meetingid = "demo-meeting-id",         // 假值；须为本应用创建的预约会议
        MeetingRoomIdList = new List<string> { targetRoomId },
        SubjectVisible = true,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(booked, "/cgi-bin/meeting/rooms/book");
    logger.LogInformation("预定回显 {Count} 个 Rooms 会议室", booked.MeetingRoomList?.Count ?? 0);

    // ⑦ E6 该会议室的会议占用：meeting_room_id 与 rooms_id 二选一、跨度 ≤90 天
    long beginAt = DateTimeOffset.Now.ToUnixTimeSeconds();
    var occupied = await rooms.ListMeetingRoomMeetingsAsync(new ListMeetingRoomMeetingsRequest
    {
        MeetingRoomId = targetRoomId,
        StartTime = beginAt,
        EndTime = beginAt + 30 * 24 * 3600,
        Limit = 20,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(occupied, "/cgi-bin/meeting/rooms/list_meetings");
    foreach (RoomsMeetingInfo item in occupied.MeetingInfoList ?? new List<RoomsMeetingInfo>())
    {
        // status 是字符串枚举（MEETING_STATE_*），不是数字
        logger.LogInformation("占用会议类型 {MeetingType}、状态字符串长度 {Len}",
            item.MeetingType, item.Status?.Length ?? 0);
    }

    // ⑧ E10 呼叫入会：meeting_room_id 与 mra_address 二选一 → 回 invite_id
    var call = await rooms.CallMeetingRoomAsync(new CallMeetingRoomRequest
    {
        Meetingid = "demo-meeting-id",
        MeetingRoomId = targetRoomId,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(call, "/cgi-bin/meeting/rooms/call");

    // ⑨ E12 应答状态：0 无应答 / 1 未呼叫 / 2 入会中 / 3 被拒绝 / 4 呼叫中 / 5 取消呼叫（仅 Rooms）/ 6 已离会
    var resp = await rooms.GetMeetingRoomResponseStatusAsync(new GetMeetingRoomResponseStatusRequest
    {
        Meetingid = "demo-meeting-id",
        MeetingRoomId = targetRoomId,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/meeting/rooms/get_response_status");
    logger.LogInformation("应答状态码 {Status}（应答时间为字符串形态）", resp.Status);

    // ⑩ E11 取消呼叫：需要 E10 返回的 invite_id
    var cancelCall = await rooms.CancelCallMeetingRoomAsync(new CancelCallMeetingRoomRequest
    {
        Meetingid = "demo-meeting-id",
        InviteId = call.InviteId!,
        MeetingRoomId = targetRoomId,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(cancelCall, "/cgi-bin/meeting/rooms/cancel_call");

    // ⑪ E2 释放：支持一次释放多个
    var released = await rooms.ReleaseMeetingRoomAsync(new ReleaseMeetingRoomRequest
    {
        Meetingid = "demo-meeting-id",
        MeetingRoomIdList = new List<string> { targetRoomId },
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(released, "/cgi-bin/meeting/rooms/release");
}

// ⑫ E4 详情 / E5 配置 / E7 设备 / E8 控制器（企业资产面，不要求会议归属）
var info = await rooms.GetMeetingRoomInfoAsync(new GetMeetingRoomInfoRequest { MeetingRoomId = targetRoomId }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/meeting/rooms/get_info");
// info.BasicInfo.Password 与 info.PmiInfo.PmiPwd 均为 base64 凭据：只判存在，不打印
logger.LogInformation("告警状态 {MonitorStatus}、是否允许被呼叫 {AllowCall}、是否有管理员密码 {HasPwd}",
    info.MonitorStatus, info.IsAllowCall, info.BasicInfo?.Password is not null);

var cfg = await rooms.GetMeetingRoomConfigAsync(new GetMeetingRoomConfigRequest { MeetingRoomId = targetRoomId }, cancellationToken);
WechatWorkException.ThrowIfFailed(cfg, "/cgi-bin/meeting/rooms/get_config");
logger.LogInformation("Rooms 水印档位 {WaterMark}、自动接听 {AutoResponse}、云录制分享档位 {ShareRecord}",
    cfg.MeetingSettings?.WaterMark, cfg.MeetingSettings?.AutoResponse, cfg.RecordSettings?.ShareRecord);

var devices = await rooms.ListRoomDevicesAsync(new ListRoomDevicesRequest { Limit = 50 }, cancellationToken);
WechatWorkException.ThrowIfFailed(devices, "/cgi-bin/meeting/rooms/list_devices");
var controllers = await rooms.ListRoomControllersAsync(new ListRoomControllersRequest { Limit = 50 }, cancellationToken);
WechatWorkException.ThrowIfFailed(controllers, "/cgi-bin/meeting/rooms/list_controllers");
// RoomsControllerInfo.Status 是字符串 "0" 离线 / "1" 在线（不是数字）
int online = (controllers.ControllerInfoList ?? new List<RoomsControllerInfo>()).Count(c => c.Status == "1");
logger.LogInformation("在线控制器 {Online} 台 / 设备 {DeviceCount} 台", online, devices.DeviceInfoList?.Count ?? 0);
```

### E2. `ReleaseMeetingRoomAsync` — 释放 Rooms 会议室

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/release")]
Task<WechatWorkResponse> ReleaseMeetingRoomAsync(
    [Body] ReleaseMeetingRoomRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）+ `meeting_room_id_list`（`List<string>?`，官方必填，**支持一次释放多个**）。**返回**：`WechatWorkResponse`（无业务负载）。

### E3. `ListMeetingRoomsAsync` — 获取 Rooms 会议室列表

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/list")]
Task<ListMeetingRoomsResponse> ListMeetingRoomsAsync(
    [Body] ListMeetingRoomsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_room_name`（模糊匹配）+ `cursor` + `limit`（**从 1 开始、最大 50、默认 20**，须与首次一致）。
**返回**：`has_more` + `next_cursor` + `meeting_room_list`（`List<RoomsMeetingRoom>?`，字段面见 E1）。

### E4. `GetMeetingRoomInfoAsync` — 获取 Rooms 会议室详情

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/get_info")]
Task<GetMeetingRoomInfoResponse> GetMeetingRoomInfoAsync(
    [Body] GetMeetingRoomInfoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_room_id`（官方必填）。**返回**四段信息 + 三个状态位：

| 段 | 类型 | 要点 |
|---|---|---|
| `basic_info` | `RoomsBasicInfo` | **`rooms_id_list`（是列表！）** / `meeting_room_name` / `city` / `building` / `floor` / `participant_number` / `device` / `desc`（**base64**）/ `password`（**管理员密码，base64**） |
| `account_info` | `RoomsAccountInfo` | `account_type`（0 普通 / 1 专款 / 2 试用）+ `valid_period`（**普通账号固定返回「-」**，即跟随企业账号有效期） |
| `hardware_info` | `RoomsHardwareInfo` | `factory` / `device_model` / `sn` / `ip` / `mac` / `rooms_version` / `firmware_version` / `health_status` / `system_type` / `meeting_room_status` / `active_time` / `cpu_info` / `cpu_usage` / `gpu_info` / `net_type` / `memory_info` / `monitor_frequency` / `camera_model` |
| `pmi_info` | `RoomsPmiInfo` | `pmi_code`（专属 ID）+ `pmi_pwd`（**入会密码，base64**） |
| 状态位 | — | `monitor_status`（0 未开启 / 1 已开启告警通知）、`is_allow_call`（`bool?`）、`scheduled_status`（0 未开放预定 / 1 开放预定） |

> **官方文档笔误留档**：`RoomsHardwareInfo.MonitorFrequency` 参数表标 `string`、官方示例为数字 ⇒ 本模型**以参数表为准**用 `string?` 承载。

### E5. `GetMeetingRoomConfigAsync` — 获取 Rooms 会议室配置

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/get_config")]
Task<GetMeetingRoomConfigResponse> GetMeetingRoomConfigAsync(
    [Body] GetMeetingRoomConfigRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_room_id`（官方必填）。**返回**：`meeting_settings`（`RoomsMeetingSettings?`）+ `record_settings`（`RoomsRecordSettings?`）。

| 型 | 字段 | 取值 |
|---|---|---|
| `RoomsMeetingSettings` | `water_mark` | **0 关闭 / 1 单排 / 2 多排**（注意与 `MeetingSettings.watermark_type` 的 0/1 两档不同） |
| | `auto_response` | **1 开启 / 2 关闭**（不是布尔） |
| | `caption` / `room_pmi` / `room_notification` | `bool?`（字幕 / 专属 ID / 屏幕消息通知） |
| `RoomsRecordSettings` | `share_record` | **0 关闭分享 / 1 全部成员 / 2 仅登录成员 / 3 仅同企业成员 / 4 仅参会成员** |
| | `download_record` | `bool?` 是否允许下载云录制 |

> `RoomsMeetingSettings` 与预约会议的 `MeetingSettings` **不同构**（MT5 断言）⇒ 不要共用类型或互相赋值。

### E6. `ListMeetingRoomMeetingsAsync` — 获取指定会议室的会议列表

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/list_meetings")]
Task<ListMeetingRoomMeetingsResponse> ListMeetingRoomMeetingsAsync(
    [Body] ListMeetingRoomMeetingsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_room_id` 与 `rooms_id` **二者填其一** + `start_time` / `end_time`（Unix 秒，**区间 ≤90 天**）+ `cursor` + `limit`（**默认与最大均为 20**，须与首次一致）。

**返回**：`has_more` + `next_cursor` + `meeting_info_list`（`List<RoomsMeetingInfo>?`）：`meetingid` / `meeting_code` / `subject`（**本族用 `subject`，预约会议详情用 `title`**）/ `status`（**字符串枚举** `MEETING_STATE_INVALID` / `_INIT` / `_CANCELLED` / `_STARTED` / `_ENDED`）/ `meeting_type`（**0 一次性 / 1 周期性 / 2 微信专属 / 4 rooms 投屏 / 5 个人会议号 / 6 网络研讨会**——注意此处 rooms 投屏是 **4**，与 `GetMeetingInfoResponse.meeting_type` 里的 **3** 不同）/ `start_time` / `end_time`。

### E7. `ListRoomDevicesAsync` — 获取可用设备列表

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/list_devices")]
Task<ListRoomDevicesResponse> ListRoomDevicesAsync(
    [Body] ListRoomDevicesRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_room_name`（模糊）+ `cursor` + `limit`（从 1 开始、最大 50、默认 20，须与首次一致）。
**返回**：`has_more` + `next_cursor` + `device_info_list`（`List<RoomsDeviceInfo>?`：`meeting_room_id` / `rooms_id` / `meeting_room_name` / `meeting_room_location` / `device_model` / `app_version` / `meeting_room_status` / `device_monitor_info`）。

`RoomsDeviceMonitorInfo`：`camera_status` / `microphone_status` / `speaker_status`（均 `bool?`，设备健康状态）。

> **官方文档笔误留档**：`RoomsDeviceInfo.AppVersion` 的文档页说明误写为「激活码」，官方示例为版本号形态 ⇒ 本模型按**版本号**语义承载。

### E8. `ListRoomControllersAsync` — 获取控制器列表

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/list_controllers")]
Task<ListRoomControllersResponse> ListRoomControllersAsync(
    [Body] ListRoomControllersRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`controller_name`（模糊）+ `cursor` + `limit`（从 1 开始、最大 50、默认 20，须与首次一致）。
**返回**：`has_more` + `next_cursor` + `controller_info_list`（`List<RoomsControllerInfo>?`：`rooms_id` / `meeting_room_name` / `meeting_room_location` / `controller_name` / `manufacture_name` / `controller_model` / `app_version` / **`status`（字符串 `"0"` 离线 / `"1"` 在线）** / `framework_version` / `ip_address` / `mac_address` / `cpu_type` / `cpu_usage` / `network_type` / `mem_usage`）。

### E9. `GetMeetingRoomInventoryAsync` — 获取 Rooms 资源（无请求体）

**签名**

```csharp
[Post("/cgi-bin/meeting/rooms/get_inventory")]
Task<GetMeetingRoomInventoryResponse> GetMeetingRoomInventoryAsync(
    CancellationToken cancellationToken = default);
```

**官方契约：本端点为无请求体的 POST**，仅以 Query 注入的 `access_token` 鉴权（MT1 断言方法参数 `ContainSingle` 且为 `CancellationToken`）⇒ **不要**给它加空请求 DTO，也不要顺手改成 GET。

**返回**：`normal_count` / `special_count` / `normal_used_count` / `special_used_count` / `normal_expired_count` / `special_expired_count`（均 `int?`）——即**普通设备 / 专款设备**的总数、已用数与过期数。

### E10 / E11 / E12. 呼叫三件套

```csharp
[Post("/cgi-bin/meeting/rooms/call")]
Task<CallMeetingRoomResponse> CallMeetingRoomAsync(
    [Body] CallMeetingRoomRequest request, CancellationToken cancellationToken = default);

[Post("/cgi-bin/meeting/rooms/cancel_call")]
Task<WechatWorkResponse> CancelCallMeetingRoomAsync(
    [Body] CancelCallMeetingRoomRequest request, CancellationToken cancellationToken = default);

[Post("/cgi-bin/meeting/rooms/get_response_status")]
Task<GetMeetingRoomResponseStatusResponse> GetMeetingRoomResponseStatusAsync(
    [Body] GetMeetingRoomResponseStatusRequest request, CancellationToken cancellationToken = default);
```

| 端点 | 请求 | 返回 |
|---|---|---|
| E10 `call` | `meetingid`（必填）+ **`meeting_room_id` 与 `mra_address` 二选一** | `invite_id`（呼叫 ID） |
| E11 `cancel_call` | `meetingid` + **`invite_id`**（均必填）+ `meeting_room_id` 与 `mra_address` 二选一 | `WechatWorkResponse` |
| E12 `get_response_status` | `meetingid`（必填）+ `meeting_room_id` 与 `mra_address` 二选一 | `status`（`int?`）+ `response_time`（**`string?`**） |

`RoomsMraAddress`：`protocol`（**1 SIP / 2 H.323**，官方必填）+ `dial_string`（官方必填；H.323 填 IP 或 E.164 号码，SIP 填 IP 或 URI）。

**`status` 七档**（E12）：**0 无应答（60 秒无回应）/ 1 未呼叫 / 2 入会中 / 3 被拒绝 / 4 呼叫中 / 5 取消呼叫（仅 Rooms 会议室有该状态）/ 6 已离会**。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **两个「会议室」域** | 本族 `/cgi-bin/meeting/rooms/`（硬件 Rooms）vs OA 域 `/cgi-bin/oa/meetingroom/`（实体会议室与预定）⇒ ID 体系、DTO、审批与预定语义全不同 | 把 `meetingroom_id`（int）与 `meeting_room_id`（string）互喂 |
| `meeting_room_id` 与 `rooms_id` **两套 ID** | `rooms_id` 是**设备/控制器 ID**（`RoomsBasicInfo.rooms_id_list` 是**列表**，一个会议室可挂多个 `rooms_id`）；E6 二者填其一，呼叫类只认 `meeting_room_id` 或 `mra_address` | 拿 `rooms_id` 去预定 |
| `meeting_type` 取值在两处不同 | 本族 `RoomsMeetingInfo.meeting_type`：**4 = rooms 投屏**；预约会议详情 `meeting_type`：**3 = Rooms 投屏** ⇒ 两套枚举不可互映射 | 用一套枚举解析另一套应答 |
| **本族 `status` 是字符串枚举** | `RoomsMeetingInfo.Status` = `MEETING_STATE_*` 字符串；预约会议详情 `Status` 是 `int?`（1~5）；控制器 `RoomsControllerInfo.Status` 又是字符串 `"0"`/`"1"` | 按数字比较 → 恒不等 |
| `subject` vs `title` | 本族会议项用 `subject`；预约会议创建与详情用 `title` | 复制字段名导致取不到值 |
| **E1 的时长与周期性硬约束** | 会议时长 **≤24 小时**、**不支持周期性会议** | 对周期会议或长会预定 Rooms |
| `subject_visible` 的实际语义 | 只在**会议开始前一小时内**生效；预定时间晚于当前 1 小时以上的会议**默认就不显示主题** | 以为设 `false` 就全程隐藏 |
| **E9 是无请求体的 POST** | MT1 断言只剩 `CancellationToken` | 加空请求 DTO 或改 GET → 直接打红 |
| `limit` 规则分两套 | E3/E7/E8：**从 1 开始、最大 50、默认 20**；E6：**默认与最大均 20** ⇒ 且所有分页都要求「后续 `limit` 与首次一致」 | 翻页中途改 `limit` |
| **配置型字段数值不连续** | `auto_response` 是 **1 开启 / 2 关闭**（不是 0/1、不是布尔）；`water_mark` 是 **0/1/2** 三档（`MeetingSettings.watermark_type` 只有 0/1） | 用布尔或 0/1 设自动接听 |
| `RoomsMeetingSettings` ≠ `MeetingSettings` | MT5 断言两者不同构 ⇒ 各写各的（本族无 `hosts`、`remind_scope` 等） | 共用类型或互相反序列化 |
| **base64 承载的三个字段** | `RoomsBasicInfo.desc` / `RoomsBasicInfo.password` / `RoomsPmiInfo.pmi_pwd` 均为 **base64 编码** ⇒ 取用前需解码，展示前需脱敏 | 直接显示 base64 或把密码写日志 |
| `valid_period` 对普通账号固定「-」 | 注释：普通账号有效期即企业账号有效期 ⇒ 不能用它判断可用水量，用 E9 | 把「-」当解析失败 |
| **两处官方文档笔误** | `RoomsDeviceInfo.AppVersion` 说明误写「激活码」（本仓按版本号）；`RoomsHardwareInfo.MonitorFrequency` 标 `string` 而示例为数字（本仓按参数表用 `string?`） | 按笔误面建模导致取值错位 |
| 呼叫要带 `invite_id` | 取消呼叫需要 E10 返回的 `invite_id`；`get_response_status` **不需要** `invite_id` | 复用错键导致取消失败 |
| 资产面与会议面权限不同 | E3/E4/E5/E7/E8/E9 只要求「可调用接口的应用」；E1/E2/E6/E10/E11/E12 还要求「该应用创建的会议」 | 用别的会议 ID 呼叫 |
| 全部官方即 POST | 本族 12 支全 POST（含 6 支查询语义端点）——全域仅 `layout/list_template` 是 GET | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 24 小时与周期性约束**、**不做资源余量预判**、**不做 base64 解码与脱敏**、**不做呼叫状态机**（`status` 轮询节奏由宿主掌握）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功回显被预定的会议室列表；E10 成功仅代表呼叫已发起，须查 E12 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

会议时长超限、周期性会议、会议室未开放预定 / 未空闲 / 离线、`invite_id` 不存在、非本应用创建的会议、`limit` 不一致等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98791~98809 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| 呼叫状态轮询建议 | E12 未给建议轮询间隔与「60 秒无应答」的计时起点 | 官方 98806 |
| `invite_id` 有效期 | 注释未记呼叫 ID 可保留多久、能否重复取消 | 官方 98804 / 98805 |
| 释放与预定是否对称 | 未记释放后能否原样重预定、以及会议已开始时的行为 | 官方 98792 |
| 资源（inventory）与 License 的关系 | 「普通/专款设备数」的采购口径、续期与过期后的行为未记录 | 官方 Rooms 资源说明 + License 域 |
| `meeting_room_status` 的可用性判定 | 0 未激活 / 1 未绑定 / 2 空闲 / 3 使用中 / 4 离线，但「哪些状态可被预定/呼叫」未逐端点说明 | 官方 98791 / 98804 |
| MRA 呼叫与 MRA 族的分工 | 本族用 `mra_address`（SIP/H.323 信令），MRA 族用 `tmp_openid` 操作会中设备 ⇒ 两者的转换链未记录 | [MeetingMra.md](./MeetingMra.md) |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（**第 11 节专门写「Rooms 在本域、MeetingRoom 是另一棵树」**；第 9 节批次上限表 #66；第 10 节 Rooms 字段形态）
- 同域近邻：[MeetingMra.md](./MeetingMra.md)（会中 MRA 设备操作）· [MeetingLayout.md](./MeetingLayout.md)（高级布局仅 H.323/SIP 会议室终端可用）· [MeetingRecording.md](./MeetingRecording.md)（`sharing_auth_type` 4/5 对 Rooms 不支持）· [Meeting.md](./Meeting.md)（被预定的会议本体）
- 跨域对照：[Booking.md](../MeetingRoom/Booking.md) 与 [Room.md](../MeetingRoom/Room.md)（**OA 会议室**：整数 `meetingroom_id`、30 分钟取整、审批流）
- 官方页：[预定 Rooms 会议室（98791）](https://developer.work.weixin.qq.com/document/path/98791) · [释放（98792）](https://developer.work.weixin.qq.com/document/path/98792) · [列表（98795）](https://developer.work.weixin.qq.com/document/path/98795) · [详情（98793）](https://developer.work.weixin.qq.com/document/path/98793) · [配置（98802）](https://developer.work.weixin.qq.com/document/path/98802) · [会议室会议列表（98796）](https://developer.work.weixin.qq.com/document/path/98796) · [设备（98798）](https://developer.work.weixin.qq.com/document/path/98798) · [控制器（98799）](https://developer.work.weixin.qq.com/document/path/98799) · [资源（98809）](https://developer.work.weixin.qq.com/document/path/98809) · [呼叫（98804）](https://developer.work.weixin.qq.com/document/path/98804) · [取消呼叫（98805）](https://developer.work.weixin.qq.com/document/path/98805) · [应答状态（98806）](https://developer.work.weixin.qq.com/document/path/98806)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
