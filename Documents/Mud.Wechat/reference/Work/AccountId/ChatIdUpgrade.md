# 群 ID 升级（对所有新授权企业，套件级凭证）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AccountId/ChatIdUpgrade/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/ChatIdUpgrade/) |
| DTO 源文件 | 本族唯一端点**无请求体**、响应为裸 [WechatWorkResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/WechatWorkResponse.cs)（该类型标注 `SerializerClassName = "Common"`，登记在 Common 上下文而非 `AccountIdJsonContext`） |
| 令牌前置源文件 | [SuiteTokenManager](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/SuiteTokenManager.cs) · [WechatSuiteTicketProvider](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/WechatSuiteTicketProvider.cs) · [内置回调处理器（写 suite_ticket）](../../../../../Src/Work/Mud.Wechat.Work.Callback/WechatCallbackHandler.cs) |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由与 GET 动词 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记）+ [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1 非 AccessToken 族不得叠加归属域键） |
| 官方文档 | 99601「群 ID 升级」（服务商代开发，接口三）· 页面最后更新 **本次未重取**，见「证据基础」 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（仅内部证据逐项比对；未做本次官方取文） |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与 AccountId 域全部示例合并逐字编译，0 错误（15 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 路由 + `typeof(GetAttribute)` 动词断言 / ACCT2 层级（父接口零端点 + 代开发子接口恰 1 + 继承链不得出现自建或第三方子接口）/ ACCT3 `SuiteAccessToken` + Query + `suite_access_token` / ACCT4 上下文登记 + TO1/TO3 + 两份接口 XML 注释 + 令牌链源码（`SuiteTokenManager`、`WechatSuiteTicketProvider`）+ 回调内置处理器源码 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) 逐端点核验 2026-10-18 · 官方：正文继承本域 2026-10-18 核验留档，**本次未重取官方页**，故凡只有官方页能支撑的事实（频率数值、专属错误码、页面更新日期、`suite_ticket` 推送节奏的官方原文）一律进 §7 |

## 1. 这个接口族做什么

对**代开发应用模板**做一次总开关式的群 ID 升级：**调用之后**新授权该模板的企业，其客户群 ID 会自动升级为服务商主体视角的群 ID，服务商不必再逐家企业去调「申请群ID的升级」。它解决的是「模板已决定全面切到服务商主体群 ID，但不想为每一家新授权企业单独排一次升级流程」这个场景。

它**不影响已授权企业**。存量企业要升级群 ID，走的是同官方页（99601）的另外两支——它们挂在 ID 转换族的代开发子接口 `IWechatWorkProviderAccountIdService` 上、用企业级 `access_token` 鉴权：先 `ApplyToUpgradeChatIdAsync` 设定升级时间，到达时间后 `ConvertChatIdAsync` 批量转换（≤100）。两支都要先把上下文切到目标企业作用域，本族那支**不需要**（套件级令牌无企业 scope）。令牌分工见 §5。

调用凭证是**代开发模板的接口调用凭证** `suite_access_token`（套件级），这决定了本族最实质的使用差异：**必须先接入回调收到 `suite_ticket`**，SDK 才换得出这个令牌（§2.2）。这是本族与同域另外两族（`access_token` 族、`provider_access_token` 族）在使用前置上的根本不同——后两者的凭据都能由 SDK 用配置里的 secret 主动换取，`suite_access_token` 不行。

本族也是本域**唯一消费 `suite_access_token` 的族**（ACCT3 的 suite 组恰两个接口：父接口 + 代开发子接口）。

