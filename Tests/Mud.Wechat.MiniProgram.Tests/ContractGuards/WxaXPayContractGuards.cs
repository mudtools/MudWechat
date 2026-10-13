// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.MiniProgram.DataModels.XPay;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「虚拟支付（XPay）」域契约守卫（XP 系列：37 端点路由与动词 + 令牌与签名参数面 +
/// 官方自相矛盾/空缺登记 + 跨线路由零交叠 + DTO 字段与 JSON 上下文登记 + 词表外凭据脱敏闭环）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页取证，<c>VirtualPayment</c> 分区全集 37 端点）</b>：
/// 代币 4（<see cref="IWxaXPayCurrencyService"/>）+ 订单 7（<see cref="IWxaXPayOrderService"/>）+
/// 道具管理 5（<see cref="IWxaXPayGoodsService"/>）+ 会员订阅 4（<see cref="IWxaXPaySubscribeService"/>）+
/// 资金与提现 10（<see cref="IWxaXPayFundsService"/>）+ 投诉与管控 7（<see cref="IWxaXPayComplaintService"/>）。
/// </para>
/// <para>
/// <b>本域与其余小程序域最大的形态差异</b>：官方在 <c>access_token</c> 之外，逐请求强制第二重凭据
/// <c>pay_sig</c>（支付签名），部分端点再叠加 <c>signature</c>（用户态签名）。两者算法属<b>宿主</b>
/// （需 <c>offer_id</c> + <c>app_key</c> 侧密钥），SDK <b>不</b>计算也不缓存签名 ⇒ 它们必须是
/// <b>方法级显式 <c>[Query]</c> 参数</b>而非 <c>[Token]</c> 注入。XP2 逐端点锁定「哪 28 支带 pay_sig、
/// 哪 9 支不带、哪 3 支双签名必填、哪 2 支 signature 选填」——写错即官方网关直接拒绝，
/// 且现象与「签名算错」难以区分，故不许靠调用方试错发现。
/// </para>
/// <para>
/// <b>敏感值口径（与 <c>permanent_code</c> 同级处置）</b>：代币余额（<c>balance</c> /
/// <c>present_balance</c> / <c>sum_*</c>）、商户可提现余额（<c>balance_available.amount</c>）、
/// 提现金额与提现单号（<c>withdraw_amount</c> / <c>wx_withdraw_no</c>）、账单与订单文件下载链接
/// （<c>url</c> / <c>bill_url</c> / <c>download_url</c>）、<c>cos_url</c>（官方明示 30 分钟有效）、
/// <c>sign</c> / <c>pay_sig</c> / <c>signature</c>、订单 <c>token</c>、投诉人
/// <c>payer_phone</c> / <c>payer_openid</c> ⇒ <b>禁止</b>写入日志、遥测或异常消息。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数
/// （MP-X5 / ContractBaseline）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；
/// Query 令牌注入白名单（QT1）由 <c>MpQueryTokenWhitelistGuard</c> 持有；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaXPayContractGuards
{
    private const string RegistryGroupName = "XPay";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.XPay";

    private const string RoutePrefix = "/xpay/";

    /// <summary>
    /// 虚拟支付域官方路由表（37 端点：代币 4 + 订单 7 + 道具 5 + 订阅 4 + 资金 10 + 投诉 7）。
    /// 路由与动词照官方原文（全 POST、全 <c>/xpay/</c> 单一族），含官方自身的拼写缺陷。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaXPayCurrencyService), nameof(IWxaXPayCurrencyService.CurrencyPayAsync), typeof(PostAttribute), "/xpay/currency_pay"),
        (typeof(IWxaXPayCurrencyService), nameof(IWxaXPayCurrencyService.CancelCurrencyPayAsync), typeof(PostAttribute), "/xpay/cancel_currency_pay"),
        (typeof(IWxaXPayCurrencyService), nameof(IWxaXPayCurrencyService.PresentCurrencyAsync), typeof(PostAttribute), "/xpay/present_currency"),
        (typeof(IWxaXPayCurrencyService), nameof(IWxaXPayCurrencyService.QueryUserBalanceAsync), typeof(PostAttribute), "/xpay/query_user_balance"),

        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.QueryOrderAsync), typeof(PostAttribute), "/xpay/query_order"),
        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.RefundOrderAsync), typeof(PostAttribute), "/xpay/refund_order"),
        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.NotifyProvideGoodsAsync), typeof(PostAttribute), "/xpay/notify_provide_goods"),
        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.StartDownloadOrderAsync), typeof(PostAttribute), "/xpay/start_download_order"),
        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.QueryDownloadOrderAsync), typeof(PostAttribute), "/xpay/query_download_order"),
        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.DownloadBillAsync), typeof(PostAttribute), "/xpay/download_bill"),
        (typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.DownloadIosSettlementBillAsync), typeof(PostAttribute), "/xpay/download_ios_settlement_bill"),

        (typeof(IWxaXPayGoodsService), nameof(IWxaXPayGoodsService.StartUploadGoodsAsync), typeof(PostAttribute), "/xpay/start_upload_goods"),
        (typeof(IWxaXPayGoodsService), nameof(IWxaXPayGoodsService.QueryUploadGoodsAsync), typeof(PostAttribute), "/xpay/query_upload_goods"),
        (typeof(IWxaXPayGoodsService), nameof(IWxaXPayGoodsService.StartPublishGoodsAsync), typeof(PostAttribute), "/xpay/start_publish_goods"),
        (typeof(IWxaXPayGoodsService), nameof(IWxaXPayGoodsService.QueryPublishGoodsAsync), typeof(PostAttribute), "/xpay/query_publish_goods"),
        (typeof(IWxaXPayGoodsService), nameof(IWxaXPayGoodsService.UploadVpFileAsync), typeof(PostAttribute), "/xpay/upload_vp_file"),

        (typeof(IWxaXPaySubscribeService), nameof(IWxaXPaySubscribeService.SendSubscribePrePaymentAsync), typeof(PostAttribute), "/xpay/send_subscribe_pre_payment"),
        (typeof(IWxaXPaySubscribeService), nameof(IWxaXPaySubscribeService.SubmitSubscribePayOrderAsync), typeof(PostAttribute), "/xpay/submit_subscribe_pay_order"),
        (typeof(IWxaXPaySubscribeService), nameof(IWxaXPaySubscribeService.QuerySubscribeContractAsync), typeof(PostAttribute), "/xpay/query_subscribe_contract"),
        (typeof(IWxaXPaySubscribeService), nameof(IWxaXPaySubscribeService.CancelSubscribeContractAsync), typeof(PostAttribute), "/xpay/cancel_subscribe_contract"),

        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.BindTransferAccountAsync), typeof(PostAttribute), "/xpay/bind_transfer_accout"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.QueryTransferAccountAsync), typeof(PostAttribute), "/xpay/query_transfer_account"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.CreateFundsBillAsync), typeof(PostAttribute), "/xpay/create_funds_bill"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.QueryFundsBillAsync), typeof(PostAttribute), "/xpay/query_funds_bill"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.QueryAdverFundsAsync), typeof(PostAttribute), "/xpay/query_adver_funds"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.QueryRecoverBillAsync), typeof(PostAttribute), "/xpay/query_recover_bill"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.DownloadAdverfundsOrderAsync), typeof(PostAttribute), "/xpay/download_adverfunds_order"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.QueryBizBalanceAsync), typeof(PostAttribute), "/xpay/query_biz_balance"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.CreateWithdrawOrderAsync), typeof(PostAttribute), "/xpay/create_withdraw_order"),
        (typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.QueryWithdrawOrderAsync), typeof(PostAttribute), "/xpay/query_withdraw_order"),

        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.GetComplaintListAsync), typeof(PostAttribute), "/xpay/get_complaint_list"),
        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.GetComplaintDetailAsync), typeof(PostAttribute), "/xpay/get_complaint_detail"),
        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.GetNegotiationHistoryAsync), typeof(PostAttribute), "/xpay/get_negotiation_history"),
        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.ResponseComplaintAsync), typeof(PostAttribute), "/xpay/response_complaint"),
        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.CompleteComplaintAsync), typeof(PostAttribute), "/xpay/complete_complaint"),
        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.GetUploadFileSignAsync), typeof(PostAttribute), "/xpay/get_upload_file_sign"),
        (typeof(IWxaXPayComplaintService), nameof(IWxaXPayComplaintService.QueryPunishmentReasonsAsync), typeof(PostAttribute), "/xpay/query_punishment_reasons"),
    };

    private static readonly Type[] DomainInterfaces =
    {
        typeof(IWxaXPayCurrencyService),
        typeof(IWxaXPayOrderService),
        typeof(IWxaXPayGoodsService),
        typeof(IWxaXPaySubscribeService),
        typeof(IWxaXPayFundsService),
        typeof(IWxaXPayComplaintService),
    };

    /// <summary>官方逐请求强制 <c>pay_sig</c> 的 28 支端点（其余 9 支官方参数表内无该字段）。</summary>
    private static readonly string[] PaySigRoutes =
    {
        "/xpay/currency_pay", "/xpay/cancel_currency_pay", "/xpay/query_user_balance",
        "/xpay/query_order", "/xpay/refund_order", "/xpay/start_download_order",
        "/xpay/query_download_order", "/xpay/download_bill", "/xpay/download_ios_settlement_bill",
        "/xpay/start_upload_goods", "/xpay/query_upload_goods", "/xpay/start_publish_goods",
        "/xpay/query_publish_goods", "/xpay/upload_vp_file",
        "/xpay/send_subscribe_pre_payment", "/xpay/submit_subscribe_pay_order",
        "/xpay/query_subscribe_contract", "/xpay/cancel_subscribe_contract",
        "/xpay/query_biz_balance", "/xpay/create_withdraw_order", "/xpay/query_withdraw_order",
        "/xpay/get_complaint_list", "/xpay/get_complaint_detail", "/xpay/get_negotiation_history",
        "/xpay/response_complaint", "/xpay/complete_complaint", "/xpay/get_upload_file_sign",
        "/xpay/query_punishment_reasons",
    };

    /// <summary>官方参数表<b>无</b> <c>pay_sig</c> 的 9 支端点（不得「顺手对齐」补上）。</summary>
    private static readonly string[] NoPaySigRoutes =
    {
        "/xpay/present_currency", "/xpay/notify_provide_goods", "/xpay/bind_transfer_accout",
        "/xpay/query_transfer_account", "/xpay/create_funds_bill", "/xpay/query_funds_bill",
        "/xpay/query_adver_funds", "/xpay/query_recover_bill", "/xpay/download_adverfunds_order",
    };

    /// <summary>双签名（<c>pay_sig</c> + <c>signature</c>）且两支<b>均为必填</b>的端点。</summary>
    private static readonly string[] RequiredDualSignatureRoutes =
    {
        "/xpay/currency_pay", "/xpay/cancel_currency_pay", "/xpay/query_user_balance",
    };

    /// <summary>
    /// 官方「注意事项」要求传 <c>signature</c>、而「请求参数表」未列该字段的端点（XP3 登记的矛盾一、二）
    /// ⇒ SDK 取<b>并集</b>：落地为<b>选填</b>参数（必填会打断官方参数表允许的正常调用，选填不会）。
    /// </summary>
    private static readonly string[] OptionalSignatureRoutes =
    {
        "/xpay/refund_order", "/xpay/get_complaint_list",
    };

    /// <summary>官方请求参数表<b>零字段</b>的端点（不设请求 DTO，无 <c>[Body]</c> 参数）。</summary>
    private static readonly string[] ZeroBodyRoutes =
    {
        "/xpay/query_punishment_reasons",
    };

    /// <summary>
    /// <c>env</c> 归类一：官方写明「仅作为签名校验 / 用于基类签名校验，查询的结果都是正式环境的」
    /// （17 支）⇒ 传 1 只影响签名基串，<b>不会</b>切到沙箱数据。
    /// </summary>
    private static readonly string[] SignatureCheckOnlyEnvRoutes =
    {
        "/xpay/bind_transfer_accout", "/xpay/complete_complaint", "/xpay/create_funds_bill",
        "/xpay/download_adverfunds_order", "/xpay/get_complaint_detail", "/xpay/get_complaint_list",
        "/xpay/get_negotiation_history", "/xpay/get_upload_file_sign", "/xpay/query_adver_funds",
        "/xpay/query_biz_balance", "/xpay/query_download_order", "/xpay/query_funds_bill",
        "/xpay/query_recover_bill", "/xpay/query_transfer_account", "/xpay/response_complaint",
        "/xpay/start_download_order", "/xpay/upload_vp_file",
    };

    /// <summary><c>env</c> 归类二：官方只写「0-正式环境 1-沙箱环境」（现网/沙箱两分，13 支）。</summary>
    private static readonly string[] PlainEnvRoutes =
    {
        "/xpay/cancel_currency_pay", "/xpay/create_withdraw_order", "/xpay/currency_pay",
        "/xpay/notify_provide_goods", "/xpay/present_currency", "/xpay/query_order",
        "/xpay/query_publish_goods", "/xpay/query_upload_goods", "/xpay/query_user_balance",
        "/xpay/query_withdraw_order", "/xpay/refund_order", "/xpay/start_publish_goods",
        "/xpay/start_upload_goods",
    };

    /// <summary><c>env</c> 归类三：官方明写「仅支持正式环境」（沙箱值直接报 <c>-15011</c>，1 支）。</summary>
    private static readonly string[] ProductionOnlyEnvRoutes =
    {
        "/xpay/submit_subscribe_pay_order",
    };

    /// <summary><c>env</c> 归类四：官方参数表<b>根本没有</b> env 字段（6 支）。</summary>
    private static readonly string[] NoEnvRoutes =
    {
        "/xpay/cancel_subscribe_contract", "/xpay/download_bill", "/xpay/download_ios_settlement_bill",
        "/xpay/query_punishment_reasons", "/xpay/query_subscribe_contract", "/xpay/send_subscribe_pre_payment",
    };

    /// <summary>错误码档一：共享域内 <c>26849_0xx</c> 码表（另有通用 <c>-1</c>）的 28 支端点。</summary>
    private static readonly string[] DomainErrorCodeRoutes =
    {
        "/xpay/currency_pay", "/xpay/cancel_currency_pay", "/xpay/present_currency", "/xpay/query_user_balance",
        "/xpay/query_order", "/xpay/refund_order", "/xpay/download_bill", "/xpay/upload_vp_file",
        "/xpay/start_upload_goods", "/xpay/query_upload_goods", "/xpay/start_publish_goods",
        "/xpay/query_publish_goods", "/xpay/bind_transfer_accout", "/xpay/query_transfer_account",
        "/xpay/create_funds_bill", "/xpay/query_funds_bill", "/xpay/query_adver_funds", "/xpay/query_recover_bill",
        "/xpay/download_adverfunds_order", "/xpay/query_biz_balance", "/xpay/create_withdraw_order",
        "/xpay/query_withdraw_order", "/xpay/get_complaint_list", "/xpay/get_complaint_detail",
        "/xpay/get_negotiation_history", "/xpay/response_complaint", "/xpay/complete_complaint",
        "/xpay/get_upload_file_sign",
    };

    /// <summary>
    /// 错误码档二：<b>各有独立码表</b>的会员订阅两支（预通知扣款 <c>674690001</c> / <c>690000000</c> /
    /// <c>690000001</c>；发起订阅扣款为米大师侧 <c>-15001 ~ -15027</c> 加 <c>1001</c>）⇒ 判错不得套域内表。
    /// </summary>
    private static readonly string[] OwnErrorCodeRoutes =
    {
        "/xpay/send_subscribe_pre_payment", "/xpay/submit_subscribe_pay_order",
    };

    /// <summary>错误码档三：官方明写「此接口没有特殊错误码」的 7 支（只有通用码，不得臆造域内码）。</summary>
    private static readonly string[] NoSpecialErrorCodeRoutes =
    {
        "/xpay/notify_provide_goods", "/xpay/start_download_order", "/xpay/download_ios_settlement_bill",
        "/xpay/query_download_order", "/xpay/query_subscribe_contract", "/xpay/query_punishment_reasons",
        "/xpay/cancel_subscribe_contract",
    };

    /// <summary>
    /// 契约守卫 XP1：37 端点路由与动词必须与官方契约逐字一致（本域只有一个官方路由族、一律 POST）。
    /// </summary>
    [Fact]
    public void XPayEndpoints_ShouldMatchOfficialRoutes()
    {
        // 37 端点 = 代币 4 + 订单 7 + 道具 5 + 订阅 4 + 资金 10 + 投诉 7（官方 VirtualPayment 分区逐页取证全集）。
        Routes.Should().HaveCount(37,
            "虚拟支付域官方开放面为 37 端点（代币 4 + 订单 7 + 道具 5 + 会员订阅 4 + 资金与提现 10 + 投诉与管控 7）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(37, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致（照官方原文拼写，禁止改写）");
            route.Should().StartWith(RoutePrefix, "本域官方路由全部落在 /xpay/ 单一族内（无 /wxa/ 或 /cgi-bin/ 前缀混入）");
        }

        // 动词分布：37 支一律 POST（官方 37 页的调用方式行无一例外）。
        Routes.Should().OnlyContain(static r => r.HttpAttribute == typeof(PostAttribute),
            "本域官方动词全为 POST（与直播/短剧两域同形态；不得为「语义像查询」的端点擅自改 GET）");

        // 路由形状：全小写 + 下划线，无驼峰、无尾斜杠。
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "本域官方路由一律小写下划线（不得顺手改成驼峰）");

        // 反直觉点锁定 1：官方路由自身的拼写缺陷 —— 绑定提现账户是 bind_transfer_accout（少一个 n）。
        // 网关按该拼写路由，改成 bind_transfer_account 即 404；方法名 BindTransferAccountAsync 保持正确拼写。
        Routes.Select(static r => r.Route).Should().Contain("/xpay/bind_transfer_accout",
            "官方路由原文即缺字母 n（bind_transfer_accout）⇒ 照原文落地，不得「顺手修正」");
        Routes.Should().NotContain(static r => r.Route == "/xpay/bind_transfer_account",
            "正确拼写的那支官方不存在");

        // 反直觉点锁定 2：官方域内动词命名不成对——下载订单任务是 start_download_order（发起）
        // 与 query_download_order（查询），而账单下载是 download_bill（一步式）⇒ 三套形态并存，不得归一。
        Routes.Select(static r => r.Route).Should()
            .Contain("/xpay/start_download_order")
            .And.Contain("/xpay/query_download_order")
            .And.Contain("/xpay/download_bill",
                "「异步任务型下载」两支 + 「一步式下载链接」一支官方形态不同，不得合并成一套命名");

        // 反直觉点锁定 3：官方无 query_bill / get_bill，账单面只有 create/query_funds_bill 与 download_bill。
        Routes.Should().NotContain(static r => r.Route == "/xpay/query_bill" || r.Route == "/xpay/get_bill");
    }

    /// <summary>
    /// 契约守卫 XP2：六接口令牌绑定与注册组 + 逐接口规模 + <b>签名参数面</b>（28 带 pay_sig /
    /// 9 不带 / 3 双签名必填 / 2 signature 选填）+ 零请求体端点。
    /// </summary>
    [Fact]
    public void XPayInterfaces_ShouldBindMpAccessToken_ShareRegistryGroup_AndLockSignatureParameterFace()
    {
        DomainInterfaces.Should().HaveCount(6, "虚拟支付域按官方子目录族分为六接口（合计 37 端点）");

        foreach (var iface in DomainInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方 37 页令牌面一致，第三方代调用为 authorizer_access_token）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（新增令牌类型会让单槽 Resolve 返回 null ⇒ errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名须照官方原文");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 XPay 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");

            iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeEmpty($"{iface.Name} 不得是空接口");
        }

        DomainInterfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域六接口共用一个注册组（AddXPayWebApiHttpClient）");

        // 端点归属自证：六接口方法数之和 == 路由表规模（防「加了接口忘了进路由表」）。
        DomainInterfaces
            .Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "六接口声明方法总数须与 XP1 路由表一致");

        MethodCount(typeof(IWxaXPayCurrencyService)).Should().Be(4, "代币族 4（扣减 / 撤销 / 赠送 / 查余额）");
        MethodCount(typeof(IWxaXPayOrderService)).Should().Be(7, "订单族 7（查询 / 退款 / 通知发货 / 发起下载 / 查下载任务 / 下载账单 / 下载 iOS 月结账单）");
        MethodCount(typeof(IWxaXPayGoodsService)).Should().Be(5, "道具族 5（发起上传 / 查上传 / 发起发布 / 查发布 / 传竖屏文件）");
        MethodCount(typeof(IWxaXPaySubscribeService)).Should().Be(4, "会员订阅族 4（预通知扣款 / 发起扣款 / 查签约关系 / 商家解约）");
        MethodCount(typeof(IWxaXPayFundsService)).Should().Be(10, "资金与提现族 10（绑定/查询提现账户 + 转账账单与广告金与追缴账单 4 + 下载广告金订单 + 余额/提现/查提现单）");
        MethodCount(typeof(IWxaXPayComplaintService)).Should().Be(7, "投诉与管控族 7（列表/详情/协商历史 + 回复/完结 + 上传签名 + 被管控原因）");

        // —— 签名参数面逐端点锁定（本域最易写错、且错了只会表现为「签名校验失败」的一处）——
        foreach (var (iface, method, _, route) in Routes)
        {
            var parameters = iface.GetMethod(method)!.GetParameters();
            var paySig = parameters.SingleOrDefault(static p => p.GetCustomAttribute<QueryAttribute>()?.Name == "pay_sig");
            var signature = parameters.SingleOrDefault(static p => p.GetCustomAttribute<QueryAttribute>()?.Name == "signature");

            if (PaySigRoutes.Contains(route))
            {
                paySig.Should().NotBeNull($"{route} 官方逐请求强制 pay_sig ⇒ 必须是方法级 [Query] 参数（不得改 [Token] 注入、不得进请求体）");
                paySig!.ParameterType.Should().Be(typeof(string), $"{route} pay_sig 官方必填 ⇒ 非空 string 且无默认值");
                paySig!.IsOptional.Should().BeFalse($"{route} pay_sig 必填 ⇒ 不得写成带默认值的可选参数（漏传即官方签名校验失败）");
            }
            else
            {
                NoPaySigRoutes.Should().Contain(route, $"{route} 的 pay_sig 面未归类 ⇒ 两份清单未覆盖 XP1 路由表全集");
                paySig.Should().BeNull($"{route} 官方参数表无 pay_sig ⇒ 不得凭空补上（补了即多传参数）");
                signature.Should().BeNull($"{route} 官方无用户态签名要求");
            }

            if (RequiredDualSignatureRoutes.Contains(route))
            {
                signature.Should().NotBeNull($"{route} 官方 pay_sig 与 signature 两支均必填");
                signature!.IsOptional.Should().BeFalse($"{route} signature 官方必填 ⇒ 不得给默认值");
            }
            else if (OptionalSignatureRoutes.Contains(route))
            {
                signature.Should().NotBeNull($"{route} 官方「注意事项」要求 signature（XP3 矛盾的取并集处置）");
                signature!.IsOptional.Should().BeTrue(
                    $"{route} 官方参数表未列 signature ⇒ 落地为选填（默认值 null），不得打断参数表口径下的合法调用");
            }
            else
            {
                signature.Should().BeNull($"{route} 官方两面均未要求 signature");
            }

            // 令牌与签名之外不得再有其他 Query 参数（本域业务参数一律走 JSON 请求体）。
            var expectedQueryNames = new List<string>();
            if (PaySigRoutes.Contains(route))
            {
                expectedQueryNames.Add("pay_sig");
            }

            if (signature != null)
            {
                expectedQueryNames.Add("signature");
            }

            parameters.Where(static p => p.GetCustomAttribute<QueryAttribute>() != null)
                .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name)
                .Should().BeEquivalentTo(expectedQueryNames,
                    $"{route} 除 pay_sig / signature 外官方无其他 Query 参数");

            var bodyCount = parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null);
            if (ZeroBodyRoutes.Contains(route))
            {
                bodyCount.Should().Be(0, $"{route} 官方请求参数表零字段 ⇒ 不得凭空造 request DTO");
            }
            else
            {
                bodyCount.Should().Be(1, $"{route} 官方请求体为 JSON 对象 ⇒ 恰有一个 [Body] 参数");
            }
        }

        PaySigRoutes.Should().HaveCount(28, "官方 37 页里 28 页强制 pay_sig");
        NoPaySigRoutes.Should().HaveCount(9, "官方 37 页里 9 页无 pay_sig");
        PaySigRoutes.Intersect(NoPaySigRoutes, StringComparer.Ordinal).Should().BeEmpty("两份清单不得交叉");
        PaySigRoutes.Concat(NoPaySigRoutes).OrderBy(static r => r, StringComparer.Ordinal)
            .Should().BeEquivalentTo(Routes.Select(static r => r.Route).OrderBy(static r => r, StringComparer.Ordinal),
            "pay_sig 两份清单的并集须恰为 XP1 路由表全集（否则新增端点漏归类）");
        RequiredDualSignatureRoutes.Should().HaveCount(3, "双签名必填端点为 3（代币扣减 / 撤销 / 查余额）");
        OptionalSignatureRoutes.Should().HaveCount(2, "signature 选填端点为 2（退款 / 投诉列表，均系 XP3 登记的官方自相矛盾）");
    }

    /// <summary>
    /// 契约守卫 XP3：官方自相矛盾与空缺的<b>登记面</b> —— <c>env</c> 四档归类、错误码三档归类、
    /// 全域无端点级频率数字。三档归类必须<b>无重叠且全覆盖</b>，否则等于「归类靠记忆」。
    /// </summary>
    [Fact]
    public void XPayOfficialContradictionsAndGaps_ShouldStayRegistered()
    {
        var allRoutes = Routes.Select(static r => r.Route).ToArray();

        // —— env 四档：17 + 13 + 1 + 6 = 37，两两不交、并集为全集 ——
        SignatureCheckOnlyEnvRoutes.Should().HaveCount(17, "官方 17 页把 env 写作「仅作为签名校验 / 用于基类签名校验」");
        PlainEnvRoutes.Should().HaveCount(13, "官方 13 页只写「0-正式环境 1-沙箱环境」");
        ProductionOnlyEnvRoutes.Should().HaveCount(1, "官方仅 submit_subscribe_pay_order 一页写「仅支持正式环境」（沙箱值报 -15011）");
        NoEnvRoutes.Should().HaveCount(6, "官方 6 页参数表根本没有 env 字段");

        var envGroups = new[] { SignatureCheckOnlyEnvRoutes, PlainEnvRoutes, ProductionOnlyEnvRoutes, NoEnvRoutes };
        foreach (var group in envGroups)
        {
            group.Should().OnlyContain(r => allRoutes.Contains(r), "env 归类里出现了不在 XP1 路由表内的路由（归类表与路由表漂移）");
        }

        for (var i = 0; i < envGroups.Length; i++)
        {
            for (var j = i + 1; j < envGroups.Length; j++)
            {
                envGroups[i].Intersect(envGroups[j], StringComparer.Ordinal).Should().BeEmpty(
                    $"env 归类第 {i + 1} / {j + 1} 档交叉 ⇒ 同一端点被两种官方语义同时认领，须先复核官方页面");
            }
        }

        envGroups.SelectMany(static g => g).OrderBy(static r => r, StringComparer.Ordinal)
            .Should().BeEquivalentTo(allRoutes.OrderBy(static r => r, StringComparer.Ordinal),
            "env 四档并集须恰为 37 端点全集（新增端点须同批归类）");

        // 机械耦合：归类为「有 env」的 31 支，其请求 DTO 必带 Env（long?）；归为无 env 的 6 支必不带。
        foreach (var (iface, method, _, route) in Routes)
        {
            var requestType = RequestTypeOf(iface, method);
            var hasEnvProperty = requestType?.GetProperty(nameof(WxaXPayCurrencyPayRequest.Env),
                BindingFlags.Public | BindingFlags.Instance) != null;

            if (NoEnvRoutes.Contains(route))
            {
                hasEnvProperty.Should().BeFalse($"{route} 官方参数表无 env ⇒ 请求 DTO 不得出现 Env（补了即向官方多传字段）");
                continue;
            }

            hasEnvProperty.Should().BeTrue($"{route} 官方参数表含 env ⇒ 请求 DTO 必须有 Env 承载（XP3 归类表与 DTO 双向自证）");
            requestType!.GetProperty("Env")!.PropertyType.Should().Be(typeof(long?),
                $"{route} 官方 env 类型列为 number ⇒ 线格式落 long?（MP 域口径）");
        }

        // —— 错误码三档：28 + 2 + 7 = 37，两两不交、并集为全集 ——
        DomainErrorCodeRoutes.Should().HaveCount(28, "官方 28 页共享同一张 26849_0xx 域内错误码表");
        OwnErrorCodeRoutes.Should().HaveCount(2, "会员订阅两支各有独立码表（690000xxx / 米大师 -150xx）");
        NoSpecialErrorCodeRoutes.Should().HaveCount(7, "官方 7 页明写「此接口没有特殊错误码」");

        var errGroups = new[] { DomainErrorCodeRoutes, OwnErrorCodeRoutes, NoSpecialErrorCodeRoutes };
        for (var i = 0; i < errGroups.Length; i++)
        {
            for (var j = i + 1; j < errGroups.Length; j++)
            {
                errGroups[i].Intersect(errGroups[j], StringComparer.Ordinal).Should().BeEmpty(
                    $"错误码档第 {i + 1} / {j + 1} 档交叉 ⇒ 同一端点被两份官方码表同时认领（域内码 / 独立码 / 无特殊码三者互斥）");
            }
        }

        errGroups.SelectMany(static g => g).OrderBy(static r => r, StringComparer.Ordinal)
            .Should().BeEquivalentTo(allRoutes.OrderBy(static r => r, StringComparer.Ordinal),
            "错误码三档并集须恰为 37 端点全集（新增端点须同批归档）");

        // 空缺留档：官方本域 37 页均无端点级频率数字（不像直播/短剧域有明确上限行）。
        // ⇒ 不得臆造频率上限写进接口注释，也不得在 SDK 侧建限流器（缺口清单侧同批留档）。
        Routes.Should().NotBeEmpty("防「扫描口径失效导致真空断言」");
    }

    /// <summary>
    /// 契约守卫 XP4：<b>跨线路由零交叠</b> —— <c>/xpay/</c> 面只允许出现在小程序线源码内。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何必须扫源码而不是扫反射</b>：微信支付 APIv3 线（<c>api.mch.weixin.qq.com</c> + 商户证书）与
    /// 本域在业务上高度相邻（同一笔支付的两套视角），最自然的错误就是「在支付线也声明一遍
    /// <c>/xpay/*</c>」——一旦声明，同一路径在两线各持一份 DTO、各绑一种凭据，且<b>两线都能编译通过</b>。
    /// 反射口径的 MP-X1 只比「小程序 vs 公众号」两包，捕获不到支付线自建的同名路由，故此处以源码字面量全仓扫描。
    /// </para>
    /// </remarks>
    [Fact]
    public void XPayRoutes_ShouldBeDeclaredOnlyInMiniProgramLine()
    {
        var declared = RouteLiteralsByFile("Src");
        declared.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：扫描根必须能扫出源码内的路由字面量");

        // 正对照（活体证明）：同一扫描必须能扫出小程序线本域的 37 条路由，否则零交叠断言静默恒真。
        var inMiniProgram = declared
            .Where(kv => kv.Key.Contains("/MiniProgram/", StringComparison.Ordinal))
            .SelectMany(static kv => kv.Value)
            .Where(static r => r.StartsWith(RoutePrefix, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        inMiniProgram.Should().HaveCount(37,
            "扫描口径必须能扫出小程序线声明的 37 条 /xpay/ 路由（扫不出即扫描根或正则失效，下面的零交叠断言会恒真）");
        inMiniProgram.Should().BeEquivalentTo(Routes.Select(static r => r.Route).Distinct(),
            "扫描到的 /xpay/ 路由集须与 XP1 路由表一致");

        // 零交叠：Src 下除 MiniProgram 外任何线（公众号 / 支付 / 开放平台 / 小店 / 广告 / 企微 / Core）不得声明 /xpay/。
        var offenders = declared
            .Where(kv => !kv.Key.Contains("/MiniProgram/", StringComparison.Ordinal))
            .Where(static kv => kv.Value.Any(static r => r.StartsWith(RoutePrefix, StringComparison.Ordinal)))
            .Select(static kv => kv.Key + " -> " + string.Join(", ", kv.Value.Where(static r => r.StartsWith(RoutePrefix, StringComparison.Ordinal))))
            .ToList();

        offenders.Should().BeEmpty(
            "/xpay/ 是小程序虚拟支付域令牌面（access_token + pay_sig）专属；微信支付 APIv3 线走商户证书与 "
            + "api.mch.weixin.qq.com，跨线声明同一路径即制造双份维护与凭据串号");

        // 正对照之二：扫描根覆盖到小程序线<b>以外</b>的源码树（否则「除 MiniProgram 外为空」恒真）。
        declared
            .Where(static kv => kv.Key.Contains("/OfficialAccount/", StringComparison.Ordinal))
            .SelectMany(static kv => kv.Value)
            .Should().Contain("/cgi-bin/token",
                "扫描根必须覆盖公众号线源码（只扫到小程序线时，跨线零交叠断言会静默恒真）");
    }

    /// <summary>
    /// 契约守卫 XP5：DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（常量类恰 5）；
    /// 字段名/类型口径照官方原文 + 共用与不共用裁决 + 官方枚举取值锁定（含官方 typo 值）。
    /// </summary>
    [Fact]
    public void XPayDataModels_ShouldLockOfficialFieldsTypesAndJsonRegistration()
    {
        var assembly = typeof(WxaXPayCurrencyPayRequest).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("虚拟支付域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 83 DTO = 代币 8 + 订单 15 + 道具 12 + 订阅 5 + 资金 26 + 投诉 17
        //（按接口端点的请求体 / 应答 / 嵌套对象逐页官方表建模；子目录计数与文件计数由生成脚本自证）。
        dtoTypes.Should().HaveCount(83,
            "虚拟支付域 37 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫"
            + "（currency 8 + order 15 + goods 12 + subscribe 5 + funds 26 + complaint 17 = 83）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 XPay");
        }

        var contextType = assembly.GetType(DtoNamespace + ".XPayJsonContext");
        contextType.Should().NotBeNull(
            "XPayJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"XPay\")]，"
                + "必须登记进 XPayJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // —— 「登记进上下文」与「合并进组件管线」是两件事：后者只体现在主包
        //     `MiniProgramJsonResolverExtensions.ConfigureDataModelsResolver` 的合并数组里。
        //     新域建了上下文却没进数组 ⇒ 门禁与单测全绿（JIT 走反射兜住），Native AOT 下才在首次调用炸。
        var mergeSource = File.ReadAllText(Path.Combine(GetSolutionRoot(),
            "Src/MiniProgram/Mud.Wechat.MiniProgram/Extensions/MiniProgramJsonResolverExtensions.cs"));

        var mergedContextNames = System.Text.RegularExpressions.Regex
            .Matches(mergeSource, @"([A-Za-z]+JsonContext)\.Default")
            .Select(static m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        mergedContextNames.Should().Contain("XPayJsonContext",
            "XPayJsonContext 未合并进组件序列化管线 ⇒ 本域 83 DTO 在 Native AOT 下无生成器路径（正对照：数组里 Vod/Live 两支在册）");
        mergedContextNames.Should().Contain("VodJsonContext").And.Contain("LiveJsonContext",
            "正对照：合并清单确实被读到（读空文件或正则跑偏时上一条会假绿）");

        var allContextNames = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Name.EndsWith("JsonContext", StringComparison.Ordinal))
            .Select(static t => t.Name)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToList();

        mergedContextNames.OrderBy(static n => n, StringComparer.Ordinal).Should().Equal(allContextNames,
            "DataModels 程序集内的生成上下文与主包合并数组必须一一对应（本线上下文计数即域计数，历史上曾因未覆盖而漏接一支）");

        var constClasses = new[]
        {
            nameof(WxaXPayOrderStatuses), nameof(WxaXPayOrderTypes),
            nameof(WxaXPayPublishTaskStatuses),
            nameof(WxaXPayOperateTypes), nameof(WxaXPayComplaintMediaTypes),
        };

        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType && !constClasses.Contains(t.Name))
            .ToList();

        unregistered.Should().BeEmpty(
            "虚拟支付域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文、亦非已登记常量类的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        constClasses.Should().HaveCount(5, "本域官方枚举常量类为 5 支");
        constClasses.Select(n => assembly.GetType(DtoNamespace + "." + n))
            .Should().OnlyContain(t => t != null,
            "五支官方枚举常量类必须存在（订单状态与类型 + 发布任务状态 + 协商操作类型 + 投诉媒体类型）");

        foreach (var name in constClasses)
        {
            var constType = assembly.GetType(DtoNamespace + "." + name)!;
            constType.IsAbstract.Should().BeTrue($"{name} 须为 static class（全仓常量类统一形态）");
            constType.GetFields().Where(static f => f.IsLiteral).Should().NotBeEmpty($"{name} 必须含 const 字段");
        }

        // —— 应答面：27 支全继承 WxaResponse，且与端点返回类型一一对应；9 支官方空回体直接用 WxaResponse。
        var responseDtos = domainTypes
            .Where(static t => t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();

        responseDtos.Should().HaveCount(27,
            "本域官方应答对象为 27（代币 4 + 订单 6 + 道具 3 + 订阅 1 + 资金 8 + 投诉 5）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "虚拟支付域响应 DTO 必须继承 WxaResponse（errcode 判错与全线同源）");

        var endpointReturnTypes = Routes
            .Select(static r => ResultTypeOf(r.Interface, r.Method))
            .Where(static t => t.Namespace == DtoNamespace)
            .Distinct()
            .ToList();

        endpointReturnTypes.Should().BeEquivalentTo(responseDtos,
            "本域应答类型必须与端点返回类型一一对应（既不得留下无人返回的孤儿应答，也不得返回命名空间外的自建应答）");

        Routes.Count(static r => ResultTypeOf(r.Interface, r.Method) == typeof(WxaResponse))
            .Should().Be(9,
            "九支官方返回体只有 errcode/errmsg 的端点（notify_provide_goods / start_upload_goods / "
            + "start_publish_goods / send_subscribe_pre_payment / submit_subscribe_pay_order / "
            + "cancel_subscribe_contract / bind_transfer_accout / response_complaint / complete_complaint）"
            + "直接用 WxaResponse，不建空壳应答");

        // —— 共用与不共用裁决（同类字段名近似但官方两张表不同源，合并即静默丢字段）——
        // 共用：download_bill 与 download_adverfunds_order 的官方应答体同为 {url} ⇒ 一支 WxaXPayDownloadUrlResponse。
        ResultTypeOf(typeof(IWxaXPayOrderService), nameof(IWxaXPayOrderService.DownloadBillAsync))
            .Should().Be(ResultTypeOf(typeof(IWxaXPayFundsService), nameof(IWxaXPayFundsService.DownloadAdverfundsOrderAsync)),
            "两支下载链接端点官方应答体同形（只有 url）⇒ 共用 WxaXPayDownloadUrlResponse，不得各建一份");
        // 共用：投诉列表与详情官方 complaint 结构同一张表 ⇒ 嵌套对象 WxaXPayComplaint 一支两用。
        AssertPropertyType<WxaXPayGetComplaintListResponse>(nameof(WxaXPayGetComplaintListResponse.Complaints),
            typeof(List<WxaXPayComplaint>), "列表元素与详情主体官方同形 ⇒ 共用 WxaXPayComplaint");
        AssertPropertyType<WxaXPayGetComplaintDetailResponse>(nameof(WxaXPayGetComplaintDetailResponse.Complaint),
            typeof(WxaXPayComplaint), "详情直接复用列表元素类型（官方两张字段表逐行相同）");
        // 不共用：三支 filter 各按官方字段表建类型（时间列语义与主键列均不同源）。
        AssertPropertyType<WxaXPayQueryFundsBillRequest>(nameof(WxaXPayQueryFundsBillRequest.Filter),
            typeof(WxaXPayFundsBillFilter), "转账账单筛选（oper_time 区间 + bill_id + request_id）");
        AssertPropertyType<WxaXPayQueryRecoverBillRequest>(nameof(WxaXPayQueryRecoverBillRequest.Filter),
            typeof(WxaXPayRecoverBillFilter), "追缴账单筛选（recover_time 区间 + bill_id 必填）⇒ 与转账账单不同源");
        AssertPropertyType<WxaXPayQueryAdverFundsRequest>(nameof(WxaXPayQueryAdverFundsRequest.Filter),
            typeof(WxaXPayAdverFundsFilter), "广告金筛选（settle 区间 + fund_type）⇒ 第三支独立形态");
        typeof(WxaXPayFundsBillFilter).Should().NotBe(typeof(WxaXPayRecoverBillFilter))
            .And .NotBe(typeof(WxaXPayAdverFundsFilter), "三支 filter 官方字段表互不相同 ⇒ 不得合并为通用分页筛选");
        // 不共用：道具上传结果与发布结果字段集不同（upload_status vs publish_status，且发布侧无 name/price）。
        AssertJsonProperty<WxaXPayUploadGoodsResult>(nameof(WxaXPayUploadGoodsResult.UploadStatus), "upload_status", "官方原文（上传任务态）");
        AssertJsonProperty<WxaXPayPublishGoodsResult>(nameof(WxaXPayPublishGoodsResult.PublishStatus), "publish_status", "官方原文（发布任务态）——与 upload_status 语义不同源，两支不共用");

        // —— 字段名与类型口径抽样（照官方原文，禁止驼峰化；官方类型列 string 的金额/时间戳不得数值化）——
        AssertJsonProperty<WxaXPayCurrencyPayRequest>(nameof(WxaXPayCurrencyPayRequest.OpenId), "openid", "官方原文全小写（不得写 open_id）");
        AssertPropertyType<WxaXPayCurrencyPayRequest>(nameof(WxaXPayCurrencyPayRequest.Env), typeof(long?), "官方类型列 number");
        AssertPropertyType<WxaXPayCurrencyPayRequest>(nameof(WxaXPayCurrencyPayRequest.Amount), typeof(long?), "官方类型列 number（代币数量）");
        AssertPropertyType<WxaXPayCurrencyPayRequest>(nameof(WxaXPayCurrencyPayRequest.Payitem), typeof(string),
            "官方类型列为 string：示例里的 [{productid,unit_price,quantity}] 只是该字符串的内容形态，不得建为对象数组");
        AssertJsonProperty<WxaXPayQueryUserBalanceResponse>(nameof(WxaXPayQueryUserBalanceResponse.PresentBalance), "present_balance", "官方原文（赠送代币余额，敏感值）");
        AssertJsonProperty<WxaXPayQueryUserBalanceResponse>(nameof(WxaXPayQueryUserBalanceResponse.SumCost), "sum_cost", "官方原文（累计消耗，敏感值）");
        AssertJsonProperty<WxaXPayQueryUserBalanceResponse>(nameof(WxaXPayQueryUserBalanceResponse.FirstSaveFlag), "first_save_flag", "官方原文（首次充值标记，boolean）");
        AssertPropertyType<WxaXPayQueryUserBalanceResponse>(nameof(WxaXPayQueryUserBalanceResponse.FirstSaveFlag), typeof(bool?), "官方类型列 boolean ⇒ bool?");

        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.WxOrderId), "wx_order_id", "官方原文（微信侧订单号）");
        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.EnvType), "env_type", "官方原文（本端点的环境字段名是 env_type 而非 env）");
        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.SettTime), "sett_time", "官方原文缩写形态（settle 被官方写作 sett）");
        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.PlatformFeeFen), "platform_fee_fen", "官方原文（带单位后缀 fen）");
        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.CpsFeeFen), "cps_fee_fen", "官方原文（带单位后缀 fen）");
        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.LeftFee), "left_fee", "官方原文（剩余可退金额，敏感值）");
        AssertJsonProperty<WxaXPayOrderInfo>(nameof(WxaXPayOrderInfo.Token), "token", "官方原文（订单 token ⇒ 敏感值，禁止进日志与遥测）");

        AssertJsonProperty<WxaXPayRefundOrderRequest>(nameof(WxaXPayRefundOrderRequest.ReqFrom), "req_from", "官方原文缩写形态（request 被官方写作 req）");
        AssertJsonProperty<WxaXPayRefundOrderResponse>(nameof(WxaXPayRefundOrderResponse.PayWxOrderId), "pay_wx_order_id", "官方原文（原支付单的微信侧单号）");
        AssertJsonProperty<WxaXPayStartDownloadOrderRequest>(nameof(WxaXPayStartDownloadOrderRequest.BeginDs), "begin_ds", "官方原文（ds = 日期分区，非 begin_date）");
        AssertPropertyType<WxaXPayStartDownloadOrderRequest>(nameof(WxaXPayStartDownloadOrderRequest.IsProvided), typeof(bool?), "官方类型列 boolean");
        AssertJsonProperty<WxaXPayQueryDownloadOrderResponse>(nameof(WxaXPayQueryDownloadOrderResponse.DownloadUrl), "download_url", "官方原文（账单文件下载链接，敏感值）");
        AssertJsonProperty<WxaXPayQueryDownloadOrderResponse>(nameof(WxaXPayQueryDownloadOrderResponse.ExpireAt), "expire_at", "官方原文（链接过期时刻）");
        AssertJsonProperty<WxaXPayDownloadUrlResponse>(nameof(WxaXPayDownloadUrlResponse.Url), "url", "官方原文裸名（两支下载端点共用，敏感值）");
        AssertJsonProperty<WxaXPayIosBillInfo>(nameof(WxaXPayIosBillInfo.BillUrl), "bill_url", "官方原文（月结账单链接，敏感值）");

        AssertJsonProperty<WxaXPayCreateFundsBillRequest>(nameof(WxaXPayCreateFundsBillRequest.RequestId), "request_id", "官方原文（幂等键：268490004 表示此前已成功，不得换 id 重放）");
        AssertJsonProperty<WxaXPayCreateFundsBillRequest>(nameof(WxaXPayCreateFundsBillRequest.AuthorizeAdvertise), "authorize_advertise", "官方原文（是否授权投放，number 0/1）");
        AssertJsonProperty<WxaXPayFundsBill>(nameof(WxaXPayFundsBill.OperTime), "oper_time", "官方原文缩写形态（operation 被官方写作 oper）");
        AssertJsonProperty<WxaXPayRecoverBill>(nameof(WxaXPayRecoverBill.RefundOrderList), "refund_order_list", "官方原文（追缴关联退款单列表，array of string）");
        AssertPropertyType<WxaXPayRecoverBill>(nameof(WxaXPayRecoverBill.RefundOrderList), typeof(List<string>), "官方 array of string ⇒ List<string>?");
        AssertJsonProperty<WxaXPayBizBalance>(nameof(WxaXPayBizBalance.Amount), "amount", "官方原文（可提现余额，敏感值）");
        AssertPropertyType<WxaXPayBizBalance>(nameof(WxaXPayBizBalance.Amount), typeof(string),
            "官方类型列 string（单位与小数位由官方定）⇒ 不得数值化为 long?");
        AssertPropertyType<WxaXPayCreateWithdrawOrderRequest>(nameof(WxaXPayCreateWithdrawOrderRequest.WithdrawAmount), typeof(string),
            "官方类型列 string ⇒ 不得数值化（与 query_user_balance 的 number 口径不同源，两套表各照各的）");
        AssertPropertyType<WxaXPayQueryWithdrawOrderResponse>(nameof(WxaXPayQueryWithdrawOrderResponse.WithdrawSuccessTimestamp), typeof(string),
            "官方类型列 string（成功时刻可为空串）⇒ 不得建为 long?");
        AssertJsonProperty<WxaXPayTransferAccount>(nameof(WxaXPayTransferAccount.TransferAccountAgencyId), "transfer_account_agency_id", "官方原文（机构 id）");
        AssertJsonProperty<WxaXPayTransferAccount>(nameof(WxaXPayTransferAccount.BindResult), "bind_result", "官方原文（绑定结果，与 state 两列并存不得合并）");

        AssertJsonProperty<WxaXPayUploadGoodsItem>(nameof(WxaXPayUploadGoodsItem.ItemUrl), "item_url", "官方原文（道具页链接）");
        AssertPropertyType<WxaXPayUploadGoodsItem>(nameof(WxaXPayUploadGoodsItem.Price), typeof(long?), "官方类型列 number（单位：分）");
        AssertJsonProperty<WxaXPayUploadVpFileRequest>(nameof(WxaXPayUploadVpFileRequest.Base64Img), "base64_img", "官方原文（与 img_url 二选一，两支均标必填 ⇒ XP3 矛盾的取并集处置见接口注释）");
        AssertJsonProperty<WxaXPayUploadVpFileResponse>(nameof(WxaXPayUploadVpFileResponse.FileId), "file_id", "官方原文（上传返回的虚拟支付文件 id）");

        AssertJsonProperty<WxaXPaySubmitSubscribePayOrderRequest>(nameof(WxaXPaySubmitSubscribePayOrderRequest.OfferId), "offer_id", "官方原文（应用 id）");
        AssertJsonProperty<WxaXPaySubmitSubscribePayOrderRequest>(nameof(WxaXPaySubmitSubscribePayOrderRequest.BuyQuantity), "buy_quantity", "官方原文（购买数量）");
        AssertJsonProperty<WxaXPaySendSubscribePrePaymentRequest>(nameof(WxaXPaySendSubscribePrePaymentRequest.DeductPrice), "deduct_price", "官方原文（下次扣款金额，须 ∈ [100, 道具价格]）");
        AssertJsonProperty<WxaXPayQuerySubscribeContractResponse>(nameof(WxaXPayQuerySubscribeContractResponse.AuthorizationState), "authorization_state", "官方原文（签约态字符串，TERMINATED 为终态）");

        AssertJsonProperty<WxaXPayComplaint>(nameof(WxaXPayComplaint.PayerPhone), "payer_phone", "官方原文（投诉人手机号 ⇒ 敏感值，禁止进日志与遥测）");
        AssertJsonProperty<WxaXPayComplaint>(nameof(WxaXPayComplaint.PayerOpenId), "payer_openid", "官方原文（投诉人 openid ⇒ 敏感值，全小写非 payer_open_id）");
        AssertJsonProperty<WxaXPayComplaint>(nameof(WxaXPayComplaint.UserComplaintTimes), "user_complaint_times", "官方原文（用户投诉次数）");
        AssertJsonProperty<WxaXPayComplaint>(nameof(WxaXPayComplaint.ComplaintFullRefunded), "complaint_full_refunded", "官方原文（是否全额退款，boolean）");
        AssertPropertyType<WxaXPayComplaint>(nameof(WxaXPayComplaint.UserTagList), typeof(List<string>), "官方 array of string（含 HIGH_RISK 等取值，官方无枚举表 ⇒ 不建常量类）");
        AssertJsonProperty<WxaXPayGetUploadFileSignResponse>(nameof(WxaXPayGetUploadFileSignResponse.CosUrl), "cos_url", "官方原文（上传目的地，官方明示 30 分钟有效 ⇒ 敏感值）");
        AssertJsonProperty<WxaXPayGetUploadFileSignResponse>(nameof(WxaXPayGetUploadFileSignResponse.Sign), "sign", "官方原文（表单签名，敏感值）");
        AssertPropertyType<WxaXPayResponseComplaintRequest>(nameof(WxaXPayResponseComplaintRequest.ResponseImages), typeof(List<string>),
            "官方 array of string 且标注必填 ⇒ 无图时须传空数组，不得留 null");
        AssertJsonProperty<WxaXPayRecoverySpecification>(nameof(WxaXPayRecoverySpecification.LimitationStartDate), "limitation_start_date", "官方原文（管控起始日）");
        AssertJsonProperty<WxaXPayRecoverySpecification>(nameof(WxaXPayRecoverySpecification.LimitationDate), "limitation_date", "官方原文（管控日期）——与 limitation_start_date 两列并存，语义不同不得互替");
        AssertJsonProperty<WxaXPayRecoverySpecification>(nameof(WxaXPayRecoverySpecification.LimitationCaseId), "limitation_case_id", "官方原文（管控流水号，与管控通知的 business_code 是官方 join 键）");

        // —— 官方枚举取值锁定（逐页照官方取值表；官方未公布枚举表的语义字段一律不建常量类）——
        AssertConstValues(typeof(WxaXPayOrderStatuses),
            ("Initialized", 0), ("Created", 1), ("PaidWaitProvide", 2), ("Providing", 3), ("Provided", 4),
            ("RefundMerchantDeducted", 5), ("Closed", 6), ("RefundFailed", 7), ("RefundUserReceived", 8),
            ("AdvertRecovered", 9), ("SplitFallbackDone", 10));
        AssertConstValues(typeof(WxaXPayOrderTypes), ("NormalPay", 0), ("NormalRefund", 1), ("IosPay", 7), ("IosRefund", 8));
        AssertConstValues(typeof(WxaXPayPublishTaskStatuses),
            ("None", 0), ("Running", 1), ("FailedOrPartialFailed", 2), ("Success", 3));
        AssertConstValues(typeof(WxaXPayComplaintMediaTypes),
            ("UserComplaintImage", "USER_COMPLAINT_IMAGE"), ("OperationImage", "OPERATION_IMAGE"));
        AssertConstValues(typeof(WxaXPayOperateTypes),
            ("UserCreateComplaint", "USER_CREATE_COMPLAINT"),
            ("UserContinueComplaint", "USER_CONTINUE_COMPLAINT"),
            ("UserResponse", "USER_RESPONSE"),
            ("PlatformResponse", "PLATFORM_RESPONSE"),
            ("MerchantResponse", "MERCHANT_RESPONSE"),
            ("MerchantConfirmComplete", "MERCHANT_CONFIRM_COMPLETE"),
            ("UserCreateComplaintSystemMessage", "USER_CREATE_COMPLAINT_SYSTEM_MESSAGE"),
            ("ComplaintFullRefundedSystemMessage", "COMPLAINT_FULL_REFUNDED_SYSTEM_MESSAGE"),
            ("UserContinueComplaintSystemMessage", "USER_CONTINUE_COMPLAINT_SYSTEM_MESSAGE"),
            ("UserRevokeComplaint", "USER_REVOKE_COMPLAINT"),
            ("UserComfirmComplaint", "USER_COMFIRM_COMPLAINT"),
            ("PlatformHelpApplication", "PLATFORM_HELP_APPLICATION"),
            ("UserApplyPlatformHelp", "USER_APPLY_PLATFORM_HELP"),
            ("MerchantApproveRefund", "MERCHANT_APPROVE_REFUND"),
            ("MerchantRefuseRerund", "MERCHANT_REFUSE_RERUND"),
            ("UserSubmitSatisfaction", "USER_SUBMIT_SATISFACTION"),
            ("ServiceOrderCancel", "SERVICE_ORDER_CANCEL"),
            ("ServiceOrderComplete", "SERVICE_ORDER_COMPLETE"),
            ("ComplaintPartialRefundedSystemMessage", "COMPLAINT_PARTIAL_REFUNDED_SYSTEM_MESSAGE"),
            ("ComplaintRefundReceivedSystemMessage", "COMPLAINT_REFUND_RECEIVED_SYSTEM_MESSAGE"));

        // 反直觉点锁定：官方协商操作类型表自带两处拼写缺陷（COMFIRM / RERUND），且它们是<b>线格式值</b>
        // ⇒ 常量名按正确语义命名，值必须照官方原文，「顺手修正」即比对不上官方枚举。
        typeof(WxaXPayOperateTypes).GetField(nameof(WxaXPayOperateTypes.UserComfirmComplaint))!
            .GetRawConstantValue().Should().Be("USER_COMFIRM_COMPLAINT", "官方取值表原文即 COMFIRM（少一个 r）");
        typeof(WxaXPayOperateTypes).GetField(nameof(WxaXPayOperateTypes.MerchantRefuseRerund))!
            .GetRawConstantValue().Should().Be("MERCHANT_REFUSE_RERUND", "官方取值表原文即 RERUND（少一个 f）");

        // 订单类型只有 0/1/7/8（2~6 官方未公布）⇒ 上方 AssertConstValues 走「字典等价」口径，
        // 为「凑齐连续值」补常量会直接红在这一条上，故此处无需额外的 NotContain 断言。
    }

    /// <summary>
    /// 契约守卫 XP6：<b>词表外凭据参数脱敏闭环</b> —— <c>pay_sig</c> 必须靠
    /// <see cref="XPaySensitiveQueryKeys"/> 登记，且登记面与组件静态词表不交叉、登记时机在模块注册委托。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 组件 <c>SensitiveUrlRedactor</c> 是<b>精确匹配</b>词表语义：词表里的 <c>sig</c> / <c>sign</c> /
    /// <c>signature</c> 覆盖不到 <c>pay_sig</c> ⇒ 若无人登记，该支付签名会随
    /// <c>ApiException.RequestUri</c> / 遥测 URL 明文外泄（AGENTS §8 的「补词表 / 登记豁免」二选一，本线取登记一支）。
    /// </para>
    /// <para>
    /// <b>反向自过期</b>：组件词表一旦收录 <c>pay_sig</c>，本条先红，此时登记调用降级为冗余，
    /// 须同批从 <see cref="XPaySensitiveQueryKeys.All"/> 移除（保留会让真实缺口被冗余条目掩盖）。
    /// </para>
    /// </remarks>
    [Fact]
    public void XPayCredentialQueryParameters_ShouldBeRedactedByRegistrationNotVocabulary()
    {
        // ① 登记面本身锁死：恰为 pay_sig 一支。
        XPaySensitiveQueryKeys.PaySig.Should().Be("pay_sig", "官方 Query 参数名原文");
        XPaySensitiveQueryKeys.All.Should().Equal(new[] { "pay_sig" },
            "本线词表外凭据登记面恰为 pay_sig；新增 Query 凭据须同批扩充本表并复跑守卫 XP6");

        var vocabulary = ReadComponentSensitiveVocabulary();
        vocabulary.Should().NotBeEmpty("未能读取组件脱敏词表（组件版本或 SensitiveFieldNames 字段名变更，请同步本守卫）");

        // ② 登记的前提：组件词表覆盖不到 pay_sig（否则降级为冗余，须同批撤销登记）。
        vocabulary.Should().NotContain(XPaySensitiveQueryKeys.PaySig,
            "组件 SensitiveUrlRedactor 静态词表已收录 pay_sig ⇒ XPay 模块注册期的 RegisterAll 调用降级为冗余，"
            + "请同批从 XPaySensitiveQueryKeys.All 移除（并同步接口注释与本守卫）");

        // ③ 词表侧的两支必须真的在词表里（否则「signature 不登记」的裁决失去依据，本域凭据面出现真缺口）。
        vocabulary.Should().Contain("access_token", "官方 Query 令牌参数须由组件词表覆盖（本线不重复登记）");
        vocabulary.Should().Contain("signature", "用户态签名由词表精确覆盖 ⇒ 本线刻意不登记（见 XPaySensitiveQueryKeys remarks）");

        // ④ 登记面与词表不交叉（自过期闸的正面表述）。
        XPaySensitiveQueryKeys.All
            .Where(n => vocabulary.Contains(n, StringComparer.Ordinal))
            .Should().BeEmpty("登记面与组件词表交叉 ⇒ 该键已由词表覆盖，登记条目已过期");

        // ⑤ 不得登记不存在的键：登记面 ⊆ 本线实际声明的 Query 参数名。
        var declaredQueryNames = typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .SelectMany(static m => m.GetParameters())
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Where(static n => !string.IsNullOrEmpty(n))
            .Select(static n => n!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        declaredQueryNames.Should().Contain(XPaySensitiveQueryKeys.PaySig,
            "正对照：pay_sig 必须真的作为 [Query] 参数存在于本线接口面上（否则本守卫退化为真空断言）");
        XPaySensitiveQueryKeys.All.Should().OnlyContain(n => declaredQueryNames.Contains(n),
            "登记面出现本线接口面上并不存在的键 ⇒ 无根据的全局脱敏扩张，会妨碍排障");

        // ⑥ 登记时机 + 运行期功能断言：走真实装配路径（AddMpApp + AddXPayApi），
        //    装配后 pay_sig 必须已被掩码，且词表外参数保留原文。
        var services = new ServiceCollection();
        services.AddMpApp(static c =>
        {
            c.AppKey = "xpay-guard";
            c.AppId = "wx-guard";
            c.AppSecret = "secret-guard";
        });

        services.AddMiniProgramServices(static b => b.AddXPayApi());

        var redact = ReadComponentRedactMethod();
        const string Url = "https://api.weixin.qq.com/xpay/query_order?access_token=AT&pay_sig=SIG&foo=bar";
        var redacted = redact.Invoke(null, new object?[] { Url }) as string;

        redacted.Should().NotBeNull();
        redacted.Should().Contain("pay_sig=***REDACTED***",
            "AddXPayApi 的模块注册委托必须完成 SensitiveUrlKeys.RegisterAll（支付签名不得随 URL 明文输出）");
        redacted.Should().Contain("access_token=***REDACTED***", "令牌面由组件词表覆盖（同 URL 一并掩码）");
        redacted.Should().Contain("foo=bar", "词表与登记面之外的参数保留原文（精确匹配语义，排障可用性）");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>）。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    /// <summary>取端点方法的 <c>[Body]</c> 请求 DTO 类型（零请求体端点返回 <c>null</c>）。</summary>
    private static Type? RequestTypeOf(Type interfaceType, string methodName)
        => interfaceType
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetParameters()
            .SingleOrDefault(static p => p.GetCustomAttribute<BodyAttribute>() != null)
            ?.ParameterType;

    /// <summary>取接口声明的方法数（DeclaredOnly，不含继承面）。</summary>
    private static int MethodCount(Type type)
        => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void AssertPropertyType<T>(string propertyName, Type expectedType, string because = "")
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType,
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}（共用/口径裁决{(because.Length > 0 ? "：" + because : string.Empty)}）");
    }

    private static void AssertConstValues(Type constType, params (string Name, object Value)[] expected)
    {
        var actual = constType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .ToDictionary(static f => f.Name, static f => f.GetRawConstantValue()!);

        actual.Should().BeEquivalentTo(expected.ToDictionary(static e => e.Name, static e => e.Value),
            because: $"{constType.Name} 的官方取值集漂移须先核对官方枚举表再同批调整本守卫");
    }

    /// <summary>组件静态脱敏词表（反射读取，与 G7 / WEB3 / ADS-B5 同源路径）。</summary>
    private static List<string> ReadComponentSensitiveVocabulary()
    {
        var redactor = typeof(Mud.HttpUtils.ApiException).Assembly
            .GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（XP6 依赖其词表）");

        var field = redactor!.GetField("SensitiveFieldNames",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull("组件词表字段名变更请同步本守卫（否则 XP6 恒真）");

        var value = field!.GetValue(null) as System.Collections.IEnumerable;
        value.Should().NotBeNull();

        return value!.Cast<string>().ToList();
    }

    private static MethodInfo ReadComponentRedactMethod()
    {
        var redactor = typeof(Mud.HttpUtils.ApiException).Assembly.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（XP6 功能断言依赖其 Redact 通路）");

        var redact = redactor!.GetMethod("Redact",
            BindingFlags.Public | BindingFlags.Static, binder: null, new[] { typeof(string) }, modifiers: null);
        redact.Should().NotBeNull("组件 SensitiveUrlRedactor.Redact(string) 必须存在（异常与遥测 URL 脱敏的统一出口）");

        return redact!;
    }

    /// <summary>
    /// 扫描目录下全部源码里的「路由字面量」（剥离注释后取引号包裹且以 <c>/</c> 开头的字符串），
    /// 返回「相对路径 → 字面量列表」。剥离注释是必要的：裁决性注释里出现的路径不算声明。
    /// </summary>
    private static Dictionary<string, List<string>> RouteLiteralsByFile(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./{}]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal))
            {
                continue;
            }

            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            var hits = literalRegex.Matches(code).Select(static m => m.Groups[1].Value).ToList();
            if (hits.Count > 0)
            {
                map[normalized] = hits;
            }
        }

        return map;
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}
