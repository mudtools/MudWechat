# PartnerTransfer 域 · 服务商批量转账到零钱

微信支付「服务商批量转账到零钱」域 SDK（**5 端点**：发起批量转账 1 + 批次查询 2 + 明细查询 2）。 接口：[IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs)；注册组 `PartnerTransfer`；模块枚举 `PayModule.PartnerTransfer`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPartnerTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransferContractGuards.cs)、[WechatPayBankTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankTransferContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPayrollCardContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayrollCardContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/PartnerTransfer`](../../DataModels/PartnerTransfer)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPartnerTransferService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPartnerTransferService` | `CreateBatchAsync` | POST | `/v3/partner-transfer/batches` | 商户证书 / APIv3 签名 | 服务商「发起批量转账」：一次提交一个转账批次（含多笔明细），返回微信支付批次单号。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40… |
| 2 | `IWechatPayPartnerTransferService` | `QueryBatchByBatchIdAsync` | GET | `/v3/partner-transfer/batches/batch-id/{batchId}` | 商户证书 / APIv3 签名 | 服务商「微信支付批次单号查询批次单」：按微信批次号查批次状态与（可选）明细列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401… |
| 3 | `IWechatPayPartnerTransferService` | `QueryDetailByDetailIdAsync` | GET | `/v3/partner-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}` | 商户证书 / APIv3 签名 | 服务商「微信支付明细单号查询明细单」：按微信明细号查单笔转账明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465781… |
| 4 | `IWechatPayPartnerTransferService` | `QueryBatchByOutBatchNoAsync` | GET | `/v3/partner-transfer/batches/out-batch-no/{outBatchNo}` | 商户证书 / APIv3 签名 | 服务商「商家批次单号查询批次单」：与 `QueryBatchByBatchIdAsync` 同表，以商户自有批次号定位。<br>官方文档：https://pay.weixin.qq.co… |
| 5 | `IWechatPayPartnerTransferService` | `QueryDetailByOutDetailNoAsync` | GET | `/v3/partner-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}` | 商户证书 / APIv3 签名 | 服务商「商家明细单号查询明细单」：与 `QueryDetailByDetailIdAsync` 同表，以商户自有明细号定位。<br>官方文档：https://pay.weixin.qq.… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（合作伙伴文档中心 ·「批量转账到零钱」，2026-10-14 逐页核验，
各页「支持商户」栏恒为【普通服务商】）：
发起批量转账 https://pay.weixin.qq.com/doc/v3/partner/4012465738、
微信支付批次单号查询批次单 https://pay.weixin.qq.com/doc/v3/partner/4012465772、
微信支付明细单号查询明细单 https://pay.weixin.qq.com/doc/v3/partner/4012465781、
商家批次单号查询批次单 https://pay.weixin.qq.com/doc/v3/partner/4012465790、
商家明细单号查询明细单 https://pay.weixin.qq.com/doc/v3/partner/4012465795。

**本域与 `Transfer`（直连商家转账）是两套契约**：
本域走 `/v3/partner-transfer/batches`、签名主体为**服务商商户号**、
报文用 `sub_mchid` / `sub_appid` / `sp_appid`；直连面（`IWechatPayTransferService`）
现行落位是**新版商家转账** `/v3/fund-app/mch-transfer/transfer-bills`（**按笔发起、无批次**）
⇒ 两域路由与 DTO 均不互通，路由交叠由守卫 PTF-B2 与 `PAY-B1` 同时锁定（本域亦零 `[Token]`）。

**批次单号一经发起不可更换**（官方硬约束）：批量转账**一旦发起不允许撤销**、
未查到明确批次结果前**不得换 `out_batch_no` 重提**、报错时**不要更换单号重试**
—— 换单重提会造成重复出资。SDK 侧不生成也不改写 `out_batch_no`，由宿主全权持有。

**异步受理 + 只可查近 30 天**：发起接口只回 `batch_id` 与受理时间，
转账结果须经查单轮询；批次/明细单只支持查询最近 30 天，更早的须登录商户平台。
「批次还在受理中」与「批次不存在」在官方语义上是**同一现象**（查不到 ≠ 失败），
官方明令此时须隔几分钟重查或**用同一 `out_batch_no` 再次发起**，
误判为受理失败的责任由商户自担。

**计数与状态字段名是本域特有的坑**：本域用 `total_num` / `success_num` / `fail_num`
与 `batch_status` / `detail_status`；`BankTransfer` 同族能力
用的是 `total_count` / `success_count` / `fail_count` 与 `batch_state` / `detail_state`
⇒ 跨域复制字段名会静默丢字段（守卫 PTF-B3 逐域锁定 snake_case）。

**明细应答的姓名字段是 `username`，明细请求的是 `user_name`**：
两处不同名，且值均为 RSA 密文（本 SDK 不解密）。

**本域批次上限一千笔**（银行面三千）；金额单位一律**分**。

**限频**：发起批量转账与两支查单各 **50QPS**（官方按接口分别标注），超频报
`FREQUENCY_LIMITED`（HTTP 429）⇒ 须降频**原单**重试。

**业务错误码**：`PARAM_ERROR` / `INVALID_REQUEST` / `SIGN_ERROR` /
`APPID_MCHID_NOT_MATCH` / `NO_AUTH` / `NOT_ENOUGH` / `ACCOUNT_ERROR` /
`QUOTA_EXCEED` / `FREQUENCY_LIMITED` / `SYSTEM_ERROR`，
原样透传至 `Code`。

**本章其余侧栏组不纳入本域**：转账电子回单五支（路由实测为 `/v3/transfer/bill-receipt`、
`/v3/transfer-detail/electronic-receipts`，**无 `partner-` 段**、与银行面
`/v3/bank-batch-transfer/receipt/` 不同族）、余额查询 / 提现 / 帐单 / 来账识别
⇒ 属后续增量，本域**零端点**（守卫 PTF-B2 带正对照）。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；服务商作用域经 `IWechatPayMerchantContext.UseMerchant(...)`
选择，`sub_mchid` 是**报文参数**而非商户作用域。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateBatchAsync` — 服务商「发起批量转账」：一次提交一个转账批次（含多笔明细），返回微信支付批次单号。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465738。

- **声明接口**：[IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs)
- **官方契约**：`POST /v3/partner-transfer/batches`

- **请求体**：`PartnerTransferBatchCreateRequest` — 请求体，字段见 `PartnerTransferBatchCreateRequest`。
- **返回**：`PartnerTransferBatchCreateResponse` — 仅含 `out_batch_no` / `batch_id` / `create_time` 的受理应答，见 `PartnerTransferBatchCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/partner-transfer/batches`；无 path / query 参数；限频 50QPS。
- **敏感字段须先加密**：明细 `user_name` / `user_id_card` 传 RSA 密文， / 请求头 `Wechatpay-Serial` 随之必填；未传入姓名时不得臆造明文。
- **受理 ≠ 转账成功**：本应答不含状态，须以返回的 `batch_id` 轮询查单端点。
- **报错后不得换单**：`SYSTEM_ERROR` / `FREQUENCY_LIMITED` 一律**原单**重试 / （官方原文：「请勿更换商家转账批次单号，请使用相同参数再次调用 API。否则可能造成资金损失」）。

### 2. `QueryBatchByBatchIdAsync` — 服务商「微信支付批次单号查询批次单」：按微信批次号查批次状态与（可选）明细列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465772。

- **声明接口**：[IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs)
- **官方契约**：`GET /v3/partner-transfer/batches/batch-id/{batchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 微信支付批次单号（官方 path `batch_id`，必填 string(64)）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool` | 是否查询转账明细单（官方 query `need_query_detail`，**必填** boolean）。 |
| `offset` | Query `offset` | `int?` | 明细起始位置（官方 query `offset`，选填 integer，默认 0）。 |
| `limit` | Query `limit` | `int?` | 明细最大条数（官方 query `limit`，选填 integer，20~100，默认 20）。 |
| `detailStatus` | Query `detail_status` | `string?` | 明细状态过滤（官方 query `detail_status`，选填，4 值）：见 `PartnerTransferDetailStateFilters`。 |
- **返回**：`PartnerTransferBatchQueryResponse` — 批次单及其明细概要，见 `PartnerTransferBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/partner-transfer/batches/batch-id/{batch_id}`。
- **`need_query_detail` 官方为必填 query**（不是「不传即不查」的选填） / ⇒ 声明式签名必须显式给出布尔值，缺省即 400 `PARAM_ERROR`。
- **过滤参数名是 `detail_status`**：银行面同名过滤叫 `detail_state`，值集也少一档 / （本域含 `WAIT_PAY`）。
- **明细列表只在批次 `FINISHED` 时返回**：批次处理中须改用两支明细查询端点。

### 3. `QueryDetailByDetailIdAsync` — 服务商「微信支付明细单号查询明细单」：按微信明细号查单笔转账明细。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465781。

- **声明接口**：[IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs)
- **官方契约**：`GET /v3/partner-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 微信支付批次单号（官方 path `batch_id`，必填 string(64)）。 |
| `detailId` | Path | `string` | 微信支付明细单号（官方 path `detail_id`，必填 string(64)）。 |
- **返回**：`PartnerTransferDetailQueryResponse` — 单笔明细，见 `PartnerTransferDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/partner-transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}`；无 query 参数。
- **明细查询无「是否查明细」开关**：与批次查询端点形态不同，本支直接回明细全字段。
- **本应答没有 `sub_mchid`**（批次应答有）：只有 `sp_mchid` + 选填 `appid`。
- **时间字段是 `initiate_time`** 而非 `create_time`，且 `username` 为密文。
- **查不到 ≠ 失败**：官方说明可能是「转账还在处理中」或「批次未开始处理」， / 须隔几分钟重查且**不换单**。

### 4. `QueryBatchByOutBatchNoAsync` — 服务商「商家批次单号查询批次单」：与 `QueryBatchByBatchIdAsync` 同表，以商户自有批次号定位。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465790。

- **声明接口**：[IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs)
- **官方契约**：`GET /v3/partner-transfer/batches/out-batch-no/{outBatchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 商家批次单号（官方 path `out_batch_no`，必填 string(32)）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool` | 是否查询转账明细单（官方 query `need_query_detail`，**必填** boolean）。 |
| `offset` | Query `offset` | `int?` | 明细起始位置（官方 query `offset`，选填 integer，默认 0）。 |
| `limit` | Query `limit` | `int?` | 明细最大条数（官方 query `limit`，选填 integer，20~100，默认 20）。 |
| `detailStatus` | Query `detail_status` | `string?` | 明细状态过滤（官方 query `detail_status`，选填，4 值）。 |
- **返回**：`PartnerTransferBatchQueryResponse` — 与微信单号支**同一应答形态**，见 `PartnerTransferBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/partner-transfer/batches/out-batch-no/{out_batch_no}`。
- **两支共用应答类型**：官方两页字段表逐字段一致 ⇒ 按「表相同则共用」归并 / （守卫 PTF-B6 断言两方法返回同一类型）。
- **本支是「受理中 / 不存在」歧义的处置入口**：官方允许「用同一 `out_batch_no` 再次发起」， / 故按商家单号查单比按微信单号更可靠（受理失败时 `batch_id` 可能从未落地）。

### 5. `QueryDetailByOutDetailNoAsync` — 服务商「商家明细单号查询明细单」：与 `QueryDetailByDetailIdAsync` 同表，以商户自有明细号定位。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465795。

- **声明接口**：[IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs)
- **官方契约**：`GET /v3/partner-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 商家批次单号（官方 path `out_batch_no`，必填 string(32)）。 |
| `outDetailNo` | Path | `string` | 商家明细单号（官方 path `out_detail_no`，必填 string(32)）。 |
- **返回**：`PartnerTransferDetailQueryResponse` — 与微信单号支**同一应答形态**，见 `PartnerTransferDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/partner-transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}`。
- **两层 path 定位**：本支须同时给批次号与明细号（官方路由形态），单给明细号无法定位。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/PartnerTransfer`](../../DataModels/PartnerTransfer) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `PartnerTransfer`。

