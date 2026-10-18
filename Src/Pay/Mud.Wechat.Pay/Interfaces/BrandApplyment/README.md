# BrandApplyment 域 · 品牌入驻申请

微信支付「品牌入驻申请」域 SDK（4 端点）。 接口：[IWechatPayBrandApplymentService](./IWechatPayBrandApplymentService.cs)；注册组 `BrandApplyment`；模块枚举 `PayModule.BrandApplyment`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBrandApplymentContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandApplymentContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/BrandApplyment`](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBrandApplymentService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBrandApplymentService` | `SubmitBrandApplymentAsync` | POST | `/v3/brand/applyments` | 商户证书 / APIv3 签名 | 品牌入驻申请提交。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016249989（【普通服务商】）。 |
| 2 | `IWechatPayBrandApplymentService` | `CancelBrandApplymentAsync` | POST | `/v3/brand/applyments/cancel-applyment` | 商户证书 / APIv3 签名 | 品牌入驻申请撤销。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016257700（【普通服务商】）。 |
| 3 | `IWechatPayBrandApplymentService` | `QueryBrandApplymentByApplymentIdAsync` | GET | `/v3/brand/applyments/applyment-id/{applymentId}` | 商户证书 / APIv3 签名 | 品牌入驻申请状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016257685（【普通服务商】）。 |
| 4 | `IWechatPayBrandApplymentService` | `QueryBrandApplymentByBusinessCodeAsync` | GET | `/v3/brand/applyments/business-code/{businessCode}` | 商户证书 / APIv3 签名 | 品牌入驻申请状态（按业务申请编号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016257694（【普通服务商】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径。本域每支路由官方**只印一页**（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
⇒ **不存在**「同路由多页字段表比对」问题，一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域四支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 RATELIMIT_EXCEEDED` /
`FREQUENCY_LIMITED` ⇒ 不得凭第三方 SDK 补数。

**本域零 query、零 `brand_id`**：申请作用域全在包体内（品牌入驻面尚无品牌 ID 可填），两支查询按
`applyment_id` / `business_code` 分列 ⇒ 分页与品牌作用域口径都无从套用。

**两套申请编号**：提交时商户侧自带 `business_code`，受理成功后微信分配 `applyment_id` ⇒ 状态查询有「按申请单号」与「按业务申请编号」**两支**，官方两页应答表**逐字段同形** ⇒ 共用一个响应类型。

**提交与撤销共用应答**：`POST /brand/applyments` 与 `POST /brand/applyments/cancel-applyment` 的官方应答表同为 `applyment_id` / `business_code`，故共用 `BrandApplymentAcceptResponse`。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**支持商户**【普通服务商】。
- （出现 4 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`）⇒ 不得凭第三方 SDK 补数。

### 1. `SubmitBrandApplymentAsync` — 品牌入驻申请提交。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016249989（【普通服务商】）。

- **声明接口**：[IWechatPayBrandApplymentService](./IWechatPayBrandApplymentService.cs)
- **官方契约**：`POST /v3/brand/applyments`

