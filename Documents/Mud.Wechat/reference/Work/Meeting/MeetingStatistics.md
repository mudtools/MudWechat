# 会议统计管理 · 会议发起记录

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 会议统计管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingStatisticsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingStatisticsService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingStatisticsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingStatisticsService_Internal.cs)（**唯一**子接口，承载 1 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、无子目录，`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`GetMeetingStartListRequest` / `GetMeetingStartListResponse` / `MeetingStartRecord`；本族在 MT4 的逐族分解中计 3 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(1)` + **专项断言路由挂在 `statistics/` 段**（官方原文如此）/ MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 1)` / MT3 令牌绑定 / MT4 上下文登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取会议发起记录 [99651](https://developer.work.weixin.qq.com/document/path/99651)（**仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（发起记录翻页））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 3/8/9 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

只有一支端点：**查询企业内「会议发起记录」**，用于统计与容量归因（谁在什么时候发起了会议、哪些发起因并发上限失败）。

| # | 方法 | 动词 | 路由 |
|---|---|---|---|
| E1 | `GetMeetingStartListAsync` | POST | `/cgi-bin/meeting/statistics/get_start_list` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）——官方未向第三方与代开发开放本族 ⇒ 不设对应子接口。

**「统计族」这个名字容易与其它统计面混淆**：本族**只给发起记录**（`userid` + `start_time`），不给参会人、时长、质量与录制数据——那些分别在各端点里取（参会人列表与明细在高级族 #12/#13，健康度 #19，录制在录制族）。

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

- 自建应用须配置到「**协作 - 会议 - 可调用接口的应用**」列表。
- `type` **官方必填**：**1 发起成功的会议记录 / 2 发起失败的会议**（企业同时发起的会议数已达上限、员工无法发起）。
- `begin_time` 与 `end_time` 官方必填（Unix 秒），**跨度 ≤30 天，查询区间为左闭右开**。
- `limit` 默认 **200**、最大 **1000**；`cursor` 首次可不填。
- **返回条数可能少于 `limit`**：官方会过滤「会议发起者不在应用可见范围」的记录 ⇒ 条数不足不代表数据到底，**判停只看 `has_more`**。
- 记录里的 `userid` 语义有一处例外：**企业外部成员作为首位参与者进入预约会议时，该字段是该预约会议的创建者 userid**。
- 发起者身份与时间属**成员行为数据**：示例用假值，聚合结果不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ 99651 | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingStatisticsService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingStatisticsService`） |
| 不可注入 | `IWechatWorkMeetingStatisticsService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetMeetingStartListAsync` — 获取会议发起记录

**签名**

```csharp
[Post("/cgi-bin/meeting/statistics/get_start_list")]
Task<GetMeetingStartListResponse> GetMeetingStartListAsync(
    [Body] GetMeetingStartListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/statistics/get_start_list`（**查询类官方即 POST，且路由多一段 `statistics/`**） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/statistics/get_start_list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `GetMeetingStartListRequest` |

**请求体** `GetMeetingStartListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `type` | `Type` | `int?` | **是** | **1 发起成功 / 2 发起失败**（并发上限导致无法发起） |
| `begin_time` | `BeginTime` | `long?` | **是** | Unix 秒；与 `end_time` 跨度 **≤30 天**，区间**左闭右开** |
| `end_time` | `EndTime` | `long?` | **是** | Unix 秒 |
| `limit` | `Limit` | `int?` | 否 | 默认 **200**、最大 **1000** |
| `cursor` | `Cursor` | `string?` | 否 | 分页游标，首次调用可不填 |

**返回** `GetMeetingStartListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `next_cursor` | `NextCursor` | `string?` | 当前数据最后一个 key，下次调用带上即从该 key 往后拉 |
| `has_more` | `HasMore` | **`bool?`** | 是否还有数据待拉取 |
| `meeting_list` | `MeetingList` | `List<MeetingStartRecord>?` | 记录列表，**按时间从大到小排序**；因过滤「发起者不可见」，返回条数可能少于 `limit` |

`MeetingStartRecord`：`userid`（`string?`，发起者；**外部成员首位入会时为预约会议创建者的 userid**）+ `start_time`（`long?`，发起时间 Unix 秒）。

**调用示例**（一次翻页拉完某窗口的失败记录）

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
var statistics = provider.GetRequiredService<IWechatWorkInternalMeetingStatisticsService>();

