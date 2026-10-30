# 微信客服 · 接待人员管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 接待人员管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfServicerService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfServicerService.cs)（父，承载全部 3 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfServicerService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfServicerService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfServicerService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（命名空间 `Mud.Wechat.Work.DataModels.Kf`；`AddKfServicerRequest` / `DeleteKfServicerRequest` / `AddKfServicerResponse` / `DeleteKfServicerResponse` / `KfServicer` / `KfServicerOperateResult` 等，`SerializerClassName = Kf`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1b** 三支路由 + `open_kfid` 非可空形态 / KF2 三类子接口零端点空标记 / KF3 令牌绑定 / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [94646](https://developer.work.weixin.qq.com/document/path/94646)（添加）/ 94647（删除）/ [94645](https://developer.work.weixin.qq.com/document/path/94645)（列表）· 代开发 96418 / 96419 / 96420 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1b / KF2 / KF3 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§9 批量语义与可见范围）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

给某个客服账号（`open_kfid`）**挂上 / 摘掉接待力量**（按成员或按部门），3 支端点：

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `AddServicerAsync` | `POST /cgi-bin/kf/servicer/add` | 批量添加接待成员 / 接待部门 |
| E2 | `DeleteServicerAsync` | `POST /cgi-bin/kf/servicer/del` | 批量删除接待成员 / 接待部门 |
| E3 | `GetServicerListAsync` | `GET /cgi-bin/kf/servicer/list` | 取某账号的接待人员列表（含按部门条目与停接待状态） |

形态：官方对**三类应用开放完全一致的 3 支** ⇒ 收敛父接口 + 三个零差异空标记子接口（KF2）。**E3 是本域仅有的两支 GET 之一**（`open_kfid` 走 Query）。

E3 的 `status` / `stop_type` 与 [KfSession.md](./KfSession.md) 的会话状态机是两件事：本族描述的是「这个人现在接不接待」，会话族描述的是「这个客户当前由谁接待」。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（KF3） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

