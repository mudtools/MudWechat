# Express 域 · 物流助手 · 商家寄件

微信小程序「物流助手 · 商家寄件」域 SDK（12 端点）—— **带令牌**通道。 接口：[IWxaExpressBillService](./IWxaExpressBillService.cs)、[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)、[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)、[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)、[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)、[IWxaExpressLocalService](./IWxaExpressLocalService.cs)；注册组 `Express`；模块枚举 `MiniProgramModule.Express`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/Express`](../../../Mud.Wechat.MiniProgram/Express/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaExpressBusinessService = 12；IWxaExpressDeliveryService = 29；IWxaExpressInsuranceFreightService = 11；IWxaExpressIntracityService = 16；IWxaExpressLocalService = 15；按 HTTP 特性实计 **82** 支，另有手工通道 **1** 支，本域合计 **83** 支。

> **计数漂移（留档）**：`IWxaExpressDeliveryService` 注释自述 29 支、实计 28 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaExpressBusinessService` | `AddOrderAsync` | POST | `/cgi-bin/express/business/order/add` | `AccessToken`（Query），Query `access_token` | 商家寄件下单（`POST /cgi-bin/express/business/order/add`）。<br>官方文档：https://developers.weixin.qq.com/… |
| 2 | `IWxaExpressBusinessService` | `GetOrderAsync` | POST | `/cgi-bin/express/business/order/get` | `AccessToken`（Query），Query `access_token` | 查询运单信息（`POST /cgi-bin/express/business/order/get`）。<br>官方文档：https://developers.weixin.qq.com/… |
| 3 | `IWxaExpressBusinessService` | `BatchGetOrderAsync` | POST | `/cgi-bin/express/business/order/batchget` | `AccessToken`（Query），Query `access_token` | 批量查询运单信息（`POST /cgi-bin/express/business/order/batchget`）。<br>官方文档：https://developers.weixin.… |
| 4 | `IWxaExpressBusinessService` | `CancelOrderAsync` | POST | `/cgi-bin/express/business/order/cancel` | `AccessToken`（Query），Query `access_token` | 取消运单（`POST /cgi-bin/express/business/order/cancel`）。<br>官方文档：https://developers.weixin.qq.com… |
| 5 | `IWxaExpressBusinessService` | `GetPathAsync` | POST | `/cgi-bin/express/business/path/get` | `AccessToken`（Query），Query `access_token` | 查询物流轨迹（`POST /cgi-bin/express/business/path/get`）。<br>官方文档：https://developers.weixin.qq.com/m… |
| 6 | `IWxaExpressBusinessService` | `GetAllDeliveryAsync` | GET | `/cgi-bin/express/business/delivery/getall` | `AccessToken`（Query），Query `access_token` | 获取快递公司列表（`GET /cgi-bin/express/business/delivery/getall`）。<br>官方文档：https://developers.weixin.… |
| 7 | `IWxaExpressBusinessService` | `GetAllAccountAsync` | GET | `/cgi-bin/express/business/account/getall` | `AccessToken`（Query），Query `access_token` | 获取快递账号列表（`GET /cgi-bin/express/business/account/getall`）。<br>官方文档：https://developers.weixin.q… |
| 8 | `IWxaExpressBusinessService` | `BindAccountAsync` | POST | `/cgi-bin/express/business/account/bind` | `AccessToken`（Query），Query `access_token` | 绑定 / 解绑快递账号（`POST /cgi-bin/express/business/account/bind`）。<br>官方文档：https://developers.weixin… |
| 9 | `IWxaExpressBusinessService` | `GetAllPrinterAsync` | GET | `/cgi-bin/express/business/printer/getall` | `AccessToken`（Query），Query `access_token` | 获取打印员列表（`GET /cgi-bin/express/business/printer/getall`）。<br>官方文档：https://developers.weixin.qq… |
| 10 | `IWxaExpressBusinessService` | `UpdatePrinterAsync` | POST | `/cgi-bin/express/business/printer/update` | `AccessToken`（Query），Query `access_token` | 绑定 / 解绑打印员（`POST /cgi-bin/express/business/printer/update`）。<br>官方文档：https://developers.weixi… |
| 11 | `IWxaExpressBusinessService` | `GetQuotaAsync` | POST | `/cgi-bin/express/business/quota/get` | `AccessToken`（Query），Query `access_token` | 查看电子面单余额（`POST /cgi-bin/express/business/quota/get`）。<br>官方文档：https://developers.weixin.qq.co… |
| 12 | `IWxaExpressBusinessService` | `TestUpdateOrderAsync` | POST | `/cgi-bin/express/business/test_update_order` | `AccessToken`（Query），Query `access_token` | 模拟快递公司更新订单状态（`POST /cgi-bin/express/business/test_update_order`，**仅测试环境**）。<br>官方文档：https://d… |
| 13 | `IWxaExpressDeliveryService` | `GetContactAsync` | POST | `/cgi-bin/express/delivery/contact/get` | `AccessToken`（Query），Query `access_token` | 获取面单联系人信息（`POST /cgi-bin/express/delivery/contact/get`）。<br>官方文档：https://developers.weixin.qq… |
| 14 | `IWxaExpressDeliveryService` | `PreviewTemplateAsync` | POST | `/cgi-bin/express/delivery/template/preview` | `AccessToken`（Query），Query `access_token` | 预览面单模板（`POST /cgi-bin/express/delivery/template/preview`）。<br>官方文档：https://developers.weixin.… |
| 15 | `IWxaExpressDeliveryService` | `UpdateBusinessAsync` | POST | `/cgi-bin/express/delivery/service/business/update` | `AccessToken`（Query），Query `access_token` | 服务商更新订单审核结果（`POST /cgi-bin/express/delivery/service/business/update`）。<br>官方文档：https://develo… |
| 16 | `IWxaExpressDeliveryService` | `UpdatePathAsync` | POST | `/cgi-bin/express/delivery/path/update` | `AccessToken`（Query），Query `access_token` | 服务商更新运单轨迹（`POST /cgi-bin/express/delivery/path/update`）。<br>官方文档：https://developers.weixin.qq… |
| 17 | `IWxaExpressDeliveryService` | `AddNoWorryReturnAsync` | POST | `/cgi-bin/express/delivery/no_worry_return/add` | `AccessToken`（Query），Query `access_token` | 创建退货 ID（无忧退货运费险，`POST /cgi-bin/express/delivery/no_worry_return/add`）。<br>官方文档：https://develo… |
| 18 | `IWxaExpressDeliveryService` | `GetNoWorryReturnAsync` | POST | `/cgi-bin/express/delivery/no_worry_return/get` | `AccessToken`（Query），Query `access_token` | 查询退货 ID 状态（无忧退货运费险，`POST /cgi-bin/express/delivery/no_worry_return/get`）。<br>官方文档：https://dev… |
| 19 | `IWxaExpressDeliveryService` | `UnbindNoWorryReturnAsync` | POST | `/cgi-bin/express/delivery/no_worry_return/unbind` | `AccessToken`（Query），Query `access_token` | 解绑退货 ID（无忧退货运费险，`POST /cgi-bin/express/delivery/no_worry_return/unbind`）。<br>官方文档：https://dev… |
| 20 | `IWxaExpressDeliveryService` | `FollowWaybillAsync` | POST | `/cgi-bin/express/delivery/open_msg/follow_waybill` | `AccessToken`（Query），Query `access_token` | 用户绑定运单（物流消息，`POST /cgi-bin/express/delivery/open_msg/follow_waybill`）。<br>官方文档：https://develo… |
| 21 | `IWxaExpressDeliveryService` | `QueryFollowTraceAsync` | POST | `/cgi-bin/express/delivery/open_msg/query_follow_trace` | `AccessToken`（Query），Query `access_token` | 查询运单轨迹（物流消息，`POST /cgi-bin/express/delivery/open_msg/query_follow_trace`）。<br>官方文档：https://de… |
| 22 | `IWxaExpressDeliveryService` | `UpdateFollowWaybillGoodsAsync` | POST | `/cgi-bin/express/delivery/open_msg/update_follow_waybill_goods` | `AccessToken`（Query），Query `access_token` | 更新运单商品信息（物流消息，`POST /cgi-bin/express/delivery/open_msg/update_follow_waybill_goods`）。<br>官方文档… |
| 23 | `IWxaExpressDeliveryService` | `OpenOpenMessageAsync` | POST | `/cgi-bin/express/delivery/open_msg/open_openmsg` | `AccessToken`（Query），Query `access_token` | 开通物流消息（`POST /cgi-bin/express/delivery/open_msg/open_openmsg`）。<br>官方文档：https://developers.we… |
| 24 | `IWxaExpressDeliveryService` | `OpenQueryPluginAsync` | POST | `/cgi-bin/express/delivery/open_msg/open_query_plugin` | `AccessToken`（Query），Query `access_token` | 开通物流查询插件（`POST /cgi-bin/express/delivery/open_msg/open_query_plugin`）。<br>官方文档：https://develo… |
| 25 | `IWxaExpressDeliveryService` | `TraceWaybillAsync` | POST | `/cgi-bin/express/delivery/open_msg/trace_waybill` | `AccessToken`（Query），Query `access_token` | 查询运单信息（物流查询插件，`POST /cgi-bin/express/delivery/open_msg/trace_waybill`）。<br>官方文档：https://devel… |
| 26 | `IWxaExpressDeliveryService` | `QueryTraceAsync` | POST | `/cgi-bin/express/delivery/open_msg/query_trace` | `AccessToken`（Query），Query `access_token` | 查询运单轨迹（物流查询插件，`POST /cgi-bin/express/delivery/open_msg/query_trace`）。<br>官方文档：https://develop… |
| 27 | `IWxaExpressDeliveryService` | `UpdateWaybillGoodsAsync` | POST | `/cgi-bin/express/delivery/open_msg/update_waybill_goods` | `AccessToken`（Query），Query `access_token` | 更新运单商品信息（物流查询插件，`POST /cgi-bin/express/delivery/open_msg/update_waybill_goods`）。<br>官方文档：http… |
| 28 | `IWxaExpressDeliveryService` | `GetDeliveryListAsync` | POST | `/cgi-bin/express/delivery/open_msg/get_delivery_list` | `AccessToken`（Query），Query `access_token` | 获取快递公司列表（物流消息 / 查询插件共用，`POST /cgi-bin/express/delivery/open_msg/get_delivery_list`）。<br>官方文档：… |
| 29 | `IWxaExpressDeliveryService` | `OpenReturnAsync` | POST | `/cgi-bin/express/delivery/return/open_return` | `AccessToken`（Query），Query `access_token` | 开通退货组件（`POST /cgi-bin/express/delivery/return/open_return`）。<br>官方文档：https://developers.weixi… |
| 30 | `IWxaExpressDeliveryService` | `AddReturnAsync` | POST | `/cgi-bin/express/delivery/return/add` | `AccessToken`（Query），Query `access_token` | 创建退货 ID（退货组件，`POST /cgi-bin/express/delivery/return/add`）。<br>官方文档：https://developers.weixin.… |
| 31 | `IWxaExpressDeliveryService` | `GetReturnAsync` | POST | `/cgi-bin/express/delivery/return/get` | `AccessToken`（Query），Query `access_token` | 查询退货 ID 状态（退货组件，`POST /cgi-bin/express/delivery/return/get`）。<br>官方文档：https://developers.weix… |
| 32 | `IWxaExpressDeliveryService` | `UnbindReturnAsync` | POST | `/cgi-bin/express/delivery/return/unbind` | `AccessToken`（Query），Query `access_token` | 解绑退货 ID（退货组件，`POST /cgi-bin/express/delivery/return/unbind`）。<br>官方文档：https://developers.weix… |
| 33 | `IWxaExpressDeliveryService` | `UpdateSingleWaybillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/update` | `AccessToken`（Query），Query `access_token` | 运力更新订单状态（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/update`）。<br>官方文档：https://develo… |
| 34 | `IWxaExpressDeliveryService` | `UpdateSingleWaybillFeeAsync` | POST | `/cgi-bin/express/delivery/single_waybill/fee` | `AccessToken`（Query），Query `access_token` | 运力更新订单费用（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/fee`）。<br>官方文档：https://developer… |
| 35 | `IWxaExpressDeliveryService` | `CancelSingleWaybillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/cancel_order` | `AccessToken`（Query），Query `access_token` | 运力取消订单（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/cancel_order`）。<br>官方文档：https://de… |
| 36 | `IWxaExpressDeliveryService` | `RefundSingleWaybillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/refund_order` | `AccessToken`（Query），Query `access_token` | 运力对订单退款（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/refund_order`）。<br>官方文档：https://d… |
| 37 | `IWxaExpressDeliveryService` | `EvaluateSingleWaybillFeedbackAsync` | POST | `/cgi-bin/express/delivery/single_waybill/evaluate_feedback` | `AccessToken`（Query），Query `access_token` | 运力回传用户评价反馈（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/evaluate_feedback`）。<br>官方文档：h… |
| 38 | `IWxaExpressDeliveryService` | `UpdateComplaintResultAsync` | POST | `/cgi-bin/express/delivery/scatter/update_complaint_result` | `AccessToken`（Query），Query `access_token` | 运力返回用户投诉处理结果（散单寄件，`POST /cgi-bin/express/delivery/scatter/update_complaint_result`）。<br>官方文档：… |
| 39 | `IWxaExpressDeliveryService` | `NotifyPathAsync` | POST | `/cgi-bin/express/delivery/pathnotify` | `AccessToken`（Query），Query `access_token` | 推送已绑定物流轨迹信息（`POST /cgi-bin/express/delivery/pathnotify`）。<br>官方文档：https://developers.weixin.q… |
| 40 | `IWxaExpressDeliveryService` | `QueryUserBindingAsync` | POST | `/cgi-bin/express/delivery/userquery` | `AccessToken`（Query），Query `access_token` | 用户手机状态查询（`POST /cgi-bin/express/delivery/userquery`）。<br>官方文档：https://developers.weixin.qq.co… |
| 41 | `IWxaExpressInsuranceFreightService` | `OpenInsuranceFreightAsync` | GET | `/wxa/business/insurance_freight/open` | `AccessToken`（Query），Query `access_token` | 开通无忧退货（`GET /wxa/business/insurance_freight/open`）。<br>官方文档：https://developers.weixin.qq.com/… |
| 42 | `IWxaExpressInsuranceFreightService` | `QueryInsuranceFreightOpenAsync` | GET | `/wxa/business/insurance_freight/query_open` | `AccessToken`（Query），Query `access_token` | 查询无忧退货开通状态（`GET /wxa/business/insurance_freight/query_open`）。<br>官方文档：https://developers.weix… |
| 43 | `IWxaExpressInsuranceFreightService` | `CreateInsuranceOrderAsync` | POST | `/wxa/business/insurance_freight/createorder` | `AccessToken`（Query），Query `access_token` | 发货时投保（`POST /wxa/business/insurance_freight/createorder`）。<br>官方文档：https://developers.weixin.… |
| 44 | `IWxaExpressInsuranceFreightService` | `ClaimAsync` | POST | `/wxa/business/insurance_freight/claim` | `AccessToken`（Query），Query `access_token` | 无忧退理赔（`POST /wxa/business/insurance_freight/claim`）。<br>官方文档：https://developers.weixin.qq.com… |
| 45 | `IWxaExpressInsuranceFreightService` | `CreateChargeIdAsync` | POST | `/wxa/business/insurance_freight/createchargeid` | `AccessToken`（Query），Query `access_token` | 申请充值订单号（`POST /wxa/business/insurance_freight/createchargeid`，支持自定义金额）。<br>官方文档：https://devel… |
| 46 | `IWxaExpressInsuranceFreightService` | `ApplyPayAsync` | POST | `/wxa/business/insurance_freight/applypay` | `AccessToken`（Query），Query `access_token` | 申请支付（`POST /wxa/business/insurance_freight/applypay`）。<br>官方文档：https://developers.weixin.qq.c… |
| 47 | `IWxaExpressInsuranceFreightService` | `GetPayOrderListAsync` | POST | `/wxa/business/insurance_freight/getpayorderlist` | `AccessToken`（Query），Query `access_token` | 拉取充值订单信息（`POST /wxa/business/insurance_freight/getpayorderlist`）。<br>官方文档：https://developers.… |
| 48 | `IWxaExpressInsuranceFreightService` | `RefundPremiumAsync` | POST | `/wxa/business/insurance_freight/refund` | `AccessToken`（Query），Query `access_token` | 充值保费退款（`POST /wxa/business/insurance_freight/refund`）。<br>官方文档：https://developers.weixin.qq.c… |
| 49 | `IWxaExpressInsuranceFreightService` | `GetSummaryAsync` | POST | `/wxa/business/insurance_freight/getsummary` | `AccessToken`（Query），Query `access_token` | 拉取理赔摘要（`POST /wxa/business/insurance_freight/getsummary`）。<br>官方文档：https://developers.weixin.… |
| 50 | `IWxaExpressInsuranceFreightService` | `GetOrderListAsync` | POST | `/wxa/business/insurance_freight/getorderlist` | `AccessToken`（Query），Query `access_token` | 拉取保单信息（`POST /wxa/business/insurance_freight/getorderlist`）。<br>官方文档：https://developers.weixi… |
| 51 | `IWxaExpressInsuranceFreightService` | `UpdateNotifyFundsAsync` | POST | `/wxa/business/insurance_freight/update_notify_funds` | `AccessToken`（Query），Query `access_token` | 设置保费告警余额（`POST /wxa/business/insurance_freight/update_notify_funds`）。<br>官方文档：https://develop… |
| 52 | `IWxaExpressIntracityService` | `ApplyIntracityAsync` | POST | `/cgi-bin/express/intracity/apply` | `AccessToken`（Query），Query `access_token` | 申请开通同城配送（`POST /cgi-bin/express/intracity/apply`）。<br>官方文档：https://developers.weixin.qq.com/m… |
| 53 | `IWxaExpressIntracityService` | `CreateStoreAsync` | POST | `/cgi-bin/express/intracity/createstore` | `AccessToken`（Query），Query `access_token` | 创建门店（`POST /cgi-bin/express/intracity/createstore`）。<br>官方文档：https://developers.weixin.qq.com… |
| 54 | `IWxaExpressIntracityService` | `QueryStoreAsync` | POST | `/cgi-bin/express/intracity/querystore` | `AccessToken`（Query），Query `access_token` | 查询门店（`POST /cgi-bin/express/intracity/querystore`）。<br>官方文档：https://developers.weixin.qq.com/… |
| 55 | `IWxaExpressIntracityService` | `UpdateStoreAsync` | POST | `/cgi-bin/express/intracity/updatestore` | `AccessToken`（Query），Query `access_token` | 更新门店（`POST /cgi-bin/express/intracity/updatestore`）。<br>官方文档：https://developers.weixin.qq.com… |
| 56 | `IWxaExpressIntracityService` | `StoreChargeAsync` | POST | `/cgi-bin/express/intracity/storecharge` | `AccessToken`（Query），Query `access_token` | 门店运费充值（`POST /cgi-bin/express/intracity/storecharge`）。<br>官方文档：https://developers.weixin.qq.c… |
| 57 | `IWxaExpressIntracityService` | `StoreRefundAsync` | POST | `/cgi-bin/express/intracity/storerefund` | `AccessToken`（Query），Query `access_token` | 门店运费退款（`POST /cgi-bin/express/intracity/storerefund`）。<br>官方文档：https://developers.weixin.qq.c… |
| 58 | `IWxaExpressIntracityService` | `QueryFlowAsync` | POST | `/cgi-bin/express/intracity/queryflow` | `AccessToken`（Query），Query `access_token` | 查询门店运费流水（`POST /cgi-bin/express/intracity/queryflow`）。<br>官方文档：https://developers.weixin.qq.c… |
| 59 | `IWxaExpressIntracityService` | `BalanceQueryAsync` | POST | `/cgi-bin/express/intracity/balancequery` | `AccessToken`（Query），Query `access_token` | 查询门店余额（`POST /cgi-bin/express/intracity/balancequery`）。<br>官方文档：https://developers.weixin.qq.… |
| 60 | `IWxaExpressIntracityService` | `PreAddOrderAsync` | POST | `/cgi-bin/express/intracity/preaddorder` | `AccessToken`（Query），Query `access_token` | 同城配送询价（`POST /cgi-bin/express/intracity/preaddorder`）。<br>官方文档：https://developers.weixin.qq.c… |
| 61 | `IWxaExpressIntracityService` | `AddIntracityOrderAsync` | POST | `/cgi-bin/express/intracity/addorder` | `AccessToken`（Query），Query `access_token` | 同城配送下单（`POST /cgi-bin/express/intracity/addorder`）。<br>官方文档：https://developers.weixin.qq.com/… |
| 62 | `IWxaExpressIntracityService` | `QueryIntracityOrderAsync` | POST | `/cgi-bin/express/intracity/queryorder` | `AccessToken`（Query），Query `access_token` | 查询配送单（`POST /cgi-bin/express/intracity/queryorder`）。<br>官方文档：https://developers.weixin.qq.com… |
| 63 | `IWxaExpressIntracityService` | `CancelIntracityOrderAsync` | POST | `/cgi-bin/express/intracity/cancelorder` | `AccessToken`（Query），Query `access_token` | 取消配送单（`POST /cgi-bin/express/intracity/cancelorder`）。<br>官方文档：https://developers.weixin.qq.co… |
| 64 | `IWxaExpressIntracityService` | `MockNotifyAsync` | POST | `/cgi-bin/express/intracity/mocknotify` | `AccessToken`（Query），Query `access_token` | 模拟配送状态变更回调（`POST /cgi-bin/express/intracity/mocknotify`，**仅测试环境**）。<br>官方文档：https://developer… |
| 65 | `IWxaExpressIntracityService` | `GetCityAsync` | POST | `/cgi-bin/express/intracity/getcity` | `AccessToken`（Query），Query `access_token` | 查询支持同城配送的城市（`POST /cgi-bin/express/intracity/getcity`）。<br>官方文档：https://developers.weixin.qq.… |
| 66 | `IWxaExpressIntracityService` | `GetPayModeAsync` | POST | `/cgi-bin/express/intracity/getpaymode` | `AccessToken`（Query），Query `access_token` | 查询扣费主体（`POST /cgi-bin/express/intracity/getpaymode`）。<br>官方文档：https://developers.weixin.qq.co… |
| 67 | `IWxaExpressIntracityService` | `SetPayModeAsync` | POST | `/cgi-bin/express/intracity/setpaymode` | `AccessToken`（Query），Query `access_token` | 设置扣费主体（`POST /cgi-bin/express/intracity/setpaymode`）。<br>官方文档：https://developers.weixin.qq.co… |
| 68 | `IWxaExpressLocalService` | `GetAllDeliveryAsync` | POST | `/cgi-bin/express/local/business/delivery/getall` | `AccessToken`（Query），Query `access_token` | 获取已支持的配送公司列表（`POST /cgi-bin/express/local/business/delivery/getall`）。<br>官方文档：https://develop… |
| 69 | `IWxaExpressLocalService` | `OpenDeliveryAsync` | POST | `/cgi-bin/express/local/business/open` | `AccessToken`（Query），Query `access_token` | 申请开通即时配送（`POST /cgi-bin/express/local/business/open`）。<br>官方文档：https://developers.weixin.qq.c… |
| 70 | `IWxaExpressLocalService` | `BindAccountAsync` | POST | `/cgi-bin/express/local/business/shop/add` | `AccessToken`（Query），Query `access_token` | 发起绑定配送公司账号请求（`POST /cgi-bin/express/local/business/shop/add`）。<br>官方文档：https://developers.wei… |
| 71 | `IWxaExpressLocalService` | `GetBindAccountAsync` | POST | `/cgi-bin/express/local/business/shop/get` | `AccessToken`（Query），Query `access_token` | 拉取已绑定账号（`POST /cgi-bin/express/local/business/shop/get`）。<br>官方文档：https://developers.weixin.q… |
| 72 | `IWxaExpressLocalService` | `PreAddOrderAsync` | POST | `/cgi-bin/express/local/business/order/pre_add` | `AccessToken`（Query），Query `access_token` | 预下配送单（`POST /cgi-bin/express/local/business/order/pre_add`）。<br>官方文档：https://developers.weixi… |
| 73 | `IWxaExpressLocalService` | `AddOrderAsync` | POST | `/cgi-bin/express/local/business/order/add` | `AccessToken`（Query），Query `access_token` | 添加配送单（`POST /cgi-bin/express/local/business/order/add`）。<br>官方文档：https://developers.weixin.qq… |
| 74 | `IWxaExpressLocalService` | `ReAddOrderAsync` | POST | `/cgi-bin/express/local/business/order/readd` | `AccessToken`（Query），Query `access_token` | 重新下单（`POST /cgi-bin/express/local/business/order/readd`）。<br>官方文档：https://developers.weixin.q… |
| 75 | `IWxaExpressLocalService` | `GetOrderAsync` | POST | `/cgi-bin/express/local/business/order/get` | `AccessToken`（Query），Query `access_token` | 拉取配送单信息（`POST /cgi-bin/express/local/business/order/get`）。<br>官方文档：https://developers.weixin.… |
| 76 | `IWxaExpressLocalService` | `CancelOrderAsync` | POST | `/cgi-bin/express/local/business/order/cancel` | `AccessToken`（Query），Query `access_token` | 取消配送单（`POST /cgi-bin/express/local/business/order/cancel`）。<br>官方文档：https://developers.weixin… |
| 77 | `IWxaExpressLocalService` | `PreCancelOrderAsync` | POST | `/cgi-bin/express/local/business/order/precancel` | `AccessToken`（Query），Query `access_token` | 预取消配送单（`POST /cgi-bin/express/local/business/order/precancel`）。<br>官方文档：https://developers.we… |
| 78 | `IWxaExpressLocalService` | `ConfirmReturnAsync` | POST | `/cgi-bin/express/local/business/order/confirm_return` | `AccessToken`（Query），Query `access_token` | 异常件退回商家确认（`POST /cgi-bin/express/local/business/order/confirm_return`）。<br>官方文档：https://devel… |
| 79 | `IWxaExpressLocalService` | `AddTipsAsync` | POST | `/cgi-bin/express/local/business/order/addtips` | `AccessToken`（Query），Query `access_token` | 添加小费（`POST /cgi-bin/express/local/business/order/addtips`）。<br>官方文档：https://developers.weixin… |
| 80 | `IWxaExpressLocalService` | `MockUpdateOrderAsync` | POST | `/cgi-bin/express/local/business/test_update_order` | `AccessToken`（Query），Query `access_token` | 沙盒环境模拟配送公司更新配送单状态（`POST /cgi-bin/express/local/business/test_update_order`）。<br>官方文档：https://… |
| 81 | `IWxaExpressLocalService` | `RealMockUpdateOrderAsync` | POST | `/cgi-bin/express/local/business/realmock_update_order` | `AccessToken`（Query），Query `access_token` | 测试环境模拟更新配送单状态（`POST /cgi-bin/express/local/business/realmock_update_order`）。<br>官方文档：https://… |
| 82 | `IWxaExpressLocalService` | `UpdateOrderStatusAsync` | POST | `/cgi-bin/express/local/delivery/update_order` | `AccessToken`（Query），Query `access_token` | 运力方更新配送单状态（`POST /cgi-bin/express/local/delivery/update_order`）。<br>官方文档：https://developers.w… |
| M1 | `IWxaExpressBillService` | `GetBillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/get_bill` | 手工注入 `access_token`（非 `[Token]`） | 拉取商家寄件对账文件（`POST /cgi-bin/express/delivery/single_waybill/get_bill`）。<br>官方文档：https://develop… |

> **手工通道**：`IWxaExpressBillService`（1 支） 不是 `[HttpClientApi]` 接口 —— 成功响应为二进制流 / 文本等原始内容，须走原始响应直读 + Content-Type 分支判错，故没有 HTTP 特性。其方法仍是官方端点，已计入本域合计，明细见下方「手工通道端点」。

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaExpressBillService`

