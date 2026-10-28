# Papay 域 · 委托代扣（PAPay 3.0 直连商户面）

微信支付「委托代扣（PAPay 3.0 直连商户面）」域 SDK（20 端点）。 接口：[IWechatPayPapayService](./IWechatPayPapayService.cs)；注册组 `Papay`；模块枚举 `PayModule.Papay`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayEducationPapayContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEducationPapayContractGuards.cs)、[WechatPayPapayContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPapayContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/Papay`](../../../Mud.Wechat.Pay/Papay/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPapayService = 20；按 HTTP 特性实计 **20** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPapayService` | `SendPapayDeductNotifyAsync` | POST | `/v3/papay/contracts/{contractId}/notify` | 商户证书 / APIv3 签名 | 预扣费通知。<br>官方文档：https://pay.weixin.qq.com/doc/v2/merchant/4011987402（【直连商户】）。 |
| 2 | `IWechatPayPapayService` | `QueryPapayContractAsync` | GET | `/v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}` | 商户证书 / APIv3 签名 | 代扣签约协议查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489245（【普通商户】）。 |
| 3 | `IWechatPayPapayService` | `TerminatePapayContractAsync` | POST | `/v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate` | 商户证书 / APIv3 签名 | 代扣签约协议解除。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489295（【普通商户】）。 |
| 4 | `IWechatPayPapayService` | `QueryPapayDeductScheduleAsync` | GET | `/v3/papay/pay/schedules/contract-id/{contractId}` | 商户证书 / APIv3 签名 | 预约扣费结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466997（【普通商户】）。 |
| 5 | `IWechatPayPapayService` | `CreatePapayDeductScheduleAsync` | POST | `/v3/papay/pay/schedules/contract-id/{contractId}/schedule` | 商户证书 / APIv3 签名 | 预约扣费受理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012467036（【普通商户】）。 |
| 6 | `IWechatPayPapayService` | `ApplyPapayHarmonyDeductAsync` | POST | `/v3/papay/pay/harmony/transactions/apply` | 商户证书 / APIv3 签名 | 代扣受理扣款。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4013388429（【普通商户】）。 |
| 7 | `IWechatPayPapayService` | `CreatePapayScheduledPreEntrustSignAppAsync` | POST | `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/app` | 商户证书 / APIv3 签名 | 预约扣费APP预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524934（【普通商户】）。 |
| 8 | `IWechatPayPapayService` | `CreatePapayScheduledPreEntrustSignH5Async` | POST | `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5` | 商户证书 / APIv3 签名 | 预约扣费H5预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489208（【普通商户】）。 |
| 9 | `IWechatPayPapayService` | `CreatePapayScheduledPreEntrustSignJsapiAsync` | POST | `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/jsapi` | 商户证书 / APIv3 签名 | 预约扣费JSAPI预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012525133（【普通商户】）。 |
| 10 | `IWechatPayPapayService` | `CreatePapayScheduledPreEntrustSignMiniProgramAsync` | POST | `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/mini-program` | 商户证书 / APIv3 签名 | 预约扣费小程序预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012525209（【普通商户】）。 |
| 11 | `IWechatPayPapayService` | `CreatePapayInsurancePreEntrustSignH5Async` | POST | `/v3/papay/insurance-sign/contracts/pre-entrust-sign/h5` | 商户证书 / APIv3 签名 | 保险自动续费H5预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529050（【普通商户】）。 |
| 12 | `IWechatPayPapayService` | `CreatePapayInsurancePreEntrustSignJsapiAsync` | POST | `/v3/papay/insurance-sign/contracts/pre-entrust-sign/jsapi` | 商户证书 / APIv3 签名 | 保险自动续费JSAPI预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012528600（【普通商户】）。 |
| 13 | `IWechatPayPapayService` | `CreatePapayInsurancePreEntrustSignMiniProgramAsync` | POST | `/v3/papay/insurance-sign/contracts/pre-entrust-sign/mini-program` | 商户证书 / APIv3 签名 | 保险自动续费小程序预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012528950（【普通商户】）。 |
| 14 | `IWechatPayPapayService` | `QueryPapayInsuranceContractAsync` | GET | `/v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}` | 商户证书 / APIv3 签名 | 保险自动续费协议。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012472641（【普通商户】）。 |
| 15 | `IWechatPayPapayService` | `TerminatePapayInsuranceContractAsync` | POST | `/v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate` | 商户证书 / APIv3 签名 | 保险自动续费协议解除。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012472767（【普通商户】）。 |
| 16 | `IWechatPayPapayService` | `RenewPapayInsuranceContractAsync` | POST | `/v3/papay/insurance-sign/contracts/plan-id/{planId}/contract-id/{contractId}/renew` | 商户证书 / APIv3 签名 | 续期自动续费协议受理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489588（【普通商户】）。 |
| 17 | `IWechatPayPapayService` | `QueryPapayInsurancePolicyPeriodAsync` | GET | `/v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}` | 商户证书 / APIv3 签名 | 保险自动续费预约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012471229（【普通商户】）。 |
| 18 | `IWechatPayPapayService` | `SchedulePapayInsurancePolicyPeriodAsync` | POST | `/v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}/schedule` | 商户证书 / APIv3 签名 | 保险自动续费预约受理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012471270（【普通商户】）。 |
| 19 | `IWechatPayPapayService` | `QueryPapayInsurancePolicyPeriodsAsync` | GET | `/v3/papay/insurance-sign/policy_periods/plan-id/{planId}/out-contract-code/{outContractCode}` | 商户证书 / APIv3 签名 | 保险扣费周期列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489530（【普通商户】）。 |
| 20 | `IWechatPayPapayService` | `ApplyPapayInsuranceDeductAsync` | POST | `/v3/papay/insurance-pay/transactions/apply` | 商户证书 / APIv3 签名 | 保险自动续费扣款受理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012471158（【普通商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 20 支中**20 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 20 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**无应答体支的返回形态**：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**涉密面是逐页核对结论**：本接口 20 支里**3 支**的官方请求头表把 `Wechatpay-Serial` 列为**必填**（敏感字段密文上送），其余支**未列**该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 `string` 承载、SDK **不代为加解密**。

**预扣费通知是扣款的前置闸**：`POST /v3/papay/contracts/{contractId}/notify` 成功后才进入扣费等待期 （官方原文「具有委托代扣扣费权限，且**另行开通了预扣费通知权限**的直连商户」）；正常应答为 **204 无内容** ⇒ 本方法返回裸 `Task`，而业务码 `RESOURCE_ALREADY_EXISTS`（已成功发送通知，无需重复调用） 是**幂等重入的正常态**，不得当失败处理。

**两套协议定位键不得互换**：签约查询与解除走 `plan_id` + `out_contract_code`（**两支都是 path 段**）， 预约扣费与续期走 `contract_id`（签约成功后微信分配）⇒ 「商户侧协议号」与「微信协议号」分属两套， SDK 不做本地映射，由宿主自行持有。

**三条产品线共存于一路由树**：`/papay/sign/*`（通用代扣签约）、`/papay/scheduled-deduct-sign/*` （预约扣费签约）、`/papay/insurance-sign/*` + `/papay/insurance-pay/*`（保险自动续费） ⇒ 应答与请求字段集互异，**不得跨产品线套用类型**。

**四端预签约各建一型**：`.../pre-entrust-sign/{app,h5,jsapi,mini-program}` 与 `.../insurance-sign/contracts/pre-entrust-sign/{h5,jsapi,mini-program}` 的官方**请求表字段集不同** （APP 支 13 行、H5 支 15 行、JSAPI/小程序支 13 行；保险面 H5 支 37 行、JSAPI 与小程序支各 35 行）⇒ 请求侧**逐端建型**； 只有**应答同形**者共用（JSAPI 与 H5 同为 1 行 `pre_entrust_sign_id` ⇒ 共用；APP 与小程序各回 3 行、形态亦不同 ⇒ 单建）。

**「受理成功」不等于「扣款成功」**：`/papay/pay/harmony/transactions/apply`、 `/papay/insurance-pay/transactions/apply` 与预约扣费两支官方均**只返回受理结果**， 扣费结果由「委托代扣支付结果通知」异步下发（亦可查订单）⇒ 不得据 HTTP 200 判定扣款成功。

**续期支无应答包体**：`POST .../contract-id/{contract_id}/renew` 官方应答节原文为「无应答包体」 ⇒ 返回裸 `Task`；其重试口径官方明确要求**先收「签解约通知」或调扣费周期列表确认续期结果**后再重试 （`500 SYSTEM_ERROR` 的解决方案原文），盲目重试会产生重复周期。

**扣费周期列表支的分页**：`GET /v3/papay/insurance-sign/policy_periods/.../out-contract-code/{out_contract_code}` 是本域唯一带 query 的支，`limit` / `offset` 官方均**选填**（与本仓营销面多支「必填分页」口径不同，照页不套）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 20 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- （出现 19 次）**支持商户**【普通商户】。
- （出现 3 次）**业务错误码**：400 INVALID_REQUEST：无效请求 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。
- （出现 11 次）**业务错误码**：400 INVALID_REQUEST：无效请求 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。
- （出现 3 次）**必须带 `Wechatpay-Serial`**：官方请求头表把该头列为**必填**（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名**不**承载该头（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径，由传输层按商户配置注入）；密文以 `string` 承载，SDK **不代为加解密**。

### 1. `SendPapayDeductNotifyAsync` — 预扣费通知。
官方文档：https://pay.weixin.qq.com/doc/v2/merchant/4011987402（【直连商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/contracts/{contractId}/notify`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 签约成功后，微信返回的委托代扣协议id。 示例值：123124412412423431（官方 path `contract_id`，**必填**，类型 `string[1,32]`）。 |

- **请求体**：`PapayContractNotifyRequest` — 预扣费通知请求体，字段见 `PapayContractNotifyRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/contracts/{contract_id}/notify`；path `contract_id`。
- **支持商户**【直连商户】。
- **业务错误码**：SYSTEMERROR：接口返回错误 / CONTRACT_NOT_EXIST：签约协议不存在 / PARAM_ERROR：参数错误 / APPID_MCHID_NOT_MATCH：appid和mchid不匹配 / INVALID_REQUEST：无效请求 / RULELIMIT：该请求存在风险 / RESOURCE_ALREADY_EXISTS：资源已经存在。

### 2. `QueryPapayContractAsync` — 代扣签约协议查询。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489245（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`GET /v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `int` | 【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path `plan_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |
- **返回**：`PapayContractInfoResponse` — 代扣签约协议查询，见 `PapayContractInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/papay/sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}`；path `plan_id` / `out_contract_code`。

### 3. `TerminatePapayContractAsync` — 代扣签约协议解除。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489295（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `int` | 【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path `plan_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`PapayContractTerminateRequest` — 代扣签约协议解除请求体，字段见 `PapayContractTerminateRequest`。
- **返回**：`PapayContractInfoResponse` — 代扣签约协议解除，见 `PapayContractInfoResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate`；path `plan_id` / `out_contract_code`。
- **应答**见 `PapayContractInfoResponse`（官方应答表逐字段建模，与 `GET /papay/sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 4. `QueryPapayDeductScheduleAsync` — 预约扣费结果。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466997（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`GET /v3/papay/pay/schedules/contract-id/{contractId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID。（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
- **返回**：`PapayDeductScheduleInfoResponse` — 预约扣费结果，见 `PapayDeductScheduleInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/papay/pay/schedules/contract-id/{contract_id}`；path `contract_id`。

### 5. `CreatePapayDeductScheduleAsync` — 预约扣费受理。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012467036（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/pay/schedules/contract-id/{contractId}/schedule`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID。（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`PapayDeductScheduleCreateRequest` — 预约扣费受理请求体，字段见 `PapayDeductScheduleCreateRequest`。
- **返回**：`PapayDeductScheduleInfoResponse` — 预约扣费受理，见 `PapayDeductScheduleInfoResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/pay/schedules/contract-id/{contract_id}/schedule`；path `contract_id`。
- **应答**见 `PapayDeductScheduleInfoResponse`（官方应答表逐字段建模，与 `GET /papay/pay/schedules/contract-id/{contract_id}` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 6. `ApplyPapayHarmonyDeductAsync` — 代扣受理扣款。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4013388429（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/pay/harmony/transactions/apply`

- **请求体**：`PapayHarmonyTransactionApplyRequest` — 代扣受理扣款请求体，字段见 `PapayHarmonyTransactionApplyRequest`。
- **返回**：`PapayHarmonyTransactionApplyResponse` — 代扣受理扣款，见 `PapayHarmonyTransactionApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/pay/harmony/transactions/apply`；无 path / query 参数。
- **应答**见 `PapayHarmonyTransactionApplyResponse`（官方应答表逐字段建模）。

### 7. `CreatePapayScheduledPreEntrustSignAppAsync` — 预约扣费APP预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012524934（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/app`

- **请求体**：`PapayScheduledPreEntrustSignAppRequest` — 预约扣费APP预签约请求体，字段见 `PapayScheduledPreEntrustSignAppRequest`。
- **返回**：`PapayScheduledPreEntrustSignAppResponse` — 预约扣费APP预签约，见 `PapayScheduledPreEntrustSignAppResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/app`；无 path / query 参数。
- **应答**见 `PapayScheduledPreEntrustSignAppResponse`（官方应答表逐字段建模）。

### 8. `CreatePapayScheduledPreEntrustSignH5Async` — 预约扣费H5预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489208（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5`

- **请求体**：`PapayScheduledPreEntrustSignH5Request` — 预约扣费H5预签约请求体，字段见 `PapayScheduledPreEntrustSignH5Request`。
- **返回**：`PapayScheduledPreEntrustSignH5Response` — 预约扣费H5预签约，见 `PapayScheduledPreEntrustSignH5Response`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5`；无 path / query 参数。
- **应答**见 `PapayScheduledPreEntrustSignH5Response`（官方应答表逐字段建模）。

### 9. `CreatePapayScheduledPreEntrustSignJsapiAsync` — 预约扣费JSAPI预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012525133（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/jsapi`

- **请求体**：`PapayScheduledPreEntrustSignJsapiRequest` — 预约扣费JSAPI预签约请求体，字段见 `PapayScheduledPreEntrustSignJsapiRequest`。
- **返回**：`PapayScheduledPreEntrustSignH5Response` — 预约扣费JSAPI预签约，见 `PapayScheduledPreEntrustSignH5Response`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/jsapi`；无 path / query 参数。
- **应答**见 `PapayScheduledPreEntrustSignH5Response`（官方应答表逐字段建模，与 `POST /papay/scheduled-deduct-sign/contracts/pre-entrust-sign/h5` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 10. `CreatePapayScheduledPreEntrustSignMiniProgramAsync` — 预约扣费小程序预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012525209（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/mini-program`

- **请求体**：`PapayScheduledPreEntrustSignMiniProgramRequest` — 预约扣费小程序预签约请求体，字段见 `PapayScheduledPreEntrustSignMiniProgramRequest`。
- **返回**：`PapayScheduledPreEntrustSignMiniProgramResponse` — 预约扣费小程序预签约，见 `PapayScheduledPreEntrustSignMiniProgramResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/scheduled-deduct-sign/contracts/pre-entrust-sign/mini-program`；无 path / query 参数。
- **应答**见 `PapayScheduledPreEntrustSignMiniProgramResponse`（官方应答表逐字段建模）。

### 11. `CreatePapayInsurancePreEntrustSignH5Async` — 保险自动续费H5预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529050（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-sign/contracts/pre-entrust-sign/h5`

- **请求体**：`PapayInsurancePreEntrustSignH5Request` — 保险自动续费H5预签约请求体，字段见 `PapayInsurancePreEntrustSignH5Request`。
- **返回**：`PapayInsurancePreEntrustSignH5Response` — 保险自动续费H5预签约，见 `PapayInsurancePreEntrustSignH5Response`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-sign/contracts/pre-entrust-sign/h5`；无 path / query 参数。
- **应答**见 `PapayInsurancePreEntrustSignH5Response`（官方应答表逐字段建模）。

### 12. `CreatePapayInsurancePreEntrustSignJsapiAsync` — 保险自动续费JSAPI预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012528600（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-sign/contracts/pre-entrust-sign/jsapi`

- **请求体**：`PapayInsurancePreEntrustSignJsapiRequest` — 保险自动续费JSAPI预签约请求体，字段见 `PapayInsurancePreEntrustSignJsapiRequest`。
- **返回**：`PapayInsurancePreEntrustSignH5Response` — 保险自动续费JSAPI预签约，见 `PapayInsurancePreEntrustSignH5Response`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-sign/contracts/pre-entrust-sign/jsapi`；无 path / query 参数。
- **应答**见 `PapayInsurancePreEntrustSignH5Response`（官方应答表逐字段建模，与 `POST /papay/insurance-sign/contracts/pre-entrust-sign/h5` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 13. `CreatePapayInsurancePreEntrustSignMiniProgramAsync` — 保险自动续费小程序预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012528950（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-sign/contracts/pre-entrust-sign/mini-program`

- **请求体**：`PapayInsurancePreEntrustSignMiniProgramRequest` — 保险自动续费小程序预签约请求体，字段见 `PapayInsurancePreEntrustSignMiniProgramRequest`。
- **返回**：`PapayInsurancePreEntrustSignMiniProgramResponse` — 保险自动续费小程序预签约，见 `PapayInsurancePreEntrustSignMiniProgramResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-sign/contracts/pre-entrust-sign/mini-program`；无 path / query 参数。
- **应答**见 `PapayInsurancePreEntrustSignMiniProgramResponse`（官方应答表逐字段建模）。

### 14. `QueryPapayInsuranceContractAsync` — 保险自动续费协议。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012472641（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`GET /v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `int` | 【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path `plan_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |
- **返回**：`PapayInsuranceContractInfoResponse` — 保险自动续费协议，见 `PapayInsuranceContractInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/papay/insurance-sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}`；path `plan_id` / `out_contract_code`。

### 15. `TerminatePapayInsuranceContractAsync` — 保险自动续费协议解除。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012472767（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-sign/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `int` | 【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path `plan_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`PapayInsuranceContractTerminateRequest` — 保险自动续费协议解除请求体，字段见 `PapayInsuranceContractTerminateRequest`。
- **返回**：`PapayInsuranceContractInfoResponse` — 保险自动续费协议解除，见 `PapayInsuranceContractInfoResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate`；path `plan_id` / `out_contract_code`。
- **应答**见 `PapayInsuranceContractInfoResponse`（官方应答表逐字段建模，与 `GET /papay/insurance-sign/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 16. `RenewPapayInsuranceContractAsync` — 续期自动续费协议受理。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489588（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-sign/contracts/plan-id/{planId}/contract-id/{contractId}/renew`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `int` | 【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path `plan_id`，**必填**，类型 `integer`）。 |
| `contractId` | Path | `string` | 【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`PapayInsuranceContractRenewRequest` — 续期自动续费协议受理请求体，字段见 `PapayInsuranceContractRenewRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-sign/contracts/plan-id/{plan_id}/contract-id/{contract_id}/renew`；path `plan_id` / `contract_id`。
- **业务错误码**：400 INVALID_REQUEST：无效请求 / 400 INVALID_REQUEST：协议处于挽留期，不支持进行除解约外其他操作 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。

### 17. `QueryPapayInsurancePolicyPeriodAsync` — 保险自动续费预约。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012471229（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`GET /v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID。（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
| `policyPeriodId` | Path | `int` | 【保单的扣费周期编号】 需要查询预约状态的保单的扣费周期编号。由商户在进行委托代扣签约时传入。需要大于0的正整数，且符合相关的业务规则。（官方 path `policy_period_id`，**必填**，类型 `integer`）。 |
- **返回**：`PapayInsurancePolicyPeriodResponse` — 保险自动续费预约，见 `PapayInsurancePolicyPeriodResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/papay/insurance-pay/policy-periods/contract-id/{contract_id}/policy-period-id/{policy_period_id}`；path `contract_id` / `policy_period_id`。

### 18. `SchedulePapayInsurancePolicyPeriodAsync` — 保险自动续费预约受理。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012471270（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-pay/policy-periods/contract-id/{contractId}/policy-period-id/{policyPeriodId}/schedule`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `contractId` | Path | `string` | 【委托代扣协议ID】签约成功后，微信返回的委托代扣协议ID。（官方 path `contract_id`，**必填**，类型 `string(32)`）。 |
| `policyPeriodId` | Path | `int` | 【保单的扣费周期编号】进行预约的扣费周期编号，由商户在进行委托代扣签约时传入。需要大于0的正整数，且符合相关的业务规则。（官方 path `policy_period_id`，**必填**，类型 `integer`）。 |

- **请求体**：`PapayInsurancePolicyPeriodScheduleRequest` — 保险自动续费预约受理请求体，字段见 `PapayInsurancePolicyPeriodScheduleRequest`。
- **返回**：`PapayInsurancePolicyPeriodResponse` — 保险自动续费预约受理，见 `PapayInsurancePolicyPeriodResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-pay/policy-periods/contract-id/{contract_id}/policy-period-id/{policy_period_id}/schedule`；path `contract_id` / `policy_period_id`。
- **应答**见 `PapayInsurancePolicyPeriodResponse`（官方应答表逐字段建模，与 `GET /papay/insurance-pay/policy-periods/contract-id/{contract_id}/policy-period-id/{policy_period_id}` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 INVALID_REQUEST：无效请求 / 400 INVALID_REQUEST：协议处于挽留期，不支持进行除解约外其他操作 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。

### 19. `QueryPapayInsurancePolicyPeriodsAsync` — 保险扣费周期列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489530（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`GET /v3/papay/insurance-sign/policy_periods/plan-id/{planId}/out-contract-code/{outContractCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `int` | 【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容。（官方 path `plan_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 设置该次请求返回的最大扣费周期条数，范围[1,120]，默认值为20（官方 query `limit`，选填，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回。默认值为0（官方 query `offset`，选填，类型 `integer`）。 |
- **返回**：`PapayInsurancePolicyPeriodListResponse` — 保险扣费周期列表，见 `PapayInsurancePolicyPeriodListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/papay/insurance-sign/policy_periods/plan-id/{plan_id}/out-contract-code/{out_contract_code}`；path `plan_id` / `out_contract_code`，query `limit` / `offset`。
- **业务错误码**：400 INVALID_REQUEST：无效请求 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。

### 20. `ApplyPapayInsuranceDeductAsync` — 保险自动续费扣款受理。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012471158（【普通商户】）。

- **声明接口**：[IWechatPayPapayService](./IWechatPayPapayService.cs)
- **官方契约**：`POST /v3/papay/insurance-pay/transactions/apply`

- **请求体**：`PapayInsuranceTransactionApplyRequest` — 保险自动续费扣款受理请求体，字段见 `PapayInsuranceTransactionApplyRequest`。
- **返回**：`PapayInsuranceTransactionApplyResponse` — 保险自动续费扣款受理，见 `PapayInsuranceTransactionApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/papay/insurance-pay/transactions/apply`；无 path / query 参数。
- **应答**见 `PapayInsuranceTransactionApplyResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 INVALID_REQUEST：无效请求 / 400 INVALID_REQUEST：协议处于挽留期，不支持进行除解约外其他操作 / 403 CONTRACT_NOT_EXIST：签约协议不存在 / 403 NO_AUTH：商户暂无权限使用此功能 / 429 FREQUENCY_LIMITED：频率超限 / 500 SYSTEM_ERROR：接口返回错误。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/Papay`](../../../Mud.Wechat.Pay/Papay/) 内共 **82** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **82** 个，`SerializerClassName` = `Papay`。

AOT 源生成上下文：[`PapayJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/PapayJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PapayContractNotifyRequest` | 请求体 | [Papay/PapayContractNotifyDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayContractNotifyDtos.cs) | `count`, `unit`, `amount`, `currency`, `mchid`, `appid`, `deduct_duration`, `estimated_amount`, `sp_mchid`, `sp_appid`, `sub_mchid`, `sub_appid` |
| `PapayContractInfoResponse` | 应答 | [Papay/PapaySignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapaySignDtos.cs) | `contract_termination_mode`, `contract_terminated_time`, `contract_termination_remark`, `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `schedule_state`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `mchid` … |
| `PapayContractTerminateRequest` | 请求体 | [Papay/PapaySignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapaySignDtos.cs) | `contract_termination_mode`, `contract_terminated_time`, `contract_termination_remark`, `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `schedule_state`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `mchid` … |
| `PapayDeductScheduleInfoResponse` | 应答 | [Papay/PapayDeductPayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayDeductPayDtos.cs) | `total`, `currency`, `schedule_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `schedule_amount`, `out_trade_no`, `description` … |
| `PapayDeductScheduleCreateRequest` | 请求体 | [Papay/PapayDeductPayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayDeductPayDtos.cs) | `total`, `currency`, `schedule_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `schedule_amount`, `out_trade_no`, `description` … |
| `PapayHarmonyTransactionApplyResponse` | 应答 | [Papay/PapayDeductPayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayDeductPayDtos.cs) | `total`, `currency`, `schedule_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `schedule_amount`, `out_trade_no`, `description` … |
| `PapayHarmonyTransactionApplyRequest` | 请求体 | [Papay/PapayDeductPayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayDeductPayDtos.cs) | `total`, `currency`, `schedule_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `schedule_amount`, `out_trade_no`, `description` … |
| `PapayScheduledPreEntrustSignAppResponse` | 应答 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayScheduledPreEntrustSignAppRequest` | 请求体 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayScheduledPreEntrustSignH5Response` | 应答 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayScheduledPreEntrustSignH5Request` | 请求体 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayScheduledPreEntrustSignJsapiRequest` | 请求体 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayScheduledPreEntrustSignMiniProgramResponse` | 应答 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayScheduledPreEntrustSignMiniProgramRequest` | 请求体 | [Papay/PapayScheduledSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayScheduledSignDtos.cs) | `total`, `currency`, `estimated_deduct_date`, `estimated_deduct_amount`, `description`, `appid`, `openid`, `plan_id`, `out_contract_code`, `contract_display_account`, `contract_notify_url`, `out_user_code` … |
| `PapayInsurancePreEntrustSignH5Response` | 应答 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsurancePreEntrustSignH5Request` | 请求体 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsurancePreEntrustSignJsapiRequest` | 请求体 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsurancePreEntrustSignMiniProgramResponse` | 应答 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsurancePreEntrustSignMiniProgramRequest` | 请求体 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsuranceContractInfoResponse` | 应答 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsuranceContractTerminateRequest` | 请求体 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsuranceContractRenewRequest` | 请求体 | [Papay/PapayInsuranceSignDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsuranceSignDtos.cs) | `total`, `currency`, `clause_name`, `clause_quota`, `policy_amount`, `policy_clauses`, `policy_period_id`, `estimated_deduct_date`, `estimated_deduct_amount`, `policy_coverage_detail`, `real_name`, `id_card_number` … |
| `PapayInsurancePolicyPeriodResponse` | 应答 | [Papay/PapayInsurancePayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsurancePayDtos.cs) | `total`, `currency`, `policy_period_id`, `policy_period_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `estimated_deduct_date`, `estimated_deduct_amount` … |
| `PapayInsurancePolicyPeriodScheduleRequest` | 请求体 | [Papay/PapayInsurancePayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsurancePayDtos.cs) | `total`, `currency`, `policy_period_id`, `policy_period_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `estimated_deduct_date`, `estimated_deduct_amount` … |
| `PapayInsurancePolicyPeriodListResponse` | 应答 | [Papay/PapayInsurancePayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsurancePayDtos.cs) | `total`, `currency`, `policy_period_id`, `policy_period_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `estimated_deduct_date`, `estimated_deduct_amount` … |
| `PapayInsuranceTransactionApplyResponse` | 应答 | [Papay/PapayInsurancePayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsurancePayDtos.cs) | `total`, `currency`, `policy_period_id`, `policy_period_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `estimated_deduct_date`, `estimated_deduct_amount` … |
| `PapayInsuranceTransactionApplyRequest` | 请求体 | [Papay/PapayInsurancePayDtos.cs](../../../Mud.Wechat.Pay/Papay/PapayInsurancePayDtos.cs) | `total`, `currency`, `policy_period_id`, `policy_period_state`, `deduct_start_date`, `deduct_end_date`, `scheduled_amount`, `deduct_amount`, `deduct_date`, `appid`, `estimated_deduct_date`, `estimated_deduct_amount` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Papay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Papay));

var service = app.Services.GetRequiredService<IWechatPayPapayService>();
var ct = CancellationToken.None;
var response = await service.ApplyPapayHarmonyDeductAsync(new PapayHarmonyTransactionApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/papay/pay/harmony/transactions/apply");
// 占位值仅用于编译示意；业务值须按 IWechatPayPapayService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyPapayHarmonyDeductAsync`）；请求体 `PapayHarmonyTransactionApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPapayService](./IWechatPayPapayService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayEducationPapayContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEducationPapayContractGuards.cs)、[`WechatPayPapayContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPapayContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `EducationPapayTopology_ShouldMatchInterfaceAndGroupRoster`、`EducationPapayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`EducationPapayDtos_ShouldExposeOfficialFieldRosters`、`EducationPapayDataModels_ShouldSplitRequestAndResponseBaseTypes`、`EducationPapayAllDtos_ShouldBeAnnotatedAndContextRegistered`、`EducationPapayRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PapayTopology_ShouldMatchInterfaceAndGroupRoster`、`PapayEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PapayDtos_ShouldExposeOfficialFieldRosters`、`PapayDataModels_ShouldSplitRequestAndResponseBaseTypes`、`PapayAllDtos_ShouldBeAnnotatedAndContextRegistered`、`PapayRegistrars_ShouldBeWiredThroughPayModuleAndResolver` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/Papay`](../../../Mud.Wechat.Pay/Papay/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayEducationPapayContractGuards.cs`、`WechatPayPapayContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
