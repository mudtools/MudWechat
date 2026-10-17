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
using Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「代码模板库 / 下单路径申请」（MiniProgramTemplate，<c>component_access_token</c> 面）契约守卫（TPL 系列，2026-10-17 逐页取证）。
/// </summary>
/// <remarks>
/// <para>两支接口共 5 端点：代码模板库管理 4（<c>gettemplatedraftlist</c> GET + <c>gettemplatelist</c> GET + <c>addtotemplate</c> POST + <c>deletetemplate</c> POST）
/// + 下单页 path 批量申请 1（<c>applysetorderpathinfo</c> POST）。</para>
/// <para><b>本守卫锁定的裁决</b>：① 令牌面统一为 <c>component_access_token</c>（服务商以自身令牌操作模板库、批量申请下单 path），Query 参数名恒 <c>access_token</c>；
/// 与授权方 <c>MiniProgramManage</c> 组令牌归属不同，故独立注册组（对齐 <c>Ams</c>/<c>AmsShare</c> 按令牌拆组先例）；
/// ② <c>gettemplatelist</c> 官方明确「请求方式是 get，不是 post」（误用 POST 报 <c>43001</c>），<c>template_type</c> 以可选 Query 过滤；
/// ③ <c>applysetorderpathinfo</c> 是授权方 <c>getorderpathinfo</c>（<c>authorizer_access_token</c>，守卫 MPG3h）的 component 面孪生端点，令牌相反、DTO 各建。</para>
/// <para>聚合面（接口总数 / 端点总数 / 路由前缀独占 / DTO 上下文登记）由 <see cref="OpenPlatformContractGuards"/>（OP-B 系列）持有。</para>
/// </remarks>
public class OpenPlatformTemplateContractGuards
{
    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformResponse).Assembly;

    /// <summary>代码模板库 <see cref="IWechatOpenPlatformTemplateService"/> 的 4 条（方法名, 动词, 路由）期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] TemplateEndpoints =
    {
        ("GetTemplateDraftListAsync", "GET", "/wxa/gettemplatedraftlist"),
        ("GetTemplateListAsync", "GET", "/wxa/gettemplatelist"),
        ("AddToTemplateAsync", "POST", "/wxa/addtotemplate"),
        ("DeleteTemplateAsync", "POST", "/wxa/deletetemplate"),
    };

    /// <summary>下单路径申请 <see cref="IWechatOpenPlatformMpOrderPathSetService"/> 的 1 条期望表。</summary>
    private static readonly (string Method, string Verb, string Route)[] OrderPathSetEndpoints =
    {
        ("ApplySetOrderPathInfoAsync", "POST", "/wxa/security/applysetorderpathinfo"),
    };

    /// <summary>
    /// TPL1：代码模板库 4 端点（方法名, 动词, 路由）逐条锁定——两支查询为 GET（<c>gettemplatelist</c> 官方特别提示非 POST）、两支写为 POST。
    /// </summary>
    [Fact]
    public void TemplateServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformTemplateService), TemplateEndpoints, 4);

    /// <summary>
    /// TPL2：下单路径申请 1 端点锁定（<c>applysetorderpathinfo</c>，恒 POST，component 面孪生 <c>getorderpathinfo</c>）。
    /// </summary>
    [Fact]
    public void OrderPathSetServiceEndpoints_ShouldMatchOfficialVerbRoutes_WhenEnumerated()
        => AssertEndpoints(typeof(IWechatOpenPlatformMpOrderPathSetService), OrderPathSetEndpoints, 1);

    /// <summary>
    /// TPL3：令牌面统一——两支接口全部 <c>component_access_token</c> + Query 参数名 <c>access_token</c> + 注册组 <c>MiniProgramTemplate</c>。
    /// </summary>
    [Fact]
    public void TokenSemantics_ShouldBeComponentSharedGroup_WhenInterfacesDeclared()
    {
        foreach (var type in new[]
                 {
                     typeof(IWechatOpenPlatformTemplateService),
                     typeof(IWechatOpenPlatformMpOrderPathSetService),
                 })
        {
            var token = type.GetCustomAttributes<TokenAttribute>(false).Single();
            token.TokenType.Should().Be(OpenPlatformTokenTypes.ComponentAccessToken,
                $"{type.Name} 以服务商自身令牌操作模板库/批量下单 path，误用 authorizer 令牌即令牌串号");
            token.Name.Should().Be("access_token",
                "官方页面 query 参数名为 access_token（值才是 component_access_token）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query);

            type.GetCustomAttributes<HttpClientApiAttribute>(false).Single()
                .RegistryGroupName.Should().Be("MiniProgramTemplate",
                    "两支接口共用一个注册组 ⇒ 一次 AddMiniProgramTemplateWebApiHttpClient() 覆盖两个接口");
        }
    }

    /// <summary>
    /// TPL4：三处一致（模块枚举 + builder 委托 + JSON resolver 合并登记）——注册组 <c>MiniProgramTemplate</c>。
    /// </summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModuleAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(OpenPlatformModule.MiniProgramTemplate,
            "模块枚举缺值 ⇒ AddModules 无法寻址本域");

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));
        builderSource.Should().Contain(
            "[OpenPlatformModule.MiniProgramTemplate] = static s => s.AddMiniProgramTemplateWebApiHttpClient()",
            "注册入口必须委托到生成器按注册组产出的 AddMiniProgramTemplateWebApiHttpClient()");

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MiniProgramTemplateJsonContext.Default",
            "域上下文未并进合并 resolver ⇒ 运行期退回反射序列化，AOT 下直接失败");
    }

    /// <summary>
    /// TPL5：本域 9 个 DTO 全量登记进 <see cref="MiniProgramTemplateJsonContext"/>，且 <c>SerializerClassName</c> 恒为域段 <c>MiniProgramTemplate</c>。
    /// </summary>
    [Fact]
    public void DataModels_ShouldBeRegisteredInDomainJsonContext_WhenSurfaceEnumerated()
    {
        var domainTypes = DomainDtoTypes();

        domainTypes.Should().HaveCount(9,
            "本域 DTO 数漂移须先核对官方参数表，再同批重跑生成脚本（模板库 6 + 下单路径申请 3）");

        var context = (JsonSerializerContext)typeof(MiniProgramTemplateJsonContext)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 MiniProgramTemplate 命名空间，必须登记进 MiniProgramTemplateJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("MiniProgramTemplate",
                $"{type.Name} 的 SerializerClassName 必须为域段 MiniProgramTemplate");
        }
    }

    /// <summary>
    /// TPL6：<c>gettemplatelist</c> 的 <c>template_type</c> 以可选 Query 过滤（<c>null</c> 即省略、返回全部），不得误做成请求体（本端点为 GET）。
    /// </summary>
    [Fact]
    public void GetTemplateList_ShouldFilterByOptionalQuery_WhenGet()
    {
        var m = typeof(IWechatOpenPlatformTemplateService).GetMethod("GetTemplateListAsync")!;
        var templateType = m.GetParameters()
            .Single(static p => p.GetCustomAttributes<QueryAttribute>(false).Any(static q => q.Name == "template_type"));
        templateType.ParameterType.Should().Be(typeof(long?),
            "template_type 选填：0 普通 / 1 标准，null 时生成器省略该 query ⇒ 返回全部");
        m.GetParameters().Should().NotContain(static p =>
            p.GetCustomAttributes<BodyAttribute>(false).Any(),
            "gettemplatelist 是 GET，无请求体");
    }

    /// <summary>
    /// TPL7：官方字段名/拼写照抄不修正（选取本域易漂移键逐条钉死）。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformTemplateDraftItem), "DraftId", "draft_id")]
    [InlineData(typeof(OpenPlatformTemplateDraftItem), "SourceMiniprogramAppid", "source_miniprogram_appid")]
    [InlineData(typeof(OpenPlatformTemplateDraftListResponse), "DraftList", "draft_list")]
    [InlineData(typeof(OpenPlatformTemplateItem), "TemplateId", "template_id")]
    [InlineData(typeof(OpenPlatformTemplateItem), "TemplateType", "template_type")]
    [InlineData(typeof(OpenPlatformTemplateItem), "AuditStatus", "audit_status")]
    [InlineData(typeof(OpenPlatformTemplateItem), "CategoryList", "category_list")]
    [InlineData(typeof(OpenPlatformTemplateCategoryItem), "FirstClass", "first_class")]
    [InlineData(typeof(OpenPlatformTemplateCategoryItem), "ThirdId", "third_id")]
    [InlineData(typeof(OpenPlatformAddToTemplateRequest), "DraftId", "draft_id")]
    [InlineData(typeof(OpenPlatformDeleteTemplateRequest), "TemplateId", "template_id")]
    [InlineData(typeof(OpenPlatformTemplateListResponse), "TemplateList", "template_list")]
    [InlineData(typeof(OpenPlatformMpOrderPathSetRequest), "BatchReq", "batch_req")]
    [InlineData(typeof(OpenPlatformMpOrderPathBatchReq), "AppidList", "appid_list")]
    [InlineData(typeof(OpenPlatformMpOrderPathBatchReq), "TestPwd", "test_pwd")]
    [InlineData(typeof(OpenPlatformMpOrderPathBatchReq), "TestRemark", "test_remark")]
    public void OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>
    /// TPL8：写端点请求体 DTO 各建——<c>addtotemplate</c>/<c>deletetemplate</c>/<c>applysetorderpathinfo</c> 分别绑各自请求 DTO，且 <c>applysetorderpathinfo</c> 请求体为 component 版 <see cref="OpenPlatformMpOrderPathSetRequest"/>（与授权方 GET 版 <c>info_type</c> 请求不同型）。
    /// </summary>
    [Fact]
    public void WriteRequests_ShouldBindDedicatedDtoPerEndpoint()
    {
        BodyParamType(typeof(IWechatOpenPlatformTemplateService), "AddToTemplateAsync")
            .Should().Be(typeof(OpenPlatformAddToTemplateRequest));
        BodyParamType(typeof(IWechatOpenPlatformTemplateService), "DeleteTemplateAsync")
            .Should().Be(typeof(OpenPlatformDeleteTemplateRequest));
        BodyParamType(typeof(IWechatOpenPlatformMpOrderPathSetService), "ApplySetOrderPathInfoAsync")
            .Should().Be(typeof(OpenPlatformMpOrderPathSetRequest),
                "component 面下单 path 请求体（batch_req + appid_list），与授权方 getorderpathinfo 的 info_type 查询请求不同型");
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

    private static void AssertEndpoints(Type interfaceType, (string Method, string Verb, string Route)[] expected, int count)
    {
        var methods = MethodsOf(interfaceType).ToArray();
        methods.Should().HaveCount(count, $"{interfaceType.Name} 恰 {count} 端点——少一条即缺口，多一条即漂移");

        foreach (var (methodName, verb, route) in expected)
        {
            var m = interfaceType.GetMethod(methodName)
                ?? throw new InvalidOperationException($"{methodName} 不存在（契约面漂移）");

            var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
            attr.HttpMethod.Method.Should().Be(verb, $"{methodName} 官方动词为 {verb}");
            attr.RequestUri.Should().Be(route, $"{methodName} 的路由照官方正文");
        }
    }

    private static IEnumerable<MethodInfo> MethodsOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    /// <summary>本域 DTO 类型集（上下文自身按 <see cref="JsonSerializerContext"/> 剔除）。</summary>
    private static Type[] DomainDtoTypes()
        => DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested
                               && t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate")
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
