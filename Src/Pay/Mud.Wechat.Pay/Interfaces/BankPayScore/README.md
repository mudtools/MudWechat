# BankPayScore 域 · 银行/从业机构支付分

微信支付「银行/从业机构支付分」域 SDK（**11 端点**：授权 3 + 服务订单 6 + 扣款信息 2）。 接口：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)；注册组 `BankPayScore`；模块枚举 `PayModule.BankPayScore`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBankPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/BankPayScore`](../../../Mud.Wechat.Pay.DataModels/BankPayScore/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBankPayScoreService = 11；按 HTTP 特性实计 **11** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBankPayScoreService` | `PreAuthorizeAsync` | POST | `/v3/payscore/acquiringbank/permissions` | 商户证书 / APIv3 签名 | 从业机构支付分「商户预授权」：以商户侧生成的授权协议号换取 `apply_permissions_token`，<br>供拉起支付分小程序授权页。<br>官方文档：https://pay.we… |
| 2 | `IWechatPayBankPayScoreService` | `QueryAuthorizationRecordAsync` | GET | `/v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}` | 商户证书 / APIv3 签名 | 从业机构支付分「查询用户授权记录（授权协议号）」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461004。 |
| 3 | `IWechatPayBankPayScoreService` | `TerminateAuthorizationAsync` | POST | `/v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}/terminate` | 商户证书 / APIv3 签名 | 从业机构支付分「解除用户授权关系（授权协议号）」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461012。 |
| 4 | `IWechatPayBankPayScoreService` | `CreateServiceOrderAsync` | POST | `/v3/payscore/acquiringbank/serviceorder` | 商户证书 / APIv3 签名 | 从业机构创建支付分订单（免确认模式），返回订单与 `order_id`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691… |
| 5 | `IWechatPayBankPayScoreService` | `QueryServiceOrderAsync` | GET | `/v3/payscore/acquiringbank/serviceorder` | 商户证书 / APIv3 签名 | 从业机构查询支付分订单（含收款信息与代金券优惠明细）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691343。 |
| 6 | `IWechatPayBankPayScoreService` | `CancelServiceOrderAsync` | POST | `/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/cancel` | 商户证书 / APIv3 签名 | 从业机构取消支付分订单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458146。 |
| 7 | `IWechatPayBankPayScoreService` | `CompleteServiceOrderAsync` | POST | `/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/complete` | 商户证书 / APIv3 签名 | 从业机构完结支付分订单（用户已实际接受服务，进入待收款状态）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012460791。 |
| 8 | `IWechatPayBankPayScoreService` | `ModifyServiceOrderAsync` | POST | `/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/modify` | 商户证书 / APIv3 签名 | 从业机构修改支付分订单金额（完结后仅可下调收款金额）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691363。 |
| 9 | `IWechatPayBankPayScoreService` | `SyncServiceOrderAsync` | POST | `/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/sync` | 商户证书 / APIv3 签名 | 从业机构同步支付分订单状态（用户已走**其它渠道**付款时补登记）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401269137… |
| 10 | `IWechatPayBankPayScoreService` | `RegisterDeductInfoAsync` | POST | `/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/registerdeductinfo` | 商户证书 / APIv3 签名 | 从业机构登记扣款信息（清算机构委托代扣的前置登记），返回回显的 `description`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partn… |
| 11 | `IWechatPayBankPayScoreService` | `QueryDeductionInfoAsync` | GET | `/v3/payscore/acquiringbank/serviceorder/deduction` | 商户证书 / APIv3 签名 | 从业机构查询扣款信息（回读已登记的 `description` 与 `out_trade_no`）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/p… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（合作伙伴文档中心 · 开发文档(银行/机构) · 「从业机构-微信支付分（免确认模式）」，
2026-10-14 逐页核验，各页「支持商户」栏恒为【从业机构（支付机构）】）：
从业机构预授权 https://pay.weixin.qq.com/doc/v3/partner/4012691489、
查询用户授权记录（授权协议号）https://pay.weixin.qq.com/doc/v3/partner/4012461004、
解除用户授权关系（授权协议号）https://pay.weixin.qq.com/doc/v3/partner/4012461012、
创建支付分订单 https://pay.weixin.qq.com/doc/v3/partner/4012691326、
查询支付分订单 https://pay.weixin.qq.com/doc/v3/partner/4012691343、
取消支付分订单 https://pay.weixin.qq.com/doc/v3/partner/4012458146、
完结支付分订单 https://pay.weixin.qq.com/doc/v3/partner/4012460791、
修改支付分订单金额 https://pay.weixin.qq.com/doc/v3/partner/4012691363、
同步支付分订单状态 https://pay.weixin.qq.com/doc/v3/partner/4012691378、
登记扣款信息 https://pay.weixin.qq.com/doc/v3/partner/4012458414、
查询扣款信息 https://pay.weixin.qq.com/doc/v3/partner/4012458665。

