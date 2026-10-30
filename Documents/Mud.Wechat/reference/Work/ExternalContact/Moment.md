# 客户联系 · 朋友圈

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 朋友圈族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactMomentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Moment/IWechatWorkExternalContactMomentService.cs)（父，承载全部 14 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Moment/IWechatWorkExternalContactMomentService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Moment/IWechatWorkExternalContactMomentService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Moment/IWechatWorkExternalContactMomentService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/Moment/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/Moment/)（含 `RequestModel/`；`SerializerClassName = Moment`） |
| 契约守卫 | [WechatExternalContactMomentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactMomentContractGuards.cs)（MO1 路由表 / MO2 层级与三空标记 / MO3 令牌绑定 / MO4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 发表任务 95094/95095/96351 · 停止发表 97612/97616/97615 · 发表记录与互动 93333/93443/96352 · 朋友圈规则组 94890/99541/99545 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：MO1~MO4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.3、§4 陷阱 3/7/10）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

企业发表到客户**朋友圈**的全链路：

| 能力块 | 端点 |
|---|---|
| 发表任务 | E1 创建任务（异步）· E2 取任务创建结果 · E3 停止发表 |
| 发表记录与互动数据 | E4 企业全部发表列表 · E5 企业发表的成员执行列表 · E6 发表时选择的可见范围 · E7 发表后实际可见客户 · E8 评论与点赞 |
| 朋友圈规则组 | E9 列表 · E10 详情 · E11 管理范围 · E12 创建 · E13 编辑 · E14 删除 |

**E1 是异步的**：不直接返回朋友圈 id，只返回 `jobid`（24 小时有效）；**必须用本域唯一的 GET（E2）轮询**，`status = 3` 后 `result.moment_id` 才拿到，之后 E5–E8 都以 `moment_id` 为主键。

形态：官方对三类应用开放完全一致的 14 支端点 ⇒ 全收敛父接口，三支子接口空标记（MO2）。除 E2 外**全部 POST**。

⚠️ 名字最容易读错的两处：`get_moment_task`（E5，方法名 `GetMomentPublishListAsync`，取的是**企业发表列表的成员执行情况**）与 `get_moment_task_result`（E2，取**任务创建结果**）**不是同义的一对**——一个是执行侧、一个是建任务侧。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（MO3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

### 2.2 权限与业务前置

**发表侧（E1–E3）**：

- 频率：**企业每月通过 API 创建朋友圈 10 万次、每分钟最多 10 次**。
- `text` 与 `attachments` **不能同时为空**；文本**最多 2000 字 / 4000 字节**，超长报 `invalid text size`。
- 附件**三选一且不可混用**（混用报 `invalid attachments msgtype`）：最多 **9 个图片**、或 **1 个视频**（≤30 秒、≤10MB）、或 **1 个链接**。
- `visible_range` 不填 = 执行者为应用可见范围内全部成员；`sender_list.user_list` **最多 10 万**；`external_contact_list.tag_list` **仅支持企业客户标签，不支持规则组标签**。
- E3 **无法撤回已经发表到客户朋友圈的信息**。

**记录侧（E4–E8）**：

- E4 起止时间必填、**间隔不超过 30 天**；**`limit` 最大 20、默认 20**（本族最小的分页上限）；**已删除的朋友圈不经 API 返回**。
- E5 **仅支持企业发表的朋友圈 id**；`limit` ≤1000、默认 500。
- E6 `limit` ≤1000、默认 500；E7 `limit` **≤5000、默认 3000**。
- E8 无分页参数，一次返回 `comment_list` 与 `like_list`。

**规则组（E9–E14）**：

- **仅支持串行调用**（勿并发创建/修改）；`admin_list` **不可包含超级管理员**、每组**最多 20 个负责人**；`range` 官方必填；管理组**最大层级 5**、每组管理范围内**最多 3000 个节点**。
- 有父规则组时：管理范围必须是父组子集，且**完全继承父组权限配置**（传入的 `privilege` 被忽略）。
- **应用仅能获取与管理由本应用创建的规则组**。
- E9/E11 `limit` 默认与上限均 **1000**。

数据口径：朋友圈内容与评论/点赞含客户 `external_userid`、成员 `userid` ⇒ 个人数据，示例用假值、不落日志；`jobid` / `moment_id` 是标识而非凭据，也别打进日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E14 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactMomentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **14** | 否 |
| `IWechatWorkInternalExternalContactMomentService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactMomentService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactMomentService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactMomentService)`。

