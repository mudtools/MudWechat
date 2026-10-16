// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.WebizpayUser;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-D 微企付企业支付「员工开通授权」域契约守卫 —— <b>2 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>两支是「商户员工 ID ↔ 微信授权关系 ID」互查的两端</b>：<c>user_id</c>（官方【商户企业员工 ID】， 6-32 个字符、只能是数字、大小写字母 <c>-_|*</c> 且同一商户号下唯一）在 path， 授权状态支应答的 <c>employee_list</c> 回 <c>employee_id</c>（<c>WeBizPay_…</c>）⇒ 额度卡域各支要的是 <c>employee_id</c>，须先由本域授权状态支取得。</para>
/// <para><b>200 应答里零链接不是错误</b>：授权链接支 Body <c>application_type</c> 必填（<c>H5</c> / <c>MINIPROGRAM</c>）， 应答 8 行中 <c>authorization_url</c> / <c>expire_time</c> / <c>mp_query</c> / <c>mp_business_type</c> <b>全为选填</b>， 官方口径「仅当可以开通时返回」⇒ 调用侧须按「有链接 / 无链接」分支，不得断言非空。</para>
/// <para><b>小程序参数有效期 35 分钟</b>（比跳转链接更短）且仅在 <c>MINIPROGRAM</c> 且允许拉起时返回； 其消费面是小程序 JSAPI <c>wx.openBusinessView</c>（官方页 4014997783，【普通商户】侧、非 HTTP 端点）⇒ <b>本域只透传 <c>mp_query</c> / <c>mp_business_type</c>，零端点</b>。</para>
/// <para><b>微企付另有 5 支「通知」页</b>（关系授权完成 4014612130、额度卡失效 4015764389、支付成功 4015764398、 退款成功 4015764405、解除授权 4015764414）：方向是<b>微信 → 商户回调地址</b>、回调地址由运营配置， 不属商户侧主动调用的 HTTP 契约面 ⇒ <b>本域零端点</b>（回调落地属支付线回调面另案）。</para>
/// <para><b>授权状态支 <c>employee_list</c> 可为 0 长度</b>（官方：同一商户下相同企业员工 ID 未开通过企业支付）， 列表元素四字段（<c>employee_id</c> / <c>authorization_state</c> / <c>authorization_time</c> / <c>authorization_revoked_time</c>）全必填 ⇒ 空数组是正常态、不是失败。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayWebizpayUserContractGuards
{
    private const string RegistryGroupName = "WebizpayUser";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.WebizpayUser";

    private static readonly Type Service = typeof(IWechatPayWebizpayUserService);

    /// <summary>WEB-US-B1：拓扑 —— 一个注册组一个接口、2 方法、2 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/webizpay/users。</summary>
    [Fact]
    public void WebizpayUserTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(2, "员工开通授权声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(2, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/webizpay/users", StringComparison.Ordinal),
            "本域路由全在 /v3/webizpay/users 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>WEB-US-B2：2 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void WebizpayUserEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 企业商户发起员工开通企业支付授权｜官方页 partner/4014507153｜身份 【普通服务商】｜官方页更新 2025.05.09
        RouteOf(nameof(IWechatPayWebizpayUserService.CreateUserAuthorizationUrlAsync)).Should().Be("/v3/webizpay/users/{userId}/authorization-url");
        PathKeysOf(nameof(IWechatPayWebizpayUserService.CreateUserAuthorizationUrlAsync)).Should().BeEquivalentTo(new[] { "userId" });
        QueryNamesOf(nameof(IWechatPayWebizpayUserService.CreateUserAuthorizationUrlAsync)).Should().BeEmpty();

        // 企业商户查询企业员工开通授权状态｜官方页 partner/4014507163｜身份 【普通服务商】｜官方页更新 2025.05.09
        RouteOf(nameof(IWechatPayWebizpayUserService.QueryUserAuthorizationStateAsync)).Should().Be("/v3/webizpay/users/{userId}/authorization-state");
        PathKeysOf(nameof(IWechatPayWebizpayUserService.QueryUserAuthorizationStateAsync)).Should().BeEquivalentTo(new[] { "userId" });
        QueryNamesOf(nameof(IWechatPayWebizpayUserService.QueryUserAuthorizationStateAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayWebizpayUserService.QueryUserAuthorizationStateAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" }, "企业商户查询企业员工开通授权状态的 sp_mchid / sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayWebizpayUserService.QueryUserAuthorizationStateAsync)).Should().BeNull("企业商户查询企业员工开通授权状态官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>WEB-US-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void WebizpayUserDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<WebizpayUserAuthorizationUrlRequest>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "application_type" },
            "企业商户发起员工开通企业支付授权请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayUserAuthorizationUrlResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "user_id", "authorization_url", "expire_time", "application_type", "mp_query", "mp_business_type" },
            "企业商户发起员工开通企业支付授权应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayUserAuthorizationStateResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "user_id", "employee_list" },
            "企业商户查询企业员工开通授权状态应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>WEB-US-B4：判错面 + AOT 上下文登记 + DI 与模块接线（1 请求根型 / 2 应答根型 / 4 个 DTO）。</summary>
    [Fact]
    public void WebizpayUserDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(4, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            WebizpayUserJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 WebizpayUserJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 WebizpayUser");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayUserAuthorizationUrlResponse)).Should().BeTrue("WebizpayUserAuthorizationUrlResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayUserAuthorizationStateResponse)).Should().BeTrue("WebizpayUserAuthorizationStateResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayUserAuthorizationUrlRequest)).Should().BeFalse("WebizpayUserAuthorizationUrlRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("WebizpayUserJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 WebizpayUser 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddWebizpayUserWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 WebizpayUser 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayWebizpayUserService.{methodName} 必须存在");

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
