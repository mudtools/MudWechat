# PartnerPapay 域 · 委托代扣（PAPay 服务商面预扣费通知）

微信支付「委托代扣（PAPay 服务商面预扣费通知）」域 SDK（1 端点）。 接口：[IWechatPayPartnerPapayService](./IWechatPayPartnerPapayService.cs)；注册组 `PartnerPapay`；模块枚举 `PayModule.PartnerPapay`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPapayContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPapayContractGuards.cs)）、本域 DTO（**无独立 DTO 目录**（类型见「DTO 与序列化面」））。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPartnerPapayService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPartnerPapayService` | `SendPartnerPapayDeductNotifyAsync` | POST | `/v3/partner-papay/contracts/{contractId}/notify` | 商户证书 / APIv3 签名 | 预扣费通知（服务商）。<br>官方文档：https://pay.weixin.qq.com/doc/v2/partner/4011988373（【服务商】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中**1 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 1 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**无应答体支的返回形态**：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**服务商面官方只印这一支**：委托代扣 3.0 的服务商开放面只有「预扣费通知」 （`POST /v3/partner-papay/contracts/{contract_id}/notify`）⇒ 签约、查询、扣款各支官方**无服务商页**， 本接口**不得**按直连面镜像补齐（照页判据，缺页即缺端点）。

**字段名与直连面不同**：服务商页包体为 `sp_mchid` / `sp_appid` / `sub_mchid` / `sub_appid`（多子商户两级）， 直连页为 `mchid` / `appid` ⇒ **不同名即不同型**，请求类型独立（`PartnerPapayContractNotifyRequest`）， 不做超集共用。

**应答与幂等口径同直连面**：官方正常示例为「无数据（Http状态码为204）」⇒ 返回裸 `Task`， `RESOURCE_ALREADY_EXISTS` 表示通知已发送成功。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SendPartnerPapayDeductNotifyAsync` — 预扣费通知（服务商）。
官方文档：https://pay.weixin.qq.com/doc/v2/partner/4011988373（【服务商】）。

- **声明接口**：[IWechatPayPartnerPapayService](./IWechatPayPartnerPapayService.cs)
- **官方契约**：`POST /v3/partner-papay/contracts/{contractId}/notify`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 签约成功后，微信返回的委托代扣协议id。 示例值：123124412412423431（官方 path `contract_id`，**必填**，类型 `string[1,32]`）。 |

- **请求体**：`PartnerPapayContractNotifyRequest` — 预扣费通知（服务商）请求体，字段见 `PartnerPapayContractNotifyRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/partner-papay/contracts/{contract_id}/notify`；path `contract_id`。
- **支持商户**【服务商】。
- **频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- **业务错误码**：SYSTEMERROR：接口返回错误 / CONTRACT_NOT_EXIST：签约协议不存在 / PARAM_ERROR：参数错误 / APPID_MCHID_NOT_MATCH：appid和mchid不匹配 / INVALID_REQUEST：无效请求 / RULELIMIT：该请求存在风险 / RESOURCE_ALREADY_EXISTS：资源已经存在。

## DTO 与序列化面

本域**没有独立的 DataModels 子目录**；端点签名直接引用的类型在下方按类型索引回溯，未命中者说明其定义不在 `Mud.Wechat.Pay.DataModels`。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PartnerPapayContractNotifyRequest` | 请求体 | [Papay/PapayContractNotifyDtos.cs](../../../Mud.Wechat.Pay.DataModels/Papay/PapayContractNotifyDtos.cs) | `count`, `unit`, `amount`, `currency`, `mchid`, `appid`, `deduct_duration`, `estimated_amount`, `sp_mchid`, `sp_appid`, `sub_mchid`, `sub_appid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Papay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PartnerPapay));

var service = app.Services.GetRequiredService<IWechatPayPartnerPapayService>();
var ct = CancellationToken.None;
await service.SendPartnerPapayDeductNotifyAsync("", new PartnerPapayContractNotifyRequest(), ct);
// 裸 Task：成功与否按 HTTP 状态码与业务错误码判定（WechatPayException）。
// 占位值仅用于编译示意；业务值须按 IWechatPayPartnerPapayService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SendPartnerPapayDeductNotifyAsync`）；请求体 `PartnerPapayContractNotifyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| DTO 落位 | 本域无独立 `Mud.Wechat.Pay.DataModels/PartnerPapay` 目录，类型散落在共享目录或其它域 | 以类型索引与源文件为准；新增 DTO 时先定落位 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPartnerPapayService](./IWechatPayPartnerPapayService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPapayContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPapayContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`PapayTopology_ShouldMatchInterfaceAndGroupRoster`、`PapayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PapayDtos_ShouldExposeOfficialFieldRosters`、`PapayDataModels_ShouldSplitRequestAndResponseBaseTypes`、`PapayAllDtos_ShouldBeAnnotatedAndContextRegistered`、`PapayRegistrars_ShouldBeWiredThroughPayModuleAndResolver` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | `Mud.Wechat.Pay.DataModels`（无本域子目录） |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayDomainContractGuards.cs`、`WechatPayPapayContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
