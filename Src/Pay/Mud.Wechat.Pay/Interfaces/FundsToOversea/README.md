# FundsToOversea 域 · 资金出境（平台商户面）

微信支付「资金出境（平台商户面）」域 SDK（6 端点）。 接口：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)；注册组 `FundsToOversea`；模块枚举 `PayModule.FundsToOversea`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayFundsToOverseaContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFundsToOverseaContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/FundsToOversea`](../../DataModels/FundsToOversea)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayFundsToOverseaService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayFundsToOverseaService` | `GetFundsToOverseaBillDownloadUrlAsync` | GET | `/v3/funds-to-oversea/bill-download-url` | 商户证书 / APIv3 签名 | 获取购付汇账单文件下载链接。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476132（【平台商户】）。 |
| 2 | `IWechatPayFundsToOverseaService` | `CreateFundsToOverseaOrderAsync` | POST | `/v3/funds-to-oversea/orders` | 商户证书 / APIv3 签名 | 申请资金出境。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476113（【平台商户】）。 |
| 3 | `IWechatPayFundsToOverseaService` | `QueryFundsToOverseaOrderAsync` | GET | `/v3/funds-to-oversea/orders/{outOrderId}` | 商户证书 / APIv3 签名 | 查询出境结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476127（【平台商户】）。 |
| 4 | `IWechatPayFundsToOverseaService` | `CreateFundsToOverseaReturnOrderAsync` | POST | `/v3/funds-to-oversea/return/return-orders` | 商户证书 / APIv3 签名 | 申请资金出境退回。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013735039（【平台商户】）。 |
| 5 | `IWechatPayFundsToOverseaService` | `QueryFundsToOverseaReturnOrderAsync` | GET | `/v3/funds-to-oversea/return/return-orders/out-return-no/{outReturnNo}` | 商户证书 / APIv3 签名 | 查询出境退回结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013735052（【平台商户】）。 |
| 6 | `IWechatPayFundsToOverseaService` | `QueryFundsToOverseaAvailableAbroadAmountAsync` | GET | `/v3/funds-to-oversea/transactions/{transactionId}/available_abroad_amounts` | 商户证书 / APIv3 签名 | 查询订单剩余可出境余额。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476109（【平台商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中**6 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 6 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**六支官方全部只印服务商页，且身份是【平台商户】**（不是【普通服务商】）⇒ 本域是收付通平台面能力， **不得**按服务商直连面补建商户页，也不得与其它「/v3/partner 树」域混认身份。

**「受理 ≠ 出境成功」是官方原文**：「接口请求成功仅代表受理成功，如需知晓业务执行情况请**通过查询接口获取**」⇒ 下单支与退回支都不得据 HTTP 200 判定业务成功；共用应答里的 `result` / `state` 与 `fail_reason` 才是判据。

**账单下载链接三条硬时效**：官方原文「下载地址的有效时间为 **30s**」、账单「出账日期为 T+1」（当天下载到的是前一天 购付汇**且出境成功**的单）、「一般在**上午 10 点**后可以下载到前一天的账单」，并要求 「将实际账单文件的哈希值和接口返回的哈希值进行**比对**以确认数据完整性」⇒ `hash_type` / `hash_value` 不是可选项； 另注「账单中涉及金额的字段单位为**元**」，与本线绝大多数「分」口径相反。

**两处写读共用应答，各按官方应答表逐字段同形判定**：出境单 POST 与按 `out_order_id` 查询的应答表 **15 行同形且必填口径一致** ⇒ 共用 `FundsToOverseaOrderResponse`；退回单两支同为 **11 行同形** ⇒ 共用 `FundsToOverseaReturnOrderResponse`。账单下载支与在途可出境额度支各建一型，**不得**并入上述两型。

**查询支的三个键不得省**：按 `out_order_id` 查出境单的官方必填 **query** 是 `sub_mchid` + `transaction_id`，而 path 段是商户出境单号 ⇒ 一支同时带「商户单号 + 微信单号 + 特约商户号」三种定位键， 照页不得只留 path 段。退回单查询支同理把 `sub_mchid` 列为必填 query 而 path 段是 `out_return_no`。

**频率限制**：六支官方页**未印**端点级数字，只在错误码表给出 `429 FREQUENCY_LIMITED`（下单支原文「资金出境限频」）； 六支请求头表均**未列** `Wechatpay-Serial` ⇒ 本域无涉密必填头。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**支持商户**【平台商户】。
- （出现 6 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。

### 1. `GetFundsToOverseaBillDownloadUrlAsync` — 获取购付汇账单文件下载链接。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476132（【平台商户】）。

