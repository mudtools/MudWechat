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
using Mud.Wechat.Pay.DataModels.PayScore;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P2 支付分域契约守卫（<b>首批 3 端点</b>：创建 / 查询 / 取消服务订单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（2026-10-09 逐页核验）</b>：创建支付分订单
/// <c>pay.weixin.qq.com/doc/v3/merchant/4012587900</c>（<b>POST</b> <c>/v3/payscore/serviceorder</c>）、
/// 查询支付分订单 <c>…/wiki/doc/apiv3/apis/chapter6_1_15.shtml</c>（<b>GET</b> 同路径 + 4 个 query）、
/// 取消支付分订单 <c>…/chapter6_1_16.shtml</c>（<b>POST</b> <c>/{out_order_no}/cancel</c>）。
/// 三页均标注支持商户：【普通商户】。
/// </para>
/// <para>
/// <b>核验中值得留档的官方原文</b>：
/// ① 创建接口<b>支持原参重入</b>（相同参数重复调用可返回成功）；
/// ② <c>out_order_no</c> <b>不可</b>用作申请退款的 <c>out_trade_no</c>；
/// ③ 「完结订单和取消订单需与创单传入的 <c>appid</c> 保持一致」；
/// ④ 查询接口的 <c>out_order_no</c> 与 <c>query_id</c> <b>必填其一，不允许都填或都不填</b>；
/// ⑤ 查询接口官方要求<b>参考「支付分订单状态流转图」</b>处理业务逻辑；
/// ⑥ 取消接口的 <c>service_id</c> 是<b>选填</b>（创单接口里它是必填）。
/// </para>
/// </remarks>
public class WechatPayPayScoreContractGuards
{
    private const string RegistryGroupName = "PayScore";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PayScore";

