# Express 域 · 物流助手 / 微信物流服务 / 即时配送

微信小程序物流大域：**83 支端点**，由**六个接口**分面承载，但**共用同一注册组 `Express`**：

| 接口 | 子族 | 支数 |
|---|---|---|
| [IWxaExpressBusinessService.cs](./IWxaExpressBusinessService.cs) | 物流助手 · 商家寄件 | **12** |
| [IWxaExpressDeliveryService.cs](./IWxaExpressDeliveryService.cs) | 服务商侧 / 物流消息 / 退货组件 / 散单寄件 / 消息推送 | **28** |
| [IWxaExpressIntracityService.cs](./IWxaExpressIntracityService.cs) | 同城配送 | **16** |
| [IWxaExpressInsuranceFreightService.cs](./IWxaExpressInsuranceFreightService.cs) | 微信物流服务 · 无忧退货 | **11** |
| [IWxaExpressLocalService.cs](./IWxaExpressLocalService.cs) | 即时配送 | **15** |
| [IWxaExpressBillService.cs](./IWxaExpressBillService.cs) | 商家寄件对账文件（**手工文本通道**） | **1** |

`12 + 28 + 16 + 11 + 15 + 1 = 83`，与守卫 MP-X5 的计数注释逐子族一致
（`ExpressBusiness 12 + ExpressDelivery 28 + ExpressIntracity 16 + ExpressInsuranceFreight 11 +
ExpressLocal 15 + ExpressBill 1`）。DI 组方法 [`AddExpressApi()`](../../Extensions/MiniProgramServiceBuilder.cs)
（`MiniProgramModule.Express`）。该模块的注册委托做**两件事**：
`s.AddExpressWebApiHttpClient()`（**五个声明式接口 82 支**随生成管线一并装载）
+ `s.TryAddSingleton<IWxaExpressBillService, WxaExpressBillService>()`
（手工通道**不进生成管线**，故单独注册）⇒ 一个模块、六个接口、两种装载路径。
接口 XML 记本域核验日为 **2026-10-11 / 2026-10-12 逐页核验**；本 README 的复核日为 **2026-10-18**。

> **事实来源限定**：本文所有断言只来自本仓的接口文件、契约守卫与 DTO 声明，
> 不引用任何外部文档原文。凡本仓核不到的（部分子族频率数字、错误码全集、各运力公司差异细则），
> 一律集中列在文末「已知留档缺口」并标 **未逐项核验**，不做推测。
> **本域无独立守卫文件**，权威断言落在
> [MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)（MP-X5 六子族逐字路由 + 全线合计 398）、
> [MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)（P3-R2）、
> [MiniProgramNonContractRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramNonContractRouteContractGuards.cs)（NC*）与
> [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)（MP-X2/X4/X6/X1），详见「事实来源」。

## 端点速览

### 令牌列总说明（先看这一段）

本域**不是「一支接口一个令牌」这么简单**，三形态并存：

| 形态 | 覆盖 | 说明 |
|---|---|---|
| **应用级 `access_token`（Query）** | 五个声明式接口的全部 82 支 | 复用 `MpTokenTypes.AccessToken`（值 `"Wechat.Mp.AccessToken"`），`TokenInjectionMode.Query`、`Name = "access_token"`（守卫 MP-X2 / MP-X3） |
| **商户侧事件推送的 `token`** | `single_waybill/*` 五支 + `scatter/update_complaint_result` | 请求体字段，**不得**用应用级令牌替代（服务商代商户调用面） |
| **运力方推送的 `wx_token`** | `/cgi-bin/express/local/delivery/update_order` | 官方原文「下单事件中推送的 token 字段」，**勿填成 `access_token`** |
| **`delivery_sign`** | 即时配送多支必填 | `appSecret` 加密校验串 ⇒ **属凭据**，禁入日志 / 遥测 / 异常消息（MP-X7 同口径） |

下面六张子表的「令牌」列因此只标**主形态 + 例外标记**。

### 1) 物流助手 · 商家寄件（`IWxaExpressBusinessService`，12 支）

| # | 方法 | 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddOrderAsync` | POST | `/cgi-bin/express/business/order/add` | `access_token`(Query) | 须已绑定快递账号且开通下单权限 |
| 2 | `GetOrderAsync` | POST | `/cgi-bin/express/business/order/get` | `access_token`(Query) | 单号查询 |
| 3 | `BatchGetOrderAsync` | POST | `/cgi-bin/express/business/order/batchget` | `access_token`(Query) | **单次最多 50 单**（体 `order_list`） |
| 4 | `CancelOrderAsync` | POST | `/cgi-bin/express/business/order/cancel` | `access_token`(Query) | **只能取消待揽件运单** |
| 5 | `GetPathAsync` | POST | `/cgi-bin/express/business/path/get` | `access_token`(Query) | 轨迹查询 |
| 6 | `GetAllDeliveryAsync` | **GET** | `/cgi-bin/express/business/delivery/getall` | `access_token`(Query) | **无请求体** |
| 7 | `GetAllAccountAsync` | **GET** | `/cgi-bin/express/business/account/getall` | `access_token`(Query) | **无请求体** |
| 8 | `BindAccountAsync` | POST | `/cgi-bin/express/business/account/bind` | `access_token`(Query) | `type=bind/unbind`；**绑定为异步**，返回仅表示受理；`password` 敏感 |
| 9 | `GetAllPrinterAsync` | **GET** | `/cgi-bin/express/business/printer/getall` | `access_token`(Query) | **无请求体** |
| 10 | `UpdatePrinterAsync` | POST | `/cgi-bin/express/business/printer/update` | `access_token`(Query) | 绑定 / 解绑打印员 |
| 11 | `GetQuotaAsync` | POST | `/cgi-bin/express/business/quota/get` | `access_token`(Query) | 面单余额；**`quota_num == -1` 表示不限制** |
| 12 | `TestUpdateOrderAsync` | POST | `/cgi-bin/express/business/test_update_order` | `access_token`(Query) | **仅测试环境**，生产不可调用 |

