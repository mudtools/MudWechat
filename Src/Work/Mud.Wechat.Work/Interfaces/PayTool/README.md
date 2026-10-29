# PayTool 域 · 收银台

企业微信「收银台」模块发票管理域第三方应用 / 服务商代开发 SDK（获取发票列表 / 标记开票状态）。 接口：[IWechatWorkPayToolInvoiceService](./IWechatWorkPayToolInvoiceService.cs)、[IWechatWorkThirdPartyPayToolInvoiceService](./IWechatWorkPayToolInvoiceService_ThirdParty.cs)、[IWechatWorkPayToolOrderService](./IWechatWorkPayToolOrderService.cs)、[IWechatWorkThirdPartyPayToolOrderService](./IWechatWorkPayToolOrderService_ThirdParty.cs)、[IWechatWorkPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService.cs)、[IWechatWorkThirdPartyPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService_ThirdParty.cs)；注册组 `PayTool`；模块枚举 `WechatModule.PayTool`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayToolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/PayTool`](../../../Mud.Wechat.Work/DataModels/PayTool/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkThirdPartyPayToolInvoiceService` | `GetInvoiceListAsync` | POST | `/cgi-bin/paytool/get_invoice_list` | `ProviderAccessToken`（Query），Query `provider_access_token` | 获取发票列表 |
| 2 | `IWechatWorkThirdPartyPayToolInvoiceService` | `MarkInvoiceStatusAsync` | POST | `/cgi-bin/paytool/mark_invoice_status` | `ProviderAccessToken`（Query），Query `provider_access_token` | 标记开票状态 |
| 3 | `IWechatWorkThirdPartyPayToolOrderService` | `CreateOrderAsync` | POST | `/cgi-bin/paytool/open_order` | `ProviderAccessToken`（Query），Query `provider_access_token` | 创建收款订单 |
| 4 | `IWechatWorkThirdPartyPayToolOrderService` | `CloseOrderAsync` | POST | `/cgi-bin/paytool/close_order` | `ProviderAccessToken`（Query），Query `provider_access_token` | 取消收款订单 |
| 5 | `IWechatWorkThirdPartyPayToolOrderService` | `GetOrderListAsync` | POST | `/cgi-bin/paytool/get_order_list` | `ProviderAccessToken`（Query），Query `provider_access_token` | 获取收款订单列表 |
| 6 | `IWechatWorkThirdPartyPayToolOrderService` | `GetOrderDetailAsync` | POST | `/cgi-bin/paytool/get_order_detail` | `ProviderAccessToken`（Query），Query `provider_access_token` | 获取收款订单详情 |
| 7 | `IWechatWorkThirdPartyPayToolVersionSuiteService` | `GetVersionOrderListAsync` | POST | `/cgi-bin/service/get_order_list` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取订单列表 |
| 8 | `IWechatWorkThirdPartyPayToolVersionSuiteService` | `GetVersionOrderDetailAsync` | POST | `/cgi-bin/service/get_order` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取订单详情 |
| 9 | `IWechatWorkThirdPartyPayToolVersionSuiteService` | `ProlongTrialAsync` | POST | `/cgi-bin/service/prolong_try` | `SuiteAccessToken`（Query），Query `suite_access_token` | 延长试用期 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkPayToolInvoiceService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 否（父接口） |
| `IWechatWorkThirdPartyPayToolInvoiceService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 是 |
| `IWechatWorkPayToolOrderService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 否（父接口） |
| `IWechatWorkThirdPartyPayToolOrderService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 是 |
| `IWechatWorkPayToolVersionSuiteService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 否（父接口） |
| `IWechatWorkThirdPartyPayToolVersionSuiteService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkPayToolInvoiceService`

官方在第三方应用开发与服务商代开发两棵文档树的「收银台 → 发票管理」分组下提供本族端点、
共享同一端点页（第三方树 99436/99437 = 代开发树 99447/99448），
企业自建应用开发文档树**无对应 API**；且走服务商凭证
`provider_access_token`，与自建 / 授权企业的 `access_token` 路由键不同，
故按「独立 provider 令牌 ⇒ 独立成族」落位：父接口零端点（`IWechatWorkPayToolInvoiceService`
为 `IsAbstract`），全部 2 个端点声明于唯一子接口
`IWechatWorkThirdPartyPayToolInvoiceService`。

