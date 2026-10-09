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
/// <para>
/// <b>【本批已补齐上面那条「未核验」】</b>（2026-10-09 逐字段核验「查询批次详情」页
/// <c>…/docs/merchant/apis/cash-coupons/stock/query-stock.html</c>，更新时间 2025.03.25，
/// 支持商户<b>【普通商户】</b>）：该页给出了 <c>status</c>（<b>全小写</b> 5 值，
/// 其中官方拼写为 <c>stoped</c> —— <b>少一个 p，属官方拼写，照录</b>）、
/// <c>coupon_type</c>（2 值）、<c>stock_type</c>（3 值）、
/// <c>trade_type</c>（6 值）、<c>business_type</c> 与地域级别（4 值）的完整取值表
/// ⇒ 已建 <c>MarketingFavorConstants.cs</c> 并逐值断言（MF4）。
/// </para>
/// <para>
/// <b>本批新增的五个端点</b>（同批核验，更新时间 2024.09.19）：
/// 激活 <c>…/stock/start-stock.html</c>、暂停 <c>…/stock/pause-stock.html</c>、
/// 重启 <c>…/stock/restart-stock.html</c>、查询批次详情（上页）、
/// 发放指定批次代金券 <c>…/doc/v3/merchant/4012463767</c>。
/// <b>三者均「支持幂等重入」</b>；发放接口的幂等键是请求体的 <c>out_request_no</c>。
/// </para>
/// <para>
/// <b>⚠️ 路径前缀族不一致（官方原文）</b>：创建批次是
/// <c>POST /v3/marketing/favor/<b>coupon-stocks</b></c>，而批次管理四动作都在
/// <c>/v3/marketing/favor/<b>stocks</b>/…</c> 下 —— <b>不得</b>为「统一」而改动任一侧。
/// </para>
/// </remarks>
public class WechatPayMarketingFavorContractGuards
{
    private const string RegistryGroupName = "MarketingFavor";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingFavor";

    /// <summary>MF1：7 端点路由与方法照官方原文；查询的两个 path 参数与 appid query 位置不可混。</summary>
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

