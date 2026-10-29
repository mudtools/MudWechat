# Meeting 域 · 视频会议（预约会议 / 网络研讨会 / 会中控制 / 布局 / 录制 / Rooms / MRA / PSTN / 高级账号）

企业微信「视频会议」域是本仓最大的单域：**105 条官方路由、106 条声明方法**（守卫
[MT1](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)：`allRoutes.Should().HaveCount(105)`，
唯一重复是 `/cgi-bin/meeting/get_info` 在自建与第三方两分支同路由各声明一次），分属**十一族**、
共 **24 份接口文件**（守卫 MT3 断言会议域 `HaveCount(24)` = 基础族 4 接口 + 其余十族各 2 接口）：

| 族 | 端点 | 接口文件 | 守卫计数依据 |
|---|---|---|---|
| 预约会议基础管理 | 4 + 差异 2 | [IWechatWorkMeetingService.cs](./IWechatWorkMeetingService.cs)（父，4）/ [_Internal.cs](./IWechatWorkMeetingService_Internal.cs)（+1）/ [_ThirdParty.cs](./IWechatWorkMeetingService_ThirdParty.cs)（+1）/ [_Provider.cs](./IWechatWorkMeetingService_Provider.cs)（0，空标记） | MT1 `MeetingBaseRoutes` 4 + `MeetingInfoRoutes` 2；MT2 `AssertFamily(父 4, 自建 1 / 代开发 0 / 第三方 1)` |
| 会议统计管理 | 1 | [IWechatWorkMeetingStatisticsService.cs](./IWechatWorkMeetingStatisticsService.cs)（零端点父）/ [_Internal.cs](./IWechatWorkMeetingStatisticsService_Internal.cs) | MT1 `HaveCount(1)` |
| 预约会议高级管理 | 19 | [IWechatWorkMeetingAdvancedService.cs](./IWechatWorkMeetingAdvancedService.cs) / [_Internal.cs](./IWechatWorkMeetingAdvancedService_Internal.cs) | MT1 `HaveCount(19)`（enroll 7 + waitingroom 2 子段另断言） |
| 会中控制管理 | 17 | [IWechatWorkMeetingControlService.cs](./IWechatWorkMeetingControlService.cs) / [_Internal.cs](./IWechatWorkMeetingControlService_Internal.cs) | MT1 `HaveCount(17)`（realcontrol 9 + poll 8） |
| 网络研讨会管理 | 14 | [IWechatWorkMeetingWebinarService.cs](./IWechatWorkMeetingWebinarService.cs) / [_Internal.cs](./IWechatWorkMeetingWebinarService_Internal.cs) | MT1 `HaveCount(14)`（webinar/enroll 7 子段） |
| 电话入会（PSTN） | 3 | [IWechatWorkMeetingPstnService.cs](./IWechatWorkMeetingPstnService.cs) / [_Internal.cs](./IWechatWorkMeetingPstnService_Internal.cs) | MT1 `HaveCount(3)` |
| Rooms 会议室 | 12 | [IWechatWorkMeetingRoomsService.cs](./IWechatWorkMeetingRoomsService.cs) / [_Internal.cs](./IWechatWorkMeetingRoomsService_Internal.cs) | MT1 `HaveCount(12)` |
| 会议室连接器（MRA） | 4 | [IWechatWorkMeetingMraService.cs](./IWechatWorkMeetingMraService.cs) / [_Internal.cs](./IWechatWorkMeetingMraService_Internal.cs) | MT1 `HaveCount(4)` |
| 会议布局和背景 | 15 | [IWechatWorkMeetingLayoutService.cs](./IWechatWorkMeetingLayoutService.cs) / [_Internal.cs](./IWechatWorkMeetingLayoutService_Internal.cs) | MT1 `HaveCount(15)`（layout 9 + advanced_layout 6；唯一 GET 断言） |
| 录制管理 | 10 | [IWechatWorkMeetingRecordingService.cs](./IWechatWorkMeetingRecordingService.cs) / [_Internal.cs](./IWechatWorkMeetingRecordingService_Internal.cs) | MT1 `HaveCount(10)`（record/transcript 3 子段） |
| 高级功能账号 | 5 | [IWechatWorkMeetingPremiumAccountService.cs](./IWechatWorkMeetingPremiumAccountService.cs) / [_Internal.cs](./IWechatWorkMeetingPremiumAccountService_Internal.cs) | MT1 `HaveCount(5)` |

