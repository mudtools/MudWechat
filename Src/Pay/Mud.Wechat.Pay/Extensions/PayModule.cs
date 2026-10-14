// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Extensions;

/// <summary>
/// 微信支付 API 模块枚举（对齐 <c>WechatModule</c>，三段式注册的第一段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>命名红线</b>：本枚举的成员即源生成器的 <c>RegistryGroupName</c>，因此<b>绝不能叫 <c>Pay</c></b> ——
/// 本仓已有「企业支付」产品线占用了 <c>WechatModule.Pay</c> 与生成的 <c>AddPayWebApiHttpClient()</c>。
/// 若两条线用同一个组名，两个程序集会各产出一个同名扩展方法，跨程序集导入即歧义。
/// 实测全仓既有 59 个组名，<c>Transactions</c> / <c>Refund</c> / <c>Bill</c> / <c>Certificates</c> 均未被占用。
/// </para>
/// <para>
/// <b>成员只列已落地域</b>：没有对应 <c>Add{X}WebApiHttpClient()</c> 的成员会落进
/// <see cref="PayServiceBuilder.AddModules"/> 的「查不到注册器 ⇒ 静默不注册」分支，
/// 是一个无声陷阱。故随域增量加入，不预先铺空值。
/// </para>
/// <para>模块注册是三段式：本枚举值 + <c>Add{域}Api()</c>（见 <see cref="PayServiceBuilder"/>）+ 源生成器产出的 <c>Add{域}WebApiHttpClient()</c>。</para>
/// </remarks>
public enum PayModule
{
    /// <summary>
    /// 基础交易（下单 / 查单 / 关单，4 端点）：JSAPI·小程序下单 + 微信支付订单号查单 + 商户订单号查单 + 关闭订单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>。
    /// </summary>
    Transactions,

    /// <summary>
    /// 退款（3 端点）：申请退款 + 查询单笔退款 + 发起异常退款。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791903"/>。
    /// </summary>
    Refund,

    /// <summary>
    /// 账单（2 端点）：申请交易账单 + 申请资金账单（账单文件本身经 <c>IWechatPayBillDownloadService</c> 下载）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791907"/>。
    /// </summary>
    Bill,

    /// <summary>
    /// 平台证书（1 端点）：获取平台证书列表（用于应答 / 回调验签与 <c>Wechatpay-Serial</c> 轮换）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551764"/>。
    /// </summary>
    Certificates,

    /// <summary>
    /// 分账（<b>P2 首批 3 端点</b>）：添加分账接收方 + 请求分账 + 查询分账结果。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524936"/>。
    /// </summary>
    /// <remarks>其余分账端点（回退 / 解冻 / 待分金额 / 删除接收方 / 分账账单）待逐页核验后增量。</remarks>
    ProfitSharing,

    /// <summary>
    /// 支付分（<b>P2 首批 3 端点</b>）：创建 / 查询 / 取消服务订单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587900"/>。
    /// </summary>
    /// <remarks>完结 / 收款 / 同步 / 授权与回调等待逐页核验后增量。</remarks>
    PayScore,

    /// <summary>
    /// 合单支付（<b>P2 首批 2 端点</b>，<b>仅服务商</b>）：JSAPI/小程序合单下单 + 合单关闭订单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>。
    /// </summary>
    /// <remarks>合单查询 / Native 下单 / 合单退款等待逐页核验后增量。</remarks>
    CombineTransactions,

    /// <summary>
    /// 商家转账（<b>P2 首批 2 端点</b>）：发起转账 + 商户单号查询转账单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>。
    /// </summary>
    /// <remarks>
    /// <b>「不换单重试」红线的最低闭环已具备</b>：发起转账遇错后可按商户单号查单，再依 <c>state</c>
    /// （<c>ACCEPTED</c>/<c>PROCESSING</c> ⇒ <b>原单重试</b>；<c>FAIL</c> ⇒ 才可重新生成单据）处置。
    /// <b>仍缺</b>：微信单号查询转账单 · 撤销转账 · 获取电子回单（商户单号/微信单号）。
    /// </remarks>
    Transfer,

    /// <summary>
    /// 电子发票（<b>P2 首批 2 端点</b>）：开具 + 查询电子发票。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>。
    /// </summary>
    /// <remarks>
    /// 已核并更正设计方案 §2.5：<b>普通商户侧有</b>完整电子发票文档（非「无独立入口」）；
    /// 服务商侧的「开具通用行业电子发票」是另一路由（<c>issue-general</c>），本域不覆盖。
    /// </remarks>
    NewTaxControlFapiao,

