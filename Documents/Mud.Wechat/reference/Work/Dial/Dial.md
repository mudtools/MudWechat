# 公费电话（拨打记录）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Dial` 域 · 公费电话族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkDialService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Dial/IWechatWorkDialService.cs)（**零端点父接口**）· [IWechatWorkInternalDialService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Dial/IWechatWorkDialService_Internal.cs)（1 支，唯一子接口） |
| DTO 源文件 | [DataModels/Dial/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Dial/)（5 型，`SerializerClassName = Dial`，落 `DialJsonContext`） |
| 契约守卫 | [WechatDialContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDialContractGuards.cs)（DL1 路由与字段名 / DL2 单应用类型层级 / DL3 令牌绑定 / DL4 JSON 上下文）＋ [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 93662（仅企业自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：DL1~DL4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Dial/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

只有一支端点：**按时间范围分页拉取企业公费电话的拨打记录**（主叫、被叫、通话类型与时长）。

```
E1  POST /cgi-bin/dial/get_dial_record
```

拿到的是「谁在什么时候打了谁、各段时长多少」的流水，不含通话内容，也不是紧急通知的语音呼叫结果——后者属 [Emergency.md](../Emergency/Emergency.md) 的 `/cgi-bin/pstncc/` 路由族。

**两棵章节树互不承载**：`/cgi-bin/dial/`（公费电话）与 `/cgi-bin/pstncc/`（紧急通知应用）在官方文档树里分属两处，路由勿混淆（守卫类级 remarks 明示）。

本域形态是「**零端点父接口 + 唯一自建子接口**」——官方仅向企业自建应用开放，代开发与第三方**均暂不支持**，故既不设那两支子接口，父接口也不承载任何端点（DL2 断言继承链上恰只有一个子接口）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（DL3 对两支接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建子接口 `Wechat.AccessToken@Internal`；父接口不声明归属键 | 运行期 `WechatAppContext.GetTokenManager`（错配 fail-fast） |

```csharp
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddDialApi());
```

### 2.2 权限与业务前置

- 官方权限口径：调用应用需配置到「**公费电话 - 可调用接口的应用**」中。
- 查询区间是 `[start_time, end_time]` **双闭区间**；`end_time < start_time` ⇒ 官方返回 **600018**（无效的起止时间）。
- **起止时间最大跨度 30 天**，超过时官方**以 `end_time` 为基准向前取 30 天**（不是报错，是静默收窄）。
- **未指定起止时间 ⇒ 默认查询最近 30 天**范围内的数据。
- **应用可见范围外用户相关的记录会被过滤**（结果条数少不等于没打过的电话）。
- 分页用 `offset` / `limit`。
- 主被叫 `userid` 与 `phone` 属个人数据：示例用假值，不得写入日志、遥测或异常消息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `get_dial_record` | ✅ | ❌ 暂不支持 | ❌ 暂不支持 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkDialService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | **0** | 否 |
| `IWechatWorkInternalDialService` | `Mud.Wechat.Work` | 自建子接口（本域唯一入口） | 1 | 是 |

注册组 `Dial`，模块枚举 `WechatModule.Dial`，装配入口 `AddDialApi()`；子接口 `InheritedFrom = nameof(WechatWorkDialService)`。

形态对齐 `IWechatWorkMsgAuditPermitUserService` 一类的零端点父接口：父接口只是继承基座，**注入它拿不到任何方法**。

## 4. 端点

### E1. `GetDialRecordAsync` — 获取公费电话拨打记录

```csharp
Task<GetDialRecordResponse> GetDialRecordAsync(
    [Body] GetDialRecordRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/dial/get_dial_record`（官方即 POST，勿改成 GET） |
| 声明接口 | `IWechatWorkInternalDialService` |
| 令牌 | Query `access_token` |

**请求参数**（`GetDialRecordRequest`，四键均 `long?`）

| 字段（JSON） | 属性（C#） | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `start_time` | `StartTime` | `long?` | 否 | 起始时间（Unix 秒）；不填按官方默认（最近 30 天） |
| `end_time` | `EndTime` | `long?` | 否 | 结束时间；**不得小于 `start_time`**（否则 600018） |
| `offset` | `Offset` | `long?` | 否 | 分页偏移 |
| `limit` | `Limit` | `long?` | 否 | 分页大小（官方上限数值注释未留档，见 §7） |

> 字段名照官方原文的**下划线形态**（`start_time` / `end_time`），DL1 逐条锁定，不得驼峰化。

**响应参数**（`GetDialRecordResponse` 继承 `WechatWorkResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `record[]` | `Record` | `List<DialRecord>?` | 拨打记录列表 |
| `record[].call_time` | `CallTime` | `long?` | 拨打时间（Unix 秒） |
| `record[].total_duration` | `TotalDuration` | `long?` | 总时长 |
| `record[].call_type` | `CallType` | `long?` | 通话类型（取值集合注释未留档，见 §7） |
| `record[].caller` | `Caller` | `DialCaller?` | 主叫（**单对象**） |
| `caller.userid` / `caller.duration` | `Userid` / `Duration` | `string?` / `long?` | 主叫成员与时长；**主叫侧没有 `phone` 字段** |
| `record[].callee[]` | `Callee` | `List<DialCallee>?` | 被叫（**数组**：一通电话可有多方） |
| `callee[].userid` / `phone` / `duration` | `Userid` / `Phone` / `Duration` | `string?` / `string?` / `long?` | 被叫成员、号码与时长 |

**调用示例**（本域唯一端点：30 天窗口 + 分页）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Dial;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddDialApi());

// 零端点父接口不注册 DI：注入唯一的自建子接口
var dial = provider.GetRequiredService<IWechatWorkInternalDialService>();

// 官方跨度上限 30 天：超过时以 end_time 为基准向前取 30 天 ⇒ 自己按 30 天切窗更可控
long end = new DateTimeOffset(new DateTime(2026, 10, 28, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds();
long start = end - 30L * 24 * 3600;

var resp = await dial.GetDialRecordAsync(
    new GetDialRecordRequest { StartTime = start, EndTime = end, Offset = 0, Limit = 100 },
    cancellationToken);
WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/dial/get_dial_record");

long seconds = 0;
int calleeLegs = 0;
foreach (DialRecord rec in resp.Record ?? new List<DialRecord>())
{
    // 可见范围外成员相关的记录已被官方过滤；caller 是单对象、callee 是数组（一方可多方）。
    seconds += rec.TotalDuration ?? 0;
    calleeLegs += rec.Callee?.Count ?? 0;
}
// phone 与 userid 是个人数据：只输出聚合量，不输出号码与账号。
logger.LogInformation("公费电话总时长（秒）：{Seconds}，被叫段数：{CalleeLegs}", seconds, calleeLegs);
```

**本端点注意事项**

- **静默收窄**：跨度超 30 天不报错，而是以 `end_time` 为基准回取 30 天 ⇒ 想要「上两个月」必须自己切窗分页。
- **双闭区间**：相邻两次切窗若复用同一秒边界会重复计入该秒的记录。
- 结果被可见范围过滤 ⇒ 不能用它做企业全量话费对账，除非应用可见范围覆盖全员。
- 未指定起止时间时的「最近 30 天」是官方默认，不保证与本地时区日历对齐。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `/cgi-bin/dial/`（本域）vs `/cgi-bin/pstncc/`（[Emergency.md](../Emergency/Emergency.md)） | 官方两棵章节树：公费电话拨打记录 vs 紧急通知语音呼叫 | 把 pstncc 结果当话费用 |
| `caller`（单对象）vs `callee`（数组） | 主叫一人、被叫可多方；且**只有被叫带 `phone`** | 两侧共用一个类型 → 解析失败或丢号码 |
| `end_time < start_time` | 官方报 **600018**，而不是返回空集 | 把参数错误当「本月没打电话」 |
| 跨度超 30 天 | **静默回取** 30 天（不报错） | 误以为取到了完整区间 |
| `record` 为空 | 可能是可见范围过滤、也可能确实无记录 | 直接下结论「无通话」 |
| 父接口 | 零端点、不注册 DI | 注入 `IWechatWorkDialService` → 无方法可调 |
| 第三方 / 代开发子接口 | **不存在**（DL2 断言继承链只有一个子接口） | 找不到注入点时误判为本仓缺实现 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌走 Query 是官方契约（`MUD005`）；SDK 不做切窗、去重与可见范围预判。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 仍需自行判 `record` 是否被过滤 |
| 600018 | 官方业务码（接口注释留档） | 无效的起止时间（`end_time` < `start_time`） | 本域唯一留档的业务码 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

其余业务码（如应用未配置到「公费电话 - 可调用接口的应用」）**接口注释未留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `limit` / `offset` 上限 | 注释只说明是「分页查询」，未给 `limit` 最大值与 `offset` 越界行为 ⇒ 未逐项核验 | 官方 93662 |
| `call_type` 取值集合 | 注释未列通话类型枚举（`long?`）⇒ 未逐项核验 | 官方 93662 |
| `total_duration` / `duration` 的单位 | 注释未标单位（秒？）⇒ 未逐项核验，**不得据此换算话费** | 官方 93662 |
| 端点级频率限制 | 注释未记录 QPS/次数 ⇒ 未逐项核验 | 官方 93662 + 全局频率附录 |
| 业务错误码全集 | 除 600018 外未逐条摘录 | 官方 93662 |
| 官方文档正文 | 本文只摘录本仓 XML 注释 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Dial/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Dial/README.md)（端点速览、令牌与认证、双闭区间与 30 天口径）
- 易混邻域：[Emergency.md](../Emergency/Emergency.md)（`/cgi-bin/pstncc/` 紧急通知语音呼叫）
- 同形态参照：[Contacts/ContactRules.md](../Contacts/ContactRules.md) 与 [CorpGroup/CorpGroupRules.md](../CorpGroup/CorpGroupRules.md)（零端点父接口 + 唯一自建子接口）
- 官方页：[获取公费电话拨打记录（93662）](https://developer.work.weixin.qq.com/document/path/93662)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
