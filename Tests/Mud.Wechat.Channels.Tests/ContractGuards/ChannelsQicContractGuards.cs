// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Qic;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「质检管理」域契约守卫（CQ 系列：Qic 域 5 端点路由 + GET/POST 动词混合照抄官方 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 质检码 camelCase 照录 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>动词混合（照抄官方）</b>：config/get 与 submitconfig/get 为 GET，code/print、submit、
/// register_logistics 为 POST——不得「统一规范」改动词。
/// </para>
/// <para>
/// <b>质检码响应 camelCase 照录</b>：printinspectcode 的 data 对象字段为 camelCase
/// （backupDeliveryId / boxDTOList 等），与小店其余端点 snake_case 不同，字段名照抄官方原文。
/// </para>
/// </remarks>
public class ChannelsQicContractGuards
{
    private const string QicRegistryGroupName = "Qic";

    /// <summary>Qic 域官方路由表（5 端点：GET 2 + POST 3，全部 /channels/ec/qic/*）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] QicRoutes =
    {
        (typeof(IChannelsQicService), nameof(IChannelsQicService.GetInspectConfigAsync), typeof(GetAttribute), "/channels/ec/qic/inspect/config/get"),
        (typeof(IChannelsQicService), nameof(IChannelsQicService.GetInspectSubmitConfigAsync), typeof(GetAttribute), "/channels/ec/qic/inspect/submitconfig/get"),
        (typeof(IChannelsQicService), nameof(IChannelsQicService.PrintInspectCodeAsync), typeof(PostAttribute), "/channels/ec/qic/inspect/code/print"),
        (typeof(IChannelsQicService), nameof(IChannelsQicService.SubmitInspectInfoAsync), typeof(PostAttribute), "/channels/ec/qic/inspect/submit"),
        (typeof(IChannelsQicService), nameof(IChannelsQicService.RegisterInspectLogisticsAsync), typeof(PostAttribute), "/channels/ec/qic/inspect/register_logistics"),
    };