**为何不是 `[HttpClientApi]` 接口**：`get_bill` 成功时返回**对账文件内容（文本，非 JSON）**，
失败时才返回 JSON 错误信封（`errcode`/`errmsg`）⇒ 必须走原始响应直读 + Content-Type 分支判错
（同 `IWxaFeedbackMediaService` / `IWxaCodeService` 的手动通道形态）。

**令牌**：手工取当前应用令牌拼 Query（`access_token`），**不进** Query 令牌白名单
（白名单只收 `[Token]` 声明式接口）；令牌失效（`40001` / `40014` / `42001`）时失效本应用令牌并重试一次。

**调用方**：面向**微信物流服务商**；请求体 `WxaExpressSingleWaybillGetBillRequest` 中
`date` 为对账日期（格式 YYYY-MM-DD）、`type` 为对账类型（ALL 全部 / SUCCESS 成功订单 / REFUND 退款订单）。

### `IWxaExpressBusinessService`

**官方文档**（小程序服务端 API → 物流 → 商家寄件，2026-10-11 逐页核验）：
下单 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder、
查询 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder、
批量查询 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder、
取消 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_cancelorder、
轨迹 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath、
快递公司列表 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getdelivery、
快递账号列表 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getaccount、
绑定账号 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_bindaccount、
打印员列表 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getprinter、
绑定打印员 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_updateprinter、
面单余额 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getquota、
模拟轨迹 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_testupdateorder。

