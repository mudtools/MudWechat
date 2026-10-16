// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.BrandApplyment;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-4 品牌线「品牌入驻申请」域契约守卫 —— <b>4 端点</b>：提交申请 + 撤销申请 + 按 <c>applyment_id</c> 查询 + 按 <c>business_code</c> 查询。
/// </summary>
/// <remarks>
/// <para><b>两套申请编号是两个定位键、不是一支的两个可选参数</b>：<c>GET …/applyment-id/{applyment_id}</c> 与<c>GET …/business-code/{business_code}</c> 是官方两条独立路由 ⇒ 两个方法，合成「一支带选填」即造出官方没有的端点（BRAP-B2）。</para>
/// <para><b>共用只发生在应答侧，且分两组</b>：受理两支（提交 / 撤销）应答 2 行同形 ⇒ 共用<c>BrandApplymentAcceptResponse</c>；查询两支应答 7 行同形 ⇒ 共用<c>BrandApplymentStateResponse</c>；受理组与查询组之间<b>不同形</b>（2 行 vs 7 行）⇒ 两型并存是本域契约，不得归一（BRAP-B3）。</para>
/// <para><b>本域路由不含 <c>/partner/</c> 段</b>，与会员卡 / 门店两族（<c>/v3/brand/partner/…</c>）拼写风格不同 ⇒ 前缀照页不得类推。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayBrandApplymentContractGuards
{
    private const string RegistryGroupName = "BrandApplyment";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BrandApplyment";

    private static readonly Type Service = typeof(IWechatPayBrandApplymentService);

    /// <summary>BRAP-B1：拓扑 —— 一个注册组一个接口、4 个方法、4 条 (方法, 路由)、动词集 POST / GET；路由前缀 /v3/brand/applyments。</summary>
    [Fact]
    public void BrandApplymentTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(4, "品牌入驻申请声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(4, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/brand/applyments", StringComparison.Ordinal),
            "本域路由全在 /v3/brand/applyments 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>BRAP-B2：4 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void BrandApplymentEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayBrandApplymentService.SubmitBrandApplymentAsync)).Should().Be("/v3/brand/applyments");
        // 品牌入驻申请提交｜官方页 partner/4016249989｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandApplymentService.CancelBrandApplymentAsync)).Should().Be("/v3/brand/applyments/cancel-applyment");
        // 品牌入驻申请撤销｜官方页 partner/4016257700｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandApplymentService.QueryBrandApplymentByApplymentIdAsync)).Should().Be("/v3/brand/applyments/applyment-id/{applymentId}");
        PathKeysOf(nameof(IWechatPayBrandApplymentService.QueryBrandApplymentByApplymentIdAsync)).Should().BeEquivalentTo(new[] { "applymentId" });
        // 品牌入驻申请状态｜官方页 partner/4016257685｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandApplymentService.QueryBrandApplymentByBusinessCodeAsync)).Should().Be("/v3/brand/applyments/business-code/{businessCode}");
        PathKeysOf(nameof(IWechatPayBrandApplymentService.QueryBrandApplymentByBusinessCodeAsync)).Should().BeEquivalentTo(new[] { "businessCode" });
        // 品牌入驻申请状态（按业务申请编号）｜官方页 partner/4016257694｜身份 普通服务商
    }

    /// <summary>BRAP-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void BrandApplymentDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BrandApplymentSubmitRequest>().Should().BeEquivalentTo(new[] { "business_code", "admin_info", "subject_info", "brand_basic_info", "trademark" },
            "品牌入驻申请提交请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandApplymentAcceptResponse>().Should().BeEquivalentTo(new[] { "applyment_id", "business_code" },
            "品牌入驻申请提交应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandApplymentCancelRequest>().Should().BeEquivalentTo(new[] { "business_code", "applyment_id" },
            "品牌入驻申请撤销请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandApplymentAcceptResponse>().Should().BeEquivalentTo(new[] { "applyment_id", "business_code" },
            "品牌入驻申请撤销应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandApplymentStateResponse>().Should().BeEquivalentTo(new[] { "applyment_id", "business_code", "applyment_state", "applyment_state_desc", "authorization_confirmation_qr_code", "reject_reason", "brand_id" },
            "品牌入驻申请状态应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandApplymentStateResponse>().Should().BeEquivalentTo(new[] { "applyment_id", "business_code", "applyment_state", "applyment_state_desc", "authorization_confirmation_qr_code", "reject_reason", "brand_id" },
            "品牌入驻申请状态（按业务申请编号）应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>BRAP-B4：判错面 + AOT 上下文登记 + DI 与模块接线（2 请求型 / 4 应答型）。</summary>
    [Fact]
    public void BrandApplymentDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(10, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            BrandApplymentJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BrandApplymentJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 BrandApplyment");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(BrandApplymentAcceptResponse), typeof(BrandApplymentAcceptResponse), typeof(BrandApplymentStateResponse), typeof(BrandApplymentStateResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(BrandApplymentSubmitRequest), typeof(BrandApplymentCancelRequest) })
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("BrandApplymentJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BrandApplyment 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBrandApplymentWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 BrandApplyment 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayBrandApplymentService.{methodName} 必须存在");

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
