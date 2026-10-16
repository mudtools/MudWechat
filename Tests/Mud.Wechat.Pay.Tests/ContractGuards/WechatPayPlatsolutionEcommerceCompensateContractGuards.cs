// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-B 平台服务商解决方案「平台商户赔付」域契约守卫 —— <b>5 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>「预下单 / 下单 / 撤销 / 两路查询」四态各建一型请求</b>：预下单应答含 <c>package_info</c>（拉起用户确认用）， 撤销应答含 <c>cancel_accept_time</c> / <c>state</c> ⇒ 两支<b>不同形</b>，不得因同为 4 行而共用。</para>
/// <para><b>两支查询应答逐字段同形</b>（18 行）⇒ 按商户单号与按微信支付单号共用 <see cref="CompensateQueryResponse"/>。</para>
/// <para><b>撤销支官方无请求体</b>：定位键 <c>out_bill_no</c> 全在 path ⇒ 声明式签名不带 <c>[Body]</c>，不得凭空造请求 DTO。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayPlatsolutionEcommerceCompensateContractGuards
{
    private const string RegistryGroupName = "PlatsolutionEcommerceCompensate";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceCompensate";

    private static readonly Type Service = typeof(IWechatPayPlatsolutionEcommerceCompensateService);

    /// <summary>PEC-CP-B1：拓扑 —— 一个注册组一个接口、5 方法、5 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/platsolution/ecommerce/mch-transfer/compensate-bills。</summary>
    [Fact]
    public void PlatsolutionEcommerceCompensateTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(5, "平台商户赔付声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(5, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/platsolution/ecommerce/mch-transfer/compensate-bills", StringComparison.Ordinal),
            "本域路由全在 /v3/platsolution/ecommerce/mch-transfer/compensate-bills 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>PEC-CP-B2：5 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void PlatsolutionEcommerceCompensateEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 请求赔付｜官方页 partner/4013504175｜身份 【平台商户】｜官方页更新 2025.07.25
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.CreateCompensateAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/compensate-bills");
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.CreateCompensateAsync)).Should().BeEmpty();

        // 请求赔付预下单｜官方页 partner/4013504176｜身份 【平台商户】｜官方页更新 2025.07.25
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.PreTransferCompensateAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/pre-transfer");
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.PreTransferCompensateAsync)).Should().BeEmpty();

        // 请求撤销赔付｜官方页 partner/4013504177｜身份 【平台商户】｜官方页更新 2025.07.25
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.CancelCompensateAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}/cancel");
        PathKeysOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.CancelCompensateAsync)).Should().BeEquivalentTo(new[] { "outBillNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.CancelCompensateAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.CancelCompensateAsync)).Should().BeNull("请求撤销赔付官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 商户单号查询赔付结果｜官方页 partner/4013504178｜身份 【平台商户】｜官方页更新 2025.07.25
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByOutBillNoAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/out-bill-no/{outBillNo}");
        PathKeysOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByOutBillNoAsync)).Should().BeEquivalentTo(new[] { "outBillNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByOutBillNoAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByOutBillNoAsync)).Should().BeNull("商户单号查询赔付结果官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 微信支付单号查询赔付结果｜官方页 partner/4013504179｜身份 【平台商户】｜官方页更新 2025.07.25
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByBillIdAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/compensate-bills/bill-id/{billId}");
        PathKeysOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByBillIdAsync)).Should().BeEquivalentTo(new[] { "billId" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByBillIdAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayPlatsolutionEcommerceCompensateService.QueryCompensateByBillIdAsync)).Should().BeNull("微信支付单号查询赔付结果官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>PEC-CP-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void PlatsolutionEcommerceCompensateDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<CompensateCreateRequest>().Should().BeEquivalentTo(new[] { "receiver", "out_bill_no", "amount", "transfer_remark", "sponsor_mchid", "transfer_scene_id", "user_recv_perception" },
            "请求赔付请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<CompensatePreTransferRequest>().Should().BeEquivalentTo(new[] { "sp_appid", "receiver", "out_bill_no", "amount", "transfer_remark", "sponsor_mchid", "user_recv_perception", "notify_url" },
            "请求赔付预下单请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<CompensateCreateResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "receiver_detail", "out_bill_no", "amount", "transfer_remark", "bill_id", "state", "accept_time", "success_time", "sponsor_mchid" },
            "请求赔付应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<CompensatePreTransferResponse>().Should().BeEquivalentTo(new[] { "out_bill_no", "bill_id", "accept_time", "package_info" },
            "请求赔付预下单应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<CompensateCancelResponse>().Should().BeEquivalentTo(new[] { "out_bill_no", "bill_id", "state", "cancel_accept_time" },
            "请求撤销赔付应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<CompensateQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "receiver_detail", "out_bill_no", "amount", "transfer_remark", "bill_id", "state", "accept_time", "success_time", "close_info", "sponsor_mchid" },
            "商户单号查询赔付结果应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<CompensateQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "receiver_detail", "out_bill_no", "amount", "transfer_remark", "bill_id", "state", "accept_time", "success_time", "close_info", "sponsor_mchid" },
            "微信支付单号查询赔付结果应答顶层字段名册（官方应答表逐字，与 <c>mch-transfer/compensate-bills/out-bill-no/{out_bill_no}</c> 同表 ⇒ 共用一个类型）");
    }

    /// <summary>PEC-CP-B4：判错面 + AOT 上下文登记 + DI 与模块接线（2 请求根型 / 4 应答根型 / 17 个 DTO）。</summary>
    [Fact]
    public void PlatsolutionEcommerceCompensateDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(17, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            PlatsolutionEcommerceCompensateJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PlatsolutionEcommerceCompensateJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 PlatsolutionEcommerceCompensate");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(CompensateCreateResponse)).Should().BeTrue("CompensateCreateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(CompensatePreTransferResponse)).Should().BeTrue("CompensatePreTransferResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(CompensateCancelResponse)).Should().BeTrue("CompensateCancelResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(CompensateQueryResponse)).Should().BeTrue("CompensateQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(CompensateCreateRequest)).Should().BeFalse("CompensateCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(CompensatePreTransferRequest)).Should().BeFalse("CompensatePreTransferRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PlatsolutionEcommerceCompensateJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PlatsolutionEcommerceCompensate 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPlatsolutionEcommerceCompensateWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 PlatsolutionEcommerceCompensate 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayPlatsolutionEcommerceCompensateService.{methodName} 必须存在");

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