第三方 / 代开发须先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**微信客服-可调用接口的应用**」中，**且须有该客服账号的管理权限**。
- 第三方 / 代开发须具有「**微信客服 → 管理账号、分配会话和收发消息**」权限（E1/E2）或「**微信客服 → 获取基础信息**」权限（E3），同样须有该账号管理权限。
- **`userid_list` 与 `department_id_list` 至少填一个**（E1/E2 同口径）。
- 容量：每个客服账号**最多 2000 个接待人员**、**20 个接待部门**。
- 单批规模：E1 的 `userid_list` **0~100**、`department_id_list` **0~20**；E2 的 `userid_list` 与 `department_id_list` **各 0~100，超出分批调用**（两侧上限不同，见 §5）。
- **批量为逐条返回结果**：`result_list` 与请求的 `userid_list` / `department_id_list` **逐条对应**，**单条失败不整体报错** ⇒ 顶层 `errcode == 0` 不等于每个人都加上了。
- 第三方 / 代开发**填密文 userid（即 `open_userid`）**；接待人员应在应用**可见范围**内。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 接待成员 userid 属员工标识：示例用假值，名单整体不要落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ✅（userid 用密文 `open_userid`） | ✅（同） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfServicerService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalKfServicerService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyKfServicerService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderKfServicerService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfServicerService)`。

## 4. 端点

### E1. `AddServicerAsync` — 添加接待人员

```csharp
[Post("/cgi-bin/kf/servicer/add")]
Task<AddKfServicerResponse> AddServicerAsync(
    [Body] AddKfServicerRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string?` | **是** | 客服账号 ID |
| `userid_list` | `UserIdList` | `List<string>?` | 条件 | **0~100** 个，超 100 分批；第三方 / 代开发填密文 `open_userid` |
| `department_id_list` | `DepartmentIdList` | `List<int>?` | 条件 | **0~20** 个（与 `userid_list` 至少填一个） |

响应：`result_list[]`（`KfServicerOperateResult`）＝ `userid` / `department_id` / **`errcode`（0 成功）** / `errmsg`（如 `success` / `ignored`），**与请求逐条对应**。

### E2. `DeleteServicerAsync` — 删除接待人员

```csharp
[Post("/cgi-bin/kf/servicer/del")]
Task<DeleteKfServicerResponse> DeleteServicerAsync(
    [Body] DeleteKfServicerRequest request, CancellationToken cancellationToken = default);
```

- 请求：`open_kfid`（必填）+ `userid_list`（**0~100**，超 100 分批）+ `department_id_list`（**0~100**，超 100 分批）——**两侧上限都是 100，与 E1 的「部门 ≤20」不同**。
- 响应：同样逐条返回 `result_list`。

### E3. `GetServicerListAsync` — 获取接待人员列表

```csharp
[Get("/cgi-bin/kf/servicer/list")]
Task<GetKfServicerListResponse> GetServicerListAsync(
    [Query("open_kfid")] string openKfId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/kf/servicer/list` |
| 参数 | Query `open_kfid`（官方必填；形参是**非可空 `string`**，KF1b 锁定该形态） |

**响应参数**（`servicer_list[]` → `KfServicer`，按成员与按部门两种条目混列）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid` | `UserId` | `string?` | **按成员接待时返回**；第三方 / 代开发拿到的是密文 `open_userid` |
| `status` | `Status` | `int?` | **0 接待中 / 1 停止接待** |
| `stop_type` | `StopType` | `int?` | 「停止接待」的子类型：**0 停止接待 / 1 暂时挂起** |
| `department_id` | `DepartmentId` | `int?` | **按部门接待时返回**（`int?`） |

**调用示例**（E1 添加成员与部门并逐条判结果 → E3 读列表区分两种条目 → E2 分批摘除）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Kf;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "kf-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddKfApi());

var servicers = provider.GetRequiredService<IWechatWorkInternalKfServicerService>();
string openKfId = "wkOpenKfId假值";

// ① 添加：userid_list 0~100、department_id_list 0~20，两者至少填一个
var added = await servicers.AddServicerAsync(
    new AddKfServicerRequest
    {
        OpenKfId = openKfId,
        UserIdList = new List<string> { "zhangsan", "lisi" },       // 假值；第三方/代开发填 open_userid
        DepartmentIdList = new List<int> { 123456 },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/kf/servicer/add");

foreach (KfServicerOperateResult item in added.ResultList ?? new List<KfServicerOperateResult>())
{
    // 顶层 errcode==0 不代表每个人都成功：逐条看 result_list[].errcode / errmsg（如 success / ignored）
    if (item.ErrorCode is not 0)
        logger.LogWarning("添加接待未成功：userid {Userid} / dept {Dept}，errcode {Code} {Message}",
            item.UserId, item.DepartmentId, item.ErrorCode, item.ErrorMessage);
}

// ② 列表：GET + Query open_kfid（非可空 string）；按成员条目有 userid，按部门条目有 department_id
var list = await servicers.GetServicerListAsync(openKfId, cancellationToken);
WechatWorkException.ThrowIfFailed(list, "/cgi-bin/kf/servicer/list");

int activeMembers = 0, stoppedMembers = 0, deptEntries = 0;
foreach (KfServicer s in list.ServicerList ?? new List<KfServicer>())
{
    if (s.DepartmentId is { } deptId) { deptEntries++; continue; }          // 按部门接待的条目
    if (s.Status == 0) activeMembers++;
    else if (s.Status == 1)
    {
        stoppedMembers++;
        // stop_type：0 停止接待 / 1 暂时挂起
        logger.LogInformation("成员 {Userid} 停止接待，stop_type {StopType}", s.UserId, s.StopType);
    }
}
logger.LogInformation("接待配置：进行中 {Active}、已停止 {Stopped}、按部门条目 {Dept}", activeMembers, stoppedMembers, deptEntries);

// ③ 摘除：成员与部门各 ≤100，超出分批；同样逐条回结果
foreach (var batch in new[] { "zhangsan" })
{
    var deleted = await servicers.DeleteServicerAsync(
        new DeleteKfServicerRequest { OpenKfId = openKfId, UserIdList = new List<string> { batch } },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/kf/servicer/del");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 vs E2 的部门上限 | 添加侧 `department_id_list` **0~20**；删除侧 **0~100**（成员两侧都是 100） | 用同一常量既加又删 → 添加时超限 |
| 「20 个部门」的两个含义 | **单次调用 ≤20**（E1）与**每个账号最多 20 个接待部门**（容量上限）是两回事 | 分 5 批各 20 个部门加到同一账号 → 撞账号容量 |
| 顶层 `errcode` vs `result_list[].errcode` | 批量**逐条**返回，单条失败不整体报错；`errmsg` 可为 `ignored` | 只看顶层就写「全部添加成功」 |
| `userid_list` / `department_id_list` | **至少填一个**；两者同时为空是非法请求 | 传空列表期望「清空」 |
| 按成员 vs 按部门条目 | 列表里两类混列：有 `userid` 的是成员条目，有 `department_id`（`int?`）的是部门条目 | 一律读 `UserId` → 部门条目恒空 |
| `status` vs `stop_type` | `status` 0 接待中 / 1 停止接待；`stop_type` 只在停止接待时区分 **0 停止接待 / 1 暂时挂起** | 把 `stop_type=1` 当永久停接待 |
| 会话状态机在另一族 | `service_state`（0/1/2/3/4）在 [KfSession.md](./KfSession.md)，本族 `status` 是**人员接待开关**，两套数字不可互换 | 用 `KfServicer.Status` 判断「能否给客户发消息」 |
| 明文 vs 密文 userid | 第三方 / 代开发**一律填/取密文 `open_userid`**；自建才是明文 userid | 跨形态照传明文 id |
| 管理权限前置 | 需**该客服账号的管理权限**（未设「允许 API 管理」或未配可管理账号即失败） | 以为是令牌问题 |
| 可见范围 | 接待人员须在应用可见范围内 | 加了不可见成员，会话分配不到人 |
| E3 的动词与形态 | 官方即 **GET**，`open_kfid` 走 Query 且形参为**非可空 `string`**（KF1b 锁） | 传 `null` / 改 POST |
| 每账号 2000 人 | 全量硬上限，与单批 100 不同层 | 无节制地循环加人 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不裁剪批次**（超 100 由官方拒绝）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求受理 | **E1/E2 必须再逐条看 `result_list[].errcode`**（0 成功，`errmsg` 可为 `success` / `ignored`） |
| 95014 | 官方业务码（会话族注释留档） | 目标接待人员未在企业微信激活 | 把未激活成员设为接待时的码（本域口径，见 §7） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

超出 2000 人 / 20 部门、账号未授权 API 管理、成员不在可见范围等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `result_list[].errcode` 的取值集 | 注释只给「0 表示成功」与 `errmsg` 形如 `success` / `ignored`，**失败态有哪些码未摘录** ⇒ 未逐项核验 | 官方 94646 / 94647 |
| 「暂时挂起」的产生方式 | 注释给出 `stop_type = 1` 的含义，但**如何进入挂起状态**（管理后台设置 / 会话分配策略）未说明 ⇒ 未核验 | 官方 94645 + 微信客服管理后台 |
| 95014 是否也适用于本族 | 该码写在**会话族**（trans 状态到人工接待时）的注释里，本族「添加未激活成员」是否返回同一码**未在本域留档** ⇒ 未核验 | 官方 94646 + 全局错误码页 |
| 删除接待人员进行中会话的处理 | 注释未说明该成员正在接待的会话如何流转 ⇒ 未核验 | 官方 94647 |
| 端点频率限制 | 注释未给数值 ⇒ 未逐项核验 | 官方访问频率附录 90312 |
| 按部门接待的成员展开口径 | 列表是否展开部门下的成员、部门新增成员是否自动生效，注释未说明 ⇒ 未核验 | 官方 94645 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§9 批量语义与可见范围、§14 「查询类官方即 POST」与本域仅两支 GET）
- 同域：[KfAccount.md](./KfAccount.md)（先有账号与「可管理的客服账号」配置）· [KfSession.md](./KfSession.md)（`service_state=3` 时 `servicer_userid` 必填、须处于「正在接待」）· [KfStatistics.md](./KfStatistics.md)（`servicer_userid` 维度统计）
- 跨线：**「企业服务人员」在客户联系域是另一概念**（[FollowUser.md](../ExternalContact/FollowUser.md) 的 `get_follow_user_list` 返回配置了**客户联系**功能的成员），与微信客服的接待人员不是同一集合
- 官方页：[获取接待人员列表（94645）](https://developer.work.weixin.qq.com/document/path/94645) · [添加接待人员（94646）](https://developer.work.weixin.qq.com/document/path/94646) · [删除接待人员（94647）](https://developer.work.weixin.qq.com/document/path/94647) · [代开发 96420](https://developer.work.weixin.qq.com/document/path/96420)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
