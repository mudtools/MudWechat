# MerchantRiskManage 域 · 商户风险管理

微信支付「商户风险管理」域 SDK（**11 端点**：风险订单同步 4 + 风险共建 3 + 违规通知 URL 四件套 4）。 接口：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)；注册组 `MerchantRiskManage`；模块枚举 `PayModule.MerchantRiskManage`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMerchantRiskManageContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantRiskManageContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayMerchantServiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantServiceContractGuards.cs)、[WechatPayMerchantSettlementContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantSettlementContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MerchantRiskManage`](../../DataModels/MerchantRiskManage)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMerchantRiskManageService = 11；按 HTTP 特性实计 **11** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantRiskManageService` | `QueryRiskTradesByPartnerAsync` | POST | `/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades` | 商户证书 / APIv3 签名 | 「查询风险订单」服务商形态：按商户与时间区间分页拉取风险订单列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013894221… |
| 2 | `IWechatPayMerchantRiskManageService` | `QueryRiskTradesByMchAsync` | POST | `/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades` | 商户证书 / APIv3 签名 | 「查询风险订单」普通商户形态：与 `QueryRiskTradesByPartnerAsync` **同方法同路由**，标识字段换成 `mchid`。<br>官方文档：https://p… |
| 3 | `IWechatPayMerchantRiskManageService` | `RecvDisposeResultByPartnerAsync` | POST | `/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result` | 商户证书 / APIv3 签名 | 「回传处置结果」服务商形态：把商户侧对某笔风险订单的最终判定与处置结论回传微信。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401… |
| 4 | `IWechatPayMerchantRiskManageService` | `RecvDisposeResultByMchAsync` | POST | `/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result` | 商户证书 / APIv3 签名 | 「回传处置结果」普通商户形态：与 `RecvDisposeResultByPartnerAsync` **同方法同路由**，应答类型共用。<br>官方文档：https://pay.wei… |
| 5 | `IWechatPayMerchantRiskManageService` | `ReportTradeUnionInformationAsync` | POST | `/v3/merchant-risk-manage/report-trade-union-information` | 商户证书 / APIv3 签名 | 「上报订单关联信息」（风险共建）：把订单侧的风控要素（含加密的手机号与证件后 6 位）上报微信以换取更准的风险判定。<br>官方文档：https://pay.weixin.qq.com/… |
| 6 | `IWechatPayMerchantRiskManageService` | `QueryTradeRiskInformationAsync` | GET | `/v3/merchant-risk-manage/trade-risk-information` | 商户证书 / APIv3 签名 | 「查询风险信息」：按商户订单号**单笔**查询微信侧给出的风险系数与处置手段（无列表、无分页）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/par… |
| 7 | `IWechatPayMerchantRiskManageService` | `ReportTradeRiskResultAsync` | POST | `/v3/merchant-risk-manage/trade-risk-result` | 商户证书 / APIv3 签名 | 「处理结果回传」（风险共建）：把商户或从业机构侧执行完的处置手段回传微信。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401246… |
| 8 | `IWechatPayMerchantRiskManageService` | `CreateViolationNotificationAsync` | POST | `/v3/merchant-risk-manage/violation-notifications` | 商户证书 / APIv3 签名 | 「创建商户违规通知回调地址」：登记接收违规处罚 / 拦截通知的回调 URL。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40124… |
| 9 | `IWechatPayMerchantRiskManageService` | `UpdateViolationNotificationUrlAsync` | PUT | `/v3/merchant-risk-manage/violation-notifications` | 商户证书 / APIv3 签名 | 「修改商户违规通知回调地址」：**PUT** 同一路由，成功应答是 **200 + `{notify_url}`（不是 204）**。<br>官方文档：https://pay.weixi… |
| 10 | `IWechatPayMerchantRiskManageService` | `QueryViolationNotificationAsync` | GET | `/v3/merchant-risk-manage/violation-notifications` | 商户证书 / APIv3 签名 | 「查询商户违规通知回调地址」：无参数的 GET，回当前生效的 `notify_url`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner… |
| 11 | `IWechatPayMerchantRiskManageService` | `DeleteViolationNotificationAsync` | DELETE | `/v3/merchant-risk-manage/violation-notifications` | 商户证书 / APIv3 签名 | 「删除商户违规通知回调地址」：**DELETE** 同一路由，成功应答是 **204 No Content（无包体）**。<br>官方文档：https://pay.weixin.qq.c… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（2026-10-14 逐页核验，两轮全覆盖）：查询风险订单
https://pay.weixin.qq.com/doc/v3/partner/4013894221（服务商）/
https://pay.weixin.qq.com/doc/v3/merchant/4013894247（普通商户）、回传处置结果
https://pay.weixin.qq.com/doc/v3/partner/4013894227 /
https://pay.weixin.qq.com/doc/v3/merchant/4013894250、上报订单关联信息
https://pay.weixin.qq.com/doc/v3/partner/4012692160、查询风险信息
https://pay.weixin.qq.com/doc/v3/partner/4012467535、处理结果回传
https://pay.weixin.qq.com/doc/v3/partner/4012467540、违规通知回调地址四支
https://pay.weixin.qq.com/doc/v3/partner/4012471333（创建）/
https://pay.weixin.qq.com/doc/v3/partner/4012471330（修改）/
https://pay.weixin.qq.com/doc/v3/partner/4012471327（查询）/
https://pay.weixin.qq.com/doc/v3/partner/4012471334（删除）。

**11 个方法但只有 9 条 (方法, 路由)**：风险订单两支与处置结果两支各自「服务商面 / 普通商户面
同方法同路由、字段表不同」⇒ 同路由多方法（与消息推送域「每个 msgtype 一个方法同路由」同形），
端点计数按方法计、路由计数按 (方法, 路由) 去重（守卫 MRB-B1 两个数都锁）。

**本域是支付线首次出现非 GET/POST 动词的面**：**PUT**（修改通知地址）与
**DELETE**（删除通知地址）各一支。二者对「204 还是 JSON」的答复相反：
同一条 `/violation-notifications` 上 PUT/POST/GET 都是 **200 + `{notify_url}`**，
只有 DELETE 是 **204 无包体** ⇒ 不得按「写操作都 204」的直觉收口（守卫 MRB-B1 逐支断言）。

**四套商户标识并存**：`mchid`（普通商户面）/ `sp_mchid`+`sub_mchid`（服务商面）/
`sp_mchid`+`acquiring_bank_id`+`channel_id`+`sub_mchid`（风险共建的从业机构与渠道面）/
`sub_orders[].mch_id`（合单子单，官方此处**带下划线**）⇒ 照抄任一别域的标识写法都会静默错参。

**同名 `risk_type` 有两套类型**：风险订单同步面是 **integer**（1~4），
风险共建面是 **string**（5 值）⇒ 两族 DTO 与常量类各一套，跨族赋值即错（守卫 MRB-B3）。

**官方拼写陷阱照页**：`risk_discove_time`（缺 r）、`same_deviced`（多 d）——
本域两处误拼均在服务商面页内逐字核得，修正拼写会静默丢字段。

**敏感字段加密**：`report-trade-union-information` 的 `phone` 与
`certificates_number`（身份证后 6 位）须以微信支付公钥（推荐）或平台证书加密，
该页 `Wechatpay-Serial` 是**参数表行**（必填）；本域其余页面均无该表行
（违规通知四页仅在示例代码里出现）⇒ 加密头由传输层与宿主协同，DTO 只承载密文字符串。

**频率限制**：本域**全部页面均未印端点级数字**（两轮复核一致），只有 429
`FREQUENCY_LIMITED`（`trade-risk-information` / `trade-risk-result` 两页另列
`RATELIMIT_EXCEEDED`，官方文案「请降低频率后重试」）⇒ 注释不得凭第三方 SDK 补数。

**业务错误码**：`PARAM_ERROR`(400) / `INVALID_REQUEST`(400) / `SIGN_ERROR`(401) /
`NO_AUTH`(403) / `MCH_NOT_EXISTS`(404) / `NOT_FOUND`(404) / `ORDER_NOT_EXIST`(404) /
`ALREADY_EXISTS`(409，仅创建通知地址支) / `SYSTEM_ERROR`(500) / `FREQUENCY_LIMITED`(429)，
原样透传至 `Code`。

**本域「查询风险信息」是单查不是列表**：官方页正文只按 `out_trade_no` 查一笔、
应答无数组节点（方案 §2.7 已勘误该行的转述），列表形态在 `query-risk-trades`（`page_index` /
`page_size` 整型页码，**不是** `offset`/`limit` 游标）。

**本域不含「消费者投诉」与「商户风险规则」侧栏组**：投诉族落 `PayModule.MerchantService`，
而支付分风控（`/v3/pay-score`）、服务商支付分（`IWechatPayPartnerPayScoreService`）、
银行面风控（`IWechatPayBankPayScoreService`）是各自独立的域，路由与字段均不互通。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，
本域**零 `[Token]`**（支付线以商户私钥签名，与公众平台令牌面无关，守卫 PAY-B1）。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `QueryRiskTradesByPartnerAsync` — 「查询风险订单」服务商形态：按商户与时间区间分页拉取风险订单列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013894221。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/ec-trade-risk/query-risk-trades`

- **请求体**：`MerchantRiskManagePartnerRiskTradeQueryRequest` — 请求体，字段见 `MerchantRiskManagePartnerRiskTradeQueryRequest`。
- **返回**：`MerchantRiskManagePartnerRiskTradeQueryResponse` — 总数与风险订单列表，见 `MerchantRiskManagePartnerRiskTradeQueryResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades`； / 支持商户【普通服务商】；无 path / query 参数。
- **官方把每个字段都标选填**（含两个商户标识），但描述内注明「若为子商户调用，须传入对应的服务商号」 / ⇒ 条件必填交官方判定。
- **分页是整型页码** `page_index` / `page_size`，不是 `offset` / `limit`。

### 2. `QueryRiskTradesByMchAsync` — 「查询风险订单」普通商户形态：与 `QueryRiskTradesByPartnerAsync` **同方法同路由**，标识字段换成 `mchid`。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4013894247。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/ec-trade-risk/query-risk-trades`

- **请求体**：`MerchantRiskManageMchRiskTradeQueryRequest` — 请求体，字段见 `MerchantRiskManageMchRiskTradeQueryRequest`。
- **返回**：`MerchantRiskManageMchRiskTradeQueryResponse` — 总数与风险订单列表（条目内**无** `sp_mchid`），见 `MerchantRiskManageMchRiskTradeQueryResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades`； / 支持商户【普通商户】。
- **两形态请求表与应答表都不相同**（服务商多 `sp_mchid`+`sub_mchid`，应答条目多 `sp_mchid`） / ⇒ 按「表不同则分建」各建类型，请求 DTO 不可互换（守卫 MRB-B6）。

### 3. `RecvDisposeResultByPartnerAsync` — 「回传处置结果」服务商形态：把商户侧对某笔风险订单的最终判定与处置结论回传微信。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013894227。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result`

- **请求体**：`MerchantRiskManagePartnerDisposeResultRequest` — 请求体，字段见 `MerchantRiskManagePartnerDisposeResultRequest`。
- **返回**：`MerchantRiskManageDisposeResultResponse` — 单字段 `rsp_msg`，见 `MerchantRiskManageDisposeResultResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result`； / 支持商户【普通服务商】。
- **`out_trade_no` 是本域唯一必填**（string(64)）⇒ 缺它即 `PARAM_ERROR`。
- **`disp_conclusion` 是 `array[integer]`**（1-未处置,2-拦截订单,3-关闭店铺）， / 不是 string 枚举。
- **`refund_amount` 官方只写「退款金额」，两轮复核均未印单位与区间** / ⇒ 本 SDK 原样透传 integer，宿主勿自行按「分」换算。

### 4. `RecvDisposeResultByMchAsync` — 「回传处置结果」普通商户形态：与 `RecvDisposeResultByPartnerAsync` **同方法同路由**，应答类型共用。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4013894250。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result`

- **请求体**：`MerchantRiskManageMchDisposeResultRequest` — 请求体，字段见 `MerchantRiskManageMchDisposeResultRequest`。
- **返回**：`MerchantRiskManageDisposeResultResponse` — 与服务商形态**同一应答类型**，见 `MerchantRiskManageDisposeResultResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result`； / 支持商户【普通商户】。
- **请求分建、应答共用**：官方两页应答表同为单字段 `rsp_msg` string(1024)，零差异 ⇒ 归并 / （守卫 MRB-B6 断言两方法返回同一类型）。

### 5. `ReportTradeUnionInformationAsync` — 「上报订单关联信息」（风险共建）：把订单侧的风控要素（含加密的手机号与证件后 6 位）上报微信以换取更准的风险判定。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012692160。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/report-trade-union-information`

- **请求体**：`MerchantRiskManageTradeUnionReportRequest` — 请求体，字段见 `MerchantRiskManageTradeUnionReportRequest`。
- **返回**：`MerchantRiskManageTradeUnionReportResponse` — 回显五项商户标识，见 `MerchantRiskManageTradeUnionReportResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/report-trade-union-information`； / 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】。
- **`Wechatpay-Serial` 在本支是参数表行（必填）**：`phone` 与 / `certificates_number`（身份证后 6 位）须加密 ⇒ 密文由宿主产出，SDK 不解密也不转换。
- **应答无状态字段**：本支结果以 HTTP 码 + `code`/`message` 判定。
- **六个整型字段官方未给取值字典**（`risk_level` / `phone_from` / / `high_risk_goods` / `recharge_fields.account_type` / `living_expenses_fields.type`） / ⇒ 按 `int?` 透传，注释不补第三方数值。

### 6. `QueryTradeRiskInformationAsync` — 「查询风险信息」：按商户订单号**单笔**查询微信侧给出的风险系数与处置手段（无列表、无分页）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467535。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`GET /v3/merchant-risk-manage/trade-risk-information`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outTradeNo` | Query `out_trade_no` | `string` | 商户订单号（官方 query `out_trade_no`，**必填** string(64)）。 |
| `spMchId` | Query `sp_mchid` | `string?` | 服务商商户号（官方 query `sp_mchid`，选填 string(32)）。 |
| `acquiringBankId` | Query `acquiring_bank_id` | `string?` | 从业机构号（官方 query `acquiring_bank_id`，选填 string(32)）。 |
| `channelId` | Query `channel_id` | `string?` | 渠道号（官方 query `channel_id`，选填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string?` | 子商户号（官方 query `sub_mchid`，官方标选填、描述内注明服务商/从业机构/渠道商调用时必填，string(32)）。 |
| `complaintsInformation` | Query `complaints_information` | `string?` | 投诉信息（官方 query `complaints_information`，选填 string(255)）。 |
- **返回**：`MerchantRiskManageTradeRiskInfoResponse` — 风险系数与处置手段，见 `MerchantRiskManageTradeRiskInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/merchant-risk-manage/trade-risk-information`； / 六个参数**全部在 query**（本域唯一纯 query 支）。
- **`out_trade_no` 是这支唯一必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **本支不是列表接口**：官方正文按单笔单号查，页内零个数组节点 / （方案 §2.7 已记录该行的转述错判）⇒ 勿按 `query-risk-trades` 的分页形态建 DTO。
- **429 有两支码**：`FREQUENCY_LIMITED` 与 `RATELIMIT_EXCEEDED`（官方文案「请降低频率后重试」）。

### 7. `ReportTradeRiskResultAsync` — 「处理结果回传」（风险共建）：把商户或从业机构侧执行完的处置手段回传微信。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012467540。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/trade-risk-result`

- **请求体**：`MerchantRiskManageTradeRiskResultRequest` — 请求体，字段见 `MerchantRiskManageTradeRiskResultRequest`。
- **返回**：`MerchantRiskManageTradeRiskResultResponse` — 回显标识与处置结论，见 `MerchantRiskManageTradeRiskResultResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/trade-risk-result`； / 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】。
- **`punish_type` 必填**（10 值），`out_trade_no` 反而**选填** ⇒ 与直觉相反，照页。
- **应答多出 `punish_description`**（请求侧是 `additional_punish_type`）⇒ 请求与应答类型不共用。

### 8. `CreateViolationNotificationAsync` — 「创建商户违规通知回调地址」：登记接收违规处罚 / 拦截通知的回调 URL。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471333。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`POST /v3/merchant-risk-manage/violation-notifications`

- **请求体**：`MerchantRiskManageViolationNotificationRequest` — 请求体（单字段 `notify_url`），见 `MerchantRiskManageViolationNotificationRequest`。
- **返回**：`MerchantRiskManageViolationNotificationResponse` — 回显 `notify_url`（HTTP 200 + JSON），见 `MerchantRiskManageViolationNotificationResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-risk-manage/violation-notifications`； / 支持商户【普通服务商】【从业机构（银行）】【从业机构（支付机构）】【渠道商】。
- **已设置过再创建报 `409 ALREADY_EXISTS`** ⇒ 幂等重放要走 / `UpdateViolationNotificationUrlAsync`，不要捕获 409 当成功。
- **回调 URL 口径**（官方原文）：外网可访问、**不得携带参数**、公网域名必须 HTTPS（专线可用 HTTP）， / 通知应答须验签。

### 9. `UpdateViolationNotificationUrlAsync` — 「修改商户违规通知回调地址」：**PUT** 同一路由，成功应答是 **200 + `{notify_url}`（不是 204）**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471330。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`PUT /v3/merchant-risk-manage/violation-notifications`

- **请求体**：`MerchantRiskManageViolationNotificationRequest` — 请求体（单字段 `notify_url`），见 `MerchantRiskManageViolationNotificationRequest`。
- **返回**：`MerchantRiskManageViolationNotificationResponse` — 回显 `notify_url`，见 `MerchantRiskManageViolationNotificationResponse`。

**备注（原文）**

- **官方契约**：**PUT** `/v3/merchant-risk-manage/violation-notifications`（支付线**第一支 PUT**）。
- **与创建支共用请求与应答类型**：官方两页请求表、应答表逐字段相同（守卫 MRB-B6）。
- **未设置过时修改报 `404 NOT_FOUND`（官方文案「更新的通知回调不存在」）** / ⇒ 本支不是 upsert，须先创建。

### 10. `QueryViolationNotificationAsync` — 「查询商户违规通知回调地址」：无参数的 GET，回当前生效的 `notify_url`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471327。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`GET /v3/merchant-risk-manage/violation-notifications`
- **返回**：`MerchantRiskManageViolationNotificationResponse` — 回 `notify_url`（选填，未配置时官方可不返回该字段），见 `MerchantRiskManageViolationNotificationResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/merchant-risk-manage/violation-notifications`；无 path / query 参数。
- **「没配过」有两种表现**：应答字段缺失**或** `404 NOT_FOUND` ⇒ 宿主两种都要接得住。

### 11. `DeleteViolationNotificationAsync` — 「删除商户违规通知回调地址」：**DELETE** 同一路由，成功应答是 **204 No Content（无包体）**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471334。

- **声明接口**：[IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs)
- **官方契约**：`DELETE /v3/merchant-risk-manage/violation-notifications`
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**DELETE** `/v3/merchant-risk-manage/violation-notifications`（支付线**第一支 DELETE**）； / 请求侧只有 `Authorization` 与 `Accept` 两个头，无包体、无参数。
- **204 无包体 ⇒ 返回裸 `Task`**：本域唯一无 JSON 应答的端点， / 但同路由的 PUT 是 200 + JSON ⇒ 「写操作都 204」在本域不成立（守卫 MRB-B1 断言返回类型）。
- **未设置时删除报 `404 NOT_FOUND`**；删除后微信停止推送违规通知。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MerchantRiskManage`](../../DataModels/MerchantRiskManage) 内共 **19** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **19** 个，`SerializerClassName` = `MerchantRiskManage`。

AOT 源生成上下文：[`MerchantRiskManageJsonContext.g.cs`](./../../DataModels/Generated/MerchantRiskManageJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantRiskManagePartnerRiskTradeQueryResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManagePartnerRiskTradeQueryRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManageMchRiskTradeQueryResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManageMchRiskTradeQueryRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManageDisposeResultResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManagePartnerDisposeResultRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManageMchDisposeResultRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageRiskTradeDtos.cs) | `sp_mchid`, `sub_mchid`, `start_time`, `end_time`, `page_index`, `page_size`, `mchid`, `total_num`, `risk_trade_list`, `out_trade_no`, `risk_type`, `risk_level` … |
| `MerchantRiskManageTradeUnionReportResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageTradeUnionDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageTradeUnionDtos.cs) | `sp_mchid`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `out_trade_no`, `openid`, `phone`, `certificates_number`, `client_ip`, `risk_level`, `line_type`, `goods_type` … |
| `MerchantRiskManageTradeUnionReportRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageTradeUnionDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageTradeUnionDtos.cs) | `sp_mchid`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `out_trade_no`, `openid`, `phone`, `certificates_number`, `client_ip`, `risk_level`, `line_type`, `goods_type` … |
| `MerchantRiskManageTradeRiskInfoResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageTradeRiskDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageTradeRiskDtos.cs) | `risk_score`, `risk_type`, `punish_type`, `punish_description`, `sp_mchid`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `out_trade_no`, `additional_punish_type`, `complaints_information` |
| `MerchantRiskManageTradeRiskResultResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageTradeRiskDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageTradeRiskDtos.cs) | `risk_score`, `risk_type`, `punish_type`, `punish_description`, `sp_mchid`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `out_trade_no`, `additional_punish_type`, `complaints_information` |
| `MerchantRiskManageTradeRiskResultRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageTradeRiskDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageTradeRiskDtos.cs) | `risk_score`, `risk_type`, `punish_type`, `punish_description`, `sp_mchid`, `acquiring_bank_id`, `channel_id`, `sub_mchid`, `out_trade_no`, `additional_punish_type`, `complaints_information` |
| `MerchantRiskManageViolationNotificationResponse` | 应答 | [MerchantRiskManage/MerchantRiskManageViolationDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageViolationDtos.cs) | `notify_url` |
| `MerchantRiskManageViolationNotificationRequest` | 请求体 | [MerchantRiskManage/MerchantRiskManageViolationDtos.cs](../../DataModels/MerchantRiskManage/MerchantRiskManageViolationDtos.cs) | `notify_url` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantRiskManage));

