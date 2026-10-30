# 数据与智能专区 · 应用调用专区程序

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `DataZone` 域 · 应用调用专区程序族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkDataZoneProgramService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneProgramService.cs)（父，承载全部 3 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneProgramService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneProgramService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneProgramService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/DataZone/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/DataZone/)（本族 6 型：3 请求 + 3 应答，与基础接口族共用 `DataZoneJsonContext`） |
| 契约守卫 | [WechatDataZoneContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDataZoneContractGuards.cs)（DZ1c 路由表 / DZ2 层级与 3 + 0/0/0 计数 / DZ3 令牌绑定 / DZ4 JSON 上下文 / DZ5 SDK 面零声明）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 同步调用 99965/99811/100020 · 异步任务与结果 99966/99812/100021 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：DZ1c~DZ4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/README.md)（§6 字符串协议与双层码）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

**应用侧**（专区外）调用**专区内程序**提供的能力——这是「数据不出专区、结果出来」的那条通道：

| 端点 | 语义 |
|---|---|
| E1 `chatdata/sync_call_program` | 同步调用，直接拿到专区程序输出 |
| E2 `chatdata/async_program_task` | 创建异步任务，只拿 `jobid` |
| E3 `chatdata/async_program_result` | 凭 `jobid` 取异步任务结果 |

两支异步端点合起来是一条**上报驱动**的链路：E2 提交 ⇒ 专区程序自行执行 ⇒ 程序经「上报异步任务结果」写入 ⇒ E3 才读得到。所以 E3 不是「任务状态查询」，而是「读上报结果」。

形态与 [Basic 族](../Basic/Basic.md) 那类「全收敛父接口」一致：官方对三类应用开放完全一致的 3 支端点 ⇒ 全落父接口，三支应用类型子接口均为**零端点空标记**（DZ2 断言 `3 + 0/0/0`）。

三条最要紧的：

1. **`request_data` / `response_data` 是 JSON 字符串、不是对象**——SDK 不做序列化编排，格式必须与管理端配置的输入/输出协议匹配。
2. **双层判错**（本域唯一）：顶层 `errcode` 只代表「接口调用成功」；专区程序是否成功要看 `response_errcode`。
3. **本族仍不承载会话存档数据面**：`sync_msg` 一类是专区程序**内部**的 SDK 接口名（无 URL），不是这里的 HTTP 端点（DZ5 零声明断言）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider` / `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（DZ3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

第三方/代开发消费授权企业级令牌，调用前切作用域（`permanent_code` 不得进日志）：

```csharp
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var out1 = await programApi.SyncCallProgramAsync(request, ct);
}
```

### 2.2 权限与业务前置

- 三支端点均要求应用具备「**数据与智能专区**」权限。
- `program_id`（应用关联的专区程序 id）与 `ability_id`（程序关联的能力 id）**官方必填**，且需存在**授权关系**；两者的配置面在专区管理端，本域无查询端点。
- `notify_id` 选填，取值来自「**专区通知应用**」返回。
- `request_data` 必须匹配管理端配置的**输入协议**；`response_data` 必须匹配**输出协议**。注释专门提醒：**专区程序 Java 版 demo 的输入无需在 `request_data` 内再包一层 `input` 对象**（输出同理，Java demo 未在 `response_data` 内包 `output`）。
- 异步链路依赖专区程序**主动上报**：程序未上报前 E3 读不到有效结果——SDK 不做隐式重试或等待。
- 超时与包体上限注释未留档 ⇒ 超时策略由宿主 HTTP 配置掌握（见 §7）。
- 进出专区的业务数据按敏感数据对待：不要把 `request_data` / `response_data` 原文写日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ✅ | ✅ |

三类完全一致 ⇒ 父接口承载全部端点、子接口空标记。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkDataZoneProgramService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalDataZoneProgramService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderDataZoneProgramService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyDataZoneProgramService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

注册组 `DataZone`（与基础接口族共用 `AddDataZoneApi()`），子接口 `InheritedFrom = nameof(WechatWorkDataZoneProgramService)`。

## 4. 端点

三支端点**全部 POST**（含结果查询 E3）。

