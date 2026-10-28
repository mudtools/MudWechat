# Order 域 · 订单管理

微信小店 / 视频号（channels 生态）「订单管理」域 SDK（27 端点：订单增查 / 搜索 / 改价 / 改地址 /
改备注 / 物流变更 / 发货协商 / 生鲜质检 / 礼物单 / 发货前换款 / 盲盒拆盒 / 敏感信息解密 /
虚拟号与真实号 / 商家私密号实名认证 / 用户预约发货）。 接口：[IChannelsOrderService](./IChannelsOrderService.cs)；注册组 `Order`；模块枚举 `ChannelsModule.Order`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsOrderContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsOrderContractGuards.cs)、[ChannelsMiniStoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsMiniStoreContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/Order`](../../../Mud.Wechat.Channels/Order/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsOrderService = 27；按 HTTP 特性实计 **27** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsOrderService` | `GetOrderAsync` | POST | `/channels/ec/order/get` | `AccessToken`（Query），Query `access_token` | 获取订单详情（`order/get`）。 |
| 2 | `IChannelsOrderService` | `GetOrderListAsync` | POST | `/channels/ec/order/list/get` | `AccessToken`（Query），Query `access_token` | 获取订单列表（`order/list/get`）。 |
| 3 | `IChannelsOrderService` | `SearchOrderAsync` | POST | `/channels/ec/order/search` | `AccessToken`（Query），Query `access_token` | 订单搜索（`order/search`）。 |
| 4 | `IChannelsOrderService` | `ChangeOrderPriceAsync` | POST | `/channels/ec/order/price/update` | `AccessToken`（Query），Query `access_token` | 修改订单价格（`order/price/update`）。 |
| 5 | `IChannelsOrderService` | `ChangeOrderAddressAsync` | POST | `/channels/ec/order/address/update` | `AccessToken`（Query），Query `access_token` | 修改订单地址（`order/address/update`）。 |
| 6 | `IChannelsOrderService` | `AcceptAddressModifyAsync` | POST | `/channels/ec/order/addressmodify/accept` | `AccessToken`（Query），Query `access_token` | 同意用户修改收货地址申请（`order/addressmodify/accept`）。 |
| 7 | `IChannelsOrderService` | `RejectAddressModifyAsync` | POST | `/channels/ec/order/addressmodify/reject` | `AccessToken`（Query），Query `access_token` | 拒绝买家的订单修改收货地址申请（`order/addressmodify/reject`）。 |
| 8 | `IChannelsOrderService` | `UpdateMerchantNotesAsync` | POST | `/channels/ec/order/merchantnotes/update` | `AccessToken`（Query），Query `access_token` | 修改订单备注（`order/merchantnotes/update`）。 |
| 9 | `IChannelsOrderService` | `AddPresentNoteAsync` | POST | `/channels/ec/order/presentnote/add` | `AccessToken`（Query），Query `access_token` | 礼物订单新增备注信息（`order/presentnote/add`）。 |
| 10 | `IChannelsOrderService` | `GetPresentSubOrderAsync` | POST | `/channels/ec/order/presentsuborder/get` | `AccessToken`（Query），Query `access_token` | 获取礼物单的子单列表（`order/presentsuborder/get`）。 |
| 11 | `IChannelsOrderService` | `GetPreShipmentChangeSkuListAsync` | POST | `/channels/ec/order/preshipmentchangesku/get` | `AccessToken`（Query），Query `access_token` | 获取所有待发货前更换 sku 待处理请求（`order/preshipmentchangesku/get`）。 |
| 12 | `IChannelsOrderService` | `ApprovePreShipmentChangeSkuAsync` | POST | `/channels/ec/order/preshipmentchangesku/approve` | `AccessToken`（Query），Query `access_token` | 同意待发货前更换 sku 请求（`order/preshipmentchangesku/approve`）。 |
| 13 | `IChannelsOrderService` | `RejectPreShipmentChangeSkuAsync` | POST | `/channels/ec/order/preshipmentchangesku/reject` | `AccessToken`（Query），Query `access_token` | 拒绝待发货前更换 sku 请求（`order/preshipmentchangesku/reject`）。 |
| 14 | `IChannelsOrderService` | `SetBlindBoxItemAsync` | POST | `/channels/ec/order/blindboxitem/set` | `AccessToken`（Query），Query `access_token` | 设置潮玩订单盲盒款式（`order/blindboxitem/set`）。 |
| 15 | `IChannelsOrderService` | `UpdateDeliveryInfoAsync` | POST | `/channels/ec/order/deliveryinfo/update` | `AccessToken`（Query），Query `access_token` | 修改物流信息（`order/deliveryinfo/update`）。 |
| 16 | `IChannelsOrderService` | `SubmitDeliveryNegotiationAsync` | POST | `/channels/ec/order/deliverynegotiation/submit` | `AccessToken`（Query），Query `access_token` | 提交发货协商申请（`order/deliverynegotiation/submit`）。 |
| 17 | `IChannelsOrderService` | `GetDeliveryNegotiationResultAsync` | POST | `/channels/ec/order/deliverynegotiation/result/get` | `AccessToken`（Query），Query `access_token` | 获取发货协商结果（`order/deliverynegotiation/result/get`）。 |
| 18 | `IChannelsOrderService` | `SubmitFreshInspectAsync` | POST | `/channels/ec/order/freshinspect/submit` | `AccessToken`（Query），Query `access_token` | 上传生鲜质检信息（`order/freshinspect/submit`）。 |
| 19 | `IChannelsOrderService` | `GetUserBookingListAsync` | POST | `/channels/ec/order/userbooking/list` | `AccessToken`（Query），Query `access_token` | 获取用户预约发货列表（`order/userbooking/list`）。 |
| 20 | `IChannelsOrderService` | `DecodeSensitiveInfoAsync` | POST | `/channels/ec/order/sensitiveinfo/decode` | `AccessToken`（Query），Query `access_token` | 解密订单中的详细收货信息（`order/sensitiveinfo/decode`）。 |
| 21 | `IChannelsOrderService` | `ApplyRealNumberAsync` | POST | `/channels/ec/order/realnumber/apply` | `AccessToken`（Query），Query `access_token` | 申请查看订单真实号码（`order/realnumber/apply`）。 |
| 22 | `IChannelsOrderService` | `GetRealNumberViewAuditAsync` | POST | `/channels/ec/order/realnumberviewaudit/get` | `AccessToken`（Query），Query `access_token` | 查看订单真实号审核状态（`order/realnumberviewaudit/get`）。 |
| 23 | `IChannelsOrderService` | `ApplyVirtualNumberAgainAsync` | POST | `/channels/ec/order/virtualnumber/applyagain` | `AccessToken`（Query），Query `access_token` | 订单再次申请虚拟号（`order/virtualnumber/applyagain`）。 |
| 24 | `IChannelsOrderService` | `DelayVirtualNumberAsync` | POST | `/channels/ec/order/virtualnumber/delay` | `AccessToken`（Query），Query `access_token` | 订单虚拟号延期（`order/virtualnumber/delay`）。 |
| 25 | `IChannelsOrderService` | `AddPrivatenumberPhoneAsync` | POST | `/channels/ec/merchant/privatenumber/addphone` | `AccessToken`（Query），Query `access_token` | 添加待认证的手机号（`order/merchant/privatenumber/addphone`）。 |
| 26 | `IChannelsOrderService` | `GetPrivatenumberPhoneAsync` | POST | `/channels/ec/merchant/privatenumber/getphone` | `AccessToken`（Query），Query `access_token` | 获取当前手机号的认证状态（`order/merchant/privatenumber/getphone`）。 |
| 27 | `IChannelsOrderService` | `SendPrivatenumberVerifyCodeAsync` | POST | `/channels/ec/merchant/privatenumber/sendverifycode` | `AccessToken`（Query），Query `access_token` | 获取短信验证码（`order/merchant/privatenumber/sendverifycode`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**路由形态（设计方案 v1 §2.2）**：本域 27 端点 = `/channels/ec/order/*` 24 条 +
`/channels/ec/merchant/privatenumber/*` 3 条（商家私密号实名认证），全部 POST。

**收货信息脱敏语义**：平台订单的收货信息（昵称 / 电话 / 详细地址）默认隐藏，需经
`order/sensitiveinfo/decode` 解密；虚拟号由官方分配（`virtualnumber/*`），
实名认证后才可拨打（`merchant/privatenumber/*`），真实号申请需走审核
（`realnumber/apply` + `realnumberviewaudit/get`）。

**代改址协商**：`order/address/update` 提交后进入「协商」流程，需买家同意
（`addressmodify/accept`）后才可修改成功；`order/price/update` 可改商品价与运费。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 27 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetOrderAsync` — 获取订单详情（`order/get`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/get`

- **请求体**：`ChannelsGetOrderRequest` — 查询请求（`order_id` 必填）。
- **返回**：`ChannelsGetOrderResponse` — 订单结构（`order`，含下单人 / 商品列表 / 支付 / 价格 / 配送 / 优惠券 / 结算等）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getorder
- 官方业务限制：订单收货信息为脱敏数据，详细收货信息需调 `order/sensitiveinfo/decode` 解密。

### 2. `GetOrderListAsync` — 获取订单列表（`order/list/get`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/list/get`

- **请求体**：`ChannelsGetOrderListRequest` — 查询请求（`create_time_range` / `update_time_range` 至少填一个）。
- **返回**：`ChannelsOrderIdListResponse` — 订单号列表与翻页上下文（`order_id_list`/`next_key`/`has_more`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getorderlist
- 官方业务限制：时间范围至少填一个；单次时间跨度不可超过 7 天；`page_size` 不超过 100。

### 3. `SearchOrderAsync` — 订单搜索（`order/search`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/search`

- **请求体**：`ChannelsSearchOrderRequest` — 搜索请求（`search_condition` 至少设置一个字段，`page_size`/`next_key` 必填）。
- **返回**：`ChannelsSearchOrderResponse` — 订单号列表与翻页上下文（`order_id_list`/`next_key`/`has_more`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_searchorder
- 官方业务限制：①搜索条件下的参数必须至少设置一个字段，否则接口报错；②收件人电话字段 / （`tel_number`）已废弃，请使用后四位 `tel_number_last4`；③**正在售后**定义为 / "处于非终止态"（USER_CANCELD / MERCHANT_REFUND_RETRY_FAIL / MERCHANT_FAIL / / MERCHANT_REFUND_SUCCESS / MERCHANT_RETURN_SUCCESS 之外的售后单状态）。

### 4. `ChangeOrderPriceAsync` — 修改订单价格（`order/price/update`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/price/update`

- **请求体**：`ChannelsChangeOrderPriceRequest` — 改价请求（`order_id`/`change_order_infos`/`change_express` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changeorderprice
- 官方业务限制：修改后的运费由 `express_fee` 指定（change_express 为 true 时必填，不填默认为 0，单位为分）。

### 5. `ChangeOrderAddressAsync` — 修改订单地址（`order/address/update`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/address/update`

- **请求体**：`ChannelsChangeOrderAddressRequest` — 改地址请求（`order_id`/`user_address` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changeorderaddress
- 官方业务限制：①提交修改后将进入「协商」流程，需买家同意后方可修改成功（详见「代改址协商」指南）； / ②普通订单（deliver_method=0）必填 `user_name`/`province_name`/`city_name`/ / `detail_info`/`tel_number`；虚拟商品订单（deliver_method=1）必填 `virtual_order_tel_number`。

### 6. `AcceptAddressModifyAsync` — 同意用户修改收货地址申请（`order/addressmodify/accept`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/addressmodify/accept`

- **请求体**：`ChannelsAcceptAddressModifyRequest` — 同意请求（`order_id` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_acceptorderaddressmodifyapply

### 7. `RejectAddressModifyAsync` — 拒绝买家的订单修改收货地址申请（`order/addressmodify/reject`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/addressmodify/reject`

- **请求体**：`ChannelsRejectAddressModifyRequest` — 拒绝请求（`order_id` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_rejectorderaddressmodifyapply

### 8. `UpdateMerchantNotesAsync` — 修改订单备注（`order/merchantnotes/update`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/merchantnotes/update`

- **请求体**：`ChannelsUpdateMerchantNotesRequest` — 备注请求（`order_id`/`merchant_notes` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changemerchantnotes

### 9. `AddPresentNoteAsync` — 礼物订单新增备注信息（`order/presentnote/add`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/presentnote/add`

- **请求体**：`ChannelsAddPresentNoteRequest` — 备注请求（`present_order_id`/`notes` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_presentnote

### 10. `GetPresentSubOrderAsync` — 获取礼物单的子单列表（`order/presentsuborder/get`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/presentsuborder/get`

- **请求体**：`ChannelsGetPresentSubOrderRequest` — 查询请求（`present_order_id` 必填）。
- **返回**：`ChannelsGetPresentSubOrderResponse` — 礼物单对应的子单订单号列表（`order_ids`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getpresentsuborder

### 11. `GetPreShipmentChangeSkuListAsync` — 获取所有待发货前更换 sku 待处理请求（`order/preshipmentchangesku/get`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/preshipmentchangesku/get`

- **请求体**：`ChannelsGetPreShipmentChangeSkuRequest` — 分页请求（`page`/`page_size` 必填）。
- **返回**：`ChannelsGetPreShipmentChangeSkuResponse` — 等待商家处理的换款请求订单 id 列表（`order_ids`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getpreshipmentchangeskuwaithandlelist

### 12. `ApprovePreShipmentChangeSkuAsync` — 同意待发货前更换 sku 请求（`order/preshipmentchangesku/approve`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/preshipmentchangesku/approve`

- **请求体**：`ChannelsApprovePreShipmentChangeSkuRequest` — 同意请求（`order_id` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_approvepreshipmentchangesku

### 13. `RejectPreShipmentChangeSkuAsync` — 拒绝待发货前更换 sku 请求（`order/preshipmentchangesku/reject`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/preshipmentchangesku/reject`

- **请求体**：`ChannelsRejectPreShipmentChangeSkuRequest` — 拒绝请求（`order_id` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_rejectpreshipmentchangesku

### 14. `SetBlindBoxItemAsync` — 设置潮玩订单盲盒款式（`order/blindboxitem/set`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/blindboxitem/set`

- **请求体**：`ChannelsSetBlindBoxItemRequest` — 回传请求（`order_id`/`item_list` 必填）。
- **返回**：`ChannelsSetBlindBoxItemResponse` — 设置失败的盲盒款式列表（`fail_item_list`，空即全部成功）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_setblindboxitem
- 官方业务限制：商家将潮玩订单拆盒结果回传给小店订单，与 `product_infos[].order_product_item_list[].item_unique_id` 一一对应。

### 15. `UpdateDeliveryInfoAsync` — 修改物流信息（`order/deliveryinfo/update`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/deliveryinfo/update`

- **请求体**：`ChannelsUpdateDeliveryInfoRequest` — 修改请求（`order_id` 必填；`delivery_list` / `change_infos` 二选一）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_changedeliveryinfo
- 官方业务限制：①整单物流信息（`delivery_list`）不支持拆单；②更新包裹物流信息（`change_infos`） / 支持拆单，`old`/`new` 各含原 / 新快递信息；③发货方式目前仅支持 1（自寄快递）。

### 16. `SubmitDeliveryNegotiationAsync` — 提交发货协商申请（`order/deliverynegotiation/submit`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/deliverynegotiation/submit`

- **请求体**：`ChannelsSubmitDeliveryNegotiationRequest` — 协商请求（`order_id`/`predict_delivery_time`/`reason` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_submitdeliverynegotiation
- 官方业务限制：发货超时订单可提交发货协商，与买家协商新的预计发货时间；处理结果经 `deliverynegotiation/result/get` 查询。

### 17. `GetDeliveryNegotiationResultAsync` — 获取发货协商结果（`order/deliverynegotiation/result/get`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/deliverynegotiation/result/get`

- **请求体**：`ChannelsGetDeliveryNegotiationResultRequest` — 查询请求（`order_id_list` 必填）。
- **返回**：`ChannelsGetDeliveryNegotiationResultResponse` — 协商结果列表（`negotiation_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_querydeliverynegotiation

### 18. `SubmitFreshInspectAsync` — 上传生鲜质检信息（`order/freshinspect/submit`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/freshinspect/submit`

- **请求体**：`ChannelsSubmitFreshInspectRequest` — 上传请求（`order_id`/`audit_items` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_submitfreshinspectinfo
- 官方业务限制：给生鲜类质检订单上传商品打包信息；审核项名称见 / `ChannelsFreshInspectItemNames`（快递单图片 / 包装箱全景视频 / 开箱全景视频 / 单个细节全景视频）。

### 19. `GetUserBookingListAsync` — 获取用户预约发货列表（`order/userbooking/list`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/userbooking/list`

- **请求体**：`ChannelsGetUserBookingListRequest` — 查询请求（`time_range_order_create` / `time_range_booking_predict` 必填）。
- **返回**：`ChannelsGetUserBookingListResponse` — 预约发货记录列表与翻页信息（`record_list`/`next_page`/`total`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_pullshopuserbookinglist
- 官方业务限制：①两个时间范围均必填且间隔不超过 7 天；②预约类型 `booking_type`：1=指定时间发货，2=暂不发货； / ③分页 `limit` 默认为 20、不能超过 100。

### 20. `DecodeSensitiveInfoAsync` — 解密订单中的详细收货信息（`order/sensitiveinfo/decode`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/sensitiveinfo/decode`

- **请求体**：`ChannelsDecodeSensitiveInfoRequest` — 解密请求（`order_id` 必填）。
- **返回**：`ChannelsDecodeSensitiveInfoResponse` — 收货信息与虚拟号信息（`address_info`/`virtual_number_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_decodesensitiveinfo
- 官方业务限制：为保护用户隐私（收货人昵称、电话号码、详细收货地址），平台订单的收货信息进行了部分隐藏。

### 21. `ApplyRealNumberAsync` — 申请查看订单真实号码（`order/realnumber/apply`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/realnumber/apply`

- **请求体**：`ChannelsApplyRealNumberRequest` — 申请请求（`order_id`/`apply_type`/`apply_reason`/`pic_media_ids` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_applyrealnumber
- 官方业务限制：订单使用虚拟号且虚拟号不满足需求时提交申请查看真实号；申请原因图片 1-5 张；审核状态经 `realnumberviewaudit/get` 查询。

### 22. `GetRealNumberViewAuditAsync` — 查看订单真实号审核状态（`order/realnumberviewaudit/get`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/realnumberviewaudit/get`

- **请求体**：`ChannelsGetRealNumberViewAuditRequest` — 查询请求（`order_id` 必填）。
- **返回**：`ChannelsGetRealNumberViewAuditResponse` — 申请时间 / 审核状态 / 申请原因 / 申请 id。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_getrealnumberviewaudit
- 官方业务限制：审核状态 `audit_state`：1 审核中、2 审核拒绝、3 审核通过（见 `ChannelsRealNumberAuditStates`）。

### 23. `ApplyVirtualNumberAgainAsync` — 订单再次申请虚拟号（`order/virtualnumber/applyagain`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/virtualnumber/applyagain`

- **请求体**：`ChannelsApplyVirtualNumberAgainRequest` — 申请请求（`order_id` 必填）。
- **返回**：`ChannelsApplyVirtualNumberAgainResponse` — 虚拟号 / 分机号 / 剩余申请次数。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_applyvirtualnumberagain
- 官方业务限制：订单虚拟号过期之后，可再次申请一次虚拟号。

### 24. `DelayVirtualNumberAsync` — 订单虚拟号延期（`order/virtualnumber/delay`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/order/virtualnumber/delay`

- **请求体**：`ChannelsDelayVirtualNumberRequest` — 延期请求（`order_id`/`has_delay_times` 必填）。
- **返回**：`ChannelsDelayVirtualNumberResponse` — 过期时间与可延期次数（`expiration`/`available_extend_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_delayvirtualnumber
- 官方业务限制：订单虚拟号过期前 7 天可通过该接口延期；`has_delay_times` 为当前已延期次数（可通过 获取订单详情 拿到，不填默认为 0）。

### 25. `AddPrivatenumberPhoneAsync` — 添加待认证的手机号（`order/merchant/privatenumber/addphone`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/merchant/privatenumber/addphone`

- **请求体**：`ChannelsPrivatenumberAddPhoneRequest` — 认证请求（`mobile`/`verify_code`/`wxusername` 必填）。
- **返回**：`ChannelsPrivatenumberAddPhoneResponse` — 认证链接与认证状态（`qrcode_url`/`status`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_privatenumberaddphone
- 官方业务限制：【虚拟号拨打实名认证】添加待认证的手机号并返回运营商实名认证的页面地址； / `verify_code` 经 `privatenumber/sendverifycode` 获取；认证状态：1=认证成功、2=认证失败、3=运营商审核中。

### 26. `GetPrivatenumberPhoneAsync` — 获取当前手机号的认证状态（`order/merchant/privatenumber/getphone`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/merchant/privatenumber/getphone`

- **请求体**：`ChannelsPrivatenumberGetPhoneRequest` — 查询请求（`mobile` 必填）。
- **返回**：`ChannelsPrivatenumberGetPhoneResponse` — 认证状态与审核失败原因（`status`/`fail_reason`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_privatenumbergetshopphone

### 27. `SendPrivatenumberVerifyCodeAsync` — 获取短信验证码（`order/merchant/privatenumber/sendverifycode`）。

- **声明接口**：[IChannelsOrderService](./IChannelsOrderService.cs)
- **官方契约**：`POST /channels/ec/merchant/privatenumber/sendverifycode`

- **请求体**：`ChannelsPrivatenumberSendVerifyCodeRequest` — 发送请求（`mobile` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-order/api_privatenumbersendverifycode
- 官方业务限制：【虚拟号拨打实名认证】第一步，获取手机号验证码。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/Order`](../../../Mud.Wechat.Channels/Order/) 内共 **93** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **93** 个，`SerializerClassName` = `Order`。

AOT 源生成上下文：[`OrderJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/OrderJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsGetOrderResponse` | 应答 | [Order/ChannelsGetOrderResponse.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetOrderResponse.cs) | `order` |
| `ChannelsGetOrderRequest` | 请求体 | [Order/ChannelsGetOrderRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetOrderRequest.cs) | `order_id` |
| `ChannelsOrderIdListResponse` | 应答 | [Order/ChannelsOrderIdListResponse.cs](../../../Mud.Wechat.Channels/Order/ChannelsOrderIdListResponse.cs) | `order_id_list`, `next_key`, `has_more` |
| `ChannelsGetOrderListRequest` | 请求体 | [Order/ChannelsGetOrderListRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetOrderListRequest.cs) | `create_time_range`, `update_time_range`, `status`, `openid`, `page_size`, `next_key` |
| `ChannelsSearchOrderResponse` | 应答 | [Order/ChannelsSearchOrderResponse.cs](../../../Mud.Wechat.Channels/Order/ChannelsSearchOrderResponse.cs) | — |
| `ChannelsSearchOrderRequest` | 请求体 | [Order/ChannelsSearchOrderRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsSearchOrderRequest.cs) | `search_condition`, `page_size`, `next_key` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsChangeOrderPriceRequest` | 请求体 | [Order/ChannelsChangeOrderPriceRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsChangeOrderPriceRequest.cs) | `order_id`, `change_order_infos`, `change_express`, `express_fee` |
| `ChannelsChangeOrderAddressRequest` | 请求体 | [Order/ChannelsChangeOrderAddressRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsChangeOrderAddressRequest.cs) | `order_id`, `user_address` |
| `ChannelsAcceptAddressModifyRequest` | 请求体 | [Order/ChannelsAcceptAddressModifyRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsAcceptAddressModifyRequest.cs) | `order_id` |
| `ChannelsRejectAddressModifyRequest` | 请求体 | [Order/ChannelsRejectAddressModifyRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsRejectAddressModifyRequest.cs) | `order_id` |
| `ChannelsUpdateMerchantNotesRequest` | 请求体 | [Order/ChannelsUpdateMerchantNotesRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsUpdateMerchantNotesRequest.cs) | `order_id`, `merchant_notes`, `merchant_notes_tag_color` |
| `ChannelsAddPresentNoteRequest` | 请求体 | [Order/ChannelsAddPresentNoteRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsAddPresentNoteRequest.cs) | `present_order_id`, `notes` |
| `ChannelsGetPresentSubOrderResponse` | 应答 | [Order/ChannelsGetPresentSubOrderResponse.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetPresentSubOrderResponse.cs) | `order_ids` |
| `ChannelsGetPresentSubOrderRequest` | 请求体 | [Order/ChannelsGetPresentSubOrderRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetPresentSubOrderRequest.cs) | `present_order_id` |
| `ChannelsGetPreShipmentChangeSkuResponse` | 应答 | [Order/ChannelsGetPreShipmentChangeSkuResponse.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetPreShipmentChangeSkuResponse.cs) | `order_ids` |
| `ChannelsGetPreShipmentChangeSkuRequest` | 请求体 | [Order/ChannelsGetPreShipmentChangeSkuRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsGetPreShipmentChangeSkuRequest.cs) | `page`, `page_size` |
| `ChannelsApprovePreShipmentChangeSkuRequest` | 请求体 | [Order/ChannelsApprovePreShipmentChangeSkuRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsApprovePreShipmentChangeSkuRequest.cs) | `order_id` |
| `ChannelsRejectPreShipmentChangeSkuRequest` | 请求体 | [Order/ChannelsRejectPreShipmentChangeSkuRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsRejectPreShipmentChangeSkuRequest.cs) | `order_id` |
| `ChannelsSetBlindBoxItemResponse` | 应答 | [Order/ChannelsSetBlindBoxItemResponse.cs](../../../Mud.Wechat.Channels/Order/ChannelsSetBlindBoxItemResponse.cs) | `fail_item_list` |
| `ChannelsSetBlindBoxItemRequest` | 请求体 | [Order/ChannelsSetBlindBoxItemRequest.cs](../../../Mud.Wechat.Channels/Order/ChannelsSetBlindBoxItemRequest.cs) | `order_id`, `item_list` |
| `ChannelsUpdateDeliveryInfoRequest` | 请求体 | [Order/Delivery/ChannelsUpdateDeliveryInfoRequest.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsUpdateDeliveryInfoRequest.cs) | `order_id`, `delivery_list`, `change_infos` |
| `ChannelsSubmitDeliveryNegotiationRequest` | 请求体 | [Order/Delivery/ChannelsSubmitDeliveryNegotiationRequest.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsSubmitDeliveryNegotiationRequest.cs) | `order_id`, `predict_delivery_time`, `reason`, `remark` |
| `ChannelsGetDeliveryNegotiationResultResponse` | 应答 | [Order/Delivery/ChannelsGetDeliveryNegotiationResultResponse.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsGetDeliveryNegotiationResultResponse.cs) | `negotiation_list` |
| `ChannelsGetDeliveryNegotiationResultRequest` | 请求体 | [Order/Delivery/ChannelsGetDeliveryNegotiationResultRequest.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsGetDeliveryNegotiationResultRequest.cs) | `order_id_list` |
| `ChannelsSubmitFreshInspectRequest` | 请求体 | [Order/Delivery/ChannelsSubmitFreshInspectRequest.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsSubmitFreshInspectRequest.cs) | `order_id`, `audit_items` |
| `ChannelsGetUserBookingListResponse` | 应答 | [Order/Delivery/ChannelsGetUserBookingListResponse.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsGetUserBookingListResponse.cs) | `record_list`, `next_page`, `total` |
| `ChannelsGetUserBookingListRequest` | 请求体 | [Order/Delivery/ChannelsGetUserBookingListRequest.cs](../../../Mud.Wechat.Channels/Order/Delivery/ChannelsGetUserBookingListRequest.cs) | `order_id_list`, `booking_type`, `time_range_order_create`, `time_range_booking_predict`, `page_info` |
| `ChannelsDecodeSensitiveInfoResponse` | 应答 | [Order/SensitiveInfo/ChannelsDecodeSensitiveInfoResponse.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsDecodeSensitiveInfoResponse.cs) | `address_info`, `virtual_number_info` |
| `ChannelsDecodeSensitiveInfoRequest` | 请求体 | [Order/SensitiveInfo/ChannelsDecodeSensitiveInfoRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsDecodeSensitiveInfoRequest.cs) | `order_id` |
| `ChannelsApplyRealNumberRequest` | 请求体 | [Order/SensitiveInfo/ChannelsApplyRealNumberRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsApplyRealNumberRequest.cs) | `order_id`, `apply_type`, `apply_reason`, `pic_media_ids` |
| `ChannelsGetRealNumberViewAuditResponse` | 应答 | [Order/SensitiveInfo/ChannelsGetRealNumberViewAuditResponse.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsGetRealNumberViewAuditResponse.cs) | `apply_time`, `audit_state`, `apply_reason`, `task_id` |
| `ChannelsGetRealNumberViewAuditRequest` | 请求体 | [Order/SensitiveInfo/ChannelsGetRealNumberViewAuditRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsGetRealNumberViewAuditRequest.cs) | `order_id` |
| `ChannelsApplyVirtualNumberAgainResponse` | 应答 | [Order/SensitiveInfo/ChannelsApplyVirtualNumberAgainResponse.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsApplyVirtualNumberAgainResponse.cs) | `virtual_number`, `extension`, `apply_quota` |
| `ChannelsApplyVirtualNumberAgainRequest` | 请求体 | [Order/SensitiveInfo/ChannelsApplyVirtualNumberAgainRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsApplyVirtualNumberAgainRequest.cs) | `order_id` |
| `ChannelsDelayVirtualNumberResponse` | 应答 | [Order/SensitiveInfo/ChannelsDelayVirtualNumberResponse.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsDelayVirtualNumberResponse.cs) | `expiration`, `available_extend_num` |
| `ChannelsDelayVirtualNumberRequest` | 请求体 | [Order/SensitiveInfo/ChannelsDelayVirtualNumberRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsDelayVirtualNumberRequest.cs) | `order_id`, `has_delay_times` |
| `ChannelsPrivatenumberAddPhoneResponse` | 应答 | [Order/SensitiveInfo/ChannelsPrivatenumberAddPhoneResponse.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsPrivatenumberAddPhoneResponse.cs) | `qrcode_url`, `status` |
| `ChannelsPrivatenumberAddPhoneRequest` | 请求体 | [Order/SensitiveInfo/ChannelsPrivatenumberAddPhoneRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsPrivatenumberAddPhoneRequest.cs) | `mobile`, `verify_code`, `wxusername` |
| `ChannelsPrivatenumberGetPhoneResponse` | 应答 | [Order/SensitiveInfo/ChannelsPrivatenumberGetPhoneResponse.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsPrivatenumberGetPhoneResponse.cs) | `status`, `fail_reason` |
| `ChannelsPrivatenumberGetPhoneRequest` | 请求体 | [Order/SensitiveInfo/ChannelsPrivatenumberGetPhoneRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsPrivatenumberGetPhoneRequest.cs) | `mobile` |
| `ChannelsPrivatenumberSendVerifyCodeRequest` | 请求体 | [Order/SensitiveInfo/ChannelsPrivatenumberSendVerifyCodeRequest.cs](../../../Mud.Wechat.Channels/Order/SensitiveInfo/ChannelsPrivatenumberSendVerifyCodeRequest.cs) | `mobile` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Order;
using Mud.Wechat.Channels.DataModels;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddOrderApi());

