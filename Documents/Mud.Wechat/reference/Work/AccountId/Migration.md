# ID 迁移完成状态的设置（迁移收口的单向闸门）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（版本唯一来源 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Migration/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Migration/) |
| DTO 源文件 | [FinishOpenIdMigrationRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/FinishOpenIdMigrationRequest.cs) · [FinishExternalUserIdMigrationRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/FinishExternalUserIdMigrationRequest.cs) · 响应为基类 [WechatWorkResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/WechatWorkResponse.cs)（两支均无业务字段） |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记）· [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1：非 AccessToken 族不得叠加归属域键） |
| 官方文档 | 第三方 99375 · 服务商代开发 99378（E2 亦在 99375 页）· 页面最后更新 **未本次取证**（官方正文继承本域 2026-10-18 逐端点核验留档，本次未重取） |
| 状态 | `待补充`（E1 的 `agentid` 必填性在内部证据里自相矛盾，正文该项无法定论，见 §5 与 §7） |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释 + 域 README 逐项比对） |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与 AccountId 域全部示例合并逐字编译，0 错误（15 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 路由表（`/cgi-bin/service/finish_openid_migration`、`/cgi-bin/service/externalcontact/finish_external_userid_migration`）+ ACCT2 层级断言 + ACCT3 令牌绑定 + ACCT4 上下文登记 + [Migration 三份接口文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Migration/) 与请求 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（§1 令牌路由键 / §3 定位键 / §6 枚举取值 / §8 一页多端点拆分，逐端点核验 2026-10-18）· 官方：**本次未重取正文**，参数表与业务限制一律以上述内部证据为限；只有官方页能支撑的事实全部进 §7 并标未逐项核验 |

## 1. 这个接口族做什么

这是服务商侧的**收口动作**：服务商（第三方应用或代开发应用）把某家授权企业的新旧 id 映射回收完之后，主动把该企业设置为「迁移完成」，此后接口与回调返回的就是升级后的 id。它不转换任何 id，只翻转官方侧的一个状态位。

两支端点都是**单向闸门，迁移完成不可回退**（父接口 summary 与请求 DTO remarks 双重留档）。因此本族的正确使用时机非常窄：只有在全量新旧 id 映射已经落库、且业务代码已全部改造为消费新 id 之后才允许调用；调用前 SDK 侧没有任何前置校验会替你拦住误操作。

官方把这两支端点写在同一个文档页（99375「ID迁移完成状态的设置」）里，本仓按「哪几种应用类型能用」拆到两处：`finish_openid_migration`（第三方 + 代开发共用）落公共父接口，`finish_external_userid_migration`（**仅第三方**）落第三方子接口。

调用凭证是**服务商级**的 `provider_access_token`，且**不需要企业作用域**——本族以请求体里的**明文 `corpid`** 定位目标企业（不是 `open_corpid`），与 `UseCorpScope` 无关。

**不该用本族的场景**：想知道当前回收状态（那是查询，走 ID 转换族的 `GetOpenIdMigrationAsync`，`/cgi-bin/corp/get_openid_migration`，用企业级 `access_token` 且无请求体，与设置端点分属两种令牌）；想转换 id 本身（ID 转换族 / corpid 转换族）；自建应用（官方对本族零开放，见 §2.3）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` 或 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动期即失败，不留到首次调用） |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken`（常量值 `Wechat.ProviderAccessToken`）→ 官方参数 `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |
| 令牌归属键 | **不声明** `TokenManagerKey`（TO1 明文：非 AccessToken 族本身已无歧义，叠加归属域键即违规） | — |
| 企业作用域 | **不需要**。本族以请求体明文 `corpid` 定位企业，与 `IWechatAppContextSwitcher.UseCorpScope` 无关（域 README §1） | — |

`provider_access_token` 由 SDK 侧的 `ProviderTokenManager` 以 `CorpId + ProviderSecret` 自动换取、缓存并刷新，**开发者不接触令牌值**。本族三支接口（父 + 两子）的 `[Token]` 由 ACCT3 逐一点名断言为 `ProviderAccessToken` + Query + 参数名 `provider_access_token`。

### 2.2 权限与业务前置

以下五条全部来自本仓三类证据（接口 XML 注释 + 请求 DTO 类级 remarks + 域 README §8），缺任一条都会造成**不可逆的错误收口**：

- 必须用 `provider_access_token` 调用（请求 DTO remarks）。
- **userid 与 corpid 只能同时设置为迁移完成，external_userid 可以单独设置**（父接口 summary；E2 的 remarks 再次留档同一口径）。落到参数上就是：`openid_type` 里的 `1` 是一个不可拆的原子单位。
- **设置迁移完成后，接口不再返回该企业相关的 unionid**（父接口 summary + E1 请求 DTO remarks）。依赖 unionid 的下游业务必须在收口之前改造完毕。
- **迁移完成不可回退**（父接口 summary + E1 请求 DTO remarks + 域 README §8）。域 README 的表述是「调用前必须已完成全量 id 回收」。
- **当该企业同时是服务商并对自己授权时，无需调用本接口**（父接口 summary、E1/E2 两支注释均留档）。E2 另加一条：该企业须**已授权该服务商的第三方应用**（E2 请求 DTO remarks）。

### 2.3 官方开放的应用类型

官方只对企业自建应用**零开放**本族，因此 ACCT2 的 `AssertDerivedInterfaces` 把迁移族的继承链钉成恰好两个子接口——**没有** `IWechatWorkInternalAccountIdMigrationService`，自建应用调不到、也不该去找它。

两支端点的开放面还不一样：E1 第三方 + 代开发共用（故落公共父接口）；E2 **仅第三方**（ACCT2 断言第三方子接口声明面恰 1、代开发子接口声明面恰 0 且为空标记）。因此「代开发应用要设置 external_userid 迁移完成」在本仓没有任何通向官方端点的路径。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdMigrationService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载 E1 | 1（E1） | 否 |
| `IWechatWorkThirdPartyAccountIdMigrationService` | `Mud.Wechat.Work` | 第三方应用子接口，另持 E2 差异端点 | 1（E2，继承 E1） | 是 |
| `IWechatWorkProviderAccountIdMigrationService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`（两子接口的 `InheritedFrom` 均为 `nameof(WechatWorkAccountIdMigrationService)`）
- 端点索引：**E1** `FinishOpenIdMigrationAsync`（设置 userid/corpid 的迁移完成状态）· **E2** `FinishExternalUserIdMigrationAsync`（设置 external_userid 的迁移完成状态，仅第三方）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI、`RegistryGroupName` 恒空，仅作契约基座）。代开发应用只能拿到 E1。

---

## 4. 端点

### E1. `FinishOpenIdMigrationAsync` — 设置企业 userid/corpid 的迁移完成状态

**签名**

```csharp
Task<WechatWorkResponse> FinishOpenIdMigrationAsync(
    [Body] FinishOpenIdMigrationRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/finish_openid_migration` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/finish_openid_migration?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发（自建应用官方未开放） |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 锁定 `FinishOpenIdMigrationRequest` 登记；该上下文为 `internal`，示例不可直接引用） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.CorpId` | `corpid` | `string?` | 是 | 目标授权企业的**明文 corpid**（provider 族以 corpid 定位企业，**不是** `open_corpid`，域 README §3）。SDK 侧不做非空校验 |
| `request.AgentId` | `agentid` | `int?` | 条件必填（代开发场景必填、第三方场景不传；注释另有「未传视为全量」口径，两说并存见 §7） | **代开发应用 id**。注释留档：第三方应用场景（99375）只传 `corpid` + `openid_type`；代开发场景（99378）另传 `agentid`；**仅传 `corpid` 不传 `agentid` 时，视为更新该企业下同服务商所有第三方应用的对应升级状态**（E1 请求 DTO 注释 + 父接口注释）。注意建模为 `int?` 而非 `string?`——本域作为**操作对象**的应用 id 一律 `int?`，作为**来源标注**的 `source_agentid` 才是 `string?`（域 README §3） |
| `request.OpenIdType` | `openid_type` | `List<int>?` | 是 | id 类型**数组**：`1` - userid 与 corpid；`3` - external_userid 及 external_tagid（照抄 DTO 注释）。与查询端点 `GetOpenIdMigrationAsync` 响应里的**同名单值**字段（`int?`，文案为 `1` - corpid/userid）形状不同、文案顺序也不同，均为照抄官方原文，**不得「对齐」成一份枚举**（域 README §6） |
| —（不进方法签名） | `provider_access_token` | — | 是 | 应用服务商的接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`OpenIdType` 为 `null`、空数组或含未留档取值都照样发出，错误以 `errcode` 返回；`CorpId` 空白同样原样交给官方。

```json
{
  "corpid": "wwCORPFAKE0001",
  "agentid": 1000002,
  "openid_type": [1]
}
```

**响应参数**

本端点**无业务响应字段**，返回类型就是统一基类 `WechatWorkResponse`（接口签名 `Task<WechatWorkResponse>`，两支端点均无派生响应 DTO；ACCT4 的迁移族注释段登记的三个类型是两支请求 DTO + `OpenIdMigrationInfoItem`，后者是**查询**端点 `GetOpenIdMigrationAsync` 的元素类型，归属 ID 转换族的路由）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| — | `IsSuccess` | `bool`（`[JsonIgnore]` 计算属性） | `ErrorCode == 0` 的便捷判定 |

```json
{
  "errcode": 0,
  "errmsg": "ok"
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                          // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;             // AddWechatApp（扩展方法落此命名空间，易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions;  // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels;               // WechatWorkResponse（本族两支端点均无业务响应字段）
using Mud.Wechat.Work.DataModels.AccountId;     // FinishOpenIdMigrationRequest

// builder / config / provider / logger / cancellationToken 为宿主约定标识符。
// 宿主为 ASP.NET Core 时，System（StringComparer）、System.Collections.Generic（List/HashSet）、
// Microsoft.Extensions.DependencyInjection（GetRequiredService）与 Microsoft.Extensions.Logging
// （logger 的扩展方法）由 Web SDK 隐式 using 提供，故文档不写这几行。

// ① 注册服务商应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
//    本族只消费 provider_access_token，但 Validate() 对第三方/代开发要求四项齐备，
//    缺 SuiteId 或 SuiteSecret 会启动期即抛 InvalidOperationException。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "thirdparty-main";
    o.AppType = WechatAppType.ThirdParty;          // 代开发改填 WechatAppType.Provider
    o.CorpId = config["Wechat:ProviderCorpId"];    // 服务商企业 CorpId，不是授权企业的
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块（令牌基座随 AddWechatApp 装配，本族无需另注册令牌链）。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析子接口（父接口 IsAbstract = true，不可注入）。
IWechatWorkThirdPartyAccountIdMigrationService migration =
    provider.GetRequiredService<IWechatWorkThirdPartyAccountIdMigrationService>();

string targetCorpId = config["Wechat:TargetCorpId"];

// ④ 「不可回退」的调用前检查：两个条件都成立才允许设置。
//    a) 收口证据 —— 全量新旧 id 映射是否已回收并落库，只有宿主自己的运维记录能证明，
//       SDK 与官方都不替你判；
//    b) 幂等台账 —— 已置过「迁移完成」的企业不再重复调用（重复调用的官方行为未留档，见 §7）。
bool mappingFullyRecycled = bool.TryParse(config["Wechat:MigrationRecycled"], out bool recycled) && recycled;
HashSet<string> alreadyFinishedCorpIds = new HashSet<string>(StringComparer.Ordinal);  // 由宿主运维记录装载

if (!mappingFullyRecycled || alreadyFinishedCorpIds.Contains(targetCorpId))
{
    logger.LogWarning(
        "跳过 {CorpId} 的迁移完成设置：映射已回收={Recycled}，此前已设置={Finished}",
        targetCorpId, mappingFullyRecycled, alreadyFinishedCorpIds.Contains(targetCorpId));
}
else
{
    // openid_type 传 1 是「userid 与 corpid 同时收口」这一个原子单位，官方不允许拆开设置。
    FinishOpenIdMigrationRequest finishOpenIdRequest = new FinishOpenIdMigrationRequest
    {
        CorpId = targetCorpId,
        // AgentId 这里刻意留空。代开发场景下留空 = 更新该企业下「同服务商所有第三方应用」的
        // 升级状态；只想动单个应用时必须显式赋值，见本端点注意事项。
        OpenIdType = new List<int> { 1 },
    };

    WechatWorkResponse finishOpenIdResponse =
        await migration.FinishOpenIdMigrationAsync(finishOpenIdRequest, cancellationToken);

    // ⑤ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
    //    本族是写操作，不判错就等于「误以为已收口」，而实际状态并未翻转。
    WechatWorkException.ThrowIfFailed(finishOpenIdResponse, "/cgi-bin/service/finish_openid_migration");

    alreadyFinishedCorpIds.Add(targetCorpId);   // 幂等台账；持久化到宿主的库/缓存由你自己接
    logger.LogInformation("企业 {CorpId} 的 userid/corpid 迁移完成状态已设置（不可回退）", targetCorpId);
}
```

**本端点注意事项**

- **单向闸门**：设置后**不可回退**（父接口 summary + DTO remarks）。SDK 没有任何二次确认、dry-run 或前置查询能力，误调用的代价是把一家企业永久推进到新 id 形态。上面的「检查 + 台账」是调用方职责，不是可选装饰。
- **省略 `agentid` 是高杀伤写法**：`int?` 留空时官方语义是「更新该企业下**同服务商所有第三方应用**的对应升级状态」，而不是「不动任何应用」。只想收口单个应用必须显式传该应用的 `agentid`；反过来，批量收口正是靠留空实现的——两种意图共用一个可空字段，代码里应当用注释钉住是哪一种。
- `openid_type` 的 `1` 覆盖 userid 与 corpid 两类且**只能同时设置**（官方口径）。想把 userid 和 corpid 分两次收口是做不到的。
- 收口生效后**接口不再返回该企业相关的 unionid**（父接口 summary），且本端点自身不返回任何业务字段，成功只由 `errcode` 表达——不要指望响应里告诉你「哪些应用被改了」。
- 企业同时是服务商并对自己授权时**无需调用**本接口（两支注释均留档），调用前的这条排除判断同样落在宿主侧。
- 查询迁移状态用的是另一支端点、另一种令牌：`GetOpenIdMigrationAsync`（[IdConvert/IWechatWorkAccountIdService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService.cs)，`/cgi-bin/corp/get_openid_migration`，`access_token`，**无请求体的 POST**，响应 `GetOpenIdMigrationResponse.migration_info`）。它属 ID 转换族，本族文档不代为成文，示例也不引用它，以免读者把「查询 + 设置」当成同一族的两次调用。

---

### E2. `FinishExternalUserIdMigrationAsync` — 设置 external_userid 的迁移完成状态（仅第三方）

**签名**

```csharp
Task<WechatWorkResponse> FinishExternalUserIdMigrationAsync(
    [Body] FinishExternalUserIdMigrationRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/externalcontact/finish_external_userid_migration` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/externalcontact/finish_external_userid_migration?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅第三方应用**（代开发子接口为空标记、自建应用官方零开放） |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 锁定 `FinishExternalUserIdMigrationRequest` 登记） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.CorpId` | `corpid` | `string?` | 是 | 目标授权企业的**明文 corpid**（DTO 注释）。请求体**只有这一个字段**：无 `agentid`、无 `openid_type`、无 `external_userid` 列表——本端点是「企业级一刀切」的收口，作用于该企业下**所有第三方应用**的 external_userid（接口 summary：「服务商完成企业下所有第三方应用 external_userid 新旧 id 迁移后，主动设置为迁移完成；此后接口与回调启用新 ID」） |
| —（不进方法签名） | `provider_access_token` | — | 是 | `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`CorpId` 为 `null` 或空白照样发出，错误以 `errcode` 返回。

```json
{
  "corpid": "wwCORPFAKE0001"
}
```

**响应参数**

与 E1 完全同形——返回类型是统一基类 `WechatWorkResponse`，无业务字段：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功 |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| — | `IsSuccess` | `bool`（`[JsonIgnore]` 计算属性） | `ErrorCode == 0` 的便捷判定 |

```json
{
  "errcode": 0,
  "errmsg": "ok"
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                       // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;          // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;    // WechatAppType
using Mud.Wechat.Work.DataModels;            // WechatWorkResponse
using Mud.Wechat.Work.DataModels.AccountId;  // FinishExternalUserIdMigrationRequest

// builder / config / provider / logger / cancellationToken 为宿主约定标识符；
// Microsoft.Extensions.DependencyInjection（GetRequiredService）与 Microsoft.Extensions.Logging
// 由 ASP.NET Core Web SDK 隐式 using 提供。

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "thirdparty-external";
    o.AppType = WechatAppType.ThirdParty;    // E2 只对第三方开放：填 Provider 会解析不到本方法
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

IWechatWorkThirdPartyAccountIdMigrationService externalMigration =
    provider.GetRequiredService<IWechatWorkThirdPartyAccountIdMigrationService>();

string externalTargetCorpId = config["Wechat:TargetCorpId"];

// external_userid 可以单独收口（官方口径），因此这一支常与 E1 分两次执行；
// 但两者同样不可回退，调用前的「该企业下所有第三方应用的 external_userid 映射均已回收」
// 仍须由宿主运维记录证明，本文用配置位代表该证据。
bool externalMappingRecycled = bool.TryParse(config["Wechat:ExternalMappingRecycled"], out bool extRecycled) && extRecycled;

if (externalMappingRecycled)
{
    FinishExternalUserIdMigrationRequest finishExternalRequest = new FinishExternalUserIdMigrationRequest
    {
        CorpId = externalTargetCorpId,   // 明文 corpid；请求体别无他字段
    };

    WechatWorkResponse finishExternalResponse =
        await externalMigration.FinishExternalUserIdMigrationAsync(finishExternalRequest, cancellationToken);

    // 判错的另一种合法写法：显式分支。errcode != 0 时 HTTP 仍是 200。
    if (!finishExternalResponse.IsSuccess)
    {
        logger.LogError("external_userid 迁移完成设置失败 {ErrorCode}: {ErrorMessage}",
            finishExternalResponse.ErrorCode, finishExternalResponse.ErrorMessage);
    }
    else
    {
        logger.LogInformation("企业 {CorpId} 的 external_userid 迁移完成状态已设置（不可回退）", externalTargetCorpId);
    }
}
else
{
    logger.LogWarning("跳过 {CorpId} 的 external_userid 收口：映射尚未全量回收", externalTargetCorpId);
}
```

**本端点注意事项**

- **仅第三方**：ACCT2 断言第三方子接口声明面恰 1、代开发子接口声明面恰 0。代开发应用在 SDK 里找不到「设置 external_userid 迁移完成」这一支，**不要**改用 E1 的 `openid_type = [3]` 去凑——两者关系本文未取证（见 §7）。
- 请求体只有 `corpid`，作用范围是该授权企业下**所有第三方应用**，无法指定单个应用；这与 E1 可以传 `agentid` 精确到应用形成对比。
- 与 E1 同属官方 99375 一页（第三方子接口 remarks 留档），令牌、注入方式、响应形态三者全同，容易误当成同一端点的两种写法。
- 「该企业须已授权该服务商第三方应用」是本端点的前置（E2 请求 DTO remarks）；企业同时是服务商并对自己授权时无需调用（同一处留档）。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 设置端点 E1/E2 vs 查询端点 `GetOpenIdMigrationAsync`（ID 转换族） | 查询走**企业级 `access_token`**、`/cgi-bin/corp/get_openid_migration`、**无请求体的 POST**；设置走**服务商级 `provider_access_token`** + JSON 请求体。两侧都带 `openid_type` 字样 | 令牌族别错配（42011 或 40014 一类失效码）；把响应里的**单值** `openid_type`（`int?`）按数组解析，或反之，反序列化直接错位 |
| E1 vs E2 | 同页、同令牌、同响应形态；E1 第三方 + 代开发、参数含 `agentid` 与 `openid_type` 数组；E2 **仅第三方**、只有 `corpid` | 在代开发子接口上找 E2 是死路（ACCT2 断言代开发空标记）；把 E2 当成「E1 的 external_userid 版」随意替换会丢掉 `agentid` 的精确作用域 |
| `openid_type`（请求 `List<int>?`）vs `openid_type`（响应 `int?`） | 同名不同形状，取值文案顺序也不同（请求侧 `1` - userid 与 corpid；响应侧 `1` - corpid/userid），域 README §6 明确要求勿「对齐」 | 复用同一份枚举/DTO ⇒ 一侧序列化形状错；或把「数组里的 1」误解成「userid 单独收口」 |
| 省略 `agentid` | 语义是「更新该企业下**同服务商所有第三方应用**」，不是「不动任何应用」 | 本想收口一个应用却把整家企业的全部第三方应用一起推过去，且**不可回退** |
| `agentid`（`int?`）vs `source_agentid`（`string?`） | 本域对「应用 id」两套建模：操作对象是 `int?` + `agentid`；来源标注是 `string?` + `source_agentid`（域 README §3） | 复制邻域 DTO 的写法会改坏 JSON 字段名与类型，官方按参数名判定作用范围 |
| 用 `WechatAppType.Internal` 配置调本族 | ACCT2 锁定本族恰两个子接口，无自建子接口；TO1 另断言「自建应用族不得消费 `Wechat.ProviderAccessToken`」 | DI 解析不到可注入服务；即便手工拼出接口，`WechatAppContext.GetTokenManager` 在归属校验处抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |

另外三条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改用 Header）。`provider_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但你自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，`errcode != 0` 时 HTTP 仍是 200 且响应照样反序列化成功。本族是写操作，不判错即「误认为已收口」，示例两种判错写法都给了。
- **配置互斥必填四项**：`WechatAppConfig.Validate()` 对 `ThirdParty` / `Provider` 要求 `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` 齐备，只用服务商凭证面也得配齐套件两项。另注意 `WechatAppConfig` **不提供 `TemplateId`**（代开发的模板 id 即 `SuiteId`）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；`errcode == 0` 即状态已翻转（本族无业务字段可核对） |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效/过期 | 本族的**族别**失效码，走恢复链路自愈 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 族失效/过期 | 本族不消费 `access_token`；出现即凭据族别错配的信号，回查应用配置与所调接口 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族不消费该令牌 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷 |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`，判定值 `{40014, 42001, 42007, 42009, 42011}`）：命中后链路是「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」，业务侧通常看不到这些码。**42011 稳定出现说明刷新本身在失败**，依次排查 `ProviderSecret` 是否正确、服务商侧能力是否开通、官方是否对 `get_provider_token` 做了频率拦截——加重试不解决这三类成因。

本族专属的业务错误码（例如「corpid 不属于本服务商」「应用未授权」一类）内部证据未留档，见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（文档号 90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

判错出口：`WechatWorkException.ThrowIfFailed(response, requestUri)`（`Mud.Wechat.Work.Abstractions.Exceptions`）或自行判 `IsSuccess`。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 第三方锚点文档号不一致 | 父接口 summary 写「第三方子接口另持差异端点（**96516**）」，而[第三方子接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Migration/IWechatWorkAccountIdMigrationService_ThirdParty.cs) 的 XML 与 ACCT1/ACCT2 注释均写 **99375**；域 README 的缺口表已把这条登记为「须核验后同批改注释与守卫 remarks」。本文按「子接口 XML + 守卫」取 99375 ⇒ 父接口那处锚点**未逐项核验** | 官方 99375 页与 96516 页对照；核验后同批改 [Migration 接口文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Migration/) 与守卫 remarks |
| E1 的 `openid_type = 3` 与 E2 的分工 | 内部证据两处并存：E1 的 `openid_type` 数组含 `3`（external_userid 及 external_tagid），同时 E2 又是一支只做 external_userid 收口的独立端点。二者是否等价、该走哪一支，本仓三类证据均未说明 ⇒ **未逐项核验，本文不作取舍** | 官方 99375 页两段正文；核验后补接口 remarks |
| `agentid` 的实际必填性 | E1 请求 DTO 注释同时写「代开发场景官方必填」与「仅传 corpid 未传 agentid 时视为更新同服务商所有第三方应用」，两句在同一注释内互斥（必填即不存在「未传」分支）⇒ 官方口径**未逐项核验**，本文两列都照抄而不合并 | 官方 99378 页参数表 |
| 本族端点的频率上限 | 接口注释与 DTO 均未留档两支端点的 QPS/日配额 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 重复调用（幂等性）行为 | 两支均为不可回退的写操作，但「对已设置为迁移完成的企业再次调用」是返回 0 还是报错，内部证据无记载 ⇒ **未逐项核验**（示例里的幂等台账因此按「宿主自行记账」写，不依赖官方幂等） | 官方 99375/99378 页；实测需真实凭据 |
| 收口后旧 id 的有效期 | 「设置迁移完成后接口不再返回该企业相关的 unionid」是本文唯一留档的后果描述；已发放的旧 userid/corpid/external_userid 是否立即失效、失效窗口多长，本域未留档 ⇒ **未逐项核验**，不得当作「旧 id 立即可用」或「立即可废」的前提 | ID 安全性升级总览页与邻域 ID 转换族 |
| 本族专属业务错误码 | 两支端点的错误码表未在接口注释留档；域 README「各端点错误码集合」同记为未逐项核验 | 官方全局错误码（90455），不在 SDK 侧硬编码码表 |
| `openid_type` 是否存在取值 2 | 请求侧与响应侧注释都只列 1 与 3，2 是否被官方保留 ⇒ **未逐项核验**（域 README 缺口表同条） | 官方 99375 页；出现新取值再决定是否建枚举 |
| provider 令牌的装配与刷新链路 | ACCT3 只锁 `TokenType`/注入模式/参数名；`provider_access_token` 在具体应用上下文中的解析与刷新由 Authentication 域承载，本域**未逐项核验** | Authentication 域及其守卫 |
| 官方页正文本次未重取 | 本文所有「官方口径」均继承本域 2026-10-18 的逐端点核验留档（域 README），**本次未重取官方页正文**；官方若改版则本文可能滞后 | 重新渲染取证 99375 / 99378，并重置本文核验日期 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 契约陷阱汇总，本域权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（§1 三种令牌路由键 / §3 定位键不对称 / §6 枚举实际取值 / §8 官方一页多端点拆分）
- 兄弟族：[TmpExternalUserId.md](./TmpExternalUserId.md)（同为 `access_token`/`provider_access_token` 分野的对照组：唯一三类应用全开放的族）· [Bot.md](./Bot.md)（同令牌的第三族）
- 兄弟族：[IdConvert.md](./IdConvert.md)（含查询迁移状态的 `GetOpenIdMigrationAsync`）· [Corpid.md](./Corpid.md)（同为 `provider_access_token` 的 corpid 转换族）· [Interop.md](./Interop.md)（自建对接族）· [ChatIdUpgrade.md](./ChatIdUpgrade.md)（套件令牌族）
- 本族源码：[Interfaces/AccountId/Migration/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Migration/) · 查询端点源码 [Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService.cs)
- 令牌归属与错配防线：[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· [WechatTokenRouting.cs](../../../../../Src/Work/Mud.Wechat.Work/WechatTokenRouting.cs)（`OwnedKeys` 单一来源）
- 凭据与多应用作用域：`guide/配置与凭据.md`（未成文）
- 官方页：[ID迁移完成状态的设置（第三方 99375）](https://developer.work.weixin.qq.com/document/path/99375) · [ID迁移完成状态的设置（代开发 99378）](https://developer.work.weixin.qq.com/document/path/99378) · [全局错误码（90455）](https://developer.work.weixin.qq.com/document/path/90455)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
