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
using Mud.Wechat.Pay.DataModels.Repayment;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-6 平台商户统一收单·还款域契约守卫（<b>3 端点</b>：预下单 + 查单 + 关单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（合作伙伴文档中心「平台收付通-金融还款解决方案 → 还款支付」，2026-10-14 逐页核验，
/// 各页「支持商户」栏恒为【平台商户】）：4015124463 还款下单、4015124483 还款单查询、4015124497 商户关单。
/// 同章侧栏另有 APP 调起支付 / H5 调起支付 / 小程序调起支付 / 微信内网页调起支付（<b>端上形态</b>）、
/// 支付成功回调通知（<b>接收面</b>）与退款（<b>另一章</b>）⇒ 均非本域 HTTP 端点，守卫 RP-B2 带正对照。
/// 逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>SKIT 清单本域 3 行 ⇒ 3 端点、零归并</b>（该树在文档中心只有这三条 HTTP 契约）。
/// </para>
/// <para>
/// <b>本守卫要守住的四处「顺手就会写错」</b>：
/// ① 子单 <c>mchid</c> 官方要求填<b>还款发起方（服务商）商户号</b> <c>combine_mchid</c> ——
/// 与合单支付「子单 mchid = 各参与商户」的语义<b>相反</b>，照抄即把钱收错账户；
/// ② 字段名带 <c>combine_</c> 前缀且<b>有数字打头段</b>（<c>h5_info</c> / <c>h5_url</c>）——
/// 提取字段名的正则若不容数字会把 <c>h5_info</c> 读成 <c>_info</c>（本次取证踩过），
/// 驼峰化或改写成 <c>H5Info</c> 之外的名单同样致命；
/// ③ 请求侧的 <c>combine_payer_info</c> 是「实名信息（<c>identity</c> 三字段）」、
/// 应答侧同名节只有选填 <c>openid</c> ⇒ 两型分建，不得复用；
/// ④ 关单是 <b>204 无包体</b> ⇒ 返回裸 <see cref="Task"/>，且还款单号只在 path、包体只有子单列表。
/// </para>
/// </remarks>
public class WechatPayRepaymentContractGuards
{
    private const string RegistryGroupName = "Repayment";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Repayment";

    private const string Prefix = "/v3/repayment/combine-transactions/partner/";

