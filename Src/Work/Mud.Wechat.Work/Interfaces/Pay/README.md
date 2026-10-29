# Pay 域 · 企业支付

企业微信「企业支付」模块「对外收款记录」域公共 SDK
（获取对外收款记录 + 获取收款项目的商户单号）。 接口：[IWechatWorkInternalPayBillService](./Bill/IWechatWorkInternalPayBillService.cs)、[IWechatWorkPayBillService](./Bill/IWechatWorkPayBillService.cs)、[IWechatWorkProviderPayBillService](./Bill/IWechatWorkProviderPayBillService.cs)、[IWechatWorkThirdPartyPayBillService](./Bill/IWechatWorkThirdPartyPayBillService.cs)、[IWechatWorkInternalPayFundFlowService](./FundFlow/IWechatWorkInternalPayFundFlowService.cs)、[IWechatWorkPayFundFlowService](./FundFlow/IWechatWorkPayFundFlowService.cs)、[IWechatWorkInternalPayMchApplyService](./MchApply/IWechatWorkInternalPayMchApplyService.cs)、[IWechatWorkPayMchApplyService](./MchApply/IWechatWorkPayMchApplyService.cs)、[IWechatWorkInternalPayMerchantService](./Merchant/IWechatWorkInternalPayMerchantService.cs)、[IWechatWorkPayMerchantService](./Merchant/IWechatWorkPayMerchantService.cs)、[IWechatWorkInternalPayOrderService](./Order/IWechatWorkInternalPayOrderService.cs)、[IWechatWorkPayOrderService](./Order/IWechatWorkPayOrderService.cs)、[IWechatWorkInternalPayRefundService](./Refund/IWechatWorkInternalPayRefundService.cs)、[IWechatWorkPayRefundService](./Refund/IWechatWorkPayRefundService.cs)、[IWechatWorkInternalPayTradeBillService](./TradeBill/IWechatWorkInternalPayTradeBillService.cs)、[IWechatWorkPayTradeBillService](./TradeBill/IWechatWorkPayTradeBillService.cs)；注册组 `Pay`；模块枚举 `WechatModule.Pay`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatPayToolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/Pay`](../../../Mud.Wechat.Work/DataModels/Pay/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **15** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkPayBillService` | `GetBillListAsync` | POST | `/cgi-bin/externalpay/get_bill_list` | `AccessToken`（Query），Query `access_token` | 获取对外收款记录 |
| 2 | `IWechatWorkPayBillService` | `GetPaymentInfoAsync` | POST | `/cgi-bin/externalpay/get_payment_info` | `AccessToken`（Query），Query `access_token` | 获取收款项目的商户单号 |
| 3 | `IWechatWorkInternalPayFundFlowService` | `GetFundFlowAsync` | POST | `/cgi-bin/externalpay/get_fund_flow` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取资金流水 |
| 4 | `IWechatWorkInternalPayMchApplyService` | `ApplyMchAsync` | POST | `/cgi-bin/miniapppay/apply_mch` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 提交创建对外收款账户的申请单 |
| 5 | `IWechatWorkInternalPayMchApplyService` | `GetApplymentStatusAsync` | POST | `/cgi-bin/miniapppay/get_applyment_status` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询申请单状态 |
| 6 | `IWechatWorkInternalPayMchApplyService` | `UploadImageAsync` | POST | `/cgi-bin/miniapppay/upload_image` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 提交图片 |
| 7 | `IWechatWorkInternalPayMerchantService` | `GetMerchantAsync` | POST | `/cgi-bin/externalpay/getmerchant` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询商户号详情 |
| 8 | `IWechatWorkInternalPayMerchantService` | `SetMerchantUseScopeAsync` | POST | `/cgi-bin/externalpay/set_mch_use_scope` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 设置商户号使用范围 |
| 9 | `IWechatWorkInternalPayOrderService` | `CreateOrderAsync` | POST | `/cgi-bin/miniapppay/create_order` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 小程序下单（普通支付） |
| 10 | `IWechatWorkInternalPayOrderService` | `GetOrderAsync` | POST | `/cgi-bin/miniapppay/get_order` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询订单（普通支付） |
| 11 | `IWechatWorkInternalPayOrderService` | `CloseOrderAsync` | POST | `/cgi-bin/miniapppay/close_order` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 关闭订单（普通支付） |
| 12 | `IWechatWorkInternalPayOrderService` | `GetSignAsync` | POST | `/cgi-bin/miniapppay/get_sign` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取支付签名（普通支付） |
| 13 | `IWechatWorkInternalPayRefundService` | `ApplyRefundAsync` | POST | `/cgi-bin/miniapppay/refund` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 申请退款 |
| 14 | `IWechatWorkInternalPayRefundService` | `GetRefundDetailAsync` | POST | `/cgi-bin/miniapppay/get_refund_detail` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询退款 |
| 15 | `IWechatWorkInternalPayTradeBillService` | `GetTradeBillAsync` | POST | `/cgi-bin/miniapppay/get_bill` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 交易账单申请 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkInternalPayBillService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayBillService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkProviderPayBillService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyPayBillService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkInternalPayFundFlowService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayFundFlowService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalPayMchApplyService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayMchApplyService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalPayMerchantService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayMerchantService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalPayOrderService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayOrderService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalPayRefundService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayRefundService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalPayTradeBillService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkPayTradeBillService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkInternalPayBillService`

官方对自建应用仅开放与三类应用公共面一致的 2 个端点（继承自
`IWechatWorkPayBillService`）；本接口不新增端点，仅作为
自建应用的类型化契约入口存在（形态对齐 `IWechatWorkInternalKfUpgradeService` 空标记）。
第三方应用见 `IWechatWorkThirdPartyPayBillService`；
服务商代开发见 `IWechatWorkProviderPayBillService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「对外收款 - 可调用接口的应用」中；仅返回应用可见范围内用户的收款记录。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayBillService`

官方对三类应用开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
自建应用见 `IWechatWorkInternalPayBillService`，
第三方应用见 `IWechatWorkThirdPartyPayBillService`，
服务商代开发见 `IWechatWorkProviderPayBillService`。

三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）：
企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

权限口径：自建应用须配置到「对外收款 - 可调用接口的应用」中；
第三方 / 代开发应用须具有「对外收款」权限。

官方约束：仅返回应用可见范围内用户的收款记录；4.1.0 及以上版本新增的部分商户号收款记录
不返回给第三方 / 代开发应用；收款项目的商户单号仅允许获取由应用本身创建的收款项目；
收款记录中的联系人信息（contact_info）第三方应用不可获取；
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderPayBillService`

