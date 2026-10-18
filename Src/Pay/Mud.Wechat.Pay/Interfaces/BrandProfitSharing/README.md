# BrandProfitSharing 域 · 品牌分账

微信支付「品牌分账」域 SDK（9 端点）。 接口：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)；注册组 `BrandProfitSharing`；模块枚举 `PayModule.BrandProfitSharing`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBrandProfitSharingContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandProfitSharingContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/BrandProfitSharing`](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBrandProfitSharingService = 9；按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBrandProfitSharingService` | `QueryBrandProfitSharingMaxRatioAsync` | GET | `/v3/brand/profitsharing/brand-configs/{brandMchid}` | 商户证书 / APIv3 签名 | 品牌最大分账比例。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467022（【普通服务商】）。 |
| 2 | `IWechatPayBrandProfitSharingService` | `FinishBrandProfitSharingOrderAsync` | POST | `/v3/brand/profitsharing/finish-order` | 商户证书 / APIv3 签名 | 品牌分账解冻剩余资金。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467016（【普通服务商】）。 |
| 3 | `IWechatPayBrandProfitSharingService` | `ApplyBrandProfitSharingOrderAsync` | POST | `/v3/brand/profitsharing/orders` | 商户证书 / APIv3 签名 | 品牌分账请求。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692975（【普通服务商】）。 |
| 4 | `IWechatPayBrandProfitSharingService` | `QueryBrandProfitSharingOrdersAsync` | GET | `/v3/brand/profitsharing/orders` | 商户证书 / APIv3 签名 | 品牌分账结果查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467002（【普通服务商】）。 |
| 5 | `IWechatPayBrandProfitSharingService` | `QueryBrandProfitSharingAmountsAsync` | GET | `/v3/brand/profitsharing/orders/{transactionId}/amounts` | 商户证书 / APIv3 签名 | 订单剩余待分金额。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467021（【普通服务商】）。 |
| 6 | `IWechatPayBrandProfitSharingService` | `AddBrandProfitSharingReceiverAsync` | POST | `/v3/brand/profitsharing/receivers/add` | 商户证书 / APIv3 签名 | 品牌分账接收方添加。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467100（【普通服务商】）。 |
| 7 | `IWechatPayBrandProfitSharingService` | `DeleteBrandProfitSharingReceiverAsync` | POST | `/v3/brand/profitsharing/receivers/delete` | 商户证书 / APIv3 签名 | 品牌分账接收方删除。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467103（【普通服务商】）。 |
| 8 | `IWechatPayBrandProfitSharingService` | `ApplyBrandProfitSharingReturnOrderAsync` | POST | `/v3/brand/profitsharing/returnorders` | 商户证书 / APIv3 签名 | 品牌分账回退请求。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467097（【普通服务商】）。 |
| 9 | `IWechatPayBrandProfitSharingService` | `QueryBrandProfitSharingReturnOrderAsync` | GET | `/v3/brand/profitsharing/returnorders` | 商户证书 / APIv3 签名 | 品牌分账回退结果查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467011（【普通服务商】）。 |

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