AOT 源生成上下文：[`PartnerTransferJsonContext.g.cs`](./../../DataModels/Generated/PartnerTransferJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PartnerTransferBatchCreateResponse` | 应答 | [PartnerTransfer/PartnerTransferBatchDtos.cs](../../DataModels/PartnerTransfer/PartnerTransferBatchDtos.cs) | `sub_mchid`, `sub_appid`, `authorization_type`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount`, `total_num`, `transfer_detail_list`, `sp_appid`, `transfer_purpose`, `transfer_scene` … |
| `PartnerTransferBatchCreateRequest` | 请求体 | [PartnerTransfer/PartnerTransferBatchDtos.cs](../../DataModels/PartnerTransfer/PartnerTransferBatchDtos.cs) | `sub_mchid`, `sub_appid`, `authorization_type`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount`, `total_num`, `transfer_detail_list`, `sp_appid`, `transfer_purpose`, `transfer_scene` … |
| `PartnerTransferBatchQueryResponse` | 应答 | [PartnerTransfer/PartnerTransferBatchDtos.cs](../../DataModels/PartnerTransfer/PartnerTransferBatchDtos.cs) | `sub_mchid`, `sub_appid`, `authorization_type`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount`, `total_num`, `transfer_detail_list`, `sp_appid`, `transfer_purpose`, `transfer_scene` … |
| `PartnerTransferDetailQueryResponse` | 应答 | [PartnerTransfer/PartnerTransferDetailDtos.cs](../../DataModels/PartnerTransfer/PartnerTransferDetailDtos.cs) | `sp_mchid`, `out_batch_no`, `batch_id`, `appid`, `out_detail_no`, `detail_id`, `detail_status`, `transfer_amount`, `transfer_remark`, `fail_reason`, `openid`, `username` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PartnerTransfer;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PartnerTransfer));

