# JsSdk 域 · JS-SDK

企业微信「JS-SDK」模块公共 SDK（获取企业 jsapi_ticket + 获取应用 jsapi_ticket，
为 `wx.config`（企业应用鉴权 getConfigSignature）与 `wx.agentConfig`
（应用鉴权 getAgentConfigSignature）两个鉴权入口提供服务端票据换取能力）。 接口：[IWechatWorkJsSdkService](./IWechatWorkJsSdkService.cs)、[IWechatWorkInternalJsSdkService](./IWechatWorkJsSdkService_Internal.cs)、[IWechatWorkProviderJsSdkService](./IWechatWorkJsSdkService_Provider.cs)、[IWechatWorkThirdPartyJsSdkService](./IWechatWorkJsSdkService_ThirdParty.cs)；注册组 `JsSdk`；模块枚举 `WechatModule.JsSdk`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatJsSdkContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatJsSdkContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work.DataModels/JsSdk`](../../../Mud.Wechat.Work.DataModels/JsSdk/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkJsSdkService` | `GetCorpJsapiTicketAsync` | GET | `/cgi-bin/get_jsapi_ticket` | `AccessToken`（Query），Query `access_token` | 获取企业 jsapi_ticket |
| 2 | `IWechatWorkJsSdkService` | `GetAgentJsapiTicketAsync` | GET | `/cgi-bin/ticket/get` | `AccessToken`（Query），Query `access_token` | 获取应用 jsapi_ticket |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkJsSdkService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalJsSdkService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderJsSdkService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyJsSdkService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkJsSdkService`

官方对自建应用、第三方应用与服务商代开发开放完全一致的 2 个端点，全部收敛声明于本接口；
应用类型子接口均为零差异端点空标记：自建应用见 `IWechatWorkInternalJsSdkService`，
第三方应用见 `IWechatWorkThirdPartyJsSdkService`，服务商代开发见 `IWechatWorkProviderJsSdkService`。
JS-SDK 签名算法本身为页面/服务器端约定（对拼接串做 SHA-1），不属于 HTTP 端点面，
官方三份文档（自建 90506 / 第三方 90539 / 代开发 96909）正文逐字一致，
签名规则随端点写入各方法 `<remarks>`。

三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`），
由多应用基座按当前应用上下文（AppKey + scope）路由——
第三方/代开发的企业级令牌须先经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId)`
建立「应用 + 企业」作用域后再调用。

官方约束：获取 jsapi_ticket 的接口有非常严格的调用频率限制
（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次），
开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalJsSdkService`

官方对自建应用开放与公共面一致的 2 个端点（继承自
`IWechatWorkJsSdkService`）；本接口不新增端点，仅作为
自建应用的类型化契约入口存在（形态对齐 `IWechatWorkInternalIdentityService` 空标记）。
服务商代开发见 `IWechatWorkProviderJsSdkService`；
第三方应用见 `IWechatWorkThirdPartyJsSdkService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方约束：
获取 jsapi_ticket 的接口有非常严格的调用频率限制（一小时内，一个企业最多可获取 400 次，
且单个应用不能超过 100 次），开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderJsSdkService`

官方对服务商代开发开放与公共面一致的 2 个端点（继承自
`IWechatWorkJsSdkService`）；本接口不新增端点，仅作为
服务商代开发的类型化契约入口存在（形态对齐 `IWechatWorkProviderIdentityService` 空标记）。
企业自建应用见 `IWechatWorkInternalJsSdkService`；
第三方应用见 `IWechatWorkThirdPartyJsSdkService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`
建立「应用 + 企业」作用域后再调用。官方约束：获取 jsapi_ticket 的接口有非常严格的调用频率限制
（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次），
开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyJsSdkService`

官方对第三方应用开放与公共面一致的 2 个端点（继承自
`IWechatWorkJsSdkService`）；本接口不新增端点，仅作为
第三方应用的类型化契约入口存在（形态对齐 `IWechatWorkProviderIdentityService` 空标记）。
企业自建应用见 `IWechatWorkInternalJsSdkService`；
服务商代开发见 `IWechatWorkProviderJsSdkService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`
建立「应用 + 企业」作用域后再调用。官方约束：获取 jsapi_ticket 的接口有非常严格的调用频率限制
（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次），
开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetCorpJsapiTicketAsync` — 获取企业 jsapi_ticket

