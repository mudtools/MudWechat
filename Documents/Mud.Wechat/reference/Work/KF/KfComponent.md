# 微信客服 · 客服组件（第三方场景）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 微信客服组件族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfComponentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfComponentService.cs)（父，**零端点**）· [IWechatWorkThirdPartyKfComponentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfComponentService_ThirdParty.cs)（承载唯一 1 支） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（`GetKfComponentStatisticResponse` + `KfComponentStatisticItem` / `KfComponentStatisticData`；请求在 `RequestModel/GetKfComponentStatisticRequest.cs`；命名空间 `Mud.Wechat.Work.DataModels.Kf`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1g** 本族独有端点路由 / **KF1g2** 组件子接口**不得**重复声明 `account/list` 与 `add_contact_way` / **KF2b** 零端点父接口 + 继承链恰 1 个第三方子接口 / KF3 令牌绑定 / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 组件数据统计 [99367](https://developer.work.weixin.qq.com/document/path/99367)；组件版「获取客服账号列表 / 链接」另页 99368 / 99400（**同路由，归客服账号域**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1g / KF1g2 / KF2b / KF3 / KF4 + 接口与 DTO XML 注释（含「单一所有者」架构裁决原文）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§2、§4、§14）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

**微信客服组件**（第三方把客服能力接入自己的场景）在本仓只有 **1 支独有端点**：

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/kf/get_statistic` | 取**组件场景下**的客服数据统计（逐日、企业级口径，**无 `open_kfid` 入参**） |

形态：**零端点父接口 + 唯一第三方子接口承载**（KF2b，落位对齐获客助手组件域等「官方只开放一类应用」的族形态）。第三方 / 组件应用之外**没有对应契约面**——自建与代开发**不设子接口**，这不是漏挂而是官方无对应页面。

**本族不含账号列表与客服链接**：官方另以 99368 / 99400 两页承载「获取客服账号列表」「获取客服账号链接」，但这两支与客服账号管理域**同路由、同 DTO** ⇒ 本仓裁决为**单一所有者**（客服账号域声明一次），**KF1g2 反向锁定组件子接口不得重复声明**。组件应用请直接注入 [KfAccount.md](./KfAccount.md) 的接口。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.ThirdParty`（组件套件；无自建 / 代开发子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（KF3 断言；**不是** `suite_access_token`） | — |
| 令牌归属键 | `Wechat.AccessToken@Corp`（**授权企业级令牌，一企一份**） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

> **令牌口径是本族最容易被写错的地方**：组件域端点走的是**授权企业级 `access_token`**（scope = `authCorpId`），须先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）建立「应用 + 企业」作用域再调用；它与获客助手组件域里那支用 `suite_access_token` 的代支付流水端点**不是同一口径**。

### 2.2 权限与业务前置

- 官方权限口径（子接口注释原文）：需「**管理接入的微信客服**」相应权限——数据统计须具有「**管理来自该场景的客服咨询 → 获取客服数据统计**」权限，**并需在场景中配置获取到的带参客服账号链接**（即先用客服账号域的链接端点生成带 `scene` 的链接、配置进组件场景，数据才会归属到该场景）。
- 时间口径与统计族一致：`start_time` / `end_time` **均官方必填**、**须填当天 0 时 0 分 0 秒**、取值范围「**昨天至前 180 天**」；闭区间、非 0 点向下取整、当天数据次日可见。
- **无数据的指标官方直接缺省不返回** ⇒ 解析须空值容错。
- 统计数字不含个人数据，但归属到「场景」维度：与账号维度（`open_kfid`）不可直接互相核对。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方（组件） | 代开发 |
|---|---|---|---|
| E1 | ❌ | ✅ | ❌ |

⇒ **不要**为自建 / 代开发补子接口：KF2b 断言继承链上只有第三方子接口。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfComponentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyKfComponentService` | `Mud.Wechat.Work` | 第三方子接口（**承载唯一 1 支**） | **1** | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfComponentService)`。

## 4. 端点

### E1. `GetStatisticAsync` — 获取组件场景的客服数据统计

