# MarketingPartnerProductCoupon 域 · 服务商商品券

微信支付「服务商商品券」域 SDK（34 端点）。 接口：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)；注册组 `MarketingPartnerProductCoupon`；模块枚举 `PayModule.MarketingPartnerProductCoupon`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingPartnerProductCouponContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPartnerProductCouponContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MarketingPartnerProductCoupon`](../../DataModels/MarketingPartnerProductCoupon)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingPartnerProductCouponService = 34；按 HTTP 特性实计 **33** 支。

> **计数漂移（留档）**：`IWechatPayMarketingPartnerProductCouponService` 注释自述 34 支、实计 33 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingPartnerProductCouponService` | `CreateProductCouponImageTaskAsync` | POST | `/v3/marketing/partner/product-coupon/image-generation-tasks` | 商户证书 / APIv3 签名 | 创建商品券图片生成任务。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4017327735（普通服务商）、https://pay.w… |
| 2 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponImageTaskAsync` | GET | `/v3/marketing/partner/product-coupon/image-generation-tasks/{taskId}` | 商户证书 / APIv3 签名 | 查询商品券图片生成任务。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4017327739（普通服务商）、https://pay.w… |
| 3 | `IWechatPayMarketingPartnerProductCouponService` | `SetProductCouponNotifyConfigAsync` | POST | `/v3/marketing/partner/product-coupon/notify-configs` | 商户证书 / APIv3 签名 | 设置商品券通知回调。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781286（普通服务商）、https://pay.wei… |
| 4 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponNotifyConfigAsync` | GET | `/v3/marketing/partner/product-coupon/notify-configs` | 商户证书 / APIv3 签名 | 查询商品券通知回调。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781284（普通服务商）、https://pay.wei… |
| 5 | `IWechatPayMarketingPartnerProductCouponService` | `CreateProductCouponAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons` | 商户证书 / APIv3 签名 | 创建商品券（单品批次形态）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781289（普通服务商）、https://pay… |
| 6 | `IWechatPayMarketingPartnerProductCouponService` | `CreateProductCouponBundleAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons` | 商户证书 / APIv3 签名 | 创建商品券（批次组合形态）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781289（普通服务商）、https://pay… |
| 7 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponAsync` | GET | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}` | 商户证书 / APIv3 签名 | 查询商品券详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016434632（普通服务商）。 |
| 8 | `IWechatPayMarketingPartnerProductCouponService` | `UpdateProductCouponAsync` | PATCH | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}` | 商户证书 / APIv3 签名 | 修改商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781296（普通服务商）、https://pay.weixin.… |
| 9 | `IWechatPayMarketingPartnerProductCouponService` | `DeactivateProductCouponAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/deactivate` | 商户证书 / APIv3 签名 | 作废商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781290（普通服务商）、https://pay.weixin.… |
| 10 | `IWechatPayMarketingPartnerProductCouponService` | `CreateProductCouponStockBundleAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles` | 商户证书 / APIv3 签名 | 创建商品券批次组合。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280622（普通服务商）。 |
| 11 | `IWechatPayMarketingPartnerProductCouponService` | `UpdateProductCouponStockBundleAsync` | PATCH | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}` | 商户证书 / APIv3 签名 | 修改商品券批次组合。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280633（普通服务商）。 |
| 12 | `IWechatPayMarketingPartnerProductCouponService` | `AssociateProductCouponStockBundleStoresAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/associate-stores` | 商户证书 / APIv3 签名 | 批次组合关联门店。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280620（普通服务商）。 |
| 13 | `IWechatPayMarketingPartnerProductCouponService` | `DisassociateProductCouponStockBundleStoresAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/disassociate-stores` | 商户证书 / APIv3 签名 | 批次组合解除门店关联。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280629（普通服务商）。 |
| 14 | `IWechatPayMarketingPartnerProductCouponService` | `UpdateProductCouponStockBundleBudgetAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/update-budget` | 商户证书 / APIv3 签名 | 修改批次组合预算。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280642（普通服务商）。 |
| 15 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponStocksAsync` | GET | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks` | 商户证书 / APIv3 签名 | 查询商品券批次列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781553（普通服务商）、https://pay.wei… |
| 16 | `IWechatPayMarketingPartnerProductCouponService` | `CreateProductCouponStockAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks` | 商户证书 / APIv3 签名 | 创建商品券批次。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781304（普通服务商）。 |
| 17 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponStockAsync` | GET | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}` | 商户证书 / APIv3 签名 | 查询商品券批次详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781542（普通服务商）、https://pay.wei… |
| 18 | `IWechatPayMarketingPartnerProductCouponService` | `UpdateProductCouponStockAsync` | PATCH | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}` | 商户证书 / APIv3 签名 | 修改商品券批次。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781556（普通服务商）。 |
| 19 | `IWechatPayMarketingPartnerProductCouponService` | `DeactivateProductCouponStockAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/deactivate` | 商户证书 / APIv3 签名 | 作废商品券批次。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781532（普通服务商）。 |
| 20 | `IWechatPayMarketingPartnerProductCouponService` | `UpdateProductCouponStockBudgetAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/update-budget` | 商户证书 / APIv3 签名 | 修改商品券批次预算。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781561（普通服务商）。 |
| 21 | `IWechatPayMarketingPartnerProductCouponService` | `AssociateProductCouponStockStoresAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associate-stores` | 商户证书 / APIv3 签名 | 批次关联门店。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781302（普通服务商）。 |
| 22 | `IWechatPayMarketingPartnerProductCouponService` | `DisassociateProductCouponStockStoresAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/disassociate-stores` | 商户证书 / APIv3 签名 | 批次解除门店关联。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781537（普通服务商）。 |
| 23 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponAssociatedStoresAsync` | GET | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associated-stores` | 商户证书 / APIv3 签名 | 查询批次已关联门店。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781546（普通服务商）、https://pay.wei… |
| 24 | `IWechatPayMarketingPartnerProductCouponService` | `UploadProductCouponStockCodesAsync` | POST | `/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/upload-coupon-codes` | 商户证书 / APIv3 签名 | 预上传商品券券码。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781572（普通服务商）、https://pay.weix… |
| 25 | `IWechatPayMarketingPartnerProductCouponService` | `SendProductCouponUserBundleAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles` | 商户证书 / APIv3 签名 | 向用户发放券包。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280664（普通服务商）。 |
| 26 | `IWechatPayMarketingPartnerProductCouponService` | `DeactivateProductCouponUserBundleAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles/{userCouponBundleId}/deactivate` | 商户证书 / APIv3 签名 | 作废用户券包。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280658（普通服务商）。 |
| 27 | `IWechatPayMarketingPartnerProductCouponService` | `SendProductCouponUserCouponAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupons` | 商户证书 / APIv3 签名 | 向用户发放商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781605（普通服务商）。 |
| 28 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponUserCouponsAsync` | GET | `/v3/marketing/partner/product-coupon/users/{openid}/coupons` | 商户证书 / APIv3 签名 | 查询用户商品券列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781590（普通服务商）、https://pay.wei… |
| 29 | `IWechatPayMarketingPartnerProductCouponService` | `QueryProductCouponUserCouponAsync` | GET | `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}` | 商户证书 / APIv3 签名 | 查询用户商品券详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781582（普通服务商）、https://pay.wei… |
| 30 | `IWechatPayMarketingPartnerProductCouponService` | `ConfirmProductCouponUserCouponAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/confirm` | 商户证书 / APIv3 签名 | 确认用户商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781575（普通服务商）、https://pay.weixi… |
| 31 | `IWechatPayMarketingPartnerProductCouponService` | `ReturnProductCouponUserCouponAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/return` | 商户证书 / APIv3 签名 | 退用户商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781599（普通服务商）、https://pay.weixin… |
| 32 | `IWechatPayMarketingPartnerProductCouponService` | `UseProductCouponUserCouponAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/use` | 商户证书 / APIv3 签名 | 核销用户商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781608（普通服务商）、https://pay.weixi… |
| 33 | `IWechatPayMarketingPartnerProductCouponService` | `DeactivateProductCouponUserCouponAsync` | POST | `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/deactivate` | 商户证书 / APIv3 签名 | 作废用户商品券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781578（普通服务商）。 |

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

