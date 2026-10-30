# 客户联系 · 数据统计

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 数据统计族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactStatisticsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Statistics/IWechatWorkExternalContactStatisticsService.cs)（父，承载全部 3 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Statistics/IWechatWorkExternalContactStatisticsService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Statistics/IWechatWorkExternalContactStatisticsService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Statistics/IWechatWorkExternalContactStatisticsService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/Statistics/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/Statistics/)（11 型，`SerializerClassName = Statistics`，登记断言见 ST4 → `StatisticsJsonContext`） |
| 契约守卫 | [WechatExternalContactStatisticsContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactStatisticsContractGuards.cs)（ST1 路由表 3 支且互不重复 / ST2 层级与三空标记 / ST3 令牌绑定 / ST4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 联系客户统计 自建 [92132](https://developer.work.weixin.qq.com/document/path/92132) / 第三方 [92275](https://developer.work.weixin.qq.com/document/path/92275) / 代开发 [96359](https://developer.work.weixin.qq.com/document/path/96359) · 群聊统计（两支共用一组文档页）92133 / 93476 / 96358 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ST1~ST4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.12、§4 陷阱 3/6）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

客户联系的**只读统计面**，3 支端点、两类维度：

| 端点 | 用途 |
|---|---|
| E1 `get_user_behavior_data` | 「联系客户统计」：按天返回成员侧的发起申请数 / 新增客户数 / 聊天数 / 发送消息数 / 已回复占比 / 平均首次回复时长 / 删除拉黑数 |
| E2 `groupchat/statistic` | 「群聊数据统计」**按群主聚合**，带排序与 `offset` + `limit` 分页 |
| E3 `groupchat/statistic_group_by_day` | 「群聊数据统计」**按自然日聚合**，字段与 E2 完全相同、无排序分页参数 |

形态：官方对三类应用开放完全一致的 3 支端点 ⇒ 全收敛父接口，三支子接口空标记（ST2）。**3 支全部 POST**，查询参数走 body。

本族是 `ExternalContact` 域里**唯一使用 `offset` + `limit` 分页**的地方——本域列表类端点用的是 `cursor` + `next_cursor`，两套不可混用（域 README §4 陷阱 3）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（ST3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- **权限分两套**：E1 要求第三方 / 代开发具有「**获取成员联系客户的数据统计**」权限（自建须用配置到「可调用应用」列表中的 secret 取的令牌）；E2 / E3 要求「企业客户权限 → **客户群** → 获取客户群的数据统计」权限。
- 传入的**成员 / 部门 / 群主须在应用可见范围内**。
- **时间口径三件套**（三支一致）：区间为**闭区间**、**最大跨度 30 天**、**数据最多保留 180 天**；**非 0 点时间戳自动向下取整到当日 0 点**。
- E2 / E3 的 `day_begin_time` **取值范围是「昨天」至前 180 天**——**当天的数据当天取不到**。
- E2 / E3 不指定群主过滤时取应用可见范围内**全部群主**，**可见范围人数超过 1000 人会报错 81017** ⇒ 大范围必须显式给 `owner_filter`。
- E1 的 `userid` 与 `partyid` **不可同时为空**，各**最多 100 个**；**传入多个 userid 时返回的是总体数据**（不按成员拆分）。
- E1 的 `reply_percentage`（已回复聊天占比）与 `avg_reply_time`（平均首次回复时长，分钟）**仅在确有聊天时返回**，且**不含群聊**。
- E2 的 `limit` **默认 500、取值 1~1000**，`offset` 默认 0；`order_by` 1 新增群数量（默认）/ 2 群总数 / 3 新增群人数 / 4 群总人数；`order_asc` 0 降序（默认）/ 1 升序。
- 统计口径下的成员 userid、群数与消息数属经营数据；`migrate_trainee_chat_cnt`（迁移群数）**仅教培行业返回**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ✅（须对应统计权限） | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactStatisticsService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalExternalContactStatisticsService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactStatisticsService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactStatisticsService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactStatisticsService)`。

## 4. 端点

签名与字段名以 [接口源文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Statistics/IWechatWorkExternalContactStatisticsService.cs) 与 [DTO 目录](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/Statistics/) 为准。「必填」列是**官方参数表口径**，SDK 不做本地必填校验。

### E1. `GetUserBehaviorDataAsync` — 获取「联系客户统计」数据

```csharp
[Post("/cgi-bin/externalcontact/get_user_behavior_data")]
Task<GetUserBehaviorDataResponse> GetUserBehaviorDataAsync(
    [Body] GetUserBehaviorDataRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetUserBehaviorDataRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `List<string>?` | 条件 | ≤100 个；**与 `partyid` 不可同时为空**；传多个时返回**总体数据** |
| `partyid` | `Partyid` | `List<long>?` | 条件 | ≤100 个部门 id；同上 |
| `start_time` | `StartTime` | `long?` | **是** | Unix 秒；非零点自动取整到当日 0 点 |
| `end_time` | `EndTime` | `long?` | **是** | 与起始时间**跨度最大 30 天**，最多查最近 180 天 |

**响应参数**（`behavior_data[]` → `UserBehaviorData`，**每项为一天的数据**）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `stat_time` | `StatTime` | `long?` | 数据日期（**当日 0 点**时间戳） |
| `new_apply_cnt` | `NewApplyCnt` | `long?` | 发起申请数（搜索手机号、扫一扫、微信好友/群聊添加、共享分配客户等渠道） |
| `new_contact_cnt` | `NewContactCnt` | `long?` | 新增客户数 |
| `chat_cnt` | `ChatCnt` | `long?` | 聊天总数（**有主动发过消息的单聊**总数） |
| `message_cnt` | `MessageCnt` | `long?` | 单聊中发送的消息总数 |
| `reply_percentage` | `ReplyPercentage` | `double?` | 已回复聊天占比（客户主动发起聊天后**当日**有回复的占比，**不含群聊**，仅在确有聊天时返回） |
| `avg_reply_time` | `AvgReplyTime` | `double?` | 平均**首次**回复时长（**分钟**；同上限制） |
| `negative_feedback_cnt` | `NegativeFeedbackCnt` | `long?` | 删除 / 拉黑成员的客户数 |

### E2. `GetGroupChatStatisticAsync` — 群聊数据统计（按群主聚合）

```csharp
[Post("/cgi-bin/externalcontact/groupchat/statistic")]
Task<GetGroupChatStatisticResponse> GetGroupChatStatisticAsync(
    [Body] GetGroupChatStatisticRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetGroupChatStatisticRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `day_begin_time` | `DayBeginTime` | `long?` | **是** | 当天 0 时 0 分 0 秒；**取值范围为昨天至前 180 天** |
| `day_end_time` | `DayEndTime` | `long?` | 否 | 不填默认同起始日期；规则同上 |
| `owner_filter.userid_list` | `OwnerFilter.UseridList` | `List<string>?` | — | ≤100 个群主 userid；**注释既标「官方必填」又说明「不填 = 全部可见群主」**（见 §5 / §7） |
| `order_by` | `OrderBy` | `int?` | 否 | 1 新增群数量（默认）/ 2 群总数 / 3 新增群人数 / 4 群总人数 |
| `order_asc` | `OrderAsc` | `int?` | 否 | 0 降序（默认）/ 1 升序 |
| `offset` | `Offset` | `int?` | 否 | 默认 0 |
| `limit` | `Limit` | `int?` | 否 | **默认 500，取值 1~1000** |

**响应参数**（`GetGroupChatStatisticResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `total` | `Total` | `long?` | 命中过滤条件的记录总数 |
| `next_offset` | `NextOffset` | `long?` | 下一页 offset，**与 `total` 相等即已取完** |
| `items[]` | `Items` | `List<GroupChatStatisticItem>?` | 按群主的记录 |
| `items[].owner` | `Owner` | `string?` | 群主 userid |
| `items[].data` | `Data` | `GroupChatStatisticData?` | 指标明细（见下表） |

`GroupChatStatisticData`（E2 / E3 共用）：`new_chat_cnt` 新增客户群数、`chat_total` 截至当天客户群总数、`chat_has_msg` 有发过消息的群数、`new_member_cnt` 新增群人数、`member_total` 群总人数、`member_has_msg` 有发过消息的群成员数、`msg_total` 群消息总数、**`migrate_trainee_chat_cnt` 新增迁移群数（仅教培行业返回）**。

### E3. `GetGroupChatStatisticGroupByDayAsync` — 群聊数据统计（按自然日聚合）

```csharp
[Post("/cgi-bin/externalcontact/groupchat/statistic_group_by_day")]
Task<GetGroupChatStatisticGroupByDayResponse> GetGroupChatStatisticGroupByDayAsync(
    [Body] GetGroupChatStatisticGroupByDayRequest request, CancellationToken cancellationToken = default);
```

- 请求：`day_begin_time`（必填）、`day_end_time`、`owner_filter`——**没有 `order_by` / `order_asc` / `offset` / `limit`**。
- 响应：`items[]`（`GroupChatStatisticDayItem`：`stat_time` 当日 0 点时间戳 + `data` 同一套 `GroupChatStatisticData`）。
- 数据量由日期跨度决定（≤30 天）⇒ **一次取完，无分页信号**，这点与 E2 不同。

**调用示例**（E1 取近 30 天行为趋势 → E2 按群主聚合分页 → E3 按天聚合）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var stats = provider.GetRequiredService<IWechatWorkInternalExternalContactStatisticsService>();

// 统计区间：闭区间、跨度 ≤30 天、只能取到「昨天」；非 0 点会被向下取整到当日 0 点
long today = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
long beginTime = today - 29 * 86400L;   // 假值：近 30 天
long endTime = today - 86400L;          // 当天数据当天取不到

// ① 联系客户统计：userid 与 partyid 不可同时为空；传多个 userid 返回的是总体数据
var behavior = await stats.GetUserBehaviorDataAsync(
    new GetUserBehaviorDataRequest
    {
        Userid = new List<string> { "zhangsan" },
        StartTime = beginTime,
        EndTime = endTime,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(behavior, "/cgi-bin/externalcontact/get_user_behavior_data");

foreach (UserBehaviorData day in behavior.BehaviorData ?? new List<UserBehaviorData>())
{
    // reply_percentage / avg_reply_time 仅在确有聊天时返回，且不含群聊
    logger.LogInformation("{StatTime} 新增客户 {New} 人，聊天 {Chat} 次，首次回复均值 {Reply} 分钟",
        day.StatTime, day.NewContactCnt, day.ChatCnt, day.AvgReplyTime);
}

// ② 群聊统计（按群主聚合）：本域唯一 offset + limit 分页；next_offset == total 即取完
int offset = 0;
long total = 0;
do
{
    var page = await stats.GetGroupChatStatisticAsync(
        new GetGroupChatStatisticRequest
        {
            DayBeginTime = beginTime,
            DayEndTime = endTime,
            OwnerFilter = new GroupChatStatisticOwnerFilter
            {
                UseridList = new List<string> { "zhangsan", "lisi" },   // 假值；可见范围 >1000 人时不填会 81017
            },
            OrderBy = 2,            // 按群总数排序
            OrderAsc = 0,           // 降序
            Offset = offset,
            Limit = 500,
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalcontact/groupchat/statistic");

    total = page.Total ?? 0;
    foreach (GroupChatStatisticItem item in page.Items ?? new List<GroupChatStatisticItem>())
    {
        logger.LogInformation("群主 {Owner}：群总数 {Total}，消息总数 {Msg}",
            item.Owner, item.Data?.ChatTotal, item.Data?.MsgTotal);
    }
    offset = (int)(page.NextOffset ?? 0);
    if (offset <= 0) break;         // 防御空返回导致的死循环
} while (offset < total);

// ③ 群聊统计（按自然日聚合）：同一套 data 字段，无排序与分页参数
var daily = await stats.GetGroupChatStatisticGroupByDayAsync(
    new GetGroupChatStatisticGroupByDayRequest
    {
        DayBeginTime = beginTime,
        DayEndTime = endTime,
        OwnerFilter = new GroupChatStatisticOwnerFilter { UseridList = new List<string> { "zhangsan" } },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(daily, "/cgi-bin/externalcontact/groupchat/statistic_group_by_day");

foreach (GroupChatStatisticDayItem day in daily.Items ?? new List<GroupChatStatisticDayItem>())
{
    logger.LogInformation("{StatTime} 新增群 {New}，新增群人数 {Member}",
        day.StatTime, day.Data?.NewChatCnt, day.Data?.NewMemberCnt);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E2 `offset` + `limit` vs 本域其它族 `cursor` + `next_cursor` | 本族是全域**唯一**用 offset 分页的统计端点；判完依据是 `next_offset == total` 而不是「游标为空」 | 按游标语义写循环 → 首页即退出或死循环 |
| E2 vs E3 | E2 按群主聚合、有排序分页；E3 按自然日聚合、**无** `order_by` / `offset` / `limit`；`data` 字段面**完全相同** | 给 E3 请求体塞排序字段 → 无效字段 |
| E2 `next_offset` 的相等对象 | 与 `total` 相等表示取完 | 与 `items.Count` 比较 → 提前结束 |
| `stat_time` / `day_begin_time` | 一律是**当日 0 点**时间戳；非 0 点入参被**向下取整** | 以为精确到秒而按小时切片 |
| 「昨天」上界 | E2/E3 的 `day_begin_time` 取值范围为昨天至前 180 天 ⇒ **当天数据当天取不到** | 拉当天却拿不到，误判接口失灵 |
| 30 天跨度 / 180 天保留 | 跨度超 30 天、区间超 180 天均不合法 | 直接查半年 → 报错或空数据 |
| E1 传多个 `userid` | 返回**总体数据**，不是逐成员明细 | 按成员聚合 → 数字成倍重复 |
| E1 `userid` vs `partyid` | **不可同时为空**，各 ≤100 | 两者都不传 → 参数错误 |
| `reply_percentage` / `avg_reply_time` | 仅「确有聊天」时返回，且**不含群聊**；单位是**分钟** | 当成百分比整数（0~100 vs 0~1）或当成秒 |
| `owner_filter` 的必填口径 | DTO 注释同时写「官方必填」与「不填 = 取全部可见群主」⇒ **两处并存、未逐项核验**（见 §7）；且**可见范围 >1000 人不填直接 81017** | 依赖「不填取全部」做大盘 → 大企业中直接报错 |
| `owner_filter` 字段面 | 本仓仅建模 `userid_list`（≤100），**无 `partyid_list`** | 需要按部门筛群主时本 SDK 走不通（见 §7） |
| `migrate_trainee_chat_cnt` | **仅教培行业**返回 | 按必有字段处理 → 恒为 null |
| 动词 | 3 支读取全 POST | 改 GET → 打坏 ST1 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验跨度/保留期/1000 人可见范围**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功仍须按 `next_offset` 与 `total` 的关系决定是否续拉 |
| 81017 | 官方业务码（接口注释留档） | 未指定群主过滤且应用可见范围人数超过 1000 | 必须给 `owner_filter` 分批拉取 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

跨度超 30 天、区间超出 180 天保留期、`userid` 与 `partyid` 同时为空等业务码**接口注释未逐条留档** ⇒ 见 §7。业务失败 HTTP 仍 200。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `owner_filter` 的必填性 | 同一句注释同时出现「官方必填」与「不填表示取应用可见范围内全部群主」⇒ **内部口径矛盾，未逐项核验** | 官方 92133 参数表；结论确定后同批修正接口与 DTO 注释 |
| `owner_filter.partyid_list` | 本仓 `GroupChatStatisticOwnerFilter` **只建模 `userid_list`**，未建模部门维度 ⇒ 是否官方提供**未核验** | 官方 92133 / 93476 参数表 |
| E1 的部门维度行为 | 注释说「传入多个 userid 返回总体数据」，未说明传 `partyid` 时的聚合口径 ⇒ 未核验 | 官方 92132 |
| 业务错误码全集 | 仅留档 81017 ⇒ 未逐项核验 | 官方各端点页 |
| 无数据日的返回形态 | 注释未说明「无数据的天是否出现在 `behavior_data` / `items` 中」⇒ 未核验 | 官方 92132 / 92133 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.12 端点表、§4 陷阱 3「游标 vs 偏移两套分页」、陷阱 6「统计粒度与跨度」）
- 同域：[GroupChat.md](./GroupChat.md)（群明细与 `chat_id`，本族只给聚合数字）· [Customer.md](./Customer.md)（客户列表/详情，本族数字的实体来源）· [GroupMsg.md](./GroupMsg.md)（群发侧的发送结果统计，另一套接口）
- 同线跨域：「应用「get_agent 统计数据」」类接口属 `Agent` 域（[Agent.md](../Agent/Agent.md)），与本族是客户联系维度，口径不可互相换算
- 官方页：[获取「联系客户统计」数据（92132）](https://developer.work.weixin.qq.com/document/path/92132) · [获取「群聊数据统计」（92133）](https://developer.work.weixin.qq.com/document/path/92133) · [代开发应用 96358](https://developer.work.weixin.qq.com/document/path/96358) / [96359](https://developer.work.weixin.qq.com/document/path/96359)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
