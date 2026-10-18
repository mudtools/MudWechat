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
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.AdvancedApi;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 企业微信「高级接口订单与代付」（<c>/cgi-bin/advanced_api/*</c>）域契约守卫（AA 系列）：
/// 6 端点路由表（官方全 POST）、服务商凭证面、接口层级、DTO 上下文登记与官方字段名陷阱。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方反直觉点（勿「顺手修正」）</b>：① 六支端点<b>全部为 POST</b>，含三支查询接口
/// （<c>list_order</c> / <c>get_order</c> / <c>get_corp_buy_info</c>）；
/// ② 本域 <c>has_more</c> 官方为<b>整型 0/1</b>，而「高级功能申请单」域（AF）的 <c>has_more</c> 为<b>布尔</b>
/// —— 两域形态各异、SDK 不归一；
/// ③ 订单详情响应的 <c>custom_corpid</c> 官方返回<b>加密后</b>的企业 id，与请求侧明文同源字段名同义不同；
/// ④ 取消订单与提交支付两支成功响应<b>无业务字段</b> ⇒ 直接复用 <see cref="WechatWorkResponse"/>，不新建空 DTO；
/// ⑤ 聊史高级接口的版本周期信息为 <c>begin_time</c>/<c>end_time</c> 双段（旧版 / 目标版各一对象），
/// 与购买信息侧的 <c>edition_list[]</c> 容器名不同。
/// </para>
/// <para>
/// <b>高危不可逆写面</b>：<c>create_order</c> 产生真实订单、<c>submit_pay</c> 提交后官方明确
/// 「无法再变更支付方式」⇒ 两支均无自动重试，且必须置于调用方幂等 / 审批保护之下（AA3 锁定写面返回形态与
/// 端点集合，防「补重试」类改动误入本域）。
/// </para>
/// <para>
/// <b>个人信息约束</b>：<c>buyer_userid</c> / <c>payer_userid</c> 为成员身份，<c>paid_price</c> /
/// <c>original_price</c> 为交易金额，均不得写入日志、遥测或异常消息。
/// </para>
/// </remarks>
public class WechatAdvancedApiContractGuards
{
    private const string AdvancedApiRegistryGroupName = "AdvancedApi";

    private const string AdvancedApiNamespace = "Mud.Wechat.Work.DataModels.AdvancedApi";

    /// <summary>高级接口订单域官方路由表（6 端点，全部 POST）。</summary>
    private static readonly (string Method, Type HttpAttribute, string Route, Type Response)[] AdvancedApiRoutes =
    {
        (nameof(IWechatWorkAdvancedApiService.CreateOrderAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_api/create_order", typeof(CreateAdvancedApiOrderResponse)),
        (nameof(IWechatWorkAdvancedApiService.CancelOrderAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_api/cancel_order", typeof(WechatWorkResponse)),
        (nameof(IWechatWorkAdvancedApiService.SubmitOrderPayAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_api/submit_pay", typeof(WechatWorkResponse)),
        (nameof(IWechatWorkAdvancedApiService.ListOrdersAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_api/list_order", typeof(ListAdvancedApiOrdersResponse)),
        (nameof(IWechatWorkAdvancedApiService.GetOrderAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_api/get_order", typeof(GetAdvancedApiOrderResponse)),
        (nameof(IWechatWorkAdvancedApiService.GetCorpBuyInfoAsync),
            typeof(PostAttribute), "/cgi-bin/advanced_api/get_corp_buy_info", typeof(GetAdvancedApiCorpBuyInfoResponse)),
    };

    // ------------------------------------------------------------------
    // AA1：路由、HTTP 动词与响应形态必须与官方契约一致。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AA1：本域恒为 6 支端点且<b>全部 POST</b>；路由、请求体与响应类型不得漂移；
    /// 取消订单 / 提交支付两支成功响应无业务字段（复用 <see cref="WechatWorkResponse"/>）。
    /// </summary>
    [Fact]
    public void AdvancedApiEndpoints_ShouldMatchOfficialRoutesAndPostOnlyShape()
    {
        AdvancedApiRoutes.Should().HaveCount(6,
            "高级接口订单面官方仅 create_order / cancel_order / submit_pay / list_order / get_order / get_corp_buy_info");
        AdvancedApiRoutes.Should().OnlyContain(r => r.HttpAttribute == typeof(PostAttribute),
            "官方六支（含三支查询）恒为 POST —— 勿「顺手」把查询改 GET");

        var iface = typeof(IWechatWorkAdvancedApiService);
        foreach (var (method, httpAttribute, route, response) in AdvancedApiRoutes)
        {
            var target = iface.GetMethod(method,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须声明于本域父接口");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");

            target.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(response),
                $"{iface.Name}.{method} 的响应形态必须与官方契约一致");

            target.GetParameters().Should().Contain(p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{iface.Name}.{method} 必须以 [Body] 承载请求体（官方六支均为 JSON 请求体）");
        }

        // 写面订单主键：create_order 应答 order_id，cancel_order / submit_pay 请求体回传同一 order_id。
        JsonNameShouldBe(typeof(CreateAdvancedApiOrderResponse),
            nameof(CreateAdvancedApiOrderResponse.OrderId), "order_id");
        JsonNameShouldBe(typeof(CancelAdvancedApiOrderRequest),
            nameof(CancelAdvancedApiOrderRequest.OrderId), "order_id");
        JsonNameShouldBe(typeof(SubmitAdvancedApiOrderPayRequest),
            nameof(SubmitAdvancedApiOrderPayRequest.OrderId), "order_id");
    }

    // ------------------------------------------------------------------
    // AA2：接口层级与注册组（唯一服务商子接口）。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AA2：官方六支端点全部以 <c>provider_access_token</c> 鉴权 ⇒
    /// 继承链上恰好只有唯一服务商子接口，父接口承载全部端点、子接口零端点。
    /// </summary>
    [Fact]
    public void AdvancedApiInterfaceHierarchy_ShouldConvergeOnProviderOnlyChild()
    {
        var assignable = typeof(IWechatWorkAdvancedApiService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t != typeof(IWechatWorkAdvancedApiService)
                        && typeof(IWechatWorkAdvancedApiService).IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkProviderAdvancedApiService) },
            "本域官方只开放服务商凭证面，不得出现自建 / 第三方子接口");

        var parent = typeof(IWechatWorkAdvancedApiService);
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.TokenManage.Should().Be(nameof(IWechatAppManager), "各域父接口 TokenManage 形态统一");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(6, $"{parent.Name} 承载本域全部 6 条官方端点");

        var child = typeof(IWechatWorkProviderAdvancedApiService);
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(AdvancedApiRegistryGroupName,
            $"{child.Name} 必须挂 {AdvancedApiRegistryGroupName} 注册组");
        childApi.InheritedFrom.Should().Be("WechatWorkAdvancedApiService",
            $"{child.Name} 必须继承父接口生成实现类");
        childApi.TokenManage.Should().Be(nameof(IWechatAppManager),
            "父子两级 TokenManage 必须统一（否则生成器以 new 隐藏基类切换成员）");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty($"{child.Name} 为标记子接口，端点全部由父接口承载");
    }

    // ------------------------------------------------------------------
    // AA3：服务商凭证面 + 高危写面端点集合。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AA3：父子两级均声明 <c>provider_access_token</c>（Query、无归属域键）；
    /// 写面三支（create_order / cancel_order / submit_pay）路由固定，防「补重试 / 改幂等键」类改动误入。
    /// </summary>
    [Fact]
    public void AdvancedApiInterfaces_ShouldDeclareProviderAccessTokenAndFixedWriteFace()
    {
        foreach (var iface in new[]
                 {
                     typeof(IWechatWorkAdvancedApiService), typeof(IWechatWorkProviderAdvancedApiService),
                 })
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]（官方以 provider_access_token 鉴权）");
            token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
                $"{iface.Name} 的令牌类型必须为服务商令牌");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 令牌注入须为 Query（企微官方契约，MUD005 已知接受风险）");
            token.Name.Should().Be("provider_access_token", $"{iface.Name} 的官方参数名不得漂移");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
                "令牌类型已无歧义，不得叠加归属域键（TO 系列同规则，永不增设 @Provider/@Suite）");
            token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
                "令牌类型已无歧义，不得叠加归属域键（TO 系列同规则）");
        }

        AdvancedApiRoutes
            .Where(r => r.Route.EndsWith("create_order") || r.Route.EndsWith("cancel_order")
                        || r.Route.EndsWith("submit_pay"))
            .Select(r => r.Route)
            .Should().BeEquivalentTo(new[]
            {
                "/cgi-bin/advanced_api/create_order",
                "/cgi-bin/advanced_api/cancel_order",
                "/cgi-bin/advanced_api/submit_pay",
            }, "高危写面恒为此三支（真实订单 + 不可变更支付方式的扣款任务），无自动重试语义");
    }

    // ------------------------------------------------------------------
    // AA4：DTO 上下文登记与官方字段名陷阱。
    // ------------------------------------------------------------------

    /// <summary>
    /// 契约守卫 AA4：本域 17 个顶层 DTO 必须登记进 <c>AdvancedApiJsonContext</c>，
    /// <c>SerializerClassName</c> 统一为域段 <c>AdvancedApi</c>；同步锁定官方字段名与类型形态陷阱。
    /// </summary>
    [Fact]
    public void AdvancedApiDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = AdvancedApiJsonContext.Default;

        var endpointContractTypes = new Type[]
        {
            typeof(CreateAdvancedApiOrderRequest), typeof(AdvancedApiChatArchiveOrderInfo),
            typeof(AdvancedApiEditionPeriodInfo), typeof(CreateAdvancedApiOrderResponse),
            typeof(CancelAdvancedApiOrderRequest), typeof(SubmitAdvancedApiOrderPayRequest),
            typeof(ListAdvancedApiOrdersRequest), typeof(ListAdvancedApiOrdersResponse),
            typeof(AdvancedApiOrderBrief), typeof(GetAdvancedApiOrderRequest),
            typeof(GetAdvancedApiOrderResponse), typeof(AdvancedApiOrderDetail),
            typeof(AdvancedApiOrderArchiveDetail), typeof(GetAdvancedApiCorpBuyInfoRequest),
            typeof(GetAdvancedApiCorpBuyInfoResponse), typeof(AdvancedApiCorpBuyInfo),
            typeof(AdvancedApiBoughtEdition),
        };

        endpointContractTypes.Should().HaveCount(17, "本域契约面类型清单漂移须先核对官方页面再同批调整本守卫");

        var domainTypes = typeof(CreateAdvancedApiOrderResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == AdvancedApiNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(17,
            "高级接口订单域契约面类型数漂移须先核对官方页面再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于高级接口订单域命名空间，必须登记进 AdvancedApiJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("AdvancedApi",
                $"{type.Name} 的 SerializerClassName 必须为高级接口订单域段 AdvancedApi");
        }

        // 官方字段名陷阱锁定（照抄原文，勿「顺手驼峰化」）。
        JsonNameShouldBe(typeof(ListAdvancedApiOrdersRequest), nameof(ListAdvancedApiOrdersRequest.AdvancedApiType), "advanced_api_type");
        JsonNameShouldBe(typeof(CreateAdvancedApiOrderRequest), nameof(CreateAdvancedApiOrderRequest.BuyerUserId), "buyer_userid");
        JsonNameShouldBe(typeof(SubmitAdvancedApiOrderPayRequest), nameof(SubmitAdvancedApiOrderPayRequest.PayerUserId), "payer_userid");
        JsonNameShouldBe(typeof(AdvancedApiChatArchiveOrderInfo), nameof(AdvancedApiChatArchiveOrderInfo.TakeEffectTime), "take_effect_time");
        JsonNameShouldBe(typeof(AdvancedApiChatArchiveOrderInfo), nameof(AdvancedApiChatArchiveOrderInfo.OldEditionInfo), "old_edition_info");
        JsonNameShouldBe(typeof(AdvancedApiChatArchiveOrderInfo), nameof(AdvancedApiChatArchiveOrderInfo.TargetEditionInfo), "target_edition_info");
        JsonNameShouldBe(typeof(AdvancedApiOrderDetail), nameof(AdvancedApiOrderDetail.ChatArchiveApi), "chat_archive_api");
        JsonNameShouldBe(typeof(AdvancedApiOrderDetail), nameof(AdvancedApiOrderDetail.PaidPrice), "paid_price");
        JsonNameShouldBe(typeof(AdvancedApiOrderArchiveDetail), nameof(AdvancedApiOrderArchiveDetail.PurchaseDurationDays), "purchase_duration_days");
        JsonNameShouldBe(typeof(AdvancedApiOrderArchiveDetail), nameof(AdvancedApiOrderArchiveDetail.OriginalPrice), "original_price");
        JsonNameShouldBe(typeof(GetAdvancedApiCorpBuyInfoResponse), nameof(GetAdvancedApiCorpBuyInfoResponse.ChatArchiveApiBuyInfo), "chat_archive_api_buy_info");
        JsonNameShouldBe(typeof(AdvancedApiCorpBuyInfo), nameof(AdvancedApiCorpBuyInfo.EditionList), "edition_list");

        // 本域 has_more 为整型 0/1（AF 域为布尔）；custom_corpid 在详情响应为加密后 corpid。
        typeof(ListAdvancedApiOrdersResponse).GetProperty(nameof(ListAdvancedApiOrdersResponse.HasMore))!
            .PropertyType.Should().Be(typeof(int?),
                "官方本域 has_more 为整型 0/1（高级功能申请单域为布尔，勿归一）");
        JsonNameShouldBe(typeof(AdvancedApiOrderDetail), nameof(AdvancedApiOrderDetail.CustomCorpId), "custom_corpid");
    }

    /// <summary>JSON 字段名断言（属性映射的官方字段名必须与官方原文一致）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
