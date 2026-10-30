# 微信客服 · 统计管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 统计管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfStatisticsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfStatisticsService.cs)（父，承载全部 2 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfStatisticsService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfStatisticsService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfStatisticsService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（`GetKfCorpStatisticResponse` + `KfCorpStatisticItem` / `KfCorpStatisticData`；`GetKfServicerStatisticResponse` + `KfServicerStatisticItem` / `KfServicerStatisticData`；请求在 `RequestModel/`，命名空间恒为 `Mud.Wechat.Work.DataModels.Kf`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1e** 两支路由 / KF2 层级 / KF3 令牌绑定 / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [95489](https://developer.work.weixin.qq.com/document/path/95489)（企业）/ [95490](https://developer.work.weixin.qq.com/document/path/95490)（接待人员）· 第三方 95492 / 95493 · 代开发 96432 / 96433 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1e / KF2 / KF3 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§11 闭区间跨度 / 取整 / 次日可见 / 空值容错）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

微信客服的**逐日统计**，2 支端点、两个聚合维度：

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/kf/get_corp_statistic` | **企业/客服账号**维度的逐日统计（会话数、客户数、消息数、智能助手与会话质量指标） |
| E2 `POST /cgi-bin/kf/get_servicer_statistic` | **接待人员**维度的逐日统计（人工会话、回复率、首次响应时长、满意度、升级服务漏斗） |

形态：官方对**三类应用开放完全一致的 2 支** ⇒ 收敛父接口 + 三个零差异空标记子接口（KF2）。两支**读取语义但官方即 POST**（域 README §14 点名）。

**组件族另有一支同名不同面的统计**：`/cgi-bin/kf/get_statistic`（无 `open_kfid` 入参、企业级口径）属 [KfComponent.md](./KfComponent.md)，不在本族。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（KF3） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

第三方 / 代开发须先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**微信客服-可调用接口的应用**」中；第三方 / 代开发须具有「**微信客服 → 服务工具 → 获取客服数据统计**」权限。
- **时间口径四件套**（两支同文）：区间为**闭区间**、**最大跨度 31 天**、**最多获取最近 180 天**；`start_time` / `end_time` **须填当天 0 时 0 分 0 秒**，**非 0 点时间戳会被官方向下取整到当天 0 点**。
- **当天数据次日才可获取**（官方建议**次日 6 点后**调用）⇒ 拉当天恒缺不是 bug。
- **开启 API 或授权第三方应用管理会话的企业，没有 2022 年 3 月 11 日以前的统计数据**。
- **无数据的指标官方直接缺省不返回** ⇒ 每个字段都必须按可空解析，不能「缺字段 = 0」。
- `open_kfid` 两支均**官方必填**（按账号取统计）；E2 的 `servicer_userid` 不指定时返回的是**客服账号维度汇总**，**不是全体接待人员的明细**。
- 统计数字本身不含个人数据，但 `servicer_userid` 是员工标识：示例用假值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1/E2 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfStatisticsService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **2** | 否 |
| `IWechatWorkInternalKfStatisticsService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyKfStatisticsService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderKfStatisticsService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfStatisticsService)`。

## 4. 端点

### E1. `GetCorpStatisticAsync` — 获取客服数据统计（企业维度）

```csharp
[Post("/cgi-bin/kf/get_corp_statistic")]
Task<GetKfCorpStatisticResponse> GetCorpStatisticAsync(
    [Body] GetKfCorpStatisticRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetKfCorpStatisticRequest`，三者均官方必填）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string?` | 客服账号 ID |
| `start_time` | `StartTime` | `long?` | 起始日期时间戳，**须当天 0 点**；取值范围「昨天至前 180 天」 |
| `end_time` | `EndTime` | `long?` | 结束日期时间戳，同上；**与起始的闭区间跨度最大 31 天** |

**响应参数**（`statistic_list[]` → `KfCorpStatisticItem`，每项一天）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `stat_time` | `StatTime` | `long?` | 统计日期（**当日 0 点时间戳**） |
| `statistic` | `Statistic` | `KfCorpStatisticData?` | 当日数据；**当天无数据或尚未计算完成时不返回** |

`KfCorpStatisticData`（全部可空，**缺省 ≠ 0**）：

| 字段 | 属性 | 类型 | 口径 |
|---|---|---|---|
| `session_cnt` | `SessionCount` | `long?` | **咨询会话数**（客户发过消息并分配给接待人员或智能助手的会话；**转接不产生新会话**） |
| `customer_cnt` | `CustomerCount` | `long?` | 咨询客户数（**同一客户多次咨询只计一次**） |
| `customer_msg_cnt` | `CustomerMsgCount` | `long?` | **客户发送的消息总数** |
| `ai_session_reply_cnt` | `AiSessionReplyCount` | `long?` | 分配给智能助手的会话数 |
| `ai_transfer_rate` | `AiTransferRate` | `double?` | 转人工率 |
| `ai_knowledge_hit_rate` | `AiKnowledgeHitRate` | `double?` | 知识命中率：**须开启智能回复原生功能并配置知识库才产生**；不返回表示无法计算；**API 托管会话分配时智能回复原生功能失效** |
| `upgrade_service_customer_cnt` | `UpgradeServiceCustomerCount` | `long?` | 升级服务客户数（同一客户加多个专员/群**只计一次**；**2022-03-10 之后才有数据**） |
| `msg_rejected_customer_cnt` | `MsgRejectedCustomerCount` | `long?` | 被拒收消息的客户数（点了「不再接收消息」） |

### E2. `GetServicerStatisticAsync` — 获取接待人员数据统计

```csharp
[Post("/cgi-bin/kf/get_servicer_statistic")]
Task<GetKfServicerStatisticResponse> GetServicerStatisticAsync(
    [Body] GetKfServicerStatisticRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string?` | **官方必填** |
| `servicer_userid` | `ServicerUserId` | `string?` | **选填**：不指定则返回**客服账号维度汇总**（并非全体接待人员明细）；第三方 / 代开发填密文 `open_userid` |
| `start_time` / `end_time` | 同名 | `long?` | 同 E1 的四件套口径 |

`KfServicerStatisticData`（`statistic_list[]` 同样是「一天一项」）：

| 字段 | 属性 | 类型 | 口径 |
|---|---|---|---|
| `session_cnt` | `SessionCount` | `long?` | **接入人工会话数**（与 E1 同名字段语义不同，见 §5） |
| `customer_cnt` / `customer_msg_cnt` | 同名 | `long?` | 咨询客户数 / 客户咨询的消息总数 |
| `reply_rate` | `ReplyRate` | `double?` | 人工回复率（**无客户发消息给接待人员则不返回**） |
| `first_reply_average_sec` | `FirstReplyAverageSec` | `double?` | 平均首次响应时长（**秒**） |
| `satisfaction_investgate_cnt` | `SatisfactionInvestgateCount` | `long?` | 满意度评价发送数——**官方字段名即为 `investgate`**（拼写照抄）；**API 托管会话分配时满意度原生功能失效，此数为 0** |
| `satisfaction_participation_rate` | `SatisfactionParticipationRate` | `double?` | 满意度参评率 |
| `satisfied_rate` / `middling_rate` / `dissatisfied_rate` | 同名 | `double?` | 「满意 / 一般 / 不满意」占比（**无人参评则不返回**） |
| `upgrade_service_customer_cnt` | `UpgradeServiceCustomerCount` | `long?` | 升级服务客户数（2022-03-10 之后才有数据） |
| `upgrade_service_member_invite_cnt` | `UpgradeServiceMemberInviteCount` | `long?` | 专员服务邀请数 |
| `upgrade_service_member_customer_cnt` | `UpgradeServiceMemberCustomerCount` | `long?` | 添加专员的客户数（**添加多个专员计多次**） |
| `upgrade_service_groupchat_invite_cnt` | `UpgradeServiceGroupchatInviteCount` | `long?` | 客户群服务邀请数 |
| `upgrade_service_groupchat_customer_cnt` | `UpgradeServiceGroupchatCustomerCount` | `long?` | 加入客户群的客户数（**加入多个群计多次**） |
| `msg_rejected_customer_cnt` | `MsgRejectedCustomerCount` | `long?` | 被拒收消息的客户数 |

**调用示例**（企业维度取近 30 天 → 人员维度取昨日并做缺省容错）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Kf;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "kf-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddKfApi());

var stats = provider.GetRequiredService<IWechatWorkInternalKfStatisticsService>();
string openKfId = "wkOpenKfId假值";

// 官方口径：闭区间、跨度 ≤31 天、只到「昨天」（当天数据次日可见，建议次日 6 点后取）
long todayMidnight = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();   // 0 点整
long start = todayMidnight - 30 * 86400L;
long end = todayMidnight - 86400L;

// ① 企业/账号维度
var corp = await stats.GetCorpStatisticAsync(
    new GetKfCorpStatisticRequest { OpenKfId = openKfId, StartTime = start, EndTime = end }, cancellationToken);
WechatWorkException.ThrowIfFailed(corp, "/cgi-bin/kf/get_corp_statistic");

long totalSessions = 0;
foreach (KfCorpStatisticItem day in corp.StatisticList ?? new List<KfCorpStatisticItem>())
{
    // statistic 在「当天无数据或尚未计算完成」时整体不返回 —— 缺省不等于 0
    if (day.Statistic is null) continue;
    totalSessions += day.Statistic.SessionCount ?? 0;
    // ai_knowledge_hit_rate 不返回表示无法计算（未开智能回复原生功能 / API 托管会话使其失效）
    logger.LogInformation("{StatTime} 会话 {Session}、客户 {Customer}、转人工率 {Transfer:0.##}",
        day.StatTime, day.Statistic.SessionCount, day.Statistic.CustomerCount, day.Statistic.AiTransferRate);
}

// ② 接待人员维度：servicer_userid 填了才是「这个人」的明细；不填是该客服账号的汇总（不是全员明细）
var perServicer = await stats.GetServicerStatisticAsync(
    new GetKfServicerStatisticRequest
    {
        OpenKfId = openKfId,
        ServicerUserId = "zhangsan",        // 假值；第三方/代开发填密文 open_userid
        StartTime = end,
        EndTime = end,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(perServicer, "/cgi-bin/kf/get_servicer_statistic");

foreach (KfServicerStatisticItem day in perServicer.StatisticList ?? new List<KfServicerStatisticItem>())
{
    KfServicerStatisticData? d = day.Statistic;
    if (d is null) continue;
    // 这里的 session_cnt 是「接入人工会话数」，与 E1 的「咨询会话数」同名不同义
    // reply_rate / 三个满意度占比在无客户发消息、无人参评时不返回
    logger.LogInformation("接待统计：人工会话 {Session}，回复率 {Reply:0.##}，首次响应 {First} 秒，发送满意度 {Send}",
        d.SessionCount, d.ReplyRate, d.FirstReplyAverageSec, d.SatisfactionInvestgateCount);
}
logger.LogInformation("近 30 天账号累计咨询会话 {Total} 次", totalSessions);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 两支的 `session_cnt` | E1 是**咨询会话数**（客户发过消息并分配给人工**或智能助手**，转接不产生新会话）；E2 是**接入人工会话数** | 把两者相加或当作同一指标 |
| 两支的 `customer_msg_cnt` | E1 注释「客户发送的消息总数」、E2「客户咨询的消息总数」，聚合维度不同 | 跨维度对齐数字 |
| 「去重」与「计多次」 | `upgrade_service_customer_cnt` **同一客户加多个专员/群只计一次**；`upgrade_service_member_customer_cnt` / `..._groupchat_customer_cnt` **计多次** | 用后者当独立客户数 |
| `satisfaction_investgate_cnt` 拼写 | 官方字段名就是 **`investgate`**（非 `investigate`），本仓照抄 | 手改成「正确英文」→ 反序列化取不到 |
| 缺省 ≠ 0 | 注释明写「无数据的指标官方直接缺省不返回」，且 `statistic` 整体可能不返回 | `?? 0` 之后当真实零值做同比 |
| 当天数据 | **次日才可获取**（建议次日 6 点后），区间右端只到昨天 | 拉当天以为接口坏 |
| 非 0 点入参 | 官方**向下取整到当天 0 点** ⇒ 传任意时刻都等价于当天 0 点 | 按小时切片查询 |
| 31 天 / 180 天 | 闭区间最大跨度 **31 天**、最多查最近 **180 天**；另「开启 API / 授权第三方管理会话」的企业**无 2022-03-11 前数据** | 拉半年一次请求 → 被拒或空 |
| E2 不填 `servicer_userid` | 返回的是**客服账号维度汇总**，**不是全体接待人员明细** | 当成「全员合计再拆分」的明细源 |
| 本族 vs 组件族统计 | 组件族 `/cgi-bin/kf/get_statistic`（**无 `open_kfid`**、企业级口径、属 [KfComponent.md](./KfComponent.md)）与本族两支路由与 DTO 均不同 | 注错接口或复用 DTO |
| 本族 vs 客户联系统计 | [Statistics.md](../ExternalContact/Statistics.md)（`get_user_behavior_data` / `groupchat/statistic`）是**客户联系**维度的统计，与本域口径不可互相换算 | 混用两套指标 |
| 比率字段类型 | `*_rate` 是 `double?`（官方比例），计数是 `long?` | 当百分比整数（0~100 vs 0~1）处理 |
| 动词 | 两支读取全 **POST** | 改 GET → 打坏 KF1e 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验跨度/保留期**、**不代填缺省值**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仍可能只返回部分天、部分指标（缺省即无数据） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`open_kfid` 非法、无「获取客服数据统计」权限、区间超 31 天 / 超 180 天、`servicer_userid` 不存在等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 各比率字段的量纲 | 注释只说「转人工率 / 参评率 / 占比」，**未给是 0~1 还是 0~100** ⇒ **未逐项核验**（示例按原值输出，不做换算） | 官方 95489 / 95490 响应参数表 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方各页 + 全局错误码页 |
| 端点频率限制 | 注释未给数值 ⇒ 未逐项核验，勿从其它域借数 | 官方访问频率附录 90312 |
| `statistic` 缺省的完整条件 | 注释写「当天无数据或尚未计算完成时不返回」，未给「计算完成」的时间保证 ⇒ 未核验（仅「建议次日 6 点后」） | 官方 95489 |
| 指标的历史起点差异 | 仅 `upgrade_service_customer_cnt` 标注「2022-03-10 之后才有数据」，其它指标是否同起点未说明 ⇒ 未核验 | 官方 95489 / 95490 |
| 分页 | 两支均无分页参数（一次返回区间内全部天），未说明 31 天上限之外的截断行为 ⇒ 未核验 | 官方 95490 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§11 统计族口径、§14 「查询类官方即 POST」汇总）
- 同域：[KfAccount.md](./KfAccount.md)（`open_kfid` 来源）· [KfServicer.md](./KfServicer.md)（`servicer_userid` 来源）· [KfSession.md](./KfSession.md)（指标背后的会话与消息机制）· [KfUpgrade.md](./KfUpgrade.md)（升级服务漏斗的配置面）· [KfComponent.md](./KfComponent.md)（组件版 `get_statistic`）
- 同线跨域：[Statistics.md](../ExternalContact/Statistics.md)（客户联系统计，另一套口径）
- 官方页：[获取客服数据统计（95489）](https://developer.work.weixin.qq.com/document/path/95489) · [获取接待人员数据统计（95490）](https://developer.work.weixin.qq.com/document/path/95490) · [第三方 95492](https://developer.work.weixin.qq.com/document/path/95492) / [95493](https://developer.work.weixin.qq.com/document/path/95493)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