- （出现 33 次）**支持商户**【普通服务商】。
- （出现 33 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**`brand_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 1. `CreateProductCouponImageTaskAsync` — 创建商品券图片生成任务。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4017327735（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4017327752（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/image-generation-tasks`

- **请求体**：`ProductCouponImageTaskCreateRequest` — 创建商品券图片生成任务请求体，字段见 `ProductCouponImageTaskCreateRequest`。
- **返回**：`ProductCouponImageTaskCreateResponse` — 创建商品券图片生成任务，见 `ProductCouponImageTaskCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/image-generation-tasks`；无 path / query 参数。
- **应答**见 `ProductCouponImageTaskCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 2. `QueryProductCouponImageTaskAsync` — 查询商品券图片生成任务。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4017327739（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4017327753（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/image-generation-tasks/{taskId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `taskId` | Path | `string` | 【图片生成的任务ID】 服务商创建图片生成任务的请求流水号，服务商侧需保持唯一性，可使用 数字、大小写字母、下划线 _、短横线 - 组成，长度在6-40个字符之间（官方 path `task_id`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
| `imageGenerationType` | Query `image_generation_type` | `string` | 【图片生成类型】 与提交图片生成任务时所传的类型一致。可选取值：`COMBINE_IMAGE` 适合全场类优惠头图，将品牌元素与商品券优惠等信息拼接生成头图 / `CUT_OUT` 适合单品类优惠头图，用于去除图片背景，保留商品主体（官方 query `image_generation_type`，**必填**，类型 `string`）。 |
- **返回**：`ProductCouponImageTaskQueryResponse` — 查询商品券图片生成任务，见 `ProductCouponImageTaskQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/image-generation-tasks/{task_id}`；path `task_id`，query `brand_id` / `image_generation_type`。
- **`brand_id` 与 `image_generation_type` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 3. `SetProductCouponNotifyConfigAsync` — 设置商品券通知回调。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781286（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435719（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/notify-configs`

