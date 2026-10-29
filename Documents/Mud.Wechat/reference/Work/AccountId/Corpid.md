# corpid 转换（企业主体明文 corpid → 服务商主体密文 open_corpid）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Corpid/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Corpid/) |
| DTO 源文件 | [CorpidToOpenCorpidRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/CorpidToOpenCorpidRequest.cs) · [CorpidToOpenCorpidResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/CorpidToOpenCorpidResponse.cs) |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记）+ [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1 非 AccessToken 族不得叠加归属域键） |
| 官方文档 | 97061（第三方应用）· 97105（服务商代开发）· 页面最后更新 **本次未重取**，见「证据基础」 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（仅内部证据逐项比对；未做本次官方取文） |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与 AccountId 域全部示例合并逐字编译，0 错误（15 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 路由与 HTTP 动词 / ACCT2 层级（父接口恰 1 端点 + 两子接口空标记 + 继承链不漂移）/ ACCT3 令牌绑定 / ACCT4 上下文登记 + TO1/TO2 + 三份接口与两份 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) 逐端点核验 2026-10-18 · 官方：正文继承本域 2026-10-18 核验留档，**本次未重取官方页**，故凡只有官方页能支撑的事实（频率数值、专属错误码、页面更新日期、「待迁移自建应用企业」的准确口径）一律进 §7 |

## 1. 这个接口族做什么

把**企业主体的明文 `corpid`** 转换成**服务商主体下的密文 `open_corpid`**。账号 ID 安全性升级后，第三方应用与代开发应用在自己的数据里不能再用明文 `corpid` 标识一家企业，必须以服务商主体视角的 `open_corpid` 作主键；本端点就是「明文 corpid → 服务商视角企业 id」这一步的唯一入口。

调用凭证是**服务商级**的 `provider_access_token`，不是企业级 `access_token`。这带来一个与同域其它族最实质、也最容易写错的差别：**本族不需要企业作用域**——既不用 `UseCorpScope`，也不需要该企业已把 `permanent_code` 交给你，因为鉴权主体是服务商本身，目标企业是**请求体里的参数**而不是令牌的作用域。