    /// <summary>
    /// 代金券（<b>P2 表内最后一项，首批 2 端点</b>）：创建代金券批次 + 查询代金券详情。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534633"/>。
    /// </summary>
    /// <remarks>
    /// 官方流程：创建 → <b>激活</b>（未实现）→ 发放；<b>创建成功 ≠ 可发放</b>。
    /// </remarks>
    MarketingFavor,

    /// <summary>
    /// 特约商户进件（<b>P0-1，6 端点</b>）：提交进件申请单 + 按申请单号 / 业务编号查状态 +
    /// 修改结算账户 + 查询结算账户 + 查询结算账户修改申请状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>。
    /// </summary>
    /// <remarks>
    /// <b>服务商形态</b>：签名主体是服务商商户号；<c>sub_mchid</c> 是<b>调用期参数</b>（path）而非商户作用域，
    /// 且进件阶段子商户号尚不存在 ⇒ 宿主应以<b>服务商自有商户号</b>的商户配置进入调用作用域。
    /// 提交申请单与修改结算账户<b>均为异步审核</b>，受理成功不等于入驻 / 改卡生效。
    /// </remarks>
    Apply4SubMerchant,

    /// <summary>
    /// 主体进件 / 商户开户意愿确认（<b>P0-2，5 端点</b>）：提交申请单 + 查询申请单审核结果 +
    /// 按业务编号 / 申请单号撤销（两个变体）+ 获取商户开户意愿确认状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>。
    /// </summary>
    /// <remarks>
    /// <b>支持商户类型为渠道商 / 从业机构（银行、支付机构）</b>，不含普通服务商 ⇒
    /// 与 <see cref="PayModule.Apply4SubMerchant"/> 是两套契约（前缀、查询定位方式、状态取值、
    /// 主体类型枚举全部不同，<b>无状态映射表可写</b>）。签名主体是服务商 / 从业机构自有商户号，
    /// <c>sub_mchid</c> 与 <c>channel_id</c> 均为<b>调用期参数 / 请求体字段</b>而非商户作用域。
    /// </remarks>
    Apply4Subject,

    /// <summary>
    /// 服务商基础支付 / 分期（<b>P0-3，9 端点</b>）：下单五族（JSAPI/小程序、Native、APP、H5、扫码支付）+
    /// 双路查单 + 关单 + 分期曝光咨询。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>。
    /// </summary>
    /// <remarks>
    /// <b>服务商形态</b>：签名主体是<b>服务商商户号</b>（<c>sp_mchid</c>），<c>sub_mchid</c> 是
    /// <b>报文 / Query 参数</b>而非商户作用域 ⇒ 宿主应以服务商自有商户配置进入调用作用域，
    /// <b>不得</b>用子商户证书调本域。与 <see cref="PayModule.Transactions"/>（直连）是两套契约：
    /// 商户标识四分、<c>payer</c> 双 openid <b>二选一</b>、<c>support_fapiao</c> 位置按族分列。
    /// 官方 SKIT 清单里的 <c>codepay</c> / <c>reverse</c> 是 <b>APIv2</b> 面（现行正文无 V3 字段表）、
    /// <c>transactions/exposure</c> 页面<b>已下架</b> ⇒ 本域零端点，前两者归 APIv2 批次。
    /// </remarks>
    PartnerTransactions,

