# MarketingBusifavor 域 · 商家券

微信支付「商家券」域 SDK（15 端点）。 接口：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)；注册组 `MarketingBusifavor`；模块枚举 `PayModule.MarketingBusifavor`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingBusifavorContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBusifavorContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MarketingBusifavor`](../../DataModels/MarketingBusifavor)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingBusifavorService = 15；按 HTTP 特性实计 **16** 支。

> **计数漂移（留档）**：`IWechatPayMarketingBusifavorService` 注释自述 15 支、实计 16 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingBusifavorService` | `QueryBusifavorCallbackAsync` | GET | `/v3/marketing/busifavor/callbacks` | 商户证书 / APIv3 签名 | 商家券回调地址。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465507（普通商户）、https://pay.weixi… |
| 2 | `IWechatPayMarketingBusifavorService` | `SetBusifavorCallbackAsync` | POST | `/v3/marketing/busifavor/callbacks` | 商户证书 / APIv3 签名 | 设置商家券回调地址。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465431（普通商户）、https://pay.wei… |
| 3 | `IWechatPayMarketingBusifavorService` | `AssociateBusifavorCouponAsync` | POST | `/v3/marketing/busifavor/coupons/associate` | 商户证书 / APIv3 签名 | 商家券关联订单补贴。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465596（普通商户）、https://pay.wei… |
| 4 | `IWechatPayMarketingBusifavorService` | `DisassociateBusifavorCouponAsync` | POST | `/v3/marketing/busifavor/coupons/disassociate` | 商户证书 / APIv3 签名 | 商家券取消关联订单补贴。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465673（普通商户）、https://pay.w… |
| 5 | `IWechatPayMarketingBusifavorService` | `DeactivateBusifavorCouponAsync` | POST | `/v3/marketing/busifavor/coupons/deactivate` | 商户证书 / APIv3 签名 | 作废商家券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465890（普通商户）、https://pay.weixin.… |
| 6 | `IWechatPayMarketingBusifavorService` | `ReturnBusifavorCouponAsync` | POST | `/v3/marketing/busifavor/coupons/return` | 商户证书 / APIv3 签名 | 商家券退款退券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465845（普通商户）、https://pay.weixi… |
| 7 | `IWechatPayMarketingBusifavorService` | `UseBusifavorCouponAsync` | POST | `/v3/marketing/busifavor/coupons/use` | 商户证书 / APIv3 签名 | 核销商家券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465292（普通商户）、https://pay.weixin.… |
| 8 | `IWechatPayMarketingBusifavorService` | `SendBusifavorProductCouponAsync` | POST | `/v3/marketing/busifavor/product-coupons/send` | 商户证书 / APIv3 签名 | 向用户发放商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4015717371（普通商户）。 |
| 9 | `IWechatPayMarketingBusifavorService` | `CreateBusifavorStockAsync` | POST | `/v3/marketing/busifavor/stocks` | 商户证书 / APIv3 签名 | 创建商家券批次。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534855（普通商户）、https://pay.weixi… |
| 10 | `IWechatPayMarketingBusifavorService` | `QueryBusifavorStockAsync` | GET | `/v3/marketing/busifavor/stocks/{stockId}` | 商户证书 / APIv3 签名 | 查询商家券批次详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534928（普通商户）、https://pay.wei… |
| 11 | `IWechatPayMarketingBusifavorService` | `UpdateBusifavorStockAsync` | PATCH | `/v3/marketing/busifavor/stocks/{stockId}` | 商户证书 / APIv3 签名 | 修改商家券批次。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012535020（普通商户）、https://pay.weixi… |
| 12 | `IWechatPayMarketingBusifavorService` | `UpdateBusifavorStockBudgetAsync` | PATCH | `/v3/marketing/busifavor/stocks/{stockId}/budget` | 商户证书 / APIv3 签名 | 修改商家券批次预算。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465739（普通商户）、https://pay.wei… |
| 13 | `IWechatPayMarketingBusifavorService` | `UploadBusifavorCouponCodesAsync` | POST | `/v3/marketing/busifavor/stocks/{stockId}/couponcodes` | 商户证书 / APIv3 签名 | 预上传商家券自定义券码。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012487038（普通商户）、https://pay.w… |
| 14 | `IWechatPayMarketingBusifavorService` | `QueryBusifavorUserCouponsAsync` | GET | `/v3/marketing/busifavor/users/{openid}/coupons` | 商户证书 / APIv3 签名 | 用户商家券列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534963（普通商户）、https://pay.weixi… |
| 15 | `IWechatPayMarketingBusifavorService` | `QueryBusifavorUserCouponAsync` | GET | `/v3/marketing/busifavor/users/{openid}/coupons/{couponCode}/appids/{appid}` | 商户证书 / APIv3 签名 | 用户商家券详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012535011（普通商户）、https://pay.weixi… |
| 16 | `IWechatPayMarketingBusifavorService` | `SendBusifavorConsumeCardAsync` | POST | `/v3/marketing/busifavor/coupons/{cardId}/send` | 商户证书 / APIv3 签名 | 发放消费卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4017164666（【直连商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对**完全同形**
⇒ **一面一方法 + 双页文档引用**，只有字段表存在差异的端点才按「同路由多方法」分建。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**分页口径不统一**：本域既有 `offset`/`limit`（整型页码）也有 `page_size` + `offset` 形态，
一律**照页**，不得互相套用；应答的 `total_count` 官方多为选填（`offset=0` 或结果为空时才回）。

