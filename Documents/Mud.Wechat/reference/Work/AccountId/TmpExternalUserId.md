# tmp_external_userid 的转换（本域唯一对三类应用全开放的一族）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（版本唯一来源 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AccountId/TmpExternalUserId/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/TmpExternalUserId/) |
| DTO 源文件 | [ConvertTmpExternalUserIdRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/ConvertTmpExternalUserIdRequest.cs) · [ConvertTmpExternalUserIdResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/ConvertTmpExternalUserIdResponse.cs) · [TmpExternalUserIdResultItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/TmpExternalUserIdResultItem.cs) |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记）· [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1 归属域键 ↔ 族别） |
| 官方文档 | 企业自建应用 98729 · 第三方应用 98412 · 服务商代开发 98741（页名「tmp_external_userid的转换」）· 页面最后更新 **未本次取证**（官方正文继承本域 2026-10-18 逐端点核验留档，本次未重取） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释 + 域 README 逐项比对） |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与 AccountId 域全部示例合并逐字编译，0 错误（15 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 路由（`/cgi-bin/idconvert/convert_tmp_external_userid`，`PostAttribute`）+ ACCT2 层级断言（父接口恰 1、三子接口全空标记、继承链恰三支）+ ACCT3 令牌绑定 + ACCT4 上下文登记（`ConvertTmpExternalUserIdRequest` / `ConvertTmpExternalUserIdResponse` / `TmpExternalUserIdResultItem`）+ TO1 归属域键（自建 `Wechat.AccessToken@Internal`、第三方与代开发 `Wechat.AccessToken@Corp`）+ 四份接口文件与三个 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（§1 令牌路由键与归属键 / §4 批量上限 / §5 同应用性 / §6 枚举取值 / §9 袋名与失效袋名）· 官方：**本次未重取正文**，凡只有官方页能支撑的事实全部进 §7 并标未逐项核验 |

## 1. 这个接口族做什么

把应用从**其它业务接口**拿到的「外部用户临时 id」（`tmp_external_userid`）换成**正式 id**：目标是客户时换成 `external_userid`，目标是企业内成员时换成 `corpid` + `userid`。官方在会议、收集表、智能表（文档）这类场景里不直接给出正式 id，而是发一个临时 id，**必须先经本族兑换才能进入本方业务库主键**——这是本族存在的全部理由。

它是本域**唯一一支三类应用（自建 / 第三方 / 代开发）都开放**的端点：父接口承载唯一端点，三个应用类型子接口**全部是空标记**，只作类型化 DI 入口（ACCT2 锁定）。也正因如此，三类应用之间最容易写错的地方不在「有没有这个接口」，而在**令牌归属键**（见 §2.1 与 §5）。

约束是**同应用性**：调用本接口的应用，必须与**取得 `tmp_external_userid` 的那个应用是同一个**（父接口 remarks + 请求 DTO remarks 双重留档）。这条约束无法用类型系统表达，写错不会编译失败。

**不该用本族的场景**：手里已经是正式 `external_userid`（本族入参只接受临时 id）；要做的是服务商主体与企业主体之间的 external_userid 新旧换算，或把 external_userid 转成 `pending_id`（都在同域 ID 转换族，`access_token` 鉴权但端点不同）；把服务商主体 id 还原成企业主体 id（自建对接族）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` · `WechatAppType.ThirdParty` · `WechatAppType.Provider`（三类均可） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性（自建） | `CorpId` · `AgentSecret`（即 corpsecret） | 同上，缺任一项抛 `InvalidOperationException`（启动期即失败，不留到首次调用） |
| 必填配置属性（第三方 / 代开发） | `CorpId`（服务商企业）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，四项缺一即抛 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名**（ACCT3 + G5 白名单，`MUD005` 已接受风险） | 每次请求 |
| 令牌归属键（`TokenManagerKey`） | 自建子接口 `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`）；第三方与代开发子接口 `Wechat.AccessToken@Corp`（`CorpAccessToken`）——**三个子接口的归属键并不相同** | 在唯一咽喉点 `WechatAppContext.GetTokenManager` → `WechatTokenRouting.ResolveBaseTokenType` 校验，错配抛 `WechatTokenOwnerMismatchException`（fail-fast，**不静默取错令牌**） |

`access_token` 由 SDK 自动换取、缓存并按失效码自愈，**开发者不接触令牌值**。两条路径的差别只在「取哪一份令牌」：

- **自建应用**：令牌就是应用自己的 `corpid + corpsecret` 换来的那一份，无需任何作用域切换。
- **第三方 / 代开发应用**：`@Corp` 是**授权企业级令牌，一企一份**（scope = `authCorpId`），调用前必须用 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 把当前异步流切到目标授权企业作用域。`permanent_code` 是**凭据**——只以参数名出现在代码里，**不得进日志、遥测或异常消息**。

### 2.2 权限与业务前置

以下四条来自本仓三类证据（父接口 remarks + 请求 DTO 类级 remarks + 域 README §4/§5/§6）：

- 单次 `tmp_external_userid_list` **最多不超过 100 个**（请求 DTO 注释、父接口 `<param>` 注释、域 README §4 三处一致）。
- **同应用性**：调用此接口的应用，和获取到 `tmp_external_userid` 的应用**必须是同一个**（父接口 remarks、请求 DTO remarks）。换了应用（哪怕还是同一个企业的另一个应用）就拿不到转换结果。
- `user_type` 为 **1（客户）** 时，应用还需具有**「客户联系」权限**（父接口 remarks 原文）。
- `business_type` 必须与临时 id 的实际来源业务一致：`1` - 会议、`2` - 收集表、`3` - 智能表（文档）。临时 id 的签发接口在邻域（如 `Interfaces/Meeting/`、`Interfaces/DataZone/`），本域不消费也不核验其来源。

### 2.3 官方开放的应用类型

官方对**企业自建应用、第三方应用、服务商代开发**开放**完全一致**的这 1 个端点（父接口 summary 原文，三个应用类型各持一份官方页：98729 / 98412 / 98741）。因此本族的形态是「端点全在公共父接口 + 三子接口皆空标记」：

- ACCT2 `AssertAbstractParentWithEndpoints(tmpParent, 1, ...)` 锁定父接口声明面恰 1；
- ACCT2 对三个子接口逐一 `AssertRegistryChild(child, 0, "WechatWorkAccountIdTmpExternalUserIdService", ...)` 锁定**零差异端点**；
- ACCT2 `AssertDerivedInterfaces` 锁定继承链上**恰好**这三个子接口，不得多出、不得少一个。

所以：在子接口源文件里找不到任何方法是**预期形态**，不是缺失。方法在父接口上，注入的子接口决定的是「用哪份令牌」。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdTmpExternalUserIdService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载唯一端点 | 1（E1） | 否 |
| `IWechatWorkInternalAccountIdTmpExternalUserIdService` | `Mud.Wechat.Work` | 企业自建应用子接口 | 0（空标记，继承 E1），`TokenManagerKey = Wechat.AccessToken@Internal` | 是 |
| `IWechatWorkThirdPartyAccountIdTmpExternalUserIdService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1），`TokenManagerKey = Wechat.AccessToken@Corp` | 是 |
| `IWechatWorkProviderAccountIdTmpExternalUserIdService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1），`TokenManagerKey = Wechat.AccessToken@Corp` | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`（三子接口的 `InheritedFrom` 均为 `nameof(WechatWorkAccountIdTmpExternalUserIdService)`）
- 端点索引：**E1** `ConvertTmpExternalUserIdAsync`（临时外部用户 id → `external_userid` 或 `corpid` + `userid`）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI、`RegistryGroupName` 恒空，仅作契约基座）。

---

## 4. 端点

### E1. `ConvertTmpExternalUserIdAsync` — 临时外部用户 id 换正式 id

**签名**

```csharp
Task<ConvertTmpExternalUserIdResponse> ConvertTmpExternalUserIdAsync(
    [Body] ConvertTmpExternalUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/convert_tmp_external_userid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/idconvert/convert_tmp_external_userid?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建取 `@Internal`、第三方/代开发取企业级 `@Corp`） |
| 适用应用类型 | 企业自建应用 · 第三方应用 · 服务商代开发（三类全开放，本域唯一） |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 锁定三个类型登记；该上下文为 `internal`，示例不可直接引用） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.BusinessType` | `business_type` | `int?` | 是 | 业务类型：`1` - 会议、`2` - 收集表、`3` - 智能表（文档）。取值照抄请求 DTO 注释，**本仓以 `int?` + 注释承载、未建枚举**（域 README §6），其它取值全集未逐项核验（§7） |
| `request.UserType` | `user_type` | `int?` | 是 | 转换的目标用户类型：`1` - 客户、`2` - 企业互联、`3` - 上下游、`4` - 互联企业（圈子）（请求 DTO 注释）。`1` 时应用需具备「客户联系」权限（父接口 remarks）；`2`/`3`/`4` 决定响应里返回的是 `corpid` + `userid` 而非 `external_userid` |
| `request.TmpExternalUserIdList` | `tmp_external_userid_list` | `List<string>?` | 是 | 外部用户临时 id 列表，**最多不超过 100 个**（请求 DTO 注释 + 域 README §4）。该键带 `_list` 后缀，与本域 #5 的 `external_userid`（唯一不带 `_list` 的数组键）不同 |
| —（不进方法签名） | `access_token` | — | 是 | 接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`BusinessType`/`UserType` 留空、`TmpExternalUserIdList` 为 `null` 或超过 100 条都照样发出，错误以 `errcode` 返回。分批是调用方职责。

```json
{
  "business_type": 1,
  "user_type": 1,
  "tmp_external_userid_list": ["tmpExternalUserIdFAKE0001", "tmpExternalUserIdFAKE0002"]
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `results` | `Results` | `List<TmpExternalUserIdResultItem>?` | 转换成功列表。袋名是**复数 `results`**——本域批量响应里只有这一支用 `results`（`items`/`result` 属别的端点，域 README §9） |
| `results[].tmp_external_userid` | `TmpExternalUserId` | `string?` | 回显输入的临时 id |
| `results[].external_userid` | `ExternalUserId` | `string?` | `user_type = 1`（客户）时返回 |
| `results[].corpid` | `CorpId` | `string?` | `user_type = 2/3/4` 时返回（`TmpExternalUserIdResultItem` 注释原文） |
| `results[].userid` | `UserId` | `string?` | `user_type = 2/3/4` 时返回 |
| `invalid_tmp_external_userid_list` | `InvalidTmpExternalUserIdList` | `List<string>?` | 无法转换的临时 id 列表，`TmpExternalUserIdResultItem` 所在响应注释为「可能非法或没有权限」 |

`TmpExternalUserIdResultItem` 是**可空超集**：四类字段都声明为可空，按请求的 `user_type` 决定哪两个有值（DTO 类级注释原文「官方按 user_type 返回不同字段：可空超集覆盖全部用户类型」）。`external_userid` / `corpid` / `userid` 属**大小写敏感**的 id（域 README §9），宿主侧不得做 `ToLower()`/`ToUpper()` 归一化。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "results": [
    { "tmp_external_userid": "tmpExternalUserIdFAKE0001", "external_userid": "wmExternalIdFAKE0001" },
    { "tmp_external_userid": "tmpExternalUserIdFAKE0002", "external_userid": "wmExternalIdFAKE0002" }
  ],
  "invalid_tmp_external_userid_list": ["tmpExternalUserIdFAKE0003"]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                       // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;          // AddWechatApp（扩展方法落此命名空间，易漏）
using Mud.Wechat.Work.Abstractions.Enums;    // WechatAppType
using Mud.Wechat.Work.DataModels.AccountId;  // 请求/响应 DTO

// builder / config / provider / logger / cancellationToken 为宿主约定标识符。
// 宿主为 ASP.NET Core 时，System.Collections.Generic（List）、System.Linq（Take/ToList/Where/Select）、
// Microsoft.Extensions.DependencyInjection（GetRequiredService）与 Microsoft.Extensions.Logging
// （logger 的扩展方法）由 Web SDK 隐式 using 提供，故文档不写这几行。

// ① 注册自建应用：本族对三类应用都开放，自建形态只需 CorpId + AgentSecret 两项。
//    AgentSecret 即 corpsecret，只从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-meeting";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:CorpSecret"];
    // 同应用性：这里的 AppKey 必须与「拿到 tmp_external_userid 的那个应用」一致，
    // 多自建应用部署时换一个 AppKey 就是换了一个应用，令牌随之不同，结果拿不到。
    o.AgentId = config["Wechat:AgentId"];
});

// ② 注册业务模块（令牌基座随 AddWechatApp 装配）。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析自建子接口（父接口 IsAbstract = true，不可注入）。
IWechatWorkInternalAccountIdTmpExternalUserIdService tmpConvert =
    provider.GetRequiredService<IWechatWorkInternalAccountIdTmpExternalUserIdService>();

List<string> pendingTmpIds = new List<string> { "tmpExternalUserIdFAKE0001", "tmpExternalUserIdFAKE0002" };

// ④ 官方上限 100，SDK 不裁剪也不代分批：超限请求原样发出，错误以 errcode 回来。
List<string> tmpExternalUserIdBatch = pendingTmpIds.Take(100).ToList();

ConvertTmpExternalUserIdRequest convertRequest = new ConvertTmpExternalUserIdRequest
{
    BusinessType = 1,        // 1 - 会议（2 收集表 / 3 智能表，本仓未建枚举，取值以 DTO 注释为准）
    UserType = 1,            // 1 - 客户 ⇒ 应用必须具备「客户联系」权限；2/3/4 时响应改为 corpid + userid
    TmpExternalUserIdList = tmpExternalUserIdBatch,
};

ConvertTmpExternalUserIdResponse convertResponse =
    await tmpConvert.ConvertTmpExternalUserIdAsync(convertRequest, cancellationToken);

// ⑤ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!convertResponse.IsSuccess)
{
    logger.LogError("tmp_external_userid 转换失败 {ErrorCode}: {ErrorMessage}",
        convertResponse.ErrorCode, convertResponse.ErrorMessage);
}
else
{
    // errcode == 0 也可能是「整批一条都没换出来」：逐条对账，别只判 errcode。
    List<TmpExternalUserIdResultItem> converted =
        convertResponse.Results ?? new List<TmpExternalUserIdResultItem>();
    List<string> invalid =
        convertResponse.InvalidTmpExternalUserIdList ?? new List<string>();

    List<(string TmpExternalUserId, string ExternalUserId)> mapped = converted
        .Where(i => i.ExternalUserId != null)
        .Select(i => (i.TmpExternalUserId!, i.ExternalUserId!))
        .ToList();

    // invalid 里是「非法或没有权限」的临时 id：属敏感标识，不要把整批原值打进日志明文。
    logger.LogInformation("转换 {Mapped} 条、未转换 {Invalid} 条（user_type=1 场景请复核客户联系权限）",
        mapped.Count, invalid.Count);
}
```

**调用示例（第三方 / 代开发形态：先切到目标授权企业作用域）**

```csharp
using Mud.Wechat.Work;                              // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;                 // AddWechatApp
using Mud.Wechat.Work.Abstractions.Authentication;  // IWechatAppContextSwitcher（UseCorpScope 在此命名空间）
using Mud.Wechat.Work.Abstractions.Enums;           // WechatAppType
using Mud.Wechat.Work.DataModels.AccountId;         // 请求/响应 DTO

// System.Collections.Generic（List）、System.Linq（Take/ToList）、
// Microsoft.Extensions.DependencyInjection（GetRequiredService）与 Microsoft.Extensions.Logging
// 由 ASP.NET Core Web SDK 隐式 using 提供。

// 第三方 / 代开发形态：子接口归属键是 Wechat.AccessToken@Corp（企业级令牌，一企一份），
// 不切作用域就没有目标企业的 access_token 可取。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "thirdparty-suite";
    o.AppType = WechatAppType.ThirdParty;    // 代开发填 WechatAppType.Provider，注入 _Provider 子接口
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

IWechatWorkThirdPartyAccountIdTmpExternalUserIdService corpConvert =
    provider.GetRequiredService<IWechatWorkThirdPartyAccountIdTmpExternalUserIdService>();
IWechatAppContextSwitcher corpSwitcher = provider.GetRequiredService<IWechatAppContextSwitcher>();

string authCorpId = config["Wechat:AuthCorpId"];
string permanentCode = config["Wechat:PermanentCode"];  // 凭据：只以参数名传递，绝不进日志/遥测/异常消息

List<string> corpTmpExternalIds = new List<string> { "tmpExternalUserIdFAKE1001" };

using (corpSwitcher.UseCorpScope("thirdparty-suite", authCorpId, permanentCode))
{
    ConvertTmpExternalUserIdResponse corpResponse = await corpConvert.ConvertTmpExternalUserIdAsync(
        new ConvertTmpExternalUserIdRequest
        {
            BusinessType = 2,                 // 2 - 收集表
            UserType = 1,
            TmpExternalUserIdList = corpTmpExternalIds.Take(100).ToList(),
        },
        cancellationToken);

    if (!corpResponse.IsSuccess)
    {
        logger.LogError("企业作用域内 tmp_external_userid 转换失败 {ErrorCode}: {ErrorMessage}",
            corpResponse.ErrorCode, corpResponse.ErrorMessage);
    }
    else
    {
        List<TmpExternalUserIdResultItem> corpItems =
            corpResponse.Results ?? new List<TmpExternalUserIdResultItem>();
        logger.LogInformation("在授权企业作用域内转换 {Count} 条", corpItems.Count);
    }
}
// UseCorpScope 释放时逆序还原「企业 → 应用」两级上下文，不残留企业作用域。
```

**本端点注意事项**

- **同应用性优先于一切**：入参里的 `tmp_external_userid` 与调用所用的令牌必须出自同一个应用。多应用部署时这意味著「哪个 AppKey 拿到的临时 id，就用哪个 AppKey 兑换」；自建形态换 `o.AppKey` 注册、第三方形态在 `UseCorpScope` 里换第一个参数，都等价于换了调用方。不满足时的具体失败形态（`errcode` 还是整批落 `invalid_tmp_external_userid_list`）内部证据未留档，见 §7。
- `user_type` 决定读哪两个字段：`1` 读 `ExternalUserId`；`2`/`3`/`4` 读 `CorpId` + `UserId`。`TmpExternalUserIdResultItem` 是可空超集，按字段名猜着读会拿到 `null`。
- `user_type = 1` 且应用无「客户联系」权限时是**权限型失败**，与「临时 id 非法」在响应里可能同形（都表现为无法转换），排错时先查权限再查 id（权限要求出处：父接口 remarks）。
- 单次上限 100 条（请求 DTO 注释）。SDK 侧不校验、不代分批；需要处理大批量时由调用方自行切片，`Take`/`Skip` 或按业务分页都可，但不要指望官方帮你截断。
- 本端点无 `corpid`/`agentid` 请求字段——企业身份由**令牌作用域**表达（域 README §3）。这与同域 provider 族（显式传明文 `corpid`）是两套定位方式，不要互相借参数。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 三个子接口的 `TokenManagerKey` | 自建 = `Wechat.AccessToken@Internal`；第三方与代开发 = `Wechat.AccessToken@Corp`（TO1 + 接口 `[Token]` 声明）。**方法完全相同，只差这一处** | 用第三方子接口去打一个配置为 `Internal` 的应用（或反之），`WechatAppContext.GetTokenManager` 在归属校验处抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）；这不是「重试能解决」的错，是应用类型与注入接口不匹配 |
| 第三方 / 代开发忘了切企业作用域 | `@Corp` 是企业级令牌，一企一份、scope = `authCorpId`，须经 `UseCorpScope(appKey, authCorpId, permanentCode)` 建立 | 取不到目标企业的令牌、调用失败；且这类错误在多应用部署里最容易「本地单应用跑得通、线上拿错企业的令牌」 |
| 三个子接口皆空标记 vs 「子接口没有方法 = 没实现」 | ACCT2 断言父接口恰 1、三子接口声明面恰 0（端点在父接口上，`InheritedFrom` 指向生成的父实现类） | 去子接口文件里找方法会以为接口残缺，进而自行在子接口上重复声明方法——那会打红 ACCT2 的 `DeclaredOnly` 计数与生成器实现面 |
| 响应袋名 `results`（复数） | 本域批量响应袋名三套写法：`items` / `result` / **`results`（仅本端点）**（域 README §9） | 复用同一份「批量转换响应」抽象 ⇒ 反序列化拿到空列表，且 `errcode` 仍是 0 |
| 只判 `errcode == 0` 就认为全部成功 | 本端点是**部分成功**语义：`results` 与 `invalid_tmp_external_userid_list` 可同时非空，也可 `errcode = 0` 而 `results` 为空 | 缺量的临时 id 被静默丢弃，下游把「未兑换」当「客户不存在」 |
| `tmp_external_userid` 与正式 `external_userid` | 前者只在会议/收集表/智能表等场景签发、需经本端点兑换；后者是正式外部联系人 id | 把正式 `external_userid` 塞进 `tmp_external_userid_list` 不会报「类型错」，只会进 `invalid` 列表（域 README 未留档该形态，见 §7） |
| 与 ID 转换族的 external_userid 端点 | 同域、同为 `access_token`、路由都在 `/cgi-bin/` 下，但端点不同：本族路由 `idconvert/convert_tmp_external_userid`，ID 转换族为 `externalcontact/get_new_external_userid` 等（域 README 速览 #2/#3/#5） | 两条路由极易复制错：拿错端点即参数与语义双双不匹配（详见 [IdConvert.md](./IdConvert.md)） |

另外三条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改用 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但你自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`（`Mud.Wechat.Work.Abstractions.Exceptions`）。
- **配置互斥必填按应用类型不同**：`WechatAppConfig.Validate()` 自建要求 `CorpId` + `AgentSecret`，第三方/代开发要求 `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` 四项齐备；`AppKey` 形状另由 `WechatAppKeyValidator` 校验（含 `:` 会造成令牌键别名 ⇒ 跨应用令牌串号）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（**不代表整批都兑换成功**，见 §5） | 正常路径 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 族失效/过期 | 本端点的**族别**失效码，走「失效缓存 → 强制刷新 → 重注入重试」自愈 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本端点不消费该令牌；出现即凭据族别错配的信号 |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效 | 同上——本族是 `access_token` 族（与同域 provider 族相对） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷 |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`，判定值 `{40014, 42001, 42007, 42009, 42011}`），业务侧通常看不到这些码。**若 40014/42001 稳定出现**，说明刷新本身在失败：自建形态回查 `CorpId` + `AgentSecret`；第三方/代开发形态回查作用域是否已用 `UseCorpScope` 建立、`permanent_code` 是否为该授权企业的当前有效值（本域不核验授权编排，见 `IWechatWorkAuthorizationService` 与 Authentication 域）。

本端点专属的业务错误码（例如「应用无客户联系权限」「临时 id 非法/过期」对应哪些码）内部证据未留档，见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（文档号 90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

域 README §5 另留档一条与本族相邻的配额事实：三档业务频率与 `mass_call_ticket` 豁免只写在 ID 转换族的 #4/#5/#9 上，**未留档适用于本端点**，不要借用那些数值给本端点设限。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 违反同应用性时的失败形态 | 父接口与 DTO 只写「必须是同一个」，未留档不满足时是报 `errcode`、还是整批落 `invalid_tmp_external_userid_list` ⇒ **未逐项核验**，本文只写约束、不写表现 | 官方 98729/98412/98741 页的返回说明；实测需真实凭据 |
| `tmp_external_userid` 的有效期与一次性语义 | 临时 id 是否过期、兑换一次后能否重复兑换，本域三类证据均未记载 ⇒ **未逐项核验**。不得按「永久可用」或「一次性」任一前提写代码 | 官方页正文 + 签发侧（会议 / 收集表 / 智能表）文档；本域 `Interfaces/AccountId/` 无此信息 |
| 本端点的频率上限 | 接口注释与 DTO 均未留档 QPS/日配额；域 README 缺口表亦记「除 #4/#5 外各端点频率数值未逐项核验」 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 超过 100 条时的官方行为 | 官方只写「最多不超过 100 个」，未说明是整批拒绝还是截断/部分处理 ⇒ **未逐项核验** | 官方 98729/98412/98741 页；同域 #5/#11 的上限表现可作对照参考但不等价 |
| `business_type` / `user_type` 的其它取值 | 域 README §6 只列注释留档值（`business_type` 1/2/3、`user_type` 1/2/3/4），明确标注「不得据本文推断『只有这些值』」 ⇒ 全集**未逐项核验** | 官方 98729 页参数表 |
| `user_type` 2/3/4 的权限前置 | 本域只留档「`user_type = 1` 需客户联系权限」；企业互联 / 上下游 / 互联企业（圈子）三种目标各需要什么权限或配置，属邻域 ⇒ **未在本域核验** | 上下游域 `Interfaces/CorpGroup/` 与其守卫；企业互联相关端点在 CorpGroup 域 |
| 本端点专属错误码 | 接口注释未留档错误码表；域 README「各端点错误码集合」同记为未逐项核验 ⇒ 本文 §6 只给通用失效码族 | 官方全局错误码（90455），不在 SDK 侧硬编码码表 |
| 传正式 `external_userid` 进临时 id 列表的表现 | 域 README §9 对 #1（`open_userid_list` 传明文 userid）留档了「不报错、整批落 invalid」这一反直觉点，**本端点无同类留档** ⇒ **未逐项核验** | 官方 98729 页返回说明；实测需真实凭据 |
| 三类应用官方页是否逐字一致 | 父接口 summary 写「官方对三类应用开放完全一致的 1 个端点」，本文按该内部证据成文；三页正文本次未分别取文 ⇒ 具体字段文案差异**未逐项核验** | 官方 98729 / 98412 / 98741 三页对照 |
| 官方页正文本次未重取 | 本文所有官方口径均继承本域 2026-10-18 的逐端点核验留档（域 README），**本次未重取官方页正文**；官方若改版则本文可能滞后 | 重新渲染取证三页，并重置本文核验日期 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 契约陷阱汇总，本域权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（§1 三种令牌路由键与归属键 / §4 批量上限一览 / §5 同应用性 / §6 枚举实际取值 / §9 袋名不统一）
- 兄弟族：[Migration.md](./Migration.md)（同域内的 provider 族对照：显式传明文 `corpid`、不需要企业作用域）· [Bot.md](./Bot.md)
- 兄弟族：[IdConvert.md](./IdConvert.md)（external_userid 的新旧换算与 pending_id 查询）· [Interop.md](./Interop.md)（自建应用反向换算）· [Corpid.md](./Corpid.md) · [ChatIdUpgrade.md](./ChatIdUpgrade.md)
- 本族源码：[Interfaces/AccountId/TmpExternalUserId/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/TmpExternalUserId/)
- 归属域键与错配防线：[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· [WechatTokenRouting.cs](../../../../../Src/Work/Mud.Wechat.Work/WechatTokenRouting.cs)（`OwnedKeys` 单一来源）· 作用域切换契约 [IWechatAppContextSwitcher.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/Interfaces/IWechatAppContextSwitcher.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（未成文）
- 官方页：[tmp_external_userid的转换（自建应用 98729）](https://developer.work.weixin.qq.com/document/path/98729) · [（第三方应用 98412）](https://developer.work.weixin.qq.com/document/path/98412) · [（服务商代开发 98741）](https://developer.work.weixin.qq.com/document/path/98741) · [全局错误码（90455）](https://developer.work.weixin.qq.com/document/path/90455)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
