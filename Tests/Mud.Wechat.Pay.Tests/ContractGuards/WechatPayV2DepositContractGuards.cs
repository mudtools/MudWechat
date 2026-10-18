// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils;
using Mud.Wechat.Pay.Tests.ContractGuards;
using Mud.Wechat.Pay.Tests.Credential;
using Mud.Wechat.Pay.V2.DataModels.PayV2;
using Mud.Wechat.Pay.V2.Extensions;
using Mud.Wechat.Pay.V2.Secure;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

/// <summary>
/// 微信支付 APIv2「受理押金」域契约守卫（<b>8 端点 = 免证书声明式 5 + 需证书手写 3</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核实「是否需要证书」栏）：
/// 统一收单下单并支付页面 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986340"/>、
/// 人脸支付 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986577"/>、
/// 付款码支付 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986623"/>、
/// 查询订单 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986679"/>、
/// 查询退款 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011987300"/>、
/// 撤销订单 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986741"/>、
/// 消费押金 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986801"/>、
/// 申请退款 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011987251"/>。</para>
/// <para>
/// <b>本守卫的独特对象是「一域两形态」</b>：同一个押金域里，五支免证书端点带 <c>[HttpClientApi]</c>
/// 走源生成管道，三支需证书端点必须手写。这个分叉最容易在下一次改动中被抹平 ——
/// 把三支「顺手对齐」成声明式，会在无证书的连接池上发出证书端点（官方直接拒绝）；
/// 把五支「顺手对齐」成手写，会凭空多一套签名口径。故两面的形态断言都在这里锁。
/// </para>
/// <para>
/// <b>域名事实</b>：八支官方全部给 <c>api.mch.weixin.qq.com</c>，<b>无一</b>落在 <c>api2.mch</c> ⇒
/// 手写三支的域名实参与 <c>/secapi/pay/*</c> 一族不同，这是本守卫最硬的一条断言。
/// </para>
/// </remarks>
public class WechatPayV2DepositContractGuards
{
    /// <summary>免证书五支的官方路由（声明式面）。</summary>
    private static readonly string[] DeclarativeRoutes =
    {
        "/deposit/unifiedorder",
        "/deposit/facepay",
        "/deposit/micropay",
        "/deposit/orderquery",
        "/deposit/refundquery",
    };

    /// <summary>需证书三支的官方路由（手写面，顺序与接口方法声明序一致）。</summary>
    private static readonly string[] SecureRoutes =
    {
        "/deposit/reverse",
        "/deposit/consume",
        "/deposit/refund",
    };

    /// <summary>声明式五支的路由表必须逐字等于官方五页，且整族挂 APIv2 命名客户端。</summary>
    [Fact]
    public void V2_Deposit_DeclarativeRoutes_ShouldMatchOfficialFive()
    {
        RoutesOf<IWechatPayV2DepositService>().Should().BeEquivalentTo(DeclarativeRoutes,
            "because 押金域官方开放 8 支，其中免证书的恰是下单/人脸/付款码/查单/查退款这 5 支");

        typeof(IWechatPayV2DepositService)
            .GetCustomAttributes(typeof(AllowAnyStatusCodeAttribute), inherit: false)
            .Should().NotBeEmpty("because APIv2 业务失败也是 HTTP 200 + return_code=FAIL");

        var api = typeof(IWechatPayV2DepositService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.DepositV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName,
            "because APIv2 端点必须走 XML + 签名 Handler 管道");
    }

    /// <summary>需证书三支必须是<b>无 [HttpClientApi]</b> 的普通接口：证书按商户绑定，声明式管道拿不到。</summary>
    [Fact]
    public void V2_Deposit_SecureInterface_ShouldBePlainWithoutHttpClientApi()
    {
        typeof(IWechatPayV2DepositSecureService).IsPublic.Should().BeTrue();
        typeof(IWechatPayV2DepositSecureService)
            .GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false)
            .Should().BeEmpty(
                "because 押金三支官方逐页标注「需要证书」，加上特性即等于在无证书连接池上发出证书端点");