**令牌形态（MP-X2 / MP-X3）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。
**GET 端点（delivery/getall、account/getall、printer/getall）**无请求体。

**业务警示**：下单须已绑定快递账号且开通下单权限；取消只能取消**待揽件**运单；
`test_update_order` 仅供**测试环境**（生产不可调用）。

### `IWxaExpressDeliveryService`

**官方文档**（2026-10-12 逐页核验）：
面单联系人 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact、
面单预览 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate、
订单审核 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatebusiness、
轨迹同步 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatepath、
无忧退 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid（及 get/unbind）、
物流消息 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill（及 query/update/get_delivery_list）、
物流查询插件 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill（及 query_trace/update_waybill_goods）、
退货组件 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn（及 get/unbind）、
散单寄件 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderstatus（及 fee/cancel/refund/evaluate_feedback）、
投诉处理 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdatecomplainresult、
消息推送 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliverypathnotify
与 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliveryuserquery。

**令牌形态（MP-X2 / MP-X3）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。
四个「开通类」端点（`open_openmsg` / `open_query_plugin` / `open_return` / `get_delivery_list`）
请求体为**空对象**（无字段）。

**业务警示**：本域大多面向**微信物流服务商（服务商代调用）**；散单寄件端点（`single_waybill/*`）
以**商户侧下单事件中推送的 Token** 作为调用凭据（非模糊查询令牌）；`get_bill` 对账文件端点
返回**文件内容（非 JSON）**，走独立通道 `IWxaExpressBillService`（不在本接口）。

### `IWxaExpressInsuranceFreightService`

**官方文档**（小程序服务端 API → 微信物流服务 → 无忧退货，2026-10-12 逐页核验）：
开通 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_open、
查询开通状态 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_query_open、
发货时投保 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createorder、
无忧退理赔 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_claim、
申请充值订单号 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createchargeid、
申请支付 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_applypay、
拉取充值订单信息 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getpayorderlist、
充值保费退款 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_refund、
拉取理赔摘要 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getsummary、
拉取保单信息 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getorderlist、
设置保费告警余额 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_update_notify_funds。

**令牌形态（MP-X2 / MP-X3）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。
官方声明本族**支持第三方平台代商家调用**（权限集 id **139**，可用 `authorizer_access_token`）；
本线按 P3-R2 裁决**不**引入开放平台令牌链，故仅承载自建形态的单级 `access_token`。

**形态差异（勿「顺手对齐」）**：`open` 与 `query_open` 官方为 **GET 且无请求体**，
`refund` 官方为 **POST 且无请求体**（参数表原文「无」），其余 8 端点为 POST ＋ 请求体 JSON。

**业务顺序（官方原文推断的调用链）**：先 `OpenInsuranceFreightAsync` 开通 →
`CreateInsuranceOrderAsync` 发货时投保 → 收到退货后 `ClaimAsync` 理赔；
保费侧：`CreateChargeIdAsync` → `ApplyPayAsync`（拿充值链接付款）。
**官方特别提示**：充值后退款**只能退回原充值账号**。

### `IWxaExpressIntracityService`

**官方文档**（小程序服务端 API → 微信物流服务 → 同城配送，2026-10-12 逐页核验）：
申请开通 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_apply、
创建门店 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore、
查询门店 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_querystore、
更新门店 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore、
运费充值 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge、
运费退款 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storerefund、
查询流水 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow、
查询余额 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_balancequery、
询价 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder、
下单 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder、
查询配送单 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder、
取消配送单 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder、
模拟回调 https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_mocknotify、
设置扣费主体 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_setpaymode、
查询扣费主体 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getpaymode、
查询支持同城配送的城市 https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getcity。

**令牌形态（MP-X2 / MP-X3）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。

**业务警示**：本域全部端点为 **POST** ＋ 请求体 JSON；`mocknotify` 仅供**测试环境**调用；
同城配送使用前须先开通并创建门店（`wx_store_id` 是后续运力订单的定位键）。

### `IWxaExpressLocalService`

**官方文档**（小程序服务端 API → 即时配送，2026-10-12 逐页核验）：
已支持配送公司列表 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getallimmedelivery、
申请开通 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_opendelivery、
绑定账号 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_bindlocalaccount、
已绑定账号 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getbindaccount、
预下单 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_preaddorder、
下单 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addlocalorder、
重新下单 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_reorder、
拉取配送单 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getlocalorder、
取消 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_cancellocalorder、
预取消 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_precancelorder、
异常件退回确认 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_abnormalconfirm、
添加小费 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addtips、
沙盒模拟状态 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_mockupdateorder、
测试环境模拟状态 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_realmockupdateorder、
运力方更新状态 https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-provider/api_updateorder。

**令牌形态（MP-X2 / MP-X3）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。
官方声明本族**支持第三方平台代商家调用**（权限集 id **51、71**；`update_order` 官方明示**不支持**）；
本线按 P3-R2 裁决**不**引入开放平台令牌链，故仅承载自建形态的单级 `access_token`。

**本域与「物流助手 / 微信物流服务」的差异（勿混用）**：本域路由前缀为 `/cgi-bin/express/local/*`，
面向**配送公司（即时配送运力）**；返回体用 `resultcode` / `resultmsg` 而非 `errcode` / `errmsg`
（见 `WxaExpressLocalResponse`）。

**敏感字段（MP-X7 同口径）**：`delivery_sign`（appSecret 加密校验串）、`phone` / `rider_phone`
**不得**入日志 / 遥测 / 异常消息；SDK 只在 DTO 上承载，不做任何回显。

**官方接入前置**：小程序须完成微信认证 + 开通事件推送（安全模式 / JSON），并在配送公司注册账号后
于小程序后台完成授权绑定。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 53 次）官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。
- （出现 4 次）官方契约：**POST** ＋ 空请求体；Query 携带 `access_token`。

### 1. `AddOrderAsync` — 商家寄件下单（`POST /cgi-bin/express/business/order/add`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/order/add`

- **请求体**：`WxaExpressAddOrderRequest` — 下单请求体（`order_id`/`openid`/`delivery_id`/`biz_id`/`add_source`/发收件人/包裹/商品/保价/服务必填），见 `WxaExpressAddOrderRequest`。
- **返回**：`WxaExpressAddOrderResponse` — 运单号与面单数据（`order_id`/`waybill_id`/`waybill_data`），见 `WxaExpressAddOrderResponse`。

**备注（原文）**

- **前置与频率（官方原文）**：调用前须已绑定快递账号且开通下单权限；官方建议单商户日下单量超 / 10 万时联系快递公司申请提额。`add_source=2`（App/AppH5 订单）时不发送物流服务通知； / `expect_time` 顺丰必传，0 表示已事先约定取件时间；选择预期揽件时间后**不要自己打单**。 / 平台型小程序 `tagid` 须与入驻方账号一一对应。
- 官方错误码：`40001`（令牌无效，走自愈）/ `40097`（参数不完整）。

### 2. `GetOrderAsync` — 查询运单信息（`POST /cgi-bin/express/business/order/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/order/get`

- **请求体**：`WxaExpressGetOrderRequest` — 查询请求体（`order_id`/`openid`/`delivery_id` 必填），见 `WxaExpressGetOrderRequest`。
- **返回**：`WxaExpressGetOrderResponse` — 运单与面单信息（含 `print_html`），见 `WxaExpressGetOrderResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。下单成功后会收到发货通知事件（推送 `waybill_id`），凭 `order_id` 查询。

### 3. `BatchGetOrderAsync` — 批量查询运单信息（`POST /cgi-bin/express/business/order/batchget`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/order/batchget`

- **请求体**：`WxaExpressBatchGetOrderRequest` — 批量查询请求体（`order_list` 必填，单次最多 50 单），见 `WxaExpressBatchGetOrderRequest`。
- **返回**：`WxaExpressBatchGetOrderResponse` — 逐单运单信息（含逐单 `errcode`/`errmsg` 信封），见 `WxaExpressBatchGetOrderResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；单次最多 50 单。

### 4. `CancelOrderAsync` — 取消运单（`POST /cgi-bin/express/business/order/cancel`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_cancelorder。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/order/cancel`

- **请求体**：`WxaExpressCancelOrderRequest` — 取消请求体（`order_id`/`openid`/`delivery_id` 必填），见 `WxaExpressCancelOrderRequest`。
- **返回**：`WxaExpressCancelOrderResponse` — 快递侧取消结果，见 `WxaExpressCancelOrderResponse`。

**备注（原文）**

- **业务限制（官方原文）**：只能取消**待揽件**运单，揽件成功或运输中的运单无法取消。

### 5. `GetPathAsync` — 查询物流轨迹（`POST /cgi-bin/express/business/path/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/path/get`

- **请求体**：`WxaExpressGetPathRequest` — 轨迹查询请求体（`openid`/`delivery_id`/`waybill_id` 必填），见 `WxaExpressGetPathRequest`。
- **返回**：`WxaExpressGetPathResponse` — 轨迹列表（`path_item_list`），见 `WxaExpressGetPathResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；`action_type` 枚举值同附录轨迹类型。

### 6. `GetAllDeliveryAsync` — 获取快递公司列表（`GET /cgi-bin/express/business/delivery/getall`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getdelivery。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`GET /cgi-bin/express/business/delivery/getall`
- **返回**：`WxaExpressGetAllDeliveryResponse` — 快递公司列表（`data`/`count`），见 `WxaExpressGetAllDeliveryResponse`。

**备注（原文）**

- 官方契约：**GET**，无请求体；Query 携带 `access_token`；`can_use_cash` 表示是否支持现金支付、`can_get_quota` 表示是否支持电子面单。

### 7. `GetAllAccountAsync` — 获取快递账号列表（`GET /cgi-bin/express/business/account/getall`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getaccount。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`GET /cgi-bin/express/business/account/getall`
- **返回**：`WxaExpressGetAllAccountResponse` — 快递账号列表（`list`/`count`），见 `WxaExpressGetAllAccountResponse`。

**备注（原文）**

- 官方契约：**GET**，无请求体；Query 携带 `access_token`。

### 8. `BindAccountAsync` — 绑定 / 解绑快递账号（`POST /cgi-bin/express/business/account/bind`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_bindaccount。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/account/bind`

- **请求体**：`WxaExpressBindAccountRequest` — 绑定 / 解绑请求体（`type`/`biz_id`/`delivery_id` 必填），见 `WxaExpressBindAccountRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示受理成功）。

**备注（原文）**

- **异步绑定（官方原文）**：`type=bind` 绑定（须传入快递公司签约的 `password`）、`type=unbind` 解绑；绑定为**异步**流程，返回结果仅表示受理，最终状态经回调/查询确认。

### 9. `GetAllPrinterAsync` — 获取打印员列表（`GET /cgi-bin/express/business/printer/getall`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getprinter。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`GET /cgi-bin/express/business/printer/getall`
- **返回**：`WxaExpressGetPrinterResponse` — 打印员 OpenId 列表（`openid`/`tagid_list`/`count`），见 `WxaExpressGetPrinterResponse`。

**备注（原文）**

- 官方契约：**GET**，无请求体；Query 携带 `access_token`。

### 10. `UpdatePrinterAsync` — 绑定 / 解绑打印员（`POST /cgi-bin/express/business/printer/update`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_updateprinter。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/printer/update`

- **请求体**：`WxaExpressUpdatePrinterRequest` — 绑定 / 解绑请求体（`update_type`/`openid` 必填），见 `WxaExpressUpdatePrinterRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- **业务语义（官方原文）**：`update_type=bind` 绑定 / `unbind` 解绑；绑定后商家端下单可指定打印员（快递员）接收面单打印通知。

### 11. `GetQuotaAsync` — 查看电子面单余额（`POST /cgi-bin/express/business/quota/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getquota。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/quota/get`

- **请求体**：`WxaExpressGetQuotaRequest` — 余额查询请求体（`delivery_id`/`biz_id` 必填），见 `WxaExpressGetQuotaRequest`。
- **返回**：`WxaExpressGetQuotaResponse` — 电子面单余额（`quota_num`），见 `WxaExpressGetQuotaResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；`quota_num` 为 -1 表示不限制。

### 12. `TestUpdateOrderAsync` — 模拟快递公司更新订单状态（`POST /cgi-bin/express/business/test_update_order`，**仅测试环境**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_testupdateorder。

- **声明接口**：[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)
- **官方契约**：`POST /cgi-bin/express/business/test_update_order`