官方对代开发应用仅开放与三类应用公共面一致的 2 个端点（继承自
`IWechatWorkPayBillService`）；本接口不新增端点，仅作为
代开发应用的类型化契约入口存在（形态对齐 `IWechatWorkProviderKfUpgradeService` 空标记）。
自建应用见 `IWechatWorkInternalPayBillService`；
第三方应用见 `IWechatWorkThirdPartyPayBillService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId）。
官方权限口径：代开发应用须具有「对外收款」权限；仅返回应用可见范围内用户的收款记录；
4.1.0 及以上版本新增的部分商户号收款记录不返回给代开发应用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyPayBillService`

官方对第三方应用仅开放与三类应用公共面一致的 2 个端点（继承自
`IWechatWorkPayBillService`）；本接口不新增端点，仅作为
第三方应用的类型化契约入口存在（形态对齐 `IWechatWorkThirdPartyKfUpgradeService` 空标记）。
自建应用见 `IWechatWorkInternalPayBillService`；
服务商代开发见 `IWechatWorkProviderPayBillService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId）。
官方权限口径：第三方应用须具有「对外收款」权限；仅返回应用可见范围内用户的收款记录；
4.1.0 及以上版本新增的部分商户号收款记录不返回给第三方应用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalPayFundFlowService`

官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
声明于本接口（形态对齐 `IWechatWorkInternalKfKnowledgeService`）。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「对外收款 - 可调用接口的应用」中；仅返回在企业微信开通的商户号资金流水。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayFundFlowService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 1 个端点声明于 `IWechatWorkInternalPayFundFlowService`
（形态对齐 `IWechatWorkKfKnowledgeService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalPayMchApplyService`

官方仅向自建应用开放本域 3 个端点（代开发应用与第三方应用均暂不支持），
全部声明于本接口（形态对齐 `IWechatWorkInternalKfKnowledgeService`）。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「对外收款 - 可调用接口的应用」中。

官方约束：申请单与申请单状态查询均仅针对该应用本身提交的申请单；
同一提现人员最多申请 200 个商户号，同一企业最多申请 2000 个商户号。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayMchApplyService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 3 个端点声明于 `IWechatWorkInternalPayMchApplyService`
（形态对齐 `IWechatWorkKfKnowledgeService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalPayMerchantService`

