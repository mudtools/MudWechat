// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线 P1-c 契约守卫（方案 §3.6 MP-X1 / X2 / X3 / X5 / X6 / X7 / X8 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何逐条锁死</b>：小程序线与公众号线<b>共用同一令牌域与同一批路由前缀</b>（<c>/wxa/</c> 等），
/// 最容易出的事故是「重复声明公众号已有路由」（v1 曾据此列出 18 条重复项）与
/// 「新增令牌类型导致 errcode 自愈静默失效」。两者都是<b>编译通过、运行期静默降级</b>的形态。
/// </para>
/// </remarks>
public class MiniProgramContractGuards
{
    /// <summary>
    /// MP-X1：跨包（公众号 ↔ 小程序）<b>全局路由字符串无重复</b>。
    /// </summary>
    /// <remarks>
    /// 参照集 = 公众号<b>主包</b>全部 <c>[RequestUri]</c>（<c>RequestUri</c> 落在方法级特性上，
    /// 故必须逐程序集反射，不可用传递性 <c>GetInterfaces()</c>）；
    /// 小程序侧 = 生成接口的路由特性 <b>＋</b> <c>IWxaCodeService</c> 手工通道的路由常量
    /// （后者不在特性上，是唯一的例外来源）。
    /// 参照集须<b>非空</b>：为空说明反射口径失效，守卫会静默假绿。
    /// </remarks>
    [Fact]
    public void Routes_ShouldNotOverlapAcrossProductLines()
    {
        var oaRoutes = CollectAttributeRoutes(typeof(Mud.Wechat.OfficialAccount.Extensions.MpServiceCollectionExtensions).Assembly);
        oaRoutes.Should().NotBeEmpty("公众号线路由是本守卫的参照集，为空说明反射口径失效");

        var mpRoutes = CollectAttributeRoutes(typeof(MiniProgramServiceBuilder).Assembly)
            .Concat(ManualChannelRoutes())
            .ToArray();

        mpRoutes.Should().NotBeEmpty("小程序线路由非空（303 特性路由 + 6 手工通道路由）");

        var overlap = mpRoutes.Intersect(oaRoutes, StringComparer.Ordinal).ToArray();
        overlap.Should().BeEmpty(
            "小程序线不得重复声明公众号线已有路由（方案 §3.6 MP-X1）：" + string.Join(", ", overlap));
    }

    /// <summary>
    /// MP-X6：v1 判定为「与公众号重复」的高危路由<b>零回潮</b>（MP-X1 的定点强化）。
    /// </summary>
    /// <remarks>定点断言 v1 清单里最容易「顺手再写一遍」的六条：令牌签发、网页授权、带参二维码、公众号统计。</remarks>
    [Theory]
    [InlineData("/cgi-bin/token")]
    [InlineData("/cgi-bin/stable_token")]
    [InlineData("/sns/userinfo")]
    [InlineData("/cgi-bin/qrcode/create")]
    [InlineData("/datacube/getusercumulate")]
    [InlineData("/datacube/getusersummary")]
    public void Routes_ShouldNotReintroduceOfficialAccountRoutes(string officialAccountRoute)
    {
        var mpRoutes = CollectAttributeRoutes(typeof(MiniProgramServiceBuilder).Assembly)
            .Concat(ManualChannelRoutes())
            .ToArray();

        mpRoutes.Should().NotContain(officialAccountRoute,
            "该路由已在公众号线落地，小程序线重复声明即制造双份维护（MP-X6）");
    }

