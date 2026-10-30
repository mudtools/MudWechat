# 微信客服 · 「升级服务」配置与触发

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 升级服务族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfUpgradeService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfUpgradeService.cs)（父，承载全部 3 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfUpgradeService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfUpgradeService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfUpgradeService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（`GetKfUpgradeServiceConfigResponse` + `KfUpgradeMemberRange` / `KfUpgradeGroupchatRange`；`UpgradeKfServiceRequest` + `KfUpgradeMember` / `KfUpgradeGroupchat`；`CancelKfUpgradeServiceRequest`，命名空间 `Mud.Wechat.Work.DataModels.Kf`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1h** 三支路由 + `department_id_list` 的 `[JsonPropertyName]` 断言 / KF2 层级 / KF3 令牌绑定 / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [94674](https://developer.work.weixin.qq.com/document/path/94674) · 第三方 [94702](https://developer.work.weixin.qq.com/document/path/94702) · 代开发 [96422](https://developer.work.weixin.qq.com/document/path/96422) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1h / KF2 / KF3 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§13 GET 无参 + 字段名冲突处置）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

把会话**升级到专员 1v1 或客户群**（企微侧称「升级服务」），3 支端点：

| 端点 | 用途 |
|---|---|
| E1 `GET customer/get_upgrade_service_config` | 查企业已配置的**专员范围 / 客户群范围**（**GET 且无请求参数**） |
| E2 `POST customer/upgrade_service` | 对某客户发起升级：`type = 1` 推荐专员、`type = 2` 推荐客户群（可带推荐语） |
| E3 `POST customer/cancel_upgrade_service` | 取消某客户的升级服务 |

形态：官方对**三类应用开放完全一致的 3 支** ⇒ 收敛父接口 + 三个零差异空标记子接口（KF2）。

**关键依赖方向**：E2 只能推**已经配置在「升级服务」范围内的专员 / 客户群**——范围由管理后台配置（或用 E1 读回），E1 与 E2 因此构成「读范围 → 在范围内选人/群 → 发起」的闭环。

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

- 自建应用须配置到「**微信客服-可调用接口的应用**」中；第三方 / 代开发须具有「**微信客服 → 服务工具 → 配置『升级服务』**」权限。
- **E2 指定的 `userid` / `chat_id` 必须已经配置在微信客服「升级服务」的专员服务或客户群服务范围内**，否则官方返回 **95021**。
- **`userid` 须在「客户联系」使用范围内**；三支端点均要求**接待人员**与 **`chat_id` 对应群的群主**在**应用可见范围**内。
- `type` 决定填哪个块：**`type = 1`（专员服务）填 `member`，其中 `member.userid` 官方必填**；**`type = 2`（客户群服务）填 `groupchat`，其中 `groupchat.chat_id` 官方必填**；`wording`（推荐语）两块各可选填。
- E3 只需 `open_kfid` + `external_userid`（均官方必填）。
- 客户 `external_userid`、专员 `userid`、群 `chat_id` 属个人/企业标识：示例用假值，名单不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfUpgradeService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalKfUpgradeService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyKfUpgradeService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderKfUpgradeService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfUpgradeService)`。

## 4. 端点

### E1. `GetUpgradeServiceConfigAsync` — 获取升级服务配置范围

```csharp
[Get("/cgi-bin/kf/customer/get_upgrade_service_config")]
Task<GetKfUpgradeServiceConfigResponse> GetUpgradeServiceConfigAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/kf/customer/get_upgrade_service_config` |
| 参数 | **官方无请求参数**（只有 Query `access_token`）⇒ 形参表仅 `cancellationToken`，也没有 Request DTO |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `member_range` | `MemberRange` | `KfUpgradeMemberRange?` | 专员服务配置范围 |
| `member_range.userid_list` | `UserIdList` | `List<string>?` | 专员 userid 列表 |
| `member_range.department_id_list` | `DepartmentIdList` | `List<int>?` | 专员部门列表——**官方参数表写作 `department_list`，官方 JSON 示例与自建文档写作 `department_id_list`**；本仓**以 JSON 示例为准**（KF1h 断言，对齐 `cusor`→`cursor` 先例） |
| `groupchat_range` | `GroupchatRange` | `KfUpgradeGroupchatRange?` | 客户群配置范围 |
| `groupchat_range.chat_id_list` | `ChatIdList` | `List<string>?` | 客户群 id 列表 |

### E2. `UpgradeServiceAsync` — 升级服务（推荐专员或客户群）

```csharp
[Post("/cgi-bin/kf/customer/upgrade_service")]
Task<WechatWorkResponse> UpgradeServiceAsync(
    [Body] UpgradeKfServiceRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string?` | **是** | 客服账号 ID |
| `external_userid` | `ExternalUserId` | `string?` | **是** | 目标客户 |
| `type` | `Type` | `int?` | **是** | **1 专员服务 / 2 客户群服务** |
| `member` | `Member` | `KfUpgradeMember?` | 条件 | **`type = 1` 时有效**：`userid`（**官方必填**，须在升级服务范围内 + 客户联系使用范围内）+ `wording`（推荐语） |
| `groupchat` | `Groupchat` | `KfUpgradeGroupchat?` | 条件 | **`type = 2` 时有效**：`chat_id`（**官方必填**，须在升级服务范围内）+ `wording`（推荐语） |

响应仅 `errcode` / `errmsg`（**返回体不含升级结果状态**；发起成功不等于客户已添加专员/入群）。

### E3. `CancelUpgradeServiceAsync` — 取消升级服务

```csharp
[Post("/cgi-bin/kf/customer/cancel_upgrade_service")]
Task<WechatWorkResponse> CancelUpgradeServiceAsync(
    [Body] CancelKfUpgradeServiceRequest request, CancellationToken cancellationToken = default);
```

- 请求：`open_kfid`（官方必填）+ `external_userid`（官方必填）——**按「账号 × 客户」取消，不区分专员/群两种类型**。

**调用示例**（读配置范围 → 在范围内挑一个专员发起升级 → 按会话维度取消）

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

var upgrade = provider.GetRequiredService<IWechatWorkInternalKfUpgradeService>();
string openKfId = "wkOpenKfId假值";
string externalUserId = "wmCustomer假值";

// ① 配置范围：GET 且无请求参数；department_id_list 以官方 JSON 示例为准（参数表误写 department_list）
var range = await upgrade.GetUpgradeServiceConfigAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(range, "/cgi-bin/kf/customer/get_upgrade_service_config");

// ② 发起升级：type=1 必须填 member.userid，且该 userid 必须在上面 member_range 里（否则官方 95021）
string? candidate = range.MemberRange?.UserIdList?.FirstOrDefault();
if (candidate is { Length: > 0 })
{
    var sent = await upgrade.UpgradeServiceAsync(
        new UpgradeKfServiceRequest
        {
            OpenKfId = openKfId,
            ExternalUserId = externalUserId,
            Type = 1,                                     // 1 专员服务 / 2 客户群服务
            Member = new KfUpgradeMember
            {
                UserId = candidate,                       // 假值；还须在「客户联系」使用范围内
                Wording = "专属顾问将为您服务（示例推荐语）",
            },
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(sent, "/cgi-bin/kf/customer/upgrade_service");
    // 应答只有 errcode/errmsg：发起成功不代表客户已添加专员
}
else
{
    // 没有可用专员时改推客户群：type=2 必填 groupchat.chat_id，且群主须在应用可见范围内
    string? chatId = range.GroupchatRange?.ChatIdList?.FirstOrDefault();
    if (chatId is { Length: > 0 })
    {
        var groupSent = await upgrade.UpgradeServiceAsync(
            new UpgradeKfServiceRequest
            {
                OpenKfId = openKfId,
                ExternalUserId = externalUserId,
                Type = 2,
                Groupchat = new KfUpgradeGroupchat { ChatId = chatId, Wording = "示例群推荐语" },
            },
            cancellationToken);
        WechatWorkException.ThrowIfFailed(groupSent, "/cgi-bin/kf/customer/upgrade_service");
    }
}

// ③ 取消：按「客服账号 × 客户」维度，不区分此前是专员还是客户群
var cancelled = await upgrade.CancelUpgradeServiceAsync(
    new CancelKfUpgradeServiceRequest { OpenKfId = openKfId, ExternalUserId = externalUserId }, cancellationToken);
WechatWorkException.ThrowIfFailed(cancelled, "/cgi-bin/kf/customer/cancel_upgrade_service");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `department_id_list` vs `department_list` | **官方参数表写 `department_list`，官方 JSON 示例与自建文档写 `department_id_list`** ⇒ 本仓 `[JsonPropertyName]` 恒为 `department_id_list`（KF1h 锁定，对齐全仓 `cusor`→`cursor` 处置先例） | 按参数表改注解 → 反序列化取不到部门范围 |
| E1 的动词与入参 | 官方即 **GET 且完全无请求参数**（连 `open_kfid` 都没有）⇒ 无 Request DTO | 改 POST 或给它造请求体 → 打坏 KF1h |
| `type` 与两个块 | `type = 1` 才看 `member`（其中 `userid` 官方必填）、`type = 2` 才看 `groupchat`（其中 `chat_id` 官方必填）；**两块都带 `wording`** | 填错块 → 参数缺失或被忽略 |
| 95021 | 指定的 `userid` / `chat_id` **不在企业已配置的升级服务范围内** | 硬编码专员 id，配置一改就整批失败 ⇒ 应先 E1 读范围 |
| `userid` 的第二重范围 | 除升级服务范围外，**还须在「客户联系」使用范围内** | 只查了范围列表仍被拒 |
| 可见范围 | 三支均要求**接待人员**在可见范围、**`chat_id` 对应群主**在可见范围 | 群主不可见时取消/发起都失败 |
| 应答无结果 | E2 返回 `WechatWorkResponse`（无业务字段）⇒ **发起成功 ≠ 客户已加专员/入群**，结果面不在本域 | 当成落地状态记账 |
| E3 的粒度 | 取消只按 `(open_kfid, external_userid)`，**没有 `type`**，一次取消即脱离升级服务 | 以为要按专员/群分别取消 |
| 与统计族的指标对应 | 升级漏斗指标在 [KfStatistics.md](./KfStatistics.md)（`upgrade_service_*` 四格 + `upgrade_service_customer_cnt`），其「去重 / 计多次」口径不同 | 用 E2 次数当「升级客户数」 |
| 应用类型 | 三类等宽，但第三方 / 代开发须**先切企业作用域** | 未切 → `WechatTokenOwnerMismatchException` |
| 动词分布 | E1 GET、E2/E3 POST | 统一成 POST 或 GET |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验 userid/chat_id 是否在企业升级服务范围内**（越界由 95021 表达）、**不校验可见范围**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功仅代表发起被受理，不代表客户完成添加 |
| **95021** | 官方业务码（接口注释留档） | 升级接口指定的 `userid` / `chat_id` **未配置在「升级服务」范围内** | 先调 E1 取范围再选对象 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

无「配置『升级服务』」权限、`open_kfid` 非法、客户不在会话范围、群主不可见等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 升级结果的查询面 | 本域只给发起 / 取消，**没有「查询某客户升级状态」端点**；结果是否经回调事件下发亦未记录 ⇒ 未核验 | 官方 94674 + 回调事件清单 |
| `wording` 的长度与展示位置 | 注释只写「推荐语」，**未给长度上限** ⇒ 未逐项核验 | 官方 94674 参数表 |
| 同一客户重复发起的行为 | 是否覆盖上一次推荐、是否频控 ⇒ 注释未说明 | 官方 94674 |
| 取消后的可再发起时机 | 注释未给冷却口径 ⇒ 未核验 | 官方 94674 |
| 业务错误码全集 | 仅留档 95021 ⇒ 未逐项核验 | 官方各页 + 全局错误码页 |
| 端点频率限制 | 注释未给数值 ⇒ 未逐项核验 | 官方访问频率附录 90312 |
| 专员范围与客户联系使用范围的交集口径 | 两个范围同时要求，交集为空时的表现未记录 ⇒ 未核验 | 官方 94674 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§13 GET 无参与字段名冲突处置、§11 升级漏斗指标）
- 同域：[KfAccount.md](./KfAccount.md)（`open_kfid`）· [KfSession.md](./KfSession.md)（会话状态与消息，升级动作发生在会话语境中）· [KfStatistics.md](./KfStatistics.md)（`upgrade_service_*` 指标）· [KfCustomer.md](./KfCustomer.md)（`external_userid` 维度资料）
- 跨域：专员须在「客户联系」使用范围内 ⇒ 可见范围与客户联系配置见 [ContactRules.md](../Contacts/ContactRules.md) / [FollowUser.md](../ExternalContact/FollowUser.md)；`chat_id` 对应客户群详情见 [GroupChat.md](../ExternalContact/GroupChat.md)
- 官方页：[升级「升级服务」配置（自建 94674）](https://developer.work.weixin.qq.com/document/path/94674) · [第三方 94702](https://developer.work.weixin.qq.com/document/path/94702) · [代开发 96422](https://developer.work.weixin.qq.com/document/path/96422)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