注册组 `Meeting`（十一族共用一个注册组与同一个
[`AddMeetingApi()`](../../Extensions/WechatWorkServiceBuilder.cs)），模块枚举 `WechatModule.Meeting`
（[WechatModule.cs](../../Extensions/WechatModule.cs)）；DTO 261 型统一进 `MeetingJsonContext`
（守卫 MT4）。逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 24 份接口文件的 XML 注释、
> [WechatMeetingContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1~MT5）、
> [Src/Work/Mud.Wechat.Work/DataModels/Meeting/](../../../Mud.Wechat.Work/DataModels/Meeting/) 下的 261 个 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。
> 注意：守卫文件内三处**注释与断言不一致**（L95「59 条官方路由」、L96「全部 POST」、MT3 横幅「8 个接口」），
> 本文一律以 `[Fact]` 断言值（105 条路由、1 条 GET、24 个接口）为准，冲突已登记缺口。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（守卫 MT3 对 24 个接口逐一断言
`TokenType = WechatTokenTypes.AccessToken` + `InjectionMode.Query` + `Name = "access_token"`，MUD005 已接受风险，
无法改 Header）；「开放面」指官方对该端点开放的应用类型，由 MT2 的层级断言锁死。
归属令牌键（自建 `InternalAccessToken` / 第三方与代开发 `CorpAccessToken`）由
[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)
全局锁定，MT3 注释明示此处不重复断言。

### 预约会议基础管理族（MT1 表 `MeetingBaseRoutes` + `MeetingInfoRoutes`，6 条声明）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateMeetingAsync` | POST | `/cgi-bin/meeting/create` | `access_token`(Query)；自建 + 第三方 + 代开发 | 99104/98148/93706/97454；返回 `meetingid` |
| 2 | `UpdateMeetingAsync` | POST | `/cgi-bin/meeting/update` | 同上 | 99047/98154/93710/97455；仅预约状态可改 |
| 3 | `CancelMeetingAsync` | POST | `/cgi-bin/meeting/cancel` | 同上 | 99048/98153/93709/97456；无业务负载，响应即 `WechatWorkResponse` |
| 4 | `GetUserMeetingIdListAsync` | POST | `/cgi-bin/meeting/get_user_meetingid` | 同上 | 99050/98714/93707/97457；跨度 ≤180 天 |
| 5 | `GetMeetingInfoAsync`（自建） | POST | `/cgi-bin/meeting/get_info` | `access_token`(Query)；**仅自建 + 第三方** | 99049/98149；MT1 断言该族差异端点只有两分支 |
| 6 | `GetMeetingInfoAsync`（第三方） | POST | `/cgi-bin/meeting/get_info` | `access_token`(Query)；同上 | 93708；代开发章节无该页 ⇒ `IWechatWorkProviderMeetingService` 为零端点空标记 |

### 会议统计管理族（1 条）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 7 | `GetMeetingStartListAsync` | POST | `/cgi-bin/meeting/statistics/get_start_list` | `access_token`(Query)；**仅自建** | 99651；路由挂 `statistics/` 段（MT1 专项断言，官方原文如此） |

### 预约会议高级管理族（19 条，全部仅自建）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 8 | `GetInviteesAsync` | POST | `/cgi-bin/meeting/get_invitees` | Query；仅自建 | 98160 |
| 9 | `SetInviteesAsync` | POST | `/cgi-bin/meeting/set_invitees` | Query；仅自建 | 98162；≤2000 人且管理员必须在列表内 |
| 10 | `CreateCustomerShortUrlAsync` | POST | `/cgi-bin/meeting/create_customer_short_url` | Query；仅自建 | 98818；不支持 Webinar，`customer_data` ≤256 字节 |
| 11 | `GetCustomerShortUrlAsync` | POST | `/cgi-bin/meeting/get_customer_short_url` | Query；仅自建 | 98819；不支持个人会议号与 Webinar |
| 12 | `GetRealtimeAttendeeListAsync` | POST | `/cgi-bin/meeting/get_realtime_attendee_list` | Query；仅自建 | 98157；`limit` ≤50 且须与首次 cursor 的 limit 一致 |
| 13 | `GetAttendeeListAsync` | POST | `/cgi-bin/meeting/get_attendee_list` | Query；仅自建 | 98156；区间 ≤31 天，两者都不传时最大跨度 90 天 |
| 14 | `GetWaitingRoomCurrentUserListAsync` | POST | `/cgi-bin/meeting/waitingroom/get_current_user_list` | Query；仅自建 | 98163；需已开启等候室且会议进行中 |
| 15 | `GetWaitingRoomUserListAsync` | POST | `/cgi-bin/meeting/waitingroom/get_user_list` | Query；仅自建 | 98164 |
| 16 | `CheckDeviceInMeetingAsync` | POST | `/cgi-bin/meeting/check_device_in_meeting` | Query；仅自建 | 98165 |
| 17 | `GetGuestsAsync` | POST | `/cgi-bin/meeting/get_guests` | Query；仅自建 | 99039 |
| 18 | `SetGuestsAsync` | POST | `/cgi-bin/meeting/set_guests` | Query；仅自建 | 99040；仅允许修改本应用创建的会议 |
| 19 | `GetQualityAsync` | POST | `/cgi-bin/meeting/get_quality` | Query；仅自建 | 98821；会议健康度，`sub_meetingid` 必传 |
| 20 | `SetEnrollConfigAsync` | POST | `/cgi-bin/meeting/enroll/set_config` | Query；仅自建 | 98797；报名问题最多 8 个选项、每选项 40 汉字、标题 40 字符 |
| 21 | `GetEnrollConfigAsync` | POST | `/cgi-bin/meeting/enroll/get_config` | Query；仅自建 | 98800；会议未开启报名返回未开启报名错误 |
| 22 | `QueryEnrollIdsAsync` | POST | `/cgi-bin/meeting/enroll/query_by_tmp_openid` | Query；仅自建 | 98794；`tmp_openid_list` 单次 ≤500 |
| 23 | `ListEnrollsAsync` | POST | `/cgi-bin/meeting/enroll/list` | Query；仅自建 | 98810；`limit` ≤50 |
| 24 | `ApproveEnrollsAsync` | POST | `/cgi-bin/meeting/enroll/approve` | Query；仅自建 | 98807；取消批准后状态回到待审批 |
| 25 | `ImportEnrollsAsync` | POST | `/cgi-bin/meeting/enroll/import` | Query；仅自建 | 98816 |
| 26 | `DeleteEnrollsAsync` | POST | `/cgi-bin/meeting/enroll/delete` | Query；仅自建 | 98817；`enroll_id_list` 为对象数组（守卫类注释陷阱条目） |

### 会中控制管理族（17 条，全部仅自建；realcontrol 9 + poll 8）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 27 | `SetRealtimeSettingsAsync` | POST | `/cgi-bin/meeting/realcontrol/set` | Query；仅自建 | 98175；仅进行中会议 |
| 28 | `SetCoHostAsync` | POST | `/cgi-bin/meeting/realcontrol/set_cohost` | Query；仅自建 | 98180；`co_hosts` 复用 `MeetingHosts` |
| 29 | `MuteUserAsync` | POST | `/cgi-bin/meeting/realcontrol/mute_user` | Query；仅自建 | 98184；`option` 为 `bool?` |
| 30 | `SwitchUserVideoAsync` | POST | `/cgi-bin/meeting/realcontrol/switch_user_video` | Query；仅自建 | 98189 |
| 31 | `CloseScreenShareAsync` | POST | `/cgi-bin/meeting/realcontrol/close_screen_share` | Query；仅自建 | 98185 |
| 32 | `SetUserNicknamesAsync` | POST | `/cgi-bin/meeting/realcontrol/set_nicknames` | Query；仅自建 | 98188；昵称 20 字符 |
| 33 | `ManageWaitingRoomUsersAsync` | POST | `/cgi-bin/meeting/realcontrol/manage_waiting_room_users` | Query；仅自建 | 98186；`allow_rejoin` 仅 `operate_type=3` 生效 |
| 34 | `KickoutUsersAsync` | POST | `/cgi-bin/meeting/realcontrol/kickout_users` | Query；仅自建 | 98181 |
| 35 | `DismissMeetingAsync` | POST | `/cgi-bin/meeting/realcontrol/dismiss` | Query；仅自建 | 98187；周期会议有子会议时 `retrieve_code`=0 |
| 36 | `CreatePollThemeAsync` | POST | `/cgi-bin/meeting/poll/create_theme` | Query；仅自建 | 98834；10 问题、主题 50 字符、描述 100 字符、每题 10 选项且最少 2 |
| 37 | `UpdatePollThemeAsync` | POST | `/cgi-bin/meeting/poll/update_theme` | Query；仅自建 | 98835；仅全覆盖修改；每题最少 1 选项 |
| 38 | `GetPollListAsync` | POST | `/cgi-bin/meeting/poll/get_poll_list` | Query；仅自建 | 98836 |
| 39 | `GetPollThemeInfoAsync` | POST | `/cgi-bin/meeting/poll/get_theme_info` | Query；仅自建 | 98837；本组唯一 `meetingid` 非必填 |
| 40 | `GetPollDetailAsync` | POST | `/cgi-bin/meeting/poll/get_poll_detail` | Query；仅自建 | 98838 |
| 41 | `DeletePollAsync` | POST | `/cgi-bin/meeting/poll/delete` | Query；仅自建 | 98839；主题 ID 与投票 ID 二选一，同传以投票 ID 为准 |
| 42 | `StartPollAsync` | POST | `/cgi-bin/meeting/poll/start` | Query；仅自建 | 98840；返回 `poll_id` |
| 43 | `FinishPollAsync` | POST | `/cgi-bin/meeting/poll/finish` | Query；仅自建 | 98841 |

### 网络研讨会管理族（14 条，全部仅自建；路由全部挂 `webinar/` 段）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 44 | `CreateWebinarAsync` | POST | `/cgi-bin/meeting/webinar/create` | Query；仅自建 | 98842；开始时间不早于当前时间半小时；最高 5w 人同时参会 |
| 45 | `UpdateWebinarAsync` | POST | `/cgi-bin/meeting/webinar/update` | Query；仅自建 | 98843；改 `hosts` 为覆盖原有 |
| 46 | `CancelWebinarAsync` | POST | `/cgi-bin/meeting/webinar/cancel` | Query；仅自建 | 98870 |
| 47 | `GetWebinarAsync` | POST | `/cgi-bin/meeting/webinar/get` | Query；仅自建 | 98860；`meetingid` / `meeting_code` 二选一，同传以 `meetingid` 为准 |
| 48 | `ListWebinarGuestsAsync` | POST | `/cgi-bin/meeting/webinar/list_guest` | Query；仅自建 | 98871 |
| 49 | `UpdateWebinarGuestsAsync` | POST | `/cgi-bin/meeting/webinar/update_guest_list` | Query；仅自建 | 98872 |
| 50 | `UpdateWebinarWarmUpAsync` | POST | `/cgi-bin/meeting/webinar/update_warm_up` | Query；仅自建 | 98882；暖场图片与视频二选一，同传以图片为准 |
| 51 | `SetWebinarEnrollConfigAsync` | POST | `/cgi-bin/meeting/webinar/enroll/set_config` | Query；仅自建 | 98875；问题类型额外支持 6-组织规模 |
| 52 | `GetWebinarEnrollConfigAsync` | POST | `/cgi-bin/meeting/webinar/enroll/get_config` | Query；仅自建 | 98874 |
| 53 | `QueryWebinarEnrollIdsAsync` | POST | `/cgi-bin/meeting/webinar/enroll/query_by_tmp_openid` | Query；仅自建 | 98873；`tmp_openid_list` ≤500 |
| 54 | `ListWebinarEnrollsAsync` | POST | `/cgi-bin/meeting/webinar/enroll/list` | Query；仅自建 | 98876；`limit` ≤50 |
| 55 | `ApproveWebinarEnrollsAsync` | POST | `/cgi-bin/meeting/webinar/enroll/approve` | Query；仅自建 | 98877 |
| 56 | `ImportWebinarEnrollsAsync` | POST | `/cgi-bin/meeting/webinar/enroll/import` | Query；仅自建 | 98880 |
| 57 | `DeleteWebinarEnrollsAsync` | POST | `/cgi-bin/meeting/webinar/enroll/delete` | Query；仅自建 | 98881 |

### 电话入会（PSTN）管理族（3 条，全部仅自建；路由全部挂 `phone/` 段）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 58 | `BatchCalloutAsync` | POST | `/cgi-bin/meeting/phone/callout` | Query；仅自建 | 98823；每次批量 50 路；Webinar 暂不支持外呼 |
| 59 | `GetCalloutStatusAsync` | POST | `/cgi-bin/meeting/phone/get_callout_status` | Query；仅自建 | 98824；不针对呼叫人做数据隔离 |
| 60 | `GetTmpOpenidAsync` | POST | `/cgi-bin/meeting/phone/get_tmp_openid` | Query；仅自建 | 98825；`phone_numbers` ≤20；同一座机可对应多个 `tmp_openid` |

### Rooms 会议室管理族（12 条，全部仅自建；路由全部挂 `rooms/` 段）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 61 | `BookMeetingRoomAsync` | POST | `/cgi-bin/meeting/rooms/book` | Query；仅自建 | 98791；会议时长 ≤24 小时且不支持周期性会议 |
| 62 | `ReleaseMeetingRoomAsync` | POST | `/cgi-bin/meeting/rooms/release` | Query；仅自建 | 98792 |
| 63 | `ListMeetingRoomsAsync` | POST | `/cgi-bin/meeting/rooms/list` | Query；仅自建 | 98795 |
| 64 | `GetMeetingRoomInfoAsync` | POST | `/cgi-bin/meeting/rooms/get_info` | Query；仅自建 | 98793 |
| 65 | `GetMeetingRoomConfigAsync` | POST | `/cgi-bin/meeting/rooms/get_config` | Query；仅自建 | 98802 |
| 66 | `ListMeetingRoomMeetingsAsync` | POST | `/cgi-bin/meeting/rooms/list_meetings` | Query；仅自建 | 98796；`meeting_room_id` 与 `rooms_id` 二选一；跨度 ≤90 天；`limit` 默认/最大 20 |
| 67 | `ListRoomDevicesAsync` | POST | `/cgi-bin/meeting/rooms/list_devices` | Query；仅自建 | 98798 |
| 68 | `ListRoomControllersAsync` | POST | `/cgi-bin/meeting/rooms/list_controllers` | Query；仅自建 | 98799 |
| 69 | `GetMeetingRoomInventoryAsync` | POST | `/cgi-bin/meeting/rooms/get_inventory` | Query；仅自建 | 98809；**无请求体 POST**，MT1 断言参数只余 `CancellationToken` |
| 70 | `CallMeetingRoomAsync` | POST | `/cgi-bin/meeting/rooms/call` | Query；仅自建 | 98804；`meeting_room_id` 与 `mra_address` 二选一 |
| 71 | `CancelCallMeetingRoomAsync` | POST | `/cgi-bin/meeting/rooms/cancel_call` | Query；仅自建 | 98805 |
| 72 | `GetMeetingRoomResponseStatusAsync` | POST | `/cgi-bin/meeting/rooms/get_response_status` | Query；仅自建 | 98806；`status` 0 无应答 / 1 未呼叫 / 2 入会中 / 3 被拒绝 / 4 呼叫中 / 5 取消呼叫（仅 Rooms）/ 6 已离会 |

### 会议室连接器（MRA）管理族（4 条，全部仅自建；路由全部挂 `mra/` 段）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 73 | `QueryMraStatusAsync` | POST | `/cgi-bin/meeting/mra/query_status` | Query；仅自建 | 98786 |
| 74 | `SetMraDefaultLayoutAsync` | POST | `/cgi-bin/meeting/mra/set_default_layout` | Query；仅自建 | 98787 |
| 75 | `SetMraRaiseHandAsync` | POST | `/cgi-bin/meeting/mra/set_raise_hand` | Query；仅自建 | 98788 |
| 76 | `HangupMraAsync` | POST | `/cgi-bin/meeting/mra/hangup` | Query；仅自建 | 98789 |

### 会议布局和背景管理族（15 条，全部仅自建；**含本域唯一 GET 端点**）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 77 | `ListLayoutTemplatesAsync` | **GET** | `/cgi-bin/meeting/layout/list_template` | Query；仅自建 | 98844；MT1 专项断言 `typeof(GetAttribute)`，且参数只余 `CancellationToken`（勿「顺手统一」为 POST） |
| 78 | `AddMeetingLayoutAsync` | POST | `/cgi-bin/meeting/layout/add` | Query；仅自建 | 98845；一场会议最多 10 个布局 |
| 79 | `AddAdvancedLayoutAsync` | POST | `/cgi-bin/meeting/advanced_layout/add` | Query；仅自建 | 98861；单会议最多 20 个；仅 H.323/SIP 会议室终端 |
| 80 | `UpdateMeetingLayoutAsync` | POST | `/cgi-bin/meeting/layout/update` | Query；仅自建 | 98846 |
| 81 | `UpdateAdvancedLayoutAsync` | POST | `/cgi-bin/meeting/advanced_layout/update` | Query；仅自建 | 98868；仅全量更新 |
| 82 | `SetDefaultMeetingLayoutAsync` | POST | `/cgi-bin/meeting/layout/set_default` | Query；仅自建 | 98847 |
| 83 | `ApplyAdvancedLayoutAsync` | POST | `/cgi-bin/meeting/advanced_layout/apply` | Query；仅自建 | 98869；`user_list` ≤20；MRA 不支持同框模式 |
| 84 | `ListMeetingLayoutsAsync` | POST | `/cgi-bin/meeting/advanced_layout/list` | Query；仅自建 | 98862 |
| 85 | `GetUserLayoutAsync` | POST | `/cgi-bin/meeting/advanced_layout/get_user_layout` | Query；仅自建 | 98865；`layout_type` 0 默认 / 2 自定义 / 3 个性 |
| 86 | `BatchDeleteLayoutsAsync` | POST | `/cgi-bin/meeting/advanced_layout/batch_delete` | Query；仅自建 | 98866；最多 20 个 ID；不校验存在性 |
| 87 | `AddMeetingBackgroundAsync` | POST | `/cgi-bin/meeting/layout/add_background` | Query；仅自建 | 98851；一场会议最多 7 个背景；PNG ≤10MB、分辨率最小 1920x1080；异步上传 |
| 88 | `SetDefaultMeetingBackgroundAsync` | POST | `/cgi-bin/meeting/layout/set_default_background` | Query；仅自建 | 98852 |
| 89 | `ListMeetingBackgroundsAsync` | POST | `/cgi-bin/meeting/layout/list_background` | Query；仅自建 | 98856 |
| 90 | `DeleteMeetingBackgroundAsync` | POST | `/cgi-bin/meeting/layout/delete_background` | Query；仅自建 | 98853 |
| 91 | `BatchDeleteMeetingBackgroundsAsync` | POST | `/cgi-bin/meeting/layout/batch_delete_background` | Query；仅自建 | 98854 |

### 录制管理族（10 条，全部仅自建；转写挂 `record/transcript/` 子段）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 92 | `ListMeetingRecordsAsync` | POST | `/cgi-bin/meeting/record/list` | Query；仅自建 | 98192；`meetingid` / `meeting_code` / `userid` 三选一；跨度 ≤31 天 |
| 93 | `GetRecordStatisticsAsync` | POST | `/cgi-bin/meeting/record/get_statistics` | Query；仅自建 | 98209；默认最近 31 天、跨度 ≤31 天 |
| 94 | `UpdateRecordSharingConfigAsync` | POST | `/cgi-bin/meeting/record/update_sharing_config` | Query；仅自建 | 98208 |
| 95 | `DeleteMeetingRecordAsync` | POST | `/cgi-bin/meeting/record/delete` | Query；仅自建 | 98206 |
| 96 | `DeleteRecordFileAsync` | POST | `/cgi-bin/meeting/record/delete_file` | Query；仅自建 | 98207 |
| 97 | `GetRecordFileAsync` | POST | `/cgi-bin/meeting/record/get_file` | Query；仅自建 | 98205 |
| 98 | `GetRecordFileListAsync` | POST | `/cgi-bin/meeting/record/get_file_list` | Query；仅自建 | 98196；播放/下载地址默认 6 小时过期（DTO 注释） |
| 99 | `GetRecordTranscriptParagraphsAsync` | POST | `/cgi-bin/meeting/record/transcript/get_paragraph_list` | Query；仅自建 | 98212；官方称返回段落总数但参数表无该字段（MT5 注释留档） |
| 100 | `GetRecordTranscriptDetailAsync` | POST | `/cgi-bin/meeting/record/transcript/get_detail` | Query；仅自建 | 98211；`pid` 含本身；续查用最后一条 `pid` |
| 101 | `SearchRecordTranscriptAsync` | POST | `/cgi-bin/meeting/record/transcript/search` | Query；仅自建 | 98213 |

### 高级功能账号管理族（5 条，全部仅自建；路由全部挂 `vip/` 段）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 102 | `AssignPremiumAccountsAsync` | POST | `/cgi-bin/meeting/vip/submit_batch_add_job` | Query；仅自建 | 99508；`userid_list` 单次 ≤100 |
| 103 | `GetAssignPremiumAccountsResultAsync` | POST | `/cgi-bin/meeting/vip/batch_add_job_result` | Query；仅自建 | 99508 同页第二端点 |
| 104 | `RevokePremiumAccountsAsync` | POST | `/cgi-bin/meeting/vip/submit_batch_del_job` | Query；仅自建 | 99509 |
| 105 | `GetRevokePremiumAccountsResultAsync` | POST | `/cgi-bin/meeting/vip/batch_del_job_result` | Query；仅自建 | 99509 同页第二端点 |
| 106 | `ListPremiumAccountsAsync` | POST | `/cgi-bin/meeting/vip/list` | Query；仅自建 | 99510；`limit` 默认 100 最大 200，须用 `has_more` 判断 |

## 1. `meetingid` 生命周期与「预约状态」状态机

创建端点 #1 的应答 [CreateMeetingResponse.cs](../../DataModels/Meeting/CreateMeetingResponse.cs)
只给出 `meetingid`（JSON 名 **`meetingid`，无下划线**，`string?`）与 `excess_users`；
`meeting_code`（会议号）与 `meeting_link`（入会链接）**仅预约会议高级管理文档页声明**，
基础文档页不承诺 ⇒ 不要按基础面假定一定能拿到链接。

写侧三端点（#2 修改、#3 取消）的状态门槛由接口 XML 注释留档，且两两不同：

| 端点 | 官方限制（接口注释原文口径） |
|---|---|
| #2 `update` | 仅允许修改**当前应用创建的**、**处于预约状态下**的会议；若会议为指定创建者的老会议，修改时不允许指定应用身份创建的新日历 |
| #3 `cancel` | 仅允许取消**当前应用创建**的会议；仅允许取消**处于预约状态下**的会议 |
| #5/#6 `get_info` | 只能拉取该应用创建的会议；快速会议仅返回已参与成员列表 |

⇒ 会议一旦进入「会议中 / 已结束」，`update` 与 `cancel` 都不再可用，会控改走
`realcontrol/*`（#27~#35，且要求仅进行中会议可调）。读取侧的状态由
[GetMeetingInfoResponse.cs](../../DataModels/Meeting/GetMeetingInfoResponse.cs)
的 `Status`（`int?`，JSON `status`）承载：**1 待开始 / 2 会议中 / 3 已结束 / 4 已取消 / 5 已过期**；
`MeetingType`（`int?`）取值 **0 一次性 / 1 周期性 / 2 微信专属 / 3 Rooms 投屏 / 5 个人会议号 / 6 网络研讨会**
（仅高级管理文档页声明该字段）。本仓**不做**本地状态校验，越界由应答 `errcode` 表达。

「创建 ⇒ 修改 ⇒ 取消」这条链上还有一个易漏点：`meetingid` 与 `meeting_code` 不是同一个 ID，
`webinar/get`（#47）与 `record/list`（#92）允许二者之一定位会议，但语义与优先级各端点不同（见 §10）。

## 2. 参会人三件套与条件必填：`invitees` / `guests` / 受邀成员列表

创建与修改请求里的参会口径完全不同，且本域存在**两种同名不同形**的受邀结构（MT5 断言锁定）：

| 类型 | 属性 → JSON 名 | 口径 |
|---|---|---|
| [MeetingInvitees.cs](../../DataModels/Meeting/MeetingInvitees.cs) | `Userid` → `userid`（`List<string>?`） | 创建/修改预约会议请求的 `invitees` 嵌套对象；**任何 userid 不合法或不在应用可见范围内将直接报错**（不是静默过滤） |
| [MeetingInvitee.cs](../../DataModels/Meeting/MeetingInvitee.cs) | `Userid` → `userid`（`string?`） | #8/#9 受邀成员列表端点的 `invitees` 为**对象数组**，与上表形态不同 ⇒ 两者不可互换 |

- 人数上限（#1 注释）：普通企业最多 **100** 人；付费企业由所购在线会议室/高级账号容量决定、最多 **300** 人；
  **超过 300 人须改走 #9 `set_invitees`**（单次 ≤ **2000** 名受邀成员，且**管理员必须在受邀成员列表中**）。
  ⇒ 「先创建 ≤300 人，再用 set_invitees 扩容」是官方规定的形态，不是本仓约束。
- [MeetingGuest.cs](../../DataModels/Meeting/MeetingGuest.cs) 的嘉宾是**手机号身份**（`area` / `phone_number` / `guest_name`，
  `area` 注释明示中国传 `86`、不是 `+86` 也不是 `0086`，姓名 1~16 位字符），
  与外部联系人 ID 无关；`guests` 仅预约会议高级管理文档页声明 ⇒ 基础面写入不保证生效。
- 主持人：`settings.hosts`（[MeetingHosts.cs](../../DataModels/Meeting/MeetingHosts.cs)，`userid` 数组）
  **仅购买了会议高级/专业功能的企业可指定**，且传入创建者 userid 会被官方**自动过滤**；
  #28 `set_cohost` 的 `co_hosts` 复用同一 `MeetingHosts` 型（MT5 断言）。
- `location` 与 `description` 均为可选：写入侧 `description` ≤ **500 字节**、`location` ≤ **128 字符**；
  读取侧 `GetMeetingInfoResponse` 注释给出的上限是 `title` ≤ **60 字节**、`description` ≤ **600 字节**
  ⇒ 读写两端的字节上限不一致，写入校验只可按请求侧口径，勿按详情侧放宽。
- `admin_userid`（发起会议的管理员 userid）与 `meeting_start` 在 [CreateMeetingRequest.cs](../../DataModels/Meeting/CreateMeetingRequest.cs)
  中注释为**官方必填**，但类型仍是 `string?` / `long?`（本域未采用「必填用非可空类型」的表达方式，与会议室域 MR8 的做法不同）
  ⇒ 缺字段不会编译报错，只能靠调用方自检；条件必填（如 Webinar 的 `admission_type=2` 时 `password` 必传）同理，SDK 不本地拦截。

## 3. 时间字段单位、时长区间与「成对修改」

- 所有时间戳都是 **Unix 秒**：`meeting_start` / `meeting_duration` / `begin_time` / `end_time` / `repeat_until` /
  `first_join_time` / `last_quit_time` 等；类型上开始时间用 `long?`、时长用 `int?`（秒），
  请求侧**没有** `meeting_end_time` 这种字段——结束时间由「开始 + 时长」推导。
- 创建（#1）官方区间：开始时间需**大于当前时间**；持续时长最小 **300 秒**、最大 **86399 秒**（即不足 24 小时）；
  标题最多 **40 字节或 20 个 utf8 字符**。
- 修改（#2）的硬约束：`meeting_start` 与 `meeting_duration` **必须成对修改**（改其一必须同时传其二），
  接口注释与 [UpdateMeetingRequest.cs](../../DataModels/Meeting/UpdateMeetingRequest.cs) 类注释双处留档。
- 修改请求另有 `remind_time`（`int?`，JSON `remind_time`），与创建请求的 `reminders.remind_before`
  **不是同一个概念**：`remind_before` 是数组、取值仅支持 `0/300/900/3600/86400`（MT5 断言 `List<int>`），
  传入其他值表现为「会议开始时提醒」。
- 查询区间的跨度上限逐端点不同，且**不是一套值**：#4 ≤ **180 天**（两者都不填时默认 `end_time` 为当前时间）、
  #13 ≤ **31 天**（为空默认前推 31 天；两者都不传时最大可查 90 天）、#92/#93 ≤ **31 天**、
  #66 ≤ **90 天**、#7 会议发起记录 ≤ **30 天**（左闭右开、按时间倒序）。
- Webinar 侧时间是**字符串**（见 §10 陷阱），与预约会议的数值时间不兼容。

## 4. 周期性会议：改单次与改整个系列

周期性完全落在 `reminders` 子对象（[MeetingReminders.cs](../../DataModels/Meeting/MeetingReminders.cs)，
创建/修改/详情三处共用同一型，MT5 断言）：

| 字段 | 类型 | 口径 |
|---|---|---|
| `is_repeat` | `int?` | 1 周期性；0 默认（数值承载，非 bool） |
| `repeat_type` | `int?` | 0 每天 / 1 每周 / 2 每月 / 7 每个工作日 |
| `repeat_interval` | `int?` | 基础文档页仅「每周」且 ≤2；高级文档页自定义重复时必填 |
| `is_custom_repeat` | `int?` | 1 自定义重复（仅 `is_repeat`=1 生效） |
| `repeat_day_of_week` / `repeat_day_of_month` | `List<int>?` | 1~7（周一至周日）/ 1~31 号 |
| `repeat_until_type` | `int?` | 0 按日期 / 1 按次数（官方参数表误写 `uint32[]`，MT5 注释锁定按单值 int） |
| `repeat_until_count` | `int?` | 默认 7 次 |
| `repeat_until` | `long?` | Unix 时间戳；超出或未设时取上限 |
| `remind_before` | `List<int>?` | 仅 0 / 300 / 900 / 3600 / 86400 |

重复次数上限（#1 注释）：**每天 / 每个工作日 / 每周最多 200 次，每两周 / 每月最多 50 次**；
超出或未设 `repeat_until` 时官方按该上限截断。

「改单次 vs 改整个系列」的分界在 **`sub_meetingid`**：
[CancelMeetingRequest.cs](../../DataModels/Meeting/CancelMeetingRequest.cs) 的
`SubMeetingid` 是 `string?`（MT5 专项断言），注释明示「**不传则取消该系列的周期性会议**」⇒
取消单次必须显式带子会议 ID。周期性还影响读取面：#12 实时参会列表要求周期会议**必传** `sub_meetingid`，
#19 健康度同样必传，#13 会中参会列表则「对周期性会议查询暂时不生效，请使用分页参数查询」。
详情应答用 `sub_meetings` / `has_more_sub_meeting` / `remain_sub_meetings` / `current_sub_meetingid`
表达系列剩余次数（接口 XML 返回值清单留档）。会控 #35 `dismiss` 在周期会议存在子会议时 `retrieve_code` 为 0。

## 5. 本域无「会议附件」通道：素材上传归 Media 域

本目录 24 份接口文件与 261 个 DTO 中**不存在** `userdoc` / `mediadoc` / `media_id` 形态的会议附件参数
（已按关键字全量核对）；创建/修改预约会议的请求体就是 §2 表格里那 11/12 个键，没有挂载文档或素材的位置。
本域与「素材」相关的只有两条：

- 布局背景 #87 `layout/add_background`：注释明示「背景图片上传方式为**异步上传**，可以通过订阅
  「素材上传结果」获取上传结果通知」；Webinar 封面同理（`cover_url` / `description` 需开启 `activity_page`，封面为异步上传）。
- 录制文件 #97/#98：返回的是**下载地址**而非素材 ID，
  [RecordFileAddress.cs](../../DataModels/Meeting/RecordFileAddress.cs) 与
  [GetRecordFileResponse.cs](../../DataModels/Meeting/GetRecordFileResponse.cs) 注释均为
  「默认 6 小时过期」；另有一处 [RecordFileDownload.cs](../../DataModels/Meeting/RecordFileDownload.cs)
  注释是「链接有效期为一个小时」⇒ 两条链路的有效期口径不同，按各自 DTO 注释取用，勿混用。

「先上传再引用」的通用素材端点属于 Media 域（[../Media/IWechatWorkMediaService.cs](../Media/IWechatWorkMediaService.cs)，
`/cgi-bin/media/upload`），本域不代理；官方文档里会议/日程侧的 `userdoc` 附件是否适用于预约会议创建接口，
本仓无声明 ⇒ 已登记缺口。

## 6. 外部参会人边界：`tmp_external_userid` / `tmp_openid` 不在本域转换

会议里的「非企业成员」有两套临时 ID，**都是不可透明化的字符串**：

- 读取侧：[GetMeetingInfoResponse.cs](../../DataModels/Meeting/GetMeetingInfoResponse.cs) 的
  `attendees`（[MeetingAttendees.cs](../../DataModels/Meeting/MeetingAttendees.cs)）分 `member`（企业成员）与
  `tmp_external_user`（[MeetingTmpExternalUser.cs](../../DataModels/Meeting/MeetingTmpExternalUser.cs)，
  含 `status` 1 已参与 / 2 未参与、`first_join_time`、`last_quit_time`、`total_join_count`；微信入会用户不返回）。
  该型的类注释留档官方契约陷阱：**`tmp_external_userid` 同一用户在不同会议中取值不同**。
- 报名侧：#22/#53 用 `tmp_openid` 反查报名 ID（`tmp_openid_list` 单次 ≤500），PSTN #60 则用手机号换 `tmp_openid`。

⇒ 转换出 `external_userid` 的能力**不在本域**，属
[../AccountId/TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService.cs](../AccountId/TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService.cs)
（「tmp_external_userid 的转换」）；客户联系侧的 `external_userid` 语义见 `ExternalContact` 域。
本域内出现 `external` 的只有 `settings.allow_external_user`（`bool?`，是否允许外部参会人入会）这一个开关，
不要把它读成「返回外部联系人 ID」。参会人姓名、手机号等示例值一律用假值。

## 7. 与日程域（Schedule）的耦合：`cal_id` 不是 `schedule_id`

会议与日程是两套 ID 体系，本域的耦合点只有一个参数：`cal_id`
（`string?`，≤64 字节，[CreateMeetingRequest.cs](../../DataModels/Meeting/CreateMeetingRequest.cs) /
[UpdateMeetingRequest.cs](../../DataModels/Meeting/UpdateMeetingRequest.cs)）。接口注释口径：

- `cal_id` **不填则插入参与者的默认日历**；**第三方应用必须指定**，且会议所属日历须为 `access_token` 对应应用创建的日历
  （#1 注释与父接口 remarks、[_ThirdParty.cs](./IWechatWorkMeetingService_ThirdParty.cs) /
  [_Provider.cs](./IWechatWorkMeetingService_Provider.cs) 的 remarks 三处一致）。
- 修改会议时「若会议为指定创建者的老会议，不允许指定应用身份创建的新日历」⇒ 日历归属有历史兼容限制。

日历与日程的写入面在 [../Schedule/Calendar/IWechatWorkScheduleCalendarService.cs](../Schedule/Calendar/IWechatWorkScheduleCalendarService.cs)
与 [../Schedule/Schedule/](../Schedule/Schedule/)，**本域不提供任何日程端点**，也不会因创建会议而返回 `schedule_id`。
反向耦合（会议室预定与日程的绑定）在 MeetingRoom 域，见
[../MeetingRoom/README.md](../MeetingRoom/README.md)。⇒ 排查「会议有没有出现在日历/日程里」时，
先用本域 `cal_id` + `meetingid` 定位，再用日程域接口读，不要用本域端点硬凑。

## 8. 查询端点官方也是 POST：动词分布只有 1 个例外

MT1 对每个路由表项断言 `HttpAttribute`，全域 **105 条路由里只有 #77
`/cgi-bin/meeting/layout/list_template` 是 GET**（专项断言 `typeof(GetAttribute)`，理由写「勿顺手统一为 POST」）。
其余 104 条（含全部 `get_*` / `list_*` / `query_*` / `search`）都是 POST，参数走请求体：

- 三个「无请求体」端点用的仍是 POST，且 MT1 断言其方法参数 `ContainSingle` 且必须是 `CancellationToken`：
  #69 `rooms/get_inventory`、#77 `layout/list_template`（GET，故无体理所当然）、以及详情族里同类形态。
  ⇒ 新增无体端点时不要给它加空请求 DTO，也不要顺手改成 GET。
- 无业务负载的写端点（如 #3 `cancel`）响应直接用 [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs)，
  MT1 有 `ReturnType.Should().Be(typeof(Task<WechatWorkResponse>))` 断言，不得新建空响应型。
- 翻页一律是 `cursor` + `limit`，响应侧字段名是 **`next_cursor`**（MT5 锁定），
  [GetUserMeetingIdListRequest.cs](../../DataModels/Meeting/GetUserMeetingIdListRequest.cs)
  注释留档「初次调用可填 `"0"`」；`next_cursor` 未返回或为空字符串表示数据取完。
  #12/#66 另有「后续 `limit` 必须与首次一致」的约束——翻页时改 `limit` 会失败。

## 9. 批次上限与频率口径

守卫与接口注释留档的数值上限（**逐端点，不同族不同值，不可互相借用**）：

| 端点 | 批次/容量上限 |
|---|---|
| #4 | `limit` 默认值与最大值均为 100；时间跨度 ≤180 天 |
| #9 | 单次 ≤2000 名受邀成员 |
| #12 | `limit` ≤50（且与首次 cursor 的 limit 一致） |
| #13 | `limit` ≤100；区间 ≤31 天 |
| #14 / #15 | 等候室当前列表 `limit` 默认 10 最大 50；历史列表默认 20 最大 50 |
| #19 | 健康度 `limit` 默认与最大 50 |
| #22 / #53 | `tmp_openid_list` ≤500 |
| #23 / #54 | 报名列表 `limit` ≤50 |
| #58 | 每次批量外呼 50 路 |
| #59 | 外呼状态分页最大 100 |
| #60 | `phone_numbers` ≤20 |
| #66 | `limit` 默认与最大 20；跨度 ≤90 天 |
| #78 / #79 / #83 / #86 / #87 | 单会议 ≤10 个布局；≤20 个高级布局；`user_list` ≤20；批量删除 ≤20 个 ID；≤7 个背景 |
| #92 | `limit` 默认 10 最大 20；跨度 ≤31 天 |
| #102 / #104 | `userid_list` 单次 ≤100 |
| #106 | `limit` 默认 100 最大 200，必须用 `has_more` 判断是否还有下一页 |
| #7 | `limit` 默认 200 最大 1000；跨度 ≤30 天；因过滤「发起者不可见」记录，返回条数可能少于 `limit` |

频率口径：本域接口注释里**没有**逐端点的 QPS/日配额数值（与上下游域不同），
会议室族走官方附录默认阈值这一说法只在
[../MeetingRoom/README.md](../MeetingRoom/README.md) 有留档。
⇒ 本域限流与批次裁剪一律不做，越界由 `errcode` 表达；频率数值属**未逐项核验**，见缺口表。

## 10. 字段名原文保留：本域的官方反直觉点（MT5 锁定）

MT5 `MeetingDataModels_ShouldLockOfficialContractTraps` 把下列形态钉成断言，改名即破坏契约：

- **`meetingid` 没有下划线**；批量侧是 `meetingid_list`（[GetMeetingQualityRequest 等]），
  `meeting_code`、`meeting_start`、`meeting_duration` 才有下划线。
- 会控的操作对象**单复数两种形态并存**：`operated_user`（单对象）与 `operated_users`（数组），
  官方文档里还存在拼写为 `opereated_users` 的页面笔误；同族另有 `instanceid`（官方示例）与
  本仓 `InstanceId`（JSON `instance_id`）的取舍记录。⇒ 以 MT5 断言的 JSON 名为准，勿按「统一命名」改写。
- 官方示例中的 `cursort`（#12 实时参会列表）是**文档笔误**，本仓按 `cursor` 建模（守卫注释留档）；
  `record_list` 等同理：参数表与示例不一致时，一律以守卫断言为准。
- 数字承载布尔的字段保持数值类型：`is_repeat`、`repeat_type`、`is_custom_repeat`、`repeat_until_type`、
  `remind_scope`（1 不提醒 / 2 仅提醒主持人 / 3 提醒所有成员 / 4 指定部分成员响铃）、
  `enable_enter_mute`（`int?`）等；而 `settings.password` 官方参数表标注 bool、示例按字符串传输
  （"1234"）⇒ [MeetingSettings.cs](../../DataModels/Meeting/MeetingSettings.cs) 用 `string?` 承载。
- `MeetingSettings` 是**三处共用的并集超集**（创建 / 修改 / 详情），仅详情回传的
  `enable_doc_upload_permission` / `current_hosts` / `co_hosts` 与仅创建页声明的 `enable_interpreter`
  都以可空属性承载、未赋值不输出；`host_key`（`string?`）是 **6 位数字字符串**，不要用整型承载。
- Webinar 族的同义字段名与普通会议不同：**`title` 而非 `subject`**、`status` 是字符串
  （`MEETING_STATE_*`）、`start_time` 是字符串；入会静音在两处分别叫
  `EnableEnterMute`（[WebinarMediaSetting.cs](../../DataModels/Meeting/WebinarMediaSetting.cs)）与
  `MuteEnableJoin`（[WebinarMediaSettingInfo.cs](../../DataModels/Meeting/WebinarMediaSettingInfo.cs)）；
  主持人列表型是 `List<WebinarHostInfo>`、嘉宾型是 `List<WebinarGuest>`（MT5 断言）。
- Rooms 族的字段形态自成一套：`meeting_room_id_list`、`rooms_id`（与 `meeting_room_id` 并存且二选一）、
  [RoomsMeetingSettings] 与 `MeetingSettings` **不同构**、`RoomsControllerInfo.Status` 是字符串 `"0"`/`"1"`、
  `RoomsMraAddress.DialString` → `dial_string`（MT5 断言）。
- MRA 族的设备引用是 [MraDeviceRef.cs](../../DataModels/Meeting/MraDeviceRef.cs)
  的 `TmpOpenid`（JSON `tmp_openid`）。
- 报名删除与审批的 `enroll_id_list` **同名不同形**：删除请求是对象数组（`MeetingEnrollIdRef`），
  审批请求是字符串数组（守卫类头部注释留档，普通会议与 Webinar 报名同构同规）。

## 11. Rooms 会议室族在本域，MeetingRoom 域是另一棵树

`/cgi-bin/meeting/rooms/*`（#61~#72，12 端点）是**视频会议域内的硬件会议室/Rooms 终端**管理：
预定的是「会议用的 Rooms 设备」，配套 MRA（#73~#76）、布局（#77~#91）与呼叫（#70~#72）。
它与 [../MeetingRoom/README.md](../MeetingRoom/README.md)（`/cgi-bin/oa/meetingroom/*`，10 端点）
分属官方**两棵章节树**、ID 体系与 DTO 型都不同（`MeetingRoomInfo` vs `RoomsMeetingRoom`，
`RoomsMeetingSettings` ≠ `MeetingSettings`，MT5 断言）。MeetingRoom 域只有自建应用开放、
无第三方/代开发面；本域 Rooms 族同样只有自建子接口，但**基础管理族**是唯一同时向三类应用开放的族。
⇒ 需求写「会议室」时先确认路由前缀是 `/cgi-bin/meeting/rooms/` 还是 `/cgi-bin/oa/meetingroom/`，
两域各写各的端点，不要在一侧补另一侧的能力。

## 12. 开放面与令牌：十一族里只有基础族对三类应用开放

MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces` 用同一个 `AssertFamily` 辅助断言十一族，规则是：

- 父接口一律 `[HttpClientApi(..., IsAbstract = true)]` ⇒ **不可注入**，且 `RegistryGroupName` 必须为空；
- 可注入子接口一律 `RegistryGroupName = "Meeting"`，其生成器契约的 `InheritedFrom` 必须等于父接口实现类名；
- 基础族：父接口声明面恰 **4**，子接口声明面分别为自建 **1**、代开发 **0**（零端点空标记）、第三方 **1**；
- 其余十族：父接口声明面恰 **0**，唯一自建子接口声明面 1/19/17/14/3/12/4/15/10/5。

⇒ 除基础族外**没有** `..._ThirdParty.cs` / `..._Provider.cs` 文件，这是官方无第三方/代开发文档页的结果，
不是本仓漏挂；给这些族「顺手补个第三方子接口」会同时改坏 MT1 与 MT2。
权限口径按接口注释：自建应用需配置到「可调用接口的应用」列表，高级功能账号族写的是
「协作 - 会议 - 可调用接口的应用」，代开发与第三方在该族暂不支持。
第三方/代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`
切到授权企业作用域（`permanent_code`、`corpsecret`、`auth_code`、`suite_ticket` 一律只写参数名，
不得进日志、遥测或异常消息）。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册会议模块（十一族共用一个注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMeetingApi());

