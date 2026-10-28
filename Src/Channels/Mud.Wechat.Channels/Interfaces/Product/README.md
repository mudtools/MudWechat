# Product 域 · 商品管理

微信小店 / 视频号（channels 生态）「商品管理」域 SDK（43 端点：商品增改查 / 上下架 / 审核 /
库存 / 赠品 / 买赠活动 / 限时抢购 / 类目辅助 / 第三方货源）。 接口：[IChannelsProductService](./IChannelsProductService.cs)；注册组 `Product`；模块枚举 `ChannelsModule.Product`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsProductContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsProductContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/Product`](../../../Mud.Wechat.Channels/Product/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsProductService = 43；按 HTTP 特性实计 **43** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsProductService` | `AddProductAsync` | POST | `/channels/ec/product/add` | `AccessToken`（Query），Query `access_token` | 添加商品（`product/add`）。 |
| 2 | `IChannelsProductService` | `UpdateProductAsync` | POST | `/channels/ec/product/update` | `AccessToken`（Query），Query `access_token` | 更新商品（`product/update`）。 |
| 3 | `IChannelsProductService` | `GetProductAsync` | POST | `/channels/ec/product/get` | `AccessToken`（Query），Query `access_token` | 获取商品（`product/get`）。 |
| 4 | `IChannelsProductService` | `GetProductListAsync` | POST | `/channels/ec/product/list/get` | `AccessToken`（Query），Query `access_token` | 获取商品列表（`product/list/get`）。 |
| 5 | `IChannelsProductService` | `ListingProductAsync` | POST | `/channels/ec/product/listing` | `AccessToken`（Query），Query `access_token` | 商品上架（`product/listing`）。 |
| 6 | `IChannelsProductService` | `DelistingProductAsync` | POST | `/channels/ec/product/delisting` | `AccessToken`（Query），Query `access_token` | 商品下架（`product/delisting`）。 |
| 7 | `IChannelsProductService` | `DeleteProductAsync` | POST | `/channels/ec/product/delete` | `AccessToken`（Query），Query `access_token` | 删除商品（`product/delete`）。 |
| 8 | `IChannelsProductService` | `CancelProductAuditAsync` | POST | `/channels/ec/product/audit/cancel` | `AccessToken`（Query），Query `access_token` | 取消商品提审（`product/audit/cancel`）。 |
| 9 | `IChannelsProductService` | `UpdateProductAuditFreeAsync` | POST | `/channels/ec/product/auditfree` | `AccessToken`（Query），Query `access_token` | 更新商品免审（`product/auditfree`）—— 部分字段更新（sku / 库存 / 运费 / 限购等）免审。 |
| 10 | `IChannelsProductService` | `GetProductAuditStrategyAsync` | POST | `/channels/ec/product/auditstrategy/get` | `AccessToken`（Query），Query `access_token` | 获取上架策略（`product/auditstrategy/get`）。 |
| 11 | `IChannelsProductService` | `SetProductAuditStrategyAsync` | POST | `/channels/ec/product/auditstrategy/set` | `AccessToken`（Query），Query `access_token` | 设置上架策略（`product/auditstrategy/set`）。 |
| 12 | `IChannelsProductService` | `GetProductAuditQuotaAsync` | POST | `/channels/ec/product/getauditquota` | `AccessToken`（Query），Query `access_token` | 获取提审配额（`product/getauditquota`）。 |
| 13 | `IChannelsProductService` | `GetProductRestrictedInfoAsync` | POST | `/channels/ec/product/getproductrestrictedinfo` | `AccessToken`（Query），Query `access_token` | 获取商品受限信息（`product/getproductrestrictedinfo`，上架 / 展示受限策略查询）。 |
| 14 | `IChannelsProductService` | `ClassifyProductCategoryAsync` | POST | `/channels/ec/product/category/classify` | `AccessToken`（Query），Query `access_token` | 类目「一物一码」鉴定 / 图片分类（`product/category/classify`）。 |
| 15 | `IChannelsProductService` | `CategoryPrecheckAsync` | POST | `/channels/ec/product/categoryprecheck` | `AccessToken`（Query），Query `access_token` | 类目预检（`product/categoryprecheck`，判断商品信息是否满足类目要求）。 |
| 16 | `IChannelsProductService` | `AddProductThirdPartySourceAsync` | POST | `/channels/ec/product/addproductthirdpartysource` | `AccessToken`（Query），Query `access_token` | 新增第三方货源商品（`product/addproductthirdpartysource`，分销铺货 / 商品搬家 / 其他场景）。 |
| 17 | `IChannelsProductService` | `RecommendProductBrandAsync` | POST | `/channels/ec/product/productbrandrecommend` | `AccessToken`（Query），Query `access_token` | 品牌推荐（`product/productbrandrecommend`，根据类目与商品信息推荐品牌）。 |
| 18 | `IChannelsProductService` | `MapExternalProductAttributeAsync` | POST | `/channels/ec/product/externalproductmapping` | `AccessToken`（Query），Query `access_token` | 外部商品属性映射（`product/externalproductmapping`）。 |
| 19 | `IChannelsProductService` | `MapExternalProductAttributeNewAsync` | POST | `/channels/ec/product/externalproductmappingnew` | `AccessToken`（Query），Query `access_token` | 外部商品属性映射（新版，`product/externalproductmappingnew`）。 |
| 20 | `IChannelsProductService` | `BeginTimingSaleAsync` | POST | `/channels/ec/product/begintimingsale` | `AccessToken`（Query），Query `access_token` | 开始定时开售（`product/begintimingsale`）。 |
| 21 | `IChannelsProductService` | `CancelTimingSaleAsync` | POST | `/channels/ec/product/canceltimingsale` | `AccessToken`（Query），Query `access_token` | 取消定时开售（`product/canceltimingsale`）。 |
| 22 | `IChannelsProductService` | `GetProductTagLinkAsync` | POST | `/channels/ec/product/taglink/get` | `AccessToken`（Query），Query `access_token` | 获取商品微信口令（`product/taglink/get`，只支持微信内打开）。 |
| 23 | `IChannelsProductService` | `GetProductQrcodeAsync` | POST | `/channels/ec/product/qrcode/get` | `AccessToken`（Query），Query `access_token` | 获取商品二维码 / 物料（`product/qrcode/get`）。 |
| 24 | `IChannelsProductService` | `GetProductH5UrlAsync` | POST | `/channels/ec/product/h5url/get` | `AccessToken`（Query），Query `access_token` | 获取商品 H5 短链（`product/h5url/get`）。 |
| 25 | `IChannelsProductService` | `GetProductSchemeAsync` | POST | `/channels/ec/product/scheme/get` | `AccessToken`（Query），Query `access_token` | 获取商品跳转 scheme（`product/scheme/get`）。 |
| 26 | `IChannelsProductService` | `GetStockAsync` | POST | `/channels/ec/product/stock/get` | `AccessToken`（Query），Query `access_token` | 获取库存（`product/stock/get`）。 |
| 27 | `IChannelsProductService` | `UpdateStockAsync` | POST | `/channels/ec/product/stock/update` | `AccessToken`（Query），Query `access_token` | 更新库存（`product/stock/update`）。 |
| 28 | `IChannelsProductService` | `BatchGetStockAsync` | POST | `/channels/ec/product/stock/batchget` | `AccessToken`（Query），Query `access_token` | 批量获取库存（`product/stock/batchget`）。 |
| 29 | `IChannelsProductService` | `GetStockFlowAsync` | POST | `/channels/ec/product/stock/getflow` | `AccessToken`（Query），Query `access_token` | 获取库存流水（`product/stock/getflow`，库存变动对账）。 |
| 30 | `IChannelsProductService` | `AddGiftProductAsync` | POST | `/channels/ec/product/gift/add` | `AccessToken`（Query），Query `access_token` | 新增赠品（`product/gift/add`，非卖商品）。 |
| 31 | `IChannelsProductService` | `GetGiftProductAsync` | POST | `/channels/ec/product/gift/get` | `AccessToken`（Query），Query `access_token` | 获取赠品（`product/gift/get`）。 |
| 32 | `IChannelsProductService` | `GetGiftProductListAsync` | POST | `/channels/ec/product/gift/list/get` | `AccessToken`（Query），Query `access_token` | 获取赠品列表（`product/gift/list/get`）。 |
| 33 | `IChannelsProductService` | `SetGiftOnsaleAsync` | POST | `/channels/ec/product/gift/onsale/set` | `AccessToken`（Query），Query `access_token` | 设置赠品上架（`product/gift/onsale/set`）。 |
| 34 | `IChannelsProductService` | `UpdateGiftStockAsync` | POST | `/channels/ec/product/gift/stock/update` | `AccessToken`（Query），Query `access_token` | 更新赠品库存（`product/gift/stock/update`）。 |
| 35 | `IChannelsProductService` | `UpdateGiftProductAsync` | POST | `/channels/ec/product/gift/update` | `AccessToken`（Query），Query `access_token` | 更新赠品（`product/gift/update`，非卖商品）。 |
| 36 | `IChannelsProductService` | `AddGiftActivityAsync` | POST | `/channels/ec/product/activity/add` | `AccessToken`（Query），Query `access_token` | 创建买赠活动（`product/activity/add`）。 |
| 37 | `IChannelsProductService` | `DeleteGiftActivityAsync` | POST | `/channels/ec/product/activity/del` | `AccessToken`（Query），Query `access_token` | 删除买赠活动（`product/activity/del`）。 |
| 38 | `IChannelsProductService` | `StopGiftActivityAsync` | POST | `/channels/ec/product/activity/stop` | `AccessToken`（Query），Query `access_token` | 停止买赠活动（`product/activity/stop`）。 |
| 39 | `IChannelsProductService` | `AddLimitedDiscountTaskAsync` | POST | `/channels/ec/product/limiteddiscounttask/add` | `AccessToken`（Query），Query `access_token` | 创建限时抢购任务（`product/limiteddiscounttask/add`）。 |
| 40 | `IChannelsProductService` | `UpdateLimitedDiscountTaskAsync` | POST | `/channels/ec/product/limiteddiscounttask/update` | `AccessToken`（Query），Query `access_token` | 更新限时抢购任务（`product/limiteddiscounttask/update`）。 |
| 41 | `IChannelsProductService` | `DeleteLimitedDiscountTaskAsync` | POST | `/channels/ec/product/limiteddiscounttask/delete` | `AccessToken`（Query），Query `access_token` | 删除限时抢购任务（`product/limiteddiscounttask/delete`）。 |
| 42 | `IChannelsProductService` | `StopLimitedDiscountTaskAsync` | POST | `/channels/ec/product/limiteddiscounttask/stop` | `AccessToken`（Query），Query `access_token` | 停止限时抢购任务（`product/limiteddiscounttask/stop`）。 |
| 43 | `IChannelsProductService` | `GetLimitedDiscountTaskListAsync` | POST | `/channels/ec/product/limiteddiscounttask/list/get` | `AccessToken`（Query），Query `access_token` | 获取限时抢购任务列表（`product/limiteddiscounttask/list/get`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**路由形态（设计方案 v1 §4.3）**：本域全部 43 个端点走 `/channels/ec/product/*` 前缀，
无 `/shop/*` 历史前缀并存。