### 2) 服务商侧 / 物流消息 / 退货 / 散单寄件 / 消息推送（`IWxaExpressDeliveryService`，28 支）

> **本接口的 XML `<summary>` 写的是「29 端点」，实际方法数与守卫 MP-X5 都是 28** ⇒ 文档注释与事实不符，
> 见「已知留档缺口」首行。**路由表与计数以守卫 MP-X5 为准。**

面单与轨迹维护 4 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 13 | `GetContactAsync` | POST | `/cgi-bin/express/delivery/contact/get` | 面单联系人；`phone` 敏感 |
| 14 | `PreviewTemplateAsync` | POST | `/cgi-bin/express/delivery/template/preview` | 面单预览 |
| 15 | `UpdateBusinessAsync` | POST | `/cgi-bin/express/delivery/service/business/update` | 订单审核 |
| 16 | `UpdatePathAsync` | POST | `/cgi-bin/express/delivery/path/update` | 轨迹同步（服务商推轨迹） |

无忧退（`no_worry_return/*`）3 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 17 | `AddNoWorryReturnAsync` | POST | `/cgi-bin/express/delivery/no_worry_return/add` | — |
| 18 | `GetNoWorryReturnAsync` | POST | `/cgi-bin/express/delivery/no_worry_return/get` | **路由段是 `gett`（双 t）** |
| 19 | `UnbindNoWorryReturnAsync` | POST | `/cgi-bin/express/delivery/no_worry_return/unbind` | — |

物流消息（`open_msg/*`）9 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 20 | `FollowWaybillAsync` | POST | `/cgi-bin/express/delivery/open_msg/follow_waybill` | **10 万次/日（appid）· 50 次/日（同一用户）· 同一用户最多同时绑 100 个运单号** |
| 21 | `QueryFollowTraceAsync` | POST | `/cgi-bin/express/delivery/open_msg/query_follow_trace` | — |
| 22 | `UpdateFollowWaybillGoodsAsync` | POST | `/cgi-bin/express/delivery/open_msg/update_follow_waybill_goods` | — |
| 23 | `OpenOpenMessageAsync` | POST | `/cgi-bin/express/delivery/open_msg/open_openmsg` | **空体（无字段）**；权限集 45；`933000`–`933004` |
| 24 | `OpenQueryPluginAsync` | POST | `/cgi-bin/express/delivery/open_msg/open_query_plugin` | **空体**；权限集 45 |
| 25 | `TraceWaybillAsync` | POST | `/cgi-bin/express/delivery/open_msg/trace_waybill` | 物流查询插件 |
| 26 | `QueryTraceAsync` | POST | `/cgi-bin/express/delivery/open_msg/query_trace` | **10 万次/日（appid）· 100 次/日（同一用户）** |
| 27 | `UpdateWaybillGoodsAsync` | POST | `/cgi-bin/express/delivery/open_msg/update_waybill_goods` | — |
| 28 | `GetDeliveryListAsync` | POST | `/cgi-bin/express/delivery/open_msg/get_delivery_list` | **空体** |

（本子族编号 20–28 共 **9 支**，与守卫 MP-X5 的 `open_msg/*` 集合逐字对应。）

退货组件（`return/*`）4 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 29 | `OpenReturnAsync` | POST | `/cgi-bin/express/delivery/return/open_return` | **空体**；**调用额度 500 次/天**；权限集 45 |
| 30 | `AddReturnAsync` | POST | `/cgi-bin/express/delivery/return/add` | — |
| 31 | `GetReturnAsync` | POST | `/cgi-bin/express/delivery/return/get` | 应答类型与无忧退共用 `WxaExpressReturnStatusResponse` |
| 32 | `UnbindReturnAsync` | POST | `/cgi-bin/express/delivery/return/unbind` | — |

散单寄件（`single_waybill/*`）5 支 + 投诉 1 支：

| # | 方法 | 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 33 | `UpdateSingleWaybillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/update` | **商户推送 `token`** | 下单事件里推来的 Token |
| 34 | `UpdateSingleWaybillFeeAsync` | POST | `/cgi-bin/express/delivery/single_waybill/fee` | **商户推送 `token`** | 加小费 / 改价 |
| 35 | `CancelSingleWaybillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/cancel_order` | **商户推送 `token`** | — |
| 36 | `RefundSingleWaybillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/refund_order` | **商户推送 `token`** | — |
| 37 | `EvaluateSingleWaybillFeedbackAsync` | POST | `/cgi-bin/express/delivery/single_waybill/evaluate_feedback` | **商户推送 `token`** | 与 #33–36 的 `token` 同源 |
| 38 | `UpdateComplaintResultAsync` | POST | `/cgi-bin/express/delivery/scatter/update_complaint_result` | **商户推送 `token`** | **官方明写不支持第三方平台代调用** |