- **请求体**：`WxaExpressTestUpdateOrderRequest` — 模拟轨迹请求体（`biz_id`/`order_id`/`delivery_id`/`waybill_id`/轨迹字段 必填），见 `WxaExpressTestUpdateOrderRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- **测试专用（官方原文）**：日常测试时调用本接口模拟快递公司推送轨迹，观察商户侧回调联调；生产环境不可调用。

### 13. `GetContactAsync` — 获取面单联系人信息（`POST /cgi-bin/express/delivery/contact/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/contact/get`

- **请求体**：`WxaExpressGetContactRequest` — 联系人查询请求体（`token`/`waybill_id` 必填），见 `WxaExpressGetContactRequest`。
- **返回**：`WxaExpressGetContactResponse` — 面单发收件人（`sender`/`receiver`），见 `WxaExpressGetContactResponse`。

**备注（原文）**

- 本组接口面向**微信物流服务商**：`token` 为商户侧下单事件中推送的 Token 字段（非模糊查询令牌）。

### 14. `PreviewTemplateAsync` — 预览面单模板（`POST /cgi-bin/express/delivery/template/preview`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/template/preview`

- **请求体**：`WxaExpressPreviewTemplateRequest` — 面单预览请求体（`waybill_id`/`waybill_template`/`waybill_data`/`custom` 必填），见 `WxaExpressPreviewTemplateRequest`。
- **返回**：`WxaExpressPreviewTemplateResponse` — 渲染后的面单 HTML（`rendered_waybill_template`），见 `WxaExpressPreviewTemplateResponse`。

**备注（原文）**

- 服务商自定义面单模板时调用，用于预览渲染效果；`waybill_template` 为需 Base64 编码的 HTML 模板内容。

### 15. `UpdateBusinessAsync` — 服务商更新订单审核结果（`POST /cgi-bin/express/delivery/service/business/update`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatebusiness。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/service/business/update`

- **请求体**：`WxaExpressUpdateBusinessRequest` — 审核结果回传请求体（`shop_app_id`/`biz_id`/`result_code` 必填），见 `WxaExpressUpdateBusinessRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 服务商承接商户绑定时异步审核（账号绑定为异步流程），审核完成后调用本接口把结果回传微信侧。

### 16. `UpdatePathAsync` — 服务商更新运单轨迹（`POST /cgi-bin/express/delivery/path/update`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatepath。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/path/update`

- **请求体**：`WxaExpressUpdatePathRequest` — 轨迹同步请求体（`token`/`waybill_id`/轨迹字段 必填），见 `WxaExpressUpdatePathRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 服务商（或运力）侧订单状态变化时调用，将轨迹实时同步给微信物流助手；`action_type` 枚举值同附录 action_type 定义。

### 17. `AddNoWorryReturnAsync` — 创建退货 ID（无忧退货运费险，`POST /cgi-bin/express/delivery/no_worry_return/add`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_addreturnid。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/no_worry_return/add`

- **请求体**：`WxaExpressAddReturnIdRequest` — 创建退货请求体（`shop_order_id`/`biz_addr`/`openid`/`order_path`/`goods_list`/`order_price`/`wx_pay_id` 必填），见 `WxaExpressAddReturnIdRequest`。
- **返回**：`WxaExpressAddReturnIdResponse` — 退货 ID（`return_id`），见 `WxaExpressAddReturnIdResponse`。

**备注（原文）**

- `shop_order_id` 与退货 ID 一一对应；“一个订单需要多次退货”可在商家内部用 1 个退货订单号映射多个 `shop_order_id`。 / `wx_pay_id` 为**已投保的微信支付单号**（必填）：无该字段返回 9300569，未投保即 9300570。 / 创建退货 ID 时平台自动下发模板消息给用户提醒退货。
- 官方错误码：`40097`（order_id 或退货地址或 wx_pay_id 为空）、`9300522`（shop_order_id 已存在）。

### 18. `GetNoWorryReturnAsync` — 查询退货 ID 状态（无忧退货运费险，`POST /cgi-bin/express/delivery/no_worry_return/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_getreturnid。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/no_worry_return/get`

- **请求体**：`WxaExpressGetReturnIdRequest` — 退货状态查询请求体（`return_id` 必填），见 `WxaExpressGetReturnIdRequest`。
- **返回**：`WxaExpressReturnStatusResponse` — 退货状态（`status`/`waybill_id`/`order_status` 等），见 `WxaExpressReturnStatusResponse`。

### 19. `UnbindNoWorryReturnAsync` — 解绑退货 ID（无忧退货运费险，`POST /cgi-bin/express/delivery/no_worry_return/unbind`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-return/api_unbindreturnid。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/no_worry_return/unbind`

- **请求体**：`WxaExpressUnbindReturnIdRequest` — 解绑退货请求体（`return_id` 必填），见 `WxaExpressUnbindReturnIdRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 与用户达成协商「无需退货」时解除商家退货单与退货 ID 的绑定；仅当用户自主填写运单号时才支持解绑。

### 20. `FollowWaybillAsync` — 用户绑定运单（物流消息，`POST /cgi-bin/express/delivery/open_msg/follow_waybill`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/follow_waybill`

- **请求体**：`WxaExpressFollowWaybillRequest` — 绑定运单请求体（`openid`/`waybill_id`/`goods_info` 必填），见 `WxaExpressFollowWaybillRequest`。
- **返回**：`WxaExpressFollowWaybillResponse` — 运单 Token（`waybill_token`），见 `WxaExpressFollowWaybillResponse`。

**备注（原文）**

- **频率上限（官方原文）**：同一 appid 最多调用 **10 万次/日**；同一用户最多被调用 **50 次/日**； / 同一用户最多同时绑定 **100 个**运单号。绑定后向该用户下发物流状态通知。

### 21. `QueryFollowTraceAsync` — 查询运单轨迹（物流消息，`POST /cgi-bin/express/delivery/open_msg/query_follow_trace`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_queryfollowtrace。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/query_follow_trace`

- **请求体**：`WxaExpressQueryFollowTraceRequest` — 轨迹查询请求体（`waybill_token`/`openid` 必填），见 `WxaExpressQueryFollowTraceRequest`。
- **返回**：`WxaExpressTraceResponse` — 运单轨迹明细（`waybill_info`/`shop_info`/`delivery_info`），见 `WxaExpressTraceResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；凭 `WaybillToken` 查询。

### 22. `UpdateFollowWaybillGoodsAsync` — 更新运单商品信息（物流消息，`POST /cgi-bin/express/delivery/open_msg/update_follow_waybill_goods`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_updatefollowwaybillgood。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/update_follow_waybill_goods`

- **请求体**：`WxaExpressUpdateFollowWaybillGoodsRequest` — 商品更新请求体（`waybill_token`/`openid`/`goods_info` 必填），见 `WxaExpressUpdateFollowWaybillGoodsRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

### 23. `OpenOpenMessageAsync` — 开通物流消息（`POST /cgi-bin/express/delivery/open_msg/open_openmsg`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/logistics-service/applyMsgPlugin.html。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/open_openmsg`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 开通后用户可绑定运单并接收物流状态通知；须具备相应接口权限集（45）。错误码 `933000`~`933004`。

### 24. `OpenQueryPluginAsync` — 开通物流查询插件（`POST /cgi-bin/express/delivery/open_msg/open_query_plugin`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/logistics-service/applyQueryPlugin.html。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/open_query_plugin`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 开通后用户可凭运单号 + 手机号查询物流轨迹（`trace_waybill` / `query_trace`）。须具备相应接口权限集（45）。

### 25. `TraceWaybillAsync` — 查询运单信息（物流查询插件，`POST /cgi-bin/express/delivery/open_msg/trace_waybill`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/trace_waybill`

- **请求体**：`WxaExpressTraceWaybillRequest` — 运单查询请求体（`openid`/`waybill_id`/`goods_info`/`delivery_id` 必填），见 `WxaExpressTraceWaybillRequest`。
- **返回**：`WxaExpressTraceWaybillResponse` — 运单 Token（`waybill_token`），见 `WxaExpressTraceWaybillResponse`。

**备注（原文）**

- `delivery_id` 必填（无单号无法匹配快递公司时可用 single-delivery 场景的运单绑定先转正）；返回 `waybill_token` 供后续查询。

### 26. `QueryTraceAsync` — 查询运单轨迹（物流查询插件，`POST /cgi-bin/express/delivery/open_msg/query_trace`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_querytrace。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/query_trace`

- **请求体**：`WxaExpressQueryTraceRequest` — 轨迹查询请求体（`waybill_token`/`openid` 必填），见 `WxaExpressQueryTraceRequest`。
- **返回**：`WxaExpressTraceResponse` — 运单轨迹明细，见 `WxaExpressTraceResponse`。

**备注（原文）**

- **频率上限（官方原文）**：一个 appid 最多调用 **10 万次/日**；一个用户最多调用 **100 次/日**。

### 27. `UpdateWaybillGoodsAsync` — 更新运单商品信息（物流查询插件，`POST /cgi-bin/express/delivery/open_msg/update_waybill_goods`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_updatewaybillgoods。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/update_waybill_goods`

- **请求体**：`WxaExpressUpdateWaybillGoodsRequest` — 商品更新请求体（`waybill_token`/`openid`/`goods_info` 必填），见 `WxaExpressUpdateWaybillGoodsRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

### 28. `GetDeliveryListAsync` — 获取快递公司列表（物流消息 / 查询插件共用，`POST /cgi-bin/express/delivery/open_msg/get_delivery_list`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_getdeliverylist。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/open_msg/get_delivery_list`
- **返回**：`WxaExpressGetDeliveryListResponse` — 支持物流消息 / 查询能力的快递公司列表（`delivery_list`/`count`），见 `WxaExpressGetDeliveryListResponse`。

### 29. `OpenReturnAsync` — 开通退货组件（`POST /cgi-bin/express/delivery/return/open_return`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/logistics-service/applyReturnPlugin.html。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/return/open_return`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 开通后可调用退货组件相关接口（`return/add`/`get`/`unbind`）。调用额度 **500 次/天**；须具备接口权限集（45）。

### 30. `AddReturnAsync` — 创建退货 ID（退货组件，`POST /cgi-bin/express/delivery/return/add`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_addreturn。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/return/add`

- **请求体**：`WxaExpressAddReturnRequest` — 创建退货请求体（`shop_order_id`/`biz_addr`/`openid`/`order_path`/`goods_list`/`order_price` 必填），见 `WxaExpressAddReturnRequest`。
- **返回**：`WxaExpressAddReturnResponse` — 退货 ID（`return_id`），见 `WxaExpressAddReturnResponse`。

**备注（原文）**

- 与无忧退货运费险不同：**本族无 `wx_pay_id`**（不要求保单）；`shop_order_id` 与退货 ID 一一对应， / “一个订单需要多次退货”时可在商家内部 1 个退货订单号映射多个 `shop_order_id`。
- 官方错误码：`40097`（order_id 或退货地址为空）、`9300522`（shop_order_id 已存在）。

### 31. `GetReturnAsync` — 查询退货 ID 状态（退货组件，`POST /cgi-bin/express/delivery/return/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_getreturn。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/return/get`

- **请求体**：`WxaExpressGetReturnRequest` — 退货状态查询请求体（`return_id` 必填），见 `WxaExpressGetReturnRequest`。
- **返回**：`WxaExpressReturnStatusResponse` — 退货状态，见 `WxaExpressReturnStatusResponse`。

### 32. `UnbindReturnAsync` — 解绑退货 ID（退货组件，`POST /cgi-bin/express/delivery/return/unbind`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_unbindreturn。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/return/unbind`

- **请求体**：`WxaExpressUnbindReturnRequest` — 解绑退货请求体（`return_id` 必填），见 `WxaExpressUnbindReturnRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 商家同意退货后与用户达成协商「无需退货」时解除绑定；仅当用户自主填写运单号（自行寄回）时才支持解绑。

### 33. `UpdateSingleWaybillAsync` — 运力更新订单状态（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/update`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderstatus。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/single_waybill/update`

- **请求体**：`WxaExpressSingleWaybillUpdateRequest` — 订单状态更新请求体（`token`/`action_time`/`action_type`/`action_msg`/取派件员信息 必填），见 `WxaExpressSingleWaybillUpdateRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- `action_type` 枚举见附录（`90001` 网点接单 / `90002` 分配业务员 / `100001` 揽件成功 …）。

### 34. `UpdateSingleWaybillFeeAsync` — 运力更新订单费用（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/fee`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderfee。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/single_waybill/fee`

- **请求体**：`WxaExpressSingleWaybillFeeRequest` — 费用更新请求体（`token`/`waybill_id`/`need_pay`/费用字段 必填），见 `WxaExpressSingleWaybillFeeRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 所有费用字段单位均为**分**；`need_pay`：0 表示无需支付，1 表示需要支付。

### 35. `CancelSingleWaybillAsync` — 运力取消订单（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/cancel_order`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterdeliverycancel。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/single_waybill/cancel_order`

- **请求体**：`WxaExpressSingleWaybillCancelRequest` — 取消订单请求体（`token`/`reason` 必填），见 `WxaExpressSingleWaybillCancelRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

### 36. `RefundSingleWaybillAsync` — 运力对订单退款（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/refund_order`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterdeliveryrefund。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/single_waybill/refund_order`

- **请求体**：`WxaExpressSingleWaybillRefundRequest` — 退款请求体（`token`/`fee` 必填，`fee` 为退款金额单位分），见 `WxaExpressSingleWaybillRefundRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

### 37. `EvaluateSingleWaybillFeedbackAsync` — 运力回传用户评价反馈（散单寄件，`POST /cgi-bin/express/delivery/single_waybill/evaluate_feedback`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterevaluatefeedback。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/single_waybill/evaluate_feedback`

- **请求体**：`WxaExpressSingleWaybillEvaluateFeedbackRequest` — 评价反馈请求体（`token`/`waybill_id`/`result` 必填），见 `WxaExpressSingleWaybillEvaluateFeedbackRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- `result`：0 已处理，1 不处理；`desc` 为处理结果描述（可选）。

### 38. `UpdateComplaintResultAsync` — 运力返回用户投诉处理结果（散单寄件，`POST /cgi-bin/express/delivery/scatter/update_complaint_result`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdatecomplainresult。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/scatter/update_complaint_result`

