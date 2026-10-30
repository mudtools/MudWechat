# 紧急通知（语音电话呼叫与接听状态）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Emergency` 域 · 紧急通知族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkEmergencyService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Emergency/IWechatWorkEmergencyService.cs)（**零端点父接口**）· [IWechatWorkInternalEmergencyService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Emergency/IWechatWorkEmergencyService_Internal.cs)（2 支，唯一子接口） |
| DTO 源文件 | [DataModels/Emergency/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Emergency/)（5 型，含 `RequestModel/`；`SerializerClassName = Emergency`，落 `EmergencyJsonContext`） |
| 契约守卫 | [WechatEmergencyContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatEmergencyContractGuards.cs)（EM1 路由表 / EM2 层级与开放面 / EM3 令牌绑定 / EM4 JSON 上下文全量 / EM5 官方契约陷阱）＋ [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 自建 91627（发起语音电话）/ 91628（获取接听状态）· 代开发页 97115 / 97116 与自建页逐字一致，但权限表标注「暂不支持」 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：EM1~EM5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Emergency/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

「**打电话叫员工回来看消息**」这一条最小闭环：

| 端点 | 用途 |
|---|---|
| E1 `pstncc/call` | 向一批成员发起自动语音来电提醒，返回每人的 `callid` |
| E2 `pstncc/getstates` | 按 `callid` + 单人查询是否接听、通话时长与结果原因 |

两步之间没有回调、也没有批量状态接口——E2 是**一次查一个人**，且只能查**七天内的 `callid`**。

官方仅向**企业自建应用**开放这 2 支端点（权限表对代开发与第三方均标注「暂不支持」），因此落位是「**零端点父接口 + 唯一自建子接口承载端点**」，继承链上不得出现代开发 / 第三方子接口（EM2 是能力漂移守卫）。

⚠️ 特别注意：**代开发文档页 97115 / 97116 与自建页逐字一致**，看上去像「也开放了」，但**权限表仍标「暂不支持」** ⇒ 不得据文档页存在就补代开发子接口（守卫类级 remarks 明示）。

与 [Dial.md](../Dial/Dial.md) 的分工：本域是 `/cgi-bin/pstncc/`（紧急通知语音呼叫），公费电话拨打记录在 `/cgi-bin/dial/`，两棵官方章节树互不承载。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（EM3 对两支接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建子接口 `Wechat.AccessToken@Internal`；父接口不声明归属键 | 运行期归属校验（错配 fail-fast） |

```csharp
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "emergency-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddEmergencyApi());
```

### 2.2 权限与业务前置

- 自建应用须配置到「**紧急通知 - 可调用接口的应用**」中（未关联 ⇒ `301049`）。
- 企业侧须**开通紧急通知应用**（未开启 ⇒ `301050`），且**账户有余额**（余额不足 ⇒ `301051`）——语音呼叫是计费能力。
- E1：`callee_userid` 官方必填且**不能为空**（数组形态，见 §5）。
- E2：`callee_userid` 与 `callid` **均必填且不能为空**，且 `callee_userid` 是**单数字符串**；**仅支持查询七天内的 `callid` 状态**。
- 两端点**无独立频率限制**，走官方全局访问频率限制。
- **自 2023-12-01 0 点起不再支持通过系统应用 secret 调用**（存量企业暂不受影响）。
- 被呼叫人 `userid` 与通话结果属个人数据：示例用假值，不得落日志、遥测或异常消息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `pstncc/call` | ✅ | ❌ 权限表「暂不支持」 | ❌ 权限表「暂不支持」（文档页 97115 存在但权限未开放） |
| E2 `pstncc/getstates` | ✅ | ❌ | ❌（同上，97116） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkEmergencyService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | **0** | 否 |
| `IWechatWorkInternalEmergencyService` | `Mud.Wechat.Work` | 自建子接口（本域唯一入口） | 2 | 是 |

注册组 `Emergency`，模块枚举 `WechatModule.Emergency`，装配入口 `AddEmergencyApi()`；子接口 `InheritedFrom = nameof(WechatWorkEmergencyService)`。

形态对齐政民沟通巡查/居民上报族与身份验证二次验证族：零端点父接口 + 唯一自建子接口。

## 4. 端点

### E1. `CallAsync` — 发起语音电话

```csharp
Task<EmergencyCallResponse> CallAsync(
    [Body] EmergencyCallRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/pstncc/call` |
| 声明接口 | `IWechatWorkInternalEmergencyService` |
| 令牌 | Query `access_token` |

**请求参数**（`EmergencyCallRequest`）

| 字段（JSON） | 属性（C#） | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `callee_userid` | `CalleeUserid` | `List<string>?` | 是 | 被呼叫人 userid **列表**，官方必填且不能为空（参数键名是单数、值是数组） |

**响应参数**（`EmergencyCallResponse` 继承 `WechatWorkResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `states[]` | `States` | `List<EmergencyCallState>?` | 逐被呼叫人的呼叫结果 |
| `states[].code` | `Code` | `int?` | 呼叫结果状态（取值集合未留档，见 §7） |
| `states[].callid` | `Callid` | `string?` | 唯一标识一通呼叫的 id ⇒ **E2 的入参** |
| `states[].userid` | `Userid` | `string?` | 该条结果对应的被呼叫人 |

**调用示例**（本域主示例：批量发起 → 逐人查接听状态）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Emergency;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "emergency-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddEmergencyApi());

// 零端点父接口不注册 DI：注入唯一的自建子接口
var emergency = provider.GetRequiredService<IWechatWorkInternalEmergencyService>();

// ① 发起语音电话：callee_userid 是数组（官方键名为单数）
var call = await emergency.CallAsync(
    new EmergencyCallRequest { CalleeUserid = new List<string> { "zhangsan", "lisi" } }, // 假值
    cancellationToken);
WechatWorkException.ThrowIfFailed(call, "/cgi-bin/pstncc/call");
// 301049 未在紧急通知应用关联 / 301050 未开启 / 301051 余额不足
foreach (EmergencyCallState state in call.States ?? new List<EmergencyCallState>())
{
    if (state.Callid is not { Length: > 0 } callid)
    {
        logger.LogWarning("成员呼叫未返回 callid，code={Code}", state.Code);
        continue;
    }
    logger.LogInformation("已发起呼叫，code={Code}（callid 不打印）", state.Code);

    // ② 查接听状态：callee_userid 是单数字符串，且只能查七天内的 callid
    var states = await emergency.GetCallStatesAsync(
        new EmergencyGetCallStatesRequest { CalleeUserid = state.Userid ?? string.Empty, Callid = callid },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(states, "/cgi-bin/pstncc/getstates");

    // istalked 为整数形态（不是 bool）；talktime 单位秒。
    logger.LogInformation("接听={IsTalked} 通话时长={TalkTime} 原因={Reason} 发起时间={CallTime}",
        states.Istalked, states.Talktime, states.Reason, states.Calltime);
}
```

**本端点注意事项**

- `states[]` 是**逐人**结果：批量呼叫要按数组取每个人的 `callid`，不能用整体 `errcode` 代替个人结果。
- 呼叫是**计费动作**：先确认企业已开启且有余额，再决定是否接入告警链路。
- `callid` 是标识而非凭据，但仍不必写进日志。

### E2. `GetCallStatesAsync` — 获取接听状态

```csharp
Task<EmergencyGetCallStatesResponse> GetCallStatesAsync(
    [Body] EmergencyGetCallStatesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/pstncc/getstates`（`getstates` 全连写；**查询语义也走 POST**） |
| 请求时效 | 仅支持查询**七天内**的 `callid` |

**请求参数**（`EmergencyGetCallStatesRequest`）

| 字段（JSON） | 属性（C#） | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `callee_userid` | `CalleeUserid` | `string?` | 是 | **单数字符串**（与 E1 的数组形态不同），不能为空 |
| `callid` | `Callid` | `string?` | 是 | E1 返回的 `callid`，不能为空 |

**响应参数**（`EmergencyGetCallStatesResponse` 继承 `WechatWorkResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `istalked` | `Istalked` | `int?` | 是否接听（**整数形态，不是布尔**） |
| `calltime` | `Calltime` | `long?` | 呼叫发起时间戳 |
| `talktime` | `Talktime` | `int?` | 通话时长（**秒**） |
| `reason` | `Reason` | `int?` | 呼叫结果状态（取值集合未留档，见 §7） |

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 `callee_userid`（`List<string>`）vs E2 `callee_userid`（`string`） | **同名键、两种形态**（官方两处示例不一致，EM5 分别锁定） | 合并成一个请求 DTO → 一侧序列化不合法 |
| 键名 `callee_userid` 的单数写法 | 参数键是单数、E1 的值却是数组 | 手拼 `callee_userids` → 官方不认 |
| `istalked` | `int?` 而非 `bool` | 改成 `bool` → 与线上传输形态漂移（EM5 打红） |
| E2 的动词与时效 | `POST` + 只能查**七天内** `callid` | 当 GET 改写 / 查历史 callid → 失败 |
| 代开发文档页 97115 / 97116 | 与自建页逐字一致，但权限表「暂不支持」 | 据页面存在补代开发子接口 → 打红 EM2 |
| `/cgi-bin/pstncc/` vs `/cgi-bin/dial/` | 紧急通知呼叫结果 vs 公费电话拨打流水（两棵树） | 互相当对方的数据用 |
| `states[].code` vs 顶层 `errcode` | 前者是**逐被呼叫人**的呼叫结果状态 | 只判顶层 → 漏掉个别人呼叫失败 |
| 父接口 | 零端点、不注册 DI | 注入 `IWechatWorkEmergencyService` → 无方法可调 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做七天窗口校验与重试**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 仍要逐条看 `states[].code`（E1） |
| 301049 | 官方专属码（接口注释留档） | 调用接口的应用未在紧急通知应用中关联 | 配置面问题，非代码缺陷 |
| 301050 | 官方专属码 | 紧急通知应用未开启 | 需企业侧开启 |
| 301051 | 官方专属码 | 紧急通知应用余额不足 | 计费前置；告警链路要有降级 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

`states[].code` 与 `reason` 的取值集合属官方枚举，本仓未留档 ⇒ 见 §7，不要凭记忆写判据。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `states[].code` / `reason` 取值集合 | 注释只说明「呼叫结果状态」，未列枚举 ⇒ 未逐项核验 | 官方 91627 / 91628 |
| `istalked` 的整数取值 | 注释未给 1/0 或其它值的确切含义 ⇒ 未逐项核验 | 官方 91628 |
| E1 单次呼叫的人数上限 | 注释只说「必填且不能为空」，未给最大条数 ⇒ 未逐项核验 | 官方 91627 |
| 全局访问频率的具体阈值 | 注释说「无独立频率限制、走官方全局限制」，未在本域摘录数值 | 官方访问频率限制附录 |
| 计费口径（每次呼叫消耗多少余额） | 本域不含计费面 ⇒ 无源可核 | 官方紧急通知应用说明 |
| 官方文档正文 | 本文只摘录本仓 XML 注释 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Emergency/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Emergency/README.md)（端点速览、`callee_userid` 双形态、代开发页存在但权限未开放）
- 易混邻域：[Dial.md](../Dial/Dial.md)（`/cgi-bin/dial/` 公费电话拨打记录）
- 同形态参照：[Contacts/ContactRules.md](../Contacts/ContactRules.md) · [CorpGroup/CorpGroupRules.md](../CorpGroup/CorpGroupRules.md) · [DeviceData.md](../DeviceData/DeviceData.md)（零端点父接口 + 唯一子接口）
- 官方页：[发起语音电话（自建 91627）](https://developer.work.weixin.qq.com/document/path/91627) · [获取接听状态（91628）](https://developer.work.weixin.qq.com/document/path/91628)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
