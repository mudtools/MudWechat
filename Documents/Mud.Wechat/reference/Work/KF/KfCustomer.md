# 微信客服 · 客户基础信息

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 客户基础信息族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfCustomerService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfCustomerService.cs)（父，承载唯一 1 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfCustomerService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfCustomerService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfCustomerService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（`BatchGetKfCustomerInfoRequest` / `BatchGetKfCustomerInfoResponse` / `KfCustomerInfo` / `KfEnterSessionContext` / `KfWechatChannelsInfo`，命名空间 `Mud.Wechat.Work.DataModels.Kf`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1d** 路由与 `batchget` 连写 / KF2 三类子接口零端点空标记 / KF3 令牌绑定 / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [95159](https://developer.work.weixin.qq.com/document/path/95159) · 第三方 [95149](https://developer.work.weixin.qq.com/document/path/95149) · 代开发 [96429](https://developer.work.weixin.qq.com/document/path/96429) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1d / KF2 / KF3 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§10 与 external contact 的边界 + 48 小时窗口、§14 频率口径）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

单支端点：**批量获取微信客服会话内客户的基础信息**（昵称 / 头像 / unionid + 最后一次进入会话的上下文）。

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/kf/customer/batchget` | 按 `external_userid` 列表（1~100 个）批量取客户信息，不合规的 id 落入 `invalid_external_userid` |

形态：官方对**三类应用开放完全一致的 1 支** ⇒ 收敛父接口 + 三个零差异空标记子接口（KF2）。路由 **`batchget` 是官方原文连写**，不是 `batch_get`（KF1d 逐字锁定）。

**与客户联系域的边界**：`external_userid` 是**同一 ID 空间、不同可见语义**——本端点只暴露客服会话维度所需的昵称 / 头像 / unionid 与进入会话上下文，客户联系域（[Customer.md](../ExternalContact/Customer.md)）给的是跟进人、标签、企业维度资料。

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

- 自建应用须配置到「**微信客服-可调用接口的应用**」中；第三方 / 代开发须具有「**微信客服 → 获取基础信息**」权限。
- **查询窗口**：对象须是**最近 48 小时内触发过「用户进入会话事件」或向该客服账号发过消息**的客户，否则落入响应的 `invalid_external_userid`（**不是报错**）。
- `external_userid_list` 官方必填、**可填 1~100 个，超出分批调用**。
- `need_enter_session_context`（`0`/`1`，默认 `0`）= `1` 时才返回 `enter_session_context`。
- **敏感字段的应用形态差异**：第三方 / 代开发**不可获取头像**、**性别统一返回 0**、**`unionid` 不可获取**（须自行获取 unionid 后经「Unionid 与 external_userid 关联接口」关联）；自建侧 `unionid` 须在管理后台**绑定微信开发者账号**（须同时绑定微信开放平台且主体一致）后才能获取。
- **「营销获客」应用仅能获取其自身带来的客户**。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 频率：官方三页正文**均未给出本端点的独立频率数字** ⇒ 按附录「访问频率限制」默认口径（每企业单 cgi/api ≤1 万次/分、15 万次/小时；第三方提供商每 IP ≤4 万次/分、120 万次/小时）。注释明确要求**不得凭第三方 SDK 注释或记忆补数**。
- 客户昵称、头像、unionid、进入会话场景均属**微信侧个人数据**：示例用假值，`customer_list` 不要整块落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅（绑定微信开发者账号后可取 unionid） | ✅（无头像、性别恒 0、无 unionid） | ✅（同第三方口径） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfCustomerService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **1** | 否 |
| `IWechatWorkInternalKfCustomerService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyKfCustomerService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderKfCustomerService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfCustomerService)`。

## 4. 端点

### E1. `BatchGetCustomerInfoAsync` — 批量获取客户基础信息

```csharp
[Post("/cgi-bin/kf/customer/batchget")]
Task<BatchGetKfCustomerInfoResponse> BatchGetCustomerInfoAsync(
    [Body] BatchGetKfCustomerInfoRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`BatchGetKfCustomerInfoRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `external_userid_list` | `ExternalUserIdList` | `List<string>?` | **是** | **1~100** 个，超出分批；须是 48 小时内进过会话或发过消息的客户 |
| `need_enter_session_context` | `NeedEnterSessionContext` | `int?` | 否 | `0` 不返回（默认）/ `1` 返回最后一次进入会话上下文 |

**响应参数**（`BatchGetKfCustomerInfoResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `customer_list[]` | `CustomerList` | `List<KfCustomerInfo>?` | 客户基础信息列表 |
| `customer_list[].external_userid` | `ExternalUserId` | `string?` | 微信客户的 external_userid |
| `customer_list[].nickname` | `Nickname` | `string?` | 微信昵称 |
| `customer_list[].avatar` | `Avatar` | `string?` | 微信头像 URL（**第三方 / 代开发不可获取**） |
| `customer_list[].gender` | `Gender` | `int?` | 性别（**第三方 / 代开发统一返回 0**） |
| `customer_list[].unionid` | `UnionId` | `string?` | 需绑定微信开发者账号；**第三方不可获取** |
| `customer_list[].enter_session_context` | `EnterSessionContext` | `KfEnterSessionContext?` | **仅 `need_enter_session_context = 1` 时返回**（48 小时内最后一次进入会话的上下文） |
| `enter_session_context.scene` | `Scene` | `string?` | 进入会话的场景值（取 E「获取客服账号链接」时自定义的 `scene`） |
| `enter_session_context.scene_param` | `SceneParam` | `string?` | 客服链接拼接的 `scene_param`，**原样返回** |
| `enter_session_context.wechat_channels` | `WechatChannels` | `KfWechatChannelsInfo?` | **仅客户从视频号进入会话时返回** |
| `wechat_channels.nickname` / `shop_nickname` | 同名 | `string?` | 视频号名称（场景值 1/2/3 返回）/ 视频号小店名称（场景值 4/5 返回） |
| `wechat_channels.scene` | `Scene` | `int?` | **1 视频号主页 / 2 直播间商品列表页 / 3 商品橱窗页 / 4 小店商品详情页 / 5 小店订单页** |
| `invalid_external_userid` | `InvalidExternalUserId` | `List<string>?` | **不满足 48 小时窗口**（或不可见）的客户 id |

**调用示例**（分批取客户信息 → 分流「拿到的人」与「不合格 id」→ 仅在需要时取进入会话上下文）

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

var customers = provider.GetRequiredService<IWechatWorkInternalKfCustomerService>();

// 待查客户须是最近 48 小时内进过会话或发过消息的（假值）
var externalUserIds = new List<string> { "wmCustomer0001", "wmCustomer0002" };
var invalid = new List<string>();
int fromChannels = 0;

// 官方单批上限 100，超出分批；SDK 不代裁剪
foreach (var batch in externalUserIds.Chunk(100))
{
    var page = await customers.BatchGetCustomerInfoAsync(
        new BatchGetKfCustomerInfoRequest
        {
            ExternalUserIdList = batch.ToList(),
            NeedEnterSessionContext = 1,      // 只要会话归因时才置 1，默认 0 不返回上下文
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/kf/customer/batchget");

    // 不满足 48 小时窗口的 id 进 invalid 列表，不影响本批其它结果
    invalid.AddRange(page.InvalidExternalUserId ?? new List<string>());

    foreach (KfCustomerInfo info in page.CustomerList ?? new List<KfCustomerInfo>())
    {
        KfEnterSessionContext? ctx = info.EnterSessionContext;
        // 第三方 / 代开发形态下：avatar 不返回、gender 恒 0、unionid 不可获取 ⇒ 一律判空
        bool hasUnionId = info.UnionId is { Length: > 0 };
        if (ctx?.WechatChannels is { } channels)
        {
            // 1 视频号主页 / 2 直播间商品列表 / 3 商品橱窗 / 4 小店商品详情 / 5 小店订单
            fromChannels++;
            logger.LogInformation("客户从视频号场景 {Scene} 进入会话（名称字段不外泄）", channels.Scene);
        }
        logger.LogInformation("客户 {ExternalUserId} 会话归因 scene={Scene}，是否取到 unionid {HasUnionId}",
            info.ExternalUserId, ctx?.Scene, hasUnionId);
    }
}

logger.LogInformation("批量查询完成：不在 48 小时窗口内的 id {Invalid} 个，视频号来源 {Channels} 个",
    invalid.Count, fromChannels);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `batchget` vs `batch_get` | 官方即**连写** `customer/batchget`（KF1d 逐字锁定） | 自创下划线写法 → 路由不识别 |
| 本族 vs 客户联系域 | **同一 `external_userid` 空间、不同可见语义**：本域只给会话维度信息，跟进人 / 标签 / 企业资料在 [Customer.md](../ExternalContact/Customer.md) | 拿本域当客户资料主源 |
| 48 小时窗口 | 不是「报错」而是**落入 `invalid_external_userid`**：查询窗口内无进入会话/发消息记录的客户 | 以为接口返回不全或批次失败 |
| 单批上限 | **1~100 个**，超出须分批；SDK 不裁剪 | 一批 500 个 → 请求被拒 |
| `need_enter_session_context` | 是**数字 0/1**（不是布尔），默认 0；置 1 才返回上下文块 | 当 `bool?` 传 → 反序列化失败 |
| `scene` 的两种类型 | `enter_session_context.scene` 是**字符串**（客服链接自定义值）；`wechat_channels.scene` 是**int**（1~5 视频号场景） | 两个 scene 混用同一变量 |
| `unionid` 的可得性 | 自建须**管理后台绑定微信开发者账号**（须同时绑微信开放平台且主体一致）；**第三方不可获取**，须自行关联（Unionid ↔ external_userid 接口，属账号 ID 域） | 按必有字段处理 |
| 头像与性别 | 第三方 / 代开发**无头像**、**性别恒 0** | 把 0 当「未知性别」的业务含义 |
| 「营销获客」应用 | **仅能获取自身带来的客户** | 全量拉取却只拿到一部分 |
| 频率数值 | 官方三页**均未给本端点独立数字** ⇒ 走附录默认阈值；注释明令**不得凭第三方 SDK 注释或记忆补数** | 编一个 QPS 写进配置 |
| 动词 | 读取语义却 **POST**（域 README §14 汇总点名的端点之一） | 改 GET → 打坏 KF1d 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不裁剪批次**、**不做窗口期校验**（48 小时由官方判定并回 `invalid_external_userid`）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | **成功时仍要读 `invalid_external_userid`**：部分客户按窗口规则被剔除 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`external_userid_list` 为空 / 超 100、无「获取基础信息」权限、营销获客应用越界取数等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未逐条摘录 ⇒ **未逐项核验** | 官方 95159 / 95149 / 96429 + 全局错误码页 |
| 端点独立频率 | 官方三页**无独立数字**，注释明确按附录默认口径 ⇒ 若官方补充数值需同批改注释与本文 | 官方访问频率附录 90312 |
| `invalid_external_userid` 的完整成因 | 注释写明 48 小时窗口，未说明「客户不在可见范围」「id 非法」是否同样落此列表 ⇒ **未逐项核验** | 官方 95159 响应说明 |
| 自建侧 unionid 的绑定校验面 | 「须绑定微信开发者账号（同时绑定开放平台且主体一致）」是管理后台动作，SDK 无探测端点 ⇒ 未核验 | 企业微信管理后台 + 官方绑定文档 |
| Unionid ↔ external_userid 关联接口归属 | 注释指路「Unionid 与 external_userid 关联接口」，该面**不在本域**（属账号 ID 域，见 [IdConvert.md](../AccountId/IdConvert.md)） | 账号 ID 域与其守卫 |
| 客户维度的会话历史拉取 | 本端点只给基础信息与最后一次进入会话上下文，**不含消息内容**（消息在 [KfSession.md](./KfSession.md) 的 `sync_msg`） | 本域两族文档对照 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§10 与 external contact 的边界 + 48 小时窗口、§14 频率与 POST 汇总）
- 同域：[KfAccount.md](./KfAccount.md)（`scene` / `scene_param` 的来源就是客服链接端点）· [KfSession.md](./KfSession.md)（消息内容与客户/接待人员定位）· [KfUpgrade.md](./KfUpgrade.md)（升级服务同样以 `external_userid` 定位客户）
- 跨域：[Customer.md](../ExternalContact/Customer.md)（企业维度的客户资料与跟进人）、[IdConvert.md](../AccountId/IdConvert.md)（unionid ↔ external_userid 关联面）
- 官方页：[批量获取客户基础信息（自建 95159）](https://developer.work.weixin.qq.com/document/path/95159) · [第三方 95149](https://developer.work.weixin.qq.com/document/path/95149) · [代开发 96429](https://developer.work.weixin.qq.com/document/path/96429)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
