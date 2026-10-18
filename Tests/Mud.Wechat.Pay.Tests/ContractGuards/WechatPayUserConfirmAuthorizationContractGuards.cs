// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.UserConfirmAuthorization;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-A FundApp 族「免确认收款授权」域契约守卫 —— <b>3 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>三段式生命周期不可折叠</b>：创建 <c>POST /user-confirm-authorization</c> → 查询 <c>GET .../out-authorization-no/{out_authorization_no}</c> → 解除 <c>POST .../close</c>；应答<b>不同形</b>（创建 4 行、查询 12 行、关闭 11 行）⇒ 各建一型。</para>
/// <para><b>本域唯一 query 是 <c>is_display_authorization</c></b>（查询支，官方<b>选填</b>布尔）⇒ 不得做成必填；关闭支的定位键只有商户侧授权单号一种。</para>
/// <para><b>创建支请求体含敏感字段</b>：<c>user_name</c> 官方要求 RSA-OAEP 密文上送 ⇒ 宿主须在调用前完成加密，SDK 不做本地加密。</para>
/// <para><b>三支明印端点级 QPS（两档 50 / 100 次/s）</b>，与相邻转账到 QQ 钱包域的三档不同形 ⇒ 逐支照页。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayUserConfirmAuthorizationContractGuards
{
    private const string RegistryGroupName = "UserConfirmAuthorization";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.UserConfirmAuthorization";

    private static readonly Type Service = typeof(IWechatPayUserConfirmAuthorizationService);

    /// <summary>UCA-B1：拓扑 —— 一个注册组一个接口、3 方法、3 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/fund-app/mch-transfer/user-confirm-authorization。</summary>
    [Fact]
    public void UserConfirmAuthorizationTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(3, "免确认收款授权声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(3, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/fund-app/mch-transfer/user-confirm-authorization", StringComparison.Ordinal),
            "本域路由全在 /v3/fund-app/mch-transfer/user-confirm-authorization 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>UCA-B2：3 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void UserConfirmAuthorizationEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 发起免确认收款授权｜官方页 merchant/4015901167｜身份 【普通商户】｜官方页更新 2026.05.08
        RouteOf(nameof(IWechatPayUserConfirmAuthorizationService.CreateUserConfirmAuthorizationAsync)).Should().Be("/v3/fund-app/mch-transfer/user-confirm-authorization");
        QueryNamesOf(nameof(IWechatPayUserConfirmAuthorizationService.CreateUserConfirmAuthorizationAsync)).Should().BeEmpty();

        // 免确认收款授权结果查询｜官方页 merchant/4014399423｜身份 【普通商户】｜官方页更新 2026.05.08
        RouteOf(nameof(IWechatPayUserConfirmAuthorizationService.QueryUserConfirmAuthorizationAsync)).Should().Be("/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}");
        PathKeysOf(nameof(IWechatPayUserConfirmAuthorizationService.QueryUserConfirmAuthorizationAsync)).Should().BeEquivalentTo(new[] { "outAuthorizationNo" });
        QueryNamesOf(nameof(IWechatPayUserConfirmAuthorizationService.QueryUserConfirmAuthorizationAsync)).Should().BeEquivalentTo(new[] { "is_display_authorization" });
        RequiredQueryOf(nameof(IWechatPayUserConfirmAuthorizationService.QueryUserConfirmAuthorizationAsync)).Should().BeEmpty("免确认收款授权结果查询的 query 官方全为选填 ⇒ 声明式签名一律给可空缺省，不得做成必填");
        BodyTypeOf(nameof(IWechatPayUserConfirmAuthorizationService.QueryUserConfirmAuthorizationAsync)).Should().BeNull("免确认收款授权结果查询官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 解除免确认收款授权｜官方页 merchant/4015653811｜身份 【普通商户】｜官方页更新 2026.05.08
        RouteOf(nameof(IWechatPayUserConfirmAuthorizationService.CloseUserConfirmAuthorizationAsync)).Should().Be("/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}/close");
        PathKeysOf(nameof(IWechatPayUserConfirmAuthorizationService.CloseUserConfirmAuthorizationAsync)).Should().BeEquivalentTo(new[] { "outAuthorizationNo" });
        QueryNamesOf(nameof(IWechatPayUserConfirmAuthorizationService.CloseUserConfirmAuthorizationAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayUserConfirmAuthorizationService.CloseUserConfirmAuthorizationAsync)).Should().BeNull("解除免确认收款授权官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>UCA-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void UserConfirmAuthorizationDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<UserConfirmAuthorizationCreateRequest>().Should().BeEquivalentTo(new[] { "out_authorization_no", "appid", "openid", "transfer_scene_id", "user_display_name", "user_recv_perception", "authorization_notify_url", "scene_info" },
            "发起免确认收款授权请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<UserConfirmAuthorizationCreateResponse>().Should().BeEquivalentTo(new[] { "out_authorization_no", "state", "create_time", "package_info" },
            "发起免确认收款授权应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<UserConfirmAuthorizationQueryResponse>().Should().BeEquivalentTo(new[] { "out_authorization_no", "appid", "openid", "user_display_name", "authorization_id", "state", "authorize_time", "close_info", "transfer_scene_id", "user_recv_perception", "create_time", "package_info" },
            "免确认收款授权结果查询应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<UserConfirmAuthorizationCloseResponse>().Should().BeEquivalentTo(new[] { "out_authorization_no", "appid", "openid", "user_display_name", "authorization_id", "state", "authorize_time", "close_info", "transfer_scene_id", "user_recv_perception", "create_time" },
            "解除免确认收款授权应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>UCA-B4：判错面 + AOT 上下文登记 + DI 与模块接线（1 请求根型 / 3 应答根型 / 7 个 DTO）。</summary>
    [Fact]
    public void UserConfirmAuthorizationDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(7, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            UserConfirmAuthorizationJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 UserConfirmAuthorizationJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 UserConfirmAuthorization");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(UserConfirmAuthorizationCreateResponse)).Should().BeTrue("UserConfirmAuthorizationCreateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(UserConfirmAuthorizationQueryResponse)).Should().BeTrue("UserConfirmAuthorizationQueryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(UserConfirmAuthorizationCloseResponse)).Should().BeTrue("UserConfirmAuthorizationCloseResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(UserConfirmAuthorizationCreateRequest)).Should().BeFalse("UserConfirmAuthorizationCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("UserConfirmAuthorizationJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 UserConfirmAuthorization 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddUserConfirmAuthorizationWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 UserConfirmAuthorization 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayUserConfirmAuthorizationService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

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
