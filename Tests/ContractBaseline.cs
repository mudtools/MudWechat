// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.ContractBaseline;

/// <summary>
/// 契约<b>聚合基线</b>：跨域 / 跨族的<b>汇总</b>数字，集中一处维护。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么集中</b>：这些数字不描述任何单一域的契约，只承担「有没有东西悄悄多出来 / 少掉」的
/// 漂移报警。散落在各守卫里会让「加一个端点 / 一组接口」变成跨 N 个文件的机械改动，
/// 且改一处漏一处时不报错。集中后：<b>改数字只改本文件一处</b>，各守卫只断言
/// 「现场推导值 == 基线值」。
/// </para>
/// <para>
/// <b>边界（勿越界收纳）</b>：<i>域内</i>的精确断言（某域的端点数、某族的 DTO 数、某域的类型数等）
/// <b>不</b>入本文件 —— 按 AGENTS §2「守卫是权威描述」，那些是领域契约的一部分，
/// 应留在对应域守卫里就近维护。本文件只收「跨域 / 跨族汇总」的报警数。
/// </para>
/// <para>
/// <b>接线</b>：本文件位于 <c>Tests/</c> 根下、不属任何工程的默认 glob，故由
/// 四个产品线测试工程以 <c>&lt;Compile Include="..\ContractBaseline.cs" Link="ContractBaseline.cs" /&gt;</c>
/// 显式链接（各工程各得一份同名类型的副本，互不影响），并在各自 <c>GlobalUsings.cs</c> 中
/// <c>global using Mud.Wechat.ContractBaseline;</c>。
/// </para>
/// <para>
/// <b>改这里的正确姿势</b>：先核对官方开放面 / 台账，再改本文件<b>一处</b>；随后跑对应守卫 ——
/// 断言消息会指向本文件，故漂移必红。禁止在守卫里写回字面量。
/// </para>
/// </remarks>
public static class Baseline
{
    /// <summary>企业微信线（`Tests/Mud.Wechat.Work.Tests`）。</summary>
    public static class Work
    {
        /// <summary>
        /// 公共父接口数：`[HttpClientApi]` 且 `IsAbstract = true`（不注册 DI，仅作继承基座）。
        /// </summary>
        /// <remarks>锁定于 `WechatInterfaceNamespaceContractGuards`（N1）。改此值须先核对官方开放面。</remarks>
        public const int AbstractParentInterfaces = 154;

        /// <summary>
        /// 可注入接口数：`[HttpClientApi]` 且非抽象（应用类型子接口 + 独立端点接口）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatInterfaceNamespaceContractGuards`（N1）。
        /// 与 <see cref="AbstractParentInterfaces"/> 合计即契约面接口总数（当前 461）。
        /// </remarks>
        public const int InjectableInterfaces = 307;

        /// <summary>
        /// 已登记 `[WechatCallbackContract]` 的事件键总数（常量声明与特性声明的**并集**）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatCallbackContractGuards`（CB 系列）。
        /// 现行推导：17 + 安全管理 1 + 微信客服 1 + 客户联系/获客族 5 + 90240 的 24 + 应用版本付费订单回调族 6
        /// + 接口调用许可族 4 + 邮箱族 2 + 文档族 5 + 智能表格族 6 + 日程族 5 + 会议族 30 + 家校沟通族 2
        /// + 会话内容存档 3 + 微盘族 9 + 直播族 1 + OA 审批族 1 = 122。
        /// </remarks>
        public const int RegisteredCallbackEventKeys = 122;
    }

    /// <summary>公众号线（`Tests/Mud.Wechat.OfficialAccount.Tests`）。</summary>
    public static class OfficialAccount
    {
        /// <summary>
        /// 主接口（`[HttpClientApi]`）路由数：**同一路由的多方法形态只计 1 条**（如 OCR 的 `*ByUpload` / `*ByUrl` 双形态）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `MpRouteCountGuard`（RC1）。
        /// 现行推导：B2a 前 188 + B2a 小域 15（媒体 2 + 短链 1 + 车牌 OCR 1（双形态各 2 方法）
        /// + 卡券统计 4 + 门店 POI 3 + 语义 1 + 电子发票开具 3）+ B2b 非税 16（JSON 面；
        /// downloadbill 走下载通道）+ B2c 卡券全族 39（53 - B2c 前存量 14）+ 流量主广告分析 1
        /// （publisher_adpos_general / cps_general / settlement 三方法共享 GET /publisher/stat 一路由，按去重口径计 1）= 259。
        /// superresolution 官方已下架（既有守卫裁决）、客服管理员与微信客服绑定 5 条归小程序线
        /// （跨线零交叠 MP-X1），均不计。
        /// </remarks>
        public const int MainInterfaceRoutes = 259;