- **请求体**：`WxaExpressUpdateComplaintResultRequest` — 投诉处理结果请求体（`token`/`waybill_id`/`result`/`desc` 必填），见 `WxaExpressUpdateComplaintResultRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- **官方原文**：`token` 为**商户侧下单事件中推送的 Token 字段**（与 / `EvaluateSingleWaybillFeedbackAsync` 同源），**不得**用应用级令牌替代； / 本接口**不支持**第三方平台代调用。

### 39. `NotifyPathAsync` — 推送已绑定物流轨迹信息（`POST /cgi-bin/express/delivery/pathnotify`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliverypathnotify。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/pathnotify`

- **请求体**：`WxaExpressPathNotifyRequest` — 轨迹推送请求体（`sender`/`receiver`/`waybill_id`/`path`/`create_time` 必填），见 `WxaExpressPathNotifyRequest`。
- **返回**：`WxaExpressPathNotifyResponse` — 推送结果（`exist` 表示用户是否绑定），见 `WxaExpressPathNotifyResponse`。

**备注（原文）**

- **业务语义**：运力方调用本接口向**已绑定**用户推送物流轨迹消息。 / `exist` 为 `0`（未绑定）时消息**不会**送达，须先引导用户完成绑定 / （绑定状态可用 `QueryUserBindingAsync` 预检）。

### 40. `QueryUserBindingAsync` — 用户手机状态查询（`POST /cgi-bin/express/delivery/userquery`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliveryuserquery。