**不该用本族的场景**：① 你已有 `open_corpid`、想还原成明文 `corpid` —— 本域没有这一支（ACCT1 的 20 条路由里没有 `opencorpid_to_corpid`，官方未开放即是证据），不要去找反向端点；② 要换的是**企业内成员**的 id 而不是企业 id，走 ID 转换族 / 自建对接族；③ 你是企业自建应用（`WechatAppType.Internal`），官方对本族无开放面，ACCT2 的继承链断言里没有 `_Internal` 子接口，DI 也解析不出来。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` 或 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（`第三方/服务商应用 {AppKey} 缺少 …`），启动即失败，不留到首次调用 |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken`（常量值 `Wechat.ProviderAccessToken`）→ 官方参数 `provider_access_token` | — |
| 令牌归属键 | **不得声明** `TokenManagerKey`（TO1：非 AccessToken 族本身已无歧义，叠加归属域键即违规；`@Provider`/`@Suite` 归属键永不增设） | 越界声明由 TO1 直接判红 |
| 企业作用域 | **不需要**：`provider_access_token` 无企业 scope，目标企业由请求体 `corpid` 表达 | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名**；参数名 `provider_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，无 G7 豁免负担（父接口 remarks） | 每次请求 |

`provider_access_token` 由 SDK 侧的 `ProviderTokenManager` 以 `CorpId + ProviderSecret` 自动换取、缓存并刷新，**开发者不接触令牌值**（换取接口的频率警示与刷新节奏口径见同域 [Bot.md](./Bot.md) §2.1，本文不重抄）。

**注意「配置互斥必填」这一条会改变写法**：`WechatAppConfig.Validate()` 对 `ThirdParty` / `Provider` 要求四项齐备，本族只消费 `CorpId + ProviderSecret` 那一条令牌链，但**只配这两项照样启动期抛异常**——`SuiteId` / `SuiteSecret` 必须一并给出（校验在配置层，不看你要调哪个端点）。若目标应用确实要用套件令牌，还需另行接入回调接收 `suite_ticket`（见 [ChatIdUpgrade.md](./ChatIdUpgrade.md) §2.2）。

### 2.2 权限与业务前置

出处为接口与请求/响应 DTO 的 XML 注释（本域 2026-10-18 留档口径），不是本文推断：

- **仅限第三方服务商**调用，转换对象是**已获授权企业**的 `corpid`，或**待迁移的自建应用企业**的 `corpid`（父接口 `<summary>` 与 `CorpidToOpenCorpidRequest` remarks 双重留档）。「待迁移」的准确判定条件与时间窗注释未留档 ⇒ 见 §7。
- **入参是明文 `corpid`，不是 `open_corpid`**：域 README §3 把本 DTO 归入「provider 族以明文 corpid 定位企业（不是 open_corpid）」一行。方向写反是这个端点最常见的用法错误。
- **单值、非批量**：请求体只有 `corpid` 一个字段（`string?`），域 README §4 的上限一览把本族归入「批量端点都不传 `corpid`/`agentid`」的对立面——本族显式传定位键、且一次一家企业。
- **密文大小写敏感**：`open_corpid`「使用时不能再转为纯小写或纯大写」（父接口 XML + 响应 DTO 注释原文）⇒ 宿主侧不得做 `ToLower()` / `ToUpper()` 归一化，否则与后续以 `open_corpid` 为键的接口对不上。

### 2.3 官方开放的应用类型

官方对**第三方应用**与**服务商代开发**开放完全一致的 1 个端点（97061 / 97105 两页），对企业自建应用无本族端点。落到代码形态：

- 端点声明在**公共父接口** `IWechatWorkAccountIdCorpidService` 上（ACCT2 断言父接口 `DeclaredOnly` 方法数 = 1）；
- `IWechatWorkThirdPartyAccountIdCorpidService` 与 `IWechatWorkProviderAccountIdCorpidService` 均为**空标记子接口**（ACCT2 各断言 = 0），仅承载 DI 注册与 `RegistryGroupName`；
- 继承链上**不得**出现第三种子接口（ACCT2 `AssertDerivedInterfaces` 期望恰两个）。

⇒ **两个空标记子接口存在的唯一理由是「一接口族一令牌路由键」**（父接口 XML 原文：「与企业级 `access_token` 端点分属不同令牌路由键，一接口族一令牌路由键，故独立成族」）。ACCT3 逐接口断言令牌路由键，把本族并进 ID 转换族会当场打红该分组断言，也会让「同一接口族只消费一种鉴权主体」这条判据失效。域 README §1 已明确「不得把它们并进 ID 转换族省两个接口文件」。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdCorpidService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载唯一端点 | 1（E1） | 否 |
| `IWechatWorkThirdPartyAccountIdCorpidService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1） | 是 |
| `IWechatWorkProviderAccountIdCorpidService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`（两子接口 `RegistryGroupName`，父接口恒空；子接口 `InheritedFrom = nameof(WechatWorkAccountIdCorpidService)`）
- 端点索引：**E1** `CorpidToOpenCorpidAsync`（明文 corpid → 服务商主体密文 open_corpid）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，仅作契约基座）。两子接口方法集完全相同，按应用类型选一个即可。

---

## 4. 端点

### E1. `CorpidToOpenCorpidAsync` — 企业主体明文 corpid 转服务商主体密文 open_corpid

**签名**