- **请求体**：`BrandApplymentSubmitRequest` — 品牌入驻申请提交请求体，字段见 `BrandApplymentSubmitRequest`。
- **返回**：`BrandApplymentAcceptResponse` — 品牌入驻申请提交，见 `BrandApplymentAcceptResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/applyments`；无 path / query 参数。
- **应答**见 `BrandApplymentAcceptResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持提交申请，请检查是否重入 / 400 PARAM_ERROR 申请资料校验失败 / 403 NO_AUTH 商户权限异常 / 429 RATELIMIT_EXCEEDED 频率限制。

### 2. `CancelBrandApplymentAsync` — 品牌入驻申请撤销。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016257700（【普通服务商】）。

- **声明接口**：[IWechatPayBrandApplymentService](./IWechatPayBrandApplymentService.cs)
- **官方契约**：`POST /v3/brand/applyments/cancel-applyment`

- **请求体**：`BrandApplymentCancelRequest` — 品牌入驻申请撤销请求体，字段见 `BrandApplymentCancelRequest`。
- **返回**：`BrandApplymentAcceptResponse` — 品牌入驻申请撤销，见 `BrandApplymentAcceptResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/applyments/cancel-applyment`；无 path / query 参数。
- **应答**见 `BrandApplymentAcceptResponse`（官方应答表逐字段建模，与 `POST /brand/applyments` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持撤销，请检查是否重入 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

### 3. `QueryBrandApplymentByApplymentIdAsync` — 品牌入驻申请状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016257685（【普通服务商】）。

- **声明接口**：[IWechatPayBrandApplymentService](./IWechatPayBrandApplymentService.cs)
- **官方契约**：`GET /v3/brand/applyments/applyment-id/{applymentId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applymentId` | Path | `string` | 【微信支付申请单号】 1、微信支付分配的申请单号，在调用 提交入驻申请 时返回； 2、数字，不超过 16 个字符。（官方 path `applyment_id`，**必填**，类型 `string(20)`）。 |
- **返回**：`BrandApplymentStateResponse` — 品牌入驻申请状态，见 `BrandApplymentStateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/applyments/applyment-id/{applyment_id}`；path `applyment_id`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

### 4. `QueryBrandApplymentByBusinessCodeAsync` — 品牌入驻申请状态（按业务申请编号）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016257694（【普通服务商】）。

- **声明接口**：[IWechatPayBrandApplymentService](./IWechatPayBrandApplymentService.cs)
- **官方契约**：`GET /v3/brand/applyments/business-code/{businessCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `businessCode` | Path | `string` | 【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号； 3、每个编号对应一个申请单； 4、填写调用 提交入驻申请 时传递的业务申请编号。（官方 path `business_code`，**必填**，类型 `string(128)`）。 |
- **返回**：`BrandApplymentStateResponse` — 品牌入驻申请状态（按业务申请编号），见 `BrandApplymentStateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/applyments/business-code/{business_code}`；path `business_code`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/BrandApplyment`](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/) 内共 **10** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **10** 个，`SerializerClassName` = `BrandApplyment`。

AOT 源生成上下文：[`BrandApplymentJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BrandApplymentJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BrandApplymentAcceptResponse` | 应答 | [BrandApplyment/BrandApplymentDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/BrandApplymentDtos.cs) | `admin_name`, `id_doc_type`, `id_card_number`, `subject_type`, `subject_name`, `unified_social_credit_code`, `brand_name`, `brand_logo`, `certificate`, `name`, `number`, `valid_begin_time` … |
| `BrandApplymentSubmitRequest` | 请求体 | [BrandApplyment/BrandApplymentDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/BrandApplymentDtos.cs) | `admin_name`, `id_doc_type`, `id_card_number`, `subject_type`, `subject_name`, `unified_social_credit_code`, `brand_name`, `brand_logo`, `certificate`, `name`, `number`, `valid_begin_time` … |
| `BrandApplymentCancelRequest` | 请求体 | [BrandApplyment/BrandApplymentDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/BrandApplymentDtos.cs) | `admin_name`, `id_doc_type`, `id_card_number`, `subject_type`, `subject_name`, `unified_social_credit_code`, `brand_name`, `brand_logo`, `certificate`, `name`, `number`, `valid_begin_time` … |
| `BrandApplymentStateResponse` | 应答 | [BrandApplyment/BrandApplymentDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/BrandApplymentDtos.cs) | `admin_name`, `id_doc_type`, `id_card_number`, `subject_type`, `subject_name`, `unified_social_credit_code`, `brand_name`, `brand_logo`, `certificate`, `name`, `number`, `valid_begin_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BrandApplyment;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandApplyment));

var service = app.Services.GetRequiredService<IWechatPayBrandApplymentService>();
var ct = CancellationToken.None;
var response = await service.CancelBrandApplymentAsync(new BrandApplymentCancelRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/brand/applyments/cancel-applyment");
// 占位值仅用于编译示意；业务值须按 IWechatPayBrandApplymentService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CancelBrandApplymentAsync`）；请求体 `BrandApplymentCancelRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBrandApplymentService](./IWechatPayBrandApplymentService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBrandApplymentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandApplymentContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BrandApplymentTopology_ShouldBeOneInterfaceOnOneGroup`、`BrandApplymentEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BrandApplymentDtos_ShouldExposeOfficialFieldRosters`、`BrandApplymentDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/BrandApplyment`](../../../Mud.Wechat.Pay.DataModels/BrandApplyment/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBrandApplymentContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
