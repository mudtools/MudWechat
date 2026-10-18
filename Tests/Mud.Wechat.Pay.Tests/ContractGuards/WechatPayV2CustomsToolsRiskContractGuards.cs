// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
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
/// 微信支付 APIv2「清关报关 / 工具 / 风控」三域契约守卫（<b>6 端点 = 报关 3 + 工具 2 + 风控 1</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核实证书栏与参数表）：
/// 报关提交 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985151"/>、
/// 报关查询 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985273"/>、
/// 报关重推 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985318"/>、
/// 付款码查询 openid <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011940944"/>、
/// 转换短链接 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937195"/>、
/// 违规记录查询 <see href="https://pay.weixin.qq.com/doc/v2/institution/4011985446"/>。</para>
/// <para>
/// <b>三域合一守卫的理由</b>：三域各自端点少（3/2/1），但共享同一批「例外事实」——
/// 报关族<b>无</b> <c>nonce_str</c>、风控族请求<b>无</b> <c>appid</c>、两支工具族官方页<b>不写</b>请求方式。
/// 这些例外是「按通用形状补齐」时最容易被顺手改错的面，集中一处比拆成三个小守卫更难漏。
/// </para>
/// </remarks>
public class WechatPayV2CustomsToolsRiskContractGuards
{
    private static readonly string[] CustomsRoutes =
    {
        "/cgi-bin/mch/customs/customdeclareorder",
        "/cgi-bin/mch/customs/customdeclarequery",
        "/cgi-bin/mch/newcustoms/customdeclareredeclare",
    };

    private static readonly string[] ToolsRoutes =
    {
        "/tools/authcodetoopenid",
        "/tools/shorturl",
    };

    [Fact]
    public void V2_Customs_Routes_ShouldMatchOfficialThree_AndUseTwoPathPrefixes()
    {
        RoutesOf<IWechatPayV2CustomsService>().Should().BeEquivalentTo(CustomsRoutes,
            "because 报关三支的官方路径分属 /cgi-bin/mch/customs/ 与 /cgi-bin/mch/newcustoms/ 两个前缀，重推支不在 customs 前缀下");

        var api = typeof(IWechatPayV2CustomsService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.CustomsV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName);

        typeof(IWechatPayV2CustomsService)
            .GetCustomAttributes(typeof(AllowAnyStatusCodeAttribute), inherit: false)
            .Should().NotBeEmpty();
    }

    /// <summary>
    /// 报关族的「无 nonce_str 例外」：三支请求与应答<b>都</b>不得出现 <c>nonce_str</c>，
    /// 且申报请求还不得出现 <c>sign_type</c>（官方该页请求表就 18 行，无此字段）。
    /// </summary>
    [Fact]
    public void V2_Customs_Requests_ShouldNotCarryNonceStr()
    {
        foreach (var requestType in new[]
                 {
                     typeof(V2CustomsDeclareRequest), typeof(V2CustomsQueryRequest), typeof(V2CustomsRedeclareRequest),
                 })
        {
            JsonNames(requestType).Should().NotContain("nonce_str",
                $"{requestType.Name} 的官方请求表无 nonce_str —— 补上即与官方报文不一致，且会进签名串改变签名结果");
        }

        JsonNames(typeof(V2CustomsDeclareRequest)).Should().NotContain("sign_type",
            "because 申报页请求侧没有 sign_type（查询/重推两页才是选填）");

        // 拆单与金额面照官方：sub_order_no + fee_type + order_fee + transport_fee + product_fee 同现。
        JsonNames(typeof(V2CustomsDeclareRequest)).Should().Contain(new[]
        {
            "sub_order_no", "fee_type", "order_fee", "transport_fee", "product_fee",
        }, "because 官方「传了 sub_order_no 就认为是拆单」，五个子单字段缺一即 132011004");
    }

