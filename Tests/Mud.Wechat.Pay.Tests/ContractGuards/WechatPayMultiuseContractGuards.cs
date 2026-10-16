// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Multiuse;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「Multiuse」契约守卫 —— 本批 <b>4 端点</b>（注册组 <c>Multiuse</c>，接口 <c>IWechatPayMultiuseService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Multiuse</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>4 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：4 / 4 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 4 支里<b>1 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>四支官方全部只印商户页</b>（「支持商户：【普通商户】」）⇒ 无服务商镜像；发放对象由 path 段 <c>openid</c> 承载， 请求体里没有 <c>sub_mchid</c> 相关键。</para>
/// <para><b>三支流水下载应答同形 ⇒ 共用一型</b>：发放 / 核销 / 退款明细（<c>send-flow</c> / <c>use-flow</c> / <c>refund-flow</c>） 官方应答表均为单键 <c>url</c> <c>string(2048)</c> 必填、query 均为 <c>begin_date</c> / <c>end_date</c> ⇒ 共用 <see cref="MultiuseFlowUrlResponse"/>；发券支应答单键 <c>coupon_id</c> 另建一型。</para>
/// <para><b>本域是行业方案里唯一逐支印限速与幂等口径的族</b>：四支官方正文均写「接口频率：不区分来源 <b>1000/min</b>」 +「幂等规则：接口<b>支持幂等重入</b>」⇒ 已逐支照抄；流水下载链接的有效期原文为 <b>30s</b>，拿到 <c>url</c> 须立即下载。</para>
/// <para><b>发券支是本域唯一涉密支</b>：官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>，且 <c>user_name</c> / <c>id_card_number</c> 类型为 <c>string(2048)</c> 密文 ⇒ 以 <c>string</c> 承载、SDK <b>不代为加解密</b>； 同域其余三支<b>未列</b>该头（GET 流水无需上送密文），涉密是<b>逐页</b>结论。</para>
/// <para><b>发券四条业务约束照页</b>：「商户<b>默认只允许发放本商户号</b>创建的消费金」「消费金<b>不涉及跨商户发券</b>」 「若发放消费金，则领取用户<b>必须为微信实名用户</b>，否则将发放失败」「该接口只能使用<b>创建活动方</b>进行调用」⇒ 幂等键是 <c>out_request_no</c>，业务码 <c>USER_NOT_EXIST</c>（实名信息错误）与 <c>NOT_ENOUGH</c>（预算/余额/单天限额）不可重试。</para>
/// <para><b>官方页引言跨页抄用 ⇒ 语义以标题 + 路由为准</b>：发放明细（<c>send-flow</c>）页首引言印的是 「可获取到某批次的<b>退款</b>明细数据」（与退款支同文）⇒ 不得据引言把三支并成一支。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayMultiuseContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Multiuse";

    private static readonly Type[] Services = { typeof(IWechatPayMultiuseService) };

    /// <summary>MUS-B1：拓扑 —— 注册组 Multiuse、本批 4 方法（接口实测 4）、4 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void MultiuseTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayMultiuseService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("Multiuse", "组名即源生成注册入口名（改组名即改 AddMultiuseWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(4, "Multiuse 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(4, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域流水下载三支全 GET、发券一支 POST ⇒ 发券不是 GET，三支 GET 的路径段（send/use/refund-flow）分列不并");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/multiuse/ 前缀内，与商家券 /v3/marketing/busifavor/、代金券 /v3/marketing/favor/ 零交集");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayMarketingFavorService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayMarketingFavorService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>MUS-B2：本批 4 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void MultiuseEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 下载批次发放明细｜官方页 v3/merchant/4012539756｜身份 【普通商户】
        RouteOf(nameof(IWechatPayMultiuseService.QueryMultiuseSendFlowAsync)).Should().Be("/v3/multiuse/stocks/{stockId}/send-flow");
        PathKeysOf(nameof(IWechatPayMultiuseService.QueryMultiuseSendFlowAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        QueryNamesOf(nameof(IWechatPayMultiuseService.QueryMultiuseSendFlowAsync)).Should().BeEquivalentTo(new[] { "begin_date", "end_date" });
        RequiredQueryOf(nameof(IWechatPayMultiuseService.QueryMultiuseSendFlowAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayMultiuseService.QueryMultiuseSendFlowAsync)).Should().BeNull("下载批次发放明细官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayMultiuseService.QueryMultiuseSendFlowAsync)).Should().Be(typeof(MultiuseFlowUrlResponse));

        // 下载批次核销明细｜官方页 v3/merchant/4012539758｜身份 【普通商户】
        RouteOf(nameof(IWechatPayMultiuseService.QueryMultiuseUseFlowAsync)).Should().Be("/v3/multiuse/stocks/{stockId}/use-flow");
        PathKeysOf(nameof(IWechatPayMultiuseService.QueryMultiuseUseFlowAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        QueryNamesOf(nameof(IWechatPayMultiuseService.QueryMultiuseUseFlowAsync)).Should().BeEquivalentTo(new[] { "begin_date", "end_date" });
        RequiredQueryOf(nameof(IWechatPayMultiuseService.QueryMultiuseUseFlowAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayMultiuseService.QueryMultiuseUseFlowAsync)).Should().BeNull("下载批次核销明细官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayMultiuseService.QueryMultiuseUseFlowAsync)).Should().Be(typeof(MultiuseFlowUrlResponse));

        // 下载批次退款明细｜官方页 v3/merchant/4012539762｜身份 【普通商户】
        RouteOf(nameof(IWechatPayMultiuseService.QueryMultiuseRefundFlowAsync)).Should().Be("/v3/multiuse/stocks/{stockId}/refund-flow");
        PathKeysOf(nameof(IWechatPayMultiuseService.QueryMultiuseRefundFlowAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        QueryNamesOf(nameof(IWechatPayMultiuseService.QueryMultiuseRefundFlowAsync)).Should().BeEquivalentTo(new[] { "begin_date", "end_date" });
        RequiredQueryOf(nameof(IWechatPayMultiuseService.QueryMultiuseRefundFlowAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayMultiuseService.QueryMultiuseRefundFlowAsync)).Should().BeNull("下载批次退款明细官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayMultiuseService.QueryMultiuseRefundFlowAsync)).Should().Be(typeof(MultiuseFlowUrlResponse));

        // 发放指定批次的消费金｜官方页 v3/merchant/4012539766｜身份 【普通商户】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPayMultiuseService.CreateMultiuseCouponAsync)).Should().Be("/v3/multiuse/users/{openid}/coupons");
        PathKeysOf(nameof(IWechatPayMultiuseService.CreateMultiuseCouponAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayMultiuseService.CreateMultiuseCouponAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayMultiuseService.CreateMultiuseCouponAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayMultiuseService.CreateMultiuseCouponAsync)).Should().Be(typeof(MultiuseCouponCreateRequest));
        ReturnTypeOf(nameof(IWechatPayMultiuseService.CreateMultiuseCouponAsync)).Should().Be(typeof(MultiuseCouponCreateResponse));
    }

    /// <summary>MUS-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void MultiuseDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<MultiuseCouponCreateRequest>().Should().BeEquivalentTo(NAMES["MultiuseCouponCreateRequest"], "MultiuseCouponCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<MultiuseFlowUrlResponse>().Should().BeEquivalentTo(NAMES["MultiuseFlowUrlResponse"], "MultiuseFlowUrlResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<MultiuseCouponCreateResponse>().Should().BeEquivalentTo(NAMES["MultiuseCouponCreateResponse"], "MultiuseCouponCreateResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>MUS-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void MultiuseDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(MultiuseFlowUrlResponse)).Should().BeTrue("MultiuseFlowUrlResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(MultiuseCouponCreateResponse)).Should().BeTrue("MultiuseCouponCreateResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(MultiuseCouponCreateRequest)).Should().BeFalse("MultiuseCouponCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>MUS-B5：本批 DTO 全量形态 —— 3 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void MultiuseAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(3, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            MultiuseJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 MultiuseJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("Multiuse", type.Name + " 的 SerializerClassName 必须为 Multiuse");
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

    /// <summary>MUS-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void MultiuseRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("Multiuse", "PayModule 须有 Multiuse 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMultiuseWebApiHttpClient()", "注册委托必须指向源生成方法（组名 Multiuse 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MultiuseJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["MultiuseCouponCreateRequest"] = new[] { "stock_id", "out_request_no", "user_name", "id_card_number", "amount", "appid", "card_type" },
        ["MultiuseFlowUrlResponse"] = new[] { "url" },
        ["MultiuseCouponCreateResponse"] = new[] { "coupon_id" },
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