// ④ 翻页：跨度必须 ≤30 天、区间左闭右开；判停只看 has_more（条数少于 limit 不代表到底）
long beginAt = DateTimeOffset.Now.AddDays(-14).ToUnixTimeSeconds();
long endAt = DateTimeOffset.Now.ToUnixTimeSeconds();
string? cursor = null;
long total = 0;
while (true)
{
    var page = await statistics.GetMeetingStartListAsync(new GetMeetingStartListRequest
    {
        Type = 2,                 // 1 发起成功 / 2 发起失败
        BeginTime = beginAt,
        EndTime = endAt,
        Limit = 1000,             // 默认 200、最大 1000
        Cursor = cursor,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/meeting/statistics/get_start_list");

    total += page.MeetingList?.Count ?? 0;   // 记录含成员身份：只累计数，不落逐条明细
    if (page.HasMore != true) break;
    cursor = page.NextCursor;
}
logger.LogInformation("近 14 天发起失败记录共 {Total} 条", total);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **路由多一段 `statistics/`** | `/cgi-bin/meeting/statistics/get_start_list`（官方原文如此，MT1 专项断言） | 按邻域惯例写成 `/cgi-bin/meeting/get_start_list` → 路由 404 |
| `type` 是**记录类型**而非会议类型 | 1 成功 / 2 失败（并发上限）；与 `GetMeetingInfoResponse.meeting_type`（0/1/2/3/5/6）毫无关系 | 混用两套枚举 |
| **左闭右开 + ≤30 天** | `begin_time` ≤ t < `end_time`，跨度上限 30 天（本域另一处 31/90/180 天，**不可互相借用**） | 按月统计时窗口重叠或超限报错 |
| 条数少于 `limit` 不等于到底 | 官方过滤「发起者不在可见范围」的记录 ⇒ 唯一判停是 `has_more` | 提前停止翻页，漏数据 |
| `has_more` 是 `bool?` | 与 Mail 域 `get_mail_list`（数值 0/1）形态不同 ⇒ 按各自 DTO 注释取用 | 按数值比较 `== 1` 永不成立 |
| `userid` 语义有例外 | 外部成员作为首位参与者进入预约会议时，返回的是**该预约会议创建者**的 userid | 把该条当成外部成员自己发起的会议 |
| 记录按时间**倒序** | 「从大到小排序」⇒ 增量拉取时以窗口而非「最后一条时间」做续点 | 用最后一条时间当起点导致漏拉 |
| 官方即 POST | 查询语义仍走 POST + 请求体（全域 105 条路由仅 `layout/list_template` 是 GET） | 改 GET → 打坏 MT1 与 G5 |
| 本族无参会与质量数据 | 只给发起记录；参会人明细 / 健康度 / 录制在其它族 | 以为统计族能给全量报表 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验跨度与必填**、**不做翻页状态管理**、**不做本地聚合与限流**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仍要按 `has_more` 决定续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`type` 非法、时间跨度超限、`limit` 越界、`cursor` 失效等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 99651 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无任何 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| 「发起失败」的失败原因码 | `type=2` 只区分成功/失败，**不返回失败原因**（除注释所述「并发上限」这一种）⇒ 其它失败原因不可归因 | 官方 99651 |
| 记录保留窗口 | 注释未记最多可回溯多久（只给了单次跨度 ≤30 天） | 官方 99651 |
| `cursor` 时效 | 未记游标有效期与失效后的行为 | 官方 99651 |
| 会议维度的统计面 | 本族不返回 `meetingid`，无法与会议详情 join ⇒ 是否有其它统计端点未核验 | 官方会议统计章节 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 3 节逐端点时间跨度对照、第 9 节批次上限表、第 8 节动词分布）
- 同域近邻：[Meeting.md](./Meeting.md)（预约会议主干与 `meeting_type` 枚举）· [MeetingAdvanced.md](./MeetingAdvanced.md)（实时参会列表 #12 / 参会列表 #13 / 健康度 #19——本族缺的那一块）
- 易混的「统计」面：录制统计在 [MeetingRecording.md](./MeetingRecording.md)（`record/get_statistics`，跨度 ≤31 天）
- 官方页：[获取会议发起记录（99651）](https://developer.work.weixin.qq.com/document/path/99651)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
