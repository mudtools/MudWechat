# Logistics 域 · 物流发货

微信小店 / 视频号（channels 生态）「物流发货」域 SDK（29 端点：地址 5 / 运费模板 4 / 电子面单 17 / 订单发货 3）。 接口：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)；注册组 `Logistics`；模块枚举 `ChannelsModule.Logistics`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsLogisticsContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLogisticsContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/Logistics`](../../../Mud.Wechat.Channels.DataModels/Logistics/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsLogisticsService = 29；按 HTTP 特性实计 **29** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsLogisticsService` | `AddAddressAsync` | POST | `/channels/ec/merchant/address/add` | `AccessToken`（Query），Query `access_token` | 添加地址（`merchant/address/add`）。 |
| 2 | `IChannelsLogisticsService` | `GetAddressAsync` | POST | `/channels/ec/merchant/address/get` | `AccessToken`（Query），Query `access_token` | 获取地址详情（`merchant/address/get`）。 |
| 3 | `IChannelsLogisticsService` | `GetAddressListAsync` | POST | `/channels/ec/merchant/address/list` | `AccessToken`（Query），Query `access_token` | 获取地址列表（`merchant/address/list`）。 |
| 4 | `IChannelsLogisticsService` | `DeleteAddressAsync` | POST | `/channels/ec/merchant/address/delete` | `AccessToken`（Query），Query `access_token` | 删除地址（`merchant/address/delete`）。 |
| 5 | `IChannelsLogisticsService` | `UpdateAddressAsync` | POST | `/channels/ec/merchant/address/update` | `AccessToken`（Query），Query `access_token` | 更新地址（`merchant/address/update`）。 |
| 6 | `IChannelsLogisticsService` | `AddFreightTemplateAsync` | POST | `/channels/ec/merchant/addfreighttemplate` | `AccessToken`（Query），Query `access_token` | 增加运费模板（`merchant/addfreighttemplate`）。 |
| 7 | `IChannelsLogisticsService` | `GetFreightTemplateDetailAsync` | POST | `/channels/ec/merchant/getfreighttemplatedetail` | `AccessToken`（Query），Query `access_token` | 查询运费模板（`merchant/getfreighttemplatedetail`）。 |
| 8 | `IChannelsLogisticsService` | `GetFreightTemplateListAsync` | POST | `/channels/ec/merchant/getfreighttemplatelist` | `AccessToken`（Query），Query `access_token` | 获取运费模板列表（`merchant/getfreighttemplatelist`）。 |
| 9 | `IChannelsLogisticsService` | `UpdateFreightTemplateAsync` | POST | `/channels/ec/merchant/updatefreighttemplate` | `AccessToken`（Query），Query `access_token` | 更新运费模板（`merchant/updatefreighttemplate`）。 |
| 10 | `IChannelsLogisticsService` | `CreateEwaybillOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/create` | `AccessToken`（Query），Query `access_token` | 电子面单取号（`ewaybill/biz/order/create`）。 |
| 11 | `IChannelsLogisticsService` | `PrecreateEwaybillOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/precreate` | `AccessToken`（Query），Query `access_token` | 电子面单预取号（`ewaybill/biz/order/precreate`）。 |
| 12 | `IChannelsLogisticsService` | `GetEwaybillOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/get` | `AccessToken`（Query），Query `access_token` | 查询面单详情（`ewaybill/biz/order/get`）。 |
| 13 | `IChannelsLogisticsService` | `CancelEwaybillOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/cancel` | `AccessToken`（Query），Query `access_token` | 电子面单取消下单（`ewaybill/biz/order/cancel`）。 |
| 14 | `IChannelsLogisticsService` | `PrintEwaybillOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/print` | `AccessToken`（Query），Query `access_token` | 打印成功通知（`ewaybill/biz/order/print`）。 |
| 15 | `IChannelsLogisticsService` | `BatchPrintEwaybillOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/batchprint` | `AccessToken`（Query），Query `access_token` | 批量打印通知（`ewaybill/biz/order/batchprint`）。 |
| 16 | `IChannelsLogisticsService` | `AddEwaybillSubOrderAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/addsuborder` | `AccessToken`（Query），Query `access_token` | 电子面单子件追加（`ewaybill/biz/order/addsuborder`）。 |
| 17 | `IChannelsLogisticsService` | `GetEwaybillDeliveryListAsync` | POST | `/channels/ec/logistics/ewaybill/biz/delivery/get` | `AccessToken`（Query），Query `access_token` | 查询开通的快递公司列表（`ewaybill/biz/delivery/get`）。 |
| 18 | `IChannelsLogisticsService` | `GetEwaybillPrintContentAsync` | POST | `/channels/ec/logistics/ewaybill/biz/print/get` | `AccessToken`（Query），Query `access_token` | 获取打印报文（`ewaybill/biz/print/get`）。 |
| 19 | `IChannelsLogisticsService` | `GetSupplierEwaybillPrintContentAsync` | POST | `/channels/ec/logistics/ewaybill/biz/order/getprintinfo` | `AccessToken`（Query），Query `access_token` | 获取打印报文（供货商视角，`ewaybill/biz/order/getprintinfo`）。 |
| 20 | `IChannelsLogisticsService` | `GetEwaybillTemplateConfigAsync` | POST | `/channels/ec/logistics/ewaybill/biz/template/config` | `AccessToken`（Query），Query `access_token` | 获取面单标准模板（`ewaybill/biz/template/config`）。 |
| 21 | `IChannelsLogisticsService` | `GetEwaybillTemplateByIdAsync` | POST | `/channels/ec/logistics/ewaybill/biz/template/getbyid` | `AccessToken`（Query），Query `access_token` | 根据模板 ID 获取面单模板信息（`ewaybill/biz/template/getbyid`）。 |
| 22 | `IChannelsLogisticsService` | `GetEwaybillTemplateAsync` | POST | `/channels/ec/logistics/ewaybill/biz/template/get` | `AccessToken`（Query），Query `access_token` | 获取面单模板信息（`ewaybill/biz/template/get`）。 |
| 23 | `IChannelsLogisticsService` | `CreateEwaybillTemplateAsync` | POST | `/channels/ec/logistics/ewaybill/biz/template/create` | `AccessToken`（Query），Query `access_token` | 新增面单模板（`ewaybill/biz/template/create`）。 |
| 24 | `IChannelsLogisticsService` | `UpdateEwaybillTemplateAsync` | POST | `/channels/ec/logistics/ewaybill/biz/template/update` | `AccessToken`（Query），Query `access_token` | 更新面单模板（`ewaybill/biz/template/update`）。 |
| 25 | `IChannelsLogisticsService` | `DeleteEwaybillTemplateAsync` | POST | `/channels/ec/logistics/ewaybill/biz/template/delete` | `AccessToken`（Query），Query `access_token` | 删除面单模板（`ewaybill/biz/template/delete`）。 |
| 26 | `IChannelsLogisticsService` | `GetEwaybillAcctAsync` | POST | `/channels/ec/logistics/ewaybill/biz/account/get` | `AccessToken`（Query），Query `access_token` | 查询开通的电子面单网点 / 账号信息（`ewaybill/biz/account/get`）。 |
| 27 | `IChannelsLogisticsService` | `SendDeliveryAsync` | POST | `/channels/ec/order/delivery/send` | `AccessToken`（Query），Query `access_token` | 订单发货（`order/delivery/send`）。 |
| 28 | `IChannelsLogisticsService` | `GetDeliveryCompanyListAsync` | POST | `/channels/ec/order/deliverycompanylist/new/get` | `AccessToken`（Query），Query `access_token` | 获取快递公司列表（`order/deliverycompanylist/new/get`）。 |
| 29 | `IChannelsLogisticsService` | `DeliveryCompensationAsync` | POST | `/channels/ec/order/delivery/compensation` | `AccessToken`（Query），Query `access_token` | 订单补发货（`order/delivery/compensation`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**路由形态（设计方案 v1 §2.2）**：本域 29 端点全部 POST，走三组前缀 ——
`/channels/ec/merchant/address|freight*`（地址 5 + 运费模板 4）、
`/channels/ec/logistics/ewaybill/*`（电子面单 17）、`/channels/ec/order/delivery*`（订单发货 3）。

