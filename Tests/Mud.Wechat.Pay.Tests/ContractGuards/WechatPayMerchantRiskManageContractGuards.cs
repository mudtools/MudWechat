// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MerchantRiskManage;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-7 商户风险管理域契约守卫（<b>11 端点 / 9 条 (方法, 路由)</b>：风险订单同步 4 + 风险共建 3 + 违规通知 URL 四件套 4）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（2026-10-14 两轮逐页核验，服务商与普通商户两棵树都覆盖）：
/// <c>4013894221</c> / <c>4013894247</c> 查询风险订单（同一路由两个商户形态）、
/// <c>4013894227</c> / <c>4013894250</c> 回传处置结果（同形态）、
/// <c>4012692160</c> 上报订单关联信息、<c>4012467535</c> 查询风险信息、<c>4012467540</c> 处理结果回传、
/// <c>4012471333</c> / <c>4012471330</c> / <c>4012471327</c> / <c>4012471334</c> 违规通知回调地址的
/// 创建 / 修改 / 查询 / 删除。逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>「11 行方法、9 条路由」是本域的第一处计数陷阱</b>：查询风险订单与回传处置结果各有
/// 「服务商面 / 普通商户面」两支，<b>同方法同路由、字段表不同</b> ⇒ 端点数按方法计（11），
/// 路由数按 (方法, 路由) 去重（9）。MRB-B1 两个数都锁：只锁一个就会在「有人把两形态合并成一支」
/// 或「有人再拆出第三条同路由」时静默通过。
/// </para>
/// <para>
/// <b>本守卫同时是支付线动词面的破例证明</b>：此域之前，<c>Src/Pay</c> 全部生成式接口只有 GET/POST。
/// 本域出现 <b>PUT</b>（修改通知地址）、<b>DELETE</b>（删除通知地址），下一域（门店）出现 <b>PATCH</b>。
/// MRB-B1 因此不只断言路由，还断言「按 <c>HttpMethodAttribute</c> 基类枚举」与「按五个具体动词属性枚举」
/// <b>得到同一个 11</b> —— 若 PUT/DELETE 不派生自该基类，一切按基类扫描的通道（含组件与本项目其余守卫）
/// 都会静默漏掉这两支，而编译与 JIT 调用都看不出异常。
/// </para>
/// <para>
/// <b>本守卫要守住的五处「顺手就会写错」</b>：
/// ① 同一条 <c>/violation-notifications</c> 上 PUT/POST/GET 是 <b>200 + <c>{notify_url}</c></b>、
/// 只有 DELETE 是 <b>204</b> ⇒ 「写操作都 204」在本域不成立；
/// ② 同名 <c>risk_type</c> 有两套类型（风险订单同步面 integer 1~4、风险共建面 string 5 值）；
/// ③ 官方两处误拼 <c>risk_discove_time</c>（缺 r）与 <c>same_deviced</c>（多 d），修正拼写即静默丢字段；
/// ④ 合单子单的商户号在本域是 <c>mch_id</c>（带下划线），与全域惯例 <c>mchid</c> 相反；
/// ⑤ 五个整型字段与 <c>risk_score</c>、<c>refund_amount</c>、<c>additional_punish_type</c> 官方<b>未给</b>
/// 取值字典 / 单位 / 范围 ⇒ 按标量透传，注释不得据第三方 SDK 补数。
/// </para>
/// </remarks>
public class WechatPayMerchantRiskManageContractGuards
{
    private const string RegistryGroupName = "MerchantRiskManage";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MerchantRiskManage";

    private const string Prefix = "/v3/merchant-risk-manage/";

    private const string RiskTradesRoute = Prefix + "ec-trade-risk/query-risk-trades";

    private const string DisposeResultRoute = Prefix + "ec-trade-risk/recv-dispose-result";

    private const string ViolationNotificationsRoute = Prefix + "violation-notifications";

