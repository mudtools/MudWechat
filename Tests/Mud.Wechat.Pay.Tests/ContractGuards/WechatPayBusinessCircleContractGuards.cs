// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.BusinessCircle;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「BusinessCircle」契约守卫 —— 本批 <b>4 端点</b>（注册组 <c>BusinessCircle</c>，接口 <c>IWechatPayBusinessCircleService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.BusinessCircle</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>0 支官方只印一页</b>、<b>4 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：1 / 4 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>无应答体支的返回形态</b>：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>四支各印商户页 + 服务商页 ⇒ 并集建模</b>：停车状态同步与积分同步两支两页请求表<b>逐字同形</b>； 「查授权状态」支服务商页另印<b>选填</b> <c>sub_mchid</c>（商户页根本没有该键）⇒ 一支一方法、身份差异写在参数注释里， <b>不得</b>按身份拆成两支方法或复制成两个 DTO。</para>
/// <para><b>两支是 204 无应答包体</b>：停车状态同步（POST parkings）与积分同步（POST points/notify）官方正常示例 <b>204 No Content</b> ⇒ 返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>积分同步支是「场内退款通知」的硬前置</b>：官方接入顺序原文「商圈会员积分同步（<b>只有接入该接口，才会获取到退款信息</b>）」， 且退款监控窗口是「对已同步过积分的会员消费，监控 <b>30 天内</b>的退款情况」⇒ 未回传积分就没有退款回调； 同支业务码「该订单积分已回传」「该订单已超过回传时效」也印证回传是一次性、有时效的。</para>
/// <para><b>本域唯一印端点级数字的是授权状态查询支</b>：官方原文「接口频率：150QPS」+「前置条件：商圈商户/服务商已开通支付即积分产品功能」； 其余三支官方<b>未印</b>数字 ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>正文有加密提示、请求头表却没列 <c>Wechatpay-Serial</c></b>：四支正文均写「商户上送敏感信息时需要使用微信支付公钥加密（推荐）」， 但四支的官方请求头表只有 <c>Authorization</c> / <c>Accept</c>（+ 写侧的 <c>Content-Type</c>）⇒ 按「涉密口径取请求头表」的判据， 本域判为<b>无涉密必填头</b>，与智慧导购、多次扣费发券支的逐页结论<b>不同</b>，不得据正文提示补头。</para>
/// <para><b>定位键三套并存、长度随页变</b>：<c>brandid</c> 官方类型是 <c>integer</c>（不是字符串），<c>sub_mchid</c> 在停车支为 <c>string(32)</c>、在积分与授权两支为 <c>string(64)</c>，<c>openid</c> 为 <c>string(64)</c> ⇒ 照页保留长度差异， 「服务商面一律 sub_mchid(32)」式归一会造出官方不存在的报文。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>2 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayBusinessCircleContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BusinessCircle";

    private static readonly Type[] Services = { typeof(IWechatPayBusinessCircleService) };

    /// <summary>BSC-B1：拓扑 —— 注册组 BusinessCircle、本批 4 方法（接口实测 4）、4 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void BusinessCircleTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayBusinessCircleService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("BusinessCircle", "组名即源生成注册入口名（改组名即改 AddBusinessCircleWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(4, "BusinessCircle 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(4, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域写支（停车状态同步、积分同步）全 POST、读支（授权状态、待积分状态）全 GET，无 PATCH/DELETE ⇒ 两支写侧的 204 与两支读侧的应答不得互挂");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/businesscircle/ 前缀内，与品牌卡 /v3/brand/ 会员面零交集");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayTransactionsService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayTransactionsService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>BSC-B2：本批 4 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void BusinessCircleEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 商圈会员停车状态同步｜官方页 v3/merchant/4012535502 + v3/partner/4012474127｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCircleParkingAsync)).Should().Be("/v3/businesscircle/parkings");
        PathKeysOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCircleParkingAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCircleParkingAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCircleParkingAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCircleParkingAsync)).Should().Be(typeof(BusinessCircleParkingSyncRequest));
        ReturnTypeOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCircleParkingAsync)).Should().Be(typeof(Task), "商圈会员停车状态同步官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 商圈会员积分同步｜官方页 v3/merchant/4012534698 + v3/partner/4012474133｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCirclePointsAsync)).Should().Be("/v3/businesscircle/points/notify");
        PathKeysOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCirclePointsAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCirclePointsAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCirclePointsAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCirclePointsAsync)).Should().Be(typeof(BusinessCirclePointsSyncRequest));
        ReturnTypeOf(nameof(IWechatPayBusinessCircleService.SyncBusinessCirclePointsAsync)).Should().Be(typeof(Task), "商圈会员积分同步官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 商圈会员积分服务授权查询｜官方页 v3/merchant/4012534848 + v3/partner/4012474135｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCircleUserAuthorizationAsync)).Should().Be("/v3/businesscircle/user-authorizations/{openid}");
        PathKeysOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCircleUserAuthorizationAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCircleUserAuthorizationAsync)).Should().BeEquivalentTo(new[] { "appid", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCircleUserAuthorizationAsync)).Should().BeEquivalentTo(new[] { "appid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCircleUserAuthorizationAsync)).Should().BeNull("商圈会员积分服务授权查询官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCircleUserAuthorizationAsync)).Should().Be(typeof(BusinessCircleUserAuthorizationInfoResponse));

        // 商圈会员待积分状态查询｜官方页 v3/merchant/4012534994 + v3/partner/4012474129｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCirclePointsCommitStateAsync)).Should().Be("/v3/businesscircle/users/{openid}/points/commit_status");
        PathKeysOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCirclePointsCommitStateAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCirclePointsCommitStateAsync)).Should().BeEquivalentTo(new[] { "brandid", "appid", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCirclePointsCommitStateAsync)).Should().BeEquivalentTo(new[] { "brandid", "appid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCirclePointsCommitStateAsync)).Should().BeNull("商圈会员待积分状态查询官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayBusinessCircleService.QueryBusinessCirclePointsCommitStateAsync)).Should().Be(typeof(BusinessCirclePointsCommitStateResponse));
    }

    /// <summary>BSC-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void BusinessCircleDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BusinessCircleParkingSyncRequest>().Should().BeEquivalentTo(NAMES["BusinessCircleParkingSyncRequest"], "BusinessCircleParkingSyncRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusinessCirclePointsSyncRequest>().Should().BeEquivalentTo(NAMES["BusinessCirclePointsSyncRequest"], "BusinessCirclePointsSyncRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusinessCircleUserAuthorizationInfoResponse>().Should().BeEquivalentTo(NAMES["BusinessCircleUserAuthorizationInfoResponse"], "BusinessCircleUserAuthorizationInfoResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusinessCirclePointsCommitStateResponse>().Should().BeEquivalentTo(NAMES["BusinessCirclePointsCommitStateResponse"], "BusinessCirclePointsCommitStateResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>BSC-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void BusinessCircleDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(BusinessCircleUserAuthorizationInfoResponse)).Should().BeTrue("BusinessCircleUserAuthorizationInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(BusinessCirclePointsCommitStateResponse)).Should().BeTrue("BusinessCirclePointsCommitStateResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(BusinessCircleParkingSyncRequest)).Should().BeFalse("BusinessCircleParkingSyncRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(BusinessCirclePointsSyncRequest)).Should().BeFalse("BusinessCirclePointsSyncRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>BSC-B5：本批 DTO 全量形态 —— 4 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void BusinessCircleAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(4, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            BusinessCircleJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 BusinessCircleJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("BusinessCircle", type.Name + " 的 SerializerClassName 必须为 BusinessCircle");
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

    /// <summary>BSC-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void BusinessCircleRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("BusinessCircle", "PayModule 须有 BusinessCircle 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBusinessCircleWebApiHttpClient()", "注册委托必须指向源生成方法（组名 BusinessCircle 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("BusinessCircleJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["BusinessCircleParkingSyncRequest"] = new[] { "sub_mchid", "brandid", "appid", "openid", "plate_number", "state", "time" },
        ["BusinessCirclePointsSyncRequest"] = new[] { "sub_mchid", "transaction_id", "appid", "openid", "earn_points", "increased_points", "points_update_time", "no_points_remarks", "total_points" },
        ["BusinessCircleUserAuthorizationInfoResponse"] = new[] { "openid", "authorize_state", "authorize_time", "deauthorize_time" },
        ["BusinessCirclePointsCommitStateResponse"] = new[] { "points_commit_status" },
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