    /// <summary>
    /// 服务商支付分（<b>P0-4，23 端点</b>）：用户授权 7（预授权 ×2 + 风险评估 + 授权记录查 ×2 + 解除授权 ×2）+
    /// 服务订单 8（创建 / 查询 / 取消 / 完结 / 修改 / 未完结提醒 / 确认付款提醒 / 不同步支付结果）+
    /// 服务额度 1 + 支付分计划 3 + 计划型服务订单 1 + 用户签约计划 3。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>。
    /// </summary>
    /// <remarks>
    /// <b>本域商户标识形态与 <see cref="PayModule.PartnerTransactions"/> 相反</b>：服务商支付分全部报文用
    /// <c>appid</c> / <c>mchid</c>（服务商侧）+ <c>sub_mchid</c> / <c>sub_appid</c>（子商户侧），
    /// <b>没有</b> <c>sp_</c> 前缀字段 ⇒ 照抄基础支付字段名会静默丢字段。
    /// 签名主体是<b>服务商商户号</b>，<c>sub_mchid</c> 按端点分列 query / body 两处。
    /// 与 <see cref="PayModule.PayScore"/>（直连）是两套契约：三套路由前缀
    /// （<c>/v3/payscore/partner/</c>、<c>/v3/payscore/plan/partner/</c>、<c>/v3/payscore/sign-plan/partner/</c>）、
    /// 两套时间格式并存、状态取值集合并非直连超集。
    /// 官方 SKIT 清单里的「商户收款」<c>POST /v3/payscore/serviceorder/{out_order_no}/pay</c>
    /// 现行目录<b>无该端点</b> ⇒ 本域零端点（守卫 PS-B2）。
    /// </remarks>
    PartnerPayScore,

    /// <summary>
    /// 银行 / 从业机构支付分（<b>P0-5，11 端点</b>）：授权 3（预授权 + 授权记录查 + 解除授权）+
    /// 服务订单 6（创建 / 查询 / 取消 / 完结 / 修改 / 同步）+ 扣款信息 2（登记 + 查询）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691326"/>。
    /// </summary>
    /// <remarks>
    /// <b>支持商户类型为「从业机构（支付机构）」</b>（官方章节挂在「开发文档(银行/机构)」树），
    /// 与 <see cref="PayModule.PartnerPayScore"/>（服务商面）是两套契约：本域<b>多一层必填的
    /// <c>channel_id</c></b>（从业机构 → 渠道商 → 子商户三级）、只开放<b>免确认模式</b>
    /// （<c>need_user_confirm</c> 固定 <c>false</c>）、<b>无支付分计划与用户签约族</b>
    /// （路由只有 <c>/v3/payscore/acquiringbank/</c> 一套前缀）。
    /// 签名主体是<b>从业机构商户号</b>，<c>sub_mchid</c> / <c>channel_id</c> 均为报文参数而非商户作用域。
    /// SKIT 清单里的「收单曝光」<c>POST /pay/acquiring-bank/transactions/exposure</c> 出处页
    /// <b>已下架</b>（现跳 50x）⇒ 该<b>零端点</b>，<c>Interfaces/BankAcquiring/</c> 不建目录（守卫 BS-B2）。
    /// </remarks>
    BankPayScore,

    /// <summary>
    /// 服务商批量转账到零钱（<b>P0-6，5 端点</b>）：发起批量转账 + 批次查询两支（微信 / 商家批次号）+
    /// 明细查询两支（微信 / 商家明细号）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465738"/>。
    /// </summary>
    /// <remarks>
    /// <b>普通服务商形态</b>：签名主体是服务商商户号，报文用 <c>sub_mchid</c> / <c>sub_appid</c> /
    /// <c>sp_appid</c>。与 <see cref="PayModule.Transfer"/>（直连<b>新版</b>商家转账、按笔发起的
    /// <c>/v3/fund-app/mch-transfer/transfer-bills</c>）
    /// 是两套契约，路由与字段均不互通。
    /// <b>一旦发起不可撤销、报错不得换 <c>out_batch_no</c> 重提</b>（换单即重复出资）；
    /// 批次上限<b>一千</b>笔；字段名是 <c>total_num</c> / <c>batch_status</c> / <c>detail_status</c>
    /// —— 与 <see cref="PayModule.BankTransfer"/> 的 <c>total_count</c> / <c>batch_state</c> /
    /// <c>detail_state</c> 分歧（守卫 PTF-B3/B4）。明细请求 <c>user_name</c>、应答 <c>username</c>。
    /// </remarks>
    PartnerTransfer,