**地址（5）**：添加 / 获取 / 列表 / 删除 / 更新地址（`merchant/address/*`）。

**运费模板（4）**：增加 / 查询详情 / 列表 / 更新（`merchant/add|get|updatefreighttemplate`）。

**电子面单（17）**：取号 create / 预取号 precreate / 查询详情 get / 取消 cancel / 打印成功通知 print /
批量打印通知 batchprint / 子件追加 addsuborder / 开通快递公司列表 delivery/get / 获取打印报文 print/get（商家）与 order/getprintinfo（供货商）/
面单标准模板 template/config / 模板 getbyid / 模板信息 get / 新增模板 create / 更新模板 update / 删除模板 delete /
开通网点账号 account/get。

**订单发货（3）**：订单发货 send / 获取快递公司列表 deliverycompanylist/new/get / 订单补发货 compensation。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 29 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `AddAddressAsync` — 添加地址（`merchant/address/add`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/address/add`

- **请求体**：`ChannelsLogisticsAddAddressRequest` — 添加请求（`address_detail` 必填）。
- **返回**：`ChannelsLogisticsAddAddressResponse` — 新增地址 id（`address_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_addaddress

### 2. `GetAddressAsync` — 获取地址详情（`merchant/address/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/address/get`

- **请求体**：`ChannelsLogisticsAddressIdRequest` — 查询请求（`address_id` 必填）。
- **返回**：`ChannelsLogisticsGetAddressResponse` — 地址详情（`address_detail`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getaddress

