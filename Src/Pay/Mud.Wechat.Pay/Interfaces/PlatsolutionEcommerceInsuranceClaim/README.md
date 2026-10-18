# PlatsolutionEcommerceInsuranceClaim 域 · 平台服务商解决方案 · 保险理赔出款

微信支付「平台服务商解决方案 · 保险理赔出款」域 SDK（5 端点）。 接口：[IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs)；注册组 `PlatsolutionEcommerceInsuranceClaim`；模块枚举 `PayModule.PlatsolutionEcommerceInsuranceClaim`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceInsuranceClaimService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceInsuranceClaimService` | `CreateInsuranceClaimAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills` | 商户证书 / APIv3 签名 | 请求保险理赔。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504184（【平台商户】，官方页更新于 2025.07.25）… |
| 2 | `IWechatPayPlatsolutionEcommerceInsuranceClaimService` | `PreTransferInsuranceClaimAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/pre-transfer` | 商户证书 / APIv3 签名 | 请求保险理赔预下单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504185（【平台商户】，官方页更新于 2025.07.… |
| 3 | `IWechatPayPlatsolutionEcommerceInsuranceClaimService` | `CancelInsuranceClaimAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{outBillNo}/cancel` | 商户证书 / APIv3 签名 | 请求撤销保险理赔。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504186（【平台商户】，官方页更新于 2025.07.2… |
| 4 | `IWechatPayPlatsolutionEcommerceInsuranceClaimService` | `QueryInsuranceClaimByOutBillNoAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{outBillNo}` | 商户证书 / APIv3 签名 | 商户单号查询保险理赔结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504187（【平台商户】，官方页更新于 2025.… |
| 5 | `IWechatPayPlatsolutionEcommerceInsuranceClaimService` | `QueryInsuranceClaimByBillIdAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/bill-id/{billId}` | 商户证书 / APIv3 签名 | 微信支付单号查询保险理赔结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504188（【平台商户】，官方页更新于 202… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径。本域每支路由官方**只印一页** ⇒ **不存在**「同路由多页字段表比对」问题，一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**「平台服务商」身份口径**：本族官方各页「支持商户」栏为【平台商户】（保险理赔功能开通族另含【普通服务商】），
请求体统一以 `sub_mchid` / `sponsor_mchid` 指向被操作的二级商户 ⇒ 出资方与被赔付方是两回事，字段不得互相顶替。

**与 `IWechatPayPlatsolutionEcommerceCompensateService` 同路由族不同契约**： 本域 `/insurance-claim-bills` 的应答比赔付域多 `business_type` 等字段、撤销支**官方有请求体** （赔付域撤销无体）⇒ 两域 DTO **不得互套**。

**两支查询应答逐字段同形**（19 行）⇒ 共用 `InsuranceClaimQueryResponse`； 两支查询的 `sub_mchid` 官方为**选填 query** ⇒ 给可空缺省。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**频率限制**（官方原样）：接口频率限制为100次/s
- （出现 5 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 1. `CreateInsuranceClaimAsync` — 请求保险理赔。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504184（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills`

- **请求体**：`InsuranceClaimCreateRequest` — 请求保险理赔请求体，字段见 `InsuranceClaimCreateRequest`。
- **返回**：`InsuranceClaimCreateResponse` — 请求保险理赔，见 `InsuranceClaimCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills`；无 path / query 参数。
- **应答**见 `InsuranceClaimCreateResponse`（官方应答表逐字段建模）。

### 2. `PreTransferInsuranceClaimAsync` — 请求保险理赔预下单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504185（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/pre-transfer`

- **请求体**：`InsuranceClaimPreTransferRequest` — 请求保险理赔预下单请求体，字段见 `InsuranceClaimPreTransferRequest`。
- **返回**：`InsuranceClaimPreTransferResponse` — 请求保险理赔预下单，见 `InsuranceClaimPreTransferResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/pre-transfer`；无 path / query 参数。
- **应答**见 `InsuranceClaimPreTransferResponse`（官方应答表逐字段建模）。

### 3. `CancelInsuranceClaimAsync` — 请求撤销保险理赔。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504186（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{outBillNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |

- **请求体**：`InsuranceClaimCancelRequest` — 请求撤销保险理赔请求体，字段见 `InsuranceClaimCancelRequest`。
- **返回**：`InsuranceClaimCancelResponse` — 请求撤销保险理赔，见 `InsuranceClaimCancelResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{out_bill_no}/cancel`；path `out_bill_no`。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **应答**见 `InsuranceClaimCancelResponse`（官方应答表逐字段建模）。

### 4. `QueryInsuranceClaimByOutBillNoAsync` — 商户单号查询保险理赔结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504187（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`InsuranceClaimQueryResponse` — 商户单号查询保险理赔结果，见 `InsuranceClaimQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/out-bill-no/{out_bill_no}`；path `out_bill_no`，query `sub_mchid`。

### 5. `QueryInsuranceClaimByBillIdAsync` — 微信支付单号查询保险理赔结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504188（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/bill-id/{billId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billId` | Path | `string` | 【微信支付单号】 微信支付单号（官方 path `bill_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`InsuranceClaimQueryResponse` — 微信支付单号查询保险理赔结果，见 `InsuranceClaimQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills/bill-id/{bill_id}`；path `bill_id`，query `sub_mchid`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/) 内共 **18** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **18** 个，`SerializerClassName` = `PlatsolutionEcommerceInsuranceClaim`。

AOT 源生成上下文：[`PlatsolutionEcommerceInsuranceClaimJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PlatsolutionEcommerceInsuranceClaimJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `InsuranceClaimCreateResponse` | 应答 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |
| `InsuranceClaimCreateRequest` | 请求体 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |
| `InsuranceClaimPreTransferResponse` | 应答 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |
| `InsuranceClaimPreTransferRequest` | 请求体 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |
| `InsuranceClaimCancelResponse` | 应答 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |
| `InsuranceClaimCancelRequest` | 请求体 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |
| `InsuranceClaimQueryResponse` | 应答 | [PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/PlatsolutionEcommerceInsuranceClaimDtos.cs) | `transaction_id`, `type`, `transaction_info`, `sub_mchid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sp_mchid`, `receiver_detail`, `bill_id`, `state` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceClaim;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceInsuranceClaim));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceInsuranceClaimService>();
var ct = CancellationToken.None;
var response = await service.CreateInsuranceClaimAsync(new InsuranceClaimCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/mch-transfer/insurance-claim-bills");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateInsuranceClaimAsync`）；请求体 `InsuranceClaimCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceInsuranceClaimService](./IWechatPayPlatsolutionEcommerceInsuranceClaimService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceInsuranceClaimTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceInsuranceClaimEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceInsuranceClaimDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceInsuranceClaimDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceInsuranceClaim/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