**频率限制口径**：本域九支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED`
（分账频率过高 / 查询频率过高两种措辞）⇒ 不得凭第三方 SDK 补数。

**本域零 `brand_id`、零列表支**：query 全部是单据定位键（`sub_mchid` + `transaction_id` +
`out_order_no`，回退单再加 `out_return_no` / `order_id`）⇒ 品牌作用域走签名主体与 `sub_mchid`，
不存在可补的分页参数。

**与 `IWechatPayProfitSharingService` 是同族不同路由**：本域全部走 `/v3/brand/profitsharing/*`， 请求/应答多 `brand_mchid`（品牌主商户号）与 `sub_mchid`（收款方）两级商户口径 ⇒ DTO 与接收方类型**不得**与普通服务商分账互相套用。

**分账与回退各两支**：发起 `POST /orders`（15 行应答）与结果查询 `GET /orders`（16 行， 多 `finish_amount` / `finish_description`，仅查询分账完结结果时存在）⇒ **不同形**、分建； 回退 `POST /returnorders` 与回退查询 `GET /returnorders` 应答**同形**（10 行）⇒ 共用。

**结果查询支的三个 query 全必填**：`sub_mchid` / `transaction_id` / `out_order_no`； 回退查询支只 `sub_mchid` 与 `out_return_no` 必填 ⇒ 两支定位口径不同，照页不得互套。

**接收方添加与删除共用应答**：`receivers/add` 与 `receivers/delete` 官方应答表同形（3 行）⇒ 共用 `BrandProfitSharingReceiverResponse`。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 9 次）**支持商户**【普通服务商】。
- （出现 9 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`）⇒ 不得凭第三方 SDK 补数。

### 1. `QueryBrandProfitSharingMaxRatioAsync` — 品牌最大分账比例。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467022（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`GET /v3/brand/profitsharing/brand-configs/{brandMchid}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandMchid` | Path | `string` | 【品牌商户号】 微信支付分配的商户号，分账出资的品牌商户号（官方 path `brand_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`BrandProfitSharingMaxRatioResponse` — 品牌最大分账比例，见 `BrandProfitSharingMaxRatioResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/profitsharing/brand-configs/{brand_mchid}`；path `brand_mchid`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMITED 商户发起查询的频率过高 / 403 NO_AUTH 服务商未开通分账功能。

### 2. `FinishBrandProfitSharingOrderAsync` — 品牌分账解冻剩余资金。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467016（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`POST /v3/brand/profitsharing/finish-order`

- **请求体**：`BrandProfitSharingFinishRequest` — 品牌分账解冻剩余资金请求体，字段见 `BrandProfitSharingFinishRequest`。
- **返回**：`BrandProfitSharingFinishResponse` — 品牌分账解冻剩余资金，见 `BrandProfitSharingFinishResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/profitsharing/finish-order`；无 path / query 参数。
- **应答**见 `BrandProfitSharingFinishResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 403 NOT_ENOUGH 分账金额为0 / 429 FREQUENCY_LIMITED 对同笔订单分账频率过高。

### 3. `ApplyBrandProfitSharingOrderAsync` — 品牌分账请求。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692975（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`POST /v3/brand/profitsharing/orders`

- **请求体**：`BrandProfitSharingOrderRequest` — 品牌分账请求请求体，字段见 `BrandProfitSharingOrderRequest`。
- **返回**：`BrandProfitSharingOrderResponse` — 品牌分账请求，见 `BrandProfitSharingOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/profitsharing/orders`；无 path / query 参数。
- **应答**见 `BrandProfitSharingOrderResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 403 RULE_LIMIT 分账金额超出最大分账比例 / 403 NOT_ENOUGH 分账金额不足 / 429 FREQUENCY_LIMITED 对同笔订单分账频率过高。

### 4. `QueryBrandProfitSharingOrdersAsync` — 品牌分账结果查询。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467002（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`GET /v3/brand/profitsharing/orders`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchid` | Query `sub_mchid` | `string` | 【特约商户号】 订单收款方商户号，填写微信支付分配的商户号，可以是品牌主商户号，也可以是门店商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `transactionId` | Query `transaction_id` | `string` | 【微信订单号】 微信支付订单号（官方 query `transaction_id`，**必填**，类型 `string(32)`）。 |
| `outOrderNo` | Query `out_order_no` | `string` | 【商户分账单号】 请求分账时的商户分账单号（官方 query `out_order_no`，**必填**，类型 `string(64)`）。 |
- **返回**：`BrandProfitSharingOrderListResponse` — 品牌分账结果查询，见 `BrandProfitSharingOrderListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/profitsharing/orders`；query `sub_mchid` / `transaction_id` / `out_order_no`。
- **`sub_mchid` 与 `transaction_id` 与 `out_order_no` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 RESOURCE_NOT_EXISTS 记录不存在 / 429 FREQUENCY_LIMITED 商户发起分账查询的频率过高。

### 5. `QueryBrandProfitSharingAmountsAsync` — 订单剩余待分金额。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467021（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`GET /v3/brand/profitsharing/orders/{transactionId}/amounts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 【微信订单号】 微信支付订单号（官方 path `transaction_id`，**必填**，类型 `string(32)`）。 |
- **返回**：`BrandProfitSharingAmountsResponse` — 订单剩余待分金额，见 `BrandProfitSharingAmountsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/profitsharing/orders/{transaction_id}/amounts`；path `transaction_id`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 RESOURCE_NOT_EXISTS 记录不存在 / 429 FREQUENCY_LIMITED 商户发起查询的频率过高。

### 6. `AddBrandProfitSharingReceiverAsync` — 品牌分账接收方添加。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467100（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`POST /v3/brand/profitsharing/receivers/add`

- **请求体**：`BrandProfitSharingReceiverAddRequest` — 品牌分账接收方添加请求体，字段见 `BrandProfitSharingReceiverAddRequest`。
- **返回**：`BrandProfitSharingReceiverResponse` — 品牌分账接收方添加，见 `BrandProfitSharingReceiverResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/profitsharing/receivers/add`；无 path / query 参数。
- **应答**见 `BrandProfitSharingReceiverResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 429 FREQUENCY_LIMITED 添加接收方频率过高。

### 7. `DeleteBrandProfitSharingReceiverAsync` — 品牌分账接收方删除。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467103（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`POST /v3/brand/profitsharing/receivers/delete`

- **请求体**：`BrandProfitSharingReceiverDeleteRequest` — 品牌分账接收方删除请求体，字段见 `BrandProfitSharingReceiverDeleteRequest`。
- **返回**：`BrandProfitSharingReceiverResponse` — 品牌分账接收方删除，见 `BrandProfitSharingReceiverResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/profitsharing/receivers/delete`；无 path / query 参数。
- **应答**见 `BrandProfitSharingReceiverResponse`（官方应答表逐字段建模，与 `POST /brand/profitsharing/receivers/add` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户无权限 / 429 FREQUENCY_LIMITED 删除接收方频率过高。

### 8. `ApplyBrandProfitSharingReturnOrderAsync` — 品牌分账回退请求。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467097（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`POST /v3/brand/profitsharing/returnorders`

- **请求体**：`BrandProfitSharingReturnOrderRequest` — 品牌分账回退请求请求体，字段见 `BrandProfitSharingReturnOrderRequest`。
- **返回**：`BrandProfitSharingReturnOrderResponse` — 品牌分账回退请求，见 `BrandProfitSharingReturnOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/profitsharing/returnorders`；无 path / query 参数。
- **应答**见 `BrandProfitSharingReturnOrderResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 回退方未开通分账回退功能 / 403 NOT_ENOUGH 剩余可回退的金额不足 / 429 FREQUENCY_LIMITED 商户发起分账回退的频率过高。

### 9. `QueryBrandProfitSharingReturnOrderAsync` — 品牌分账回退结果查询。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467011（【普通服务商】）。

- **声明接口**：[IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs)
- **官方契约**：`GET /v3/brand/profitsharing/returnorders`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchid` | Query `sub_mchid` | `string` | 【特约商户号】 微信支付分配的特约商户号，分账回退的接收商户，对应原分账出资的分账方商户（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `outReturnNo` | Query `out_return_no` | `string` | 【商户回退单号】 调用 请求分账回退 接口时传入的商户回退单号（官方 query `out_return_no`，**必填**，类型 `string(64)`）。 |
| `orderId` | Query `order_id` | `string?` | 【微信分账单号】 原发起分账请求时，微信返回的微信分账单号，与商户分账单号一一对应。 微信分账单号与商户分账单号二选一填写（官方 query `order_id`，选填，类型 `string(64)`）。 |
| `outOrderNo` | Query `out_order_no` | `string?` | 【商户分账单号】 原发起分账请求时使用的商户系统内部的分账单号。微信分账单号与商户分账单号二选一填写（官方 query `out_order_no`，选填，类型 `string(64)`）。 |
- **返回**：`BrandProfitSharingReturnOrderResponse` — 品牌分账回退结果查询，见 `BrandProfitSharingReturnOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/profitsharing/returnorders`；query `sub_mchid` / `out_return_no` / `order_id` / `out_order_no`。
- **`sub_mchid` 与 `out_return_no` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 RESOURCE_NOT_EXISTS 记录不存在 / 429 FREQUENCY_LIMITED 商户发起分账回退查询的频率过高。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/BrandProfitSharing`](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/) 内共 **15** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **15** 个，`SerializerClassName` = `BrandProfitSharing`。

AOT 源生成上下文：[`BrandProfitSharingJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BrandProfitSharingJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BrandProfitSharingMaxRatioResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingFinishResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingFinishRequest` | 请求体 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingOrderResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingOrderRequest` | 请求体 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingOrderListResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingAmountsResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingReceiverResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingReceiverAddRequest` | 请求体 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingReceiverDeleteRequest` | 请求体 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingReturnOrderResponse` | 应答 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |
| `BrandProfitSharingReturnOrderRequest` | 请求体 | [BrandProfitSharing/BrandProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/BrandProfitSharingDtos.cs) | `brand_mchid`, `max_ratio`, `sub_mchid`, `transaction_id`, `out_order_no`, `description`, `order_id`, `type`, `account`, `amount`, `name`, `appid` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BrandProfitSharing;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandProfitSharing));

var service = app.Services.GetRequiredService<IWechatPayBrandProfitSharingService>();
var ct = CancellationToken.None;
var response = await service.AddBrandProfitSharingReceiverAsync(new BrandProfitSharingReceiverAddRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/brand/profitsharing/receivers/add");
// 占位值仅用于编译示意；业务值须按 IWechatPayBrandProfitSharingService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddBrandProfitSharingReceiverAsync`）；请求体 `BrandProfitSharingReceiverAddRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBrandProfitSharingService](./IWechatPayBrandProfitSharingService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBrandProfitSharingContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandProfitSharingContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BrandProfitSharingTopology_ShouldBeOneInterfaceOnOneGroup`、`BrandProfitSharingEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BrandProfitSharingDtos_ShouldExposeOfficialFieldRosters`、`BrandProfitSharingDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/BrandProfitSharing`](../../../Mud.Wechat.Pay.DataModels/BrandProfitSharing/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBrandProfitSharingContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
