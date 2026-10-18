# ProfitSharing 域 · 分账

微信支付「分账」域 SDK（APIv3 分账，**7 端点**：添加接收方 / 请求分账 / 查询分账结果 /
请求分账回退 / 查询分账回退结果 / 解冻剩余资金 / 查询剩余待分金额）。 接口：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)；注册组 `ProfitSharing`；模块枚举 `PayModule.ProfitSharing`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayProfitSharingContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayProfitSharingContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayEcommerceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEcommerceContractGuards.cs)、[WechatPaySmartGuideContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPaySmartGuideContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/ProfitSharing`](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayProfitSharingService = 9；按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayProfitSharingService` | `AddReceiverAsync` | POST | `/v3/profitsharing/receivers/add` | 商户证书 / APIv3 签名 | 添加分账接收方。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012528995。 |
| 2 | `IWechatPayProfitSharingService` | `CreateOrderAsync` | POST | `/v3/profitsharing/orders` | 商户证书 / APIv3 签名 | 请求分账。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524936。 |
| 3 | `IWechatPayProfitSharingService` | `QueryOrderAsync` | GET | `/v3/profitsharing/orders/{outOrderNo}` | 商户证书 / APIv3 签名 | 查询分账结果。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_2.shtml。 |
| 4 | `IWechatPayProfitSharingService` | `CreateReturnOrderAsync` | POST | `/v3/profitsharing/return-orders` | 商户证书 / APIv3 签名 | 请求分账回退。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_3.shtml。 |
| 5 | `IWechatPayProfitSharingService` | `QueryReturnOrderAsync` | GET | `/v3/profitsharing/return-orders/{outReturnNo}` | 商户证书 / APIv3 签名 | 查询分账回退结果。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_4.shtml。 |
| 6 | `IWechatPayProfitSharingService` | `UnfreezeAsync` | POST | `/v3/profitsharing/orders/unfreeze` | 商户证书 / APIv3 签名 | 解冻剩余资金。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_5.shtml。 |
| 7 | `IWechatPayProfitSharingService` | `QueryUnsplitAmountAsync` | GET | `/v3/profitsharing/transactions/{transactionId}/amounts` | 商户证书 / APIv3 签名 | 查询剩余待分金额。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_6.shtml。 |
| 8 | `IWechatPayProfitSharingService` | `GetBillAsync` | GET | `/v3/profitsharing/bills` | 商户证书 / APIv3 签名 | 申请分账账单。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529628。 |
| 9 | `IWechatPayProfitSharingService` | `DeleteReceiverAsync` | POST | `/v3/profitsharing/receivers/delete` | 商户证书 / APIv3 签名 | 删除分账接收方。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529590。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（普通商户文档中心，2026-10-09 逐页核验，页面更新时间均为 2025.09.29）：
请求分账 https://pay.weixin.qq.com/doc/v3/merchant/4012524936、
添加分账接收方 https://pay.weixin.qq.com/doc/v3/merchant/4012528995、
查询分账结果 https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_2.shtml。
支持商户类型：**普通商户**（三页均标注）。

**✅ 本域现已 9 端点**（原「尚未覆盖」清单已随增量关闭）：请求分账 · 添加 / **删除**分账接收方 ·
查询分账结果 · **请求分账回退** · **查询分账回退结果** · **解冻剩余资金** ·
**查询剩余待分金额** · **申请分账账单**。

**两个资源族别混**：「分账单」在 `/v3/profitsharing/orders…`，「回退单」在
`/v3/profitsharing/return-orders…` —— 回退**不是**分账单的子资源（实测纠正了先验预判）。

**回调面**：分账动态通知已支持（归回调包），且**必须**靠
`resource.original_type = profitsharing` 与其它产品线区分（分账复用交易的
`event_type`，不像支付分那样自带前缀）——这是本仓回调面已经踩实的一处形态差异。

**不要凭推断补路由**——官方新文档站不暴露兄弟页 `docId`，每个端点须按接口名检索到其页面再核验
（已实测可行路径：`/wiki/doc/apiv3/apis/chapter8_1_N.shtml` 家族逐页核对标题）。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `AddReceiverAsync` — 添加分账接收方。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012528995。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`POST /v3/profitsharing/receivers/add`

- **请求体**：`ProfitSharingAddReceiverRequest` — 添加接收方请求体，见 `ProfitSharingAddReceiverRequest`。
- **返回**：`ProfitSharingReceiver` — 已建立的接收方关系（官方应答与请求同字段），见 `ProfitSharingReceiver`。

**备注（原文）**

- **官方契约**：**POST** `/v3/profitsharing/receivers/add`；无 path / query 参数； / 必带 `Accept: application/json` 与 `Content-Type: application/json`。
- **幂等与上限**：官方明示「同一接收方重复添加」由微信侧处理；商户接收方数量**上限 2 万**， / 达上限须先删除未使用的接收方。
- **错误码**：`PARAM_ERROR` / `INVALID_REQUEST` / `SIGN_ERROR` / / `NO_AUTH` / `FREQUENCY_LIMITED`（添加接收方频率过高）/ `SYSTEM_ERROR`。
- **敏感字段**：`name` 须按官方用**微信支付公钥**（推荐）或平台证书公钥以 / **RSAES-OAEP** 加密，并把 `Wechatpay-Serial` 指向对应公钥 —— 该头由传输层按配置写入。

### 2. `CreateOrderAsync` — 请求分账。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524936。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`POST /v3/profitsharing/orders`

- **请求体**：`ProfitSharingOrderRequest` — 请求分账请求体，见 `ProfitSharingOrderRequest`。
- **返回**：`ProfitSharingOrderResponse` — 分账单（**异步受理**：通常先返回 `PROCESSING`），见 `ProfitSharingOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/profitsharing/orders`；无 path / query 参数。
- **异步处理模式（官方原文）**：接口采用异步处理，受理后先返回，最终结果须经 / `QueryOrderAsync` 查询或分账动态通知获取 ⇒ **调用方不得把受理成功当成分账到账**。
- **幂等**：官方明示「同一分账单号（`out_order_no`）多次请求等同一次」。
- **错误码**：`RULE_LIMIT`（超出最大分账比例）/ `NOT_ENOUGH`（分账金额不足）/ / `NO_AUTH` / `FREQUENCY_LIMITED`（同笔订单分账频率过高）等。

### 3. `QueryOrderAsync` — 查询分账结果。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_2.shtml。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`GET /v3/profitsharing/orders/{outOrderNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户分账单号（官方 path `out_order_no`，必填 string(64)）。 |
| `transactionId` | Query `transaction_id` | `string` | 微信支付订单号（官方 query `transaction_id`，必填 string(32)）。 |
- **返回**：`ProfitSharingOrderResponse` — 分账结果（与请求分账**同一应答形态**），见 `ProfitSharingOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/profitsharing/orders/{out_order_no}?transaction_id=…`； / path 必带 `out_order_no`、query 必带 `transaction_id`（官方两参数均标注必填）。
- **占位符写法**：路由占位符用的是 **C# 参数名**（`{outOrderNo}`）而非官方 `out_order_no` —— / 生成器**强制**「占位符 ↔ `[Path]` 参数名」一一对应（违反即 `HTTPCLIENT013`）， / 且占位符名**不出现在报文里**（只做替换），故对线上契约零影响。与交易域既有端点同款。
- **一接口两用途（官方原文）**：既查分账结果，也查**解冻剩余资金**的执行结果 / （解冻请求本身走另一端点，本批未实现）。
- **错误码**：含 `404 RESOURCE_NOT_EXISTS`（记录不存在 ⇒ 检查单号）、 / `429 FREQUENCY_LIMITED`（查询频率过高）。

### 4. `CreateReturnOrderAsync` — 请求分账回退。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_3.shtml。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`POST /v3/profitsharing/return-orders`

- **请求体**：`ProfitSharingReturnOrderRequest` — 回退请求体，见 `ProfitSharingReturnOrderRequest`。
- **返回**：`ProfitSharingReturnOrderResponse` — 回退单（异步受理：通常先 `PROCESSING`），见 `ProfitSharingReturnOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/profitsharing/return-orders`；**无** path / query 参数。
- **⚠️ 路由纠偏**：回退是**独立资源族**，不是 `/orders/{out_order_no}/return` / （官方页面实测如此，勿凭直觉补）。
- **二选一定位**：`order_id`（微信分账单号）与 `out_order_no`（商户分账单号） / **二选一**填写，两者皆选填但不可都不填。
- **回退方约束**：`return_mchid` **只能**是原分账单中某个接收方的商户号； / `amount` 不得超过原分账金额。

### 5. `QueryReturnOrderAsync` — 查询分账回退结果。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_4.shtml。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`GET /v3/profitsharing/return-orders/{outReturnNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outReturnNo` | Path | `string` | 商户回退单号（官方 path `out_return_no`，必填 string(64)）。 |
| `outOrderNo` | Query `out_order_no` | `string` | 商户分账单号（官方 query `out_order_no`，必填 string(64)）。 |
- **返回**：`ProfitSharingReturnOrderResponse` — 回退结果（与请求分账回退**同一应答形态**），见 `ProfitSharingReturnOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/profitsharing/return-orders/{out_return_no}?out_order_no=…`； / path 必带 `out_return_no`、query 必带 `out_order_no`。
- **终态判定**：`result` 取 `PROCESSING`（非终态）/ `SUCCESS` / `FAILED`； / 与分账单的 `state`（`PROCESSING`/`FINISHED`）是**两套枚举**，勿混用。

### 6. `UnfreezeAsync` — 解冻剩余资金。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_5.shtml。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`POST /v3/profitsharing/orders/unfreeze`

- **请求体**：`ProfitSharingUnfreezeRequest` — 解冻请求体，见 `ProfitSharingUnfreezeRequest`。
- **返回**：`ProfitSharingOrderResponse` — 分账单（**应答形态与请求分账相同** ⇒ 复用 `ProfitSharingOrderResponse`）。

**备注（原文）**

- **官方契约**：**POST** `/v3/profitsharing/orders/unfreeze`；无 path / query 参数。
- **与「请求分账」的关系**：`unfreeze_unsplit=false` 允许对同一笔订单**多次**分账； / 分完后调用本端点把**剩余**未分金额全部解冻给发起方 —— 一旦解冻即**不可再次分账**。 / 查结果用 `QueryOrderAsync`（官方该页注明它兼查解冻结果）。
- **幂等**：官方原文「同一 `out_order_no` 多次请求等同一次」。

### 7. `QueryUnsplitAmountAsync` — 查询剩余待分金额。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_6.shtml。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`GET /v3/profitsharing/transactions/{transactionId}/amounts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 微信支付订单号（官方 path `transaction_id`，必填 string(32)）。 |
- **返回**：`ProfitSharingAmountsResponse` — 剩余待分金额（`unsplit_amount`，单位分），见 `ProfitSharingAmountsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/profitsharing/transactions/{transaction_id}/amounts`； / 仅 path 参数 `transaction_id`，无 body / query。**用途**：分账前先探余额， / 避免 `NOT_ENOUGH`（分账金额不足）类拒绝（官方常见问题即以此接口确认剩余可分金额）。

### 8. `GetBillAsync` — 申请分账账单。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529628。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`GET /v3/profitsharing/bills`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billDate` | Query `bill_date` | `string` | 账单日期（官方 query `bill_date`，必填 string(10)，格式 `yyyy-MM-dd`）。 |
| `tarType` | Query `tar_type` | `string?` | 压缩类型（官方 query `tar_type`，选填，取值 `GZIP`；不传则返回原始文件）。 |
- **返回**：`BillDownloadInfoResponse` — 账单下载信息（`hash_type` / `hash_value` / `download_url`），见 `BillDownloadInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/profitsharing/bills?bill_date=…[&tar_type=…]`； / 无 path / body 参数（2026-10-09 核验，更新时间 2025.09.29）。
- **应答为何复用 `BillDownloadInfoResponse`**：官方本页应答字段表为 / `hash_type` / `hash_value` / `download_url`，与「申请交易账单」「申请资金账单」 / **逐项相同** —— 三者本就是同一个「账单下载地址」协议，另设类型只会让同一份事实在两处漂移。
- **文件下载复用账单通道**：`download_url` 指向的文件仍须经 / `IWechatPayBillDownloadService` 下载（它带**主机白名单前置闸**，防 `download_url` 被替换）。 / 该服务随 `PayModule.Bill` 注册 —— 只用分账域而需下载文件的宿主，须一并启用账单域 / （或自行注册该端口）。**本端点只返回地址，不下载**。
- **时限**：官方原文「只能下载**三个月以内**的账单」；金额单位为**元**。

### 9. `DeleteReceiverAsync` — 删除分账接收方。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529590。

- **声明接口**：[IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs)
- **官方契约**：`POST /v3/profitsharing/receivers/delete`

- **请求体**：`ProfitSharingDeleteReceiverRequest` — 删除请求体，见 `ProfitSharingDeleteReceiverRequest`。
- **返回**：`ProfitSharingDeleteReceiverResponse` — 被删除的接收方身份（**仅** `type` / `account`），见 `ProfitSharingDeleteReceiverResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/profitsharing/receivers/delete`；无 path / query 参数。
- **⚠️ 参数必填性**：本接口 `appid` **必填**（与「请求分账」中它的选填语义**不同**， / 官方原样，勿统一）。
- **业务含义**：删除后不再支持把该商户结算后的资金分给该接收方；商户接收方上限 2 万， / 达上限须先删除未使用项。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/ProfitSharing`](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/) 内共 **12** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **12** 个，`SerializerClassName` = `ProfitSharing`。

AOT 源生成上下文：[`ProfitSharingJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/ProfitSharingJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ProfitSharingReceiver` | 应答 | [ProfitSharing/ProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingDtos.cs) | `appid`, `type`, `account`, `name`, `relation_type`, `custom_relation`, `transaction_id`, `out_order_no`, `receivers`, `unfreeze_unsplit`, `amount`, `description` … |
| `ProfitSharingAddReceiverRequest` | 请求体 | [ProfitSharing/ProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingDtos.cs) | `appid`, `type`, `account`, `name`, `relation_type`, `custom_relation`, `transaction_id`, `out_order_no`, `receivers`, `unfreeze_unsplit`, `amount`, `description` … |
| `ProfitSharingOrderResponse` | 应答 | [ProfitSharing/ProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingDtos.cs) | `appid`, `type`, `account`, `name`, `relation_type`, `custom_relation`, `transaction_id`, `out_order_no`, `receivers`, `unfreeze_unsplit`, `amount`, `description` … |
| `ProfitSharingOrderRequest` | 请求体 | [ProfitSharing/ProfitSharingDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingDtos.cs) | `appid`, `type`, `account`, `name`, `relation_type`, `custom_relation`, `transaction_id`, `out_order_no`, `receivers`, `unfreeze_unsplit`, `amount`, `description` … |
| `ProfitSharingReturnOrderResponse` | 应答 | [ProfitSharing/ProfitSharingReturnDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingReturnDtos.cs) | `order_id`, `out_order_no`, `out_return_no`, `return_mchid`, `amount`, `description`, `return_id`, `result`, `fail_reason`, `create_time`, `finish_time`, `transaction_id` … |
| `ProfitSharingReturnOrderRequest` | 请求体 | [ProfitSharing/ProfitSharingReturnDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingReturnDtos.cs) | `order_id`, `out_order_no`, `out_return_no`, `return_mchid`, `amount`, `description`, `return_id`, `result`, `fail_reason`, `create_time`, `finish_time`, `transaction_id` … |
| `ProfitSharingUnfreezeRequest` | 请求体 | [ProfitSharing/ProfitSharingReturnDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingReturnDtos.cs) | `order_id`, `out_order_no`, `out_return_no`, `return_mchid`, `amount`, `description`, `return_id`, `result`, `fail_reason`, `create_time`, `finish_time`, `transaction_id` … |
| `ProfitSharingAmountsResponse` | 应答 | [ProfitSharing/ProfitSharingReturnDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingReturnDtos.cs) | `order_id`, `out_order_no`, `out_return_no`, `return_mchid`, `amount`, `description`, `return_id`, `result`, `fail_reason`, `create_time`, `finish_time`, `transaction_id` … |
| `BillDownloadInfoResponse` | 应答 | [Bill/BillDownloadInfoResponse.cs](../../../Mud.Wechat.Pay.DataModels/Bill/BillDownloadInfoResponse.cs) | `hash_type`, `hash_value`, `download_url` |
| `ProfitSharingDeleteReceiverResponse` | 应答 | [ProfitSharing/ProfitSharingReceiverDeleteDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingReceiverDeleteDtos.cs) | `appid`, `type`, `account` |
| `ProfitSharingDeleteReceiverRequest` | 请求体 | [ProfitSharing/ProfitSharingReceiverDeleteDtos.cs](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/ProfitSharingReceiverDeleteDtos.cs) | `appid`, `type`, `account` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.ProfitSharing;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.ProfitSharing));

var service = app.Services.GetRequiredService<IWechatPayProfitSharingService>();
var ct = CancellationToken.None;
var response = await service.AddReceiverAsync(new ProfitSharingAddReceiverRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/profitsharing/receivers/add");
// 占位值仅用于编译示意；业务值须按 IWechatPayProfitSharingService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddReceiverAsync`）；请求体 `ProfitSharingAddReceiverRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayProfitSharingService](./IWechatPayProfitSharingService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayProfitSharingContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayProfitSharingContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayEcommerceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEcommerceContractGuards.cs)、[`WechatPaySmartGuideContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPaySmartGuideContractGuards.cs) |
| 守卫中的代表性测试方法 | `ProfitSharingEndpoints_ShouldMatchOfficialRoutes`、`ProfitSharingDtos_ShouldExposeOfficialFieldNames`、`ProfitSharingOrderResponse_ShouldDeriveFromWechatPayResponse`、`ProfitSharingEnumConstants_ShouldMatchOfficialValues`、`ProfitSharingDataModels_ShouldBeRegisteredInJsonContext`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`EcommerceTopology_ShouldBeEightInterfacesOnOneGroupWithFiftyFiveMethods`、`EcommerceMerchantIdentityFaces_ShouldStayFivePartnerEndpointsWithDualIdentityChannel` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/ProfitSharing`](../../../Mud.Wechat.Pay.DataModels/ProfitSharing/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayProfitSharingContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayEcommerceContractGuards.cs`、`WechatPaySmartGuideContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
