// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-B 平台服务商解决方案「平台商户付款（商户到商户）」域契约守卫 —— <b>3 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>本域是「付款给商户」</b>（<c>receiver_detail.receiver.mch_info</c>），与赔付 / 理赔 / 保证金三族共用 <c>/v3/platsolution/ecommerce/mch-transfer/*</c> 前缀但应答字段集各异 ⇒ 类型不得跨族复用。</para>
/// <para><b>两支查询应答逐字段同形</b>（19 行）⇒ 共用 <see cref="MchTransferQueryResponse"/>； 按 <c>bill_id</c> 支官方<b>同时</b>给出 <c>sub_mchid</c> 与 <c>business_type</c> 两个选填 query， 按 <c>out_bill_no</c> 支只给 <c>business_type</c> ⇒ 照页逐支声明，不得合并成一套参数。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayPlatsolutionEcommerceMchTransferContractGuards
{
    private const string RegistryGroupName = "PlatsolutionEcommerceMchTransfer";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer";

    private static readonly Type Service = typeof(IWechatPayPlatsolutionEcommerceMchTransferService);

    /// <summary>PEC-MT-B1：拓扑 —— 一个注册组一个接口、3 方法、3 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/platsolution/ecommerce/mch-transfer/transfer-bills。</summary>
    [Fact]
    public void PlatsolutionEcommerceMchTransferTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(3, "平台商户付款（商户到商户）声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(3, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/platsolution/ecommerce/mch-transfer/transfer-bills", StringComparison.Ordinal),
            "本域路由全在 /v3/platsolution/ecommerce/mch-transfer/transfer-bills 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>PEC-MT-B2：3 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void PlatsolutionEcommerceMchTransferEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 请求商户付款｜官方页 partner/4013504198｜身份 【平台商户】｜官方页更新 2025.03.21
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.CreateMchTransferAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/transfer-bills");
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.CreateMchTransferAsync)).Should().BeEmpty();

        // 商户单号查询商户付款记录｜官方页 partner/4013504199｜身份 【平台商户】｜官方页更新 2025.03.21
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByOutBillNoAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}");
        PathKeysOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByOutBillNoAsync)).Should().BeEquivalentTo(new[] { "outBillNo" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByOutBillNoAsync)).Should().BeEquivalentTo(new[] { "business_type" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByOutBillNoAsync)).Should().BeEmpty("商户单号查询商户付款记录的 query 官方全为选填 ⇒ 声明式签名一律给可空缺省，不得做成必填");
        BodyTypeOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByOutBillNoAsync)).Should().BeNull("商户单号查询商户付款记录官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 微信支付单号查询商户付款记录｜官方页 partner/4013504200｜身份 【平台商户】｜官方页更新 2025.03.21
        RouteOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByBillIdAsync)).Should().Be("/v3/platsolution/ecommerce/mch-transfer/transfer-bills/bill-id/{billId}");
        PathKeysOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByBillIdAsync)).Should().BeEquivalentTo(new[] { "billId" });
        QueryNamesOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByBillIdAsync)).Should().BeEquivalentTo(new[] { "sub_mchid", "business_type" });
        RequiredQueryOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByBillIdAsync)).Should().BeEmpty("微信支付单号查询商户付款记录的 query 官方全为选填 ⇒ 声明式签名一律给可空缺省，不得做成必填");
        BodyTypeOf(nameof(IWechatPayPlatsolutionEcommerceMchTransferService.QueryMchTransferByBillIdAsync)).Should().BeNull("微信支付单号查询商户付款记录官方无请求体 ⇒ 不得凭空造请求 DTO");
    }

    /// <summary>PEC-MT-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void PlatsolutionEcommerceMchTransferDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<MchTransferCreateRequest>().Should().BeEquivalentTo(new[] { "sub_mchid", "sp_appid", "business_type", "receiver", "out_bill_no", "amount", "transfer_remark", "receiver_remark" },
            "请求商户付款请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<MchTransferCreateResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "sp_appid", "business_type", "receiver_detail", "out_bill_no", "amount", "transfer_remark", "receiver_remark", "bill_id", "state", "accept_time", "success_time", "sponsor_mchid" },
            "请求商户付款应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<MchTransferQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "business_type", "receiver_detail", "out_bill_no", "amount", "transfer_remark", "receiver_remark", "bill_id", "state", "accept_time", "success_time", "close_info", "sponsor_mchid" },
            "商户单号查询商户付款记录应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<MchTransferQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "business_type", "receiver_detail", "out_bill_no", "amount", "transfer_remark", "receiver_remark", "bill_id", "state", "accept_time", "success_time", "close_info", "sponsor_mchid" },
            "微信支付单号查询商户付款记录应答顶层字段名册（官方应答表逐字，与 <c>mch-transfer/transfer-bills/out-bill-no/{out_bill_no}</c> 同表 ⇒ 共用一个类型）");
    }

    /// <summary>PEC-MT-B4：判错面 + AOT 上下文登记 + DI 与模块接线（1 请求根型 / 2 应答根型 / 12 个 DTO）。</summary>
    [Fact]
    public void PlatsolutionEcommerceMchTransferDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(12, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            PlatsolutionEcommerceMchTransferJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PlatsolutionEcommerceMchTransferJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 PlatsolutionEcommerceMchTransfer");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(MchTransferCreateResponse)).Should().BeTrue("MchTransferCreateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(MchTransferQueryResponse)).Should().BeTrue("MchTransferQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(MchTransferCreateRequest)).Should().BeFalse("MchTransferCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PlatsolutionEcommerceMchTransferJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PlatsolutionEcommerceMchTransfer 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPlatsolutionEcommerceMchTransferWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 PlatsolutionEcommerceMchTransfer 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayPlatsolutionEcommerceMchTransferService.{methodName} 必须存在");

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