    /// <summary>
    /// 报关查询应答的下标键（<c>customs_$n</c> / <c>detail_$n</c> 一类）由 <c>[JsonExtensionData]</c> 承接 ——
    /// 扁平字段表 + 固定属性的组合会静默丢掉多海关申报的逐条结果。
    /// </summary>
    [Fact]
    public void V2_Customs_QueryResponse_ShouldCarryIndexedFieldsAsExtensionData()
    {
        var property = typeof(V2CustomsQueryResponse)
            .GetProperty("IndexedFields", BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull(
            "because 官方把第 n 个海关的结果编成 customs_$n 形态的键，固定属性表读不全");
        property!.GetCustomAttribute<JsonExtensionDataAttribute>().Should().NotBeNull();
        property.PropertyType.Should().Be(typeof(IDictionary<string, JsonElement>));

        // 三个「与下标无关」的汇总字段仍是固定属性，勿一并塞进袋里。
        JsonNames(typeof(V2CustomsQueryResponse)).Should().Contain(new[]
        {
            "count", "verify_department", "verify_department_trade_id",
        });
    }

    [Fact]
    public void V2_Tools_Routes_ShouldMatchOfficialTwo()
    {
        RoutesOf<IWechatPayV2ToolsService>().Should().BeEquivalentTo(ToolsRoutes,
            "because 官方路径是 /tools/shorturl（无下划线），而应答字段是 short_url（有下划线）—— 两者拼写不同不可互相「修正」");

        var api = typeof(IWechatPayV2ToolsService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.ToolsV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName);

        JsonNames(typeof(V2ShortUrlResponse)).Should().Contain("short_url");
        JsonNames(typeof(V2ShortUrlRequest)).Should().Contain("long_url");
    }

    /// <summary>付款码查询 openid 的请求表官方就 5 行，多一个字段即与官方报文不符。</summary>
    [Fact]
    public void V2_Tools_AuthCodeToOpenId_Request_ShouldBeTheOfficialFiveFields()
    {
        JsonNames(typeof(V2AuthCodeToOpenIdRequest)).Should().BeEquivalentTo(new[]
        {
            "appid", "mch_id", "auth_code", "nonce_str", "sign",
        }, "because 官方该页请求参数表恰 5 行；auth_code 是扫码后的支付授权码，属一次性凭据，SDK 不落日志");
    }

    [Fact]
    public void V2_Risk_ShouldBeHandWrittenOnApiMch()
    {
        typeof(IWechatPayV2RiskService)
            .GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false)
            .Should().BeEmpty(
                "because 违规记录查询官方「是否需要证书」为是（错误码表含 CERT_ERROR）⇒ 证书按商户绑定，只能手写");

        var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2RiskService.cs"));
        text.Should().Contain("\"/risk/getviolation\"", "because 官方路径唯一，写错即 404");
        text.Should().Contain("WechatPayV2HttpClientNames.BaseAddress, request",
            "because 官方本页给的是 api.mch，不是 /secapi/* 那一族的 api2.mch");
        text.Should().NotContain("SecureBaseAddress");

        // 风控族请求无 appid（官方表 9 行，主体标识只有 mch_id）。
        JsonNames(typeof(V2ViolationRecordQueryRequest)).Should().NotContain("appid",
            "because 官方本页请求参数表没有 appid —— 按「通用形状」补齐会改变签名串并与官方表不符");
        JsonNames(typeof(V2ViolationRecordQueryRequest)).Should().Contain("violation_records_format");
        JsonNames(typeof(V2ViolationRecordQueryResponse)).Should().Contain(new[] { "result_msg", "violation_records" });
    }

    /// <summary>模块注册面：报关/工具走生成组，风控是纯手写族（须被白名单认领）。</summary>
    [Fact]
    public void V2_CustomsToolsRisk_Modules_ShouldBindRegistrars()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Extensions/PayV2ServiceBuilder.cs"));

