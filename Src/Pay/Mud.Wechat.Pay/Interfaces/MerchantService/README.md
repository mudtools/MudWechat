# MerchantService 域 · 商户消费者投诉

微信支付「商户消费者投诉」域 SDK（**11 端点**：通知地址 CRUD + 投诉列表 / 详情 / 协商历史 + 回复 / 完成 / 退款审批 + 即时服务）。 接口：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)；注册组 `MerchantService`；模块枚举 `PayModule.MerchantService`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMerchantServiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantServiceContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/MerchantService`](../../../Mud.Wechat.Pay/MerchantService/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMerchantService = 11；按 HTTP 特性实计 **11** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantService` | `CreateComplaintNotificationAsync` | POST | `/v3/merchant-service/complaint-notifications` | 商户证书 / APIv3 签名 | 「创建投诉通知回调地址」：POST `/v3/merchant-service/complaint-notifications`。<br>官方文档：https://pay.weixin.… |
| 2 | `IWechatPayMerchantService` | `UpdateComplaintNotificationAsync` | PUT | `/v3/merchant-service/complaint-notifications` | 商户证书 / APIv3 签名 | 「更新投诉通知回调地址」：PUT `/v3/merchant-service/complaint-notifications`。<br>官方文档：https://pay.weixin.q… |
| 3 | `IWechatPayMerchantService` | `GetComplaintNotificationAsync` | GET | `/v3/merchant-service/complaint-notifications` | 商户证书 / APIv3 签名 | 「查询投诉通知回调地址」：GET `/v3/merchant-service/complaint-notifications`。<br>官方文档：https://pay.weixin.q… |
| 4 | `IWechatPayMerchantService` | `DeleteComplaintNotificationAsync` | DELETE | `/v3/merchant-service/complaint-notifications` | 商户证书 / APIv3 签名 | 「删除投诉通知回调地址」：DELETE `/v3/merchant-service/complaint-notifications`。<br>官方文档：https://pay.weixi… |
| 5 | `IWechatPayMerchantService` | `ListComplaintsAsync` | GET | `/v3/merchant-service/complaints-v2` | 商户证书 / APIv3 签名 | 「查询投诉单列表」：GET `/v3/merchant-service/complaints-v2`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/… |
| 6 | `IWechatPayMerchantService` | `GetComplaintDetailAsync` | GET | `/v3/merchant-service/complaints-v2/{complaintId}` | 商户证书 / APIv3 签名 | 「查询投诉单详情」：GET `/v3/merchant-service/complaints-v2/{complaint_id}`。<br>官方文档：https://pay.weixin… |
| 7 | `IWechatPayMerchantService` | `GetNegotiationHistoryAsync` | GET | `/v3/merchant-service/complaints-v2/{complaintId}/negotiation-historys` | 商户证书 / APIv3 签名 | 「查询投诉单协商历史」：GET `/v3/merchant-service/complaints-v2/{complaint_id}/negotiation-historys`。<br>… |
| 8 | `IWechatPayMerchantService` | `RespondToComplaintAsync` | POST | `/v3/merchant-service/complaints-v2/{complaintId}/response` | 商户证书 / APIv3 签名 | 「回复投诉单」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/response`。<br>官方文档：https://pa… |
| 9 | `IWechatPayMerchantService` | `CompleteComplaintAsync` | POST | `/v3/merchant-service/complaints-v2/{complaintId}/complete` | 商户证书 / APIv3 签名 | 「标记投诉单处理完成」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/complete`。<br>官方文档：https:… |
| 10 | `IWechatPayMerchantService` | `UpdateRefundProgressAsync` | POST | `/v3/merchant-service/complaints-v2/{complaintId}/update-refund-progress` | 商户证书 / APIv3 签名 | 「反馈退款审批结果」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/update-refund-progress`… |
| 11 | `IWechatPayMerchantService` | `RespondToImmediateServiceAsync` | POST | `/v3/merchant-service/complaints-v2/{complaintId}/response-immediate-service` | 商户证书 / APIv3 签名 | 「回复需要即时服务的投诉单」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/response-immediate-… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（2026-10-14 十三页逐字段核验）：通知地址创建
https://pay.weixin.qq.com/doc/v3/merchant/4012458679、更新
https://pay.weixin.qq.com/doc/v3/merchant/4012459282、查询
https://pay.weixin.qq.com/doc/v3/merchant/4012459014、删除
https://pay.weixin.qq.com/doc/v3/merchant/4012460452；
投诉列表 https://pay.weixin.qq.com/doc/v3/merchant/4012533431、详情
https://pay.weixin.qq.com/doc/v3/merchant/4012458106、协商历史
https://pay.weixin.qq.com/doc/v3/merchant/4012467250；
回复 https://pay.weixin.qq.com/doc/v3/merchant/4012467254、完成处理
https://pay.weixin.qq.com/doc/v3/merchant/4012467255、退款审批
https://pay.weixin.qq.com/doc/v3/merchant/4012467256、即时服务
https://pay.weixin.qq.com/doc/v3/merchant/4017151596。

**本域是支付线唯一同时含 POST/PUT/GET/DELETE 四动词的域**：通知地址四支共用同一路由
`/v3/merchant-service/complaint-notifications`，其中 POST/PUT 请求体相同（守卫 MC-B6），
GET/POST/PUT 应答相同（守卫 MC-B6），DELETE 为 204 无包体。

**写支返回形态分裂**：回复 / 完成 / 退款审批三支为 204 无包体 ⇒ 裸 `Task`；
即时服务支为 200 + JSON（回 `log_id`）⇒ 有返回类型。与门店域的 PATCH 204 同理，
不能把 204 当「成功即生效」——本域的状态闭环靠查询支读 `complaint_state`。

**图片通道独立**：上传与拉取两支走手工通道（非声明式），见 `IWechatPayComplaintMediaService`，
因为上传是 multipart、拉取是二进制流，无法用 `[Post]`/`[Get]` 表达。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；本域**零 `[Token]`**（守卫 PAY-B1）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateComplaintNotificationAsync` — 「创建投诉通知回调地址」：POST `/v3/merchant-service/complaint-notifications`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012458679。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`POST /v3/merchant-service/complaint-notifications`