**本域是「从业机构（支付机构）」面，不是服务商面**：官方章节挂在
「开发文档(银行/机构)」树下，签名主体是**从业机构商户号**；渠道商与子商户只出现在报文里
—— 守卫 `PAY-B1` fail-closed 断言全域无 `[Token]`。

**比服务商支付分域（P0-4）多一层 `channel_id`**（守卫 BS-B1 逐端点锁定）：
从业机构 → **渠道商** → 子商户三级，`channel_id`（渠道商商户号）在**十一端点的请求侧处处必填**
（四支 GET 与「查询授权记录」走 query，其余走 body），而服务商支付分域完全没有该字段。
**应答侧必填性会变**：创建/查询订单应答为必填，授权记录应答降为**选填** ⇒ 不能按请求形态推应答。

**单一路由前缀**（与 P0-4 的三前缀形态相反）：全域 `/v3/payscore/acquiringbank/…`，
无 `/v3/payscore/plan/…`、无 `/v3/payscore/sign-plan/…` —— 本域**没有**支付分计划与用户签约计划族
（守卫 BS-B2 以零声明断言钉住，并须落位相邻路由作正对照）。

**本域只有免确认模式**：创单 `need_user_confirm` 官方**必填且固定 `false`**，
故 `state_description` 值集只有 `USER_CONFIRM`/`MCH_COMPLETE` 两值，
**不存在** P0-4「基于签约计划创建服务订单」页的 `MERCHANT_COMPLETE` 拼写（守卫 BS-B4 锁定）。

**官方无「商户收款」「曝光」端点**：SKIT 清单的 `POST /pay/acquiring-bank/transactions/exposure`
出处页（4014326541）现已跳至 50x（公益 404），现行侧栏 API 列表 16 项里也没有该能力；
侧栏的「拉起支付分小程序授权页」「拉起支付分小程序订单详情页」是**端上形态**、
「开启/解除授权服务回调通知」「商户预下单通知」「支付成功回调通知」是**回调面**
⇒ 三类均**零端点**，不得按服务商面或直连面路由补进来（守卫 BS-B2）。

**两套时间格式并存**（守卫 BS-B3 锁定）：`time_range` 用紧凑格式
（`yyyyMMddHHmmss`/`yyyyMMdd`，且 `start_time` 另有 `OnAccept` 取值）、
`collection.details[].paid_time` 与同步的 `detail.paid_time` 亦为紧凑格式，
而 `complete_time`、授权记录的 `cancel_authorization_time`/`authorization_success_time` 走 rfc3339
（官方原文把该标准写作「Rfc399」，是其自身笔误）⇒ DTO 时间字段一律保持 `string`，不经
`DateTimeOffset` 往返。

**本域业务错误码不建专有常量类**（守卫 BS-B5 记录裁决）：官方各页业务码全为通用名
（`INVALID_ORDER_STATE`/`INVALID_REQUEST`/`ORDER_CANCELED`/`ORDER_DONE`/
`ORDER_NOT_EXIST`/`ALREADY_EXIST`/`NO_AUTH`/`RULE_LIMIT`/`FREQUENCY_LIMITED`/
`SYSTEM_ERROR`），且「无授权资格」一支**本域十一页统一写 `NO_AUTH`（有下划线）** ——
与 P0-4 服务商域「计划族写 `NOAUTH`」的形态相反；`ALREADY_EXIST` 只在登记扣款信息页出现。
错误面统一由 `Code` 原样透传与既有 `WechatPayErrorCodes` 承载。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`），不复用默认实例，否则与企微线撞 BaseUrl。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code`/`Message`，
由 `WechatPayException.ThrowIfFailed` 判错；否则组件抛 `ApiException` 把官方业务码丢掉。

