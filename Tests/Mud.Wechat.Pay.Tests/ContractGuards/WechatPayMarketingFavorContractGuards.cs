// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MarketingFavor;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P2 代金券域契约守卫（<b>P2 表内最后一项</b>，首批 2 端点：创建批次 + 查询券详情）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（2026-10-09 逐页核验，均为普通商户文档中心）</b>：创建代金券批次
/// <c>…/doc/v3/merchant/4012534633</c>（<b>POST</b> <c>/v3/marketing/favor/coupon-stocks</c>，更新 2024.10.31）、
/// 查询代金券详情 <c>…/doc/v3/merchant/4012486942</c>
/// （<b>GET</b> <c>/v3/marketing/favor/users/{openid}/coupons/{coupon_id}?appid=…</c>，更新 2024.09.19）。
/// 产品归属「营销产品 &gt; 代金券」，官方<b>未</b>声明下线。
/// </para>
/// <para>
/// <b>核验中值得留档的官方原文</b>：① 创建批次后须<b>再激活</b>才可发放（<c>…/stocks/{stock_id}/start</c>）
/// ⇒ <b>创建成功 ≠ 可发放</b>；② <c>coupon_use_rule.fixed_normal_coupon</c> 在 <c>stock_type = NORMAL</c>
/// 时<b>必填</b>（跨字段条件）；③ 查询接口「支持批次创建商户号与批次发放商户调用」且<b>支持幂等重入</b>。
/// </para>
/// <para>
/// <b>⚠️ 值域未核验</b>：<c>stock_type</c> / <c>status</c> / <c>coupon_type</c> / <c>business_type</c>
/// 的取值表本轮未取得 ⇒ 守卫<b>不</b>断言任何常量。
/// </para>
/// </remarks>
public class WechatPayMarketingFavorContractGuards
{
    private const string RegistryGroupName = "MarketingFavor";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingFavor";

    /// <summary>MF1：2 端点路由与方法照官方原文；查询的两个 path 参数与 appid query 位置不可混。</summary>
    [Fact]
    public void MarketingFavorEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(
            nameof(IWechatPayMarketingFavorService.CreateCouponStockAsync),
            "/v3/marketing/favor/coupon-stocks");
        AssertRoute<GetAttribute>(
            nameof(IWechatPayMarketingFavorService.QueryCouponAsync),
            "/v3/marketing/favor/users/{openId}/coupons/{couponId}");

        var query = FindMethod(nameof(IWechatPayMarketingFavorService.QueryCouponAsync));
        query.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "openId", "couponId" },
                "官方两个 path 参数 openid 与 coupon_id 均必填；顺序为 openid 在前");
        query.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "appid" }, "官方 query 只有 appid，且为必填");
    }

    /// <summary>MF2：官方字段名锁定（含创建批次的三层嵌套与查询应答的四类券型信息）。</summary>
    [Fact]
    public void MarketingFavorDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<CouponStockCreateRequest>(nameof(CouponStockCreateRequest.StockName), "stock_name");
        JsonNameShouldBe<CouponStockCreateRequest>(nameof(CouponStockCreateRequest.BelongMerchant), "belong_merchant");
        JsonNameShouldBe<CouponStockCreateRequest>(nameof(CouponStockCreateRequest.StockUseRule), "stock_use_rule");
        JsonNameShouldBe<CouponStockCreateRequest>(nameof(CouponStockCreateRequest.CouponUseRule), "coupon_use_rule");
        JsonNameShouldBe<CouponStockCreateRequest>(nameof(CouponStockCreateRequest.OutRequestNo), "out_request_no");
        JsonNameShouldBe<CouponStockCreateRequest>(nameof(CouponStockCreateRequest.NoCash), "no_cash");

        JsonNameShouldBe<CouponStockUseRule>(nameof(CouponStockUseRule.MaxCouponsPerUser), "max_coupons_per_user");
        JsonNameShouldBe<CouponStockUseRule>(nameof(CouponStockUseRule.PreventApiAbuse), "prevent_api_abuse");
        JsonNameShouldBe<CouponPatternInfo>(nameof(CouponPatternInfo.MerchantLogo), "merchant_logo");
        JsonNameShouldBe<CouponUseRule>(nameof(CouponUseRule.FixedNormalCoupon), "fixed_normal_coupon");
        JsonNameShouldBe<CouponUseRule>(nameof(CouponUseRule.AvailableMerchants), "available_merchants");
        JsonNameShouldBe<CouponAvailableTime>(nameof(CouponAvailableTime.FixAvailableTime), "fix_available_time");
        JsonNameShouldBe<CouponFixAvailableTime>(nameof(CouponFixAvailableTime.AvailableWeekDay), "available_week_day");
        JsonNameShouldBe<CouponFixedNormalCoupon>(nameof(CouponFixedNormalCoupon.TransactionMinimum), "transaction_minimum");
        JsonNameShouldBe<CouponLimitCard>(nameof(CouponLimitCard.Bin), "bin");

        JsonNameShouldBe<CouponStockCreateResponse>(nameof(CouponStockCreateResponse.StockId), "stock_id");
        JsonNameShouldBe<CouponQueryResponse>(nameof(CouponQueryResponse.StockCreatorMchId), "stock_creator_mchid");
        JsonNameShouldBe<CouponQueryResponse>(nameof(CouponQueryResponse.CutToMessage), "cut_to_message");
        JsonNameShouldBe<CouponQueryResponse>(nameof(CouponQueryResponse.NormalCouponInformation), "normal_coupon_information");
        JsonNameShouldBe<CouponQueryResponse>(nameof(CouponQueryResponse.DiscountMessage), "discount_msg");
        JsonNameShouldBe<CouponCutToMessage>(nameof(CouponCutToMessage.CutToPrice), "cut_to_price");
        JsonNameShouldBe<CouponDiscountMessage>(nameof(CouponDiscountMessage.DiscountPercent), "discount_percent");
    }

    /// <summary>MF3：判错面 + AOT 上下文登记（分组名一致）。</summary>
    [Fact]
    public void MarketingFavorDataModels_ShouldBeRegisteredInJsonContext()
    {
        foreach (var responseType in new[] { typeof(CouponStockCreateResponse), typeof(CouponQueryResponse) })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");
        }

        var domainTypes = typeof(CouponStockCreateRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(13,
            "代金券域 DTO：创建族 9（请求/发放规则/详情页/核销规则/生效时间/固定时段/满减券/卡BIN/应答）" +
            " + 查询族 4（应答/立减信息/普通券信息/折扣信息）");

        foreach (var type in domainTypes)
        {
            MarketingFavorJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingFavorJsonContext（Native AOT 无元数据会静默失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMarketingFavorService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayMarketingFavorService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }
}