消息推送 2 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 39 | `NotifyPathAsync` | POST | `/cgi-bin/express/delivery/pathnotify` | 推送轨迹给微信侧 |
| 40 | `QueryUserBindingAsync` | POST | `/cgi-bin/express/delivery/userquery` | 查用户绑定；`phone` 敏感（MP-X7 同口径） |

### 3) 同城配送（`IWxaExpressIntracityService`，16 支）

**全部 POST + 请求体 JSON**；令牌均为主形态 `access_token`(Query)。

| # | 方法 | 路由 | 备注 |
|---|---|---|---|
| 41 | `ApplyIntracityAsync` | `/cgi-bin/express/intracity/apply` | **无请求体**；使用前须先开通 |
| 42 | `CreateStoreAsync` | `/cgi-bin/express/intracity/createstore` | 建门店，产出 `wx_store_id` 定位键 |
| 43 | `QueryStoreAsync` | `/cgi-bin/express/intracity/querystore` | — |
| 44 | `UpdateStoreAsync` | `/cgi-bin/express/intracity/updatestore` | — |
| 45 | `StoreChargeAsync` | `/cgi-bin/express/intracity/storecharge` | **`amount` 单位：分**；应答 `payurl` 拉起微信支付 |
| 46 | `StoreRefundAsync` | `/cgi-bin/express/intracity/storerefund` | — |
| 47 | `QueryFlowAsync` | `/cgi-bin/express/intracity/queryflow` | `flow_type`：**1 充值 / 2 扣款 / 3 退款 / 4 扣款退回**（DTO 上是**非空 `int`**，是全域唯一无 `?` 的枚举位） |
| 48 | `BalanceQueryAsync` | `/cgi-bin/express/intracity/balancequery` | 余额查询 |
| 49 | `PreAddOrderAsync` | `/cgi-bin/express/intracity/preaddorder` | 体 `WxaExpressIntracityOrderBaseRequest`（与正式下单共用基类） |
| 50 | `AddIntracityOrderAsync` | `/cgi-bin/express/intracity/addorder` | — |
| 51 | `QueryIntracityOrderAsync` | `/cgi-bin/express/intracity/queryorder` | — |
| 52 | `CancelIntracityOrderAsync` | `/cgi-bin/express/intracity/cancelorder` | `cancel_reason_id`：**1001 用户 / 1002 门店 / 1003 骑手 / 1004 其它**；骑手已接单后取消可能产生 `deductfee` |
| 53 | `MockNotifyAsync` | `/cgi-bin/express/intracity/mocknotify` | **仅测试环境** |
| 54 | `GetCityAsync` | `/cgi-bin/express/intracity/getcity` | `service_trans_id` 如 `SFTC`（顺丰同城）/ `DADA`（达达）；错误码 `48001` 未开通 / `934003` 运力 ID 错 / `934009` 不支持该门店城市 |
| 55 | `GetPayModeAsync` | `/cgi-bin/express/intracity/getpaymode` | `appid` **必须匹配当前 `access_token`**，否则 `934012`；`pay_appid`（扣费主体为小程序）与 `pay_component_appid`（服务商）**互斥出现** |
| 56 | `SetPayModeAsync` | `/cgi-bin/express/intracity/setpaymode` | `pay_mode` 常量见 `WxaExpressPayModes`：`PAY_MODE_STORE` / `PAY_MODE_APP` / `PAY_MODE_COMPONENT` |

### 4) 微信物流服务 · 无忧退货（`IWxaExpressInsuranceFreightService`，11 支）

**前缀是 `/wxa/business/insurance_freight/`**，不是 `/cgi-bin/express/`（见前缀族一节）。

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 57 | `OpenInsuranceFreightAsync` | **GET** | `/wxa/business/insurance_freight/open` | **GET 且无请求体** |
| 58 | `QueryInsuranceFreightOpenAsync` | **GET** | `/wxa/business/insurance_freight/query_open` | **GET 且无请求体** |
| 59 | `CreateInsuranceOrderAsync` | POST | `/wxa/business/insurance_freight/createorder` | 发货时投保 |
| 60 | `ClaimAsync` | POST | `/wxa/business/insurance_freight/claim` | 收到退货后理赔 |
| 61 | `CreateChargeIdAsync` | POST | `/wxa/business/insurance_freight/createchargeid` | `order_id` 是 **64 位整数** ⇒ JS 侧精度丢失风险，本仓按 DTO 声明承载 |
| 62 | `ApplyPayAsync` | POST | `/wxa/business/insurance_freight/applypay` | 拿充值链接付款 |
| 63 | `GetPayOrderListAsync` | POST | `/wxa/business/insurance_freight/getpayorderlist` | — |
| 64 | `RefundPremiumAsync` | POST | `/wxa/business/insurance_freight/refund` | **POST 且无请求体**；**只能退回原充值账号** |
| 65 | `GetSummaryAsync` | POST | `/wxa/business/insurance_freight/getsummary` | — |
| 66 | `GetOrderListAsync` | POST | `/wxa/business/insurance_freight/getorderlist` | `limit` 默认与上限均为 **100** |
| 67 | `UpdateNotifyFundsAsync` | POST | `/wxa/business/insurance_freight/update_notify_funds` | 路由段带**下划线**（`update_notify_funds`） |