```csharp
Task<CorpidToOpenCorpidResponse> CorpidToOpenCorpidAsync(
    [Body] CorpidToOpenCorpidRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/corpid_to_opencorpid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/corpid_to_opencorpid?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成；服务商级凭证） |
| 适用应用类型 | 第三方应用 · 服务商代开发（自建应用官方未开放） |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 已登记请求与响应两型） |
| 官方文档 | 97061（第三方）· 97105（代开发） |

路由带 `service/` 前缀，是本域服务商凭证端点的共同特征（`/cgi-bin/service/...`）；ACCT1 逐条断言 `typeof(PostAttribute)`，本条虽为纯查询语义仍走 POST——**不要按「查询用 GET」的直觉改写动词**（域 README §7）。

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.CorpId` | `corpid` | `string?` | 是 | **企业主体明文 corpid**（待获取 `open_corpid` 的那家企业），非 `open_corpid`；官方口径为「已获授权企业，或待迁移的自建应用企业」（DTO remarks） |
| —（不进方法签名） | `provider_access_token` | — | 是 | 应用服务商的接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`CorpId` 为 `null` 或空白串也照样发出，错误以 `errcode` 返回。本端点无批量入参，多企业换算需逐个调用（是否可并发、并发上限官方未留档 ⇒ §7）。

```json
{
  "corpid": "wwAUTHCORPIDFAKE"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `open_corpid` | `OpenCorpId` | `string?` | 该服务商第三方应用下的企业 ID（服务商主体下的密文 corpid；**大小写敏感**，不能再转为纯小写或纯大写） |

响应是**单值**、无 `invalid_*` 袋：只有「拿到 `open_corpid`」与「`errcode` 非 0」两种形态（就本仓可核验的部分而言；失败形态全集见 §7）。注意 `open_corpid` 是**按服务商主体视角**的 id —— 同一家企业在不同服务商下得到不同 `open_corpid`，不可跨服务商共用主键（DTO 注释「该服务商第三方应用下的企业 ID」）。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "open_corpid": "wpOPENCPIDFAKE0001"
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;            // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException（判错出口）
using Mud.Wechat.Work.DataModels.AccountId;    // 请求/响应 DTO

// 宿主为 ASP.NET Core（Web SDK）+ ImplicitUsings 时，System.Linq / System.Collections.Generic /
// Microsoft.Extensions.DependencyInjection（GetRequiredService）/ Microsoft.Extensions.Logging
// 均由 SDK 隐式提供；换非 Web 宿主形态须自行补齐 using。

// ① 注册服务商应用：Validate() 对 ThirdParty/Provider 要求四项齐备，本族虽只用
//    CorpId + ProviderSecret 取令牌，缺 SuiteId/SuiteSecret 仍会在启动期抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.ThirdParty;   // 代开发改填 WechatAppType.Provider，方法集完全相同
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配，无需额外注册令牌链）。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析子接口（父接口 IsAbstract = true、不注册 DI，不可解析）。
//    本族令牌是服务商级：无需 UseCorpScope/BeginScope，目标企业就是下面的 corpid 参数。
IWechatWorkThirdPartyAccountIdCorpidService corpid =
    provider.GetRequiredService<IWechatWorkThirdPartyAccountIdCorpidService>();

string targetPlainCorpId = "wwAUTHCORPIDFAKE";   // 假值：待换算的授权企业明文 corpid

// ④ 单值转换：一次一家企业，多企业请自行逐个调用。
CorpidToOpenCorpidResponse corpidResponse = await corpid.CorpidToOpenCorpidAsync(
    new CorpidToOpenCorpidRequest { CorpId = targetPlainCorpId },
    cancellationToken);

// ⑤ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
WechatWorkException.ThrowIfFailed(corpidResponse, "/cgi-bin/service/corpid_to_opencorpid");

// ⑥ 取业务字段。密文大小写敏感：落库/比较前不得 ToLower()/ToUpper()，
//    否则与后续以 open_corpid 为键的接口对不上。
string openCorpId = corpidResponse.OpenCorpId ?? string.Empty;
if (openCorpId.Length == 0)
    logger.LogWarning("errcode 为 0 但未返回 open_corpid，corpId 可能不属于本服务商可见范围");
```

**本端点注意事项**