- **声明接口**：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)
- **官方契约**：`GET /v3/funds-to-oversea/bill-download-url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billDate` | Query `bill_date` | `string` | 【账单日期】格式YYYY-MM-DD（官方 query `bill_date`，**必填**，类型 `string(10)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】若商户是直连商户： <ul><li>无需填写该字段</li></ul> 若商户是服务商： <ul><li>不填则默认返回服务商下的账单数据</li><li>如需下载某个子商户下的账单数据，则该字段必填</li></ul>（官方 query `sub_mchid`，选填，类型 `string(20)`）。 |
- **返回**：`FundsToOverseaBillDownloadUrlResponse` — 获取购付汇账单文件下载链接，见 `FundsToOverseaBillDownloadUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/funds-to-oversea/bill-download-url`；query `bill_date` / `sub_mchid`。
- **`bill_date` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：403 NO_AUTH：商户无权限申请资金出境。

### 2. `CreateFundsToOverseaOrderAsync` — 申请资金出境。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476113（【平台商户】）。

- **声明接口**：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)
- **官方契约**：`POST /v3/funds-to-oversea/orders`

- **请求体**：`FundsToOverseaOrderCreateRequest` — 申请资金出境请求体，字段见 `FundsToOverseaOrderCreateRequest`。
- **返回**：`FundsToOverseaOrderResponse` — 申请资金出境，见 `FundsToOverseaOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/funds-to-oversea/orders`；无 path / query 参数。
- **应答**见 `FundsToOverseaOrderResponse`（官方应答表逐字段建模）。
- **业务错误码**：403 NO_AUTH：商户无权限申请资金出境 / 404 NOT_FOUND：请求的资源不存在 / 429 FREQUENCY_LIMITED：资金出境限频。

### 3. `QueryFundsToOverseaOrderAsync` — 查询出境结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476127（【平台商户】）。

- **声明接口**：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)
- **官方契约**：`GET /v3/funds-to-oversea/orders/{outOrderId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderId` | Path | `string` | 【商户出境单号】本次需要查询的商户出境单号，由商户在发起资金出境请求时生成。（官方 path `out_order_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】申请资金出境的二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(64)`）。 |
| `transactionId` | Query `transaction_id` | `string` | 【微信支付订单号】微信支付返回的支付订单号（官方 query `transaction_id`，**必填**，类型 `string(64)`）。 |
- **返回**：`FundsToOverseaOrderResponse` — 查询出境结果，见 `FundsToOverseaOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/funds-to-oversea/orders/{out_order_id}`；path `out_order_id`，query `sub_mchid` / `transaction_id`。
- **`sub_mchid` 与 `transaction_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：403 NO_AUTH：商户无权限申请资金出境。

### 4. `CreateFundsToOverseaReturnOrderAsync` — 申请资金出境退回。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013735039（【平台商户】）。

- **声明接口**：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)
- **官方契约**：`POST /v3/funds-to-oversea/return/return-orders`

- **请求体**：`FundsToOverseaReturnOrderCreateRequest` — 申请资金出境退回请求体，字段见 `FundsToOverseaReturnOrderCreateRequest`。
- **返回**：`FundsToOverseaReturnOrderResponse` — 申请资金出境退回，见 `FundsToOverseaReturnOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/funds-to-oversea/return/return-orders`；无 path / query 参数。
- **应答**见 `FundsToOverseaReturnOrderResponse`（官方应答表逐字段建模）。
- **业务错误码**：403 NO_AUTH：商户无权限申请资金出境退回 / 429 FREQUENCY_LIMITED：频率限制。

### 5. `QueryFundsToOverseaReturnOrderAsync` — 查询出境退回结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013735052（【平台商户】）。

- **声明接口**：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)
- **官方契约**：`GET /v3/funds-to-oversea/return/return-orders/out-return-no/{outReturnNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outReturnNo` | Path | `string` | 【商户出境退回单号】 商户在发起资金出境退回请求时生成，要求在同一个商户号下唯一。只能由数字，大小写字母_-组成（官方 path `out_return_no`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 申请资金出境退回的二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(64)`）。 |
- **返回**：`FundsToOverseaReturnOrderResponse` — 查询出境退回结果，见 `FundsToOverseaReturnOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/funds-to-oversea/return/return-orders/out-return-no/{out_return_no}`；path `out_return_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：404 NOT_FOUND：资金出境退回单不存在 / 429 FREQUENCY_LIMITED：频率限制。

### 6. `QueryFundsToOverseaAvailableAbroadAmountAsync` — 查询订单剩余可出境余额。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476109（【平台商户】）。

