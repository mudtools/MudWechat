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
using Mud.Wechat.Pay.DataModels.MarketingGoodsSubsidyActivity;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-3 营销批次「零售小店补贴活动」域契约守卫 —— <b>8 端点</b>：representative 三支 + 素材一支 + 门店四支。
/// </summary>
/// <remarks>
/// <para><b>两支 DELETE 带 JSON 请求体</b>：删除 representative 与删除门店都要提交 <c>out_request_no</c> + <c>delete_time</c> + 明细数组 ⇒ 「DELETE 一定无包体」在本域不成立，声明式方法必须同时带 <c>[Body]</c>（MGSA-B1）。</para>
/// <para><b>path 段名一分为二</b>：<c>retail-store-act/{activity_id}/representative(s)</c> 用活动号，<c>retail-store-act/{brand_id}/…</c> 用品牌号 ⇒ 同为删除但定位键不同，不得类推（MGSA-B2）。</para>
/// <para><b>幂等键 <c>out_request_no</c> 只在两支删除与素材上传出现</b>；门店添加/查询无幂等面（重复添加由官方判重）。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayMarketingGoodsSubsidyActivityContractGuards
{
    private const string RegistryGroupName = "MarketingGoodsSubsidyActivity";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingGoodsSubsidyActivity";

    private static readonly Type Service = typeof(IWechatPayMarketingGoodsSubsidyActivityService);

    /// <summary>MGSA-B1：拓扑 —— 一个注册组一个接口、8 个方法、8 条 (方法, 路由)、动词集 PUT / DELETE / GET / POST；路由前缀 /v3/marketing/goods-subsidy-activity/。</summary>
    [Fact]
    public void GoodsSubsidyTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(8, "商品补贴活动（零售小店）声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(8, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PutAttribute", "DeleteAttribute", "GetAttribute", "PostAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/marketing/goods-subsidy-activity/", StringComparison.Ordinal),
            "本域路由全在 /v3/marketing/goods-subsidy-activity/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>MGSA-B2：8 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void GoodsSubsidyEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.SetRetailStoreRepresentativeAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.SetRetailStoreRepresentativeAsync)).Should().BeEquivalentTo(new[] { "activityId" });
        // 设置活动门店 representative｜官方页 partner/4012384386｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.DeleteRetailStoreRepresentativeAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representative");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.DeleteRetailStoreRepresentativeAsync)).Should().BeEquivalentTo(new[] { "activityId" });
        // 删除活动门店 representative｜官方页 partner/4012384399｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreRepresentativesAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{activityId}/representatives");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreRepresentativesAsync)).Should().BeEquivalentTo(new[] { "activityId" });
        QueryNamesOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreRepresentativesAsync)).Should().BeEquivalentTo(new[] { "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreRepresentativesAsync)).Should().BeEquivalentTo(new[] { "offset", "limit" }, "查询活动门店 representative 列表的 offset / limit 官方必填：给默认值即把 400 变成静默错参");
        // 查询活动门店 representative 列表｜官方页 partner/4012384405｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.UploadRetailStoreMaterialsAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/materials");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.UploadRetailStoreMaterialsAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        // 上传小店活动素材｜官方页 partner/4012384395｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.AddRetailStoreStoresAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.AddRetailStoreStoresAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        // 添加小店活动门店｜官方页 partner/4012384421｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.DeleteRetailStoreStoresAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.DeleteRetailStoreStoresAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        // 删除小店活动门店｜官方页 partner/4012384431｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreStoresAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreStoresAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        QueryNamesOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreStoresAsync)).Should().BeEquivalentTo(new[] { "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreStoresAsync)).Should().BeEquivalentTo(new[] { "offset", "limit" }, "查询小店活动门店列表的 offset / limit 官方必填：给默认值即把 400 变成静默错参");
        // 查询小店活动门店列表｜官方页 partner/4012384414｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreStoreAsync)).Should().Be("/v3/marketing/goods-subsidy-activity/retail-store-act/{brandId}/stores/{storeCode}");
        PathKeysOf(nameof(IWechatPayMarketingGoodsSubsidyActivityService.QueryRetailStoreStoreAsync)).Should().BeEquivalentTo(new[] { "brandId", "storeCode" });
        // 查询小店活动门店详情｜官方页 partner/4012384437｜身份 服务商
    }

    /// <summary>MGSA-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void GoodsSubsidyDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<RetailStoreActRepresentativeSetRequest>().Should().BeEquivalentTo(new[] { "representative_info_list", "out_request_no", "add_time" },
            "设置活动门店 representative请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<RetailStoreActRepresentativeSetResponse>().Should().BeEquivalentTo(new[] { "activity_id", "failed_representative_info_list", "add_time" },
            "设置活动门店 representative应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActRepresentativeDeleteRequest>().Should().BeEquivalentTo(new[] { "representative_info_list", "out_request_no", "delete_time" },
            "删除活动门店 representative请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<RetailStoreActRepresentativeDeleteResponse>().Should().BeEquivalentTo(new[] { "activity_id", "failed_representative_info_list", "delete_time" },
            "删除活动门店 representative应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActRepresentativeListResponse>().Should().BeEquivalentTo(new[] { "total_count", "data", "offset", "limit" },
            "查询活动门店 representative 列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActMaterialUploadRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "material_num" },
            "上传小店活动素材请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<RetailStoreActMaterialUploadResponse>().Should().BeEquivalentTo(new[] { "material_list" },
            "上传小店活动素材应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActStoreAddRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "add_time", "stores" },
            "添加小店活动门店请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<RetailStoreActStoreAddResponse>().Should().BeEquivalentTo(new[] { "add_time", "failed_stores" },
            "添加小店活动门店应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActStoreDeleteRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "delete_time", "stores" },
            "删除小店活动门店请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<RetailStoreActStoreDeleteResponse>().Should().BeEquivalentTo(new[] { "delete_time", "failed_stores" },
            "删除小店活动门店应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActStoreListResponse>().Should().BeEquivalentTo(new[] { "total_count", "data", "offset", "limit" },
            "查询小店活动门店列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<RetailStoreActStoreQueryResponse>().Should().BeEquivalentTo(new[] { "store_code", "store_name" },
            "查询小店活动门店详情应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>MGSA-B4：判错面 + AOT 上下文登记 + DI 与模块接线（5 请求型 / 8 应答型）。</summary>
    [Fact]
    public void GoodsSubsidyDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(24, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            MarketingGoodsSubsidyActivityJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingGoodsSubsidyActivityJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MarketingGoodsSubsidyActivity");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(RetailStoreActRepresentativeSetResponse), typeof(RetailStoreActRepresentativeDeleteResponse), typeof(RetailStoreActRepresentativeListResponse), typeof(RetailStoreActMaterialUploadResponse), typeof(RetailStoreActStoreAddResponse), typeof(RetailStoreActStoreDeleteResponse), typeof(RetailStoreActStoreListResponse), typeof(RetailStoreActStoreQueryResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(RetailStoreActRepresentativeSetRequest), typeof(RetailStoreActRepresentativeDeleteRequest), typeof(RetailStoreActMaterialUploadRequest), typeof(RetailStoreActStoreAddRequest), typeof(RetailStoreActStoreDeleteRequest) })
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
        resolverSource.Should().Contain("MarketingGoodsSubsidyActivityJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MarketingGoodsSubsidyActivity 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMarketingGoodsSubsidyActivityWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 MarketingGoodsSubsidyActivity 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMarketingGoodsSubsidyActivityService.{methodName} 必须存在");

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