## 4. 端点

### E1. `AddMomentTaskAsync` — 创建发表朋友圈任务

```csharp
Task<AddMomentTaskResponse> AddMomentTaskAsync(
    [Body] AddMomentTaskRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/add_moment_task` |
| 令牌 | Query `access_token` |

**请求参数**（`AddMomentTaskRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `visible_range` | `VisibleRange` | `MomentVisibleRange?` | 否 | 不填 = 应用可见范围内全部成员为执行者 |
| `visible_range.sender_list.user_list` | `SenderList.UserList` | `List<string>?` | 否 | 执行者成员，**最多 10 万** |
| `visible_range.sender_list.department_list` | `SenderList.DepartmentList` | `List<long>?` | 否 | 执行者部门 |
| `visible_range.external_contact_list.tag_list` | `ExternalContactList.TagList` | `List<string>?` | 否 | **仅企业客户标签，不支持规则组标签** |
| `text` | `Text` | `MomentText?` | 条件 | ≤2000 字 / 4000 字节；**与 attachments 不能同时为空** |
| `attachments` | `Attachments` | `List<MomentAttachment>?` | 条件 | 9 图 / 1 视频 / 1 链接，**三选一** |

附件形态（按 `msgtype`）：`image.media_id`（必填）、`link.title`（≤64 字 / 128 字节）+ `link.url`（必填）+ `link.media_id`（封面，必填）、`video.media_id`（必填，≤30 秒、≤10MB，不填报 `invalid msg`）。

**响应参数**：`jobid`（`string?`，最大 64 字节、**24 小时有效**）。

**调用示例**（本族主示例：建任务 → 轮询取 moment_id → 看成员执行情况）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.Moment;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var moments = provider.GetRequiredService<IWechatWorkInternalExternalContactMomentService>();

// ① 创建发表任务（每月 10 万次、每分钟 10 次；文本与附件不可同时为空）
var task = await moments.AddMomentTaskAsync(
    new AddMomentTaskRequest
    {
        VisibleRange = new MomentVisibleRange
        {
            SenderList = new MomentSenderList { DepartmentList = new List<long> { 2 } },
            ExternalContactList = new MomentExternalContactList { TagList = new List<string> { "<企业标签id>" } },
        },
        Text = new MomentText { Content = "本周新品上架" },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(task, "/cgi-bin/externalcontact/add_moment_task");
string jobid = task.JobId ?? throw new InvalidOperationException("官方未返回 jobid");

// ② 轮询任务创建结果：本域唯一 GET，status=3 后 result 才有效
GetMomentTaskResultResponse progress;
while (true)
{
    progress = await moments.GetMomentTaskResultAsync(jobid, cancellationToken);
    WechatWorkException.ThrowIfFailed(progress, "/cgi-bin/externalcontact/get_moment_task_result");
    if (progress.Status == 3) break;   // 1 开始 / 2 正在创建 / 3 创建完成
    await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
}

// 双层判错：result.errcode 是任务执行结果码，与顶层 errcode 不同层
if (progress.Result?.ErrorCode is int taskErrcode && taskErrcode != 0)
    throw new InvalidOperationException($"朋友圈任务执行失败，errcode={taskErrcode}");

string momentId = progress.Result?.MomentId ?? throw new InvalidOperationException("任务完成但未返回 moment_id");
// 不合法的执行者 / 客户（不存在的 id 或不在可见范围内）在 result 的两个列表里
logger.LogInformation("朋友圈已创建 moment_id 长度 {Len}，非法执行者 {SenderCount} 个",
    momentId.Length, progress.Result?.InvalidSenderList is null ? 0 : 1);

// ③ 成员执行情况（仅支持企业发表的朋友圈 id；limit ≤1000、默认 500）
string? cursor = null;
do
{
    var exec = await moments.GetMomentPublishListAsync(
        new GetMomentPublishListRequest { MomentId = momentId, Cursor = cursor, Limit = 1000 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(exec, "/cgi-bin/externalcontact/get_moment_task");

    foreach (MomentPublishTaskItem item in exec.TaskList ?? new List<MomentPublishTaskItem>())
    {
        // publish_status：0 未发表 / 1 已发表
        logger.LogInformation("成员发表状态 {Status}", item.PublishStatus);
    }
    cursor = exec.NextCursor;
} while (!string.IsNullOrEmpty(cursor));
```

### E2. `GetMomentTaskResultAsync` — 获取任务创建结果

