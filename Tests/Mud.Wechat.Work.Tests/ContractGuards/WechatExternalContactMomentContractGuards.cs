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
using Mud.Wechat.Work.DataModels.ExternalContact.Moment;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「客户朋友圈」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 14 个端点（发表任务 3 条 + 发表记录数据 5 条 + 规则组 6 条）
/// 收敛于父接口 <see cref="IWechatWorkExternalContactMomentService"/>；
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记。
/// </para>
/// </remarks>
public class WechatExternalContactMomentContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactMomentService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 14 条公共端点；get_moment_task_result 为 GET + jobid Query，其余 13 条为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 发表任务族（发表 95094/95095/96351、停止 97612/97616/97615）。
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.AddMomentTaskAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/add_moment_task"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentTaskResultAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get_moment_task_result"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.CancelMomentTaskAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/cancel_moment_task"),
        // 发表记录数据族（93333/93443/96352）。
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_moment_list"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentPublishListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_moment_task"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentCustomerListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_moment_customer_list"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentSendResultAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_moment_send_result"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentCommentsAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_moment_comments"),
        // 朋友圈规则组族（94890/99541/99545）。
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentStrategyListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/moment_strategy/list"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentStrategyDetailAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/moment_strategy/get"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.GetMomentStrategyRangeAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/moment_strategy/get_range"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.CreateMomentStrategyAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/moment_strategy/create"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.UpdateMomentStrategyAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/moment_strategy/edit"),
        (typeof(IWechatWorkExternalContactMomentService),
            nameof(IWechatWorkExternalContactMomentService.DeleteMomentStrategyAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/moment_strategy/del"),
    };

    /// <summary>
    /// 契约守卫 MO1：客户朋友圈域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意 <c>get_moment_task_result</c>（创建任务结果查询）与 <c>get_moment_task</c>（企业发表列表）
    /// 是两条不同路由；朋友圈规则组路由（<c>moment_strategy/*</c>）与客户联系规则组
    /// （<c>customer_strategy/*</c>）互不重叠。
    /// </summary>
    [Fact]
    public void MomentEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(14,
            "本域 14 个端点（发表 3 + 数据 5 + 规则组 6）为三类应用公共面，全部收敛父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(14, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：公共端点必须落在父接口自身声明，不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方仅 get_moment_task_result 为 GET（jobid 走 Query），其余 13 条均为 POST。
        var jobidParam = typeof(IWechatWorkExternalContactMomentService)
            .GetMethod(nameof(IWechatWorkExternalContactMomentService.GetMomentTaskResultAsync))!
            .GetParameters()[0];
        jobidParam.GetCustomAttribute<QueryAttribute>()!
            .Name.Should().Be("jobid", "任务结果查询的 jobid 必须以官方契约的 Query 参数传入");
    }

    /// <summary>
    /// 契约守卫 MO2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建 / 第三方 / 代开发三个子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void MomentInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactMomentService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactMomentService),
            typeof(IWechatWorkThirdPartyExternalContactMomentService),
            typeof(IWechatWorkProviderExternalContactMomentService),
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
                $"（与既有的六个客户联系域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 MO1/MO2。
        foreach (var child in children)
        {
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 MO3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void MomentTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactMomentService),
            typeof(IWechatWorkInternalExternalContactMomentService),
            typeof(IWechatWorkThirdPartyExternalContactMomentService),
            typeof(IWechatWorkProviderExternalContactMomentService),
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
    /// 契约守卫 MO4：客户朋友圈域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void MomentDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = MomentJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(AddMomentTaskRequest), typeof(CancelMomentTaskRequest),
            typeof(GetMomentListRequest), typeof(GetMomentPublishListRequest),
            typeof(GetMomentCustomerListRequest), typeof(GetMomentSendResultRequest),
            typeof(GetMomentCommentsRequest),
            typeof(GetMomentStrategyListRequest), typeof(GetMomentStrategyDetailRequest),
            typeof(GetMomentStrategyRangeRequest), typeof(CreateMomentStrategyRequest),
            typeof(UpdateMomentStrategyRequest), typeof(DeleteMomentStrategyRequest),
            typeof(MomentVisibleRange), typeof(MomentSenderList), typeof(MomentExternalContactList),
            typeof(MomentText), typeof(MomentAttachment), typeof(MomentImageAttachment),
            typeof(MomentLinkAttachment), typeof(MomentVideoAttachment),
            typeof(MomentStrategyPrivilege), typeof(MomentStrategyRangeNode),
            typeof(AddMomentTaskResponse), typeof(GetMomentTaskResultResponse), typeof(MomentTaskResult),
            typeof(GetMomentListResponse), typeof(MomentRecord), typeof(MomentImageMedia),
            typeof(MomentVideoMedia), typeof(MomentLinkMedia), typeof(MomentLocation),
            typeof(GetMomentPublishListResponse), typeof(MomentPublishTaskItem),
            typeof(GetMomentCustomerListResponse), typeof(MomentCustomerItem),
            typeof(GetMomentSendResultResponse), typeof(MomentSendResultItem),
            typeof(GetMomentCommentsResponse), typeof(MomentCommentItem),
            typeof(GetMomentStrategyListResponse), typeof(MomentStrategyIdItem),
            typeof(GetMomentStrategyDetailResponse), typeof(MomentStrategy),
            typeof(GetMomentStrategyRangeResponse), typeof(CreateMomentStrategyResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是客户朋友圈域契约面类型，必须登记进 MomentJsonContext（AOT 源生成）");
        }
    }
}