    /// <summary>
    /// 从业机构（银行）批量转账与电子回单（<b>P0-6，9 端点</b>）：转账 5（发起 + 批次查两支 + 明细查两支）+
    /// 电子回单 4（汇总 / 明细各一支 POST 受理 + 一支 GET 查询）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465889"/>。
    /// </summary>
    /// <remarks>
    /// <b>从业机构形态</b>：签名主体是从业机构商户号，包体<b>六层</b>商户标识
    /// （<c>platform_mchid</c> / <c>specialized_payment_mchid</c> / <c>sponsor_mchid</c> /
    /// <c>user_authorized_mchid</c> / <c>user_authorized_appid</c>，应答另加 <c>bank_sp_mchid</c>），
    /// 与 <see cref="PayModule.PartnerTransfer"/> 的两层标识是两套契约（守卫 BT-B3）。
    /// <b>两套前缀同域</b>：转账 <c>/v3/bank-transfer/</c>、回单 <c>/v3/bank-batch-transfer/</c>。
    /// <b>回单族 POST 与 GET 共用同一路径</b> ⇒ 端点计数按 (方法, 路由)（守卫 BT-B1）。
    /// 批次上限<b>三千</b>笔、明细 <c>user_name</c> 必填、状态集<b>无</b> <c>WAIT_PAY</c> / <c>INIT</c>；
    /// <c>download_url</c> 是动态路由 + 非 JSON ⇒ 不进生成式接口（守卫 BT-B2）。
    /// </remarks>
    BankTransfer,

    /// <summary>
    /// 平台商户统一收单·还款（<b>P0-6，3 端点</b>）：还款合单预下单 + 按还款单订单号查询 + 商户关单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015124463"/>。
    /// </summary>
    /// <remarks>
    /// <b>平台商户形态</b>：路由 <c>/v3/repayment/combine-transactions/partner/</c>，
    /// 是「合单支付」（<see cref="PayModule.CombineTransactions"/>）之外的<b>专用面</b> ——
    /// 官方在子单 <c>mchid</c> 一栏原文要求「填写服务商(还款发起方)的商户号 <c>combine_mchid</c>」，
    /// 照抄合单支付的填法会把钱收错账户（守卫 RP-B3）。预下单请求头 <c>Wechatpay-Serial</c> 必填
    /// （实名信息 <c>identity.number</c> / <c>name</c> 须密文）；<b>关单为 204 无包体</b>；
    /// 关单<b>不支持关闭部分还款单</b>且五要素须与下单完全一致。
    /// 端上调起四支与支付成功回调<b>不属本 HTTP 面</b>（守卫 RP-B2）。
    /// </remarks>
    Repayment,

    /// <summary>
    /// 商户风险管理（<b>P0-7，11 端点</b>）：风险订单同步 4（查询两支 + 处置结果回传两支）+
    /// 风险共建 3（上报订单关联信息 / 查询风险信息 / 处理结果回传）+ 违规通知回调地址四件套 4。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013894221"/>。
    /// </summary>
    /// <remarks>
    /// <b>11 个方法只有 9 条 (方法, 路由)</b>：查询风险订单与处置结果回传各自「服务商面 / 普通商户面」
    /// <b>同方法同路由而字段表不同</b> ⇒ 分建两套 DTO、同路由多方法（守卫 MRB-B1 两个数都锁），
    /// 与 <see cref="PayModule.MerchantStore"/> 的「十页五端点、两形态共用」判据相反 ——
    /// 分建还是共用只看官方表是否相同。<b>支付线首次出现 PUT 与 DELETE</b>：同一条
    /// <c>/v3/merchant-risk-manage/violation-notifications</c> 上 PUT 回 200 + JSON、
    /// DELETE 才是 204（守卫 MRB-B1），「写操作都 204」的直觉在本域不成立。
    /// 四套商户标识并存（<c>mchid</c> / <c>sp_mchid</c>+<c>sub_mchid</c> /
    /// 加 <c>acquiring_bank_id</c>+<c>channel_id</c> / 子单 <c>mch_id</c> 带下划线）；
    /// 同名 <c>risk_type</c> 有两套类型（同步面 integer 1~4、共建面 string 5 值）；
    /// 官方误拼 <c>risk_discove_time</c>（缺 r）与 <c>same_deviced</c>（多 d）照页。
    /// 上报订单关联信息是本域唯一把 <c>Wechatpay-Serial</c> 列成参数表行的页面
    /// （<c>phone</c> / <c>certificates_number</c> 须密文）；十页零端点级频率数字。
    /// </remarks>
    MerchantRiskManage,

