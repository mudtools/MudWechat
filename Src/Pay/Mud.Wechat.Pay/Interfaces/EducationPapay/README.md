# EducationPapay 域 · 教育委托代扣（EducationPAPay）

微信支付「教育委托代扣（EducationPAPay）」域 SDK（8 端点）。 接口：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)；注册组 `EducationPapay`；模块枚举 `PayModule.EducationPapay`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayEducationPapayContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEducationPapayContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/EducationPapay`](../../DataModels/EducationPapay)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayEducationPapayService = 8；按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayEducationPapayService` | `TerminateEducationPapayContractAsync` | DELETE | `/v3/edu-papay/contracts/{contractId}` | 商户证书 / APIv3 签名 | 解除教育代扣签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466882（【普通商户】）。 |
| 2 | `IWechatPayEducationPapayService` | `QueryEducationPapayContractByIdAsync` | GET | `/v3/edu-papay/contracts/id/{contractId}` | 商户证书 / APIv3 签名 | 按微信协议号查询教育代扣签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012522951（【普通商户】）、https://… |
| 3 | `IWechatPayEducationPapayService` | `QueryEducationPapayUserContractsAsync` | GET | `/v3/edu-papay/user/{openid}/contracts` | 商户证书 / APIv3 签名 | 按用户查询教育代扣签约列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012523265（【普通商户】）、https://p… |
| 4 | `IWechatPayEducationPapayService` | `CreateEducationPapayContractPresignAsync` | POST | `/v3/edu-papay/contracts/presign` | 商户证书 / APIv3 签名 | 教育代扣预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012522560（【普通商户】）、https://pay.wei… |
| 5 | `IWechatPayEducationPapayService` | `CreateEducationPapayTransactionAsync` | POST | `/v3/edu-papay/transactions` | 商户证书 / APIv3 签名 | 教育代扣下单（受理）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524731（【普通商户】）、https://pay.… |
| 6 | `IWechatPayEducationPapayService` | `QueryEducationPapayTransactionByIdAsync` | GET | `/v3/edu-papay/transactions/id/{transactionId}` | 商户证书 / APIv3 签名 | 教育代扣订单查询（微信订单号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012523468（【普通商户】）、https:/… |
| 7 | `IWechatPayEducationPapayService` | `QueryEducationPapayTransactionByOutTradeNoAsync` | GET | `/v3/edu-papay/transactions/out-trade-no/{outTradeNo}` | 商户证书 / APIv3 签名 | 教育代扣订单查询（商户订单号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012523591（【普通商户】）、https:/… |
| 8 | `IWechatPayEducationPapayService` | `SendEducationPapayUserNotificationAsync` | POST | `/v3/edu-papay/user-notifications/{contractId}/send` | 商户证书 / APIv3 签名 | 发送扣费前用户通知。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524814（【普通商户】）、https://pay.w… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 8 支中**1 支官方只印一页**、**7 支印多页**（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 8 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**无应答体支的返回形态**：本域 3 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**七支印双页、一支只印商户页**：本域 `/edu-papay/*` 八支里七支同时印【普通商户】与【普通服务商】两页 ⇒ 按**并集**建模、一面一方法，身份差异（服务商页多 `sub_mchid` / `sub_appid`）写进参数注释而不拆方法； 但 `DELETE /v3/edu-papay/contracts/{contract_id}`（解除协议）**官方只印商户页** ⇒ 该支无服务商镜像， **不得**按「同域其余支都有双页」补齐，也不得把服务商页文档链接错挂到本支（处置表原引的服务商 docId 实为 `GET /v3/edu-papay/user/{openid}/contracts` 的服务商页，已逐对核正）。

**三支官方成功应答是 204 无包体**：解除协议（DELETE）、下单受理（POST transactions）、 发送预扣费通知（POST user-notifications）⇒ 一律返回裸 `Task`，不建响应类型； 其中「下单」是**受理**不是扣款成功，扣款结果只由异步通知与查单支判定。

**两支查单应答逐字段同形 ⇒ 共用一型**：`/transactions/id/{transaction_id}` 与 `/transactions/out-trade-no/{out_trade_no}` 官方应答表 33 行同形且必填口径一致 ⇒ 共用一个响应类型。 但「按 `contract_id` 查协议」与「按 `openid` 查协议列表」两支**各建一应答**： 列表支的 `data[]` 元素与单查应答字段同形，本线口径是**共用只发生在应答根类型**、 子树按「端点 + 路径段」机械拼装独立成类型 ⇒ 不得为「看起来一样」把单查应答当成列表元素类型。

