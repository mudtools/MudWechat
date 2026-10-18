# PlatsolutionInsuranceReceipt 域 · 平台商户保险理赔商家转账 · 电子回单（批次汇总 + 明细）

微信支付「平台商户保险理赔商家转账 · 电子回单（批次汇总 + 明细）」域 SDK（4 端点）。 接口：[IWechatPayPlatsolutionInsuranceReceiptService](./IWechatPayPlatsolutionInsuranceReceiptService.cs)；注册组 `PlatsolutionInsuranceReceipt`；模块枚举 `PayModule.PlatsolutionInsuranceReceipt`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionInsuranceReceiptContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionInsuranceReceiptContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionInsuranceReceiptService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionInsuranceReceiptService` | `ApplyInsuranceSummaryReceiptAsync` | POST | `/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}/apply` | 商户证书 / APIv3 签名 | 受理商家转账批次汇总电子回单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504226（【平台商户】，官方页更新于 202… |
| 2 | `IWechatPayPlatsolutionInsuranceReceiptService` | `QueryInsuranceSummaryReceiptAsync` | GET | `/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}` | 商户证书 / APIv3 签名 | 查询商家转账批次汇总电子回单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504227（【平台商户】，官方页更新于 202… |
| 3 | `IWechatPayPlatsolutionInsuranceReceiptService` | `ApplyInsuranceDetailReceiptAsync` | POST | `/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{outBatchNo}/out-detail-no/{outDetailNo}/apply` | 商户证书 / APIv3 签名 | 受理商家转账明细电子回单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504228（【平台商户】，官方页更新于 2025.… |
| 4 | `IWechatPayPlatsolutionInsuranceReceiptService` | `QueryInsuranceDetailReceiptAsync` | GET | `/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{outBatchNo}/out-detail-no/{outDetailNo}` | 商户证书 / APIv3 签名 | 查询商家转账明细电子回单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504229（【平台商户】，官方页更新于 2025.… |

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

**「平台商户」身份口径**：本域 13 页「支持商户」栏**恒为【平台商户】**（无【普通服务商】分身页），
请求体与查询 query 统一以 `sub_mchid`（二级商户号，出资方）定位 ⇒ 平台商户身份由调用侧签名承载，本域**零** `sp_mchid` 请求参数。

**四支四型、两两不同形**：汇总申请 3 行、汇总查询 7 行、明细申请 4 行、明细查询 9 行 ⇒ 申请与查询不同形（查询支才返回 `hash_type` / `hash_value` / `download_url`）， 汇总与明细亦不同形（明细支多 `out_detail_no` / `detail_id`）⇒ 四支各建一型，**不得**因「字段名大量重合」合并。

**同名 `sub_mchid` 两支落位不同**：申请支是**请求体唯一字段**（定位键 `out_batch_no` 在 path）， 查询支是**必填 query** 且零请求体 ⇒ 申请与查询不得共用一个请求 DTO。

**只支持查询最近 2 年内**的转账批次单 / 明细单（官方前置条件）；回单文件生成**结束后**查询支才带下载地址 ⇒ 拿到 `receipt_state` 非终态时须继续轮询而非报错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 1. `ApplyInsuranceSummaryReceiptAsync` — 受理商家转账批次汇总电子回单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504226（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReceiptService](./IWechatPayPlatsolutionInsuranceReceiptService.cs)
- **官方契约**：`POST /v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}/apply`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |

- **请求体**：`InsuranceSummaryReceiptApplyRequest` — 受理商家转账批次汇总电子回单请求体，字段见 `InsuranceSummaryReceiptApplyRequest`。
- **返回**：`InsuranceSummaryReceiptApplyResponse` — 受理商家转账批次汇总电子回单，见 `InsuranceSummaryReceiptApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{out_batch_no}/apply`；path `out_batch_no`。
- **应答**见 `InsuranceSummaryReceiptApplyResponse`（官方应答表逐字段建模）。

### 2. `QueryInsuranceSummaryReceiptAsync` — 查询商家转账批次汇总电子回单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504227（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReceiptService](./IWechatPayPlatsolutionInsuranceReceiptService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceSummaryReceiptQueryResponse` — 查询商家转账批次汇总电子回单，见 `InsuranceSummaryReceiptQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{out_batch_no}`；path `out_batch_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 3. `ApplyInsuranceDetailReceiptAsync` — 受理商家转账明细电子回单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504228（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReceiptService](./IWechatPayPlatsolutionInsuranceReceiptService.cs)
- **官方契约**：`POST /v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{outBatchNo}/out-detail-no/{outDetailNo}/apply`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `outDetailNo` | Path | `string` | 【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path `out_detail_no`，**必填**，类型 `string(32)`）。 |

- **请求体**：`InsuranceDetailReceiptApplyRequest` — 受理商家转账明细电子回单请求体，字段见 `InsuranceDetailReceiptApplyRequest`。
- **返回**：`InsuranceDetailReceiptApplyResponse` — 受理商家转账明细电子回单，见 `InsuranceDetailReceiptApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{out_batch_no}/out-detail-no/{out_detail_no}/apply`；path `out_batch_no` / `out_detail_no`。
- **应答**见 `InsuranceDetailReceiptApplyResponse`（官方应答表逐字段建模）。

### 4. `QueryInsuranceDetailReceiptAsync` — 查询商家转账明细电子回单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504229（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReceiptService](./IWechatPayPlatsolutionInsuranceReceiptService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{outBatchNo}/out-detail-no/{outDetailNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `outDetailNo` | Path | `string` | 【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path `out_detail_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceDetailReceiptQueryResponse` — 查询商家转账明细电子回单，见 `InsuranceDetailReceiptQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/batches/detail-receipts/out-batch-no/{out_batch_no}/out-detail-no/{out_detail_no}`；path `out_batch_no` / `out_detail_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `PlatsolutionInsuranceReceipt`。

AOT 源生成上下文：[`PlatsolutionInsuranceReceiptJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PlatsolutionInsuranceReceiptJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `InsuranceSummaryReceiptApplyResponse` | 应答 | [PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs) | `sub_mchid`, `out_batch_no`, `receipt_state`, `batch_id`, `hash_type`, `hash_value`, `download_url`, `out_detail_no`, `detail_id` |
| `InsuranceSummaryReceiptApplyRequest` | 请求体 | [PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs) | `sub_mchid`, `out_batch_no`, `receipt_state`, `batch_id`, `hash_type`, `hash_value`, `download_url`, `out_detail_no`, `detail_id` |
| `InsuranceSummaryReceiptQueryResponse` | 应答 | [PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs) | `sub_mchid`, `out_batch_no`, `receipt_state`, `batch_id`, `hash_type`, `hash_value`, `download_url`, `out_detail_no`, `detail_id` |
| `InsuranceDetailReceiptApplyResponse` | 应答 | [PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs) | `sub_mchid`, `out_batch_no`, `receipt_state`, `batch_id`, `hash_type`, `hash_value`, `download_url`, `out_detail_no`, `detail_id` |
| `InsuranceDetailReceiptApplyRequest` | 请求体 | [PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs) | `sub_mchid`, `out_batch_no`, `receipt_state`, `batch_id`, `hash_type`, `hash_value`, `download_url`, `out_detail_no`, `detail_id` |
| `InsuranceDetailReceiptQueryResponse` | 应答 | [PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/PlatsolutionInsuranceReceiptDtos.cs) | `sub_mchid`, `out_batch_no`, `receipt_state`, `batch_id`, `hash_type`, `hash_value`, `download_url`, `out_detail_no`, `detail_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReceipt;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionInsuranceReceipt));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionInsuranceReceiptService>();
var ct = CancellationToken.None;
var response = await service.ApplyInsuranceSummaryReceiptAsync("", new InsuranceSummaryReceiptApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/insurance/mch-transfer/batches/summary-receipts/out-batch-no/{outBatchNo}/apply");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionInsuranceReceiptService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyInsuranceSummaryReceiptAsync`）；请求体 `InsuranceSummaryReceiptApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionInsuranceReceiptService](./IWechatPayPlatsolutionInsuranceReceiptService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionInsuranceReceiptContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionInsuranceReceiptContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionInsuranceReceiptTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionInsuranceReceiptEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionInsuranceReceiptDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionInsuranceReceiptDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt`](../../../Mud.Wechat.Pay.DataModels/PlatsolutionInsuranceReceipt/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionInsuranceReceiptContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
