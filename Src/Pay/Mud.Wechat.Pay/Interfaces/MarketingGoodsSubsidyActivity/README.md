# MarketingGoodsSubsidyActivity 域 · 商品补贴活动

微信支付「商品补贴活动」域 SDK（8 端点）。 接口：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)；注册组 `MarketingGoodsSubsidyActivity`；模块枚举 `PayModule.MarketingGoodsSubsidyActivity`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingGoodsSubsidyActivityContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingGoodsSubsidyActivityContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity`](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingGoodsSubsidyActivityService = 8；按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingGoodsSubsidyActivityService` | `SetRetailStoreRepresentativeAsync` | PUT | `/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative` | 商户证书 / APIv3 签名 | 设置活动门店 representative。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384386（普通服务商）。 |
| 2 | `IWechatPayMarketingGoodsSubsidyActivityService` | `DeleteRetailStoreRepresentativeAsync` | DELETE | `/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative` | 商户证书 / APIv3 签名 | 删除活动门店 representative。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384399（普通服务商）。 |
| 3 | `IWechatPayMarketingGoodsSubsidyActivityService` | `QueryRetailStoreRepresentativesAsync` | GET | `/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representatives` | 商户证书 / APIv3 签名 | 查询活动门店 representative 列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384405（普通服务商）。 |
| 4 | `IWechatPayMarketingGoodsSubsidyActivityService` | `UploadRetailStoreMaterialsAsync` | POST | `/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/materials` | 商户证书 / APIv3 签名 | 上传小店活动素材。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384395（普通服务商）。 |
| 5 | `IWechatPayMarketingGoodsSubsidyActivityService` | `AddRetailStoreStoresAsync` | POST | `/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores` | 商户证书 / APIv3 签名 | 添加小店活动门店。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384421（普通服务商）。 |
| 6 | `IWechatPayMarketingGoodsSubsidyActivityService` | `DeleteRetailStoreStoresAsync` | DELETE | `/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores` | 商户证书 / APIv3 签名 | 删除小店活动门店。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384431（普通服务商）。 |
| 7 | `IWechatPayMarketingGoodsSubsidyActivityService` | `QueryRetailStoreStoresAsync` | GET | `/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores` | 商户证书 / APIv3 签名 | 查询小店活动门店列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384414（普通服务商）。 |
| 8 | `IWechatPayMarketingGoodsSubsidyActivityService` | `QueryRetailStoreStoreAsync` | GET | `/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores/{storeCode}` | 商户证书 / APIv3 签名 | 查询小店活动门店详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384437（普通服务商）。 |

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

- （出现 8 次）**支持商户**【普通服务商】。
- （出现 8 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 7 次）**业务错误码**：400 INVALID_REQUEST 无效请求 / 400 INVALID_REQUEST 参数错误 / 500 SYSTEM_ERROR 系统错误。

### 1. `SetRetailStoreRepresentativeAsync` — 设置活动门店 representative。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384386（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`PUT /v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【零售小店活动ID】 零售小店活动ID（官方 path `activity_id`，**必填**，类型 `string(128)`）。 |

- **请求体**：`RetailStoreActRepresentativeSetRequest` — 设置活动门店 representative请求体，字段见 `RetailStoreActRepresentativeSetRequest`。
- **返回**：`RetailStoreActRepresentativeSetResponse` — 设置活动门店 representative，见 `RetailStoreActRepresentativeSetResponse`。

**备注（原文）**

- **官方契约**：**PUT** `/v3/marketing/goods-subsidy-activity/retail-store-act/{activity_id}/representative`；path `activity_id`。
- **应答**见 `RetailStoreActRepresentativeSetResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 INVALID_REQUEST 无效请求 / 500 SYSTEM_ERROR 系统错误。

### 2. `DeleteRetailStoreRepresentativeAsync` — 删除活动门店 representative。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384399（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`DELETE /v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【零售小店活动ID】 零售小店活动ID（官方 path `activity_id`，**必填**，类型 `string(128)`）。 |

