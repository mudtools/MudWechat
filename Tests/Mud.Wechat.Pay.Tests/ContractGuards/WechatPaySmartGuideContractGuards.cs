// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.SmartGuide;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「SmartGuide」契约守卫 —— 本批 <b>4 端点</b>（注册组 <c>SmartGuide</c>，接口 <c>IWechatPaySmartGuideService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.SmartGuide</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中<b>0 支官方只印一页</b>、<b>4 支印多页</b>（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 4 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>涉密面是逐页核对结论</b>：本接口 4 支里<b>3 支</b>的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>（敏感字段密文上送），其余支<b>未列</b>该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>四支各印商户页 + 服务商页 ⇒ 并集建模</b>：服务商页统一多一个<b>选填</b> <c>sub_mchid</c>（<c>string(64)</c>）， 其余字段与商户页同形 ⇒ 一支一方法，身份差异写在参数注释内，不拆方法也不复制 DTO。</para>
/// <para><b>注册与查询共用同一 URL、靠动词区分</b>：官方注意原文「服务人员注册API和查询API请求URL相同，区别主要是在body和query， 请注意区分」（<c>GET /v3/smartguide/guides</c> 查询、<c>POST /v3/smartguide/guides</c> 注册）⇒ 本域两支路由字符串相同， 守卫按「动词 + 路由」计数，<b>不得</b>合并成一支或改动路由写法。</para>
/// <para><b>涉密面是逐页结论</b>：查询、注册、更新三支的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b> （<c>mobile</c> / <c>qr_code</c> / <c>avatar</c> 等按 APIv3 加密上送），而<b>分配支未列</b>该头 ⇒ 本接口内同时存在「须带序列号」与「不带」两类，密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>，声明式签名不承载该头。</para>
/// <para><b>两支是 204 无应答包体</b>：<c>PATCH .../guides/{guide_id}</c>（更新）与 <c>POST .../guides/{guide_id}/assign</c>（分配） ⇒ 返回裸 <see cref="Task"/>；查询与注册各建应答（列表包装 <c>data[]</c> + <c>total_count</c> / <c>limit</c> / <c>offset</c>， 注册应答单键 <c>guide_id</c>）。</para>
/// <para><b>两套身份键互斥、单次查询上限 10 条</b>：官方错误码原文「个人微信商家不应填写企业微信ID」「企业微信商家不应填写工号」 ⇒ <c>userid</c>（企微员工 ID）与 <c>work_id</c>（工号）属两种商家形态，SDK 两者皆可空承载、由宿主择一； <c>store_id</c> 是<b>必填 query</b>，且「最大资源条数大于10」即 <c>limit</c> 官方上限 10。</para>
/// <para><b>分配须在支付完成之前</b>：官方原文「调用服务人员分配接口需在完成支付之前，若分配服务人员晚于支付完成， 则将<b>无法在支付凭证上出现服务人员名片入口</b>」⇒ <c>out_trade_no</c> 指向的订单必须尚未支付； 同域码「服务人员已注册」属注册支、「所有可更新的参数都没有填」「个人微信商家不能改二维码」属更新支，<b>不得</b>互挂。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>2 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPaySmartGuideContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.SmartGuide";

    private static readonly Type[] Services = { typeof(IWechatPaySmartGuideService) };

    /// <summary>SMG-B1：拓扑 —— 注册组 SmartGuide、本批 4 方法（接口实测 4）、4 条 (动词, 路由)；动词集 Get / Patch / Post。</summary>
    [Fact]
    public void SmartGuideTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPaySmartGuideService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("SmartGuide", "组名即源生成注册入口名（改组名即改 AddSmartGuideWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(4, "SmartGuide 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(4, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PatchAttribute", "PostAttribute" }, "本域查询 GET、注册 POST、更新 PATCH、分配 POST 四支并存，且 GET 与 POST 共用同一 URL（官方原文「请求URL相同，区别在body和query」）⇒ 动词是唯一的分支判据");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/smartguide/guides 前缀内，更新与分配另带 /{guideId} 段");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayProfitSharingService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayProfitSharingService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>SMG-B2：本批 4 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void SmartGuideEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 服务人员查询｜官方页 v3/merchant/4012535123 + v3/partner/4012688558｜身份 【普通商户】、【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPaySmartGuideService.QuerySmartGuidesAsync)).Should().Be("/v3/smartguide/guides");
        PathKeysOf(nameof(IWechatPaySmartGuideService.QuerySmartGuidesAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPaySmartGuideService.QuerySmartGuidesAsync)).Should().BeEquivalentTo(new[] { "store_id", "userid", "mobile", "work_id", "limit", "offset", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPaySmartGuideService.QuerySmartGuidesAsync)).Should().BeEquivalentTo(new[] { "store_id" }, "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPaySmartGuideService.QuerySmartGuidesAsync)).Should().BeNull("服务人员查询官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPaySmartGuideService.QuerySmartGuidesAsync)).Should().Be(typeof(SmartGuideListResponse));

        // 服务人员注册｜官方页 v3/merchant/4012535138 + v3/partner/4012688564｜身份 【普通商户】、【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPaySmartGuideService.RegisterSmartGuideAsync)).Should().Be("/v3/smartguide/guides");
        PathKeysOf(nameof(IWechatPaySmartGuideService.RegisterSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPaySmartGuideService.RegisterSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPaySmartGuideService.RegisterSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPaySmartGuideService.RegisterSmartGuideAsync)).Should().Be(typeof(SmartGuideRegisterRequest));
        ReturnTypeOf(nameof(IWechatPaySmartGuideService.RegisterSmartGuideAsync)).Should().Be(typeof(SmartGuideRegisterResponse));

        // 服务人员更新｜官方页 v3/merchant/4012535160 + v3/partner/4012688570｜身份 【普通商户】、【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPaySmartGuideService.UpdateSmartGuideAsync)).Should().Be("/v3/smartguide/guides/{guideId}");
        PathKeysOf(nameof(IWechatPaySmartGuideService.UpdateSmartGuideAsync)).Should().BeEquivalentTo(new[] { "guideId" });
        QueryNamesOf(nameof(IWechatPaySmartGuideService.UpdateSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPaySmartGuideService.UpdateSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPaySmartGuideService.UpdateSmartGuideAsync)).Should().Be(typeof(SmartGuideUpdateRequest));
        ReturnTypeOf(nameof(IWechatPaySmartGuideService.UpdateSmartGuideAsync)).Should().Be(typeof(Task), "服务人员更新官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 服务人员分配｜官方页 v3/merchant/4012535161 + v3/partner/4012474145｜身份 【普通商户】、【普通服务商】
        RouteOf(nameof(IWechatPaySmartGuideService.AssignSmartGuideAsync)).Should().Be("/v3/smartguide/guides/{guideId}/assign");
        PathKeysOf(nameof(IWechatPaySmartGuideService.AssignSmartGuideAsync)).Should().BeEquivalentTo(new[] { "guideId" });
        QueryNamesOf(nameof(IWechatPaySmartGuideService.AssignSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPaySmartGuideService.AssignSmartGuideAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPaySmartGuideService.AssignSmartGuideAsync)).Should().Be(typeof(SmartGuideAssignRequest));
        ReturnTypeOf(nameof(IWechatPaySmartGuideService.AssignSmartGuideAsync)).Should().Be(typeof(Task), "服务人员分配官方成功应答是 204 无包体 ⇒ 裸 Task");
    }

    /// <summary>SMG-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void SmartGuideDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<SmartGuideRegisterRequest>().Should().BeEquivalentTo(NAMES["SmartGuideRegisterRequest"], "SmartGuideRegisterRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<SmartGuideUpdateRequest>().Should().BeEquivalentTo(NAMES["SmartGuideUpdateRequest"], "SmartGuideUpdateRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<SmartGuideAssignRequest>().Should().BeEquivalentTo(NAMES["SmartGuideAssignRequest"], "SmartGuideAssignRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<SmartGuideListResponse>().Should().BeEquivalentTo(NAMES["SmartGuideListResponse"], "SmartGuideListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<SmartGuideRegisterResponse>().Should().BeEquivalentTo(NAMES["SmartGuideRegisterResponse"], "SmartGuideRegisterResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>SMG-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void SmartGuideDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(SmartGuideListResponse)).Should().BeTrue("SmartGuideListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(SmartGuideRegisterResponse)).Should().BeTrue("SmartGuideRegisterResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(SmartGuideRegisterRequest)).Should().BeFalse("SmartGuideRegisterRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(SmartGuideUpdateRequest)).Should().BeFalse("SmartGuideUpdateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(SmartGuideAssignRequest)).Should().BeFalse("SmartGuideAssignRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>SMG-B5：本批 DTO 全量形态 —— 6 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void SmartGuideAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(6, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            SmartGuideJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 SmartGuideJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("SmartGuide", type.Name + " 的 SerializerClassName 必须为 SmartGuide");
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

    /// <summary>SMG-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void SmartGuideRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("SmartGuide", "PayModule 须有 SmartGuide 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddSmartGuideWebApiHttpClient()", "注册委托必须指向源生成方法（组名 SmartGuide 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("SmartGuideJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["SmartGuideRegisterRequest"] = new[] { "corpid", "store_id", "userid", "name", "mobile", "qr_code", "avatar", "group_qrcode", "sub_mchid" },
        ["SmartGuideUpdateRequest"] = new[] { "name", "mobile", "qr_code", "avatar", "group_qrcode", "sub_mchid" },
        ["SmartGuideAssignRequest"] = new[] { "out_trade_no", "sub_mchid" },
        ["SmartGuideListResponse"] = new[] { "data", "total_count", "limit", "offset" },
        ["SmartGuideRegisterResponse"] = new[] { "guide_id" },
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