**预签约支只回 `presign_token`**：官方两页应答表单行且**必填** ⇒ 该 token 是调起签约小程序/公众号页面的入参， 不等于签约成功；签约结果须回查协议支。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- （出现 8 次）**业务错误码**：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户无权限 / 403 NOT_ENOUGH：余额不足 / 429 RATELIMIT_EXCEEDED：频率限制 / 400 INVALID_REQUEST：请求的业务参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误 / 400 ALREADY_EXISTS：资源已存在 / 404 NOT_FOUND：请求的资源不存在。
- （出现 7 次）**支持商户**【普通商户】【普通服务商】。
- （出现 7 次）**双页并集**：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按**并集**建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。

### 1. `TerminateEducationPapayContractAsync` — 解除教育代扣签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466882（【普通商户】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`DELETE /v3/edu-papay/contracts/{contractId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**DELETE** `/v3/edu-papay/contracts/{contract_id}`；path `contract_id`。
- **支持商户**【普通商户】。

### 2. `QueryEducationPapayContractByIdAsync` — 按微信协议号查询教育代扣签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012522951（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012465367（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`GET /v3/edu-papay/contracts/id/{contractId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
| `appid` | Query `appid` | `string` | 【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方 query `appid`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
| `subAppid` | Query `sub_appid` | `string?` | 【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方 query `sub_appid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
- **返回**：`EducationPapayContractInfoResponse` — 按微信协议号查询教育代扣签约，见 `EducationPapayContractInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/edu-papay/contracts/id/{contract_id}`；path `contract_id`，query `appid` / `sub_mchid` / `sub_appid`。
- **`appid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 3. `QueryEducationPapayUserContractsAsync` — 按用户查询教育代扣签约列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012523265（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012489548（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`GET /v3/edu-papay/user/{openid}/contracts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【服务商APPID下的用户标识】 用户在服务商appid下的唯一标识（官方 path `openid`，**必填**，类型 `string(128)`）。 |
| `appid` | Query `appid` | `string` | 【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方 query `appid`，**必填**，类型 `string(32)`）。 |
| `planId` | Query `plan_id` | `string` | 【签约模版号】 签约模版号，签约模版唯一标识（官方 query `plan_id`，**必填**，类型 `string(32)`）。 |
| `contractStatus` | Query `contract_status` | `string?` | 【签约状态】 ADD-签约（默认）; DELETE-解约（官方 query `contract_status`，选填，类型 `string(32)`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 该次请求资源的起始位置，从0开始计数。调用方选填，默认为0（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 该次请求可返回的最大资源条数。调用方选填，默认值为20（官方 query `limit`，选填，类型 `integer`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
| `subAppid` | Query `sub_appid` | `string?` | 【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方 query `sub_appid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
| `subOpenid` | Query `sub_openid` | `string?` | 【子商户APPID下的用户标识】 用户在子商户appid下的唯一标识（官方 query `sub_openid`，选填，类型 `string(128)`，**仅服务商页**印发）。 |
- **返回**：`EducationPapayUserContractListResponse` — 按用户查询教育代扣签约列表，见 `EducationPapayUserContractListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/edu-papay/user/{openid}/contracts`；path `openid`，query `appid` / `plan_id` / `contract_status` / `offset` / `limit` / `sub_mchid` / `sub_appid` / `sub_openid`。
- **`appid` 与 `plan_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 4. `CreateEducationPapayContractPresignAsync` — 教育代扣预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012522560（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012465360（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`POST /v3/edu-papay/contracts/presign`

- **请求体**：`EducationPapayContractPresignRequest` — 教育代扣预签约请求体，字段见 `EducationPapayContractPresignRequest`。
- **返回**：`EducationPapayContractPresignResponse` — 教育代扣预签约，见 `EducationPapayContractPresignResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/edu-papay/contracts/presign`；无 path / query 参数。
- **应答**见 `EducationPapayContractPresignResponse`（官方应答表逐字段建模）。

### 5. `CreateEducationPapayTransactionAsync` — 教育代扣下单（受理）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524731（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012465548（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`POST /v3/edu-papay/transactions`

- **请求体**：`EducationPapayTransactionCreateRequest` — 教育代扣下单（受理）请求体，字段见 `EducationPapayTransactionCreateRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/edu-papay/transactions`；无 path / query 参数。

### 6. `QueryEducationPapayTransactionByIdAsync` — 教育代扣订单查询（微信订单号）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012523468（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012465567（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`GET /v3/edu-papay/transactions/id/{transactionId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 【微信支付订单号】 微信支付订单号（官方 path `transaction_id`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 子商户号（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
- **返回**：`EducationPapayTransactionQueryResponse` — 教育代扣订单查询（微信订单号），见 `EducationPapayTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/edu-papay/transactions/id/{transaction_id}`；path `transaction_id`，query `sub_mchid`。

### 7. `QueryEducationPapayTransactionByOutTradeNoAsync` — 教育代扣订单查询（商户订单号）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012523591（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012465574（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`GET /v3/edu-papay/transactions/out-trade-no/{outTradeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outTradeNo` | Path | `string` | 【商户订单号】 商户订单号（官方 path `out_trade_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
- **返回**：`EducationPapayTransactionQueryResponse` — 教育代扣订单查询（商户订单号），见 `EducationPapayTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/edu-papay/transactions/out-trade-no/{out_trade_no}`；path `out_trade_no`，query `sub_mchid`。

### 8. `SendEducationPapayUserNotificationAsync` — 发送扣费前用户通知。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524814（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012855408（【普通服务商】）。

- **声明接口**：[IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs)
- **官方契约**：`POST /v3/edu-papay/user-notifications/{contractId}/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【签约协议号】 标识用户与某签约模版的一次签约，具有唯一性（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`EducationPapayUserNotificationSendRequest` — 发送扣费前用户通知请求体，字段见 `EducationPapayUserNotificationSendRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/edu-papay/user-notifications/{contract_id}/send`；path `contract_id`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/EducationPapay`](../../DataModels/EducationPapay) 内共 **16** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **16** 个，`SerializerClassName` = `EducationPapay`。

