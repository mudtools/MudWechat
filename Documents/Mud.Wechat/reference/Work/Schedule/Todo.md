# 待办

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Schedule` 域 · 待办族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkScheduleTodoService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Todo/IWechatWorkScheduleTodoService.cs)（父，**零端点**）· [IWechatWorkScheduleTodoService_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/Todo/IWechatWorkScheduleTodoService_Internal.cs)（承载全部 2 支） |
| DTO 源文件 | [DataModels/Schedule/Todo/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Schedule/Todo/)（5 型；请求型在 `Todo/RequestModel/`，**命名空间仍是 `Mud.Wechat.Work.DataModels.Schedule`**） |
| 契约守卫 | [WechatScheduleContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatScheduleContractGuards.cs)（SC1c 路由表 / SC2c 父 0 + 唯一自建子 2 / SC3 待办方向性断言 / SC5）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取待办详情 [101524](https://developer.work.weixin.qq.com/document/path/101524) · 更新待办状态 [101534](https://developer.work.weixin.qq.com/document/path/101534)（均企业自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SC1c / SC2c / SC3 / SC5 + 2 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md)（2026-10-18）· 官方：本批未取文 |

## 1. 这个接口族做什么

待办的读取（E1）与状态修改（E2）。形态是「零端点父接口 + 唯一自建子接口」：官方只给自建应用开放，**本仓不承载待办的创建与删除**——`todo_id` 由待办自身的创建侧（不在本仓）签发。

**不该用本族的场景**：日程 / 日历的读写用 [Schedule.md](./Schedule.md) 与 [Calendar.md](./Calendar.md)；待办路由在 `/cgi-bin/todo/` 段，与日程的 `/cgi-bin/oa/` 段是两条路径。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（第三方 / 代开发官方无对应 API） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`（**父接口不得声明归属域**，SC3 双向断言） | 错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 调用应用的**可见范围必须包含根部门**；仅允许修改**当前应用创建**的待办；**不允许修改已删除**的待办（E2 remarks）。
- `attendees` ≤**20** 人；元素的 `userid` 与 `status` 在传入时**均官方必填**。
- `content`、创建人、参与人名单属个人或工作数据：示例假值，不落日志 / 遥测 / 异常消息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E2 | ✅ | ❌ | ❌ |

