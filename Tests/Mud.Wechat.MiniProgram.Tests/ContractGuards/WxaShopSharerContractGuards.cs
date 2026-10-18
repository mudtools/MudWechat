// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.MiniProgram.DataModels.ShopSharer;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「自定义交易组件 V2 · 分享员（推客/分销族）」域契约守卫（SHOPSH 系列：8 端点路由与动词 +
/// 令牌绑定与注册组 + 装配面 + 参数形态 + 官方字段名与形态陷阱 + DTO 登记计数 + 跨线不交叉）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 minishopopencomponent2「分享员接口」分区 8 页，2026-10-16 逐页取证）</b>：
/// bind / unbind / get_sharer_list / search_sharer / get_sharer_data_summary /
/// get_sharer_live_order_list / get_sharer_live_summary_list / set_share_mode，
/// 路由一律 <c>/shop/sharer/*</c>、动词一律 POST；8 页均 200 且正文含「请求方式 + 请求地址」小节、
/// 同前缀兄弟页导航在册（目录树在位）⇒ <b>零判非契约条目</b>，本守卫不设零声明清单。
/// </para>
/// <para>
/// <b>官方逐页未标注权限集编号、云调用与第三方代调用支持面</b>（取证正则扫描 8 页均无该字段）⇒
/// 本域只按自建 <c>access_token</c> 面落地，此为「官方空缺留档」而非本仓放宽判定。
/// </para>
/// <para>
/// <b>2026-10-16 逐页取证的四处官方文档自身缺陷</b>（本守卫逐条锁定，防按表/按示例单边回潮）：
/// ① <c>set_share_mode</c> 请求示例键 <c>share_mode</c> vs 参数表键 <c>sharer_mode</c> ⇒ 取示例；
/// ② <c>get_sharer_live_order_list</c> 的 <c>product_infos[]</c> 中 <c>path</c> / <c>head_image</c> /
/// <c>title</c> 表列类型 number、说明与示例均为字符串 ⇒ 建 <c>string?</c>；
/// ③ <c>get_sharer_list</c> / <c>search_sharer</c> 的 <c>invited_time</c> / <c>bind_time</c> /
/// <c>bind_status</c> 表标 number、示例为字符串 ⇒ 建数值类型；
/// ④ <c>pay_info.pay_method</c> 仅示例出现、表未列 ⇒ 随示例建模。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：端点聚合计数（<c>ContractBaseline.MiniProgram</c> +8）、跨线路由零重叠（MP-X1）、
/// 带令牌接口计数（MP-X2）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaShopSharerContractGuards
{
    private const string RegistryGroupName = "ShopSharer";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.ShopSharer";

    /// <summary>本域唯一官方路由前缀（8 支全落 /shop/sharer/ 族内，无第二种前缀）。</summary>
    private const string RoutePrefix = "/shop/sharer/";

    /// <summary>
    /// 分享员域官方路由表（8 端点；路由与动词照官方页「接口调用请求说明」原文，2026-10-16 逐页取证）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.BindSharerAsync), typeof(PostAttribute), "/shop/sharer/bind"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.UnbindSharerAsync), typeof(PostAttribute), "/shop/sharer/unbind"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.GetSharerListAsync), typeof(PostAttribute), "/shop/sharer/get_sharer_list"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.SearchSharerAsync), typeof(PostAttribute), "/shop/sharer/search_sharer"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.GetSharerDataSummaryAsync), typeof(PostAttribute), "/shop/sharer/get_sharer_data_summary"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.GetSharerLiveOrderListAsync), typeof(PostAttribute), "/shop/sharer/get_sharer_live_order_list"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.GetSharerLiveSummaryListAsync), typeof(PostAttribute), "/shop/sharer/get_sharer_live_summary_list"),
        (typeof(IWxaShopSharerService), nameof(IWxaShopSharerService.SetShareModeAsync), typeof(PostAttribute), "/shop/sharer/set_share_mode"),
    };

    private static readonly Type[] DomainInterfaces =
    {
        typeof(IWxaShopSharerService),
    };

    /// <summary>官方返回体只有 errcode / errmsg 的端点（全仓口径：直接返回基类 <see cref="WxaResponse"/>，不建空壳应答）。</summary>
    private static readonly string[] BareResponseMethods =
    {
        nameof(IWxaShopSharerService.SetShareModeAsync),
    };

    /// <summary>契约守卫 SHOPSH1：8 端点路由与动词必须与官方契约一致，且声明面与路由表全集相等。</summary>
    [Fact]
    public void ShopSharerEndpoints_ShouldMatchOfficialRoutes()
    {
        // 8 端点 = 官方「分享员接口」分区 8 页全集（bind/unbind/list/search/data_summary/live_order/live_summary/set_share_mode）。
        Routes.Should().HaveCount(8,
            "分享员域官方开放面为 8 端点（2026-10-16 逐页取证：8 页均 200、正文含请求小节、目录树在册，零判非契约条目）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(8, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文拼写，禁止改写）");

            // 8 支官方一律 POST（含五支「查询/列表」——POST 查询是本域动词口径，勿顺手改 GET）。
            attr.Should().BeOfType<PostAttribute>($"{route} 官方调用方式为 POST");
            httpAttribute.Should().Be(typeof(PostAttribute), $"{route} 路由表的动词登记须与官方一致");
        }

        // 单一路由族锁定：8 支全落 /shop/sharer/，无第二种前缀混入；且全小写下划线连写。
        Routes.Should().OnlyContain(static r => r.Route.StartsWith(RoutePrefix, StringComparison.Ordinal),
            "本域 8 支官方一律 /shop/sharer/ 前缀");
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "本域官方路由一律全小写（不得顺手改成驼峰）");

        // 文档页名与路由名不互抄：set_share_mode 的页面叫 share_attribute.html，
        // bind/unbind 的页面叫 bind_sharer/unbind_sharer.html ⇒ 声明面出现页面名形态即为错挂。
        Routes.Should().NotContain(static r =>
            r.Route.EndsWith("/bind_sharer", StringComparison.Ordinal)
            || r.Route.EndsWith("/unbind_sharer", StringComparison.Ordinal)
            || r.Route.EndsWith("/share_attribute", StringComparison.Ordinal),
            "页面文件名（bind_sharer / unbind_sharer / share_attribute）不是路由名，路由照官方「请求地址」原文");

        // 声明面全集自证：Interfaces/ShopSharer/ 里出现的每一条路由字面量都必须在路由表内，反向也成立。
        var declared = DeclaredRoutesInShopSharerInterfaces();
        declared.Should().BeEquivalentTo(Routes.Select(static r => r.Route),
            "Interfaces/ShopSharer/ 的声明面必须与 SHOPSH1 路由表逐条相等（多一条即漏登记，少一条即路由漂移）");
    }

    /// <summary>契约守卫 SHOPSH2：令牌绑定与注册组（官方 8 页均未标注权限集 ⇒ 自建小程序 access_token 面，空缺留档）。</summary>
    [Fact]
    public void ShopSharerInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        DomainInterfaces.Should().HaveCount(1, "分享员域 8 端点共用单接口（无官方子目录族需要分拆）");

        foreach (var iface in DomainInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方每页请求地址均以 access_token 为 Query 凭据）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（改绑小店或企微令牌会让单槽 Resolve 返回 null，errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约把 access_token 强制放 Query（MP-X3 同口径）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 ShopSharer 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序令牌底座（AddMpApp 硬前置）");
        }

        // 端点归属自证：接口方法数须等于路由表规模。
        DomainInterfaces
            .Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "接口声明方法总数须与 SHOPSH1 路由表一致（8）");
    }

    /// <summary>
    /// 契约守卫 SHOPSH3：装配面——注册组名一致性 + <c>MiniProgramModule.ShopSharer</c> 枚举成员与
    /// <c>AddShopSharerApi()</c> 建造者方法在册（共享文件由主控合入，本断言锁定合入产物、缺合入即红）。
    /// </summary>
    [Fact]
    public void ShopSharerModule_ShouldBeWiredIntoBuilderSurface()
    {
        DomainInterfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域接口共用一个注册组（AddShopSharerWebApiHttpClient）");

        Enum.IsDefined(typeof(MiniProgramModule), RegistryGroupName).Should().BeTrue(
            "MiniProgramModule 必须含 ShopSharer 成员（三段式注册第一段；成员即 RegistryGroupName，缺合入即装配面漂移）");

        typeof(MiniProgramServiceBuilder)
            .GetMethod("AddShopSharerApi", BindingFlags.Public | BindingFlags.Instance)
            .Should().NotBeNull("MiniProgramServiceBuilder 必须提供 AddShopSharerApi() 建造者方法（三段式注册第二段）");
    }

    /// <summary>
    /// 契约守卫 SHOPSH4：参数形态——8 支官方一律「单 JSON 请求体 + 零业务 Query」，
    /// 一支裸应答端点直接返回 <see cref="WxaResponse"/>，七支返回本域应答 DTO。
    /// </summary>
    [Fact]
    public void ShopSharerEndpoints_ShouldUseSingleBodyAndZeroQuery()
    {
        foreach (var (iface, method, _, route) in Routes)
        {
            var target = iface.GetMethod(method)!;
            var parameters = target.GetParameters();

            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{route} 本域除 access_token 外无官方 Query 参数，业务参数须走 [Body]");

            parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
            parameters.Should().NotContain(static p => p.GetCustomAttribute<MultipartFormAttribute>() != null,
                $"{route} 本域官方无 multipart 通路");

            parameters.Should().HaveCount(2, $"{route} 形态为 (request, cancellationToken = default)");
            parameters[1].ParameterType.Should().Be(typeof(CancellationToken), $"{route} 第二参数须为 CancellationToken");
            parameters[1].HasDefaultValue.Should().BeTrue($"{route} 的 CancellationToken 须有 default 缺省值（全仓异步约定）");

            if (BareResponseMethods.Contains(method))
            {
                ResultTypeOf(iface, method).Should().Be(typeof(WxaResponse),
                    $"{route} 官方返回体只有 errcode / errmsg ⇒ 复用基类 WxaResponse（全仓口径，不建空壳应答）");
            }
            else
            {
                ResultTypeOf(iface, method).Namespace.Should().Be(DtoNamespace,
                    $"{route} 有业务应答字段 ⇒ 必须返回本域应答 DTO");
            }
        }

        Routes.Count(static r => BareResponseMethods.Contains(r.Method)).Should().Be(1,
            "本域官方返回体只有 errcode/errmsg 的端点为 1（set_share_mode）");
        Routes.Count(static r => !BareResponseMethods.Contains(r.Method)).Should().Be(7,
            "其余 7 支返回本域应答 DTO（与 SHOPSH6 的应答 DTO 计数 7 互为印证）");

        Routes.Select(static r => BodyTypeOf(r.Interface, r.Method)).Should()
            .OnlyContain(t => t.Namespace == DtoNamespace, "8 支的请求体类型全部落本域 DTO 命名空间");
    }

    /// <summary>
    /// 契约守卫 SHOPSH5：官方字段名原文 + 四处官方文档自身缺陷的裁决锁定 + 形态负锁（三列表并存、同名不同型、键名三分）。
    /// </summary>
    [Fact]
    public void ShopSharerDataModels_ShouldLockOfficialFieldsAndRulings()
    {
        // bind / unbind 请求体同键 openids，但应答不同形（bind 多 result_list）⇒ 两套类型独立。
        AssertJsonProperty<WxaShopSharerBindRequest>(nameof(WxaShopSharerBindRequest.OpenIds), "openids", "官方原文拼写（复数）");
        AssertJsonProperty<WxaShopSharerUnbindRequest>(nameof(WxaShopSharerUnbindRequest.OpenIds), "openids", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerBindResponse>(nameof(WxaShopSharerBindResponse.SuccessList), "success_list", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerBindResponse>(nameof(WxaShopSharerBindResponse.FailList), "fail_list", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerBindResponse>(nameof(WxaShopSharerBindResponse.RefuseList), "refuse_list", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerBindResponse>(nameof(WxaShopSharerBindResponse.ResultList), "result_list",
            "官方原文拼写（三列表与 result_list 新旧两套口径并存，不得只留一种）");
        AssertPropertyType<WxaShopSharerBindResponse>(nameof(WxaShopSharerBindResponse.ResultList), typeof(List<WxaShopSharerBindResult>));
        AssertJsonProperty<WxaShopSharerBindResult>(nameof(WxaShopSharerBindResult.ResultCode), "result_code",
            "官方原文拼写（0 成功 / 1 失败可重试 / 2 拒绝不可重试）");
        AssertJsonProperty<WxaShopSharerBindResult>(nameof(WxaShopSharerBindResult.ReasonCode), "reason_code",
            "官方原文拼写（100/200/201/202/203/204 邀请错误码）");

        typeof(WxaShopSharerUnbindResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.Name).Should().BeEquivalentTo(
                new[] { nameof(WxaShopSharerUnbindResponse.SuccessList),
                        nameof(WxaShopSharerUnbindResponse.FailList),
                        nameof(WxaShopSharerUnbindResponse.RefuseList) },
                because: "unbind 官方应答只有三列表、无 result_list ⇒ 与 bind 应答不共用类型");

        // 列表键名三分：sharers / orders / lives（无 list / data.list 形态）。
        AssertJsonProperty<WxaShopSharerListResponse>(nameof(WxaShopSharerListResponse.Sharers), "sharers",
            "官方原文拼写（不是 list，也不是 data.list 包装）");
        AssertJsonProperty<WxaShopSharerLiveOrderListResponse>(nameof(WxaShopSharerLiveOrderListResponse.Orders), "orders", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerLiveSummaryListResponse>(nameof(WxaShopSharerLiveSummaryListResponse.Lives), "lives", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerListResponse>(nameof(WxaShopSharerListResponse.TotalNum), "total_num", "官方原文拼写");

        // 翻页形态：page 从 1 起 + page_size，无游标；三支请求体均不得出现 cursor / offset。
        AssertJsonProperty<WxaShopSharerListRequest>(nameof(WxaShopSharerListRequest.Page), "page", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerListRequest>(nameof(WxaShopSharerListRequest.PageSize), "page_size",
            "官方原文拼写（本支与 live_order 上限 100、live_summary 上限 20）");
        foreach (var t in new[] { typeof(WxaShopSharerListRequest), typeof(WxaShopSharerLiveOrderListRequest), typeof(WxaShopSharerLiveSummaryListRequest) })
        {
            t.GetProperty("Cursor", BindingFlags.Public | BindingFlags.Instance).Should().BeNull($"{t.Name} 官方无游标分页键");
            t.GetProperty("Offset", BindingFlags.Public | BindingFlags.Instance).Should().BeNull($"{t.Name} 官方无 offset 键");
        }

        // search 应答平铺（无包装对象）且独有 bind_status；列表条目无该字段 ⇒ 两类型不共用。
        AssertJsonProperty<WxaShopSharerSearchResponse>(nameof(WxaShopSharerSearchResponse.BindStatus), "bind_status",
            "官方原文拼写（1 已绑定 / 2 拒绝邀请 / 3 邀请中 / 4 邀请过期；仅单查侧可见邀请中/过期）");
        typeof(WxaShopSharerInfo).GetProperty("BindStatus", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("get_sharer_list 条目官方无 bind_status（只回已绑定者）⇒ 与 search 应答不同形");

        // 表/示例类型冲突裁决③：invited_time / bind_time 表标 number ⇒ 建数值类型、不建 string。
        AssertJsonProperty<WxaShopSharerInfo>(nameof(WxaShopSharerInfo.InvitedTime), "invited_time", "官方原文拼写");
        AssertPropertyType<WxaShopSharerInfo>(nameof(WxaShopSharerInfo.InvitedTime), typeof(long?),
            "官方表标 number（示例为字符串是文档缺陷，取表口径并留档）");
        AssertPropertyType<WxaShopSharerSearchResponse>(nameof(WxaShopSharerSearchResponse.BindStatus), typeof(int?),
            "官方表标 number（示例 \"1\" 为字符串是文档缺陷）");

        // 金额面：gmv 两支、price_info 五支一律单位分；键名照官方。
        AssertJsonProperty<WxaShopSharerDataSummaryResponse>(nameof(WxaShopSharerDataSummaryResponse.Gmv), "gmv",
            "官方原文拼写（有效带货额，单位分）");
        AssertJsonProperty<WxaShopSharerDataSummaryResponse>(nameof(WxaShopSharerDataSummaryResponse.OrderCnt), "order_cnt", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerDataSummaryResponse>(nameof(WxaShopSharerDataSummaryResponse.UserCnt), "user_cnt", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerPriceInfo>(nameof(WxaShopSharerPriceInfo.OrderPrice), "order_price", "官方原文拼写（单位分）");
        AssertJsonProperty<WxaShopSharerPriceInfo>(nameof(WxaShopSharerPriceInfo.Freight), "freight", "官方原文拼写（单位分）");
        AssertJsonProperty<WxaShopSharerPriceInfo>(nameof(WxaShopSharerPriceInfo.AdditionalRemarks), "additional_remarks", "官方原文拼写");

        // live order 树：订单定位键与五段包装名。
        AssertJsonProperty<WxaShopSharerLiveOrder>(nameof(WxaShopSharerLiveOrder.OrderId), "order_id",
            "官方原文拼写（number(uint64)，裸数字示例 ⇒ 建 long?）");
        AssertPropertyType<WxaShopSharerLiveOrder>(nameof(WxaShopSharerLiveOrder.OrderId), typeof(long?));
        AssertJsonProperty<WxaShopSharerLiveOrder>(nameof(WxaShopSharerLiveOrder.OutOrderId), "out_order_id", "官方原文拼写（string，与 order_id 不同型）");
        AssertJsonProperty<WxaShopSharerLiveOrder>(nameof(WxaShopSharerLiveOrder.OrderDetail), "order_detail", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerOrderDetail>(nameof(WxaShopSharerOrderDetail.PromotionInfo), "promotion_info", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerOrderDetail>(nameof(WxaShopSharerOrderDetail.ProductInfos), "product_infos", "官方原文拼写（复数）");
        AssertJsonProperty<WxaShopSharerOrderDetail>(nameof(WxaShopSharerOrderDetail.DeliveryDetail), "delivery_detail", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerPromotionInfo>(nameof(WxaShopSharerPromotionInfo.SharerOpenId), "sharer_openid",
            "官方原文拼写（订单树内带 sharer_ 前缀，与请求侧裸 openid 不同键）");
        AssertJsonProperty<WxaShopSharerPayInfo>(nameof(WxaShopSharerPayInfo.TransactionId), "transaction_id", "官方原文拼写（支付凭据，禁落日志）");
        AssertJsonProperty<WxaShopSharerPayInfo>(nameof(WxaShopSharerPayInfo.PayMethodType), "pay_method_type", "官方原文拼写（0 微信 / 1 货到 / 2 储蓄卡）");
        // 缺陷裁决④：pay_method 仅示例出现、表未列 ⇒ 随示例建模。
        AssertJsonProperty<WxaShopSharerPayInfo>(nameof(WxaShopSharerPayInfo.PayMethod), "pay_method",
            "官方回包示例在册键（参数表漏列，缺陷裁决④随示例建模）");
        AssertJsonProperty<WxaShopSharerDeliveryItem>(nameof(WxaShopSharerDeliveryItem.WaybillId), "waybill_id", "官方原文拼写（快递单号，禁落日志）");
        AssertJsonProperty<WxaShopSharerDeliveryItem>(nameof(WxaShopSharerDeliveryItem.DeliveryId), "delivery_id", "官方原文拼写（快递公司 ID）");
        AssertPropertyType<WxaShopSharerDeliveryDetail>(nameof(WxaShopSharerDeliveryDetail.FinishAllDelivery), typeof(int?),
            "官方类型即 number 非 boolean ⇒ 不得改成 bool? 破坏线格式");

        // 缺陷裁决②：product_infos[] 的 path / head_image / title 表列 number、示例为字符串 ⇒ 按语义建 string。
        AssertJsonProperty<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.OutProductId), "out_product_id", "官方原文拼写");
        AssertJsonProperty<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.SalePrice), "sale_price", "官方原文拼写（单位分）");
        AssertJsonProperty<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.RealPrice), "real_price", "官方原文拼写（单位分）");
        AssertPropertyType<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.Path), typeof(string),
            "官方表类型列笔误写 number、说明与示例均为字符串 ⇒ 按语义建 string（缺陷裁决②）");
        AssertPropertyType<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.HeadImage), typeof(string));
        AssertPropertyType<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.Title), typeof(string));

        // 同名不同型锁：live_start_time 在订单侧 string、直播汇总侧 long ⇒ 两类型不得共用。
        AssertPropertyType<WxaShopSharerPromotionInfo>(nameof(WxaShopSharerPromotionInfo.LiveStartTime), typeof(string),
            "官方示例「2020-04-25 12:05:25」为格式化文本");
        AssertPropertyType<WxaShopSharerLiveInfo>(nameof(WxaShopSharerLiveInfo.LiveStartTime), typeof(long?),
            "官方表标 number（unix 时间戳）⇒ 与订单侧同名不同型，不共用类型");
        AssertJsonProperty<WxaShopSharerLiveInfo>(nameof(WxaShopSharerLiveInfo.LiveEndTime), "live_end_time",
            "官方原文拼写（直播中恒为 0，判直播中用 live_status==1）");
        AssertJsonProperty<WxaShopSharerLiveInfo>(nameof(WxaShopSharerLiveInfo.LiveExportId), "live_export_id",
            "官方原文拼写（get_sharer_live_order_list 必填键的取值来源）");

        // 缺陷裁决①：set_share_mode 键名官方自相矛盾（示例 share_mode vs 表 sharer_mode）⇒ 取示例。
        AssertJsonProperty<WxaShopSharerSetShareModeRequest>(nameof(WxaShopSharerSetShareModeRequest.ShareMode), "share_mode",
            "官方请求示例键名（参数表误写 sharer_mode，缺陷裁决①取示例，宿主遇拒收以实测为准回改并同批改守卫）");
        typeof(WxaShopSharerSetShareModeRequest).GetProperty("SharerMode", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("不得按表侧笔误键名另建 sharer_mode 属性（单键请求体出现两键即形态漂移）");

        // 数值化 id 负锁：商品/订单/直播间的 id 类字段官方均为 string ⇒ 不得数值化。
        AssertPropertyType<WxaShopSharerProductInfo>(nameof(WxaShopSharerProductInfo.OutProductId), typeof(string));
        AssertPropertyType<WxaShopSharerLiveOrder>(nameof(WxaShopSharerLiveOrder.OutOrderId), typeof(string));
        AssertPropertyType<WxaShopSharerLiveInfo>(nameof(WxaShopSharerLiveInfo.LiveExportId), typeof(string));
    }

    /// <summary>契约守卫 SHOPSH6：26 支 DTO 全量登记进 ShopSharerJsonContext；命名空间清单锁死；应答 DTO 与端点返回类型一一对应。</summary>
    [Fact]
    public void ShopSharerDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaShopSharerInfo).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("分享员域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 26 DTO = bind 3 + unbind 2 + list 3 + search 2 + data_summary 2 + live_order 10 + live_summary 3 + set_share_mode 1
        //（按官方逐页请求体 / 应答 / 嵌套子对象建模，2026-10-16 取证同批）。
        dtoTypes.Should().HaveCount(26,
            "分享员域 DTO 总数漂移须先核对官方文档再同批调整本守卫（3+2+3+2+2+10+3+1 = 26）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 ShopSharer");
        }

        var contextType = assembly.GetType(DtoNamespace + ".ShopSharerJsonContext");
        contextType.Should().NotBeNull(
            "ShopSharerJsonContext 尚未生成：主控收口时统一跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（后者必须显式带 -TargetProject / -OutputDir 指向本线）——待生成上下文，本断言在生成前预期为红");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (System.Text.Json.Serialization.JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"ShopSharer\")]，"
                + "必须登记进 ShopSharerJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：本域无常量类/枚举类（官方取值全部写在 DTO 属性注释里），除 26 DTO 与生成上下文外不得出现新类型。
        var unregistered = domainTypes.Where(t => !dtoTypes.Contains(t) && t != contextType).ToList();
        unregistered.Should().BeEmpty(
            "分享员域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        // 空壳应答不得回潮：裸应答端点（set_share_mode）复用基类。
        dtoTypes.Should().NotContain(t => t.Name == "WxaShopSharerSetShareModeResponse",
            "裸应答端点不得建空壳 WxaResponse 子类（set_share_mode 返回基类 WxaResponse，全仓同口径）");

        var responseDtos = domainTypes
            .Where(static t => t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();
        responseDtos.Should().HaveCount(7, "本域官方带业务字段的应答对象为 7（与 SHOPSH4 的七支非裸应答互为印证）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "分享员域响应 DTO 必须继承 WxaResponse（errcode 判错与全线同源）");

        var endpointReturnTypes = Routes
            .Select(static r => ResultTypeOf(r.Interface, r.Method))
            .Where(static t => t.Namespace == DtoNamespace)
            .Distinct()
            .ToList();

        endpointReturnTypes.Should().BeEquivalentTo(responseDtos,
            "本域应答类型必须与端点返回类型一一对应（既无孤儿应答，也不返回命名空间外的自建应答）");

        Routes.Count(static r => ResultTypeOf(r.Interface, r.Method) == typeof(WxaResponse))
            .Should().Be(1, "一支裸应答端点直接返回 WxaResponse（与 SHOPSH4 的 BareResponseMethods 同一清单）");
    }

    /// <summary>
    /// 契约守卫 SHOPSH7：跨线不交叉——本域 8 条全部小程序令牌面 <c>/shop/sharer/*</c>，
    /// 与小店线 <c>/channels/ec/*</c> 零交叠；本域无判非契约条目（8 页全部在册），
    /// 但官方页面文件名形态（bind_sharer 等）与前端面（getChannelsShareKey）不得被声明为路由。
    /// </summary>
    [Fact]
    public void ShopSharerRoutes_ShouldNotCrossOtherLines()
    {
        Routes.Should().NotContain(static r => r.Route.Contains("/channels/", StringComparison.Ordinal),
            "/shop/* 属小程序令牌面，小店线 /channels/ec/* 令牌链不同（CH 系列裁决），两线不得混用");

        // 本域声明面之外，全仓任何 /shop/sharer/ 路由字面量都只应出现在本域接口文件（防别域错挂同前缀路由）。
        var shopRoot = Path.Combine(GetSolutionRoot(), "Src", "MiniProgram", "Mud.Wechat.MiniProgram", "Interfaces");
        Directory.Exists(shopRoot).Should().BeTrue($"扫描根必须存在（{shopRoot}）");
        var sharerRouteOwners = Directory
            .EnumerateFiles(shopRoot, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.EndsWith("IWxaShopSharerService.cs", StringComparison.Ordinal))
            .Where(static f => Regex.IsMatch(File.ReadAllText(f), @"""\/shop\/sharer\/"))
            .Select(Path.GetFileName)
            .ToList();
        sharerRouteOwners.Should().BeEmpty(
            "/shop/sharer/* 前缀只许 ShopSharer 域声明，以下文件出现即跨域错挂：" + string.Join(", ", sharerRouteOwners));
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>），守卫比对的是应答对象本身而非 Task 壳。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    /// <summary>取端点方法 <c>[Body]</c> 参数的声明类型。</summary>
    private static Type BodyTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var body = method!.GetParameters().Single(static p => p.GetCustomAttribute<BodyAttribute>() != null);
        return body.ParameterType;
    }

    /// <summary>
    /// 扫描 <c>Src/MiniProgram/Mud.Wechat.MiniProgram/Interfaces/ShopSharer/</c> 剥离注释后的路由字面量集合
    /// （形态同 NV1 / RD1 的声明面自证：必须先证扫得出在册路由，零声明与全集断言才不是空转）。
    /// </summary>
    private static HashSet<string> DeclaredRoutesInShopSharerInterfaces()
    {
        var root = Path.Combine(GetSolutionRoot(), "Src", "MiniProgram", "Mud.Wechat.MiniProgram", "Interfaces", "ShopSharer");
        Directory.Exists(root).Should().BeTrue($"扫描根必须存在（{root}）——目录漂移会让 SHOPSH1 的声明面自证静默恒真");

        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (System.Text.RegularExpressions.Match m in literalRegex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        // 正对照：扫出的集合必须覆盖路由表全集，否则是扫描口径失效（而非「本域真的没有多余路由」）。
        foreach (var control in new[] { "/shop/sharer/bind", "/shop/sharer/get_sharer_live_order_list" })
        {
            set.Should().Contain(control, $"SHOPSH1 的声明面扫描必须能捕获本域在册路由（{control}）——否则全集断言静默恒真");
        }

        return set;
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void AssertPropertyType<T>(string propertyName, Type expectedType, string because = "")
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType,
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}（形态裁决{(because.Length > 0 ? "：" + because : string.Empty)}）");
    }
}
