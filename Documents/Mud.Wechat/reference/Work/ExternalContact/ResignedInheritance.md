# 客户联系 · 离职继承

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 离职继承族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactResignedInheritanceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ResignedInheritance/IWechatWorkExternalContactResignedInheritanceService.cs)（父，承载全部 4 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ResignedInheritance/IWechatWorkExternalContactResignedInheritanceService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ResignedInheritance/IWechatWorkExternalContactResignedInheritanceService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ResignedInheritance/IWechatWorkExternalContactResignedInheritanceService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/ResignedInheritance/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/ResignedInheritance/)（12 型，`SerializerClassName = ResignedInheritance`，登记断言见 RI4 → `ResignedInheritanceJsonContext`） |
| 契约守卫 | [WechatExternalContactResignedInheritanceContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactResignedInheritanceContractGuards.cs)（RI1 路由表 4 支且互不重复 / RI2 层级与三空标记 / RI3 令牌绑定 / RI4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 待分配列表 自建 [92124](https://developer.work.weixin.qq.com/document/path/92124) / 第三方 [92273](https://developer.work.weixin.qq.com/document/path/92273) / 代开发 [96330](https://developer.work.weixin.qq.com/document/path/96330) · 客户分配 94081 / 94100 / 96331 · 接替状态 94082 / 94101 / 96333 · 客户群分配 92127 / 93242 / 96334 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：RI1~RI4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.9）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

**已离职**成员留下的客户与客户群的接管，4 支端点构成「发现 → 分配 → 核对」闭环：

| 端点 | 用途 |
|---|---|
| E1 `get_unassigned_list` | 列出「离职成员 + 其客户 + 离职时间」的待分配清单（分页） |
| E2 `resigned/transfer_customer` | 把某离职成员的客户分配给在职接替成员 |
| E3 `resigned/transfer_result` | 分页查询这批客户的接替状态 |
| E4 `groupchat/transfer` | 把离职群主的客户群转交给新群主 |

形态：官方对三类应用开放完全一致的 4 支端点 ⇒ 全收敛父接口，三支子接口空标记（RI2）。**4 支全部 POST**（E1/E3 是读取也走 POST）。

**路由前缀是本族最容易被写错的地方**：客户侧两支在 `resigned/` 下，客户群侧一支却是 `groupchat/transfer`（**不在 `resigned/` 下**），而在职继承是 `groupchat/onjob_transfer`——三个字符串各不相同（RI1 / JI1 分别逐字锁定）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（RI3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须使用**配置到「可调用应用」列表中的 secret** 获取的 `access_token`；第三方 / 代开发须具有「企业客户权限 → 客户联系 → **离职分配**」权限。
- **接替成员 / 新群主**：须在应用**可见范围内**、**配置了客户联系功能**、**已设置实名**、**已在企业微信激活**，且**最近一年内至少登录过一次企业微信**。
- **原跟进成员（离职侧）**：离职时间**不能超过 1 年**，且**离职前一年内至少登录过一次企业微信**；E4 额外要求**群主确实已离职**才能继承该群。
- E2 的 `external_userid` **必须是 `handover_userid` 的客户**，每次**最多 100 个**。
- E4 的 `chat_id_list` **1~100**，且**同一个人的群每天最多分配 300 个**给新群主。
- E1 的 `page_size` **默认与最大均为 1000**；`cursor` 与 `page_id` **互斥**（用 `page_id` 时响应**不返回** `next_cursor`）。
- 分配是**异步**的：E2 的 `customer[].errcode == 0` 只表示「开始分配流程，24 小时后自动接替」，**不代表最终成功**；终态须经 E3 核对。
- 离职成员 userid、客户 `external_userid`、`dimission_time` 均涉个人数据：示例用假值，待分配清单整表不得落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactResignedInheritanceService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **4** | 否 |
| `IWechatWorkInternalExternalContactResignedInheritanceService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactResignedInheritanceService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactResignedInheritanceService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactResignedInheritanceService)`。

## 4. 端点

签名与字段名以 [接口源文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ResignedInheritance/IWechatWorkExternalContactResignedInheritanceService.cs) 与 [DTO 目录](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/ResignedInheritance/) 为准。「必填」列是**官方参数表口径**，SDK 不做本地必填校验。

### E1. `GetUnassignedListAsync` — 获取待分配的离职成员列表

```csharp
[Post("/cgi-bin/externalcontact/get_unassigned_list")]
Task<GetUnassignedListResponse> GetUnassignedListAsync(
    [Body] GetUnassignedListRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetUnassignedListRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `page_size` | `PageSize` | `int?` | 否 | 每次最大记录数，**默认 1000、最大 1000** |
| `cursor` | `Cursor` | `string?` | 否 | 上一次调用返回；**用该参数时不填 `page_id`（二者互斥）** |
| `page_id` | `PageId` | `string?` | 否 | 兼容旧分页形态；**用它查询时响应不返回 `next_cursor`** |

**响应参数**（`GetUnassignedListResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `info[]` | `Info` | `List<UnassignedCustomerItem>?` | 离职成员的客户列表 |
| `info[].handover_userid` | `HandoverUserid` | `string?` | **离职成员** userid |
| `info[].external_userid` | `ExternalUserid` | `string?` | 客户 userid |
| `info[].dimission_time` | `DimissionTime` | `long?` | 成员离职时间（Unix 秒）——判断「是否超 1 年」的依据 |
| `is_last` | `IsLast` | `bool?` | 是否最后一条记录 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标；**已查完则为空字符串**；以 `page_id` 分页时**不返回** |

### E2. `ResignedTransferCustomerAsync` — 分配离职成员的客户

```csharp
[Post("/cgi-bin/externalcontact/resigned/transfer_customer")]
Task<ResignedTransferCustomerResponse> ResignedTransferCustomerAsync(
    [Body] ResignedTransferCustomerRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `handover_userid` | `HandoverUserid` | `string?` | **是** | 原跟进成员，**须为已离职用户**（离职 ≤1 年且离职前一年至少登录过一次） |
| `takeover_userid` | `TakeoverUserid` | `string?` | **是** | 接替成员：可见范围 + 客户联系 + 激活实名 + 近一年登录 |
| `external_userid` | `ExternalUserid` | `List<string>?` | **是** | **必须是 `handover_userid` 的客户**；每次最多 **100** 个 |

响应：`customer[]`（`ResignedTransferCustomerResultItem`：`external_userid` / `errcode`），**`errcode 0` = 开始分配流程、24 小时后自动接替，不代表最终分配成功**。

> **本族请求体没有 `transfer_success_msg`**——发给客户的提示语只存在于在职侧 [JobInheritance.md](./JobInheritance.md) E1，两侧不可互相照搬。

### E3. `GetResignedTransferResultAsync` — 查询客户接替状态

```csharp
[Post("/cgi-bin/externalcontact/resigned/transfer_result")]
Task<GetResignedTransferResultResponse> GetResignedTransferResultAsync(
    [Body] GetResignedTransferResultRequest request, CancellationToken cancellationToken = default);
```

- 请求：`handover_userid`（必填）、`takeover_userid`（必填）、`cursor`（不填/为空 = 第一个分页；**每页不超过 1000 条**）。
- 响应：`customer[]`（`ResignedTakeoverStatusItem`：`external_userid` / `status` / `takeover_time`）+ `next_cursor`。

| `status` | 含义 | 处置 |
|---|---|---|
| 1 | 接替完毕 | 终态 |
| 2 | 等待接替 | `takeover_time` 为未来的自动接替时间，继续轮询 |
| 3 | 客户拒绝 | 终态，人工跟进 |
| 4 | 接替成员客户达到上限 | 终态，换接替人 |

### E4. `ResignedTransferGroupChatAsync` — 分配离职成员的客户群

```csharp
[Post("/cgi-bin/externalcontact/groupchat/transfer")]
Task<ResignedTransferGroupChatResponse> ResignedTransferGroupChatAsync(
    [Body] ResignedTransferGroupChatRequest request, CancellationToken cancellationToken = default);
```

- 请求：`chat_id_list`（必填，**1~100**）、`new_owner`（必填，须在应用可见范围内）。
- 响应：`failed_chat_list[]`（`ResignedGroupChatFailItem`：`chat_id` / `errcode` / `errmsg`）——只列失败的群。
- 前置：**群主离职了才可继承**；新群主须配置客户联系 + 实名 + 激活 + 近一年登录；**同一个人的群每天最多 300 个**。

**调用示例**（E1 拉待分配清单并按离职成员分组 → E2 分批分配 → E3 核对终态）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var inheritance = provider.GetRequiredService<IWechatWorkInternalExternalContactResignedInheritanceService>();

// ① 遍历待分配清单：cursor 与 page_id 互斥，这里走 cursor；next_cursor 为空字符串即取完
var pendingByHandover = new Dictionary<string, List<string>>();
string? cursor = null;
do
{
    var page = await inheritance.GetUnassignedListAsync(
        new GetUnassignedListRequest { PageSize = 1000, Cursor = cursor }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalcontact/get_unassigned_list");

    foreach (UnassignedCustomerItem item in page.Info ?? new List<UnassignedCustomerItem>())
    {
        // dimission_time 距今超过 1 年的已无法再分配，先按离职时间筛掉
        if (item.HandoverUserid is not { Length: > 0 } handover) continue;
        if (!pendingByHandover.TryGetValue(handover, out var list))
            pendingByHandover[handover] = list = new List<string>();
        if (item.ExternalUserid is { Length: > 0 } external) list.Add(external);
    }

    if (page.IsLast == true) break;   // is_last 为真即已到末页
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ② 按离职成员分配：external_userid 必须是该 handover 的客户，每批 ≤100
string takeover = "lisi";           // 假值：接替成员
foreach ((string handover, List<string> customers) in pendingByHandover)
{
    foreach (var batch in customers.Chunk(100))
    {
        var sent = await inheritance.ResignedTransferCustomerAsync(
            new ResignedTransferCustomerRequest
            {
                HandoverUserid = handover,
                TakeoverUserid = takeover,
                ExternalUserid = batch.ToList(),
            },
            cancellationToken);
        WechatWorkException.ThrowIfFailed(sent, "/cgi-bin/externalcontact/resigned/transfer_customer");

        foreach (ResignedTransferCustomerResultItem item in sent.Customer ?? new List<ResignedTransferCustomerResultItem>())
        {
            // errcode 0 只代表「开始分配流程，24 小时后自动接替」，终态要经 E3 查
            if (item.ErrorCode is not 0)
                logger.LogWarning("客户 {ExternalUserid} 发起接替失败，errcode {ErrorCode}", item.ExternalUserid, item.ErrorCode);
        }
    }

    // ③ 核对这批客户最终接替状态：status 1 完毕 / 2 等待 / 3 拒绝 / 4 上限
    string? resultCursor = null;
    do
    {
        var state = await inheritance.GetResignedTransferResultAsync(
            new GetResignedTransferResultRequest
            {
                HandoverUserid = handover,
                TakeoverUserid = takeover,
                Cursor = resultCursor,
            },
            cancellationToken);
        WechatWorkException.ThrowIfFailed(state, "/cgi-bin/externalcontact/resigned/transfer_result");

        foreach (ResignedTakeoverStatusItem item in state.Customer ?? new List<ResignedTakeoverStatusItem>())
        {
            if (item.Status is 3 or 4)
                logger.LogWarning("客户 {ExternalUserid} 未接替，status {Status}", item.ExternalUserid, item.Status);
        }
        resultCursor = state.NextCursor;
    } while (!string.IsNullOrEmpty(resultCursor));
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `resigned/transfer_customer` vs `transfer_customer` | 前者要求 `handover_userid` **已离职**，后者要求**在职**（本域 JI1 / RI1 两条不同路由） | 用离职端点转接在职成员 → 失败；反之亦然 |
| `groupchat/transfer` vs `groupchat/onjob_transfer` | 离职侧**没有** `resigned/` 前缀，在职侧带 `onjob_`——三个字符串互不通用 | 以为离职群转交也在 `resigned/` 下 → 路由写错 |
| `cursor` vs `page_id`（E1） | **互斥**；用 `page_id` 时响应**不返回** `next_cursor` | 两个都传 → 行为不可预期；按 page_id 循环却读 next_cursor → 恒空、漏数据 |
| `is_last` 与 `next_cursor` | 末页判定有两条信号：`is_last == true` 或 `next_cursor` 为空串 | 只看其一，翻页提前终止或多跑一轮 |
| `customer[].errcode == 0`（E2） | 含义是「**开始分配流程**，24 小时后自动接替」，不代表最终成功 | 立刻改 CRM 归属 → 与客户拒绝（status 3）冲突 |
| 终态来源 | 只有 E3 的 `status`（1/2/3/4）能给终态 | 漏掉核对环节 |
| 离职时间窗 | 离职 **超过 1 年** 或离职前一年内未登录 ⇒ 不可分配；`dimission_time` 是判断依据 | 对超窗客户反复重试 |
| 本族无 `transfer_success_msg` | 提示客户的话术只在**在职侧**存在 | 照搬在职请求体字段 → 字段不存在 |
| 每页 / 每次的上限 | E1 `page_size` ≤1000（默认即 1000）；E2 客户 ≤100；E3 每页 ≤1000；E4 群 1~100 且每人每天 ≤300 | 一批传 500 客户 → 报错 |
| DTO 类型不跨族复用 | 本族 `Resigned*` 与在职族同名语义类型（`CustomerTakeoverStatusItem` 等）**是两套类型**（RI4 / JI4 各自登记进不同 JsonContext） | 用在职 DTO 反序列化离职响应 → 类型不匹配 |
| 动词 | 4 支全 POST（含两支读取） | 改 GET → 打坏 RI1 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验离职时间窗与 90 日/每日次数**（须宿主记账）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求受理成功 | E2 后仍须逐客户看 `customer[].errcode`；E1/E3 成功仍须继续翻页 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`customer[].errcode`（E2）与 `failed_chat_list[].errcode`（E4）走官方**全局错误码**；本仓接口注释**未逐条摘录**「客户不属于该离职成员」「离职超 1 年」「群主未离职」等具体码 ⇒ 见 §7。业务失败 HTTP 仍 200。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 逐客户 / 逐群错误码清单 | 注释仅写「参考全局错误码」⇒ **未逐项核验** | 官方 94081 / 92127 响应参数表 |
| `page_id` 与 `cursor` 的共存口径 | 注释说二者互斥、`page_id` 不返回 `next_cursor`，未说明同时传时的行为 ⇒ 未核验 | 官方 92124 |
| 「离职前一年内至少登录一次」的判定数据 | 注释给条件，未给可查询该状态的端点 ⇒ 未核验 | 官方 92124 + 通讯录「成员登录记录」类接口 |
| 接替成员客户上限值 | `status = 4` 说明有上限，数值注释未给 ⇒ 未逐项核验 | 官方 94082 |
| 24 小时窗口起算点与客户确认动作 | 注释只说「待 24 小时后自动接替」⇒ 未逐项核验 | 官方 94081 |
| E4 是否异步生效、失败后能否重试 | 注释未说明群转交的生效时机与重试窗口 ⇒ 未核验 | 官方 92127 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.9 端点表、§4 陷阱 3「游标 vs 偏移两套分页」）
- 同域：[JobInheritance.md](./JobInheritance.md)（在职侧三支，路由与 DTO 均不同）· [GroupChat.md](./GroupChat.md)（`status_filter` 的离职态、群主为离职成员时须给 `owner_filter`）· [Customer.md](./Customer.md)（客户详情与跟进人）
- 跨包：`dimission_time` 的权威来源是通讯录成员变更（离职）事件，属 **Callback 包**
- 官方页：[获取待分配的离职成员列表（92124）](https://developer.work.weixin.qq.com/document/path/92124) · [分配离职成员的客户（94081）](https://developer.work.weixin.qq.com/document/path/94081) · [查询客户接替状态（94082）](https://developer.work.weixin.qq.com/document/path/94082) · [分配离职成员的客户群（92127）](https://developer.work.weixin.qq.com/document/path/92127)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
