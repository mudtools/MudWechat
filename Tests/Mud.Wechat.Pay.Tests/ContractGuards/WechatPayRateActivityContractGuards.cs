// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.RateActivity;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「RateActivity」契约守卫 —— 本批 <b>3 端点</b>（注册组 <c>RateActivity</c>，接口 <c>IWechatPayRateActivityService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.RateActivity</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 3 支中<b>3 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 3 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>三支官方全部只印服务商页，身份是【从业机构（银行）】+【从业机构（支付机构）】双标签</b> （正文「支持商户：【从业机构（银行）】 【从业机构（支付机构）】」）⇒ 两类从业机构共用契约， <b>不得</b>按银行/支付机构拆接口或补商户页。</para>
/// <para><b>报名与修改的应答同形 ⇒ 共用一型</b>：POST <c>/rate-activity/applications</c> 与 PATCH <c>/rate-activity/applications/id/{application_id}</c> 官方应答表均为单键 <c>application_id</c> <c>string(32)</c> 必填 ⇒ 共用 <see cref="RateActivityApplicationResponse"/>；GET 查状态支应答为 <c>application_state</c> / <c>sub_mchid</c> / <c>reject_parameter</c> / <c>reject_reason</c> / <c>pass_time</c> 五键 ⇒ <b>单建一型</b>。</para>
/// <para><b><c>sub_mchid</c> 两侧类型不同</b>：请求侧是 <c>string(32)</c>，<b>应答</b>侧官方印 <c>integer</c> ⇒ 「同名即同型」的直觉在本域会造成反序列化失败，请求与应答类型分别照页。</para>
/// <para><b>修改支的请求体全为选填</b>：<c>activity_detail_modification</c> 与 <c>additional_information</c> 官方均标选填， 「至少要给一个」由业务码 <c>INVALID_REQUEST</c>（参数符合格式但不符合业务规则）兜 ⇒ SDK <b>不做</b>空体拦截， 照页保持两支皆可空。</para>
/// <para><b>重复报名是业务码不是 HTTP 冲突</b>：官方码原文「<c>ALREADY_EXISTS</c>：商户提交过报名申请」（HTTP 400）⇒ 重试前先查状态支；<c>NO_AUTH</c>「商户信息不合法」与前置「已完成<b>开户意愿确认</b>的商户」同页印发。</para>
/// <para><b>频率限制</b>：三支官方页<b>全部未印</b>端点级数字，只在错误码表给 <c>429 FREQUENCY_LIMITED</c> ⇒ 不得补数。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayRateActivityContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.RateActivity";

    private static readonly Type[] Services = { typeof(IWechatPayRateActivityService) };

    /// <summary>RTA-B1：拓扑 —— 注册组 RateActivity、本批 3 方法（接口实测 3）、3 条 (动词, 路由)；动词集 Get / Patch / Post。</summary>
    [Fact]
    public void RateActivityTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayRateActivityService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("RateActivity", "组名即源生成注册入口名（改组名即改 AddRateActivityWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(3, "RateActivity 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(3, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PatchAttribute", "PostAttribute" }, "本域报名 POST、修改 PATCH（官方原文 PATCH，不是 PUT）、查状态 GET ⇒ 三支动词各异，缺一即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/rate-activity/applications 前缀内，修改与查状态另带 /id/ 段");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayRefundService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayRefundService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>RTA-B2：本批 3 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void RateActivityEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 报名费率优惠活动｜官方页 v3/partner/4012698774｜身份 【从业机构（银行
        RouteOf(nameof(IWechatPayRateActivityService.CreateRateActivityApplicationAsync)).Should().Be("/v3/rate-activity/applications");
        PathKeysOf(nameof(IWechatPayRateActivityService.CreateRateActivityApplicationAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayRateActivityService.CreateRateActivityApplicationAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayRateActivityService.CreateRateActivityApplicationAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayRateActivityService.CreateRateActivityApplicationAsync)).Should().Be(typeof(RateActivityApplicationCreateRequest));
        ReturnTypeOf(nameof(IWechatPayRateActivityService.CreateRateActivityApplicationAsync)).Should().Be(typeof(RateActivityApplicationResponse));

        // 修改单据信息｜官方页 v3/partner/4012698885｜身份 【从业机构（银行
        RouteOf(nameof(IWechatPayRateActivityService.UpdateRateActivityApplicationAsync)).Should().Be("/v3/rate-activity/applications/id/{applicationId}");
        PathKeysOf(nameof(IWechatPayRateActivityService.UpdateRateActivityApplicationAsync)).Should().BeEquivalentTo(new[] { "applicationId" });
        QueryNamesOf(nameof(IWechatPayRateActivityService.UpdateRateActivityApplicationAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayRateActivityService.UpdateRateActivityApplicationAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayRateActivityService.UpdateRateActivityApplicationAsync)).Should().Be(typeof(RateActivityApplicationUpdateRequest));
        ReturnTypeOf(nameof(IWechatPayRateActivityService.UpdateRateActivityApplicationAsync)).Should().Be(typeof(RateActivityApplicationResponse));

        // 根据申请单ID查询申请单结果｜官方页 v3/partner/4012471722｜身份 【从业机构（银行
        RouteOf(nameof(IWechatPayRateActivityService.QueryRateActivityApplicationStateAsync)).Should().Be("/v3/rate-activity/applications/id/{applicationId}");
        PathKeysOf(nameof(IWechatPayRateActivityService.QueryRateActivityApplicationStateAsync)).Should().BeEquivalentTo(new[] { "applicationId" });
        QueryNamesOf(nameof(IWechatPayRateActivityService.QueryRateActivityApplicationStateAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayRateActivityService.QueryRateActivityApplicationStateAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayRateActivityService.QueryRateActivityApplicationStateAsync)).Should().BeNull("根据申请单ID查询申请单结果官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayRateActivityService.QueryRateActivityApplicationStateAsync)).Should().Be(typeof(RateActivityApplicationStateResponse));
    }

    /// <summary>RTA-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void RateActivityDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<RateActivityApplicationCreateRequest>().Should().BeEquivalentTo(NAMES["RateActivityApplicationCreateRequest"], "RateActivityApplicationCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<RateActivityApplicationUpdateRequest>().Should().BeEquivalentTo(NAMES["RateActivityApplicationUpdateRequest"], "RateActivityApplicationUpdateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<RateActivityApplicationResponse>().Should().BeEquivalentTo(NAMES["RateActivityApplicationResponse"], "RateActivityApplicationResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<RateActivityApplicationStateResponse>().Should().BeEquivalentTo(NAMES["RateActivityApplicationStateResponse"], "RateActivityApplicationStateResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>RTA-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void RateActivityDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(RateActivityApplicationResponse)).Should().BeTrue("RateActivityApplicationResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(RateActivityApplicationStateResponse)).Should().BeTrue("RateActivityApplicationStateResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(RateActivityApplicationCreateRequest)).Should().BeFalse("RateActivityApplicationCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(RateActivityApplicationUpdateRequest)).Should().BeFalse("RateActivityApplicationUpdateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>RTA-B5：本批 DTO 全量形态 —— 10 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void RateActivityAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(10, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            RateActivityJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 RateActivityJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("RateActivity", type.Name + " 的 SerializerClassName 必须为 RateActivity");
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

    /// <summary>RTA-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void RateActivityRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("RateActivity", "PayModule 须有 RateActivity 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddRateActivityWebApiHttpClient()", "注册委托必须指向源生成方法（组名 RateActivity 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("RateActivityJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["RateActivityApplicationCreateRequest"] = new[] { "acquiring_bank_id", "channel_id", "sub_mchid", "activity_detail", "additional_information" },
        ["RateActivityApplicationUpdateRequest"] = new[] { "activity_detail_modification", "additional_information" },
        ["RateActivityApplicationResponse"] = new[] { "application_id" },
        ["RateActivityApplicationStateResponse"] = new[] { "application_state", "sub_mchid", "reject_parameter", "reject_reason", "pass_time" },
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