**无需签名**：与同域「收款工具」族不同，本族 2 个端点的官方参数表**不含**
`nonce_str` / `ts` / `sig`，调用时不要附加签名三要素。

同域另两族：收款工具族见 `IWechatWorkPayToolOrderService`（同走 provider_access_token，**需签名**）；
应用版本付费族见 `IWechatWorkPayToolVersionSuiteService`（走 suite_access_token）。

### `IWechatWorkThirdPartyPayToolInvoiceService`

官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
全部 2 个端点声明于本接口；企业自建应用官方不开放，不设对应子接口。

消费服务商级 provider_access_token（路由键 `ProviderAccessToken`）。

官方权限口径：服务商需有在收银台完成商户号注册；「标记开票状态」的操作人还需有
「收银台-发票管理」的权限。

官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。

### `IWechatWorkPayToolOrderService`

官方在第三方应用开发与服务商代开发两棵文档树的「收银台」分组下提供本族端点、共享同一端点页
（第三方树 98045/98046/98053/98054 = 代开发树 99358/99359/99360/99361），
企业自建应用开发文档树**无对应 API**；且走服务商凭证
`provider_access_token`，与自建 / 授权企业的 `access_token` 路由键不同，
故按「独立 provider 令牌 ⇒ 独立成族」落位：父接口零端点（`IWechatWorkPayToolOrderService`
为 `IsAbstract`），全部 4 个端点声明于唯一子接口
`IWechatWorkThirdPartyPayToolOrderService`
（落位形态对齐 `IWechatWorkServicePromotionQrCodeService` 零端点父接口 + 唯一第三方子接口承载）。

