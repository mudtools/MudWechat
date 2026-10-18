// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MarketingBank;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-3 营销批次「银行营销」域契约守卫 —— <b>2 端点</b>（券码包上传任务创建 + 任务列表查询）。
/// </summary>
/// <remarks>
/// <para><b>路由以 <c>package_id</c> 走 path，且两支同段不同动词</b>：创建是 <c>POST …/packages/{package_id}/tasks</c>、列表是 <c>GET …/packages/{package_id}/tasks</c> ⇒ (方法, 路由) 两条、无定位键差异（MBT-B1）。</para>
/// <para><b>分页口径写在描述里而不是类型上限</b>：官方列表支 <c>offset</c>/<c>limit</c> 标<b>选填</b>，「单次最多 20 条」是描述文字 ⇒ SDK 不做本地拦截，更不得把它当成必填（MBT-B2）。</para>
/// <para><b>本域与银行支付分（<c>BankPayScore</c>）、银行转账（<c>BankTransfer</c>）是三条独立路由族</b>：<c>/v3/marketing/bank/</c> 之外的银行面互不交叠，令牌与签名主体也不同（商户签名，无 access_token）。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayMarketingBankContractGuards
{
    private const string RegistryGroupName = "MarketingBank";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingBank";

    private static readonly Type Service = typeof(IWechatPayMarketingBankService);

    /// <summary>MBT-B1：拓扑 —— 一个注册组一个接口、2 个方法、2 条 (方法, 路由)、动词集 POST / GET；路由前缀 /v3/marketing/bank/。</summary>
    [Fact]
    public void MarketingBankTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(2, "银行营销券码包任务声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(2, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/marketing/bank/", StringComparison.Ordinal),
            "本域路由全在 /v3/marketing/bank/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>MBT-B2：2 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void MarketingBankEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayMarketingBankService.CreateBankPackageTaskAsync)).Should().Be("/v3/marketing/bank/packages/{packageId}/tasks");
        PathKeysOf(nameof(IWechatPayMarketingBankService.CreateBankPackageTaskAsync)).Should().BeEquivalentTo(new[] { "packageId" });
        // 银行营销券码包上传任务｜官方页 merchant/4012466703 + partner/4012466728｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBankService.QueryBankPackageTasksAsync)).Should().Be("/v3/marketing/bank/packages/{packageId}/tasks");
        PathKeysOf(nameof(IWechatPayMarketingBankService.QueryBankPackageTasksAsync)).Should().BeEquivalentTo(new[] { "packageId" });
        QueryNamesOf(nameof(IWechatPayMarketingBankService.QueryBankPackageTasksAsync)).Should().BeEquivalentTo(new[] { "filename", "offset", "limit", "status" });
        // 银行营销券码包上传任务列表｜官方页 merchant/4012466699 + partner/4012466722｜身份 商户/服务商
    }

    /// <summary>MBT-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void MarketingBankDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<MarketingBankPackageTaskCreateResponse>().Should().BeEquivalentTo(new[] { "task_id", "package_id", "filename", "create_time", "update_time", "status", "success_count", "fail_count", "success_user_count", "bank_type" },
            "银行营销券码包上传任务应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<MarketingBankPackageTaskQueryResponse>().Should().BeEquivalentTo(new[] { "data", "total_count", "offset", "limit" },
            "银行营销券码包上传任务列表应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>MBT-B4：判错面 + AOT 上下文登记 + DI 与模块接线（0 请求型 / 2 应答型）。</summary>
    [Fact]
    public void MarketingBankDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(3, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            MarketingBankJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingBankJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MarketingBank");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(MarketingBankPackageTaskCreateResponse), typeof(MarketingBankPackageTaskQueryResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        // 本域官方页面<b>没有请求体表</b>（参数全在 path / query）⇒ 没有任何方法带 [Body]。
        Methods().SelectMany(static m => m.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("官方 Body 表为空：造一个请求 DTO 即上送官方不认的报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MarketingBankJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MarketingBank 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMarketingBankWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 MarketingBank 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMarketingBankService.{methodName} 必须存在");

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