**不该用本族的场景**：① 要给**已授权**企业升级群 ID —— 那是 ID 转换族代开发子接口的两支；② 你是第三方应用（非代开发模板）或企业自建应用，官方对本族无开放面（ACCT2 期望子接口集合只有 `_Provider`）；③ 你只是想要一个「查询是否已开启升级」的开关状态 —— 本域没有这种端点，别按 `GET` 动词把它当查询接口用，它是**写操作**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Provider`（服务商代开发；本族唯一开放面） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（`第三方/服务商应用 {AppKey} 缺少 …`）；本族取令牌只用 `SuiteId + SuiteSecret`，但另两项属配置层互斥必填，缺则启动期即失败 |
| 令牌类型 | `WechatTokenTypes.SuiteAccessToken`（常量值 `Wechat.SuiteAccessToken`）→ 官方参数 `suite_access_token` | — |
| 令牌归属键 | **不得声明** `TokenManagerKey`（TO1：非 AccessToken 族本身已无歧义，`@Provider`/`@Suite` 归属键永不增设） | 越界声明由 TO1 直接判红 |
| 企业作用域 | **不需要**：套件级凭证、无企业 scope，不经由令牌作用域机制表达（父接口 XML 原文） | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名**；参数名 `suite_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，无 G7 豁免负担（父接口 remarks） | 每次请求 |

`SuiteId` 与代开发 `template_id` 是同一个值——`WechatAppConfig` **不提供独立的 `TemplateId` 属性**（协议上「代开发模板 id 即 suite_id」，非法状态在类型层面不可表达），所以本族的「模板」身份就是 `SuiteId`。

### 2.2 权限与业务前置（本族的关键前置：suite_ticket）

`suite_access_token` 由 `SuiteTokenManager` 换取，其唯一模板点是：**先读 `suite_ticket`，再用 `SuiteId + SuiteSecret + SuiteTicket` 换套件令牌**（源码 `SuiteTokenManager.RefreshTokenFromApiAsync`）。而 `suite_ticket` **只能由官方定时推送、本仓没有任何主动获取它的端点** —— 默认实现 `WechatSuiteTicketProvider` 只从 `IWechatSuiteTicketStore` 读，读不到就抛：

```
套件 {suiteId} 的 suite_ticket 尚未接收：请接入 Mud.Wechat.Work.Callback 回调接收（微信每 10 分钟推送一次），
或自行实现 IWechatSuiteTicketStore 写入最新票据后再调用套件令牌接口。
```

⇒ **宿主必须接入回调接收 `suite_ticket`**，否则本族的令牌链在「取凭证」这一步就断，端点根本调不到。该节奏在仓内有**两处**独立留档（异常文案 + [WechatCallbackEventTypes](../../../../../Src/Work/Mud.Wechat.Work/Callback/WechatCallbackEventTypes.cs) 中 `suite_ticket` 常量注释「每 10 分钟一次，驱动 get_suite_token」），但两处都是仓内文本、不是本次官方取文，故官方原文与是否可变标为未核验（§7）。仓内的落地形态是内置兜底回调处理器 `WechatCallbackHandler`：识别 `suite_ticket` 事件后按 `evt.SuiteId` 写入 `IWechatSuiteTicketStore`（`SuiteId` 缺失或票据为空时各自告警丢弃，不污染其它套件槽位）。多实例部署下该存储须换成分布式实现（`Mud.Wechat.Redis`），否则收票据的进程与调本族的进程不是同一个内存槽位。

配套约束（出处为回调配置源码）：`suite_ticket` 属**授权族**，只在**套件指令通道**（`WechatCallbackChannel.Suite`）上许可分发，且自建应用不得占用套件通道；因此代开发应用要在回调侧收到它，必须把该应用配成套件通道。

其余业务前置（出处为父接口与端点 XML 注释）：

- **只作用于「调用之后新授权」的企业**，已授权企业不受影响（父接口 + 子接口 `<summary>` 双重留档）。
- 与逐企业升级是**两条独立路径**：调本族后「无需逐个企业调用『申请群ID的升级』接口」（父接口 XML）。
- 无请求体、无参数，因此本族**没有任何调用级业务入参可配** —— 幂等性、可否撤销、重复调用的官方行为注释未留档 ⇒ §7。

令牌未就绪时的失败形态是 `InvalidOperationException`（在刷新期抛出，**不是** `errcode`），示例如下：

