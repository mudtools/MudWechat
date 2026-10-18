// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils;
using Mud.Wechat.Pay.Tests.ContractGuards;
using Mud.Wechat.Pay.Tests.Credential;
using Mud.Wechat.Pay.V2.Extensions;
using Mud.Wechat.Pay.V2.Secure;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

/// <summary>
/// 微信支付 APIv2 <b>纯手写证书族</b>契约守卫（<b>11 端点 = 红包 5 + 企业付款 4 + 付款到银行卡 2</b>）。
/// </summary>
/// <remarks>
/// <para><b>本守卫的范围界定</b>：只覆盖「<b>整族</b>手写」的三支。押金域的需证书三支与风控域的
/// 违规记录查询同样是证书端点，但分别落在 <c>WechatPayV2DepositContractGuards</c> 与
/// <c>WechatPayV2CustomsToolsRiskContractGuards</c>（押金域还有免证书的五支声明式端点，
/// 拆到本守卫会把「一域两形态」这一事实抹平）⇒ 域名/路径的断言在各守卫各自成立，不重复也不遗漏。</para>
/// <para><b>官方文档</b>（本守卫覆盖的路由出处，逐支已在现行文档中心核实）：
/// 现金红包 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011974053"/>、
/// 小程序红包 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937425"/>、
/// 查询红包 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011981612"/>、
/// 企业付款到零钱 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011989673"/>、
/// 查询企业付款 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011989767"/>、
/// 付款到银行卡 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011939840"/>、
/// 查询付款到银行卡 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011941066"/>；
/// 企业红包双支与向员工付款双支在企业微信开发者中心
/// （<see href="https://developer.work.weixin.qq.com/document/path/90275"/>、
/// <see href="https://developer.work.weixin.qq.com/document/path/90276"/>、
/// <see href="https://developer.work.weixin.qq.com/document/path/90278"/>、
/// <see href="https://developer.work.weixin.qq.com/document/path/90279"/>）。</para>
/// <para>
/// <b>本族为什么必须由源文本断言</b>：三支手写族<b>不带</b> <c>[HttpClientApi]</c> ⇒ 路由不在特性上，
/// 反射读不到「哪支方法打哪条路径」。证书与商户绑定使它们只能走逐商户通道，代价就是路由与域名
/// 这两个最易错的面失去了编译期载体，故本守卫按仓库既有惯例（<c>SourcePath</c> + 方法体文本）
/// 把它们锁回去：<b>路径写错</b>（如把 <c>gethbinfo</c> 写成 <c>getredpackinfo</c>）与
/// <b>域名写错</b>（把 <c>BaseAddress</c> 顺手「对齐」成 <c>SecureBaseAddress</c>）都会在这里打红。
/// </para>
/// <para>
/// <b>域名是本守卫存在的首要理由</b>：<c>/secapi/pay/*</c> 与两份账单在 <c>api2.mch</c>，
/// 而本族十一支官方全部给 <c>api.mch</c> <b>却同样要求客户端证书</b> ——
/// 「要证书」与「在证书域名上」是两条独立的官方事实，任何一次「统一」都会造成 404 或不呈现证书被拒。
/// </para>
/// </remarks>
public class WechatPayV2CertFamilyContractGuards
{
    /// <summary>
    /// 纯手写族的<b>模块成员名</b>：它们有意<b>不</b>对应任何生成注册组。
    /// <c>RiskV2</c> 同属此类（违规记录查询官方判「需要证书」）；
    /// <c>SubMerchantConfigV2</c>／<c>MerchantMicroV2</c>／<c>InstitutionSubV2</c>／<c>MerchantMediaV2</c>
    /// 四族亦属此类（官方「是否需要证书」逐页为「是」，媒体支还叠加 multipart 载体）；
    /// <c>BillCommentV2</c>／<c>FraudV2</c>／<c>FundV2</c> 三族同样逐页为「需要」，
    /// 其中 <c>FraudV2</c> 挂第四个闭集域名 <c>fraud.mch</c>、<c>BillCommentV2</c> 走原文出口 ——
    /// 三族的路由/域名/报文断言在 <c>WechatPayV2BillCommentFraudFundContractGuards</c>，
    /// <b>本守卫的 11 端点路由表只覆盖红包/付款三族</b>（其余各族按域分文件，见各类注释）。
    /// 而押金域是<b>一域两形态</b>（免证书五支有 <c>DepositV2</c> 注册组 + 需证书三支手写），
    /// 故 <c>PayV2Module.DepositV2</c> <b>不得</b>进本名单 —— 它确实有生成注册器。
    /// </summary>
    internal static readonly string[] HandWrittenModules =
    {
        nameof(PayV2Module.RedPackV2),
        nameof(PayV2Module.TransferV2),
        nameof(PayV2Module.PartnerTransferV2),
        nameof(PayV2Module.RiskV2),
        nameof(PayV2Module.SubMerchantConfigV2),
        nameof(PayV2Module.MerchantMicroV2),
        nameof(PayV2Module.InstitutionSubV2),
        nameof(PayV2Module.MerchantMediaV2),
        nameof(PayV2Module.BillCommentV2),
        nameof(PayV2Module.FraudV2),
        nameof(PayV2Module.FundV2),
    };

