// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels;
using Mud.Wechat.Channels.DataModels.Logistics;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「物流发货」域契约守卫（设计方案 v1 P1 / CL 系列：Logistics 域 28 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域边界（设计方案 v1 §2.2）</b>：物流发货 = 地址 5 + 运费模板 4 + 电子面单 16 + 订单发货 3 = 28 端点
/// （全部 POST）。与 SHIPINHAO 本地生活重叠的 <c>merchant/address/*</c>、<c>ewaybill/biz/*</c> 只计 1；
/// <c>order/delivery/*</c> 3 端点归本域（订单域不落发货端点）。
/// </para>
/// <para>
/// <b>三组前缀并存</b>：<c>/channels/ec/merchant/address|freight*</c>（9）、
/// <c>/channels/ec/logistics/ewaybill/biz/*</c>（16）、<c>/channels/ec/order/delivery*</c>（3）。
/// </para>
/// </remarks>
public class ChannelsLogisticsContractGuards
{
    private const string LogisticsRegistryGroupName = "Logistics";

    /// <summary>Logistics 域官方路由表（28 端点，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] LogisticsRoutes =
    {
        // 地址（5）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.AddAddressAsync), typeof(PostAttribute), "/channels/ec/merchant/address/add"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetAddressAsync), typeof(PostAttribute), "/channels/ec/merchant/address/get"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetAddressListAsync), typeof(PostAttribute), "/channels/ec/merchant/address/list"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.DeleteAddressAsync), typeof(PostAttribute), "/channels/ec/merchant/address/delete"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.UpdateAddressAsync), typeof(PostAttribute), "/channels/ec/merchant/address/update"),
        // 运费模板（4）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.AddFreightTemplateAsync), typeof(PostAttribute), "/channels/ec/merchant/addfreighttemplate"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetFreightTemplateDetailAsync), typeof(PostAttribute), "/channels/ec/merchant/getfreighttemplatedetail"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetFreightTemplateListAsync), typeof(PostAttribute), "/channels/ec/merchant/getfreighttemplatelist"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.UpdateFreightTemplateAsync), typeof(PostAttribute), "/channels/ec/merchant/updatefreighttemplate"),
        // 电子面单 - 下单（5）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.CreateEwaybillOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/create"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.PrecreateEwaybillOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/precreate"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/get"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.CancelEwaybillOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/cancel"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.PrintEwaybillOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/print"),
        // 电子面单 - 批量 / 子件（2）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.BatchPrintEwaybillOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/batchprint"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.AddEwaybillSubOrderAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/order/addsuborder"),
        // 电子面单 - 快递公司 / 打印报文（2）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillDeliveryListAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/delivery/get"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillPrintContentAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/print/get"),
        // 电子面单 - 模板（7）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillTemplateConfigAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/template/config"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillTemplateByIdAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/template/getbyid"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillTemplateAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/template/get"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.CreateEwaybillTemplateAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/template/create"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.UpdateEwaybillTemplateAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/template/update"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.DeleteEwaybillTemplateAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/template/delete"),
        // 电子面单 - 账号（1）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetEwaybillAcctAsync), typeof(PostAttribute), "/channels/ec/logistics/ewaybill/biz/account/get"),
        // 订单发货（3）
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.SendDeliveryAsync), typeof(PostAttribute), "/channels/ec/order/delivery/send"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.GetDeliveryCompanyListAsync), typeof(PostAttribute), "/channels/ec/order/deliverycompanylist/new/get"),
        (typeof(IChannelsLogisticsService), nameof(IChannelsLogisticsService.DeliveryCompensationAsync), typeof(PostAttribute), "/channels/ec/order/delivery/compensation"),
    };

    /// <summary>契约守卫 CL1：Logistics 域 28 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void LogisticsEndpoints_ShouldMatchOfficialRoutes()
    {
        LogisticsRoutes.Should().HaveCount(28, "Logistics 域恰 28 端点（地址 5 + 运费模板 4 + 电子面单 16 + 订单发货 3）");
        LogisticsRoutes.Select(r => r.Route).Distinct().Should().HaveCount(28);

        // 三组前缀并存。
        LogisticsRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/merchant/address", StringComparison.Ordinal))
            .Should().Be(5, "地址 5 条走 /channels/ec/merchant/address/* 前缀");
        LogisticsRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/merchant/", StringComparison.Ordinal) && !r.StartsWith("/channels/ec/merchant/address", StringComparison.Ordinal))
            .Should().Be(4, "运费模板 4 条走 /channels/ec/merchant/*freight* 前缀");
        LogisticsRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/logistics/ewaybill/biz/", StringComparison.Ordinal))
            .Should().Be(16, "电子面单 16 条走 /channels/ec/logistics/ewaybill/biz/* 前缀");
        LogisticsRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/order/delivery", StringComparison.Ordinal))
            .Should().Be(3, "订单发货 3 条走 /channels/ec/order/delivery* 前缀");

        foreach (var (iface, method, httpAttribute, route) in LogisticsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CL2：Logistics 域 28 端点全部为 POST；除 get_template_config 外均带 [Body]。</summary>
    [Fact]
    public void LogisticsEndpoints_ShouldAllBePost()
    {
        foreach (var (iface, method, _, _) in LogisticsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（物流发货无 GET 端点）");
        }

        // 27 个端点有请求体；get_template_config 官方无请求体（请求体 Request Payload：无）。
        var bodyless = LogisticsRoutes.Where(static r => r.Route.EndsWith("/template/config", StringComparison.Ordinal)).ToArray();
        bodyless.Should().HaveCount(1, "get_template_config 是本域唯一无请求体的端点");

        foreach (var (iface, method, _, route) in LogisticsRoutes.Where(static r => !r.Route.EndsWith("/template/config", StringComparison.Ordinal)))
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().NotBeEmpty($"{iface.Name}.{method} 官方契约有请求体，必须声明 [Body]（{route}）");
        }
    }

    /// <summary>契约守卫 CL3：令牌绑定与注册形态。</summary>
    [Fact]
    public void LogisticsInterface_ShouldBindChannelsToken_AndRegisterUnderLogisticsGroup()
    {
        var token = typeof(IChannelsLogisticsService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Logistics 域 28 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsLogisticsService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(LogisticsRegistryGroupName, "必须挂 Logistics 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Logistics");
        typeof(ChannelsServiceBuilder).GetMethod("AddLogisticsApi").Should().NotBeNull("必须提供 AddLogisticsApi 链式注册方法");
    }

    /// <summary>契约守卫 CL4：Logistics 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void LogisticsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = LogisticsJsonContext.Default;

        var domainTypes = typeof(ChannelsLogisticsAddAddressRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Logistics"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(89,
            "Logistics 域 28 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（地址 10 + 运费模板 14 + 电子面单 55 + 订单发货 10 = 89，口径以 LogisticsJsonContext.g.cs 登记为准）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Logistics 域命名空间，必须登记进 LogisticsJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(LogisticsRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Logistics");
        }

        // 静态常量类（static class ⇔ IsAbstract && IsSealed）不得被误标 [HttpJsonSerializable]。
        var constantClasses = domainTypes.Where(static t => t.IsAbstract && t.IsSealed).ToList();
        constantClasses.Should().BeEmpty("Logistics 域无静态常量类，若后续引入须同批登记本守卫");
    }

    /// <summary>契约守卫 CL5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void LogisticsDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 地址（增改查共用 ChannelsLogisticsAddressDetail / AddressInfo）。
        AssertJsonProperty<ChannelsLogisticsAddAddressRequest>("address_detail", "添加地址请求体");
        AssertJsonProperty<ChannelsLogisticsAddressDetail>("address_id", "地址 id");
        AssertJsonProperty<ChannelsLogisticsAddressInfo>("user_name", "收货人姓名");
        AssertJsonProperty<ChannelsLogisticsAddressInfo>("tel_number", "收货人手机号");
        AssertJsonProperty<ChannelsLogisticsAddressInfo>("detail_info", "详细收货地址");
        AssertJsonProperty<ChannelsLogisticsGetAddressListResponse>("address_id_list", "地址 id 列表");
        AssertJsonProperty<ChannelsLogisticsGetAddressResponse>("address_detail", "地址详情");

        // 运费模板（增改查共用 ChannelsLogisticsFreightTemplate）。
        AssertJsonProperty<ChannelsLogisticsAddFreightTemplateRequest>("freight_template", "运费模板详细信息");
        AssertJsonProperty<ChannelsLogisticsFreightTemplate>("template_id", "模板 id");
        AssertJsonProperty<ChannelsLogisticsFreightTemplate>("valuation_type", "计费类型");
        AssertJsonProperty<ChannelsLogisticsFreightTemplate>("shipping_method", "计费方式");
        AssertJsonProperty<ChannelsLogisticsFreightTemplate>("all_condition_free_detail", "条件包邮详情");
        AssertJsonProperty<ChannelsLogisticsFreightTemplate>("all_freight_calc_method", "具体计费方法");
        AssertJsonProperty<ChannelsLogisticsFreightTemplate>("not_send_area", "不发货区域");
        AssertJsonProperty<ChannelsLogisticsGetFreightTemplateListResponse>("template_id_list", "运费模板 id 列表");

        // 电子面单（create / precreate 共用请求形态 ChannelsLogisticsEwaybillCreateOrderRequest）。
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("ewaybill_order_id", "电子面单订单 id");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("delivery_id", "快递公司 id");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("ewaybill_acct_id", "物流账号编码");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("sender", "寄件人");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("receiver", "收件人");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("ec_order_list", "订单信息");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderRequest>("shop_id", "面单主体 ID");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderResponse>("waybill_id", "快递单号");
        AssertJsonProperty<ChannelsLogisticsEwaybillCreateOrderResponse>("print_info", "打印报文");
        AssertJsonProperty<ChannelsLogisticsEwaybillOrderInfo>("status", "电子面单状态");
        AssertJsonProperty<ChannelsLogisticsEwaybillPath>("status", "轨迹状态");

        // 面单模板（create / update 共用 ChannelsLogisticsEwaybillTemplateInfoRequest）。
        AssertJsonProperty<ChannelsLogisticsEwaybillTemplateInfoRequest>("template_name", "模板名");
        AssertJsonProperty<ChannelsLogisticsEwaybillTemplateInfoRequest>("options", "模板信息选项");
        AssertJsonProperty<ChannelsLogisticsEwaybillTemplateOption>("option_id", "信息类型 0-7");
        AssertJsonProperty<ChannelsLogisticsEwaybillGetTemplateResponse>("total_template", "所有快递公司模板信息汇总");

        // 订单发货（send / compensation 共用 ChannelsLogisticsDeliveryItem）。
        AssertJsonProperty<ChannelsLogisticsSendDeliveryRequest>("order_id", "订单 id");
        AssertJsonProperty<ChannelsLogisticsSendDeliveryRequest>("delivery_list", "物流信息");
        AssertJsonProperty<ChannelsLogisticsDeliveryItem>("waybill_id", "快递单号");
        AssertJsonProperty<ChannelsLogisticsDeliveryItem>("deliver_type", "发货方式");
        AssertJsonProperty<ChannelsLogisticsDeliveryProductInfo>("product_cnt", "商品数量");
        AssertJsonProperty<ChannelsLogisticsDeliveryCompensationRequest>("reason", "补发原因");
        AssertJsonProperty<ChannelsLogisticsGetDeliveryCompanyListResponse>("company_list", "快递公司列表");

        // 共用 DTO 裁决（严禁同字段重复建模）。
        // (1) 地址 get / delete 共用 ChannelsLogisticsAddressIdRequest 请求形态。
        // (2) 电子面单 create / precreate 共用 ChannelsLogisticsEwaybillCreateOrderRequest（预取号复用取号入参）。
        // (3) 面单模板 create / update 共用 ChannelsLogisticsEwaybillTemplateInfoRequest（update 原地覆盖写全字段）。
        // (4) 订单发货 send / compensation 共用 ChannelsLogisticsDeliveryItem 物流项形态。
        typeof(ChannelsLogisticsEwaybillPrecreateOrderResponse).BaseType
            .Should().Be(typeof(ChannelsResponse), "precreate 响应在 ChannelsResponse 上扩展");
    }

    /// <summary>断言类型的 JSON 字段名（官方参数名照抄）。</summary>
    private static void AssertJsonProperty<T>(string expected, string description)
    {
        var propertyNames = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        propertyNames.Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().Contain(expected, $"{typeof(T).Name} 必须按官方参数名建模 {description}（{expected}）");
    }
}
