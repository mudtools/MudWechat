# 管理日历

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Schedule` 域 · 管理日历族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkScheduleCalendarService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Calendar/IWechatWorkScheduleCalendarService.cs)（父，承载全部 4 支）＋ 自建 / 代开发 / 第三方三个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Calendar/IWechatWorkScheduleCalendarService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Calendar/IWechatWorkScheduleCalendarService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Calendar/IWechatWorkScheduleCalendarService_ThirdParty.cs)） |
| DTO 源文件 | [DataModels/Schedule/Calendar/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Schedule/Calendar/)（13 型，`SerializerClassName = Schedule` → `ScheduleJsonContext`） |
| 契约守卫 | [WechatScheduleContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs)（SC1 路由表 / SC2 父 4 + 三子 0 / SC3 令牌绑定 / SC5 字段名）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 创建 [93647](https://developer.work.weixin.qq.com/document/path/93647) / [93702](https://developer.work.weixin.qq.com/document/path/93702) / [96823](https://developer.work.weixin.qq.com/document/path/96823)；更新 [97716](https://developer.work.weixin.qq.com/document/path/97716) / [97783](https://developer.work.weixin.qq.com/document/path/97783) / [97758](https://developer.work.weixin.qq.com/document/path/97758)；详情 [97717](https://developer.work.weixin.qq.com/document/path/97717) / [97784](https://developer.work.weixin.qq.com/document/path/97784) / [97759](https://developer.work.weixin.qq.com/document/path/97759)；删除 [97718](https://developer.work.weixin.qq.com/document/path/97718) / [97785](https://developer.work.weixin.qq.com/document/path/97785) / [97760](https://developer.work.weixin.qq.com/document/path/97760)（依次自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SC1 / SC2 / SC3 / SC5 断言 + 4 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

管理「日历」这一容器对象：创建（E1）→ 更新（E2）→ 按 ID 批查（E3）→ 删除（E4）。E1 返回的 `cal_id` 是后续一切日程操作的定位键——创建日程时 `schedule.cal_id` 填的就是它。

**不该用本族的场景**：日程本身（创建 / 改期 / 参与人 / 取消）用 [Schedule.md](./Schedule.md)；会议室预定用 [MeetingRoom/Booking.md](../MeetingRoom/Booking.md)，两者不是同一棵资源树。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` 三选一 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（三族同键，**不按应用类型拆分**） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

令牌由基座自动换取、缓存与刷新，调用方不接触 `corpsecret` / `permanent_code`；第三方与代开发须先切企业作用域（§2.2）。

### 2.2 权限与业务前置

- 第三方 / 代开发调用前须 `using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`，否则取到的是应用级而非授权企业级令牌（参数名 `permanentCode` 是凭据，**不得落日志**）。
- 官方容量闸（父接口 remarks + DTO 注释）：管理员须在通知范围成员中且 ≤**3** 人；通知范围 ≤**2000** 人；公开范围成员 ≤**1000**、部门 ≤**100**；每人最多创建或订阅 **100** 个公共日历；每企业最多 **20** 个全员日历。
- 可见范围约束：E3 只能取到**本应用创建**的日历；`userid`、成员名单与日历标题/描述属个人或工作数据，示例一律假值且不得落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅ | ✅ | ✅ |

三类应用开放面等宽 ⇒ 四个端点全部声明在公共父接口，三个应用类型子接口是**零端点空标记**。禁为「让注入更顺手」往子接口复制端点（SC2 的父/子计数会当场打红）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkScheduleCalendarService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 4 | 否 |
| `IWechatWorkInternalScheduleCalendarService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderScheduleCalendarService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyScheduleCalendarService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.Schedule` / `AddScheduleApi()`；注册组名 `Schedule`。
- 端点索引：E1 `AddCalendarAsync`（建日历） · E2 `UpdateCalendarAsync`（覆盖式改） · E3 `GetCalendarAsync`（按 ID 批查） · E4 `DelCalendarAsync`（删日历）。

---

## 4. 端点

### E1. `AddCalendarAsync` — 创建日历

```csharp
[Post("/cgi-bin/oa/calendar/add")]
Task<AddScheduleCalendarResponse> AddCalendarAsync(
    [Body] AddScheduleCalendarRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/calendar/add`（官方路由是 `add`，**不是** `create`） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`AddScheduleCalendarRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Calendar` | `calendar` | `ScheduleCalendar?` | 是 | 日历信息，字段见下表 |
| `AgentId` | `agentid` | `int?` | 否 | **仅旧的第三方多应用套件需填**，新套件不填 |

`ScheduleCalendar`（创建侧）：`summary`（**必填**，1~128 字符）、`color`（**必填**，RGB 16 进制，如 `#0000FF`）、`description`（0~512 字符）、`admins`（≤3 且须在 `shares` 中）、`set_as_default`（0/1，仅创建可传，**第三方不支持**）、`is_public`（0/1，仅创建可传）、`public_range`（仅公共日历有效；`userids` ≤1000、`partyids` ≤100，**连写复数**）、`is_corp_calendar`（0/1，仅创建可传；全员日历**必须**给 `public_range`，且不支持颜色 / 默认日历 / 只读权限）、`shares[]`（`userid` + `permission`；`permission` 不填默认可查看，注释列 `1` 可查看 / `3` 仅查看闲忙）。