```csharp
using Mud.Wechat.Work;                         // 可注入子接口
using Mud.Wechat.Work.DataModels;              // WechatWorkResponse（裸响应基类）
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException

// 前置：宿主已 AddWechatCallback 并让本应用走套件通道，suite_ticket 已入 IWechatSuiteTicketStore。
IWechatWorkProviderAccountIdChatIdUpgradeService precheckUpgrade =
    provider.GetRequiredService<IWechatWorkProviderAccountIdChatIdUpgradeService>();

try
{
    WechatWorkResponse precheckResponse =
        await precheckUpgrade.UpgradeChatIdForNewCorpAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(precheckResponse, "/cgi-bin/idconvert/upgrade_chatid_for_new_corp");
}
catch (InvalidOperationException ex)
{
    // 套件票据缺失走的是这条：SDK 在换 suite_access_token 时抛，请求根本没发出。
    // 仓内文案只含 SuiteId，不含 suite_ticket 值，可安全入日志；换成打印整异常对象前先复核自家文案。
    logger.LogError("suite_ticket 尚未就绪，套件令牌无法换取，本族调用未起飞：{Message}", ex.Message);
    throw;
}
```

### 2.3 官方开放的应用类型

官方仅向**代开发模板**开放本端点（父接口 XML：「官方仅向代开发模板开放」）。落到代码形态：

- 公共父接口 `IWechatWorkAccountIdChatIdUpgradeService` **零端点**（ACCT2 断言 `DeclaredOnly` 方法数 = 0）；
- 唯一端点声明在代开发子接口 `IWechatWorkProviderAccountIdChatIdUpgradeService`（ACCT2 断言恰 1）；
- 继承链上**不得**出现自建/第三方子接口（ACCT2 `AssertDerivedInterfaces` 的 because 文案原文）。

⇒ 第三方应用与自建应用没有通向本族的 DI 入口。子接口为 `_Provider` 前缀命名，按 TO1 的族别判定归 `Provider` 族，但其 `TokenType` 是 `SuiteAccessToken`，故 TO1 的另一条规则同时生效：**自建族不得消费该令牌**、且**该令牌不得叠加归属域键**。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdChatIdUpgradeService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），零端点，仅作契约基座 | 0 | 否 |
| `IWechatWorkProviderAccountIdChatIdUpgradeService` | `Mud.Wechat.Work` | 服务商代开发子接口，承载唯一端点 | 1（E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`（子接口 `RegistryGroupName`，父接口恒空；子接口 `InheritedFrom = nameof(WechatWorkAccountIdChatIdUpgradeService)`）
- 端点索引：**E1** `UpgradeChatIdForNewCorpAsync`（对模板的所有**新增**授权企业升级群 ID，无请求体的 GET）

最终用户注入**代开发子接口**，不要消费父接口（父接口不注册 DI，且一个端点也没有）。

---

## 4. 端点

### E1. `UpgradeChatIdForNewCorpAsync` — 对所有新授权企业升级群 ID

**签名**

```csharp
Task<WechatWorkResponse> UpgradeChatIdForNewCorpAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/idconvert/upgrade_chatid_for_new_corp` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/idconvert/upgrade_chatid_for_new_corp?suite_access_token=SUITE_ACCESS_TOKEN` |
| 凭据参数 | `suite_access_token`（Query 注入，SDK 自动完成；代开发模板/套件级凭证） |
| 适用应用类型 | 仅服务商代开发 |
| 请求体 | **无**（GET、无参数、无 body，只靠 Query 令牌鉴权） |
| 响应类型 | 裸 `WechatWorkResponse`（仅 `errcode` / `errmsg`，无业务字段） |
| 官方文档 | 99601（接口三） |

**这是本域 20 支端点里仅有的 2 支 GET 之一**（另一支是 ID 转换族的 `ApplyMassCallTicketAsync`），且两支都是「无请求体」（域 README §7）。ACCT1 逐条断言 `typeof(GetAttribute)`，所以「GET + 无请求体」是钉死的契约形态；反过来也要注意：**GET 在这里不代表查询**，它是对模板开关的一次写操作。

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（本端点无业务入参） | — | — | — | 方法签名只有 `CancellationToken`；被升级的模板由令牌本身（`SuiteId`）确定，不由参数表达 |
| —（不进方法签名） | `suite_access_token` | — | 是 | 代开发模板的接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传；其换取依赖 `suite_ticket`（§2.2） |

