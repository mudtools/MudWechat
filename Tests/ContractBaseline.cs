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
        /// downloadbill 走下载通道）+ B2c 卡券全族 39（53 - B2c 前存量 14）= 258。
        /// superresolution 官方已下架（既有守卫裁决）、客服管理员与微信客服绑定 5 条归小程序线
        /// （跨线零交叠 MP-X1），均不计。
        /// </remarks>
        public const int MainInterfaceRoutes = 258;

        /// <summary>
        /// 全量去重路由数（主接口 + 认证/票据基座 + 非 JSON 下载通道）。
        /// </summary>
        /// <remarks>锁定于 `MpRouteCountGuard`（RC2）。现行推导：258 + AbstractsAuth 3 + 下载通道 4（素材 3 + 非税账单 1）= 265。</remarks>
        public const int TotalRoutes = 265;

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
        /// 现行推导：303 特性路由 + 6 手工通道（小程序码 3 + 反馈媒体 1 + 对账文件 1 + 客服临时素材 1）= 309；
        /// 按域拆分即 Auth 8 + QrCodeLink 9 + Security 3 + DataAnalysis 11 + SubscribeMessage 4 +
        /// DynamicMessage 3 + Kf 11 + HardwareDevice 9 + Operation 10 + Plugin 2 + Charge 2 +
        /// NearbyPoi 4 + Search 1 + Soter 1 + ServiceMarket 2 + RedPacketCover 1 + Student 1 +
        /// FaceVerify 2 + LaborUse 2 + ExpressBusiness 12 + ExpressDelivery 28 + ExpressIntracity 16 +
        /// ExpressInsuranceFreight 11 + ExpressLocal 15 + ExpressBill 1 + Vip 6 +
        /// TradeGuarantee 2 + TradeComment 9 + TradeComplaint 5 + Live 38 + Vod 32 + XPay 37 +
        /// OrderShipping 12 + Novel 23 + RealName 1 + Game 20 = 354
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
        /// 同日依官方 novelreadersetrecmdnovel 分区 23 页取证补齐小说作品管理域 23
        /// （作品本体 6 + 章节管理 8 + 授权管理 3 + 账号授权 3 + 读者运营 3），权限集 169；
        /// 同日依官方 minigame 树 20 页取证补齐小游戏域 20（帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 +
        /// 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5））。
        /// </remarks>
        public const int Endpoints = 354;
    }

    /// <summary>微信支付 APIv3 线（`Tests/Mud.Wechat.Pay.Tests`）。</summary>
    public static class Pay
    {
        /// <summary>
        /// `[HttpClientApi]` 接口数（二十六个业务域各一）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatPayDomainContractGuards`（PAY-B 系列）。
        /// 现行域集：Transactions / Refund / Bill / Certificates / ProfitSharing / PayScore /
        /// CombineTransactions / Transfer / NewTaxControlFapiao / MarketingFavor / Apply4SubMerchant /
        /// Apply4Subject / PartnerTransactions / PartnerPayScore / BankPayScore / PartnerTransfer /
        /// BankTransfer / Repayment / MerchantRiskManage / MerchantStore / MerchantAlterApply /
        /// MerchantFund / MerchantSettlement / MerchantService / MerchantManage / MerchantOperationManage。
        /// </remarks>
        public const int HttpApiInterfaces = 26;

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
        /// + P0-7 商户运营管控情况查询 1（查询子商户管控情况；同前缀尽调族三支属【平台商户】面 ⇒ 零端点，守卫 MOM-B2）= 175。
        /// </remarks>
        public const int Endpoints = 175;
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
        /// OpenAccount / Account / Sns / Category（类目管理）—— Component 域双接口形态（对齐公众号 OpenApi 域 I4 裁决）。
        /// </remarks>
        public const int HttpApiInterfaces = 6;

        /// <summary>
        /// 端点总数（各接口方法之和）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `OpenPlatformContractGuards`。现行推导：Component 14（clear_quota/v2 +
        /// fastregisterweapp 双方法 + 管理员换绑 account/componentrebindadmin 1 + 域名 2 + 确认文件 1 +
        /// 隐私 3 + 授权方信息/列表/选项/设选项 4）
        /// + ComponentTicketFree 1（start_push_ticket）+ OpenAccount 6 + Account 4 + Sns 2 + Category 12 = 39。
        /// 令牌链 4 端点（api_component_token 等）由 Provider raw HTTP 承载，**不计**（T2 裁定）。
        /// </remarks>
        public const int Endpoints = 39;
    }
}
