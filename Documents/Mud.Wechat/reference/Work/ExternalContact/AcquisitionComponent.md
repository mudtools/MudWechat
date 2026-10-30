# 客户联系 · 获客助手组件（服务商代支付）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 获客助手组件族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactAcquisitionComponentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/AcquisitionComponent/IWechatWorkExternalContactAcquisitionComponentService.cs)（零端点父接口）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/AcquisitionComponent/IWechatWorkExternalContactAcquisitionComponentService_ThirdParty.cs)（2 支组件专属端点）；[IWechatWorkExternalContactAcquisitionComponentBillService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/AcquisitionComponent/IWechatWorkExternalContactAcquisitionComponentBillService.cs)（零端点父接口）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/AcquisitionComponent/IWechatWorkExternalContactAcquisitionComponentBillService_ThirdParty.cs)（1 支流水端点） |
| DTO 源文件 | [DataModels/ExternalContact/AcquisitionComponent/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/AcquisitionComponent/)（`SerializerClassName = AcquisitionComponent`） |
| 契约守卫 | [WechatExternalContactAcquisitionComponentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactAcquisitionComponentContractGuards.cs)（**AC0 反重复守卫** / AC1 路由表 / AC2 层级与父接口零端点 / AC3 令牌分裂 / AC4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 组件授权信息 99610 · 生成代支付 key 99603 · 代支付流水 99602（**均仅第三方应用树**） |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：AC0~AC4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.14、§4 陷阱 12）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

**服务商侧**的获客助手组件能力——企业与服务商之间的「**谁付钱、怎么授权、代付流水**」：

| 端点 | 用途 | 令牌 |
|---|---|---|
| E1 `customer_acquisition/get_comp_auth_info` | 取企业获客助手组件的授权信息（付费模式、代付单价、链接授权模式、授权应用列表） | `access_token`（企业级） |
| E2 `customer_acquisition/create_once_key` | 为某条获客链接生成一次性代支付 key | `access_token`（企业级） |
| E3 `service/customer_acquisition/get_bill_list` | 服务商取**代支付流水** | **`suite_access_token`（套件级）** |

本族有两个「反直觉」的结构，都是守卫钉死的：

1. **一族两接口族、两套令牌路由键**：E3 的鉴权是 `WechatTokenTypes.SuiteAccessToken`（Query `suite_access_token`、**无企业 scope、不切 corp 作用域**），与 E1/E2 的 `Wechat.AccessToken` 不同 ⇒ 按「一接口族一令牌路由键」拆成 `AcquisitionComponentService` 与 `AcquisitionComponentBillService` 两个接口族（AC3 断言令牌按 suite/access 分裂）。
2. **AC0 反重复守卫**：获客链接列表 / 详情 / 使用统计 / 成员收消息详情这 4 条端点，官方以「组件版」与「直连版」两套文档承载**同一路由**，且组件版返回字段是直连版的**可空真子集** ⇒ 按 ADR-14「一份可空超集载荷覆盖多模式」原则，这 4 条**只在获客助手域声明一次**，本域不再重复声明（见 [CustomerAcquisition.md](./CustomerAcquisition.md)）。**不要**因为「组件文档页存在」就在本域补端点。

官方仅向**第三方应用**开放本域：两个父接口都是**零端点抽象**，端点全部收敛在第三方子接口，**不设自建/代开发子接口**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| E1/E2 令牌 | `WechatTokenTypes.AccessToken` → Query `access_token`，归属键 `Wechat.AccessToken@Corp`（授权企业级，scope = authCorpId） | 运行期归属校验 |
| E3 令牌 | `WechatTokenTypes.SuiteAccessToken` → Query `suite_access_token`（**套件级、一套件一份、不带企业 scope**） | 套件令牌链路 |

```csharp
// E1/E2：先切到目标授权企业作用域（permanent_code 是凭据，不得进日志）
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var auth = await component.GetAcquisitionComponentAuthInfoAsync(ct);
}

// E3：套件级令牌，不切 corp 作用域；auth_corpid 作为业务参数传
var bill = await billApi.GetAcquisitionBillListAsync(
    new GetAcquisitionBillListRequest { BeginTime = begin, EndTime = end, AuthCorpid = authCorpId }, ct);
```

