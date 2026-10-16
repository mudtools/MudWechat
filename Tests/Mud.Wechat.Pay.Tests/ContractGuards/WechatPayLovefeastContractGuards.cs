// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Lovefeast;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「Lovefeast（爱心餐品牌信息）」契约守卫 —— 本批 <b>1 端点</b>（注册组 <c>Lovefeast</c>，接口 <c>IWechatPayLovefeastService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Lovefeast</c>）。
/// </summary>
/// <remarks>
/// <para><b>本域是取证勘误的产物（务必保留此段）</b>：原 P1-5 逐行处置表把爱心餐这支判为「非契约」，
/// 依据是「SKIT 清单里的出处 URL（老版 <c>docs/五、商家服务/爱心餐</c> 形态）重定向落到产品介绍总览页」。
/// <b>该判据无效</b>——旧路径失效与「现行文档中心有无该端点契约页」是两件事。按中文业务名「爱心餐」在现行文档树重检后
/// 证得该支<b>在册</b>（docId 4012472863）⇒ 转为真缺口落地，<b>不得</b>再退回非契约路由零声明名册。</para>
/// <para><b>逐端点契约</b>：方法 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支<b>官方只印一页</b>（【普通商户】）⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>官方爱心餐章节 API 列表恰此一支</b>：该章节其余节点是产品介绍 / 开发接入准备 / 接入指引 / 查券使用指引 ⇒ 本域只 1 方法不是「漏建」，
/// 而是官方 HTTP 面只有这一支。<b>不得</b>据第三方 SDK 或社区镜像补建官方未印的端点。</para>
/// <para><b>「爱心餐查券」不属本域 ⇒ 由既有 <c>MarketingFavor</c> 域承载</b>：官方《爱心餐查券接口使用指引》把查券能力指向
/// <c>GET /v3/marketing/favor/users/{openid}/coupons</c>（既存 <see cref="IWechatPayMarketingFavorService"/>，守卫 MF 系列）⇒
/// 本域<b>不得</b>重复建一支查券方法，也不得为该指引另造路由。</para>
/// <para><b>本域是支付线里「官方未印频率」的实例</b>：该页无「接口频率」条目、错误码表亦无 429 条目 ⇒ 守卫不锁数字，
/// 且<b>不得</b>凭第三方 SDK 补一个 QPS 进 remarks。</para>
/// <para><b>不涉密是逐页核对结论</b>：官方请求头表只列 <c>Authorization</c> / <c>Accept</c>，未列 <c>Wechatpay-Serial</c> ⇒ 本域无密文承载需求。</para>
/// <para><b>应答两键语义不对称 ⇒ 只一型</b>：<c>donate_count</c> 必填、<c>consume_count</c> 仅「供餐品牌」返回 ⇒
/// 普通品牌该键为 <c>null</c>，属性须可空且<b>不得</b>据其缺失判失败（B3 名册逐字照页）。</para>
/// <para><b>红线口径</b>：本域端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回强类型应答，无裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键与字段名册<b>由官方页逐字段生成</b>（现行文档中心逐页取证），
/// 改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayLovefeastContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Lovefeast";

    private static readonly Type[] Services = { typeof(IWechatPayLovefeastService) };

    /// <summary>LVF-B1：拓扑 —— 注册组 Lovefeast、本批 1 方法（接口实测 1）、1 条 (动词, 路由)；动词集只有 Get。</summary>
    [Fact]
    public void LovefeastTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayLovefeastService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("Lovefeast", "组名即源生成注册入口名（改组名即改 AddLovefeastWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(1, "Lovefeast 覆盖的接口声明式方法数（官方爱心餐章节 API 列表恰此一支）");
        DistinctVerbRoutes().Should().HaveCount(1, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute" }, "品牌信息查询官方请求方式为 GET ⇒ 出现任何写方法即偏离官方契约");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/lovefeast/", StringComparison.Ordinal),
            "本域路由在 /v3/lovefeast/ 前缀内，与商家券 /v3/marketing/busifavor/、代金券 /v3/marketing/favor/ 零交集");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayMarketingFavorService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayMarketingFavorService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>LVF-B2：本批 1 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void LovefeastEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 查询爱心餐品牌信息｜官方页 v3/merchant/4012472863｜身份 【普通商户】
        RouteOf(nameof(IWechatPayLovefeastService.QueryLovefeastBrandAsync)).Should().Be("/v3/lovefeast/brands/{brandId}");
        PathKeysOf(nameof(IWechatPayLovefeastService.QueryLovefeastBrandAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        QueryNamesOf(nameof(IWechatPayLovefeastService.QueryLovefeastBrandAsync)).Should().BeEquivalentTo(Array.Empty<string>(),
            "官方该支除 path 外零参数 ⇒ 加 query 即造官方没有的过滤面");
        RequiredQueryOf(nameof(IWechatPayLovefeastService.QueryLovefeastBrandAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        BodyTypeOf(nameof(IWechatPayLovefeastService.QueryLovefeastBrandAsync)).Should().BeNull("GET 支官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayLovefeastService.QueryLovefeastBrandAsync)).Should().Be(typeof(LovefeastBrandInfoResponse));
    }

    /// <summary>LVF-B3：官方字段表顶层名册（应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void LovefeastDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<LovefeastBrandInfoResponse>().Should().BeEquivalentTo(NAMES["LovefeastBrandInfoResponse"],
            "LovefeastBrandInfoResponse 的顶层字段名册必须与官方字段表逐字一致（本域官方应答无嵌套子树 ⇒ 只一型）");
    }

    /// <summary>LVF-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，本域零请求体 ⇒ 域内不得有第二型）。</summary>
    [Fact]
    public void LovefeastDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(LovefeastBrandInfoResponse))
            .Should().BeTrue("LovefeastBrandInfoResponse 必须承载官方 code/message（判错面）");

        DtoTypes().Should().ContainSingle(t => t == typeof(LovefeastBrandInfoResponse),
            "官方本域只有 GET 一支、应答无嵌套子树 ⇒ 出现第二型即凭空造 DTO");
    }

    /// <summary>LVF-B5：本批 DTO 全量形态 —— 1 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void LovefeastAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(1, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            LovefeastJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 LovefeastJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("Lovefeast", type.Name + " 的 SerializerClassName 必须为 Lovefeast");
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

    /// <summary>LVF-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void LovefeastRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("Lovefeast", "PayModule 须有 Lovefeast 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddLovefeastWebApiHttpClient()", "注册委托必须指向源生成方法（组名 Lovefeast 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("LovefeastJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由现行文档中心逐页取证产出）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["LovefeastBrandInfoResponse"] = new[] { "donate_count", "consume_count" },
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
