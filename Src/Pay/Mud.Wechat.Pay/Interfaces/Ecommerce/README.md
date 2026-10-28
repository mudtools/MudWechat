# Ecommerce 域 · 平台收付通 · 账户注销 / 个人收款方 / 主体子商户

微信支付「平台收付通 · 账户注销 / 个人收款方 / 主体子商户」域 SDK（**10 端点**：
注销并提现 5 + 子商户注销申请 2 + 个人收款方签约查询 2 + 主体名下子商户列表 1）。 接口：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)、[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)、[IWechatPayEcommerceBillService](./IWechatPayEcommerceBillService.cs)、[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)、[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)、[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)、[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)、[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)；注册组 `Ecommerce`；模块枚举 `PayModule.Ecommerce`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayEcommerceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEcommerceContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPlatsolutionEcommerceCompensateContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceCompensateContractGuards.cs)、[WechatPayPlatsolutionEcommerceDepositContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceDepositContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/Ecommerce`](../../../Mud.Wechat.Pay/Ecommerce/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayEcommerceAccountService = 10；IWechatPayEcommerceApplymentService = 7；IWechatPayEcommerceBillService = 1；IWechatPayEcommerceFundService = 5；IWechatPayEcommerceMchTransferService = 6；IWechatPayEcommercePapayService = 6；IWechatPayEcommerceProfitSharingService = 11；IWechatPayEcommerceRefundService = 9；按 HTTP 特性实计 **55** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayEcommerceAccountService` | `ApplyCancelWithdrawAsync` | POST | `/v3/ecommerce/account/apply-cancel-withdraw` | 商户证书 / APIv3 签名 | 「商户号注销并提现申请」：代被管控的二级商户发起注销、并把账户余额打款到商户银行卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4… |
| 2 | `IWechatPayEcommerceAccountService` | `QueryCancelWithdrawByApplymentIdAsync` | GET | `/v3/ecommerce/account/apply-cancel-withdraw/applyment-id/{applymentId}` | 商户证书 / APIv3 签名 | 「按微信支付注销单号查询注销提现申请」：读审批与出款进度。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013892765。 |
| 3 | `IWechatPayEcommerceAccountService` | `QueryCancelWithdrawByOutRequestNoAsync` | GET | `/v3/ecommerce/account/apply-cancel-withdraw/out-request-no/{outRequestNo}` | 商户证书 / APIv3 签名 | 「按商户注销申请单号查询注销提现申请」：与上一支**同一应答类型**，定位键换成商户自有单号。<br>官方文档：https://pay.weixin.qq.com/doc/v3/part… |
| 4 | `IWechatPayEcommerceAccountService` | `ConfirmCancelWithdrawAsync` | POST | `/v3/ecommerce/account/apply-cancel-withdraw/confirm` | 商户证书 / APIv3 签名 | 「平台代商户确认注销」：单据处于「待商户确认注销」时代商户执行确认动作。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013892… |
| 5 | `IWechatPayEcommerceAccountService` | `ValidateCancelAsync` | GET | `/v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{subMchId}` | 商户证书 / APIv3 签名 | 「注销资格前置校验」：发起注销提现前判断商户是否满足注销条件，并带回各账户实时余额与阻塞原因。<br>官方文档：https://pay.weixin.qq.com/doc/v3/part… |
| 6 | `IWechatPayEcommerceAccountService` | `ApplyCancelAsync` | POST | `/v3/ecommerce/account/cancel-applications` | 商户证书 / APIv3 签名 | 「电商子商户注销申请」：提交普通注销申请（资料以 `application_media_id` 承载）。<br>官方文档：https://pay.weixin.qq.com/doc/v3… |
| 7 | `IWechatPayEcommerceAccountService` | `QueryCancelApplicationAsync` | GET | `/v3/ecommerce/account/cancel-applications/out-apply-no/{outApplyNo}` | 商户证书 / APIv3 签名 | 「查询子商户注销申请状态」：按商户申请单号读注销进展。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476223。 |
| 8 | `IWechatPayEcommerceAccountService` | `QueryIndividualContractByOpenIdAsync` | GET | `/v3/ecommerce/individual-contracts/{openId}` | 商户证书 / APIv3 签名 | 「查询个人收款方授权结果」：按用户 `openid` 取该用户在平台下的个人收款方授权状态与授权 ID。<br>官方文档：https://pay.weixin.qq.com/doc/v3… |
| 9 | `IWechatPayEcommerceAccountService` | `QueryIndividualContractByOutContractCodeAsync` | GET | `/v3/ecommerce/individual-contracts/out-contract-code/{outContractCode}` | 商户证书 / APIv3 签名 | 「按业务申请单号查询个人收款方签约详情」：用户完成预签约后取签约信息、用户 OpenID 与实名授权结果。<br>官方文档：https://pay.weixin.qq.com/doc/v… |
| 10 | `IWechatPayEcommerceAccountService` | `QuerySubjectSubMerchantsAsync` | GET | `/v3/ecommerce/subject-sub-merchants` | 商户证书 / APIv3 签名 | 「查询主体名下子商户列表」：按主体类型 + 证件号码定位主体，取其未注销且与当前服务商有效绑定的子商户。<br>官方文档：https://pay.weixin.qq.com/doc/v3… |
| 11 | `IWechatPayEcommerceApplymentService` | `SubmitApplymentAsync` | POST | `/v3/ecommerce/applyments/` | 商户证书 / APIv3 签名 | 「二级商户进件」：平台商户代其二级商户提交入驻申请，成为微信支付商户（**受理语义，须轮询查状态**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3… |
| 12 | `IWechatPayEcommerceApplymentService` | `QueryApplymentByApplymentIdAsync` | GET | `/v3/ecommerce/applyments/{applymentId}` | 商户证书 / APIv3 签名 | 「按申请单号查询进件状态」：官方要求提交后**定期轮询**至终态。<br>官方文档：[4012691469](https://pay.weixin.qq.com/doc/v3/partn… |
| 13 | `IWechatPayEcommerceApplymentService` | `QueryApplymentByOutRequestNoAsync` | GET | `/v3/ecommerce/applyments/out-request-no/{outRequestNo}` | 商户证书 / APIv3 签名 | 「按业务申请编号查询进件状态」：与 `QueryApplymentByApplymentIdAsync` 同一应答类型，只是定位键不同。<br>官方文档：[4012691376](htt… |
| 14 | `IWechatPayEcommerceApplymentService` | `RevokeApplymentByApplymentIdAsync` | POST | `/v3/ecommerce/applyments/{applymentId}/revoke` | 商户证书 / APIv3 签名 | 「按申请单号撤销进件申请」：仅在六个非终态下允许，**官方零 body 参数**。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40… |
| 15 | `IWechatPayEcommerceApplymentService` | `RevokeApplymentByOutRequestNoAsync` | POST | `/v3/ecommerce/applyments/out-request-no/{outRequestNo}/revoke` | 商户证书 / APIv3 签名 | 「按业务申请编号撤销进件申请」：与 `RevokeApplymentByApplymentIdAsync` 同契约，只是定位键不同。<br>官方文档：https://pay.weixin… |
| 16 | `IWechatPayEcommerceApplymentService` | `SubmitAccountHostingApplymentAsync` | POST | `/v3/ecommerce/applyments/submit-account-hosting-applyment` | 商户证书 / APIv3 签名 | 「账户托管进件」：账号托管类平台商户的进件入口，多出代理签约/代理核身模式声明（**受理语义**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/p… |
| 17 | `IWechatPayEcommerceApplymentService` | `SubmitSignVerifyInfoAsync` | POST | `/v3/ecommerce/applyments/submit-sign-verify-info` | 商户证书 / APIv3 签名 | 「提交代理签约核身信息」：账号托管进件里声明了「平台代理签约 + 代理核身入驻模式」后回填核身结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/p… |
| 18 | `IWechatPayEcommerceBillService` | `ApplyFundFlowBillAsync` | GET | `/v3/ecommerce/bill/fundflowbill` | 商户证书 / APIv3 签名 | 「申请二级商户资金账单」：取该电商平台二级商户资金流水账单的**分片下载地址列表**。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/… |
| 19 | `IWechatPayEcommerceFundService` | `QueryBalanceAsync` | GET | `/v3/ecommerce/fund/balance/{subMchId}` | 商户证书 / APIv3 签名 | 「查询二级商户账户实时余额」：按二级商户号取可用与不可用金额（单位**分**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401… |
| 20 | `IWechatPayEcommerceFundService` | `QueryDayEndBalanceAsync` | GET | `/v3/ecommerce/fund/enddaybalance/{subMchId}` | 商户证书 / APIv3 签名 | 「查询二级商户账户日终余额」：取指定日期当天 24 点的账面余额，与实时余额**同一应答类型**。<br>官方文档：https://pay.weixin.qq.com/doc/v3/pa… |
| 21 | `IWechatPayEcommerceFundService` | `ApplyWithdrawAsync` | POST | `/v3/ecommerce/fund/withdraw` | 商户证书 / APIv3 签名 | 「申请二级商户预约提现」：代二级商户发起账户预约提现（**受理应答没有状态**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40… |
| 22 | `IWechatPayEcommerceFundService` | `QueryWithdrawByWithdrawIdAsync` | GET | `/v3/ecommerce/fund/withdraw/{withdrawId}` | 商户证书 / APIv3 签名 | 「按微信支付提现单号查询预约提现状态」：读提现单终态（六值）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476665。 |
| 23 | `IWechatPayEcommerceFundService` | `QueryWithdrawByOutRequestNoAsync` | GET | `/v3/ecommerce/fund/withdraw/out-request-no/{outRequestNo}` | 商户证书 / APIv3 签名 | 「按商户预约提现单号查询预约提现状态」：与上一支**同一应答类型**，只是定位键换成商户侧单号。<br>官方文档：https://pay.weixin.qq.com/doc/v3/par… |
| 24 | `IWechatPayEcommerceMchTransferService` | `ApplyTransferAuthorizationAsync` | POST | `/v3/ecommerce/mch-transfer/authorizations` | 商户证书 / APIv3 签名 | 「申请二级商户商家转账授权」：上送已线下签署的转账授权协议清单（**受理语义，回 200 不等于已授权**）。<br>官方文档：https://pay.weixin.qq.com/doc… |
| 25 | `IWechatPayEcommerceMchTransferService` | `QueryTransferAuthorizationAsync` | GET | `/v3/ecommerce/mch-transfer/authorizations/{subMchId}` | 商户证书 / APIv3 签名 | 「查询二级商户商家转账授权状态」：转账发起前的前置校验入口。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504209。 |
| 26 | `IWechatPayEcommerceMchTransferService` | `CreateTransferBillAsync` | POST | `/v3/ecommerce/mch-transfer/transfer-bills` | 商户证书 / APIv3 签名 | 「发起商家转账」：平台商户代二级商户向用户 openid 转账，等待用户在微信收款确认页面确认收款。<br>官方文档：https://pay.weixin.qq.com/doc/v3/p… |
| 27 | `IWechatPayEcommerceMchTransferService` | `QueryMchTransferBillByOutBillNoAsync` | GET | `/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}` | 商户证书 / APIv3 签名 | 「按商户单号查询商家转账订单」：转账终态与失败原因的唯一权威来源。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504213… |
| 28 | `IWechatPayEcommerceMchTransferService` | `CancelTransferBillAsync` | POST | `/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel` | 商户证书 / APIv3 签名 | 「撤销转账」：在**用户确认收款之前**撤销付款请求（**异步受理**，返回成功只表示撤销请求已受理）。<br>官方文档：https://pay.weixin.qq.com/doc/v3… |
| 29 | `IWechatPayEcommerceMchTransferService` | `QueryMchTransferBillByTransferBillNoAsync` | GET | `/v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}` | 商户证书 / APIv3 签名 | 「按微信转账单号查询商家转账订单」：与 `QueryMchTransferBillByOutBillNoAsync` 同一应答类型，只是定位键不同。<br>官方文档：https://pa… |
| 30 | `IWechatPayEcommercePapayService` | `PreEntrustSignAsync` | POST | `/v3/ecommerce/combine-papay/contracts/pre-entrust-sign` | 商户证书 / APIv3 签名 | 「预签约」：外部 App 拉起微信客户端发起委托代扣签约前，须后台先调本接口换取签约会话 ID（**2 小时有效**）。<br>官方文档：https://pay.weixin.qq.co… |
| 31 | `IWechatPayEcommercePapayService` | `QueryPapayContractAsync` | GET | `/v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}` | 商户证书 / APIv3 签名 | 「查询签约协议」：签约状态的**唯一权威来源**（用户放弃签约时官方不通知）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012… |
| 32 | `IWechatPayEcommercePapayService` | `TerminatePapayContractAsync` | POST | `/v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate` | 商户证书 / APIv3 签名 | 「解约」：对**已生效**的委托代扣签约协议发起解约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884072。 |
| 33 | `IWechatPayEcommercePapayService` | `CreatePapayCombineTransactionAsync` | POST | `/v3/ecommerce/combine-papay/transactions` | 商户证书 / APIv3 签名 | 「合单免密扣款」：凭已生效的签约协议对多笔子单一并发起委托代扣（**同步扣费，商户侧超时窗口 1 分钟**）。<br>官方文档：https://pay.weixin.qq.com/doc… |
| 34 | `IWechatPayEcommercePapayService` | `QueryPapayCombineTransactionAsync` | GET | `/v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}` | 商户证书 / APIv3 签名 | 「查询合单扣款」：按商户合单号读合单与子单状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884130。 |
| 35 | `IWechatPayEcommercePapayService` | `ReversePapayCombineTransactionAsync` | POST | `/v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}/reverse` | 商户证书 / APIv3 签名 | 「撤销合单扣款」：对已发起的免密合单支付按子单粒度撤销（**官方应答零业务字段**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/… |
| 36 | `IWechatPayEcommerceProfitSharingService` | `CreateProfitSharingOrderAsync` | POST | `/v3/ecommerce/profitsharing/orders` | 商户证书 / APIv3 签名 | 「请求分账」：平台商户代二级商户把结算后的资金分给分账接收方（**异步受理**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40… |
| 37 | `IWechatPayEcommerceProfitSharingService` | `QueryProfitSharingOrderAsync` | GET | `/v3/ecommerce/profitsharing/orders` | 商户证书 / APIv3 签名 | 「查询分账结果」：按 `sub_mchid` + `transaction_id` + `out_order_no` 三键查分账单，<br>也是**分账完结结果的唯一观察点**。<br>官方文… |
| 38 | `IWechatPayEcommerceProfitSharingService` | `QueryUnsplitAmountAsync` | GET | `/v3/ecommerce/profitsharing/orders/{transactionId}/amounts` | 商户证书 / APIv3 签名 | 「查询订单剩余待分金额」：这笔微信订单还能分多少（单位分）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477751。 |
| 39 | `IWechatPayEcommerceProfitSharingService` | `FinishOrderAsync` | POST | `/v3/ecommerce/profitsharing/finish-order` | 商户证书 / APIv3 签名 | 「分账完结（解冻剩余资金）」：把订单剩余金额全部解冻给二级商户（**异步受理、应答无状态**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/par… |
| 40 | `IWechatPayEcommerceProfitSharingService` | `AddReceiverAsync` | POST | `/v3/ecommerce/profitsharing/receivers/add` | 商户证书 / APIv3 签名 | 「添加分账接收方」：在**电商平台维度**维护接收方列表（其他二级商户共用该列表）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4… |
| 41 | `IWechatPayEcommerceProfitSharingService` | `DeleteReceiverAsync` | POST | `/v3/ecommerce/profitsharing/receivers/delete` | 商户证书 / APIv3 签名 | 「删除分账接收方」：删除后不能再把二级商户的资金分给该接收方。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477759。 |
| 42 | `IWechatPayEcommerceProfitSharingService` | `CreateProfitSharingReturnAsync` | POST | `/v3/ecommerce/profitsharing/returnorders` | 商户证书 / APIv3 签名 | 「请求分账回退」：退款前把已分给接收方的资金退回分账方（**同步返回结果**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401… |
| 43 | `IWechatPayEcommerceProfitSharingService` | `QueryProfitSharingReturnAsync` | GET | `/v3/ecommerce/profitsharing/returnorders` | 商户证书 / APIv3 签名 | 「查询分账回退结果」：核实回退单的最终状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477740。 |
| 44 | `IWechatPayEcommerceProfitSharingService` | `CreateSubsidyAsync` | POST | `/v3/ecommerce/subsidies/create` | 商户证书 / APIv3 签名 | 「发起补差」：平台向二级商户订单补入差额（**与普通服务商身份绑定，一笔订单只能补一次**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/part… |
| 45 | `IWechatPayEcommerceProfitSharingService` | `ReturnSubsidyAsync` | POST | `/v3/ecommerce/subsidies/return` | 商户证书 / APIv3 签名 | 「补差回退」：订单退款时把补差成功的资金退回平台（**同步处理，一笔退款单只允许回退一次**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/par… |
| 46 | `IWechatPayEcommerceProfitSharingService` | `CancelSubsidyAsync` | POST | `/v3/ecommerce/subsidies/cancel` | 商户证书 / APIv3 签名 | 「取消补差」：带补差标识的订单若不需补差，可在分账前取消。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477639。 |
| 47 | `IWechatPayEcommerceRefundService` | `ApplyRefundAsync` | POST | `/v3/ecommerce/refunds/apply` | 商户证书 / APIv3 签名 | 「申请退款 · 二级商户身份」：按 `sub_mchid` 定位收款方发起退款（**受理语义，不代表退款成功**）。<br>官方文档：https://pay.weixin.qq.com/… |
| 48 | `IWechatPayEcommerceRefundService` | `QueryRefundByRefundIdAsync` | GET | `/v3/ecommerce/refunds/id/{refundId}` | 商户证书 / APIv3 签名 | 「查单 · 二级商户身份，按微信退款单号」：读退款终态与资金去向。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476908… |
| 49 | `IWechatPayEcommerceRefundService` | `QueryRefundByOutRefundNoAsync` | GET | `/v3/ecommerce/refunds/out-refund-no/{outRefundNo}` | 商户证书 / APIv3 签名 | 「查单 · 二级商户身份，按商户退款单号」：与 `QueryRefundByRefundIdAsync` 同一应答类型，只是定位键不同。<br>官方文档：https://pay.weix… |
| 50 | `IWechatPayEcommerceRefundService` | `ReturnAdvanceAsync` | POST | `/v3/ecommerce/refunds/{refundId}/return-advance` | 商户证书 / APIv3 签名 | 「发起垫付退款回补」：把电商平台垫付的退款资金从二级商户回补回垫付账户（**方向与退款相反**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/pa… |
| 51 | `IWechatPayEcommerceRefundService` | `QueryReturnAdvanceAsync` | GET | `/v3/ecommerce/refunds/{refundId}/return-advance` | 商户证书 / APIv3 签名 | 「查询垫付退款回补结果」：回补是异步动作，重试结果需查单确认。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476916。 |
| 52 | `IWechatPayEcommerceRefundService` | `ApplyIndividualRefundAsync` | POST | `/v3/ecommerce/refunds/apply` | 商户证书 / APIv3 签名 | 「申请退款 · 商品单个人收款方身份」：按 `individual_auth_id` 定位收款方发起退款。<br>官方文档：https://pay.weixin.qq.com/doc/v… |
| 53 | `IWechatPayEcommerceRefundService` | `QueryIndividualRefundByRefundIdAsync` | GET | `/v3/ecommerce/refunds/id/{refundId}` | 商户证书 / APIv3 签名 | 「查单 · 个人收款方身份，按微信退款单号」。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012650253。 |
| 54 | `IWechatPayEcommerceRefundService` | `QueryIndividualRefundByOutRefundNoAsync` | GET | `/v3/ecommerce/refunds/out-refund-no/{outRefundNo}` | 商户证书 / APIv3 签名 | 「查单 · 个人收款方身份，按商户退款单号」：与 `QueryIndividualRefundByRefundIdAsync` 同一应答类型，只是定位键不同。<br>官方文档：https… |
| 55 | `IWechatPayEcommerceRefundService` | `ApplyAbnormalRefundAsync` | POST | `/v3/ecommerce/refunds/{refundId}/apply-abnormal-refund` | 商户证书 / APIv3 签名 | 「申请异常退款」：对查单/通知确认为 `ABNORMAL`（原路退银行卡失败）的个人收款方退款单，改退至用户银行账户。<br>官方文档：https://pay.weixin.qq.com… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatPayEcommerceAccountService`

**官方文档**（2026-10-15 逐页逐字段核验）：注销并提现申请
https://pay.weixin.qq.com/doc/v3/partner/4013892756、按微信单号查
https://pay.weixin.qq.com/doc/v3/partner/4013892765、按商户单号查
https://pay.weixin.qq.com/doc/v3/partner/4013892759、平台代商户确认
https://pay.weixin.qq.com/doc/v3/partner/4013892772、注销资格前置校验
https://pay.weixin.qq.com/doc/v3/partner/4016420099、注销申请
https://pay.weixin.qq.com/doc/v3/partner/4012476217、注销申请查询
https://pay.weixin.qq.com/doc/v3/partner/4012476223、按 `openid` 查个人收款方授权
https://pay.weixin.qq.com/doc/v3/partner/4012739528、按业务申请单号查签约详情
https://pay.weixin.qq.com/doc/v3/partner/4015851492、主体名下子商户
https://pay.weixin.qq.com/doc/v3/partner/4033742105。

**两支「注销」是两套契约，不是同一件事的两个入口**：
`ApplyCancelWithdrawAsync`（`/account/apply-cancel-withdraw`，【平台商户】）
是「注销**并可提走余额**」，面向**已被微信支付管控且无法解脱**的二级商户（如营业执照注销吊销），
审核通过后把账户**全部余额**打到商户银行卡、多账户会多笔入账；
`ApplyCancelAsync`（`/account/cancel-applications`，【平台商户】）是普通的
「电商子商户注销申请」，走 `application_info` 资料 + 媒体 ID 路线。
两支的单据定位键、状态枚举、应答字段表都不同 ⇒ 各自一型，不得互相套用。

**「先校验、再申请、后确认」是本域的强制时序**：`ValidateCancelAsync` 回
`validate_result` 两值 + `block_reasons[]`（官方原文要求**发起清退前再次调用确认**，
因为返回的余额是实时数、之后仍可能有新交易入账）；申请支的受理应答**没有状态**；
单据进入「待商户确认」后还要 `ConfirmCancelWithdrawAsync` 代商户确认，
而官方明文「审核通过后**超过 30 天**未经商户确认的单据将做撤销处理」⇒ 三个动作缺一不可、
且确认动作有到期日。

**加密面覆盖两支**（逐页核对 Header 参数表，本域其余八页无 `Wechatpay-Serial`）：
https://pay.weixin.qq.com/doc/v3/partner/4013892756 要求 `account_name`/
`account_number`/`identification_name`/`identification_no` 传密文，
https://pay.weixin.qq.com/doc/v3/partner/4033742105 要求 `cert_number` 传密文
（且它是 **Query** 参数，不是包体字段）⇒ 这两支调用必须带 `Wechatpay-Serial`，
DTO 侧只承载密文 string（守卫 EC-C1 锁定「本域加密面恰两支」这一核对结论）。

**本域含全线唯一的「Query 里传敏感信息」**：`cert_number` 是主体证件号码，
官方要求加密后放入 Query ⇒ 密文仍会出现在 URL 上，宿主不得把请求 URI 原文写进日志或异常消息
（支付线的 `WechatPayException.RequestUri` 已剥离 query，自定义埋点须走同一口径）。

**两支个人收款方查询的定位键不同、应答形态也不同**：按 `openid` 支回三字段（须带
**必填** query `permission_type`，官方只给 `INDIVIDUAL_PAYEE` 一个取值）；
按 `out_contract_code` 支回八字段（多 `signed_time`、`individual_openid` 与
`real_name_auth_result` 子树）。**两支的 `appid` 都是必填 query** ⇒ 不得用同一 DTO
「超集」糊过去（共用/分建裁决见两个响应类型的类级注释）。

**应答里的 `access_token` 是实名授权访问令牌**（`EcommerceRealNameAuthResult`）：
它**不可刷新**、过期后须重新走实名授权流程 ⇒ 属凭据，**禁止**进日志、遥测或异常消息，
也不得回传给前端。

**频率上限**：本域十页页内**均无端点级频率数字**（逐页复核）；官方只给出通用 429
`FREQUENCY_LIMITED` 兜底 ⇒ 宿主需自备节流，不能依赖 SDK 限流。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域的平台商户号
（**例外两支**：`ValidateCancelAsync`（`4016420099`）与 `QueryCancelApplicationAsync`
（`4012476223`）官方标【普通服务商】⇒ 本域十页里八页平台商户、两页普通服务商，
这两支须以服务商自有商户配置进入调用作用域）；
本域**零 `[Token]`**。`media_id` 由 `IWechatPayEcommerceCancelMediaService`
手工通道取得（multipart 无法声明式表达），不在本接口内。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

### `IWechatPayEcommerceApplymentService`

**官方文档**（2026-10-15 逐页逐字段核验，九页全标【平台商户】）：普通二级商户进件
https://pay.weixin.qq.com/doc/v3/partner/4012713017、按申请单号查状态
[4012691469](https://pay.weixin.qq.com/doc/v3/partner/4012691469) /
[4015616714](https://pay.weixin.qq.com/doc/v3/partner/4015616714)、按业务申请编号查状态
[4012691376](https://pay.weixin.qq.com/doc/v3/partner/4012691376) /
[4015616708](https://pay.weixin.qq.com/doc/v3/partner/4015616708)、按申请单号撤销
https://pay.weixin.qq.com/doc/v3/partner/4015616720、按业务申请编号撤销
https://pay.weixin.qq.com/doc/v3/partner/4015616721、账户托管进件
https://pay.weixin.qq.com/doc/v3/partner/4015616688、提交代理签约核身信息
https://pay.weixin.qq.com/doc/v3/partner/4015616722。

**路由尾斜杠是官方原文，不是笔误**：进件提交的路由写成 `/v3/ecommerce/applyments/`（带尾 `/`），
与查询/撤销各支都没有尾斜杠不同 ⇒ 照页保留，去掉即与官方契约不一致（守卫 EC-A1）。

**同一路由由两页成文，本域共两处**：`GET /v3/ecommerce/applyments/{applyment_id}` 与
`GET /v3/ecommerce/applyments/out-request-no/{out_request_no}` 各由「普通进件」与「账户托管进件」两页分别文档化，
2026-10-15 四页应答表逐字段比对为**完全一致**（22 行）⇒ 共用一个查询应答类型、共用一个方法，
不按进件模式拆成两套（守卫 EC-A2）。

**本域加密面恰两支**：只有进件提交 https://pay.weixin.qq.com/doc/v3/partner/4012713017
与账户托管进件 https://pay.weixin.qq.com/doc/v3/partner/4015616688 把 `Wechatpay-Serial`
列为参数表行；官方在普通进件页明文标注需加密的字段共 **12** 个：
`id_card_info.id_card_name`、`id_card_info.id_card_number`、`id_doc_info.id_doc_name`、
`id_doc_info.id_doc_number`、`ubo_info_list[].ubo_id_doc_name`、`ubo_info_list[].ubo_id_doc_number`、
`ubo_info_list[].ubo_id_doc_address`、`account_info.account_name`、`account_info.account_number`、
`contact_info.contact_name`、`contact_info.contact_id_card_number`、`contact_info.mobile_phone`
⇒ 查询/撤销/核身四支不得支配该头（守卫 EC-A3）。官方推荐**微信支付公钥**加密（须附公钥 ID），
也可用平台证书公钥（须先调接口取证书并解密）；加密属宿主证书资产，SDK 不代为加解密，
上述字段**绝不写入日志、遥测或异常消息**。

**「提交 ≠ 完成」：本域的主线是状态机轮询**：官方原文「当服务商提交申请单后，需要定期调用此接口查询
申请单的审核状态」⇒ 进件受理后须按 `EcommerceApplymentStates` 轮询到 `FINISH`（或 `REJECTED` /
`FROZEN` / `CANCELED`）才算收敛。八个状态各有伴随字段：`sign_url` 只在 `NEED_SIGN` 返回、
`sub_mchid` 只在 `NEED_SIGN` 或 `FINISH` 返回、`account_validation` 只在
`ACCOUNT_NEED_VERIFY` 返回、`audit_detail` 只在 `REJECTED` 返回 ⇒
**判状态须用枚举比对，不能用「某字段是否为 null」倒推**。

**应答侧同样有密文**：`account_validation.account_name`（付款户名）与
`account_validation.account_no`（付款卡号）官方明确要求「需进行解密处理」（参见
https://pay.weixin.qq.com/doc/v3/partner/4013059060）⇒ SDK 原样透传密文字符串、不解密。

**「被驳回时填相同业务申请编号即覆盖修改原申请单」**（官方原文，两支提交皆然）：
这是本域唯一的覆盖写语义 ⇒ `out_request_no` 是幂等键而不是唯一性键，重提时**不要**换新编号
（换新编号会造出并行申请单）。`out_request_no` 官方限 **124** 字符（撤销页标注），
与查询页的路径参数同一口径。

**撤销有六状态白名单**：官方原文「仅当申请单状态为资料校验中、待账户验证、审核中、已驳回、待签约、
已冻结时才允许撤销」⇒ `FINISH`（已完成，商户号已下发）与 `CANCELED`（已作废）不可撤销，
对这两个状态调用必失败；两支撤销**官方零 body 参数**（定位键全在 path）⇒ 方法不带请求体（守卫 EC-A4）。

**两支撤销同应答、与两支提交同型**：撤销应答为 `{applyment_id, out_request_no}` 两行，
与普通进件/账户托管提交的应答逐字段相同（仅 `out_request_no` 的长度标注写法不同）⇒
三支共用 `EcommerceApplymentSubmitResponse`；而代理核身支的应答**只有 `applyment_id` 一行**
（2026-10-15 比对为 DIFF）⇒ 独立类型 `EcommerceSignVerifyInfoResponse`，不做超集。

**两支提交的字段必填性分叉，故分建两个请求类型**：账户托管页把 `business_license_info` 整族降为选填、
把 `id_card_info.id_card_name`/`id_card_number` 与 `id_doc_info` 三支姓名/号码升为必填，
并多出 `id_card_complete_statement` 与 **必填的 `proxy_sign_verify_mode`**；
普通进件页无这两字段 ⇒ 共用一个请求类型即造出「一类型两套必填语义」。

**代理核身支只在托管模式下存在**：官方原文「在申请单中声明『走平台代理签约和代理核身入驻模式』时，
可使用该接口传入代理签约和代理核身的信息」⇒ `pre_sign_time` 与 `proxy_verify_identity_info`
的条件必填由进件时 `proxy_sign_verify_mode=true` 决定；本支应答不含任何状态字段，
核身是否被采纳须回到查询支读 `applyment_state`。

**能力边界**：官方明文「本接口不支持进件境外个人小微商户」（两支提交同文）⇒
该组合由官方判错，SDK 不加本地闸门。**各页均未给出频率数字**，只列通用错误码表
（400 `PARAM_ERROR` / 401 `SIGN_ERROR` / 500 `SYSTEM_ERROR`）⇒ SDK 不做限流与自动重试。

**媒体前置**：证照图片类字段（`*_copy`、`finance_license_pics`、`business_addition_pics` 等）
官方要求填**媒体库图片 ID** ⇒ 须先经
`UploadMediaAsync` 或
`UploadCancelProofImageAsync`
上传换取 ID，不能直传二进制。

**与特约商户进件域不共用**：`IWechatPayApply4SubMerchantService` 走
`/v3/applyment4sub/*`、`IWechatPayMerchantAlterApplyService` 走 `/v3/merchant-test/alterapply/*`，
三者的状态枚举、字段名与加密面各自成文 ⇒ 本域类型一律 `Ecommerce` 前缀，不复用他域 DTO。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域商户号；
本域**零 `[Token]`**。`[AllowAnyStatusCode]` 使 APIv3 的 4xx/5xx 错误体原样落到
`Code`，由 `WechatPayException.ThrowIfFailed` 判错。

### `IWechatPayEcommerceBillService`

**官方文档**（2026-10-15 逐字段核验）：
https://pay.weixin.qq.com/doc/v3/partner/4012760697，「支持商户」【平台商户】。

**与直连资金账单是两套应答形态，不是「同接口加了个 sub_mchid」**：直连
`GetFundFlowBillAsync` 回**单文件**三字段
（`hash_type`/`hash_value`/`download_url`），本支回**文件分片列表**
（`download_bill_count` + `download_bill_list[]`，见 `EcommerceFundFlowBillResponse`）
⇒ 必须按 `BillSequence`（**从 1 起**）逐片下载、逐片校验解密后合并。
把直连「下载一个文件即账单」的处理逻辑照搬过来会**静默丢数据**。

**请求侧也有两处与直连相反**：① `account_type` 官方明文「本接口只支持填 `ALL`」
（直连是按账户类型分列下载），且本支**必填**；② `algorithm` 在本支是**必填**
（直连无此要求），取值见 `EcommerceBillAlgorithms`（`AEAD_AES_256_GCM` / `SM4_GCM`）。
`hash_value` **不是本支的请求参数**（摘要值出现在每个分片的应答里）。

**权限可见范围**：调用前须先联系微信支付运营申请权限入口，通过后在【服务商平台 → 产品中心】
自助开通「下载二级商户资金流水账单」产品权限 ⇒ 未开通时本支必然失败，属配置缺失而非参数错误。

**时效窗口很窄**：① 只支持**三个月内**的账单，且官方注明二级商户资金流水账单**仅能通过本接口下载**
⇒ 过期即永久丢失，须自行留存；② 当日账单**次日上午 9 点**才开始生成，官方建议 10 点后获取；
③ 每个 `download_url` **5 分钟内有效** ⇒ 拿到地址必须立刻下载。

**账单金额单位是「元」**（官方原文），与本线 API 金额的「分」口径**相反** ⇒
不要把账单数值与接口数值直接相加对账。

**`tar_type` 选填**：不填即返回**未压缩**数据流，SDK 不代为补默认值（补 `GZIP`
会让宿主按 gzip 解压而失败）。`encrypt_key` 是**用商户证书公钥加密后 Base64** 的对称密钥，
须以自身 API 客户端私钥解出，与 `nonce` 配套解密账单。

**本支只给下载地址**：真实的文件下载走 `GET {download_url}`（动态路由 + 非 JSON 二进制），
由 `IWechatPayBillDownloadService` 承载，与直连账单共用同一下载通道
⇒ 本域不重复声明下载端点。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域的平台商户号；
本域**零 `[Token]`**、**无密文入参**（Header 参数表只有 `Authorization` 与 `Accept`）。

### `IWechatPayEcommerceFundService`

**官方文档**（2026-10-15 逐页逐字段核验）：实时余额
https://pay.weixin.qq.com/doc/v3/partner/4012476690、日终余额
https://pay.weixin.qq.com/doc/v3/partner/4012476693、预约提现
https://pay.weixin.qq.com/doc/v3/partner/4012476652、按 `withdraw_id` 查
https://pay.weixin.qq.com/doc/v3/partner/4012476665、按 `out_request_no` 查
https://pay.weixin.qq.com/doc/v3/partner/4012476656。五页「支持商户」一律是【平台商户】。

**本域的身份是「平台商户（电商平台自身）」，不是服务商**：签名主体是电商平台的 `sp_mchid`，
二级商户只以 `sub_mchid` 出现在 path/body ⇒ 与
`IWechatPayMerchantFundService`（【普通服务商】面、`account_type` 直接进 path 的两支余额）
是**同族不同身份的两套契约**，路由前缀（`/v3/ecommerce/fund` 对 `/v3/merchant/fund`）与
应答字段表都不同，不得互相套用 DTO。

**两支余额共用一个应答类型、两支查询共用一个应答类型**（逐字段比对结论见
`EcommerceBalanceResponse` 与 `EcommerceWithdrawQueryResponse` 的类级注释）；
差异全在请求侧：日终支多一个**必填** query `date`，两支查询的定位键一个在 path、`sub_mchid`
恒定是**必填** query。

**「受理 ≠ 到账」是本域最核心的陷阱**：预约提现的成功应答只有三字段、**没有状态** ⇒
拿到 `EcommerceWithdrawApplyResponse` 不能记账，必须落库 `withdraw_id`
（或 `out_request_no`）后经查询支读 `status`（六值，见 `EcommerceWithdrawStatuses`）。
官方另注明**「`SUCCESS` 不等于银行入账成功」**——部分情况会在约 7 个工作日内退票成 `REFUND`
⇒ 资金结论只能取查询支的终态，且 `SUCCESS` 之后仍有一段会翻转的窗口。

**「不得换单重试」是官方明文的资金安全约束**：`SYSTEM_ERROR`、回包解析报错、新增未知错误码
三种情形官方一律要求**用原单（所有请求参数保持不变）重试**；而「相同 `out_request_no` + `sub_mchid`」
只提供 **6 天**防重 ⇒ 换单等于对同一笔提现重复出款，超过 6 天换单更没有任何防重保护。

**频率上限**：预约提现 **40 次/秒**（平台服务商维度），两支查询各 **100 次/秒**；
两支余额页页内无端点级频率数字。超限回 429 `FREQUENCY_LIMITED`。

**业务限制**：单笔预约提现金额上限「不能超过 8 亿」（单位分 ⇒ 以 `long` 承载）；
`out_request_no` 限 32 且**仅字母数字**；查询支只可查 **90 日内**发起的提现单；
日终余额只可查 **90 天内**、且当日数据**次日 10:00 后**才生成；提现发起后**次日**才实际出款。

**零 `[Token]`、零加密字段**：五页的 Header 参数表都只有 `Authorization` 与
`Accept`，未把 `Wechatpay-Serial` 列为参数 ⇒ 本域无需密文（守卫 EC-F5 锁定该核对结论）。
`account_number` 在查询应答里**只有后四位**（string(4)），仅够展示与人工核对，不是账号指纹，
也不得据此写日志之外的资金判断。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`，备域名 `api2` 需宿主自行处理）；商户作用域经
`IWechatPayMerchantContext.UseMerchant(...)` 选择，签名主体即当前作用域的平台商户号。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

### `IWechatPayEcommerceMchTransferService`

**官方文档**（2026-10-15 逐页逐字段核验）：申请授权
https://pay.weixin.qq.com/doc/v3/partner/4013504208、查询授权状态
https://pay.weixin.qq.com/doc/v3/partner/4013504209、发起转账
https://pay.weixin.qq.com/doc/v3/partner/4013504211、按商户单号查单
https://pay.weixin.qq.com/doc/v3/partner/4013504213、撤销转账
https://pay.weixin.qq.com/doc/v3/partner/4013504212、按微信单号查单
https://pay.weixin.qq.com/doc/v3/partner/4013504214。

**授权是转账的前置闸门，且申请成功不等于已授权**：官方在授权状态里给了中间态
`AuthorizeProcessing`（「授权申请正在处理中，请稍后查询授权结果」）
⇒ 必须先 `ApplyTransferAuthorizationAsync` 再 `QueryTransferAuthorizationAsync` 轮询到
`AUTHORIZED` 才能发起转账。**本域的授权状态是三值 `authorization_state`，与个人收款方签约面的
两值 `auth_state`（见 `EcommerceAuthStates`）字段名与值集都不得互换**（守卫 EC-M1）。

**两支查单同应答、与受理应答不同**：`4013504213` 与 `4013504214` 应答表 14 行**逐行相同**
⇒ 共用 `EcommerceMchTransferBillQueryResponse`；而发起转账的受理应答（7 行）比查单少
`update_time` 却多出 `package_info`，撤销应答（5 行）只回 `state` 两值 ⇒ 三支三型，不做超集。

**`package_info` 只出现在受理应答**：它是「拉起微信收款确认页面」所需的参数，官方在两支查单应答里
**不返回**该字段 ⇒ 错过受理应答就再也取不到，须自行持久化（守卫 EC-M2）。

**转账是「用户确认收款」型资金动作，状态机有八个值**：见 `EcommerceMchTransferBillStates`
（`ACCEPTED` → `PROCESSING` / `WAIT_USER_CONFIRM` → `TRANSFERING` → `SUCCESS` / `FAIL`，
撤销走 `CANCELING` → `CANCELLED`）。官方对 `PROCESSING` 的原文提示是
「如一直处于此状态，建议检查账户余额是否足够」⇒ 长时间 `PROCESSING` 是资金问题而不是网络问题。

**撤销受理 ≠ 撤销完成**：官方原文「该接口返回成功仅表示撤销请求已受理，系统会异步处理退款等操作，
以最终查询单据返回状态为准」，且「在**用户确认收款之前**可以通过该接口撤销付款」⇒ 用户已确认后撤销必失败；
撤销应答的 `state` 只有 `CANCELING`/`CANCELLED` 两值，须回查 `QueryMchTransferBillByOutBillNoAsync` 收敛。

**本域加密面恰一支**：只有 https://pay.weixin.qq.com/doc/v3/partner/4013504211 把
`Wechatpay-Serial` 列为参数表行，密文入参是 `user_name`（string(1024)，标准 RSA 与国密均支持，
公钥由微信侧提供）⇒ 其余五支不得支配该头（守卫 EC-M3）。该字段是收款人真实姓名，
**绝不写入日志、遥测或异常消息**；加密属宿主证书资产，SDK 不代为加密。

**条件必填的阈值以元表述、金额以分传递**：官方原文「转账金额 >= 2,000 元时，该笔明细必须填写
`user_name`」⇒ 判断须换算为 `200000` 分；传入后微信支付校验收款用户与姓名是否一致并提供电子回单。
SDK 不加本地闸门（阈值可能随官方调整，硬编码即造出误拒）。

**`out_bill_no` 字符集比本域退款单号严格**：官方要求「只能由数字、大小写字母组成」，
不含 `- _ | * @`；同域 `out_refund_no` 却允许这些符号 ⇒ 复用同一套单号生成器会在此报错。

**`appid` 必须是 `openid` 所属的那个 AppID**（官方：「请填写收款用户 OpenID 对应的 AppID」）
⇒ AppID 与 OpenID 不匹配时官方直接校验失败；`transfer_scene_id` 须在「平台商家转账 - 产品设置」中已获取，
`notify_url` 须公网可访问、**必须 HTTPS** 且不能携带参数。

**各页未给出频率数字**：本域六页只列通用错误码表（含 500 `SYSTEM_ERROR`「系统异常，请稍后重试」）
⇒ SDK 侧不做限流与自动重试，重入策略由宿主按单号幂等保证。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域商户号；
本域**零 `[Token]`**。`[AllowAnyStatusCode]` 使 APIv3 的 4xx/5xx 错误体原样落到
`Code`，由 `WechatPayException.ThrowIfFailed` 判错。

### `IWechatPayEcommercePapayService`

**官方文档**（2026-10-15 逐页逐字段核验，六页全标【平台商户】）：预签约
https://pay.weixin.qq.com/doc/v3/partner/4012881608、查询签约信息
https://pay.weixin.qq.com/doc/v3/partner/4012884060、解约
https://pay.weixin.qq.com/doc/v3/partner/4012884072、合单下单（免密扣款）
https://pay.weixin.qq.com/doc/v3/partner/4012884121、合单查询
https://pay.weixin.qq.com/doc/v3/partner/4012884130、合单撤销
https://pay.weixin.qq.com/doc/v3/partner/4012884126。

**设计文档表格给的是旧版 wiki 链接**（`/wiki/doc/apiv3_partner/Offline/apis/chapter5_5_*.shtml`）：
本轮按现行文档页 `4012881608` 等六页逐字段核验，字段表与旧版一致但 URL 以现行页为准。

**三支产品能力前置，缺一不可**：官方逐页给出前置条件 ⇒ ① 预签约「使用前请确认已开通委托代扣能力」；
② 合单扣款「调用前请确认电商服务商已经开通**合单**委托代扣能力并已经和用户签约，**且签约的模板有合单委托代扣能力**」
—— 能力开通是「服务商级 + 模板级」两级，模板没有合单能力时即使服务商已开通也会失败；
③ 解约「调用前请确认对应的签约协议是**已生效状态**」⇒ 对未生效或已解约的协议解约必失败（守卫 EC-S1）。

**签约会话只有 2 小时**：官方原文「商户获取的签约会话有效期为 2 小时」，
`pre_entrustweb_id` 的描述同文「两个小时内有效」⇒ 取到后必须在窗口内拉起微信客户端，
过期即需重新预签约（重新预签约会换会话 ID，不复用旧值）。

**签约结果走异步通知且「放弃签约不通知」**：官方原文「用户在微信的页面中完成委托代扣签约后，
微信会同时将签约信息通过异步通知的方式通知给商户后台。**如果用户放弃签约或签约失败则不通知**」⇒
没有通知不等于失败，只有 `QueryPapayContractAsync` 的 `contract_state` 是权威；
因此回调后必须回查，而不是把「未收到通知」当状态使用。

**预签约应答的两个条件字段决定调起方式**：`miniprogram_username` / `miniprogram_path`
只在「模板为 2025-09-23 及之后申请」或「2025-09-23 之前但申请了 `WXLaunchMiniProgram` 权限」时返回 ⇒
返回则用 LaunchMiniProgram SDK 调起签约（字段名与值**原样填入**），未返回则该模板只能通过
OpenBusinessWebview SDK 调起 —— 官方以「是否返回该字段」判别可用调起通道，SDK 不代为选择。

**扣款是同步模式且窗口只有 1 分钟**：官方原文「当前扣费接口为**同步扣费模式**，
商户超时时间为 **1 分钟**，超过 1 分钟未能返回结果时，商户可主动发起撤销」⇒
这是本域唯一由「商户侧计时」决定是否撤销的端点：1 分钟内无结果即应走 `ReversePapayCombineTransactionAsync`，
不要盲目重下（合单扣款官方未声明幂等，重复下单以 `combine_out_trade_no` 定位的既有单为准）。

**撤销支官方零应答字段**：https://pay.weixin.qq.com/doc/v3/partner/4012884126 的应答节
只有 `200 OK`、**参数表为空**（2026-10-15 核验：应答 0 行）⇒ 本方法返回裸 `Task`，
撤销结果必须回 `QueryPapayCombineTransactionAsync` 读子单状态，不得伪造应答类型（守卫 EC-S2）。

**扣款与查询两支共用同一应答类型**：`4012884121` 与 `4012884130` 的应答表 21 行
**逐行相同**（2026-10-15 比对为 IDENTICAL）⇒ 共用 `EcommercePapayCombineTransactionResponse`；
两支的**请求**形态则完全不同（下单整棵合单树、查询只有 path 上的 `combine_out_trade_no`）。

**本域应答的交易类型只有一个值**：`sub_orders[].trade_type` 官方枚举仅 `PAP`（委托代扣，
见 `EcommercePapayTradeTypes`），与直连合单的 `JSAPI`/`NATIVE`/`APP` 形态不同；
子单状态族见 `EcommercePapayTradeStates`（六个值，无 `REVOKED`）⇒
撤销后的收敛态须以查询应答的实际值判读，不得臆造第七个值。

**签约定位键是「模板 + 商户签约协议号」二元组**：查询与解约两支都走
`/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}`，`plan_id` 官方类型为
**integer**（本方法用 `long` 承载）而 `out_contract_code` 是 string(32) 且
「商户侧需保证唯一性」⇒ 与 `EcommercePapayPreEntrustSignRequest` 里的同一字段是同一个键，
签约、查签、解约三支共用；扣款用的是 `contract_id`（微信侧签约协议号），两者不可互换。

**本域零加密面**：六页 Header 参数表只有 `Authorization` / `Accept` / `Content-Type`，
**没有任何一页把 `Wechatpay-Serial` 列为参数**（2026-10-15 逐页核验）⇒ 本接口六支一律不得支配该头
（守卫 EC-S3）。签约链上的 `openid` 在官方口径里是明文标识符，不是敏感字段；
但 `contract_display_account`（协议展示账号）会出现在用户可见的签约页面，属业务文案而非凭据。

**各页均未给出频率数字**：只列通用错误码表 ⇒ SDK 不做限流与自动重试；撤销与扣款的时序由宿主按
1 分钟窗口掌握。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域商户号；
本域**零 `[Token]`**。`[AllowAnyStatusCode]` 使 APIv3 的 4xx/5xx 错误体原样落到
`Code`，由 `WechatPayException.ThrowIfFailed` 判错。

### `IWechatPayEcommerceProfitSharingService`

**官方文档**（2026-10-15 逐页逐字段核验）：请求分账
https://pay.weixin.qq.com/doc/v3/partner/4012691594、查询分账结果
https://pay.weixin.qq.com/doc/v3/partner/4012477734、查询剩余待分金额
https://pay.weixin.qq.com/doc/v3/partner/4012477751、分账完结
https://pay.weixin.qq.com/doc/v3/partner/4012477745、添加接收方
https://pay.weixin.qq.com/doc/v3/partner/4012477758、删除接收方
https://pay.weixin.qq.com/doc/v3/partner/4012477759、请求分账回退
https://pay.weixin.qq.com/doc/v3/partner/4012477737、查询回退结果
https://pay.weixin.qq.com/doc/v3/partner/4012477740、发起补差
https://pay.weixin.qq.com/doc/v3/partner/4012477631、补差回退
https://pay.weixin.qq.com/doc/v3/partner/4012477636、取消补差
https://pay.weixin.qq.com/doc/v3/partner/4012477639。

**本域身份是分裂的，不是笔误**：分账八页标【平台商户】，而**补差三页标【普通服务商】**。
补差能力由服务商侧开通、资金动作仍发生在收付通订单上 ⇒ 同一个 `/v3/ecommerce/subsidies/*` 前缀下
的签名主体口径与分账不同。守卫 EC-P7 把「八支平台商户 + 三支普通服务商」钉成事实，防止后续按同域一刀切。

**与直连分账不共用 DTO**：`IWechatPayProfitSharingService` 走 `/v3/profitsharing/*`
且身份字段是 `receiver_account`+`mchid` 单层；本域全部报文带 `sub_mchid`、请求分账的接收方对象是
`receivers[].receiver_account`，且多出 `finish` 与 `description` 口径 ⇒ 两侧路由、字段、状态枚举都不同。

**「受理 ≠ 结果」是本域的主线**：请求分账与分账完结都是**异步处理**（官方原文「先受理请求再异步处理」），
`EcommerceProfitSharingFinishResponse` 官方只回四行、**没有结果状态** ⇒ 完结成功与否只能回到
`QueryProfitSharingOrderAsync` 读 `state`（见 `EcommerceSplitOrderStatuses`）；
而分账回退与补差三支是**同步处理**、实时返回结果。同一域内两种处理模式并存，按支区分、不能一刀切。

**180 天是本域三条独立时限**，常被混为一谈：① 分账资金冻结期默认 180 天，自订单支付成功起
未发起分账则待分资金**自动解冻给分账方**；② 分账回退的时限是 180 天；③ 补差重入有效期 180 天。
各自起算点不同 ⇒ 过期后没有任何接口可以补救。

**次数上限**：同一笔订单最多发起 **50** 次分账请求、每次最多 **50** 个接收方；
同一笔分账单最多 **50** 次回退；一笔订单**只能补差一笔**、一笔微信退款单**只允许补差回退一次**；
分账接收方数量上限 **2 万**（平台维度统一维护，非二级商户维度）。

**频率上限**（官方逐页给出，是本线少见的多级限频）：请求分账「单个平台商户 **2000 QPS**」
且「单个交易收款商户 **300 QPS**」；分账完结「单个分账接收方商户 **60 QPS**」；
其余各页只列通用 429 `FREQUENCY_LIMITED`。超限即 429，官方建议拆分商户号避免集中。

**幂等与「不得换单」**：请求分账官方明文「支持幂等重入」；补差与补差回退遇到 `SYSTEM_ERROR`
**必须用原参数、原单号重入**（官方原文「如更换金额重试，可能会导致重复扣款，系统会在 1 天后回退到原账户」）
⇒ SDK 不做任何自动重试与单号改写。

**加密面只覆盖请求分账一支**：只有 https://pay.weixin.qq.com/doc/v3/partner/4012691594
把 `Wechatpay-Serial` 列为参数表行，密文字段是 `receivers[].receiver_name`（个人接收方姓名）⇒
本接口其余十支不得支配该头（守卫 EC-P6）。分账接收方为个人时官方还要求「电商平台须确保已向微信支付
传输用户身份信息做一致性校验并合法征得用户授权」⇒ 授权合规责任在宿主，SDK 不代替判断。

**回退资格是硬条件**：仅支持「分账给 `MERCHANT_ID` 商户号类型」且分账单
`state=FINISHED`、`result=SUCCESS` 的分账单；**不支持对「分账给个人」的分账单发起回退**；
商户类型接收方已自动开通回退能力。回退与退款无强耦合（余额充足时可直接退款）⇒ 是否回退由平台决定。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域商户号；
本域**零 `[Token]`**。`[AllowAnyStatusCode]` 使 APIv3 的 4xx/5xx 错误体原样落到
`Code`，由 `WechatPayException.ThrowIfFailed` 判错。

### `IWechatPayEcommerceRefundService`

**官方文档**（2026-10-15 逐页逐字段核验）：申请退款（二级商户）
https://pay.weixin.qq.com/doc/v3/partner/4012476892、按 `refund_id` 查单
https://pay.weixin.qq.com/doc/v3/partner/4012476908、按 `out_refund_no` 查单
https://pay.weixin.qq.com/doc/v3/partner/4012476911、查询垫付回补
https://pay.weixin.qq.com/doc/v3/partner/4012476916、发起垫付回补
https://pay.weixin.qq.com/doc/v3/partner/4012476927、申请退款（个人收款方）
https://pay.weixin.qq.com/doc/v3/partner/4012649457、个人收款方按 `refund_id` 查单
https://pay.weixin.qq.com/doc/v3/partner/4012650253、个人收款方按 `out_refund_no` 查单
https://pay.weixin.qq.com/doc/v3/partner/4012650314、异常退款
https://pay.weixin.qq.com/doc/v3/partner/4012649274。

**同一路由有两份契约，本域共三处**：`POST /v3/ecommerce/refunds/apply`、
`GET /v3/ecommerce/refunds/id/{refund_id}`、`GET /v3/ecommerce/refunds/out-refund-no/{out_refund_no}`
三支各由「二级商户身份（按 `sub_mchid` 定位）」与「商品单个人收款方身份（按 `individual_auth_id` 定位）」
两页分别成文。两族的**请求表与应答表均不重合**（二级商户查单 26 行 vs 个人收款方 21 行，差集恰是
`amount.from` 子表、`amount.advance`、`refund_account` 五项）⇒ 本接口按身份分建方法、
SDK 不提供「一个方法两种语义」的合并入口（守卫 EC-R1）。

**「受理 ≠ 结果」是本域的主线**：官方原文「申请退款接口的返回仅代表业务的受理情况，具体退款是否成功，
需要通过退款查询接口获取结果」，且两支受理应答**都没有 `status` 字段** ⇒ 拿到
`EcommerceRefundApplyResponse` 只表示退款单已创建，终态必须走 `QueryRefundByRefundIdAsync`
一类查单方法读 `EcommerceRefundStatuses`。

**365 天与 50 次是本域两条硬时限**：交易时间超过一年（按支付成功时间 `+365` 天）无法提交退款；
每个支付订单的部分退款次数不超过 **50** 次。两者超限后没有任何接口可补救。

**「一笔退款失败后重新提交，请不要更换退款单号，请使用原商户退款单号」**（官方原文，两支同文）⇒
SDK 不做任何单号改写或自动重试；同一 `out_refund_no` 多次请求官方只退一笔。

**频率上限**（官方逐页给出）：申请退款两支均「**150 QPS**，且**单笔订单 1 QPM**」
—— 单笔订单每分钟只能申请一次是本线少见的单粒度限频，批量重试脚本必须先按订单排队；
异常退款支 150 QPS；查单与回补各页只列通用 429 `FREQUENCY_LIMITED`。

**`amount.from` 与 `funds_account` 互斥**（官方原文「该功能优先级高于……且二者不可同时使用」）：
出资账户指定多账户退款时须满足「基本账户可用 + 不可用出资金额之和 = 退款金额」「账户类型不得重复」
「订单属于分账订单且分账处于待分账或分账中」等条件 ⇒ 条件校验交给官方判定，SDK 侧不加本地闸门。

**垫付退款是需先开通的能力**：官方原文「垫付退款需要向微信支付申请开通权限，开通权限时需要指定一个
垫付出款账户」，且「请联系微信支付客服」。回补方向与退款**相反**（资金回到垫付方）⇒
`EcommerceReturnAdvanceResponse` 的 `payer_*` 是出款方、`payee_*` 是入账方，
两对字段名是本域最易读反的地方。

**回补的 90 天重试窗口不可跳过**：官方原文「垫付回补结果为处理中，微信支付会在 90 天内定期重试，
重试结果需要查单确认」；若 90 天后出款方账户仍余额不足，回补结果置为**回补失败**且
「系统不再重试，商户也无法重试，后续不能再发起此退款单的回补」⇒ 处理期间须保证出款账户余额充足。

**`ABNORMAL` 的处置路径按身份分叉**：二级商户支的异常单官方指引是「登录商户平台交易中心人工处理」，
而个人收款方支才有机器可调的 `ApplyAbnormalRefundAsync`（异常退款 API）⇒ 对个人收款方之外的单据
调本方法属误用。异常退款官方限定**只支持退款至用户银行账户**（`type` 唯一取值
`EcommerceAbnormalRefundTypes`.`USER_BANK_CARD`），且仅支持招行、交通、农行、建行、工商、
中行、平安、浦发、中信、光大、民生、兴业、广发、邮储、宁波银行这 15 家银行的**借记卡**。

**本域加密面恰一支**：只有 https://pay.weixin.qq.com/doc/v3/partner/4012649274 把
`Wechatpay-Serial` 列为参数表行，密文入参是 `bank_account` 与 `real_name`（均 string(1024)）⇒
其余八支不得支配该头（守卫 EC-R6）。两个字段属收款人银行账号与真实姓名，
**绝不写入日志、遥测或异常消息**。

**与直连退款域不共用契约**：`IWechatPayRefundService` 走 `/v3/refund/domestic/refunds/*`
且按 `sub_mchid` 定位；该域已承载**普通服务商**面的异常退款
`POST /v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund`
（https://pay.weixin.qq.com/doc/v3/partner/4013080627）⇒ 与本域 `ApplyAbnormalRefundAsync`
是同名不同路由的两支，不可互相替代。

**退款到账时延**：官方原文「用零钱支付的退款 20 分钟内到账，银行卡支付的退款 3 个工作日后重新查询退款状态」
⇒ 查单节奏应按支付方式区分，银行卡单过早轮询只会读到 `PROCESSING`。

**客户端与凭据**：走支付专用命名客户端 `wechat-pay`，签名主体是当前作用域商户号；
本域**零 `[Token]`**。`[AllowAnyStatusCode]` 使 APIv3 的 4xx/5xx 错误体原样落到
`Code`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `ApplyCancelWithdrawAsync` — 「商户号注销并提现申请」：代被管控的二级商户发起注销、并把账户余额打款到商户银行卡。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013892756。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`POST /v3/ecommerce/account/apply-cancel-withdraw`

- **请求体**：`EcommerceCancelWithdrawApplyRequest` — 注销并提现请求，见 `EcommerceCancelWithdrawApplyRequest`。
- **返回**：`EcommerceCancelWithdrawApplyResponse` — 只回 `applyment_id` / `out_request_no`，**没有状态**，见 `EcommerceCancelWithdrawApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/account/apply-cancel-withdraw`；页内无频率数字。
- **必须带 `Wechatpay-Serial`**：收款户名/账号与经办人姓名/证件号四个字段是密文， / 明文直传既被拒又违规。
- **适用面很窄**：官方限定「仅针对被微信支付管控、且无法解脱该管控的商户」⇒ / 正常退出应走 `ApplyCancelAsync`，不要把本支当通用提现出口。
- **受理 ≠ 生效**：必须落库单号后经查询支读 `cancel_state`；审核通过后**超过 30 天** / 未经商户确认会被撤销。资金清退是「账户全部遗留资金」⇒ 调用前应先用 `ValidateCancelAsync` 复核。

### 2. `QueryCancelWithdrawByApplymentIdAsync` — 「按微信支付注销单号查询注销提现申请」：读审批与出款进度。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013892765。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/account/apply-cancel-withdraw/applyment-id/{applymentId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applymentId` | Path | `string` | 微信支付注销单号（官方 path `applyment_id`，必填 string(32)）：申请受理时返回。 |
- **返回**：`EcommerceCancelWithdrawQueryResponse` — 注销提现单详情，见 `EcommerceCancelWithdrawQueryResponse`（与按商户单号查**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/account/apply-cancel-withdraw/applyment-id/{applyment_id}`； / 无 query、无包体。
- **两支查询应答 18 行逐字段相同** ⇒ 共用一型；差异只在定位键（微信单号对商户单号）。
- **`WAITING_MERCHANT_CONFIRM` 态下本支是唯一能看见待确认事实的地方**（应答的 / `confirm_cancel` 子树）⇒ 确认后仍以查询支的终态为准，`ConfirmCancelWithdrawAsync` 的成功应答不代表注销完成。

### 3. `QueryCancelWithdrawByOutRequestNoAsync` — 「按商户注销申请单号查询注销提现申请」：与上一支**同一应答类型**，定位键换成商户自有单号。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013892759。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/account/apply-cancel-withdraw/out-request-no/{outRequestNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRequestNo` | Path | `string` | 商户注销申请单号（官方 path `out_request_no`，必填 string(32)，仅字母数字）。 |
- **返回**：`EcommerceCancelWithdrawQueryResponse` — 与按 `applyment_id` 查**同一应答类型**，见 `EcommerceCancelWithdrawQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/account/apply-cancel-withdraw/out-request-no/{out_request_no}`。
- **本支是「申请应答丢失」时的唯一自救入口**：只要宿主存了自己的 `out_request_no`， / 就不必依赖 `applyment_id` 落库成功。

### 4. `ConfirmCancelWithdrawAsync` — 「平台代商户确认注销」：单据处于「待商户确认注销」时代商户执行确认动作。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013892772。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`POST /v3/ecommerce/account/apply-cancel-withdraw/confirm`

- **请求体**：`EcommerceCancelWithdrawConfirmRequest` — 确认请求，见 `EcommerceCancelWithdrawConfirmRequest`。
- **返回**：`EcommerceCancelWithdrawConfirmResponse` — 确认结果，见 `EcommerceCancelWithdrawConfirmResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/account/apply-cancel-withdraw/confirm`；页内无频率数字。
- **`applyment_id` 与 `out_request_no` 官方标「二选一」**（两者都列选填）⇒ / SDK 不做条件必填校验，一个都不传由官方判 `PARAM_ERROR`。
- **`operator_identification` 是留证字段，不是鉴权字段**：官方原文「仅做留存， / 未来出现商户纠纷时依据此标识调证」⇒ 必须传真实操作员标识（≤100，允许大小写字母、数字与汉字）， / 填占位值等于把纠纷举证责任丢掉。`confirm_time` 与 `cancel_contract_version` 同为必填， / 版本号须与商户实际勾选的《微信支付注销申请书》一致。

### 5. `ValidateCancelAsync` — 「注销资格前置校验」：发起注销提现前判断商户是否满足注销条件，并带回各账户实时余额与阻塞原因。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016420099。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{subMchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Path | `string` | 二级商户号（官方 path `sub_mchid`，必填 string(32)）。 |
- **返回**：`EcommerceValidateCancelResponse` — 商户号状态 + 资格结论 + 余额清单 + 不可注销原因，见 `EcommerceValidateCancelResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{sub_mchid}`； / 「支持商户」【普通服务商】（本域两支非【平台商户】页之一，另一支见 `QueryCancelApplicationAsync`； / 两页路由仍是 `/v3/ecommerce/` 前缀）。
- **余额是实时数，不是锁定数**：官方明文「查询后商户可能仍有新的交易入账导致余额变动， / 发起资金清退申请前请再次调用本接口进行确认」⇒ 本支只用于**引导与决策**， / 提交 `ApplyCancelWithdrawAsync` 前须现查现用，不能拿历史值做资金承诺。
- **`out_account_type` 是另一套四值枚举**（`BASIC_ACCOUNT`/`OPERATE_ACCOUNT`/ / `MARGIN_ACCOUNT`/`TRADE_FEE_ACCOUNT`，见 `EcommerceOutAccountTypes`）⇒ / 与提现支的 `BASIC`/`FEES`/`OPERATION` 三值**不通用**，跨字段照抄必错。
- **`block_reasons` 可能多条**（同个商户多个不可注销原因）⇒ 只取第一条会误导商户。

### 6. `ApplyCancelAsync` — 「电商子商户注销申请」：提交普通注销申请（资料以 `application_media_id` 承载）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476217。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`POST /v3/ecommerce/account/cancel-applications`

- **请求体**：`EcommerceCancelApplicationRequest` — 注销申请请求，见 `EcommerceCancelApplicationRequest`。
- **返回**：`EcommerceCancelApplicationResponse` — 注销申请单，见 `EcommerceCancelApplicationResponse`（与查询支**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/account/cancel-applications`；页内无频率数字。
- **`application_media_id` 必须先经本域媒体上传通道换取** / （`IWechatPayEcommerceCancelMediaService`， / `POST /v3/ecommerce/account/cancel-applications/media`，只收 JPG/BMP/PNG、≤2M）⇒ / 直传本地路径或图片字节必然失败。
- **`out_apply_no` 是幂等键**（商户侧唯一）：重复提交同单号不得当成新申请。

### 7. `QueryCancelApplicationAsync` — 「查询子商户注销申请状态」：按商户申请单号读注销进展。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476223。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/account/cancel-applications/out-apply-no/{outApplyNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outApplyNo` | Path | `string` | 商户注销申请单号（官方 path `out_apply_no`，必填 string(32)）。 |
- **返回**：`EcommerceCancelApplicationResponse` — 与提交支**同一应答类型**，见 `EcommerceCancelApplicationResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/account/cancel-applications/out-apply-no/{out_apply_no}`； / 「支持商户」【普通服务商】。**只有 `out_apply_no` 一条定位路径**（官方没有按微信单号查的页面） / ⇒ `out_apply_no` 必须落库。
- **两支应答表 5 行逐字段相同** ⇒ 共用一型；状态口径见 `EcommerceCancelApplicationStates`。

### 8. `QueryIndividualContractByOpenIdAsync` — 「查询个人收款方授权结果」：按用户 `openid` 取该用户在平台下的个人收款方授权状态与授权 ID。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012739528。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/individual-contracts/{openId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openId` | Path | `string` | 用户 OpenID（官方 path `openid`，必填 string(128)）。 |
| `appId` | Query `appid` | `string` | 应用 AppID（官方 query `appid`，**必填** string(128)）：须与<br> 同应用。 |
| `permissionType` | Query `permission_type` | `string` | 权限类型（官方 query `permission_type`，**必填**）：官方唯一取值见 `EcommercePermissionTypes`。 |
- **返回**：`EcommerceIndividualContractByOpenIdResponse` — 三字段应答，见 `EcommerceIndividualContractByOpenIdResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/individual-contracts/{openid}?appid=…&permission_type=INDIVIDUAL_PAYEE`； / 页内无频率数字。
- **本支的官方用途**：① 个人卖家开通授权收款后回查；② **交易下单前取 `individual_auth_id`** / ——它是本线个人收款方退款/异常退款支的身份定位键，拿不到就无法对这笔交易退款。
- **三个必填参数一个都不能省**：官方把 `appid` 与 `permission_type` 都列 `true`； / `permission_type` 虽然只有一个合法取值，仍是必填（SDK 不代为补默认值，补默认值会把 / 「将来出现第二种权限」的官方扩展静默吞掉）。

### 9. `QueryIndividualContractByOutContractCodeAsync` — 「按业务申请单号查询个人收款方签约详情」：用户完成预签约后取签约信息、用户 OpenID 与实名授权结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015851492。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/individual-contracts/out-contract-code/{outContractCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outContractCode` | Path | `string` | 业务申请单号（官方 path `out_contract_code`，必填 string(128)）：服务商下唯一，仅数字、字母、`-`、`_`。 |
| `appId` | Query `appid` | `string` | 签约和付款的 AppID（官方 query `appid`，**必填** string(32)）。 |
- **返回**：`EcommerceIndividualContractResponse` — 八字段应答（比 `openid` 支多签约时间与实名授权子树），见 `EcommerceIndividualContractResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/individual-contracts/out-contract-code/{out_contract_code}?appid=…`。
- **签约结果有延迟**：官方原文「当用户用预签约 ID 完成签约后才返回 `individual_auth_id`、 / `individual_openid`」⇒ 这两字段标选填，**空值表示用户还没签**，不是数据缺失，须轮询而不是报错。
- **`real_name_auth_result` 只在预签约用到实名授权/实名验证时返回**，其 / `access_token` **不可刷新**、过期即须重新走授权流程 ⇒ 属凭据，禁止进日志与遥测。
- **`appid` 的长度口径与上一支不同**（本页 string(32)、上一页 string(128)）⇒ / 照抄校验规则会误拒，本域不做长度校验。

### 10. `QuerySubjectSubMerchantsAsync` — 「查询主体名下子商户列表」：按主体类型 + 证件号码定位主体，取其未注销且与当前服务商有效绑定的子商户。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4033742105。

- **声明接口**：[IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)
- **官方契约**：`GET /v3/ecommerce/subject-sub-merchants`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `organizationType` | Query `organization_type` | `string` | 主体类型（官方 query `organization_type`，**必填**）：见 `EcommerceSubjectOrganizationTypes`。 |
| `certNumber` | Query `cert_number` | `string` | 证件号码密文（官方 query `cert_number`，**必填** string(512)）：须以微信支付公钥/平台证书加密，并带 `Wechatpay-Serial`。 |
| `limit` | Query `limit` | `int?` | 每页条数（官方 query `limit`，选填 integer）。 |
| `offset` | Query `offset` | `int?` | 偏移量（官方 query `offset`，选填 integer）。 |
- **返回**：`EcommerceSubjectSubMerchantsResponse` — 分页列表，见 `EcommerceSubjectSubMerchantsResponse`（元素见 `EcommerceSubjectSubMerchantItem`）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/subject-sub-merchants?organization_type=…&cert_number=…`； / 页内无频率数字。
- **必须带 `Wechatpay-Serial` 且 `cert_number` 是密文 Query**：明文即违规， / 密文也仍会进入 URL ⇒ 请求 URI 不得落日志（见本接口 Remarks 的敏感 Query 警示）。
- **结果集被官方收窄过三层**：只返回「属于指定主体」且「与当前服务商存在**有效父子绑定**」 / 且「**未注销**」的子商户 ⇒ 查不到不等于该主体没有子商户，历史/已注销的必须另走查询支取证。
- **分页是 `limit`/`offset` 双选填**（官方未给上限口径）⇒ SDK 不补默认值， / 也不把「空列表」当错误。

### 11. `SubmitApplymentAsync` — 「二级商户进件」：平台商户代其二级商户提交入驻申请，成为微信支付商户（**受理语义，须轮询查状态**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012713017。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`POST /v3/ecommerce/applyments/`

- **请求体**：`EcommerceApplymentRequest` — 进件申请，见 `EcommerceApplymentRequest`（12 个敏感字段须**密文**上送）。
- **返回**：`EcommerceApplymentSubmitResponse` — 申请单定位键（`applyment_id` + `out_request_no`），见 `EcommerceApplymentSubmitResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/applyments/`（**路由带尾斜杠**，照页）。
- **必须带 `Wechatpay-Serial`**：官方推荐微信支付公钥加密（头填公钥 ID），也可用平台证书序列号。
- **`out_request_no` 是幂等键**：被驳回后用**相同编号**重提即覆盖原申请单；换新编号会造出并行申请单。
- **不支持进件境外个人小微商户**（官方明文）；证照字段填媒体库图片 ID，须先走上传通道。

### 12. `QueryApplymentByApplymentIdAsync` — 「按申请单号查询进件状态」：官方要求提交后**定期轮询**至终态。
官方文档：[4012691469](https://pay.weixin.qq.com/doc/v3/partner/4012691469) /
[4015616714](https://pay.weixin.qq.com/doc/v3/partner/4015616714)（两页同契约）。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`GET /v3/ecommerce/applyments/{applymentId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applymentId` | Path | `long` | 微信支付申请单号（官方 path `applyment_id`，必填 integer）。 |
- **返回**：`EcommerceApplymentQueryResponse` — 进件状态全量（22 行），见 `EcommerceApplymentQueryResponse`（四页查询**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/applyments/{applyment_id}`；无加密头。
- **状态与伴随字段一一对应**：`sign_url`（仅 `NEED_SIGN`）、`sub_mchid` / （仅 `NEED_SIGN`/`FINISH`）、`account_validation`（仅 `ACCOUNT_NEED_VERIFY`）、 / `audit_detail`（仅 `REJECTED`）⇒ 判状态用 `EcommerceApplymentStates` 比对，勿以字段是否为 null 倒推。
- **`account_validation` 内的户名与卡号是密文**，须宿主用 API 证书私钥解密；SDK 原样透传。

### 13. `QueryApplymentByOutRequestNoAsync` — 「按业务申请编号查询进件状态」：与 `QueryApplymentByApplymentIdAsync` 同一应答类型，只是定位键不同。
官方文档：[4012691376](https://pay.weixin.qq.com/doc/v3/partner/4012691376) /
[4015616708](https://pay.weixin.qq.com/doc/v3/partner/4015616708)（两页同契约）。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`GET /v3/ecommerce/applyments/out-request-no/{outRequestNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRequestNo` | Path | `string` | 业务申请编号（官方 path `out_request_no`，必填 string(124)）：服务商自定义的唯一编号。 |
- **返回**：`EcommerceApplymentQueryResponse` — 与按申请单号查询**共用同一应答类型**，见 `EcommerceApplymentQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/applyments/out-request-no/{out_request_no}`。
- **本支适合「宿主只保留自己的编号」的场景**：官方四页应答逐字段相同 ⇒ 取回的状态视图与申请单号支完全一致。

### 14. `RevokeApplymentByApplymentIdAsync` — 「按申请单号撤销进件申请」：仅在六个非终态下允许，**官方零 body 参数**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015616720。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`POST /v3/ecommerce/applyments/{applymentId}/revoke`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applymentId` | Path | `long` | 微信支付申请单号（官方 path `applyment_id`，必填 integer）。 |
- **返回**：`EcommerceApplymentSubmitResponse` — 撤销后的申请单定位键，见 `EcommerceApplymentSubmitResponse`（与提交、另支撤销**三支共用**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/applyments/{applyment_id}/revoke`； / 定位键只在 path，**请求体为空**（官方参数表 Path 一行、Body 无）。
- **六状态白名单**：官方原文「仅当申请单状态为资料校验中、待账户验证、审核中、已驳回、待签约、 / 已冻结时才允许撤销」⇒ `FINISH`/`CANCELED` 不可撤销。
- **撤销后状态是 `CANCELED`**：结果仍须回 `QueryApplymentByApplymentIdAsync` 确认， / 应答本身不回状态。

### 15. `RevokeApplymentByOutRequestNoAsync` — 「按业务申请编号撤销进件申请」：与 `RevokeApplymentByApplymentIdAsync` 同契约，只是定位键不同。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015616721。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`POST /v3/ecommerce/applyments/out-request-no/{outRequestNo}/revoke`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRequestNo` | Path | `string` | 业务申请编号（官方 path `out_request_no`，必填 string(124)）。 |
- **返回**：`EcommerceApplymentSubmitResponse` — 与另支撤销**共用同一应答类型**，见 `EcommerceApplymentSubmitResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/applyments/out-request-no/{out_request_no}/revoke`； / 同样**零 body 参数**、同样六状态白名单。
- **两页应答逐字段相同**（`applyment_id` + `out_request_no`）⇒ 两支共用一个类型。

### 16. `SubmitAccountHostingApplymentAsync` — 「账户托管进件」：账号托管类平台商户的进件入口，多出代理签约/代理核身模式声明（**受理语义**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015616688。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`POST /v3/ecommerce/applyments/submit-account-hosting-applyment`

- **请求体**：`EcommerceAccountHostingApplymentRequest` — 托管进件申请，见 `EcommerceAccountHostingApplymentRequest`（敏感字段须**密文**上送）。
- **返回**：`EcommerceApplymentSubmitResponse` — 与普通进件**共用同一应答类型**，见 `EcommerceApplymentSubmitResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/applyments/submit-account-hosting-applyment`。
- **必须带 `Wechatpay-Serial`**：本方法与 `SubmitApplymentAsync` 是本域两支加密端点。
- **与普通进件的必填性分叉**：本页营业执照族整体降为选填、身份证/证件姓名与号码升为必填， / 且**必填 `proxy_sign_verify_mode`** 并多出 `id_card_complete_statement` ⇒ 故分建请求类型。
- **声明代理签约/代理核身后须补提交核身信息**（`SubmitSignVerifyInfoAsync`）， / 时序是「进件声明模式 → 提交核身 → 轮询状态」。

### 17. `SubmitSignVerifyInfoAsync` — 「提交代理签约核身信息」：账号托管进件里声明了「平台代理签约 + 代理核身入驻模式」后回填核身结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015616722。

- **声明接口**：[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)
- **官方契约**：`POST /v3/ecommerce/applyments/submit-sign-verify-info`

- **请求体**：`EcommerceSignVerifyInfoRequest` — 核身信息，见 `EcommerceSignVerifyInfoRequest`（`out_request_no` 与 `applyment_id` 二选一定位）。
- **返回**：`EcommerceSignVerifyInfoResponse` — 申请单号，见 `EcommerceSignVerifyInfoResponse`（**官方只回一行**，与三支提交/撤销的应答不同）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/applyments/submit-sign-verify-info`；无加密头 / （本支传的是核身**方式与结果布尔值**，不传证件明文）。
- **只在托管模式下存在**：官方原文「在申请单中声明『走平台代理签约和代理核身入驻模式』时，可使用该接口」 / ⇒ `pre_sign_time` 与 `proxy_verify_identity_info` 的条件必填由 `proxy_sign_verify_mode=true` 决定。
- **应答没有任何状态字段**：核身是否被采纳须回 `QueryApplymentByApplymentIdAsync` 读 / `applyment_state`；`verify_identity_result=false` 表示核身不通过，官方未定义额外重试入参。

### 18. `ApplyFundFlowBillAsync` — 「申请二级商户资金账单」：取该电商平台二级商户资金流水账单的**分片下载地址列表**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012760697。

- **声明接口**：[IWechatPayEcommerceBillService](./IWechatPayEcommerceBillService.cs)
- **官方契约**：`GET /v3/ecommerce/bill/fundflowbill`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billDate` | Query `bill_date` | `string` | 账单日期（官方 query `bill_date`，**必填** string(10)）：`yyyy-MM-DD`，仅支持三个月内。 |
| `accountType` | Query `account_type` | `string` | 资金账户类型（官方 query `account_type`，**必填**）：本支**只接受** `ALL`。 |
| `algorithm` | Query `algorithm` | `string` | 账单文件加密算法（官方 query `algorithm`，**必填**）：见 `EcommerceBillAlgorithms`。 |
| `tarType` | Query `tar_type` | `string?` | 压缩类型（官方 query `tar_type`，**选填**）：不填即返回未压缩数据流，见 `EcommerceBillTarTypes`。 |
- **返回**：`EcommerceFundFlowBillResponse` — 分片计数与分片列表，见 `EcommerceFundFlowBillResponse`；须按 `bill_sequence` 合并。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/bill/fundflowbill?bill_date=…&account_type=ALL&algorithm=…&tar_type=…`。
- **页内无端点级频率数字**（与直连资金账单一致）。
- **业务错误码**：400 `PARAM_ERROR` / 400 `INVALID_REQUEST` / 401 `SIGN_ERROR` / / 403 `NO_AUTH`（未开通产品权限）/ 500 `SYSTEM_ERROR` / 429 `FREQUENCY_LIMITED`， / 原样透传至 `Code`。

### 19. `QueryBalanceAsync` — 「查询二级商户账户实时余额」：按二级商户号取可用与不可用金额（单位**分**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476690。

- **声明接口**：[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)
- **官方契约**：`GET /v3/ecommerce/fund/balance/{subMchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Path | `string` | 二级商户号（官方 path `sub_mchid`，必填 string(32)）：由微信支付生成并下发。 |
| `accountType` | Query `account_type` | `string?` | 账户类型（官方 query `account_type`，**选填**）：取值见 `EcommerceAccountTypes`，缺省语义见 Remarks。 |
- **返回**：`EcommerceBalanceResponse` — 二级商户余额，见 `EcommerceBalanceResponse`（与日终支**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/fund/balance/{sub_mchid}`；`account_type` 是**选填 query**。
- **`account_type` 的取值口径按页分列**：本页请求侧列四值（`BASIC`/`FEES`/ / `OPERATION`/`DEPOSIT`），而**提现支只接受三值**（无 `DEPOSIT`）⇒ 保证金查得到、提不出。
- **应答的 `pending_amount` 与 `account_type` 官方标选填** ⇒ 字段缺省不等于 0 / 空串。

### 20. `QueryDayEndBalanceAsync` — 「查询二级商户账户日终余额」：取指定日期当天 24 点的账面余额，与实时余额**同一应答类型**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476693。

- **声明接口**：[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)
- **官方契约**：`GET /v3/ecommerce/fund/enddaybalance/{subMchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Path | `string` | 二级商户号（官方 path `sub_mchid`，必填 string(32)）。 |
| `date` | Query `date` | `string` | 日期（官方 query `date`，**必填** string(10)）：格式 `YYYY-MM-DD`，只可查 90 天内。 |
| `accountType` | Query `account_type` | `string?` | 账户类型（官方 query `account_type`，**选填**）：本页应答侧只列 `BASIC`/`DEPOSIT` 两值。 |
- **返回**：`EcommerceBalanceResponse` — 与实时余额**同一应答类型**，见 `EcommerceBalanceResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/fund/enddaybalance/{sub_mchid}?date=YYYY-MM-DD`。
- **`date` 是必填 query**：漏传即 400 `PARAM_ERROR`。**当日日终余额次日才生成**， / 官方建议上午 10 点后查询 ⇒ 查当天拿不到数。
- **日终余额是结算后的账面数**，与实时余额的差属时间口径差，不是资金异常。

### 21. `ApplyWithdrawAsync` — 「申请二级商户预约提现」：代二级商户发起账户预约提现（**受理应答没有状态**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476652。

- **声明接口**：[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)
- **官方契约**：`POST /v3/ecommerce/fund/withdraw`

- **请求体**：`EcommerceWithdrawRequest` — 提现请求，见 `EcommerceWithdrawRequest`。
- **返回**：`EcommerceWithdrawApplyResponse` — 仅 `sub_mchid` / `withdraw_id` / `out_request_no` 三字段，见 `EcommerceWithdrawApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/fund/withdraw`；限频 **40 次/秒**。
- **本支是「次日发起、当日到账」的预约单**，不是即时出款：提交后系统**次日**才发起提现。
- **失败重试口径**：`SYSTEM_ERROR` 与回包解析异常**不得换单**，须以原单（全部参数不变）重试； / 防重窗口只有 **6 天**（按 `out_request_no` + `sub_mchid`）⇒ 换单重试等于重复出款。
- **金额上限 8 亿（分）**⇒ 越出 int32；`account_type` 三值（无 `DEPOSIT`）； / `notify_url` 必须 HTTPS 且不携带参数。未配回调时须自行轮询查询支闭环。

### 22. `QueryWithdrawByWithdrawIdAsync` — 「按微信支付提现单号查询预约提现状态」：读提现单终态（六值）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476665。

- **声明接口**：[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)
- **官方契约**：`GET /v3/ecommerce/fund/withdraw/{withdrawId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `withdrawId` | Path | `string` | 微信支付预约提现单号（官方 path `withdraw_id`，必填 string(128)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string）。 |
- **返回**：`EcommerceWithdrawQueryResponse` — 提现单详情，见 `EcommerceWithdrawQueryResponse`（与按单号查**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/fund/withdraw/{withdraw_id}?sub_mchid=…`；限频 **100 次/秒**。
- **`sub_mchid` 是必填 query**：官方把它放在 Query 参数表而非 Path ⇒ 漏传即参数错误。
- **只可查 90 日内**发起的提现单（以微信侧创建时间为准）。`INIT` 态须用**原单**重试发起， / `CREATE_SUCCESS` 态表示已受理等待银行结果 ⇒ 两者都不是失败，不得据此判负。

### 23. `QueryWithdrawByOutRequestNoAsync` — 「按商户预约提现单号查询预约提现状态」：与上一支**同一应答类型**，只是定位键换成商户侧单号。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476656。

- **声明接口**：[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)
- **官方契约**：`GET /v3/ecommerce/fund/withdraw/out-request-no/{outRequestNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRequestNo` | Path | `string` | 商户预约提现单号（官方 path `out_request_no`，必填 string(32)，仅字母数字）。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string）。 |
- **返回**：`EcommerceWithdrawQueryResponse` — 与按 `withdraw_id` 查**同一应答类型**，见 `EcommerceWithdrawQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/fund/withdraw/out-request-no/{out_request_no}?sub_mchid=…`； / 限频 **100 次/秒**。官方明说两支查询「返回结果一致」⇒ 本支是宿主只有自有单号时的等价入口。
- **`SUCCESS` 之后仍可能翻转为 `REFUND`**（银行约 7 个工作日内处理退票，资金自动退回原账户） / ⇒ 财务对账要以「超过退票窗口后的状态」为准，不能把 `SUCCESS` 当最终态立即结算。

### 24. `ApplyTransferAuthorizationAsync` — 「申请二级商户商家转账授权」：上送已线下签署的转账授权协议清单（**受理语义，回 200 不等于已授权**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504208。

- **声明接口**：[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)
- **官方契约**：`POST /v3/ecommerce/mch-transfer/authorizations`

- **请求体**：`EcommerceTransferAuthorizationRequest` — 授权申请，见 `EcommerceTransferAuthorizationRequest`（`agreement_list` 承载协议编号/版本/签约机构/签约时间）。
- **返回**：`EcommerceTransferAuthorizationResponse` — 授权状态，见 `EcommerceTransferAuthorizationResponse`（与 `QueryTransferAuthorizationAsync` 共用）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/mch-transfer/authorizations`；无加密头。
- **须再查单收敛**：官方值集含 `AUTHORIZE_PROCESSING` ⇒ 拿到本应答后须用 / `QueryTransferAuthorizationAsync` 轮询到 `AUTHORIZED` 才能发起转账。
- **`accept_time` 只在本页文档化**（查询页未列出）⇒ 勿以该字段是否出现来判断走的是哪一支。

### 25. `QueryTransferAuthorizationAsync` — 「查询二级商户商家转账授权状态」：转账发起前的前置校验入口。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504209。

- **声明接口**：[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)
- **官方契约**：`GET /v3/ecommerce/mch-transfer/authorizations/{subMchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Path | `string` | 二级商户号（官方 path `sub_mchid`，必填 string(32)）：需授权商家转账功能的商户号。 |
- **返回**：`EcommerceTransferAuthorizationResponse` — 与申请支**共用同一应答类型**，见 `EcommerceTransferAuthorizationResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/mch-transfer/authorizations/{sub_mchid}`； / 二级商户号走 **path**（申请支走 body）⇒ 同一凭据两种传法，照页实现。
- **`UNAUTHORIZED` 是「未授权」而非「授权失败」**：三值语义见 `EcommerceTransferAuthorizationStates`。

### 26. `CreateTransferBillAsync` — 「发起商家转账」：平台商户代二级商户向用户 openid 转账，等待用户在微信收款确认页面确认收款。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504211。

- **声明接口**：[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)
- **官方契约**：`POST /v3/ecommerce/mch-transfer/transfer-bills`

- **请求体**：`EcommerceTransferBillCreateRequest` — 转账请求，见 `EcommerceTransferBillCreateRequest`（`user_name` 为**密文**）。
- **返回**：`EcommerceTransferBillCreateResponse` — 转账受理单，见 `EcommerceTransferBillCreateResponse`（含**仅此支返回**的 `package_info`）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/mch-transfer/transfer-bills`。
- **必须带 `Wechatpay-Serial`**：本方法是本域唯一需要该头的端点，`user_name` / 须以微信支付公钥（推荐）或平台证书公钥加密；转账金额 >= 2,000 元（即 200000 分）时该字段**必填**。
- **前置：授权须已是 `AUTHORIZED`**（`QueryTransferAuthorizationAsync`）。
- **`package_info` 须自行持久化**：用于拉起微信收款确认页面，两支查单**不再返回**该字段。

### 27. `QueryMchTransferBillByOutBillNoAsync` — 「按商户单号查询商家转账订单」：转账终态与失败原因的唯一权威来源。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504213。

- **声明接口**：[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)
- **官方契约**：`GET /v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 商户单号（官方 path `out_bill_no`，必填 string(32)）：只能是数字与大小写字母，商户系统内唯一。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
- **返回**：`EcommerceMchTransferBillQueryResponse` — 转账单全量（14 行），见 `EcommerceMchTransferBillQueryResponse`（与 `QueryMchTransferBillByTransferBillNoAsync`**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}?sub_mchid=`； / `sub_mchid` 是**必填 query**，漏传即 400 `PARAM_ERROR`。
- **`fail_reason` 是选填**：官方只在失败态给出，缺省不代表无失败； / `PROCESSING` 长期不收敛时官方建议检查账户余额。

### 28. `CancelTransferBillAsync` — 「撤销转账」：在**用户确认收款之前**撤销付款请求（**异步受理**，返回成功只表示撤销请求已受理）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504212。

- **声明接口**：[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)
- **官方契约**：`POST /v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBillNo` | Path | `string` | 商户单号（官方 path `out_bill_no`，必填 string(32)）：待撤销的转账单。 |

- **请求体**：`EcommerceTransferBillCancelRequest` — 撤销请求，见 `EcommerceTransferBillCancelRequest`（官方入参只有 `sub_mchid` 一行）。
- **返回**：`EcommerceTransferBillCancelResponse` — 撤销受理结果（`state` 只有 `CANCELING`/`CANCELLED` 两值），见 `EcommerceTransferBillCancelResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}/cancel`； / `sub_mchid` 走 **body**（两支查单走 query）⇒ 同一凭据三种传法，照页实现。
- **窗口只在用户确认收款之前**：官方原文「在用户确认收款之前可以通过该接口撤销付款」⇒ / 状态已是 `TRANSFERING`/`SUCCESS` 时撤销必失败。
- **受理 ≠ 完成**：官方原文「系统会异步处理退款等操作，以最终查询单据返回状态为准」⇒ / 须回查 `QueryMchTransferBillByOutBillNoAsync` 直到 `CANCELLED`。

### 29. `QueryMchTransferBillByTransferBillNoAsync` — 「按微信转账单号查询商家转账订单」：与 `QueryMchTransferBillByOutBillNoAsync` 同一应答类型，只是定位键不同。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504214。

- **声明接口**：[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)
- **官方契约**：`GET /v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transferBillNo` | Path | `string` | 微信转账单号（官方 path `transfer_bill_no`，必填 string(64)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
- **返回**：`EcommerceMchTransferBillQueryResponse` — 与按商户单号查单**共用同一应答类型**，见 `EcommerceMchTransferBillQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transfer_bill_no}?sub_mchid=`。
- **两页应答表 14 行逐行相同**（2026-10-15 比对）⇒ 共用一个类型； / 本域不含「转账列表」面，官方无按商户号枚举转账单的端点。

### 30. `PreEntrustSignAsync` — 「预签约」：外部 App 拉起微信客户端发起委托代扣签约前，须后台先调本接口换取签约会话 ID（**2 小时有效**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012881608。

- **声明接口**：[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)
- **官方契约**：`POST /v3/ecommerce/combine-papay/contracts/pre-entrust-sign`

- **请求体**：`EcommercePapayPreEntrustSignRequest` — 预签约请求，见 `EcommercePapayPreEntrustSignRequest`。
- **返回**：`EcommercePapayPreEntrustSignResponse` — 签约会话，见 `EcommercePapayPreEntrustSignResponse`（含 `pre_entrustweb_id` 与两个条件字段）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/combine-papay/contracts/pre-entrust-sign`；无加密头。
- **前置**：「使用前请确认已开通委托代扣能力」。
- **会话 2 小时过期**：取到 `pre_entrustweb_id` 后须在窗口内拉起客户端；过期只能重新预签约。
- **`miniprogram_username`/`miniprogram_path` 决定调起 SDK**：返回则用 LaunchMiniProgram / 且字段名与值原样填入，未返回则该模板只能走 OpenBusinessWebview。

### 31. `QueryPapayContractAsync` — 「查询签约协议」：签约状态的**唯一权威来源**（用户放弃签约时官方不通知）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884060。

- **声明接口**：[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)
- **官方契约**：`GET /v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `long` | 委托代扣协议模板 ID（官方 path `plan_id`，必填 integer）。 |
| `outContractCode` | Path | `string` | 商户签约协议号（官方 path `out_contract_code`，必填 string(32)）：商户侧唯一。 |
- **返回**：`EcommercePapayContractQueryResponse` — 签约协议详情（15 行），见 `EcommercePapayContractQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/combine-papay/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}`。
- **前置**：「商户已经成功获取过委托代扣的签约」⇒ 未签约时本支报错，不返回空对象。
- **「未收到签约通知」不等于失败**：官方只对成功的签约下发异步通知 ⇒ 判状态须用 / `contract_state`（见 `EcommercePapayContractStates`）。
- **解约信息是条件子表**：`contract_terminate_info` 只在已解约时返回， / 其 `contract_termination_mode` 见 `EcommercePapayTerminationModes`。

### 32. `TerminatePapayContractAsync` — 「解约」：对**已生效**的委托代扣签约协议发起解约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884072。

- **声明接口**：[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)
- **官方契约**：`POST /v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `long` | 委托代扣协议模板 ID（官方 path `plan_id`，必填 integer）。 |
| `outContractCode` | Path | `string` | 商户签约协议号（官方 path `out_contract_code`，必填 string(32)）。 |

- **请求体**：`EcommercePapayContractTerminateRequest` — 解约请求，见 `EcommercePapayContractTerminateRequest`（官方入参只有 `contract_termination_remark` 一行且必填）。
- **返回**：`EcommercePapayContractTerminateResponse` — 解约后的定位键（3 行），见 `EcommercePapayContractTerminateResponse`（**不回状态**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/combine-papay/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate`。
- **前置**：「调用前请确认对应的签约协议是已生效状态的签约协议」⇒ 未生效/已解约的协议调用必失败。
- **`contract_termination_remark` 官方标必填**：会体现在解约信息子表里，属留证文本。
- **应答只有三个定位键、没有状态字段**：解约结果须回 `QueryPapayContractAsync` 读 / `contract_state` 与 `contract_terminate_info` 确认。

### 33. `CreatePapayCombineTransactionAsync` — 「合单免密扣款」：凭已生效的签约协议对多笔子单一并发起委托代扣（**同步扣费，商户侧超时窗口 1 分钟**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884121。

- **声明接口**：[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)
- **官方契约**：`POST /v3/ecommerce/combine-papay/transactions`

- **请求体**：`EcommercePapayCombineTransactionRequest` — 合单扣款请求，见 `EcommercePapayCombineTransactionRequest`。
- **返回**：`EcommercePapayCombineTransactionResponse` — 合单结果（21 行），见 `EcommercePapayCombineTransactionResponse`（与 `QueryPapayCombineTransactionAsync`**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/combine-papay/transactions`；无加密头。
- **两级前置**：服务商须开通**合单**委托代扣能力、用户已签约，**且签约的模板本身具备合单代扣能力**。
- **同步扣费 + 1 分钟窗口**：官方原文「商户超时时间为 1 分钟，超过 1 分钟未能返回结果时， / 商户可主动发起撤销」（`ReversePapayCombineTransactionAsync`）⇒ 窗口归宿主计时。
- **应答的 `sub_orders[].trade_type` 官方只有 `PAP` 一个值**（见 `EcommercePapayTradeTypes`）， / 与直连合单的 `JSAPI`/`NATIVE`/`APP` 形态不同。

### 34. `QueryPapayCombineTransactionAsync` — 「查询合单扣款」：按商户合单号读合单与子单状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884130。

- **声明接口**：[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)
- **官方契约**：`GET /v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `combineOutTradeNo` | Path | `string` | 商户合单总订单号（官方 path `combine_out_trade_no`，必填 string(32)）。 |
- **返回**：`EcommercePapayCombineTransactionResponse` — 与扣款支**共用同一应答类型**（21 行逐字段相同），见 `EcommercePapayCombineTransactionResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/combine-papay/transactions/{combine_out_trade_no}`。
- **前置**：「调用前请商户确认已经发起了免密的合单支付」。
- **撤销与扣款的结果都收敛到本方法**：撤销支零应答 ⇒ 判撤销是否生效只能读这里的子单状态族 / （`EcommercePapayTradeStates`）。

### 35. `ReversePapayCombineTransactionAsync` — 「撤销合单扣款」：对已发起的免密合单支付按子单粒度撤销（**官方应答零业务字段**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012884126。

- **声明接口**：[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)
- **官方契约**：`POST /v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}/reverse`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `combineOutTradeNo` | Path | `string` | 商户合单总订单号（官方 path `combine_out_trade_no`，必填 string(32)）：待撤销的合单。 |

- **请求体**：`EcommercePapayReverseRequest` — 撤销请求，见 `EcommercePapayReverseRequest`（`combine_appid` + 待撤销子单列表）。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/combine-papay/transactions/{combine_out_trade_no}/reverse`。
- **前置**：「调用前请商户确认已经发起了免密的合单支付」；扣款超过 1 分钟无结果时官方指引即走本支。
- **不建应答类型**：本页应答节只有 `200 OK`、无任何参数行 ⇒ / 伪造一个「看起来合理」的应答类型即造出官方永不返回的字段（守卫 EC-S2）。
- **200 不等于撤销成功**：本支不回状态，须查单读子单 `trade_state` 收敛。

### 36. `CreateProfitSharingOrderAsync` — 「请求分账」：平台商户代二级商户把结算后的资金分给分账接收方（**异步受理**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012691594。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/profitsharing/orders`

- **请求体**：`EcommerceProfitSharingOrderRequest` — 分账请求，见 `EcommerceProfitSharingOrderRequest`。
- **返回**：`EcommerceProfitSharingOrderResponse` — 分账单（受理态），见 `EcommerceProfitSharingOrderResponse`（与查询支**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/profitsharing/orders`；限频 2000 QPS（平台商户） / / 300 QPS（交易收款商户）。
- **必须带 `Wechatpay-Serial`**：`receivers[].receiver_name` 是密文字段。
- **异步处理**：应答只代表受理，最终结果用 `QueryProfitSharingOrderAsync` 读 / `state` + `receivers[].result`。同一笔订单最多 50 次分账、每次最多 50 个接收方。
- **官方支持幂等重入**（同参数重复请求等同一笔）⇒ 重试请保持参数原样，不要换 `out_order_no`。

### 37. `QueryProfitSharingOrderAsync` — 「查询分账结果」：按 `sub_mchid` + `transaction_id` + `out_order_no` 三键查分账单，
也是**分账完结结果的唯一观察点**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477734。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`GET /v3/ecommerce/profitsharing/orders`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
| `transactionId` | Query `transaction_id` | `string` | 微信支付订单号（官方 query `transaction_id`，**必填** string(32)）。 |
| `outOrderNo` | Query `out_order_no` | `string` | 商户分账单号（官方 query `out_order_no`，**必填** string(64)）。 |
- **返回**：`EcommerceProfitSharingOrderResponse` — 与请求分账**同一应答类型**，见 `EcommerceProfitSharingOrderResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/profitsharing/orders?sub_mchid=…&transaction_id=…&out_order_no=…`； / **三个 query 全部必填**（官方没有「只给 transaction_id 查全部」的形态）。
- **本支同时承载「完结结果」**：官方原文「发起分账完结请求后，可调用此接口查询分账完结的结果」⇒ / `FinishOrderAsync` 之后仍以本支读数，不要为完结另建查询。

### 38. `QueryUnsplitAmountAsync` — 「查询订单剩余待分金额」：这笔微信订单还能分多少（单位分）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477751。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`GET /v3/ecommerce/profitsharing/orders/{transactionId}/amounts`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `transactionId` | Path | `string` | 微信支付订单号（官方 path `transaction_id`，必填 string(32)）。 |
- **返回**：`EcommerceUnsplitAmountResponse` — 剩余待分金额，见 `EcommerceUnsplitAmountResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/profitsharing/orders/{transaction_id}/amounts`； / 无 query、无包体。**本支是官方少见的「金额单值」应答** ⇒ 用于决定补差/分账额度， / 但不代表解冻状态（解冻看 `QueryProfitSharingOrderAsync` 的 `state`）。

### 39. `FinishOrderAsync` — 「分账完结（解冻剩余资金）」：把订单剩余金额全部解冻给二级商户（**异步受理、应答无状态**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477745。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/profitsharing/finish-order`

- **请求体**：`EcommerceProfitSharingFinishRequest` — 完结请求，见 `EcommerceProfitSharingFinishRequest`。
- **返回**：`EcommerceProfitSharingFinishResponse` — 只回四行、**没有结果状态**，见 `EcommerceProfitSharingFinishResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/profitsharing/finish-order`； / 限频「单个分账接收方商户 60 QPS」。
- **方案里这一行重复出现两次（row 38/39）是同一端点** ⇒ 本域只声明一个方法， / 守卫 EC-P1 锁定 `(POST, /v3/ecommerce/profitsharing/finish-order)` 唯一。
- **不可逆**：完结后剩余资金全额解冻、该订单不能再分账 ⇒ 误调没有补救接口。 / 结果一律回到 `QueryProfitSharingOrderAsync` 确认。

### 40. `AddReceiverAsync` — 「添加分账接收方」：在**电商平台维度**维护接收方列表（其他二级商户共用该列表）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477758。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/profitsharing/receivers/add`

- **请求体**：`EcommerceAddReceiverRequest` — 添加请求，见 `EcommerceAddReceiverRequest`。
- **返回**：`EcommerceReceiverResponse` — 只回 `type` + `account`，见 `EcommerceReceiverResponse`（增删两支**共用**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/profitsharing/receivers/add`；页内无频率数字。
- **接收方归属平台而非二级商户**：官方明文「添加的分账接收方统一在电商平台维度管理， / 其他二级商户均可向该列表中的接收方分账」⇒ 不要按二级商户重复添加（数量上限 2 万）。
- **个人接收方须加密姓名并确认授权**：`name` 官方标选填但为个人类型时须密文； / 本页参数表无 `Wechatpay-Serial` ⇒ 加密口径以官方字段说明为准（守卫 EC-P6 记录「仅请求分账页列该头」）。

### 41. `DeleteReceiverAsync` — 「删除分账接收方」：删除后不能再把二级商户的资金分给该接收方。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477759。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/profitsharing/receivers/delete`

- **请求体**：`EcommerceDeleteReceiverRequest` — 删除请求，见 `EcommerceDeleteReceiverRequest`。
- **返回**：`EcommerceReceiverResponse` — 与添加支**同一应答类型**，见 `EcommerceReceiverResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/profitsharing/receivers/delete`；页内无频率数字。
- **删除不回溯历史分账单**：已完成的分账不受影响，但删除后同一 `account` 再想收款须重新添加 / ⇒ 与 2 万上限一起构成本域的容量治理问题。

### 42. `CreateProfitSharingReturnAsync` — 「请求分账回退」：退款前把已分给接收方的资金退回分账方（**同步返回结果**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477737。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/profitsharing/returnorders`

- **请求体**：`EcommerceProfitSharingReturnRequest` — 回退请求，见 `EcommerceProfitSharingReturnRequest`。
- **返回**：`EcommerceProfitSharingReturnResponse` — 回退单，见 `EcommerceProfitSharingReturnResponse`（与查询支**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/profitsharing/returnorders`；页内无频率数字。
- **资格三条缺一不可**：接收方类型是 `MERCHANT_ID`、分账单 `state=FINISHED`、 / `result=SUCCESS`；**分账给个人的分账单不支持回退**。累计回退金额不得超过原分给该接收方的金额， / 同一笔分账单最多 50 次回退，时限 180 天。
- **同步处理**：与请求分账相反，本支实时返回结果；若仍是「处理中」则用 / `QueryProfitSharingReturnAsync` 复核。**回退与退款无强耦合**（余额充足可直接退款）⇒ / 是否回退由平台决定，SDK 不做隐式串联。

### 43. `QueryProfitSharingReturnAsync` — 「查询分账回退结果」：核实回退单的最终状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477740。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`GET /v3/ecommerce/profitsharing/returnorders`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
| `outReturnNo` | Query `out_return_no` | `string` | 商户回退单号（官方 query `out_return_no`，**必填** string(64)）。 |
| `orderId` | Query `order_id` | `string?` | 微信分账订单号（官方 query `order_id`，选填 string(64)）。 |
| `outOrderNo` | Query `out_order_no` | `string?` | 商户分账单号（官方 query `out_order_no`，选填 string(64)）。 |
- **返回**：`EcommerceProfitSharingReturnResponse` — 与请求回退**同一应答类型**，见 `EcommerceProfitSharingReturnResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/profitsharing/returnorders?sub_mchid=…&out_return_no=…`； / 方案里这一行也重复出现两次（row 45/47）⇒ 单方法承载，守卫 EC-P1 锁定唯一。
- **只有 `sub_mchid` 与 `out_return_no` 必填**，`order_id`/`out_order_no` 是选填辅助定位 / ⇒ 不要要求调用方两者都传。

### 44. `CreateSubsidyAsync` — 「发起补差」：平台向二级商户订单补入差额（**与普通服务商身份绑定，一笔订单只能补一次**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477631。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/subsidies/create`

- **请求体**：`EcommerceCreateSubsidyRequest` — 补差请求，见 `EcommerceCreateSubsidyRequest`。
- **返回**：`EcommerceCreateSubsidyResponse` — 补差单，见 `EcommerceCreateSubsidyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/subsidies/create`，「支持商户」【普通服务商】。
- **时序前置**：必须「下单带补差标识 → 支付成功并完成结算 → 分账之前」调用；补差金额须与下单约定一致 / （用户退款后可小于下单金额），事后不可提额。
- **重入有效期 180 天**；`SYSTEM_ERROR` **必须原参数重入**——换金额重试可能重复扣款 / （系统 1 天后回退到原账户）。

### 45. `ReturnSubsidyAsync` — 「补差回退」：订单退款时把补差成功的资金退回平台（**同步处理，一笔退款单只允许回退一次**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477636。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/subsidies/return`

- **请求体**：`EcommerceReturnSubsidyRequest` — 回退请求，见 `EcommerceReturnSubsidyRequest`。
- **返回**：`EcommerceReturnSubsidyResponse` — 补差回退单，见 `EcommerceReturnSubsidyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/subsidies/return`，「支持商户」【普通服务商】。
- **累计回退不得超过补差金额**，且**一笔微信退款单只允许补差回退一次** ⇒ / `refund_id` 是天然幂等边界，重试请保持原商户回退单号与原参数。
- **本支失败拼写是 `FAIL`**（`EcommerceSubsidyReturnResults`）， / 与回补垫付支的 `FAILED` 不同 ⇒ 两个常量类不合并、判错逻辑不得跨支复用。

### 46. `CancelSubsidyAsync` — 「取消补差」：带补差标识的订单若不需补差，可在分账前取消。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012477639。

- **声明接口**：[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)
- **官方契约**：`POST /v3/ecommerce/subsidies/cancel`

- **请求体**：`EcommerceCancelSubsidyRequest` — 取消请求，见 `EcommerceCancelSubsidyRequest`。
- **返回**：`EcommerceCancelSubsidyResponse` — 取消结果，见 `EcommerceCancelSubsidyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/subsidies/cancel`，「支持商户」【普通服务商】。
- **前置条件是「订单发生退款」**（官方原文第 3 条）⇒ 未退款的订单取消补差会被拒； / 取消完成后才能对未补差的订单分账。

### 47. `ApplyRefundAsync` — 「申请退款 · 二级商户身份」：按 `sub_mchid` 定位收款方发起退款（**受理语义，不代表退款成功**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476892。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`POST /v3/ecommerce/refunds/apply`

- **请求体**：`EcommerceRefundApplyRequest` — 退款请求，见 `EcommerceRefundApplyRequest`。
- **返回**：`EcommerceRefundApplyResponse` — 退款受理单（**无 `status`**），见 `EcommerceRefundApplyResponse`；终态须查单。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/refunds/apply`；限频 150 QPS 且**单笔订单 1 QPM**。
- **`sub_mchid` 官方标选填、注「二级商户收款必填」**（条件必填）；`transaction_id` / 与 `out_trade_no` 均选填 ⇒ 原交易定位由宿主二选一保证。
- **个人收款方请改用 `ApplyIndividualRefundAsync`**：本方法报文不含 / `individual_auth_id`，混用即造出官方不认的载荷。

### 48. `QueryRefundByRefundIdAsync` — 「查单 · 二级商户身份，按微信退款单号」：读退款终态与资金去向。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476908。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`GET /v3/ecommerce/refunds/id/{refundId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `refundId` | Path | `string` | 微信支付退款单号（官方 path `refund_id`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
- **返回**：`EcommerceRefundQueryResponse` — 退款单全量，见 `EcommerceRefundQueryResponse`（与 `QueryRefundByOutRefundNoAsync`**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/refunds/id/{refund_id}?sub_mchid=`； / `sub_mchid` 是**必填 query**，漏传即 400 `PARAM_ERROR`。
- **查单比受理多出七个字段**：`status`、`transaction_id`、`out_trade_no`、 / `channel`、`user_received_account`、`success_time`、`funds_account` ⇒ / 退款终态与资金去向只能从本类方法取。
- **时延口径**：零钱退款 20 分钟内到账，银行卡退款 3 个工作日后再查。

### 49. `QueryRefundByOutRefundNoAsync` — 「查单 · 二级商户身份，按商户退款单号」：与 `QueryRefundByRefundIdAsync` 同一应答类型，只是定位键不同。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476911。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`GET /v3/ecommerce/refunds/out-refund-no/{outRefundNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRefundNo` | Path | `string` | 商户退款单号（官方 path `out_refund_no`，必填 string(64)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
- **返回**：`EcommerceRefundQueryResponse` — 与按 `refund_id` 查单**共用同一应答类型**，见 `EcommerceRefundQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/refunds/out-refund-no/{out_refund_no}?sub_mchid=`。
- **两页应答表逐行相同**（2026-10-15 比对，26 行）⇒ 共用一个类型；本域不含「退款列表」面， / 官方无按商户号枚举退款单的端点。

### 50. `ReturnAdvanceAsync` — 「发起垫付退款回补」：把电商平台垫付的退款资金从二级商户回补回垫付账户（**方向与退款相反**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476927。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`POST /v3/ecommerce/refunds/{refundId}/return-advance`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `refundId` | Path | `string` | 微信退款单号（官方 path `refund_id`，必填 string(32)）：**必须是垫付退款的微信退款单**。 |

- **请求体**：`EcommerceReturnAdvanceRequest` — 回补请求，见 `EcommerceReturnAdvanceRequest`（官方入参只有 `sub_mchid` 一行）。
- **返回**：`EcommerceReturnAdvanceResponse` — 回补结果，见 `EcommerceReturnAdvanceResponse`（与 `QueryReturnAdvanceAsync`**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/refunds/{refund_id}/return-advance`； / `sub_mchid` 走 **body**（查询支走 query）⇒ 同一凭据两种传法，照页实现。
- **非垫付单回补会被拒**：官方注明回补对象必须是垫付退款的微信退款单。
- **能力需先开通**（联系微信支付客服）；结果为处理中时官方在 **90 天**内定期重试， / 90 天后仍余额不足即置回补失败且**不可再发起**。

### 51. `QueryReturnAdvanceAsync` — 「查询垫付退款回补结果」：回补是异步动作，重试结果需查单确认。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012476916。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`GET /v3/ecommerce/refunds/{refundId}/return-advance`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `refundId` | Path | `string` | 微信退款单号（官方 path `refund_id`，必填 string(32)）。 |
| `subMchId` | Query `sub_mchid` | `string` | 二级商户号（官方 query `sub_mchid`，**必填** string(32)）。 |
- **返回**：`EcommerceReturnAdvanceResponse` — 与发起回补**共用同一应答类型**，见 `EcommerceReturnAdvanceResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/refunds/{refund_id}/return-advance?sub_mchid=`。
- **`result` 为处理中时商户可以重试**（取值见 `EcommerceReturnAdvanceResults`）； / 判读方向注意 `payer_*` 是出款方、`payee_*` 是入账方。

### 52. `ApplyIndividualRefundAsync` — 「申请退款 · 商品单个人收款方身份」：按 `individual_auth_id` 定位收款方发起退款。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012649457。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`POST /v3/ecommerce/refunds/apply`

- **请求体**：`EcommerceIndividualRefundApplyRequest` — 退款请求，见 `EcommerceIndividualRefundApplyRequest`。
- **返回**：`EcommerceIndividualRefundApplyResponse` — 退款受理单（**无 `status`**，14 行），见 `EcommerceIndividualRefundApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/refunds/apply`（与二级商户身份支**同路由不同契约**）； / 限频 150 QPS 且单笔订单 1 QPM。
- **本页 `funds_account` 必填且官方只列 `UNSETTLED`**（未结算资金）；二级商户支该字段选填 / 且另有 `AVAILABLE`/`PREPAID` ⇒ 同名字段的必填性与取值集同时分叉。
- **`transaction_id` 在本页是必填**（二级商户支选填）；本页无 `sub_mchid`、 / `sub_appid`、`refund_account` 与 `amount.from` 四项 ⇒ 传了即造出官方不认的载荷。
- **结果通知口径不同**：本页官方写「需要通过退款查询接口**或退款结果通知**获取结果」。

### 53. `QueryIndividualRefundByRefundIdAsync` — 「查单 · 个人收款方身份，按微信退款单号」。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012650253。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`GET /v3/ecommerce/refunds/id/{refundId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `refundId` | Path | `string` | 微信支付退款单号（官方 path `refund_id`，必填 string(32)）。 |
| `individualAuthId` | Query `individual_auth_id` | `string` | 受理授权 ID（官方 query `individual_auth_id`，**必填** string(32)）：身份定位键，取代二级商户支的 `sub_mchid`。 |
- **返回**：`EcommerceIndividualRefundQueryResponse` — 退款单全量（21 行），见 `EcommerceIndividualRefundQueryResponse`（与 `QueryIndividualRefundByOutRefundNoAsync`、`ApplyAbnormalRefundAsync`**三支共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/refunds/id/{refund_id}?individual_auth_id=`。
- **与二级商户支查单应答的差集恰五项**：`amount.from`（含子表两行）、`amount.advance`、 / `refund_account` ⇒ 个人收款方**无垫付退款概念**，故不建「一个超集类型」。
- **本身份支的 `ABNORMAL` 指向异常退款 API**（`ApplyAbnormalRefundAsync`）， / 与二级商户支「商户平台人工处理」的处置路径不同。

### 54. `QueryIndividualRefundByOutRefundNoAsync` — 「查单 · 个人收款方身份，按商户退款单号」：与 `QueryIndividualRefundByRefundIdAsync` 同一应答类型，只是定位键不同。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012650314。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`GET /v3/ecommerce/refunds/out-refund-no/{outRefundNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRefundNo` | Path | `string` | 商户退款单号（官方 path `out_refund_no`，必填 string(64)）。 |
| `individualAuthId` | Query `individual_auth_id` | `string` | 受理授权 ID（官方 query `individual_auth_id`，**必填** string(32)）。 |
- **返回**：`EcommerceIndividualRefundQueryResponse` — 与按 `refund_id` 查单**共用同一应答类型**，见 `EcommerceIndividualRefundQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/ecommerce/refunds/out-refund-no/{out_refund_no}?individual_auth_id=`。
- **三页应答表逐行相同**（2026-10-15 比对，21 行，含对 `ABNORMAL` 的处置指引文本亦一致）⇒ 共用一个类型。

### 55. `ApplyAbnormalRefundAsync` — 「申请异常退款」：对查单/通知确认为 `ABNORMAL`（原路退银行卡失败）的个人收款方退款单，改退至用户银行账户。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012649274。

- **声明接口**：[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs)
- **官方契约**：`POST /v3/ecommerce/refunds/{refundId}/apply-abnormal-refund`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `refundId` | Path | `string` | 微信支付退款单号（官方 path `refund_id`，必填 string(32)）：待处理的异常退款单。 |

- **请求体**：`EcommerceAbnormalRefundRequest` — 异常退款请求，见 `EcommerceAbnormalRefundRequest`（`bank_account`、`real_name` 为**密文**）。
- **返回**：`EcommerceIndividualRefundQueryResponse` — 处理后的退款单，见 `EcommerceIndividualRefundQueryResponse`（与个人收款方查单两支**共用同一应答类型**）。

**备注（原文）**

- **官方契约**：**POST** `/v3/ecommerce/refunds/{refund_id}/apply-abnormal-refund`；限频 150 QPS。
- **必须带 `Wechatpay-Serial`**：本方法是**本域唯一**需要该头的端点， / `bank_account` 与 `real_name` 须以微信支付公钥（推荐）或平台证书公钥加密； / 两个字段属收款人银行账号与真实姓名，禁止进日志、遥测或异常消息。
- **前置状态是 `ABNORMAL`**：官方原文「查询退款或退款结果通知确认状态为退款异常，可调用此接口」⇒ / 对 `PROCESSING`/`SUCCESS` 单调用属误用。
- **只支持退款至用户银行卡**（`type` 唯一取值 `USER_BANK_CARD`），且仅支持 15 家银行的**借记卡**； / `bank_type` 官方标选填、退款至用户时**必填**（条件必填）。
- **与服务商面的同名端点不是同一契约**：`/v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund` / （https://pay.weixin.qq.com/doc/v3/partner/4013080627）属普通服务商，已由 / `IWechatPayRefundService` 承载 ⇒ 路由不同、字段不同，不可互相替代。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/Ecommerce`](../../../Mud.Wechat.Pay/Ecommerce/) 内共 **104** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **104** 个，`SerializerClassName` = `Ecommerce`。

AOT 源生成上下文：[`EcommerceJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/EcommerceJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `EcommerceCancelWithdrawApplyResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceCancelWithdrawApplyRequest` | 请求体 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceCancelWithdrawQueryResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceCancelWithdrawConfirmResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceCancelWithdrawConfirmRequest` | 请求体 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceValidateCancelResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceCancelApplicationResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceCancelApplicationRequest` | 请求体 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceIndividualContractByOpenIdResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceIndividualContractResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceSubjectSubMerchantsResponse` | 应答 | [Ecommerce/EcommerceAccountDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceAccountDtos.cs) | `sub_mchid`, `out_request_no`, `withdraw`, `payee_info`, `proof_medias`, `additional_materials`, `remark`, `account_type`, `bank_account_info`, `identity_info`, `account_name`, `account_bank` … |
| `EcommerceApplymentSubmitResponse` | 应答 | [Ecommerce/EcommerceApplymentDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceApplymentDtos.cs) | `out_request_no`, `organization_type`, `finance_institution`, `business_license_info`, `finance_institution_info`, `id_holder_type`, `id_doc_type`, `authorize_letter_copy`, `id_card_info`, `id_doc_info`, `owner`, `ubo_info_list` … |
| `EcommerceApplymentRequest` | 请求体 | [Ecommerce/EcommerceApplymentDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceApplymentDtos.cs) | `out_request_no`, `organization_type`, `finance_institution`, `business_license_info`, `finance_institution_info`, `id_holder_type`, `id_doc_type`, `authorize_letter_copy`, `id_card_info`, `id_doc_info`, `owner`, `ubo_info_list` … |
| `EcommerceApplymentQueryResponse` | 应答 | [Ecommerce/EcommerceApplymentDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceApplymentDtos.cs) | `out_request_no`, `organization_type`, `finance_institution`, `business_license_info`, `finance_institution_info`, `id_holder_type`, `id_doc_type`, `authorize_letter_copy`, `id_card_info`, `id_doc_info`, `owner`, `ubo_info_list` … |
| `EcommerceAccountHostingApplymentRequest` | 请求体 | [Ecommerce/EcommerceApplymentDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceApplymentDtos.cs) | `out_request_no`, `organization_type`, `finance_institution`, `business_license_info`, `finance_institution_info`, `id_holder_type`, `id_doc_type`, `authorize_letter_copy`, `id_card_info`, `id_doc_info`, `owner`, `ubo_info_list` … |
| `EcommerceSignVerifyInfoResponse` | 应答 | [Ecommerce/EcommerceApplymentDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceApplymentDtos.cs) | `out_request_no`, `organization_type`, `finance_institution`, `business_license_info`, `finance_institution_info`, `id_holder_type`, `id_doc_type`, `authorize_letter_copy`, `id_card_info`, `id_doc_info`, `owner`, `ubo_info_list` … |
| `EcommerceSignVerifyInfoRequest` | 请求体 | [Ecommerce/EcommerceApplymentDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceApplymentDtos.cs) | `out_request_no`, `organization_type`, `finance_institution`, `business_license_info`, `finance_institution_info`, `id_holder_type`, `id_doc_type`, `authorize_letter_copy`, `id_card_info`, `id_doc_info`, `owner`, `ubo_info_list` … |
| `EcommerceFundFlowBillResponse` | 应答 | [Ecommerce/EcommerceBillDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceBillDtos.cs) | `download_bill_count`, `download_bill_list`, `bill_sequence`, `hash_type`, `hash_value`, `download_url`, `encrypt_key`, `nonce` |
| `EcommerceBalanceResponse` | 应答 | [Ecommerce/EcommerceFundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceFundDtos.cs) | `sub_mchid`, `available_amount`, `pending_amount`, `account_type`, `out_request_no`, `amount`, `remark`, `bank_memo`, `notify_url`, `withdraw_id`, `sp_mchid`, `status` … |
| `EcommerceWithdrawApplyResponse` | 应答 | [Ecommerce/EcommerceFundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceFundDtos.cs) | `sub_mchid`, `available_amount`, `pending_amount`, `account_type`, `out_request_no`, `amount`, `remark`, `bank_memo`, `notify_url`, `withdraw_id`, `sp_mchid`, `status` … |
| `EcommerceWithdrawRequest` | 请求体 | [Ecommerce/EcommerceFundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceFundDtos.cs) | `sub_mchid`, `available_amount`, `pending_amount`, `account_type`, `out_request_no`, `amount`, `remark`, `bank_memo`, `notify_url`, `withdraw_id`, `sp_mchid`, `status` … |
| `EcommerceWithdrawQueryResponse` | 应答 | [Ecommerce/EcommerceFundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceFundDtos.cs) | `sub_mchid`, `available_amount`, `pending_amount`, `account_type`, `out_request_no`, `amount`, `remark`, `bank_memo`, `notify_url`, `withdraw_id`, `sp_mchid`, `status` … |
| `EcommerceTransferAuthorizationResponse` | 应答 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommerceTransferAuthorizationRequest` | 请求体 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommerceTransferBillCreateResponse` | 应答 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommerceTransferBillCreateRequest` | 请求体 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommerceMchTransferBillQueryResponse` | 应答 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommerceTransferBillCancelResponse` | 应答 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommerceTransferBillCancelRequest` | 请求体 | [Ecommerce/EcommerceMchTransferDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceMchTransferDtos.cs) | `sub_mchid`, `agreement_list`, `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `authorization_state`, `accept_time`, `authorize_time`, `appid`, `out_bill_no`, `transfer_scene_id` … |
| `EcommercePapayPreEntrustSignResponse` | 应答 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayPreEntrustSignRequest` | 请求体 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayContractQueryResponse` | 应答 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayContractTerminateResponse` | 应答 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayContractTerminateRequest` | 请求体 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayCombineTransactionResponse` | 应答 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayCombineTransactionRequest` | 请求体 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommercePapayReverseRequest` | 请求体 | [Ecommerce/EcommercePapayDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommercePapayDtos.cs) | `appid`, `plan_id`, `out_contract_code`, `contract_display_account`, `notify_url`, `return_app`, `attach`, `pre_entrustweb_id`, `mchid`, `miniprogram_username`, `miniprogram_path`, `contract_id` … |
| `EcommerceProfitSharingOrderResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceProfitSharingOrderRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceUnsplitAmountResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceProfitSharingFinishResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceProfitSharingFinishRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceReceiverResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceAddReceiverRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceDeleteReceiverRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceProfitSharingReturnResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceProfitSharingReturnRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceCreateSubsidyResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceCreateSubsidyRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceReturnSubsidyResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceReturnSubsidyRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceCancelSubsidyResponse` | 应答 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceCancelSubsidyRequest` | 请求体 | [Ecommerce/EcommerceProfitSharingDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceProfitSharingDtos.cs) | `appid`, `sub_mchid`, `transaction_id`, `out_order_no`, `receivers`, `finish`, `type`, `receiver_account`, `amount`, `description`, `receiver_name`, `order_id` … |
| `EcommerceRefundApplyResponse` | 应答 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceRefundApplyRequest` | 请求体 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceRefundQueryResponse` | 应答 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceReturnAdvanceResponse` | 应答 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceReturnAdvanceRequest` | 请求体 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceIndividualRefundApplyResponse` | 应答 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceIndividualRefundApplyRequest` | 请求体 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceIndividualRefundQueryResponse` | 应答 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |
| `EcommerceAbnormalRefundRequest` | 请求体 | [Ecommerce/EcommerceRefundDtos.cs](../../../Mud.Wechat.Pay/Ecommerce/EcommerceRefundDtos.cs) | `sub_mchid`, `sp_appid`, `sub_appid`, `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `amount`, `notify_url`, `refund_account`, `funds_account`, `refund` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Ecommerce;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Ecommerce));

var service = app.Services.GetRequiredService<IWechatPayEcommerceProfitSharingService>();
var ct = CancellationToken.None;
var response = await service.AddReceiverAsync(new EcommerceAddReceiverRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/ecommerce/profitsharing/receivers/add");
// 占位值仅用于编译示意；业务值须按 IWechatPayEcommerceProfitSharingService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddReceiverAsync`）；请求体 `EcommerceAddReceiverRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayEcommerceAccountService](./IWechatPayEcommerceAccountService.cs)、[IWechatPayEcommerceApplymentService](./IWechatPayEcommerceApplymentService.cs)、[IWechatPayEcommerceBillService](./IWechatPayEcommerceBillService.cs)、[IWechatPayEcommerceFundService](./IWechatPayEcommerceFundService.cs)、[IWechatPayEcommerceMchTransferService](./IWechatPayEcommerceMchTransferService.cs)、[IWechatPayEcommercePapayService](./IWechatPayEcommercePapayService.cs)、[IWechatPayEcommerceProfitSharingService](./IWechatPayEcommerceProfitSharingService.cs)、[IWechatPayEcommerceRefundService](./IWechatPayEcommerceRefundService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayEcommerceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEcommerceContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPlatsolutionEcommerceCompensateContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceCompensateContractGuards.cs)、[`WechatPayPlatsolutionEcommerceDepositContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceDepositContractGuards.cs) |
| 守卫中的代表性测试方法 | `EcommerceTopology_ShouldBeEightInterfacesOnOneGroupWithFiftyFiveMethods`、`EcommerceMerchantIdentityFaces_ShouldStayFivePartnerEndpointsWithDualIdentityChannel`、`EcommerceDataModels_ShouldBeFullyRegisteredInJsonContextAndWiredIntoDi`、`EcommerceDtos_ShouldFollowLineWideShapeRedLines`、`EcommerceFundEndpoints_ShouldMatchOfficialRoutesAndShareResponseTypes`、`EcommerceBill_ShouldDeclareOneGetWithFourRequiredQueries`、`EcommerceApplymentRoutes_ShouldKeepTrailingSlashAndFollowSharedResponseTypeRulings`、`EcommerceAccountFaces_ShouldKeepTwoCancelContractsAndQuerySideCiphertext`、`EcommerceProfitSharingRoutes_ShouldKeepSameRoutePairsAndStatelessFinishResponse`、`EcommerceSubsidyFaces_ShouldBeTheOnlyThreePartnerIdentityEndpointsInProfitSharing`、`EcommerceRefundRoutes_ShouldKeepSameRouteTwinsAndNoAfterSalesFace`、`EcommerceMchTransfer_ShouldKeepThreeValueAuthorizationStateDistinctFromTwoValueAuthState` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/Ecommerce`](../../../Mud.Wechat.Pay/Ecommerce/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayEcommerceContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayPlatsolutionEcommerceCompensateContractGuards.cs`、`WechatPayPlatsolutionEcommerceDepositContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