**PATCH 与 PUT**：批次/活动的改预算与改状态走 **PATCH**，终止走 **POST .../terminate**
⇒ 「写操作动词」在本域不统一，照页不得合并。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 14 次）**支持商户**【普通商户】【普通服务商】 【渠道商】。
- （出现 12 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 8 次）**业务错误码**：400 APPID_MCHID_NOT_MATCH AppID与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 INVALID_REQUEST 上传的自定义code已达上限 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 批次已存在 / 400 RESOURCE_ALREADY_EXISTS 券已被其他订单核销 / 400 SYSTEM_ERROR 系统错误 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券不在有效期内 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。

### 1. `QueryBusifavorCallbackAsync` — 商家券回调地址。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465507（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465557（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`GET /v3/marketing/busifavor/callbacks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `mchid` | Query `mchid` | `string?` | 【商户号】商户号，不填默认查询调用方商户号（官方 query `mchid`，选填，类型 `string(15)`）。 |
- **返回**：`BusifavorCallbackQueryResponse` — 商家券回调地址，见 `BusifavorCallbackQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/busifavor/callbacks`；query `mchid`。

### 2. `SetBusifavorCallbackAsync` — 设置商家券回调地址。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465431（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465425（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/callbacks`

- **请求体**：`BusifavorCallbackSetRequest` — 设置商家券回调地址请求体，字段见 `BusifavorCallbackSetRequest`。
- **返回**：`BusifavorCallbackSetResponse` — 设置商家券回调地址，见 `BusifavorCallbackSetResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/callbacks`；无 path / query 参数。
- **应答**见 `BusifavorCallbackSetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 3. `AssociateBusifavorCouponAsync` — 商家券关联订单补贴。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465596（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465640（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/coupons/associate`

- **请求体**：`BusifavorCouponAssociateRequest` — 商家券关联订单补贴请求体，字段见 `BusifavorCouponAssociateRequest`。
- **返回**：`BusifavorCouponAssociateResponse` — 商家券关联订单补贴，见 `BusifavorCouponAssociateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/coupons/associate`；无 path / query 参数。
- **频率限制**（官方原样）：频率：500QPS。
- **应答**见 `BusifavorCouponAssociateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 INVALID_REQUEST 券不存在 / 400 INVALID_REQUEST 券状态非可用态 / 400 INVALID_REQUEST 券已被其他单号关联 / 400 INVALID_REQUEST 券已被其他单号关联，请用正确单号重试 / 400 INVALID_REQUEST 商户无权限操作该券。

### 4. `DisassociateBusifavorCouponAsync` — 商家券取消关联订单补贴。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465673（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465703（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/coupons/disassociate`

- **请求体**：`BusifavorCouponDisassociateRequest` — 商家券取消关联订单补贴请求体，字段见 `BusifavorCouponDisassociateRequest`。
- **返回**：`BusifavorCouponDisassociateResponse` — 商家券取消关联订单补贴，见 `BusifavorCouponDisassociateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/coupons/disassociate`；无 path / query 参数。
- **频率限制**（官方原样）：频率：500QPS。
- **应答**见 `BusifavorCouponDisassociateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 INVALID_REQUEST 券不存在 / 400 INVALID_REQUEST 券状态非可用态 / 400 INVALID_REQUEST 券已被其他单号关联 / 400 INVALID_REQUEST 券已被其他单号关联，请用正确单号重试 / 400 INVALID_REQUEST 商户无权限操作该券。

### 5. `DeactivateBusifavorCouponAsync` — 作废商家券。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465890（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465924（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/coupons/deactivate`

