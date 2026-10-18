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
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

/// <summary>
/// 微信支付 APIv2「车主平台双族 + 乘车码双族」契约守卫（<b>8 端点 = 直连车主 3 + 服务商车主 3 + 直连乘车码 1 + 服务商乘车码 1</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-18 逐页核验证书栏、域名栏与参数表）：
/// 车主直连 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985272"/>、
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985331"/>、
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986415"/>；
/// 车主服务商 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988204"/>、
/// <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988205"/>、
/// <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988212"/>；
/// 乘车码直连 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986341"/>、
/// 乘车码服务商 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011989909"/>。</para>
/// <para>
/// <b>本守卫锁的四类易错事实</b>：
/// ① <b>八支全部免证书</b> ⇒ 整族走声明式生成（与 <c>/secapi/</c> 一族不同），
/// 一旦有人「看前缀像涉密」就改成手写族，路由载体即从特性消失；
/// ② <b>直连侧与服务商侧不同形</b>：路径段（<c>pay</c> vs <c>partnerpay</c>）、子商户字段、
/// <c>trade_scene</c> 档数、应答字段集四处差异 ⇒ 两族 DTO <b>不得</b>共用，模块分列；
/// ③ <b><c>sign_type</c> 默认值即契约面</b>（八页均写「默认为HMAC-SHA256」且不列 MD5 备选，
/// 而 APIv2 的算法判定看字段表里有没有 <c>sign_type=HMAC-SHA256</c> ⇒ 清空即按 MD5 签、必报签名错）；
/// ④ <b>乘车码应答的下标字段</b>（<c>coupon_type_$n</c> 等）只有 <c>[JsonExtensionData]</c> 能收全。
/// </para>
/// </remarks>
public class WechatPayV2VehicleTransitContractGuards
{
    private static readonly string[] VehicleRoutes =
    {
        "/vehicle/pay/notification",
        "/vehicle/pay/payapply",
        "/vehicle/pay/querystate",
    };

    private static readonly string[] PartnerVehicleRoutes =
    {
        "/vehicle/partnerpay/notification",
        "/vehicle/partnerpay/payapply",
        "/vehicle/partnerpay/querystate",
    };

    /// <summary>三支车主直连端点的路由、注册组与「免证书」形态。</summary>
    [Fact]
    public void V2_Vehicle_Routes_ShouldMatchOfficialThree_AndStayDeclarative()
    {
        RoutesOf<IWechatPayV2VehicleService>().Should().Equal(VehicleRoutes,
            "because 官方三页的接口链接栏逐支给出这三条路径（notification / payapply / querystate），顺序即接口声明顺序");

        var api = typeof(IWechatPayV2VehicleService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.VehicleV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName,
            "because 本族免证书 ⇒ 挂普通命名客户端（api.mch + 签名 Handler），不得挂证书通道");

        typeof(IWechatPayV2VehicleService)
            .GetCustomAttributes(typeof(AllowAnyStatusCodeAttribute), inherit: false)
            .Should().NotBeEmpty();
    }