- **声明接口**：[IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs)
- **官方契约**：`GET /v3/funds-to-oversea/transactions/{transactionId}/available_abroad_amounts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 【微信支付订单号】微信支付返回的支付订单号（官方 path `transaction_id`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】查询剩余可出境金额的二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`FundsToOverseaTransactionAvailableAbroadAmountResponse` — 查询订单剩余可出境余额，见 `FundsToOverseaTransactionAvailableAbroadAmountResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/funds-to-oversea/transactions/{transaction_id}/available_abroad_amounts`；path `transaction_id`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 403 NO_AUTH：商户无权限申请资金出境 / 429 FREQUENCY_LIMITED：频率限制 / 500 SYSTEM_ERROR：接口返回错误。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/FundsToOversea`](../../DataModels/FundsToOversea) 内共 **11** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **11** 个，`SerializerClassName` = `FundsToOversea`。

AOT 源生成上下文：[`FundsToOverseaJsonContext.g.cs`](./../../DataModels/Generated/FundsToOverseaJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `FundsToOverseaBillDownloadUrlResponse` | 应答 | [FundsToOversea/FundsToOverseaBillDtos.cs](../../DataModels/FundsToOversea/FundsToOverseaBillDtos.cs) | `hash_type`, `hash_value`, `download_url` |
| `FundsToOverseaOrderResponse` | 应答 | [FundsToOversea/FundsToOverseaOrderDtos.cs](../../DataModels/FundsToOversea/FundsToOverseaOrderDtos.cs) | `goods_name`, `goods_category`, `goods_unit_price`, `goods_quantity`, `oversea_business_name`, `oversea_shop_name`, `seller_id`, `courier_number`, `express_company_name`, `payee_id`, `type`, `total_amount` … |
| `FundsToOverseaOrderCreateRequest` | 请求体 | [FundsToOversea/FundsToOverseaOrderDtos.cs](../../DataModels/FundsToOversea/FundsToOverseaOrderDtos.cs) | `goods_name`, `goods_category`, `goods_unit_price`, `goods_quantity`, `oversea_business_name`, `oversea_shop_name`, `seller_id`, `courier_number`, `express_company_name`, `payee_id`, `type`, `total_amount` … |
| `FundsToOverseaReturnOrderResponse` | 应答 | [FundsToOversea/FundsToOverseaReturnOrderDtos.cs](../../DataModels/FundsToOversea/FundsToOverseaReturnOrderDtos.cs) | `out_return_no`, `sub_mchid`, `out_order_id`, `transaction_id`, `supplement_info`, `amount`, `return_id`, `state`, `create_time`, `success_time`, `fail_reason` |
| `FundsToOverseaReturnOrderCreateRequest` | 请求体 | [FundsToOversea/FundsToOverseaReturnOrderDtos.cs](../../DataModels/FundsToOversea/FundsToOverseaReturnOrderDtos.cs) | `out_return_no`, `sub_mchid`, `out_order_id`, `transaction_id`, `supplement_info`, `amount`, `return_id`, `state`, `create_time`, `success_time`, `fail_reason` |
| `FundsToOverseaTransactionAvailableAbroadAmountResponse` | 应答 | [FundsToOversea/FundsToOverseaTransactionDtos.cs](../../DataModels/FundsToOversea/FundsToOverseaTransactionDtos.cs) | `transaction_id`, `available_abroad_amount` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.FundsToOversea;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.FundsToOversea));

var service = app.Services.GetRequiredService<IWechatPayFundsToOverseaService>();
var ct = CancellationToken.None;
var response = await service.CreateFundsToOverseaOrderAsync(new FundsToOverseaOrderCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/funds-to-oversea/orders");
// 占位值仅用于编译示意；业务值须按 IWechatPayFundsToOverseaService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateFundsToOverseaOrderAsync`）；请求体 `FundsToOverseaOrderCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayFundsToOverseaService](./IWechatPayFundsToOverseaService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayFundsToOverseaContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFundsToOverseaContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `FundsToOverseaTopology_ShouldMatchInterfaceAndGroupRoster`、`FundsToOverseaEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`FundsToOverseaDtos_ShouldExposeOfficialFieldRosters`、`FundsToOverseaDataModels_ShouldSplitRequestAndResponseBaseTypes`、`FundsToOverseaAllDtos_ShouldBeAnnotatedAndContextRegistered`、`FundsToOverseaRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/FundsToOversea`](../../DataModels/FundsToOversea) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayFundsToOverseaContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