- **请求体**：`ProductCouponNotifyConfigSetRequest` — 设置商品券通知回调请求体，字段见 `ProductCouponNotifyConfigSetRequest`。
- **返回**：`ProductCouponNotifyConfigSetResponse` — 设置商品券通知回调，见 `ProductCouponNotifyConfigSetResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/notify-configs`；无 path / query 参数。
- **应答**见 `ProductCouponNotifyConfigSetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 4. `QueryProductCouponNotifyConfigAsync` — 查询商品券通知回调。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781284（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435718（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/notify-configs`
- **返回**：`ProductCouponNotifyConfigQueryResponse` — 查询商品券通知回调，见 `ProductCouponNotifyConfigQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/notify-configs`；无 path / query 参数。

### 5. `CreateProductCouponAsync` — 创建商品券（单品批次形态）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781289（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434630（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons`

- **请求体**：`ProductCouponCouponCreateRequest` — 创建商品券（单品批次形态）请求体，字段见 `ProductCouponCouponCreateRequest`。
- **返回**：`ProductCouponCouponCreateResponse` — 创建商品券（单品批次形态），见 `ProductCouponCouponCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons`；无 path / query 参数。
- **应答**见 `ProductCouponCouponCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 6. `CreateProductCouponBundleAsync` — 创建商品券（批次组合形态）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781289（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434630（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons`