**响应参数**（`AddScheduleCalendarResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `cal_id` | `CalId` | `string?` | 新日历 ID，后续一切定位键 |
| `fail_result.shares` | `FailResult` → `Shares` | `ScheduleCalendarFailResult?` | **整体成功仍可能有无效成员**：元素 `errcode` / `errmsg` / `userid` |

**本端点注意事项**

- `errcode == 0` 不代表通知范围成员全部有效，必须另查 `fail_result.shares`（§5）。
- 全员日历（`is_corp_calendar = 1`）本身也是公共日历，`public_range` 必填；每人 100 个公共日历、每企业 20 个全员日历的上限由官方判定，SDK 不预检。

### E2. `UpdateCalendarAsync` — 更新日历（覆盖式）

```csharp
[Post("/cgi-bin/oa/calendar/update")]
Task<UpdateScheduleCalendarResponse> UpdateCalendarAsync(
    [Body] UpdateScheduleCalendarRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/calendar/update` |
| 凭据 | Query `access_token` |

**请求参数**（`UpdateScheduleCalendarRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `SkipPublicRange` | `skip_public_range` | `int?` | 否 | `1` = 本次**不更新**可订阅范围；默认 `0` = **会**更新 |
| `Calendar` | `calendar` | `ScheduleCalendar?` | 是 | `cal_id` **更新侧必填**；`is_public` / `is_corp_calendar` / `set_as_default` 不可更新（更新参数表不含三者） |

**响应参数**（`UpdateScheduleCalendarResponse`）：仅 `fail_result.shares`（同 E1 形态），无业务负载。

**本端点注意事项**

- **更新是覆盖式而非增量式**：未传字段是否被重置为默认值本仓未留档 ⇒ 写侧一律「先读后写」全量提交。
- 不想动可订阅范围时必须显式 `skip_public_range = 1`，否则会按请求体重写。

### E3. `GetCalendarAsync` — 获取日历详情

```csharp
[Post("/cgi-bin/oa/calendar/get")]
Task<GetScheduleCalendarResponse> GetCalendarAsync(
    [Body] GetScheduleCalendarRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/calendar/get`（**读取也是 POST**） |
| 凭据 | Query `access_token` |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `CalIdList` | `cal_id_list` | `List<string>?` | 是 | 一次最多 **1000** 条；只能取本应用创建的日历 |

**响应参数**（`calendar_list[]` → `ScheduleCalendarInfo`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `cal_id` | `CalId` | `string?` | 日历 ID |
| `adminis` | `Adminis` | `List<string>?` | 管理员列表——**键名照抄官方返回示例的 `adminis`**（参数表作 `admins`，以示例为准，勿「顺手修正」） |
| `summary` / `color` / `description` | 同名 | `string?` | 标题 / 颜色 / 描述 |
| `shares` | `Shares` | `List<ScheduleCalendarShare>?` | 通知范围成员 + `permission` |
| `is_public` / `is_corp_calendar` | 同名 | `int?` | 公共日历 / 全员日历 |
| `public_range` | `PublicRange` | `ScheduleCalendarPublicRange?` | `userids` / `partyids` |

**本端点注意事项**

- 解析侧必须按 `adminis` 读；按 `admins` 写的反序列化逻辑会得到空列表。

### E4. `DelCalendarAsync` — 删除日历

```csharp
[Post("/cgi-bin/oa/calendar/del")]
Task<WechatWorkResponse> DelCalendarAsync(
    [Body] DelScheduleCalendarRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/calendar/del` |
| 凭据 | Query `access_token` |

**请求参数**：`cal_id`（`string?`，官方必填）。

**响应参数**：仅 `errcode` / `errmsg`（无业务负载）⇒ 返回类型是 `WechatWorkResponse`，**不得新建空响应 DTO**（SC1 单独断言返回类型）。

**本端点注意事项**

- 删除不可回退；与「取消日程」（[Schedule.md](./Schedule.md) E7）语义不同，后者保留可查记录（`status = 1`）。

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Schedule;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";
    o.AppType = WechatAppType.Internal;          // 第三方 / 代开发换 AppType 并切企业作用域
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddScheduleApi());

var calendars = provider.GetRequiredService<IWechatWorkInternalScheduleCalendarService>();