```csharp
[Post("/cgi-bin/kf/get_statistic")]
Task<GetKfComponentStatisticResponse> GetStatisticAsync(
    [Body] GetKfComponentStatisticRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetKfComponentStatisticRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `start_time` | `StartTime` | `long?` | **是** | 须填当天 0 时 0 分 0 秒；取值范围「昨天至前 180 天」 |
| `end_time` | `EndTime` | `long?` | **是** | 同上；**本端点没有 `open_kfid` 入参**（企业级 / 场景级口径） |

**响应参数**（`statistic_list[]` → `KfComponentStatisticItem`，一天一项）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `stat_time` | `StatTime` | `long?` | 统计日期（当日 0 点时间戳） |
| `statistic` | `Statistic` | `KfComponentStatisticData?` | 当日数据；**当天无数据或尚未计算完成时不返回** |

`KfComponentStatisticData`（全部可空，**缺省 ≠ 0**）：

| 字段 | 属性 | 类型 | 口径 |
|---|---|---|---|
| `session_cnt` | `SessionCount` | `long?` | 咨询会话数（客户发过消息并分配给接待人员或智能助手；转接不产生新会话） |
| `customer_cnt` | `CustomerCount` | `long?` | 咨询客户数（同一客户多次咨询只计一次） |
| `customer_msg_cnt` | `CustomerMsgCount` | `long?` | 咨询消息总数 |
| `ai_session_reply_cnt` | `AiSessionReplyCount` | `long?` | **机器人会话数** |
| `servicer_session_cnt` | `ServicerSessionCount` | `long?` | **接入人工会话数**（组件侧同时给出机器人/人工两格，命名与统计族不同，见 §5） |
| `reply_rate` | `ReplyRate` | `double?` | 人工回复率（无客户发消息给接待人员则不返回） |
| `first_reply_average_sec` | `FirstReplyAverageSec` | `double?` | 平均首次响应时长（秒） |
| `satisfaction_investgate_cnt` | `SatisfactionInvestgateCount` | `long?` | 满意度评价发送数（**官方字段名即 `investgate`**；API 托管会话分配时满意度原生功能失效，此数为 0） |
| `satisfaction_participation_rate` | `SatisfactionParticipationRate` | `double?` | 满意度参评率 |
| `satisfied_rate` / `middling_rate` / `dissatisfied_rate` | 同名 | `double?` | 「满意 / 一般 / 不满意」占比（无人参评则不返回） |

**调用示例**（切到授权企业作用域 → 取组件场景近 14 天统计并做缺省容错）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Kf;

var component = provider.GetRequiredService<IWechatWorkThirdPartyKfComponentService>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();

long todayMidnight = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();   // 0 点整
long start = todayMidnight - 14 * 86400L;
long end = todayMidnight - 86400L;                                             // 右端只到「昨天」

// 组件域消费「授权企业级」access_token（scope = authCorpId），必须先进入 应用 + 企业 作用域
using (switcher.UseCorpScope(appKey: "suite-a", authCorpId: "wpAuthCorp假值"))
{
    var stat = await component.GetStatisticAsync(
        new GetKfComponentStatisticRequest { StartTime = start, EndTime = end }, cancellationToken);
    WechatWorkException.ThrowIfFailed(stat, "/cgi-bin/kf/get_statistic");

    long robotSessions = 0, humanSessions = 0;
    foreach (KfComponentStatisticItem day in stat.StatisticList ?? new List<KfComponentStatisticItem>())
    {
        if (day.Statistic is null) continue;         // 无数据或未计算完成时整体不返回：缺省不等于 0
        robotSessions += day.Statistic.AiSessionReplyCount ?? 0;
        humanSessions += day.Statistic.ServicerSessionCount ?? 0;
        logger.LogInformation("{StatTime} 机器人会话 {Robot}、人工会话 {Human}、回复率 {Reply:0.##}",
            day.StatTime, day.Statistic.AiSessionReplyCount, day.Statistic.ServicerSessionCount, day.Statistic.ReplyRate);
    }
    logger.LogInformation("组件场景近 14 天：机器人 {Robot} / 人工 {Human}", robotSessions, humanSessions);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族 E1 vs 统计族 | `get_statistic`（组件、**无 `open_kfid`**）vs `get_corp_statistic` / `get_servicer_statistic`（账号 / 人员维度、`open_kfid` 必填）——**三条路由、三套 DTO** | 用组件接口去核对账号维度数字 |
| `session_cnt` 的第三种含义 | 组件侧 `session_cnt` = **咨询会话数**，且另有 `ai_session_reply_cnt`（机器人会话数）与 **`servicer_session_cnt`（接入人工会话数）**；而统计族的「人工会话数」在 E2 里叫 `session_cnt` | 跨接口按字段名对齐指标 |
| 令牌类型 | 本族走**企业级 `access_token`**（`Wechat.AccessToken@Corp`）；**不是** `suite_access_token`。获客助手组件域的账单端点才是 `suite_access_token`（[AcquisitionComponent.md](../ExternalContact/AcquisitionComponent.md)） | 按「组件 = 套件令牌」直觉切错令牌链 |
| 账号列表 / 客服链接的所有权 | 官方有组件版文档页 99368 / 99400，但**与本域同路由同 DTO** ⇒ 只在客服账号域声明一次，**KF1g2 禁止组件子接口重复声明**；组件应用调 `account/list` 时响应**不返回 `manage_privilege`** | 在组件族再造两支端点 |
| 前置的「带参链接」 | 数据统计要求**先在场景里配置带参客服账号链接**（`scene` 来自客服账号域 E5）；未配置则场景维度无归属数据 | 以为统计接口坏掉 |
| 应用类型 | 仅第三方（组件套件）有子接口；自建 / 代开发**没有** | 注入 `IWechatWorkInternalKfComponentService`（不存在） |
| `satisfaction_investgate_cnt` | 官方拼写照抄（非 `investigate`） | 改拼写 → 取不到字段 |
| 缺省与 0 | 「无数据的指标官方直接缺省不返回」，且 `statistic` 整体可能缺失 | 把缺省当 0 做同比 |
| 时间口径 | 0 点整、昨天至前 180 天、闭区间、次日可见、非 0 点向下取整 | 拉当天、按小时切片 |
| 动词 | 读取语义却 **POST**（域 README §14 点名） | 改 GET → 打坏 KF1g 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验作用域是否已切换**（归属键错配由基座 fail-fast）、**不代填缺省值**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仍可能返回空列表或部分指标缺省 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（企业级令牌） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

未切企业作用域时归属键错配抛 **`WechatTokenOwnerMismatchException`**（fail-fast，不是 `errcode`）；无「获取客服数据统计」权限、区间越界等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 比率字段的量纲 | 注释未给 0~1 还是 0~100 ⇒ **未逐项核验** | 官方 99367 响应参数表 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方 99367 + 全局错误码页 |
| 端点频率限制 | 注释未给数值 ⇒ 未逐项核验，勿从统计族借 | 官方访问频率附录 90312 |
| 「场景」维度的定义与归属 | 注释说需「在场景中配置获取到的带参客服账号链接」，但**场景 id 不出现在请求或响应里**，数据如何按场景切分未说明 ⇒ 未核验 | 官方组件接入文档（99351 类） |
| 与客服账号域两支共用路由的行为差异 | 除「不返回 `manage_privilege`」外，是否还有其它字段差异 ⇒ **未逐项核验** | 官方 99368 / 99400 与 94661 / 94665 对照 |
| 自建 / 代开发是否可取同类统计 | 官方无对应页面 ⇒ 本域**零声明**，若官方开放需同批改形态与守卫 | 官方组件文档树 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§2 八族开放面不等宽、§4 组件版共用路由的单一所有者裁决、§14 POST 汇总）
- 同域：[KfAccount.md](./KfAccount.md)（组件应用实际使用的两支共用端点 + `scene` 来源）· [KfStatistics.md](./KfStatistics.md)（账号 / 人员维度统计，与本族指标命名不同）· [KfSession.md](./KfSession.md)（会话与消息机制）
- 同形态参照：[AcquisitionComponent.md](../ExternalContact/AcquisitionComponent.md)（获客助手组件域，**令牌口径不同**：其账单端点用 `suite_access_token`）
- 官方页：[获取客服数据统计（组件，99367）](https://developer.work.weixin.qq.com/document/path/99367) · [获取客服账号列表（组件版 99368）](https://developer.work.weixin.qq.com/document/path/99368) · [获取客服账号链接（组件版 99400）](https://developer.work.weixin.qq.com/document/path/99400)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