- 权限集 **139**；官方声明支持第三方代商家调用（可 `authorizer_access_token`），
  **本线按 P3-R2 只承载自建形态**。
- **`claim` 的时间约束**：`pay_time` 与微信支付时间误差须在 **3 天内**；常见错误码 `41009`（missing openid）。
- **`createchargeid` 的 `order_id` 是 64 位整数**，接口 XML 明写「JS 等语言须按 `string` 传递以防精度丢失」
  ⇒ C# 侧按 DTO 声明承载，与前端联调时须自行核对传输形态。
- **`getorderlist` 全字段可选**，`limit` 默认 **100**、最大 **100**（无请求必填项，不等于无上限）。
- **调用链**：`open` 开通 → `createorder` 投保 → `claim` 理赔；保费侧 `createchargeid` → `applypay`。

### 5) 即时配送（`IWxaExpressLocalService`，15 支）

**面向配送公司（即时配送运力）**；应答是 `resultcode`/`resultmsg`（见下节）；
令牌均为主形态 `access_token`(Query)，但 `/delivery/update_order` 的体里另带 **`wx_token`**。

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 68 | `GetAllDeliveryAsync` | POST | `/cgi-bin/express/local/business/delivery/getall` | **POST 且无请求体** |
| 69 | `OpenDeliveryAsync` | POST | `/cgi-bin/express/local/business/open` | **POST 且无请求体** |
| 70 | `BindAccountAsync` | POST | `/cgi-bin/express/local/business/shop/add` | 门店绑定 |
| 71 | `GetBindAccountAsync` | POST | `/cgi-bin/express/local/business/shop/get` | **POST 且无请求体** |
| 72 | `PreAddOrderAsync` | POST | `/cgi-bin/express/local/business/order/pre_add` | **路由段有下划线**；体与正式下单同为 `WxaExpressLocalAddOrderRequest`；顺丰同城 / 闪送返回费用与预计接单时间但**不支持 `delivery_token`** |
| 73 | `AddOrderAsync` | POST | `/cgi-bin/express/local/business/order/add` | — |
| 74 | `ReAddOrderAsync` | POST | `/cgi-bin/express/local/business/order/readd` | 重新下单 |
| 75 | `GetOrderAsync` | POST | `/cgi-bin/express/local/business/order/get` | `shopid`/`shop_order_id`/`shop_no`/`delivery_sign` 必填 |
| 76 | `CancelOrderAsync` | POST | `/cgi-bin/express/local/business/order/cancel` | 各家规则不同：顺丰同城急送/闪送/美团配送——配送完成前任意节点可取消；**达达——骑手取货前** |
| 77 | `PreCancelOrderAsync` | POST | `/cgi-bin/express/local/business/order/precancel` | **预检违约金**，与 #76 共用请求体类型 |
| 78 | `ConfirmReturnAsync` | POST | `/cgi-bin/express/local/business/order/confirm_return` | `shopid`/`shop_order_id`/`waybill_id`/`delivery_sign`/`shop_no` 必填 |
| 79 | `AddTipsAsync` | POST | `/cgi-bin/express/local/business/order/addtips` | **仅待接单可加小费**；以**最新一次**为准 ⇒ 下次金额必须**大于**上一次；**美团配送不支持**；**闪送须按固定档位 2/3/5/10/15/20/50/100** |
| 80 | `MockUpdateOrderAsync` | POST | `/cgi-bin/express/local/business/test_update_order` | 沙箱：`shopid` 必须为 `test_shop_id`、**不传 `delivery_sign`** |
| 81 | `RealMockUpdateOrderAsync` | POST | `/cgi-bin/express/local/business/realmock_update_order` | **`delivery_sign` 必填**（与 #80 共用 `WxaExpressLocalMockUpdateOrderRequest`） |
| 82 | `UpdateOrderStatusAsync` | POST | `/cgi-bin/express/local/delivery/update_order` | **运力方侧一支**：`wx_token`（下单事件推送的 token）+ `order_status`/`waybill_id`/`action_time`/`shopid`/`shop_order_id`/`wxa_path` 必填；官方明示**不支持**第三方代调用 |

- 权限集 **51、71**；官方声明本族支持第三方代商家调用，**但 `update_order` 明示不支持**。
- 接入前置（接口 XML）：小程序须完成微信认证 + 开通事件推送（安全模式 / JSON），
  并在配送公司注册账号后于小程序后台完成授权绑定。

### 6) 对账文件（`IWxaExpressBillService`，1 支·**手工通道**）

| # | 方法 | 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 83 | `GetBillAsync` | POST | `/cgi-bin/express/delivery/single_waybill/get_bill` | `access_token`(Query)，**手工拼接** | 返回 **`Task<string>`**（对账文件文本）；失败才返回 JSON ⇒ 抛 `WxaException` |

