# PlatsolutionInsuranceBatch 域 · 平台商户保险理赔商家转账 · 商家转账批次与明细（保险场景）

微信支付「平台商户保险理赔商家转账 · 商家转账批次与明细（保险场景）」域 SDK（5 端点）。 接口：[IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs)；注册组 `PlatsolutionInsuranceBatch`；模块枚举 `PayModule.PlatsolutionInsuranceBatch`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionInsuranceBatchContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionInsuranceBatchContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/PlatsolutionInsuranceBatch`](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionInsuranceBatchService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionInsuranceBatchService` | `ApplyInsuranceBatchAsync` | POST | `/v3/platsolution/insurance/mch-transfer/batches/apply` | 商户证书 / APIv3 签名 | 受理商家转账。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504220（【平台商户】，官方页更新于 2025.01.21）… |
| 2 | `IWechatPayPlatsolutionInsuranceBatchService` | `QueryInsuranceBatchByOutBatchNoAsync` | GET | `/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}` | 商户证书 / APIv3 签名 | 商户转账批次单号查询批次单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504222（【平台商户】，官方页更新于 2025… |
| 3 | `IWechatPayPlatsolutionInsuranceBatchService` | `QueryInsuranceBatchByBatchIdAsync` | GET | `/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}` | 商户证书 / APIv3 签名 | 微信支付转账批次单号查询批次单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504221（【平台商户】，官方页更新于 20… |
| 4 | `IWechatPayPlatsolutionInsuranceBatchService` | `QueryInsuranceBatchDetailByOutDetailNoAsync` | GET | `/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}` | 商户证书 / APIv3 签名 | 商户明细单号查询明细单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504224（【平台商户】，官方页更新于 2025.0… |
| 5 | `IWechatPayPlatsolutionInsuranceBatchService` | `QueryInsuranceBatchDetailByDetailIdAsync` | GET | `/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}` | 商户证书 / APIv3 签名 | 微信支付转账明细单号查询明细单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504223（【平台商户】，官方页更新于 20… |

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

**五支建四型**：批次两支查询（按 `out_batch_no` / 按 `batch_id`）应答**逐字段同形**（24 行， 含 `transfer_batch` 对象与 `data` 明细数组 + `offset` / `limit`）⇒ 共用 `InsuranceBatchQueryResponse`； 明细两支查询同形（17 行）⇒ 共用 `InsuranceBatchDetailQueryResponse`；受理支单建一型。

**受理成功 ≠ 转账成功（官方原文）**：受理应答只回批次号与 `batch_state`，须走查询支取付款状态； 幂等标识 `out_batch_no`、有效性 5 年 ⇒ 不得把受理应答的 `batch_state` 当终态判定。

**分页 query 只印在批次查询两支**：`need_query_detail`（必填）/ `offset` / `limit` / `detail_state`； 明细两支**零分页**且仅 `sub_mchid` 必填 query ⇒ 不得把批次查询的参数表套到明细查询上。

**与 `IWechatPayTransferService` / `IWechatPayTransferToQqWalletService` 不同契约**： 本域是**【平台商户】保险场景**的 `/v3/platsolution/insurance/mch-transfer/*`，应答多 `sp_mchid` / `sp_appid` / `sub_appid` 三层标识、字段名用 `batch_id`（不是 `transfer_bill_no`）⇒ DTO 不得与商家转账各域互相套用。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 5 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 1. `ApplyInsuranceBatchAsync` — 受理商家转账。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504220（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs)
- **官方契约**：`POST /v3/platsolution/insurance/mch-transfer/batches/apply`

- **请求体**：`InsuranceBatchApplyRequest` — 受理商家转账请求体，字段见 `InsuranceBatchApplyRequest`。
- **返回**：`InsuranceBatchApplyResponse` — 受理商家转账，见 `InsuranceBatchApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/insurance/mch-transfer/batches/apply`；无 path / query 参数。
- **应答**见 `InsuranceBatchApplyResponse`（官方应答表逐字段建模）。

