# EducationSchoolPay 域 · 教育学费缴纳（EducationSchoolPay）

微信支付「教育学费缴纳（EducationSchoolPay）」域 SDK（8 端点）。 接口：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)；注册组 `EducationSchoolPay`；模块枚举 `PayModule.EducationSchoolPay`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayEducationSchoolPayContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEducationSchoolPayContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/EducationSchoolPay`](../../../Mud.Wechat.Pay/EducationSchoolPay/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayEducationSchoolPayService = 8；按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayEducationSchoolPayService` | `CreateEducationSchoolPayContractPresignAsync` | POST | `/v3/eduschoolpay/contracts/presign` | 商户证书 / APIv3 签名 | 校园缴费预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012468952（【普通服务商】）。 |
| 2 | `IWechatPayEducationSchoolPayService` | `QueryEducationSchoolPayContractAsync` | GET | `/v3/eduschoolpay/contracts/{contractId}` | 商户证书 / APIv3 签名 | 查询校园缴费签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012468959（【普通服务商】）。 |
| 3 | `IWechatPayEducationSchoolPayService` | `TerminateEducationSchoolPayContractAsync` | POST | `/v3/eduschoolpay/contracts/{contractId}/terminate` | 商户证书 / APIv3 签名 | 解除校园缴费签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469025（【普通服务商】）。 |
| 4 | `IWechatPayEducationSchoolPayService` | `QueryEducationSchoolPayUserContractsAsync` | GET | `/v3/eduschoolpay/users/{openid}/contracts` | 商户证书 / APIv3 签名 | 按用户查询校园缴费签约列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012468993（【普通服务商】）。 |
| 5 | `IWechatPayEducationSchoolPayService` | `QueryEducationSchoolPayTransactionByIdAsync` | GET | `/v3/eduschoolpay/transactions/id/{transactionId}` | 商户证书 / APIv3 签名 | 校园缴费订单查询（微信订单号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469063（【普通服务商】）。 |
| 6 | `IWechatPayEducationSchoolPayService` | `CreateEducationSchoolPayTransactionAsync` | POST | `/v3/eduschoolpay/transactions` | 商户证书 / APIv3 签名 | 校园缴费下单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469030（【普通服务商】）。 |
| 7 | `IWechatPayEducationSchoolPayService` | `QueryEducationSchoolPayTransactionByOutTradeNoAsync` | GET | `/v3/eduschoolpay/transactions/out-trade-no/{outTradeNo}` | 商户证书 / APIv3 签名 | 校园缴费订单查询（商户订单号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469097（【普通服务商】）。 |
| 8 | `IWechatPayEducationSchoolPayService` | `QueryEducationSchoolPayUserDebtStateAsync` | GET | `/v3/eduschoolpay/users/{openid}/debt-state` | 商户证书 / APIv3 签名 | 查询用户欠费状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469161（【普通服务商】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 8 支中**8 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 8 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**无应答体支的返回形态**：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**涉密面是逐页核对结论**：本接口 8 支里**1 支**的官方请求头表把 `Wechatpay-Serial` 列为**必填**（敏感字段密文上送），其余支**未列**该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 `string` 承载、SDK **不代为加解密**。

**本域八支官方全部只印服务商页**（各页均标「支持商户：【普通服务商】」）⇒ 无商户面镜像、无双页归并问题， 一支一页一方法；**不得**按教育代扣域的双页形态补建商户面方法。

**预签约支的密文字段恰四枚**：`identity.real_name` / `identity.id_card_number` / `bank_card.bank_card_no` / `bank_card.phone` 官方逐枚标「按照APIV3标准加密该字段」且类型一律 `string(512)` ⇒ 以 `string` 承载、SDK **不代为加解密**；同支其余字段（`appid` / `openid` / `plan_id` / `school_id` 等）官方**无**加密要求，不得整体按密文处理。

**解除协议支 204 无包体** ⇒ 返回裸 `Task`；缴费下单（POST transactions）官方应答表与 两支查单（按 `transaction_id` / 按 `out_trade_no`）**逐字段同形且必填口径一致** ⇒ 三支共用同一响应类型， 这是本域唯一「写侧与读侧共用应答」的一对。

**下单请求里没有 `openid`**：扣款对象由 `contract_id` + `user_id` 定位（签约协议 + 学生账户）， 而**应答**侧的 `payer` 只有 `openid` / `sub_openid` ⇒ 请求与应答两侧的定位口径不同，不得按「下单也要传 openid」补字段。