        /// <summary>
        /// 全量去重路由数（主接口 + 认证/票据基座 + 非 JSON 下载通道）。
        /// </summary>
        /// <remarks>锁定于 `MpRouteCountGuard`（RC2）。现行推导：259 + AbstractsAuth 3 + 下载通道 4（素材 3 + 非税账单 1）= 266。</remarks>
        public const int TotalRoutes = 266;

        /// <summary>
        /// 官方面唯一路由数（索引页表格取值并集口径）。
        /// </summary>
        /// <remarks>锁定于 `MpRouteCountGuard`（RC3）。与 `.docs/公众号服务号/` 台账逐条比对；fresh clone 无 `.docs` 时该断言跳过。</remarks>
        public const int OfficialRoutes = 196;
    }

    /// <summary>小程序线（`Tests/Mud.Wechat.MiniProgram.Tests`）。</summary>
    public static class MiniProgram
    {
        /// <summary>
        /// 端点总数（特性声明的路由 + 手工通道路由）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `MiniProgramContractGuards`（MP-X5）。
        /// 现行推导：392 特性路由 + 6 手工通道（小程序码 3 + 反馈媒体 1 + 对账文件 1 + 客服临时素材 1）= 398；
        /// 按域拆分即 Auth 8 + QrCodeLink 9 + Security 3 + DataAnalysis 11 + SubscribeMessage 4 +
        /// DynamicMessage 3 + Kf 11 + HardwareDevice 9 + Operation 10 + Plugin 2 + Charge 2 +
        /// NearbyPoi 4 + Search 1 + Soter 1 + ServiceMarket 2 + RedPacketCover 1 + Student 1 +
        /// FaceVerify 2 + LaborUse 2 + ExpressBusiness 12 + ExpressDelivery 28 + ExpressIntracity 16 +
        /// ExpressInsuranceFreight 11 + ExpressLocal 15 + ExpressBill 1 + Vip 6 +
        /// TradeGuarantee 2 + TradeComment 9 + TradeComplaint 5 + Live 38 + Vod 32 + XPay 37 +
        /// OrderShipping 12 + Novel 23 + RealName 1 + Game 20 + CloudBase 44 = 398
        /// （Vip 域为 2026-10 小店线设计方案 §4.1 结论 A 划入；2026-10-12 依官方清单核对后：
        /// ExpressDelivery 28 / ExpressIntracity 16 补齐「消息推送 2 + 投诉处理 1」与「同城扣费主体与城市 3」，
        /// 新增 ExpressInsuranceFreight 11（无忧退货）与 ExpressLocal 15（即时配送）两族，
        /// Kf 11 = 原 9 + 客服输入状态 1 + 临时素材二进制通道 1；
        /// 2026-10-13 依官方 transaction-guarantee 分区补齐交易保障域 16 = 基础 2 + 评价管理 9 + 投诉管理 5，
        /// 其中投诉管理 5 条沿用历史 <c>minishop</c> 前缀，由 P3-R1 收窄后按清单在册；
        /// 同日依官方 livebroadcast 分区 38 页取证补齐直播域 38（直播间本体 11 + 小助手与主播副号 8 +
        /// 房间内商品 7 + 商品库管理 7 + 成员角色 3 + 长期订阅群发 2），同批解除 P3-R1 第 2 项「不实现」裁决；
        /// 同日依官方 minidrama 分区 31 页取证补齐短剧媒资域 31（上传 6 + 媒资 4 + 剧目 7 + CDN 3 + 授权 11），
        /// 缺口清单原记「短剧 25」系第三方转述漏项，已按逐页取证更正为 31；
        /// 同日依官方 VirtualPayment 分区 37 页取证补齐虚拟支付域 37（代币 4 + 订单 7 + 道具 5 + 会员订阅 4 +
        /// 资金与提现 10 + 投诉与管控 7），该域另有 28 端点逐请求强制宿主计算的 pay_sig（守卫 XP2 / XP6）；
        /// 同日依官方 order_shipping 分区 10 页 + famousbrand 分区 2 页取证补齐发货信息管理与品牌申请域 12
        /// （发货录入与报备 4 + 订单与管理状态查询 4 + 设置与交易类型变更 2 + 品牌申请 2），
        /// 缺口清单原记「10 条」系漏项，特殊发货报备 <c>opspecialorder</c> 与交易类型变更申请
        /// <c>setwxatradetypecgi</c> 两支为逐页取证补回；
        /// 同日依官方小说 API 索引页（<c>platform-capabilities/industry/novel-api</c>）树下的 23 个端点页取证补齐小说作品管理域 23
        /// （作品本体 6 + 章节管理 8 + 授权管理 3 + 账号授权 3 + 读者运营 3），权限集 169；
        /// 同日依官方 minigame 树 20 页取证补齐小游戏域 20（帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 +
        /// 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5）；
        /// 同日依官方 cloudbase 两棵文档树（<c>miniprogram/dev/server/API/cloudbase/*</c> 与
        /// <c>doc/oplatform/openApi/cloudbase-common/*</c> + <c>cloudbase-batch/*</c>）96 条路由逐页取证补齐
        /// 云开发域 44（环境与账号 8 + 数据库与云存储 16 + 云函数 9 + 其它能力 11；<c>/tcb/*</c> 42 +
        /// <c>/wxa/getopendata</c> + <c>/wxa/getvoipsign</c>，后两支为清单路由错配的纠正）。
        /// 同语料的 <c>/componenttcb/*</c> 面（仅 <c>component_access_token</c>）归开放平台线，
        /// 两线不交叉由守卫 CBZ1 / CBZ2 与开放平台线 CBT 系列双向锁定）。
        /// </remarks>
        public const int Endpoints = 398;
    }

