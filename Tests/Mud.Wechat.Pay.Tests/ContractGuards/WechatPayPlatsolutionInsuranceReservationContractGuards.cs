// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReservation;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-C 平台商户保险理赔商家转账「预约商家转账（保险场景）」域契约守卫 —— <b>4 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>四支建三型</b>：受理支应答 6 行、含<b>必填</b> <c>package_info</c>（拉起微信支付客户端用户确认页的跳转信息）； 按 <c>out_reservation_no</c> 查询、按 <c>reservation_id</c> 查询与关闭三支的官方应答表<b>逐字段同形且必填口径一致</b>（18 行） ⇒ 共用 <see cref="InsuranceReservationQueryResponse"/>；<b>关闭支官方印有应答表</b> ⇒ 既不得返回裸 <c>Task</c>，也不得凭空造请求 DTO。</para>
/// <para><b>幂等与时效（官方原文）</b>：预创建的预约记录<b>24 小时内有效</b>，用户未及时确认由系统自动关闭；接口支持幂等， 幂等标识为 <c>out_reservation_no</c>、幂等有效性 <b>5 年</b> ⇒ 商户单号必须稳定可重放，不得每次随机生成。</para>
/// <para><b>敏感字段</b>：<c>user_real_name</c> 官方要求<b>加密后传入</b>（<c>string(1024)</c>，选填， 但「预计转账金额 ≥ 2,000 元时必须填写」）⇒ 请求头须带 <c>Wechatpay-Serial</c>；本 SDK <b>不做加密也不代填</b>，密文由调用侧产出后原样传入。</para>
/// <para><b>查询两支与关闭支的 <c>sub_mchid</c> 是官方必填 query</b>（受理支走 Body 必填）⇒ 同一字段在受理侧与后三支落位不同， 请求 DTO 与查询签名不得互套；<b>本域 4 支官方页均未印</b>端点级 QPS ⇒ 不得凭第三方 SDK 补数。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayPlatsolutionInsuranceReservationContractGuards
{
    private const string RegistryGroupName = "PlatsolutionInsuranceReservation";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReservation";

    private static readonly Type Service = typeof(IWechatPayPlatsolutionInsuranceReservationService);

    /// <summary>PSI-RS-B1：拓扑 —— 一个注册组一个接口、4 方法、4 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/platsolution/insurance/mch-transfer/reservation。</summary>
    [Fact]
    public void PlatsolutionInsuranceReservationTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(4, "预约商家转账（保险场景）声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(4, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/platsolution/insurance/mch-transfer/reservation", StringComparison.Ordinal),
            "本域路由全在 /v3/platsolution/insurance/mch-transfer/reservation 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>PSI-RS-B2：4 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void PlatsolutionInsuranceReservationEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 受理单次预约商家转账｜官方页 partner/4013504216｜身份 【平台商户】｜官方页更新 2025.01.21
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.ApplyInsuranceReservationAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/reservation/apply");
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.ApplyInsuranceReservationAsync)).Should().BeEmpty();

        // 商户预约单号查询预约商家转账记录｜官方页 partner/4013504218｜身份 【平台商户】｜官方页更新 2025.01.21
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByOutReservationNoAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByOutReservationNoAsync)).Should().BeEquivalentTo(new[] { "outReservationNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByOutReservationNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByOutReservationNoAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "商户预约单号查询预约商家转账记录的 sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByOutReservationNoAsync)).Should().BeNull("商户预约单号查询预约商家转账记录官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 微信支付预约单号查询预约商家转账记录｜官方页 partner/4013504217｜身份 【平台商户】｜官方页更新 2025.01.21
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByReservationIdAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/reservation/reservation-id/{reservationId}");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByReservationIdAsync)).Should().BeEquivalentTo(new[] { "reservationId" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByReservationIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByReservationIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "微信支付预约单号查询预约商家转账记录的 sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.QueryInsuranceReservationByReservationIdAsync)).Should().BeNull("微信支付预约单号查询预约商家转账记录官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 关闭预约商家转账记录｜官方页 partner/4013504219｜身份 【平台商户】｜官方页更新 2025.01.21
        RouteOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.CloseInsuranceReservationAsync)).Should().Be("/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}/close");
        PathKeysOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.CloseInsuranceReservationAsync)).Should().BeEquivalentTo(new[] { "outReservationNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.CloseInsuranceReservationAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.CloseInsuranceReservationAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "关闭预约商家转账记录的 sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPlatsolutionInsuranceReservationService.CloseInsuranceReservationAsync)).Should().BeNull("关闭预约商家转账记录官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>PSI-RS-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void PlatsolutionInsuranceReservationDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<InsuranceReservationApplyRequest>().Should().BeEquivalentTo(new[] { "sub_mchid", "sp_appid", "sub_appid", "out_reservation_no", "transfer_scene_id", "estimated_date", "estimated_max_amount", "exact_amount", "openid", "user_real_name", "reserve_remark", "notify_url", "scene_report_data" },
            "受理单次预约商家转账请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceReservationApplyResponse>().Should().BeEquivalentTo(new[] { "sub_mchid", "out_reservation_no", "reservation_id", "accept_time", "state", "package_info" },
            "受理单次预约商家转账应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceReservationQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "sp_appid", "sub_appid", "reservation_id", "out_reservation_no", "transfer_scene_id", "estimated_date", "estimated_max_amount", "exact_amount", "openid", "bank_type", "state", "accept_time", "reserve_time", "close_info" },
            "商户预约单号查询预约商家转账记录应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<InsuranceReservationQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "sp_appid", "sub_appid", "reservation_id", "out_reservation_no", "transfer_scene_id", "estimated_date", "estimated_max_amount", "exact_amount", "openid", "bank_type", "state", "accept_time", "reserve_time", "close_info" },
            "微信支付预约单号查询预约商家转账记录应答顶层字段名册（官方应答表逐字，与 <c>reservation/out-reservation-no/{out_reservation_no}</c> 同表 ⇒ 共用一个类型）");

        DeclaredJsonNamesOf<InsuranceReservationQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "sp_appid", "sub_appid", "reservation_id", "out_reservation_no", "transfer_scene_id", "estimated_date", "estimated_max_amount", "exact_amount", "openid", "bank_type", "state", "accept_time", "reserve_time", "close_info" },
            "关闭预约商家转账记录应答顶层字段名册（官方应答表逐字，与 <c>reservation/out-reservation-no/{out_reservation_no}</c> 同表 ⇒ 共用一个类型）");
    }

    /// <summary>PSI-RS-B4：判错面 + AOT 上下文登记 + DI 与模块接线（1 请求根型 / 2 应答根型 / 6 个 DTO）。</summary>
    [Fact]
    public void PlatsolutionInsuranceReservationDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(6, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            PlatsolutionInsuranceReservationJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PlatsolutionInsuranceReservationJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 PlatsolutionInsuranceReservation");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceReservationApplyResponse)).Should().BeTrue("InsuranceReservationApplyResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceReservationQueryResponse)).Should().BeTrue("InsuranceReservationQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(InsuranceReservationApplyRequest)).Should().BeFalse("InsuranceReservationApplyRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PlatsolutionInsuranceReservationJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PlatsolutionInsuranceReservation 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPlatsolutionInsuranceReservationWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 PlatsolutionInsuranceReservation 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayPlatsolutionInsuranceReservationService.{methodName} 必须存在");

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