- **声明接口**：[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/userquery`

- **请求体**：`WxaExpressUserQueryRequest` — 查询请求体（`phone` 必填），见 `WxaExpressUserQueryRequest`。
- **返回**：`WxaExpressUserQueryResponse` — 绑定状态（`exist`：`0` 未绑定，`1` 已绑定），见 `WxaExpressUserQueryResponse`。

**备注（原文）**

- **敏感字段（MP-X7 同口径）**：`phone` 为**用户手机号**， / **不得**入日志 / 遥测 / 异常消息；SDK 只在 DTO 上承载，不做任何回显。

### 41. `OpenInsuranceFreightAsync` — 开通无忧退货（`GET /wxa/business/insurance_freight/open`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_open。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`GET /wxa/business/insurance_freight/open`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示开通成功）。

**备注（原文）**

- 官方契约：**GET**，**无请求体**；Query 携带 `access_token`。
- 开通结果可用 `QueryInsuranceFreightOpenAsync` 复核（`is_open`）。

### 42. `QueryInsuranceFreightOpenAsync` — 查询无忧退货开通状态（`GET /wxa/business/insurance_freight/query_open`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_query_open。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`GET /wxa/business/insurance_freight/query_open`
- **返回**：`WxaExpressQueryInsuranceOpenResponse` — 开通状态（`is_open`：`0` 否，`1` 是），见 `WxaExpressQueryInsuranceOpenResponse`。

**备注（原文）**

- 官方契约：**GET**，**无请求体**；Query 携带 `access_token`。

### 43. `CreateInsuranceOrderAsync` — 发货时投保（`POST /wxa/business/insurance_freight/createorder`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createorder。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/createorder`

- **请求体**：`WxaExpressCreateInsuranceOrderRequest` — 投保请求体（`openid`/`order_no`/`pay_time`/`pay_amount`/`delivery_no`/`delivery_place`/`receipt_place`/`product_info` 必填），见 `WxaExpressCreateInsuranceOrderRequest`。
- **返回**：`WxaExpressCreateInsuranceOrderResponse` — 保单信息（`policy_no`/`insurance_end_date`/`estimate_amount`/`premium`），见 `WxaExpressCreateInsuranceOrderResponse`。

**备注（原文）**

- **官方约束**：`openid` 必须和理赔 `openid` 一致；一个微信支付单号**只能投保一次**； / `pay_time` 与微信支付时间误差须在 3 天内。常见错误码 `41009`（missing openid）。

### 44. `ClaimAsync` — 无忧退理赔（`POST /wxa/business/insurance_freight/claim`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_claim。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/claim`

- **请求体**：`WxaExpressClaimRequest` — 理赔请求体（`openid`/`order_no`/`refund_delivery_no`/`refund_company` 必填），见 `WxaExpressClaimRequest`。
- **返回**：`WxaExpressClaimResponse` — 理赔结果（`report_no`/`is_home_pick_up`），见 `WxaExpressClaimResponse`。

**备注（原文）**

- **官方原文**：**收到用户退货后**再触发；`openid` / `order_no` 须与投保时**保持一致**，`refund_delivery_no` 唯一。

### 45. `CreateChargeIdAsync` — 申请充值订单号（`POST /wxa/business/insurance_freight/createchargeid`，支持自定义金额）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createchargeid。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/createchargeid`

- **请求体**：`WxaExpressCreateChargeIdRequest` — 充值申请请求体（`quota` 必填，单位：分），见 `WxaExpressCreateChargeIdRequest`。
- **返回**：`WxaExpressCreateChargeIdResponse` — 充值订单号（`order_id`），见 `WxaExpressCreateChargeIdResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。`order_id` 为 64 位整数，JS 等语言须按 string 传递以防精度丢失。

### 46. `ApplyPayAsync` — 申请支付（`POST /wxa/business/insurance_freight/applypay`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_applypay。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/applypay`

- **请求体**：`WxaExpressApplyPayRequest` — 支付申请请求体（`order_id` 必填），见 `WxaExpressApplyPayRequest`。
- **返回**：`WxaExpressApplyPayResponse` — 充值链接（`pay_url`），见 `WxaExpressApplyPayResponse`。

**备注（原文）**

- **官方特别提示**：充值后退款**只能退回原充值账号**。

### 47. `GetPayOrderListAsync` — 拉取充值订单信息（`POST /wxa/business/insurance_freight/getpayorderlist`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getpayorderlist。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/getpayorderlist`

- **请求体**：`WxaExpressGetPayOrderListRequest` — 查询请求体（`status_list` 必填，`offset`/`limit` 可选），见 `WxaExpressGetPayOrderListRequest`。
- **返回**：`WxaExpressGetPayOrderListResponse` — 充值订单列表（`total`/`list`），见 `WxaExpressGetPayOrderListResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。订单状态与退款状态枚举见 `WxaExpressPayOrderItem`。

### 48. `RefundPremiumAsync` — 充值保费退款（`POST /wxa/business/insurance_freight/refund`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_refund。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/refund`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示退款受理成功）。

**备注（原文）**

- 官方契约：**POST**，**无请求体**（官方「请求体 Request Payload」原文为「无」）；Query 携带 `access_token`。
- **官方特别提示**：充值后退款**只能退回原充值账号**。

### 49. `GetSummaryAsync` — 拉取理赔摘要（`POST /wxa/business/insurance_freight/getsummary`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getsummary。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/getsummary`

- **请求体**：`WxaExpressGetSummaryRequest` — 摘要查询请求体（`begin_time`/`end_time` 必填），见 `WxaExpressGetSummaryRequest`。
- **返回**：`WxaExpressGetSummaryResponse` — 当前保费、投保单量、理赔单量、账号余额等，见 `WxaExpressGetSummaryResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。官方错误码 `-202` 为内部错误（可稍后重试）。

### 50. `GetOrderListAsync` — 拉取保单信息（`POST /wxa/business/insurance_freight/getorderlist`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getorderlist。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/getorderlist`

- **请求体**：`WxaExpressGetOrderListRequest` — 保单查询请求体（**全字段可选**；`limit` 默认 100、最大 100），见 `WxaExpressGetOrderListRequest`。
- **返回**：`WxaExpressGetOrderListResponse` — 保单列表（`total`/`list`），见 `WxaExpressGetOrderListResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。保单状态与排序方式枚举见 `WxaExpressInsuranceOrderItem` / `SortDirection`。

### 51. `UpdateNotifyFundsAsync` — 设置保费告警余额（`POST /wxa/business/insurance_freight/update_notify_funds`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_update_notify_funds。

- **声明接口**：[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)
- **官方契约**：`POST /wxa/business/insurance_freight/update_notify_funds`

- **请求体**：`WxaExpressUpdateNotifyFundsRequest` — 告警余额请求体（`notify_funds` 必填，单位：分；置 `0` 表示不通知），见 `WxaExpressUpdateNotifyFundsRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示设置成功）。

**备注（原文）**

- **官方原文**：余额为设定值时通知小程序管理员（余额无改动情况下 24h 内通知一次），通知消息经**微信公众平台**下发。

### 52. `ApplyIntracityAsync` — 申请开通同城配送（`POST /cgi-bin/express/intracity/apply`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_apply。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/apply`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示受理成功）。

**备注（原文）**

- 官方契约：**POST**，**无请求体**；Query 携带 `access_token`。
- **业务语义（官方原文）**：使用同城配送能力前须先申请开通；本接口**无请求体**， / 申请结果经**审核商户事件**（`review_merchant`）异步通知，后续方可创建门店并下单。

### 53. `CreateStoreAsync` — 创建门店（`POST /cgi-bin/express/intracity/createstore`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/createstore`

- **请求体**：`WxaExpressCreateStoreRequest` — 创建门店请求体（`out_store_id`/`store_name`/`address_info` 必填），见 `WxaExpressCreateStoreRequest`。
- **返回**：`WxaExpressCreateStoreResponse` — 门店编号（`out_store_id`/`wx_store_id`/`appid`），见 `WxaExpressCreateStoreResponse`。

**备注（原文）**

- `out_store_id` 自定义门店编号需唯一；创建后系统生成全局唯一 `wx_store_id`， / 后续创建运力订单需要该门店编号。`order_pattern`：1 价格优先，2 运力优先（默认价格优先）； / `order_pattern=2` 时 `service_trans_prefer` 必填。

### 54. `QueryStoreAsync` — 查询门店（`POST /cgi-bin/express/intracity/querystore`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_querystore。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/querystore`

- **请求体**：`WxaExpressQueryStoreRequest` — 查询门店请求体（`out_store_id` 与 `wx_store_id` 二选一），见 `WxaExpressQueryStoreRequest`。
- **返回**：`WxaExpressQueryStoreResponse` — 门店列表（`store_list`/`total`），见 `WxaExpressQueryStoreResponse`。

### 55. `UpdateStoreAsync` — 更新门店（`POST /cgi-bin/express/intracity/updatestore`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/updatestore`

- **请求体**：`WxaExpressUpdateStoreRequest` — 更新门店请求体（`keys`/`content` 必填），见 `WxaExpressUpdateStoreRequest`。
- **返回**：`WxaExpressUpdateStoreResponse` — 更新后门店编号，见 `WxaExpressUpdateStoreResponse`。

**备注（原文）**

- `keys` 为门店定位（`wx_store_id` 与 `out_store_id` 二选一），`content` 为待更新内容（均可选，只更新传入字段）。

### 56. `StoreChargeAsync` — 门店运费充值（`POST /cgi-bin/express/intracity/storecharge`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/storecharge`

- **请求体**：`WxaExpressStoreChargeRequest` — 充值请求体（`wx_store_id`/`service_trans_id`/`amount` 必填），见 `WxaExpressStoreChargeRequest`。
- **返回**：`WxaExpressStoreChargeResponse` — 支付链接（`payurl`），见 `WxaExpressStoreChargeResponse`。

**备注（原文）**

- 充值金额 `amount` 单位：分；充值成功后返回支付链接 `payurl` 拉起微信支付完成充值。

### 57. `StoreRefundAsync` — 门店运费退款（`POST /cgi-bin/express/intracity/storerefund`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storerefund。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/storerefund`

- **请求体**：`WxaExpressStoreRefundRequest` — 退款请求体（`wx_store_id`/`service_trans_id` 必填），见 `WxaExpressStoreRefundRequest`。
- **返回**：`WxaExpressStoreRefundResponse` — 退款结果（`refund_amount`），见 `WxaExpressStoreRefundResponse`。

### 58. `QueryFlowAsync` — 查询门店运费流水（`POST /cgi-bin/express/intracity/queryflow`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/queryflow`

- **请求体**：`WxaExpressQueryFlowRequest` — 流水查询请求体（`wx_store_id`/`flow_type` 必填），见 `WxaExpressQueryFlowRequest`。
- **返回**：`WxaExpressQueryFlowResponse` — 流水列表（`flow_list`/`total`），见 `WxaExpressQueryFlowResponse`。

**备注（原文）**

- `flow_type`：1 充值，2 扣款，3 退款，4 扣款退回。

### 59. `BalanceQueryAsync` — 查询门店余额（`POST /cgi-bin/express/intracity/balancequery`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_balancequery。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/balancequery`

- **请求体**：`WxaExpressBalanceQueryRequest` — 余额查询请求体（`wx_store_id`/`service_trans_id` 必填），见 `WxaExpressBalanceQueryRequest`。
- **返回**：`WxaExpressBalanceQueryResponse` — 余额明细（`all_balance`/`balance_detail`），见 `WxaExpressBalanceQueryResponse`。

### 60. `PreAddOrderAsync` — 同城配送询价（`POST /cgi-bin/express/intracity/preaddorder`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/preaddorder`

- **请求体**：`WxaExpressIntracityOrderBaseRequest` — 询价请求体（`wx_store_id`/`user_*`/`cargo` 必填），见 `WxaExpressIntracityOrderBaseRequest`。
- **返回**：`WxaExpressPreAddOrderResponse` — 预估费用与服务信息（`service_trans_id`/`distance`/`est_fee`），见 `WxaExpressPreAddOrderResponse`。

**备注（原文）**

- 调用后返回预估费用与服务信息，商家可据此展示给用户；`service_trans_id` 在下单时须传回。

### 61. `AddIntracityOrderAsync` — 同城配送下单（`POST /cgi-bin/express/intracity/addorder`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/addorder`

- **请求体**：`WxaExpressAddIntracityOrderRequest` — 下单请求体（`wx_store_id`/`store_order_id`/`user_*`/`cargo` 必填），见 `WxaExpressAddIntracityOrderRequest`。
- **返回**：`WxaExpressAddIntracityOrderResponse` — 配送单信息（`wx_order_id`/`fee`/`distance`），见 `WxaExpressAddIntracityOrderResponse`。

**备注（原文）**

- `store_order_id` 用于幂等（重复下单返回同一配送单）；回调 `callback_url` 配置后配送状态变化会异步通知。

### 62. `QueryIntracityOrderAsync` — 查询配送单（`POST /cgi-bin/express/intracity/queryorder`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryorder。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/queryorder`

- **请求体**：`WxaExpressQueryIntracityOrderRequest` — 查询请求体（`wx_store_id` 必填，`store_order_id` 与 `wx_order_id` 二选一），见 `WxaExpressQueryIntracityOrderRequest`。
- **返回**：`WxaExpressQueryIntracityOrderResponse` — 配送单全量明细，见 `WxaExpressQueryIntracityOrderResponse`。

### 63. `CancelIntracityOrderAsync` — 取消配送单（`POST /cgi-bin/express/intracity/cancelorder`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/cancelorder`

- **请求体**：`WxaExpressCancelIntracityOrderRequest` — 取消请求体（`cancel_reason_id` 必填），见 `WxaExpressCancelIntracityOrderRequest`。
- **返回**：`WxaExpressCancelIntracityOrderResponse` — 取消结果（`order_status`/`deductfee`），见 `WxaExpressCancelIntracityOrderResponse`。

**备注（原文）**

- `cancel_reason_id`：1001 用户取消，1002 门店取消，1003 骑手取消，1004 其它；骑手已接单后取消可能产生扣款（`deductfee`）。

### 64. `MockNotifyAsync` — 模拟配送状态变更回调（`POST /cgi-bin/express/intracity/mocknotify`，**仅测试环境**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_mocknotify。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/mocknotify`

- **请求体**：`WxaExpressMockNotifyRequest` — 模拟回调请求体（`order_status` 必填），见 `WxaExpressMockNotifyRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 仅测试环境可用：模拟配送单状态变化，触发配置的 `callback_url` 回调，用于商家侧联调。

### 65. `GetCityAsync` — 查询支持同城配送的城市（`POST /cgi-bin/express/intracity/getcity`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getcity。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/getcity`

- **请求体**：`WxaExpressGetCityRequest` — 城市查询请求体（`service_trans_id` 可选，不传返回全部运力），见 `WxaExpressGetCityRequest`。
- **返回**：`WxaExpressGetCityResponse` — 各运力支持的城市列表，见 `WxaExpressGetCityResponse`。

**备注（原文）**

- **业务语义（官方原文）**：用于查询全国可以使用微信同城配送的城市； / 指定 `service_trans_id`（如 `SFTC` 顺丰同城、`DADA` 达达）时只返回该运力结果。 / 常见错误码：`48001`（未开通同城配送权限）、`934003`（运力 ID 错误）、`934009`（不支持该门店所在城市）。

### 66. `GetPayModeAsync` — 查询扣费主体（`POST /cgi-bin/express/intracity/getpaymode`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getpaymode。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/getpaymode`

- **请求体**：`WxaExpressGetPayModeRequest` — 扣费主体查询请求体（`appid` 必填），见 `WxaExpressGetPayModeRequest`。
- **返回**：`WxaExpressGetPayModeResponse` — 扣费主体与对应的扣费 AppId，见 `WxaExpressGetPayModeResponse`。

**备注（原文）**

- **官方原文约束**：`appid` **必须**与当前 `access_token` 匹配，否则返回 `934012`。 / 返回 `pay_appid`（扣费主体为小程序）与 `pay_component_appid`（扣费主体为服务商）**互斥**出现。

### 67. `SetPayModeAsync` — 设置扣费主体（`POST /cgi-bin/express/intracity/setpaymode`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_setpaymode。

- **声明接口**：[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)
- **官方契约**：`POST /cgi-bin/express/intracity/setpaymode`

- **请求体**：`WxaExpressSetPayModeRequest` — 扣费主体设置请求体（`appid` / `pay_mode` 必填），见 `WxaExpressSetPayModeRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示设置成功）。

**备注（原文）**

- `pay_mode` 取值见 `WxaExpressPayModes`（`PAY_MODE_STORE` 门店 / / `PAY_MODE_APP` 小程序 / `PAY_MODE_COMPONENT` 服务商）； / 与 `GetPayModeAsync` 配对使用（后者读、本端点写）。

### 68. `GetAllDeliveryAsync` — 获取已支持的配送公司列表（`POST /cgi-bin/express/local/business/delivery/getall`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getallimmedelivery。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/delivery/getall`
- **返回**：`WxaExpressLocalGetAllDeliveryResponse` — 配送公司列表（`list`：`delivery_id` / `delivery_name`），见 `WxaExpressLocalGetAllDeliveryResponse`。

**备注（原文）**

- 官方契约：**POST**，**无请求体**（官方「请求体 Request Payload」原文为「无」）；Query 携带 `access_token`。

### 69. `OpenDeliveryAsync` — 申请开通即时配送（`POST /cgi-bin/express/local/business/open`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_opendelivery。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/open`
- **返回**：`WxaExpressLocalResponse` — 运力侧结果（`resultcode` / `resultmsg`），见 `WxaExpressLocalResponse`。

**备注（原文）**

- 官方契约：**POST**，**无请求体**；Query 携带 `access_token`。
- **官方约束**：只能由**第三方服务商**调用，为代开发的小程序发起开通；调用成功后小程序管理员收到模板消息完成开通。适用范围为「小程序（仅认证）」。

### 70. `BindAccountAsync` — 发起绑定配送公司账号请求（`POST /cgi-bin/express/local/business/shop/add`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_bindlocalaccount。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/shop/add`

- **请求体**：`WxaExpressLocalBindAccountRequest` — 绑定请求体（`delivery_id` 必填），见 `WxaExpressLocalBindAccountRequest`。
- **返回**：`WxaExpressLocalResponse` — 运力侧结果，见 `WxaExpressLocalResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。官方约束：只能由第三方服务商调用。

### 71. `GetBindAccountAsync` — 拉取已绑定账号（`POST /cgi-bin/express/local/business/shop/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getbindaccount。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/shop/get`
- **返回**：`WxaExpressLocalGetBindAccountResponse` — 已绑定的商家签约账号列表（`shop_list`），见 `WxaExpressLocalGetBindAccountResponse`。

**备注（原文）**

- 官方契约：**POST**，**无请求体**；Query 携带 `access_token`。`audit_result`：`0` 通过 / `1` 审核中 / `2` 不通过。

### 72. `PreAddOrderAsync` — 预下配送单（`POST /cgi-bin/express/local/business/order/pre_add`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_preaddorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/pre_add`

- **请求体**：`WxaExpressLocalAddOrderRequest` — 预下单请求体（与下单同构；`delivery_token` 由本接口产出、下单时回传），见 `WxaExpressLocalAddOrderRequest`。
- **返回**：`WxaExpressLocalPreAddOrderResponse` — 运费预估与 `delivery_token`，见 `WxaExpressLocalPreAddOrderResponse`。

**备注（原文）**

- **运力差异（官方原文）**：达达返回费用 / 距离 / 预计接单时间与 `delivery_token`（有效期 3 分钟）； / 顺丰同城、闪送返回费用 / 距离 / 预计接单时间但**不支持** `delivery_token`； / 美团配送返回 `0` 仅表示校验通过。也可不调用本接口直接下单。

### 73. `AddOrderAsync` — 添加配送单（`POST /cgi-bin/express/local/business/order/add`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addlocalorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/add`

- **请求体**：`WxaExpressLocalAddOrderRequest` — 下单请求体，见 `WxaExpressLocalAddOrderRequest`。
- **返回**：`WxaExpressLocalAddOrderResponse` — 配送单与运费明细，见 `WxaExpressLocalAddOrderResponse`。

**备注（原文）**

- **字段适用性**：`wxa_appid`（`WxaAppId`）**仅本端点生效**；`delivery_token` 取 `PreAddOrderAsync` 的返回。

### 74. `ReAddOrderAsync` — 重新下单（`POST /cgi-bin/express/local/business/order/readd`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_reorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/readd`

- **请求体**：`WxaExpressLocalAddOrderRequest` — 重新下单请求体（与 `AddOrderAsync` 同构），见 `WxaExpressLocalAddOrderRequest`。
- **返回**：`WxaExpressLocalAddOrderResponse` — 配送单与运费明细，见 `WxaExpressLocalAddOrderResponse`。

### 75. `GetOrderAsync` — 拉取配送单信息（`POST /cgi-bin/express/local/business/order/get`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_getlocalorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/get`

- **请求体**：`WxaExpressLocalGetOrderRequest` — 查询请求体（`shopid` / `shop_order_id` / `shop_no` / `delivery_sign` 必填），见 `WxaExpressLocalGetOrderRequest`。
- **返回**：`WxaExpressLocalGetOrderResponse` — 配送单状态与骑手信息，见 `WxaExpressLocalGetOrderResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。`order_status` 枚举见 `WxaExpressLocalOrderStatus`。

### 76. `CancelOrderAsync` — 取消配送单（`POST /cgi-bin/express/local/business/order/cancel`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_cancellocalorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/cancel`

- **请求体**：`WxaExpressLocalCancelOrderRequest` — 取消请求体（`cancel_reason_id` / `cancel_reason` 必填），见 `WxaExpressLocalCancelOrderRequest`。
- **返回**：`WxaExpressLocalCancelOrderResponse` — 扣除的违约金与说明，见 `WxaExpressLocalCancelOrderResponse`。

**备注（原文）**

- **各家取消规则（官方原文）**：顺丰同城急送 / 闪送 / 美团配送——配送完成前任意节点可取消；达达——骑手取货之前可取消。可先用 `PreCancelOrderAsync` 预检违约金。

### 77. `PreCancelOrderAsync` — 预取消配送单（`POST /cgi-bin/express/local/business/order/precancel`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_precancelorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/precancel`

- **请求体**：`WxaExpressLocalCancelOrderRequest` — 预取消请求体（`cancel_reason_id` / `cancel_reason` 可选），见 `WxaExpressLocalCancelOrderRequest`。
- **返回**：`WxaExpressLocalCancelOrderResponse` — 预计扣除的违约金与说明，见 `WxaExpressLocalCancelOrderResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。官方场景：正式取消前先查询该订单是否可取消及需扣费用。

### 78. `ConfirmReturnAsync` — 异常件退回商家确认（`POST /cgi-bin/express/local/business/order/confirm_return`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_abnormalconfirm。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/confirm_return`

- **请求体**：`WxaExpressLocalConfirmReturnRequest` — 确认请求体（`shopid` / `shop_order_id` / `waybill_id` / `delivery_sign` / `shop_no` 必填），见 `WxaExpressLocalConfirmReturnRequest`。
- **返回**：`WxaExpressLocalResponse` — 运力侧结果，见 `WxaExpressLocalResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。官方场景：配送异常、骑手把货物退还商家，商家收货后调用本接口确认。

### 79. `AddTipsAsync` — 添加小费（`POST /cgi-bin/express/local/business/order/addtips`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_addtips。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/order/addtips`

- **请求体**：`WxaExpressLocalAddTipsRequest` — 加小费请求体（`tips` / `remark` 必填），见 `WxaExpressLocalAddTipsRequest`。
- **返回**：`WxaExpressLocalResponse` — 运力侧结果，见 `WxaExpressLocalResponse`。

**备注（原文）**

- **官方约束**：仅可对**待接单状态**订单加小费；小费以**最新一次**为准 ⇒ 下次金额必须**大于**上一次； / 美团配送不支持；闪送须按固定档位（2/3/5/10/15/20/50/100）。

### 80. `MockUpdateOrderAsync` — 沙盒环境模拟配送公司更新配送单状态（`POST /cgi-bin/express/local/business/test_update_order`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_mockupdateorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/test_update_order`

- **请求体**：`WxaExpressLocalMockUpdateOrderRequest` — 模拟请求体（`shopid` 必须为 `test_shop_id`；**不传** `delivery_sign`），见 `WxaExpressLocalMockUpdateOrderRequest`。
- **返回**：`WxaExpressLocalResponse` — 运力侧结果，见 `WxaExpressLocalResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。**官方原文**：只用于沙盒环境，订单**不会**真实流转到运力方。

### 81. `RealMockUpdateOrderAsync` — 测试环境模拟更新配送单状态（`POST /cgi-bin/express/local/business/realmock_update_order`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-business/api_realmockupdateorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/business/realmock_update_order`

- **请求体**：`WxaExpressLocalMockUpdateOrderRequest` — 模拟请求体（`delivery_sign` **必填**），见 `WxaExpressLocalMockUpdateOrderRequest`。
- **返回**：`WxaExpressLocalResponse` — 运力侧结果，见 `WxaExpressLocalResponse`。

**备注（原文）**

- **官方约束**：只能用于测试，请求会转发到**运力测试环境**，目前仅支持顺丰同城与达达（须用官方提供的测试账号）。

### 82. `UpdateOrderStatusAsync` — 运力方更新配送单状态（`POST /cgi-bin/express/local/delivery/update_order`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/immediate-delivery/deliver-by-provider/api_updateorder。

- **声明接口**：[IWxaExpressLocalService](./IWxaExpressLocalService.cs)
- **官方契约**：`POST /cgi-bin/express/local/delivery/update_order`

- **请求体**：`WxaExpressLocalUpdateOrderRequest` — 状态更新请求体（`wx_token` / `order_status` / `waybill_id` / `action_time` / `shopid` / `shop_order_id` / `wxa_path` 必填），见 `WxaExpressLocalUpdateOrderRequest`。
- **返回**：`WxaExpressLocalResponse` — 运力侧结果，见 `WxaExpressLocalResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；本端点**不支持**第三方平台代调用。
- **`wx_token` 语义（勿填错）**：官方原文为「下单事件中推送的 token 字段」， / 与物流助手散单寄件的 `token` 同源，**不是**应用级令牌。

### 手工通道端点（非 `[HttpClientApi]`）

以下接口不带 HTTP 特性，路由与参数取自方法 XML 注释，调用经手工原始响应通道（非生成管线）。

#### M1. `GetBillAsync` — 拉取商家寄件对账文件（`POST /cgi-bin/express/delivery/single_waybill/get_bill`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scattergetbill。

- **声明接口**：[IWxaExpressBillService](./IWxaExpressBillService.cs)
- **官方契约**：`POST /cgi-bin/express/delivery/single_waybill/get_bill`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `request` | Body | `WxaExpressSingleWaybillGetBillRequest` | 对账文件请求体（`date`/`type` 必填），见 `WxaExpressSingleWaybillGetBillRequest`。 |
- **返回**：`string` — 对账文件文本内容（成功时）；失败时抛 `WxaException`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；成功响应为文本文件（非 JSON）。
- 对账类型 `type`：ALL 全部 / SUCCESS 成功订单 / REFUND 退款订单。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/Express`](../../../Mud.Wechat.MiniProgram/Express/) 内共 **165** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **165** 个，`SerializerClassName` = `Express`。