- **声明接口**：[IWechatWorkJsSdkService](./IWechatWorkJsSdkService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/get_jsapi_ticket`
- **返回**：`GetCorpJsapiTicketResponse` — 生成签名所需的 jsapi_ticket（ticket，最长 512 字节）与凭证的有效时间（expires_in，秒）。

**补充说明**

- 企业的 jsapi_ticket 是企业页面调用企业微信 JS 接口的临时票据，用于企业应用鉴权 （getConfigSignature，配合 `wx.config` 注入完成 JS-SDK 基础接口的权限校验）。
- 签名算法：参与签名的参数为 jsapi_ticket、noncestr（随机字符串）、timestamp（当前时间戳，单位为秒） 与 url（当前页面的 URL，不包含「#」及后面部分），按 `jsapi_ticket=JSAPI_TICKET&noncestr=NONCESTR&timestamp=TIMESTAMP&url=URL` 顺序拼接后对拼接串做 SHA-1，结果即为 JS 接口签名——只需按上述规则进行拼接， 不要改变参数顺序，不要进行 URL encode；出于安全考虑，官方要求开发者必须在服务器端实现签名的逻辑。

**备注（原文）**

- 频率限制：获取 jsapi_ticket 的接口有非常严格的调用频率限制 / （一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次）， / 开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存； / 正常情况下 jsapi_ticket 的有效期为 7200 秒（2 小时），具体过期时间需要参考接口返回的 expires_in 属性。
- **企业自建应用·JS-SDK 签名算法**SDK文档：https://developer.work.weixin.qq.com/document/path/90506
- **第三方应用·JS-SDK 签名算法**SDK文档：https://developer.work.weixin.qq.com/document/path/90539
- **服务商代开发·JS-SDK 签名算法**SDK文档：https://developer.work.weixin.qq.com/document/path/96909

### 2. `GetAgentJsapiTicketAsync` — 获取应用 jsapi_ticket

- **声明接口**：[IWechatWorkJsSdkService](./IWechatWorkJsSdkService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/ticket/get`
- **返回**：`GetAgentJsapiTicketResponse` — 生成签名所需的 jsapi_ticket（ticket，最长 512 字节）与凭证的有效时间（expires_in，秒）。

**补充说明**

- 应用的 jsapi_ticket 是应用调用企业微信 JS 接口的临时票据，用于应用鉴权 （getAgentConfigSignature，配合 `wx.agentConfig` 注入完成更多 JS-SDK 接口的权限校验）。
- 官方请求地址以固定 Query 参数携带 `type=agent_config`（参数表不再单列）， 本 SDK 以方法级固定 Query 参数发射该值；签名算法与企业 jsapi_ticket 完全一致 （见 `GetCorpJsapiTicketAsync`，同样不要改变参数顺序、不要进行 URL encode， 并须在服务器端实现签名逻辑）。

**备注（原文）**

- 频率限制：官方在「获取企业 jsapi_ticket」节对获取 jsapi_ticket 的接口统一标注 / 非常严格的调用频率限制（一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次）， / 开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存； / 正常情况下 jsapi_ticket 的有效期为 7200 秒（2 小时），具体过期时间需要参考接口返回的 expires_in 属性。
- **企业自建应用·JS-SDK 签名算法**SDK文档：https://developer.work.weixin.qq.com/document/path/90506
- **第三方应用·JS-SDK 签名算法**SDK文档：https://developer.work.weixin.qq.com/document/path/90539
- **服务商代开发·JS-SDK 签名算法**SDK文档：https://developer.work.weixin.qq.com/document/path/96909

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work.DataModels/JsSdk`](../../../Mud.Wechat.Work.DataModels/JsSdk/) 内共 **2** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **2** 个，`SerializerClassName` = `JsSdk`。

AOT 源生成上下文：[`JsSdkJsonContext.g.cs`](./../../../Mud.Wechat.Work.DataModels/Generated/JsSdkJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetCorpJsapiTicketResponse` | 应答 | [JsSdk/GetCorpJsapiTicketResponse.cs](../../../Mud.Wechat.Work.DataModels/JsSdk/GetCorpJsapiTicketResponse.cs) | `ticket`, `expires_in` |
| `GetAgentJsapiTicketResponse` | 应答 | [JsSdk/GetAgentJsapiTicketResponse.cs](../../../Mud.Wechat.Work.DataModels/JsSdk/GetAgentJsapiTicketResponse.cs) | `ticket`, `expires_in` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.JsSdk;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddJsSdkApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalJsSdkService>();
var ct = CancellationToken.None;
var response = await service.GetAgentJsapiTicketAsync(ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/ticket/get");
// 占位值仅用于编译示意；业务值须按 IWechatWorkJsSdkService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAgentJsapiTicketAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkJsSdkService](./IWechatWorkJsSdkService.cs)、[IWechatWorkInternalJsSdkService](./IWechatWorkJsSdkService_Internal.cs)、[IWechatWorkProviderJsSdkService](./IWechatWorkJsSdkService_Provider.cs)、[IWechatWorkThirdPartyJsSdkService](./IWechatWorkJsSdkService_ThirdParty.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatJsSdkContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatJsSdkContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `JsSdkEndpoints_ShouldMatchOfficialRoutes`、`JsSdkInterfaceHierarchy_ShouldConvergeOnJsSdkRegistry`、`JsSdkTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`JsSdkDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work.DataModels/JsSdk`](../../../Mud.Wechat.Work.DataModels/JsSdk/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatJsSdkContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
