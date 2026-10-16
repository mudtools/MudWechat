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
using Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-3 营销批次「品牌商品券」域契约守卫 —— <b>33 个声明式方法 / 32 条 (方法, 路由)</b> + <b>1 条 multipart 手工通道</b>（不计入端点数）。
/// </summary>
/// <remarks>
/// <para><b>本域是本批次最大面且全部【普通服务商】</b>（签名主体是品牌服务商，不是平台商户）⇒ 与 <c>MarketingBusifavor</c> 的商户面不可互换。</para>
/// <para><b>同路由两契约</b>：<c>POST /v3/marketing/partner/product-coupon/product-coupons</c> 官方拆成「单品形态」（<c>product_coupon_type = SINGLE</c>）与「批次组合形态」两页，字段表<b>不同形</b> ⇒ 一面两方法（守卫两个数都锁）。</para>
/// <para><b>改支统一是 PATCH</b>（商品券 / 批次 / 批次组合 / 预算四支），<b>作废与发券是 POST 动词段</b>。</para>
/// <para><b>图片上传不走声明式接口</b>：官方 <c>Content-Type</c> 是 <c>multipart/form-data</c>（<c>partner/4015781275</c>、<c>partner/4016435731</c>）⇒ 归手工通道 <c>IWechatPayMarketingMediaService</c>，落进生成式接口即造出无 Content-Type 的假端点（MPPC-B3 带正对照）。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayMarketingPartnerProductCouponContractGuards
{
    private const string RegistryGroupName = "MarketingPartnerProductCoupon";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon";

    private static readonly Type Service = typeof(IWechatPayMarketingPartnerProductCouponService);

    /// <summary>MPPC-B1：拓扑 —— 一个注册组一个接口、33 个方法、32 条 (方法, 路由)、动词集 POST / GET / PATCH；路由前缀 /v3/marketing/partner/product-coupon/。</summary>
    [Fact]
    public void ProductCouponTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(33, "商品券（服务商）声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(32, "同路由两契约 ⇒ 方法数与 (方法, 路由) 数不等");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute", "PatchAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/marketing/partner/product-coupon/", StringComparison.Ordinal),
            "本域路由全在 /v3/marketing/partner/product-coupon/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>MPPC-B2：33 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void ProductCouponEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponImageTaskAsync)).Should().Be("/v3/marketing/partner/product-coupon/image-generation-tasks");
        // 创建商品券图片生成任务｜官方页 partner/4017327735 + partner/4017327752｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponImageTaskAsync)).Should().Be("/v3/marketing/partner/product-coupon/image-generation-tasks/{taskId}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponImageTaskAsync)).Should().BeEquivalentTo(new[] { "taskId" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponImageTaskAsync)).Should().BeEquivalentTo(new[] { "brand_id", "image_generation_type" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponImageTaskAsync)).Should().BeEquivalentTo(new[] { "brand_id", "image_generation_type" }, "查询商品券图片生成任务的 brand_id / image_generation_type 官方必填：给默认值即把 400 变成静默错参");
        // 查询商品券图片生成任务｜官方页 partner/4017327739 + partner/4017327753｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.SetProductCouponNotifyConfigAsync)).Should().Be("/v3/marketing/partner/product-coupon/notify-configs");
        // 设置商品券通知回调｜官方页 partner/4015781286 + partner/4016435719｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponNotifyConfigAsync)).Should().Be("/v3/marketing/partner/product-coupon/notify-configs");
        // 查询商品券通知回调｜官方页 partner/4015781284 + partner/4016435718｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons");
        // 创建商品券（单品批次形态）｜官方页 partner/4015781289 + partner/4016434630｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponBundleAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons");
        // 创建商品券（批次组合形态）｜官方页 partner/4015781289 + partner/4016434630｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAsync)).Should().BeEquivalentTo(new[] { "productCouponId" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAsync)).Should().BeEquivalentTo(new[] { "brand_id" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "查询商品券详情的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 查询商品券详情｜官方页 partner/4016434632｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponAsync)).Should().BeEquivalentTo(new[] { "productCouponId" });
        // 修改商品券｜官方页 partner/4015781296 + partner/4016434633｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/deactivate");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponAsync)).Should().BeEquivalentTo(new[] { "productCouponId" });
        // 作废商品券｜官方页 partner/4015781290 + partner/4016434631｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponStockBundleAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponStockBundleAsync)).Should().BeEquivalentTo(new[] { "productCouponId" });
        // 创建商品券批次组合｜官方页 partner/4016280622｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockBundleAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockBundleAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockBundleId" });
        // 修改商品券批次组合｜官方页 partner/4016280633｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.AssociateProductCouponStockBundleStoresAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/associate-stores");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.AssociateProductCouponStockBundleStoresAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockBundleId" });
        // 批次组合关联门店｜官方页 partner/4016280620｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.DisassociateProductCouponStockBundleStoresAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/disassociate-stores");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.DisassociateProductCouponStockBundleStoresAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockBundleId" });
        // 批次组合解除门店关联｜官方页 partner/4016280629｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockBundleBudgetAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stock-bundles/{stockBundleId}/update-budget");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockBundleBudgetAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockBundleId" });
        // 修改批次组合预算｜官方页 partner/4016280642｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStocksAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStocksAsync)).Should().BeEquivalentTo(new[] { "productCouponId" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStocksAsync)).Should().BeEquivalentTo(new[] { "state", "page_size", "page_token", "brand_id", "stock_bundle_id" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStocksAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "查询商品券批次列表的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 查询商品券批次列表｜官方页 partner/4015781553 + partner/4016434641｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponStockAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.CreateProductCouponStockAsync)).Should().BeEquivalentTo(new[] { "productCouponId" });
        // 创建商品券批次｜官方页 partner/4015781304｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStockAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStockAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStockAsync)).Should().BeEquivalentTo(new[] { "brand_id" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponStockAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "查询商品券批次详情的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 查询商品券批次详情｜官方页 partner/4015781542 + partner/4016434649｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        // 修改商品券批次｜官方页 partner/4015781556｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponStockAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/deactivate");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponStockAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        // 作废商品券批次｜官方页 partner/4015781532｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockBudgetAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/update-budget");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UpdateProductCouponStockBudgetAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        // 修改商品券批次预算｜官方页 partner/4015781561｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.AssociateProductCouponStockStoresAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associate-stores");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.AssociateProductCouponStockStoresAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        // 批次关联门店｜官方页 partner/4015781302｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.DisassociateProductCouponStockStoresAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/disassociate-stores");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.DisassociateProductCouponStockStoresAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        // 批次解除门店关联｜官方页 partner/4015781537｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAssociatedStoresAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/associated-stores");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAssociatedStoresAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAssociatedStoresAsync)).Should().BeEquivalentTo(new[] { "page_size", "page_token", "brand_id" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponAssociatedStoresAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "查询批次已关联门店的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 查询批次已关联门店｜官方页 partner/4015781546 + partner/4016434665｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UploadProductCouponStockCodesAsync)).Should().Be("/v3/marketing/partner/product-coupon/product-coupons/{productCouponId}/stocks/{stockId}/upload-coupon-codes");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UploadProductCouponStockCodesAsync)).Should().BeEquivalentTo(new[] { "productCouponId", "stockId" });
        // 预上传商品券券码｜官方页 partner/4015781572 + partner/4016434668｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.SendProductCouponUserBundleAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.SendProductCouponUserBundleAsync)).Should().BeEquivalentTo(new[] { "openid" });
        // 向用户发放券包｜官方页 partner/4016280664｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponUserBundleAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupon-bundles/{userCouponBundleId}/deactivate");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponUserBundleAsync)).Should().BeEquivalentTo(new[] { "openid", "userCouponBundleId" });
        // 作废用户券包｜官方页 partner/4016280658｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.SendProductCouponUserCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.SendProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid" });
        // 向用户发放商品券｜官方页 partner/4015781605｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponsAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponsAsync)).Should().BeEquivalentTo(new[] { "openid" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponsAsync)).Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "coupon_state", "user_coupon_bundle_id", "page_size", "page_token", "brand_id" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponsAsync)).Should().BeEquivalentTo(new[] { "appid", "brand_id" }, "查询用户商品券列表的 appid / brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 查询用户商品券列表｜官方页 partner/4015781590 + partner/4016435703｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid", "couponCode" });
        QueryNamesOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "brand_id" });
        RequiredQueryOf(nameof(IWechatPayMarketingPartnerProductCouponService.QueryProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "brand_id" }, "查询用户商品券详情的 product_coupon_id / stock_id / appid / brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 查询用户商品券详情｜官方页 partner/4015781582 + partner/4016435702｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.ConfirmProductCouponUserCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/confirm");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.ConfirmProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid", "couponCode" });
        // 确认用户商品券｜官方页 partner/4015781575 + partner/4016435562｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.ReturnProductCouponUserCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/return");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.ReturnProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid", "couponCode" });
        // 退用户商品券｜官方页 partner/4015781599 + partner/4016435674｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.UseProductCouponUserCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/use");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.UseProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid", "couponCode" });
        // 核销用户商品券｜官方页 partner/4015781608 + partner/4016435636｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponUserCouponAsync)).Should().Be("/v3/marketing/partner/product-coupon/users/{openid}/coupons/{couponCode}/deactivate");
        PathKeysOf(nameof(IWechatPayMarketingPartnerProductCouponService.DeactivateProductCouponUserCouponAsync)).Should().BeEquivalentTo(new[] { "openid", "couponCode" });
        // 作废用户商品券｜官方页 partner/4015781578｜身份 服务商
    }

    /// <summary>MPPC-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void ProductCouponDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<ProductCouponImageTaskCreateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "task_id", "image_generation_type", "combine_image", "cut_out" },
            "创建商品券图片生成任务请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponImageTaskCreateResponse>().Should().BeEquivalentTo(new[] { "task_id", "brand_id" },
            "创建商品券图片生成任务应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponImageTaskQueryResponse>().Should().BeEquivalentTo(new[] { "brand_id", "task_id", "image_generation_type", "task_state", "combine_image_result", "cut_out_result" },
            "查询商品券图片生成任务应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponNotifyConfigSetRequest>().Should().BeEquivalentTo(new[] { "notify_url" },
            "设置商品券通知回调请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponNotifyConfigSetResponse>().Should().BeEquivalentTo(new[] { "notify_url", "update_time" },
            "设置商品券通知回调应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponNotifyConfigQueryResponse>().Should().BeEquivalentTo(new[] { "notify_url", "update_time" },
            "查询商品券通知回调应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponCouponCreateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "scope", "type", "usage_mode", "single_usage_info", "display_info", "out_product_no", "stock", "brand_id", "show_as_exchange", "available_product_no_list" },
            "创建商品券（单品批次形态）请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponCouponCreateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "scope", "type", "usage_mode", "single_usage_info", "display_info", "out_product_no", "state", "stock", "show_as_exchange", "available_product_no_list" },
            "创建商品券（单品批次形态）应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponBundleCouponCreateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "scope", "type", "usage_mode", "single_usage_info", "display_info", "out_product_no", "stock", "brand_id", "show_as_exchange", "available_product_no_list" },
            "创建商品券（批次组合形态）请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponBundleCouponCreateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "scope", "type", "usage_mode", "single_usage_info", "display_info", "out_product_no", "state", "stock", "show_as_exchange", "available_product_no_list" },
            "创建商品券（批次组合形态）应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponCouponQueryResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "scope", "type", "usage_mode", "single_usage_info", "progressive_bundle_usage_info", "display_info", "out_product_no", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id", "show_as_exchange", "available_product_no_list" },
            "查询商品券详情应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponCouponUpdateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "display_info", "brand_id", "available_product_no_list" },
            "修改商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponCouponUpdateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "scope", "type", "usage_mode", "single_usage_info", "progressive_bundle_usage_info", "display_info", "out_product_no", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id", "show_as_exchange", "available_product_no_list" },
            "修改商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponCouponDeactivateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "deactivate_reason", "brand_id" },
            "作废商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponCouponDeactivateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "scope", "type", "usage_mode", "single_usage_info", "progressive_bundle_usage_info", "display_info", "out_product_no", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id", "show_as_exchange", "available_product_no_list" },
            "作废商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockBundleCreateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "stock_bundle", "brand_id" },
            "创建商品券批次组合请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockBundleCreateResponse>().Should().BeEquivalentTo(new[] { "stock_bundle_id", "stock_list" },
            "创建商品券批次组合应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockBundleUpdateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "remark", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "brand_id" },
            "修改商品券批次组合请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockBundleUpdateResponse>().Should().BeEquivalentTo(new[] { "stock_bundle_id", "stock_list" },
            "修改商品券批次组合应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockBundleAssociateStoresRequest>().Should().BeEquivalentTo(new[] { "store_list", "brand_id" },
            "批次组合关联门店请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockBundleAssociateStoresResponse>().Should().BeEquivalentTo(new[] { "total_count", "success_store_list", "failed_store_list" },
            "批次组合关联门店应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockBundleDisassociateStoresRequest>().Should().BeEquivalentTo(new[] { "store_list", "brand_id" },
            "批次组合解除门店关联请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockBundleDisassociateStoresResponse>().Should().BeEquivalentTo(new[] { "total_count", "success_store_list", "failed_store_list" },
            "批次组合解除门店关联应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockBundleBudgetRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "update_mode", "current_max_count", "target_max_count", "current_max_count_per_day", "target_max_count_per_day", "brand_id" },
            "修改批次组合预算请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockBundleBudgetResponse>().Should().BeEquivalentTo(new[] { "stock_bundle_id", "stock_list" },
            "修改批次组合预算应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockListResponse>().Should().BeEquivalentTo(new[] { "total_count", "stock_list", "next_page_token" },
            "查询商品券批次列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockCreateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "stock", "brand_id" },
            "创建商品券批次请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockCreateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "remark", "coupon_code_mode", "coupon_code_count_info", "stock_send_rule", "single_usage_rule", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "sent_count_info", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id" },
            "创建商品券批次应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockQueryResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "remark", "coupon_code_mode", "coupon_code_count_info", "stock_send_rule", "single_usage_rule", "progressive_bundle_usage_rule", "stock_bundle_info", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "sent_count_info", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id" },
            "查询商品券批次详情应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockUpdateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "remark", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "brand_id" },
            "修改商品券批次请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockUpdateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "remark", "coupon_code_mode", "coupon_code_count_info", "stock_send_rule", "single_usage_rule", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "sent_count_info", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id" },
            "修改商品券批次应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockDeactivateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "deactivate_reason", "brand_id" },
            "作废商品券批次请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockDeactivateResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "remark", "coupon_code_mode", "coupon_code_count_info", "stock_send_rule", "single_usage_rule", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "sent_count_info", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id" },
            "作废商品券批次应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockBudgetRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "update_mode", "current_max_count", "target_max_count", "current_max_count_per_day", "target_max_count_per_day", "brand_id" },
            "修改商品券批次预算请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockBudgetResponse>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "remark", "coupon_code_mode", "coupon_code_count_info", "stock_send_rule", "single_usage_rule", "usage_rule_display_info", "coupon_display_info", "notify_config", "store_scope", "sent_count_info", "state", "deactivate_request_no", "deactivate_time", "deactivate_reason", "brand_id" },
            "修改商品券批次预算应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockAssociateStoresRequest>().Should().BeEquivalentTo(new[] { "store_list", "brand_id" },
            "批次关联门店请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockAssociateStoresResponse>().Should().BeEquivalentTo(new[] { "total_count", "success_store_list", "failed_store_list" },
            "批次关联门店应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockDisassociateStoresRequest>().Should().BeEquivalentTo(new[] { "store_list", "brand_id" },
            "批次解除门店关联请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockDisassociateStoresResponse>().Should().BeEquivalentTo(new[] { "total_count", "success_store_list", "failed_store_list" },
            "批次解除门店关联应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockAssociatedStoresResponse>().Should().BeEquivalentTo(new[] { "total_count", "store_list", "next_page_token" },
            "查询批次已关联门店应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponStockUploadCodesRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "code_list", "brand_id" },
            "预上传商品券券码请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponStockUploadCodesResponse>().Should().BeEquivalentTo(new[] { "total_count", "success_code_list", "failed_code_list", "already_exist_code_list", "duplicate_code_list" },
            "预上传商品券券码应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserBundleSendRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_bundle_id", "appid", "send_request_no", "attach", "brand_id", "coupon_tag_info" },
            "向用户发放券包请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserBundleSendResponse>().Should().BeEquivalentTo(new[] { "user_coupon_bundle_id", "user_product_coupon_list" },
            "向用户发放券包应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserBundleDeactivateRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_bundle_id", "appid", "out_request_no", "deactivate_reason", "brand_id" },
            "作废用户券包请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserBundleDeactivateResponse>().Should().BeEquivalentTo(new[] { "user_coupon_bundle_id", "user_product_coupon_list" },
            "作废用户券包应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponSendRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "coupon_code", "appid", "send_request_no", "attach", "brand_id", "coupon_tag_info" },
            "向用户发放商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserCouponSendResponse>().Should().BeEquivalentTo(new[] { "coupon_code", "coupon_state", "valid_begin_time", "valid_end_time", "receive_time", "send_request_no", "send_channel", "confirm_request_no", "confirm_time", "deactivate_request_no", "deactivate_time", "deactivate_reason", "single_usage_detail", "product_coupon", "stock", "attach", "channel_custom_info", "coupon_tag_info" },
            "向用户发放商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponListResponse>().Should().BeEquivalentTo(new[] { "total_count", "user_coupon_list", "next_page_token" },
            "查询用户商品券列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponQueryResponse>().Should().BeEquivalentTo(new[] { "coupon_code", "coupon_state", "valid_begin_time", "valid_end_time", "receive_time", "send_request_no", "send_channel", "confirm_request_no", "confirm_time", "deactivate_request_no", "deactivate_time", "deactivate_reason", "single_usage_detail", "progressive_bundle_usage_detail", "user_product_coupon_bundle_info", "product_coupon", "stock", "attach", "channel_custom_info", "coupon_tag_info" },
            "查询用户商品券详情应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponConfirmRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "out_request_no", "brand_id" },
            "确认用户商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserCouponConfirmResponse>().Should().BeEquivalentTo(new[] { "coupon_code", "coupon_state", "valid_begin_time", "valid_end_time", "receive_time", "send_request_no", "send_channel", "confirm_request_no", "confirm_time", "deactivate_request_no", "deactivate_time", "deactivate_reason", "single_usage_detail", "progressive_bundle_usage_detail", "user_product_coupon_bundle_info", "product_coupon", "stock", "attach", "channel_custom_info", "coupon_tag_info" },
            "确认用户商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponReturnRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "out_request_no", "brand_id" },
            "退用户商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserCouponReturnResponse>().Should().BeEquivalentTo(new[] { "coupon_code", "coupon_state", "valid_begin_time", "valid_end_time", "receive_time", "send_request_no", "send_channel", "confirm_request_no", "confirm_time", "deactivate_request_no", "deactivate_time", "deactivate_reason", "single_usage_detail", "progressive_bundle_usage_detail", "user_product_coupon_bundle_info", "product_coupon", "stock", "attach", "channel_custom_info", "coupon_tag_info" },
            "退用户商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponUseRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "use_time", "out_request_no", "brand_id", "store_id", "associated_order_info", "associated_pay_score_order_info", "saved_amount", "use_product_no_list" },
            "核销用户商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserCouponUseResponse>().Should().BeEquivalentTo(new[] { "coupon_code", "coupon_state", "valid_begin_time", "valid_end_time", "receive_time", "send_request_no", "send_channel", "confirm_request_no", "confirm_time", "deactivate_request_no", "deactivate_time", "deactivate_reason", "single_usage_detail", "progressive_bundle_usage_detail", "user_product_coupon_bundle_info", "product_coupon", "stock", "attach", "channel_custom_info", "coupon_tag_info" },
            "核销用户商品券应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ProductCouponUserCouponDeactivateRequest>().Should().BeEquivalentTo(new[] { "product_coupon_id", "stock_id", "appid", "out_request_no", "deactivate_reason", "brand_id" },
            "作废用户商品券请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ProductCouponUserCouponDeactivateResponse>().Should().BeEquivalentTo(new[] { "coupon_code", "coupon_state", "valid_begin_time", "valid_end_time", "receive_time", "send_request_no", "send_channel", "confirm_request_no", "confirm_time", "deactivate_request_no", "deactivate_time", "deactivate_reason", "single_usage_detail", "product_coupon", "stock", "attach", "channel_custom_info", "coupon_tag_info" },
            "作废用户商品券应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>MPPC-B4：判错面 + AOT 上下文登记 + DI 与模块接线（25 请求型 / 33 应答型）。</summary>
    [Fact]
    public void ProductCouponDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(774, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            MarketingPartnerProductCouponJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingPartnerProductCouponJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MarketingPartnerProductCoupon");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(ProductCouponImageTaskCreateResponse), typeof(ProductCouponImageTaskQueryResponse), typeof(ProductCouponNotifyConfigSetResponse), typeof(ProductCouponNotifyConfigQueryResponse), typeof(ProductCouponCouponCreateResponse), typeof(ProductCouponBundleCouponCreateResponse), typeof(ProductCouponCouponQueryResponse), typeof(ProductCouponCouponUpdateResponse), typeof(ProductCouponCouponDeactivateResponse), typeof(ProductCouponStockBundleCreateResponse), typeof(ProductCouponStockBundleUpdateResponse), typeof(ProductCouponStockBundleAssociateStoresResponse), typeof(ProductCouponStockBundleDisassociateStoresResponse), typeof(ProductCouponStockBundleBudgetResponse), typeof(ProductCouponStockListResponse), typeof(ProductCouponStockCreateResponse), typeof(ProductCouponStockQueryResponse), typeof(ProductCouponStockUpdateResponse), typeof(ProductCouponStockDeactivateResponse), typeof(ProductCouponStockBudgetResponse), typeof(ProductCouponStockAssociateStoresResponse), typeof(ProductCouponStockDisassociateStoresResponse), typeof(ProductCouponStockAssociatedStoresResponse), typeof(ProductCouponStockUploadCodesResponse), typeof(ProductCouponUserBundleSendResponse), typeof(ProductCouponUserBundleDeactivateResponse), typeof(ProductCouponUserCouponSendResponse), typeof(ProductCouponUserCouponListResponse), typeof(ProductCouponUserCouponQueryResponse), typeof(ProductCouponUserCouponConfirmResponse), typeof(ProductCouponUserCouponReturnResponse), typeof(ProductCouponUserCouponUseResponse), typeof(ProductCouponUserCouponDeactivateResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(ProductCouponImageTaskCreateRequest), typeof(ProductCouponNotifyConfigSetRequest), typeof(ProductCouponCouponCreateRequest), typeof(ProductCouponBundleCouponCreateRequest), typeof(ProductCouponCouponUpdateRequest), typeof(ProductCouponCouponDeactivateRequest), typeof(ProductCouponStockBundleCreateRequest), typeof(ProductCouponStockBundleUpdateRequest), typeof(ProductCouponStockBundleAssociateStoresRequest), typeof(ProductCouponStockBundleDisassociateStoresRequest), typeof(ProductCouponStockBundleBudgetRequest), typeof(ProductCouponStockCreateRequest), typeof(ProductCouponStockUpdateRequest), typeof(ProductCouponStockDeactivateRequest), typeof(ProductCouponStockBudgetRequest), typeof(ProductCouponStockAssociateStoresRequest), typeof(ProductCouponStockDisassociateStoresRequest), typeof(ProductCouponStockUploadCodesRequest), typeof(ProductCouponUserBundleSendRequest), typeof(ProductCouponUserBundleDeactivateRequest), typeof(ProductCouponUserCouponSendRequest), typeof(ProductCouponUserCouponConfirmRequest), typeof(ProductCouponUserCouponReturnRequest), typeof(ProductCouponUserCouponUseRequest), typeof(ProductCouponUserCouponDeactivateRequest) })
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
        resolverSource.Should().Contain("MarketingPartnerProductCouponJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MarketingPartnerProductCoupon 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMarketingPartnerProductCouponWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 MarketingPartnerProductCoupon 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMarketingPartnerProductCouponService.{methodName} 必须存在");

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