- **请求体**：`ProductCouponBundleCouponCreateRequest` — 创建商品券（批次组合形态）请求体，字段见 `ProductCouponBundleCouponCreateRequest`。
- **返回**：`ProductCouponBundleCouponCreateResponse` — 创建商品券（批次组合形态），见 `ProductCouponBundleCouponCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons`；无 path / query 参数。
- **应答**见 `ProductCouponBundleCouponCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 7. `QueryProductCouponAsync` — 查询商品券详情。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016434632（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
- **返回**：`ProductCouponCouponQueryResponse` — 查询商品券详情，见 `ProductCouponCouponQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}`；path `product_coupon_id`，query `brand_id`。

### 8. `UpdateProductCouponAsync` — 修改商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781296（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434633（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`PATCH /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponCouponUpdateRequest` — 修改商品券请求体，字段见 `ProductCouponCouponUpdateRequest`。
- **返回**：`ProductCouponCouponUpdateResponse` — 修改商品券，见 `ProductCouponCouponUpdateResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}`；path `product_coupon_id`。
- **应答**见 `ProductCouponCouponUpdateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 9. `DeactivateProductCouponAsync` — 作废商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781290（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434631（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/deactivate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponCouponDeactivateRequest` — 作废商品券请求体，字段见 `ProductCouponCouponDeactivateRequest`。
- **返回**：`ProductCouponCouponDeactivateResponse` — 作废商品券，见 `ProductCouponCouponDeactivateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/deactivate`；path `product_coupon_id`。
- **应答**见 `ProductCouponCouponDeactivateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 10. `CreateProductCouponStockBundleAsync` — 创建商品券批次组合。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280622（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockBundleCreateRequest` — 创建商品券批次组合请求体，字段见 `ProductCouponStockBundleCreateRequest`。
- **返回**：`ProductCouponStockBundleCreateResponse` — 创建商品券批次组合，见 `ProductCouponStockBundleCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles`；path `product_coupon_id`。
- **应答**见 `ProductCouponStockBundleCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 11. `UpdateProductCouponStockBundleAsync` — 修改商品券批次组合。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280633（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`PATCH /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockBundleId` | Path | `string` | 【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成，本接口会同步修改批次组内所有批次的基本信息，请确保该批次组属于 product_coupon_id 对应的商品券（官方 path `stock_bundle_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockBundleUpdateRequest` — 修改商品券批次组合请求体，字段见 `ProductCouponStockBundleUpdateRequest`。
- **返回**：`ProductCouponStockBundleUpdateResponse` — 修改商品券批次组合，见 `ProductCouponStockBundleUpdateResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}`；path `product_coupon_id` / `stock_bundle_id`。
- **应答**见 `ProductCouponStockBundleUpdateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 12. `AssociateProductCouponStockBundleStoresAsync` — 批次组合关联门店。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280620（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/associate-stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockBundleId` | Path | `string` | 【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成，本接口会同步关联门店信息到批次组内所有批次。请确保该批次组属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法关联门店（官方 path `stock_bundle_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockBundleAssociateStoresRequest` — 批次组合关联门店请求体，字段见 `ProductCouponStockBundleAssociateStoresRequest`。
- **返回**：`ProductCouponStockBundleAssociateStoresResponse` — 批次组合关联门店，见 `ProductCouponStockBundleAssociateStoresResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}/associate-stores`；path `product_coupon_id` / `stock_bundle_id`。
- **应答**见 `ProductCouponStockBundleAssociateStoresResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 13. `DisassociateProductCouponStockBundleStoresAsync` — 批次组合解除门店关联。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280629（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/disassociate-stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockBundleId` | Path | `string` | 【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成，本接口会同步取消关联门店信息到批次组内所有批次。请确保该批次组属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法取消关联门店（官方 path `stock_bundle_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockBundleDisassociateStoresRequest` — 批次组合解除门店关联请求体，字段见 `ProductCouponStockBundleDisassociateStoresRequest`。
- **返回**：`ProductCouponStockBundleDisassociateStoresResponse` — 批次组合解除门店关联，见 `ProductCouponStockBundleDisassociateStoresResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}/disassociate-stores`；path `product_coupon_id` / `stock_bundle_id`。
- **应答**见 `ProductCouponStockBundleDisassociateStoresResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 14. `UpdateProductCouponStockBundleBudgetAsync` — 修改批次组合预算。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280642（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/update-budget`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockBundleId` | Path | `string` | 【批次组ID】 商品券批次组的唯一标识，由微信支付生成，本接口会同步修改批次组内所有批次的对应预算信息。请确保该批次组属于 product_coupon_id 对应的商品券（官方 path `stock_bundle_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockBundleBudgetRequest` — 修改批次组合预算请求体，字段见 `ProductCouponStockBundleBudgetRequest`。
- **返回**：`ProductCouponStockBundleBudgetResponse` — 修改批次组合预算，见 `ProductCouponStockBundleBudgetResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stock-bundles/{stock_bundle_id}/update-budget`；path `product_coupon_id` / `stock_bundle_id`。
- **应答**见 `ProductCouponStockBundleBudgetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 15. `QueryProductCouponStocksAsync` — 查询商品券批次列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781553（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434641（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
| `state` | Query `state` | `string?` | 【批次状态】 不填默认查询所有状态的批次。可选取值：`AUDITING` 审批中 / `SENDING` 发放中 / `PAUSED` 已暂停 / `STOPPED` 已停止，当前已到达结束时间 / `DEACTIVATED` 已失效，品牌方主动调用失效接口使批次失效（官方 query `state`，选填，类型 `string`）。 |
| `pageSize` | Query `page_size` | `int?` | 【分页大小】 单次拉取的数据条数上限，不填默认为20，最大值50（官方 query `page_size`，选填，类型 `integer`）。 |
| `pageToken` | Query `page_token` | `string?` | 【分页Token】 分页查询时，需要传入上一次调用返回的 next_page_token，首次调用不填（官方 query `page_token`，选填，类型 `string`）。 |
| `stockBundleId` | Query `stock_bundle_id` | `string?` | 【批次组ID】 商品券批次组的唯一标识，【 创建商品券(多次优惠)】或【 添加商品券批次组】时由微信支付生成。（官方 query `stock_bundle_id`，选填，类型 `string`）。 |
- **返回**：`ProductCouponStockListResponse` — 查询商品券批次列表，见 `ProductCouponStockListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks`；path `product_coupon_id`，query `brand_id` / `state` / `page_size` / `page_token` / `stock_bundle_id`。