    /// <summary>
    /// 商户门店管理（<b>P0-7，5 端点</b>）：创建 + 更新（PATCH·204）+ 查询 + 绑定收款号 + 解绑收款号。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/>。
    /// </summary>
    /// <remarks>
    /// <b>十个文档页（商户面 + 服务商面各五）归并为五端点</b>：两侧路由逐字符相同、字段表只差一个
    /// <c>sub_mchid</c>（查询面在 query、其余在包体）、应答表零差异 ⇒ 共用方法与 DTO，
    /// 本域<b>没有</b> <c>Partner</c> 前缀分身（守卫 MST-B6）。<b>支付线第一支 PATCH</b>，
    /// 且更新支成功是 <b>204 无包体</b>（守卫 MST-B1）。
    /// <b>写支成功 ≠ 生效</b>：本域是三方确认流（收款商户号超管确认 + 地址核实），
    /// 唯一闭环路径是查询支读 <c>store_state</c>（六值状态集，三档在等对方动作）。
    /// 门店面无任何加密字段、十页零频率数字；官方<b>没有</b>「分页查门店」与「删除门店」两支 HTTP 面
    /// （删除须登录商户平台）⇒ 零端点由守卫 MST-B2 带正对照锁定。
    /// </remarks>
    MerchantStore,

    /// <summary>
    /// 商户资料变更（<b>P0-7，12 端点</b>）：主体 4 + 超级管理员 4 + 商家简称 4，各「提交 + 两个查询 + 撤销」。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>。
    /// </summary>
    /// <remarks>
    /// <b>三条独立路由族、共用与分建逐类型裁定</b>：<c>mchsubjectalterapplyment</c> /
    /// <c>mchcontactalterapplyment</c> / <c>merchantnamealterapplyment</c>；三族「提交」应答表相同 ⇒ 共用一型，
    /// 超管与简称的「撤销」应答表相同 ⇒ 共用一型，而主体族撤销回 <c>{result}</c>（<c>SUCCESS</c> / <c>FAIL</c>）⇒ 分建；
    /// 三族「查询」各自一型（主体族 6 值含 <c>EDITTING</c>、超管族 5 值，简称族另有 <c>sub_mchid</c> 且<b>无</b>
    /// <c>audit_reject_detail</c>）。<b>定位键分族</b>：主体与超管用 <c>merchant_code</c>、简称族用
    /// <c>sub_mchid</c>（路径段名同为 <c>merchant/</c>）⇒「按业务单号查」不是一条模板（守卫 MAA-B1）。
    /// <b>加密头表行只覆盖两支</b>：主体族与超管族的提交页把 <c>Wechatpay-Serial</c> 列为必填（姓名/证件号/住址/手机号须密文），
    /// 简称族全明文 ⇒ 不按「同域都要加密」处理（守卫 MAA-B5）。
    /// 官方误拼 <c>APPLYMENT_STATE_EDITTING</c>（三 T）与 <c>APPLYMENT_STATE_MODIFING</c>（单 F）照页；
    /// <c>out_request_no</c> 长度在提交侧 64/124/124、在查询应答侧一律 64（同域两页不一致，官方原文如此）。
    /// <b>提交只是受理</b>：三族都是审核流，唯一闭环路径是轮询查询支读 <c>state</c>；
    /// 驳回后用<b>同一</b> <c>out_request_no</c> 重提才是覆盖，换号即产生第二笔申请单。
    /// 与 <see cref="PayModule.Apply4SubMerchant"/> / <see cref="PayModule.Apply4Subject"/> 是三个独立契约，
    /// 逐字比对后只有一个同名嵌套对象 ⇒ <b>跨域零复用</b>。十二页零端点级频率数字。
    /// </remarks>
    MerchantAlterApply,

    /// <summary>
    /// 商户账户资金管理（<b>P0-7，2 端点</b>）：查询账户实时余额 + 查询账户日终余额。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012721477"/>。
    /// </summary>
    /// <remarks>
    /// <b>本域的存在本身就是裁决收窄的证据</b>：资金面守卫从「零 <c>/v3/merchant/fund</c> 路由」收窄为
    /// 「零 <c>/v3/merchant/fund/withdraw</c> 路由」，判据是逐页「支持商户」一栏——这两支是【普通服务商】，
    /// 提现族十一页只标【平台商户】⇒ 保持零端点，且守卫 MF-B2 <b>以本域两支作正对照</b>（AGENTS §6
    /// 「扫描必须以已落位相邻路由作正对照」），防止正则漂移使断言静默恒真。
    /// 两支 GET、同前缀 <c>/v3/merchant/fund</c>、<b>应答表逐字段相同</b> ⇒ 共用一型；日终支多一个必填 query
    /// <c>date</c>(<c>YYYY-MM-DD</c>)。<c>account_type</c> 落 <b>path</b>（三值 <c>BASIC</c>/<c>OPERATION</c>/<c>FEES</c>），
    /// 未开通账户是 <b>400 <c>INVALID_REQUEST</c></b>（「输入账户类型未开通」）而不是 404。
    /// <b>金额单位是分且以 <c>long</c> 承载</b>（官方 integer，余额量级可越出 int32）；
    /// <c>pending_amount</c> 官方标选填 ⇒ 缺失不等于 0。<b>余额 ≠ 可提现头寸</b>（还要扣在途与冻结），
    /// 也不用于对账（对账走 <see cref="PayModule.Bill"/> 的资金账单）。两页零加密字段、零频率数字。
    /// </remarks>
    MerchantFund,