### 2.2 权限与业务前置

- E1 官方契约：**POST 且无请求体参数**（签名只剩取消令牌）。
- E2：**需企业已授权为「服务商代支付模式」（`pay_mode = 1`）方可调用**；`key_num` 默认 100、**最大 1000**；生成的 key **一次性使用、14 天内有效**，用法是把 `once_key` 参数**拼到 `comp_scene` 链接尾部**。
- E3：流水**起止间隔不可超过 31 天**；`begin_time` / `end_time` / `auth_corpid` **必填**；`limit` 默认 100、**最大 1000**。
- **E3 的时间对齐口径**（注释原文）：若要与企业侧使用量统计周期一致，须按「**当天 00:00:01 到次日 00:00:00**」的时间戳查询（不是常见的 0 点到 0 点）。
- E3 的稀疏字段：获客链接已删除时**不返回 `link_id`**；加好友 `state` 为空时**不返回 `state`**。
- 企业通过代付产生使用量后，**次日**才可在获客助手中看到代付订单记录 ⇒ 当日流水不完整。
- `suite_access_token` / `permanent_code` / `provider_access_token` 是凭据，只写参数名，**不得进日志、遥测或异常消息**；`auth_corpid`、`price` 属企业商务数据，示例用假值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 组件授权信息 | ❌ | ✅（99610） | ❌ |
| E2 生成代支付 key | ❌ | ✅（99603） | ❌ |
| E3 代支付流水 | ❌ | ✅（99602） | ❌ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 令牌路由键 | 可注入 DI |
|---|---|---|---|---|---|
| `IWechatWorkExternalContactAcquisitionComponentService` | `Mud.Wechat.Work.Interfaces` | 零端点父接口 | **0** | `Wechat.AccessToken` | 否 |
| `IWechatWorkThirdPartyExternalContactAcquisitionComponentService` | `Mud.Wechat.Work` | 第三方子接口 | **2** | `Wechat.AccessToken`（归属键 `@Corp`） | 是 |
| `IWechatWorkExternalContactAcquisitionComponentBillService` | `Mud.Wechat.Work.Interfaces` | 零端点父接口 | **0** | `Wechat.SuiteAccessToken` | 否 |
| `IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService` | `Mud.Wechat.Work` | 第三方子接口 | **1** | `Wechat.SuiteAccessToken` | 是 |

注册组统一为 `ExternalContact`（`AddExternalContactApi()`）；两支第三方子接口分别 `InheritedFrom = nameof(WechatWorkExternalContactAcquisitionComponentService)` 与 `nameof(WechatWorkExternalContactAcquisitionComponentBillService)`。

## 4. 端点

**3 支全部 POST**（含 E3 这个查询语义端点）。

### E1. `GetAcquisitionComponentAuthInfoAsync` — 获取组件授权信息

```csharp
Task<GetAcquisitionComponentAuthInfoResponse> GetAcquisitionComponentAuthInfoAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info` |
| 请求 | **无请求体**（官方契约如此） |
| 令牌 | Query `access_token`（授权企业级） |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `pay_mode` | `PayMode` | `int?` | **0 扣企业费用 / 1 扣服务商费用（即服务商代支付模式）** |
| `price` | `Price` | `long?` | 获客链接代付单价，**单位分**；**仅 `pay_mode = 1` 时返回** |
| `link_auth_mode` | `LinkAuthMode` | `int?` | **0 企业选择链接授权 / 1 关联应用创建的链接自动授权** |
| `auth_apps[]` | `AuthApps` | `List<AcquisitionComponentAuthApp>?` | 授权应用列表，元素仅 `agentid`（`int?`） |

### E2. `CreateAcquisitionComponentOnceKeyAsync` — 生成代支付 key

```csharp
Task<CreateAcquisitionComponentOnceKeyResponse> CreateAcquisitionComponentOnceKeyAsync(
    [Body] CreateAcquisitionComponentOnceKeyRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/create_once_key`。
- 请求：`link_id`（必填）、`key_num`（`int?`，默认 100、**最大 1000**）。
- 响应：`keys`（`List<string>?`）——**一次性使用、14 天内有效**。
- **前置**：企业须已授权为 `pay_mode = 1`（先调 E1 判定）；用法是把 `once_key` 拼到 `comp_scene` 链接尾部。

