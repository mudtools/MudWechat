# RateActivity 域 · 优惠费率活动报名（从业机构面）

微信支付「优惠费率活动报名（从业机构面）」域 SDK（3 端点）。 接口：[IWechatPayRateActivityService](./IWechatPayRateActivityService.cs)；注册组 `RateActivity`；模块枚举 `PayModule.RateActivity`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayRateActivityContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRateActivityContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/RateActivity`](../../DataModels/RateActivity)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayRateActivityService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayRateActivityService` | `CreateRateActivityApplicationAsync` | POST | `/v3/rate-activity/applications` | 商户证书 / APIv3 签名 | 报名费率优惠活动。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698774（【从业机构（银行）】【从业机构（支付机构）】）… |
| 2 | `IWechatPayRateActivityService` | `UpdateRateActivityApplicationAsync` | PATCH | `/v3/rate-activity/applications/id/{applicationId}` | 商户证书 / APIv3 签名 | 修改单据信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698885（【从业机构（银行）】【从业机构（支付机构）】）。 |
| 3 | `IWechatPayRateActivityService` | `QueryRateActivityApplicationStateAsync` | GET | `/v3/rate-activity/applications/id/{applicationId}` | 商户证书 / APIv3 签名 | 根据申请单ID查询申请单结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471722（【从业机构（银行）】【从业机构（支… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 3 支中**3 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 3 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**三支官方全部只印服务商页，身份是【从业机构（银行）】+【从业机构（支付机构）】双标签** （正文「支持商户：【从业机构（银行）】 【从业机构（支付机构）】」）⇒ 两类从业机构共用契约， **不得**按银行/支付机构拆接口或补商户页。

**报名与修改的应答同形 ⇒ 共用一型**：POST `/rate-activity/applications` 与 PATCH `/rate-activity/applications/id/{application_id}` 官方应答表均为单键 `application_id` `string(32)` 必填 ⇒ 共用 `RateActivityApplicationResponse`；GET 查状态支应答为 `application_state` / `sub_mchid` / `reject_parameter` / `reject_reason` / `pass_time` 五键 ⇒ **单建一型**。

**`sub_mchid` 两侧类型不同**：请求侧是 `string(32)`，**应答**侧官方印 `integer` ⇒ 「同名即同型」的直觉在本域会造成反序列化失败，请求与应答类型分别照页。

**修改支的请求体全为选填**：`activity_detail_modification` 与 `additional_information` 官方均标选填， 「至少要给一个」由业务码 `INVALID_REQUEST`（参数符合格式但不符合业务规则）兜 ⇒ SDK **不做**空体拦截， 照页保持两支皆可空。

**重复报名是业务码不是 HTTP 冲突**：官方码原文「`ALREADY_EXISTS`：商户提交过报名申请」（HTTP 400）⇒ 重试前先查状态支；`NO_AUTH`「商户信息不合法」与前置「已完成**开户意愿确认**的商户」同页印发。

**频率限制**：三支官方页**全部未印**端点级数字，只在错误码表给 `429 FREQUENCY_LIMITED` ⇒ 不得补数。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）**支持商户**【从业机构（银行）】【从业机构（支付机构）】。
- （出现 3 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- （出现 3 次）**业务错误码**：400 ALREADY_EXISTS：商户提交过报名申请 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户信息不合法 / 404 NOT_FOUND：申请单不存在 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：系统错误。

### 1. `CreateRateActivityApplicationAsync` — 报名费率优惠活动。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698774（【从业机构（银行）】【从业机构（支付机构）】）。

- **声明接口**：[IWechatPayRateActivityService](./IWechatPayRateActivityService.cs)
- **官方契约**：`POST /v3/rate-activity/applications`

- **请求体**：`RateActivityApplicationCreateRequest` — 报名费率优惠活动请求体，字段见 `RateActivityApplicationCreateRequest`。
- **返回**：`RateActivityApplicationResponse` — 报名费率优惠活动，见 `RateActivityApplicationResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/rate-activity/applications`；无 path / query 参数。
- **应答**见 `RateActivityApplicationResponse`（官方应答表逐字段建模）。