**「受理 ≠ 扣款成功」**：共用应答里的 `trade_state` 可为中间态，官方错误码原文即 「商户订单号重复，请查询订单确定交易状态。确定交易失败后换订单号重试」⇒ 结果由查单支判定，**不得**据 HTTP 2xx 判定扣款成功。

**欠费状态支应答五键官方全必填**（`appid` / `openid` / `state` / `debt_count` / `update_time`）；其 `state` 是「用户欠款风控状态」（`NORMAL` / `FORBIDDEN`），与订单支的 `trade_state` **同名不同义 ⇒ 不得互套**。本线 DTO 惯例是属性一律可空承载、必填口径只由文档注释与守卫断言表达， 故「全必填」不等于「做成非空类型」。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）**支持商户**【普通服务商】。
- （出现 8 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。

### 1. `CreateEducationSchoolPayContractPresignAsync` — 校园缴费预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012468952（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`POST /v3/eduschoolpay/contracts/presign`

- **请求体**：`EducationSchoolPayContractPresignRequest` — 校园缴费预签约请求体，字段见 `EducationSchoolPayContractPresignRequest`。
- **返回**：`EducationSchoolPayContractPresignResponse` — 校园缴费预签约，见 `EducationSchoolPayContractPresignResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/eduschoolpay/contracts/presign`；无 path / query 参数。
- **必须带 `Wechatpay-Serial`**：官方请求头表把该头列为**必填**（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名**不**承载该头（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径，由传输层按商户配置注入）；密文以 `string` 承载，SDK **不代为加解密**。
- **应答**见 `EducationSchoolPayContractPresignResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 ALREADY_EXISTS：已存在使用该out_contract_no的合约 / 400 INVALID_REQUEST：不支持的商户 / 400 INVALID_REQUEST：商户号和公众号没有绑定关系 / 400 INVALID_REQUEST：用户信息有误，请检查OpenID / 400 INVALID_REQUEST：签约模板非法，请检查plan_id / 400 INVALID_REQUEST：签约模板ID或学校ID有误，请检查plan_id与school_id / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。

### 2. `QueryEducationSchoolPayContractAsync` — 查询校园缴费签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012468959（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`GET /v3/eduschoolpay/contracts/{contractId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成。（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
- **返回**：`EducationSchoolPayContractInfoResponse` — 查询校园缴费签约，见 `EducationSchoolPayContractInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/eduschoolpay/contracts/{contract_id}`；path `contract_id`。
- **业务错误码**：400 PARAM_ERROR：参数错误，请检查contract_id / 404 NOT_FOUND：未找到对应合约 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。

### 3. `TerminateEducationSchoolPayContractAsync` — 解除校园缴费签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469025（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`POST /v3/eduschoolpay/contracts/{contractId}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【签约协议号】 微信用户在校园轻松付下的合约的唯一标识，用户在校园轻松付小程序完成签约后，由微信支付生成。（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/eduschoolpay/contracts/{contract_id}/terminate`；path `contract_id`。
- **业务错误码**：400 PARAM_ERROR：参数错误，请检查contract_id / 404 NOT_FOUND：未找到对应合约 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。

### 4. `QueryEducationSchoolPayUserContractsAsync` — 按用户查询校园缴费签约列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012468993（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`GET /v3/eduschoolpay/users/{openid}/contracts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【商户AppID下的用户标识】 OpenID是由微信生成的，代表某个微信用户在AppID下的唯一标识。相同微信用户，AppID不同，则生成的OpenID不同。 请注意：调用该接口传入的OpenID应当与签约appid对应，如果传入的OpenID不合法，或与签约AppID不对应，则可能出现：1、查询结果为空或者报错；2、查询返回的OpenID与请求传入的OpenID不一致。（官方 path `openid`，**必填**，类型 `string(128)`）。 |
| `planId` | Query `plan_id` | `string` | 【签约模板号】 签约模板的唯一标识。商户接入校园轻松付并申请签约模板后由微信支付生成。（官方 query `plan_id`，**必填**，类型 `string(32)`）。 |
| `contractStatus` | Query `contract_status` | `string` | 【签约状态】 希望查询的签约协议的状态。枚举值：ADD-已签约; DELETE-已解约; ALL-同时查询已签约和已解约（官方 query `contract_status`，**必填**，类型 `string(32)`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 该次请求资源的起始位置，从0开始计数，请保持为分页大小 limit 的整数倍。调用方选填，默认为0。（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 该次请求可返回的最大资源条数。调用方选填，默认值为20，最小为1，最大为40。（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`EducationSchoolPayUserContractListResponse` — 按用户查询校园缴费签约列表，见 `EducationSchoolPayUserContractListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/eduschoolpay/users/{openid}/contracts`；path `openid`，query `plan_id` / `contract_status` / `offset` / `limit`。
- **`plan_id` 与 `contract_status` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 INVALID_REQUEST：用户信息有误，请检查OpenID / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。

### 5. `QueryEducationSchoolPayTransactionByIdAsync` — 校园缴费订单查询（微信订单号）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469063（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`GET /v3/eduschoolpay/transactions/id/{transactionId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 【微信支付订单号】 微信支付订单号（官方 path `transaction_id`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系。如果是服务商模式接入则可选；如果是直连商户模式接入则不用填写。（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`EducationSchoolPayTransactionQueryResponse` — 校园缴费订单查询（微信订单号），见 `EducationSchoolPayTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/eduschoolpay/transactions/id/{transaction_id}`；path `transaction_id`，query `sub_mchid`。
- **业务错误码**：400 INVALID_REQUEST：非法商户，请确认参数与订单创建商户一致 / 400 PARAM_ERROR：参数错误，请检查transaction_id / 404 NOT_FOUND：订单不存在 / 429 RATELIMIT_EXCEEDED：达到调用速率限制。

### 6. `CreateEducationSchoolPayTransactionAsync` — 校园缴费下单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469030（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`POST /v3/eduschoolpay/transactions`

- **请求体**：`EducationSchoolPayTransactionCreateRequest` — 校园缴费下单请求体，字段见 `EducationSchoolPayTransactionCreateRequest`。
- **返回**：`EducationSchoolPayTransactionQueryResponse` — 校园缴费下单，见 `EducationSchoolPayTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/eduschoolpay/transactions`；无 path / query 参数。
- **应答**见 `EducationSchoolPayTransactionQueryResponse`（官方应答表逐字段建模，与 `GET /eduschoolpay/transactions/id/{transaction_id}` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 ALREADY_EXISTS：out_trade_no冲突 / 400 INVALID_REQUEST：非法商户，服务商模式下请确认子商户正确 / 400 INVALID_REQUEST：商户号和公众号没有绑定关系 / 400 INVALID_REQUEST：商户号、子商户号以及子公众号没有绑定关系 / 400 INVALID_REQUEST：订单状态异常 / 400 INVALID_REQUEST：模板与学校不匹配 / 400 INVALID_REQUEST：学生账户与合约不匹配 / 400 INVALID_REQUEST：签约无效或已解除 / 400 INVALID_REQUEST：商户没有大额扣费场景权限 / 400 INVALID_REQUEST：单日扣款总金额超过限额 / 400 INVALID_REQUEST：商户订单号重复，请查询订单确定交易状态。确定交易失败后换订单号重试 / 400 INVALID_REQUEST：交易存在风险，不能发起扣款 / 403 NO_AUTH：用户禁用当前商户扣款权限 / 403 NOT_ENOUGH：用户欠款过多，暂停交易 / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：用户数据异常，请稍后重试 / 500 SYSTEM_ERROR：出现内部服务器错误。

### 7. `QueryEducationSchoolPayTransactionByOutTradeNoAsync` — 校园缴费订单查询（商户订单号）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469097（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`GET /v3/eduschoolpay/transactions/out-trade-no/{outTradeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outTradeNo` | Path | `string` | 【商户订单号】 商户在调用「扣款接口」时自行生成的订单号，要求64个字符内，只能是数字、大小写字母和 _-，且在同一个商户号下唯一。（官方 path `out_trade_no`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系。如果是服务商模式接入则可选；如果是直连商户模式接入则不用填写。（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`EducationSchoolPayTransactionQueryResponse` — 校园缴费订单查询（商户订单号），见 `EducationSchoolPayTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/eduschoolpay/transactions/out-trade-no/{out_trade_no}`；path `out_trade_no`，query `sub_mchid`。
- **业务错误码**：400 INVALID_REQUEST：非法商户，请确认参数与订单创建商户一致 / 400 PARAM_ERROR：参数错误，请检查out_trade_no / 404 NOT_FOUND：订单不存在 / 429 RATELIMIT_EXCEEDED：达到调用速率限制。

### 8. `QueryEducationSchoolPayUserDebtStateAsync` — 查询用户欠费状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012469161（【普通服务商】）。

- **声明接口**：[IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs)
- **官方契约**：`GET /v3/eduschoolpay/users/{openid}/debt-state`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【商户AppID下的用户标识】 OpenID是由微信生成的，代表某个微信用户在AppID下的唯一标识。相同微信用户，AppID不同，则生成的OpenID不同。请注意：调用该接口传入的OpenID应当与商户在校园轻松付登记的AppID对应。如果传入的OpenID不合法或者与登记的AppID不对应，则可能出现：1、查询报错；2、查询返回的OpenID与请求上传的OpenID不一致。（官方 path `openid`，**必填**，类型 `string(128)`）。 |
- **返回**：`EducationSchoolPayUserDebtStateResponse` — 查询用户欠费状态，见 `EducationSchoolPayUserDebtStateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/eduschoolpay/users/{openid}/debt-state`；path `openid`。
- **业务错误码**：400 INVALID_REQUEST：用户信息有误，请检查OpenID / 400 INVALID_REQUEST：商户未登记AppID / 429 RATELIMIT_EXCEEDED：达到调用速率限制 / 500 SYSTEM_ERROR：出现内部服务器错误。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/EducationSchoolPay`](../../../Mud.Wechat.Pay/EducationSchoolPay/) 内共 **18** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **18** 个，`SerializerClassName` = `EducationSchoolPay`。

AOT 源生成上下文：[`EducationSchoolPayJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/EducationSchoolPayJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `EducationSchoolPayContractPresignResponse` | 应答 | [EducationSchoolPay/EducationSchoolPayPresignDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayPresignDtos.cs) | `real_name`, `credential_type`, `id_card_number`, `bank_card_no`, `valid_thru`, `phone`, `bank_type`, `appid`, `openid`, `plan_id`, `user_id`, `school_id` … |
| `EducationSchoolPayContractPresignRequest` | 请求体 | [EducationSchoolPay/EducationSchoolPayPresignDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayPresignDtos.cs) | `real_name`, `credential_type`, `id_card_number`, `bank_card_no`, `valid_thru`, `phone`, `bank_type`, `appid`, `openid`, `plan_id`, `user_id`, `school_id` … |
| `EducationSchoolPayContractInfoResponse` | 应答 | [EducationSchoolPay/EducationSchoolPayContractDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayContractDtos.cs) | `contract_id`, `mchid`, `appid`, `openid`, `plan_id`, `contract_status`, `create_time`, `out_contract_code`, `next`, `prev`, `self`, `data` … |
| `EducationSchoolPayUserContractListResponse` | 应答 | [EducationSchoolPay/EducationSchoolPayContractDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayContractDtos.cs) | `contract_id`, `mchid`, `appid`, `openid`, `plan_id`, `contract_status`, `create_time`, `out_contract_code`, `next`, `prev`, `self`, `data` … |
| `EducationSchoolPayTransactionQueryResponse` | 应答 | [EducationSchoolPay/EducationSchoolPayTransactionDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayTransactionDtos.cs) | `openid`, `sub_openid`, `total`, `payer_total`, `discount_total`, `currency`, `device_id`, `device_ip`, `coupon_id`, `name`, `scope`, `type` … |
| `EducationSchoolPayTransactionCreateRequest` | 请求体 | [EducationSchoolPay/EducationSchoolPayTransactionDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayTransactionDtos.cs) | `openid`, `sub_openid`, `total`, `payer_total`, `discount_total`, `currency`, `device_id`, `device_ip`, `coupon_id`, `name`, `scope`, `type` … |
| `EducationSchoolPayUserDebtStateResponse` | 应答 | [EducationSchoolPay/EducationSchoolPayDebtStateDtos.cs](../../../Mud.Wechat.Pay/EducationSchoolPay/EducationSchoolPayDebtStateDtos.cs) | `appid`, `openid`, `state`, `debt_count`, `update_time` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.EducationSchoolPay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.EducationSchoolPay));

var service = app.Services.GetRequiredService<IWechatPayEducationSchoolPayService>();
var ct = CancellationToken.None;
var response = await service.CreateEducationSchoolPayContractPresignAsync(new EducationSchoolPayContractPresignRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/eduschoolpay/contracts/presign");
// 占位值仅用于编译示意；业务值须按 IWechatPayEducationSchoolPayService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateEducationSchoolPayContractPresignAsync`）；请求体 `EducationSchoolPayContractPresignRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayEducationSchoolPayService](./IWechatPayEducationSchoolPayService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayEducationSchoolPayContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEducationSchoolPayContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `EducationSchoolPayTopology_ShouldMatchInterfaceAndGroupRoster`、`EducationSchoolPayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`EducationSchoolPayDtos_ShouldExposeOfficialFieldRosters`、`EducationSchoolPayDataModels_ShouldSplitRequestAndResponseBaseTypes`、`EducationSchoolPayAllDtos_ShouldBeAnnotatedAndContextRegistered`、`EducationSchoolPayRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/EducationSchoolPay`](../../../Mud.Wechat.Pay/EducationSchoolPay/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayEducationSchoolPayContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
