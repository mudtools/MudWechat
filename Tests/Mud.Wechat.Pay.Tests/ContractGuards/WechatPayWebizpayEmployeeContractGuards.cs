// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.WebizpayEmployee;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2-D 微企付企业支付「额度卡与员工授权」域契约守卫 —— <b>6 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>六支建五型</b>：按 <c>card_no</c> 与按 <c>out_card_no</c> 两支查询的官方应答表<b>逐字段同形且必填口径一致</b>（20 行） ⇒ 共用 <see cref="WebizpayQuotaCardQueryResponse"/>；下发 / 作废 / 支付链接 / 解除授权各建一型。</para>
/// <para><b>定位键分工（本域最易踩）</b>：path 一律 <c>employee_id</c>（官方【微信授权关系 ID】，形如 <c>WeBizPay_a968402a-…</c>，由注册回调或员工查询面产出，<b>不是</b>商户企业员工 ID <c>user_id</c>）； 卡级操作只认<b>微信卡号 <c>card_no</c></b> ⇒ 作废与支付链接两支<b>不接受商户卡号</b>， 只持有 <c>out_card_no</c> 的调用方须先走 <c>out-quota-cards</c> 查询支换取 <c>card_no</c>。</para>
/// <para><b>同名 <c>sp_mchid</c> / <c>sub_mchid</c> 按动词落位不同</b>：GET 三支（两路查询 + 支付链接）落<b>必填 query</b>、零请求体， POST 三支落 <b>Body</b> ⇒ 请求 DTO 与查询签名不得互套；支付链接支另有必填 query <c>application_type</c> （<c>H5</c> / <c>MINIPROGRAM</c>）。</para>
/// <para><b>解除授权支请求用 <c>employee_id</c>、应答回显 <c>user_id</c></b>（商户企业员工 ID），且官方口径为<b>同步</b>： 应答 <c>authorization_state</c> 返回 <c>REVOKED</c> 即认定该员工在本服务商-出资子商户下授权已解除、 其在本商户下的企业支付额度卡<b>全部作废</b> ⇒ 不得把该支当异步受理。</para>
/// <para><b>查询应答 20 行的 6 个选填字段</b>（<c>reason</c> / <c>card_usage_time</c> / <c>transaction_id</c> / <c>card_disabled_time</c> / <c>card_remark</c> / <c>avail_mch_range_id</c>）⇒ 判「卡可用」须同时看 <c>card_state</c>（仅 <c>ACTIVE</c> / <c>DISABLED</c> 两值）与 <c>effective_end_time</c>，<b>不得</b>只看 <c>remain_amount</c> &gt; 0。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayWebizpayEmployeeContractGuards
{
    private const string RegistryGroupName = "WebizpayEmployee";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.WebizpayEmployee";

    private static readonly Type Service = typeof(IWechatPayWebizpayEmployeeService);

    /// <summary>WEB-EP-B1：拓扑 —— 一个注册组一个接口、6 方法、6 条 (动词, 路由)、动词集 Post / Get；路由前缀 /v3/webizpay/employees。</summary>
    [Fact]
    public void WebizpayEmployeeTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(6, "额度卡与员工授权声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(6, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/webizpay/employees", StringComparison.Ordinal),
            "本域路由全在 /v3/webizpay/employees 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>WEB-EP-B2：6 支端点的路由、path 键、query 名册与请求体有无（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void WebizpayEmployeeEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 企业商户为员工下发企业支付额度卡｜官方页 partner/4014507148｜身份 【普通服务商】｜官方页更新 2025.05.09
        RouteOf(nameof(IWechatPayWebizpayEmployeeService.ApplyQuotaCardAsync)).Should().Be("/v3/webizpay/employees/{employeeId}/quota-cards");
        PathKeysOf(nameof(IWechatPayWebizpayEmployeeService.ApplyQuotaCardAsync)).Should().BeEquivalentTo(new[] { "employeeId" });
        QueryNamesOf(nameof(IWechatPayWebizpayEmployeeService.ApplyQuotaCardAsync)).Should().BeEmpty();

        // 企业商户查询企业支付额度卡｜官方页 partner/4014507119｜身份 【普通服务商】｜官方页更新 2025.05.09
        RouteOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByCardNoAsync)).Should().Be("/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}");
        PathKeysOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByCardNoAsync)).Should().BeEquivalentTo(new[] { "employeeId", "cardNo" });
        QueryNamesOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByCardNoAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByCardNoAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" }, "企业商户查询企业支付额度卡的 sp_mchid / sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByCardNoAsync)).Should().BeNull("企业商户查询企业支付额度卡官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 企业商户使用商户卡号查询企业支付额度卡｜官方页 partner/4015001625｜身份 【普通服务商】｜官方页更新 2025.05.29
        RouteOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByOutCardNoAsync)).Should().Be("/v3/webizpay/employees/{employeeId}/out-quota-cards/{outCardNo}");
        PathKeysOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByOutCardNoAsync)).Should().BeEquivalentTo(new[] { "employeeId", "outCardNo" });
        QueryNamesOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByOutCardNoAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByOutCardNoAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" }, "企业商户使用商户卡号查询企业支付额度卡的 sp_mchid / sub_mchid 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayWebizpayEmployeeService.QueryQuotaCardByOutCardNoAsync)).Should().BeNull("企业商户使用商户卡号查询企业支付额度卡官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 企业商户作废员工企业支付额度卡｜官方页 partner/4014507127｜身份 【普通服务商】｜官方页更新 2025.05.09
        RouteOf(nameof(IWechatPayWebizpayEmployeeService.CancelQuotaCardAsync)).Should().Be("/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/cancel");
        PathKeysOf(nameof(IWechatPayWebizpayEmployeeService.CancelQuotaCardAsync)).Should().BeEquivalentTo(new[] { "employeeId", "cardNo" });
        QueryNamesOf(nameof(IWechatPayWebizpayEmployeeService.CancelQuotaCardAsync)).Should().BeEmpty();

        // 企业商户获取指定额度卡支付链接｜官方页 partner/4014507160｜身份 【普通服务商】｜官方页更新 2025.05.29
        RouteOf(nameof(IWechatPayWebizpayEmployeeService.GetQuotaCardPaymentUrlAsync)).Should().Be("/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/payment-url");
        PathKeysOf(nameof(IWechatPayWebizpayEmployeeService.GetQuotaCardPaymentUrlAsync)).Should().BeEquivalentTo(new[] { "employeeId", "cardNo" });
        QueryNamesOf(nameof(IWechatPayWebizpayEmployeeService.GetQuotaCardPaymentUrlAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "application_type" });
        RequiredQueryOf(nameof(IWechatPayWebizpayEmployeeService.GetQuotaCardPaymentUrlAsync)).Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "application_type" }, "企业商户获取指定额度卡支付链接的 sp_mchid / sub_mchid / application_type 官方必填：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayWebizpayEmployeeService.GetQuotaCardPaymentUrlAsync)).Should().BeNull("企业商户获取指定额度卡支付链接官方无请求体 ⇒ 不得凭空造请求 DTO");

        // 企业商户发起解除员工企业支付授权｜官方页 partner/4014507156｜身份 【普通服务商】｜官方页更新 2025.05.16
        RouteOf(nameof(IWechatPayWebizpayEmployeeService.RevokeEmployeeAsync)).Should().Be("/v3/webizpay/employees/{employeeId}/revoke");
        PathKeysOf(nameof(IWechatPayWebizpayEmployeeService.RevokeEmployeeAsync)).Should().BeEquivalentTo(new[] { "employeeId" });
        QueryNamesOf(nameof(IWechatPayWebizpayEmployeeService.RevokeEmployeeAsync)).Should().BeEmpty();
    }

    /// <summary>WEB-EP-B3：官方字段表名册（请求体与应答顶层，逐字照页），共用应答类型只建一型。</summary>
    [Fact]
    public void WebizpayEmployeeDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<WebizpayQuotaCardApplyRequest>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "card_template_id", "amount", "card_name", "out_card_no", "effective_end_time", "card_remark", "avail_mch_range_id" },
            "企业商户为员工下发企业支付额度卡请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayQuotaCardCancelRequest>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "reason" },
            "企业商户作废员工企业支付额度卡请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayEmployeeRevokeRequest>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" },
            "企业商户发起解除员工企业支付授权请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayQuotaCardApplyResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "out_card_no", "employee_id", "card_no", "effective_begin_time", "effective_end_time" },
            "企业商户为员工下发企业支付额度卡应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayQuotaCardQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "card_no", "out_card_no", "employee_id", "card_template_id", "card_rule", "refund_rule", "amount", "remain_amount", "effective_end_time", "effective_begin_time", "card_state", "card_name", "reason", "card_usage_time", "transaction_id", "card_disabled_time", "card_remark", "avail_mch_range_id" },
            "企业商户查询企业支付额度卡应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayQuotaCardQueryResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "card_no", "out_card_no", "employee_id", "card_template_id", "card_rule", "refund_rule", "amount", "remain_amount", "effective_end_time", "effective_begin_time", "card_state", "card_name", "reason", "card_usage_time", "transaction_id", "card_disabled_time", "card_remark", "avail_mch_range_id" },
            "企业商户使用商户卡号查询企业支付额度卡应答顶层字段名册（官方应答表逐字，与 <c>employees/{employee_id}/quota-cards/{card_no}</c> 同表 ⇒ 共用一个类型）");

        DeclaredJsonNamesOf<WebizpayQuotaCardCancelResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "employee_id", "card_no", "card_state", "card_disabled_time" },
            "企业商户作废员工企业支付额度卡应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayQuotaCardPaymentUrlResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "employee_id", "card_no", "expire_time", "payment_url", "application_type", "mp_query", "mp_business_type" },
            "企业商户获取指定额度卡支付链接应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<WebizpayEmployeeRevokeResponse>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid", "user_id", "authorization_state", "authorization_revoked_time", "reason" },
            "企业商户发起解除员工企业支付授权应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>WEB-EP-B4：判错面 + AOT 上下文登记 + DI 与模块接线（3 请求根型 / 5 应答根型 / 8 个 DTO）。</summary>
    [Fact]
    public void WebizpayEmployeeDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(8, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            WebizpayEmployeeJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 WebizpayEmployeeJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 WebizpayEmployee");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayQuotaCardApplyResponse)).Should().BeTrue("WebizpayQuotaCardApplyResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayQuotaCardQueryResponse)).Should().BeTrue("WebizpayQuotaCardQueryResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayQuotaCardCancelResponse)).Should().BeTrue("WebizpayQuotaCardCancelResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayQuotaCardPaymentUrlResponse)).Should().BeTrue("WebizpayQuotaCardPaymentUrlResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayEmployeeRevokeResponse)).Should().BeTrue("WebizpayEmployeeRevokeResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayQuotaCardApplyRequest)).Should().BeFalse("WebizpayQuotaCardApplyRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayQuotaCardCancelRequest)).Should().BeFalse("WebizpayQuotaCardCancelRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(WebizpayEmployeeRevokeRequest)).Should().BeFalse("WebizpayEmployeeRevokeRequest 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("WebizpayEmployeeJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 WebizpayEmployee 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddWebizpayEmployeeWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 WebizpayEmployee 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayWebizpayEmployeeService.{methodName} 必须存在");

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