无请求体 ⇒ 没有请求示例 JSON。

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（`WechatWorkResponse`，`IsSuccess => ErrorCode == 0`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

本端点**不回传任何受影响企业清单或升级计数** —— 除 `errcode` / `errmsg` 外无业务字段，所以「调用成功」只能说明开关已置位，不能用来核对到底生效了哪些企业（域内无查询端点，见 §5）。本域另有 3 支同样是裸 `WechatWorkResponse` 的端点（`ApplyToUpgradeChatIdAsync` 与 ID 迁移完成状态族的两支），按仓内 grep 计数而非推测。

```json
{
  "errcode": 0,
  "errmsg": "ok"
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;            // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException（判错出口）
using Mud.Wechat.Work.DataModels;              // WechatWorkResponse（裸响应基类）

// 宿主为 ASP.NET Core（Web SDK）+ ImplicitUsings 时，System.Linq / System.Collections.Generic /
// Microsoft.Extensions.DependencyInjection（GetRequiredService）/ Microsoft.Extensions.Logging
// 均由 SDK 隐式提供；换非 Web 宿主形态须自行补齐 using。

// ① 注册代开发应用：SuiteId 即代开发模板 id（官方无独立 TemplateId），四项齐备才过 Validate()。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-app";
    o.AppType = WechatAppType.Provider;
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块。本族的前置不止「配好凭据」：宿主还必须接入回调收 suite_ticket，
//    否则第 ③ 步取令牌时抛 InvalidOperationException（见 §2.2），请求根本发不出去。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析代开发子接口（父接口 IsAbstract = true、零端点、不注册 DI，不可解析）。
//    套件级令牌无企业 scope：这里不需要 UseCorpScope，也不传任何企业标识。
IWechatWorkProviderAccountIdChatIdUpgradeService suiteUpgrade =
    provider.GetRequiredService<IWechatWorkProviderAccountIdChatIdUpgradeService>();

// ④ 无请求体的 GET：唯一入参是取消令牌。
WechatWorkResponse upgradeResponse =
    await suiteUpgrade.UpgradeChatIdForNewCorpAsync(cancellationToken);

// ⑤ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
WechatWorkException.ThrowIfFailed(upgradeResponse, "/cgi-bin/idconvert/upgrade_chatid_for_new_corp");

// ⑥ 取业务字段：本端点除 errcode/errmsg 外没有载荷，"取值"只能是判成功 + 留审计文本。
//    生效范围只覆盖「本次调用之后新授权的企业」，存量企业仍需逐企业走 ID 转换族那两支。
logger.LogInformation("模板 {SuiteId} 的新授权企业群 ID 升级已置位：{ErrorMessage}",
    config["Wechat:SuiteId"], upgradeResponse.ErrorMessage);
```

**本端点注意事项**

- **重复调用是否幂等、能否撤销、置位后是否影响后续模板版本**，接口注释与本域留档均无表述 ⇒ **未逐项核验**（§7）。它是写操作，宿主侧应自行加「只执行一次 / 需人工确认」的门禁，不要把它放进自动重试循环。
- 无业务字段 ⇒ 不能靠响应确认「哪几家企业已升级」。存量企业的真实升级结果只能由 ID 转换族的 `ConvertChatIdAsync`（`/cgi-bin/idconvert/chatid`，返回 `items` 与 `invalid_chat_id_list`）逐企业核对。
- 令牌链是三层依赖：`suite_ticket`（回调推送，不可主动获取）→ `suite_access_token`（SDK 换取）→ 本端点。任一层断掉的表现不同：票据缺失是 `InvalidOperationException`（本地抛，未发请求），令牌失效是 `errcode`（42009 族，走自愈）。排查时先分清这两类。

---

## 5. 陷阱与易混对照

**官方一页三接口、本域拆两族两令牌**（99601「群 ID 升级」的三条接口在本仓分属两个族，鉴权主体与作用范围都不同；域 README §8 已把这条列为本域两处「一页多端点」拆分之一）：

