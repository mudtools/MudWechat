// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MarketingBusifavor;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-3 营销批次「商家券」域契约守卫 —— <b>15 端点</b>：批次 CRUD 与预算（4）+ 券状态机（5）+ 自定义券码（1）+ 用户券（2）+ 回调地址（2）+ 商品券发券（1）。
/// </summary>
/// <remarks>
/// <para><b>批次三写支动词不统一</b>：创建 POST、<b>改批次与改预算都是 PATCH</b>（不是 PUT）、自定义券码上传是 <c>POST /stocks/{stock_id}/couponcodes</c> ⇒ 动词集合含 PATCH 是本域特征（MBF-B1）。</para>
/// <para><b>「双面同表 ⇒ 一面一方法」在本域是 13/15</b>：15 支里 13 支同时有【普通商户】与【普通服务商】两页且字段表逐字段同形，只有商品券发券与用户券详情两支单面 ⇒ 守卫锁方法数 15 与 (方法, 路由) 数 15 相等（无同路由分身）。</para>
/// <para><b>用户券两支把 <c>openid</c> 放在 path</b>，单券查询还要 path 上的 <c>coupon_code</c> 与 <c>appid</c> ⇒ 三元组定位一张券，少任一即 404。</para>
/// <para><b>券状态机五支（核销 / 退货 / 作废 / 关联补贴 / 取消关联）都是 POST 动词段且互不覆盖</b>：「退款自动退券」与「主动退券」是两条路径，不得合并（MBF-B2）。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayMarketingBusifavorContractGuards
{
    private const string RegistryGroupName = "MarketingBusifavor";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingBusifavor";

    private static readonly Type Service = typeof(IWechatPayMarketingBusifavorService);

    /// <summary>MBF-B1：拓扑 —— 一个注册组一个接口、15 个方法、15 条 (方法, 路由)、动词集 GET / POST / PATCH；路由前缀 /v3/marketing/busifavor/。</summary>
    [Fact]
    public void BusifavorTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(15, "商家券声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(15, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute", "PatchAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/marketing/busifavor/", StringComparison.Ordinal),
            "本域路由全在 /v3/marketing/busifavor/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>MBF-B2：15 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void BusifavorEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorCallbackAsync)).Should().Be("/v3/marketing/busifavor/callbacks");
        QueryNamesOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorCallbackAsync)).Should().BeEquivalentTo(new[] { "mchid" });
        // 商家券回调地址｜官方页 merchant/4012465507 + partner/4012465557｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.SetBusifavorCallbackAsync)).Should().Be("/v3/marketing/busifavor/callbacks");
        // 设置商家券回调地址｜官方页 merchant/4012465431 + partner/4012465425｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.AssociateBusifavorCouponAsync)).Should().Be("/v3/marketing/busifavor/coupons/associate");
        // 商家券关联订单补贴｜官方页 merchant/4012465596 + partner/4012465640｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.DisassociateBusifavorCouponAsync)).Should().Be("/v3/marketing/busifavor/coupons/disassociate");
        // 商家券取消关联订单补贴｜官方页 merchant/4012465673 + partner/4012465703｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.DeactivateBusifavorCouponAsync)).Should().Be("/v3/marketing/busifavor/coupons/deactivate");
        // 作废商家券｜官方页 merchant/4012465890 + partner/4012465924｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.ReturnBusifavorCouponAsync)).Should().Be("/v3/marketing/busifavor/coupons/return");
        // 商家券退款退券｜官方页 merchant/4012465845 + partner/4012465865｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.UseBusifavorCouponAsync)).Should().Be("/v3/marketing/busifavor/coupons/use");
        // 核销商家券｜官方页 merchant/4012465292 + partner/4012465359｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.SendBusifavorProductCouponAsync)).Should().Be("/v3/marketing/busifavor/product-coupons/send");
        // 向用户发放商品券｜官方页 merchant/4015717371｜身份 商户

        RouteOf(nameof(IWechatPayMarketingBusifavorService.CreateBusifavorStockAsync)).Should().Be("/v3/marketing/busifavor/stocks");
        // 创建商家券批次｜官方页 merchant/4012534855 + partner/4012692931｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorStockAsync)).Should().Be("/v3/marketing/busifavor/stocks/{stockId}");
        PathKeysOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorStockAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        // 查询商家券批次详情｜官方页 merchant/4012534928 + partner/4012693047｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.UpdateBusifavorStockAsync)).Should().Be("/v3/marketing/busifavor/stocks/{stockId}");
        PathKeysOf(nameof(IWechatPayMarketingBusifavorService.UpdateBusifavorStockAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        // 修改商家券批次｜官方页 merchant/4012535020 + partner/4012696764｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.UpdateBusifavorStockBudgetAsync)).Should().Be("/v3/marketing/busifavor/stocks/{stockId}/budget");
        PathKeysOf(nameof(IWechatPayMarketingBusifavorService.UpdateBusifavorStockBudgetAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        // 修改商家券批次预算｜官方页 merchant/4012465739 + partner/4012465809｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.UploadBusifavorCouponCodesAsync)).Should().Be("/v3/marketing/busifavor/stocks/{stockId}/couponcodes");
        PathKeysOf(nameof(IWechatPayMarketingBusifavorService.UploadBusifavorCouponCodesAsync)).Should().BeEquivalentTo(new[] { "stockId" });
        // 预上传商家券自定义券码｜官方页 merchant/4012487038 + partner/4012492853｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorUserCouponsAsync)).Should().Be("/v3/marketing/busifavor/users/{openid}/coupons");
        PathKeysOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorUserCouponsAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorUserCouponsAsync)).Should().BeEquivalentTo(new[] { "appid", "stock_id", "creator_merchant", "belong_merchant", "sender_merchant", "offset", "limit", "coupon_state" });
        RequiredQueryOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorUserCouponsAsync)).Should().BeEquivalentTo(new[] { "appid" }, "用户商家券列表的 appid 官方必填：给默认值即把 400 变成静默错参");
        // 用户商家券列表｜官方页 merchant/4012534963 + partner/4012693087｜身份 商户/服务商

        RouteOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorUserCouponAsync)).Should().Be("/v3/marketing/busifavor/users/{openid}/coupons/{couponCode}/appids/{appid}");
        PathKeysOf(nameof(IWechatPayMarketingBusifavorService.QueryBusifavorUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid", "couponCode", "appid" });
        // 用户商家券详情｜官方页 merchant/4012535011 + partner/4012693103｜身份 商户/服务商
    }

    /// <summary>MBF-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void BusifavorDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BusifavorCallbackQueryResponse>().Should().BeEquivalentTo(new[] { "mchid", "notify_url" },
            "商家券回调地址应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCallbackSetRequest>().Should().BeEquivalentTo(new[] { "mchid", "notify_url" },
            "设置商家券回调地址请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCallbackSetResponse>().Should().BeEquivalentTo(new[] { "update_time", "notify_url", "mchid" },
            "设置商家券回调地址应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCouponAssociateRequest>().Should().BeEquivalentTo(new[] { "stock_id", "coupon_code", "out_trade_no", "out_request_no" },
            "商家券关联订单补贴请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCouponAssociateResponse>().Should().BeEquivalentTo(new[] { "wechatpay_associate_time" },
            "商家券关联订单补贴应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCouponDisassociateRequest>().Should().BeEquivalentTo(new[] { "stock_id", "coupon_code", "out_trade_no", "out_request_no" },
            "商家券取消关联订单补贴请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCouponDisassociateResponse>().Should().BeEquivalentTo(new[] { "wechatpay_disassociate_time" },
            "商家券取消关联订单补贴应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCouponDeactivateRequest>().Should().BeEquivalentTo(new[] { "coupon_code", "stock_id", "deactivate_request_no", "deactivate_reason" },
            "作废商家券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCouponDeactivateResponse>().Should().BeEquivalentTo(new[] { "wechatpay_deactivate_time" },
            "作废商家券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCouponReturnRequest>().Should().BeEquivalentTo(new[] { "coupon_code", "stock_id", "return_request_no" },
            "商家券退款退券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCouponReturnResponse>().Should().BeEquivalentTo(new[] { "wechatpay_return_time" },
            "商家券退款退券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCouponUseRequest>().Should().BeEquivalentTo(new[] { "coupon_code", "stock_id", "appid", "use_time", "use_request_no", "openid" },
            "核销商家券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCouponUseResponse>().Should().BeEquivalentTo(new[] { "stock_id", "openid", "wechatpay_use_time" },
            "核销商家券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorProductCouponSendRequest>().Should().BeEquivalentTo(new[] { "openid", "appid", "stock_id", "out_request_no", "coupon_code", "coupon_type", "member_card_id" },
            "向用户发放商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorProductCouponSendResponse>().Should().BeEquivalentTo(new[] { "send_coupon_result" },
            "向用户发放商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorStockCreateRequest>().Should().BeEquivalentTo(new[] { "stock_name", "belong_merchant", "comment", "goods_name", "stock_type", "coupon_use_rule", "stock_send_rule", "out_request_no", "custom_entrance", "display_pattern_info", "coupon_code_mode", "notify_config", "subsidy" },
            "创建商家券批次请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorStockCreateResponse>().Should().BeEquivalentTo(new[] { "stock_id", "create_time" },
            "创建商家券批次应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorStockQueryResponse>().Should().BeEquivalentTo(new[] { "stock_name", "belong_merchant", "comment", "goods_name", "stock_type", "coupon_use_rule", "stock_send_rule", "custom_entrance", "display_pattern_info", "stock_state", "coupon_code_mode", "stock_id", "coupon_code_count", "notify_config", "send_count_information", "subsidy" },
            "查询商家券批次详情应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorStockUpdateRequest>().Should().BeEquivalentTo(new[] { "custom_entrance", "stock_name", "comment", "goods_name", "out_request_no", "display_pattern_info", "coupon_use_rule", "stock_send_rule", "notify_config", "subsidy" },
            "修改商家券批次请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        ReturnTypeOf(nameof(IWechatPayMarketingBusifavorService.UpdateBusifavorStockAsync)).Should().Be(typeof(Task),
            "修改商家券批次官方应答表无业务字段 ⇒ 裸 Task（补空 DTO 即造不存在的契约）");

        DeclaredJsonNamesOf<BusifavorStockBudgetRequest>().Should().BeEquivalentTo(new[] { "target_max_coupons", "current_max_coupons", "target_max_coupons_by_day", "current_max_coupons_by_day", "modify_budget_request_no" },
            "修改商家券批次预算请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorStockBudgetResponse>().Should().BeEquivalentTo(new[] { "max_coupons", "max_coupons_by_day" },
            "修改商家券批次预算应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorCouponCodesUploadRequest>().Should().BeEquivalentTo(new[] { "coupon_code_list", "upload_request_no" },
            "预上传商家券自定义券码请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BusifavorCouponCodesUploadResponse>().Should().BeEquivalentTo(new[] { "stock_id", "total_count", "success_count", "success_codes", "success_time", "fail_count", "fail_codes", "exist_codes", "duplicate_codes" },
            "预上传商家券自定义券码应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorUserCouponListResponse>().Should().BeEquivalentTo(new[] { "data", "total_count", "limit", "offset" },
            "用户商家券列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BusifavorUserCouponQueryResponse>().Should().BeEquivalentTo(new[] { "belong_merchant", "stock_name", "comment", "goods_name", "stock_type", "transferable", "shareable", "coupon_state", "display_pattern_info", "coupon_use_rule", "custom_entrance", "coupon_code", "stock_id", "available_start_time", "expire_time", "receive_time", "send_request_no", "use_request_no", "use_time", "associate_out_trade_no", "return_request_no", "return_time", "deactivate_request_no", "deactivate_time", "deactivate_reason" },
            "用户商家券详情应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>MBF-B4：判错面 + AOT 上下文登记 + DI 与模块接线（11 请求型 / 14 应答型）。</summary>
    [Fact]
    public void BusifavorDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(89, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            MarketingBusifavorJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingBusifavorJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MarketingBusifavor");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(BusifavorCallbackQueryResponse), typeof(BusifavorCallbackSetResponse), typeof(BusifavorCouponAssociateResponse), typeof(BusifavorCouponDisassociateResponse), typeof(BusifavorCouponDeactivateResponse), typeof(BusifavorCouponReturnResponse), typeof(BusifavorCouponUseResponse), typeof(BusifavorProductCouponSendResponse), typeof(BusifavorStockCreateResponse), typeof(BusifavorStockQueryResponse), typeof(BusifavorStockBudgetResponse), typeof(BusifavorCouponCodesUploadResponse), typeof(BusifavorUserCouponListResponse), typeof(BusifavorUserCouponQueryResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(BusifavorCallbackSetRequest), typeof(BusifavorCouponAssociateRequest), typeof(BusifavorCouponDisassociateRequest), typeof(BusifavorCouponDeactivateRequest), typeof(BusifavorCouponReturnRequest), typeof(BusifavorCouponUseRequest), typeof(BusifavorProductCouponSendRequest), typeof(BusifavorStockCreateRequest), typeof(BusifavorStockUpdateRequest), typeof(BusifavorStockBudgetRequest), typeof(BusifavorCouponCodesUploadRequest) })
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MarketingBusifavorJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MarketingBusifavor 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMarketingBusifavorWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 MarketingBusifavor 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMarketingBusifavorService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
