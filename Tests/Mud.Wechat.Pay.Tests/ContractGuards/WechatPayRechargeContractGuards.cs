// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Recharge;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「Recharge」契约守卫 —— 本批 <b>2 端点</b>（注册组 <c>Recharge</c>，接口 <c>IWechatPayRechargeService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Recharge</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 2 支中<b>2 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 2 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>两支官方全部只印商户页</b>（「支持商户：【普通商户】」）⇒ 无服务商镜像、无双页归并问题。</para>
/// <para><b>申领支是充值流程的前置闸</b>：官方原文「商户系统<b>须通过调用此接口获取银行转账账号，随后发起充值流程</b>」⇒ <c>POST .../apply</c> 的应答 <c>transfer_in_account</c>（<c>bank_name</c> / <c>bank_address_code</c> / <c>bank_code</c> / <c>bank_account_name</c> / <c>bank_account_no</c>）是后续<b>线下银行转账</b>的收款账号，HTTP 成功不等于到账。</para>
/// <para><b>两支应答不同形 ⇒ 各建一型，不得按「字段重合」合并</b>：申领支应答 6 键（<c>recharge_id</c> + <c>out_recharge_no</c> + <c>transfer_in_account</c> 子树），查询支应答 17 键（另含 <c>mchid</c>、<c>account_type</c>、 <c>recharge_state</c>、<c>bank_transfer_info</c> 子树与 <c>accept_time</c> / <c>success_time</c> / <c>close_time</c>）⇒ 两个响应类型独立建模。</para>
/// <para><b>定位键只有商户单号</b>：查询支把 <c>out_recharge_no</c> 放在 <b>path</b> 段、<b>无</b> query； 官方分配的 <c>recharge_id</c> 长度是罕见的 <c>string(27)</c> ⇒ 照页，不用微信单号作查询键。</para>
/// <para><b>退回是异步态</b>：查询应答的 <c>bank_transfer_info</c> 里 <c>return_time</c> / <c>return_reason</c> / <c>bank_card_tail</c> 均为选填，配合 <c>recharge_state</c> 与 <c>recharge_state_desc</c> 表达「已退回」等中间态 ⇒ <b>不得</b>据 <c>success_time</c> 缺失判定失败，也不得自造 <c>state</c> 枚举闭集。</para>
/// <para><b>频率限制</b>：两支官方页<b>未印</b>端点级数字，错误码给 <c>429 FREQUENCY_LIMITED</c> 与 <c>429 FREQUENCY_LIMIT_EXCEED</c> <b>两种</b>拼写（申领支前者、查询支后者，照页各归各支）⇒ 不得「统一」成一条。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayRechargeContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Recharge";

    private static readonly Type[] Services = { typeof(IWechatPayRechargeService) };

    /// <summary>RCH-B1：拓扑 —— 注册组 Recharge、本批 2 方法（接口实测 2）、2 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void RechargeTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayRechargeService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("Recharge", "组名即源生成注册入口名（改组名即改 AddRechargeWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(2, "Recharge 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(2, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域申领 POST、查询 GET 两支 ⇒ 查询走商户单号 path 段，不得补 PUT/DELETE（官方无关闭与撤销支）");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/recharge/bank-transfer-recharges/ 前缀内，与平台收付面充值的 /v3/platsolution/ecommerce/recharges/ 零交集（两支同名 out-recharge-no path 段但前缀不同）");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayTransferService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayTransferService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>RCH-B2：本批 2 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void RechargeEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 申请银行转账充值｜官方页 v3/merchant/4012585425｜身份 【普通商户】
        RouteOf(nameof(IWechatPayRechargeService.ApplyRechargeBankTransferAsync)).Should().Be("/v3/recharge/bank-transfer-recharges/apply");
        PathKeysOf(nameof(IWechatPayRechargeService.ApplyRechargeBankTransferAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayRechargeService.ApplyRechargeBankTransferAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayRechargeService.ApplyRechargeBankTransferAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayRechargeService.ApplyRechargeBankTransferAsync)).Should().Be(typeof(RechargeBankTransferApplyRequest));
        ReturnTypeOf(nameof(IWechatPayRechargeService.ApplyRechargeBankTransferAsync)).Should().Be(typeof(RechargeBankTransferApplyResponse));

        // 查询银行转账充值结果｜官方页 v3/merchant/4012585430｜身份 【普通商户】
        RouteOf(nameof(IWechatPayRechargeService.QueryRechargeBankTransferAsync)).Should().Be("/v3/recharge/bank-transfer-recharges/out-recharge-no/{outRechargeNo}");
        PathKeysOf(nameof(IWechatPayRechargeService.QueryRechargeBankTransferAsync)).Should().BeEquivalentTo(new[] { "outRechargeNo" });
        QueryNamesOf(nameof(IWechatPayRechargeService.QueryRechargeBankTransferAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayRechargeService.QueryRechargeBankTransferAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayRechargeService.QueryRechargeBankTransferAsync)).Should().BeNull("查询银行转账充值结果官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayRechargeService.QueryRechargeBankTransferAsync)).Should().Be(typeof(RechargeBankTransferQueryResponse));
    }

    /// <summary>RCH-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void RechargeDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<RechargeBankTransferApplyRequest>().Should().BeEquivalentTo(NAMES["RechargeBankTransferApplyRequest"], "RechargeBankTransferApplyRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<RechargeBankTransferApplyResponse>().Should().BeEquivalentTo(NAMES["RechargeBankTransferApplyResponse"], "RechargeBankTransferApplyResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<RechargeBankTransferQueryResponse>().Should().BeEquivalentTo(NAMES["RechargeBankTransferQueryResponse"], "RechargeBankTransferQueryResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>RCH-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void RechargeDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(RechargeBankTransferApplyResponse)).Should().BeTrue("RechargeBankTransferApplyResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(RechargeBankTransferQueryResponse)).Should().BeTrue("RechargeBankTransferQueryResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(RechargeBankTransferApplyRequest)).Should().BeFalse("RechargeBankTransferApplyRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>RCH-B5：本批 DTO 全量形态 —— 7 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void RechargeAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(7, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            RechargeJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 RechargeJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("Recharge", type.Name + " 的 SerializerClassName 必须为 Recharge");
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

    /// <summary>RCH-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void RechargeRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("Recharge", "PayModule 须有 Recharge 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddRechargeWebApiHttpClient()", "注册委托必须指向源生成方法（组名 Recharge 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("RechargeJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["RechargeBankTransferApplyRequest"] = new[] { "out_recharge_no", "recharge_scene", "account_type", "recharge_amount" },
        ["RechargeBankTransferApplyResponse"] = new[] { "recharge_id", "out_recharge_no", "transfer_in_account" },
        ["RechargeBankTransferQueryResponse"] = new[] { "mchid", "recharge_id", "out_recharge_no", "account_type", "recharge_state", "recharge_scene", "recharge_state_desc", "recharge_amount", "bank_transfer_info", "accept_time", "success_time", "close_time" },
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
