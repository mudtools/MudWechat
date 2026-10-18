# PartnerPayScore 域 · 服务商支付分

微信支付「服务商支付分」域 SDK（**23 端点**：授权 7 + 服务订单 9 + 签约计划 7）。 接口：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)；注册组 `PartnerPayScore`；模块枚举 `PayModule.PartnerPayScore`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPartnerPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerPayScoreContractGuards.cs)、[WechatPayBankPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PartnerPayScore`](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPartnerPayScoreService = 23；按 HTTP 特性实计 **23** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPartnerPayScoreService` | `PreAuthorizeAsync` | POST | `/v3/payscore/partner/permissions` | 商户证书 / APIv3 签名 | 服务商支付分「商户预授权」：以商户侧生成的授权协议号换取 `apply_permissions_token`，<br>供拉起支付分小程序授权页。<br>官方文档：https://pay.wei… |
| 2 | `IWechatPayPartnerPayScoreService` | `PreAuthorizeForSceneAsync` | POST | `/v3/payscore/partner/permissions/apply-for-scene` | 商户证书 / APIv3 签名 | 服务商支付分「开启场景服务权限」（交易场景预授权），换取场景级<br>`transaction_permissions_token`。<br>官方文档：https://pay.weixin.q… |
| 3 | `IWechatPayPartnerPayScoreService` | `AssessUserRiskLevelAsync` | POST | `/v3/payscore/partner/permissions/assess-user-risk-level` | 商户证书 / APIv3 签名 | 服务商支付分「评估用户分层」（先用后付风控评估）：返回用户分层与建议先享额度。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012… |
| 4 | `IWechatPayPartnerPayScoreService` | `QueryAuthorizationRecordAsync` | GET | `/v3/payscore/partner/permissions/authorization-code/{authorizationCode}` | 商户证书 / APIv3 签名 | 服务商支付分「查询用户授权记录（授权协议号）」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692687。 |
| 5 | `IWechatPayPartnerPayScoreService` | `QueryAuthorizationRecordByOpenIdAsync` | GET | `/v3/payscore/partner/permissions/search` | 商户证书 / APIv3 签名 | 服务商支付分「查询用户授权记录（OpenID）」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692798。 |
| 6 | `IWechatPayPartnerPayScoreService` | `TerminateAuthorizationByCodeAsync` | POST | `/v3/payscore/partner/permissions/authorization-code/{authorizationCode}/terminate` | 商户证书 / APIv3 签名 | 服务商支付分「解除用户授权关系（授权协议号）」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012382049。 |
| 7 | `IWechatPayPartnerPayScoreService` | `TerminateAuthorizationByOpenIdAsync` | POST | `/v3/payscore/partner/permissions/terminate` | 商户证书 / APIv3 签名 | 服务商支付分「解除用户授权关系（OpenID）」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461154。 |
| 8 | `IWechatPayPartnerPayScoreService` | `CreateServiceOrderAsync` | POST | `/v3/payscore/partner/serviceorder` | 商户证书 / APIv3 签名 | 服务商创建支付分订单（免确认模式），返回订单与 `order_id`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40126921… |
| 9 | `IWechatPayPartnerPayScoreService` | `QueryServiceOrderAsync` | GET | `/v3/payscore/partner/serviceorder` | 商户证书 / APIv3 签名 | 服务商查询支付分订单（含收款信息与代金券优惠明细）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692256。 |
| 10 | `IWechatPayPartnerPayScoreService` | `CancelServiceOrderAsync` | POST | `/v3/payscore/partner/serviceorder/{outOrderNo}/cancel` | 商户证书 / APIv3 签名 | 服务商取消支付分订单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458758。 |
| 11 | `IWechatPayPartnerPayScoreService` | `CompleteServiceOrderAsync` | POST | `/v3/payscore/partner/serviceorder/{outOrderNo}/complete` | 商户证书 / APIv3 签名 | 服务商完结支付分订单（确认最终收款金额）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692408。 |
| 12 | `IWechatPayPartnerPayScoreService` | `ModifyServiceOrderAsync` | POST | `/v3/payscore/partner/serviceorder/{outOrderNo}/modify` | 商户证书 / APIv3 签名 | 服务商修改支付分订单金额。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692360。 |
| 13 | `IWechatPayPartnerPayScoreService` | `SendPendingMessageAsync` | POST | `/v3/payscore/partner/serviceorder/{outOrderNo}/send-pendingmessage` | 商户证书 / APIv3 签名 | 服务商「下发服务费用待处理消息」：向用户推送待处理服务费用提醒。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013394914。 |
| 14 | `IWechatPayPartnerPayScoreService` | `SendPrepaidMessageAsync` | POST | `/v3/payscore/partner/serviceorder/{outOrderNo}/send-prepaidmessage` | 商户证书 / APIv3 签名 | 服务商「下发服务预扣费消息」：向用户推送预扣费提醒。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013394929。 |
| 15 | `IWechatPayPartnerPayScoreService` | `SyncServiceOrderAsync` | POST | `/v3/payscore/partner/serviceorder/{outOrderNo}/sync` | 商户证书 / APIv3 签名 | 服务商同步服务订单状态：把「用户已在商户自有渠道付款」这一事实同步给支付分。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40124… |
| 16 | `IWechatPayPartnerPayScoreService` | `GetServiceQuotasAsync` | GET | `/v3/payscore/partner/servicequotas/service-id/{serviceId}` | 商户证书 / APIv3 签名 | 服务商查询「用户分层对应建议先享金额」字典。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013394486。 |
| 17 | `IWechatPayPartnerPayScoreService` | `CreatePayScorePlanAsync` | POST | `/v3/payscore/plan/partner/payscore-plans` | 商户证书 / APIv3 签名 | 服务商创建支付分计划（多次扣费的计划模板）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463290。 |
| 18 | `IWechatPayPartnerPayScoreService` | `QueryPayScorePlanAsync` | GET | `/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}` | 商户证书 / APIv3 签名 | 服务商按商户侧计划号查询支付分计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463318。 |
| 19 | `IWechatPayPartnerPayScoreService` | `StopPayScorePlanAsync` | POST | `/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}/stop` | 商户证书 / APIv3 签名 | 服务商终止支付分计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463322。 |
| 20 | `IWechatPayPartnerPayScoreService` | `CreateSignPlanServiceOrderAsync` | POST | `/v3/payscore/sign-plan/partner/serviceorder` | 商户证书 / APIv3 签名 | 服务商「基于签约计划创建支付分服务订单」（计划履约的一次扣费订单）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401246346… |
| 21 | `IWechatPayPartnerPayScoreService` | `CreateUserSignPlanAsync` | POST | `/v3/payscore/sign-plan/partner/user-sign-plans` | 商户证书 / APIv3 签名 | 服务商创建用户签约计划，返回签约计划与拉起签约页所需的 `package`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40124… |
| 22 | `IWechatPayPartnerPayScoreService` | `QueryUserSignPlanAsync` | GET | `/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}` | 商户证书 / APIv3 签名 | 服务商按商户侧签约计划号查询用户签约计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463526。 |
| 23 | `IWechatPayPartnerPayScoreService` | `StopUserSignPlanAsync` | POST | `/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}/stop` | 商户证书 / APIv3 签名 | 服务商解除用户签约计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463534。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（服务商文档中心 · 微信支付分（免确认模式），2026-10-14 逐页核验）：
商户预授权 https://pay.weixin.qq.com/doc/v3/partner/4012382030、
开启场景服务权限 https://pay.weixin.qq.com/doc/v3/partner/4012778802、
评估用户分层 https://pay.weixin.qq.com/doc/v3/partner/4012783585、
查询授权记录（协议号）https://pay.weixin.qq.com/doc/v3/partner/4012692687、
查询授权记录（OpenID）https://pay.weixin.qq.com/doc/v3/partner/4012692798、
解除授权（协议号）https://pay.weixin.qq.com/doc/v3/partner/4012382049、
解除授权（OpenID）https://pay.weixin.qq.com/doc/v3/partner/4012461154、
创建订单 https://pay.weixin.qq.com/doc/v3/partner/4012692136、
查询订单 https://pay.weixin.qq.com/doc/v3/partner/4012692256、
取消订单 https://pay.weixin.qq.com/doc/v3/partner/4012458758、
完结订单 https://pay.weixin.qq.com/doc/v3/partner/4012692408、
修改订单金额 https://pay.weixin.qq.com/doc/v3/partner/4012692360、
下发服务费用待处理消息 https://pay.weixin.qq.com/doc/v3/partner/4013394914、
下发服务预扣费消息 https://pay.weixin.qq.com/doc/v3/partner/4013394929、
同步服务订单状态 https://pay.weixin.qq.com/doc/v3/partner/4012458816、
查询建议先享金额 https://pay.weixin.qq.com/doc/v3/partner/4013394486、
创建支付分计划 https://pay.weixin.qq.com/doc/v3/partner/4012463290、
查询支付分计划 https://pay.weixin.qq.com/doc/v3/partner/4012463318、
终止支付分计划 https://pay.weixin.qq.com/doc/v3/partner/4012463322、
基于签约计划创建服务订单 https://pay.weixin.qq.com/doc/v3/partner/4012463469、
创建用户签约计划 https://pay.weixin.qq.com/doc/v3/partner/4012463521、
查询用户签约计划 https://pay.weixin.qq.com/doc/v3/partner/4012463526、
解除用户签约计划 https://pay.weixin.qq.com/doc/v3/partner/4012463534。

**⚠️ 本域 AppID/商户号形态与服务商基础支付族相反**（守卫 PS-B3 锁定）：
本域用 `appid`（服务商）+ `sub_appid`（子商户），应答里服务商商户号叫 `mchid` ——
**全域不存在 `sp_appid`/`sp_mchid`**。跨域抄 `PartnerJsapiPrepayRequest` 会静默错参。

**三族路由前缀并存**（守卫 PS-B2 锁定）：授权与服务订单族在 `/v3/payscore/partner/…`、
支付分计划在 `/v3/payscore/plan/partner/…`、用户签约在 `/v3/payscore/sign-plan/partner/…` ——
同一业务域内三套前缀，**不能**按基础支付族的 `/v3/pay/partner/` 直觉拼接。

**`sub_mchid` 的两处位置**（守卫 PS-B1 逐端点锁定）：多数在 body；
六个 GET（授权记录查 ×2、查询订单、查询建议先享金额、查询计划、查询签约计划）与「评估用户分层」
在 **query**（官方 curl 原文 `…?sub_mchid=`）。签名方法参数须逐端点核对，不得按一个模板套全族。

**官方无「商户收款」端点**：SKIT 清单里的 `POST /v3/payscore/partner/serviceorder/{out_order_no}/pay`
出处是**已下架旧 wiki 页**（现 302 跳到无关页），现行服务商支付分 API 列表侧栏 12 项不含该能力 ⇒
本域**零端点**；亦**不得**按直连面 `POST /v3/payscore/serviceorder/{out_order_no}/pay` 补进来
（守卫 PS-B2 以零声明断言钉住，并须落位相邻路由作正对照）。

**两套时间格式并存**（守卫 PS-B3 锁定）：`time_range` 与 `collection.details[].paid_time`
用紧凑格式（`yyyyMMddHHmmss`/`yyyyMMdd`）、`prepaid_time` 只有 8 位，
而 `complete_time`/`stop_time`/`plan_over_time`/`cancel_sign_time` 及签约明细的三个时间走 rfc3339。
故 DTO 里时间字段一律保持 `string`，不经 `DateTimeOffset` 往返。

**本域业务错误码不建专有常量类**（守卫 PS-B5 记录裁决）：官方各页业务码全为通用名
（`PARAM_ERROR`/`INVALID_REQUEST`/`INVALID_ORDER_STATE`/`ORDER_NOT_EXIST`/`NOAUTH`或`NO_AUTH`/`RULE_LIMIT`/`FREQUENCY_LIMITED`/`SYSTEM_ERROR`），
且「无授权资格」一支**同域两套拼写并存**（2026-10-14 逐页核验）：授权族五页与服务额度页写
`NO_AUTH`（有下划线），计划族与用户签约族 7 页写 `NOAUTH`（无下划线），而服务订单族多数页面只有公共四码 ⇒ 任何「统一拼写」的常量表都会在一半端点上失真，
错误面统一由 `Code` 原样透传与既有 `WechatPayErrorCodes` 承载。

**无 `[Token]`、走商户签名**（守卫 `PAY-B1` fail-closed）：APIv3 全程以商户 API 证书做
RSA-SHA256 请求签名，签名主体是**服务商商户号**，子商户号只出现在报文里 —— 服务商**不得**用子商户证书调本域。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`），不复用默认实例，否则与企微线撞 BaseUrl。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code`/`Message`，
由 `WechatPayException.ThrowIfFailed` 判错；否则组件抛 `ApiException` 把官方业务码丢掉。

**不做运行时多态**：每端点一个方法 + 各自请求 DTO；八支 **204 无包体**端点返回裸
`Task`（守卫 PS-B1 计数锁定），不为其伪造响应类型。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `PreAuthorizeAsync` — 服务商支付分「商户预授权」：以商户侧生成的授权协议号换取 `apply_permissions_token`，
供拉起支付分小程序授权页。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012382030。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/permissions`

- **请求体**：`PartnerPayScorePermissionsRequest` — 请求体，字段见 `PartnerPayScorePermissionsRequest`。
- **返回**：`PartnerPayScorePermissionsResponse` — 仅含 `apply_permissions_token` 的应答，见 `PartnerPayScorePermissionsResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/permissions`；无 path / query 参数。
- **`authorization_code` 是商户侧生成的授权单号**：32 字符内、仅数字大小写字母与 `_-|*`、 / 同一**服务商商户号**下唯一，官方明令「多个用户授权不能重复使用同一授权协议号」。
- **`token` 时效 1 小时**（官方原文），过期须重新调用本接口，不得缓存复用。
- **`notify_url` 在本页是选填且 string(255)**：与创单页 `notify_url`（必填、256）不同，照录各自页。

### 2. `PreAuthorizeForSceneAsync` — 服务商支付分「开启场景服务权限」（交易场景预授权），换取场景级
`transaction_permissions_token`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012778802。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/permissions/apply-for-scene`

- **请求体**：`PartnerPayScoreScenePermissionsRequest` — 请求体，字段见 `PartnerPayScoreScenePermissionsRequest`。
- **返回**：`PartnerPayScoreScenePermissionsResponse` — 回显 `scene` 与 `transaction_permissions_token`，见 `PartnerPayScoreScenePermissionsResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/permissions/apply-for-scene`。
- **`scene` 当前官方只开放 `Transaction`**， / 且该场景要求填 `transaction_scene_info`（内含 `mchid` + `sub_mchid`）⇒ 事实上必填。
- **与 `PreAuthorizeAsync` 的差异**：本端点应答是**场景令牌**而非 / `apply_permissions_token`，两者用途不同，不可互换传给拉起授权页的参数。

### 3. `AssessUserRiskLevelAsync` — 服务商支付分「评估用户分层」（先用后付风控评估）：返回用户分层与建议先享额度。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012783585。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/permissions/assess-user-risk-level`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 **query** `sub_mchid`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreRiskAssessRequest` — 请求体，字段见 `PartnerPayScoreRiskAssessRequest`。
- **返回**：`PartnerPayScoreRiskAssessResponse` — 授权记录字段的全集再加风险分层四项，见 `PartnerPayScoreRiskAssessResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/permissions/assess-user-risk-level?sub_mchid=…`； / **⚠️ 本端点最易漏的一处**：`sub_mchid` **不在请求体里**，官方 curl 原文把它放在 query ⇒ / 放进 body 会得到 `PARAM_ERROR`。
- **`authorization_code` / `openid` / `sub_openid` 三选一**（官方原文）， / 比创单族的「双 openid 二选一」多一个入口。
- **商品信息是硬约束**：`shopping_info.goods` 一旦上送则各商品 / `name`/`picture`/`amount`/`count` 全必填，且官方注明 / 「所有商品单价 × 数量之和**不能超过服务风险金**」；`category_id` 最多 3 个且仅支持整数型品类 ID。

### 4. `QueryAuthorizationRecordAsync` — 服务商支付分「查询用户授权记录（授权协议号）」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692687。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/partner/permissions/authorization-code/{authorizationCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizationCode` | Path | `string` | 授权协议号（官方 path `authorization_code`，必填 string(32)）。 |
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
- **返回**：`PartnerPayScoreAuthorizationRecordResponse` — 授权记录，见 `PartnerPayScoreAuthorizationRecordResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/partner/permissions/authorization-code/{authorization_code}`， / `service_id` 与 `sub_mchid` 走 **query**。
- **`authorization_state` 本页列 3 值**：含 `UnbindUser`， / 而 OpenID 页只列 2 值 —— 同一字段名两套值集（守卫 PS-B4 锁定）。

### 5. `QueryAuthorizationRecordByOpenIdAsync` — 服务商支付分「查询用户授权记录（OpenID）」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692798。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/partner/permissions/search`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
| `appId` | Query `appid` | `string` | 服务商 AppID（官方 query `appid`，必填）。 |
| `openId` | Query `openid` | `string?` | 用户在服务商 AppID 下的标识（官方 query `openid`，选填）。 |
| `subAppId` | Query `sub_appid` | `string?` | 子商户 AppID（官方 query `sub_appid`，选填）。 |
| `subOpenId` | Query `sub_openid` | `string?` | 用户在子商户 AppID 下的标识（官方 query `sub_openid`，选填）。 |
- **返回**：`PartnerPayScoreAuthorizationRecordResponse` — 授权记录（与协议号页**共用**同一类型），见 `PartnerPayScoreAuthorizationRecordResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/partner/permissions/search`，六个参数**全部走 query**。
- **与直连域的根本差异**：直连侧按 OpenID 查授权是 / `GET /v3/payscore/serviceorder/permissions/openid/{openid}`（path 形态）⇒ / 本域是 `/permissions/search` + query，**不得**照直连改路由。
- **本端点无 body**：全部入参在 query，故签名不接请求 DTO。

### 6. `TerminateAuthorizationByCodeAsync` — 服务商支付分「解除用户授权关系（授权协议号）」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012382049。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/permissions/authorization-code/{authorizationCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizationCode` | Path | `string` | 授权协议号（官方 path `authorization_code`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreTerminateByCodeRequest` — 请求体（`service_id` + `sub_mchid` + `reason`），见 `PartnerPayScoreTerminateByCodeRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/permissions/authorization-code/{authorization_code}/terminate`； / 成功 `204` ⇒ 本方法返回裸 `Task`。
- **影响面**：解除授权后该用户在本服务下不可再免确认创单；官方另有 / 「用户解除服务授权时连带取消服务下已签约协议」（`RevokeService`） / ⇒ 解除授权须评估对进行中签约计划的影响。

### 7. `TerminateAuthorizationByOpenIdAsync` — 服务商支付分「解除用户授权关系（OpenID）」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461154。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/permissions/terminate`

- **请求体**：`PartnerPayScoreTerminateByOpenIdRequest` — 请求体，字段见 `PartnerPayScoreTerminateByOpenIdRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/permissions/terminate`（**无 path 参数**， / 用户标识在 body 里）；成功 `204`。
- **与协议号页的字段差异**：本请求体多 `appid`(必) / `sub_appid`(选) / / `openid` / `sub_openid`（二选一），而协议号页只有三字段 ⇒ 两个请求类型**不共用**。

### 8. `CreateServiceOrderAsync` — 服务商创建支付分订单（免确认模式），返回订单与 `order_id`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692136。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder`

- **请求体**：`PartnerPayScoreCreateOrderRequest` — 下单请求体，字段见 `PartnerPayScoreCreateOrderRequest`。
- **返回**：`PartnerPayScoreCreateOrderResponse` — 订单表 + `package`（本域可忽略），见 `PartnerPayScoreCreateOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder`；无 path / query 参数。
- **幂等**：官方原文「支持原参重入，相同参数重复调用可以返回成功」⇒ 可安全重试； / 重入成功不等于业务只发生一次，落库仍以 `out_order_no` 为幂等键。
- **`need_user_confirm` 固定 `false`**；`risk_fund` 与 `time_range` **必填**， / 且 `risk_fund.name` 官方要求固定 `EstimateOrderCost`。
- **红线**：`out_order_no` **不可**用作申请退款接口的 `out_trade_no`（官方原文）—— / 退款须用查询应答 `collection.details[].transaction_id`。
- **业务限制**：`appid`/`sub_appid` 须与对应商户号绑定； / 用户达周期内创单上限时官方返回 `403 RULE_LIMIT`（须用户主动在支付分「服务管理页」去限制或次日再下单）。

### 9. `QueryServiceOrderAsync` — 服务商查询支付分订单（含收款信息与代金券优惠明细）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692256。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/partner/serviceorder`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
| `outOrderNo` | Query `out_order_no` | `string` | 商户服务订单号（官方 query `out_order_no`，必填）。 |
| `serviceId` | Query `service_id` | `string?` | 服务 ID（官方 query `service_id`，**选填**）。 |
- **返回**：`PartnerPayScoreQueryOrderResponse` — 订单全集，见 `PartnerPayScoreQueryOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/partner/serviceorder`，三参数全走 query； / 官方必填组合是 `sub_mchid` + `out_order_no`，而 `service_id` 在请求侧为**选填** / （应答侧为必填）。
- **与创建应答形态不同**：本应答**无** `package`、有 `collection`/ / `promotion_detail`/`total_amount`/`openid`/`sub_openid` ⇒ 分建两类型。
- **状态机**：官方原文要求**参考「支付分订单状态流转图」**处理， / `state` 须结合 `collection.state` 与 `state_description` 判断； / `collection` 仅在调完结后返回，完结金额为 0 时不返回。

### 10. `CancelServiceOrderAsync` — 服务商取消支付分订单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458758。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder/{outOrderNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreCancelOrderRequest` — 请求体（`service_id` 选填 / `sub_mchid` 必填 / `reason` 必填），见 `PartnerPayScoreCancelOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder/{out_order_no}/cancel`；成功 `204`。
- **`service_id` 在本页是选填**（创单/完结页必填）—— 三页字段表各不同，照录本页。
- **`reason` 用户可见**：官方注明取消订单的消息通知会展示该字段，长度上限 50。

### 11. `CompleteServiceOrderAsync` — 服务商完结支付分订单（确认最终收款金额）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692408。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder/{outOrderNo}/complete`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreCompleteOrderRequest` — 请求体，字段见 `PartnerPayScoreCompleteOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder/{out_order_no}/complete`；成功 `204` / —— **与直连域完结接口返回订单体不同**，本域完结后须另调查单确认状态。
- **金额校验式**：`total_amount` = `post_payments.amount` 之和 − `post_discounts.amount` 之和， / 且 ≤ 服务 ID 风险金额上限（官方注明该上限须与 BD 确认）。
- **时间约束**：`complete_time`（rfc3339）须晚于或等于 `end_time`、且早于或等于调接口当前时间； / 本域 `time_range` 走紧凑格式，两处格式不同勿统一。
- **分账标记**：`profit_sharing=true` 时收款资金**冻结**待请求分账， / 未分账且距支付完成超 30 天自动解冻 —— 属分账域编排，本方法只负责上送标记。

### 12. `ModifyServiceOrderAsync` — 服务商修改支付分订单金额。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692360。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder/{outOrderNo}/modify`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreModifyOrderRequest` — 请求体，字段见 `PartnerPayScoreModifyOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder/{out_order_no}/modify`；成功 `204`。
- **与完结页的字段差异**：本页 `service_id` **选填**、新增必填 `reason`、 / 无 `time_range`/`location`/`profit_sharing`/`complete_time`/`goods_tag` ⇒ 两请求类型不共用。
- **金额上限与校验式同完结页**（官方同页原文），且修改后的总额仍受服务 ID 风险金额上限约束。

### 13. `SendPendingMessageAsync` — 服务商「下发服务费用待处理消息」：向用户推送待处理服务费用提醒。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013394914。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder/{outOrderNo}/send-pendingmessage`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreMessageRequest` — 请求体（与 `SendPrepaidMessageAsync` **共用**），见 `PartnerPayScoreMessageRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder/{out_order_no}/send-pendingmessage`；成功 `204`。
- **两支消息端点请求表逐项一致** ⇒ 共用 `PartnerPayScoreMessageRequest`（守卫 PS-B6 锁定）， / 差异只在路由与消息语义。
- **`prepaid_time` 只有 8 位（`YYYYMMDD`）且须 ≥ 调接口时间 + 7 天**： / 官方原文示例「2023-12-01 调用可传 20231208」——按本域第三种时间形态处理，勿写 rfc3339。

### 14. `SendPrepaidMessageAsync` — 服务商「下发服务预扣费消息」：向用户推送预扣费提醒。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013394929。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder/{outOrderNo}/send-prepaidmessage`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreMessageRequest` — 请求体（与 `SendPendingMessageAsync` **共用**），见 `PartnerPayScoreMessageRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder/{out_order_no}/send-prepaidmessage`；成功 `204`。
- **两支消息端点各自独立**：官方把它们列为 API 列表的两项（待处理 / 预扣费）， / 请求表一致但**不可**合并成一个方法带「消息类型参数」运行期分派（不做运行时多态）。

### 15. `SyncServiceOrderAsync` — 服务商同步服务订单状态：把「用户已在商户自有渠道付款」这一事实同步给支付分。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458816。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/partner/serviceorder/{outOrderNo}/sync`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreSyncOrderRequest` — 请求体，字段见 `PartnerPayScoreSyncOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/partner/serviceorder/{out_order_no}/sync`；成功 `204`。
- **`type` 官方固定值**：`OrderPaid`（`Order_Paid`， / 大小写照原文），且该值存在时 `detail` 必填。
- **60 秒窗口**：官方要求「调完结接口时间 < `paid_time` ≤ 调同步接口时间 + 60 秒」⇒ / 同步必须在完结之后、且实付时间如实填写。
- **副作用**：微信支付分据本接口更新用户侧**守约记录 / 负面记录**， / 虚报实付时间会造成客诉；成功后查单的 `collection.details[].paid_type` 返回 / `Merchant`。

### 16. `GetServiceQuotasAsync` — 服务商查询「用户分层对应建议先享金额」字典。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013394486。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/partner/servicequotas/service-id/{serviceId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `serviceId` | Path | `string` | 服务 ID（官方 path `service_id`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
| `appId` | Query `appid` | `string` | 服务商 AppID（官方 query `appid`，必填）。 |
| `riskLevelVersion` | Query `risk_level_version` | `int` | 分层版本（官方 query `risk_level_version`，必填整型）。 |
| `subAppId` | Query `sub_appid` | `string?` | 子商户 AppID（官方 query `sub_appid`，选填）。 |
- **返回**：`PartnerPayScoreServiceQuotaResponse` — 分层 → 建议先享金额列表，见 `PartnerPayScoreServiceQuotaResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/partner/servicequotas/service-id/{service_id}`： / 本域**唯一 path + query 混合**的 GET，且 `servicequotas` 段不属于 / `serviceorder` 资源族。
- **`risk_level_version` 必填整型**：该版本来自 / `AssessUserRiskLevelAsync` 应答的 `risk_level_version`，须成对使用； / 分层与金额都是**整型**，不按字符串枚举处理。
- **本端点是字典式查询**：返回的是「该服务 + 该版本下」的分层额度表，与具体用户无关 ⇒ / 不落库为订单数据，宿主可缓存。

### 17. `CreatePayScorePlanAsync` — 服务商创建支付分计划（多次扣费的计划模板）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463290。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/plan/partner/payscore-plans`

- **请求体**：`PartnerPayScoreCreatePlanRequest` — 请求体，字段见 `PartnerPayScoreCreatePlanRequest`。
- **返回**：`PartnerPayScorePlanResponse` — 计划表（查询 / 终止计划**共用**同一应答类型），见 `PartnerPayScorePlanResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/plan/partner/payscore-plans`； / 注意前缀是 `/v3/payscore/plan/partner/`，**不在**服务订单族的 `/v3/payscore/partner/` 下。
- **官方四条一致性校验**（400 直接拒）：`deduction_quantity` 须等于明细条数； / 明细实付金额须**依序不递增**；每条 `original_price` ≥ `actual_price`； / 两个总额须与明细对应金额之和一致且总实付不得为 0 ⇒ 上送前应在宿主侧自查。
- **明细序号由微信侧生成**：官方按列表顺序从 1 编号，商户不得自报 `plan_detail_no`， / 且使用时须**依序**。
- **后续定位用 `merchant_plan_no`**：查询与终止计划的 path 都用商户侧计划号，而非 `plan_id`。

### 18. `QueryPayScorePlanAsync` — 服务商按商户侧计划号查询支付分计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463318。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `merchantPlanNo` | Path | `string` | 商户侧计划号（官方 path `merchant_plan_no`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
- **返回**：`PartnerPayScorePlanResponse` — 计划表，见 `PartnerPayScorePlanResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchant_plan_no}?sub_mchid=…`。
- **path 段是 `merchant-plan-no` 的字面量前缀**：照官方原文，不得简写成 `plan-no`。
- **应答表与创建 / 终止两页逐项一致** ⇒ 三支端点共用 `PartnerPayScorePlanResponse`（守卫 PS-B6）。

### 19. `StopPayScorePlanAsync` — 服务商终止支付分计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463322。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}/stop`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `merchantPlanNo` | Path | `string` | 商户侧计划号（官方 path `merchant_plan_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreStopPlanRequest` — 请求体（**仅 `sub_mchid` 一个字段**），见 `PartnerPayScoreStopPlanRequest`。
- **返回**：`PartnerPayScorePlanResponse` — 终止后的计划表（含 `stop_mchid`/`stop_time`），见 `PartnerPayScorePlanResponse`。

**备注（原文）**

- **官方契约**：**POST** / `/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchant_plan_no}/stop`； / 本端点**有应答体**（200，计划表），与服务订单族的八支 204 端点形态相反。
- **请求体没有终止原因**：官方本页 body 只有 `sub_mchid`； / 带 `stop_reason` 的是「解除用户签约计划」⇒ 勿据那一页补字段。
- **终止语义**：计划转 `Stop` 后不可新签约， / 但**已签约的计划仍可继续完成服务**（官方原文）。

### 20. `CreateSignPlanServiceOrderAsync` — 服务商「基于签约计划创建支付分服务订单」（计划履约的一次扣费订单）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463469。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/sign-plan/partner/serviceorder`

- **请求体**：`PartnerPayScoreSignPlanServiceOrderRequest` — 请求体，字段见 `PartnerPayScoreSignPlanServiceOrderRequest`。
- **返回**：`PartnerPayScoreSignPlanServiceOrderResponse` — 订单表子集（金额派生自签约计划详情），见 `PartnerPayScoreSignPlanServiceOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/sign-plan/partner/serviceorder`； / 前缀是 `/v3/payscore/sign-plan/partner/`（第三套前缀），且路由段是 `serviceorder` 单数 / 而**不带** `/{out_order_no}` —— 订单号由 body 的 `out_trade_no` 提供。
- **请求字段名是 `out_trade_no`、应答字段名回落成 `out_order_no`**：官方两表拼写不同，照录。
- **`out_trade_no` 必须等于该明细在创建签约计划时上送的 `merchant_plan_detail_no`**（官方原文）。
- **履约串行约束**：同时只能有一个签约计划存在服务中的订单，且不允许跳序号 —— / 创建下一条明细的订单前须先取消上一订单，或完结并等支付成功。
- **不重复上送金额**：本请求无 `post_payments`/`risk_fund`/`total_amount`， / 全部金额由订单对应的签约计划详情派生；**本域无「计划收款 / 商户收款」端点**（设计方案 §2.4 零端点裁决）。

### 21. `CreateUserSignPlanAsync` — 服务商创建用户签约计划，返回签约计划与拉起签约页所需的 `package`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463521。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/sign-plan/partner/user-sign-plans`

- **请求体**：`PartnerPayScoreCreateUserSignPlanRequest` — 请求体，字段见 `PartnerPayScoreCreateUserSignPlanRequest`。
- **返回**：`PartnerPayScoreCreateUserSignPlanResponse` — **包装形态** `{sign_plan, package}`，见 `PartnerPayScoreCreateUserSignPlanResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/sign-plan/partner/user-sign-plans`（复数资源）。
- **本支应答是三支签约端点中唯一的包装形态**：查询与解除把签约计划字段**平铺**在应答根 / （返回 `PartnerPayScoreSignPlan`）⇒ 两形态分建，勿合并。
- **`package` 必填且须 URLEncode 后使用**：官方注明它是「跳转微信侧确认签约小程序的数据」—— / 与服务订单族那个可忽略的 `package` 语义相反，不得丢弃。
- **`sign_plan_detail` 是「明细序号 + 商户侧单号」配对列表**（必填）： / 该 `merchant_plan_detail_no` 后续即该明细服务订单的 `out_trade_no`，两处必须一致。
- **`notify_url` 本页上限 string(1024)**（签约成功事件通知），比服务订单族的 256 宽得多，照录本页。

### 22. `QueryUserSignPlanAsync` — 服务商按商户侧签约计划号查询用户签约计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463526。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `merchantSignPlanNo` | Path | `string` | 商户侧签约计划号（官方 path `merchant_sign_plan_no`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填：须是该签约计划的**创建方**子商户）。 |
- **返回**：`PartnerPayScoreSignPlan` — 签约计划（平铺形态），见 `PartnerPayScoreSignPlan`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}?sub_mchid=…`。
- **应答表与「解除用户签约计划」页逐项一致** ⇒ 两支共用 `PartnerPayScoreSignPlan`； / 该类型同时承载创建应答里的嵌套 `sign_plan`（守卫 PS-B6 锁定三处共用）。
- **读侧判据**：`sign_state`（五值）+ `going_detail_no`（`0` 表示尚未使用任何明细） / + `signed_detail_list[].plan_detail_state`（四值）三层共同决定能否继续为该签约计划创建下一笔订单。
- **金额字段拼写**：本表用 `total_origin_price`，计划表用 `total_original_price` —— 官方两套拼写，勿统一。

### 23. `StopUserSignPlanAsync` — 服务商解除用户签约计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012463534。

- **声明接口**：[IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}/stop`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `merchantSignPlanNo` | Path | `string` | 商户侧签约计划号（官方 path `merchant_sign_plan_no`，必填 string(32)）。 |