- **请求体**：`RetailStoreActRepresentativeDeleteRequest` — 删除活动门店 representative请求体，字段见 `RetailStoreActRepresentativeDeleteRequest`。
- **返回**：`RetailStoreActRepresentativeDeleteResponse` — 删除活动门店 representative，见 `RetailStoreActRepresentativeDeleteResponse`。

**备注（原文）**

- **官方契约**：**DELETE** `/v3/marketing/goods-subsidy-activity/retail-store-act/{activity_id}/representative`；path `activity_id`。
- **应答**见 `RetailStoreActRepresentativeDeleteResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 3. `QueryRetailStoreRepresentativesAsync` — 查询活动门店 representative 列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384405（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`GET /v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representatives`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【零售小店活动ID】 零售小店活动ID（官方 path `activity_id`，**必填**，类型 `string(128)`）。 |
| `offset` | Query `offset` | `int` | 【分页页码】 页码从0开始，默认第0页（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小，默认查询10个业代（官方 query `limit`，**必填**，类型 `integer`）。 |
- **返回**：`RetailStoreActRepresentativeListResponse` — 查询活动门店 representative 列表，见 `RetailStoreActRepresentativeListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/goods-subsidy-activity/retail-store-act/{activity_id}/representatives`；path `activity_id`，query `offset` / `limit`。
- **`offset` 与 `limit` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 4. `UploadRetailStoreMaterialsAsync` — 上传小店活动素材。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384395（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`POST /v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/materials`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 品牌ID（官方 path `brand_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`RetailStoreActMaterialUploadRequest` — 上传小店活动素材请求体，字段见 `RetailStoreActMaterialUploadRequest`。
- **返回**：`RetailStoreActMaterialUploadResponse` — 上传小店活动素材，见 `RetailStoreActMaterialUploadResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/materials`；path `brand_id`。
- **应答**见 `RetailStoreActMaterialUploadResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 5. `AddRetailStoreStoresAsync` — 添加小店活动门店。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384421（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`POST /v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 品牌ID（官方 path `brand_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`RetailStoreActStoreAddRequest` — 添加小店活动门店请求体，字段见 `RetailStoreActStoreAddRequest`。
- **返回**：`RetailStoreActStoreAddResponse` — 添加小店活动门店，见 `RetailStoreActStoreAddResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores`；path `brand_id`。
- **应答**见 `RetailStoreActStoreAddResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 6. `DeleteRetailStoreStoresAsync` — 删除小店活动门店。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384431（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`DELETE /v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 品牌ID（官方 path `brand_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`RetailStoreActStoreDeleteRequest` — 删除小店活动门店请求体，字段见 `RetailStoreActStoreDeleteRequest`。
- **返回**：`RetailStoreActStoreDeleteResponse` — 删除小店活动门店，见 `RetailStoreActStoreDeleteResponse`。

**备注（原文）**

- **官方契约**：**DELETE** `/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores`；path `brand_id`。
- **应答**见 `RetailStoreActStoreDeleteResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 7. `QueryRetailStoreStoresAsync` — 查询小店活动门店列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384414（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`GET /v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 品牌ID（官方 path `brand_id`，**必填**，类型 `string(32)`）。 |
| `offset` | Query `offset` | `int` | 【分页页码】 页码从0开始，默认第0页（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小，默认查询10家门店（官方 query `limit`，**必填**，类型 `integer`）。 |
- **返回**：`RetailStoreActStoreListResponse` — 查询小店活动门店列表，见 `RetailStoreActStoreListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores`；path `brand_id`，query `offset` / `limit`。
- **`offset` 与 `limit` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 8. `QueryRetailStoreStoreAsync` — 查询小店活动门店详情。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012384437（普通服务商）。

