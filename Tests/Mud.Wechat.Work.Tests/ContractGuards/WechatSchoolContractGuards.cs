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
using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 家校沟通模块（School 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （家校沟通基础域：三类应用公共面收敛父接口 + 空标记子接口；
/// 家校管理配置域：官方仅自建与第三方开放，不设代开发子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：家校沟通基础域对齐企业支付域对外收款记录族（三类公共面收敛）；
/// 家校管理配置域对齐通讯录异步导入域（仅自建 + 第三方子接口，官方未向代开发开放）。
/// </para>
/// <para>
/// 既有覆盖注记：发送「学校通知」（<c>externalcontact/message/send</c>）由消息推送域
/// 家校学校通知族承载（三类应用公共面）；获取外部联系人详情（<c>externalcontact/get</c>，
/// 家校版文档 91670）由客户联系域客户管理族承载，其响应中的家校字段
/// （is_subscribe / subscriber_info）已补入 <c>ExternalContactInfo</c>——两路由均不在本模块重复声明。
/// </para>
/// </remarks>
public class WechatSchoolContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string SchoolParentImplementationClassName = "WechatWorkSchoolService";

    private const string SchoolSettingParentImplementationClassName = "WechatWorkSchoolSettingService";

    private const string SchoolRegistryGroupName = "School";

    /// <summary>
    /// 家校沟通基础域官方路由表（三类应用公共面，7 条端点全部收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolRoutes =
    {
        // 获取「学校通知」二维码（自建 92320、第三方 92197、代开发 96719；官方即 GET）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetSubscribeQrCodeAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get_subscribe_qr_code"),
        // 设置关注「学校通知」的模式（自建 92318、第三方 92290）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.SetSubscribeModeAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/set_subscribe_mode"),
        // 获取关注「学校通知」的模式（自建 92318、第三方 92290；官方即 GET）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetSubscribeModeAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get_subscribe_mode"),
        // 获取「班级群创建方式」（92430；官方即 GET）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetChatCreateModeAsync),
            typeof(GetAttribute), "/cgi-bin/school/get_chat_create_mode"),
        // 设置「班级群创建方式」（92430）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.SetChatCreateModeAsync),
            typeof(PostAttribute), "/cgi-bin/school/set_chat_create_mode"),
        // 外部联系人 openid 转换（自建 92323、第三方 92292、代开发 96721）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.ConvertToOpenIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/convert_to_openid"),
        // 获取可使用的家长范围（自建 94895、第三方 94960、代开发 96725；官方即 GET，agentid 走 Query）。
        (typeof(IWechatWorkSchoolService),
            nameof(IWechatWorkSchoolService.GetAllowScopeAsync),
            typeof(GetAttribute), "/cgi-bin/school/agent/get_allow_scope"),
    };

    /// <summary>
    /// 家校管理配置域官方路由表（官方仅自建与第三方开放，3 条端点全部收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SchoolSettingRoutes =
    {
        // 设置「老师可查看班级」的模式（92652）。
        (typeof(IWechatWorkSchoolSettingService),
            nameof(IWechatWorkSchoolSettingService.SetTeacherViewModeAsync),
            typeof(PostAttribute), "/cgi-bin/school/set_teacher_view_mode"),
        // 获取「老师可查看班级」的模式（92652；官方即 GET）。
        (typeof(IWechatWorkSchoolSettingService),
            nameof(IWechatWorkSchoolSettingService.GetTeacherViewModeAsync),
            typeof(GetAttribute), "/cgi-bin/school/get_teacher_view_mode"),
        // 手机号转外部联系人 ID（92506）。
        (typeof(IWechatWorkSchoolSettingService),
            nameof(IWechatWorkSchoolSettingService.BatchToExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/batch_to_external_userid"),
    };

    /// <summary>
    /// 契约守卫 SCH1a：家校沟通基础域全部端点路由必须与官方契约一致
    /// （7 个端点为三类应用公共面，全部收敛父接口；二维码/关注模式/班级群创建方式/家长范围
    /// 官方即 GET，勿改成 POST）。
    /// </summary>
    [Fact]
    public void SchoolEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolRoutes.Should().HaveCount(7,
            "家校沟通基础域 7 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = SchoolRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(7, "家校沟通基础域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolRoutes)
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
    /// 契约守卫 SCH1b：家校管理配置域全部端点路由必须与官方契约一致
    /// （官方仅自建与第三方开放；获取老师可查看班级模式官方即 GET）。
    /// </summary>
    [Fact]
    public void SchoolSettingEndpoints_ShouldMatchOfficialRoutes()
    {
        SchoolSettingRoutes.Should().HaveCount(3,
            "家校管理配置域 3 个端点官方仅自建与第三方应用开放，全部声明于父接口");

        var distinctRoutes = SchoolSettingRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "家校管理配置域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in SchoolSettingRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // get_allow_scope 的 agentid 必须以 [Query("agentid")] 显式 Query 参数传递（官方契约）。
        var methodInfo = typeof(IWechatWorkSchoolService).GetMethod(
            nameof(IWechatWorkSchoolService.GetAllowScopeAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        methodInfo.Should().NotBeNull();
        methodInfo!.GetParameters()
            .Any(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "agentid")
            .Should().BeTrue("get_allow_scope 的 agentid 为官方 Query 参数，必须以 [Query(\"agentid\")] 标注");
    }

    /// <summary>
    /// 契约守卫 SCH2：接口层级与生成器注册形态——
    /// 家校沟通基础域：三类应用公共面收敛父接口（IsAbstract），自建/第三方/代开发子接口均为
    /// 零差异端点空标记；家校管理配置域：父接口承载端点（IsAbstract），继承链上恰好只有
    /// 自建与第三方子接口（官方未向代开发开放，能力漂移守卫）。
    /// </summary>
    [Fact]
    public void SchoolInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        var families = new[]
        {
            (typeof(IWechatWorkSchoolService),
                new[] { typeof(IWechatWorkInternalSchoolService), typeof(IWechatWorkThirdPartySchoolService), typeof(IWechatWorkProviderSchoolService) },
                SchoolParentImplementationClassName,
                "家校沟通基础域 7 个端点为三类应用公共面，继承链上不得出现其它子接口"),
            (typeof(IWechatWorkSchoolSettingService),
                new[] { typeof(IWechatWorkInternalSchoolSettingService), typeof(IWechatWorkThirdPartySchoolSettingService) },
                SchoolSettingParentImplementationClassName,
                "家校管理配置域官方仅向自建与第三方应用开放，继承链上不得出现代开发等其它子接口"),
        };

        foreach (var (parent, children, implementationClassName, because) in families)
        {
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();

            assignable.Should().BeEquivalentTo(children, because);

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeEmpty("公共端点全部声明于父接口");

            foreach (var child in children)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(SchoolRegistryGroupName,
                    $"{child.Name} 必须挂 {SchoolRegistryGroupName} 注册组" +
                    $"（School 模块共用 Add{SchoolRegistryGroupName}WebApiHttpClient()）");
                childApi.InheritedFrom.Should().Be(implementationClassName,
                    $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

                // 三类应用开放面完全一致（或自建+第三方一致）：任何子接口出现差异端点均为能力漂移。
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
            }
        }
    }

    /// <summary>
    /// 契约守卫 SCH3：令牌绑定——School 模块全部 7 个接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void SchoolTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkSchoolService),
            typeof(IWechatWorkInternalSchoolService),
            typeof(IWechatWorkThirdPartySchoolService),
            typeof(IWechatWorkProviderSchoolService),
            typeof(IWechatWorkSchoolSettingService),
            typeof(IWechatWorkInternalSchoolSettingService),
            typeof(IWechatWorkThirdPartySchoolSettingService),
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
    /// 契约守卫 SCH4：家校沟通模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 17 个契约面类型）。
    /// </summary>
    [Fact]
    public void SchoolDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = SchoolJsonContext.Default;

        var requiredTypes = new[]
        {
            // 「学校通知」二维码 + 关注模式。
            typeof(GetSchoolSubscribeQrCodeResponse),
            typeof(SetSchoolSubscribeModeRequest), typeof(GetSchoolSubscribeModeResponse),
            // 班级群创建方式。
            typeof(GetSchoolChatCreateModeResponse), typeof(SetSchoolChatCreateModeRequest),
            // 外部联系人 openid 转换。
            typeof(SchoolConvertToOpenIdRequest), typeof(SchoolConvertToOpenIdResponse),
            // 可使用的家长范围。
            typeof(GetSchoolAllowScopeResponse), typeof(SchoolAllowScope),
            typeof(SchoolAllowScopeStudents), typeof(SchoolAllowScopeDepartments),
            // 老师可查看班级模式。
            typeof(SetSchoolTeacherViewModeRequest), typeof(GetSchoolTeacherViewModeResponse),
            // 手机号转外部联系人 ID。
            typeof(SchoolBatchToExternalUserIdRequest), typeof(SchoolBatchToExternalUserIdResponse),
            typeof(SchoolMobileConvertSuccessItem), typeof(SchoolMobileConvertFailItem),
        };

        requiredTypes.Should().HaveCount(17, "家校沟通域契约面共 17 型");
        requiredTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是家校沟通模块契约面类型，必须登记进 SchoolJsonContext（AOT 源生成）");
        }
    }
}