### 2. `QueryInsuranceBatchByOutBatchNoAsync` — 商户转账批次单号查询批次单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504222（【平台商户】，官方页更新于 2025.03.11）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool` | 【是否查询转账明细单】 true-是；false-否，默认否。商户可选择是否查询指定状态的转账明细单，当转账批次单状态为“FINISHED”（已完成）时，才会返回满足条件的转账明细单（官方 query `need_query_detail`，**必填**，类型 `boolean`）。 |
| `offset` | Query `offset` | `int?` | 【请求资源起始位置】 该次请求资源（转账明细单）的起始位置，从0开始，默认值为0（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【最大资源条数】 该次请求可返回的最大资源（转账明细单）条数，最小20条，最大100条，不传则默认20条。不足20条按实际条数返回（官方 query `limit`，选填，类型 `integer`）。 |
| `detailState` | Query `detail_state` | `string?` | 【明细状态】 查询指定状态的转账明细单。。可选取值：`ALL` 全部。查询批次下所有明细单 / `WAIT_PAY` 待二级商户员工（转账验密人）确认, 符合免确认条件时, 系统会自动扭转为转账中 / `SUCCESS` 转账成功 / `FAIL` 需要确认失败原因后，再决定是否重新发起对该笔明细单的转账（并非整个转账批次单） / `BANK_REFUND` 银行返回转账成功后，拒绝入账，资金会返回商户账户。（官方 query `detail_state`，选填，类型 `string`）。 |
- **返回**：`InsuranceBatchQueryResponse` — 商户转账批次单号查询批次单，见 `InsuranceBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{out_batch_no}`；path `out_batch_no`，query `sub_mchid` / `need_query_detail` / `offset` / `limit` / `detail_state`。
- **`sub_mchid` 与 `need_query_detail` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 3. `QueryInsuranceBatchByBatchIdAsync` — 微信支付转账批次单号查询批次单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504221（【平台商户】，官方页更新于 2025.03.11）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方 path `batch_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool` | 【是否查询转账明细单】 true-是；false-否，默认否。商户可选择是否查询指定状态的转账明细单，当转账批次单状态为“FINISHED”（已完成）时，才会返回满足条件的转账明细单（官方 query `need_query_detail`，**必填**，类型 `boolean`）。 |
| `offset` | Query `offset` | `int?` | 【请求资源起始位置】 该次请求资源的起始位置。返回的明细是按照设置的明细条数进行分页展示的，一次查询可能无法返回所有明细，我们使用该参数标识查询开始位置，默认值为0（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【最大资源条数】 该次请求可返回的最大明细条数，最小20条，最大100条，不传则默认20条。不足20条按实际条数返回（官方 query `limit`，选填，类型 `integer`）。 |
| `detailState` | Query `detail_state` | `string?` | 【明细状态】 查询指定状态的转账明细单。。可选取值：`ALL` 全部。查询批次下所有明细单 / `WAIT_PAY` 待二级商户员工（转账验密人）确认, 符合免确认条件时, 系统会自动扭转为转账中 / `SUCCESS` 转账成功 / `FAIL` 需要确认失败原因后，再决定是否重新发起对该笔明细单的转账（并非整个转账批次单） / `BANK_REFUND` 银行返回转账成功后，拒绝入账，资金会返回商户账户。（官方 query `detail_state`，选填，类型 `string`）。 |
- **返回**：`InsuranceBatchQueryResponse` — 微信支付转账批次单号查询批次单，见 `InsuranceBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batch_id}`；path `batch_id`，query `sub_mchid` / `need_query_detail` / `offset` / `limit` / `detail_state`。
- **`sub_mchid` 与 `need_query_detail` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 4. `QueryInsuranceBatchDetailByOutDetailNoAsync` — 商户明细单号查询明细单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504224（【平台商户】，官方页更新于 2025.03.11）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `outDetailNo` | Path | `string` | 【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path `out_detail_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceBatchDetailQueryResponse` — 商户明细单号查询明细单，见 `InsuranceBatchDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}`；path `out_batch_no` / `out_detail_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 5. `QueryInsuranceBatchDetailByDetailIdAsync` — 微信支付转账明细单号查询明细单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504223（【平台商户】，官方页更新于 2025.03.11）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方 path `batch_id`，**必填**，类型 `string(64)`）。 |
| `detailId` | Path | `string` | 【商家转账明细单号】 微信商家转账系统内部区分转账批次单下不同转账明细单的唯一标识（官方 path `detail_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceBatchDetailQueryResponse` — 微信支付转账明细单号查询明细单，见 `InsuranceBatchDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}`；path `batch_id` / `detail_id`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/PlatsolutionInsuranceBatch`](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `PlatsolutionInsuranceBatch`。

AOT 源生成上下文：[`PlatsolutionInsuranceBatchJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/PlatsolutionInsuranceBatchJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `InsuranceBatchApplyResponse` | 应答 | [PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs) | `out_detail_no`, `transfer_amount`, `transfer_remark`, `reservation_id`, `openid`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount` … |
| `InsuranceBatchApplyRequest` | 请求体 | [PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs) | `out_detail_no`, `transfer_amount`, `transfer_remark`, `reservation_id`, `openid`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount` … |
| `InsuranceBatchQueryResponse` | 应答 | [PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs) | `out_detail_no`, `transfer_amount`, `transfer_remark`, `reservation_id`, `openid`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount` … |
| `InsuranceBatchDetailQueryResponse` | 应答 | [PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/PlatsolutionInsuranceBatchDtos.cs) | `out_detail_no`, `transfer_amount`, `transfer_remark`, `reservation_id`, `openid`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceBatch;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionInsuranceBatch));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionInsuranceBatchService>();
var ct = CancellationToken.None;
var response = await service.ApplyInsuranceBatchAsync(new InsuranceBatchApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/insurance/mch-transfer/batches/apply");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionInsuranceBatchService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyInsuranceBatchAsync`）；请求体 `InsuranceBatchApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionInsuranceBatchService](./IWechatPayPlatsolutionInsuranceBatchService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionInsuranceBatchContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionInsuranceBatchContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionInsuranceBatchTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionInsuranceBatchEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionInsuranceBatchDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionInsuranceBatchDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/PlatsolutionInsuranceBatch`](../../../Mud.Wechat.Pay/PlatsolutionInsuranceBatch/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionInsuranceBatchContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