**草稿 / 线上双份数据语义**：添加商品只影响草稿，需经上架 + 审核通过后草稿才覆盖线上数据
（`product/add`）；`product/get` 以 `data_type` 区分返回线上 / 草稿数据。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 43 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `AddProductAsync` — 添加商品（`product/add`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/add`

- **请求体**：`ChannelsAddProductRequest` — 添加请求（`title`/`head_imgs`/`deliver_method`/`cats` 或 `cats_v2`/`skus`/`extra_service` 必填）。
- **返回**：`ChannelsAddProductResponse` — 商品 ID 与创建时间（`product_id`/`create_time`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addproduct
- 官方业务限制：①**添加商品后仅影响草稿**，需调上架接口并审核通过后才覆盖线上数据正式生效； / ②sku 数量超过 25 个时接口会异步更新；③图片（head_img / desc_info.imgs / product_qua_infos[].qua_url[] / / skus[].thumb_img 等）务必使用「上传图片」接口（resp_type=1）并回填返回的 img_url（前缀 mmecimage.cn/p/）， / 不接受其他格式。
- 专属错误码 `10020052`（商品不存在）/ `10020103`（商品数量超限）等，详见守卫。

### 2. `UpdateProductAsync` — 更新商品（`product/update`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/update`

- **请求体**：`ChannelsUpdateProductRequest` — 更新请求（在 `ChannelsAddProductRequest` 基础上带必填 `product_id`）。
- **返回**：`ChannelsUpdateProductResponse` — 商品 ID 与更新时间（`product_id`/`update_time`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updateproduct
- 官方业务限制：①草稿状态非审核中（`edit_status != 2`）时可更新；②**上架过的商品部分字段不可修改 / **（类目 / SKU 维度等），请查阅官方文档；③更新后草稿不会自动铺到线上。

