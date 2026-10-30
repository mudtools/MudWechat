# 网络研讨会 · Webinar 的预定、嘉宾、暖场与报名审批

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 网络研讨会管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingWebinarService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingWebinarService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingWebinarService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingWebinarService_Internal.cs)（**唯一**子接口，承载全部 14 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`CreateWebinarRequest/Response`、`UpdateWebinarRequest`、`CancelWebinarRequest`、`GetWebinarRequest/Response`、`ListWebinarGuestsRequest/Response`、`UpdateWebinarGuestsRequest`、`UpdateWebinarWarmUpRequest`、`WebinarGuest`、`WebinarHostInfo`、`WebinarMediaSetting`、`WebinarMediaSettingInfo` 与报名七支的 `Set/Get/Query/List/Approve/Import/Delete WebinaarEnroll*`（报名问题与记录结构 `MeetingEnroll*` 与预约会议共用）；本族在 MT4 的逐族分解中计 32 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(14)`、全部挂 `webinar/` 段、`webinar/enroll/` 七支子段单独断言 / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 14)` / MT3 令牌绑定 / MT4 上下文登记 / MT5「Webinar 族同义字段名与普通会议不同：`title` 而非 `subject`、`status` 与 `start_time` 是字符串、入会静音两处名为 `EnableEnterMute` 与 `MuteEnableJoin`、主持人型 `List<WebinarHostInfo>`、嘉宾型 `List<WebinarGuest>`」）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建 [98842](https://developer.work.weixin.qq.com/document/path/98842) · 修改 98843 · 取消 98870 · 详情 98860 · 嘉宾列表 98871 · 更新嘉宾 98872 · 暖场 98882 · 报名配置 98875 / 98874 · 报名反查 98873 · 报名列表 98876 · 审批 98877 · 导入 98880 · 删除 98881（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（密码式研讨会创建 → 嘉宾 → 暖场 → 报名配置与审批 → 详情 → 取消））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 / MT5 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释（含四处官方文档陷阱留档）+ DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 3/10 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**网络研讨会（Webinar）**——沙龙 / 圆桌 / 行业峰会场景，官方口径支持**最高 5 万人同时参会**，14 支端点全部挂 `/cgi-bin/meeting/webinar/`：

| 层 | # | 方法 | 路由 |
|---|---|---|---|
| 主体生命周期 | E1 | `CreateWebinarAsync` | `webinar/create` |
| | E2 | `UpdateWebinarAsync` | `webinar/update` |
| | E3 | `CancelWebinarAsync` | `webinar/cancel` |
| | E4 | `GetWebinarAsync` | `webinar/get` |
| 嘉宾与暖场 | E5 | `ListWebinarGuestsAsync` | `webinar/list_guest` |
| | E6 | `UpdateWebinarGuestsAsync` | `webinar/update_guest_list` |
| | E7 | `UpdateWebinarWarmUpAsync` | `webinar/update_warm_up` |
| 报名七件套 | E8 | `SetWebinarEnrollConfigAsync` | `webinar/enroll/set_config` |
| | E9 | `GetWebinarEnrollConfigAsync` | `webinar/enroll/get_config` |
| | E10 | `QueryWebinarEnrollIdsAsync` | `webinar/enroll/query_by_tmp_openid` |
| | E11 | `ListWebinarEnrollsAsync` | `webinar/enroll/list` |
| | E12 | `ApproveWebinarEnrollsAsync` | `webinar/enroll/approve` |
| | E13 | `ImportWebinarEnrollsAsync` | `webinar/enroll/import` |
| | E14 | `DeleteWebinarEnrollsAsync` | `webinar/enroll/delete` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）；权限口径还要求「**成员需要在应用的可见范围内**」。

**Webinar 不是「大号预约会议」**：同名字段形态不同（`title` 而非 `subject`、时间与状态是**字符串**）、嘉宾是独立身份体系（内部 / 外部 / 链接）、多了活动页、暖场、问答与人工审核面。预约会议侧的报名七件套在 [MeetingAdvanced.md](./MeetingAdvanced.md)，**结构同构但路由不同**（`/cgi-bin/meeting/enroll/` vs `/cgi-bin/meeting/webinar/enroll/`）。

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

- 自建应用须配置在「**可调用接口的应用**」列表中；**仅允许操作 / 获取该应用创建的网络研讨会的数据**；成员须**在应用可见范围内**（详情类端点还注明「仅返回可见范围内成员」）。
- **时间闸**：`start_time` **不能少于当前时间戳半小时以上**；`start_time` / `end_time` 官方必填且**是字符串形态的秒级时间戳**（参数表与示例均为字符串，本模型按字符串承载）。
- **观众观看限制 `admission_type` 官方必填**：**0 公开 / 1 报名 / 2 密码**；`= 2` 时 `password`（**4~6 位数字**）必传且仅此时生效。
- **回放联动**：`playback_for_audience` **官方必填**（默认 `false`）；**开启时必须开启云录制**（`media_setting.auto_record_type = cloud`）。
- **活动页联动**：`cover_url` 与 `description` 均需 `activity_page = true`；封面仅 **PNG/JPEG、分辨率 >640×360（推荐 1280×720）、≤5MB**，且**接口上传封面是异步上传**（须订阅「素材上传结果」事件取结果）；`description` 仅支持纯文本 **1~5000 位**；`display_number_of_attendees`（0 不展示 / 1 展示）同样依赖活动页。
- **敏感词与审核**：`sensitive_words` **最多 50 个、单个 ≤10 个中文字符**；`enable_manual_check = true` 时创建应答才返回 `manual_check_link` 与 `manual_check_password`。
- **主持人覆盖写**：`hosts` 默认为 `admin_userid`，**修改时传入即覆盖原有设置**。
- **嘉宾三型**：`guest_type` **1 内部 / 2 外部 / 3 链接嘉宾**（更新嘉宾列表文档页仅列 1/2）；内部嘉宾必填 `userid`；外部嘉宾必填 `area`（中国传 **`86`**，不是 `+86`）/ `phone_number` / `guest_name`（1~16 位），`email` 选填；**经嘉宾链接入会的嘉宾无 `phone_number` 与 `email`**。
- **暖场二选一**：`warm_up_picture` 与 `warm_up_video` **同时传入以图片为准**；图片推荐 1280×720、png/jpg、≤5M；视频 1280×720、mp4、≤1G；**会议开始前 15 分钟进入的观众可观看，且仅 3.3.0 及以上客户端支持**。
- **报名问题类型**：Webinar 侧在预约会议的类型之上**额外支持 `6` - 组织规模**（见 E8）。
- 凭据与个人数据：`password`（观看密码）、`manual_check_password`（审核密码）、三类入会/审核链接、嘉宾手机号与邮箱一律**只写参数名、不得进日志与异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E14 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingWebinarService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingWebinarService`） |
| 不可注入 | `IWechatWorkMeetingWebinarService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `CreateWebinarAsync` — 创建网络研讨会

**签名**

```csharp
[Post("/cgi-bin/meeting/webinar/create")]
Task<CreateWebinarResponse> CreateWebinarAsync(
    [Body] CreateWebinarRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/webinar/create` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/webinar/create?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `CreateWebinarRequest` |

**请求体** `CreateWebinarRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `admin_userid` | `AdminUserid` | `string?` | **是** | 网络研讨会管理员 userid |
| `title` | `Title` | `string?` | **是** | 主题 **1~255 位字符**（本族字段名是 `title`，非预约会议的 `subject`；也非创建请求里的…见 §5） |
| `sponsor` | `Sponsor` | `string?` | 否 | 主办方名称 1~40 位 |
| `start_time` | `StartTime` | **`string?`** | **是** | 秒级时间戳的**字符串**形态；不早于当前时间半小时 |
| `end_time` | `EndTime` | **`string?`** | **是** | 同上 |
| `admission_type` | `AdmissionType` | `int?` | **是** | 0 公开 / 1 报名 / 2 密码 |
| `hosts` | `Hosts` | `List<WebinarHostInfo>?` | 否 | 主持人（每项只有 `userid`）；默认管理员；**修改时覆盖** |
| `password` | `Password` | `string?` | 条件 | `admission_type=2` 时必传，4~6 位数字 |
| `cover_url` | `CoverUrl` | `string?` | 否 | 需 `activity_page=true`；PNG/JPEG、>640×360、≤5MB；**异步上传** |
| `description` | `Description` | `string?` | 否 | 需 `activity_page=true`；纯文本 1~5000 位 |
| `enable_guest_invite_link` | `EnableGuestInviteLink` | `bool?` | 否 | 默认 false；开启后经嘉宾链接入会者自动成为嘉宾 |
| `media_setting` | `MediaSetting` | `WebinarMediaSetting?` | 否 | 媒体参数（下表）；**官方参数表标 `object[]`、示例为单对象 ⇒ 本仓按单对象承载** |
| `enable_qa` | `EnableQa` | `bool?` | 否 | 是否开启问答（默认 true） |
| `sensitive_words` | `SensitiveWords` | `List<string>?` | 否 | ≤50 个、单个 ≤10 个中文字符 |
| `enable_manual_check` | `EnableManualCheck` | `bool?` | 否 | 人工审核（默认 false） |
| `activity_page` | `ActivityPage` | `bool?` | 否 | 活动页（默认开启；查询时返回默认 true） |
| `display_number_of_attendees` | `DisplayNumberOfAttendees` | `int?` | 否 | 0 不展示 / 1 展示（需 `activity_page=true`） |
| `playback_for_audience` | `PlaybackForAudience` | `bool?` | **是** | 允许观众看回放（默认 false）；**开启须 `media_setting.auto_record_type=cloud`** |
| `preparation_mode` | `PreparationMode` | `bool?` | 否 | 准备模式 |

`WebinarMediaSetting`（写入侧）：`enable_enter_mute`（默认 **true**）、`allow_unmute_self`（默认 true）、`allow_enter_before_host`（默认 true）、`enable_screen_watermark`（默认 false）、`watermark_type`（0 单排 / 1 多排）、`allow_external_user`（**官方两文档页口径相反**，见 §7）、`auto_record_type`（`none`/`local`/`cloud`，**依赖企业账户锁定值、仅客户端 2.7.0+ 生效**）、`attendee_join_auto_record` / `enable_host_pause_auto_record`（**仅 `cloud` 时生效**）。

**返回** `CreateWebinarResponse`：`title` / `meetingid` / `meeting_code` / `start_time` / `end_time` / `admission_type` / `password` / **`audience_join_link`（观众入会链接）** / **`guest_join_link`（嘉宾入会链接：`enable_guest_invite_link=true` 时经此入会自动成为嘉宾）** / `manual_check_link` / `manual_check_password`（后两者仅开启人工审核时返回）。

**调用示例**（创建密码式 Webinar → 加嘉宾 → 设暖场 → 开报名 → 审批 → 收尾取消）

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
var webinar = provider.GetRequiredService<IWechatWorkInternalMeetingWebinarService>();

// ④ E1 创建：start_time / end_time 是「字符串形态的秒级时间戳」；playback_for_audience 官方必填
long now = DateTimeOffset.Now.ToUnixTimeSeconds();
string startTime = (now + 3600).ToString();          // 须不早于当前时间半小时以上
string endTime = (now + 3 * 3600).ToString();
var created = await webinar.CreateWebinarAsync(new CreateWebinarRequest
{
    AdminUserid = "demo_user_0001",                   // 假值
    Title = "行业峰会直播",                              // 1~255 位
    Sponsor = "示例主办方",
    StartTime = startTime,
    EndTime = endTime,
    AdmissionType = 2,                                // 0 公开 / 1 报名 / 2 密码
    Password = "123456",                              // 凭据：不外发、不打印
    PlaybackForAudience = true,                       // 开启回放必须同时开启云录制
    MediaSetting = new WebinarMediaSetting
    {
        AutoRecordType = "cloud",
        EnableEnterMute = true,
    },
    ActivityPage = true,
    DisplayNumberOfAttendees = 1,
    EnableQa = true,
    SensitiveWords = new List<string> { "示例敏感词" },   // ≤50 个、单个 ≤10 个中文字符
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/meeting/webinar/create");
string meetingid = created.Meetingid!;
// 链接与审核密码是入会凭据：只判存在，不落日志
logger.LogInformation("创建成功（观众链接与嘉宾链接是否齐备：{HasLinks}）",
    created.AudienceJoinLink is not null && created.GuestJoinLink is not null);

// ⑤ E6 更新嘉宾列表：内部嘉宾填 userid；外部嘉宾填 area/phone_number/guest_name（中国 area = 86）
var guestsUpdated = await webinar.UpdateWebinarGuestsAsync(new UpdateWebinarGuestsRequest
{
    Meetingid = meetingid,
    Guests = new List<WebinarGuest>
    {
        new WebinarGuest { GuestType = 1, Userid = "demo_user_0002" },
        new WebinarGuest { GuestType = 2, Area = "86", PhoneNumber = "13800000000", GuestName = "外部嘉宾" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(guestsUpdated, "/cgi-bin/meeting/webinar/update_guest_list");

// ⑥ E7 暖场：图片与视频二选一（同传以图片为准）
var warmUp = await webinar.UpdateWebinarWarmUpAsync(new UpdateWebinarWarmUpRequest
{
    Meetingid = meetingid,
    WarmUpPicture = config["Demo:WarmUpImageUrl"],
    AllowAttendeesInviteOthers = true,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(warmUp, "/cgi-bin/meeting/webinar/update_warm_up");

// ⑦ E8 + E9 报名配置：审批类型 + 是否收集问题；问题类型比预约会议多一档 6-组织规模
var enrollConfig = await webinar.SetWebinarEnrollConfigAsync(new SetWebinarEnrollConfigRequest
{
    Meetingid = meetingid,
    ApproveType = 2,                    // 1 自动审批（默认）/ 2 手动审批
    IsCollectQuestion = 2,              // 1 不收集（默认）/ 2 收集
    NoRegistrationNeededForStaff = true, // 本企业成员无需报名
    QuestionList = new List<MeetingEnrollQuestion>
    {
        new MeetingEnrollQuestion { QuestionTitle = "公司名称", QuestionType = 3, SpecialType = 5, IsRequired = 2 },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(enrollConfig, "/cgi-bin/meeting/webinar/enroll/set_config");

var readBack = await webinar.GetWebinarEnrollConfigAsync(
    new GetWebinarEnrollConfigRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(readBack, "/cgi-bin/meeting/webinar/enroll/get_config");
logger.LogInformation("报名问题 {QuestionCount} 个", readBack.QuestionList?.Count ?? 0);

// ⑧ E11 + E12 待审批列表与批量批准：enroll_id_list 在审批请求里是「字符串数组」
var enrolls = await webinar.ListWebinarEnrollsAsync(new ListWebinarEnrollsRequest
{
    Meetingid = meetingid,
    Status = 1,                         // 0 全部 / 1 待审批 / 2 已拒绝 / 3 已批准
    Limit = 50,                         // 最大 50、默认 50
}, cancellationToken);
WechatWorkException.ThrowIfFailed(enrolls, "/cgi-bin/meeting/webinar/enroll/list");
string? firstEnrollId = (enrolls.EnrollList ?? new List<MeetingEnrollInfo>())
    .Select(e => e.EnrollId).FirstOrDefault(id => id is not null);
if (firstEnrollId is not null)
{
    var approved = await webinar.ApproveWebinarEnrollsAsync(new ApproveWebinarEnrollsRequest
    {
        Meetingid = meetingid,
        EnrollIdList = new List<string> { firstEnrollId },
        Action = 3,                     // 1 取消批准（状态回到待审批）/ 2 拒绝 / 3 批准
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(approved, "/cgi-bin/meeting/webinar/enroll/approve");
    logger.LogInformation("成功处理 {HandledCount} 条", approved.HandledCount);
}

// ⑨ E4 详情（meetingid 与 meeting_code 二选一，同传以 meetingid 为准）
var detail = await webinar.GetWebinarAsync(new GetWebinarRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/meeting/webinar/get");
// status 是字符串枚举（MEETING_STATE_*）、start_time 是字符串时间戳
logger.LogInformation("Webinar 状态字符串长度 {Len}、主持人 {HostCount} 位", detail.Status?.Length ?? 0, detail.Hosts?.Count ?? 0);

// ⑩ E3 取消：仅本应用创建的研讨会
var cancelled = await webinar.CancelWebinarAsync(new CancelWebinarRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(cancelled, "/cgi-bin/meeting/webinar/cancel");
```

### E2. `UpdateWebinarAsync` — 修改网络研讨会

**签名**

```csharp
[Post("/cgi-bin/meeting/webinar/update")]
Task<WechatWorkResponse> UpdateWebinarAsync(
    [Body] UpdateWebinarRequest request,
    CancellationToken cancellationToken = default);
```

请求体字段面与 E1 基本一致（`meetingid` 必填 + `title` / `sponsor` / `start_time` / `end_time` / `admission_type` / `hosts` / `password` / `cover_url` / `description` / `media_setting` / `activity_page` / `playback_for_audience` …），官方限制三条：**仅可改本应用创建的数据、成员须在可见范围内、`playback_for_audience` 官方必填且开启时须 `cloud` 录制**；**修改 `hosts` 会覆盖原有设置**。**返回**：`WechatWorkResponse`。

### E3. `CancelWebinarAsync` — 取消网络研讨会

```csharp
[Post("/cgi-bin/meeting/webinar/cancel")]
Task<WechatWorkResponse> CancelWebinarAsync(
    [Body] CancelWebinarRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填，仅本应用创建的研讨会）。**返回**：`WechatWorkResponse`。

### E4. `GetWebinarAsync` — 获取网络研讨会详情

```csharp
[Post("/cgi-bin/meeting/webinar/get")]
Task<GetWebinarResponse> GetWebinarAsync(
    [Body] GetWebinarRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` 与 `meeting_code` **二者必须送一个，同传以 `meetingid` 为准**。

**返回** `GetWebinarResponse`（要点）：`title`（**官方参数表作 `subject`、响应示例作 `title`，本仓以示例为准**）、`meetingid`、`meeting_code`、**`status`（字符串枚举）**、`sponsor`、**`start_time` / `end_time`（字符串秒级时间戳）**、`admission_type`、`hosts`（`List<WebinarHostInfo>?`）、`password`、`cover_url`、`description`、`media_setting`（`WebinarMediaSettingInfo`，字段名与写入侧不同，见 §5）、活动页与回放相关字段。

### E5. `ListWebinarGuestsAsync` — 获取嘉宾列表

```csharp
[Post("/cgi-bin/meeting/webinar/list_guest")]
Task<ListWebinarGuestsResponse> ListWebinarGuestsAsync(
    [Body] ListWebinarGuestsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` 与 `meeting_code` 二选一（同传以 `meetingid` 为准）。
**返回**：`guests`（`List<WebinarGuest>?`——**官方参数表标 object、示例为数组，本模型按数组承载**）；每项 `guest_type` / `userid` / `area` / `phone_number` / `guest_name` / `email`。

### E6. `UpdateWebinarGuestsAsync` — 更新嘉宾列表

```csharp
[Post("/cgi-bin/meeting/webinar/update_guest_list")]
Task<WechatWorkResponse> UpdateWebinarGuestsAsync(
    [Body] UpdateWebinarGuestsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（必填）+ `guests`（`List<WebinarGuest>?`，**官方必填**）。**返回**：`WechatWorkResponse`。

> 嘉宾身份必填面按 `guest_type` 分支（内部：`userid`；外部：`area` + `phone_number` + `guest_name`，`email` 选填；链接嘉宾：`phone_number` 与 `email` 缺失）。本仓不做本地分支校验。

### E7. `UpdateWebinarWarmUpAsync` — 暖场设置

```csharp
[Post("/cgi-bin/meeting/webinar/update_warm_up")]
Task<WechatWorkResponse> UpdateWebinarWarmUpAsync(
    [Body] UpdateWebinarWarmUpRequest request,
    CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `meetingid` | `Meetingid` | `string?` | 官方必填 |
| `warm_up_picture` | `WarmUpPicture` | `string?` | **与视频二选一，同传以图片为准**；推荐 1280×720、png/jpg、≤5M；开始前 15 分钟进入的观众可观看；**仅 3.3.0+ 客户端** |
| `warm_up_video` | `WarmUpVideo` | `string?` | 推荐 1280×720、mp4、≤1G |
| `allow_attendees_invite_others` | `AllowAttendeesInviteOthers` | `bool?` | 是否允许参会者在暖场中邀请成员（默认允许） |

**返回**：`WechatWorkResponse`；上传结果可订阅「**网络研讨会暖场上传结果**」事件（回调面，不在本域）。

### E8. `SetWebinarEnrollConfigAsync` — 设置报名配置

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/set_config")]
Task<SetWebinarEnrollConfigResponse> SetWebinarEnrollConfigAsync(
    [Body] SetWebinarEnrollConfigRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `approve_type`（1 自动审批（默认）/ 2 手动审批）+ `is_collect_question`（1 不收集（默认）/ 2 收集）+ `no_registration_needed_for_staff`（`bool?`）+ `question_list[]`。**返回**：`question_count`（不收集问题时返回 **0**）。

**与预约会议报名的两处差异**：**问题类型额外支持 `6` - 组织规模**（其余取值同 `MeetingEnrollQuestion`：`question_type` 1 单选 / 2 多选 / 3 简答、`special_type` 1 无 / 2 手机号 / 3 邮箱 / 4 姓名 / 5 公司名称且**目前特殊问题均为简答题**、`is_required` **1 否 / 2 是**）；**非特殊问题按传入顺序排序、特殊问题优先置于最前**。容量同规：**问题 ≤8 个、标题 ≤40 字符、每选项 ≤40 汉字且 ≤8 个**。

### E9. `GetWebinarEnrollConfigAsync` — 获取报名配置

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/get_config")]
Task<GetWebinarEnrollConfigResponse> GetWebinarEnrollConfigAsync(
    [Body] GetWebinarEnrollConfigRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）。**返回**：`approve_type` / `is_collect_question` / `no_registration_needed_for_staff` / `question_list[]`。**闸门**：会议未开启报名时官方返回「未开启报名错误」（具体码见 §7）。

### E10. `QueryWebinarEnrollIdsAsync` — 按 `tmp_openid` 查询报名 ID

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/query_by_tmp_openid")]
Task<QueryWebinarEnrollIdsResponse> QueryWebinarEnrollIdsAsync(
    [Body] QueryWebinarEnrollIdsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `tmp_openid_list`（**单次 ≤500 条**）+ `sorting_rules`（1 优先手机号导入、再手动报名（默认）/ 2 优先手动报名）。**返回**：`enroll_id_list[]`（`MeetingEnrollId`：`tmp_openid` + `enroll_id`）——**仅返回已报名成员的报名 ID，若传入成员无人报名则该字段不返回**。

### E11. `ListWebinarEnrollsAsync` — 获取报名列表

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/list")]
Task<ListWebinarEnrollsResponse> ListWebinarEnrollsAsync(
    [Body] ListWebinarEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `status`（**0 全部 / 1 待审批 / 2 已拒绝 / 3 已批准**，默认全部）+ `cursor` + `limit`（**默认与最大 50**）。**返回**：`has_more` + `next_cursor` + `enroll_list[]`（`MeetingEnrollInfo`，字段面与预约会议同型：`enroll_time` 是 `"2023-03-23 14:00"` 形态的 UTC+8 **字符串**、`nick_name` **手机号导入且未设昵称时显示手机号**、`enroll_code` 为**电话入会凭证**）。

### E12. `ApproveWebinarEnrollsAsync` — 审批报名

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/approve")]
Task<ApproveWebinarEnrollsResponse> ApproveWebinarEnrollsAsync(
    [Body] ApproveWebinarEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`enroll_id_list`（`List<string>?`——字符串数组）** + `action`（**1 取消批准 → 状态回到待审批 / 2 拒绝 / 3 批准**）。**返回**：`handled_count`（成功处理数量）。

### E13. `ImportWebinarEnrollsAsync` — 批量导入报名

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/import")]
Task<ImportWebinarEnrollsResponse> ImportWebinarEnrollsAsync(
    [Body] ImportWebinarEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `enroll_list[]`（`MeetingEnrollImportItem`：`userid`，**或** `area` + `phone_number`（用手机号导入时 `area` 必填，中国传 `86`、不是 `+86`））。**返回**：`total_count`（成功导入条数）+ `enroll_list[]`（`MeetingEnrollImportResult`：`enroll_id` / `userid` / `area` / `phone_number` / `nick_name` / `enroll_code`）。

### E14. `DeleteWebinarEnrollsAsync` — 删除报名

```csharp
[Post("/cgi-bin/meeting/webinar/enroll/delete")]
Task<DeleteWebinarEnrollsResponse> DeleteWebinarEnrollsAsync(
    [Body] DeleteWebinarEnrollsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + **`enroll_id_list`（`MeetingEnrollIdRef` 对象数组，每项仅 `enroll_id`）**。**返回**：`total_count`（成功删除数量）。

> **`enroll_id_list` 同名不同形**（守卫类头部注释留档，普通会议与 Webinar **同构同规**）：**审批（E12）是字符串数组**、**删除（E14）是对象数组** ⇒ 两个类型不可互换。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`title` 而非 `subject`** | 官方参数表作 `subject`、响应示例作 `title` ⇒ 本仓**以示例为准**（MT5 锁定 `GetWebinarResponse.Title`） | 按参数表读 `subject` 取不到值 |
| **时间与状态是字符串** | `start_time` / `end_time` 为「秒级时间戳的字符串形态」；`status` 为 `MEETING_STATE_*` 字符串枚举 | 按数值 / 整数状态码解析（预约会议侧才是 `long?` 与 `int?`） |
| `media_setting` 两型不同名 | 写入侧 `WebinarMediaSetting.EnableEnterMute`（JSON `enable_enter_mute`，默认 true）；详情侧 `WebinarMediaSettingInfo.MuteEnableJoin`（JSON `mute_enable_join`，**默认 false**）⇒ MT5 专门锁定这两处 | 用同一类型读写，或以为默认值一致 |
| **`playback_for_audience` 与录制的联动** | 官方必填；`=true` 时 `media_setting.auto_record_type` **必须**为 `cloud` | 开回放未开云录制被拒 |
| 活动页依赖链 | `cover_url` / `description` / `display_number_of_attendees` 三者都需要 `activity_page=true` | 未开活动页时设封面无效 |
| 封面与暖场都是异步上传 | `cover_url`、暖场素材走异步上传；结果须订阅「素材上传结果」/「网络研讨会暖场上传结果」事件 | 同步取素材 ID 后立刻断言可用 |
| **嘉宾与预约会议嘉宾不是一回事** | Webinar 用 `WebinarGuest`（`guest_type` 1/2/3 + `email`）；预约会议高级族用 `MeetingGuest`（仅 `area`/`phone_number`/`guest_name`） | 复制结构体或混用字段 |
| `area` 是**字符串** `"86"` | `WebinarGuest.Area` 为 `string?`（中国传 `86`，不是 `+86`/`0086`）；而 PSTN 族的 `PstnPhoneNumber.Area` 是 **`int?`** | 跨族复制写法导致类型或值不合法 |
| **`enroll_id_list` 同名不同形** | 删除=对象数组（`MeetingEnrollIdRef`）、审批=字符串数组（`string`） | 互换导致序列化形态与官方报文不符 |
| `action` 的三档 | 1 取消批准（→ 回到**待审批**）/ 2 拒绝 / 3 批准；`is_required` 是 **1 否 / 2 是** | 用 0/1 表达布尔 |
| **`no_registration_needed_for_staff`** | `true` = 本企业成员无需报名；默认（false）= 成员与外部都需报名 | 以为本企业成员默认免报名 |
| 报名问题数量与长度 | 与高级族同规：**问题最多 8 个、每选项 ≤40 汉字、标题 ≤40 字符**；`question_count` 不收集问题时为 0 | 超出被官方拒 |
| `hosts` 覆盖写 | E1/E2 传入即**覆盖原有主持人设置** | 以为追加 |
| 详情只返回可见范围成员 | 注释明确「仅返回在应用可见范围内的成员」 | 把缺员当数据缺失 |
| Webinar 与预约会议能否互转 | 注释未给依据，二者共用 `meetingid` / `meeting_code` 口径**不代表可互转**（域 README 已登记缺口） | 据字段同名推断可互转 |
| 全部官方即 POST | 本族 14 支全 POST（含 `webinar/get`、`list_guest`、`enroll/list`）——全域仅 `layout/list_template` 是 GET | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验半小时时间闸与必填组合**、**不校验回放-录制联动**、**不做嘉宾必填分支校验**、**不做报名审批状态机**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功才拿到 `meetingid` 与两类入会链接 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`admission_type=2` 未传密码、开启回放但未开云录制、封面越界、`meetingid` / `meeting_code` 均空、非本应用创建、报名未开启（E9 注释：「会议未开启报名返回未开启报名错误」）等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98842~98882 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| `allow_external_user` 的口径 | DTO 注释明示「**官方两文档页口径相反**」⇒ 语义需按官方页重新裁决（本仓原样透传，不做本地解释） | 官方 98842 与 98860 对照 |
| `status` 字符串枚举全集 | 注释列出 `MEETING_STATE_INVALID/_INIT/_CANCELLED/_STARTED/_ENDED`，是否还有其它值未核验 | 官方 98860 |
| 「5 万人同时参会」的前置 | 是否需购买容量、是否分档未记录 | 官方 Webinar 说明页 |
| 暖场与封面素材的上传端点 | 本域只有 `url` 型参数，异步上传与「素材上传结果」事件面不在本域 | `Callback` 包 + 官方素材页 |
| 报名审批的推送面 | 审批结果是否有回调事件未在本域留档 | 官方报名事件页 |
| 人工审核链接的有效期与重置 | `manual_check_link` / `manual_check_password` 的时效未记录 | 官方 98842 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 3 节「Webinar 侧时间是字符串，与预约会议的数值时间不兼容」、第 10 节 Webinar 族同义字段名清单）
- 同域近邻：[MeetingAdvanced.md](./MeetingAdvanced.md)（预约会议报名七件套与 `MeetingEnroll*` 结构本体；等候室、健康度、专属链接也在该族）· [MeetingRecording.md](./MeetingRecording.md)（`playback_for_audience` 打开后的回放取回面）· [MeetingRooms.md](./MeetingRooms.md)（`meeting_type=6` 即 Webinar）
- 跨域边界：MRA 状态应答含 `webinar_member_role`（0 普通参会 / 1 内部嘉宾 / 2 外部嘉宾 / 3 邀请链接入会嘉宾 / 4 观众），见 [MeetingMra.md](./MeetingMra.md)
- 官方页：[创建网络研讨会（98842）](https://developer.work.weixin.qq.com/document/path/98842) · [修改（98843）](https://developer.work.weixin.qq.com/document/path/98843) · [取消（98870）](https://developer.work.weixin.qq.com/document/path/98870) · [详情（98860）](https://developer.work.weixin.qq.com/document/path/98860) · [嘉宾列表（98871）](https://developer.work.weixin.qq.com/document/path/98871) · [更新嘉宾（98872）](https://developer.work.weixin.qq.com/document/path/98872) · [暖场（98882）](https://developer.work.weixin.qq.com/document/path/98882) · [报名配置（98875）](https://developer.work.weixin.qq.com/document/path/98875) · [报名审批（98877）](https://developer.work.weixin.qq.com/document/path/98877)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