### 16. `CreateProductCouponStockAsync` — 创建商品券批次。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781304（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockCreateRequest` — 创建商品券批次请求体，字段见 `ProductCouponStockCreateRequest`。
- **返回**：`ProductCouponStockCreateResponse` — 创建商品券批次，见 `ProductCouponStockCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks`；path `product_coupon_id`。
- **应答**见 `ProductCouponStockCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 17. `QueryProductCouponStockAsync` — 查询商品券批次详情。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781542（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434649（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path `stock_id`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
- **返回**：`ProductCouponStockQueryResponse` — 查询商品券批次详情，见 `ProductCouponStockQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}`；path `product_coupon_id` / `stock_id`，query `brand_id`。

### 18. `UpdateProductCouponStockAsync` — 修改商品券批次。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781556（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`PATCH /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockUpdateRequest` — 修改商品券批次请求体，字段见 `ProductCouponStockUpdateRequest`。
- **返回**：`ProductCouponStockUpdateResponse` — 修改商品券批次，见 `ProductCouponStockUpdateResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}`；path `product_coupon_id` / `stock_id`。
- **应答**见 `ProductCouponStockUpdateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 19. `DeactivateProductCouponStockAsync` — 作废商品券批次。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781532（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/deactivate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockDeactivateRequest` — 作废商品券批次请求体，字段见 `ProductCouponStockDeactivateRequest`。
- **返回**：`ProductCouponStockDeactivateResponse` — 作废商品券批次，见 `ProductCouponStockDeactivateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/deactivate`；path `product_coupon_id` / `stock_id`。
- **应答**见 `ProductCouponStockDeactivateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 20. `UpdateProductCouponStockBudgetAsync` — 修改商品券批次预算。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781561（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/update-budget`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockBudgetRequest` — 修改商品券批次预算请求体，字段见 `ProductCouponStockBudgetRequest`。
- **返回**：`ProductCouponStockBudgetResponse` — 修改商品券批次预算，见 `ProductCouponStockBudgetResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/update-budget`；path `product_coupon_id` / `stock_id`。
- **应答**见 `ProductCouponStockBudgetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 21. `AssociateProductCouponStockStoresAsync` — 批次关联门店。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781302（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associate-stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法关联门店（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockAssociateStoresRequest` — 批次关联门店请求体，字段见 `ProductCouponStockAssociateStoresRequest`。
- **返回**：`ProductCouponStockAssociateStoresResponse` — 批次关联门店，见 `ProductCouponStockAssociateStoresResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/associate-stores`；path `product_coupon_id` / `stock_id`。
- **应答**见 `ProductCouponStockAssociateStoresResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 22. `DisassociateProductCouponStockStoresAsync` — 批次解除门店关联。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781537（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/disassociate-stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券，且 store_scope 为 SPECIFIC，否则无法取消关联门店（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockDisassociateStoresRequest` — 批次解除门店关联请求体，字段见 `ProductCouponStockDisassociateStoresRequest`。
- **返回**：`ProductCouponStockDisassociateStoresResponse` — 批次解除门店关联，见 `ProductCouponStockDisassociateStoresResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/disassociate-stores`；path `product_coupon_id` / `stock_id`。
- **应答**见 `ProductCouponStockDisassociateStoresResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 23. `QueryProductCouponAssociatedStoresAsync` — 查询批次已关联门店。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781546（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434665（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associated-stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path `stock_id`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
| `pageSize` | Query `page_size` | `int?` | 【分页大小】 单次拉取的数据条数上限，不填默认为20，最大值200（官方 query `page_size`，选填，类型 `integer`）。 |
| `pageToken` | Query `page_token` | `string?` | 【分页Token】 分页查询时，需要传入上一次调用返回的 next_page_token，首次调用不填（官方 query `page_token`，选填，类型 `string`）。 |
- **返回**：`ProductCouponStockAssociatedStoresResponse` — 查询批次已关联门店，见 `ProductCouponStockAssociatedStoresResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/associated-stores`；path `product_coupon_id` / `stock_id`，query `brand_id` / `page_size` / `page_token`。

### 24. `UploadProductCouponStockCodesAsync` — 预上传商品券券码。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781572（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016434668（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/upload-coupon-codes`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `productCouponId` | Path | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 path `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Path | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 path `stock_id`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponStockUploadCodesRequest` — 预上传商品券券码请求体，字段见 `ProductCouponStockUploadCodesRequest`。
- **返回**：`ProductCouponStockUploadCodesResponse` — 预上传商品券券码，见 `ProductCouponStockUploadCodesResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/product-coupons/{product_coupon_id}/stocks/{stock_id}/upload-coupon-codes`；path `product_coupon_id` / `stock_id`。
- **应答**见 `ProductCouponStockUploadCodesResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 25. `SendProductCouponUserBundleAsync` — 向用户发放券包。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280664（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponUserBundleSendRequest` — 向用户发放券包请求体，字段见 `ProductCouponUserBundleSendRequest`。
- **返回**：`ProductCouponUserBundleSendResponse` — 向用户发放券包，见 `ProductCouponUserBundleSendResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles`；path `openid`。
- **应答**见 `ProductCouponUserBundleSendResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 26. `DeactivateProductCouponUserBundleAsync` — 作废用户券包。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016280658（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles/{userCouponBundleId}/deactivate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `userCouponBundleId` | Path | `string` | 【用户券组ID】 用户券组的唯一标识，【 向用户发放商品券批次组】时由微信支付生成，本接口会同步失效用户商品券组内所有用户商品券（官方 path `user_coupon_bundle_id`，**必填**，类型 `string(40)`）。 |

- **请求体**：`ProductCouponUserBundleDeactivateRequest` — 作废用户券包请求体，字段见 `ProductCouponUserBundleDeactivateRequest`。
- **返回**：`ProductCouponUserBundleDeactivateResponse` — 作废用户券包，见 `ProductCouponUserBundleDeactivateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles/{user_coupon_bundle_id}/deactivate`；path `openid` / `user_coupon_bundle_id`。
- **应答**见 `ProductCouponUserBundleDeactivateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 27. `SendProductCouponUserCouponAsync` — 向用户发放商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781605（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |

- **请求体**：`ProductCouponUserCouponSendRequest` — 向用户发放商品券请求体，字段见 `ProductCouponUserCouponSendRequest`。
- **返回**：`ProductCouponUserCouponSendResponse` — 向用户发放商品券，见 `ProductCouponUserCouponSendResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupons`；path `openid`。
- **应答**见 `ProductCouponUserCouponSendResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 28. `QueryProductCouponUserCouponsAsync` — 查询用户商品券列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781590（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435703（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/users/{openid}/coupons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `appid` | Query `appid` | `string` | 【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）。（官方 query `appid`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
| `productCouponId` | Query `product_coupon_id` | `string?` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成。查询指定商品券下的用户商品券列表，不传入时查询范围扩大至品牌下的所有商品券（官方 query `product_coupon_id`，选填，类型 `string`）。 |
| `stockId` | Query `stock_id` | `string?` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建）。查询指定商品券批次下的用户商品券列表。（官方 query `stock_id`，选填，类型 `string`）。 |
| `couponState` | Query `coupon_state` | `string?` | 【用户商品券状态】 查询指定状态的用户商品券。可选取值：`CONFIRMING` 待确认，用户商品券发放需要品牌方调用【 [确认发放用户商品券API(单券)](https://pay.weixin.qq.com/doc/v3/partner/4015781575.md)】或【 [确认发放用户商品券API(多次优惠)](https://pay.weixin.qq.com/doc/v3/partner/4016435562.md)】后才能生效 / `PENDING` 已发放待生效，用户商品券已发放成功但尚未到达可用开始时间 / `EFFECTIVE` 已生效，用户商品券已成功发放且到达可用开始时间…（官方 query `coupon_state`，选填，类型 `string`）。 |
| `userCouponBundleId` | Query `user_coupon_bundle_id` | `string?` | 【用户券组ID】 用户券组的唯一标识，【 向用户发放商品券批次组】时由微信支付生成，用于查询特定用户券组内的券（官方 query `user_coupon_bundle_id`，选填，类型 `string`）。 |
| `pageSize` | Query `page_size` | `int?` | 【分页大小】 单次拉取的数据条数上限，不填默认为20，最大值50（官方 query `page_size`，选填，类型 `integer`）。 |
| `pageToken` | Query `page_token` | `string?` | 【分页Token】 分页查询时，需要传入上一次调用返回的 next_page_token，首次调用不填（官方 query `page_token`，选填，类型 `string`）。 |
- **返回**：`ProductCouponUserCouponListResponse` — 查询用户商品券列表，见 `ProductCouponUserCouponListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/users/{openid}/coupons`；path `openid`，query `appid` / `brand_id` / `product_coupon_id` / `stock_id` / `coupon_state` / `user_coupon_bundle_id` / `page_size` / `page_token`。
- **`appid` 与 `brand_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 29. `QueryProductCouponUserCouponAsync` — 查询用户商品券详情。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781582（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435702（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`GET /v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `couponCode` | Path | `string` | 【用户商品券Code】 券的唯一标识（官方 path `coupon_code`，**必填**，类型 `string(40)`）。 |
| `productCouponId` | Query `product_coupon_id` | `string` | 【商品券ID】 商品券的唯一标识， 创建商品券 时由微信支付生成（官方 query `product_coupon_id`，**必填**，类型 `string`）。 |
| `stockId` | Query `stock_id` | `string` | 【批次ID】 商品券批次的唯一标识，商品券批次创建时由微信支付生成（可使用【 创建商品券API】或【 添加商品券批次API】创建），请确保该批次属于 product_coupon_id 对应的商品券（官方 query `stock_id`，**必填**，类型 `string`）。 |
| `appid` | Query `appid` | `string` | 【公众账号ID】 公众账号ID也称AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序(APP、小程序、公众号)。 开发者需要先在微信开放平台或微信公众平台中申请ID，然后与品牌方或服务商绑定（详见 服务商商户号与AppID账号关联管理）。（官方 query `appid`，**必填**，类型 `string`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 微信支付为品牌方分配的唯一标识，该品牌应与服务商存在授权关系（官方 query `brand_id`，**必填**，类型 `string`）。 |
- **返回**：`ProductCouponUserCouponQueryResponse` — 查询用户商品券详情，见 `ProductCouponUserCouponQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}`；path `openid` / `coupon_code`，query `product_coupon_id` / `stock_id` / `appid` / `brand_id`。
- **`product_coupon_id` 与 `stock_id` 与 `appid` 与 `brand_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 30. `ConfirmProductCouponUserCouponAsync` — 确认用户商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781575（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435562（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/confirm`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `couponCode` | Path | `string` | 【用户商品券Code】 用户商品券的唯一标识（官方 path `coupon_code`，**必填**，类型 `string(40)`）。 |

- **请求体**：`ProductCouponUserCouponConfirmRequest` — 确认用户商品券请求体，字段见 `ProductCouponUserCouponConfirmRequest`。
- **返回**：`ProductCouponUserCouponConfirmResponse` — 确认用户商品券，见 `ProductCouponUserCouponConfirmResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/confirm`；path `openid` / `coupon_code`。
- **应答**见 `ProductCouponUserCouponConfirmResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 31. `ReturnProductCouponUserCouponAsync` — 退用户商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781599（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435674（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/return`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `couponCode` | Path | `string` | 【用户商品券Code】 用户商品券的唯一标识（官方 path `coupon_code`，**必填**，类型 `string(40)`）。 |

- **请求体**：`ProductCouponUserCouponReturnRequest` — 退用户商品券请求体，字段见 `ProductCouponUserCouponReturnRequest`。
- **返回**：`ProductCouponUserCouponReturnResponse` — 退用户商品券，见 `ProductCouponUserCouponReturnResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/return`；path `openid` / `coupon_code`。
- **应答**见 `ProductCouponUserCouponReturnResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 32. `UseProductCouponUserCouponAsync` — 核销用户商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781608（普通服务商）、https://pay.weixin.qq.com/doc/v3/partner/4016435636（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/use`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `couponCode` | Path | `string` | 【用户商品券Code】 用户商品券的唯一标识（官方 path `coupon_code`，**必填**，类型 `string(40)`）。 |

