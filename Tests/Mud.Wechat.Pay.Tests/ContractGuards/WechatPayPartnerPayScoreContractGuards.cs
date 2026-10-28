// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.PartnerPayScore;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-4 服务商支付分域契约守卫（<b>23 端点</b>：授权 7 + 服务订单 8 + 服务额度 1 + 支付分计划 3 +
/// 计划型服务订单 1 + 用户签约计划 3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（服务商文档中心，2026-10-14 逐页核验，23 页一一对应）</b>：
/// 4012382030 商户预授权、4012778802 开启场景服务权限、4012783585 评估用户分层、
/// 4012692687 / 4012692798 授权记录两路查询、4012382049 / 4012461154 解除授权两变体、
/// 4012692136 创建订单、4012692256 查询订单、4012458758 取消、4012692408 完结、
/// 4012692360 修改金额、4013394914 未完结提醒、4013394929 确认付款提醒、4012458816 同步支付结果、
/// 4013394486 查询建议先享金额、4012463290 / 4012463318 / 4012463322 计划三支、
/// 4012463469 计划型服务订单、4012463521 / 4012463526 / 4012463534 用户签约计划三支。
/// 逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>SKIT 清单 28 行 ⇒ 本域 23 端点的裁定</b>（守卫 PS-B2 以「零声明 + 正对照」锁定）：
/// ① 「商户收款」<c>POST /v3/payscore/serviceorder/{out_order_no}/pay</c> 的出处是<b>已下架旧 wiki 页</b>
/// （现 302 跳到无关页），现行服务商支付分 API 列表侧栏 12 项不含该能力 ⇒ 本域零端点，
/// <b>不得</b>按直连面同名路由补进来；② 授权记录查询、计划应答等在多页重复声明的行按同端点归并。
/// </para>
/// <para>
/// <b>本守卫要守住的五处「顺手就会写错」</b>（多数源自与相邻两域的形似神异）：
/// ① 商户标识<b>与服务商基础支付族相反</b> —— 本域用 <c>appid</c>/<c>mchid</c>（服务商）+
/// <c>sub_appid</c>/<c>sub_mchid</c>（子商户），<b>全域无 <c>sp_</c> 前缀</b>；
/// ② 同一业务域内<b>三套路由前缀</b>（<c>/v3/payscore/partner/</c>、<c>/v3/payscore/plan/partner/</c>、
/// <c>/v3/payscore/sign-plan/partner/</c>）；③ <c>sub_mchid</c> 在 7 支端点走 <b>query</b> 而非 body；
/// ④ <b>两套时间格式并存</b>（紧凑 <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c> 与 rfc3339）⇒ DTO 时间字段恒 <c>string</c>；
/// ⑤ 官方自身跨页拼写不一致（<c>MCH_COMPLETE</c> 与 <c>MERCHANT_COMPLETE</c>、
/// <c>total_original_price</c> 与 <c>total_origin_price</c>）⇒ 照各自页原文，不「统一」。
/// </para>
/// </remarks>
public class WechatPayPartnerPayScoreContractGuards
{
    private const string RegistryGroupName = "PartnerPayScore";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PartnerPayScore";

    private const string PartnerPrefix = "/v3/payscore/partner/";

    private const string PlanPrefix = "/v3/payscore/plan/partner/";

    private const string SignPlanPrefix = "/v3/payscore/sign-plan/partner/";

