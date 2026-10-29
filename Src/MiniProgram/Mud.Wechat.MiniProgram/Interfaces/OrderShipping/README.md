# OrderShipping 域 · 品牌申请

微信小程序「品牌申请」域 SDK（2 端点：品牌申请 / 小程序品牌申请状态查询；
官方 order_shipping 分区 `/wxa/sec/famousbrand/*` 路由族）。 接口：[IWxaFamousBrandService](./IWxaFamousBrandService.cs)、[IWxaOrderQueryService](./IWxaOrderQueryService.cs)、[IWxaOrderSettingService](./IWxaOrderSettingService.cs)、[IWxaOrderShippingService](./IWxaOrderShippingService.cs)；注册组 `OrderShipping`；模块枚举 `MiniProgramModule.OrderShipping`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaOrderShippingContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaOrderShippingContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/OrderShipping`](../../DataModels/OrderShipping)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaFamousBrandService = 2；IWxaOrderQueryService = 4；IWxaOrderSettingService = 2；IWxaOrderShippingService = 4；按 HTTP 特性实计 **12** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaFamousBrandService` | `ApplyAsync` | POST | `/wxa/sec/famousbrand/apply` | `AccessToken`（Query），Query `access_token` | 品牌申请（`POST /wxa/sec/famousbrand/apply`，**不支持云调用**，权限集 142）。<br>官方文档：https://developers.weixin… |
| 2 | `IWxaFamousBrandService` | `GetApplyStatusAsync` | POST | `/wxa/sec/famousbrand/get_status` | `AccessToken`（Query），Query `access_token` | 小程序品牌申请状态查询（`POST /wxa/sec/famousbrand/get_status`，**不支持云调用**，权限集 142）。<br>官方文档：https://devel… |
| 3 | `IWxaOrderQueryService` | `GetOrderAsync` | POST | `/wxa/sec/order/get_order` | `AccessToken`（Query），Query `access_token` | 查询订单发货状态（`POST /wxa/sec/order/get_order`，云调用 `wxa.sec.order.getOrder`，权限集 142）。<br>官方文档：https… |
| 4 | `IWxaOrderQueryService` | `GetOrderListAsync` | POST | `/wxa/sec/order/get_order_list` | `AccessToken`（Query），Query `access_token` | 查询订单列表（`POST /wxa/sec/order/get_order_list`，云调用 `wxa.sec.order.getOrderList`，**不支持第三方平台调用*… |
| 5 | `IWxaOrderQueryService` | `IsTradeManagedAsync` | POST | `/wxa/sec/order/is_trade_managed` | `AccessToken`（Query），Query `access_token` | 查询小程序是否已开通发货信息管理服务（`POST /wxa/sec/order/is_trade_managed`，云调用 `wxa.sec.order.isTradeManage… |
| 6 | `IWxaOrderQueryService` | `IsTradeManagementConfirmationCompletedAsync` | POST | `/wxa/sec/order/is_trade_management_confirmation_completed` | `AccessToken`（Query），Query `access_token` | 查询小程序是否已完成交易结算管理确认（`POST /wxa/sec/order/is_trade_management_confirmation_completed`，**不支持云… |
| 7 | `IWxaOrderSettingService` | `SetMsgJumpPathAsync` | POST | `/wxa/sec/order/set_msg_jump_path` | `AccessToken`（Query），Query `access_token` | 消息跳转路径设置（`POST /wxa/sec/order/set_msg_jump_path`，云调用 `wxa.sec.order.setMsgJumpPath`，**不支持第… |
| 8 | `IWxaOrderSettingService` | `ApplyTradeTypeChangeAsync` | POST | `/wxa/sec/order/setwxatradetypecgi` | `AccessToken`（Query），Query `access_token` | 小程序交易类型变更申请（`POST /wxa/sec/order/setwxatradetypecgi`，**不支持云调用**，权限集 142）。<br>官方文档：https://dev… |
| 9 | `IWxaOrderShippingService` | `UploadShippingInfoAsync` | POST | `/wxa/sec/order/upload_shipping_info` | `AccessToken`（Query），Query `access_token` | 发货信息录入（`POST /wxa/sec/order/upload_shipping_info`，云调用 `wxa.sec.order.uploadShippingInfo`，权… |
| 10 | `IWxaOrderShippingService` | `UploadCombinedShippingInfoAsync` | POST | `/wxa/sec/order/upload_combined_shipping_info` | `AccessToken`（Query），Query `access_token` | 发货信息合单录入（`POST /wxa/sec/order/upload_combined_shipping_info`，云调用 `wxa.sec.order.uploadComb… |
| 11 | `IWxaOrderShippingService` | `NotifyConfirmReceiveAsync` | POST | `/wxa/sec/order/notify_confirm_receive` | `AccessToken`（Query），Query `access_token` | 确认收货提醒（`POST /wxa/sec/order/notify_confirm_receive`，云调用 `wxa.sec.order.notifyConfirmReceiv… |
| 12 | `IWxaOrderShippingService` | `ReportSpecialOrderAsync` | POST | `/wxa/sec/order/opspecialorder` | `AccessToken`（Query），Query `access_token` | 特殊发货报备（`POST /wxa/sec/order/opspecialorder`，**不支持云调用**，权限集 142）。<br>官方文档：https://developers.w… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaFamousBrandService`