    /// <summary>
    /// MRB-B1：11 端点的方法与路由照官方原文；<b>11 个方法对应 9 条 (方法, 路由)</b>；
    /// 只有 DELETE 是 204 裸 <see cref="Task"/>；path / query 位置逐支锁定。
    /// </summary>
    [Fact]
    public void MerchantRiskManageEndpoints_ShouldMatchOfficialRoutesAndVerbFaces()
    {
        // 风险订单同步族：同一路由两个商户形态（字段表不同 ⇒ 方法与包体类型都不同）。
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByPartnerAsync), RiskTradesRoute);
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByMchAsync), RiskTradesRoute);
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.RecvDisposeResultByPartnerAsync), DisposeResultRoute);
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.RecvDisposeResultByMchAsync), DisposeResultRoute);

        // 风险共建族：三支各自独立路由。
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.ReportTradeUnionInformationAsync),
            Prefix + "report-trade-union-information");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantRiskManageService.QueryTradeRiskInformationAsync),
            Prefix + "trade-risk-information");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.ReportTradeRiskResultAsync),
            Prefix + "trade-risk-result");

        // 违规通知四件套：同一资源路径上的 POST / PUT / GET / DELETE 四个面。
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantRiskManageService.CreateViolationNotificationAsync),
            ViolationNotificationsRoute);
        AssertRoute<PutAttribute>(nameof(IWechatPayMerchantRiskManageService.UpdateViolationNotificationUrlAsync),
            ViolationNotificationsRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantRiskManageService.QueryViolationNotificationAsync),
            ViolationNotificationsRoute);
        AssertRoute<DeleteAttribute>(nameof(IWechatPayMerchantRiskManageService.DeleteViolationNotificationAsync),
            ViolationNotificationsRoute);

        // 计数双锁：方法数 11、(方法, 路由) 去重后 9。
        DeclaredMethods().Should().HaveCount(11,
            "本域 11 端点按方法计（风险订单与处置结果各有两个商户形态）；数量变化须同批更新 MRB-B1 与 Tests/ContractBaseline.cs");
        AllEndpointMethods().Should().HaveCount(11,
            "按 HttpMethodAttribute 基类枚举同样得到 11：两条计数口径一致才说明 PUT / DELETE 真被基类捕获");
        DistinctVerbRoutes().Should().HaveCount(9,
            "去重后 9 条 (方法, 路由)：query-risk-trades 与 recv-dispose-result 各被两支方法共用");

        // 动词面破例的机制锁：按基类枚举与按五个具体动词属性枚举必须得到同一个集合。
        var viaBase = typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .ToArray();
        var viaConcrete = typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Where(static a => a is PostAttribute or GetAttribute or PutAttribute
                               or PatchAttribute or DeleteAttribute)
            .ToArray();
        viaConcrete.Should().HaveCount(11,
            "PUT / DELETE 必须真是 HttpMethodAttribute 的子类：否则按基类扫描的每一条通道都会静默漏掉这两支");
        viaBase.Length.Should().Be(viaConcrete.Length);

        // 204 面：本域恰一支无应答包体（DELETE），PUT / POST / GET 三支同路由却是 200 + JSON。
        NoContentMethodsOf(typeof(IWechatPayMerchantRiskManageService))
            .Should().BeEquivalentTo(new[] { nameof(IWechatPayMerchantRiskManageService.DeleteViolationNotificationAsync) },
            "官方只有「删除通知地址」是 204：把同路由的 PUT 也写成裸 Task 即丢掉回显的 notify_url");
        FindMethod(nameof(IWechatPayMerchantRiskManageService.UpdateViolationNotificationUrlAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantRiskManageViolationNotificationResponse>));

        // path / query 形态：全域只有查询风险信息一支带 query，且无任何 path 参数。
        NoContentMethodsOf(typeof(IWechatPayMerchantRiskManageService)).Should().HaveCount(1);
        foreach (var methodName in AllEndpointMethods())
        {
            PathKeysOf(methodName).Should().BeEmpty($"{methodName}：本域九条路由零 path 参数");
        }

        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.QueryTradeRiskInformationAsync))
            .Should().BeEquivalentTo(new[] { "out_trade_no", "sp_mchid", "acquiring_bank_id", "channel_id", "sub_mchid", "complaints_information" },
            "本域唯一纯 query 支，六参数名照官方参数表");
        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByPartnerAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.CreateViolationNotificationAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.UpdateViolationNotificationUrlAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.DeleteViolationNotificationAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.QueryViolationNotificationAsync)).Should().BeEmpty();

        // 包体形态：DELETE 与 GET 无请求体；POST / PUT 各有明确的请求类型。
        FindMethod(nameof(IWechatPayMerchantRiskManageService.DeleteViolationNotificationAsync))
            .GetParameters().Should().OnlyContain(static p => p.ParameterType == typeof(CancellationToken),
            "删除支请求侧只有 Authorization / Accept 两个头：塞入包体即造出官方不接受的字段");
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.CreateViolationNotificationAsync))
            .Should().Be(typeof(MerchantRiskManageViolationNotificationRequest));
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.UpdateViolationNotificationUrlAsync))
            .Should().Be(typeof(MerchantRiskManageViolationNotificationRequest));
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.ReportTradeUnionInformationAsync))
            .Should().Be(typeof(MerchantRiskManageTradeUnionReportRequest));
    }

    /// <summary>
    /// MRB-B2：路由全在 <c>/v3/merchant-risk-manage/</c> 前缀下 + 三处「非本域」零声明
    /// （消费者投诉通知面 / 支付分风控面 / 门店面），每处均须正对照。
    /// </summary>
    [Fact]
    public void MerchantRiskManageRoutes_ShouldStayOnOwnPrefixAndNotDeclareNeighbouringFaces()
    {
        var routes = RoutesOf(typeof(IWechatPayMerchantRiskManageService));

        routes.Should().HaveCount(11, "本域 11 端点（按方法计，见 MRB-B1 的双计数裁决）");
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(11, "本域十一支全在 /v3/merchant-risk-manage/ 下，无第二前缀");

        // 零声明①：消费者投诉通知 URL 族（complaint-notifications）不属本域。
        routes.Should().NotContain(static r => r.Contains("complaint", StringComparison.Ordinal),
            "投诉通知面在商户服务域，且其字段名是 url（必填）、应答另含 mchid ⇒ 与本域 notify_url 不同契约");
        // 正对照：本域确有名为 complaints_information 的 query 字段 —— 「同词根 ≠ 同面」，扫描机制没漏。
        QueryNamesOf(nameof(IWechatPayMerchantRiskManageService.QueryTradeRiskInformationAsync))
            .Should().Contain("complaints_information");

        // 零声明②：支付分风控面（/v3/payscore/…）与商户风险面互斥。
        routes.Should().NotContain(static r => r.Contains("pay-score", StringComparison.Ordinal)
                                               || r.Contains("payscore", StringComparison.Ordinal),
            "支付分风控是独立域：路由前缀与字段体系均不互通，混入即跨域错参");
        // 正对照：直连支付分域在同口径扫描下确有 /v3/payscore/ 路由（本域零声明不是扫描失效）。
        RoutesOf(typeof(IWechatPayPayScoreService))
            .Should().Contain(static r => r.StartsWith("/v3/payscore/", StringComparison.Ordinal));

        // 零声明③：门店面（/v3/merchant-store/）与本域同批落地但互不重叠。
        routes.Intersect(RoutesOf(typeof(IWechatPayMerchantStoreService)))
            .Should().BeEmpty("风险面与门店面是两个域，路由交集非空即说明有人把端点挂错了域");
        RoutesOf(typeof(IWechatPayMerchantStoreService))
            .Should().Contain(static r => r.StartsWith("/v3/merchant-store/", StringComparison.Ordinal),
            "正对照：门店域确有路由，上面的零交集断言不是两边都空");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", RegistryGroupName))
            .Should().BeTrue("正对照：Interfaces/MerchantRiskManage/ 确实已落位");
    }

    /// <summary>
    /// MRB-B3：官方字段名逐项锁定 —— 含两处<b>官方误拼</b>、一处<b>带下划线的 mch_id</b>、
    /// 三处<b>数字打尾段</b>（<c>extra_fields1/2/3</c>）与同名 <c>risk_type</c> 的双类型。
    /// </summary>
    [Fact]
    public void MerchantRiskManageDtos_ShouldExposeOfficialFieldNamesIncludingMisspellings()
    {
        // 商户标识四套体系（照页，不得统一成 mchid）。
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeQueryRequest>(nameof(MerchantRiskManagePartnerRiskTradeQueryRequest.SpMchId), "sp_mchid");
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeQueryRequest>(nameof(MerchantRiskManagePartnerRiskTradeQueryRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<MerchantRiskManageMchRiskTradeQueryRequest>(nameof(MerchantRiskManageMchRiskTradeQueryRequest.MchId), "mchid");
        JsonNameShouldBe<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.AcquiringBankId), "acquiring_bank_id");
        JsonNameShouldBe<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.ChannelId), "channel_id");
        JsonNameShouldBe<MerchantRiskManageTradeUnionSubOrder>(nameof(MerchantRiskManageTradeUnionSubOrder.MchId), "mch_id",
            "合单子单的商户号官方此处带下划线（全域其余面均写 mchid）⇒ 按惯例「纠正」即丢字段");

        // 分页与计数：本域是整型页码 page_index / page_size、总数 total_num。
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeQueryRequest>(nameof(MerchantRiskManagePartnerRiskTradeQueryRequest.PageIndex), "page_index");
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeQueryRequest>(nameof(MerchantRiskManagePartnerRiskTradeQueryRequest.PageSize), "page_size");
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeQueryResponse>(nameof(MerchantRiskManagePartnerRiskTradeQueryResponse.TotalNum), "total_num");
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeQueryResponse>(nameof(MerchantRiskManagePartnerRiskTradeQueryResponse.RiskTradeList), "risk_trade_list");

        // 两处官方误拼：只容「照抄」，不容「修好」。
        JsonNameShouldBe<MerchantRiskManagePartnerRiskTradeItem>(nameof(MerchantRiskManagePartnerRiskTradeItem.RiskDiscoveTime), "risk_discove_time");
        JsonNameShouldBe<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.SameDeviced), "same_deviced");
        DeclaredJsonNamesOf<MerchantRiskManagePartnerRiskTradeItem>()
            .Should().NotContain("risk_discover_time", "出现正确拼写即说明有人「顺手修正」了官方误拼");
        DeclaredJsonNamesOf<MerchantRiskManageTradeUnionReportRequest>()
            .Should().NotContain("same_device");

        // 数字打尾段：提取字段名的正则若不容数字会把 extra_fields1 读成 extra_fields。
        JsonNameShouldBe<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.ExtraFields1), "extra_fields1");
        JsonNameShouldBe<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.ExtraFields2), "extra_fields2");
        JsonNameShouldBe<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.ExtraFields3), "extra_fields3");
        DeclaredJsonNamesOf<MerchantRiskManageTradeUnionReportRequest>().Should().Contain("combine_mchid");

        // 处置结果族：array[integer] 与 boolean 的行内枚举字段名。
        JsonNameShouldBe<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.DispConclusion), "disp_conclusion");
        JsonNameShouldBe<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.DispAddition), "disp_addition");
        JsonNameShouldBe<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.ExistComplaint), "exist_complaint");
        JsonNameShouldBe<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.FinalRiskResult), "final_risk_result");
        JsonNameShouldBe<MerchantRiskManageDisposeResultResponse>(nameof(MerchantRiskManageDisposeResultResponse.RspMsg), "rsp_msg");

        // 风险共建族：请求与应答的字段名差（punish_description 只在应答侧）。
        JsonNameShouldBe<MerchantRiskManageTradeRiskResultRequest>(nameof(MerchantRiskManageTradeRiskResultRequest.AdditionalPunishType), "additional_punish_type");
        JsonNameShouldBe<MerchantRiskManageTradeRiskResultResponse>(nameof(MerchantRiskManageTradeRiskResultResponse.PunishDescription), "punish_description");
        JsonNameShouldBe<MerchantRiskManageTradeRiskInfoResponse>(nameof(MerchantRiskManageTradeRiskInfoResponse.RiskScore), "risk_score");
        JsonNameShouldBe<MerchantRiskManageViolationNotificationRequest>(nameof(MerchantRiskManageViolationNotificationRequest.NotifyUrl), "notify_url");

        // 生活缴费 / 充值子树：官方名单是 acc_no / acc_name（不是 account_no）。
        JsonNameShouldBe<MerchantRiskManageLivingExpensesFields>(nameof(MerchantRiskManageLivingExpensesFields.AccNo), "acc_no");
        JsonNameShouldBe<MerchantRiskManageLivingExpensesFields>(nameof(MerchantRiskManageLivingExpensesFields.AccName), "acc_name");
        JsonNameShouldBe<MerchantRiskManageRechargeFields>(nameof(MerchantRiskManageRechargeFields.AccountType), "account_type");

        // 同名 risk_type 的双类型（integer 面 vs string 面）——跨族赋值即错。
        PropertyTypeOf<MerchantRiskManagePartnerRiskTradeItem>(nameof(MerchantRiskManagePartnerRiskTradeItem.RiskType))
            .Should().Be(typeof(int?), "风险订单同步面的 risk_type 官方是 integer（行内枚举 1~4）");
        PropertyTypeOf<MerchantRiskManageTradeRiskInfoResponse>(nameof(MerchantRiskManageTradeRiskInfoResponse.RiskType))
            .Should().Be(typeof(string), "风险共建面的 risk_type 官方是 string（5 值），两套类型不得共用属性");

        // 全域名单形态：官方名单全小写下划线（数字尾段除外），出现大写字母即被驼峰化。
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotBeEmpty("参照集为空会让下面的名单形态断言静默恒真");
        allNames.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写字母即说明有人把 snake_case 驼峰化了");
        allNames.Should().Contain(static n => n.Contains('_'),
            "正对照：本域确有下划线名单，上面的形态断言不是空集恒真");

        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset");
                property.PropertyType.Should().NotBe(typeof(DateTime));
                if (property.Name.EndsWith("Time", StringComparison.Ordinal))
                {
                    property.PropertyType.Should().Be(typeof(string), $"{type.Name}.{property.Name} 时间字段恒 string");
                }
            }
        }
    }

    /// <summary>
    /// MRB-B4：官方取值集合逐项锁定（常量类恰 2 个）+ <c>notify_url</c> 的「官方选填」事实。
    /// </summary>
    [Fact]
    public void MerchantRiskManageConstants_ShouldMatchOfficialValueSetsAndRoster()
    {
        ValuesOf(typeof(MerchantRiskManageTradeRiskTypes)).Should().BeEquivalentTo(new[]
        {
            "OTHER_RISK", "INDUCE_PAYMENT", "FRAUD", "GAMBLING", "CYBERSEX",
        }, "官方 risk_type（string 面）5 值：与风险订单同步面的 integer 1~4 是两套体系，不得互映射");

        ValuesOf(typeof(MerchantRiskManagePunishTypes)).Should().BeEquivalentTo(new[]
        {
            "OTHER_PUNISH", "DISABLE_PAYMENT", "SUSPEND_SETTLEMENT", "DISABLE_WITHDRAW",
            "REMOVE_MERCHANTS", "ADD_MERCHANTS_BLACKLIST", "REFUND",
            "FREEZING_FUND_BY_TRANSACTION", "FREEZING_FUND_BY_MERCHANTS", "BLOCKED_USER",
        }, "官方 punish_type 10 值：REFUND 与退款域的语义无关，是本值集的一个处置手段");

        // 正对照：另一支付分域的值集不与本域交叉（防「跨域取别名」）。
        ValuesOf(typeof(MerchantRiskManagePunishTypes))
            .Should().NotContain("WAIT_PAY");

        ConstantClassNames().Should().BeEquivalentTo(new[]
        {
            nameof(MerchantRiskManageTradeRiskTypes),
            nameof(MerchantRiskManagePunishTypes),
        }, "本域常量类恰 2 个：其余整数枚举官方未给取值字典，建常量即臆造（见 MRB-B5）");

        // notify_url 两支都「选填」：SDK 不做客户端必填校验，此事实须被锁住而非「顺手补齐」。
        PropertyTypeOf<MerchantRiskManageViolationNotificationRequest>(nameof(MerchantRiskManageViolationNotificationRequest.NotifyUrl))
            .Should().Be(typeof(string));
        DeclaredJsonNamesOf<MerchantRiskManageViolationNotificationRequest>().Should().BeEquivalentTo(new[] { "notify_url" },
            "写入包体只有 notify_url：加任何 mchid 回显字段即造出官方没有的双写入口");
        DeclaredJsonNamesOf<MerchantRiskManageViolationNotificationResponse>().Should().BeEquivalentTo(new[] { "notify_url" },
            "本域应答不回显 mchid（投诉通知族才回 mchid + url）⇒ 两族应答不得共用");
    }

    /// <summary>
    /// MRB-B5：「官方未给 / 不建模」裁决的可机械化部分 —— 无取值字典的整型字段按 <see cref="int"/> 透传、
    /// <c>refund_amount</c> 无单位、<c>additional_punish_type</c> 是自由文本、<c>risk_score</c> 无范围。
    /// </summary>
    /// <remarks>
    /// 本条守的是<b>注释与类型的诚实性</b>：这类字段最容易被后来者按第三方 SDK 或「常识」补齐，
    /// 一旦补齐，官方改口径时本 SDK 就成了错误契约的唯一持有者，且症状是运行期才被拒。
    /// </remarks>
    [Fact]
    public void MerchantRiskManageUnspecifiedOfficialFields_ShouldStayScalarsWithoutInventedEnums()
    {
        // 官方未给取值字典的整型字段：一律 int?，不得建常量类。
        var unspecifiedIntFields = new[]
        {
            (typeof(MerchantRiskManageTradeUnionReportRequest), nameof(MerchantRiskManageTradeUnionReportRequest.RiskLevel)),
            (typeof(MerchantRiskManageTradeUnionReportRequest), nameof(MerchantRiskManageTradeUnionReportRequest.PhoneFrom)),
            (typeof(MerchantRiskManageTradeUnionReportRequest), nameof(MerchantRiskManageTradeUnionReportRequest.HighRiskGoods)),
            (typeof(MerchantRiskManageRechargeFields), nameof(MerchantRiskManageRechargeFields.AccountType)),
            (typeof(MerchantRiskManageLivingExpensesFields), nameof(MerchantRiskManageLivingExpensesFields.Type)),
            (typeof(MerchantRiskManageTradeRiskInfoResponse), nameof(MerchantRiskManageTradeRiskInfoResponse.RiskScore)),
        };
        foreach (var (type, propertyName) in unspecifiedIntFields)
        {
            type.GetProperty(propertyName)!.PropertyType.Should().Be(typeof(int?),
                $"{type.Name}.{propertyName} 官方未给取值字典：建成枚举即臆造契约");
        }

        // refund_amount：官方只写「退款金额」，两轮复核均未印单位 ⇒ 按 int? 透传，不得当作「分」。
        PropertyTypeOf<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.RefundAmount))
            .Should().Be(typeof(int?));
        DeclaredJsonNamesOf<MerchantRiskManagePartnerDisposeResultRequest>()
            .Should().NotContain(new[] { "refund_amount_in_cents", "currency" },
            "官方本页应答与请求均无币种字段：补 currency 即造出官方不认的字段");

        // additional_punish_type：自由文本（示例形态 SUSPEND_SETTLEMENT_NDAYS:10），官方未给字典。
        PropertyTypeOf<MerchantRiskManageTradeRiskResultRequest>(nameof(MerchantRiskManageTradeRiskResultRequest.AdditionalPunishType))
            .Should().Be(typeof(string));

        // disp_conclusion 是 array[integer]（1-未处置,2-拦截订单,3-关闭店铺），不是 string 枚举。
        PropertyTypeOf<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.DispConclusion))
            .Should().Be(typeof(List<int>), "官方类型是 array[integer]：建成字符串列表即被 PARAM_ERROR 拒");

        // exist_complaint 是 boolean（false-无，true-有），不是 0/1 整型。
        PropertyTypeOf<MerchantRiskManagePartnerDisposeResultRequest>(nameof(MerchantRiskManagePartnerDisposeResultRequest.ExistComplaint))
            .Should().Be(typeof(bool?));

        // 本域不建模消费者投诉通知面（属商户服务域）：类型名不得出现该族。
        DomainTypeNames().Should().NotContain(static n => n.Contains("Complaint", StringComparison.Ordinal),
            "投诉通知面属商户服务域（字段名是 url 且必填、应答另含 mchid），本域不得建其 DTO");

        // 上报订单关联信息的敏感字段是密文载体：DTO 侧恒 string，SDK 不做加解密也不建密文类型。
        PropertyTypeOf<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.Phone))
            .Should().Be(typeof(string));
        PropertyTypeOf<MerchantRiskManageTradeUnionReportRequest>(nameof(MerchantRiskManageTradeUnionReportRequest.CertificatesNumber))
            .Should().Be(typeof(string));
    }

    /// <summary>
    /// MRB-B6：判错面继承 + 本域「请求分建 / 应答共用」与「同路由多形态」的归并裁决。
    /// </summary>
    [Fact]
    public void MerchantRiskManageResponses_ShouldDeriveFromBaseAndFollowShareSplitRulings()
    {
        var responseTypes = new[]
        {
            typeof(MerchantRiskManagePartnerRiskTradeQueryResponse),
            typeof(MerchantRiskManageMchRiskTradeQueryResponse),
            typeof(MerchantRiskManageDisposeResultResponse),
            typeof(MerchantRiskManageTradeUnionReportResponse),
            typeof(MerchantRiskManageTradeRiskInfoResponse),
            typeof(MerchantRiskManageTradeRiskResultResponse),
            typeof(MerchantRiskManageViolationNotificationResponse),
        };
        foreach (var responseType in responseTypes)
        {
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        var requestTypes = new[]
        {
            typeof(MerchantRiskManagePartnerRiskTradeQueryRequest),
            typeof(MerchantRiskManageMchRiskTradeQueryRequest),
            typeof(MerchantRiskManagePartnerDisposeResultRequest),
            typeof(MerchantRiskManageMchDisposeResultRequest),
            typeof(MerchantRiskManageTradeUnionReportRequest),
            typeof(MerchantRiskManageTradeRiskResultRequest),
            typeof(MerchantRiskManageViolationNotificationRequest),
        };
        foreach (var requestType in requestTypes)
        {
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求类型，不得挂在应答基类上");
        }

        // 分建：查询风险订单两支的请求与应答都不同（两页字段表都不同）。
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByPartnerAsync))
            .Should().Be(typeof(MerchantRiskManagePartnerRiskTradeQueryRequest));
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByMchAsync))
            .Should().Be(typeof(MerchantRiskManageMchRiskTradeQueryRequest));
        FindMethod(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByPartnerAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantRiskManagePartnerRiskTradeQueryResponse>));
        FindMethod(nameof(IWechatPayMerchantRiskManageService.QueryRiskTradesByMchAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantRiskManageMchRiskTradeQueryResponse>));
        // 条目级的分建点：服务商应答多 sp_mchid 一行，普通商户应答没有。
        DeclaredJsonNamesOf<MerchantRiskManagePartnerRiskTradeItem>().Should().Contain("sp_mchid");
        DeclaredJsonNamesOf<MerchantRiskManageMchRiskTradeItem>().Should().NotContain("sp_mchid");

        // 分建 + 共用：回传处置结果两支请求分建、应答共用（两页应答表零差异）。
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.RecvDisposeResultByPartnerAsync))
            .Should().Be(typeof(MerchantRiskManagePartnerDisposeResultRequest));
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.RecvDisposeResultByMchAsync))
            .Should().Be(typeof(MerchantRiskManageMchDisposeResultRequest));
        FindMethod(nameof(IWechatPayMerchantRiskManageService.RecvDisposeResultByPartnerAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantRiskManageService.RecvDisposeResultByMchAsync)).ReturnType,
            "官方两页应答表同为单字段 rsp_msg ⇒ 共用一个应答类型（分身即造出永不填充的假字段）");

        // 共用：创建与修改共用请求；创建 / 修改 / 查询三支共用应答（DELETE 无应答）。
        BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.CreateViolationNotificationAsync))
            .Should().Be(BodyTypeOf(nameof(IWechatPayMerchantRiskManageService.UpdateViolationNotificationUrlAsync)));
        FindMethod(nameof(IWechatPayMerchantRiskManageService.CreateViolationNotificationAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantRiskManageService.QueryViolationNotificationAsync)).ReturnType);

        // 分建：处理结果回传的请求与应答不是同一类型（应答多 punish_description、请求多不出）。
        DeclaredJsonNamesOf<MerchantRiskManageTradeRiskResultResponse>().Should().Contain("punish_description");
        DeclaredJsonNamesOf<MerchantRiskManageTradeRiskResultRequest>().Should().NotContain("punish_description");

        // 本域不得为 204 端点伪造应答类型。
        DomainTypeNames().Should().NotContain(static n => n.EndsWith("DeleteResponse", StringComparison.Ordinal),
            "删除支是 204 无包体：伪造应答类型即造出永远为空的假字段");
    }

    /// <summary>
    /// MRB-B7：本域 19 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void MerchantRiskManageDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(19,
            "风险 DTO：风险订单族 9（两形态请求 2 + 应答 2 + 条目 2 + 两形态处置请求 2 + 共用应答 1）+ "
            + "上报关联信息族 5（请求 + 充值/生活缴费子树 + 合单子单 + 应答）+ 风险共建族 3 + 违规通知族 2 = 19");

        foreach (var type in domainTypes)
        {
            MerchantRiskManageJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MerchantRiskManageJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（本域类型 = 19 DTO + 2 常量类，上下文类已排除）。
        DomainTypeNames().Should().HaveCount(21, "本域类型 = 19 DTO + 2 常量类");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MerchantRiskManageJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MerchantRiskManage 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMerchantRiskManageWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayMerchantRiskManageService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayMerchantRiskManageService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMerchantRiskManageService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayMerchantRiskManageService.{methodName} 必须存在");

    private static string[] DeclaredMethods()
        => typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static m => m.Name)
            .ToArray();

    private static string[] AllEndpointMethods()
        => typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.GetCustomAttributes<HttpMethodAttribute>().Any())
            .Select(static m => m.Name)
            .ToArray();

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => (a.GetType().Name, a.RequestUri!)))
            .Distinct()
            .ToArray();

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] NoContentMethodsOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static IEnumerable<Type> DomainTypes()
        => typeof(MerchantRiskManageViolationNotificationRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(MerchantRiskManageViolationNotificationRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(MerchantRiskManageViolationNotificationRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName, string because = "")
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName, because);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName, string because = "")
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）{because}");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be(expectedJsonName, because);
    }

    private static Type PropertyTypeOf<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在");
        return property!.PropertyType;
    }

    private static string[] DeclaredJsonNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
