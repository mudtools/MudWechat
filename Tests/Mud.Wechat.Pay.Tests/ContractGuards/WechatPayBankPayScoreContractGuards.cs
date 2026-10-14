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
using Mud.Wechat.Pay.DataModels.BankPayScore;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-5 银行 / 从业机构支付分域契约守卫（<b>11 端点</b>：授权 3 + 服务订单 6 + 扣款信息 2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（合作伙伴文档中心「开发文档(银行/机构)」树 ·「从业机构-微信支付分（免确认模式）」，
/// 2026-10-14 逐页核验，各页「支持商户」栏恒为【从业机构（支付机构）】）：
/// 4012691489 从业机构预授权、4012461004 查询用户授权记录（授权协议号）、4012461012 解除用户授权关系、
/// 4012691326 创建支付分订单、4012691343 查询支付分订单、4012458146 取消、4012460791 完结、
/// 4012691363 修改订单金额、4012691378 同步订单状态、4012458414 登记扣款信息、4012458665 查询扣款信息。
/// 逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>SKIT 清单 15 行 ⇒ 本域 11 端点的裁定</b>（守卫 BS-B2 以「零声明 + 正对照」锁定）：
/// ① 「收单曝光」<c>POST /pay/acquiring-bank/transactions/exposure</c>（docId <c>4014326541</c>）
/// 现跳 50x（公益 404）⇒ <b>零端点，<c>Interfaces/BankAcquiring/</c> 不建目录</b>；
/// ② 查询授权记录、查询订单两行在清单里重复声明同一端点，按同端点归并；
/// ③ 侧栏的「拉起支付分小程序授权页 / 订单详情页」是端上形态、
/// 「开启/解除授权服务回调通知 / 商户预下单通知 / 支付成功回调通知」是回调面 ⇒ 均非本接口端点。
/// </para>
/// <para>
/// <b>本守卫要守住的五处「顺手就会写错」</b>（多数源自与相邻两支付分域的形似神异）：
/// ① 本域比服务商面<b>多一层必填 <c>channel_id</c></b>（从业机构 → 渠道商 → 子商户三级），
/// 且<b>请求侧必填、应答侧可能降为选填</b>（授权记录应答）；
/// ② 本域<b>只有一套房</b> —— 单一前缀 <c>/v3/payscore/acquiringbank/</c>，
/// <b>没有</b>支付分计划与用户签约族；③ 只开放<b>免确认模式</b>（<c>need_user_confirm</c> 必填且固定 <c>false</c>），
/// 故 <c>state_description</c> 只有 <c>MCH_COMPLETE</c> 一种拼写，<b>不存在</b>服务商面的
/// <c>MERCHANT_COMPLETE</c>；④ <c>complete_time</c> 走 rfc3339 而同页 <c>time_range</c> 与两支
/// <c>paid_time</c> 走紧凑格式 ⇒ 时间字段恒 <c>string</c>；
/// ⑤ 官方跨页自身不一致：完结页 path 的 <c>out_order_no</c> 写 <c>string(64)</c> 而同族其余三页写
/// <c>string(32)</c>，风险金示例写 <c>DEPOSIT</c> 而字段表规定 <c>ESTIMATE_ORDER_COST</c> —— 照原文，不「统一」。
/// </para>
/// </remarks>
public class WechatPayBankPayScoreContractGuards
{
    private const string RegistryGroupName = "BankPayScore";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BankPayScore";

    private const string BankPrefix = "/v3/payscore/acquiringbank/";

    private const string PartnerPrefix = "/v3/payscore/partner/";