### 3. `GetProductAsync` — 获取商品（`product/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/get`

- **请求体**：`ChannelsGetProductRequest` — 查询请求（`product_id` 必填；`data_type` 缺省 1）。
- **返回**：`ChannelsGetProductResponse` — 商品线上 / 草稿数据（`product`/`edit_product`）及售卖上限提醒 / 信息质量 / 高价预警 / 审核信息。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproduct
- 官方业务限制：`data_type`：1 获取线上数据，2 获取草稿数据，3 同时获取线上和草稿数据（上架过的商品才有线上数据）。

### 4. `GetProductListAsync` — 获取商品列表（`product/list/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/list/get`

- **请求体**：`ChannelsProductListRequest` — 查询请求（`page_size` 必填；`status` 不填默认拉全部商品（不含草稿与回收站））。
- **返回**：`ChannelsProductIdListResponse` — 商品 id 列表与翻页上下文（`product_ids`/`next_key`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductlist
- 官方业务限制：`status`：0 初始值，5 上架，6 回收站（仅传入 6 才返回回收站），11 所有下架商品 / （含自主下架 11 / 违规下架 15 等）；`page_size` 默认 10、不超过 30。

### 5. `ListingProductAsync` — 商品上架（`product/listing`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/listing`

- **请求体**：`ChannelsProductIdRequest` — 上架请求（`product_id` 必填；赠品 ID 亦可）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_listingproduct
- 官方业务限制：上架触发商品审核；审核中商品不可重复上架。

### 6. `DelistingProductAsync` — 商品下架（`product/delisting`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/delisting`

- **请求体**：`ChannelsProductIdRequest` — 下架请求（`product_id` 必填；赠品 ID 亦可）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_delistingproduct

### 7. `DeleteProductAsync` — 删除商品（`product/delete`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/delete`

- **请求体**：`ChannelsProductIdRequest` — 删除请求（`product_id` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_deleteproduct
- 官方业务限制：删除为**逻辑删除（移入回收站）**，可从回收站恢复（内部机制）。

### 8. `CancelProductAuditAsync` — 取消商品提审（`product/audit/cancel`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/audit/cancel`

- **请求体**：`ChannelsProductIdRequest` — 取消提审请求（`product_id` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_cancelaudit
- 官方业务限制：审核中（`edit_status==2`）的商品可取消提审，取消后回到草稿态。

### 9. `UpdateProductAuditFreeAsync` — 更新商品免审（`product/auditfree`）—— 部分字段更新（sku / 库存 / 运费 / 限购等）免审。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/auditfree`

- **请求体**：`ChannelsUpdateProductAuditFreeRequest` — 更新请求（`product_id` 与 `skus` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updateproductauditfree
- 官方业务限制：仅对**曾经上架成功过的商品**适用（草稿状态非审核中，`edit_status != 2`）； / 本地生活商品不受该规则约束。

### 10. `GetProductAuditStrategyAsync` — 获取上架策略（`product/auditstrategy/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/auditstrategy/get`
- **返回**：`ChannelsGetProductAuditStrategyResponse` — 上架策略（`audit_strategy`：隐藏商品信息上架 / 可上架相似品 / 命中低风险规则可上架）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getauditstrategy
- 官方契约：**POST** + 空请求体（官方原文「调用接口时传空的json串即可」）。

### 11. `SetProductAuditStrategyAsync` — 设置上架策略（`product/auditstrategy/set`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/auditstrategy/set`

- **请求体**：`ChannelsSetProductAuditStrategyRequest` — 设置请求（`audit_strategy` 必填；各 flag 0 不修改 / 1 打开 / 2 关闭）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_setauditstrategy

### 12. `GetProductAuditQuotaAsync` — 获取提审配额（`product/getauditquota`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/getauditquota`
- **返回**：`ChannelsGetProductAuditQuotaResponse` — 审核限额（`audit_quota`：是否限制 / 可用配额 / 总配额 / 不限判断 / 新增商品配额）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getauditquota
- 官方契约：**POST** + 空请求体；`block_status==1` 时提审商品会变为提审 quota 不足状态。

### 13. `GetProductRestrictedInfoAsync` — 获取商品受限信息（`product/getproductrestrictedinfo`，上架 / 展示受限策略查询）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/getproductrestrictedinfo`

