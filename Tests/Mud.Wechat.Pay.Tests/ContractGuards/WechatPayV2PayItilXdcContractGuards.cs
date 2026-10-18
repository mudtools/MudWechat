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
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

/// <summary>
/// 微信支付 APIv2「交易信息上报（<c>/payitil/report</c>）+ 沙箱取密钥（<c>/xdc/…</c>）」契约守卫（<b>2 端点</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-18 逐页核验）：
/// 交易信息上报 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935222"/>；
/// 沙箱取 <c>sandbox_signkey</c> 是支付验收指引页内的一节
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011984810"/>（该端点<b>没有</b>独立文档页）。</para>
/// <para>
/// <b>两族的共同点</b>：应答<b>都不带 <c>sign</c></b> ⇒ 不可验签（收发器的放行条件是「应答无 sign」这一报文事实，
/// 不是两族的例外声明）；两族请求的 <c>sign_type</c> 缺省档也不同 ——
/// 上报页官方默认 <b>MD5</b>（故 DTO <b>不设</b>默认值），验收页那一支请求面<b>根本没有</b> <c>sign_type</c> 字段
/// （⇒ 恒按 MD5）。这两处是最容易被「按通用形状补齐 HMAC-SHA256」改错的面，故合一守卫集中锁定。
/// </para>
/// <para><b>另一个共同点</b>：两支都<b>免证书</b> ⇒ 走声明式管道，与 <c>/secapi/</c> 一族分属两条通道。</para>
/// </remarks>
public class WechatPayV2PayItilXdcContractGuards
{
    /// <summary>上报支的路由与注册组。</summary>
    [Fact]
    public void V2_PayItil_Route_ShouldMatchOfficialPage()
    {
        RoutesOf<IWechatPayV2PayItilService>().Should().Equal(new[] { "/payitil/report" },
            "because 官方本页接口链接栏为 https://api.mch.weixin.qq.com/payitil/report（无 secapi 段、无下划线）");

        var api = typeof(IWechatPayV2PayItilService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.PayItilV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName,
            "because 本支免证书 ⇒ 挂普通命名客户端（api.mch）");
    }

    /// <summary>
    /// <b>官方字段名自带尾下划线</b>：<c>execute_time_</c>（官方原文注明「该参数最后带有下划线，
    /// 参数设计如此，非文档问题」）⇒ 照抄不得「修正」成 <c>execute_time</c>。
    /// </summary>
    [Fact]
    public void V2_PayItil_Request_ShouldKeepTheTrailingUnderscoreFieldName()
    {
        var names = JsonNames(typeof(V2ReportTransactionRequest));

        names.Should().Contain("execute_time_",
            "because 官方参数表逐字写 execute_time_ 并注明非文档笔误；改名即与官方报文不符，该字段会被丢弃");
        names.Should().NotContain("execute_time",
            "because 去掉尾下划线是一个「看起来更正确」的错误拼写，XML 节点名与签名串都会跟着变");

        names.Should().Contain(new[] { "interface_url", "user_ip", "time" },
            "because 本支上报的是交易执行现场（接口地址、耗时、发起方 IP、时间），三字段是页面上的核心行");

        // 官方「签名类型，默认为MD5」⇒ 本 DTO 不设默认值（缺字段即按 MD5 签），与 HMAC-SHA256 各族区分。
        names.Should().Contain("sign_type", "because 本页请求表确有 sign_type 行（默认 MD5，可显式改档）");
        new V2ReportTransactionRequest().SignType.Should().BeNull(
            "because 官方缺省档是 MD5：把默认值钉成 HMAC-SHA256 会静默换算法，与本页契约面不符");
    }

    /// <summary>上报应答只有三行、<b>没有 sign</b> ⇒ 不可验签；官方错误码栏写「无」。</summary>
    [Fact]
    public void V2_PayItil_Response_ShouldHaveNoSignAndNoErrorCodes()
    {
        var names = JsonNames(typeof(V2ReportTransactionResponse));

        names.Should().Equal(new[] { "return_code", "return_msg", "result_code" },
            "because 官方返回参数表恰此三行；本支是旁路遥测，字段集比交易族窄得多");

        names.Should().NotContain("sign",
            "because 应答无 sign ⇒ 收发器按「应答不带 sign」放行；若有人补一个 sign 属性，验签会把每次上报都判成失败");

        names.Should().NotContain(new[] { "err_code", "openid", "transaction_id" },
            "because 本页没有错误码表（官方「错误码」栏写无），也不回订单标识 —— 上报失败不影响主链路");
    }