    /// <summary>PY-B1：3 端点的方法与路由照官方原文；查询的 4 个 query 参数与取消的 path 参数位置不可改动。</summary>
    [Fact]
    public void PayScoreEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.CreateServiceOrderAsync), "/v3/payscore/serviceorder");
        AssertRoute<GetAttribute>(nameof(IWechatPayPayScoreService.QueryServiceOrderAsync), "/v3/payscore/serviceorder");
        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.CancelServiceOrderAsync), "/v3/payscore/serviceorder/{outOrderNo}/cancel");
        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.CompleteServiceOrderAsync), "/v3/payscore/serviceorder/{outOrderNo}/complete");
        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.ModifyServiceOrderAsync), "/v3/payscore/serviceorder/{outOrderNo}/modify");

        // 官方接口名是「发起催收扣款」，路由为 /pay（不是 /collect 之类的直觉名）。
        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.CollectServiceOrderAsync), "/v3/payscore/serviceorder/{outOrderNo}/pay");

        // 授权面（免确认模式）：预授权 + 查授权记录（授权协议号走 path、service_id 走 query）。
        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.PreAuthorizeAsync), "/v3/payscore/permissions");
        AssertRoute<GetAttribute>(
            nameof(IWechatPayPayScoreService.QueryAuthorizationRecordAsync),
            "/v3/payscore/permissions/authorization-code/{authorizationCode}");

        AssertRoute<PostAttribute>(nameof(IWechatPayPayScoreService.SyncServiceOrderAsync), "/v3/payscore/serviceorder/{outOrderNo}/sync");

        // 解除授权是**动作式**路径（/terminate），不是对 permissions/{code} 发 DELETE。
        AssertRoute<PostAttribute>(
            nameof(IWechatPayPayScoreService.TerminateAuthorizationAsync),
            "/v3/payscore/permissions/authorization-code/{authorizationCode}/terminate");

        // 解除授权官方「无应答包体」（204）⇒ 方法必须声明为无返回值（Task），
        // 防后来者「顺手」给它加一个应答 DTO 而去解一个不存在的包体。
        FindMethod(nameof(IWechatPayPayScoreService.TerminateAuthorizationAsync))
            .ReturnType.Should().Be(typeof(Task), "官方 204 No Content ⇒ 无应答体，返回类型必须是 Task");
        FindMethod(nameof(IWechatPayPayScoreService.CancelServiceOrderAsync))
            .ReturnType.Should().NotBe(typeof(Task), "取消订单有应答体（5 字段），不得退化成无返回");

        // 授权面·按 OPENID 查询：两个 query 参数缺一不可。
        AssertRoute<GetAttribute>(
            nameof(IWechatPayPayScoreService.QueryAuthorizationRecordByOpenIdAsync),
            "/v3/payscore/permissions/openid/{openId}");
        FindMethod(nameof(IWechatPayPayScoreService.QueryAuthorizationRecordByOpenIdAsync))
            .GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "service_id", "appid" },
                "官方把 service_id 与 appid 都标为必填（与「授权协议号」版只需 service_id 不同）");

        // 两个「查授权记录」入口的应答表完全一致 ⇒ 必须共用同一 DTO（防两处字段各自漂移）。
        FindMethod(nameof(IWechatPayPayScoreService.QueryAuthorizationRecordByOpenIdAsync))
            .ReturnType.Should().Be(typeof(Task<PayScoreAuthorizationRecordResponse>),
                "官方两页应答字段表逐项一致 ⇒ 复用同一 DTO");
    }

    /// <summary>
    /// PY-B9：<b>创单结单合并 <c>direct-complete</c> 刻意不建模</b>（官方声明暂未对外开放）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方事实（2026-10-09 核验，页面 chapter6_1_1 / 更新时间 2024.11.06）</b>：
    /// </para>
    /// <list type="number">
    /// <item><b>「特别提醒：创单结单合并接口暂未对外开放</b>，如有需要请咨询对接的微信支付运营人员，
    /// <b>申请开通调用权限</b>」——即对绝大多数商户不可调用；</item>
    /// <item>限制条件：<b>免确认订单模式</b>，且用户需处于<b>已授权</b>状态；</item>
    /// <item><b>官方自相矛盾（照录）</b>：接口说明给的路径是 <c>/payscore/serviceorder/direct-complete</c>，
    /// 而<b>请求示例</b>用的是 <c>/v3/payscore/<b>partner</b>/serviceorder/direct-complete</c>。</item>
    /// </list>
    /// <para>
    /// <b>处置</b>：按本仓「官方声明不可用即不建模」的既定纪律（同 <c>/cv/img/superresolution</c> 已下架页）
    /// <b>不实现</b> —— 否则 SDK 会多出一个整体不可调用的公开面。待宿主实际取得开通权限、
    /// 且路径矛盾被官方澄清后再增量。
    /// </para>
    /// <para>
    /// <b>字段契约已核验并留档在此</b>（下次无需重新核验）：请求 <c>out_order_no</c> / <c>appid</c> /
    /// <c>openid</c> / <c>service_id</c> / <c>service_introduction</c> / <c>post_payments</c>（必填，1~100 条）/
    /// <c>post_discounts</c>（选填，<b>最多 5 条</b>，注意与其它接口的 30 条不同）/ <c>time_range</c>（必填）/
    /// <c>location</c>（选填）/ <c>total_amount</c>（必填）/ <c>profit_sharing</c> / <c>goods_tag</c> /
    /// <c>attach</c> / <c>notify_url</c>；应答为 <c>appid</c> / <c>mchid</c> / <c>out_order_no</c> /
    /// <c>service_id</c> / <c>order_id</c> / <c>service_introduction</c> / <c>state</c> /
    /// <c>state_description</c> / <c>post_payments</c> / <c>post_discounts</c> / <c>time_range</c> /
    /// <c>location</c> / <c>total_amount</c> / <c>attach</c> / <c>notify_url</c>。
    /// </para>
    /// </remarks>
    [Fact]
    public void DirectCompleteEndpoint_ShouldStayUnmodeled()
    {
        var routes = typeof(IWechatPayPayScoreService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri)
            .ToList();

        routes.Should().NotContain("/v3/payscore/serviceorder/direct-complete",
            "官方声明「暂未对外开放」⇒ 不建模；恢复建模前须先确认开通权限与真实路径");
        routes.Should().NotContain("/v3/payscore/partner/serviceorder/direct-complete",
            "官方示例里的 partner 路径与接口说明矛盾，未经澄清不得采用");

        routes.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        // 查询：必填 service_id + appid；out_order_no 与 query_id 为「二选一」的可选参数。
        var query = FindMethod(nameof(IWechatPayPayScoreService.QueryServiceOrderAsync));
        query.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "service_id", "appid", "out_order_no", "query_id" });

        // 取消：out_order_no 走 path（占位符名 = C# 参数名，生成器 HTTPCLIENT013 强制）。
        var cancel = FindMethod(nameof(IWechatPayPayScoreService.CancelServiceOrderAsync));
        cancel.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "outOrderNo" });

        // 创建与取消为 POST、查询为 GET（防「顺手把查询改 POST」）。
        FindMethod(nameof(IWechatPayPayScoreService.CreateServiceOrderAsync))
            .GetCustomAttribute<PostAttribute>().Should().NotBeNull();
        FindMethod(nameof(IWechatPayPayScoreService.QueryServiceOrderAsync))
            .GetCustomAttribute<GetAttribute>().Should().NotBeNull();
    }

    /// <summary>PY-B2：官方字段名锁定（含反直觉的必填性事实）。</summary>
    [Fact]
    public void PayScoreDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<PayScoreServiceOrderRequest>(nameof(PayScoreServiceOrderRequest.OutOrderNo), "out_order_no");
        JsonNameShouldBe<PayScoreServiceOrderRequest>(nameof(PayScoreServiceOrderRequest.ServiceIntroduction), "service_introduction");
        JsonNameShouldBe<PayScoreServiceOrderRequest>(nameof(PayScoreServiceOrderRequest.PostPayments), "post_payments");
        JsonNameShouldBe<PayScoreServiceOrderRequest>(nameof(PayScoreServiceOrderRequest.RiskFund), "risk_fund");
        JsonNameShouldBe<PayScoreServiceOrderRequest>(nameof(PayScoreServiceOrderRequest.NeedUserConfirm), "need_user_confirm");
        JsonNameShouldBe<PayScoreServiceOrderRequest>(nameof(PayScoreServiceOrderRequest.NotifyUrl), "notify_url");

        JsonNameShouldBe<PayScoreTimeRange>(nameof(PayScoreTimeRange.StartTimeRemark), "start_time_remark");
        JsonNameShouldBe<PayScoreRiskFund>(nameof(PayScoreRiskFund.Name), "name");
        JsonNameShouldBe<PayScoreDevice>(nameof(PayScoreDevice.MaterielNo), "materiel_no");

        // 创单应答的 package 是「拉起支付分小程序确认订单页」的凭据，字段名就是 package。
        JsonNameShouldBe<PayScoreServiceOrderResponse>(nameof(PayScoreServiceOrderResponse.Package), "package");
        JsonNameShouldBe<PayScoreServiceOrderResponse>(nameof(PayScoreServiceOrderResponse.OrderId), "order_id");

        JsonNameShouldBe<PayScoreServiceOrderQueryResponse>(nameof(PayScoreServiceOrderQueryResponse.NeedCollection), "need_collection");
        JsonNameShouldBe<PayScoreServiceOrderQueryResponse>(nameof(PayScoreServiceOrderQueryResponse.TotalAmount), "total_amount");
        JsonNameShouldBe<PayScoreCollection>(nameof(PayScoreCollection.PayingAmount), "paying_amount");
        JsonNameShouldBe<PayScoreCollectionDetail>(nameof(PayScoreCollectionDetail.PaidType), "paid_type");
        JsonNameShouldBe<PayScorePromotionDetail>(nameof(PayScorePromotionDetail.WechatpayContribute), "wechatpay_contribute");
        JsonNameShouldBe<PayScorePromotionGoodsDetail>(nameof(PayScorePromotionGoodsDetail.DiscountAmount), "discount_amount");

        JsonNameShouldBe<PayScoreCancelOrderRequest>(nameof(PayScoreCancelOrderRequest.Reason), "reason");
        JsonNameShouldBe<PayScoreCancelOrderResponse>(nameof(PayScoreCancelOrderResponse.OrderId), "order_id");

        JsonNameShouldBe<PayScoreCompleteOrderRequest>(nameof(PayScoreCompleteOrderRequest.TotalAmount), "total_amount");
        JsonNameShouldBe<PayScoreCompleteOrderRequest>(nameof(PayScoreCompleteOrderRequest.ProfitSharing), "profit_sharing");
        JsonNameShouldBe<PayScoreCompleteOrderRequest>(nameof(PayScoreCompleteOrderRequest.GoodsTag), "goods_tag");
        JsonNameShouldBe<PayScoreCompleteOrderResponse>(nameof(PayScoreCompleteOrderResponse.NeedCollection), "need_collection");

        JsonNameShouldBe<PayScoreModifyOrderRequest>(nameof(PayScoreModifyOrderRequest.Reason), "reason");
        JsonNameShouldBe<PayScoreModifyOrderRequest>(nameof(PayScoreModifyOrderRequest.TotalAmount), "total_amount");
        JsonNameShouldBe<PayScoreModifyOrderResponse>(nameof(PayScoreModifyOrderResponse.Collection), "collection");
        JsonNameShouldBe<PayScoreModifyCollection>(nameof(PayScoreModifyCollection.PromotionDetail), "promotion_detail");

        JsonNameShouldBe<PayScoreCollectRequest>(nameof(PayScoreCollectRequest.ServiceId), "service_id");
        JsonNameShouldBe<PayScoreCollectResponse>(nameof(PayScoreCollectResponse.OrderId), "order_id");

        JsonNameShouldBe<PayScorePermissionsRequest>(nameof(PayScorePermissionsRequest.AuthorizationCode), "authorization_code");
        JsonNameShouldBe<PayScorePermissionsResponse>(nameof(PayScorePermissionsResponse.ApplyPermissionsToken), "apply_permissions_token");
        JsonNameShouldBe<PayScoreAuthorizationRecordResponse>(nameof(PayScoreAuthorizationRecordResponse.AuthorizationState), "authorization_state");
        JsonNameShouldBe<PayScoreAuthorizationRecordResponse>(nameof(PayScoreAuthorizationRecordResponse.CancelAuthorizationTime), "cancel_authorization_time");

        JsonNameShouldBe<PayScoreTerminateAuthorizationRequest>(nameof(PayScoreTerminateAuthorizationRequest.Reason), "reason");
        JsonNameShouldBe<PayScoreSyncOrderRequest>(nameof(PayScoreSyncOrderRequest.Type), "type");
        JsonNameShouldBe<PayScoreSyncOrderRequest>(nameof(PayScoreSyncOrderRequest.Detail), "detail");
        JsonNameShouldBe<PayScoreSyncOrderDetail>(nameof(PayScoreSyncOrderDetail.PaidTime), "paid_time");
        JsonNameShouldBe<PayScoreSyncOrderResponse>(nameof(PayScoreSyncOrderResponse.OpenId), "openid");
    }

    /// <summary>PY-B8：同步应答<b>不得</b>并入查询应答（官方本页无 <c>promotion_detail</c>）。</summary>
    [Fact]
    public void SyncOrderResponse_ShouldNotBeMergedWithQueryResponse()
    {
        typeof(PayScoreSyncOrderResponse).GetProperty("PromotionDetail").Should().BeNull(
            "官方同步订单状态页的应答字段表没有 promotion_detail");
        typeof(PayScoreServiceOrderQueryResponse).GetProperty("PromotionDetail").Should().NotBeNull();
    }

    /// <summary>
    /// PY-B7：<b>两个 <c>collection</c> 类型不得合并</b>（官方两页嵌套结构不同）。
    /// </summary>
    /// <remarks>
    /// 修改页把 <c>promotion_detail</c> / <c>goods_detail</c> <b>嵌在 collection 下</b>；
    /// 查询页把它们放在<b>顶层</b>。合并任一方向都会让一个接口带上另一个接口才有的嵌套，
    /// 调用方据此写出的解析路径在真实报文中必然落空。
    /// </remarks>
    [Fact]
    public void CollectionShapes_ShouldStayStructurallySeparate()
    {
        typeof(PayScoreModifyCollection).GetProperty("PromotionDetail").Should().NotBeNull(
            "修改页 collection 内嵌 promotion_detail");
        typeof(PayScoreModifyCollection).GetProperty("GoodsDetail").Should().NotBeNull(
            "修改页 collection 内嵌 goods_detail");

        typeof(PayScoreCollection).GetProperty("PromotionDetail").Should().BeNull(
            "查询页的 collection 不含 promotion_detail（它在顶层）");
        typeof(PayScoreServiceOrderQueryResponse).GetProperty("PromotionDetail").Should().NotBeNull(
            "查询页的 promotion_detail 在顶层");
    }

    /// <summary>
    /// PY-B6：<b>完结应答与查询应答不得合并</b>（完结应答是查询应答的真子集，但官方两页字段表不同）。
    /// </summary>
    /// <remarks>
    /// 合并会给出「本接口永不返回的字段」（<c>collection</c> / <c>promotion_detail</c> / <c>attach</c> /
    /// <c>notify_url</c> / <c>openid</c>）——永不返回的字段是静默误导，调用方会据此写出永不命中的分支。
    /// </remarks>
    [Fact]
    public void CompleteOrderResponse_ShouldNotBeMergedWithQueryResponse()
    {
        typeof(PayScoreCompleteOrderResponse).GetProperty("Collection").Should().BeNull(
            "官方完结应答字段表没有 collection");
        typeof(PayScoreCompleteOrderResponse).GetProperty("NotifyUrl").Should().BeNull(
            "官方完结应答字段表没有 notify_url");
        typeof(PayScoreCompleteOrderResponse).GetProperty("TotalAmount").Should().NotBeNull();

        typeof(PayScoreServiceOrderQueryResponse).GetProperty("Collection").Should().NotBeNull();
    }

    /// <summary>
    /// PY-B3：<b>三个应答形态互不相同</b>，不得合并（官方三页字段表各异）。
    /// </summary>
    /// <remarks>
    /// 创建应答有 <c>package</c> 无 <c>collection</c>；查询应答有 <c>collection</c>/<c>promotion_detail</c>
    /// 无 <c>package</c>；取消应答只有 5 个顶层字段。合并会在任一端点上给出「不存在的字段」。
    /// </remarks>
    [Fact]
    public void PayScoreResponses_ShouldStaySeparate_AndDeriveFromWechatPayResponse()
    {
        typeof(PayScoreServiceOrderResponse).GetProperty("Package").Should().NotBeNull();
        typeof(PayScoreServiceOrderResponse).GetProperty("Collection").Should().BeNull(
            "创建应答官方字段表没有 collection（那是查询应答的字段）");

        typeof(PayScoreServiceOrderQueryResponse).GetProperty("Collection").Should().NotBeNull();
        typeof(PayScoreServiceOrderQueryResponse).GetProperty("Package").Should().BeNull(
            "查询应答官方字段表没有 package（那是创建应答的字段）");

        foreach (var responseType in new[]
                 {
                     typeof(PayScoreServiceOrderResponse),
                     typeof(PayScoreServiceOrderQueryResponse),
                     typeof(PayScoreCancelOrderResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse 以承载官方 code/message（判错面）");
        }
    }

    /// <summary>PY-B4：官方枚举取值逐项锁定（状态 / 状态说明 / 风险金 / 收款 / 优惠）。</summary>
    [Fact]
    public void PayScoreEnumConstants_ShouldMatchOfficialValues()
    {
        var states = LiteralsOf(typeof(PayScoreServiceOrderStates));
        states.Should().BeEquivalentTo(new[] { "CREATED", "DOING", "DONE", "REVOKED", "EXPIRED" });
        states.Should().HaveCount(5, "官方服务订单状态为 5 值");

        var riskFundNames = LiteralsOf(typeof(PayScoreRiskFundNames));
        riskFundNames.Should().BeEquivalentTo(new[]
        {
            "DEPOSIT", "ADVANCE", "CASH_DEPOSIT", "ESTIMATE_ORDER_COST",
        });
        riskFundNames.Should().HaveCount(4, "官方风险金名称 4 值（先免 3 + 先享 1）");

        PayScoreStartTimeKeywords.OnAccept.Should().Be("OnAccept");
        PayScoreStateDescriptions.UserConfirm.Should().Be("USER_CONFIRM");
        PayScoreStateDescriptions.MchComplete.Should().Be("MCH_COMPLETE");

        LiteralsOf(typeof(PayScoreCollectionStates)).Should().BeEquivalentTo(new[] { "USER_PAYING", "USER_PAID" });
        LiteralsOf(typeof(PayScoreCollectionPaidTypes)).Should().BeEquivalentTo(new[] { "NEWTON", "ADVANCE", "BALANCE" });
        LiteralsOf(typeof(PayScorePromotionScopes)).Should().BeEquivalentTo(new[] { "GLOBAL", "SINGLE" });
        LiteralsOf(typeof(PayScorePromotionTypes)).Should().BeEquivalentTo(new[] { "CASH", "DISCOUNT" });

        // 授权状态：UNBINDUSER（仅完成预授权）与 UNAVAILABLE（授权后解除）语义相反，两者都必须在表内。
        var authorizationStates = LiteralsOf(typeof(PayScoreAuthorizationStates));
        authorizationStates.Should().BeEquivalentTo(new[] { "UNBINDUSER", "AVAILABLE", "UNAVAILABLE" });
        authorizationStates.Should().HaveCount(3, "官方授权状态为 3 值");

        // 同步场景类型：官方仅一值，且是**混合大小写** Order_Paid（勿「规范化」）。
        PayScoreSyncOrderTypes.OrderPaid.Should().Be("Order_Paid");
        LiteralsOf(typeof(PayScoreSyncOrderTypes)).Should().HaveCount(1, "官方 type 只给出 Order_Paid 一个取值");
    }

    /// <summary>PY-B5：支付分域 DTO 全量登记进 AOT 上下文且分组名一致。</summary>
    [Fact]
    public void PayScoreDataModels_ShouldBeRegisteredInJsonContext()
    {
        var domainTypes = typeof(PayScoreServiceOrderResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(29,
            "支付分域 DTO：创单族 7（请求/后付费/优惠/时间段/位置/风险金/设备）" +
            " + 创建应答 1 + 查询应答 1 + 查询嵌套 4（收款/收款明细/优惠/优惠单品）" +
            " + 取消族 2（请求/应答） + 完结族 2（请求/应答）" +
            " + 修改族 3（请求/收款/应答） + 催收扣款族 2（请求/应答）" +
            " + 授权面 3（预授权请求/预授权应答/授权记录应答）");

        foreach (var type in domainTypes)
        {
            PayScoreJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PayScoreJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
    }

    // ---- helpers -------------------------------------------------------------

    private static string[] LiteralsOf(Type type)
        => type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayPayScoreService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayPayScoreService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }
}