- **请求体**：`ProductCouponUserCouponUseRequest` — 核销用户商品券请求体，字段见 `ProductCouponUserCouponUseRequest`。
- **返回**：`ProductCouponUserCouponUseResponse` — 核销用户商品券，见 `ProductCouponUserCouponUseResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/use`；path `openid` / `coupon_code`。
- **应答**见 `ProductCouponUserCouponUseResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 33. `DeactivateProductCouponUserCouponAsync` — 作废用户商品券。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015781578（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs)
- **官方契约**：`POST /v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/deactivate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识，获取方式参考 OpenID（官方 path `openid`，**必填**，类型 `string`）。 |
| `couponCode` | Path | `string` | 【用户商品券Code】 用户商品券的唯一标识（官方 path `coupon_code`，**必填**，类型 `string(40)`）。 |

- **请求体**：`ProductCouponUserCouponDeactivateRequest` — 作废用户商品券请求体，字段见 `ProductCouponUserCouponDeactivateRequest`。
- **返回**：`ProductCouponUserCouponDeactivateResponse` — 作废用户商品券，见 `ProductCouponUserCouponDeactivateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/product-coupon/users/{openid}/coupons/{coupon_code}/deactivate`；path `openid` / `coupon_code`。
- **应答**见 `ProductCouponUserCouponDeactivateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MarketingPartnerProductCoupon`](../../DataModels/MarketingPartnerProductCoupon) 内共 **774** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **774** 个，`SerializerClassName` = `MarketingPartnerProductCoupon`。