- 明文 `corpid` 从哪来不由本域决定（授权事件、企业后台、既有主数据），本域只消费；把它当 `open_corpid` 传进去是最常见的用法反向，其失败形态本文未取证（§7）。
- `open_corpid` 的稳定性与有效期（是否随服务商主体恒定、企业重新授权后是否变更）注释与 README 均无表述 ⇒ **未逐项核验**，本文不作假设性承诺；若要长期以它作主键，先按 §7 核验。
- 本族与 ID 迁移完成状态族、智能机器人族同属 `provider_access_token`（ACCT3 九接口一组），令牌链复用同一条，切换族别不需要改配置。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族入参 `corpid` vs 输出 `open_corpid` | 方向是「明文 corpid → 密文 open_corpid」；本域**没有**反向端点（ACCT1 的 20 条路由无 `opencorpid_to_corpid`） | 把已持有的 `open_corpid` 当 `corpid` 传进去，属于契约外用法，失败形态未留档（§7）；更不要写「反解 open_corpid」的兜底逻辑，官方不给这条路 |
| ID 转换族的批量端点（`/cgi-bin/batch/userid_to_openuserid` 等） | 那些端点**不传** `corpid`/`agentid`，企业身份由 `access_token` 的作用域表达；本族是服务商级令牌、企业身份只能显式写在请求体（域 README §1、§3） | 照批量端点的写法「切好作用域再调本族、请求体留空 corpid」→ 作用域对本族无效，`corpid` 为 `null` 仍会发出请求；反过来给批量端点补 `corpid` 也是无效字段 |
| `IWechatWorkProviderAccountIdBotService` / 迁移完成族子接口 | 同为 `provider_access_token`、同为「第三方 + 代开发」开放面，只差业务动作与 DTO | 令牌链相同 ⇒ 族别写错**不会**报凭据错，只会拿到形态完全不同的响应，属静默串族；靠方法名与路由辨别 |
| 「不需要企业作用域」 vs「不需要 `UseAppScope`」 | 本族免的是**企业 scope**（`UseCorpScope`）；多**应用**归属切换仍照常生效，因为 `provider_access_token` 是按应用配置取的 | 误以为服务商级令牌与配置无关，在应用 A 的上下文里期望拿到应用 B 的 `provider_access_token` ⇒ 拿到的是当前应用的令牌，`open_corpid` 是「当前服务商主体视角」的值 |
| 自建应用（`WechatAppType.Internal`）调本族 | SDK 不为自建应用提供本族子接口（ACCT2 期望集合恰两个），且 `Validate()` 要求自建填 `AgentSecret`、无 `ProviderSecret` 路径 | DI 解析不到可注入服务；即使强行解析父接口类型也无从取 `provider_access_token`（自建应用不装配服务商令牌管理器） |
| 用 `open_corpid` 当内部主键后又做大小写归一化 | 官方口径「密文大小写敏感，使用时不能再转为纯小写或纯大写」（父接口 XML + 响应 DTO 注释） | 归一化后与官方返回值不相等 ⇒ 后续所有以 `open_corpid` 为键的调用与 join 全部失效，且无异常提示 |

另外三条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 为已接受风险，无法改用 Header）。`provider_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。
- **归属域键不得叠加**：TO1 明文规定 `ProviderAccessToken` / `SuiteAccessToken` 族「不得声明 `TokenManagerKey`」，且永不增设 `@Provider`/`@Suite` 键。看到有人为本族补归属键，那是违规改动而非缺失加固。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径（仍可能未返回 `open_corpid`，见 E1 示例第 ⑥ 步） |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效/过期 | 本端点的族别失效码，命中即走令牌自愈链路，业务侧通常看不到 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 族失效/过期 | 本端点不消费 `access_token`；出现即凭据族别错配的信号，回查应用配置与所调接口 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本端点不消费该令牌（套件令牌只归同域 [ChatIdUpgrade.md](./ChatIdUpgrade.md) 一族） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷（未 `AddWechatApp`、或未 `AddAccountIdApi()`） |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`。**若 42011 稳定出现，说明服务商令牌刷新本身在失败**，可依次排查 `ProviderSecret` 是否正确、服务商企业是否已开通相应能力、以及官方侧是否对 `get_provider_token` 做了频率拦截（该警示的仓内留档见 [Bot.md](./Bot.md) §2.1 与 §6）。加重试不解决这三类成因。