## 路由前缀族：本域有四套前缀，**不要**抽公共前缀常量

| 前缀族 | 支数 | 归属 |
|---|---|---|
| `/cgi-bin/express/business/` | 12 | 商家寄件 |
| `/cgi-bin/express/delivery/` | 28（+ 对账 `single_waybill/get_bill` 1 支） | 服务商侧 / 物流消息 / 退货 / 散单寄件 / 消息推送 / **对账文件** |
| `/cgi-bin/express/intracity/` | 16 | 同城配送 |
| `/cgi-bin/express/local/` | 15 | 即时配送（再分 `business/*` 14 支与 `delivery/update_order` 1 支） |
| `/wxa/business/insurance_freight/` | 11 | 无忧退货——**前缀与其余四族完全不同** |

- **最大的坑：无忧退货不在 `/cgi-bin/express/` 下**，而在 `/wxa/business/insurance_freight/`。
  按「物流 ⇒ `/cgi-bin/express/`」的直觉生成即 404。
- **`get_bill` 的路由属 `delivery/single_waybill/` 族**（`/cgi-bin/express/delivery/single_waybill/get_bill`），
  而它的**接口**叫 `IWxaExpressBillService`、子族文档归「商家寄件对账文件」
  ⇒ 接口归属与路由族**不同名**，别按接口名推路由。
- **连写族（无下划线）**：`batchget`、`pathnotify`、`userquery`、`getall`、`no_worry_return`（**有**下划线）、
  `createclouduser` 式连写在物流侧对应 `createchargeid`、`applypay`、`getpayorderlist`、`getorderlist`、
  `preaddorder`、`addorder`、`queryorder`、`cancelorder`、`mocknotify`、`querystore`、`updatestore`、
  `balancequery`、`getcity`、`getpaymode`、`setpaymode`、`readd`、`addtips`。
- **带下划线的少数派（务必逐字）**：`no_worry_return`、`open_msg`、`single_waybill`、`update_notify_funds`、
  `order/add`|`order/get`|`order/cancel`（层级下划线）、`pre_add`、`query_follow_trace`、
  `update_follow_waybill_goods`、`query_trace`、`update_waybill_goods`、`get_delivery_list`、
  `open_openmsg`、`open_query_plugin`、`business/update`、`path/update`、`service/business/update`。
- **两个真正的官方拼写缺陷**：`/cgi-bin/express/delivery/no_worry_return/gett`（**双 t**）
  与散单侧 `cancel_order` / `refund_order`（同族 `fee` / `update` 却无 `order` 后缀）。
  **照原文承载，「顺手修正」即与线上报文对不上**（MP-X5 的逐字数组钉死）。

## 「官方查询端点其实是 POST」——本域最集中

- 本域 83 支里 **78 支是 POST、仅 5 支 GET**：`business/{delivery,account,printer}/getall` **3 支**
  + `insurance_freight/{open,query_open}` **2 支**；其余（含手工通道 `get_bill`）全为 POST。
- 反例同样重要：即时配送的 `delivery/getall`、`business/open`、`shop/get` 名字像查询、
  语义像 GET，官方却是 **POST 且无请求体**；无忧退货的 `refund` 是 **POST 且无请求体**。
  ⇒ **「无请求体」不等于「GET」**，本域两种组合都存在，逐字以接口上的 `[Get]` / `[Post]` 为准。
- 本域 GET 支**全部只有令牌 Query**，没有业务 `[Query]` 参数。

## 手工文本通道：`get_bill` 为什么不是 `[HttpClientApi]`

- 成功时返回**对账文件内容（文本，非 JSON）**，失败时才返回 JSON 错误信封
  ⇒ 生成管线按 JSON 反序列化，装不下 ⇒ 走**原始响应直读 + Content-Type 分支判错**
  （同 `IWxaFeedbackMediaService` / `IWxaCodeService` / `IWxaKfMediaService` 的手动通道形态）。
- 实现 [../../Express/WxaExpressBillService.cs](../../Express/WxaExpressBillService.cs)：
  路由以常量 `public const string BillPath` 存在（**不在 HTTP 特性上**），
  因此 MP-X5 把 `BillPath` 单独并入 `ManualChannelRoutes()`（共 6 条）参与合计计数，
  否则会出现「反射口径失效 ⇒ 静默假绿」。
- **令牌**：手工取当前应用令牌拼 Query，**不进** Query 令牌白名单（白名单只收 `[Token]` 声明式接口）；
  令牌失效（`40001` / `40014` / `42001`）时**失效本应用令牌并重试一次**。
- **应用上下文**：`_appContextHolder.Current as IMpAppContext ?? _appManager?.GetDefaultApp()`
  ⇒ 无上下文时**回落默认应用**（守卫 MP-X10 扫描本文件在内的 4 个手动通道实现）。
- **返回值是 `Task<string>`，不是 `IDisposable`** ⇒ **无需** `using`。
  本线需要释放的是 `QrCodeResult`/`WxaCodeResult` 那类图片流通道（`QrCodeLink` 域），别把两者混为一谈。
