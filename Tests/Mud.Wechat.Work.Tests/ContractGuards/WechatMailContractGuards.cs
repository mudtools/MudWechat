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
/// （应用邮箱侧：发送邮件族与获取接收的邮件族为三类应用公共面收敛父接口 + 空标记子接口，
/// 普通/日程/会议三端点共用 compose_send 路由，管理应用邮箱账号族官方仅自建开放；
/// 管理端侧：管理邮件群组 / 管理公共邮箱 / 高级功能账号 / 成员邮箱操作 / 其他邮件客户端登录设置五族
/// 官方均仅自建应用开放，零端点父接口 + 仅自建子接口承载）。
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

    private const string MailGroupParentImplementationClassName = "WechatWorkMailGroupService";

    private const string MailPublicMailParentImplementationClassName = "WechatWorkMailPublicMailService";

    private const string MailVipParentImplementationClassName = "WechatWorkMailVipService";

    private const string MailUserParentImplementationClassName = "WechatWorkMailUserService";

    private const string MailUserOptionParentImplementationClassName = "WechatWorkMailUserOptionService";

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
    /// 管理邮件群组族官方路由表（官方仅自建应用开放，5 条端点全部承载于自建子接口；
    /// 获取详情与模糊搜索官方即 GET、参数走 Query，勿改 POST 包体）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailGroupRoutes =
    {
        // 创建邮件群组（自建 95510）：成员列表至少一项非空。
        (typeof(IWechatWorkInternalMailGroupService),
            nameof(IWechatWorkInternalMailGroupService.CreateMailGroupAsync), "/cgi-bin/exmail/group/create"),
        // 更新邮件群组（自建 97995）：列表字段不传不变、传空清空，成员不允许全部清空。
        (typeof(IWechatWorkInternalMailGroupService),
            nameof(IWechatWorkInternalMailGroupService.UpdateMailGroupAsync), "/cgi-bin/exmail/group/update"),
        // 删除邮件群组（自建 97996）。
        (typeof(IWechatWorkInternalMailGroupService),
            nameof(IWechatWorkInternalMailGroupService.DeleteMailGroupAsync), "/cgi-bin/exmail/group/delete"),
        // 获取邮件群组详情（自建 97997）：官方即 GET，groupid 走 Query。
        (typeof(IWechatWorkInternalMailGroupService),
            nameof(IWechatWorkInternalMailGroupService.GetMailGroupAsync), "/cgi-bin/exmail/group/get"),
        // 模糊搜索邮件群组（自建 97998）：官方即 GET，fuzzy/groupid 走 Query。
        (typeof(IWechatWorkInternalMailGroupService),
            nameof(IWechatWorkInternalMailGroupService.SearchMailGroupAsync), "/cgi-bin/exmail/group/search"),
    };

    /// <summary>
    /// 管理公共邮箱族官方路由表（官方仅自建应用开放，7 条端点全部承载于自建子接口；
    /// 模糊搜索官方即 GET、参数走 Query；获取详情官方即 POST 且以 id_list 平铺数组传参）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailPublicMailRoutes =
    {
        // 创建公共邮箱（自建 95511）：可选创建客户端专用密码，密码仅返回一次。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.CreatePublicMailAsync), "/cgi-bin/exmail/publicmail/create"),
        // 更新公共邮箱（自建 98000）：别名覆盖式更新，API 创建的密码上限 10 个。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.UpdatePublicMailAsync), "/cgi-bin/exmail/publicmail/update"),
        // 删除公共邮箱（自建 98001）。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.DeletePublicMailAsync), "/cgi-bin/exmail/publicmail/delete"),
        // 获取公共邮箱详情（自建 98002）：官方即 POST，id_list 平铺数组。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.GetPublicMailAsync), "/cgi-bin/exmail/publicmail/get"),
        // 模糊搜索公共邮箱（自建 98003）：官方即 GET，fuzzy/email 走 Query。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.SearchPublicMailAsync), "/cgi-bin/exmail/publicmail/search"),
        // 获取客户端专用密码列表（自建 100183）：不返回密码本身。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.GetPublicMailAuthCodeListAsync), "/cgi-bin/exmail/publicmail/get_auth_code_list"),
        // 删除客户端专用密码（自建 100184）。
        (typeof(IWechatWorkInternalMailPublicMailService),
            nameof(IWechatWorkInternalMailPublicMailService.DeletePublicMailAuthCodeAsync), "/cgi-bin/exmail/publicmail/delete_auth_code"),
    };

    /// <summary>
    /// 高级功能账号族官方路由表（官方仅自建应用开放，3 条端点全部承载于自建子接口；
    /// 同步批量形态，区别于安全管理域 /cgi-bin/security/vip/* 异步任务形态，勿混用路由）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailVipRoutes =
    {
        // 分配高级功能账号（自建 99316）：单次最多 100 个。
        (typeof(IWechatWorkInternalMailVipService),
            nameof(IWechatWorkInternalMailVipService.BatchAddVipAsync), "/cgi-bin/exmail/vip/batch_add"),
        // 取消高级功能账号（自建 99317）：单次最多 100 个。
        (typeof(IWechatWorkInternalMailVipService),
            nameof(IWechatWorkInternalMailVipService.BatchDelVipAsync), "/cgi-bin/exmail/vip/batch_del"),
        // 获取高级功能账号列表（自建 99318）：cursor + has_more 翻页，limit 默认 100 最大 200。
        (typeof(IWechatWorkInternalMailVipService),
            nameof(IWechatWorkInternalMailVipService.ListVipAsync), "/cgi-bin/exmail/vip/list"),
    };

    /// <summary>
    /// 成员邮箱操作族官方路由表（官方仅自建应用开放，2 条端点全部承载于自建子接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailUserRoutes =
    {
        // 禁用/启用邮箱账号（自建 95512）：userid 与 publicemail_id 至少传一项，不可禁用超管与企业创建人。
        (typeof(IWechatWorkInternalMailUserService),
            nameof(IWechatWorkInternalMailUserService.SetEmailAccountStatusAsync), "/cgi-bin/exmail/account/act_email"),
        // 获取邮件未读数（自建 95514）。
        (typeof(IWechatWorkInternalMailUserService),
            nameof(IWechatWorkInternalMailUserService.GetMailUnreadCountAsync), "/cgi-bin/exmail/mail/get_newcount"),
    };

    /// <summary>
    /// 其他邮件客户端登录设置族官方路由表（官方仅自建应用开放，2 条端点全部承载于自建子接口；
    /// 官方即 POST，功能属性以 option.list 结构承载，勿改 GET）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] MailUserOptionRoutes =
    {
        // 获取用户功能属性（自建 95513）。
        (typeof(IWechatWorkInternalMailUserOptionService),
            nameof(IWechatWorkInternalMailUserOptionService.GetUserOptionAsync), "/cgi-bin/exmail/useroption/get"),
        // 更改用户功能属性（自建 98008）。
        (typeof(IWechatWorkInternalMailUserOptionService),
            nameof(IWechatWorkInternalMailUserOptionService.UpdateUserOptionAsync), "/cgi-bin/exmail/useroption/update"),
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
    /// 契约守卫 M5：管理邮件群组族全部端点路由必须与官方契约一致，且开放面与官方一致——
    /// 官方仅自建应用开放本域；获取详情与模糊搜索官方即 GET、参数走 Query，不得改 POST 包体。
    /// </summary>
    [Fact]
    public void MailGroupEndpoints_ShouldMatchOfficialOpenSurface()
    {
        MailGroupRoutes.Should().HaveCount(5, "管理邮件群组族 5 个端点官方仅自建应用开放");
        AssertRoutes(MailGroupRoutes);

        // 获取详情：官方即 GET，groupid 走 Query。
        var get = typeof(IWechatWorkInternalMailGroupService).GetMethod(
            nameof(IWechatWorkInternalMailGroupService.GetMailGroupAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        get.Should().NotBeNull();
        get!.GetCustomAttribute<HttpMethodAttribute>()!.RequestUri.Should().Be("/cgi-bin/exmail/group/get");
        get.GetParameters().Any(p =>
                p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "groupid")
            .Should().BeTrue("group/get 的 groupid 必须以 [Query(\"groupid\")] 显式承载");

        // 模糊搜索：官方即 GET，fuzzy（必填）/ groupid（选填）走 Query。
        var search = typeof(IWechatWorkInternalMailGroupService).GetMethod(
            nameof(IWechatWorkInternalMailGroupService.SearchMailGroupAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        search.Should().NotBeNull();
        search!.GetCustomAttribute<HttpMethodAttribute>()!.RequestUri.Should().Be("/cgi-bin/exmail/group/search");
        search.GetParameters().Any(p =>
                p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "fuzzy")
            .Should().BeTrue("group/search 的 fuzzy 必须以 [Query(\"fuzzy\")] 显式承载");
        search.GetParameters().Any(p =>
                p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "groupid")
            .Should().BeTrue("group/search 的 groupid 必须以 [Query(\"groupid\")] 显式承载");
    }

    /// <summary>
    /// 契约守卫 M6：管理公共邮箱族全部端点路由必须与官方契约一致，且开放面与官方一致——
    /// 官方仅自建应用开放本域；模糊搜索官方即 GET、参数走 Query，不得改 POST 包体。
    /// </summary>
    [Fact]
    public void MailPublicMailEndpoints_ShouldMatchOfficialOpenSurface()
    {
        MailPublicMailRoutes.Should().HaveCount(7, "管理公共邮箱族 7 个端点官方仅自建应用开放");
        AssertRoutes(MailPublicMailRoutes);

        // 模糊搜索：官方即 GET，fuzzy（必填）/ email（选填）走 Query。
        var search = typeof(IWechatWorkInternalMailPublicMailService).GetMethod(
            nameof(IWechatWorkInternalMailPublicMailService.SearchPublicMailAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        search.Should().NotBeNull();
        search!.GetCustomAttribute<HttpMethodAttribute>()!.RequestUri.Should().Be("/cgi-bin/exmail/publicmail/search");
        search.GetParameters().Any(p =>
                p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "fuzzy")
            .Should().BeTrue("publicmail/search 的 fuzzy 必须以 [Query(\"fuzzy\")] 显式承载");
        search.GetParameters().Any(p =>
                p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "email")
            .Should().BeTrue("publicmail/search 的 email 必须以 [Query(\"email\")] 显式承载");
    }

    /// <summary>
    /// 契约守卫 M7：高级功能账号族全部端点路由必须与官方契约一致，且开放面与官方一致——
    /// 官方仅自建应用开放本域，同步批量形态区别于安全管理域 /cgi-bin/security/vip/* 异步任务形态。
    /// </summary>
    [Fact]
    public void MailVipEndpoints_ShouldMatchOfficialRoutes()
    {
        MailVipRoutes.Should().HaveCount(3, "高级功能账号族 3 个端点官方仅自建应用开放");
        MailVipRoutes.Select(r => r.Route).Should().OnlyContain(r => r.StartsWith("/cgi-bin/exmail/vip/"),
            "高级功能账号族路由必须落在 exmail/vip/ 下，不得与安全管理域 security/vip 混用");

        AssertRoutes(MailVipRoutes);
    }

    /// <summary>
    /// 契约守卫 M8：成员邮箱操作族全部端点路由必须与官方契约一致，且开放面与官方一致——
    /// 官方仅自建应用开放本域。
    /// </summary>
    [Fact]
    public void MailUserEndpoints_ShouldMatchOfficialRoutes()
    {
        MailUserRoutes.Should().HaveCount(2, "成员邮箱操作族 2 个端点官方仅自建应用开放");
        AssertRoutes(MailUserRoutes);
    }

    /// <summary>
    /// 契约守卫 M9：其他邮件客户端登录设置族全部端点路由必须与官方契约一致，且开放面与官方一致——
    /// 官方仅自建应用开放本域，官方即 POST，不得改 GET。
    /// </summary>
    [Fact]
    public void MailUserOptionEndpoints_ShouldMatchOfficialRoutes()
    {
        MailUserOptionRoutes.Should().HaveCount(2, "其他邮件客户端登录设置族 2 个端点官方仅自建应用开放");
        AssertRoutes(MailUserOptionRoutes);
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
    /// 契约守卫 M2b：邮件模块管理端 5 族（管理邮件群组 / 管理公共邮箱 / 高级功能账号 / 成员邮箱操作 /
    /// 其他邮件客户端登录设置）接口层级与生成器注册形态——官方均仅向自建应用开放：
    /// 零端点 IsAbstract 父接口 + 仅自建子接口承载全部端点，且不得声明第三方/代开发子接口（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void MailAdminInterfaceHierarchy_ShouldCarryEndpointsOnInternalChildOnly()
    {
        AssertFamilyHierarchy(
            typeof(IWechatWorkMailGroupService),
            MailGroupParentImplementationClassName,
            expectedDeclaredMethods: 0,
            new[] { (typeof(IWechatWorkInternalMailGroupService), 5) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailPublicMailService),
            MailPublicMailParentImplementationClassName,
            expectedDeclaredMethods: 0,
            new[] { (typeof(IWechatWorkInternalMailPublicMailService), 7) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailVipService),
            MailVipParentImplementationClassName,
            expectedDeclaredMethods: 0,
            new[] { (typeof(IWechatWorkInternalMailVipService), 3) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailUserService),
            MailUserParentImplementationClassName,
            expectedDeclaredMethods: 0,
            new[] { (typeof(IWechatWorkInternalMailUserService), 2) });

        AssertFamilyHierarchy(
            typeof(IWechatWorkMailUserOptionService),
            MailUserOptionParentImplementationClassName,
            expectedDeclaredMethods: 0,
            new[] { (typeof(IWechatWorkInternalMailUserOptionService), 2) });
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
            typeof(IWechatWorkMailGroupService),
            typeof(IWechatWorkInternalMailGroupService),
            typeof(IWechatWorkMailPublicMailService),
            typeof(IWechatWorkInternalMailPublicMailService),
            typeof(IWechatWorkMailVipService),
            typeof(IWechatWorkInternalMailVipService),
            typeof(IWechatWorkMailUserService),
            typeof(IWechatWorkInternalMailUserService),
            typeof(IWechatWorkMailUserOptionService),
            typeof(IWechatWorkInternalMailUserOptionService),
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
    /// 契约守卫 M4：邮件模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 53 个契约面类型）。
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
            // 管理邮件群组族。
            typeof(CreateMailGroupRequest), typeof(UpdateMailGroupRequest), typeof(DeleteMailGroupRequest),
            typeof(GetMailGroupResponse), typeof(SearchMailGroupResponse), typeof(MailGroupBrief),
            // 管理公共邮箱族。
            typeof(CreatePublicMailRequest), typeof(CreatePublicMailResponse),
            typeof(UpdatePublicMailRequest), typeof(UpdatePublicMailResponse),
            typeof(DeletePublicMailRequest), typeof(GetPublicMailRequest), typeof(GetPublicMailResponse),
            typeof(SearchPublicMailResponse), typeof(PublicMailInfo), typeof(PublicMailBrief),
            typeof(GetPublicMailAuthCodeListRequest), typeof(GetPublicMailAuthCodeListResponse),
            typeof(PublicMailAuthCode), typeof(DeletePublicMailAuthCodeRequest), typeof(PublicMailAuthCodeInfo),
            // 高级功能账号族。
            typeof(BatchAddMailVipRequest), typeof(BatchAddMailVipResponse),
            typeof(BatchDelMailVipRequest), typeof(BatchDelMailVipResponse),
            typeof(ListMailVipRequest), typeof(ListMailVipResponse),
            // 成员邮箱操作族。
            typeof(SetEmailAccountStatusRequest),
            typeof(GetMailUnreadCountRequest), typeof(GetMailUnreadCountResponse),
            // 其他邮件客户端登录设置族。
            typeof(GetMailUserOptionRequest), typeof(GetMailUserOptionResponse),
            typeof(UpdateMailUserOptionRequest), typeof(MailUserOption), typeof(MailUserOptionList),
            // 管理端共享包装结构。
            typeof(MailStringList), typeof(MailUintList),
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