### 3. `GetAddressListAsync` — 获取地址列表（`merchant/address/list`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/address/list`

- **请求体**：`ChannelsLogisticsGetAddressListRequest` — 查询请求（`offset`+`limit` 必填）。
- **返回**：`ChannelsLogisticsGetAddressListResponse` — 地址 id 列表（`address_id_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getaddresslist

### 4. `DeleteAddressAsync` — 删除地址（`merchant/address/delete`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/address/delete`

- **请求体**：`ChannelsLogisticsAddressIdRequest` — 删除请求（`address_id` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_deleteaddress

### 5. `UpdateAddressAsync` — 更新地址（`merchant/address/update`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/address/update`

- **请求体**：`ChannelsLogisticsUpdateAddressRequest` — 更新请求（`address_detail` 必填，原地覆盖写入须带全字段）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_updateaddress

### 6. `AddFreightTemplateAsync` — 增加运费模板（`merchant/addfreighttemplate`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/addfreighttemplate`

- **请求体**：`ChannelsLogisticsAddFreightTemplateRequest` — 添加请求（`freight_template` 必填）。
- **返回**：`ChannelsLogisticsAddFreightTemplateResponse` — 运费模板 id（`template_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_addfreighttemplate
- 官方业务限制：`valuation_type` PIECE 按件数 / WEIGHT 按重量；`shipping_method` FREE 包邮 / / CONDITION_FREE 条件包邮 / NO_FREE 不包邮；`delivery_type` 仅 EXPRESS 快递。

### 7. `GetFreightTemplateDetailAsync` — 查询运费模板（`merchant/getfreighttemplatedetail`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/getfreighttemplatedetail`

- **请求体**：`ChannelsLogisticsGetFreightTemplateDetailRequest` — 查询请求（`template_id` 必填）。
- **返回**：`ChannelsLogisticsGetFreightTemplateDetailResponse` — 运费模板详细信息（`freight_template`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getfreighttemplatedetail

### 8. `GetFreightTemplateListAsync` — 获取运费模板列表（`merchant/getfreighttemplatelist`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/getfreighttemplatelist`

- **请求体**：`ChannelsLogisticsGetFreightTemplateListRequest` — 查询请求（`offset`+`limit` 必填）。
- **返回**：`ChannelsLogisticsGetFreightTemplateListResponse` — 运费模板 id 列表（`template_id_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getfreighttemplatelist

### 9. `UpdateFreightTemplateAsync` — 更新运费模板（`merchant/updatefreighttemplate`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/merchant/updatefreighttemplate`

- **请求体**：`ChannelsLogisticsUpdateFreightTemplateRequest` — 更新请求（`freight_template` 必填，原地覆盖写入须带全字段）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_updatefreighttemplate

### 10. `CreateEwaybillOrderAsync` — 电子面单取号（`ewaybill/biz/order/create`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/create`

- **请求体**：`ChannelsLogisticsEwaybillCreateOrderRequest` — 取号请求（`delivery_id`/`ewaybill_acct_id`/`sender`/`receiver`/`ec_order_list`/`shop_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillCreateOrderResponse` — 电子面单订单 id / 运单号 / 子母单号 / 风险与集运信息（`ewaybill_order_id`/`waybill_id`…）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_createorder
- 官方业务限制：`ewaybill_order_id` 数据内容要求 Uint64 以 String 传递；合并寄件须保证 `address_info.hash_code` 一致。

