# Mud.Wechat.Pay.DataModels

微信支付 **APIv3** SDK **数据模型包**——官方接口出入参与回调载荷模型（548 个 `[HttpJsonSerializable]` 类型），独立交付以避免宿主引入不必要的运行时依赖。

> 与**企业微信**线的 `Mud.Wechat.Work.DataModels`（其中 `Pay` / `PayTool` 子域是企微「企业支付」「收银台」，走 `access_token`）分属两条产品线；本包模型对应 `https://api.mch.weixin.qq.com` 的 APIv3 报文，无令牌概念。

## 内容

- **域 DTO**（29 个域目录，共 **548** 个 `[HttpJsonSerializable]` 类型，均标注 `SerializerClassName`）：
  - `Common/` 1 —— 响应基底 `WechatPayResponse`
  - `Transactions/` 27 —— 直连下单四族（JSAPI/小程序、Native、APP、H5）请求与应答（JSAPI 与 APP 共用 `PrepayIdResponse`；含 `JsapiAmountInfo` / `JsapiPayerInfo` / 商品明细 / 场景 / 门店 / 结算信息、`H5SceneInfo`+`H5Info`、`AppSubsidyInfo` 补贴族）、查单应答、关单请求、小程序调起支付五参数
  - `Refund/` 8、`Bill/` 1、`Certificates/` 3、`Transfer/` 7
  - `ProfitSharing/` 12、`PayScore/` 29、`Combine/` 22（命名空间 `…DataModels.Combine`）、`Fapiao/` 18（命名空间 `…DataModels.Fapiao`）、`MarketingFavor/` 22
  - `Apply4SubMerchant/` 28 —— 特约商户进件（申请单 + 主体/证件/经营/结算四族请求构件 + 结算账户三面应答 + 官方枚举常量集）
  - `Apply4Subject/` 14 —— 主体进件（申请单 + 主体/证件/联系人/法定代表人/受益人构件 + 三面应答 + 官方枚举常量集）
  - `PartnerTransactions/` 30 —— 服务商基础支付 + 分期（下单五族请求 + 三支应答、查单应答及其促销/商品明细构件、关单请求、分期曝光咨询四件，以及**按族分列**的三套 `scene_info` 与两套 `store_info`、两套 `settle_info`；`PartnerTransactionConstants` 为 6 组官方枚举常量集，不标注序列化）
  - `PartnerPayScore/` 49 —— 服务商支付分（授权五族请求/应答 + 服务订单创建/修改/同步/取消与查询应答 + 共享构件 `PostPayment`/`PostDiscount`/`TimeRange`/`Location`/`Device`/`RiskFund` + 支付分计划与用户签约计划四族 + 三支 204 端点专用请求；`PartnerPayScoreConstants` 等 15 组官方枚举常量集，不标注序列化。**字段名与直连支付分域不通用**：服务商面用 `appid`/`mchid` 而非 `sp_*`，且 `total_original_price`（计划应答）与 `total_origin_price`（签约计划）是官方原文的两个不同拼写）
  - `BankPayScore/` 24 —— 银行 / 从业机构支付分（预授权与授权记录 + 服务订单创建/查询/取消/完结/修改/同步的请求与两支应答 + 扣款信息登记/查询两支应答 + 共享构件 `PostPayment`/`PostDiscount`/`TimeRange`/`Location`/`RiskFund`/`Collection` 族 + 促销明细两件；`BankPayScoreConstants` 等 11 组官方枚举常量集，不标注序列化。**与相邻两支付分域均不通用**：本域在 `appid`/`mchid` + `sub_*` 之外多一层 `channel_id`（请求侧处处必填、预授权应答降为选填），失效码是 `NO_AUTH` 而非服务商面的 `NOAUTH`，查询应答**无** `need_collection`（服务商面有且固定 `true`）⇒ 两域应答不可共用类型）
  - `Callback/` 22 —— 通知信封 `WechatPayNotification` / `WechatPayNotificationResource` + 各域解密后 `resource` 载荷
  - `PartnerTransfer/` 15 —— 服务商批量转账到零钱（发起请求 / 受理应答 + 批次查询应答与批次内明细构件 + 明细输入与明细查询应答；`PartnerTransfer*` 等 **9** 组官方枚举常量集，不标注序列化。**与直连转账域字段名不通用**：本域是 `total_num`/`success_num`/`fail_num` 与 `batch_status`(5)/`detail_status`(6)，直连与银行面是 `*_count` 与 `*_state`；明细请求姓名 `user_name`、应答姓名 `username`，两处不同名且皆 RSA 密文；明细应答**无** `sub_mchid`）
  - `BankTransfer/` 18 —— 从业机构（银行）批量转账与电子回单（转账请求/应答五件 + 明细输入与明细应答 + 汇总与明细回单各「请求 + 应答」四件；`BankTransfer*` 等 **8** 组官方枚举常量集，不标注序列化。**六层商户标识**：`platform_mchid` / `specialized_payment_mchid` / `sponsor_mchid` / `user_authorized_mchid` / `user_authorized_appid` + 应答 `bank_sp_mchid`，本域**无** `sp_*`/`sub_appid`；状态 `batch_state`(4) / `detail_state`(4) 无 `WAIT_PAY`/`INIT` 档；回单应答的 `hash_type`/`hash_value`/`download_url` 仅 `FINISHED` 出现 ⇒ `download_url` 不落模型）
  - `Repayment/` 20 —— 平台收付通·还款支付（下单请求 + 子单/金额/结算/实名/四场景对象七件 + 下单应答；查单应答 + 支付者/子单/金额三件；关单请求 + 关单子单；`RepaymentIdentityTypes` / `RepaymentH5SceneTypes` / `RepaymentTradeStates` **3** 组常量，不标注序列化。**请求与应答同名不同表**：`combine_payer_info` 请求侧是 `identity` 三节密文、应答侧只有 `openid`；子单金额请求 2 字段、应答 4 字段；`trade_state` 本域只 3 值（服务商基础支付 7 值）；字段名含数字打头段 `h5_info` / `h5_url`）
  - `MerchantRiskManage/` 21 —— 商户风险管理（风险订单同步的「服务商 / 普通商户」两形态各一对请求与应答 + 风险共建三族：订单关联信息上报 / 风险信息查询 / 处理结果回传 + 违规通知回调地址四件套 `notify_url` 请求与应答；`MerchantRiskManageTradeRiskTypes`(5) / `MerchantRiskManagePunishTypes`(10) **2** 组常量，不标注序列化。**四套商户标识并存**：`mchid`（普通商户面）/ `sp_mchid`+`sub_mchid`（服务商面）/ `sp_mchid`+`acquiring_bank_id`+`channel_id`+`sub_mchid`（从业机构与渠道面）/ `sub_orders[].mch_id`（合单子单，官方此处**带下划线**）；同名 `risk_type` 有两套类型（风险订单同步面 `integer` 1~4、风险共建面 `string` 5 值）⇒ 两族常量与 DTO 各一套；官方两处误拼 `risk_discove_time` / `same_deviced` 照页保留（守卫 MRB-B3 同时断言「不得写成正确拼写」）
  - `MerchantStore/` 14 —— 商户门店管理（创建请求 + 受理应答、更新请求（**只含 `store_basics` / `store_address` / `store_business` 三棵子树，无 `store_recipient`**）、查询应答（四棵 + `store_state`）、收款绑定与解绑两请求 + 一共用应答 + 失败明细构件；`MerchantStoreStateValues`(6) **1** 组常量，不标注序列化。**十页共用一套模型**：商户面与服务商面字段表只差一个 `sub_mchid`、应答表零差异 ⇒ 无 `Partner` 分身类型（守卫 MST-B6）；`store_id` 官方 `integer` 而 SDK 以 `long?` 承载模型字段、path 参数按线内惯例是 `string`；经纬度 `longitude`/`latitude` 官方即 **string**；`brand_id` `long?`；绑定与解绑**请求分建两型、应答共用一型**（逐项回 `failed_reason`）
  - `MerchantFund/` 1 —— 商户账户余额（实时余额与日终余额**共用一个应答类型**，两键 `available_amount` / `pending_amount` 皆 `long?`（官方 integer 单位分，可越出 int32）；`MerchantFundAccountTypes`(3) **1** 组常量。**同前缀的提现族四支属【平台商户】面 ⇒ 零端点**，本包因此没有提现相关类型（守卫 MF-B2 与裁决守卫 C1 互为正对照））
  - `MerchantSettlement/` 2 —— 商户结算规则申请（创建请求四字段 + 两支共用应答八键；`MerchantSettlementApplicationStates`(3) **1** 组常量。**从业机构面标识**：`acquiring_bank_id` / `channel_id` / `sub_mchid` 三层，本包**无** `sp_*` / `sub_appid`；`settle_rule_id` 官方整型 ⇒ 两侧同 `long?`；`application_process_info` 是 `string(256)` 描述串、官方未给结构 ⇒ 不做子树建模）
  - `MerchantAlterApply/` 17 —— 商户资料变更（主体 / 超级管理员 / 商家简称三族各「提交请求 + 查询应答」，加撤销与驳回明细共用件；`MerchantAlterApply*` **9** 组官方枚举常量集，不标注序列化。**共用/分建逐类型裁定**：三族提交应答同表 ⇒ 共用一型；超管与简称的撤销同表 ⇒ 共用、主体撤销回 `{result}` 分建；三族查询应答**各建一型**（主体族与超管族字段名逐字相同、只差 `state` 值清单；简称族另有 `sub_mchid` 且**无** `audit_reject_detail`）；驳回明细项类型两族共用。**证件字段两套体系不互换**：本域 `card_*` / `id_doc_*` 且 `license` 美式拼写，进件域才是 `identification_*` / `licence`；官方误拼 `EDITTING`（三 T）/ `MODIFING`（单 F）照页保留（守卫 MAA-B3/B4））
  - `MerchantService/` 34 —— 商户消费者投诉（通知地址 CRUD + 投诉列表 / 详情 / 协商历史 + 回复 / 完成处理 / 退款审批 / 即时服务的请求与应答及其嵌套构件；`MerchantService*` **14** 组官方枚举常量集，不标注序列化。图片上传（multipart）与拉取（二进制流）两支走手工通道 ⇒ 本目录无上传表单 DTO）
  - `MerchantManage/` 5 —— 商户管理记录（查询单条应答 + 提交资料请求（`submit_data` 官方是 **`string(4000000)` 的 JSON 串、不是对象**）+ 按 `sub_mchid` 分页查询应答与构件；`MerchantManageConstants` **1** 组。上传管理记录文件走手工通道）
  - `MerchantMedia/` 1 —— 商户媒体上传应答（`media_id` 单键）；上传与下载两支都是手工通道 ⇒ 本目录只有这一个响应类型
  - `MerchantOperationManage/` 2 —— 商户运营管控情况查询（应答 + 限制项构件；`MerchantOperationManageConstants` **1** 组。同前缀尽调族属【平台商户】面 ⇒ 零建模，守卫 MOM-B2）
  - `Ecommerce/` 104 —— 平台收付通（**八个子域共用一个注册组**：账户 / 进件 / 账单 / 资金 / 分账与补差 / 退款 / 商家转账 / 代扣合单）；`Ecommerce*` **58** 组官方枚举常量集。**共用 / 分建逐族裁定**：退款族三支「二级商户面 / 个人收款方面」同方法同路由 ⇒ 请求与应答各分建两型；余额两支、提现查询两支、进件两路查询、注销提现查询两支、普通注销「申请 + 查询」、分账「请求 + 查询」与回退单「请求 + 查询」、回补两支、接收方「添加 + 删除」、转账授权两支、代扣下单与查单各**共用一型**（判据是官方字段表，不是页面数）。**两支注销是两套契约**：`apply-cancel-withdraw`（状态九值）与 `cancel-applications`（三值）单据定位键、状态枚举、应答表全不同 ⇒ 不得互相套用。密文键（`account_name` / `account_number` / `identification_name` / `identification_no` / `cert_number` / `user_name` / `receiver_name` / `bank_account` / `real_name` / `id_card_*`）一律 `string` 承载、SDK 不代为加解密；应答的 `real_name_auth_result.access_token` 是**不可刷新凭据** ⇒ 禁进日志（守卫 EC-A3 / EC-C1 / EC-M3 / EC-R6 / EC-X4）
