// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.BrandRedpacket;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-A FundApp 族「品牌商户红包批次」域契约守卫 —— <b>5 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>两套定位键各建一支</b>：批次查询有「微信批次号 <c>batch_no</c>」与「商家批次号 <c>out_batch_no</c>」两页、明细查询有「<c>detail_no</c>」与「<c>out_detail_no</c>」两页，官方两两应答表<b>逐字段同形</b>（19 行 / 16 行）⇒ 各共用一个响应类型；请求侧两支皆零 Body。</para>
/// <para><b>查询支的 <c>need_query_detail</c> / <c>detail_state</c> 官方均为选填</b> ⇒ 声明式签名给可空缺省，不得做成必填；本域<b>无整型分页列表支</b>。</para>
/// <para><b><c>brand_id</c> 在本域是报文字段而非 query</b>，官方类型是 <c>integer</c>（品牌线各域为 <c>string(20)</c> / <c>string(32)</c>）⇒ 照页建成整型，不得按「品牌标识一律字符串」归一。</para>
/// <para><b>五支官方页只写「频率限制，会报错 <c>FREQUENCY_LIMITED</c>」未印 QPS 数字</b> ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayBrandRedpacketContractGuards
{
    private const string RegistryGroupName = "BrandRedpacket";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BrandRedpacket";

    private static readonly Type Service = typeof(IWechatPayBrandRedpacketService);

    /// <summary>BRP-B1：拓扑 —— 一个注册组一个接口、5 方法、5 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/fund-app/brand-redpacket。</summary>
    [Fact]
    public void BrandRedpacketTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(5, "品牌商户红包批次声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(5, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/fund-app/brand-redpacket", StringComparison.Ordinal),
            "本域路由全在 /v3/fund-app/brand-redpacket 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>BRP-B2：5 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void BrandRedpacketEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 品牌商户红包批次发放｜官方页 merchant/4014310358｜身份 【普通商户】｜官方页更新 2025.04.15
        RouteOf(nameof(IWechatPayBrandRedpacketService.CreateBrandRedpacketBatchAsync)).Should().Be("/v3/fund-app/brand-redpacket/brand-merchant-batches");
        QueryNamesOf(nameof(IWechatPayBrandRedpacketService.CreateBrandRedpacketBatchAsync)).Should().BeEmpty();

        // 品牌商户红包批次查询（微信批次号）｜官方页 merchant/4014310369｜身份 【普通商户】｜官方页更新 2025.04.15
        RouteOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByBatchNoAsync)).Should().Be("/v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}");
        PathKeysOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByBatchNoAsync)).Should().BeEquivalentTo(new[] { "batchNo" });
        QueryNamesOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByBatchNoAsync)).Should().BeEquivalentTo(new[] { "need_query_detail", "detail_state" });
        RequiredQueryOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByBatchNoAsync)).Should().BeEmpty("品牌商户红包批次查询（微信批次号）的 query 官方全为选填 ⇒ 声明式签名一律给可空缺省，不得做成必填");
        BodyTypeOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByBatchNoAsync)).Should().BeNull("品牌商户红包批次查询（微信批次号）官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 品牌商户红包明细查询（微信明细号）｜官方页 merchant/4014310384｜身份 【普通商户】｜官方页更新 2025.04.15
        RouteOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByDetailNoAsync)).Should().Be("/v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}/details/{detailNo}");
        PathKeysOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByDetailNoAsync)).Should().BeEquivalentTo(new[] { "batchNo", "detailNo" });
        QueryNamesOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByDetailNoAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByDetailNoAsync)).Should().BeNull("品牌商户红包明细查询（微信明细号）官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 品牌商户红包批次查询（商家批次号）｜官方页 merchant/4014310378｜身份 【普通商户】｜官方页更新 2025.04.15
        RouteOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByOutBatchNoAsync)).Should().Be("/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}");
        PathKeysOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByOutBatchNoAsync)).Should().BeEquivalentTo(new[] { "outBatchNo" });
        QueryNamesOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByOutBatchNoAsync)).Should().BeEquivalentTo(new[] { "need_query_detail", "detail_state" });
        RequiredQueryOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByOutBatchNoAsync)).Should().BeEmpty("品牌商户红包批次查询（商家批次号）的 query 官方全为选填 ⇒ 声明式签名一律给可空缺省，不得做成必填");
        BodyTypeOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketBatchByOutBatchNoAsync)).Should().BeNull("品牌商户红包批次查询（商家批次号）官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 品牌商户红包明细查询（商家明细号）｜官方页 merchant/4014310391｜身份 【普通商户】｜官方页更新 2025.04.15
        RouteOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByOutDetailNoAsync)).Should().Be("/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}/out-details/{outDetailNo}");
        PathKeysOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByOutDetailNoAsync)).Should().BeEquivalentTo(new[] { "outBatchNo", "outDetailNo" });
        QueryNamesOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByOutDetailNoAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayBrandRedpacketService.QueryBrandRedpacketDetailByOutDetailNoAsync)).Should().BeNull("品牌商户红包明细查询（商家明细号）官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>BRP-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void BrandRedpacketDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BrandRedpacketBatchCreateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "brand_appid", "scene", "template_id", "out_batch_no", "batch_name", "batch_remark", "total_amount", "total_num", "detail_list" },
            "品牌商户红包批次发放请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandRedpacketBatchCreateResponse>().Should().BeEquivalentTo(new[] { "out_batch_no", "batch_no", "create_time", "batch_state" },
            "品牌商户红包批次发放应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandRedpacketBatchQueryResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "batch_no", "out_batch_no", "brand_id", "template_id", "brand_appid", "batch_state", "batch_name", "batch_remark", "close_reason", "total_amount", "total_num", "create_time", "update_time", "success_amount", "success_num", "fail_amount", "fail_num", "detail_list" },
            "品牌商户红包批次查询（微信批次号）应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandRedpacketDetailQueryResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "out_batch_no", "batch_no", "out_detail_no", "detail_no", "detail_state", "amount", "remark", "fail_reason", "openid", "user_name", "initiate_time", "update_time", "brand_id", "template_id", "brand_appid" },
            "品牌商户红包明细查询（微信明细号）应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandRedpacketBatchQueryResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "batch_no", "out_batch_no", "brand_id", "template_id", "brand_appid", "batch_state", "batch_name", "batch_remark", "close_reason", "total_amount", "total_num", "create_time", "update_time", "success_amount", "success_num", "fail_amount", "fail_num", "detail_list" },
            "品牌商户红包批次查询（商家批次号）应答顶层字段名册（官方应答表逐字，与 <c>brand-redpacket/brand-merchant-batches/{batch_no}</c> 同表 ⇒ 共用一个类型）");

        DeclaredJsonNamesOf<BrandRedpacketDetailQueryResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "out_batch_no", "batch_no", "out_detail_no", "detail_no", "detail_state", "amount", "remark", "fail_reason", "openid", "user_name", "initiate_time", "update_time", "brand_id", "template_id", "brand_appid" },
            "品牌商户红包明细查询（商家明细号）应答顶层字段名册（官方应答表逐字，与 <c>brand-redpacket/brand-merchant-batches/{batch_no}/details/{detail_no}</c> 同表 ⇒ 共用一个类型）");
    }

    /// <summary>BRP-B4：判错面 + AOT 上下文登记 + DI 与模块接线（1 请求根型 / 3 应答根型 / 6 个 DTO）。</summary>
    [Fact]
    public void BrandRedpacketDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(6, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            BrandRedpacketJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BrandRedpacketJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 BrandRedpacket");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(BrandRedpacketBatchCreateResponse)).Should().BeTrue("BrandRedpacketBatchCreateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(BrandRedpacketBatchQueryResponse)).Should().BeTrue("BrandRedpacketBatchQueryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(BrandRedpacketDetailQueryResponse)).Should().BeTrue("BrandRedpacketDetailQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(BrandRedpacketBatchCreateRequest)).Should().BeFalse("BrandRedpacketBatchCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("BrandRedpacketJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BrandRedpacket 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBrandRedpacketWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 BrandRedpacket 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayBrandRedpacketService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
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
        => Service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
            .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
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

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string Root
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
