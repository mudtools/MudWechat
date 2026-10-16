# Mud.Wechat.Pay.DataModels

微信支付 **APIv3** SDK **数据模型包**——官方接口出入参与回调载荷模型（1704 个 `[HttpJsonSerializable]` 类型），独立交付以避免宿主引入不必要的运行时依赖。

> 与**企业微信**线的 `Mud.Wechat.Work.DataModels`（其中 `Pay` / `PayTool` 子域是企微「企业支付」「收银台」，走 `access_token`）分属两条产品线；本包模型对应 `https://api.mch.weixin.qq.com` 的 APIv3 报文，无令牌概念。

## 内容

- **域 DTO**（**47 个域目录**，共 **1704** 个 `[HttpJsonSerializable]` 类型，均标注 `SerializerClassName`；**下列每目录数字为序列化型数**，官方枚举常量集不进该计数、在括注里另计）：
  - `Common/` 1 —— 响应基底 `WechatPayResponse`
  - `Transactions/` 27 —— 直连下单四族（JSAPI/小程序、Native、APP、H5）请求与应答（JSAPI 与 APP 共用 `PrepayIdResponse`；含 `JsapiAmountInfo` / `JsapiPayerInfo` / 商品明细 / 场景 / 门店 / 结算信息、`H5SceneInfo`+`H5Info`、`AppSubsidyInfo` 补贴族）、查单应答、关单请求、小程序调起支付五参数
  - `Refund/` 8、`Bill/` 1、`Certificates/` 3、`Transfer/` 7
  - `ProfitSharing/` 12、`PayScore/` 29、`Combine/` 22（命名空间 `…DataModels.Combine`）、`Fapiao/` 18（命名空间 `…DataModels.Fapiao`）、`MarketingFavor/` 41 —— 代金券：批次创建/激活/暂停/重启/查询与列表 + 用户券发放/查询/列表 + 回调通知地址设置与查询 + 资金流与商户列表四支统计查询；`CouponTypes` / `CouponStockTypes` / `CouponTradeTypes` / `CouponRegionTypes` / `CouponBusinessTypes` / `CouponStockStatuses` **6** 组官方枚举常量集，不标注序列化。**同域两套路由参数命名并存**：首批查询支用 path 上的 `{openId}`（camel），P1-3 补差支照官方写 `{openid}`（snake）⇒ 模型侧不能合并成一个共用请求类型
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
  - `BrandApplyment/` 10 —— 品牌入驻（提交请求 + 其 `subject_info` / `brand_basic_info` / `admin_info` / `trademark`（含两支 `*_trademark_registration_certificate`）四族构件 + 受理应答 + 撤销请求 + 状态应答。**四支路由两两共用应答**：两支受理（提交 / 撤销）同表 ⇒ 共用 `BrandApplymentAcceptResponse`，两支查询（按 `applyment_id` / 按 `out_request_no`）同表 ⇒ 共用 `BrandApplymentStateResponse`；**共用只发生在应答侧**，请求侧因必填口径不同一律分建。本域**两套编号并存**：`applyment_id`（官方受理号）与 `out_request_no`（商户业务号），撤销请求只认前者）
  - `BrandCardConfig/` 11 —— 品牌名片配置（查询 / 申请 / 撤销 / 预览 / 发布五支各一应答 + 申请请求的 `brand_mini_program_info` / `brand_customer_service` / `service_list[]` 三棵构件 + 撤销与发布各自请求）：**申请 → 预览 → 发布是两段式**，查询支回的是「当前生效配置」、预览支回的是「待发布草稿」⇒ 两应答分建（守卫 BCC-B3）
  - `BrandCardLink/` 9 —— 交易连接名片（列表 / 申请 / 状态 / 撤销 / 解绑五支）：列表应答的 `active_link_list[]` 独立成构件；**申请（6 行）与状态（8 行）两支应答不同形 ⇒ 各建一型**，撤销与解绑亦各建一型。分页口径是 `page_index` / `page_size`，与本批会员卡两支的 `offset` / `limit` **不同形**，不得互相套用
  - `BrandCardMember/` 73 —— 会员卡与会员（模板四支 + 用户卡五支 + 预授权 + 积分 / 流水，本批最大品牌目录）：模板子树（`code_jump_information` / `valid_date_information` / `base_info` / `note_info` / `background_information` / `wallet_info` / `membership_promotion` 等）按「端点 + 请求/应答 + 路径段」机械拼装独立成类型。**模板四支（创建 / 查询 / 修改 / 作废）共用一份 49 行应答**、列表支 53 行单建；**用户卡五支（查询 / 修改 / 确认 / 作废 / 导入）共用一份 28 行应答**、列表支 32 行单建。**请求侧不分共用**：同族各支必填口径不同 ⇒ 一请求一类型
  - `BrandProfitSharing/` 15 —— 品牌分账（发起 / 查询 / 列表 / 完结 / 最大比例 / 待分金额 + 接收方添加与删除 + 回退单）：回退「请求 + 应答」与接收方「添加 + 删除」各按同表共用；发起 15 行与查询 16 行**分建**（查询多 `finish_amount` / `finish_description`）。`receivers[]` 在请求 / 应答 / 列表三处各建构件（必填口径与字段集逐表不同）。**与直连 `ProfitSharing/` 是同族不同路由**：本域在 `/v3/brand/partner/profitsharing/`，模型不复用
  - `BrandStore/` 27 —— 品牌门店（创建 / 查询 / 更新 / 列表 / 删除 / 绑定收款号 / 解绑收款号 / 打烊 / 恢复九支）：`store_basics` / `store_address` / `store_business` / `store_recipient[]` 四棵树在「写侧请求」与「读侧应答」各建一套（请求无 `store_recipient`、应答有）；创建 / 查询 / 更新**共用 21 行应答**，`close` / `resume` **共用 3 行状态应答**，绑定 5 行与解绑 1 行**分建**；**删除支官方未印发应表** ⇒ 接口返回裸 `Task`，本目录因此没有对应应答类型
  - `PasswordExemptContract/` 12 —— 免密支付（契约查询 / 解约 + 四支预签约）：**四支预签约各建一型**——`qs` 面与 `sp` 面、小程序场景与支付后场景两两正交，字段名 `appid` / `openid`（`string(128)`）与 `sp_appid` / `sp_openid`（`string(64)`）**不可互换**（守卫 PEC-B3）；契约应答的 `contract_terminate_info` 独立成构件。**与 `PayScore/`、`PartnerPayScore/` 无交集**：本域是免密协议面，令牌与签名主体为品牌商户