- **请求体**：`BusifavorCouponDeactivateRequest` — 作废商家券请求体，字段见 `BusifavorCouponDeactivateRequest`。
- **返回**：`BusifavorCouponDeactivateResponse` — 作废商家券，见 `BusifavorCouponDeactivateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/coupons/deactivate`；无 path / query 参数。
- **应答**见 `BusifavorCouponDeactivateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：403 RULE_LIMIT 券不属于指定的批次 / 403 RULE_LIMIT 仅批次创建方有权限使券失效 / 403 RULE_LIMIT 券已被核销 / 403 RULE_LIMIT 券已过期，不能再进行其他操作 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 404 RESOURCE_NOT_EXISTS 券不存在 / 404 RESOURCE_NOT_EXISTS 券已被删除。

### 6. `ReturnBusifavorCouponAsync` — 商家券退款退券。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465845（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465865（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/coupons/return`

- **请求体**：`BusifavorCouponReturnRequest` — 商家券退款退券请求体，字段见 `BusifavorCouponReturnRequest`。
- **返回**：`BusifavorCouponReturnResponse` — 商家券退款退券，见 `BusifavorCouponReturnResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/coupons/return`；无 path / query 参数。
- **应答**见 `BusifavorCouponReturnResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 RESOURCE_ALREADY_EXISTS 已使用过此单号退券 / 403 NO_AUTH 商户无权限操作该券 / 403 RULE_LIMIT 券当前是可用的 / 403 RULE_LIMIT 券已不在使用有效期 / 404 RESOURCE_NOT_EXISTS 券不存在 / 404 RESOURCE_NOT_EXISTS 券已被删除。

### 7. `UseBusifavorCouponAsync` — 核销商家券。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465292（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465359（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/coupons/use`

- **请求体**：`BusifavorCouponUseRequest` — 核销商家券请求体，字段见 `BusifavorCouponUseRequest`。
- **返回**：`BusifavorCouponUseResponse` — 核销商家券，见 `BusifavorCouponUseResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/coupons/use`；无 path / query 参数。
- **频率限制**（官方原样）：核销频率：500QPS。
- **应答**见 `BusifavorCouponUseResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 8. `SendBusifavorProductCouponAsync` — 向用户发放商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4015717371（普通商户）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/product-coupons/send`

- **请求体**：`BusifavorProductCouponSendRequest` — 向用户发放商品券请求体，字段见 `BusifavorProductCouponSendRequest`。
- **返回**：`BusifavorProductCouponSendResponse` — 向用户发放商品券，见 `BusifavorProductCouponSendResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/product-coupons/send`；无 path / query 参数。
- **支持商户**【普通服务商】　【渠道商】。
- **应答**见 `BusifavorProductCouponSendResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH AppID 与请求方商户无关联关系 / 400 INVALID_REQUEST 发券模式不合法 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 RESOURCE_ALREADY_EXISTS 发券凭证已存在 / 400 STOCK_NOT_EXISTS 批次不存在 / 400 STOCK_STATE_ERROR 批次状态错误 / 400 STOCK_EXPIRED 批次已过期 / 400 STOCK_INSUFFICIENT 批次库存不足 / 400 STOCK_NO_PRODUCT 批次未关联商品信息 / 400 USER_ALREADY_RECEIVED 用户已领取 / 400 USER_RECEIVE_LIMIT 用户领取限制 / 403 NO_AUTH 无权限 / 403 RULE_LIMIT 券批次未关联商品信息 / 403 RULE_LIMIT 品牌未开通名片功能 / 403 RULE_LIMIT 用户未开通会员功能 / 404 RESOURCE_NOT_EXISTS 查询的资源不存在 / 404 USER_NOT_EXISTS OpenID不正确 / 429 FREQUENCY_LIMITED 频率限制 / 500 SYSTEM_ERROR 系统失败。

### 9. `CreateBusifavorStockAsync` — 创建商家券批次。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534855（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012692931（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/stocks`