```csharp
Task<GetMomentTaskResultResponse> GetMomentTaskResultAsync(
    [Query("jobid")] string jobid, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/externalcontact/get_moment_task_result`（**本域唯一的 GET**） |
| 请求 | Query `jobid`（≤64 字节，E1 返回，24 小时有效） |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **1 开始创建 / 2 正在创建中 / 3 创建任务已完成** |
| `type` | `Type` | `string?` | 固定为 `add_moment_task` |
| `result` | `Result` | `MomentTaskResult?` | **仅 `status = 3` 后有效** |
| `result.errcode` / `result.errmsg` | `ErrorCode` / `ErrorMessage` | `int?` / `string?` | **任务执行结果码**（与顶层 `errcode` 分属两层） |
| `result.moment_id` | `MomentId` | `string?` | 朋友圈 id，E5–E8 的主键 |
| `result.invalid_sender_list` | `InvalidSenderList` | `MomentSenderList?` | 不合法的执行者（不存在或不在可见范围的成员/部门） |
| `result.invalid_external_contact_list` | `InvalidExternalContactList` | `MomentExternalContactList?` | 不合法的外部联系人（含不合法的客户标签列表） |

- **只能查询已经提交过的历史任务**。

### E3. `CancelMomentTaskAsync` — 停止发表企业朋友圈

```csharp
Task<WechatWorkResponse> CancelMomentTaskAsync(
    [Body] CancelMomentTaskRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/cancel_moment_task`；请求 `moment_id`（必填）。
- 只停「尚未发送」的发表任务；**已发到客户朋友圈的内容无法撤回**。

### E4. `GetMomentListAsync` — 获取企业全部的发表列表