### 11. `PrecreateEwaybillOrderAsync` — 电子面单预取号（`ewaybill/biz/order/precreate`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/precreate`

- **请求体**：`ChannelsLogisticsEwaybillCreateOrderRequest` — 预取号请求（`delivery_id`/`ewaybill_acct_id`/`sender`/`receiver`/`ec_order_list`/`shop_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillPrecreateOrderResponse` — 全局唯一电子面单订单 id（`ewaybill_order_id`，用于取号接口）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_precreateorder

### 12. `GetEwaybillOrderAsync` — 查询面单详情（`ewaybill/biz/order/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/get`

- **请求体**：`ChannelsLogisticsEwaybillGetOrderRequest` — 查询请求（`ewaybill_order_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillGetOrderResponse` — 电子面单详情（`order_info`，含轨迹 `path_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_getorder

### 13. `CancelEwaybillOrderAsync` — 电子面单取消下单（`ewaybill/biz/order/cancel`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/cancel`

- **请求体**：`ChannelsLogisticsEwaybillCancelOrderRequest` — 取消请求（`ewaybill_order_id`/`delivery_id`/`waybill_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillCancelOrderResponse` — 取消结果与快递公司错误码（`delivery_error_msg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_cancelorder

### 14. `PrintEwaybillOrderAsync` — 打印成功通知（`ewaybill/biz/order/print`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/print`

- **请求体**：`ChannelsLogisticsEwaybillPrintRequest` — 打印通知请求（`ewaybill_order_id`/`delivery_id`/`waybill_id`/`re_print` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_printorder
- 官方业务限制：`re_print` 1 补打单成功 / 0 首次打单成功。

### 15. `BatchPrintEwaybillOrderAsync` — 批量打印通知（`ewaybill/biz/order/batchprint`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/batchprint`

- **请求体**：`ChannelsLogisticsEwaybillBatchPrintOrderRequest` — 批量打印通知请求（`req_list` 必填，单次不超过 50 个）。
- **返回**：`ChannelsLogisticsEwaybillBatchPrintOrderResponse` — 成功与失败的单号列表（`succ_ewaybill_order_id`/`fail_ewaybill_order_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_batchprintorder

### 16. `AddEwaybillSubOrderAsync` — 电子面单子件追加（`ewaybill/biz/order/addsuborder`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/addsuborder`

- **请求体**：`ChannelsLogisticsEwaybillAddSubOrderRequest` — 追加请求（`ewaybill_order_id`/`waybill_id`/`delivery_id`/`add_package_quantity`/`shop_id`/`ewaybill_acct_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillAddSubOrderResponse` — 子母单号列表 / 打印报文（`waybill_id_list`/`print_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_addsuborder
- 官方业务限制：`add_package_quantity` 1 ≤ value ≤ 300；支持子母单的快递公司：顺丰 / 德邦 / 京东 / 顺心捷达 / 韵达快运 / 安能物流。

### 17. `GetEwaybillDeliveryListAsync` — 查询开通的快递公司列表（`ewaybill/biz/delivery/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/delivery/get`

- **请求体**：`ChannelsLogisticsEwaybillGetDeliveryListRequest` — 查询请求（`delivery_id`/`status` 可选过滤）。
- **返回**：`ChannelsLogisticsEwaybillGetDeliveryListResponse` — 店铺 id 与快递公司列表（`shop_id`/`list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_getdeliverylist
- 官方业务限制：`status` 1 绑定审核中；2 取消绑定审核中；3 已绑定；4 已解除绑定；5 绑定未通过；6 取消绑定未通过。

### 18. `GetEwaybillPrintContentAsync` — 获取打印报文（`ewaybill/biz/print/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/print/get`

- **请求体**：`ChannelsLogisticsEwaybillGetPrintContentRequest` — 获取请求（`ewaybill_order_id`/`template_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillGetPrintContentResponse` — 电子面单打印报文（`print_info`，打单时传给打印机使用）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_get_print_content_supplier
- 官方业务限制：`sub_waybill_id` 不传则返回整单的打印报文。
- **路径视角**：本接口是商家侧路径；供货商侧同能力走 `ewaybill/biz/order/getprintinfo`（见 `GetSupplierEwaybillPrintContentAsync`），两侧报文同形、请求与响应 DTO 共用。

### 19. `GetSupplierEwaybillPrintContentAsync` — 获取打印报文（供货商视角，`ewaybill/biz/order/getprintinfo`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/order/getprintinfo`