- **请求体**：`BusifavorStockCreateRequest` — 创建商家券批次请求体，字段见 `BusifavorStockCreateRequest`。
- **返回**：`BusifavorStockCreateResponse` — 创建商家券批次，见 `BusifavorStockCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/stocks`；无 path / query 参数。
- **频率限制**（官方原样）：频率限制：接口级限制1000/min。
- **应答**见 `BusifavorStockCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 10. `QueryBusifavorStockAsync` — 查询商家券批次详情。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534928（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012693047（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`GET /v3/marketing/busifavor/stocks/{stockId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】微信为每个商家券批次分配的唯一ID（官方 path `stock_id`，**必填**，类型 `string`）。 |
- **返回**：`BusifavorStockQueryResponse` — 查询商家券批次详情，见 `BusifavorStockQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/busifavor/stocks/{stock_id}`；path `stock_id`。

### 11. `UpdateBusifavorStockAsync` — 修改商家券批次。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012535020（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012696764（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`PATCH /v3/marketing/busifavor/stocks/{stockId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】批次号（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`BusifavorStockUpdateRequest` — 修改商家券批次请求体，字段见 `BusifavorStockUpdateRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/busifavor/stocks/{stock_id}`；path `stock_id`。
- **业务错误码**：400 INVALID_REQUEST 修改跳转到小程序的自定义入口请填入小程序AppID / 400 INVALID_REQUEST 修改字段中包含敏感词 / 400 INVALID_REQUEST 修改的AppID与商户号没有绑定关系 / 400 INVALID_REQUEST 修改的图片URL包含敏感内容 / 403 RULE_LIMIT 修改流程中仅允许创建方进行操作 / 403 RULE_LIMIT 修改的营销馆无权限 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 404 RESOURCE_NOT_EXISTS 修改的AppID不合法 / 404 RESOURCE_NOT_EXISTS 营销馆ID不存在。

### 12. `UpdateBusifavorStockBudgetAsync` — 修改商家券批次预算。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012465739（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012465809（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`PATCH /v3/marketing/busifavor/stocks/{stockId}/budget`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】批次号（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`BusifavorStockBudgetRequest` — 修改商家券批次预算请求体，字段见 `BusifavorStockBudgetRequest`。
- **返回**：`BusifavorStockBudgetResponse` — 修改商家券批次预算，见 `BusifavorStockBudgetResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/busifavor/stocks/{stock_id}/budget`；path `stock_id`。
- **应答**见 `BusifavorStockBudgetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：403 RULE_LIMIT 批次库存的当前值错误，请获取批次详情查看最新值 / 403 RULE_LIMIT 仅创建方有权限使券失效 / 403 RULE_LIMIT 批次正在修改中，请稍后重试 / 403 RULE_LIMIT 目标库存数目不能小于批次已发放库存数目 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 404 RESOURCE_NOT_EXISTS 批次已过期 / 429 FREQUENCY_LIMITED 调用太频繁，请降低调用接口频率。

### 13. `UploadBusifavorCouponCodesAsync` — 预上传商家券自定义券码。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012487038（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012492853（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/stocks/{stockId}/couponcodes`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】商家券批次号（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`BusifavorCouponCodesUploadRequest` — 预上传商家券自定义券码请求体，字段见 `BusifavorCouponCodesUploadRequest`。
- **返回**：`BusifavorCouponCodesUploadResponse` — 预上传商家券自定义券码，见 `BusifavorCouponCodesUploadResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/stocks/{stock_id}/couponcodes`；path `stock_id`。
- **应答**见 `BusifavorCouponCodesUploadResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 14. `QueryBusifavorUserCouponsAsync` — 用户商家券列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534963（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012693087（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`GET /v3/marketing/busifavor/users/{openid}/coupons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户标识】Openid信息，用户在AppID下的唯一标识。 校验规则：传入的OpenID得是调用方商户号（即请求头里面的商户号）有绑定关系的AppID获取的OpenID或传入的OpenID得是归属商户号有绑定关系的AppID获取的OpenID。获取OpenID文档（官方 path `openid`，**必填**，类型 `string`）。 |
| `appid` | Query `appid` | `string` | 【公众账号ID】支持传入与当前调用接口商户号有绑定关系的AppID。支持小程序AppID与公众号AppID。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID 或 传入的AppID得是归属商户号有绑定关系的AppID（官方 query `appid`，**必填**，类型 `string`）。 |
| `stockId` | Query `stock_id` | `string?` | 【批次号】批次号，是否指定批次号查询（官方 query `stock_id`，选填，类型 `string`）。 |
| `creatorMerchant` | Query `creator_merchant` | `string?` | 【创建批次的商户号】批次创建方商户号 校验规则：当调用方商户号（即请求头中的商户号）为创建批次方商户号时，该参数必传（官方 query `creator_merchant`，选填，类型 `string`）。 |
| `belongMerchant` | Query `belong_merchant` | `string?` | 【批次归属商户号】批次归属商户号 校验规则：当调用方商户号（即请求头中的商户号）为批次归属商户号时，该参数必传（官方 query `belong_merchant`，选填，类型 `string`）。 |
| `senderMerchant` | Query `sender_merchant` | `string?` | 【批次发放商户号】批次发放商户号 校验规则：当调用方商户号（即请求头中的商户号）为批次发放商户号时，该参数必传；委托营销关系下，请填写委托发券的商户号（官方 query `sender_merchant`，选填，类型 `string`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】分页页码（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】分页大小（官方 query `limit`，选填，类型 `integer`）。 |
| `couponState` | Query `coupon_state` | `string?` | 【券状态】商家券状态。可选取值：`SENDED` 可用 / `USED` 已核销 / `EXPIRED` 已过期 / `DELETED` 已删除 / `DEACTIVATED` 已失效（官方 query `coupon_state`，选填，类型 `string`）。 |
- **返回**：`BusifavorUserCouponListResponse` — 用户商家券列表，见 `BusifavorUserCouponListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/busifavor/users/{openid}/coupons`；path `openid`，query `appid` / `stock_id` / `creator_merchant` / `belong_merchant` / `sender_merchant` / `offset` / `limit` / `coupon_state`。
- **`appid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 15. `QueryBusifavorUserCouponAsync` — 用户商家券详情。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012535011（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012693103（普通服务商）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`GET /v3/marketing/busifavor/users/{openid}/coupons/{couponCode}/appids/{appid}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】OpenID信息，用户在AppID下的唯一标识。 校验规则：传入的OpenID得是调用方商户号（即请求头里面的商户号）有绑定关系的AppID获取的OpenID或传入的OpenID得是归属商户号有绑定关系的AppID获取的OpenID。获取OpenID文档（官方 path `openid`，**必填**，类型 `string`）。 |
| `couponCode` | Path | `string` | 【券code】券的唯一标识（官方 path `coupon_code`，**必填**，类型 `string`）。 |
| `appid` | Path | `string` | 【公众账号ID】支持传入与当前调用接口商户号有绑定关系的AppID。支持小程序AppID与公众号AppID。 校验规则：传入的AppID得是与调用方商户号（即请求头里面的商户号）有绑定关系的AppID或传入的AppID得是归属商户号有绑定关系的AppID（官方 path `appid`，**必填**，类型 `string`）。 |
- **返回**：`BusifavorUserCouponQueryResponse` — 用户商家券详情，见 `BusifavorUserCouponQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/busifavor/users/{openid}/coupons/{coupon_code}/appids/{appid}`；path `openid` / `coupon_code` / `appid`。

### 16. `SendBusifavorConsumeCardAsync` — 发放消费卡。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4017164666（【直连商户】）。

- **声明接口**：[IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs)
- **官方契约**：`POST /v3/marketing/busifavor/coupons/{cardId}/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【消费卡ID】 card_id，即创建消费卡后获得的唯一标识（官方 path `card_id`，**必填**，类型 `string[1,28]`）。 |

- **请求体**：`BusifavorConsumeCardSendRequest` — 发放消费卡请求体，字段见 `BusifavorConsumeCardSendRequest`。
- **返回**：`BusifavorConsumeCardSendResponse` — 发放消费卡，见 `BusifavorConsumeCardSendResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/busifavor/coupons/{card_id}/send`；path `card_id`，body 4 键（`appid`/`openid`/`out_request_no`/`send_time`），resp 1 键（`card_code`）。
- **支持商户**【直连商户】。
- **前置警示**：此功能仅向指定邀约商户开放，如有需要请联系微信支付运营经理。
- **业务错误码**：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MarketingBusifavor`](../../DataModels/MarketingBusifavor) 内共 **91** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **91** 个，`SerializerClassName` = `MarketingBusifavor`。