| 官方接口 | 本仓方法 | 所在族 / 子接口 | HTTP 与路由 | 令牌与归属键 | 作用范围 | 企业作用域 |
|---|---|---|---|---|---|---|
| 接口一 申请群 ID 的升级 | `ApplyToUpgradeChatIdAsync` | ID 转换族 / `IWechatWorkProviderAccountIdService` | `POST` `/cgi-bin/idconvert/apply_to_upgrade_chatid` | `access_token` + `Wechat.AccessToken@Corp` | 单家**已授权**企业，设定升级时间 | **需要** `UseCorpScope(appKey, authCorpId, permanentCode)` |
| 接口二 群 ID 转换 | `ConvertChatIdAsync` | ID 转换族 / `IWechatWorkProviderAccountIdService` | `POST` `/cgi-bin/idconvert/chatid` | `access_token` + `Wechat.AccessToken@Corp` | 单家已授权企业，≤100 条群 id，须先调接口一 | **需要** |
| 接口三 对所有新授权企业升级 | **`UpgradeChatIdForNewCorpAsync`（本族 E1）** | 群 ID 升级族 / `IWechatWorkProviderAccountIdChatIdUpgradeService` | `GET` `/cgi-bin/idconvert/upgrade_chatid_for_new_corp`（**无请求体**） | `suite_access_token`（套件级，**不声明**归属域键） | 模板下**调用之后新授权**的企业 | **不需要**（无企业 scope） |

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| ID 转换族代开发子接口的 `ApplyToUpgradeChatIdAsync` / `ConvertChatIdAsync` | 同官方页，但走**企业级** `access_token`（归属键 `Wechat.AccessToken@Corp`）、需先切到目标企业作用域、且只对已授权企业有效 | 把它俩当本族的「批量版」来调：既漏了 `UseCorpScope`（会在 `WechatAppContext.GetTokenManager` 处 fail-fast 抛 `WechatTokenOwnerMismatchException`），又误以为一条调用能覆盖全部企业；反向混淆（用本族去处理存量企业）则**静默无效**——官方只保证影响新授权企业 |
| `provider_access_token` 族（同域 corpid 转换族、智能机器人族、ID 迁移完成状态族） | 三者都是服务商级、免企业作用域；本族是**套件级**，`SuiteId + SuiteSecret + suite_ticket` 才能换出令牌 | 看到「服务商形态、免企业 scope」就以为凭据链同源：`provider_access_token` 可用 `CorpId + ProviderSecret` **主动换取**，`suite_access_token` **必须**有推送来的 `suite_ticket`，否则在换令牌阶段就抛 `InvalidOperationException`（§2.2）。注意声明式客户端不会把两者混注——`[Token]` 已钉死 TokenType 与 Query 参数名（ACCT3）；真正会混的是宿主自行拼 URL 或直接读缓存令牌的场景，那类代码不在 SDK 保护范围内 |
| `GET` 动词 | 本端点确为 GET（ACCT1 断言 `typeof(GetAttribute)`），但语义是**写**（置位模板的升级开关），且无请求体 | 按 REST 直觉当幂等查询放进轮询/重试逻辑；或照本域 18 支 POST 的写法给它补一个 body |
| 「新授权企业」的判定时点 | 注释只写「只对调用该接口后授权的企业生效，不影响已授权企业」，未定义「后」的精确边界（以调用成功响应时刻还是官方受理时刻） | 在授权流程进行中并发调用，可能落在边界之外；边界未核验（§7）⇒ 按「调用完成后的下一次授权」保守假设，并用接口二逐企业复核 |
| 本族的 DTO 面 | 族内**没有任何专属 DTO**：无请求体，响应为裸 `WechatWorkResponse`（登记在 Common 上下文，不在 `AccountIdJsonContext`） | 去 `DataModels/AccountId/` 找 `UpgradeChatId...Request/Response` 会找不到；照兄弟族写请求体反序列化代码是空转 |
| 套件通道的回调配置 | `WechatCallbackOptions` 拒绝「自建应用占用套件通道」，授权族（含 `suite_ticket`）仅第三方/代开发的**套件通道**许可分发 | 把代开发应用配成应用数据通道 ⇒ `suite_ticket` 事件被族闸拒绝（返回 200、不重推），票据永不上库，本族调用稳定抛 `InvalidOperationException`，且回调侧看不出错误 |

