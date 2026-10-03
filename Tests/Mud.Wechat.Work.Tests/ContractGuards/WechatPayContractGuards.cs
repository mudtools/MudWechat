// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.Pay;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 企业支付模块（Pay 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （对外收款记录域 + 收款商户号管理域（仅自建） + 资金流水域（仅自建） +
/// 创建对外收款账户域（仅自建））。
/// </summary>
/// <remarks>
/// <para>
/// 形态：对外收款记录域为三类应用公共面收敛父接口（自建 / 第三方 / 代开发子接口均为零差异端点空标记）；
/// 收款商户号管理域、资金流水域、创建对外收款账户域官方仅自建应用开放（代开发 / 第三方暂不支持），
/// 均为零端点父接口 + 仅自建子接口承载端点（形态对齐机器人管理域守卫）。
/// </para>
/// </remarks>
public class WechatPayContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string BillParentImplementationClassName = "WechatWorkPayBillService";

    private const string MerchantParentImplementationClassName = "WechatWorkPayMerchantService";

    private const string FundFlowParentImplementationClassName = "WechatWorkPayFundFlowService";

    private const string MchApplyParentImplementationClassName = "WechatWorkPayMchApplyService";

    private const string PayRegistryGroupName = "Pay";

    /// <summary>
    /// 对外收款记录域官方路由表（父接口 2 条公共端点，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BillRoutes =
    {
        // 获取对外收款记录（自建 93667、第三方 93727、代开发 96701）。
        (typeof(IWechatWorkPayBillService),
            nameof(IWechatWorkPayBillService.GetBillListAsync),
            typeof(PostAttribute), "/cgi-bin/externalpay/get_bill_list"),
        // 获取收款项目的商户单号（自建 95944、第三方 95936、代开发 96702）。
        (typeof(IWechatWorkPayBillService),
            nameof(IWechatWorkPayBillService.GetPaymentInfoAsync),
            typeof(PostAttribute), "/cgi-bin/externalpay/get_payment_info"),
    };

    /// <summary>
    /// 收款商户号管理域官方路由表（仅自建开放，2 条端点全部声明于自建子接口，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MerchantRoutes =
    {
        // 查询商户号详情（93666；官方路由为 getmerchant 单词连写，勿改 get_merchant）。
        (typeof(IWechatWorkInternalPayMerchantService),
            nameof(IWechatWorkInternalPayMerchantService.GetMerchantAsync),
            typeof(PostAttribute), "/cgi-bin/externalpay/getmerchant"),
        // 设置商户号使用范围（93666）。
        (typeof(IWechatWorkInternalPayMerchantService),
            nameof(IWechatWorkInternalPayMerchantService.SetMerchantUseScopeAsync),
            typeof(PostAttribute), "/cgi-bin/externalpay/set_mch_use_scope"),
    };

    /// <summary>
    /// 资金流水域官方路由表（仅自建开放，1 条端点声明于自建子接口，POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] FundFlowRoutes =
    {
        // 获取资金流水（98100）。
        (typeof(IWechatWorkInternalPayFundFlowService),
            nameof(IWechatWorkInternalPayFundFlowService.GetFundFlowAsync),
            typeof(PostAttribute), "/cgi-bin/externalpay/get_fund_flow"),
    };

    /// <summary>
    /// 创建对外收款账户域官方路由表（仅自建开放，3 条端点全部声明于自建子接口，全部为 POST；
    /// 路由在 /cgi-bin/miniapppay/ 下，区别于对外收款记录域的 /cgi-bin/externalpay/）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MchApplyRoutes =
    {
        // 提交创建对外收款账户的申请单（98973）。
        (typeof(IWechatWorkInternalPayMchApplyService),
            nameof(IWechatWorkInternalPayMchApplyService.ApplyMchAsync),
            typeof(PostAttribute), "/cgi-bin/miniapppay/apply_mch"),
        // 查询申请单状态（98974）。
        (typeof(IWechatWorkInternalPayMchApplyService),
            nameof(IWechatWorkInternalPayMchApplyService.GetApplymentStatusAsync),
            typeof(PostAttribute), "/cgi-bin/miniapppay/get_applyment_status"),
        // 提交图片（98972；multipart/form-data 上传）。
        (typeof(IWechatWorkInternalPayMchApplyService),
            nameof(IWechatWorkInternalPayMchApplyService.UploadImageAsync),
            typeof(PostAttribute), "/cgi-bin/miniapppay/upload_image"),
    };

    /// <summary>
    /// 契约守卫 PAY1a：对外收款记录域全部端点路由必须与官方契约一致
    /// （2 个端点为三类应用公共面，全部收敛父接口）。
    /// </summary>
    [Fact]
    public void PayBillEndpoints_ShouldMatchOfficialRoutes()
    {
        BillRoutes.Should().HaveCount(2,
            "对外收款记录域 2 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = BillRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "对外收款记录域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in BillRoutes)
        {
            // DeclaredOnly：公共端点必须落在父接口自身声明，不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 PAY1b：收款商户号管理域全部端点路由必须与官方契约一致
    /// （官方仅自建开放，路由 <c>externalpay/getmerchant</c> 为官方单词连写原文，勿改 get_merchant）。
    /// </summary>
    [Fact]
    public void PayMerchantEndpoints_ShouldMatchOfficialRoutes()
    {
        MerchantRoutes.Should().HaveCount(2,
            "收款商户号管理域 2 个端点（查询商户号详情 + 设置商户号使用范围）官方仅自建应用开放，全部声明于自建子接口");

        var distinctRoutes = MerchantRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "收款商户号管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in MerchantRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 PAY1c：资金流水域全部端点路由必须与官方契约一致
    /// （官方仅自建开放；官方多数统计/查询接口即 POST，勿改成 GET）。
    /// </summary>
    [Fact]
    public void PayFundFlowEndpoints_ShouldMatchOfficialRoutes()
    {
        FundFlowRoutes.Should().HaveCount(1,
            "资金流水域 1 个端点官方仅自建应用开放，声明于自建子接口");

        foreach (var (iface, method, httpAttribute, route) in FundFlowRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 PAY1d：创建对外收款账户域全部端点路由必须与官方契约一致
    /// （官方仅自建开放，路由在 <c>miniapppay/</c> 下；
    /// upload_image 为 multipart/form-data 上传，须以 <c>[MultipartForm]</c> 承载）。
    /// </summary>
    [Fact]
    public void PayMchApplyEndpoints_ShouldMatchOfficialRoutes()
    {
        MchApplyRoutes.Should().HaveCount(3,
            "创建对外收款账户域 3 个端点（提交申请单 + 查询申请单状态 + 提交图片）官方仅自建应用开放，全部声明于自建子接口");

        var distinctRoutes = MchApplyRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "创建对外收款账户域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in MchApplyRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // upload_image 为 multipart/form-data 上传：请求体参数必须以 [MultipartForm] 标注
        // （形态对齐上传附件资源域 95098 先例），不得误标 [Body]（JSON 序列化通路）。
        var uploadMethod = typeof(IWechatWorkInternalPayMchApplyService).GetMethod(
            nameof(IWechatWorkInternalPayMchApplyService.UploadImageAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        uploadMethod.Should().NotBeNull();
        uploadMethod!.GetParameters()
            .Any(p => p.GetCustomAttribute<MultipartFormAttribute>() is not null)
            .Should().BeTrue("提交图片接口请求体为 multipart/form-data，必须以 [MultipartForm] 标注（对齐上传附件资源域先例）");
    }

    /// <summary>
    /// 契约守卫 PAY2：三类应用公共面域的接口层级与生成器注册形态——对外收款记录域公共端点收敛于
    /// IsAbstract 父接口，自建 / 第三方 / 代开发子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void PayBillInterfaceHierarchy_ShouldConvergeOnAbstractParentWithPayRegistry()
    {
        var children = new[]
        {
            typeof(IWechatWorkInternalPayBillService),
            typeof(IWechatWorkThirdPartyPayBillService),
            typeof(IWechatWorkProviderPayBillService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkPayBillService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkPayBillService");
        }

        var parentApi = typeof(IWechatWorkPayBillService).GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var child in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(PayRegistryGroupName,
                $"{child.Name} 必须挂 {PayRegistryGroupName} 注册组" +
                $"（Pay 模块共用 Add{PayRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(BillParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 PAY1a/PAY2。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 PAY2b：仅单类应用开放域的接口层级——收款商户号管理域、资金流水域与创建对外收款账户域
    /// 官方仅自建应用开放，均为零端点父接口 + 仅自建子接口承载端点，
    /// 且继承链上不得出现第三方 / 代开发子接口（形态对齐机器人管理域守卫）。
    /// </summary>
    [Fact]
    public void PaySingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var singleChildFamilies = new[]
        {
            (typeof(IWechatWorkPayMerchantService),
                new[] { typeof(IWechatWorkInternalPayMerchantService) },
                "收款商户号管理域官方仅向自建应用开放（代开发/第三方暂不支持）"),
            (typeof(IWechatWorkPayFundFlowService),
                new[] { typeof(IWechatWorkInternalPayFundFlowService) },
                "资金流水域官方仅向自建应用开放（代开发/第三方暂不支持）"),
            (typeof(IWechatWorkPayMchApplyService),
                new[] { typeof(IWechatWorkInternalPayMchApplyService) },
                "创建对外收款账户域官方仅向自建应用开放（代开发/第三方暂不支持）"),
        };

        foreach (var (parent, expectedChildren, because) in singleChildFamilies)
        {
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();

            assignable.Should().BeEquivalentTo(expectedChildren,
                $"继承链上不得出现自建之外的子接口：{because}");

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty("零端点父接口：端点全部声明于唯一的子接口");

            foreach (var child in expectedChildren)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(PayRegistryGroupName,
                    $"{child.Name} 必须挂 {PayRegistryGroupName} 注册组");
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().NotBeEmpty($"{child.Name} 为唯一端点承载接口：{because}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 PAY3：令牌绑定——Pay 模块全部 10 个接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void PayTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkPayBillService),
            typeof(IWechatWorkInternalPayBillService),
            typeof(IWechatWorkThirdPartyPayBillService),
            typeof(IWechatWorkProviderPayBillService),
            typeof(IWechatWorkPayMerchantService),
            typeof(IWechatWorkInternalPayMerchantService),
            typeof(IWechatWorkPayFundFlowService),
            typeof(IWechatWorkInternalPayFundFlowService),
            typeof(IWechatWorkPayMchApplyService),
            typeof(IWechatWorkInternalPayMchApplyService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 PAY4：企业支付模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 34 个契约面类型）。
    /// </summary>
    [Fact]
    public void PayDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = PayJsonContext.Default;

        var requiredTypes = new[]
        {
            // 对外收款记录域。
            typeof(GetPayBillListRequest), typeof(GetPayBillListResponse),
            typeof(PayBillItem), typeof(PayBillCommodity), typeof(PayBillRefund),
            typeof(PayBillContact), typeof(PayBillMiniprogramInfo),
            typeof(GetPayPaymentInfoRequest), typeof(GetPayPaymentInfoResponse), typeof(PayOutTradeNoItem),
            // 收款商户号管理域。
            typeof(GetPayMerchantRequest), typeof(GetPayMerchantResponse), typeof(PayMerchantUseScope),
            typeof(SetPayMerchantUseScopeRequest),
            // 资金流水域。
            typeof(GetPayFundFlowRequest), typeof(GetPayFundFlowResponse),
            typeof(PayFundFlowItem), typeof(PayFundFlowGroup),
            // 创建对外收款账户域（进件申请单 + 状态查询 + 图片上传）。
            typeof(ApplyPayMchRequest), typeof(ApplyPayMchResponse),
            typeof(PayBusinessLicenseInfo), typeof(PayFinanceInstitutionInfo), typeof(PayIdCardInfo),
            typeof(PayContactInfo), typeof(PayAccountInfo), typeof(PayBankCardSupplement),
            typeof(PaySalesSceneInfo), typeof(PayPicIdList),
            typeof(GetPayApplymentStatusRequest), typeof(GetPayApplymentStatusResponse),
            typeof(PayApplymentStatus), typeof(PayApplymentAuditDetail), typeof(PayAccountValidation),
            typeof(UploadPayImageResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是企业支付模块契约面类型，必须登记进 PayJsonContext（AOT 源生成）");
        }
    }
}