**不做运行时多态**：每端点一个方法 + 各自请求 DTO；五支 **204 无包体**端点
（解除授权、取消、完结、修改金额、同步状态）返回裸 `Task`（守卫 BS-B1 计数锁定），
不为其伪造响应类型。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `PreAuthorizeAsync` — 从业机构支付分「商户预授权」：以商户侧生成的授权协议号换取 `apply_permissions_token`，
供拉起支付分小程序授权页。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691489。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/permissions`

- **请求体**：`BankPayScoreApplyPermissionsRequest` — 请求体，字段见 `BankPayScoreApplyPermissionsRequest`。
- **返回**：`BankPayScoreApplyPermissionsResponse` — 仅含 `apply_permissions_token` 的应答，见 `BankPayScoreApplyPermissionsResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/acquiringbank/permissions`；无 path / query 参数。
- **`channel_id` 是本域比服务商面多出的必填项**：渠道商须在服务商平台完成入驻， / `sub_mchid` 由从业机构或渠道商用「开户意愿确认接口」生成。
- **`authorization_code` 是商户侧生成的授权单号**：32 字符内、同一商户号下唯一， / 官方明令多个用户授权不得复用同一协议号。
- **应答 token 有时效**（官方注明 1 小时），过期须重新调用本接口，不得缓存复用。

### 2. `QueryAuthorizationRecordAsync` — 从业机构支付分「查询用户授权记录（授权协议号）」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461004。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizationCode` | Path | `string` | 授权协议号（官方 path `authorization_code`，必填 string(32)）。 |
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填）。 |
| `channelId` | Query `channel_id` | `string` | 渠道商商户号（官方 query `channel_id`，必填）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
- **返回**：`BankPayScoreAuthorizationRecordResponse` — 授权记录，见 `BankPayScoreAuthorizationRecordResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/payscore/acquiringbank/permissions/authorization-code/{authorization_code}`， / 其余三个参数**全部走 query**且**全必填**（官方 curl 原文 / `…?service_id=…&channel_id=…&sub_mchid=…`）。
- **本域只开放协议号一支**：官方侧栏无「按 OpenID 查授权记录」页（服务商域有两支） / ⇒ 本域恰 3 支授权端点（守卫 BS-B1 锁定）。
- **应答里的 `channel_id` 是选填**（请求 query 必填）：本域请求与应答的必填性不镜像，勿按直觉推。
- **`authorization_state` 本页列 3 值**（含 `UnbindUser` / =「仅完成预授权」）⇒ 预授权成功但用户未确认时状态是 `UNBINDUSER`，不得据此判定已获授权。
- **`user_label` 官方值集为 A/B/C 三值**，且「仅授权成功时返回、并且不保证一定返回」； / 官方正文的比较说明自身被截断 ⇒ 不承诺排序语义。

### 3. `TerminateAuthorizationAsync` — 从业机构支付分「解除用户授权关系（授权协议号）」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012461012。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizationCode` | Path | `string` | 授权协议号（官方 path `authorization_code`，必填 string(32)）。 |

- **请求体**：`BankPayScoreTerminatePermissionsRequest` — 请求体（`service_id` + `channel_id` + `sub_mchid` + `reason`），见 `BankPayScoreTerminatePermissionsRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** / `/v3/payscore/acquiringbank/permissions/authorization-code/{authorization_code}/terminate`； / 成功 `204` ⇒ 本方法返回裸 `Task`。
- **影响面**：解除授权后该用户在本服务下不可再免确认创单，应答侧 / `authorization_state` 转为 `Unavailable` / 并由 `cancel_authorization_time` 记录时点。

### 4. `CreateServiceOrderAsync` — 从业机构创建支付分订单（免确认模式），返回订单与 `order_id`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691326。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/serviceorder`

- **请求体**：`BankPayScoreCreateOrderRequest` — 下单请求体，字段见 `BankPayScoreCreateOrderRequest`。
- **返回**：`BankPayScoreCreateOrderResponse` — 订单表 + `package`（本域可忽略），见 `BankPayScoreCreateOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/acquiringbank/serviceorder`；无 path / query 参数。
- **`need_user_confirm` 必填且固定 `false`**：本域只开放免确认模式； / `risk_fund` 与 `time_range` 在本域**必填**（服务商域同为必填，但本域 `notify_url` 亦必填）。
- **`openid` / `sub_openid` 二选一必传**，传 `sub_openid` 则 `sub_appid` 必填。
- **红线**：`out_order_no` **不可**用作申请退款接口的 `out_trade_no`（官方原文）—— / 退款须用查询应答 `collection.details[].transaction_id`；`out_order_no` 是服务订单号， / 而 `out_trade_no` 属于扣款单（见 `RegisterDeductInfoAsync`）。
- **业务限制**：`appid` 须与从业机构商户号绑定、`sub_appid` 须与 `sub_mchid` 绑定； / 用户达周期内创单上限时官方返回 `403 RULE_LIMIT`（须用户主动在支付分「服务管理页」去限制或次日再下单）。

