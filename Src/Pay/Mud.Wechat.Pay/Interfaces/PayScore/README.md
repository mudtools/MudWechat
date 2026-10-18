# PayScore 域 · 支付分

微信支付「支付分」域 SDK（APIv3 支付分，**首批 3 端点**：创建 / 查询 / 取消服务订单）。 接口：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)；注册组 `PayScore`；模块枚举 `PayModule.PayScore`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBankPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[WechatPayPartnerPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerPayScoreContractGuards.cs)、[WechatPayPayScoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayScoreContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PayScore`](../../../Mud.Wechat.Pay.DataModels/PayScore/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPayScoreService = 11；按 HTTP 特性实计 **11** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPayScoreService` | `CreateServiceOrderAsync` | POST | `/v3/payscore/serviceorder` | 商户证书 / APIv3 签名 | 创建支付分订单。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012587900。 |
| 2 | `IWechatPayPayScoreService` | `QueryServiceOrderAsync` | GET | `/v3/payscore/serviceorder` | 商户证书 / APIv3 签名 | 查询支付分订单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_15.shtml。 |
| 3 | `IWechatPayPayScoreService` | `CancelServiceOrderAsync` | POST | `/v3/payscore/serviceorder/{outOrderNo}/cancel` | 商户证书 / APIv3 签名 | 取消支付分订单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_16.shtml。 |
| 4 | `IWechatPayPayScoreService` | `CompleteServiceOrderAsync` | POST | `/v3/payscore/serviceorder/{outOrderNo}/complete` | 商户证书 / APIv3 签名 | 完结支付分订单。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012587955。 |
| 5 | `IWechatPayPayScoreService` | `ModifyServiceOrderAsync` | POST | `/v3/payscore/serviceorder/{outOrderNo}/modify` | 商户证书 / APIv3 签名 | 修改订单金额。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_17.shtml。 |
| 6 | `IWechatPayPayScoreService` | `CollectServiceOrderAsync` | POST | `/v3/payscore/serviceorder/{outOrderNo}/pay` | 商户证书 / APIv3 签名 | 发起催收扣款。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_19.shtml。 |
| 7 | `IWechatPayPayScoreService` | `PreAuthorizeAsync` | POST | `/v3/payscore/permissions` | 商户证书 / APIv3 签名 | 商户预授权。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_2.shtml。 |
| 8 | `IWechatPayPayScoreService` | `QueryAuthorizationRecordAsync` | GET | `/v3/payscore/permissions/authorization-code/{authorizationCode}` | 商户证书 / APIv3 签名 | 查询用户授权记录（授权协议号）。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_3.shtml。 |
| 9 | `IWechatPayPayScoreService` | `SyncServiceOrderAsync` | POST | `/v3/payscore/serviceorder/{outOrderNo}/sync` | 商户证书 / APIv3 签名 | 同步订单状态。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_20.shtml。 |
| 10 | `IWechatPayPayScoreService` | `TerminateAuthorizationAsync` | POST | `/v3/payscore/permissions/authorization-code/{authorizationCode}/terminate` | 商户证书 / APIv3 签名 | 解除用户授权（授权协议号）。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_4.shtml。 |
| 11 | `IWechatPayPayScoreService` | `QueryAuthorizationRecordByOpenIdAsync` | GET | `/v3/payscore/permissions/openid/{openId}` | 商户证书 / APIv3 签名 | 查询用户授权记录（OPENID）。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_5.shtml。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（普通商户文档中心，2026-10-09 逐页核验）：
创建支付分订单 https://pay.weixin.qq.com/doc/v3/merchant/4012587900、
查询支付分订单 https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_15.shtml、
取消支付分订单 https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_16.shtml。
三页均标注支持商户：**普通商户**。

**✅ 本域现已 11 端点**（原「尚未覆盖」清单已随增量关闭）：创建订单 · 查询订单 · 取消订单 ·
**完结服务订单** · **修改订单金额** · **订单收款** · **同步订单** ·
**商户预授权** · **查询用户授权记录** · **查询用户授权状态** · **解除授权**。

**⚠️ 唯一刻意不建模的是「创单结单合并」**
（`POST /v3/payscore/serviceorder/direct-complete`）：官方页明示
「特别提醒：创单结单合并接口**暂未对外开放**」⇒ 按「未开放不建模」纪律暂缺
（守卫 `PY-B9` 固化该裁决，恢复前须先确认开通权限与真实路径）。

**回调面**：已覆盖确认订单 / 支付成功 / 授权成功 / 解除授权四类
（`PAYSCORE.USER_CONFIRM` / `USER_PAID` / `USER_OPEN_SERVICE` / `USER_CLOSE_SERVICE`，
均**大写**前缀），归回调包。
**⚠️ 留档一处未核验**：「订单状态变更通知」是否存在**本轮未单独核实** ⇒
勿按其它产品线的习惯臆造事件名。

**不要凭推断补路由**——须先按接口名核验其页面。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateServiceOrderAsync` — 创建支付分订单。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012587900。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/serviceorder`

- **请求体**：`PayScoreServiceOrderRequest` — 创单请求体，见 `PayScoreServiceOrderRequest`。
- **返回**：`PayScoreServiceOrderResponse` — 服务订单（创单成功 `state = CREATED`，含拉起确认订单页的 `package`），见 `PayScoreServiceOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/serviceorder`；无 path / query 参数。
- **幂等**：官方原文「该接口**支持原参重入**，相同参数重复调用可以返回成功」—— / 但商户侧仍须以 `out_order_no` 做业务幂等（重入成功≠业务只发生一次）。
- **红线**：`out_order_no` **不可**用作申请退款接口的 `out_trade_no`（官方原文）。

### 2. `QueryServiceOrderAsync` — 查询支付分订单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_15.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/serviceorder`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填 string(32)）。 |
| `appId` | Query `appid` | `string` | 公众账号 ID（官方 query `appid`，必填 string(32)）。 |
| `outOrderNo` | Query `out_order_no` | `string?` | 商户服务订单号（官方 query `out_order_no`，选填）：与<br> **二选一**。 |
| `queryId` | Query `query_id` | `string?` | 微信侧查询凭据（官方 query `query_id`，选填 string(512)）：与<br> **二选一**。 |
- **返回**：`PayScoreServiceOrderQueryResponse` — 服务订单（含收款与优惠信息），见 `PayScoreServiceOrderQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/serviceorder?out_order_no=…&service_id=…&appid=…&query_id=…`； / 必填 `service_id` + `appid`；`out_order_no` 与 `query_id` / **二者必填其一，不允许都填或都不填**（官方原文明示的互斥约束，本签名以可选参数表达， / 互斥校验须由服务端返回的 `PARAM_ERROR` 或调用方前置校验承担）。
- **状态机**：官方要求**参考「支付分订单状态流转图」**处理业务逻辑，不得自行推断流转。

### 3. `CancelServiceOrderAsync` — 取消支付分订单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_16.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/serviceorder/{outOrderNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PayScoreCancelOrderRequest` — 取消请求体，见 `PayScoreCancelOrderRequest`。
- **返回**：`PayScoreCancelOrderResponse` — 被取消的订单标识（仅 5 字段），见 `PayScoreCancelOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/serviceorder/{out_order_no}/cancel`； / path 必带 `out_order_no`，body 含 `appid`(必) / `service_id`(**选填**) / `reason`(必)。
- **可取消状态**：`CREATED` 与 `DOING`（含已完结但收款状态为待支付 `USER_PAYING`）。
- **appid 一致性**：官方两处强调「完结订单和取消订单需与创单传入的 appid 保持一致」。

### 4. `CompleteServiceOrderAsync` — 完结支付分订单。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012587955。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/serviceorder/{outOrderNo}/complete`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(64)）。 |

- **请求体**：`PayScoreCompleteOrderRequest` — 完结请求体，见 `PayScoreCompleteOrderRequest`。
- **返回**：`PayScoreCompleteOrderResponse` — 完结后的订单（`need_collection` 固定为 `true`），见 `PayScoreCompleteOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/serviceorder/{out_order_no}/complete`； / path 必带 `out_order_no`；body 的 `post_payments` 与 `total_amount` 均为**必填**。
- **与创单的必填性差异（官方原样）**：`post_payments` 在创单里是**选填**、在完结里是**必填**； / `total_amount` 只在本接口出现（创单不含最终金额）。
- **金额上限**：官方注明 `total_amount` **受服务 ID 风险金额上限影响** —— / 超限会被风控拒绝，不是本地校验能替的。
- **后续动作**：完结只代表金额已确认并进入待收款（`need_collection = true`）， / **不等于**已收到款；收款侧须另用「发起催收扣款」（`POST /v3/payscore/serviceorder/{out_order_no}/pay`） / 或等用户自动扣款，并以查询接口/回调确认终态。

### 5. `ModifyServiceOrderAsync` — 修改订单金额。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_17.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/serviceorder/{outOrderNo}/modify`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PayScoreModifyOrderRequest` — 修改请求体，见 `PayScoreModifyOrderRequest`。
- **返回**：`PayScoreModifyOrderResponse` — 修改后的订单（含收款信息），见 `PayScoreModifyOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/serviceorder/{out_order_no}/modify`； / path 必带 `out_order_no`；body 的 `post_payments` / `total_amount` / `reason` 均**必填**。
- **必填性差异**：`post_payments` 在此为必填（创单选填）；`post_discounts[].name` 在此为选填（创单必填）。
- **结构差异（勿按查询应答解析）**：本接口应答的 `collection` **内嵌** / `promotion_detail` 与 `goods_detail`，而查询应答把它们放在顶层。

### 6. `CollectServiceOrderAsync` — 发起催收扣款。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_19.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/serviceorder/{outOrderNo}/pay`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PayScoreCollectRequest` — 扣款请求体（仅 `appid` + `service_id`），见 `PayScoreCollectRequest`。
- **返回**：`PayScoreCollectResponse` — 被催收的订单标识（仅 4 字段），见 `PayScoreCollectResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/serviceorder/{out_order_no}/pay`； / path 必带 `out_order_no`；body 仅 `appid` / `service_id`（两者均必填）。
- **⚠️ 语义**：官方接口名是「**发起催收扣款**」—— 对已完结且待收款（`USER_PAYING`） / 的订单**主动发起扣款**；**不等于**入账、也不等于查询。扣款结果须经查询接口或支付成功回调确认。

### 7. `PreAuthorizeAsync` — 商户预授权。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_2.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/permissions`

- **请求体**：`PayScorePermissionsRequest` — 预授权请求体，见 `PayScorePermissionsRequest`。
- **返回**：`PayScorePermissionsResponse` — 预授权令牌（唯一字段），见 `PayScorePermissionsResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/permissions`；无 path / query 参数； / body 必填 `service_id` / `appid` / `authorization_code`，`notify_url` 选填。
- **产品线归属**：官方把本端点归入「微信支付分（**免确认模式**）」—— 免确认模式才有预授权流程。

### 8. `QueryAuthorizationRecordAsync` — 查询用户授权记录（授权协议号）。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_3.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/permissions/authorization-code/{authorizationCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizationCode` | Path | `string` | 授权协议号（官方 path `authorization_code`，必填 string(32)）。 |
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填 string(32)）。 |
- **返回**：`PayScoreAuthorizationRecordResponse` — 授权记录（含授权状态与授权/取消时间），见 `PayScoreAuthorizationRecordResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/permissions/authorization-code/{authorization_code}?service_id=…`； / path 必带 `authorization_code`、query 必带 `service_id`。
- **未完成项（诚实记录）**：`authorization_state` 的**值域**本轮未取得（核验只覆盖字段表） / ⇒ SDK 不臆造枚举常量；使用方须以官方值表判定，后续增量将补该值表并加守卫。

### 9. `SyncServiceOrderAsync` — 同步订单状态。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_20.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/serviceorder/{outOrderNo}/sync`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outOrderNo` | Path | `string` | 商户服务订单号（官方 path `out_order_no`，必填 string(32)）。 |

- **请求体**：`PayScoreSyncOrderRequest` — 同步请求体，见 `PayScoreSyncOrderRequest`。
- **返回**：`PayScoreSyncOrderResponse` — 同步后的订单，见 `PayScoreSyncOrderResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/serviceorder/{out_order_no}/sync`； / path 必带 `out_order_no`；body 必填 `appid` / `service_id` / `type` / `detail` / （`detail.paid_time` 必填）。
- **`type` 取值**：官方只给 `Order_Paid`（收款场景固定值，**混合大小写**）， / 见 `PayScoreSyncOrderTypes`。

### 10. `TerminateAuthorizationAsync` — 解除用户授权（授权协议号）。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_4.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`POST /v3/payscore/permissions/authorization-code/{authorizationCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizationCode` | Path | `string` | 授权协议号（官方 path `authorization_code`，必填 string(32)）。 |

- **请求体**：`PayScoreTerminateAuthorizationRequest` — 解除请求体（`service_id` + `reason` 均必填），见 `PayScoreTerminateAuthorizationRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/payscore/permissions/authorization-code/{authorization_code}/terminate`； / path 必带 `authorization_code`，**无 query**；body 必填 `service_id` / `reason`。
- **返回类型为何是 `Task`**：官方明确「**无应答包体**」，成功状态码 **204 No Content** / ⇒ 与交易域关单（同为 204）同款处置：声明为无返回值，避免调用方去解一个不存在的包体。

### 11. `QueryAuthorizationRecordByOpenIdAsync` — 查询用户授权记录（OPENID）。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_5.shtml。

- **声明接口**：[IWechatPayPayScoreService](./IWechatPayPayScoreService.cs)
- **官方契约**：`GET /v3/payscore/permissions/openid/{openId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openId` | Path | `string` | 用户标识（官方 path `openid`，必填 string(128)）。 |
| `serviceId` | Query `service_id` | `string` | 服务 ID（官方 query `service_id`，必填 string(32)）。 |
| `appId` | Query `appid` | `string` | 公众账号 ID（官方 query `appid`，必填 string(32)）。 |
- **返回**：`PayScoreAuthorizationRecordResponse` — 授权记录，见 `PayScoreAuthorizationRecordResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/payscore/permissions/openid/{openid}?service_id=…&appid=…`； / path 必带 `openid`，query 必填 `service_id` 与 `appid`（**两个都是必填**）。
- **返回类型为何与「授权协议号」版共用**：官方两页的应答字段表**逐项完全一致** / （8 个平铺字段，含 `authorization_state`），只是**入参维度**不同（按协议号 vs 按 openid）。 / 同一份事实两处类型只会让字段漂移 —— 故复用同一 DTO（与「请求分账/查询分账结果」共用应答同款判断： / **表相同则共用，表不同则分建**）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PayScore`](../../../Mud.Wechat.Pay.DataModels/PayScore/) 内共 **29** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **29** 个，`SerializerClassName` = `PayScore`。

AOT 源生成上下文：[`PayScoreJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PayScoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PayScoreServiceOrderResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreServiceOrderRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreServiceOrderQueryResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreCancelOrderResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreCancelOrderRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreCompleteOrderResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreCompleteOrderRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreModifyOrderResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreModifyOrderRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreCollectResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreCollectRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScorePermissionsResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScorePermissionsRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreAuthorizationRecordResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreSyncOrderResponse` | 应答 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreSyncOrderRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |
| `PayScoreTerminateAuthorizationRequest` | 请求体 | [PayScore/PayScoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/PayScore/PayScoreDtos.cs) | `out_order_no`, `appid`, `service_id`, `service_introduction`, `post_payments`, `post_discounts`, `time_range`, `location`, `risk_fund`, `attach`, `notify_url`, `need_user_confirm` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PayScore;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PayScore));

var service = app.Services.GetRequiredService<IWechatPayPayScoreService>();
var ct = CancellationToken.None;
var response = await service.CreateServiceOrderAsync(new PayScoreServiceOrderRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/payscore/serviceorder");
// 占位值仅用于编译示意；业务值须按 IWechatPayPayScoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateServiceOrderAsync`）；请求体 `PayScoreServiceOrderRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPayScoreService](./IWechatPayPayScoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBankPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankPayScoreContractGuards.cs)、[`WechatPayPartnerPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerPayScoreContractGuards.cs)、[`WechatPayPayScoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayScoreContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BankPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankPayScoreRoutes_ShouldStayOnSingleAcquiringBankPrefixAndNotDeclareDelistedOrForeignEndpoints`、`BankPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings`、`BankPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromNeighbouringDomains`、`BankPayScore_ShouldNotDeclareDomainSpecificErrorCodes`、`BankPayScoreResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch`、`BankPayScoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PartnerPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`PartnerPayScoreRoutes_ShouldStayOnThreePartnerPrefixesAndNotDeclareMerchantCollectPay`、`PartnerPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings`、`PartnerPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromDirectDomain`、`PartnerPayScore_ShouldNotDeclareDomainSpecificErrorCodes` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PayScore`](../../../Mud.Wechat.Pay.DataModels/PayScore/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBankPayScoreContractGuards.cs`、`WechatPayPartnerPayScoreContractGuards.cs`、`WechatPayPayScoreContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
