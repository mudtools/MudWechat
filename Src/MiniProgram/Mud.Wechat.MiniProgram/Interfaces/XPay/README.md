# XPay 域 · 虚拟支付 · 支付投诉

微信小程序「虚拟支付 · 支付投诉」域 SDK（7 端点：获取投诉列表 / 获取投诉详情 / 获取协商历史 /
回复用户 / 完成投诉处理 / 获取投诉图片签名头部 / 商户被管控原因查询）。 接口：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)、[IWxaXPayCurrencyService](./IWxaXPayCurrencyService.cs)、[IWxaXPayFundsService](./IWxaXPayFundsService.cs)、[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)、[IWxaXPayOrderService](./IWxaXPayOrderService.cs)、[IWxaXPaySubscribeService](./IWxaXPaySubscribeService.cs)；注册组 `XPay`；模块枚举 `MiniProgramModule.XPay`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaXPayContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaXPayContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/XPay`](../../../Mud.Wechat.MiniProgram/XPay/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaXPayComplaintService = 7；IWxaXPayCurrencyService = 4；IWxaXPayFundsService = 10；IWxaXPayGoodsService = 5；IWxaXPayOrderService = 7；IWxaXPaySubscribeService = 4；按 HTTP 特性实计 **37** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaXPayComplaintService` | `GetComplaintListAsync` | POST | `/xpay/get_complaint_list` | `AccessToken`（Query），Query `access_token` | 获取投诉列表（`POST /xpay/get_complaint_list`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.… |
| 2 | `IWxaXPayComplaintService` | `GetComplaintDetailAsync` | POST | `/xpay/get_complaint_detail` | `AccessToken`（Query），Query `access_token` | 获取投诉详情（`POST /xpay/get_complaint_detail`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.q… |
| 3 | `IWxaXPayComplaintService` | `GetNegotiationHistoryAsync` | POST | `/xpay/get_negotiation_history` | `AccessToken`（Query），Query `access_token` | 获取协商历史（`POST /xpay/get_negotiation_history`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixi… |
| 4 | `IWxaXPayComplaintService` | `ResponseComplaintAsync` | POST | `/xpay/response_complaint` | `AccessToken`（Query），Query `access_token` | 回复用户（`POST /xpay/response_complaint`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.co… |
| 5 | `IWxaXPayComplaintService` | `CompleteComplaintAsync` | POST | `/xpay/complete_complaint` | `AccessToken`（Query），Query `access_token` | 完成投诉处理（`POST /xpay/complete_complaint`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.… |
| 6 | `IWxaXPayComplaintService` | `GetUploadFileSignAsync` | POST | `/xpay/get_upload_file_sign` | `AccessToken`（Query），Query `access_token` | 获取微信支付反馈投诉图片的签名头部（`POST /xpay/get_upload_file_sign`，不支持云调用，权限集 157）。<br>官方文档：https://develope… |
| 7 | `IWxaXPayComplaintService` | `QueryPunishmentReasonsAsync` | POST | `/xpay/query_punishment_reasons` | `AccessToken`（Query），Query `access_token` | 商户被管控原因查询（`POST /xpay/query_punishment_reasons`，不支持云调用，权限集 157；<br>**官方明写无特殊错误码**）。<br>官方文档：http… |
| 8 | `IWxaXPayCurrencyService` | `CurrencyPayAsync` | POST | `/xpay/currency_pay` | `AccessToken`（Query），Query `access_token` | 扣减代币（`POST /xpay/currency_pay`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.com/mini… |
| 9 | `IWxaXPayCurrencyService` | `CancelCurrencyPayAsync` | POST | `/xpay/cancel_currency_pay` | `AccessToken`（Query），Query `access_token` | 代币支付退款（`POST /xpay/cancel_currency_pay`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq… |
| 10 | `IWxaXPayCurrencyService` | `PresentCurrencyAsync` | POST | `/xpay/present_currency` | `AccessToken`（Query），Query `access_token` | 代币赠送（`POST /xpay/present_currency`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https://develope… |
| 11 | `IWxaXPayCurrencyService` | `QueryUserBalanceAsync` | POST | `/xpay/query_user_balance` | `AccessToken`（Query），Query `access_token` | 查询代币余额（`POST /xpay/query_user_balance`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.… |
| 12 | `IWxaXPayFundsService` | `BindTransferAccountAsync` | POST | `/xpay/bind_transfer_accout` | `AccessToken`（Query），Query `access_token` | 绑定广告金充值账户（`POST /xpay/bind_transfer_accout`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https:/… |
| 13 | `IWxaXPayFundsService` | `QueryTransferAccountAsync` | POST | `/xpay/query_transfer_account` | `AccessToken`（Query），Query `access_token` | 查询广告金充值账户（`POST /xpay/query_transfer_account`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https… |
| 14 | `IWxaXPayFundsService` | `CreateFundsBillAsync` | POST | `/xpay/create_funds_bill` | `AccessToken`（Query），Query `access_token` | 充值广告金（`POST /xpay/create_funds_bill`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https://develo… |
| 15 | `IWxaXPayFundsService` | `QueryFundsBillAsync` | POST | `/xpay/query_funds_bill` | `AccessToken`（Query），Query `access_token` | 查询广告金充值记录（`POST /xpay/query_funds_bill`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https://dev… |
| 16 | `IWxaXPayFundsService` | `QueryAdverFundsAsync` | POST | `/xpay/query_adver_funds` | `AccessToken`（Query），Query `access_token` | 查询广告金发放记录（`POST /xpay/query_adver_funds`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https://de… |
| 17 | `IWxaXPayFundsService` | `QueryRecoverBillAsync` | POST | `/xpay/query_recover_bill` | `AccessToken`（Query），Query `access_token` | 查询广告金回收记录（`POST /xpay/query_recover_bill`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https://d… |
| 18 | `IWxaXPayFundsService` | `DownloadAdverfundsOrderAsync` | POST | `/xpay/download_adverfunds_order` | `AccessToken`（Query），Query `access_token` | 下载广告金对应的商户订单信息（`POST /xpay/download_adverfunds_order`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方… |
| 19 | `IWxaXPayFundsService` | `QueryBizBalanceAsync` | POST | `/xpay/query_biz_balance` | `AccessToken`（Query），Query `access_token` | 查询商家账户可提现余额（`POST /xpay/query_biz_balance`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin… |
| 20 | `IWxaXPayFundsService` | `CreateWithdrawOrderAsync` | POST | `/xpay/create_withdraw_order` | `AccessToken`（Query），Query `access_token` | 创建提现单（`POST /xpay/create_withdraw_order`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.q… |
| 21 | `IWxaXPayFundsService` | `QueryWithdrawOrderAsync` | POST | `/xpay/query_withdraw_order` | `AccessToken`（Query），Query `access_token` | 查询提现单（`POST /xpay/query_withdraw_order`，不支持云调用，权限集 157）。<br>文档：https://developers.weixin.qq.c… |
| 22 | `IWxaXPayGoodsService` | `StartUploadGoodsAsync` | POST | `/xpay/start_upload_goods` | `AccessToken`（Query），Query `access_token` | 批量上传道具（`POST /xpay/start_upload_goods`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.… |
| 23 | `IWxaXPayGoodsService` | `QueryUploadGoodsAsync` | POST | `/xpay/query_upload_goods` | `AccessToken`（Query），Query `access_token` | 查询批量上传道具任务（`POST /xpay/query_upload_goods`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin… |
| 24 | `IWxaXPayGoodsService` | `StartPublishGoodsAsync` | POST | `/xpay/start_publish_goods` | `AccessToken`（Query），Query `access_token` | 启动批量发布道具任务（`POST /xpay/start_publish_goods`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixi… |
| 25 | `IWxaXPayGoodsService` | `QueryPublishGoodsAsync` | POST | `/xpay/query_publish_goods` | `AccessToken`（Query），Query `access_token` | 查询批量发布道具任务（`POST /xpay/query_publish_goods`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixi… |
| 26 | `IWxaXPayGoodsService` | `UploadVpFileAsync` | POST | `/xpay/upload_vp_file` | `AccessToken`（Query），Query `access_token` | 上传媒体文件（`POST /xpay/upload_vp_file`，不支持云调用，权限集 157）：为道具图与投诉凭证换取 `file_id`。<br>官方文档：https://dev… |
| 27 | `IWxaXPayOrderService` | `QueryOrderAsync` | POST | `/xpay/query_order` | `AccessToken`（Query），Query `access_token` | 查询创建的订单（`POST /xpay/query_order`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.com/mi… |
| 28 | `IWxaXPayOrderService` | `RefundOrderAsync` | POST | `/xpay/refund_order` | `AccessToken`（Query），Query `access_token` | 启动订单退款任务（`POST /xpay/refund_order`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.com/… |
| 29 | `IWxaXPayOrderService` | `NotifyProvideGoodsAsync` | POST | `/xpay/notify_provide_goods` | `AccessToken`（Query），Query `access_token` | 通知已发货完成（`POST /xpay/notify_provide_goods`，不支持云调用，权限集 157、**无签名 Query 参数**）。<br>官方文档：https://d… |
| 30 | `IWxaXPayOrderService` | `StartDownloadOrderAsync` | POST | `/xpay/start_download_order` | `AccessToken`（Query），Query `access_token` | 下载支付订单（创建账单下载任务，`POST /xpay/start_download_order`，不支持云调用，权限集 157）。<br>官方文档：https://developers… |
| 31 | `IWxaXPayOrderService` | `QueryDownloadOrderAsync` | POST | `/xpay/query_download_order` | `AccessToken`（Query），Query `access_token` | 查询下载订单任务（`POST /xpay/query_download_order`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin… |
| 32 | `IWxaXPayOrderService` | `DownloadBillAsync` | POST | `/xpay/download_bill` | `AccessToken`（Query），Query `access_token` | 下载小程序账单（`POST /xpay/download_bill`，不支持云调用，权限集 157）。<br>官方文档：https://developers.weixin.qq.com/… |
| 33 | `IWxaXPayOrderService` | `DownloadIosSettlementBillAsync` | POST | `/xpay/download_ios_settlement_bill` | `AccessToken`（Query），Query `access_token` | 下载虚拟支付 iOS 月结账单（`POST /xpay/download_ios_settlement_bill`，不支持云调用，权限集 157）。<br>官方文档：https://de… |
| 34 | `IWxaXPaySubscribeService` | `SendSubscribePrePaymentAsync` | POST | `/xpay/send_subscribe_pre_payment` | `AccessToken`（Query），Query `access_token` | 预通知扣款（`POST /xpay/send_subscribe_pre_payment`，不支持云调用，权限集 157）。<br>官方文档：https://developers.wei… |
| 35 | `IWxaXPaySubscribeService` | `SubmitSubscribePayOrderAsync` | POST | `/xpay/submit_subscribe_pay_order` | `AccessToken`（Query），Query `access_token` | 发起订阅扣款（`POST /xpay/submit_subscribe_pay_order`，不支持云调用，权限集 157）。<br>官方文档：https://developers.we… |
| 36 | `IWxaXPaySubscribeService` | `QuerySubscribeContractAsync` | POST | `/xpay/query_subscribe_contract` | `AccessToken`（Query），Query `access_token` | 查询签约关系（`POST /xpay/query_subscribe_contract`，不支持云调用，权限集 157、**官方明写无特殊错误码**）。<br>官方文档：https://… |
| 37 | `IWxaXPaySubscribeService` | `CancelSubscribeContractAsync` | POST | `/xpay/cancel_subscribe_contract` | `AccessToken`（Query），Query `access_token` | 商家解约（`POST /xpay/cancel_subscribe_contract`，不支持云调用，**官方页面完全没有第三方调用段**、<br>因而**无权限集 id**，且官方明写… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaXPayComplaintService`

域统一形态（POST only、不支持云调用、权限集 157、`pay_sig` 落 Query 的凭据处置、敏感值口径、
错误码表三分与 `env` 的三类语义）见 `IWxaXPayCurrencyService`，本文件只写本族特有事实。

**本族 7 端点全部要求 `pay_sig`，且全部是「签名校验用 env」**（官方原文「仅作为签名校验，
查询的结果都是正式环境的」）⇒ 投诉与管控数据**只有现网一份**，沙箱值不会切出另一套数据。

**本族是敏感值最重的一章**：投诉人手机号 `payer_phone` 与 `payer_openid` 属个人信息，
凭证图片地址、`sign` 与 `cos_url`（官方明示 30 分钟有效）属访问凭据 ⇒
**严禁**写进日志、遥测或异常消息（与 `permanent_code` 同级处置）。

**处理动作有官方优先级依据**：`problem_type = REFUND`（申请退款）官方原文「是需要最高优先处理的单据」，
`user_tag_list` 含 `HIGH_RISK` 时原文「请按照运营要求优先妥善处理」⇒ 队列排序应读这两支而非投诉时间。

### `IWxaXPayCurrencyService`

**域构成口径（2026-10-13 依官方 VirtualPayment 分区逐页取证）**：官方把 37 个 `/xpay/*` 端点编成
代币、订单与账单、道具、会员订阅、广告金与提现、支付投诉六个章节，本线按章节分六支接口、
共用注册组 `XPay` 与 DTO 命名空间 `Mud.Wechat.MiniProgram.DataModels.XPay`（守卫 XP1~XP6）。

**全域统一形态**：官方 37 端点**全部是 POST**（无 GET），**全部不支持云调用**；
除「商家解约」一页**完全没有第三方调用段**外，其余 36 端点均声明「支持第三方平台代商家调用」
且权限集 id 为 **157**，`access_token` 位可用 `authorizer_access_token` 代调用。
适用范围段全域统一为「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用」
⇒ **该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数**。

**令牌与签名注入（MP-X2 / MP-X3 同口径 + 本域特有）**：令牌复用 `MpTokenTypes.AccessToken` +
Query 注入 `access_token`；本域在其之外**还强制 Query 支付签名**，官方调用示例逐字为
`?access_token=ACCESS_TOKEN&pay_sig=PAY_SIG`（双签名端点为 `&signature=SIGNATURE&pay_sig=PAY_SIG`）
⇒ `pay_sig` / `signature` 是**方法级显式参数**而非 `[Token]` 注入，签名算法与
`session_key` 相关，属宿主职责，SDK 不代签。

**`pay_sig` 的凭据处置（AGENTS §8「二选一」）**：该参数名不在组件
`SensitiveUrlRedactor` 静态词表内（词表有 `sig`/`sign`/`signature`，是**精确匹配**语义、
覆盖不到 `pay_sig`），而组件登记门面是公开 API ⇒ 本模块注册期经
`SensitiveUrlKeys.Register("pay_sig")` 登记为进程级强制掩码键，形态与企微线群机器人
`key`（守卫 WEB3）、广告线 `user_token`（守卫 ADS-B5）一致，由守卫 XP6 锁定「登记表 ⊆ 词表补集」。

**敏感值处置（全域）**：代币余额、商户可提现余额与提现金额、账单/订单下载链接、`cos_url`（官方明示
30 分钟有效）、`sign`、`pay_sig`、`signature`、投诉人手机号与 openid、订单 `token`
⇒ **严禁**写进日志、遥测或异常消息（与 `permanent_code` 同级处置）。

**错误码面三分（逐页取证，不得合并口径）**：官方 **28** 端点共享同一张 `26849_0xx` 域内错误码表
（另有 `-1` 系统错误）；会员订阅的两支**各有独立码表** —— 预通知扣款是 `674690001` /
`690000000` / `690000001`，发起订阅扣款是米大师侧 `-15001 ~ -15027` 加 `1001`；
余 **7** 端点（商家解约、通知已发货完成、下载 iOS 月结账单、查询下载订单任务、查询签约关系、商户被管控原因查询、
下载支付订单）官方明写「此接口没有特殊错误码」⇒ 只有通用错误码，不得臆造域内码。

**`env` 有三类官方语义，不可当成同一个「环境开关」**：**17** 端点的 `env` 原文标注
「仅作为签名校验，查询的结果都是正式环境的」或「用于基类签名校验」（含本域绑定/充值/查询广告金与全部投诉端点、
账单任务两支）⇒ 传沙箱值不改变查询结果；**13** 端点是真正的「0 正式 / 1 沙箱」数据面语义；
发起订阅扣款虽标 0/1 却官方明写**仅支持 0**（错误码 `-15011`）；余 **6** 端点
参数表里**根本没有 `env` 支**（商家解约、查询签约关系、预通知扣款、商户被管控原因查询、
下载小程序账单、下载 iOS 月结账单）⇒ **不得**给它们造出环境入参。逐端点归类由守卫 XP3 锁定。

### `IWxaXPayFundsService`

域统一形态（POST only、不支持云调用、权限集 157、`pay_sig` 落 Query 的凭据处置、敏感值口径、
错误码表三分与 `env` 的三类语义）见 `IWxaXPayCurrencyService`，本文件只写本族特有事实。

**本族是「签名面」分裂最严重的一章**：10 端点里只有 `QueryBizBalanceAsync`、
`CreateWithdrawOrderAsync`、`QueryWithdrawOrderAsync` 三支要求 `pay_sig`，
其余 7 支（广告金侧全部）Query 只有 `access_token` ⇒ 该分界是官方逐页正文的差异，
**不是** SDK 漏参，由守卫 XP2 逐端点锁定。

**官方本域 37 页均无端点级频率数字**：只有错误码 `268490015`「频率限制」与订阅侧 `-15020`
「操作过快，请稍候再试」证明确有限频存在，但**没有任何一页给出次/分钟量级**
⇒ 注释与守卫一律写「官方正文未给端点级频率数字」，**不得**凭第三方 SDK 注释或记忆补数。

**资金方向的三步链路是官方必填面强制的**：绑定账户（`BindTransferAccountAsync`）→
用发放记录的结算周期定位并充值（`QueryAdverFundsAsync` → `CreateFundsBillAsync`）→
余额与提现（`QueryBizBalanceAsync` → `CreateWithdrawOrderAsync`）。

### `IWxaXPayGoodsService`

域统一形态（POST only、不支持云调用、权限集 157、`pay_sig` 落 Query 的凭据处置、敏感值口径与
全域错误码表）见 `IWxaXPayCurrencyService`，本文件只写本族特有事实。

**本族是「开发环境 → 现网」的两段式发布**：上传只把道具放进开发环境，发布才把它推到现网 ⇒
跳过发布直接让用户买单，官方按 `268490008`（代币/道具未发布）拒绝。

**上传与发布两支各占一条任务位**：官方 `268490012` 原文「批量任务运行中，请等待完成后才能再次运行」
⇒ 提交前须先查任务状态，而非并行提交后靠错误码兜。

**本族 5 端点全部只带 `pay_sig`**，无用户态签名支（道具是商户侧资产，不属任一用户）。

### `IWxaXPayOrderService`

域统一形态（POST only、不支持云调用、权限集 157、`pay_sig` 落 Query 的凭据处置、敏感值口径与
全域错误码表）见 `IWxaXPayCurrencyService`，本文件只写本族特有事实。

**账单下载是三条不同通路，不可互相替代**：日粒度区间账单走 `DownloadBillAsync`（同步取链接）、
按订单类型筛选的明细走「创建任务 + 轮询」两支（`StartDownloadOrderAsync` +
`QueryDownloadOrderAsync`）、iOS 月结走 `DownloadIosSettlementBillAsync`（月粒度）。

**本族是「先查后动」的强依赖关系**：官方 `268490016` 明写退款 `left_fee` 不符时
「请通过query_order接口查询确认」⇒ `RefundOrderAsync` 的入参必须来自
`QueryOrderAsync` 的实时结果，不得用本地缓存金额。

### `IWxaXPaySubscribeService`

域统一形态（POST only、不支持云调用、权限集 157、`pay_sig` 落 Query 的凭据处置、敏感值口径与
全域错误码表三分）见 `IWxaXPayCurrencyService`，本文件只写本族特有事实。

**本族是全域唯一「两端点自带独立错误码表」的章节**：`SendSubscribePrePaymentAsync` 回
`674690001` / `690000000` / `690000001`，`SubmitSubscribePayOrderAsync` 回米大师侧
`-15001 ~ -15027` 与 `1001` —— 二者都**不是**本域共享的 `26849_0xx` 表 ⇒ 判定器不得按域表前缀过滤。

**三步时序由官方错误码强制**：签约关系（`QuerySubscribeContractAsync`）→ 预通知
（`SendSubscribePrePaymentAsync`）→ 扣款（`SubmitSubscribePayOrderAsync`）。
官方 `690000000`「user not subscribed」拦未签约、`-15025`「不存在预通知单/重复下单」拦未预通知 ⇒
顺序错不是参数错，重试同一请求不会成功。

**本族的定位键是协议号 `out_contract_code` + 订阅制道具 `product_id` 的组合**（三支端点都要求这两支），
且官方对道具单列「需为订阅制道具」⇒ 用直购道具 id 调用本族必然失败。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetComplaintListAsync` — 获取投诉列表（`POST /xpay/get_complaint_list`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/get_complaint_list`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |
| `signature` | Query `signature` | `string?` | 用户态签名（官方 Query 参数 `signature`）。<br>**官方本页自相矛盾**（与 `refund_order` 同形）：「注意事项」原文写「使用用户态签名与支付签名」，<br>而「查询参数」表只列 `access_token` 与 `pay_sig` 两支 ⇒ 本 SDK 以**参数表为准**定 `pay_sig` 必填、<br>以**注意事项为准**开放可选 `signature`（默认不发送）。该取舍与 `refund_order` 一并由守卫 XP3 登记。 |

- **请求体**：`WxaXPayGetComplaintListRequest` — 请求体（`env` / `begin_date` / `end_date` / `offset` / `limit` 五支全必填），见 `WxaXPayGetComplaintListRequest`。
- **返回**：`WxaXPayGetComplaintListResponse` — 总数 + 投诉单列表（列表元素与详情端点同形共用），见 `WxaXPayGetComplaintListResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；**时间窗与分页五支全部必填**（本域必填面最严的查询页之一）， / 且日期格式是 `yyyy-mm-dd` 字符串 ⇒ 与账单侧 `YYYYMMDD` 数字不同形，两支不可互喂。
- **翻页游标是 `offset` 且官方原文「从0开始」** ⇒ 处理过程中列表本身会因新投诉插入而位移， / 按 `offset` 批处理须以 `complaint_id` 去重。
- **两支「待办」信号官方单列在响应里**：`incoming_user_response`（有待回复用户留言）与 / `complaint_state`（PENDING / PROCESSING / PROCESSED，取值写在说明列、无枚举表）⇒ SLA 看板须两支一起看。
- **敏感值**：`payer_phone` / `payer_openid` / 凭证图片地址 ⇒ 不得进日志、遥测或异常消息。

### 2. `GetComplaintDetailAsync` — 获取投诉详情（`POST /xpay/get_complaint_detail`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_detail.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/get_complaint_detail`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayGetComplaintDetailRequest` — 请求体（`env` / `complaint_id` 必填），见 `WxaXPayGetComplaintDetailRequest`。
- **返回**：`WxaXPayGetComplaintDetailResponse` — 单个投诉单（与列表元素共用类型），见 `WxaXPayGetComplaintDetailResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，以 `complaint_id` 定位（该值官方注明来自 / `GetComplaintListAsync`）。
- **列表与详情的子表字段名集逐字段相同** ⇒ 共用 `WxaXPayComplaint` 类型（由 `sameAs` 取证、 / 守卫 XP5 锁定）；差异只在官方类型列的详略标注（列表侧把三支嵌套标 `objarray`、详情侧标 `array`， / 而示例证明都是对象数组）。
- **详情比列表多不出新信息**：本页返回体就是单个投诉对象 ⇒ 不要用「详情」当补字段的手段，缺字段是官方没有。

### 3. `GetNegotiationHistoryAsync` — 获取协商历史（`POST /xpay/get_negotiation_history`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_negotiation_history.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/get_negotiation_history`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayGetNegotiationHistoryRequest` — 请求体（`env` / `complaint_id` / `offset` / `limit` 全必填），见 `WxaXPayGetNegotiationHistoryRequest`。
- **返回**：`WxaXPayGetNegotiationHistoryResponse` — 流水总数 + 逐条协商记录（`operate_type` 取值字典出处），见 `WxaXPayGetNegotiationHistoryResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；**本端点是本域唯一给出操作类型枚举表的一页** / （`WxaXPayOperateTypes` 20 值，含两处官方 typo 照录）。
- **六值是系统通知而非人发言**（三支 `*_SYSTEM_MESSAGE` 加撤诉/确认两支）⇒ 做「商户是否已回复」判据时 / 须排除系统通知支，否则官方自动文案会被读成处理动作。
- **两值仅存在于历史投诉单**（官方原文对 `USER_REVOKE_COMPLAINT` 与 `USER_COMFIRM_COMPLAINT` 的说明）⇒ / **不得**据本页取值集反推投诉列表的状态机全集。

### 4. `ResponseComplaintAsync` — 回复用户（`POST /xpay/response_complaint`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_response_complaint.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/response_complaint`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayResponseComplaintRequest` — 请求体（`env` / `complaint_id` / `response_content` / `response_images` 四支全必填），见 `WxaXPayResponseComplaintRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；**`response_images` 官方必填**且元素是 / `upload_vp_file` 返回的 `file_id`（原文「传upload_vp_file接口返回的file_id」）⇒ / 纯文字回复也须传**空数组**而不是省略该字段。
- **回复内容受内容审核**：撞 `268490007`（图片或文字存在敏感内容）时官方不会部分生效 ⇒ 须改文案重发。
- **回复不等于结单**：状态推进须走 `CompleteComplaintAsync`，否则投诉单停留在处理中并继续计入 `incoming_user_response` 面。

### 5. `CompleteComplaintAsync` — 完成投诉处理（`POST /xpay/complete_complaint`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_complete_complaint.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/complete_complaint`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayCompleteComplaintRequest` — 请求体（`env` / `complaint_id` 必填），见 `WxaXPayCompleteComplaintRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，请求面只有定位键两支。
- **结单不等于退款**：本端点只把投诉单置为处理完成，资金动作须走 / `IWxaXPayOrderService.RefundOrderAsync` ⇒ 对 `problem_type = REFUND` 的单据， / 官方口径是「需最高优先处理」，顺序应为先处理资金再结单。
- **结单是终态动作**：`complaint_state` 一旦推进到 `PROCESSED`，同单再回复的官方语义本页未给 ⇒ / **不得**推断「结单后仍可继续协商」，需重开时以官方正文或错误码为准。

### 6. `GetUploadFileSignAsync` — 获取微信支付反馈投诉图片的签名头部（`POST /xpay/get_upload_file_sign`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_upload_file_sign.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/get_upload_file_sign`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayGetUploadFileSignRequest` — 请求体（`env` / `wxpay_url` / `convert_cos` / `complaint_id` 四支全必填），见 `WxaXPayGetUploadFileSignRequest`。
- **返回**：`WxaXPayGetUploadFileSignResponse` — `Authorization` 头部值 + `convert_cos` 为真时的临时转存地址，见 `WxaXPayGetUploadFileSignResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；**本端点只签发凭证、不代下载** —— 官方原文要求下载微信支付图片时 / 必须同时带三个头部：`Authorization`（取本响应 `sign`）、`Accept: application/json`、 / 非空 `User-Agent`，缺任一支官方报错 ⇒ 下载动作归宿主。
- **`wxpay_url` 有官方限定的地址形态**（原文示例 / `https://api.mch.weixin.qq.com/v3/merchant-service/images/{xxxxxx}`）⇒ 传小程序侧地址不在本页契约内。
- **`cos_url` 仅当 `convert_cos` 为 true 才有意义，且官方明示 30 分钟有效** ⇒ 不得持久化复用。
- **敏感值**：`sign` 是访问凭证 ⇒ 不得进日志、遥测或异常消息（本响应两支都属凭据形态）。

### 7. `QueryPunishmentReasonsAsync` — 商户被管控原因查询（`POST /xpay/query_punishment_reasons`，不支持云调用，权限集 157；
**官方明写无特殊错误码**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_punishment_reasons.html。

- **声明接口**：[IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)
- **官方契约**：`POST /xpay/query_punishment_reasons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |
- **返回**：`WxaXPayQueryPunishmentReasonsResponse` — 管控能力清单 + 逐条被管控原因与解脱路径，见 `WxaXPayQueryPunishmentReasonsResponse`。

**备注（原文）**

- **官方请求参数表字段数为 0**（本域唯一，且无 `env` 支）：本端点只定位当前商户自身 ⇒ / 不设请求 DTO、方法无 `[Body]` 参数，与本线短剧授权端点同一「空请求体」形态。
- **两支能力面都要读**：`limited_functions` 是标准枚举能力名列表、`other_limited_functions` / 是未被枚举覆盖的自由文本（官方为该场景单设字段）⇒ 只读前者会漏掉部分管控能力。
- **`limitation_case_id` 是与管控流水通知 `business_code` 关联的官方 join 键**（原文说明）⇒ / 收到管控通知后应回本页取解脱路径，而非直接改代码。
- **两支时间不可互替**：`limitation_start_date` 仅延迟管控时返回（预计开始），实际生效时刻看 / `limitation_date`。
- **官方明写本页无特殊错误码**，且三支枚举语义字段（原因类型 / 解脱方式 / 管控生效方式）官方无枚举表⇒ 按 XP5 口径不建常量类。

### 8. `CurrencyPayAsync` — 扣减代币（`POST /xpay/currency_pay`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_currency_pay.html。

- **声明接口**：[IWxaXPayCurrencyService](./IWxaXPayCurrencyService.cs)
- **官方契约**：`POST /xpay/currency_pay`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |
| `signature` | Query `signature` | `string` | 用户态签名（官方 Query 参数 `signature`，必填，由宿主以 `session_key` 计算）。 |

- **请求体**：`WxaXPayCurrencyPayRequest` — 请求体，见 `WxaXPayCurrencyPayRequest`。
- **返回**：`WxaXPayCurrencyPayResponse` — 扣减后余额与本次消耗的赠送部分，见 `WxaXPayCurrencyPayResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体，Query 同时带 `access_token` / `signature` / `pay_sig` / 三支（本域仅 `currency_pay`、`cancel_currency_pay`、`query_user_balance` 三支是双签名形态）。
- **幂等判定**：`268490004` 官方原文表示「之前的操作已经成功」⇒ 该码须按成功处理， / 换单号重试会造成重复扣减。
- **本端点不产生支付单营收**：代币是用户已充值余额的扣减，退款语义在 `CancelCurrencyPayAsync`。

### 9. `CancelCurrencyPayAsync` — 代币支付退款（`POST /xpay/cancel_currency_pay`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_cancel_currency_pay.html。

- **声明接口**：[IWxaXPayCurrencyService](./IWxaXPayCurrencyService.cs)
- **官方契约**：`POST /xpay/cancel_currency_pay`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |
| `signature` | Query `signature` | `string` | 用户态签名（官方 Query 参数 `signature`，必填）。 |

- **请求体**：`WxaXPayCancelCurrencyPayRequest` — 请求体（`pay_order_id` 是原支付单号、`order_id` 是本次退款单号），见 `WxaXPayCancelCurrencyPayRequest`。
- **返回**：`WxaXPayCancelCurrencyPayResponse` — 退款订单号，见 `WxaXPayCancelCurrencyPayResponse`。

**备注（原文）**

- 官方契约：**POST** + 双签名 Query（`signature` + `pay_sig`），六支请求字段官方全部标注必填。
- **与订单退款的分工**：本端点退的是**代币扣减**，真金白银的支付单退款走 / `IWxaXPayOrderService.RefundOrderAsync`；官方对已走后者退款的单回 `268490005` / （「订单已经通过cancel_currency_pay接口退款，不支持再退款」）⇒ 两条退款线互斥，不可双退。
- **`268490006`（金额不足）**是余额校验失败而非系统错误 ⇒ 重试前须先核对 `amount` 与原单金额。

### 10. `PresentCurrencyAsync` — 代币赠送（`POST /xpay/present_currency`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_present_currency.html。

- **声明接口**：[IWxaXPayCurrencyService](./IWxaXPayCurrencyService.cs)
- **官方契约**：`POST /xpay/present_currency`

- **请求体**：`WxaXPayPresentCurrencyRequest` — 请求体，见 `WxaXPayPresentCurrencyRequest`。
- **返回**：`WxaXPayPresentCurrencyResponse` — 赠送后余额与累计赠送额，见 `WxaXPayPresentCurrencyResponse`。

**备注（原文）**

- 官方契约：**POST**，Query **只有 `access_token`**（官方调用示例 / `?access_token=ACCESS_TOKEN`，本域九支无签名端点之一）⇒ **不得**为本端点索要 `pay_sig`， / 守卫 XP3 锁定「有签名 / 无签名」两族清单。
- **赠送走无价账户**：`present_balance` 与有价余额分账，官方扣减顺序是先赠送后有价（见 / `WxaXPayCurrencyPayResponse`）⇒ 赠送不可计入营收口径。
- **`268490004` 同样表示「已赠送成功」**（官方原文把赠送列入该码适用范围）。

### 11. `QueryUserBalanceAsync` — 查询代币余额（`POST /xpay/query_user_balance`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_user_balance.html。

- **声明接口**：[IWxaXPayCurrencyService](./IWxaXPayCurrencyService.cs)
- **官方契约**：`POST /xpay/query_user_balance`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |
| `signature` | Query `signature` | `string` | 用户态签名（官方 Query 参数 `signature`，必填）。 |

- **请求体**：`WxaXPayQueryUserBalanceRequest` — 请求体（`openid` / `env` / `user_ip` 三支全部必填），见 `WxaXPayQueryUserBalanceRequest`。
- **返回**：`WxaXPayQueryUserBalanceResponse` — 有价/赠送两账户余额与四项累计量，见 `WxaXPayQueryUserBalanceResponse`。

**备注（原文）**

- 官方契约：**POST** + 双签名 Query。官方注意事项给出的签名拼接示例正是本页 / （`https://api.weixin.qq.com/xpay/query_user_balance?access_token=xxxx&pay_sig=xxx`） / ⇒ `pay_sig` 落 Query 是官方明示而非推断。
- **本端点是「用户态」余额**（按 `openid` 定位）；商户侧可提现余额走 / `IWxaXPayFundsService.QueryBizBalanceAsync`，两者单位与币种口径都不同，不可互替。
- **敏感值**：响应全部字段是用户资产数据 ⇒ 不得进日志、遥测或异常消息。

### 12. `BindTransferAccountAsync` — 绑定广告金充值账户（`POST /xpay/bind_transfer_accout`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_bind_transfer_accout.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/bind_transfer_accout`

- **请求体**：`WxaXPayBindTransferAccountRequest` — 请求体（三支官方全部非必填），见 `WxaXPayBindTransferAccountRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`，绑定结果须走 `QueryTransferAccountAsync`。

**备注（原文）**

- **路由拼写官方原文是 `bind_transfer_accout`（缺字母 n）** ⇒ 路由照官方原文， / 方法名用正确拼写 `BindTransferAccountAsync`；该拼写由守卫 XP1 逐字锁定，**不得**「顺手修正」路由。
- 官方契约：本端点 Query 只有 `access_token`，且三支字段**全部标注非必填**（本域唯一）⇒ / 「至少定位一个账户」是正文语义、表格不承载，SDK 不本地校验。
- **本页最常见的三类失败都有专属码**：`268490018` 行业 id 不匹配、`268490019` 账户已绑定其他 / appid、`268490020` 主体名称错误 ⇒ 绑定不是幂等操作，重复绑定同账户会撞 19。

### 13. `QueryTransferAccountAsync` — 查询广告金充值账户（`POST /xpay/query_transfer_account`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_transfer_account.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/query_transfer_account`

- **请求体**：`WxaXPayQueryTransferAccountRequest` — 请求体（只有 `env` 一支必填，且其官方语义是「仅作签名校验」），见 `WxaXPayQueryTransferAccountRequest`。
- **返回**：`WxaXPayQueryTransferAccountResponse` — 账户列表（`acct_list`），见 `WxaXPayQueryTransferAccountResponse`。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token`；请求单字段 ⇒ **本查询无分页、无筛选**， / 返回的是当前 appid 下的账户全集。
- **`state`（审核 0 待审 / 1 通过 / 2 驳回）与 `bind_result`（绑定 1 成功 / 2 失败）是两段独立状态， / 且后者从 1 开始编号** ⇒ 把 0 读成「绑定失败」会误判未审核的中间态。
- **本端点是充值的前置**：`create_funds_bill` 的 `transfer_account_uid` / / `transfer_account_name` / `transfer_account_agency_id` 三支必填值都来自本页 ⇒ 不得凭本地记录拼。

### 14. `CreateFundsBillAsync` — 充值广告金（`POST /xpay/create_funds_bill`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_create_funds_bill.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/create_funds_bill`

- **请求体**：`WxaXPayCreateFundsBillRequest` — 请求体（十支全部必填，`request_id` 是幂等键），见 `WxaXPayCreateFundsBillRequest`。
- **返回**：`WxaXPayCreateFundsBillResponse` — 充值单 ID（`bill_id`），见 `WxaXPayCreateFundsBillResponse`。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token`；**十支全部必填是本域请求面最严的一页**。
- **`request_id` 是官方幂等键**（原文「相同 id 的不同请求视为重复请求」，上限 1024 字符）⇒ / 重复请求返回 `268490004`，语义是「此前已成功」而非失败，**不得**据此换 id 重放（会重复充值）。
- **充值定位的是发放记录而不是余额**：`settle_begin`/`settle_end` 必须是某条发放记录的结算周期 / （由 `QueryAdverFundsAsync` 取得），且 `fund_type` 与 `authorize_advertise` / 两支官方要求显式赋值 ⇒ 金额不足撞 `268490023`、零金额撞 `268490024`、账户未进件撞 `268490021`。
- **敏感值**：充值金额与账户信息 ⇒ 不得进日志、遥测或异常消息。

### 15. `QueryFundsBillAsync` — 查询广告金充值记录（`POST /xpay/query_funds_bill`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_funds_bill.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/query_funds_bill`

- **请求体**：`WxaXPayQueryFundsBillRequest` — 请求体（`page` / `page_size` / `filter` / `env` 四支全必填），见 `WxaXPayQueryFundsBillRequest`。
- **返回**：`WxaXPayQueryFundsBillResponse` — 逐充值单 + 总页数，见 `WxaXPayQueryFundsBillResponse`。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token`；**分页与筛选官方全部必填**（与本族 / `QueryAdverFundsAsync` 的「四支全非必填」相反），且 `filter` 内时间窗两支亦必填 ⇒ / 不传时间窗无法查询。
- **`env` 在此只影响签名校验**（官方原文「仅作为签名校验，查询的结果都是正式环境的」）⇒ / 传 1 不会读到沙箱记录。
- **结果里的 `request_id` 原样回显充值时传入值** ⇒ 幂等核对应以它而不是 `bill_id` 为键。

### 16. `QueryAdverFundsAsync` — 查询广告金发放记录（`POST /xpay/query_adver_funds`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_adver_funds.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/query_adver_funds`

- **请求体**：`WxaXPayQueryAdverFundsRequest` — 请求体（四支官方全部非必填），见 `WxaXPayQueryAdverFundsRequest`。
- **返回**：`WxaXPayQueryAdverFundsResponse` — 逐发放记录 + 总页数（`adver_funds_list` / `total_page`），见 `WxaXPayQueryAdverFundsResponse`。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token`；本端点是本域「全字段选填」的查询端点 ⇒ / 可无条件全量翻页。
- **`fund_type` 决定后续可用性**（0 通用赠送 / 1 广告激励 / 2 定向激励）：`DownloadAdverfundsOrderAsync` / 官方原文**仅支持通用赠送（0）** ⇒ 用 1/2 的 `fund_id` 去下载订单必然拿不到结果。
- **`remain_amount` 与 `expire_time` 须同时读**：广告金有过期时刻，只看余额会在过期日突然失血。

### 17. `QueryRecoverBillAsync` — 查询广告金回收记录（`POST /xpay/query_recover_bill`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_recover_bill.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/query_recover_bill`

- **请求体**：`WxaXPayQueryRecoverBillRequest` — 请求体（四支全必填，`filter.bill_id` 亦必填），见 `WxaXPayQueryRecoverBillRequest`。
- **返回**：`WxaXPayQueryRecoverBillResponse` — 逐回收单 + 总页数，见 `WxaXPayQueryRecoverBillResponse`。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token`。
- **本端点 `filter.bill_id` 官方标注必填**（充值记录侧为选填）⇒ 两支筛选形状不同， / SDK 已分立为 `WxaXPayRecoverBillFilter` 与 `WxaXPayFundsBillFilter`， / **不得**共用（守卫 XP5 锁定）。
- **回收是官方主动动作，不是商户可选项**：结果里的 `refund_order_list`（对应的退款订单 id 列表）说明 / 回收由退款触发 ⇒ 营收预测须把本查询纳入对账，只看充值记录会高估可用广告金。
- **同名不同形**：本表与充值记录侧的子表官方都叫 `bill_list` 而字段集几乎不交 ⇒ 判据以类型为准，不以键名为准。

### 18. `DownloadAdverfundsOrderAsync` — 下载广告金对应的商户订单信息（`POST /xpay/download_adverfunds_order`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_adverfunds_order.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/download_adverfunds_order`

- **请求体**：`WxaXPayDownloadAdverfundsOrderRequest` — 请求体（`fund_id` / `env` 必填），见 `WxaXPayDownloadAdverfundsOrderRequest`。
- **返回**：`WxaXPayDownloadUrlResponse` — 下载链接（与「下载小程序账单」共用应答形态），见 `WxaXPayDownloadUrlResponse`。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token`（本族广告金侧全部不要求签名）。
- **本端点是「生成 + 轮询」两段式而非同步取链接**：官方注意事项原文「第一次调用触发生成下载url， / 可以间隔轮训来获取最终生成的下载url」⇒ 首次调用拿到空链接是正常形态， / 须轮询到链接非空而非据首次结果报错。该空缺没有官方延迟数字，**不得**臆造轮询间隔上限。
- **官方明写「当前仅支持通用赠送广告金对应订单下载」**（`fund_type = 0`）⇒ 见 `QueryAdverFundsAsync`。
- **敏感值**：链接本身是订单明细入口 ⇒ 不得进日志、遥测或异常消息，也不得持久化复用。

### 19. `QueryBizBalanceAsync` — 查询商家账户可提现余额（`POST /xpay/query_biz_balance`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_biz_balance.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/query_biz_balance`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQueryBizBalanceRequest` — 请求体（只有 `env` 一支必填），见 `WxaXPayQueryBizBalanceRequest`。
- **返回**：`WxaXPayQueryBizBalanceResponse` — 余额袋（`balance_available`：金额 + 币种），见 `WxaXPayQueryBizBalanceResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig` —— **本端点是广告金侧与提现侧的分界**：从这一支起本族三支都要求签名， / 因为它读的是可动资金。
- **金额单位是「元」且类型为 `string`**（官方原文「单位元」）⇒ 与本域订单侧「分为单位的数字」相反， / 换算须固定精度，禁止用浮点累加。
- **敏感值**：商户账户余额 ⇒ 不得进日志、遥测或异常消息。

### 20. `CreateWithdrawOrderAsync` — 创建提现单（`POST /xpay/create_withdraw_order`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_create_withdraw_order.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/create_withdraw_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayCreateWithdrawOrderRequest` — 请求体（`withdraw_no` / `withdraw_amount` / `env`），见 `WxaXPayCreateWithdrawOrderRequest`。
- **返回**：`WxaXPayCreateWithdrawOrderResponse` — 商户侧与微信侧双单号，见 `WxaXPayCreateWithdrawOrderResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；**「创建」只表示受理**，到账结果须走 `QueryWithdrawOrderAsync`。
- **本页官方自相矛盾已留档**：`withdraw_amount` 参数表标必填，说明列原文却写「允许不传， / 不传的情况下表示全额提现」⇒ 本 SDK 取**参数表**形态（属性可空即允许省略）， / 该取舍由守卫 XP3 登记，**不得**当成「SDK 漏参」。
- **单号格式是硬约束**：长度 [8,32]、仅字母/数字/下划线/短横线（与订阅扣款单号同一规则）。 / SDK 不本地校验（照官方错误码 `268490002`）。
- **敏感值**：提现金额与双单号 ⇒ 不得进日志、遥测或异常消息。

### 21. `QueryWithdrawOrderAsync` — 查询提现单（`POST /xpay/query_withdraw_order`，不支持云调用，权限集 157）。
文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_withdraw_order.html。

- **声明接口**：[IWxaXPayFundsService](./IWxaXPayFundsService.cs)
- **官方契约**：`POST /xpay/query_withdraw_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQueryWithdrawOrderRequest` — 请求体（`withdraw_no` / `env` 必填），见 `WxaXPayQueryWithdrawOrderRequest`。
- **返回**：`WxaXPayQueryWithdrawOrderResponse` — 提现单全貌（状态、金额、微信侧单号、成功时刻、失败原因），见 `WxaXPayQueryWithdrawOrderResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，以商户侧 `withdraw_no` 定位（不支持按微信侧单号反查）。
- **状态三值从 1 开始**（1 创建成功提现中 / 2 提现成功 / 3 提现失败，官方写在说明列、无枚举表）⇒ / `fail_reason` 仅在 3 时有意义，成功时刻 `withdraw_success_timestamp` 仅在 2 时有值。
- **三支金额/时间字段官方类型是 `string`**（`withdraw_amount` 元、两支时间戳）⇒ / 线格式照官方落字符串，**不得**「顺手」改成数字或时间类型（守卫 XP5 锁定）。

### 22. `StartUploadGoodsAsync` — 批量上传道具（`POST /xpay/start_upload_goods`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_upload_goods.html。

- **声明接口**：[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)
- **官方契约**：`POST /xpay/start_upload_goods`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayStartUploadGoodsRequest` — 请求体（`upload_item` 逐道具五支全必填 + `env`），见 `WxaXPayStartUploadGoodsRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`，道具结果须走 `QueryUploadGoodsAsync`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；端点名里的 `start` 是语义核心 —— 本调用只**提交任务**， / 同步成功不等于道具已入库。
- **`item_url` 在本请求里是「原始地址」、在查询结果里是「微信转存后地址」**（官方两处说明原文不同）⇒ / 对账不得比对二者，道具图以官方转存结果为准。
- **长度上限官方照录于条目注释**（`id`/`name` (0,20]、`remark` (0,1024]、`price` >0， / 且中文算一个字符）⇒ SDK 不本地校验，超限由官方 `268490002` 判回。

### 23. `QueryUploadGoodsAsync` — 查询批量上传道具任务（`POST /xpay/query_upload_goods`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_upload_goods.html。

- **声明接口**：[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)
- **官方契约**：`POST /xpay/query_upload_goods`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQueryUploadGoodsRequest` — 请求体（只有 `env` 一支必填），见 `WxaXPayQueryUploadGoodsRequest`。
- **返回**：`WxaXPayQueryUploadGoodsResponse` — 任务总状态 + 逐道具上传结果，见 `WxaXPayQueryUploadGoodsResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，请求无分页、无筛选 ⇒ **本查询返回的是当前环境的全量条目结果**， / 不能当增量游标使用。
- **任务级 `status` 与条目级 `upload_status` 是两个口径**（前者见 `WxaXPayPublishTaskStatuses` / 的取值集，后者 0 上传中 / 1 id 已存在 / 2 上传成功 / 3 上传失败，官方写在说明列、无枚举表故不建常量类）⇒ / 只有任务跑完才逐条看结果。
- **`upload_status = 1`（id 已存在）不是失败**：官方把它单列一值 ⇒ 重复提交同 id 时不得当成错误重试。

### 24. `StartPublishGoodsAsync` — 启动批量发布道具任务（`POST /xpay/start_publish_goods`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_publish_goods.html。

- **声明接口**：[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)
- **官方契约**：`POST /xpay/start_publish_goods`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayStartPublishGoodsRequest` — 请求体（`publish_item` 每项只有 `id` + `env`），见 `WxaXPayStartPublishGoodsRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`，发布结果须走 `QueryPublishGoodsAsync`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；发布请求**只需道具 id**（官方原文「添加到开发环境时传的道具id」） / ⇒ 名称与价格来自上传阶段，本端点不重传、也无法在此修正。
- **`env` 必填且这里指的是签名校验环境**（0 现网 / 1 沙箱，与本域其余端点同口径）⇒ / 沙箱环境发布的道具不会进入现网可售列表。

### 25. `QueryPublishGoodsAsync` — 查询批量发布道具任务（`POST /xpay/query_publish_goods`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_publish_goods.html。

- **声明接口**：[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)
- **官方契约**：`POST /xpay/query_publish_goods`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQueryPublishGoodsRequest` — 请求体（只有 `env` 一支必填），见 `WxaXPayQueryPublishGoodsRequest`。
- **返回**：`WxaXPayQueryPublishGoodsResponse` — 任务总状态 + 逐道具发布结果（`id` + `publish_status` + `errmsg`），见 `WxaXPayQueryPublishGoodsResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；`WxaXPayPublishTaskStatuses` 是本页官方枚举表出处 / （0 无任务 / 1 进行中 / 2 失败或部分失败 / 3 成功），**只有 2 才需要逐条目读 `errmsg`**。
- **发布结果侧没有 `name`/`price`**（与上传结果侧不同）⇒ 不要把两支结果类型共用， / 该差异由守卫 XP5 锁定。

### 26. `UploadVpFileAsync` — 上传媒体文件（`POST /xpay/upload_vp_file`，不支持云调用，权限集 157）：为道具图与投诉凭证换取 `file_id`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_upload_vp_file.html。

- **声明接口**：[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)
- **官方契约**：`POST /xpay/upload_vp_file`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayUploadVpFileRequest` — 请求体（`env` / `base64_img` / `img_url` / `file_name`），见 `WxaXPayUploadVpFileRequest`。
- **返回**：`WxaXPayUploadVpFileResponse` — 可供投诉回复引用的 `file_id`，见 `WxaXPayUploadVpFileResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，`Content-Type` 是 **JSON 而非 multipart** ⇒ / 图片走字段承载（`base64_img` 或 `img_url`），与本线短剧媒资的分片上传通路形态不同。
- **两支通道的上限不同且官方明示优先后者**：`base64_img` ≤1MB、`img_url` ≤2MB， / 且 `img_url` 必须能直接下载（**不能是 302 类跳转地址**）。
- **参数表的必填标记与「二选一」语义冲突**：官方把 `base64_img` 与 `img_url` 都标为必填， / 而正文语义是「用哪个传哪个」⇒ 本 SDK 照参数表保留两支可空属性，由宿主按通道赋值， / **不**把其一并列成必填（该取舍由守卫 XP3 登记）。
- **`268490007` 是本页最常见的失败形态**（图片或文字存在敏感内容）⇒ 道具图与凭证图同受内容审核。

### 27. `QueryOrderAsync` — 查询创建的订单（`POST /xpay/query_order`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_order.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/query_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQueryOrderRequest` — 请求体（`openid` / `env` 必填，`order_id` 与 `wx_order_id` 二选一），见 `WxaXPayQueryOrderRequest`。
- **返回**：`WxaXPayQueryOrderResponse` — 订单详情袋（23 字段，见 `WxaXPayOrderInfo`），见 `WxaXPayQueryOrderResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig` Query；本页也是本域字段面最宽的订单模型出处。
- **退款进度的官方口径是 `status` 的 5 / 7 / 8 三值**（`WxaXPayOrderStatuses`）： / 5 只代表商户侧扣款完成，用户侧收款完成才是 8 ⇒ 把 5 当「退款完成」会早于用户实际到账。
- **结算相关三支（`sett_time` / `sett_state` / 两支服务费）互相约束**：服务费仅 / `sett_state = 2` 时有值 ⇒ 分账核算须先判结算状态。
- **敏感值**：响应内 `token` 是米大师下单凭证 ⇒ 不得进日志、遥测或异常消息。

### 28. `RefundOrderAsync` — 启动订单退款任务（`POST /xpay/refund_order`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_refund_order.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/refund_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |
| `signature` | Query `signature` | `string?` | 用户态签名（官方 Query 参数 `signature`）。<br>**官方本页自相矛盾**：「注意事项」原文写「使用用户态签名与支付签名」，而「查询参数」表只列<br>`access_token` 与 `pay_sig` 两支（调用示例亦然）⇒ 本 SDK 以**参数表为准**把 `pay_sig` 定为必填、<br>以**注意事项为准**额外开放可选的 `signature`（默认不发送），宿主若被官方判 `268490003`（签名错误）<br>时应补传该支。该取舍由守卫 XP3 显式登记，**不得**当成「SDK 漏参」。 |

- **请求体**：`WxaXPayRefundOrderRequest` — 请求体（`left_fee` 须等于查询到的剩余可退金额），见 `WxaXPayRefundOrderRequest`。
- **返回**：`WxaXPayRefundOrderResponse` — 退款单与关联支付单的双侧单号，见 `WxaXPayRefundOrderResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig` Query；「启动任务」四字是语义核心 —— 本端点**同步只回单号**， / 到账结果须回 `QueryOrderAsync` 看 `status` 8。
- **三个退款约束（官方错误码反推）**：已核销单禁退 `268490013`、金额不足 `268490006`、 / `left_fee` 与实际不符 `268490016` ⇒ 退款前必须查单；进行中 `268490014` 官方原文 / 「稍后可以使用相同参数重试」⇒ **必须**原参重试，换 `refund_order_id` 重发会重复退款。
- **退款原因与来源两支是官方必填的审计字段**（`refund_reason` / `req_from`）⇒ 不得填空串糊过去。

### 29. `NotifyProvideGoodsAsync` — 通知已发货完成（`POST /xpay/notify_provide_goods`，不支持云调用，权限集 157、**无签名 Query 参数**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_notify_provide_goods.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/notify_provide_goods`

- **请求体**：`WxaXPayNotifyProvideGoodsRequest` — 请求体（`order_id` 与 `wx_order_id` 二选一），见 `WxaXPayNotifyProvideGoodsRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg`（页面原文返回体「无」）⇒ 直接返回 `WxaResponse`，不造 DTO。

**备注（原文）**

- 官方契约：**POST**，Query 只有 `access_token` ⇒ 本端点是本域「无签名 + 无返回体」双例外端点。
- **虚拟商品的「发货」是合规动作而非物流动作**：订单状态机里 2（待发货）→ 3（发货中）→ 4（已发货） / 由本端点驱动（`WxaXPayOrderStatuses`），未通知发货的订单官方侧不结算 ⇒ / 交付完成后漏调本端点会直接压住资金。
- 官方明写本页「此接口没有特殊错误码」⇒ 只有通用错误码。

### 30. `StartDownloadOrderAsync` — 下载支付订单（创建账单下载任务，`POST /xpay/start_download_order`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_download_order.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/start_download_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayStartDownloadOrderRequest` — 请求体（区间不超 31 天；`is_provided` 与 `refund_status` 按 `order_type` 条件必填），见 `WxaXPayStartDownloadOrderRequest`。
- **返回**：`WxaXPayStartDownloadOrderResponse` — 任务 id（须再走 `QueryDownloadOrderAsync` 取链接），见 `WxaXPayStartDownloadOrderResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；本端点是**异步任务创建**，官方明写本页无特殊错误码 ⇒ / 失败只能通过轮询任务状态发现，不能假定同步返回即成功。
- **四支筛选参数的适用面官方写在说明列**（无枚举表，故不建常量类）： / `order_type` 1 代币交易 / 2 道具直购 / 3 会员订阅 / 4 退款；`refund_status` 仅 `order_type = 4` / 有效（0 全部 / 2 已退款 / 4 退款中 / 5 退款失败，默认 0）；`is_provided` 在 2/3 时必传（默认 true）； / `pay_channel` 1 普通虚拟支付 / 2 苹果 IAP 且**必填**。
- **官方类型列把 `is_provided` 标为 `boolean`** ⇒ 线格式落 `bool?`，与短剧域同类处置一致。

### 31. `QueryDownloadOrderAsync` — 查询下载订单任务（`POST /xpay/query_download_order`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_download_order.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/query_download_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQueryDownloadOrderRequest` — 请求体（`task_id` / `env` 必填），见 `WxaXPayQueryDownloadOrderRequest`。
- **返回**：`WxaXPayQueryDownloadOrderResponse` — 任务状态 + 下载链接 + 失效时刻，见 `WxaXPayQueryDownloadOrderResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；官方明写本页无特殊错误码，失败形态是任务状态未就绪而非报错。
- **`download_url` 必须即时消费**：官方给 `expire_at` 字段本身就说明链接有时效 ⇒ / 不得持久化后复用，也**不得**写进日志、遥测或异常消息（与广告金订单下载链接同处置）。

### 32. `DownloadBillAsync` — 下载小程序账单（`POST /xpay/download_bill`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_bill.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/download_bill`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayDownloadBillRequest` — 请求体（`begin_ds` / `end_ds`，形如 `20230801`），见 `WxaXPayDownloadBillRequest`。
- **返回**：`WxaXPayDownloadUrlResponse` — 账单下载链接（与广告金商户订单下载共用应答形态），见 `WxaXPayDownloadUrlResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，请求只有日期区间两支 ⇒ 与 `StartDownloadOrderAsync` / 的区别是**本端点同步回链接、不提供类型筛选**。
- **`begin_ds`/`end_ds` 官方类型是 `number`**（日期写作数字）⇒ 线格式照官方落 `long?`， / 与投诉侧 `begin_date` 的 `YYYY-MM-DD` 字符串不同形。

### 33. `DownloadIosSettlementBillAsync` — 下载虚拟支付 iOS 月结账单（`POST /xpay/download_ios_settlement_bill`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_ios_settlement_bill.html。

- **声明接口**：[IWxaXPayOrderService](./IWxaXPayOrderService.cs)
- **官方契约**：`POST /xpay/download_ios_settlement_bill`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayDownloadIosSettlementBillRequest` — 请求体（`start_month` / `end_month`，形如 `YYYYMM`），见 `WxaXPayDownloadIosSettlementBillRequest`。
- **返回**：`WxaXPayDownloadIosSettlementBillResponse` — 逐月账单条目（`month` + `bill_url`），见 `WxaXPayDownloadIosSettlementBillResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`，一次请求返回**区间内逐月列表**（不是单条）⇒ / 与 `DownloadBillAsync` 的单链接形态不同。
- **本页官方明写无特殊错误码**，且 iOS 支只走苹果 IAP 通道（与订单模型 `order_type` 7/8 对应） / ⇒ 用安卓流水去对月结账单必然对不平。
- **失效时长官方未给数字**（原文「及时使用，一定时间后失效」）⇒ 不得套用 `cos_url` 的 30 分钟口径。

### 34. `SendSubscribePrePaymentAsync` — 预通知扣款（`POST /xpay/send_subscribe_pre_payment`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_send_subscribe_pre_payment.html。

- **声明接口**：[IWxaXPaySubscribeService](./IWxaXPaySubscribeService.cs)
- **官方契约**：`POST /xpay/send_subscribe_pre_payment`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPaySendSubscribePrePaymentRequest` — 请求体（`openid` / `deduct_price` / `product_id` / `out_contract_code` 四支全必填），见 `WxaXPaySendSubscribePrePaymentRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`，不造 DTO。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；**本端点参数表没有 `env` 支**（本族四端点里唯一有 / `deduct_price` 下限的一支：官方原文「金额/分，属于 [100，道具价格]」⇒ 低于 1 元的预通知被 / `674690001` 拒绝）。
- **预通知不等于扣款**：本调用只向用户推送扣款提醒，实际资金动作走 `SubmitSubscribePayOrderAsync`； / 官方 `690000001`「not allowed to trigger pre payment at this time」说明提醒有**时间窗约束**， / 不是随时可发。
- **`690000000`（未签约）是本页最常见的失败形态** ⇒ 扣款前应先以 / `QuerySubscribeContractAsync` 确认 `SIGNED`。

### 35. `SubmitSubscribePayOrderAsync` — 发起订阅扣款（`POST /xpay/submit_subscribe_pay_order`，不支持云调用，权限集 157）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_submit_subscribe_pay_order.html。

- **声明接口**：[IWxaXPaySubscribeService](./IWxaXPaySubscribeService.cs)
- **官方契约**：`POST /xpay/submit_subscribe_pay_order`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPaySubmitSubscribePayOrderRequest` — 请求体（九支除 `env` 外全必填），见 `WxaXPaySubmitSubscribePayOrderRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`；扣款结果须回 `IWxaXPayOrderService.QueryOrderAsync` 查单。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；本端点是本域**请求面最宽**的一支（9 字段），且 / `offer_id` 是米大师侧应用 id（公众号支付基础配置里的 offerid）、`currency_type` 官方目前只接受 / `CNY`（`-15004`）。
- **`env` 在本端点上只能填 0**（官方原文「环境配置, 0 正式环境, 仅支持正式环境」，错误码 / `-15011` 明示沙箱值被拒）⇒ 与本页参数表「否（选填）」的标注并不冲突，但**不可**据此认为沙箱可用。
- **错误码表与全域不同形**：本端点回米大师侧负数码 `-15001 ~ -15027` 与 `1001`， / 其中 `-15002`（单号重复）官方原文「请换新单号重试」与 `SendSubscribePrePaymentAsync` / 侧 `-15025`（不存在预通知单/重复下单）共同说明**单号不可复用**；而 `-15012` / （调用米大师失败导致关单）同样要求换新单号 ⇒ 幂等重放必须原单号，这两类必须换单号， / **宿主须按码值分流而非一律重试**。
- **官方自注唯一性校验不可强依赖**（原文「不建议业务强依赖平台对这里的唯一性校验」）⇒ 幂等责任在调用方单号体系。
- **发布生效延迟**：`-15014` 原文「道具/代币发布未生效，禁止下单，大概 10 分钟后生效」⇒ / 刚发布的道具立即扣款会失败，这是官方正文里本域唯一的**延迟数字**，照录不做推断。

### 36. `QuerySubscribeContractAsync` — 查询签约关系（`POST /xpay/query_subscribe_contract`，不支持云调用，权限集 157、**官方明写无特殊错误码**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_subscribe_contract.html。

- **声明接口**：[IWxaXPaySubscribeService](./IWxaXPaySubscribeService.cs)
- **官方契约**：`POST /xpay/query_subscribe_contract`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayQuerySubscribeContractRequest` — 请求体（`openid` / `product_id` / `out_contract_code` 三支全必填），见 `WxaXPayQuerySubscribeContractRequest`。
- **返回**：`WxaXPayQuerySubscribeContractResponse` — 签约状态字符串（三取值官方写在说明列），见 `WxaXPayQuerySubscribeContractResponse`。

**备注（原文）**

- 官方契约：**POST** + `pay_sig`；本端点**参数表没有 `env` 支**且官方明写「此接口没有特殊错误码」 / ⇒ 失败只可能是通用码（令牌/签名），签约关系不存在时返回的是状态值而不是错误。
- **三状态语义直接决定能否再扣款**：`SIGNED` 生效中、`TERMINATED` 已解约且**该协议号到达终态** / （官方原文「该协议无法再次进行签约；可更换协议号再发起签约」）、`UNBINDUSER` 从未签约 ⇒ / 把 `TERMINATED` 当「可恢复」是错误用法，须换新 `out_contract_code`。
- **官方无枚举表**（三值写在字段说明列）⇒ 按 XP5 口径不建常量类，取值照录进 DTO 注释。

### 37. `CancelSubscribeContractAsync` — 商家解约（`POST /xpay/cancel_subscribe_contract`，不支持云调用，**官方页面完全没有第三方调用段**、
因而**无权限集 id**，且官方明写无特殊错误码）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_cancel_subscribe_contract.html。

- **声明接口**：[IWxaXPaySubscribeService](./IWxaXPaySubscribeService.cs)
- **官方契约**：`POST /xpay/cancel_subscribe_contract`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `paySig` | Query `pay_sig` | `string` | 支付签名（官方 Query 参数 `pay_sig`，必填）。 |

- **请求体**：`WxaXPayCancelSubscribeContractRequest` — 请求体（`openid` / `termination_reason` / `product_id` / `out_contract_code` 四支全必填），见 `WxaXPayCancelSubscribeContractRequest`。
- **返回**：`WxaResponse` — 官方应答体只有 `errcode` / `errmsg` ⇒ 直接返回 `WxaResponse`。

**备注（原文）**

- **本端点是全域 37 端点里唯一「无第三方调用段」的一支**：官方页面既没有「支持第三方平台代商家调用」小节， / 也就**没有权限集 id** ⇒ 本 SDK 不宣称代商家调用可用，代调用方若需该能力须先取得官方口径。 / 该空缺是官方正文口径，**不得**凭第三方 SDK 注释推断成「同 157」。
- 官方契约：**POST** + `pay_sig`，请求体**没有 `env` 支**。
- **`termination_reason` 是官方必填的审计字段** ⇒ 不得填空串；解约动作对用户的直接后果是后续扣款 / 被 `QuerySubscribeContractAsync` 读成 `TERMINATED`，且该协议号**不可复用**。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/XPay`](../../../Mud.Wechat.MiniProgram/XPay/) 内共 **83** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **83** 个，`SerializerClassName` = `XPay`。

AOT 源生成上下文：[`XPayJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/XPayJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaXPayGetComplaintListResponse` | 应答 | [XPay/Complaint/WxaXPayGetComplaintListResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetComplaintListResponse.cs) | `total`, `complaints` |
| `WxaXPayGetComplaintListRequest` | 请求体 | [XPay/Complaint/WxaXPayGetComplaintListRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetComplaintListRequest.cs) | `env`, `begin_date`, `end_date`, `offset`, `limit` |
| `WxaXPayGetComplaintDetailResponse` | 应答 | [XPay/Complaint/WxaXPayGetComplaintDetailResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetComplaintDetailResponse.cs) | `complaint` |
| `WxaXPayGetComplaintDetailRequest` | 请求体 | [XPay/Complaint/WxaXPayGetComplaintDetailRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetComplaintDetailRequest.cs) | `env`, `complaint_id` |
| `WxaXPayGetNegotiationHistoryResponse` | 应答 | [XPay/Complaint/WxaXPayGetNegotiationHistoryResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetNegotiationHistoryResponse.cs) | `total`, `history` |
| `WxaXPayGetNegotiationHistoryRequest` | 请求体 | [XPay/Complaint/WxaXPayGetNegotiationHistoryRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetNegotiationHistoryRequest.cs) | `env`, `complaint_id`, `offset`, `limit` |
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaXPayResponseComplaintRequest` | 请求体 | [XPay/Complaint/WxaXPayResponseComplaintRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayResponseComplaintRequest.cs) | `env`, `complaint_id`, `response_content`, `response_images` |
| `WxaXPayCompleteComplaintRequest` | 请求体 | [XPay/Complaint/WxaXPayCompleteComplaintRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayCompleteComplaintRequest.cs) | `env`, `complaint_id` |
| `WxaXPayGetUploadFileSignResponse` | 应答 | [XPay/Complaint/WxaXPayGetUploadFileSignResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetUploadFileSignResponse.cs) | `sign`, `cos_url` |
| `WxaXPayGetUploadFileSignRequest` | 请求体 | [XPay/Complaint/WxaXPayGetUploadFileSignRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayGetUploadFileSignRequest.cs) | `env`, `wxpay_url`, `convert_cos`, `complaint_id` |
| `WxaXPayQueryPunishmentReasonsResponse` | 应答 | [XPay/Complaint/WxaXPayQueryPunishmentReasonsResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Complaint/WxaXPayQueryPunishmentReasonsResponse.cs) | `appid`, `nickname`, `merchant_code`, `limited_functions`, `other_limited_functions`, `recovery_specifications` |
| `WxaXPayCurrencyPayResponse` | 应答 | [XPay/Currency/WxaXPayCurrencyPayResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayCurrencyPayResponse.cs) | `order_id`, `balance`, `used_present_amount` |
| `WxaXPayCurrencyPayRequest` | 请求体 | [XPay/Currency/WxaXPayCurrencyPayRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayCurrencyPayRequest.cs) | `openid`, `env`, `user_ip`, `amount`, `order_id`, `payitem`, `remark` |
| `WxaXPayCancelCurrencyPayResponse` | 应答 | [XPay/Currency/WxaXPayCancelCurrencyPayResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayCancelCurrencyPayResponse.cs) | `order_id` |
| `WxaXPayCancelCurrencyPayRequest` | 请求体 | [XPay/Currency/WxaXPayCancelCurrencyPayRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayCancelCurrencyPayRequest.cs) | `openid`, `env`, `user_ip`, `pay_order_id`, `order_id`, `amount` |
| `WxaXPayPresentCurrencyResponse` | 应答 | [XPay/Currency/WxaXPayPresentCurrencyResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayPresentCurrencyResponse.cs) | `balance`, `order_id`, `present_balance` |
| `WxaXPayPresentCurrencyRequest` | 请求体 | [XPay/Currency/WxaXPayPresentCurrencyRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayPresentCurrencyRequest.cs) | `openid`, `env`, `order_id`, `amount` |
| `WxaXPayQueryUserBalanceResponse` | 应答 | [XPay/Currency/WxaXPayQueryUserBalanceResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayQueryUserBalanceResponse.cs) | `balance`, `present_balance`, `sum_save`, `sum_present`, `sum_balance`, `sum_cost`, `first_save_flag` |
| `WxaXPayQueryUserBalanceRequest` | 请求体 | [XPay/Currency/WxaXPayQueryUserBalanceRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Currency/WxaXPayQueryUserBalanceRequest.cs) | `openid`, `env`, `user_ip` |
| `WxaXPayBindTransferAccountRequest` | 请求体 | [XPay/Funds/WxaXPayBindTransferAccountRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayBindTransferAccountRequest.cs) | `transfer_account_uid`, `transfer_account_org_name`, `env` |
| `WxaXPayQueryTransferAccountResponse` | 应答 | [XPay/Funds/WxaXPayQueryTransferAccountResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryTransferAccountResponse.cs) | `acct_list` |
| `WxaXPayQueryTransferAccountRequest` | 请求体 | [XPay/Funds/WxaXPayQueryTransferAccountRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryTransferAccountRequest.cs) | `env` |
| `WxaXPayCreateFundsBillResponse` | 应答 | [XPay/Funds/WxaXPayCreateFundsBillResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayCreateFundsBillResponse.cs) | `bill_id` |
| `WxaXPayCreateFundsBillRequest` | 请求体 | [XPay/Funds/WxaXPayCreateFundsBillRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayCreateFundsBillRequest.cs) | `transfer_amount`, `transfer_account_uid`, `transfer_account_name`, `transfer_account_agency_id`, `request_id`, `settle_begin`, `settle_end`, `env`, `authorize_advertise`, `fund_type` |
| `WxaXPayQueryFundsBillResponse` | 应答 | [XPay/Funds/WxaXPayQueryFundsBillResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryFundsBillResponse.cs) | `bill_list`, `total_page` |
| `WxaXPayQueryFundsBillRequest` | 请求体 | [XPay/Funds/WxaXPayQueryFundsBillRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryFundsBillRequest.cs) | `page`, `page_size`, `filter`, `env` |
| `WxaXPayQueryAdverFundsResponse` | 应答 | [XPay/Funds/WxaXPayQueryAdverFundsResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryAdverFundsResponse.cs) | `adver_funds_list`, `total_page` |
| `WxaXPayQueryAdverFundsRequest` | 请求体 | [XPay/Funds/WxaXPayQueryAdverFundsRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryAdverFundsRequest.cs) | `page`, `page_size`, `filter`, `env` |
| `WxaXPayQueryRecoverBillResponse` | 应答 | [XPay/Funds/WxaXPayQueryRecoverBillResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryRecoverBillResponse.cs) | `bill_list`, `total_page` |
| `WxaXPayQueryRecoverBillRequest` | 请求体 | [XPay/Funds/WxaXPayQueryRecoverBillRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryRecoverBillRequest.cs) | `page`, `page_size`, `filter`, `env` |
| `WxaXPayDownloadUrlResponse` | 应答 | [XPay/Order/WxaXPayDownloadUrlResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayDownloadUrlResponse.cs) | `url` |
| `WxaXPayDownloadAdverfundsOrderRequest` | 请求体 | [XPay/Funds/WxaXPayDownloadAdverfundsOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayDownloadAdverfundsOrderRequest.cs) | `fund_id`, `env` |
| `WxaXPayQueryBizBalanceResponse` | 应答 | [XPay/Funds/WxaXPayQueryBizBalanceResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryBizBalanceResponse.cs) | `balance_available` |
| `WxaXPayQueryBizBalanceRequest` | 请求体 | [XPay/Funds/WxaXPayQueryBizBalanceRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryBizBalanceRequest.cs) | `env` |
| `WxaXPayCreateWithdrawOrderResponse` | 应答 | [XPay/Funds/WxaXPayCreateWithdrawOrderResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayCreateWithdrawOrderResponse.cs) | `withdraw_no`, `wx_withdraw_no` |
| `WxaXPayCreateWithdrawOrderRequest` | 请求体 | [XPay/Funds/WxaXPayCreateWithdrawOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayCreateWithdrawOrderRequest.cs) | `withdraw_no`, `withdraw_amount`, `env` |
| `WxaXPayQueryWithdrawOrderResponse` | 应答 | [XPay/Funds/WxaXPayQueryWithdrawOrderResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryWithdrawOrderResponse.cs) | `withdraw_no`, `status`, `withdraw_amount`, `wx_withdraw_no`, `withdraw_success_timestamp`, `create_time`, `fail_reason` |
| `WxaXPayQueryWithdrawOrderRequest` | 请求体 | [XPay/Funds/WxaXPayQueryWithdrawOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Funds/WxaXPayQueryWithdrawOrderRequest.cs) | `withdraw_no`, `env` |
| `WxaXPayStartUploadGoodsRequest` | 请求体 | [XPay/Goods/WxaXPayStartUploadGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayStartUploadGoodsRequest.cs) | `upload_item`, `env` |
| `WxaXPayQueryUploadGoodsResponse` | 应答 | [XPay/Goods/WxaXPayQueryUploadGoodsResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayQueryUploadGoodsResponse.cs) | `upload_item`, `status` |
| `WxaXPayQueryUploadGoodsRequest` | 请求体 | [XPay/Goods/WxaXPayQueryUploadGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayQueryUploadGoodsRequest.cs) | `env` |
| `WxaXPayStartPublishGoodsRequest` | 请求体 | [XPay/Goods/WxaXPayStartPublishGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayStartPublishGoodsRequest.cs) | `publish_item`, `env` |
| `WxaXPayQueryPublishGoodsResponse` | 应答 | [XPay/Goods/WxaXPayQueryPublishGoodsResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayQueryPublishGoodsResponse.cs) | `publish_item`, `status` |
| `WxaXPayQueryPublishGoodsRequest` | 请求体 | [XPay/Goods/WxaXPayQueryPublishGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayQueryPublishGoodsRequest.cs) | `env` |
| `WxaXPayUploadVpFileResponse` | 应答 | [XPay/Goods/WxaXPayUploadVpFileResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayUploadVpFileResponse.cs) | `file_id` |
| `WxaXPayUploadVpFileRequest` | 请求体 | [XPay/Goods/WxaXPayUploadVpFileRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Goods/WxaXPayUploadVpFileRequest.cs) | `env`, `base64_img`, `img_url`, `file_name` |
| `WxaXPayQueryOrderResponse` | 应答 | [XPay/Order/WxaXPayQueryOrderResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayQueryOrderResponse.cs) | `order` |
| `WxaXPayQueryOrderRequest` | 请求体 | [XPay/Order/WxaXPayQueryOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayQueryOrderRequest.cs) | `openid`, `env`, `order_id`, `wx_order_id` |
| `WxaXPayRefundOrderResponse` | 应答 | [XPay/Order/WxaXPayRefundOrderResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayRefundOrderResponse.cs) | `refund_order_id`, `refund_wx_order_id`, `pay_order_id`, `pay_wx_order_id` |
| `WxaXPayRefundOrderRequest` | 请求体 | [XPay/Order/WxaXPayRefundOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayRefundOrderRequest.cs) | `openid`, `order_id`, `wx_order_id`, `refund_order_id`, `left_fee`, `refund_fee`, `biz_meta`, `refund_reason`, `req_from`, `env` |
| `WxaXPayNotifyProvideGoodsRequest` | 请求体 | [XPay/Order/WxaXPayNotifyProvideGoodsRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayNotifyProvideGoodsRequest.cs) | `order_id`, `wx_order_id`, `env` |
| `WxaXPayStartDownloadOrderResponse` | 应答 | [XPay/Order/WxaXPayStartDownloadOrderResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayStartDownloadOrderResponse.cs) | `task_id` |
| `WxaXPayStartDownloadOrderRequest` | 请求体 | [XPay/Order/WxaXPayStartDownloadOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayStartDownloadOrderRequest.cs) | `begin_ds`, `end_ds`, `order_type`, `order_info`, `is_provided`, `refund_status`, `env`, `pay_channel` |
| `WxaXPayQueryDownloadOrderResponse` | 应答 | [XPay/Order/WxaXPayQueryDownloadOrderResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayQueryDownloadOrderResponse.cs) | `task_id`, `status`, `download_url`, `expire_at` |
| `WxaXPayQueryDownloadOrderRequest` | 请求体 | [XPay/Order/WxaXPayQueryDownloadOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayQueryDownloadOrderRequest.cs) | `task_id`, `env` |
| `WxaXPayDownloadBillRequest` | 请求体 | [XPay/Order/WxaXPayDownloadBillRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayDownloadBillRequest.cs) | `begin_ds`, `end_ds` |
| `WxaXPayDownloadIosSettlementBillResponse` | 应答 | [XPay/Order/WxaXPayDownloadIosSettlementBillResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayDownloadIosSettlementBillResponse.cs) | `bill_list` |
| `WxaXPayDownloadIosSettlementBillRequest` | 请求体 | [XPay/Order/WxaXPayDownloadIosSettlementBillRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Order/WxaXPayDownloadIosSettlementBillRequest.cs) | `start_month`, `end_month` |
| `WxaXPaySendSubscribePrePaymentRequest` | 请求体 | [XPay/Subscribe/WxaXPaySendSubscribePrePaymentRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Subscribe/WxaXPaySendSubscribePrePaymentRequest.cs) | `openid`, `deduct_price`, `product_id`, `out_contract_code` |
| `WxaXPaySubmitSubscribePayOrderRequest` | 请求体 | [XPay/Subscribe/WxaXPaySubmitSubscribePayOrderRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Subscribe/WxaXPaySubmitSubscribePayOrderRequest.cs) | `openid`, `offer_id`, `buy_quantity`, `env`, `currency_type`, `product_id`, `deduct_price`, `order_id`, `attach` |
| `WxaXPayQuerySubscribeContractResponse` | 应答 | [XPay/Subscribe/WxaXPayQuerySubscribeContractResponse.cs](../../../Mud.Wechat.MiniProgram/XPay/Subscribe/WxaXPayQuerySubscribeContractResponse.cs) | `authorization_state` |
| `WxaXPayQuerySubscribeContractRequest` | 请求体 | [XPay/Subscribe/WxaXPayQuerySubscribeContractRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Subscribe/WxaXPayQuerySubscribeContractRequest.cs) | `openid`, `product_id`, `out_contract_code` |
| `WxaXPayCancelSubscribeContractRequest` | 请求体 | [XPay/Subscribe/WxaXPayCancelSubscribeContractRequest.cs](../../../Mud.Wechat.MiniProgram/XPay/Subscribe/WxaXPayCancelSubscribeContractRequest.cs) | `openid`, `termination_reason`, `product_id`, `out_contract_code` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.XPay;
using Mud.Wechat.MiniProgram.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddXPayApi());

var service = app.Services.GetRequiredService<IWxaXPayFundsService>();
var ct = CancellationToken.None;
var response = await service.BindTransferAccountAsync(new WxaXPayBindTransferAccountRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/xpay/bind_transfer_accout");
// 占位值仅用于编译示意；业务值须按 IWxaXPayFundsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`BindTransferAccountAsync`）；请求体 `WxaXPayBindTransferAccountRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaXPayComplaintService](./IWxaXPayComplaintService.cs)、[IWxaXPayCurrencyService](./IWxaXPayCurrencyService.cs)、[IWxaXPayFundsService](./IWxaXPayFundsService.cs)、[IWxaXPayGoodsService](./IWxaXPayGoodsService.cs)、[IWxaXPayOrderService](./IWxaXPayOrderService.cs)、[IWxaXPaySubscribeService](./IWxaXPaySubscribeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaXPayContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaXPayContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `XPayEndpoints_ShouldMatchOfficialRoutes`、`XPayInterfaces_ShouldBindMpAccessToken_ShareRegistryGroup_AndLockSignatureParameterFace`、`XPayOfficialContradictionsAndGaps_ShouldStayRegistered`、`XPayRoutes_ShouldBeDeclaredOnlyInMiniProgramLine`、`XPayDataModels_ShouldLockOfficialFieldsTypesAndJsonRegistration`、`XPayCredentialQueryParameters_ShouldBeRedactedByRegistrationNotVocabulary`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/XPay`](../../../Mud.Wechat.MiniProgram/XPay/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaXPayContractGuards.cs`、`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
