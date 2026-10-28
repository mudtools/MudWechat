# PayrollCard 域 · 代发薪（PayrollCard 服务商面）

微信支付「代发薪（PayrollCard 服务商面）」域 SDK（6 端点）。 接口：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)；注册组 `PayrollCard`；模块枚举 `PayModule.PayrollCard`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPayrollCardContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayrollCardContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/PayrollCard`](../../../Mud.Wechat.Pay/PayrollCard/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPayrollCardService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPayrollCardService` | `CreatePayrollCardTokenAsync` | POST | `/v3/payroll-card/tokens` | 商户证书 / APIv3 签名 | 获取代发薪用户授权令牌。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461589（【普通服务商】）。 |
| 2 | `IWechatPayPayrollCardService` | `QueryPayrollCardRelationAsync` | GET | `/v3/payroll-card/relations/{openid}` | 商户证书 / APIv3 签名 | 查询用户代发薪授权与开通状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461728（【普通服务商】）。 |
| 3 | `IWechatPayPayrollCardService` | `CreatePayrollCardAuthenticationPreOrderAsync` | POST | `/v3/payroll-card/authentications/pre-order-with-auth` | 商户证书 / APIv3 签名 | 凭授权单号预下单取令牌。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461761（【普通服务商】）。 |
| 4 | `IWechatPayPayrollCardService` | `QueryPayrollCardAuthenticationAsync` | GET | `/v3/payroll-card/authentications/{authenticateNumber}` | 商户证书 / APIv3 签名 | 查询代发薪授权单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461785（【普通服务商】）。 |
| 5 | `IWechatPayPayrollCardService` | `QueryPayrollCardAuthenticationsAsync` | GET | `/v3/payroll-card/authentications` | 商户证书 / APIv3 签名 | 按日期分页查询代发薪授权单列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461820（【普通服务商】）。 |
| 6 | `IWechatPayPayrollCardService` | `CreatePayrollCardTransferBatchAsync` | POST | `/v3/payroll-card/transfer-batches` | 商户证书 / APIv3 签名 | 发起代发薪批量转账。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461824（【普通服务商】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中**6 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：1 / 6 支官方页**印有**端点级数字（已原样抄入各方法 remarks），其余支**未印** ⇒ 未印者不得凭第三方 SDK 补数。

**涉密面是逐页核对结论**：本接口 6 支里**3 支**的官方请求头表把 `Wechatpay-Serial` 列为**必填**（敏感字段密文上送），其余支**未列**该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 `string` 承载、SDK **不代为加解密**。

**六支官方全部只印服务商页**（各页均标「支持商户：【普通服务商】」）⇒ 无直连面镜像、无双页归并问题。 **本域没有 `sp_mchid`**：请求侧只有 `sub_mchid`（服务商身份由签名主体承载）， 而**应答**侧的服务商商户号字段名是 `mchid`；AppID 命名亦两套并存——令牌申领、授权关系与认证单三支用 `appid`（服务商）+ `sub_appid`，发起批次支用 `sp_appid` + `sub_appid` ⇒ 按「服务商面必是 sp_*」归一即错。

**路由分四段、生命周期是两段式**：`/payroll-card/tokens`（申领授权令牌）→ `/payroll-card/authentications/pre-order-with-auth`（凭核身单号预下单取令牌）→ `/payroll-card/transfer-batches`（发起批次），外加 `/relations/{openid}` 与认证单两支查询。 「申领令牌」与「凭令牌发薪」不可折叠成一支，`/payroll-card/authentications` 的三支也各按定位键分列。

**密文与必填口径逐页钉死，不得跨页套用**：`user_name` 与 `id_card_number` 在令牌支和预下单支 官方都标「按照APIV3标准加密该字段」且类型 `string(1024)`；但 `employment_type` 在预下单支是**必填**、 在令牌支**选填**（官方原文「未授权必填，授权过非必填」）⇒ 两个请求类型各建、口径照页不得归一。 本域证件号叫 `id_card_number`，进件域是 `identification_no` ⇒ 跨域「对齐字段名」即造出官方不存在的报文。

**令牌申领应答与预下单应答不同形 ⇒ 各建一型**：前者 5 键（`openid` / `mchid` / `sub_mchid` / `token` / `expires_in`），后者多 `authenticate_number`（商家核身单号，即认证单查询支的定位键）⇒ 6 键。 按「字段高度重合」合并即造出官方不存在的契约。

**认证单两支各建一应答**：按 `authenticate_number` 单查的应答与列表支 `data[]` 元素 官方字段表**逐字同形**（13 键同序），但本线口径是**共用只发生在应答根类型**、子树按「端点 + 路径段」 机械拼装独立成类型 ⇒ 列表元素类型不与单查应答共用；列表支另带 `total_count` / `offset` / `limit` 包装。