AOT 源生成上下文：[`MarketingPartnerProductCouponJsonContext.g.cs`](./../../DataModels/Generated/MarketingPartnerProductCouponJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ProductCouponImageTaskCreateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponImageTaskDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponImageTaskDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `exchange_price`, `scope`, `type`, `usage_mode`, `normal_coupon`, `discount_coupon`, `exchange_coupon`, `background_color`, `image_url` … |
| `ProductCouponImageTaskCreateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponImageTaskDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponImageTaskDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `exchange_price`, `scope`, `type`, `usage_mode`, `normal_coupon`, `discount_coupon`, `exchange_coupon`, `background_color`, `image_url` … |
| `ProductCouponImageTaskQueryResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponImageTaskDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponImageTaskDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `exchange_price`, `scope`, `type`, `usage_mode`, `normal_coupon`, `discount_coupon`, `exchange_coupon`, `background_color`, `image_url` … |
| `ProductCouponNotifyConfigSetResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponConfigDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponConfigDtos.cs) | `notify_url`, `update_time` |
| `ProductCouponNotifyConfigSetRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponConfigDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponConfigDtos.cs) | `notify_url`, `update_time` |
| `ProductCouponNotifyConfigQueryResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponConfigDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponConfigDtos.cs) | `notify_url`, `update_time` |
| `ProductCouponCouponCreateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponCouponCreateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponBundleCouponCreateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponBundleCouponCreateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponCouponQueryResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponCouponUpdateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponCouponUpdateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponCouponDeactivateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponCouponDeactivateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponMainDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponMainDtos.cs) | `threshold`, `discount_amount`, `percent_off`, `normal_coupon`, `discount_coupon`, `name`, `price`, `count`, `image_url`, `mini_program_appid`, `mini_program_path`, `pick_count` … |
| `ProductCouponStockBundleCreateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleCreateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleUpdateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleUpdateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleAssociateStoresResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleAssociateStoresRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleDisassociateStoresResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleDisassociateStoresRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleBudgetResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockBundleBudgetRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockBundleDtos.cs) | `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days`, `wait_days_after_receive`, `weekly_available_period` … |
| `ProductCouponStockListResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockCreateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockCreateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockQueryResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockUpdateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockUpdateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockDeactivateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockDeactivateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockBudgetResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockBudgetRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockAssociateStoresResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockAssociateStoresRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockDisassociateStoresResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockDisassociateStoresRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockAssociatedStoresResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockUploadCodesResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponStockUploadCodesRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponStockDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponStockDtos.cs) | `total_count`, `available_count`, `max_count`, `max_count_per_day`, `max_count_per_user`, `begin_time`, `end_time`, `day_list`, `day_period_list`, `available_begin_time`, `available_end_time`, `available_days` … |
| `ProductCouponUserBundleSendResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_bundle_id`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id`, `out_trade_no` … |
| `ProductCouponUserBundleSendRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_bundle_id`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id`, `out_trade_no` … |
| `ProductCouponUserBundleDeactivateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_bundle_id`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id`, `out_trade_no` … |
| `ProductCouponUserBundleDeactivateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserBundleDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_bundle_id`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id`, `out_trade_no` … |
| `ProductCouponUserCouponSendResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponSendRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponListResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponQueryResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponConfirmResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponConfirmRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponReturnResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponReturnRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponUseResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponUseRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponDeactivateResponse` | 应答 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |
| `ProductCouponUserCouponDeactivateRequest` | 请求体 | [MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs](../../DataModels/MarketingPartnerProductCoupon/ProductCouponUserCouponDtos.cs) | `member_card_id`, `coupon_tag_list`, `member_tag_info`, `product_coupon_id`, `stock_id`, `coupon_code`, `appid`, `send_request_no`, `attach`, `brand_id`, `coupon_tag_info`, `transaction_id` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingPartnerProductCoupon));

var service = app.Services.GetRequiredService<IWechatPayMarketingPartnerProductCouponService>();
var ct = CancellationToken.None;
var response = await service.QueryProductCouponNotifyConfigAsync(ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/partner/product-coupon/notify-configs");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingPartnerProductCouponService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryProductCouponNotifyConfigAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IWechatPayMarketingPartnerProductCouponService`：注释 34 / 实计 33 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingPartnerProductCouponService](./IWechatPayMarketingPartnerProductCouponService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingPartnerProductCouponContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPartnerProductCouponContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `ProductCouponTopology_ShouldBeOneInterfaceOnOneGroup`、`ProductCouponEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`ProductCouponDtos_ShouldExposeOfficialFieldRosters`、`ProductCouponDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MarketingPartnerProductCoupon`](../../DataModels/MarketingPartnerProductCoupon) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingPartnerProductCouponContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