- **请求体**：`ChannelsLogisticsEwaybillGetPrintContentRequest` — 获取请求（`ewaybill_order_id`/`template_id` 必填；`sub_waybill_id` 选填）。
- **返回**：`ChannelsLogisticsEwaybillGetPrintContentResponse` — 电子面单打印报文（`print_info`，打单时传给打印机使用）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/supplier/api/delivery/electronic/api_get_print_content.html（2026-10-17 逐页核验）
- **供货商路径差异**：官方本页明示「供货商的 HTTPS 调用路径与商家存在差异，请注意识别」—— / 商家侧为 `/channels/ec/logistics/ewaybill/biz/print/get`，供货商侧为 / `/channels/ec/logistics/ewaybill/biz/order/getprintinfo`；两支路径都是官方原文，不得互相对齐或合并。
- **报文同形 ⇒ 复用 DTO**：两侧请求参数表（`ewaybill_order_id`/`template_id`/`sub_waybill_id`） / 与返回参数表（`print_info`）逐字段一致 ⇒ 复用 `ChannelsLogisticsEwaybillGetPrintContentRequest` 与 / `ChannelsLogisticsEwaybillGetPrintContentResponse`，不分建（守卫 CL5 锁类型同一）。
- 官方业务限制：①**不支持云调用**；②支持第三方平台代供货商调用，权限集 id 为 `130`、`159`、`192`； / ③适用范围为「微信小店」与「小店供货商」两种账号类型（官方表格均标 ✔），其余账号类型不可调用； / ④`template_id` 如无需使用后台模板，可直接传 `template_type`（如 `single`）作默认模板（官方原文，值不做本地枚举拦截）。
- 官方错误码：`1000003`（订单不存在）/ `10025106`（模板 ID 不存在）。

### 20. `GetEwaybillTemplateConfigAsync` — 获取面单标准模板（`ewaybill/biz/template/config`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/template/config`
- **返回**：`ChannelsLogisticsEwaybillGetTemplateConfigResponse` — 所有快递公司模板信息汇总（`config`，键为快递公司 delivery_id）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_get_template_config
- 官方业务限制：本接口**无请求体**（请求体 Request Payload：无）。

### 21. `GetEwaybillTemplateByIdAsync` — 根据模板 ID 获取面单模板信息（`ewaybill/biz/template/getbyid`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/template/getbyid`

- **请求体**：`ChannelsLogisticsEwaybillGetTemplateByIdRequest` — 查询请求（`delivery_id`/`template_id` 必填）。
- **返回**：`ChannelsLogisticsEwaybillGetTemplateByIdResponse` — 模板信息（`template_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_gettemplatebyid

### 22. `GetEwaybillTemplateAsync` — 获取面单模板信息（`ewaybill/biz/template/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/template/get`

- **请求体**：`ChannelsLogisticsEwaybillGetTemplateRequest` — 查询请求（`delivery_id` 可选）。
- **返回**：`ChannelsLogisticsEwaybillGetTemplateResponse` — 所有快递公司模板信息汇总（`total_template`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_gettemplate

### 23. `CreateEwaybillTemplateAsync` — 新增面单模板（`ewaybill/biz/template/create`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/template/create`

- **请求体**：`ChannelsLogisticsEwaybillCreateTemplateRequest` — 新增请求（`delivery_id`/`info` 必填）。
- **返回**：`ChannelsLogisticsEwaybillCreateTemplateResponse` — 快递公司编码与模板编码（`delivery_id`/`template_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_createtemplate
- 官方业务限制：`options` 总数必须为 8 个且 `option_id` 不得重复；`template_name` 同一快递公司下不可重复。

### 24. `UpdateEwaybillTemplateAsync` — 更新面单模板（`ewaybill/biz/template/update`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/template/update`

- **请求体**：`ChannelsLogisticsEwaybillUpdateTemplateRequest` — 更新请求（`delivery_id`/`info` 必填，原地覆盖写入须带全字段）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_updatetemplate

### 25. `DeleteEwaybillTemplateAsync` — 删除面单模板（`ewaybill/biz/template/delete`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/template/delete`

- **请求体**：`ChannelsLogisticsEwaybillDeleteTemplateRequest` — 删除请求（`delivery_id`/`template_id` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_deltemplate