**分页三键在「应答侧」必填、在「query 侧」选填**：列表支的 `total_count` / `offset` / `limit` 是官方**应答表**必填三键，而其请求 query 的 `offset` / `limit` 官方标**选填**，单查支根本没有这两键 ⇒ 同名字段分属两侧、两套口径不得互套，也不得把 query 侧做成必填。

**本域唯一印端点级 QPS 的接口**：`POST /v3/payroll-card/transfer-batches` 官方原文 「单个服务商（发起批量转账接口）50QPS，超过限制会报 `FREQUENCY_LIMITED`」⇒ 已照页写入该方法 remarks； 其余五支官方未印数字，不得凭第三方 SDK 补数。

**清单里两条 `/payroll-card/wesure/*`（insurance、token-validations）判非契约、零声明零建模**： 该路由族在 PayrollCard 六支现行页正文里**零命中**，且表内为 token-validations 引的官方页 4012461785 实为 `GET /v3/payroll-card/authentications/{authenticate_number}`（张冠李戴）⇒ 与 P0-9 微企付 「官方出处缺失」同一证据面，由零声明守卫断言其在 `Src/` 全源码零声明。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**支持商户**【普通服务商】。
- （出现 5 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- （出现 3 次）**必须带 `Wechatpay-Serial`**：官方请求头表把该头列为**必填**（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名**不**承载该头（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径，由传输层按商户配置注入）；密文以 `string` 承载，SDK **不代为加解密**。

### 1. `CreatePayrollCardTokenAsync` — 获取代发薪用户授权令牌。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461589（【普通服务商】）。

- **声明接口**：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)
- **官方契约**：`POST /v3/payroll-card/tokens`

- **请求体**：`PayrollCardTokenRequest` — 获取代发薪用户授权令牌请求体，字段见 `PayrollCardTokenRequest`。
- **返回**：`PayrollCardTokenResponse` — 获取代发薪用户授权令牌，见 `PayrollCardTokenResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payroll-card/tokens`；无 path / query 参数。
- **应答**见 `PayrollCardTokenResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_MISSING：参数缺失 / 400 PARAM_ILLEGAL：参数值不合法 / 401 MCH_TICKET_ILLEGAL：服务商票据校验失败 / 400 AUTH_ERROR：校验身份或者产品权限失败 / 403 GEN_TOKEN_ERROR：生成token失败 / 403 MERCHANT_INFO_ERROR：获取商户信息失败 / 403 AUTH_INFO_ERROR：获取授权信息失败。

### 2. `QueryPayrollCardRelationAsync` — 查询用户代发薪授权与开通状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461728（【普通服务商】）。

- **声明接口**：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)
- **官方契约**：`GET /v3/payroll-card/relations/{openid}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 微信用户OpenID（官方 path `openid`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【特约商户号】 特约商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `appid` | Query `appid` | `string?` | 【服务商AppID】 当输入服务商AppID时会校验其与服务商商户号绑定关系。 服务商AppID和特约商户AppID至少输入一个。（官方 query `appid`，选填，类型 `string(32)`）。 |
| `subAppid` | Query `sub_appid` | `string?` | 【特约商户AppID】 当输入特约商户AppID时会校验其与特约商户号关系。 特约商户AppID和服务商AppID至少输入一个。（官方 query `sub_appid`，选填，类型 `string(32)`）。 |
- **返回**：`PayrollCardRelationResponse` — 查询用户代发薪授权与开通状态，见 `PayrollCardRelationResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payroll-card/relations/{openid}`；path `openid`，query `sub_mchid` / `appid` / `sub_appid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_MISSING：参数缺失 / 400 PARAM_ILLEGAL：参数值不合法 / 401 MCH_TICKET_ILLEGAL：服务商票据校验失败 / 400 AUTH_ERROR：校验身份或者产品权限失败 / 403 MERCHANT_INFO_ERROR：获取商户信息失败 / 403 AUTH_INFO_ERROR：获取授权信息失败。

### 3. `CreatePayrollCardAuthenticationPreOrderAsync` — 凭授权单号预下单取令牌。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461761（【普通服务商】）。

- **声明接口**：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)
- **官方契约**：`POST /v3/payroll-card/authentications/pre-order-with-auth`

- **请求体**：`PayrollCardAuthenticationPreOrderRequest` — 凭授权单号预下单取令牌请求体，字段见 `PayrollCardAuthenticationPreOrderRequest`。
- **返回**：`PayrollCardAuthenticationPreOrderResponse` — 凭授权单号预下单取令牌，见 `PayrollCardAuthenticationPreOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payroll-card/authentications/pre-order-with-auth`；无 path / query 参数。
- **应答**见 `PayrollCardAuthenticationPreOrderResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 INVALID_REQUEST：商家核身单号已使用，请调整后再试 / 400 INVALID_REQUEST：该商户当日核身下单已超限额 / 400 INVALID_REQUEST：校验商户身份失败 / 400 INVALID_REQUEST：校验商户身份或者产品权限失败 / 400 INVALID_REQUEST：下单用工类型与授权用工类型不匹配 / 400 INVALID_REQUEST：核身下单过于频繁 / 400 PARAM_ERROR：参数值不正确，必填参数为空。