- 请求体 `WxaExpressSingleWaybillGetBillRequest { Date→"date" string?, BillType→"type" string? }`：
  `date` 为对账日期（**格式 `YYYY-MM-DD`**，不是本线常见的 `yyyymmdd`），
  `type` 为 `ALL` / `SUCCESS` / `REFUND`。面向**微信物流服务商**。
- 请求体序列化优先走 `IAotJsonContentSerializer`（`ExpressJsonContext`），上下文是 `internal`，
  **不得在宿主示例里引用**。

## 即时配送的应答信封不是 `errcode`/`errmsg`

`/cgi-bin/express/local/*` 全族官方用 **`resultcode` / `resultmsg`** 表达运力侧结果
⇒ 该族响应统一继承 `WxaExpressLocalResponse`（在 `WxaResponse` 之上补齐两字段），
**判错以 `resultcode == 0` 为准**（2026-10-12 逐页核验，包 README 同段留档）。

- 因此本域**两层判错口径并存**：五个子族的普通 `WxaResponse`（`errcode`/`errmsg`）
  与即时配送的 `WxaExpressLocalResponse`（`resultcode`/`resultmsg`）⇒
  **不能**用一句 `ThrowIfFailed` 覆盖全运输族。
- 该类**属性名与 JSON 键不同名**（易踩）：`ResultCode`（**非空 `int`** ⇔ `resultcode`）与
  **`ResultMessage`**（⇔ `resultmsg`，属性名拼全）；且它**继承 `WxaResponse`**
  ⇒ 同一对象上 `errcode`/`errmsg` 也在，**判错只看 `ResultCode == 0`**，外层 `ErrorCode` 兜不住运力侧结果。
- 该族官方字段名**页间不一致，照各页原文承载、勿跨页统一**：
  `insurancfee`（下单页）vs `insurancefee`（预下单页）、`couponfee`（下单页）vs `couponFee`（预下单页）。

## 批量、分页与「受理 ≠ 完成」

| 面 | 事实 |
|---|---|
| 批量上限 | `order/batchget` 体 `order_list` **单次最多 50 单** |
| 绑定异步 | `account/bind`（`type=bind/unbind`）**是异步流程**，返回仅表示受理，最终状态须经回调 / 查询确认 |
| 面单余额 | `quota/get` 的 `quota_num == -1` 表示**不限制**（不是「查询失败」） |
| 频率数字（本仓已登记的） | `follow_waybill` 10 万次/日（appid）· 50 次/日（用户）· 同时绑 100 运单；`query_trace` 10 万次/日 · 100 次/日（用户）；`open_return` 500 次/天 |
| 分页 | `insurance_freight/getorderlist` 的 `limit` 默认与上限 **100**；`intracity/queryflow` **不分页**，用 `begin_time` / `end_time` 时间窗（`WxaExpressQueryFlowRequest`：`wx_store_id` `string?`、**`flow_type` 为非空 `int`**、`service_trans_id` `string?`、`begin_time`/`end_time` `long?`）⇒ **本域没有统一分页助手可用** |
| 金额单位 | `intracity/storecharge` 的 `amount` 单位 **分** |
| 未登记频率 | 除上述三支外，本域其余端点的官方频率数字**未逐项核验** ⇒ 见缺口 |

## 权限集与第三方代调用面

| 子族 | 权限集（接口 XML 登记值） | 第三方代调用 |
|---|---|---|
| 商家寄件（12） | 本仓未登记 | **未逐项核验** |
| 服务商侧 / 物流消息（28） | **45**（`open_openmsg` / `open_query_plugin` / `open_return` 明写） | 本域大多面向**微信物流服务商**；`scatter/update_complaint_result` **官方明写不支持**第三方代调用 |
| 同城配送（16） | 本仓未登记 | **未逐项核验** |
| 无忧退货（11） | **139** | 官方声明支持（可 `authorizer_access_token`），**本线只承载自建形态** |
| 即时配送（15） | **51、71** | 官方声明支持，**`update_order` 明示不支持** |
| 对账文件（1） | 本仓未登记 | 面向微信物流服务商 |

- **P3-R2 裁决**（[MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)）：
  本线**不引入 component / authorizer 令牌链**。上表「官方声明支持」只是**留档**，
  不等于 SDK 提供该形态。改这些接口 XML 时注意：该守卫的 `RoutesInCodeLines` 会**剥离注释行**，
  历史上曾因边界注释把 `IWxaDataAnalysisService.cs` 误判为违规。
- **面向服务商 ≠ 代调用**：`IWxaExpressDeliveryService` 的多数端点由**微信物流服务商**调用，
  `single_waybill/*` 的 `token` 是**商户侧下单事件推来的字段**，属**业务参数**而非 SDK 令牌面
  ⇒ **不得**把它做成 `[Token]` 声明（`[Token]` 只会注入 `access_token`，语义错误）。`wx_token` 同理。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram.DataModels.Express;

// 两段式装配：MP 令牌底座先行；AddExpressApi() 一次装载六个接口
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddExpressApi());

// 注入（示例用字段名 business / delivery / intracity / insurance / local / bill）
// 1) 商家寄件：GET 无请求体的三支
var deliveryList = await business.GetAllDeliveryAsync(ct);      // GET，无体
WxaException.ThrowIfFailed(deliveryList, "/cgi-bin/express/business/delivery/getall");

