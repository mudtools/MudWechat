# 客户联系 · 在职继承

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 在职继承族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactJobInheritanceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/JobInheritance/IWechatWorkExternalContactJobInheritanceService.cs)（父，承载全部 3 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/JobInheritance/IWechatWorkExternalContactJobInheritanceService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/JobInheritance/IWechatWorkExternalContactJobInheritanceService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/JobInheritance/IWechatWorkExternalContactJobInheritanceService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/JobInheritance/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/JobInheritance/)（9 型，`SerializerClassName = JobInheritance`，登记断言见 JI4 → `JobInheritanceJsonContext`） |
| 契约守卫 | [WechatExternalContactJobInheritanceContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactJobInheritanceContractGuards.cs)（JI1 路由表 3 支且互不重复 / JI2 层级与三空标记 / JI3 令牌绑定 / JI4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 客户接替 自建 [92125](https://developer.work.weixin.qq.com/document/path/92125) / 第三方 [94096](https://developer.work.weixin.qq.com/document/path/94096) / 代开发 [96325](https://developer.work.weixin.qq.com/document/path/96325) · 接替状态 94088 / 94097 / 96327 · 客户群接替 95703 / 95742 / 96328 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：JI1~JI4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.11）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

**在职**成员的资产转接，3 支端点：

| 端点 | 用途 |
|---|---|
| E1 `transfer_customer` | 把在职原跟进成员的客户分配给接替成员（异步，24 小时后自动接替） |
| E2 `transfer_result` | 分页查询这批客户的**接替状态**（成功/等待/拒绝/上限） |
| E3 `groupchat/onjob_transfer` | 把在职群主为群的客户群转交给新群主 |

形态：官方对三类应用开放完全一致的 3 支端点 ⇒ 全收敛父接口，三支子接口空标记（JI2）。**3 支全部 POST**，且都是「写操作 + 异步生效」。

**与离职继承的分工**：本族针对**未离职**成员（`transfer_customer` / `transfer_result` / `groupchat/onjob_transfer`）；已离职成员走 [ResignedInheritance.md](./ResignedInheritance.md)（`resigned/transfer_customer` / `resigned/transfer_result` / `groupchat/transfer`）。两族路由不同、DTO 类型不同，**不可互相套用**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（JI3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须使用**配置到「可调用应用」列表中的 secret** 获取的 `access_token` 调用；第三方 / 代开发须具有「企业客户权限 → 客户联系 → **在职继承**」（E1/E2）与「… → **分配在职成员的客户群**」（E3）权限。
- **接替成员 / 新群主必须在应用可见范围内**，且**配置了客户联系功能、已设置实名、已在企业微信激活**。
- **原跟进成员与接替成员在最近一年内需至少登录过一次企业微信**（E3 的新群主同要求）。
- **90 个自然日内，在职成员的每位客户仅可被转接 2 次**；**每个客户群仅可被转接 2 次**。
- **同一个人的群每天最多分配 300 个给新群主**（E3）。
- E1 的 `external_userid` **每次最多 100 个**；E2 每页**不超过 1000 条**；E3 的 `chat_id_list` **取值范围 1~100**。
- `transfer_success_msg` 最多 **200 个字符**，不填则用官方默认文案；这条消息会**直接发给客户**。
- **旧接口口径**：原「分配在职或离职成员的客户」「查询客户接替结果」官方已不再更新维护，注释明确要求改用本族 E1/E2。
- `userid`、`external_userid`、客户群 ID 均为个人/企业敏感标识：示例用假值，转接结果与消息文案不得整份落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ✅ | ✅ |

代开发文档树（96325 / 96327 / 96328）与自建 / 第三方同路由（守卫 `remarks` 留档）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactJobInheritanceService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalExternalContactJobInheritanceService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactJobInheritanceService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactJobInheritanceService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactJobInheritanceService)`。

## 4. 端点

签名与字段名以 [接口源文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/JobInheritance/IWechatWorkExternalContactJobInheritanceService.cs) 与 [DTO 目录](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/JobInheritance/) 为准。「必填」列是**官方参数表口径**，SDK 不做本地必填校验。

### E1. `TransferCustomerAsync` — 分配在职成员的客户

```csharp
[Post("/cgi-bin/externalcontact/transfer_customer")]
Task<TransferCustomerResponse> TransferCustomerAsync(
    [Body] TransferCustomerRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`TransferCustomerRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `handover_userid` | `HandoverUserid` | `string?` | **是** | 原跟进成员 userid（**在职**） |
| `takeover_userid` | `TakeoverUserid` | `string?` | **是** | 接替成员 userid：可见范围内、配置客户联系、已激活实名 |
| `external_userid` | `ExternalUserid` | `List<string>?` | **是** | 客户列表，**每次最多 100 个** |
| `transfer_success_msg` | `TransferSuccessMsg` | `string?` | 否 | 转移成功后发给客户的消息，**≤200 字符**；不填用默认文案 |

**响应参数**（`TransferCustomerResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `customer[]` | `Customer` | `List<TransferCustomerResultItem>?` | **逐客户**结果 |
| `customer[].external_userid` | `ExternalUserid` | `string?` | 客户 id |
| `customer[].errcode` | `ErrorCode` | `int?` | **0 = 成功发起接替**（待 24 小时后自动接替），**不代表最终成功**；其它值参考全局错误码 |

### E2. `GetTransferResultAsync` — 查询客户接替状态

```csharp
[Post("/cgi-bin/externalcontact/transfer_result")]
Task<GetTransferResultResponse> GetTransferResultAsync(
    [Body] GetTransferResultRequest request, CancellationToken cancellationToken = default);
```

- 请求：`handover_userid`（必填）、`takeover_userid`（必填）、`cursor`（不填/为空 = 第一个分页；**每页不超过 1000 条**）。
- 响应：`customer[]`（`CustomerTakeoverStatusItem`：`external_userid` / `status` / `takeover_time`）+ `next_cursor`。

| `status` | 含义 | 处置 |
|---|---|---|
| 1 | 接替完毕 | 终态，可收尾 |
| 2 | 等待接替 | `takeover_time` 为**未来的自动接替时间**，继续轮询 |
| 3 | 客户拒绝 | 终态，需人工跟进 |
| 4 | 接替成员客户达到上限 | 终态，换接替人或先清理 |

### E3. `TransferGroupChatAsync` — 分配在职成员的客户群

```csharp
[Post("/cgi-bin/externalcontact/groupchat/onjob_transfer")]
Task<TransferGroupChatResponse> TransferGroupChatAsync(
    [Body] TransferGroupChatRequest request, CancellationToken cancellationToken = default);
```

- 请求：`chat_id_list`（必填，**1~100** 个）、`new_owner`（必填，新群主 userid）。
- 响应：`failed_chat_list[]`（`TransferGroupChatFailItem`：`chat_id` / `errcode` / `errmsg`）——**只列失败的群**，不在列表里的即已发起转交。
- 约束：新群主须配置客户联系 + 实名 + 已激活，且群主在原可见范围内；**同一人的群每天最多 300 个**；**90 日内每个群仅可转接 2 次**。

**调用示例**（E1 分批发起转接 → E2 轮询接替状态 → E3 转交客户群并收失败清单）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var inheritance = provider.GetRequiredService<IWechatWorkInternalExternalContactJobInheritanceService>();

// 转接对象（假值）：原在职成员的客户清单
string handover = "zhangsan";
string takeover = "lisi";
var externalUserIds = new List<string> { "wmExternalId0001", "wmExternalId0002" };

// ① 每批最多 100 个客户；逐客户看 customer[].errcode，顶层 errcode 为 0 不代表每个客户都成功
foreach (var batch in externalUserIds.Chunk(100))
{
    var sent = await inheritance.TransferCustomerAsync(
        new TransferCustomerRequest
        {
            HandoverUserid = handover,
            TakeoverUserid = takeover,
            ExternalUserid = batch.ToList(),
            TransferSuccessMsg = "你的服务顾问已变更为小李（示例文案，≤200 字符）",
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(sent, "/cgi-bin/externalcontact/transfer_customer");

    foreach (TransferCustomerResultItem item in sent.Customer ?? new List<TransferCustomerResultItem>())
    {
        // errcode 0 = 成功发起接替，24 小时后自动接替；最终结论以 E2 的 status 为准
        if (item.ErrorCode is not 0)
            logger.LogWarning("客户 {ExternalUserid} 发起接替失败，errcode {ErrorCode}", item.ExternalUserid, item.ErrorCode);
    }
}

// ② 轮询接替状态（每页 ≤1000）：status 2 表示等待接替，takeover_time 是未来的自动接替时间
string? cursor = null;
var rejected = new List<string>();
do
{
    var state = await inheritance.GetTransferResultAsync(
        new GetTransferResultRequest { HandoverUserid = handover, TakeoverUserid = takeover, Cursor = cursor },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(state, "/cgi-bin/externalcontact/transfer_result");

    foreach (CustomerTakeoverStatusItem item in state.Customer ?? new List<CustomerTakeoverStatusItem>())
    {
        if (item.Status is 3 or 4) rejected.Add(item.ExternalUserid!); // 3 客户拒绝 / 4 接替人客户数已达上限
    }
    cursor = state.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ③ 客户群转交：chat_id_list 1~100；响应只回 failed_chat_list，未出现的群即已发起转交
var chatIds = new List<string> { "wrChatId0001", "wrChatId0002" };
foreach (var batch in chatIds.Chunk(100))
{
    var moved = await inheritance.TransferGroupChatAsync(
        new TransferGroupChatRequest { ChatIdList = batch.ToList(), NewOwner = takeover },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(moved, "/cgi-bin/externalcontact/groupchat/onjob_transfer");

    foreach (TransferGroupChatFailItem fail in moved.FailedChatList ?? new List<TransferGroupChatFailItem>())
    {
        logger.LogWarning("群 {ChatId} 未能继承：{ErrorCode} {ErrorMessage}", fail.ChatId, fail.ErrorCode, fail.ErrorMessage);
    }
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `transfer_customer`（本族 E1）vs `resigned/transfer_customer` | 前者原成员**在职**，后者**已离职**（且离职 ≤1 年、离职前一年至少登录过一次） | 用在职端点转接离职成员客户 → 失败 |
| `groupchat/onjob_transfer` vs `groupchat/transfer` | 「onjob」拼写属官方契约，路由互不通用（JI1 / RI1 各自锁定） | 手写 URL 少写 `onjob_` → 打到离职继承 |
| `transfer_result` vs `resigned/transfer_result` | 同名语义、不同路由与**不同 DTO 类型**（`CustomerTakeoverStatusItem` vs `ResignedTakeoverStatusItem`） | 跨族复用 DTO 反序列化 → 类型对不上 |
| 顶层 `errcode` vs `customer[].errcode` | 顶层 0 只说明请求被受理，**每个客户各自有结果** | 只看顶层就认为全部转接成功 |
| `customer[].errcode == 0` | 含义是「**成功发起**接替，24 小时后自动接替」，**不代表最终成功** | 立即把 CRM 里的归属改过来 → 与客户拒绝的现实冲突 |
| 终态判定 | 最终结论只有 E2 的 `status`（1 完毕 / 2 等待 / 3 拒绝 / 4 上限）能给出 | 漏掉轮询环节 |
| E1 的 100 / E2 的 1000 / E3 的 100 | 三个数字分属「单次客户数 / 每页条数 / 单次群数」 | 一次传 500 个客户 → 报错 |
| 90 日 2 次 vs 每天 300 群 | 前者是**每客户 / 每群**的转接次数上限，后者是**每个原群主**每天的转交量上限 | 反复重试同一客户触发次数上限 |
| `transfer_success_msg` | ≤200 字符且**直接发给客户** | 把内部工单号或客户手机号写进文案 |
| 旧接口 | 「分配在职或离职成员的客户」「查询客户接替结果」官方不再维护，注释要求用 E1/E2 | 沿用旧端点，未来失效 |
| 动词 | 3 支全 POST | 改成 GET → 打坏 JI1 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不做可见范围/次数上限前置校验**（次数与 90 日窗口须宿主自行记账）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求受理成功 | E1 后仍须逐客户看 `customer[].errcode`；E2 成功仍须看 `next_cursor` 续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`customer[].errcode`（E1 逐客户）与 `failed_chat_list[].errcode`（E3 逐群）走官方**全局错误码**，本仓接口注释**未逐条摘录**本族专属码 ⇒ 见 §7。业务失败时 HTTP 仍返回 200，判定权在调用方（`WechatWorkException.ThrowIfFailed`）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 逐客户 / 逐群错误码清单 | 注释只写「参考全局错误码」，未列具体码 ⇒ **未逐项核验** | 官方 92125 / 95703 响应参数表 + 全局错误码页 |
| 接替成员客户上限值 | `status = 4` 说明存在上限，但**上限数值注释未给** ⇒ 未逐项核验 | 官方 92125 / 94088 |
| 24 小时窗口的起算点 | 注释说「发起接替后待 24 小时自动接替」，未说是否受客户确认动作影响 ⇒ 未逐项核验 | 官方 92125 |
| 每日 / 每分钟频率 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |
| E3 是否异步生效 | 注释未说明群主转交是即时还是异步，只给「失败清单」⇒ **未逐项核验** | 官方 95703 |
| `next_cursor` 过期语义 | E2 未给游标有效期（本域 ServedContact 侧明写 4 小时，本端点未写）⇒ 未逐项核验 | 官方 94088 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.11 端点表）
- 同域：[ResignedInheritance.md](./ResignedInheritance.md)（离职侧三支，路由与 DTO 均不同）· [Customer.md](./Customer.md)（客户列表 / 详情，转接前的客户归属来源）· [GroupChat.md](./GroupChat.md)（`chat_id` 与群主、`status_filter` 的离职态）
- 跨包：客户联系变更事件（含接替相关事件）属 **Callback 包**，本域只声明 HTTP 端点
- 官方页：[分配在职成员的客户（92125）](https://developer.work.weixin.qq.com/document/path/92125) · [查询客户接替状态（94088）](https://developer.work.weixin.qq.com/document/path/94088) · [分配在职成员的客户群（95703）](https://developer.work.weixin.qq.com/document/path/95703)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
