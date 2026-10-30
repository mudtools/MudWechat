# 录制管理 · 云录制列表、地址、共享配置与转写

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 录制管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingRecordingService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingRecordingService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingRecordingService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingRecordingService_Internal.cs)（**唯一**子接口，承载全部 10 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`ListMeetingRecordsRequest/Response`、`MeetingRecord`、`MeetingRecordFile`、`GetRecordStatisticsRequest/Response`、`RecordStatisticsSummary`、`UpdateRecordSharingConfigRequest`、`RecordSharingConfig`、`DeleteMeetingRecordRequest`、`DeleteRecordFileRequest`、`GetRecordFileRequest/Response`、`GetRecordFileListRequest/Response`、`RecordFileAddress`、`RecordFileDownload`、转写三支的 `GetRecordTranscript*` / `SearchRecordTranscript*` 与 `RecordTranscript*` 系列；本族在 MT4 的逐族分解中计 35 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(10)`、`record/` 主段 + `record/transcript/` 三子段、全部官方即 POST / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 10)` / MT3 令牌绑定 / MT4 上下文登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取录制列表 [98192](https://developer.work.weixin.qq.com/document/path/98192) · 录制统计 98209 · 修改共享配置 98208 · 删除录制 98206 · 删除录制文件 98207 · 获取录制文件 98205 · 获取文件列表 98196 · 转写段落列表 98212 · 转写详情 98211 · 转写搜索 98213（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（录制列表 → 文件清单 → 单文件 → 统计 → 共享配置 → 转写段落/详情/搜索 → 删文件/删录制））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释（含两处有效期口径与「官方称返回总数但参数表无该字段」的留档）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 5/8/9/10 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

围绕**云录制**的四层读与两类破坏性动作，10 支端点：

| 层 | # | 方法 | 路由 |
|---|---|---|---|
| 录制对象 | E1 | `ListMeetingRecordsAsync` | `record/list` |
| | E4 | `DeleteMeetingRecordAsync` | `record/delete` |
| 文件 | E7 | `GetRecordFileListAsync` | `record/get_file_list` |
| | E6 | `GetRecordFileAsync` | `record/get_file` |
| | E5 | `DeleteRecordFileAsync` | `record/delete_file` |
| 共享与统计 | E3 | `UpdateRecordSharingConfigAsync` | `record/update_sharing_config` |
| | E2 | `GetRecordStatisticsAsync` | `record/get_statistics` |
| 转写 | E8 | `GetRecordTranscriptParagraphsAsync` | `record/transcript/get_paragraph_list` |
| | E9 | `GetRecordTranscriptDetailAsync` | `record/transcript/get_detail` |
| | E10 | `SearchRecordTranscriptAsync` | `record/transcript/search` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）；前缀恒为 `/cgi-bin/meeting/record/`（与 OA 会议、会议室预订是两套体系）。

**本族是「取回与治理」，不含开录**：开启 / 暂停 / 停止云录制属会中控制面（[MeetingControl.md](./MeetingControl.md)）与 `settings.auto_record_type`（[Meeting.md](./Meeting.md)）；本族只处理已生成的录制对象。

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

- 自建应用须配置在「**可调用接口的应用**」列表中；**仅允许获取 / 操作该应用创建的会议的录制数据**。
- **三级 ID 链路**：`meetingid`（会议）→ `meeting_record_id`（会议录制）→ `record_file_id`（录制文件）——三支删改与转写端点都需要**成对传**（见 §5）。
- E1 的定位方式三选一：`meetingid` / `meeting_code` / `userid`（**`meetingid` 非空时优先**；两者都空且填 `userid` ⇒ 查该成员所有会议的录制列表）。
- **时间跨度 ≤31 天**：E1 的 `start_time` / `end_time` 官方必填（**单位秒**）；E2 默认展示最近 31 天、区间也不允许超过 31 天。
- **`limit` 必须与首次一致**（E1：默认 **10**、最大 **20**；本域多处有此约束，属官方游标形态）。
- **录制状态闸门**：`MeetingRecord.state` **1 录制中 / 3 转码完成**，**只有转码完成才返回录制文件列表**。
- **地址类字段的有效期有两个口径**：`view_address` / `download_address` / `audio_address` 注释为「**默认 6 小时过期**」，而纪要与转写文件 `RecordFileDownload.DownloadAddress` 注释为「**链接有效期为一个小时**」⇒ 按各自 DTO 注释取用，勿混用。
- 共享配置里的 `password`（共享密码，默认随机生成）与 `sharing_url` 是**访问凭据**：只写参数名、不回显、不进日志。
- **转写文本是会议内容**（可能含个人信息、决策与商业秘密）：`text` / `words[].text` / 发言人 `userid` 一律不得落日志，导出须按宿主数据分级处理。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E10 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingRecordingService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingRecordingService`） |
| 不可注入 | `IWechatWorkMeetingRecordingService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `ListMeetingRecordsAsync` — 获取会议录制列表

**签名**

```csharp
[Post("/cgi-bin/meeting/record/list")]
Task<ListMeetingRecordsResponse> ListMeetingRecordsAsync(
    [Body] ListMeetingRecordsRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/record/list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/record/list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `ListMeetingRecordsRequest` |

**请求体** `ListMeetingRecordsRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `meetingid` | `Meetingid` | `string?` | 三选一 | **不为空时优先按会议 ID 查询** |
| `meeting_code` | `MeetingCode` | `string?` | 三选一 | `meetingid` 为空且本字段非空时按会议 code 查 |
| `userid` | `Userid` | `string?` | 三选一 | 两者均为空时按成员查其**所有会议**的录制列表 |
| `start_time` | `StartTime` | `int?` | **是** | UNIX 秒；**跨度 ≤31 天** |
| `end_time` | `EndTime` | `int?` | **是** | UNIX 秒 |
| `cursor` | `Cursor` | `string?` | 否 | 上次的 `next_cursor`，初次可不传 |
| `limit` | `Limit` | `int?` | 否 | 默认 **10**、最大 **20**；**必须与首次取到 `cursor` 时的 `limit` 一致** |

**返回** `ListMeetingRecordsResponse`：`has_more`（`bool?`）+ `next_cursor` + `record_list`（`List<MeetingRecord>?`）。

`MeetingRecord`：`meeting_record_id` / `meetingid` / `meeting_code` / `host_user_id` / `meeting_start_time`（**毫秒**）/ `title` / `state`（**1 录制中 / 3 转码完成**）/ `record_file_list`（`List<MeetingRecordFile>?`）。

`MeetingRecordFile`：`record_file_id` / `record_start_time` / `record_end_time`（**毫秒**）/ `record_size`（字节）/ `sharing_state`（**0 未开启 / 1 开启**）+ 开启共享时才回传的一组字段（`sharing_url` / `required_same_corp` / `required_attendee` / `password` / `sharing_expire` / `allow_download`）。

**调用示例**（列表 → 文件清单 → 统计 → 改共享 → 转写三段；ID 三级链路要成对携带）

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
var records = provider.GetRequiredService<IWechatWorkInternalMeetingRecordingService>();

// ④ E1 录制列表：meetingid / meeting_code / userid 三选一；跨度 ≤31 天；limit 翻页期间不可变
int beginAt = (int)DateTimeOffset.Now.AddDays(-7).ToUnixTimeSeconds();
int endAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
var listed = await records.ListMeetingRecordsAsync(new ListMeetingRecordsRequest
{
    Meetingid = "demo-meeting-id",           // 假值；须为本应用创建的会议
    StartTime = beginAt,
    EndTime = endAt,
    Limit = 20,                              // 默认 10、最大 20
}, cancellationToken);
WechatWorkException.ThrowIfFailed(listed, "/cgi-bin/meeting/record/list");

foreach (MeetingRecord record in listed.RecordList ?? new List<MeetingRecord>())
{
    // state：1 录制中 / 3 转码完成 —— 只有转码完成才有文件列表
    if (record.State != 3)
    {
        logger.LogInformation("录制 {RecordId} 尚未转码完成（state={State}）", record.MeetingRecordId, record.State);
        continue;
    }
    foreach (MeetingRecordFile file in record.RecordFileList ?? new List<MeetingRecordFile>())
    {
        logger.LogInformation("文件 {FileId} 大小 {Size} 字节、共享是否开启 {SharingState}",
            file.RecordFileId, file.RecordSize, file.SharingState);
        // file.SharingUrl / file.Password 是访问凭据：不得进日志与异常消息
    }
}

// ⑤ E7 文件清单（含播放/下载地址，默认 6 小时过期）：meeting_record_id + meetingid 都要传
var fileList = await records.GetRecordFileListAsync(new GetRecordFileListRequest
{
    MeetingRecordId = "demo-meeting-record-id",
    Meetingid = "demo-meeting-id",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(fileList, "/cgi-bin/meeting/record/get_file_list");
string? firstFileId = (fileList.RecordFiles ?? new List<RecordFileAddress>()).FirstOrDefault()?.RecordFileId;

// ⑥ E6 单个录制文件（视频/音频地址 + 纪要 + AI 转写文件）；纪要与转写链接有效期为 1 小时
if (firstFileId is not null)
{
    var one = await records.GetRecordFileAsync(new GetRecordFileRequest
    {
        RecordFileId = firstFileId,
        Meetingid = "demo-meeting-id",
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(one, "/cgi-bin/meeting/record/get_file");
    logger.LogInformation("录制文件取回：视频格式 {VideoType}、音频格式 {AudioType}、纪要 {SummaryCount} 份",
        one.DownloadAddressFileType, one.AudioAddressFileType, one.MeetingSummary?.Count ?? 0);
    // one.ViewAddress / DownloadAddress / AudioAddress：短时地址，只交下载器，不落日志

    // ⑦ E2 录制统计（按天）：默认最近 31 天、区间 ≤31 天
    var stat = await records.GetRecordStatisticsAsync(new GetRecordStatisticsRequest
    {
        MeetingRecordId = "demo-meeting-record-id",
        StartTime = beginAt,
        EndTime = endAt,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(stat, "/cgi-bin/meeting/record/get_statistics");
    int views = (stat.Summaries ?? new List<RecordStatisticsSummary>())
        .Sum(s => s.ViewCount ?? 0);          // summaries 元素含 date（yyyy-MM-dd）/ view_count / download_count
    logger.LogInformation("窗口内观看合计 {Views} 次", views);

    // ⑧ E3 修改共享配置：enable_sharing=false 时不得再设其余参数；关闭共享后原共享设置不保存
    var sharing = await records.UpdateRecordSharingConfigAsync(new UpdateRecordSharingConfigRequest
    {
        MeetingRecordId = "demo-meeting-record-id",
        Meetingid = "demo-meeting-id",
        SharingConfig = new RecordSharingConfig
        {
            EnableSharing = true,
            SharingAuthType = 2,               // 0 登录成员 / 1 企业内 / 2 参会成员 / 3 全部 / 4 审批（Rooms 不支持）/ 5 微信特邀链接（Rooms 不支持）
            EnablePassword = false,            // false 时不可传 password
            EnableSharingExpire = false,        // false 时不可传 sharing_expire
            AllowDownload = false,
        },
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(sharing, "/cgi-bin/meeting/record/update_sharing_config");

    // ⑨ E8 转写段落列表（pid + 起止毫秒，用于定位）
    var paragraphs = await records.GetRecordTranscriptParagraphsAsync(new GetRecordTranscriptParagraphsRequest
    {
        RecordFileId = firstFileId,
        Meetingid = "demo-meeting-id",
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(paragraphs, "/cgi-bin/meeting/record/transcript/get_paragraph_list");
    logger.LogInformation("声纹识别状态 {AudioDetect}、段落数 {Count}",
        paragraphs.AudioDetect, paragraphs.Paragraphs?.Count ?? 0);

    // ⑩ E9 转写详情：pid 含本身；未发完时用最后一条 pid 续查
    string? pid = null;
    while (true)
    {
        var detail = await records.GetRecordTranscriptDetailAsync(new GetRecordTranscriptDetailRequest
        {
            RecordFileId = firstFileId,
            Meetingid = "demo-meeting-id",
            Pid = pid,                          // 默认从 0 开始
            Limit = 50,                         // 不传即查全量
        }, cancellationToken);
        WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/meeting/record/transcript/get_detail");
        if (detail.HasMore != true) break;
        pid = (detail.Transcripts?.Paragraphs ?? new List<RecordTranscriptDetailParagraph>()).LastOrDefault()?.Pid;
        if (pid is null) break;
    }

    // ⑪ E10 转写搜索：命中段落/句子偏移与时间轴
    var hits = await records.SearchRecordTranscriptAsync(new SearchRecordTranscriptRequest
    {
        RecordFileId = firstFileId,
        Meetingid = "demo-meeting-id",
        Text = "预算",                          // 关键词属会议内容：不进日志
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(hits, "/cgi-bin/meeting/record/transcript/search");
    logger.LogInformation("转写命中 {HitCount} 处、时间轴 {TimelineCount} 点",
        hits.Hits?.Count ?? 0, hits.Timelines?.Count ?? 0);
}

// ⑫ E5 删除单个录制文件 / E4 删除整场会议录制：均为不可逆
var fileDeleted = await records.DeleteRecordFileAsync(new DeleteRecordFileRequest
{
    RecordFileId = firstFileId!,
    Meetingid = "demo-meeting-id",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(fileDeleted, "/cgi-bin/meeting/record/delete_file");

var recordDeleted = await records.DeleteMeetingRecordAsync(new DeleteMeetingRecordRequest
{
    MeetingRecordId = "demo-meeting-record-id",
    Meetingid = "demo-meeting-id",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(recordDeleted, "/cgi-bin/meeting/record/delete");
```

### E2. `GetRecordStatisticsAsync` — 获取录制统计

**签名**

```csharp
[Post("/cgi-bin/meeting/record/get_statistics")]
Task<GetRecordStatisticsResponse> GetRecordStatisticsAsync(
    [Body] GetRecordStatisticsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_record_id`（官方必填）+ `start_time` / `end_time`（`int?`，UNIX **秒**；默认最近 31 天、区间 ≤31 天）。
**返回**：`summaries`（`List<RecordStatisticsSummary>?`）——`date`（`yyyy-MM-dd`）+ `view_count` + `download_count`（均为当天数据，默认 0）。

### E3. `UpdateRecordSharingConfigAsync` — 修改录制共享配置

**签名**

```csharp
[Post("/cgi-bin/meeting/record/update_sharing_config")]
Task<WechatWorkResponse> UpdateRecordSharingConfigAsync(
    [Body] UpdateRecordSharingConfigRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_record_id` + `meetingid`（均官方必填）+ `sharing_config`（`RecordSharingConfig?`）。

`RecordSharingConfig`：

| 字段 | 属性 | 类型 | 取值 / 约束 |
|---|---|---|---|
| `enable_sharing` | `EnableSharing` | `bool?` | 默认 `true`；**未开启时不允许设置以下参数**；**改为 `false` 关闭共享后，之前设置的共享设置将不保存** |
| `sharing_auth_type` | `SharingAuthType` | `int?` | 0 仅登录成员 / 1 仅企业内 / 2 仅参会成员 / 3 全部成员 / 4 权限审批成员（**Rooms 不支持**）/ 5 微信特邀链接成员（**Rooms 不支持**） |
| `enable_password` | `EnablePassword` | `bool?` | 默认 `true` |
| `password` | `Password` | `string?` | 默认随机生成；**`enable_password=true` 时必传、`=false` 时不可传** |
| `enable_sharing_expire` | `EnableSharingExpire` | `bool?` | 默认 `false` |
| `sharing_expire` | `SharingExpire` | `long?` | UNIX **毫秒**时间戳；**`enable_sharing_expire=true` 时必传、false 时不可传** |
| `allow_download` | `AllowDownload` | `bool?` | 默认 `false` |

**返回**：`WechatWorkResponse`。

### E4. `DeleteMeetingRecordAsync` — 删除会议录制

**签名**

```csharp
[Post("/cgi-bin/meeting/record/delete")]
Task<WechatWorkResponse> DeleteMeetingRecordAsync(
    [Body] DeleteMeetingRecordRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_record_id` + `meetingid`（**两者官方均必填**）⇒ 删除整场会议录制（其下录制文件一并消失）。**返回**：`WechatWorkResponse`（无业务字段）。**不可逆**，回收站与级联行为见 §7。

### E5. `DeleteRecordFileAsync` — 删除单个录制文件

**签名**

```csharp
[Post("/cgi-bin/meeting/record/delete_file")]
Task<WechatWorkResponse> DeleteRecordFileAsync(
    [Body] DeleteRecordFileRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`record_file_id` + `meetingid`（均官方必填）⇒ 只删指定录制文件，不动同场其它文件。**返回**：`WechatWorkResponse`。

### E6. `GetRecordFileAsync` — 获取录制文件

**签名**

```csharp
[Post("/cgi-bin/meeting/record/get_file")]
Task<GetRecordFileResponse> GetRecordFileAsync(
    [Body] GetRecordFileRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`record_file_id` + `meetingid`（均官方必填）。

**返回** `GetRecordFileResponse`：`record_file_id` / `meetingid` / `meeting_code` / `record_name`（录制文件名）/ `meeting_record_name` / `start_time` / `end_time`（`long?`）+ **`view_address`（播放）、`download_address`（下载）、`audio_address`（音频）——三者注释均标「默认 6 小时过期」** + `download_address_file_type`（如 `mp4`）+ `audio_address_file_type`（如 `m4a`）+ `meeting_summary[]`（纪要）+ `ai_meeting_transcripts[]`（AI 转写文件）。

两支文件项类型 **`RecordFileDownload`**：`download_address`（注释「**链接有效期为一个小时**」）+ `file_type`（如 `txt` / `pdf` / `docx`）。

> 本端点两处口径须按 DTO 注释取用：① 应答 `start_time` / `end_time` **参数表标 `int64`、官方示例为字符串** ⇒ 本仓以参数表为准用 `long?` 承载；② 视频/音频地址 **6 小时** vs 纪要与 AI 转写文件 **1 小时** ⇒ 两套有效期勿混用（域 README 第 5 节）。
### E7. `GetRecordFileListAsync` — 获取录制文件列表

**签名**

```csharp
[Post("/cgi-bin/meeting/record/get_file_list")]
Task<GetRecordFileListResponse> GetRecordFileListAsync(
    [Body] GetRecordFileListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meeting_record_id` + `meetingid`（均官方必填）。

**返回** `GetRecordFileListResponse`：`meeting_record_id` / `meetingid` / `meeting_code` / `title` + `record_files[]`（`RecordFileAddress`——即 E6 的那组地址字段：`record_file_id` / `view_address` / `download_address`（**默认 6 小时过期**）/ `download_address_file_type` / `audio_address`（**默认 6 小时过期**）/ `audio_address_file_type` / `meeting_summary[]`），但**不含 `record_name` 与录制起止时间**。
### E8. `GetRecordTranscriptParagraphsAsync` — 转写段落列表

**签名**

```csharp
[Post("/cgi-bin/meeting/record/transcript/get_paragraph_list")]
Task<GetRecordTranscriptParagraphsResponse> GetRecordTranscriptParagraphsAsync(
    [Body] GetRecordTranscriptParagraphsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`record_file_id` + `meetingid`（均官方必填）。
**返回**：`audio_detect`（**0 未完成 / 1 已完成**——声纹识别状态，与转写生成过程独立；无需声纹识别或已完成时为 1）+ `paragraphs`（`List<RecordTranscriptParagraph>?`：`pid` / `start_time` / `end_time`，**毫秒**）。

> 接口注释留档一处官方不一致：**官方称本接口返回「段落总数」，但参数表没有该字段**（MT5 注释登记）⇒ 本模型不建模总数，计数请用 `paragraphs.Count`。

### E9. `GetRecordTranscriptDetailAsync` — 转写详情

**签名**

```csharp
[Post("/cgi-bin/meeting/record/transcript/get_detail")]
Task<GetRecordTranscriptDetailResponse> GetRecordTranscriptDetailAsync(
    [Body] GetRecordTranscriptDetailRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`record_file_id` + `meetingid`（均必填）+ `pid`（**获取 `pid` 后（含）的段落**，默认从 0 开始）+ `limit`（**默认查询全量数据**）。

**返回** `GetRecordTranscriptDetailResponse`：`has_more`（`bool?`——**传入 `pid` 或 `limit` 且未一次返回完时为 true**）+ `transcripts`（`RecordTranscriptDetail?`）：

- `paragraphs`：`RecordTranscriptDetailParagraph[]` ⇒ `pid` + 起止（毫秒）+ `speaker_info`（`RecordTranscriptSpeaker`，**`userid` 仅当发言人属同一企业时返回**）+ `sentences`；
- `sentences`：`RecordTranscriptSentence[]` ⇒ `sid` + 起止（毫秒）+ `words`；
- `words`：`RecordTranscriptWord[]` ⇒ `wid` + 起止（毫秒）+ `text`（**逐词文本**）；
- `keywords`（`List<string>?`）、`audio_detect`。

> 续查方式：拿本页**最后一条段落的 `pid`** 作为下一次请求的 `pid`（`pid` 含本身，注意去重）。

### E10. `SearchRecordTranscriptAsync` — 转写搜索

**签名**

```csharp
[Post("/cgi-bin/meeting/record/transcript/search")]
Task<SearchRecordTranscriptResponse> SearchRecordTranscriptAsync(
    [Body] SearchRecordTranscriptRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`record_file_id` + `meetingid` + `text`（**三者均官方必填**）。

**返回**：`hits`（`RecordTranscriptHit[]`：`pid` / `sid` / `offset` / `length`）+ `timelines`（`RecordTranscriptTimeline[]`：`pid` / `sid` / `start_time`（毫秒））⇒ 命中偏移用于高亮，时间轴用于跳转播放点。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **ID 三级链路且必须成对传** | E4/E7 用 `meeting_record_id` + `meetingid`；E5/E6/E8/E9/E10 用 `record_file_id` + `meetingid` ⇒ 官方参数表就是「业务 ID + 会议 ID」双键 | 只传一个 → 官方报参数缺失 |
| **两套链接有效期** | 视频/音频地址「默认 **6 小时**过期」；纪要与 AI 转写文件「有效期为 **1 小时**」 | 缓存地址后下载失败 |
| `state` 闸门 | **1 录制中 / 3 转码完成**；仅转码完成返回录制文件列表 | 在 state=1 时读文件列表判空为「无录制」 |
| **时间单位三种混用** | E1/E2 的 `start_time` / `end_time` 是 **UNIX 秒**（`int?`！）；`MeetingRecord.meeting_start_time`、`MeetingRecordFile.record_*_time`、转写的段落/句子/词条时间是 **毫秒**；`sharing_expire` 是**毫秒时间戳** | 秒/毫秒互抄导致窗口整体偏移 |
| `start_time` 类型是 `int?` | E1/E2 用 `int?` 承载秒级时间戳（与多数 `long?` 字段不同）⇒ 2038 年前后需注意 | 赋 `long` 值编译失败或溢出 |
| **`limit` 必须与首次一致** | E1 注释原文（默认 10、最大 20）；本域多处同规 | 翻页中途改 `limit` → 游标失效 |
| **`sharing_expire` 与 `password` 的条件互斥** | `enable_password=true` ⇒ `password` 必传；`=false` ⇒ 不可传。`enable_sharing_expire=true` ⇒ `sharing_expire` 必传；`=false` ⇒ 不可传；`enable_sharing=false` ⇒ 其余参数不允许设置 | 关闭共享仍传共享参数 |
| **关闭共享不保留原设置** | 注释：`enable_sharing` 改 `false` 后「之前设置的共享设置将不保存」 | 以为可「暂停共享后再恢复」 |
| `sharing_auth_type` 的 4/5 不支持 Rooms | 注释逐条标注「Rooms 不支持」 | 对 Rooms 会议设这两档 |
| **段落总数不存在** | E8 官方称返回「段落总数」但参数表无该字段（MT5 注释留档）⇒ 本模型无该属性 | 期望 `total` 字段 |
| `pid` 含本身 | E9 的 `pid` 是「获取 pid **后（含）**的段落」⇒ 续查最后一条 pid 会重复一条 | 不去重导致纪要重复行 |
| `has_more` 的触发条件 | E9：仅「传入 `pid` 或 `limit` 且未一次返回完」才为 true；默认一次拉全量 | 无条件翻页 |
| `speaker_info.userid` 条件返回 | **仅发言人属同一企业时返回**，非本企业成员不返回 | 假定人人有 userid |
| 删改端点无回执 | E3/E4/E5 返回 `WechatWorkResponse` ⇒ 生效要再调 E1/E6 复核 | 凭应答断定删除成功 |
| `start_time`/`end_time` 的官方形态不一致 | E6 应答两字段：参数表 `int64`、官方示例为字符串 ⇒ 本仓按参数表以 `long?` 承载 | 按示例写 `string?` |
| 全部官方即 POST | 本族 10 支全 POST（含 `record/list`、`get_statistics`、转写三支）——全域仅 `layout/list_template` 是 GET | 改 GET → 打坏 MT1 与 G5 |
| 凭据与内容红线 | `sharing_url` / `password` / 短时地址 / 转写正文一律不进日志、遥测与异常消息 | 排障时整包落日志 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 31 天跨度与三选一必填**、**不缓存或续期短时地址**、**不做转写段落去重与聚合**、**不做删除前的二次确认**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 接口调用成功 | E1 成功仍要按 `has_more` 续拉；E4/E5 成功不代表业务面复核 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`meeting_record_id` / `record_file_id` 不存在、录制仍在进行、共享参数组合非法、地址已过期等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98192 / 98205~98213 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| 录制保留期与自动清理 | 注释未记云录制的存储期限、企业容量配额与超期清理策略 | 官方会议录制说明页 |
| 删除是否可恢复 | 本域无回收站/恢复端点，官方是否存在恢复面未记录 | 官方 98206 / 98207 |
| `state` 的完整取值 | 注释只给 1 录制中 / 3 转码完成（**缺 2**）⇒ 其余取值的含义未逐项核验 | 官方 98192 参数表 |
| 「转码中/失败」的可观测面 | 无对应状态与查询端点，失败原因不可归因 | 官方 98192 |
| 转写生成时机与可用性判定 | 本域无「转写是否已生成」的开关字段（E8 只有声纹状态） | 官方 98211 / 98212 |
| `sharing_expire` 与 `enable_sharing_expire` 的默认组合 | 注释给「默认为空」，但未记不开启有效期时的实际过期行为（永久 or 默认时长） | 官方 98208 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 5 节「本域无会议附件通道，录制返回的是下载地址而非素材 ID」及两条有效期口径；第 8 节动词分布；第 9 节批次上限表 #92/#93）
- 同域近邻：[Meeting.md](./Meeting.md)（`settings.auto_record_type` / `attendee_join_auto_record` 决定录制是否自动生成）· [MeetingControl.md](./MeetingControl.md)（主持人暂停/停止云录制的会中动作）· [MeetingMra.md](./MeetingMra.md) 与 [MeetingRooms.md](./MeetingRooms.md)（`sharing_auth_type` 4/5 对 Rooms 不支持的对象侧）
- 跨域对照：短时下载地址与 [Media.md](../Media/Media.md) 的 `media_id` 是两种机制（本域不经素材上传）；导出文件的数据分级处理归宿主
- 官方页：[获取录制列表（98192）](https://developer.work.weixin.qq.com/document/path/98192) · [获取录制文件（98205）](https://developer.work.weixin.qq.com/document/path/98205) · [删除录制（98206）](https://developer.work.weixin.qq.com/document/path/98206) · [删除录制文件（98207）](https://developer.work.weixin.qq.com/document/path/98207) · [修改共享配置（98208）](https://developer.work.weixin.qq.com/document/path/98208) · [录制统计（98209）](https://developer.work.weixin.qq.com/document/path/98209) · [转写段落（98212）](https://developer.work.weixin.qq.com/document/path/98212) · [转写详情（98211）](https://developer.work.weixin.qq.com/document/path/98211) · [转写搜索（98213）](https://developer.work.weixin.qq.com/document/path/98213)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