var service = app.Services.GetRequiredService<IChannelsOrderService>();
var ct = CancellationToken.None;
var response = await service.AcceptAddressModifyAsync(new ChannelsAcceptAddressModifyRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/order/addressmodify/accept");
// 占位值仅用于编译示意；业务值须按 IChannelsOrderService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AcceptAddressModifyAsync`）；请求体 `ChannelsAcceptAddressModifyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsOrderService](./IChannelsOrderService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsOrderContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsOrderContractGuards.cs)、[`ChannelsMiniStoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsMiniStoreContractGuards.cs) |
| 守卫中的代表性测试方法 | `OrderEndpoints_ShouldMatchOfficialRoutes`、`OrderEndpoints_ShouldAllBePost_WithBody`、`OrderInterface_ShouldBindChannelsToken_AndRegisterUnderOrderGroup`、`OrderDataModels_ShouldBeRegisteredInJsonContext`、`OrderDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`OrderEnums_ShouldLockOfficialValues`、`MiniStoreEndpoints_ShouldMatchOfficialRoutes`、`MiniStoreEndpoints_ShouldAllBePost_WithUploadQueryRules`、`MiniStoreInterface_ShouldBindChannelsToken_AndRegisterUnderMiniStoreGroup`、`MiniStoreDataModels_ShouldBeRegisteredInJsonContext`、`MiniStoreDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`MiniStoreEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/Order`](../../../Mud.Wechat.Channels/Order/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsOrderContractGuards.cs`、`ChannelsMiniStoreContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