var service = app.Services.GetRequiredService<IWechatPayPartnerTransferService>();
var ct = CancellationToken.None;
var response = await service.CreateBatchAsync(new PartnerTransferBatchCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/partner-transfer/batches");
// 占位值仅用于编译示意；业务值须按 IWechatPayPartnerTransferService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateBatchAsync`）；请求体 `PartnerTransferBatchCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPartnerTransferService](./IWechatPayPartnerTransferService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPartnerTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransferContractGuards.cs)、[`WechatPayBankTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankTransferContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPayrollCardContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayrollCardContractGuards.cs) |
| 守卫中的代表性测试方法 | `PartnerTransferEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`PartnerTransferRoutes_ShouldStayOnSinglePartnerTransferPrefixAndNotDeclareNeighbourGroups`、`PartnerTransferDtos_ShouldExposeOfficialFieldNamesAndDivergeFromBankDomain`、`PartnerTransferEnumConstants_ShouldMatchOfficialValuesAndDivergeFromBankDomain`、`PartnerTransfer_ShouldNotDeclareFailReasonOrErrorCodeConstants`、`PartnerTransferResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch`、`PartnerTransferDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`BankTransferEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankTransferRoutes_ShouldSplitAcrossTwoPrefixesAndNotDeclareDownloadOrForeignGroups`、`BankTransferDtos_ShouldExposeOfficialFieldNamesAndDivergeFromPartnerDomain`、`BankTransferEnumConstants_ShouldMatchOfficialValuesAndDivergeFromPartnerDomain`、`BankTransfer_ShouldNotDeclareFailReasonOrErrorCodeConstants` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/PartnerTransfer`](../../DataModels/PartnerTransfer) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPartnerTransferContractGuards.cs`、`WechatPayBankTransferContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayPayrollCardContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