    /// <summary>
    /// PS-B1：23 端点的方法与路由照官方原文；<c>sub_mchid</c> 的 query / body 位置逐端点锁定，
    /// 八支 204 端点必须返回裸 <see cref="Task"/>。
    /// </summary>
    [Fact]
    public void PartnerPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions()
    {
        // 授权族 7 支。
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.PreAuthorizeAsync),
            PartnerPrefix + "permissions");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.PreAuthorizeForSceneAsync),
            PartnerPrefix + "permissions/apply-for-scene");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.AssessUserRiskLevelAsync),
            PartnerPrefix + "permissions/assess-user-risk-level");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerPayScoreService.QueryAuthorizationRecordAsync),
            PartnerPrefix + "permissions/authorization-code/{authorizationCode}");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerPayScoreService.QueryAuthorizationRecordByOpenIdAsync),
            PartnerPrefix + "permissions/search");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.TerminateAuthorizationByCodeAsync),
            PartnerPrefix + "permissions/authorization-code/{authorizationCode}/terminate");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.TerminateAuthorizationByOpenIdAsync),
            PartnerPrefix + "permissions/terminate");

        // 服务订单族 8 支 + 服务额度 1 支。
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.CreateServiceOrderAsync),
            PartnerPrefix + "serviceorder");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerPayScoreService.QueryServiceOrderAsync),
            PartnerPrefix + "serviceorder");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.CancelServiceOrderAsync),
            PartnerPrefix + "serviceorder/{outOrderNo}/cancel");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.CompleteServiceOrderAsync),
            PartnerPrefix + "serviceorder/{outOrderNo}/complete");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.ModifyServiceOrderAsync),
            PartnerPrefix + "serviceorder/{outOrderNo}/modify");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.SendPendingMessageAsync),
            PartnerPrefix + "serviceorder/{outOrderNo}/send-pendingmessage");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.SendPrepaidMessageAsync),
            PartnerPrefix + "serviceorder/{outOrderNo}/send-prepaidmessage");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.SyncServiceOrderAsync),
            PartnerPrefix + "serviceorder/{outOrderNo}/sync");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerPayScoreService.GetServiceQuotasAsync),
            PartnerPrefix + "servicequotas/service-id/{serviceId}");

        // 支付分计划 3 支。
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.CreatePayScorePlanAsync),
            PlanPrefix + "payscore-plans");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerPayScoreService.QueryPayScorePlanAsync),
            PlanPrefix + "payscore-plans/merchant-plan-no/{merchantPlanNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.StopPayScorePlanAsync),
            PlanPrefix + "payscore-plans/merchant-plan-no/{merchantPlanNo}/stop");

        // 计划型服务订单 1 支 + 用户签约计划 3 支。
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.CreateSignPlanServiceOrderAsync),
            SignPlanPrefix + "serviceorder");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.CreateUserSignPlanAsync),
            SignPlanPrefix + "user-sign-plans");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerPayScoreService.QueryUserSignPlanAsync),
            SignPlanPrefix + "user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerPayScoreService.StopUserSignPlanAsync),
            SignPlanPrefix + "user-sign-plans/merchant-sign-plan-no/{merchantSignPlanNo}/stop");

        // sub_mchid 位置：7 支在 query（六支 GET + 评估用户分层这支 POST），其余在 body。
        var querySubMchId = new[]
        {
            nameof(IWechatPayPartnerPayScoreService.AssessUserRiskLevelAsync),
            nameof(IWechatPayPartnerPayScoreService.QueryAuthorizationRecordAsync),
            nameof(IWechatPayPartnerPayScoreService.QueryAuthorizationRecordByOpenIdAsync),
            nameof(IWechatPayPartnerPayScoreService.QueryServiceOrderAsync),
            nameof(IWechatPayPartnerPayScoreService.GetServiceQuotasAsync),
            nameof(IWechatPayPartnerPayScoreService.QueryPayScorePlanAsync),
            nameof(IWechatPayPartnerPayScoreService.QueryUserSignPlanAsync),
        };
        foreach (var name in querySubMchId)
        {
            QueryNamesOf(name).Should().Contain("sub_mchid",
                $"{name} 的 sub_mchid 官方放在 query：下沉进 body 即 PARAM_ERROR");
        }

        // 评估用户分层是唯一「POST + query + body」形态：body 里不得再有 sub_mchid。
        QueryNamesOf(nameof(IWechatPayPartnerPayScoreService.AssessUserRiskLevelAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid" }, $"{nameof(IWechatPayPartnerPayScoreService.AssessUserRiskLevelAsync)} 只有 sub_mchid 一个 query 参数");
        JsonNamesOf<PartnerPayScoreRiskAssessRequest>().Should().NotContain("sub_mchid",
            "官方本页请求体表无 sub_mchid（它在 query），照基础支付族抄进来即制造非法字段");

        // 六支 GET 一律无请求体；七支纯 body 端点一律无 path / query。
        foreach (var method in typeof(IWechatPayPartnerPayScoreService)
                     .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(static m => m.GetCustomAttribute<GetAttribute>() != null))
        {
            method.GetParameters().Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method.Name} 是 GET，官方无请求体");
        }

        foreach (var name in new[]
                 {
                     nameof(IWechatPayPartnerPayScoreService.PreAuthorizeAsync),
                     nameof(IWechatPayPartnerPayScoreService.PreAuthorizeForSceneAsync),
                     nameof(IWechatPayPartnerPayScoreService.TerminateAuthorizationByOpenIdAsync),
                     nameof(IWechatPayPartnerPayScoreService.CreateServiceOrderAsync),
                     nameof(IWechatPayPartnerPayScoreService.CreatePayScorePlanAsync),
                     nameof(IWechatPayPartnerPayScoreService.CreateSignPlanServiceOrderAsync),
                     nameof(IWechatPayPartnerPayScoreService.CreateUserSignPlanAsync),
                 })
        {
            var parameters = FindMethod(name).GetParameters();
            parameters.Should().NotContain(static p => p.GetCustomAttribute<PathAttribute>() != null,
                $"{name} 官方无 path 参数");
            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{name} 官方无 query 参数");
        }

        // path 定位键逐支锁定：官方用 out_order_no / authorization_code / merchant_plan_no / merchant_sign_plan_no / service_id。
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.TerminateAuthorizationByCodeAsync))
            .Should().Be("authorizationCode");
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.QueryAuthorizationRecordAsync))
            .Should().Be("authorizationCode");
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.GetServiceQuotasAsync)).Should().Be("serviceId");
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.QueryPayScorePlanAsync)).Should().Be("merchantPlanNo");
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.StopPayScorePlanAsync)).Should().Be("merchantPlanNo");
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.QueryUserSignPlanAsync)).Should().Be("merchantSignPlanNo");
        PathKeyOf(nameof(IWechatPayPartnerPayScoreService.StopUserSignPlanAsync)).Should().Be("merchantSignPlanNo");

        // 八支 204 端点：官方成功应答无包体 ⇒ 返回裸 Task（声明成响应类型即造出永不填充的假字段）。
        var noContent = typeof(IWechatPayPartnerPayScoreService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();
        noContent.Should().BeEquivalentTo(new[]
        {
            nameof(IWechatPayPartnerPayScoreService.TerminateAuthorizationByCodeAsync),
            nameof(IWechatPayPartnerPayScoreService.TerminateAuthorizationByOpenIdAsync),
            nameof(IWechatPayPartnerPayScoreService.CancelServiceOrderAsync),
            nameof(IWechatPayPartnerPayScoreService.CompleteServiceOrderAsync),
            nameof(IWechatPayPartnerPayScoreService.ModifyServiceOrderAsync),
            nameof(IWechatPayPartnerPayScoreService.SendPendingMessageAsync),
            nameof(IWechatPayPartnerPayScoreService.SendPrepaidMessageAsync),
            nameof(IWechatPayPartnerPayScoreService.SyncServiceOrderAsync),
        }, "本域恰八支 204 端点，其余 15 支均须有应答类型");
    }

    /// <summary>
    /// PS-B2：三套前缀的分布 + 「商户收款」零声明（正对照：直连域确有同名能力端点）+ 与直连支付分域前缀互斥。
    /// </summary>
    [Fact]
    public void PartnerPayScoreRoutes_ShouldStayOnThreePartnerPrefixesAndNotDeclareMerchantCollectPay()
    {
        var routes = RoutesOf(typeof(IWechatPayPartnerPayScoreService));

        routes.Should().HaveCount(23, "本域 23 端点（数量变化须同批更新 PS-B1 与 Tests/ContractBaseline.cs）");

        routes.Count(static r => r.StartsWith(PartnerPrefix, StringComparison.Ordinal))
            .Should().Be(16, "授权 7 + 服务订单 8 + 服务额度 1 共 16 条在 /v3/payscore/partner/ 下");
        routes.Count(static r => r.StartsWith(PlanPrefix, StringComparison.Ordinal))
            .Should().Be(3, "支付分计划三支在 /v3/payscore/plan/partner/ 下");
        routes.Count(static r => r.StartsWith(SignPlanPrefix, StringComparison.Ordinal))
            .Should().Be(4, "计划型服务订单 1 + 用户签约计划 3 共四支在 /v3/payscore/sign-plan/partner/ 下");

        // 官方服务商面没有「商户收款」端点（旧 wiki 页已 302 到无关页，现行 API 列表 12 项无该能力）。
        routes.Should().NotContain(static r => r.EndsWith("/pay", StringComparison.Ordinal),
            "服务商支付分无商户收款端点：按直连面补进来即制造官方不存在的契约");
        routes.Should().NotContain(static r => r.StartsWith("/v3/payscore/serviceorder", StringComparison.Ordinal),
            "直连面前缀不得出现在本域");

        // 正对照：同一扫描口径下，直连域确实声明了 …/serviceorder/{out_order_no}/pay ⇒
        // 证明「扫描根与匹配形态有效」，否则上面的零声明断言会因漂移而静默恒真。
        var directRoutes = RoutesOf(typeof(IWechatPayPayScoreService));
        directRoutes.Should().Contain(static r => r.EndsWith("/pay", StringComparison.Ordinal),
            "正对照：直连支付分域确实有商户收款端点，本域的零声明是有据裁决而非漏落");

        // 两线前缀互斥：本域全部带 partner 段，直连域全部不带。
        directRoutes.Should().NotContain(static r => r.Contains("/partner/", StringComparison.Ordinal),
            "直连域不得混入服务商段");
        routes.Intersect(directRoutes).Should().BeEmpty("两线路由不得重叠，否则同一宿主配置会打到不同契约面");
    }

    /// <summary>
    /// PS-B3：官方字段名逐项锁定（<b>与服务商基础支付族相反</b>，全域无 <c>sp_</c> 前缀）+
    /// 官方跨页拼写分歧照各自页 + 两套时间格式并存下时间字段恒 <c>string</c>。
    /// </summary>
    [Fact]
    public void PartnerPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings()
    {
        // 商户标识：本域服务商侧就叫 appid / mchid，服务商基础支付族的 sp_ 前缀在此不存在。
        JsonNameShouldBe<PartnerPayScoreCreateOrderRequest>(nameof(PartnerPayScoreCreateOrderRequest.AppId), "appid");
        JsonNameShouldBe<PartnerPayScoreCreateOrderRequest>(nameof(PartnerPayScoreCreateOrderRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<PartnerPayScoreCreateOrderRequest>(nameof(PartnerPayScoreCreateOrderRequest.SubAppId), "sub_appid");
        JsonNameShouldBe<PartnerPayScoreCreateOrderResponse>(nameof(PartnerPayScoreCreateOrderResponse.MchId), "mchid");
        JsonNameShouldBe<PartnerPayScoreCreateOrderResponse>(nameof(PartnerPayScoreCreateOrderResponse.AppId), "appid");
        JsonNameShouldBe<PartnerPayScoreAuthorizationRecordResponse>(nameof(PartnerPayScoreAuthorizationRecordResponse.MchId), "mchid");

        // 全域扫一遍：任何 DTO 都不得出现 sp_ 前缀字段（跨域抄 PartnerJsapiPrepayRequest 的必然症状）。
        var allJsonNames = typeof(PartnerPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && t.Namespace == DomainNamespace)
            .SelectMany(static t => JsonNamesOf(t))
            .ToArray();
        allJsonNames.Should().NotContain(static n => n.StartsWith("sp_", StringComparison.Ordinal),
            "本域官方字段是 appid/mchid，不存在 sp_ 前缀形态");
        allJsonNames.Should().NotBeEmpty("参照集为空会让上面的零声明断言静默恒真");
        JsonNamesOf<PartnerJsapiPrepayRequest>().Should().Contain("sp_appid",
            "正对照：服务商基础支付族确有 sp_ 前缀，两域形态相反");

        // 三支 token / 场景节：命名各异，不可「统一」成一个字段名。
        JsonNameShouldBe<PartnerPayScorePermissionsResponse>(nameof(PartnerPayScorePermissionsResponse.ApplyPermissionsToken), "apply_permissions_token");
        JsonNameShouldBe<PartnerPayScoreScenePermissionsResponse>(nameof(PartnerPayScoreScenePermissionsResponse.TransactionPermissionsToken), "transaction_permissions_token");
        JsonNameShouldBe<PartnerPayScoreScenePermissionsRequest>(nameof(PartnerPayScoreScenePermissionsRequest.TransactionSceneInfo), "transaction_scene_info");
        JsonNameShouldBe<PartnerPayScoreTransactionSceneInfo>(nameof(PartnerPayScoreTransactionSceneInfo.MchId), "mchid");

        // 免确认模式开关 + 消息 / 同步两支的独有字段。
        JsonNameShouldBe<PartnerPayScoreCreateOrderRequest>(nameof(PartnerPayScoreCreateOrderRequest.NeedUserConfirm), "need_user_confirm");
        JsonNameShouldBe<PartnerPayScoreMessageRequest>(nameof(PartnerPayScoreMessageRequest.PrepaidTime), "prepaid_time");
        JsonNameShouldBe<PartnerPayScoreSyncOrderRequest>(nameof(PartnerPayScoreSyncOrderRequest.Type), "type");
        JsonNameShouldBe<PartnerPayScoreSyncOrderDetail>(nameof(PartnerPayScoreSyncOrderDetail.PaidTime), "paid_time");

        // materiel_no 是 device 的子字段（官方创单示例报文即嵌在 device 内），提到顶层会静默丢字段。
        JsonNamesOf<PartnerPayScoreDevice>().Should().Contain("materiel_no");
        JsonNamesOf<PartnerPayScoreCreateOrderRequest>().Should().NotContain("materiel_no",
            "该字段属 device 节，请求顶层出现即报文错位");

        // 官方跨页拼写分歧照各自页：计划族 total_original_price vs 签约计划族 total_origin_price。
        JsonNameShouldBe<PartnerPayScorePlanResponse>(nameof(PartnerPayScorePlanResponse.TotalOriginalPrice), "total_original_price");
        JsonNameShouldBe<PartnerPayScoreSignPlan>(nameof(PartnerPayScoreSignPlan.TotalOriginPrice), "total_origin_price");
        JsonNameShouldBe<PartnerPayScoreSignPlan>(nameof(PartnerPayScoreSignPlan.GoingDetailNo), "going_detail_no");
        JsonNameShouldBe<PartnerPayScoreUserSignPlanDetail>(nameof(PartnerPayScoreUserSignPlanDetail.MerchantPlanDetailNo), "merchant_plan_detail_no");
        JsonNameShouldBe<PartnerPayScoreSignPlanServiceOrderRequest>(nameof(PartnerPayScoreSignPlanServiceOrderRequest.OutTradeNo), "out_trade_no");

        // 两套时间格式并存 ⇒ 全部时间字段保持 string，不做 DateTimeOffset 往返（紧凑格式无法被 rfc3339 解析器接受）。
        typeof(PartnerPayScoreTimeRange).GetProperty(nameof(PartnerPayScoreTimeRange.StartTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(PartnerPayScoreSyncOrderDetail).GetProperty(nameof(PartnerPayScoreSyncOrderDetail.PaidTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(PartnerPayScoreCompleteOrderRequest).GetProperty(nameof(PartnerPayScoreCompleteOrderRequest.CompleteTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(PartnerPayScoreSignPlan).GetProperty(nameof(PartnerPayScoreSignPlan.PlanOverTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(PartnerPayScoreSignPlan).GetProperty(nameof(PartnerPayScoreSignPlan.CancelSignTime))!
            .PropertyType.Should().Be(typeof(string));

        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset：本域两套时间格式并存，类型化即丢格式");
                property.PropertyType.Should().NotBe(typeof(DateTime));
            }
        }
    }

    /// <summary>PS-B4：官方枚举取值逐项锁定 + 与直连支付分域的四处取值分歧（防「顺手复用直连常量」）。</summary>
    [Fact]
    public void PartnerPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromDirectDomain()
    {
        ValuesOf(typeof(PartnerPayScoreServiceOrderStates)).Should().BeEquivalentTo(new[]
        {
            "CREATED", "DOING", "DONE", "REVOKED", "EXPIRED",
        }, "官方 state 为 5 值，状态流转须按官方流转图，不得凭直觉跳转");

        ValuesOf(typeof(PartnerPayScoreStateDescriptions)).Should().BeEquivalentTo(new[]
        {
            "USER_CONFIRM", "MCH_COMPLETE", "MERCHANT_COMPLETE",
        }, "官方跨页拼写不一致：完结页用 MCH_COMPLETE、计划型订单页用 MERCHANT_COMPLETE ⇒ 两个取值都收，不做「统一」");

        ValuesOf(typeof(PartnerPayScoreCollectionStates)).Should().BeEquivalentTo(new[] { "USER_PAYING", "USER_PAID" });

        ValuesOf(typeof(PartnerPayScoreCollectionPaidTypes)).Should().BeEquivalentTo(new[] { "NEWTON", "MCH" },
            "服务商面只有新收款与商户收款两值");

        ValuesOf(typeof(PartnerPayScoreSyncTypes)).Should().BeEquivalentTo(new[] { "Order_Paid" },
            "官方 type 值大小写与下划线混排（Order_Paid），不得驼峰化");

        ValuesOf(typeof(PartnerPayScoreRiskFundNames)).Should().BeEquivalentTo(new[] { "ESTIMATE_ORDER_COST" },
            "服务商创单页固定传 ESTIMATE_ORDER_COST（先享模式），直连面的三项先免模式风险名在本域不可用");

        ValuesOf(typeof(PartnerPayScoreStartTimeKeywords)).Should().BeEquivalentTo(new[] { "OnAccept" });
        ValuesOf(typeof(PartnerPayScoreScenes)).Should().BeEquivalentTo(new[] { "TRANSACTION" });

        ValuesOf(typeof(PartnerPayScoreAuthorizationStates)).Should().BeEquivalentTo(new[]
        {
            "UNAVAILABLE", "AVAILABLE", "UNBINDUSER",
        });

        ValuesOf(typeof(PartnerPayScorePromotionScopes)).Should().BeEquivalentTo(new[] { "GLOBAL", "SINGLE" });
        ValuesOf(typeof(PartnerPayScorePromotionTypes)).Should().BeEquivalentTo(new[] { "CASH", "NOCASH" },
            "本域是 NOCASH（无现金），直连面同名字段官方给的是 DISCOUNT ⇒ 两域常量不共用");

        ValuesOf(typeof(PartnerPayScorePlanStates)).Should().BeEquivalentTo(new[] { "NORMAL", "STOP" });

        ValuesOf(typeof(PartnerPayScoreSignStates)).Should().BeEquivalentTo(new[]
        {
            "UNSIGNED", "SIGNED", "SIGN_PLAN_CANCEL", "COMPLETE", "EXPIRE",
        }, "本域是 EXPIRE（动词原形），不是 EXPIRED");

        ValuesOf(typeof(PartnerPayScoreCancelSignTypes)).Should().BeEquivalentTo(new[]
        {
            "NOT_CANCEL", "USER", "MERCHANT", "REVOKE_SERVICE",
        });

        ValuesOf(typeof(PartnerPayScorePlanDetailStates)).Should().BeEquivalentTo(new[]
        {
            "NOT_USED", "USING", "USED", "SIGN_PLAN_DETAIL_CANCEL",
        });

        // 与直连域的四处分歧（正对照）：证明两域常量各自独立，没有「顺手复用」。
        ValuesOf(typeof(PayScoreCollectionPaidTypes)).Should().BeEquivalentTo(new[] { "NEWTON", "ADVANCE", "BALANCE" });
        ValuesOf(typeof(PayScoreStateDescriptions)).Should().NotContain("MERCHANT_COMPLETE");
        ValuesOf(typeof(PayScorePromotionTypes)).Should().Contain("DISCOUNT");
        ValuesOf(typeof(PayScoreRiskFundNames)).Should().HaveCount(4);

        // 常量类清单锁定：本域恰 15 个 static class（值集差异的权威清单），且全部不带 [HttpJsonSerializable]
        // —— 它们不是报文类型，标了只会给源生成上下文添无意义节点。
        var constantClasses = typeof(PartnerPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();
        constantClasses.Should().BeEquivalentTo(new[]
        {
            nameof(PartnerPayScoreServiceOrderStates),
            nameof(PartnerPayScoreStateDescriptions),
            nameof(PartnerPayScoreCollectionStates),
            nameof(PartnerPayScoreCollectionPaidTypes),
            nameof(PartnerPayScoreSyncTypes),
            nameof(PartnerPayScoreRiskFundNames),
            nameof(PartnerPayScoreStartTimeKeywords),
            nameof(PartnerPayScoreScenes),
            nameof(PartnerPayScoreAuthorizationStates),
            nameof(PartnerPayScorePromotionScopes),
            nameof(PartnerPayScorePromotionTypes),
            nameof(PartnerPayScorePlanStates),
            nameof(PartnerPayScoreSignStates),
            nameof(PartnerPayScoreCancelSignTypes),
            nameof(PartnerPayScorePlanDetailStates),
        }, "本域常量类恰 15 个");
    }

    /// <summary>
    /// PS-B5：本域<b>不建专有错误码常量类</b>的裁决锁定（官方各页业务码全是通用名）+ 正对照。
    /// </summary>
    /// <remarks>
    /// 「无授权资格」一支在本域<b>两套拼写并存</b>（2026-10-14 逐页核验）：授权族五页与服务额度页写
    /// <c>NO_AUTH</c>，计划族与用户签约族 7 页写 <c>NOAUTH</c>，服务订单族多数页面只有公共四码 —— 共享常量类只覆盖前者，
    /// 宿主按 <see cref="WechatPayResponse.Code"/> 原样字符串判错，不能依赖常量做「全域归一」。
    /// </remarks>
    [Fact]
    public void PartnerPayScore_ShouldNotDeclareDomainSpecificErrorCodes()
    {
        DomainTypeNames().Should().NotContain(static n => n.EndsWith("ErrorCodes", StringComparison.Ordinal),
            "本域官方错误码全为 APIv3 通用名：建专有码常量类即制造「能识别本域专有错误」的假象");

        // 正对照：进件域确有专有码，两域判错面不同。
        ValuesOf(typeof(Apply4SubMerchantErrorCodes)).Should().Contain("APPLYMENT_NOT_EXIST");

        // 通用码由既有承载面覆盖（判错统一走 WechatPayResponse.Code + 本线通用码常量）。
        var commonCodes = ValuesOf(typeof(Mud.Wechat.Pay.Abstractions.Enums.WechatPayErrorCodes));
        commonCodes.Should().Contain(new[] { "PARAM_ERROR", "INVALID_REQUEST", "SYSTEM_ERROR", "FREQUENCY_LIMITED" },
            "本域官方业务码全为通用名，既有承载面已覆盖");
    }

    /// <summary>
    /// PS-B6：判错面继承 + 「表相同则共用、表不同则分建」的三处共用与两处不共用。
    /// </summary>
    [Fact]
    public void PartnerPayScoreResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch()
    {
        foreach (var responseType in new[]
                 {
                     typeof(PartnerPayScorePermissionsResponse),
                     typeof(PartnerPayScoreScenePermissionsResponse),
                     typeof(PartnerPayScoreRiskAssessResponse),
                     typeof(PartnerPayScoreAuthorizationRecordResponse),
                     typeof(PartnerPayScoreCreateOrderResponse),
                     typeof(PartnerPayScoreQueryOrderResponse),
                     typeof(PartnerPayScoreServiceQuotaResponse),
                     typeof(PartnerPayScorePlanResponse),
                     typeof(PartnerPayScoreSignPlanServiceOrderResponse),
                     typeof(PartnerPayScoreCreateUserSignPlanResponse),
                     typeof(PartnerPayScoreSignPlan),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        // 共用①：未完结提醒与确认付款提醒两支的请求体表逐项一致 ⇒ 同一类型。
        BodyTypeOf(nameof(IWechatPayPartnerPayScoreService.SendPendingMessageAsync))
            .Should().Be(BodyTypeOf(nameof(IWechatPayPartnerPayScoreService.SendPrepaidMessageAsync)),
                "两支消息端点官方字段表相同，共用 PartnerPayScoreMessageRequest");

        // 共用②：创建 / 查询 / 停止计划的应答表相同 ⇒ 同一类型（停止应答多出的 stop_* 已在该类型内）。
        ReturnTypeOf(nameof(IWechatPayPartnerPayScoreService.CreatePayScorePlanAsync))
            .Should().Be(typeof(PartnerPayScorePlanResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerPayScoreService.QueryPayScorePlanAsync))
            .Should().Be(typeof(PartnerPayScorePlanResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerPayScoreService.StopPayScorePlanAsync))
            .Should().Be(typeof(PartnerPayScorePlanResponse));

        // 共用③：签约计划类型同时服务「创建应答的嵌套节」与「查询 / 停止应答的平铺节」。
        ReturnTypeOf(nameof(IWechatPayPartnerPayScoreService.QueryUserSignPlanAsync))
            .Should().Be(typeof(PartnerPayScoreSignPlan));
        ReturnTypeOf(nameof(IWechatPayPartnerPayScoreService.StopUserSignPlanAsync))
            .Should().Be(typeof(PartnerPayScoreSignPlan));
        typeof(PartnerPayScoreCreateUserSignPlanResponse)
            .GetProperty(nameof(PartnerPayScoreCreateUserSignPlanResponse.SignPlan))!
            .PropertyType.Should().Be(typeof(PartnerPayScoreSignPlan));

        // 不共用①：创单应答有 package（拉起确认页凭据）而无 total_amount / collection；查单应答反之。
        DeclaredJsonNamesOf<PartnerPayScoreCreateOrderResponse>().Should().Contain("package");
        DeclaredJsonNamesOf<PartnerPayScoreCreateOrderResponse>().Should().NotContain(new[] { "total_amount", "collection", "openid" });
        DeclaredJsonNamesOf<PartnerPayScoreQueryOrderResponse>().Should().NotContain("package",
            "官方查询页应答表无 package：与创单应答合并类型即制造「查得到 package」的假象");
        DeclaredJsonNamesOf<PartnerPayScoreQueryOrderResponse>().Should().Contain(new[] { "total_amount", "need_collection", "collection", "openid", "sub_openid" });

        // 不共用②：两支预授权应答字段名不同（apply_permissions_token vs transaction_permissions_token）⇒ 分建类型。
        DeclaredJsonNamesOf<PartnerPayScorePermissionsResponse>().Should().BeEquivalentTo(new[] { "apply_permissions_token" });
        DeclaredJsonNamesOf<PartnerPayScoreScenePermissionsResponse>().Should().BeEquivalentTo(new[] { "scene", "transaction_permissions_token" });

        // 风险评估应答 = 授权记录字段全集 + 风险分层四项（官方两页字段表是包含关系，但形态不同 ⇒ 分建不继承）。
        DeclaredJsonNamesOf<PartnerPayScoreRiskAssessResponse>().Should().Contain(
            DeclaredJsonNamesOf<PartnerPayScoreAuthorizationRecordResponse>(),
            "风险评估应答是授权记录的超集");
        DeclaredJsonNamesOf<PartnerPayScoreRiskAssessResponse>().Should().Contain(new[]
        {
            "user_label", "channel_id", "user_risk_level", "risk_level_version",
        });
        typeof(PartnerPayScoreRiskAssessResponse).IsSubclassOf(typeof(PartnerPayScoreAuthorizationRecordResponse))
            .Should().BeFalse("两页各自成表：继承会把授权记录应答的字段表语义绑死");
    }

    /// <summary>
    /// PS-B7：本域 49 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void PartnerPayScoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(49,
            "服务商支付分 DTO：共享构件 6 + 授权族 12 + 服务订单请求 7 + 服务订单应答 8 + 计划与签约 16");

        foreach (var type in domainTypes)
        {
            PartnerPayScoreJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PartnerPayScoreJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（否则源生成上下文里出现无意义节点）：本域类型 = 49 DTO + 15 常量类。
        DomainTypeNames().Should().HaveCount(64,
            "本域类型 = 49 DTO + 15 常量类（上下文类已排除）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", 
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PartnerPayScoreJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        // 三段式：枚举成员 + 建造者注册委托 + 源生成注册方法名。
        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PartnerPayScore 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPartnerPayScoreWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        // 域内形态：组名 + 支付专用客户端 + 零 [Token]（本线红线 PAY-B1 的域内落点）。
        var api = typeof(IWechatPayPartnerPayScoreService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api).Should().NotBeNull(
            "本域必须绑支付专用命名客户端（wechat-pay），复用默认实例会与企微线撞 BaseUrl");
        typeof(IWechatPayPartnerPayScoreService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token");
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
        => typeof(IWechatPayPartnerPayScoreService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayPartnerPayScoreService.{methodName} 必须存在");

    private static Type ReturnTypeOf(string methodName)
    {
        var taskType = FindMethod(methodName).ReturnType;
        taskType.IsGenericType.Should().BeTrue($"{methodName} 应答必须是 Task<T>");
        return taskType.GetGenericArguments()[0];
    }

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string PathKeyOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .Should()
            .ContainSingle("每支带 path 的端点只有一个定位键")
            .Subject;

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static IEnumerable<Type> DomainTypes()
        => typeof(PartnerPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(PartnerPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName)
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static string[] DeclaredJsonNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] JsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
