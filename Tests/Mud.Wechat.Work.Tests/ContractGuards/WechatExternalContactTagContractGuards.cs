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
using Mud.Wechat.Work.DataModels.ExternalContact.Tag;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「客户标签管理」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 9 个端点（获取企业标签库、添加 / 编辑 / 删除企业客户标签、
/// 编辑客户企业标签、获取 / 添加 / 编辑 / 删除指定规则组下的企业客户标签）收敛于父接口
/// <see cref="IWechatWorkExternalContactTagService"/>；自建 / 第三方 / 代开发子接口均为零差异端点空标记
/// （代开发文档树 96320 / 96322 / 99544 与自建 / 第三方同路由）。
/// </para>
/// </remarks>
public class WechatExternalContactTagContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactTagService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 9 条公共端点，全部 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.GetCorpTagListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_corp_tag_list"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.AddCorpTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/add_corp_tag"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.EditCorpTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/edit_corp_tag"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.DelCorpTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/del_corp_tag"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.MarkTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/mark_tag"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.GetStrategyTagListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_strategy_tag_list"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.AddStrategyTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/add_strategy_tag"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.EditStrategyTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/edit_strategy_tag"),
        (typeof(IWechatWorkExternalContactTagService),
            nameof(IWechatWorkExternalContactTagService.DelStrategyTagAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/del_strategy_tag"),
    };

    /// <summary>
    /// 契约守卫 CT1：客户标签管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意企业客户标签增删改以 <c>corp_tag</c> 命名、规则组标签以 <c>strategy_tag</c> 命名，
    /// 与异步导入的 <c>batch/getresult</c>、异步导出的 <c>export/get_result</c> 拼写差异无涉（均为官方契约）。
    /// </summary>
    [Fact]
    public void TagEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(9,
            "本域 9 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(9, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
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
    /// 契约守卫 CT2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建 / 第三方 / 代开发三个子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void TagInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactTagService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactTagService),
            typeof(IWechatWorkThirdPartyExternalContactTagService),
            typeof(IWechatWorkProviderExternalContactTagService),
        };

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
            childApi!.RegistryGroupName.Should().Be(ExternalContactRegistryGroupName,
                $"{child.Name} 必须挂 {ExternalContactRegistryGroupName} 注册组" +
                $"（与企业服务人员管理域、客户管理域、在职继承域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 CT1/CT2。
        foreach (var child in children)
        {
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 CT3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void TagTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactTagService),
            typeof(IWechatWorkInternalExternalContactTagService),
            typeof(IWechatWorkThirdPartyExternalContactTagService),
            typeof(IWechatWorkProviderExternalContactTagService),
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
    /// 契约守卫 CT4：客户标签管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void TagDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = Mud.Wechat.Work.DataModels.WechatWorkJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(GetCorpTagListRequest), typeof(AddCorpTagRequest), typeof(CorpTagCreateItem),
            typeof(EditCorpTagRequest), typeof(DelCorpTagRequest), typeof(MarkTagRequest),
            typeof(GetStrategyTagListRequest), typeof(AddStrategyTagRequest),
            typeof(EditStrategyTagRequest), typeof(DelStrategyTagRequest),
            typeof(CorpTagItem), typeof(CorpTagGroupItem), typeof(StrategyTagGroupItem),
            typeof(GetCorpTagListResponse), typeof(AddCorpTagResponse),
            typeof(GetStrategyTagListResponse), typeof(AddStrategyTagResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是客户标签管理域契约面类型，必须登记进 WechatWorkJsonContext（AOT 源生成）");
        }
    }
}
