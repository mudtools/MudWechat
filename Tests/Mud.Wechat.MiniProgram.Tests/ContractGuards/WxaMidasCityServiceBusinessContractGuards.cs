// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.BusinessCapability;
using Mud.Wechat.MiniProgram.DataModels.CityService;
using Mud.Wechat.MiniProgram.DataModels.Midas;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「米大师虚拟支付（Midas）/ 城市服务（CityService）/ 行业能力（BusinessCapability）」三域契约守卫
/// （MCB 系列：18 端点路由表 + 令牌与注册三段式 + DTO 上下文登记 + 官方字段名 + 非契约零声明 + 敏感面红线）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三域同在 2026-10-18 批次的 224 条端点工作单内</b>：Midas 8（正式 4 + 沙箱 4）、
/// CityService 5（基础 3 + 适老化医疗 2）、BusinessCapability 5（B2b 门店 3 + WMPF 硬件 2），
/// 合计 18 条，全部 POST（含查询类端点，官方原文即 POST）。
/// </para>
/// <para>
/// <b>非契约面四支</b>：<c>/cityservice/face/identify/*</c> 三支只存在于 SKIT 派生清单，官方文档树无对应详情页
/// （零 URL 即零契约）；<c>/wxa/business/applyliveinfo</c> 属开放平台「代小程序管理」面
/// （<c>component_access_token</c> 令牌链），由开放平台线 <c>IWechatOpenPlatformMpLiveService</c> 承载，
/// 小程序线不得声明（否则令牌串号）。两支清单错配的教训见 <c>WxaLeaguePromoterContractGuards</c>。
/// </para>
/// <para>
/// <b>Midas 的签名与云调用边界</b>：<c>sig</c> 由调用方用米大师密钥对「全部参数 + uri + 密钥」做
/// HMAC-SHA256，SDK 不持有密钥、不代算；官方每页并列的云调用（<c>openapi.midas.*</c>）属微信云开发运行时，
/// 不产生 HTTP 路由。域内仅承载 8 条 HTTPS 路由。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：端点聚合计数（MP-X5 / ContractBaseline）、Query 令牌注入白名单（QT1）、
/// 跨线路由零重叠（MP-X1）由 <c>MiniProgramContractGuards</c> 与 <c>MpQueryTokenWhitelistGuard</c> 承担；
/// 本文件只锁三域内的精确契约。
/// </para>
/// </remarks>
public class WxaMidasCityServiceBusinessContractGuards
{
    private const string MidasRegistryGroupName = "Midas";
    private const string CityServiceRegistryGroupName = "CityService";
    private const string BusinessRegistryGroupName = "BusinessCapability";

    private const string MidasDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.Midas";
    private const string CityServiceDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.CityService";
    private const string BusinessDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.BusinessCapability";

