// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels;
using Mud.Wechat.Channels.DataModels.Aftersale;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「售后管理」域契约守卫（设计方案 v1 P1 / CA 系列：Aftersale 域 27 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域边界（设计方案 v1 §2.2）</b>：售后管理 27 端点全部走 <c>/channels/ec/aftersale/*</c>、
/// 全部 POST：售后单 16 + 纠纷单 4 + 保障单 6 + 全量售后原因 / 拒绝原因。
/// 与 SHIPINHAO 本地生活重叠的 <c>aftersale/getaftersaleorder</c> 只计 1（CH-R2 路由表同源）。
/// </para>
/// <para>
/// <b>空请求体形态</b>：<c>reason/get</c> 与 <c>rejectreason/get</c> 官方为「POST + 空请求体」，
/// 不得人为造请求体 DTO（对齐 Funds 域 getbalance 先例）。
/// </para>
/// </remarks>
public class ChannelsAftersaleContractGuards
{
    private const string AftersaleRegistryGroupName = "Aftersale";

    /// <summary>Aftersale 域官方路由表（27 端点，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AftersaleRoutes =
    {
        // 售后单（14）
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetAftersaleOrderAsync), typeof(PostAttribute), "/channels/ec/aftersale/getaftersaleorder"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetAftersaleListAsync), typeof(PostAttribute), "/channels/ec/aftersale/getaftersalelist"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.AcceptAftersaleAsync), typeof(PostAttribute), "/channels/ec/aftersale/acceptapply"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.RejectAftersaleAsync), typeof(PostAttribute), "/channels/ec/aftersale/rejectapply"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.AcceptExchangeReshipAsync), typeof(PostAttribute), "/channels/ec/aftersale/acceptexchangereship"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.RejectExchangeReshipAsync), typeof(PostAttribute), "/channels/ec/aftersale/rejectexchangereship"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantReshipAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantreship"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantUpdateReshipExpressAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantupdatereshipexpress"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantUpdateAftersaleAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantupdateaftersale"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GenAftersaleOrderAsync), typeof(PostAttribute), "/channels/ec/aftersale/genaftersaleorder"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.RefundPriceDiffAsync), typeof(PostAttribute), "/channels/ec/aftersale/refundpricediff"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetExchangeableSkuListAsync), typeof(PostAttribute), "/channels/ec/aftersale/getexchangeableskulist"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.ApplyVirtualTelNumAsync), typeof(PostAttribute), "/channels/ec/aftersale/applyvirtualtelnum"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.HandleFastExchangeReceiptAsync), typeof(PostAttribute), "/channels/ec/aftersale/handlefastexchangereceipt"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.UploadRefundCertificateAsync), typeof(PostAttribute), "/channels/ec/aftersale/uploadrefundcertificate"),
        // 售后原因 / 拒绝原因（2，官方空请求体）
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetAftersaleReasonAsync), typeof(PostAttribute), "/channels/ec/aftersale/reason/get"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetAftersaleRejectReasonAsync), typeof(PostAttribute), "/channels/ec/aftersale/rejectreason/get"),
        // 纠纷单（4）
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetComplaintOrderAsync), typeof(PostAttribute), "/channels/ec/aftersale/getcomplaintorder"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.AddComplaintMaterialAsync), typeof(PostAttribute), "/channels/ec/aftersale/addcomplaintmaterial"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.AddComplaintProofAsync), typeof(PostAttribute), "/channels/ec/aftersale/addcomplaintproof"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.SyncWorkOrderAsync), typeof(PostAttribute), "/channels/ec/aftersale/syncworkorder"),
        // 保障单（6）
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.GetGuaranteeOrderAsync), typeof(PostAttribute), "/channels/ec/aftersale/getguaranteeorder"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.SearchGuaranteeOrderAsync), typeof(PostAttribute), "/channels/ec/aftersale/searchguaranteeorder"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantAcceptGuaranteeAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantacceptguarantee"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantModifyGuaranteeAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantmodifyguarantee"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantProofGuaranteeAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantproofguarantee"),
        (typeof(IChannelsAftersaleService), nameof(IChannelsAftersaleService.MerchantRefuseGuaranteeAsync), typeof(PostAttribute), "/channels/ec/aftersale/merchantrefuseguarantee"),
    };

    /// <summary>契约守卫 CA1：Aftersale 域 27 端点路由必须与官方契约一致（全部 POST、单一前缀）。</summary>
    [Fact]
    public void AftersaleEndpoints_ShouldMatchOfficialRoutes()
    {
        // 27 端点 = 售后单 15（含原因/拒绝原因 2）+ 纠纷单 4 + 保障单 6 + 售后原因 / 拒绝原因 2
        //（设计方案 v1 §2.2 计数口径；getaftersaleorder 与 SHIPINHAO 重叠只计 1）。
        AftersaleRoutes.Should().HaveCount(27, "Aftersale 域恰 27 端点（设计方案 v1 §2.2）");
        AftersaleRoutes.Select(r => r.Route).Distinct().Should().HaveCount(27);

        AftersaleRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/aftersale/", StringComparison.Ordinal))
            .Should().Be(27, "本域全部端点走 /channels/ec/aftersale/*，无 /shop/* 历史前缀并存");

        foreach (var (iface, method, httpAttribute, route) in AftersaleRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CA2：Aftersale 域 27 端点全部为 POST；除两个官方空请求体端点外必须携带 [Body]。</summary>
    [Fact]
    public void AftersaleEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in AftersaleRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（售后管理无 GET 端点）");
        }

        // 官方「POST + 空请求体」的两端点不得人为造请求体 DTO。
        foreach (var emptyEndpoint in new[]
                 {
                     nameof(IChannelsAftersaleService.GetAftersaleReasonAsync),
                     nameof(IChannelsAftersaleService.GetAftersaleRejectReasonAsync),
                 })
        {
            var target = typeof(IChannelsAftersaleService).GetMethod(emptyEndpoint)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{emptyEndpoint} 官方规定空请求体（拉取全量原因字典）——不得声明 [Body]");
        }

        // 其余 25 端点全部有请求体。
        var withBody = AftersaleRoutes
            .Select(r => r.Method)
            .Except(new[] { nameof(IChannelsAftersaleService.GetAftersaleReasonAsync), nameof(IChannelsAftersaleService.GetAftersaleRejectReasonAsync) });
        foreach (var method in withBody)
        {
            var target = typeof(IChannelsAftersaleService).GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().NotBeEmpty($"{method} 官方契约有请求体，必须声明 [Body]");
        }
    }

    /// <summary>契约守卫 CA3：令牌绑定与注册形态。</summary>
    [Fact]
    public void AftersaleInterface_ShouldBindChannelsToken_AndRegisterUnderAftersaleGroup()
    {
        var token = typeof(IChannelsAftersaleService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Aftersale 域 27 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsAftersaleService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(AftersaleRegistryGroupName, "必须挂 Aftersale 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Aftersale");
        typeof(ChannelsServiceBuilder).GetMethod("AddAftersaleApi").Should().NotBeNull("必须提供 AddAftersaleApi 链式注册方法");
    }

    /// <summary>契约守卫 CA4：Aftersale 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void AftersaleDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = AftersaleJsonContext.Default;

        var domainTypes = typeof(ChannelsGetAftersaleOrderRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Aftersale"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(70,
            "Aftersale 域 27 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（售后单结构族 OrderDtos 24 + 操作请求族 OperationDtos 16 + 纠纷单族 ComplaintDtos 13 + 保障单族 GuaranteeDtos 17 = 70，口径以 AftersaleJsonContext.g.cs 登记为准）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Aftersale 域命名空间，必须登记进 AftersaleJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(AftersaleRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Aftersale");
        }

        // 静态常量类（static class ⇔ IsAbstract && IsSealed）不得被误标 [HttpJsonSerializable]。
        var constantClasses = domainTypes.Where(static t => t.IsAbstract && t.IsSealed).ToList();
        constantClasses.Should().HaveCount(7,
            "Aftersale 域静态常量类计数漂移须核对（售后状态 / 售后原因 / 售后类型 / 售后子类型 / 纠纷状态 / 保障状态 / 保障类型 = 7）");
        constantClasses.Select(static t => t.Name).OrderBy(static n => n, StringComparer.Ordinal).Should().Equal(
            new[]
            {
                nameof(ChannelsAftersaleReasons),
                nameof(ChannelsAftersaleStatuses),
                nameof(ChannelsAftersaleSubTypes),
                nameof(ChannelsAftersaleTypes),
                nameof(ChannelsComplaintStatuses),
                nameof(ChannelsGuaranteeStatuses),
                nameof(ChannelsGuaranteeTypes),
            }.OrderBy(static n => n, StringComparer.Ordinal),
            "常量类集合漂移须先核对官方文档");
        constantClasses.Should().OnlyContain(
            static t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null,
            "常量类只有静态常量、无序列化语义，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CA5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void AftersaleDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 获取售后单（顶层 after_sale_order 结构 + 退款 / 退货 / 换货子结构）。
        AssertJsonProperty<ChannelsGetAftersaleOrderRequest>("after_sale_order_id", "售后单 ID（查询必填）");
        AssertJsonProperty<ChannelsGetAftersaleOrderResponse>("after_sale_order", "售后单结构");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("after_sale_order_id", "售后单号");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("status", "售后单状态");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("order_id", "关联订单号");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("product_info", "售后商品信息");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("refund_info", "退款信息");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("return_info", "退货信息");
        AssertJsonProperty<ChannelsAftersaleOrderInfo>("exchange_info", "换货信息");
        AssertJsonProperty<ChannelsAftersaleProductInfo>("product_id", "商品 id");
        AssertJsonProperty<ChannelsAftersaleProductInfo>("sku_id", "sku id");
        AssertJsonProperty<ChannelsAftersaleProductInfo>("count", "商品数量");

        // 列表 / 原因字典。
        AssertJsonProperty<ChannelsGetAftersaleListRequest>("begin_create_time", "创建时间范围起（24 小时窗口）");
        AssertJsonProperty<ChannelsGetAftersaleListRequest>("end_update_time", "更新时间范围止（24 小时窗口）");
        AssertJsonProperty<ChannelsGetAftersaleListResponse>("after_sale_order_id_list", "售后单号列表");
        AssertJsonProperty<ChannelsGetAftersaleListResponse>("next_key", "分页参数");
        AssertJsonProperty<ChannelsGetAftersaleListResponse>("has_more", "是否还有下一页");
        AssertJsonProperty<ChannelsAftersaleReason>("reason", "售后原因");
        AssertJsonProperty<ChannelsAftersaleRejectReason>("reject_reason_type", "拒绝原因类型");
        AssertJsonProperty<ChannelsAftersaleRejectReason>("reject_reason_type_text", "拒绝原因文案");

        // 同意 / 拒绝 / 换货发货 / 商家协商。
        AssertJsonProperty<ChannelsAcceptAftersaleRequest>("accept_type", "同意类型（1 退货换货、2 仅退款）");
        AssertJsonProperty<ChannelsAcceptAftersaleRequest>("address_id", "退货地址 id（同意退货必传）");
        AssertJsonProperty<ChannelsRejectAftersaleRequest>("reject_reason", "拒绝原因");
        AssertJsonProperty<ChannelsRejectExchangeReshipRequest>("reject_certificates", "拒绝凭证");
        AssertJsonProperty<ChannelsAftersaleReshipRequest>("waybill_id", "快递单号");
        AssertJsonProperty<ChannelsAftersaleReshipRequest>("delivery_id", "快递公司 id");
        AssertJsonProperty<ChannelsHandleFastExchangeReceiptRequest>("reject_confirm_exchange", "拒绝换货确认凭证材料");
        AssertJsonProperty<ChannelsMerchantUpdateAftersaleRequest>("merchant_update_type", "商家协商类型");
        AssertJsonProperty<ChannelsMerchantUpdateAftersaleRequest>("merchant_update_desc", "商家协商说明");

        // 代发起 / 退差价 / 换 SKU / 虚拟号 / 极速换货 / 退款凭证。
        AssertJsonProperty<ChannelsGenAftersaleOrderRequest>("order_id", "订单号（商家代发起售后）");
        AssertJsonProperty<ChannelsRefundPriceDiffRequest>("amount", "退差金额（分）");
        AssertJsonProperty<ChannelsGetExchangeableSkuListRequest>("sku_id", "待换 sku id");
        AssertJsonProperty<ChannelsApplyVirtualTelNumResponse>("virtual_tel_number", "虚拟号");
        AssertJsonProperty<ChannelsHandleFastExchangeReceiptRequest>("act", "极速换货收货操作类型");
        AssertJsonProperty<ChannelsUploadRefundCertificateRequest>("refund_certificates", "退款凭证");

        // 纠纷单。
        AssertJsonProperty<ChannelsGetComplaintOrderRequest>("complaint_id", "纠纷单号");
        AssertJsonProperty<ChannelsGetComplaintOrderResponse>("status", "纠纷单状态");
        AssertJsonProperty<ChannelsGetComplaintOrderResponse>("history", "纠纷处理历史");
        AssertJsonProperty<ChannelsComplaintHistory>("content", "纠纷留言内容");
        AssertJsonProperty<ChannelsAddComplaintMaterialRequest>("complaint_id", "纠纷单号");
        AssertJsonProperty<ChannelsAddComplaintMaterialRequest>("media_id_list", "媒体素材 id 列表");
        AssertJsonProperty<ChannelsAddComplaintProofRequest>("content", "举证文字内容");
        // 裁决：addcomplaintmaterial（留言）与 addcomplaintproof（举证）请求字段集当前一致
        //（complaint_id/content/media_id_list），但为两个独立官方端点、参数表可独立演化，按端点分建不合并。
        AssertJsonProperty<ChannelsAddComplaintProofRequest>("media_id_list", "举证图片 media_id 列表");
        AssertJsonProperty<ChannelsSyncWorkOrderRequest>("complaint_id", "纠纷单号（工单同步归属）");
        AssertJsonProperty<ChannelsSyncWorkOrderRequest>("work_order_info", "工单信息");

        // 保障单。
        AssertJsonProperty<ChannelsGetGuaranteeOrderRequest>("guarantee_order_id", "保障单号");
        AssertJsonProperty<ChannelsGetGuaranteeOrderResponse>("guarantee_order", "保障单结构");
        AssertJsonProperty<ChannelsGuaranteeOrderInfo>("guarantee_order_id", "保障单号");
        AssertJsonProperty<ChannelsGuaranteeOrderInfo>("type", "保障类型");
        AssertJsonProperty<ChannelsGuaranteeOrderInfo>("order_id", "关联订单号");
        AssertJsonProperty<ChannelsGuaranteeOrderInfo>("status", "保障单状态");
        AssertJsonProperty<ChannelsGuaranteeOrderInfo>("fake_one_pay_four_info", "假一赔四保障信息");
        AssertJsonProperty<ChannelsGuaranteeOrderInfo>("bad_pay_info", "坏损包退保障信息");
        AssertJsonProperty<ChannelsSearchGuaranteeOrderRequest>("guarantee_order_id_list", "保障单号列表");
        AssertJsonProperty<ChannelsSearchGuaranteeOrderRequest>("status_list", "保障单状态过滤");
        AssertJsonProperty<ChannelsSearchGuaranteeOrderRequest>("offset", "分页偏移");
        AssertJsonProperty<ChannelsSearchGuaranteeOrderRequest>("limit", "分页条数");

        // 共用 DTO 裁决（严禁同字段重复建模）：
        // acceptexchangereship / merchantreship / merchantupdatereshipexpress 三端点的
        // 「快递公司 + 运单号」发货形态一致 ⇒ 共用 ChannelsAftersaleReshipRequest。
        foreach (var sharedReship in new[]
                 {
                     nameof(IChannelsAftersaleService.AcceptExchangeReshipAsync),
                     nameof(IChannelsAftersaleService.MerchantReshipAsync),
                     nameof(IChannelsAftersaleService.MerchantUpdateReshipExpressAsync),
                 })
        {
            typeof(IChannelsAftersaleService).GetMethod(sharedReship)!
                .GetParameters().Should().Contain(
                    static p => p.ParameterType == typeof(ChannelsAftersaleReshipRequest),
                    $"{sharedReship} 必须共用 ChannelsAftersaleReshipRequest（发货形态一致，不得重复建模）");
        }

        // 响应继承裁决：纯 errcode/errmsg 端点的响应直接复用基线 ChannelsResponse。
        typeof(ChannelsGetAftersaleReasonResponse).IsAssignableTo(typeof(ChannelsResponse))
            .Should().BeTrue("原因字典响应继承基线响应（errcode/errmsg + 业务字段）");
        typeof(ChannelsGetGuaranteeOrderResponse).IsAssignableTo(typeof(ChannelsResponse))
            .Should().BeTrue("保障单响应继承基线响应");
    }

    /// <summary>契约守卫 CA6：Aftersale 域枚举常量漂移锁定（官方原文含历史拼写，照抄不改）。</summary>
    [Fact]
    public void AftersaleEnums_ShouldLockOfficialLiteralValues()
    {
        // 售后单状态（官方原文即大写下划线字符串；Canceld/WaitHandle 为官方历史拼写，照抄勿「纠正」）。
        typeof(ChannelsAftersaleStatuses).GetField(nameof(ChannelsAftersaleStatuses.UserCanceld))!.GetRawConstantValue()
            .Should().Be("USER_CANCELD", "用户取消（官方历史拼写 Canceld 照抄）");
        typeof(ChannelsAftersaleStatuses).GetField(nameof(ChannelsAftersaleStatuses.MerchantProcessing))!.GetRawConstantValue()
            .Should().Be("MERCHANT_PROCESSING", "商家处理中");
        typeof(ChannelsAftersaleStatuses).GetField(nameof(ChannelsAftersaleStatuses.UserWaitHandleMerchantAfterSale))!.GetRawConstantValue()
            .Should().Be("USER_WAIT_HANDLE_MERCHANT_AFTER_SALE", "待用户处理商家售后");
        typeof(ChannelsAftersaleStatuses).GetField(nameof(ChannelsAftersaleStatuses.MerchantRefundSuccess))!.GetRawConstantValue()
            .Should().Be("MERCHANT_REFUND_SUCCESS", "商家退款成功");
        typeof(ChannelsAftersaleStatuses).GetField(nameof(ChannelsAftersaleStatuses.MerchantExchangeSuccess))!.GetRawConstantValue()
            .Should().Be("MERCHANT_EXCHANGE_SUCCESS", "商家换货成功");

        // 售后原因。
        typeof(ChannelsAftersaleReasons).GetField(nameof(ChannelsAftersaleReasons.IncorrectSelection))!.GetRawConstantValue()
            .Should().Be("INCORRECT_SELECTION", "选错/多选");
        typeof(ChannelsAftersaleReasons).GetField(nameof(ChannelsAftersaleReasons.NoReason7Days))!.GetRawConstantValue()
            .Should().Be("NO_REASON_7_DAYS", "七天无理由");
        typeof(ChannelsAftersaleReasons).GetField(nameof(ChannelsAftersaleReasons.InitiateByPlatform))!.GetRawConstantValue()
            .Should().Be("INITIATE_BY_PLATFORM", "平台发起");

        // 售后类型 / 子类型。
        typeof(ChannelsAftersaleTypes).GetField(nameof(ChannelsAftersaleTypes.Refund))!.GetRawConstantValue()
            .Should().Be("REFUND", "仅退款");
        typeof(ChannelsAftersaleTypes).GetField(nameof(ChannelsAftersaleTypes.Return))!.GetRawConstantValue()
            .Should().Be("RETURN", "退货退款");
        typeof(ChannelsAftersaleTypes).GetField(nameof(ChannelsAftersaleTypes.Exchange))!.GetRawConstantValue()
            .Should().Be("EXCHANGE", "换货");
        typeof(ChannelsAftersaleTypes).GetField(nameof(ChannelsAftersaleTypes.Reship))!.GetRawConstantValue()
            .Should().Be("RESHIP", "补寄");
        typeof(ChannelsAftersaleSubTypes).GetField(nameof(ChannelsAftersaleSubTypes.Default))!.GetRawConstantValue()
            .Should().Be("DEFAULT", "默认售后");
        typeof(ChannelsAftersaleSubTypes).GetField(nameof(ChannelsAftersaleSubTypes.RefundPriceDiff))!.GetRawConstantValue()
            .Should().Be("REFUND_PRICE_DIFF", "退差价");

        // 纠纷单状态（数值）。
        typeof(ChannelsComplaintStatuses).GetField(nameof(ChannelsComplaintStatuses.WaitMerchantHandle))!.GetRawConstantValue()
            .Should().Be(100, "待商家处理");
        typeof(ChannelsComplaintStatuses).GetField(nameof(ChannelsComplaintStatuses.WaitCustomerServiceHandle))!.GetRawConstantValue()
            .Should().Be(101, "待客服处理");
        typeof(ChannelsComplaintStatuses).GetField(nameof(ChannelsComplaintStatuses.CancelCustomerServiceIntervention))!.GetRawConstantValue()
            .Should().Be(102, "客服终止介入");

        // 保障单状态 / 类型（COMFIRM 为官方历史拼写，照抄勿「纠正」）。
        typeof(ChannelsGuaranteeStatuses).GetField(nameof(ChannelsGuaranteeStatuses.WaitBothProof))!.GetRawConstantValue()
            .Should().Be("STATUS_WAIT_BOTH_PROOF", "双方待举证");
        typeof(ChannelsGuaranteeStatuses).GetField(nameof(ChannelsGuaranteeStatuses.WaitOpConfirm))!.GetRawConstantValue()
            .Should().Be("STATUS_WAIT_OP_COMFIRM", "待微信支付操作确认（官方历史拼写 COMFIRM 照抄）");
        typeof(ChannelsGuaranteeStatuses).GetField(nameof(ChannelsGuaranteeStatuses.PaySucc))!.GetRawConstantValue()
            .Should().Be("STATUS_PAY_SUCC", "支付成功");
        typeof(ChannelsGuaranteeStatuses).GetField(nameof(ChannelsGuaranteeStatuses.UserCancel))!.GetRawConstantValue()
            .Should().Be("STATUS_USER_CANCEL", "用户取消");
        typeof(ChannelsGuaranteeTypes).GetField(nameof(ChannelsGuaranteeTypes.FakeOnePayFour))!.GetRawConstantValue()
            .Should().Be(1, "假一赔四");
        typeof(ChannelsGuaranteeTypes).GetField(nameof(ChannelsGuaranteeTypes.BadPay))!.GetRawConstantValue()
            .Should().Be(2, "坏损包退");
    }

    /// <summary>断言类型的 JSON 字段名（官方参数名照抄）。</summary>
    private static void AssertJsonProperty<T>(string expected, string description)
    {
        var propertyNames = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        propertyNames.Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().Contain(expected, $"{typeof(T).Name} 必须按官方参数名建模 {description}（{expected}）");
    }
}