### 4. `QueryPayrollCardAuthenticationAsync` — 查询代发薪授权单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461785（【普通服务商】）。

- **声明接口**：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)
- **官方契约**：`GET /v3/payroll-card/authentications/{authenticateNumber}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authenticateNumber` | Path | `string` | 【商家核身单号】 商户系统内部的商家核身单号，要求此参数只能由数字、大小写字母组成，在服务商内部唯一（官方 path `authenticate_number`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【特约商户号】 特约商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`PayrollCardAuthenticationInfoResponse` — 查询代发薪授权单，见 `PayrollCardAuthenticationInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payroll-card/authentications/{authenticate_number}`；path `authenticate_number`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：500 SYSTEM_ERROR：系统错误。

### 5. `QueryPayrollCardAuthenticationsAsync` — 按日期分页查询代发薪授权单列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461820（【普通服务商】）。

- **声明接口**：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)
- **官方契约**：`GET /v3/payroll-card/authentications`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Query `openid` | `string` | 【用户OpenID】 微信用户OpenID（官方 query `openid`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【特约商户号】 特约商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `authenticateDate` | Query `authenticate_date` | `string` | 【核身日期】 核身日期，一次只能查询一天，最久可查询90天内的记录，格式为“yyyy-MM-DD”（官方 query `authenticate_date`，**必填**，类型 `string(10)`）。 |
| `appid` | Query `appid` | `string?` | 【服务商AppID】 当输入服务商AppID时会校验其与服务商商户号绑定关系。 服务商AppID和特约商户AppID至少输入一个。（官方 query `appid`，选填，类型 `string(32)`）。 |
| `subAppid` | Query `sub_appid` | `string?` | 【特约商户AppID】 当输入特约商户AppID时会校验其与特约商户号关系。 特约商户AppID和服务商AppID至少输入一个。（官方 query `sub_appid`，选填，类型 `string(32)`）。 |
| `authenticateState` | Query `authenticate_state` | `string?` | 【核身状态】 核身状态，列表查询仅提供成功状态的核身记录查询，故此字段固定默认值即可（官方 query `authenticate_state`，选填，类型 `string(32)`）。 |
| `offset` | Query `offset` | `int?` | 【本次查询偏移量】 非负整数，表示该次请求资源的起始位置，从0开始计数。调用方选填，默认为0。offset为10，limit为10时，查询第10-19条数据（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【本次请求最大查询条数】 非0非负的整数，该次请求可返回的最大资源条数，默认值为10，最大支持10条。（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`PayrollCardAuthenticationListResponse` — 按日期分页查询代发薪授权单列表，见 `PayrollCardAuthenticationListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payroll-card/authentications`；query `openid` / `sub_mchid` / `authenticate_date` / `appid` / `sub_appid` / `authenticate_state` / `offset` / `limit`。
- **`openid` 与 `sub_mchid` 与 `authenticate_date` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：500 SYSTEM_ERROR：系统错误。

### 6. `CreatePayrollCardTransferBatchAsync` — 发起代发薪批量转账。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461824（【普通服务商】）。

- **声明接口**：[IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs)
- **官方契约**：`POST /v3/payroll-card/transfer-batches`

