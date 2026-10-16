// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.FundsToOversea;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「FundsToOversea」契约守卫 —— 本批 <b>6 端点</b>（注册组 <c>FundsToOversea</c>，接口 <c>IWechatPayFundsToOverseaService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.FundsToOversea</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中<b>6 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 6 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>六支官方全部只印服务商页，且身份是【平台商户】</b>（不是【普通服务商】）⇒ 本域是收付通平台面能力， <b>不得</b>按服务商直连面补建商户页，也不得与其它「/v3/partner 树」域混认身份。</para>
/// <para><b>「受理 ≠ 出境成功」是官方原文</b>：「接口请求成功仅代表受理成功，如需知晓业务执行情况请<b>通过查询接口获取</b>」⇒ 下单支与退回支都不得据 HTTP 200 判定业务成功；共用应答里的 <c>result</c> / <c>state</c> 与 <c>fail_reason</c> 才是判据。</para>
/// <para><b>账单下载链接三条硬时效</b>：官方原文「下载地址的有效时间为 <b>30s</b>」、账单「出账日期为 T+1」（当天下载到的是前一天 购付汇<b>且出境成功</b>的单）、「一般在<b>上午 10 点</b>后可以下载到前一天的账单」，并要求 「将实际账单文件的哈希值和接口返回的哈希值进行<b>比对</b>以确认数据完整性」⇒ <c>hash_type</c> / <c>hash_value</c> 不是可选项； 另注「账单中涉及金额的字段单位为<b>元</b>」，与本线绝大多数「分」口径相反。</para>
/// <para><b>两处写读共用应答，各按官方应答表逐字段同形判定</b>：出境单 POST 与按 <c>out_order_id</c> 查询的应答表 <b>15 行同形且必填口径一致</b> ⇒ 共用 <see cref="FundsToOverseaOrderResponse"/>；退回单两支同为 <b>11 行同形</b> ⇒ 共用 <see cref="FundsToOverseaReturnOrderResponse"/>。账单下载支与在途可出境额度支各建一型，<b>不得</b>并入上述两型。</para>
/// <para><b>查询支的三个键不得省</b>：按 <c>out_order_id</c> 查出境单的官方必填 <b>query</b> 是 <c>sub_mchid</c> + <c>transaction_id</c>，而 path 段是商户出境单号 ⇒ 一支同时带「商户单号 + 微信单号 + 特约商户号」三种定位键， 照页不得只留 path 段。退回单查询支同理把 <c>sub_mchid</c> 列为必填 query 而 path 段是 <c>out_return_no</c>。</para>
/// <para><b>频率限制</b>：六支官方页<b>未印</b>端点级数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c>（下单支原文「资金出境限频」）； 六支请求头表均<b>未列</b> <c>Wechatpay-Serial</c> ⇒ 本域无涉密必填头。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayFundsToOverseaContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.FundsToOversea";

    private static readonly Type[] Services = { typeof(IWechatPayFundsToOverseaService) };

    /// <summary>FTO-B1：拓扑 —— 注册组 FundsToOversea、本批 6 方法（接口实测 6）、6 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void FundsToOverseaTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayFundsToOverseaService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("FundsToOversea", "组名即源生成注册入口名（改组名即改 AddFundsToOverseaWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(6, "FundsToOversea 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(6, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域出境下单与退回下单两支 POST、四支查询全 GET，无 PATCH/DELETE ⇒ 退回不是对出境单的 PATCH，而是独立集合 POST");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/funds-to-oversea/ 前缀内，退回支另落 /v3/funds-to-oversea/return/ 子段");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayBillService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayBillService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>FTO-B2：本批 6 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void FundsToOverseaEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 获取购付汇账单文件下载链接｜官方页 v3/partner/4012476132｜身份 【平台商户】
        RouteOf(nameof(IWechatPayFundsToOverseaService.GetFundsToOverseaBillDownloadUrlAsync)).Should().Be("/v3/funds-to-oversea/bill-download-url");
        PathKeysOf(nameof(IWechatPayFundsToOverseaService.GetFundsToOverseaBillDownloadUrlAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayFundsToOverseaService.GetFundsToOverseaBillDownloadUrlAsync)).Should().BeEquivalentTo(new[] { "bill_date", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFundsToOverseaService.GetFundsToOverseaBillDownloadUrlAsync)).Should().BeEquivalentTo(new[] { "bill_date" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFundsToOverseaService.GetFundsToOverseaBillDownloadUrlAsync)).Should().BeNull("获取购付汇账单文件下载链接官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFundsToOverseaService.GetFundsToOverseaBillDownloadUrlAsync)).Should().Be(typeof(FundsToOverseaBillDownloadUrlResponse));

        // 申请资金出境｜官方页 v3/partner/4012476113｜身份 【平台商户】
        RouteOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaOrderAsync)).Should().Be("/v3/funds-to-oversea/orders");
        PathKeysOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaOrderAsync)).Should().Be(typeof(FundsToOverseaOrderCreateRequest));
        ReturnTypeOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaOrderAsync)).Should().Be(typeof(FundsToOverseaOrderResponse));

        // 查询出境结果｜官方页 v3/partner/4012476127｜身份 【平台商户】
        RouteOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaOrderAsync)).Should().Be("/v3/funds-to-oversea/orders/{outOrderId}");
        PathKeysOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaOrderAsync)).Should().BeEquivalentTo(new[] { "outOrderId" });
        QueryNamesOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaOrderAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id" });
        RequiredQueryOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaOrderAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaOrderAsync)).Should().BeNull("查询出境结果官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaOrderAsync)).Should().Be(typeof(FundsToOverseaOrderResponse));

        // 申请资金出境退回｜官方页 v3/partner/4013735039｜身份 【平台商户】
        RouteOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaReturnOrderAsync)).Should().Be("/v3/funds-to-oversea/return/return-orders");
        PathKeysOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaReturnOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaReturnOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaReturnOrderAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaReturnOrderAsync)).Should().Be(typeof(FundsToOverseaReturnOrderCreateRequest));
        ReturnTypeOf(nameof(IWechatPayFundsToOverseaService.CreateFundsToOverseaReturnOrderAsync)).Should().Be(typeof(FundsToOverseaReturnOrderResponse));

        // 查询出境退回结果｜官方页 v3/partner/4013735052｜身份 【平台商户】
        RouteOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaReturnOrderAsync)).Should().Be("/v3/funds-to-oversea/return/return-orders/out-return-no/{outReturnNo}");
        PathKeysOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaReturnOrderAsync)).Should().BeEquivalentTo(new[] { "outReturnNo" });
        QueryNamesOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaReturnOrderAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaReturnOrderAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaReturnOrderAsync)).Should().BeNull("查询出境退回结果官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaReturnOrderAsync)).Should().Be(typeof(FundsToOverseaReturnOrderResponse));

        // 查询订单剩余可出境余额｜官方页 v3/partner/4012476109｜身份 【平台商户】
        RouteOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaAvailableAbroadAmountAsync)).Should().Be("/v3/funds-to-oversea/transactions/{transactionId}/available_abroad_amounts");
        PathKeysOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaAvailableAbroadAmountAsync)).Should().BeEquivalentTo(new[] { "transactionId" });
        QueryNamesOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaAvailableAbroadAmountAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaAvailableAbroadAmountAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaAvailableAbroadAmountAsync)).Should().BeNull("查询订单剩余可出境余额官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFundsToOverseaService.QueryFundsToOverseaAvailableAbroadAmountAsync)).Should().Be(typeof(FundsToOverseaTransactionAvailableAbroadAmountResponse));
    }

    /// <summary>FTO-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void FundsToOverseaDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<FundsToOverseaOrderCreateRequest>().Should().BeEquivalentTo(NAMES["FundsToOverseaOrderCreateRequest"], "FundsToOverseaOrderCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<FundsToOverseaReturnOrderCreateRequest>().Should().BeEquivalentTo(NAMES["FundsToOverseaReturnOrderCreateRequest"], "FundsToOverseaReturnOrderCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<FundsToOverseaBillDownloadUrlResponse>().Should().BeEquivalentTo(NAMES["FundsToOverseaBillDownloadUrlResponse"], "FundsToOverseaBillDownloadUrlResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<FundsToOverseaOrderResponse>().Should().BeEquivalentTo(NAMES["FundsToOverseaOrderResponse"], "FundsToOverseaOrderResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<FundsToOverseaReturnOrderResponse>().Should().BeEquivalentTo(NAMES["FundsToOverseaReturnOrderResponse"], "FundsToOverseaReturnOrderResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<FundsToOverseaTransactionAvailableAbroadAmountResponse>().Should().BeEquivalentTo(NAMES["FundsToOverseaTransactionAvailableAbroadAmountResponse"], "FundsToOverseaTransactionAvailableAbroadAmountResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>FTO-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void FundsToOverseaDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FundsToOverseaBillDownloadUrlResponse)).Should().BeTrue("FundsToOverseaBillDownloadUrlResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FundsToOverseaOrderResponse)).Should().BeTrue("FundsToOverseaOrderResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FundsToOverseaReturnOrderResponse)).Should().BeTrue("FundsToOverseaReturnOrderResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FundsToOverseaTransactionAvailableAbroadAmountResponse)).Should().BeTrue("FundsToOverseaTransactionAvailableAbroadAmountResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(FundsToOverseaOrderCreateRequest)).Should().BeFalse("FundsToOverseaOrderCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FundsToOverseaReturnOrderCreateRequest)).Should().BeFalse("FundsToOverseaReturnOrderCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>FTO-B5：本批 DTO 全量形态 —— 11 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void FundsToOverseaAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(11, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            FundsToOverseaJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 FundsToOverseaJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("FundsToOversea", type.Name + " 的 SerializerClassName 必须为 FundsToOversea");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 名册与 B3 的期望值同源：每个根型的全部名单都必须出现在 NAMES 表里（防「B3 漏列某型」）。
        foreach (var type in batchDtos.Where(t => NAMES.ContainsKey(t.Name)))
            DeclaredJsonNamesOf(type).Should().BeEquivalentTo(NAMES[type.Name], type.Name + " 的名单与 B3 期望值必须同源");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>FTO-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void FundsToOverseaRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("FundsToOversea", "PayModule 须有 FundsToOversea 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddFundsToOverseaWebApiHttpClient()", "注册委托必须指向源生成方法（组名 FundsToOversea 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("FundsToOverseaJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["FundsToOverseaOrderCreateRequest"] = new[] { "out_order_id", "sub_mchid", "transaction_id", "amount", "foreign_currency", "goods_info", "seller_info", "express_info", "payee_info", "presale_info" },
        ["FundsToOverseaReturnOrderCreateRequest"] = new[] { "out_return_no", "sub_mchid", "out_order_id", "transaction_id", "supplement_info", "amount" },
        ["FundsToOverseaBillDownloadUrlResponse"] = new[] { "hash_type", "hash_value", "download_url" },
        ["FundsToOverseaOrderResponse"] = new[] { "out_order_id", "sub_mchid", "order_id", "result", "fail_reason", "amount", "foreign_amount", "foreign_currency", "rate", "exchange_rate_time", "estimate_exchange_rate_time", "departure_amount", "fee", "charge_mchid", "charge_account_type" },
        ["FundsToOverseaReturnOrderResponse"] = new[] { "out_return_no", "sub_mchid", "return_id", "out_order_id", "transaction_id", "supplement_info", "amount", "state", "create_time", "success_time", "fail_reason" },
        ["FundsToOverseaTransactionAvailableAbroadAmountResponse"] = new[] { "transaction_id", "available_abroad_amount" },
    };

    // ---- helpers -------------------------------------------------------------

    private static object? ApiProp(Type service, string name)
    {
        var api = service.GetCustomAttributes(false).Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        return api.GetType().GetProperty(name)!.GetValue(api);
    }

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException(methodName + " 必须存在于本域接口");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull(methodName + " 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Services.SelectMany(static s => s.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .ToArray();

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant() + " " + a.RequestUri))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string[] DeclaredJsonNamesOf(Type type) => JsonNamesOf(type);

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(RepoRootDir, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                               && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string RepoRootDir
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
