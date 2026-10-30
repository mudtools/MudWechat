# 企业直播 · 直播管理与观看统计

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Living` 域 · 直播管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkLivingService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Living/IWechatWorkLivingService.cs)（父，承载全部 9 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Living/IWechatWorkLivingService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Living/IWechatWorkLivingService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Living/IWechatWorkLivingService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Living/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Living/)（19 型，`SerializerClassName = Living` → `LivingJsonContext`；字段名由 `LivingDataModels_ShouldLockOfficialContractTraps` 逐个钉死） |
| 契约守卫 | [WechatLivingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLivingContractGuards.cs)（`LivingEndpoints_ShouldMatchOfficialRoutes` 9 支路由 / `LivingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces` 三类等宽 + 三空标记 / `LivingTokenBinding_ShouldBeAccessTokenInjectedViaQuery` / `LivingDataModels_ShouldBeRegisteredInJsonContext` / `LivingDataModels_ShouldLockOfficialContractTraps`）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建 [93637](https://developer.work.weixin.qq.com/document/path/93637) · 取消 93638 · 详情 93635 · 修改 93640 · 观看凭证 93641 · 观看明细 93636 · 成员直播 ID 93634 · 删除回放 93874 · 小程序商城观众 94442（第三方与代开发同页：93717/93718/93715/93720/93721/93716/93714/93719/94578 与 96837/96838/96835/96839/96840/96836/96834/96841/96843） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（详情应答 `LivingDetail` 全字段读取 + 观看明细续拉编排））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：五组守卫断言 + 接口与 DTO XML 注释（含守卫类 remarks 的官方反直觉清单）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Living/README.md)· 官方：本批未重新取文 |

## 1. 这个接口族做什么

企业微信「直播」的服务端管理面，9 支端点分四层：

| 层 | 端点 | 用途 |
|---|---|---|
| 预约生命周期 | E1 `living/create` · E2 `living/modify` · E3 `living/cancel` | 建 / 改 / 取消预约直播 |
| 详情与清单 | E7 `get_living_info`（**GET**）· E6 `get_user_all_livingid` | 看单个直播详情、列某成员的全部直播 |
| 观看与统计 | E8 `get_watch_stat` · E9 `get_living_share_info` | 观看人员明细（企业成员 + 外部成员）、小程序商城回传的观众/邀请人 |
| 出口与清理 | E5 `get_living_code` · E4 `delete_replay_data` | 换微信侧观看凭证、删回放 |

形态：官方对三类应用开放**完全一致**的 9 支 ⇒ 全收敛父接口 + 三个零差异空标记子接口。

**本域只做「预约与管理」**：真正的开播 / 观看动作靠 `livingid` 走小程序接口或 JS-SDK「进入直播」，不在本 SDK 的 HTTP 面内。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**上课直播/直播 - 可调用接口的应用**」中；第三方 / 代开发须具有「**直播**」权限。
- **全域只有一道所有权闸门：只能获取 / 操作「本应用创建」的直播**（E2/E3/E4/E5/E6/E7/E8/E9 全部受此约束）⇒ 换应用就看不到同一批直播。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 状态闸门：**E2 修改与 E3 取消只允许作用于「预约状态」的直播 id**（已开始 / 已结束的都改不动）。
- 内容量纲（E1）：标题 `theme` **≤20 个 utf8 字符**；简介 `description` **≤100 个 utf8 字符**，且**仅对通用直播 / 小班课 / 大班课 / 企业培训生效**（活动直播的简介走 `activity_detail.description`）；`type` **1 小班课 / 2 大班课仅 k12 学校和 IT 行业可发起**；活动直播附图 `activity_detail.image_list` **最多 5 张，超过取前五张**。
- E1 的 `activity_cover_mediaid` / `activity_share_mediaid` / `activity_detail.image_list` 都是 **mediaId**，须由「上传临时素材」产生（见 [Media.md](../Media/Media.md)）。
- E5 的 `living_code`：**5 分钟内可重复使用、仅能在微信上使用**；唤起方式为微信 H5 开放标签（`username` 固定 `gh_25e071b83ee0`）或直播小程序（`appId` 固定 `wx7424030d69bde86e`，路径 `pages/watch/index?living_code=...`，回放页再追加 `&replay=1`）。
- E9 的 `ww_share_code`：**五分钟内有效**，且**跳转的小程序需与企业有绑定关系**。
- 观看明细（E8）与观众身份（E9）含**成员 userid、外部人员 external_userid / 昵称、观看时长、是否评论、是否连麦、邀请人**，全部属个人数据：示例用假值，**统计结果不要整体落日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E9 | ✅ | ✅ | ✅ |

三类应用的路由、动词、请求面完全一致（差别只在令牌归属与企业权限勾选项）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkLivingService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **9** | 否 |
| `IWechatWorkInternalLivingService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyLivingService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderLivingService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Living`，装配入口 `AddLivingApi()`（模块枚举 `WechatModule.Living`）；子接口 `InheritedFrom = nameof(WechatWorkLivingService)`。

## 4. 端点

### E1. `CreateLivingAsync` — 创建预约直播

```csharp
[Post("/cgi-bin/living/create")]
Task<CreateLivingResponse> CreateLivingAsync(
    [Body] CreateLivingRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`CreateLivingRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `anchor_userid` | `AnchorUserid` | `string?` | **是** | 直播发起者（主播）userid |
| `theme` | `Theme` | `string?` | **是** | 直播标题，**≤20 个 utf8 字符** |
| `living_start` | `LivingStart` | `long?` | **是** | 开播时间 Unix 秒 |
| `living_duration` | `LivingDuration` | `int?` | **是** | 时长（秒） |
| `type` | `Type` | `int?` | 否 | **0 通用直播 / 1 小班课 / 2 大班课 / 3 企业培训 / 4 活动直播**，默认 0；**1、2 仅 k12 与 IT 行业** |
| `description` | `Description` | `string?` | 否 | 简介 ≤100 个 utf8 字符；**活动直播不生效**（走 `activity_detail.description`） |
| `remind_time` | `RemindTime` | `int?` | 否 | 开播前多久提醒（相对 `living_start` 的秒数），默认 0 |
| `agentid` | `Agentid` | `int?` | 否 | **仅旧的第三方多应用套件**需填 |
| `activity_cover_mediaid` | `ActivityCoverMediaid` | `string?` | 否 | 活动直播：直播间封面图 mediaId |
| `activity_share_mediaid` | `ActivityShareMediaid` | `string?` | 否 | 活动直播：分享卡片图 mediaId |
| `activity_detail` | `ActivityDetail` | `LivingActivityDetail?` | 否 | 活动直播：`description` + `image_list[]`（mediaId，**最多 5 张，超出取前五**） |

响应：`livingid`（**字段名官方即无下划线**，守卫逐个钉死）。该 id 可继续用于小程序 / JS-SDK 的「进入直播」接口。

### E2. `ModifyLivingAsync` — 修改预约直播

```csharp
[Post("/cgi-bin/living/modify")]
Task<WechatWorkResponse> ModifyLivingAsync(
    [Body] ModifyLivingRequest request, CancellationToken cancellationToken = default);
```

- 请求：`livingid`（必填）+ `theme` / `living_start` / `living_duration` / `type` / `description` / `remind_time`。
- **仅可改本应用创建的、且处于预约状态的直播**；本端点注释给的量纲是**字节**：标题 **≤60 字节**、简介 **≤300 字节**（与 E1 的「20 / 100 个 utf8 字符」表述并存，见 §5 与 §7）。
- 响应仅信封。

### E3. `CancelLivingAsync` — 取消预约直播

```csharp
[Post("/cgi-bin/living/cancel")]
Task<WechatWorkResponse> CancelLivingAsync(
    [Body] CancelLivingRequest request, CancellationToken cancellationToken = default);
```

- 请求：`livingid`（必填）；**仅可取消本应用创建的、预约状态下的直播**。

### E4. `DeleteReplayDataAsync` — 删除直播回放

```csharp
[Post("/cgi-bin/living/delete_replay_data")]
Task<WechatWorkResponse> DeleteReplayDataAsync(
    [Body] DeleteLivingReplayDataRequest request, CancellationToken cancellationToken = default);
```

- 请求：`livingid`（必填）；**仅可删除本应用自己创建的直播的回放**。
- 删除后详情里的 `replay_status` 变为 **2 回放已删除**。

### E5. `GetLivingCodeAsync` — 获取微信观看直播凭证

```csharp
[Post("/cgi-bin/living/get_living_code")]
Task<GetLivingCodeResponse> GetLivingCodeAsync(
    [Body] GetLivingCodeRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `livingid` | `Livingid` | `string?` | **是** | 直播 id |
| `openid` | `Openid` | `string?` | **是** | **微信用户**的 openid（注意不是 userid） |

响应：`living_code`（`string?`）——**5 分钟内可重复使用、仅能在微信上使用**；配合 §2.2 的固定 `username` / `appid` 唤起直播小程序观看直播或回放（回放路径追加 `&replay=1`）。

### E6. `GetUserAllLivingIdAsync` — 获取成员直播 ID 列表

```csharp
[Post("/cgi-bin/living/get_user_all_livingid")]
Task<GetUserAllLivingIdResponse> GetUserAllLivingIdAsync(
    [Body] GetUserAllLivingIdRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | **是** | 企业成员 |
| `cursor` | `Cursor` | `string?` | 否 | 上次的 `next_cursor`；首次不填 |
| `limit` | `Limit` | `int?` | 否 | **建议 20；默认值与最大值都是 100** |

响应：`livingid_list[]` + `next_cursor`（**返回空字符串代表已是最后一页**）。
**只返回本应用创建的直播**；该路由与「家校沟通·上课直播域」是**官方同一端点、分文档承载**（两域各自声明，不互斥，见 §5）。

### E7. `GetLivingInfoAsync` — 获取直播详情

```csharp
[Get("/cgi-bin/living/get_living_info")]
Task<GetLivingInfoResponse> GetLivingInfoAsync(
    [Query("livingid")] string livingid, CancellationToken cancellationToken = default);
```

- **本域唯一 GET**（`livingid` 走 Query，无请求 DTO）；其余 8 支官方即 POST。
- 响应 `living_info`（`LivingDetail`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `theme` | `Theme` | `string?` | 直播主题 |
| `living_start` / `living_duration` | 同名 | `long?` / `int?` | 开播时间戳（秒）/ 时长（秒） |
| `status` | `Status` | `int?` | **0 预约中 / 1 直播中 / 2 已结束 / 3 已过期 / 4 已取消** |
| `reserve_start` / `reserve_living_duration` | 同名 | `long?` / `int?` | **预约**的开始时间与时长（与正式直播分开） |
| `description` | `Description` | `string?` | 描述 |
| `anchor_userid` / `main_department` | 同名 | `string?` / `int?` | 主播 userid / 主播**主部门** id |
| `viewer_num` / `comment_num` / `mic_num` | 同名 | `int?` | 观看总人数 / 评论数 / 连麦发言人数 |
| `online_count` / `subscribe_count` | 同名 | `int?` | **当前在线观看人数** / **直播预约人数** |
| `open_replay` | `OpenReplay` | `int?` | 是否开启回放：1 开启 / 0 关闭 |
| `replay_status` | `ReplayStatus` | `int?` | **仅 `open_replay = 1` 时返回**：0 生成成功 / 1 生成中 / **2 回放已删除** / 3 生成失败 |
| `type` | `Type` | `int?` | 0 通用 / 1 小班课 / 2 大班课 / 3 企业培训 / 4 活动直播 |
| `push_stream_url` | `PushStreamUrl` | `string?` | 推流地址——**仅活动直播且状态为待开播时返回** |

### E8. `GetWatchStatAsync` — 获取直播观看明细

```csharp
[Post("/cgi-bin/living/get_watch_stat")]
Task<GetLivingWatchStatResponse> GetWatchStatAsync(
    [Body] GetLivingWatchStatRequest request, CancellationToken cancellationToken = default);
```

- 请求：`livingid`（必填）+ `next_key`（**初次调用可填 `"0"`**，之后带上一次返回的 `next_key`）。
- 响应：`ending`（**0 还有更多需继续拉 / 1 已拉完**，**唯一判停依据**）+ `next_key` + `stat_info`：
  - `stat_info.users[]`（`LivingWatchStatUser`）＝ `userid` / `watch_time`（秒）/ `is_comment` 0/1 / `is_mic` 0/1 / `invitor_userid` / `invitor_external_userid`；
  - `stat_info.external_users[]`（`LivingWatchStatExternalUser`）＝ `external_userid` / **`type`（1 微信用户 / 2 企业微信用户）** / `name` / 同样的 `watch_time` / `is_comment` / `is_mic` / 邀请人两字段。
- **邀请人两字段仅「推广产品」直播支持**；邀请人指「观众首次进入直播时所用卡片 / 二维码对应的分享人」。

### E9. `GetLivingShareInfoAsync` — 获取跳转小程序商城的直播观众信息

```csharp
[Post("/cgi-bin/living/get_living_share_info")]
Task<GetLivingShareInfoResponse> GetLivingShareInfoAsync(
    [Body] GetLivingShareInfoRequest request, CancellationToken cancellationToken = default);
```

- 请求：`ww_share_code`（必填）——「推广产品」直播观众跳小程序商城时由 path 携带而来，**五分钟有效**，**跳转的小程序需与企业有绑定关系**。
- 响应：`livingid` + `viewer_userid`（观众是内部成员时）/ `viewer_external_userid`（非内部成员时）+ `invitor_userid` / `invitor_external_userid`（邀请人同理二选一）。

**调用示例**（E1 建直播 → E7 看详情并按状态分流 → E6 分页取成员直播 → E8 用 `ending` 判停的观看明细）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Living;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "live-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddLivingApi());

var living = provider.GetRequiredService<IWechatWorkInternalLivingService>();

// ① 创建预约直播：theme ≤20 个 utf8 字符；type 1/2（小/大班课）仅 k12 与 IT 行业可用
var created = await living.CreateLivingAsync(
    new CreateLivingRequest
    {
        AnchorUserid = "zhangsan",                              // 假值：主播
        Theme = "季度产品说明会",                                 // ≤20 个 utf8 字符
        LivingStart = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds(),
        LivingDuration = 3600,
        Type = 0,                                               // 0 通用直播
        Description = "示例直播简介",                            // ≤100 个 utf8 字符
        RemindTime = 300,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/living/create");
string livingId = created.Livingid ?? throw new InvalidOperationException("响应未返回 livingid");   // 官方字段名无下划线

// ② 详情：官方即 GET，livingid 走 Query；status 0 预约中 / 1 直播中 / 2 已结束 / 3 已过期 / 4 已取消
var detail = await living.GetLivingInfoAsync(livingId, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/living/get_living_info");

LivingDetail? info = detail.LivingInfo;
logger.LogInformation("直播 {LivingId} 状态 {Status}，主播 {Anchor}，预约 {Subscribe} 人、观看 {Viewer} 人",
    livingId, info?.Status, info?.AnchorUserid, info?.SubscribeCount, info?.ViewerNum);
// push_stream_url 仅活动直播且待开播时返回；replay_status 仅 open_replay=1 时返回
bool replayReady = info?.OpenReplay == 1 && info.ReplayStatus == 0;

// ③ 成员直播 ID 列表：cursor 分页，next_cursor 为空字符串即最后一页（limit 建议 20、上限 100）
string? cursor = null;
var livingIds = new List<string> { livingId };
do
{
    var page = await living.GetUserAllLivingIdAsync(
        new GetUserAllLivingIdRequest { Userid = "zhangsan", Cursor = cursor, Limit = 20 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/living/get_user_all_livingid");

    livingIds.AddRange(page.LivingidList ?? new List<string>());
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ④ 观看明细：只能用 ending 判停（0 还要拉 / 1 已拉完），next_key 初次可填 "0"
string nextKey = "0";
int memberWatchers = 0, externalWatchers = 0;
while (true)
{
    var stat = await living.GetWatchStatAsync(
        new GetLivingWatchStatRequest { Livingid = livingId, NextKey = nextKey }, cancellationToken);
    WechatWorkException.ThrowIfFailed(stat, "/cgi-bin/living/get_watch_stat");

    memberWatchers += stat.StatInfo?.Users?.Count ?? 0;
    externalWatchers += stat.StatInfo?.ExternalUsers?.Count ?? 0;

    if (stat.Ending == 1) break;          // 只认 ending，不能按列表是否为空判停
    nextKey = stat.NextKey ?? nextKey;
}
// 观看人、外部成员昵称、邀请人均为个人数据：只输出计数，不输出身份标识
logger.LogInformation("直播 {LivingId} 观看统计：企业成员 {Member} 人、外部成员 {External} 人",
    livingId, memberWatchers, externalWatchers);
```


## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| **两种翻页形态并存** | E6 用 `cursor` / `next_cursor`（**空字符串 = 最后一页**）；E8 用 `next_key` / `ending`（**只能靠 `ending` 判停**，初次 `next_key` 可填 `"0"`） | 用 `next_cursor` 的逻辑套 E8 → 死循环或提前停 |
| `livingid` 的拼写 | 官方字段名**无下划线**（`livingid` / `livingid_list`），守卫逐个钉死，别写成 `living_id` | 反序列化取不到 |
| `invitor_*` 的拼写 | 官方即 **`invitor_userid` / `invitor_external_userid`**（不是 `inviter`），**且仅「推广产品」直播支持** | 「纠正拼写」→ 取不到；普通直播里当必有字段 |
| E1 与 E2 的量纲表述 | E1 注释按「utf8 字符」给（20 / 100 字符），E2 注释按「字节」给（60 / 300 字节）⇒ **两处并存、未逐项核验**（见 §7） | 用一处上限去校验另一处 |
| `E5` 的 `openid` | 是**微信用户 openid**（观众在微信侧的身份），不是企业成员 userid | 传错主体 → 拿不到可用凭证 |
| `living_code` vs `ww_share_code` | 前者本域**签出**、用于在**微信里唤起直播小程序**（5 分钟、可重复、仅微信内）；后者由小程序 path **带回来**、用于**换取观众与邀请人身份**（5 分钟、需小程序与企业绑定） | 方向搞反：拿 code 去换身份、或拿 share_code 去唤起 |
| `reserve_*` vs `living_*` | 详情里 `living_start` / `living_duration` 是**正式直播**，`reserve_start` / `reserve_living_duration` 是**预约段** | 用预约时间算开播提醒 |
| `viewer_num` vs `online_count` | 前者是**累计观看总人数**，后者是**当前在线观看人数** | 报表口径混用 |
| `replay_status` 的可缺性 | **仅 `open_replay = 1` 时返回**；E4 删除回放后应变为 **2 回放已删除** | 未开回放时读该字段判分支 |
| `push_stream_url` 的极窄条件 | **仅活动直播（type=4）且状态为待开播**时返回 | 通用直播里读它 → 恒 null |
| 与家校沟通域的同路由 | `get_user_all_livingid` 与 `delete_replay_data` 在「家校沟通·上课直播域」（School 模块）**分文档承载同一端点** ⇒ 本域按直播文档页独立声明接口与 DTO，**两域同路由不互斥** | 误以为要合并成一处声明、或跨域复用 DTO |
| 所有权闸门 | **只能获取 / 操作本应用创建的直播**（E2–E9 全线），故 E6 列出的也只是本应用建的 | 拿别的应用建的 livingid 来查 |
| 状态闸门 | 修改与取消**只作用于预约状态** | 对直播中 / 已结束的直播重试 |
| `agentid` 字段 | 仅**旧的第三方多应用套件**需要填 | 新接入误填导致参数冲突 |
| 动词分布 | E7 详情是 **GET**，E6/E8/E9 三支查询是 **POST** | 「查询都 GET」→ 打坏路由守卫与 G5 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200，须 `WechatWorkException.ThrowIfFailed`）、**不做本地必填与量纲校验**、**不做翻页状态管理**（`cursor` / `next_key` 由宿主携带）、**不校验所有权与直播状态**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功才拿得到 `livingid`；E6/E8 成功仍要按翻页信号决定是否续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`livingid` 非本应用创建、非预约状态被改 / 取消、`living_code` 或 `ww_share_code` 过期、`type` 与行业不符（小 / 大班课）等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 标题 / 简介的量纲口径 | E1 注释写「20 / 100 个 utf8 字符」、E2 注释写「60 / 300 字节」⇒ **两处并存、无裁决依据，未逐项核验** | 官方 93637 / 93640 参数表；结论确定后同批修正注释 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方各页 + 全局错误码页 |
| 端点频率限制 | 注释未给数值 ⇒ 未逐项核验 | 官方全局频率附录 |
| 开播 / 观看动作的接口面 | 注释只说「可通过 `livingid` 调用进入直播接口（小程序接口与 JS-SDK 接口）」，该面**不在本 SDK 的 HTTP 端点内** ⇒ 本域不建模 | 官方小程序 / JS-SDK 文档 |
| 「推广产品」直播的创建入口 | E8 / E9 多处标注「仅推广产品直播支持」，但创建请求的 `type` 枚举里**没有该类型** ⇒ 其创建与管理面未核验 | 官方直播「推广产品」文档 |
| `living_code` 唤起参数的现行有效性 | 固定 `username` / `appId` 与路径来自接口注释留档，官方是否仍用同一小程序未在本批复测 | 官方 93641 |
| 回放生成时长与有效期 | `replay_status = 1 生成中` 的等待时长、回放保留期均未记录 | 官方 93874 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Living/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Living/README.md)（9 支端点速览、四接口令牌表、逐端点契约明细与 DTO 表）
- 同线跨域：[Media.md](../Media/Media.md)（活动直播封面 / 分享图 / 附图的 mediaId 来源，**并含 multipart 上传腿的现行缺陷说明**）· 家校沟通「上课直播」域（School 模块，与本域共享 `get_user_all_livingid` / `delete_replay_data` 两条路由）
- 跨域：主播 userid 与主部门 id 的解析在通讯录（[Users.md](../Contacts/Users.md) / [Departments.md](../Contacts/Departments.md)）；外部成员 `external_userid` 的企业维度资料在 [Customer.md](../ExternalContact/Customer.md)
- 官方页：[创建预约直播（93637）](https://developer.work.weixin.qq.com/document/path/93637) · [获取直播详情（93635）](https://developer.work.weixin.qq.com/document/path/93635) · [获取直播观看明细（93636）](https://developer.work.weixin.qq.com/document/path/93636) · [获取微信观看直播凭证（93641）](https://developer.work.weixin.qq.com/document/path/93641) · [获取小程序商城观众信息（94442）](https://developer.work.weixin.qq.com/document/path/94442)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
