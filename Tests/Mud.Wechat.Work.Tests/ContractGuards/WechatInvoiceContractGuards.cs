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
using Mud.Wechat.Work.DataModels.Invoice;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 电子发票模块（Invoice 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （查询电子发票域 + 更新发票状态域 + 批量更新发票状态域 + 批量查询电子发票域：三类应用公共面收敛父接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放的电子发票端点集完全一致（查询电子发票 / 更新发票状态 / 批量更新发票状态 /
/// 批量查询电子发票，全部 POST、全部走 access_token），收敛于 IsAbstract 父接口，
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记。
/// </para>
/// </remarks>
public class WechatInvoiceContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string InvoiceParentImplementationClassName = "WechatWorkInvoiceService";

    private const string InvoiceRegistryGroupName = "Invoice";

    /// <summary>
    /// 电子发票域官方路由表（父接口 4 条公共端点，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] InvoiceRoutes =
    {
        // 查询电子发票（自建 90284、第三方 90420、代开发 99451）。
        (typeof(IWechatWorkInvoiceService),
            nameof(IWechatWorkInvoiceService.GetInvoiceInfoAsync),
            "/cgi-bin/card/invoice/reimburse/getinvoiceinfo"),
        // 更新发票状态（自建 90285、第三方 90421、代开发 99452），单张发票锁定/解锁/报销。
        (typeof(IWechatWorkInvoiceService),
            nameof(IWechatWorkInvoiceService.UpdateInvoiceStatusAsync),
            "/cgi-bin/card/invoice/reimburse/updateinvoicestatus"),
        // 批量更新发票状态（自建 90286、第三方 90422、代开发 99453），事务性操作、同 openid 约束。
        (typeof(IWechatWorkInvoiceService),
            nameof(IWechatWorkInvoiceService.UpdateInvoiceStatusBatchAsync),
            "/cgi-bin/card/invoice/reimburse/updatestatusbatch"),
        // 批量查询电子发票（自建 90287、第三方 90423、代开发 99454），官方即 POST（勿改 GET）。
        (typeof(IWechatWorkInvoiceService),
            nameof(IWechatWorkInvoiceService.GetInvoiceInfoBatchAsync),
            "/cgi-bin/card/invoice/reimburse/getinvoiceinfobatch"),
    };

    /// <summary>
    /// 契约守卫 IV1：电子发票域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意路由均在 <c>/cgi-bin/card/invoice/reimburse/</c> 下且官方即 POST；
    /// 单张更新（updateinvoicestatus）与批量更新（updatestatusbatch）请求体形状不同：
    /// 前者按 card_id + encrypt_code 定位，后者按 openid + invoice_list 定位（勿混用）。
    /// </summary>
    [Fact]
    public void InvoiceEndpoints_ShouldMatchOfficialRoutes()
    {
        InvoiceRoutes.Should().HaveCount(4,
            "电子发票域 4 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = InvoiceRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "电子发票域各端点路由互不重复");

        foreach (var (iface, method, route) in InvoiceRoutes)
        {
            // DeclaredOnly：公共端点必须落在父接口自身声明，不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var post = target!.GetCustomAttribute<PostAttribute>();
            post.Should().NotBeNull($"{iface.Name}.{method} 官方即 POST，必须声明 [Post] 路由");
            post!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 IV2：电子发票域接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，
    /// 自建 / 第三方 / 代开发子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void InvoiceInterfaceHierarchy_ShouldConvergeOnAbstractParentWithInvoiceRegistry()
    {
        var children = new[]
        {
            typeof(IWechatWorkInternalInvoiceService),
            typeof(IWechatWorkThirdPartyInvoiceService),
            typeof(IWechatWorkProviderInvoiceService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkInvoiceService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkInvoiceService");
        }

        var parentApi = typeof(IWechatWorkInvoiceService).GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var child in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(InvoiceRegistryGroupName,
                $"{child.Name} 必须挂 {InvoiceRegistryGroupName} 注册组" +
                $"（Invoice 模块共用 Add{InvoiceRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(InvoiceParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 IV1/IV2。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 IV3：令牌绑定——Invoice 模块全部 4 个接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void InvoiceTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkInvoiceService),
            typeof(IWechatWorkInternalInvoiceService),
            typeof(IWechatWorkThirdPartyInvoiceService),
            typeof(IWechatWorkProviderInvoiceService),
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
    /// 契约守卫 IV4：电子发票模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 10 个契约面类型）。
    /// </summary>
    [Fact]
    public void InvoiceDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = InvoiceJsonContext.Default;

        var requiredTypes = new[]
        {
            // 查询电子发票。
            typeof(GetInvoiceInfoRequest), typeof(GetInvoiceInfoResponse),
            typeof(InvoiceUserInfo), typeof(InvoiceItemInfo),
            // 批量查询电子发票。
            typeof(GetInvoiceInfoBatchRequest), typeof(GetInvoiceInfoBatchResponse), typeof(InvoiceInfoItem),
            // 更新发票状态（单张 + 批量），标识项两接口共用。
            typeof(UpdateInvoiceStatusRequest), typeof(UpdateInvoiceStatusBatchRequest), typeof(InvoiceIdentifier),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是电子发票模块契约面类型，必须登记进 InvoiceJsonContext（AOT 源生成）");
        }
    }
}