    /// <summary>
    /// 商户结算规则申请（<b>P0-7，2 端点</b>）：创建结算规则申请单 + 查询申请单状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697924"/>。
    /// </summary>
    /// <remarks>
    /// <b>从业机构面</b>：两支页面的「支持商户」都是【从业机构（银行）】【从业机构（支付机构）】⇒ 标识是
    /// <c>acquiring_bank_id</c> + <c>channel_id</c> + <c>sub_mchid</c> 三层，<b>没有</b> <c>sp_</c>/<c>sub_appid</c>
    /// 那套服务商前缀，也不与 <see cref="PayModule.BankTransfer"/> 的六层标识互换（守卫 MSET-B3）。
    /// 两支同前缀 <c>/v3/merchant-settlement/merchant-settle-rule-applications</c>（POST 无尾段、
    /// GET 带 <c>/{application_id}</c>），<b>应答表逐字段相同 ⇒ 共用一型</b>（守卫 MSET-B6）。
    /// <c>settle_rule_id</c> 官方是 <b>integer</b>（非 string）；<c>update_time</c> 是 rfc3339 字符串 ⇒ 按全域口径落 <c>string</c>。
    /// <b>创建 ≠ 生效</b>：状态三值只有 <c>PASSED</c>/<c>FAILED</c> 是终态，创建应答可为 <c>PROCESSING</c>；
    /// <c>application_process_info</c> 是给人看的描述串、不是结构化进度，判状态只看 <c>application_state</c>。
    /// <b>只有申请单号一条定位路径</b>（官方无「按商户号查最新申请单」）⇒ <c>application_id</c> 必须落库，
    /// 丢了就没有查询通道（守卫 MSET-B2）。两页零加密字段、零频率数字。
    /// </remarks>
    MerchantSettlement,

    /// <summary>
    /// 消费者投诉（<b>P0-7，11 端点 + 2 手工通道</b>）：通知地址 CRUD + 投诉列表 / 详情 / 协商历史 +
    /// 回复 / 完成 / 退款审批 + 即时服务 + 图片上传 / 拉取。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>。
    /// </summary>
    /// <remarks>
    /// <b>本域是支付线唯一同时含 POST/PUT/GET/DELETE 四动词的域</b>：通知地址四支共用同一路由
    /// <c>/v3/merchant-service/complaint-notifications</c>，其中 POST/PUT 请求体相同、GET/POST/PUT 应答相同
    /// （守卫 MC-B6），DELETE 为 204 无包体。<b>写支返回形态分裂</b>：回复 / 完成 / 退款审批三支为 204 无包体，
    /// 即时服务支为 200 + JSON（回 <c>log_id</c>）。<b>图片通道独立</b>：上传（multipart）与拉取（二进制流）
    /// 走手工通道 <see cref="MerchantService.IWechatPayComplaintMediaService"/>，因为无法用声明式表达。
    /// <b>首次回复即改变状态</b>（待处理 → 处理中），但受理 ≠ 可用，仍须经查询支读 <c>complaint_state</c> 闭环。
    /// <b><c>complainted_mchid</c> 在写支间不一致</b>：回复 / 完成有该字段、退款审批支<b>没有</b>（守卫 MC-B3）。
    /// <b><c>idempotent_id</c> 必填</b>：即时服务支官方提供幂等闸，但须宿主生成（留空即 <c>PARAM_ERROR</c>）。
    /// 十一页零端点级频率数字、零加密字段。
    /// </remarks>
    MerchantService,
}