        builderSource.Should().Contain("AddCustomsV2WebApiHttpClient()");
        builderSource.Should().Contain("AddToolsV2WebApiHttpClient()");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2RiskService, WechatPayV2RiskService>");
        builderSource.Should().NotContain("AddRiskV2WebApiHttpClient()",
            "because RiskV2 是手写族，出现生成注册器即说明有人给它补了 [HttpClientApi]");

        WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().Contain(nameof(PayV2Module.RiskV2),
            "because 模块成员 = 生成注册组 ∪ 手写白名单，漏认领即「查不到注册器 ⇒ 静默不注册」");
    }

    [Fact]
    public void V2_CustomsToolsRisk_Services_ShouldResolvePerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(
                PayV2Module.CustomsV2, PayV2Module.ToolsV2, PayV2Module.RiskV2)));

        provider.GetRequiredService<IWechatPayV2CustomsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayV2ToolsService>().Should().NotBeNull();

        var risk = provider.GetRequiredService<IWechatPayV2RiskService>();
        risk.Should().BeOfType<WechatPayV2RiskService>();

        using (var scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IWechatPayV2RiskService>()
                .Should().BeSameAs(risk, "because 证书通道按商户复用，服务须是 Singleton");
        }

        provider.GetService<IWechatPayV2DepositService>().Should().BeNull(
            "because 只装三域时押金服务不应凭空出现");
    }

    /// <summary>本批 6 端点的路由不得与押金/基础支付族交叠。</summary>
    [Fact]
    public void V2_CustomsToolsRisk_Routes_ShouldNotOverlapOtherV2Families()
    {
        var routes = RoutesOf<IWechatPayV2CustomsService>()
            .Concat(RoutesOf<IWechatPayV2ToolsService>())
            .Append("/risk/getviolation")
            .ToList();

        routes.Should().HaveCount(6, "because 本守卫覆盖报关 3 + 工具 2 + 风控 1");
        routes.Distinct().Should().HaveCount(6);
        routes.Should().NotContain(r => r.StartsWith("/deposit/", StringComparison.Ordinal)
                                        || r.StartsWith("/pay/", StringComparison.Ordinal)
                                        || r.StartsWith("/secapi/", StringComparison.Ordinal),
            "because /deposit/* 属押金守卫、/pay/* 与 /secapi/* 属基础支付守卫");
    }

    /// <summary>报关 6 型 + 工具 4 型 + 风控 2 型须全部登记进 <c>PayV2JsonContext</c>。</summary>
    [Fact]
    public void V2_CustomsToolsRisk_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        var types = new[]
        {
            typeof(V2CustomsDeclareRequest), typeof(V2CustomsDeclareResponse),
            typeof(V2CustomsQueryRequest), typeof(V2CustomsQueryResponse),
            typeof(V2CustomsRedeclareRequest), typeof(V2CustomsRedeclareResponse),
            typeof(V2AuthCodeToOpenIdRequest), typeof(V2AuthCodeToOpenIdResponse),
            typeof(V2ShortUrlRequest), typeof(V2ShortUrlResponse),
            typeof(V2ViolationRecordQueryRequest), typeof(V2ViolationRecordQueryResponse),
        };

        types.Should().HaveCount(12, "because 6 端点全是 POST 成对 ⇒ 恰 12 型");

        var contextType = typeof(V2CustomsDeclareRequest).Assembly
            .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2.PayV2JsonContext", throwOnError: false);
        contextType.Should().NotBeNull();
        var context = (global::System.Text.Json.Serialization.JsonSerializerContext)
            contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;

        foreach (var type in types)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 未登记进 PayV2JsonContext ⇒ 重跑 scripts/GenerateJsonContext.ps1");
        }
    }

    private static List<string> RoutesOf<TInterface>()
        where TInterface : class
        => typeof(TInterface)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(r => r != null)
            .Select(r => r!)
            .ToList();

    private static List<string?> JsonNames(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .ToList();

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
