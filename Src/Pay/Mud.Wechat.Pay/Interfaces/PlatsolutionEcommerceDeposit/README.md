# PlatsolutionEcommerceDeposit 域 · 平台服务商解决方案 · 保证金赔付（售后 / 运费 / 保费）

微信支付「平台服务商解决方案 · 保证金赔付（售后 / 运费 / 保费）」域 SDK（7 端点）。 接口：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)；注册组 `PlatsolutionEcommerceDeposit`；模块枚举 `PayModule.PlatsolutionEcommerceDeposit`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceDepositContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceDepositContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceDepositService = 7；按 HTTP 特性实计 **7** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceDepositService` | `CreateDepositAfterSalesCompensationAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills` | 商户证书 / APIv3 签名 | 请求保证金售后赔付。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504194（【平台商户】，官方页更新于 2025.03.… |
| 2 | `IWechatPayPlatsolutionEcommerceDepositService` | `QueryDepositAfterSalesCompensationByOutBillNoAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/out-bill-no/{outBillNo}` | 商户证书 / APIv3 签名 | 商户单号查询保证金售后赔付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504195（【平台商户】，官方页更新于 20… |
| 3 | `IWechatPayPlatsolutionEcommerceDepositService` | `QueryDepositAfterSalesCompensationByBillIdAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/bill-id/{billId}` | 商户证书 / APIv3 签名 | 微信支付单号查询保证金售后赔付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504196（【平台商户】，官方页更新于 … |
| 4 | `IWechatPayPlatsolutionEcommerceDepositService` | `CreateDepositShippingCostCompensationAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills` | 商户证书 / APIv3 签名 | 请求保证金运费赔付。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504202（【平台商户】，官方页更新于 2025.03.… |
| 5 | `IWechatPayPlatsolutionEcommerceDepositService` | `QueryDepositShippingCostCompensationByOutBillNoAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills/out-bill-no/{outBillNo}` | 商户证书 / APIv3 签名 | 商户单号查询保证金运费赔付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504203（【平台商户】，官方页更新于 20… |
| 6 | `IWechatPayPlatsolutionEcommerceDepositService` | `CreateDepositInsurancePremiumsCompensationAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills` | 商户证书 / APIv3 签名 | 请求保证金保费赔付。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014010905（【平台商户】，官方页更新于 2025.03.… |
| 7 | `IWechatPayPlatsolutionEcommerceDepositService` | `QueryDepositInsurancePremiumsCompensationByOutBillNoAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills/{outBillNo}` | 商户证书 / APIv3 签名 | 商户单号查询保证金保费赔付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014010920（【平台商户】，官方页更新于 20… |

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

**一个注册组三笔子族**：售后赔付（下单 + 两路查询）、运费赔付（下单 + 商户单号查询）、保费赔付（下单 + 商户单号查询）⇒ 七支建六个响应类型：售后族两路查询**逐字段同形** ⇒ 共用一型，其余各建一型。**三笔子族的下单应答并非「互不同形」**：运费与保费两支字段集相同（官方表格仅行序不同）、售后支少一个 `jump_wxa_option` ⇒ 本域选择逐子族独立建型是隔离演进的设计决定，不是形态强制。

**售后族两支查询应答逐字段同形**（21 行）⇒ 共用 `DepositAfterSalesQueryResponse`。

**保费族的定位键是裸 path 段**：`.../deposit-insurance-premiums-compensation-bills/{out_bill_no}` （官方**没有** `out-bill-no/` 前缀，与售后/运费两支不同）⇒ **照官方原文，不得「顺手对齐」**。

**保费查询支 `sub_mchid` 官方必填**，而售后/运费查询支同一参数**选填** ⇒ 同一族内必填口径也不统一，照页不得互套。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 7 次）**频率限制**（官方原样）：接口频率限制为100次/s
- （出现 3 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NOT_ENOUGH 资金不足 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。
- （出现 4 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 记录不存在 / 429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快。

### 1. `CreateDepositAfterSalesCompensationAsync` — 请求保证金售后赔付。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504194（【平台商户】，官方页更新于 2025.03.21）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills`

- **请求体**：`DepositAfterSalesCreateRequest` — 请求保证金售后赔付请求体，字段见 `DepositAfterSalesCreateRequest`。
- **返回**：`DepositAfterSalesCreateResponse` — 请求保证金售后赔付，见 `DepositAfterSalesCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills`；无 path / query 参数。
- **应答**见 `DepositAfterSalesCreateResponse`（官方应答表逐字段建模）。

