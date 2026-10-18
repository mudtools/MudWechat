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
/// 小程序线「基础交易工具（原标准版交易组件 V1）」（<c>/product/*</c>）商品 / SKU / 订单 / 优惠券 / 素材五域契约守卫
/// （PRODUCTV1 系列：73 端点路由与动词 + 令牌绑定与注册组 + DTO 上下文登记 + 双形态与文档缺口裁决）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成</b>：ProductCatalog 22 + ProductSku 17 + ProductOrder 14 + ProductCoupon 10 + ProductMedia 10 = 73 端点
/// （ProductFunds 7 已有独立守卫 PF）。全域消费小程序 <c>access_token</c>（Query），路由族 <c>/product/*</c>。
/// </para>
/// <para>
/// <b>逐页取证锁定的文档缺口裁决</b>：
/// ① <c>/product/store/get_info</c> 在 SKIT 派生清单里动词列为空，官方页正文为 POST ⇒ 按 POST 落地；
/// ② <c>/product/store/get_shopcat</c> 在清单中缺失，官方门店分类页在册 ⇒ 补入本域（22 = 清单 21 + 1）；
/// ③ <c>/product/img/upload</c> 一条路由承载两种形态：<c>upload_type=0</c> 二进制 multipart 与
/// <c>upload_type=1</c> 图片 URL JSON ⇒ 两个方法同路由，计数按 1 条（73 而非 74）；
/// ④ <c>/product/order/*</c> 的退款审核与售后查询是两套端点（acceptapply / rejectrefund / getaftersaleorder /
/// batchgetaftersaleorder），不得合并成单端点。
/// </para>
/// </remarks>
public class WxaProductV1ContractGuards
{
    /// <summary>74 个方法 / 73 条去重路由（上传图片双形态同路由）。全部官方 POST。</summary>
    private static readonly (string Domain, Type Interface, string Method, string Route)[] Routes =
    {
        // ProductCatalog 22：分类与品牌 2 + 配送 2 + 运费 3 + 线下 4 + 入驻 4 + 服务市场 3 + 物流方式 2 + 门店 2
        ("ProductCatalog", typeof(IWxaProductCatalogService), nameof(IWxaProductCatalogService.GetCategoryListAsync), "/product/category/get"),
        ("ProductCatalog", typeof(IWxaProductCatalogService), nameof(IWxaProductCatalogService.GetBrandListAsync), "/product/brand/get"),
        ("ProductCatalog", typeof(IWxaProductDeliveryService), nameof(IWxaProductDeliveryService.GetDeliveryCompanyListAsync), "/product/delivery/get_company_list"),
        ("ProductCatalog", typeof(IWxaProductDeliveryService), nameof(IWxaProductDeliveryService.SendDeliveryAsync), "/product/delivery/send"),
        ("ProductCatalog", typeof(IWxaProductFreightService), nameof(IWxaProductFreightService.GetFreightTemplateListAsync), "/product/delivery/get_freight_template"),
        ("ProductCatalog", typeof(IWxaProductFreightService), nameof(IWxaProductFreightService.InsertFreightTemplateAsync), "/product/delivery/insert_freight_template"),
        ("ProductCatalog", typeof(IWxaProductFreightService), nameof(IWxaProductFreightService.UpdateFreightTemplateAsync), "/product/delivery/update_freight_template"),
        ("ProductCatalog", typeof(IWxaProductOfflineService), nameof(IWxaProductOfflineService.GetPickupTemplateAsync), "/product/offline/get_pickup_template"),
        ("ProductCatalog", typeof(IWxaProductOfflineService), nameof(IWxaProductOfflineService.UpdatePickupTemplateAsync), "/product/offline/update_pickup_template"),
        ("ProductCatalog", typeof(IWxaProductOfflineService), nameof(IWxaProductOfflineService.GetSameCityTemplateAsync), "/product/offline/get_same_city_template"),
        ("ProductCatalog", typeof(IWxaProductOfflineService), nameof(IWxaProductOfflineService.UpdateSameCityTemplateAsync), "/product/offline/update_same_city_template"),
        ("ProductCatalog", typeof(IWxaProductRegisterService), nameof(IWxaProductRegisterService.SubmitBasicInfoAsync), "/product/register/submit_basicinfo"),
        ("ProductCatalog", typeof(IWxaProductRegisterService), nameof(IWxaProductRegisterService.SubmitMerchantInfoAsync), "/product/register/submit_merchantinfo"),
        ("ProductCatalog", typeof(IWxaProductRegisterService), nameof(IWxaProductRegisterService.CheckAuditStatusAsync), "/product/register/check_audit_status"),
        ("ProductCatalog", typeof(IWxaProductRegisterService), nameof(IWxaProductRegisterService.RegisterShopAsync), "/product/register/register_shop"),
        ("ProductCatalog", typeof(IWxaProductServiceMarketService), nameof(IWxaProductServiceMarketService.CheckServiceAuthAsync), "/product/service/check_auth"),
        ("ProductCatalog", typeof(IWxaProductServiceMarketService), nameof(IWxaProductServiceMarketService.GetServiceListAsync), "/product/service/get_list"),
        ("ProductCatalog", typeof(IWxaProductServiceMarketService), nameof(IWxaProductServiceMarketService.GetServiceOrderListAsync), "/product/service/get_order_list"),
        ("ProductCatalog", typeof(IWxaProductShipMethodService), nameof(IWxaProductShipMethodService.GetShippingMethodsAsync), "/product/shipmethods/get"),
        ("ProductCatalog", typeof(IWxaProductShipMethodService), nameof(IWxaProductShipMethodService.SetShippingMethodsAsync), "/product/shipmethods/set"),
        ("ProductCatalog", typeof(IWxaProductStoreService), nameof(IWxaProductStoreService.GetStoreInfoAsync), "/product/store/get_info"),
        ("ProductCatalog", typeof(IWxaProductStoreService), nameof(IWxaProductStoreService.GetShopCategoryListAsync), "/product/store/get_shopcat"),

        // ProductSku 17：SKU 7 + SPU 8 + 库存 2
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.AddSkuAsync), "/product/sku/add"),
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.BatchAddSkuAsync), "/product/sku/batch_add"),
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.DeleteSkuAsync), "/product/sku/del"),
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.GetSkuAsync), "/product/sku/get"),
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.GetSkuListAsync), "/product/sku/get_list"),
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.UpdateSkuAsync), "/product/sku/update"),
        ("ProductSku", typeof(IWxaProductSkuService), nameof(IWxaProductSkuService.UpdateSkuPriceAsync), "/product/sku/update_price"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.AddSpuAsync), "/product/spu/add"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.DeleteSpuAsync), "/product/spu/del"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.DelistSpuAsync), "/product/spu/delisting"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.GetSpuAsync), "/product/spu/get"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.GetSpuListAsync), "/product/spu/get_list"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.ListSpuAsync), "/product/spu/listing"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.SearchSpuAsync), "/product/spu/search"),
        ("ProductSku", typeof(IWxaProductSpuService), nameof(IWxaProductSpuService.UpdateSpuAsync), "/product/spu/update"),
        ("ProductSku", typeof(IWxaProductStockService), nameof(IWxaProductStockService.GetStockAsync), "/product/stock/get"),
        ("ProductSku", typeof(IWxaProductStockService), nameof(IWxaProductStockService.UpdateStockAsync), "/product/stock/update"),

        // ProductOrder 14：地址 4 + 订单 10
        ("ProductOrder", typeof(IWxaProductAddressService), nameof(IWxaProductAddressService.AddAddressAsync), "/product/address/add"),
        ("ProductOrder", typeof(IWxaProductAddressService), nameof(IWxaProductAddressService.DeleteAddressAsync), "/product/address/delete"),
        ("ProductOrder", typeof(IWxaProductAddressService), nameof(IWxaProductAddressService.GetAddressAsync), "/product/address/get"),
        ("ProductOrder", typeof(IWxaProductAddressService), nameof(IWxaProductAddressService.UpdateAddressAsync), "/product/address/update"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.AcceptRefundApplyAsync), "/product/order/acceptapply"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.BatchGetAfterSaleOrderAsync), "/product/order/batchgetaftersaleorder"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.ChangeMerchantNotesAsync), "/product/order/change_merchant_notes"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.ChangeOrderPriceAsync), "/product/order/change_order_price"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.GetOrderAsync), "/product/order/get"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.GetOrderListAsync), "/product/order/get_list"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.GetAfterSaleOrderAsync), "/product/order/getaftersaleorder"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.RejectRefundAsync), "/product/order/rejectrefund"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.SearchOrderAsync), "/product/order/search"),
        ("ProductOrder", typeof(IWxaProductOrderService), nameof(IWxaProductOrderService.UploadRefundCertificateAsync), "/product/order/upload_refund_certificate"),

        // ProductCoupon 10：优惠券 7 + 限时抢购 3
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.CreateCouponAsync), "/product/coupon/create"),
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.GetCouponAsync), "/product/coupon/get"),
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.GetCouponListAsync), "/product/coupon/get_list"),
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.GetUserCouponAsync), "/product/coupon/get_user"),
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.PushCouponAsync), "/product/coupon/push"),
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.UpdateCouponAsync), "/product/coupon/update"),
        ("ProductCoupon", typeof(IWxaProductCouponService), nameof(IWxaProductCouponService.UpdateCouponStatusAsync), "/product/coupon/update_status"),
        ("ProductCoupon", typeof(IWxaProductLimitedDiscountService), nameof(IWxaProductLimitedDiscountService.AddLimitedDiscountAsync), "/product/limiteddiscount/add"),
        ("ProductCoupon", typeof(IWxaProductLimitedDiscountService), nameof(IWxaProductLimitedDiscountService.GetLimitedDiscountListAsync), "/product/limiteddiscount/get_list"),
        ("ProductCoupon", typeof(IWxaProductLimitedDiscountService), nameof(IWxaProductLimitedDiscountService.UpdateLimitedDiscountStatusAsync), "/product/limiteddiscount/update_status"),

        // ProductMedia 11 方法 / 10 条路由：装修 6 + 图片上传 2（同路由）+ 二维码 3
        ("ProductMedia", typeof(IWxaProductDecorationService), nameof(IWxaProductDecorationService.SetDecorationServiceAsync), "/product/decoration/decoration_service"),
        ("ProductMedia", typeof(IWxaProductDecorationService), nameof(IWxaProductDecorationService.OpenDecorationServiceExperienceAsync), "/product/decoration/decoration_service_experience"),
        ("ProductMedia", typeof(IWxaProductDecorationService), nameof(IWxaProductDecorationService.CheckDecorationStatusAsync), "/product/decoration/wxaproduct_decoration_check_status"),
        ("ProductMedia", typeof(IWxaProductDecorationService), nameof(IWxaProductDecorationService.StartPressureTestAsync), "/product/decoration/wxaproduct_decoration_pressure_test"),
        ("ProductMedia", typeof(IWxaProductDecorationService), nameof(IWxaProductDecorationService.GetPressureTestStatusAsync), "/product/decoration/wxaproduct_decoration_query_pressure_test_status"),
        ("ProductMedia", typeof(IWxaProductDecorationService), nameof(IWxaProductDecorationService.GetPressureTestReportAsync), "/product/decoration/wxaproduct_decoration_get_pressure_test_report"),
        ("ProductMedia", typeof(IWxaProductImageService), nameof(IWxaProductImageService.UploadImageByBinaryAsync), "/product/img/upload"),
        ("ProductMedia", typeof(IWxaProductImageService), nameof(IWxaProductImageService.UploadImageByUrlAsync), "/product/img/upload"),
        ("ProductMedia", typeof(IWxaProductQrCodeService), nameof(IWxaProductQrCodeService.GenQrCodeAsync), "/product/qrcode/gen"),
        ("ProductMedia", typeof(IWxaProductQrCodeService), nameof(IWxaProductQrCodeService.GetQrCodeAsync), "/product/qrcode/get"),
        ("ProductMedia", typeof(IWxaProductQrCodeService), nameof(IWxaProductQrCodeService.CheckQrCodeAsync), "/product/qrcode/check"),
    };

    private static readonly (string Domain, int RouteCount, int DtoCount, int InterfaceCount)[] Domains =
    {
        ("ProductCatalog", 22, 72, 8),
        ("ProductSku", 17, 47, 3),
        ("ProductOrder", 14, 47, 2),
        ("ProductCoupon", 10, 34, 2),
        ("ProductMedia", 10, 19, 3),
    };

    /// <summary>契约守卫 PRODUCTV1-1：73 条路由与动词逐条对齐官方（全 POST），声明面与路由表全集相等。</summary>
    [Fact]
    public void ProductV1Endpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(74,
            "本守卫覆盖 74 个方法：73 条去重路由 + /product/img/upload 双形态各一方法");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(73,
            "73 条去重路由（ProductMedia 的 /product/img/upload 二进制与 URL 双形态共路由）");

        foreach (var (_, iface, method, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方「请求地址」原文一致");
        }

        Routes.Should().OnlyContain(static r => r.Route.StartsWith("/product/", StringComparison.Ordinal),
            "V1 五域官方路由一律 /product/ 前缀");
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "官方路由一律全小写（delisting / update_price 等下划线连写，不得驼峰化）");

        // 文档缺口裁决①：store/get_info 官方页正文为 POST。
        Routes.Should().Contain(static r => r.Route == "/product/store/get_info",
            "官方门店信息页为 POST（SKIT 清单动词列为空系派生缺口）");
        // 文档缺口裁决②：get_shopcat 官方页在册但清单缺失。
        Routes.Should().Contain(static r => r.Route == "/product/store/get_shopcat",
            "门店分类 get_shopcat 官方页在册，属本域补回的第 22 支");

        // 上传双形态：二进制流走 [MultipartForm]，URL 模式走 [Body]。
        var uploadMethods = typeof(IWxaProductImageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.GetCustomAttribute<PostAttribute>()?.RequestUri == "/product/img/upload")
            .ToArray();
        uploadMethods.Should().HaveCount(2, "上传图片一条路由两种形态（upload_type=0/1）");
        uploadMethods.Count(static m => m.GetParameters().Any(p => p.GetCustomAttribute<MultipartFormAttribute>() != null))
            .Should().Be(1, "二进制流形态恰有一个 [MultipartForm] 参数");
        uploadMethods.Count(static m => m.GetParameters().Any(p => p.GetCustomAttribute<BodyAttribute>() != null))
            .Should().Be(1, "图片 URL 形态恰有一个 [Body] 参数");

        // 声明面全集自证：五个 Interfaces/<域>/ 目录的路由字面量（去重）与路由表相等。
        var declared = Domains
            .SelectMany(static d => RouteLiteralsUnder(Path.Combine("Src", "MiniProgram", "Mud.Wechat.MiniProgram", "Interfaces", d.Domain)))
            .ToHashSet(StringComparer.Ordinal);
        declared.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：必须真的扫出已声明路由");
        declared.Should().BeEquivalentTo(Routes.Select(static r => r.Route).Distinct(),
            "Interfaces/{ProductCatalog,ProductSku,ProductOrder,ProductCoupon,ProductMedia}/ 的声明面必须与 PRODUCTV1-1 路由表逐条相等");

        // 两代分代：V1 接口不得出现 /shop/ 路由。
        Routes.Should().NotContain(static r => r.Route.StartsWith("/shop/", StringComparison.Ordinal),
            "V1 接口不得声明 V2 /shop/* 路由（ProductOrder 与 ShopOrder 是两代契约）");
    }

    /// <summary>契约守卫 PRODUCTV1-2：令牌绑定（Query access_token）、注册组、模块枚举与建造器入口。</summary>
    [Fact]
    public void ProductV1Interfaces_ShouldBindMpAccessToken_AndRegisterModules()
    {
        var interfaces = Routes.Select(static r => r.Interface).Distinct().ToArray();
        interfaces.Should().HaveCount(18, "V1 五域共 18 支声明式接口");

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

        foreach (var (domain, _, _, interfaceCount) in Domains)
        {
            interfaces.Count(i => Routes.First(r => r.Interface == i).Domain == domain)
                .Should().Be(interfaceCount, $"{domain} 接口数漂移");

            Enum.TryParse<MiniProgramModule>(domain, out _).Should().BeTrue(
                $"MiniProgramModule 必须含 {domain} 枚举值（缺枚举 ⇒ 模块永远无法注册）");
            typeof(MiniProgramServiceBuilder).GetMethod("Add" + domain + "Api", Type.EmptyTypes)
                .Should().NotBeNull($"MiniProgramServiceBuilder 必须提供 Add{domain}Api() 入口");
        }
    }

    /// <summary>契约守卫 PRODUCTV1-3：DTO 总数、SerializerClassName 与 JsonContext 全量登记（AOT 生死线）。</summary>
    [Fact]
    public void ProductV1DataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaResponse).Assembly;

        foreach (var (domain, _, expectedCount, _) in Domains)
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

            // 被同域其它 DTO 以属性引用的 *Response 是嵌套业务对象（如券的 ExtInfo/PromoteInfo），
            // 不是端点应答外壳 ⇒ 不参与继承断言；其余 *Response 必须是端点信封。
            var nestedBusinessObjects = dtoTypes
                .Where(t => dtoTypes.Any(other => other.GetProperties().Any(p => p.PropertyType == t)))
                .ToHashSet();

            assembly.GetTypes()
                .Where(t => t.Namespace == ns && t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
                .Where(t => !nestedBusinessObjects.Contains(t))
                .Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
                    $"{domain} 域端点应答 DTO 必须继承 WxaResponse（errcode 判错与全线同源；嵌套业务对象不计）");
        }
    }

    /// <summary>扫描指定目录代码行中的 /product/ 路由字面量（剥离注释与构建产物）。</summary>
    private static IEnumerable<string> RouteLiteralsUnder(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
        var regex = new Regex("\"(/product/[A-Za-z0-9_\\-./]+)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);

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
