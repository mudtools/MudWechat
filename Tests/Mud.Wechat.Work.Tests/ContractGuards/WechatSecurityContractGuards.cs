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
using Mud.Wechat.Work.DataModels.Security;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 安全管理域（Security 模块）契约守卫：路由表、接口层级、令牌绑定与 JSON 上下文登记锁定。
/// </summary>
/// <remarks>
/// <para>
/// 官方「安全管理」目录（文件防泄漏 98079 / 设备管理 98920 / 截屏录屏管理 100128 / 域名 IP 信息 100079 /
/// 高级功能账号管理 99503、99505、99506 / 操作日志 100178、100179）共 16 个端点，
/// <b>全部仅向企业自建应用开放</b>（第三方/代开发官方无文档）⇒ 按官方功能分组拆三个接口族
/// （安全管理 / 高级功能账号管理 / 操作日志），形态同通讯录查看权限管理域：
/// <b>父接口零端点（IsAbstract）+ 端点全落自建子接口</b>，不设第三方/代开发子接口。
/// </para>
/// </remarks>
public class WechatSecurityContractGuards
{
    private const string SecurityRegistryGroupName = "Security";

    /// <summary>安全管理族官方路由表（文件防泄漏 1 + 设备管理 6 + 截屏录屏 1 + 域名 IP 1 = 9 条，全在自建子接口）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SecurityRoutes =
    {
        // ── 文件防泄漏（98079） ──
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.GetFileOperRecordAsync), typeof(PostAttribute), "/cgi-bin/security/get_file_oper_record"),

        // ── 设备管理（98920，trustdevice 六端点） ──
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.ImportTrustDevicesAsync), typeof(PostAttribute), "/cgi-bin/security/trustdevice/import"),
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.GetTrustDevicesAsync), typeof(PostAttribute), "/cgi-bin/security/trustdevice/list"),
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.GetTrustDevicesByUserAsync), typeof(PostAttribute), "/cgi-bin/security/trustdevice/get_by_user"),
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.DeleteTrustDevicesAsync), typeof(PostAttribute), "/cgi-bin/security/trustdevice/delete"),
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.ApproveTrustDevicesAsync), typeof(PostAttribute), "/cgi-bin/security/trustdevice/approve"),
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.RejectTrustDevicesAsync), typeof(PostAttribute), "/cgi-bin/security/trustdevice/reject"),

        // ── 截屏/录屏管理（100128） ──
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.GetScreenOperRecordAsync), typeof(PostAttribute), "/cgi-bin/security/get_screen_oper_record"),

        // ── 域名 IP 信息（100079，官方唯一 GET 端点） ──
        (typeof(IWechatWorkInternalSecurityService), nameof(IWechatWorkInternalSecurityService.GetServerDomainIpAsync), typeof(GetAttribute), "/cgi-bin/security/get_server_domain_ip"),
    };

    /// <summary>高级功能账号管理族官方路由表（99503/99505/99506，5 条全 POST，全在自建子接口）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] VipRoutes =
    {
        (typeof(IWechatWorkInternalSecurityVipService), nameof(IWechatWorkInternalSecurityVipService.SubmitBatchAddVipJobAsync), typeof(PostAttribute), "/cgi-bin/security/vip/submit_batch_add_job"),
        (typeof(IWechatWorkInternalSecurityVipService), nameof(IWechatWorkInternalSecurityVipService.GetBatchAddVipJobResultAsync), typeof(PostAttribute), "/cgi-bin/security/vip/batch_add_job_result"),
        (typeof(IWechatWorkInternalSecurityVipService), nameof(IWechatWorkInternalSecurityVipService.SubmitBatchDelVipJobAsync), typeof(PostAttribute), "/cgi-bin/security/vip/submit_batch_del_job"),
        (typeof(IWechatWorkInternalSecurityVipService), nameof(IWechatWorkInternalSecurityVipService.GetBatchDelVipJobResultAsync), typeof(PostAttribute), "/cgi-bin/security/vip/batch_del_job_result"),
        (typeof(IWechatWorkInternalSecurityVipService), nameof(IWechatWorkInternalSecurityVipService.ListVipAccountsAsync), typeof(PostAttribute), "/cgi-bin/security/vip/list"),
    };

    /// <summary>操作日志族官方路由表（100178/100179，2 条全 POST，全在自建子接口）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] OperLogRoutes =
    {
        (typeof(IWechatWorkInternalSecurityOperLogService), nameof(IWechatWorkInternalSecurityOperLogService.ListMemberOperLogsAsync), typeof(PostAttribute), "/cgi-bin/security/member_oper_log/list"),
        (typeof(IWechatWorkInternalSecurityOperLogService), nameof(IWechatWorkInternalSecurityOperLogService.ListAdminOperLogsAsync), typeof(PostAttribute), "/cgi-bin/security/admin_oper_log/list"),
    };

    /// <summary>
    /// 契约守卫 SEC1：安全管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void SecurityEndpoints_ShouldMatchOfficialRoutes()
    {
        var allRoutes = SecurityRoutes.Concat(VipRoutes).Concat(OperLogRoutes).ToList();

        allRoutes.Should().HaveCount(16,
            "官方安全管理目录共 16 个端点（文件防泄漏 1 + 设备管理 6 + 截屏录屏 1 + 域名 IP 1 + 高级功能账号 5 + 操作日志 2）");

        var distinctRoutes = allRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(16, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in allRoutes)
        {
            // DeclaredOnly：端点必须落在自建子接口自身声明，而非从父接口继承。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 SEC2：接口层级——三个接口族均为「父接口零端点（IsAbstract）+ 端点全落自建子接口」，
    /// 不设第三方/代开发子接口（官方安全管理对第三方/代开发均无文档；能力漂移守卫）。
    /// </summary>
    [Fact]
    public void SecurityInterfaceHierarchy_ShouldConvergeOnInternalChildren()
    {
        var families = new (Type Parent, Type InternalChild, int InternalEndpointCount, string ParentImplementation)[]
        {
            (typeof(IWechatWorkSecurityService), typeof(IWechatWorkInternalSecurityService), 9, "WechatWorkSecurityService"),
            (typeof(IWechatWorkSecurityVipService), typeof(IWechatWorkInternalSecurityVipService), 5, "WechatWorkSecurityVipService"),
            (typeof(IWechatWorkSecurityOperLogService), typeof(IWechatWorkInternalSecurityOperLogService), 2, "WechatWorkSecurityOperLogService"),
        };

        foreach (var (parent, internalChild, endpointCount, parentImplementation) in families)
        {
            internalChild.Should().BeAssignableTo(parent, $"{internalChild.Name} 必须继承公共父接口 {parent.Name}");

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"官方仅向自建应用开放 {parent.Name} 对应功能组的端点，父接口不存在公共面，不得声明端点");

            var internalApi = internalChild.GetCustomAttribute<HttpClientApiAttribute>();
            internalApi.Should().NotBeNull($"{internalChild.Name} 必须声明 [HttpClientApi]");
            internalApi!.RegistryGroupName.Should().Be(SecurityRegistryGroupName,
                $"{internalChild.Name} 必须挂 {SecurityRegistryGroupName} 注册组（经 Add{SecurityRegistryGroupName}Api() 注册）");
            internalApi.InheritedFrom.Should().Be(parentImplementation,
                $"{internalChild.Name} 必须继承父接口生成实现类，避免生成器重复实现");
            internalChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{internalChild.Name} 必须恰好承载 {endpointCount} 个自建端点");
        }

        // 应用类型集合漂移守卫：继承链上不得出现其它子接口（新增第三方/代开发子接口须先核对官方文档并同批调整本守卫与 G5）。
        foreach (var (parent, _, _, _) in families)
        {
            var derived = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && parent.IsAssignableFrom(t) && t != parent)
                .Select(t => t.Name)
                .ToList();
            derived.Should().HaveCount(1,
                $"{parent.Name} 继承链上只允许存在自建子接口（官方安全管理对第三方/代开发均无文档）");
            derived[0].Should().StartWith("IWechatWorkInternal", "唯一子接口必须为自建（Internal）子接口");
        }
    }

    /// <summary>
    /// 契约守卫 SEC3：安全管理域令牌绑定——三族六接口统一消费 AccessToken 路由键并以 Query 注入。
    /// </summary>
    [Fact]
    public void SecurityTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkSecurityService),
            typeof(IWechatWorkInternalSecurityService),
            typeof(IWechatWorkSecurityVipService),
            typeof(IWechatWorkInternalSecurityVipService),
            typeof(IWechatWorkSecurityOperLogService),
            typeof(IWechatWorkInternalSecurityOperLogService),
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
    /// 契约守卫 SEC4：安全管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文（38 型）。
    /// </summary>
    [Fact]
    public void SecurityDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = SecurityJsonContext.Default;

        var requiredTypes = new[]
        {
            // 文件防泄漏族
            typeof(GetFileOperRecordRequest), typeof(FileOperRecordOperation),
            typeof(FileOperRecordExternalUser), typeof(FileOperRecordItem),
            typeof(GetFileOperRecordResponse),
            // 设备管理族
            typeof(ImportTrustDeviceItem), typeof(ImportTrustDevicesRequest),
            typeof(TrustDeviceImportResult), typeof(ImportTrustDevicesResponse),
            typeof(GetTrustDevicesRequest), typeof(TrustDeviceInfo), typeof(GetTrustDevicesResponse),
            typeof(GetTrustDevicesByUserRequest), typeof(GetTrustDevicesByUserResponse),
            typeof(DeleteTrustDevicesRequest),
            typeof(ApproveTrustDevicesRequest), typeof(ApproveTrustDevicesResponse),
            typeof(RejectTrustDevicesRequest), typeof(RejectTrustDevicesResponse),
            // 截屏/录屏管理族
            typeof(GetScreenOperRecordRequest), typeof(ScreenWindowsDevice),
            typeof(ScreenMacDevice), typeof(ScreenMobileDevice),
            typeof(ScreenRecordDeviceInfo), typeof(ScreenOperRecordItem),
            typeof(GetScreenOperRecordResponse),
            // 域名 IP 信息族
            typeof(ServerDomainInfo), typeof(ServerIpInfo), typeof(GetServerDomainIpResponse),
            // 高级功能账号管理族
            typeof(SubmitBatchAddVipJobRequest), typeof(SubmitBatchAddVipJobResponse),
            typeof(GetBatchAddVipJobRequest), typeof(VipJobResult), typeof(GetBatchAddVipJobResultResponse),
            typeof(SubmitBatchDelVipJobRequest), typeof(SubmitBatchDelVipJobResponse),
            typeof(GetBatchDelVipJobRequest), typeof(GetBatchDelVipJobResultResponse),
            typeof(ListVipAccountsRequest), typeof(ListVipAccountsResponse),
            // 操作日志族
            typeof(ListMemberOperLogsRequest), typeof(MemberOperLogItem), typeof(ListMemberOperLogsResponse),
            typeof(ListAdminOperLogsRequest), typeof(AdminOperLogItem), typeof(ListAdminOperLogsResponse),
        };

        requiredTypes.Should().HaveCount(46, "安全管理域契约面共 46 个 DTO 类型");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是安全管理域契约面类型，必须登记进 SecurityJsonContext（AOT 源生成）");
        }
    }
}
