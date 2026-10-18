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
using Mud.Wechat.Pay.DataModels.BrandStore;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-4 品牌线「品牌门店」域契约守卫 —— <b>9 端点</b>：门店创建 / 列表 / 查询 / 更新 / 删除 + 收款商户号绑定与解绑 + 暂停与恢复营业。
/// </summary>
/// <remarks>
/// <para><b>本域是支付线首个出现 <c>DELETE</c> 动词的域族</b>（守卫的动词集合断言含 <c>DeleteAttribute</c>）⇒「微信支付 v3 没有 DELETE」的旧口径在此失效；其余域的删除一律是 POST 动词段。</para>
/// <para><b>删除支官方未印发应表</b> ⇒ 返回裸 <see cref="Task"/>（以 HTTP 码 + <c>code</c>/<c>message</c> 判定），但仍带 <c>brand_id</c> 请求体 ⇒ 「DELETE 一定无包体」也不成立（BST-B2）。</para>
/// <para><b>五种变更彼此独立</b>：PATCH 改信息、DELETE 删记录、<c>/close</c> 与 <c>/resume</c> 改营业状态（应答 3 行同形 ⇒ 共用）、<c>/bindrecipient</c>（5 行）与 <c>/unbindrecipient</c>（1 行，只回 <c>failed_reason</c>）改收款绑定 ⇒ 分建（BST-B3）。</para>
/// <para><b>创建 / 查询 / 更新三支应答同形</b>（21 行）⇒ 共用<c>BrandStoreResponse</c>，但创建 Body（13 行）与更新 Body（13 行）<b>必填口径不同</b> ⇒ 请求侧仍各建。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayBrandStoreContractGuards
{
    private const string RegistryGroupName = "BrandStore";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BrandStore";

    private static readonly Type Service = typeof(IWechatPayBrandStoreService);

    /// <summary>BST-B1：拓扑 —— 一个注册组一个接口、9 个方法、9 条 (方法, 路由)、动词集 POST / GET / PATCH / DELETE；路由前缀 /v3/brand/partner/store/brandstores。</summary>
    [Fact]
    public void BrandStoreTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(9, "品牌门店声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(9, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute", "PatchAttribute", "DeleteAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/brand/partner/store/brandstores", StringComparison.Ordinal),
            "本域路由全在 /v3/brand/partner/store/brandstores 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>BST-B2：9 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void BrandStoreEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayBrandStoreService.CreateBrandStoreAsync)).Should().Be("/v3/brand/partner/store/brandstores");
        // 品牌门店创建｜官方页 partner/4015783183｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.QueryBrandStoresAsync)).Should().Be("/v3/brand/partner/store/brandstores");
        QueryNamesOf(nameof(IWechatPayBrandStoreService.QueryBrandStoresAsync)).Should().BeEquivalentTo(new[] { "brand_id", "store_state", "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayBrandStoreService.QueryBrandStoresAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "品牌门店列表的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 品牌门店列表｜官方页 partner/4016756674｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.QueryBrandStoreAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}");
        PathKeysOf(nameof(IWechatPayBrandStoreService.QueryBrandStoreAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        QueryNamesOf(nameof(IWechatPayBrandStoreService.QueryBrandStoreAsync)).Should().BeEquivalentTo(new[] { "brand_id" });
        RequiredQueryOf(nameof(IWechatPayBrandStoreService.QueryBrandStoreAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "品牌门店信息的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 品牌门店信息｜官方页 partner/4015783153｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.UpdateBrandStoreAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}");
        PathKeysOf(nameof(IWechatPayBrandStoreService.UpdateBrandStoreAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        // 品牌门店更新｜官方页 partner/4015783158｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.DeleteBrandStoreAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}");
        PathKeysOf(nameof(IWechatPayBrandStoreService.DeleteBrandStoreAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        // 品牌门店删除｜官方页 partner/4015783113｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.BindBrandStoreRecipientAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}/bindrecipient");
        PathKeysOf(nameof(IWechatPayBrandStoreService.BindBrandStoreRecipientAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        // 门店收款商户号绑定｜官方页 partner/4015783177｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.UnbindBrandStoreRecipientAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}/unbindrecipient");
        PathKeysOf(nameof(IWechatPayBrandStoreService.UnbindBrandStoreRecipientAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        // 门店收款商户号解绑｜官方页 partner/4015783188｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.CloseBrandStoreAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}/close");
        PathKeysOf(nameof(IWechatPayBrandStoreService.CloseBrandStoreAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        // 暂停门店营业｜官方页 partner/4016756671｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandStoreService.ResumeBrandStoreAsync)).Should().Be("/v3/brand/partner/store/brandstores/{storeId}/resume");
        PathKeysOf(nameof(IWechatPayBrandStoreService.ResumeBrandStoreAsync)).Should().BeEquivalentTo(new[] { "storeId" });
        // 恢复门店营业｜官方页 partner/4016756672｜身份 普通服务商
    }

    /// <summary>BST-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void BrandStoreDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BrandStoreCreateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "store_basics", "store_address", "store_business" },
            "品牌门店创建请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandStoreResponse>().Should().BeEquivalentTo(new[] { "brand_id", "store_id", "store_state", "audit_state", "review_reject_reason", "store_basics", "store_address", "store_business", "store_recipient" },
            "品牌门店创建应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreListResponse>().Should().BeEquivalentTo(new[] { "brand_id", "data", "offset", "limit", "total_count" },
            "品牌门店列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreResponse>().Should().BeEquivalentTo(new[] { "brand_id", "store_id", "store_state", "audit_state", "review_reject_reason", "store_basics", "store_address", "store_business", "store_recipient" },
            "品牌门店信息应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreUpdateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "store_basics", "store_address", "store_business" },
            "品牌门店更新请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandStoreResponse>().Should().BeEquivalentTo(new[] { "brand_id", "store_id", "store_state", "audit_state", "review_reject_reason", "store_basics", "store_address", "store_business", "store_recipient" },
            "品牌门店更新应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreDeleteRequest>().Should().BeEquivalentTo(new[] { "brand_id" },
            "品牌门店删除请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        ReturnTypeOf(nameof(IWechatPayBrandStoreService.DeleteBrandStoreAsync)).Should().Be(typeof(Task),
            "品牌门店删除官方应答表无业务字段 ⇒ 裸 Task（补空 DTO 即造不存在的契约）");

        DeclaredJsonNamesOf<BrandStoreBindRecipientRequest>().Should().BeEquivalentTo(new[] { "brand_id", "mchid", "company_name" },
            "门店收款商户号绑定请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandStoreBindRecipientResponse>().Should().BeEquivalentTo(new[] { "brand_id", "store_id", "mchid", "company_name", "recipient_state" },
            "门店收款商户号绑定应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreUnbindRecipientRequest>().Should().BeEquivalentTo(new[] { "brand_id", "mchid" },
            "门店收款商户号解绑请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandStoreUnbindRecipientResponse>().Should().BeEquivalentTo(new[] { "failed_reason" },
            "门店收款商户号解绑应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreCloseRequest>().Should().BeEquivalentTo(new[] { "brand_id" },
            "暂停门店营业请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandStoreStateResponse>().Should().BeEquivalentTo(new[] { "brand_id", "store_id", "store_state" },
            "暂停门店营业应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandStoreResumeRequest>().Should().BeEquivalentTo(new[] { "brand_id" },
            "恢复门店营业请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandStoreStateResponse>().Should().BeEquivalentTo(new[] { "brand_id", "store_id", "store_state" },
            "恢复门店营业应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>BST-B4：判错面 + AOT 上下文登记 + DI 与模块接线（7 请求型 / 8 应答型）。</summary>
    [Fact]
    public void BrandStoreDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(27, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            BrandStoreJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BrandStoreJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 BrandStore");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(BrandStoreResponse), typeof(BrandStoreListResponse), typeof(BrandStoreResponse), typeof(BrandStoreResponse), typeof(BrandStoreBindRecipientResponse), typeof(BrandStoreUnbindRecipientResponse), typeof(BrandStoreStateResponse), typeof(BrandStoreStateResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(BrandStoreCreateRequest), typeof(BrandStoreUpdateRequest), typeof(BrandStoreDeleteRequest), typeof(BrandStoreBindRecipientRequest), typeof(BrandStoreUnbindRecipientRequest), typeof(BrandStoreCloseRequest), typeof(BrandStoreResumeRequest) })
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
        resolverSource.Should().Contain("BrandStoreJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BrandStore 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBrandStoreWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 BrandStore 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayBrandStoreService.{methodName} 必须存在");

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