    /// <summary>
    /// BS-B1：11 端点的方法与路由照官方原文；<c>channel_id</c> 的 query / body 位置逐端点锁定，
    /// 五支 204 端点必须返回裸 <see cref="Task"/>。
    /// </summary>
    [Fact]
    public void BankPayScoreEndpoints_ShouldMatchOfficialRoutesAndParameterPositions()
    {
        // 授权族 3 支（本域只开放协议号一支查询与一支解除，服务商面各有两路）。
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.PreAuthorizeAsync),
            BankPrefix + "permissions");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankPayScoreService.QueryAuthorizationRecordAsync),
            BankPrefix + "permissions/authorization-code/{authorizationCode}");
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.TerminateAuthorizationAsync),
            BankPrefix + "permissions/authorization-code/{authorizationCode}/terminate");

        // 服务订单族 6 支。
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.CreateServiceOrderAsync),
            BankPrefix + "serviceorder");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankPayScoreService.QueryServiceOrderAsync),
            BankPrefix + "serviceorder");
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.CancelServiceOrderAsync),
            BankPrefix + "serviceorder/{outOrderNo}/cancel");
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.CompleteServiceOrderAsync),
            BankPrefix + "serviceorder/{outOrderNo}/complete");
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.ModifyServiceOrderAsync),
            BankPrefix + "serviceorder/{outOrderNo}/modify");
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.SyncServiceOrderAsync),
            BankPrefix + "serviceorder/{outOrderNo}/sync");

        // 扣款信息族 2 支：登记走 path，查询走**固定段 /deduction**（不是 {out_order_no}/deduction）。
        AssertRoute<PostAttribute>(nameof(IWechatPayBankPayScoreService.RegisterDeductInfoAsync),
            BankPrefix + "serviceorder/{outOrderNo}/registerdeductinfo");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankPayScoreService.QueryDeductionInfoAsync),
            BankPrefix + "serviceorder/deduction");

        // channel_id 位置：三支 GET 在 query，其余八支在 body（本域请求侧十一端点处处必填）。
        var queryChannelId = new[]
        {
            nameof(IWechatPayBankPayScoreService.QueryAuthorizationRecordAsync),
            nameof(IWechatPayBankPayScoreService.QueryServiceOrderAsync),
            nameof(IWechatPayBankPayScoreService.QueryDeductionInfoAsync),
        };
        foreach (var name in queryChannelId)
        {
            QueryNamesOf(name).Should().Contain("channel_id",
                $"{name} 的 channel_id 官方放在 query：下沉进 body 即 PARAM_ERROR");
        }

        // 三支 GET 的 query 参数集合逐支锁定（官方各页参数集不同形，不得按一个模板套）。
        QueryNamesOf(nameof(IWechatPayBankPayScoreService.QueryAuthorizationRecordAsync))
            .Should().BeEquivalentTo(new[] { "service_id", "channel_id", "sub_mchid" });
        QueryNamesOf(nameof(IWechatPayBankPayScoreService.QueryServiceOrderAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid", "channel_id", "out_order_no", "service_id" });
        QueryNamesOf(nameof(IWechatPayBankPayScoreService.QueryDeductionInfoAsync))
            .Should().BeEquivalentTo(new[] { "service_id", "appid", "sub_mchid", "channel_id", "out_order_no", "sub_appid" },
                "查询扣款信息是六参数全走 query，且 sub_appid 是该支唯一选填项");

        // GET 一律无请求体；两支无 path 的 POST 一律无 query。
        foreach (var method in typeof(IWechatPayBankPayScoreService)
                     .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(static m => m.GetCustomAttribute<GetAttribute>() != null))
        {
            method.GetParameters().Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method.Name} 是 GET，官方无请求体");
        }

        foreach (var name in new[]
                 {
                     nameof(IWechatPayBankPayScoreService.PreAuthorizeAsync),
                     nameof(IWechatPayBankPayScoreService.CreateServiceOrderAsync),
                 })
        {
            var parameters = FindMethod(name).GetParameters();
            parameters.Should().NotContain(static p => p.GetCustomAttribute<PathAttribute>() != null,
                $"{name} 官方无 path 参数");
            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{name} 官方无 query 参数");
        }

        // path 定位键逐支锁定：授权族用 authorization_code，订单与扣款族用 out_order_no。
        PathKeyOf(nameof(IWechatPayBankPayScoreService.QueryAuthorizationRecordAsync))
            .Should().Be("authorizationCode");
        PathKeyOf(nameof(IWechatPayBankPayScoreService.TerminateAuthorizationAsync))
            .Should().Be("authorizationCode");
        foreach (var name in new[]
                 {
                     nameof(IWechatPayBankPayScoreService.CancelServiceOrderAsync),
                     nameof(IWechatPayBankPayScoreService.CompleteServiceOrderAsync),
                     nameof(IWechatPayBankPayScoreService.ModifyServiceOrderAsync),
                     nameof(IWechatPayBankPayScoreService.SyncServiceOrderAsync),
                     nameof(IWechatPayBankPayScoreService.RegisterDeductInfoAsync),
                 })
        {
            PathKeyOf(name).Should().Be("outOrderNo", $"{name} 官方以 out_order_no 定位");
        }

        // 五支 204 端点：官方成功应答「无应答包体」⇒ 返回裸 Task（声明成响应类型即造出永不填充的假字段）。
        NoContentMethodsOf(typeof(IWechatPayBankPayScoreService)).Should().BeEquivalentTo(new[]
        {
            nameof(IWechatPayBankPayScoreService.TerminateAuthorizationAsync),
            nameof(IWechatPayBankPayScoreService.CancelServiceOrderAsync),
            nameof(IWechatPayBankPayScoreService.CompleteServiceOrderAsync),
            nameof(IWechatPayBankPayScoreService.ModifyServiceOrderAsync),
            nameof(IWechatPayBankPayScoreService.SyncServiceOrderAsync),
        }, "本域恰五支 204 端点，其余 6 支均须有应答类型");
    }

    /// <summary>
    /// BS-B2：单一前缀 <c>/v3/payscore/acquiringbank/</c> 的全量归属 + 三处「官方无此端点」零声明
    /// （收单曝光 / 计划与签约族 / 服务商面独有变体），每处均须有相邻正对照防止扫描漂移后静默恒真。
    /// </summary>
    [Fact]
    public void BankPayScoreRoutes_ShouldStayOnSingleAcquiringBankPrefixAndNotDeclareDelistedOrForeignEndpoints()
    {
        var routes = RoutesOf(typeof(IWechatPayBankPayScoreService));

        routes.Should().HaveCount(11, "本域 11 端点（数量变化须同批更新 BS-B1 与 Tests/ContractBaseline.cs）");

        routes.Count(static r => r.StartsWith(BankPrefix, StringComparison.Ordinal))
            .Should().Be(11, "本域十一端点全在单一前缀 /v3/payscore/acquiringbank/ 下（服务商面是三前缀形态，勿照抄）");

        // 零声明①：SKIT 清单的「收单曝光」出处页已下架（4014326541 现跳 50x）⇒ BankAcquiring 目录不建。
        routes.Should().NotContain(static r => r.Contains("exposure", StringComparison.Ordinal),
            "从业机构收单无曝光咨询端点：现行官方目录无该页，补进来即制造不存在的契约");
        // 正对照：同口径扫描下服务商基础支付域确实声明了曝光端点 ⇒ 扫描根与匹配形态有效。
        RoutesOf(typeof(IWechatPayPartnerTransactionsService))
            .Should().Contain(static r => r.Contains("exposure", StringComparison.Ordinal),
                "正对照：曝光能力只存在于服务商分期域，本域的零声明是有据裁决而非漏落");

        // 零声明②：本域无支付分计划 / 用户签约族（那两族只在服务商面存在）。
        routes.Should().NotContain(static r => r.Contains("plan", StringComparison.Ordinal),
            "从业机构面无 payscore-plans / user-sign-plans：照服务商面抄进来即跨面借契约");
        // 正对照：服务商面两族前缀真实存在。
        RoutesOf(typeof(IWechatPayPartnerPayScoreService))
            .Should().Contain(static r => r.Contains("/payscore-plans", StringComparison.Ordinal)
                                          && r.Contains("/user-sign-plans", StringComparison.Ordinal) == false,
                "正对照：服务商面的支付分计划族确实在 /v3/payscore/plan/partner/ 下");

        // 零声明③：服务商面独有的三前缀变体与本域互斥。
        routes.Should().NotContain(static r => r.StartsWith(PartnerPrefix, StringComparison.Ordinal),
            "服务商面前缀不得出现在本域");
        routes.Should().NotContain(static r => r.StartsWith("/v3/payscore/serviceorder", StringComparison.Ordinal),
            "直连面前缀不得出现在本域");
        routes.Should().NotContain(static r => r.EndsWith("/permissions/search", StringComparison.Ordinal),
            "本域授权记录查询只开放协议号一支（服务商面才有 OpenID 检索支）");

        // 三支付分域两两互斥：同一宿主配置不得打到不同契约面。
        var partnerRoutes = RoutesOf(typeof(IWechatPayPartnerPayScoreService));
        var directRoutes = RoutesOf(typeof(IWechatPayPayScoreService));
        routes.Intersect(partnerRoutes).Should().BeEmpty("从业机构面与服务商面路由不得重叠");
        routes.Intersect(directRoutes).Should().BeEmpty("从业机构面与直连面路由不得重叠");

        // Interfaces/BankAcquiring/ 不建目录（裁决：exposure 页已下架）——正对照：相邻目录确在。
        File.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"),
            "Interfaces", "BankAcquiring", "IWechatPayBankAcquiringService.cs"))
            .Should().BeFalse("本线不建银行收单域：官方出处页已下架（方案 §2.5 核验结论）");
        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", "BankPayScore"))
            .Should().BeTrue("正对照：Interfaces/BankPayScore/ 确实已落位，上一条零声明不是因为整目录缺失而恒真");
    }

    /// <summary>
    /// BS-B3：官方字段名逐项锁定（<b>多一层 <c>channel_id</c></b>、无 <c>sp_</c> 前缀）+
    /// 两套时间格式并存下时间字段恒 <c>string</c>。
    /// </summary>
    [Fact]
    public void BankPayScoreDtos_ShouldExposeOfficialFieldNamesAndKeepTimeAsStrings()
    {
        // 商户标识：从业机构侧是 appid / mchid（官方应答标注「[从业机构商户号]」），无 sp_ 前缀。
        JsonNameShouldBe<BankPayScoreCreateOrderRequest>(nameof(BankPayScoreCreateOrderRequest.AppId), "appid");
        JsonNameShouldBe<BankPayScoreCreateOrderRequest>(nameof(BankPayScoreCreateOrderRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<BankPayScoreCreateOrderRequest>(nameof(BankPayScoreCreateOrderRequest.SubAppId), "sub_appid");
        JsonNameShouldBe<BankPayScoreCreateOrderResponse>(nameof(BankPayScoreCreateOrderResponse.MchId), "mchid");
        JsonNameShouldBe<BankPayScoreRegisterDeductRequest>(nameof(BankPayScoreRegisterDeductRequest.OutTradeNo), "out_trade_no");

        // channel_id：本域比服务商面多的一层，八支 body 端点的请求体全部声明该字段。
        JsonNameShouldBe<BankPayScoreCreateOrderRequest>(nameof(BankPayScoreCreateOrderRequest.ChannelId), "channel_id");
        foreach (var requestType in new[]
                 {
                     typeof(BankPayScoreApplyPermissionsRequest),
                     typeof(BankPayScoreTerminatePermissionsRequest),
                     typeof(BankPayScoreCreateOrderRequest),
                     typeof(BankPayScoreCancelOrderRequest),
                     typeof(BankPayScoreCompleteOrderRequest),
                     typeof(BankPayScoreModifyOrderRequest),
                     typeof(BankPayScoreSyncOrderRequest),
                     typeof(BankPayScoreRegisterDeductRequest),
                 })
        {
            JsonNamesOf(requestType).Should().Contain("channel_id",
                $"{requestType.Name} 缺 channel_id 即丢掉本域必填的渠道商层");
        }

        // 正对照：服务商支付分域的 DTO 全域没有 channel_id（除风险评估应答），两域字段面差一层。
        JsonNamesOf<PartnerPayScoreCreateOrderRequest>().Should().NotContain("channel_id",
            "正对照：服务商面创单请求无 channel_id，抄表时不得把本域字段带过去");
        JsonNamesOf<PartnerPayScoreRiskAssessResponse>().Should().Contain("channel_id");

        // 全域扫一遍：本域任何 DTO 都不得出现 sp_ 前缀字段。
        var allJsonNames = DomainTypes()
            .SelectMany(static t => JsonNamesOf(t))
            .ToArray();
        allJsonNames.Should().NotContain(static n => n.StartsWith("sp_", StringComparison.Ordinal),
            "本域官方字段是 appid/mchid，不存在 sp_ 前缀形态");
        allJsonNames.Should().NotBeEmpty("参照集为空会让上面的零声明断言静默恒真");

        // 免确认模式开关 + 扣款族两支的独有字段。
        JsonNameShouldBe<BankPayScoreCreateOrderRequest>(nameof(BankPayScoreCreateOrderRequest.NeedUserConfirm), "need_user_confirm");
        JsonNameShouldBe<BankPayScoreRegisterDeductResponse>(nameof(BankPayScoreRegisterDeductResponse.Description), "description");
        JsonNameShouldBe<BankPayScoreDeductionQueryResponse>(nameof(BankPayScoreDeductionQueryResponse.OutTradeNo), "out_trade_no");
        JsonNameShouldBe<BankPayScoreSyncOrderRequest>(nameof(BankPayScoreSyncOrderRequest.Detail), "detail");

        // 授权记录应答的 channel_id 是选填（请求 query 必填）：形态照本页，不按请求侧推。
        DeclaredJsonNamesOf<BankPayScoreAuthorizationRecordResponse>().Should().Contain("channel_id",
            "官方本页应答表确有 channel_id（选填）：删掉即丢失渠道商回显");

        // 两套时间格式并存 ⇒ 全部时间字段保持 string，不做 DateTimeOffset 往返。
        typeof(BankPayScoreTimeRange).GetProperty(nameof(BankPayScoreTimeRange.StartTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(BankPayScoreSyncDetail).GetProperty(nameof(BankPayScoreSyncDetail.PaidTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(BankPayScoreCollectionDetail).GetProperty(nameof(BankPayScoreCollectionDetail.PaidTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(BankPayScoreCompleteOrderRequest).GetProperty(nameof(BankPayScoreCompleteOrderRequest.CompleteTime))!
            .PropertyType.Should().Be(typeof(string));
        typeof(BankPayScoreAuthorizationRecordResponse)
            .GetProperty(nameof(BankPayScoreAuthorizationRecordResponse.AuthorizationSuccessTime))!
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

    /// <summary>BS-B4：官方枚举取值逐项锁定 + 与服务商面 / 直连面的三处取值分歧（防「顺手复用常量」）。</summary>
    [Fact]
    public void BankPayScoreEnumConstants_ShouldMatchOfficialValuesAndDivergeFromNeighbouringDomains()
    {
        ValuesOf(typeof(BankPayScoreServiceOrderStates)).Should().BeEquivalentTo(new[]
        {
            "CREATED", "DOING", "DONE", "REVOKED", "EXPIRED",
        }, "官方 state 为 5 值，三终态，流转须按官方流转图");

        ValuesOf(typeof(BankPayScoreStateDescriptions)).Should().BeEquivalentTo(new[] { "USER_CONFIRM", "MCH_COMPLETE" },
            "本域只有免确认模式，无签约计划页 ⇒ 不存在服务商面的 MERCHANT_COMPLETE 拼写");

        ValuesOf(typeof(BankPayScoreCollectionStates)).Should().BeEquivalentTo(new[] { "USER_PAYING", "USER_PAID" });

        ValuesOf(typeof(BankPayScoreCollectionPaidTypes)).Should().BeEquivalentTo(new[] { "NEWTON", "MCH" },
            "MCH 只在商户调同步订单状态接口成功后出现，直连面的 ADVANCE/BALANCE 在本域不存在");

        ValuesOf(typeof(BankPayScoreSyncTypes)).Should().BeEquivalentTo(new[] { "Order_Paid" },
            "官方 type 值大小写与下划线混排（Order_Paid），不得驼峰化或全大写");

        ValuesOf(typeof(BankPayScoreRiskFundNames)).Should().BeEquivalentTo(new[] { "ESTIMATE_ORDER_COST" },
            "字段表规定固定传 ESTIMATE_ORDER_COST；官方同页示例报文里的 DEPOSIT 是示例残留，不建模");

        ValuesOf(typeof(BankPayScoreStartTimeKeywords)).Should().BeEquivalentTo(new[] { "OnAccept" });

        ValuesOf(typeof(BankPayScoreAuthorizationStates)).Should().BeEquivalentTo(new[]
        {
            "UNBINDUSER", "AVAILABLE", "UNAVAILABLE",
        }, "UNBINDUSER 是官方自身的不规则拼写（不是 UNBIND_USER），且预授权成功仅到该态");

        ValuesOf(typeof(BankPayScoreUserLabels)).Should().BeEquivalentTo(new[] { "A", "B", "C" },
            "官方枚举值三枚；其比较说明在官方页面正文里即被截断，故本仓不承诺排序语义");

        ValuesOf(typeof(BankPayScorePromotionScopes)).Should().BeEquivalentTo(new[] { "GLOBAL", "SINGLE" });
        ValuesOf(typeof(BankPayScorePromotionTypes)).Should().BeEquivalentTo(new[] { "CASH", "NOCASH" });

        // 与相邻两域的分歧（正对照）：证明三域常量各自独立，没有「顺手复用」。
        ValuesOf(typeof(PartnerPayScoreStateDescriptions)).Should().Contain("MERCHANT_COMPLETE",
            "正对照：服务商面确有第二种拼写，本域不得据此扩值集");
        ValuesOf(typeof(PayScoreCollectionPaidTypes)).Should().BeEquivalentTo(new[] { "NEWTON", "ADVANCE", "BALANCE" });
        ValuesOf(typeof(PartnerPayScoreCollectionPaidTypes)).Should().BeEquivalentTo(new[] { "NEWTON", "MCH" });

        // 常量类清单锁定：本域恰 11 个 static class（值集差异的权威清单），且全部不带 [HttpJsonSerializable]。
        ConstantClassNames().Should().BeEquivalentTo(new[]
        {
            nameof(BankPayScoreServiceOrderStates),
            nameof(BankPayScoreStateDescriptions),
            nameof(BankPayScoreCollectionStates),
            nameof(BankPayScoreCollectionPaidTypes),
            nameof(BankPayScoreSyncTypes),
            nameof(BankPayScoreRiskFundNames),
            nameof(BankPayScoreStartTimeKeywords),
            nameof(BankPayScoreAuthorizationStates),
            nameof(BankPayScoreUserLabels),
            nameof(BankPayScorePromotionScopes),
            nameof(BankPayScorePromotionTypes),
        }, "本域常量类恰 11 个");
    }

    /// <summary>
    /// BS-B5：本域<b>不建专有错误码常量类</b>的裁决锁定（官方各页业务码全是通用名）+ 正对照。
    /// </summary>
    /// <remarks>
    /// 「无授权资格」一支在本域<b>十一页统一写 <c>NO_AUTH</c>（有下划线）</b>（2026-10-14 逐页核验），
    /// 与服务商面「计划族与签约族写 <c>NOAUTH</c>」的形态相反 ⇒ 本域判错可以直接用共享常量
    /// <c>WechatPayErrorCodes.NoAuth</c>；但 <c>INVALID_ORDER_STATE</c>/<c>ORDER_CANCELED</c>/
    /// <c>ORDER_DONE</c>/<c>ALREADY_EXIST</c> 四码<b>不在共享表内</b>（<c>ALREADY_EXIST</c> 只在登记扣款信息页出现），
    /// 故宿主判错仍须以 <see cref="WechatPayResponse.Code"/> 原样字符串为准。
    /// </remarks>
    [Fact]
    public void BankPayScore_ShouldNotDeclareDomainSpecificErrorCodes()
    {
        DomainTypeNames().Should().NotContain(static n => n.EndsWith("ErrorCodes", StringComparison.Ordinal),
            "本域官方错误码全为 APIv3 通用名：建专有码常量类即制造「能识别本域专有错误」的假象");

        // 正对照：进件域确有专有码，两域判错面不同。
        ValuesOf(typeof(Apply4SubMerchantErrorCodes)).Should().Contain("APPLYMENT_NOT_EXIST");

        // 通用码由既有承载面覆盖（本域官方用 NO_AUTH，恰与共享表同形）。
        var commonCodes = ValuesOf(typeof(Mud.Wechat.Pay.Abstractions.Enums.WechatPayErrorCodes));
        commonCodes.Should().Contain(new[] { "NO_AUTH", "INVALID_REQUEST", "SYSTEM_ERROR", "FREQUENCY_LIMITED", "ORDER_NOT_EXIST" },
            "本域官方业务码全为通用名，既有承载面已覆盖");
        commonCodes.Should().NotContain(new[] { "INVALID_ORDER_STATE", "ORDER_CANCELED", "ORDER_DONE", "ALREADY_EXIST" },
            "这四码不在共享表内：判错须按 WechatPayResponse.Code 原样字符串，勿据常量表假定可枚举");
    }

    /// <summary>
    /// BS-B6：判错面继承 + 「表相同则共用、表不同则分建」的四处共用与四处不共用。
    /// </summary>
    [Fact]
    public void BankPayScoreResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch()
    {
        foreach (var responseType in new[]
                 {
                     typeof(BankPayScoreApplyPermissionsResponse),
                     typeof(BankPayScoreAuthorizationRecordResponse),
                     typeof(BankPayScoreCreateOrderResponse),
                     typeof(BankPayScoreQueryOrderResponse),
                     typeof(BankPayScoreRegisterDeductResponse),
                     typeof(BankPayScoreDeductionQueryResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse：挂在兄弟应答之下即继承其字段表");
        }

        // 共用①：后付费 / 优惠 / 风险金 / 时间段 / 位置五节在创单、完结、修改、应答四面重复出现 ⇒ 共享构件类型。
        JsonNamesOf<BankPayScorePostPayment>().Should().BeEquivalentTo(new[] { "name", "amount", "description", "count" });
        BodyTypeOf(nameof(IWechatPayBankPayScoreService.CompleteServiceOrderAsync))
            .GetProperty("PostPayments")!.PropertyType
            .Should().Be(BodyTypeOf(nameof(IWechatPayBankPayScoreService.CreateServiceOrderAsync))
                .GetProperty("PostPayments")!.PropertyType,
            "完结与创单的后付费节字段表相同，共用 BankPayScorePostPayment");

        // 共用②：创单与查询应答的 collection / promotion 构件同源 ⇒ 查询应答引用的类型即本域声明的那一个。
        typeof(BankPayScoreQueryOrderResponse).GetProperty(nameof(BankPayScoreQueryOrderResponse.Collection))!
            .PropertyType.Should().Be(typeof(BankPayScoreCollection));
        typeof(BankPayScoreCollection).GetProperty(nameof(BankPayScoreCollection.Details))!
            .PropertyType.GetGenericArguments()[0].Should().Be(typeof(BankPayScoreCollectionDetail));

        // 不共用①：创单应答有 package（拉起确认页凭据）而无 total_amount / collection；查单应答反之。
        DeclaredJsonNamesOf<BankPayScoreCreateOrderResponse>().Should().Contain("package");
        DeclaredJsonNamesOf<BankPayScoreCreateOrderResponse>().Should().NotContain(new[] { "total_amount", "collection", "openid" });
        DeclaredJsonNamesOf<BankPayScoreQueryOrderResponse>().Should().NotContain("package",
            "官方查询页应答表无 package：与创单应答合并类型即制造「查得到 package」的假象");
        DeclaredJsonNamesOf<BankPayScoreQueryOrderResponse>().Should().Contain(new[] { "total_amount", "collection", "openid", "sub_openid", "promotion_detail" });

        // 不共用②：本域查询应答**无 need_collection**（服务商面有且固定 true）⇒ 与服务商域类型不可互换。
        DeclaredJsonNamesOf<BankPayScoreQueryOrderResponse>().Should().NotContain("need_collection",
            "从业机构面官方应答表没有该字段：从服务商面抄过来即多出一个恒缺省字段");
        DeclaredJsonNamesOf<PartnerPayScoreQueryOrderResponse>().Should().Contain("need_collection",
            "正对照：服务商面查询应答确有 need_collection");

        // 不共用③：登记扣款应答只有 description，查询扣款应答两字段 ⇒ 分建两类型。
        DeclaredJsonNamesOf<BankPayScoreRegisterDeductResponse>().Should().BeEquivalentTo(new[] { "description" });
        DeclaredJsonNamesOf<BankPayScoreDeductionQueryResponse>().Should().BeEquivalentTo(new[] { "description", "out_trade_no" });

        // 不共用④：同步的 detail.paid_time（string(64)，两种紧凑格式）与收款明细的 paid_time（string(32)）
        // 官方宽度与语义均不同 ⇒ 两个类型不合并。
        BodyTypeOf(nameof(IWechatPayBankPayScoreService.SyncServiceOrderAsync))
            .GetProperty("Detail")!.PropertyType.Should().Be(typeof(BankPayScoreSyncDetail));
        typeof(BankPayScoreSyncDetail).Should().NotBeSameAs(typeof(BankPayScoreCollectionDetail),
            "同步入参只有一个 paid_time，收款明细另有 seq/amount/paid_type/transaction_id");

        // 两支预授权与授权记录：本域 token 字段与服务商面同名但应答面更窄（恰一字段）。
        DeclaredJsonNamesOf<BankPayScoreApplyPermissionsResponse>().Should().BeEquivalentTo(new[] { "apply_permissions_token" });
    }

    /// <summary>
    /// BS-B7：本域 24 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void BankPayScoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(24,
            "从业机构支付分 DTO：共享构件 6 + 授权族 4 + 服务订单请求 6 + 服务订单应答 8");

        foreach (var type in domainTypes)
        {
            BankPayScoreJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BankPayScoreJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（否则源生成上下文里出现无意义节点）：本域类型 = 24 DTO + 11 常量类。
        DomainTypeNames().Should().HaveCount(35, "本域类型 = 24 DTO + 11 常量类（上下文类已排除）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("BankPayScoreJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        // 三段式：枚举成员 + 建造者注册委托 + 源生成注册方法名。
        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BankPayScore 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBankPayScoreWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        // 域内形态：组名 + 支付专用客户端 + 零 [Token]（本线红线 PAY-B1 的域内落点）。
        var api = typeof(IWechatPayBankPayScoreService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api).Should().NotBeNull(
            "本域必须绑支付专用命名客户端（wechat-pay），复用默认实例会与企微线撞 BaseUrl");
        typeof(IWechatPayBankPayScoreService)
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
        => typeof(IWechatPayBankPayScoreService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayBankPayScoreService.{methodName} 必须存在");

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

    private static string[] NoContentMethodsOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static IEnumerable<Type> DomainTypes()
        => typeof(BankPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(BankPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(BankPayScoreCreateOrderRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
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
