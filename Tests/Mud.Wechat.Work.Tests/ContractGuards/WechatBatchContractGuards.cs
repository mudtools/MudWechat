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
using Mud.Wechat.Work.DataModels.Contacts.Batch;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 异步导入接口域（Contact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态与标签域同构：官方对自建应用与第三方应用开放完全一致的 4 个端点（增量更新成员、全量覆盖成员、
/// 全量覆盖部门、获取异步任务结果），全部端点收敛于父接口 <see cref="IWechatWorkBatchService"/>，
/// 自建与第三方子接口均为空标记；服务商代开发无此功能，不设对应子接口。
/// </para>
/// </remarks>
public class WechatBatchContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkBatchService";

    private const string ContactRegistryGroupName = "Contact";

    /// <summary>官方路由表（4 个端点全部声明于父接口；自建与第三方官方文档路由完全一致）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkBatchService), nameof(IWechatWorkBatchService.SyncUsersAsync), typeof(PostAttribute), "/cgi-bin/batch/syncuser"),
        (typeof(IWechatWorkBatchService), nameof(IWechatWorkBatchService.ReplaceUsersAsync), typeof(PostAttribute), "/cgi-bin/batch/replaceuser"),
        (typeof(IWechatWorkBatchService), nameof(IWechatWorkBatchService.ReplaceDepartmentsAsync), typeof(PostAttribute), "/cgi-bin/batch/replaceparty"),
        (typeof(IWechatWorkBatchService), nameof(IWechatWorkBatchService.GetBatchJobResultAsync), typeof(GetAttribute), "/cgi-bin/batch/getresult"),
    };

    /// <summary>
    /// 契约守卫 B1：异步导入接口域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void BatchEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(4, "官方对自建/第三方应用开放一致的 4 个异步导入端点，全部声明于父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：端点必须落在父接口自身声明，而非从子接口继承。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 B2：接口层级与生成器注册形态——全部端点收敛父接口（IsAbstract），
    /// 自建与第三方应用类型子接口均为空标记（能力漂移守卫：任何子接口不得新增端点）；
    /// 官方未向服务商代开发开放本域，故不存在代开发子接口（应用类型子接口仅覆盖官方实际开放的应用类型）。
    /// </summary>
    [Fact]
    public void BatchInterfaceHierarchy_ShouldConvergeOnAbstractParentWithContactRegistry()
    {
        var parent = typeof(IWechatWorkBatchService);
        var children = new[]
        {
            typeof(IWechatWorkInternalBatchService),
            typeof(IWechatWorkThirdPartyBatchService),
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
            childApi!.RegistryGroupName.Should().Be(ContactRegistryGroupName,
                $"{child.Name} 必须挂 Contact 注册组（与成员/部门/标签/通讯录查看权限域共用 Add{ContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 官方对自建/第三方开放一致端点集、未向代开发开放：任何子接口不得新增端点（能力集合漂移守卫）。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty(
                    $"{child.Name} 为应用类型空标记：4 个异步导入端点全部声明于父接口，" +
                    "新增差异端点须先核对官方文档并同批调整 B1/B2");
        }
    }

    /// <summary>
    /// 契约守卫 B3：令牌绑定——父/自建/第三方三接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void BatchTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkBatchService),
            typeof(IWechatWorkInternalBatchService),
            typeof(IWechatWorkThirdPartyBatchService),
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
    /// 契约守卫 B4：异步导入接口域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void BatchDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = BatchJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(BatchCallbackRequest), typeof(BatchImportUsersRequest),
            typeof(BatchImportPartiesRequest), typeof(BatchJobResponse),
            typeof(BatchTaskResultItem), typeof(GetBatchJobResultResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是异步导入接口域契约面类型，必须登记进 BatchJsonContext（AOT 源生成）");
        }
    }
}