    /// <summary>三支车主服务商端点：路径段是 <c>partnerpay</c>，不是 <c>pay</c>。</summary>
    [Fact]
    public void V2_PartnerVehicle_Routes_ShouldUsePartnerPaySegment()
    {
        RoutesOf<IWechatPayV2PartnerVehicleService>().Should().Equal(PartnerVehicleRoutes,
            "because 服务商侧三页的接口链接栏路径段是 /vehicle/partnerpay/，与直连侧仅这一段不同 ⇒ 抄错即 404");

        var api = typeof(IWechatPayV2PartnerVehicleService).GetCustomAttribute<HttpClientApiAttribute>()!;
        api.RegistryGroupName.Should().Be(nameof(PayV2Module.PartnerVehicleV2));
        api.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName);
    }

    /// <summary>
    /// <b>两族 DTO 不得共用</b>：服务商侧三支请求同时要求 <c>sub_appid</c> 与 <c>sub_mch_id</c>，
    /// 直连侧请求面则<b>没有</b>这两个字段。
    /// </summary>
    [Fact]
    public void V2_Vehicle_AndPartnerVehicle_RequestFields_ShouldNotBeInterchangeable()
    {
        foreach (var requestType in new[]
                 {
                     typeof(V2PartnerVehicleNotificationRequest),
                     typeof(V2PartnerVehiclePayApplyRequest),
                     typeof(V2PartnerVehicleStateQueryRequest),
                 })
        {
            JsonNames(requestType).Should().Contain(new[] { "sub_appid", "sub_mch_id" },
                $"{requestType.Name} 的服务商身份两字段官方同为必填，缺一个即拿不到子商户路由");
        }

        foreach (var requestType in new[]
                 {
                     typeof(V2VehicleNotificationRequest),
                     typeof(V2VehiclePayApplyRequest),
                     typeof(V2VehicleStateQueryRequest),
                 })
        {
            JsonNames(requestType).Should().NotContain("sub_mch_id",
                $"{requestType.Name} 是直连商户侧报文，官方表无子商户字段 —— 补上即与官方不符并改变签名串");
        }

        // 状态查询支的「三选一」在服务商侧多一个 sub_openid（官方：openid、sub_openid、plate_number 必传其一）。
        JsonNames(typeof(V2PartnerVehicleStateQueryRequest)).Should().Contain("sub_openid");
        // 直连侧入场通知无 openid（用户身份靠 plate_number 承载），服务商侧入场通知有 openid + sub_openid + space_number。
        JsonNames(typeof(V2VehicleNotificationRequest)).Should().NotContain("openid",
            "because 直连侧入场通知页请求表无 openid 行；服务商侧同支才有 openid/sub_openid/space_number");
        JsonNames(typeof(V2PartnerVehicleNotificationRequest)).Should()
            .Contain(new[] { "openid", "sub_openid", "space_number" });
    }

    /// <summary>
    /// 服务商侧「应答比请求少字段」的官方例外：入场通知的<b>应答</b>不回显 <c>sub_*</c>，
    /// 而扣款申请的应答会回显 ⇒ 两组应答 DTO 亦不可合并。
    /// </summary>
    [Fact]
    public void V2_PartnerVehicle_ResponseShapes_ShouldFollowEachOfficialPage()
    {
        JsonNames(typeof(V2PartnerVehicleNotificationResponse)).Should().NotContain("sub_mch_id",
            "because 官方该页返回表只到 mch_id，没有子商户回显 —— 按请求表补齐会得到恒 null 的假字段");

        JsonNames(typeof(V2PartnerVehiclePayApplyResponse)).Should().Contain("sub_mch_id");
        JsonNames(typeof(V2PartnerVehicleStateQueryResponse)).Should()
            .Contain(new[] { "sub_openid", "plate_number_list" });
    }

    /// <summary>
    /// 六支车主请求的 <c>sign_type</c> 默认值：官方三页 × 两侧均写「默认为HMAC-SHA256」⇒
    /// 默认值即算法开关，清空即 MD5、必然签名错。
    /// </summary>
    [Fact]
    public void V2_Vehicle_Requests_ShouldDefaultSignTypeToHmacSha256()
    {
        new V2VehicleNotificationRequest().SignType.Should().Be("HMAC-SHA256");
        new V2VehiclePayApplyRequest().SignType.Should().Be("HMAC-SHA256");
        new V2VehicleStateQueryRequest().SignType.Should().Be("HMAC-SHA256");
        new V2PartnerVehicleNotificationRequest().SignType.Should().Be("HMAC-SHA256");
        new V2PartnerVehiclePayApplyRequest().SignType.Should().Be("HMAC-SHA256");
        new V2PartnerVehicleStateQueryRequest().SignType.Should().Be("HMAC-SHA256");
    }

    /// <summary>
    /// 扣款申请支的应答<b>没有任何金额与交易状态字段</b>（官方返回表 10 行）⇒
    /// 「受理成功」不等于「扣款成功」，这条红线由字段集缺失本身锁住。
    /// </summary>
    [Fact]
    public void V2_Vehicle_PayApplyResponse_ShouldCarryNoAmountOrTradeState()
    {
        var names = JsonNames(typeof(V2VehiclePayApplyResponse));
        names.Should().Contain(new[] { "return_code", "result_code" });
        names.Should().NotContain(new[] { "total_fee", "trade_state", "transaction_id" },
            "because 本支是异步受理语义：据同步应答放行车辆即资损，官方返回表里根本没有金额与状态字段");

        var partner = JsonNames(typeof(V2PartnerVehiclePayApplyResponse));
        partner.Should().NotContain(new[] { "total_fee", "trade_state", "sub_openid" },
            "because 服务商侧扣款申请应答同样只有通用 10 行 + 子商户回显，且官方该页无 sub_openid 行");
    }

    /// <summary>入场通知与状态查询两页的「用户状态」字段<b>档数不同</b>（两档 vs 四档），故不可共用枚举。</summary>
    [Fact]
    public void V2_Vehicle_UserStateFields_ShouldBeDistinctPerPage()
    {
        // 两支都同时带 user_state 与 user_status（官方两页均并列给出），形态一致但取值集不同 —— 断言字段名不漂移即可。
        JsonNames(typeof(V2VehicleNotificationResponse)).Should()
            .Contain(new[] { "user_state", "user_status", "deduct_mode" });
        JsonNames(typeof(V2VehicleStateQueryResponse)).Should()
            .Contain(new[] { "user_state", "user_status", "deduct_mode", "path" });

        // path 只在状态查询支出现（欠费/未授权时官方回引导入口）。
        JsonNames(typeof(V2VehiclePayApplyResponse)).Should().NotContain("path");
    }

    /// <summary>两支乘车码查询的路由与注册组。</summary>
    [Fact]
    public void V2_Transit_Routes_ShouldMatchOfficialTwo()
    {
        RoutesOf<IWechatPayV2TransitService>().Should().Equal(new[] { "/transit/queryorder" },
            "because 直连乘车码查询页的接口链接栏是 /transit/queryorder（无 partner 段）");
        RoutesOf<IWechatPayV2PartnerTransitService>().Should().Equal(new[] { "/transit/partnerpay/queryorder" },
            "because 服务商侧同族路径多一段 partnerpay");

        typeof(IWechatPayV2TransitService).GetCustomAttribute<HttpClientApiAttribute>()!
            .RegistryGroupName.Should().Be(nameof(PayV2Module.TransitV2));
        typeof(IWechatPayV2PartnerTransitService).GetCustomAttribute<HttpClientApiAttribute>()!
            .RegistryGroupName.Should().Be(nameof(PayV2Module.PartnerTransitV2));
    }

    /// <summary>乘车码请求的 <c>sign_type</c> 默认 HMAC-SHA256；服务商侧多 <c>contract_id</c> 与 <c>version</c>。</summary>
    [Fact]
    public void V2_Transit_Requests_ShouldCarryOfficialFields()
    {
        new V2TransitQueryOrderRequest().SignType.Should().Be("HMAC-SHA256",
            "because 官方该页 sign_type 必填栏为否、默认即 HMAC-SHA256 ⇒ 默认值属契约面");

        var partner = JsonNames(typeof(V2PartnerTransitQueryOrderRequest));
        partner.Should().Contain(new[] { "sub_appid", "sub_mch_id", "contract_id", "version" });

        // 二选一：transaction_id 与 out_trade_no 同时存在于两支请求面，SDK 不做取舍校验（官方口径）。
        JsonNames(typeof(V2TransitQueryOrderRequest)).Should().Contain(new[] { "transaction_id", "out_trade_no" });
    }

    /// <summary>
    /// <b>两族应答的状态档数不同</b>：直连侧 <c>trade_state</c> 三档、服务商侧四档（多 <c>REFUND</c>），
    /// 且服务商侧独有 <c>user_repaid</c> ⇒ 两型不得合并。
    /// </summary>
    [Fact]
    public void V2_Transit_Responses_ShouldKeepThePartnerSuperset()
    {
        var direct = JsonNames(typeof(V2TransitQueryOrderResponse));
        var partner = JsonNames(typeof(V2PartnerTransitQueryOrderResponse));

        direct.Should().Contain(new[] { "trade_state", "trade_state_desc", "bank_type", "coupon_count" });
        partner.Should().Contain(new[]
        {
            "sub_appid", "sub_mch_id", "sub_openid", "contract_id", "user_repaid", "trade_state",
        }, "because 服务商侧页返回表比直连侧多这五项，合并即丢字段");

        partner.Should().Contain("user_repaid",
            "because 官方 version=2.0 时才会出现 bank_type=BPA（垫资）与 user_repaid ⇒ 服务商侧应答是直连侧的超集");

        // 服务商侧请求带 version（接口版本号），应答表却不回显它 —— 按请求表补齐会得到恒 null 的假字段。
        JsonNames(typeof(V2PartnerTransitQueryOrderRequest)).Should().Contain("version");
        partner.Should().NotContain("version");
    }

    /// <summary>
    /// 代金券下标字段（<c>coupon_type_$n</c> / <c>coupon_id_$n</c> / <c>coupon_fee_$n</c>，$n 自 0 起、
    /// 条数取 <c>coupon_count</c>）由 <c>[JsonExtensionData]</c> 承接 —— 两族各一份，不得只给一族。
    /// </summary>
    [Fact]
    public void V2_Transit_Responses_ShouldCarryIndexedFieldsAsExtensionData()
    {
        foreach (var responseType in new[] { typeof(V2TransitQueryOrderResponse), typeof(V2PartnerTransitQueryOrderResponse) })
        {
            var property = responseType.GetProperty("IndexedFields", BindingFlags.Public | BindingFlags.Instance);
            property.Should().NotBeNull(
                $"{responseType.Name} 缺 IndexedFields ⇒ coupon_type_0/coupon_id_0 这类下标键会静默丢失");
            property!.GetCustomAttribute<JsonExtensionDataAttribute>().Should().NotBeNull();
            property.PropertyType.Should().Be(typeof(IDictionary<string, JsonElement>));
        }

        // 汇总字段仍是固定属性，勿一并塞进袋里。
        JsonNames(typeof(V2TransitQueryOrderResponse)).Should().Contain(new[] { "coupon_fee", "coupon_count" });
    }

    /// <summary>乘车码应答的三闸口径：<c>return_code</c>／<c>result_code</c>／<c>trade_state</c> 三层缺一不可。</summary>
    [Fact]
    public void V2_Transit_Responses_ShouldExposeTheThreeGates()
    {
        foreach (var responseType in new[] { typeof(V2TransitQueryOrderResponse), typeof(V2PartnerTransitQueryOrderResponse) })
        {
            JsonNames(responseType).Should().Contain(new[] { "return_code", "result_code", "trade_state" },
                $"{responseType.Name} 的官方成功判定是三层闸门，少判一层即把 FAIL 应答当成功");
        }
    }

    /// <summary>模块注册面：四族全部走生成注册组，且不得被误认领为手写族。</summary>
    [Fact]
    public void V2_VehicleTransit_Modules_ShBindGeneratedRegistrars()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay.V2", "Extensions/PayV2ServiceBuilder.cs"));

        foreach (var module in new[]
                 {
                     nameof(PayV2Module.VehicleV2), nameof(PayV2Module.PartnerVehicleV2),
                     nameof(PayV2Module.TransitV2), nameof(PayV2Module.PartnerTransitV2),
                 })
        {
            builderSource.Should().Contain($"[PayV2Module.{module}]");
            builderSource.Should().Contain($"Add{module}WebApiHttpClient()",
                $"{module} 的端点免证书 ⇒ 必须由源生成客户端承载");
            WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().NotContain(module,
                $"{module} 不属手写族名单（证书按商户绑定才需要手写通道，本族不需要）");
        }
    }

    /// <summary>本批 8 端点的路由不得与押金、红包、乘车码两族互相交叠。</summary>
    [Fact]
    public void V2_VehicleTransit_Routes_ShouldNotOverlapOtherV2Families()
    {
        var routes = RoutesOf<IWechatPayV2VehicleService>()
            .Concat(RoutesOf<IWechatPayV2PartnerVehicleService>())
            .Concat(RoutesOf<IWechatPayV2TransitService>())
            .Concat(RoutesOf<IWechatPayV2PartnerTransitService>())
            .ToList();

        routes.Should().HaveCount(8, "because 本守卫覆盖车主 3+3 与乘车码 1+1");
        routes.Distinct().Should().HaveCount(8);
        routes.Should().OnlyContain(r => r.StartsWith("/vehicle/", StringComparison.Ordinal)
                                         || r.StartsWith("/transit/", StringComparison.Ordinal),
            "because 两域路径前缀即域边界；出现别的前缀说明路由被写到了别的族上");
    }

    /// <summary>16 型（8 端点成对）须全部登记进 <c>PayV2JsonContext</c>。</summary>
    [Fact]
    public void V2_VehicleTransit_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        var types = new[]
        {
            typeof(V2VehicleNotificationRequest), typeof(V2VehicleNotificationResponse),
            typeof(V2VehiclePayApplyRequest), typeof(V2VehiclePayApplyResponse),
            typeof(V2VehicleStateQueryRequest), typeof(V2VehicleStateQueryResponse),
            typeof(V2PartnerVehicleNotificationRequest), typeof(V2PartnerVehicleNotificationResponse),
            typeof(V2PartnerVehiclePayApplyRequest), typeof(V2PartnerVehiclePayApplyResponse),
            typeof(V2PartnerVehicleStateQueryRequest), typeof(V2PartnerVehicleStateQueryResponse),
            typeof(V2TransitQueryOrderRequest), typeof(V2TransitQueryOrderResponse),
            typeof(V2PartnerTransitQueryOrderRequest), typeof(V2PartnerTransitQueryOrderResponse),
        };

        types.Should().HaveCount(16, "because 8 端点全是 POST 成对 ⇒ 恰 16 型");

        var contextType = typeof(V2VehicleNotificationRequest).Assembly
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

    /// <summary>四族服务可解析，且只装车主时不应凭空多出乘车码服务（模块隔离）。</summary>
    [Fact]
    public void V2_VehicleTransit_Services_ShouldResolvePerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(
                PayV2Module.VehicleV2, PayV2Module.PartnerVehicleV2,
                PayV2Module.TransitV2, PayV2Module.PartnerTransitV2)));

        provider.GetRequiredService<IWechatPayV2VehicleService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayV2PartnerVehicleService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayV2TransitService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayV2PartnerTransitService>().Should().NotBeNull();

        using var vehicleOnly = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.VehicleV2)));

        vehicleOnly.GetService<IWechatPayV2TransitService>().Should().BeNull(
            "because 只装车主模块时乘车码服务不应出现");
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