- **请求体**：`MerchantServiceComplaintNotificationUrlRequest` — 请求体，见 `MerchantServiceComplaintNotificationUrlRequest`（仅 `url` 必填）。
- **返回**：`MerchantServiceComplaintNotificationResponse` — 应答体，见 `MerchantServiceComplaintNotificationResponse`（`mchid` + `url`）。

### 2. `UpdateComplaintNotificationAsync` — 「更新投诉通知回调地址」：PUT `/v3/merchant-service/complaint-notifications`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012459282。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`PUT /v3/merchant-service/complaint-notifications`

- **请求体**：`MerchantServiceComplaintNotificationUrlRequest` — 请求体，与创建支**同一类型**，见 `MerchantServiceComplaintNotificationUrlRequest`。
- **返回**：`MerchantServiceComplaintNotificationResponse` — 应答体，与创建/查询支**同一类型**，见 `MerchantServiceComplaintNotificationResponse`。

**备注（原文）**

- **请求体与创建支完全一致**（守卫 MC-B6）：仅 `url string(255)` 必填。

### 3. `GetComplaintNotificationAsync` — 「查询投诉通知回调地址」：GET `/v3/merchant-service/complaint-notifications`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012459014。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`GET /v3/merchant-service/complaint-notifications`
- **返回**：`MerchantServiceComplaintNotificationResponse` — 应答体，与创建/更新支**同一类型**，见 `MerchantServiceComplaintNotificationResponse`。

### 4. `DeleteComplaintNotificationAsync` — 「删除投诉通知回调地址」：DELETE `/v3/merchant-service/complaint-notifications`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012460452。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`DELETE /v3/merchant-service/complaint-notifications`
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

### 5. `ListComplaintsAsync` — 「查询投诉单列表」：GET `/v3/merchant-service/complaints-v2`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012533431。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`GET /v3/merchant-service/complaints-v2`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `limit` | Query `limit` | `long?` | 分页大小（query `limit`，选填 integer）：范围 [1,50]。 |
| `offset` | Query `offset` | `long?` | 分页开始位置（query `offset`，选填 integer）：从 0 计数。 |
| `beginDate` | Query `begin_date` | `string?` | 开始日期（query `begin_date`，**必填** string(10)）：格式 yyyy-MM-DD，跨度 ≤30 天。 |
| `endDate` | Query `end_date` | `string?` | 结束日期（query `end_date`，**必填** string(10)）：格式 yyyy-MM-DD。 |
| `complaintedMchId` | Query `complainted_mchid` | `string?` | 被诉商户号（query `complainted_mchid`，选填 string(64)）：服务商查指定子商户时传。 |
- **返回**：`MerchantServiceComplaintListResponse` — 列表应答，见 `MerchantServiceComplaintListResponse`。

**备注（原文）**

- **官方契约**：**GET**，query 参数。`total_count` 仅 `offset=0` 时返回。
- **本域 `payer_openid` 不在列表里**：只有详情支返回 ⇒ 需要 openid 做客服触达时须逐单调详情（守卫 MC-B6）。