    /// <summary>微信支付 APIv3 线（`Tests/Mud.Wechat.Pay.Tests`）。</summary>
    public static class Pay
    {
        /// <summary>
        /// `[HttpClientApi]` 接口数（八十二注册组中的八十九个声明式接口：除平台收付通组外的八十一组各一 +
        /// 平台收付通组<b>八分身</b>；P1-2 四批（FundApp / PlatsolutionEcommerce / PlatsolutionInsurance /
        /// Webizpay）共十四组各一，逐批清单见 remarks）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatPayDomainContractGuards`（PAY-B 系列）。
        /// 现行域集：Transactions / Refund / Bill / Certificates / ProfitSharing / PayScore /
        /// CombineTransactions / Transfer / NewTaxControlFapiao / MarketingFavor / Apply4SubMerchant /
        /// Apply4Subject / PartnerTransactions / PartnerPayScore / BankPayScore / PartnerTransfer /
        /// BankTransfer / Repayment / MerchantRiskManage / MerchantStore / MerchantAlterApply /
        /// MerchantFund / MerchantSettlement / MerchantService / MerchantManage / MerchantOperationManage /
        /// Ecommerce（按子域拆为 Fund / Bill / Account / ProfitSharing / Refund / MchTransfer / Applyment /
        /// Papay 八个接口，共享 <c>RegistryGroupName = "Ecommerce"</c> ⇒ 一支注册器，守卫 EC-X1；
        /// 这是全仓唯一「一组多接口」的域，勿按「一域一接口」的默认形态拆分）
        /// + P1-3 商家券与营销十域（MarketingBusifavor / MarketingPartnerProductCoupon /
        /// MarketingPartnerDeliveryPlan / MarketingGoodsSubsidyActivity / MarketingPayGiftActivity /
        /// MarketingShoppingReceipt / MarketingPartnerships / IndustryCoupon / MarketingBank /
        /// MarketingBankFavor，各一接口）
        /// + P1-4 品牌七域（BrandApplyment / BrandCardConfig / BrandCardLink / BrandCardMember /
        /// BrandProfitSharing / BrandStore / PasswordExemptContract，各一接口）
        /// + P1-2 平台/服务商四域（AbnormalFundProcessing / BrandRedpacket / TransferToQqWallet /
        /// UserConfirmAuthorization，各一接口）
        /// + P1-2-B 平台服务商解决方案八域（PlatsolutionEcommerceInsuranceContract / Compensate / Deposit /
        /// InsuranceClaim / MchTransfer / RechargeEmployee / Recharge / Settle，各一接口）
        /// + P1-2-C 平台商户保险理赔商家转账三域（PlatsolutionInsuranceReservation /
        /// PlatsolutionInsuranceBatch / PlatsolutionInsuranceReceipt，各一接口）
        /// + P1-2-D 微企付企业支付四域（WebizpayBill / WebizpayEmployee / WebizpayStore / WebizpayUser，各一接口）
        /// + P1-5-A 委托代扣 PAPay 3.0 两域（Papay 直连面 20 端点 / PartnerPapay 服务商面 1 端点，各一接口；
        /// 服务商面官方只印预扣费通知一支，不得按直连面镜像补齐）
        /// + P1-5-C 车主出行两域（Vehicle 10 端点 / TaxiInvoice 9 端点，各一接口；出租车发票族的
        /// 「上传发票文件」是 multipart 手工通道 ⇒ 不入本数，其成员随 <c>TaxiInvoice</c> 组一并注册）
        /// + P1-5-D 教育与代发薪三域（EducationPapay 8 端点 / EducationSchoolPay 8 端点 / PayrollCard 6 端点，各一接口）
        /// + P1-5-E 行业方案剩余十域（BusinessCircle 4 / Catering 1 / FundsToOversea 6 / GoldPlan 5 / Multiuse 4 /
        /// PayInstallment 1 / RateActivity 3 / Recharge 2 / SmartGuide 4 / TransactionBlock 4，各一接口）
        /// + P1-5-G 取证勘误补回两域（Capital 6 / Lovefeast 1，各一接口；二者原列 P1-5 判非契约清单，
        /// 按「现行文档中心树内检索」重检后证得在册 ⇒ 转真缺口，<b>不得</b>退回零声明名册）
        /// + P1-6 旧版会员卡与会员有礼活动两域（MarketingMemberCardOpen 25 端点 / MarketingMemberCardActivity 4 端点，各一接口；
        /// 两套并存契约：品牌会员卡 <c>/v3/brand/partner/card-member/*</c> 与本域是<b>并存非替代</b>关系，
        /// 四支 204 无应答包体 ⇒ 裸 Task，守卫 MCO-B1~B6 / MCA-B1~B5）
        /// 。<b>不计入本数</b>：<c>MerchantMedia</c>、<c>MarketingMedia</c> 两个成员只有 multipart 手工通道、
        /// 无 <c>[HttpClientApi]</c> 接口（守卫 MC-B1 / EC-X2 口径），故 <c>PayModule</c> 成员数 =
        /// 本数 - 8（收付通八合一）+ 3（收付通成员 + 两个手工通道成员）。
        /// </remarks>
        public const int HttpApiInterfaces = 91;

