# Mud.Wechat.Pay

微信支付 **APIv3** SDK **主包**：84 个业务域 91 个接口 593 端点的声明式客户端、模块注册器（`PayModule`，86 个成员）、十类非声明式服务（小程序调起签名 / 账单下载 / 发票文件上传 / 出租车电子发票文件上传 / 平台证书刷新 / 商户媒体上传 / 管理记录文件上传 / 投诉图片上传与拉取 / 注销资料图片上传 / 营销图片上传）、AOT JsonContext 合并。

> **与企业微信线是两条不同产品线**：本包凭据是**商户 API 证书 RSA 私钥的请求签名**（`Authorization: WECHATPAY2-SHA256-RSA2048 …`），**没有 `access_token`**、四个 `Mud.Wechat.Pay*` 包**零 `[Token]` 声明**（守卫 `PAY-B1` fail-closed），BaseAddress 恒为 `https://api.mch.weixin.qq.com`。
> 企业微信的「企业支付」`AddPayApi()` 与「收银台」`AddPayToolApi()`（`Src/Work/`，走企微 `access_token`）与本包**无关**；本包入口名为 `AddPayApp()`（凭据底座）与 `AddWechatPayApi()`（业务接口）。

## 内容

- **声明式业务客户端**（`Interfaces/`，89 个接口 / 562 端点，全部落 `namespace Mud.Wechat.Pay`，实现类由 `Mud.HttpUtils.Generator` 产出到 `Mud.Wechat.Pay.Internal`；该分区由守卫 `WechatPayInterfaceNamespaceContractGuards`（PAY-NS1~NS3）锁定，**注意本线口径与企微线 N1~N3 不同**：支付线无「公共父接口 `IsAbstract`」分面，且手工通道服务另落 `Mud.Wechat.Pay.{域}`（如 `Mud.Wechat.Pay.MerchantManage.IWechatPayMerchantManageFileService`），不在该守卫口径内）：
  - `Interfaces/Transactions/` `IWechatPayTransactionsService` — 基础交易 7 端点：JSAPI/小程序、Native、APP、H5 四族下单 + 微信订单号查单 + 商户订单号查单 + 关闭订单
  - `Interfaces/Refund/` `IWechatPayRefundService` — 退款 3 端点：申请退款、查询单笔退款、发起异常退款
  - `Interfaces/Bill/` `IWechatPayBillService` — 账单 2 端点：申请交易账单、申请资金账单（文件另走下载通道）
  - `Interfaces/Certificates/` `IWechatPayCertificatesService` — 平台证书 1 端点：获取平台证书列表（应答/回调验签与序列号轮换）
  - `Interfaces/ProfitSharing/` `IWechatPayProfitSharingService` — 分账 9 端点：接收方添加/删除、请求分账 + 查询、回退单 + 查询、解冻、待分金额查询、分账账单
  - `Interfaces/PayScore/` `IWechatPayPayScoreService` — 支付分 11 端点：服务订单创建/查询/取消/完结/修改/收款/同步 + 授权预授权/解除/按授权码或 openid 查询
  - `Interfaces/Combine/` `IWechatPayCombineService` — 合单支付 6 端点：JSAPI/Native/APP/H5 下单 + 合单查询 + 合单关单（两面路由共用，域 DTO 以服务商面建模）
  - `Interfaces/Transfer/` `IWechatPayTransferService` — 商家转账 10 端点（含「不换单重试」所需的查单/撤销/电子回单；P1-2-A 增量四支：电子回单**申请**两支 `POST /v3/fund-app/mch-transfer/elecsign/{out-bill-no|transfer-bill-no}`（与「查询」两支同族不同动词）+ `pre-transfer-with-authorization`（授权预下单）+ `transfer`（凭授权扣款））
  - `Interfaces/Fapiao/` `IWechatPayFapiaoService` — 电子发票 **18 端点**：商户面 5（开具、查询、冲红、获取下载信息、插卡）+ **P1-5-B 服务商 / 数电面 13**（卡券模板、通用行业与不动产租赁开具、能力邀请两支、子商户功能状态两支、商户开票基础信息、开发选项查询与配置、税收分类编码、用户抬头信息与填写链接）
  - `Interfaces/MarketingFavor/` `IWechatPayMarketingFavorService` — 代金券 15 端点（P2 首批 7 + P1-3 增量 8）：批次创建/启动/暂停/续期/查询 + 发券 + 券详情 + 回调地址两支（POST 设置 / GET 查询）+ 批次列表 + 可用单品 / 可用商户 / 退款明细 / 核销明细 + 条件查询用户券（**创建成功 ≠ 可发放**，须先启动批次；**批次列表支的 `offset`/`limit`/`stock_creator_mchid` 官方必填**；**退款与核销明细两支应答表逐字相同但是两条独立路由** ⇒ 共用 DTO、分建方法；**用户券两支的 path 占位符官方一页写 `{openid}` 一页写 `{openId}`**，照页不「统一」。守卫 MF1 / MF5 / MF6）
  - `Interfaces/Apply4SubMerchant/` `IWechatPayApply4SubMerchantService` — 特约商户进件（**服务商面**）6 端点：提交申请单 + 申请单号/业务编号两路查状态 + 结算账户修改/查询/修改申请状态查询（**提交 ≠ 通过**，两族前缀互为镜像 `applyment4sub` / `apply4sub`）
  - `Interfaces/Apply4Subject/` `IWechatPayApply4SubjectService` — 主体进件 / 商户开户意愿确认（**渠道商 + 从业机构面**）5 端点：提交申请单（POST 带尾斜杠）+ 查询审核结果（**同路径 GET 无尾斜杠、定位键走 query**）+ 业务编号/申请单号两个撤销变体（204 无包体）+ 按 `sub_mchid` 查授权状态（**与 `apply4sub` 是两套契约：状态与主体类型取值不重合、`licence` 为英式拼写、应答无 `sign_url` 而是 base64 `qrcode_data`**）
  - `Interfaces/PartnerTransactions/` `IWechatPayPartnerTransactionsService` — 服务商基础支付 + 分期（**服务商面**）9 端点：JSAPI/小程序、Native、APP、H5、扫码支付五族下单 + 微信订单号/商户订单号双路查单（**`sp_mchid` + `sub_mchid` 走 query**）+ 关闭订单（204 无包体）+ 分期曝光咨询（**唯一不在 `/v3/pay/partner/transactions/` 前缀内**，走 `/v3/pay/instal/partner/exposure`，200 次/秒）。**与直连 `Transactions` 是两套契约**：商户标识四分（`sp_`/`sub_` 前缀）、`payer` 只有 JSAPI 与扫码支付两族有且双 openid 二选一、`support_fapiao` 唯扫码支付在顶层而其余四族在 `settle_info` 内、三套 `scene_info` 与两套 `store_info` 各自成类型；官方 `codepay`/`reverse` 两页现行正文是 APIv2 指引（无 V3 字段表）、`transactions/exposure` 页面已下架 ⇒ **SKIT 清单 12 行实落 9 端点**
  - `Interfaces/PartnerPayScore/` `IWechatPayPartnerPayScoreService` — 服务商支付分（**服务商面**）23 端点：用户授权 7（商户预授权、开启场景服务权限、评估用户分层、按协议号/OpenID 查授权记录、按协议号/OpenID 解除授权）+ 服务订单 8（创建/查询/取消/完结/修改 + 待处理消息/预扣费消息/状态同步）+ 服务额度 1 + 支付分计划 3 + 计划型服务订单 1 + 用户签约计划 3。**与本线其余服务商域形态相反**：商户标识是 `appid`（服务商）+ `sub_appid` + `mchid`/`sub_mchid`，**全域无 `sp_appid`/`sp_mchid`**，照抄 `PartnerJsapiPrepayRequest` 会静默错参；同一业务域内三套路由前缀并存（`/v3/payscore/partner/` 16、`/v3/payscore/plan/partner/` 3、`/v3/payscore/sign-plan/partner/` 4）；`sub_mchid` 位置逐端点分列 query（6 支 GET + 评估用户分层）与 body，不能一套模板套全族；两套时间格式并存（紧凑 `yyyyMMddHHmmss` 与 rfc3339）⇒ 时间字段一律 `string`；官方无「商户收款」端点（SKIT 清单 28 行实落 23）
  - `Interfaces/BankPayScore/` `IWechatPayBankPayScoreService` — 银行 / 从业机构支付分（**从业机构（支付机构）面**）11 端点：授权 3（预授权、按协议号查授权记录、按协议号解除授权）+ 服务订单 6（创建/查询/取消/完结/修改/同步）+ 扣款信息 2（登记扣款信息、查询扣款信息）。**与相邻两支付分域形态都不同**：① 商户标识在 `appid`/`mchid` + `sub_appid`/`sub_mchid` 之上**多一层 `channel_id`（渠道商商户号）**，且该字段在请求侧**处处必填**（GET 走 query、POST 走 body），在应答侧**可降为选填**（预授权应答即选填）——勿按「请求必填则应答也必填」推断；② **只开放免确认模式**（报文无 `need_user_confirm`），官方无服务商面的支付分计划 / 用户签约计划族 ⇒ 单一前缀 `/v3/payscore/acquiringbank/`；③ 授权失效码本域十一页统一 `NO_AUTH`，与服务商面 `NOAUTH` 拼写**相反**，照各自页原文不「统一」；④ 两套紧凑时间格式并存且 `start_time` 额外接受关键字 `OnAccept` ⇒ 时间字段一律 `string`；⑤ 五支端点官方成功应答为 204 ⇒ 返回裸 `Task`；SKIT 清单 15 行实落 11——「收单曝光」出处页已下架（现跳 50x）⇒ 零端点且 `Interfaces/BankAcquiring/` **不建目录**（守卫 BS-B2 以正对照锁定）
  - `Interfaces/PartnerTransfer/` `IWechatPayPartnerTransferService` — 服务商批量转账到零钱（**普通服务商面**）5 端点：发起批量转账 + 批次查询两支（微信批次号 / 商家批次号）+ 明细查询两支（微信明细号 / 商家明细号）。**与直连 `Transfer` 是两套契约**：本域走 `/v3/partner-transfer/batches`（批次形态），直连面现行落位是**新版商家转账** `/v3/fund-app/mch-transfer/transfer-bills`（按笔发起、无批次）⇒ 路由与 DTO 均不互通；**批次单号一经发起不可更换**（一旦发起不允许撤销、报错不要换单重试，换单重提即重复出资）；发起只回 `batch_id` + 受理时间，结果须轮询且**只可查近 30 天**（「查不到」≠「失败」）；计数与状态字段名是 `total_num`/`success_num`/`fail_num` 与 `batch_status`/`detail_status`（银行面为 `*_count` 与 `*_state`）；明细请求 `user_name`、应答 `username`（两处不同名且皆密文）；批次上限一千笔、发起与查单各 50QPS
  - `Interfaces/BankTransfer/` `IWechatPayBankTransferService` — 从业机构（银行）批量转账与电子回单（**从业机构面**）9 端点：转账 5（形态同服务商面）+ 电子回单 4（汇总/明细各「受理 POST + 查询 GET」）。**两套前缀同域共存**（转账 `/v3/bank-transfer/batches`、回单 `/v3/bank-batch-transfer/receipt/`），且回单两支 POST 与 GET **路径完全相同** ⇒ **端点计数按 (方法, 路由) 二元组**，9 支去重后是 7 条路由（守卫 BT-B1 双断言）；商户标识是**六层**（`platform_mchid` / `specialized_payment_mchid` / `sponsor_mchid` / `user_authorized_mchid` / `user_authorized_appid` + 应答 `bank_sp_mchid`），全为报文参数而非作用域；字段名同源不同形（`total_count` / `batch_state` 4 值 / `detail_state` 4 值，本域**无** `WAIT_PAY`/`INIT` 档）；回单生成**异步**（受理只回 `receipt_state = ACCEPTED`，`download_url` 仅 `FINISHED` 出现且**不进生成式接口**，同账单下载通道形态）；批次上限三千笔
  - `Interfaces/Repayment/` `IWechatPayRepaymentService` — 平台收付通·还款支付（**平台商户形态，仅借「商户号 + 私钥」承载面**）3 端点：还款下单 + 还款单查询 + 商户关单（204 无包体 ⇒ 裸 `Task`）。**四处照抄即错**：子单 `mchid` 官方要求填**还款发起方（服务商）商户号** `combine_mchid`（与合单支付「子单 mchid = 各参与商户」语义相反）；AppID 与场景信息**嵌在 `h5_info`/`app_info`/`miniprogram_info`/`wechat_web_info` 内**（四支至少传一个，顶层没有 AppID）；字段名含数字打头段 `h5_info`/`h5_url`；`combine_payer_info` 请求侧是实名信息（`identity` 三节、密文）而应答侧只有 `openid` ⇒ 分建两型。端上调起四支与支付成功回调、还款退款**均属其他面** ⇒ 本域零端点（守卫 RP-B2 带正对照）
  - `Interfaces/MerchantRiskManage/` `IWechatPayMerchantRiskManageService` — 商户风险管理（**普通商户 / 服务商 / 从业机构与渠道面并存**）11 端点 / 9 条 `(方法, 路由)`：风险订单同步两形态各「查询 + 回传处置结果」（`query-risk-trades` 与 `recv-dispose-result` **服务商面与普通商户面同方法同路由、字段表不同** ⇒ 分建两型，按方法计数）+ 风险共建 3（上报订单关联信息、查询风险信息、处理结果回传）+ 违规通知回调地址四件套（同一条 `/violation-notifications` 上 POST/PUT/GET 回 200+JSON 而 **DELETE 回 204**）。**四套商户标识并存**（`mchid` / `sp_mchid`+`sub_mchid` / `sp_mchid`+`acquiring_bank_id`+`channel_id`+`sub_mchid` / `sub_orders[].mch_id`——官方此处**带下划线**）；同名 `risk_type` 有两套类型（订单同步面 `integer`、风险共建面 `string` 5 值）⇒ 常量与 DTO 各一套；官方两处误拼 `risk_discove_time` / `same_deviced` **照页保留**（守卫 MRB-B3 同时断言「不得写成正确拼写」）
  - `Interfaces/MerchantStore/` `IWechatPayMerchantStoreService` — 商户门店管理 5 端点：创建（POST）+ 更新（PATCH，**204**）+ 查询（GET）+ 绑定收款号 + 解绑收款号（POST 带动词段 `…/recipients/bind`、`…/recipients/unbind`，回 JSON）。**官方十页归并为五端点**：商户面与服务商面的字段表只差一个 `sub_mchid`、应答表零差异 ⇒ **无 `Partner` 分身类型**（守卫 MST-B6）；更新请求**只含 `store_basics`/`store_address`/`store_business` 三棵子树、无 `store_recipient`**；`store_id` 官方 `integer`（模型 `long?`、path 参数按线内惯例是 `string`）、经纬度官方即 **string**；绑定与解绑**请求分建两型、应答共用一型**（逐项回 `failed_reason`）。分页查门店与删除门店官方无 HTTP 面 ⇒ 零声明
  - `Interfaces/MerchantAlterApply/` `IWechatPayMerchantAlterApplyService` — 商户资料变更（**服务商面**）12 端点：主体 / 超级管理员 / 商家简称三族各「提交申请单 + 按 `apply_id` 查 + 按「商户号 + `out_request_no`」查 + 撤销」。全部落 `/v3/mchalterapply/` 单一前缀；**撤销是 POST 动词段 `/{applyId}/revoke` 而非 DELETE**，且官方无「按业务单号撤销」⇒ 撤销支恰三支；本域**零 204**。三族**定位键参数名不同形**（主体/超管族用 `merchant/{merchantCode}`，简称族用 `merchant/{subMchId}`）、状态值集不同族（主体族 6 值含 `EDITTING`、超管族 5 值**无**该值），故三族应答各建一型；加密要求三支相异——主体族与超管族提交把 `Wechatpay-Serial` 列为请求头表行必填，简称族**无该表行且全明文**；官方拼写 `APPLYMENT_STATE_EDITTING`（三 T）/ `MODIFING`（单 F）照页保留。**清单原始错配已收口**：联系人族四行在清单里挂的是主体族四页 docId（那四页正文只文档 `mchsubjectalterapplyment`）⇒ 守卫逐对钉住「方法 ↔ docId」（MAA-B2）
  - `Interfaces/MerchantFund/` `IWechatPayMerchantFundService` — 商户账户余额 2 端点：实时余额 + 日终余额（`/v3/merchant/fund/balance/{accountType}`、`/v3/merchant/fund/dayendbalance/{accountType}`，均 GET、`date` 走 query、两支**共用同一应答类型**且只有 `available_amount`/`pending_amount` 两字段）。`accountType` 值域只有 `BASIC`/`OPERATION`/`FEES`（**无 `MERCHANT`**）。**同前缀下的提现族四支与所得记录两支属【平台商户】面 ⇒ 本域零端点**，由守卫 MF-B2 与裁决守卫 C1 互为正对照（C1 现记该前缀在册两支）
  - `Interfaces/MerchantSettlement/` `IWechatPayMerchantSettlementService` — 商户结算规则申请（**从业机构面**）2 端点：创建结算规则申请单 + 按 `application_id` 查询。商户标识是 `acquiring_bank_id` + `channel_id` + `sub_mchid` **三层**（本域**无** `sp_mchid`/`sp_appid`/`sub_appid`）；`settle_rule_id` 官方 `integer`（两侧皆 `long?`）；状态三值 `PROCESSING`/`PASSED`/`FAILED`；`application_process_info` 官方是**非结构化 string**（不是数组）⇒ 不得解析成列表。官方无「按商户号查最新申请单」、无规则列表/删除面 ⇒ 零声明
  - `Interfaces/MerchantService/` `IWechatPayMerchantService` — 商户消费者投诉（**普通商户 / 服务商两面并存**）11 端点：通知地址 CRUD 4（四支**共用同一条** `/v3/merchant-service/complaint-notifications`，POST 与 PUT 请求体相同、POST/PUT/GET 应答相同而 **DELETE 回 204** ⇒ 本域是支付线唯一同时含 POST/PUT/GET/DELETE 四动词的域）+ 投诉列表 / 详情 / 协商历史 3 + 回复 / 完成处理 / 退款审批 3（三支 **204 无包体** ⇒ 裸 `Task`，状态闭环只能回查 `complaint_state`）+ 即时服务 1（200 + `log_id`）。图片上传与拉取两支走手工通道（multipart + 二进制流，见 `IWechatPayComplaintMediaService`）⇒ **不计入生成式接口**
  - `Interfaces/MerchantManage/` `IWechatPayMerchantManageService` — 商户管理记录（**普通服务商面**）3 端点：查询单条管理记录 + 提交资料（POST **204**）+ 按 `sub_mchid` 分页查询列表（`limit` 必填、上限 10）。`submit_data` 官方类型是 `string(4000000)`（**JSON 序列化后的字符串、不是对象**）⇒ 建成子树即与官方字段类型冲突；上传管理记录文件走 multipart 手工通道，其表单字段名是 `submission_file` + `meta`（**与媒体上传通道的 `file` + `meta` 不同名**）
  - `Interfaces/MerchantOperationManage/` `IWechatPayMerchantOperationManageService` — 商户运营管控情况查询（**普通服务商面**）1 端点：按 `sub_mchid` 查子商户管控情况（`GET /v3/mch-operation-manage/merchant-limitations/sub-mchid/{sub_mchid}`）。**同前缀的尽调族三支标【平台商户】** ⇒ 本包零声明（守卫 MOM-B2，正对照取本端点）
  - `Interfaces/Ecommerce/` **八个接口一个注册组**（`RegistryGroupName = "Ecommerce"` ⇒ 只产出一支 `AddEcommerceWebApiHttpClient()`，全仓唯一「一组多接口」形态）`IWechatPayEcommerce{Fund,Bill,Account,ProfitSharing,Refund,MchTransfer,Applyment,Papay}Service` — 平台收付通 / 电商收付通（**【平台商户】为主，五支【普通服务商】按支分裂**）55 端点 / 52 条 `(方法, 路由)`：账户 10 + 分账与补差 11 + 退款 9 + 进件 7 + 商家转账 6 + 代扣合单 6 + 资金 5 + 账单 1。**八处照抄即错**：① 前缀 `/v3/ecommerce/` 与直连分账 `/v3/profitsharing/`、退款 `/v3/refund/`、商家转账 `/v3/fund-app/mch-transfer/` **零交集**（守卫 EC-X5）；② 退款族三支「同方法同路由两契约」（二级商户面 / 个人收款方面）⇒ 9 方法归并 6 条路由、按方法计端点；③ 进件提交支路由**带尾斜杠** `/v3/ecommerce/applyments/`（全域唯一，守卫 EC-A1）；④ 身份按**支**不按域：补差三支（守卫 EC-P7）+ 注销资格校验 + 注销申请单查询（守卫 EC-X2）签名主体是服务商；⑤ 加密面恰七页，且 `cert_number` 是**密文走 Query**（全线唯一）⇒ 请求 URI 严禁进日志（`WechatPayException.RequestUri` 已剥离 query）；⑥ 「受理 ≠ 结果」在本域是七处而非一处；⑦ 注销并提现（`/account/apply-cancel-withdraw`）与普通注销（`/account/cancel-applications`）是**两套契约**（单据定位键、状态枚举九值 vs 三值、应答表全不同）；⑧ 官方目录内**无** `after-sales-orders` ⇒ 零声明（守卫 EC-R7 带正对照）。注销资料图片上传（multipart，官方页同时标两身份）走手工通道，其 `meta` 键名 `file_name`/`file_digest` 与商户媒体通道的 `filename`/`sha256` **不通用**
  - `Interfaces/MarketingBusifavor/` `IWechatPayMarketingBusifavorService` — 商家券（**普通商户 / 服务商双面**）15 端点：批次创建/更新/预算 + 券核销/退货/关联与解绑补贴 + 自定义 code 上传 + 用户券查询 + 回调地址 + 商品券发券。**批次三写支动词不统一**（创建 POST、改批次与**改预算都是 PATCH**）；**上传自定义 code 是 POST 到 `/stocks/{stock_id}/couponcodes`**；15 支里 **13 支同时有两页且逐页字段表指纹完全同形** ⇒ 一面一方法 + 双页文档引用（分身即造出 15 条官方不存在的契约）；**用户券两支把 `openid` 放在 path**（查询单支还要 path 上的 `coupon_code` + `appid` ⇒ 三元组定位一张券，少任一即 404）。守卫 MBF-B1
  - `Interfaces/MarketingPartnerProductCoupon/` `IWechatPayMarketingPartnerProductCouponService` — 商品券（**全部【普通服务商】**，本批次最大面）33 方法 / 32 条 `(方法, 路由)`：商品券 CRUD + 券批次 + 券包 + 门店关联 + 预算 + 用户券 + 图片生成任务 + 发券与通知回调。**改支统一是 PATCH**；`POST /v3/marketing/partner/product-coupon/product-coupons` 官方拆成「单品形态」与「券包形态」**两页同路由两契约** ⇒ 端点计数按方法、路由计数按 `(方法, 路由)` 去重（守卫 MPPC-B1）；图片上传是 `multipart/form-data` ⇒ 归手工通道，不计入生成式接口（守卫 MPPC-B3）
  - `Interfaces/MarketingPartnerDeliveryPlan/` `IWechatPayMarketingPartnerDeliveryPlanService` — 品牌投放计划（服务商）5 端点：创建 / 更新 / 查询 / 列表 / 终止。**全部路由带品牌标识 path 段**；**终止支官方零 Body 零应答字段** ⇒ 返回裸 `Task`，结果只以 HTTP 码判定（守卫 MDP-B1）；**回调地址支的应答表与请求表同形**，但应答须承载 `code`/`message` ⇒ 独立类型，不复用请求类
  - `Interfaces/MarketingGoodsSubsidyActivity/` `IWechatPayMarketingGoodsSubsidyActivityService` — 商品补贴活动（服务商）8 端点：补贴活动 CRUD + 零售门店活动 + 业代与门店绑定。**两支 DELETE 带 JSON 请求体**（删除须提交 `out_request_no` + `delete_time` + 明细数组）⇒「DELETE 一定无包体」在本域不成立（守卫 MGSA-B1）
  - `Interfaces/MarketingPayGiftActivity/` `IWechatPayMarketingPayGiftActivityService` — 支付有礼活动 8 端点：门槛活动创建 / 列表 / 详情 / 终止 + 商品与商户池维护。商户池三支与商品池一支各自独立路由，`terminate` 是 POST 动词段；**列表支的 `offset`/`limit` 官方必填**（本域唯一）⇒ 缺省即 400（守卫 MPGA-B1）
  - `Interfaces/MarketingShoppingReceipt/` `IWechatPayMarketingShoppingReceiptService` — 小票营销（服务商）5 端点：自定义入口创建（POST，无 path）/ 查询（GET `/{brandId}`）/ 修改（**PATCH** `/{brandId}`）+ 跳转信息上传 + 发送购物小票。**本域无 DELETE**（「CRUD」里的 D 不存在）；**发送购物小票支官方零 Body** ⇒ 只有 `CancellationToken`（守卫 MSRA-B1）
  - `Interfaces/MarketingPartnerships/` `IWechatPayMarketingPartnershipsService` — 商户合作关系 2 端点：建立合作关系 + 查询合作关系列表。**查询支的两个 query 是对象**（`partner` / `authorized_data`），官方 curl 示例逐字显示线格式是 **URL 编码后的 JSON 串** ⇒ 声明式签名以 `string` 承载，宿主负责序列化（守卫 MKP-B1）
  - `Interfaces/IndustryCoupon/` `IWechatPayIndustryCouponService` — 行业券（出行优惠券）1 端点：快捷切卡组件 token 申领。`coupon_list` 的官方字段表以**点号路径**给出（`coupon_list.stock_id`）⇒ C# 侧以独立子类型承载、`[JsonPropertyName]` 落**叶子名**；本域其余能力全在小程序/公众号侧，支付文档中心无 HTTP 面 ⇒ 不得按第三方 SDK 补路由（守卫 MIC-B1）
  - `Interfaces/MarketingBank/` `IWechatPayMarketingBankService` — 银行营销任务 2 端点：包（package）任务创建与查询。**两支官方 Body 表为空** ⇒ 零请求体（造一个 DTO 即上送官方不认的报文）；路由以 `package_id` 定位，查询支 `offset`/`limit` 选填、单次最多 20 条写在描述里而非类型上限 ⇒ 不得本地硬拦（守卫 MBT-B1）
  - `Interfaces/MarketingBankFavor/` `IWechatPayMarketingBankFavorService` — 银行代金券 2 端点：多活动发券 + 按 `coupon_id` 查询银行券。**两支都以 `openid` / `coupon_id` 走 path**；与 `BankPayScore`、`BankTransfer` 的路由面互不交叠
  - `Interfaces/MarketingMemberCardOpen/` `IWechatPayMarketingMemberCardOpenService` — 旧版会员卡模板与用户卡管理（**普通商户**）**25 端点**：模板族 5（创建 / 列表 / 查询 / PATCH 修改 / DELETE 删除）+ 模板升级 1 + code 预存 1 + 用户卡族 8（查询 / PATCH 修改 / 作废 / 确认入会 / OpenID 导入 / 手机号导入 / 二维码创建 / code 解密）+ 关联小程序 1 + 设置管理页入口 1 + 激活码 1。**两套并存契约**：本域 `/v3/marketing/membercard-open/*` 与品牌会员卡 `/v3/brand/partner/card-member/*` 是并存非替代关系。**四支 204 无应答包体**（删除模板 / 升级模板 / 作废卡 code / 激活卡 code）⇒ 返回裸 `Task`；动词不统一：改模板走 PATCH、删模板走 DELETE、激活走 POST ⇒ 照页不得合并；**响应侧 Code 字段遮蔽基类**（业务字段 `code` = 会员卡 code，与 `WechatPayResponse.Code` 冲突）⇒ 5 个响应类型加 `[new]` 显式声明隐藏意图。守卫 MCO-B1~B6
  - `Interfaces/MarketingMemberCardActivity/` `IWechatPayMarketingMemberCardActivityService` — 会员有礼活动（**普通商户**）4 端点：创建活动 + 列表查询 + 详情查询 + 终止活动。**全部路由落 `/v3/marketing/membercard-activity/activities`**，终止支走 POST 动词段 `…/{activity_id}/terminate`；官方四页全部只印【普通商户】一面 ⇒ 一支一方法。守卫 MCA-B1~B5
  - `Interfaces/BrandApplyment/` `IWechatPayBrandApplymentService` — 品牌入驻申请（**普通服务商面**）4 端点：提交申请单 + 按 `applyment_id` / 按 `business_code` 两路查询 + 撤销申请。**两套申请编号**（商户侧 `business_code` 与微信分配的 `applyment_id`）⇒ 查询两支，且两支应答逐字段同形 ⇒ 共用一个响应类型；提交与撤销的应答同为 `applyment_id` + `business_code` ⇒ 亦共用。本域**无 path 上的商户号**，品牌作用域在包体内
  - `Interfaces/BrandCardConfig/` `IWechatPayBrandCardConfigService` — 品牌商家名片配置（**普通服务商面**）5 端点：查询配置 + 提交配置申请 + 撤销申请 + 预览 + 发布。**「申请 → 发布」两段式**：`POST /brand/card/card-configs` 只落审核态，生效须 `…/publish`（其请求体另有 `publish_type` 与条件字段 `scheduled_publish_time`），撤销走 `…/cancel-applyment` ⇒ 三支语义不同、不得合并。**预览支与发布支应答同为 5 行但字段名不同**（预览回 `card_preview_url` / `url_expired_time`，发布回 `publish_type` / `scheduled_publish_time`）⇒ 各建一型（守卫 BCC-B3）
  - `Interfaces/BrandCardLink/` `IWechatPayBrandCardLinkService` — 品牌交易连接名片（**普通服务商面**）5 端点：名片规则列表 + 申请 + 按 `business_code` 查状态 + 撤销 + 场景解除。**`unbind-card-link` 是「解除交易连接场景的挂载」而不是删除规则**，与 `cancel-applyment`（撤销未生效的申请）分属两支 ⇒ 不得折叠；申请受理（6 行）与状态查询（8 行）应答不同形 ⇒ 各建一型；**列表支分页是 `page_index`/`page_size`**（本批唯一该形态的列表支，官方均选填），与会员卡两支的 `offset`/`limit` 不互套（守卫 BCL-B2）
  - `Interfaces/BrandCardMember/` `IWechatPayBrandCardMemberService` — 品牌会员卡与会员（**普通服务商面**）15 端点：模板族 5（创建 / 列表 / 查询 / PATCH 修改 / `invalidate`）+ 预授权 token 1 + 用户卡族 6（查询 / 列表 / PATCH 修改 / `confirm` / `invalidate` / `import-by-openid`）+ 会员动态 1 + 积分两支 2。**模板与用户卡是两套生命周期与两套 ID 体系**（`card_id` ↔ `user_card_code`）⇒ 类型不得互套；写动词不统一（改走 PATCH，作废与确认走 POST 动词段）。**应答共用逐族裁定**（含必填标记的逐表签名比对）：模板四支（创建 / 查询 / 修改 / 作废）49 行同形 ⇒ 共用，列表支 53 行单建；用户卡五支（查询 / 修改 / 确认 / 作废 / 导入）28 行同形 ⇒ 共用，列表支 32 行单建。**请求侧一律分建**：同族各支必填口径不同（守卫 BCM-B4）
  - `Interfaces/BrandProfitSharing/` `IWechatPayBrandProfitSharingService` — 品牌分账（**普通服务商面**）9 端点：最大分账比例查询 + 分账完结 + 发起分账 + 结果查询 + 待分金额查询 + 接收方添加 / 删除 + 回退单发起 / 查询。**与直连 `ProfitSharing` 是同族不同路由**（`/v3/brand/profitsharing/`），且商户口径是 `brand_mchid` + `sub_mchid` ⇒ DTO 与接收方类型不得跨域套用（守卫 BPS-B3）；发起（15 行）与结果查询（16 行，多 `finish_amount` / `finish_description`）**不同形 ⇒ 分建**，回退两支同形 ⇒ 共用，接收方添加与删除同形 ⇒ 共用；结果查询支三个 query 全必填（`sub_mchid` / `transaction_id` / `out_order_no`），回退查询支只两个 ⇒ 定位口径照页不得互套
  - `Interfaces/BrandStore/` `IWechatPayBrandStoreService` — 品牌门店（**普通服务商面**）9 端点：创建 / 删除（DELETE）/ PATCH 更新 / 查询 / 列表 + 收款号绑定与解绑（POST 动词段 `…/bindrecipient`、`…/unbindrecipient`）+ 暂停营业与恢复营业（POST `…/close`、`…/resume`）。**四种「变更」彼此独立**：改信息、删记录、改营业状态、改收款绑定 ⇒ 五支语义不得折叠；创建 / 查询 / 更新三支应答同形（21 行）⇒ 共用，`close` / `resume` 共用 3 行状态应答，绑定 5 行与解绑 1 行（只有 `failed_reason`）**分建**；**删除支官方未印发应表** ⇒ 返回裸 `Task`（补空 DTO 即造契约，守卫 BST-B3）
  - `Interfaces/PasswordExemptContract/` `IWechatPayPasswordExemptContractService` — 免密支付契约（**从业机构（支付机构）/ 普通服务商两面**）6 端点：小程序预签约与支付后预签约**各按面分列**（`/contracts/qs/*` 与 `/contracts/service-provider/*`）+ 协议查询 + 协议解约。**两套标识体系不得互换**：qs 面用 `appid` / `openid`（`string(128)`）、服务商面用 `sp_appid` / `sp_openid`（`string(64)`），且服务商面应答多 `sub_mchid` ⇒ 四支预签约**各建一型**（守卫 PEC-B3）；小程序场景应答比支付后场景多 `sign_mp_appid` / `sign_mp_path` ⇒ 两场景亦不共用。协议查询与解约按 `service_id`（integer）+ `out_contract_code` 两段 path 定位，官方无按 `contract_id` 查询支，解约后同一 `out_contract_code` 不可复用
  - `Interfaces/AbnormalFundProcessing/` `IWechatPayAbnormalFundProcessingService` — 在途异常资金处理（**平台商户 / 普通服务商**）3 端点：付款单查询 `GET /v3/abnormal-fund-processing/receipts/{receipt_id}` + 指令发起 `POST …/transfer-instructions` + 指令执行结果查询 `GET …/transfer-instructions/{out_instruction_no}`。**三支应答不同形 ⇒ 各建一型**；**每支路由官方只印一页**（不像营销面存在【平台商户】/【普通服务商】双页）⇒ 无「同路由多页比对」问题；本域**零 query、零列表支**（三支全部 path 定位）、七页树全部未印端点级 QPS ⇒ 不得凭第三方 SDK 补数（守卫 AFP-B1~B3）
  - `Interfaces/BrandRedpacket/` `IWechatPayBrandRedpacketService` — 品牌商户红包批次（**普通商户**）5 端点：发放 + 批次查询两支（微信批次号 / 商家批次号）+ 明细查询两支（微信明细号 / 商家明细号）。**两套定位键各自成支**，两两应答表逐字段同形（19 行 / 16 行）⇒ 各共用一型；**`brand_id` 在本域是报文字段而非 query 且官方类型是 `integer`**（品牌线他域为 `string(20)` / `string(32)`）⇒ 不得按「品牌标识一律字符串」归一；`need_query_detail` / `detail_state` 官方选填、本域无整型分页列表支（守卫 BRP-B1~B4）
  - `Interfaces/TransferToQqWallet/` `IWechatPayTransferToQqWalletService` — 转账到 QQ 钱包（**普通商户**）3 端点：发起 + 按商户单号查询 + 撤销（POST 动词段 `…/{out_bill_no}/cancel`）。**与普通 `Transfer` 是两套契约**（本域走 `/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/*`、报文多 `qq_wallet_openid`）⇒ DTO 不得互套；**撤销支官方印有 5 行应答表**（`update_time`，非裸 `Task`）且零请求体；三支应答各异（5 / 13 / 5 行）⇒ 各建一型；QPS 三档明印 100 / 200 / 20 次/s（守卫 TQW-B1~B4）
  - `Interfaces/UserConfirmAuthorization/` `IWechatPayUserConfirmAuthorizationService` — 免确认收款授权（**普通商户**）3 端点：发起授权 + 按 `out_authorization_no` 查询 + 解除（`POST …/close`）。**三段式生命周期不可折叠**，三支应答各异（4 / 12 / 11 行，关闭支 = 查询支去掉 `package_info`）⇒ 各建一型；**本域唯一 query 是选填布尔 `is_display_authorization`**、官方无「按微信授权单号关闭」支；`user_name` 官方要求 RSA-OAEP 密文上送 ⇒ 加密由宿主完成（守卫 UCA-B1~B4）
  - `Interfaces/PlatsolutionEcommerceInsuranceContract/` `IWechatPayPlatsolutionEcommerceInsuranceContractService` — 保险理赔功能开通（**平台商户 / 普通服务商**）2 端点：开通 `POST /v3/platsolution/ecommerce/insurance-compensation-contracts` + 状态查询 `POST …/sub-mchid/{sub_mchid}/check-opened`。**状态查询支是 POST 且零请求体**（定位键在 path）⇒ 照页不得改成 GET；两支应答逐字段同形（3 行）⇒ 共用一型，请求侧只有开通支有 Body ⇒ 不共用（守卫 PEC-IC-B1~B4）
  - `Interfaces/PlatsolutionEcommerceCompensate/` `IWechatPayPlatsolutionEcommerceCompensateService` — 平台商户赔付（**平台商户**）5 端点：预下单 + 下单 + 撤销 + 商户单号 / 微信支付单号两路查询。**「预下单 / 下单 / 撤销 / 查询」四态请求各建一型**；预下单应答含 `package_info`、撤销应答含 `cancel_accept_time` + `state` ⇒ 同为 4 行也**不同形**；两路查询应答 18 行同形 ⇒ 共用；撤销支官方零请求体（定位键 `out_bill_no` 全在 path）（守卫 PEC-CP-B1~B4）
  - `Interfaces/PlatsolutionEcommerceInsuranceClaim/` `IWechatPayPlatsolutionEcommerceInsuranceClaimService` — 保险理赔出款（**平台商户**）5 端点：形态同赔付域，但**与赔付域同路由族不同契约**——应答多 `business_type` 等字段、**撤销支官方有请求体**（赔付域无）⇒ 两域 DTO 不得互套；两路查询 19 行同形 ⇒ 共用，其 `sub_mchid` 官方为**选填** query（守卫 PEC-CL-B1~B4）
  - `Interfaces/PlatsolutionEcommerceDeposit/` `IWechatPayPlatsolutionEcommerceDepositService` — 保证金赔付（售后 / 运费 / 保费，**平台商户**）7 端点：一个注册组三笔子族 ⇒ 七支按子族建六个响应类型：售后族两路查询逐字段同形 ⇒ **共用一型**，其余各建一型。**注意「互不同形」只对在途侧成立**：运费与保费两支的下单应答**字段集相同**（官方表格仅行序不同），售后支少一个 `jump_wxa_option` ⇒ 三者**可**共用而不违规，本域仍选择逐子族独立建型（隔离后续演进）**保费族定位键是裸 path 段** `…/deposit-insurance-premiums-compensation-bills/{out_bill_no}`（官方**没有** `out-bill-no/` 前缀，与售后 / 运费两支不同）⇒ 照官方原文不得「顺手对齐」；**保费查询支 `sub_mchid` 必填而售后 / 查询支同参数选填** ⇒ 族内必填口径也不统一（守卫 PEC-DP-B1~B4）
  - `Interfaces/PlatsolutionEcommerceMchTransfer/` `IWechatPayPlatsolutionEcommerceMchTransferService` — 平台商户付款（商户到商户，**平台商户**）3 端点：发起 + 两路查询。**本域收款方是商户**（`receiver_detail.receiver.mch_info`），与赔付 / 理赔 / 保证金三族共用 `/v3/platsolution/ecommerce/mch-transfer/*` 前缀但字段集各异 ⇒ 类型不得跨族复用；按 `bill_id` 支官方给 `sub_mchid` + `business_type` **两个**选填 query、按 `out_bill_no` 支只给 `business_type` ⇒ 逐支声明不得合并（守卫 PEC-MT-B1~B4）
  - `Interfaces/PlatsolutionEcommerceRechargeEmployee/` `IWechatPayPlatsolutionEcommerceRechargeEmployeeService` — 二级商户可扫码充值员工（**平台商户**）3 端点：添加 + 删除（POST 动词段 `…/sp-openid/{spOpenid}/delete`）+ 按 `sub_mchid` 列表。添加与删除应答同形（5 行）⇒ 共用；**列表支与单条支不同形**（同一结构包在 `employees` 数组里，6 行）⇒ 单建一型，不得因「字段名相同」复用（守卫 PEC-RE-B1~B4）
  - `Interfaces/PlatsolutionEcommerceRecharge/` `IWechatPayPlatsolutionEcommerceRechargeService` — 二级商户充值（**平台商户**）3 端点：申请 + 按 `out_recharge_no` 查询 + 关闭（POST 动词段 `…/close`）。查询与关闭应答逐字段同形（17 行，含 `available_recharge_channels` 数组）⇒ 共用；**两支的 `sub_mchid` 官方必填 query** 且零请求体。**另两支「银行转账充值」路由**（`/v3/platsolution/ecommerce/bank-transfer-recharges/*`）按 path 与中文业务名在现行文档中心两路检索**零契约页** ⇒ 零声明，由非契约路由守卫第八条锁定；出处页 404 只作背景、不作判据（旧路径失效 ≠ 契约面消失）（守卫 PEC-RC-B1~B4）
  - `Interfaces/PlatsolutionEcommerceSettle/` `IWechatPayPlatsolutionEcommerceSettleService` — 预付直连结算（**平台商户**）3 端点：申请结算 + 按 `settle_batch_no` 查询 + 异常批次处理（`POST …/{settle_batch_no}/apply-abnormal-settle`）。**三支应答逐字段同形**（14 行）⇒ 共用一个响应类型，而请求侧三支各异（申请 17 行、异常处理 4 行、查询零体）⇒ 各建一型；异常处理支与查询支**同 path 不同动词 / 不同尾段** ⇒ 照页不得合并（守卫 PEC-ST-B1~B4）
  - `Interfaces/PlatsolutionInsuranceReservation/` `IWechatPayPlatsolutionInsuranceReservationService` — 预约商家转账·保险场景（**平台商户**）4 端点：受理预约 + 按 `out_reservation_no` / 按 `reservation_id` 两路查询 + 关闭。**四支建三型**：按商户单号查询、按微信单号查询与关闭三支官方应答表逐字段同形且必填口径一致 ⇒ 共用一型，受理支（含必填 `package_info`）单建。**预约记录 24 小时内有效**、幂等标识 `out_reservation_no` 有效性 5 年 ⇒ 商户单号必须稳定可重放。`user_real_name` 官方要求**加密后传入**（预计转账金额 ≥ 2,000 元时必填）⇒ 请求头须带 `Wechatpay-Serial`，本 SDK 不代填。查询两支与关闭支的 `sub_mchid` 是**必填 query** 而受理支走 Body ⇒ 落位不同不得互套；四支官方页**均未印**端点级 QPS（守卫 PSI-RS-B1~B4）
  - `Interfaces/PlatsolutionInsuranceBatch/` `IWechatPayPlatsolutionInsuranceBatchService` — 商家转账批次与明细·保险场景（**平台商户**）5 端点：受理转账 + 批次两路查询 + 明细两路查询。**五支建四型**：批次两路查询同形（含 `transfer_batch` + `data` 数组 + `offset`/`limit`）⇒ 共用，明细两路查询同形 ⇒ 共用，受理支单建。**受理成功 ≠ 转账成功**（受理只回批次号与 `batch_state`）⇒ 不得把受理应答当终态。分页 query（`need_query_detail` 必填 / `offset` / `limit` / `detail_state`）**只印在批次查询两支**，明细查询零分页；本域字段名用 `batch_id`（不是 `transfer_bill_no`）且多 `sp_mchid` / `sp_appid` / `sub_appid` 三层标识 ⇒ 与 `Transfer` / `TransferToQqWallet` 各域 DTO 不得互套（守卫 PSI-BT-B1~B4）
  - `Interfaces/PlatsolutionInsuranceReceipt/` `IWechatPayPlatsolutionInsuranceReceiptService` — 电子回单（批次汇总 + 明细，**平台商户**）4 端点：汇总回单申请 + 查询、明细回单申请 + 查询。**四支四型、两两不同形**（3 / 7 / 4 / 9 行）⇒ 不得因「字段名大量重合」合并；`hash_type` / `hash_value` / `download_url` **只在查询支**返回。同名 `sub_mchid` 申请支是**请求体唯一字段**（定位键在 path）而查询支是**必填 query** 且零请求体。官方前置条件：**只支持最近 2 年内**的批次单 / 明细单，且回单生成结束后查询支才带下载地址 ⇒ `receipt_state` 非终态时须继续轮询（守卫 PSI-RC-B1~B4）
  - `Interfaces/WebizpayBill/` `IWechatPayWebizpayBillService` — 微企付企业支付账单与出资凭证（**普通服务商**）2 端点：申请出资凭证 + 申请业务账单。**两支四型、两两不同形**（请求 6 / 4 行、应答 11 / 7 行）⇒ 不得因「都返回 `download_url`」合并；两个下载地址官方均**5 分钟内有效** ⇒ 取到即下、不得缓存。账单支另带 `hash_type`（官方固定 `SHA1`）+ `hash_value` 供服务商侧校验文件一致性，凭证支零摘要字段；日期口径也不同：凭证支 `start_date`+`end_date` 区间（限三个月内）+ 选填 `entity_name`（按主体筛单便于开票），账单支只有 `date`（官方「仅支持按一天的纬度查询」）且 `type` 当前**只有 `QYZFALL`**；两支官方页均未印端点级 QPS（守卫 WEB-BL-B1~B4）
  - `Interfaces/WebizpayEmployee/` `IWechatPayWebizpayEmployeeService` — 微企付额度卡与员工授权（**普通服务商**）6 端点：下发额度卡 + 按微信卡号 / 按商户卡号两路查询 + 作废 + 取支付链接 + 解除员工授权。**六支建五型**：两支查询官方应答表 20 行逐字段同形且必填口径一致 ⇒ 共用一型。**定位键分工最易踩**：path 一律 `employee_id`（官方【微信授权关系 ID】`WeBizPay_<uuid>`，**不是**商户企业员工 ID `user_id`），而作废与支付链接两支只认微信卡号 `card_no`、**不接受商户卡号** ⇒ 只有 `out_card_no` 时须先走 `out-quota-cards` 查询支换取。同名 `sp_mchid`/`sub_mchid` 在 GET 三支落**必填 query**、POST 三支落 **Body**；支付链接支另有必填 query `application_type`（`H5`/`MINIPROGRAM`）。解除授权支请求用 `employee_id` 而应答回显 `user_id`，且官方为**同步**语义：返回 `REVOKED` 即该员工在本服务商-出资子商户下授权解除、其额度卡**全部作废**；判「卡可用」须同看 `card_state`（仅 `ACTIVE`/`DISABLED`）与 `effective_end_time`，不得只看 `remain_amount > 0`（守卫 WEB-EP-B1~B4）
  - `Interfaces/WebizpayStore/` `IWechatPayWebizpayStoreService` — 微企付门店主体匹配（**普通服务商**）2 端点：发起匹配 + 按微信支付批次单号查询结果。**本族 12 页中唯一只印 `sp_mchid`、零 `sub_mchid` 的一对** ⇒ 不得按其余十支的「服务商 + 出资子商户」双层形态补字段。**受理 ≠ 结果**：发起支应答只回批次号与 `state`（`ACCEPTED`/`IN_PROGRESS`/`COMPLETED`/`CLOSED`），明细只在查询支的 `match_results` 出现，非 `COMPLETED` 须继续轮询；官方明写「若结果为空，列表长度为 0」⇒ `COMPLETED` + 空数组是合法结果。`match_details` 官方标选填但元素三字段全必填，`product_type` 取值照官方列表取 `CODE_PAY`/`NONE`（正文「仅支持 CODEPAY」为无分隔写法，不做归一）；幂等标识 `out_batch_id`（6-32 字符、同一商户号下唯一）须稳定可重放（守卫 WEB-ST-B1~B4）
  - `Interfaces/WebizpayUser/` `IWechatPayWebizpayUserService` — 微企付员工开通授权（**普通服务商**）2 端点：按商户企业员工 ID 取授权跳转链接 + 查询该员工的微信授权关系列表。**两支是「`user_id` ↔ `employee_id`」互查的两端**：额度卡域各支要的 `employee_id` 须先由授权状态支取得（`employee_list` 官方可为 0 长度 = 该员工从未开通，属正常态）。授权链接支 Body `application_type` 必填，应答中 `authorization_url`/`expire_time`/`mp_query`/`mp_business_type` **全为选填**（官方「仅当可以开通时返回」）⇒ 200 应答零链接不是错误，须按分支处理；小程序两参数有效期 **35 分钟**（比跳转链接更短），其消费面是小程序 JSAPI `wx.openBusinessView`（官方页 4014997783，非 HTTP 端点 ⇒ **零端点**）。微企付另有 **5 支「通知」页**（关系授权完成 4014612130、额度卡失效 4015764389、支付成功 4015764398、退款成功 4015764405、解除授权 4015764414）方向是**微信 → 商户回调地址**、由运营配置 ⇒ 不属商户侧主动调用的 HTTP 契约面，**零端点**（回调落地属支付线回调面另案）（守卫 WEB-US-B1~B4）
  - `Interfaces/Papay/` `IWechatPayPapayService` — 委托代扣 PAPay 3.0（**直连 / 普通商户面**）20 端点：预扣费通知 + 签约查询 / 解除（path 定位键 `plan_id` + `out_contract_code`）+ 预约扣费受理 / 查询（`contract_id`）+ 四端预签约（`pre-entrust-sign/{app,h5,jsapi,mini-program}`）+ 保险自动续费一族（三端预签约、协议查询 / 解除、续期、预约受理、扣款受理、扣费周期列表）。**本域最易照抄即错处**：① **两套协议定位键分属两套**（商户侧协议号 vs 微信协议号），官方**无**按 `contract_id` 查通用协议的支，SDK 不做本地映射；② 四端预签约**请求表字段集互异**（13 / 15 / 13 / 13 行）⇒ 逐端建型，只有应答同形者共用；③ **预扣费通知是扣款前置闸**且正常应答为 **204 无内容** ⇒ 返回裸 `Task`，而 `RESOURCE_ALREADY_EXISTS`（已发送过通知）是**幂等重入的正常态**、不得当失败；④ 续期支同为零包体，且官方明写重试前须先收签解约通知或查扣费周期（盲重试产生重复周期）；⑤ 「受理 ≠ 扣款成功」：扣款与预约两支只回受理结果，扣费结果异步下发；⑥ 三条产品线共存于 `/papay/sign/*`、`/papay/scheduled-deduct-sign/*`、`/papay/insurance-*` 一路由树 ⇒ 类型不得跨线套用；⑦ 本域 20 支官方页**全部未印**端点级 QPS（守卫 PPY-B1~B6）
  - `Interfaces/PartnerPapay/` `IWechatPayPartnerPapayService` — 委托代扣 3.0 预扣费通知（**普通服务商面**）**官方只印 1 支** ⇒ 签约 / 查询 / 扣款各支**无服务商页**，不得按直连面镜像补齐。请求包体是 `sp_mchid` / `sp_appid` / `sub_mchid` / `sub_appid` 四层（直连面为 `mchid` / `appid`）⇒ **不同名即不同型**，请求类型与直连面分建；应答同为「无参数返回 + 204」⇒ 裸 `Task`，`RESOURCE_ALREADY_EXISTS` 亦为正常重入态（守卫 PPY-B1 方法数与 PPY-B2 路由名册）
  - `Interfaces/Vehicle/` `IWechatPayVehicleService` — 车主出行（ETC + 停车）10 端点：ETC 6（按 `contract_id` 查签约状态、按「`openid` + `plan_id` + `plate_number`」查绑定、预开通、高速场景扣款下单、微信订单号 / 商户订单号两路查单）+ 停车 4（入场记录创建、服务查询、按商户订单号查单、扣费受理下单）。**两族开放面不同**：ETC 六支官方【只印服务商页】⇒ 请求以 `sp_mchid` + `sub_mchid` 定位；停车四支是「商户页 + 服务商页」双页同路由 ⇒ **字段取并集、必填只取交集**（一页必填另一页选填即按选填建模，守卫 VEH-B3）。**两支「受理」不是「成功」**：`POST …/transactions/parking` 官方明写异步扣款、`POST …/etc/preopen` 只回 `preopen_id` ⇒ 结果由通知与查询支判定，不得据 HTTP 2xx 判扣款或开通成功；ETC 用户绑定支的 `plan_id` + `plate_number` 是官方必填 query（缺省即 400）；本域十页全部未印端点级 QPS（守卫 VEH-B1~B6）
  - `Interfaces/TaxiInvoice/` `IWechatPayTaxiInvoiceService` — 出租车电子发票 9 端点（**十支里的第十支「上传出租车电子发票文件」是 multipart ⇒ 走手工通道、不计入生成式接口**）：司机签到签退 + 插卡（两支官方 204 无应答包体 ⇒ 裸 `Task`）+ 司机 / 出租车 / 开票公司三对「更新（POST）+ 按证照或车牌或商户号查询（GET）」+ 按 token 查用户打车订单。**十页全部只印【普通服务商】**、`region_id` 是查询族的定位基座（三支必填 query，`find-by-token` 支选填），且官方页示例里 `region_id` 有字符串与数字两种写法 ⇒ 一律照字段表按 `integer` 建模；上传通道的摘要字段名是 **`digest_algorithm`（拼写正常）+ `DIGEST_ALGORITHM_SM3`**，与电子发票域的 `digest_alogrithm` + `SM3` **是两套契约不得互套**（守卫 TXI-B1~B6）
  - `Interfaces/EducationPapay/` `IWechatPayEducationPapayService` — 教育委托代扣（**商户页 + 服务商页双页**）8 端点：解除协议（DELETE）+ 按 `contract_id` 查协议 + 按 `openid` 查协议列表 + 预签约 + 下单受理 + 微信订单号 / 商户订单号两路查单 + 发送预扣费通知。**八支里七支同时印两页 ⇒ 字段取并集、必填只取交集**，但 **`DELETE …/contracts/{contract_id}` 官方只印商户页** ⇒ 该支无服务商镜像，不得按「同域其余支都有双页」补齐（处置表原引的服务商 docId 实为「按用户查协议列表」的服务商页，已逐对核正）；三支官方 204 无应答包体（解除 / 下单 / 预扣费通知）⇒ 裸 `Task`；两路查单应答 33 行同形 ⇒ 共用一型，而「按 `contract_id` 单查协议」与「按 `openid` 查列表」各建一型（**共用只发生在应答根类型**，列表元素另立类型）；预签约只回 `presign_token` ⇒ 调起签约页面的入参而非签约成功；本域八页全部未印端点级 QPS（守卫 EDP-B1~B6）
  - `Interfaces/EducationSchoolPay/` `IWechatPayEducationSchoolPayService` — 校园缴费（教育学费缴纳，**普通服务商**）8 端点：预签约 + 按 `contract_id` 查签约 + 解除协议（POST 动词段 `…/terminate`，**非 DELETE**）+ 按 `openid` 查签约列表 + 缴费下单 + 两路查单 + 用户欠费状态。**八支全部只印服务商页** ⇒ 无商户面镜像，不得按教育代扣域的双页形态补方法；**下单（POST `transactions`）与两支查单应答逐字段同形且必填口径一致 ⇒ 三支共用同一响应类型**（本线唯一「写读共用应答」的一对），而解除协议支 204 ⇒ 裸 `Task`；**下单请求没有 `openid`**，扣款对象由 `contract_id` + `user_id` 定位（应答侧 `payer` 才有 `openid`/`sub_openid`）⇒ 请求与应答定位口径不同不得互套；预签约的四枚密文字段（`identity.real_name` / `identity.id_card_number` / `bank_card.bank_card_no` / `bank_card.phone`）官方逐枚标「按 APIV3 标准加密」且一律 `string(512)`，同支其余字段无加密要求；欠费状态支 `state` 是欠款风控状态（`NORMAL`/`FORBIDDEN`）与订单 `trade_state` 同名不同义；八支未印 QPS，只有 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED`（守卫 ESP-B1~B6）
  - `Interfaces/PayrollCard/` `IWechatPayPayrollCardService` — 代发薪（**普通服务商**）6 端点：申领授权令牌 + 查询用户授权与开通状态 + 凭核身单号预下单取令牌 + 按 `authenticate_number` 查授权单 + 按日期分页查授权单列表 + 发起批量转账。**六支全部只印服务商页**，且**本域没有 `sp_mchid`**（请求侧只有 `sub_mchid`、应答侧服务商商户号叫 `mchid`；AppID 命名两套并存：令牌 / 授权关系 / 认证单三支用 `appid` + `sub_appid`，发起批次支用 `sp_appid` + `sub_appid`）⇒ 按「服务商面必是 `sp_*`」归一即错。**生命周期是两段式**：申领令牌 → 凭令牌预下单 → 发起批次，不可折叠；令牌应答 5 键而预下单应答多 `authenticate_number` ⇒ 各建一型；认证单单查应答与列表 `data[]` 元素 13 键同形但各建一型（共用只在应答根成立）。**必填口径逐页钉死**：`employment_type` 预下单支必填、令牌支选填（官方「未授权必填，授权过非必填」）；分页三键 `total_count` / `offset` / `limit` 在**列表应答**必填而其**请求 query** 的 `offset` / `limit` 选填 ⇒ 两侧不得互套；证件号本域叫 `id_card_number`（进件域是 `identification_no`）。**三支官方请求头表把 `Wechatpay-Serial` 列为必填**（`user_name` / `id_card_number` 密文上送，`string(1024)`）⇒ 涉密是逐页结论、不得按同域补齐，声明式签名不承载该头；发起批次是本线**唯一印端点级 QPS** 的一支（50QPS）；清单两条 `/payroll-card/wesure/*` 判非契约 ⇒ 零声明（守卫 PRC-B1~B6）
  - `Interfaces/BusinessCircle/` `IWechatPayBusinessCircleService` — 商圈会员 / 支付即积分 4 端点：停车状态同步 + 积分同步（两支官方 204 无应答包体 ⇒ 裸 `Task`）+ 积分服务授权查询 + 待积分状态查询。**四支全部同时印商户页与服务商页** ⇒ 字段取并集、必填取交集，但授权查询支的 `sub_mchid` **只在服务商页印发**（商户页根本没有该键）⇒ 一面一方法、不拆分身。定位键三套并存且长度随页变：`brandid` 官方类型是 `integer`（不是字符串）、`sub_mchid` 在停车支是 `string(32)` 而在积分与授权两支是 `string(64)`。积分同步支是**场内退款通知的硬前置**（官方原文「只有接入该接口，才会获取到退款信息」，退款监控窗口是对已同步积分的消费回查 **30 天**）⇒ 未回传积分即没有退款回调。本域**唯一印端点级数字**的是授权查询支（150QPS），其余三支未印。**四支正文均有加密提示、但请求头表只列 `Authorization` / `Accept`（写侧另有 `Content-Type`）** ⇒ 按「涉密口径取请求头表」判为**无涉密必填头**，不得据正文提示补 `Wechatpay-Serial`（守卫 BSC-B1~B6）
  - `Interfaces/Catering/` `IWechatPayCateringService` — 餐饮点餐订单同步 1 端点：`POST /v3/catering/orders/sync-status`。**官方只印服务商页**；「状态变化即上传**全量**菜品明细」（`CREATE_DEAL` / `PAY_SUCCESS`），不是增量；正常应答 **204 无包体** ⇒ 裸 `Task`，页内 `code`/`message`/`detail` 属「异常返回」示例 ⇒ **不建应答类型**。请求表 31 行两层摊平 ⇒ 菜品子树另立类型；`properties` 官方只标 `object` 未给元素结构 ⇒ 以 `JsonElement` 原样承载；`count` 官方类型是 **`Float`** ⇒ `double`；类型列 `String` / `int` 混排照页归一为 `string` / `long`（守卫 CTR-B1~B6）
  - `Interfaces/FundsToOversea/` `IWechatPayFundsToOverseaService` — 出境笔 / 资金出境 6 端点：下单 + 按 `out_order_id` / 「`sub_mchid` + `transaction_id`」两路查单 + 退货单创建 + 按 `out_return_no` 查退货单 + 按 `transaction_id` 查可出境金额 + 申请账单下载链接。**六支全部只印服务商页且身份是【平台商户】**（不是【普通服务商】）。**「受理 ≠ 出境成功」**：下单只回 `result` + `order_id`，汇率与到账金额须回查；账单是 **T+1、上午 10 点**生成、下载链接 **30 秒**有效且须比对 `hash_type` / `hash_value`、**账单金额单位是元**（报文其余金额是分）。两处「写读共用」：下单与按 `out_order_id` 查单、退货创建与按 `out_return_no` 查退货各**共用同一应答根型**；查询支三个定位键（`sub_mchid` + `transaction_id`）都是必填 query，省任一即 400。六支请求头表均未列 `Wechatpay-Serial` ⇒ 本域无涉密必填头（守卫 FTO-B1~B6）
  - `Interfaces/GoldPlan/` `IWechatPayGoldPlanService` — 点金计划（银行贵金属广告）5 端点：自定义页开关 + 计划开关 + 关闭广告展示 + 开启广告展示 + 设置广告行业标签。**五支全部只印服务商页**、身份是**【普通服务商】+【渠道商】两行**（只抄一个即漏开放面）。动词三形并存且**不可按「关闭一律 DELETE」推断**：关闭广告是 `POST …/close-advertising-show`、**开启广告是 `PATCH …/open-advertising-show`**、设置行业标签是 `POST`。三支 204 无应答包体 ⇒ 裸 `Task`；两支状态切换应答同形（只回 `sub_mchid`）⇒ 共用一型。行业标签是**覆盖原有**语义，35 项值域以 `List<string>` 承载不建枚举（管理支必填、开通支选填）；五支官方页**全部未印**端点级 QPS（守卫 GLD-B1~B6）
  - `Interfaces/Multiuse/` `IWechatPayMultiuseService` — 多次扣费场景发券 4 端点：向用户发券 + 发券流水 / 用券流水 / 退款流水三支账单下载链接。**四支全部只印商户页**；三支流水应答逐字段同形（只回 `url`）⇒ 共用一型，链接 30 秒有效。四支均印「接口调用频率限制：**不区分来源** 1000 次/分钟」与「支持幂等重入」⇒ 是本批次唯一印数字的四支。发券一支是本域**唯一涉密支**（官方请求头表把 `Wechatpay-Serial` 列为必填，`user_name` / `id_card_number` 类型 `string(2048)` 是密文，`out_request_no` 是幂等键）；`send-flow` 页引言跨页抄用了「退款明细」措辞 ⇒ 语义以页面标题 + 路由为准（守卫 MUS-B1~B6）
  - `Interfaces/PayInstallment/` `IWechatPayPayInstallmentService` — 分付分期曝光咨询 1 端点：`POST /v3/pay/instal/exposure`。**只印商户页**且**查询走 POST**（不是 GET）；路由段是 `instal`，写成 `installment` 即 404。官方页原文「每个商户限流 200 次/秒，**上线前必须与微信支付运营确认**」+「接口耗时：200ms」⇒ 照页记入 remarks。`user_id_type` 与 `openid` 的联动（选 `OPENID` 才须带 `openid`）**不做本地校验**，交官方判错；`repayment_info` 是 `array[object]` ⇒ 另立元素类型；`recommend_exposure` 语义是「**建议**曝光」而非「必须曝光」（守卫 PIN-B1~B6）
  - `Interfaces/RateActivity/` `IWechatPayRateActivityService` — 费率活动 3 端点：提交费率活动申请 + `PATCH` 修改申请 + 按 `application_id` 查询。**三支全部只印服务商页**、身份是**【从业机构（银行）】+【从业机构（支付机构）】两行**。POST 与 PATCH 应答同形（只回 `application_id`）⇒ 共用一型，GET 的五键（含 `reject_parameter` / `reject_reason` / `pass_time`）单建一型。**同一字段名两侧类型不同**：`sub_mchid` 请求是 `string(32)`、查询应答是 `integer` ⇒ 照页不「统一」。PATCH 请求的全部字段官方均标选填 ⇒ 不做本地必填拦截；`ALREADY_EXISTS` 是 400 业务码而非幂等成功（守卫 RTA-B1~B6）
  - `Interfaces/Recharge/` `IWechatPayRechargeService` — 银行转账充值 2 端点：申请充值 + 按 `out_recharge_no` 查询充值单。**两支全部只印商户页**。申请支是**前置闸**：应答回 `transfer_in_account` 五键（银行名 / 联行号 / 账户名 / 账号 / 地区码）供宿主走**线下银行汇款**，HTTP 成功 ≠ 充值到账。申请请求 6 键 vs 查询应答 17 键 ⇒ 各建一型；查询以 `out_recharge_no` 作 path 且**无 query**；`recharge_id` 官方长度是 `string(27)`（不是 32/64）；退回（`return_time` / `return_reason` / `bank_card_tail`）是**异步态**。错误码两种拼写并存（`FREQUENCY_LIMITED` 与 `FREQUENCY_LIMIT_EXCEED`）⇒ 照页各归各支，不得合并。路由面与平台收付通充值 `/v3/platsolution/ecommerce/recharges/` **零交集**（守卫 RCH-B1~B6）
  - `Interfaces/SmartGuide/` `IWechatPaySmartGuideService` — 智慧导购 4 端点：导购列表查询 + 导购注册 + 导购信息更新（`PATCH`，204）+ 订单分配导购（204）。**四支全部双页**（商户页 + 服务商页），服务商页多一个**选填** `sub_mchid` ⇒ 并集建模。**查询与注册是同一个 URL**（官方原文「请求URL相同，区别在body和query」）⇒ 一支 GET 带 query、一支 POST 带 body，不得合并成一条契约。三支（查询 / 注册 / 更新）官方请求头表印 `Wechatpay-Serial` 必填、**分配支未列** ⇒ 涉密是逐页结论。分配支必须在**支付完成之前**调用；`userid` 与 `work_id` 二选一互斥、`store_id` 是必填 query、`limit` 上限 10（守卫 SMG-B1~B6）
  - `Interfaces/TransactionBlock/` `IWechatPayTransactionBlockService` — 交易拦截 4 端点：按 `block_record_id` 查拦截记录 + 按 `sub_mchid` 分页查记录列表 + 提交拦截申诉 + 按 `block_submission_id` 查申诉单。**四支全部只印服务商页且仅白名单开放**（未开通即 `NO_AUTH`）。**定位键四支四形**（记录单查走 path 的 `block_record_id`、列表走 path 的 `sub_mchid` + `limit` 必填 query、申诉提交走 path 的 `sub_mchid`、申诉查询走 path 的 `block_submission_id`）。申诉两支五行同形（提交与查询回同一结构）⇒ 共用一型；记录两支各建（单查六键 vs 列表 `data[]` + 分页三键）。`submit_data` 与 `requested_item_info` 官方类型是 `string(4000000)`（**约 4MB 的序列化字符串，不是对象**）⇒ 建成子树即与官方字段类型冲突。**同商户已存在「审核中」申诉单时不允许再提交**，`merchant_submit_id` 是商户侧幂等键。四支官方页全部未印端点级频率（守卫 TRB-B1~B6）
  - `Interfaces/Capital/` `IWechatPayCapitalService` — 银行组件省市 / 银行 / 支行字典 6 端点（P1-5-G **取证勘误补回**：原列「判非契约」，依据是 SKIT 清单老 wiki 路径 301 落到 JSAPI 概述页 —— 该判据无效，按现行文档中心 path 重检证得六支全部在册）。**六支全 GET、全部只印服务商页**；省份与城市两支官方**零请求参数**（全量返回 + `total_count`），对公 / 对私 / 支行三支 `limit` **必填**、`offset` 选填 ⇒ `limit` 做成必传参数、**不给默认值**（给默认值即把官方 400 变成静默错参）。六支逐支印「单商户单接口最大 300TPS」⇒ 本域不属「官方未印 QPS」那类。**只有「获取对私银行卡号开户银行」一支**把 `Wechatpay-Serial` 列为必填且 `account_number` 须密文上送 ⇒ 涉密是逐页结论，不按「同域一律涉密」补齐。纯查询字典族官方要求前端缓存、定期更新 ⇒ SDK 不建缓存层。守卫 CAP-B1~B6
  - `Interfaces/Lovefeast/` `IWechatPayLovefeastService` — 爱心餐品牌信息查询 1 端点：`GET /v3/lovefeast/brands/{brand_id}`（P1-5-G 同上勘误补回，官方爱心餐章节 API 列表恰此一支）。**只印商户页**、**频率官方未印** ⇒ 不得凭第三方 SDK 补数；「爱心餐查券」官方指向既存 `MarketingFavor` 域 ⇒ 不重复建。守卫 LVF-B1~B6
- **模块注册器**（`Extensions/`）：`AddWechatPayApi(...)` + `PayServiceBuilder`（`AddAllApis()` / `AddModules(params PayModule[])` / `Build()`）。`PayModule` 84 个成员即各接口 `[HttpClientApi(RegistryGroupName = …)]` 的组名，**叫 `Transactions` 等而非 `Pay`**——本仓「企业支付」已占用 `WechatModule.Pay` 与生成的 `AddPayWebApiHttpClient()`，同名会跨程序集歧义。**成员数 ≠ 接口数**（三处例外）：`Ecommerce` 一个组名由八个接口共享 ⇒ 一支注册器；`MerchantMedia` 与 `MarketingMedia` 是纯手工通道、无声明式接口 ⇒ 有成员无接口（计数式由守卫 EC-X3 锁定：`成员 = 接口数 − 8 + 3`）。
- **非声明式服务**（十类；官方列在 TOC 内但无 HTTP 形态，或路由/应答非 JSON；一律 `TryAddSingleton`，宿主预注册者胜出）：
  - `Transactions/` `IWechatPayMiniProgramPaySignService` — 小程序调起支付五参数（**本地 RSA 签名、无请求路由**）
  - `Download/` `IWechatPayBillDownloadService` — 账单文件下载通道（`download_url` 动态给出、应答非 JSON）
  - `Fapiao/` `IWechatPayFapiaoFileService` — 发票文件上传（`multipart/form-data` + SM3 摘要）
  - `TaxiInvoice/` `IWechatPayTaxiInvoiceFileService` — 出租车电子发票文件上传（`POST /v3/taxi-invoice/cards/upload-file`，`file` + `meta{company_mchid, digest, digest_algorithm = DIGEST_ALGORITHM_SM3, region_id}`；`region_id` 官方是 `integer` ⇒ meta JSON 里写成**数字**；仅 PDF、≤2M，回来的 `fapiao_media_id` **3 天有效**，逾期须重新上传。摘要字段名拼写**正常**（`digest_algorithm`），与电子发票域的 `digest_alogrithm` + `SM3` 不通用；官方该页示例的 `region_id` 有带引号与不带引号两种写法，实现照字段表取数字并在源码注释里留档。上传成功≠插卡成功，插卡是声明面 `POST /v3/taxi-invoice/cards`（守卫 TXI-B1 三证：路由零声明 + 通道类型存在 + DI 接线存在）
  - `Certificates/` `WechatPayPlatformCertificateRefresher` — 未知序列号时的按需刷新（三道防放大闸 + fail-closed）
  - `Media/` `IWechatPayMerchantMediaService` — 商户媒体上传与图片下载（`file` + `meta{filename, sha256}`，2 支通道）
  - `MerchantManage/` `IWechatPayMerchantManageFileService` — 管理记录文件上传（表单字段名是 `submission_file` + `meta`，**与媒体通道的 `file` 不同名**）
  - `MerchantService/` `IWechatPayComplaintMediaService` — 投诉图片上传（multipart）与拉取（二进制流，2 支通道）
  - `Ecommerce/` `IWechatPayEcommerceCancelMediaService` — 注销资料图片上传（`file` + `meta{file_name, file_digest}`，**键名与商户媒体通道不通用**，守卫 EC-X2 逐字锁在通道源码里；官方页同时标【普通服务商】【平台商户】）
  - `MarketingMedia/` `IWechatPayMarketingMediaService` — 营销图片上传 2 支通道（favor 侧 `POST /v3/marketing/favor/media/image-upload` 回 `media_url`、商品券侧 `POST /v3/marketing/partner/product-coupon/media/upload-image` 回 `image_url`；`file` + `meta{filename, sha256}` 键名同商户媒体通道，**与注销资料通道的 `file_name`/`file_digest` 不通用**；`meta.filename` 上限两支不同 128 / 150 ⇒ 守卫 MMK-B1~B4 逐字锁在通道源码里，不计入生成式接口）
- **AOT JsonContext 合并**（`Extensions/PayJsonResolverExtensions.cs`）：`Build()` 把 DataModels 的 84 个生成上下文经 `JsonTypeInfoResolver.Combine` 注进 `wechat-pay` 命名客户端管线。

## 用法

### 注册

```csharp
// Program.cs：凭据底座（Abstractions 的 AddPayApp）必须先于业务接口
builder.Services.AddSingleton<ISecretProvider>(new MyVaultSecretProvider());   // 密钥端口由宿主实现
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
                .AddWechatPayApi(b => b.AddModules(PayModule.Transactions, PayModule.Refund));
// 或全量：.AddWechatPayApi(b => b.AddAllApis())
```

`AddPayApp` 与 `AddWechatPayApi` 各管一段：`Build()` 会校验 `IWechatPayMerchantManager` 已注册，漏装底座**立即抛出并点名修复方式**（支付无 errcode 令牌自愈，坏配置拖到真实下单才暴露的排查成本极高）。

### 下单并下发小程序调起参数

```csharp
public sealed class OrderAppService(
    IWechatPayTransactionsService transactions,
    IWechatPayMiniProgramPaySignService paySign)
{
    public async Task<WechatPayMiniProgramPaySign> CreateAsync(
        string appId, string openId, string outTradeNo, long totalInCents, string notifyUrl, CancellationToken ct)
    {
        var response = await transactions.CreateJsapiOrderAsync(new JsapiPrepayRequest
        {
            AppId = appId,
            OutTradeNo = outTradeNo,                                   // 6-32 字符、商户号下唯一
            Description = "订单描述",                                  // 官方必填
            NotifyUrl = notifyUrl,                                     // 须与回调路由一致（见 Callback 包）
            Amount = new JsapiAmountInfo { Total = totalInCents, Currency = "CNY" },
            Payer = new JsapiPayerInfo { OpenId = openId },
        }, ct);

        // prepay_id 仅 2 小时有效；paySign 由服务端用商户私钥算出，私钥绝不下发到端上
        return await paySign.CreatePaySignAsync(appId, response.PrepayId!, ct);
    }
}
```

### 多商户作用域

单商户宿主零样板（唯一商户即默认商户）；多商户宿主未开作用域时解析**直接 fail-fast**，不会静默取错商户：

```csharp
public sealed class SubMchCaller(IWechatPayMerchantContext merchantContext, IWechatPayRefundService refund)
{
    public Task<RefundResponse> RefundForAsync(string merchantKey, RefundApplyRequest request, CancellationToken ct)
    {
        using (merchantContext.UseMerchant(merchantKey))   // using 一次性作用域，释放即还原
        {
            return refund.CreateRefundAsync(request, ct);   // 签名头与 mchid 均解析到该商户
        }
    }
}
```

### 错误处理

APIv3 业务失败是 **HTTP 4xx/5xx + `{"code","message"}`**，接口一律 `[AllowAnyStatusCode]` 让错误体原样落到 `WechatPayResponse.Code`/`Message`，再由 `WechatPayException` 判定：

```csharp
try
{
    var order = await transactions.QueryByOutTradeNoAsync(outTradeNo, mchId, ct);
}
catch (WechatPayException ex) when (ex.PayErrorCode == WechatPayErrorCodes.OrderNotExist)
{
    // 官方业务码为字符串（WechatPayErrorCodes 常量表）；NumericFailureSentinel = -1 表示无官方码
}
```

也可对响应显式断言：`WechatPayException.ThrowIfFailed(response, requestUri)`。

## 依赖

- `Mud.Wechat.Pay.Abstractions`、`Mud.Wechat.Pay.DataModels`、企业微信/公众号/支付共用的叶层 `Mud.Wechat.Abstractions`
- `Mud.HttpUtils` 3.0.9 + `Mud.HttpUtils.Generator` 3.0.9（分析器，`PrivateAssets=all`）
- **不引用** `Mud.Wechat.Work*` / `Mud.Wechat.OfficialAccount*`（守卫 `PAY-B11` 锁定工程引用隔离）

## 说明

- **目标框架为受控例外**：`net6.0;net8.0;net10.0`，无 `netstandard2.0`——回调 `resource` 解密强制 `AEAD_AES_256_GCM`，而 `AesGcm` 在 ns2.0 不存在；手写 GCM 与引入第三方密码学包均被否（守卫 `PAY-B9` / `AB-G8` 锁定，签名与加解密全走 BCL、AOT 安全）。
- **无 `[Token]`**：签名在传输层 `WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文。主包 csproj 仍带 `<NoWarn>MUD005</NoWarn>` 只是「防未来误加时误读」，真正的禁令由 `PAY-B1` 断言。
- **不用默认 `IEnhancedHttpClient` 实例**：走命名客户端 `wechat-pay`（`WechatPayHttpClientNames`），否则会与企微线 `qyapi.weixin.qq.com` 撞 BaseUrl 并给企微请求套上商户签名头。SSRF 白名单由 `AddPayApp` 经公用层窄入口登记，**数组零改动**（`PAY-B8`）。
- **端点/路由/字段名的权威是契约守卫**，不是本文与 `PayModule` 的 XML 注释（后者仍留有早期「首批 N 端点」口径）：`Tests/Mud.Wechat.Pay.Tests/ContractGuards/` 85 个文件——`PAY-B1`（零 `[Token]`）、`PAY-B2`（签名/验签串黄金向量）、`PAY-B3`（验签与时间窗 fail-closed）、`PAY-B4`（GCM 参数与黄金向量）、`PAY-B5`（562 端点计数 + 官方路由 + snake_case 字段照抄原文）、`PAY-B6`（无反射、无第三方密码学）、`PAY-B7`（凭据不入日志）、`PAY-B9`（TFM）、`PAY-B11`（引用隔离）、`PAY-NS1~NS3`（声明式接口命名空间恒 `Mud.Wechat.Pay` + 实现类一对一落 `Mud.Wechat.Pay.Internal` + 禁 `Mud.Wechat.Pay.Interfaces.*` 回潮，逐条带正对照）+ 逐域守卫（**基础交易 TX1~TX3** / 分账 / 支付分 / 合单 / 转账 / 发票 / **代金券 MF1~MF6（P1-3 增量八支 + 官方字段名册）** / **特约商户进件 A4S-B1~B7** / **主体进件 A4SJ-B1~B7** / **服务商基础支付 PT-B1~B7** / **服务商支付分 PS-B1~B7** / **从业机构支付分 BS-B1~B7** / **服务商批量转账 PTF-B1~B7** / **从业机构转账与回单 BT-B1~B7** / **还款支付 RP-B1~B6** / **商户风险管理 MRB-B1~B7** / **商户门店管理 MST-B1~B7** / **商户资料变更 MAA-B1~B7** / **商户账户余额 MF-B1~B7** / **商户结算规则申请 MSET-B1~B7** / **平台收付通 EC-X1~X5** / **商家券 MBF-B1~B4** / **商品券 MPPC-B1~B4** / **品牌投放计划 MDP-B1~B4** / **商品补贴活动 MGSA-B1~B4** / **支付有礼 MPGA-B1~B4** / **小票营销 MSRA-B1~B4** / **合作关系 MKP-B1~B4** / **行业券 MIC-B1~B4** / **银行营销任务 MBT-B1~B4** / **银行代金券 MBFA-B1~B4** / **营销图片通道 MMK-B1~B4** / **品牌入驻 BRAP-B1~B4** / **名片配置 BCC-B1~B4** / **交易连接名片 BCL-B1~B4** / **会员卡与会员 BCM-B1~B4** / **品牌分账 BPS-B1~B4** / **品牌门店 BST-B1~B4** / **免密支付契约 PEC-B1~B4** / **在途异常资金 AFP-B1~B3** / **品牌红包 BRP-B1~B4** / **转账到 QQ 钱包 TQW-B1~B4** / **用户确认授权 UCA-B1~B4** / **平台服务商解决方案八域 PEC-IC·PEC-CP·PEC-CL·PEC-DP·PEC-MT·PEC-RE·PEC-RC·PEC-ST 各 B1~B4** / **平台商户保险理赔商家转账三域 PSI-RS·PSI-BT·PSI-RC 各 B1~B4** / **微企付企业支付四域 WEB-BL·WEB-EP·WEB-ST·WEB-US 各 B1~B4** / **委托代扣 PAPay 3.0 两域 PPY-B1~B6·PPY-PS** / **电子发票服务商 / 数电面 13 支 FPX-B1~B6（追加进既存 IWechatPayFapiaoService，无新接口无新注册组；商户面 5 支仍由 FP1~FP4 锁定）** / **车主出行 VEH-B1~B6** / **出租车电子发票 TXI-B1~B6（含第十支 multipart 通道的「零声明 + 类型存在 + DI 接线」三证）** / **教育委托代扣 EDP-B1~B6** / **校园缴费 ESP-B1~B6** / **代发薪 PRC-B1~B6** / **行业方案余十域 BSC·CTR·FTO·GLD·MUS·PIN·RTA·RCH·SMG·TRB 各 B1~B6** / **银行组件与爱心餐 CAP-B1~B6·LVF-B1~B6（P1-5-G 取证勘误补回：原判非契约的七行按现行文档中心 path 重检证实在册 ⇒ 转真缺口）**）与 `WechatPayCapitalRulingContractGuards`（资金域未建模面裁决：**提现族四支与所得记录两支属【平台商户】面 ⇒ `/v3/merchant/fund/` 前缀下在册只有余额两支**，与 `MF-B2` 互为正对照）、`WechatPayNonContractRouteZeroDeclarationContractGuards`（非契约路由 **36 条零声明 / 38 清单行**：P0-7 的 Compliance 2 + MerchantOperate 3 + 尽调族 3 + RealName 1 + 周周减 3、P1-3 营销余 9、P1-2-B 银行转账充值 2、P1-5 行业方案 3、P1-5-F 余 10，逐条带同前缀正对照。判据口径经**两次勘误复核**收敛为「现行官方文档中心按 path + 中文业务名两路检索证无」，**不得**以旧 wiki / 旧 docs 路径 301·302·404 作判据 ⇒ 累计翻案 **31 条**：7 条（P1-5-G 已落地的 Capital / Lovefeast）+ 24 条（P1-4 旧版会员卡两族 22 条 + 「发放消费卡」「终止合作关系」，转 P1-6 / P1-5-H 待落地）。
- **HTTP 动词面自 P0-7 起不再是「只有 GET/POST」**：`MerchantRiskManage` 带 PUT 与 DELETE、`MerchantStore` 带 PATCH；`MerchantAlterApply` 的「撤销」是 **POST 动词段** `/{applyId}/revoke` 而非 DELETE。声明式客户端对五支动词的支持由 MRB-B1 / MST-B1 的「具体动词特性计数 == 基类 `HttpMethodAttribute` 计数」断言锁定，**「成功应答是 204 还是 JSON」逐支断言**（同一条 `/violation-notifications` 上 PUT/POST/GET 回 200+JSON 而 DELETE 回 204；门店 PATCH 回 204 而同路由 GET 回 200；**变更域十二支零 204**）⇒ 不得按「写操作一律 204」或「同路由同应答」收口。跨域反直觉的归并判据也在此批成对固化：风险管理是**同路由分建两形态**（11 方法 / 9 条 `(方法, 路由)`），门店管理是**两页同一份表 ⇒ 共用一套方法与 DTO**（10 页 / 5 端点 / 4 条路由字符串），变更管理是**三族各四页、页页独立 ⇒ 12 页 12 支零归并**（守卫 MAA-B1）。
- **发票文件下载有意不实现**：官方文件域名 `pay.wechatpay.cn` 不在进程级白名单内，白名单零改动优先级高于该便利（`PAY-B8`）。账单 `download_url` 5 分钟有效且一次性，取到即下载。
- 新增 `[HttpJsonSerializable]` DTO 后运行 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）重新生成，并把新上下文登记进 `PayJsonResolverExtensions`；未登记类型在 Native AOT 下会因无元数据失败（JIT 下靠反射侥幸可用）。

---

## 原 `Mud.Wechat.Pay.Abstractions` 包面（2026-10 包合并并入本工程，命名空间不变）


微信支付 **APIv3** SDK **凭据与密码学基座包**：商户配置面、请求签名/应答与回调验签端口、`AEAD_AES_256_GCM` 加解密、时间戳窗口闸、平台证书存储端口、多商户上下文、命名 HTTP 客户端与签名 Handler、`AddPayApp` 注册入口。

> **本包是企业微信线之外的另一条产品线**：凭据是**商户 API 证书 RSA 私钥对请求签名**，**没有 `access_token`**、**不声明任何 `[Token]`**（守卫 `PAY-B1` fail-closed），也没有令牌管理器/刷新阈值这类概念；请求恒发往 `https://api.mch.weixin.qq.com`。企业微信的 `AddPayApi()`（企业支付）/ `AddPayToolApi()`（收银台）走企微 `access_token`，与本包无关。

## 内容

- **商户配置面**（`Configuration/WechatPayMerchantConfig.cs`）：`MchId` / `SerialNumber`（商户 API 证书序列号）/ `PrivateKeySecretName` / `ApiKeySecretName` / `SpMchId` / `SubMchId`。派生面：`IsServicePartner`（`SpMchId` + `SubMchId` 齐备即服务商模式）、`MerchantKey`（服务商为 `{SpMchId}:{SubMchId}`，否则 `MchId`，即多商户路由键）、`AuthorizationMchId`（签名头里的 `mchid`）。`Validate()` 在启动期校验互斥必填，并**拒绝任何密钥材料形状**（`-----BEGIN` / PEM / `MII…` Base64）——配置里只允许出现密钥**名称**。`ToString()` 输出掩码。本类**刻意不继承** `WechatAppConfigBase`（后者的 `BaseUrl` / `AppKey` / `TokenRefreshThreshold` 在支付线无对应概念）。
- **签名串构造与常量**（`Credential/WechatPaySignatureMessages.cs`）：`AuthorizationSchema = "WECHATPAY2-SHA256-RSA2048"`、`SignatureAlgorithm = "SHA256-RSA2048"`、`DefaultTimestampSkewSeconds = 300`、`NonceLength = 32`、`MiniProgramPaySignType = "RSA"`；`BuildCanonicalUrl` / `BuildRequestMessage`（`方法\nURL\ntimestamp\nnonce\nbody\n`）/ `BuildVerifyMessage`（`timestamp\nnonce\nbody\n`）/ `BuildMiniProgramPaySignMessage` / `BuildPackageValue`（`prepay_id=…`）/ `BuildAuthorization` / `CreateNonce`。**逐字节对齐官方**是 `PAY-B2` 黄金向量的锁定项——任一字节漂移都会让线上验签 100% 失败且只在真实交易时暴露。
- **签名/验签端口与实现**（`Credential/`）：`IWechatPaySignatureProvider`（`Sign(message)` → `WechatPaySignatureResult`（只带**签名值 + 序列号**两个可公开字段）、`Verify(serialNumber, message, signature)`）与按商户缓存的 `IWechatPaySignatureProviderFactory`；算法为 RSA-SHA256 + `RSASignaturePadding.Pkcs1`，BCL 直调。
- **时间戳窗口闸**（`Credential/WechatPayTimestampGate.cs`）：返回 `Verdict`（`Valid` / `Missing` / `NotNumeric` / `OutOfWindow`）而非异常——结论枚举天然不含报文内容（`PAY-B7`）；缺失/非数字/超窗一律拒（`PAY-B3`）。
- **GCM 加解密**（`Credential/WechatPayAesGcmCodec.cs`）：`AEAD_AES_256_GCM`，`KeySizeBytes = 32` / `NonceSizeBytes = 12` / `TagSizeBytes = 16`（tag 附在 Base64 密文尾部）。`TryDecrypt` / `TryDecryptOfficialPayload` **fail-closed 返回 false 而不抛出**——异常消息会夹带密文字节；`WechatPayGcmPayload` 为 `readonly struct`（Nonce/Ciphertext/Tag）。
- **凭据取用端口**（`Credential/IWechatPayMerchantCredentialProvider.cs`）：`LoadPrivateKeyAsync(config)` 返回**调用方自有**的 `RSA` 实例（用完须 `Dispose`），`LoadApiKeyAsync(config)` 返回 32 字节 APIv3 密钥；两者一律经组件端口 `ISecretProvider` 按**名称**取用。
- **多商户管理与上下文**（`Credential/WechatPayMerchant*`）：`IWechatPayMerchantManager`（注册表单一来源，未登记键 fail-fast 并给出已注册键清单）、`IWechatPayMerchantContext`（`AsyncLocal` 环境商户上下文：`ResolveCurrent()` / `UseMerchant(merchantKey)` 一次性 `using` 作用域；单商户即默认商户，多商户未开作用域**直接抛**，绝不静默挑第一个）。
- **平台证书端口**（`Credential/`）：读端口 `IWechatPayPlatformCertificateStore`（`TryGetCertificate(serial, out cert)`）、写端口 `IWechatPayPlatformCertificateWriter`、刷新端口 `IWechatPayPlatformCertificateRefresher`（实现随证书域在主包）；默认进程内实现 `WechatPayPlatformCertificateCache` 同时实现读写两端口并**指向同一实例**（否则刷新写 A、验签读 B 会静默不一致）。
- **传输层**（`Transport/`）：`WechatPayHttpClientNames`（`ClientName = "wechat-pay"`、`BaseAddress = "https://api.mch.weixin.qq.com"`、`TypeName` 供 `[HttpClientApi(HttpClient = …)]` 引用）、标记接口 `IWechatPayHttpClient : IEnhancedHttpClient`、`WechatPayAuthorizationHandler`（DelegatingHandler：取当前商户 → 构造签名串 → 只添加一个 **`Authorization`** 头，其值内含 `mchid` / `nonce_str` / `timestamp` / `serial_no` / `signature`；请求已带 `Authorization` 时**幂等跳过**，签名失败一律上抛不静默放行）。
- **注册入口**（`Extensions/PayAppExtensions.cs`）：`AddPayApp` 三重载（`IConfiguration + sectionName`（默认节名 `WechatPayMerchants`）/ `Action<WechatPayMerchantConfig>` / `List<WechatPayMerchantConfig>`），装配上述全部单例并建命名客户端；配置绑定走源生成器。
- **错误码与异常**（`Enums/WechatPayErrorCodes.cs`、`Exceptions/WechatPayException.cs`）：官方 `code` 字符串常量表（`SYSTEM_ERROR` / `SIGN_ERROR` / `PARAM_ERROR` / `NO_AUTH` / `FREQUENCY_LIMITED` / `ORDER_NOT_EXIST` / `OUT_TRADE_NO_USED` …），`WechatPayException : WechatApiException` 带 `PayErrorCode`，`NumericFailureSentinel = -1` 表示无官方码。

## 用法

### 注册凭据底座

```csharp
// 密钥端口由宿主实现（SDK 不抢占注册：密钥治理权在宿主）
builder.Services.AddSingleton<ISecretProvider>(new MyVaultSecretProvider());
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants");
```

```json
{
  "WechatPayMerchants": [
    {
      "MchId": "1900000001",
      "SerialNumber": "5157F09EFDC096DE15EBE01A57495577",
      "PrivateKeySecretName": "wechat-pay/apiv3/private-key/1900000001",
      "ApiKeySecretName": "wechat-pay/apiv3-key/1900000001"
    },
    {
      "SpMchId": "1900000100",
      "SubMchId": "1900000200",
      "SerialNumber": "…",
      "PrivateKeySecretName": "wechat-pay/apiv3/private-key/1900000100",
      "ApiKeySecretName": "wechat-pay/apiv3-key/1900000100"
    }
  ]
}
```

值一律是**密钥名称**（经 `ISecretProvider` 解析），不是密钥内容。商户管理器在**注册期**立刻构造一次，把「配置非法 / 重复商户键 / 空集合」钉在启动阶段。

### 只用底座、不起业务接口

回调包验签与解密**只依赖本包**——纯回调宿主调 `AddPayApp` 即可，命名客户端是惰性的，不发支付请求就不会真的建连：

```csharp
builder.Services.AddPayApp(configuration, "WechatPayMerchants")
                .AddWechatPayCallback(configuration);
```

### 直接复用密码学原语（自建传输或离线校验时）

```csharp
long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
string nonce = WechatPaySignatureMessages.CreateNonce();          // 32 位随机串

// 请求签名串（逐字节即官方规范串：方法\nURL\ntimestamp\nnonce\nbody\n）
string message = WechatPaySignatureMessages.BuildRequestMessage(
    "POST", "/v3/pay/transactions/jsapi", timestamp, nonce, body);

// 官方回调 resource 解密：失败返回 false，绝不抛出、绝不回显密文/密钥
bool ok = WechatPayAesGcmCodec.TryDecryptOfficialPayload(
    apiKey32Bytes, resource.Nonce, resource.Ciphertext, resource.AssociatedData, out string? plaintext);
```

## 依赖

- `Mud.Wechat.Abstractions`（跨产品线共享叶层：SSRF 主机登记窄入口、回调异常与抗重放端口、响应判错契约）
- `Mud.Wechat.Pay.DataModels`
- `Mud.HttpUtils` 3.0.9 + `Mud.HttpUtils.Generator` 3.0.9（分析器，`PrivateAssets=all`）
- `Microsoft.Extensions.Http` / `Microsoft.Extensions.Hosting.Abstractions`：net8.0/net10.0 为 10.0.9，net6.0 为 8.0.1
- `InternalsVisibleTo`：`Mud.Wechat.Pay`、`Mud.Wechat.Pay.Callback`、`Mud.Wechat.Pay.Tests`、`Mud.Wechat.Pay.Callback.Tests`、`DynamicProxyGenAssembly2`

## 说明

- **目标框架是记录在案的受控例外**：`net6.0;net8.0;net10.0`，**无 `netstandard2.0`**（本仓红线要求四档）。原因：APIv3 强制 `AEAD_AES_256_GCM`，而 `System.Security.Cryptography.AesGcm` 在 ns2.0 不存在；手写 GCM 与引入 BouncyCastle 等第三方密码学包两项备选均被否。自 net6.0 起 `AesGcm` / `RSA.ImportFromPem` / `RSA.SignData` / `RSA.Decrypt` 原生可用 ⇒ 三档全走 BCL、AOT 安全。守卫 `PAY-B9` / `AB-G8` 锁定该例外不被静默放宽。
- `csproj` 抑制的是 `SYSLIB1100` / `SYSLIB1101`（源生成器相关）；`MUD005` 抑制只随主包，本包无 `[Token]` 声明。
- **不注册默认 `ISecretProvider`**（组件端口，属宿主密钥治理）；宿主漏装时首次解析凭据取用器给出**点名错误**（说明是谁要求注册的），而非难懂的 DI 缺失异常。
- **SSRF 白名单零改动**（`PAY-B8` / `AB-G9`）：`AddPayApp` 经公用层窄入口 `AddWechatApiHosts()` 登记——支付线不走 `[Token]` 因而不会触发 `AddWechatTokenRecovery`，纯支付宿主若不登记，每一笔请求都会被组件的连接期 SSRF 严格模式拦下（实测缺陷）。
- **命名客户端默认带 `Accept: application/json` 与 `User-Agent`**（`PAY-B12`，`WechatPayHttpClientNames.AcceptHeaderValue` / `UserAgentProductName`）：官方退款域 `/v3/refund/domestic/*` 在**验签之前**就校验这两个头，缺任一直接 `400 INVALID_REQUEST`「Http头缺少Accept或User-Agent」——即整个退款域一笔都调不通，且错误信息与凭据无关、极易误读成签名缺陷。`.NET` 的 `HttpClient` 两者都不默认发送，故登记点只能在 `AddMudHttpClient` 的客户端配置里（由 DM-06 实测抓出，回归锁 `WechatPayTransportPipelineTests`）。
- 多实例部署时，`IWechatPayPlatformCertificateStore` 的进程内默认实现可由宿主以 `TryAdd` 前置注册替换为分布式实现；`IWechatPayMerchantManager` 同理（如需从配置中心动态增删商户）。
- 本包端口面（`IWechatPay*`）即支付线全部公开契约面；业务接口、下载通道与证书刷新实现落在主包 `Mud.Wechat.Pay`。


---

## 原 `Mud.Wechat.Pay.DataModels` 包面（2026-10 包合并并入本工程，命名空间不变）


微信支付 **APIv3** SDK **数据模型包**——官方接口出入参与回调载荷模型（2339 个 `[HttpJsonSerializable]` 类型），独立交付以避免宿主引入不必要的运行时依赖。

> 与**企业微信**线的 `Mud.Wechat.Work.DataModels`（其中 `Pay` / `PayTool` 子域是企微「企业支付」「收银台」，走 `access_token`）分属两条产品线；本包模型对应 `https://api.mch.weixin.qq.com` 的 APIv3 报文，无令牌概念。

## 内容

- **域 DTO**（**86 个域目录** = 接口侧 84 个业务域 − `PartnerPapay`（服务商面单支，DTO 与 `Papay/` 同域共目录）+ `Common` + `Callback` + `MerchantMedia`（手工通道域，无 `[HttpClientApi]` 接口），共 **2339** 个 `[HttpJsonSerializable]` 类型，均标注 `SerializerClassName`；**下列每目录数字为序列化型数**，官方枚举常量集不进该计数、在括注里另计）：
  - `Common/` 1 —— 响应基底 `WechatPayResponse`
  - `Transactions/` 27 —— 直连下单四族（JSAPI/小程序、Native、APP、H5）请求与应答（JSAPI 与 APP 共用 `PrepayIdResponse`；含 `JsapiAmountInfo` / `JsapiPayerInfo` / 商品明细 / 场景 / 门店 / 结算信息、`H5SceneInfo`+`H5Info`、`AppSubsidyInfo` 补贴族）、查单应答、关单请求、小程序调起支付五参数
  - `Refund/` 8、`Bill/` 1、`Certificates/` 3、`Transfer/` 15（7 原有 + P1-2-A 电子回单授权族增量 8）
  - `ProfitSharing/` 12、`PayScore/` 29、`Combine/` 22（命名空间 `…DataModels.Combine`）、`Fapiao/` 53（18 商户面 + P1-5-B 服务商 / 数电面 35）（命名空间 `…DataModels.Fapiao`）、`MarketingFavor/` 41 —— 代金券：批次创建/激活/暂停/重启/查询与列表 + 用户券发放/查询/列表 + 回调通知地址设置与查询 + 资金流与商户列表四支统计查询；`CouponTypes` / `CouponStockTypes` / `CouponTradeTypes` / `CouponRegionTypes` / `CouponBusinessTypes` / `CouponStockStatuses` **6** 组官方枚举常量集，不标注序列化。**同域两套路由参数命名并存**：首批查询支用 path 上的 `{openId}`（camel），P1-3 补差支照官方写 `{openid}`（snake）⇒ 模型侧不能合并成一个共用请求类型
  - `Apply4SubMerchant/` 28 —— 特约商户进件（申请单 + 主体/证件/经营/结算四族请求构件 + 结算账户三面应答 + 官方枚举常量集）
  - `Apply4Subject/` 14 —— 主体进件（申请单 + 主体/证件/联系人/法定代表人/受益人构件 + 三面应答 + 官方枚举常量集）
  - `PartnerTransactions/` 30 —— 服务商基础支付 + 分期（下单五族请求 + 三支应答、查单应答及其促销/商品明细构件、关单请求、分期曝光咨询四件，以及**按族分列**的三套 `scene_info` 与两套 `store_info`、两套 `settle_info`；`PartnerTransactionConstants` 为 6 组官方枚举常量集，不标注序列化）
  - `PartnerPayScore/` 49 —— 服务商支付分（授权五族请求/应答 + 服务订单创建/修改/同步/取消与查询应答 + 共享构件 `PostPayment`/`PostDiscount`/`TimeRange`/`Location`/`Device`/`RiskFund` + 支付分计划与用户签约计划四族 + 三支 204 端点专用请求；`PartnerPayScoreConstants` 等 15 组官方枚举常量集，不标注序列化。**字段名与直连支付分域不通用**：服务商面用 `appid`/`mchid` 而非 `sp_*`，且 `total_original_price`（计划应答）与 `total_origin_price`（签约计划）是官方原文的两个不同拼写）
  - `BankPayScore/` 24 —— 银行 / 从业机构支付分（预授权与授权记录 + 服务订单创建/查询/取消/完结/修改/同步的请求与两支应答 + 扣款信息登记/查询两支应答 + 共享构件 `PostPayment`/`PostDiscount`/`TimeRange`/`Location`/`RiskFund`/`Collection` 族 + 促销明细两件；`BankPayScoreConstants` 等 11 组官方枚举常量集，不标注序列化。**与相邻两支付分域均不通用**：本域在 `appid`/`mchid` + `sub_*` 之外多一层 `channel_id`（请求侧处处必填、预授权应答降为选填），失效码是 `NO_AUTH` 而非服务商面的 `NOAUTH`，查询应答**无** `need_collection`（服务商面有且固定 `true`）⇒ 两域应答不可共用类型）
  - `Callback/` 22 —— 通知信封 `WechatPayNotification` / `WechatPayNotificationResource` + 各域解密后 `resource` 载荷
  - `PartnerTransfer/` 6 —— 服务商批量转账到零钱（发起请求 / 受理应答 + 批次查询应答与批次内明细构件 + 明细输入与明细查询应答；`PartnerTransfer*` 等 **9** 组官方枚举常量集，不标注序列化。**与直连转账域字段名不通用**：本域是 `total_num`/`success_num`/`fail_num` 与 `batch_status`(5)/`detail_status`(6)，直连与银行面是 `*_count` 与 `*_state`；明细请求姓名 `user_name`、应答姓名 `username`，两处不同名且皆 RSA 密文；明细应答**无** `sub_mchid`）
  - `BankTransfer/` 10 —— 从业机构（银行）批量转账与电子回单（转账请求/应答五件 + 明细输入与明细应答 + 汇总与明细回单各「请求 + 应答」四件；`BankTransfer*` 等 **8** 组官方枚举常量集，不标注序列化。**六层商户标识**：`platform_mchid` / `specialized_payment_mchid` / `sponsor_mchid` / `user_authorized_mchid` / `user_authorized_appid` + 应答 `bank_sp_mchid`，本域**无** `sp_*`/`sub_appid`；状态 `batch_state`(4) / `detail_state`(4) 无 `WAIT_PAY`/`INIT` 档；回单应答的 `hash_type`/`hash_value`/`download_url` 仅 `FINISHED` 出现 ⇒ `download_url` 不落模型）
  - `Repayment/` 17 —— 平台收付通·还款支付（下单请求 + 子单/金额/结算/实名/四场景对象七件 + 下单应答；查单应答 + 支付者/子单/金额三件；关单请求 + 关单子单；`RepaymentIdentityTypes` / `RepaymentH5SceneTypes` / `RepaymentTradeStates` **3** 组常量，不标注序列化。**请求与应答同名不同表**：`combine_payer_info` 请求侧是 `identity` 三节密文、应答侧只有 `openid`；子单金额请求 2 字段、应答 4 字段；`trade_state` 本域只 3 值（服务商基础支付 7 值）；字段名含数字打头段 `h5_info` / `h5_url`）
  - `MerchantRiskManage/` 19 —— 商户风险管理（风险订单同步的「服务商 / 普通商户」两形态各一对请求与应答 + 风险共建三族：订单关联信息上报 / 风险信息查询 / 处理结果回传 + 违规通知回调地址四件套 `notify_url` 请求与应答；`MerchantRiskManageTradeRiskTypes`(5) / `MerchantRiskManagePunishTypes`(10) **2** 组常量，不标注序列化。**四套商户标识并存**：`mchid`（普通商户面）/ `sp_mchid`+`sub_mchid`（服务商面）/ `sp_mchid`+`acquiring_bank_id`+`channel_id`+`sub_mchid`（从业机构与渠道面）/ `sub_orders[].mch_id`（合单子单，官方此处**带下划线**）；同名 `risk_type` 有两套类型（风险订单同步面 `integer` 1~4、风险共建面 `string` 5 值）⇒ 两族常量与 DTO 各一套；官方两处误拼 `risk_discove_time` / `same_deviced` 照页保留（守卫 MRB-B3 同时断言「不得写成正确拼写」）
  - `MerchantStore/` 13 —— 商户门店管理（创建请求 + 受理应答、更新请求（**只含 `store_basics` / `store_address` / `store_business` 三棵子树，无 `store_recipient`**）、查询应答（四棵 + `store_state`）、收款绑定与解绑两请求 + 一共用应答 + 失败明细构件；`MerchantStoreStateValues`(6) **1** 组常量，不标注序列化。**十页共用一套模型**：商户面与服务商面字段表只差一个 `sub_mchid`、应答表零差异 ⇒ 无 `Partner` 分身类型（守卫 MST-B6）；`store_id` 官方 `integer` 而 SDK 以 `long?` 承载模型字段、path 参数按线内惯例是 `string`；经纬度 `longitude`/`latitude` 官方即 **string**；`brand_id` `long?`；绑定与解绑**请求分建两型、应答共用一型**（逐项回 `failed_reason`）
  - `MerchantFund/` 1 —— 商户账户余额（实时余额与日终余额**共用一个应答类型**，两键 `available_amount` / `pending_amount` 皆 `long?`（官方 integer 单位分，可越出 int32）；`MerchantFundAccountTypes`(3) **1** 组常量。**同前缀的提现族四支属【平台商户】面 ⇒ 零端点**，本包因此没有提现相关类型（守卫 MF-B2 与裁决守卫 C1 互为正对照））
  - `MerchantSettlement/` 2 —— 商户结算规则申请（创建请求四字段 + 两支共用应答八键；`MerchantSettlementApplicationStates`(3) **1** 组常量。**从业机构面标识**：`acquiring_bank_id` / `channel_id` / `sub_mchid` 三层，本包**无** `sp_*` / `sub_appid`；`settle_rule_id` 官方整型 ⇒ 两侧同 `long?`；`application_process_info` 是 `string(256)` 描述串、官方未给结构 ⇒ 不做子树建模）
  - `MerchantAlterApply/` 17 —— 商户资料变更（主体 / 超级管理员 / 商家简称三族各「提交请求 + 查询应答」，加撤销与驳回明细共用件；`MerchantAlterApply*` **9** 组官方枚举常量集，不标注序列化。**共用/分建逐类型裁定**：三族提交应答同表 ⇒ 共用一型；超管与简称的撤销同表 ⇒ 共用、主体撤销回 `{result}` 分建；三族查询应答**各建一型**（主体族与超管族字段名逐字相同、只差 `state` 值清单；简称族另有 `sub_mchid` 且**无** `audit_reject_detail`）；驳回明细项类型两族共用。**证件字段两套体系不互换**：本域 `card_*` / `id_doc_*` 且 `license` 美式拼写，进件域才是 `identification_*` / `licence`；官方误拼 `EDITTING`（三 T）/ `MODIFING`（单 F）照页保留（守卫 MAA-B3/B4））
  - `MerchantService/` 34 —— 商户消费者投诉（通知地址 CRUD + 投诉列表 / 详情 / 协商历史 + 回复 / 完成处理 / 退款审批 / 即时服务的请求与应答及其嵌套构件；`MerchantService*` **14** 组官方枚举常量集，不标注序列化。图片上传（multipart）与拉取（二进制流）两支走手工通道 ⇒ 本目录无上传表单 DTO）
  - `MerchantManage/` 5 —— 商户管理记录（查询单条应答 + 提交资料请求（`submit_data` 官方是 **`string(4000000)` 的 JSON 串、不是对象**）+ 按 `sub_mchid` 分页查询应答与构件；`MerchantManageConstants` **1** 组。上传管理记录文件走手工通道）
  - `MerchantMedia/` 1 —— 商户媒体上传应答（`media_id` 单键）；上传与下载两支都是手工通道 ⇒ 本目录只有这一个响应类型
  - `MerchantOperationManage/` 2 —— 商户运营管控情况查询（应答 + 限制项构件；`MerchantOperationManageConstants` **1** 组。同前缀尽调族属【平台商户】面 ⇒ 零建模，守卫 MOM-B2）
  - `Ecommerce/` 104 —— 平台收付通（**八个子域共用一个注册组**：账户 / 进件 / 账单 / 资金 / 分账与补差 / 退款 / 商家转账 / 代扣合单）；`Ecommerce*` **58** 组官方枚举常量集。**共用 / 分建逐族裁定**：退款族三支「二级商户面 / 个人收款方面」同方法同路由 ⇒ 请求与应答各分建两型；余额两支、提现查询两支、进件两路查询、注销提现查询两支、普通注销「申请 + 查询」、分账「请求 + 查询」与回退单「请求 + 查询」、回补两支、接收方「添加 + 删除」、转账授权两支、代扣下单与查单各**共用一型**（判据是官方字段表，不是页面数）。**两支注销是两套契约**：`apply-cancel-withdraw`（状态九值）与 `cancel-applications`（三值）单据定位键、状态枚举、应答表全不同 ⇒ 不得互相套用。密文键（`account_name` / `account_number` / `identification_name` / `identification_no` / `cert_number` / `user_name` / `receiver_name` / `bank_account` / `real_name` / `id_card_*`）一律 `string` 承载、SDK 不代为加解密；应答的 `real_name_auth_result.access_token` 是**不可刷新凭据** ⇒ 禁进日志（守卫 EC-A3 / EC-C1 / EC-M3 / EC-R6 / EC-X4）
  - `MarketingBusifavor/` 89 —— 商家券（批次 CRUD 与预算 / 券核销退货 / 关联与解绑补贴 / 自定义 code 上传 / 用户券 / 回调地址 / 商品券发券）。**双面同表 ⇒ 一型一份**：15 支里 13 支的【普通商户】与【普通服务商】两页字段表逐字同形 ⇒ 模型不因身份分身（分身即造出官方不存在的契约）。**用户券两支按官方定位键分建**：列表支只在 path 带 `openid`，查单支是 `openid` + `coupon_code` + `appid` 三元组 ⇒ 两个请求与应答类型不共用
  - `MarketingPartnerProductCoupon/` **774** —— 商品券（服务商，本包最大目录）：商品券「单品 / 券包」两形态 + 券批次 + 券包 + 门店关联 + 预算 + 用户券 + 图片生成任务 + 发券与回调。**官方字段表以点号路径深度嵌套**（`stock.stock_usage_rule.…`）⇒ 每个子树独立成类型、`[JsonPropertyName]` 恒落**叶子名**，类型名按「端点 + 请求/应答 + 路径段」机械拼装；同路由两契约（单品 / 券包）⇒ 两套请求类型不可合并
  - `MarketingPartnerDeliveryPlan/` 11 —— 品牌投放计划（创建 / 更新 / 查询 / 列表 / 终止）：**终止支官方零字段** ⇒ 本目录无对应请求与应答类型。**回调地址支的应答与请求同形但独立建型**（应答须承载基底 `Code` / `Message`）
  - `MarketingGoodsSubsidyActivity/` 24 —— 商品补贴活动 + 零售门店活动 + 业代与门店绑定：**两支 DELETE 带 JSON 请求体** ⇒ 删除有独立请求类型（`out_request_no` + `delete_time` + 明细数组），不存在「DELETE 一定无包体」的共用空型
  - `MarketingPayGiftActivity/` 44 —— 支付有礼活动（门槛活动四支 + 商品池 / 商户池维护）：池成员增删与查询各建独立类型，池明细构件在多支间共用；`offset` / `limit` 在列表支官方必填 ⇒ 以非空值语义交给宿主
  - `MarketingShoppingReceipt/` 18 —— 小票营销（自定义入口创建 / 查询 / 修改 + 跳转信息上传 + 发送购物小票）：**本域无 DELETE** ⇒ 无删除型；**发送购物小票官方零 Body** ⇒ 无请求类型
  - `MarketingPartnerships/` 10 —— 商户合作关系（建立 + 列表查询）：查询支的 `partner` / `authorized_data` 官方是**对象**，但声明式签名以 `string` 承载 URL 编码后的 JSON 串 ⇒ 模型侧只提供嵌套构件类型
  - `IndustryCoupon/` 3 —— 行业券（出行优惠券）切卡 token 申领：`coupon_list` 官方点号路径 ⇒ 子树独立成类型、落叶子名
  - `MarketingBank/` 3 —— 银行营销任务（包任务创建 / 查询）：**两支官方 Body 表为空** ⇒ 无请求类型，仅应答与其嵌套构件
  - `MarketingBankFavor/` 4 —— 银行代金券（多活动发券 + 按 `coupon_id` 查询）：`openid` / `coupon_id` 走 path ⇒ 请求类型只承载业务参数，不含定位键
  - `MarketingMemberCardOpen/` **151** —— 旧版会员卡模板与用户卡管理（本批最大目录之一）：模板族 5（创建 / 列表 / 查询 / PATCH 修改 / DELETE 删除）+ 模板升级 1 + code 预存 1 + 用户卡族 8（查询 / PATCH 修改 / 作废 / 确认入会 / OpenID 导入 / 手机号导入 / 二维码创建 / code 解密）+ 关联小程序 1 + 设置管理页入口 1 + 激活码 1。**四支 204 无应答包体**（删除模板 / 升级模板 / 作废卡 code / 激活卡 code）⇒ 零 DTO；**响应侧 Code 字段遮蔽基类**（业务字段 `code` = 会员卡 code，与 `WechatPayResponse.Code` 冲突）⇒ 5 个响应类型加 `[new]` 显式声明隐藏意图。嵌套子树包括 `brand` / `date_information` / `balance_information` / `user_information_form` / `additional_statement` / `favor_*` / `bonus_information` / `custom_field_list[]` / `rights_info` 等，按「端点 + 请求/应答 + 路径段」机械拼装独立成类型。**两套并存契约**：本域 `/v3/marketing/membercard-open/*` 与品牌会员卡 `/v3/brand/partner/card-member/*` 是并存非替代关系
  - `MarketingMemberCardActivity/` **58** —— 会员有礼活动（创建活动 + 列表查询 + 详情查询 + 终止活动）：全部路由落 `/v3/marketing/membercard-activity/activities`，终止支走 POST 动词段 `…/{activity_id}/terminate`。嵌套子树包括 `base_info` / `member_card_activity_info` / `benefit_info` / `available_time` / `use_condition` / `stock_info` 等，按端点分建独立类型
  - `BrandApplyment/` 10 —— 品牌入驻（提交请求 + 其 `subject_info` / `brand_basic_info` / `admin_info` / `trademark`（含两支 `*_trademark_registration_certificate`）四族构件 + 受理应答 + 撤销请求 + 状态应答。**四支路由两两共用应答**：两支受理（提交 / 撤销）同表 ⇒ 共用 `BrandApplymentAcceptResponse`，两支查询（按 `applyment_id` / 按 `out_request_no`）同表 ⇒ 共用 `BrandApplymentStateResponse`；**共用只发生在应答侧**，请求侧因必填口径不同一律分建。本域**两套编号并存**：`applyment_id`（官方受理号）与 `out_request_no`（商户业务号），撤销请求只认前者）
  - `BrandCardConfig/` 11 —— 品牌名片配置（查询 / 申请 / 撤销 / 预览 / 发布五支各一应答 + 申请请求的 `brand_mini_program_info` / `brand_customer_service` / `service_list[]` 三棵构件 + 撤销与发布各自请求）：**申请 → 预览 → 发布是两段式**，查询支回的是「当前生效配置」、预览支回的是「待发布草稿」⇒ 两应答分建（守卫 BCC-B3）
  - `BrandCardLink/` 9 —— 交易连接名片（列表 / 申请 / 状态 / 撤销 / 解绑五支）：列表应答的 `active_link_list[]` 独立成构件；**申请（6 行）与状态（8 行）两支应答不同形 ⇒ 各建一型**，撤销与解绑亦各建一型。分页口径是 `page_index` / `page_size`，与本批会员卡两支的 `offset` / `limit` **不同形**，不得互相套用
  - `BrandCardMember/` 73 —— 会员卡与会员（模板四支 + 用户卡五支 + 预授权 + 积分 / 流水，本批最大品牌目录）：模板子树（`code_jump_information` / `valid_date_information` / `base_info` / `note_info` / `background_information` / `wallet_info` / `membership_promotion` 等）按「端点 + 请求/应答 + 路径段」机械拼装独立成类型。**模板四支（创建 / 查询 / 修改 / 作废）共用一份 49 行应答**、列表支 53 行单建；**用户卡五支（查询 / 修改 / 确认 / 作废 / 导入）共用一份 28 行应答**、列表支 32 行单建。**请求侧不分共用**：同族各支必填口径不同 ⇒ 一请求一类型
  - `BrandProfitSharing/` 15 —— 品牌分账（发起 / 查询 / 列表 / 完结 / 最大比例 / 待分金额 + 接收方添加与删除 + 回退单）：回退「请求 + 应答」与接收方「添加 + 删除」各按同表共用；发起 15 行与查询 16 行**分建**（查询多 `finish_amount` / `finish_description`）。`receivers[]` 在请求 / 应答 / 列表三处各建构件（必填口径与字段集逐表不同）。**与直连 `ProfitSharing/` 是同族不同路由**：本域在 `/v3/brand/partner/profitsharing/`，模型不复用
  - `BrandStore/` 27 —— 品牌门店（创建 / 查询 / 更新 / 列表 / 删除 / 绑定收款号 / 解绑收款号 / 打烊 / 恢复九支）：`store_basics` / `store_address` / `store_business` / `store_recipient[]` 四棵树在「写侧请求」与「读侧应答」各建一套（请求无 `store_recipient`、应答有）；创建 / 查询 / 更新**共用 21 行应答**，`close` / `resume` **共用 3 行状态应答**，绑定 5 行与解绑 1 行**分建**；**删除支官方未印发应表** ⇒ 接口返回裸 `Task`，本目录因此没有对应应答类型
  - `PasswordExemptContract/` 12 —— 免密支付（契约查询 / 解约 + 四支预签约）：**四支预签约各建一型**——`qs` 面与 `sp` 面、小程序场景与支付后场景两两正交，字段名 `appid` / `openid`（`string(128)`）与 `sp_appid` / `sp_openid`（`string(64)`）**不可互换**（守卫 PEC-B3）；契约应答的 `contract_terminate_info` 独立成构件。**与 `PayScore/`、`PartnerPayScore/` 无交集**：本域是免密协议面，令牌与签名主体为品牌商户
  - `AbnormalFundProcessing/` 15 —— 在途异常资金处理（付款单应答 + 指令发起请求/应答 + 指令查询应答）：**三支应答各建一型**（付款单含 `transfer_amount` 与三条指令列表，指令发起与指令查询的 `instruction` 结构同名不同表 ⇒ 独立成类型）；`commander` / `receiver` 构件在请求侧与应答侧各建一套
  - `BrandRedpacket/` 6 —— 品牌商户红包批次（发起 + 按 `batch_no` / `out_batch_no` 两路查询 + 按 `detail_no` / `out_detail_no` 两路查明细）：**两支批次查询共用 `BrandRedpacketBatchQueryResponse`、两支明细查询共用 `BrandRedpacketDetailQueryResponse`，发起支单建** ⇒ **共用只在应答侧**；`brand_id` 官方是 `integer`（品牌线各域为 `string(20)` / `string(32)`），照页建整型
  - `TransferToQqWallet/` 5 —— 转账到QQ钱包（发起 / 查询 / 撤销）：`transfer_scene_report_infos` 列表独立成构件；撤销支应答与查询支**不同形 ⇒ 分建**
  - `UserConfirmAuthorization/` 7 —— 免确认收款授权（申请 / 查询 / 关闭）：申请与查询、关闭各建应答；`is_display_authorization` 是 query 而非报文字段 ⇒ 不进模型
  - `PlatsolutionEcommerceInsuranceContract/` 3 —— 平台服务商解决方案·保险理赔功能开通（开通 + 查询）：请求的 `agreement_list[]` 签约协议构件独立成类型；两支应答同形 ⇒ 共用一型
  - `PlatsolutionEcommerceCompensate/` 17 —— 赔付账单（`/v3/platsolution/ecommerce/mch-transfer/compensate-bills`）：发起 / 查询 / 撤销及其 `receiver` / `amount` 构件。**本域 `receiver.type` 官方只印 `TRANSACTION_USER`**（`PlatsolutionEcommerceCompensateReceiverTypes` 单值），与理赔 / 押金族的多值集**不得互套**
  - `PlatsolutionEcommerceInsuranceClaim/` 18 —— 保险理赔账单：发起 / 查询 / 撤销及其构件；`receiver.type` 值集是 `MERCHANT` + `TRANSACTION_USER` **两支**（守卫 PEC-CL-B4 与常量类的逐页出处注释同源）
  - `PlatsolutionEcommerceDeposit/` 45 —— 押金族（`mch-transfer/deposit-*` 七支：售后赔付「发起 + 按 `out_bill_no` / 按 `bill_id` 两路查询」、运费赔付「发起 + 查询」、保费赔付「发起 + 查询」，本批最大平台服务商目录）：**三族各建一套请求与应答**，且三族的查询路由形态互异（售后与运费走 `…/out-bill-no/{out_bill_no}`，保费直接是 `…/{out_bill_no}`）⇒ 定位键照页不得归一；`receiver.type` 三族三套值集（售后 `MERCHANT`+`TRANSACTION_USER`+`WXPAY_USER`、保费 `MERCHANT` 单值、运费族官方页**不印** `receiver.type` ⇒ 零常量），照页不得合并成一个常量集
  - `PlatsolutionEcommerceMchTransfer/` 12 —— 平台服务商二级商户转账（`/v3/platsolution/ecommerce/mch-transfer/transfer-bills`）：发起 / 查询 / 撤销及构件。**出款状态 `PlatsolutionEcommerceTransferStates`（6 值）与 `PlatsolutionEcommerceTransactionTypes`（`WXPAY` / `WXVALUE`）在本目录四族（转账 / 赔付 / 理赔 / 押金）间共用**（值集逐页同形，全仓只建一份常量类，各族以全限定 `cref` 指回，不得各族复制一份）；`business_type` 4 值即四族路由的枚举镜像，本族 `receiver.type` 只有 `MERCHANT`
  - `PlatsolutionEcommerceRechargeEmployee/` 4 —— 充值员工管理：`employee_type`（`ADMIN` / `STAFF` / `LEGAL_PERSON`）与 `state`（`EFFECTIVE` / `DELETED`）两组常量
  - `PlatsolutionEcommerceRecharge/` 8 —— 二级商户充值（申请 / 查询 / 关闭）：**查询与关闭应答逐字段同形（36 行）⇒ 共用 `RechargeQueryResponse`**，申请支单建；`account_type` / `recharge_channel` / `recharge_scene` / `recharge_state` / `online_bank_type` 五组常量。两支「银行转账充值」路由官方页 404 ⇒ **零声明、零建模**（非契约路由守卫第八条）
  - `PlatsolutionEcommerceSettle/` 7 —— 个人收款批量结算（申请 / 查询 / 异常批次处理）：**三支应答同形（14 行）⇒ 共用 `SettleApplyResponse`**；`settle_info[].settle_detail_info[]` 两棵子树独立成类型；`settle_type`（必填 2 值）/ `receiver_type`（选填，`USER` 而非其它族的 `TRANSACTION_USER`）/ `abnormal_type` / `abnormal_settle_type` / `trade_scenario` 五组常量
  - `PlatsolutionInsuranceReservation/` 6 —— 平台商户预约商家转账·保险理赔（受理 + 按 `out_reservation_no` / 按 `reservation_id` 两路查询 + 关闭）：**四支建三型**——受理应答 6 行含**必填** `package_info`（拉起客户端用户确认页）单建，后三支官方应答表逐字段同形（18 行）⇒ 共用 `InsuranceReservationQueryResponse`；**关闭支官方印有应答表** ⇒ 既不返回裸 `Task` 也不凭空造请求类型。`scene_report_data.insurance_policy` 子树独立成类型；`user_real_name` 官方要求**加密后传入** ⇒ `string` 承载、SDK 不代为加解密
  - `PlatsolutionInsuranceBatch/` 7 —— 平台商户商家转账批次与明细·保险理赔（受理 + 批次两路查询 + 明细两路查询）：**五支建四型**——批次两支查询同形（24 行）⇒ 共用 `InsuranceBatchQueryResponse`（其 `transfer_batch` 与 `data[]` 各独立成构件），明细两支同形（17 行）⇒ 共用 `InsuranceBatchDetailQueryResponse`，受理支单建。**分页 query 只印在批次两支**（`need_query_detail` 必填 / `offset` / `limit` / `detail_state`），明细两支零分页 ⇒ 参数表不得互套；本域字段名是 `batch_id` 而非 `transfer_bill_no`，与 `Transfer/`、`TransferToQqWallet/` 不复用
  - `PlatsolutionInsuranceReceipt/` 6 —— 保险理赔商家转账电子回单（汇总「申请 + 查询」+ 明细「申请 + 查询」）：**四支四型、两两不同形**——申请支 3/4 行、查询支 7/9 行 ⇒ 只有查询支才返回 `hash_type` / `hash_value` / `download_url`，汇总与明细又差 `out_detail_no` / `detail_id` ⇒ 不得因「字段名大量重合」合并；申请支 `sub_mchid` 是**请求体唯一字段**（定位键在 path）、查询支是**必填 query 且零请求体** ⇒ 两型请求 DTO 不共用
  - `WebizpayBill/` 4 —— 微企付企业支付账单与出资凭证（申请出资凭证 + 申请业务账单）：**两支四型、两两不同形**（凭证 6 行请求 / 11 行应答、账单 4 行请求 / 7 行应答）⇒ 不得因「都返回 `download_url`」合并。日期口径不同：凭证支是 `start_date` + `end_date` 区间、账单支只有 `date`（官方明写「仅支持按一天的纬度查询」）且 `type` 当前只有 `QYZFALL`；`entity_name` **只在凭证支**；账单支才有 `hash_type`（官方固定 SHA1）/ `hash_value`。两支 `download_url` **5 分钟内有效** ⇒ 取到即下、不得缓存复用
  - `WebizpayEmployee/` 8 —— 微企付额度卡与员工授权（下发 + 按 `card_no` / 按 `out_card_no` 两路查询 + 作废 + 支付链接 + 解除授权）：**六支建五型**——两支查询官方应答表逐字段同形且必填口径一致（20 行）⇒ 共用 `WebizpayQuotaCardQueryResponse`，其余各建一型。**定位键分工**：path 恒为 `employee_id`（官方【微信授权关系 ID】`WeBizPay_<uuid>`，**不是**商户企业员工 ID `user_id`）；作废与支付链接两支**只认微信卡号 `card_no`**、不接受 `out_card_no`。同名 `sp_mchid`/`sub_mchid` 按动词落位不同：GET 三支落必填 query、零请求体，POST 三支落 Body
  - `WebizpayStore/` 5 —— 微企付门店主体匹配（发起 + 按 `batch_id` 查询）：`match_details[]` 与 `match_results[]` 两棵子树各独立成类型。**本域两支是微企付 12 页中唯一只印 `sp_mchid`、零 `sub_mchid` 的一对** ⇒ 不得按本族其余十支的双层形态补字段；`match_details` 官方标选填、但元素三字段全必填；`product_type` 可选取值照官方写 `CODE_PAY`（不做拼写归一）
  - `WebizpayUser/` 4 —— 微企付员工开通授权（授权链接 + 授权状态）：授权状态应答的 `employee_list[]` 独立成构件。**两支是「`user_id` ↔ `employee_id`」互查的两端**：`user_id` 在 path、应答回 `employee_id` ⇒ 额度卡域各支的 path 参数由本域取得。**授权链接支 8 行应答里 `authorization_url` / `expire_time` / `mp_query` / `mp_business_type` 全为选填**（官方「仅当可以开通时返回」）⇒ 200 零链接不是错误；`employee_list` 亦可为 0 长度。小程序参数有效期 **35 分钟**，其消费面是 JSAPI `wx.openBusinessView`（非 HTTP 端点）⇒ 本包只透传字段、零端点
  - `Papay/` 82 —— 委托代扣 PAPay 3.0（直连面 20 端点 + 服务商面 1 端点，按六条产品线分文件）：**四端预签约各建一型**（APP 13 / H5 15 / JSAPI 13 / 小程序 13 行，请求表字段集互异），保险自动续费三端同理（37 / 35 / 35 行）⇒ 只有**应答同形**者共用（JSAPI 与 H5 皆单行 `redirect_url`；APP 与小程序各回 3 行、形态亦不同 ⇒ 单建）。**共用只发生在应答侧**：签约查询与解除、预约扣费查询与受理、保险协议查询与解除、保险周期查询与受理四对官方应答表逐字段同形且必填口径一致 ⇒ 各共用一型。服务商面 notify 包体是 `sp_mchid` / `sp_appid` / `sub_mchid` / `sub_appid` 四层、直连面是 `mchid` / `appid` ⇒ **不同名即不同型**，两支请求各建（不做超集）。三支官方应答节原文「无应答包体」/「无参数返回」（正常示例 204）⇒ **不建响应类型**
  - `Vehicle/` 34 —— 车主出行（ETC 六支 + 停车四支，按「ETC 签约 / 用户绑定 / 预开通 / 高速下单 / 交易查询 / 停车入场 / 停车服务 / 停车交易」分文件）：**两族开放面不同 ⇒ 模型按并集建模**——ETC 族官方只印服务商页（`sp_mchid` + `sub_mchid`），停车族是商户页 + 服务商页双页同路由，字段取并集、必填只取交集；两路 ETC 查单（微信单号 / 商户单号）与两支签约查询各自应答同形 ⇒ 共用；ETC 预开通与停车扣费两支「受理 ≠ 成功」⇒ 应答只含受理字段，另建查询型不合并
  - `TaxiInvoice/` 24 —— 出租车电子发票（司机 / 出租车 / 开票公司三对更新与查询 + 签到签退 + 插卡 + 按 token 查订单 + 文件上传应答）：签到签退与插卡两支 204 无应答包体 ⇒ **不建响应类型**；更新支与其查询支**字段集不同 ⇒ 各建一型**（更新请求不含 `region_id` 之外的定位回显）；`TaxiInvoiceUploadFileResponse` 只是 `fapiao_media_id` 单键应答，其**请求侧无 DTO**——上传走手工通道，`meta{company_mchid, digest, digest_algorithm, region_id}` 由通道用 `Utf8JsonWriter` 现场写出（`region_id` 官方 `integer` ⇒ 写数字而非字符串）
  - `PayrollCard/` 11 —— 代发薪（工资卡）：令牌申领、授权关系查询、认证单预下单 / 单查 / 列表、发起代发薪批次六支。**六支官方全部只印服务商页** ⇒ 无直连面镜像。**本域无 `sp_mchid`**：请求侧只有 `sub_mchid`（服务商身份由签名主体承载），应答侧服务商商户号却叫 `mchid`；AppID 两套命名并存（令牌 / 关系 / 认证单三支 `appid` + `sub_appid`，发起批次支 `sp_appid` + `sub_appid`）⇒ 按「服务商面必是 `sp_*`」归一即错。**认证单两支各建一应答**：单查应答与列表 `data[]` 元素官方逐字同形（13 键同序），但共用只发生在应答根类型 ⇒ 列表元素独立成类型。**涉密是逐页结论**：6 支里 3 支的官方请求头表把 `Wechatpay-Serial` 列为必填，密文姓名与证件号以 `string` 承载、SDK 不代为加解密
  - `EducationPapay/` 16 —— 教育委托代扣（EducationPAPay）八支：解除协议、按 `contract_id` 查协议、按 `openid` 查协议列表、预签约、下单受理、两支查单、发送预扣费通知。**七支双页并集 + 一支只印商户页**：七支同时印【普通商户】与【普通服务商】⇒ 字段取并集、必填取交集、一面一方法；`DELETE /v3/edu-papay/contracts/{contract_id}` 官方**只有商户页** ⇒ 该支无服务商镜像。**三支 204 无应答包体**（解除协议 / 下单受理 / 发送预扣费通知）⇒ 接口返回裸 `Task`、本目录无响应类型。**两支查单应答逐字段同形（33 行）⇒ 共用 `EducationPapayTransactionQueryResponse`**；但协议单查应答与列表 `data[]` 元素字段同形仍**各建一型**（共用只在应答根类型）。预签约应答单行且必填 `presign_token` ⇒ 是调起签约页面的入参、不等于签约成功
  - `EducationSchoolPay/` 18 —— 校园缴费（教育学费缴纳）八支：预签约、解除协议、协议单查 / 列表、缴费下单、两支查单、欠费状态查询。**八支官方全部只印服务商页** ⇒ 无商户面镜像，不得按教育代扣域的双页形态补建。**下单应答与两支查单应答逐字段同形 ⇒ 三支共用 `EducationSchoolPayTransactionQueryResponse`**（本域唯一「写侧与读侧共用应答」的一对）；解除协议 204 ⇒ 裸 `Task`。**请求侧无 `openid`**（扣款对象由 `contract_id` + `user_id` 定位），应答 `payer` 只有 `openid` / `sub_openid` ⇒ 两侧定位口径不同。**四枚密文字段只在预签约支**（`identity.real_name` / `identity.id_card_number` / `bank_card.bank_card_no` / `bank_card.phone`）⇒ `Wechatpay-Serial` 必填逐页核对为该一支，其余支未列。欠费支 `state`（`NORMAL` / `FORBIDDEN`，用户欠款风控状态）与订单支 `trade_state` **同名不同义** ⇒ 两套值域不得互套
  - `BusinessCircle/` 4 —— 商圈会员（支付即积分）四支：停车状态同步、积分同步、积分服务授权查询、待积分状态查询。**四支官方全部同时印商户页与服务商页** ⇒ 请求并集建模，其中授权查询支的 `sub_mchid` **只在服务商页印发**（商户页无该键）；两支 204 无应答包体 ⇒ 本目录不含其响应类型。`brandid` 官方类型是 `integer`（不是字符串）、`sub_mchid` 在停车支是 `string(32)` 而在积分与授权两支是 `string(64)` ⇒ 照页保留长度差异，不做「服务商面一律 32」归一（守卫 BSC-B3 / BSC-B5）
  - `Catering/` 1 —— 点餐订单信息同步单支。**官方只印服务商页**，请求表 31 行是两层摊平 ⇒ 菜品子树另立 `CateringOrderDish`；`properties` 官方只标 `object` 未给元素结构 ⇒ 以 `System.Text.Json.JsonElement` 原样承载（本包唯一使用该形态的字段）；`count` 官方类型是 `Float` ⇒ `double`；正常应答 204 ⇒ 无响应类型（守卫 CTR-B3 / CTR-B5）
  - `FundsToOversea/` 11 —— 资金出境六支：出境下单、两支查单（`out_order_id` / 「`sub_mchid` + `transaction_id`」）、退货单创建与查询、可出境金额查询、账单下载链接。**六支官方全部只印服务商页且身份是【平台商户】**。**下单支与两支查单应答逐字段同形 ⇒ 共用 `FundsToOverseaOrderResponse`**，退货两支同理共用 `FundsToOverseaReturnOrderResponse`；`goods_info` 是数组 ⇒ 独立元素类型，`seller_info` / `express_info` / `payee_info` / `presale_info` 四棵子树各立类型（守卫 FTO-B3 / FTO-B5）
  - `GoldPlan/` 6 —— 点金计划五支：自定义页开关、计划开关、关闭广告展示、开启广告展示、设置广告行业标签。**五支官方全部只印服务商页**且身份是**【普通服务商】+【渠道商】两行**。两支状态切换应答同形（只回 `sub_mchid`）⇒ 共用 `GoldPlanStatusResponse`；三支 204 无应答包体 ⇒ 不建响应类型；`advertising_industry_filters` 是 `array[string]` 且 35 项值域 ⇒ 以 `List<string>` 承载不建枚举（守卫 GLD-B3 / GLD-B5）
  - `Multiuse/` 3 —— 多次扣费场景发券四支：发券请求与应答 + 发券 / 用券 / 退款三支流水下载链接应答。**四支官方全部只印商户页**；三支流水应答逐字段同形（只回 `url`）⇒ **一支类型服务三支端点**。`user_name` / `id_card_number` 官方类型 `string(2048)` 是**密文** ⇒ 以 `string` 承载、SDK 不代为加解密（守卫 MUS-B3 / MUS-B5）
  - `PayInstallment/` 3 —— 分付分期曝光咨询单支（**查询走 POST**）。请求四键 + 应答两键，`repayment_info` 是 `array[object]` ⇒ 独立元素类型；路由段是 `instal`（不是 `installment`）（守卫 PIN-B2 / PIN-B5）
  - `RateActivity/` 10 —— 费率活动三支：提交申请、`PATCH` 修改申请、按 `application_id` 查询。**三支官方全部只印服务商页**、身份是**【从业机构（银行）】+【从业机构（支付机构）】两行**。POST 与 PATCH 应答同形（只回 `application_id`）⇒ 共用 `RateActivityApplicationResponse`，GET 的五键单建一型；`sub_mchid` 请求 `string(32)` 而查询应答 `integer` ⇒ 照页不「统一」；`activity_detail` / `additional_information` 及 `activity_apply_information` 列表元素各立类型（守卫 RTA-B3 / RTA-B5）
  - `Recharge/` 7 —— 银行转账充值两支：申请充值 + 按 `out_recharge_no` 查询。**两支官方全部只印商户页**。申请请求 6 键 vs 查询应答 17 键 ⇒ 各建一型，不做「查询即申请的应答」归一；`recharge_amount` / `transfer_in_account` / `bank_transfer_info` 三棵子树各立类型；`recharge_id` 官方长度 `string(27)`（不是 32/64）（守卫 RCH-B3 / RCH-B5）
  - `SmartGuide/` 6 —— 智慧导购四支：导购列表查询、注册、信息更新（`PATCH`）、订单分配。**四支官方全部同时印商户页与服务商页** ⇒ 服务商页多出的**选填** `sub_mchid` 并入同一请求类型，不拆分身。查询应答是分页外壳 + `data[]` 元素 ⇒ 两型；注册与查询**共用同一条 URL**（差异只在方法与报文形态）；两支 204 ⇒ 不建响应类型；`mobile` / `qr_code` / `avatar` 涉密 ⇒ `string` 承载（守卫 SMG-B3 / SMG-B5）
  - `TransactionBlock/` 5 —— 交易拦截四支：按 `block_record_id` 查拦截记录、按 `sub_mchid` 分页查记录列表、提交拦截申诉、按 `block_submission_id` 查申诉单。
  - `Capital/` 12 —— 银行组件省市 / 银行 / 支行字典六支（P1-5-G 取证勘误补回，原列「判非契约」）：省份与城市两支官方**零请求参数**（全量 + `total_count`）、对公 / 对私 / 支行三支 `limit` **必填**、卡号开户行一支是涉密支（`account_number` 密文）。对公与对私应答逐字段同形 ⇒ 共用 `CapitalBankPageResponse`；支行支根层另印四个银行定位键、卡号支无 `count` / `offset` / `links` 骨架 ⇒ 各建一型。翻页子树 `*Links` 独立建型。
  - `Lovefeast/` 1 —— 爱心餐品牌信息查询单支（P1-5-G 同上勘误补回）：应答只回 `donate_count` / `consume_count` 两键。「爱心餐查券」官方指向既存 `MarketingFavor` 域 ⇒ 本目录不重复建型。**四支官方全部只印服务商页且仅白名单开放**。申诉两支应答逐字段同形（五行）⇒ 共用 `TransactionBlockSubmissionResponse`；记录两支各建（单查六键 vs 列表 `data[]` + 分页三键）。`submit_data` 与 `requested_item_info` 官方类型是 `string(4000000)` ⇒ **约 4MB 的序列化字符串、不是对象**，建成子树即与官方字段类型冲突（守卫 TRB-B3 / TRB-B5）
- **响应基底**（`Common/WechatPayResponse.cs`）：APIv3 的业务失败是 HTTP 4xx/5xx + `{"code","message"}`，**没有整数 errcode**——`Code` / `Message` 为官方字符串码与描述，成功响应两字段恒缺省，故 `IsSuccess => string.IsNullOrEmpty(Code)`；实现叶层契约 `IWechatApiResponse`（`ErrorCode` 成功 `0` / 失败 `-1` 哨兵、`ErrorMessage`），使跨产品线单点判错与 `WechatPayException.ThrowIfFailed` 成立。
- **回调事件键常量**（`Callback/WechatPayNotification.cs`）：`WechatPayNotificationEventTypes` **13** 个（`TRANSACTION.SUCCESS`；`REFUND.SUCCESS` / `ABNORMAL` / `CLOSED`；`PAYSCORE.USER_PAID` / `USER_CONFIRM` / `USER_OPEN_SERVICE` / `USER_CLOSE_SERVICE`；`FAPIAO.USER_APPLIED` / `ISSUED` / `CARD_INSERTED` / `REVERSED` / `CARD_DISCARDED`）、`WechatPayNotificationOriginalTypes`（`transaction` / `refund` / `profitsharing` / `payscore`）与发票状态值域（`FapiaoStatuses` / `FapiaoCardStatuses`）。
- **`Generated/`**（**84 个上下文文件**，与 84 个 `SerializerClassName` 组一一对应）：由 `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）按标注生成的 AOT 源序列化上下文（`<auto-generated>`，勿手改），禁止运行时反射序列化。**跑该脚本须显式带 `-TargetProject Src/Pay/Mud.Wechat.Pay/Mud.Wechat.Pay.csproj -OutputDir Src/Pay/Mud.Wechat.Pay/Generated`**（脚本默认落企微线 DataModels，用默认值重跑只会刷新 Work 侧、支付线上下文原地不动，漏登记只以 `AOT006` 暴露）。

## 用法

DTO 是纯数据面：宿主只负责填字段，序列化由声明式客户端经源生成 `JsonTypeInfo` 完成。

```csharp
// 请求模型：字段名与官方报文的对应关系由 [JsonPropertyName] 固定（勿改属性名去「凑」snake_case）
var refundRequest = new RefundApplyRequest
{
    TransactionId = transactionId,
    OutRefundNo = outRefundNo,                     // 商户退款单号：幂等键（重复提交报 OUT_TRADE_NO_USED）
    Reason = "用户取消",
    NotifyUrl = "https://merchant.example.com/pay/1900000001",
    Amount = new RefundApplyAmount { Total = 100, Refund = 100, Currency = "CNY" },   // 单位为分
};

// 响应模型：统一继承 WechatPayResponse，判错走 Code / Message（字符串码，非整数 errcode）
RefundResponse response = await refund.CreateRefundAsync(refundRequest, ct);
WechatPayException.ThrowIfFailed(response);
if (!response.IsSuccess)
{
    // response.Code == "REFUND_FEE_INVALID" / "INVALID_REQUEST" 等，见 WechatPayErrorCodes 常量表
}
```

回调载荷模型由回调包的类型化访问器解析（宿主不手动反序列化）：

```csharp
WechatPayTransactionResource? transaction = context.GetTransaction();   // 解密后的 resource
string? outTradeNo = transaction?.OutTradeNo;                            // 幂等落库键
```

## 说明

- **字段名一律照官方原文**：`snake_case` 请求/响应字段逐属性显式 `[JsonPropertyName]`，与 `PAY-B5` 守卫的「路由 + 字段名照抄官方」断言同源；生成上下文另带 `PropertyNameNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower` 与 `PropertyNameCaseInsensitive = true` 兜底（大小写不敏感读取是官方应答字段与文档拼写存在出入时的实际救命项）。
- `SerializerClassName` = 域组名（**84 个组**：`Common` / `Transactions` / `Refund` / `Bill` / `Certificates` / `ProfitSharing` / `PayScore` / `PartnerPayScore` / `BankPayScore` / `Transfer` / `PartnerTransfer` / `BankTransfer` / `Repayment` / `Callback` / `Apply4SubMerchant` / `Apply4Subject` / `PartnerTransactions` / `MerchantRiskManage` / `MerchantStore` / `MerchantAlterApply` / `MerchantFund` / `MerchantSettlement` / `MerchantService` / `MerchantManage` / `MerchantMedia` / `MerchantOperationManage` / `Ecommerce` / `MarketingFavor` / `MarketingBusifavor` / `MarketingPartnerProductCoupon` / `MarketingPartnerDeliveryPlan` / `MarketingGoodsSubsidyActivity` / `MarketingPayGiftActivity` / `MarketingShoppingReceipt` / `MarketingPartnerships` / `MarketingBank` / `MarketingBankFavor` / `IndustryCoupon` / `BrandApplyment` / `BrandCardConfig` / `BrandCardLink` / `BrandCardMember` / `BrandProfitSharing` / `BrandStore` / `PasswordExemptContract` / `AbnormalFundProcessing` / `BrandRedpacket` / `TransferToQqWallet` / `UserConfirmAuthorization` / `PlatsolutionEcommerceInsuranceContract` / `PlatsolutionEcommerceCompensate` / `PlatsolutionEcommerceDeposit` / `PlatsolutionEcommerceInsuranceClaim` / `PlatsolutionEcommerceMchTransfer` / `PlatsolutionEcommerceRechargeEmployee` / `PlatsolutionEcommerceRecharge` / `PlatsolutionEcommerceSettle` / `PlatsolutionInsuranceReservation` / `PlatsolutionInsuranceBatch` / `PlatsolutionInsuranceReceipt` / `WebizpayBill` / `WebizpayEmployee` / `WebizpayStore` / `WebizpayUser` / `Papay`（委托代扣 3.0 直连与服务商两支同组）/ `Vehicle` / `TaxiInvoice` / `EducationPapay` / `EducationSchoolPay` / `PayrollCard` / `BusinessCircle` / `Catering` / `FundsToOversea` / `GoldPlan` / `Multiuse` / `PayInstallment` / `RateActivity` / `Recharge` / `SmartGuide` / `TransactionBlock` / `Capital` / `Lovefeast` 与命名空间末段同；合单组为 `CombineTransactions`、发票组为 `NewTaxControlFapiao`，其命名空间末段分别是 `Combine` / `Fapiao`），生成的上下文类名即 `{组名}JsonContext`、与其域 DTO 同命名空间；**域分组全量清单以 `Generated/` 目录为准**，本文不逐一维护。
- 生成上下文以 `#if NET8_0_OR_GREATER` 门控（低 TFM 为反射 STJ，AOT 门禁仅对 net8.0+ 构建）。
- 新增/变更 DTO 后的流程：`scripts/AddHttpJsonSerializable.ps1` 标注 → `scripts/GenerateJsonContext.ps1` 重新生成 → 提交 `Generated/` 变更；未登记的标注类型被组件分析器 `AOT006` 拦下。**新增域或回调载荷时还须同步两处消费点**：主包 `PayJsonResolverExtensions` 的上下文合并清单，以及回调包手写的 `WechatPayCallbackJsonContext`。
- 回调载荷类型**同时**出现在 `Callback/`（本包）与回调包的类型化访问器（`WechatPayCallbackContext.GetTransaction()` 等）中：模型在本包、`JsonTypeInfo` 登记在回调包——载荷属数据面，中间件不重复声明。
- 生成上下文为 `internal`，经 `InternalsVisibleTo` 供 `Mud.Wechat.Pay`、`Mud.Wechat.Pay.Abstractions`、`Mud.Wechat.Pay.Callback`、`Mud.Wechat.Pay.Tests` 直读（合并与契约守卫断言）。
- DTO 均走 `JsonTypeInfo` 源生成序列化，勿以反射方式调用 `JsonSerializer.Serialize<T>/Deserialize<T>`（`PAY-B6` 源码扫描锁定无反射序列化、无反射发现）。
- 模型面刻意窄于官方全域：未建模域（如 `/v3/merchant/fund` 商户资金）由 `WechatPayCapitalRulingContractGuards` 记录裁决理由，而非留一堆空 DTO。

## 依赖

`Mud.Wechat.Abstractions`（叶层，仅取 `IWechatApiResponse` 响应契约）+ `Mud.HttpUtils.Attributes` 3.0.9（零依赖特性包）+ `Mud.HttpUtils.Generator` 3.0.9（仅分析器，`PrivateAssets=all`）。

目标框架 `net6.0;net8.0;net10.0`（支付线两包同集的受控例外，完整理由见本工程 csproj 的 `TargetFrameworks` 注释与守卫 `PAY-B9` / `AB-G8`）。


---

## 原 `Mud.Wechat.Pay.V2` 包面（2026-10 包合并并入本工程，命名空间不变）

微信支付 **APIv2** SDK **主包**：XML 报文 + 报文体内 `sign`（MD5 / HMAC-SHA256）+ 商户 API 证书双向 TLS 的传输基座，以及 **14 个模块 / 58 支端点**（57 条不同路径，`/papay/deletecontract` 为商户与服务商两族共用）。

> **与 APIv3 主包（`Mud.Wechat.Pay`）是两条独立传输线**：APIv3 走 JSON + `Authorization: WECHATPAY2-SHA256-RSA2048 …` 头签名；APIv2 走 XML + **报文体里的 `sign`**，业务失败也用 HTTP 200 + `return_code=FAIL` 表达。两线**不共用** DTO、序列化器、命名客户端与契约守卫，也**不共用密钥**（`ApiV2KeySecretName` 与 `ApiKeySecretName` 是两个字段、两把值）。

## 内容

- **`Interfaces/Pay/IWechatPayV2Service.cs`** — 声明式基础支付 8 端点（`[HttpClientApi(RegistryGroupName = "BasePayV2", HttpClient = IWechatPayV2HttpClient)]` + `[AllowAnyStatusCode]`）：
  `/pay/unifiedorder` 统一下单、`/pay/micropay` 付款码支付、`/pay/orderquery` 查单、`/pay/closeorder` 关单、`/pay/refundquery` 退款查询、`/pay/refundqueryv2` 退款查询 v2、`/pay/settlementquery` 结算查询、`/pay/queryexchagerate` 汇率查询（官方原文即少一个 `t`，照抄）。
- **`Interfaces/Secure/IWechatPayV2SecureService.cs`** — 需双向证书的 5 端点（**无** `[HttpClientApi]`，手写通道服务）：
  `/secapi/pay/refund`、`/secapi/pay/refundv2`、`/secapi/pay/reverse`、`/pay/downloadbill`、`/pay/downloadfundflow`。后两支成功应答是 **CSV 原文** ⇒ 返回 `Task<string>`、不做应答验签。
- **`Transport/`** — `WechatPayV2ContentSerializer`（DTO ⇄ XML，源生成 `PayV2JsonContext` 驱动）、`WechatPayV2XmlCodec`（扁平字段表 ⇄ `<xml>`）、`WechatPayV2SigningHandler`（出站补 `sign` + 入站验签）、`WechatPayV2HttpClient` / `WechatPayV2HttpClientNames`。
- **`Security/`** — `WechatPayV2SignatureBuilder`（stringA 规范化 + MD5 / HMAC-SHA256 + 验签）、`WechatPayV2Signer`（算法判定与应答双试）。
- **`Credential/`** — `IWechatPayV2CredentialProvider` / 默认实现：经 `ISecretProvider` 取 APIv2 签名密钥（校验 32 字节）与 `apiclient_cert.p12`（口令默认商户号，可另配密钥名）。
- **`Secure/`** — `WechatPayV2SecureChannelProvider` / `WechatPayV2SecureChannel`：逐商户一条独立 `HttpClient`（含独立连接池）承载双向 TLS。
- **`Extensions/`** — `PayV2Module` 枚举 + `PayV2ServiceBuilder` + `AddWechatPayV2Api()` 入口。

## 域清单（模块 ≡ 注册组 ∪ 手写族名单）

| 模块 | 端点 | 形态 | 域名 |
|---|---|---|---|
| `BasePayV2` | 13 = 8 声明式 + 5 证书 | 混合 | `api.mch` / `api2.mch` |
| `PapayV2` | 6 | 声明式 | `api.mch` |
| `PartnerPapayV2` | 5 | 声明式 | `api.mch` |
| `RedPackV2` | 5 | **手写证书族** | `api.mch` |
| `TransferV2` | 4 | **手写证书族**（含企微双签两支） | `api.mch` |
| `PartnerTransferV2` | 2 | **手写证书族** | `api.mch` |
| `DepositV2` | 8 = 5 声明式 + 3 证书 | 混合 | `api.mch` / `api2.mch` |
| `CustomsV2` | 3 | 声明式（**无** `nonce_str`） | `api.mch` |
| `ToolsV2` | 2 | 声明式 | `api.mch` |
| `RiskV2` | 1 | **手写证书族**（请求**无** `appid`） | `api.mch` |
| `SubMerchantConfigV2` | 3 | **手写证书族** | `api.mch` |
| `MerchantMicroV2` | 2 | **手写证书族**（敏感字段须加密） | `api.mch` |
| `InstitutionSubV2` | 3 | **手写证书族**（境外，仅 MD5） | `apihk.mch` |
| `MerchantMediaV2` | 1 | **手写证书族**（本线唯一 `multipart/form-data`） | `apihk.mch` |

## 接入

```csharp
services.AddPayApp(configuration, "WechatPayMerchants")   // 凭据底座（两线共用同一份商户配置）
        .AddWechatPayApi(builder => builder.AddAllApis())  // APIv3（可选）
        .AddWechatPayV2Api(builder => builder.AddAllApis()); // APIv2
```

`AddWechatPayV2Api` 在 `AddPayApp` **之后**调用，否则注册期即抛（`IWechatPayMerchantManager` 未注册）。

## 已踩陷阱（改动前必读）

1. **签名放 Handler，不放序列化器**：`sign` 的作用域是报文体全部非空顶层字段，必须在体成形后计算；而密钥出自异步 `ISecretProvider`，`ToHttpContent<T>` 是同步方法。两难只能在 `DelegatingHandler` 里同时解开。
2. **证书端点不能进命名客户端**：`IHttpClientFactory` 的主 Handler 按**客户端名**缓存、连接池亦按名复用 ⇒ 一个名字绑不了多商户证书，否则「商户 A 的请求跑在商户 B 的 TLS 会话上」。本线全部需证书端点（基础支付 5 支 + 红包 5 + 付款 4 + 服务商付款 2 + 押金 3 + 风控 1 + 子商户配置 3 + 小微 2 + 机构进件 3 + 媒体上传 1）一律走 `WechatPayV2SecureChannelProvider` 的逐商户通道，**别顺手给任何手写族接口补 `[HttpClientApi]`**。
3. **13 端点分两面由守卫加法锁定**：`WechatPayV2BasePaymentContractGuards` 的 `…_ShouldHaveEightEndpoints` + `…_ShouldHaveFiveEndpoints` + `…_ShouldSumToThirteen` 三处同批维护，任何一侧增减都会打红。
4. **应答验签双试**：官方应答体**不带** `sign_type`，算法实际由请求侧选定 ⇒ `WechatPayV2Signer.Verify` 对 MD5 与 HMAC-SHA256 **两种都试、任一命中即通过**（伪造仍需密钥，不削弱验签），否则会自伤成「HMAC 下单 ⇒ 应答验签必败」。应答若自带 `sign_type` 则严格按声明判。
5. **已带 `sign` 的请求不重签**（幂等放行）：字段表口径一旦与首发有差就会产出「体与 `sign` 互相矛盾」的报文，官方只回 `SIGNERROR`，比漏签更难查。回归锁：`WechatPayV2TransportPipelineTests.V2Client_ShouldNotResign_WhenBodyAlreadyCarriesSign`。
6. **`T == string` 直发原文**：CSV 下载类端点没有 XML 应答可反序列化，序列化器对该类型短路返回原始文本。
7. **未登记 DTO 即点名报错**（不回落到反射序列化）：新增 APIv2 DTO 必须标 `[HttpJsonSerializable(SerializerClassName = "PayV2")]` 并重跑 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1`。
8. **密钥轮换需重启**：APIv2 签名密钥按商户缓存（`WechatPayV2SigningHandler` / `WechatPayV2SecureService` 各一份缓存），与 APIv3 缓存签名提供者同一口径。
9. **异常消息红线**（PAY-B7 同线）：任何异常与日志**不得**出现密钥值、`sign` 值、签名原文串或证书材料，只允许商户键、配置字段名与截断后的诊断文本。
10. **SSRF 白名单是「三个编译期常量」的闭集**：`api.mch.weixin.qq.com`、`api2.mch.weixin.qq.com`、`apihk.mch.weixin.qq.com` 都已被 `WechatApiHosts` 的 `weixin.qq.com` 后缀覆盖；本线**不给**商户级 `BaseUrl` 覆盖留门。证书通道绕过 `IHttpClientFactory`，其唯一的域名闸是 `WechatPayV2SecureChannelProvider.GetChannelAsync` 里的三段 `!=` 判断 ⇒ **新增第四个域名必须同时改常量表与这段白名单**（少改一处即误拒，多放一个商户可控值即绕过防线）。
11. **「要证书」与「挂在证书域名上」是两件独立的事**：`/secapi/mch/*`（子商户配置）与 `/applyment/micro/*`（小微）要商户证书却仍挂 `api.mch`；只有 `api2.mch` 上的 `/secapi/pay/*` 与国内 `api.mch` 上的 CSV 下载构成 `SecureBaseAddress` 的适用范围；境外两族（机构进件、媒体上传）官方「接口说明」栏只印 `apihk.mch`。照前缀猜域名 ⇒ 404。
12. **`sign_type` 的「有无」与「默认值」就是算法开关**：`WechatPayV2Signer` 只在字段表带 `sign_type=HMAC-SHA256` 时走 HMAC。故 DTO 的属性初始化器（`= "HMAC-SHA256"`、`= "1.0"`）属**契约面**：清空一个必填默认值 ⇒ 报文静默降级成 MD5 被打回 `SIGNERROR`；给官方没有 `sign_type` 的那几支（支付目录两支、机构进件三支、报关三支）补一个 ⇒ 同一症状。逐型锁定见 `WechatPayV2MerchantProfileContractGuards` 与 `…CustomsToolsRiskContractGuards`。
13. **本线唯一 `multipart/form-data` 端点**是 `/secapi/mch/uploadmedia`：字段仍按 XML 的同一张表签名、**文件分节不参与签名**（完整性由**小写** MD5 的 `media_hash` 承载），而应答仍是需验签的 XML ⇒ 走 `WechatPayV2SecureInvoker.PostMultipartAsync`，不要改用 `PostAsync` 把文件拼进 XML 体。
14. **敏感字段加密只有一条路**：`WechatPayV2SensitiveFieldEncrypter` 固定 `RSAES-OAEP + SHA-1 + MGF1(SHA-1)`（官方 Java 示例 `RSAWithSHA1AndMGF1Padding`），密钥是**微信支付平台证书**公钥，输出 Base64。这与 APIv3 的 `RSAES_OAEP_256` **不同源**，混用会得到「官方报参数错误」而不是「加密失败」。密文与 `cert_sn` 必须同源：按序列号从 `IWechatPayPlatformCertificateStore` 取证书，取不到即 fail-closed（`WechatPayV2SensitiveFieldEncrypter.Encrypt(plaintext, store, serialNumber)`）。
15. **双签端点的 Secret 端口是惰性可选依赖**：`sendworkwxredpack` / `paywwsptrans2pocket` 需要企微自建应用 Secret，经 `IServiceProvider.GetService<IWechatPayV2WorkWxSecretProvider>()` 在**调用时**解析。不要把它提成 `WechatPayV2SecureInvoker` 的构造参数——那会把全部宿主绑上企微凭据桥接。
16. **用户身份与金融字段零落地**：`mobile`/`email`/`creid`/`qq`/`outerid`/`risk_info`/`re_openid`/`openid`/`desc`、`enc_bank_no`/`enc_true_name`/`bank_no_md5`/`true_name_md5`、报关的 `cert_id`/`name`、小微的 `account_number`/`mobile_phone`/`email`（无论明文密文）、`violation_records` 原文、`sandbox_signkey` 与 `getpublickey` 返回的公钥材料——一律**不进日志、不入库、不入异常消息**（PAY-B7 同线口径）。

## 契约与守卫

- `WechatPayV2BasePaymentContractGuards` — 基础支付 13 支（8 + 5）、注册组、注入客户端、与 APIv3 的程序集隔离、`PayV2Module ≡ 注册组 ∪ 手写族名单` 总不变式。
- `WechatPayV2PapayContractGuards` — 委托代扣双族 11 支。
- `WechatPayV2CertFamilyContractGuards` — 整族手写的红包 / 企业付款 / 服务商付款（11 支）+ `HandWrittenModules` 名单（8 项）+ DI 单例与模块隔离。
- `WechatPayV2DepositContractGuards` — 押金「一域两形态」（5 声明式 + 3 证书）。
- `WechatPayV2CustomsToolsRiskContractGuards` — 报关 3 + 工具 2 + 风控 1 的例外事实（无 `nonce_str`、请求无 `appid`、官方不写请求方式）。
- `WechatPayV2MerchantProfileContractGuards` — 子商户配置 3 + 小微 2 + 机构进件 3 + 媒体上传 1：域名二分、`sign_type` 逐型、`app_id` vs `appid`、multipart 出口、SSRF 三常量闭集、加密器算法与 fail-closed。
- `Tests/Mud.Wechat.Pay.Tests/Transport/WechatPayV2TransportPipelineTests.cs` — 真实 DI 管道：`sign` 独立复算（MD5 / HMAC）、幂等放行、应答验签双向前置、跨 scope 单例、底座顺序 fail-fast、证书缺失 fail-fast。


---

## 原 `Mud.Wechat.Pay.V2.DataModels` 包面（2026-10 包合并并入本工程，命名空间不变）


微信支付 **APIv2 线**的官方 DTO 包：`PayV2/` 下按功能族分文件的请求/响应 DTO
（统一下单、付款码、查单关单、退款、红包、转账、企业付款、海关、分账、风控、代扣、行业、
车辆、账单评论等 **27 个功能族文件**），仅被 `Mud.Wechat.Pay.V2` 主包消费。

## 序列化形态（改前先读）

- 业务报文是 **XML**（`WechatPayV2XmlCodec` 扁平字段表 ⇄ `<xml>`；报文体内 `sign` 由
  `WechatPayV2SigningHandler` 出站补签 / 入站验签），**不是** JSON——DTO 层面没有
  `JsonPropertyName` 之类的 JSON 映射诉求。
- 但 DTO 仍统一标注 `[HttpJsonSerializable(SerializerClassName = "PayV2")]` 并生成
  `Generated/PayV2JsonContext.g.cs`：这是 **AOT 净零的统一管线**（`Mud.HttpUtils` 的
  `[HttpClientApi]` 内容序列化基建要求所有 DTO 面走源生成上下文，`IL2026`/`IL3050` 不因
  线的报文编码不同而豁免）。生成物**提交进版本控制、勿手改**；加 DTO 后重跑
  `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`。
- 命名空间恒为 `Mud.Wechat.Pay.V2.DataModels`（`PayV2/` 子目录仅作组织）。

## 与 APIv3（`Mud.Wechat.Pay.DataModels`）的隔离理由（勿「顺手合并」）

- 两线**不共用** DTO、序列化器、命名客户端与契约守卫，也**不共用密钥**（`ApiV2KeySecretName`
  与 `ApiKeySecretName` 是两个字段、两把值）——合并即让两条传输线的 DTO 面互相进入彼此的
  JsonContext 登记与审计面（`audit-config-keys.ps1` / AB-G6 按 DTO 登记表双向不变式）。
- V2 大量响应是「`return_code` + 业务结果混合体」，字段命名与 V3 的 JSON 信封完全不同族；
  共享包会制造大量同名异义类型（如 `RefundQuery` 在两线字段集不同）。
- 依赖方向：本包只引 Core 叶层（`Mud.Wechat.Abstractions`），V2 主包引本包 + V2 Abstractions，
  与全仓 `主包 → {Abstractions, DataModels}` 家族形态一致。

## 守卫索引

支付线契约守卫见 `Tests/Mud.Wechat.Pay.Tests/`（DTO 上下文登记断言随域守卫同批维护）。