    /// <summary>
    /// MP-X2：令牌形态 —— 全部 <c>[Token]</c> 接口复用公众号 <c>access_token</c>（不新增令牌类型）。
    /// </summary>
    [Fact]
    public void TokenInterfaces_ShouldReuseMpAccessToken()
    {
        MpTokenTypes.AccessToken.Should().Be("Wechat.Mp.AccessToken");

        var tokenInterfaces = typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false).Any(static a => a.GetType().Name == "TokenAttribute"))
            .ToArray();

        tokenInterfaces.Should().HaveCount(55,
            "带令牌接口 = Auth / QrCodeLink / Security / DataAnalysis / SubscribeMessage / DynamicMessage / Kf / " +
            "HardwareDevice / Operation / Plugin / Charge / NearbyPoi / Search / Soter / ServiceMarket / " +
            "RedPacketCover / Student / FaceVerify / LaborUse / ExpressBusiness / ExpressDelivery / ExpressIntracity / " +
            "ExpressInsuranceFreight / ExpressLocal / Vip / TradeGuarantee / TradeComment / TradeComplaint / " +
            "LiveRoom / LiveAssistant / LiveRoomGoods / LiveGoods / LiveRole / LiveSubscribe / " +
            "VodUpload / VodMedia / VodDrama / VodCdn / VodAuthorization / " +
            "XPayCurrency / XPayOrder / XPayGoods / XPaySubscribe / XPayFunds / XPayComplaint / " +
            "OrderShipping / OrderQuery / OrderSetting / FamousBrand / " +
            "NovelBook / NovelChapter / NovelAuth / NovelAccountAuth / NovelReader / RealName" +
            "（登录 code2Session 为免令牌独立接口；图片通道 Code / FeedbackMedia " +
            "与对账文件通道 Bill 为手工服务不走 [Token]；Vip 域为 2026-10 小店线设计方案 §4.1 结论 A 划入；" +
            "ExpressInsuranceFreight / ExpressLocal 为 2026-10-12 依官方清单补齐的无忧退货族与即时配送族；" +
            "TradeGuarantee / TradeComment / TradeComplaint 为 2026-10-13 依官方 transaction-guarantee 分区补齐的交易保障域三接口；" +
            "Live 族六接口为 2026-10-13 依官方 livebroadcast 分区 38 页取证补齐，同批解除 P3-R1 第 2 项「不实现」裁决；" +
            "Vod 族五接口为 2026-10-13 依官方 minidrama 分区 31 页取证补齐的短剧媒资域；" +
            "XPay 族六接口为 2026-10-13 依官方 VirtualPayment 分区 37 页取证补齐的虚拟支付域" +
            "（pay_sig / signature 属宿主计算的支付签名，落方法级 [Query] 参数而非 [Token]，见守卫 XP2 / XP6）；" +
            "OrderShipping 族四接口为 2026-10-13 依官方 order_shipping 10 页 + famousbrand 2 页取证补齐的" +
            "发货信息管理与品牌申请域，本域凭据面只有 access_token 一支、无方法级 [Query] 参数（见守卫 OS3）；" +
            "Novel 族五接口为 2026-10-13 依官方 novelreadersetrecmdnovel 分区 23 页取证补齐的小说作品管理域，权限集 169）");

        foreach (var iface in tokenInterfaces)
        {
            var token = iface.GetCustomAttributes(false).Single(static a => a.GetType().Name == "TokenAttribute");
            var type = token.GetType();

            type.GetProperty("TokenType")!.GetValue(token).Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号令牌类型（新增类型会让单槽 Resolve 返回 null ⇒ 自愈静默失效）");
            type.GetProperty("InjectionMode")!.GetValue(token)!.ToString().Should().Be("Query",
                $"{iface.Name} 官方契约强制 Query 注入");
        }

        // 不得出现小程序自有令牌类型常量类。
        typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsClass && t.IsPublic && t.Name.EndsWith("TokenTypes", StringComparison.Ordinal))
            .Should().BeEmpty("小程序线不得自建令牌类型常量");
    }

    /// <summary>MP-X3：<c>[Token].Name</c> 恒为官方契约的 <c>access_token</c>。</summary>
    [Fact]
    public void TokenInterfaces_ShouldDeclareOfficialParameterName()
    {
        foreach (var iface in typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
                     .Where(static t => t.IsInterface && t.IsPublic)
                     .Where(static t => t.GetCustomAttributes(false).Any(static a => a.GetType().Name == "TokenAttribute")))
        {
            var token = iface.GetCustomAttributes(false).Single(static a => a.GetType().Name == "TokenAttribute");
            token.GetType().GetProperty("Name")!.GetValue(token).Should().Be("access_token");
        }
    }

    /// <summary>
    /// MP-X5：<b>端点计数 309</b> + 逐域路由表 + 字段名照官方原文（<c>session_key</c> / <c>js_code</c> /
    /// <c>trace_id</c> / <c>page_url</c> 等<b>不得驼峰化</b>）。
    /// </summary>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutesAndCount()
    {
        var asm = typeof(MiniProgramServiceBuilder).Assembly;

        RoutesOf(asm, nameof(IWxaAuthService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/checksession", "/wxa/resetusersessionkey",
            "/wxa/business/getuserphonenumber", "/wxa/getpaidunionid",
            "/wxa/getpluginopenpid", "/wxa/business/checkencryptedmsg",
            "/wxa/business/getuserencryptkey",
        });

        RoutesOf(asm, nameof(IWxaCode2SessionService)).Should().BeEquivalentTo(new[] { "/sns/jscode2session" });

        RoutesOf(asm, nameof(IWxaQrCodeLinkService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/generate_urllink", "/wxa/query_urllink",
            "/wxa/generatescheme", "/wxa/queryscheme", "/wxa/genwxashortlink",
            "/wxa/generatenfcscheme",
        });

        RoutesOf(asm, nameof(IWxaSecurityService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/msg_sec_check", "/wxa/media_check_async", "/wxa/getuserriskrank",
        });

        RoutesOf(asm, nameof(IWxaDataAnalysisService)).Should().BeEquivalentTo(new[]
        {
            "/datacube/getweanalysisappiddailyvisittrend",
            "/datacube/getweanalysisappidweeklyvisittrend",
            "/datacube/getweanalysisappidmonthlyvisittrend",
            "/datacube/getweanalysisappiddailyretaininfo",
            "/datacube/getweanalysisappidweeklyretaininfo",
            "/datacube/getweanalysisappidmonthlyretaininfo",
            "/datacube/getweanalysisappiduserportrait",
            "/datacube/getweanalysisappidvisitdistribution",
            "/datacube/getweanalysisappidvisitpage",
            "/datacube/getweanalysisappiddailysummarytrend",
            "/wxa/business/performance/boot",
        });

        RoutesOf(asm, nameof(IWxaSubscribeMessageService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/message/subscribe/send",
            "/wxa/set_user_notify", "/wxa/set_user_notifyext", "/wxa/get_user_notify",
        });

        RoutesOf(asm, nameof(IWxaDynamicMessageService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/message/wxopen/activityid/create",
            "/cgi-bin/message/wxopen/updatablemsg/send",
            "/cgi-bin/message/wxopen/chattoolmsg/send",
        });

        RoutesOf(asm, nameof(IWxaKfService)).Should().BeEquivalentTo(new[]
        {
            "/customservice/kfaccount/setadmin", "/customservice/kfaccount/canceladmin",
            "/cgi-bin/business/register", "/cgi-bin/business/update",
            "/cgi-bin/business/get", "/cgi-bin/business/list",
            "/customservice/work/get", "/customservice/work/bind", "/customservice/work/unbind",
            "/cgi-bin/message/custom/business/typing",
        });

        RoutesOf(asm, nameof(IWxaHardwareDeviceService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/message/device/subscribe/send",
            "/wxa/getsnticket", "/wxa/business/group/createid", "/wxa/business/group/getinfo",
            "/wxa/business/group/adddevice", "/wxa/business/group/removedevice",
            "/wxa/business/license/getpkglist", "/wxa/business/license/activedevice",
            "/wxa/business/license/getdeviceinfo",
        });

        RoutesOf(asm, nameof(IWxaOperationService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/getwxadevinfo", "/wxaapi/log/get_performance", "/wxaapi/log/get_scene",
            "/wxaapi/log/get_client_version", "/wxaapi/userlog/userlog_search",
            "/wxaapi/feedback/list", "/wxaapi/log/jserr_detail", "/wxaapi/log/jserr_list",
            "/wxa/getgrayreleaseplan",
        });

        RoutesOf(asm, nameof(IWxaPluginService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/devplugin", "/wxa/plugin",
        });

        RoutesOf(asm, nameof(IWxaChargeService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/charge/usage/get", "/wxa/charge/usage/get_recent_average",
        });

        RoutesOf(asm, nameof(IWxaNearbyPoiService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/addnearbypoi", "/wxa/delnearbypoi",
            "/wxa/getnearbypoilist", "/wxa/setnearbypoishowstatus",
        });

        RoutesOf(asm, nameof(IWxaSearchService)).Should().BeEquivalentTo(new[] { "/wxa/search/wxaapi_submitpages" });
        RoutesOf(asm, nameof(IWxaSoterService)).Should().BeEquivalentTo(new[] { "/cgi-bin/soter/verify_signature" });

        RoutesOf(asm, nameof(IWxaServiceMarketService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/servicemarket", "/wxa/servicemarketretrieve",
        });

        RoutesOf(asm, nameof(IWxaRedPacketCoverService)).Should().BeEquivalentTo(new[]
        {
            "/redpacketcover/wxapp/cover_url/get_by_token",
        });

        RoutesOf(asm, nameof(IWxaStudentService)).Should().BeEquivalentTo(new[] { "/intp/quickcheckstudentidentity" });

        RoutesOf(asm, nameof(IWxaFaceVerifyService)).Should().BeEquivalentTo(new[]
        {
            "/cityservice/face/identify/getverifyid", "/cityservice/face/identify/queryverifyinfo",
        });

        RoutesOf(asm, nameof(IWxaLaborUseService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/message/wxopen/employeerelationmsg/send",
            "/wxa/business/unbinduserb2cauthinfo",
        });

        // 物流助手 · 商家寄件（12）
        RoutesOf(asm, nameof(IWxaExpressBusinessService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/express/business/order/add", "/cgi-bin/express/business/order/get",
            "/cgi-bin/express/business/order/batchget", "/cgi-bin/express/business/order/cancel",
            "/cgi-bin/express/business/path/get", "/cgi-bin/express/business/delivery/getall",
            "/cgi-bin/express/business/account/getall", "/cgi-bin/express/business/account/bind",
            "/cgi-bin/express/business/printer/getall", "/cgi-bin/express/business/printer/update",
            "/cgi-bin/express/business/quota/get", "/cgi-bin/express/business/test_update_order",
        });

        // 物流助手 · 服务商 / 物流消息 / 退货 / 散单寄件 / 消息推送（28，get_bill 为手动通道不计特性路由）
        RoutesOf(asm, nameof(IWxaExpressDeliveryService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/express/delivery/contact/get", "/cgi-bin/express/delivery/template/preview",
            "/cgi-bin/express/delivery/service/business/update", "/cgi-bin/express/delivery/path/update",
            "/cgi-bin/express/delivery/no_worry_return/add", "/cgi-bin/express/delivery/no_worry_return/get",
            "/cgi-bin/express/delivery/no_worry_return/unbind", "/cgi-bin/express/delivery/open_msg/follow_waybill",
            "/cgi-bin/express/delivery/open_msg/query_follow_trace", "/cgi-bin/express/delivery/open_msg/update_follow_waybill_goods",
            "/cgi-bin/express/delivery/open_msg/open_openmsg", "/cgi-bin/express/delivery/open_msg/open_query_plugin",
            "/cgi-bin/express/delivery/open_msg/trace_waybill", "/cgi-bin/express/delivery/open_msg/query_trace",
            "/cgi-bin/express/delivery/open_msg/update_waybill_goods", "/cgi-bin/express/delivery/open_msg/get_delivery_list",
            "/cgi-bin/express/delivery/return/open_return", "/cgi-bin/express/delivery/return/add",
            "/cgi-bin/express/delivery/return/get", "/cgi-bin/express/delivery/return/unbind",
            "/cgi-bin/express/delivery/single_waybill/update", "/cgi-bin/express/delivery/single_waybill/fee",
            "/cgi-bin/express/delivery/single_waybill/cancel_order", "/cgi-bin/express/delivery/single_waybill/refund_order",
            "/cgi-bin/express/delivery/single_waybill/evaluate_feedback",
            "/cgi-bin/express/delivery/scatter/update_complaint_result",
            "/cgi-bin/express/delivery/pathnotify", "/cgi-bin/express/delivery/userquery",
        });

        // 物流助手 · 同城配送（16）
        RoutesOf(asm, nameof(IWxaExpressIntracityService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/express/intracity/apply", "/cgi-bin/express/intracity/createstore",
            "/cgi-bin/express/intracity/querystore", "/cgi-bin/express/intracity/updatestore",
            "/cgi-bin/express/intracity/storecharge", "/cgi-bin/express/intracity/storerefund",
            "/cgi-bin/express/intracity/queryflow", "/cgi-bin/express/intracity/balancequery",
            "/cgi-bin/express/intracity/preaddorder", "/cgi-bin/express/intracity/addorder",
            "/cgi-bin/express/intracity/queryorder", "/cgi-bin/express/intracity/cancelorder",
            "/cgi-bin/express/intracity/mocknotify",
            "/cgi-bin/express/intracity/getcity", "/cgi-bin/express/intracity/getpaymode",
            "/cgi-bin/express/intracity/setpaymode",
        });

        // 微信物流服务 · 无忧退货（11；open / query_open 官方为 GET 且无请求体，refund 为 POST 且无请求体）
        RoutesOf(asm, nameof(IWxaExpressInsuranceFreightService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/business/insurance_freight/open", "/wxa/business/insurance_freight/query_open",
            "/wxa/business/insurance_freight/createorder", "/wxa/business/insurance_freight/claim",
            "/wxa/business/insurance_freight/createchargeid", "/wxa/business/insurance_freight/applypay",
            "/wxa/business/insurance_freight/getpayorderlist", "/wxa/business/insurance_freight/refund",
            "/wxa/business/insurance_freight/getsummary", "/wxa/business/insurance_freight/getorderlist",
            "/wxa/business/insurance_freight/update_notify_funds",
        });

        // 即时配送（15：小程序方 14 + 运力方 1；getall / open / shop/get 官方为无请求体的 POST）
        RoutesOf(asm, nameof(IWxaExpressLocalService)).Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/express/local/business/delivery/getall", "/cgi-bin/express/local/business/open",
            "/cgi-bin/express/local/business/shop/add", "/cgi-bin/express/local/business/shop/get",
            "/cgi-bin/express/local/business/order/pre_add", "/cgi-bin/express/local/business/order/add",
            "/cgi-bin/express/local/business/order/readd", "/cgi-bin/express/local/business/order/get",
            "/cgi-bin/express/local/business/order/cancel", "/cgi-bin/express/local/business/order/precancel",
            "/cgi-bin/express/local/business/order/confirm_return", "/cgi-bin/express/local/business/order/addtips",
            "/cgi-bin/express/local/business/test_update_order",
            "/cgi-bin/express/local/business/realmock_update_order",
            "/cgi-bin/express/local/delivery/update_order",
        });

        ManualChannelRoutes().Should().BeEquivalentTo(new[]
        {
            WxaCodeService.UnlimitedCodePath, WxaCodeService.CodePath, WxaCodeService.QrCodePath,
            WxaFeedbackMediaService.FeedbackMediaPath,
            WxaExpressBillService.BillPath,
            WxaKfMediaService.MediaPath,
        });

        // 合计计数（MP-X5 的单一来源）：323 特性路由 + 6 手工通道路由 = 329（基线见 Tests/ContractBaseline.cs）。
        (CollectAttributeRoutes(asm).Length + ManualChannelRoutes().Length)
            .Should().Be(Baseline.MiniProgram.Endpoints,
                "小程序线合计 329 端点（Auth 8 + QrCodeLink 9 + Security 3 + DataAnalysis 11 + "
                + "SubscribeMessage 4 + DynamicMessage 3 + Kf 11 + HardwareDevice 9 + Operation 10 + "
                + "Plugin 2 + Charge 2 + NearbyPoi 4 + Search 1 + Soter 1 + ServiceMarket 2 + "
                + "RedPacketCover 1 + Student 1 + FaceVerify 2 + LaborUse 2 + ExpressBusiness 12 + "
                + "ExpressDelivery 28 + ExpressIntracity 16 + ExpressInsuranceFreight 11 + ExpressLocal 15 + "
                + "ExpressBill 1 + Vip 6 + TradeGuarantee 2 + TradeComment 9 + TradeComplaint 5 + "
                + "Live 38 + Vod 31 + XPay 37 + OrderShipping 12 + Novel 23 + RealName 1 + Game 20）；"
                + "数量变化须同批调整 Tests/ContractBaseline.cs");
    }

    /// <summary>MP-X5（字段名照官方原文）。</summary>
    [Fact]
    public void Dtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<WxaCode2SessionResponse>(nameof(WxaCode2SessionResponse.SessionKey), "session_key");
        JsonNameShouldBe<WxaPhoneInfo>(nameof(WxaPhoneInfo.PurePhoneNumber), "purePhoneNumber");

        JsonNameShouldBe<WxaShortLinkRequest>(nameof(WxaShortLinkRequest.PageUrl), "page_url");
        JsonNameShouldBe<WxaUrlLinkResponse>(nameof(WxaUrlLinkResponse.UrlLink), "url_link");
        JsonNameShouldBe<WxaSchemeResponse>(nameof(WxaSchemeResponse.OpenLink), "openlink");
        JsonNameShouldBe<WxaShortLinkResponse>(nameof(WxaShortLinkResponse.Link), "link");

        JsonNameShouldBe<WxaMediaCheckAsyncRequest>(nameof(WxaMediaCheckAsyncRequest.MediaUrl), "media_url");
        JsonNameShouldBe<WxaMsgSecCheckResponse>(nameof(WxaMsgSecCheckResponse.TraceId), "trace_id");

        JsonNameShouldBe<WxaDateRangeRequest>(nameof(WxaDateRangeRequest.BeginDate), "begin_date");
        JsonNameShouldBe<WxaVisitDistributionItem>(nameof(WxaVisitDistributionItem.ItemList), "item_list");
        JsonNameShouldBe<WxaVisitPageItem>(nameof(WxaVisitPageItem.PageVisitPv), "page_visit_pv");

        // 官方 URL Link 示例里的 `cloud_base.doamin` 拼写不动；本线不建模该示例专属字段。
        JsonNameShouldBe<WxaCodeUnlimitRequest>(nameof(WxaCodeUnlimitRequest.Scene), "scene");

        // 登录端点的 Query 参数名是官方契约（js_code），不得驼峰化。
        var jsCodeParameter = typeof(IWxaCode2SessionService)
            .GetMethod(nameof(IWxaCode2SessionService.Code2SessionAsync))!
            .GetParameters()
            .Single(static p => p.Name == "jsCode");
        QueryNameOf(jsCodeParameter).Should().Be("js_code");
    }

    /// <summary>
    /// MP-X7：<c>session_key</c> 与手机号 <c>code</c> <b>不入日志</b>（与 <c>DecryptedXml</c> 同级红线）。
    /// </summary>
    /// <remarks>
    /// 机械判定：① 源文件中任何日志调用行<b>不得</b>出现 <c>SessionKey</c>；
    /// ② 承载会话密钥的 DTO 不得被当成「可打印对象」——不重写 <c>ToString</c>（避免有人
    /// <c>Log("{@Dto}")</c> 把密钥结构化打出）。
    /// </remarks>
    [Fact]
    public void SessionKey_ShouldNeverBeLogged()
    {
        var root = SourcePath("Mud.Wechat.MiniProgram");
        var offenders = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .SelectMany(static f => File.ReadAllLines(f)
                .Select((line, index) => (File: f, Line: line, No: index + 1)))
            .Where(static e => e.Line.Contains("Log", StringComparison.Ordinal)
                               && e.Line.Contains("SessionKey", StringComparison.Ordinal))
            .Select(static e => $"{Path.GetFileName(e.File)}:{e.No}")
            .ToArray();

        offenders.Should().BeEmpty("session_key 是用户会话密钥，不得出现在任何日志调用中（MP-X7）");

        typeof(WxaCode2SessionResponse).GetMethod("ToString", Type.EmptyTypes)!.DeclaringType
            .Should().Be(typeof(object), "会话密钥 DTO 不得重写 ToString（防被结构化日志整体打印）");
    }

    /// <summary>
    /// MP-X8：SSRF 白名单<b>小程序线零改动</b> —— 小程序沿用 <c>api.weixin.qq.com</c>，
    /// 已被 <c>weixin.qq.com</c> 后缀覆盖 ⇒ 既不得新增/修改条目，也不得自行调用
    /// <c>ConfigureAllowedDomains</c>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>断言形态已从「白名单数组恰等于两条」改为「后缀覆盖 + 无小程序专属条目」</b>（2026-10-10，
    /// 广告线接入时同批调整）。原写法把<b>进程级全局</b>白名单的完整快照钉在一条<b>产品线</b>守卫里，
    /// 于是任何其它产品线合法地追加后缀域（广告线 <c>e.qq.com</c>）都会把小程序守卫打红 ——
    /// 那是<b>跨线耦合的假红</b>，不是小程序线的契约漂移。守卫要锁的是本线的不变式
    /// （默认域名被覆盖、且本线未往全局数组里塞东西），而非全局数组的长度。
    /// </para>
    /// <para>全局白名单的变更评审面由公用层 AB-G9 与广告线 ADS-B4 承担（那才是它的单点归属）。</para>
    /// </remarks>
    [Fact]
    public void SsrfWhitelist_ShouldRemainUnchanged()
    {
        var allowed = WechatApiHosts.AllowedBaseUrlDomains;

        allowed.Should().Contain("weixin.qq.com",
            "小程序默认域名 api.weixin.qq.com 的放行完全依赖该后缀项");

        foreach (var own in new[] { "api.weixin.qq.com", "mp.weixin.qq.com" })
        {
            allowed.Should().NotContain(own,
                $"{own} 已被 weixin.qq.com 后缀覆盖，小程序线不得为其新增独立条目（新增即扩大进程级放行面）");
        }

        var root = SourcePath("Mud.Wechat.MiniProgram");
        Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Should().NotContain(f => File.ReadAllText(f).Contains("ConfigureAllowedDomains", StringComparison.Ordinal),
                "产品线不得自行登记进程级白名单（整体替换语义会清空其它产品线）");
    }

    /// <summary>
    /// MP-X9：<b>智能接口共享面留在公众号线承载</b> —— 小程序线<b>不得</b>克隆 <c>IMpSmartApiService</c>
    /// 的 12 端点（OCR 7 + 图像处理 2 + AI 语音 3），同时公众号线<b>必须</b>继续声明全部 12 条路由。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>判据（2026-10-10 定夺）</b>：智能接口的<b>请求路径与报文形态在公众号与小程序两侧逐字相同</b>
    /// ——官方文档按产品线各出一份页面（<c>doc/offiaccount/Intelligent_Interface/OCR.html</c> ↔
    /// <c>miniprogram/dev/api-backend/open-api/ocr/ocr.idcard.html</c>），但落到线上是同一个 URI，
    /// 且令牌同域（小程序侧亦消费 <c> MpTokenTypes.AccessToken</c>）。既然路径<b>无任何前缀差异</b>，
    /// 克隆进小程序线即同时踩 MP-X1（跨线路由零重复）与 MP-X6（不得回潮公众号已有路由），
    /// 并制造「两份 DTO + 两个 JsonContext + 两套守卫」的纯重复维护。
    /// </para>
    /// <para>
    /// <b>与补齐方案的关系</b>：方案曾把「小程序 SmartApi」列为小额残口之一（拟 <c>24 → 36</c> 端点）。
    /// 本守卫即该拟议的<b>否决留档</b>：小程序侧调用智能接口请直接使用公众号线的
    /// <c>IMpSmartApiService</c>（同一 <c>api.weixin.qq.com</c>、同一令牌底座，<c>AddMpApp</c> 已是本线硬前置）。
    /// </para>
    /// <para><b>反静默绿</b>：同时断言公众号线仍声明这 12 条 —— 若 OA 侧被删，本守卫必须报红而不是「MP 侧也确实没有」。</para>
    /// </remarks>
    [Fact]
    public void SmartApiRoutes_ShouldStayOnOfficialAccountLine()
    {
        var sharedRoutes = new[]
        {
            "/cgi-bin/media/voice/addvoicetorecofortext",
            "/cgi-bin/media/voice/queryrecoresultfortext",
            "/cgi-bin/media/voice/translatecontent",
            "/cv/ocr/idcard",
            "/cv/ocr/bankcard",
            "/cv/ocr/driving",
            "/cv/ocr/drivinglicense",
            "/cv/ocr/bizlicense",
            "/cv/ocr/comm",
            "/cv/ocr/menu",
            "/cv/img/aicrop",
            "/cv/img/qrcode",
        };

        var mpRoutes = CollectAttributeRoutes(typeof(MiniProgramServiceBuilder).Assembly)
            .Concat(ManualChannelRoutes())
            .ToArray();

        mpRoutes.Should().NotIntersectWith(sharedRoutes,
            "智能接口 12 端点两侧路径逐字相同，克隆即踩 MP-X1 / MP-X6（本守卫为该拟议的否决留档）");

        var oaRoutes = CollectAttributeRoutes(
            typeof(Mud.Wechat.OfficialAccount.Extensions.MpServiceCollectionExtensions).Assembly);
        oaRoutes.Should().Contain(sharedRoutes,
            "共享面由公众号线承载：OA 侧缺席即意味着小程序用户失去该能力，本守卫不得静默放行");
    }

    /// <summary>装配面：未先装令牌底座时必须在<b>注册期</b>点名 fail-fast。</summary>
    [Fact]
    public void AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMiniProgramServices(b => b.AddAllApis());

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddMpApp*");
    }

    /// <summary>
    /// 装配面：<c>AddMpApp</c> + <c>AddMiniProgramServices(AddAllApis)</c> 后四域接口与图片通道均可解析。
    /// </summary>
    /// <remarks>
    /// 锁定「枚举成员 ↔ 注册器 ↔ 源生成 <c>Add{域}WebApiHttpClient()</c>」三段式同批成立；
    /// 也证明「小程序复用公众号令牌底座」这一步真的接通（<c>IMpAppManager</c> 是硬前置）。
    /// </remarks>
    [Fact]
    public void AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains()
    {
        var services = new ServiceCollection();
        services.AddMpApp(static c =>
        {
            c.AppKey = "mp-test";
            c.AppId = "wx-test";
            c.AppSecret = "secret-test";
        });

        services.AddMiniProgramServices(static b => b.AddAllApis());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });

        provider.GetRequiredService<IWxaAuthService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaCode2SessionService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaQrCodeLinkService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaSecurityService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaDataAnalysisService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaSubscribeMessageService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaDynamicMessageService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaKfService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaHardwareDeviceService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaOperationService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaPluginService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaChargeService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaNearbyPoiService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaSearchService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaSoterService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaServiceMarketService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaRedPacketCoverService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaStudentService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaFaceVerifyService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaLaborUseService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaCodeService>().Should().NotBeNull("小程序码图片通道随 QrCodeLink 模块注册");
        provider.GetRequiredService<IWxaFeedbackMediaService>().Should().NotBeNull("反馈图片通道随 Operation 模块注册");
        provider.GetRequiredService<IWxaKfMediaService>().Should().NotBeNull("客服临时素材二进制通道随 Kf 模块注册");
        provider.GetRequiredService<IWxaExpressBusinessService>().Should().NotBeNull("物流助手商家寄件随 Express 模块注册");
        provider.GetRequiredService<IWxaExpressDeliveryService>().Should().NotBeNull("物流助手服务商侧随 Express 模块注册");
        provider.GetRequiredService<IWxaExpressIntracityService>().Should().NotBeNull("物流助手同城配送随 Express 模块注册");
        provider.GetRequiredService<IWxaExpressInsuranceFreightService>().Should().NotBeNull("微信物流服务无忧退货随 Express 模块注册");
        provider.GetRequiredService<IWxaExpressLocalService>().Should().NotBeNull("即时配送随 Express 模块注册");
        provider.GetRequiredService<IWxaExpressBillService>().Should().NotBeNull("对账文件通道随 Express 模块注册");
        provider.GetRequiredService<IWxaVipService>().Should().NotBeNull("会员服务随 Vip 模块注册");
        provider.GetRequiredService<IWxaTradeGuaranteeService>().Should().NotBeNull("交易保障基础随 TransactionGuarantee 模块注册");
        provider.GetRequiredService<IWxaTradeCommentService>().Should().NotBeNull("评价管理随 TransactionGuarantee 模块注册");
        provider.GetRequiredService<IWxaTradeComplaintService>().Should().NotBeNull("投诉管理随 TransactionGuarantee 模块注册");

        // 直播域六接口共用 Live 注册组 ⇒ 一条生成入口装载全部 38 端点。
        provider.GetRequiredService<IWxaLiveRoomService>().Should().NotBeNull("直播间本体随 Live 模块注册");
        provider.GetRequiredService<IWxaLiveAssistantService>().Should().NotBeNull("小助手与主播副号随 Live 模块注册");
        provider.GetRequiredService<IWxaLiveRoomGoodsService>().Should().NotBeNull("房间内商品随 Live 模块注册");
        provider.GetRequiredService<IWxaLiveGoodsService>().Should().NotBeNull("商品库管理随 Live 模块注册");
        provider.GetRequiredService<IWxaLiveRoleService>().Should().NotBeNull("成员角色随 Live 模块注册");
        provider.GetRequiredService<IWxaLiveSubscribeService>().Should().NotBeNull("长期订阅群发随 Live 模块注册");

        // 虚拟支付域六接口共用 XPay 注册组 ⇒ 一条生成入口装载全部 37 端点（注册委托同时完成 pay_sig 登记，见守卫 XP6）。
        provider.GetRequiredService<IWxaXPayCurrencyService>().Should().NotBeNull("代币扣减与余额随 XPay 模块注册");
        provider.GetRequiredService<IWxaXPayOrderService>().Should().NotBeNull("订单与账单下载随 XPay 模块注册");
        provider.GetRequiredService<IWxaXPayGoodsService>().Should().NotBeNull("道具管理随 XPay 模块注册");
        provider.GetRequiredService<IWxaXPaySubscribeService>().Should().NotBeNull("会员订阅随 XPay 模块注册");
        provider.GetRequiredService<IWxaXPayFundsService>().Should().NotBeNull("资金与提现随 XPay 模块注册");
        provider.GetRequiredService<IWxaXPayComplaintService>().Should().NotBeNull("投诉与管控随 XPay 模块注册");

        // 发货信息与品牌申请域四接口共用 OrderShipping 注册组 ⇒ 一条生成入口装载全部 12 端点（守卫 OS2）。
        provider.GetRequiredService<IWxaOrderShippingService>().Should().NotBeNull("发货录入与报备随 OrderShipping 模块注册");
        provider.GetRequiredService<IWxaOrderQueryService>().Should().NotBeNull("订单与管理状态查询随 OrderShipping 模块注册");
        provider.GetRequiredService<IWxaOrderSettingService>().Should().NotBeNull("消息跳转路径与交易类型变更随 OrderShipping 模块注册");
        provider.GetRequiredService<IWxaFamousBrandService>().Should().NotBeNull("品牌申请随 OrderShipping 模块注册");

        // 小说作品管理域五接口共用 Novel 注册组 ⇒ 一条生成入口装载全部 23 端点。
        provider.GetRequiredService<IWxaNovelBookService>().Should().NotBeNull("作品本体随 Novel 模块注册");
        provider.GetRequiredService<IWxaNovelChapterService>().Should().NotBeNull("章节管理随 Novel 模块注册");
        provider.GetRequiredService<IWxaNovelAuthService>().Should().NotBeNull("授权管理随 Novel 模块注册");
        provider.GetRequiredService<IWxaNovelAccountAuthService>().Should().NotBeNull("账号授权随 Novel 模块注册");
        provider.GetRequiredService<IWxaNovelReaderService>().Should().NotBeNull("读者运营随 Novel 模块注册");

        // RealName 域（1 端点）
        provider.GetRequiredService<IWxaRealNameService>().Should().NotBeNull("实名认证随 RealName 模块注册");
    }

    // ---- helpers -------------------------------------------------------------

    private static string[] ManualChannelRoutes()
        => new[]
        {
            WxaCodeService.UnlimitedCodePath, WxaCodeService.CodePath, WxaCodeService.QrCodePath,
            WxaFeedbackMediaService.FeedbackMediaPath,
            WxaExpressBillService.BillPath,
            WxaKfMediaService.MediaPath,
        };

    private static string[] RoutesOf(Assembly asm, string interfaceName)
    {
        var type = asm.GetTypes().SingleOrDefault(t => t.Name == interfaceName);
        type.Should().NotBeNull($"未找到接口 {interfaceName}");

        return type!.GetMethods()
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .Select(static uri => uri!)
            .ToArray();
    }

    private static string[] CollectAttributeRoutes(Assembly asm)
        => asm.GetTypes()
            .SelectMany(static t => t.GetMethods())
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .Select(static uri => uri!)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttributes(false)
            .Where(static a => a.GetType().Name == "JsonPropertyNameAttribute")
            .Select(static a => a.GetType().GetProperty("Name")!.GetValue(a) as string)
            .SingleOrDefault();

        jsonName.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文");
    }

    private static string? QueryNameOf(ParameterInfo parameter)
        => parameter.GetCustomAttributes(false)
            .Where(static a => a.GetType().Name == "QueryAttribute")
            .Select(static a => a.GetType().GetProperty("Name")?.GetValue(a) as string)
            .FirstOrDefault();

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

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
