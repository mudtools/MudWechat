// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.MiniProgram.DataModels.OrderShipping;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「发货信息管理与品牌申请（OrderShipping）」域契约守卫（OS 系列：12 端点路由与动词 +
/// 令牌与注册组 + 请求体面 + 上传/查询两侧字段面不对称 + DTO 与 JSON 上下文登记 + 官方开放面矩阵）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页取证，<c>order_shipping</c> 与 <c>famousbrand</c> 两族共 12 端点）</b>：
/// 发货录入与报备 4（<see cref="IWxaOrderShippingService"/>）+ 订单与管理状态查询 4
/// （<see cref="IWxaOrderQueryService"/>）+ 设置与交易类型变更 2（<see cref="IWxaOrderSettingService"/>）+
/// 品牌申请 2（<see cref="IWxaFamousBrandService"/>）。四接口共用一个注册组，故本域是「一次注册装载四接口」形态。
/// </para>
/// <para>
/// <b>本域最易出错的一处是「上传侧与查询侧字段名不对称」</b>：同一笔支付单在两支接口里的定位键写法不同
/// （<c>mchid</c> / <c>out_trade_no</c> 对 <c>merchant_id</c> / <c>merchant_trade_no</c>），
/// 同一份物流信息在上传时写作 <c>item_desc</c>、查询时写作 <c>goods_desc</c>，
/// 同名键 <c>upload_time</c> 在顶层是 RFC 3339 字符串、在查询侧物流单元素里是 number 时间戳。
/// 这些差异<b>不会</b>被编译器发现，一旦为「统一命名」而合并 DTO 或改写 JSON 名，
/// 现象是官方网关报「参数错误」或字段被静默忽略 ⇒ OS4 逐条锁死。
/// </para>
/// <para>
/// <b>敏感值口径（与 MP-B4 同一处置）</b>：用户 <c>openid</c>、实付金额 <c>paid_amount</c>（单位分）、
/// 收发货人联系方式 <c>contact.consignor_contact</c> / <c>contact.receiver_contact</c>
/// （官方要求掩码传输，且最后 4 位数字不得参与掩码）⇒ <b>禁止</b>写入日志、遥测或异常消息。
/// 本域凭据面只有 <c>access_token</c> 一支（已由组件静态词表覆盖），故 OS3 断言「全域零 <c>[Query]</c> 业务参数」。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数（MP-X5 /
/// <c>ContractBaseline</c>）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；Query 令牌注入白名单（QT1）
/// 由 <c>MpQueryTokenWhitelistGuard</c> 持有；/wxa/sec/vod 与 /wxa/sec/order 两族的路由交叉由本文件 OS6 与
/// 守卫 XP4 分域把守。本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaOrderShippingContractGuards
{
    private const string RegistryGroupName = "OrderShipping";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.OrderShipping";

    private const string OrderRoutePrefix = "/wxa/sec/order/";

    private const string BrandRoutePrefix = "/wxa/sec/famousbrand/";

    /// <summary>
    /// 本域官方路由表（12 端点：发货录入与报备 4 + 订单与管理状态查询 4 + 设置 2 + 品牌申请 2）。
    /// 路由照官方原文，含 <c>setwxatradetypecgi</c> 这类全小写连写且带历史后缀 <c>cgi</c> 的形态。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaOrderShippingService), nameof(IWxaOrderShippingService.UploadShippingInfoAsync), typeof(PostAttribute), "/wxa/sec/order/upload_shipping_info"),
        (typeof(IWxaOrderShippingService), nameof(IWxaOrderShippingService.UploadCombinedShippingInfoAsync), typeof(PostAttribute), "/wxa/sec/order/upload_combined_shipping_info"),
        (typeof(IWxaOrderShippingService), nameof(IWxaOrderShippingService.NotifyConfirmReceiveAsync), typeof(PostAttribute), "/wxa/sec/order/notify_confirm_receive"),
        (typeof(IWxaOrderShippingService), nameof(IWxaOrderShippingService.ReportSpecialOrderAsync), typeof(PostAttribute), "/wxa/sec/order/opspecialorder"),

        (typeof(IWxaOrderQueryService), nameof(IWxaOrderQueryService.GetOrderAsync), typeof(PostAttribute), "/wxa/sec/order/get_order"),
        (typeof(IWxaOrderQueryService), nameof(IWxaOrderQueryService.GetOrderListAsync), typeof(PostAttribute), "/wxa/sec/order/get_order_list"),
        (typeof(IWxaOrderQueryService), nameof(IWxaOrderQueryService.IsTradeManagedAsync), typeof(PostAttribute), "/wxa/sec/order/is_trade_managed"),
        (typeof(IWxaOrderQueryService), nameof(IWxaOrderQueryService.IsTradeManagementConfirmationCompletedAsync), typeof(PostAttribute), "/wxa/sec/order/is_trade_management_confirmation_completed"),

        (typeof(IWxaOrderSettingService), nameof(IWxaOrderSettingService.SetMsgJumpPathAsync), typeof(PostAttribute), "/wxa/sec/order/set_msg_jump_path"),
        (typeof(IWxaOrderSettingService), nameof(IWxaOrderSettingService.ApplyTradeTypeChangeAsync), typeof(PostAttribute), "/wxa/sec/order/setwxatradetypecgi"),

        (typeof(IWxaFamousBrandService), nameof(IWxaFamousBrandService.ApplyAsync), typeof(PostAttribute), "/wxa/sec/famousbrand/apply"),
        (typeof(IWxaFamousBrandService), nameof(IWxaFamousBrandService.GetApplyStatusAsync), typeof(PostAttribute), "/wxa/sec/famousbrand/get_status"),
    };

    private static readonly Type[] DomainInterfaces =
    {
        typeof(IWxaOrderShippingService),
        typeof(IWxaOrderQueryService),
        typeof(IWxaOrderSettingService),
        typeof(IWxaFamousBrandService),
    };

    /// <summary>官方给出 <c>wxa.sec.order.*</c> 云调用方法名的 7 支端点。</summary>
    private static readonly string[] CloudCallRoutes =
    {
        "/wxa/sec/order/upload_shipping_info", "/wxa/sec/order/upload_combined_shipping_info",
        "/wxa/sec/order/notify_confirm_receive", "/wxa/sec/order/get_order", "/wxa/sec/order/get_order_list",
        "/wxa/sec/order/is_trade_managed", "/wxa/sec/order/set_msg_jump_path",
    };

    /// <summary>官方明写「本接口不支持云调用」的 5 支端点。</summary>
    private static readonly string[] NoCloudCallRoutes =
    {
        "/wxa/sec/order/opspecialorder",
        "/wxa/sec/order/is_trade_management_confirmation_completed",
        "/wxa/sec/order/setwxatradetypecgi",
        "/wxa/sec/famousbrand/apply", "/wxa/sec/famousbrand/get_status",
    };

    /// <summary>官方明写「本接口不支持第三方平台调用」的 2 支（同时是<b>唯一无权限集行</b>的两支）。</summary>
    private static readonly string[] NoThirdPartyRoutes =
    {
        "/wxa/sec/order/get_order_list", "/wxa/sec/order/set_msg_jump_path",
    };

    /// <summary>权限集 <c>18、142</c> 两支（服务商须被授权 18 或 142 才能查询管理状态）。</summary>
    private static readonly string[] PermissionSet18And142Routes =
    {
        "/wxa/sec/order/is_trade_managed", "/wxa/sec/order/is_trade_management_confirmation_completed",
    };

    /// <summary>适用范围段明写「支持「小程序」账号类型调用」的 2 支（其余 10 支为「暂未明确」留档）。</summary>
    private static readonly string[] MiniProgramScopeRoutes =
    {
        "/wxa/sec/famousbrand/apply", "/wxa/sec/famousbrand/get_status",
    };

    /// <summary>
    /// 错误码档一：官方给出 <c>1006xxxx</c> + <c>2684851xx</c> 交易面码表的 5 支
    /// （规模 28 / 29 / 32 / 28 / 28，五表同源但有增减行，不得互为基线）。
    /// </summary>
    private static readonly string[] TradeErrorCodeRoutes =
    {
        "/wxa/sec/order/get_order", "/wxa/sec/order/get_order_list", "/wxa/sec/order/notify_confirm_receive",
        "/wxa/sec/order/upload_shipping_info", "/wxa/sec/order/upload_combined_shipping_info",
    };

    /// <summary>错误码档二：品牌申请两支的 <c>10210001/002/003</c> 码表（7 支 / 10 支，后者多令牌三支）。</summary>
    private static readonly string[] BrandErrorCodeRoutes =
    {
        "/wxa/sec/famousbrand/apply", "/wxa/sec/famousbrand/get_status",
    };

    /// <summary>错误码档三：管理状态两支，官方两张表逐行相同（-1 40013 40097 44990 61003 61004 61011）。</summary>
    private static readonly string[] ManagedErrorCodeRoutes =
    {
        "/wxa/sec/order/is_trade_managed", "/wxa/sec/order/is_trade_management_confirmation_completed",
    };

    /// <summary>错误码档四：各有独立码表的 3 支（特殊报备无 <c>-1</c>；路径设置仅 <c>-1</c>；交易类型变更含 <c>0</c> 与 268486048）。</summary>
    private static readonly string[] SpecialErrorCodeRoutes =
    {
        "/wxa/sec/order/opspecialorder", "/wxa/sec/order/set_msg_jump_path", "/wxa/sec/order/setwxatradetypecgi",
    };

    /// <summary>官方请求参数表<b>零字段</b>的端点（不设请求 DTO，无 <c>[Body]</c> 参数）。</summary>
    private static readonly string[] ZeroBodyRoutes =
    {
        "/wxa/sec/famousbrand/get_status",
    };

    /// <summary>
    /// 契约守卫 OS1：12 端点路由与动词必须与官方契约逐字一致（两族前缀、一律 POST、照原文拼写）。
    /// </summary>
    [Fact]
    public void OrderShippingEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(12,
            "本域官方开放面为 12 端点（发货录入与报备 4 + 订单与管理状态查询 4 + 设置与交易类型变更 2 + 品牌申请 2）");
        Routes.Select(static r => r.Route).Distinct(StringComparer.Ordinal).Should().HaveCount(12, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致（照官方原文拼写，禁止改写）");
            (route.StartsWith(OrderRoutePrefix, StringComparison.Ordinal) || route.StartsWith(BrandRoutePrefix, StringComparison.Ordinal))
                .Should().BeTrue($"{iface.Name}.{method} 路由必须落在本域两个官方族之一");
        }

        // 动词分布：12 支一律 POST（含两支「查询管理状态」这类语义上像 GET 的端点，官方调用方式行即 POST）。
        Routes.Should().OnlyContain(static r => r.HttpAttribute == typeof(PostAttribute),
            "本域官方动词全为 POST（不得为「语义像查询」的端点擅自改 GET）");

        // 路由形状：全小写、无驼峰、无尾斜杠。
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "本域官方路由一律小写（不得顺手改成驼峰）");

        // 族规模：/wxa/sec/order/ 10 条 + /wxa/sec/famousbrand/ 2 条。
        Routes.Count(static r => r.Route.StartsWith(OrderRoutePrefix, StringComparison.Ordinal))
            .Should().Be(10, "订单发货族 10 条（官方 order_shipping 分区全集）");
        Routes.Count(static r => r.Route.StartsWith(BrandRoutePrefix, StringComparison.Ordinal))
            .Should().Be(2, "品牌申请族 2 条（官方 famousbrand 分区全集，与本域同批落位、共用注册组）");

        // 反直觉点锁定 1：交易类型变更申请的路由是全小写连写且带历史后缀 cgi（setwxatradetypecgi），
        // 而同一页的接口英文名亦为 setwxatradetypecgi ⇒ 不得「顺手」改成 set_wxa_trade_type 形态。
        Routes.Select(static r => r.Route).Should().Contain("/wxa/sec/order/setwxatradetypecgi",
            "官方路由原文即连写含 cgi 后缀 ⇒ 照原文落地，不得改成下划线形态");
        Routes.Should().NotContain(static r => r.Route == "/wxa/sec/order/set_wxa_trade_type",
            "下划线形态的那支官方不存在");

        // 反直觉点锁定 2：特殊发货报备是 opspecialorder（连写，且官方接口英文名同名），
        // 与本域其余动名下划线形态不一致 ⇒ 两支并存是官方事实，不得归一。
        Routes.Select(static r => r.Route).Should().Contain("/wxa/sec/order/opspecialorder");

        // 反直觉点锁定 3：单条录入与合单录入是两支独立端点（upload_shipping_info / upload_combined_shipping_info），
        // 请求体形状不同（见 OS4），不得合并为一个方法 + 布尔开关。
        Routes.Select(static r => r.Route).Should()
            .Contain("/wxa/sec/order/upload_shipping_info")
            .And.Contain("/wxa/sec/order/upload_combined_shipping_info",
                "统一录入与合单录入官方分两支，请求体顶层字段集不同 ⇒ 两个方法、两套 DTO");

        // 反直觉点锁定 4：品牌申请族内两支命名不同源（apply 裸动词 / get_status 下划线），照原文各落各的。
        Routes.Select(static r => r.Route).Should().Contain("/wxa/sec/famousbrand/apply")
            .And.Contain("/wxa/sec/famousbrand/get_status");

        // 与短剧媒资族的零交叉：/wxa/sec/ 之下本域只声明 order 与 famousbrand 两支前缀，vod 族归 Vod 域。
        Routes.Should().NotContain(static r => r.Route.StartsWith("/wxa/sec/vod", StringComparison.Ordinal),
            "/wxa/sec/vod/* 归短剧媒资域（权限集 153），本域不得重复声明");
    }

    /// <summary>
    /// 契约守卫 OS2：四接口令牌绑定（复用小程序 access_token + Query 注入）与共用注册组 + 逐接口规模。
    /// </summary>
    [Fact]
    public void OrderShippingInterfaces_ShouldBindMpAccessToken_ShareRegistryGroup_AndLockMethodCounts()
    {
        DomainInterfaces.Should().HaveCount(4, "本域按官方形态分为四接口（合计 12 端点）");

        foreach (var iface in DomainInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方 12 页令牌面一致，第三方代调用为 authorizer_access_token）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（新增令牌类型会让单槽 Resolve 返回 null ⇒ errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名须照官方原文");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 OrderShipping 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");

            iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeEmpty($"{iface.Name} 不得是空接口");
        }

        DomainInterfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域四接口共用一个注册组（AddOrderShippingWebApiHttpClient）");

        // 端点归属自证：四接口方法数之和 == 路由表规模（防「加了接口忘了进路由表」）。
        DomainInterfaces
            .Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "四接口声明方法总数须与 OS1 路由表一致");

        MethodCount(typeof(IWxaOrderShippingService)).Should().Be(4, "发货录入与报备族 4（单条录入 / 合单录入 / 确认收货提醒 / 特殊发货报备）");
        MethodCount(typeof(IWxaOrderQueryService)).Should().Be(4, "查询族 4（订单发货状态 / 订单列表 / 是否开通发货管理 / 是否完成交易结算管理确认）");
        MethodCount(typeof(IWxaOrderSettingService)).Should().Be(2, "设置族 2（消息跳转路径 / 交易类型变更申请）");
        MethodCount(typeof(IWxaFamousBrandService)).Should().Be(2, "品牌申请族 2（提交申请 / 查询申请状态）");
    }

    /// <summary>
    /// 契约守卫 OS3：<b>参数面</b> —— 11 支恰一个 <c>[Body]</c>、1 支零请求体，且<b>全域零业务 <c>[Query]</c> 参数</b>
    /// ⇒ 本域凭据面只有 <c>access_token</c>，不需要扩充组件脱敏词表，也不需要 <c>SensitiveUrlKeys</c> 登记（对照 XP6）。
    /// </summary>
    [Fact]
    public void OrderShippingMethods_ShouldUseJsonBodyOnly_AndDeclareNoQueryCredential()
    {
        ZeroBodyRoutes.Should().HaveCount(1, "官方 12 页里只有品牌申请状态查询的请求体表原文为「无」");

        foreach (var (iface, method, _, route) in Routes)
        {
            var parameters = iface.GetMethod(method)!.GetParameters();

            var bodyCount = parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null);
            if (ZeroBodyRoutes.Contains(route))
            {
                bodyCount.Should().Be(0, $"{route} 官方请求参数表零字段 ⇒ 不得凭空造 request DTO");
                parameters.Should().HaveCount(1, $"{route} 只允许 CancellationToken 一个参数（令牌由 [Token] 注入，不占方法参数）");
            }
            else
            {
                bodyCount.Should().Be(1, $"{route} 官方请求体为 JSON 对象 ⇒ 恰有一个 [Body] 参数");
                parameters.Should().HaveCount(2, $"{route} 参数面 = [Body] 请求 DTO + CancellationToken");
            }

            // 本域不得出现任何方法级 [Query] 业务参数（区别于虚拟支付域的 pay_sig / signature）。
            parameters.Where(static p => p.GetCustomAttribute<QueryAttribute>() != null)
                .Should().BeEmpty($"{route} 官方 Query 串只有 access_token（由 [Token] 注入）⇒ 出现 [Query] 参数即新增词表面，须走 QT1/XP6 二选一处置");
        }

        // 正向自证：本域 12 支里确有零请求体支，防「ZeroBodyRoutes 与反射面双双为空」的真空断言。
        Routes.Count(static r => ZeroBodyRoutes.Contains(r.Route)).Should().Be(1);
        Routes.Count(static r => RequestTypeOf(r.Interface, r.Method) == null).Should().Be(1,
            "零 [Body] 参数的端点数须与 ZeroBodyRoutes 一致（否则归类表与接口面漂移）");
    }

    /// <summary>
    /// 契约守卫 OS4：<b>上传侧与查询侧的字段面不对称</b> + 顶层与嵌套同名异型 + 品牌申请的大写包装键。
    /// </summary>
    /// <remarks>
    /// 这些差异全部来自官方两张字段表逐行对照，编译器与序列化器都不会报错，
    /// 只有守卫能拦住「为统一命名而合并 DTO」这一类改动。
    /// </remarks>
    [Fact]
    public void OrderShippingFields_ShouldKeepOfficialAsymmetries()
    {
        // —— 品牌申请：请求侧包装键是首字母大写的 `Application`，响应侧是小写 `application` ——
        AssertJsonProperty<WxaFamousBrandApplyRequest>(nameof(WxaFamousBrandApplyRequest.Application), "Application",
            "官方请求示例原文即首字母大写（网关按该键取参数，改成小写即 10210001 参数错误）");
        AssertJsonProperty<WxaFamousBrandApplyStatusResponse>(nameof(WxaFamousBrandApplyStatusResponse.Application), "application",
            "官方返回体原文为小写 ⇒ 与请求侧大小写不同源，不得互相「对齐」");

        // —— 订单定位键：上传侧 order_key 用 mchid / out_trade_no，查询侧用 merchant_id / merchant_trade_no ——
        AssertJsonProperty<WxaOrderKey>(nameof(WxaOrderKey.MchId), "mchid", "官方原文连写（微信支付商户号，无下划线分词）");
        AssertJsonProperty<WxaOrderKey>(nameof(WxaOrderKey.OutTradeNo), "out_trade_no", "官方原文（商户系统内部订单号）");
        AssertJsonProperty<WxaOrderKey>(nameof(WxaOrderKey.OrderNumberType), "order_number_type", "官方原文（1=商户侧单号 2=微信支付单号）");
        AssertJsonProperty<WxaOrderQueryRequest>(nameof(WxaOrderQueryRequest.MerchantId), "merchant_id", "官方原文（查询侧商户号写法与上传侧 mchid 不同源）");
        AssertJsonProperty<WxaOrderQueryRequest>(nameof(WxaOrderQueryRequest.MerchantTradeNo), "merchant_trade_no", "官方原文（查询侧商户单号）");
        AssertJsonProperty<WxaOrderQueryRequest>(nameof(WxaOrderQueryRequest.SubMerchantId), "sub_merchant_id", "官方原文（二级商户号，查询侧独有）");
        AssertJsonProperty<WxaOrderConfirmReceiveRequest>(nameof(WxaOrderConfirmReceiveRequest.MerchantId), "merchant_id",
            "确认收货提醒取查询侧写法（官方参数表即 merchant_id，不是 mchid）");

        AssertPropertyAbsent<WxaOrderKey>("MerchantId", "上传侧官方无 merchant_id ⇒ 补了即向官方多传字段");
        AssertPropertyAbsent<WxaOrderQueryRequest>("MchId", "查询侧官方无 mchid ⇒ 两支定位键 DTO 不同源，不得合并");
        AssertPropertyAbsent<WxaOrderQueryRequest>("OrderNumberType", "查询侧无 order_number_type（官方靠单号本身区分）");
        typeof(WxaOrderKey).Should().NotBe(typeof(WxaOrderQueryRequest),
            "上传侧 order_key 与查询侧定位参数是官方两张表 ⇒ 各自建模（合并即静默丢字段）");

        // —— 物流单元素：上传侧 item_desc（且无 upload_time），查询侧 goods_desc + upload_time(number) ——
        AssertJsonProperty<WxaOrderShippingItem>(nameof(WxaOrderShippingItem.ItemDesc), "item_desc", "官方原文（商品信息，限 120 字）");
        AssertJsonProperty<WxaOrderShippingRecord>(nameof(WxaOrderShippingRecord.GoodsDesc), "goods_desc",
            "官方原文（小程序后台发货信息录入页录入的商品描述）——与上传侧 item_desc 语义相邻但键名不同源");
        AssertPropertyAbsent<WxaOrderShippingItem>("UploadTime", "上传侧物流单元素官方无 upload_time（时间在顶层）");
        AssertPropertyAbsent<WxaOrderShippingRecord>("ItemDesc", "查询侧物流单元素官方无 item_desc");
        typeof(WxaOrderShippingItem).Should().NotBe(typeof(WxaOrderShippingRecord),
            "上传与查询两侧物流单元素字段集不同 ⇒ 两套 DTO（「统一命名」合并即丢 upload_time 或 item_desc）");

        // —— 同名异型：顶层 upload_time 是 RFC 3339 字符串，查询侧 shipping_list 内同名键是 number ——
        AssertJsonProperty<WxaOrderShippingUploadRequest>(nameof(WxaOrderShippingUploadRequest.UploadTime), "upload_time", "官方原文");
        AssertPropertyType<WxaOrderShippingUploadRequest>(nameof(WxaOrderShippingUploadRequest.UploadTime), typeof(string),
            "官方类型列 string（示例 `2022-12-15T13:29:35.120+08:00`）⇒ 不得数值化，也不得由 SDK 生成时钟（官方用以标识请求先后顺序）");
        AssertJsonProperty<WxaOrderShippingRecord>(nameof(WxaOrderShippingRecord.UploadTime), "upload_time", "官方原文（与顶层同名）");
        AssertPropertyType<WxaOrderShippingRecord>(nameof(WxaOrderShippingRecord.UploadTime), typeof(long?),
            "官方类型列 number（时间戳）⇒ 同名键在两处类型不同，不得共享 DTO");

        // —— 两支「是否已…」查询：请求体同形（共用一支 DTO），响应键名各异 ——
        RequestTypeOf(typeof(IWxaOrderQueryService), nameof(IWxaOrderQueryService.IsTradeManagedAsync))
            .Should().Be(RequestTypeOf(typeof(IWxaOrderQueryService), nameof(IWxaOrderQueryService.IsTradeManagementConfirmationCompletedAsync)),
            "官方两页请求参数表逐字段相同（appid，必填）⇒ 共用 WxaOrderTradeManagedQueryRequest");
        AssertJsonProperty<WxaOrderTradeManagedResponse>(nameof(WxaOrderTradeManagedResponse.IsTradeManaged), "is_trade_managed", "官方原文");
        AssertJsonProperty<WxaOrderTradeConfirmationResponse>(nameof(WxaOrderTradeConfirmationResponse.Completed), "completed",
            "官方原文即 completed（不是 is_completed / confirmation_completed）⇒ 两支响应键不同，不得对齐");
        AssertPropertyType<WxaOrderTradeManagedResponse>(nameof(WxaOrderTradeManagedResponse.IsTradeManaged), typeof(bool?), "官方类型列 boolean");
        AssertPropertyType<WxaOrderTradeConfirmationResponse>(nameof(WxaOrderTradeConfirmationResponse.Completed), typeof(bool?), "官方类型列 boolean");

        // —— 合单录入：顶层没有 logistics_type / delivery_mode / shipping_list，全部下放到 sub_orders 元素 ——
        AssertPropertyAbsent<WxaOrderCombinedShippingUploadRequest>("LogisticsType", "合单顶层官方无该字段（只在 sub_orders 元素内）");
        AssertPropertyAbsent<WxaOrderCombinedShippingUploadRequest>("DeliveryMode", "合单顶层官方无该字段");
        AssertPropertyAbsent<WxaOrderCombinedShippingUploadRequest>("ShippingList", "合单顶层官方无该字段（每笔子单各自带物流单列表）");
        AssertPropertyType<WxaOrderCombinedShippingUploadRequest>(nameof(WxaOrderCombinedShippingUploadRequest.SubOrders),
            typeof(List<WxaOrderSubOrder>), "官方 objarray ⇒ List<T>?（子单结构复用上传侧形状）");
        AssertPropertyType<WxaOrderSubOrder>(nameof(WxaOrderSubOrder.ShippingList), typeof(List<WxaOrderShippingItem>),
            "子单物流单与单条录入端点官方同表 ⇒ 共用 WxaOrderShippingItem");
        AssertJsonProperty<WxaOrderCombinedShippingUploadRequest>(nameof(WxaOrderCombinedShippingUploadRequest.UploadTime), "upload_time",
            "合单侧顶层同名键亦为 string（与单条录入同一口径）");
        AssertPropertyType<WxaOrderCombinedShippingUploadRequest>(nameof(WxaOrderCombinedShippingUploadRequest.UploadTime), typeof(string));

        // —— 特殊发货报备：官方用单字符串 order_id，没有 order_key 对象、也没有 order_number_type ——
        AssertJsonProperty<WxaOrderSpecialReportRequest>(nameof(WxaOrderSpecialReportRequest.OrderId), "order_id",
            "官方原文（可传微信支付单号或商户单号，二者混装一支字段 ⇒ 不得拆成 order_key）");
        AssertPropertyAbsent<WxaOrderSpecialReportRequest>("OrderKey", "本端点官方无 order_key 对象");
        AssertPropertyAbsent<WxaOrderSpecialReportRequest>("OrderNumberType", "本端点官方无单号类型开关");
        AssertPropertyType<WxaOrderSpecialReportRequest>(nameof(WxaOrderSpecialReportRequest.DelayTo), typeof(long?),
            "官方类型列 number（预计发货时间 unix 时间戳，type=1 时必填）");

        // —— 顶层字段集逐支锁死（增删字段即红在这里，须先回官方页面）——
        AssertJsonNameSet<WxaOrderShippingUploadRequest>("单条录入官方请求表 7 行",
            "order_key", "logistics_type", "delivery_mode", "is_all_delivered", "shipping_list", "upload_time", "payer");
        AssertJsonNameSet<WxaOrderCombinedShippingUploadRequest>("合单录入官方请求表 4 行",
            "order_key", "sub_orders", "upload_time", "payer");
        AssertJsonNameSet<WxaOrderKey>("订单定位键官方表 4 行",
            "order_number_type", "transaction_id", "mchid", "out_trade_no");
        AssertJsonNameSet<WxaOrderQueryRequest>("查询订单官方表 4 行",
            "transaction_id", "merchant_id", "sub_merchant_id", "merchant_trade_no");
        AssertJsonNameSet<WxaOrderConfirmReceiveRequest>("确认收货提醒官方表 5 行",
            "transaction_id", "merchant_id", "sub_merchant_id", "merchant_trade_no", "received_time");
        AssertJsonNameSet<WxaOrderSpecialReportRequest>("特殊发货报备官方表 3 行",
            "order_id", "type", "delay_to");
        AssertJsonNameSet<WxaOrderListRequest>("查询订单列表官方表 5 行",
            "pay_time_range", "order_state", "openid", "last_index", "page_size");
        AssertJsonNameSet<WxaOrderShippingItem>("上传侧物流单元素官方表 4 行",
            "tracking_no", "express_company", "item_desc", "contact");
        AssertJsonNameSet<WxaOrderShippingRecord>("查询侧物流单元素官方表 5 行",
            "tracking_no", "express_company", "goods_desc", "upload_time", "contact");
        AssertJsonNameSet<WxaOrderInfo>("查询侧支付单官方表 12 行",
            "transaction_id", "merchant_id", "sub_merchant_id", "merchant_trade_no", "description",
            "paid_amount", "openid", "trade_create_time", "pay_time", "order_state", "in_complaint", "shipping");
        AssertJsonNameSet<WxaOrderShippingState>("查询侧发货信息官方表 6 行",
            "delivery_mode", "logistics_type", "finish_shipping", "goods_desc", "finish_shipping_count", "shipping_list");
        AssertJsonNameSet<WxaFamousBrandAuditInfo>("品牌申请材料面官方表 6 行",
            "brand_name", "brand_type", "flagship_in_which_ec_platform",
            "ec_platform_proof_list", "other_material_list", "authority_certified_proof_list");
        AssertJsonNameSet<WxaOrderTradeTypeChangeRequest>("交易类型变更申请官方表 3 行",
            "trade_type", "material_list", "reason");

        // —— 类型口径抽样（官方 number→long?、boolean→bool?、string 原样；敏感键名照原文）——
        AssertJsonProperty<WxaOrderInfo>(nameof(WxaOrderInfo.PaidAmount), "paid_amount", "官方原文（实付金额，单位分 ⇒ 敏感值，禁止进日志与遥测）");
        AssertPropertyType<WxaOrderInfo>(nameof(WxaOrderInfo.PaidAmount), typeof(long?), "官方类型列 number");
        AssertPropertyType<WxaOrderInfo>(nameof(WxaOrderInfo.InComplaint), typeof(bool?), "官方类型列 boolean");
        AssertPropertyType<WxaOrderInfo>(nameof(WxaOrderInfo.OrderState), typeof(long?), "官方类型列 number（订单状态枚举 1~6）");
        AssertJsonProperty<WxaOrderInfo>(nameof(WxaOrderInfo.OpenId), "openid", "官方原文全小写（不得写 open_id ⇒ 敏感值）");
        AssertJsonProperty<WxaOrderPayer>(nameof(WxaOrderPayer.OpenId), "openid", "官方原文（上传侧 payer 子对象只有 openid）");
        AssertJsonProperty<WxaOrderListResponse>(nameof(WxaOrderListResponse.HasMore), "has_more", "官方原文（boolean）");
        AssertPropertyType<WxaOrderListResponse>(nameof(WxaOrderListResponse.HasMore), typeof(bool?));
        AssertPropertyType<WxaOrderListRequest>(nameof(WxaOrderListRequest.PageSize), typeof(long?),
            "官方类型列 number（默认 100，属规模限制而非频率限制）");
        AssertPropertyType<WxaOrderShippingState>(nameof(WxaOrderShippingState.FinishShipping), typeof(bool?), "官方类型列 boolean");
        AssertPropertyType<WxaOrderShippingState>(nameof(WxaOrderShippingState.FinishShippingCount), typeof(long?),
            "官方 number：未完成 0 / 完成 1 / 重新发货并完成后 2（=2 即唯一一次重新发货机会已用尽）");
        AssertJsonProperty<WxaOrderContact>(nameof(WxaOrderContact.ConsignorContact), "consignor_contact",
            "官方原文（发货人联系方式 ⇒ 官方要求掩码传输，且最后 4 位数字不得参与掩码 ⇒ 敏感值）");
        AssertJsonProperty<WxaOrderContact>(nameof(WxaOrderContact.ReceiverContact), "receiver_contact", "官方原文（收货人联系方式，敏感值）");
        AssertJsonProperty<WxaOrderMsgJumpPathRequest>(nameof(WxaOrderMsgJumpPathRequest.Path), "path", "官方原文裸名（无 page_path 之类前缀）");
        AssertJsonProperty<WxaOrderTradeTypeMaterial>(nameof(WxaOrderTradeTypeMaterial.MediaId), "media_id",
            "官方原文（新增临时素材返回的 media_id，图片最多 10 个、视频最多 3 个属规模限制）");
        AssertJsonProperty<WxaFamousBrandAuditInfo>(nameof(WxaFamousBrandAuditInfo.FlagshipInWhichEcPlatform), "flagship_in_which_ec_platform",
            "官方原文长键名（不得缩写为 ec_platform）");
        AssertPropertyType<WxaFamousBrandAuditInfo>(nameof(WxaFamousBrandAuditInfo.EcPlatformProofList), typeof(List<string>),
            "官方 array of string：元素是 media_id 字符串（官方示例即一串 media_id），不是对象数组");
        AssertPropertyType<WxaFamousBrandAuditInfo>(nameof(WxaFamousBrandAuditInfo.AuthorityCertifiedProofList), typeof(List<string>));
        AssertPropertyType<WxaOrderTradeTypeChangeRequest>(nameof(WxaOrderTradeTypeChangeRequest.MaterialList), typeof(List<WxaOrderTradeTypeMaterial>),
            "官方 objarray ⇒ List<T>?（材料类型 1 图片 / 2 视频）");

        // —— 查询侧订单列表的应答只有三字段（官方返回体表无 errmsg 之外的额外业务键）——
        AssertJsonNameSet<WxaOrderListResponse>("查询订单列表官方返回体表 3 行（errcode/errmsg 由 WxaResponse 承载）",
            "last_index", "has_more", "order_list");
        typeof(WxaOrderListResponse).Should().BeAssignableTo<WxaResponse>("本域应答统一继承 WxaResponse（errcode 判错与全线同源）");
    }

    /// <summary>
    /// 契约守卫 OS5：DTO 全量登记进 AOT JSON 上下文并合并进组件管线；命名空间清单锁死（常量类恰 10）；
    /// 应答面与端点返回类型一一对应；官方枚举取值照原文锁定。
    /// </summary>
    [Fact]
    public void OrderShippingDataModels_ShouldLockOfficialEnumsAndJsonRegistration()
    {
        var assembly = typeof(WxaOrderShippingUploadRequest).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("本域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 30 DTO = 品牌申请 7 + 发货录入与报备 9 + 查询 11 + 设置 3（按接口端点请求体/应答/嵌套对象逐页官方表建模）。
        dtoTypes.Should().HaveCount(30,
            "本域 12 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫"
            + "（famousbrand 7 + shipping 9 + query 11 + setting 3 = 30）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 OrderShipping");
        }

        var contextType = assembly.GetType(DtoNamespace + ".OrderShippingJsonContext");
        contextType.Should().NotBeNull(
            "OrderShippingJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"OrderShipping\")]，"
                + "必须登记进 OrderShippingJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // —— 「登记进上下文」与「合并进组件管线」是两件事（一一对应关系由守卫 XP5 全域锁定，此处只作本域正向自证）。
        var mergeSource = File.ReadAllText(Path.Combine(GetSolutionRoot(),
            "Src/MiniProgram/Mud.Wechat.MiniProgram/Extensions/MiniProgramJsonResolverExtensions.cs"));

        var mergedContextNames = System.Text.RegularExpressions.Regex
            .Matches(mergeSource, @"([A-Za-z]+JsonContext)\.Default")
            .Select(static m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        mergedContextNames.Should().Contain("OrderShippingJsonContext",
            "OrderShippingJsonContext 未合并进组件序列化管线 ⇒ 本域 30 DTO 在 Native AOT 下无生成器路径"
            + "（正对照：数组里 XPay/Vod 两支在册）");
        mergedContextNames.Should().Contain("XPayJsonContext").And.Contain("VodJsonContext",
            "正对照：合并清单确实被读到（读空文件或正则跑偏时上一条会假绿）");

        var constClasses = new[]
        {
            nameof(WxaOrderNumberTypeValues), nameof(WxaOrderDeliveryModeValues), nameof(WxaOrderLogisticsTypeValues),
            nameof(WxaOrderSpecialReportTypeValues), nameof(WxaOrderStateValues),
            nameof(WxaOrderTradeTypeValues), nameof(WxaOrderTradeTypeMaterialTypeValues),
            nameof(WxaFamousBrandApplyForValues), nameof(WxaFamousBrandTypeValues), nameof(WxaFamousBrandApplyStatusValues),
        };

        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType && !constClasses.Contains(t.Name))
            .ToList();

        unregistered.Should().BeEmpty(
            "本域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文、亦非已登记常量类的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        constClasses.Should().HaveCount(10, "本域官方枚举常量类为 10 支（四族各 1~4 支）");
        constClasses.Select(n => assembly.GetType(DtoNamespace + "." + n))
            .Should().OnlyContain(t => t != null, "十支官方枚举常量类必须存在");

        foreach (var name in constClasses)
        {
            var constType = assembly.GetType(DtoNamespace + "." + name)!;
            constType.IsAbstract.Should().BeTrue($"{name} 须为 static class（全仓常量类统一形态）");
            constType.GetFields().Where(static f => f.IsLiteral).Should().NotBeEmpty($"{name} 必须含 const 字段");
        }

        // —— 应答面：5 支域内应答 + 7 支官方空回体直接用 WxaResponse。
        var responseDtos = domainTypes
            .Where(static t => t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();

        responseDtos.Should().HaveCount(5,
            "本域官方应答对象为 5（订单详情 / 订单列表 / 是否开通发货管理 / 是否完成管理确认 / 品牌申请状态）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "本域响应 DTO 必须继承 WxaResponse（errcode 判错与全线同源）");

        var endpointReturnTypes = Routes
            .Select(static r => ResultTypeOf(r.Interface, r.Method))
            .Where(static t => t.Namespace == DtoNamespace)
            .Distinct()
            .ToList();

        endpointReturnTypes.Should().BeEquivalentTo(responseDtos,
            "本域应答类型必须与端点返回类型一一对应（既不得留下无人返回的孤儿应答，也不得返回命名空间外的自建应答）");

        Routes.Count(static r => ResultTypeOf(r.Interface, r.Method) == typeof(WxaResponse))
            .Should().Be(7,
            "七支官方返回体只有 errcode/errmsg 的端点（upload_shipping_info / upload_combined_shipping_info / "
            + "notify_confirm_receive / opspecialorder / set_msg_jump_path / setwxatradetypecgi / famousbrand/apply）"
            + "直接用 WxaResponse，不建空壳应答");

        // —— 官方枚举取值逐表锁定（含官方未公布的取值缺口：品牌申请状态表内无 0，官方示例却给 0）——
        AssertConstValues(typeof(WxaOrderNumberTypeValues), ("MerchantSide", 1), ("WechatPaySide", 2));
        AssertConstValues(typeof(WxaOrderDeliveryModeValues), ("UnifiedDelivery", 1), ("SplitDelivery", 2));
        AssertConstValues(typeof(WxaOrderLogisticsTypeValues),
            ("PhysicalLogistics", 1), ("IntraCityDelivery", 2), ("VirtualGoods", 3), ("UserSelfPickup", 4));
        AssertConstValues(typeof(WxaOrderSpecialReportTypeValues), ("PresaleOrder", 1), ("TestOrder", 2));
        AssertConstValues(typeof(WxaOrderStateValues),
            ("PendingDelivery", 1), ("Delivered", 2), ("Received", 3), ("Completed", 4), ("Refunded", 5), ("PendingSettlement", 6));
        AssertConstValues(typeof(WxaOrderTradeTypeValues),
            ("Comprehensive", 1), ("PhysicalECommerce", 2), ("OfflineService", 3), ("OnlineService", 4));
        AssertConstValues(typeof(WxaOrderTradeTypeMaterialTypeValues), ("Image", 1), ("Video", 2));
        AssertConstValues(typeof(WxaFamousBrandApplyForValues), ("FamousBrand", 1), ("ShopGoodsComponent", 2));
        AssertConstValues(typeof(WxaFamousBrandTypeValues),
            ("IndustryConsumerList", 1), ("CcfaOnlineRetail", 2), ("ChinaTimeHonoredBrand", 3),
            ("ECommerceFlagshipStore", 4), ("GlobalFortune500Brand", 5), ("ChinaFortune500Brand", 6),
            ("AuthorityCertifiedTrademark", 7), ("IndustryGrowingBrandList", 8));
        AssertConstValues(typeof(WxaFamousBrandApplyStatusValues), ("Auditing", 1), ("Rejected", 2), ("Approved", 3));

        // 反直觉点锁定：品牌申请状态的官方取值表只有 1/2/3，而官方「获取申请状态」返回示例里 status = 0
        // ⇒ 0 未被官方枚举表收录，刻意<b>不建</b>常量（建了即把示例值当契约），宿主读该字段须容忍未知值。
        typeof(WxaFamousBrandApplyStatusValues).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral && System.Convert.ToInt32(f.GetRawConstantValue()) == 0)
            .Should().BeEmpty("官方取值表无 0（返回示例里的 0 未入表）⇒ 不得为「覆盖示例」补常量");

        // 订单状态只有 1~6（官方无 0 态，且 6=资金待结算是本域专有语义，与虚拟支付域的 0~10 码表不同源）。
    }

    /// <summary>
    /// 契约守卫 OS6：官方<b>开放面矩阵与空缺</b>的登记面 —— 云调用 7/5、第三方 10/2、权限集两档、
    /// 适用范围两档、错误码四档。各档须<b>无重叠且全覆盖</b>，否则等于「归类靠记忆」。
    /// </summary>
    [Fact]
    public void OrderShippingOpenSurfaceAndErrorCodes_ShouldStayRegistered()
    {
        var allRoutes = Routes.Select(static r => r.Route).ToArray();

        // —— 云调用两档：7 + 5 = 12 ——
        CloudCallRoutes.Should().HaveCount(7, "官方 7 页给出 wxa.sec.order.* 云调用方法名");
        NoCloudCallRoutes.Should().HaveCount(5, "官方 5 页明写「本接口不支持云调用」");
        CloudCallRoutes.Intersect(NoCloudCallRoutes, StringComparer.Ordinal).Should().BeEmpty("云调用两档不得交叉");
        CloudCallRoutes.Concat(NoCloudCallRoutes).OrderBy(static r => r, StringComparer.Ordinal)
            .Should().BeEquivalentTo(allRoutes.OrderBy(static r => r, StringComparer.Ordinal),
            "云调用两档并集须恰为 12 端点全集（新增端点须同批归类）");

        // 云调用面与第三方面<b>不相关</b>：不支持第三方的两支恰恰都支持云调用 ⇒ 不得合并成一张「开放面」表。
        NoThirdPartyRoutes.Should().OnlyContain(r => CloudCallRoutes.Contains(r),
            "官方 get_order_list 与 set_msg_jump_path 不支持第三方平台，但两者都给出云调用方法名 ⇒ 两维独立");

        // —— 第三方与权限集：10 支持（其中 2 支为 18、142）+ 2 不支持 ——
        NoThirdPartyRoutes.Should().HaveCount(2, "官方 2 页明写「本接口不支持第三方平台调用」");
        NoThirdPartyRoutes.Should().OnlyContain(r => allRoutes.Contains(r));
        PermissionSet18And142Routes.Should().HaveCount(2, "官方管理状态两支的权限集 id 为「18、142」");
        PermissionSet18And142Routes.Intersect(NoThirdPartyRoutes, StringComparer.Ordinal).Should().BeEmpty(
            "无权限集行的两支（不支持第三方）不得出现在权限集归类里");

        // —— 适用范围两档：2 明确「小程序」+ 10「暂未明确」——
        MiniProgramScopeRoutes.Should().HaveCount(2, "官方只有品牌申请两页的适用范围段写「支持「小程序」账号类型调用」");
        MiniProgramScopeRoutes.Should().OnlyContain(r => NoCloudCallRoutes.Contains(r),
            "品牌申请两支同时属于「不支持云调用」档 ⇒ 本域唯一「既限定账号类型、又不支持云调用」的组合；其余 10 页适用范围段官方均为「暂未明确」");

        // —— 错误码四档：5 + 2 + 2 + 3 = 12，两两不交、并集为全集 ——
        TradeErrorCodeRoutes.Should().HaveCount(5, "官方 5 页给出 1006xxxx + 2684851xx 交易面码表（28/29/32/28/28 行，五表同源但行数不同）");
        BrandErrorCodeRoutes.Should().HaveCount(2, "品牌申请两支共享 10210001/002/003 码（7 行 / 10 行，后者多 41001/42001/61004）");
        ManagedErrorCodeRoutes.Should().HaveCount(2, "管理状态两支官方错误码表逐行相同（-1 40013 40097 44990 61003 61004 61011）");
        SpecialErrorCodeRoutes.Should().HaveCount(3, "特殊报备（3 行、无 -1）/ 路径设置（仅 -1）/ 交易类型变更（-1 0 268486048）各有独立码表");

        var errGroups = new[] { TradeErrorCodeRoutes, BrandErrorCodeRoutes, ManagedErrorCodeRoutes, SpecialErrorCodeRoutes };
        for (var i = 0; i < errGroups.Length; i++)
        {
            for (var j = i + 1; j < errGroups.Length; j++)
            {
                errGroups[i].Intersect(errGroups[j], StringComparer.Ordinal).Should().BeEmpty(
                    $"错误码档第 {i + 1} / {j + 1} 档交叉 ⇒ 同一端点被两份官方码表同时认领，须先复核官方页面");
            }
        }

        errGroups.SelectMany(static g => g).OrderBy(static r => r, StringComparer.Ordinal)
            .Should().BeEquivalentTo(allRoutes.OrderBy(static r => r, StringComparer.Ordinal),
            "错误码四档并集须恰为 12 端点全集（新增端点须同批归档）");

        // 权限集档的完备性自证：支持第三方的 10 支 = 全集 − 无权限集行的 2 支；其中 2 支为「18、142」，余 8 支仅 142。
        allRoutes.Except(NoThirdPartyRoutes, StringComparer.Ordinal).Should().HaveCount(10,
            "官方 10 页声明「本接口支持第三方平台代商家调用」并给出权限集 id（余 2 页两行皆无）");
        allRoutes.Except(NoThirdPartyRoutes, StringComparer.Ordinal).Except(PermissionSet18And142Routes, StringComparer.Ordinal)
            .Should().HaveCount(8, "8 支的权限集 id 只有 142（两支管理状态查询另有 18）");

        // 每档的组内码表须真的同源（本域四档里只有管理状态两支是「逐行相同」，其余各档内部就有差异）：
        // 以品牌申请档为例，两支共享三支业务码，但 get_status 额外含令牌与 IP 白名单三支 ⇒ 差异留接口注释，
        // 此处只锁「档内至少共有一支业务码」这一弱关系，避免把「同源」误读成「同一张表」。
        ManagedErrorCodeRoutes.Should().HaveCount(2, "档内两支官方表逐行相同（唯一一档如此）");

        // 空缺留档：官方本域 12 页<b>均无端点级频率数字</b>（无「次/分钟」类上限行），
        // 只有规模限制（shipping_list 多重性 [1,15]、材料最多 10 个且视频最多 3 个、page_size 默认 100、item_desc 限 120 字）
        // ⇒ 不得臆造频率上限写进接口注释，也不得在 SDK 侧建限流器（缺口清单侧同批留档）。
        Routes.Should().NotBeEmpty("防「扫描口径失效导致真空断言」");

        // 官方自身的命名缺陷留档：品牌申请两页的接口英文名分别是 famousBrandApply / getFamousBrandApplyStatus，
        // 消息跳转路径页的接口英文名原文错拼为 setMsgJumPpath（大写 P），而其云调用方法名是 setMsgJumpPath
        // ⇒ 路由与 DTO 一律照 HTTPS 契约面（set_msg_jump_path），本条只作留档，不得据英文名改写路由。
        Routes.Select(static r => r.Route).Should().Contain("/wxa/sec/order/set_msg_jump_path",
            "官方接口英文名错拼（setMsgJumPpath）不影响路由面 ⇒ 路由照 order_shipping 分区原文");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>）。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    /// <summary>取端点方法的 <c>[Body]</c> 请求 DTO 类型（零请求体端点返回 <c>null</c>）。</summary>
    private static Type? RequestTypeOf(Type interfaceType, string methodName)
        => interfaceType
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetParameters()
            .SingleOrDefault(static p => p.GetCustomAttribute<BodyAttribute>() != null)
            ?.ParameterType;

    /// <summary>取接口声明的方法数（DeclaredOnly，不含继承面）。</summary>
    private static int MethodCount(Type type)
        => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;

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
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}（不对称/口径裁决{(because.Length > 0 ? "：" + because : string.Empty)}）");
    }

    /// <summary>断言属性<b>不存在</b>：官方两张表的字段差集，合并 DTO 即在此处报红。</summary>
    private static void AssertPropertyAbsent<T>(string propertyName, string because)
        where T : class
        => typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull($"{typeof(T).Name} 不应出现 {propertyName} —— 官方字段表无该列（{because}）");

    /// <summary>断言某 DTO 的 <c>[JsonPropertyName]</c> 集合（按声明顺序）恰为给定官方字段表。</summary>
    private static void AssertJsonNameSet<T>(string because, params string[] expected)
        where T : class
    {
        var actual = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

        actual.Should().NotBeEmpty($"{typeof(T).Name} 必须带显式 [JsonPropertyName]（AOT 生成器口径）");
        actual.Should().Equal(expected,
            $"{typeof(T).Name} 的官方字段表须逐行一致（{because}；增删改序都要先回官方页面）");
    }

    private static void AssertConstValues(Type constType, params (string Name, object Value)[] expected)
    {
        var actual = constType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .ToDictionary(static f => f.Name, static f => f.GetRawConstantValue()!);

        actual.Should().BeEquivalentTo(expected.ToDictionary(static e => e.Name, static e => e.Value),
            because: $"{constType.Name} 的官方取值集漂移须先核对官方枚举表再同批调整本守卫");
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
}
