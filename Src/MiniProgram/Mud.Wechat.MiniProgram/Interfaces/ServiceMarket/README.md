# ServiceMarket 域 · 微信服务市场

微信小程序「微信服务市场」域 SDK（2 端点：调用服务市场接口 + 异步获取处理数据）。 接口：[IWxaServiceMarketService](./IWxaServiceMarketService.cs)；注册组 `ServiceMarket`；模块枚举 `MiniProgramModule.ServiceMarket`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/ServiceMarket`](../../../Mud.Wechat.MiniProgram/ServiceMarket/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaServiceMarketService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaServiceMarketService` | `InvokeAsync` | POST | `/wxa/servicemarket` | `AccessToken`（Query），Query `access_token` | 调用服务市场接口。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/wx-service-marke… |
| 2 | `IWxaServiceMarketService` | `RetrieveAsync` | POST | `/wxa/servicemarketretrieve` | `AccessToken`（Query），Query `access_token` | 异步获取服务市场处理数据。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/wx-service-m… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 微信服务市场，2026-10-10 依据官方清单核验）：
https://developers.weixin.qq.com/miniprogram/dev/server/API/wx-service-market/api_invokeservice、https://developers.weixin.qq.com/miniprogram/dev/server/API/wx-service-market/api_servicemarketretrieve。

**业务形态（官方原文）**：服务市场是面向开发者的增值服务（能力）交易/调用平台；
本接口用于调用服务平台上架的 API（适用于公众号、小程序与第三方平台，区别仅在 access_token 的生成）。
调用 `async=true` 的异步 API 时，先经 `InvokeAsync` 取得 `request_id`，
再经 `RetrieveAsync` 拉取处理后的数据。

**令牌与鉴权**：全部走应用级 `access_token`（Query）。两端点官方均为 **POST**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `InvokeAsync` — 调用服务市场接口。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/wx-service-market/api_invokeservice。

- **声明接口**：[IWxaServiceMarketService](./IWxaServiceMarketService.cs)
- **官方契约**：`POST /wxa/servicemarket`

- **请求体**：`DataModels.ServiceMarket.WxaServiceMarketRequest` — 调用请求（`service` / `api` 必填），见 `WxaServiceMarketRequest`。
- **返回**：`DataModels.ServiceMarket.WxaServiceMarketResponse` — 服务方回包（`data` JSON 字符串；异步场景含 `request_id`），见 `WxaServiceMarketResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/servicemarket` ＋ 请求体 JSON；Query 携带 `access_token`。
- **参数与失败（官方原文）**：`service`（服务 ID）与 `api`（接口名）必填； / `data` 为服务方接口定义的数据对象；服务方业务失败以 `errcode` 非零表达 / （部分服务方错误码与微信 `errcode` 同域）。`data` 可能承载业务敏感数据，禁止落日志。

### 2. `RetrieveAsync` — 异步获取服务市场处理数据。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/wx-service-market/api_servicemarketretrieve。

- **声明接口**：[IWxaServiceMarketService](./IWxaServiceMarketService.cs)
- **官方契约**：`POST /wxa/servicemarketretrieve`

- **请求体**：`DataModels.ServiceMarket.WxaServiceMarketRetrieveRequest` — 拉取请求（`request_id` 必填），见 `WxaServiceMarketRetrieveRequest`。
- **返回**：`DataModels.ServiceMarket.WxaServiceMarketRetrieveResponse` — 异步处理后数据（`data` JSON 字符串），见 `WxaServiceMarketRetrieveResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/servicemarketretrieve` ＋ 请求体 JSON；Query 携带 `access_token`。
- **时机（官方原文）**：调用服务市场接口（`InvokeAsync` 且 `async=true`）后 / 根据返回的 `request_id` 拉取异步处理结果；`data` 为服务方回包 JSON 字符串，可能承载业务敏感数据，禁止落日志。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/ServiceMarket`](../../../Mud.Wechat.MiniProgram/ServiceMarket/) 内共 **4** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **4** 个，`SerializerClassName` = `ServiceMarket`。

AOT 源生成上下文：[`ServiceMarketJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/ServiceMarketJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaServiceMarketResponse` | 应答 | [ServiceMarket/WxaServiceMarketResponse.cs](../../../Mud.Wechat.MiniProgram/ServiceMarket/WxaServiceMarketResponse.cs) | `request_id`, `data` |
| `WxaServiceMarketRequest` | 请求体 | [ServiceMarket/WxaServiceMarketRequest.cs](../../../Mud.Wechat.MiniProgram/ServiceMarket/WxaServiceMarketRequest.cs) | `service`, `api`, `async`, `data`, `client_msg_id` |
| `WxaServiceMarketRetrieveResponse` | 应答 | [ServiceMarket/WxaServiceMarketRetrieveResponse.cs](../../../Mud.Wechat.MiniProgram/ServiceMarket/WxaServiceMarketRetrieveResponse.cs) | `data` |
| `WxaServiceMarketRetrieveRequest` | 请求体 | [ServiceMarket/WxaServiceMarketRetrieveRequest.cs](../../../Mud.Wechat.MiniProgram/ServiceMarket/WxaServiceMarketRetrieveRequest.cs) | `request_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.ServiceMarket;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddServiceMarketApi());

var service = app.Services.GetRequiredService<IWxaServiceMarketService>();
var ct = CancellationToken.None;
var response = await service.InvokeAsync(new WxaServiceMarketRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/servicemarket");
// 占位值仅用于编译示意；业务值须按 IWxaServiceMarketService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`InvokeAsync`）；请求体 `WxaServiceMarketRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaServiceMarketService](./IWxaServiceMarketService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/ServiceMarket`](../../../Mud.Wechat.MiniProgram/ServiceMarket/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