    /// <summary>
    /// RP-B1：3 端点的方法与路由照官方原文；关单支<b>必须</b>返回裸 <see cref="Task"/>（204），
    /// 另两支各有应答类型；path 只有 <c>combine_out_trade_no</c> 一个键。
    /// </summary>
    [Fact]
    public void RepaymentEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentForClose()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayRepaymentService.PrepayAsync), Prefix + "prepay");
        AssertRoute<GetAttribute>(nameof(IWechatPayRepaymentService.QueryByCombineOutTradeNoAsync),
            Prefix + "out-trade-no/{combineOutTradeNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayRepaymentService.CloseAsync),
            Prefix + "out-trade-no/{combineOutTradeNo}/close");

        // 204 面：本域恰一支无应答包体（关单），另两支有类型。
        NoContentMethodsOf(typeof(IWechatPayRepaymentService))
            .Should().BeEquivalentTo(new[] { nameof(IWechatPayRepaymentService.CloseAsync) },
            "官方仅「商户关单」成功应答为 204 无包体：把有应答的端点写成裸 Task 即丢字段");
        FindMethod(nameof(IWechatPayRepaymentService.PrepayAsync)).ReturnType
            .Should().Be(typeof(Task<RepaymentPrepayResponse>));
        FindMethod(nameof(IWechatPayRepaymentService.QueryByCombineOutTradeNoAsync)).ReturnType
            .Should().Be(typeof(Task<RepaymentQueryResponse>));

        // path / query 形态：预下单无 path 无 query；查单只有 path；关单 path + 包体。
        PathKeysOf(nameof(IWechatPayRepaymentService.PrepayAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayRepaymentService.PrepayAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayRepaymentService.QueryByCombineOutTradeNoAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayRepaymentService.CloseAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayRepaymentService.QueryByCombineOutTradeNoAsync))
            .Should().BeEquivalentTo(new[] { "combineOutTradeNo" });
        PathKeysOf(nameof(IWechatPayRepaymentService.CloseAsync))
            .Should().BeEquivalentTo(new[] { "combineOutTradeNo" },
            "还款单号只在 path：包体只有 sub_orders，重复塞单号即造出双写入口");
        BodyTypeOf(nameof(IWechatPayRepaymentService.CloseAsync)).Should().Be(typeof(RepaymentCloseRequest));
        BodyTypeOf(nameof(IWechatPayRepaymentService.PrepayAsync)).Should().Be(typeof(RepaymentPrepayRequest));

        // GET 无请求体。
        FindMethod(nameof(IWechatPayRepaymentService.QueryByCombineOutTradeNoAsync)).GetParameters()
            .Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null);
    }

    /// <summary>
    /// RP-B2：路由全在 <c>/v3/repayment/</c> 前缀下 + 三处「非本 HTTP 面」零声明
    /// （端上调起四支 / 支付成功回调 / 退款族），每处均须正对照。
    /// </summary>
    [Fact]
    public void RepaymentRoutes_ShouldStayOnRepaymentPrefixAndNotDeclareClientSideOrCallbackOrRefundFaces()
    {
        var routes = RoutesOf(typeof(IWechatPayRepaymentService));

        routes.Should().HaveCount(3, "本域 3 端点（数量变化须同批更新 RP-B1 与 Tests/ContractBaseline.cs）");
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(3, "本域三支全在 /v3/repayment/combine-transactions/partner/ 下");

        // 零声明①：调起支付是端上形态（四支），官方无对应 HTTP 端点。
        routes.Should().NotContain(static r => r.Contains("invoke", StringComparison.Ordinal)
                                               || r.Contains("/pay/", StringComparison.Ordinal),
            "APP / H5 / 小程序 / 微信内网页「调起支付」是前端参数签名形态，不属本 HTTP 面");
        // 正对照：本域应答确实给出端上凭据（prepay_id / h5_url），说明「不建模调起」≠「丢能力」。
        DeclaredJsonNamesOf<RepaymentPrepayResponse>().Should().BeEquivalentTo(new[] { "prepay_id", "h5_url" });

        // 零声明②：支付成功回调通知是接收面（由 Mud.Wechat.Pay.Callback 承载），不得做成端点。
        routes.Should().NotContain(static r => r.Contains("notify", StringComparison.Ordinal),
            "回调是接收侧：把它声明成出站端点即造出「SDK 主动上报」的假契约");
        // 正对照：请求侧的 notify_url 字段确在（回调地址由宿主提供、由微信回调我方）。
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.NotifyUrl), "notify_url");

        // 零声明③：退款属同解决方案的另一章（官方侧栏「退款」），不复用直连退款面。
        routes.Should().NotContain(static r => r.Contains("refund", StringComparison.Ordinal),
            "还款退款是平台收付通的另一章，与直连 /v3/refund/domestic/ 不同契约 ⇒ 本域零端点");
        // 正对照：直连退款域在同口径扫描下确有路由（本域零声明不是扫描失效）。
        RoutesOf(typeof(IWechatPayRefundService))
            .Should().Contain(static r => r.StartsWith("/v3/refund/", StringComparison.Ordinal));

        // 零声明④：本域不是合单支付面（无 repayment 段的 combine-transactions 属 CombineTransactions 域）。
        RoutesOf(typeof(IWechatPayCombineService))
            .Should().Contain(static r => r.StartsWith("/v3/combine-transactions/", StringComparison.Ordinal)
                                          && !r.Contains("/repayment/", StringComparison.Ordinal),
            "正对照：合单支付面确有路由，两域互斥");
        routes.Intersect(RoutesOf(typeof(IWechatPayCombineService))).Should().BeEmpty("还款面与合单支付面路由不得重叠");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", "Repayment"))
            .Should().BeTrue("正对照：Interfaces/Repayment/ 确实已落位");
    }

    /// <summary>
    /// RP-B3：官方字段名逐项锁定（含 <c>h5_info</c> / <c>h5_url</c> 两个<b>数字打头段</b>名单）+
    /// 时间字段恒 <c>string</c>。
    /// </summary>
    [Fact]
    public void RepaymentDtos_ShouldExposeOfficialFieldNamesIncludingDigitBearingOnes()
    {
        // 合单标识（全部带 combine_ 前缀）+ 端型信息块名单。
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.CombineOutTradeNo), "combine_out_trade_no");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.CombineMchId), "combine_mchid");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.SubOrders), "sub_orders");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.H5Info), "h5_info");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.AppInfo), "app_info");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.MiniprogramInfo), "miniprogram_info");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.WechatWebInfo), "wechat_web_info");
        JsonNameShouldBe<RepaymentPrepayRequest>(nameof(RepaymentPrepayRequest.CombinePayerInfo), "combine_payer_info");
        JsonNameShouldBe<RepaymentPrepayResponse>(nameof(RepaymentPrepayResponse.H5Url), "h5_url");
        JsonNameShouldBe<RepaymentPrepayResponse>(nameof(RepaymentPrepayResponse.PrepayId), "prepay_id");

        // 子单：mchid 官方要求填 combine_mchid（语义反转，见接口 remarks）；金额与结算节字段名照原文。
        JsonNameShouldBe<RepaymentSubOrder>(nameof(RepaymentSubOrder.MchId), "mchid");
        JsonNameShouldBe<RepaymentSubOrder>(nameof(RepaymentSubOrder.SubMchId), "sub_mchid");
        JsonNameShouldBe<RepaymentSubOrder>(nameof(RepaymentSubOrder.OutTradeNo), "out_trade_no");
        JsonNameShouldBe<RepaymentSubOrder>(nameof(RepaymentSubOrder.SettleInfo), "settle_info");
        JsonNameShouldBe<RepaymentSubOrderAmount>(nameof(RepaymentSubOrderAmount.TotalAmount), "total_amount");
        JsonNameShouldBe<RepaymentSubOrderAmount>(nameof(RepaymentSubOrderAmount.Currency), "currency");
        JsonNameShouldBe<RepaymentSubOrderSettleInfo>(nameof(RepaymentSubOrderSettleInfo.ProfitSharing), "profit_sharing");
        JsonNameShouldBe<RepaymentSubOrderSettleInfo>(nameof(RepaymentSubOrderSettleInfo.SubsidyAmount), "subsidy_amount");

        // h5_info 内部字段（payer_client_ip 宽度 45、type 混合大小写值集见 RP-B4）。
        JsonNameShouldBe<RepaymentH5Info>(nameof(RepaymentH5Info.CombineAppId), "combine_appid");
        JsonNameShouldBe<RepaymentH5Info>(nameof(RepaymentH5Info.PayerClientIp), "payer_client_ip");
        JsonNameShouldBe<RepaymentH5Info>(nameof(RepaymentH5Info.AppUrl), "app_url");
        JsonNameShouldBe<RepaymentH5Info>(nameof(RepaymentH5Info.BundleId), "bundle_id");
        JsonNameShouldBe<RepaymentH5Info>(nameof(RepaymentH5Info.PackageName), "package_name");
        JsonNameShouldBe<RepaymentWechatWebInfo>(nameof(RepaymentWechatWebInfo.WebviewUrl), "webview_url");
        JsonNameShouldBe<RepaymentPayerIdentity>(nameof(RepaymentPayerIdentity.Number), "number");
        JsonNameShouldBe<RepaymentPayerIdentity>(nameof(RepaymentPayerIdentity.Name), "name");

        // 查单应答：合单四件套 + 子单状态与金额四件套。
        JsonNameShouldBe<RepaymentQueryResponse>(nameof(RepaymentQueryResponse.CombineTransactionId), "combine_transaction_id");
        JsonNameShouldBe<RepaymentQueryResponse>(nameof(RepaymentQueryResponse.CombineAppId), "combine_appid");
        JsonNameShouldBe<RepaymentQuerySubOrder>(nameof(RepaymentQuerySubOrder.TradeState), "trade_state");
        JsonNameShouldBe<RepaymentQuerySubOrder>(nameof(RepaymentQuerySubOrder.TradeType), "trade_type");
        JsonNameShouldBe<RepaymentQuerySubOrder>(nameof(RepaymentQuerySubOrder.BankType), "bank_type");
        JsonNameShouldBe<RepaymentQuerySubOrder>(nameof(RepaymentQuerySubOrder.SuccessTime), "success_time");
        JsonNameShouldBe<RepaymentQuerySubOrderAmount>(nameof(RepaymentQuerySubOrderAmount.PayerAmount), "payer_amount");
        JsonNameShouldBe<RepaymentQuerySubOrderAmount>(nameof(RepaymentQuerySubOrderAmount.PayerCurrency), "payer_currency");

        // 关单包体：只有 sub_orders，子单三字段与下单侧同名。
        DeclaredJsonNamesOf<RepaymentCloseRequest>().Should().BeEquivalentTo(new[] { "sub_orders" });
        JsonNameShouldBe<RepaymentCloseSubOrder>(nameof(RepaymentCloseSubOrder.MchId), "mchid");
        JsonNameShouldBe<RepaymentCloseSubOrder>(nameof(RepaymentCloseSubOrder.OutTradeNo), "out_trade_no");
        JsonNameShouldBe<RepaymentCloseSubOrder>(nameof(RepaymentCloseSubOrder.SubMchId), "sub_mchid");

        // 全域名单形态：官方名单全小写下划线，出现大写字母即被驼峰化（防「顺手纠正」）。
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
    /// RP-B4：官方取值集合逐项锁定（3 个常量类）+ 请求 / 应答「同名不同形」的四处类型分建。
    /// </summary>
    [Fact]
    public void RepaymentConstantsAndTypeSplits_ShouldMatchOfficialTables()
    {
        ValuesOf(typeof(RepaymentIdentityTypes)).Should().BeEquivalentTo(new[]
        {
            "IDCARD", "HONGKONG_MACAO", "HONGKONG_MACAO_RESIDENT", "TAIWAN_RESIDENT",
            "FOREIGN_RESIDENT", "OVERSEA_PASSPORT", "TAIWAN_COMPATRIOT_PERMIT",
        }, "官方 identity.type 七值（港澳台与外籍证件分开），不得跨域取别名");

        ValuesOf(typeof(RepaymentH5SceneTypes)).Should().BeEquivalentTo(new[] { "Wap", "iOS", "Android", "Harmony" },
            "官方 h5_info.type 是**混合大小写**（Wap / iOS / Android / Harmony），不得全大写或驼峰化");

        ValuesOf(typeof(RepaymentTradeStates)).Should().BeEquivalentTo(new[] { "SUCCESS", "NOTPAY", "CLOSED" },
            "本域子单状态只有三值：服务商基础支付是七值集合，判据不得复用");

        // 正对照：服务商基础支付面的状态集更宽（7 值），证明本域三值是收敛裁决而非漏项。
        ValuesOf(typeof(Mud.Wechat.Pay.DataModels.PartnerTransactions.PartnerTradeStates))
            .Should().Contain(new[] { "SUCCESS", "NOTPAY", "CLOSED" })
            .And.HaveCount(7);

        ConstantClassNames().Should().BeEquivalentTo(new[]
        {
            nameof(RepaymentIdentityTypes),
            nameof(RepaymentH5SceneTypes),
            nameof(RepaymentTradeStates),
        }, "本域常量类恰 3 个（值集差异的权威清单）");

        // 分建①：combine_payer_info 请求侧是实名信息（identity），应答侧只有 openid。
        DeclaredJsonNamesOf<RepaymentPayerInfo>().Should().BeEquivalentTo(new[] { "identity" });
        DeclaredJsonNamesOf<RepaymentQueryPayerInfo>().Should().BeEquivalentTo(new[] { "openid" });

        // 分建②：子单金额请求两字段、应答四字段（payer_amount / payer_currency 只在应答出现）。
        DeclaredJsonNamesOf<RepaymentSubOrderAmount>().Should().BeEquivalentTo(new[] { "total_amount", "currency" });
        DeclaredJsonNamesOf<RepaymentQuerySubOrderAmount>()
            .Should().BeEquivalentTo(new[] { "total_amount", "payer_amount", "currency", "payer_currency" });

        // 分建③：子单请求含 attach/description/settle_info/amount，应答含 transaction_id/trade_state/bank_type/success_time。
        DeclaredJsonNamesOf<RepaymentSubOrder>().Should()
            .Contain(new[] { "attach", "description", "settle_info", "amount" })
            .And.NotContain(new[] { "transaction_id", "trade_state", "success_time" });
        DeclaredJsonNamesOf<RepaymentQuerySubOrder>().Should()
            .Contain(new[] { "transaction_id", "trade_state", "bank_type", "success_time", "amount" })
            .And.NotContain(new[] { "description", "settle_info" });

        // 分建④：四支端型信息块字段面各不相同（app_info 只有 combine_appid）。
        DeclaredJsonNamesOf<RepaymentAppInfo>().Should().BeEquivalentTo(new[] { "combine_appid" });
        DeclaredJsonNamesOf<RepaymentMiniprogramInfo>().Should().BeEquivalentTo(new[] { "combine_appid", "openid" });
        DeclaredJsonNamesOf<RepaymentWechatWebInfo>()
            .Should().BeEquivalentTo(new[] { "combine_appid", "openid", "webview_url" });
        DeclaredJsonNamesOf<RepaymentH5Info>().Should().Contain(new[] { "combine_appid", "payer_client_ip", "type", "app_url" });
        FindMethod(nameof(IWechatPayRepaymentService.PrepayAsync)).GetParameters()
            .Select(static p => p.ParameterType)
            .Should().Contain(typeof(RepaymentPrepayRequest), "预下单只有单一包体参数");
    }

    /// <summary>
    /// RP-B5：判错面继承 —— 两支有包体的应答直接继承 <see cref="WechatPayResponse"/>，
    /// 204 的关单不伪造响应类型。
    /// </summary>
    [Fact]
    public void RepaymentResponses_ShouldDeriveFromWechatPayResponseAndNotFakeNoContentTypes()
    {
        foreach (var responseType in new[] { typeof(RepaymentPrepayResponse), typeof(RepaymentQueryResponse) })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse");
        }

        // 关单无应答 ⇒ 本域不得出现 CloseResponse 类型（伪造即造出永不填充的假字段）。
        DomainTypeNames().Should().NotContain("RepaymentCloseResponse",
            "本域不得为 204 端点伪造应答类型");
        DomainTypeNames().Should().NotContain(static n => n.StartsWith("RepaymentClose", StringComparison.Ordinal)
                                                         && n.EndsWith("Response", StringComparison.Ordinal));

        // 请求类型不得继承 WechatPayResponse（判错面只在应答侧）。
        foreach (var requestType in new[]
                 {
                     typeof(RepaymentPrepayRequest),
                     typeof(RepaymentCloseRequest),
                     typeof(RepaymentSubOrder),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求类型，不得挂在应答基类上");
        }
    }

    /// <summary>
    /// RP-B6：本域 17 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void RepaymentDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(17,
            "还款 DTO：下单文件 11（预下单请求 + 子单族 6 + 场景信息族 3 + 应答）+ 查询文件 6（查单应答族 4 + 关单请求族 2）= 17");

        foreach (var type in domainTypes)
        {
            RepaymentJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 RepaymentJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（本域类型 = 17 DTO + 3 常量类，上下文类已排除）。
        DomainTypeNames().Should().HaveCount(20, "本域类型 = 17 DTO + 3 常量类");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("RepaymentJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 Repayment 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddRepaymentWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayRepaymentService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayRepaymentService)
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
        => typeof(IWechatPayRepaymentService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayRepaymentService.{methodName} 必须存在");

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
        => typeof(RepaymentPrepayRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(RepaymentPrepayRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(RepaymentPrepayRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName)
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
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