        /// <summary>
        /// 端点总数（各域端点之和；账单 / 发票的**文件下载通道**与媒体上传等**手工通道**不计入生成式接口）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatPayDomainContractGuards`。现行推导：原 54 + 直连下单三族（Native / APP / H5）= 57
        /// + P0-1 特约商户进件 6（申请单族 3 + 结算账户族 3，守卫 A4S-B1）= 63
        /// + P0-2 主体进件 5（提交 / 查询 / 两个撤销变体 / 商户授权状态，守卫 A4SJ-B1）= 68
        /// + P0-3 服务商基础支付与分期 9（下单五族 + 双路查单 + 关单 + 分期曝光，守卫 PT-B1；
        /// SKIT 清单 12 行中 codepay / reverse 属 APIv2、transactions-exposure 页面已下架）= 77
        /// + P0-4 服务商支付分 23（授权 7 + 服务订单 8 + 服务额度 1 + 支付分计划 3 + 计划型服务订单 1 +
        /// 用户签约计划 3，守卫 PS-B1；SKIT 清单 28 行中「商户收款」现行目录无该端点、
        /// 三支端点在授权记录 / 计划应答多页重复声明已归并）= 100
        /// + P0-5 银行/从业机构支付分 11（授权 3 + 服务订单 6 + 扣款信息 2，守卫 BS-B1；
        /// SKIT 清单 15 行中「收单曝光」出处页已下架（现跳 50x）⇒ 零端点、两支 GET 与查询订单
        /// 在多行重复声明已归并）= 111
        /// + P0-6 服务商批量转账 5（发起 1 + 批次查 2 + 明细查 2，守卫 PTF-B1）= 116
        /// + P0-6 从业机构（银行）批量转账与电子回单 9（转账 5 + 回单 4；回单的汇总 / 明细两条路径
        /// 各有 POST 受理与 GET 查询 ⇒ 按 (方法, 路由) 计数，守卫 BT-B1）= 125
        /// + P0-6 平台商户还款 3（预下单 + 查单 + 关单，关单为 204，守卫 RP-B1；
        /// 端上调起四支与支付成功回调不属本 HTTP 面 ⇒ 零声明，守卫 RP-B2）= 128
        /// + P0-7 商户风险管理 11（风险订单同步 4：query-risk-trades 与 recv-dispose-result 各有
        /// 「服务商面 / 普通商户面」两支同方法同路由、字段表不同 ⇒ 端点按方法计而 (方法, 路由) 去重后为 9，
        /// 守卫 MRB-B1 两个数都锁 + 风险共建 3 + 违规通知回调地址四件套 4，守卫 MRB-B1）= 139
        /// + P0-7 商户门店管理 5（创建 + 更新 + 查询 + 绑定收款号 + 解绑收款号；官方十页因
        /// 「商户面与服务商面字段表只差一个 sub_mchid、应答表零差异」而归并为五端点，守卫 MST-B6；
        /// 分页查门店与删除门店官方无 HTTP 面 ⇒ 零声明，守卫 MST-B2）= 144
        /// + P0-7 商户资料变更 12（主体族 4 + 超级管理员族 4 + 商家简称族 4；官方 4013 系转述页与现行
        /// 页路由不同形，八行核得两族各独立页面 ⇒ 三族各四支，守卫 MAA-B1/B2；
        /// 撤销是 POST 动词段而非 DELETE、本域零 204）= 156
        /// + P0-7 商户账户余额 2（实时余额 + 日终余额；同前缀 /v3/merchant/fund/ 下的提现族四支
        /// 属【平台商户】面 ⇒ 零端点，守卫 MF-B2 与裁决守卫 C1 互为正对照）= 158
        /// + P0-7 商户结算规则申请 2（创建 + 按 application_id 查；从业机构面，官方无「按商户号查最新
        /// 申请单」与规则列表/删除面 ⇒ 零声明，守卫 MSET-B2）= 160
        /// + P0-7 消费者投诉 11（通知地址 CRUD 4 + 投诉列表 / 详情 / 协商历史 3 + 回复 / 完成 / 退款审批 / 即时服务 4；
        /// 图片上传 / 拉取两支走手工通道，不计入生成式接口，守卫 MC-B1）= 171
        /// + P0-7 商户管理记录 3（查询单条管理记录 + 提交资料（204）+ 按 sub_mchid 查询列表；
        /// 上传管理记录文件走手工通道，不计入生成式接口）= 174
        /// + P0-7 商户运营管控情况查询 1（查询子商户管控情况；同前缀尽调族三支属【平台商户】面 ⇒ 零端点，守卫 MOM-B2）= 175
        /// + P1-1 平台收付通 55（账户 10 + 进件 7 + 分账与补差 11 + 退款 9 + 商家转账 6 + 代扣合单 6 + 资金 5 + 账单 1，
        /// 八个声明式接口共用注册组 Ecommerce；退款族三支「同方法同路由两契约」⇒ 55 方法归并 52 条 (方法, 路由)，
        /// 端点计数按 HTTP 方法特性实例计，守卫 EC-X1；注销资料图片上传走 multipart 手工通道，不计入生成式接口，
        /// 守卫 EC-X2；after-sales-orders 两支官方现行目录无契约页 ⇒ 零声明，守卫 EC-R7）= 230
        /// + P1-3 商家券与营销 89（MarketingFavor 增量 8：回调地址两支 + 批次列表 + 可用单品 / 可用商户 /
        /// 退款明细 / 核销明细 + 条件查询用户券，守卫 MF5 / MF6；MarketingBusifavor 15、
        /// MarketingPartnerProductCoupon 33（同路由两契约 ⇒ 33 方法 / 32 条 (方法, 路由)，守卫 MPPC-B1）、
        /// MarketingPartnerDeliveryPlan 5、MarketingGoodsSubsidyActivity 8、MarketingPayGiftActivity 8、
        /// MarketingShoppingReceipt 5、MarketingPartnerships 2、IndustryCoupon 1、MarketingBank 2、
        /// MarketingBankFavor 2；营销图片上传两支为 multipart 手工通道 ⇒ 不计入，守卫 MMK-B1）= 319
        /// + P1-4 品牌七域 53（品牌入驻 4：受理两支共用 2 行应答、查询两支共用 7 行应答；名片配置 5；交易连接名片 5；
        /// 会员卡与会员 15（模板四支共用 49 行应答、用户卡五支共用 28 行应答，两族列表支各独立形态；请求侧因必填口径不同一律分建）；
        /// 品牌分账 9（回退两支共用、接收方增删共用，发起 15 行与查询 16 行分建）；品牌门店 9（创建 / 查询 / 更新共用 21 行，
        /// close / resume 共用 3 行，<b>删除支官方未印发应表 ⇒ 裸 Task</b>，绑定 5 行与解绑 1 行分建）；
        /// 免密支付契约 6（qs 面与 sp 面、小程序场景与支付后场景<b>四支各建一型</b>：字段名 appid/openid(128) 与
        /// sp_appid/sp_openid(64) 不可互换）——<b>共用只发生在应答侧</b>，逐表签名（含 req 标记）比对见各域守卫）= 372
        /// + P1-2-A Transfer 增量 4（elecsign 申请两支 POST + pre-transfer-with-authorization + transfer）= 376
        /// + P1-2-A 新域 14（AbnormalFundProcessing 3 + BrandRedpacket 5 + TransferToQqWallet 3 + UserConfirmAuthorization 3）= 390
        /// + P1-2-B 平台服务商解决方案 31（InsuranceContract 2 + Compensate 5 + Deposit 7 + InsuranceClaim 5 + MchTransfer 3 + RechargeEmployee 3 + Recharge 3 + Settle 3）= 421
        /// + P1-2-C 平台商户保险理赔商家转账 13（Reservation 4 + Batch 5 + Receipt 4）= 434
        /// + P1-2-D 微企付企业支付 12（Bill 2 + Employee 6 + Store 2 + User 2；官方目录树内另有 5 支「通知」页
        /// 与 1 支小程序 JSAPI <c>wx.openBusinessView</c>，均非商户侧 HTTP 契约面 ⇒ 零端点）= 446
        /// + P1-5-A 委托代扣 PAPay 3.0 21（直连面 20：签约四端预签约各建一型 + 通用/保险两条协议线 + 预约扣费 + 扣费周期；
        /// 服务商面 1（官方只印预扣费通知一支）；两支 notify 与续期支官方「无应答包体」⇒ 裸 Task 但仍按方法计端点）= 467
        /// + P1-5-B 电子发票服务商 / 数电面 13（<b>追加</b>进既存 IWechatPayFapiaoService，无新接口无新注册组：
        /// 卡券模板 1 + 两支开具（issue-general / real-estate-leasing，各建一方法，官方 202「受理」语义）+
        /// 能力邀请 2 + 子商户功能状态 2 + 商户开票信息与开发选项 4（GET/PATCH development-config <b>同路由两动词各计一支</b>）+
        /// 税收分类编码 1 + 用户抬头 2；三支「无应答包体」为裸 Task 仍按方法计 ⇒ 守卫 FPX-B1~B6）= 480
        /// + P1-5-C 车主出行 + 出租车电子发票 19（Vehicle 10：ETC 六支（官方只印服务商页）+ 停车四支（商户页 + 服务商页并集）；
        /// TaxiInvoice 9：签到签退与插卡两支官方「无应答包体」⇒ 裸 Task 仍按方法计，而第十支「上传出租车电子发票文件」是
        /// <c>multipart/form-data</c> + 国密 SM3 ⇒ 不入声明式基线，由 <c>WechatPayTaxiInvoiceFileService</c> 手工通道承载
        /// （守卫 VEH-B1~B6 / TXI-B1~B6，TXI-B1 同时锁定该路由在声明名册中零出现）= 499
        /// + P1-5-D 教育与代发薪三域 22（EducationPapay 8：七支印商户页 + 服务商页双页、解除协议支官方<b>只印商户页</b>，
        /// 三支「无应答包体」⇒ 裸 Task 仍按方法计，两支查单应答同形共用一型；EducationSchoolPay 8：八支全部只印服务商页，
        /// 解除协议支 204 ⇒ 裸 Task，下单与两支查单<b>写读共用同一应答型</b>；PayrollCard 6：六支全部只印服务商页，
        /// 三支官方请求头表印 <c>Wechatpay-Serial</c> 必填 ⇒ 涉密是逐页结论，令牌申领与预下单应答不同形各建一型；
        /// 守卫 EDP-B1~B6 / ESP-B1~B6 / PRC-B1~B6，清单行 85 与 103~104 共三支判非契约 ⇒ 零声明见非契约路由守卫第九条）= 521
        /// + P1-5-E 行业方案剩余十域 34（BusinessCircle 4：四支双页并集、两支 204，正文有加密提示但请求头表未列 ⇒ <b>判无涉密必填头</b>；
        /// Catering 1：只印服务商页、状态变化须上传全量明细、204；FundsToOversea 6：六支只印服务商页且身份【平台商户】、
        /// 下单与按 out_order_id 查单<b>写读共用同一应答型</b>；GoldPlan 5：只印服务商页且双身份【普通服务商】+【渠道商】，
        /// close 是 POST、open 是 <b>PATCH</b>、set-filter 是 POST，三支 204、两支状态切换共用一型；Multiuse 4：只印商户页，
        /// 三支流水应答同形共用一型，<b>发券一支</b>官方请求头表印 <c>Wechatpay-Serial</c> 必填；PayInstallment 1：只印商户页且
        /// <b>查询走 POST</b>、路由段是 <c>instal</c> 不是 <c>installment</c>；RateActivity 3：只印服务商页、双从业机构身份，
        /// POST 受理与 PATCH 修改应答同形共用；Recharge 2：只印商户页，申请支 6 键与查询支 17 键各建一型；
        /// SmartGuide 4：四支双页并集（服务商页多选填 <c>sub_mchid</c>），查询与注册<b>同一 URL</b> 仅方法不同，
        /// 两支 204，查询/注册/更新<b>三支</b>印 <c>Wechatpay-Serial</c> 必填而分配支未列 ⇒ 涉密逐页结论；
        /// TransactionBlock 4：四支只印服务商页且仅白名单开放，申诉两支五行同形共用、记录两支各建）= 555
        /// + P1-5-G 取证勘误补回 7（Capital 银行组件 6：六支全 GET、只印服务商页、逐支印「单商户单接口 300TPS」，
        /// 省份与城市两支官方<b>零请求参数</b>、银行/支行三支 <c>limit</c> 必填、卡号开户行支是<b>唯一</b>印
        /// <c>Wechatpay-Serial</c> 必填的涉密支；对公/对私应答同形共用一型，支行支与卡号支各建一型；
        /// Lovefeast 1：只印商户页、官方爱心餐章节 API 列表恰此一支、<b>频率官方未印</b>，「爱心餐查券」官方指向既存
        /// <c>MarketingFavor</c> 域 ⇒ 不重复建；守卫 CAP-B1~B6 / LVF-B1~B6）= 562
        /// + P1-6 旧版会员卡与会员有礼活动两域 29（MarketingMemberCardOpen 25：创建模板 / 查询列表 / 查询详情 / 修改模板 /
        /// 删除模板（204）/ 升级模板（204）/ code预存 / 查询用户卡 / 修改用户卡 / 作废code（204）/ 确认入会 / 导入用户（OpenID）/
        /// 导入用户（手机号）/ 创建二维码 / 解密code / 查询激活信息 / 激活（204）/ 授权令牌 / 设置权益（用户级 PATCH）/
        /// 设置权益（模板级 PATCH）/ 提交积分（204）/ 预下单积分（204）/ 设置回调（PATCH）/ 查询回调 / 查询用户所有卡；
        /// MarketingMemberCardActivity 4：创建活动 / 查询列表 / 查询详情 / 终止活动；四支 204 无应答包体 ⇒ 裸 Task，
        /// 两套并存契约：品牌会员卡与本域是<b>并存非替代</b>关系，守卫 MCO-B1~B6 / MCA-B1~B5）
        /// + P1-6 既有域增量 2（MarketingPartnerships 终止合作关系 / MarketingBusifavor 发送核销券，各一方法）= 593
        /// + P0 基础支付主动作 3（付款码支付 / 订单曝光 / 撤销订单，守卫 TX-B1）= 596。
        /// </remarks>
        public const int Endpoints = 596;
    }