另外三条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 为已接受风险，无法改用 Header）。`suite_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。
- **`suite_ticket` 是凭据**：不得进日志、遥测或异常消息（仓内 AGENTS.md §8 口径；`WechatCallbackEvent.SuiteTicket` 明确列入不得外泄清单）。上面 §2.2 片段打印的 `InvalidOperationException.Message` 只含 `SuiteId`，不含票据值——换打印其它异常前先复核。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（**不代表存量企业被影响**，见 E1 注意事项） | 正常路径 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本端点的族别失效码，命中即走令牌自愈链路，业务侧通常看不到 |
| 61028 | `InvalidSuiteTicket` | `suite_ticket` 失效 | 本族特有风险的信号：票据由官方推送，若推送中断或被旧值覆盖，换取套件令牌会命中此码；自愈链路仍需一份**新的**票据 ⇒ 回查回调接入与存储 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 族失效/过期 | 本端点不消费 `access_token`；出现即凭据族别错配的信号（多为误调 ID 转换族那两支） |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效 | 本端点不消费该令牌（服务商级 ≠ 套件级） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷（未 `AddWechatApp`、未 `AddAccountIdApi()`，或套件令牌链未装配） |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`。注意 **`61028` 不在该集合内**：套件票据失效无法靠「重新取一次」恢复（本仓无获取票据的端点），必须由回调推送新票据或宿主自行写库。

本族还有第二类完全不同的失败形态，**不走 `errcode`**：`suite_ticket` 尚未入库时，SDK 在换令牌阶段就抛 `InvalidOperationException`（请求未发出），处置见 §2.2。

