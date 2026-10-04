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
using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 邮件模块（Mail 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定
/// （发送邮件族：三类应用公共面收敛父接口 + 空标记子接口，普通/日程/会议三端点共用 compose_send 路由；
/// 获取接收的邮件族：三类应用公共面收敛父接口 + 空标记子接口；
/// 管理应用邮箱账号族：官方仅自建应用开放，零端点父接口 + 仅自建子接口承载）。
/// </summary>
public class WechatMailContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string MailSendParentImplementationClassName = "WechatWorkMailSendService";

    private const string MailReceiveParentImplementationClassName = "WechatWorkMailReceiveService";

    private const string MailAccountParentImplementationClassName = "WechatWorkMailAccountService";

    private const string MailRegistryGroupName = "Mail";

    /// <summary>
    /// 发送邮件族官方路由表（父接口 3 条端点；官方普通/日程/会议邮件共用
    /// <c>/cgi-bin/exmail/app/compose_send</c> 路由，以请求包体差异区分，勿拆分路由）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailSendRoutes =
    {
        // 发送普通邮件（自建 97445、第三方 97515、代开发 97504）。
        (typeof(IWechatWorkMailSendService),
            nameof(IWechatWorkMailSendService.SendNormalMailAsync), "/cgi-bin/exmail/app/compose_send"),
        // 发送日程邮件（自建 97854、第三方 97867、代开发 97865）：schedule 官方必填。
        (typeof(IWechatWorkMailSendService),
            nameof(IWechatWorkMailSendService.SendScheduleMailAsync), "/cgi-bin/exmail/app/compose_send"),
        // 发送会议邮件（自建 97855、第三方 97868、代开发 97866）：schedule + meeting 官方必填。
        (typeof(IWechatWorkMailSendService),
            nameof(IWechatWorkMailSendService.SendMeetingMailAsync), "/cgi-bin/exmail/app/compose_send"),
    };

    /// <summary>
    /// 获取接收的邮件族官方路由表（父接口 2 条端点，官方即 POST，勿改 GET）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailReceiveRoutes =
    {
        // 获取收件箱邮件列表（自建 97369、第三方 97516、代开发 97505）：limit 默认 100、最大 1000。
        (typeof(IWechatWorkMailReceiveService),
            nameof(IWechatWorkMailReceiveService.GetMailListAsync), "/cgi-bin/exmail/app/get_mail_list"),
        // 获取邮件内容（自建 97979、第三方 97983、代开发 97982）：返回邮件 eml 数据。
        (typeof(IWechatWorkMailReceiveService),
            nameof(IWechatWorkMailReceiveService.ReadMailAsync), "/cgi-bin/exmail/app/read_mail"),
    };

    /// <summary>
    /// 管理应用邮箱账号族官方路由表（官方仅自建应用开放，2 条端点全部承载于自建子接口；
    /// 第三方/代开发官方未开放本域，不得声明对应子接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailAccountRoutes =
    {
        // 更新应用邮箱账号（自建 97373）：更新后原账号保留为别名邮箱且仍可收信。
        (typeof(IWechatWorkInternalMailAccountService),
            nameof(IWechatWorkInternalMailAccountService.UpdateEmailAliasAsync), "/cgi-bin/exmail/app/update_email_alias"),
        // 查询应用邮箱账号（自建 97991）：官方不需要请求包体，方法无 [Body] 参数。
        (typeof(IWechatWorkInternalMailAccountService),
            nameof(IWechatWorkInternalMailAccountService.GetEmailAliasAsync), "/cgi-bin/exmail/app/get_email_alias"),
    };

    /// <summary>
    /// 契约守卫 M1：发送邮件族全部端点路由必须与官方契约一致——三条端点共用 compose_send 路由
    /// （同路由多方法形态对齐 Message 域 message/send，新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void MailSendEndpoints_ShouldMatchOfficialRoutes()
    {
        MailSendRoutes.Should().HaveCount(3,
            "发送邮件族 3 个端点（普通/日程/会议）为三类应用公共面，全部收敛父接口");

        AssertRoutes(MailSendRoutes);

        // 三端点官方共用同一路由，以请求包体差异区分（不做运行时多态，路由不得拆分）。
        MailSendRoutes.Select(r => r.Route).Distinct().Should().HaveCount(1,
            "官方普通/日程/会议邮件共用 compose_send 路由，须与官方契约一致");
    }

    /// <summary>
    /// 契约守卫 M1b：获取接收的邮件族全部端点路由必须与官方契约一致。
    /// </summary>
    [Fact]
    public void MailReceiveEndpoints_ShouldMatchOfficialRoutes()
    {
        MailReceiveRoutes.Should().HaveCount(2,
            "获取接收的邮件族 2 个端点为三类应用公共面，全部收敛父接口");
        MailReceiveRoutes.Select(r => r.Route).Distinct().Should().HaveCount(2, "各端点路由互不重复");

        AssertRoutes(MailReceiveRoutes);
    }

    /// <summary>
    /// 契约守卫 M1c：管理应用邮箱账号族全部端点路由必须与官方契约一致，且开放面与官方一致——
    /// 官方仅自建应用开放本域；查询应用邮箱账号官方不需要请求包体，不得引入 [Body] 参数。
    /// </summary>
    [Fact]
    public void MailAccountEndpoints_ShouldMatchOfficialOpenSurface()
    {
        MailAccountRoutes.Should().HaveCount(2, "管理应用邮箱账号族 2 个端点官方仅自建应用开放");
        AssertRoutes(MailAccountRoutes);

        // 查询应用邮箱账号：官方「不需要请求包体」，方法仅可带 CancellationToken。
        var query = typeof(IWechatWorkInternalMailAccountService).GetMethod(
            nameof(IWechatWorkInternalMailAccountService.GetEmailAliasAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        query.Should().NotBeNull();
        query!.GetParameters().Should().ContainSingle("get_email_alias 官方不需要请求包体，不得引入 [Body] 参数")
            .Which.ParameterType.Should().Be(typeof(CancellationToken));
    }

    /// <summary>
    /// 契约守卫 M2：邮件模块接口层级与生成器注册形态——发送/接收族公共端点收敛于 IsAbstract 父接口、
    /// 三个应用类型子接口均为空标记；管理应用邮箱账号族父接口零端点、仅自建子接口承载全部端点
    /// （第三方/代开发官方未开放本域，继承链上恰好只有自建子接口，能力漂移守卫）。
    /// </summary>
    [Fact]
    public void MailInterfaceHierarchy_ShouldConvergeOnAbstractParentWithMailRegistry()
    {
        // —— 发送邮件族 ——
        var sendChildren = new[]
        {
            typeof(IWechatWorkInternalMailSendService),
            typeof(IWechatWorkThirdPartyMailSendService),
            typeof(IWechatWorkProviderMailSendService),
        };

        foreach (var child in sendChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkMailSendService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkMailSendService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailSendService),
            MailSendParentImplementationClassName,
            expectedDeclaredMethods: 3,
            new[] { (typeof(IWechatWorkInternalMailSendService), 0),
                    (typeof(IWechatWorkThirdPartyMailSendService), 0),
                    (typeof(IWechatWorkProviderMailSendService), 0) });

        // —— 获取接收的邮件族 ——
        var receiveChildren = new[]
        {
            typeof(IWechatWorkInternalMailReceiveService),
            typeof(IWechatWorkThirdPartyMailReceiveService),
            typeof(IWechatWorkProviderMailReceiveService),
        };

        foreach (var child in receiveChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkMailReceiveService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkMailReceiveService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailReceiveService),
            MailReceiveParentImplementationClassName,
            expectedDeclaredMethods: 2,
            new[] { (typeof(IWechatWorkInternalMailReceiveService), 0),
                    (typeof(IWechatWorkThirdPartyMailReceiveService), 0),
                    (typeof(IWechatWorkProviderMailReceiveService), 0) });

        // —— 管理应用邮箱账号族（仅自建） ——
        typeof(IWechatWorkInternalMailAccountService).Should().BeAssignableTo(typeof(IWechatWorkMailAccountService),
            "自建子接口必须继承公共父接口 IWechatWorkMailAccountService");

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailAccountService),
            MailAccountParentImplementationClassName,
            expectedDeclaredMethods: 0,
            new[] { (typeof(IWechatWorkInternalMailAccountService), 2) });
    }

    /// <summary>
    /// 契约守卫 M3：令牌绑定——邮件模块全部 10 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；第三方/代开发消费授权企业级令牌，scope = authCorpId）。
    /// </summary>
    [Fact]
    public void MailTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkMailSendService),
            typeof(IWechatWorkInternalMailSendService),
            typeof(IWechatWorkThirdPartyMailSendService),
            typeof(IWechatWorkProviderMailSendService),
            typeof(IWechatWorkMailReceiveService),
            typeof(IWechatWorkInternalMailReceiveService),
            typeof(IWechatWorkThirdPartyMailReceiveService),
            typeof(IWechatWorkProviderMailReceiveService),
            typeof(IWechatWorkMailAccountService),
            typeof(IWechatWorkInternalMailAccountService),
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
    /// 契约守卫 M4：邮件模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 16 个契约面类型）。
    /// </summary>
    [Fact]
    public void MailDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = MailJsonContext.Default;

        var requiredTypes = new Type[]
        {
            // 发送邮件族（普通/日程/会议三种邮件请求；公共字段收敛基类）。
            typeof(MailSendRequest), typeof(MailSendScheduleRequest), typeof(MailSendMeetingRequest),
            typeof(MailRecipient), typeof(MailAttachment), typeof(MailSchedule),
            typeof(MailReminders), typeof(MailMeeting), typeof(MailMeetingOption),
            // 获取接收的邮件族。
            typeof(GetMailListRequest), typeof(GetMailListResponse), typeof(MailListItem),
            typeof(ReadMailRequest), typeof(ReadMailResponse),
            // 管理应用邮箱账号族。
            typeof(UpdateEmailAliasRequest), typeof(GetEmailAliasResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是邮件模块契约面类型，必须登记进 MailJsonContext（AOT 源生成）");
        }
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, string Route)[] routes)
    {
        foreach (var (iface, method, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<HttpMethodAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 HTTP 方法特性");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 接口层级断言：父接口 IsAbstract 且不进注册组；子接口挂 Mail 注册组、继承父实现类；
    /// 各子接口的自身声明端点数必须与官方开放面一致。
    /// </summary>
    private static void AssertFamilyHierarchy(
        Type parent,
        string parentImplementationClassName,
        int? expectedDeclaredMethods,
        (Type Child, int DeclaredMethods)[] children)
    {
        if (expectedDeclaredMethods.HasValue)
        {
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(expectedDeclaredMethods.Value, $"{parent.Name} 公共端点数漂移");
        }

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var (child, declaredMethods) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(MailRegistryGroupName,
                $"{child.Name} 必须挂 {MailRegistryGroupName} 注册组" +
                $"（Mail 模块共用 Add{MailRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(declaredMethods,
                    $"{child.Name} 自身声明端点数必须与官方开放面一致（能力漂移须先核对官方文档再同批调整守卫）");
        }
    }
}