- **请求体**：`ChannelsGetProductRestrictedInfoRequest` — 查询请求（`product_id` 必填）。
- **返回**：`ChannelsGetProductRestrictedInfoResponse` — 受限数据（`restricted_data`：受限场景 / 来源 / 描述）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductrestrictedinfo
- 官方业务限制：受限场景枚举见 `ChannelsProductRestrictedScenes`， / 受限策略来源枚举见 `ChannelsProductRestrictedSources`。

### 14. `ClassifyProductCategoryAsync` — 类目「一物一码」鉴定 / 图片分类（`product/category/classify`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/category/classify`

- **请求体**：`ChannelsProductClassifyRequest` — 鉴定请求（`cat_ids` 候选叶子类目与 `img_urls` 主图必填）。
- **返回**：`ChannelsProductClassifyResponse` — 类目鉴定结果（`cats`：命中类目列表）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getcategoryclassify

### 15. `CategoryPrecheckAsync` — 类目预检（`product/categoryprecheck`，判断商品信息是否满足类目要求）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/categoryprecheck`

- **请求体**：`ChannelsProductCategoryPrecheckRequest` — 预检请求（`cat_id` 与商品信息必填）。
- **返回**：`ChannelsProductCategoryPrecheckResponse` — 预检结果（`all_pass` 是否可用 + `fail_reasons` 不通过原因）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_categoryprecheck

### 16. `AddProductThirdPartySourceAsync` — 新增第三方货源商品（`product/addproductthirdpartysource`，分销铺货 / 商品搬家 / 其他场景）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/addproductthirdpartysource`

- **请求体**：`ChannelsAddProductThirdPartySourceRequest` — 新增请求（`scene_value`/`supplier`/`product_source_info` 必填）。
- **返回**：`ChannelsAddProductThirdPartySourceResponse` — 货源 id（`third_party_source_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addproductthirdpartysource
- 官方业务限制：场景值 `ChannelsThirdPartySourceScenes`（1 分销铺货，2 商品搬家，3 其他场景）。

### 17. `RecommendProductBrandAsync` — 品牌推荐（`product/productbrandrecommend`，根据类目与商品信息推荐品牌）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/productbrandrecommend`

- **请求体**：`ChannelsProductBrandRecommendRequest` — 推荐请求（`cat_id`/`head_imgs`/`title` 必填）。
- **返回**：`ChannelsProductBrandRecommendResponse` — 推荐品牌（`brand_id`/`brand_name_chinese`/`brand_name_english`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_productbrandrecommend

### 18. `MapExternalProductAttributeAsync` — 外部商品属性映射（`product/externalproductmapping`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/externalproductmapping`

- **请求体**：`ChannelsExternalProductMappingRequest` — 映射请求（`cat_id`/`external_attribute_name` 必填）。
- **返回**：`ChannelsExternalProductMappingResponse` — 外部属性到内部属性的映射（`external_attribute_name`/`external_attribute_value`/
`internal_attribute_name`/`internal_attribute_value`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_externalproductmapping

### 19. `MapExternalProductAttributeNewAsync` — 外部商品属性映射（新版，`product/externalproductmappingnew`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/externalproductmappingnew`

- **请求体**：`ChannelsExternalProductMappingNewRequest` — 映射请求（`cat_id`/`head_imgs`/`title`/`external_attributes` 必填）。
- **返回**：`ChannelsExternalProductMappingNewResponse` — 映射属性结果列表（`attributes`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_externalproductmappingnew

### 20. `BeginTimingSaleAsync` — 开始定时开售（`product/begintimingsale`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/begintimingsale`

- **请求体**：`ChannelsBeginTimingSaleRequest` — 开售请求（`product_id` 与 `task_id` 必填，task_id 来自商品里的字段）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_begintimingsale

### 21. `CancelTimingSaleAsync` — 取消定时开售（`product/canceltimingsale`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/canceltimingsale`

- **请求体**：`ChannelsProductIdRequest` — 取消请求（`product_id` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_canceltimingsale

### 22. `GetProductTagLinkAsync` — 获取商品微信口令（`product/taglink/get`，只支持微信内打开）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/taglink/get`

- **请求体**：`ChannelsProductTagLinkRequest` — 查询请求（`product_id` 必填；可带企微关联账号 id）。
- **返回**：`ChannelsGetProductTagLinkResponse` — 商品微信口令（`product_taglink`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproducttaglink

### 23. `GetProductQrcodeAsync` — 获取商品二维码 / 物料（`product/qrcode/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/qrcode/get`

- **请求体**：`ChannelsGetProductQrcodeRequest` — 查询请求（`product_id` 必填；`qrcode_type` 缺省 1）。
- **返回**：`ChannelsGetProductQrcodeResponse` — 商品二维码链接（`product_qrcode`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductqrcode
- 官方业务限制：`qrcode_type`：1 二维码，2 标准物料，3 送礼物物料。

### 24. `GetProductH5UrlAsync` — 获取商品 H5 短链（`product/h5url/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/h5url/get`

- **请求体**：`ChannelsGetProductH5UrlRequest` — 查询请求（`product_id` 必填；可带企微关联账号 id）。
- **返回**：`ChannelsGetProductH5UrlResponse` — 商品 H5 短链（`product_h5url`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproducth5url

### 25. `GetProductSchemeAsync` — 获取商品跳转 scheme（`product/scheme/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/scheme/get`

- **请求体**：`ChannelsGetProductSchemeRequest` — 查询请求（`product_id`/`from_appid`/`expire` 必填）。
- **返回**：`ChannelsGetProductSchemeResponse` — 商品跳转 scheme 码（`openlink`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductscheme

### 26. `GetStockAsync` — 获取库存（`product/stock/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/stock/get`

- **请求体**：`ChannelsGetStockRequest` — 查询请求（`product_id` 与 `sku_id` 必填）。
- **返回**：`ChannelsGetStockResponse` — 库存数据（`data`：通用库存 / 库存总量 / 区域库存列表）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getstock