### 2. `UpdateRateActivityApplicationAsync` — 修改单据信息。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698885（【从业机构（银行）】【从业机构（支付机构）】）。

- **声明接口**：[IWechatPayRateActivityService](./IWechatPayRateActivityService.cs)
- **官方契约**：`PATCH /v3/rate-activity/applications/id/{applicationId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applicationId` | Path | `string` | 【申请单编号】微信支付分配的申请单号（官方 path `application_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`RateActivityApplicationUpdateRequest` — 修改单据信息请求体，字段见 `RateActivityApplicationUpdateRequest`。
- **返回**：`RateActivityApplicationResponse` — 修改单据信息，见 `RateActivityApplicationResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/rate-activity/applications/id/{application_id}`；path `application_id`。
- **应答**见 `RateActivityApplicationResponse`（官方应答表逐字段建模，与 `POST /rate-activity/applications` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 3. `QueryRateActivityApplicationStateAsync` — 根据申请单ID查询申请单结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471722（【从业机构（银行）】【从业机构（支付机构）】）。

- **声明接口**：[IWechatPayRateActivityService](./IWechatPayRateActivityService.cs)
- **官方契约**：`GET /v3/rate-activity/applications/id/{applicationId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applicationId` | Path | `string` | 【申请单编号】微信支付分配的申请单号.（官方 path `application_id`，**必填**，类型 `string(32)`）。 |
- **返回**：`RateActivityApplicationStateResponse` — 根据申请单ID查询申请单结果，见 `RateActivityApplicationStateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/rate-activity/applications/id/{application_id}`；path `application_id`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/RateActivity`](../../DataModels/RateActivity) 内共 **10** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **10** 个，`SerializerClassName` = `RateActivity`。

AOT 源生成上下文：[`RateActivityJsonContext.g.cs`](./../../DataModels/Generated/RateActivityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `RateActivityApplicationResponse` | 应答 | [RateActivity/RateActivityApplicationDtos.cs](../../DataModels/RateActivity/RateActivityApplicationDtos.cs) | `apply_material_id`, `apply_material_information`, `activity_id`, `activity_rate`, `activity_apply_information`, `additional_message`, `additional_material`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `activity_detail`, `additional_information` … |
| `RateActivityApplicationCreateRequest` | 请求体 | [RateActivity/RateActivityApplicationDtos.cs](../../DataModels/RateActivity/RateActivityApplicationDtos.cs) | `apply_material_id`, `apply_material_information`, `activity_id`, `activity_rate`, `activity_apply_information`, `additional_message`, `additional_material`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `activity_detail`, `additional_information` … |
| `RateActivityApplicationUpdateRequest` | 请求体 | [RateActivity/RateActivityApplicationDtos.cs](../../DataModels/RateActivity/RateActivityApplicationDtos.cs) | `apply_material_id`, `apply_material_information`, `activity_id`, `activity_rate`, `activity_apply_information`, `additional_message`, `additional_material`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `activity_detail`, `additional_information` … |
| `RateActivityApplicationStateResponse` | 应答 | [RateActivity/RateActivityApplicationStateDtos.cs](../../DataModels/RateActivity/RateActivityApplicationStateDtos.cs) | `application_state`, `sub_mchid`, `reject_parameter`, `reject_reason`, `pass_time` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.RateActivity;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.RateActivity));

var service = app.Services.GetRequiredService<IWechatPayRateActivityService>();
var ct = CancellationToken.None;
var response = await service.CreateRateActivityApplicationAsync(new RateActivityApplicationCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/rate-activity/applications");
// 占位值仅用于编译示意；业务值须按 IWechatPayRateActivityService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateRateActivityApplicationAsync`）；请求体 `RateActivityApplicationCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayRateActivityService](./IWechatPayRateActivityService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayRateActivityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRateActivityContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `RateActivityTopology_ShouldMatchInterfaceAndGroupRoster`、`RateActivityEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`RateActivityDtos_ShouldExposeOfficialFieldRosters`、`RateActivityDataModels_ShouldSplitRequestAndResponseBaseTypes`、`RateActivityAllDtos_ShouldBeAnnotatedAndContextRegistered`、`RateActivityRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/RateActivity`](../../DataModels/RateActivity) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayRateActivityContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