- **声明接口**：[IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs)
- **官方契约**：`GET /v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores/{storeCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 品牌ID（官方 path `brand_id`，**必填**，类型 `string(32)`）。 |
| `storeCode` | Path | `string` | 【门店编码】 门店编码，可包含英文字母，数字，\|，_， _，-等内容，不允许出现其他不合法符号。_（官方 path `store_code`，**必填**，类型 `string(128)`）。 |
- **返回**：`RetailStoreActStoreQueryResponse` — 查询小店活动门店详情，见 `RetailStoreActStoreQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/goods-subsidy-activity/retail-store-act/{brand_id}/stores/{store_code}`；path `brand_id` / `store_code`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity`](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/) 内共 **24** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **24** 个，`SerializerClassName` = `MarketingGoodsSubsidyActivity`。

AOT 源生成上下文：[`MarketingGoodsSubsidyActivityJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MarketingGoodsSubsidyActivityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `RetailStoreActRepresentativeSetResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs) | `openid`, `representative_info_list`, `out_request_no`, `add_time`, `activity_id`, `failed_representative_info_list`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActRepresentativeSetRequest` | 请求体 | [MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs) | `openid`, `representative_info_list`, `out_request_no`, `add_time`, `activity_id`, `failed_representative_info_list`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActRepresentativeDeleteResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs) | `openid`, `representative_info_list`, `out_request_no`, `add_time`, `activity_id`, `failed_representative_info_list`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActRepresentativeDeleteRequest` | 请求体 | [MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs) | `openid`, `representative_info_list`, `out_request_no`, `add_time`, `activity_id`, `failed_representative_info_list`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActRepresentativeListResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActRepresentativeDtos.cs) | `openid`, `representative_info_list`, `out_request_no`, `add_time`, `activity_id`, `failed_representative_info_list`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActMaterialUploadResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActMaterialDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActMaterialDtos.cs) | `out_request_no`, `material_num`, `material_id`, `material_url`, `material_list` |
| `RetailStoreActMaterialUploadRequest` | 请求体 | [MarketingGoodsSubsidyActivity/RetailStoreActMaterialDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActMaterialDtos.cs) | `out_request_no`, `material_num`, `material_id`, `material_url`, `material_list` |
| `RetailStoreActStoreAddResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs) | `store_code`, `store_name`, `out_request_no`, `add_time`, `stores`, `failed_stores`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActStoreAddRequest` | 请求体 | [MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs) | `store_code`, `store_name`, `out_request_no`, `add_time`, `stores`, `failed_stores`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActStoreDeleteResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs) | `store_code`, `store_name`, `out_request_no`, `add_time`, `stores`, `failed_stores`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActStoreDeleteRequest` | 请求体 | [MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs) | `store_code`, `store_name`, `out_request_no`, `add_time`, `stores`, `failed_stores`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActStoreListResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs) | `store_code`, `store_name`, `out_request_no`, `add_time`, `stores`, `failed_stores`, `delete_time`, `total_count`, `data`, `offset`, `limit` |
| `RetailStoreActStoreQueryResponse` | 应答 | [MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/RetailStoreActStoreDtos.cs) | `store_code`, `store_name`, `out_request_no`, `add_time`, `stores`, `failed_stores`, `delete_time`, `total_count`, `data`, `offset`, `limit` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingGoodsSubsidyActivity;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingGoodsSubsidyActivity));

var service = app.Services.GetRequiredService<IWechatPayMarketingGoodsSubsidyActivityService>();
var ct = CancellationToken.None;
var response = await service.AddRetailStoreStoresAsync("", new RetailStoreActStoreAddRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingGoodsSubsidyActivityService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddRetailStoreStoresAsync`）；请求体 `RetailStoreActStoreAddRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingGoodsSubsidyActivityService](./IWechatPayMarketingGoodsSubsidyActivityService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingGoodsSubsidyActivityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingGoodsSubsidyActivityContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `GoodsSubsidyTopology_ShouldBeOneInterfaceOnOneGroup`、`GoodsSubsidyEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`GoodsSubsidyDtos_ShouldExposeOfficialFieldRosters`、`GoodsSubsidyDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity`](../../../Mud.Wechat.Pay.DataModels/MarketingGoodsSubsidyActivity/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingGoodsSubsidyActivityContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