AOT 源生成上下文：[`EducationPapayJsonContext.g.cs`](./../../DataModels/Generated/EducationPapayJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `EducationPapayContractInfoResponse` | 应答 | [EducationPapay/EducationPapayContractDtos.cs](../../DataModels/EducationPapay/EducationPapayContractDtos.cs) | `contract_id`, `contract_status`, `create_time`, `sp_mchid`, `appid`, `openid`, `plan_id`, `contract_information`, `sub_mchid`, `sub_appid`, `sub_openid`, `data` … |
| `EducationPapayUserContractListResponse` | 应答 | [EducationPapay/EducationPapayContractDtos.cs](../../DataModels/EducationPapay/EducationPapayContractDtos.cs) | `contract_id`, `contract_status`, `create_time`, `sp_mchid`, `appid`, `openid`, `plan_id`, `contract_information`, `sub_mchid`, `sub_appid`, `sub_openid`, `data` … |
| `EducationPapayContractPresignResponse` | 应答 | [EducationPapay/EducationPapayPresignDtos.cs](../../DataModels/EducationPapay/EducationPapayPresignDtos.cs) | `appid`, `openid`, `plan_id`, `user_id`, `period_start_date`, `trade_scene`, `sub_mchid`, `sub_appid`, `sub_openid`, `presign_token` |
| `EducationPapayContractPresignRequest` | 请求体 | [EducationPapay/EducationPapayPresignDtos.cs](../../DataModels/EducationPapay/EducationPapayPresignDtos.cs) | `appid`, `openid`, `plan_id`, `user_id`, `period_start_date`, `trade_scene`, `sub_mchid`, `sub_appid`, `sub_openid`, `presign_token` |
| `EducationPapayTransactionCreateRequest` | 请求体 | [EducationPapay/EducationPapayTransactionDtos.cs](../../DataModels/EducationPapay/EducationPapayTransactionDtos.cs) | `total`, `currency`, `device_id`, `device_ip`, `appid`, `body`, `attach`, `out_trade_no`, `goods_tag`, `notify_url`, `contract_id`, `trade_scene` … |
| `EducationPapayTransactionQueryResponse` | 应答 | [EducationPapay/EducationPapayTransactionDtos.cs](../../DataModels/EducationPapay/EducationPapayTransactionDtos.cs) | `total`, `currency`, `device_id`, `device_ip`, `appid`, `body`, `attach`, `out_trade_no`, `goods_tag`, `notify_url`, `contract_id`, `trade_scene` … |
| `EducationPapayUserNotificationSendRequest` | 请求体 | [EducationPapay/EducationPapayNotificationDtos.cs](../../DataModels/EducationPapay/EducationPapayNotificationDtos.cs) | `appid`, `sub_mchid`, `sub_appid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.EducationPapay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.EducationPapay));

var service = app.Services.GetRequiredService<IWechatPayEducationPapayService>();
var ct = CancellationToken.None;
var response = await service.CreateEducationPapayContractPresignAsync(new EducationPapayContractPresignRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/edu-papay/contracts/presign");
// 占位值仅用于编译示意；业务值须按 IWechatPayEducationPapayService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateEducationPapayContractPresignAsync`）；请求体 `EducationPapayContractPresignRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayEducationPapayService](./IWechatPayEducationPapayService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayEducationPapayContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEducationPapayContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `EducationPapayTopology_ShouldMatchInterfaceAndGroupRoster`、`EducationPapayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`EducationPapayDtos_ShouldExposeOfficialFieldRosters`、`EducationPapayDataModels_ShouldSplitRequestAndResponseBaseTypes`、`EducationPapayAllDtos_ShouldBeAnnotatedAndContextRegistered`、`EducationPapayRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/EducationPapay`](../../DataModels/EducationPapay) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayEducationPapayContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
