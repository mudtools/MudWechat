# PartnerTransactions 域 · 服务商基础支付 / 分期

微信支付「服务商基础支付 / 分期」域 SDK（服务商下单五族 + 双路查单 + 关单 + 分期曝光咨询，
**9 端点**）。 接口：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)；注册组 `PartnerTransactions`；模块枚举 `PayModule.PartnerTransactions`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPartnerTransactionsContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransactionsContractGuards.cs)、[WechatPayBankPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[WechatPayCateringContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCateringContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PartnerTransactions`](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPartnerTransactionsService = 9；按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPartnerTransactionsService` | `CreateJsapiOrderAsync` | POST | `/v3/pay/partner/transactions/jsapi` | 商户证书 / APIv3 签名 | 服务商 JSAPI / 小程序下单，获取 `prepay_id`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012714911… |
| 2 | `IWechatPayPartnerTransactionsService` | `CreateNativeOrderAsync` | POST | `/v3/pay/partner/transactions/native` | 商户证书 / APIv3 签名 | 服务商 Native（扫码支付）下单，获取支付二维码链接 `code_url`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401… |
| 3 | `IWechatPayPartnerTransactionsService` | `CreateAppOrderAsync` | POST | `/v3/pay/partner/transactions/app` | 商户证书 / APIv3 签名 | 服务商 APP 下单，获取 `prepay_id`（供移动 SDK 调起）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40127… |
| 4 | `IWechatPayPartnerTransactionsService` | `CreateH5OrderAsync` | POST | `/v3/pay/partner/transactions/h5` | 商户证书 / APIv3 签名 | 服务商 H5（移动端网页）下单，获取可直接跳转的支付链接 `h5_url`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40127… |
| 5 | `IWechatPayPartnerTransactionsService` | `CreateScannedPosOrderAsync` | POST | `/v3/pay/partner/transactions/scannedpos` | 商户证书 / APIv3 签名 | 服务商扫码支付（被扫）下单，获取 `prepay_id`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012492476。 |
| 6 | `IWechatPayPartnerTransactionsService` | `QueryByTransactionIdAsync` | GET | `/v3/pay/partner/transactions/id/{transactionId}` | 商户证书 / APIv3 签名 | 服务商通过**微信支付订单号**查询订单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012738964<br>（同契约产品族分身页 … |
| 7 | `IWechatPayPartnerTransactionsService` | `QueryByOutTradeNoAsync` | GET | `/v3/pay/partner/transactions/out-trade-no/{outTradeNo}` | 商户证书 / APIv3 签名 | 服务商通过**商户订单号**查询订单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012739008<br>（同契约产品族分身页 ht… |
| 8 | `IWechatPayPartnerTransactionsService` | `CloseOrderAsync` | POST | `/v3/pay/partner/transactions/out-trade-no/{outTradeNo}/close` | 商户证书 / APIv3 签名 | 服务商关闭订单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012739019<br>（同契约产品族分身页 https://pay.w… |
| 9 | `IWechatPayPartnerTransactionsService` | `GetInstalExposureAdviceAsync` | POST | `/v3/pay/instal/partner/exposure` | 商户证书 / APIv3 签名 | 服务商分期支付「曝光咨询」：查询某用户是否推荐曝光分期入口，及各档期有无息费减免券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（服务商文档中心，2026-10-14 逐页核验）：JSAPI/小程序下单
https://pay.weixin.qq.com/doc/v3/partner/4012714911、Native 下单
https://pay.weixin.qq.com/doc/v3/partner/4012714902、APP 下单
https://pay.weixin.qq.com/doc/v3/partner/4012714669、H5 下单
https://pay.weixin.qq.com/doc/v3/partner/4012714759、扫码支付下单
https://pay.weixin.qq.com/doc/v3/partner/4012492476、订单号查询
https://pay.weixin.qq.com/doc/v3/partner/4012738964、商户订单号查询
https://pay.weixin.qq.com/doc/v3/partner/4012739008、关闭订单
https://pay.weixin.qq.com/doc/v3/partner/4012739019、分期曝光咨询
https://pay.weixin.qq.com/doc/v3/partner/4016196820。
除曝光咨询外，各端点另有产品族分身页（同一契约、不同接入形态章节），逐端点在方法 remarks 内列出。

**与直连域（`IWechatPayTransactionsService`）的形态差异**，本域纪律「表相同则共用，
表不同则分建」下的三条判据（守卫 PT-B1/PT-B3/PT-B4 锁定）：
① 商户标识**四分** —— `sp_appid`/`sp_mchid`/`sub_appid`/`sub_mchid`，
直连的 `appid`/`mchid` 在本域**不存在**；
② `payer` 是 `sp_openid` + `sub_openid` **二选一**（官方原文「必须要填一个」），
且**只有 JSAPI 与扫码支付两族有 `payer`**；
③ `support_fapiao` 在四族位于 `settle_info` 内、在扫码支付族位于**请求顶层**。
两套 DTO **不共用**（跨产品线共用会把直连域的序列化上下文登记牵进本域）。

**SKIT 清单里的三行本轮不落端点**（设计方案 §2.3 核验结论）：
`/pay/partner/transactions/codepay` 与 `/pay/partner/transactions/out-trade-no/{out_trade_no}/reverse`
的官方现行正文已是「付款码支付（V2）」指引页、无 APIv3 字段表 ⇒ 归 **APIv2 批次（P0-8）**；
`/pay/partner/transactions/exposure` 页面已下架（302 到 50x）⇒ 曝光只落分期族的
`GetInstalExposureAdviceAsync`。守卫 PT-B2 以**零声明**断言钉住这三条路由不出现在本线。

**无 `[Token]`、走商户签名**（守卫 `PAY-B1` fail-closed）：APIv3 全程以商户 API 证书做
RSA-SHA256 请求签名，本接口**不声明 `[Token]`**（违反即 MUD005 与 Query 白名单漂移）。
签名在传输层 `WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文。
**签名主体是服务商商户号**（`sp_mchid`），子商户号只出现在报文里 —— 服务商**不得**用子商户证书调本域。

**客户端**：走支付专用 `IWechatPayHttpClient`（命名客户端 `wechat-pay`，
BaseAddress 恒为 `https://api.mch.weixin.qq.com`）——不复用 `IEnhancedHttpClient` 默认实例，
否则会与企微线的 `qyapi.weixin.qq.com` 撞 BaseUrl。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 **HTTP 4xx/5xx** +
`{"code","message"}`。若不加本特性，组件会在非 2xx 直接抛 `ApiException`、把官方业务码丢掉；
加上后错误体原样落到 `WechatPayResponse.Code`/`Message`，调用方经
`WechatPayException.ThrowIfFailed` 判错。

**不做运行时多态**：五个下单族各自一个端点方法 + 各自请求 DTO（设计方案 §2.2）——
AOT 下源生成按声明类型序列化，族间差异（有无 `payer`、`scene_info` 形态、
`support_fapiao` 位置）用类型精确表达，不做「一个方法接枚举参数」的运行期分派。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateJsapiOrderAsync` — 服务商 JSAPI / 小程序下单，获取 `prepay_id`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012714911
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4012738519、
https://pay.weixin.qq.com/doc/v3/partner/4012759974、
https://pay.weixin.qq.com/doc/v3/partner/4012714678）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/partner/transactions/jsapi`

- **请求体**：`PartnerJsapiPrepayRequest` — 下单请求体，字段见 `PartnerJsapiPrepayRequest`。
- **返回**：`PartnerPrepayIdResponse` — 仅含 `prepay_id`（2 小时有效）的应答，见 `PartnerPrepayIdResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/partner/transactions/jsapi`； / 必带 `Accept: application/json` 与 `Content-Type: application/json`。
- **业务限制（官方原文要点）**：① `out_trade_no` 同一商户号下唯一，重复提交报 / `OUT_TRADE_NO_USED`(403)；② `sp_appid` 须与 `sp_mchid` 绑定、`sub_appid` 须与 / `sub_mchid` 绑定，错配报 `APPID_MCHID_NOT_MATCH`(400)；③ `payer` 内 / `sp_openid` 与 `sub_openid` **必须要填一个**，传 `sub_openid` 则 `sub_appid` 必填， / 归属错配报 `OPENID_MISMATCH`；④ `time_expire` 须在下单时间 15 天内且不得早于下单后 1 分钟， / 不传则默认 7 天未支付失效；⑤ `amount.total` 单位为分、必须大于 0。
- **补差金额**：`settle_info.subsidy_amount` 仅当 `profit_sharing = true` 时生效， / 官方限制单笔订单最高补差 5000 元。
- **频率**：超限返回 `FREQUENCY_LIMITED`(429)，官方要求降低请求频率。
- **官方业务错误码表（逐条核验）**：`400 PARAM_ERROR` / `400 INVALID_REQUEST` / / `401 SIGN_ERROR` / `400 APPID_MCHID_NOT_MATCH` / `400 MCH_NOT_EXISTS` / / `400 ORDER_CLOSED`（订单已关闭，须重新下单）/ `403 ACCOUNT_ERROR`（用户账号异常， / 官方明确**无需更多操作**，不得重试）/ `403 NO_AUTH` / `403 OUT_TRADE_NO_USED` / / `403 RULE_LIMIT` / `403 TRADE_ERROR` / `404 ORDER_NOT_EXIST` / / `429 FREQUENCY_LIMITED` / `500 SYSTEM_ERROR` / `500 BANK_ERROR`（银行系统异常， / 官方要求**用相同参数重新调用**）/ `500 INVALID_TRANSACTIONID` / `500 OPENID_MISMATCH`。

### 2. `CreateNativeOrderAsync` — 服务商 Native（扫码支付）下单，获取支付二维码链接 `code_url`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012714902
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4012738659）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/partner/transactions/native`

- **请求体**：`PartnerNativePrepayRequest` — 下单请求体，字段见 `PartnerNativePrepayRequest`。
- **返回**：`PartnerNativePrepayResponse` — 仅含 `code_url` 的应答，见 `PartnerNativePrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/partner/transactions/native`；无 path / query 参数。
- **与 `CreateJsapiOrderAsync` 的根本差异**：请求**无 `payer`** / （下单时不存在支付者标识），应答**只有 `code_url`** —— 支付由用户**扫码**发起， / 服务端须自行把 `code_url` 生成二维码展示。
- **`code_url` 不得拼接改写**；失效后须**重新调用本接口**取新链接。 / 业务限制（订单号唯一性 / AppID-商户号绑定 / `time_expire` 窗口 / 金额单位为分）同 JSAPI 族口径。

### 3. `CreateAppOrderAsync` — 服务商 APP 下单，获取 `prepay_id`（供移动 SDK 调起）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012714669
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4013080231）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/partner/transactions/app`

- **请求体**：`PartnerAppPrepayRequest` — 下单请求体，字段见 `PartnerAppPrepayRequest`。
- **返回**：`PartnerPrepayIdResponse` — 仅含 `prepay_id`（2 小时有效）的应答，见 `PartnerPrepayIdResponse`
（官方 APP 页应答表与 JSAPI 逐项一致 ⇒ **共用**同一类型）。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/partner/transactions/app`；无 path / query 参数。
- **AppID 形态收窄**：`sp_appid` / `sub_appid` 须为微信开放平台**移动应用** AppID， / 且各自须与对应商户号有绑定关系。
- **与直连域 APP 下单的差异**：官方服务商 APP 页字段表**没有** `subsidy_info`（补贴节） / ⇒ `PartnerAppPrepayRequest` 不提供该字段，不得据直连形态补。
- **调起支付在客户端侧**：`prepay_id` 回传 APP 后由移动 SDK 二次签名调起， / **服务端不得**替客户端组装该签名串；调起参数注入须按 `appid` + `partner_id` / 识别服务商形态。

### 4. `CreateH5OrderAsync` — 服务商 H5（移动端网页）下单，获取可直接跳转的支付链接 `h5_url`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012714759
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4012738604）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/partner/transactions/h5`

- **请求体**：`PartnerH5PrepayRequest` — 下单请求体，字段见 `PartnerH5PrepayRequest`。
- **返回**：`PartnerH5PrepayResponse` — 仅含 `h5_url` 的应答，见 `PartnerH5PrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/partner/transactions/h5`；无 path / query 参数。
- **⚠️ 本族最容易漏的一处**：`scene_info` **整体必填**且类型是 / `PartnerH5SceneInfo`（比其它族多一节必填的 `PartnerH5Info`）—— / 从 JSAPI / Native 请求体复制会**传不了** `h5_info`。请求另**无** `payer`。
- **拿到 `h5_url` 后**须按官方《H5 调起支付》指引跳转（含 `Referer` 等要求）， / **严禁**改写或拼装该链接。
- **仅在微信外浏览器场景可用**：微信内置浏览器场景须走 `CreateJsapiOrderAsync`。

### 5. `CreateScannedPosOrderAsync` — 服务商扫码支付（被扫）下单，获取 `prepay_id`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012492476。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/partner/transactions/scannedpos`

- **请求体**：`PartnerScannedPosPrepayRequest` — 下单请求体，字段见 `PartnerScannedPosPrepayRequest`。
- **返回**：`PartnerPrepayIdResponse` — 仅含 `prepay_id`（2 小时有效）的应答，见 `PartnerPrepayIdResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/partner/transactions/scannedpos`；无 path / query 参数。
- **本族字段约束与其余四族处处相反**（守卫 PT-B4 逐条锁定）： / ① `sp_appid` 与 `sub_appid` **二选一必填**（其余四族 `sp_appid` 恒必填）； / ② `notify_url` **选填**（其余四族必填）；③ `support_fapiao` 在**请求顶层**； / ④ `settle_info` 只有 `profit_sharing`（`PartnerScannedPosSettleInfo`）； / ⑤ `scene_info` **必填**、`payer_client_ip` **选填**、`store_info` **必填** / 且只有 `id`/`out_id`（`PartnerScannedPosSceneInfo`）。
- **本族不是 APIv2 的付款码支付**：官方 `codepay` 与 `reverse` 两页现行正文是 V2 指引、 / 无 APIv3 字段表 ⇒ 本域**零端点**，V2 归 P0-8 批次；守卫 PT-B2 断言这两条路由在本线零声明。

### 6. `QueryByTransactionIdAsync` — 服务商通过**微信支付订单号**查询订单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012738964
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4012738969、
https://pay.weixin.qq.com/doc/v3/partner/4012738971、
https://pay.weixin.qq.com/doc/v3/partner/4012738973、
https://pay.weixin.qq.com/doc/v3/partner/4012760565、
https://pay.weixin.qq.com/doc/v3/partner/4013080234）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`GET /v3/pay/partner/transactions/id/{transactionId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 微信支付订单号（路径参数，string(32)，必填）。 |
| `spMchId` | Query `sp_mchid` | `string` | 服务商户号（Query `sp_mchid`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（Query `sub_mchid`，必填 string(32)）。 |
- **返回**：`PartnerTransactionQueryResponse` — 订单详情，见 `PartnerTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/pay/partner/transactions/id/{transaction_id}`； / Query `sp_mchid` 与 `sub_mchid` **皆必填**（直连域只需 `mchid` 一个）； / 必带 `Accept: application/json`。
- **业务限制（官方原文要点）**：① 只能查**本服务商**下子商户的订单； / ② 查单**不可代替支付结果通知**，须以 `trade_state` 为准；③ 订单不存在返回 / `ORDER_NOT_EXIST`；④ 频率超限返回 `FREQUENCY_LIMITED`(429)。
- **与商户订单号查询共用应答**：两路由官方应答字段表**完全一致** ⇒ 同一 / `PartnerTransactionQueryResponse`，仅查询键不同。

### 7. `QueryByOutTradeNoAsync` — 服务商通过**商户订单号**查询订单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012739008
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4012759661、
https://pay.weixin.qq.com/doc/v3/partner/4012759714、
https://pay.weixin.qq.com/doc/v3/partner/4012760115、
https://pay.weixin.qq.com/doc/v3/partner/4012760568、
https://pay.weixin.qq.com/doc/v3/partner/4013080235）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`GET /v3/pay/partner/transactions/out-trade-no/{outTradeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outTradeNo` | Path | `string` | 商户订单号（路径参数，string(32)，必填）。 |
| `spMchId` | Query `sp_mchid` | `string` | 服务商户号（Query `sp_mchid`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（Query `sub_mchid`，必填 string(32)）。 |
- **返回**：`PartnerTransactionQueryResponse` — 订单详情，见 `PartnerTransactionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/pay/partner/transactions/out-trade-no/{out_trade_no}`； / Query `sp_mchid` 与 `sub_mchid` **皆必填**；必带 `Accept: application/json`。
- 业务限制同 `QueryByTransactionIdAsync`（查单不可代替支付结果通知； / 订单不存在报 `ORDER_NOT_EXIST`）。

### 8. `CloseOrderAsync` — 服务商关闭订单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012739019
（同契约产品族分身页 https://pay.weixin.qq.com/doc/v3/partner/4012759669、
https://pay.weixin.qq.com/doc/v3/partner/4012759725、
https://pay.weixin.qq.com/doc/v3/partner/4012760108、
https://pay.weixin.qq.com/doc/v3/partner/4012760574、
https://pay.weixin.qq.com/doc/v3/partner/4013080236）。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/partner/transactions/out-trade-no/{outTradeNo}/close`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outTradeNo` | Path | `string` | 商户订单号（路径参数，string(32)，必填）。 |

- **请求体**：`PartnerCloseOrderRequest` — 关单请求体（`sp_mchid` + `sub_mchid`），见 `PartnerCloseOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/partner/transactions/out-trade-no/{out_trade_no}/close`； / 必带 `Accept: application/json` 与 `Content-Type: application/json`；成功应答 `204`。
- **请求体只有两个字段**：`sp_mchid` / `sub_mchid`，**不含任何 AppID**（守卫 PT-B1）。
- **业务限制（官方原文要点）**：① 订单**已支付成功不能关闭**（须走退款，否则 / `403 TRADE_ERROR`）；② 关单后用户无法再支付该订单；③ 关单**异步**生效，须经查单确认； / ④ 已超时未支付订单官方自动关闭。
- **官方错误码表**：`400 PARAM_ERROR` / `401 SIGN_ERROR` / `500 SYSTEM_ERROR` / / `400 MCH_NOT_EXISTS` / `403 RULE_LIMIT` / `403 TRADE_ERROR` / `429 FREQUENCY_LIMITED`。
- **本域没有撤销（reverse）端点**：官方 `…/reverse` 页现行正文是 APIv2 付款码支付指引 / ⇒ 归 P0-8，守卫 PT-B2 零声明锁定。

### 9. `GetInstalExposureAdviceAsync` — 服务商分期支付「曝光咨询」：查询某用户是否推荐曝光分期入口，及各档期有无息费减免券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016196820。

- **声明接口**：[IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs)
- **官方契约**：`POST /v3/pay/instal/partner/exposure`

- **请求体**：`PartnerInstalExposureRequest` — 咨询请求体，字段见 `PartnerInstalExposureRequest`。
- **返回**：`PartnerInstalExposureResponse` — 曝光建议，见 `PartnerInstalExposureResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/instal/partner/exposure`； / 必带 `Accept: application/json` 与 `Content-Type: application/json`。
- **路由前缀例外**：本端点在 `/v3/pay/instal/` 下，**不在** / `/v3/pay/partner/transactions/` 前缀内 —— 守卫 PT-B2 按「8 + 1」形态钉住。
- **频率上限 200 次/秒**（服务商商户号维度），超限须降频。
- **用户标识形态与下单族不同**：本请求的 `user_id` 是**对象**， / `user_id_type` 必填（四值见 `PartnerUserIdTypes`）， / 不是下单族 `payer` 的平铺双 openid。
- **应答不含金额与费率**：只有 `recommend_exposure` 与逐档期的 / `exist_cash_off_coupon`（档期取值见 `PartnerRepaymentTypes`）。
- **SKIT 清单里的 `/pay/partner/transactions/exposure` 不落端点**：官方页面已下架 / （302 到 50x，两次独立核验），曝光能力只由本方法承载。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PartnerTransactions`](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/) 内共 **30** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **30** 个，`SerializerClassName` = `PartnerTransactions`。

AOT 源生成上下文：[`PartnerTransactionsJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PartnerTransactionsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PartnerPrepayIdResponse` | 应答 | [PartnerTransactions/PartnerPrepayResponses.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerPrepayResponses.cs) | `prepay_id`, `code_url`, `h5_url` |
| `PartnerJsapiPrepayRequest` | 请求体 | [PartnerTransactions/PartnerJsapiPrepayRequest.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerJsapiPrepayRequest.cs) | `sp_appid`, `sp_mchid`, `sub_appid`, `sub_mchid`, `description`, `out_trade_no`, `time_expire`, `attach`, `notify_url`, `goods_tag`, `settle_info`, `amount` … |
| `PartnerNativePrepayResponse` | 应答 | [PartnerTransactions/PartnerPrepayResponses.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerPrepayResponses.cs) | `prepay_id`, `code_url`, `h5_url` |
| `PartnerNativePrepayRequest` | 请求体 | [PartnerTransactions/PartnerNativePrepayRequest.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerNativePrepayRequest.cs) | `sp_appid`, `sp_mchid`, `sub_appid`, `sub_mchid`, `description`, `out_trade_no`, `time_expire`, `attach`, `notify_url`, `goods_tag`, `settle_info`, `amount` … |
| `PartnerAppPrepayRequest` | 请求体 | [PartnerTransactions/PartnerAppPrepayRequest.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerAppPrepayRequest.cs) | `sp_appid`, `sp_mchid`, `sub_appid`, `sub_mchid`, `description`, `out_trade_no`, `time_expire`, `attach`, `notify_url`, `goods_tag`, `settle_info`, `amount` … |
| `PartnerH5PrepayResponse` | 应答 | [PartnerTransactions/PartnerPrepayResponses.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerPrepayResponses.cs) | `prepay_id`, `code_url`, `h5_url` |
| `PartnerH5PrepayRequest` | 请求体 | [PartnerTransactions/PartnerH5PrepayRequest.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerH5PrepayRequest.cs) | `sp_appid`, `sp_mchid`, `sub_appid`, `sub_mchid`, `description`, `out_trade_no`, `time_expire`, `attach`, `notify_url`, `goods_tag`, `settle_info`, `amount` … |
| `PartnerScannedPosPrepayRequest` | 请求体 | [PartnerTransactions/PartnerScannedPosPrepayRequest.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerScannedPosPrepayRequest.cs) | `sp_appid`, `sp_mchid`, `sub_appid`, `sub_mchid`, `description`, `out_trade_no`, `time_expire`, `attach`, `notify_url`, `goods_tag`, `support_fapiao`, `amount` … |
| `PartnerTransactionQueryResponse` | 应答 | [PartnerTransactions/PartnerTransactionQueryResponse.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerTransactionQueryResponse.cs) | `sp_appid`, `sp_mchid`, `sub_appid`, `sub_mchid`, `out_trade_no`, `transaction_id`, `trade_type`, `trade_state`, `trade_state_desc`, `bank_type`, `attach`, `success_time` … |
| `PartnerCloseOrderRequest` | 请求体 | [PartnerTransactions/PartnerCloseOrderRequest.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerCloseOrderRequest.cs) | `sp_mchid`, `sub_mchid` |
| `PartnerInstalExposureResponse` | 应答 | [PartnerTransactions/PartnerInstalExposureDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerInstalExposureDtos.cs) | `sp_mchid`, `sp_appid`, `sub_mchid`, `sub_appid`, `user_id`, `user_id_type`, `sp_openid`, `sub_openid`, `recommend_exposure`, `repayment_info`, `repayment_type`, `exist_cash_off_coupon` |
| `PartnerInstalExposureRequest` | 请求体 | [PartnerTransactions/PartnerInstalExposureDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/PartnerInstalExposureDtos.cs) | `sp_mchid`, `sp_appid`, `sub_mchid`, `sub_appid`, `user_id`, `user_id_type`, `sp_openid`, `sub_openid`, `recommend_exposure`, `repayment_info`, `repayment_type`, `exist_cash_off_coupon` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PartnerTransactions;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PartnerTransactions));

var service = app.Services.GetRequiredService<IWechatPayPartnerTransactionsService>();
var ct = CancellationToken.None;
var response = await service.CreateAppOrderAsync(new PartnerAppPrepayRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/pay/partner/transactions/app");
// 占位值仅用于编译示意；业务值须按 IWechatPayPartnerTransactionsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateAppOrderAsync`）；请求体 `PartnerAppPrepayRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPartnerTransactionsService](./IWechatPayPartnerTransactionsService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPartnerTransactionsContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransactionsContractGuards.cs)、[`WechatPayBankPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[`WechatPayCateringContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCateringContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PartnerTransactionsEndpoints_ShouldMatchOfficialRoutes`、`PartnerTransactionsRoutes_ShouldStayOnPartnerPrefixAndNotDeclareV2OrDelistedEndpoints`、`PartnerTransactionsDtos_ShouldExposeOfficialFieldNames`、`PartnerOrderFamilies_ShouldKeepTheirOfficialShapeDifferencesInTypes`、`PartnerTransactionEnumConstants_ShouldMatchOfficialValues`、`PartnerTransactionsResponses_ShouldDeriveFromWechatPayResponseAndMatchOfficialBodies`、`PartnerTransactionsDataModels_ShouldBeRegisteredInJsonContext`、`BankPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankPayScoreRoutes_ShouldStayOnSingleAcquiringBankPrefixAndNotDeclareDelistedOrForeignEndpoints`、`BankPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings`、`BankPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromNeighbouringDomains`、`BankPayScore_ShouldNotDeclareDomainSpecificErrorCodes` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PartnerTransactions`](../../../Mud.Wechat.Pay.DataModels/PartnerTransactions/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPartnerTransactionsContractGuards.cs`、`WechatPayBankPayScoreContractGuards.cs`、`WechatPayCateringContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