    /// <summary>开放平台线（`Tests/Mud.Wechat.OpenPlatform.Tests`，B1 起）。</summary>
    public static class OpenPlatform
    {
        /// <summary>
        /// `[HttpClientApi]` 接口数。
        /// </summary>
        /// <remarks>
        /// 锁定于 `OpenPlatformContractGuards`（OP-B 系列）。
        /// 现行接口集：Component（管理面）/ ComponentTicketFree（推票引导，免令牌）/
        /// OpenAccount / Account / Sns / Category（类目管理）—— Component 域双接口形态（对齐公众号 OpenApi 域 I4 裁决）；
        /// 另有 ComponentTcb（批量代云开发）<b>六支</b>接口共用注册组 <c>ComponentTcb</c>
        /// （Env 7 + Database 9 + File 4 + Scf 7 + CloudRun 19 + StaticStore 4），逐接口表由 CBT 系列守卫持有。
        /// Embedded（半屏小程序）单接口 6 端点为 2026-10-17 依官方 embedded-management 6 页取证落地，
        /// 令牌面 <c>authorizer_access_token</c>，注册组 <c>Embedded</c>，逐接口表由 EMB 系列守卫持有
        /// （该域按留档 MP-D-BUCKET-RULING-01 归开放平台线，小程序线由守卫 NC2 锁定零声明）。
        /// Auth（微信认证）单接口 5 端点为 2026-10-17 依官方 weapp-wxverify 5 页取证落地，
        /// 令牌面 <c>authorizer_access_token</c>，注册组 <c>Auth</c>，逐接口表由 AUTH 系列守卫持有
        /// （同按 MP-D-BUCKET-RULING-01 归开放平台线，小程序线 NC2 锁定零声明）。
        /// Ams（流量主代运营管理面）单接口 19 端点 + AmsShare（分账面）单接口 3 端点为 2026-10-17 依官方 ams/* 22 页取证落地：
        /// Ams 恒 POST <c>/wxa/operationams</c>、以 <c>action</c> 分派、令牌面 <c>authorizer_access_token</c>、注册组 <c>Ams</c>；
        /// AmsShare 恒 POST <c>/wxa/getdefaultamsinfo</c>·<c>/wxa/setdefaultamsinfo</c>、令牌面 <c>component_access_token</c>、注册组 <c>AmsShare</c>
        /// （两面令牌归属不同故拆两支接口/注册组）。逐接口表由 AM 系列守卫持有。
        /// MiniProgramManage（代小程序管理 · 授权方令牌面）为 2026-10-17 依官方 miniprogram-management/* 逐页取证分批落地（30-1 起）：
        /// 30-1 落「代码管理 15 + 名称与搜索 5 + 类目 1」三支、30-2 落「域名与 DNS 9 + 体验成员 3 + 违规申诉 2 + 订阅组件 3」四支、
        /// 30-3 落「ICP 备案 JSON 面 12 + 隐私接口 3 + 订单路径查询 1」三支、30-4 落「试用小程序核验 2」（转正 / 改名，逐页取证为 <c>authorizer_access_token</c>、权限集 81）一支，
        /// 十一支子接口共用注册组 <c>MiniProgramManage</c>、令牌面 <c>authorizer_access_token</c>，逐接口表由 MPG 系列守卫持有。<c>applysetorderpathinfo</c>（订单路径申请）经逐页复核为 <b>component_access_token</b> 面，
        /// 与授权方 <c>getorderpathinfo</c>（authorizer）令牌归属相反，故不入本授权方组。
        /// MiniProgramTemplate（代码模板库 / 下单路径申请 · <c>component_access_token</c> 面）为 30-4 新注册组：模板库管理 4（草稿列表 / 模板列表 / 添加模板 / 删除模板）+ 下单页 path 批量申请 1（<c>applysetorderpathinfo</c>），
        /// 两支子接口共用注册组 <c>MiniProgramTemplate</c>（对齐 Ams/AmsShare 按令牌拆组先例），逐接口表由 TPL 系列守卫持有。
        /// 30-5 落三支特殊形态：<c>uploadmedia</c>（代码管理接口内，表单仅 <c>media</c>）与 <c>upload_icp_media</c>（备案接口内，表单四支字段）为 multipart、零 JSON 请求 DTO，并入授权方 <c>MiniProgramManage</c> 组；
        /// <c>get_qrcode</c> 成功应答为 image/jpeg 二进制流 ⇒ 不走声明式管线，由手写通道 <c>IOpenPlatformMpTrialQrCodeService</c>（非 <c>[HttpClientApi]</c>，随 MiniProgramManage 模块装配）承载，守卫 QRC 系列锁定。
        /// 30-6 判非契约：试用小程序转正族的 2.0 旧版 <c>/wxa/getmpadminauth</c> 与 <c>/wxa/mpverifybetaweapp</c> 不建接口、不声明路由，由守卫 OP-NC 系列（<c>OpenPlatformNonContractRouteZeroDeclarationContractGuards</c>）锁定零声明。
        /// </remarks>
        public const int HttpApiInterfaces = 29;