- **响应基底**（`Common/WechatPayResponse.cs`）：APIv3 的业务失败是 HTTP 4xx/5xx + `{"code","message"}`，**没有整数 errcode**——`Code` / `Message` 为官方字符串码与描述，成功响应两字段恒缺省，故 `IsSuccess => string.IsNullOrEmpty(Code)`；实现叶层契约 `IWechatApiResponse`（`ErrorCode` 成功 `0` / 失败 `-1` 哨兵、`ErrorMessage`），使跨产品线单点判错与 `WechatPayException.ThrowIfFailed` 成立。
- **回调事件键常量**（`Callback/WechatPayNotification.cs`）：`WechatPayNotificationEventTypes` **13** 个（`TRANSACTION.SUCCESS`；`REFUND.SUCCESS` / `ABNORMAL` / `CLOSED`；`PAYSCORE.USER_PAID` / `USER_CONFIRM` / `USER_OPEN_SERVICE` / `USER_CLOSE_SERVICE`；`FAPIAO.USER_APPLIED` / `ISSUED` / `CARD_INSERTED` / `REVERSED` / `CARD_DISCARDED`）、`WechatPayNotificationOriginalTypes`（`transaction` / `refund` / `profitsharing` / `payscore`）与发票状态值域（`FapiaoStatuses` / `FapiaoCardStatuses`）。
- **`Generated/`**（30 个上下文文件）：由 `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）按标注生成的 AOT 源序列化上下文（`<auto-generated>`，勿手改），禁止运行时反射序列化。

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
- `SerializerClassName` = 域组名（25 个组：`Common` / `Transactions` / `Refund` / `Bill` / `Certificates` / `ProfitSharing` / `PayScore` / `PartnerPayScore` / `BankPayScore` / `Transfer` / `PartnerTransfer` / `BankTransfer` / `Repayment` / `Callback` / `MarketingFavor` / `Apply4SubMerchant` / `Apply4Subject` / `PartnerTransactions` / `MerchantRiskManage` / `MerchantStore` / `MerchantAlterApply` / `MerchantFund` / `MerchantSettlement` 与命名空间同末段；合单组为 `CombineTransactions`、发票组为 `NewTaxControlFapiao`，其命名空间末段分别是 `Combine` / `Fapiao`），生成的上下文类名即 `{组名}JsonContext`、与其域 DTO 同命名空间；**域分组全量清单以 `Generated/` 目录为准**，本文不逐一维护。
- 生成上下文以 `#if NET8_0_OR_GREATER` 门控（低 TFM 为反射 STJ，AOT 门禁仅对 net8.0+ 构建）。
- 新增/变更 DTO 后的流程：`scripts/AddHttpJsonSerializable.ps1` 标注 → `scripts/GenerateJsonContext.ps1` 重新生成 → 提交 `Generated/` 变更；未登记的标注类型被组件分析器 `AOT006` 拦下。**新增域或回调载荷时还须同步两处消费点**：主包 `PayJsonResolverExtensions` 的上下文合并清单，以及回调包手写的 `WechatPayCallbackJsonContext`。
- 回调载荷类型**同时**出现在 `Callback/`（本包）与回调包的类型化访问器（`WechatPayCallbackContext.GetTransaction()` 等）中：模型在本包、`JsonTypeInfo` 登记在回调包——载荷属数据面，中间件不重复声明。
- 生成上下文为 `internal`，经 `InternalsVisibleTo` 供 `Mud.Wechat.Pay`、`Mud.Wechat.Pay.Abstractions`、`Mud.Wechat.Pay.Callback`、`Mud.Wechat.Pay.Tests` 直读（合并与契约守卫断言）。
- DTO 均走 `JsonTypeInfo` 源生成序列化，勿以反射方式调用 `JsonSerializer.Serialize<T>/Deserialize<T>`（`PAY-B6` 源码扫描锁定无反射序列化、无反射发现）。
- 模型面刻意窄于官方全域：未建模域（如 `/v3/merchant/fund` 商户资金）由 `WechatPayCapitalRulingContractGuards` 记录裁决理由，而非留一堆空 DTO。

## 依赖

`Mud.Wechat.Abstractions`（叶层，仅取 `IWechatApiResponse` 响应契约）+ `Mud.HttpUtils.Attributes` 3.0.3（零依赖特性包）+ `Mud.HttpUtils.Generator` 3.0.3（仅分析器，`PrivateAssets=all`）。

目标框架 `net6.0;net8.0;net10.0`（支付线四包同集的受控例外，理由见 `Mud.Wechat.Pay.Abstractions/README.md` 与守卫 `PAY-B9`）。
