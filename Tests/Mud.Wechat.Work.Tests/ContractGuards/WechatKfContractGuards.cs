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
using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 微信客服模块（Kf 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （客服账号管理域 + 接待人员管理域）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 8 个端点（客服账号管理 5 条 + 接待人员管理 3 条）
/// 收敛于两个父接口 <see cref="IWechatWorkKfAccountService"/> / <see cref="IWechatWorkKfServicerService"/>；
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记。
/// </para>
/// </remarks>
public class WechatKfContractGuards
{
    /// <summary>
    /// 客服账号管理域父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string AccountParentImplementationClassName = "WechatWorkKfAccountService";

    /// <summary>接待人员管理域父接口生成实现类名（锁定规则同上）。</summary>
    private const string ServicerParentImplementationClassName = "WechatWorkKfServicerService";

    private const string KfRegistryGroupName = "Kf";

    /// <summary>
    /// 客服账号管理域官方路由表（父接口 5 条公共端点，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AccountRoutes =
    {
        // 客服账号管理族（自建/第三方 94661、代开发 96404）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.AddAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/add"),
        // 获取客服账号列表（自建/第三方 94662、代开发 96415）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.GetAccountListAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/list"),
        // 删除客服账号（自建/第三方 94663、代开发 96405）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.DeleteAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/del"),
        // 修改客服账号（自建/第三方 94664、代开发 96406）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.UpdateAccountAsync),
            typeof(PostAttribute), "/cgi-bin/kf/account/update"),
        // 获取客服账号链接（自建/第三方 94665、代开发 96416）。
        (typeof(IWechatWorkKfAccountService),
            nameof(IWechatWorkKfAccountService.GetAccountContactWayAsync),
            typeof(PostAttribute), "/cgi-bin/kf/add_contact_way"),
    };

    /// <summary>
    /// 接待人员管理域官方路由表（父接口 3 条公共端点；servicer/list 为 GET，其余 2 条为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ServicerRoutes =
    {
        // 添加接待人员（自建/第三方 94646、代开发 96418）。
        (typeof(IWechatWorkKfServicerService),
            nameof(IWechatWorkKfServicerService.AddServicerAsync),
            typeof(PostAttribute), "/cgi-bin/kf/servicer/add"),
        // 删除接待人员（自建/第三方 94647、代开发 96419）。
        (typeof(IWechatWorkKfServicerService),
            nameof(IWechatWorkKfServicerService.DeleteServicerAsync),
            typeof(PostAttribute), "/cgi-bin/kf/servicer/del"),
        // 获取接待人员列表（自建/第三方 94645、代开发 96420），open_kfid 走 Query。
        (typeof(IWechatWorkKfServicerService),
            nameof(IWechatWorkKfServicerService.GetServicerListAsync),
            typeof(GetAttribute), "/cgi-bin/kf/servicer/list"),
    };

    /// <summary>
    /// 契约守卫 KF1a：客服账号管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意 <c>kf/account/list</c>（列表）官方即 POST（勿改 GET）；
    /// <c>kf/add_contact_way</c>（获取客服账号链接）直接挂在 <c>/cgi-bin/kf/</c> 根下，
    /// 不在 <c>account/</c> 子路径，与其余端点形状不同。
    /// </summary>
    [Fact]
    public void KfAccountEndpoints_ShouldMatchOfficialRoutes()
    {
        AccountRoutes.Should().HaveCount(5,
            "客服账号管理域 5 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = AccountRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(5, "客服账号管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in AccountRoutes)
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
    /// 契约守卫 KF1b：接待人员管理域全部端点路由必须与官方契约一致；
    /// <c>kf/servicer/list</c> 官方即 GET（open_kfid 走 Query），勿改成 POST。
    /// </summary>
    [Fact]
    public void KfServicerEndpoints_ShouldMatchOfficialRoutes()
    {
        ServicerRoutes.Should().HaveCount(3,
            "接待人员管理域 3 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = ServicerRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "接待人员管理域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in ServicerRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // servicer/list 的 open_kfid 为官方必填 Query 参数，须以 [Query("open_kfid")] 显式声明（非可空）。
        var listMethod = typeof(IWechatWorkKfServicerService).GetMethod(
            nameof(IWechatWorkKfServicerService.GetServicerListAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        listMethod.Should().NotBeNull();
        var queryParam = listMethod!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "open_kfid");
        queryParam.Should().NotBeNull("servicer/list 的 open_kfid 必须以 [Query(\"open_kfid\")] 显式承载");
        queryParam!.ParameterType.Should().Be<string>("open_kfid 为官方必填参数，不得声明为可选");
    }

    /// <summary>
    /// 契约守卫 KF2：接口层级与生成器注册形态——两域公共端点分别收敛于 IsAbstract 父接口，
    /// 自建 / 第三方 / 代开发子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void KfInterfaceHierarchy_ShouldConvergeOnAbstractParentsWithKfRegistry()
    {
        var families = new[]
        {
            (AccountParentImplementationClassName, typeof(IWechatWorkKfAccountService), new[]
            {
                typeof(IWechatWorkInternalKfAccountService),
                typeof(IWechatWorkThirdPartyKfAccountService),
                typeof(IWechatWorkProviderKfAccountService),
            }),
            (ServicerParentImplementationClassName, typeof(IWechatWorkKfServicerService), new[]
            {
                typeof(IWechatWorkInternalKfServicerService),
                typeof(IWechatWorkThirdPartyKfServicerService),
                typeof(IWechatWorkProviderKfServicerService),
            }),
        };

        foreach (var (parentImplementationClassName, parent, children) in families)
        {
            foreach (var child in children)
            {
                child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
            }

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

            foreach (var child in children)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(KfRegistryGroupName,
                    $"{child.Name} 必须挂 {KfRegistryGroupName} 注册组" +
                    $"（Kf 模块共用 Add{KfRegistryGroupName}WebApiHttpClient()）");
                childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                    $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

                // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 KF1/KF2。
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
            }
        }
    }

    /// <summary>
    /// 契约守卫 KF3：令牌绑定——两域共 8 接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void KfTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkKfAccountService),
            typeof(IWechatWorkInternalKfAccountService),
            typeof(IWechatWorkThirdPartyKfAccountService),
            typeof(IWechatWorkProviderKfAccountService),
            typeof(IWechatWorkKfServicerService),
            typeof(IWechatWorkInternalKfServicerService),
            typeof(IWechatWorkThirdPartyKfServicerService),
            typeof(IWechatWorkProviderKfServicerService),
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
    /// 契约守卫 KF4：微信客服模块的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void KfDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = KfJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(AddKfAccountRequest), typeof(GetKfAccountListRequest), typeof(DeleteKfAccountRequest),
            typeof(UpdateKfAccountRequest), typeof(GetKfAccountContactWayRequest),
            typeof(AddKfServicerRequest), typeof(DeleteKfServicerRequest),
            typeof(AddKfAccountResponse), typeof(GetKfAccountListResponse),
            typeof(GetKfAccountContactWayResponse),
            typeof(AddKfServicerResponse), typeof(DeleteKfServicerResponse), typeof(GetKfServicerListResponse),
            typeof(KfAccount), typeof(KfServicer), typeof(KfServicerOperateResult),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是微信客服模块契约面类型，必须登记进 KfJsonContext（AOT 源生成）");
        }
    }
}
