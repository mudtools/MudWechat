// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.Abstractions.Extensions;

namespace Mud.Wechat.Ads.Tests.Extensions;

/// <summary>
/// 广告线装配面测试：<c>AddAdsApp</c>（凭据底座）+ <c>AddWechatAdsApi</c>（业务接口）的三段式出口。
/// </summary>
/// <remarks>
/// <para>
/// 三段式 = <see cref="AdsModule"/> 枚举值 + <c>Add{域}Api()</c> + 源生成器产出的
/// <c>Add{域}WebApiHttpClient()</c>。第三段没有签入源文件，只有真装配才能证明它被挂上（AGENTS §4）。
/// </para>
/// <para>
/// <b>本线特有的两条附加不变式</b>：① <c>IAdsAuthorizationService</c> 与 <c>IAdsAccessTokenProvider</c>
/// 必须是<b>同一实例</b> —— 每 AppKey 的单飞闸挂在服务实例字段上，各自解析一份等于把并发刷新放大一倍
/// （ADS-B3）；② <c>AddWechatAdsApi</c> 必须在 <c>AddAdsApp</c> 之后，否则命名客户端不存在。
/// </para>
/// </remarks>
public class AdsServiceCollectionExtensionsTests
{
    private const string ClientId = "12345678";
    private const string ClientSecret = "test-client-secret";

