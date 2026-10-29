# 智能机器人 userid 转换（未明确企业身份场景）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Bot/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Bot/) |
| DTO 源文件 | [ServiceUserIdToOpenUserIdRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/ServiceUserIdToOpenUserIdRequest.cs) · [ServiceUserIdToOpenUserIdResponse](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/ServiceUserIdToOpenUserIdResponse.cs) · [OpenUserIdMapItem](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/OpenUserIdMapItem.cs) |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记） |
| 官方文档 | 第三方 97062 · 服务商代开发 97106（页名「userid的转换」，正文段「未明确企业身份场景」）· 页面最后更新 2026-05-21 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（官方页为浏览器渲染后取文；内部证据为守卫 + 接口/DTO XML 注释逐项比对） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）逐字编译通过，0 错误。两轮编译各暴露一处本文缺陷并已修：① `AddWechatApp` 落 `Mud.Wechat.Work.Abstractions`（原示例 using 不足）；② 示例引用未声明的自由变量 `encryptedUserIds` / `sourceBotId`（现已在示例内落声明）。仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 上下文登记 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) 逐端点核验 2026-10-18 · 官方：本次渲染取文 97062 与 97106（2026-10-29），非继承 |

## 1. 这个接口族做什么

把**企业主体下的加密 userid** 转换成**服务商主体下的 open_userid**，服务于智能机器人的「未明确企业身份场景」——机器人收到消息时只知道企业主体下的加密 userid，而服务商侧的业务库以自身主体的 `open_userid` 为主键，必须先换算才能定位到「这是哪个用户」。

调用凭证是**服务商级**的 `provider_access_token`，不是企业级 `access_token`：本接口不针对某一家授权企业，而是以服务商主体身份做一次 ID 换算，返回中还带上「机器人所在企业 ID」（`open_corpid`），供调用方判断这批 id 属于谁。

**不该用本族的场景**：你已经知道是哪个授权企业、手里拿的是明文 `userid` —— 那属于「已明确企业身份场景」，走同域 ID 转换族的 `UserIdToOpenUserIdAsync`（`access_token` 鉴权，明文 → 密文）。反过来要把服务商主体 id 还原成企业主体 id，官方只对企业自建应用开放，走自建对接族。详见 §5 对照表。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` 或 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败，不留到首次调用） |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken`（常量值 `Wechat.ProviderAccessToken`）→ 官方参数 `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |

`provider_access_token` 由 SDK 侧的 `ProviderTokenManager` 以 `CorpId + ProviderSecret` 自动换取、缓存并按 `expires_in`（官方正常 7200 秒）刷新，**开发者不接触令牌值**。官方对 `get_provider_token` 明确警示「不能频繁调用，否则会受到频率拦截」——不要把自行取令牌作为兜底手段，交给 SDK。

### 2.2 权限与业务前置

官方页（97062 / 97106，2026-05-21 版）的「权限说明」两条，缺任一条都会拿到 `errcode = 0` 但结果为空的静默失败：

- 传入的 `open_userid` 需要在**智能机器人的可见范围**内；
- 智能机器人所在企业需要**已安装该服务商的第三方应用或代开发应用**，且传入的 `open_userid` 需要在**已安装应用的可见范围**内。

`source_botid` 填企业智能机器人 ID（官方必填）。该 ID 的来源链（机器人回调帧 / 智能机器人开发文档）不在本域核验范围，见 §7。

### 2.3 官方开放的应用类型

官方只对**第三方应用**与**服务商代开发**开放本端点，对企业自建应用无此接口。因此本族的端点声明在公共父接口上，第三方 / 代开发子接口是空标记，**没有** `_Internal` 子接口——自建应用调不到、也不该去找它。

> 本族的令牌是服务商级，不依赖 `suite_ticket`（套件令牌才需要，且 `suite_ticket` 只能由官方定时推送、不能主动获取）。若同一应用还要消费 `suite_access_token` 族接口，则必须另行接入回调接收 `suite_ticket`。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdBotService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载唯一端点 | 1（E1） | 否 |
| `IWechatWorkThirdPartyAccountIdBotService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1） | 是 |
| `IWechatWorkProviderAccountIdBotService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`
- 端点索引：**E1** `ServiceUserIdToOpenUserIdAsync`（企业主体加密 userid → 服务商主体 open_userid）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，`RegistryGroupName` 恒空，仅作契约基座）。

---

## 4. 端点

### E1. `ServiceUserIdToOpenUserIdAsync` — 企业主体加密 userid 转服务商主体 open_userid

**签名**

```csharp
Task<ServiceUserIdToOpenUserIdResponse> ServiceUserIdToOpenUserIdAsync(
    [Body] ServiceUserIdToOpenUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/batch/userid_to_openuserid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/batch/userid_to_openuserid?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发（自建应用官方未开放） |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 锁定登记） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OpenUserIdList` | `open_userid_list` | `List<string>?` | 是 | **企业主体下的加密 userid**，最多不超过 1000 个（官方 97062/97106）。字段名字面写的是 `open_userid`，承载的却是企业主体加密 userid——照官方原文，不是笔误 |