### 26. `GetEwaybillAcctAsync` — 查询开通的电子面单网点 / 账号信息（`ewaybill/biz/account/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/logistics/ewaybill/biz/account/get`

- **请求体**：`ChannelsLogisticsEwaybillGetAcctRequest` — 查询请求（`need_balance`/`limit` 必填）。
- **返回**：`ChannelsLogisticsEwaybillGetAcctResponse` — 总条数与账号列表（`total_num`/`account_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_getacct
- 官方业务限制：`need_balance=true` 时 `limit` 建议小于 20，否则 limit 过大导致接口超时无返回。

### 27. `SendDeliveryAsync` — 订单发货（`order/delivery/send`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/order/delivery/send`

- **请求体**：`ChannelsLogisticsSendDeliveryRequest` — 发货请求（`order_id`/`delivery_list` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_senddelivery
- 官方业务限制：`deliver_type` 1 自寄快递发货 / 3 虚拟商品无需物流发货（仅 deliver_method=1 的订单可用）；`delivery_id` 非主流可填 OTHER。

### 28. `GetDeliveryCompanyListAsync` — 获取快递公司列表（`order/deliverycompanylist/new/get`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/order/deliverycompanylist/new/get`

- **请求体**：`ChannelsLogisticsGetDeliveryCompanyListRequest` — 查询请求（`ewaybill_only` 必填）。
- **返回**：`ChannelsLogisticsGetDeliveryCompanyListResponse` — 快递公司列表（`company_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getdeliverycompanylistnew
- **旧版路由不声明（守卫 CL6）**：官方曾另有 `order/deliverycompanylist/get`（第三方 SDK 清单仍转述该路径）， / 其文档页现已重定向到「订单管理」目录页、正文脱树 ⇒ 按「脱树即非契约」判据不予声明，本端点是唯一现行面； / 路径中的 `new/` 段属官方原文，禁止「对齐规范」删除。

### 29. `DeliveryCompensationAsync` — 订单补发货（`order/delivery/compensation`）。

- **声明接口**：[IChannelsLogisticsService](./IChannelsLogisticsService.cs)
- **官方契约**：`POST /channels/ec/order/delivery/compensation`

- **请求体**：`ChannelsLogisticsDeliveryCompensationRequest` — 补发请求（`order_id`/`delivery_list`/`reason` 必填）。
- **返回**：`ChannelsResponse` — 成功 / 失败（`errcode`/`errmsg`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_deliverycompensation
- 官方业务限制：一个订单最多可补发 10 次；`reason` 1 商品漏发 / 2 商品拆分包裹 / 3 商品坏损 / 4 赠品。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/Logistics`](../../../Mud.Wechat.Channels.DataModels/Logistics/) 内共 **89** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **89** 个，`SerializerClassName` = `Logistics`。