// 自建应用注入子接口（父接口 IsAbstract，不可注入）
public sealed class MeetingOps(
    IWechatWorkInternalMeetingService meetings,
    IWechatWorkInternalMeetingAdvancedService advanced,
    IWechatWorkInternalMeetingStatisticsService statistics)
{
    // 1) 创建预约会议：开始时间必须大于当前时间，时长 300~86399 秒，参会人须在应用可见范围内
    public async Task<string> CreateAsync(string ownerUserId, CancellationToken ct)
    {
        var resp = await meetings.CreateMeetingAsync(new CreateMeetingRequest
        {
            AdminUserid = ownerUserId,                      // 官方必填
            Title = "季度评审",                              // ≤40 字节或 20 个 utf8 字符
            MeetingStart = DateTimeOffsetOffset(),          // long Unix 秒，须大于当前时间
            MeetingDuration = 3600,                         // 秒，300~86399
            Location = "线上",                               // ≤128 字符
            Description = "议题见周报",                       // ≤500 字节
            Invitees = new MeetingInvitees
            {
                Userid = ["zhangsan.fake", "lisi.fake"],    // 假值示例；不合法直接报错，不静默过滤
            },
            Reminders = new MeetingReminders
            {
                RemindBefore = [300],                       // 仅 0/300/900/3600/86400
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/meeting/create");
        return resp.Meetingid!;                             // JSON 名 meetingid，无下划线
    }

    private static long DateTimeOffsetOffset()
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600;

    // 2) 参会人超过 300 人：改用「更新会议受邀成员列表」，单次 ≤2000 且管理员必须在列表内
    public async Task ExpandInviteesAsync(string meetingid, List<string> userIds, CancellationToken ct)
    {
        var resp = await advanced.SetInviteesAsync(new SetMeetingInviteesRequest
        {
            Meetingid = meetingid,
            Invitees = userIds.Select(u => new MeetingInvitee { Userid = u }).ToList(),
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/meeting/set_invitees");
    }

    // 3) 拉取某成员的会议 ID 列表：cursor 初次可填 "0"，用 next_cursor 续拉
    public async Task<List<string>> ListMeetingIdsAsync(string userId, CancellationToken ct)
    {
        var collected = new List<string>();
        string? cursor = "0";
        while (true)
        {
            var resp = await meetings.GetUserMeetingIdListAsync(new GetUserMeetingIdListRequest
            {
                Userid = userId,
                Cursor = cursor,
                Limit = 100,                                // 默认值与最大值均为 100
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/meeting/get_user_meetingid");

            List<string> batch = resp.MeetingidList ?? new List<string>();
            collected.AddRange(batch);

            if (string.IsNullOrEmpty(resp.NextCursor)) break;
            cursor = resp.NextCursor;
        }

        return collected;
    }

    // 4) 取消周期会议的「某一次」：sub_meetingid 不传会取消整个系列
    public async Task CancelOneOccurrenceAsync(string meetingid, string subMeetingid, CancellationToken ct)
    {
        var resp = await meetings.CancelMeetingAsync(new CancelMeetingRequest
        {
            Meetingid = meetingid,
            SubMeetingid = subMeetingid,                   // 仅高级管理文档页声明
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/meeting/cancel");
    }

    // 5) 会议发起记录：跨度 ≤30 天、左闭右开，返回条数可能少于 limit
    public async Task<List<MeetingStartRecord>> RecentStartsAsync(
        long beginTime, long endTime, CancellationToken ct)
    {
        var resp = await statistics.GetMeetingStartListAsync(new GetMeetingStartListRequest
        {
            BeginTime = beginTime,
            EndTime = endTime,
            Limit = 200,
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/meeting/statistics/get_start_list");

        List<MeetingStartRecord> records = resp.StartList ?? new List<MeetingStartRecord>();
        return records;
    }
}

// 第三方应用：先切到授权企业作用域，且必须指定 cal_id（日历须为本应用创建）
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var created = await thirdPartyMeeting.CreateMeetingAsync(new CreateMeetingRequest
    {
        AdminUserid = ownerUserId,
        Title = "巡检例会",
        MeetingStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1800,
        MeetingDuration = 1800,
        CalId = calendarIdCreatedByThisApp,                 // 第三方必填
    }, ct);
    WechatWorkException.ThrowIfFailed(created, "/cgi-bin/meeting/create");
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域逐端点频率/日配额 | 24 份接口注释里**没有**任何 QPS、日配额数值（仅批次上限与跨度上限）⇒ **未逐项核验** | 需要限流时逐页核验官方频率附录，再补接口 remarks，勿借用邻域数值 |
| 会议附件（`userdoc` / `mediadoc` / `media_id`） | 本域接口与 261 个 DTO 全量核对后**无该形态参数**；素材上传能力在 Media 域 ⇒ 官方预约会议参数表是否含文档附件**未在本域核验** | 需要挂载文档时先核验官方页；确属日程域能力则归 `Schedule`，勿在本域加参数 |
| `settings` / `reminders` 的条件必填组合 | 注释只留档 `repeat_interval` 的「基础页仅每周、高级页自定义重复时必填」与 `password`/`host_key` 的形态说明；其余「何时必填」无独立断言 ⇒ **未逐项核验** | 需要本地校验前先补 MT5 级守卫断言，勿在 SDK 内加隐式默认值 |
| 高级账号（#102~#105）的任务终态与留存 | 注释只说明「异步任务 + 按 `jobid` 查询结果」，无轮询间隔与任务有效期数值 ⇒ **未逐项核验** | 轮询节奏由宿主自定，勿在 SDK 侧加隐式重试 |
| `GetMeetingInfoResponse.title` 60 字节 vs 请求侧 40 字节 | 两侧上限来自各自注释，是否官方笔误**未逐项核验** | 写入校验按请求侧口径，读取展示按详情侧口径，勿合并放宽 |
| 代开发（Provider）面的能力全集 | MT2 只断言基础族代开发子接口为**零端点空标记**；其余十族官方代开发章节是否有页面**未逐项核验** | 官方补页须同批改 MT1/MT2 与本文，并考虑新增 `_Provider.cs` 子接口 |
| Webinar 与普通会议的互转/共享 ID 空间 | #44~#57 用同一 `meetingid` / `meeting_code` 口径，但「预约会议能否转网络研讨会」无注释依据 ⇒ **未逐项核验** | 不要据字段同名推断可互转；先核验官方页 |
| `meeting_code` / `meeting_link` 在基础面的可得性 | 注释明示「仅高级管理文档页声明」，但自建基础页是否实际回传**未逐项核验** | 展示入会入口时以详情接口取值为准，勿依赖创建应答 |
| 守卫注释与断言冲突（三处） | L95「59 条官方路由」、L96「全部 POST」、MT3 横幅「会议域 8 个接口」均与 `[Fact]` 断言（105 条 / 1 条 GET / 24 个接口）不一致 | 以断言为准；修注释属守卫文件改动，须与本文同批（本文不改代码） |
| [WechatModule.cs](../../Extensions/WechatModule.cs) 的 `Meeting` 备注 | 只写「预约会议基础管理族 + 会议统计管理族」两族；[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddMeetingApi()` XML 同样只述两族；[../../README.md](../../README.md) 的域清单条目亦只述两族 ⇒ 与守卫十一族**不一致** | 以 MT1/MT2 为准；补述三处 XML/README 时须同批，本文不改代码 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（300/86399 秒、40 字节/20 字符、100/300/2000/500/50/20/100 等上限、200 与 50 次重复上限、2023-12-01 口径） | 本目录 24 份接口文件的 XML 注释（remarks / returns） |
| 十一族路由表与逐族计数（4 / 2 / 1 / 19 / 17 / 14 / 3 / 12 / 4 / 15 / 10 / 5）、总路由 105、唯一 GET、无体端点参数形态 | MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`（横幅 L585，`[Fact]` L590 起；路由表 L99–L583；`HaveCount(105)` L860） |
| 接口层级 / 开放面收敛 / `IsAbstract` / 注册组 / `InheritedFrom` | MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`（横幅 L902，`[Fact]` L906；辅助 `AssertFamily` 于其下） |
| 24 个接口的 `TokenType` + Query + `access_token` | MT3 `MeetingTokenBinding_ShouldBeAccessTokenInjectedViaQuery`（横幅 L1065，`[Fact]` L1069，`HaveCount(24)`） |
| DTO 261 型全量登记 + `SerializerClassName = "Meeting"` + 逐族分解（17/3/55/32/28/10/33/6/35/31/11） | MT4 `MeetingDataModels_ShouldBeRegisteredInJsonContext`（横幅 L1115，`[Fact]` L1119，`HaveCount(261)` L1130） |
| 官方契约陷阱（共用 `MeetingSettings`/`MeetingReminders`/`MeetingInvitees`、`meetingid` 拼写、`remind_before` 数组、`repeat_until_type` 单值、会控单复数、Webinar 型名、Rooms/MRA 形态） | MT5 `MeetingDataModels_ShouldLockOfficialContractTraps`（横幅 L1297，`[Fact]` L1301）+ 守卫类头部注释（L19–L89） |
| 令牌归属键（Internal / Corp）与 Query 注入白名单 | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)；口径见 [AGENTS.md](../../../../../AGENTS.md) |
| 命名空间二分（父接口在 `.Interfaces`、可注入子接口在 `.Work`） | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work/DataModels/Meeting/](../../../Mud.Wechat.Work/DataModels/Meeting/)（平铺 261 型，无子目录） |
| 会议 JSON 上下文源生成文件 | [../../../Mud.Wechat.Work/DataModels/Generated/MeetingJsonContext.g.cs](../../DataModels/Generated/MeetingJsonContext.g.cs)（`internal`，示例不得引用） |
| 模块注册三段式与合并解析器登记 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（`MeetingJsonContext.Default` 在合并清单内） |
| 邻域边界（素材上传、tmp_external_userid 转换、日历/日程、会议室两棵树） | [../Media/IWechatWorkMediaService.cs](../Media/IWechatWorkMediaService.cs)、[../AccountId/TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService.cs](../AccountId/TmpExternalUserId/IWechatWorkAccountIdTmpExternalUserIdService.cs)、[../Schedule/Calendar/IWechatWorkScheduleCalendarService.cs](../Schedule/Calendar/IWechatWorkScheduleCalendarService.cs)、[../MeetingRoom/README.md](../MeetingRoom/README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与限制，**同批**改
   [WechatMeetingContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)
   对应的 `Meeting*Routes` 表（L99–L583 共十二张表）、该族 `HaveCount`、
   MT1 末尾 `allRoutes` 去重清单与 `HaveCount(105)`；改 HTTP 动词须同时改 MT1 的 `HttpAttribute` 断言
   （本域唯一 GET 是 `layout/list_template`，新增 GET 要显式写明理由）。
2. 改开放面（官方为高级族/统计族/Webinar 族补第三方或代开发文档页）→ 先核验，再新增
   `IWechatWorkThirdParty...` / `IWechatWorkProvider...` 子接口，并同批改 MT2 的 `AssertFamily` 参数
   （父声明面 0 → n、子接口集合）与 MT3 的 24 接口清单及 `HaveCount(24)`；
   命名空间二分（父在 `Mud.Wechat.Work.Interfaces`、可注入子接口在 `Mud.Wechat.Work`）由
   [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)
   全局约束，不要在本域开特例。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Meeting")]`（十一族共用同一个 `Meeting` 段）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 MT4 的类型清单与 `HaveCount(261)`（含 reason 里的逐族分解）与
   [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) 的合并解析器清单。
   契约陷阱类改动（改名、把 `string?` 改 `long`、把数组改单值）必须同步改 MT5，否则守卫失去意义。
4. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员与 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddMeetingApi()` XML
   （**当前只述两族，须在下次动本域时一并补全为十一族**）、可注入子接口的 `RegistryGroupName = "Meeting"`（父接口保持空组）。
5. 凭据面改动一律不得引入真值：`access_token`、`corpsecret`、`permanent_code`、`auth_code`、`suite_ticket`、
   `meeting_code`（会议号可视作入会凭据）、`host_key`、录制播放/下载地址 只写参数名，
   不得进日志、遥测或异常消息；新增敏感参数名须同批登记脱敏词表（`SensitiveUrlKeys.Register` / G7）。
   参会人 `userid`、嘉宾手机号与姓名示例一律用假值。
6. 本文与 [../../README.md](../../README.md) 的域内端点数（105 条路由 / 106 条声明 / 十一族）与族清单表述须同步更新
   （**Work 线 README 当前只列两族，属过期表述**）；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