**路由族与订单发货域不同**：本域是 `/wxa/sec/famousbrand/`（2 条），订单面是
`/wxa/sec/order/`（10 条）⇒ 官方把两支放在同一 order_shipping 目录树下，本仓按两个路由族
分接口、共用一个注册组（守卫 OS1 锁定两族计数）。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`；两支均支持第三方平台以 `authorizer_access_token` 代调用（权限集 142），
两支均**不支持云调用**。

**适用范围段与同域其它端点不同**：本域两页正文写的是「本接口支持「小程序」账号类型调用。
其他账号类型如无特殊说明，均不可调用」，而 `/wxa/sec/order/*` 十页均为「暂未明确可调用账号类型」
⇒ 这是本线少数带**正面账号类型限定**的域，宿主不要把「其它线也能调」当成默认。

**调用限制**：两端点官方正文均未公布端点级频率数字。

### `IWxaOrderQueryService`

**两支订单查询与两支开通状态查询同族的理由**：前两者给「单笔/多笔支付单的发货事实」，
后两者给「本账号能否使用本域的准入条件」⇒ 官方把它们都放在 order_shipping 目录内，
且后两支是接入本域前的**前置探测**（未开通即无法调用 `IWxaOrderShippingService` 的录入端点）。

**定位键名与上传侧不同**：本域用 `merchant_id` / `merchant_trade_no`（+ `sub_merchant_id`），
上传侧 `WxaOrderKey` 用 `mchid` / `out_trade_no` ⇒ 由守卫 OS4 分别锁定，不得合并。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`。**第三方开放面在本接口内即不统一**：`GetOrderAsync` 与
两支开通状态查询支持 `authorizer_access_token`（权限集 142，后两支为 18、142），
`GetOrderListAsync` 官方明写「本接口不支持第三方平台调用」。

**敏感值**：`openid`（支付者）、`shipping.shipping_list[].contact` 的收寄件人联系方式
禁止写入日志、遥测或异常消息；`paid_amount` 是金额（单位分），同样按敏感值处置。

**调用限制**：四端点官方正文均未公布端点级频率数字；`GetOrderListAsync` 的 `page_size` 默认 100 属规模限制，不得当频率解读。

### `IWxaOrderSettingService`

**两支的共同点不是「配置」而是「账号级、非幂等的申报动作」**：跳转路径是账号级唯一值（重复设置即覆盖），
交易类型变更则是**一个账号只能申请一次**的审批类端点 ⇒ 两者都不适合放进重试管道。

**第三方开放面在本接口内也不统一**：`SetMsgJumpPathAsync` 官方明写
「本接口不支持第三方平台调用」，`ApplyTradeTypeChangeAsync` 支持以
`authorizer_access_token` 代调用（权限集 142）。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入 `access_token`。

**调用限制**：两端点官方正文均未公布端点级频率数字；「材料最多 10 个、其中视频最多 3 个」是规模限制，不得当频率解读。

### `IWxaOrderShippingService`

**本域是「资金解冻」的前置动作**（官方原文「用户交易后，默认资金将会进入冻结状态，开发者在发货后，
需要在小程序平台录入相关发货信息」）：未录入即未发货，用户的确认收货与平台的结算都不会推进。
前置开通状态由 `IsTradeManagedAsync` 查询。

**一次机会语义（本域最重要的约束）**：官方对录入两支逐字写明「以下情况将视为重新发货，
每笔支付单仅有一次重新发货机会 —— (1) 对已完成发货的支付单再次调用该 API；
(2) 使用该 API 修改发货模式或物流模式」⇒ **本域端点一律不是幂等的**，
网络超时后的盲目重试会直接消耗掉这次机会。判据回读 `order.shipping.finish_shipping_count`
（见 `GetOrderAsync`）：0 未发货、1 完成、2 已重新发货并已用尽机会。

**「同一请求体的字段名在上传侧与查询侧不同」**：上传侧订单定位对象用 `mchid` / `out_trade_no`，
查询侧用 `merchant_id` / `merchant_trade_no`；上传侧物流单商品描述为 `item_desc`，
查询侧为 `goods_desc`。两套形态由守卫 OS4 分别锁定，**不得**为「统一命名」而合并 DTO。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`；四端点官方均支持第三方平台以 `authorizer_access_token` 代调用，权限集 142。

**敏感值**：`payer.openid`、`contact.consignor_contact` / `receiver_contact`
（官方要求**掩码**传输、最后 4 位数字不得打掩码）禁止写入日志、遥测或异常消息。

**调用限制**：四端点官方正文均未公布端点级频率数字（注意事项段只给业务约束），不得凭第三方 SDK 注释或记忆补数。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `ApplyAsync` — 品牌申请（`POST /wxa/sec/famousbrand/apply`，**不支持云调用**，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html。

- **声明接口**：[IWxaFamousBrandService](./IWxaFamousBrandService.cs)
- **官方契约**：`POST /wxa/sec/famousbrand/apply`

- **请求体**：`WxaFamousBrandApplyRequest` — 请求体（包装键为首字母大写的 `Application`，其内 `apply_for` 必填），见 `WxaFamousBrandApplyRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`（官方返回体表除通用两字段外无业务字段）。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体，注意事项原文点名「Content-Type 需要指定为 application/json」。
- **`Application` 是全仓唯一首字母大写的请求体包装键**（官方请求体表与示例逐字如此） / ⇒ `JsonPropertyName` 不得改写成小写形态，守卫 OS4 锁定该拼写。
- **材料条件必填**：`audit_info` 在申请类型为「知名品牌（`1`）」时必填；品牌类型 `4` / （电商平台旗舰店）要求 `flagship_in_which_ec_platform` + `ec_platform_proof_list`， / 品牌类型 `7`（驰名商标、著名商标）要求 `authority_certified_proof_list`。 / 申请类型 `2`（已接入小店商品组件）不需要品牌材料。
- **三个佐证数组的元素是 `media_id` 字符串**（官方示例为临时素材 id 形态）：须先走「新增临时素材」上传图片，临时素材有效期约束下不得长期复用。
- **错误码 `10210003`「图片解析失败」**说明材料校验发生在服务端解析阶段 ⇒ 提交成功不等于材料可用，须回读 `GetApplyStatusAsync` 的 `audit_reason`。

### 2. `GetApplyStatusAsync` — 小程序品牌申请状态查询（`POST /wxa/sec/famousbrand/get_status`，**不支持云调用**，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getfamousbrandapplystatus.html。

- **声明接口**：[IWxaFamousBrandService](./IWxaFamousBrandService.cs)
- **官方契约**：`POST /wxa/sec/famousbrand/get_status`
- **返回**：`WxaFamousBrandApplyStatusResponse` — 申请进度（`progress.status`）与申请信息（`application`，含驳回原因），见 `WxaFamousBrandApplyStatusResponse`。

**备注（原文）**

- 官方契约：**POST**，请求体表原文为「无」⇒ 本端点无 `[Body]` 参数，由守卫 OS3 与 `/xpay` 域同口径锁定零请求体面。
- **页面自相矛盾处（留档不改契约）**：请求体表写「无」，而同页注意事项要求 `Content-Type: application/json`、 / 错误码表含 `44002`「POST内容为空」⇒ 本仓按**请求体表**（权威参数面）落地为无请求体； / 若宿主实测得到 `44002`，属官方页面口径待证，**不得**自行加空对象参数来绕过。
- **状态在两处给出**：`progress.status` 与 `application.status` 共用一张取值表（`WxaFamousBrandApplyStatusValues`，1 审核中 / 2 驳回 / 3 通过）。
- **官方返回示例两处均为 `0`，而枚举表只有 1/2/3** ⇒ 本表不收录 0，也不得把 0 解释成某种业务态（官方正文无此口径）。
- **驳回原因只在驳回态出现**（官方原文「只有在审核驳回时才有值」）⇒ 通过态下 `audit_reason` 为空属正常，不得当作数据缺失告警。
- **本域错误码含令牌与 IP 白名单族**（`41001` / `42001` / `61004`）：这两支在 `/wxa/sec/order/*` 页面未出现，属页面码表差异。

### 3. `GetOrderAsync` — 查询订单发货状态（`POST /wxa/sec/order/get_order`，云调用 `wxa.sec.order.getOrder`，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html。

- **声明接口**：[IWxaOrderQueryService](./IWxaOrderQueryService.cs)
- **官方契约**：`POST /wxa/sec/order/get_order`

- **请求体**：`WxaOrderQueryRequest` — 请求体（四支定位键官方全标选填，实为「微信单号」或「商户号+商户单号」二选一），见 `WxaOrderQueryRequest`。
- **返回**：`WxaOrderGetResponse` — 支付单信息（`order`，含发货信息与物流单列表），见 `WxaOrderGetResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；定位方式与微信支付侧一致 —— 交易单号形式或商户号 + 商户单号形式，二选一。
- **定位键写错即 `10060001`「支付单不存在」**（官方错误码说明原文点名「检查 transaction_id 字段或 mchid、out_trade_no 字段是否有误」） / —— 注意官方这段**说明文字用的是上传侧的键名 `mchid` / `out_trade_no`**，而本端点请求体实际是 / `merchant_id` / `merchant_trade_no`，属官方页面自身的名实不一致，不得据此改本端点字段面。
- **`order_state` 有 6 档**（1 待发货 / 2 已发货 / 3 确认收货 / 4 交易完成 / 5 已退款 / 6 资金待结算，见 `WxaOrderStateValues`）：与虚拟支付域的订单状态是两套取值，不得跨域复用。
- **`finish_shipping_count` 是「重新发货机会」计数器**（官方：未完成 0、完成 1、重新发货并完成后 2）⇒ 值为 2 表示本单已用掉唯一一次重新发货机会。
- **`description` 由官方截断**（超 120 字截断加省略号），而 `shipping.goods_desc` 是后台录入页的原值 ⇒ 两者可能不等长，别互相校验。

### 4. `GetOrderListAsync` — 查询订单列表（`POST /wxa/sec/order/get_order_list`，云调用 `wxa.sec.order.getOrderList`，**不支持第三方平台调用**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorderlist.html。

- **声明接口**：[IWxaOrderQueryService](./IWxaOrderQueryService.cs)
- **官方契约**：`POST /wxa/sec/order/get_order_list`

- **请求体**：`WxaOrderListRequest` — 请求体（五个参数全填选填：时间范围 / 订单状态 / openid / 游标 / 页长），见 `WxaOrderListRequest`。
- **返回**：`WxaOrderListResponse` — 支付单列表 + 游标（`last_index` / `has_more`），见 `WxaOrderListResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；可按支付时间、支付者 openid 或订单状态检索。
- **游标翻页，不是 offset**：第一页**不传** `last_index`；返回体 `has_more` 为 `true` 时 / 把**本次**返回的 `last_index` 原样带上取下一页 ⇒ 不得自行构造、跨批复用或缓存后回灌。
- **时间窗默认值不对称**：`begin_time` 缺省为 0，`end_time` 缺省为 32 位无符号整型最大值且**含**该时刻 ⇒ 全不传即全量扫描，须自行收窄。
- **本端点不支持第三方代调用**（官方原文），与同域其它查询端点不同 ⇒ 服务商场景请改走 `GetOrderAsync` 逐单查。
- **返回体表未列 `errmsg`**（只有 `errcode` 与业务三字段）⇒ 判错须以 `errcode` 为准，不得依赖 `errmsg` 非空。

### 5. `IsTradeManagedAsync` — 查询小程序是否已开通发货信息管理服务（`POST /wxa/sec/order/is_trade_managed`，云调用 `wxa.sec.order.isTradeManaged`，权限集 18、142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanaged.html。

- **声明接口**：[IWxaOrderQueryService](./IWxaOrderQueryService.cs)
- **官方契约**：`POST /wxa/sec/order/is_trade_managed`

- **请求体**：`WxaOrderTradeManagedQueryRequest` — 请求体（只有 `appid` 一字段且**必填**，与 `IsTradeManagementConfirmationCompletedAsync` 共用），见 `WxaOrderTradeManagedQueryRequest`。
- **返回**：`WxaOrderTradeManagedResponse` — `is_trade_managed`：是否已开通小程序发货信息管理服务，见 `WxaOrderTradeManagedResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；「已开通的小程序，可接入发货信息管理服务 API 进行发货管理」⇒ 本端点是调用录入端点的前置探测。
- **`appid` 必填且非服务商只能查本账号**（官方原文）：与同域订单查询「按单号定位」的形态不同， / 本端点**不接**任何订单参数，只做账号级状态查询。
- **服务商前置**（官方注意事项）：须被授权 **18 或 142** 权限集之一才能查询 —— 与 `GetOrderAsync` 只需 142 不同。
- **错误码面为通用码族**（`-1` / `40013` / `40097` / `44990` / `61003` / `61004` / `61011`，共 7 支）：与本域订单查询端的 `1006xxxx` / `2684851xx` 族完全不同，判错不得跨端点套表。

### 6. `IsTradeManagementConfirmationCompletedAsync` — 查询小程序是否已完成交易结算管理确认（`POST /wxa/sec/order/is_trade_management_confirmation_completed`，**不支持云调用**，权限集 18、142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanagementconfirmationcompleted.html。

- **声明接口**：[IWxaOrderQueryService](./IWxaOrderQueryService.cs)
- **官方契约**：`POST /wxa/sec/order/is_trade_management_confirmation_completed`

- **请求体**：`WxaOrderTradeManagedQueryRequest` — 请求体（只有 `appid` 一字段且必填，与 `IsTradeManagedAsync` 共用同一类型），见 `WxaOrderTradeManagedQueryRequest`。
- **返回**：`WxaOrderTradeConfirmationResponse` — `completed`：是否已完成交易结算管理确认，见 `WxaOrderTradeConfirmationResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；语义是「已对小程序关联的**所有**商户号都完成订单管理授权或解绑」。
- **响应键名不跟随路由**：本端点返回 `completed`，而 `IsTradeManagedAsync` 返回 / `is_trade_managed` ⇒ 两支请求体同形、响应键名各异（守卫 OS4 锁定两个键名，不得对齐）。
- **业务后果**（官方原文）：已完成订单管理授权的商户号，其产生的**所有**订单均须通过发货信息管理服务发货 ⇒ 该状态为 true 后，未录入发货的订单不会结算。
- **本端点不支持云调用**，而其同请求体的 `IsTradeManagedAsync` 支持（`wxa.sec.order.isTradeManaged`）⇒ 同一对镜像端点在云调用面上也不统一。

### 7. `SetMsgJumpPathAsync` — 消息跳转路径设置（`POST /wxa/sec/order/set_msg_jump_path`，云调用 `wxa.sec.order.setMsgJumpPath`，**不支持第三方平台调用**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setmsgjumppath.html。

- **声明接口**：[IWxaOrderSettingService](./IWxaOrderSettingService.cs)
- **官方契约**：`POST /wxa/sec/order/set_msg_jump_path`

- **请求体**：`WxaOrderMsgJumpPathRequest` — 请求体（只有 `path` 一字段，必填），见 `WxaOrderMsgJumpPathRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；前提是**已接入平台确认收货组件**，作用是让用户点击发货 / 确认收货消息直接进到自己的小程序页面。
- **成功响应不代表生效**（官方注意事项）：设置为空路径或小程序中**不存在**的路径时调用照样成功， / 用户仍跳平台默认确认收货页 ⇒ 宿主须自行校验页面路径存在性，不能以 `errcode=0` 当作落地证据。
- **平台会往路径上追加 Query 参数**（官方原文）：`transaction_id`、`merchant_id`、 / `merchant_trade_no`，存在二级商户号时再加 `sub_merchant_id` ⇒ 自定义 Query **必须避开这四个键名**， / 小程序侧用 `onLaunch` 等方式读取。
- **官方接口英文名拼写为 `setMsgJumPpath`**（页面「接口英文名」字段原文），与路由及云调用方法名 `setMsgJumpPath` 不一致 ⇒ 本仓按路由与云调用名落地，英文名差异仅留档。
- **本端点官方只有通用错误码 `-1`**（错误码表恰 1 行）⇒ 无域内码可套。

### 8. `ApplyTradeTypeChangeAsync` — 小程序交易类型变更申请（`POST /wxa/sec/order/setwxatradetypecgi`，**不支持云调用**，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setwxatradetypecgi.html。

- **声明接口**：[IWxaOrderSettingService](./IWxaOrderSettingService.cs)
- **官方契约**：`POST /wxa/sec/order/setwxatradetypecgi`

- **请求体**：`WxaOrderTradeTypeChangeRequest` — 请求体（`trade_type` / `material_list` / `reason` 三支全必填），见 `WxaOrderTradeTypeChangeRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；`trade_type` 取值见 `WxaOrderTradeTypeValues`（1 综合类 / 2 实物电商类 / 3 线下服务类 / 4 在线服务类）。
- **一个账号只能申请一次**（官方注意事项原文）：重复提交返回 `268486048`，需多次申请须联系小程序客服 ⇒ 本端点**不得**放进自动重试链路。
- **材料须先上传**（官方原文）：图片与视频都要先经「新增临时素材」接口上传换 `media_id` 再提交；规模上限为最多 10 个材料、其中视频最多 3 个。
- **本端点无查询伴侣**：官方 order_shipping 目录内没有「查询当前交易类型」页面 ⇒ 变更结果只能通过小程序后台或客服确认，宿主不要指望轮询本域端点得到结论。
- **路由名照官方原文**（`setwxatradetypecgi` 全小写连写、含历史后缀 `cgi`），不得写成下划线形态；守卫 OS1 锁定。

### 9. `UploadShippingInfoAsync` — 发货信息录入（`POST /wxa/sec/order/upload_shipping_info`，云调用 `wxa.sec.order.uploadShippingInfo`，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html。

- **声明接口**：[IWxaOrderShippingService](./IWxaOrderShippingService.cs)
- **官方契约**：`POST /wxa/sec/order/upload_shipping_info`

- **请求体**：`WxaOrderShippingUploadRequest` — 请求体（`order_key` / `logistics_type` / `delivery_mode` / `shipping_list` / `upload_time` / `payer` 必填，`is_all_delivered` 条件必填），见 `WxaOrderShippingUploadRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`（官方返回体表除通用两字段外无业务字段）。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；`shipping_list` 多重性 **[1, 15]**。
- **`upload_time` 是 RFC 3339 字符串**（官方类型 `string`，示例 `2022-12-15T13:29:35.120+08:00`）， / 且「如果要更新物流信息，上传时间必须比之前的请求更新」⇒ 传过去的时间戳串会被拒。
- **条件必填面由业务态决定，SDK 不校验**：`is_all_delivered` 在分拆发货（`delivery_mode=2`）时必填； / `tracking_no` / `express_company` 在物流快递（`logistics_type=1`）时必填； / `contact` 在顺丰时必填且收寄件人二选一。分拆发货**仅支持**物流快递。
- **物流公司编码**须取「获取运力 id 列表 `get_delivery_list`」（本仓已落位于 `IWxaExpressDeliveryService`），不得自行硬编码快递名。
- **兜底通道**（官方原文）：未对接本接口时可在小程序后台「发货信息录入页」手工录入 ⇒ 两条通路写入同一份数据，宿主不得双写。

### 10. `UploadCombinedShippingInfoAsync` — 发货信息合单录入（`POST /wxa/sec/order/upload_combined_shipping_info`，云调用 `wxa.sec.order.uploadCombinedShippingInfo`，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadcombinedshippinginfo.html。

- **声明接口**：[IWxaOrderShippingService](./IWxaOrderShippingService.cs)
- **官方契约**：`POST /wxa/sec/order/upload_combined_shipping_info`

- **请求体**：`WxaOrderCombinedShippingUploadRequest` — 请求体（`order_key` / `upload_time` / `payer` 必填，物流信息只挂在 `sub_orders` 上），见 `WxaOrderCombinedShippingUploadRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；注意事项与「发货信息录入」逐字同族（一次重新发货机会、 / 分拆仅支持物流快递、最多 15 个包裹）。
- **顶层没有 `logistics_type` / `delivery_mode` / `shipping_list`**：合单是把物流信息 / 按子单分摊，模式与物流类型都是**子单级**属性 ⇒ 与 `UploadShippingInfoAsync` 的请求体 / 结构不同，守卫 OS4 锁定该形状差异。
- **主单与子单的 `order_number_type` 必须一致**（官方原文）：混用「商户侧单号」与「微信支付单号」两种形式即为参数错误。

### 11. `NotifyConfirmReceiveAsync` — 确认收货提醒（`POST /wxa/sec/order/notify_confirm_receive`，云调用 `wxa.sec.order.notifyConfirmReceive`，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_notifyconfirmreceive.html。

- **声明接口**：[IWxaOrderShippingService](./IWxaOrderShippingService.cs)
- **官方契约**：`POST /wxa/sec/order/notify_confirm_receive`

- **请求体**：`WxaOrderConfirmReceiveRequest` — 请求体（`received_time` 必填，四支订单定位键按「微信单号」或「商户号+商户单号」二选一），见 `WxaOrderConfirmReceiveRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；用途是「提醒用户及时确认收货，以提高资金结算效率」。
- **每个订单仅可调用一次**（官方正文原话）⇒ 与录入两支同为非幂等端点，重试须宿主自行判重。
- **两条硬前置**（官方注意事项）：① 只有物流类型为**物流快递**时才能提醒（虚拟商品 / 自提 / 同城配送 / 调用即失败）；② `received_time` 由商户传入且**必须在发货时间之后**，该值会直接显示在推给用户的提醒消息里。
- **签收时间真实性**：官方要求「已从快递物流服务方获知用户确实签收」才可调 ⇒ 伪造签收时间属业务违规，SDK 不做校验也不背书。

### 12. `ReportSpecialOrderAsync` — 特殊发货报备（`POST /wxa/sec/order/opspecialorder`，**不支持云调用**，权限集 142）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_opspecialorder.html。

- **声明接口**：[IWxaOrderShippingService](./IWxaOrderShippingService.cs)
- **官方契约**：`POST /wxa/sec/order/opspecialorder`

- **请求体**：`WxaOrderSpecialReportRequest` — 请求体（`order_id` + `type` 必填，`delay_to` 在预售类型下必填），见 `WxaOrderSpecialReportRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；适用「预售商品订单和测试订单」两类，**发货时效超过 15 天的订单必须报备为预售**。
- **定位键形态与同域其它端点不同**：`order_id` 是**单个字符串**（微信支付单号**或**商户单号均可）， / 没有 `order_number_type` 判别位，也不接 `order_key` 对象 ⇒ 不得复用 `WxaOrderKey`。
- **预售时间窗**（官方注意事项）：`delay_to` 必须在订单支付时间**之后 15 天以上、330 天以内**；报备后用户收到预计发货时间通知。
- **测试订单不触发退款**（官方原文）：报备后无需发货，如需退回资金须前往商户号后台操作退款或按正常发货流程处理。
- **本端点错误码表只有 3 支**（`268546000` ~ `268546002`，官方正文即如此，**不含通用码 `-1`**）⇒ 判错以本表为准。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/OrderShipping`](../../DataModels/OrderShipping) 内共 **30** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **30** 个，`SerializerClassName` = `OrderShipping`。

AOT 源生成上下文：[`OrderShippingJsonContext.g.cs`](./../../DataModels/Generated/OrderShippingJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaFamousBrandApplyRequest` | 请求体 | [OrderShipping/FamousBrand/WxaFamousBrandApplyRequest.cs](../../DataModels/OrderShipping/FamousBrand/WxaFamousBrandApplyRequest.cs) | `Application` |
| `WxaFamousBrandApplyStatusResponse` | 应答 | [OrderShipping/FamousBrand/WxaFamousBrandApplyStatusResponse.cs](../../DataModels/OrderShipping/FamousBrand/WxaFamousBrandApplyStatusResponse.cs) | `progress`, `application` |
| `WxaOrderGetResponse` | 应答 | [OrderShipping/Query/WxaOrderGetResponse.cs](../../DataModels/OrderShipping/Query/WxaOrderGetResponse.cs) | `order` |
| `WxaOrderQueryRequest` | 请求体 | [OrderShipping/Query/WxaOrderQueryRequest.cs](../../DataModels/OrderShipping/Query/WxaOrderQueryRequest.cs) | `transaction_id`, `merchant_id`, `sub_merchant_id`, `merchant_trade_no` |
| `WxaOrderListResponse` | 应答 | [OrderShipping/Query/WxaOrderListResponse.cs](../../DataModels/OrderShipping/Query/WxaOrderListResponse.cs) | `last_index`, `has_more`, `order_list` |
| `WxaOrderListRequest` | 请求体 | [OrderShipping/Query/WxaOrderListRequest.cs](../../DataModels/OrderShipping/Query/WxaOrderListRequest.cs) | `pay_time_range`, `order_state`, `openid`, `last_index`, `page_size` |
| `WxaOrderTradeManagedResponse` | 应答 | [OrderShipping/Query/WxaOrderTradeManagedResponse.cs](../../DataModels/OrderShipping/Query/WxaOrderTradeManagedResponse.cs) | `is_trade_managed` |
| `WxaOrderTradeManagedQueryRequest` | 请求体 | [OrderShipping/Query/WxaOrderTradeManagedQueryRequest.cs](../../DataModels/OrderShipping/Query/WxaOrderTradeManagedQueryRequest.cs) | `appid` |
| `WxaOrderTradeConfirmationResponse` | 应答 | [OrderShipping/Query/WxaOrderTradeConfirmationResponse.cs](../../DataModels/OrderShipping/Query/WxaOrderTradeConfirmationResponse.cs) | `completed` |
| `WxaOrderMsgJumpPathRequest` | 请求体 | [OrderShipping/Setting/WxaOrderMsgJumpPathRequest.cs](../../DataModels/OrderShipping/Setting/WxaOrderMsgJumpPathRequest.cs) | `path` |
| `WxaOrderTradeTypeChangeRequest` | 请求体 | [OrderShipping/Setting/WxaOrderTradeTypeChangeRequest.cs](../../DataModels/OrderShipping/Setting/WxaOrderTradeTypeChangeRequest.cs) | `trade_type`, `material_list`, `reason` |
| `WxaOrderShippingUploadRequest` | 请求体 | [OrderShipping/Shipping/WxaOrderShippingUploadRequest.cs](../../DataModels/OrderShipping/Shipping/WxaOrderShippingUploadRequest.cs) | `order_key`, `logistics_type`, `delivery_mode`, `is_all_delivered`, `shipping_list`, `upload_time`, `payer` |
| `WxaOrderCombinedShippingUploadRequest` | 请求体 | [OrderShipping/Shipping/WxaOrderCombinedShippingUploadRequest.cs](../../DataModels/OrderShipping/Shipping/WxaOrderCombinedShippingUploadRequest.cs) | `order_key`, `sub_orders`, `upload_time`, `payer` |
| `WxaOrderConfirmReceiveRequest` | 请求体 | [OrderShipping/Shipping/WxaOrderConfirmReceiveRequest.cs](../../DataModels/OrderShipping/Shipping/WxaOrderConfirmReceiveRequest.cs) | `transaction_id`, `merchant_id`, `sub_merchant_id`, `merchant_trade_no`, `received_time` |
| `WxaOrderSpecialReportRequest` | 请求体 | [OrderShipping/Shipping/WxaOrderSpecialReportRequest.cs](../../DataModels/OrderShipping/Shipping/WxaOrderSpecialReportRequest.cs) | `order_id`, `type`, `delay_to` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.OrderShipping;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddOrderShippingApi());

var service = app.Services.GetRequiredService<IWxaFamousBrandService>();
var ct = CancellationToken.None;
var response = await service.GetApplyStatusAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/sec/famousbrand/get_status");
// 占位值仅用于编译示意；业务值须按 IWxaFamousBrandService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetApplyStatusAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaFamousBrandService](./IWxaFamousBrandService.cs)、[IWxaOrderQueryService](./IWxaOrderQueryService.cs)、[IWxaOrderSettingService](./IWxaOrderSettingService.cs)、[IWxaOrderShippingService](./IWxaOrderShippingService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaOrderShippingContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaOrderShippingContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `OrderShippingEndpoints_ShouldMatchOfficialRoutes`、`OrderShippingInterfaces_ShouldBindMpAccessToken_ShareRegistryGroup_AndLockMethodCounts`、`OrderShippingMethods_ShouldUseJsonBodyOnly_AndDeclareNoQueryCredential`、`OrderShippingFields_ShouldKeepOfficialAsymmetries`、`OrderShippingDataModels_ShouldLockOfficialEnumsAndJsonRegistration`、`OrderShippingOpenSurfaceAndErrorCodes_ShouldStayRegistered`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/OrderShipping`](../../DataModels/OrderShipping) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaOrderShippingContractGuards.cs`、`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
