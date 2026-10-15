# Mud.Wechat.Pay

微信支付 **APIv3** SDK **主包**：27 个业务域 34 个接口 230 端点的声明式客户端、模块注册器（`PayModule`，28 个成员）、八类非声明式服务（小程序调起签名 / 账单下载 / 发票文件上传 / 平台证书刷新 / 商户媒体上传 / 管理记录文件上传 / 投诉图片上传与拉取 / 注销资料图片上传）、AOT JsonContext 合并。

> **与企业微信线是两条不同产品线**：本包凭据是**商户 API 证书 RSA 私钥的请求签名**（`Authorization: WECHATPAY2-SHA256-RSA2048 …`），**没有 `access_token`**、四个 `Mud.Wechat.Pay*` 包**零 `[Token]` 声明**（守卫 `PAY-B1` fail-closed），BaseAddress 恒为 `https://api.mch.weixin.qq.com`。
> 企业微信的「企业支付」`AddPayApi()` 与「收银台」`AddPayToolApi()`（`Src/Work/`，走企微 `access_token`）与本包**无关**；本包入口名为 `AddPayApp()`（凭据底座）与 `AddWechatPayApi()`（业务接口）。

## 内容

- **声明式业务客户端**（`Interfaces/`，34 个接口 / 230 端点，全部落 `namespace Mud.Wechat.Pay`，实现类由 `Mud.HttpUtils.Generator` 产出到 `Mud.Wechat.Pay.Internal`）：
  - `Interfaces/Transactions/` `IWechatPayTransactionsService` — 基础交易 7 端点：JSAPI/小程序、Native、APP、H5 四族下单 + 微信订单号查单 + 商户订单号查单 + 关闭订单
  - `Interfaces/Refund/` `IWechatPayRefundService` — 退款 3 端点：申请退款、查询单笔退款、发起异常退款
  - `Interfaces/Bill/` `IWechatPayBillService` — 账单 2 端点：申请交易账单、申请资金账单（文件另走下载通道）
  - `Interfaces/Certificates/` `IWechatPayCertificatesService` — 平台证书 1 端点：获取平台证书列表（应答/回调验签与序列号轮换）
  - `Interfaces/ProfitSharing/` `IWechatPayProfitSharingService` — 分账 9 端点：接收方添加/删除、请求分账 + 查询、回退单 + 查询、解冻、待分金额查询、分账账单
  - `Interfaces/PayScore/` `IWechatPayPayScoreService` — 支付分 11 端点：服务订单创建/查询/取消/完结/修改/收款/同步 + 授权预授权/解除/按授权码或 openid 查询
  - `Interfaces/Combine/` `IWechatPayCombineService` — 合单支付 6 端点：JSAPI/Native/APP/H5 下单 + 合单查询 + 合单关单（两面路由共用，域 DTO 以服务商面建模）
  - `Interfaces/Transfer/` `IWechatPayTransferService` — 商家转账 6 端点（含「不换单重试」所需的查单/撤销/电子回单）
  - `Interfaces/Fapiao/` `IWechatPayFapiaoService` — 电子发票 5 端点：开具、查询、冲红、获取下载信息、插卡
  - `Interfaces/MarketingFavor/` `IWechatPayMarketingFavorService` — 代金券 7 端点：批次创建/启动/暂停/续期/查询 + 发券 + 券详情（**创建成功 ≠ 可发放**，须先启动批次）
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
- **模块注册器**（`Extensions/`）：`AddWechatPayApi(...)` + `PayServiceBuilder`（`AddAllApis()` / `AddModules(params PayModule[])` / `Build()`）。`PayModule` 28 个成员即各接口 `[HttpClientApi(RegistryGroupName = …)]` 的组名，**叫 `Transactions` 等而非 `Pay`**——本仓「企业支付」已占用 `WechatModule.Pay` 与生成的 `AddPayWebApiHttpClient()`，同名会跨程序集歧义。**成员数 ≠ 接口数**（两处例外）：`Ecommerce` 一个组名由八个接口共享 ⇒ 一支注册器；`MerchantMedia` 是纯手工通道、无声明式接口 ⇒ 有成员无接口。
- **非声明式服务**（官方列在 TOC 内但无 HTTP 形态，或路由/应答非 JSON；一律 `TryAddSingleton`，宿主预注册者胜出）：
  - `Transactions/` `IWechatPayMiniProgramPaySignService` — 小程序调起支付五参数（**本地 RSA 签名、无请求路由**）
  - `Download/` `IWechatPayBillDownloadService` — 账单文件下载通道（`download_url` 动态给出、应答非 JSON）
  - `Fapiao/` `IWechatPayFapiaoFileService` — 发票文件上传（`multipart/form-data` + SM3 摘要）
  - `Certificates/` `WechatPayPlatformCertificateRefresher` — 未知序列号时的按需刷新（三道防放大闸 + fail-closed）
  - `Media/` `IWechatPayMerchantMediaService` — 商户媒体上传与图片下载（`file` + `meta{filename, sha256}`，2 支通道）
  - `MerchantManage/` `IWechatPayMerchantManageFileService` — 管理记录文件上传（表单字段名是 `submission_file` + `meta`，**与媒体通道的 `file` 不同名**）
  - `MerchantService/` `IWechatPayComplaintMediaService` — 投诉图片上传（multipart）与拉取（二进制流，2 支通道）
  - `Ecommerce/` `IWechatPayEcommerceCancelMediaService` — 注销资料图片上传（`file` + `meta{file_name, file_digest}`，**键名与商户媒体通道不通用**，守卫 EC-X2 逐字锁在通道源码里；官方页同时标【普通服务商】【平台商户】）
