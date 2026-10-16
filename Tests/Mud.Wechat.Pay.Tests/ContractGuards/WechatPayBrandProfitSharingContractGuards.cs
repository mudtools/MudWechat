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
using Mud.Wechat.Pay.DataModels.BrandProfitSharing;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-4 品牌线「品牌分账」域契约守卫 —— <b>9 端点</b>：最大比例 + 分账请求 / 结果查询 + 解冻 + 待分金额 + 接收方增删 + 回退请求 / 查询。
/// </summary>
/// <remarks>
/// <para><b>与 <c>ProfitSharing</c>（普通服务商分账）是同族不同路由</b>：本域前缀 <c>/v3/brand/profitsharing/</c>，口径是 <c>brand_mchid</c>（品牌主商户号）+ <c>sub_mchid</c>（收款方，可为门店商户号）两级 ⇒ 两域 DTO 与接收方类型<b>不得互套</b>，路由亦零交叠（BPS-B1）。</para>
/// <para><b>「发起 / 查询」两支在同域内分形不同</b>：分账发起应答 15 行、结果查询 16 行（多 <c>finish_amount</c> /<c>finish_description</c>，仅查询分账完结结果时存在）⇒ 分建；而回退两支（10 行）与接收方增删（3 行）各自同形 ⇒ 共用（BPS-B3）。</para>
/// <para><b>结果查询支三个 query 全必填</b>（<c>sub_mchid</c> / <c>transaction_id</c> / <c>out_order_no</c>），回退查询支只有 <c>sub_mchid</c> 与 <c>out_return_no</c> 必填、<c>order_id</c>/<c>out_order_no</c> 选填 ⇒ 两支定位口径不同（BPS-B2）。</para>
/// <para><b>接收方删除无响应业务字段差异</b>：官方增删两支应答同为 3 行 ⇒ 共用一型；「删除比添加返回更少」的直觉在本域不成立。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayBrandProfitSharingContractGuards
{
    private const string RegistryGroupName = "BrandProfitSharing";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BrandProfitSharing";

    private static readonly Type Service = typeof(IWechatPayBrandProfitSharingService);

    /// <summary>BPS-B1：拓扑 —— 一个注册组一个接口、9 个方法、9 条 (方法, 路由)、动词集 GET / POST；路由前缀 /v3/brand/profitsharing/。</summary>
    [Fact]
    public void BrandProfitSharingTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(9, "品牌分账声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(9, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/brand/profitsharing/", StringComparison.Ordinal),
            "本域路由全在 /v3/brand/profitsharing/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>BPS-B2：9 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void BrandProfitSharingEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingMaxRatioAsync)).Should().Be("/v3/brand/profitsharing/brand-configs/{brandMchid}");
        PathKeysOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingMaxRatioAsync)).Should().BeEquivalentTo(new[] { "brandMchid" });
        // 品牌最大分账比例｜官方页 partner/4012467022｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.FinishBrandProfitSharingOrderAsync)).Should().Be("/v3/brand/profitsharing/finish-order");
        // 品牌分账解冻剩余资金｜官方页 partner/4012467016｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.ApplyBrandProfitSharingOrderAsync)).Should().Be("/v3/brand/profitsharing/orders");
        // 品牌分账请求｜官方页 partner/4012692975｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingOrdersAsync)).Should().Be("/v3/brand/profitsharing/orders");
        QueryNamesOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingOrdersAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id", "out_order_no" });
        RequiredQueryOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingOrdersAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id", "out_order_no" }, "品牌分账结果查询的 sub_mchid / transaction_id / out_order_no 官方必填：给默认值即把 400 变成静默错参");
        // 品牌分账结果查询｜官方页 partner/4012467002｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingAmountsAsync)).Should().Be("/v3/brand/profitsharing/orders/{transactionId}/amounts");
        PathKeysOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingAmountsAsync)).Should().BeEquivalentTo(new[] { "transactionId" });
        // 订单剩余待分金额｜官方页 partner/4012467021｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.AddBrandProfitSharingReceiverAsync)).Should().Be("/v3/brand/profitsharing/receivers/add");
        // 品牌分账接收方添加｜官方页 partner/4012467100｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.DeleteBrandProfitSharingReceiverAsync)).Should().Be("/v3/brand/profitsharing/receivers/delete");
        // 品牌分账接收方删除｜官方页 partner/4012467103｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.ApplyBrandProfitSharingReturnOrderAsync)).Should().Be("/v3/brand/profitsharing/returnorders");
        // 品牌分账回退请求｜官方页 partner/4012467097｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingReturnOrderAsync)).Should().Be("/v3/brand/profitsharing/returnorders");
        QueryNamesOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingReturnOrderAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "out_return_no", "order_id", "out_order_no" });
        RequiredQueryOf(nameof(IWechatPayBrandProfitSharingService.QueryBrandProfitSharingReturnOrderAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "out_return_no" }, "品牌分账回退结果查询的 sub_mchid / out_return_no 官方必填：给默认值即把 400 变成静默错参");
        // 品牌分账回退结果查询｜官方页 partner/4012467011｜身份 普通服务商
    }

    /// <summary>BPS-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void BrandProfitSharingDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BrandProfitSharingMaxRatioResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "max_ratio" },
            "品牌最大分账比例应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingFinishRequest>().Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id", "out_order_no", "description" },
            "品牌分账解冻剩余资金请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandProfitSharingFinishResponse>().Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id", "out_order_no", "order_id" },
            "品牌分账解冻剩余资金应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingOrderRequest>().Should().BeEquivalentTo(new[] { "brand_mchid", "sub_mchid", "appid", "sub_appid", "transaction_id", "out_order_no", "receivers", "finish" },
            "品牌分账请求请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandProfitSharingOrderResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "sub_mchid", "transaction_id", "out_order_no", "order_id", "receivers", "status" },
            "品牌分账请求应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingOrderListResponse>().Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id", "out_order_no", "order_id", "status", "receivers", "finish_amount", "finish_description" },
            "品牌分账结果查询应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingAmountsResponse>().Should().BeEquivalentTo(new[] { "transaction_id", "unsplit_amount" },
            "订单剩余待分金额应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingReceiverAddRequest>().Should().BeEquivalentTo(new[] { "brand_mchid", "appid", "sub_appid", "type", "account", "name", "relation_type" },
            "品牌分账接收方添加请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandProfitSharingReceiverResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "type", "account" },
            "品牌分账接收方添加应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingReceiverDeleteRequest>().Should().BeEquivalentTo(new[] { "brand_mchid", "appid", "sub_appid", "type", "account" },
            "品牌分账接收方删除请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandProfitSharingReceiverResponse>().Should().BeEquivalentTo(new[] { "brand_mchid", "type", "account" },
            "品牌分账接收方删除应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingReturnOrderRequest>().Should().BeEquivalentTo(new[] { "sub_mchid", "order_id", "out_order_no", "out_return_no", "return_mchid", "amount", "description" },
            "品牌分账回退请求请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandProfitSharingReturnOrderResponse>().Should().BeEquivalentTo(new[] { "sub_mchid", "order_id", "out_order_no", "out_return_no", "return_mchid", "amount", "return_no", "result", "fail_reason", "finish_time" },
            "品牌分账回退请求应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandProfitSharingReturnOrderResponse>().Should().BeEquivalentTo(new[] { "sub_mchid", "order_id", "out_order_no", "out_return_no", "return_no", "return_mchid", "amount", "result", "fail_reason", "finish_time" },
            "品牌分账回退结果查询应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>BPS-B4：判错面 + AOT 上下文登记 + DI 与模块接线（5 请求型 / 9 应答型）。</summary>
    [Fact]
    public void BrandProfitSharingDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(15, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            BrandProfitSharingJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BrandProfitSharingJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 BrandProfitSharing");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(BrandProfitSharingMaxRatioResponse), typeof(BrandProfitSharingFinishResponse), typeof(BrandProfitSharingOrderResponse), typeof(BrandProfitSharingOrderListResponse), typeof(BrandProfitSharingAmountsResponse), typeof(BrandProfitSharingReceiverResponse), typeof(BrandProfitSharingReceiverResponse), typeof(BrandProfitSharingReturnOrderResponse), typeof(BrandProfitSharingReturnOrderResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(BrandProfitSharingFinishRequest), typeof(BrandProfitSharingOrderRequest), typeof(BrandProfitSharingReceiverAddRequest), typeof(BrandProfitSharingReceiverDeleteRequest), typeof(BrandProfitSharingReturnOrderRequest) })
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
        resolverSource.Should().Contain("BrandProfitSharingJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BrandProfitSharing 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBrandProfitSharingWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 BrandProfitSharing 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayBrandProfitSharingService.{methodName} 必须存在");

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