### 5. `QueryServiceOrderAsync` — 从业机构查询支付分订单（含收款信息与代金券优惠明细）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691343。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/acquiringbank/serviceorder`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
| `channelId` | Query `channel_id` | `string` | 渠道商商户号（官方 query `channel_id`，必填）。 |
| `outOrderNo` | Query `out_order_no` | `string?` | 商户服务订单号（官方 query `out_order_no`，**选填**）。 |
| `serviceId` | Query `service_id` | `string?` | 服务 ID（官方 query `service_id`，**选填**）。 |
- **返回**：`BankPayScoreQueryOrderResponse` — 订单全集，见 `BankPayScoreQueryOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/acquiringbank/serviceorder`，四参数全走 query； / 官方必填组合是 `sub_mchid` + `channel_id`，而 `service_id` 与 `out_order_no` / **在本请求侧都是选填**（应答侧 `service_id` 为必填）—— 与服务商域 / 「`sub_mchid` + `out_order_no` 必填」不同形（守卫 BS-B1 逐端点锁定）。
- **与创建应答形态不同**：本应答**无** `package`、有 `collection`/ / `promotion_detail`/`total_amount`/`openid`/`sub_openid` ⇒ 分建两类型； / **本域亦无 `need_collection`**（服务商域查询应答有该字段且固定返回 `true`）。
- **状态机**：官方原文要求**参考「支付分订单状态流转图」**处理， / `state` 须结合 `collection.state` 与 `state_description` 判断； / `collection` 仅在调完结后返回，完结金额为 0 时不返回。
- **退款口径**：收款明细里 `paid_type=NEWTON` 才返回 `transaction_id`， / 申请退款用该单号；`paid_type=MCH`（用户走其它渠道、商户调同步接口补登记）无该单号。

### 6. `CancelServiceOrderAsync` — 从业机构取消支付分订单。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458146。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/serviceorder/{outOrderNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`BankPayScoreCancelOrderRequest` — 请求体（`service_id` 选填 / `sub_mchid` 必填 / `channel_id` 必填 / `reason` 必填），见 `BankPayScoreCancelOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/acquiringbank/serviceorder/{out_order_no}/cancel`； / 成功 `204`。
- **`service_id` 在本页是选填**（完结/修改/同步页必填）—— 四页字段表各不同，照录本页。
- **`reason` 用户可见**：官方注明取消订单的消息通知会展示该字段，长度上限 50。

### 7. `CompleteServiceOrderAsync` — 从业机构完结支付分订单（用户已实际接受服务，进入待收款状态）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012460791。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/serviceorder/{outOrderNo}/complete`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 **string(64)**）。 |

- **请求体**：`BankPayScoreCompleteOrderRequest` — 请求体，字段见 `BankPayScoreCompleteOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/acquiringbank/serviceorder/{out_order_no}/complete`； / 成功 `204`。
- **⚠️ 本端点 path 参数官方宽度写 `string(64)`**，而同族取消/修改/同步三页写 / `string(32)` —— 官方自身不一致，照录各自页（守卫 BS-B6 锁定）， / 商户侧单号生成仍以创单页的 32 为约束。
- **金额必须自洽**：`total_amount` = `post_payments.amount` 总和 − / `post_discounts.amount` 总和，且 ≤「服务 ID 风险金额上限」（须与微信支付运营/BD 确认）。
- **`complete_time` 是本域唯一的 rfc3339 请求字段**：官方约束落在 / `[t−24h, t+60s]`（`t` 为调用完结接口的当前时间），越界即 `PARAM_ERROR`。