### 27. `UpdateStockAsync` — 更新库存（`product/stock/update`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/stock/update`

- **请求体**：`ChannelsUpdateStockRequest` — 更新请求（`product_id`/`sku_id`/`diff_type`/`num` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updatestock
- 官方业务限制：`diff_type`：1 增加，2 减少，3 设置；建议使用 1 或 2， / 不建议使用 3（高并发场景可能出现预期外表现）。

### 28. `BatchGetStockAsync` — 批量获取库存（`product/stock/batchget`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/stock/batchget`

- **请求体**：`ChannelsBatchGetStockRequest` — 查询请求（`product_id` 必填，上限 50；`stock_type` 不填默认 0）。
- **返回**：`ChannelsBatchGetStockResponse` — SPU 库存列表（`data.spu_stock_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_batchgetstock
- 官方业务限制：`stock_type==1`（达人专属计划营销库存）时 `finder_id` 必填； / `stock_type` 不为 0 且不为 1 时 `stock_type_id` 必填。

### 29. `GetStockFlowAsync` — 获取库存流水（`product/stock/getflow`，库存变动对账）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/stock/getflow`

- **请求体**：`ChannelsGetStockFlowRequest` — 查询请求（`product_id`/`sku_id`/`stock_type`/`begin_time`/`end_time`/`page_size` 必填）。
- **返回**：`ChannelsGetStockFlowResponse` — 库存流水与翻页上下文（`data.stock_flow_info_list`/`data.next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getstockflow
- 官方业务限制：事件类型枚举见 `ChannelsStockFlowOpTypes`、库存子类型见 / `ChannelsStockSubTypes`；翻页回传 `next_key`。

### 30. `AddGiftProductAsync` — 新增赠品（`product/gift/add`，非卖商品）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/gift/add`

- **请求体**：`ChannelsAddGiftProductRequest` — 新增请求（`title`/`head_imgs`/`cats_v2`/`skus` 必填；仅支持单 sku）。
- **返回**：`ChannelsAddGiftProductResponse` — 非卖商品 id 与创建时间（`product_id`/`create_time`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addgift

### 31. `GetGiftProductAsync` — 获取赠品（`product/gift/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/gift/get`

- **请求体**：`ChannelsGetGiftProductRequest` — 查询请求（`product_id` 必填；`data_type` 缺省 1）。
- **返回**：`ChannelsGetGiftProductResponse` — 赠品线上 / 草稿数据（`product`/`edit_product`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getgift
- 官方业务限制：`data_type`：1 线上数据，2 草稿数据，3 同时获取线上和草稿数据； / 赠品类型见 `ProductType`（4 在售赠品只读，5 非卖赠品）。

### 32. `GetGiftProductListAsync` — 获取赠品列表（`product/gift/list/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/gift/list/get`

- **请求体**：`ChannelsProductListRequest` — 查询请求（与商品列表字段集一致 ⇒ 复用 `ChannelsProductListRequest`）。
- **返回**：`ChannelsProductIdListResponse` — 赠品 id 列表与翻页上下文（`product_ids`/`next_key`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getgiftlist

### 33. `SetGiftOnsaleAsync` — 设置赠品上架（`product/gift/onsale/set`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/gift/onsale/set`

- **请求体**：`ChannelsSetGiftOnsaleRequest` — 设置请求（`product_id` 原始商品 ID 与 `skus` 必填）。
- **返回**：`ChannelsProductIdListResponse` — 赠品 id 列表（`product_ids`/`next_key`/`total_num`；官方文档返回形态与商品列表一致，照录）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_setgiftonsale
- 官方业务限制：目前仅支持将**单品商品**设置为赠品；划拨库存直接从原始商品库存扣除、不参与售卖。

### 34. `UpdateGiftStockAsync` — 更新赠品库存（`product/gift/stock/update`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/gift/stock/update`

- **请求体**：`ChannelsUpdateStockRequest` — 更新请求（与商品库存更新字段集一致 ⇒ 复用 `ChannelsUpdateStockRequest`）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updategiftstock

### 35. `UpdateGiftProductAsync` — 更新赠品（`product/gift/update`，非卖商品）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/gift/update`

- **请求体**：`ChannelsUpdateGiftProductRequest` — 更新请求（`product_id`/`title`/`head_imgs`/`skus`/`cats_v2` 必填）。
- **返回**：`ChannelsUpdateGiftProductResponse` — 小店内部非卖商品 ID 与更新时间（`product_id`/`update_time`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_updategift

### 36. `AddGiftActivityAsync` — 创建买赠活动（`product/activity/add`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/activity/add`

- **请求体**：`ChannelsAddGiftActivityRequest` — 创建请求（`title`/`start_time`/`end_time`/`detail` 必填）。
- **返回**：`ChannelsAddGiftActivityResponse` — 买赠活动 ID（`activity_id`，创建成功后返回）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_addgiftactivity
- 官方业务限制：① `start_time` 只能取大于等于当前时间、且距离当前时间不得超过 30 天； / ② `end_time` 必须大于当前时间以及 `start_time`，活动持续时间需 ≥10 分钟、≤30 天； / ③**当前 Open API 仅支持传 0（全场景）**，`show_scene==1` 可能无法生效。

### 37. `DeleteGiftActivityAsync` — 删除买赠活动（`product/activity/del`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/activity/del`

- **请求体**：`ChannelsGiftActivityDelRequest` — 删除请求（`activity_id` 必填，官方类型为 number）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_delgiftactivity

### 38. `StopGiftActivityAsync` — 停止买赠活动（`product/activity/stop`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/activity/stop`

- **请求体**：`ChannelsGiftActivityStopRequest` — 停止请求（`activity_id` 必填，官方类型为 string，与 activity/del 的 number 不同）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_stopgiftactivity

