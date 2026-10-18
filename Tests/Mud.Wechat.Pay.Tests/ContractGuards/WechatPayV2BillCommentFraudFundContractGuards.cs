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
/// 微信支付 APIv2「订单评价拉取 / 防欺诈取公钥 / 按日期查自动提现」三支<b>需证书手写族</b>契约守卫（<b>3 端点</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-18 逐页核验证书栏、域名栏与返回表）：
/// 订单评价拉取 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4014326528"/>、
/// 防欺诈取公钥 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011941097"/>、
/// 查询提现状态 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115346"/>。</para>
/// <para>
/// <b>本守卫锁的三件独立事实</b>（三支各占其一，是「按红包族形状对齐」最容易改错的三面）：
/// ① <b>域名</b>：<c>FraudV2</c> 挂<b>第四个</b>闭集域名 <c>fraud.mch</c>（官方本页接口链接栏逐字给出），
/// 另两支挂 <c>api.mch</c> —— 三支都<b>不</b>挂 <c>api2.mch</c>；
/// ② <b>出口形态</b>：<c>BillCommentV2</c> 的应答不是 XML 而是文本表 ⇒ 只能走返回原文的 <c>SendAsync</c>，
/// 服务签名返回 <see cref="string"/>；
/// ③ <b>应答可验签性</b>：<c>FraudV2</c> 应答无 <c>sign</c>（放行）、<c>FundV2</c> 应答有 <c>sign</c> 与
/// <c>sign_type</c>（常规验签）⇒ 同一出口的两种报文事实，不能靠「本族特判」表达。
/// </para>
/// <para><b>三族的共同点</b>：官方「是否需要证书」逐页为「需要」⇒ 均为手写族、无生成注册组，
/// 须被 <c>WechatPayV2CertFamilyContractGuards.HandWrittenModules</c> 认领（漏认领即「查不到注册器 ⇒ 静默不注册」）。</para>
/// </remarks>
public class WechatPayV2BillCommentFraudFundContractGuards
{
    /// <summary>三族接口 ⇒ 实现类源文件（路由与域名在此逐支锁定）。</summary>
    private static readonly (Type Interface, Type Impl, string SourceFile, string Route, string DomainNeedle)[] Families =
    {
        (typeof(IWechatPayV2BillCommentService), typeof(WechatPayV2BillCommentService),
            "Secure/WechatPayV2BillCommentService.cs", "/billcommentsp/batchquerycomment",
            "WechatPayV2HttpClientNames.BaseAddress, request"),
        (typeof(IWechatPayV2FraudService), typeof(WechatPayV2FraudService),
            "Secure/WechatPayV2FraudService.cs", "/risk/getpublickey",
            "WechatPayV2HttpClientNames.FraudBaseAddress, request"),
        (typeof(IWechatPayV2FundService), typeof(WechatPayV2FundService),
            "Secure/WechatPayV2FundService.cs", "/fund/queryautowithdrawbydate",
            "WechatPayV2HttpClientNames.BaseAddress, request"),
    };