    /// <summary>沙箱取 key 支的路由与注册组（路径两段式前缀 <c>/xdc/apiv2getsignkey/</c>）。</summary>
    [Fact]
    public void V2_Xdc_Route_ShouldMatchOfficialSandboxPath()
    {
        RoutesOf<IWechatPayV2XdcService>().Should()
            .Equal(new[] { "/xdc/apiv2getsignkey/sign/getsignkey" },
                "because 官方验收指引页给出的沙箱取 key 地址逐字为此路径（apiv2getsignkey 段是沙箱专用，勿按常规形状简化）");

        var api = typeof(IWechatPayV2XdcService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.XdcV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName);
    }

    /// <summary>
    /// 沙箱请求面<b>只有三行</b>（<c>mch_id</c>／<c>nonce_str</c>／<c>sign</c>）：
    /// 无 <c>appid</c>、无 <c>sign_type</c> ⇒ 恒按 MD5 签。
    /// </summary>
    [Fact]
    public void V2_Xdc_Request_ShouldBeTheOfficialThreeFields()
    {
        JsonNames(typeof(V2GetSandboxSignKeyRequest)).Should().Equal(new[] { "mch_id", "nonce_str", "sign" },
            "because 官方本页请求参数表恰 3 行；补 appid 或 sign_type 都会改变签名串并与官方报文不符");

        typeof(V2GetSandboxSignKeyRequest)
            .GetProperty("SignType", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull(
                "because 请求面没有 sign_type ⇒ 签名器按 MD5 缺省档计算；补一个默认值为 HMAC-SHA256 的属性会静默换算法");
    }

    /// <summary>沙箱应答无 <c>sign</c>、也无 <c>result_code</c>；<c>sandbox_signkey</c> 是凭据。</summary>
    [Fact]
    public void V2_Xdc_Response_ShouldCarrySandboxSignKeyWithoutSign()
    {
        var names = JsonNames(typeof(V2GetSandboxSignKeyResponse));

        names.Should().Equal(new[] { "return_code", "return_msg", "mch_id", "sandbox_signkey" },
            "because 官方本页返回表恰此四行 —— 注意没有 result_code，判成功只能看 return_code");

        names.Should().NotContain("sign",
            "because 应答无 sign ⇒ 不可验签（与上报支同一放行事实），且本页无错误码表");
        names.Should().NotContain("result_code",
            "because 本页返回表没有 result_code 行；按通用双层闸门建模会恒判失败");
    }

    /// <summary>两族模块注册面：都是生成注册组，不得被认领为手写证书族。</summary>
    [Fact]
    public void V2_PayItilXdc_Modules_ShouldBindGeneratedRegistrars()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Extensions/PayV2ServiceBuilder.cs"));

        foreach (var module in new[] { nameof(PayV2Module.PayItilV2), nameof(PayV2Module.XdcV2) })
        {
            builderSource.Should().Contain($"[PayV2Module.{module}]");
            builderSource.Should().Contain($"Add{module}WebApiHttpClient()",
                $"{module} 免证书 ⇒ 由源生成客户端承载");
            WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().NotContain(module);
        }
    }

    /// <summary>4 型（2 端点成对）须登记进 <c>PayV2JsonContext</c>。</summary>
    [Fact]
    public void V2_PayItilXdc_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        var types = new[]
        {
            typeof(V2ReportTransactionRequest), typeof(V2ReportTransactionResponse),
            typeof(V2GetSandboxSignKeyRequest), typeof(V2GetSandboxSignKeyResponse),
        };

        var contextType = typeof(V2ReportTransactionRequest).Assembly
            .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2.PayV2JsonContext", throwOnError: false);
        contextType.Should().NotBeNull();
        var context = (JsonSerializerContext)
            contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;

        foreach (var type in types)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 未登记进 PayV2JsonContext ⇒ 重跑 scripts/GenerateJsonContext.ps1");
        }
    }

    /// <summary>两支服务可解析；未注册该模块时应凭空不出现。</summary>
    [Fact]
    public void V2_PayItilXdc_Services_ShouldResolvePerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(
                PayV2Module.PayItilV2, PayV2Module.XdcV2)));

        provider.GetRequiredService<IWechatPayV2PayItilService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayV2XdcService>().Should().NotBeNull();

        using var baseOnly = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.BasePayV2)));

        baseOnly.GetService<IWechatPayV2XdcService>().Should().BeNull(
            "because 只装基础支付时不应凭空多出沙箱取 key 服务");
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