### E3. `GetAcquisitionBillListAsync` — 获取代支付流水

```csharp
Task<GetAcquisitionBillListResponse> GetAcquisitionBillListAsync(
    [Body] GetAcquisitionBillListRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/service/customer_acquisition/get_bill_list`（**路由挂 `/cgi-bin/service/` 前缀**，不在 `externalcontact/` 下） |
| 令牌 | **Query `suite_access_token`**（套件级，无企业 scope） |

**请求参数**（`GetAcquisitionBillListRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `begin_time` | `BeginTime` | `long?` | 是 | Unix 秒；与 `end_time` 间隔 **≤31 天** |
| `end_time` | `EndTime` | `long?` | 是 | 同上；与企业侧口径对齐须用「当日 00:00:01 → 次日 00:00:00」 |
| `auth_corpid` | `AuthCorpid` | `string?` | 是 | **授权企业 corpid**（作为业务参数传，不靠令牌 scope 表达） |
| `cursor` | `Cursor` | `string?` | 否 | 首次不填 |
| `limit` | `Limit` | `int?` | 否 | 默认 100、**最大 1000** |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `bill_list[]` | `BillList` | `List<AcquisitionBillRecord>?` | 代支付流水 |
| `bill_list[].timestamp` | `Timestamp` | `long?` | 消耗时间（Unix 秒） |
| `bill_list[].link_id` | `LinkId` | `string?` | 获客链接 id；**链接已删除时不返回** |
| `bill_list[].state` | `State` | `string?` | 加好友时的 state；**为空时不返回** |
| `bill_list[].price` | `Price` | `long?` | **服务商代付金额，单位分** |
| `next_cursor` | `NextCursor` | `string?` | 已无更多数据时**不返回该字段** |

**调用示例**（本族主示例：判付费模式 → 生成 key → 拉代付流水）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;   // IWechatAppContextSwitcher
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "third-party-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];     // 凭据：只从配置读
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var component = provider.GetRequiredService<IWechatWorkThirdPartyExternalContactAcquisitionComponentService>();
var bills = provider.GetRequiredService<IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
string authCorpId = config["Wechat:AuthCorpId"]!;
string permanentCode = config["Wechat:PermanentCode"]!;

// ① E1：企业级令牌，先切作用域；无请求体
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var auth = await component.GetAcquisitionComponentAuthInfoAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(auth, "/cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info");

    // pay_mode = 1 才是服务商代支付，才可申请 key
    if (auth.PayMode == 1)
    {
        var keyResp = await component.CreateAcquisitionComponentOnceKeyAsync(
            new CreateAcquisitionComponentOnceKeyRequest { LinkId = "<link_id>", KeyNum = 100 },
            cancellationToken);
        WechatWorkException.ThrowIfFailed(keyResp, "/cgi-bin/externalcontact/customer_acquisition/create_once_key");
        // keys 一次性、14 天有效；拼到 comp_scene 链接尾部使用，不进日志。
        logger.LogInformation("已生成代支付 key {Count} 个", keyResp.Keys?.Count ?? 0);
    }
}

// ② E3：套件级令牌（不切 corp 作用域），企业以 auth_corpid 作为业务参数
// 时间对齐企业侧口径：当日 00:00:01 → 次日 00:00:00，间隔 ≤31 天
var dayStart = new DateTime(2026, 10, 28, 0, 0, 1, DateTimeKind.Utc);
long begin = new DateTimeOffset(dayStart).ToUnixTimeSeconds();
long end = new DateTimeOffset(dayStart.AddDays(1)).ToUnixTimeSeconds();

string? cursor = null;
long totalFen = 0;
do
{
    var page = await bills.GetAcquisitionBillListAsync(
        new GetAcquisitionBillListRequest
        {
            BeginTime = begin,
            EndTime = end,
            AuthCorpid = authCorpId,
            Cursor = cursor,
            Limit = 1000,
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/service/customer_acquisition/get_bill_list");

    foreach (AcquisitionBillRecord rec in page.BillList ?? new List<AcquisitionBillRecord>())
    {
        totalFen += rec.Price ?? 0;   // 单位：分
    }
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

logger.LogInformation("服务商代付合计 {Yuan} 元", totalFen / 100m);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1/E2 的 `access_token` vs E3 的 `suite_access_token` | **两个令牌路由键、两个接口族**（E3 无企业 scope） | 给 E3 切 corp 作用域 / 给 E1 用套件令牌 → 鉴权失败 |
| `auth_corpid`（E3 业务参数）vs `UseCorpScope` 的企业上下文 | 套件级接口用参数表达企业 | 以为切了作用域就能省略 `auth_corpid` → 必填校验失败 |
| E3 的路由前缀 `/cgi-bin/service/` | 与其余 `externalcontact/` 前缀不同，但**仍属本域** | 按前缀把 E3 归到服务商认证域 → 找不到端点 |
| 组件版 vs 直连版的 4 条同路由端点 | 只在获客助手域声明一次（AC0 反重复）；组件版字段是直连版可空真子集 | 在本域补 `list_link`/`statistic` → 打红 AC0 且造成双份契约 |
| `pay_mode` 0 vs 1 | 0 扣企业、1 扣服务商；**只有 1 才能申请 key** | 未判定就调 E2 → 失败 |
| `link_auth_mode` 0 vs 1 | 0 需企业选择链接授权、1 关联应用创建的链接自动授权 | 以为链接天然可用 → 拿不到授权应用范围 |
| `price` 的单位 | E1 单价与 E3 金额都是**分** | 当元汇总 → 账单差 100 倍 |
| once_key 用法与时效 | 一次性、14 天有效，拼到 `comp_scene` 链接尾部 | 复用同一个 key / 存进日志 |
| E3 的时间窗口 | 间隔 ≤31 天，且与企业侧口径对齐须用 `00:00:01 → 次日 00:00:00` | 用 0 点到 0 点 → 与企微后台数字不一致 |
| `bill_list` 的稀疏字段 | 链接已删不返回 `link_id`；state 空不返回 `state` | 当必然存在来 join → 漏记录 |
| 代付可见时点 | 产生用量后**次日**才可在获客助手看到订单 | 当日对账以为缺数据 |
| 父接口 | 两支都是零端点抽象，不注册 DI | 注入父接口 → 无方法可调 |

SDK 侧通用约束：**不自动判错**、**不做本地计费换算与时间对齐**；`access_token` / `suite_access_token` 走 Query 都是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 成功仍要按 `next_cursor` 续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 42009 | `InvalidSuiteAccessToken` | 套件令牌失效 | **E3 专属**：自愈集合成员，按套件令牌链路恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本族接口注释**未留档业务错误码**（未开通代支付模式、key 数量超限、时间窗口超限、企业未授权组件）⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码 | 注释未列码值 ⇒ 未逐项核验 | 官方 99602 / 99603 / 99610 |
| 各端点频率 | 注释未给 QPS ⇒ 未逐项核验 | 官方 + 全局频率附录 |
| `comp_scene` 链接的构造方式 | 注释只说「把 `once_key` 拼到 `comp_scene` 链接尾部」，未给链接模板与参数全集 | 官方 99603 与组件接入说明 |
| key 的核销/作废机制 | 注释只给「一次性、14 天有效」，未说明未用完的 key 如何处理 | 官方 99603 |
| 代付单价的变更口径 | E1 返回 `price`，但单价调整时机与生效范围未留档 | 官方服务商侧说明 |
| 组件版与直连版的字段差异清单 | AC0 只断言不重复声明；「可空真子集」的具体字段未逐项列举 | 官方组件版文档页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.14 端点表、§4 陷阱 12「令牌例外——获客组件账单」）
- 同域：[CustomerAcquisition.md](./CustomerAcquisition.md)（获客链接与统计的**单一所有者**，4 条同路由端点在那里）· [FollowUser.md](./FollowUser.md)（可建联成员决定链接可用范围）
- 令牌链路：`suite_access_token` 的签发与套件票据见 `Interfaces/Authentication/`（[ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)）
- 官方页：[获取组件授权信息（99610）](https://developer.work.weixin.qq.com/document/path/99610) · [生成代支付 key（99603）](https://developer.work.weixin.qq.com/document/path/99603) · [获取代支付流水（99602）](https://developer.work.weixin.qq.com/document/path/99602)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
