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
        var interfaces = LoadProductLine("Mud.Wechat.Pay")
            .GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "HttpClientApiAttribute"))
            .OrderBy(static t => t.Name, StringComparer.Ordinal)
            .ToArray();

        // 数量下限防枚举空跑（AGENTS §6）：五域接口 = Transactions / Refund / Bill / Certificates / ProfitSharing。
        interfaces.Should().HaveCount(5,
            "支付线五域接口（Transactions/Refund/Bill/Certificates/ProfitSharing）——数量变化须同批更新 PayModule 与本守卫");

        interfaces.Select(static t => t.Name).Should().BeEquivalentTo(new[]
        {
            "IWechatPayTransactionsService",
            "IWechatPayRefundService",
            "IWechatPayBillService",
            "IWechatPayCertificatesService",
            "IWechatPayProfitSharingService",
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
    /// PAY-B5：端点计数（<b>17</b>）+ 官方路由表逐条比对（照官方原文，<b>不得「纠正」</b>）。
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
    /// <b>P2 分账首批</b>（同日核验）：<c>4012524936</c> 请求分账 / <c>4012528995</c> 添加分账接收方 /
    /// <c>chapter8_1_2</c> 查询分账结果。
    /// </para>
    /// </remarks>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutes()
    {
        var asm = LoadProductLine("Mud.Wechat.Pay");

        RoutesOf(asm, "IWechatPayTransactionsService").Should().BeEquivalentTo(new[]
        {
            "/v3/pay/transactions/jsapi",
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

        // 合计计数（PAY-B5 断言的单一来源）。
        asm.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods())
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Count(static uri => !string.IsNullOrWhiteSpace(uri))
            .Should().Be(19, "支付线端点总数为 19（4 交易 + 3 退款 + 2 账单 + 1 平台证书 + 9 分账）");
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
                     typeof(JsapiPrepayResponse),
                     typeof(TransactionQueryResponse),
                     typeof(RefundResponse),
                     typeof(BillDownloadInfoResponse),
                     typeof(PlatformCertificatesResponse),
                     typeof(ProfitSharingReceiver),
                     typeof(ProfitSharingOrderResponse),
                     typeof(ProfitSharingReturnOrderResponse),
                     typeof(ProfitSharingAmountsResponse),
                     typeof(ProfitSharingDeleteReceiverResponse),
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
        var interfaces = LoadProductLine("Mud.Wechat.Pay")
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

    private static Assembly LoadProductLine(string name)
    {
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == name)
            ?? Assembly.Load(name);
        asm.GetName().Name.Should().Be(name);
        return asm;
    }

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
