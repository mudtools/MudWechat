# 高级功能账号 · 会议 VIP 账号的异步批量分配与撤销

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 高级功能账号管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingPremiumAccountService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingPremiumAccountService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingPremiumAccountService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingPremiumAccountService_Internal.cs)（**唯一**子接口，承载全部 5 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`AssignPremiumAccountsRequest/Response`、`GetAssignPremiumAccountsResultRequest/Response`、`RevokePremiumAccountsRequest/Response`、`GetRevokePremiumAccountsResultRequest/Response`、`ListPremiumAccountsRequest/Response`、`PremiumAccountJobResult`；本族在 MT4 的逐族分解中计 11 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(5)`、全部挂 `vip/` 段且官方即 POST / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 5)` / MT3 令牌绑定 / MT4 上下文登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 分配 [99508](https://developer.work.weixin.qq.com/document/path/99508)（**同一页含提交任务与查结果两支**）· 撤销 99509（同上）· 获取列表 99510（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（提交分配任务 → 轮询取结果 → 翻页列账号 → 提交撤销并取结果））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 8/9 节与缺口表「高级账号任务终态与留存」条） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

给企业成员**分配 / 撤销「会议高级功能账号」**，并查询现有清单，5 支端点全部挂 `/cgi-bin/meeting/vip/`：

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `AssignPremiumAccountsAsync` | `vip/submit_batch_add_job` | **提交**批量分配任务，回 `jobid` |
| E2 | `GetAssignPremiumAccountsResultAsync` | `vip/batch_add_job_result` | 按 `jobid` **取结果** |
| E3 | `RevokePremiumAccountsAsync` | `vip/submit_batch_del_job` | 提交批量撤销任务，回 `jobid` |
| E4 | `GetRevokePremiumAccountsResultAsync` | `vip/batch_del_job_result` | 按 `jobid` 取结果 |
| E5 | `ListPremiumAccountsAsync` | `vip/list` | 分页列出符合条件的高级功能账号 `userid` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）；权限口径明写「**代开发应用、第三方应用暂不支持**」。

**与 Mail 域的高级功能账号族形态不同**：邮件侧（[Vip.md](../Mail/Vip.md)）是 `exmail/vip/batch_add` **同步批量**、当场返回成败两列表；本族是 **`submit_*_job` + `*_job_result` 两段式异步任务** ⇒ 不要把两侧的调用编排互相套用。

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

- 自建应用须配置到「**协作 - 会议 - 可调用接口的应用**」中（五支注释逐条重复该口径）。
- **E1 / E3 的 `userid_list` 单次操作最多 100 个成员**；非法成员（不在应用可见范围、无法识别）落 `invalid_userid_list`，**不会**进入任务。
- **E5 的 `limit` 默认 100、最大 200**，且官方明确「**不保证每次返回的数据刚好为指定 `limit`，必须用返回的 `has_more` 判断是否继续请求**」。
- **异步任务链路必须落库 `jobid`**：注释只说明「`jobid` 可用于查询分配/取消结果」，**未给任务有效期与轮询间隔** ⇒ 超窗后无源可查的风险由宿主承担（见 §7）。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E5 | ✅ | ❌（暂不支持） | ❌（暂不支持） |

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
| 可注入接口 | `IWechatWorkInternalMeetingPremiumAccountService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingPremiumAccountService`） |
| 不可注入 | `IWechatWorkMeetingPremiumAccountService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `AssignPremiumAccountsAsync` — 批量分配高级功能账号（提交任务）

**签名**

```csharp
[Post("/cgi-bin/meeting/vip/submit_batch_add_job")]
Task<AssignPremiumAccountsResponse> AssignPremiumAccountsAsync(
    [Body] AssignPremiumAccountsRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/vip/submit_batch_add_job` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/vip/submit_batch_add_job?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `AssignPremiumAccountsRequest`：`userid_list`（官方必填，**单次 ≤100**） |

**返回** `AssignPremiumAccountsResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `jobid` | `Jobid` | `string?` | 批量分配任务 ID（交 E2 查询结果） |
| `invalid_userid_list` | `InvalidUseridList` | `List<string>?` | **非法 userid**：不在应用可见范围的 userid 以及无法识别的 userid |

**调用示例**（提交分配 → 轮询取结果 → 翻页列清单 → 提交撤销 → 取结果；`jobid` 必须落库）

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
var vips = provider.GetRequiredService<IWechatWorkInternalMeetingPremiumAccountService>();

// ④ E1 提交分配任务：单次 ≤100，本仓不裁批（宿主自己 Take(100)）
var candidates = new List<string> { "demo_user_0001", "demo_user_0002" };   // 假值
var submitted = await vips.AssignPremiumAccountsAsync(new AssignPremiumAccountsRequest
{
    UseridList = candidates.Take(100).ToList(),
}, cancellationToken);
WechatWorkException.ThrowIfFailed(submitted, "/cgi-bin/meeting/vip/submit_batch_add_job");
string? jobid = submitted.Jobid;
// invalid_userid_list 里的是「不可见 / 无法识别」成员：它们没进任务，需要另行修数据
logger.LogInformation("任务已提交（非法成员 {InvalidCount} 个未入任务）", submitted.InvalidUseridList?.Count ?? 0);

// ⑤ E2 取结果：jobid 是任务句柄；有效期注释未给 ⇒ 宿主须落库并尽快取回
PremiumAccountJobResult? result = null;
if (jobid is not null)
{
    for (var attempt = 0; attempt < 30; attempt++)
    {
        var got = await vips.GetAssignPremiumAccountsResultAsync(
            new GetAssignPremiumAccountsResultRequest { Jobid = jobid }, cancellationToken);
        WechatWorkException.ThrowIfFailed(got, "/cgi-bin/meeting/vip/batch_add_job_result");
        result = got.JobResult;
        // 注释未提供「执行中」状态位：拿到 job_result 即为终态，否则按宿主策略重试
        if (result is not null) break;
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
// 分配场景下 succ_userid_list 包括「本来就是高级功能账号」的 userid ⇒ 幂等重跑不算失败
logger.LogInformation("分配成功 {Succ} 个、失败 {Fail} 个",
    result?.SuccUseridList?.Count ?? 0, result?.FailUseridList?.Count ?? 0);

// ⑥ E5 翻页列清单：limit 默认 100、最大 200；判停只看 has_more
string? cursor = null;
var accounts = new List<string>();
while (true)
{
    var listed = await vips.ListPremiumAccountsAsync(new ListPremiumAccountsRequest
    {
        Cursor = cursor,
        Limit = 200,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(listed, "/cgi-bin/meeting/vip/list");
    accounts.AddRange(listed.UseridList ?? new List<string>());
    if (listed.HasMore != true) break;
    cursor = listed.NextCursor;
}

// ⑦ E3 + E4 撤销链路：与分配同构（submit → *_job_result）
var revoked = await vips.RevokePremiumAccountsAsync(new RevokePremiumAccountsRequest
{
    UseridList = accounts.Take(100).ToList(),
}, cancellationToken);
WechatWorkException.ThrowIfFailed(revoked, "/cgi-bin/meeting/vip/submit_batch_del_job");
if (revoked.Jobid is not null)
{
    var revokeResult = await vips.GetRevokePremiumAccountsResultAsync(
        new GetRevokePremiumAccountsResultRequest { Jobid = revoked.Jobid }, cancellationToken);
    WechatWorkException.ThrowIfFailed(revokeResult, "/cgi-bin/meeting/vip/batch_del_job_result");
}
```

### E2. `GetAssignPremiumAccountsResultAsync` — 查询分配任务结果

**签名**

```csharp
[Post("/cgi-bin/meeting/vip/batch_add_job_result")]
Task<GetAssignPremiumAccountsResultResponse> GetAssignPremiumAccountsResultAsync(
    [Body] GetAssignPremiumAccountsResultRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`jobid`（`string?`，官方必填）。

**返回** `GetAssignPremiumAccountsResultResponse.JobResult`（`PremiumAccountJobResult?`，JSON `job_result`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `succ_userid_list` | `SuccUseridList` | `List<string>?` | 操作成功；**分配场景下包括已经是高级功能账号的 userid** |
| `fail_userid_list` | `FailUseridList` | `List<string>?` | 操作失败 |

### E3. `RevokePremiumAccountsAsync` — 批量撤销（提交任务）

**签名**

```csharp
[Post("/cgi-bin/meeting/vip/submit_batch_del_job")]
Task<RevokePremiumAccountsResponse> RevokePremiumAccountsAsync(
    [Body] RevokePremiumAccountsRequest request,
    CancellationToken cancellationToken = default);
```

与 E1 **同构**：请求 `userid_list`（官方必填，**单次 ≤100**）；返回 `jobid` + `invalid_userid_list`。撤销场景下 `succ_userid_list` **无**「本来就是高级账号」那条注记。

### E4. `GetRevokePremiumAccountsResultAsync` — 查询撤销任务结果

**签名**

```csharp
[Post("/cgi-bin/meeting/vip/batch_del_job_result")]
Task<GetRevokePremiumAccountsResultResponse> GetRevokePremiumAccountsResultAsync(
    [Body] GetRevokePremiumAccountsResultRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`jobid`（官方必填）。**返回**：`job_result`（`PremiumAccountJobResult?`，与 E2 同一型）。

### E5. `ListPremiumAccountsAsync` — 获取高级功能账号列表

**签名**

```csharp
[Post("/cgi-bin/meeting/vip/list")]
Task<ListPremiumAccountsResponse> ListPremiumAccountsAsync(
    [Body] ListPremiumAccountsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`cursor`（官方选填，由上一次调用返回、首次可不填）+ `limit`（`int?`，**默认 100、最大 200**）。

**返回**：`has_more`（`bool?`）、`next_cursor`（`string?`）、`userid_list`（`List<string>?`）。

> 官方原话进 DTO 注释：「**不保证每次返回的数据刚好为指定 `limit`，必须用返回的 `has_more` 判断是否继续请求**」⇒ 条数少于 `limit` 不是末页信号。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **两段式任务，不是同步批量** | `submit_*_job` 只回 `jobid`（+ 非法名单），真正的成败在 `*_job_result` | 拿 E1 应答当最终结果，误报「全部成功」 |
| **`invalid_userid_list` ≠ 失败列表** | 非法（不可见 / 无法识别）成员**根本没进任务**；失败列表是任务内执行失败 | 把两者相加当「失败总数」，或指望重试能救不可见成员 |
| `jobid` 是任务句柄，须落库 | 注释未给有效期与轮询间隔 ⇒ 超窗即无源可查 | 只在内存里持有 `jobid` |
| **两套数值不可互借** | E1/E3 `userid_list` ≤**100**；E5 `limit` 默认 100、最大 **200** | 用 200 提交分配任务 |
| 判停只看 `has_more` | E5 明示条数可能少于 `limit` | 以 `count < limit` 结束翻页 |
| 分配幂等、撤销语义不同 | `succ_userid_list` 在分配场景含「本来就是高级账号」；撤销场景无该注记 | 用「成功列表」判断是否首次分配 |
| **与 Mail 域同名不同形** | 邮件侧 `exmail/vip/batch_add` 是同步返回成败两列表、`has_more` 为布尔；本族是异步任务 | 互抄调用编排与响应结构 |
| 顶层 `errcode` 的层次 | `errcode=0` 只代表「接口调用成功 / 任务已受理」，**不代表业务动作完成** | 单层判错就收尾 |
| 仅自建、代开与第三方暂不支持 | 权限口径在接口注释里逐条写明 ⇒ MT2 只有自建子接口 | 给本族补 `_ThirdParty` / `_Provider` 子接口 → 打坏 MT2 |
| 全部官方即 POST | 含查询语义的 E2/E4/E5 都是 POST（全域仅 `layout/list_template` 是 GET） | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不做批次裁剪与轮询编排**、**不校验 userid 可见性**、**不持久化 `jobid`**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 接口调用成功 | E1/E3 成功=任务已受理；E2/E4 成功仍要读 `job_result` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`userid_list` 超 100、`jobid` 不存在或已过期、`limit` 越界、`cursor` 失效等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **任务终态与留存** | 注释只说「异步任务 + 按 `jobid` 查询结果」，**无轮询间隔、无任务有效期、无「执行中」状态位** ⇒ 未逐项核验；轮询节奏由宿主自定，勿在 SDK 侧加隐式重试 | 官方 99508 / 99509 |
| 业务错误码全集 | 注释**未逐条摘录** ⇒ 未逐项核验 | 官方 99508~99510 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| `fail_userid_list` 的失败原因 | 只有 userid 列表，无逐项 `errcode` ⇒ 不可归因 | 官方 99508 |
| 高级功能账号的容量与计费 | 「可分配多少账号、超出怎么办」未记录 | 官方会议版本与账号说明 |
| 与 License 域的关系 | 企业微信「接口许可」账号（License 域）与「会议高级功能账号」是否为同一容量池，本域无声明 | License 域 + 官方会议版本页 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 9 节批次上限表 #102/#104/#106；缺口表「高级账号任务终态与留存」条）
- 同域近邻：[Meeting.md](./Meeting.md)（高级/专业功能购买后的主持人指定与容量上限，正是本族账号的用武之地）
- 跨域对照：[Vip.md](../Mail/Vip.md)（邮件域高级功能账号，**同步批量**、`has_more` 为布尔）· [LicenseAccount.md](../License/LicenseAccount.md)（接口许可账号的激活与转移，另一套容量体系）
- 官方页：[分配高级功能账号（99508）](https://developer.work.weixin.qq.com/document/path/99508) · [取消（99509）](https://developer.work.weixin.qq.com/document/path/99509) · [获取列表（99510）](https://developer.work.weixin.qq.com/document/path/99510)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