禁止为第三方 / 代开发补子接口：SC2c 断言继承链子接口集合**恰为** `{ IWechatInternalScheduleTodoService }`。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkScheduleTodoService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalScheduleTodoService` | `Mud.Wechat.Work` | 自建子接口（承载全部 2 支） | 2 | 是 |

- 模块注册：`WechatModule.Schedule` / `AddScheduleApi()`；注册组名 `Schedule`；子接口 `InheritedFrom = nameof(WechatWorkScheduleTodoService)`。
- 端点索引：E1 `GetTodoAsync`（读详情） · E2 `UpdateTodoAsync`（改状态 / 参与人）。

---

## 4. 端点

### E1. `GetTodoAsync` — 获取待办详情

```csharp
[Post("/cgi-bin/todo/get")]
Task<GetScheduleTodoResponse> GetTodoAsync(
    [Body] GetScheduleTodoRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/todo/get`（**挂 `/cgi-bin/todo/` 段**，不是 `oa/`） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 仅自建 |

**请求参数**：`todo_id`（`string?`，官方必填）。

**响应参数**（`GetScheduleTodoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `content` | `Content` | `string?` | 待办内容（个人工作数据，勿落日志） |
| `creator` | `Creator` | `string?` | 创建人 userid |
| `status` | `Status` | `int?` | 详情口径：**`0` 已完成 / `1` 进行中** |
| `create_time` / `end_time` | 同名 | `long?` | Unix 秒级 |
| `attendees[]` | `Attendees` | `List<ScheduleTodoAttendee>?` | `userid` + `status` |
| `reminders[]` | `Reminders` | `List<ScheduleTodoReminder>?` | 元素仅 `remind_time`（Unix 秒） |

**本端点注意事项**：`create_time` / `end_time` / `reminders` / `creator` 均为**只读**，E2 不接受它们。

### E2. `UpdateTodoAsync` — 更新待办状态

```csharp
[Post("/cgi-bin/todo/update")]
Task<WechatWorkResponse> UpdateTodoAsync(
    [Body] UpdateScheduleTodoRequest request, CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `TodoId` | `todo_id` | `string?` | 是 | 待办 ID |
| `Status` | `status` | `int?` | 否 | 更新请求口径：`0` 完成 / `1` 进行中 |
| `Attendees` | `attendees` | `List<ScheduleTodoAttendee>?` | 否 | **不传或传空数组 = 不修改参与人列表**；≤20；元素 `userid` + `status` 均必填 |

**响应**：仅 `errcode` / `errmsg`（返回 `WechatWorkResponse`）。

**本端点注意事项**

- 这是本域唯一的「不传即不动」写端点，与日程 E2 的覆盖式更新方向相反。
- 官方请求示例出现 `attendees.status = 2`，而参数表只列 `0` / `1` ⇒ **以参数表为准**，模型的 `int?` 只是兼容载体。

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
    o.AppType = WechatAppType.Internal;        // 官方仅自建：无第三方 / 代开发子接口
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddScheduleApi());

var todos = provider.GetRequiredService<IWechatWorkInternalScheduleTodoService>();
string todoId = "TODO示例";                     // todo_id 由本域之外的创建侧签发

var detail = await todos.GetTodoAsync(new GetScheduleTodoRequest { TodoId = todoId }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/todo/get");

// 详情口径：0 已完成 / 1 进行中；content 属个人工作数据，不落日志
logger.LogInformation("待办状态 {Status}，参与人 {Count} 人", detail.Status, detail.Attendees?.Count ?? 0);

// 只改状态：attendees 不传 = 不动参与人列表
var updateStatus = await todos.UpdateTodoAsync(new UpdateScheduleTodoRequest
{
    TodoId = todoId,
    Status = 0,                                  // 更新请求口径：0 完成 / 1 进行中
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updateStatus, "/cgi-bin/todo/update");

// 改参与人：≤20，元素 userid + status 均必填
var updateAttendees = await todos.UpdateTodoAsync(new UpdateScheduleTodoRequest
{
    TodoId = todoId,
    Attendees = new List<ScheduleTodoAttendee>
    {
        new ScheduleTodoAttendee { Userid = "zhangsan", Status = 0 },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updateAttendees, "/cgi-bin/todo/update");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 路由段 | 待办是 `/cgi-bin/todo/*`，日程 / 日历是 `/cgi-bin/oa/*` | 按 `oa/todo` 拼路径 → 404 |
| `status` 两套中文口径 | 详情「0 已完成 / 1 进行中」；更新请求「0 完成 / 1 进行中」 | 混用注释写业务判定 |
| 增量语义 | `attendees` 不传 / 空数组 = 不动；不同于日程 E2 覆盖式 | 想保留参与人却传了 `[]`（能保留）与想清空却以为 `[]` 能清空（不能） |
| 官方示例 `status = 2` | 与参数表 0/1 冲突，以参数表为准 | 按 2 做分支 |
| 仅自建 | 第三方 / 代开发无子接口 | 注入不存在的接口 |
| 只读字段 | `creator` / `create_time` / `end_time` / `reminders` 不可写 | 在 E2 传它们期待生效 |

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈 |
| 60011 | `NoPrivilege` | 无权限 | 可见范围不含根部门 / 非本应用创建 / 已删除 |

本域接口注释未留档专属业务码 ⇒ 其余以官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）为准。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `todo_id` 的签发面 | 本域只有读 + 改，没有创建 / 删除端点 ⇒ 官方是否存在 add/del 未在本域核验 | 官方待办文档树 |
| 官方示例 `attendees.status = 2` 的语义 | 与参数表冲突 ⇒ 未核验 | 官方 101534 |
| 待办两页的章节归属 | 本仓把待办挂在 `Schedule` 模块，路由段却是 `todo/` ⇒ 官方章节归属未逐项核验 | 官方文档树 |
| 频率上限 | 注释无数值 ⇒ 未核验 | 官方访问频率说明 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Schedule/README.md) §11
- 兄弟族：[Calendar.md](./Calendar.md) · [Schedule.md](./Schedule.md)
- 同形态参照（零端点父 + 仅自建子）：[Emergency.md](../Emergency/Emergency.md) · [Journal.md](../Journal/Journal.md) · [Dial.md](../Dial/Dial.md)
- 官方页：[获取待办详情 101524](https://developer.work.weixin.qq.com/document/path/101524) · [更新待办状态 101534](https://developer.work.weixin.qq.com/document/path/101534)