// 2) 批量查单：order_list 单次最多 50 单
var batch = await business.BatchGetOrderAsync(
    new WxaExpressBatchGetOrderRequest
    {
        OrderList =
        [
            new WxaExpressBatchGetOrderItem
            {
                OrderId = "<ORDER_ID>",
                DeliveryId = "<DELIVERY_ID>",
                WaybillId = "<WAYBILL_ID>",
            },
        ],
    }, ct);
WxaException.ThrowIfFailed(batch, "/cgi-bin/express/business/order/batchget");

// 3) 面单余额：-1 表示不限制，不要当作异常值
var quota = await business.GetQuotaAsync(new WxaExpressGetQuotaRequest(), ct);

// 4) 同城配送：金额单位是分；取消原因必须取 1001–1004
var cities = await intracity.GetCityAsync(
    new WxaExpressGetCityRequest { ServiceTransId = "SFTC" }, ct);   // 48001/934003/934009
// pay_mode 用常量类，不要写字面量：WxaExpressPayModes 的三个取值
// 注意 934012：appid 必须与当前 access_token 匹配

// 5) 无忧退货：open / query_open 是 GET 且无体；refund 是 POST 且无体
var opened = await insurance.QueryInsuranceFreightOpenAsync(ct);
// 权限集 139；充值后退款只能退回原充值账号

// 6) 即时配送：判错看 resultcode，不是 errcode
var localDelivery = await local.GetAllDeliveryAsync(ct);   // POST 且无体
// if (localDelivery.ResultCode != 0) { /* 运力侧失败：ResultMessage（JSON 键 resultmsg） */ }
// 该族含 delivery_sign / phone / rider_phone ⇒ 禁入日志 / 遥测 / 异常消息
// /delivery/update_order 的 wx_token 是下单事件推来的 token，绝非 access_token

// 7) 对账文件：手工文本通道，返回 string，无需 using
var billText = await bill.GetBillAsync(
    new WxaExpressSingleWaybillGetBillRequest { Date = "2026-10-17", BillType = "ALL" }, ct);
