# 家校上课直播 · 详情与观看 / 未观看统计

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 上课直播族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolLivingService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Living/IWechatWorkSchoolLivingService.cs)（父，承载全部 5 支）＋ 自建 / 第三方 / 代开发三个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Living/IWechatWorkInternalSchoolLivingService.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Living/IWechatWorkThirdPartySchoolLivingService.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Living/IWechatWorkProviderSchoolLivingService.cs)） |
| DTO 源文件 | [DataModels/School/Living/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/Living/)（本族 22 个文件 / 22 型，`SerializerClassName = Living` → `LivingJsonContext`，命名空间 `...DataModels.School.Living`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1g 5 路由均落 `/cgi-bin/school/living/` / SCH2 父 5 + 三子 0 / SCH3 令牌绑定 / SCH4 Living 22 型）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 直播详情 [93740](https://developer.work.weixin.qq.com/document/path/93740) / [93857](https://developer.work.weixin.qq.com/document/path/93857) / [97128](https://developer.work.weixin.qq.com/document/path/97128)；观看统计 [93741](https://developer.work.weixin.qq.com/document/path/93741) / [93858](https://developer.work.weixin.qq.com/document/path/93858) / [97129](https://developer.work.weixin.qq.com/document/path/97129)；未观看统计 [93742](https://developer.work.weixin.qq.com/document/path/93742) / [93859](https://developer.work.weixin.qq.com/document/path/93859) / [97130](https://developer.work.weixin.qq.com/document/path/97130)；观看 V2 [95793](https://developer.work.weixin.qq.com/document/path/95793) / [95799](https://developer.work.weixin.qq.com/document/path/95799) / [97132](https://developer.work.weixin.qq.com/document/path/97132)；未观看 V2 [95795](https://developer.work.weixin.qq.com/document/path/95795) / [95800](https://developer.work.weixin.qq.com/document/path/95800) / [97133](https://developer.work.weixin.qq.com/document/path/97133)（依次自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1g / SCH2 / SCH3 / SCH4 断言 + 5 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

老师在客户端发起「上课直播」后，应用侧通过这一族读直播详情、以及「谁看了 / 谁没看」的统计：

| 端点 | 用途 | 分页 |
|---|---|---|
| E1 `school/living/get_living_info` | 直播详情（主题 / 开播时间 / 时长 / 主播 / 范围 / 观看评论数 / 回放开关） | 无 |
| E2 `school/living/get_watch_stat` | 观看统计 V1（学生 + 游客） | `next_key` + `ending` |
| E3 `school/living/get_unwatch_stat` | 未观看统计 V1（仅学生） | `next_key` + `ending` |
| E4 `school/living/get_watch_stat_v2` | 观看统计 V2（学生 + 家长 + 游客） | `next_cursor` + `has_more` |
| E5 `school/living/get_unwatch_stat_v2` | 未观看统计 V2（学生 + 家长） | `next_cursor` + `has_more` |

**不该用本族的场景**：

- **「获取老师直播 ID 列表」**（`/cgi-bin/living/get_user_all_livingid`）与**「删除直播回放」**（`/cgi-bin/living/delete_replay_data`）归**「直播」域** `IWechatWorkLivingService` 单一所有者——官方用两套文档承载同一端点，本域不重复声明。
- 发起 / 结束直播是客户端能力，本域只有读取端点。
- 学校通知的发送在消息推送域；家长范围在 [Basic.md](./Basic.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` 三选一 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（三类同键） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 本接口全部端点位于 **`/cgi-bin/school/living/`** 段（家校专属），不是通用直播域的 `/cgi-bin/living/`。
- **只能获取本应用创建的直播**：跨应用 / 跨学校的 `livingid` 一律不可见。
- 权限口径：E1~E5 自建应用须配置到「上课直播 - 可调用接口的应用」和「家长可以使用的应用」中；第三方 / 代开发须同时具有「直播」和「家校沟通」权限。
- E1 的 `push_stream_url` 只在「活动直播 + 待开播」时返回，属推流凭据，**不得落日志**。
- 学生 / 家长 `userid`、游客 `nickname`、观看时长与进出时间都是可关联到个人的数据：示例一律假值，日志只记条数。
- 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 直播详情 | ✅ | ✅ | ✅ | 三页齐全 |
| E2 观看统计 V1 | ✅ | ✅ | ✅ | 三页齐全 |
| E3 未观看统计 V1 | ✅ | ✅ | ✅ | 三页齐全 |
| E4 观看统计 V2 | ✅ | ✅ | ✅ | 三页齐全 |
| E5 未观看统计 V2 | ✅ | ✅ | ✅ | 三页齐全 |

> 三类应用端点完全一致，三个子接口都是**零端点空标记**（SCH2 锁死）；不会出现「某个子接口少几支」的情况。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolLivingService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 5 | 否 |
| `IWechatWorkInternalSchoolLivingService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolLivingService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderSchoolLivingService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `GetLivingInfoAsync` · E2 `GetWatchStatAsync` · E3 `GetUnwatchStatAsync` · E4 `GetWatchStatV2Async` · E5 `GetUnwatchStatV2Async`。
- DTO 落 `DataModels.School.Living` 命名空间（守卫以别名引用 `LivingJsonContext`），不随主域 `SchoolJsonContext`。

---

## 4. 端点

### E1. `GetLivingInfoAsync` — 获取直播详情

```csharp
[Get("/cgi-bin/school/living/get_living_info")]
Task<LivingGetLivingInfoResponse> GetLivingInfoAsync(
    [Query("livingid")] string livingId,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/living/get_living_info` |
| 凭据 | Query `access_token` + Query `livingid` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `livingId` | `livingid` | `string` | 是 | 直播 id，走 **Query**（SCH1g 专项断言） |

**响应参数**（`LivingGetLivingInfoResponse` → `living_info`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `theme` | `LivingInfo.Theme` | `string?` | 直播主题 |
| `living_start` | `LivingInfo.LivingStart` | `long?` | 开播时间戳 |
| `living_duration` | `LivingInfo.LivingDuration` | `long?` | 直播时长 |
| `anchor_userid` | `LivingInfo.AnchorUserId` | `string?` | 主播 userid（个人标识） |
| `living_range.partyids` | `LivingInfo.LivingRange.PartyIds` | `List<long>?` | 直播范围部门 id（**`long`**） |
| `living_range.group_names` | `LivingInfo.LivingRange.GroupNames` | `List<string>?` | 直播范围群名 |
| `viewer_num` | `LivingInfo.ViewerNum` | `long?` | 观看人数 |
| `comment_num` | `LivingInfo.CommentNum` | `long?` | 评论数 |
| `open_replay` | `LivingInfo.OpenReplay` | `int?` | 是否开启回放 |
| `push_stream_url` | `LivingInfo.PushStreamUrl` | `string?` | 推流地址（**仅活动直播 + 待开播时返回**） |
| `online_count` | `LivingInfo.OnlineCount` | `long?` | 在线人数 |
| `subscribe_count` | `LivingInfo.SubscribeCount` | `long?` | 订阅人数 |

**本端点注意事项**

- `partyids` 是 `List<long>`，与部门 `id` 的 `int` **不同宽**，别合并建模。
- `push_stream_url` 是推流凭据：只交给推流端，不落日志 / 不进异常消息。
- 未开播 / 已结束的直播，部分统计字段（在线 / 订阅）可能缺失，属正常形态。

### E2. `GetWatchStatAsync` — 获取观看直播统计（V1）

```csharp
[Post("/cgi-bin/school/living/get_watch_stat")]
Task<LivingGetWatchStatResponse> GetWatchStatAsync(
    [Body] LivingGetWatchStatRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/living/get_watch_stat`（**统计类官方即 POST**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`LivingGetWatchStatRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `LivingId` | `livingid` | `string`（非可空，默认 `""`） | 是 | 直播 id |
| `NextKey` | `next_key` | `string?` | 否 | 翻页 key，首屏不传，后续传响应值 |

**响应参数**（`LivingGetWatchStatResponse`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `ending` | `Ending` | `int?` | **0 = 还有更多、1 = 已拉完** |
| `next_key` | `NextKey` | `string?` | 下一页 key |
| `stat_infoes.students` | `StatInfoes.Students` | `List<LivingWatchStudent>?` | 观看学生 |
| `stat_infoes.visitors` | `StatInfoes.Visitors` | `List<LivingVisitor>?` | 观看游客 |

`LivingWatchStudent` 元素：`student_userid` / `parent_userid` / `partyids` (`List<long>`) / `watch_time` / `enter_time` / `leave_time` / `is_comment`。
`LivingVisitor` 元素：`nickname` / `watch_time` / `enter_time` / `leave_time` / `is_comment`。

**本端点注意事项**

- 容器字段名是 **`stat_infoes`**（官方原文拼写，不是 `stat_infos`），`[JsonPropertyName]` 已钉住。
- V1 的**学生条目自带 `parent_userid`**；V2 把家长拆成独立列表，学生条目**没有** `parent_userid`（见 E4）。
- `nickname` 是游客个人标识，日志只记人数。

### E3. `GetUnwatchStatAsync` — 获取未观看直播统计（V1）

```csharp
[Post("/cgi-bin/school/living/get_unwatch_stat")]
Task<LivingGetUnwatchStatResponse> GetUnwatchStatAsync(
    [Body] LivingGetUnwatchStatRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/living/get_unwatch_stat` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`LivingGetUnwatchStatRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `LivingId` | `livingid` | `string`（非可空，默认 `""`） | 是 | 直播 id |
| `NextKey` | `next_key` | `string?` | 否 | 翻页 key |

**响应参数**（`LivingGetUnwatchStatResponse`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `ending` | `Ending` | `int?` | **0 = 还有更多、1 = 已拉完** |
| `next_key` | `NextKey` | `string?` | 下一页 key |
| `stat_info.students` | `StatInfo.Students` | `List<LivingUnwatchStudent>?` | 未观看学生 |

`LivingUnwatchStudent` 元素：`student_userid` / `parent_userid` / `partyids` (`List<long>`)。
（未观看条目**没有**观看时长字段——他们没看，自然没有。）

**本端点注意事项**

- **只统计家长已关注「学校通知」的学生**：未关注家庭不会出现在未观看名单里，别把「名单为空」当成全员观看。
- 容器字段是 `stat_info`（单数），与 E2 的 `stat_infoes` 不同形。

### E4. `GetWatchStatV2Async` — 获取观看直播统计 V2

```csharp
[Post("/cgi-bin/school/living/get_watch_stat_v2")]
Task<LivingGetWatchStatV2Response> GetWatchStatV2Async(
    [Body] LivingGetWatchStatV2Request request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/living/get_watch_stat_v2` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`LivingGetWatchStatV2Request`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `LivingId` | `livingid` | `string`（非可空，默认 `""`） | 是 | 直播 id |
| `NextCursor` | `next_cursor` | `string?` | 否 | 翻页游标（V2 用 `cursor`，不是 `key`） |

**响应参数**（`LivingGetWatchStatV2Response`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `int?` | **1 = 还有更多、0 = 已拉完**（与 V1 `ending` 语义相反） |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `stat_info.students` | `StatInfo.Students` | `List<LivingWatchStudentV2>?` | 观看学生 |
| `stat_info.parents` | `StatInfo.Parents` | `List<LivingWatchParent>?` | 观看家长（V2 新增） |
| `stat_info.visitors` | `StatInfo.Visitors` | `List<LivingVisitor>?` | 观看游客 |

元素字段：

| 类型 | 字段 |
|---|---|
| `LivingWatchStudentV2` | `student_userid` / `partyids` / `watch_time` / `enter_time` / `leave_time` / `is_comment`（**无 `parent_userid`**） |
| `LivingWatchParent` | `parent_userid` / `student_userid` / `partyids` / `watch_time` / `enter_time` / `leave_time` / `is_comment` |
| `LivingVisitor` | `nickname` / `watch_time` / `enter_time` / `leave_time` / `is_comment` |

**本端点注意事项**

- **`has_more` 与 V1 `ending` 语义相反**：V1 `1` = 完了，V2 `1` = 还有——翻页循环条件写反就是死循环或只取一页。
- V2 把家长单独列出：统计「家长观看」要看 `stat_info.parents`，不能从学生条目里找 `parent_userid`（V2 已移除该字段）。
- 同一场直播 V1 / V2 都能查，但字段结构不同；新代码优先用 V2。

### E5. `GetUnwatchStatV2Async` — 获取未观看直播统计 V2

```csharp
[Post("/cgi-bin/school/living/get_unwatch_stat_v2")]
Task<LivingGetUnwatchStatV2Response> GetUnwatchStatV2Async(
    [Body] LivingGetUnwatchStatV2Request request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/living/get_unwatch_stat_v2` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`LivingGetUnwatchStatV2Request`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `LivingId` | `livingid` | `string`（非可空，默认 `""`） | 是 | 直播 id |
| `NextCursor` | `next_cursor` | `string?` | 否 | 翻页游标 |

**响应参数**（`LivingGetUnwatchStatV2Response`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `int?` | **1 = 还有更多、0 = 已拉完** |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `stat_info.students` | `StatInfo.Students` | `List<LivingUnwatchStudentV2>?` | 未观看学生 |
| `stat_info.parents` | `StatInfo.Parents` | `List<LivingUnwatchParent>?` | 未观看家长（V2 新增） |

元素字段：

| 类型 | 字段 |
|---|---|
| `LivingUnwatchStudentV2` | `student_userid` / `partyids`（**无 `parent_userid`**） |
| `LivingUnwatchParent` | `parent_userid` / `student_userid` / `partyids` |

**本端点注意事项**

- 官方口径：**学生及其家长必须已关注「学校通知」才纳入统计**——未关注家庭不在名单里。
- `has_more` 语义与 E4 相同（`1` = 继续），与 E3 的 `ending` 相反。

**调用示例**（详情 → V1 观看翻页 → V2 观看翻页 → V2 未观看翻页）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.School.Living;

// builder / provider / config / logger / livingId / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "school-a";
    o.AppType = WechatAppType.Internal;          // 第三方 / 代开发换 AppType 并切企业作用域
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

var living = provider.GetRequiredService<IWechatWorkInternalSchoolLivingService>();

// E1：只能查本应用创建的直播；push_stream_url 属推流凭据，不落日志
var detail = await living.GetLivingInfoAsync(livingId, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/school/living/get_living_info");

LivingInfo? info = detail.LivingInfo;
logger.LogInformation("直播《{Theme}》开播 {Start}，观看 {Viewer} 人，评论 {Comment} 条",
    info?.Theme, info?.LivingStart, info?.ViewerNum, info?.CommentNum);

// E2：V1 观看统计，ending 0 继续 / 1 已完；容器字段是 stat_infoes
string? nextKey = null;
do
{
    var watch = await living.GetWatchStatAsync(new LivingGetWatchStatRequest
    {
        LivingId = livingId,
        NextKey = nextKey,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(watch, "/cgi-bin/school/living/get_watch_stat");

    int studentCount = watch.StatInfoes?.Students?.Count ?? 0;
    int visitorCount = watch.StatInfoes?.Visitors?.Count ?? 0;
    // 学生 userid / 游客 nickname 属个人标识，只记条数
    logger.LogInformation("V1 观看：学生 {Student} 条 / 游客 {Visitor} 条", studentCount, visitorCount);

    nextKey = watch.NextKey;
    if (watch.Ending == 1) break;
} while (!string.IsNullOrEmpty(nextKey));

// E4：V2 观看统计，has_more 1 继续 / 0 已完（与 V1 语义相反）
string? nextCursor = null;
do
{
    var watchV2 = await living.GetWatchStatV2Async(new LivingGetWatchStatV2Request
    {
        LivingId = livingId,
        NextCursor = nextCursor,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(watchV2, "/cgi-bin/school/living/get_watch_stat_v2");

    int students = watchV2.StatInfo?.Students?.Count ?? 0;
    int parents = watchV2.StatInfo?.Parents?.Count ?? 0;      // V2 家长单独成列
    int visitors = watchV2.StatInfo?.Visitors?.Count ?? 0;
    logger.LogInformation("V2 观看：学生 {Student} / 家长 {Parent} / 游客 {Visitor}",
        students, parents, visitors);

    nextCursor = watchV2.NextCursor;
    if (watchV2.HasMore != 1) break;
} while (!string.IsNullOrEmpty(nextCursor));

// E5：V2 未观看（只含已关注「学校通知」的家庭）
string? unwatchCursor = null;
do
{
    var unwatch = await living.GetUnwatchStatV2Async(new LivingGetUnwatchStatV2Request
    {
        LivingId = livingId,
        NextCursor = unwatchCursor,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(unwatch, "/cgi-bin/school/living/get_unwatch_stat_v2");

    logger.LogInformation("V2 未观看：学生 {Student} / 家长 {Parent}",
        unwatch.StatInfo?.Students?.Count ?? 0, unwatch.StatInfo?.Parents?.Count ?? 0);

    unwatchCursor = unwatch.NextCursor;
    if (unwatch.HasMore != 1) break;
} while (!string.IsNullOrEmpty(unwatchCursor));
```

> 注：示例省略了 E3（V1 未观看）——其调用形态与 E2 完全一致（`next_key` + `ending`），只是方法名换成 `GetUnwatchStatAsync`、容器字段换成 `stat_info`。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 翻页终态 | V1 `ending`：0 继续 / 1 完；V2 `has_more`：1 继续 / 0 完 | 直接照抄 → 死循环或只取一页 |
| 翻页键名 | V1 `next_key`；V2 `next_cursor` | 传错字段 → 永远第一页 |
| 容器字段 | E2 是 `stat_infoes`（官方拼写）；其余是 `stat_info` | 按 `stat_info` 读 E2 → 永远空 |
| V1 / V2 学生条目 | V2 学生**没有** `parent_userid`，家长在独立 `parents` 列表 | 从 V2 学生里找家长 → 取不到 |
| 路由段 | 全部在 `/cgi-bin/school/living/`；通用直播在 `/cgi-bin/living/` | 混段 → 404 |
| 应用隔离 | 只能查本应用创建的直播 | 拿别处 livingid → 报错 |
| 未观看名单口径 | 只含已关注「学校通知」的家庭 | 把空名单当「全员观看」 |
| `partyids` 宽度 | `List<long>`，与部门 `id` 的 `int` 不同 | 复用 `List<int>` → 反序列化失败 |
| `push_stream_url` | 仅活动直播 + 待开播返回，属推流凭据 | 落日志 → 凭据泄露 |
| 游客 `nickname` | 个人标识 | 写入日志 → 合规风险 |
| 单所有者端点 | `living/get_user_all_livingid` / `living/delete_replay_data` 在「直播」域 | 在本域找 → 零声明 |

SDK 侧通用约束：不自动判错、不自动翻页、不缓存 `next_key` / `next_cursor`；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本域注释未留档**专属**业务错误码：直播不存在、跨应用访问、`next_key` 失效等失败均以官方业务码表达，码值未逐项核验。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 时间字段单位 | `living_start` / `watch_time` / `enter_time` / `leave_time` / `living_duration` 只标时间戳 / 时长，未注明秒或毫秒 ⇒ 未逐项核验 | 官方 93740 / 93741 |
| `next_key` / `next_cursor` 留存时长 | 翻页 key 有效期与并发安全未留档 ⇒ 未核验 | 官方 93741 / 95793 |
| 观看统计延迟 | 直播进行中统计是否实时、滞后多久 ⇒ 未核验 | 官方 93741 |
| V1 与 V2 数据口径差异 | 同一场直播 V1 / V2 人数是否严格一致 ⇒ 未核验 | 官方 95793 |
| `open_replay` / `push_stream_url` 生命周期 | 回放开放与推流地址的有效期 ⇒ 未核验 | 官方 93740 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Basic.md](./Basic.md) · [Setting.md](./Setting.md) · [User.md](./User.md) · [Department.md](./Department.md) · [Auth.md](./Auth.md) · [HealthReport.md](./HealthReport.md) · [ClassPay.md](./ClassPay.md)
- 邻域：「直播」域 `IWechatWorkLivingService`（老师直播 ID 列表、删除回放，单一所有者）
- 数据模型：[DataModels/School/Living/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/Living/)（22 型，独立 `LivingJsonContext`）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[直播详情 93740](https://developer.work.weixin.qq.com/document/path/93740) · [观看统计 93741](https://developer.work.weixin.qq.com/document/path/93741) · [未观看统计 93742](https://developer.work.weixin.qq.com/document/path/93742) · [观看 V2 95793](https://developer.work.weixin.qq.com/document/path/95793) · [未观看 V2 95795](https://developer.work.weixin.qq.com/document/path/95795)