    /// <summary>三支必须是<b>无 <c>[HttpClientApi]</c></b> 的普通公共接口（证书按商户绑定，不走源生成）。</summary>
    [Fact]
    public void V2_BillCommentFraudFund_Interfaces_ShouldBePlainPublicWithoutHttpClientApi()
    {
        foreach (var (interfaceType, _, _, _, _) in Families)
        {
            interfaceType.IsPublic.Should().BeTrue($"{interfaceType.Name} 是宿主直接注入的公开面");
            interfaceType.GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false)
                .Should().BeEmpty(
                    $"{interfaceType.Name} 承载官方「需要证书」的端点：证书按商户绑定 ⇒ 不得挂命名客户端");
        }
    }

    /// <summary>逐支锁定路由与域名实参位（源文本是这三族唯一的路由契约载体）。</summary>
    [Fact]
    public void V2_BillCommentFraudFund_EachMethod_ShouldBindOfficialRouteAndDomain()
    {
        foreach (var (interfaceType, _, sourceFile, route, domainNeedle) in Families)
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", sourceFile));

            text.Should().Contain($"\"{route}\"",
                $"{interfaceType.Name} 的官方路径是 {route} —— 写错即 404，且本族无特性载体可校验");

            Occurrences(text, domainNeedle).Should().Be(1,
                $"{interfaceType.Name} 的域名实参位必须逐支显式钉住（{domainNeedle}），不靠默认值");
        }
    }

    /// <summary>
    /// <b>域名口径</b>：三支都不得出现在 <c>api2.mch</c> 上；<c>/risk/</c> 前缀里
    /// 「取公钥挂 fraud.mch、违规记录挂 api.mch」这一分裂必须同时成立。
    /// </summary>
    [Fact]
    public void V2_BillCommentFraudFund_Domains_ShouldNotUseApi2Mch()
    {
        foreach (var (interfaceType, _, sourceFile, _, _) in Families)
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", sourceFile));
            text.Should().NotContain("SecureBaseAddress",
                $"{interfaceType.Name} 的官方域名不是 api2.mch；「顺手对齐」成证书域名会打到无此路由的域名上");
        }

        var fraud = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2FraudService.cs"));
        fraud.Should().Contain("FraudBaseAddress",
            "because 防欺诈取公钥页的接口链接栏逐字印 https://fraud.mch.weixin.qq.com/risk/getpublickey");

        var risk = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2RiskService.cs"));
        risk.Should().Contain("/risk/getviolation")
            .And.NotContain("FraudBaseAddress",
                "because 同为 /risk/ 前缀的违规记录查询挂 api.mch —— 前缀不能推域名，两处必须各自钉住");

        WechatPayV2HttpClientNames.FraudBaseAddress.Should().Be("https://fraud.mch.weixin.qq.com");
    }

    /// <summary>
    /// <b>出口形态</b>：账单评论走原文出口（返回 <see cref="string"/>），另两支走 DTO 出口；
    /// 三支各自<b>只有一处</b>出口调用 ⇒ 不得出现第二套收发路径。
    /// </summary>
    [Fact]
    public void V2_BillCommentFraudFund_Exits_ShouldMatchResponseFormat()
    {
        var comment = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2BillCommentService.cs"));
        comment.Should().Contain("SendAsync(",
            "because 官方成功应答是逐行以反引号开头的文本表（首行为下一批 offset），不是 XML ⇒ 反序列化会吞掉整张表");
        comment.Should().NotContain("_invoker.PostAsync",
            "because 本族没有应答 DTO：为它补一个 DTO 出口等于把文本表当 XML 解");

        typeof(IWechatPayV2BillCommentService)
            .GetMethod("BatchQueryCommentAsync")!.ReturnType
            .Should().Be(typeof(Task<string>),
                "because 公开签名返回原文 string，失败 XML 信封亦原样交回由调用方判别（与 /pay/downloadbill 同口径）");

        // 反欺诈与资金两支是 XML 应答 ⇒ DTO 出口。
        File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2FraudService.cs"))
            .Should().Contain("PostAsync<");
        File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Secure/WechatPayV2FundService.cs"))
            .Should().Contain("PostAsync<");
    }

    /// <summary>账单评论请求面：官方仅支持 HMAC-SHA256 ⇒ 默认值是契约面；offset 游标与 limit 上限照官方。</summary>
    [Fact]
    public void V2_BillComment_Request_ShouldPinHmacDefaultAndCursorFields()
    {
        JsonNames(typeof(V2BatchQueryCommentRequest)).Should().Equal(new[]
        {
            "appid", "mch_id", "nonce_str", "sign_type", "begin_time", "end_time", "offset", "limit", "sign",
        }, "because 官方本页请求参数表恰此九行，顺序即表序 —— 补字段会改变签名串");

        new V2BatchQueryCommentRequest().SignType.Should().Be("HMAC-SHA256",
            "because 官方「目前仅支持HMAC-SHA256，默认就是HMAC-SHA256」：留空即按 MD5 签，而本支不支持 MD5 ⇒ 必 SIGNERROR");
    }

    /// <summary>取公钥请求<b>无 appid</b>、<c>sign_type</c> 无默认值（官方双档并存）；应答无 sign。</summary>
    [Fact]
    public void V2_Fraud_PublicKey_ShouldHaveNoAppIdAndNoResponseSign()
    {
        var names = JsonNames(typeof(V2GetPublicKeyRequest));
        names.Should().NotContain("appid",
            "because 本页请求表只有 mch_id / nonce_str / sign_type / sign 四行；补 appid 即与官方报文不符");
        names.Should().Contain("sign_type",
            "because 官方「支持HMAC-SHA256和MD5」⇒ 字段存在但两档皆可，本 DTO 不设默认值（示例页为 MD5）");

        new V2GetPublicKeyRequest().SignType.Should().BeNull(
            "because 双档端点钉任何一档默认值都是替商户做算法决定");

        JsonNames(typeof(V2GetPublicKeyResponse)).Should().NotContain("sign",
            "because 官方返回表无 sign 行 ⇒ 应答不可验签；收发器按「应答不带 sign」放行，本族不需要特判");

        JsonNames(typeof(V2GetPublicKeyResponse)).Should().Contain("pub_key",
            "because 产物是 PKCS#1 形态的 RSA 公钥（付款到银行卡的 enc_bank_no / enc_true_name 加密前置）；"
            + "公钥本身非机密，但异常消息仍不回显其内容（PAY-B7 同线口径）");
    }

    /// <summary>提现查询请求<b>无 appid</b> 且 <c>mch_id</c>+<c>sub_mch_id</c> 同必填；应答<b>带 sign</b> ⇒ 常规验签。</summary>
    [Fact]
    public void V2_Fund_AutoWithdraw_ShouldBePartnerShapedAndVerifiable()
    {
        var request = JsonNames(typeof(V2AutoWithdrawQueryRequest));
        request.Should().NotContain("appid",
            "because 本支面向服务商侧小微商户，官方请求表无 appid / sub_appid 两行");
        request.Should().Contain(new[] { "mch_id", "sub_mch_id", "date", "sign_type" });

        new V2AutoWithdrawQueryRequest().SignType.Should().Be("HMAC-SHA256",
            "because 官方「目前支持HMAC-SHA256。该字段需参与签名sign的计算」⇒ 既是算法开关又是签名入参");

        var response = JsonNames(typeof(V2AutoWithdrawQueryResponse));
        response.Should().Contain(new[] { "sign", "sign_type" },
            "because 与反欺诈支相反：本页返回表有 sign ⇒ 走常规应答验签");
        response.Should().Contain(new[] { "withdraw_status", "fail_reason", "withdraw_id", "amount" });
    }

    /// <summary>模块注册面：三支均为手写族（挂 <c>TryAddSingleton</c>、无生成注册器、被白名单认领）。</summary>
    [Fact]
    public void V2_BillCommentFraudFund_Modules_ShouldBindSingletonServicesInBuilder()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Extensions/PayV2ServiceBuilder.cs"));

        foreach (var module in new[]
                 {
                     nameof(PayV2Module.BillCommentV2), nameof(PayV2Module.FraudV2), nameof(PayV2Module.FundV2),
                 })
        {
            builderSource.Should().Contain($"[PayV2Module.{module}]", $"because 成员无注册器即「静默不注册」");
            builderSource.Should().NotContain($"Add{module}WebApiHttpClient()",
                $"{module} 是手写族，出现生成注册器即说明证书族被误挂到声明式管道上");
            WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().Contain(module,
                "because 模块成员 = 生成注册组 ∪ 手写白名单，漏认领即「查不到注册器 ⇒ 静默不注册」");
        }

        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2BillCommentService, WechatPayV2BillCommentService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2FraudService, WechatPayV2FraudService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2FundService, WechatPayV2FundService>");
    }

    /// <summary>DI 可解析 + 跨 scope 同一实例；只装基础支付时三族服务不应凭空出现。</summary>
    [Fact]
    public void V2_BillCommentFraudFund_Services_ShouldResolveAsSingletonsPerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(
                PayV2Module.BillCommentV2, PayV2Module.FraudV2, PayV2Module.FundV2)));

        var comment = provider.GetRequiredService<IWechatPayV2BillCommentService>();
        var fraud = provider.GetRequiredService<IWechatPayV2FraudService>();
        var fund = provider.GetRequiredService<IWechatPayV2FundService>();

        comment.Should().BeOfType<WechatPayV2BillCommentService>();
        fraud.Should().BeOfType<WechatPayV2FraudService>();
        fund.Should().BeOfType<WechatPayV2FundService>();

        using (var scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IWechatPayV2FraudService>()
                .Should().BeSameAs(fraud, "because 证书通道按 (商户, 域名) 复用，服务须是 Singleton");
        }

        using var baseOnly = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.BasePayV2)));

        baseOnly.GetService<IWechatPayV2FundService>().Should().BeNull(
            "because 未注册 FundV2 模块就不应凭空多出提现查询服务");
        baseOnly.GetRequiredService<WechatPayV2SecureInvoker>().Should().NotBeNull(
            "because 收发器属底座，单独注册 BasePayV2 也必须自洽");
    }

    /// <summary>8 型（3 端点：评论族无应答 DTO ⇒ 9−1=8）须登记进 <c>PayV2JsonContext</c>。</summary>
    [Fact]
    public void V2_BillCommentFraudFund_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        var types = new[]
        {
            typeof(V2BatchQueryCommentRequest),
            typeof(V2GetPublicKeyRequest), typeof(V2GetPublicKeyResponse),
            typeof(V2AutoWithdrawQueryRequest), typeof(V2AutoWithdrawQueryResponse),
        };

        types.Should().HaveCount(5, "because 三支里只有账单评论族没有应答 DTO（应答是文本表）");

        var contextType = typeof(V2BatchQueryCommentRequest).Assembly
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

    private static List<string?> JsonNames(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .ToList();

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