AOT 源生成上下文：[`LogisticsJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/LogisticsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsLogisticsAddAddressResponse` | 应答 | [Logistics/Address/ChannelsLogisticsAddAddressResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsAddAddressResponse.cs) | `address_id` |
| `ChannelsLogisticsAddAddressRequest` | 请求体 | [Logistics/Address/ChannelsLogisticsAddAddressRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsAddAddressRequest.cs) | `address_detail` |
| `ChannelsLogisticsGetAddressResponse` | 应答 | [Logistics/Address/ChannelsLogisticsGetAddressResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsGetAddressResponse.cs) | `address_detail` |
| `ChannelsLogisticsAddressIdRequest` | 请求体 | [Logistics/Address/ChannelsLogisticsAddressIdRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsAddressIdRequest.cs) | `address_id` |
| `ChannelsLogisticsGetAddressListResponse` | 应答 | [Logistics/Address/ChannelsLogisticsGetAddressListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsGetAddressListResponse.cs) | `address_id_list` |
| `ChannelsLogisticsGetAddressListRequest` | 请求体 | [Logistics/Address/ChannelsLogisticsGetAddressListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsGetAddressListRequest.cs) | `offset`, `limit` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels.DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsLogisticsUpdateAddressRequest` | 请求体 | [Logistics/Address/ChannelsLogisticsUpdateAddressRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Address/ChannelsLogisticsUpdateAddressRequest.cs) | `address_detail` |
| `ChannelsLogisticsAddFreightTemplateResponse` | 应答 | [Logistics/Freight/ChannelsLogisticsAddFreightTemplateResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsAddFreightTemplateResponse.cs) | `template_id` |
| `ChannelsLogisticsAddFreightTemplateRequest` | 请求体 | [Logistics/Freight/ChannelsLogisticsAddFreightTemplateRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsAddFreightTemplateRequest.cs) | `freight_template` |
| `ChannelsLogisticsGetFreightTemplateDetailResponse` | 应答 | [Logistics/Freight/ChannelsLogisticsGetFreightTemplateDetailResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsGetFreightTemplateDetailResponse.cs) | `freight_template` |
| `ChannelsLogisticsGetFreightTemplateDetailRequest` | 请求体 | [Logistics/Freight/ChannelsLogisticsGetFreightTemplateDetailRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsGetFreightTemplateDetailRequest.cs) | `template_id` |
| `ChannelsLogisticsGetFreightTemplateListResponse` | 应答 | [Logistics/Freight/ChannelsLogisticsGetFreightTemplateListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsGetFreightTemplateListResponse.cs) | `template_id_list` |
| `ChannelsLogisticsGetFreightTemplateListRequest` | 请求体 | [Logistics/Freight/ChannelsLogisticsGetFreightTemplateListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsGetFreightTemplateListRequest.cs) | `offset`, `limit` |
| `ChannelsLogisticsUpdateFreightTemplateRequest` | 请求体 | [Logistics/Freight/ChannelsLogisticsUpdateFreightTemplateRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Freight/ChannelsLogisticsUpdateFreightTemplateRequest.cs) | `freight_template` |
| `ChannelsLogisticsEwaybillCreateOrderResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateOrderResponse.cs) | `ewaybill_order_id`, `waybill_id`, `delivery_error_msg`, `print_info`, `waybill_id_list`, `order_risk_info`, `consolidation_waybill_info` |
| `ChannelsLogisticsEwaybillCreateOrderRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateOrderRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateOrderRequest.cs) | `ewaybill_order_id`, `delivery_id`, `site_code`, `ewaybill_acct_id`, `sender`, `receiver`, `ec_order_list`, `remark`, `shop_id`, `return_address`, `template_id`, `order_type` … |
| `ChannelsLogisticsEwaybillPrecreateOrderResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillPrecreateOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillPrecreateOrderResponse.cs) | `ewaybill_order_id` |
| `ChannelsLogisticsEwaybillGetOrderResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetOrderResponse.cs) | `order_info` |
| `ChannelsLogisticsEwaybillGetOrderRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetOrderRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetOrderRequest.cs) | `ewaybill_order_id` |
| `ChannelsLogisticsEwaybillCancelOrderResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillCancelOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillCancelOrderResponse.cs) | `delivery_error_msg` |
| `ChannelsLogisticsEwaybillCancelOrderRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillCancelOrderRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillCancelOrderRequest.cs) | `ewaybill_order_id`, `delivery_id`, `waybill_id` |
| `ChannelsLogisticsEwaybillPrintRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillPrintRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillPrintRequest.cs) | `ewaybill_order_id`, `delivery_id`, `waybill_id`, `re_print` |
| `ChannelsLogisticsEwaybillBatchPrintOrderResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillBatchPrintOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillBatchPrintOrderResponse.cs) | `succ_ewaybill_order_id`, `fail_ewaybill_order_id` |
| `ChannelsLogisticsEwaybillBatchPrintOrderRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillBatchPrintOrderRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillBatchPrintOrderRequest.cs) | `req_list` |
| `ChannelsLogisticsEwaybillAddSubOrderResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillAddSubOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillAddSubOrderResponse.cs) | `delivery_error_msg`, `waybill_id`, `waybill_id_list`, `print_info`, `ewaybill_order_id` |
| `ChannelsLogisticsEwaybillAddSubOrderRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillAddSubOrderRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillAddSubOrderRequest.cs) | `ewaybill_order_id`, `waybill_id`, `delivery_id`, `add_package_quantity`, `template_id`, `shop_id`, `ewaybill_acct_id`, `subpackage_list`, `sub_order_ext_info` |
| `ChannelsLogisticsEwaybillGetDeliveryListResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetDeliveryListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetDeliveryListResponse.cs) | `shop_id`, `list` |
| `ChannelsLogisticsEwaybillGetDeliveryListRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetDeliveryListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetDeliveryListRequest.cs) | `delivery_id`, `status` |
| `ChannelsLogisticsEwaybillGetPrintContentResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetPrintContentResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetPrintContentResponse.cs) | `print_info` |
| `ChannelsLogisticsEwaybillGetPrintContentRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetPrintContentRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetPrintContentRequest.cs) | `ewaybill_order_id`, `template_id`, `sub_waybill_id` |
| `ChannelsLogisticsEwaybillGetTemplateConfigResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateConfigResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateConfigResponse.cs) | `config` |
| `ChannelsLogisticsEwaybillGetTemplateByIdResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateByIdResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateByIdResponse.cs) | `template_info` |
| `ChannelsLogisticsEwaybillGetTemplateByIdRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateByIdRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateByIdRequest.cs) | `delivery_id`, `template_id` |
| `ChannelsLogisticsEwaybillGetTemplateResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateResponse.cs) | `total_template` |
| `ChannelsLogisticsEwaybillGetTemplateRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetTemplateRequest.cs) | `delivery_id` |
| `ChannelsLogisticsEwaybillCreateTemplateResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateTemplateResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateTemplateResponse.cs) | `delivery_id`, `template_id` |
| `ChannelsLogisticsEwaybillCreateTemplateRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateTemplateRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillCreateTemplateRequest.cs) | `delivery_id`, `info` |
| `ChannelsLogisticsEwaybillUpdateTemplateRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillUpdateTemplateRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillUpdateTemplateRequest.cs) | `delivery_id`, `info` |
| `ChannelsLogisticsEwaybillDeleteTemplateRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillDeleteTemplateRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillDeleteTemplateRequest.cs) | `delivery_id`, `template_id` |
| `ChannelsLogisticsEwaybillGetAcctResponse` | 应答 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetAcctResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetAcctResponse.cs) | `total_num`, `account_list` |
| `ChannelsLogisticsEwaybillGetAcctRequest` | 请求体 | [Logistics/Ewaybill/ChannelsLogisticsEwaybillGetAcctRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Ewaybill/ChannelsLogisticsEwaybillGetAcctRequest.cs) | `delivery_id`, `need_balance`, `status`, `offset`, `limit`, `acct_id` |
| `ChannelsLogisticsSendDeliveryRequest` | 请求体 | [Logistics/Delivery/ChannelsLogisticsSendDeliveryRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Delivery/ChannelsLogisticsSendDeliveryRequest.cs) | `order_id`, `delivery_list` |
| `ChannelsLogisticsGetDeliveryCompanyListResponse` | 应答 | [Logistics/Delivery/ChannelsLogisticsGetDeliveryCompanyListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Delivery/ChannelsLogisticsGetDeliveryCompanyListResponse.cs) | `company_list` |
| `ChannelsLogisticsGetDeliveryCompanyListRequest` | 请求体 | [Logistics/Delivery/ChannelsLogisticsGetDeliveryCompanyListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Delivery/ChannelsLogisticsGetDeliveryCompanyListRequest.cs) | `ewaybill_only` |
| `ChannelsLogisticsDeliveryCompensationRequest` | 请求体 | [Logistics/Delivery/ChannelsLogisticsDeliveryCompensationRequest.cs](../../../Mud.Wechat.Channels.DataModels/Logistics/Delivery/ChannelsLogisticsDeliveryCompensationRequest.cs) | `order_id`, `delivery_list`, `reason` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Logistics;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddLogisticsApi());

var service = app.Services.GetRequiredService<IChannelsLogisticsService>();
var ct = CancellationToken.None;
var response = await service.GetEwaybillTemplateConfigAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/logistics/ewaybill/biz/template/config");
// 占位值仅用于编译示意；业务值须按 IChannelsLogisticsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetEwaybillTemplateConfigAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsLogisticsService](./IChannelsLogisticsService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsLogisticsContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLogisticsContractGuards.cs) |
| 守卫中的代表性测试方法 | `LogisticsEndpoints_ShouldMatchOfficialRoutes`、`LogisticsEndpoints_ShouldAllBePost`、`LogisticsInterface_ShouldBindChannelsToken_AndRegisterUnderLogisticsGroup`、`LogisticsDataModels_ShouldBeRegisteredInJsonContext`、`LogisticsDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`RemovedDeliveryCompanyListLegacyRoute_ShouldHaveZeroDeclaration`、`RemovedLogisticsVirtualNumberRoutes_ShouldHaveZeroDeclaration` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/Logistics`](../../../Mud.Wechat.Channels.DataModels/Logistics/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsLogisticsContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
