# MeetingRoom 域 · 会议室预定管理

企业微信「会议室预定管理」模块企业自建应用 SDK
（查询会议室预定信息 + 预定会议室 + 按日程预定 + 按会议预定 + 取消预定 + 获取预定详情，共 6 端点）。 接口：[IWechatWorkMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService.cs)、[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)、[IWechatWorkMeetingRoomService](./Room/IWechatWorkMeetingRoomService.cs)、[IWechatWorkInternalMeetingRoomService](./Room/IWechatWorkMeetingRoomService_Internal.cs)；注册组 `MeetingRoom`；模块枚举 `WechatModule.MeetingRoom`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatMeetingRoomContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingRoomContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/MeetingRoom`](../../../Mud.Wechat.Work/DataModels/MeetingRoom/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatWorkInternalMeetingRoomBookingService = 6（含继承）；按 HTTP 特性实计 **10** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalMeetingRoomBookingService` | `GetMeetingRoomBookingInfoAsync` | POST | `/cgi-bin/oa/meetingroom/get_booking_info` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询会议室预定信息 |
| 2 | `IWechatWorkInternalMeetingRoomBookingService` | `BookMeetingRoomAsync` | POST | `/cgi-bin/oa/meetingroom/book` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 预定会议室 |
| 3 | `IWechatWorkInternalMeetingRoomBookingService` | `BookMeetingRoomByScheduleAsync` | POST | `/cgi-bin/oa/meetingroom/book_by_schedule` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 按日程预定会议室 |
| 4 | `IWechatWorkInternalMeetingRoomBookingService` | `BookMeetingRoomByMeetingAsync` | POST | `/cgi-bin/oa/meetingroom/book_by_meeting` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 按会议预定会议室 |
| 5 | `IWechatWorkInternalMeetingRoomBookingService` | `CancelMeetingRoomBookAsync` | POST | `/cgi-bin/oa/meetingroom/cancel_book` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 取消会议室预定 |
| 6 | `IWechatWorkInternalMeetingRoomBookingService` | `GetMeetingRoomBookInfoAsync` | POST | `/cgi-bin/oa/meetingroom/bookinfo/get` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取会议室预定详情 |
| 7 | `IWechatWorkInternalMeetingRoomService` | `AddMeetingRoomAsync` | POST | `/cgi-bin/oa/meetingroom/add` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 添加会议室 |
| 8 | `IWechatWorkInternalMeetingRoomService` | `GetMeetingRoomListAsync` | POST | `/cgi-bin/oa/meetingroom/list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询会议室列表 |
| 9 | `IWechatWorkInternalMeetingRoomService` | `EditMeetingRoomAsync` | POST | `/cgi-bin/oa/meetingroom/edit` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 编辑会议室 |
| 10 | `IWechatWorkInternalMeetingRoomService` | `DeleteMeetingRoomAsync` | POST | `/cgi-bin/oa/meetingroom/del` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 删除会议室 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkMeetingRoomBookingService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalMeetingRoomBookingService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkMeetingRoomService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalMeetingRoomService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkMeetingRoomBookingService`

官方「会议室」文档树只在**企业自建应用开发**根下存在页面（93620 会议室预定管理，分类 93617），
第三方应用开发与服务商代开发两棵树均无对应页面 ⇒ 本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 6 个端点声明于 `IWechatWorkInternalMeetingRoomBookingService`
（形态对齐 `IWechatWorkMeetingRoomService` 零端点父接口）。

落位与形态对齐 `IWechatWorkDialService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

与「日程」域（`IWechatWorkScheduleService`）的日程排期是两套 ID 体系：本域排期内的
`schedule_id` / `meeting_id` 由会议室预定接口返回并以字符串透传，不经日程接口换取。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalMeetingRoomBookingService`

官方「会议室」文档树只在企业自建应用开发根下存在页面，本域端点全部声明于本接口
（形态对齐 `IWechatWorkInternalDialService`）。

消费应用自身 access_token（路由键 `AccessToken`）。

官方权限说明（93620「权限说明」一节原文）：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中，
代开发应用与第三方应用均为「暂不支持」——故本族只有唯一的自建子接口。
官方注：自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

