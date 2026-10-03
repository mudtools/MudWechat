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
using Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「消息推送（群发）」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 11 个端点（企业群发管理 3 条 + 群发记录与执行结果 3 条 +
/// 新客户欢迎语 1 条 + 入群欢迎语素材管理 4 条）收敛于父接口
/// <see cref="IWechatWorkExternalContactGroupMsgService"/>；
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记。
/// </para>
/// </remarks>
public class WechatExternalContactGroupMsgContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactGroupMsgService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 11 条公共端点，全部 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 企业群发管理（创建 92135/92698/96366、提醒 97610/97613/97618、停止 97611/97614/97619）。
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.AddGroupMsgTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/add_msg_template"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.RemindGroupMsgSendAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/remind_groupmsg_send"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.CancelGroupMsgSendAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/cancel_groupmsg_send"),
        // 群发记录与执行结果（93338/93439/96355）。
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.GetGroupMsgListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_groupmsg_list_v2"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.GetGroupMsgTaskListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_groupmsg_task"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.GetGroupMsgSendResultAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/get_groupmsg_send_result"),
        // 新客户欢迎语（92137/92599/96356）。
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.SendWelcomeMsgAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/send_welcome_msg"),
        // 入群欢迎语素材管理（92366/93438/96357）。
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.AddGroupWelcomeTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/group_welcome_template/add"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.UpdateGroupWelcomeTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/group_welcome_template/edit"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.GetGroupWelcomeTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/group_welcome_template/get"),
        (typeof(IWechatWorkExternalContactGroupMsgService),
            nameof(IWechatWorkExternalContactGroupMsgService.DeleteGroupWelcomeTemplateAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/group_welcome_template/del"),
    };

    /// <summary>
    /// 契约守卫 GM1：消息推送（群发）域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意群发记录列表为 <c>get_groupmsg_list_v2</c>（带版本后缀 v2，旧接口不带 _v2）；
    /// 群聊统计路由（<c>groupchat/statistic*</c>，统计管理域）与本域 <c>group_welcome_template/*</c> 互不重叠。
    /// </summary>
    [Fact]
    public void GroupMsgEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(11,
            "本域 11 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(11, "本域各端点路由互不重复");

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
    /// 契约守卫 GM2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建 / 第三方 / 代开发三个子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void GroupMsgInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactGroupMsgService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactGroupMsgService),
            typeof(IWechatWorkThirdPartyExternalContactGroupMsgService),
            typeof(IWechatWorkProviderExternalContactGroupMsgService),
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
                $"（与既有的九个客户联系域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 GM1/GM2。
        foreach (var child in children)
        {
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 GM3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void GroupMsgTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactGroupMsgService),
            typeof(IWechatWorkInternalExternalContactGroupMsgService),
            typeof(IWechatWorkThirdPartyExternalContactGroupMsgService),
            typeof(IWechatWorkProviderExternalContactGroupMsgService),
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
    /// 契约守卫 GM4：消息推送（群发）域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void GroupMsgDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = GroupMsgJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(AddGroupMsgTemplateRequest), typeof(RemindGroupMsgSendRequest),
            typeof(CancelGroupMsgSendRequest), typeof(GetGroupMsgListRequest),
            typeof(GetGroupMsgTaskListRequest), typeof(GetGroupMsgSendResultRequest),
            typeof(SendWelcomeMsgRequest), typeof(AddGroupWelcomeTemplateRequest),
            typeof(UpdateGroupWelcomeTemplateRequest), typeof(GetGroupWelcomeTemplateRequest),
            typeof(DeleteGroupWelcomeTemplateRequest),
            typeof(GroupMsgTextContent), typeof(GroupMsgAttachment), typeof(GroupMsgImageAttachment),
            typeof(GroupMsgLinkAttachment), typeof(GroupMsgMiniProgramAttachment),
            typeof(GroupMsgVideoAttachment), typeof(GroupMsgFileAttachment),
            typeof(GroupMsgTagFilter), typeof(GroupMsgTagGroup),
            typeof(AddGroupMsgTemplateResponse), typeof(GetGroupMsgListResponse), typeof(GroupMsgRecord),
            typeof(GetGroupMsgTaskListResponse), typeof(GroupMsgTaskItem),
            typeof(GetGroupMsgSendResultResponse), typeof(GroupMsgSendResultItem),
            typeof(AddGroupWelcomeTemplateResponse), typeof(GetGroupWelcomeTemplateResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是消息推送（群发）域契约面类型，必须登记进 GroupMsgJsonContext（AOT 源生成）");
        }
    }
}