官方仅向自建应用开放本域 2 个端点（代开发应用与第三方应用均暂不支持），
全部声明于本接口（形态对齐 `IWechatWorkInternalKfKnowledgeService`）。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「对外收款 - 可调用接口的应用」中；范围设置仅对已绑定商户号生效。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayMerchantService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 2 个端点声明于 `IWechatWorkInternalPayMerchantService`
（形态对齐 `IWechatWorkKfKnowledgeService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalPayOrderService`

官方仅向自建应用开放本域 4 个端点（代开发应用与第三方应用均暂不支持），
全部声明于本接口（形态对齐 `IWechatWorkInternalPayMchApplyService`）。

消费应用自身 access_token（路由键 `AccessToken`）。
官方权限口径：须先在企微管理端开通对外收款能力，商户号为「由企业微信生成并下发」的二级商户号。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayOrderService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 4 个端点声明于 `IWechatWorkInternalPayOrderService`
（形态对齐 `IWechatWorkPayMchApplyService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalPayRefundService`

官方仅向自建应用开放本域 2 个端点（代开发应用与第三方应用均暂不支持），
全部声明于本接口（形态对齐 `IWechatWorkInternalPayMchApplyService`）。

消费应用自身 access_token（路由键 `AccessToken`）。
官方权限口径：须先在企微管理端开通对外收款能力，商户号为「由企业微信生成并下发」的二级商户号。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayRefundService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 2 个端点声明于 `IWechatWorkInternalPayRefundService`
（形态对齐 `IWechatWorkPayMchApplyService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalPayTradeBillService`

官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
声明于本接口（形态对齐 `IWechatWorkInternalPayFundFlowService`）。

消费应用自身 access_token（路由键 `AccessToken`）。
官方权限口径：须先在企微管理端开通对外收款能力，商户号为「由企业微信生成并下发」的二级商户号。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkPayTradeBillService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 1 个端点声明于 `IWechatWorkInternalPayTradeBillService`
（形态对齐 `IWechatWorkPayMchApplyService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 11 次）官方业务限制：代开发应用、第三方应用均暂不支持本接口。

### 1. `GetBillListAsync` — 获取对外收款记录

- **声明接口**：[IWechatWorkPayBillService](./Bill/IWechatWorkPayBillService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalpay/get_bill_list`

- **请求体**：`GetPayBillListRequest` — 查询请求体（`GetPayBillListRequest`：
begin_time / end_time（间隔 ≤ 1 个月）/ payee_userid / cursor / limit（≤ 1000））。
- **返回**：`GetPayBillListResponse` — 交易单详情列表（bill_list）与分页游标（next_cursor，无更多数据时不返回）。

**补充说明**

- 获取企业对外收款记录（收款 / 退款交易单详情）， 仅返回应用可见范围内用户的收款记录。
- 官方业务限制：收款记录起止时间间隔不能超过 1 个月； 会过滤收款人不在可见范围中的记录，返回数可能小于 limit；无 next_cursor 时表示数据已全部拉取完。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93667
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93727
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96701

### 2. `GetPaymentInfoAsync` — 获取收款项目的商户单号