官方频率：本族 6 个页面小节均无独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值
（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时；每 IP 不可超过 2 万次/分、60 万次/小时），
https://developer.work.weixin.qq.com/document/path/90312。

官方「会议室预定管理」页（2024-06-24 正文）只含上述 6 个小节：曾由第三方 SDK 转述的
`/cgi-bin/oa/meetingroom/get_booking_info_by_meeting_id` 已不在该页与「会议室」树任何节点内，
本域不声明该端点，由 RD1 零声明守卫持有（追踪号 P2-QY-MEETINGROOM-BYMEETINGID-07）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkMeetingRoomService`

官方「会议室」文档树只在**企业自建应用开发**根下存在页面（93619 会议室管理、
93620 会议室预定管理，分类 93617），第三方应用开发与服务商代开发两棵树均无对应页面
⇒ 本父接口没有公共端点，亦不设第三方 / 代开发子接口；全部 4 个端点声明于
`IWechatWorkInternalMeetingRoomService`
（形态对齐 `IWechatWorkDialService` 与 `IWechatWorkGovPatrolService` 零端点父接口）。

落位与形态对齐 `IWechatWorkDialService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

与「视频会议」域（`IWechatWorkMeetingService` 及其 MeetingRooms 会议室子接口，路由
`/cgi-bin/meeting/rooms/*`）分属官方两棵章节树：本域是 **OA 会议室**（实体会议室资源与预定），
后者是视频会议硬件会议室，路由 `/cgi-bin/oa/meetingroom/` 与 `/cgi-bin/meeting/` 互不承载。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalMeetingRoomService`

官方「会议室」文档树只在企业自建应用开发根下存在页面，本域 4 个端点全部声明于本接口
（形态对齐 `IWechatWorkInternalDialService` 与 `IWechatWorkInternalGovPatrolService`）。

消费应用自身 access_token（路由键 `AccessToken`）。

官方权限说明（93619「权限说明」一节原文）：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中，
代开发应用与第三方应用均为「暂不支持」——故本族只有唯一的自建子接口。
官方注：自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

官方频率：本族页面均无独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值
（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时；每 IP 不可超过 2 万次/分、60 万次/小时），
https://developer.work.weixin.qq.com/document/path/90312。

官方附录：会议室支持的设备 ID 为 1-电视 / 2-电话 / 3-投影 / 4-白板 / 5-视频。

与「视频会议」域的硬件会议室（`/cgi-bin/meeting/rooms/*`）分属官方两棵章节树，勿混淆。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93620
- （出现 10 次）官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。
- （出现 4 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93619

### 1. `GetMeetingRoomBookingInfoAsync` — 查询会议室预定信息

- **声明接口**：[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/get_booking_info`

- **请求体**：`GetMeetingRoomBookingInfoRequest` — 请求体（`GetMeetingRoomBookingInfoRequest`：meetingroom_id 与 city / building / floor 两种定位方式，start_time / end_time 限定时间窗）。
- **返回**：`GetMeetingRoomBookingInfoResponse` — 预定列表（官方 booking_list，元素为「会议室 ID + schedule 排期数组」，见 `MeetingRoomBooking`）。

**补充说明**

- 按会议室或按城市 / 楼宇 / 楼层与时间范围查询会议室的排期与预定状态。
- 官方限制：**不支持跨天查询**；start_time 默认为当前时间、end_time 默认为明日 0 时； 若按位置信息查询，须保证其上一级位置信息已填写（如用楼宇过滤必须同时填写城市）； 条目 status 官方口径为 0-已预定 / 2-申请中 / 3-审批中（本端点不回传 1-已取消）； schedule_id 在会议室已取消预定（未保留日历）的情况下无法再获取到日程详情。

**备注（原文）**

- 官方业务限制：本族 6 端点均只在企业自建应用文档树内开放； / 官方响应中的排期容器名为单数 `schedule` 但形态是数组，字段名照官方原文保留（见守卫 MR5）。

### 2. `BookMeetingRoomAsync` — 预定会议室

- **声明接口**：[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/book`

- **请求体**：`MeetingRoomBookRequest` — 请求体（`MeetingRoomBookRequest`：meetingroom_id / start_time / end_time / booker 必填，subject / attendees 可选）。
- **返回**：`MeetingRoomBookResponse` — 预定 ID（booking_id）与关联日程 ID（schedule_id）。

**补充说明**

- 以时间区间直接预定会议室并创建会议与关联日程。
- 官方限制：最小预定时长为 30 分钟；预定与结束时间会**自动按 30 分钟取整**（官方例：传入 15:15 与 15:45 会自动取整为 15:00 与 16:00）； 本端点**仅可预定无需审批的会议室**；当前时间已晚于预定时间时按三条规则处理——已过结束时间则不允许预定、 处于开始时间 15 分钟内则允许预定、超过开始时间 15 分钟则自动把开始时间顺延一个 30 分钟窗口。

**备注（原文）**

- 官方业务限制：booker 为预定者的成员账号（userid），attendees 为参与人的 userid 列表； / 与「按日程 / 按会议预定」的差异是本端点由会议室侧生成日程并回传 schedule_id。

### 3. `BookMeetingRoomByScheduleAsync` — 按日程预定会议室

- **声明接口**：[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/book_by_schedule`

- **请求体**：`MeetingRoomBookByScheduleRequest` — 请求体（`MeetingRoomBookByScheduleRequest`：meetingroom_id + schedule_id + booker）。
- **返回**：`MeetingRoomBookByScheduleResponse` — 预定 ID（booking_id）；冲突日期以官方 conflict_date 时间戳数组返回。

**补充说明**

- 以已存在的日程 ID 抢占预定会议室，支持重复日程预定。
- 官方限制：schedule_id **仅可使用同应用创建的日程**；本端点仅可预定无需审批的会议室； 预定后该日程将**不能**再通过更新日程接口编辑，只能调用新增 / 删除日程参与者接口； 若需更新日程时间等字段，须先取消会议室预定、再更新日程、之后重新预定。

**备注（原文）**

- 官方业务限制：官方把本端点归为「通过日程预定会议室」，schedule_id 取自建日程接口创建的日程 ID； / 响应字段面与 `BookMeetingRoomAsync` 不同：本端点回传 booking_id 与 conflict_date（当天 0 点时间戳数组， / 重复日程部分日期与既有预定冲突时返回），不回传 schedule_id。

### 4. `BookMeetingRoomByMeetingAsync` — 按会议预定会议室

- **声明接口**：[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/book_by_meeting`

- **请求体**：`MeetingRoomBookByMeetingRequest` — 请求体（`MeetingRoomBookByMeetingRequest`：meetingroom_id + meetingid + booker）。
- **返回**：`MeetingRoomBookByMeetingResponse` — 预定 ID（booking_id）；冲突日期以官方 conflict_date 时间戳数组返回。

**补充说明**

- 把已创建的会议挂到会议室上（复用会议已有信息生成预定），支持重复会议预定。
- 官方限制：meetingid **仅可使用同应用创建的会议**；本端点仅可预定无需审批的会议室； 冲突语义与按日程侧一致（conflict_date 为当天 0 点时间戳数组）。

**备注（原文）**

- 官方业务限制：**官方契约陷阱**：本端点的会议 ID 请求字段官方拼写为 `meetingid`（无下划线）， / 与同域其余端点的 `meeting_id` 不同名，字段名照官方原文保留（见守卫 MR5）。

### 5. `CancelMeetingRoomBookAsync` — 取消会议室预定

- **声明接口**：[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/cancel_book`

- **请求体**：`MeetingRoomCancelBookRequest` — 请求体（`MeetingRoomCancelBookRequest`：booking_id 必填，keep_schedule / cancel_date 可选）。
- **返回**：`WechatWorkResponse` — 官方成功响应仅有 errcode / errmsg，无业务字段。

**补充说明**

- 按预定 ID 取消会议室预定，并可选择是否保留关联日程、只取消重复预定中的某一天。
- 官方限制：booking_id 必填（官方口径为「会议室的预定 id」，**不是**会议 ID）； keep_schedule 为 0-同步删除 / 1-保留，且**仅对非重复日程有效**； cancel_date 针对重复日程——不填表示取消所有重复预定，填写（当天 0 点时间戳）表示仅取消对应日期当天的预定。

**备注（原文）**

- 官方业务限制：官方响应体不含业务字段，故返回 `WechatWorkResponse`，不另建响应 DTO； / 取消后该预定的 schedule_id 若未保留日历，将无法再经日程接口取到详情。

### 6. `GetMeetingRoomBookInfoAsync` — 获取会议室预定详情

- **声明接口**：[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/bookinfo/get`

- **请求体**：`MeetingRoomBookInfoGetRequest` — 请求体（`MeetingRoomBookInfoGetRequest`：meetingroom_id + booking_id）。
- **返回**：`MeetingRoomBookInfoGetResponse` — 会议室 ID 与预定排期（官方 schedule 对象形态，比查询侧多回传 booking_id / master_booking_id）。

**补充说明**

- 按预定 ID 查询该预定的会议室与排期信息。
- 官方限制：meetingroom_id 与 booking_id 均必填；本端点条目的 status 官方口径比查询侧多一档—— 0-已预定 / 1-已取消 / 2-申请中 / 3-审批中；附录另述审批流转（无需审批：申请即已预定； 需审批：申请中 → 提交审批后审批中 → 审批完成后已预定，再取消为已取消）。

**备注（原文）**

- 官方业务限制：路由为 `bookinfo/get`（斜杠分层），与本域其余端点的平级路由形态不同； / 官方以 master_booking_id 回传主预定 ID（该预定是某个周期性预定的一部分时返回，字段面见 `MeetingRoomSchedule`）。

### 7. `AddMeetingRoomAsync` — 添加会议室

- **声明接口**：[IWechatWorkInternalMeetingRoomService](./Room/IWechatWorkMeetingRoomService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/add`

- **请求体**：`MeetingRoomAddRequest` — 请求体（`MeetingRoomAddRequest`：name / capacity 为必填项，city / building / floor / coordinate / range / equipment 可缺省）。
- **返回**：`MeetingRoomAddResponse` — 新建会议室的 ID（官方 meetingroom_id，后续编辑、删除与预定均以该值定位）。

**补充说明**

- 创建一个实体会议室（名称、容纳人数、位置、使用范围与支持的设备）。
- 官方限制：name 最多 30 个字符，capacity 为可容纳人数；range.user_list 最多指定 1000 个成员、 range.department_list 最多指定 1000 个部门，**不填 range 则默认为全公司可用**； 若要设置位置信息，必须同时填写 city、building 与 floor 三个参数。

**备注（原文）**

- 官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）； / 官方以 range 的 user_list / department_list 表达会议室使用范围（成员 UserId 与部门 ID）， / coordinate 为所在建筑经纬度（官方建议经腾讯地图坐标拾取器获取）、equipment 为支持的设备 ID 列表。

### 8. `GetMeetingRoomListAsync` — 查询会议室列表

- **声明接口**：[IWechatWorkInternalMeetingRoomService](./Room/IWechatWorkMeetingRoomService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/list`

- **请求体**：`MeetingRoomListRequest` — 请求体（`MeetingRoomListRequest`：city / building / floor / equipment 均为可选过滤项）。
- **返回**：`MeetingRoomListResponse` — 会议室列表（官方 meetingroom_list，元素结构见 `MeetingRoomInfo`，比添加侧多回传 need_approval）。

**补充说明**

- 按城市、楼宇、楼层与设备 ID 过滤查询会议室列表。
- 官方限制：使用位置信息过滤时须保证其上一级已填写（如用楼宇过滤必须同时填写城市）； 官方无分页参数，请求体即四个过滤项。

**备注（原文）**

- 官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）； / city / building / floor / equipment 四个过滤项均可缺省； / 列表元素比添加侧多回传 need_approval（0-无需审批 / 1-需要审批）； / 官方注明 range 的 user_list / department_list **仅会议室系统应用查询时返回**。

### 9. `EditMeetingRoomAsync` — 编辑会议室

- **声明接口**：[IWechatWorkInternalMeetingRoomService](./Room/IWechatWorkMeetingRoomService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/edit`

- **请求体**：`MeetingRoomEditRequest` — 请求体（`MeetingRoomEditRequest`：meetingroom_id 必填，其余字段为可改项）。
- **返回**：`WechatWorkResponse` — 官方成功响应仅有 errcode / errmsg，无业务字段。

**补充说明**

- 按会议室 ID 修改其名称、容纳人数、位置、使用范围与支持的设备。
- 官方限制：**会议室使用范围（range）为覆盖操作而非增量添加**， user_list / department_list 各最多 1000 项且填写后即覆盖整个使用范围； 修改位置信息须同时输入城市、楼宇与楼层三个参数，**已生成的建筑暂不支持修改经纬度**； 除 meetingroom_id 外其余参数官方均标为可选（不传即不改）。

**备注（原文）**

- 官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）； / 官方响应体不含业务字段，故返回 `WechatWorkResponse`，不另建响应 DTO。

### 10. `DeleteMeetingRoomAsync` — 删除会议室

- **声明接口**：[IWechatWorkInternalMeetingRoomService](./Room/IWechatWorkMeetingRoomService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/meetingroom/del`

- **请求体**：`MeetingRoomDeleteRequest` — 请求体（`MeetingRoomDeleteRequest`：仅 meetingroom_id）。
- **返回**：`WechatWorkResponse` — 官方成功响应仅有 errcode / errmsg，无业务字段。

**补充说明**

- 按会议室 ID 删除会议室。
- 官方限制：请求侧仅 meetingroom_id 一个业务参数（官方标为必填），无其他约束。

**备注（原文）**

- 官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）； / 官方响应体不含业务字段，故返回 `WechatWorkResponse`，不另建响应 DTO。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/MeetingRoom`](../../../Mud.Wechat.Work/DataModels/MeetingRoom/) 内共 **22** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **22** 个，`SerializerClassName` = `MeetingRoom`。

AOT 源生成上下文：[`MeetingRoomJsonContext.g.cs`](./../../DataModels/Generated/MeetingRoomJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetMeetingRoomBookingInfoResponse` | 应答 | [MeetingRoom/Booking/GetMeetingRoomBookingInfoResponse.cs](../../DataModels/MeetingRoom/Booking/GetMeetingRoomBookingInfoResponse.cs) | `booking_list` |
| `GetMeetingRoomBookingInfoRequest` | 请求体 | [MeetingRoom/Booking/GetMeetingRoomBookingInfoRequest.cs](../../DataModels/MeetingRoom/Booking/GetMeetingRoomBookingInfoRequest.cs) | `meetingroom_id`, `start_time`, `end_time`, `city`, `building`, `floor` |
| `MeetingRoomBookResponse` | 应答 | [MeetingRoom/Booking/MeetingRoomBookResponse.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookResponse.cs) | `booking_id`, `schedule_id` |
| `MeetingRoomBookRequest` | 请求体 | [MeetingRoom/Booking/MeetingRoomBookRequest.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookRequest.cs) | `meetingroom_id`, `subject`, `start_time`, `end_time`, `booker`, `attendees` |
| `MeetingRoomBookByScheduleResponse` | 应答 | [MeetingRoom/Booking/MeetingRoomBookByScheduleResponse.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookByScheduleResponse.cs) | `booking_id`, `conflict_date` |
| `MeetingRoomBookByScheduleRequest` | 请求体 | [MeetingRoom/Booking/MeetingRoomBookByScheduleRequest.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookByScheduleRequest.cs) | `meetingroom_id`, `schedule_id`, `booker` |
| `MeetingRoomBookByMeetingResponse` | 应答 | [MeetingRoom/Booking/MeetingRoomBookByMeetingResponse.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookByMeetingResponse.cs) | `booking_id`, `conflict_date` |
| `MeetingRoomBookByMeetingRequest` | 请求体 | [MeetingRoom/Booking/MeetingRoomBookByMeetingRequest.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookByMeetingRequest.cs) | `meetingroom_id`, `meetingid`, `booker` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `MeetingRoomCancelBookRequest` | 请求体 | [MeetingRoom/Booking/MeetingRoomCancelBookRequest.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomCancelBookRequest.cs) | `booking_id`, `keep_schedule`, `cancel_date` |
| `MeetingRoomBookInfoGetResponse` | 应答 | [MeetingRoom/Booking/MeetingRoomBookInfoGetResponse.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookInfoGetResponse.cs) | `meetingroom_id`, `schedule` |
| `MeetingRoomBookInfoGetRequest` | 请求体 | [MeetingRoom/Booking/MeetingRoomBookInfoGetRequest.cs](../../DataModels/MeetingRoom/Booking/MeetingRoomBookInfoGetRequest.cs) | `meetingroom_id`, `booking_id` |
| `MeetingRoomAddResponse` | 应答 | [MeetingRoom/Room/MeetingRoomAddResponse.cs](../../DataModels/MeetingRoom/Room/MeetingRoomAddResponse.cs) | `meetingroom_id` |
| `MeetingRoomAddRequest` | 请求体 | [MeetingRoom/Room/MeetingRoomAddRequest.cs](../../DataModels/MeetingRoom/Room/MeetingRoomAddRequest.cs) | `name`, `capacity`, `city`, `building`, `floor`, `coordinate`, `range`, `equipment` |
| `MeetingRoomListResponse` | 应答 | [MeetingRoom/Room/MeetingRoomListResponse.cs](../../DataModels/MeetingRoom/Room/MeetingRoomListResponse.cs) | `meetingroom_list` |
| `MeetingRoomListRequest` | 请求体 | [MeetingRoom/Room/MeetingRoomListRequest.cs](../../DataModels/MeetingRoom/Room/MeetingRoomListRequest.cs) | `city`, `building`, `floor`, `equipment` |
| `MeetingRoomEditRequest` | 请求体 | [MeetingRoom/Room/MeetingRoomEditRequest.cs](../../DataModels/MeetingRoom/Room/MeetingRoomEditRequest.cs) | `meetingroom_id`, `name`, `capacity`, `city`, `building`, `floor`, `coordinate`, `range`, `equipment` |
| `MeetingRoomDeleteRequest` | 请求体 | [MeetingRoom/Room/MeetingRoomDeleteRequest.cs](../../DataModels/MeetingRoom/Room/MeetingRoomDeleteRequest.cs) | `meetingroom_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.MeetingRoom;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMeetingRoomApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalMeetingRoomService>();
var ct = CancellationToken.None;
var response = await service.AddMeetingRoomAsync(new MeetingRoomAddRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/oa/meetingroom/add");
// 占位值仅用于编译示意；业务值须按 IWechatWorkMeetingRoomService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddMeetingRoomAsync`）；请求体 `MeetingRoomAddRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService.cs)、[IWechatWorkInternalMeetingRoomBookingService](./Booking/IWechatWorkMeetingRoomBookingService_Internal.cs)、[IWechatWorkMeetingRoomService](./Room/IWechatWorkMeetingRoomService.cs)、[IWechatWorkInternalMeetingRoomService](./Room/IWechatWorkMeetingRoomService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatMeetingRoomContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingRoomContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatNonContractRouteZeroDeclarationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs) |
| 守卫中的代表性测试方法 | `MeetingRoomRoomEndpoints_ShouldMatchOfficialRoutes`、`MeetingRoomBookingEndpoints_ShouldMatchOfficialRoutes`、`MeetingRoomSingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent`、`MeetingRoomTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`MeetingRoomDataModels_ShouldBeRegisteredInJsonContext`、`MeetingRoomOfficialFieldNames_ShouldBePreservedVerbatim`、`MeetingRoomNumericalBooleanFields_ShouldBeCarriedAsNumbers`、`MeetingRoomOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces`、`NonContractRoutes_ShouldHaveZeroDeclaredRoutes_WhenScannedAcrossSrc` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/MeetingRoom`](../../../Mud.Wechat.Work/DataModels/MeetingRoom/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatMeetingRoomContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatNonContractRouteZeroDeclarationContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
