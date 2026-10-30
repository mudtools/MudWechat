# 会议室预定管理 · OA 会议室排期与预定（仅企业自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `MeetingRoom` 域 · 会议室预定管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingRoomBookingService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingRoomBookingService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)（**唯一**子接口，承载全部 6 支） |
| DTO 源文件 | [DataModels/MeetingRoom/Booking/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/MeetingRoom/Booking/)（与会议室管理族同属 `SerializerClassName = MeetingRoom` → `MeetingRoomJsonContext`，全域 22 型；`Booking/` 子目录仅作组织、不入命名空间） |
| 契约守卫 | [WechatMeetingRoomContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingRoomContractGuards.cs)（`MeetingRoomBookingEndpoints_ShouldMatchOfficialRoutes` 6 支路由 + `cancel_book`/`bookinfo/get` 定位键必须为 `booking_id` / `MeetingRoomSingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent` 继承链只有自建子接口 / `MeetingRoomTokenBinding_ShouldBeAccessTokenInjectedViaQuery` / `MeetingRoomDataModels_ShouldBeRegisteredInJsonContext` 全域 22 型 / `MeetingRoomOfficialFieldNames_ShouldBePreservedVerbatim` / `MeetingRoomNumericalBooleanFields_ShouldBeCarriedAsNumbers` / `MeetingRoomOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable`）＋ G5 ＋ TO1~TO3 ＋ `WechatNonContractRouteZeroDeclarationContractGuards`（RD1） |
| 官方文档 | 会议室预定管理 [93620](https://developer.work.weixin.qq.com/document/path/93620)（**6 支端点同页的六个小节**；「会议室」分类树 93617；访问频率附录 90312）——第三方应用与服务商代开发两棵文档树**均无对应页面** |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（E1 查排期 → E2 预定 → E6 详情 → E5 取消））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MR1~MR8 七组守卫断言 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/README.md)（含 93619/93620 参数表复核结论与追踪号） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**实体会议室**的排期查询与预定管理（官方 OA 会议室，不是视频会议硬件会议室），6 支端点全部官方即 POST：

| 层 | 端点 | 用途 |
|---|---|---|
| 读 | E1 `oa/meetingroom/get_booking_info` | 按会议室 ID 或按「城市 / 楼宇 / 楼层」+ 时间窗查排期与占用状态 |
| 写（三种预定制） | E2 `book` · E3 `book_by_schedule` · E4 `book_by_meeting` | 按时间区间预定（会议室侧生成日程）/ 挂到已有**日程** / 挂到已有**会议** |
| 收尾 | E6 `bookinfo/get` · E5 `cancel_book` | 按 `booking_id` 查预定详情、取消预定 |

形态：**零端点父接口 + 唯一自建子接口承载**（MR3 断言继承链上恰等于 `{ IWechatWorkInternalMeetingRoomBookingService }`）——官方「会议室」文档树只在企业自建应用根下存在，第三方与代开发均为「暂不支持」⇒ **不得**为对齐邻域补 `*ThirdParty*` / `*Provider*` 子接口。

**与两条同名近邻分属不同树**（详见 §5）：会议室资源本身的增删改查在同域另一族 [Room.md](./Room.md)；视频会议硬件会议室在 `Meeting` 域的 MeetingRooms 子接口（`/cgi-bin/meeting/rooms/*`）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`，MR4 锁定） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中**；代开发与第三方官方标注「暂不支持」（93620「权限说明」原文）⇒ 本族只有唯一的自建子接口。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- **E2/E3/E4 三支预定端点均「仅可预定无需审批的会议室」**：需审批的会议室走官方审批流转（申请中 → 审批中 → 已预定），API 侧不覆盖。
- 时间量纲：全部时间戳为 **Unix 秒**；**最小预定时长 30 分钟**，且预定/结束时间**自动按 30 分钟取整**（官方例：传 15:15 与 15:45 → 取整为 15:00 与 16:00）。
- **E1 不支持跨天查询**；`start_time` 默认当前时间、`end_time` 默认定明日 0 时；按位置过滤时**上一级必须同时填写**（用 `building` 必须同时填 `city`）。
- **所有权边界**：E3 的 `schedule_id` **仅可使用同应用创建的日程**；E4 的 `meetingid` **仅可使用同应用创建的会议**。
- 人员字段 `booker` / `attendees` 是成员账号（userid），属个人数据：示例用假值，**预定详情与参与人名单不要整体落日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E6 | ✅ | ❌ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册会议室模块（预定族与管理族同组同模块）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMeetingRoomApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.MeetingRoom` |
| 注册入口 | `WechatWorkServiceBuilder.AddMeetingRoomApi()`（→ 源生成 `AddMeetingRoomWebApiHttpClient()`） |
| 注册组名 | `"MeetingRoom"`（与管理族同组，MR3 锁定） |
| 可注入接口 | `IWechatWorkInternalMeetingRoomBookingService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingRoomBookingService`） |
| 不可注入 | `IWechatWorkMeetingRoomBookingService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`，`RegistryGroupName` 空） |

## 4. 端点契约明细

> **必填性由类型系统表达**（MR8）：官方参数表「必须=是」的字段声明为**非可空**（`int` / `long` / `string`），「必须=否」的保持**可空**以便省略 ⇒ 写请求体时非可空字段必须赋值，可选字段留空即不发送。SDK 不做本地必填校验，越界由官方错误码表达。

### E1. `GetMeetingRoomBookingInfoAsync` — 查询会议室预定信息

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/get_booking_info")]
Task<GetMeetingRoomBookingInfoResponse> GetMeetingRoomBookingInfoAsync(
    [Body] GetMeetingRoomBookingInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/meetingroom/get_booking_info` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/oa/meetingroom/get_booking_info?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `GetMeetingRoomBookingInfoRequest` |

**请求体**（**全部字段官方均为选填**，MR8 的可空侧；两种定位方式二选一）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `meetingroom_id` | `MeetingRoomId` | `int?` | 按会议室 ID 定位 |
| `start_time` | `StartTime` | `long?` | 起始时间戳（秒），默认当前时间 |
| `end_time` | `EndTime` | `long?` | 结束时间戳（秒），默认明日 0 时 |
| `city` / `building` / `floor` | `City` / `Building` / `Floor` | `string?` | 按位置定位；**上级必须同时填写**（填楼宇必须填城市） |

**返回** `GetMeetingRoomBookingInfoResponse` → `BookingList`（JSON `booking_list`，元素 `MeetingRoomBooking`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `meetingroom_id` | `MeetingRoomId` | `int?` | 会议室 ID |
| `schedule` | `ScheduleList` | `List<MeetingRoomSchedule>?` | **容器名官方恒为单数 `schedule`，形态是数组**（MR6） |

`MeetingRoomSchedule`（排期载体，两种形态共用）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `start_time` / `end_time` | `StartTime` / `EndTime` | `long?` | 排期起止（秒） |
| `schedule_id` | `ScheduleId` | `string?` | 关联日程 ID |
| `meeting_id` | `MeetingId` | `string?` | 会议 ID——**现行 93620 两处 schedule 参数表均未列**，DTO 注释明说保留为可空超集；**取消预定请用 `BookingId`** |
| `booker` | `BookerUserId` | `string?` | 预定者 userid |
| `status` | `Status` | `int?` | 本端点口径：**0 已预定 / 2 申请中 / 3 审批中**（**不回传 1 已取消**） |
| `booking_id` | `BookingId` | `string?` | **仅 E6 的 `schedule` 回传** |
| `master_booking_id` | `MasterBookingId` | `string?` | **仅 E6 回传**：该预定属于某个周期性预定时的主预定 ID |

**调用示例**（E1 查排期 → E2 预定 → E6 查详情 → E5 取消，一条完整链路；E3/E4 见本节末注意事项）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMeetingRoomApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.MeetingRoom;  // 请求/响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册会议室模块（预定族 + 管理族同模块）
builder.Services.AddWechatWorkServices(b => b.AddMeetingRoomApi());

// ③ 注入唯一的自建子接口（本域无第三方 / 代开发子接口）
var booking = provider.GetRequiredService<IWechatWorkInternalMeetingRoomBookingService>();

// ④ E1 查询排期：官方不支持跨天，end_time 用当天窗口内取值
var now = DateTimeOffset.Now.ToUnixTimeSeconds();
var info = await booking.GetMeetingRoomBookingInfoAsync(new GetMeetingRoomBookingInfoRequest
{
    City = "深圳",
    Building = "总部",
    Floor = "3",
    StartTime = now,
    EndTime = now + 4 * 3600,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/oa/meetingroom/get_booking_info");

foreach (var room in info.BookingList ?? new List<MeetingRoomBooking>())
{
    // 排期容器官方名为单数 schedule、形态是数组（MR6 锁定，勿「顺手改成 schedules」）
    foreach (var schedule in room.ScheduleList ?? new List<MeetingRoomSchedule>())
    {
        // 本端点 status 只有 0 / 2 / 3，不会出现 1（已取消）
        logger.LogInformation("会议室 {MeetingRoomId} 于 {StartTime} 起占用，status={Status}",
            room.MeetingRoomId, schedule.StartTime, schedule.Status);
    }
}

// ⑤ E2 直接预定：最小 30 分钟，起止时间官方自动按 30 分钟取整
var booked = await booking.BookMeetingRoomAsync(new MeetingRoomBookRequest
{
    MeetingRoomId = 1,                       // 非可空：官方必填
    StartTime = now,                         // 非可空：官方必填
    EndTime = now + 3600,                    // 非可空：官方必填
    BookerUserId = "emp-0001",               // 非可空：官方必填（假值，勿用真人账号）
    Subject = "周会",                         // 可空：官方选填
    AttendeeUserIdList = new List<string> { "emp-0002" },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(booked, "/cgi-bin/oa/meetingroom/book");

// ⑥ E6 查预定详情：定位键是 meetingroom_id + booking_id（不是会议 ID）
var detail = await booking.GetMeetingRoomBookInfoAsync(new MeetingRoomBookInfoGetRequest
{
    MeetingRoomId = 1,
    BookingId = booked.BookingId!,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/meetingroom/bookinfo/get");

// 本端点 status 比查询侧多一档：0 已预定 / 1 已取消 / 2 申请中 / 3 审批中
logger.LogInformation("预定详情 status={Status}，主预定 ID 是否复用={HasMaster}",
    detail.Schedule?.Status, detail.Schedule?.MasterBookingId is not null);

// ⑦ E5 取消：keep_schedule 官方用数字 0/1 承载（0 同步删除 / 1 保留，仅对非重复日程有效）
var cancelled = await booking.CancelMeetingRoomBookAsync(new MeetingRoomCancelBookRequest
{
    BookingId = booked.BookingId!,
    KeepSchedule = 1,
    // CancelDate：重复日程才填（当天 0 点时间戳）；不填表示取消全部重复预定
}, cancellationToken);
WechatWorkException.ThrowIfFailed(cancelled, "/cgi-bin/oa/meetingroom/cancel_book");
```

### E2. `BookMeetingRoomAsync` — 预定会议室

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/book")]
Task<MeetingRoomBookResponse> BookMeetingRoomAsync(
    [Body] MeetingRoomBookRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `MeetingRoomBookRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `meetingroom_id` | `MeetingRoomId` | `int` | **是** | 会议室 ID（来自 [Room.md](./Room.md) 的 `add` 或 `list`） |
| `start_time` | `StartTime` | `long` | **是** | 开始时间戳（秒），官方按 30 分钟取整 |
| `end_time` | `EndTime` | `long` | **是** | 结束时间戳（秒），最小跨度 30 分钟 |
| `booker` | `BookerUserId` | `string` | **是** | 预定者成员账号（userid） |
| `subject` | `Subject` | `string?` | 否 | 会议主题 |
| `attendees` | `AttendeeUserIdList` | `List<string>?` | 否 | 参与人 userid 列表 |

**返回** `MeetingRoomBookResponse`：`booking_id`（预定 ID）+ **`schedule_id`（仅本端点回传**——本端点由会议室侧生成日程；按日程/按会议两支不回传该字段）。

**「当前时间已晚于预定时间」的官方三条处理规则**：已过 `end_time` → **不允许预定**；处于 `start_time` 的 15 分钟内 → **允许预定**；超过 `start_time` 15 分钟 → **自动把开始时间顺延一个 30 分钟窗口**。

### E3. `BookMeetingRoomByScheduleAsync` — 按日程预定会议室

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/book_by_schedule")]
Task<MeetingRoomBookByScheduleResponse> BookMeetingRoomByScheduleAsync(
    [Body] MeetingRoomBookByScheduleRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**（三字段**均官方必填**，故全为非可空）：`meetingroom_id`（`int`）· `schedule_id`（`string`，**仅同应用创建的日程**）· `booker`（`string`）。

**返回** `MeetingRoomBookByScheduleResponse`：`booking_id` + `conflict_date`（`List<long>?`，**字段名官方即单数**，MR6）。`conflict_date` 是**当天 0 点时间戳数组**：重复日程中部分日期与既有预定冲突时返回。

> **副作用要记住**：经本端点预定后，该日程**不能再通过「更新日程」接口编辑**，只能调用新增 / 删除日程参与者接口；若要改时间，须**先取消会议室预定 → 更新日程 → 重新预定**。

### E4. `BookMeetingRoomByMeetingAsync` — 按会议预定会议室

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/book_by_meeting")]
Task<MeetingRoomBookByMeetingResponse> BookMeetingRoomByMeetingAsync(
    [Body] MeetingRoomBookByMeetingRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**（三字段均必填）：`meetingroom_id`（`int`）· **`meetingid`（`string`，官方拼写无下划线，MR6 锁定）** · `booker`（`string`）。`meetingid` **仅可使用同应用创建的会议**。

**返回** 与 E3 同形：`booking_id` + `conflict_date`（当天 0 点时间戳数组）。

> 请求字段 `meetingid`（无下划线）与排期载体里的 `meeting_id`（有下划线）**同域不同名**，两处都照官方原文保留，不要互相「对齐」。

### E5. `CancelMeetingRoomBookAsync` — 取消会议室预定

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/cancel_book")]
Task<WechatWorkResponse> CancelMeetingRoomBookAsync(
    [Body] MeetingRoomCancelBookRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `MeetingRoomCancelBookRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `booking_id` | `BookingId` | `string` | **是** | **会议室的预定 id，不是会议 ID**（MR2 直接断言该定位键） |
| `keep_schedule` | `KeepSchedule` | `int?` | 否 | **0 同步删除 / 1 保留**，且**仅对非重复日程有效**；官方用数字承载布尔语义（MR7：改成 `bool?` 即破坏线格式） |
| `cancel_date` | `CancelDate` | `long?` | 否 | 针对重复日程：**不填 = 取消全部重复预定**；填（当天 0 点时间戳）= 仅取消该日期当天 |

**返回**：`WechatWorkResponse`——官方成功响应仅 `errcode` / `errmsg`，**不另建响应 DTO**（MR6 断言返回类型恰为该形态）。

### E6. `GetMeetingRoomBookInfoAsync` — 获取会议室预定详情

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/bookinfo/get")]
Task<MeetingRoomBookInfoGetResponse> GetMeetingRoomBookInfoAsync(
    [Body] MeetingRoomBookInfoGetRequest request,
    CancellationToken cancellationToken = default);
```

- **请求体**：`meetingroom_id`（`int`，必填）+ `booking_id`（`string`，必填）——两者均必填。
- **返回** `MeetingRoomBookInfoGetResponse`：`meetingroom_id`（`int?`）+ `schedule`（**`MeetingRoomSchedule` 单对象形态**，MR6）。比 E1 的排期多回传 `booking_id` 与 `master_booking_id`。
- **status 口径比 E1 多一档**：**0 已预定 / 1 已取消 / 2 申请中 / 3 审批中**；附录另述审批流转（无需审批：申请即已预定；需审批：申请中 → 提交审批后审批中 → 审批完成后已预定，再取消为已取消）。
- **路由形态独有**：`bookinfo/get` 是**斜杠分层**，与本域其余端点的平级路由（`book` / `cancel_book` / `get_booking_info`）不同，照抄原文。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| `meetingid` vs `meeting_id` | E4 请求字段官方即 `meetingid`（无下划线）；排期载体里的会议 ID 是 `meeting_id` ⇒ 两处分别锁定（MR6） | 驼峰化或补下划线 → 官方收不到参数 |
| `schedule` 单数名 · 两形态 | E1 里是**数组**（`MeetingRoomBooking.ScheduleList`）、E6 里是**单对象**（`MeetingRoomBookInfoGetResponse.Schedule`），JSON 名同为 `schedule` | 改成 `schedules` / 按一侧形态硬套另一侧 |
| `keep_schedule` 是数字不是布尔 | 官方线格式 `0`/`1` ⇒ 以 `int?` 原样承载（MR7）；本仓**无数值-布尔转换器** | 声明 `bool?`：读侧抛 `JsonException`、写侧发出与官方不符的 `true`/`false` |
| 必填性写在类型上 | MR8 逐字段断言：必填侧**非可空**、选填侧**可空**（含「`list` 的四个过滤项、`get_booking_info` 的全部参数、`subject`/`attendees`/`keep_schedule`/`cancel_date` 必须保持可空」） | 把选填改成非可空 → 每个请求被迫带该参数；把必填改成可空 → 请求体可省略官方必收的参数 |
| `booking_id` ≠ 会议 ID | E5/E6 的定位键是**会议室的预定 id**（MR2 直接断言属性名与 JSON 名） | 拿 `meeting_id` 去取消 → 找不到预定 |
| 三种预定制的响应面 | E2 回 `booking_id` + `schedule_id`；E3/E4 回 `booking_id` + `conflict_date`，**不回 `schedule_id`** | 用同一响应 DTO 承接（仓内是三个独立响应类型，不共用） |
| E1 与 E6 的 `status` 枚举不等宽 | E1：0 / 2 / 3（**无 1**）；E6：0 / 1 / 2 / 3 | 按 E6 口径解析 E1 结果，或以为 E1 能查出已取消的预定 |
| 时间自动取整 | 30 分钟最小粒度 + 起止自动取整（15:15 → 15:00） | 以「传入值即生效值」做后续比对；短于 30 分钟的预定被拒 |
| 位置过滤须带上级 | 填 `building` 必须同时填 `city`；填 `floor` 同理（E1 与 [Room.md](./Room.md) 的 `list` 同一口径） | 只传楼层 → 官方过滤失效或报错 |
| 按日程预定后日程锁死 | E3 之后该日程不可再「更新日程」编辑，只能增删参与者 | 业务侧改时间失败，须「取消预定 → 改日程 → 重新预定」 |
| ID 体系两套 | 本域的 `schedule_id` / `meeting_id` 由会议室预定接口返回并**以字符串透传**，**不经日程接口换取**（接口注释自述，与「日程」域 `IWechatWorkScheduleService` 是两套 ID） | 以为日程域的 ID 可直接用于 E3 |
| 与视频会议会议室不同树 | 本域是 **OA 会议室**（`/cgi-bin/oa/meetingroom/`），视频会议硬件会议室是 `/cgi-bin/meeting/rooms/*`（`Meeting` 域 MeetingRooms 子接口）⇒ 两条路由互不承载 | 把两个「会议室」当同一实体做 join |
| 全部查询官方即 POST | E1/E6 两支查询都是 POST（带请求体），本域**零 GET** | 按 REST 直觉改 GET → 打坏 MR2 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不做时间取整与 30 分钟校验**、**不校验跨天与位置上级完整性**、**不做预定状态机**（`status` 流转由宿主判断）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E5/E6 无业务负载，判成功即结束 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

会议室业务码（`schedule_id` / `meetingid` 非本应用创建、目标会议室需审批、预定冲突、时间越界、`booking_id` 不存在等）的**具体值接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口 XML 注释**未逐条摘录** ⇒ **未逐项核验**；需要按码分支时先核验官方页再补注释 | 官方 93620 + 全局错误码页 |
| `get_booking_info_by_meeting_id` | 曾由第三方 SDK 转述的这条路由**已不在 93620 页与「会议室」树任何节点内** ⇒ 本域**不声明**，由 RD1 零声明守卫持有（追踪号 `P2-QY-MEETINGROOM-BYMEETINGID-07`）；**不得**按旧 SDK 补回 | 官方「会议室」树 93617 目录 |
| 排期里的 `meeting_id` | 现行 93620 两处 `schedule` 参数表**均未列**该字段，DTO 以可空超集保留 ⇒ 其出现条件与语义**未核验** | 官方 93620 响应示例 |
| 审批流对 API 的可见性 | 注释说本族「仅可预定无需审批的会议室」，但「申请中 / 审批中」条目如何经 API 推进（是否有对应端点）未记录 ⇒ 本域不建模 | 官方 OA 审批域 |
| `conflict_date` 的触发细节 | 注释只说「重复日程部分日期与既有预定冲突时返回，值为当天 0 点时间戳」⇒ 与 `booking_id`/`master_booking_id` 的对应关系未核验 | 官方 93620 |
| 端点频率限制 | 本族 6 个页面小节**均无独立数值阈值**，走官方附录默认口径（每企业单 cgi/api 1 万次/分、15 万次/小时；每 IP 2 万次/分、60 万次/小时，90312） | 官方 90312 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/README.md)（10 支端点速览、四接口令牌表、逐端点契约明细与 DTO 表）
- 同域另一族：[Room.md](./Room.md)（会议室资源 add / list / edit / del —— `meetingroom_id` 的来源与设备 ID 附录）
- 跨域：`schedule_id` 的产生面在「日程」域（`IWechatWorkScheduleService`，`MeetingRoom` 与日程是两套 ID 体系）；`meetingid` 的产生面在「视频会议」域 [Meeting.md](../Meeting/Meeting.md)
- 易混的近邻：`Meeting` 域 MeetingRooms 子接口是**视频会议硬件会议室**（`/cgi-bin/meeting/rooms/*`），与本域 OA 会议室不同树
- 官方页：[会议室预定管理（93620）](https://developer.work.weixin.qq.com/document/path/93620) · [会议室管理（93619）](https://developer.work.weixin.qq.com/document/path/93619) · [访问频率限制（90312）](https://developer.work.weixin.qq.com/document/path/90312)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