### 39. `AddLimitedDiscountTaskAsync` — 创建限时抢购任务（`product/limiteddiscounttask/add`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/limiteddiscounttask/add`

- **请求体**：`ChannelsAddLimitedDiscountTaskRequest` — 创建请求（`product_id`/`start_time`/`end_time`/`limited_discount_skus` 必填）。
- **返回**：`ChannelsAddLimitedDiscountTaskResponse` — 限时抢购任务 ID（`task_id`，创建成功后返回）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_add
- 官方业务限制：① `start_time` 只能取大于等于当前时间（允许最多十分钟误差）、且距离当前时间不得超过 / 一年（365 天）；② 抢购价格必须小于原价（原价为 1 分钱的商品无法创建抢购任务）； / ③ 参与抢购的库存必须小于等于现有库存。

### 40. `UpdateLimitedDiscountTaskAsync` — 更新限时抢购任务（`product/limiteddiscounttask/update`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/limiteddiscounttask/update`

- **请求体**：`ChannelsUpdateLimitedDiscountTaskRequest` — 更新请求（全字段必填；`status` 为乐观锁校验快照）。
- **返回**：`ChannelsUpdateLimitedDiscountTaskResponse` — 限时抢购任务 ID 与活动名称（`task_id`/`title`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_update
- 官方业务限制：`status`：0 待开始，1 进行中（传入时校验与实际状态是否一致，乐观锁）。

### 41. `DeleteLimitedDiscountTaskAsync` — 删除限时抢购任务（`product/limiteddiscounttask/delete`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/limiteddiscounttask/delete`

- **请求体**：`ChannelsLimitedDiscountTaskIdRequest` — 删除请求（`task_id` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_delete
- 官方业务限制：任务进行中（status==1）或创建完成未开始（status==0）时不能直接调用删除接口。

### 42. `StopLimitedDiscountTaskAsync` — 停止限时抢购任务（`product/limiteddiscounttask/stop`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/limiteddiscounttask/stop`

- **请求体**：`ChannelsLimitedDiscountTaskIdRequest` — 停止请求（`task_id` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_stop

### 43. `GetLimitedDiscountTaskListAsync` — 获取限时抢购任务列表（`product/limiteddiscounttask/list/get`）。

- **声明接口**：[IChannelsProductService](./IChannelsProductService.cs)
- **官方契约**：`POST /channels/ec/product/limiteddiscounttask/list/get`

