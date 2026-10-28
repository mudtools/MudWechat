# BankTransfer 域 · 从业机构（银行）批量转账到零钱 + 电子回单

微信支付「从业机构（银行）批量转账到零钱 + 电子回单」域 SDK（**9 端点**：转账 5 + 电子回单 4）。 接口：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)；注册组 `BankTransfer`；模块枚举 `PayModule.BankTransfer`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBankTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankTransferContractGuards.cs)、[WechatPayCapitalContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCapitalContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPartnerTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransferContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/BankTransfer`](../../../Mud.Wechat.Pay/BankTransfer/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBankTransferService = 9；按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBankTransferService` | `CreateBatchAsync` | POST | `/v3/bank-transfer/batches` | 商户证书 / APIv3 签名 | 从业机构「发起批量转账」：一次提交一个转账批次（含多笔明细），返回微信批次号。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012… |
| 2 | `IWechatPayBankTransferService` | `QueryBatchByBatchIdAsync` | GET | `/v3/bank-transfer/batches/batch-id/{batchId}` | 商户证书 / APIv3 签名 | 从业机构「微信支付批次单号查询批次单」：按微信批次号查批次状态与（可选）明细列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40… |
| 3 | `IWechatPayBankTransferService` | `QueryBatchByOutBatchNoAsync` | GET | `/v3/bank-transfer/batches/out-batch-no/{outBatchNo}` | 商户证书 / APIv3 签名 | 从业机构「商家批次单号查询批次单」：与 `QueryBatchByBatchIdAsync` 同表，以银行自有批次号定位。<br>官方文档：https://pay.weixin.qq.c… |
| 4 | `IWechatPayBankTransferService` | `QueryDetailByDetailIdAsync` | GET | `/v3/bank-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}` | 商户证书 / APIv3 签名 | 从业机构「微信支付明细单号查询明细单」：按微信明细号查单笔转账明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401246589… |
| 5 | `IWechatPayBankTransferService` | `QueryDetailByOutDetailNoAsync` | GET | `/v3/bank-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}` | 商户证书 / APIv3 签名 | 从业机构「商家明细单号查询明细单」：与 `QueryDetailByDetailIdAsync` 同表，以银行自有明细号定位。<br>官方文档：https://pay.weixin.qq… |
| 6 | `IWechatPayBankTransferService` | `AcceptSummaryReceiptAsync` | POST | `/v3/bank-batch-transfer/receipt/summary-receipts` | 商户证书 / APIv3 签名 | 从业机构「受理汇总电子回单」：提交批次汇总回单的申请（异步生成文件）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40124707… |
| 7 | `IWechatPayBankTransferService` | `QuerySummaryReceiptAsync` | GET | `/v3/bank-batch-transfer/receipt/summary-receipts` | 商户证书 / APIv3 签名 | 从业机构「查询汇总电子回单受理结果」：与 `AcceptSummaryReceiptAsync` 同路径，按商家批次号定位。<br>官方文档：https://pay.weixin.qq.… |
| 8 | `IWechatPayBankTransferService` | `AcceptDetailReceiptAsync` | POST | `/v3/bank-batch-transfer/receipt/detail-receipts` | 商户证书 / APIv3 签名 | 从业机构「受理明细电子回单」：提交单笔转账明细的回单申请（异步生成文件）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401247… |
| 9 | `IWechatPayBankTransferService` | `QueryDetailReceiptAsync` | GET | `/v3/bank-batch-transfer/receipt/detail-receipts` | 商户证书 / APIv3 签名 | 从业机构「查询明细电子回单受理结果」：与 `AcceptDetailReceiptAsync` 同路径，按商家批次号 + 明细号定位。<br>官方文档：https://pay.weixi… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（合作伙伴文档中心 ·「开发文档(银行/机构)」树，2026-10-14 逐页核验，
各页「支持商户」栏恒为【从业机构（银行）】）：发起批量转账
https://pay.weixin.qq.com/doc/v3/partner/4012465889、微信支付批次单号查询批次单
https://pay.weixin.qq.com/doc/v3/partner/4012465891、微信支付明细单号查询明细单
https://pay.weixin.qq.com/doc/v3/partner/4012465897、商家批次单号查询批次单
https://pay.weixin.qq.com/doc/v3/partner/4012465903、商家明细单号查询明细单
https://pay.weixin.qq.com/doc/v3/partner/4012465915；受理汇总电子回单
https://pay.weixin.qq.com/doc/v3/partner/4012470746、查询汇总电子回单
https://pay.weixin.qq.com/doc/v3/partner/4012470761、受理明细电子回单
https://pay.weixin.qq.com/doc/v3/partner/4012470770、查询明细电子回单
https://pay.weixin.qq.com/doc/v3/partner/4012470775。

**两套路由前缀同域共存**：转账族走 `/v3/bank-transfer/batches`，电子回单族走
`/v3/bank-batch-transfer/receipt/` ⇒ 同名不成立（守卫 BT-B1 逐端点比对 (方法, 路由)）。
`PartnerTransfer`（服务商面）则是 `/v3/partner-transfer/batches` 单前缀。

**本域商户标识是六层，而非服务商面的两层**：包体必填 `platform_mchid` /
`specialized_payment_mchid` / `sponsor_mchid` / `user_authorized_mchid` /
`user_authorized_appid`，应答另加 `bank_sp_mchid`。签名主体是**从业机构商户号**
⇒ 宿主以从业机构商户配置进入作用域，五标识全为**报文参数**而非作用域。

**字段名与服务商面同源不同形**（照抄必静默丢字段，守卫 BT-B3/BT-B4 逐域锁定）：
本域 `total_count` / `success_count` / `fail_count` 对服务商面
`total_num` / `success_num` / `fail_num`；本域 `batch_state`（4 值）/
`detail_state`（4 值）对服务商面 `batch_status`（5 值）/ `detail_status`（6 值）；
本域明细过滤 query 为 `detail_state`（3 值）对服务商面 `detail_status`（4 值）。
本域**没有** `WAIT_PAY` / `INIT` 档（出资方直接扣款，无付款确认环节）。

**批次上限三千笔**（服务商面一千）；金额单位一律**分**。

**电子回单是「同一路径、POST 与 GET 各自成端点」**：受理走 POST、查受理结果走 GET，
路径完全相同、定位键同名（汇总支 `out_batch_no`、明细支 `+out_detail_no`）
⇒ **端点计数须按 (方法, 路由) 二元组**，否则 9 支被数成 7 支（守卫 BT-B1）。

**回单生成为异步**：受理成功只回 `receipt_state = ACCEPTED`，
`hash_type` / `hash_value` / `download_url` 官方注明仅在 `FINISHED` 时出现
⇒ 受理后立刻读 URL 必为 `null`，须按状态轮询。**`download_url` 不进生成式接口**
（动态路由 + 非 JSON，同账单下载通道形态，守卫 BT-B2 零声明断言带正对照）。

**姓名密文两域同名不同必填**：本域明细 `user_name` **必填**（应答字段名为
`username`，亦必填）、`user_id_card` 选填；服务商面两者皆选填。
敏感字段须 RSA 加密后传入（本 SDK 不做加解密，由 `Wechatpay-Serial` 承载证书序列号）。

**限频**：发起批量转账 50QPS（官方标注）；超频报 `FREQUENCY_LIMITED`（HTTP 429）
⇒ 降频后**原单**重试，不得更换 `out_batch_no`。

**业务错误码**：`PARAM_ERROR` / `INVALID_REQUEST` / `SIGN_ERROR` /
`NO_AUTH`（官方两行不同文案，分别对应「权限未开通」与「出资方授权关系不符」）/
`NOT_ENOUGH` / `ACCOUNT_ERROR` / `QUOTA_EXCEED` / `FREQUENCY_LIMITED` /
`SYSTEM_ERROR`，原样透传至 `Code`。

**本章其余侧栏组不纳入本域**：来账识别、余额查询、余额提现、资金账单
（及服务商面的转账电子回单五支）⇒ 属后续增量，本域**零端点**（守卫 BT-B2 带正对照）。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse` 的
`Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateBatchAsync` — 从业机构「发起批量转账」：一次提交一个转账批次（含多笔明细），返回微信批次号。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465889。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`POST /v3/bank-transfer/batches`

- **请求体**：`BankTransferBatchCreateRequest` — 请求体，字段见 `BankTransferBatchCreateRequest`。
- **返回**：`BankTransferBatchCreateResponse` — 仅含 `out_batch_no` / `batch_id` / `create_time` 的受理应答，见 `BankTransferBatchCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/bank-transfer/batches`；无 path / query 参数；限频 50QPS。
- **五个商户 / 应用标识全为必填包体字段**（平台商户号、专项支付商户号、出资商户号、 / 被授权商户号与其 appid）：本域不提供任何默认值，错配即 `NO_AUTH`。 / 官方在权限说明里明确「`sponsor_mchid` 须与 `user_authorized_mchid` 建立授权关系」。
- **`transfer_detail_list` 官方标为选填**（与服务商面的必填相反）： / 允许先建空批次、后续逐笔追加 ⇒ 传空列表不报 `PARAM_ERROR`，勿按服务商面经验校验。
- **`transfer_scene` / `transfer_purpose` 本面必填**（服务商面选填）， / 且 `PAYROLL_CARD_TRANSFER` 场景下明细 `user_name` 密文必填。
- **受理 ≠ 转账成功**：应答不含状态，须以 `batch_id` 或 `out_batch_no` 轮询查单。

### 2. `QueryBatchByBatchIdAsync` — 从业机构「微信支付批次单号查询批次单」：按微信批次号查批次状态与（可选）明细列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465891。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`GET /v3/bank-transfer/batches/batch-id/{batchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 微信支付批次单号（官方 path `batch_id`，必填 string(64)）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool` | 是否查询转账明细单（官方 query `need_query_detail`，**必填** boolean）。 |
| `offset` | Query `offset` | `int?` | 明细起始位置（官方 query `offset`，选填 integer，默认 0）。 |
| `limit` | Query `limit` | `int?` | 明细最大条数（官方 query `limit`，选填 integer，20~100，默认 20）。 |
| `detailState` | Query `detail_state` | `string?` | 明细状态过滤（官方 query `detail_state`，选填，3 值）：见 `BankTransferDetailStateFilters`。 |
- **返回**：`BankTransferBatchQueryResponse` — 批次单及其明细概要，见 `BankTransferBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/bank-transfer/batches/batch-id/{batch_id}`。
- **过滤参数名是 `detail_state`**（不是服务商面的 `detail_status`）， / 值集为 ALL / SUCCESS / FAIL **三档**（服务商面四档、含 `WAIT_PAY`）。
- **`need_query_detail` 官方为必填 query**：缺省即 400 `PARAM_ERROR`。
- **明细列表只在批次 `FINISHED` 时返回**：处理中须改用两支明细查询端点。

### 3. `QueryBatchByOutBatchNoAsync` — 从业机构「商家批次单号查询批次单」：与 `QueryBatchByBatchIdAsync` 同表，以银行自有批次号定位。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465903。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`GET /v3/bank-transfer/batches/out-batch-no/{outBatchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 银行批次单号（官方 path `out_batch_no`，必填 string(32)）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool` | 是否查询转账明细单（官方 query `need_query_detail`，**必填** boolean）。 |
| `offset` | Query `offset` | `int?` | 明细起始位置（官方 query `offset`，选填 integer，默认 0）。 |
| `limit` | Query `limit` | `int?` | 明细最大条数（官方 query `limit`，选填 integer，20~100，默认 20）。 |
| `detailState` | Query `detail_state` | `string?` | 明细状态过滤（官方 query `detail_state`，选填，3 值）。 |
- **返回**：`BankTransferBatchQueryResponse` — 与微信单号支**同一应答形态**，见 `BankTransferBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/bank-transfer/batches/out-batch-no/{out_batch_no}`。
- **两支共用应答类型**：官方两页字段表逐字段一致 ⇒ 按「表相同则共用」归并 / （守卫 BT-B6 断言两方法返回同一类型）。

### 4. `QueryDetailByDetailIdAsync` — 从业机构「微信支付明细单号查询明细单」：按微信明细号查单笔转账明细。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465897。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`GET /v3/bank-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 微信支付批次单号（官方 path `batch_id`，必填 string(64)）。 |
| `detailId` | Path | `string` | 微信支付明细单号（官方 path `detail_id`，必填 string(64)）。 |
- **返回**：`BankTransferDetailQueryResponse` — 单笔明细，见 `BankTransferDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/bank-transfer/batches/batch-id/{batch_id}/details/detail-id/{detail_id}`；无 query 参数。
- **明细应答的 `username` 必填**（服务商面选填），且本支字段名是 `username`、 / 请求侧是 `user_name` —— 两处不同名。
- **`account_type` 本面必填、`initiate_time` 本面选填**： / 与服务商面正好相反（那里 `account_type` 选填、`initiate_time` 必填）。
- **官方本页未附 `account_type` 取值表** ⇒ 本域按 / `PartnerTransfer` / 的 WECHATPAY_ACCOUNT / BANK_ACCOUNT 语义使用，但**不得**据此做穷尽校验（守卫 BT-B4 只锁 / 有官方取值表的集合）。
- **查不到 ≠ 失败**：官方说明可能是批次尚未开始处理，须隔几分钟重查且**不换单**。

### 5. `QueryDetailByOutDetailNoAsync` — 从业机构「商家明细单号查询明细单」：与 `QueryDetailByDetailIdAsync` 同表，以银行自有明细号定位。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012465915。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`GET /v3/bank-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 银行批次单号（官方 path `out_batch_no`，必填 string(32)）。 |
| `outDetailNo` | Path | `string` | 银行明细单号（官方 path `out_detail_no`，必填 string(32)）。 |
- **返回**：`BankTransferDetailQueryResponse` — 与微信单号支**同一应答形态**，见 `BankTransferDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/bank-transfer/batches/out-batch-no/{out_batch_no}/details/out-detail-no/{out_detail_no}`。
- **两层 path 定位**：须同时给批次号与明细号，单给明细号无法定位。

### 6. `AcceptSummaryReceiptAsync` — 从业机构「受理汇总电子回单」：提交批次汇总回单的申请（异步生成文件）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012470746。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`POST /v3/bank-batch-transfer/receipt/summary-receipts`

- **请求体**：`BankTransferSummaryReceiptRequest` — 请求体，字段见 `BankTransferSummaryReceiptRequest`。
- **返回**：`BankTransferSummaryReceiptResponse` — 回单受理结果，见 `BankTransferSummaryReceiptResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/bank-batch-transfer/receipt/summary-receipts`； / 注意前缀是 `bank-batch-transfer`，与转账族的 `bank-transfer` **不同**。
- **与 `QuerySummaryReceiptAsync` 同一路径**：本支 POST 发起、后者 GET 查询， / 共用同一应答类型 ⇒ 计数按 (方法, 路由)（守卫 BT-B1）。
- **仅已完成批次可受理**：官方要求 `out_batch_no` 指向**已完成**的批次， / 处理中申请会失败。

### 7. `QuerySummaryReceiptAsync` — 从业机构「查询汇总电子回单受理结果」：与 `AcceptSummaryReceiptAsync` 同路径，按商家批次号定位。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012470761。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`GET /v3/bank-batch-transfer/receipt/summary-receipts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Query `out_batch_no` | `string` | 银行批次单号（官方 query `out_batch_no`，必填 string(32)）。 |
- **返回**：`BankTransferSummaryReceiptResponse` — 与受理支**同一应答形态**，见 `BankTransferSummaryReceiptResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/bank-batch-transfer/receipt/summary-receipts?out_batch_no={out_batch_no}`。
- **定位键从包体换成 query，参数名不变**（`out_batch_no`）。
- **`download_url` 只在 `receipt_state = FINISHED` 时出现**： / 取到 URL 后的文件下载**不在本域**（动态路由 + 非 JSON，守卫 BT-B2）。

### 8. `AcceptDetailReceiptAsync` — 从业机构「受理明细电子回单」：提交单笔转账明细的回单申请（异步生成文件）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012470770。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`POST /v3/bank-batch-transfer/receipt/detail-receipts`

- **请求体**：`BankTransferDetailReceiptRequest` — 请求体，字段见 `BankTransferDetailReceiptRequest`。
- **返回**：`BankTransferDetailReceiptResponse` — 回单受理结果，见 `BankTransferDetailReceiptResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/bank-batch-transfer/receipt/detail-receipts`。
- **比汇总支多一个定位键**：`out_batch_no` + `out_detail_no` 皆必填。
- **明细应答与汇总应答不共用类型**：本型多 `out_detail_no`、 / 但**没有** `create_time` / `update_time`（「表相同则共用，表不同则分建」，守卫 BT-B6）。

### 9. `QueryDetailReceiptAsync` — 从业机构「查询明细电子回单受理结果」：与 `AcceptDetailReceiptAsync` 同路径，按商家批次号 + 明细号定位。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012470775。

- **声明接口**：[IWechatPayBankTransferService](./IWechatPayBankTransferService.cs)
- **官方契约**：`GET /v3/bank-batch-transfer/receipt/detail-receipts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Query `out_batch_no` | `string` | 银行批次单号（官方 query `out_batch_no`，必填 string(32)）。 |
| `outDetailNo` | Query `out_detail_no` | `string` | 银行明细单号（官方 query `out_detail_no`，必填 string(32)）。 |
- **返回**：`BankTransferDetailReceiptResponse` — 与受理支**同一应答形态**，见 `BankTransferDetailReceiptResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/bank-batch-transfer/receipt/detail-receipts?out_batch_no=…&out_detail_no=…`。
- **两支皆必填 query**：缺任一即 `PARAM_ERROR`（官方未提供默认值）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/BankTransfer`](../../../Mud.Wechat.Pay/BankTransfer/) 内共 **10** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **10** 个，`SerializerClassName` = `BankTransfer`。

AOT 源生成上下文：[`BankTransferJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/BankTransferJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BankTransferBatchCreateResponse` | 应答 | [BankTransfer/BankTransferBatchDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferBatchDtos.cs) | `platform_mchid`, `specialized_payment_mchid`, `sponsor_mchid`, `user_authorized_mchid`, `user_authorized_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `attach_remark`, `total_amount`, `total_count`, `transfer_scene` … |
| `BankTransferBatchCreateRequest` | 请求体 | [BankTransfer/BankTransferBatchDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferBatchDtos.cs) | `platform_mchid`, `specialized_payment_mchid`, `sponsor_mchid`, `user_authorized_mchid`, `user_authorized_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `attach_remark`, `total_amount`, `total_count`, `transfer_scene` … |
| `BankTransferBatchQueryResponse` | 应答 | [BankTransfer/BankTransferBatchDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferBatchDtos.cs) | `platform_mchid`, `specialized_payment_mchid`, `sponsor_mchid`, `user_authorized_mchid`, `user_authorized_appid`, `out_batch_no`, `batch_name`, `batch_remark`, `attach_remark`, `total_amount`, `total_count`, `transfer_scene` … |
| `BankTransferDetailQueryResponse` | 应答 | [BankTransfer/BankTransferDetailDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferDetailDtos.cs) | `bank_sp_mchid`, `platform_mchid`, `specialized_payment_mchid`, `sponsor_mchid`, `user_authorized_mchid`, `user_authorized_appid`, `out_batch_no`, `batch_id`, `transfer_scene`, `out_detail_no`, `detail_id`, `openid` … |
| `BankTransferSummaryReceiptResponse` | 应答 | [BankTransfer/BankTransferReceiptDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferReceiptDtos.cs) | `out_batch_no`, `receipt_no`, `receipt_state`, `hash_type`, `hash_value`, `download_url`, `create_time`, `update_time`, `out_detail_no` |
| `BankTransferSummaryReceiptRequest` | 请求体 | [BankTransfer/BankTransferReceiptDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferReceiptDtos.cs) | `out_batch_no`, `receipt_no`, `receipt_state`, `hash_type`, `hash_value`, `download_url`, `create_time`, `update_time`, `out_detail_no` |
| `BankTransferDetailReceiptResponse` | 应答 | [BankTransfer/BankTransferReceiptDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferReceiptDtos.cs) | `out_batch_no`, `receipt_no`, `receipt_state`, `hash_type`, `hash_value`, `download_url`, `create_time`, `update_time`, `out_detail_no` |
| `BankTransferDetailReceiptRequest` | 请求体 | [BankTransfer/BankTransferReceiptDtos.cs](../../../Mud.Wechat.Pay/BankTransfer/BankTransferReceiptDtos.cs) | `out_batch_no`, `receipt_no`, `receipt_state`, `hash_type`, `hash_value`, `download_url`, `create_time`, `update_time`, `out_detail_no` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BankTransfer;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BankTransfer));

var service = app.Services.GetRequiredService<IWechatPayBankTransferService>();
var ct = CancellationToken.None;
var response = await service.AcceptDetailReceiptAsync(new BankTransferDetailReceiptRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/bank-batch-transfer/receipt/detail-receipts");
// 占位值仅用于编译示意；业务值须按 IWechatPayBankTransferService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AcceptDetailReceiptAsync`）；请求体 `BankTransferDetailReceiptRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBankTransferService](./IWechatPayBankTransferService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBankTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankTransferContractGuards.cs)、[`WechatPayCapitalContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCapitalContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPartnerTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransferContractGuards.cs) |
| 守卫中的代表性测试方法 | `BankTransferEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankTransferRoutes_ShouldSplitAcrossTwoPrefixesAndNotDeclareDownloadOrForeignGroups`、`BankTransferDtos_ShouldExposeOfficialFieldNamesAndDivergeFromPartnerDomain`、`BankTransferEnumConstants_ShouldMatchOfficialValuesAndDivergeFromPartnerDomain`、`BankTransfer_ShouldNotDeclareFailReasonOrErrorCodeConstants`、`BankTransferResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch`、`BankTransferDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`CapitalTopology_ShouldMatchInterfaceAndGroupRoster`、`CapitalEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`CapitalDtos_ShouldExposeOfficialFieldRosters`、`CapitalDataModels_ShouldSplitRequestAndResponseBaseTypes`、`CapitalAllDtos_ShouldBeAnnotatedAndContextRegistered` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/BankTransfer`](../../../Mud.Wechat.Pay/BankTransfer/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBankTransferContractGuards.cs`、`WechatPayCapitalContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayPartnerTransferContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
