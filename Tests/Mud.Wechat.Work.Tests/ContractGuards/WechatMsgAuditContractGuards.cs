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
using Mud.Wechat.Work.DataModels.MsgAudit;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 会话内容存档模块（MsgAudit 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （开启成员列表域 + 机器人信息域 + 会话同意情况域 + 内部群信息域，官方仅自建应用开放）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：四个域均为零端点父接口 + 仅自建子接口承载端点（形态对齐企业支付域守卫）；
/// 官方「获取会话内容」页（91774）主体为原生 C SDK（GetChatData / DecryptData / GetMediaData），
/// 不属于 HTTP 端点面，本模块仅承载该页面的 get_robot_info。
/// </para>
/// </remarks>
public class WechatMsgAuditContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string PermitUserParentImplementationClassName = "WechatWorkMsgAuditPermitUserService";

    private const string RobotParentImplementationClassName = "WechatWorkMsgAuditRobotService";

    private const string AgreeParentImplementationClassName = "WechatWorkMsgAuditAgreeService";

    private const string GroupChatParentImplementationClassName = "WechatWorkMsgAuditGroupChatService";

    private const string MsgAuditRegistryGroupName = "MsgAudit";

    /// <summary>
    /// 开启成员列表域官方路由表（仅自建开放，1 条端点声明于自建子接口，POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] PermitUserRoutes =
    {
        // 获取会话内容存档开启成员列表（91614）。
        (typeof(IWechatWorkInternalMsgAuditPermitUserService),
            nameof(IWechatWorkInternalMsgAuditPermitUserService.GetPermitUserListAsync),
            typeof(PostAttribute), "/cgi-bin/msgaudit/get_permit_user_list"),
    };

    /// <summary>
    /// 机器人信息域官方路由表（仅自建开放，1 条端点声明于自建子接口；
    /// 官方即 GET，robot_id 以 Query 参数传递）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] RobotRoutes =
    {
        // 获取机器人信息（91774「获取会话内容」页内的唯一 HTTP API）。
        (typeof(IWechatWorkInternalMsgAuditRobotService),
            nameof(IWechatWorkInternalMsgAuditRobotService.GetRobotInfoAsync),
            typeof(GetAttribute), "/cgi-bin/msgaudit/get_robot_info"),
    };

    /// <summary>
    /// 会话同意情况域官方路由表（仅自建开放，2 条端点全部声明于自建子接口，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AgreeRoutes =
    {
        // 获取单聊会话同意情况（91782；≤2500 次/分钟，一次最多 100 条目）。
        (typeof(IWechatWorkInternalMsgAuditAgreeService),
            nameof(IWechatWorkInternalMsgAuditAgreeService.CheckSingleAgreeAsync),
            typeof(PostAttribute), "/cgi-bin/msgaudit/check_single_agree"),
        // 获取群聊会话同意情况（91782；≤1500 次/分钟）。
        (typeof(IWechatWorkInternalMsgAuditAgreeService),
            nameof(IWechatWorkInternalMsgAuditAgreeService.CheckRoomAgreeAsync),
            typeof(PostAttribute), "/cgi-bin/msgaudit/check_room_agree"),
    };

    /// <summary>
    /// 内部群信息域官方路由表（仅自建开放，1 条端点声明于自建子接口，POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] GroupChatRoutes =
    {
        // 获取会话内容存档内部群信息（92951；官方路由为 groupchat/get，仅支持内部群）。
        (typeof(IWechatWorkInternalMsgAuditGroupChatService),
            nameof(IWechatWorkInternalMsgAuditGroupChatService.GetGroupChatInfoAsync),
            typeof(PostAttribute), "/cgi-bin/msgaudit/groupchat/get"),
    };

    /// <summary>
    /// 契约守卫 MA1a：开启成员列表域端点路由必须与官方契约一致（官方仅自建开放）。
    /// </summary>
    [Fact]
    public void MsgAuditPermitUserEndpoints_ShouldMatchOfficialRoutes()
    {
        PermitUserRoutes.Should().HaveCount(1,
            "开启成员列表域 1 个端点官方仅自建应用开放，声明于自建子接口");

        foreach (var (iface, method, httpAttribute, route) in PermitUserRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 MA1b：机器人信息域端点路由必须与官方契约一致
    /// （官方即 GET，robot_id 走 Query 参数，勿改成 POST/Body）。
    /// </summary>
    [Fact]
    public void MsgAuditRobotEndpoints_ShouldMatchOfficialRoutes()
    {
        RobotRoutes.Should().HaveCount(1,
            "机器人信息域 1 个端点官方仅自建应用开放，声明于自建子接口");

        foreach (var (iface, method, httpAttribute, route) in RobotRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // get_robot_info 的 robot_id 必须以 [Query("robot_id")] 显式 Query 参数传递（官方契约）。
        var methodInfo = typeof(IWechatWorkInternalMsgAuditRobotService).GetMethod(
            nameof(IWechatWorkInternalMsgAuditRobotService.GetRobotInfoAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        methodInfo.Should().NotBeNull();
        methodInfo!.GetParameters()
            .Any(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "robot_id")
            .Should().BeTrue("get_robot_info 的 robot_id 为官方 Query 参数，必须以 [Query(\"robot_id\")] 标注");
    }

    /// <summary>
    /// 契约守卫 MA1c：会话同意情况域端点路由必须与官方契约一致
    /// （官方仅自建开放；单聊/群聊两路由不同，勿合并；官方多数查询接口即 POST，勿改成 GET）。
    /// </summary>
    [Fact]
    public void MsgAuditAgreeEndpoints_ShouldMatchOfficialRoutes()
    {
        AgreeRoutes.Should().HaveCount(2,
            "会话同意情况域 2 个端点（单聊 + 群聊）官方仅自建应用开放，全部声明于自建子接口");

        var distinctRoutes = AgreeRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "会话同意情况域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in AgreeRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方反直觉拼写：单聊请求与响应的外部成员标识字段官方原文为 exteranalopenid，照抄勿改。
        var requestItem = typeof(MsgAuditAgreeQueryItem).GetProperty(nameof(MsgAuditAgreeQueryItem.ExteranalOpenid));
        requestItem.Should().NotBeNull();
        requestItem!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("exteranalopenid", "官方原文即此拼写（external 的官方笔误），照抄官方原文");

        var agreeInfo = typeof(MsgAuditAgreeInfo).GetProperty(nameof(MsgAuditAgreeInfo.ExteranalOpenid));
        agreeInfo.Should().NotBeNull();
        agreeInfo!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("exteranalopenid", "官方原文即此拼写（external 的官方笔误），照抄官方原文");
    }

    /// <summary>
    /// 契约守卫 MA1d：内部群信息域端点路由必须与官方契约一致
    /// （官方仅自建开放；路由为 groupchat/get，仅支持内部群）。
    /// </summary>
    [Fact]
    public void MsgAuditGroupChatEndpoints_ShouldMatchOfficialRoutes()
    {
        GroupChatRoutes.Should().HaveCount(1,
            "内部群信息域 1 个端点官方仅自建应用开放，声明于自建子接口");

        foreach (var (iface, method, httpAttribute, route) in GroupChatRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 MA2：仅单类应用开放域的接口层级——四个域官方仅自建应用开放，
    /// 均为零端点父接口 + 仅自建子接口承载端点，
    /// 且继承链上不得出现第三方 / 代开发子接口（形态对齐企业支付域守卫 PAY2b）。
    /// </summary>
    [Fact]
    public void MsgAuditSingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var singleChildFamilies = new[]
        {
            (typeof(IWechatWorkMsgAuditPermitUserService),
                new[] { typeof(IWechatWorkInternalMsgAuditPermitUserService) },
                "开启成员列表域官方仅向自建应用开放（代开发/第三方暂不支持）"),
            (typeof(IWechatWorkMsgAuditRobotService),
                new[] { typeof(IWechatWorkInternalMsgAuditRobotService) },
                "机器人信息域官方仅向自建应用开放（代开发/第三方暂不支持）"),
            (typeof(IWechatWorkMsgAuditAgreeService),
                new[] { typeof(IWechatWorkInternalMsgAuditAgreeService) },
                "会话同意情况域官方仅向自建应用开放（代开发/第三方暂不支持）"),
            (typeof(IWechatWorkMsgAuditGroupChatService),
                new[] { typeof(IWechatWorkInternalMsgAuditGroupChatService) },
                "内部群信息域官方仅向自建应用开放（代开发/第三方暂不支持）"),
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
                childApi!.RegistryGroupName.Should().Be(MsgAuditRegistryGroupName,
                    $"{child.Name} 必须挂 {MsgAuditRegistryGroupName} 注册组");
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().NotBeEmpty($"{child.Name} 为唯一端点承载接口：{because}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 MA3：令牌绑定——MsgAudit 模块全部 8 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；且必须由「会话内容存档」应用 secret 获取，宿主侧配置约束）。
    /// </summary>
    [Fact]
    public void MsgAuditTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkMsgAuditPermitUserService),
            typeof(IWechatWorkInternalMsgAuditPermitUserService),
            typeof(IWechatWorkMsgAuditRobotService),
            typeof(IWechatWorkInternalMsgAuditRobotService),
            typeof(IWechatWorkMsgAuditAgreeService),
            typeof(IWechatWorkInternalMsgAuditAgreeService),
            typeof(IWechatWorkMsgAuditGroupChatService),
            typeof(IWechatWorkInternalMsgAuditGroupChatService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（会话存档应用 secret 换发的 access_token）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 MA4：会话内容存档模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 12 个契约面类型）。
    /// </summary>
    [Fact]
    public void MsgAuditDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = MsgAuditJsonContext.Default;

        var requiredTypes = new[]
        {
            // 开启成员列表域。
            typeof(GetMsgAuditPermitUserListRequest), typeof(GetMsgAuditPermitUserListResponse),
            // 机器人信息域。
            typeof(GetMsgAuditRobotInfoResponse), typeof(MsgAuditRobotInfo),
            // 会话同意情况域（单聊 + 群聊共用响应）。
            typeof(CheckMsgAuditSingleAgreeRequest), typeof(MsgAuditAgreeQueryItem),
            typeof(CheckMsgAuditRoomAgreeRequest),
            typeof(CheckMsgAuditAgreeResponse), typeof(MsgAuditAgreeInfo),
            // 内部群信息域。
            typeof(GetMsgAuditGroupChatRequest), typeof(GetMsgAuditGroupChatResponse),
            typeof(MsgAuditGroupChatMember),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是会话内容存档模块契约面类型，必须登记进 MsgAuditJsonContext（AOT 源生成）");
        }
    }
}
