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
using Mud.Wechat.Work.DataModels.Hr;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 人事助手模块（Hr 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （花名册域：官方仅自建应用开放——代开发与第三方权限表均标注暂不支持）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：零端点父接口 + 仅自建子接口承载端点（形态对齐会话内容存档域守卫）。
/// 官方文档陷阱锁定：get_fields 的字段信息说明表记作 <c>field_id</c>，
/// 而请求/响应示例均为 <c>fieldid</c>（与 get_staff_info / update_staff_info 请求侧一致），本仓以示例为准。
/// </para>
/// </remarks>
public class WechatHrContractGuards
{
    private const string HrRegistryGroupName = "Hr";

    /// <summary>
    /// 花名册域官方路由表（仅自建开放，3 条端点声明于自建子接口；
    /// get_fields 官方即 GET 且无请求体，其余官方即 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] HrRoutes =
    {
        // 获取员工字段配置（99131；官方即 GET，仅 access_token，无请求体）。
        (typeof(IWechatWorkInternalHrService),
            nameof(IWechatWorkInternalHrService.GetFieldsAsync),
            typeof(GetAttribute), "/cgi-bin/hr/get_fields"),
        // 获取员工花名册信息（99132）。
        (typeof(IWechatWorkInternalHrService),
            nameof(IWechatWorkInternalHrService.GetStaffInfoAsync),
            typeof(PostAttribute), "/cgi-bin/hr/get_staff_info"),
        // 更新员工花名册信息（99133；update_items / remove_items / insert_items 三者不能全部为空）。
        (typeof(IWechatWorkInternalHrService),
            nameof(IWechatWorkInternalHrService.UpdateStaffInfoAsync),
            typeof(PostAttribute), "/cgi-bin/hr/update_staff_info"),
    };

    /// <summary>
    /// 契约守卫 HR1：花名册域端点路由必须与官方契约一致
    ///（get_fields 官方即 GET 无请求体，勿改成 POST/Body；其余官方即 POST，勿改成 GET）。
    /// </summary>
    [Fact]
    public void HrEndpoints_ShouldMatchOfficialRoutes()
    {
        HrRoutes.Should().HaveCount(3,
            "花名册域 3 个端点官方仅自建应用开放，声明于自建子接口");

        var distinctRoutes = HrRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "花名册域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in HrRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // get_fields 官方即 GET 且无请求体：方法不得引入 [Body] 参数。
        var getFields = typeof(IWechatWorkInternalHrService).GetMethod(
            nameof(IWechatWorkInternalHrService.GetFieldsAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getFields.Should().NotBeNull();
        getFields!.GetParameters()
            .Should().NotContain(p => p.GetCustomAttribute<BodyAttribute>() != null,
                "hr/get_fields 官方仅携带 access_token，无请求体");
    }

    /// <summary>
    /// 契约守卫 HR2：官方文档陷阱锁定——字段 id 官方示例为 fieldid（参数表文本误记 field_id），
    /// 请求与响应两侧的 DTO 必须以 fieldid 为准，防止双向漂移。
    /// </summary>
    [Fact]
    public void HrFieldId_ShouldFollowOfficialExamples()
    {
        typeof(HrFieldConfig).GetProperty(nameof(HrFieldConfig.Fieldid))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("fieldid", "get_fields 响应示例即 fieldid（参数表文本的 field_id 为官方笔误）");

        typeof(HrFieldQueryItem).GetProperty(nameof(HrFieldQueryItem.Fieldid))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("fieldid", "get_staff_info 请求侧官方原文即 fieldid");

        typeof(HrFieldInfo).GetProperty(nameof(HrFieldInfo.Fieldid))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("fieldid", "get_staff_info 响应侧官方原文即 fieldid");

        typeof(HrUpdateFieldItem).GetProperty(nameof(HrUpdateFieldItem.Fieldid))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("fieldid", "update_staff_info 请求侧官方原文即 fieldid");
    }

    /// <summary>
    /// 契约守卫 HR3：仅单类应用开放域的接口层级——花名册域官方仅自建应用开放，
    /// 为零端点父接口 + 仅自建子接口承载端点，继承链上不得出现第三方 / 代开发子接口。
    /// </summary>
    [Fact]
    public void HrSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var parent = typeof(IWechatWorkHrService);
        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkInternalHrService) },
            "继承链上不得出现自建之外的子接口：官方仅向自建应用开放（代开发/第三方暂不支持）");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("零端点父接口：端点全部声明于唯一的子接口");

        var childApi = typeof(IWechatWorkInternalHrService).GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull("子接口必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(HrRegistryGroupName,
            $"{nameof(IWechatWorkInternalHrService)} 必须挂 {HrRegistryGroupName} 注册组");
        typeof(IWechatWorkInternalHrService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(3, "自建子接口承载花名册域全部 3 个端点");
    }

    /// <summary>
    /// 契约守卫 HR4：令牌绑定——Hr 模块两个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；自建子接口必须声明 InternalAccessToken 归属域键）。
    /// </summary>
    [Fact]
    public void HrTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        foreach (var iface in new[] { typeof(IWechatWorkHrService), typeof(IWechatWorkInternalHrService) })
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 HR5：人事助手模块的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void HrDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = HrJsonContext.Default;

        var requiredTypes = new[]
        {
            // 获取员工字段配置。
            typeof(GetHrFieldsResponse), typeof(HrFieldGroup), typeof(HrFieldConfig), typeof(HrFieldOption),
            // 获取员工花名册信息。
            typeof(GetHrStaffInfoRequest), typeof(HrFieldQueryItem),
            typeof(GetHrStaffInfoResponse), typeof(HrFieldInfo), typeof(HrMobileValue), typeof(HrFileValue),
            // 更新员工花名册信息。
            typeof(UpdateHrStaffInfoRequest), typeof(HrUpdateFieldItem),
            typeof(HrRemoveFieldGroup), typeof(HrInsertFieldGroup),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是人事助手模块契约面类型，必须登记进 HrJsonContext（AOT 源生成）");
        }
    }
}
