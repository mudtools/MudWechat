# MarketingFavor 域 · 代金券

微信支付「代金券」域 SDK（APIv3 营销·代金券，**15 端点**：批次生命周期 6 + 券读写 3 + 批次明细 4 + 回调地址 2）。 接口：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)；注册组 `MarketingFavor`；模块枚举 `PayModule.MarketingFavor`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingFavorContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingFavorContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayLovefeastContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayLovefeastContractGuards.cs)、[WechatPayMultiuseContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMultiuseContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MarketingFavor`](../../DataModels/MarketingFavor)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingFavorService = 15；按 HTTP 特性实计 **15** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingFavorService` | `CreateCouponStockAsync` | POST | `/v3/marketing/favor/coupon-stocks` | 商户证书 / APIv3 签名 | 创建代金券批次。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534633。 |
| 2 | `IWechatPayMarketingFavorService` | `QueryCouponAsync` | GET | `/v3/marketing/favor/users/{openId}/coupons/{couponId}` | 商户证书 / APIv3 签名 | 查询代金券详情。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012486942。 |
| 3 | `IWechatPayMarketingFavorService` | `StartCouponStockAsync` | POST | `/v3/marketing/favor/stocks/{stockId}/start` | 商户证书 / APIv3 签名 | 激活代金券批次。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/start-stock.h… |
| 4 | `IWechatPayMarketingFavorService` | `PauseCouponStockAsync` | POST | `/v3/marketing/favor/stocks/{stockId}/pause` | 商户证书 / APIv3 签名 | 暂停代金券批次。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/pause-stock.h… |
| 5 | `IWechatPayMarketingFavorService` | `RestartCouponStockAsync` | POST | `/v3/marketing/favor/stocks/{stockId}/restart` | 商户证书 / APIv3 签名 | 重启代金券批次。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/restart-stock… |
| 6 | `IWechatPayMarketingFavorService` | `QueryCouponStockAsync` | GET | `/v3/marketing/favor/stocks/{stockId}` | 商户证书 / APIv3 签名 | 查询批次详情。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/query-stock.ht… |
| 7 | `IWechatPayMarketingFavorService` | `IssueCouponAsync` | POST | `/v3/marketing/favor/users/{openId}/coupons` | 商户证书 / APIv3 签名 | 发放指定批次的代金券。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463767。 |
| 8 | `IWechatPayMarketingFavorService` | `SetCouponCallbackAsync` | POST | `/v3/marketing/favor/callbacks` | 商户证书 / APIv3 签名 | 设置代金券回调通知 url。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012464198（普通商户）、https://pay… |
| 9 | `IWechatPayMarketingFavorService` | `QueryCouponCallbackAsync` | GET | `/v3/marketing/favor/callbacks` | 商户证书 / APIv3 签名 | 查询代金券回调通知 url。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012464070（普通商户）、https://pay… |
| 10 | `IWechatPayMarketingFavorService` | `QueryCouponStocksAsync` | GET | `/v3/marketing/favor/stocks` | 商户证书 / APIv3 签名 | 条件查询代金券批次列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012460489（普通商户）、https://pay.w… |
| 11 | `IWechatPayMarketingFavorService` | `QueryCouponStockItemsAsync` | GET | `/v3/marketing/favor/stocks/{stockId}/items` | 商户证书 / APIv3 签名 | 查询代金券可用单品。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463442（普通商户）、https://pay.wei… |
| 12 | `IWechatPayMarketingFavorService` | `QueryCouponStockMerchantsAsync` | GET | `/v3/marketing/favor/stocks/{stockId}/merchants` | 商户证书 / APIv3 签名 | 查询代金券可用商户。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463358（普通商户）、https://pay.wei… |
| 13 | `IWechatPayMarketingFavorService` | `QueryCouponStockRefundFlowAsync` | GET | `/v3/marketing/favor/stocks/{stockId}/refund-flow` | 商户证书 / APIv3 签名 | 下载批次退款明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463523（普通商户）、https://pay.weix… |
| 14 | `IWechatPayMarketingFavorService` | `QueryCouponStockUseFlowAsync` | GET | `/v3/marketing/favor/stocks/{stockId}/use-flow` | 商户证书 / APIv3 签名 | 下载批次核销明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463585（普通商户）、https://pay.weix… |
| 15 | `IWechatPayMarketingFavorService` | `QueryCouponUserCouponsAsync` | GET | `/v3/marketing/favor/users/{openid}/coupons` | 商户证书 / APIv3 签名 | 条件查询用户代金券列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534690（普通商户）、https://pay.w… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（普通商户文档中心，2026-10-09 核验；产品归属「营销产品 > 代金券」，
官方**未**声明下线）：
创建代金券批次 https://pay.weixin.qq.com/doc/v3/merchant/4012534633（更新 2024.10.31）、
查询代金券详情 https://pay.weixin.qq.com/doc/v3/merchant/4012486942（更新 2024.09.19）。
两页均标注**支持商户：【普通商户】**。

**官方批次生命周期（页面导航照录）**：创建 → **激活**（`POST /v3/marketing/favor/stocks/{stock_id}/start`）
→ 发放 → 暂停（`…/pause`）/ 重启（`…/restart`）；另有条件查询批次列表、查询批次详情、
查询代金券可用商户 / 可用单品、下载批次退款明细 / 核销明细、图片上传（营销专用）。
⚠️ **创建成功 ≠ 可发放**：须先激活。

**✅ 本域现已 15 端点**（原「尚未覆盖：激活 / 暂停 / 重启 / 批次详情」与 P1-3 的「列表 / 明细 / 回调地址」留档均已关闭）：
创建批次 + 激活批次 + 暂停批次 + 重启批次 + 查询批次详情 + 条件查询批次列表 +
查询券详情 + 条件查询用户券列表 + 发放代金券 +
查询可用单品 / 可用商户 / 下载批次退款明细 / 核销明细 + 设置 / 查询回调通知 url
⇒ 「创建 → 激活 → 发放 → 暂停 / 重启」主流程与对账明细下载可完整执行。

**本域仍未覆盖**：营销专用图片上传（归 `MarketingMedia` 域，multipart 手工通道）
与代金券相关**通知接收**（归回调包）。**不要凭推断补路由**。

**双面同表 ⇒ 一面一方法**：本域新增 8 支同时有【普通商户】与【普通服务商】两页，
逐页字段表指纹比对**完全同形** ⇒ 只声明一个方法、注释引用两页（与
`IWechatPayMerchantRiskManageService` 的「表不同则分建」判据同源）。

**✅ 值表已补齐（原「值域未核验」留档已关闭）**：由官方「查询批次详情」页逐字取得 ——
`status`（**全小写**，含官方拼写 `stoped`）、`coupon_type`、`stock_type`、
`trade_type`、`business_type` 与地域级别，见 `MarketingFavorConstants.cs`。
**⚠️ 注意两条风格陷阱**：① 本域 `status` 是**全小写**（与支付分/发票的
`event_type` **全大写**风格相反）；② 官方拼写 `stoped`（少一个 p）**必须照录**。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）**支持商户**【普通商户】【普通服务商】 【从业机构（银行）】 【从业机构（支付机构）】 【渠道商】 【清算机构】。
- （出现 3 次）**`offset` 与 `limit` 与 `stock_creator_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 1. `CreateCouponStockAsync` — 创建代金券批次。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534633。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`POST /v3/marketing/favor/coupon-stocks`

- **请求体**：`CouponStockCreateRequest` — 创建批次请求体，见 `CouponStockCreateRequest`。
- **返回**：`CouponStockCreateResponse` — 批次号与创建时间（**仅 2 字段**），见 `CouponStockCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/favor/coupon-stocks`；无 path / query 参数。
- **跨字段约束（官方原文）**：`coupon_use_rule.fixed_normal_coupon` 在 / `stock_type = NORMAL` 时**必填**；`coupon_use_rule` 内多数子字段选填， / 但 `available_merchants` 必填。
- **后续动作**：创建后须调「激活代金券批次」该批次才可发放（本批未实现该端点）。

### 2. `QueryCouponAsync` — 查询代金券详情。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012486942。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/users/{openId}/coupons/{couponId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openId` | Path | `string` | 用户 openid（官方 path `openid`，必填 string(128)）。 |
| `couponId` | Path | `string` | 代金券或消费金 id（官方 path `coupon_id`，必填 string(20)）。 |
| `appId` | Query `appid` | `string` | 公众账号 ID（官方 query `appid`，必填 string(128)）。 |
- **返回**：`CouponQueryResponse` — 代金券详情（含状态与各类券型信息），见 `CouponQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/users/{openid}/coupons/{coupon_id}?appid=…`； / 两个 path 参数与 `appid` query **均必填**。
- **调用方范围**：官方注明「支持批次创建商户号与批次发放商户调用」。
- **幂等**：官方注明「接口支持幂等重入」；频率限制 1000/s、单 IP 500/s。

### 3. `StartCouponStockAsync` — 激活代金券批次。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/start-stock.html。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`POST /v3/marketing/favor/stocks/{stockId}/start`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 批次号（官方 path `stock_id`，必填 string(20)）。 |

- **请求体**：`CouponStockOperationRequest` — 操作请求体（仅 `stock_creator_mchid`），见 `CouponStockOperationRequest`。
- **返回**：`CouponStockStartResponse` — 激活时间与批次号，见 `CouponStockStartResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/favor/stocks/{stock_id}/start`（更新时间 2024.09.19）。
- **官方原文**：免充值券激活**不扣款**，预充值券激活会锁定对应金额至营销资金池；接口支持**幂等**重入。

### 4. `PauseCouponStockAsync` — 暂停代金券批次。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/pause-stock.html。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`POST /v3/marketing/favor/stocks/{stockId}/pause`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 批次号（官方 path `stock_id`，必填 string(20)）。 |

- **请求体**：`CouponStockOperationRequest` — 操作请求体，见 `CouponStockOperationRequest`。
- **返回**：`CouponStockPauseResponse` — 暂停时间与批次号，见 `CouponStockPauseResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/favor/stocks/{stock_id}/pause`。
- **官方原文**：暂停后该批次**暂停发放**，用户**无法通过任何渠道**再领取该批次的券； / 接口支持幂等重入。**注意**「暂停」不同于「终止」—— 暂停可经 / `RestartCouponStockAsync` 恢复发放，而已 `stop_time` 终止的批次不可恢复。

### 5. `RestartCouponStockAsync` — 重启代金券批次。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/restart-stock.html。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`POST /v3/marketing/favor/stocks/{stockId}/restart`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 批次号（官方 path `stock_id`，必填 string(20)）。 |

- **请求体**：`CouponStockOperationRequest` — 操作请求体，见 `CouponStockOperationRequest`。
- **返回**：`CouponStockRestartResponse` — 重启时间与批次号，见 `CouponStockRestartResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/favor/stocks/{stock_id}/restart`。
- **官方原文**：通过此接口可重启指定代金券批次；重启后该批次**可以再次发放**；接口支持幂等重入。

### 6. `QueryCouponStockAsync` — 查询批次详情。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/query-stock.html。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/stocks/{stockId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 批次号（官方 path `stock_id`，必填 string(20)）。 |
| `stockCreatorMchId` | Query `stock_creator_mchid` | `string` | 创建批次的商户号（官方 query `stock_creator_mchid`，必填 string(20)）：接口传入的批次号须由该商户号创建。 |
- **返回**：`CouponStockQueryResponse` — 批次详情（含状态与使用规则），见 `CouponStockQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/stocks/{stock_id}?stock_creator_mchid=…`； / path 与 query **两者均必填**（更新时间 2025.03.25）。
- **路径为何没有 `coupon-` 前缀**（与创建批次的 `/coupon-stocks` 不同）： / 创建是 `POST /v3/marketing/favor/coupon-stocks`，而批次管理的四个动作都在 / `/v3/marketing/favor/stocks/…` 下 —— **两族路径前缀不一致**，官方原文如此，勿「统一」。
- **用途**：发放前先查 `status`（须为 `running`）；官方发券接口的「非法的批次状态」错误即源于此。

### 7. `IssueCouponAsync` — 发放指定批次的代金券。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463767。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`POST /v3/marketing/favor/users/{openId}/coupons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openId` | Path | `string` | 用户 openid（官方 path `openid`，必填 string(128)）：须与请求体的 `appid` 对应。 |

- **请求体**：`CouponIssueRequest` — 发放请求体，见 `CouponIssueRequest`。
- **返回**：`CouponIssueResponse` — 代金券 id，见 `CouponIssueResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/favor/users/{openid}/coupons`；**无 query 参数**。
- **前置**：批次须为**已激活且运营中**（`status = running`）—— 否则官方报 / `param_error`「非法的批次状态」。
- **幂等**：由请求体的 `out_request_no` 承担（商户侧凭据号）⇒ 网络重试时 / **必须**沿用同一个 `out_request_no`，否则可能重复发券。

### 8. `SetCouponCallbackAsync` — 设置代金券回调通知 url。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012464198（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012464176（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`POST /v3/marketing/favor/callbacks`

- **请求体**：`CouponCallbackSetRequest` — 设置代金券回调通知 url请求体，字段见 `CouponCallbackSetRequest`。
- **返回**：`CouponCallbackSetResponse` — 设置代金券回调通知 url，见 `CouponCallbackSetResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/favor/callbacks`；无 path / query 参数。
- **频率限制**（官方原样）：接口频率： 不区分来源 1000/min 单ip 5/min。
- **应答**见 `CouponCallbackSetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 9. `QueryCouponCallbackAsync` — 查询代金券回调通知 url。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012464070（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012464155（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/callbacks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `mchid` | Query `mchid` | `string` | 【商户号】 微信支付商户号（官方 query `mchid`，**必填**，类型 `string(20)`）。 |
- **返回**：`CouponCallbackQueryResponse` — 查询代金券回调通知 url，见 `CouponCallbackQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/callbacks`；query `mchid`。
- **频率限制**（官方原样）：接口频率： 2000QPS。
- **`mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 10. `QueryCouponStocksAsync` — 条件查询代金券批次列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012460489（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012460518（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/stocks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `offset` | Query `offset` | `int` | 【分页页码】页码从0开始，默认第0页（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】分页大小，最大10（官方 query `limit`，**必填**，类型 `integer`）。 |
| `stockCreatorMchid` | Query `stock_creator_mchid` | `string` | 【创建批次的商户号】批次创建方商户号。 校验规则：接口传入的批次号需由stock_creator_mchid所创建。（官方 query `stock_creator_mchid`，**必填**，类型 `string(20)`）。 |
| `createStartTime` | Query `create_start_time` | `string?` | 【起始时间】 起始创建时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 校验规则：get请求，参数在 url中，需要进行 url 编码传递（官方 query `create_start_time`，选填，类型 `string(64)`）。 |
| `createEndTime` | Query `create_end_time` | `string?` | 【终止时间】 终止创建时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒。 校验规则：get请求，参数在 url中，需要进行 url 编码传递（官方 query `create_end_time`，选填，类型 `string(664)`）。 |
| `status` | Query `status` | `string?` | 【批次状态】 批次状态，枚举值： unactivated：未激活 audit：审核中 running：运行中 stoped：已停止 paused：暂停发放（官方 query `status`，选填，类型 `string(12)`）。 |
- **返回**：`CouponStockListResponse` — 条件查询代金券批次列表，见 `CouponStockListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/stocks`；query `offset` / `limit` / `stock_creator_mchid` / `create_start_time` / `create_end_time` / `status`。
- **频率限制**（官方原样）：接口频率： 不区分来源 1000/s 单ip 500/s；接口频率：不区分来源 1000/s 单ip 500/s。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。

### 11. `QueryCouponStockItemsAsync` — 查询代金券可用单品。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463442（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012463475（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/stocks/{stockId}/items`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】 批次号 校验规则：必须为代金券（全场券或单品券）批次号，不支持立减与折扣。（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
| `offset` | Query `offset` | `int` | 【分页页码】 分页页码，最大500（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小，最大100（官方 query `limit`，**必填**，类型 `integer`）。 |
| `stockCreatorMchid` | Query `stock_creator_mchid` | `string` | 【创建批次的商户号】 批次创建方商户号。 校验规则：接口传入的批次号需由stock_creator_mchid所创建（官方 query `stock_creator_mchid`，**必填**，类型 `string(20)`）。 |
- **返回**：`CouponStockItemsResponse` — 查询代金券可用单品，见 `CouponStockItemsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/stocks/{stock_id}/items`；path `stock_id`，query `offset` / `limit` / `stock_creator_mchid`。
- **频率限制**（官方原样）：接口频率：不区分来源 1000/min，单ip 200/min；接口频率：不区分来源 1000/min，单ip 200/min。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。

### 12. `QueryCouponStockMerchantsAsync` — 查询代金券可用商户。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463358（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012463409（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/stocks/{stockId}/merchants`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】 微信为每个批次分配的唯一id 校验规则：必须为代金券（全场券或单品券）批次号，不支持立减与折扣（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
| `offset` | Query `offset` | `int` | 【分页页码】 分页页码，最大1000（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小，最大50（官方 query `limit`，**必填**，类型 `integer`）。 |
| `stockCreatorMchid` | Query `stock_creator_mchid` | `string` | 【创建批次的商户号】 批次创建方商户号。 校验规则：接口传入的批次号需由stock_creator_mchid所创建。（官方 query `stock_creator_mchid`，**必填**，类型 `string(20)`）。 |
- **返回**：`CouponStockMerchantsResponse` — 查询代金券可用商户，见 `CouponStockMerchantsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/stocks/{stock_id}/merchants`；path `stock_id`，query `offset` / `limit` / `stock_creator_mchid`。
- **频率限制**（官方原样）：接口频率：不区分来源 1000/min，单ip 200/min；接口频率：不区分来源 1000/min，单ip 200/min。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。

### 13. `QueryCouponStockRefundFlowAsync` — 下载批次退款明细。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463523（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012463548（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/stocks/{stockId}/refund-flow`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】 微信为每个代金券批次分配的唯一id。 校验规则： 1、支持全场券;单品券;立减券;折扣券 2、需要是调用创建方创建的批次号（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
- **返回**：`CouponStockRefundFlowResponse` — 下载批次退款明细，见 `CouponStockRefundFlowResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/stocks/{stock_id}/refund-flow`；path `stock_id`。
- **频率限制**（官方原样）：接口频率：不区分来源 1000/min ，单ip 5/min；接口频率：不区分来源 1000/min ，单ip 5/min。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。

### 14. `QueryCouponStockUseFlowAsync` — 下载批次核销明细。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012463585（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012463698（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/stocks/{stockId}/use-flow`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】 微信为每个代金券批次分配的唯一id。 校验规则： 1、支持全场券;单品券;立减券;折扣券 2、需要是调用创建方创建的批次号（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
- **返回**：`CouponStockUseFlowResponse` — 下载批次核销明细，见 `CouponStockUseFlowResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/stocks/{stock_id}/use-flow`；path `stock_id`。
- **频率限制**（官方原样）：接口频率：不区分来源 1000/min， 单ip 5/min；接口频率：不区分来源 1000/min， 单ip 5/min。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。

### 15. `QueryCouponUserCouponsAsync` — 条件查询用户代金券列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534690（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012494237（普通服务商）。

- **声明接口**：[IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs)
- **官方契约**：`GET /v3/marketing/favor/users/{openid}/coupons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户标识】 用户在商户appid下的唯一标识。 校验规则：该openid需要与接口传入中的appid有对应关系。（官方 path `openid`，**必填**，类型 `string(128)`）。 |
| `appid` | Query `appid` | `string` | 【公众账号ID】 微信为发券方商户分配的公众账号ID，接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的）或APP的appid（在open.weixin.qq.com申请的）。 校验规则： 1、该appid需要与接口传入中的openid有对应关系； 2、该appid需要与调用接口的商户号（即请求头中的商户号）有绑定关系，若未绑定，可参考该指引完成绑定（ 商家商户号与AppID账号关联管理）（官方 query `appid`，**必填**，类型 `string(128)`）。 |
| `stockId` | Query `stock_id` | `string?` | 【批次号】 批次号，是否指定批次号查询，填写available_mchid，该字段不生效。（官方 query `stock_id`，选填，类型 `string(20)`）。 |
| `status` | Query `status` | `string?` | 【券或消费金状态】 代金券或消费金状态： 选填creator_mchid时， SENDED：返回可用 USED：返回可用+已实扣 选填available_mchid时，该字段不生效，仅返回 可用 状态的券或消费金。（官方 query `status`，选填，类型 `string(6)`）。 |
| `businessType` | Query `business_type` | `string?` | 【业务类型】 若选择MULTIUSE，则仅返回查询用户拥有的消费金列表 枚举值： MULTIUSE：消费金。可选取值：`MULTIUSE` 消费金类型（官方 query `business_type`，选填，类型 `string`）。 |
| `creatorMchid` | Query `creator_mchid` | `string?` | 【创建批次的商户号】 批次创建方商户号。请求参数传创建商户号返回除过期以外所有状态的用户券。creator_mchid与available_mchid二选一，如果同时存在，优先使用creator_mchid。（官方 query `creator_mchid`，选填，类型 `string(20)`）。 |
| `availableMchid` | Query `available_mchid` | `string?` | 【可用商户号】 可用商户号请求参数传可用商户号只能返回可用的代金券，实扣的与过期的无法返回。creator_mchid与available_mchid二选一，如果同时存在，优先使用creator_mchid。（官方 query `available_mchid`，选填，类型 `string(20)`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 分页页码，默认0，填写available_mchid，该字段不生效（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小，默认20，填写available_mchid，该字段不生效（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`CouponUserCouponListResponse` — 条件查询用户代金券列表，见 `CouponUserCouponListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/favor/users/{openid}/coupons`；path `openid`，query `appid` / `stock_id` / `status` / `business_type` / `creator_mchid` / `available_mchid` / `offset` / `limit`。
- **频率限制**（官方原样）：接口频率：不区分来源 2000/s 单ip 500/s。
- **`appid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 INVALID_REQUEST 活动已结束或未激活 / 400 INVALID_REQUEST 非法的商户号 / 400 MCH_NOT_EXISTS 商户号不合法 / 400 PARAM_ERROR 回调URL不能为空 / 400 PARAM_ERROR 回调商户不能为空 / 400 PARAM_ERROR 券ID必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 页大小超过阈值 / 400 PARAM_ERROR 输入时间格式错误 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 商户号必填 / 400 PARAM_ERROR 非法的批次状态 / 403 NOT_ENOUGH 批次预算不足 / 403 REQUEST_BLOCKED 调用商户无权限 / 403 REQUEST_BLOCKED 商户无权发券 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 403 REQUEST_BLOCKED 用户被限领拦截 / 403 USER_ACCOUNT_ABNORMAL 用户非法 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 429 FREQUENCY_LIMITED 请求过于频繁。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MarketingFavor`](../../DataModels/MarketingFavor) 内共 **41** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **41** 个，`SerializerClassName` = `MarketingFavor`。

AOT 源生成上下文：[`MarketingFavorJsonContext.g.cs`](./../../DataModels/Generated/MarketingFavorJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CouponStockCreateResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponStockCreateRequest` | 请求体 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponQueryResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponStockStartResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponStockOperationRequest` | 请求体 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponStockPauseResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponStockRestartResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponStockQueryResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponIssueResponse` | 应答 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponIssueRequest` | 请求体 | [MarketingFavor/MarketingFavorDtos.cs](../../DataModels/MarketingFavor/MarketingFavorDtos.cs) | `stock_name`, `comment`, `belong_merchant`, `available_begin_time`, `available_end_time`, `stock_use_rule`, `pattern_info`, `coupon_use_rule`, `no_cash`, `stock_type`, `out_request_no`, `ext_info` … |
| `CouponCallbackSetResponse` | 应答 | [MarketingFavor/MarketingFavorCallbackDtos.cs](../../DataModels/MarketingFavor/MarketingFavorCallbackDtos.cs) | `mchid`, `notify_url`, `switch`, `appid`, `update_time` |
| `CouponCallbackSetRequest` | 请求体 | [MarketingFavor/MarketingFavorCallbackDtos.cs](../../DataModels/MarketingFavor/MarketingFavorCallbackDtos.cs) | `mchid`, `notify_url`, `switch`, `appid`, `update_time` |
| `CouponCallbackQueryResponse` | 应答 | [MarketingFavor/MarketingFavorCallbackDtos.cs](../../DataModels/MarketingFavor/MarketingFavorCallbackDtos.cs) | `mchid`, `notify_url`, `switch`, `appid`, `update_time` |
| `CouponStockListResponse` | 应答 | [MarketingFavor/MarketingFavorStockDtos.cs](../../DataModels/MarketingFavor/MarketingFavorStockDtos.cs) | `coupon_amount`, `transaction_minimum`, `discount_amount_max`, `discount_percent`, `max_coupons`, `max_amount`, `max_amount_by_day`, `fixed_normal_coupon`, `max_coupons_per_user`, `coupon_type`, `goods_tag`, `trade_type` … |
| `CouponStockItemsResponse` | 应答 | [MarketingFavor/MarketingFavorStockDtos.cs](../../DataModels/MarketingFavor/MarketingFavorStockDtos.cs) | `coupon_amount`, `transaction_minimum`, `discount_amount_max`, `discount_percent`, `max_coupons`, `max_amount`, `max_amount_by_day`, `fixed_normal_coupon`, `max_coupons_per_user`, `coupon_type`, `goods_tag`, `trade_type` … |
| `CouponStockMerchantsResponse` | 应答 | [MarketingFavor/MarketingFavorStockDtos.cs](../../DataModels/MarketingFavor/MarketingFavorStockDtos.cs) | `coupon_amount`, `transaction_minimum`, `discount_amount_max`, `discount_percent`, `max_coupons`, `max_amount`, `max_amount_by_day`, `fixed_normal_coupon`, `max_coupons_per_user`, `coupon_type`, `goods_tag`, `trade_type` … |
| `CouponStockRefundFlowResponse` | 应答 | [MarketingFavor/MarketingFavorStockDtos.cs](../../DataModels/MarketingFavor/MarketingFavorStockDtos.cs) | `coupon_amount`, `transaction_minimum`, `discount_amount_max`, `discount_percent`, `max_coupons`, `max_amount`, `max_amount_by_day`, `fixed_normal_coupon`, `max_coupons_per_user`, `coupon_type`, `goods_tag`, `trade_type` … |
| `CouponStockUseFlowResponse` | 应答 | [MarketingFavor/MarketingFavorStockDtos.cs](../../DataModels/MarketingFavor/MarketingFavorStockDtos.cs) | `coupon_amount`, `transaction_minimum`, `discount_amount_max`, `discount_percent`, `max_coupons`, `max_amount`, `max_amount_by_day`, `fixed_normal_coupon`, `max_coupons_per_user`, `coupon_type`, `goods_tag`, `trade_type` … |
| `CouponUserCouponListResponse` | 应答 | [MarketingFavor/MarketingFavorUserDtos.cs](../../DataModels/MarketingFavor/MarketingFavorUserDtos.cs) | `single_price_max`, `cut_to_price`, `coupon_amount`, `transaction_minimum`, `discount_amount_max`, `discount_percent`, `stock_creator_mchid`, `stock_id`, `coupon_id`, `cut_to_message`, `coupon_name`, `status` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingFavor;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingFavor));

var service = app.Services.GetRequiredService<IWechatPayMarketingFavorService>();
var ct = CancellationToken.None;
var response = await service.CreateCouponStockAsync(new CouponStockCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/favor/coupon-stocks");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingFavorService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateCouponStockAsync`）；请求体 `CouponStockCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingFavorService](./IWechatPayMarketingFavorService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingFavorContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingFavorContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayLovefeastContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayLovefeastContractGuards.cs)、[`WechatPayMultiuseContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMultiuseContractGuards.cs) |
| 守卫中的代表性测试方法 | `MarketingFavorEndpoints_ShouldMatchOfficialRoutes`、`MarketingFavorDtos_ShouldExposeOfficialFieldNames`、`CouponUseRuleShapes_ShouldStayStructurallySeparate`、`MarketingFavorDataModels_ShouldBeRegisteredInJsonContext`、`MarketingFavorIncrementEndpoints_ShouldMatchOfficialRoutes`、`MarketingFavorIncrementDtos_ShouldExposeOfficialFieldRosters`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`LovefeastTopology_ShouldMatchInterfaceAndGroupRoster` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MarketingFavor`](../../DataModels/MarketingFavor) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingFavorContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayLovefeastContractGuards.cs`、`WechatPayMultiuseContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
