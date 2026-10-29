# PlatsolutionEcommerceMchTransfer 域 · 平台服务商解决方案 · 平台商户付款（商户到商户）

微信支付「平台服务商解决方案 · 平台商户付款（商户到商户）」域 SDK（3 端点）。 接口：[IWechatPayPlatsolutionEcommerceMchTransferService](./IWechatPayPlatsolutionEcommerceMchTransferService.cs)；注册组 `PlatsolutionEcommerceMchTransfer`；模块枚举 `PayModule.PlatsolutionEcommerceMchTransfer`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceMchTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceMchTransferContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceMchTransfer`](../../DataModels/PlatsolutionEcommerceMchTransfer)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceMchTransferService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceMchTransferService` | `CreateMchTransferAsync` | POST | `/v3/platsolution/ecommerce/mch-transfer/transfer-bills` | 商户证书 / APIv3 签名 | 请求商户付款。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504198（【平台商户】，官方页更新于 2025.03.21）… |
| 2 | `IWechatPayPlatsolutionEcommerceMchTransferService` | `QueryMchTransferByOutBillNoAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}` | 商户证书 / APIv3 签名 | 商户单号查询商户付款记录。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504199（【平台商户】，官方页更新于 2025.… |
| 3 | `IWechatPayPlatsolutionEcommerceMchTransferService` | `QueryMchTransferByBillIdAsync` | GET | `/v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{billId}` | 商户证书 / APIv3 签名 | 微信支付单号查询商户付款记录。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504200（【平台商户】，官方页更新于 202… |

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

**本域是「付款给商户」**（`receiver_detail.receiver.mch_info`），与赔付 / 理赔 / 保证金三族共用 `/v3/platsolution/ecommerce/mch-transfer/*` 前缀但应答字段集各异 ⇒ 类型不得跨族复用。

**两支查询应答逐字段同形**（19 行）⇒ 共用 `MchTransferQueryResponse`； 按 `bill_id` 支官方**同时**给出 `sub_mchid` 与 `business_type` 两个选填 query， 按 `out_bill_no` 支只给 `business_type` ⇒ 照页逐支声明，不得合并成一套参数。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）**频率限制**（官方原样）：接口频率限制为100次/s
- （出现 3 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 1. `CreateMchTransferAsync` — 请求商户付款。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504198（【平台商户】，官方页更新于 2025.03.21）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceMchTransferService](./IWechatPayPlatsolutionEcommerceMchTransferService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/mch-transfer/transfer-bills`

- **请求体**：`MchTransferCreateRequest` — 请求商户付款请求体，字段见 `MchTransferCreateRequest`。
- **返回**：`MchTransferCreateResponse` — 请求商户付款，见 `MchTransferCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/mch-transfer/transfer-bills`；无 path / query 参数。
- **应答**见 `MchTransferCreateResponse`（官方应答表逐字段建模）。

### 2. `QueryMchTransferByOutBillNoAsync` — 商户单号查询商户付款记录。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504199（【平台商户】，官方页更新于 2025.03.21）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceMchTransferService](./IWechatPayPlatsolutionEcommerceMchTransferService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 【商户单号】 商户系统内部的单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_bill_no`，**必填**，类型 `string(32)`）。 |
| `businessType` | Query `business_type` | `string?` | 【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方 query `business_type`，选填，类型 `string`）。 |
- **返回**：`MchTransferQueryResponse` — 商户单号查询商户付款记录，见 `MchTransferQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}`；path `out_bill_no`，query `business_type`。

### 3. `QueryMchTransferByBillIdAsync` — 微信支付单号查询商户付款记录。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504200（【平台商户】，官方页更新于 2025.03.21）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceMchTransferService](./IWechatPayPlatsolutionEcommerceMchTransferService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{billId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billId` | Path | `string` | 【微信支付单号】 微信支付单号（官方 path `bill_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
| `businessType` | Query `business_type` | `string?` | 【业务类型】 业务类型。可选取值：`DEPOSIT_COMPENSATION` 保证金赔付 / `PLATFORM_AFTER_SALES_COMPENSATION` 平台售后赔付 / `INSURANCE_CLAIM` 保险理赔 / `DEPOSIT_AFTER_SALES_COMPENSATION` 保证金售后赔付（用户）（官方 query `business_type`，选填，类型 `string`）。 |
- **返回**：`MchTransferQueryResponse` — 微信支付单号查询商户付款记录，见 `MchTransferQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{bill_id}`；path `bill_id`，query `sub_mchid` / `business_type`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceMchTransfer`](../../DataModels/PlatsolutionEcommerceMchTransfer) 内共 **12** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **12** 个，`SerializerClassName` = `PlatsolutionEcommerceMchTransfer`。

AOT 源生成上下文：[`PlatsolutionEcommerceMchTransferJsonContext.g.cs`](./../../DataModels/Generated/PlatsolutionEcommerceMchTransferJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MchTransferCreateResponse` | 应答 | [PlatsolutionEcommerceMchTransfer/PlatsolutionEcommerceMchTransferDtos.cs](../../DataModels/PlatsolutionEcommerceMchTransfer/PlatsolutionEcommerceMchTransferDtos.cs) | `mchid`, `type`, `mch_info`, `sub_mchid`, `sp_appid`, `business_type`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `receiver_remark`, `sp_mchid` … |
| `MchTransferCreateRequest` | 请求体 | [PlatsolutionEcommerceMchTransfer/PlatsolutionEcommerceMchTransferDtos.cs](../../DataModels/PlatsolutionEcommerceMchTransfer/PlatsolutionEcommerceMchTransferDtos.cs) | `mchid`, `type`, `mch_info`, `sub_mchid`, `sp_appid`, `business_type`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `receiver_remark`, `sp_mchid` … |
| `MchTransferQueryResponse` | 应答 | [PlatsolutionEcommerceMchTransfer/PlatsolutionEcommerceMchTransferDtos.cs](../../DataModels/PlatsolutionEcommerceMchTransfer/PlatsolutionEcommerceMchTransferDtos.cs) | `mchid`, `type`, `mch_info`, `sub_mchid`, `sp_appid`, `business_type`, `receiver`, `out_bill_no`, `amount`, `transfer_remark`, `receiver_remark`, `sp_mchid` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceMchTransfer));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceMchTransferService>();
var ct = CancellationToken.None;
var response = await service.CreateMchTransferAsync(new MchTransferCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/mch-transfer/transfer-bills");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceMchTransferService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateMchTransferAsync`）；请求体 `MchTransferCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceMchTransferService](./IWechatPayPlatsolutionEcommerceMchTransferService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceMchTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceMchTransferContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceMchTransferTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceMchTransferEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceMchTransferDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceMchTransferDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceMchTransfer`](../../DataModels/PlatsolutionEcommerceMchTransfer) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceMchTransferContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
