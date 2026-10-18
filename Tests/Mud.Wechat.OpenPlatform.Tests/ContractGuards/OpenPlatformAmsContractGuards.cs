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
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.Ams;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「流量主代运营（AMS）」域（<c>/wxa/operationams</c> + <c>/wxa/getdefaultamsinfo</c> + <c>/wxa/setdefaultamsinfo</c>，
/// 两支接口共 22 端点）契约守卫（AM 系列，2026-10-17 逐页取证）。
/// </summary>
/// <remarks>
/// <para><b>本守卫锁定的裁决</b>：① 单路由 <c>/wxa/operationams</c> 的 <c>action</c> 分派（19 条，动词恒 POST，含「获取」类亦 POST）；
/// ② <b>令牌归属二分</b>——管理面 19 端点消费 <c>authorizer_access_token</c>（接口 <c>Ams</c> / 注册组 <c>Ams</c>），
/// 分账面 3 端点消费 <c>component_access_token</c>（接口 <c>AmsShare</c> / 注册组 <c>AmsShare</c>），两者 Query 参数名恒 <c>access_token</c>；
/// ③ 应答信封 <c>{ret, err_msg}</c>（独立基底 <see cref="OpenPlatformAmsResponse"/>，非 <c>{errcode, errmsg}</c>）；
/// ④ 共用 DTO（<c>ad_unit_id</c> 两支查询、结算两支）；⑤ 官方「更新广告单元」页 <c>action</c> 示例复制缺陷的留档。</para>
/// <para>聚合面（接口总数 / 端点总数 / 路由前缀独占）由 <see cref="OpenPlatformContractGuards"/>（OP-B 系列）持有，本守卫逐条锁定本域明细。</para>
/// </remarks>
public class OpenPlatformAmsContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformAmsService).Assembly;

    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformAmsResponse).Assembly;

    /// <summary>管理面 <see cref="IWechatOpenPlatformAmsService"/> 的 19 条（方法名, action 字面量）期望表（路由恒 <c>/wxa/operationams</c>）。</summary>
    private static readonly (string Method, string Action)[] AmsEndpoints =
    {
        ("CreateAdUnitAsync", "agency_create_adunit"),
        ("UpdateAdUnitAsync", "agency_update_adunit"),
        ("GetAdUnitCodeAsync", "agency_get_adunit_code"),
        ("GetAdUnitListAsync", "agency_get_adunit_list"),
        ("GetTmplTypeAsync", "agency_get_tmpl_type"),
        ("GetCoverAdposSceneAsync", "agency_get_cover_adpos_scene"),
        ("GetCoverAdposStatusAsync", "agency_get_cover_adpos_status"),
        ("SetCoverAdposSceneAsync", "agency_set_cover_adpos_scene"),
        ("SetCoverAdposStatusAsync", "agency_set_cover_adpos_status"),
        ("GetAgencyAdUnitListAsync", "get_agency_ad_unit_list"),
        ("GetBlackListAsync", "agency_get_black_list"),
        ("SetBlackListAsync", "agency_set_black_list"),
        ("GetAmsCategoryBlackListAsync", "agency_get_mp_amscategory_blacklist"),
        ("SetAmsCategoryBlackListAsync", "agency_set_mp_amscategory_blacklist"),
        ("GetAdposGeneralAsync", "agency_get_adpos_genenral"),
        ("GetAdUnitGeneralAsync", "agency_get_adunit_general"),
        ("CheckCanOpenPublisherAsync", "agency_check_can_open_publisher"),
        ("CreatePublisherAsync", "agency_create_publisher"),
        ("GetSettlementAsync", "agency_get_settlement"),
    };

    /// <summary>分账面 <see cref="IWechatOpenPlatformAmsShareService"/> 的 3 条（方法名, 路由, action）期望表。</summary>
    private static readonly (string Method, string Route, string Action)[] AmsShareEndpoints =
    {
        ("GetCustomShareRatioAsync", "/wxa/getdefaultamsinfo", "agency_get_custom_share_ratio"),
        ("SetCustomShareRatioAsync", "/wxa/setdefaultamsinfo", "agency_set_custom_share_ratio"),
        ("GetAgencySettledRevenueAsync", "/wxa/getdefaultamsinfo", "get_agency_settled_revenue"),
    };

    /// <summary>
    /// AM1：管理面 19 端点恒为 POST <c>/wxa/operationams</c>、逐条 <c>action</c> 字面量与官方示例一致（含官方拼写 <c>genenral</c>、无前缀 <c>get_agency_ad_unit_list</c>）。
    /// </summary>
    [Fact]
    public void AmsServiceEndpoints_ShouldBeSingleRouteActionDispatch_WhenEnumerated()
    {
        var type = typeof(IWechatOpenPlatformAmsService);
        var methods = MethodsOf(type).ToArray();

        methods.Should().HaveCount(19, "管理面恰 19 端点——少一条即缺口，多一条即漂移");

        foreach (var (methodName, expectedAction) in AmsEndpoints)
        {
            var m = type.GetMethod(methodName)
                ?? throw new InvalidOperationException($"{methodName} 不存在（契约面漂移）");

            var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
            attr.HttpMethod.Method.Should().Be("POST", $"{methodName} 官方为 POST（含查询类端点）");
            attr.RequestUri.Should().Be("/wxa/operationams",
                $"{methodName} 走单路由 action 分派，路由不得含 action");

            ActionLiteral(m).Should().Be(expectedAction,
                $"{methodName} 的 action 默认值即官方分派语义（拼写照官方原文，含 genenral 非 general、get_agency_ad_unit_list 无 agency_ 前缀）");
        }
    }

    /// <summary>
    /// AM2：分账面 3 端点的（方法名, 路由, action）表逐条锁定，动词恒 POST。
    /// </summary>
    [Fact]
    public void AmsShareServiceEndpoints_ShouldMatchOfficialRoutes_WhenEnumerated()
    {
        var type = typeof(IWechatOpenPlatformAmsShareService);
        MethodsOf(type).Should().HaveCount(3, "分账面恰 3 端点");

        foreach (var (methodName, route, expectedAction) in AmsShareEndpoints)
        {
            var m = type.GetMethod(methodName)!;
            var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
            attr.HttpMethod.Method.Should().Be("POST");
            attr.RequestUri.Should().Be(route, $"{methodName} 的路由照官方正文（getdefaultamsinfo / setdefaultamsinfo）");
            ActionLiteral(m).Should().Be(expectedAction,
                $"{methodName} 的 action 分派值（get_agency_settled_revenue 无 agency_ 前缀，照官方原文）");
        }
    }

    /// <summary>
    /// AM3：令牌归属二分——管理面 = <c>authorizer_access_token</c>，分账面 = <c>component_access_token</c>，两者 Query 参数名恒 <c>access_token</c>、注入恒 Query；注册组分别 <c>Ams</c> / <c>AmsShare</c>。
    /// </summary>
    [Fact]
    public void TokenSemantics_ShouldSplitAuthorizerAndComponent_WhenInterfacesDeclared()
    {
        var ams = typeof(IWechatOpenPlatformAmsService)
            .GetCustomAttributes<TokenAttribute>(false).Single();
        ams.TokenType.Should().Be(OpenPlatformTokenTypes.AuthorizerAccessToken,
            "管理面代授权方小程序调用，取平台自身 component 令牌即令牌串号");
        ams.Name.Should().Be("access_token",
            "官方页面 query 参数名为 access_token（值才是 authorizer_access_token）");
        ams.InjectionMode.Should().Be(TokenInjectionMode.Query);
        typeof(IWechatOpenPlatformAmsService)
            .GetCustomAttributes<HttpClientApiAttribute>(false).Single()
            .RegistryGroupName.Should().Be("Ams");

        var share = typeof(IWechatOpenPlatformAmsShareService)
            .GetCustomAttributes<TokenAttribute>(false).Single();
        share.TokenType.Should().Be(OpenPlatformTokenTypes.ComponentAccessToken,
            "分账面为服务商平台自身令牌（官方各页「access_token 为 component_access_token」）——与管理面令牌归属不同故拆接口");
        share.Name.Should().Be("access_token");
        share.InjectionMode.Should().Be(TokenInjectionMode.Query);
        typeof(IWechatOpenPlatformAmsShareService)
            .GetCustomAttributes<HttpClientApiAttribute>(false).Single()
            .RegistryGroupName.Should().Be("AmsShare");
    }

    /// <summary>
    /// AM4：三处一致（模块枚举 + builder 委托 + JSON resolver 合并登记）——Ams 与 AmsShare 两注册组。
    /// </summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModuleAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(OpenPlatformModule.Ams)
            .And.Contain(OpenPlatformModule.AmsShare,
                "模块枚举缺值 ⇒ AddModules 无法寻址本域");

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));
        builderSource.Should().Contain("[OpenPlatformModule.Ams] = static s => s.AddAmsWebApiHttpClient()",
            "注册入口必须委托到生成器按注册组产出的 AddAmsWebApiHttpClient()");
        builderSource.Should().Contain("[OpenPlatformModule.AmsShare] = static s => s.AddAmsShareWebApiHttpClient()",
            "注册入口必须委托到 AddAmsShareWebApiHttpClient()");

        foreach (var module in Enum.GetValues<OpenPlatformModule>())
        {
            builderSource.Should().Contain($"[OpenPlatformModule.{module}] =",
                $"模块 {module} 无注册委托 ⇒ AddAllApis() 漏注册");
        }

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("AmsJsonContext.Default",
            "域上下文未并进合并 resolver ⇒ 运行期退回反射序列化，AOT 下直接失败");
    }

    /// <summary>
    /// AM5：本域 41 个 DTO 全量登记进 <see cref="AmsJsonContext"/>，且 <c>SerializerClassName</c> 恒为域段 <c>Ams</c>。
    /// </summary>
    [Fact]
    public void DataModels_ShouldBeRegisteredInAmsJsonContext_WhenDomainSurfaceEnumerated()
    {
        var domainTypes = DomainDtoTypes();

        domainTypes.Should().HaveCount(41,
            "本域 DTO 数漂移须先核对官方参数表（1 基底 + 14 请求 + 14 应答 + 12 内嵌），再同批重跑生成脚本");

        var context = (JsonSerializerContext)typeof(AmsJsonContext)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Ams 命名空间，必须登记进 AmsJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Ams",
                $"{type.Name} 的 SerializerClassName 必须为域段 Ams");
        }
    }

    /// <summary>
    /// AM6：共用 DTO 与无请求体端点照官方——<c>ad_unit_id</c> 两支查询共用一支、结算两支共用请求+应答、6 支无体端点不造 DTO。
    /// </summary>
    [Fact]
    public void SharedAndBodylessEndpoints_ShouldStayAsOfficial()
    {
        var ams = typeof(IWechatOpenPlatformAmsService);

        // get_adunit_code 与 get_tmpl_type 请求体逐字相同（仅 ad_unit_id）⇒ 共用 AdUnitIdRequest。
        BodyParamType(ams, "GetAdUnitCodeAsync").Should().Be(typeof(OpenPlatformAmsAdUnitIdRequest));
        BodyParamType(ams, "GetTmplTypeAsync").Should().Be(typeof(OpenPlatformAmsAdUnitIdRequest),
            "两页请求体同为仅 ad_unit_id，共用一支 DTO；各造一支即契约失真");

        // 结算两支：operationams(agency_get_settlement) 与 getdefaultamsinfo(get_agency_settled_revenue) 请求体+应答逐字相同 ⇒ 共用。
        BodyParamType(ams, "GetSettlementAsync").Should().Be(typeof(OpenPlatformAmsRevenuePageRequest));
        BodyParamType(typeof(IWechatOpenPlatformAmsShareService), "GetAgencySettledRevenueAsync")
            .Should().Be(typeof(OpenPlatformAmsRevenuePageRequest));
        ReturnType(ams, "GetSettlementAsync").Should().Be(typeof(OpenPlatformAmsSettlementResponse));
        ReturnType(typeof(IWechatOpenPlatformAmsShareService), "GetAgencySettledRevenueAsync")
            .Should().Be(typeof(OpenPlatformAmsSettlementResponse));

        // 6 支无请求体端点（官方示例体为 {}）：不得带 [Body] 参数。
        foreach (var methodName in new[]
                 {
                     "GetCoverAdposSceneAsync", "GetCoverAdposStatusAsync", "GetBlackListAsync",
                     "GetAmsCategoryBlackListAsync", "CheckCanOpenPublisherAsync", "CreatePublisherAsync",
                 })
        {
            var m = ams.GetMethod(methodName)!;
            m.GetParameters().Should().NotContain(p => p.GetCustomAttributes<BodyAttribute>(false).Any(),
                $"{methodName} 官方无请求体——不得造请求 DTO");
        }
    }

    /// <summary>
    /// AM7：应答信封独立基底——<see cref="OpenPlatformAmsResponse"/> 用 <c>ret</c>/<c>err_msg</c> 判错，且 <c>ret</c> 缺省 0 视为成功。
    /// </summary>
    [Fact]
    public void ResponseBase_ShouldUseRetEnvelope_WhenDeclared()
    {
        typeof(OpenPlatformAmsResponse).Should()
            .BeAssignableTo<Mud.Wechat.Abstractions.Contracts.IWechatApiResponse>(
                "AMS 响应基底必须实现公用层判错契约");

        var newResponse = new OpenPlatformAmsResponse();
        newResponse.IsSuccess.Should().BeTrue("ret 缺省 0 即成功");

        // 判错面走 ret（ErrorCode => Ret），非 errcode——若误用 OpenPlatformResponse 会因 errcode 恒缺省而静默吞错。
        typeof(OpenPlatformAmsResponse).GetProperty("Ret")!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("ret");
        typeof(OpenPlatformAmsResponse).GetProperty("ErrorMessage")!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("err_msg");
    }

    /// <summary>
    /// AM8：官方页面复制缺陷留档——「更新广告单元」页示例把 <c>action</c> 误写为 <c>agency_create_adunit</c>，本仓以正确值 <c>agency_update_adunit</c> 声明。
    /// </summary>
    [Fact]
    public void UpdateAdUnitAction_ShouldBeCorrectValueDespiteDocBug()
    {
        var m = typeof(IWechatOpenPlatformAmsService).GetMethod("UpdateAdUnitAsync")!;
        ActionLiteral(m).Should().Be("agency_update_adunit",
            "官方 AgencyUpdateAdunit.html 示例把 action 复制成了 agency_create_adunit（页面缺陷）；本仓按页面正文语义用 agency_update_adunit，" +
            "不据示例造重复的创建端点——追踪号 AM-UPDATEDOCBUG-01");
    }

    /// <summary>
    /// AM9：官方字段名/拼写照抄不修正 + number 字段用整型/浮点承载。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformAmsAdUnitListRequest), "PageSize", "page_size")]
    [InlineData(typeof(OpenPlatformAmsAdUnitCreateRequest), "TmplType", "tmpl_type")]
    [InlineData(typeof(OpenPlatformAmsAdUnitUpdateRequest), "AdUnitId", "ad_unit_id")]
    [InlineData(typeof(OpenPlatformAmsBlackListSetRequest), "Op", "op")]
    [InlineData(typeof(OpenPlatformAmsCategoryBlackListSetRequest), "AmsCategory", "ams_category")]
    [InlineData(typeof(OpenPlatformAmsCoverSceneSetRequest), "CoverSceneList", "cover_scene_list")]
    [InlineData(typeof(OpenPlatformAmsShareRatioGetRequest), "AppId", "appid")]
    [InlineData(typeof(OpenPlatformAmsSettlementResponse), "RevenueAll", "revenue_all")]
    [InlineData(typeof(OpenPlatformAmsSettlement), "SettStatus", "sett_status")]
    [InlineData(typeof(OpenPlatformAmsSettlement), "MailSendCnt", "mail_send_cnt")]
    [InlineData(typeof(OpenPlatformAmsSlotRevenue), "SlotSettledRevenue", "slot_settled_revenue")]
    [InlineData(typeof(OpenPlatformAmsCoverStatusResponse), "NextOpenTime", "next_open_time")]
    [InlineData(typeof(OpenPlatformAmsAgencyAdUnitListResponse), "TotalNum", "total_num")]
    [InlineData(typeof(OpenPlatformAmsAdposGeneralResponse), "Items", "list")]
    public void OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>
    /// AM10：官方标 number 的计数/金额字段本仓用整型承载，比率字段用 double，不写成字符串（例外：服务商模板列表 total_num 官方即 string）。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformAmsAdposGeneralItem), "ReqSuccCount", typeof(long))]
    [InlineData(typeof(OpenPlatformAmsAdposGeneralItem), "PublisherIncome", typeof(long))]
    [InlineData(typeof(OpenPlatformAmsAdposGeneralItem), "Ecpm", typeof(long))]
    [InlineData(typeof(OpenPlatformAmsAdposGeneralItem), "ExposureRate", typeof(double))]
    [InlineData(typeof(OpenPlatformAmsAdposGeneralItem), "ClickRate", typeof(double))]
    [InlineData(typeof(OpenPlatformAmsCoverStatusResponse), "NextOpenTime", typeof(long))]
    [InlineData(typeof(OpenPlatformAmsCheckCanOpenPublisherResponse), "Status", typeof(long))]
    [InlineData(typeof(OpenPlatformAmsShareRatioResponse), "ShareRatio", typeof(long))]
    [InlineData(typeof(OpenPlatformAmsAgencyAdUnitListResponse), "TotalNum", typeof(string))]
    public void OfficialFieldTypes_ShouldBeFaithful(Type dtoType, string property, Type expected)
    {
        var pi = dtoType.GetProperty(property)!;
        var underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
        underlying.Should().Be(expected,
            $"{dtoType.Name}.{property} 官方为 {(expected == typeof(double) ? "number(比率)" : expected == typeof(string) ? "string(官方原文)" : "number(计数/金额)")}，类型失真即契约失真");
    }

    private static string? ActionLiteral(MethodInfo method)
        => method.GetParameters()
            .Where(static p => p.GetCustomAttributes<QueryAttribute>(false)
                .Any(static q => q.Name == "action"))
            .Select(static p => p.DefaultValue as string)
            .FirstOrDefault();

    private static Type BodyParamType(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName)
            ?? throw new InvalidOperationException($"{methodName} 不存在");
        var body = method.GetParameters()
            .FirstOrDefault(static p => p.GetCustomAttributes<BodyAttribute>(false).Any());
        body.Should().NotBeNull($"{methodName} 官方有 JSON 请求体，须带 [Body] 参数");
        return body!.ParameterType;
    }

    private static Type ReturnType(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName)!;
        return method.ReturnType.GetGenericArguments()[0];
    }

    private static IEnumerable<MethodInfo> MethodsOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    /// <summary>本域 DTO 类型集（上下文自身按 <see cref="JsonSerializerContext"/> 剔除）。</summary>
    private static Type[] DomainDtoTypes()
        => DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested
                               && t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.Ams")
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToArray();

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