- **请求体**：`PartnerPayScoreStopUserSignPlanRequest` — 请求体（`sub_mchid` + `stop_reason`），见 `PartnerPayScoreStopUserSignPlanRequest`。
- **返回**：`PartnerPayScoreSignPlan` — 解除后的签约计划（平铺形态），见 `PartnerPayScoreSignPlan`。

**备注（原文）**

- **官方契约**：**POST** / `/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}/stop`； / **本支有 200 应答体**（与计划族的 stop 同形态）。
- **与「终止支付分计划」的两处差异**：本请求体多必填 `stop_reason`(128)， / 且 `sub_mchid` 须是**该签约计划的创建方**子商户 ⇒ 两请求类型不共用。
- **解除后应答**：官方在 `cancel_sign_type` 返回 / `Merchant`、`sign_state` 转 / `SignPlanCancel`，并以 `cancel_sign_time`/`cancel_reason` 留痕。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PartnerPayScore`](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/) 内共 **49** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **49** 个，`SerializerClassName` = `PartnerPayScore`。

AOT 源生成上下文：[`PartnerPayScoreJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PartnerPayScoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PartnerPayScorePermissionsResponse` | 应答 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScorePermissionsRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreScenePermissionsResponse` | 应答 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreScenePermissionsRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreRiskAssessResponse` | 应答 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreRiskAssessRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreAuthorizationRecordResponse` | 应答 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreTerminateByCodeRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreTerminateByOpenIdRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePermissionsDtos.cs) | `service_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `scene`, `transaction_scene_info`, `mchid`, `transaction_permissions_token`, `openid` … |
| `PartnerPayScoreCreateOrderResponse` | 应答 | [PartnerPayScore/PartnerPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts`, `risk_fund` … |
| `PartnerPayScoreCreateOrderRequest` | 请求体 | [PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location`, `openid` … |
| `PartnerPayScoreQueryOrderResponse` | 应答 | [PartnerPayScore/PartnerPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts`, `risk_fund` … |
| `PartnerPayScoreCancelOrderRequest` | 请求体 | [PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location`, `openid` … |
| `PartnerPayScoreCompleteOrderRequest` | 请求体 | [PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location`, `openid` … |
| `PartnerPayScoreModifyOrderRequest` | 请求体 | [PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location`, `openid` … |
| `PartnerPayScoreMessageRequest` | 请求体 | [PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location`, `openid` … |
| `PartnerPayScoreSyncOrderRequest` | 请求体 | [PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location`, `openid` … |
| `PartnerPayScoreServiceQuotaResponse` | 应答 | [PartnerPayScore/PartnerPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts`, `risk_fund` … |
| `PartnerPayScorePlanResponse` | 应答 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreCreatePlanRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreStopPlanRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreSignPlanServiceOrderResponse` | 应答 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreSignPlanServiceOrderRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreCreateUserSignPlanResponse` | 应答 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreCreateUserSignPlanRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreSignPlan` | 应答 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |
| `PartnerPayScoreStopUserSignPlanRequest` | 请求体 | [PartnerPayScore/PartnerPayScorePlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/PartnerPayScorePlanDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `plan_name`, `plan_duration`, `deduction_quantity`, `total_original_price`, `total_actual_price`, `plan_detail_list`, `merchant_plan_no`, `original_price` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PartnerPayScore;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PartnerPayScore));

var service = app.Services.GetRequiredService<IWechatPayPartnerPayScoreService>();
var ct = CancellationToken.None;
var response = await service.CreatePayScorePlanAsync(new PartnerPayScoreCreatePlanRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/payscore/plan/partner/payscore-plans");
// 占位值仅用于编译示意；业务值须按 IWechatPayPartnerPayScoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreatePayScorePlanAsync`）；请求体 `PartnerPayScoreCreatePlanRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPartnerPayScoreService](./IWechatPayPartnerPayScoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPartnerPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerPayScoreContractGuards.cs)、[`WechatPayBankPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PartnerPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`PartnerPayScoreRoutes_ShouldStayOnThreePartnerPrefixesAndNotDeclareMerchantCollectPay`、`PartnerPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings`、`PartnerPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromDirectDomain`、`PartnerPayScore_ShouldNotDeclareDomainSpecificErrorCodes`、`PartnerPayScoreResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch`、`PartnerPayScoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`BankPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankPayScoreRoutes_ShouldStayOnSingleAcquiringBankPrefixAndNotDeclareDelistedOrForeignEndpoints`、`BankPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings`、`BankPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromNeighbouringDomains`、`BankPayScore_ShouldNotDeclareDomainSpecificErrorCodes` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PartnerPayScore`](../../../Mud.Wechat.Pay.DataModels/PartnerPayScore/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPartnerPayScoreContractGuards.cs`、`WechatPayBankPayScoreContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