### 8. `ModifyServiceOrderAsync` — 从业机构修改支付分订单金额（完结后仅可下调收款金额）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691363。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/serviceorder/{outOrderNo}/modify`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`BankPayScoreModifyOrderRequest` — 请求体，字段见 `BankPayScoreModifyOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/acquiringbank/serviceorder/{out_order_no}/modify`； / 成功 `204`。官方为「修改金额」单设一页，SKIT 清单该行误挂取消页 4012458146，此处按官方侧栏修正。
- **可调用窗口**：仅 `state=DOING` 且 `state_description=MCH_COMPLETE`， / 且只能**下调**收款金额。
- **扣款单互斥**：存在登记生效的扣款单（`RegisterDeductInfoAsync`）时须等其 24 小时失效、 / 或先调清算机构（银联/网联）关单，才能改金额。

### 9. `SyncServiceOrderAsync` — 从业机构同步支付分订单状态（用户已走**其它渠道**付款时补登记）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691378。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/serviceorder/{outOrderNo}/sync`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`BankPayScoreSyncOrderRequest` — 请求体，字段见 `BankPayScoreSyncOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/acquiringbank/serviceorder/{out_order_no}/sync`； / 成功 `204`。
- **`type` 官方单值**：`OrderPaid`（混合大小写，非全大写蛇形）， / 该 `type` 下 `detail` 必填。
- **`detail.paid_time` 有硬性区间**：官方要求 / 「商户调用完结接口时间 < `paid_time` ≤ 商户调用同步订单接口时间 + 60 秒」， / 支持 `yyyyMMddHHmmss` 与 `yyyyMMdd` 两种紧凑格式；微信支付分据该时间更新用户侧守约/负面记录 / ⇒ **必须如实填写**，造假将直接引发客诉。
- **与完结同样的窗口与扣款单约束**：仅 `DOING` + `MCH_COMPLETE` 可调， / 存在生效扣款单时须先失效或关单。

### 10. `RegisterDeductInfoAsync` — 从业机构登记扣款信息（清算机构委托代扣的前置登记），返回回显的 `description`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458414。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/acquiringbank/serviceorder/{outOrderNo}/registerdeductinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`BankPayScoreRegisterDeductRequest` — 请求体，字段见 `BankPayScoreRegisterDeductRequest`。
- **返回**：`BankPayScoreRegisterDeductResponse` — 仅含 `description` 的应答，见 `BankPayScoreRegisterDeductResponse`。

**备注（原文）**

- **官方契约**：**POST** / `/v3/payscore/acquiringbank/serviceorder/{out_order_no}/registerdeductinfo`；成功 `200`。
- **一致性是本端点的存在理由**：登记的 `out_trade_no` 与 `description` 必须与后续调用 / 清算机构（银联/网联）委托代扣接口时传的商户订单号、商品描述**逐字一致**，否则代扣失败。
- **时效 24 小时**：生效期间该订单**不得**修改金额或同步状态， / 须等扣款单失效或先走清算机构关单。
- **`ALREADY_EXIST` 只在本端点出现**：重复登记同一扣款单时官方返回该码， / 属可重试判错面（守卫 BS-B5 记录，不建专有码常量类）。

### 11. `QueryDeductionInfoAsync` — 从业机构查询扣款信息（回读已登记的 `description` 与 `out_trade_no`）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012458665。

- **声明接口**：[IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/acquiringbank/serviceorder/deduction`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填）。 |
| `appId` | Query `appid` | `string` | 从业机构 AppID（官方 query `appid`，必填）。 |
| `subMchId` | Query `sub_mchid` | `string` | 子商户号（官方 query `sub_mchid`，必填）。 |
| `channelId` | Query `channel_id` | `string` | 渠道商商户号（官方 query `channel_id`，必填）。 |
| `outOrderNo` | Query `out_order_no` | `string` | 商户服务订单号（官方 query `out_order_no`，必填）。 |
| `subAppId` | Query `sub_appid` | `string?` | 子商户 AppID（官方 query `sub_appid`，**选填**）。 |
- **返回**：`BankPayScoreDeductionQueryResponse` — 扣款信息，见 `BankPayScoreDeductionQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/acquiringbank/serviceorder/deduction`， / 六个参数**全部走 query**（无 path 参数）。
- **⚠️ 路由形态陷阱**：本端点与 `QueryServiceOrderAsync` 同为 / `GET /v3/payscore/acquiringbank/serviceorder…` 前缀，本端点是**固定段 `/deduction`** / 而非 `/{out_order_no}/deduction` —— 按服务订单族的直觉拼 path 会得 `ORDER_NOT_EXIST` / （守卫 BS-B2 以两支相邻路由并存锁定）。
- **应答两字段均必填**：查询即回读登记值，用于代扣前自检一致性。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/BankPayScore`](../../../Mud.Wechat.Pay.DataModels/BankPayScore/) 内共 **24** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **24** 个，`SerializerClassName` = `BankPayScore`。

