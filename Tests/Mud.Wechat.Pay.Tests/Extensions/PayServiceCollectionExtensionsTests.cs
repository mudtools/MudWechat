// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Tests.Credential;
using System.Text.Json.Serialization.Metadata;

namespace Mud.Wechat.Pay.Tests.Extensions;

/// <summary>
/// <see cref="PayServiceCollectionExtensions"/>（<c>AddWechatPayApi</c>）三段式注册测试。
/// </summary>
/// <remarks>
/// 三段式 = <see cref="PayModule"/> 枚举值 + <c>Add{域}Api()</c> + 源生成器产出的
/// <c>Add{域}WebApiHttpClient()</c>；三者必须同批成立，本组是其装配层的出口门禁。
/// </remarks>
public class PayServiceCollectionExtensionsTests
{
    private const string MerchantKeyId = "pay:merchant:1900000000:key";
    private const string ApiKeyId = "pay:merchant:1900000000:apikey";

    /// <summary>
    /// 注册后业务接口必须可解析，且为<b>跨 scope 同一实例来源下的正常瞬时服务</b>
    /// （AGENTS §7：新增 DI 面必补「可解析 + 根/作用域均可用」）。
    /// </summary>
    [Fact]
    public void AddWechatPayApi_ShouldRegisterDomainClients_ResolvableInRootAndScope()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(PayModule.Transactions));

        IWechatPayTransactionsService Resolve(IServiceProvider sp) =>
            sp.GetRequiredService<IWechatPayTransactionsService>();

        Resolve(provider).Should().NotBeNull();

        using var scope = provider.CreateScope();
        Resolve(scope.ServiceProvider).Should().NotBeNull();
    }

    /// <summary>
    /// 漏装凭据基座必须<b>在注册期 fail-fast 并点名修复方式</b> ——
    /// 否则要等到首次真实下单才发现 <c>IWechatPayHttpClient</c> 解析失败，代价极高。
    /// </summary>
    [Fact]
    public void AddWechatPayApi_ShouldFailFast_WhenAddPayAppMissing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatPayApi(PayModule.Transactions);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*AddPayApp*");
    }

    /// <summary>零模块注册是「什么都没装却以为装了」，必须拒绝。</summary>
    [Fact]
    public void AddWechatPayApi_ShouldFailFast_WhenNoModuleSelected()
    {
        var services = new ServiceCollection();
        services.AddPayApp(CreateMerchant());

        var act = () => services.AddWechatPayApi();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*至少需要添加一个模块*");
    }

    /// <summary>建造者委托重载与枚举重载装配出等价的可解析面。</summary>
    [Fact]
    public void AddWechatPayApi_ShouldSupportConfigureOverload_WhenUsingBuilderAction()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant())
                    .AddWechatPayApi(b => b.AddAllApis()));

        provider.GetRequiredService<IWechatPayTransactionsService>().Should().NotBeNull();
    }

    /// <summary>
    /// <b>AOT 生死线</b>：注册完成后，组件的 <c>JsonSerializerOptions</c> 必须能解析支付 DTO。
    /// </summary>
    /// <remarks>
    /// 生成的实现类在未注入序列化器时回退 <c>HttpContentSerializerFactory.CreateDefault()</c>，
    /// 该工厂的 <b>AOT 分支只组合已登记的源生成上下文、绝不回退反射</b> ——
    /// 漏登记 <c>TransactionsJsonContext</c> 时 JIT 一切正常、Native AOT 在首笔交易才炸。
    /// 本用例是唯一能在合入前抓到该缺陷的形态。
    /// </remarks>
    [Fact]
    public void AddWechatPayApi_ShouldRegisterPayJsonResolver_WhenBuiltOnNet8OrGreater()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(PayModule.Transactions));

#if NET8_0_OR_GREATER
        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        options.TypeInfoResolver.Should().NotBeNull(
            "因为漏登记即意味着 Native AOT 下支付 DTO 无元数据");

        options.TypeInfoResolver!
            .GetTypeInfo(typeof(JsapiPrepayRequest), options)
            .Should().NotBeNull("下单请求体必须能被源生成上下文解析");

        options.TypeInfoResolver!
            .GetTypeInfo(typeof(JsapiPrepayResponse), options)
            .Should().NotBeNull("下单应答体必须能被源生成上下文解析");
#else
        // net6.0：组件未提供 AddMudHttpClientJsonContext，Pay 与企微线同样以 NET8_0_OR_GREATER 裁剪。
        // 该 TFM 走 JIT 反射路径，不参与本仓 AOT 门禁（见 PayJsonResolverExtensions remarks）。
        provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Should().NotBeNull();