**签名强制**：本族 4 个端点的请求体均须携带 `nonce_str` / `ts` / `sig`，
签名算法见官方 [path 98768 签名算法](https://developer.work.weixin.qq.com/document/path/98768)
（HMAC-SHA256 + Base64，密钥取自「工作台→企业微信服务商助手→工具→收银台→收银台 API 调用密钥」），
SDK 侧实现见 `WechatPayToolSignature`。同域「发票管理」族**无需签名**。

同域另两族：发票管理族见 `IWechatWorkPayToolInvoiceService`（同走 provider_access_token）；
应用版本付费族见 `IWechatWorkPayToolVersionSuiteService`（走 suite_access_token）。

### `IWechatWorkThirdPartyPayToolOrderService`

官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
全部 4 个端点声明于本接口；企业自建应用官方不开放，不设对应子接口。

消费服务商级 provider_access_token（路由键 `ProviderAccessToken`）。

官方权限口径：创建与取消收款订单要求「服务商需有在收银台完成商户号注册」（支付方式为「免支付」的订单可不受此限制）；
获取订单列表与订单详情页面标注「无特殊权限」。

官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。

### `IWechatWorkPayToolVersionSuiteService`

官方在**第三方应用开发**文档树的「收银台 → 应用版本付费」分组下提供本族端点
（91910 / 91909 / 91913），企业自建应用开发与服务商代开发文档树**均无对应 API**；
且三个端点均消费 **suite_access_token**（套件级令牌），与同域另两族的
`provider_access_token` 路由键不同，故按「独立套件令牌 ⇒ 独立成族」落位：
父接口零端点（`IWechatWorkPayToolVersionSuiteService` 为 `IsAbstract`），
全部 3 个端点声明于唯一子接口 `IWechatWorkThirdPartyPayToolVersionSuiteService`
（落位形态对齐 `IWechatWorkIdentitySuiteService` 零端点父接口 + 唯一第三方子接口承载）。

**与既有授权流族的边界**：同族官方文档另列「获取企业永久授权码（91911）/ 获取企业授权信息（91912）」，
二者与授权流接口族（`IWechatWorkProviderAuthenticationService`）为**同一端点**，
已在该族承载，故本族**不重复声明**，避免同一路由出现两处映射。

**签名**：本族端点走 `suite_access_token` 鉴权，官方参数表**不含**
`nonce_str` / `ts` / `sig`（签名仅「收款工具」族需要）。

### `IWechatWorkThirdPartyPayToolVersionSuiteService`

官方仅在第三方应用开发文档树开放本族端点，全部 3 个端点声明于本接口；
企业自建应用与服务商代开发官方不开放，不设对应子接口。
官方「获取企业永久授权码 / 获取企业授权信息」与授权流接口族为同一端点，已在
`IWechatWorkProviderAuthenticationService` 承载，本接口不重复声明。

消费服务商套件级 suite_access_token（路由键 `SuiteAccessToken`）。

官方约束：订单事件的回调通知（应用版本回调通知族：下单成功 / 改单 / 支付成功 / 退款 /
应用版本变更 / 取消订单）经**指令回调 URL** 推送到套件通道，回调事件键见
`WechatCallbackEventTypes`。

官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（suite_access_token），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 9 次）官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。

### 1. `GetInvoiceListAsync` — 获取发票列表

- **声明接口**：[IWechatWorkThirdPartyPayToolInvoiceService](./IWechatWorkPayToolInvoiceService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/paytool/get_invoice_list`

- **请求体**：`GetPayToolInvoiceListRequest` — 请求体（`GetPayToolInvoiceListRequest`：start_time 开始时间 /
end_time 结束时间 / cursor 分页游标 / limit 返回的最大记录数）。
- **返回**：`GetPayToolInvoiceListResponse` — 是否有更多（has_more）/ 分页游标（next_cursor）/ 发票列表（invoice_list）。 列表项含 order_id / custom_corpid / apply_time / invoice_type / paid_price / invoice_status /
invoice_title / tax_number / send_way / contact_name / contact_tel / contact_addr / contact_postcode /
receive_email / company_addr / company_tel / bank_name / bank_account_number / invoice_note。

**补充说明**

- 服务商可以使用该接口查询客户企业已提交的开票申请。
- 官方权限：服务商需有在收银台完成商户号注册。
- 官方约束：start_time 与 end_time **不能单独指定，必须同时指定**； limit 为可选整型，最大值 100、默认值 50；cursor 由上一次调用返回，首次调用可不填。
- 官方契约陷阱：本端点参数表**不含** nonce_str / ts / sig（与收款工具族不同），调用时不要附加签名三要素。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 99436 获取发票列表](https://developer.work.weixin.qq.com/document/path/99436)
- **服务商代开发**SDK文档（官方两棵文档树共享同一端点页）：[path 99447 获取发票列表](https://developer.work.weixin.qq.com/document/path/99447)

### 2. `MarkInvoiceStatusAsync` — 标记开票状态

- **声明接口**：[IWechatWorkThirdPartyPayToolInvoiceService](./IWechatWorkPayToolInvoiceService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/paytool/mark_invoice_status`

- **请求体**：`MarkPayToolInvoiceStatusRequest` — 请求体（`MarkPayToolInvoiceStatusRequest`：order_id 订单号 /
oper_userid 操作人 userid / invoice_status 开票状态 / invoice_note 开票备注）。
- **返回**：`WechatWorkResponse` — 仅 errcode / errmsg（官方本端点无业务负载）。

**补充说明**

- 服务商可以使用该接口标记某个应用订单发票的开票状态。
- 官方权限：服务商需有在收银台完成商户号注册。
- 官方约束：操作人需要有「收银台-发票管理」的权限；invoice_status 取 `1`-已开具纸质发票并邮寄给客户 / `2`-已开具电子发票并发送至客户邮箱 / `3`-取消开具发票（取消后企业可再次申请）； **若订单对应开票状态为已开票，此次标记将不生效**。
- 官方约束：invoice_note 为官方必填，不超过 200 字节，客户侧可见。
- 官方契约陷阱：本端点参数表不含 nonce_str / ts / sig（与收款工具族不同）。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 99437 标记开票状态](https://developer.work.weixin.qq.com/document/path/99437)
- **服务商代开发**SDK文档（官方两棵文档树共享同一端点页）：[path 99448 标记开票状态](https://developer.work.weixin.qq.com/document/path/99448)

### 3. `CreateOrderAsync` — 创建收款订单

- **声明接口**：[IWechatWorkThirdPartyPayToolOrderService](./IWechatWorkPayToolOrderService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/paytool/open_order`

- **请求体**：`CreatePayToolOrderRequest` — 请求体（`CreatePayToolOrderRequest`：business_type 业务类型 /
custom_corpid 客户企业 corpid / pay_type 支付方式 / bank_receipt_media_id 银行收款回单凭证 media_id /
creator 订单创建人 userid / product_list 购买商品 / nonce_str 随机字符串 / ts 时间戳 / sig 签名）。
- **返回**：`CreatePayToolOrderResponse` — 收款订单号（order_id）/ 收款订单链接（order_url）/ 原价（origin_price）/ 折后价（paid_price）。 官方口径：order_url 对客户支付订单为支付链接、对服务商代支付与免支付订单为订单确认链接；
两个价格仅部分可以确定价格的请求下会返回，代开发应用的原价和折后价一致。

**补充说明**

- 服务商可以使用该接口创建各种业务的收款订单。
- 官方权限：服务商需有在收银台完成商户号注册（支付方式为「免支付」的订单可以不受此限制）。
- 官方约束：请求体须携带 nonce_str / ts / sig 三要素，签名算法见官方 [path 98768](https://developer.work.weixin.qq.com/document/path/98768)； ts 为 unix 时间戳（中国时区，精确到秒），**业务系统的机器时间与腾讯的时间相差不能超过 15 分钟**； nonce_str 长度在 32 字节以内且**需保证 15 分钟内不能重复**。
- 官方约束：product_list 按 business_type 三选一 —— 1 普通第三方应用填 third_app、2 代开发应用填 customized_app、 3 行业解决方案填 promotion_case；buy_info_list 可填充个数 1 ~ 20。
- 官方约束：notify_custom_corp 开启后，服务商代支付与免支付订单将向企业管理员推送确认提醒， **超时（72 小时）未确认会自动确认**；客户支付订单创建后将向企业管理员推送支付提醒； 指定的免支付订单创建后默认推送订单通知、不可取消。
- 官方说明：bank_receipt_media_id 需通过「服务商上传临时素材」上传（须指定 attachment_type=3 专用于收银台）。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 98045 创建收款订单](https://developer.work.weixin.qq.com/document/path/98045)
- **服务商代开发**SDK文档（官方两棵文档树共享同一端点页）：[path 99358 创建收款订单](https://developer.work.weixin.qq.com/document/path/99358)
- **签名算法**SDK文档：[path 98768 签名算法](https://developer.work.weixin.qq.com/document/path/98768)（代开发树同文：[path 99362](https://developer.work.weixin.qq.com/document/path/99362)）

### 4. `CloseOrderAsync` — 取消收款订单

- **声明接口**：[IWechatWorkThirdPartyPayToolOrderService](./IWechatWorkPayToolOrderService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/paytool/close_order`

- **请求体**：`ClosePayToolOrderRequest` — 请求体（`ClosePayToolOrderRequest`：order_id 收款订单号 /
nonce_str 随机字符串 / ts 时间戳 / sig 签名）。
- **返回**：`WechatWorkResponse` — 仅 errcode / errmsg（官方本端点无业务负载）。

**补充说明**

- 服务商可以使用该接口取消指定的收款订单。
- 官方权限：服务商需有在收银台完成商户号注册。
- 官方约束：请求体须携带 nonce_str / ts / sig 三要素，签名算法见官方 [path 98768](https://developer.work.weixin.qq.com/document/path/98768)。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 98046 取消收款订单](https://developer.work.weixin.qq.com/document/path/98046)
- **服务商代开发**SDK文档（官方两棵文档树共享同一端点页）：[path 99359 取消收款订单](https://developer.work.weixin.qq.com/document/path/99359)

### 5. `GetOrderListAsync` — 获取收款订单列表

- **声明接口**：[IWechatWorkThirdPartyPayToolOrderService](./IWechatWorkPayToolOrderService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/paytool/get_order_list`

- **请求体**：`GetPayToolOrderListRequest` — 请求体（`GetPayToolOrderListRequest`：business_type 业务类型 /
start_time 起始时间 / end_time 结束时间 / cursor 分页游标 / limit 分页条数 /
nonce_str 随机字符串 / ts 时间戳 / sig 签名）。
- **返回**：`GetPayToolOrderListResponse` — 分页游标（next_cursor）/ 是否还有更多（has_more）/ 订单列表（pay_order_list）。 列表项含 order_id / create_time / custom_corpid / buy_content / origin_price / paid_price /
order_status / order_from / creator / pay_type。

**补充说明**

- 服务商可以使用该接口查询指定时间段内创建的各种业务的收款订单列表。
- 官方权限：本端点页面标注「无特殊权限」。
- 官方约束：limit 为官方必填，取值范围 1 ~ 2000；cursor 由上一次调用返回，首次调用不填； 请求体须携带 nonce_str / ts / sig 三要素。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 98053 获取收款订单列表](https://developer.work.weixin.qq.com/document/path/98053)
- **服务商代开发**SDK文档（官方两棵文档树共享同一端点页）：[path 99360 获取收款订单列表](https://developer.work.weixin.qq.com/document/path/99360)

### 6. `GetOrderDetailAsync` — 获取收款订单详情

- **声明接口**：[IWechatWorkThirdPartyPayToolOrderService](./IWechatWorkPayToolOrderService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/paytool/get_order_detail`

- **请求体**：`GetPayToolOrderDetailRequest` — 请求体（`GetPayToolOrderDetailRequest`：order_id 订单号 /
nonce_str 随机字符串 / ts 时间戳 / sig 签名）。
- **返回**：`GetPayToolOrderDetailResponse` — 订单详情（pay_order）。 含 order_id / create_time / custom_corpid / buy_content / origin_price / paid_price /
order_status / order_from / creator / pay_type / custom_corp_name / pay_channel / channel_order_id /
paid_time / business_type / income_type / income_time / income_amount / product_list 购买明细。 官方口径：pay_channel、channel_order_id、paid_time 未支付时为空；
income_type、income_time、income_amount 收入未到账时为空。

**补充说明**

- 服务商可以使用该接口查询指定收款订单的详情。
- 官方权限：本端点页面标注「无特殊权限」。
- 官方约束：请求体须携带 nonce_str / ts / sig 三要素。
- 官方契约陷阱：返回参数表同时列出 pay_order.pay_from 与 pay_order.pay_type，而返回示例只用 `pay_type`，本 SDK 照官方返回示例承载 `pay_type`。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 98054 获取收款订单详情](https://developer.work.weixin.qq.com/document/path/98054)
- **服务商代开发**SDK文档（官方两棵文档树共享同一端点页）：[path 99361 获取收款订单详情](https://developer.work.weixin.qq.com/document/path/99361)

### 7. `GetVersionOrderListAsync` — 获取订单列表

- **声明接口**：[IWechatWorkThirdPartyPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/service/get_order_list`

- **请求体**：`GetPayToolVersionOrderListRequest` — 请求体（`GetPayToolVersionOrderListRequest`：start_time 起始时间 /
end_time 终止时间 / test_mode 正式或测试授权订单）。
- **返回**：`GetPayToolVersionOrderListResponse` — 订单列表（order_list）。 列表项含 orderid / order_status / order_type / paid_corpid / operator_id / suiteid / appid /
edition_id / edition_name / price / user_count / order_period / order_time / paid_time /
begin_time / end_time / order_from / operator_corpid / service_share_amount /
platform_share_amount / dealer_share_amount / dealer_corp_info。

**补充说明**

- 查询指定时间范围内的应用版本付费订单列表。
- 官方约束：start_time / end_time 为官方必填（UNIX 时间戳）； test_mode 指定拉取正式或测试授权的订单，默认值 0（0-正式授权，1-测试授权）。
- 官方说明：本端点无分页游标，一次返回时间窗内的订单列表。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 91910 获取订单列表](https://developer.work.weixin.qq.com/document/path/91910)

### 8. `GetVersionOrderDetailAsync` — 获取订单详情

- **声明接口**：[IWechatWorkThirdPartyPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/service/get_order`

- **请求体**：`GetPayToolVersionOrderDetailRequest` — 请求体（`GetPayToolVersionOrderDetailRequest`：orderid 订单号）。
- **返回**：`GetPayToolVersionOrderDetailResponse` — 订单详情（订单字段平铺于响应根级，见 `PayToolVersionOrder`）。 官方口径：operator_id 在服务商代下单、服务商下的免支付订单等情形没有该字段；
dealer_corp_info 仅当有渠道商报备后才会有此字段。

**补充说明**

- 查询指定订单的详情；订单字段平铺在响应根级（与订单列表元素同构）。
- 官方契约陷阱：请求参数名为全小写 `orderid`（非 `order_id`），本 SDK 照抄官方原文。
- 官方说明：改单（服务商管理员修改订单价格）会产生**新的订单号**， 服务商须用新的订单号来查询订单详情以及关联授权应用。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 91909 获取订单详情](https://developer.work.weixin.qq.com/document/path/91909)

### 9. `ProlongTrialAsync` — 延长试用期

- **声明接口**：[IWechatWorkThirdPartyPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/service/prolong_try`

- **请求体**：`ProlongPayToolVersionTrialRequest` — 请求体（`ProlongPayToolVersionTrialRequest`：buyer_corpid 购买方 corpid /
prolong_days 延长天数 / appid 套件应用 id）。
- **返回**：`ProlongPayToolVersionTrialResponse` — 延长后的试用到期时间（try_end_time，UNIX 时间戳）。

**补充说明**

- 延长指定购买方企业的试用有效期。
- 官方约束：buyer_corpid / prolong_days 为官方必填；appid 仅旧套件需要填。

**备注（原文）**

- **第三方应用**开发SDK文档：[path 91913 延长试用期](https://developer.work.weixin.qq.com/document/path/91913)

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/PayTool`](../../../Mud.Wechat.Work/DataModels/PayTool/) 内共 **27** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **27** 个，`SerializerClassName` = `PayTool`。

AOT 源生成上下文：[`PayToolJsonContext.g.cs`](./../../DataModels/Generated/PayToolJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetPayToolInvoiceListResponse` | 应答 | [PayTool/GetPayToolInvoiceListResponse.cs](../../DataModels/PayTool/GetPayToolInvoiceListResponse.cs) | `has_more`, `next_cursor`, `invoice_list` |
| `GetPayToolInvoiceListRequest` | 请求体 | [PayTool/RequestModel/GetPayToolInvoiceListRequest.cs](../../DataModels/PayTool/RequestModel/GetPayToolInvoiceListRequest.cs) | `start_time`, `end_time`, `cursor`, `limit` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `MarkPayToolInvoiceStatusRequest` | 请求体 | [PayTool/RequestModel/MarkPayToolInvoiceStatusRequest.cs](../../DataModels/PayTool/RequestModel/MarkPayToolInvoiceStatusRequest.cs) | `order_id`, `oper_userid`, `invoice_status`, `invoice_note` |
| `CreatePayToolOrderResponse` | 应答 | [PayTool/CreatePayToolOrderResponse.cs](../../DataModels/PayTool/CreatePayToolOrderResponse.cs) | `order_id`, `order_url`, `origin_price`, `paid_price` |
| `CreatePayToolOrderRequest` | 请求体 | [PayTool/RequestModel/CreatePayToolOrderRequest.cs](../../DataModels/PayTool/RequestModel/CreatePayToolOrderRequest.cs) | `business_type`, `custom_corpid`, `pay_type`, `bank_receipt_media_id`, `creator`, `product_list`, `nonce_str`, `ts`, `sig` |
| `ClosePayToolOrderRequest` | 请求体 | [PayTool/RequestModel/ClosePayToolOrderRequest.cs](../../DataModels/PayTool/RequestModel/ClosePayToolOrderRequest.cs) | `order_id`, `nonce_str`, `ts`, `sig` |
| `GetPayToolOrderListResponse` | 应答 | [PayTool/GetPayToolOrderListResponse.cs](../../DataModels/PayTool/GetPayToolOrderListResponse.cs) | `next_cursor`, `has_more`, `pay_order_list` |
| `GetPayToolOrderListRequest` | 请求体 | [PayTool/RequestModel/GetPayToolOrderListRequest.cs](../../DataModels/PayTool/RequestModel/GetPayToolOrderListRequest.cs) | `business_type`, `start_time`, `end_time`, `cursor`, `limit`, `nonce_str`, `ts`, `sig` |
| `GetPayToolOrderDetailResponse` | 应答 | [PayTool/GetPayToolOrderDetailResponse.cs](../../DataModels/PayTool/GetPayToolOrderDetailResponse.cs) | `pay_order` |
| `GetPayToolOrderDetailRequest` | 请求体 | [PayTool/RequestModel/GetPayToolOrderDetailRequest.cs](../../DataModels/PayTool/RequestModel/GetPayToolOrderDetailRequest.cs) | `order_id`, `nonce_str`, `ts`, `sig` |
| `GetPayToolVersionOrderListResponse` | 应答 | [PayTool/GetPayToolVersionOrderListResponse.cs](../../DataModels/PayTool/GetPayToolVersionOrderListResponse.cs) | `order_list` |
| `GetPayToolVersionOrderListRequest` | 请求体 | [PayTool/RequestModel/GetPayToolVersionOrderListRequest.cs](../../DataModels/PayTool/RequestModel/GetPayToolVersionOrderListRequest.cs) | `start_time`, `end_time`, `test_mode` |
| `GetPayToolVersionOrderDetailResponse` | 应答 | [PayTool/GetPayToolVersionOrderDetailResponse.cs](../../DataModels/PayTool/GetPayToolVersionOrderDetailResponse.cs) | — |
| `GetPayToolVersionOrderDetailRequest` | 请求体 | [PayTool/RequestModel/GetPayToolVersionOrderDetailRequest.cs](../../DataModels/PayTool/RequestModel/GetPayToolVersionOrderDetailRequest.cs) | `orderid` |
| `ProlongPayToolVersionTrialResponse` | 应答 | [PayTool/ProlongPayToolVersionTrialResponse.cs](../../DataModels/PayTool/ProlongPayToolVersionTrialResponse.cs) | `try_end_time` |
| `ProlongPayToolVersionTrialRequest` | 请求体 | [PayTool/RequestModel/ProlongPayToolVersionTrialRequest.cs](../../DataModels/PayTool/RequestModel/ProlongPayToolVersionTrialRequest.cs) | `buyer_corpid`, `prolong_days`, `appid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.PayTool;
using Mud.Wechat.Work.DataModels;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());

var service = app.Services.GetRequiredService<IWechatWorkThirdPartyPayToolOrderService>();
var ct = CancellationToken.None;
var response = await service.CloseOrderAsync(new ClosePayToolOrderRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/paytool/close_order");
// 占位值仅用于编译示意；业务值须按 IWechatWorkPayToolOrderService_ThirdParty.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CloseOrderAsync`）；请求体 `ClosePayToolOrderRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkPayToolInvoiceService](./IWechatWorkPayToolInvoiceService.cs)、[IWechatWorkThirdPartyPayToolInvoiceService](./IWechatWorkPayToolInvoiceService_ThirdParty.cs)、[IWechatWorkPayToolOrderService](./IWechatWorkPayToolOrderService.cs)、[IWechatWorkThirdPartyPayToolOrderService](./IWechatWorkPayToolOrderService_ThirdParty.cs)、[IWechatWorkPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService.cs)、[IWechatWorkThirdPartyPayToolVersionSuiteService](./IWechatWorkPayToolVersionSuiteService_ThirdParty.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayToolContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `PayToolEndpoints_ShouldMatchOfficialRoutes`、`PayToolTokenBinding_ShouldSplitProviderAndSuiteRoutes`、`PayToolSignatureFields_ShouldExistOnlyOnOrderFamily`、`PayToolDataModels_ShouldBeRegisteredInJsonContext`、`PayToolDataModels_ShouldLockOfficialContractTraps`、`PayToolVersionFamily_ShouldNotDuplicateAuthorizationEndpoints`、`PayToolSignature_ShouldFollowOfficialAlgorithm`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/PayTool`](../../../Mud.Wechat.Work/DataModels/PayTool/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayToolContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
