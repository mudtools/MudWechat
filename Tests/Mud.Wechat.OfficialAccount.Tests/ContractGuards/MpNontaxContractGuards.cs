// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OfficialAccount.DataModels.Nontax;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 非税支付域契约守卫（B2b 落地）。
/// </summary>
/// <remarks>
/// <b>双通道分工（对齐素材域 I3 裁决）</b>：JSON 面 16 端点走生成管线；
/// <c>downloadbill</c> 成功响应为 CSV 文本（官方 FAQ 原文），走
/// <see cref="Mud.Wechat.OfficialAccount.MpNontaxBillDownloadService"/> 独立下载通道
/// ——本守卫同时锁定「downloadbill 不得进入生成管线」防两通道重复建模。
/// </remarks>
public class MpNontaxContractGuards
{
    private const string RegistryGroupName = "Nontax";

    /// <summary>官方路由表（JSON 面 16 端点，全 POST、全带 body）。</summary>
    private static readonly (string Method, string Route)[] Routes =
    {
        (nameof(IMpNontaxService.QueryFeeAsync), "/nontax/queryfee"),
        (nameof(IMpNontaxService.UnifiedOrderAsync), "/nontax/unifiedorder"),
        (nameof(IMpNontaxService.GetOrderAsync), "/nontax/getorder"),
        (nameof(IMpNontaxService.RefundAsync), "/nontax/refund"),
        (nameof(IMpNontaxService.GetOrderListAsync), "/nontax/getorderlist"),
        (nameof(IMpNontaxService.MicroPayAsync), "/nontax/micropay"),
        (nameof(IMpNontaxService.NotifyInconsistentOrderAsync), "/nontax/notifyinconsistentorder"),
        (nameof(IMpNontaxService.MockNotificationAsync), "/nontax/mocknotification"),
        (nameof(IMpNontaxService.MockQueryFeeAsync), "/nontax/mockqueryfee"),
        (nameof(IMpNontaxService.GetRealNameAsync), "/nontax/getrealname"),
        (nameof(IMpNontaxService.QueryVehicleStateAsync), "/nontax/vehicle/querystate"),
        (nameof(IMpNontaxService.VehicleEntranceNotifyAsync), "/nontax/vehicle/entrancenotify"),
        (nameof(IMpNontaxService.VehiclePayApplyAsync), "/nontax/vehicle/payapply"),
        (nameof(IMpNontaxService.CreateBillCardAsync), "/nontax/createbillcard"),
        (nameof(IMpNontaxService.InsertBillAsync), "/nontax/insertbill"),
        (nameof(IMpNontaxService.GetBillAuthUrlAsync), "/nontax/getbillauthurl"),
    };

    /// <summary>契约守卫 NT1：16 端点路由与官方契约一致（全 POST /nontax/*）；下载通道单列。</summary>
    [Fact]
    public void NontaxEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(16, "非税 JSON 面 16 端点（查询 5 + 支付 1 + 通知/模拟 3 + 实名 1 + 车辆 3 + 票据卡券 3）；downloadbill 在下载通道");
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(16, "各端点路由互不重复");

        foreach (var (method, route) in Routes)
        {
            var target = typeof(IMpNontaxService)
                .GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");

            target.GetParameters().Should().Contain(p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method} 官方契约要求 JSON 请求体");
        }

        typeof(IMpNontaxService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("16 端点官方全部为 POST");

        // 双通道分工：downloadbill 不得进入生成管线（成功响应为 CSV 文本，非 JSON）。
        typeof(IMpNontaxService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().NotContain(static a => a.RequestUri == "/nontax/downloadbill",
                "downloadbill 成功响应为 CSV 文本（官方 FAQ 原文），走独立下载通道（对齐素材下载 I3 裁决）");
        Mud.Wechat.OfficialAccount.MpNontaxBillDownloadService.DownloadBillPath
            .Should().Be("/nontax/downloadbill", "下载通道路径经常量锁定");
    }

    /// <summary>契约守卫 NT2：非税 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void NontaxDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = NontaxJsonContext.Default;

        var domainTypes = typeof(MpNontaxQueryFeeRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Nontax"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 42;
        domainTypes.Should().HaveCount(expectedCount,
            "非税域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（核心 22：查询/下单/查单/退款/被扫/列表/通知/模拟/实名 + 车辆 8 + 票据卡券 12，" +
            "明细项与订单项分立两形）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于非税域命名空间，必须登记进 NontaxJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
    }
}