        /// <summary>
        /// 端点总数（各接口方法之和）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `OpenPlatformContractGuards`。现行推导：Component 16（clear_quota/v2 +
        /// fastregisterweapp 双方法 + 管理员换绑 account/componentrebindadmin 1 + 域名 2 + 确认文件 1 +
        /// 隐私 3 + 授权方信息/列表/选项/设选项 4 + 快速注册测试版小程序 1 + 快速注册个人小程序 1）
        /// + ComponentTicketFree 1（start_push_ticket）+ OpenAccount 6 + Account 4 + Sns 3 + Category 12 = 42
        /// + ComponentTcb 50（/componenttcb/* 全域，恒 POST，2026-10-15 逐页核验零路由错配）
        /// + Embedded 6（/wxaapi/wxaembedded/* 半屏小程序，4 POST + 2 GET，2026-10-17 逐页取证）
        /// + Auth 5（/wxa/sec/* 微信认证：身份树 1 GET + 提审/重审/进度查询 3 POST + 补充材料 multipart 1 POST，2026-10-17 逐页取证）
        /// + Ams 19（/wxa/operationams 单路由 action 分派，恒 POST，2026-10-17 逐页取证）
        /// + AmsShare 3（/wxa/getdefaultamsinfo ×2 + /wxa/setdefaultamsinfo ×1，恒 POST，2026-10-17 逐页取证）= 125
        /// + MiniProgramManage 22（30-1：代码管理 15 + 名称与搜索 6 + 类目 1，2026-10-17 逐页取证）
        /// + MiniProgramManage 17（30-2：域名与 DNS 9 + 体验成员 3 + 违规申诉 2 + 订阅组件 3，2026-10-17 逐页取证）
        /// + MiniProgramManage 16（30-3：ICP 备案 JSON 面 12 + 隐私接口 3 + 订单路径查询 1，2026-10-17 逐页取证）
        /// + MiniProgramManage 2（30-4：试用小程序核验 2，转正 / 改名，authorizer_access_token，2026-10-17 逐页取证）
        /// + MiniProgramTemplate 5（30-4 新注册组：模板库 4 + 下单路径申请 1，component_access_token，2026-10-17 逐页取证）
        /// + MiniProgramManage 2（30-5：multipart 上传 2，代码管理 uploadmedia 1 + 备案 upload_icp_media 1，authorizer_access_token，2026-10-17 逐页取证）= 189。
        /// 令牌链 4 端点（api_component_token 等）由 Provider raw HTTP 承载，**不计**（T2 裁定）；
        /// 体验版二维码 /wxa/get_qrcode 由**非声明式图片流通道** IOpenPlatformMpTrialQrCodeService 承载（成功应答是 image/jpeg 二进制），
        /// 同口径**不计**入声明式端点总数，由 QRC 系列守卫单独锁定；
        /// 试用小程序转正族 2.0 旧版两支 /wxa/getmpadminauth + /wxa/mpverifybetaweapp 经逐页核验（旧页正文仅剩「优化后重新上线」、无 301、不在现行 openApi/OpenApiDoc 目录树内）判为**非契约**，
        /// 由 OP-NC 系列守卫锁定 Src/ 零声明，**恒不计**入端点总数（追踪号 P1-OP-LEGACY20-BETAVERIFY-01）。
        /// </remarks>
        public const int Endpoints = 189;
    }
}