本端点专属的业务错误码，接口注释未留档、本文亦未向官方页取证 ⇒ **未逐项核验**，见 §7。其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（文档号 90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方页面最后更新日期 | 本次未重取官方页（任务纪律：官方取证由文档负责人串行完成），正文继承本域 2026-10-18 逐端点核验留档 ⇒ **未逐项核验** | 99601 官方页渲染取文后回填元信息表 |
| `suite_ticket` 推送节奏的官方原文 | 仓内**两处**文本一致（`WechatSuiteTicketProvider` 异常文案「微信每 10 分钟推送一次」、`WechatCallbackEventTypes` 的 `suite_ticket` 常量注释「每 10 分钟一次」），但两者都是**仓内文本**而非官方取文；官方页原文、是否可变、断推后的最长空窗本文**未重取** ⇒ **未逐项核验**，本文不据此推算刷新节奏 | 99601 所属的第三方接口总览与套件票据官方页 |
| `get_suite_token` 的官方文档号 | 本仓只在 `IWechatWorkProviderAuthentication` 上留档路由 `POST /cgi-bin/service/get_suite_token`，**未留档其官方文档号** ⇒ 本文不给该页链接、不填凭印象的文档号 ⇒ **未逐项核验** | Authentication 域接口注释补齐后回填；§2.2 的前置说明不依赖该号 |
| 套件令牌有效期与刷新阈值 | `SuiteTokenManager` 按 `get_suite_token` 响应的 `expires_in` 走通用刷新，具体数值与提前刷新阈值不属本域契约 ⇒ 本域**未逐项核验** | Authentication 域与其守卫 |
| 本端点幂等性 / 可否撤销 / 重复调用行为 | 注释无表述，且本域无「查询升级开关状态」的端点（ACCT1 路由表即全集）⇒ **未逐项核验** | 99601 页；核验前先不要在 SDK 侧写「重复调用安全」的假设 |
| 「调用之后新授权」的精确判定时点 | 注释只写「只对调用该接口后授权的企业生效」，未定义边界（受理时刻 vs 响应时刻）⇒ **未逐项核验** | 99601 页 + 与接口一/接口二的顺序关系 |
| `upgrade_time` 的单位与量纲（接口一） | 与本族同页的 `ApplyToUpgradeChatIdRequest` 建模为 `int?`，注释只写「完成升级的时间戳」「不得早于当前时间或 7 天之后」，秒/毫秒未留档 ⇒ **未逐项核验**（域 README 缺口表同款） | [ApplyToUpgradeChatIdRequest.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/ApplyToUpgradeChatIdRequest.cs) 与 99601 示例 |
| 接口二的批量上限越界行为 | 域 README §4 留档 ≤100，但超限是整批拒绝还是部分处理未记 ⇒ **未逐项核验** | 99601 页；本域 [IdConvert 代开发子接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService_Provider.cs) |
| 本端点的独立频率数值 | 注释无 QPS/日配额 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 本端点专属错误码集合 | 注释未留档错误码表；`61028` 是仓内码表命中本族的唯一相关项 ⇒ 其余 **未逐项核验** | 官方附录「全局错误码」（90455）逐端点核对后补接口 remarks |
| 内部证据不一致（ACCT4 的族名注释） | [ACCT4](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs) 的类型清单把 `typeof(ChatIdConvertItem)` 注在「群 ID 升级（新授权企业）族」名下，而本族唯一端点**无请求体、响应为裸 `WechatWorkResponse`**，不消费该类型；`ChatIdConvertItem` 实为 ID 转换族接口二 `ConvertChatIdResponse` 的元素 ⇒ 断言本体（该类型必须登记）不受影响，属注释归属错位，**未逐项核验孰为本意** | ACCT1 路由表（本族恰 1 条 GET）+ `ConvertChatIdResponse` 声明 |
| 内部证据不一致（域 README 的非 AccessToken 子接口计数） | [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) §1 归属域表格写「六个 ProviderAccessToken / SuiteAccessToken 子接口 **不得声明**」，而 ACCT3 的 provider 组子接口恰 6 个、suite 组子接口恰 1 个（本族代开发子接口），合计 **7** 个 ⇒ 该「六个」只在「仅统计 ProviderAccessToken 子接口」时成立，文案把 Suite 子接口一并计入则偏少一项 ⇒ **未逐项核验孰为本意** | ACCT3 的三个接口数组（权威）＋ TO1 的非 AccessToken 分支 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、回调接入三篇尚未成文，本文相关处以内联官方链接与源码链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + §1 三种令牌路由键、§7 GET 分布、§8 官方一页多端点拆分）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)（本文引用不重抄）
- 同官方页（99601）另两支的落点：[IdConvert/IWechatWorkAccountIdService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService_Provider.cs)，族文档见 [IdConvert.md](./IdConvert.md)（同域 ID 转换族，含 `ApplyToUpgradeChatIdAsync` 与 `ConvertChatIdAsync` 两支及企业作用域要求）
- 同令牌族对照（`provider_access_token`，服务商级、同样免企业作用域）：[Corpid.md](./Corpid.md) · [Bot.md](./Bot.md)
- 反向换算族（`access_token` + `Wechat.AccessToken@Internal`，仅自建应用）：[Interop.md](./Interop.md)
- `suite_ticket` 的接收与入库：[WechatCallbackHandler.cs](../../../../../Src/Work/Mud.Wechat.Work.Callback/WechatCallbackHandler.cs) · [回调包 README](../../../../../Src/Work/Mud.Wechat.Work.Callback/README.md) · [域回调守卫 WechatCallbackContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/)
- 套件令牌换取链：[SuiteTokenManager.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/SuiteTokenManager.cs) · [WechatSuiteTicketProvider.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/WechatSuiteTicketProvider.cs) · [IWechatSuiteTicketStore.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/IWechatSuiteTicketStore.cs)
- 归属域与令牌错配的 fail-fast 契约：[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）
- 凭据与多应用作用域：`guide/配置与凭据.md`（**尚未成文**）
- 官方页：[群 ID 升级（代开发 99601）](https://developer.work.weixin.qq.com/document/path/99601)
- 套件令牌签发端点（非本域，仅供定位令牌链）：[IWechatWorkProviderAuthentication.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/Interfaces/IWechatWorkProviderAuthentication.cs) 上的 `POST /cgi-bin/service/get_suite_token`。其官方文档号本仓未留档，**不得**在文档里凭印象填写 ⇒ 见 §7。
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