        // 批次生命周期三动作：动作式路径（/start /pause /restart），且批次号走 path。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayMarketingFavorService.StartCouponStockAsync),
            "/v3/marketing/favor/stocks/{stockId}/start");
        AssertRoute<PostAttribute>(
            nameof(IWechatPayMarketingFavorService.PauseCouponStockAsync),
            "/v3/marketing/favor/stocks/{stockId}/pause");
        AssertRoute<PostAttribute>(
            nameof(IWechatPayMarketingFavorService.RestartCouponStockAsync),
            "/v3/marketing/favor/stocks/{stockId}/restart");

        foreach (var name in new[]
                 {
                     nameof(IWechatPayMarketingFavorService.StartCouponStockAsync),
                     nameof(IWechatPayMarketingFavorService.PauseCouponStockAsync),
                     nameof(IWechatPayMarketingFavorService.RestartCouponStockAsync),
                 })
        {
            FindMethod(name)
                .GetParameters()
                .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
                .Select(static p => p.Name)
                .Should().BeEquivalentTo(new[] { "stockId" }, "批次号 stock_id 走 path（占位符名 = C# 参数名）");
        }

        // 查询批次详情：GET，且**path 与 query 两个参数均必填**（stock_creator_mchid 是易漏的一项）。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayMarketingFavorService.QueryCouponStockAsync),
            "/v3/marketing/favor/stocks/{stockId}");
        var queryStock = FindMethod(nameof(IWechatPayMarketingFavorService.QueryCouponStockAsync));
        queryStock.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "stockId" });
        queryStock.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "stock_creator_mchid" },
                "官方该页 query 参数只有 stock_creator_mchid，且为**必填**（不是可选）");

        // 发放：openid 走 path、**无 query**（appid 在请求体里，不在 query）。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayMarketingFavorService.IssueCouponAsync),
            "/v3/marketing/favor/users/{openId}/coupons");
        var issue = FindMethod(nameof(IWechatPayMarketingFavorService.IssueCouponAsync));
        issue.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "openId" });
        issue.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("发放接口的 appid 在**请求体**内（官方字段表），不在 query");
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

        JsonNameShouldBe<CouponStockOperationRequest>(nameof(CouponStockOperationRequest.StockCreatorMchId), "stock_creator_mchid");
        JsonNameShouldBe<CouponStockStartResponse>(nameof(CouponStockStartResponse.StartTime), "start_time");
        JsonNameShouldBe<CouponStockPauseResponse>(nameof(CouponStockPauseResponse.PauseTime), "pause_time");
        JsonNameShouldBe<CouponStockRestartResponse>(nameof(CouponStockRestartResponse.RestartTime), "restart_time");
        JsonNameShouldBe<CouponStockQueryResponse>(nameof(CouponStockQueryResponse.AvailableBeginTime), "available_begin_time");
        JsonNameShouldBe<CouponStockQueryResponse>(nameof(CouponStockQueryResponse.AvailableRegionList), "available_region_list");
        JsonNameShouldBe<CouponStockQueryResponse>(nameof(CouponStockQueryResponse.AvailableIndustryList), "available_industry_list");
        JsonNameShouldBe<CouponStockQueryUseRule>(nameof(CouponStockQueryUseRule.MaxAmountByDay), "max_amount_by_day");
        JsonNameShouldBe<CouponStockQueryUseRule>(nameof(CouponStockQueryUseRule.FixedDiscountCoupon), "fixed_discount_coupon");
        JsonNameShouldBe<CouponAvailableRegion>(nameof(CouponAvailableRegion.District), "district");
        JsonNameShouldBe<CouponIssueRequest>(nameof(CouponIssueRequest.OutRequestNo), "out_request_no");
        JsonNameShouldBe<CouponIssueRequest>(nameof(CouponIssueRequest.StockCreatorMchId), "stock_creator_mchid");
        JsonNameShouldBe<CouponIssueResponse>(nameof(CouponIssueResponse.CouponId), "coupon_id");
    }

    /// <summary>
    /// MF4：<b>三张「使用规则」字段表必须分开</b> + 复用关系 + 取值常量逐值锁定。
    /// </summary>
    /// <remarks>
    /// 本域同一份业务里有三处形似而不同的规则表（创建的 <c>stock_use_rule</c>、创建的
    /// <c>coupon_use_rule</c>、查询应答的 <c>stock_use_rule</c>），合并任一方向都会造出
    /// 「某接口永不返回的字段」；另有 <c>available_begin_time</c> 属<b>顶层</b>而非规则内的坑。
    /// </remarks>
    [Fact]
    public void CouponUseRuleShapes_ShouldStayStructurallySeparate()
    {
        // 三张表互不相同：各自有对方没有的字段。
        typeof(CouponStockUseRule).GetProperty("PreventApiAbuse").Should().NotBeNull();
        typeof(CouponStockUseRule).GetProperty("CouponType").Should().BeNull(
            "创建侧的 stock_use_rule 没有 coupon_type（它在 coupon_use_rule 与查询应答里）");

        typeof(CouponUseRule).GetProperty("AvailableMerchants").Should().NotBeNull();
        typeof(CouponStockQueryUseRule).GetProperty("AvailableMerchants").Should().BeNull(
            "查询应答的 stock_use_rule 没有可用商户（那是创建侧 coupon_use_rule 的字段）");

        typeof(CouponStockQueryUseRule).GetProperty("CouponType").Should().NotBeNull();
        typeof(CouponStockQueryUseRule).GetProperty("FixedDiscountCoupon").Should().NotBeNull();

        // available_begin_time / available_end_time 在**应答顶层**，不在规则对象里。
        typeof(CouponStockQueryResponse).GetProperty("AvailableBeginTime").Should().NotBeNull();
        typeof(CouponStockQueryUseRule).GetProperty("AvailableBeginTime").Should().BeNull(
            "官方该页把可用时间放在顶层，规则对象下**没有** available_time / available_begin_time");

        // 复用关系（表相同则共用，禁另建同形类）。
        typeof(CouponStockQueryResponse).GetProperty("CutToMessage")!.PropertyType
            .Should().Be<CouponCutToMessage>();
        typeof(CouponStockQueryUseRule).GetProperty("FixedNormalCoupon")!.PropertyType
            .Should().Be<CouponFixedNormalCoupon>();
        typeof(CouponStockQueryUseRule).GetProperty("FixedDiscountCoupon")!.PropertyType
            .Should().Be<CouponDiscountMessage>("与券详情的 discount_msg 三字段一致 ⇒ 复用");

        // 三个生命周期应答**必须分建**（时间字段名各不相同，合并会给出永不返回的字段）。
        typeof(CouponStockStartResponse).GetProperty("PauseTime").Should().BeNull();
        typeof(CouponStockPauseResponse).GetProperty("StartTime").Should().BeNull();
        typeof(CouponStockRestartResponse).GetProperty("StartTime").Should().BeNull();

        // 取值常量逐值锁定（官方原文，注意 status 全小写且 stoped 官方拼写少一个 p）。
        CouponStockStatuses.Unactivated.Should().Be("unactivated");
        CouponStockStatuses.Audit.Should().Be("audit");
        CouponStockStatuses.Running.Should().Be("running");
        CouponStockStatuses.Stoped.Should().Be("stoped");
        CouponStockStatuses.Paused.Should().Be("paused");
        CouponStockStatuses.Stoped.Should().NotBe("stopped",
            "官方页面逐字为 stoped（少一个 p）—— 擅自「纠正」拼写会导致状态永远匹配不上");

        CouponTypes.Normal.Should().Be("NORMAL");
        CouponTypes.CutTo.Should().Be("CUT_TO");
        CouponStockTypes.DiscountCut.Should().Be("DISCOUNT_CUT");
        CouponTradeTypes.Ppay.Should().Be("PPAY");
        CouponTradeTypes.MicroApp.Should().Be("MICROAPP");
        CouponRegionTypes.Country.Should().Be("COUNTRY");
        CouponRegionTypes.District.Should().Be("DISTRICT");
        CouponBusinessTypes.MultiUse.Should().Be("MULTIUSE");

        // 枚举族口径：status 全小写，而其余取值全大写 —— 两种风格混在同一产品线里，勿类推。
        CouponStockStatuses.Running.Should().NotBe(CouponStockStatuses.Running.ToUpperInvariant());
    }

    /// <summary>MF3：判错面 + AOT 上下文登记（分组名一致）。</summary>
    [Fact]
    public void MarketingFavorDataModels_ShouldBeRegisteredInJsonContext()
    {
        foreach (var responseType in new[]
                 {
                     typeof(CouponStockCreateResponse), typeof(CouponQueryResponse),
                     typeof(CouponStockStartResponse), typeof(CouponStockPauseResponse),
                     typeof(CouponStockRestartResponse), typeof(CouponStockQueryResponse),
                     typeof(CouponIssueResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");
        }

        var domainTypes = typeof(CouponStockCreateRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(22,
            "代金券域 DTO：创建族 9（请求/发放规则/详情页/核销规则/生效时间/固定时段/满减券/卡BIN/应答）" +
            " + 查询族 4（应答/立减信息/普通券信息/折扣信息）" +
            " + 批次管理族 7（操作请求/激活应答/暂停应答/重启应答/批次应答/批次规则/地域）" +
            " + 发放族 2（请求/应答）");

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