    /// <summary>契约守卫 CQ1：Qic 域 5 端点路由必须与官方契约一致（GET 2 + POST 3）。</summary>
    [Fact]
    public void QicEndpoints_ShouldMatchOfficialRoutes()
    {
        // 5 端点 = 质检仓配置 1 + 送检配置模板 1 + 质检码 1 + 绑定送检 1 + 自寄送检 1。
        QicRoutes.Should().HaveCount(5, "质检仓 / 送检配置模板 / 质检码 / 绑定送检 / 自寄送检 = 5 端点");
        QicRoutes.Select(r => r.Route).Distinct().Should().HaveCount(5);

        QicRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/qic/", StringComparison.Ordinal))
            .Should().Be(5, "5 端点全部走 /channels/ec/qic/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in QicRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（下划线路由 register_logistics 照抄官方）");
        }
    }

    /// <summary>契约守卫 CQ2：动词照抄官方（GET 2 + POST 3）；GET 端点不得携带 [Body]；POST 端点必须携带 [Body]。</summary>
    [Fact]
    public void QicEndpoints_ShouldMatchOfficialVerbs_WithBodyRules()
    {
        // 官方 GET 端点：config/get 与 submitconfig/get（官方页写 GET）。
        var getRoutes = QicRoutes.Where(r => r.HttpAttribute == typeof(GetAttribute)).ToArray();
        getRoutes.Should().HaveCount(2, "getinspectconfig 与 getinspectsubmitconfig 官方为 GET");
        var postRoutes = QicRoutes.Where(r => r.HttpAttribute == typeof(PostAttribute)).ToArray();
        postRoutes.Should().HaveCount(3, "printinspectcode / submitinspectinfo / registerlogistics 官方为 POST");

        foreach (var (iface, method, _, _) in QicRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            var bodyParams = target!.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();

            if (target.GetCustomAttribute<GetAttribute>() != null)
            {
                bodyParams.Should().BeEmpty(
                    $"{method} 官方为 GET 无请求体——不得声明 [Body]");
            }
            else
            {
                bodyParams.Should().NotBeEmpty(
                    $"{method} 官方为 POST 且有请求体，必须声明 [Body]");
            }
        }

        // submitconfig/get 的 order_id 官方规定走 URL 参数（2025-11-13 变更日志由 order_id_list 改为 order_id）。
        var submitConfigEndpoint = typeof(IChannelsQicService).GetMethod(nameof(IChannelsQicService.GetInspectSubmitConfigAsync))!;
        submitConfigEndpoint.GetParameters()
            .Should().Contain(p => p.GetCustomAttribute<QueryAttribute>() != null && p.GetCustomAttribute<QueryAttribute>()!.Name == "order_id",
                "getinspectsubmitconfig 官方契约 order_id 走 URL 参数");
    }

    /// <summary>契约守卫 CQ3：令牌绑定与注册形态。</summary>
    [Fact]
    public void QicInterface_ShouldBindChannelsToken_AndRegisterUnderQicGroup()
    {
        var token = typeof(IChannelsQicService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Qic 域 5 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsQicService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(QicRegistryGroupName, "必须挂 Qic 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Qic");
        typeof(ChannelsServiceBuilder).GetMethod("AddQicApi").Should().NotBeNull("必须提供 AddQicApi 链式注册方法");
    }

    /// <summary>契约守卫 CQ4：Qic 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void QicDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = QicJsonContext.Default;

        var domainTypes = typeof(ChannelsGetInspectConfigResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Qic"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(17,
            "Qic 域 5 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（质检仓配置 3 + 送检配置模板 6 + 质检码 4 + 绑定送检 2 + 自寄送检 2 = 17）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Qic 域命名空间，必须登记进 QicJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(QicRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Qic");
        }
    }

    /// <summary>契约守卫 CQ5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决 + camelCase 照录）。</summary>
    [Fact]
    public void QicDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 查询质检仓配置。
        AssertJsonProperty<ChannelsGetInspectConfigResponse>("inspect_config", "质检仓配置信息");
        AssertJsonProperty<ChannelsInspectConfig>("warehouse_id", "质检仓ID");
        AssertJsonProperty<ChannelsInspectConfig>("delivery_address", "发货地址");
        AssertJsonProperty<ChannelsInspectConfig>("return_address", "退货地址");
        AssertJsonProperty<ChannelsInspectConfig>("warehouse_addr", "质检仓地址");
        AssertJsonProperty<ChannelsInspectAddress>("contact_name", "联系人");
        AssertJsonProperty<ChannelsInspectAddress>("contact_phone", "联系电话");
        AssertJsonProperty<ChannelsInspectAddress>("county", "区");

        // 查询送检配置模板信息。
        AssertJsonProperty<ChannelsGetInspectSubmitConfigResponse>("submit_config", "送检配置模板信息");
        AssertJsonProperty<ChannelsInspectSubmitConfig>("delivery_list", "快递公司列表");
        AssertJsonProperty<ChannelsInspectSubmitConfig>("inspect_org_list", "质检机构列表");
        AssertJsonProperty<ChannelsInspectSubmitConfig>("charge_url", "资费标准");
        AssertJsonProperty<ChannelsInspectDelivery>("delivery_products", "快递产品列表");
        AssertJsonProperty<ChannelsInspectDeliveryProduct>("enable_insure", "是否支持保价");
        AssertJsonProperty<ChannelsInspectInsureType>("upper_limit_type", "上限类型");
        AssertJsonProperty<ChannelsInspectInsureType>("upper_limit_amount", "上限金额（分）");
        AssertJsonProperty<ChannelsInspectOrg>("org_category", "质检机构类别");

        // 打印质检码（camelCase 照录）。
        AssertJsonProperty<ChannelsPrintInspectCodeRequest>("order_id", "订单id");
        AssertJsonProperty<ChannelsPrintInspectCodeResponse>("data", "质检码详情");
        AssertJsonProperty<ChannelsInspectCodeDetail>("backupDeliveryId", "备用发货快递编码（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectCodeDetail>("boxDTOList", "包装盒列表（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectCodeDetail>("channelAppId", "视频号店铺appid（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectCodeDetail>("inspectCode", "质检码（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectCodeDetail>("printInfo", "打印模板数据和模板html地址（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectCodeDetail>("needLabel", "是否需要打标（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectBox>("boxId", "包装盒id（camelCase 照录）");
        AssertJsonProperty<ChannelsInspectBox>("boxNum", "包装盒数量（camelCase 照录）");

        // 绑定送检信息。
        AssertJsonProperty<ChannelsSubmitInspectRequest>("order_id", "订单id");
        AssertJsonProperty<ChannelsSubmitInspectRequest>("inspect_info", "送检信息");
        AssertJsonProperty<ChannelsInspectInfo>("delivery_id", "发货快递公司编码");
        AssertJsonProperty<ChannelsInspectInfo>("backup_delivery_id", "备用发货快递公司编码");
        AssertJsonProperty<ChannelsInspectInfo>("express_insure", "是否物流保价");
        AssertJsonProperty<ChannelsInspectInfo>("express_insure_amount", "保价金额（分）");
        AssertJsonProperty<ChannelsInspectInfo>("express_merge", "是否合并发货");
        AssertJsonProperty<ChannelsInspectInfo>("inspect_org_id", "珠宝质检机构id");
        AssertJsonProperty<ChannelsInspectInfo>("refund_intercept", "售后拦截策略");
        AssertJsonProperty<ChannelsInspectInfo>("delivery_product_id", "发货快递公司产品id");
        AssertJsonProperty<ChannelsInspectInfo>("backup_delivery_product_id", "备用发货快递公司产品id");
        AssertJsonProperty<ChannelsInspectInfo>("backup_express_insure", "备用发货是否物流保价");
        AssertJsonProperty<ChannelsInspectInfo>("agarwood_inspect_org_id", "沉香质检机构编号");

        // 自寄快递送检。
        AssertJsonProperty<ChannelsRegisterInspectLogisticsRequest>("order_id_list", "订单号列表");
        AssertJsonProperty<ChannelsRegisterInspectLogisticsRequest>("logistics_info", "快递物流信息");
        AssertJsonProperty<ChannelsInspectLogisticsInfo>("waybill_id", "快递单号");
        AssertJsonProperty<ChannelsInspectLogisticsInfo>("delivery_type", "发货方式（固定 1 自寄快递发货）");

        // 共用 DTO 裁决 1：delivery_address 与 return_address 字段集一致 ⇒ 共用 ChannelsInspectAddress。
        typeof(ChannelsInspectConfig).GetProperty(nameof(ChannelsInspectConfig.DeliveryAddress))!
            .PropertyType.Should().Be(typeof(ChannelsInspectAddress));
        typeof(ChannelsInspectConfig).GetProperty(nameof(ChannelsInspectConfig.ReturnAddress))!
            .PropertyType.Should().Be(typeof(ChannelsInspectAddress));

        // 共用 DTO 裁决 2：绑定送检 / 自寄送检响应均为基线 ChannelsResponse（仅 errcode/errmsg），无独立响应 DTO。
        typeof(IChannelsQicService).GetMethod(nameof(IChannelsQicService.SubmitInspectInfoAsync))!.ReturnType
            .Should().Be(typeof(Task<ChannelsResponse>));
        typeof(IChannelsQicService).GetMethod(nameof(IChannelsQicService.RegisterInspectLogisticsAsync))!.ReturnType
            .Should().Be(typeof(Task<ChannelsResponse>));

        // 分建裁决：质检码 data 对象为 camelCase 命名空间，与送检配置 snake_case 形态分建、字段名互不借用。
        typeof(ChannelsInspectCodeDetail).GetProperties()
            .All(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name != "delivery_id")
            .Should().BeTrue("质检码 data 对象字段为 camelCase（deliveryId），不存在 snake_case 的 delivery_id 字段");
    }

    /// <summary>契约守卫 CQ6：枚举常量字面量锁定。</summary>
    [Fact]
    public void QicEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsInspectOrgCategories.Jewelry.Should().Be(0, "官方原文「0-珠宝质检机构」");
        ChannelsInspectOrgCategories.Agarwood.Should().Be(1, "官方原文「1-沉香质检机构」");

        ChannelsInspectInsureSupports.Supported.Should().Be(1, "官方原文「1-支持」");
        ChannelsInspectInsureSupports.Unsupported.Should().Be(2, "官方原文「2-不支持」");

        ChannelsInspectUpperLimitTypes.Limited.Should().Be(1, "官方原文「1-有上限」");
        ChannelsInspectUpperLimitTypes.Unlimited.Should().Be(2, "官方原文「2-无上限」");

        ChannelsInspectRefundIntercepts.InterceptOnAfterSale.Should().Be(0, "官方原文「0-发生售后即拦截」");
        ChannelsInspectRefundIntercepts.IgnorePartialRefund.Should().Be(1, "官方原文「1-部分退款不拦截」");

        ChannelsInspectDeliveryTypes.SelfExpress.Should().Be(1, "官方原文「1: 自寄快递发货」");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
