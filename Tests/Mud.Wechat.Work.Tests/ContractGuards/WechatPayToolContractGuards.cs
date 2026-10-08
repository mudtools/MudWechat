// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.PayTool;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 收银台域（PayTool 模块）契约守卫：路由表、三族两令牌形态、接口层级、签名面与
/// JSON 上下文全量登记锁定（收款工具 4 端点 + 发票管理 2 端点 + 应用版本付费 3 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>开放面</b>：官方在第三方应用开发与服务商代开发两棵文档树的「收银台」分组下提供本域端点、
/// 共享同一端点页（第三方树 98045/98046/98053/98054/99436/99437 = 代开发树 99358/99359/99360/99361/99447/99448），
/// 企业自建应用开发文档树<b>无对应 API</b>，故全域不设自建子接口。
/// </para>
/// <para>
/// <b>三族两令牌（本域最关键的结构约束）</b>：9 个端点消费两种互不兼容的凭据，故按令牌路由键拆为三族：
/// ① 收款工具族（<see cref="IWechatWorkThirdPartyPayToolOrderService"/>）4 端点与
/// ② 发票管理族（<see cref="IWechatWorkThirdPartyPayToolInvoiceService"/>）2 端点走服务商
/// <c>provider_access_token</c>，二者均为「零端点父接口 + 唯一第三方子接口承载」；
/// ③ 应用版本付费族（<see cref="IWechatWorkThirdPartyPayToolVersionSuiteService"/>）3 端点走套件
/// <c>suite_access_token</c>，同为「零端点父接口 + 唯一第三方子接口承载」。
/// </para>
/// <para>
/// <b>官方契约陷阱（勿「顺手修正」）</b>：
/// ① <b>签名面按族分叉</b>——收款工具族 4 端点必须携带 nonce_str / ts / sig（HMAC-SHA256 + Base64，见
/// <see cref="WechatPayToolSignature"/>），发票管理族与应用版本付费族官方参数表<b>不含</b>签名三要素；
/// ② 应用版本付费的「获取企业永久授权码（91911）/ 获取企业授权信息（91912）」与授权流接口族
/// <b>同一端点</b>，已在 <see cref="IWechatWorkProviderAuthenticationService"/> 承载，本域<b>不重复声明</b>；
/// ③ 收款订单详情官方同时列出 <c>pay_order.pay_from</c> 与 <c>pay_order.pay_type</c>，而返回示例只用
/// <c>pay_type</c>，本 SDK 照示例承载 <c>pay_type</c>；
/// ④ 应用版本付费订单字段官方为全小写 <c>orderid</c> / <c>suiteid</c>（非 <c>order_id</c> / <c>suite_id</c>）；
/// ⑤ 官方代开发应用下单示例首个 <c>total_price</c> 后保留了逗号（JSON 语法无效），调用须删除。
/// </para>
/// </remarks>
public class WechatPayToolContractGuards
{
    private const string PayToolRegistryGroupName = "PayTool";

    private const string OrderParentImplementationClassName = "WechatWorkPayToolOrderService";

    private const string InvoiceParentImplementationClassName = "WechatWorkPayToolInvoiceService";

    private const string VersionSuiteParentImplementationClassName = "WechatWorkPayToolVersionSuiteService";

    /// <summary>收银台域官方路由表（9 端点分属三族：收款工具 4 条 + 发票管理 2 条走 provider 令牌，应用版本付费 3 条走 suite 令牌）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 收款工具族（官方 98045/98046/98053/98054，路由挂 /cgi-bin/paytool/ 段，官方全部即 POST）。
        (typeof(IWechatWorkThirdPartyPayToolOrderService),
            nameof(IWechatWorkThirdPartyPayToolOrderService.CreateOrderAsync),
            typeof(PostAttribute), "/cgi-bin/paytool/open_order"),
        (typeof(IWechatWorkThirdPartyPayToolOrderService),
            nameof(IWechatWorkThirdPartyPayToolOrderService.CloseOrderAsync),
            typeof(PostAttribute), "/cgi-bin/paytool/close_order"),
        (typeof(IWechatWorkThirdPartyPayToolOrderService),
            nameof(IWechatWorkThirdPartyPayToolOrderService.GetOrderListAsync),
            typeof(PostAttribute), "/cgi-bin/paytool/get_order_list"),
        (typeof(IWechatWorkThirdPartyPayToolOrderService),
            nameof(IWechatWorkThirdPartyPayToolOrderService.GetOrderDetailAsync),
            typeof(PostAttribute), "/cgi-bin/paytool/get_order_detail"),