| `request.SourceBotId` | `source_botid` | `string?` | 是 | 企业智能机器人 ID（官方 97062/97106） |
| —（不进方法签名） | `provider_access_token` | — | 是 | 应用服务商的接口调用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`OpenUserIdList` 为 `null` 或超过 1000 条都照样发出，错误以 `errcode` 返回。分批是调用方职责。

```json
{
  "open_userid_list": ["wojigengoegeojgoe", "wosgjeiogng"],
  "source_botid": "BOTID"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `open_corpid` | `OpenCorpId` | `string?` | 智能机器人所在企业 ID |
| `items` | `Items` | `List<OpenUserIdMapItem>?` | ID 转换结果 |
| `items[].userid` | `UserId` | `string?` | 企业主体下的加密 userid（回显入参值） |
| `items[].open_userid` | `OpenUserId` | `string?` | 服务商主体下的加密 userid |
| `invalid_open_userid_list` | `InvalidOpenUserIdList` | `List<string>?` | 无法转换的列表；**如果传入的是明文 userid，也会落在这个列表里**（官方原文） |

`OpenUserIdMapItem` 是三个 userid/open_userid 转换端点共用的结果元素，本端点里 `userid` = 企业主体加密 userid，`open_userid` = 服务商主体加密 userid；在「已明确企业身份场景」的对应端点里 `userid` 是明文——同一类型、两种语义，按端点区分。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "items": [
    { "userid": "woxxxx", "open_userid": "wonewxxxx" }
  ],
  "open_corpid": "wpxnigenogneg",
  "invalid_open_userid_list": ["userid", "userid1"]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                      // 可注入子接口 + AddWechatWorkServices / AddAccountIdApi
using Mud.Wechat.Work.Abstractions;         // AddWechatApp（扩展方法落此命名空间，易漏）
using Mud.Wechat.Work.Abstractions.Enums;   // WechatAppType
using Mud.Wechat.Work.DataModels.AccountId; // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，Microsoft.Extensions.DependencyInjection（GetRequiredService）与
// Microsoft.Extensions.Logging（logger 扩展方法）由 Web SDK 隐式 using 提供，无需显式引入。

// ① 注册服务商应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
//    本族只用 provider_access_token，但 Validate() 对第三方/代开发要求四项齐备，缺 SuiteSecret 会启动期即抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.Provider;      // 第三方应用改填 WechatAppType.ThirdParty
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配，无需额外注册令牌链）。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析子接口并调用（多应用部署时先切作用域，见 guide/配置与凭据）。
var bot = provider.GetRequiredService<IWechatWorkProviderAccountIdBotService>();

// 待转换的「企业主体加密 userid」来自机器人回调帧；与机器人 ID 一并按业务实际填充。
var encryptedUserIds = new List<string>();
var sourceBotId = "<企业智能机器人 ID>";

var response = await bot.ServiceUserIdToOpenUserIdAsync(
    new ServiceUserIdToOpenUserIdRequest
    {
        OpenUserIdList = encryptedUserIds.Take(1000).ToList(),  // 官方上限 1000，超出须自行分批
        SourceBotId = sourceBotId,
    },
    cancellationToken);

// ④ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
//    不判就是把空集合当成功。
if (!response.IsSuccess)
{
    logger.LogError("userid 转换失败 {ErrorCode}: {ErrorMessage}",
        response.ErrorCode, response.ErrorMessage);
    throw new InvalidOperationException($"userid 转换失败：{response.ErrorCode}");
}

// ⑤ errcode == 0 也可能是「整批一条都没转成」：逐条对账，别只判 errcode。
//    最常见成因是传了明文 userid，或 open_userid 不在机器人/已安装应用的可见范围内。
var converted = response.Items is null
    ? Array.Empty<(string EnterpriseUserId, string ServiceProviderUserId)>()
    : response.Items.Select(i => (i.UserId!, i.OpenUserId!)).ToArray();

if (converted.Length == 0)
    logger.LogWarning("整批未转换，invalid 列表：{Invalid}",
        string.Join(",", response.InvalidOpenUserIdList ?? new List<string>()));

logger.LogInformation("机器人所在企业 {OpenCorpId}，转换 {Count} 条",
    response.OpenCorpId, converted.Length);
```

**本端点注意事项**