- **请求体**：`PayrollCardTransferBatchCreateRequest` — 发起代发薪批量转账请求体，字段见 `PayrollCardTransferBatchCreateRequest`。
- **返回**：`PayrollCardTransferBatchCreateResponse` — 发起代发薪批量转账，见 `PayrollCardTransferBatchCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payroll-card/transfer-batches`；无 path / query 参数。
- **频率限制**（官方原样）：单个服务商（发起批量转账接口）50QPS，如果超过频率限制，会报错FREQUENCY\_LIMITED，请降低频率请求。。
- **应答**见 `PayrollCardTransferBatchCreateResponse`（官方应答表逐字段建模）。
- **业务错误码**：500 SYSTEM_ERROR：系统错误 / 403 NO_AUTH：商户信息不合法 / 403 ACCOUNT_ERROR：商户账户付款受限 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 APPID_MCHID_NOT_MATCH：商户号和AppID没有绑定关系 / 403 NOT_ENOUGH：资金不足 / 429 QUOTA_EXCEED：超出商户单日转账额度 / 429 FREQUENCY_LIMITED：频率超限 / 403 NO_AUTH：未开通产品权限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/PayrollCard`](../../../Mud.Wechat.Pay/PayrollCard/) 内共 **11** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **11** 个，`SerializerClassName` = `PayrollCard`。

AOT 源生成上下文：[`PayrollCardJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/PayrollCardJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PayrollCardTokenResponse` | 应答 | [PayrollCard/PayrollCardTokenDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardTokenDtos.cs) | `openid`, `appid`, `sub_mchid`, `sub_appid`, `user_name`, `id_card_number`, `employment_type`, `mchid`, `token`, `expires_in` |
| `PayrollCardTokenRequest` | 请求体 | [PayrollCard/PayrollCardTokenDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardTokenDtos.cs) | `openid`, `appid`, `sub_mchid`, `sub_appid`, `user_name`, `id_card_number`, `employment_type`, `mchid`, `token`, `expires_in` |
| `PayrollCardRelationResponse` | 应答 | [PayrollCard/PayrollCardRelationDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardRelationDtos.cs) | `openid`, `mchid`, `sub_mchid`, `authorize_state`, `authorize_time`, `deauthorize_time`, `register_state`, `register_time`, `close_time` |
| `PayrollCardAuthenticationPreOrderResponse` | 应答 | [PayrollCard/PayrollCardAuthenticationDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardAuthenticationDtos.cs) | `openid`, `appid`, `sub_mchid`, `sub_appid`, `authenticate_number`, `project_name`, `employer_name`, `user_name`, `id_card_number`, `employment_type`, `authenticate_type`, `mchid` … |
| `PayrollCardAuthenticationPreOrderRequest` | 请求体 | [PayrollCard/PayrollCardAuthenticationDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardAuthenticationDtos.cs) | `openid`, `appid`, `sub_mchid`, `sub_appid`, `authenticate_number`, `project_name`, `employer_name`, `user_name`, `id_card_number`, `employment_type`, `authenticate_type`, `mchid` … |
| `PayrollCardAuthenticationInfoResponse` | 应答 | [PayrollCard/PayrollCardAuthenticationDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardAuthenticationDtos.cs) | `openid`, `appid`, `sub_mchid`, `sub_appid`, `authenticate_number`, `project_name`, `employer_name`, `user_name`, `id_card_number`, `employment_type`, `authenticate_type`, `mchid` … |
| `PayrollCardAuthenticationListResponse` | 应答 | [PayrollCard/PayrollCardAuthenticationDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardAuthenticationDtos.cs) | `openid`, `appid`, `sub_mchid`, `sub_appid`, `authenticate_number`, `project_name`, `employer_name`, `user_name`, `id_card_number`, `employment_type`, `authenticate_type`, `mchid` … |
| `PayrollCardTransferBatchCreateResponse` | 应答 | [PayrollCard/PayrollCardTransferBatchDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardTransferBatchDtos.cs) | `out_detail_no`, `transfer_amount`, `transfer_remark`, `openid`, `user_name`, `sub_mchid`, `sub_appid`, `authorization_type`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount` … |
| `PayrollCardTransferBatchCreateRequest` | 请求体 | [PayrollCard/PayrollCardTransferBatchDtos.cs](../../../Mud.Wechat.Pay/PayrollCard/PayrollCardTransferBatchDtos.cs) | `out_detail_no`, `transfer_amount`, `transfer_remark`, `openid`, `user_name`, `sub_mchid`, `sub_appid`, `authorization_type`, `out_batch_no`, `batch_name`, `batch_remark`, `total_amount` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PayrollCard;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PayrollCard));

var service = app.Services.GetRequiredService<IWechatPayPayrollCardService>();
var ct = CancellationToken.None;
var response = await service.CreatePayrollCardAuthenticationPreOrderAsync(new PayrollCardAuthenticationPreOrderRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/payroll-card/authentications/pre-order-with-auth");
// 占位值仅用于编译示意；业务值须按 IWechatPayPayrollCardService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreatePayrollCardAuthenticationPreOrderAsync`）；请求体 `PayrollCardAuthenticationPreOrderRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPayrollCardService](./IWechatPayPayrollCardService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPayrollCardContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayrollCardContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayrollCardTopology_ShouldMatchInterfaceAndGroupRoster`、`PayrollCardEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PayrollCardDtos_ShouldExposeOfficialFieldRosters`、`PayrollCardDataModels_ShouldSplitRequestAndResponseBaseTypes`、`PayrollCardAllDtos_ShouldBeAnnotatedAndContextRegistered`、`PayrollCardRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/PayrollCard`](../../../Mud.Wechat.Pay/PayrollCard/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPayrollCardContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
