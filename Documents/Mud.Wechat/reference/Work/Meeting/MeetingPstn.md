# 电话入会（PSTN）· 会中外呼、外呼状态与临时成员 ID

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 电话入会（PSTN）管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingPstnService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingPstnService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingPstnService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingPstnService_Internal.cs)（**唯一**子接口，承载全部 3 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`PstnBatchCalloutRequest/Response`、`PstnGetCalloutStatusRequest/Response`、`PstnGetTmpOpenidRequest/Response`、`PstnPhoneNumber`、`PstnCalloutPhoneNumber`、`PstnCalloutStatusPhoneNumber`、`PstnTmpOpenidPhoneNumber`；本族在 MT4 的逐族分解中计 10 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(3)`、全部挂 `phone/` 段且官方即 POST / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 3)` / MT3 令牌绑定 / MT4 上下文登记 / MT5 契约陷阱）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 批量外呼 [98823](https://developer.work.weixin.qq.com/document/path/98823) · 获取外呼状态 98824 · 获取 tmp_openid 98825（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（批量外呼 → 状态翻页 → 号码反查 `tmp_openid`））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 / MT5 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 6/8/9 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

用**电话（PSTN）**把成员拉进正在进行的会议，并取回电话入会成员的身份 ID，3 支端点全部挂 `/cgi-bin/meeting/phone/`：

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `BatchCalloutAsync` | `phone/callout` | 批量外呼（每次 ≤50 路） |
| E2 | `GetCalloutStatusAsync` | `phone/get_callout_status` | 分页查外呼状态（含 `tmp_openid`） |
| E3 | `GetTmpOpenidAsync` | `phone/get_tmp_openid` | 用号码反查已接通成员的 `tmp_openid`（≤20 个号码） |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）——第三方与代开发章节均无对应 API。

**本族是「呼叫 + 身份」，不是电话账单面**：没有通话时长、费用与录音开关；`tmp_openid` 的下游用途是会控（`realcontrol/*` 的被操作者标识）与报名反查（高级族 `enroll/query_by_tmp_openid`）。

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

- 自建应用须配置在「**可调用接口的应用**」列表中；**仅允许操作 / 获取该应用创建的会议的数据**。
- **支持「会议未开始」与「会中外呼」两种时机**；**Webinar（网络研讨会）暂不支持外呼**。
- E1 **每次调用批量外呼 50 路**；支持境外电话号与分机号。
- E2 **分页最大 100 条**；口径明确：**无论从客户端、API 还是 Web 端发起的外呼均可获取，不针对呼叫人做数据隔离**。
- E3 `phone_numbers` **上限 20 个**；**仅当 PSTN 接通后返回**；**同一座机号可能对应多个 `tmp_openid`**（一个座机同时拨入的情形）；支持查询会议下 PSTN 的**历史** `tmp_openid` 列表。
- **`area` 是中国传 `86`**（`int?` 形态，不是 `"+86"` 也不是 `"0086"`）。
- 电话号码属**个人敏感数据**：示例一律假值，`phone` / 外呼结果**不得进日志、遥测或异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E3 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingPstnService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingPstnService`） |
| 不可注入 | `IWechatWorkMeetingPstnService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `BatchCalloutAsync` — 批量外呼

**签名**

```csharp
[Post("/cgi-bin/meeting/phone/callout")]
Task<PstnBatchCalloutResponse> BatchCalloutAsync(
    [Body] PstnBatchCalloutRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/phone/callout` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/phone/callout?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `PstnBatchCalloutRequest` |

**请求体**：`meetingid`（官方必填）+ `phone_numbers`（`List<PstnPhoneNumber>?`，官方必填，**每次 ≤50 路**）。

`PstnPhoneNumber`：`area`（`int?`，官方必填，中国 `86`）+ `phone`（`string?`，官方必填，手机号或固定电话总机号）+ `extension_number`（`string?`，固定电话分机号）。

**返回** `PstnBatchCalloutResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `phone_numbers` | `PhoneNumbers` | `List<PstnCalloutPhoneNumber>?` | **成功外呼**的号码（`area` / `phone` / `extension_number` / `status`） |
| `invalid_phone_numbers` | `InvalidPhoneNumbers` | `List<PstnPhoneNumber>?` | **不合法**的外呼号码 |

> **拨打后立刻返回呼叫状态**；后续状态变化须靠 E2 查询接口与回调事件——`status` 是**瞬时值**。

**调用示例**（外呼 → 分页取状态 → 用号码反查 tmp_openid；号码为假值且不落日志）

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
var pstn = provider.GetRequiredService<IWechatWorkInternalMeetingPstnService>();

// ④ E1 批量外呼：一次 ≤50 路；area 中国传 86；Webinar 不支持外呼
var callout = await pstn.BatchCalloutAsync(new PstnBatchCalloutRequest
{
    Meetingid = "demo-meeting-id",           // 须为本应用创建的会议
    PhoneNumbers = new List<PstnPhoneNumber>
    {
        new PstnPhoneNumber { Area = 86, Phone = "13800000000" },              // 假号
        new PstnPhoneNumber { Area = 86, Phone = "075500000000", ExtensionNumber = "8001" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(callout, "/cgi-bin/meeting/phone/callout");

// 只记条数，不记号码：成功列表与不合法列表分开的两个集合
logger.LogInformation("外呼受理 {Accepted} 路、号码不合法 {Rejected} 路",
    callout.PhoneNumbers?.Count ?? 0, callout.InvalidPhoneNumbers?.Count ?? 0);

// ⑤ E2 分页取外呼状态：不针对呼叫人做数据隔离（客户端/API/Web 发起的都看得到）
string? cursor = null;
var tmpOpenids = new List<string>();
while (true)
{
    var status = await pstn.GetCalloutStatusAsync(new PstnGetCalloutStatusRequest
    {
        Meetingid = "demo-meeting-id",
        Cursor = cursor,
        Limit = 100,                          // 默认 20、最大 100
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(status, "/cgi-bin/meeting/phone/get_callout_status");

    foreach (PstnCalloutStatusPhoneNumber item in status.PhoneNumbers ?? new List<PstnCalloutStatusPhoneNumber>())
    {
        if (!string.IsNullOrEmpty(item.TmpOpenid)) tmpOpenids.Add(item.TmpOpenid);
    }

    if (status.HasMore != true) break;        // 判停只看 has_more
    cursor = status.NextCursor;
}

// ⑥ E3 号码反查 tmp_openid：≤20 个号码、仅 PSTN 接通后返回；同一座机可能对应多个 tmp_openid
var resolved = await pstn.GetTmpOpenidAsync(new PstnGetTmpOpenidRequest
{
    Meetingid = "demo-meeting-id",
    PhoneNumbers = new List<PstnPhoneNumber>
    {
        new PstnPhoneNumber { Area = 86, Phone = "13800000000" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(resolved, "/cgi-bin/meeting/phone/get_tmp_openid");
foreach (PstnTmpOpenidPhoneNumber item in resolved.TmpOpenidList ?? new List<PstnTmpOpenidPhoneNumber>())
{
    tmpOpenids.Add(item.TmpOpenid!);          // 该 tmp_openid 可继续用于会控操作与报名反查
}
logger.LogInformation("共取得 {Count} 个 PSTN 成员的 tmp_openid", tmpOpenids.Count);
```

### E2. `GetCalloutStatusAsync` — 获取会议的外呼状态

**签名**

```csharp
[Post("/cgi-bin/meeting/phone/get_callout_status")]
Task<PstnGetCalloutStatusResponse> GetCalloutStatusAsync(
    [Body] PstnGetCalloutStatusRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）+ `cursor`（首次可不传）+ `limit`（默认 **20**、最大 **100**）。

**返回** `PstnGetCalloutStatusResponse`：`phone_numbers`（`List<PstnCalloutStatusPhoneNumber>?`，比 E1 的成功列表**多一个 `tmp_openid`**）+ `next_cursor` + `has_more`（`bool?`）。

### E3. `GetTmpOpenidAsync` — 电话入会成员的 `tmp_openid`

**签名**

```csharp
[Post("/cgi-bin/meeting/phone/get_tmp_openid")]
Task<PstnGetTmpOpenidResponse> GetTmpOpenidAsync(
    [Body] PstnGetTmpOpenidRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid`（官方必填）+ `phone_numbers`（官方必填，**上限 20**）。

**返回**：`tmp_openid_list`（`List<PstnTmpOpenidPhoneNumber>?`：`area` / `phone` / `extension_number` / `tmp_openid`）。

> `tmp_openid` 是 **PSTN 成员的识别标识**：成员拨号进入后也可在「获取实时会中成员列表」等 API 与回调事件中拿到；它是**当场会议的临时 ID**，不可跨会议复用（同域 `tmp_external_userid` 的同类约束，见域 README 第 6 节）。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **三套批次数值各不同** | E1 每次 ≤**50** 路；E2 分页最大 **100**；E3 `phone_numbers` ≤**20** ⇒ 不可互相借用 | 用 100 去外呼 / 用 20 去分页 |
| 成功与不合法**分两个列表** | E1 应答 `phone_numbers`（受理）+ `invalid_phone_numbers`（不合法）⇒ **顶层 `errcode=0` 不代表每个号码都拨出** | 只判 `ThrowIfFailed` 就认为全量外呼成功 |
| **`status` 是瞬时值** | 注释明示「拨打后立刻返回呼叫状态，可通过查询接口和回调事件获取后续状态变化」 | 把首次应答当最终接通结果 |
| **E2 不做呼叫人隔离** | 「无论从客户端、API 还是 Web 端发起的外呼均可获取」⇒ 本族可见面比一般域宽 | 以为只能看到自己发起的呼叫 |
| **一座机可对应多个 `tmp_openid`** | E3 注释原文（同一座机同时拨入）⇒ 返回值是数组，不可假定一一对应 | 按号码建唯一索引、丢数据 |
| **仅接通后返回** | E3「仅当 PSTN 接通后返回」，未接通查不到 `tmp_openid` | 拿空列表断定号码无效 |
| `area` 是 **`int` 86** | `PstnPhoneNumber.Area` 为 `int?`，中国传 `86`（不是 `+86`/`0086`，也不同于会议嘉宾 `MeetingGuest.Area` 的 `string?`） | 复制嘉宾侧写法 → 类型或值不合法 |
| `tmp_openid` 的下游 | 用于会控（`realcontrol/*` 的 `operated_user.tmp_openid` + `instance_id`）与报名反查（`enroll/query_by_tmp_openid`） | 当 `external_userid` / `openid` 用 |
| **Webinar 不支持外呼** | E1 与 E2 注释两处都写「Webinar 暂不支持外呼」 | 对网络研讨会 ID 调用 |
| 会议所有权闸门 | 「仅允许操作 / 获取该应用创建的会议的数据」⇒ 别的渠道创建的会议拿不到状态 | 用别的应用的 `meetingid` |
| 电话号码是敏感数据 | `area` / `phone` / `extension_number` 不得进日志、遥测与异常消息 | 排障时整包打日志 |
| 全部官方即 POST | 含查询语义的 E2/E3 也是 POST + 请求体（全域仅 `layout/list_template` 是 GET） | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验号码数量与 `area` 取值**、**不做批次裁剪与轮询编排**、**不做号码脱敏**（脱敏属宿主日志层）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 接口调用成功 | E1 仍要读 `invalid_phone_numbers`；E2 仍要按 `has_more` 续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

号码越界、`meetingid` 非本应用创建、目标为 Webinar、会议状态不允许外呼、`cursor` 失效等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98823 / 98824 / 98825 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| `status` 取值枚举 | 注释只说「外呼状态」，**未列取值字符串集合**（也未给「接通/未接/拒接」等终态）⇒ 未逐项核验 | 官方 98823 / 98824 与「电话入会」回调事件页 |
| 呼叫相关回调事件 | 注释提「可通过查询接口**和回调事件**获取后续状态变化」，但事件键与载荷不在本域 ⇒ 未在本域核验 | `Mud.Wechat.Work.Callback` 的事件族 |
| 外呼费用与额度 | 本族不含计费面；企业可用路数与计费口径未记录 | 官方电话入会（PSTN）说明页 |
| `tmp_openid` 有效期 | 注释只说「当场会议的成员临时 ID」，未记跨会议/超时后的失效行为 | 官方 98825 |
| 境外号码支持面 | 注释称「支持境外电话号及分机号的外呼」，但可用国家码清单未留档 | 官方 98823 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 6 节外部参会人边界 `tmp_openid` / `tmp_external_userid`；第 9 节批次上限表逐端点数值）
- 同域近邻：[MeetingControl.md](./MeetingControl.md)（`tmp_openid` + `instance_id` 作为被操作者标识）· [MeetingAdvanced.md](./MeetingAdvanced.md)（`enroll/query_by_tmp_openid`、实时会中成员列表）· [MeetingWebinar.md](./MeetingWebinar.md)（本族对其不支持）
- 跨域边界：MRA 设备的会中身份同样用 `tmp_openid`，见 [MeetingMra.md](./MeetingMra.md)
- 官方页：[批量外呼（98823）](https://developer.work.weixin.qq.com/document/path/98823) · [获取外呼状态（98824）](https://developer.work.weixin.qq.com/document/path/98824) · [获取 tmp_openid（98825）](https://developer.work.weixin.qq.com/document/path/98825)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