var memberIds = new List<string> { "zhangsan", "lisi" };   // 假值：真实名单属个人数据，不得落日志
var add = await calendars.AddCalendarAsync(new AddScheduleCalendarRequest
{
    Calendar = new ScheduleCalendar
    {
        Summary = "示例项目日历",                              // 必填，1 ~ 128 字符
        Color = "#0000FF",                                    // 必填，RGB 16 进制
        Description = "示例描述",                              // 0 ~ 512 字符
        Admins = new List<string> { "zhangsan" },             // ≤3，且必须是 shares 成员
        Shares = new List<ScheduleCalendarShare>
        {
            new ScheduleCalendarShare { Userid = "zhangsan", Permission = 1 },
            new ScheduleCalendarShare { Userid = "lisi", Permission = 3 },   // 3 = 仅查看闲忙
        },
        IsPublic = 0,                                         // 仅创建可传，属性不可更新
        SetAsDefault = 0,                                     // 第三方不支持
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(add, "/cgi-bin/oa/calendar/add");

// errcode == 0 仍可能有无效通知成员：逐条处理 fail_result
if (add.FailResult?.Shares is { Count: > 0 } invalidShares)
{
    foreach (ScheduleCalendarFailShare share in invalidShares)
    {
        logger.LogWarning("日历成员无效：errcode={Errcode} errmsg={Errmsg}", share.Errcode, share.Errmsg);
    }
}
string calId = add.CalId ?? throw new InvalidOperationException("创建日历未返回 cal_id");

// 覆盖式更新：不想动可订阅范围必须显式 skip_public_range = 1
var update = await calendars.UpdateCalendarAsync(new UpdateScheduleCalendarRequest
{
    SkipPublicRange = 1,
    Calendar = new ScheduleCalendar { CalId = calId, Summary = "示例项目日历（改名）" },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(update, "/cgi-bin/oa/calendar/update");

// 按 ID 批查（≤1000）：管理员键是 adminis
var detail = await calendars.GetCalendarAsync(
    new GetScheduleCalendarRequest { CalIdList = new List<string> { calId } }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/calendar/get");

ScheduleCalendarInfo? info = detail.CalendarList?.FirstOrDefault();
logger.LogInformation("日历 {CalId} 管理员 {AdminCount} 人", info?.CalId, info?.Adminis?.Count ?? 0);

// 删除不可回退
var del = await calendars.DelCalendarAsync(
    new DelScheduleCalendarRequest { CalId = calId }, cancellationToken);
WechatWorkException.ThrowIfFailed(del, "/cgi-bin/oa/calendar/del");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `admins` vs **`adminis`** | 响应日历管理员键官方示例作 `adminis`（创建 / 更新的请求侧才是 `admins`） | 响应按 `admins` 解析 → 管理员恒空 |
| 创建路由 `add` | 不是 `create`；删除是 `del` 不是 `delete` | 自造路由 → 404 |
| 覆盖式更新 | E2 默认会重写 `public_range`；唯一开关是 `skip_public_range = 1` | 只传 title 更新 → 公开范围被清空 |
| 整体成功 ≠ 成员全有效 | E1/E2 的 `errcode == 0` 仍可能带 `fail_result.shares` | 忽略 `fail_result` → 悄悄漏成员 |
| 三类应用等宽 | 三个子接口都是零端点空标记 | 往子接口复制端点 → SC2 计数打红 |
| `is_public` / `is_corp_calendar` / `set_as_default` | 仅**创建**可传，不可更新；`set_as_default` 第三方不支持 | 更新时传它们期待生效 |
| `partyids` 连写复数 | 本域是 `userids` / `partyids`（部门 `List<long>`），与打卡域 `party_id` 不同 | 跨域共用一个「部门 id 列表」模型 |
| 读取也是 POST | E3 官方即 POST | 改 GET → 打坏 SC1 与 G5 |

SDK 侧通用约束：不自动判错、不做本地必填与容量校验、令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1/E2 仍须查 `fail_result` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |
| 60011 | `NoPrivilege` | 无权限 | 应用可见范围 / 管理员权限不足，不重试 |

其余业务码以官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）为准；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 覆盖式更新的未传字段行为 | 注释只写「覆盖式」+ `skip_public_range`，未说明未传字段被重置还是保留 ⇒ 未逐项核验 | 官方 97716 / 97783 / 97758 |
| `permission` 完整取值集 | 注释只列 `1` / `3`，无 `2` ⇒ 未逐项核验 | 官方创建页参数表 |
| 频率上限 | 本域接口注释无「次/分钟」数值 ⇒ 未逐项核验 | 官方访问频率说明 |
| 全员日历 `public_range` 的必填校验形态 | 注释说「需要指定」，官方是否在缺省时报错未留档 ⇒ 未逐项核验 | 官方 93647 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md)
- 兄弟族：[Schedule.md](./Schedule.md)（管理日程）· [Todo.md](./Todo.md)（待办，仅自建）
- 邻域：[MeetingRoom/Booking.md](../MeetingRoom/Booking.md)（会议室预定与日程的关联）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)（企业作用域切换与令牌归属）
- 官方页：[创建日历 93647](https://developer.work.weixin.qq.com/document/path/93647) · [更新日历 97716](https://developer.work.weixin.qq.com/document/path/97716) · [获取日历详情 97717](https://developer.work.weixin.qq.com/document/path/97717) · [删除日历 97718](https://developer.work.weixin.qq.com/document/path/97718)