#endif
    }

    /// <summary>
    /// 四域三段式齐备：<see cref="PayServiceBuilder.AddAllApis"/> 后四域接口与账单下载通道全部可解析。
    /// </summary>
    /// <remarks>
    /// 锁定「枚举成员 ↔ 注册器 ↔ 源生成 <c>Add{域}WebApiHttpClient()</c>」三者同批成立
    /// （AGENTS §4 三段式的出口门禁）；某一域漏挂会在此 fail，而不是等到宿主首次调用才发现。
    /// </remarks>
    [Fact]
    public void AddWechatPayApi_AddAllApis_ShouldRegisterAllDomainsAndBillDownload()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(b => b.AddAllApis()));

        provider.GetRequiredService<IWechatPayTransactionsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayRefundService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayBillService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayCertificatesService>().Should().NotBeNull();

        // P2 首批分账域（三段式第三段：源生成 AddProfitSharingWebApiHttpClient 是否真被挂上）。
        provider.GetRequiredService<IWechatPayProfitSharingService>().Should().NotBeNull();

        // P2 支付分首批（三段式第三段：源生成 AddPayScoreWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayPayScoreService>().Should().NotBeNull();

        // 账单下载通道随账单模块注册（无 [HttpClientApi]，故无对应的源生成注册方法）。
        provider.GetRequiredService<IWechatPayBillDownloadService>().Should().NotBeNull();

        // 小程序调起支付签名是**本地密码学运算**（官方 TOC 内但无 HTTP 路由），同样随交易域注册 ——
        // 它是 P1-a 清单里最后一个非 HTTP 端点，缺它则小程序端无法调起支付。
        provider.GetRequiredService<IWechatPayMiniProgramPaySignService>().Should().NotBeNull();
    }

    /// <summary>
    /// 新增域 DTO 必须并入 AOT resolver：退款 / 账单 / 平台证书三域与 Common 基底均须可解析。
    /// </summary>
    [Fact]
    public void AddWechatPayApi_ShouldRegisterAllDomainJsonContexts_WhenBuiltOnNet8OrGreater()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(b => b.AddAllApis()));

#if NET8_0_OR_GREATER
        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var resolver = options.TypeInfoResolver;
        resolver.Should().NotBeNull();

        foreach (var dto in new[]
                 {
                     typeof(WechatPayResponse),
                     typeof(RefundApplyRequest),
                     typeof(RefundResponse),
                     typeof(BillDownloadInfoResponse),
                     typeof(PlatformCertificatesResponse),
                     typeof(ProfitSharingOrderResponse),
                     typeof(ProfitSharingAddReceiverRequest),
                     typeof(ProfitSharingReturnOrderResponse),
                     typeof(ProfitSharingAmountsResponse),
                     typeof(ProfitSharingDeleteReceiverResponse),
                     typeof(PayScoreServiceOrderResponse),
                     typeof(PayScoreServiceOrderRequest),
                     typeof(PayScoreCompleteOrderResponse),
                     typeof(PayScoreModifyOrderResponse),
                     typeof(PayScoreCollectResponse),
                 })
        {
            resolver!.GetTypeInfo(dto, options).Should().NotBeNull(
                $"{dto.Name} 必须被源生成上下文覆盖，否则 Native AOT 下无元数据");
        }
#else
        provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Should().NotBeNull();
#endif
    }

    /// <summary>
    /// 平台证书<b>读写端口必须指向同一实例</b>，且证书域注册后按需刷新器可解析。
    /// </summary>
    /// <remarks>
    /// 若两个端口各 new 一份默认缓存，则「刷新写进 A、验签读 B」——刷新看起来成功而验签继续失败，
    /// 是本线最难排查的一类静默不一致，故在此锁死实例同一性。
    /// </remarks>
    [Fact]
    public void AddWechatPayApi_ShouldWireCertificateStoreAndWriterToSameInstance()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(b => b.AddAllApis()));

        var cache = provider.GetRequiredService<WechatPayPlatformCertificateCache>();

        provider.GetRequiredService<IWechatPayPlatformCertificateStore>().Should().BeSameAs(cache);
        provider.GetRequiredService<IWechatPayPlatformCertificateWriter>().Should().BeSameAs(cache,
            "刷新写入的缓存必须就是验签读取的存储，否则「刷新成功」而验签继续失败（静默不一致）");

        provider.GetRequiredService<IWechatPayPlatformCertificateRefresher>().Should().NotBeNull(
            "证书域注册后按需刷新器可解析（回调包以可选方式消费它）");
    }

    /// <summary>辅助：单商户 + 注册密钥端口后构建根容器（开 scope 校验）。</summary>
    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
            new Dictionary<string, string>
            {
                [MerchantKeyId] = "unused",
                [ApiKeyId] = "unused",
            }));

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });
    }

    /// <summary>辅助：构造一个合法商户（= 默认商户）。</summary>
    private static List<WechatPayMerchantConfig> CreateMerchant() => new()
    {
        new()
        {
            MchId = "1900000000",
            SerialNumber = "SERIAL-0",
            PrivateKeySecretName = MerchantKeyId,
            ApiKeySecretName = ApiKeyId,
        },
    };
}
