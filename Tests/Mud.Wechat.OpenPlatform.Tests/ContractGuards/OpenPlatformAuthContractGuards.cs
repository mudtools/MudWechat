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
using Mud.Wechat.OpenPlatform.DataModels.Auth;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「小程序微信认证 / 安全认证」域（<c>/wxa/sec/*</c>，5 端点 / 一支接口）契约守卫。
/// </summary>
/// <remarks>
/// <para><b>落位裁决</b>：本域归开放平台线而非小程序线——令牌面是 <c>authorizer_access_token</c>（第三方平台代授权方小程序发起认证），
/// 由留档 <c>MP-D-BUCKET-RULING-01</c> 与小程序线守卫 <c>NC2</c> 双向锁定（小程序线对 5 条认证路由零声明）。
/// 本守卫为该裁决的本线镜像。</para>
/// <para><b>官方反直觉点（照抄不修正）</b>：<c>/wxa/sec/authidentitytree</c> 官方动词为 <b>GET</b>（清单误记 POST）；
/// <c>wxaauth</c>（首次提审）与 <c>reauth</c>（重新提审）请求体子树与应答逐字段相同 ⇒ 共用 DTO；
/// <c>uploadauthmaterial</c> 为 multipart 而非 JSON。逐页取证见接口 XML 注释。</para>
/// </remarks>
public class OpenPlatformAuthContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformAuthService).Assembly;

    private static readonly Assembly DataModelsAssembly = typeof(OpenPlatformAuthData).Assembly;

    private static readonly (string Method, string Route)[] ExpectedEndpoints =
    {
        ("GET", "/wxa/sec/authidentitytree"),
        ("POST", "/wxa/sec/wxaauth"),
        ("POST", "/wxa/sec/reauth"),
        ("POST", "/wxa/sec/queryauth"),
        ("POST", "/wxa/sec/uploadauthmaterial"),
    };

    /// <summary>
    /// AUTH1：一支接口 / 5 端点的（方法, 路由）表逐条锁定，且动词分布 = 1 GET（身份树）+ 4 POST。
    /// </summary>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutes_WhenEnumerated()
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformAuthService)}")!;

        var actual = MethodsOf(type)
            .Select(static m =>
            {
                var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
                return (attr.HttpMethod.Method.ToUpperInvariant(), attr.RequestUri!);
            })
            .ToArray();

        actual.Should().HaveCount(5, "本域恰 5 条端点——少一条即缺口或扫描空跑");
        actual.Should().BeEquivalentTo(ExpectedEndpoints,
            because: "官方 weapp-wxverify 五页正文的请求地址与动词逐一锁定；authidentitytree 官方为 GET");
    }

    /// <summary>
    /// AUTH2：令牌语义（授权方令牌、参数名 access_token、Query 注入）+ 注册组 <c>Auth</c>。
    /// </summary>
    [Fact]
    public void TokenSemantics_ShouldBeAuthorizerQuery_WhenInterfaceDeclared()
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformAuthService)}")!;

        var token = type.GetCustomAttributes<TokenAttribute>(false).Single();
        token.TokenType.Should().Be(OpenPlatformTokenTypes.AuthorizerAccessToken,
            "本域代授权方小程序发起认证，取平台自身 component_access_token 即令牌串号");
        token.Name.Should().Be("access_token",
            "官方五页正文的 query 参数名为 access_token（值才是 authorizer_access_token）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query,
            "本域无 Header 令牌形态，官方一律 Query 传递");

        var api = type.GetCustomAttributes<HttpClientApiAttribute>(false).Single();
        api.RegistryGroupName.Should().Be("Auth",
            "注册组漂移即 AddModules(Auth) 无法寻址本域");
    }

    /// <summary>
    /// AUTH3：目录 / 命名空间 / 注册入口三处一致（模块枚举 + builder 委托 + JSON resolver 合并登记）。
    /// </summary>
    [Fact]
    public void RegistrationSurface_ShouldBeWired_WhenModuleAdded()
    {
        Enum.GetValues<OpenPlatformModule>().Should().Contain(OpenPlatformModule.Auth,
            "模块枚举缺值 ⇒ AddModules 无法寻址本域");

        var repoRoot = FindRepoRoot();
        var builderSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformServiceBuilder.cs"));
        builderSource.Should().Contain("[OpenPlatformModule.Auth] = static s => s.AddAuthWebApiHttpClient()",
            "注册入口必须委托到生成器按注册组产出的 AddAuthWebApiHttpClient()");

        foreach (var module in Enum.GetValues<OpenPlatformModule>())
        {
            builderSource.Should().Contain($"[OpenPlatformModule.{module}] =",
                $"模块 {module} 无注册委托 ⇒ AddAllApis() 漏注册");
        }

        var resolverSource = File.ReadAllText(Path.Combine(repoRoot,
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/Extensions/OpenPlatformJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("AuthJsonContext.Default",
            "域上下文未并进合并 resolver ⇒ 运行期退回反射序列化，AOT 下直接 IL 诊断或抛异常");
    }

    /// <summary>
    /// AUTH4：本域 14 个 DTO 全量登记进 <see cref="AuthJsonContext"/>，且 <c>SerializerClassName</c> 恒为域段 <c>Auth</c>。
    /// </summary>
    [Fact]
    public void DataModels_ShouldBeRegisteredInAuthJsonContext_WhenDomainSurfaceEnumerated()
    {
        var domainTypes = DomainDtoTypes();

        domainTypes.Should().HaveCount(14,
            "本域 DTO 数漂移须先核对官方参数表（提审子树 + 身份树递归 + 三应答 + 两请求），再同批重跑生成脚本");

        var context = (JsonSerializerContext)typeof(AuthJsonContext)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Auth 命名空间，必须登记进 AuthJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Auth",
                $"{type.Name} 的 SerializerClassName 必须为域段 Auth（错段即落到别的上下文，本上下文反而漏登记）");
        }
    }

    /// <summary>
    /// AUTH5：wxaauth 与 reauth 共用提审请求/应答、authidentitytree 为 GET 零请求体、uploadauthmaterial 走 multipart 非 JSON。
    /// </summary>
    [Fact]
    public void SharedAndSpecialFormEndpoints_ShouldStayAsOfficial()
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformAuthService)}")!;

        // 首次提审与重新提审：请求体子树逐字相同 ⇒ 共用 SubmitRequest / SubmitResponse。
        BodyParamType(type, "SubmitAuthAsync").Should().Be(typeof(OpenPlatformAuthSubmitRequest));
        BodyParamType(type, "ReauthAsync").Should().Be(typeof(OpenPlatformAuthSubmitRequest),
            "wxaauth 与 reauth 的 auth_data 子树官方逐字段相同，各造一支 DTO 即契约失真");
        ResponseType(type, "SubmitAuthAsync").Should().Be(typeof(OpenPlatformAuthSubmitResponse));
        ResponseType(type, "ReauthAsync").Should().Be(typeof(OpenPlatformAuthSubmitResponse),
            "两支提审端点应答表相同 ⇒ 共用 SubmitResponse");

        // 身份树：GET、无请求体、无显式 [Query]（仅令牌注入）。
        var tree = type.GetMethod("GetAuthIdentityTreeAsync")!;
        tree.GetParameters().Should().ContainSingle(p => p.Name == "cancellationToken",
            "authidentitytree 官方 GET 且零业务请求参数——不得造请求 DTO");
        tree.GetParameters().Should().NotContain(p => p.GetCustomAttributes<BodyAttribute>(false).Any());

        // 补充材料上传：multipart 表单（IFormContent + [MultipartForm]），非 JSON 请求体 DTO。
        var upload = type.GetMethod("UploadAuthMaterialAsync")!;
        var form = upload.GetParameters()
            .FirstOrDefault(static p => p.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "MultipartFormAttribute"));
        form.Should().NotBeNull("uploadauthmaterial 官方为 multipart/form-data，须走 [MultipartForm] IFormContent 通路");
        form!.ParameterType.Should().Be(typeof(IFormContent),
            "表单由宿主拼装（二进制字段无法用 JSON DTO 承载）");
    }

    /// <summary>
    /// AUTH6：官方字段名/类型照抄——提审子树 snake_case 键、数值字段 number、应答 queryauth 的类型分叉。
    /// </summary>
    [Theory]
    [InlineData(typeof(OpenPlatformAuthSubmitRequest), "AuthData", "auth_data")]
    [InlineData(typeof(OpenPlatformAuthData), "CustomerType", "customer_type")]
    [InlineData(typeof(OpenPlatformAuthData), "TaskId", "taskid")]
    [InlineData(typeof(OpenPlatformAuthData), "ContactInfo", "contact_info")]
    [InlineData(typeof(OpenPlatformAuthData), "InvoiceInfo", "invoice_info")]
    [InlineData(typeof(OpenPlatformAuthData), "QualificationOther", "qualification_other")]
    [InlineData(typeof(OpenPlatformAuthData), "AccountNameType", "account_name_type")]
    [InlineData(typeof(OpenPlatformAuthData), "AuthIdentMaterial", "auth_ident_material")]
    [InlineData(typeof(OpenPlatformAuthData), "ThirdPartyPhone", "third_party_phone")]
    [InlineData(typeof(OpenPlatformAuthData), "ServiceAppId", "service_appid")]
    [InlineData(typeof(OpenPlatformAuthInvoiceVatInfo), "EnterprisePhone", "enterprise_phone")]
    [InlineData(typeof(OpenPlatformAuthQueryRequest), "TaskId", "taskid")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "TaskStatus", "task_status")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "ApplyStatus", "apply_status")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "OrderId", "orderid")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "RefillReason", "refill_reason")]
    [InlineData(typeof(OpenPlatformAuthUploadMaterialResponse), "MediaId", "mediaid")]
    [InlineData(typeof(OpenPlatformAuthIdentityTreeNode), "NodeId", "node_id")]
    [InlineData(typeof(OpenPlatformAuthIdentityTreeNode), "NodeList", "node_list")]
    [InlineData(typeof(OpenPlatformAuthIdentityTreeNode), "LeafInfo", "leaf_info")]
    [InlineData(typeof(OpenPlatformAuthIdentityTreeResponse), "IdentityTreeList", "identity_tree_list")]
    public void OfficialJsonKeyNames_ShouldBePreserved(Type dtoType, string property, string jsonKey)
    {
        var pi = dtoType.GetProperty(property)
            ?? throw new InvalidOperationException($"{dtoType.Name} 缺属性 {property}（官方字段名漂移）");
        pi.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonKey,
            $"{dtoType.Name}.{property} 的 JSON 键是官方页面原文，改名即字段静默丢失");
    }

    /// <summary>AUTH6b：官方标 number 的字段本仓以整型 long 承载，不写成字符串。</summary>
    [Theory]
    [InlineData(typeof(OpenPlatformAuthData), "CustomerType")]
    [InlineData(typeof(OpenPlatformAuthData), "AccountNameType")]
    [InlineData(typeof(OpenPlatformAuthData), "PayType")]
    [InlineData(typeof(OpenPlatformAuthInvoiceInfo), "InvoiceType")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "TaskStatus")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "ApplyStatus")]
    [InlineData(typeof(OpenPlatformAuthQueryResponse), "OrderId")]
    [InlineData(typeof(OpenPlatformAuthIdentityTreeNode), "NodeId")]
    [InlineData(typeof(OpenPlatformAuthIdentityTreeRootInfo), "Type")]
    public void OfficialNumericFields_ShouldBeIntegral(Type dtoType, string property)
    {
        var pi = dtoType.GetProperty(property)!;
        var underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
        underlying.Should().Be(typeof(long),
            $"{dtoType.Name}.{property} 官方为 number，本仓以整型 long 承载（写成 string 即契约失真）");
    }

    /// <summary>
    /// AUTH7：递归身份树自指——<c>node_list</c> 的元素类型必须是节点本身（非 <c>object</c> / 非拍平）。
    /// </summary>
    [Fact]
    public void IdentityTree_ShouldBeRecursive()
    {
        var nodeList = typeof(OpenPlatformAuthIdentityTreeNode).GetProperty("NodeList")!;
        nodeList.PropertyType.IsGenericType.Should().BeTrue("node_list 为泛型列表");
        nodeList.PropertyType.GetGenericArguments()[0]
            .Should().Be(typeof(OpenPlatformAuthIdentityTreeNode),
                "官方 node_list 是与本节点同形状的递归数组；拍平或退化为 object 即丢失树语义");
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

    private static Type ResponseType(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName)!;
        return method.ReturnType.GetGenericArguments()[0];
    }

    private static IEnumerable<MethodInfo> MethodsOf(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == interfaceType
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    private static Type[] DomainDtoTypes()
        => DataModelsAssembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsNested
                               && t.Namespace == "Mud.Wechat.OpenPlatform.DataModels.Auth")
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