var service = app.Services.GetRequiredService<IWechatPayMerchantRiskManageService>();
var ct = CancellationToken.None;
await service.DeleteViolationNotificationAsync(ct);
// 裸 Task：成功与否按 HTTP 状态码与业务错误码判定（WechatPayException）。
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantRiskManageService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`DeleteViolationNotificationAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantRiskManageService](./IWechatPayMerchantRiskManageService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMerchantRiskManageContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantRiskManageContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayMerchantServiceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantServiceContractGuards.cs)、[`WechatPayMerchantSettlementContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantSettlementContractGuards.cs) |
| 守卫中的代表性测试方法 | `MerchantRiskManageEndpoints_ShouldMatchOfficialRoutesAndVerbFaces`、`MerchantRiskManageRoutes_ShouldStayOnOwnPrefixAndNotDeclareNeighbouringFaces`、`MerchantRiskManageDtos_ShouldExposeOfficialFieldNamesIncludingMisspellings`、`MerchantRiskManageConstants_ShouldMatchOfficialValueSetsAndRoster`、`MerchantRiskManageUnspecifiedOfficialFields_ShouldStayScalarsWithoutInventedEnums`、`MerchantRiskManageResponses_ShouldDeriveFromBaseAndFollowShareSplitRulings`、`MerchantRiskManageDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MerchantRiskManage`](../../DataModels/MerchantRiskManage) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMerchantRiskManageContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayMerchantServiceContractGuards.cs`、`WechatPayMerchantSettlementContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