AOT 源生成上下文：[`ExpressJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/ExpressJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaExpressAddOrderResponse` | 应答 | [Express/Order/WxaExpressAddOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Order/WxaExpressAddOrderResponse.cs) | `delivery_resultcode`, `delivery_resultmsg`, `order_id`, `waybill_id`, `waybill_data` |
| `WxaExpressAddOrderRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressAddOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressAddOrderRequest.cs) | `order_id`, `openid`, `delivery_id`, `biz_id`, `custom_remark`, `tagid`, `add_source`, `wx_appid`, `sender`, `receiver`, `cargo`, `shop` … |
| `WxaExpressGetOrderResponse` | 应答 | [Express/Order/WxaExpressGetOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Order/WxaExpressGetOrderResponse.cs) | `order_id`, `delivery_id`, `waybill_id`, `waybill_data`, `print_html`, `order_status` |
| `WxaExpressGetOrderRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressGetOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressGetOrderRequest.cs) | `order_id`, `openid`, `delivery_id`, `waybill_id`, `print_type` |
| `WxaExpressBatchGetOrderResponse` | 应答 | [Express/Order/WxaExpressBatchGetOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Order/WxaExpressBatchGetOrderResponse.cs) | `order_list` |
| `WxaExpressBatchGetOrderRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressBatchGetOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressBatchGetOrderRequest.cs) | `order_list` |
| `WxaExpressCancelOrderResponse` | 应答 | [Express/Order/WxaExpressCancelOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Order/WxaExpressCancelOrderResponse.cs) | `delivery_resultcode`, `delivery_resultmsg` |
| `WxaExpressCancelOrderRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressCancelOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressCancelOrderRequest.cs) | `order_id`, `openid`, `delivery_id`, `waybill_id` |
| `WxaExpressGetPathResponse` | 应答 | [Express/Path/WxaExpressGetPathResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Path/WxaExpressGetPathResponse.cs) | `openid`, `delivery_id`, `waybill_id`, `path_item_num`, `path_item_list` |
| `WxaExpressGetPathRequest` | 请求体 | [Express/Path/RequestModel/WxaExpressGetPathRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Path/RequestModel/WxaExpressGetPathRequest.cs) | `openid`, `delivery_id`, `waybill_id` |
| `WxaExpressGetAllDeliveryResponse` | 应答 | [Express/Account/WxaExpressGetAllDeliveryResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Account/WxaExpressGetAllDeliveryResponse.cs) | `count`, `data` |
| `WxaExpressGetAllAccountResponse` | 应答 | [Express/Account/WxaExpressGetAllAccountResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Account/WxaExpressGetAllAccountResponse.cs) | `count`, `list` |
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaExpressBindAccountRequest` | 请求体 | [Express/Account/RequestModel/WxaExpressBindAccountRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Account/RequestModel/WxaExpressBindAccountRequest.cs) | `type`, `biz_id`, `delivery_id`, `password`, `remark_content` |
| `WxaExpressGetPrinterResponse` | 应答 | [Express/Common/WxaExpressGetPrinterResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Common/WxaExpressGetPrinterResponse.cs) | `count`, `openid`, `tagid_list` |
| `WxaExpressUpdatePrinterRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressUpdatePrinterRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressUpdatePrinterRequest.cs) | `update_type`, `openid`, `tagid_list` |
| `WxaExpressGetQuotaResponse` | 应答 | [Express/Common/WxaExpressGetQuotaResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Common/WxaExpressGetQuotaResponse.cs) | `quota_num` |
| `WxaExpressGetQuotaRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressGetQuotaRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressGetQuotaRequest.cs) | `delivery_id`, `biz_id` |
| `WxaExpressTestUpdateOrderRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressTestUpdateOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressTestUpdateOrderRequest.cs) | `biz_id`, `order_id`, `delivery_id`, `waybill_id`, `action_time`, `action_type`, `action_msg` |
| `WxaExpressGetContactResponse` | 应答 | [Express/Contact/WxaExpressGetContactResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Contact/WxaExpressGetContactResponse.cs) | `waybill_id`, `sender`, `receiver` |
| `WxaExpressGetContactRequest` | 请求体 | [Express/Contact/RequestModel/WxaExpressGetContactRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Contact/RequestModel/WxaExpressGetContactRequest.cs) | `token`, `waybill_id` |
| `WxaExpressPreviewTemplateResponse` | 应答 | [Express/Template/WxaExpressPreviewTemplateResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Template/WxaExpressPreviewTemplateResponse.cs) | `waybill_id`, `rendered_waybill_template` |
| `WxaExpressPreviewTemplateRequest` | 请求体 | [Express/Template/RequestModel/WxaExpressPreviewTemplateRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Template/RequestModel/WxaExpressPreviewTemplateRequest.cs) | `waybill_id`, `waybill_template`, `waybill_data`, `custom` |
| `WxaExpressUpdateBusinessRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressUpdateBusinessRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressUpdateBusinessRequest.cs) | `shop_app_id`, `biz_id`, `result_code`, `result_msg` |
| `WxaExpressUpdatePathRequest` | 请求体 | [Express/Path/RequestModel/WxaExpressUpdatePathRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Path/RequestModel/WxaExpressUpdatePathRequest.cs) | `token`, `waybill_id`, `action_time`, `action_type`, `action_msg` |
| `WxaExpressAddReturnIdResponse` | 应答 | [Express/Return/WxaExpressAddReturnIdResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Return/WxaExpressAddReturnIdResponse.cs) | `return_id` |
| `WxaExpressAddReturnIdRequest` | 请求体 | [Express/Return/RequestModel/WxaExpressAddReturnIdRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Return/RequestModel/WxaExpressAddReturnIdRequest.cs) | `shop_order_id`, `biz_addr`, `user_addr`, `openid`, `order_path`, `goods_list`, `order_price`, `wx_pay_id` |
| `WxaExpressReturnStatusResponse` | 应答 | [Express/Return/WxaExpressReturnStatusResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Return/WxaExpressReturnStatusResponse.cs) | `status`, `waybill_id`, `order_status`, `delivery_id`, `delivery_name` |
| `WxaExpressGetReturnIdRequest` | 请求体 | [Express/Return/RequestModel/WxaExpressGetReturnIdRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Return/RequestModel/WxaExpressGetReturnIdRequest.cs) | `return_id` |
| `WxaExpressUnbindReturnIdRequest` | 请求体 | [Express/Return/RequestModel/WxaExpressUnbindReturnIdRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Return/RequestModel/WxaExpressUnbindReturnIdRequest.cs) | `return_id` |
| `WxaExpressFollowWaybillResponse` | 应答 | [Express/Path/WxaExpressFollowWaybillResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Path/WxaExpressFollowWaybillResponse.cs) | `waybill_token` |
| `WxaExpressFollowWaybillRequest` | 请求体 | [Express/Path/RequestModel/WxaExpressFollowWaybillRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Path/RequestModel/WxaExpressFollowWaybillRequest.cs) | `openid`, `waybill_id`, `sender_phone`, `receiver_phone`, `goods_info`, `trans_id`, `order_detail_path`, `delivery_id` |
| `WxaExpressTraceResponse` | 应答 | [Express/Trace/WxaExpressTraceResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Trace/WxaExpressTraceResponse.cs) | `waybill_info`, `shop_info`, `delivery_info` |
| `WxaExpressQueryFollowTraceRequest` | 请求体 | [Express/Trace/RequestModel/WxaExpressQueryFollowTraceRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Trace/RequestModel/WxaExpressQueryFollowTraceRequest.cs) | `waybill_token`, `openid` |
| `WxaExpressUpdateFollowWaybillGoodsRequest` | 请求体 | [Express/Path/RequestModel/WxaExpressUpdateFollowWaybillGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Path/RequestModel/WxaExpressUpdateFollowWaybillGoodsRequest.cs) | `waybill_token`, `openid`, `goods_info` |
| `WxaExpressTraceWaybillResponse` | 应答 | [Express/Trace/WxaExpressTraceWaybillResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Trace/WxaExpressTraceWaybillResponse.cs) | `waybill_token` |
| `WxaExpressTraceWaybillRequest` | 请求体 | [Express/Trace/RequestModel/WxaExpressTraceWaybillRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Trace/RequestModel/WxaExpressTraceWaybillRequest.cs) | `openid`, `waybill_id`, `sender_phone`, `receiver_phone`, `goods_info`, `delivery_id`, `trans_id`, `order_detail_path` |
| `WxaExpressQueryTraceRequest` | 请求体 | [Express/Trace/RequestModel/WxaExpressQueryTraceRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Trace/RequestModel/WxaExpressQueryTraceRequest.cs) | `waybill_token`, `openid` |
| `WxaExpressUpdateWaybillGoodsRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressUpdateWaybillGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressUpdateWaybillGoodsRequest.cs) | `waybill_token`, `openid`, `goods_info` |
| `WxaExpressGetDeliveryListResponse` | 应答 | [Express/Account/WxaExpressGetDeliveryListResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Account/WxaExpressGetDeliveryListResponse.cs) | `count`, `delivery_list` |
| `WxaExpressAddReturnResponse` | 应答 | [Express/Return/WxaExpressAddReturnResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Return/WxaExpressAddReturnResponse.cs) | `return_id` |
| `WxaExpressAddReturnRequest` | 请求体 | [Express/Return/RequestModel/WxaExpressAddReturnRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Return/RequestModel/WxaExpressAddReturnRequest.cs) | `shop_order_id`, `biz_addr`, `user_addr`, `openid`, `order_path`, `goods_list`, `order_price` |
| `WxaExpressGetReturnRequest` | 请求体 | [Express/Return/RequestModel/WxaExpressGetReturnRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Return/RequestModel/WxaExpressGetReturnRequest.cs) | `return_id` |
| `WxaExpressUnbindReturnRequest` | 请求体 | [Express/Return/RequestModel/WxaExpressUnbindReturnRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Return/RequestModel/WxaExpressUnbindReturnRequest.cs) | `return_id` |
| `WxaExpressSingleWaybillUpdateRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressSingleWaybillUpdateRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressSingleWaybillUpdateRequest.cs) | `token`, `waybill_id`, `action_time`, `action_type`, `action_msg`, `pickup_courier_name`, `pickup_courier_phone`, `delivery_courier_name`, `delivery_courier_phone` |
| `WxaExpressSingleWaybillFeeRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressSingleWaybillFeeRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressSingleWaybillFeeRequest.cs) | `token`, `waybill_id`, `need_pay`, `fee`, `original_fee`, `base_fee`, `insured_fee`, `other_fee`, `remark`, `pay_goods_name` |
| `WxaExpressSingleWaybillCancelRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressSingleWaybillCancelRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressSingleWaybillCancelRequest.cs) | `token`, `reason` |
| `WxaExpressSingleWaybillRefundRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressSingleWaybillRefundRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressSingleWaybillRefundRequest.cs) | `token`, `fee` |
| `WxaExpressSingleWaybillEvaluateFeedbackRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressSingleWaybillEvaluateFeedbackRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressSingleWaybillEvaluateFeedbackRequest.cs) | `token`, `waybill_id`, `result`, `desc` |
| `WxaExpressUpdateComplaintResultRequest` | 请求体 | [Express/Complaint/RequestModel/WxaExpressUpdateComplaintResultRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Complaint/RequestModel/WxaExpressUpdateComplaintResultRequest.cs) | `token`, `waybill_id`, `result`, `desc` |
| `WxaExpressPathNotifyResponse` | 应答 | [Express/Path/WxaExpressPathNotifyResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Path/WxaExpressPathNotifyResponse.cs) | `exist` |
| `WxaExpressPathNotifyRequest` | 请求体 | [Express/Path/RequestModel/WxaExpressPathNotifyRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Path/RequestModel/WxaExpressPathNotifyRequest.cs) | `sender`, `receiver`, `waybill_id`, `path`, `create_time` |
| `WxaExpressUserQueryResponse` | 应答 | [Express/Common/WxaExpressUserQueryResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Common/WxaExpressUserQueryResponse.cs) | `exist` |
| `WxaExpressUserQueryRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressUserQueryRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressUserQueryRequest.cs) | `phone` |
| `WxaExpressQueryInsuranceOpenResponse` | 应答 | [Express/Insurance/WxaExpressQueryInsuranceOpenResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Insurance/WxaExpressQueryInsuranceOpenResponse.cs) | `is_open` |
| `WxaExpressCreateInsuranceOrderResponse` | 应答 | [Express/Insurance/WxaExpressCreateInsuranceOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Insurance/WxaExpressCreateInsuranceOrderResponse.cs) | `policy_no`, `insurance_end_date`, `estimate_amount`, `premium` |
| `WxaExpressCreateInsuranceOrderRequest` | 请求体 | [Express/Insurance/RequestModel/WxaExpressCreateInsuranceOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Insurance/RequestModel/WxaExpressCreateInsuranceOrderRequest.cs) | `openid`, `order_no`, `pay_time`, `pay_amount`, `delivery_no`, `delivery_place`, `receipt_place`, `product_info` |
| `WxaExpressClaimResponse` | 应答 | [Express/Common/WxaExpressClaimResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Common/WxaExpressClaimResponse.cs) | `report_no`, `is_home_pick_up` |
| `WxaExpressClaimRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressClaimRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressClaimRequest.cs) | `openid`, `order_no`, `refund_delivery_no`, `refund_company` |
| `WxaExpressCreateChargeIdResponse` | 应答 | [Express/Pay/WxaExpressCreateChargeIdResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressCreateChargeIdResponse.cs) | `order_id` |
| `WxaExpressCreateChargeIdRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressCreateChargeIdRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressCreateChargeIdRequest.cs) | `quota` |
| `WxaExpressApplyPayResponse` | 应答 | [Express/Pay/WxaExpressApplyPayResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressApplyPayResponse.cs) | `pay_url` |
| `WxaExpressApplyPayRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressApplyPayRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressApplyPayRequest.cs) | `order_id` |
| `WxaExpressGetPayOrderListResponse` | 应答 | [Express/Pay/WxaExpressGetPayOrderListResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressGetPayOrderListResponse.cs) | `total`, `list` |
| `WxaExpressGetPayOrderListRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressGetPayOrderListRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressGetPayOrderListRequest.cs) | `status_list`, `offset`, `limit` |
| `WxaExpressGetSummaryResponse` | 应答 | [Express/Common/WxaExpressGetSummaryResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Common/WxaExpressGetSummaryResponse.cs) | `total`, `claim_num`, `claim_succ_num`, `premium`, `funds`, `need_close` |
| `WxaExpressGetSummaryRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressGetSummaryRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressGetSummaryRequest.cs) | `begin_time`, `end_time` |
| `WxaExpressGetOrderListResponse` | 应答 | [Express/Order/WxaExpressGetOrderListResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Order/WxaExpressGetOrderListResponse.cs) | `total`, `list` |
| `WxaExpressGetOrderListRequest` | 请求体 | [Express/Order/RequestModel/WxaExpressGetOrderListRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Order/RequestModel/WxaExpressGetOrderListRequest.cs) | `openid`, `order_no`, `policy_no`, `report_no`, `delivery_no`, `refund_delivery_no`, `begin_time`, `end_time`, `status_list`, `offset`, `limit`, `sort_direct` |
| `WxaExpressUpdateNotifyFundsRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressUpdateNotifyFundsRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressUpdateNotifyFundsRequest.cs) | `notify_funds` |
| `WxaExpressCreateStoreResponse` | 应答 | [Express/Account/WxaExpressCreateStoreResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Account/WxaExpressCreateStoreResponse.cs) | `out_store_id`, `wx_store_id`, `appid` |
| `WxaExpressCreateStoreRequest` | 请求体 | [Express/Account/RequestModel/WxaExpressCreateStoreRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Account/RequestModel/WxaExpressCreateStoreRequest.cs) | `out_store_id`, `store_name`, `order_pattern`, `service_trans_prefer`, `address_info` |
| `WxaExpressQueryStoreResponse` | 应答 | [Express/Account/WxaExpressQueryStoreResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Account/WxaExpressQueryStoreResponse.cs) | `store_list`, `total` |
| `WxaExpressQueryStoreRequest` | 请求体 | [Express/Account/RequestModel/WxaExpressQueryStoreRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Account/RequestModel/WxaExpressQueryStoreRequest.cs) | `out_store_id`, `wx_store_id` |
| `WxaExpressUpdateStoreResponse` | 应答 | [Express/Account/WxaExpressUpdateStoreResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Account/WxaExpressUpdateStoreResponse.cs) | `out_store_id`, `wx_store_id`, `appid` |
| `WxaExpressUpdateStoreRequest` | 请求体 | [Express/Account/RequestModel/WxaExpressUpdateStoreRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Account/RequestModel/WxaExpressUpdateStoreRequest.cs) | `keys`, `content` |
| `WxaExpressStoreChargeResponse` | 应答 | [Express/Pay/WxaExpressStoreChargeResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressStoreChargeResponse.cs) | `payurl`, `wx_store_id`, `appid` |
| `WxaExpressStoreChargeRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressStoreChargeRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressStoreChargeRequest.cs) | `wx_store_id`, `service_trans_id`, `amount` |
| `WxaExpressStoreRefundResponse` | 应答 | [Express/Pay/WxaExpressStoreRefundResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressStoreRefundResponse.cs) | `wx_store_id`, `appid`, `refund_amount` |
| `WxaExpressStoreRefundRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressStoreRefundRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressStoreRefundRequest.cs) | `wx_store_id`, `service_trans_id` |
| `WxaExpressQueryFlowResponse` | 应答 | [Express/Common/WxaExpressQueryFlowResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Common/WxaExpressQueryFlowResponse.cs) | `flow_list`, `total` |
| `WxaExpressQueryFlowRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressQueryFlowRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressQueryFlowRequest.cs) | `wx_store_id`, `flow_type`, `service_trans_id`, `begin_time`, `end_time` |
| `WxaExpressBalanceQueryResponse` | 应答 | [Express/Pay/WxaExpressBalanceQueryResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressBalanceQueryResponse.cs) | `wx_store_id`, `appid`, `all_balance`, `balance_detail` |
| `WxaExpressBalanceQueryRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressBalanceQueryRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressBalanceQueryRequest.cs) | `wx_store_id`, `service_trans_id` |
| `WxaExpressPreAddOrderResponse` | 应答 | [Express/Order/WxaExpressPreAddOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Order/WxaExpressPreAddOrderResponse.cs) | `service_trans_id`, `distance`, `est_fee`, `expected_finished_time`, `promise_delivery_time` |
| `WxaExpressIntracityOrderBaseRequest` | 请求体 | [Express/Intracity/WxaExpressIntracityOrderBaseRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/WxaExpressIntracityOrderBaseRequest.cs) | `wx_store_id`, `user_name`, `user_phone`, `user_lng`, `user_lat`, `user_address`, `cargo`, `use_sandbox` |
| `WxaExpressAddIntracityOrderResponse` | 应答 | [Express/Intracity/WxaExpressAddIntracityOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/WxaExpressAddIntracityOrderResponse.cs) | `wx_store_id`, `store_order_id`, `wx_order_id`, `service_trans_id`, `trans_order_id`, `waybill_id`, `distance`, `fee`, `fetch_code`, `order_seq` |
| `WxaExpressAddIntracityOrderRequest` | 请求体 | [Express/Intracity/RequestModel/WxaExpressAddIntracityOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/RequestModel/WxaExpressAddIntracityOrderRequest.cs) | `store_order_id`, `user_openid`, `order_seq`, `verify_code_type`, `order_detail_path`, `callback_url`, `item_list` |
| `WxaExpressQueryIntracityOrderResponse` | 应答 | [Express/Intracity/WxaExpressQueryIntracityOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/WxaExpressQueryIntracityOrderResponse.cs) | `wx_store_id`, `wx_order_id`, `store_order_id`, `appid`, `order_status`, `user_openid`, `service_trans_id`, `delivery_no`, `distance`, `actualfee`, `deductfee`, `create_time` … |
| `WxaExpressQueryIntracityOrderRequest` | 请求体 | [Express/Intracity/RequestModel/WxaExpressQueryIntracityOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/RequestModel/WxaExpressQueryIntracityOrderRequest.cs) | `wx_store_id`, `store_order_id`, `wx_order_id` |
| `WxaExpressCancelIntracityOrderResponse` | 应答 | [Express/Intracity/WxaExpressCancelIntracityOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/WxaExpressCancelIntracityOrderResponse.cs) | `wx_store_id`, `wx_order_id`, `store_order_id`, `appid`, `order_status`, `deductfee` |
| `WxaExpressCancelIntracityOrderRequest` | 请求体 | [Express/Intracity/RequestModel/WxaExpressCancelIntracityOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Intracity/RequestModel/WxaExpressCancelIntracityOrderRequest.cs) | `wx_store_id`, `store_order_id`, `wx_order_id`, `cancel_reason_id`, `cancel_reason` |
| `WxaExpressMockNotifyRequest` | 请求体 | [Express/Common/RequestModel/WxaExpressMockNotifyRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Common/RequestModel/WxaExpressMockNotifyRequest.cs) | `wx_store_id`, `store_order_id`, `wx_order_id`, `order_status` |
| `WxaExpressGetCityResponse` | 应答 | [Express/City/WxaExpressGetCityResponse.cs](../../../Mud.Wechat.MiniProgram/Express/City/WxaExpressGetCityResponse.cs) | `support_list` |
| `WxaExpressGetCityRequest` | 请求体 | [Express/City/RequestModel/WxaExpressGetCityRequest.cs](../../../Mud.Wechat.MiniProgram/Express/City/RequestModel/WxaExpressGetCityRequest.cs) | `service_trans_id` |
| `WxaExpressGetPayModeResponse` | 应答 | [Express/Pay/WxaExpressGetPayModeResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/WxaExpressGetPayModeResponse.cs) | `pay_mode`, `pay_appid`, `pay_component_appid` |
| `WxaExpressGetPayModeRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressGetPayModeRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressGetPayModeRequest.cs) | `appid` |
| `WxaExpressSetPayModeRequest` | 请求体 | [Express/Pay/RequestModel/WxaExpressSetPayModeRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Pay/RequestModel/WxaExpressSetPayModeRequest.cs) | `appid`, `pay_mode` |
| `WxaExpressLocalGetAllDeliveryResponse` | 应答 | [Express/Local/WxaExpressLocalGetAllDeliveryResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalGetAllDeliveryResponse.cs) | `list` |
| `WxaExpressLocalResponse` | 应答 | [Express/Local/WxaExpressLocalResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalResponse.cs) | `resultcode`, `resultmsg` |
| `WxaExpressLocalBindAccountRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalBindAccountRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalBindAccountRequest.cs) | `delivery_id` |
| `WxaExpressLocalGetBindAccountResponse` | 应答 | [Express/Local/WxaExpressLocalGetBindAccountResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalGetBindAccountResponse.cs) | `shop_list` |
| `WxaExpressLocalPreAddOrderResponse` | 应答 | [Express/Local/WxaExpressLocalPreAddOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalPreAddOrderResponse.cs) | `fee`, `deliverfee`, `couponFee`, `tips`, `insurancefee`, `distance`, `delivery_token`, `dispatch_duration` |
| `WxaExpressLocalAddOrderRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalAddOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalAddOrderRequest.cs) | `shopid`, `shop_order_id`, `delivery_id`, `openid`, `sender`, `receiver`, `cargo`, `order_info`, `shop`, `delivery_token`, `delivery_sign`, `shop_no` … |
| `WxaExpressLocalAddOrderResponse` | 应答 | [Express/Local/WxaExpressLocalAddOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalAddOrderResponse.cs) | `fee`, `deliverfee`, `couponfee`, `tips`, `insurancfee`, `distance`, `waybill_id`, `order_status`, `finish_code`, `pickup_code`, `dispatch_duration` |
| `WxaExpressLocalGetOrderResponse` | 应答 | [Express/Local/WxaExpressLocalGetOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalGetOrderResponse.cs) | `order_status`, `waybill_id`, `rider_name`, `rider_phone`, `rider_lng`, `rider_lat`, `reach_time` |
| `WxaExpressLocalGetOrderRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalGetOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalGetOrderRequest.cs) | `shopid`, `shop_order_id`, `shop_no`, `delivery_sign` |
| `WxaExpressLocalCancelOrderResponse` | 应答 | [Express/Local/WxaExpressLocalCancelOrderResponse.cs](../../../Mud.Wechat.MiniProgram/Express/Local/WxaExpressLocalCancelOrderResponse.cs) | `deduct_fee`, `desc` |
| `WxaExpressLocalCancelOrderRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalCancelOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalCancelOrderRequest.cs) | `shopid`, `shop_order_id`, `delivery_id`, `waybill_id`, `cancel_reason_id`, `cancel_reason`, `shop_no`, `delivery_sign` |
| `WxaExpressLocalConfirmReturnRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalConfirmReturnRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalConfirmReturnRequest.cs) | `shopid`, `shop_order_id`, `waybill_id`, `delivery_sign`, `shop_no`, `remark` |
| `WxaExpressLocalAddTipsRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalAddTipsRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalAddTipsRequest.cs) | `shopid`, `shop_order_id`, `waybill_id`, `tips`, `remark`, `delivery_sign`, `shop_no` |
| `WxaExpressLocalMockUpdateOrderRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalMockUpdateOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalMockUpdateOrderRequest.cs) | `shopid`, `shop_order_id`, `order_status`, `action_time`, `action_msg`, `delivery_sign` |
| `WxaExpressLocalUpdateOrderRequest` | 请求体 | [Express/Local/RequestModel/WxaExpressLocalUpdateOrderRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Local/RequestModel/WxaExpressLocalUpdateOrderRequest.cs) | `wx_token`, `order_status`, `waybill_id`, `action_msg`, `action_time`, `agent`, `shopid`, `shop_order_id`, `shop_no`, `wxa_path`, `expected_delivery_time` |
| `WxaExpressSingleWaybillGetBillRequest` | 请求体 | [Express/Waybill/RequestModel/WxaExpressSingleWaybillGetBillRequest.cs](../../../Mud.Wechat.MiniProgram/Express/Waybill/RequestModel/WxaExpressSingleWaybillGetBillRequest.cs) | `date`, `type` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddExpressApi());

var service = app.Services.GetRequiredService<IWxaExpressIntracityService>();
var ct = CancellationToken.None;
var response = await service.ApplyIntracityAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/cgi-bin/express/intracity/apply");
// 占位值仅用于编译示意；业务值须按 IWxaExpressIntracityService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyIntracityAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IWxaExpressDeliveryService`：注释 29 / 实计 28 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaExpressBillService](./IWxaExpressBillService.cs)、[IWxaExpressBusinessService](./IWxaExpressBusinessService.cs)、[IWxaExpressDeliveryService](./IWxaExpressDeliveryService.cs)、[IWxaExpressInsuranceFreightService](./IWxaExpressInsuranceFreightService.cs)、[IWxaExpressIntracityService](./IWxaExpressIntracityService.cs)、[IWxaExpressLocalService](./IWxaExpressLocalService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/Express`](../../../Mud.Wechat.MiniProgram/Express/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