- 单次上限 1000 条（官方 97062/97106）。SDK 侧不校验、不代分批：超限请求会原样发出，官方在超出上限时的具体失败形态本文未取证，见 §7 缺口——需要处理大批量时由调用方自行切片。
- 返回值是**服务商主体视角**的 id（`items[].open_userid`）与机器人所在企业 ID（`open_corpid`）。官方页未描述本接口的幂等性、副作用与映射有效期，因此不要把「映射永久稳定」当默认前提——需要长期存储映射关系时，按 §7 的缺口项自行评估。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 「已明确企业身份场景」`IWechatWorkAccountIdService.UserIdToOpenUserIdAsync`（同域 ID 转换族） | 路由 `/cgi-bin/batch/userid_to_openuserid`（**无 `service/` 前缀**）、`access_token` 鉴权、入参是**明文** `userid_list`、方向为明文 → 密文 | 两条路由尾段同名，极易复制错：拿企业令牌调本族路由或反之，均为凭据类错误 |
| `IWechatWorkInternalAccountIdInteropService.OpenUserIdToUserIdForBotAsync`（自建对接族） | 方向相反（服务商主体 → 企业主体）、`/cgi-bin/batch/openuserid_to_userid`、`access_token`、**仅自建应用** | 把它当本端的失败兜底 = 换了个方向，仍拿不到目标 id |
| 把明文 `userid` 传进 `open_userid_list` | 官方**不报错**，`errcode` 仍为 0，明文值整批落 `invalid_open_userid_list` | 业务侧表现为「全部转换失败但调用成功」，日志无异常，最难定位 |
| 只判 `errcode == 0` 就认为成功 | 本端点是**部分成功**语义：`items` 与 `invalid_open_userid_list` 可同时非空 | 缺量的 id 被静默丢弃，下游按「已转换」处理导致主键对不上 |
| 用自建应用（`WechatAppType.Internal`）配置调本族 | SDK 不为自建应用提供 `_Internal` 子接口（ACCT2 锁定本族恰两分支接口）；官方权限说明要求「机器人所在企业已安装服务商的第三方应用或代开发应用」 | DI 解析不到可注入服务；自建应用没有通向本端点的凭据路径（`provider_access_token` 需服务商 `ProviderSecret`） |

另外三条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 为已接受风险，无法改用 Header）。`provider_access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，异常消息里的 `RequestUri` 在构造期剥离 query 与 userinfo——但你自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判或显式调用 `WechatWorkException.ThrowIfFailed(response)`。
- **配置互斥必填**：`WechatAppConfig.Validate()` 对 `ThirdParty` / `Provider` 要求 `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` 四项齐备。只用服务商凭证面也得配齐套件两项，否则启动期 `InvalidOperationException`。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（**不代表全部转换成功**，见 §5） | 正常路径 |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效/过期 | 本端点的族别失效码，走恢复链路自愈 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 族失效/过期 | 本端点不消费 `access_token`；出现即凭据族别错配的信号，回查应用配置与所调接口 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本端点不消费该令牌 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷 |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`，命中后的链路是「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」，业务侧通常看不到这些码。**若 42011 稳定出现，说明刷新本身在失败**，可依次排查：`ProviderSecret` 是否正确、服务商企业是否已开通相应能力、官方侧是否对 `get_provider_token` 做了频率拦截（官方对该换取接口明确警示「不能频繁调用」）。加重试不解决这三类成因。

本端点专属的业务错误码官方未在本页列出，其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（文档号 90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文，见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `source_botid` 的取值来源链 | 本域只消费机器人 ID，其签发来源（智能机器人回调帧）属 Aibot 邻域 ⇒ **未逐项核验** | `Interfaces/Aibot/README.md`、Aibot 域文档与守卫 |
| 本端点的独立频率上限 | 官方 97062/97106 页未公布本端点频率数值（仅 `get_provider_token` 有「不能频繁调用」警示）⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 「出错次数过多导致 1 天不可调用」是否适用本端 | 该限制写在官方同页的「**已明确企业身份场景**」段落，「未明确企业身份场景」段落未列 ⇒ **不作为本端点契约断言** | 97062/97106 同页两段对照 |
| 第三方锚点不一致 | 接口 XML 注释与域 README 以 **96516** 为第三方锚点，96516 实为「企业微信账号ID安全性全面升级」**总览页**；本端点参数表正文在 **97062**（第三方 tab）⇒ 本文按 97062 取证。若要统一锚点，须同批改接口注释与域 README | 本次渲染取证（2026-10-29） |
| 超过 1000 条时的官方行为 | 官方页只写「最多不超过 1000 个」，未说明超限是整批拒绝还是部分处理 ⇒ **未逐项核验** | 实测或官方附录「访问频率限制」（90454） |
| 本接口的幂等性 / 副作用 / 映射有效期 | 官方页无相关表述 ⇒ **未逐项核验**，本文不作假设性承诺 | 邻域智能机器人方案与 ID 安全性升级总览页（96516） |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 契约陷阱汇总）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)
- 兄弟族：[IdConvert.md](./IdConvert.md)（已明确企业身份场景的 ID 转换族）· [Interop.md](./Interop.md)（自建应用对接族）· [Corpid.md](./Corpid.md)（同为 `provider_access_token` 的 corpid 转换族）
- 智能机器人邻域（消费本族产出的下游）：`Interfaces/Aibot/`
- 凭据与多应用作用域：`guide/配置与凭据.md`（未成文）
- 官方页：[userid的转换（第三方 97062）](https://developer.work.weixin.qq.com/document/path/97062) · [userid的转换（代开发 97106）](https://developer.work.weixin.qq.com/document/path/97106) · [获取服务商凭证（91200）](https://developer.work.weixin.qq.com/document/path/91200)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