- **响应基底**（`Common/WechatPayResponse.cs`）：APIv3 的业务失败是 HTTP 4xx/5xx + `{"code","message"}`，**没有整数 errcode**——`Code` / `Message` 为官方字符串码与描述，成功响应两字段恒缺省，故 `IsSuccess => string.IsNullOrEmpty(Code)`；实现叶层契约 `IWechatApiResponse`（`ErrorCode` 成功 `0` / 失败 `-1` 哨兵、`ErrorMessage`），使跨产品线单点判错与 `WechatPayException.ThrowIfFailed` 成立。
- **回调事件键常量**（`Callback/WechatPayNotification.cs`）：`WechatPayNotificationEventTypes` **13** 个（`TRANSACTION.SUCCESS`；`REFUND.SUCCESS` / `ABNORMAL` / `CLOSED`；`PAYSCORE.USER_PAID` / `USER_CONFIRM` / `USER_OPEN_SERVICE` / `USER_CLOSE_SERVICE`；`FAPIAO.USER_APPLIED` / `ISSUED` / `CARD_INSERTED` / `REVERSED` / `CARD_DISCARDED`）、`WechatPayNotificationOriginalTypes`（`transaction` / `refund` / `profitsharing` / `payscore`）与发票状态值域（`FapiaoStatuses` / `FapiaoCardStatuses`）。
- **`Generated/`**（**47 个上下文文件**，与 47 个 `SerializerClassName` 组一一对应）：由 `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）按标注生成的 AOT 源序列化上下文（`<auto-generated>`，勿手改），禁止运行时反射序列化。

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
- `SerializerClassName` = 域组名（**47 个组**：`Common` / `Transactions` / `Refund` / `Bill` / `Certificates` / `ProfitSharing` / `PayScore` / `PartnerPayScore` / `BankPayScore` / `Transfer` / `PartnerTransfer` / `BankTransfer` / `Repayment` / `Callback` / `Apply4SubMerchant` / `Apply4Subject` / `PartnerTransactions` / `MerchantRiskManage` / `MerchantStore` / `MerchantAlterApply` / `MerchantFund` / `MerchantSettlement` / `MerchantService` / `MerchantManage` / `MerchantMedia` / `MerchantOperationManage` / `Ecommerce` / `MarketingFavor` / `MarketingBusifavor` / `MarketingPartnerProductCoupon` / `MarketingPartnerDeliveryPlan` / `MarketingGoodsSubsidyActivity` / `MarketingPayGiftActivity` / `MarketingShoppingReceipt` / `MarketingPartnerships` / `MarketingBank` / `MarketingBankFavor` / `IndustryCoupon` / `BrandApplyment` / `BrandCardConfig` / `BrandCardLink` / `BrandCardMember` / `BrandProfitSharing` / `BrandStore` / `PasswordExemptContract` 与命名空间同末段；合单组为 `CombineTransactions`、发票组为 `NewTaxControlFapiao`，其命名空间末段分别是 `Combine` / `Fapiao`），生成的上下文类名即 `{组名}JsonContext`、与其域 DTO 同命名空间；**域分组全量清单以 `Generated/` 目录为准**，本文不逐一维护。
- 生成上下文以 `#if NET8_0_OR_GREATER` 门控（低 TFM 为反射 STJ，AOT 门禁仅对 net8.0+ 构建）。
- 新增/变更 DTO 后的流程：`scripts/AddHttpJsonSerializable.ps1` 标注 → `scripts/GenerateJsonContext.ps1` 重新生成 → 提交 `Generated/` 变更；未登记的标注类型被组件分析器 `AOT006` 拦下。**新增域或回调载荷时还须同步两处消费点**：主包 `PayJsonResolverExtensions` 的上下文合并清单，以及回调包手写的 `WechatPayCallbackJsonContext`。
- 回调载荷类型**同时**出现在 `Callback/`（本包）与回调包的类型化访问器（`WechatPayCallbackContext.GetTransaction()` 等）中：模型在本包、`JsonTypeInfo` 登记在回调包——载荷属数据面，中间件不重复声明。
- 生成上下文为 `internal`，经 `InternalsVisibleTo` 供 `Mud.Wechat.Pay`、`Mud.Wechat.Pay.Abstractions`、`Mud.Wechat.Pay.Callback`、`Mud.Wechat.Pay.Tests` 直读（合并与契约守卫断言）。
- DTO 均走 `JsonTypeInfo` 源生成序列化，勿以反射方式调用 `JsonSerializer.Serialize<T>/Deserialize<T>`（`PAY-B6` 源码扫描锁定无反射序列化、无反射发现）。
- 模型面刻意窄于官方全域：未建模域（如 `/v3/merchant/fund` 商户资金）由 `WechatPayCapitalRulingContractGuards` 记录裁决理由，而非留一堆空 DTO。

## 依赖

`Mud.Wechat.Abstractions`（叶层，仅取 `IWechatApiResponse` 响应契约）+ `Mud.HttpUtils.Attributes` 3.0.7（零依赖特性包）+ `Mud.HttpUtils.Generator` 3.0.7（仅分析器，`PrivateAssets=all`）。

目标框架 `net6.0;net8.0;net10.0`（支付线四包同集的受控例外，理由见 `Mud.Wechat.Pay.Abstractions/README.md` 与守卫 `PAY-B9`）。