- **请求体**：`ChannelsGetLimitedDiscountTaskListRequest` — 查询请求（`page_size` 必填；`status` 不填获取所有状态）。
- **返回**：`ChannelsGetLimitedDiscountTaskListResponse` — 限时抢购任务列表、翻页上下文与总数（`limited_discount_tasks`/`next_key`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_limiteddiscounttask_listget
- 官方业务限制：状态枚举见 `ChannelsLimitedDiscountTaskStatuses`；`page_size` 默认 10、不超过 50。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/Product`](../../../Mud.Wechat.Channels/Product/) 内共 **122** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **122** 个，`SerializerClassName` = `Product`。

AOT 源生成上下文：[`ProductJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/ProductJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsAddProductResponse` | 应答 | [Product/ChannelsAddProductResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsAddProductResponse.cs) | `data` |
| `ChannelsAddProductRequest` | 请求体 | [Product/ChannelsAddProductRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsAddProductRequest.cs) | `out_product_id`, `title`, `sub_title`, `short_title`, `head_imgs`, `deliver_method`, `deliver_acct_type`, `desc_info`, `cats`, `cats_v2`, `attrs`, `spu_code` … |
| `ChannelsUpdateProductResponse` | 应答 | [Product/ChannelsUpdateProductResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsUpdateProductResponse.cs) | `data` |
| `ChannelsUpdateProductRequest` | 请求体 | [Product/ChannelsUpdateProductRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsUpdateProductRequest.cs) | `product_id` |
| `ChannelsGetProductResponse` | 应答 | [Product/ChannelsGetProductResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductResponse.cs) | `product`, `edit_product`, `sale_limit_info`, `info_score`, `cmp_price_info`, `audit_info` |
| `ChannelsGetProductRequest` | 请求体 | [Product/ChannelsGetProductRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductRequest.cs) | `product_id`, `data_type` |
| `ChannelsProductIdListResponse` | 应答 | [Product/ChannelsProductIdListResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductIdListResponse.cs) | `product_ids`, `next_key`, `total_num` |
| `ChannelsProductListRequest` | 请求体 | [Product/ChannelsProductListRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductListRequest.cs) | `status`, `page_size`, `next_key` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsProductIdRequest` | 请求体 | [Product/ChannelsProductIdRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductIdRequest.cs) | `product_id` |
| `ChannelsUpdateProductAuditFreeRequest` | 请求体 | [Product/ChannelsUpdateProductAuditFreeRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsUpdateProductAuditFreeRequest.cs) | `product_id`, `spu_code`, `skus`, `limited_info`, `express_info`, `extra_service`, `deliver_method`, `hide_in_window`, `timing_onsale_info`, `spu_deliver_info`, `after_sale_info` |
| `ChannelsGetProductAuditStrategyResponse` | 应答 | [Product/ChannelsGetProductAuditStrategyResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductAuditStrategyResponse.cs) | `audit_strategy` |
| `ChannelsSetProductAuditStrategyRequest` | 请求体 | [Product/ChannelsSetProductAuditStrategyRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsSetProductAuditStrategyRequest.cs) | `audit_strategy` |
| `ChannelsGetProductAuditQuotaResponse` | 应答 | [Product/ChannelsGetProductAuditQuotaResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductAuditQuotaResponse.cs) | `audit_quota` |
| `ChannelsGetProductRestrictedInfoResponse` | 应答 | [Product/ChannelsGetProductRestrictedInfoResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductRestrictedInfoResponse.cs) | `data` |
| `ChannelsGetProductRestrictedInfoRequest` | 请求体 | [Product/ChannelsGetProductRestrictedInfoRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductRestrictedInfoRequest.cs) | `product_ids` |
| `ChannelsProductClassifyResponse` | 应答 | [Product/ChannelsProductClassifyResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductClassifyResponse.cs) | `categories`, `wrong_cat` |
| `ChannelsProductClassifyRequest` | 请求体 | [Product/ChannelsProductClassifyRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductClassifyRequest.cs) | `req_type`, `title`, `head_imgs`, `cat_id` |
| `ChannelsProductCategoryPrecheckResponse` | 应答 | [Product/ChannelsProductCategoryPrecheckResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductCategoryPrecheckResponse.cs) | `all_pass`, `fail_reasons` |
| `ChannelsProductCategoryPrecheckRequest` | 请求体 | [Product/ChannelsProductCategoryPrecheckRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductCategoryPrecheckRequest.cs) | `cat_id` |
| `ChannelsAddProductThirdPartySourceResponse` | 应答 | [Product/ChannelsAddProductThirdPartySourceResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsAddProductThirdPartySourceResponse.cs) | `third_party_source_id` |
| `ChannelsAddProductThirdPartySourceRequest` | 请求体 | [Product/ChannelsAddProductThirdPartySourceRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsAddProductThirdPartySourceRequest.cs) | `scene_value`, `publish_method`, `supplier`, `supplier_shop_performance`, `product_source_info` |
| `ChannelsProductBrandRecommendResponse` | 应答 | [Product/ChannelsProductBrandRecommendResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductBrandRecommendResponse.cs) | `brand_id`, `brand_name_chinese`, `brand_name_english` |
| `ChannelsProductBrandRecommendRequest` | 请求体 | [Product/ChannelsProductBrandRecommendRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductBrandRecommendRequest.cs) | `cat_id`, `head_imgs`, `detail_imgs`, `title` |
| `ChannelsExternalProductMappingResponse` | 应答 | [Product/ChannelsExternalProductMappingResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsExternalProductMappingResponse.cs) | `external_attribute_name`, `external_attribute_value`, `internal_attribute_name`, `internal_attribute_value` |
| `ChannelsExternalProductMappingRequest` | 请求体 | [Product/ChannelsExternalProductMappingRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsExternalProductMappingRequest.cs) | `cat_id`, `external_attribute_name`, `external_attribute_value`, `external_category_name` |
| `ChannelsExternalProductMappingNewResponse` | 应答 | [Product/ChannelsExternalProductMappingNewResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsExternalProductMappingNewResponse.cs) | `attributes` |
| `ChannelsExternalProductMappingNewRequest` | 请求体 | [Product/ChannelsExternalProductMappingNewRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsExternalProductMappingNewRequest.cs) | `cat_id`, `external_category_name`, `head_imgs`, `detail_imgs`, `title`, `external_attributes` |
| `ChannelsBeginTimingSaleRequest` | 请求体 | [Product/ChannelsBeginTimingSaleRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsBeginTimingSaleRequest.cs) | `product_id`, `task_id` |
| `ChannelsGetProductTagLinkResponse` | 应答 | [Product/ChannelsGetProductTagLinkResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductTagLinkResponse.cs) | `product_taglink` |
| `ChannelsProductTagLinkRequest` | 请求体 | [Product/ChannelsProductTagLinkRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsProductTagLinkRequest.cs) | `product_id`, `wecom_corp_id`, `wecom_user_id` |
| `ChannelsGetProductQrcodeResponse` | 应答 | [Product/ChannelsGetProductQrcodeResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductQrcodeResponse.cs) | `product_qrcode` |
| `ChannelsGetProductQrcodeRequest` | 请求体 | [Product/ChannelsGetProductQrcodeRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductQrcodeRequest.cs) | `product_id`, `qrcode_type`, `wecom_corp_id`, `wecom_user_id` |
| `ChannelsGetProductH5UrlResponse` | 应答 | [Product/ChannelsGetProductH5UrlResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductH5UrlResponse.cs) | `product_h5url` |
| `ChannelsGetProductH5UrlRequest` | 请求体 | [Product/ChannelsGetProductH5UrlRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductH5UrlRequest.cs) | `product_id`, `wecom_corp_id`, `wecom_user_id` |
| `ChannelsGetProductSchemeResponse` | 应答 | [Product/ChannelsGetProductSchemeResponse.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductSchemeResponse.cs) | `openlink` |
| `ChannelsGetProductSchemeRequest` | 请求体 | [Product/ChannelsGetProductSchemeRequest.cs](../../../Mud.Wechat.Channels/Product/ChannelsGetProductSchemeRequest.cs) | `product_id`, `from_appid`, `expire`, `ext_info` |
| `ChannelsGetStockResponse` | 应答 | [Product/Stock/ChannelsGetStockResponse.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsGetStockResponse.cs) | `data` |
| `ChannelsGetStockRequest` | 请求体 | [Product/Stock/ChannelsGetStockRequest.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsGetStockRequest.cs) | `product_id`, `sku_id` |
| `ChannelsUpdateStockRequest` | 请求体 | [Product/Stock/ChannelsUpdateStockRequest.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsUpdateStockRequest.cs) | `product_id`, `sku_id`, `diff_type`, `num` |
| `ChannelsBatchGetStockResponse` | 应答 | [Product/Stock/ChannelsBatchGetStockResponse.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsBatchGetStockResponse.cs) | `data` |
| `ChannelsBatchGetStockRequest` | 请求体 | [Product/Stock/ChannelsBatchGetStockRequest.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsBatchGetStockRequest.cs) | `product_id`, `stock_type`, `finder_id`, `stock_type_id` |
| `ChannelsGetStockFlowResponse` | 应答 | [Product/Stock/ChannelsGetStockFlowResponse.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsGetStockFlowResponse.cs) | `data` |
| `ChannelsGetStockFlowRequest` | 请求体 | [Product/Stock/ChannelsGetStockFlowRequest.cs](../../../Mud.Wechat.Channels/Product/Stock/ChannelsGetStockFlowRequest.cs) | `product_id`, `sku_id`, `stock_type`, `finder_id`, `begin_time`, `end_time`, `op_type_list`, `page_size`, `next_key`, `stock_type_id` |
| `ChannelsAddGiftProductResponse` | 应答 | [Product/Gift/ChannelsAddGiftProductResponse.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsAddGiftProductResponse.cs) | `product_id`, `create_time` |
| `ChannelsAddGiftProductRequest` | 请求体 | [Product/Gift/ChannelsAddGiftProductRequest.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsAddGiftProductRequest.cs) | `out_product_id`, `title`, `head_imgs`, `desc_info`, `cats_v2`, `attrs`, `spu_code`, `brand_id`, `skus`, `listing` |
| `ChannelsGetGiftProductResponse` | 应答 | [Product/Gift/ChannelsGetGiftProductResponse.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsGetGiftProductResponse.cs) | `product`, `edit_product` |
| `ChannelsGetGiftProductRequest` | 请求体 | [Product/Gift/ChannelsGetGiftProductRequest.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsGetGiftProductRequest.cs) | `product_id`, `data_type` |
| `ChannelsSetGiftOnsaleRequest` | 请求体 | [Product/Gift/ChannelsSetGiftOnsaleRequest.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsSetGiftOnsaleRequest.cs) | `product_id`, `skus` |
| `ChannelsUpdateGiftProductResponse` | 应答 | [Product/Gift/ChannelsUpdateGiftProductResponse.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsUpdateGiftProductResponse.cs) | `product_id`, `update_time` |
| `ChannelsUpdateGiftProductRequest` | 请求体 | [Product/Gift/ChannelsUpdateGiftProductRequest.cs](../../../Mud.Wechat.Channels/Product/Gift/ChannelsUpdateGiftProductRequest.cs) | `product_id`, `out_product_id`, `title`, `head_imgs`, `desc_info`, `attrs`, `skus`, `listing`, `cats_v2`, `spu_code`, `brand_id` |
| `ChannelsAddGiftActivityResponse` | 应答 | [Product/Activity/ChannelsAddGiftActivityResponse.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsAddGiftActivityResponse.cs) | `activity_id` |
| `ChannelsAddGiftActivityRequest` | 请求体 | [Product/Activity/ChannelsAddGiftActivityRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsAddGiftActivityRequest.cs) | `activity_id`, `title`, `start_time`, `end_time`, `detail` |
| `ChannelsGiftActivityDelRequest` | 请求体 | [Product/Activity/ChannelsGiftActivityDelRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsGiftActivityDelRequest.cs) | `activity_id` |
| `ChannelsGiftActivityStopRequest` | 请求体 | [Product/Activity/ChannelsGiftActivityStopRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsGiftActivityStopRequest.cs) | `activity_id` |
| `ChannelsAddLimitedDiscountTaskResponse` | 应答 | [Product/Activity/ChannelsAddLimitedDiscountTaskResponse.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsAddLimitedDiscountTaskResponse.cs) | `task_id` |
| `ChannelsAddLimitedDiscountTaskRequest` | 请求体 | [Product/Activity/ChannelsAddLimitedDiscountTaskRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsAddLimitedDiscountTaskRequest.cs) | `product_id`, `start_time`, `end_time`, `limited_discount_skus`, `title`, `is_limit_purchase`, `limit_purchase_num` |
| `ChannelsUpdateLimitedDiscountTaskResponse` | 应答 | [Product/Activity/ChannelsUpdateLimitedDiscountTaskResponse.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsUpdateLimitedDiscountTaskResponse.cs) | `task_id`, `title` |
| `ChannelsUpdateLimitedDiscountTaskRequest` | 请求体 | [Product/Activity/ChannelsUpdateLimitedDiscountTaskRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsUpdateLimitedDiscountTaskRequest.cs) | `task_id`, `status`, `start_time`, `end_time`, `title`, `limited_discount_skus` |
| `ChannelsLimitedDiscountTaskIdRequest` | 请求体 | [Product/Activity/ChannelsLimitedDiscountTaskIdRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsLimitedDiscountTaskIdRequest.cs) | `task_id` |
| `ChannelsGetLimitedDiscountTaskListResponse` | 应答 | [Product/Activity/ChannelsGetLimitedDiscountTaskListResponse.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsGetLimitedDiscountTaskListResponse.cs) | `limited_discount_tasks`, `next_key`, `total_num` |
| `ChannelsGetLimitedDiscountTaskListRequest` | 请求体 | [Product/Activity/ChannelsGetLimitedDiscountTaskListRequest.cs](../../../Mud.Wechat.Channels/Product/Activity/ChannelsGetLimitedDiscountTaskListRequest.cs) | `status`, `page_size`, `next_key` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Product;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddProductApi());

var service = app.Services.GetRequiredService<IChannelsProductService>();
var ct = CancellationToken.None;
var response = await service.GetProductAuditQuotaAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/product/getauditquota");
// 占位值仅用于编译示意；业务值须按 IChannelsProductService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetProductAuditQuotaAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsProductService](./IChannelsProductService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsProductContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsProductContractGuards.cs) |
| 守卫中的代表性测试方法 | `ProductEndpoints_ShouldMatchOfficialRoutes`、`ProductEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`ProductInterface_ShouldBindChannelsToken_AndRegisterUnderProductGroup`、`ProductDataModels_ShouldBeRegisteredInJsonContext`、`ProductDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`ProductEnums_ShouldLockOfficialValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/Product`](../../../Mud.Wechat.Channels/Product/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsProductContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