    /// <summary>手写族接口 ⇒ 官方路由清单（11 支，键为接口方法所属族）。</summary>
    private static readonly Dictionary<Type, string[]> FamilyRoutes = new()
    {
        [typeof(IWechatPayV2RedPackService)] = new[]
        {
            "/mmpaymkttransfers/sendredpack",
            "/mmpaymkttransfers/sendminiprogramhb",
            "/mmpaymkttransfers/gethbinfo",
            "/mmpaymkttransfers/sendworkwxredpack",
            "/mmpaymkttransfers/queryworkwxredpack",
        },
        [typeof(IWechatPayV2TransferService)] = new[]
        {
            "/mmpaymkttransfers/promotion/transfers",
            "/mmpaymkttransfers/gettransferinfo",
            "/mmpaymkttransfers/promotion/paywwsptrans2pocket",
            "/mmpaymkttransfers/promotion/querywwsptrans2pocket",
        },
        [typeof(IWechatPayV2PartnerTransferService)] = new[]
        {
            "/mmpaysptrans/pay_bank",
            "/mmpaysptrans/query_bank",
        },
    };

    /// <summary>手写族接口 ⇒ 实现类（实现类内不写路由以外的逻辑，路由即契约面）。</summary>
    private static readonly Dictionary<Type, (Type Impl, string SourceFile)> Implementations = new()
    {
        [typeof(IWechatPayV2RedPackService)] =
            (typeof(WechatPayV2RedPackService), "Secure/WechatPayV2RedPackService.cs"),
        [typeof(IWechatPayV2TransferService)] =
            (typeof(WechatPayV2TransferService), "Secure/WechatPayV2TransferService.cs"),
        [typeof(IWechatPayV2PartnerTransferService)] =
            (typeof(WechatPayV2PartnerTransferService), "Secure/WechatPayV2PartnerTransferService.cs"),
    };