    /// <summary>米大师虚拟支付域（8 端点：正式 4 + 沙箱 4，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] MidasRoutes =
    {
        (typeof(IWxaMidasService), nameof(IWxaMidasService.PayAsync), "/cgi-bin/midas/pay"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.CancelPayAsync), "/cgi-bin/midas/cancelpay"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.GetBalanceAsync), "/cgi-bin/midas/getbalance"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.PresentAsync), "/cgi-bin/midas/present"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.PayInSandboxAsync), "/cgi-bin/midas/sandbox/pay"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.CancelPayInSandboxAsync), "/cgi-bin/midas/sandbox/cancelpay"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.GetBalanceInSandboxAsync), "/cgi-bin/midas/sandbox/getbalance"),
        (typeof(IWxaMidasService), nameof(IWxaMidasService.PresentInSandboxAsync), "/cgi-bin/midas/sandbox/present"),
    };

    /// <summary>城市服务域（5 端点：基础 3 + 适老化医疗 2，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] CityServiceRoutes =
    {
        (typeof(IWxaCityServiceBasicService), nameof(IWxaCityServiceBasicService.GetServicePathAsync), "/cityservice/getservicepath"),
        (typeof(IWxaCityServiceBasicService), nameof(IWxaCityServiceBasicService.SendChannelMsgAsync), "/cityservice/sendchannelmsg"),
        (typeof(IWxaCityServiceBasicService), nameof(IWxaCityServiceBasicService.SendMsgDataAsync), "/cityservice/sendmsgdata"),
        (typeof(IWxaCityServiceElderMedicalService), nameof(IWxaCityServiceElderMedicalService.GetMedRealNameAsync), "/cityservice/getmedrealname"),
        (typeof(IWxaCityServiceElderMedicalService), nameof(IWxaCityServiceElderMedicalService.GetMsgRelationAsync), "/cityservice/getmsgrelation"),
    };

    /// <summary>行业能力域（5 端点：B2b 门店 3 + WMPF 硬件 2，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] BusinessRoutes =
    {
        (typeof(IWxaBusinessRetailService), nameof(IWxaBusinessRetailService.BatchCreateRetailAsync), "/wxa/business/batchcreateretail"),
        (typeof(IWxaBusinessRetailService), nameof(IWxaBusinessRetailService.GetRetailInfoAsync), "/wxa/business/getretailinfo"),
        (typeof(IWxaBusinessRetailService), nameof(IWxaBusinessRetailService.RetailNotifyBusinessAsync), "/wxa/business/retailnotifybusiness"),
        (typeof(IWxaBusinessRuntimeService), nameof(IWxaBusinessRuntimeService.AddDeviceAsync), "/wxa/business/runtime/adddevice"),
        (typeof(IWxaBusinessRuntimeService), nameof(IWxaBusinessRuntimeService.PushAppMsgAsync), "/wxa/business/runtime/pushappmsg"),
    };

    /// <summary>官方回包只有 errcode/errmsg 的四支端点（返回 WxaResponse，不得为空壳造派生 DTO）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] BareEnvelopeEndpoints =
    {
        (typeof(IWxaCityServiceBasicService), nameof(IWxaCityServiceBasicService.SendChannelMsgAsync), "/cityservice/sendchannelmsg"),
        (typeof(IWxaBusinessRetailService), nameof(IWxaBusinessRetailService.RetailNotifyBusinessAsync), "/wxa/business/retailnotifybusiness"),
        (typeof(IWxaBusinessRuntimeService), nameof(IWxaBusinessRuntimeService.AddDeviceAsync), "/wxa/business/runtime/adddevice"),
        (typeof(IWxaBusinessRuntimeService), nameof(IWxaBusinessRuntimeService.PushAppMsgAsync), "/wxa/business/runtime/pushappmsg"),
    };

    /// <summary>
    /// MCB-Z1：本批次判非契约的四条路由（城市服务人脸识别 3 + 开放平台归属 1）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 仅 SKIT 派生清单在册：官方文档树无任何 /cityservice/face/* 详情页（出处 URL 为空）。
        "/cityservice/face/identify/getimage",
        "/cityservice/face/identify/getinfo",
        "/cityservice/face/identify/getuseridkey",
        // 令牌面为 component_access_token，属开放平台线「代小程序管理」（applyLivelnfo）。
        "/wxa/business/applyliveinfo",
    };

    // ------------------------------------------------------------------
    // MCB1：18 端点路由表必须与官方契约逐字一致（全部 POST）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 MCB1：三域 18 端点路由表与官方请求地址逐字一致，全域 POST。</summary>
    [Fact]
    public void McbEndpoints_ShouldMatchOfficialRoutes()
    {
        var all = MidasRoutes.Concat(CityServiceRoutes).Concat(BusinessRoutes).ToArray();

        all.Should().HaveCount(18,
            "本批次三域共 18 端点（Midas 8 + CityService 5 + BusinessCapability 5，2026-10-18 逐页取证）");
        all.Select(static r => r.Route).Distinct(StringComparer.Ordinal).Should().HaveCount(18,
            "路由表不得出现重复条目");

        foreach (var (iface, method, route) in all)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 官方调用方式恒为 POST（本批次无 GET 端点）");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须照官方「请求地址」原文，禁止改写");
        }

        // 前缀分布：Midas 8（/cgi-bin/midas/）+ CityService 5（/cityservice/）+ Business 5（/wxa/business/）。
        all.Count(static r => r.Route.StartsWith("/cgi-bin/midas/", StringComparison.Ordinal)).Should().Be(8,
            "米大师全域落在 /cgi-bin/midas/（含 sandbox 子族 4 支）");
        all.Count(static r => r.Route.StartsWith("/cityservice/", StringComparison.Ordinal)).Should().Be(5,
            "城市服务全域落在 /cityservice/（基础 3 + 适老化医疗 2）");
        all.Count(static r => r.Route.StartsWith("/wxa/business/", StringComparison.Ordinal)).Should().Be(5,
            "行业能力全域落在 /wxa/business/（B2b 门店 3 + WMPF runtime 2）");

        // 沙箱是独立环境：4 支 sandbox 路由与正式 4 支成对存在，不得只落其一。
        var sandbox = all.Where(static r => r.Route.Contains("/sandbox/", StringComparison.Ordinal))
            .Select(static r => r.Route["/cgi-bin/midas/sandbox".Length..])
            .OrderBy(static r => r, StringComparer.Ordinal)
            .ToList();
        sandbox.Should().Equal(new[] { "/cancelpay", "/getbalance", "/pay", "/present" },
            "沙箱 4 支必须与正式环境同契约成对（仅联调用途，数据隔离）");
    }

    // ------------------------------------------------------------------
    // MCB2：令牌绑定 + 注册三段式（5 接口 / 3 注册组 / 3 模块 / 3 建造者方法）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 MCB2：五接口复用小程序 access_token（Query），按域共用三个注册组。</summary>
    [Theory]
    [InlineData(typeof(IWxaMidasService), MidasRegistryGroupName)]
    [InlineData(typeof(IWxaCityServiceBasicService), CityServiceRegistryGroupName)]
    [InlineData(typeof(IWxaCityServiceElderMedicalService), CityServiceRegistryGroupName)]
    [InlineData(typeof(IWxaBusinessRetailService), BusinessRegistryGroupName)]
    [InlineData(typeof(IWxaBusinessRuntimeService), BusinessRegistryGroupName)]
    public void McbInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup(Type interfaceType, string expectedGroup)
    {
        var token = interfaceType.GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull($"{interfaceType.Name} 必须声明接口级 [Token]（本批次全部端点消费小程序 access_token）");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
            "三域均复用公众号/小程序同域令牌 Wechat.Mp.AccessToken（不得新增令牌类型）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3 / QT1）");
        token.Name.Should().Be("access_token", "Query 参数名须照官方原文");

        var api = interfaceType.GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull($"{interfaceType.Name} 必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(expectedGroup, $"{interfaceType.Name} 的注册组必须为 {expectedGroup}");
        api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
    }

    /// <summary>契约守卫 MCB2b：三段式注册面 —— 3 个模块枚举成员 ↔ 3 个 <c>Add…Api()</c> 同批成立。</summary>
    [Fact]
    public void McbModules_ShouldBeRegisteredInBuilder()
    {
        var moduleNames = Enum.GetNames<MiniProgramModule>();
        moduleNames.Should().Contain(new[] { "Midas", "CityService", "BusinessCapability" },
            "三个域必须有模块枚举成员（模块枚举 ↔ 注册入口 ↔ 源生成注册组三段式）");

        var builderMethods = typeof(MiniProgramServiceBuilder)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static m => m.Name)
            .ToList();
        builderMethods.Should().Contain(new[] { "AddMidasApi", "AddCityServiceApi", "AddBusinessCapabilityApi" },
            "三个建造者入口必须存在，否则各域端点无装配入口");

        // 正对照：枚举与建造者的扫描口径必须能命中已落位域（Contain 断言才不是空转）。
        moduleNames.Should().Contain("ShopSpu", "MCB2b 的正对照：枚举成员扫描口径有效");
        builderMethods.Should().Contain(static n => n == "AddAuthApi", "MCB2b 的正对照：建造者方法扫描口径有效");
    }

    // ------------------------------------------------------------------
    // MCB3：DTO 全量登记进 AOT JSON 上下文（8 / 12 / 11，含常量类清单锁死）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 MCB3：三域 DTO 全量登记进各自 JsonContext，命名空间类型清单锁死。</summary>
    [Theory]
    [InlineData(MidasDtoNamespace, MidasRegistryGroupName, 8, 2, "MidasJsonContext")]
    [InlineData(CityServiceDtoNamespace, CityServiceRegistryGroupName, 12, 4, "CityServiceJsonContext")]
    [InlineData(BusinessDtoNamespace, BusinessRegistryGroupName, 11, 0, "BusinessCapabilityJsonContext")]
    public void McbDataModels_ShouldBeRegisteredInJsonContext(
        string dtoNamespace, string serializerClassName, int expectedDtoCount, int expectedConstantCount, string contextName)
    {
        var assembly = typeof(WxaMidasPayRequest).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(t => !t.IsNested && t.Namespace == dtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty($"{dtoNamespace} 非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(expectedDtoCount,
            $"{serializerClassName} 域 DTO 总数漂移须先核对官方参数表/响应表再同批调整本守卫");

        var constantTypes = domainTypes
            .Where(static t => t.IsAbstract && t.IsSealed)
            .ToList();
        constantTypes.Should().HaveCount(expectedConstantCount,
            $"{serializerClassName} 域的常量类清单已锁死（Midas 2 / CityService 4 / BusinessCapability 0）；新增须同批更新本守卫");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(serializerClassName, $"{type.Name} 的 SerializerClassName 必须为 {serializerClassName}");
        }

        var contextType = assembly.GetType(dtoNamespace + "." + contextName);
        contextType.Should().NotBeNull(
            $"{contextName} 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"{serializerClassName}\")]，必须登记进 {contextName}" +
                "（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：除 DTO、常量类与生成上下文外不得出现新类型。
        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && !constantTypes.Contains(t) && t != contextType)
            .ToList();
        unregistered.Should().BeEmpty(
            $"{serializerClassName} 域出现既非 [HttpJsonSerializable] 登记类型、也非常量类/生成上下文的新类型：" +
            string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 须同批更新本守卫");

        // 响应 DTO 一律继承 WxaResponse（统一错误信封），而裸信封端点直接返回 WxaResponse。
        foreach (var type in dtoTypes.Where(static t => t.Name.EndsWith("Response", StringComparison.Ordinal)))
        {
            type.IsAssignableTo(typeof(WxaResponse)).Should().BeTrue($"{type.Name} 必须继承 WxaResponse");
        }
    }

    /// <summary>契约守卫 MCB3b：官方回包只有 errcode/errmsg 的四支直接返回 <see cref="WxaResponse"/>（不造空壳 DTO）。</summary>
    [Fact]
    public void BareEnvelopeEndpoints_ShouldReturnWxaResponseDirectly()
    {
        BareEnvelopeEndpoints.Should().HaveCount(4,
            "城市服务 sendchannelmsg + B2b 门店消息 + WMPF adddevice/pushappmsg：官方回包仅 errcode/errmsg");

        foreach (var (iface, method, route) in BareEnvelopeEndpoints)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.ReturnType.Should().Be(typeof(Task<WxaResponse>),
                $"{route} 的回包仅统一错误信封 ⇒ 必须返回 Task<WxaResponse>，不得为空壳造派生响应 DTO");
        }
    }

    // ------------------------------------------------------------------
    // MCB4：官方字段名抽样（含反直觉拼写与数值类型口径）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 MCB4：官方字段名抽样 —— Midas 短名、CityService 反直觉拼写、Business 数组字段。</summary>
    [Fact]
    public void McbDataModels_ShouldLockOfficialFieldNames()
    {
        // Midas 短名族：ts / pf / amt / pay_item / app_remark —— 不得按属性名机械下划线化。
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.OpenId), "openid", "官方参数表原文（无下划线）");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.AppId), "appid", "官方参数表原文（无下划线）");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.Timestamp), "ts", "官方参数表原文（短名）");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.Platform), "pf", "官方参数表原文（短名）");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.UserIp), "user_ip", "官方参数表原文");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.Amount), "amt", "官方参数表原文（短名）");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.BillNo), "bill_no", "官方参数表原文（幂等键）");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.PayItem), "pay_item", "官方参数表原文");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.AppRemark), "app_remark", "官方参数表原文");
        JsonNameShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.Sig), "sig", "官方参数表原文（调用方计算后透传，SDK 不代算）");
        JsonNameShouldBe<WxaMidasGetBalanceResponse>(nameof(WxaMidasGetBalanceResponse.SaveAmount), "save_amt", "官方响应表原文（不是 save_amount）");
        JsonNameShouldBe<WxaMidasGetBalanceResponse>(nameof(WxaMidasGetBalanceResponse.GenBalance), "gen_balance", "官方响应表原文");
        JsonNameShouldBe<WxaMidasPayResponse>(nameof(WxaMidasPayResponse.UsedGenAmount), "used_gen_amt", "官方响应表原文");
        JsonNameShouldBe<WxaMidasPresentRequest>(nameof(WxaMidasPresentRequest.PresentCounts), "present_counts", "官方参数表原文");

        // CityService 反直觉拼写：fullpath（无下划线）与 business_type（不是回包示例的 bussiness_type）。
        JsonNameShouldBe<WxaCityServiceRedirectPage>(nameof(WxaCityServiceRedirectPage.FullPath), "fullpath",
            "官方参数表原文（单字连写，非 full_path；回包示例的 bussiness_type 是官方笔误，按参数表建模）");
        JsonNameShouldBe<WxaCityServiceGetServicePathResponse>(nameof(WxaCityServiceGetServicePathResponse.BusinessType), "business_type",
            "官方参数表原文（回包示例拼作 bussiness_type，属示例缺陷，不覆盖字段表）");
        JsonNameShouldBe<WxaCityServiceGetServicePathResponse>(nameof(WxaCityServiceGetServicePathResponse.UserName), "username",
            "官方响应表原文（username 单字，非 user_name）");
        JsonNameShouldBe<WxaCityServiceGetMedRealNameRequest>(nameof(WxaCityServiceGetMedRealNameRequest.WxmedAuthcode), "wxmed_authcode",
            "官方参数表原文（10 分钟有效，SDK 不缓存不重放）");
        JsonNameShouldBe<WxaCityServiceGetMedRealNameResponse>(nameof(WxaCityServiceGetMedRealNameResponse.CipherRealName), "cipher_real_name",
            "官方响应表原文（密文，SDK 不持有密钥、不解密）");
        JsonNameShouldBe<WxaCityServiceSendChannelMsgRequest>(nameof(WxaCityServiceSendChannelMsgRequest.MsgId), "msg_id", "官方参数表原文");
        JsonNameShouldBe<WxaCityServiceSendMsgDataRequest>(nameof(WxaCityServiceSendMsgDataRequest.BizTemplateId), "biz_template_id", "官方参数表原文");
        JsonNameShouldBe<WxaCityServiceSendMsgDataResponse>(nameof(WxaCityServiceSendMsgDataResponse.ResultPageUrl), "result_page_url", "官方响应表原文");
        JsonNameShouldBe<WxaCityServiceGetMsgRelationResponse>(nameof(WxaCityServiceGetMsgRelationResponse.IsSubscribed), "is_subscribed", "官方响应表原文");

        // Business 复数数组族。
        JsonNameShouldBe<WxaBusinessBatchCreateRetailRequest>(nameof(WxaBusinessBatchCreateRetailRequest.RetailInfoList), "retail_info_list", "官方参数表原文");
        JsonNameShouldBe<WxaBusinessBatchCreateRetailResponse>(nameof(WxaBusinessBatchCreateRetailResponse.NumFailure), "num_failure", "官方响应表原文");
        JsonNameShouldBe<WxaBusinessBatchCreateRetailResponse>(nameof(WxaBusinessBatchCreateRetailResponse.FailureRecordList), "failure_record_list", "官方响应表原文");
        JsonNameShouldBe<WxaBusinessGetRetailInfoResponse>(nameof(WxaBusinessGetRetailInfoResponse.Info), "info", "官方响应示例结构（字段表平铺、示例包裹在 info 数组，按示例建模）");
        JsonNameShouldBe<WxaBusinessRetailInfo>(nameof(WxaBusinessRetailInfo.StaffList), "staff_list", "官方响应表原文");
        JsonNameShouldBe<WxaBusinessRetailInfo>(nameof(WxaBusinessRetailInfo.LegalPersonName), "legal_person_name", "官方响应表原文");
        JsonNameShouldBe<WxaBusinessRetailNotifyBusinessRequest>(nameof(WxaBusinessRetailNotifyBusinessRequest.ToUserList), "to_user_list", "官方参数表原文（接收人上限 200）");
        JsonNameShouldBe<WxaBusinessRuntimeAddDeviceRequest>(nameof(WxaBusinessRuntimeAddDeviceRequest.DeviceIdList), "device_id_list", "官方参数表原文");
        JsonNameShouldBe<WxaBusinessRuntimePushAppMsgRequest>(nameof(WxaBusinessRuntimePushAppMsgRequest.PushToken), "push_token", "官方参数表原文（设备凭据）");

        // 数值类型口径：金额/余额/流水为定点 long；业务编号与计数为 int。
        PropertyTypeShouldBe<WxaMidasPayRequest>(nameof(WxaMidasPayRequest.Amount), typeof(long?));
        PropertyTypeShouldBe<WxaMidasGetBalanceResponse>(nameof(WxaMidasGetBalanceResponse.Balance), typeof(long?));
        PropertyTypeShouldBe<WxaMidasGetBalanceResponse>(nameof(WxaMidasGetBalanceResponse.CostSum), typeof(long?));
        PropertyTypeShouldBe<WxaCityServiceGetServicePathRequest>(nameof(WxaCityServiceGetServicePathRequest.ServiceId), typeof(long?));
        PropertyTypeShouldBe<WxaCityServiceGetMsgRelationRequest>(nameof(WxaCityServiceGetMsgRelationRequest.BusinessId), typeof(int?));
        PropertyTypeShouldBe<WxaBusinessGetRetailInfoRequest>(nameof(WxaBusinessGetRetailInfoRequest.OpenId), typeof(string));
        PropertyTypeShouldBe<WxaBusinessRetailInfo>(nameof(WxaBusinessRetailInfo.Latitude), typeof(double?));
        PropertyTypeShouldBe<WxaBusinessRuntimeAddDeviceRequest>(nameof(WxaBusinessRuntimeAddDeviceRequest.ProductId), typeof(long?));

        // 模板变量对象由 SDK 透传：data 必须是 JsonElement?，不得臆造具体模板结构。
        typeof(WxaCityServiceSendMsgDataRequest)
            .GetProperty(nameof(WxaCityServiceSendMsgDataRequest.Data))!
            .PropertyType.Should().Be(typeof(JsonElement?),
                "官方 data 结构随 biz_template_id 变化 ⇒ 以 JsonElement 透传，不做本地结构校验");

        // Midas 签名不代算：请求 DTO 的 sig 是可空 string（由调用方填值透传），SDK 不提供签名方法。
        typeof(WxaMidasPayRequest).GetProperty(nameof(WxaMidasPayRequest.Sig))!
            .PropertyType.Should().Be(typeof(string), "sig 由调用方计算后透传（SDK 不持有米大师密钥，不代算）");
    }

    // ------------------------------------------------------------------
    // MCB-Z：四条非契约路由在小程序线零声明（正对照含同前缀在册路由）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 MCB-Z1：城市服务人脸识别 3 支 + 开放平台归属 1 支在小程序线零声明。</summary>
    [Fact]
    public void NonContractRoutes_ShouldStayUndeclared_InMiniProgramLine()
    {
        NonContractRoutes.Should().HaveCount(4,
            "人脸识别 3（仅 SKIT 清单、无官方详情页）+ applyliveinfo 1（开放平台令牌面）；清单增删须先复核官方页");
        NonContractRoutes.Distinct(StringComparer.Ordinal).Should().HaveCount(4, "清单内路由不得重复");

        var declared = DeclaredRoutes(Path.Combine("Src", "MiniProgram"));

        // 正对照：同前缀的已落位路由必须被扫到，否则「零声明」静默恒真。
        foreach (var control in new[] { "/cityservice/getservicepath", "/wxa/business/batchcreateretail", "/cgi-bin/midas/pay" })
        {
            declared.Should().Contain(control,
                "MCB-Z1 的扫描必须能捕获小程序线已声明的同前缀相邻路由（否则零声明断言静默恒真）");
        }

        var offenders = declared.Intersect(NonContractRoutes, StringComparer.Ordinal).ToList();
        offenders.Should().BeEmpty(
            "四条路由不属小程序线契约：/cityservice/face/identify/* 无官方详情页（零 URL 即零契约）；"
            + "/wxa/business/applyliveinfo 的令牌面是 component_access_token，归开放平台线（MP-X1 跨线不交叉）");

        // 归属自证：applyliveinfo 确实已在开放平台线在册（分工事实基础，而非两边都没落的悬空结论）。
        DeclaredRoutes(Path.Combine("Src", "OpenPlatform"))
            .Should().Contain("/wxa/business/applyliveinfo",
                "applyliveinfo 必须由开放平台线「代小程序管理」承载（component_access_token 令牌链）");
    }

    // ------------------------------------------------------------------
    // MCB5：敏感面红线（入参 DTO 不重写 ToString；域内源文件真实日志调用不携带敏感名）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 MCB5：承载支付凭据/个人信息的 DTO 不得重写 <c>ToString</c>（防结构化日志整体打印）。</summary>
    [Theory]
    [InlineData(typeof(WxaMidasPayRequest))]
    [InlineData(typeof(WxaMidasCancelPayRequest))]
    [InlineData(typeof(WxaMidasPresentRequest))]
    [InlineData(typeof(WxaCityServiceGetMedRealNameRequest))]
    [InlineData(typeof(WxaCityServiceGetMedRealNameResponse))]
    [InlineData(typeof(WxaBusinessRetailInfo))]
    [InlineData(typeof(WxaBusinessRuntimePushAppMsgRequest))]
    public void SensitiveDataModels_ShouldNotOverrideToString(Type dataType)
    {
        dataType.GetMethod("ToString", Type.EmptyTypes)!.DeclaringType
            .Should().Be(typeof(object),
                $"{dataType.Name} 承载 openid / sig / bill_no / user_ip / wxmed_authcode / 密文姓名 / 手机号 / push_token 等敏感面，" +
                "不得重写 ToString（一旦被 Log(\"{{Dto}}\") 即整体泄露，AGENTS §8）");
    }

    /// <summary>契约守卫 MCB5b：三域源文件的真实日志调用不得携带敏感字段名。</summary>
    [Fact]
    public void McbDomainSourceFiles_ShouldNotLogSensitiveFields()
    {
        var sensitiveNames = new[]
        {
            "OpenId", "Openid", "Sig", "BillNo", "UserIp", "WxmedAuthcode",
            "CipherRealName", "MobilePhone", "PushToken", "RegistrationNumber", "LegalPersonName", "PatName",
        };

        var root = SourceProjectDir("Mud.Wechat.MiniProgram");
        var files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => Path.GetFileName(f).StartsWith("IWxaMidas", StringComparison.Ordinal)
                               || Path.GetFileName(f).StartsWith("IWxaCityService", StringComparison.Ordinal)
                               || Path.GetFileName(f).StartsWith("IWxaBusiness", StringComparison.Ordinal))
            .ToList();

        files.Should().HaveCountGreaterThanOrEqualTo(5,
            "扫描根必须覆盖三域五个接口源文件（定位失效会让 offenders 恒空）");

        var logInvocation = new Regex(@"\bLog(?:Information|Warning|Error|Debug|Trace|Critical)?\s*\(",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        var offenders = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var code = StripLineComment(lines[i]);
                if (!logInvocation.IsMatch(code))
                {
                    continue;
                }

                var statement = string.Concat(
                    code,
                    i + 1 < lines.Length ? StripLineComment(lines[i + 1]) : string.Empty,
                    i + 2 < lines.Length ? StripLineComment(lines[i + 2]) : string.Empty);
                var hit = sensitiveNames.FirstOrDefault(p => statement.Contains(p, StringComparison.Ordinal));
                if (hit is not null)
                {
                    offenders.Add($"{Path.GetFileName(file)}:{i + 1}({hit})");
                }
            }
        }

        offenders.Should().BeEmpty(
            "米大师支付凭据、医保实名密文、门店手机号/信用代码、WMPF push_token 等一律禁入日志、遥测与异常消息（AGENTS §8）");
    }

    // ---- helpers -------------------------------------------------------------

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void PropertyTypeShouldBe<T>(string propertyName, Type expectedType)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType, $"{typeof(T).Name}.{propertyName} 的类型须照官方参数表（{expectedType.Name}）");
    }

    /// <summary>剥行内与整行注释（<c>///</c> 亦经此路归空）：裁决注释会原样写出敏感名，不排除即假红。</summary>
    private static string StripLineComment(string line)
    {
        var quote = 0;
        for (var i = 0; i < line.Length - 1; i++)
        {
            if (line[i] == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                quote ^= 1;
            }
            else if (quote == 0 && line[i] == '/' && line[i + 1] == '/')
            {
                return line[..i];
            }
        }

        return line;
    }

    /// <summary>
    /// 扫描根下全部「已声明路由」：剥离注释后取引号包裹的路径字面量（HTTP 特性与路由常量一并覆盖）。
    /// </summary>
    private static HashSet<string> DeclaredRoutes(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
        Directory.Exists(root).Should().BeTrue($"扫描根必须存在（{root}）——目录漂移会让零声明断言静默恒真");

        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./{}]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal))
            {
                continue;
            }

            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (System.Text.RegularExpressions.Match m in literalRegex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        set.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：扫描根必须能扫出已声明路由");
        return set;
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

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(GetSolutionRoot(), $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