本端点专属的业务错误码，接口注释未留档、本文亦未向官方页取证 ⇒ **未逐项核验**，见 §7。其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（文档号 90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方页面最后更新日期 | 本次未重取官方页（任务纪律：官方取证由文档负责人串行完成），正文继承本域 2026-10-18 逐端点核验留档 ⇒ **未逐项核验** | 97061 / 97105 官方页渲染取文后回填元信息表 |
| 本端点的独立频率数值 | 接口与 DTO 注释只留档「仅限第三方服务商」「单值 corpid」，无 QPS/日配额 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 本端点专属错误码集合 | 注释未留档错误码表；令牌失效码由通用判定器覆盖 ⇒ **未逐项核验** | 官方附录「全局错误码」（90455）逐端点核对后补接口 remarks |
| 「待迁移的自建应用企业 corpid」的准确口径 | 注释只留档这一短语，未定义何为「待迁移」、可转换时间窗、迁移完成后是否仍可转换 ⇒ **未逐项核验** | 97061 页的权限/适用范围段落 + ID 安全性升级总览页（96516） |
| `open_corpid` 的稳定性与有效期 | 是否随服务商主体恒定、企业重新授权/迁移后是否变更，注释无表述 ⇒ **未逐项核验**，本文不作假设性承诺 | 97061/97105 页与 96516 总览页 |
| 传入 `open_corpid`（方向写反）时的官方行为 | 未说明是 `errcode` 非 0 还是返回空 `open_corpid` ⇒ **未逐项核验** | 97061 页实测口径 |
| 多企业批量/并发换算的官方建议 | 本端点单值入参，注释与本域留档均未给出批量或并发上限 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| `provider_access_token` 的换取/刷新链路细节 | 令牌注入与参数名由 ACCT3/TO1 锁定，其换取、缓存与刷新路由属 Authentication 域承载 ⇒ 本域**未逐项核验**（域 README 缺口表同款留档） | [Src/Work/Mud.Wechat.Work/Authentication/](../../../../../Src/Work/Mud.Wechat.Work/Authentication/) 及其守卫 |
| 内部证据不一致（域 README 的非 AccessToken 子接口计数） | [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) §1 归属域表格写「六个 ProviderAccessToken / SuiteAccessToken 子接口 **不得声明**」，而 ACCT3 的 provider 组子接口恰 6 个（本族 2 + 迁移 2 + 机器人 2）、suite 组子接口 1 个（群 ID 升级族代开发子接口），合计 **7** 个非 AccessToken 子接口 ⇒ 该「六个」只在「仅统计 ProviderAccessToken 子接口」时成立，文案把 Suite 子接口一并计入则偏少一项 ⇒ **未逐项核验孰为本意** | ACCT3 的三个接口数组（权威）＋ TO1 的非 AccessToken 分支 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + §1 三种令牌路由键、§3 定位键不对称、§4 批量上限、§7 GET 分布）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（本文引用不重抄）
- 同令牌（`provider_access_token`）兄弟族：
  - [Bot.md](./Bot.md) — 智能机器人 userid 转换族（含服务商令牌的凭据面细则与刷新排查）
  - [Migration.md](./Migration.md)（ID 迁移完成状态族，**不可回退**的单向闸门）
- 不同令牌的对照族（套件级令牌 + 必须接入回调）：[ChatIdUpgrade.md](./ChatIdUpgrade.md)
- 反向换算（服务商密文 → 企业明文）族：[Interop.md](./Interop.md)
- `open_corpid` 的消费方：企业微信账号 ID 安全性升级后的各服务商侧接口，见同域 ID 转换族 [IdConvert.md](./IdConvert.md)
- 归属域与令牌错配的 fail-fast 契约：[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）
- 凭据与多应用作用域：`guide/配置与凭据.md`（**尚未成文**）
- 官方页：[corpid 转换（第三方 97061）](https://developer.work.weixin.qq.com/document/path/97061) · [corpid 转换（代开发 97105）](https://developer.work.weixin.qq.com/document/path/97105) · [获取服务商凭证（91200）](https://developer.work.weixin.qq.com/document/path/91200)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