### 6. `GetComplaintDetailAsync` — 「查询投诉单详情」：GET `/v3/merchant-service/complaints-v2/{complaint_id}`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012458106。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`GET /v3/merchant-service/complaints-v2/{complaintId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintId` | Path | `string` | 投诉单号（path `complaint_id`，必填 string(64)）。 |
- **返回**：`MerchantServiceComplaintDetail` — 详情应答，见 `MerchantServiceComplaintDetail`（比列表多 `payer_openid` 等键）。

### 7. `GetNegotiationHistoryAsync` — 「查询投诉单协商历史」：GET `/v3/merchant-service/complaints-v2/{complaint_id}/negotiation-historys`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012467250。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`GET /v3/merchant-service/complaints-v2/{complaintId}/negotiation-historys`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintId` | Path | `string` | 投诉单号（path `complaint_id`，必填 string(64)）。 |
| `limit` | Query `limit` | `long?` | 分页大小（query `limit`，选填 integer）：范围 [1,300]（上限高于列表支）。 |
| `offset` | Query `offset` | `long?` | 分页开始位置（query `offset`，选填 integer）：从 0 计数。 |
- **返回**：`MerchantServiceNegotiationHistoryResponse` — 协商历史应答，见 `MerchantServiceNegotiationHistoryResponse`。

**备注（原文）**

- **本域 `limit` 上界不同**：列表支 50、协商历史支 300（守卫 MC-B1 锁定该差异）。

### 8. `RespondToComplaintAsync` — 「回复投诉单」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/response`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012467254。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`POST /v3/merchant-service/complaints-v2/{complaintId}/response`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintId` | Path | `string` | 投诉单号（path `complaint_id`，必填 string(64)）。 |

- **请求体**：`MerchantServiceComplaintResponseRequest` — 请求体，见 `MerchantServiceComplaintResponseRequest`（含文字链 / 小程序跳转信息）。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **首次回复即改变状态**：待处理 → 处理中（官方原文），但受理 ≠ 可用，仍须经查询支确认。

### 9. `CompleteComplaintAsync` — 「标记投诉单处理完成」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/complete`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012467255。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`POST /v3/merchant-service/complaints-v2/{complaintId}/complete`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintId` | Path | `string` | 投诉单号（path `complaint_id`，必填 string(64)）。 |

- **请求体**：`MerchantServiceComplaintCompleteRequest` — 请求体，见 `MerchantServiceComplaintCompleteRequest`（仅 `complainted_mchid` 必填）。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **调用后即对用户下发满意度评价**（官方原文），无法撤回；重复投诉率是考核指标之一。

### 10. `UpdateRefundProgressAsync` — 「反馈退款审批结果」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/update-refund-progress`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012467256。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`POST /v3/merchant-service/complaints-v2/{complaintId}/update-refund-progress`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintId` | Path | `string` | 投诉单号（path `complaint_id`，必填 string(64)）。 |

- **请求体**：`MerchantServiceRefundProgressUpdateRequest` — 请求体，见 `MerchantServiceRefundProgressUpdateRequest`（**无** `complainted_mchid`）。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **同意不落退款**：`APPROVE` 后必须额外调退款接口发起原路退款，到账后状态才自动扭转为处理完成； / `REJECT` 则当场扭转。
- **本支请求体没有 `complainted_mchid`**：与同族其他写支不同（守卫 MC-B3 锁定该差异）。

### 11. `RespondToImmediateServiceAsync` — 「回复需要即时服务的投诉单」：POST `/v3/merchant-service/complaints-v2/{complaint_id}/response-immediate-service`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4017151596。

- **声明接口**：[IWechatPayMerchantService](./IWechatPayMerchantService.cs)
- **官方契约**：`POST /v3/merchant-service/complaints-v2/{complaintId}/response-immediate-service`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `complaintId` | Path | `string` | 投诉单号（path `complaint_id`，必填 string(64)）。 |

- **请求体**：`MerchantServiceImmediateServiceRequest` — 请求体，见 `MerchantServiceImmediateServiceRequest`（新版消息格式 + 幂等 ID 必填）。
- **返回**：`MerchantServiceImmediateServiceResponse` — 应答体，见 `MerchantServiceImmediateServiceResponse`（回 `log_id`）。

**备注（原文）**