        // 发票管理族（官方 99436/99437，路由挂 /cgi-bin/paytool/ 段，官方全部即 POST）。
        (typeof(IWechatWorkThirdPartyPayToolInvoiceService),
            nameof(IWechatWorkThirdPartyPayToolInvoiceService.GetInvoiceListAsync),
            typeof(PostAttribute), "/cgi-bin/paytool/get_invoice_list"),
        (typeof(IWechatWorkThirdPartyPayToolInvoiceService),
            nameof(IWechatWorkThirdPartyPayToolInvoiceService.MarkInvoiceStatusAsync),
            typeof(PostAttribute), "/cgi-bin/paytool/mark_invoice_status"),

        // 应用版本付费族（官方 91910/91909/91913，路由挂 /cgi-bin/service/ 段，官方全部即 POST）。
        (typeof(IWechatWorkThirdPartyPayToolVersionSuiteService),
            nameof(IWechatWorkThirdPartyPayToolVersionSuiteService.GetVersionOrderListAsync),
            typeof(PostAttribute), "/cgi-bin/service/get_order_list"),
        (typeof(IWechatWorkThirdPartyPayToolVersionSuiteService),
            nameof(IWechatWorkThirdPartyPayToolVersionSuiteService.GetVersionOrderDetailAsync),
            typeof(PostAttribute), "/cgi-bin/service/get_order"),
        (typeof(IWechatWorkThirdPartyPayToolVersionSuiteService),
            nameof(IWechatWorkThirdPartyPayToolVersionSuiteService.ProlongTrialAsync),
            typeof(PostAttribute), "/cgi-bin/service/prolong_try"),
    };

    /// <summary>
    /// 契约守卫 PT1：收银台域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void PayToolEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(9,
            "收银台域官方共 9 个端点 = 收款工具族 4 条 + 发票管理族 2 条（provider_access_token）+ 应用版本付费族 3 条（suite_access_token）");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(9, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 PT2：三族接口层级——均为零端点父接口（IsAbstract）+ 唯一第三方子接口承载；
    /// 官方自建文档树无对应 API（代开发树与第三方树共享同一端点页、消费同一 provider / suite 令牌，
    /// 不另设子接口），继承链上不得出现其它子接口（能力漂移守卫）。
    /// </summary>
    [Theory]
    [InlineData(typeof(IWechatWorkPayToolOrderService), typeof(IWechatWorkThirdPartyPayToolOrderService), 4, "WechatWorkPayToolOrderService")]
    [InlineData(typeof(IWechatWorkPayToolInvoiceService), typeof(IWechatWorkThirdPartyPayToolInvoiceService), 2, "WechatWorkPayToolInvoiceService")]
    [InlineData(typeof(IWechatWorkPayToolVersionSuiteService), typeof(IWechatWorkThirdPartyPayToolVersionSuiteService), 3, "WechatWorkPayToolVersionSuiteService")]
    public void PayToolHierarchy_ShouldConvergeOnThirdPartyChildOnly(
        Type parent, Type child, int endpointCount, string parentImplementationClassName)
    {
        child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承零端点父接口 {parent.Name}");
        parentImplementationClassName.Should().Be(parent.Name.TrimStart('I'));

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅第三方应用开放本域端点：父接口零端点，端点全部由唯一第三方子接口承载");

        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
        derived.Should().BeEquivalentTo(new[] { child.Name },
            "收银台域官方自建应用不开放（代开发树与第三方树共享同一端点页），继承链上不得出现其它子接口");

        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(PayToolRegistryGroupName,
            $"{child.Name} 必须挂 {PayToolRegistryGroupName} 注册组（经 Add{PayToolRegistryGroupName}WebApiHttpClient() 注册）");
        childApi.InheritedFrom.Should().Be(parentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类 {parentImplementationClassName}");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(endpointCount, $"{child.Name} 承载本族全部 {endpointCount} 条官方端点");
    }

    /// <summary>
    /// 契约守卫 PT3：令牌绑定——收款工具 / 发票管理两族统一消费 ProviderAccessToken、
    /// 应用版本付费族消费 SuiteAccessToken，均以 Query 注入，
    /// 且<b>不声明凭据归属域键</b>（这两个令牌本身已无歧义，见归属域守卫 TO1）。
    /// </summary>
    [Fact]
    public void PayToolTokenBinding_ShouldSplitProviderAndSuiteRoutes()
    {
        var providerTokenInterfaces = new[]
        {
            typeof(IWechatWorkPayToolOrderService),
            typeof(IWechatWorkThirdPartyPayToolOrderService),
            typeof(IWechatWorkPayToolInvoiceService),
            typeof(IWechatWorkThirdPartyPayToolInvoiceService),
        };
        var suiteTokenInterfaces = new[]
        {
            typeof(IWechatWorkPayToolVersionSuiteService),
            typeof(IWechatWorkThirdPartyPayToolVersionSuiteService),
        };

        foreach (var iface in providerTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
                $"{iface.Name} 令牌路由键必须为 ProviderAccessToken（服务商级凭证）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("provider_access_token",
                $"{iface.Name} Query 注入参数名必须为官方契约的 provider_access_token");
            AssertNoOwningKey(iface);
        }

        foreach (var iface in suiteTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.SuiteAccessToken,
                $"{iface.Name} 令牌路由键必须为 SuiteAccessToken（套件级凭证）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("suite_access_token",
                $"{iface.Name} Query 注入参数名必须为官方契约的 suite_access_token");
            AssertNoOwningKey(iface);
        }

        static void AssertNoOwningKey(Type iface)
        {
            // 反射陷阱：未显式书写时读到的是构造函数默认值哨兵，故断言「不等于两个归属域键」。
            var token = iface.GetCustomAttribute<TokenAttribute>()!;
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
                $"{iface.Name} 已消费 provider / suite 令牌，不得叠加自建归属域键");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
                $"{iface.Name} 已消费 provider / suite 令牌，不得叠加授权企业归属域键");
        }
    }

    /// <summary>
    /// 契约守卫 PT4：签名面按族分叉——收款工具族 4 端点请求体必带 nonce_str / ts / sig；
    /// 发票管理族与应用版本付费族官方参数表<b>不含</b>签名三要素（不得凭空补字段）。
    /// </summary>
    [Fact]
    public void PayToolSignatureFields_ShouldExistOnlyOnOrderFamily()
    {
        var signedProperties = new[] { "NonceStr", "Ts", "Sig" };

        foreach (var method in new[]
                 {
                     nameof(IWechatWorkThirdPartyPayToolOrderService.CreateOrderAsync),
                     nameof(IWechatWorkThirdPartyPayToolOrderService.CloseOrderAsync),
                     nameof(IWechatWorkThirdPartyPayToolOrderService.GetOrderListAsync),
                     nameof(IWechatWorkThirdPartyPayToolOrderService.GetOrderDetailAsync),
                 })
        {
            var bodyType = BodyTypeOf(method, typeof(IWechatWorkThirdPartyPayToolOrderService));
            foreach (var property in signedProperties)
            {
                bodyType.GetProperty(property).Should().NotBeNull(
                    $"{method} 官方要求携带签名三要素，请求体 {bodyType.Name} 必须声明 {property}");
            }
        }

        foreach (var method in new[]
                 {
                     nameof(IWechatWorkThirdPartyPayToolInvoiceService.GetInvoiceListAsync),
                     nameof(IWechatWorkThirdPartyPayToolInvoiceService.MarkInvoiceStatusAsync),
                     nameof(IWechatWorkThirdPartyPayToolVersionSuiteService.GetVersionOrderListAsync),
                     nameof(IWechatWorkThirdPartyPayToolVersionSuiteService.GetVersionOrderDetailAsync),
                     nameof(IWechatWorkThirdPartyPayToolVersionSuiteService.ProlongTrialAsync),
                 })
        {
            var bodyType = BodyTypeOf(method, method.Contains("Invoice", StringComparison.Ordinal)
                ? typeof(IWechatWorkThirdPartyPayToolInvoiceService)
                : typeof(IWechatWorkThirdPartyPayToolVersionSuiteService));
            foreach (var property in signedProperties)
            {
                bodyType.GetProperty(property).Should().BeNull(
                    $"{bodyType.Name} 官方参数表不含签名三要素（nonce_str / ts / sig），不得凭空补字段");
            }
        }

        static Type BodyTypeOf(string method, Type iface)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            var body = target!.GetParameters().First(p => p.GetCustomAttribute<BodyAttribute>() != null);
            return body.ParameterType;
        }
    }

    /// <summary>
    /// 契约守卫 PT5：收银台模块的请求/响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （SerializerClassName 统一为 PayTool）。
    /// </summary>
    [Fact]
    public void PayToolDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = PayToolJsonContext.Default;

        var domainTypes = typeof(GetPayToolInvoiceListResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.PayTool"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记（请求 9 + 响应 8 + 嵌套对象 10 = 27）。
        domainTypes.Should().HaveCount(27,
            "收银台模块契约面类型数漂移须先核对官方文档再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于收银台域命名空间，必须登记进 PayToolJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be(PayToolRegistryGroupName,
                $"{type.Name} 的 SerializerClassName 必须为收银台域段 PayTool");
        }
    }

    /// <summary>
    /// 契约守卫 PT6：官方契约陷阱锁定——字段名一律照抄官方原文（含全小写 <c>orderid</c> / <c>suiteid</c>）、
    /// 收款订单详情只承载 <c>pay_type</c>（官方参数表重复列出的 <c>pay_from</c> 不承载）、
    /// 请求/响应方向专属字段的可空形态，以及「不签名」族的字段面。
    /// </summary>
    [Fact]
    public void PayToolDataModels_ShouldLockOfficialContractTraps()
    {
        // 应用版本付费：官方字段名为全小写 orderid / suiteid（照抄原文，勿「顺手修正」为 order_id / suite_id）。
        JsonNameShouldBe(typeof(GetPayToolVersionOrderDetailRequest),
            nameof(GetPayToolVersionOrderDetailRequest.OrderId), "orderid");
        JsonNameShouldBe(typeof(PayToolVersionOrder), nameof(PayToolVersionOrder.OrderId), "orderid");
        JsonNameShouldBe(typeof(PayToolVersionOrder), nameof(PayToolVersionOrder.SuiteId), "suiteid");
        JsonNameShouldBe(typeof(GetPayToolVersionOrderListRequest),
            nameof(GetPayToolVersionOrderListRequest.TestMode), "test_mode");

        // 收款订单详情：官方同时列出 pay_from 与 pay_type，示例只用 pay_type ⇒ 只承载 pay_type。
        typeof(PayToolOrderDetail).GetProperty(nameof(PayToolOrderDetail.PayType)).Should().NotBeNull();
        JsonNameShouldBe(typeof(PayToolOrderDetail), nameof(PayToolOrderDetail.PayType), "pay_type");
        typeof(PayToolOrderDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(p => p.Name).Should().NotContain("PayFrom",
                "官方返回参数表重复列出的 pay_from 未经示例证实，本 SDK 只承载 pay_type（勿「补齐」）。");

        // 收款工具请求体：签名三要素 + 业务字段名照抄官方原文。
        foreach (var property in new[] { "NonceStr", "Ts", "Sig" })
        {
            JsonNameShouldBe(typeof(CreatePayToolOrderRequest), property, ToSnakeCase(property));
            JsonNameShouldBe(typeof(ClosePayToolOrderRequest), property, ToSnakeCase(property));
            JsonNameShouldBe(typeof(GetPayToolOrderListRequest), property, ToSnakeCase(property));
            JsonNameShouldBe(typeof(GetPayToolOrderDetailRequest), property, ToSnakeCase(property));
        }

        JsonNameShouldBe(typeof(CreatePayToolOrderRequest), nameof(CreatePayToolOrderRequest.BusinessType), "business_type");
        JsonNameShouldBe(typeof(CreatePayToolOrderRequest), nameof(CreatePayToolOrderRequest.CustomCorpid), "custom_corpid");
        JsonNameShouldBe(typeof(CreatePayToolOrderRequest), nameof(CreatePayToolOrderRequest.PayType), "pay_type");
        JsonNameShouldBe(typeof(CreatePayToolOrderRequest), nameof(CreatePayToolOrderRequest.ProductList), "product_list");

        // product_list 三业务分支互斥（官方按 business_type 三选一），不做「合成为一个超集对象」的归一。
        JsonNameShouldBe(typeof(PayToolProductList), nameof(PayToolProductList.ThirdApp), "third_app");
        JsonNameShouldBe(typeof(PayToolProductList), nameof(PayToolProductList.CustomizedApp), "customized_app");
        JsonNameShouldBe(typeof(PayToolProductList), nameof(PayToolProductList.PromotionCase), "promotion_case");

        // 方向专属字段：请求侧 total_price / discount_info，响应侧 origin_price / paid_price。
        JsonNameShouldBe(typeof(PayToolBuyInfo), nameof(PayToolBuyInfo.TotalPrice), "total_price");
        JsonNameShouldBe(typeof(PayToolBuyInfo), nameof(PayToolBuyInfo.DiscountInfo), "discount_info");
        JsonNameShouldBe(typeof(PayToolBuyInfo), nameof(PayToolBuyInfo.OriginPrice), "origin_price");
        JsonNameShouldBe(typeof(PayToolBuyInfo), nameof(PayToolBuyInfo.PaidPrice), "paid_price");
        typeof(PayToolBuyInfo).GetProperty(nameof(PayToolBuyInfo.TotalPrice))!.PropertyType
            .Should().Be(typeof(long?), "total_price 官方为 uint32、单位分且需大于 0 且不超过 500 万，以 long? 承载");
        typeof(PayToolDiscountInfo).GetProperty(nameof(PayToolDiscountInfo.DiscountRemarks))!.PropertyType
            .Should().Be(typeof(string), "discount_remarks 为官方必填字符串");

        // 发票管理族：字段名照抄官方原文；limit 为可选（最大值 100、默认 50）。
        JsonNameShouldBe(typeof(GetPayToolInvoiceListRequest), nameof(GetPayToolInvoiceListRequest.Limit), "limit");
        typeof(GetPayToolInvoiceListRequest).GetProperty(nameof(GetPayToolInvoiceListRequest.Limit))!.PropertyType
            .Should().Be(typeof(int?), "limit 官方可选（不填默认 50），故可空");
        JsonNameShouldBe(typeof(MarkPayToolInvoiceStatusRequest), nameof(MarkPayToolInvoiceStatusRequest.OperUserid), "oper_userid");
        JsonNameShouldBe(typeof(MarkPayToolInvoiceStatusRequest), nameof(MarkPayToolInvoiceStatusRequest.InvoiceStatus), "invoice_status");
        JsonNameShouldBe(typeof(MarkPayToolInvoiceStatusRequest), nameof(MarkPayToolInvoiceStatusRequest.InvoiceNote), "invoice_note");
        JsonNameShouldBe(typeof(GetPayToolInvoiceListResponse), nameof(GetPayToolInvoiceListResponse.InvoiceList), "invoice_list");

        static string ToSnakeCase(string property)
            => property switch
            {
                "NonceStr" => "nonce_str",
                "Ts" => "ts",
                "Sig" => "sig",
                _ => throw new ArgumentOutOfRangeException(nameof(property), property, "未登记的属性"),
            };
    }

    /// <summary>
    /// 契约守卫 PT7：应用版本付费族的「获取企业永久授权码 / 获取企业授权信息」官方文档（91911 / 91912）
    /// 与授权流接口族为<b>同一端点</b>，本域<b>不得重复声明路由</b>（否则同一路由出现两处映射）。
    /// </summary>
    [Fact]
    public void PayToolVersionFamily_ShouldNotDuplicateAuthorizationEndpoints()
    {
        var duplicated = Routes
            .Where(r => r.Route is "/cgi-bin/service/get_permanent_code" or "/cgi-bin/service/get_auth_info")
            .ToList();
        duplicated.Should().BeEmpty(
            "获取企业永久授权码（91911）/ 获取企业授权信息（91912）即授权流接口族的 get_permanent_code / get_auth_info，"
            + "已由 IWechatWorkProviderAuthenticationService 承载，收银台域不得重复声明");

        typeof(IWechatWorkThirdPartyPayToolVersionSuiteService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(3,
                "应用版本付费族只承载 91910 / 91909 / 91913 三个新端点；91911 / 91912 属授权流族");
    }

    /// <summary>
    /// 契约守卫 PT8：签名算法（官方 98768）——排序、空值剔除、sig 不参与、嵌套元组递归摊平。
    /// </summary>
    [Fact]
    public void PayToolSignature_ShouldFollowOfficialAlgorithm()
    {
        // 官方示例 1：键值对按 ASCII 字典序排序、空值不参与、sig 不参与。
        var parameters = new List<KeyValuePair<string, string?>>
        {
            new("orderid", "ord7"),
            new("buyer_corpid", "ww66302cfadbdd3c64"),
            new("buyer_userid", "invitetest"),
            new("product_id", "product_id_xxx"),
            new("product_name", "product_name_xxx"),
            new("product_detail", "product_detail_xxx"),
            new("unit_name", "台"),
            new("unit_price", "1"),
            new("num", "3"),
            new("nonce_str", "129031823"),
            new("ts", "1548302135"),
            new("sig", "mPOwVW/vQ74xN+b+Yu1KMa9RrmhKJaJjAtXHTof+EpU="),
            new("empty_value", null),
            new("empty_string", string.Empty),
        };

        WechatPayToolSignature.BuildSignString(parameters).Should().Be(
            "buyer_corpid=ww66302cfadbdd3c64&buyer_userid=invitetest&nonce_str=129031823&num=3&orderid=ord7"
            + "&product_detail=product_detail_xxx&product_id=product_id_xxx&product_name=product_name_xxx"
            + "&ts=1548302135&unit_name=台&unit_price=1",
            "PT8：官方示例 1 的 stringA 逐字一致（sig 与空值不参与、区分大小写、按 ASCII 字典序排序）");

        // 官方示例 1 给出的期望签名：secret 与 stringA 固定 ⇒ 结果确定。
        WechatPayToolSignature.Sign(
            "at23pxnPBNQY3JiA8N5U1gabiQqxZwqH_Gihg7a_wrULmlOPVP-iiRjv9JWYPrDk", parameters)
            .Should().Be("/WTXl/L2kJCYKJE5yY2JZvPq3rUjFf/pf39UhyJ2GUo=",
                "PT8：官方示例 1 的最终签名逐字一致（HMAC-SHA256 + Base64）");

        // 官方规则：元组（嵌套对象 / 对象数组）不直接参与签名，递归用子节点签名。
        var nested = WechatPayToolSignature.Flatten(new List<KeyValuePair<string, object?>>
        {
            new("business_type", 1),
            new("nonce_str", "129031823"),
            new("ts", 1548302135L),
            new("product_list", new Dictionary<string, object?>
            {
                ["third_app"] = new Dictionary<string, object?>
                {
                    ["order_type"] = 0,
                    ["buy_info_list"] = new List<object?>
                    {
                        new Dictionary<string, object?>
                        {
                            ["suiteid"] = "wx63bea8582b858ee7",
                            ["edition_id"] = "sp7fd5170a8e807a44",
                            ["duration_days"] = 1,
                        },
                    },
                },
            }),
        });

        nested.Select(p => p.Key + "=" + p.Value).Should().Equal(
            new[]
            {
                "business_type=1", "nonce_str=129031823", "ts=1548302135",
                "order_type=0", "suiteid=wx63bea8582b858ee7", "edition_id=sp7fd5170a8e807a44", "duration_days=1",
            },
            "官方规则：product_list 及其子元组均不直接参与签名，摊平后按子节点参与（保持传入顺序，不排序）");

        WechatPayToolSignature.SignNested("secret", new List<KeyValuePair<string, object?>>
        {
            new("a", "1"),
            new("nested", new Dictionary<string, object?> { ["b"] = 2 }),
        }).Should().Be(WechatPayToolSignature.Sign("secret", new List<KeyValuePair<string, string?>>
        {
            new("a", "1"),
            new("b", "2"),
        }), "SignNested 等价于先摊平再签名");

        // 空白密钥 fail-fast（官方未定义，SDK 侧显式拒绝以免产出必然失败的请求）。
        var blank = () => WechatPayToolSignature.Sign("   ", parameters);
        blank.Should().Throw<ArgumentException>();
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName, $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}