AOT 源生成上下文：[`MarketingBusifavorJsonContext.g.cs`](./../../DataModels/Generated/MarketingBusifavorJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BusifavorCallbackQueryResponse` | 应答 | [MarketingBusifavor/BusifavorCallbackDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCallbackDtos.cs) | `mchid`, `notify_url`, `update_time` |
| `BusifavorCallbackSetResponse` | 应答 | [MarketingBusifavor/BusifavorCallbackDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCallbackDtos.cs) | `mchid`, `notify_url`, `update_time` |
| `BusifavorCallbackSetRequest` | 请求体 | [MarketingBusifavor/BusifavorCallbackDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCallbackDtos.cs) | `mchid`, `notify_url`, `update_time` |
| `BusifavorCouponAssociateResponse` | 应答 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponAssociateRequest` | 请求体 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponDisassociateResponse` | 应答 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponDisassociateRequest` | 请求体 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponDeactivateResponse` | 应答 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponDeactivateRequest` | 请求体 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponReturnResponse` | 应答 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponReturnRequest` | 请求体 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponUseResponse` | 应答 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorCouponUseRequest` | 请求体 | [MarketingBusifavor/BusifavorCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorCouponDtos.cs) | `stock_id`, `coupon_code`, `out_trade_no`, `out_request_no`, `wechatpay_associate_time`, `wechatpay_disassociate_time`, `deactivate_request_no`, `deactivate_reason`, `wechatpay_deactivate_time`, `return_request_no`, `wechatpay_return_time`, `appid` … |
| `BusifavorProductCouponSendResponse` | 应答 | [MarketingBusifavor/BusifavorProductCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorProductCouponDtos.cs) | `openid`, `appid`, `stock_id`, `out_request_no`, `coupon_code`, `coupon_type`, `member_card_id`, `send_coupon_result` |
| `BusifavorProductCouponSendRequest` | 请求体 | [MarketingBusifavor/BusifavorProductCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorProductCouponDtos.cs) | `openid`, `appid`, `stock_id`, `out_request_no`, `coupon_code`, `coupon_type`, `member_card_id`, `send_coupon_result` |
| `BusifavorStockCreateResponse` | 应答 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorStockCreateRequest` | 请求体 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorStockQueryResponse` | 应答 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorStockUpdateRequest` | 请求体 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorStockBudgetResponse` | 应答 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorStockBudgetRequest` | 请求体 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorCouponCodesUploadResponse` | 应答 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorCouponCodesUploadRequest` | 请求体 | [MarketingBusifavor/BusifavorStockDtos.cs](../../DataModels/MarketingBusifavor/BusifavorStockDtos.cs) | `begin_time`, `end_time`, `week_day`, `available_day_time`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `available_week`, `irregulary_avaliable_time`, `wait_days_after_receive`, `available_second_after_receive`, `discount_amount` … |
| `BusifavorUserCouponListResponse` | 应答 | [MarketingBusifavor/BusifavorUserCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorUserCouponDtos.cs) | `finder_id`, `finder_video_id`, `finder_video_cover_image_url`, `description`, `merchant_logo_url`, `merchant_name`, `background_color`, `coupon_image_url`, `finder_info`, `begin_time`, `end_time`, `week_day` … |
| `BusifavorUserCouponQueryResponse` | 应答 | [MarketingBusifavor/BusifavorUserCouponDtos.cs](../../DataModels/MarketingBusifavor/BusifavorUserCouponDtos.cs) | `finder_id`, `finder_video_id`, `finder_video_cover_image_url`, `description`, `merchant_logo_url`, `merchant_name`, `background_color`, `coupon_image_url`, `finder_info`, `begin_time`, `end_time`, `week_day` … |
| `BusifavorConsumeCardSendResponse` | 应答 | [MarketingBusifavor/BusifavorConsumeCardDtos.cs](../../DataModels/MarketingBusifavor/BusifavorConsumeCardDtos.cs) | `appid`, `openid`, `out_request_no`, `send_time`, `card_code` |
| `BusifavorConsumeCardSendRequest` | 请求体 | [MarketingBusifavor/BusifavorConsumeCardDtos.cs](../../DataModels/MarketingBusifavor/BusifavorConsumeCardDtos.cs) | `appid`, `openid`, `out_request_no`, `send_time`, `card_code` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingBusifavor;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingBusifavor));

var service = app.Services.GetRequiredService<IWechatPayMarketingBusifavorService>();
var ct = CancellationToken.None;
var response = await service.AssociateBusifavorCouponAsync(new BusifavorCouponAssociateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/busifavor/coupons/associate");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingBusifavorService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AssociateBusifavorCouponAsync`）；请求体 `BusifavorCouponAssociateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IWechatPayMarketingBusifavorService`：注释 15 / 实计 16 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingBusifavorService](./IWechatPayMarketingBusifavorService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingBusifavorContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBusifavorContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BusifavorTopology_ShouldBeOneInterfaceOnOneGroup`、`BusifavorEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BusifavorDtos_ShouldExposeOfficialFieldRosters`、`BusifavorDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MarketingBusifavor`](../../DataModels/MarketingBusifavor) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingBusifavorContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