- **声明接口**：[IWechatWorkPayBillService](./Bill/IWechatWorkPayBillService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/externalpay/get_payment_info`

- **请求体**：`GetPayPaymentInfoRequest` — 查询请求体（`GetPayPaymentInfoRequest`：payment_id，在发起对外收款时返回）。
- **返回**：`GetPayPaymentInfoResponse` — 收款单号列表（bill_list，每笔支付对应一个收款单号）。

**补充说明**

- 获取对外收款项目中每笔收款的商户单号； 仅允许获取由应用本身创建的收款项目的收款单号列表。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/95944
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/95936
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96702

### 3. `GetFundFlowAsync` — 获取资金流水

- **声明接口**：[IWechatWorkInternalPayFundFlowService](./FundFlow/IWechatWorkInternalPayFundFlowService.cs)
- **官方契约**：`POST /cgi-bin/externalpay/get_fund_flow`

- **请求体**：`GetPayFundFlowRequest` — 查询请求体（`GetPayFundFlowRequest`：
begin_time / end_time（间隔 ≤ 31 天）/ mch_id / cursor / limit（≤ 200））。
- **返回**：`GetPayFundFlowResponse` — 资金流水记录列表（fund_flow_list）与分页游标（next_cursor，无更多数据时不返回）。

**补充说明**

- 获取企业微信开通商户号的资金流水记录（动账明细）。
- 官方业务限制：可拉取不早于 2022 年 12 月 1 日的记录（最长保留 3 年）； 起止时间间隔不能超过 31 天；当日流水在次日上午 11 点后生成； 会过滤操作人不在应用可见范围内的记录，返回条数可能小于 limit。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98100

### 4. `ApplyMchAsync` — 提交创建对外收款账户的申请单

- **声明接口**：[IWechatWorkInternalPayMchApplyService](./MchApply/IWechatWorkInternalPayMchApplyService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/apply_mch`

- **请求体**：`ApplyPayMchRequest` — 进件申请请求体（`ApplyPayMchRequest`：
主体类型 / 营业执照 / 证件信息 / 超级管理员 / 结算账户 / 经营场景等完整材料）。
- **返回**：`ApplyPayMchResponse` — 提交结果（errcode/errmsg 与参数错误详情 error_description）。

**补充说明**

- 企业通过本接口递交材料以创建对外收款账户申请单； 仅能操作该应用本身提交的申请单。
- 官方业务限制：图片类字段均须先经「提交图片」接口获取图片 ID， 且只能使用该应用本身提交的图片；同一提现人员最多申请 200 个商户号， 同一企业最多申请 2000 个商户号。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98973

### 5. `GetApplymentStatusAsync` — 查询申请单状态

- **声明接口**：[IWechatWorkInternalPayMchApplyService](./MchApply/IWechatWorkInternalPayMchApplyService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/get_applyment_status`

- **请求体**：`GetPayApplymentStatusRequest` — 查询请求体（`GetPayApplymentStatusRequest`：out_request_no，1~32 个字符）。
- **返回**：`GetPayApplymentStatusResponse` — 申请单状态（status：applyment_state / sign_state / sign_url / sub_mchid /
audit_detail / account_validation / legal_validation_url）与申请阶段（apply_state）、
签约阶段（real_sign_state）、驳回理由（reject_reason）。

**补充说明**

- 查询已提交的创建对外收款账户申请单状态（申请状态 / 签约状态 / 驳回原因 / 签约与法人验证链接 / 汇款账户验证信息）， 使用提交申请单时填写的 out_request_no 查询，仅能查询该应用本身提交的申请单。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98974

### 6. `UploadImageAsync` — 提交图片

- **声明接口**：[IWechatWorkInternalPayMchApplyService](./MchApply/IWechatWorkInternalPayMchApplyService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/upload_image`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（请求体为 multipart/form-data，由调用方构建<br>`IFormContent` 实现，含 filename / filelength / content-type 信息）。<br>须标 `[MultipartForm]` —— 生成器 3.0.1 对 `[FormContent]` 发射的<br>`GetFormDataContentAsync` 调用在运行时接口上不存在（CS1061），<br>`[MultipartForm]` 才走 `ToHttpContentAsync` 通路<br>（形态对齐 `UploadAttachmentAsync` 先例）。 |
- **返回**：`UploadPayImageResponse` — 上传后得到的图片 ID（open_wx_pay_media_id，30 天后过期）。

**补充说明**

- 上传图片得到图片 ID（open_wx_pay_media_id），用于在提交创建对外收款账户申请单时 填写图片相关字段；图片按应用隔离，不能使用其他应用提交的图片 ID。
- 官方业务限制：图片文件大小应在 5B ~ 2MB 之间，仅支持 JPG/JPEG、PNG、BMP 格式； 图片 ID 距离上传超过 30 天、或已被用于成功提交申请单且所有相关申请单均走完流程时立即失效。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98972

### 7. `GetMerchantAsync` — 查询商户号详情

- **声明接口**：[IWechatWorkInternalPayMerchantService](./Merchant/IWechatWorkInternalPayMerchantService.cs)
- **官方契约**：`POST /cgi-bin/externalpay/getmerchant`

- **请求体**：`GetPayMerchantRequest` — 查询请求体（`GetPayMerchantRequest`：mch_id，不超过 32 字节）。
- **返回**：`GetPayMerchantResponse` — 商户号信息（mch_id / merchant_name / bind_status：1 申请中、2 已绑定、3 已撤销）
与使用范围（allow_use_scope，仅已绑定时返回）。

**补充说明**

- 查询企业已绑定的微信支付商户号信息及使用范围。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93666

### 8. `SetMerchantUseScopeAsync` — 设置商户号使用范围

- **声明接口**：[IWechatWorkInternalPayMerchantService](./Merchant/IWechatWorkInternalPayMerchantService.cs)
- **官方契约**：`POST /cgi-bin/externalpay/set_mch_use_scope`

- **请求体**：`SetPayMerchantUseScopeRequest` — 设置请求体（`SetPayMerchantUseScopeRequest`：
mch_id + allow_use_scope（user / partyid / tagid））。
- **返回**：`WechatWorkResponse` — 设置结果（errcode/errmsg）。

**补充说明**

- 设置企业已绑定微信支付商户号的使用范围，支持按成员、部门或标签设置。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93666
- 官方业务限制：代开发应用、第三方应用均暂不支持本接口；范围设置仅对已绑定商户号生效。

### 9. `CreateOrderAsync` — 小程序下单（普通支付）

- **声明接口**：[IWechatWorkInternalPayOrderService](./Order/IWechatWorkInternalPayOrderService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/create_order`

- **请求体**：`CreatePayOrderRequest` — 下单请求体（`CreatePayOrderRequest`：
appid / mchid / out_trade_no / description / amount / payer / scene_info 等完整下单要素）。
- **返回**：`CreatePayOrderResponse` — 预支付交易会话标识（prepay_id，有效期 2 小时）。

**补充说明**

- 商户系统内部订单号在同一商户号下唯一；返回预支付交易会话标识 prepay_id （有效期 2 小时），配合「获取支付签名」接口供小程序侧拉起支付。
- 官方业务限制：境内商户号仅支持人民币（CNY）；scenekey 用于统计企微成员 发出小程序的交易业绩（展示在「对外收款 - 成员业绩」），不传则不统计。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97322

### 10. `GetOrderAsync` — 查询订单（普通支付）

- **声明接口**：[IWechatWorkInternalPayOrderService](./Order/IWechatWorkInternalPayOrderService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/get_order`

- **请求体**：`GetPayOrderRequest` — 查询请求体（`GetPayOrderRequest`：mchid + out_trade_no）。
- **返回**：`GetPayOrderResponse` — 订单详情（trade_state / trade_state_desc / payer / transaction_id /
amount / promotion_detail 等；trade_state：SUCCESS / REFUND / NOTPAY / CLOSED /
REVOKED / USERPAYING / PAYERROR / ACCEPT）。

**补充说明**

- 查询单笔订单的支付状态与交易详情，用于未收到支付通知、支付接口返回 系统错误或未知状态、以及调关单/撤销前确认支付状态的场景。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97323

### 11. `CloseOrderAsync` — 关闭订单（普通支付）

- **声明接口**：[IWechatWorkInternalPayOrderService](./Order/IWechatWorkInternalPayOrderService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/close_order`

- **请求体**：`ClosePayOrderRequest` — 关单请求体（`ClosePayOrderRequest`：mchid + out_trade_no）。
- **返回**：`WechatWorkResponse` — 关闭结果（errcode/errmsg）。

**补充说明**

- 关闭指定订单，订单生成后不能马上调用关单接口，最短调用时间间隔为 5 分钟。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97324
- 官方业务限制：订单生成后不能马上调用关单接口，最短调用时间间隔为 5 分钟； / 代开发应用、第三方应用均暂不支持本接口。

### 12. `GetSignAsync` — 获取支付签名（普通支付）

- **声明接口**：[IWechatWorkInternalPayOrderService](./Order/IWechatWorkInternalPayOrderService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/get_sign`

- **请求体**：`GetPaySignRequest` — 签名请求体（`GetPaySignRequest`：
appid / prepay_id / nonce / timestamp，sign_type 可选）。
- **返回**：`GetPaySignResponse` — 支付签名值（pay_sign）。

**补充说明**

- 使用字段 appid、timestamp、nonce、prepay_id 计算支付签名值（pay_sign）， 供小程序侧拉起支付时使用。
- 官方业务限制：仅支持下单两小时内的 prepay_id；随机字符串不长于 32 位 且仅支持数字、大小写字母；签名方式默认 RSA，仅支持 RSA。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98130

### 13. `ApplyRefundAsync` — 申请退款

- **声明接口**：[IWechatWorkInternalPayRefundService](./Refund/IWechatWorkInternalPayRefundService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/refund`

- **请求体**：`ApplyPayRefundRequest` — 退款请求体（`ApplyPayRefundRequest`：
mchid / appid / out_trade_no / out_refund_no / amount，reason 与 funds_account 可选）。
- **返回**：`ApplyPayRefundResponse` — 退款受理结果（out_refund_no / amount / promotion_detail）。

**补充说明**

- 提交退款申请，支持单笔交易分多次退款；接口返回仅代表受理情况， 退款是否成功须通过「查询退款」接口或退款通知回调确认。
- 官方业务限制：交易时间超过一年的订单无法提交退款； 总退款金额不能超过订单金额，每个支付订单的部分退款次数不能超过 50 次； 退款失败重试须沿用原退款单号（同一退款单号多次请求只退一笔）； 请求频率限制 150 qps，单笔订单 1 qpm（每分钟申请退款不超过 1 次）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97333

### 14. `GetRefundDetailAsync` — 查询退款

- **声明接口**：[IWechatWorkInternalPayRefundService](./Refund/IWechatWorkInternalPayRefundService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/get_refund_detail`

- **请求体**：`GetPayRefundDetailRequest` — 查询请求体（`GetPayRefundDetailRequest`：mchid + out_refund_no）。
- **返回**：`GetPayRefundDetailResponse` — 退款单详情（refund_id / status：SUCCESS / CLOSE / PROCESSING / ABNORMAL、
channel / user_received_account / amount / promotion_detail 等）。

**补充说明**

- 按商户退款单号查询退款单的受理与到账状态。
- 官方业务限制：退款有一定延时，用零钱支付的退款 20 分钟内到账， 银行卡支付的退款 3 个工作日后重新查询退款状态。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97352

### 15. `GetTradeBillAsync` — 交易账单申请

- **声明接口**：[IWechatWorkInternalPayTradeBillService](./TradeBill/IWechatWorkInternalPayTradeBillService.cs)
- **官方契约**：`POST /cgi-bin/miniapppay/get_bill`

- **请求体**：`GetPayTradeBillRequest` — 申请请求体（`GetPayTradeBillRequest`：
bill_date（yyyy-MM-dd）/ mchid，bill_type 与 tar_type 可选）。
- **返回**：`GetPayTradeBillResponse` — 账单下载信息（hash_type / hash_value / download_url / auth）。

**补充说明**

- 申请指定商户号、指定日期的交易账单文件下载信息。
- 官方业务限制：仅支持三个月内的账单下载申请；返回的下载地址 30 秒内有效； 下载账单文件时须以响应中的 auth 作为 https 校验的 Authorization 请求头； 账单文件含明细与汇总四部分，字段以英文逗号分隔。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98115

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/Pay`](../../../Mud.Wechat.Work/DataModels/Pay/) 内共 **60** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **60** 个，`SerializerClassName` = `Pay`。

AOT 源生成上下文：[`PayJsonContext.g.cs`](./../../DataModels/Generated/PayJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetPayBillListResponse` | 应答 | [Pay/GetPayBillListResponse.cs](../../DataModels/Pay/GetPayBillListResponse.cs) | `next_cursor`, `bill_list`, `transaction_id`, `bill_type`, `trade_state`, `pay_time`, `out_trade_no`, `out_refund_no`, `external_userid`, `total_fee`, `payee_userid`, `payment_type` … |
| `GetPayBillListRequest` | 请求体 | [Pay/RequestModel/GetPayBillListRequest.cs](../../DataModels/Pay/RequestModel/GetPayBillListRequest.cs) | `begin_time`, `end_time`, `payee_userid`, `cursor`, `limit` |
| `GetPayPaymentInfoResponse` | 应答 | [Pay/GetPayPaymentInfoResponse.cs](../../DataModels/Pay/GetPayPaymentInfoResponse.cs) | `bill_list`, `out_trade_no` |
| `GetPayPaymentInfoRequest` | 请求体 | [Pay/RequestModel/GetPayPaymentInfoRequest.cs](../../DataModels/Pay/RequestModel/GetPayPaymentInfoRequest.cs) | `payment_id` |
| `GetPayFundFlowResponse` | 应答 | [Pay/GetPayFundFlowResponse.cs](../../DataModels/Pay/GetPayFundFlowResponse.cs) | `next_cursor`, `fund_flow_list`, `timestamp`, `request_no`, `transaction_type`, `fund_flow_type`, `transaction_amount`, `account_balance`, `out_trade_no`, `mch_id`, `operator_userid`, `group_list` … |
| `GetPayFundFlowRequest` | 请求体 | [Pay/RequestModel/GetPayFundFlowRequest.cs](../../DataModels/Pay/RequestModel/GetPayFundFlowRequest.cs) | `begin_time`, `end_time`, `mch_id`, `cursor`, `limit` |
| `ApplyPayMchResponse` | 应答 | [Pay/ApplyPayMchResponse.cs](../../DataModels/Pay/ApplyPayMchResponse.cs) | `error_description` |
| `ApplyPayMchRequest` | 请求体 | [Pay/RequestModel/ApplyPayMchRequest.cs](../../DataModels/Pay/RequestModel/ApplyPayMchRequest.cs) | `out_request_no`, `organization_type`, `business_license_info`, `finance_institution_info`, `merchant_short_name`, `id_card_info`, `owner`, `ubo_info`, `contact_info`, `account_info`, `sales_scene_info`, `business_id` … |
| `GetPayApplymentStatusResponse` | 应答 | [Pay/GetPayApplymentStatusResponse.cs](../../DataModels/Pay/GetPayApplymentStatusResponse.cs) | `status`, `apply_state`, `real_sign_state`, `reject_reason`, `applyment_state`, `applyment_state_desc`, `sign_state`, `sign_url`, `sub_mchid`, `audit_detail`, `account_validation`, `legal_validation_url` … |
| `GetPayApplymentStatusRequest` | 请求体 | [Pay/RequestModel/GetPayApplymentStatusRequest.cs](../../DataModels/Pay/RequestModel/GetPayApplymentStatusRequest.cs) | `out_request_no` |
| `UploadPayImageResponse` | 应答 | [Pay/UploadPayImageResponse.cs](../../DataModels/Pay/UploadPayImageResponse.cs) | `open_wx_pay_media_id` |
| `GetPayMerchantResponse` | 应答 | [Pay/GetPayMerchantResponse.cs](../../DataModels/Pay/GetPayMerchantResponse.cs) | `mch_id`, `merchant_name`, `bind_status`, `allow_use_scope`, `user`, `partyid`, `tagid` |
| `GetPayMerchantRequest` | 请求体 | [Pay/RequestModel/GetPayMerchantRequest.cs](../../DataModels/Pay/RequestModel/GetPayMerchantRequest.cs) | `mch_id` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `SetPayMerchantUseScopeRequest` | 请求体 | [Pay/RequestModel/SetPayMerchantUseScopeRequest.cs](../../DataModels/Pay/RequestModel/SetPayMerchantUseScopeRequest.cs) | `mch_id`, `allow_use_scope` |
| `CreatePayOrderResponse` | 应答 | [Pay/CreatePayOrderResponse.cs](../../DataModels/Pay/CreatePayOrderResponse.cs) | `prepay_id` |
| `CreatePayOrderRequest` | 请求体 | [Pay/RequestModel/CreatePayOrderRequest.cs](../../DataModels/Pay/RequestModel/CreatePayOrderRequest.cs) | `appid`, `mchid`, `out_trade_no`, `description`, `scenekey`, `amount`, `payer`, `time_expire`, `attach`, `goods_tag`, `detail`, `scene_info` … |
| `GetPayOrderResponse` | 应答 | [Pay/GetPayOrderResponse.cs](../../DataModels/Pay/GetPayOrderResponse.cs) | `mchid`, `out_trade_no`, `trade_type`, `trade_state`, `trade_state_desc`, `payer`, `transaction_id`, `bank_type`, `attach`, `success_time`, `amount`, `scene_info` … |
| `GetPayOrderRequest` | 请求体 | [Pay/RequestModel/GetPayOrderRequest.cs](../../DataModels/Pay/RequestModel/GetPayOrderRequest.cs) | `mchid`, `out_trade_no` |
| `ClosePayOrderRequest` | 请求体 | [Pay/RequestModel/ClosePayOrderRequest.cs](../../DataModels/Pay/RequestModel/ClosePayOrderRequest.cs) | `mchid`, `out_trade_no` |
| `GetPaySignResponse` | 应答 | [Pay/GetPaySignResponse.cs](../../DataModels/Pay/GetPaySignResponse.cs) | `pay_sign` |
| `GetPaySignRequest` | 请求体 | [Pay/RequestModel/GetPaySignRequest.cs](../../DataModels/Pay/RequestModel/GetPaySignRequest.cs) | `appid`, `prepay_id`, `sign_type`, `nonce`, `timestamp` |
| `ApplyPayRefundResponse` | 应答 | [Pay/ApplyPayRefundResponse.cs](../../DataModels/Pay/ApplyPayRefundResponse.cs) | `out_refund_no`, `amount`, `promotion_detail`, `refund`, `payer_refund`, `discount_refund`, `currency`, `promotion_id`, `scope`, `type`, `refund_amount` |
| `ApplyPayRefundRequest` | 请求体 | [Pay/RequestModel/ApplyPayRefundRequest.cs](../../DataModels/Pay/RequestModel/ApplyPayRefundRequest.cs) | `mchid`, `appid`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `funds_account`, `refund`, `total`, `currency` |
| `GetPayRefundDetailResponse` | 应答 | [Pay/GetPayRefundDetailResponse.cs](../../DataModels/Pay/GetPayRefundDetailResponse.cs) | `refund_id`, `out_refund_no`, `transaction_id`, `out_trade_no`, `channel`, `user_received_account`, `success_time`, `create_time`, `status`, `amount`, `promotion_detail` |
| `GetPayRefundDetailRequest` | 请求体 | [Pay/RequestModel/GetPayRefundDetailRequest.cs](../../DataModels/Pay/RequestModel/GetPayRefundDetailRequest.cs) | `mchid`, `out_refund_no` |
| `GetPayTradeBillResponse` | 应答 | [Pay/GetPayTradeBillResponse.cs](../../DataModels/Pay/GetPayTradeBillResponse.cs) | `hash_type`, `hash_value`, `download_url`, `auth` |
| `GetPayTradeBillRequest` | 请求体 | [Pay/RequestModel/GetPayTradeBillRequest.cs](../../DataModels/Pay/RequestModel/GetPayTradeBillRequest.cs) | `bill_date`, `mchid`, `bill_type`, `tar_type` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Pay;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalPayMchApplyService>();
var ct = CancellationToken.None;
var response = await service.ApplyMchAsync(new ApplyPayMchRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/miniapppay/apply_mch");
// 占位值仅用于编译示意；业务值须按 IWechatWorkInternalPayMchApplyService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyMchAsync`）；请求体 `ApplyPayMchRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkInternalPayBillService](./Bill/IWechatWorkInternalPayBillService.cs)、[IWechatWorkPayBillService](./Bill/IWechatWorkPayBillService.cs)、[IWechatWorkProviderPayBillService](./Bill/IWechatWorkProviderPayBillService.cs)、[IWechatWorkThirdPartyPayBillService](./Bill/IWechatWorkThirdPartyPayBillService.cs)、[IWechatWorkInternalPayFundFlowService](./FundFlow/IWechatWorkInternalPayFundFlowService.cs)、[IWechatWorkPayFundFlowService](./FundFlow/IWechatWorkPayFundFlowService.cs)、[IWechatWorkInternalPayMchApplyService](./MchApply/IWechatWorkInternalPayMchApplyService.cs)、[IWechatWorkPayMchApplyService](./MchApply/IWechatWorkPayMchApplyService.cs)、[IWechatWorkInternalPayMerchantService](./Merchant/IWechatWorkInternalPayMerchantService.cs)、[IWechatWorkPayMerchantService](./Merchant/IWechatWorkPayMerchantService.cs)、[IWechatWorkInternalPayOrderService](./Order/IWechatWorkInternalPayOrderService.cs)、[IWechatWorkPayOrderService](./Order/IWechatWorkPayOrderService.cs)、[IWechatWorkInternalPayRefundService](./Refund/IWechatWorkInternalPayRefundService.cs)、[IWechatWorkPayRefundService](./Refund/IWechatWorkPayRefundService.cs)、[IWechatWorkInternalPayTradeBillService](./TradeBill/IWechatWorkInternalPayTradeBillService.cs)、[IWechatWorkPayTradeBillService](./TradeBill/IWechatWorkPayTradeBillService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatPayToolContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayBillEndpoints_ShouldMatchOfficialRoutes`、`PayMerchantEndpoints_ShouldMatchOfficialRoutes`、`PayFundFlowEndpoints_ShouldMatchOfficialRoutes`、`PayMchApplyEndpoints_ShouldMatchOfficialRoutes`、`PayOrderEndpoints_ShouldMatchOfficialRoutes`、`PayRefundEndpoints_ShouldMatchOfficialRoutes`、`PayTradeBillEndpoints_ShouldMatchOfficialRoutes`、`PayBillInterfaceHierarchy_ShouldConvergeOnAbstractParentWithPayRegistry`、`PaySingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent`、`PayTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`PayDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` … |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/Pay`](../../../Mud.Wechat.Work/DataModels/Pay/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatPayToolContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