- **官方适用对象**：列表返回的 `need_immediate_service=true` 的投诉单（2026 年新页）。
- **`idempotent_id` 必填**：官方 string(128)，「相同 idempotent_id 的请求只会被处理一次」，留空即 `PARAM_ERROR`。
- **本域唯一返回包体的写支**：其余三支均为 204，只有这一支能拿到操作流水号，用于与协商历史条目对齐。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/MerchantService`](../../../Mud.Wechat.Pay/MerchantService/) 内共 **34** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **34** 个，`SerializerClassName` = `MerchantService`。

AOT 源生成上下文：[`MerchantServiceJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/MerchantServiceJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantServiceComplaintNotificationResponse` | 应答 | [MerchantService/MerchantServiceNotificationDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceNotificationDtos.cs) | `url`, `mchid` |
| `MerchantServiceComplaintNotificationUrlRequest` | 请求体 | [MerchantService/MerchantServiceNotificationDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceNotificationDtos.cs) | `url`, `mchid` |
| `MerchantServiceComplaintListResponse` | 应答 | [MerchantService/MerchantServiceComplaintQueryDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceComplaintQueryDtos.cs) | `data`, `limit`, `offset`, `total_count`, `complaint_id`, `complaint_time`, `complaint_detail`, `complaint_state`, `complainted_mchid`, `payer_phone`, `complaint_order_info`, `complaint_full_refunded` … |
| `MerchantServiceComplaintDetail` | 应答 | [MerchantService/MerchantServiceComplaintQueryDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceComplaintQueryDtos.cs) | `data`, `limit`, `offset`, `total_count`, `complaint_id`, `complaint_time`, `complaint_detail`, `complaint_state`, `complainted_mchid`, `payer_phone`, `complaint_order_info`, `complaint_full_refunded` … |
| `MerchantServiceNegotiationHistoryResponse` | 应答 | [MerchantService/MerchantServiceComplaintQueryDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceComplaintQueryDtos.cs) | `data`, `limit`, `offset`, `total_count`, `complaint_id`, `complaint_time`, `complaint_detail`, `complaint_state`, `complainted_mchid`, `payer_phone`, `complaint_order_info`, `complaint_full_refunded` … |
| `MerchantServiceComplaintResponseRequest` | 请求体 | [MerchantService/MerchantServiceProcessDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceProcessDtos.cs) | `complainted_mchid`, `response_content`, `response_images`, `jump_url`, `jump_url_text`, `mini_program_jump_info`, `appid`, `path`, `text`, `action`, `launch_refund_day`, `reject_reason` … |
| `MerchantServiceComplaintCompleteRequest` | 请求体 | [MerchantService/MerchantServiceProcessDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceProcessDtos.cs) | `complainted_mchid`, `response_content`, `response_images`, `jump_url`, `jump_url_text`, `mini_program_jump_info`, `appid`, `path`, `text`, `action`, `launch_refund_day`, `reject_reason` … |
| `MerchantServiceRefundProgressUpdateRequest` | 请求体 | [MerchantService/MerchantServiceProcessDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceProcessDtos.cs) | `complainted_mchid`, `response_content`, `response_images`, `jump_url`, `jump_url_text`, `mini_program_jump_info`, `appid`, `path`, `text`, `action`, `launch_refund_day`, `reject_reason` … |
| `MerchantServiceImmediateServiceResponse` | 应答 | [MerchantService/MerchantServiceProcessDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceProcessDtos.cs) | `complainted_mchid`, `response_content`, `response_images`, `jump_url`, `jump_url_text`, `mini_program_jump_info`, `appid`, `path`, `text`, `action`, `launch_refund_day`, `reject_reason` … |
| `MerchantServiceImmediateServiceRequest` | 请求体 | [MerchantService/MerchantServiceProcessDtos.cs](../../../Mud.Wechat.Pay/MerchantService/MerchantServiceProcessDtos.cs) | `complainted_mchid`, `response_content`, `response_images`, `jump_url`, `jump_url_text`, `mini_program_jump_info`, `appid`, `path`, `text`, `action`, `launch_refund_day`, `reject_reason` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantService));

var service = app.Services.GetRequiredService<IWechatPayMerchantService>();
var ct = CancellationToken.None;
await service.DeleteComplaintNotificationAsync(ct);
// 裸 Task：成功与否按 HTTP 状态码与业务错误码判定（WechatPayException）。
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`DeleteComplaintNotificationAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 部分端点无官方文档备注 | 有端点的方法 XML 注释没有 `<remarks>` 段 ⇒ 该端点的官方页 / 限制未在本仓留档 | 逐页补注释；本文不对其作额外断言 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantService](./IWechatPayMerchantService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMerchantServiceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantServiceContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `MerchantServiceEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentForFourWrites`、`MerchantServiceRoutes_ShouldStayOnOwnPrefixAndNotDeclareCursorOrDeleteFaces`、`MerchantServiceDtos_ShouldExposeOfficialFieldNamesAndSubtreeDifference`、`MerchantServiceConstants_ShouldMatchOfficialValueSets`、`MerchantServiceUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted`、`MerchantServiceResponses_ShouldDeriveFromBaseAndFollowSharedSubtreeRulings`、`MerchantServiceDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/MerchantService`](../../../Mud.Wechat.Pay/MerchantService/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMerchantServiceContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