    /// <summary>
    /// 三支接口必须是<b>无 <c>[HttpClientApi]</c></b> 的普通公共接口 ——
    /// 一旦有人「顺手对齐」加上特性，源生成客户端就会在无证书的连接池上发出证书端点。
    /// </summary>
    [Fact]
    public void V2_CertFamily_Interfaces_ShouldBePlainPublicWithoutHttpClientApi()
    {
        foreach (var family in FamilyRoutes.Keys)
        {
            family.IsPublic.Should().BeTrue($"{family.Name} 是宿主直接注入的公开面");
            family.GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false)
                .Should().BeEmpty(
                    $"{family.Name} 承载需商户证书的端点：证书按商户绑定，与命名客户端「一名一连接池」的复用模型冲突 ⇒ 不得走源生成");
        }
    }

    /// <summary>每族的方法集不得漂移（增删端点必须同批改本守卫的路由清单）。</summary>
    [Fact]
    public void V2_CertFamily_MethodSets_ShouldMatchOfficialFamilies()
    {
        foreach (var (interfaceType, routes) in FamilyRoutes)
        {
            var methods = DeclaredMethods(interfaceType);

            methods.Should().HaveCount(routes.Length,
                $"{interfaceType.Name} 的端点数须与官方族页面支数一致（红包 5 / 企业付款 4 / 付款到银行卡 2）");

            foreach (var method in methods)
            {
                method.Name.Should().EndWith("Async", "because 全仓异步方法命名口径");
                method.ReturnType.Should().BeAssignableTo(typeof(Task),
                    "because 应答形态必须是可等待的官方 DTO 任务");
            }
        }

        // 族接口的成员总数即「11 端点」这一契约面的加法锁。
        FamilyRoutes.Sum(f => DeclaredMethods(f.Key).Length).Should().Be(11,
            "because 需证书的手写族共 11 支；与基础支付的证书 5 支、声明式 8 分属不同守卫");
    }

    /// <summary>
    /// 路由逐支绑定：实现类源文本里，每个 <c>...Async(</c> 之后紧跟的第一条路径字面量必须等于官方路由。
    /// </summary>
    [Fact]
    public void V2_CertFamily_EachMethod_ShouldBindTheOfficialRoute()
    {
        foreach (var (interfaceType, routes) in FamilyRoutes)
        {
            var source = Implementations[interfaceType];
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", source.SourceFile));

            var methods = DeclaredMethods(interfaceType);
            methods.Should().HaveSameCount(routes,
                $"{interfaceType.Name} 的方法数与守卫路由清单须同批（否则 Zip 会静默截断，把漏配的端点藏起来）");

            foreach (var (method, expectedRoute) in methods.Zip(routes))
            {
                var callIndex = text.IndexOf(method.Name + "(", StringComparison.Ordinal);
                callIndex.Should().BeGreaterThanOrEqualTo(0,
                    $"{interfaceType.Name}.{method.Name} 在实现类 {source.SourceFile} 中找不到 ⇒ 实现与接口脱钩");

                var routeIndex = text.IndexOf('\"', callIndex);
                routeIndex.Should().BeGreaterThanOrEqualTo(0);
                var routeEnd = text.IndexOf('\"', routeIndex + 1);
                var actualRoute = text.Substring(routeIndex + 1, routeEnd - routeIndex - 1);

                actualRoute.Should().Be(expectedRoute,
                    $"{method.Name} 的路由必须照抄官方本页（{expectedRoute}）——本族无特性载体，源文本是唯一的路由契约面");
            }
        }
    }

    /// <summary>
    /// 域名口径：十一支<b>全部</b>挂在 <c>BaseAddress</c>（<c>api.mch</c>）上，
    /// 且实现源文本<b>一个</b> <c>SecureBaseAddress</c> 都不许出现。
    /// </summary>
    [Fact]
    public void V2_CertFamily_ShouldUseApiMchDomain_NotSecureBaseAddress()
    {
        foreach (var (interfaceType, source) in Implementations)
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", source.SourceFile));

            var callSites = FamilyRoutes[interfaceType].Length;

            // 只数「实参位」上的域名实参（注释里的 cref 也提到这个常量，故带上前置逗号一起匹配）。
            Occurrences(text, "WechatPayV2HttpClientNames.BaseAddress, request")
                .Should().Be(callSites,
                    $"{interfaceType.Name} 的 {callSites} 支端点须逐支显式钉在 api.mch 域名上（不靠默认值，避免下次改动漏掉）");

            text.Should().NotContain("SecureBaseAddress",
                "because 本族官方域名是 api.mch；改成 api2.mch 该域名下无这些路由 ⇒ 404，这是最容易被「顺手对齐」搞错的一处");
        }

        // 反向确认：证书域名常量本身仍属基础支付证书族，本守卫只锁「本族不用它」。
        WechatPayV2HttpClientNames.BaseAddress.Should().Be("https://api.mch.weixin.qq.com");
        WechatPayV2HttpClientNames.SecureBaseAddress.Should().Be("https://api2.mch.weixin.qq.com");
    }

    /// <summary>
    /// 双签覆盖面：<b>恰好两支</b>写操作走 <c>PostWorkWxAsync</c>，且各自挂对应白名单；
    /// 其余九支（含全部查询）只算 <c>sign</c>。
    /// </summary>
    [Fact]
    public void V2_CertFamily_WorkWxDualSign_ShouldCoverExactlyTwoWrites()
    {
        var redPack = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2RedPackService.cs"));
        var transfer = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2TransferService.cs"));
        var bank = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2PartnerTransferService.cs"));

        Occurrences(redPack, "PostWorkWxAsync").Should().Be(1, "because 红包族只有 sendworkwxredpack 需 workwx_sign");
        Occurrences(transfer, "PostWorkWxAsync").Should().Be(1, "because 付款族只有 paywwsptrans2pocket 需 workwx_sign");
        Occurrences(bank, "PostWorkWxAsync").Should().Be(0, "because 付款到银行卡官方不要求 workwx_sign");

        redPack.Should().Contain("WechatPayV2WorkWxSigner.SendRedPackWhitelist",
            "because 白名单是官方字段集，写错字段集即签不出官方认可的 workwx_sign");
        transfer.Should().Contain("WechatPayV2WorkWxSigner.PayPocketWhitelist");

        // 查询三支 + 其余六支一律走单签出口 ⇒ 未注册企微 Secret 端口的宿主不应被双签族牵连。
        Occurrences(redPack, "PostAsync<").Should().Be(4);
        Occurrences(transfer, "PostAsync<").Should().Be(3);
        Occurrences(bank, "PostAsync<").Should().Be(2);
    }

    /// <summary>三支手写族各自有模块成员，且注册器挂在 <c>TryAddSingleton</c> 上（而非生成注册方法）。</summary>
    [Fact]
    public void V2_CertFamily_HandWrittenModules_ShouldBindSingletonServicesInBuilder()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Extensions/PayV2ServiceBuilder.cs"));

        foreach (var module in HandWrittenModules)
        {
            builderSource.Should().Contain($"[PayV2Module.{module}]", $"because 成员无注册器即「静默不注册」");
            Enum.TryParse<PayV2Module>(module, out _).Should().BeTrue();
        }

        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2RedPackService, WechatPayV2RedPackService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2TransferService, WechatPayV2TransferService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2PartnerTransferService, WechatPayV2PartnerTransferService>");

        // 手写族不得有同名生成注册器（那意味着有人给它们补了 [HttpClientApi]）。
        foreach (var module in HandWrittenModules)
        {
            builderSource.Should().NotContain($"Add{module}WebApiHttpClient()",
                $"{module} 是手写族，出现生成注册器即说明证书族被误挂到声明式管道上");
        }
    }

    /// <summary>
    /// DI 可解析 + 跨 scope 同一实例（新增 Singleton 的必补用例）；
    /// 且<b>只装基础支付时</b>解析不到本族服务 ⇒ 模块隔离有效。
    /// </summary>
    [Fact]
    public void V2_CertFamily_Services_ShouldResolveAsSingletonsPerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(
                PayV2Module.RedPackV2, PayV2Module.TransferV2, PayV2Module.PartnerTransferV2)));

        var redPack = provider.GetRequiredService<IWechatPayV2RedPackService>();
        var transfer = provider.GetRequiredService<IWechatPayV2TransferService>();
        var bank = provider.GetRequiredService<IWechatPayV2PartnerTransferService>();

        redPack.Should().BeOfType<WechatPayV2RedPackService>();
        transfer.Should().BeOfType<WechatPayV2TransferService>();
        bank.Should().BeOfType<WechatPayV2PartnerTransferService>();

        using (var scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IWechatPayV2RedPackService>()
                .Should().BeSameAs(redPack, "because 证书通道是商户级复用，服务必须是 Singleton（Scoped 会逐请求重建通道表）");
        }

        using var baseOnly = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.BasePayV2)));

        baseOnly.GetService<IWechatPayV2RedPackService>().Should().BeNull(
            "because 未注册 RedPackV2 模块就不应凭空多出红包服务");
        baseOnly.GetRequiredService<IWechatPayV2SecureService>().Should().NotBeNull(
            "because 基础支付的证书五支与手写族共用底座，单独注册 BasePayV2 必须自洽");
    }

    /// <summary>底座（通道持有者 + 收发器）不随模块增减：任一手写族单独注册即可解析。</summary>
    [Fact]
    public void V2_CertFamily_SecureInvoker_ShouldBeBaseNotPerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.TransferV2)));

        provider.GetRequiredService<WechatPayV2SecureInvoker>().Should().NotBeNull();
        provider.GetRequiredService<WechatPayV2SecureChannelProvider>().Should().NotBeNull();
    }

    /// <summary>证书族不得与基础支付证书五支重叠（同一端点两条走法 ⇒ 签名与验签口径分叉）。</summary>
    [Fact]
    public void V2_CertFamily_Routes_ShouldNotOverlapSecureBasePay()
    {
        var certRoutes = FamilyRoutes.Values.SelectMany(static r => r).ToList();

        certRoutes.Should().HaveCount(11, "because 去重前后同为 11 条即无重复路由");
        certRoutes.Should().NotContain(r => r.StartsWith("/secapi/", StringComparison.Ordinal)
                                            || r.StartsWith("/pay/", StringComparison.Ordinal),
            "because /secapi/pay/* 与 /pay/download* 归 IWechatPayV2SecureService（api2.mch），本族只覆盖 api.mch 上的红包/付款路径");

        // 三族路径前缀互不相同，是官方给的事实。
        certRoutes.Should().OnlyContain(r =>
            r.StartsWith("/mmpaymkttransfers/", StringComparison.Ordinal)
            || r.StartsWith("/mmpaysptrans/", StringComparison.Ordinal));
    }

    /// <summary>与 APIv3 的付款/红包线不得共用接口（两条线的报文与凭据模型不同）。</summary>
    [Fact]
    public void V2_CertFamily_Interfaces_ShouldLiveInPayV2Assembly()
    {
        foreach (var family in FamilyRoutes.Keys)
        {
            family.Assembly.GetName().Name.Should().Be("Mud.Wechat.Pay.V2",
                "because APIv2 手写族与 APIv3 的 Transfer / RedPacket 域分属两个程序集");
            family.Namespace.Should().Be("Mud.Wechat.Pay.V2",
                "because 可注入接口落线根命名空间（与 IWechatPayV2SecureService 同面）");
        }
    }

    /// <summary>
    /// <b>手写族的 Secret 端口是可选依赖</b>：收发器经 <c>IServiceProvider.GetService</c> 惰性解析
    /// <c>IWechatPayV2WorkWxSecretProvider</c> ⇒ 未注册它的宿主仍能装配全部证书族（单签端点照常可用），
    /// 而双签支在调用时才 fail-fast。构造期强依赖会把所有宿主都绑上企微凭据桥接，属过度耦合。
    /// </summary>
    [Fact]
    public void V2_CertFamily_WorkWxSecretPort_ShouldBeLazyNotConstructorDependency()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddAllApis()));

        // 未注册 IWechatPayV2WorkWxSecretProvider 时，三支服务与收发器仍须全部可解析。
        provider.GetRequiredService<WechatPayV2SecureInvoker>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayV2RedPackService>().Should().NotBeNull();

        var ctor = typeof(WechatPayV2SecureInvoker).GetConstructors().Single();
        ctor.GetParameters().Select(static p => p.ParameterType)
            .Should().NotContain(typeof(Mud.Wechat.Pay.V2.Credential.IWechatPayV2WorkWxSecretProvider),
                "because 双签 Secret 端口只在两支写操作里被用到，注册为构造依赖会强制全部宿主桥接企微凭据");
    }

    private static MethodInfo[] DeclaredMethods(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static int Occurrences(string text, string needle)
    {
        var count = 0;
        var index = text.IndexOf(needle, StringComparison.Ordinal);
        while (index >= 0)
        {
            count++;
            index = text.IndexOf(needle, index + needle.Length, StringComparison.Ordinal);
        }

        return count;
    }

    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
            new Dictionary<string, string>
            {
                ["pay:merchant:1900000000:key"] = "placeholder",
                ["pay:merchant:1900000000:apikey"] = new string('a', 32),
                ["pay:merchant:1900000000:apiv2key"] = new string('k', 32),
            }));

        services.AddPayApp(new List<WechatPayMerchantConfig>
        {
            new()
            {
                MchId = "1900000000",
                SerialNumber = WechatPayGoldenVectors.MerchantSerialNumber,
                PrivateKeySecretName = "pay:merchant:1900000000:key",
                ApiKeySecretName = "pay:merchant:1900000000:apikey",
                ApiV2KeySecretName = "pay:merchant:1900000000:apiv2key",
            },
        });

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

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
