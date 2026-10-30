# 会中控制 · realcontrol 九个动作与 poll 投票八支

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 会中控制管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingControlService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingControlService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingControlService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingControlService_Internal.cs)（**唯一**子接口，承载全部 17 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`SetMeetingRealtimeSettingsRequest`、`SetMeetingCoHostRequest`、`MuteMeetingUserRequest`、`SwitchMeetingUserVideoRequest`、`CloseMeetingScreenShareRequest`、`SetMeetingUserNicknamesRequest`、`ManageMeetingWaitingRoomUsersRequest`、`KickoutMeetingUsersRequest`、`DismissMeetingRequest`、`MeetingOperatedUser`、`MeetingOperatedNicknameUser`、`MeetingHosts` 与投票八支的 `*Poll*` 请求响应及 `MeetingPollQuestion` / `MeetingPollThemeQuestion` / `MeetingPollDetailQuestion` / `MeetingPollInfo` / `MeetingPollThemeInfo` / `MeetingPollDetailOption` / `MeetingPollOptionUser`；本族在 MT4 的逐族分解中计 28 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(17)` = `realcontrol/` 9 + `poll/` 8、全部官方即 POST / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 17)` / MT3 令牌绑定 / MT4 上下文登记 / MT5「会控被操作对象单复数两形态并存：`operated_user` 与 `operated_users`」）＋ G5 ＋ TO1~TO3 |
| 官方文档 | realcontrol 设置项 [98175](https://developer.work.weixin.qq.com/document/path/98175) · 联席主持人 98180 · 静音 98184 · 开关视频 98189 · 关闭共享 98185 · 改昵称 98188 · 等候室移入移出 98186 · 移出成员 98181 · 结束会议 98187；投票 98834（建主题）/ 98835（改主题）/ 98836（列表）/ 98837（主题详情）/ 98838（结果详情）/ 98839（删除）/ 98840（发起）/ 98841（结束）（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（会中设置 → 单人静音 → 等候室移入 → 移出 → 投票建主题/发起/结果/结束/删除 → 结束会议））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 / MT5 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 1/8/10 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**会议进行中**的管理动作，17 支端点分两组、两个前缀：

| 组 | # | 方法 | 路由 | 动作 |
|---|---|---|---|---|
| `realcontrol/` | E1 | `SetRealtimeSettingsAsync` | `realcontrol/set` | 会中设置项（全体静音、聊天、锁定会议、等候室…） |
| | E2 | `SetCoHostAsync` | `realcontrol/set_cohost` | 设置 / 撤销联席主持人 |
| | E3 | `MuteUserAsync` | `realcontrol/mute_user` | 单人静音 / 解除 |
| | E4 | `SwitchUserVideoAsync` | `realcontrol/switch_user_video` | 开关成员视频（**开启仅支持 MRA**） |
| | E5 | `CloseScreenShareAsync` | `realcontrol/close_screen_share` | 关闭指定成员屏幕共享 |
| | E6 | `SetUserNicknamesAsync` | `realcontrol/set_nicknames` | 改成员昵称（≤20 字符） |
| | E7 | `ManageWaitingRoomUsersAsync` | `realcontrol/manage_waiting_room_users` | 等候室移入 / 移出 |
| | E8 | `KickoutUsersAsync` | `realcontrol/kickout_users` | 移出成员（可选是否允许再入会） |
| | E9 | `DismissMeetingAsync` | `realcontrol/dismiss` | 结束会议（可强制、可回收会议号） |
| `poll/` | E10 | `CreatePollThemeAsync` | `poll/create_theme` | 创建投票主题 |
| | E11 | `UpdatePollThemeAsync` | `poll/update_theme` | 修改投票主题（**全覆盖修改**） |
| | E12 | `GetPollListAsync` | `poll/get_poll_list` | 取会议内投票主题列表 |
| | E13 | `GetPollThemeInfoAsync` | `poll/get_theme_info` | 取主题定义 |
| | E14 | `GetPollDetailAsync` | `poll/get_poll_detail` | 取某次投票结果 |
| | E15 | `DeletePollAsync` | `poll/delete` | 删主题 / 删实例（二选一） |
| | E16 | `StartPollAsync` | `poll/start` | 发起投票，回 `poll_id` |
| | E17 | `FinishPollAsync` | `poll/finish` | 结束投票 |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）。九支 `realcontrol/*` 除 E1 外全部返回 `WechatWorkResponse`。

**本族只在「会议中」有意义**：预约会议的改期 / 取消在 [Meeting.md](./Meeting.md)（且**仅预约状态**可用），一旦会议进入「会议中」，管理动作就落到本族（E1~E9）；投票八支的注释更是逐条写明「**仅进行中的会议可以调用**」。

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

- 自建应用须配置在「**可调用接口的应用**」列表中；**仅允许操作该应用创建的会议**。
- **投票八支额外两条闸门**：**仅进行中的会议可调用**，且**操作者（`operator_userid`）须是主持人或会议管理员**。
- **被操作者定位键**：`MeetingOperatedUser` = `tmp_openid`（**当场会议的成员临时 ID，官方必填**）+ `instance_id`（终端设备类型，官方必填）；改昵称用的是 `MeetingOperatedNicknameUser`（再带 `nickname`）。注释明确「**被操作成员的设备类型须与其一致，否则不生效**」⇒ `instance_id` 必须取自该成员实际入会设备（来源：高级族 E2 实时会中成员列表、PSTN `get_tmp_openid`、MRA `query_status`）。
- **MRA 设备的两面性**：`realcontrol` 多数端点注释「**目前暂不支持 MRA 设备作为被操作者**」（E2/E3/E5/E6 等）；但 E4 `video=true`（开启视频）**仅支持 MRA 设备**、E7 的 `allow_rejoin` **对 MRA 设备不生效** ⇒ 同一族内 MRA 支持度逐端点不同。
- **`allow_unmute_self` 与 `mute_all` 的联动**（E1 注释）：`mute_all` **必传**，且 `mute_all=true` 时 `allow_unmute_self` 的设置才生效。
- **E9 结束会议**：`force_dismiss` 默认 **1**（强制结束，有参会者也强停）、**0** 则有参会者时无法强停；`retrieve_code` 默认 **0**（不回收会议号、可重新入会）、**1** 回收后不可再入会 —— **周期性会议若还有子会议必须设 0**，否则后续子会议无法正常进行；**该字段对快速会议不生效（快速会议强制收回会议号）**。
- **昵称限制 20 个字符**（E6）；被操作成员列表官方必填（数组）。
- 投票容量：主题**最多 10 个问题**、主题标题 ≤**50 字符**、描述 ≤**100 字符**、每题**最多 10 个选项且每选项 ≤36 字符**；**创建时每题最少 2 个选项、修改时最少 1 个**（两处口径不同，见 §5）。
- 成员昵称、临时 ID、健康度与网络类型属个人与设备数据：示例用假值，**不要把 `tmp_openid`、昵称与 `net` 落日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E17 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingControlService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingControlService`） |
| 不可注入 | `IWechatWorkMeetingControlService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `SetRealtimeSettingsAsync` — 管理进行中会议的设置项

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/set")]
Task<WechatWorkResponse> SetRealtimeSettingsAsync(
    [Body] SetMeetingRealtimeSettingsRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/realcontrol/set` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/realcontrol/set?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `SetMeetingRealtimeSettingsRequest` |

**请求体** `SetMeetingRealtimeSettingsRequest`（`meetingid` 官方必填，其余为可空开关）：

| 字段 | 属性 | 类型 | 取值 |
|---|---|---|---|
| `mute_all` | `MuteAll` | `bool?` | 全体静音；**本字段必传**，`allow_unmute_self` 仅在它为 `true` 时生效 |
| `allow_unmute_self` | `AllowUnmuteSelf` | `bool?` | 是否允许成员自己取消静音 |
| `enable_enter_mute` | `EnableEnterMute` | `int?` | **0 关闭 / 1 开启 / 2 超 6 人自动开启**（数值，不是布尔） |
| `meeting_locked` | `MeetingLocked` | `bool?` | 是否锁定会议 |
| `hide_meeting_code_password` | `HideMeetingCodePassword` | `bool?` | 是否隐藏会议号和密码 |
| `allow_chat` | `AllowChat` | `int?` | **0 自由聊天 / 1 仅公开聊天 / 2 仅私聊主持人** |
| `allow_share_screen` | `AllowShareScreen` | `bool?` | 是否允许参会者发起屏幕共享 |
| `allow_external_user` | `AllowExternalUser` | `bool?` | **true 仅企业成员可入会 / false 不限制**（注意与 `MeetingSettings.allow_external_user` 的语义方向相反，见 §5） |
| `play_ivr_on_join` | `PlayIvrOnJoin` | `bool?` | 入会是否播放提示音 |
| `enable_waiting_room` | `EnableWaitingRoom` | `bool?` | 是否开启等候室 |

**调用示例**（E1 静音策略 → E7/E8 等候室与移出 → E10~E17 投票全链路 → E9 结束会议）

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
var control = provider.GetRequiredService<IWechatWorkInternalMeetingControlService>();

const string meetingid = "demo-meeting-id";       // 须为本应用创建、且处于「会议中」
const string operatorUserid = "demo_host_0001";   // 假值；投票八支要求主持人或会议管理员
const int hostInstance = 1;                       // 1 = PC（取值口径同会控成员设备类型）

// ④ E1 会中设置：mute_all 必传；allow_unmute_self 仅在 mute_all=true 时生效
var settings = await control.SetRealtimeSettingsAsync(new SetMeetingRealtimeSettingsRequest
{
    Meetingid = meetingid,
    MuteAll = true,
    AllowUnmuteSelf = false,
    EnableEnterMute = 2,                 // 2 = 超过 6 人自动开启
    AllowChat = 1,                       // 1 = 仅允许公开聊天
    EnableWaitingRoom = true,
    HideMeetingCodePassword = true,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(settings, "/cgi-bin/meeting/realcontrol/set");

// ⑤ E3 单人静音：被操作者用 tmp_openid + instance_id 定位（设备类型必须与实际一致，否则不生效）
//    tmp_openid 来自高级族的实时会中成员列表 / PSTN get_tmp_openid / MRA query_status
var mute = await control.MuteUserAsync(new MuteMeetingUserRequest
{
    Meetingid = meetingid,
    Option = false,                      // true 静音 / false 解除静音
    OperatedUser = new MeetingOperatedUser { TmpOpenid = "demo-tmp-openid", InstanceId = 1 },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(mute, "/cgi-bin/meeting/realcontrol/mute_user");

// ⑥ E7 等候室操作：operate_type 1 移入会议 / 2 移入等候室 / 3 移出等候室；allow_rejoin 仅 operate_type=3 可设
var waitingRoom = await control.ManageWaitingRoomUsersAsync(new ManageMeetingWaitingRoomUsersRequest
{
    Meetingid = meetingid,
    OperateType = 1,
    OperatedUsers = new List<MeetingOperatedUser>
    {
        new MeetingOperatedUser { TmpOpenid = "demo-tmp-openid", InstanceId = 1 },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(waitingRoom, "/cgi-bin/meeting/realcontrol/manage_waiting_room_users");

// ⑦ E8 移出成员：allow_rejoin 官方必填
var kicked = await control.KickoutUsersAsync(new KickoutMeetingUsersRequest
{
    Meetingid = meetingid,
    AllowRejoin = true,
    OperatedUsers = new List<MeetingOperatedUser>
    {
        new MeetingOperatedUser { TmpOpenid = "demo-tmp-openid", InstanceId = 1 },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(kicked, "/cgi-bin/meeting/realcontrol/kickout_users");

// ⑧ E10 创建投票主题：每题最多 10 选项、创建时最少 2 个；主题 ≤50 字符、描述 ≤100 字符
var theme = await control.CreatePollThemeAsync(new CreateMeetingPollThemeRequest
{
    OperatorUserid = operatorUserid,
    InstanceId = hostInstance,
    Meetingid = meetingid,               // 仅进行中的会议可调用
    PollTopic = "会前议题表决",
    PollDesc = "请选择你关注的议题",
    IsAnony = 0,                         // 0 实名（默认）/ 1 匿名
    PollQuestions = new List<MeetingPollQuestion>
    {
        new MeetingPollQuestion
        {
            QuestionDesc = "你最关注哪个议题",   // ≤50 字符
            QuestionType = 0,                    // 0 单选 / 1 多选
            PollOption = new List<string> { "议题A", "议题B" },  // 每选项 ≤36 字符
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(theme, "/cgi-bin/meeting/poll/create_theme");
string? pollThemeId = theme.PollThemeId;

if (pollThemeId is not null)
{
    // ⑨ E16 发起投票 → 回 poll_id；E14 取结果（poll_id）
    var started = await control.StartPollAsync(new StartMeetingPollRequest
    {
        OperatorUserid = operatorUserid,
        InstanceId = hostInstance,
        Meetingid = meetingid,
        PollThemeId = pollThemeId,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(started, "/cgi-bin/meeting/poll/start");

    if (started.PollId is not null)
    {
        var detail = await control.GetPollDetailAsync(new GetMeetingPollDetailRequest
        {
            OperatorUserid = operatorUserid,
            InstanceId = hostInstance,
            Meetingid = meetingid,
            PollId = started.PollId,
        }, cancellationToken);
        WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/meeting/poll/get_poll_detail");
        // 只取计数：投票人数与题目数，不落逐人投票选项（option_user 含成员身份）
        logger.LogInformation("投票 {VoteTotalNum} 人参与、{QuestionCount} 题",
            detail.VoteTotalNum, detail.PollQuestionData?.Count ?? 0);

        // ⑩ E17 结束投票：poll_theme_id 与 poll_id 均必填
        var finished = await control.FinishPollAsync(new FinishMeetingPollRequest
        {
            OperatorUserid = operatorUserid,
            InstanceId = hostInstance,
            Meetingid = meetingid,
            PollThemeId = pollThemeId,
            PollId = started.PollId,
        }, cancellationToken);
        WechatWorkException.ThrowIfFailed(finished, "/cgi-bin/meeting/poll/finish");
    }

    // ⑪ E15 删除：主题 ID 与投票 ID 二选一，同传以 poll_id 为准
    var deleted = await control.DeletePollAsync(new DeleteMeetingPollRequest
    {
        OperatorUserid = operatorUserid,
        InstanceId = hostInstance,
        Meetingid = meetingid,
        PollThemeId = pollThemeId,      // 删主题不影响已产生的投票实例
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/meeting/poll/delete");
}

// ⑫ E12 列表 / E13 主题定义（E13 是本组唯一 meetingid 非必填的一支）
var pollList = await control.GetPollListAsync(new GetMeetingPollListRequest
{
    OperatorUserid = operatorUserid,
    InstanceId = hostInstance,
    Meetingid = meetingid,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(pollList, "/cgi-bin/meeting/poll/get_poll_list");

var themeInfo = await control.GetPollThemeInfoAsync(new GetMeetingPollThemeInfoRequest
{
    OperatorUserid = operatorUserid,
    InstanceId = hostInstance,
    PollThemeId = pollThemeId ?? string.Empty,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(themeInfo, "/cgi-bin/meeting/poll/get_theme_info");

// ⑬ E9 结束会议：force_dismiss 默认 1（强制）；周期性会议还有子会议时 retrieve_code 必须为 0
var dismissed = await control.DismissMeetingAsync(new DismissMeetingRequest
{
    Meetingid = meetingid,
    ForceDismiss = 1,
    RetrieveCode = 0,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(dismissed, "/cgi-bin/meeting/realcontrol/dismiss");
```

### E2. `SetCoHostAsync` — 设置 / 撤销联席主持人

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/set_cohost")]
Task<WechatWorkResponse> SetCoHostAsync(
    [Body] SetMeetingCoHostRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`action`（`bool?`，官方必填：true 设置联席主持人 / false 撤销）** + `operated_user`（单对象 `MeetingOperatedUser`）。注释：**目前暂不支持 MRA 设备作为被操作者**；联席主持人结构复用 `MeetingHosts`（MT5 断言）。**返回**：`WechatWorkResponse`。

### E3. `MuteUserAsync` — 静音 / 解除静音单个成员

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/mute_user")]
Task<WechatWorkResponse> MuteUserAsync(
    [Body] MuteMeetingUserRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`option`（`bool?`，官方必填：true 静音 / false 解除静音）** + `operated_user`（`tmp_openid` + `instance_id`）。注释明确「**被操作成员的设备类型须与其实际设备保持一致，否则不生效**」，且**暂不支持 MRA 作为被操作者**。**返回**：`WechatWorkResponse`。

### E4. `SwitchUserVideoAsync` — 开关成员视频

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/switch_user_video")]
Task<WechatWorkResponse> SwitchUserVideoAsync(
    [Body] SwitchMeetingUserVideoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `video`（`bool?`：**默认 false 关闭视频；true 开启视频仅支持 MRA 设备**）+ `operated_user`。⇒ 与 E3 相反：**这是本族唯一「开启侧只对 MRA 有效」的动作**，MRA 支持度在本族逐端点不同。**返回**：`WechatWorkResponse`。

### E5. `CloseScreenShareAsync` — 关闭指定成员的屏幕共享

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/close_screen_share")]
Task<WechatWorkResponse> CloseScreenShareAsync(
    [Body] CloseMeetingScreenShareRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `operated_user`（均官方必填）。**暂不支持 MRA 设备作为被操作者**。**返回**：`WechatWorkResponse`。

### E6. `SetUserNicknamesAsync` — 修改成员昵称

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/set_nicknames")]
Task<WechatWorkResponse> SetUserNicknamesAsync(
    [Body] SetMeetingUserNicknamesRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`operated_users`（`MeetingOperatedNicknameUser` 列表，官方必填）**，每项 `tmp_openid` + `instance_id` + `nickname`（**昵称限制 20 个字符**）。**暂不支持 MRA**。**返回**：`WechatWorkResponse`。

> **单复数两形态并存**（MT5 锁定）：E2~E5 用 **`operated_user`**（单对象），E6/E8 用 **`operated_users`**（数组）；官方文档另有拼写为 `opereated_users` 的笔误页面 ⇒ **以守卫断言的 JSON 名为准**，勿「统一命名」。
### E7. `ManageWaitingRoomUsersAsync` — 等候室移入 / 移出

```csharp
[Post("/cgi-bin/meeting/realcontrol/manage_waiting_room_users")]
Task<WechatWorkResponse> ManageWaitingRoomUsersAsync(
    [Body] ManageMeetingWaitingRoomUsersRequest request,
    CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `meetingid` | `Meetingid` | `string?` | 官方必填 |
| `operate_type` | `OperateType` | `int?` | **1 等候室成员移入会议 / 2 会中成员移入等候室 / 3 等候室成员移出等候室**（官方必填） |
| `allow_rejoin` | `AllowRejoin` | `bool?` | **仅 `operate_type=3` 时才允许设置**；**对 MRA 设备不生效** |
| `operated_users` | `OperatedUsers` | `List<MeetingOperatedUser>?` | 官方必填 |

**返回**：`WechatWorkResponse`。

### E8. `KickoutUsersAsync` — 移出成员

**签名**

```csharp
[Post("/cgi-bin/meeting/realcontrol/kickout_users")]
Task<WechatWorkResponse> KickoutUsersAsync(
    [Body] KickoutMeetingUsersRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`allow_rejoin`（`bool?`，官方必填：true 允许再次入会 / false 不允许）** + **`operated_users`（数组，官方必填）**。**返回**：`WechatWorkResponse`。


### E9. `DismissMeetingAsync` — 结束会议

```csharp
[Post("/cgi-bin/meeting/realcontrol/dismiss")]
Task<WechatWorkResponse> DismissMeetingAsync(
    [Body] DismissMeetingRequest request,
    CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 取值 |
|---|---|---|---|
| `meetingid` | `Meetingid` | `string?` | 官方必填 |
| `force_dismiss` | `ForceDismiss` | `int?` | **0 不强制（有参会者则无法强停）/ 1 强制结束（默认）** |
| `retrieve_code` | `RetrieveCode` | `int?` | **0 不回收会议号（可重新入会，默认）/ 1 回收（不可再入会）**；**周期性会议还有子会议时必须设 0**；**对快速会议不生效（强制收回会议号）** |

> 域 README 第 4 节另记：周期性会议存在子会议时，`dismiss` 的 `retrieve_code` 为 0 是官方要求而非偏好。

> **投票八支的共同必传字段**（三支例外见各小节）：`operator_userid`（**操作者的 openid**，官方必填）、`instance_id`（操作者入会设备 id，官方必填；取值口径同会控成员设备类型）、`meetingid`（官方必填）。**两道闸门**：仅**进行中的会议**可调用、操作者须是**主持人或会议管理员**。
>
> **主题与实例是两层**：`poll_theme_id` 是题目定义，`poll_id` 是一次具体投票实例——`create_theme` **只回主题 ID**，必须经 E16 发起后才产生实例（E14 取结果与 E17 结束都用 `poll_id`）。

### E10. `CreatePollThemeAsync` — 创建投票主题

```csharp
[Post("/cgi-bin/meeting/poll/create_theme")]
Task<CreateMeetingPollThemeResponse> CreatePollThemeAsync(
    [Body] CreateMeetingPollThemeRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**（三个共同字段外）：`poll_topic`（**≤50 字符**，必填）+ `poll_desc`（**≤100 字符**，必填）+ `is_anony`（0 实名（默认）/ 1 匿名）+ `poll_questions[]`（必填，**最多 10 个问题**）。每题 `MeetingPollQuestion`：`question_desc`（**≤50 字符**，必填）+ `question_type`（**0 单选 / 1 多选**，必填）+ `poll_option`（`List<string>?`，必填，**最多 10 个、每个 ≤36 字符、创建时最少 2 个**）。

**返回**：`poll_theme_id`。

### E11. `UpdatePollThemeAsync` — 修改投票主题

```csharp
[Post("/cgi-bin/meeting/poll/update_theme")]
Task<WechatWorkResponse> UpdatePollThemeAsync(
    [Body] UpdateMeetingPollThemeRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：三个共同字段 + `poll_theme_id`（必填）+ 与 E10 同形的 `poll_topic` / `poll_desc` / `is_anony` / `poll_questions`。**注释：仅允许全覆盖修改**（须整份题目重传）；**选项最少 1 个**（比创建时的 2 个宽松）。**返回**：`WechatWorkResponse`。

### E12. `GetPollListAsync` — 获取会议内投票列表

```csharp
[Post("/cgi-bin/meeting/poll/get_poll_list")]
Task<GetMeetingPollListResponse> GetPollListAsync(
    [Body] GetMeetingPollListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：三个共同字段（无其它参数）。**返回**：`polls_theme_info[]`（`MeetingPollThemeInfo`：`poll_theme_id` + `polls_info[]`→`MeetingPollInfo`：`poll_id` / `poll_topic` / **`status` 0 未开始 / 1 投票中 / 2 已结束** / **`is_shared` 0 未共享 / 1 已共享**）。

### E13. `GetPollThemeInfoAsync` — 获取投票主题定义

```csharp
[Post("/cgi-bin/meeting/poll/get_theme_info")]
Task<GetMeetingPollThemeInfoResponse> GetPollThemeInfoAsync(
    [Body] GetMeetingPollThemeInfoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`operator_userid` + `instance_id` + **`poll_theme_id`（必填）**；`meetingid` **官方参数表标注非必填——本组投票端点中唯一可选的会议 ID**。

**返回**：`poll_topic` / `poll_desc` / `is_anony` / `poll_question_data[]`（`MeetingPollThemeQuestion`：`question_desc` / `question_type` / `option_info[]`（每项仅 `option_desc`））——**只有题目定义，不含投票结果**。

### E14. `GetPollDetailAsync` — 获取投票结果详情

```csharp
[Post("/cgi-bin/meeting/poll/get_poll_detail")]
Task<GetMeetingPollDetailResponse> GetPollDetailAsync(
    [Body] GetMeetingPollDetailRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：三个共同字段 + **`poll_id`（必填）**。

**返回**：`poll_theme_id` / `poll_topic` / `poll_desc` / `is_anony` / **`status`（1 投票中 / 2 已结束——注意比 E12 少一档 0）** / `is_shared` / **`vote_total_num`（投票人数）** / `poll_question_data[]`（`MeetingPollDetailQuestion` → `option_info[]`→`MeetingPollDetailOption`：`option_id` / `option_desc` / `option_num` 票数 / `rate` 比率 / **`option_user[]` 投票用户（成员身份，勿落日志）**）。

### E15. `DeletePollAsync` — 删除投票主题或投票实例

```csharp
[Post("/cgi-bin/meeting/poll/delete")]
Task<WechatWorkResponse> DeletePollAsync(
    [Body] DeleteMeetingPollRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：三个共同字段 + `poll_theme_id`（传入即删主题，**删除主题不影响已产生的投票实例**）与 `poll_id`（传入即删实例，**主题下实例全部被删时主题也删**）——**二选一，同传时以 `poll_id` 为准**。**返回**：`WechatWorkResponse`。

### E16. `StartPollAsync` — 发起投票

```csharp
[Post("/cgi-bin/meeting/poll/start")]
Task<StartMeetingPollResponse> StartPollAsync(
    [Body] StartMeetingPollRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：三个共同字段 + `poll_theme_id`（必填）。**返回**：**`poll_id`**——发起才产生实例，后续 E14 / E17 都用它。

### E17. `FinishPollAsync` — 结束投票

```csharp
[Post("/cgi-bin/meeting/poll/finish")]
Task<WechatWorkResponse> FinishPollAsync(
    [Body] FinishMeetingPollRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：三个共同字段 + `poll_theme_id` + `poll_id`（**两者均官方必填**）。**返回**：`WechatWorkResponse`。
## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`operated_user` vs `operated_users`** | 单对象用于 E2~E5；数组用于 E6/E8；官方文档另有 `opereated_users` 笔误页 ⇒ 以 MT5 断言的 JSON 名为准 | 「统一命名」后与官方报文错配 |
| `tmp_openid` + `instance_id` 必须成对且**与设备一致** | 注释：设备类型须与被操作者的实际设备一致，否则**不生效**（不报错） | 静音指令「成功」但成员仍发言 |
| **MRA 支持度逐端点相反** | E3/E5/E6/E2/E7 的 `allow_rejoin`：不支持 MRA；E4 `video=true`：**仅** MRA 可开启视频 | 用同一策略批量下发 |
| `option` / `action` / `video` / `allow_rejoin` 都是布尔 | E3 `option`、E2 `action`、E4 `video`、E8 `allow_rejoin` 均 `bool?`；而 `operate_type` / `enable_enter_mute` / `allow_chat` / `force_dismiss` / `retrieve_code` 是 `int?` | 混用类型或「统一成数字」 |
| **`allow_external_user` 语义方向相反** | 会控 E1：`true` = **仅企业成员可入会**；预约会议 `MeetingSettings.allow_external_user`：`true` = **所有成员可入会** | 同一布尔值在两侧效果相反 |
| `mute_all` 必传且门控 `allow_unmute_self` | E1 注释明确该联动 | 只设取消静音 |
| E9 的默认值是「强停 + 不回收」 | `force_dismiss` 默认 **1**、`retrieve_code` 默认 **0**；周期会议有子会议时**必须** 0；快速会议强制收回 | 误传 1 导致周期系列后续子会议不可用 |
| **投票三必传字段 + 主持人身份** | `operator_userid` 是**操作者的 openid**（注释原文，不是 userid）、`instance_id` 与 `meetingid` 必传；且**仅进行中会议 + 主持人或会议管理员** | 用应用身份或成员 userid 当 operator |
| **主题 ≠ 实例** | `poll_theme_id`（定义）与 `poll_id`（一次投票）；`get_poll_detail` / `finish` 用 `poll_id`，`start` 才产生它 | 用主题 ID 查结果 |
| 创建与修改的选项下限不同 | **创建每题最少 2 个选项；修改每题最少 1 个**；均 ≤10 个、每个 ≤36 字符、每题 `question_desc` ≤50、主题 ≤10 题 | 复用同一构造在修改时报错 |
| **E11 是全覆盖修改** | 注释「仅允许全覆盖修改」⇒ 必须整份题目重传 | 只传要改的题目导致其它题被清 |
| **E13 的 `meetingid` 非必填** | 本组唯一（官方参数表如此） | 强行本地校验必填 |
| E15 的优先级 | 主题 ID 与投票 ID 同传时**以 `poll_id` 为准**；删主题不影响已产生的实例 | 以为「都传就是全删」 |
| `status` 两套值 | 列表侧 `MeetingPollInfo.status`：**0 未开始 / 1 投票中 / 2 已结束**；结果侧 `GetMeetingPollDetailResponse.status`：**1 投票中 / 2 已结束**（无 0） | 用一套枚举覆盖另一套 |
| 结果是逐人可归因的数据 | `option_user[]` 含成员身份 ⇒ 只取计数，不落逐人选项到日志 | 暴露个人投票行为 |
| 全部官方即 POST | 本族 17 支全 POST（含 `get_poll_list` / `get_theme_info` / `get_poll_detail`）——全域仅 `layout/list_template` 是 GET | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验设备类型一致性与 MRA 支持面**、**不校验题目/选项数量与长度**、**不做「定义 vs 实例」的自动挑选**、**不做主持人身份预判**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 接口调用成功 | **「成功」不等于「设备侧生效」**：设备类型不一致时官方不生效但仍可能回 0 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

会议非进行中、操作者非主持人/管理员、`tmp_openid` 不存在、MRA 不受支持、题目或选项数量越界、`poll_id` / `poll_theme_id` 不存在、周期性会议仍有子会议却回收会议号等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98175~98189 / 98834~98841 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| 「不生效」的可观测方式 | 设备类型不一致时只说不生效，未给错误码或状态位 ⇒ 判定路径未记录 | [MeetingAdvanced.md](./MeetingAdvanced.md)（实时会中成员列表回读） |
| `instance_id` 取值全集 | 注释给出的枚举到 20/21（Rooms for Touch Windows / …）被截断显示，完整清单未留档 | 官方会控参数表 |
| `operator_userid` 的 openid 来源 | 注释称「操作者的 openid」，但本域无签发 openid 的端点 ⇒ 获取路径未记录（对照 `tmp_openid`） | 官方会控文档 + 回调事件 |
| 投票匿名与实名下的结果粒度 | `is_anony=1` 时 `option_user[]` 是否仍返回未记录 | 官方 98838 |
| `is_shared` 的设置端点 | 列表与详情都回传 `is_shared`，但**本域无共享开关端点** | 官方投票文档 |
| 联席主持人数量上限 | E2 未记可设置多少联席主持人 | 官方 98180 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 1 节状态机：预约状态走 `update/cancel`、会议中走 `realcontrol/*`；第 8 节动词分布；第 10 节 `operated_user(s)` 单复数与 `opereated_users` 笔误留档）
- 同域近邻：[Meeting.md](./Meeting.md)（预约状态的改期与取消、`settings.mute_all` / `allow_unmute_self` 的会前形态）· [MeetingAdvanced.md](./MeetingAdvanced.md)（**实时会中成员列表**——`tmp_openid` 与 `instance_id` 的取得处；等候室成员列表；会议健康度）· [MeetingMra.md](./MeetingMra.md)（MRA 设备的状态与操作，与本族的 MRA 限制互为对照）· [MeetingRecording.md](./MeetingRecording.md)（会中录制的暂停与停止动作、录制取回）
- 跨域边界：`Webhook` 域的群机器人与本族无关；投票结果的人员归因属个人数据，处理口径见 `AGENTS.md` §8
- 官方页：[会中设置（98175）](https://developer.work.weixin.qq.com/document/path/98175) · [联席主持人（98180）](https://developer.work.weixin.qq.com/document/path/98180) · [静音（98184）](https://developer.work.weixin.qq.com/document/path/98184) · [结束会议（98187）](https://developer.work.weixin.qq.com/document/path/98187) · [创建投票主题（98834）](https://developer.work.weixin.qq.com/document/path/98834) · [发起投票（98840）](https://developer.work.weixin.qq.com/document/path/98840) · [投票详情（98838）](https://developer.work.weixin.qq.com/document/path/98838)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
