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
using Mud.Wechat.OpenPlatform.DataModels.Embedded;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「半屏小程序 / 嵌入式小程序管理」域（<c>/wxaapi/wxaembedded/*</c>，6 端点 / 一支接口）契约守卫。
/// </summary>
/// <remarks>
/// <para><b>落位裁决</b>：本域归开放平台线而非小程序线——令牌面是 <c>authorizer_access_token</c>（第三方平台代授权方小程序调用），
/// 由留档 <c>MP-D-BUCKET-RULING-01</c> 与小程序线守卫 <c>NC2</c>（<c>Src/MiniProgram</c> 对 <c>/wxaapi/wxaembedded/*</c> 零声明）双向锁定。
/// 本守卫为该裁决的<b>本线镜像</b>：锁定路由 / 动词 / 令牌语义 / 三处注册一致 / DTO 面。</para>
/// <para>逐页取证（2026-10-17）见 <see cref="IWechatOpenPlatformEmbeddedService"/> 的 XML 注释。</para>
/// </remarks>
public class OpenPlatformEmbeddedContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformEmbeddedService).Assembly;

    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformEmbeddedAddRequest).Assembly;

    /// <summary>本域 6 条（HTTP 方法, 路由）期望表：路由拼写照官方正文，非页面文件名。</summary>
    private static readonly (string Method, string Route)[] ExpectedEndpoints =
    {
        ("POST", "/wxaapi/wxaembedded/add_embedded"),
        ("POST", "/wxaapi/wxaembedded/del_embedded"),
        ("POST", "/wxaapi/wxaembedded/del_authorize"),
        ("POST", "/wxaapi/wxaembedded/set_authorize"),
        ("GET", "/wxaapi/wxaembedded/get_list"),
        ("GET", "/wxaapi/wxaembedded/get_own_list"),
    };

    /// <summary>
    /// EMB1：一支接口 / 6 端点的（方法, 路由）表逐条锁定，且动词分布 = 4 POST + 2 GET。
    /// </summary>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutes_WhenEnumerated()
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformEmbeddedService)}")!;

        var actual = MethodsOf(type)
            .Select(static m =>
            {
                var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
                return (attr.HttpMethod.Method.ToUpperInvariant(), attr.RequestUri!);
            })
            .ToArray();

        actual.Should().HaveCount(6, "本域恰 6 条端点（4 POST 写 + 2 GET 列表）——少一条即缺口或扫描空跑");
        actual.Should().BeEquivalentTo(ExpectedEndpoints,
            because: "官方 embedded-management 六页正文的请求地址逐一锁定；路由拼写须照正文（add_embedded）而非页面文件名（addEmbedded）");
    }

    /// <summary>
    /// EMB2：令牌语义（授权方令牌、参数名 access_token、Query 注入）+ 注册组 <c>Embedded</c>。
    /// </summary>
    /// <remarks>
    /// 令牌<b>值</b>是 <c>authorizer_access_token</c>，但官方<b>query 参数名</b>恒为 <c>access_token</c>
    /// （2026-10-17 逐页核验：六页调用凭证说明均写「使用授权方 access_token」且参数名为 <c>access_token</c>）
    /// ⇒ 把 <c>Name</c> 改成 <c>authorizer_access_token</c> 会让全部端点 40001。
    /// </remarks>
    [Fact]
    public void TokenSemantics_ShouldBeAuthorizerQuery_WhenInterfaceDeclared()
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformEmbeddedService)}")!;

        var token = type.GetCustomAttributes<TokenAttribute>(false).Single();
        token.TokenType.Should().Be(OpenPlatformTokenTypes.AuthorizerAccessToken,
            "本域代授权方小程序调用，取平台自身 component_access_token 即令牌串号");
        token.Name.Should().Be("access_token",
            "官方六页正文的 query 参数名为 access_token（值才是 authorizer_access_token）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query,
            "本域无 Header 令牌形态，官方一律 Query 传递");

        var api = type.GetCustomAttributes<HttpClientApiAttribute>(false).Single();
        api.RegistryGroupName.Should().Be("Embedded",
            "注册组漂移即 AddModules(Embedded) 无法寻址本域");
    }

    /// <summary>
    /// EMB3：目录 / 命名空间 / 注册入口三处一致（模块枚举 + builder 委托 + JSON resolver 合并登记）。
    /// </summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModuleAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(OpenPlatformModule.Embedded,
            "模块枚举缺值 ⇒ AddModules 无法寻址本域");

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));
        builderSource.Should().Contain("[OpenPlatformModule.Embedded] = static s => s.AddEmbeddedWebApiHttpClient()",
            "注册入口必须委托到生成器按注册组产出的 AddEmbeddedWebApiHttpClient()");

        // 每个模块枚举值都要有委托，否则 AddAllApis() 静默漏域。
        foreach (var module in Enum.GetValues<OpenPlatformModule>())
        {
            builderSource.Should().Contain($"[OpenPlatformModule.{module}] =",
                $"模块 {module} 无注册委托 ⇒ AddAllApis() 漏注册");
        }

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("EmbeddedJsonContext.Default",
            "域上下文未并进合并 resolver ⇒ 运行期退回反射序列化，AOT 下直接 IL 诊断或抛异常");
    }

    /// <summary>
    /// EMB4：本域 5 个 DTO 全量登记进 <see cref="EmbeddedJsonContext"/>，且 <c>SerializerClassName</c> 恒为域段 <c>Embedded</c>。
    /// </summary>
    [Fact]
    public void DataModels_ShouldBeRegisteredInEmbeddedJsonContext_WhenDomainSurfaceEnumerated()
    {
        var domainTypes = DomainDtoTypes();

        domainTypes.Should().HaveCount(5,
            "本域 DTO 数漂移须先核对官方参数表（AddRequest / AppidRequest / SetAuthorizeRequest / ListItem / ListResponse），再同批重跑生成脚本");

        var context = (JsonSerializerContext)typeof(EmbeddedJsonContext)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Embedded 命名空间，必须登记进 EmbeddedJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Embedded",
                $"{type.Name} 的 SerializerClassName 必须为域段 Embedded（错段即落到别的上下文，本上下文反而漏登记）");
        }
    }

    /// <summary>
    /// EMB5：官方请求体一致的端点共用 DTO、GET 端点零请求体——防人为造空壳。
    /// </summary>
    [Fact]
    public void SharedAndBodylessEndpoints_ShouldStayAsOfficial()
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformEmbeddedService)}")!;

        // del_embedded 与 del_authorize 请求体逐字相同（仅 appid）⇒ 共用 AppidRequest。
        BodyParamType(type, "DeleteEmbeddedAsync")
            .Should().Be(typeof(OpenPlatformEmbeddedAppidRequest));
        BodyParamType(type, "DeleteAuthorizedEmbeddedAsync")
            .Should().Be(typeof(OpenPlatformEmbeddedAppidRequest),
                "两页请求体同为仅 appid，共用一支 DTO；各造一支即契约失真");

        // 两支 GET 列表端点：无 [Body]、有 [Query] start/num、共用 ListResponse。
        foreach (var methodName in new[] { "GetEmbeddedListAsync", "GetOwnEmbeddedListAsync" })
        {
            var method = type.GetMethod(methodName)!;
            method.GetParameters()
                .Should().NotContain(p => p.GetCustomAttributes<BodyAttribute>(false).Any(),
                    $"{methodName} 官方为 GET、无请求体——不得造请求 DTO");
            method.GetParameters()
                .SelectMany(p => p.GetCustomAttributes<QueryAttribute>(false))
                .Select(static a => a.Name)
                .Should().BeEquivalentTo(new[] { "start", "num" },
                    $"{methodName} 的官方 Query 参数恰为 start + num");
            method.ReturnType.GetGenericArguments()[0]
                .Should().Be(typeof(OpenPlatformEmbeddedListResponse),
                    $"{methodName} 应答形状与另一支列表逐字相同 ⇒ 共用 ListResponse");
        }
    }

    /// <summary>
    /// EMB6：官方字段名/拼写照抄不修正——<c>headimg</c>（非 head_img）、<c>create_time</c>/<c>status</c>/<c>embedded_flag</c> 为 number、请求体键 <c>appid</c>/<c>apply_reason</c>/<c>flag</c>。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformEmbeddedAddRequest), "AppId", "appid")]
    [InlineData(typeof(OpenPlatformEmbeddedAddRequest), "ApplyReason", "apply_reason")]
    [InlineData(typeof(OpenPlatformEmbeddedAppidRequest), "AppId", "appid")]
    [InlineData(typeof(OpenPlatformEmbeddedSetAuthorizeRequest), "Flag", "flag")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "AppId", "appid")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "CreateTime", "create_time")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "HeadImg", "headimg")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "Nickname", "nickname")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "Reason", "reason")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "Status", "status")]
    [InlineData(typeof(OpenPlatformEmbeddedListResponse), "EmbeddedFlag", "embedded_flag")]
    [InlineData(typeof(OpenPlatformEmbeddedListResponse), "EmbeddedList", "wxa_embedded_list")]
    public void OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文（headimg 非 head_img），改名即字段静默丢失");
    }

    /// <summary>CBT5 同款数值面：官方标 number 的字段本仓用整型/可空承载，不写成字符串。</summary>
    [Theory]
    [InlineData(typeof(OpenPlatformEmbeddedSetAuthorizeRequest), "Flag")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "CreateTime")]
    [InlineData(typeof(OpenPlatformEmbeddedListItem), "Status")]
    [InlineData(typeof(OpenPlatformEmbeddedListResponse), "EmbeddedFlag")]
    public void OfficialNumericFields_ShouldBeIntegral(Type dtoType, string property)
    {
        var pi = dtoType.GetProperty(property)!;
        var underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
        underlying.Should().Be(typeof(long),
            $"{dtoType.Name}.{property} 官方为 number，本仓以整型 long 承载（写成 string 即契约失真）");
    }

    private static Type BodyParamType(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName)
            ?? throw new InvalidOperationException($"{methodName} 不存在");
        var body = method.GetParameters()
            .FirstOrDefault(static p => p.GetCustomAttributes<BodyAttribute>(false).Any());
        body.Should().NotBeNull($"{methodName} 官方有 JSON 请求体，须带 [Body] 参数");
        return body!.ParameterType;
    }

    private static IEnumerable<MethodInfo> MethodsOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    /// <summary>本域 DTO 类型集（上下文自身按 <see cref="JsonSerializerContext"/> 剔除）。</summary>
    private static Type[] DomainDtoTypes()
        => DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested
                               && t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.Embedded")
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
