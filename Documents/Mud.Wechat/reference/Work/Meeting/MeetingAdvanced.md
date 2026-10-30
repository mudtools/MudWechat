# 预约会议高级管理 · 受邀成员、等候室、实时成员、健康度与报名

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 预约会议高级管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingAdvancedService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingAdvancedService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingAdvancedService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingAdvancedService_Internal.cs)（**唯一**子接口，承载全部 19 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`Get/SetMeetingInvitees*`、`MeetingInvitee`、`CreateMeetingCustomerShortUrl*`、`GetMeetingCustomerShortUrl*`、`MeetingCustomerShortUrl`、`GetMeetingRealtimeAttendeeList*`、`MeetingRealtimeAttendee`、`GetMeetingAttendeeList*`、`MeetingAttendedAttendee`、`GetMeetingWaitingRoom*`、`MeetingWaitingRoomCurrentUser`、`MeetingWaitingRoomUserRecord`、`CheckMeetingDeviceInMeeting*`、`MeetingDeviceCheckResult`、`Get/SetMeetingGuests*`、`MeetingGuest`、`GetMeetingQuality*`、`MeetingQualityAttendee` 与报名七支的 `*MeetingEnroll*` + `MeetingEnroll*` 系列；本族在 MT4 的逐族分解中计 55 型，是本域最大的一族） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(19)`、`enroll/` 七支与 `waitingroom/` 两支子段单独断言 / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 19)` / MT3 令牌绑定 / MT4 上下文登记 / MT5「受邀成员两种同名不同形 `MeetingInvitees` 与 `MeetingInvitee`」「`enroll_id_list` 同名不同形」）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 受邀成员 [98160](https://developer.work.weixin.qq.com/document/path/98160) / 98162 · 专属链接 98818 / 98819 · 实时成员 98157 · 参会列表 98156 · 等候室 98163 / 98164 · 设备检查 98165 · 嘉宾 99039 / 99040 · 健康度 98821 · 报名 98797 / 98800 / 98794 / 98810 / 98807 / 98816 / 98817（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（受邀成员扩容与翻页 → 专属链接 → 实时/历史成员 → 等候室 → 设备检查 → 嘉宾 → 健康度 → 报名七支闭环））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 / MT5 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 1/2/3/6/8/9/10 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

预约会议的**高级面**（基础族 [Meeting.md](./Meeting.md) 之外的能力），19 支端点分五组：

| 组 | # | 方法 | 路由 |
|---|---|---|---|
| 受邀成员 | E1 | `GetInviteesAsync` | `meeting/get_invitees` |
| | E2 | `SetInviteesAsync` | `meeting/set_invitees` |
| 专属参会链接 | E3 | `CreateCustomerShortUrlAsync` | `meeting/create_customer_short_url` |
| | E4 | `GetCustomerShortUrlAsync` | `meeting/get_customer_short_url` |
| 成员与设备 | E5 | `GetRealtimeAttendeeListAsync` | `meeting/get_realtime_attendee_list` |
| | E6 | `GetAttendeeListAsync` | `meeting/get_attendee_list` |
| | E7 | `GetWaitingRoomCurrentUserListAsync` | `meeting/waitingroom/get_current_user_list` |
| | E8 | `GetWaitingRoomUserListAsync` | `meeting/waitingroom/get_user_list` |
| | E9 | `CheckDeviceInMeetingAsync` | `meeting/check_device_in_meeting` |
| | E12 | `GetQualityAsync` | `meeting/get_quality` |
| 嘉宾 | E10 | `GetGuestsAsync` | `meeting/get_guests` |
| | E11 | `SetGuestsAsync` | `meeting/set_guests` |
| 报名七件套 | E13 | `SetEnrollConfigAsync` | `meeting/enroll/set_config` |
| | E14 | `GetEnrollConfigAsync` | `meeting/enroll/get_config` |
| | E15 | `QueryEnrollIdsAsync` | `meeting/enroll/query_by_tmp_openid` |
| | E16 | `ListEnrollsAsync` | `meeting/enroll/list` |
| | E17 | `ApproveEnrollsAsync` | `meeting/enroll/approve` |
| | E18 | `ImportEnrollsAsync` | `meeting/enroll/import` |
| | E19 | `DeleteEnrollsAsync` | `meeting/enroll/delete` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）。

**本族是「高级管理文档页」那一侧的落位**：域 README 第 2 节把「基础页 vs 高级页」声明的字段差异列了出来——`guests`、`meeting_code`、`meeting_link`、`sub_meetingid`、`meeting_type`、`has_vote` 等**只在高级页出现**，基础创建应答不承诺 ⇒ 想要这些能力就在本族取。

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

- 自建应用须配置在「**可调用接口的应用**」列表中；本族逐端点标注「**仅允许获取 / 修改 / 操作该应用创建的会议的数据**」，多数读侧还加「**仅返回在应用可见范围内的成员**」。
- **容量与身份**：E2 受邀成员**最多 2000 人**（与企业所购在线会议室最大方数相关），**管理员必须在受邀成员列表中**；E15 `tmp_openid_list` **单次 ≤500**；E16/E5 的 `limit` 最大 **50**（E5 默认值另见 §4；E16 默认 50）。
- **不支持的对象**：E3 **不支持网络研讨会（Webinar）**；E4 **不支持个人会议号会议与 Webinar**；`customer_data` **≤256 字节**且须为 `{"ver":"1.0","userData":"自定义字段"}` 结构 **Base64** 编码。
- **等候室闸门**：E7 要求会议**已开启等候室且处于「会议进行中」**，否则返回空列表。
- **周期性会议**：E5 与 E12 对周期性会议**必传 `sub_meetingid`**（可由详情应答的 `current_sub_meetingid` 取得）；E6 注释相反——**对周期性会议查询暂时不生效，请使用分页参数查询**。
- **报名闸门**：E13/E14/E18 均要求**会议已开启报名**（未开启时返回「未开启报名错误」）；问题**最多 8 个**、标题 **≤40 字符**、每选项 **≤40 个汉字**、最多 8 个选项。
- 健康度（E12）可查区间为**过去 7 天到现在**，`limit` 默认与最大均 **50**。
- **个人数据面最宽的一族**：成员 userid、临时 ID、入退会时间、麦克风/摄像头/共享状态、网络类型、健康度、嘉宾手机号、报名答案与昵称 ⇒ **一律示例假值、不得整体落日志**；`enroll_code`（电话入会凭证）与专属链接是可被使用的入会入口，不得回显。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E19 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingAdvancedService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingAdvancedService`） |
| 不可注入 | `IWechatWorkMeetingAdvancedService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetInviteesAsync` — 获取受邀成员

**签名**

```csharp
[Post("/cgi-bin/meeting/get_invitees")]
Task<GetMeetingInviteesResponse> GetInviteesAsync(
    [Body] GetMeetingInviteesRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/get_invitees` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/get_invitees?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `GetMeetingInviteesRequest`：`meetingid`（官方必填）+ `cursor`（首次可不传） |

**返回** `GetMeetingInviteesResponse`：`has_more`（`bool?`）+ `next_cursor` + `invitees`（**`List<MeetingInvitee>?`——对象数组，每项只有 `userid`**）。

> **与创建/修改请求的 `invitees` 形态不同**（MT5）：那里是 `MeetingInvitees{userid: List<string>}`（**一个对象包数组**），这里是**对象数组** ⇒ 两者不可互换。本端点**无 `limit` 参数**。

**调用示例**（扩容受邀成员 → 生成专属链接 → 会中取实时成员与会控身份 → 等候室 → 健康度 → 报名闭环）

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
var advanced = provider.GetRequiredService<IWechatWorkInternalMeetingAdvancedService>();

const string meetingid = "demo-meeting-id";        // 假值；须为本应用创建的会议
const string adminUserid = "demo_user_0001";

// ④ E2 扩容受邀成员：最多 2000 人，且管理员必须在列表内（超过创建面 100/300 的官方规定形态）
var invited = await advanced.SetInviteesAsync(new SetMeetingInviteesRequest
{
    Meetingid = meetingid,
    Invitees = new List<MeetingInvitee>
    {
        new MeetingInvitee { Userid = adminUserid },     // 管理员必须在列表内
        new MeetingInvitee { Userid = "demo_user_0002" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(invited, "/cgi-bin/meeting/set_invitees");

// ⑤ E1 读回：游标翻页；invitees 是对象数组
string? inviteeCursor = null;
long inviteeCount = 0;
while (true)
{
    var page = await advanced.GetInviteesAsync(new GetMeetingInviteesRequest
    {
        Meetingid = meetingid,
        Cursor = inviteeCursor,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/meeting/get_invitees");
    inviteeCount += page.Invitees?.Count ?? 0;
    if (page.HasMore != true) break;
    inviteeCursor = page.NextCursor;
}
logger.LogInformation("受邀成员共 {Count} 人（明细不落日志）", inviteeCount);

// ⑥ E3 创建专属参会链接：不支持 Webinar；customer_data ≤256 字节且为 Base64 的 {"ver":"1.0","userData":"..."}
var shortUrl = await advanced.CreateCustomerShortUrlAsync(new CreateMeetingCustomerShortUrlRequest
{
    Meetingid = meetingid,
    CustomerData = "eyJ2ZXIiOiIxLjAiLCJ1c2VyRGF0YSI6ImRlbW8ifQ==",   // 占位示例
}, cancellationToken);
WechatWorkException.ThrowIfFailed(shortUrl, "/cgi-bin/meeting/create_customer_short_url");
// meeting_short_url 是入会入口：不打印、不回显给非当事方
bool issued = shortUrl.MeetingShortUrlCustomerData?.MeetingShortUrl is not null;
logger.LogInformation("专属链接是否已生成：{Issued}", issued);

// ⑦ E4 查全部专属链接（不支持个人会议号会议与 Webinar）
var urls = await advanced.GetCustomerShortUrlAsync(
    new GetMeetingCustomerShortUrlRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(urls, "/cgi-bin/meeting/get_customer_short_url");
logger.LogInformation("专属链接条目数 {Count}", urls.MeetingShortUrlCustomerDataList?.Count ?? 0);

// ⑧ E5 实时会中成员：limit ≤50 且必须与首次取 cursor 时的 limit 一致；周期性会议必传 sub_meetingid
var realtime = await advanced.GetRealtimeAttendeeListAsync(new GetMeetingRealtimeAttendeeListRequest
{
    Meetingid = meetingid,
    Limit = 50,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(realtime, "/cgi-bin/meeting/get_realtime_attendee_list");
// tmp_openid + instance_id 就是会控（MeetingControl 族）的被操作者定位键
foreach (MeetingRealtimeAttendee attendee in realtime.Attendees ?? new List<MeetingRealtimeAttendee>())
{
    logger.LogInformation("成员角色 {Role}、设备类型 {InstanceId}（不落 userid 与 tmp_openid）",
        attendee.Role, attendee.InstanceId);
}

// ⑨ E6 历史参会列表：区间 ≤31 天；两者都不传时最大跨度 90 天；对周期性会议不生效（改用分页）
var attended = await advanced.GetAttendeeListAsync(new GetMeetingAttendeeListRequest
{
    Meetingid = meetingid,
    StartTime = DateTimeOffset.Now.AddDays(-7).ToUnixTimeSeconds(),
    EndTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
    Limit = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(attended, "/cgi-bin/meeting/get_attendee_list");
logger.LogInformation("历史参会 {Count} 条", attended.Attendees?.Count ?? 0);

// ⑩ E7 / E8 等候室：E7 需已开启等候室且会议进行中；两支的 limit 都必须与首次一致
var waitingNow = await advanced.GetWaitingRoomCurrentUserListAsync(new GetMeetingWaitingRoomCurrentUserListRequest
{
    Meetingid = meetingid,
    Limit = 10,                 // 默认 10、最大 50
}, cancellationToken);
WechatWorkException.ThrowIfFailed(waitingNow, "/cgi-bin/meeting/waitingroom/get_current_user_list");

var waitingHistory = await advanced.GetWaitingRoomUserListAsync(new GetMeetingWaitingRoomUserListRequest
{
    Meetingid = meetingid,
    Limit = 20,                 // 默认 20、最大 50；join/quit_time 单位毫秒
}, cancellationToken);
WechatWorkException.ThrowIfFailed(waitingHistory, "/cgi-bin/meeting/waitingroom/get_user_list");

// ⑪ E9 检查设备是否在会中：meetingid_list 须为本企业创建的会议；instance_id_list 不传即查全部设备
var checkedDevices = await advanced.CheckDeviceInMeetingAsync(new CheckMeetingDeviceInMeetingRequest
{
    Userid = adminUserid,
    MeetingidList = new List<string> { meetingid },
    InstanceIdList = new List<int> { 1, 4 },    // 1 PC / 4 iOS
}, cancellationToken);
WechatWorkException.ThrowIfFailed(checkedDevices, "/cgi-bin/meeting/check_device_in_meeting");
logger.LogInformation("已入会设备 {Count} 台", checkedDevices.ResultList?.Count ?? 0);

// ⑫ E11 / E10 嘉宾：覆盖式更新（先读后写，避免误清空）
var guestsGet = await advanced.GetGuestsAsync(new GetMeetingGuestsRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(guestsGet, "/cgi-bin/meeting/get_guests");

var guestsSet = await advanced.SetGuestsAsync(new SetMeetingGuestsRequest
{
    Meetingid = meetingid,
    Guests = new List<MeetingGuest>
    {
        // area 中国传 "86"（不是 +86 / 0086）；guest_name 1~16 位
        new MeetingGuest { Area = "86", PhoneNumber = "13800000000", GuestName = "示例嘉宾" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(guestsSet, "/cgi-bin/meeting/set_guests");

// ⑬ E12 会议健康度：start_time 必填（可查区间为过去 7 天到现在）；limit 默认与最大 50
var quality = await advanced.GetQualityAsync(new GetMeetingQualityRequest
{
    Meetingid = meetingid,
    StartTime = (int)DateTimeOffset.Now.AddHours(-2).ToUnixTimeSeconds(),   // 官方用 int 承载秒级时间戳
    Limit = 50,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(quality, "/cgi-bin/meeting/get_quality");
// quality：0 无数据 / 1 健康 / 2 告警；problems 是告警项列表
logger.LogInformation("会议健康度 {Quality}、告警项 {ProblemCount} 条、成员明细 {AttendeeCount} 条",
    quality.Quality, quality.Problems?.Count ?? 0, quality.Attendees?.Count ?? 0);

// ⑭ E13~E19 报名闭环
var configSet = await advanced.SetEnrollConfigAsync(new SetMeetingEnrollConfigRequest
{
    Meetingid = meetingid,
    ApproveType = 2,                 // 1 自动审批（默认）/ 2 手动审批
    IsCollectQuestion = 2,           // 1 不收集（默认）/ 2 收集
    QuestionList = new List<MeetingEnrollQuestion>
    {
        // ≤8 个问题；标题 ≤40 字符；单选/多选每选项 ≤40 汉字、最多 8 个
        new MeetingEnrollQuestion
        {
            QuestionTitle = "你关注的议题",
            QuestionType = 1,                     // 1 单选 / 2 多选 / 3 简答
            IsRequired = 2,                       // 1 否 / 2 是（注意不是 0/1）
            OptionList = new List<MeetingEnrollQuestionOption>
            {
                new MeetingEnrollQuestionOption { Content = "议题A" },
                new MeetingEnrollQuestionOption { Content = "议题B" },
            },
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(configSet, "/cgi-bin/meeting/enroll/set_config");

var configGet = await advanced.GetEnrollConfigAsync(
    new GetMeetingEnrollConfigRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(configGet, "/cgi-bin/meeting/enroll/get_config");

// E18 导入报名（手机号或 userid 两种身份）
var imported = await advanced.ImportEnrollsAsync(new ImportMeetingEnrollsRequest
{
    Meetingid = meetingid,
    EnrollList = new List<MeetingEnrollImportItem>
    {
        new MeetingEnrollImportItem { Area = "86", PhoneNumber = "13800000001" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(imported, "/cgi-bin/meeting/enroll/import");
logger.LogInformation("导入成功 {TotalCount} 条", imported.TotalCount);

// E16 列表（limit ≤50、默认 50）→ E17 审批（enroll_id_list 是字符串数组）→ E19 删除（对象数组）
var enrolls = await advanced.ListEnrollsAsync(new ListMeetingEnrollsRequest
{
    Meetingid = meetingid,
    Status = 1,                 // 0 全部 / 1 待审批 / 2 已拒绝 / 3 已批准
    Limit = 50,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(enrolls, "/cgi-bin/meeting/enroll/list");

string? enrollId = (enrolls.EnrollList ?? new List<MeetingEnrollInfo>())
    .Select(e => e.EnrollId).FirstOrDefault(id => id is not null);
if (enrollId is not null)
{
    var approved = await advanced.ApproveEnrollsAsync(new ApproveMeetingEnrollsRequest
    {
        Meetingid = meetingid,
        EnrollIdList = new List<string> { enrollId },
        Action = 3,              // 1 取消批准（回到待审批）/ 2 拒绝 / 3 批准
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(approved, "/cgi-bin/meeting/enroll/approve");

    // E15 用 tmp_openid 反查报名 ID：单次 ≤500；无人报名则不返回该字段
    var ids = await advanced.QueryEnrollIdsAsync(new QueryMeetingEnrollIdsRequest
    {
        Meetingid = meetingid,
        TmpOpenidList = new List<string> { "demo-tmp-openid" },
        SortingRules = 1,        // 1 优先手机号导入（默认）/ 2 优先手动报名
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(ids, "/cgi-bin/meeting/enroll/query_by_tmp_openid");

    var deleted = await advanced.DeleteEnrollsAsync(new DeleteMeetingEnrollsRequest
    {
        Meetingid = meetingid,
        EnrollIdList = new List<MeetingEnrollIdRef> { new MeetingEnrollIdRef { EnrollId = enrollId } },
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/meeting/enroll/delete");
}
```

### E2. `SetInviteesAsync` — 更新受邀成员列表

```csharp
[Post("/cgi-bin/meeting/set_invitees")]
Task<WechatWorkResponse> SetInviteesAsync(
    [Body] SetMeetingInviteesRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）+ `invitees`（`List<MeetingInvitee>?`，**最大 2000 人**、与企业所购在线会议室最大方数相关、**管理员必须在列表中**）。**返回**：`WechatWorkResponse`。

> 这是官方规定的「**创建时 ≤300 人、更多人群改走本端点**」的扩容路径（域 README 第 2 节）⇒ 不是可选优化。

### E3. `CreateCustomerShortUrlAsync` — 创建用户专属参会链接

```csharp
[Post("/cgi-bin/meeting/create_customer_short_url")]
Task<CreateMeetingCustomerShortUrlResponse> CreateCustomerShortUrlAsync(
    [Body] CreateMeetingCustomerShortUrlRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `customer_data`（**均官方必填**）。

`customer_data` **≤256 字节**，且须为 `{"ver":"1.0","userData":"自定义字段"}` 结构的 **Base64 编码**——本仓不校验该结构，宿主自行保证。

**返回**：`meeting_short_url_customer_data`（`MeetingCustomerShortUrl`：`meeting_short_url` 用户专属参会链接 + `customer_data` **原样回显**）。

**限制与用法**：**该接口不支持网络研讨会（Webinar）**；`customer_data` 可经「用户入会 / 用户进入等候室」事件与 **E7** 等候室当前成员列表读回（用于把入会行为归因到渠道）。链接是入会入口 ⇒ **不回显给无关方、不落日志**。

### E4. `GetCustomerShortUrlAsync` — 获取用户专属参会链接列表

```csharp
[Post("/cgi-bin/meeting/get_customer_short_url")]
Task<GetMeetingCustomerShortUrlResponse> GetCustomerShortUrlAsync(
    [Body] GetMeetingCustomerShortUrlRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）。**返回**：`meeting_short_url_customer_data_list[]`（`MeetingCustomerShortUrl`，同 E3）。

**限制**：**该接口不支持个人会议号会议与网络研讨会（Webinar）**——比 E3 多一条限制（个人会议号即 `meeting_type = 5`）。
### E5. `GetRealtimeAttendeeListAsync` — 获取实时会中成员列表

```csharp
[Post("/cgi-bin/meeting/get_realtime_attendee_list")]
Task<GetMeetingRealtimeAttendeeListResponse> GetRealtimeAttendeeListAsync(
    [Body] GetMeetingRealtimeAttendeeListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（必填）+ **`sub_meetingid`（周期性会议必传，取自详情应答的 `current_sub_meetingid`）** + `cursor` + `limit`（**最大 50**，**必须与首次取 `cursor` 时一致**）。

**返回**：`has_more` + `next_cursor` + `attendees[]`（`MeetingRealtimeAttendee`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid` | `Userid` | `string?` | 企业内参会成员 userid |
| `tmp_openid` | `TmpOpenid` | `string?` | **会中临时 ID——会控（MeetingControl 族）的被操作者定位键** |
| `join_time` | `JoinTime` | **`string?`** | 加入会议时间戳（**单位秒、字符串形态**） |
| `instance_id` | `InstanceId` | `int?` | 终端设备类型：0 PSTN / 1 PC / 2 Mac / 3 Android / 4 iOS / 5 Web / 6 iPad / 7 Android Pad / 8 小程序 / **9 voip·sip（MRA）** / 10 linux / 20·21 Rooms for Touch |
| `role` | `Role` | `int?` | 0 普通 / 1 创建者 / 2 主持人 / 3 创建者+主持人 / **4 游客** / 5 游客+主持人 / 6 联席主持人 / 7 创建者+联席主持人 |
| `join_type` | `JoinType` | `int?` | 0 PSTN 普通 / 1 普通 VOIP / 2 附属投屏 VOIP / 3 linux sdk for VOIP / 4 附属语音 PSTN / 5 附属视频 PSTN / 6 linux sdk for PSTN |
| `audio_state` / `video_state` / `screen_shared_state` | 同名 | `bool?` | 麦克风 / 摄像头 / 屏幕共享 |

### E6. `GetAttendeeListAsync` — 获取参会成员列表（历史）

```csharp
[Post("/cgi-bin/meeting/get_attendee_list")]
Task<GetMeetingAttendeeListResponse> GetAttendeeListAsync(
    [Body] GetMeetingAttendeeListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `sub_meetingid` + `start_time` / `end_time`（`long?`，单位秒；**区间 ≤31 天**，为空时默认前推 31 天 / 取当前时间；**两者都不传时最大跨度 90 天**；**对周期性会议查询暂时不生效，请改用分页参数**）+ `cursor` + `limit`（**最大 100**，须与首次一致）。

**返回**：`has_more` + `next_cursor` + `attendees[]`（`MeetingAttendedAttendee`）——比 E5 多 `quit_time`（**同为字符串秒**）、`webinar_role`（0 普通参会 / 1 内部嘉宾 / 2 外部嘉宾 / 3 邀请链接入会嘉宾 / 4 观众）、`net`（有线 / WIFI / 2G / 3G / 4G / 未知，**仅成员在会中时才返回**）与 `customer_data`（经专属链接入会时携带 E3 的值）；外部成员 `userid` 为空。

### E7. `GetWaitingRoomCurrentUserListAsync` — 等候室当前成员

```csharp
[Post("/cgi-bin/meeting/waitingroom/get_current_user_list")]
Task<GetMeetingWaitingRoomCurrentUserListResponse> GetWaitingRoomCurrentUserListAsync(
    [Body] GetMeetingWaitingRoomCurrentUserListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（**需已开启等候室且会议处于「会议进行中」，否则返回空列表**）+ `limit`（**默认 10、最大 50**，须与首次一致）+ `cursor`。

**返回**：`has_more` + `next_cursor` + `user_list[]`（`MeetingWaitingRoomCurrentUser`：`userid`（**企业外成员为空字符串**）/ `instance_id` / `customer_data`（经专属链接入会时携带创建时指定的值））。

### E8. `GetWaitingRoomUserListAsync` — 等候室成员历史

```csharp
[Post("/cgi-bin/meeting/waitingroom/get_user_list")]
Task<GetMeetingWaitingRoomUserListResponse> GetWaitingRoomUserListAsync(
    [Body] GetWaitingRoomUserListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `limit`（**默认 20、最大 50**，须与首次一致）+ `cursor`。

**返回**：`has_more` + `next_cursor` + `user_list[]`（`MeetingWaitingRoomUserRecord`：`userid` / `tmp_openid` / `instance_id` / `join_time` / `quit_time`——**这两个时间是 `long?` 毫秒**，与 E5/E6 的「字符串秒」不同，见 §5）。
### E9. `CheckDeviceInMeetingAsync` — 检查设备是否在会中

```csharp
[Post("/cgi-bin/meeting/check_device_in_meeting")]
Task<CheckMeetingDeviceInMeetingResponse> CheckDeviceInMeetingAsync(
    [Body] CheckMeetingDeviceInMeetingRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`userid`（官方必填）+ `meetingid_list`（官方必填，**须为本企业创建的会议**）+ `instance_id_list`（**不带则查所有设备**，取值 0 PSTN / 1 PC / 2 Mac / 3 Android / 4 iOS / 5 Web / 6 iPad / 7 Android Pad / 8 小程序 / 9 voip·sip / 10 linux / 20·21 Rooms for Touch…）。
**返回**：`result_list[]`（`MeetingDeviceCheckResult`：`meetingid` + `instance_id`）——**只回已入会的设备**。

### E10. `GetGuestsAsync` — 获取会议嘉宾列表

```csharp
[Post("/cgi-bin/meeting/get_guests")]
Task<GetMeetingGuestsResponse> GetGuestsAsync(
    [Body] GetMeetingGuestsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）。**返回**：`meetingid` / `meeting_code` / `title` / `guests[]`（`MeetingGuest`：`area` / `phone_number` / `guest_name`）——**嘉宾是手机号身份，与外部联系人 ID 无关**（域 README 第 2 节）。

### E11. `SetGuestsAsync` — 更新会议嘉宾列表

```csharp
[Post("/cgi-bin/meeting/set_guests")]
Task<WechatWorkResponse> SetGuestsAsync(
    [Body] SetMeetingGuestsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `guests`（`List<MeetingGuest>?`，官方必填）——**覆盖式更新** ⇒ 想保留原嘉宾须先 E10 读回再整份提交。`guest_name` **1~16 位字符**、`area` 中国传 `86`（不是 `+86` 也不是 `0086`）。**闸门**：**仅允许修改本应用创建的会议**。**返回**：`WechatWorkResponse`。

> 嘉宾手机号属个人敏感数据：示例用假值，名单与手机号不得落日志。
### E12. `GetQualityAsync` — 会议健康度

```csharp
[Post("/cgi-bin/meeting/get_quality")]
Task<GetMeetingQualityResponse> GetQualityAsync(
    [Body] GetMeetingQualityRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（必填）+ **`start_time`（必填，UNIX 秒；可查区间为过去 7 天到现在）** + `sub_meetingid`（周期性会议必传）+ `cursor` + `limit`（**默认与最大 50**，须与首次一致）。

**返回**：`quality` / `audio_quality` / `video_quality` / `screen_share_quality` / `network_quality`（**0 无数据 / 1 好 / 2 较好 / 3 中 / 4 差**；`quality` 为 **0 无数据 / 1 健康 / 2 告警**）+ `problems[]`（告警具体问题）+ `attendees[]`（`MeetingQualityAttendee`：`userid` / `tmp_openid` / `instance_id` / `quality` / 三项质量，**按入会时间正序、同成员多端平铺返回**）+ `next_cursor` + `has_more`。

> 七支与 Webinar 侧的 `webinar/enroll/*`（见 [MeetingWebinar.md](./MeetingWebinar.md)）**同构**：共用 `MeetingEnrollQuestion` / `MeetingEnrollInfo` / `MeetingEnrollAnswer` / `MeetingEnrollImportItem` / `MeetingEnrollImportResult` / `MeetingEnrollId` / `MeetingEnrollIdRef`，差异只在路由前缀与 Webinar 额外支持的问题类型。

### E13. `SetEnrollConfigAsync` — 修改会议报名配置

```csharp
[Post("/cgi-bin/meeting/enroll/set_config")]
Task<SetMeetingEnrollConfigResponse> SetEnrollConfigAsync(
    [Body] SetMeetingEnrollConfigRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `approve_type`（1 自动审批（默认）/ 2 手动审批）+ `is_collect_question`（1 不收集（默认）/ 2 收集）+ `no_registration_needed_for_staff`（`bool?`）+ `question_list[]`。**闸门**：**需要会议已开启报名**。**返回**：`question_count`。

**题目容量与排序**（注释原文）：问题**最多 8 个**、标题 **≤40 字符**、每选项 **≤40 个汉字**且**最多 8 个选项**；**非特殊问题按传入顺序排序、特殊问题优先置于最前**；`is_required` 是 **1 否 / 2 是**（不是 0/1）；`special_type` **1 无 / 2 手机号 / 3 邮箱 / 4 姓名 / 5 公司名称**（**目前特殊问题均为简答题**，且此时 `question_title` 与 `question_type` 无效）。

### E14. `GetEnrollConfigAsync` — 获取会议报名配置

```csharp
[Post("/cgi-bin/meeting/enroll/get_config")]
Task<GetMeetingEnrollConfigResponse> GetEnrollConfigAsync(
    [Body] GetMeetingEnrollConfigRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）。**返回**：`approve_type` / `is_collect_question` / `no_registration_needed_for_staff` / `question_list[]`。**闸门**：**会议未开启报名时会返回「未开启报名错误」**。

### E15. `QueryEnrollIdsAsync` — 按 `tmp_openid` 查询报名 ID

```csharp
[Post("/cgi-bin/meeting/enroll/query_by_tmp_openid")]
Task<QueryMeetingEnrollIdsResponse> QueryEnrollIdsAsync(
    [Body] QueryMeetingEnrollIdsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `tmp_openid_list`（**单次最多 500 条**）+ `sorting_rules`（1 优先手机号导入、再手动报名（默认）/ 2 优先手动报名）。`tmp_openid` 取自 E5 实时会中成员列表或 PSTN `get_tmp_openid`。

**返回**：`enroll_id_list[]`（`MeetingEnrollId`：`tmp_openid` + `enroll_id`；**仅返回已报名成员的报名 ID，若传入成员无人报名则该字段不返回**）。

### E16. `ListEnrollsAsync` — 获取报名列表

```csharp
[Post("/cgi-bin/meeting/enroll/list")]
Task<ListMeetingEnrollsResponse> ListEnrollsAsync(
    [Body] ListMeetingEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `status`（**0 全部 / 1 待审批 / 2 已拒绝 / 3 已批准**，默认全部）+ `cursor` + `limit`（**默认与最大 50**，**须与首次取 `cursor` 时一致**）。

**返回**：`has_more` + `next_cursor` + `enroll_list[]`（`MeetingEnrollInfo`：`enroll_id` / **`enroll_time` 是 `"2023-03-23 14:00"` 形态的 UTC+8 字符串、不是时间戳** / `enroll_source_type` 1 手动 / 2 批量导入 / `nick_name`（**手机号导入且未设昵称时显示手机号**）/ `status` 1 待审批 / 2 已拒绝 / 3 已批准 / `userid`（企业内成员时返回）/ `tmp_openid` / `enroll_code`（**电话入会凭证**）/ `answer_list[]`）。

### E17. `ApproveEnrollsAsync` — 审批报名

```csharp
[Post("/cgi-bin/meeting/enroll/approve")]
Task<ApproveMeetingEnrollsResponse> ApproveEnrollsAsync(
    [Body] ApproveMeetingEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`enroll_id_list`（`List<string>?`——字符串数组）** + `action`（**1 取消批准 → 状态回到待审批 / 2 拒绝 / 3 批准**）。**返回**：`handled_count`（成功处理数量）。**闸门**：**仅允许修改该应用创建的会议**。

### E18. `ImportEnrollsAsync` — 批量导入报名

```csharp
[Post("/cgi-bin/meeting/enroll/import")]
Task<ImportMeetingEnrollsResponse> ImportEnrollsAsync(
    [Body] ImportMeetingEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `enroll_list[]`（`MeetingEnrollImportItem`：`userid`，**或** `area` + `phone_number`（用手机号导入时 `area` 必填，中国传 `86`））。**闸门**：**会议未开启报名时会返回未开启报名错误**。**返回**：`total_count`（成功导入条数）+ `enroll_list[]`（`MeetingEnrollImportResult`：`enroll_id` / `userid` / `area` / `phone_number` / `nick_name` / `enroll_code`）。

### E19. `DeleteEnrollsAsync` — 删除报名

```csharp
[Post("/cgi-bin/meeting/enroll/delete")]
Task<DeleteMeetingEnrollsResponse> DeleteEnrollsAsync(
    [Body] DeleteMeetingEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`enroll_id_list`（`List<MeetingEnrollIdRef>?`——对象数组，每项仅 `enroll_id`）**。**返回**：`total_count`（成功删除数量）。

> **`enroll_id_list` 同名不同形**（守卫类头部注释与 MT5）：**E17 审批是字符串数组**、**E19 删除是对象数组** ⇒ 两个类型不可互换，Webinar 侧同构同规。
## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **受邀成员两种形态** | 创建/修改请求是 `MeetingInvitees{userid:[…]}`（对象包数组）；本族 E1/E2 是 `List<MeetingInvitee>`（对象数组）⇒ MT5 锁定 | 复用同一类型 → 序列化形态与官方报文不符 |
| **扩容受邀成员是官方规定路径** | 创建面普通 100 / 付费最多 300；**超过就走 E2（≤2000）**，且**管理员必须在列表内** | 在创建请求里塞 500 人 |
| `enroll_id_list` 两形 | E17 审批是 `List<string>`、E19 删除是 `List<MeetingEnrollIdRef>` | 互换 → 参数被拒 |
| **时间单位在本族内部就不统一** | E5/E6 的 `join_time` / `quit_time` 是**字符串秒**；E8 等候室记录的 `join_time` / `quit_time` 是**`long?` 毫秒**；`enroll_time` 是 `"yyyy/MM/dd HH:mm"` 字符串 | 一套解析函数处理全部 |
| **`limit` 必须与首次一致** | E5 ≤50、E6 ≤100、E7 默认 10 ≤50、E8 默认 20 ≤50、E12 ≤50、E16 ≤50 ⇒ 翻页中途改 `limit` 会失败 | 游标失效或报错 |
| 周期性会议三种态度 | E5 **必传 `sub_meetingid`**；E12 **必传**；E6 **对周期性会议不生效（改分页）** | 用同一策略处理周期会议 |
| **两个「角色」枚举不等价** | E5 `role`：**4 = 游客（微信小程序入会）**；MRA 侧 `user_role`：**4 = 小程序**；另有 `webinar_role`（0~4）与 `join_type`（0~6）四套 | 用一套枚举解释四套值 |
| `customer_data` 是结构不是文本 | `{"ver":"1.0","userData":"…"}` 的 **Base64**、≤256 字节；E3 创建后原样回显、E6/E7 读回 | 直接塞裸文本 |
| **E3/E4 的对象限制** | E3 不支持 **Webinar**；E4 不支持 **个人会议号会议与 Webinar** | 对 Webinar 或 `meeting_type=5` 调用 |
| E7 的空列表语义 | 未开启等候室 / 会议不在进行中 ⇒ **返回空列表而非报错** | 把空当作「无人等候」 |
| 嘉宾是覆盖式更新 | E11 提交即整份替换；嘉宾身份是**手机号 + 姓名**（`area` 传 `86`），与外部联系人 ID 无关 | 增量式提交把原嘉宾清掉；或拿 `external_userid` 当嘉宾 |
| **报名闸门两处** | E13/E14/E18 要求「会议已开启报名」；配合创建时 `settings.enable_enroll`（[Meeting.md](./Meeting.md)）⇒ 未开启时报错 | 只配 `enable_enroll=false` 却调 E13 |
| 报名问题容量与排序 | **≤8 题**、标题 ≤40 字符、每选项 ≤40 汉字且 ≤8 个；`is_required` 是 **1 否 / 2 是**；**特殊问题优先置顶**、非特殊按传入顺序 | 用 0/1 表达必填 |
| `nick_name` 可能是手机号 | E16/E18 注释：手机号导入且未设昵称时该字段显示手机号 ⇒ **落日志等于落手机号** | 把昵称当安全字段 |
| `enroll_code` 是电话入会凭证 | 出现在 E16/E18 应答 ⇒ 与 `tmp_openid` 配对使用，属凭据 | 打印或外发 |
| 健康度区间只有 7 天 | E12 的 `start_time` 必填且只能查**过去 7 天到现在**；同成员多端**平铺**返回 | 期望 31 天窗口或按成员去重 |
| **全部官方即 POST** | 本族 19 支全 POST（含 `get_*` / `list` / `check_*` 语义）——全域仅 `layout/list_template` 是 GET | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验人数/题目/选项容量与 `limit` 一致性**、**不做 Base64 与 `customer_data` 结构校验**、**不做读-改-写合并**（嘉宾覆盖写、报名全量提交由宿主保证）、**不缓存游标**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E7 成功也可能是空列表（状态不满足）；E16 成功仍要按 `has_more` 续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

受邀成员超 2000、管理员不在列表、`sub_meetingid` 缺失、会议未开启报名、`tmp_openid_list` 超 500、目标为 Webinar 或个人会议号、非本应用创建的会议等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验**（含「未开启报名错误」的具体码值） | 官方 98156~98165 / 98794~98821 / 99039 / 99040 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| E1 的 `limit` | 请求只有 `meetingid` + `cursor`，官方未给单页条数 ⇒ 未核验 | 官方 98160 |
| 「2000 人与所购在线会议室方数」的换算 | 注释只说相关，未给映射关系与超容行为 | 官方会议容量说明 |
| 专属链接的有效期与重置 | E3/E4 未记 `meeting_short_url` 有效期、能否撤销 | 官方 98818 / 98819 |
| `customer_data` 事件侧 | 注释称可经「用户入会 / 进入等候室」事件读出，事件载荷不在本域 | `Mud.Wechat.Work.Callback` 事件族 |
| `problems` 的取值集合 | E12 只说是「告警的具体问题列表」，未列字符串取值 | 官方 98821 |
| 报名审批的推送面 | 审批结果是否有回调事件未在本域留档 | 官方报名文档 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 2 节参会人三件套与两种 `invitees` 形态；第 3 节逐端点时间跨度；第 4 节周期性与 `sub_meetingid`；第 9 节批次上限表 #9/#12/#13/#14/#15/#19/#22/#23）
- 同域近邻：[Meeting.md](./Meeting.md)（创建 / 修改 / 取消与 `settings.enable_enroll`、`meeting_code` / `meeting_link` 的高级页声明）· [MeetingControl.md](./MeetingControl.md)（**本族 E5 的 `tmp_openid` + `instance_id` 是全部会控动作的定位键**）· [MeetingPstn.md](./MeetingPstn.md)（电话入会成员的 `tmp_openid` 与 `enroll_code` 联动）· [MeetingMra.md](./MeetingMra.md)（`user_role` 与 `webinar_member_role` 的另一套取值）
- 跨域对照：报名七件套在 Webinar 侧的同构路由见 [MeetingWebinar.md](./MeetingWebinar.md)；`MeetingGuest.area` 的写法与 `WebinarGuest` 一致，但两者类型面不同
- 官方页：[获取受邀成员（98160）](https://developer.work.weixin.qq.com/document/path/98160) · [更新受邀成员（98162）](https://developer.work.weixin.qq.com/document/path/98162) · [实时会中成员（98157）](https://developer.work.weixin.qq.com/document/path/98157) · [参会成员列表（98156）](https://developer.work.weixin.qq.com/document/path/98156) · [等候室（98163）](https://developer.work.weixin.qq.com/document/path/98163) · [会议健康度（98821）](https://developer.work.weixin.qq.com/document/path/98821) · [报名配置（98797）](https://developer.work.weixin.qq.com/document/path/98797) · [报名审批（98807）](https://developer.work.weixin.qq.com/document/path/98807)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
