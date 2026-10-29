# PlatsolutionEcommerceCompensate 域 · 平台服务商解决方案 · 平台商户赔付

微信支付「平台服务商解决方案 · 平台商户赔付」域 SDK（5 端点）。 接口：[IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs)；注册组 `PlatsolutionEcommerceCompensate`；模块枚举 `PayModule.PlatsolutionEcommerceCompensate`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceCompensateContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceCompensateContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceCompensate`](../../DataModels/PlatsolutionEcommerceCompensate)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceCompensateService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceCompensateService` | `CreateCompensateAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/compensate-bills` | 商户证书 / APIv3 签名 | 请求赔付。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504175（【平台商户】，官方页更新于 2025.07.25）。 |
| 2 | `IWechatPayPlatsolutionEcommerceCompensateService` | `PreTransferCompensateAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/pre-transfer` | 商户证书 / APIv3 签名 | 请求赔付预下单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504176（【平台商户】，官方页更新于 2025.07.25… |
| 3 | `IWechatPayPlatsolutionEcommerceCompensateService` | `CancelCompensateAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}/cancel` | 商户证书 / APIv3 签名 | 请求撤销赔付。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504177（【平台商户】，官方页更新于 2025.07.25）… |
| 4 | `IWechatPayPlatsolutionEcommerceCompensateService` | `QueryCompensateByOutBillNoAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}` | 商户证书 / APIv3 签名 | 商户单号查询赔付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504178（【平台商户】，官方页更新于 2025.07… |
| 5 | `IWechatPayPlatsolutionEcommerceCompensateService` | `QueryCompensateByBillIdAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/bill-id/{billId}` | 商户证书 / APIv3 签名 | 微信支付单号查询赔付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504179（【平台商户】，官方页更新于 2025.… |

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

**「预下单 / 下单 / 撤销 / 两路查询」四态各建一型请求**：预下单应答含 `package_info`（拉起用户确认用）， 撤销应答含 `cancel_accept_time` / `state` ⇒ 两支**不同形**，不得因同为 4 行而共用。

**两支查询应答逐字段同形**（18 行）⇒ 按商户单号与按微信支付单号共用 `CompensateQueryResponse`。

**撤销支官方无请求体**：定位键 `out_bill_no` 全在 path ⇒ 声明式签名不带 `[Body]`，不得凭空造请求 DTO。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**频率限制**（官方原样）：接口频率限制为100次/s
- （出现 5 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 1. `CreateCompensateAsync` — 请求赔付。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504175（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/compensate-bills`

- **请求体**：`CompensateCreateRequest` — 请求赔付请求体，字段见 `CompensateCreateRequest`。
- **返回**：`CompensateCreateResponse` — 请求赔付，见 `CompensateCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/compensate-bills`；无 path / query 参数。
- **应答**见 `CompensateCreateResponse`（官方应答表逐字段建模）。

### 2. `PreTransferCompensateAsync` — 请求赔付预下单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504176（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/compensate-bills/pre-transfer`

- **请求体**：`CompensatePreTransferRequest` — 请求赔付预下单请求体，字段见 `CompensatePreTransferRequest`。
- **返回**：`CompensatePreTransferResponse` — 请求赔付预下单，见 `CompensatePreTransferResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/pre-transfer`；无 path / query 参数。
- **应答**见 `CompensatePreTransferResponse`（官方应答表逐字段建模）。

### 3. `CancelCompensateAsync` — 请求撤销赔付。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504177（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
- **返回**：`CompensateCancelResponse` — 请求撤销赔付，见 `CompensateCancelResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{out_bill_no}/cancel`；path `out_bill_no`。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。

### 4. `QueryCompensateByOutBillNoAsync` — 商户单号查询赔付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504178（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
- **返回**：`CompensateQueryResponse` — 商户单号查询赔付结果，见 `CompensateQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{out_bill_no}`；path `out_bill_no`。

### 5. `QueryCompensateByBillIdAsync` — 微信支付单号查询赔付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504179（【平台商户】，官方页更新于 2025.07.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/compensate-bills/bill-id/{billId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billId` | Path | `string` | 【微信支付单号】 微信支付单号（官方 path `bill_id`，**必填**，类型 `string(64)`）。 |
- **返回**：`CompensateQueryResponse` — 微信支付单号查询赔付结果，见 `CompensateQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/compensate-bills/bill-id/{bill_id}`；path `bill_id`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceCompensate`](../../DataModels/PlatsolutionEcommerceCompensate) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `PlatsolutionEcommerceCompensate`。

AOT 源生成上下文：[`PlatsolutionEcommerceCompensateJsonContext.g.cs`](./../../DataModels/Generated/PlatsolutionEcommerceCompensateJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CompensateCreateResponse` | 应答 | [PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs](../../DataModels/PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs) | `transaction_id`, `type`, `transaction_info`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sponsor_mchid`, `transfer_scene_id`, `user_recv_perception`, `sp_mchid`, `receiver_detail` … |
| `CompensateCreateRequest` | 请求体 | [PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs](../../DataModels/PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs) | `transaction_id`, `type`, `transaction_info`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sponsor_mchid`, `transfer_scene_id`, `user_recv_perception`, `sp_mchid`, `receiver_detail` … |
| `CompensatePreTransferResponse` | 应答 | [PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs](../../DataModels/PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs) | `transaction_id`, `type`, `transaction_info`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sponsor_mchid`, `transfer_scene_id`, `user_recv_perception`, `sp_mchid`, `receiver_detail` … |
| `CompensatePreTransferRequest` | 请求体 | [PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs](../../DataModels/PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs) | `transaction_id`, `type`, `transaction_info`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sponsor_mchid`, `transfer_scene_id`, `user_recv_perception`, `sp_mchid`, `receiver_detail` … |
| `CompensateCancelResponse` | 应答 | [PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs](../../DataModels/PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs) | `transaction_id`, `type`, `transaction_info`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sponsor_mchid`, `transfer_scene_id`, `user_recv_perception`, `sp_mchid`, `receiver_detail` … |
| `CompensateQueryResponse` | 应答 | [PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs](../../DataModels/PlatsolutionEcommerceCompensate/PlatsolutionEcommerceCompensateDtos.cs) | `transaction_id`, `type`, `transaction_info`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `sponsor_mchid`, `transfer_scene_id`, `user_recv_perception`, `sp_mchid`, `receiver_detail` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceCompensate));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceCompensateService>();
var ct = CancellationToken.None;
var response = await service.CancelCompensateAsync("", ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}/cancel");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceCompensateService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CancelCompensateAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceCompensateService](./IWechatPayPlatsolutionEcommerceCompensateService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceCompensateContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceCompensateContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceCompensateTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceCompensateEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceCompensateDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceCompensateDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`PlatsolutionEcommerceInsuranceClaimTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceInsuranceClaimEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceInsuranceClaimDtos_ShouldExposeOfficialFieldRosters` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceCompensate`](../../DataModels/PlatsolutionEcommerceCompensate) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceCompensateContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayPlatsolutionEcommerceInsuranceClaimContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
