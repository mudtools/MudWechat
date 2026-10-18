// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Match = System.Text.RegularExpressions.Match;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「自定义交易组件 V2」（<c>/shop/*</c>）商店账号 / 资金 / 分享员 / 商品 / 优惠券 / 订单六域契约守卫
/// （SHOPV2 系列：74 端点路由与动词 + 令牌绑定与注册组 + DTO 上下文登记 + 反直觉点与跨线负锁）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成</b>：ShopAccount 16 + ShopSpu 9 + ShopOrder 20 + ShopAfterSale 18 + ShopCoupon 11 = 74 端点
/// （ShopFunds 9 与 ShopSharer 8 已有独立守卫 SHOPF / SHOPSH）。六域全部消费小程序 <c>access_token</c>（Query），
/// 路由族 <c>/shop/*</c>，与小店线 <c>/channels/ec/*</c> 的独立令牌面零交叉。
/// </para>
/// <para>
/// <b>逐页取证锁定的反直觉点</b>：
/// ① 确认收货官方页文件名拼作 <c>recieve.html</c>，但「请求地址」原文是 <c>/shop/delivery/receive</c> ⇒ 路由取请求地址；
/// ② 全族查询 / 列表端点官方一律 POST（含 get_list / search），勿顺手改 GET；
/// ③ V2 售后两套契约并存：<c>/shop/ecaftersale/*</c>（11）与 <c>/shop/aftersale/*</c>（4）分属两个接口，
/// 交易纠纷 <c>/shop/complaint/*</c>（3）单列，三者在同一注册组内不得互抄路由或状态枚举；
/// ④ 优惠券用户券族路由仍落 <c>/shop/coupon/*</c>（官方原文），不得改写成 <c>/shop/user_coupon/*</c>。
/// </para>
/// </remarks>
public class WxaShopV2ContractGuards
{
    /// <summary>74 端点期望表（域, 接口, 方法, 路由）。全部官方 POST。</summary>
    private static readonly (string Domain, Type Interface, string Method, string Route)[] Routes =
    {
        // ShopAccount 16：账号与信息 4 + 审核 4 + 类目 1 + 图片 1 + 推客 1 + 入驻准入 4 + 场景 1
        ("ShopAccount", typeof(IWxaShopAccountService), nameof(IWxaShopAccountService.GetBrandListAsync), "/shop/account/get_brand_list"),
        ("ShopAccount", typeof(IWxaShopAccountService), nameof(IWxaShopAccountService.GetCategoryListAsync), "/shop/account/get_category_list"),
        ("ShopAccount", typeof(IWxaShopAccountService), nameof(IWxaShopAccountService.GetInfoAsync), "/shop/account/get_info"),
        ("ShopAccount", typeof(IWxaShopAccountService), nameof(IWxaShopAccountService.UpdateInfoAsync), "/shop/account/update_info"),
        ("ShopAccount", typeof(IWxaShopAuditService), nameof(IWxaShopAuditService.AuditBrandAsync), "/shop/audit/audit_brand"),
        ("ShopAccount", typeof(IWxaShopAuditService), nameof(IWxaShopAuditService.AuditCategoryAsync), "/shop/audit/audit_category"),
        ("ShopAccount", typeof(IWxaShopAuditService), nameof(IWxaShopAuditService.GetMiniappCertificateAsync), "/shop/audit/get_miniapp_certificate"),
        ("ShopAccount", typeof(IWxaShopAuditService), nameof(IWxaShopAuditService.GetAuditResultAsync), "/shop/audit/result"),
        ("ShopAccount", typeof(IWxaShopCategoryService), nameof(IWxaShopCategoryService.GetThirdCategoryListAsync), "/shop/cat/get"),
        ("ShopAccount", typeof(IWxaShopImageService), nameof(IWxaShopImageService.UploadImageAsync), "/shop/img/upload"),
        ("ShopAccount", typeof(IWxaShopPromoterService), nameof(IWxaShopPromoterService.GetPromoterListAsync), "/shop/promoter/list"),
        ("ShopAccount", typeof(IWxaShopRegisterService), nameof(IWxaShopRegisterService.ApplyAccessAsync), "/shop/register/apply"),
        ("ShopAccount", typeof(IWxaShopRegisterService), nameof(IWxaShopRegisterService.ApplySceneAsync), "/shop/register/apply_scene"),
        ("ShopAccount", typeof(IWxaShopRegisterService), nameof(IWxaShopRegisterService.CheckAccessAsync), "/shop/register/check"),
        ("ShopAccount", typeof(IWxaShopRegisterService), nameof(IWxaShopRegisterService.FinishAccessInfoAsync), "/shop/register/finish_access_info"),
        ("ShopAccount", typeof(IWxaShopSceneService), nameof(IWxaShopSceneService.CheckSceneAsync), "/shop/scene/check"),

        // ShopSpu 9：新增 / 删除 / 删除审核 / 下架 / 详情 / 列表 / 上架 / 更新 / 免审更新
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.AddSpuAsync), "/shop/spu/add"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.DeleteSpuAsync), "/shop/spu/del"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.DeleteSpuAuditAsync), "/shop/spu/del_audit"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.DelistSpuAsync), "/shop/spu/delisting"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.GetSpuAsync), "/shop/spu/get"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.GetSpuListAsync), "/shop/spu/get_list"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.ListSpuAsync), "/shop/spu/listing"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.UpdateSpuAsync), "/shop/spu/update"),
        ("ShopSpu", typeof(IWxaShopSpuService), nameof(IWxaShopSpuService.UpdateSpuWithoutAuditAsync), "/shop/spu/update_without_audit"),

        // ShopOrder 20：配送 3 + 订单 8 + 支付 6 + 微信支付 3
        ("ShopOrder", typeof(IWxaShopDeliveryService), nameof(IWxaShopDeliveryService.GetDeliveryCompanyListAsync), "/shop/delivery/get_company_list"),
        ("ShopOrder", typeof(IWxaShopDeliveryService), nameof(IWxaShopDeliveryService.ReceiveDeliveryAsync), "/shop/delivery/receive"),
        ("ShopOrder", typeof(IWxaShopDeliveryService), nameof(IWxaShopDeliveryService.SendDeliveryAsync), "/shop/delivery/send"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.AddOrderAsync), "/shop/order/add"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.CloseOrderAsync), "/shop/order/close"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.GetOrderAsync), "/shop/order/get"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.GetOrderListAsync), "/shop/order/get_list"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.GetOrderListByFinderAsync), "/shop/order/get_list_by_finder"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.GetOrderListBySharerAsync), "/shop/order/get_list_by_sharer"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.GetPaymentParamsAsync), "/shop/order/getpaymentparams"),
        ("ShopOrder", typeof(IWxaShopOrderService), nameof(IWxaShopOrderService.PayOrderAsync), "/shop/order/pay"),
        ("ShopOrder", typeof(IWxaShopPayService), nameof(IWxaShopPayService.CreateOrderAsync), "/shop/pay/createorder"),
        ("ShopOrder", typeof(IWxaShopPayService), nameof(IWxaShopPayService.CloseOrderAsync), "/shop/pay/closeorder"),
        ("ShopOrder", typeof(IWxaShopPayService), nameof(IWxaShopPayService.GetOrderAsync), "/shop/pay/getorder"),
        ("ShopOrder", typeof(IWxaShopPayService), nameof(IWxaShopPayService.GetOrderListAsync), "/shop/pay/getorderlist"),
        ("ShopOrder", typeof(IWxaShopPayService), nameof(IWxaShopPayService.ProfitSharingOrderAsync), "/shop/pay/profitsharingorder"),
        ("ShopOrder", typeof(IWxaShopPayService), nameof(IWxaShopPayService.RefundOrderAsync), "/shop/pay/refundorder"),
        ("ShopOrder", typeof(IWxaShopWxPayService), nameof(IWxaShopWxPayService.ApplyAsync), "/shop/wxpay/apply"),
        ("ShopOrder", typeof(IWxaShopWxPayService), nameof(IWxaShopWxPayService.GetAsync), "/shop/wxpay/get"),
        ("ShopOrder", typeof(IWxaShopWxPayService), nameof(IWxaShopWxPayService.GetInfoAsync), "/shop/wxpay/get_info"),

        // ShopAfterSale 18：V2 售后 11 + V1 售后 4 + 交易纠纷 3
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.AddAftersaleAsync), "/shop/ecaftersale/add"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.CancelAftersaleAsync), "/shop/ecaftersale/cancel"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.GetAftersaleAsync), "/shop/ecaftersale/get"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.GetAftersaleListAsync), "/shop/ecaftersale/get_list"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.UpdateAftersaleAsync), "/shop/ecaftersale/update"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.AcceptRefundAsync), "/shop/ecaftersale/acceptrefund"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.AcceptReturnAsync), "/shop/ecaftersale/acceptreturn"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.RejectAftersaleAsync), "/shop/ecaftersale/reject"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.UploadCertificatesAsync), "/shop/ecaftersale/upload_certificates"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.UploadReturnInfoAsync), "/shop/ecaftersale/uploadreturninfo"),
        ("ShopAfterSale", typeof(IWxaEcAftersaleService), nameof(IWxaEcAftersaleService.MerchantTestAsync), "/shop/ecaftersale/merchanttest"),
        ("ShopAfterSale", typeof(IWxaShopAftersaleService), nameof(IWxaShopAftersaleService.AddAftersaleAsync), "/shop/aftersale/add"),
        ("ShopAfterSale", typeof(IWxaShopAftersaleService), nameof(IWxaShopAftersaleService.GetAftersaleAsync), "/shop/aftersale/get"),
        ("ShopAfterSale", typeof(IWxaShopAftersaleService), nameof(IWxaShopAftersaleService.UpdateAftersaleAsync), "/shop/aftersale/update"),
        ("ShopAfterSale", typeof(IWxaShopAftersaleService), nameof(IWxaShopAftersaleService.UpdateAftersaleDeadlineAsync), "/shop/aftersale/update_deadline"),
        ("ShopAfterSale", typeof(IWxaShopComplaintService), nameof(IWxaShopComplaintService.GetComplaintAsync), "/shop/complaint/get"),
        ("ShopAfterSale", typeof(IWxaShopComplaintService), nameof(IWxaShopComplaintService.GetComplaintListAsync), "/shop/complaint/get_list"),
        ("ShopAfterSale", typeof(IWxaShopComplaintService), nameof(IWxaShopComplaintService.UploadComplaintMaterialAsync), "/shop/complaint/upload_material"),

        // ShopCoupon 11：商家券 7 + 用户券 4
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.ConfirmCouponCallbackAsync), "/shop/coupon/confirm"),
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.AddCouponAsync), "/shop/coupon/add"),
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.UpdateCouponAsync), "/shop/coupon/update"),
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.GetCouponAsync), "/shop/coupon/get"),
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.GetCouponListAsync), "/shop/coupon/get_list"),
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.UpdateCouponStockAsync), "/shop/coupon/update_coupon_stock"),
        ("ShopCoupon", typeof(IWxaShopCouponService), nameof(IWxaShopCouponService.UpdateCouponStatusAsync), "/shop/coupon/update_status"),
        ("ShopCoupon", typeof(IWxaShopUserCouponService), nameof(IWxaShopUserCouponService.AddUserCouponAsync), "/shop/coupon/add_user_coupon"),
        ("ShopCoupon", typeof(IWxaShopUserCouponService), nameof(IWxaShopUserCouponService.UpdateUserCouponAsync), "/shop/coupon/update_user_coupon"),
        ("ShopCoupon", typeof(IWxaShopUserCouponService), nameof(IWxaShopUserCouponService.GetUserCouponListAsync), "/shop/coupon/get_usercoupon_list"),
        ("ShopCoupon", typeof(IWxaShopUserCouponService), nameof(IWxaShopUserCouponService.UpdateUserCouponStatusAsync), "/shop/coupon/update_usercoupon_status"),
    };

    /// <summary>域 → DTO 命名空间 / 注册组 / DTO 总数 / 接口数。</summary>
    private static readonly (string Domain, int DtoCount, int InterfaceCount)[] Domains =
    {
        ("ShopAccount", 38, 7),
        ("ShopSpu", 18, 1),
        ("ShopOrder", 76, 4),
        ("ShopAfterSale", 37, 3),
        ("ShopCoupon", 31, 2),
    };

    /// <summary>契约守卫 SHOPV2-1：74 端点路由与动词逐条对齐官方（全 POST），声明面与路由表全集相等。</summary>
    [Fact]
    public void ShopV2Endpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(74,
            "V2 商店六域（ShopFunds / ShopSharer 另守卫）合计 74 端点：ShopAccount 16 + ShopSpu 9 + ShopOrder 20 + ShopAfterSale 18 + ShopCoupon 11");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(74, "路由表不得出现重复条目");

        foreach (var (_, iface, method, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方「请求地址」原文一致");
        }

        Routes.Should().OnlyContain(static r => r.Route.StartsWith("/shop/", StringComparison.Ordinal),
            "V2 六域官方路由一律 /shop/ 前缀");
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "官方路由一律全小写下划线连写（不得驼峰化）");
        Routes.Should().NotContain(static r => r.Route.Contains("/channels/", StringComparison.Ordinal),
            "/shop/* 属小程序 access_token 面，与小店线 /channels/ec/* 不得混用");

        // 反直觉点①：页面文件名 recieve.html 不是路由名，请求地址原文为 receive。
        Routes.Should().Contain(static r => r.Route == "/shop/delivery/receive",
            "确认收货官方「请求地址」原文 /shop/delivery/receive（页面文件名 recieve.html 系官方拼写笔误）");
        Routes.Should().NotContain(static r => r.Route.EndsWith("/recieve", StringComparison.Ordinal),
            "不得照页面文件名把路由写成 /shop/delivery/recieve");
        // 总账名册按页面文件名记的整条路由（SKIT 行）在此显式钉死：它是笔误、不是端点。
        Routes.Should().NotContain(static r => r.Route == "/shop/delivery/recieve",
            "SKIT 清单的 /shop/delivery/recieve 是页面文件名笔误，不得作为契约路由");

        // 声明面全集自证：五个 Interfaces/<域>/ 目录的路由字面量与路由表逐条相等。
        var declared = Domains
            .SelectMany(static d => RouteLiteralsUnder(Path.Combine("Src", "MiniProgram", "Mud.Wechat.MiniProgram", "Interfaces", d.Domain)))
            .ToHashSet(StringComparer.Ordinal);
        declared.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：必须真的扫出已声明路由");
        declared.Should().BeEquivalentTo(Routes.Select(static r => r.Route),
            "Interfaces/{ShopAccount,ShopSpu,ShopOrder,ShopAfterSale,ShopCoupon}/ 的声明面必须与 SHOPV2-1 路由表逐条相等");
    }

    /// <summary>契约守卫 SHOPV2-2：令牌绑定（Query access_token）、注册组、模块枚举与建造器入口。</summary>
    [Fact]
    public void ShopV2Interfaces_ShouldBindMpAccessToken_AndRegisterModules()
    {
        var interfaces = Routes.Select(static r => r.Interface).Distinct().ToArray();
        interfaces.Should().HaveCount(17, "V2 五域（除 ShopFunds / ShopSharer）共 17 支声明式接口（域内接口数 7+1+4+3+2）");

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用小程序 access_token（改绑小店/企微令牌会让单槽 Resolve 返回 null）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约把 access_token 强制放 Query");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull($"{iface.Name} 必须声明 [HttpClientApi]");
            api!.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序令牌底座（AddMpApp 硬前置）");

            var domain = Routes.First(r => r.Interface == iface).Domain;
            api.RegistryGroupName.Should().Be(domain, $"{iface.Name} 必须挂 {domain} 注册组");
        }

        foreach (var (domain, _, interfaceCount) in Domains)
        {
            interfaces.Count(i => Routes.First(r => r.Interface == i).Domain == domain)
                .Should().Be(interfaceCount, $"{domain} 接口数漂移");

            Enum.TryParse<MiniProgramModule>(domain, out _).Should().BeTrue(
                $"MiniProgramModule 必须含 {domain} 枚举值（缺枚举 ⇒ 模块永远无法注册）");
            typeof(MiniProgramServiceBuilder).GetMethod("Add" + domain + "Api", Type.EmptyTypes)
                .Should().NotBeNull($"MiniProgramServiceBuilder 必须提供 Add{domain}Api() 入口");
        }
    }

    /// <summary>契约守卫 SHOPV2-3：DTO 总数、SerializerClassName 与 JsonContext 全量登记（AOT 生死线）。</summary>
    [Fact]
    public void ShopV2DataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaResponse).Assembly;

        foreach (var (domain, expectedCount, _) in Domains)
        {
            var ns = "Mud.Wechat.MiniProgram.DataModels." + domain;
            var dtoTypes = assembly.GetTypes()
                .Where(t => !t.IsNested && t.Namespace == ns && t.IsClass && !t.IsAbstract
                            && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
                .ToList();

            dtoTypes.Should().HaveCount(expectedCount,
                $"{domain} 域 DTO 总数漂移须先核对官方文档再同批调整本守卫");

            foreach (var type in dtoTypes)
            {
                type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                    .Should().Be(domain, $"{type.Name} 的 SerializerClassName 必须为 {domain}");
            }

            var contextType = assembly.GetType(ns + "." + domain + "JsonContext");
            contextType.Should().NotBeNull($"{domain}JsonContext 尚未生成：跑 scripts/GenerateJsonContext.ps1");

            var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");
            var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

            foreach (var type in dtoTypes)
            {
                jsonContext.GetTypeInfo(type).Should().NotBeNull(
                    $"{type.Name} 必须登记进 {domain}JsonContext（AOT 源生成缺失即运行期抛异常）");
            }

            assembly.GetTypes()
                .Where(t => t.Namespace == ns && t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
                .Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
                    $"{domain} 域应答 DTO 必须继承 WxaResponse（errcode 判错与全线同源）");
        }
    }

    /// <summary>契约守卫 SHOPV2-4：V2 与 V1 分代隔离——接口不得引用对代 DTO，路由不得互抄。</summary>
    [Fact]
    public void ShopV2_ShouldStaySeparatedFromProductV1()
    {
        var v2Namespaces = Domains.Select(static d => "Mud.Wechat.MiniProgram.DataModels." + d).ToArray();
        var v1Namespaces = new[]
        {
            "Mud.Wechat.MiniProgram.DataModels.ProductCatalog",
            "Mud.Wechat.MiniProgram.DataModels.ProductSku",
            "Mud.Wechat.MiniProgram.DataModels.ProductOrder",
            "Mud.Wechat.MiniProgram.DataModels.ProductCoupon",
            "Mud.Wechat.MiniProgram.DataModels.ProductFunds",
            "Mud.Wechat.MiniProgram.DataModels.ProductMedia",
        };

        foreach (var iface in Routes.Select(static r => r.Interface).Distinct())
        {
            foreach (var method in iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                foreach (var parameter in method.GetParameters())
                {
                    var ns = parameter.ParameterType.IsGenericType
                        ? parameter.ParameterType.GetGenericArguments()[0].Namespace
                        : parameter.ParameterType.Namespace;
                    v1Namespaces.Should().NotContain(ns,
                        $"{iface.Name}.{method.Name} 不得引用 V1 DTO（两代契约不得互用类型）");
                }

                var resultNs = method.ReturnType.IsGenericType
                    ? method.ReturnType.GetGenericArguments()[0].Namespace
                    : method.ReturnType.Namespace;
                v1Namespaces.Should().NotContain(resultNs,
                    $"{iface.Name}.{method.Name} 不得返回 V1 应答 DTO");
            }
        }

        v2Namespaces.Should().NotBeEmpty();
    }

    /// <summary>扫描指定目录代码行中的路由字面量（剥离注释与构建产物）。</summary>
    private static IEnumerable<string> RouteLiteralsUnder(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
        var regex = new Regex("\"(/shop/[A-Za-z0-9_\\-./]+)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal))
            {
                continue;
            }

            var code = string.Join("\n", File.ReadAllLines(file)
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (Match m in regex.Matches(code))
            {
                yield return m.Groups[1].Value;
            }
        }
    }

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
