// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.TransactionBlock;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「TransactionBlock」契约守卫 —— 本批 <b>4 端点</b>（注册组 <c>TransactionBlock</c>，接口 <c>IWechatPayTransactionBlockService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.TransactionBlock</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>4 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 4 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>四支官方全部只印服务商页</b>（「支持商户：【普通服务商】」）⇒ 无商户面镜像；且业务码原文 「此接口仅对部分合作伙伴开放，当前商户号未开放」⇒ 本域是<b>白名单开放面</b>，未开通即 400，属预期而非 SDK 缺陷。</para>
/// <para><b>定位键四支四形，照页不得统一</b>：拦截记录单查把 <c>block_record_id</c> 放 <b>path</b>、<c>sub_mchid</c> 放<b>必填 query</b>； 列表把 <c>sub_mchid</c> 放 <b>path</b>、<c>limit</c> <b>必填</b>而 <c>offset</c> 选填；申诉单查以 <c>block_submission_id</c> 为 path 且<b>无 query</b>；申诉提交的 <c>sub_mchid</c> 在 path、请求体内<b>不含</b>该键。</para>
/// <para><b>申诉两支应答逐字段同形 ⇒ 共用一型</b>：按 <c>block_submission_id</c> 查询与提交申诉的官方应答表均为 <c>block_submission_id</c> / <c>submit_time</c> / <c>review_result</c> / <c>review_time</c> / <c>review_reject_reason</c> 五行同形 ⇒ 共用同一响应类型；拦截记录两支各建（单查 6 键、列表 <c>data[]</c> 元素 + <c>offset</c> / <c>limit</c> / <c>total_count</c> 包装）， 元素与单查字段虽同形，但本线口径是<b>共用只发生在应答根类型</b>。</para>
/// <para><b>两个巨型字段以 <c>string</c> 承载，官方上限 4000000 字符</b>：应答侧 <c>requested_item_info</c>、 请求侧 <c>submit_data</c> 官方类型均写 <c>string(4000000)</c>（约 4MB）⇒ SDK 不解析其内部结构、不做长度校验， 但宿主须知请求与应答的实际体积上限（勿整段进日志）。</para>
/// <para><b>同商户的申诉单是「一商户一在审」</b>：提交支业务码原文「该商户下已经存在<b>审核中</b>的申诉单，不允许提交申诉」⇒ 提交前应先查记录/申诉状态；<c>merchant_submit_id</c>（商家申诉单号，<c>string(128)</c> 必填）是商户侧幂等键， SDK 不做本地去重。</para>
/// <para><b>频率限制</b>：四支官方页<b>全部未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayTransactionBlockContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.TransactionBlock";

    private static readonly Type[] Services = { typeof(IWechatPayTransactionBlockService) };

    /// <summary>TRB-B1：拓扑 —— 注册组 TransactionBlock、本批 4 方法（接口实测 4）、4 条 (动词, 路由)；动词集 Get / Post。</summary>
    [Fact]
    public void TransactionBlockTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayTransactionBlockService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("TransactionBlock", "组名即源生成注册入口名（改组名即改 AddTransactionBlockWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(4, "TransactionBlock 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(4, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" }, "本域三支查询全 GET、申诉提交一支 POST ⇒ 申诉不是 PUT 到单查路由，而是 POST 到 sub-mchid/{subMchid} 集合段");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/transaction-block/ 前缀内，记录族是 transaction-block-records、申诉族是 transaction-block-submissions");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayBankTransferService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayBankTransferService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>TRB-B2：本批 4 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void TransactionBlockEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 查询子商户交易拦截记录详情｜官方页 v3/partner/4014940435｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordAsync)).Should().Be("/v3/transaction-block/transaction-block-records/{blockRecordId}");
        PathKeysOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordAsync)).Should().BeEquivalentTo(new[] { "blockRecordId" });
        QueryNamesOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordAsync)).Should().BeEquivalentTo(new[] { "sub_mchid" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordAsync)).Should().BeNull("查询子商户交易拦截记录详情官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordAsync)).Should().Be(typeof(TransactionBlockRecordInfoResponse));

        // 分页查询子商户名下的交易拦截记录｜官方页 v3/partner/4014940432｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordsAsync)).Should().Be("/v3/transaction-block/transaction-block-records/sub-mchid/{subMchid}");
        PathKeysOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordsAsync)).Should().BeEquivalentTo(new[] { "subMchid" });
        QueryNamesOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordsAsync)).Should().BeEquivalentTo(new[] { "limit", "offset" });
        RequiredQueryOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordsAsync)).Should().BeEquivalentTo(new[] { "limit" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordsAsync)).Should().BeNull("分页查询子商户名下的交易拦截记录官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockRecordsAsync)).Should().Be(typeof(TransactionBlockRecordListResponse));

        // 查询交易拦截申诉详情｜官方页 v3/partner/4014940449｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockSubmissionAsync)).Should().Be("/v3/transaction-block/transaction-block-submissions/{blockSubmissionId}");
        PathKeysOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockSubmissionAsync)).Should().BeEquivalentTo(new[] { "blockSubmissionId" });
        QueryNamesOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockSubmissionAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockSubmissionAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockSubmissionAsync)).Should().BeNull("查询交易拦截申诉详情官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayTransactionBlockService.QueryTransactionBlockSubmissionAsync)).Should().Be(typeof(TransactionBlockSubmissionInfoResponse));

        // 发起交易拦截申诉｜官方页 v3/partner/4014940446｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayTransactionBlockService.CreateTransactionBlockSubmissionAsync)).Should().Be("/v3/transaction-block/transaction-block-submissions/sub-mchid/{subMchid}");
        PathKeysOf(nameof(IWechatPayTransactionBlockService.CreateTransactionBlockSubmissionAsync)).Should().BeEquivalentTo(new[] { "subMchid" });
        QueryNamesOf(nameof(IWechatPayTransactionBlockService.CreateTransactionBlockSubmissionAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayTransactionBlockService.CreateTransactionBlockSubmissionAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayTransactionBlockService.CreateTransactionBlockSubmissionAsync)).Should().Be(typeof(TransactionBlockSubmissionCreateRequest));
        ReturnTypeOf(nameof(IWechatPayTransactionBlockService.CreateTransactionBlockSubmissionAsync)).Should().Be(typeof(TransactionBlockSubmissionInfoResponse));
    }

    /// <summary>TRB-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void TransactionBlockDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<TransactionBlockSubmissionCreateRequest>().Should().BeEquivalentTo(NAMES["TransactionBlockSubmissionCreateRequest"], "TransactionBlockSubmissionCreateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<TransactionBlockRecordInfoResponse>().Should().BeEquivalentTo(NAMES["TransactionBlockRecordInfoResponse"], "TransactionBlockRecordInfoResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<TransactionBlockRecordListResponse>().Should().BeEquivalentTo(NAMES["TransactionBlockRecordListResponse"], "TransactionBlockRecordListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<TransactionBlockSubmissionInfoResponse>().Should().BeEquivalentTo(NAMES["TransactionBlockSubmissionInfoResponse"], "TransactionBlockSubmissionInfoResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>TRB-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void TransactionBlockDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TransactionBlockRecordInfoResponse)).Should().BeTrue("TransactionBlockRecordInfoResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TransactionBlockRecordListResponse)).Should().BeTrue("TransactionBlockRecordListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(TransactionBlockSubmissionInfoResponse)).Should().BeTrue("TransactionBlockSubmissionInfoResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(TransactionBlockSubmissionCreateRequest)).Should().BeFalse("TransactionBlockSubmissionCreateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>TRB-B5：本批 DTO 全量形态 —— 5 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void TransactionBlockAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(5, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            TransactionBlockJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 TransactionBlockJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("TransactionBlock", type.Name + " 的 SerializerClassName 必须为 TransactionBlock");
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

    /// <summary>TRB-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void TransactionBlockRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("TransactionBlock", "PayModule 须有 TransactionBlock 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddTransactionBlockWebApiHttpClient()", "注册委托必须指向源生成方法（组名 TransactionBlock 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("TransactionBlockJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["TransactionBlockSubmissionCreateRequest"] = new[] { "submit_data", "merchant_submit_id" },
        ["TransactionBlockRecordInfoResponse"] = new[] { "block_record_id", "block_time", "block_reason", "number_of_block", "recover_way", "requested_item_info" },
        ["TransactionBlockRecordListResponse"] = new[] { "data", "offset", "limit", "total_count" },
        ["TransactionBlockSubmissionInfoResponse"] = new[] { "block_submission_id", "submit_time", "review_result", "review_time", "review_reject_reason" },
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