AOT 源生成上下文：[`BankPayScoreJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BankPayScoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BankPayScoreApplyPermissionsResponse` | 应答 | [BankPayScore/BankPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScorePermissionsDtos.cs) | `service_id`, `channel_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `reason`, `mchid`, `openid`, `authorization_state` … |
| `BankPayScoreApplyPermissionsRequest` | 请求体 | [BankPayScore/BankPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScorePermissionsDtos.cs) | `service_id`, `channel_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `reason`, `mchid`, `openid`, `authorization_state` … |
| `BankPayScoreAuthorizationRecordResponse` | 应答 | [BankPayScore/BankPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScorePermissionsDtos.cs) | `service_id`, `channel_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `reason`, `mchid`, `openid`, `authorization_state` … |
| `BankPayScoreTerminatePermissionsRequest` | 请求体 | [BankPayScore/BankPayScorePermissionsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScorePermissionsDtos.cs) | `service_id`, `channel_id`, `appid`, `sub_appid`, `sub_mchid`, `authorization_code`, `notify_url`, `apply_permissions_token`, `reason`, `mchid`, `openid`, `authorization_state` … |
| `BankPayScoreCreateOrderResponse` | 应答 | [BankPayScore/BankPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `channel_id`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts` … |
| `BankPayScoreCreateOrderRequest` | 请求体 | [BankPayScore/BankPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `channel_id`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location` … |
| `BankPayScoreQueryOrderResponse` | 应答 | [BankPayScore/BankPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `channel_id`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts` … |
| `BankPayScoreCancelOrderRequest` | 请求体 | [BankPayScore/BankPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `channel_id`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location` … |
| `BankPayScoreCompleteOrderRequest` | 请求体 | [BankPayScore/BankPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `channel_id`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location` … |
| `BankPayScoreModifyOrderRequest` | 请求体 | [BankPayScore/BankPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `channel_id`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location` … |
| `BankPayScoreSyncOrderRequest` | 请求体 | [BankPayScore/BankPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `channel_id`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location` … |
| `BankPayScoreRegisterDeductResponse` | 应答 | [BankPayScore/BankPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `channel_id`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts` … |
| `BankPayScoreRegisterDeductRequest` | 请求体 | [BankPayScore/BankPayScoreServiceOrderRequestDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderRequestDtos.cs) | `service_id`, `appid`, `sub_mchid`, `sub_appid`, `channel_id`, `out_order_no`, `service_introduction`, `post_payments`, `post_discounts`, `risk_fund`, `time_range`, `location` … |
| `BankPayScoreDeductionQueryResponse` | 应答 | [BankPayScore/BankPayScoreServiceOrderResponseDtos.cs](../../../Mud.Wechat.Pay.DataModels/BankPayScore/BankPayScoreServiceOrderResponseDtos.cs) | `out_order_no`, `service_id`, `appid`, `mchid`, `sub_appid`, `sub_mchid`, `channel_id`, `service_introduction`, `state`, `state_description`, `post_payments`, `post_discounts` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BankPayScore;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BankPayScore));

var service = app.Services.GetRequiredService<IWechatPayBankPayScoreService>();
var ct = CancellationToken.None;
var response = await service.CreateServiceOrderAsync(new BankPayScoreCreateOrderRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/payscore/acquiringbank/serviceorder");
// 占位值仅用于编译示意；业务值须按 IWechatPayBankPayScoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateServiceOrderAsync`）；请求体 `BankPayScoreCreateOrderRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBankPayScoreService](./IWechatPayBankPayScoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBankPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BankPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankPayScoreRoutes_ShouldStayOnSingleAcquiringBankPrefixAndNotDeclareDelistedOrForeignEndpoints`、`BankPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings`、`BankPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromNeighbouringDomains`、`BankPayScore_ShouldNotDeclareDomainSpecificErrorCodes`、`BankPayScoreResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch`、`BankPayScoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/BankPayScore`](../../../Mud.Wechat.Pay.DataModels/BankPayScore/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBankPayScoreContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
