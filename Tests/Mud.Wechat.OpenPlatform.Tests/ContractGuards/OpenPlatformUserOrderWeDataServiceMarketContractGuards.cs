// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Mud.HttpUtils;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Abstractions.Contracts;
using Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;
using Mud.Wechat.OpenPlatform.DataModels.UserOrder;
using Mud.Wechat.OpenPlatform.DataModels.WeData;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线 2026-10-18 新增三域契约守卫：小程序购物订单（<c>/user-order/*</c>，7 端点）、
/// We 分析（<c>/wedata/*</c>，7 端点，双令牌面）、第三方服务市场（<c>/wxa/servicemarket/*</c>，3 端点）。
/// </summary>
/// <remarks>
/// <para>
/// 三域均由小程序线守卫 NC2 判归开放平台线（<c>Src/MiniProgram</c> 零声明），本守卫为其本线镜像：
/// 锁定路由 / 动词 / 令牌语义 / 三处注册一致 / DTO 上下文登记。
/// </para>
/// <para>
/// <b>服务市场登录验证判非契约</b>：官方「登录验证」页明写「本接口不支持第三方平台调用」，
/// 其令牌面为服务商自有 <c>access_token</c>（非本线 component / authorizer 两键系）——
/// 故 <c>login_auth</c> 不建接口、不声明路由，由本守卫 SM2 锁定 Src/ 零声明
/// （追踪号 <c>SM-LOGINAUTH-NONCONTRACT-01</c>），恒不计入端点总数。
/// </para>
/// </remarks>
public class OpenPlatformUserOrderWeDataServiceMarketContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformUserOrderService).Assembly;

    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformUserOrderKey).Assembly;

    private static readonly (string Method, string Route)[] UserOrderEndpoints =
    {
        ("POST", "/user-order/orders"),
        ("POST", "/user-order/orders/shippings"),
        ("POST", "/user-order/combine-orders"),
        ("POST", "/user-order/combine-orders/shippings"),
        ("POST", "/user-order/shoppinginfo/verify"),
        ("POST", "/user-order/orders-permission/open"),
        ("POST", "/user-order/orders-permission/confirm"),
    };

    private static readonly (string Method, string Route)[] WeDataConfigEndpoints =
    {
        ("POST", "/wedata/wedata_get_login_config"),
        ("POST", "/wedata/wedata_set_login_config"),
    };

    private static readonly (string Method, string Route)[] WeDataUserEndpoints =
    {
        ("POST", "/wedata/wedata_get_perm_list"),
        ("POST", "/wedata/wedata_set_user_perm"),
        ("POST", "/wedata/wedata_query_bind_list"),
        ("POST", "/wedata/wedata_unbind_user"),
        ("POST", "/wedata/wedata_login"),
    };

    private static readonly (string Method, string Route)[] ServiceMarketEndpoints =
    {
        ("POST", "/wxa/servicemarket/service/get_service_buyer_list"),
        ("POST", "/wxa/servicemarket/service/get_service_buyer"),
        ("POST", "/wxa/servicemarket/get_paid_order_list"),
    };

    /// <summary>UD1：购物订单域 7 端点（恒 POST）逐条锁定。</summary>
    [Fact]
    public void UserOrderEndpoints_ShouldMatchOfficialRoutes_WhenEnumerated()
    {
        var actual = RoutesOf(typeof(IWechatOpenPlatformUserOrderService));

        actual.Should().HaveCount(7, "购物订单域恰 7 端点（官方 shopping-orders 树）——少一条即缺口或扫描空跑");
        actual.Should().BeEquivalentTo(UserOrderEndpoints,
            because: "官方上传购物详情 / 物流 / 合单 / 验证 / 产品权限七页正文的请求地址逐一锁定");
    }

    /// <summary>UD2：购物订单域令牌语义——授权方令牌、Query 注入、参数名 access_token、注册组 UserOrder。</summary>
    [Fact]
    public void UserOrderTokenSemantics_ShouldBeAuthorizerQuery()
    {
        AssertToken(typeof(IWechatOpenPlatformUserOrderService),
            OpenPlatformTokenTypes.AuthorizerAccessToken, "UserOrder");
    }

    /// <summary>WD1：WeData 双接口合计 7 端点（恒 POST）逐条锁定。</summary>
    [Fact]
    public void WeDataEndpoints_ShouldMatchOfficialRoutes_WhenEnumerated()
    {
        RoutesOf(typeof(IWechatOpenPlatformWeDataService)).Should().BeEquivalentTo(WeDataConfigEndpoints,
            because: "登录配置两页（官方 weDataGet/SetLoginConfig）的请求地址必须逐字一致");

        RoutesOf(typeof(IWechatOpenPlatformWeDataUserService)).Should().BeEquivalentTo(WeDataUserEndpoints,
            because: "权限列表 / 设置权限 / 绑定列表 / 解绑 / 登录五页（官方 we-analysis 树）的请求地址必须逐字一致");

        // 两接口共用注册组 WeData，但令牌归属相反 ⇒ 必须拆两支接口（同 Ams/AmsShare 先例）。
        typeof(IWechatOpenPlatformWeDataService)
            .GetCustomAttributes<HttpClientApiAttribute>(false).Single().RegistryGroupName
            .Should().Be("WeData");
        typeof(IWechatOpenPlatformWeDataUserService)
            .GetCustomAttributes<HttpClientApiAttribute>(false).Single().RegistryGroupName
            .Should().Be("WeData");
    }

    /// <summary>
    /// WD2：WeData 令牌面按端点二分——登录配置 = component_access_token，用户五端点 = authorizer_access_token。
    /// </summary>
    /// <remarks>
    /// 2026-10-18 逐页核验：<c>weDataGetLoginConfig</c> / <c>weDataSetLoginConfig</c> 页明写「仅第三方平台可调用」；
    /// 其余五页为第三方平台代授权方小程序调用（权限集 18）。把两者并入同一令牌面即令牌串号。
    /// </remarks>
    [Fact]
    public void WeDataTokenSemantics_ShouldSplitByOfficialTokenFace()
    {
        AssertToken(typeof(IWechatOpenPlatformWeDataService),
            OpenPlatformTokenTypes.ComponentAccessToken, "WeData");
        AssertToken(typeof(IWechatOpenPlatformWeDataUserService),
            OpenPlatformTokenTypes.AuthorizerAccessToken, "WeData");
    }

    /// <summary>SM1：服务市场域 3 端点（恒 POST）+ 令牌语义（component_access_token / 注册组 ServiceMarket）。</summary>
    [Fact]
    public void ServiceMarketEndpointsAndToken_ShouldMatchOfficialContract()
    {
        RoutesOf(typeof(IWechatOpenPlatformServiceMarketService)).Should().BeEquivalentTo(ServiceMarketEndpoints,
            because: "官方服务市场三页（buyer_list / buyer / paid_order_list）的请求地址逐一锁定");

        AssertToken(typeof(IWechatOpenPlatformServiceMarketService),
            OpenPlatformTokenTypes.ComponentAccessToken, "ServiceMarket");
    }

    /// <summary>
    /// SM2：<c>login_auth</c> 判非契约——全仓 <c>Src/</c> 零声明。
    /// </summary>
    /// <remarks>
    /// 官方「登录验证」页原文：「本接口不支持第三方平台调用」，其调用凭证为服务商自有 <c>access_token</c>；
    /// 本仓无该令牌面（仅 component / authorizer 两键系）⇒ 声明之即为「取错令牌的死路由」。
    /// 追踪号 <c>SM-LOGINAUTH-NONCONTRACT-01</c>。
    /// </remarks>
    [Fact]
    public void LoginAuth_ShouldBeZeroDeclared_WhenNonContractRulingApplied()
    {
        var routeRegex = new Regex(
            @"\[(?:Get|Post|Put|Delete|Patch)\(""/wxa/servicemarket/service/login_auth""\)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        var offenders = EnumerateSourceFiles("Src")
            .Where(file => routeRegex.IsMatch(File.ReadAllText(file)))
            .ToArray();

        offenders.Should().BeEmpty(
            "官方明写 login_auth 不支持第三方平台调用（服务商自有 access_token 面）⇒ 本仓七线均不得声明；" +
            "如官方后续开放第三方平台面，须先同批更新基线、守卫与本裁决留档");
    }

    /// <summary>REG1：模块枚举 + builder 注册委托 + JSON resolver 合并登记三处一致。</summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModulesAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(new[]
        {
            OpenPlatformModule.UserOrder,
            OpenPlatformModule.WeData,
            OpenPlatformModule.ServiceMarket,
        });

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));

        builderSource.Should().Contain("[OpenPlatformModule.UserOrder] = static s => s.AddUserOrderWebApiHttpClient()");
        builderSource.Should().Contain("[OpenPlatformModule.WeData] = static s => s.AddWeDataWebApiHttpClient()");
        builderSource.Should().Contain("[OpenPlatformModule.ServiceMarket] = static s => s.AddServiceMarketWebApiHttpClient()");

        foreach (var module in Enum.GetValues<OpenPlatformModule>())
        {
            builderSource.Should().Contain($"[OpenPlatformModule.{module}] =",
                $"模块 {module} 无注册委托 ⇒ AddAllApis() 漏注册");
        }

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));

        foreach (var context in new[] { "UserOrderJsonContext.Default", "WeDataJsonContext.Default", "ServiceMarketJsonContext.Default" })
        {
            resolverSource.Should().Contain(context,
                $"{context} 未并进合并 resolver ⇒ AOT 下该域 DTO 无元数据（JIT 下靠反射侥幸可用，最易漏测）");
        }
    }

    /// <summary>DTO1：三域 DTO 全量登记进各自 JsonContext，且 <c>SerializerClassName</c> 恒为域段。</summary>
    [Theory]
    [InlineData("Mud.Wechat.OpenPlatform.DataModels.UserOrder", "UserOrder", 16)]
    [InlineData("Mud.Wechat.OpenPlatform.DataModels.WeData", "WeData", 12)]
    [InlineData("Mud.Wechat.OpenPlatform.DataModels.ServiceMarket", "ServiceMarket", 9)]
    public void DataModels_ShouldBeRegisteredInDomainJsonContexts(
        string ns, string segment, int expectedCount)
    {
        var domainTypes = DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested)
            .Where(t => t.Namespace == ns)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToArray();

        domainTypes.Should().HaveCount(expectedCount,
            $"{ns} 的 DTO 数漂移须先核对官方参数表，再同批重跑 GenerateJsonContext.ps1");

        var contextType = DataModelsAssembly.GetType($"{ns}.{segment}JsonContext")
            ?? throw new InvalidOperationException($"{segment}JsonContext 缺失（生成物漂移）");
        var context = (JsonSerializerContext)(contextType
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null) ?? throw new InvalidOperationException($"{contextType.Name}.Default 缺失"));

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 {ns}，必须登记进 {contextType.Name}（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be(segment,
                $"{type.Name} 的 SerializerClassName 必须为域段 {segment}");
        }
    }

    /// <summary>DTO2：官方反直觉字段名 / 数值面 / 特殊信封逐点锁定。</summary>
    [Theory]
    // 购物订单：订单号类型 number；跳转类型为字符串枚举；验证 / 审核结果字段名照抄官方。
    [InlineData(typeof(OpenPlatformUserOrderKey), "OrderNumberType", "order_number_type")]
    [InlineData(typeof(OpenPlatformUserOrderJumpLink), "Type", "type")]
    [InlineData(typeof(OpenPlatformUserOrderVerifyResponse), "VerifyResult", "verify_result")]
    [InlineData(typeof(OpenPlatformUserOrderPermissionConfirmResponse), "LastResult", "last_result")]
    [InlineData(typeof(OpenPlatformUserOrderShippingSubOrder), "DeliveryMode", "delivery_mode")]
    // WeData：权限列表键为单数 perm；绑定标记 is_bind；登录应答信封 base_resp。
    [InlineData(typeof(OpenPlatformWeDataPermission), "PermissionId", "perm_id")]
    [InlineData(typeof(OpenPlatformWeDataPermissionListResponse), "PermissionList", "perm")]
    [InlineData(typeof(OpenPlatformWeDataBindInfo), "IsBind", "is_bind")]
    [InlineData(typeof(OpenPlatformWeDataBindListResponse), "UserList", "info")]
    [InlineData(typeof(OpenPlatformWeDataLoginResponse), "BaseResp", "base_resp")]
    [InlineData(typeof(OpenPlatformWeDataLoginResponse), "RedirectUrl", "redirect_url")]
    [InlineData(typeof(OpenPlatformWeDataLoginConfigResponse), "AppInfo", "appinfo")]
    // 服务市场：列表键 buyer_list / order_list；手机号为官方字段 phone。
    [InlineData(typeof(OpenPlatformServiceMarketBuyerListResponse), "BuyerList", "buyer_list")]
    [InlineData(typeof(OpenPlatformServiceMarketPaidOrderListResponse), "OrderList", "order_list")]
    [InlineData(typeof(OpenPlatformServiceMarketPaidOrder), "Phone", "phone")]
    public void OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>
    /// DTO3：WeData 登录应答走 <c>base_resp.ret</c> 判错（无顶层 errcode），
    /// 且实现公用判错契约——否则业务失败会被静默吞掉。
    /// </summary>
    [Fact]
    public void WeDataLoginResponse_ShouldJudgeByBaseResp()
    {
        typeof(IWechatApiResponse).IsAssignableFrom(typeof(OpenPlatformWeDataLoginResponse))
            .Should().BeTrue("本线判错统一走公用契约 IWechatApiResponse");

        var response = new OpenPlatformWeDataLoginResponse
        {
            BaseResp = new OpenPlatformWeDataBaseResponse { Ret = 40001 },
        };

        response.ErrorCode.Should().Be(40001);
        response.IsSuccess.Should().BeFalse("base_resp.ret != 0 即失败；若以顶层 errcode 判错会恒真");
    }

    /// <summary>数值面：官方标 number 的字段本仓用整型承载，不写成字符串。</summary>
    [Theory]
    [InlineData(typeof(OpenPlatformUserOrderKey), "OrderNumberType")]
    [InlineData(typeof(OpenPlatformWeDataBindInfo), "IsBind")]
    [InlineData(typeof(OpenPlatformWeDataBindInfo), "CreateTime")]
    [InlineData(typeof(OpenPlatformServiceMarketBuyer), "ServiceId")]
    [InlineData(typeof(OpenPlatformServiceMarketPaidOrder), "OrderId")]
    [InlineData(typeof(OpenPlatformServiceMarketPaidOrder), "TotalPrice")]
    public void OfficialNumericFields_ShouldBeIntegral(Type dtoType, string property)
    {
        var pi = dtoType.GetProperty(property)!;
        var underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

        (underlying == typeof(int) || underlying == typeof(long)).Should().BeTrue(
            $"{dtoType.Name}.{property} 官方为 number，本仓以整型承载（写成 string 即契约失真）");
    }

    private static void AssertToken(Type interfaceType, string expectedTokenType, string expectedGroup)
    {
        var token = interfaceType.GetCustomAttributes<TokenAttribute>(false).Single();
        token.TokenType.Should().Be(expectedTokenType, $"{interfaceType.Name} 的令牌面须与官方页面一致");
        token.Name.Should().Be("access_token",
            "官方全部端点的令牌 query 参数名恒为 access_token（值才是 component / authorizer token）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query,
            "本线无 Header 令牌形态，官方一律 Query 传递");

        interfaceType.GetCustomAttributes<HttpClientApiAttribute>(false).Single().RegistryGroupName
            .Should().Be(expectedGroup, "注册组漂移即 AddModules 无法寻址本域");
    }

    private static (string Method, string Route)[] RoutesOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType)
            .Select(static m =>
            {
                var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
                return (attr.HttpMethod.Method.ToUpperInvariant(), attr.RequestUri!);
            })
            .ToArray();

    private static IEnumerable<string> EnumerateSourceFiles(string relativeDir)
    {
        var root = Path.Combine(FindRepoRoot(), relativeDir);
        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal)
                || normalized.Contains("/Generated/", StringComparison.Ordinal))
            {
                continue;
            }

            yield return file;
        }
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName
            ?? throw new InvalidOperationException("未找到仓库根（缺 Mud.Wechat.slnx）——请在检出树内运行测试。");
    }
}
