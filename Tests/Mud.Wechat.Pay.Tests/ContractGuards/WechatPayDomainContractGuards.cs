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

        // 聚合计数（防枚举空跑，AGENTS §6）：十域接口 = Transactions / Refund / Bill / Certificates / ProfitSharing /
        // PayScore / CombineTransactions / Transfer / NewTaxControlFapiao / MarketingFavor。
        // 数字集中维护在 Tests/ContractBaseline.cs；数量变化须同批更新 PayModule 与该基线。
        interfaces.Should().HaveCount(Baseline.Pay.HttpApiInterfaces,
            "支付线十域接口——数量变化须同批更新 PayModule 与 Tests/ContractBaseline.cs");

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
    /// PAY-B5：端点计数（<b>57</b>）+ 官方路由表逐条比对（照官方原文，<b>不得「纠正」</b>）。
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

        // 合计计数（PAY-B5 断言的单一来源）。
        asm.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods())
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Count(static uri => !string.IsNullOrWhiteSpace(uri))
            .Should().Be(Baseline.Pay.Endpoints,
                "支付线端点总数为 57（原 54 + 直连下单三族：Native/APP/H5）；数量变化须同批调整 Tests/ContractBaseline.cs");
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