    /// <summary>业务接口在根容器与作用域内都必须可解析（AGENTS §7 的 DI 门禁形态）。</summary>
    [Fact]
    public void AddWechatAdsApi_ShouldRegisterAdvertiserClient_ResolvableInRootAndScope()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(AdsModule.Advertiser));

        provider.GetRequiredService<IWechatAdsAdvertiserService>().Should().NotBeNull();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatAdsAdvertiserService>().Should().NotBeNull();
    }

    /// <summary>
    /// 三段式第三段：<c>AddAllApis()</c> 后每个已落地域都必须可解析 ——
    /// 漏挂 <c>Add{域}WebApiHttpClient()</c> 时枚举与建造者都看不出问题，只有真装配能抓到。
    /// </summary>
    /// <remarks>
    /// 枚举成员集合被钉死：新增 <see cref="AdsModule"/> 值即在断言 1 处变红，
    /// 逼着改动者同批补「该域接口可解析」的断言（否则第三段漏挂仍是绿的）。
    /// </remarks>
    [Fact]
    public void AddWechatAdsApi_AddAllApis_ShouldRegisterEveryLandedModule()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(b => b.AddAllApis()));

        Enum.GetValues(typeof(AdsModule)).Cast<AdsModule>()
            .Should().Equal(new[]
                {
                    AdsModule.Advertiser, AdsModule.Adgroups, AdsModule.Reports,
                    AdsModule.DynamicCreatives, AdsModule.Components, AdsModule.Images,
                    AdsModule.Videos, AdsModule.AsyncTasks,
                },
                "新增 AdsModule 成员必须同批在本用例补该域接口的可解析断言");

        provider.GetRequiredService<IWechatAdsAdvertiserService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsAdgroupService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsReportService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsDynamicCreativeService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsComponentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsImageService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsVideoService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsAsyncTaskService>().Should().NotBeNull();
        // 素材两域的 multipart 上传通道随模块装配（实现随 Images/Videos 的 Add{域}Api 挂载）。
        provider.GetRequiredService<IWechatAdsImageUploadService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatAdsVideoUploadService>().Should().NotBeNull();
    }

    /// <summary>
    /// reports 域单独注册也必须可解析。本域的第三段 <c>AddReportsWebApiHttpClient()</c> 同样是
    /// 生成器产出、无签入源文件，而它的注册组名（<c>Reports</c>）<b>一支跨三支资源族</b>
    /// （<c>daily_reports</c> / <c>hourly_reports</c> / <c>async_reports</c>）⇒ 只装上一个描述符、
    /// 漏挂另两支时接口照样可解析但请求期报「未注册的路由」，只有真装配 + 路由表守卫能同时抓到。
    /// </summary>
    [Fact]
    public void AddWechatAdsApi_ShouldRegisterReportClient_ResolvableInRootAndScope()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(AdsModule.Reports));

        provider.GetRequiredService<IWechatAdsReportService>().Should().NotBeNull();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatAdsReportService>().Should().NotBeNull();
    }

    /// <summary><c>AddReportsApi()</c> 与枚举重载 <c>AddWechatAdsApi(AdsModule.Reports)</c> 装配面等价。</summary>
    [Fact]
    public void AddWechatAdsApi_ShouldSupportReportsApiShortcut()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(b => b.AddReportsApi()));

        provider.GetRequiredService<IWechatAdsReportService>().Should().NotBeNull();
    }

    /// <summary>
    /// adgroups 域单独注册也必须可解析（<c>AddAdgroupsApi()</c> 走的是生成器产出的
    /// <c>AddAdgroupsWebApiHttpClient()</c>，无签入源文件 ⇒ 只有真装配能证明挂上了）。
    /// </summary>
    [Fact]
    public void AddWechatAdsApi_ShouldRegisterAdgroupClient_ResolvableInRootAndScope()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(AdsModule.Adgroups));

        provider.GetRequiredService<IWechatAdsAdgroupService>().Should().NotBeNull();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatAdsAdgroupService>().Should().NotBeNull();
    }

    /// <summary>漏装凭据底座必须在<b>注册期</b> fail-fast 并点名 <c>AddAdsApp</c>，而不是等首次真实请求。</summary>
    [Fact]
    public void AddWechatAdsApi_ShouldFailFast_WhenAddAdsAppMissing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatAdsApi(AdsModule.Advertiser);

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddAdsApp*");
    }

    /// <summary>零模块注册是「什么都没装却以为装了」，必须拒绝。</summary>
    [Fact]
    public void AddWechatAdsApi_ShouldFailFast_WhenNoModuleSelected()
    {
        var services = new ServiceCollection();
        services.AddAdsApp(CreateApps());

        var act = () => services.AddWechatAdsApi();

        act.Should().Throw<InvalidOperationException>().WithMessage("*至少需要添加一个模块*");
    }

    /// <summary>建造者委托重载与枚举重载装配出等价的可解析面。</summary>
    [Fact]
    public void AddWechatAdsApi_ShouldSupportConfigureOverload_WhenUsingBuilderAction()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(b => b.AddAdvertiserApi().AddAllApis()));

        provider.GetRequiredService<IWechatAdsAdvertiserService>().Should().NotBeNull();
    }

    /// <summary>
    /// <b>ADS-B3 的装配前提</b>：取令牌端口与授权服务必须解析到<b>同一实例</b>（单飞闸是实例字段）。
    /// </summary>
    [Fact]
    public void AddAdsApp_ShouldWireAccessTokenProviderToSameAuthorizationServiceInstance()
    {
        using var provider = BuildProvider(services => services.AddAdsApp(CreateApps()));

        var authorization = provider.GetRequiredService<IAdsAuthorizationService>();
        provider.GetRequiredService<IAdsAccessTokenProvider>().Should().BeSameAs(authorization,
            "各解析一份会让「取令牌」与「显式刷新」用两把进程闸，并发刷新照旧打架");
    }

    /// <summary>底座端口在装配后可解析，且跨 scope 为同一实例（Singleton 语义，AGENTS §7）。</summary>
    [Fact]
    public void AddAdsApp_ShouldRegisterSingletonPorts()
    {
        using var provider = BuildProvider(services => services.AddAdsApp(CreateApps()));

        foreach (var serviceType in new[]
                 {
                     typeof(IAdsAppManager),
                     typeof(IAdsAppContextSwitcher),
                     typeof(IWechatAdsAuthorizationStore),
                     typeof(IAdsClock),
                     typeof(IAdsHttpClient),
                     typeof(IAdsOAuthHttpClient),
                 })
        {
            var root = provider.GetRequiredService(serviceType);
            root.Should().NotBeNull($"{serviceType.Name} 必须可解析");

            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService(serviceType).Should().BeSameAs(root,
                $"{serviceType.Name} 必须是 Singleton（授权状态与命名客户端都是跨请求态）");
        }
    }

    /// <summary>
    /// OAuth 客户端<b>不挂</b>令牌注入 Handler（官方 <c>oauth/token</c> 页：OAuth 接口无需全局参数），
    /// 业务客户端<b>恰好挂一次</b> —— Handler 被挂两次会让同一次请求写入两份 <c>access_token</c>，
    /// 而官方网关取哪一支未定义。这里的落点是「描述符只注册一次 + 只有业务客户端调用
    /// <c>AddHttpMessageHandler</c>」，行为面由 <c>AdsAuthorizationHandlerTests</c> 与
    /// 守卫 ADS-B1/B2 共同承担。
    /// </summary>
    [Fact]
    public void AddAdsApp_ShouldRegisterAuthorizationHandlerOnceAndBothNamedClients()
    {
        var services = new ServiceCollection();
        services.AddAdsApp(CreateApps());

        services.Count(static d => d.ServiceType == typeof(AdsAuthorizationHandler))
            .Should().Be(1, "Handler 重复注册即在一次请求里追加两组全局参数");

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        // 两个命名客户端各占一个 DI 类型键（每线一对而非每应用一对）。
        factory.CreateClient(AdsHttpClientNames.ClientName).BaseAddress!.AbsoluteUri
            .Should().Be("https://api.e.qq.com/");
        factory.CreateClient(AdsHttpClientNames.OAuthClientName).BaseAddress!.AbsoluteUri
            .Should().Be("https://api.e.qq.com/");
    }

    /// <summary>重复 <c>AddAdsApp</c> 会让后一次覆盖前一次的注册表（先注册的应用静默丢失）⇒ 注册期拒。</summary>
    [Fact]
    public void AddAdsApp_ShouldFailFast_WhenCalledTwice()
    {
        var services = new ServiceCollection();
        services.AddAdsApp(CreateApps());

        var act = () => services.AddAdsApp(CreateApps("other"));

        act.Should().Throw<InvalidOperationException>().WithMessage("*已被调用过*");
    }

    /// <summary>配置节名写错时 <c>Bind</c> 得到空列表且<b>不报错</b> ⇒ 必须在装配期点名，而不是拖到首次取令牌。</summary>
    [Fact]
    public void AddAdsApp_ShouldFailFast_WhenConfigListIsEmpty()
    {
        var services = new ServiceCollection();

        var act = () => services.AddAdsApp(new List<AdsAppConfig>());

        act.Should().Throw<InvalidOperationException>().WithMessage("*没有收到任何应用配置*");
    }

    /// <summary>AppKey 重复会让命名客户端与令牌槽位互相覆盖 ⇒ 注册期拒（公用层多应用同一口径）。</summary>
    [Fact]
    public void AddAdsApp_ShouldFailFast_WhenAppKeyDuplicated()
    {
        var services = new ServiceCollection();

        var act = () => services.AddAdsApp(new List<AdsAppConfig>
        {
            new() { AppKey = "a", ClientId = ClientId, ClientSecret = ClientSecret },
            new() { AppKey = "a", ClientId = "87654321", ClientSecret = ClientSecret },
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*重复的 AppKey*");
    }

    /// <summary>非数字 <c>client_id</c> 属配置错误，必须在启动期点名（官方类型为 integer）。</summary>
    [Fact]
    public void AddAdsApp_ShouldFailFast_WhenClientIdIsNotNumeric()
    {
        var services = new ServiceCollection();

        var act = () => services.AddAdsApp(new List<AdsAppConfig>
        {
            new() { AppKey = "a", ClientId = "abc-123", ClientSecret = ClientSecret },
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*纯数字*");
    }

    /// <summary>多应用且无默认标记时不得「随机取第一个」⇒ 注册期抛并给出两种修复方式。</summary>
    [Fact]
    public void AddAdsApp_ShouldFailFast_WhenMultipleAppsWithoutDefault()
    {
        var services = new ServiceCollection();

        var act = () => services.AddAdsApp(new List<AdsAppConfig>
        {
            new() { AppKey = "a", ClientId = ClientId, ClientSecret = ClientSecret },
            new() { AppKey = "b", ClientId = "87654321", ClientSecret = ClientSecret },
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*没有默认应用*");
    }

    /// <summary>
    /// <b>AOT 生死线</b>：只调 <c>AddAdsApp</c>（不装任何业务模块）也必须能解析 OAuth 报文类型 ——
    /// 换码/刷新是手写传输，其 DTO 落在 DataModels，若上下文登记挂在业务注册入口，这条最小路径会在
    /// Native AOT 下静默失败。
    /// </summary>
    [Fact]
    public void AddAdsApp_ShouldRegisterOAuthAndCommonJsonContexts_WithoutAnyBusinessModule()
    {
        using var provider = BuildProvider(services => services.AddAdsApp(CreateApps()));

        var resolver = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value.TypeInfoResolver;
        resolver.Should().NotBeNull("漏登记即意味着 Native AOT 下广告 DTO 无元数据（JIT 靠反射侥幸可用）");

        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        foreach (var dto in new[]
                 {
                     typeof(AdsTokenResponse),
                     typeof(AdsTokenData),
                     typeof(AdsAuthorizerInfo),
                     typeof(AdsBatchResultItem),
                     typeof(AdsPageInfo),
                     typeof(AdsFiltering),
                 })
        {
            resolver!.GetTypeInfo(dto, options).Should().NotBeNull(
                $"{dto.Name} 必须被源生成上下文覆盖，否则 Native AOT 下无元数据");
        }
    }

    /// <summary>业务域的上下文同样在 <c>AddAdsApp</c> 内登记（登记点是全量，不是逐模块）。</summary>
    [Fact]
    public void AddAdsApp_ShouldRegisterAdvertiserJsonContexts()
    {
        using var provider = BuildProvider(services => services.AddAdsApp(CreateApps()));

        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var resolver = options.TypeInfoResolver;

        foreach (var dto in new[]
                 {
                     typeof(AdsAdvertiserGetResponse),
                     typeof(AdsAdvertiserInfo),
                     typeof(AdsAdvertiserUpdateRequest),
                     typeof(AdsAdvertiserUpdateDailyBudgetResponse),
                     typeof(AdsAdvertiserDailyBudgetResultItem),
                     typeof(AdsAdvertiserCursorPageInfo),
                 })
        {
            resolver!.GetTypeInfo(dto, options).Should().NotBeNull(
                $"{dto.Name} 未被登记即在 AOT 下反序列化失败（守卫 ADS-B6 要求逐上下文追加）");
        }
    }

    /// <summary>
    /// 业务面装配后，本线的词表外 Query 凭据键必须进入组件的强制掩码登记（ADS-B5 的功能面证据）。
    /// </summary>
    [Fact]
    public void AddWechatAdsApi_ShouldRegisterSensitiveQueryKeys_WhenBusinessModulesBuilt()
    {
        using var provider = BuildProvider(services =>
            services.AddAdsApp(CreateApps()).AddWechatAdsApi(AdsModule.Advertiser));

        var redacted = RedactSampleUrl();

        redacted.Should().Contain(AdsSensitiveQueryKeys.UserToken);
        redacted.Should().NotContain("secret-user-token-value",
            "user_token 不在组件静态词表内，未登记即随异常消息 / 遥测 URL 明文外泄");
    }

    /// <summary>组件脱敏门面（反射调用 internal 判定器，测试工程单 TFM net8.0、不参与 AOT 冒烟）。</summary>
    private static string RedactSampleUrl()
    {
        var url = "https://api.e.qq.com/v3.0/advertiser/update?user_token=secret-user-token-value&account_id=1";
        return SensitiveUrlProbe.Redact(url);
    }

    /// <summary>辅助：构造根容器（开 scope 校验）。</summary>
    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });
    }

    private static List<AdsAppConfig> CreateApps(string appKey = "default") => new()
    {
        new AdsAppConfig
        {
            AppKey = appKey,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            RedirectUri = "https://example.com/callback",
        },
    };

    /// <summary>反射调用组件的 URL 脱敏实现（<c>SensitiveUrlRedactor</c> 是 internal <b>static</b> 类，SDK 无法编译期引用）。</summary>
    private static class SensitiveUrlProbe
    {
        private static readonly MethodInfo? RedactMethod = ResolveRedactMethod();

        internal static string Redact(string url)
        {
            RedactMethod.Should().NotBeNull(
                "组件脱敏入口 internal static string SensitiveUrlRedactor.Redact(string) 变更即需同步本探针（ADS-B5 的行为面证据）");

            return (string)RedactMethod!.Invoke(obj: null, new object?[] { url })!;
        }

        private static MethodInfo? ResolveRedactMethod()
        {
            var type = typeof(Mud.HttpUtils.ApiException).Assembly
                .GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");

            // 取单参重载：双参重载多一个 extraKeys（请求级键集），本探针只验进程级登记面。
            return type?.GetMethod(
                "Redact",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                new[] { typeof(string) },
                modifiers: null);
        }
    }
}