        typeof(IWechatPayV2DepositSecureService)
            .GetCustomAttributes(typeof(AllowAnyStatusCodeAttribute), inherit: false)
            .Should().BeEmpty("because 手写族不经声明式管道，AllowAnyStatusCode 对它无语义");
    }

    /// <summary>手写三支的方法集与路由不得漂移（源文本是唯一的路由载体）。</summary>
    [Fact]
    public void V2_Deposit_SecureMethods_ShouldBindOfficialRoutesOnApiMch()
    {
        var methods = typeof(IWechatPayV2DepositSecureService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        methods.Should().HaveSameCount(SecureRoutes);

        var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2DepositSecureService.cs"));

        foreach (var (method, expectedRoute) in methods.Zip(SecureRoutes))
        {
            var callIndex = text.IndexOf(method.Name + "(", StringComparison.Ordinal);
            callIndex.Should().BeGreaterThanOrEqualTo(0,
                $"{method.Name} 在实现类中找不到 ⇒ 实现与接口脱钩");

            var routeIndex = text.IndexOf('"', callIndex);
            var routeEnd = text.IndexOf('"', routeIndex + 1);
            text.Substring(routeIndex + 1, routeEnd - routeIndex - 1).Should().Be(expectedRoute,
                $"{method.Name} 的路由必须照抄官方本页（{expectedRoute}）");
        }

        // 域名：逐支显式钉在 api.mch，且整个文件不得出现证书域名常量。
        Occurrences(text, "WechatPayV2HttpClientNames.BaseAddress, request")
            .Should().Be(SecureRoutes.Length,
                "because 押金三支官方域名是 api.mch，不靠默认值可避免下次改动漏掉实参");
        text.Should().NotContain("SecureBaseAddress",
            "because 改成 api2.mch 后该域名下无 /deposit/* 路由 ⇒ 404；这是最容易被「顺手对齐」搞错的一处");

        // 三支全部单签（无 workwx_sign 需求）。
        Occurrences(text, "PostWorkWxAsync").Should().Be(0,
            "because 押金族官方无企业微信侧签名要求，误挂双签会把未注册 Secret 端口的宿主拖进 fail-fast");
    }

    /// <summary>8 端点的加法锁 + 两形态互斥：一支挪窝即打红。</summary>
    [Fact]
    public void V2_Deposit_TotalEndpoints_ShouldSumToEight_WithNoRouteOverlap()
    {
        RoutesOf<IWechatPayV2DepositService>().Count.Should().Be(5);
        SecureRoutes.Length.Should().Be(3);

        var all = RoutesOf<IWechatPayV2DepositService>().Concat(SecureRoutes).ToList();
        all.Should().HaveCount(8, "because 押金域官方共 8 支");
        all.Distinct().Should().HaveCount(8, "because 同一端点不得同时有声明式与手写两条走法");

        // 与基础支付族不交叠（/pay/* 与 /secapi/* 归 IWechatPayV2Service / IWechatPayV2SecureService）。
        all.Should().OnlyContain(r => r.StartsWith("/deposit/", StringComparison.Ordinal),
            "because 押金族路径前缀统一是 /deposit/，出现别的前缀即说明路由写错域");
    }

    /// <summary>
    /// 模块注册器必须<b>同时</b>挂生成注册组与手写服务：只挂其一即「半个域静默不注册」。
    /// </summary>
    [Fact]
    public void V2_Deposit_ModuleRegistrar_ShouldBindBothShapes()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Extensions/PayV2ServiceBuilder.cs"));

        builderSource.Should().Contain("[PayV2Module.DepositV2]");
        builderSource.Should().Contain("AddDepositV2WebApiHttpClient()",
            "because 免证书五支属声明式组，缺注册器即五支静默不可用");
        builderSource.Should().Contain(
            "TryAddSingleton<IWechatPayV2DepositSecureService, WechatPayV2DepositSecureService>",
            "because 需证书三支手写，缺这一行即三支静默不可用");

        // 押金域有生成注册组 ⇒ 不得被认作「纯手写族」白名单成员（那会关掉它的注册组断言）。
        WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().NotContain(nameof(PayV2Module.DepositV2),
            "because 白名单的含义是「该成员没有生成注册组」，而 DepositV2 有");
    }

    /// <summary>
    /// DI：两支服务同由 <c>DepositV2</c> 模块注册、均为 Singleton，且只装本模块时不泄漏他域服务。
    /// </summary>
    [Fact]
    public void V2_Deposit_Services_ShouldResolveAsSingletonsAndNotLeakOtherDomains()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.DepositV2)));

        var open = provider.GetRequiredService<IWechatPayV2DepositService>();
        var secure = provider.GetRequiredService<IWechatPayV2DepositSecureService>();

        secure.Should().BeOfType<WechatPayV2DepositSecureService>();

        using (var scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IWechatPayV2DepositSecureService>()
                .Should().BeSameAs(secure, "because 证书通道按商户复用，服务须是 Singleton");
        }

        provider.GetService<IWechatPayV2CustomsService>().Should().BeNull();
        provider.GetService<IWechatPayV2RiskService>().Should().BeNull();

        // 声明式接口的生成实现可解析（解析不到即注册组名与特性不一致）。
        open.Should().NotBeNull();
    }

    /// <summary>押金 16 型（8 端点成对）必须全部登记进 <c>PayV2JsonContext</c>。</summary>
    [Fact]
    public void V2_Deposit_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        var expected = new[]
        {
            nameof(V2DepositUnifiedOrderRequest), nameof(V2DepositUnifiedOrderResponse),
            nameof(V2DepositFacePayRequest), nameof(V2DepositFacePayResponse),
            nameof(V2DepositMicropayRequest), nameof(V2DepositMicropayResponse),
            nameof(V2DepositOrderQueryRequest), nameof(V2DepositOrderQueryResponse),
            nameof(V2DepositRefundQueryRequest), nameof(V2DepositRefundQueryResponse),
            nameof(V2DepositReverseRequest), nameof(V2DepositReverseResponse),
            nameof(V2DepositConsumeRequest), nameof(V2DepositConsumeResponse),
            nameof(V2DepositRefundRequest), nameof(V2DepositRefundResponse),
        };

        expected.Should().HaveCount(16, "because 押金 8 端点全是 POST 且各有请求/应答型 ⇒ 恰 16 型");

        var context = PayV2Context();
        foreach (var name in expected)
        {
            var type = typeof(V2DepositUnifiedOrderRequest).Assembly
                .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2." + name, throwOnError: false);
            type.Should().NotBeNull($"{name} 必须存在于 PayV2 命名空间");
            context.GetTypeInfo(type!).Should().NotBeNull(
                $"{name} 未登记进 PayV2JsonContext ⇒ 重跑 scripts/GenerateJsonContext.ps1");
        }

        // 付款码应答复用人脸应答的字段面（官方两页应答表逐字相同）⇒ 继承是有意的，改名会破 AOT003 豁免口径。
        typeof(V2DepositMicropayResponse).Should().BeDerivedFrom(typeof(V2DepositFacePayResponse),
            "because 官方两页的应答字段表完全一致；分开两份即同一份契约写两遍，改一处漏一处");
    }

    /// <summary>官方字段名的两处反直觉拼写必须照抄，不得「顺手修正」。</summary>
    [Fact]
    public void V2_Deposit_OfficialMisspellingAndAmountFields_ShouldStayAsOfficialSpelling()
    {
        JsonNameOf<V2DepositRefundQueryResponse>(nameof(V2DepositRefundQueryResponse.RefundRecvAccout))
            .Should().Be("refund_recv_accout",
                "because 官方原文就少一个 n；写成 refund_recv_account 永远读不到值");

        // 押金消费的金额面是 total_fee + consume_fee（官方无 deposit_total_fee 一类字段）。
        JsonNameOf<V2DepositConsumeRequest>(nameof(V2DepositConsumeRequest.ConsumeFee)).Should().Be("consume_fee");
        JsonNameOf<V2DepositOrderQueryResponse>(nameof(V2DepositOrderQueryResponse.ConsumeFee)).Should().Be("consume_fee");
        JsonNameOf<V2DepositOrderQueryResponse>(nameof(V2DepositOrderQueryResponse.TradeStateDesc))
            .Should().Be("trade_state_desc");
    }

    private static List<string> RoutesOf<TInterface>()
        where TInterface : class
        => typeof(TInterface)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(r => r != null)
            .Select(r => r!)
            .ToList();

    private static string? JsonNameOf<T>(string propertyName)
        => typeof(T).GetProperty(propertyName)!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;

    private static global::System.Text.Json.Serialization.JsonSerializerContext PayV2Context()
    {
        var contextType = typeof(V2DepositUnifiedOrderRequest).Assembly
            .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2.PayV2JsonContext", throwOnError: false);
        contextType.Should().NotBeNull();
        return (global::System.Text.Json.Serialization.JsonSerializerContext)
            contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
    }

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