### E1. `SyncCallProgramAsync` — 应用同步调用专区程序

```csharp
Task<SyncCallDataZoneProgramResponse> SyncCallProgramAsync(
    [Body] SyncCallDataZoneProgramRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/chatdata/sync_call_program` |
| 声明接口 | 父接口 |
| 令牌 | Query `access_token` |

**请求参数**（`SyncCallDataZoneProgramRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `program_id` | `ProgramId` | `string?` | 是 | 应用关联的专区程序 id；需有授权关系 |
| `ability_id` | `AbilityId` | `string?` | 是 | 程序关联的能力 id |
| `notify_id` | `NotifyId` | `string?` | 否 | 来自「专区通知应用」返回 |
| `request_data` | `RequestData` | `string?` | 是（按协议） | **JSON 字符串**，须匹配配置的输入协议；官方示例形如 `"{\"input\":\"xxx\"}"` |

**响应参数**（`SyncCallDataZoneProgramResponse`）：`response_data`（`string?`）——**自定义 JSON 字符串**，须与管理端配置的输出协议匹配。

### E2. `CreateAsyncProgramTaskAsync` — 创建专区程序调用任务

```csharp
Task<CreateDataZoneAsyncProgramTaskResponse> CreateAsyncProgramTaskAsync(
    [Body] CreateDataZoneAsyncProgramTaskRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/async_program_task`。
- 请求：`program_id` / `ability_id` / `request_data`（同 E1；**本端点没有 `notify_id`**）。
- 响应：`jobid`（C# 属性 `Jobid`，`string?`，**全小写连写**）——仅任务受理凭证。

### E3. `GetAsyncProgramResultAsync` — 获取专区程序任务结果

```csharp
Task<GetDataZoneAsyncProgramResultResponse> GetAsyncProgramResultAsync(
    [Body] GetDataZoneAsyncProgramResultRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/chatdata/async_program_result`（查询也走 POST） |

**请求参数**：`jobid`（`GetDataZoneAsyncProgramResultRequest.Jobid`，`string?`，官方必填）。

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `response_errcode` | `ResponseErrcode` | `long?` | **「上报异步任务结果」中上报的 errcode**，代表专区程序返回的错误码 ⇒ 与顶层 `errcode` 语义不同 |
| `response_data` | `ResponseData` | `string?` | 自定义 JSON 字符串，须匹配输出协议 |

**调用示例**（本族主示例：同步调用 + 异步提交与轮询，含双层判错）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.DataZone;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddDataZoneApi());

// 三支端点全部继承自父接口；注入自建子接口
var program = provider.GetRequiredService<IWechatWorkInternalDataZoneProgramService>();

string programId = config["Wechat:ZoneProgramId"]!;   // 专区管理端配置
string abilityId = config["Wechat:ZoneAbilityId"]!;

// ① 同步调用：request_data / response_data 都是 JSON 字符串（勿塞对象）
var sync = await program.SyncCallProgramAsync(
    new SyncCallDataZoneProgramRequest
    {
        ProgramId = programId,
        AbilityId = abilityId,
        RequestData = """{"input":"xxx"}""",   // 须匹配管理端配置的输入协议
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(sync, "/cgi-bin/chatdata/sync_call_program");
// 顶层 errcode 只代表接口成功；专区程序自身的成败看输出协议里的业务字段。
logger.LogInformation("专区程序同步输出（长度 {Length}）", sync.ResponseData?.Length ?? 0);

// ② 异步提交：只拿 jobid
var task = await program.CreateAsyncProgramTaskAsync(
    new CreateDataZoneAsyncProgramTaskRequest
    {
        ProgramId = programId,
        AbilityId = abilityId,
        RequestData = """{"input":"xxx"}""",
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(task, "/cgi-bin/chatdata/async_program_task");

// ③ 取结果：专区程序执行完并经「上报异步任务结果」写入后才有值 ⇒ 必须自行轮询
string? data = null;
for (int attempt = 0; attempt < 60; attempt++)
{
    var result = await program.GetAsyncProgramResultAsync(
        new GetDataZoneAsyncProgramResultRequest { Jobid = task.Jobid! }, cancellationToken);
    WechatWorkException.ThrowIfFailed(result, "/cgi-bin/chatdata/async_program_result");

    if (result.ResponseErrcode is { } programErrcode && programErrcode != 0)
    {
        // 双层判错：这是专区程序上报的码，不是接口层 errcode。
        throw new InvalidOperationException($"专区程序返回 {programErrcode}");
    }
    if (!string.IsNullOrEmpty(result.ResponseData))
    {
        data = result.ResponseData;
        break;
    }
    await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
}
if (data is null) throw new TimeoutException("专区程序结果尚未上报（jobid 已提交）");
```

**本端点注意事项**

- E3 在「程序还没上报」时的返回形态注释未留档 ⇒ 示例按「`response_data` 为空即继续等」处理，接入时以实测为准。
- 轮询节奏、最大次数由宿主决定；**SDK 不做隐式重试**。
- `jobid` 与 `program_id`/`ability_id` 都是配置面产物，不要在代码里造值。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 顶层 `errcode` vs `response_errcode` | 前者=接口调用、后者=专区程序上报的业务码 | 只判 `ThrowIfFailed` → 程序失败被当成功 |
| `request_data` / `response_data` 的类型 | **JSON 字符串**，不是对象/字典 | 直接塞对象 → 序列化不匹配、协议校验失败 |
| Java 版 demo 的包裹层 | 输入无需在 `request_data` 内再包一层 `input`（输出无 `output`） | 照 Python 示例套 → 专区程序解析不到参数 |
| E2 vs E3 | E2 只是受理（`jobid`），结果靠程序**上报** | 以为提交即执行完成 |
| `notify_id` 的归属 | 只在 E1 请求体里；E2 **没有**该字段 | 给异步任务传 notify_id → 编译不过 |
| `jobid` 拼写 | 全小写连写（`jobid`），与异步导入/导出族的 `jobid` 同名但不同链路 | 跨域复用同一个任务表字段语义 |
| 本族 vs 专区程序 SDK 面 | 本族是**应用侧 HTTP**；`sync_msg` 等只有接口名、无 URL，主体是专区内程序 | 把 SDK 接口名当 HTTP 路由加回 → 打红 DZ5 |
| 子接口 | 三支全空标记（DZ2：`3 + 0/0/0`） | 找 `IWechatWorkThirdPartyDataZoneProgramService.XXX` 的差异端点 → 不存在 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` / 字段 | 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| `errcode = 0` | `WechatErrorCodes.Success` | 接口调用成功 | **不代表专区程序成功**，还要看 `response_errcode` |
| `response_errcode` | 专区程序「上报异步任务结果」 | 程序侧业务错误码 | 具体取值由专区程序自定义；本域不解释 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 各端点频率上限 | 注释未留档 QPS 数值 ⇒ 未逐项核验 | 官方 99965/99966 |
| `sync_call_program` 的超时与包体上限 | 注释只给协议匹配要求 ⇒ 未逐项核验；超时由宿主 HTTP 配置 | 官方 99965 |
| E3 在「结果未上报」时的响应形态 | 未留档（`response_errcode`/`response_data` 是否缺省、是否有状态字段） | 官方 99966 |
| 专区程序上报结果的有效期限 | `jobid` 结果可查窗口未留档 | 官方 99966 |
| `notify_id` 的产生与用途细节 | 注释只说「由『专区通知应用』返回」，该通知面不在本域 | 官方专区通知页 |
| `ability_id` 的取值集合 | 配置在专区管理端，本域无查询端点 | 管理端 / 官方文档 |
| 业务错误码 | 未登记码值 | 官方对应页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/DataZone/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/README.md)（§1 专区 SDK 面零声明的三条判据、§6 字符串协议与双层码）
- 同域：[DataZone.md](./DataZone.md)（基础接口：公钥 / 授权成员 / 上传 / 调试模式 / 授权信息）
- 官方页：[应用同步调用专区程序（自建 99965）](https://developer.work.weixin.qq.com/document/path/99965) · [异步调用与结果（99966）](https://developer.work.weixin.qq.com/document/path/99966)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