- **AOT JsonContext 合并**（`Extensions/PayJsonResolverExtensions.cs`）：`Build()` 把 DataModels 的 30 个生成上下文经 `JsonTypeInfoResolver.Combine` 注进 `wechat-pay` 命名客户端管线。

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
- `Mud.HttpUtils` 3.0.7 + `Mud.HttpUtils.Generator` 3.0.7（分析器，`PrivateAssets=all`）
- **不引用** `Mud.Wechat.Work*` / `Mud.Wechat.OfficialAccount*`（守卫 `PAY-B11` 锁定工程引用隔离）

## 说明

- **目标框架为受控例外**：`net6.0;net8.0;net10.0`，无 `netstandard2.0`——回调 `resource` 解密强制 `AEAD_AES_256_GCM`，而 `AesGcm` 在 ns2.0 不存在；手写 GCM 与引入第三方密码学包均被否（守卫 `PAY-B9` / `AB-G8` 锁定，签名与加解密全走 BCL、AOT 安全）。
- **无 `[Token]`**：签名在传输层 `WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文。主包 csproj 仍带 `<NoWarn>MUD005</NoWarn>` 只是「防未来误加时误读」，真正的禁令由 `PAY-B1` 断言。
- **不用默认 `IEnhancedHttpClient` 实例**：走命名客户端 `wechat-pay`（`WechatPayHttpClientNames`），否则会与企微线 `qyapi.weixin.qq.com` 撞 BaseUrl 并给企微请求套上商户签名头。SSRF 白名单由 `AddPayApp` 经公用层窄入口登记，**数组零改动**（`PAY-B8`）。
- **端点/路由/字段名的权威是契约守卫**，不是本文与 `PayModule` 的 XML 注释（后者仍留有早期「首批 N 端点」口径）：`Tests/Mud.Wechat.Pay.Tests/ContractGuards/` 25 个文件——`PAY-B1`（零 `[Token]`）、`PAY-B2`（签名/验签串黄金向量）、`PAY-B3`（验签与时间窗 fail-closed）、`PAY-B4`（GCM 参数与黄金向量）、`PAY-B5`（160 端点计数 + 官方路由 + snake_case 字段照抄原文）、`PAY-B6`（无反射、无第三方密码学）、`PAY-B7`（凭据不入日志）、`PAY-B9`（TFM）、`PAY-B11`（引用隔离）+ 逐域守卫（**基础交易 TX1~TX3** / 分账 / 支付分 / 合单 / 转账 / 发票 / 代金券 / **特约商户进件 A4S-B1~B7** / **主体进件 A4SJ-B1~B7** / **服务商基础支付 PT-B1~B7** / **服务商支付分 PS-B1~B7** / **从业机构支付分 BS-B1~B7** / **服务商批量转账 PTF-B1~B7** / **从业机构转账与回单 BT-B1~B7** / **还款支付 RP-B1~B6** / **商户风险管理 MRB-B1~B7** / **商户门店管理 MST-B1~B7** / **商户资料变更 MAA-B1~B7** / **商户账户余额 MF-B1~B7** / **商户结算规则申请 MSET-B1~B7**）与 `WechatPayCapitalRulingContractGuards`（资金域未建模面裁决：**提现族四支与所得记录两支属【平台商户】面 ⇒ `/v3/merchant/fund/` 前缀下在册只有余额两支**，与 `MF-B2` 互为正对照）。
- **HTTP 动词面自 P0-7 起不再是「只有 GET/POST」**：`MerchantRiskManage` 带 PUT 与 DELETE、`MerchantStore` 带 PATCH；`MerchantAlterApply` 的「撤销」是 **POST 动词段** `/{applyId}/revoke` 而非 DELETE。声明式客户端对五支动词的支持由 MRB-B1 / MST-B1 的「具体动词特性计数 == 基类 `HttpMethodAttribute` 计数」断言锁定，**「成功应答是 204 还是 JSON」逐支断言**（同一条 `/violation-notifications` 上 PUT/POST/GET 回 200+JSON 而 DELETE 回 204；门店 PATCH 回 204 而同路由 GET 回 200；**变更域十二支零 204**）⇒ 不得按「写操作一律 204」或「同路由同应答」收口。跨域反直觉的归并判据也在此批成对固化：风险管理是**同路由分建两形态**（11 方法 / 9 条 `(方法, 路由)`），门店管理是**两页同一份表 ⇒ 共用一套方法与 DTO**（10 页 / 5 端点 / 4 条路由字符串），变更管理是**三族各四页、页页独立 ⇒ 12 页 12 支零归并**（守卫 MAA-B1）。
- **发票文件下载有意不实现**：官方文件域名 `pay.wechatpay.cn` 不在进程级白名单内，白名单零改动优先级高于该便利（`PAY-B8`）。账单 `download_url` 5 分钟有效且一次性，取到即下载。
- 新增 `[HttpJsonSerializable]` DTO 后运行 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）重新生成，并把新上下文登记进 `PayJsonResolverExtensions`；未登记类型在 Native AOT 下会因无元数据失败（JIT 下靠反射侥幸可用）。