// 失败时实现内部已按 Content-Type 分支解析 errcode 并抛 WxaException；
// 令牌失效（40001/40014/42001）会失效本应用令牌并重试一次。
```

上例中所有 `<…>` 均为**占位符**。本域凭据级字段 —— `access_token`、`delivery_sign`（由 `appSecret`
加密而来）、商户推送 `token`、`wx_token`、`account/bind` 的 `password`、`contact/get` 与 `userquery`
的 `phone`、`rider_phone` —— **只写参数名，禁入日志、遥测与异常消息**（MP-X7 同口径；
`phone` 在接口 XML 里已按该口径标注）。`ExpressJsonContext` 与各域 resolver 扩展是 `internal`，
宿主代码**不得**引用。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **`IWxaExpressDeliveryService` 文档注释与事实不符** | 接口 `<summary>` 写「**29 端点**」，实际方法 **28** 支，MP-X5 与包 README 也都是 **28**（含 `get_bill` 也不到 29）⇒ 本文按守卫口径取 **28**；**本域合计 83 = 12+28+16+11+15+1** | 修 XML 注释属改 `.cs`，**本 README 任务不触碰**；由后续代码任务同批改为 28 |
| **注册器注释与事实不符** | [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs) 的 `MiniProgramModule.Express` 注释写「其余 **50** 端点走生成管线」，实际声明式端点是 **82** 支（83 − 1 支手工通道）⇒ 同域第二处文档数字失焦 | 同上：本文按 MP-X5 的 **82 + 1** 口径书写，注释由后续代码任务修正 |
| **本域无独立守卫文件** | 六个接口的断言全部落在 `MiniProgramContractGuards.cs`：MP-X5 的**四段 Express 路由数组**（12/28/16/11/15）+ `ManualChannelRoutes()` 的 `BillPath`（1）+ 合计 398；MP-X10 扫 `Express/WxaExpressBillService.cs`；令牌面 MP-X2/X3；跨线路由 MP-X1/X6；代调用面 P3-R2；`phone` 等敏感字段的日志面沿用 MP-X7 通用扫描 | 若要收拢需新建 `WxaExpressContractGuards.cs` 并同批更新本文「事实来源」 |
| 商家寄件 / 同城配送 / 对账文件三支子族的权限集 ID | 接口 XML **未登记** ⇒ **未逐项核验** | 不得写入本文 |
| 除 `follow_waybill` / `query_trace` / `open_return` 外其余各端的官方频率 | **未逐项核验** | 不得凭记忆补数 |
| 各子族错误码全集 | 只登记了 `933000`–`933004`、`48001`、`934003`、`934009`、`934012` 等零散值 | **未逐项核验** |
| 各运力公司（顺丰同城 / 闪送 / 达达 / 美团配送）的差异细则 | 只承载接口 XML 明写的取消规则、`delivery_token` 支持面、小费档位 | **未逐项核验**，勿在封装里做运力侧白名单 |
| `WxaExpressPayModes` 常量全集之外是否有新 `pay_mode` | 只登记三个取值 | **未逐项核验** |
| 本域各 DTO 的必填 / 长度 / 单位全集 | 本文只列出速览与示例用到的字段 | **未逐项核验**；写请求体前逐字段读 `DataModels/Express/` |
| `insurance_freight/createchargeid` 的 `order_id` 精度处置 | 接口 XML 记「64 位整数、JS 侧有精度风险」，C# 侧按 DTO 声明承载 | 与前端联调时须**自行核对**，本仓不做转换 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、官方 URL、频率上限、权限集、运力差异、业务警示 | 六个接口文件的 XML 注释（链接见各小节表头） |
| 手工通道的 Content-Type 判错 / 令牌失效重试 / 默认应用回落 | [../../Express/WxaExpressBillService.cs](../../Express/WxaExpressBillService.cs)（`BillPath` 常量在此） |
| 六子族逐字路由（12/28/16/11/15）与 6 条手工通道参照集 | **MP-X5** `Endpoints_ShouldMatchOfficialRoutesAndCount`（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)） |
| 合计计数 398 = 392 特性路由 + 6 手工通道；本域占 83 | 同上的合计断言 + `Tests/ContractBaseline.cs` |
| 四个手动通道无上下文时回落默认应用 | **MP-X10**（同上文件，扫描清单含 `Express/WxaExpressBillService.cs`） |
| 令牌复用与 `[Token].Name == access_token` | **MP-X2** / **MP-X3**；`MiniProgramScaffoldContractGuards` |
| 平铺命名空间、无 Callback 工程、与公众号线路由不重叠 | **MP-X4** / **MP-X6** / **MP-X1**（`MiniProgramScaffoldContractGuards`） |
| 本线不建 component/authorizer 令牌链 | **P3-R2**（`MiniProgramP3RulingsContractGuards`） |
| 非本线契约路由参照集 | **NC1/NC2/NC3**（`MiniProgramNonContractRouteContractGuards`） |
| 敏感字段不入日志（本域沿用全线口径） | **MP-X7** |
| 请求 / 应答 DTO（按子族分目录） | [../../../Mud.Wechat.MiniProgram.DataModels/Express/](../../../Mud.Wechat.MiniProgram.DataModels/Express/)（`Account`/`Cargo`/`City`/`Common`/`Complaint`/`Contact`/`Insurance`/`Intracity`/`Local`/`Order`/`Path`/`Pay`/`Return`/`Template`/`Trace`/`Waybill`，`SerializerClassName = "Express"`；请求体多落在各子目录的 `RequestModel/`） |
| AOT 序列化上下文（`internal`，示例中不得引用） | `../../../Mud.Wechat.MiniProgram.DataModels/Generated/ExpressJsonContext.g.cs` |
| 本线端点总表与装配入口 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方 URL 与逐参数约束，**同批**更新 MP-X5 的**对应子族路由数组**
   （12 / 28 / 16 / 11 / 15）与**全线合计计数**（398 = 392 + 6，基线 `Tests/ContractBaseline.cs`）；
   改动手工通道时还要同步 `ManualChannelRoutes()` 参照集与 **MP-X10** 的实现文件扫描清单。
   新落地 `clear_quota` 这类官方在册、本仓未声明的端点 ⇒ 走「加端点 + 同批改计数」，不要只加接口。
2. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Express`）→
   跑 `scripts/AddHttpJsonSerializable.ps1` 与 `scripts/GenerateJsonContext.ps1` →
   确认并入 `ExpressJsonContext`；字段名**照各页原文**，包括 `insurancfee` / `insurancefee` /
   `couponfee` / `couponFee` 这类页间不一致 ⇒ **不得跨页统一**。
3. **改注册面** → `MiniProgramModule.Express` + `AddExpressApi()`
   （[MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）；
   六个接口**共用** `RegistryGroupName = "Express"`，新增接口不得另起组名。
4. **更新本文**：六张子表、四套前缀族、动词与空体表、权限集表与缺口。
5. **新增敏感属性**（`token` / `wx_token` / `delivery_sign` / `phone` / `rider_phone` / `password`）
   → 同批确认其不出现在任何日志调用与异常消息中，并检查 **MP-X7** 的扫描面是否覆盖新增文件；
   承载 DTO **不要**覆写 `ToString`。
6. **不要**把 `single_waybill/*` 的 `token` 或 `update_order` 的 `wx_token` 做成 `[Token]` 声明；
   **不要**把 `gett` 改成 `get`、把 `open_msg` 改成 `openmsg`、把无忧退货挪进 `/cgi-bin/express/`；
   **不要**用 `errcode` 判即时配送的错；**不要**把 `quota_num == -1` 当失败；
   **不要**引入 component/authorizer 令牌参数（P3-R2）；
   **不要**把手工通道的 `GetBillAsync` 返回值当流去 `using`。
7. **本线无回调工程**：消息与事件（含本域 `pathnotify`、下单事件推送 `token`、散单事件等）
   的**接收侧**属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），形态由
   [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)
   （MP-X4：`Mud.Wechat.MiniProgram.Callback.csproj` 不得存在）锁定 ——
   本域的 `pathnotify` 是**服务商主动推给微信**的出向端点，**不是**回调接收面，
   **不要**「顺手补」一个回调工程来收物流事件。
8. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
