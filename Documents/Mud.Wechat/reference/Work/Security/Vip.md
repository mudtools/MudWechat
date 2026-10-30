# 高级功能账号 · 批量分配、取消与账号列表

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Security` 域 · 高级功能账号族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSecurityVipService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/IWechatWorkSecurityVipService.cs)（父，零端点）＋ [IWechatWorkInternalSecurityVipService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/IWechatWorkSecurityVipService_Internal.cs)（自建子接口，承载全部 5 支） |
| DTO 源文件 | [DataModels/Security/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/RequestModel/)（本族 11 个文件 / 11 型，`SerializerClassName = Security` → `SecurityJsonContext`） |
| 契约守卫 | [WechatSecurityContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSecurityContractGuards.cs)（SEC1 16 路由 / SEC2 父 0 + 自建子 5 / SEC3 令牌绑定 / SEC4 DTO 46 型登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 分配与结果查询 [99503](https://developer.work.weixin.qq.com/document/path/99503)；取消与结果查询 [99505](https://developer.work.weixin.qq.com/document/path/99505)；账号列表 [99506](https://developer.work.weixin.qq.com/document/path/99506) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SEC1 / SEC2 / SEC3 / SEC4 断言 + 5 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

「高级功能」是按成员分配的付费席位。这一族负责席位的分配、取消与盘点，且分配 / 取消都是**异步任务**：

| 端点 | 用途 | 同步性 |
|---|---|---|
| E1 `security/vip/submit_batch_add_job` | 提交批量分配任务 | 异步（立即返回 `jobid`） |
| E2 `security/vip/batch_add_job_result` | 查分配任务结果 | 同步查询 |
| E3 `security/vip/submit_batch_del_job` | 提交批量取消任务 | 异步（立即返回 `jobid`） |
| E4 `security/vip/batch_del_job_result` | 查取消任务结果 | 同步查询 |
| E5 `security/vip/list` | 分页列出已分配且可见的账号 | 同步查询 |

**不处理高级功能的场景**：普通成员的通讯录增删改在 `Contacts` 域；「确认高级功能订单」这类**管理动作**只会出现在 [OperLog.md](./OperLog.md) 的管理端日志里，不是本族端点。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（本族仅自建应用可用） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 应用须配置到「安全与管理 - 可调用接口的应用」中。
- **仅可操作应用可见范围内的企业成员**：范围外的 userid 会进 `invalid_userid_list`，不会让整批失败。
- `userid_list` 单次最多 **100** 个成员；超出官方拒绝，SDK 不本地裁剪。
- 分配与取消都是**异步任务**：E1 / E3 只返回 `jobid`，真正的执行结果要用 E2 / E4 以 `jobid` 查询。
- 分配任务的结果里，**此前已分配过的成员也会出现在成功列表**（幂等语义），不能据此判断「本次新增了几人」。
- `userid` 是可关联到具体员工的标识：示例一律假值，调用日志只记数量。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 / E2 分配与结果 | ✅ | ❌ | ❌ | 99503 |
| E3 / E4 取消与结果 | ✅ | ❌ | ❌ | 99505 |
| E5 账号列表 | ✅ | ❌ | ❌ | 99506 |

> 第三方与服务商代开发官方无对应文档，故不设子接口；父接口 `IsAbstract = true` 且不可注入 DI。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSecurityVipService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 0 | 否 |
| `IWechatWorkInternalSecurityVipService` | `Mud.Wechat.Work` | 自建子接口（承载全部端点） | 5 | 是 |

- 模块注册：`WechatModule.Security` / `AddSecurityApi()`；注册组名 `Security`。
- 端点索引：E1 `SubmitBatchAddVipJobAsync` · E2 `GetBatchAddVipJobResultAsync` · E3 `SubmitBatchDelVipJobAsync` · E4 `GetBatchDelVipJobResultAsync` · E5 `ListVipAccountsAsync`。

---

## 4. 端点

### E1. `SubmitBatchAddVipJobAsync` — 分配高级功能账号

```csharp
[Post("/cgi-bin/security/vip/submit_batch_add_job")]
Task<SubmitBatchAddVipJobResponse> SubmitBatchAddVipJobAsync(
    [Body] SubmitBatchAddVipJobRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/vip/submit_batch_add_job` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`SubmitBatchAddVipJobRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `UseridList` | `userid_list` | `List<string>?` | 是 | 待分配的成员 userid，单次 ≤ 100 个，须在应用可见范围内 |

**响应参数**（`SubmitBatchAddVipJobResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `jobid` | `Jobid` | `string?` | 异步任务 id，用于 E2 查结果 |
| `invalid_userid_list` | `InvalidUseridList` | `List<string>?` | 非法 userid（不在可见范围或无法识别） |

**本端点注意事项**

- **返回值不是执行结果**：`errcode=0` + `jobid` 只表示任务已受理，成员是否真的拿到席位要看 E2。
- `invalid_userid_list` 里的成员被**跳过**，其余成员仍会进入任务——整批不会因个别非法 id 失败。
- `jobid` 必须持久化：任务可能几分钟后才完成，进程重启后仍要能续查。

### E2. `GetBatchAddVipJobResultAsync` — 查询分配高级功能账号结果

```csharp
[Post("/cgi-bin/security/vip/batch_add_job_result")]
Task<GetBatchAddVipJobResultResponse> GetBatchAddVipJobResultAsync(
    [Body] GetBatchAddVipJobRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/vip/batch_add_job_result` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`GetBatchAddVipJobRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Jobid` | `jobid` | `string` | 是 | E1 返回的任务 id（非可空） |

**响应参数**（`GetBatchAddVipJobResultResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `job_result` | `JobResult` | `VipJobResult?` | 任务执行结果 |

`VipJobResult` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `succ_userid_list` | `SuccUseridList` | `List<string>?` | 执行成功的成员（**含此前已分配过的成员**） |
| `fail_userid_list` | `FailUseridList` | `List<string>?` | 执行失败的成员 |

**本端点注意事项**

- **成功列表含「本来就已经有席位」的人**：想统计「本次新增」必须与操作前的 E5 快照做差集。
- 任务未完成时返回什么形态（`job_result` 为空 / 特定 errcode），注释未留档（见 §7）——宿主须按自己的节奏重试，不要假设「查一次就有结果」。
- 只认 `jobid`，不接受 userid 反查。

### E3. `SubmitBatchDelVipJobAsync` — 取消高级功能账号

```csharp
[Post("/cgi-bin/security/vip/submit_batch_del_job")]
Task<SubmitBatchDelVipJobResponse> SubmitBatchDelVipJobAsync(
    [Body] SubmitBatchDelVipJobRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/vip/submit_batch_del_job` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`SubmitBatchDelVipJobRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `UseridList` | `userid_list` | `List<string>?` | 是 | 待撤销的成员 userid，单次 ≤ 100 个，须在应用可见范围内 |

**响应参数**（`SubmitBatchDelVipJobResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `jobid` | `Jobid` | `string?` | 异步任务 id，用于 E4 查结果 |
| `invalid_userid_list` | `InvalidUseridList` | `List<string>?` | 非法 userid（不在可见范围或无法识别） |

**本端点注意事项**

- 与 E1 完全对称：先拿 `jobid`，再用 E4 查结果。
- 取消的是**席位分配**，不是删除成员；成员本身仍在通讯录里。

### E4. `GetBatchDelVipJobResultAsync` — 查询取消高级功能账号结果

```csharp
[Post("/cgi-bin/security/vip/batch_del_job_result")]
Task<GetBatchDelVipJobResultResponse> GetBatchDelVipJobResultAsync(
    [Body] GetBatchDelVipJobRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/vip/batch_del_job_result` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`GetBatchDelVipJobRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Jobid` | `jobid` | `string` | 是 | E3 返回的任务 id（非可空） |

**响应参数**（`GetBatchDelVipJobResultResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `job_result` | `JobResult` | `VipJobResult?` | 任务执行结果（`succ_userid_list` / `fail_userid_list`，结构同 E2） |

**本端点注意事项**

- **分配与取消的 `jobid` 不通用**：E2 只认分配任务 id，E4 只认取消任务 id，传错会查不到。
- `job_result` 的成功列表语义是「已成功移除席位」，与 E2 的「已成功拥有席位」方向相反。

### E5. `ListVipAccountsAsync` — 获取高级功能账号列表

```csharp
[Post("/cgi-bin/security/vip/list")]
Task<ListVipAccountsResponse> ListVipAccountsAsync(
    [Body] ListVipAccountsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/vip/list` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`ListVipAccountsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Cursor` | `cursor` | `string?` | 否 | 分页游标，首次不填 |
| `Limit` | `limit` | `int?` | 否 | 默认 100，最大 200；返回条数不保证等于它 |

**响应参数**（`ListVipAccountsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有更多未获取 |
| `next_cursor` | `NextCursor` | `string?` | 下次请求的 `cursor` |
| `userid_list` | `UseridList` | `List<string>?` | 已分配且可见的成员 userid |

**本端点注意事项**

- 只返回**已分配高级功能且在应用可见范围内**的账号：范围外成员不出现，不能当作企业全量席位清单。
- **必须按 `has_more` 续读**：官方明确不保证每次返回恰好 `limit` 条。
- `limit` 上限是 **200**（不是操作日志的 400，也不是文件记录的 1000）。

**调用示例**（提交分配 → 查结果 → 盘点 → 取消 → 查结果）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Security;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "security-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSecurityApi());

var vip = provider.GetRequiredService<IWechatWorkInternalSecurityVipService>();

// E5：先盘点现有席位（分页；只含可见范围内成员）
var before = new HashSet<string>(StringComparer.Ordinal);
string? cursor = null;
do
{
    var page = await vip.ListVipAccountsAsync(new ListVipAccountsRequest
    {
        Cursor = cursor,
        Limit = 200,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/security/vip/list");
    foreach (string userid in page.UseridList ?? new List<string>())
    {
        before.Add(userid);
    }
    cursor = page.HasMore == true ? page.NextCursor : null;
}
while (cursor is not null);
logger.LogInformation("操作前已有席位 {Count} 个", before.Count);

// E1：提交分配（≤ 100 个 userid；非法 id 进 invalid_userid_list，不会拖垮整批）
var addJob = await vip.SubmitBatchAddVipJobAsync(new SubmitBatchAddVipJobRequest
{
    UseridList = ["staff_0001", "staff_0002"],          // 假值；真实 userid 不得落日志
}, cancellationToken);
WechatWorkException.ThrowIfFailed(addJob, "/cgi-bin/security/vip/submit_batch_add_job");
logger.LogInformation("分配任务已受理，非法 userid {Invalid} 个",
    addJob.InvalidUseridList?.Count ?? 0);

// E2：以 jobid 查结果。异步任务未留档「未完成」形态，生产环境应轮询并持久化 jobid
if (addJob.Jobid is { Length: > 0 } addJobId)
{
    var addResult = await vip.GetBatchAddVipJobResultAsync(
        new GetBatchAddVipJobRequest { Jobid = addJobId }, cancellationToken);
    WechatWorkException.ThrowIfFailed(addResult, "/cgi-bin/security/vip/batch_add_job_result");
    List<string> succeeded = addResult.JobResult?.SuccUseridList ?? new List<string>();
    int newlyAdded = succeeded.Count(id => !before.Contains(id));   // 成功列表含「本来就有」的人
    logger.LogInformation("分配成功 {Ok} 人，其中本次新增 {New}", succeeded.Count, newlyAdded);
}

// E3 / E4：取消同一批席位，再用取消任务的 jobid 查结果（jobid 不跨任务复用）
var delJob = await vip.SubmitBatchDelVipJobAsync(new SubmitBatchDelVipJobRequest
{
    UseridList = ["staff_0001", "staff_0002"],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(delJob, "/cgi-bin/security/vip/submit_batch_del_job");
if (delJob.Jobid is { Length: > 0 } delJobId)
{
    var delResult = await vip.GetBatchDelVipJobResultAsync(
        new GetBatchDelVipJobRequest { Jobid = delJobId }, cancellationToken);
    WechatWorkException.ThrowIfFailed(delResult, "/cgi-bin/security/vip/batch_del_job_result");
    logger.LogInformation("取消成功 {Ok} 人，失败 {Fail} 人",
        delResult.JobResult?.SuccUseridList?.Count ?? 0,
        delResult.JobResult?.FailUseridList?.Count ?? 0);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 提交 ≠ 完成 | E1 / E3 只返回 `jobid`，结果在 E2 / E4 | 拿提交响应当最终结果 → 统计为 0 |
| 成功列表语义 | 分配成功列表**含此前已分配**的成员 | 直接计数 → 高估新增人数 |
| `jobid` 归属 | 分配与取消的任务 id 不通用 | 混用 → 查不到结果 |
| 非法 id | `invalid_userid_list` 只跳过个别人，整批仍执行 | 见非空就重提 → 重复任务 |
| 批量上限 | `userid_list` ≤ 100 | 传 200 → 被拒 |
| 列表上限 | E5 `limit` ≤ 200（别套用 400 / 1000） | 传 1000 → 被拒 |
| 列表范围 | E5 只含应用可见范围内的已分配账号 | 当企业全量席位 → 少算 |
| 列表续读 | 只能看 `has_more`，不能看「条数 < limit」 | 提前终止 → 漏账号 |
| 「取消」的含义 | 撤销席位分配，不是删除成员 | 误以为会删人 |
| 未完成形态 | 任务未完成时 `job_result` 的返回形态未留档 | 假设非空 → 空引用或误判失败 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不预检批量上限；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族注释未留档**专属**业务错误码：无高级功能权益、批量超限、`jobid` 不存在或已过期、可见范围外 userid 等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 任务未完成时的返回形态 | `job_result` 为空、还是特定 errcode / 状态位 ⇒ 未核验 | 官方 99503 / 99505 |
| `jobid` 有效期 | 任务结果可保留多久、过期后查询报什么 ⇒ 未核验 | 官方页 |
| 任务执行时长 | 典型完成时间、最大等待时长 ⇒ 未核验 | 官方页 |
| 重复提交同一批 | 同一 userid 连续提交两个分配任务的语义 ⇒ 未核验 | 官方 99503 |
| 席位容量上限 | 企业可分配席位总数与超限错误码 ⇒ 未核验 | 官方页 |
| E5 的排序 | `userid_list` 是否有序、按什么排序 ⇒ 未核验 | 官方 99506 |
| 与订单的关系 | 分配是否依赖已购订单、未购时返回什么 ⇒ 未核验 | 官方页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md)
- 兄弟族：[OperLog.md](./OperLog.md)（含「确认高级功能订单」的管理端日志）· [Security.md](./Security.md)（文件 / 设备 / 截屏 / 域名 IP）
- 数据模型：[VipJobResult.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/VipJobResult.cs) · `RequestModel/SubmitBatchAddVipJobRequest.cs` / `GetBatchAddVipJobRequest.cs` / `ListVipAccountsRequest.cs`
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[分配高级功能账号 99503](https://developer.work.weixin.qq.com/document/path/99503) · [取消高级功能账号 99505](https://developer.work.weixin.qq.com/document/path/99505) · [获取账号列表 99506](https://developer.work.weixin.qq.com/document/path/99506)
