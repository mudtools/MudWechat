// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OfficialAccount.DataModels.Poi;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 微信门店旧版（POI）域契约守卫（B2a 落地）。
/// </summary>
/// <remarks>
/// <b>与 <c>Store</c> 域的硬边界</b>：本域锁的是<b>旧版</b>微信门店接口（<c>/cgi-bin/poi/*</c>）；
/// <c>MpStoreContractGuards</c> 锁的是新版小程序店铺 API（<c>/wxa/*</c>）。两代接口同仓并存，
/// 本守卫同时锁定「本域路由不得出现在 Store 域」防跨代合并。
/// </remarks>
public class MpPoiContractGuards
{
    private const string RegistryGroupName = "Poi";

    /// <summary>官方路由表（3 端点，全 POST、全带 body）。</summary>
    private static readonly (string Method, string Route)[] Routes =
    {
        (nameof(IMpPoiService.GetPoiAsync), "/cgi-bin/poi/getpoi"),
        (nameof(IMpPoiService.GetPoiListAsync), "/cgi-bin/poi/getpoilist"),
        (nameof(IMpPoiService.DeletePoiAsync), "/cgi-bin/poi/delpoi"),
    };

    /// <summary>契约守卫 PO1：3 端点路由与官方契约一致（全 POST /cgi-bin/poi/*）。</summary>
    [Fact]
    public void PoiEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(3, "旧版门店接口官方开放面恰 3 个查询/删除端点（新建/更新无 HTTP 入口）");
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(3, "各端点路由互不重复");

        foreach (var (method, route) in Routes)
        {
            var target = typeof(IMpPoiService)
                .GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");

            target.GetParameters().Should().Contain(p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method} 官方契约要求 JSON 请求体");
        }

        typeof(IMpPoiService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("3 端点官方全部为 POST");
    }

    /// <summary>契约守卫 PO2：门店 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void PoiDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = PoiJsonContext.Default;

        var domainTypes = typeof(MpPoiBaseInfo).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Poi"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 7;
        domainTypes.Should().HaveCount(expectedCount,
            "门店域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（请求 3 + 响应 2 + 超集基础信息 1 + 图片项 1；get 与 list 的 base_info 以单一超集类承载）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于门店域命名空间，必须登记进 PoiJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
    }
}
