// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.PayInstallment;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「PayInstallment」契约守卫 —— 本批 <b>1 端点</b>（注册组 <c>PayInstallment</c>，接口 <c>IWechatPayPayInstallmentService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.PayInstallment</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中<b>1 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：1 / 1 支官方页<b>印有</b>端点级数字（已原样抄入各方法 remarks），其余支<b>未印</b> ⇒ 未印者不得凭第三方 SDK 补数。</para>
/// <para><b>一支、官方只印商户页，且「查询」语义走 POST</b>：<c>POST /v3/pay/instal/exposure</c>（原文请求方式即 POST）⇒ 照页不得改成 GET。</para>
/// <para><b>官方把限速与耗时都印在正文</b>：「接口频率：<b>每个商户限流200/s</b>，上线前必须与微信支付运营确认」 +「接口耗时：200ms」⇒ 数字照页入 remarks，且「上线前须与运营确认」是<b>前置动作</b>，不得省略为纯技术阈值。</para>
/// <para><b>入参两键联动、SDK 不做本地校验</b>：<c>user_id_type</c> 决定用户标识口径，<c>openid</c> 官方标<b>选填</b> （传 OPENID 口径时才需给出）⇒ 两者均以可空参数承载，联动关系写在注释而<b>不</b>做成必传参数或抛异常。</para>
/// <para><b>应答 <c>repayment_info</c> 是 array[object] 且官方给元素两键</b>（<c>repayment_type</c>、 <c>exist_cash_off_coupon</c>）⇒ 建元素类型而非 <c>JsonElement</c>；根键 <c>recommend_exposure</c> 是必填布尔， 含义是「<b>建议曝光</b>」而非「用户已开通分付」⇒ 不得据其判定开通态，只作曝光决策入参。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayPayInstallmentContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PayInstallment";

    private static readonly Type[] Services = { typeof(IWechatPayPayInstallmentService) };

    /// <summary>PIN-B1：拓扑 —— 注册组 PayInstallment、本批 1 方法（接口实测 1）、1 条 (动词, 路由)；动词集 Post。</summary>
    [Fact]
    public void PayInstallmentTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayPayInstallmentService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("PayInstallment", "组名即源生成注册入口名（改组名即改 AddPayInstallmentWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(1, "PayInstallment 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(1, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute" }, "本域唯一一支官方印 POST（曝光建议查询走 POST，请求体带用户标识）⇒ 不得按「查询即 GET」改成 GET + query");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域唯一路由是 /v3/pay/instal/exposure（路由段是 instal，不是 installment）⇒ 照页不得「补全」拼写");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayCombineService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayCombineService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>PIN-B2：本批 1 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void PayInstallmentEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 获取微信分付曝光建议｜官方页 v3/merchant/4016196762｜身份 【普通商户】
        RouteOf(nameof(IWechatPayPayInstallmentService.QueryPayInstallmentExposureAsync)).Should().Be("/v3/pay/instal/exposure");
        PathKeysOf(nameof(IWechatPayPayInstallmentService.QueryPayInstallmentExposureAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayPayInstallmentService.QueryPayInstallmentExposureAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayPayInstallmentService.QueryPayInstallmentExposureAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayPayInstallmentService.QueryPayInstallmentExposureAsync)).Should().Be(typeof(PayInstallmentExposureRequest));
        ReturnTypeOf(nameof(IWechatPayPayInstallmentService.QueryPayInstallmentExposureAsync)).Should().Be(typeof(PayInstallmentExposureResponse));
    }

    /// <summary>PIN-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void PayInstallmentDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<PayInstallmentExposureRequest>().Should().BeEquivalentTo(NAMES["PayInstallmentExposureRequest"], "PayInstallmentExposureRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<PayInstallmentExposureResponse>().Should().BeEquivalentTo(NAMES["PayInstallmentExposureResponse"], "PayInstallmentExposureResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>PIN-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void PayInstallmentDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayInstallmentExposureResponse)).Should().BeTrue("PayInstallmentExposureResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(PayInstallmentExposureRequest)).Should().BeFalse("PayInstallmentExposureRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>PIN-B5：本批 DTO 全量形态 —— 3 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void PayInstallmentAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(3, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            PayInstallmentJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 PayInstallmentJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("PayInstallment", type.Name + " 的 SerializerClassName 必须为 PayInstallment");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 名册与 B3 的期望值同源：每个根型的全部名单都必须出现在 NAMES 表里（防「B3 漏列某型」）。
        foreach (var type in batchDtos.Where(t => NAMES.ContainsKey(t.Name)))
            DeclaredJsonNamesOf(type).Should().BeEquivalentTo(NAMES[type.Name], type.Name + " 的名单与 B3 期望值必须同源");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>PIN-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void PayInstallmentRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("PayInstallment", "PayModule 须有 PayInstallment 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPayInstallmentWebApiHttpClient()", "注册委托必须指向源生成方法（组名 PayInstallment 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PayInstallmentJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["PayInstallmentExposureRequest"] = new[] { "mchid", "appid", "user_id_type", "openid" },
        ["PayInstallmentExposureResponse"] = new[] { "recommend_exposure", "repayment_info" },
    };

    // ---- helpers -------------------------------------------------------------

    private static object? ApiProp(Type service, string name)
    {
        var api = service.GetCustomAttributes(false).Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        return api.GetType().GetProperty(name)!.GetValue(api);
    }

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException(methodName + " 必须存在于本域接口");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull(methodName + " 必须声明动词特性");
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
        => Services.SelectMany(static s => s.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .ToArray();

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
            .Select(static a => a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant() + " " + a.RequestUri))
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

    private static string[] DeclaredJsonNamesOf(Type type) => JsonNamesOf(type);

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(RepoRootDir, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                               && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string RepoRootDir
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