### 2. `QueryDepositAfterSalesCompensationByOutBillNoAsync` — 商户单号查询保证金售后赔付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504195（【平台商户】，官方页更新于 2025.03.21）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/out-bill-no/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`DepositAfterSalesQueryResponse` — 商户单号查询保证金售后赔付结果，见 `DepositAfterSalesQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/out-bill-no/{out_bill_no}`；path `out_bill_no`，query `sub_mchid`。

### 3. `QueryDepositAfterSalesCompensationByBillIdAsync` — 微信支付单号查询保证金售后赔付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504196（【平台商户】，官方页更新于 2025.03.21）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/bill-id/{billId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billId` | Path | `string` | 【微信支付单号】 微信支付单号（官方 path `bill_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`DepositAfterSalesQueryResponse` — 微信支付单号查询保证金售后赔付结果，见 `DepositAfterSalesQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills/bill-id/{bill_id}`；path `bill_id`，query `sub_mchid`。

### 4. `CreateDepositShippingCostCompensationAsync` — 请求保证金运费赔付。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504202（【平台商户】，官方页更新于 2025.03.11）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills`

- **请求体**：`DepositShippingCostCreateRequest` — 请求保证金运费赔付请求体，字段见 `DepositShippingCostCreateRequest`。
- **返回**：`DepositShippingCostCreateResponse` — 请求保证金运费赔付，见 `DepositShippingCostCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills`；无 path / query 参数。
- **应答**见 `DepositShippingCostCreateResponse`（官方应答表逐字段建模）。

### 5. `QueryDepositShippingCostCompensationByOutBillNoAsync` — 商户单号查询保证金运费赔付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504203（【平台商户】，官方页更新于 2025.03.11）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills/out-bill-no/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`DepositShippingCostQueryResponse` — 商户单号查询保证金运费赔付结果，见 `DepositShippingCostQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/deposit-shipping-cost-compensation-bills/out-bill-no/{out_bill_no}`；path `out_bill_no`，query `sub_mchid`。

### 6. `CreateDepositInsurancePremiumsCompensationAsync` — 请求保证金保费赔付。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014010905（【平台商户】，官方页更新于 2025.03.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills`

- **请求体**：`DepositInsurancePremiumsCreateRequest` — 请求保证金保费赔付请求体，字段见 `DepositInsurancePremiumsCreateRequest`。
- **返回**：`DepositInsurancePremiumsCreateResponse` — 请求保证金保费赔付，见 `DepositInsurancePremiumsCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills`；无 path / query 参数。
- **应答**见 `DepositInsurancePremiumsCreateResponse`（官方应答表逐字段建模）。

### 7. `QueryDepositInsurancePremiumsCompensationByOutBillNoAsync` — 商户单号查询保证金保费赔付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014010920（【平台商户】，官方页更新于 2025.03.25）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`DepositInsurancePremiumsQueryResponse` — 商户单号查询保证金保费赔付结果，见 `DepositInsurancePremiumsQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/deposit-insurance-premiums-compensation-bills/{out_bill_no}`；path `out_bill_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/) 内共 **45** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **45** 个，`SerializerClassName` = `PlatsolutionEcommerceDeposit`。

AOT 源生成上下文：[`PlatsolutionEcommerceDepositJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PlatsolutionEcommerceDepositJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `DepositAfterSalesCreateResponse` | 应答 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositAfterSalesCreateRequest` | 请求体 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositAfterSalesQueryResponse` | 应答 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositShippingCostCreateResponse` | 应答 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositShippingCostCreateRequest` | 请求体 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositShippingCostQueryResponse` | 应答 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositInsurancePremiumsCreateResponse` | 应答 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositInsurancePremiumsCreateRequest` | 请求体 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |
| `DepositInsurancePremiumsQueryResponse` | 应答 | [PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/PlatsolutionEcommerceDepositDtos.cs) | `transaction_id`, `type`, `sp_openid`, `transaction_info`, `user_info`, `sub_mchid`, `sp_appid`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `transfer_scene_id` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceDeposit;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceDeposit));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceDepositService>();
var ct = CancellationToken.None;
var response = await service.CreateDepositAfterSalesCompensationAsync(new DepositAfterSalesCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/mch-transfer/deposit-after-sales-compensation-bills");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceDepositService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateDepositAfterSalesCompensationAsync`）；请求体 `DepositAfterSalesCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceDepositService](./IWechatPayPlatsolutionEcommerceDepositService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceDepositContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceDepositContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceDepositTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceDepositEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceDepositDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceDepositDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionEcommerceDeposit/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceDepositContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
