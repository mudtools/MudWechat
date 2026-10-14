// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Bill;
using Mud.Wechat.Pay.DataModels.Certificates;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.Refund;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 支付线 P1-a 核心域架构守卫（方案 §2.8 PAY-B1 / PAY-B5 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要本守卫</b>：APIv3 与另两条产品线的令牌范式<b>根本不同</b>——凭据是商户 RSA 私钥签名
/// （不是 <c>access_token</c>），失败是 HTTP 4xx + 字符串 <c>code</c>（不是 HTTP 200 + 整数 <c>errcode</c>）。
/// 这些差异若在增量落地时被「顺手」按公众号形态改写，运行期症状是「令牌注入静默退化」「错误体丢失、
/// 排查时只见 ApiException」——排查成本极高，故必须在契约面锁死。
/// </para>
/// </remarks>
public class WechatPayDomainContractGuards
{
    /// <summary>
    /// PAY-B1：支付线<b>全部</b> <c>[HttpClientApi]</c> 接口<b>不得</b>声明 <c>[Token]</c>，
    /// 且各域接口数量须与模块枚举一致（防漏注册 / 防误加令牌）。
    /// </summary>
    /// <remarks>
    /// 理由链见 <c>WechatPayScaffoldContractGuards.TokenInjectionMode_ShouldHaveNoAsymmetricSignatureMode_WhenScaffold</c>
    /// （<c>HmacSignature</c> 是对称 HMAC、输出单标量，装不下 APIv3 的五字段 <c>Authorization</c>）。
    /// </remarks>
    [Fact]
    public void PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount()
    {
        var interfaces = LoadPayAssembly()
            .GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "HttpClientApiAttribute"))
            .OrderBy(static t => t.Name, StringComparer.Ordinal)
            .ToArray();

        // 聚合计数（防枚举空跑，AGENTS §6）：二十四域接口 = Transactions / Refund / Bill / Certificates / ProfitSharing /
        // PayScore / CombineTransactions / Transfer / NewTaxControlFapiao / MarketingFavor / Apply4SubMerchant /
        // Apply4Subject / PartnerTransactions / PartnerPayScore / BankPayScore / PartnerTransfer / BankTransfer /
        // Repayment / MerchantRiskManage / MerchantStore / MerchantAlterApply / MerchantFund / MerchantSettlement /
        // MerchantService。
        // 数字集中维护在 Tests/ContractBaseline.cs；数量变化须同批更新 PayModule 与该基线。
        interfaces.Should().HaveCount(Baseline.Pay.HttpApiInterfaces,
            "支付线二十四域接口——数量变化须同批更新 PayModule 与 Tests/ContractBaseline.cs");

        interfaces.Select(static t => t.Name).Should().BeEquivalentTo(new[]
        {
            "IWechatPayTransactionsService",
            "IWechatPayRefundService",
            "IWechatPayBillService",
            "IWechatPayCertificatesService",
            "IWechatPayProfitSharingService",
            "IWechatPayPayScoreService",
            "IWechatPayCombineService",
            "IWechatPayTransferService",
            "IWechatPayFapiaoService",
            "IWechatPayMarketingFavorService",
            "IWechatPayApply4SubMerchantService",
            "IWechatPayApply4SubjectService",
            "IWechatPayPartnerTransactionsService",
            "IWechatPayPartnerPayScoreService",
            "IWechatPayBankPayScoreService",
            "IWechatPayPartnerTransferService",
            "IWechatPayBankTransferService",
            "IWechatPayRepaymentService",
            "IWechatPayMerchantRiskManageService",
            "IWechatPayMerchantStoreService",
            "IWechatPayMerchantAlterApplyService",
            "IWechatPayMerchantFundService",
            "IWechatPayMerchantSettlementService",
            "IWechatPayMerchantService",
        });

        var offenders = interfaces
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "TokenAttribute"))
            .ToArray();

        offenders.Should().BeEmpty(
            "支付线凭据为商户 RSA 私钥签名，不存在 access_token；[Token] 注入会静默退化（方案 §2.8 PAY-B1）：" +
            string.Join(", ", offenders.Select(static t => t.FullName)));
    }

    /// <summary>
    /// PAY-B5：端点计数（<b>160</b>）+ 官方路由表逐条比对（照官方原文，<b>不得「纠正」</b>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 路由来源（2026-10-09 逐页核验普通商户文档中心）：
    /// <c>4012791897</c> 下单 / <c>4012791899</c> 微信支付订单号查单 / <c>4012791900</c> 商户订单号查单 /
    /// <c>4012791901</c> 关单 / <c>4012791903</c> 退款申请 / <c>4012791904</c> 查询退款 /
    /// <c>4012791905</c> 异常退款 / <c>4012791907</c> 交易账单 / <c>4012791908</c> 资金账单 /
    /// <c>4012551764</c> 平台证书。
    /// </para>
    /// <para>
    /// <b>下单四族的路由来源</b>：JSAPI <c>4012791897</c> 为本仓逐页核验过的锚点；Native <c>4012791877</c> /
    /// APP <c>4013070347</c> / H5 <c>4012791834</c> 三页 docId 与路由取自本地 SKIT <c>TenpayV3</c>
    /// 的登记（2026-10-10 对齐），<b>官方逐页核验待补</b>（文档中心为 SPA，正文不可达）。
    /// 本守卫只锁<b>路由字符串</b>与<b>已对齐的 DTO 形态</b>，不声称字段表已经官方核验。
    /// </para>
    /// <para>
    /// <b>P2 分账首批</b>（同日核验）：<c>4012524936</c> 请求分账 / <c>4012528995</c> 添加分账接收方 /
    /// <c>chapter8_1_2</c> 查询分账结果。
    /// </para>
    /// <para>
    /// <b>P0-1 特约商户进件</b>（2026-10-14 逐页核验服务商文档中心）：<c>4012719997</c> 提交申请单 /
    /// <c>4012697052</c> 与 <c>4012697168</c> 两路查询申请状态 / <c>4012761102</c> 修改结算账户 /
    /// <c>4012761113</c> 查询结算账户 / <c>4012761169</c> 查询结算账户修改申请状态。
    /// </para>
    /// <para>
    /// <b>P0-2 主体进件 / 商户开户意愿确认</b>（2026-10-14 逐页核验服务商文档中心）：
    /// <c>4012722388</c> 提交申请单 / <c>4012697715</c> 查询申请单审核结果 /
    /// <c>4012697627</c> 撤销申请单（同页两个 path 变体）/ <c>4012467549</c> 获取商户开户意愿确认状态。
    /// </para>
    /// <para>
    /// <b>P0-3 服务商基础支付 + 分期</b>（2026-10-14 逐页核验服务商文档中心）：<c>4012714911</c> JSAPI/小程序下单 /
    /// <c>4012714902</c> Native / <c>4012714669</c> APP / <c>4012714759</c> H5 / <c>4012492476</c> 扫码支付 /
    /// <c>4012738964</c> 订单号查单 / <c>4012739008</c> 商户订单号查单 / <c>4012739019</c> 关单 /
    /// <c>4016196820</c> 分期曝光咨询。<b>SKIT 清单 12 行 ⇒ 9 端点</b>：
    /// <c>codepay</c> 与 <c>reverse</c> 的现行正文是付款码支付（V2）指引页（无 APIv3 字段表，归 APIv2 批次）、
    /// <c>transactions/exposure</c> 已下架（302→50x）⇒ 三支零端点；查单与关单的其余 docId 是同一端点的产品族分身页。
    /// </para>
    /// <para>
    /// <b>P0-4 服务商支付分</b>（2026-10-14 逐页核验服务商文档中心）：<c>4012382030</c> 商户预授权 /
    /// <c>4012778802</c> 场景预授权 / <c>4012783585</c> 评估用户分层 / <c>4012692687</c> 与
    /// <c>4012692798</c> 授权记录两路查询 / <c>4012382049</c> 与 <c>4012461154</c> 解除授权两变体 /
    /// <c>4012692136</c> 创建订单 / <c>4012692256</c> 查询订单 / <c>4012458758</c> 取消 /
    /// <c>4012692408</c> 完结 / <c>4012692360</c> 修改 / <c>4013394914</c> 未完结提醒 /
    /// <c>4013394929</c> 确认付款提醒 / <c>4012458816</c> 同步支付结果 / <c>4013394486</c> 服务额度 /
    /// <c>4012463290</c> 创建计划 / <c>4012463318</c> 查询计划 / <c>4012463322</c> 停止计划 /
    /// <c>4012463469</c> 计划型服务订单 / <c>4012463521</c> 创建用户签约计划 / <c>4012463526</c> 查询 /
    /// <c>4012463534</c> 停止签约计划（23 页逐页取证，页内 URL 已落在接口 XML 注释）。
    /// <b>SKIT 清单 28 行 ⇒ 23 端点</b>：「商户收款」<c>/v3/payscore/serviceorder/{out_order_no}/pay</c>
    /// 服务商侧现行目录无该端点 ⇒ 零声明（守卫 PS-B2），授权记录查询与计划应答的重复行按同端点归并。
    /// </para>
    /// <para>
    /// <b>P0-5 银行/从业机构支付分</b>（2026-10-14 逐页核验合作伙伴文档中心「开发文档(银行/机构)」树 ·
    /// 「从业机构-微信支付分（免确认模式）」，各页「支持商户」栏恒为【从业机构（支付机构）】）：
    /// <c>4012691489</c> 从业机构预授权 / <c>4012461004</c> 查询用户授权记录（授权协议号）/
    /// <c>4012461012</c> 解除用户授权关系（授权协议号）/ <c>4012691326</c> 创建支付分订单 /
    /// <c>4012691343</c> 查询支付分订单 / <c>4012458146</c> 取消 / <c>4012460791</c> 完结 /
    /// <c>4012691363</c> 修改订单金额 / <c>4012691378</c> 同步订单状态 /
    /// <c>4012458414</c> 登记扣款信息 / <c>4012458665</c> 查询扣款信息
    /// （11 页逐页取证，页内 URL 已落在接口 XML 注释）。
    /// <b>SKIT 清单 15 行 ⇒ 11 端点</b>：「收单曝光」<c>/pay/acquiring-bank/transactions/exposure</c>
    /// 出处页 <c>4014326541</c> 现跳 50x（公益 404）⇒ 零声明、<c>Interfaces/BankAcquiring/</c> 不建目录
    ///（守卫 BS-B2）；查询授权记录与查询订单两行在清单里重复声明同一端点，已按同端点归并。
    /// </para>
    /// <para>
    /// <b>P0-6 三域转账 / 回单 / 还款</b>（2026-10-14 逐页核验合作伙伴文档中心，
    /// 各页「支持商户」栏：服务商转账【普通服务商】、银行转账与回单【从业机构（银行）】、
    /// 还款【平台商户】）：服务商批量转账 <c>4012465738</c> 发起 / <c>4012465772</c>、
    /// <c>4012465790</c> 批次两路 / <c>4012465781</c>、<c>4012465795</c> 明细两路（5 端点）；
    /// 银行批量转账 <c>4012465889</c> 发起 / <c>4012465891</c>、<c>4012465903</c> 批次两路 /
    /// <c>4012465897</c>、<c>4012465915</c> 明细两路，电子回单 <c>4012470746</c>、<c>4012470761</c>
    /// 汇总（POST 与 GET <b>同一路径</b>）、<c>4012470770</c>、<c>4012470775</c> 明细
    /// （9 端点，<b>按 (方法, 路由) 计数</b>）；还款 <c>4015124463</c> 预下单 /
    /// <c>4015124483</c> 查单 / <c>4015124497</c> 关单（3 端点，关单为 204）。
    /// <b>SKIT 清单 17 行 ⇒ 17 端点、零归并</b>；三域各自的下架 / 非本面裁决见守卫 PTF-B2、BT-B2、RP-B2。
    /// </para>
    /// <para>
    /// <b>P0-7 首批商户经营两域</b>（2026-10-14 逐页核验，两轮共覆盖该批 71 行清单）：商户风险管理
    /// <c>4013894221</c>/<c>4013894247</c> 查询风险订单（同路由两形态）、<c>4013894227</c>/<c>4013894250</c>
    /// 回传处置结果（同路由两形态）、<c>4012692160</c> 上报订单关联信息、<c>4012467535</c> 查询风险信息、
    /// <c>4012467540</c> 处理结果回传、<c>4012471333</c>/<c>4012471330</c>/<c>4012471327</c>/<c>4012471334</c>
    /// 违规通知回调地址四件套 ⇒ <b>11 个方法 / 9 条 (方法, 路由)</b>（守卫 MRB-B1 两个数都锁）；
    /// 商户门店管理十页（创建 / 更新 / 查询 / 绑定 / 解绑 各成对）⇒ <b>归并为 5 端点</b>（守卫 MST-B6）。
    /// <b>本批是支付线动词面的破例</b>：此前 <c>Src/Pay</c> 的生成式接口只有 GET/POST，本批出现
    /// PUT / PATCH / DELETE，且「成功应答是 204 还是 JSON」逐支相反（风险域只有 DELETE 是 204、
    /// 门店域只有 PATCH 是 204）⇒ 由 MRB-B1 / MST-B1 逐支断言返回类型，
    /// 并断言「按 <c>HttpMethodAttribute</c> 基类枚举」与「按五个具体动词属性枚举」得到同一集合。
    /// </para>
    /// <para>
    /// <b>P0-7 7-B 商户经营三域</b>（2026-10-14 十六页逐字段核验，各页「支持商户」栏是本轮形态归属的判据）：
    /// 商户资料变更 12 端点 —— 主体族 <c>4014090649</c>/<c>4014090655</c>/<c>4014090667</c>/<c>4014090672</c>、
    /// 超级管理员族 <c>4015617906</c>/<c>4015617911</c>/<c>4015617915</c>/<c>4015617919</c>、
    /// 商家简称族 <c>4016166071</c>/<c>4016166074</c>/<c>4016166085</c>/<c>4016166090</c>
    /// （<b>清单把主体族四个 docId 同时挂在联系人族四行上</b>，逐页核得两族各有独立页面 ⇒ 八行是八个页面，
    /// 守卫 MAA-B2 锁转述形态零声明）；
    /// 商户账户余额 2 端点 <c>4012721477</c> 实时 / <c>4012721446</c> 日终（【普通服务商】），
    /// 同前缀的提现族四支（<c>4012476670</c> 等）标注【平台商户】⇒ 零端点，裁决守卫 C1 因此
    /// 由「零 <c>/v3/merchant/fund</c>」<b>收窄为</b>「零 <c>/v3/merchant/fund/withdraw</c>」；
    /// 商户结算规则申请 2 端点 <c>4012697924</c> 创建 / <c>4012698574</c> 查状态（【从业机构（银行）】
    /// 【从业机构（支付机构）】，标识体系是 acquiring_bank_id + channel_id + sub_mchid 三层）。
    /// <b>三域三种归并结果并置</b>：变更域 12 页 ⇒ 12 支（三族字段表各异）、门店域 10 页 ⇒ 5 支（字段表只差
    /// 一个 sub_mchid）、风险域 9 路由 ⇒ 11 方法（同路由两形态）——判据始终是「官方字段表是否相同」。
    /// </para>
    /// </remarks>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutes()
    {
        var asm = LoadPayAssembly();

        RoutesOf(asm, "IWechatPayTransactionsService").Should().BeEquivalentTo(new[]
        {
            "/v3/pay/transactions/jsapi",
            "/v3/pay/transactions/native",
            "/v3/pay/transactions/app",
            "/v3/pay/transactions/h5",
            "/v3/pay/transactions/id/{transactionId}",
            "/v3/pay/transactions/out-trade-no/{outTradeNo}",
            "/v3/pay/transactions/out-trade-no/{outTradeNo}/close",
        });

        RoutesOf(asm, "IWechatPayRefundService").Should().BeEquivalentTo(new[]
        {
            "/v3/refund/domestic/refunds",
            "/v3/refund/domestic/refunds/{outRefundNo}",
            "/v3/refund/domestic/refunds/{refundId}/apply-abnormal-refund",
        });

        RoutesOf(asm, "IWechatPayBillService").Should().BeEquivalentTo(new[]
        {
            "/v3/bill/tradebill",
            "/v3/bill/fundflowbill",
        });

        RoutesOf(asm, "IWechatPayCertificatesService").Should().BeEquivalentTo(new[]
        {
            "/v3/certificates",
        });

        // P2 分账 7 端点：接收方入库 + 请求分账 + 查询 + 回退请求 + 回退查询 + 解冻剩余资金 + 查剩余待分金额。
        // ⚠️ 「回退单」是独立资源族（/v3/profitsharing/return-orders…），不是分账单的子资源（官方实测）。
        RoutesOf(asm, "IWechatPayProfitSharingService").Should().BeEquivalentTo(new[]
        {
            "/v3/profitsharing/receivers/add",
            "/v3/profitsharing/orders",
            "/v3/profitsharing/orders/{outOrderNo}",
            "/v3/profitsharing/orders/unfreeze",
            "/v3/profitsharing/return-orders",
            "/v3/profitsharing/return-orders/{outReturnNo}",
            "/v3/profitsharing/transactions/{transactionId}/amounts",
            "/v3/profitsharing/bills",
            "/v3/profitsharing/receivers/delete",
        });

        // P2 支付分 8 端点：服务订单 6（创建/查询/取消/完结/修改/催收扣款）+ 授权面 2（预授权/查授权记录）。
        RoutesOf(asm, "IWechatPayPayScoreService").Should().BeEquivalentTo(new[]
        {
            "/v3/payscore/serviceorder",
            "/v3/payscore/serviceorder",
            "/v3/payscore/serviceorder/{outOrderNo}/cancel",
            "/v3/payscore/serviceorder/{outOrderNo}/complete",
            "/v3/payscore/serviceorder/{outOrderNo}/modify",
            "/v3/payscore/serviceorder/{outOrderNo}/pay",
            "/v3/payscore/permissions",
            "/v3/payscore/permissions/authorization-code/{authorizationCode}",
            "/v3/payscore/serviceorder/{outOrderNo}/sync",
            "/v3/payscore/permissions/authorization-code/{authorizationCode}/terminate",
            "/v3/payscore/permissions/openid/{openId}",
        });

        // P2 合单支付 6 端点（本域按**服务商面**建模）：四个下单场景 + 关单 + 查询。
        // ⚠️ 无「合单退款」路由 —— 官方明示合单订单只能按**子单**退款（走退款域），守卫 CB6 固化。
        RoutesOf(asm, "IWechatPayCombineService").Should().BeEquivalentTo(new[]
        {
            "/v3/combine-transactions/jsapi",
            "/v3/combine-transactions/native",
            "/v3/combine-transactions/app",
            "/v3/combine-transactions/h5",
            "/v3/combine-transactions/out-trade-no/{combineOutTradeNo}/close",
            "/v3/combine-transactions/out-trade-no/{combineOutTradeNo}",
        });

        // P2 商家转账 6 端点：发起 + 两查询 + 撤销 + 两电子回单查询（两组各自共用应答 DTO）。
        // ⚠️ 撤销走的是**普通商户**面路由；服务商侧的 /partner/ 变体属另一套文档，不得混入。
        RoutesOf(asm, "IWechatPayTransferService").Should().BeEquivalentTo(new[]
        {
            "/v3/fund-app/mch-transfer/transfer-bills",
            "/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}",
            "/v3/fund-app/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}",
            "/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel",
            "/v3/fund-app/mch-transfer/elecsign/out-bill-no/{outBillNo}",
            "/v3/fund-app/mch-transfer/elecsign/transfer-bill-no/{transferBillNo}",
        });

        // P2 电子发票首批 2 端点（普通商户侧）：开具 + 查询。
        // P2 电子发票 5 端点：开具 + 查询 + 冲红 + 获取下载信息 + 插入卡包。
        // （《上传电子发票文件》是 multipart、非 JSON 端点；《下载发票文件》按 30s 有效 URL 直取且不签名验签 ⇒ 均不在本生成式接口内。）
        RoutesOf(asm, "IWechatPayFapiaoService").Should().BeEquivalentTo(new[]
        {
            "/v3/new-tax-control-fapiao/fapiao-applications",
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}",
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/reverse",
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/fapiao-files",
            "/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/insert-cards",
        });

        // P2 代金券 7 端点：创建批次 + 查询券详情 + 激活/暂停/重启批次 + 查询批次详情 + 发放。
        // ⚠️ 两族路径前缀**不一致**：创建走 /coupon-stocks，批次管理四个动作走 /stocks/…（官方原文如此）。
        RoutesOf(asm, "IWechatPayMarketingFavorService").Should().BeEquivalentTo(new[]
        {
            "/v3/marketing/favor/coupon-stocks",
            "/v3/marketing/favor/users/{openId}/coupons/{couponId}",
            "/v3/marketing/favor/stocks/{stockId}/start",
            "/v3/marketing/favor/stocks/{stockId}/pause",
            "/v3/marketing/favor/stocks/{stockId}/restart",
            "/v3/marketing/favor/stocks/{stockId}",
            "/v3/marketing/favor/users/{openId}/coupons",
        });

        // P0-1 特约商户进件 6 端点：申请单族 3（提交 + 两路状态查询，无 sub_mchid path）+
        // 结算账户族 3（修改 / 查当前账户 / 查修改申请，恒以 sub_mchid 定位）。
        // ⚠️ 两族前缀互为镜像（applyment4sub vs apply4sub），逐条锁定见守卫 A4S-B1 / A4S-B2。
        RoutesOf(asm, "IWechatPayApply4SubMerchantService").Should().BeEquivalentTo(new[]
        {
            "/v3/applyment4sub/applyment/",
            "/v3/applyment4sub/applyment/applyment_id/{applymentId}",
            "/v3/applyment4sub/applyment/business_code/{businessCode}",
            "/v3/apply4sub/sub_merchants/{subMchId}/modify-settlement",
            "/v3/apply4sub/sub_merchants/{subMchId}/settlement",
            "/v3/apply4sub/sub_merchants/{subMchId}/application/{applicationNo}",
        });

        // P0-2 主体进件 / 商户开户意愿确认 5 端点：单前缀 /v3/apply4subject/，
        // ⚠️ 提交 POST 带尾斜杠而查询 GET 不带（同资源异形），撤销是两个 path 变体，
        //    与特约商户进件的 applyment4sub / apply4sub 两族前缀均不同 —— 逐条锁定见 A4SJ-B1 / A4SJ-B2。
        RoutesOf(asm, "IWechatPayApply4SubjectService").Should().BeEquivalentTo(new[]
        {
            "/v3/apply4subject/applyment/",
            "/v3/apply4subject/applyment",
            "/v3/apply4subject/applyment/{businessCode}/cancel",
            "/v3/apply4subject/applyment/{applymentId}/cancel",
            "/v3/apply4subject/applyment/merchants/{subMchId}/state",
        });

        // P0-3 服务商基础支付 + 分期 9 端点：8 条在 /v3/pay/partner/transactions/ 下、
        // 分期曝光咨询在 /v3/pay/instal/partner/exposure（唯一前缀例外）；
        // 与直连域 /v3/pay/transactions/ 前缀互斥，SKIT 清单的 codepay / reverse（V2 面）
        // 与 transactions/exposure（页面已下架）三支零端点 —— 逐条锁定见 PT-B1 / PT-B2。
        RoutesOf(asm, "IWechatPayPartnerTransactionsService").Should().BeEquivalentTo(new[]
        {
            "/v3/pay/partner/transactions/jsapi",
            "/v3/pay/partner/transactions/native",
            "/v3/pay/partner/transactions/app",
            "/v3/pay/partner/transactions/h5",
            "/v3/pay/partner/transactions/scannedpos",
            "/v3/pay/partner/transactions/id/{transactionId}",
            "/v3/pay/partner/transactions/out-trade-no/{outTradeNo}",
            "/v3/pay/partner/transactions/out-trade-no/{outTradeNo}/close",
            "/v3/pay/instal/partner/exposure",
        });

        // P0-4 服务商支付分 23 端点：三套前缀（/v3/payscore/partner/ 16 + /v3/payscore/plan/partner/ 3 +
        // /v3/payscore/sign-plan/partner/ 4），与直连支付分域 /v3/payscore/ 前缀互斥；
        // SKIT 清单的「商户收款」/v3/payscore/serviceorder/{out_order_no}/pay 本域零声明
        //（正对照：直连域确有该端点）—— 逐条锁定见 PS-B1 / PS-B2。
        RoutesOf(asm, "IWechatPayPartnerPayScoreService").Should().BeEquivalentTo(new[]
        {
            "/v3/payscore/partner/permissions",
            "/v3/payscore/partner/permissions/apply-for-scene",
            "/v3/payscore/partner/permissions/assess-user-risk-level",
            "/v3/payscore/partner/permissions/authorization-code/{authorizationCode}",
            "/v3/payscore/partner/permissions/search",
            "/v3/payscore/partner/permissions/authorization-code/{authorizationCode}/terminate",
            "/v3/payscore/partner/permissions/terminate",
            "/v3/payscore/partner/serviceorder",
            "/v3/payscore/partner/serviceorder",
            "/v3/payscore/partner/serviceorder/{outOrderNo}/cancel",
            "/v3/payscore/partner/serviceorder/{outOrderNo}/complete",
            "/v3/payscore/partner/serviceorder/{outOrderNo}/modify",
            "/v3/payscore/partner/serviceorder/{outOrderNo}/send-pendingmessage",
            "/v3/payscore/partner/serviceorder/{outOrderNo}/send-prepaidmessage",
            "/v3/payscore/partner/serviceorder/{outOrderNo}/sync",
            "/v3/payscore/partner/servicequotas/service-id/{serviceId}",
            "/v3/payscore/plan/partner/payscore-plans",
            "/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}",
            "/v3/payscore/plan/partner/payscore-plans/merchant-plan-no/{merchantPlanNo}/stop",
            "/v3/payscore/sign-plan/partner/serviceorder",
            "/v3/payscore/sign-plan/partner/user-sign-plans",
            "/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}",
            "/v3/payscore/sign-plan/partner/user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}/stop",
        });

        // P0-5 银行/从业机构支付分 11 端点：**单一前缀** /v3/payscore/acquiringbank/（授权 3 + 服务订单 6 +
        // 扣款信息 2），与服务商面 /v3/payscore/partner/ 和直连面 /v3/payscore/ 两两互斥（正对照：那两域的路由
        // 已各自在上方成块）；SKIT 清单的「收单曝光」/pay/acquiring-bank/transactions/exposure 出处页已下架
        //（现跳 50x）⇒ 本仓零声明 —— 逐条锁定见 BS-B1 / BS-B2。
        RoutesOf(asm, "IWechatPayBankPayScoreService").Should().BeEquivalentTo(new[]
        {
            "/v3/payscore/acquiringbank/permissions",
            "/v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}",
            "/v3/payscore/acquiringbank/permissions/authorization-code/{authorizationCode}/terminate",
            "/v3/payscore/acquiringbank/serviceorder",
            "/v3/payscore/acquiringbank/serviceorder",
            "/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/cancel",
            "/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/complete",
            "/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/modify",
            "/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/sync",
            "/v3/payscore/acquiringbank/serviceorder/{outOrderNo}/registerdeductinfo",
            "/v3/payscore/acquiringbank/serviceorder/deduction",
        });

        RoutesOf(asm, "IWechatPayPartnerTransferService").Should().BeEquivalentTo(new[]
        {
            "/v3/partner-transfer/batches",
            "/v3/partner-transfer/batches/batch-id/{batchId}",
            "/v3/partner-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}",
            "/v3/partner-transfer/batches/out-batch-no/{outBatchNo}",
            "/v3/partner-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}",
        });

        // 银行面：转账五支在 /v3/bank-transfer/、回单四支在 /v3/bank-batch-transfer/，
        // 且回单的汇总 / 明细两条路径各有 POST 与 GET ⇒ 期望集里**同名路由出现两次**（计数按方法+路由）。
        RoutesOf(asm, "IWechatPayBankTransferService").Should().BeEquivalentTo(new[]
        {
            "/v3/bank-transfer/batches",
            "/v3/bank-transfer/batches/batch-id/{batchId}",
            "/v3/bank-transfer/batches/batch-id/{batchId}/details/detail-id/{detailId}",
            "/v3/bank-transfer/batches/out-batch-no/{outBatchNo}",
            "/v3/bank-transfer/batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}",
            "/v3/bank-batch-transfer/receipt/summary-receipts",
            "/v3/bank-batch-transfer/receipt/summary-receipts",
            "/v3/bank-batch-transfer/receipt/detail-receipts",
            "/v3/bank-batch-transfer/receipt/detail-receipts",
        });

        RoutesOf(asm, "IWechatPayRepaymentService").Should().BeEquivalentTo(new[]
        {
            "/v3/repayment/combine-transactions/partner/prepay",
            "/v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}",
            "/v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}/close",
        });

        // P0-7 商户风险管理 11 端点（**按方法计**）：单一前缀 /v3/merchant-risk-manage/，
        // 但去重后只有 9 条 (方法, 路由) —— query-risk-trades 与 recv-dispose-result 各有
        // 「服务商面 / 普通商户面」两支同方法同路由、字段表不同的方法；violation-notifications
        // 一条路径上并置 POST / PUT / GET / DELETE 四面（只有 DELETE 是 204）。
        // 本批是支付线**首次出现非 GET/POST 动词**——逐条锁定见 MRB-B1 / MRB-B2。
        RoutesOf(asm, "IWechatPayMerchantRiskManageService").Should().BeEquivalentTo(new[]
        {
            "/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades",
            "/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades",
            "/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result",
            "/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result",
            "/v3/merchant-risk-manage/report-trade-union-information",
            "/v3/merchant-risk-manage/trade-risk-information",
            "/v3/merchant-risk-manage/trade-risk-result",
            "/v3/merchant-risk-manage/violation-notifications",
            "/v3/merchant-risk-manage/violation-notifications",
            "/v3/merchant-risk-manage/violation-notifications",
            "/v3/merchant-risk-manage/violation-notifications",
        });

        // P0-7 商户门店管理 5 端点：单一前缀 /v3/merchant-store/，官方十页（商户面与服务商面成对）
        // 因「字段表只差一个 sub_mchid、应答表零差异」而归并为五支 —— 与上一域的「同路由分建两形态」相反，
        // 判据同为「官方字段表是否相同」。更新支是支付线第一支 PATCH 且 204 无包体。
        // 逐条锁定见 MST-B1 / MST-B6。
        RoutesOf(asm, "IWechatPayMerchantStoreService").Should().BeEquivalentTo(new[]
        {
            "/v3/merchant-store/stores",
            "/v3/merchant-store/stores/{storeId}",
            "/v3/merchant-store/stores/{storeId}",
            "/v3/merchant-store/stores/{storeId}/recipients/bind",
            "/v3/merchant-store/stores/{storeId}/recipients/unbind",
        });

        // P0-7 商户资料变更 12 端点：三族各四支（提交 / 按申请单号查 / 按业务单号查 / 撤销），
        // 单一前缀 /v3/mchalterapply/。撤销是 POST 动词段而非 DELETE；本域零 204。
        // 「八行核得两族独立页面」的出处错配见 MAA-B2；三族的共用/分建裁决见 MAA-B6。
        RoutesOf(asm, "IWechatPayMerchantAlterApplyService").Should().BeEquivalentTo(new[]
        {
            "/v3/mchalterapply/mchsubjectalterapplyment",
            "/v3/mchalterapply/mchsubjectalterapplyment/{applyId}",
            "/v3/mchalterapply/mchsubjectalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}",
            "/v3/mchalterapply/mchsubjectalterapplyment/{applyId}/revoke",
            "/v3/mchalterapply/mchcontactalterapplyment",
            "/v3/mchalterapply/mchcontactalterapplyment/{applyId}",
            "/v3/mchalterapply/mchcontactalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}",
            "/v3/mchalterapply/mchcontactalterapplyment/{applyId}/revoke",
            "/v3/mchalterapply/merchantnamealterapplyment",
            "/v3/mchalterapply/merchantnamealterapplyment/{applyId}",
            "/v3/mchalterapply/merchantnamealterapplyment/merchant/{subMchId}/out-request-no/{outRequestNo}",
            "/v3/mchalterapply/merchantnamealterapplyment/{applyId}/revoke",
        });

        // P0-7 商户账户余额 2 端点：裁决守卫 C1 由「零 /v3/merchant/fund」收窄为
        // 「零 /v3/merchant/fund/withdraw」后落地的两支（官方标注支持商户【普通服务商】）。
        RoutesOf(asm, "IWechatPayMerchantFundService").Should().BeEquivalentTo(new[]
        {
            "/v3/merchant/fund/balance/{accountType}",
            "/v3/merchant/fund/dayendbalance/{accountType}",
        });

        // P0-7 商户结算规则申请 2 端点：从业机构面（创建 + 按 application_id 查），两支应答表相同 ⇒ 共用应答。
        RoutesOf(asm, "IWechatPayMerchantSettlementService").Should().BeEquivalentTo(new[]
        {
            "/v3/merchant-settlement/merchant-settle-rule-applications",
            "/v3/merchant-settlement/merchant-settle-rule-applications/{applicationId}",
        });

        // 合计计数（PAY-B5 断言的单一来源）。
        asm.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods())
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Count(static uri => !string.IsNullOrWhiteSpace(uri))
            .Should().Be(Baseline.Pay.Endpoints,
                "支付线端点总数须与 Tests/ContractBaseline.cs 一致（当前 = 原 57 + 特约商户进件 6 + 主体进件 5 + 服务商基础支付 9 + 服务商支付分 23 + 从业机构支付分 11 + P0-6 三域 17 + P0-7 风险 11 + 门店 5 + 变更 12 + 余额 2 + 结算规则 2）");
    }

    /// <summary>
    /// PAY-B5（字段名照官方原文）：官方以 snake_case 且拼写不由我方「纠正」。
    /// </summary>
    /// <remarks>
    /// 这是最易被「顺手驼峰化」的一类漂移：一旦改名，报文对不上官方，且症状是运行期 <c>PARAM_ERROR</c>
    /// 而非编译错误。故逐条锁定代表字段。
    /// </remarks>
    [Fact]
    public void Dtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<JsapiPrepayRequest>(nameof(JsapiPrepayRequest.OutTradeNo), "out_trade_no");
        JsonNameShouldBe<JsapiPrepayRequest>(nameof(JsapiPrepayRequest.SupportFapiao), "support_fapiao");

        // 直连下单三族：应答字段名即三族形态的区分点（code_url / prepay_id / h5_url），拼写照官方原文。
        JsonNameShouldBe<NativePrepayResponse>(nameof(NativePrepayResponse.CodeUrl), "code_url");
        JsonNameShouldBe<H5PrepayResponse>(nameof(H5PrepayResponse.H5Url), "h5_url");
        JsonNameShouldBe<H5Info>(nameof(H5Info.BundleId), "bundle_id");
        JsonNameShouldBe<H5SceneInfo>(nameof(H5SceneInfo.PayerClientIp), "payer_client_ip");
        JsonNameShouldBe<AppSubsidyDetail>(nameof(AppSubsidyDetail.SubsidyPeriodType), "subsidy_period_type");
        JsonNameShouldBe<AppSubsidyPlan>(nameof(AppSubsidyPlan.SubsidyInstallmentNum), "subsidy_installment_num");
        JsonNameShouldBe<CloseOrderRequest>(nameof(CloseOrderRequest.MchId), "mchid");

        JsonNameShouldBe<TransactionQueryResponse>(nameof(TransactionQueryResponse.TradeStateDesc), "trade_state_desc");
        JsonNameShouldBe<TransactionAmountInfo>(nameof(TransactionAmountInfo.PayerTotal), "payer_total");

        JsonNameShouldBe<RefundApplyRequest>(nameof(RefundApplyRequest.OutRefundNo), "out_refund_no");
        JsonNameShouldBe<RefundFundsFrom>(nameof(RefundFundsFrom.Account), "account");
        JsonNameShouldBe<RefundGoodsDetail>(nameof(RefundGoodsDetail.RefundQuantity), "refund_quantity");
        JsonNameShouldBe<RefundResponse>(nameof(RefundResponse.UserReceivedAccount), "user_received_account");
        JsonNameShouldBe<RefundAmount>(nameof(RefundAmount.SettlementRefund), "settlement_refund");
        JsonNameShouldBe<AbnormalRefundRequest>(nameof(AbnormalRefundRequest.RealName), "real_name");

        JsonNameShouldBe<BillDownloadInfoResponse>(nameof(BillDownloadInfoResponse.DownloadUrl), "download_url");

        JsonNameShouldBe<PlatformCertificate>(nameof(PlatformCertificate.SerialNumber), "serial_no");
        JsonNameShouldBe<PlatformCertificate>(nameof(PlatformCertificate.EffectiveTime), "effective_time");
        JsonNameShouldBe<PlatformCertificateCipher>(nameof(PlatformCertificateCipher.AssociatedData), "associated_data");
    }

    /// <summary>
    /// 判错面契约：所有支付响应须继承 <see cref="WechatPayResponse"/>（承载 <c>code</c>/<c>message</c>）。
    /// </summary>
    /// <remarks>
    /// 支付接口带 <c>[AllowAnyStatusCode]</c>（4xx 错误体不被组件拦截），错误体须有落点才能判错；
    /// 漏继承的响应在失败场景下会「字段全空但调用方以为成功」——最危险的静默形态。
    /// </remarks>
    [Fact]
    public void Responses_ShouldDeriveFromWechatPayResponse()
    {
        JsonNameShouldBe<WechatPayResponse>(nameof(WechatPayResponse.Code), "code");
        JsonNameShouldBe<WechatPayResponse>(nameof(WechatPayResponse.Message), "message");

        foreach (var responseType in new[]
                 {
                     typeof(PrepayIdResponse),
                     typeof(NativePrepayResponse),
                     typeof(H5PrepayResponse),
                     typeof(TransactionQueryResponse),
                     typeof(RefundResponse),
                     typeof(BillDownloadInfoResponse),
                     typeof(PlatformCertificatesResponse),
                     typeof(ProfitSharingReceiver),
                     typeof(ProfitSharingOrderResponse),
                     typeof(ProfitSharingReturnOrderResponse),
                     typeof(ProfitSharingAmountsResponse),
                     typeof(ProfitSharingDeleteReceiverResponse),
                     typeof(PayScoreServiceOrderResponse),
                     typeof(PayScoreServiceOrderQueryResponse),
                     typeof(PayScoreCancelOrderResponse),
                     typeof(PayScoreCompleteOrderResponse),
                     typeof(PayScoreModifyOrderResponse),
                     typeof(PayScoreCollectResponse),
                     typeof(PayScorePermissionsResponse),
                     typeof(PayScoreAuthorizationRecordResponse),
                     typeof(PayScoreSyncOrderResponse),
                     typeof(CombinePrepayResponse),
                     typeof(CombineQueryResponse),
                     typeof(TransferBillResponse),
                     typeof(TransferBillQueryResponse),
                     typeof(TransferElecsignResponse),
                     typeof(TransferRevokeResponse),
                     typeof(FapiaoQueryResponse),
                     typeof(CouponStockCreateResponse),
                     typeof(CouponQueryResponse),
                     typeof(Apply4SubApplymentResponse),
                     typeof(Apply4SubApplymentStateResponse),
                     typeof(Apply4SubModifySettlementResponse),
                     typeof(Apply4SubSettlementResponse),
                     typeof(Apply4SubSettlementModifyApplicationResponse),
                     typeof(Apply4SubjectApplymentResponse),
                     typeof(Apply4SubjectApplymentStateResponse),
                     typeof(Apply4SubjectMerchantStateResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse 以承载官方 code/message（判错面）");
        }
    }

    /// <summary>
    /// <c>[AllowAnyStatusCode]</c> 必要性锁：四域接口均须声明该特性，否则 4xx 会被组件直接抛
    /// <c>ApiException</c>、把官方业务码丢掉（方案 §2.8 的隐含前提）。
    /// </summary>
    [Fact]
    public void Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable()
    {
        var interfaces = LoadPayAssembly()
            .GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "HttpClientApiAttribute"))
            .ToArray();

        interfaces.Should().NotBeEmpty("参照集为空会让本守卫静默假绿");

        foreach (var type in interfaces)
        {
            type.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute")
                .Should().BeTrue(
                    $"{type.Name} 须声明 [AllowAnyStatusCode]，否则 APIv3 的 4xx 错误体被组件拦截、code 丢失");
        }
    }

    // ---- helpers -------------------------------------------------------------

    /// <summary>
    /// 取支付主包程序集：以该包内的<b>锚定类型</b>定位（<c>typeof(T).Assembly</c>）。
    /// 不用 <c>Assembly.Load("Mud.Wechat.Pay")</c> —— 按名加载在 AOT / 裁剪下不可用，
    /// 且把「程序集叫什么」写进断言（改名 / 合并即红，掩盖真正要守的分层语义）。
    /// </summary>
    private static Assembly LoadPayAssembly() =>
        typeof(Mud.Wechat.Pay.Extensions.PayModule).Assembly;

    private static string[] RoutesOf(Assembly asm, string interfaceName)
    {
        var type = asm.GetTypes().SingleOrDefault(t => t.Name == interfaceName);
        type.Should().NotBeNull($"未找到接口 {interfaceName}");

        return type!.GetMethods()
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .Select(static uri => uri!)
            .OrderBy(static uri => uri, StringComparer.Ordinal)
            .ToArray();
    }

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttributes(false)
            .Where(static a => a.GetType().Name == "JsonPropertyNameAttribute")
            .Select(static a => a.GetType().GetProperty("Name")!.GetValue(a) as string)
            .SingleOrDefault();

        jsonName.Should().Be(expectedJsonName,
            $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（不得驼峰化）");
    }
}