```csharp
Task<GetMomentListResponse> GetMomentListAsync(
    [Body] GetMomentListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `start_time` / `end_time` | `StartTime` / `EndTime` | `long?` | **是** | 间隔 **≤30 天**，仅取区间内数据 |
| `creator` | `Creator` | `string?` | 否 | 创建人；不填 = 全部创建人 |
| `filter_type` | `FilterType` | `int?` | 否 | `0` 企业发表 / `1` 个人发表 / `2` 所有（**默认 2**） |
| `cursor` | `Cursor` | `string?` | 否 | 首次不填 |
| `limit` | `Limit` | `int?` | 否 | **最大 20、默认 20** |

**响应 `moment_list[]`** → `MomentRecord`：`moment_id` / `creator`（**企业创建的朋友圈不返回**）/ `create_time` / `create_type`（0 企业 / 1 个人）/ `visible_type`（**0 部分可见 / 1 公开**）/ `text` / `image[]` / `video`（含 `thumb_media_id`）/ `link` / `location`（`latitude` / `longitude` / `name`）。**已删除的朋友圈不返回**。

### E5. `GetMomentPublishListAsync` — 企业发表列表的成员执行情况

```csharp
Task<GetMomentPublishListResponse> GetMomentPublishListAsync(
    [Body] GetMomentPublishListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_moment_task`（**方法与路由不同名**：`get_moment_task` = 发表列表执行情况）。
- 请求 `moment_id`（必填，**仅支持企业发表的朋友圈 id**）+ `cursor` / `limit`（≤1000、默认 500）。
- 响应 `task_list[]`：`userid`、`publish_status`（**0 未发表 / 1 已发表**）；`next_cursor` 无更多时不返回。

### E6. `GetMomentCustomerListAsync` — 发表时选择的可见范围

```csharp
Task<GetMomentCustomerListResponse> GetMomentCustomerListAsync(
    [Body] GetMomentCustomerListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_moment_customer_list`；请求 `moment_id` + `userid`（**均必填**）+ `cursor` / `limit`（≤1000、默认 500）。
- `userid` 的取法：企业发表的朋友圈用 E5 拿成员列表；**个人发表的朋友圈其创建人 userid 即发表者**。
- 响应 `customer_list[]`：`userid`（发表成员）、`external_userid`（可见客户）。
- 语义是「**发表时选中的可见范围**」，不等于实际看到的人（实际看没看要用 E7）。

### E7. `GetMomentSendResultAsync` — 发表后的实际可见客户

```csharp
Task<GetMomentSendResultResponse> GetMomentSendResultAsync(
    [Body] GetMomentSendResultRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_moment_send_result`；请求 `moment_id` + `userid`（必填）+ `cursor` / `limit`（**≤5000、默认 3000**）。
- 响应 `customer_list[]`：`external_userid`（发送成功的客户）。

### E8. `GetMomentCommentsAsync` — 朋友圈互动数据

```csharp
Task<GetMomentCommentsResponse> GetMomentCommentsAsync(
    [Body] GetMomentCommentsRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_moment_comments`；请求 `moment_id` + `userid`（均必填）；**无分页参数**。
- 响应 `comment_list[]` / `like_list[]`（元素同为 `MomentCommentItem`）：`external_userid`（**仅客户互动时返回**）、`userid`（**仅企业成员互动时返回**）、`create_time`。
- **两者不会同时出现**于同一元素——按互动主体类型二选一读取。

### E9–E11. 朋友圈规则组读取三支

```csharp
Task<GetMomentStrategyListResponse> GetMomentStrategyListAsync([Body] GetMomentStrategyListRequest request, CancellationToken cancellationToken = default);
Task<GetMomentStrategyDetailResponse> GetMomentStrategyDetailAsync([Body] GetMomentStrategyDetailRequest request, CancellationToken cancellationToken = default);
Task<GetMomentStrategyRangeResponse> GetMomentStrategyRangeAsync([Body] GetMomentStrategyRangeRequest request, CancellationToken cancellationToken = default);
```

| 端点 | 路由 | 请求 | 响应 |
|---|---|---|---|
| E9 列表 | `POST /cgi-bin/externalcontact/moment_strategy/list` | `cursor?` / `limit?`（默认与上限 1000） | `strategy[]`（元素仅 `strategy_id`）+ `next_cursor` |
| E10 详情 | `POST /cgi-bin/externalcontact/moment_strategy/get` | `strategy_id`（必填） | `strategy`（`MomentStrategy`） |
| E11 管理范围 | `POST /cgi-bin/externalcontact/moment_strategy/get_range` | `strategy_id` + `cursor?` / `limit?`（默认与上限 1000） | `range[]` + `next_cursor` |

`MomentStrategy`：`strategy_id` / `parent_id`（无父组为 `0`）/ `strategy_name` / `create_time` / `admin_list[]` / `privilege`。
`privilege`（三项，**默认均 `true`**）：`view_moment_list`（查看成员全部朋友圈发表）、`send_moment`（允许发表）、`manage_moment_cover_and_sign`（管理封面与签名）。
范围节点 `MomentStrategyRangeNode`：`type`（**1 成员 / 2 部门**）、`userid`（仅 type=1）、`partyid`（仅 type=2，`long?`）。

### E12. `CreateMomentStrategyAsync` — 创建朋友圈规则组

```csharp
Task<CreateMomentStrategyResponse> CreateMomentStrategyAsync(
    [Body] CreateMomentStrategyRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/moment_strategy/create`；请求 `strategy_name`（必填）、`admin_list`（**必填，不含超级管理员，≤20**）、`range`（**必填**）、`parent_id?`、`privilege?`（**有父组时被忽略**）。
- 响应 `strategy_id`（`long?`）。
- **危险操作**：仅串行调用；层级 ≤5；每组管理范围 ≤3000 节点。

### E13. `UpdateMomentStrategyAsync` — 编辑规则组及其管理范围

```csharp
Task<WechatWorkResponse> UpdateMomentStrategyAsync(
    [Body] UpdateMomentStrategyRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/moment_strategy/edit`；`strategy_id` 必填。
- **不填则不修改，有值则整体覆盖**（`admin_list` / `privilege`）；管理范围走 `range_add` / `range_del` 两个**增删列表**。
- 编辑含父组的规则组时，管理范围须为父组子集并完全继承其权限（`privilege` 被忽略）。

### E14. `DeleteMomentStrategyAsync` — 删除朋友圈规则组

```csharp
Task<WechatWorkResponse> DeleteMomentStrategyAsync(
    [Body] DeleteMomentStrategyRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/moment_strategy/del`；请求 `strategy_id`（必填）；仅能删本应用创建的规则组。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `get_moment_task`（E5）vs `get_moment_task_result`（E2） | 前者是「企业发表列表的成员执行情况（POST）」，后者是「建任务的异步结果（GET）」；**方法名 `GetMomentPublishListAsync` 与路由不同名** | 以为是一对 → 拿 jobid 调 E5 或拿 moment_id 调 E2 |
| 顶层 `errcode` vs `result.errcode` | 后者是**任务执行结果**码（E2） | 只判接口层 → 任务实际失败被当成功 |
| `jobid`（24h）vs `moment_id` | jobid 只用于 E2；moment_id 才是 E5–E8 的主键 | 用 jobid 查执行情况 → 参数错误 |
| `status` 1/2/3 vs `result` 有效性 | **只有 3 之后 `result` 才有值** | 提前读 moment_id → 空引用 |
| E6（发表时选择的可见范围）vs E7（发表后实际可见客户） | 选择 ≠ 送达 | 用 E6 算触达率 |
| E4 `limit` ≤**20** vs E5/E6 ≤1000 vs E7 ≤5000 | 四个上限并存 | 用 1000 打 E4 → 被截到 20 或报错 |
| `create_type` vs `visible_type` | 前者 0 企业/1 个人；后者 0 部分可见/1 公开 | 拿 create_type 判「是否公开」 |
| `tag_list` 的标签面 | 朋友圈可见范围**只支持企业客户标签，不支持规则组标签**（[Tag.md](./Tag.md)） | 传规则组标签 → 进 `invalid_external_contact_list` |
| 附件三选一 | 9 图 / 1 视频 / 1 链接，混用报 `invalid attachments msgtype` | 图 + 链接一起发 → 失败 |
| 已删除的朋友圈 | E4 不返回；已发表的无法撤回（E3 只停未发送部分） | 以为 cancel 能删内容或列表能查到历史 |
| `creator` | **企业创建的朋友圈不返回该字段** | 按创建人聚合时全为空 |
| 规则组的超级管理员 | `admin_list` **不可含超级管理员** | 提交后失败或权限异常 |
| 规则组的 `privilege` 继承 | 有父组时被忽略、完全继承父组 | 以为能给子组单独关权限 |
| 动词 | 仅 E2 是 GET，其余 13 支 POST（含 E4/E5/E9 等读取） | 「读取即 GET」改写 → 打坏 MO1 与 G5 |

SDK 侧通用约束：**不自动判错**、**不轮询 E2**、**不做并发串行保护**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 只代表任务受理；真实结果看 E2 的 `result.errcode` |
| `invalid text size` / `invalid attachments msgtype` / `invalid msg` / `invalid attachments size` | 官方**文字**错误口径（DTO 注释留档） | 文本超长、附件类型混用、视频缺 media_id、图片超 9 个 | 以 `errmsg` 文本出现，非独立码值 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

数值型业务错误码（每月/每分钟超限、moment_id 非法、非本应用创建的规则组）**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 数值型错误码全集 | 注释只留了 `invalid text size` 等文字口径 ⇒ 未逐项核验 | 官方各端点页 |
| E2 轮询建议间隔与任务留存时长 | 注释给「24 小时有效、只能查历史任务」，未给轮询节奏 | 官方 95094 |
| `invalid_sender_list` / `invalid_external_contact_list` 的具体字段 | 结构复用 `MomentSenderList` / `MomentExternalContactList`，其字段集未逐项核验 | 官方 95094 + DTO 注释 |
| 图片/视频的素材规格 | 注释给「≤30 秒、≤10MB」与「尺寸约束同图片附件」，未给图片的确切尺寸与格式 | 官方 95094 |
| 朋友圈内容尺寸/条数的其它限制 | 每月 10 万次与每分钟 10 次之外，未给企业侧总条数 | 官方 95094 |
| 规则组数量上限与命名约束 | 注释给管理员/节点/层级上限，未给规则组总数与名称长度 | 官方 94890 |
| `location` 的写入侧 | 响应含 `location`，但请求侧未见对应字段 ⇒ 定位是否可经 API 发表**未核验** | 官方 95094 |
| 评论/点赞的排序与全量口径 | E8 无分页参数，是否截断未说明 | 官方 93333 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.3 端点表、§4 陷阱 7「朋友圈异步任务：两个 `get_moment_task*` 的区别」、陷阱 3 分页口径、陷阱 10 读取即 POST）
- 同域：[Tag.md](./Tag.md)（`tag_list` 只能用企业客户标签）· [Customer.md](./Customer.md)（规则组两面：客户联系规则组 vs 朋友圈规则组）· [GroupMsg.md](./GroupMsg.md)（另一套「任务 + 成员执行 + 送达结果」三段式，形态高度相似）
- 官方页：[创建发表朋友圈任务（自建 95094）](https://developer.work.weixin.qq.com/document/path/95094) · [获取发表列表（93333）](https://developer.work.weixin.qq.com/document/path/93333) · [朋友圈规则组（94890）](https://developer.work.weixin.qq.com/document/path/94890)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
